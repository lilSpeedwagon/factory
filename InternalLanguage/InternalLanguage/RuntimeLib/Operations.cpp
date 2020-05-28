#include "pch.h"
#include "Operations.h"
#include "Definitions.h"
#include <sstream>

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
		pScope = pScope->GetParentScope().get();
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
		pScope = pScope->GetParentScope().get();
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
		pScope = pScope->GetParentScope().get();
	} while (pScope != nullptr);

	return false;
}

void OperationScope::AddOperation(OperationPtr pOperation)
{
	m_listOperations.push_back(pOperation);
}
// OperationScope end

// OperatorAssign
void OperationAssign::Execute()
{
	m_pParentScope->SetVariableValue(m_pIdentifier->GetName(), m_pExpression->Calculate());
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
// OperationControlFlow end

// OperationFunctionCall
void OperationExpression::Execute()
{
	m_pExpr->Calculate();
}
// OperationFunctionCall end

// ZeroArgsExpression
runtime::Value ZeroArgsExpression::Calculate()
{
	return m_function();
}
// ZeroArgsExpression end

// UnaryExpression
runtime::Value UnaryExpression::Calculate()
{
	return m_function(m_pOperand->Calculate());
}
// UnaryExpression end

// BinaryExpression
runtime::Value BinaryExpression::Calculate()
{
	return m_function(m_pLeftOperand->Calculate(), m_pRightOperand->Calculate());
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
// IdentifierExpression end