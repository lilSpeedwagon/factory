// ReSharper disable CppClangTidyClangDiagnosticMicrosoftCast
#include "pch.h"
#include "Operations.h"
#include "Definitions.h"
#include "FileHelpers.h"


ExpressionPtr getExpressionByType(ExpressionType t)
{
	switch (t)
	{
	case ETZeroArgExpression:
		return std::make_shared<ZeroArgsExpression>();
	case ETUnaryExpression:
		return std::make_shared<UnaryExpression>();
	case ETBinaryExpression:
		return std::make_shared<BinaryExpression>();
	case ETIdentifierExpression:
		return std::make_shared<IdentifierExpression>();
	case ETValueExpression:
		return std::make_shared<ValueExpression>();
	default:
		throw serializer::SerializationError(serializer::msg_corruptedData);
	}
}


// OperationScope
void OperationScope::Execute()
{
	for (auto op : m_listOperations)
	{
		op->Execute();
	}
}

runtime::Value OperationScope::GetVariableValue(std::string const& varName) const
{
	const OperationScope* pScope = this;
	do
	{
		auto it = pScope->m_mapVariables.find(varName);
		if (it != pScope->m_mapVariables.end())
		{
			return it->second;
		}

		auto itStatic = pScope->m_mapStaticVariables.find(varName);
		if (itStatic != pScope->m_mapStaticVariables.end())
		{
			return itStatic->second;
		}
		
		pScope = pScope->GetParentScope();
	} while (pScope != nullptr);

	throw runtime::RuntimeException("Unknown identifier " + varName);
}

void OperationScope::SetVariableValue(std::string const& varName, runtime::Value const& val)
{
	// trying to find variable in parent scopes
	OperationScope* pScope = this;
	do
	{
		auto it = pScope->m_mapVariables.find(varName);
		if (it != pScope->m_mapVariables.end())
		{
			pScope->m_mapVariables[varName] = val;
			return;
		}

		auto itStatic = pScope->m_mapStaticVariables.find(varName);
		if (itStatic != pScope->m_mapStaticVariables.end())
		{
			pScope->m_mapStaticVariables[varName] = val;
			return;
		}
		
		pScope = pScope->GetParentScope();
	} while (pScope != nullptr);

	// if variable doesn't exist - create it in current scope
	m_mapVariables[varName] = val;
}

bool OperationScope::IsVariableExist(std::string const& varName) const
{
	const OperationScope* pScope = this;
	do
	{
		auto it = pScope->m_mapVariables.find(varName);
		if (it != pScope->m_mapVariables.end())
		{
			return true;
		}

		auto itStatic = pScope->m_mapStaticVariables.find(varName);
		if (itStatic != pScope->m_mapStaticVariables.end())
		{
			return true;
		}
		
		pScope = pScope->GetParentScope();
	} while (pScope != nullptr);

	return false;
}

void OperationScope::AddStaticVariable(std::string const& name)
{
	m_mapStaticVariables[name] = runtime::Value();
}

void OperationScope::AddOperation(OperationPtr pOperation)
{
	m_listOperations.push_back(pOperation);
}

void OperationScope::Reset()
{
	m_mapVariables.clear();
	for (OperationPtr operation : m_listOperations)
	{
		operation->Reset();
	}
}

serializer::BinaryFile& OperationScope::operator<<(serializer::BinaryFile& file)
{
	uint16_t operationsCount = 0;
	file >> operationsCount;

	for (uint16_t i = 0; i < operationsCount; i++)
	{
		uint8_t raw_type;
		file >> raw_type;
		const OperationType type = static_cast<OperationType>(raw_type);

		OperationPtr pOper;
		switch (type)
		{
		case OTScope:
			pOper = std::make_shared<OperationScope>();
			break;
		case OTAssign:
			pOper = std::make_shared<OperationAssign>();
			break;
		case OTOpExpression:
			pOper = std::make_shared<OperationExpression>();
			break;
		case OTControlFlow:
			pOper = std::make_shared<OperationControlFlow>();
			break;
		default:
			throw serializer::SerializationError(serializer::msg_corruptedData);
		}

		pOper->operator<<(file);
		m_listOperations.push_back(pOper);
	}

	uint16_t staticVariablesCount = 0;
	file >> staticVariablesCount;

	for (uint16_t i = 0; i < staticVariablesCount; i++)
	{
		std::string name;
		file >> name;
		m_mapStaticVariables[name] = runtime::Value();
	}

	return file;
}

serializer::BinaryFile& OperationScope::operator>>(serializer::BinaryFile& file)
{
	file << raw_type();	
	const uint16_t operationsCount = static_cast<uint16_t>(m_listOperations.size());
	file << operationsCount;

	for (OperationPtr pOper : m_listOperations)
	{
		pOper->operator>>(file);
	}

	const uint16_t staticVariablesCount = static_cast <uint16_t> (m_mapStaticVariables.size());
	file << staticVariablesCount;

	for (auto const& [name, value] : m_mapStaticVariables)
	{
		file << name;
	}

	return file;
}

void OperationScope::SetParentScope(OperationScope* pScope)
{
	if (pScope != this)
	{
		m_pParentScope = pScope;
	}
	
	for (OperationPtr pOper : m_listOperations)
	{
		pOper->SetParentScope(this);
	}
}
// OperationScope end



// OperatorAssign
void OperationAssign::Execute()
{
	m_pParentScope->SetVariableValue(m_pIdentifier->GetName(), m_pExpression->Calculate());
}

serializer::BinaryFile& OperationAssign::operator<<(serializer::BinaryFile& file)
{
	uint8_t raw_type = 0;
	file >> raw_type;
	if (static_cast<ExpressionType>(raw_type) != ETIdentifierExpression)
		throw serializer::SerializationError(serializer::msg_corruptedData);
	m_pIdentifier = std::make_shared<IdentifierExpression>();
	m_pIdentifier->operator<<(file);

	uint8_t raw_exprType = 0;
	file >> raw_exprType;
	m_pExpression = getExpressionByType(static_cast<ExpressionType>(raw_exprType));
	m_pExpression->operator<<(file);
	return file;
}

serializer::BinaryFile& OperationAssign::operator>>(serializer::BinaryFile& file)
{
	file << raw_type();
	m_pIdentifier->operator>>(file);
	m_pExpression->operator>>(file);
	return file;
}

void OperationAssign::SetParentScope(OperationScope* pScope)
{
	m_pParentScope = pScope;
	m_pIdentifier->SetScope(pScope);
	m_pExpression->SetScope(pScope);
}
// OperatorAssign end


// OperationControlFlow
void OperationControlFlow::Execute()
{
	if (!m_isLoop)
	{
		runtime::Value condition = m_pCondition->Calculate();
		if (condition.toBool().getValue<bool>() == true)
		{
			m_pScopeIfTrue->Execute();
		}
		else if (m_pScopeElse != nullptr)
		{
			m_pScopeElse->Execute();
		}
	}
	else
	{
		while(m_pCondition->Calculate().toBool().getValue<bool>() == true)
		{
			m_pScopeIfTrue->Execute();
		}
	}
}

serializer::BinaryFile& OperationControlFlow::operator<<(serializer::BinaryFile& file)
{
	uint8_t rawLoop = 0;
	file >> rawLoop;
	m_isLoop = U8toBool(rawLoop);
	uint8_t rawElse = 0;
	file >> rawElse;
	const bool hasElseBlock = U8toBool(rawElse);
	uint8_t raw_conditionType = 0;
	file >> raw_conditionType;
	
	m_pCondition = getExpressionByType(static_cast<ExpressionType>(raw_conditionType));
	m_pCondition->operator<<(file);

	uint8_t raw_trueType = 0;
	file >> raw_trueType;
	if (static_cast<OperationType>(raw_trueType) != OTScope)
		throw serializer::SerializationError(serializer::msg_corruptedData);
	m_pScopeIfTrue = std::make_shared<OperationScope>();
	m_pScopeIfTrue->operator<<(file);
	
	if (hasElseBlock)
	{
		uint8_t raw_elseType = 0;
		file >> raw_elseType;
		if (static_cast<OperationType>(raw_elseType) != OTScope)
			throw serializer::SerializationError(serializer::msg_corruptedData);
		m_pScopeElse = std::make_shared<OperationScope>();
		m_pScopeElse->operator<<(file);
	}
	else
	{
		m_pScopeElse = nullptr;
	}
	return file;
}

serializer::BinaryFile& OperationControlFlow::operator>>(serializer::BinaryFile& file)
{
	file << raw_type();
	file << boolToU8(m_isLoop);
	const bool hasElseBlock = m_pScopeElse != nullptr;
	file << boolToU8(hasElseBlock);
	m_pCondition->operator>>(file);
	m_pScopeIfTrue->operator>>(file);
	if (hasElseBlock)
	{
		m_pScopeElse->operator>>(file);
	}
	return file;
}

void OperationControlFlow::SetParentScope(OperationScope* pScope)
{
	m_pParentScope = pScope;
	m_pCondition->SetScope(pScope);
	m_pScopeIfTrue-> SetParentScope(pScope);
	if (m_pScopeElse != nullptr)
	{
		m_pScopeElse->SetParentScope(pScope);
	}
}
// OperationControlFlow end


// OperationExpression
void OperationExpression::Execute()
{
	m_pExpr->Calculate();
}

serializer::BinaryFile& OperationExpression::operator<<(serializer::BinaryFile& file)
{
	uint8_t raw_type = 0;
	file >> raw_type;
	m_pExpr = getExpressionByType(static_cast<ExpressionType>(raw_type));
	m_pExpr->operator<<(file);
	return file;
}

serializer::BinaryFile& OperationExpression::operator>>(serializer::BinaryFile& file)
{
	file << raw_type();
	m_pExpr->operator>>(file);
	return file;
}

void OperationExpression::SetParentScope(OperationScope* pScope)
{
	m_pParentScope = pScope;
	m_pExpr->SetScope(pScope);
}
// OperationExpression end


// ZeroArgsExpression
runtime::Value ZeroArgsExpression::Calculate()
{
	return m_function();
}

serializer::BinaryFile& ZeroArgsExpression::operator<<(serializer::BinaryFile& file)
{
	std::string funcName;
	file >> funcName;
	m_function = runtime::findZeroArgsFunction(funcName);
	if (m_function == nullptr)
		throw serializer::SerializationError(serializer::msg_corruptedData);
	return file;
}

serializer::BinaryFile& ZeroArgsExpression::operator>>(serializer::BinaryFile& file)
{
	file << raw_type();
	file << m_funcName;
	return file;
}
// ZeroArgsExpression end


// UnaryExpression
runtime::Value UnaryExpression::Calculate()
{
	return m_function(m_pOperand->Calculate());
}

serializer::BinaryFile& UnaryExpression::operator<<(serializer::BinaryFile& file)
{
	std::string funcName;
	file >> funcName;
	m_function = runtime::findUnaryFunction(funcName);
	if (m_function == nullptr)
		throw serializer::SerializationError(serializer::msg_corruptedData);
	
	uint8_t raw_type = 0;
	file >> raw_type;
	m_pOperand = getExpressionByType(static_cast<ExpressionType>(raw_type));
	m_pOperand->operator<<(file);
	
	return file;
}

serializer::BinaryFile& UnaryExpression::operator>>(serializer::BinaryFile& file)
{
	file << raw_type();
	file << m_funcName;
	m_pOperand->operator>>(file);
	return file;
}

void UnaryExpression::SetScope(OperationScope* pScope)
{
	m_pScope = pScope;
	m_pOperand->SetScope(pScope);
}
// UnaryExpression end


// BinaryExpression
runtime::Value BinaryExpression::Calculate()
{
	return m_function(m_pLeftOperand->Calculate(), m_pRightOperand->Calculate());
}

serializer::BinaryFile& BinaryExpression::operator<<(serializer::BinaryFile& file)
{
	std::string funcName;
	file >> funcName;
	m_function = runtime::findBinaryFunction(funcName);
	if (m_function == nullptr)
		throw serializer::SerializationError(serializer::msg_corruptedData);
	
	uint8_t raw_lType = 0;
	file >> raw_lType;
	m_pLeftOperand = getExpressionByType(static_cast<ExpressionType>(raw_lType));
	m_pLeftOperand->operator<<(file);

	uint8_t raw_rType = 0;
	file >> raw_rType;
	m_pRightOperand = getExpressionByType(static_cast<ExpressionType>(raw_rType));
	m_pRightOperand->operator<<(file);
	
	return file;
}

serializer::BinaryFile& BinaryExpression::operator>>(serializer::BinaryFile& file)
{
	file << raw_type();
	file << m_funcName;
	m_pLeftOperand->operator>>(file);
	m_pRightOperand->operator>>(file);
	return file;
}

void BinaryExpression::SetScope(OperationScope* pScope)
{
	m_pScope = pScope;
	m_pLeftOperand->SetScope(pScope);
	m_pRightOperand->SetScope(pScope);
}
// BinaryExpression end


// ValueExpression
ValueExpression::ValueExpression(std::string strValue)
{
	if (KeyWords::isStringLiteral(strValue))
	{
		std::string unquotedStr;
		std::copy(strValue.begin() + 1, strValue.end() - 1, std::back_inserter(unquotedStr));
		m_value = unquotedStr;
		return;
	}

	if (strValue == KeyWords::BoolLiteralTrue)
	{
		m_value = true;
		return;
	}

	if (strValue == KeyWords::BoolLiteralFalse)
	{
		m_value = false;
		return;
	}

	// if string contains '.' - try to consider it as floating-point number
	if (strValue.find('.') != std::string::npos)
	{
		try
		{
			const float value = std::stof(strValue);
			m_value = value;
			return;
		}
		catch (std::invalid_argument&) {}
		catch (std::out_of_range& e)
		{
			std::stringstream ss;
			ss << "Out of range exception \"" << e.what() << "\" in token " << strValue;
		}
	}
	
	try
	{
		const int value = std::stoi(strValue);
		m_value = value;
		return;
	}
	catch(std::invalid_argument&) {}
	catch(std::out_of_range& e)
	{
		std::stringstream ss;
		ss << "Out of range exception \"" << e.what() << " in token " << strValue;
	}

	m_value = runtime::Value();
}

runtime::Value ValueExpression::Calculate()
{
	return m_value;
}

serializer::BinaryFile& ValueExpression::operator<<(serializer::BinaryFile& file)
{
	m_value << file;
	return file;
}

serializer::BinaryFile& ValueExpression::operator>>(serializer::BinaryFile& file)
{
	file << type;
	m_value >> file;
	return file;
}
// ValueExpression end


// IdentifierExpression
runtime::Value IdentifierExpression::Calculate()
{
	if (m_pScope == nullptr)
	{
		throw runtime::RuntimeException("no scope found");
	}

	return m_pScope->GetVariableValue(m_strIdentifier);
}

serializer::BinaryFile& IdentifierExpression::operator<<(serializer::BinaryFile& file)
{
	file >> m_strIdentifier;
	return file;
}

serializer::BinaryFile& IdentifierExpression::operator>>(serializer::BinaryFile& file)
{
	file << type;
	file << m_strIdentifier;
	return file;
}
// IdentifierExpression end
