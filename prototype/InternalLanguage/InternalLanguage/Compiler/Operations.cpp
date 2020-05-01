#include "stdafx.h"
#include "Operations.h"
#include "DataTypes.h"
#include "Runtime.h"
#include "sstream"

void OperationScope::Execute()
{
	while (!m_qOperations.empty())
	{
		m_qOperations.front()->Execute();
		m_qOperations.pop();
	}
}

Value OperationScope::GetVariableValue(std::string const& varName) const
{
	auto it = m_mapVariables.find(varName);
	if (it == m_mapVariables.end())
	{
		throw Runtime::RuntimeException();
	}

	return it->second;
}

void OperationScope::AddOperation(OperationPtr pOperation)
{
	m_qOperations.push(pOperation);
}

void OperationAssign::Execute()
{
	
}

Value UnaryExpression::Calculate()
{
	return m_function(m_pOperand->Calculate());
}

Value BinaryExpression::Calculate()
{
	return m_function(m_pLeftOperand->Calculate(), m_pRightOperand->Calculate());
}

ValueExpression::ValueExpression(std::string strValue)
{
	if (DataTypes::isStringLiteral(strValue))
	{
		m_value = strValue;
		return;
	}

	if (strValue == DataTypes::BoolLiteralTrue)
	{
		m_value = true;
		return;
	}

	if (strValue == DataTypes::BoolLiteralFalse)
	{
		m_value = false;
		return;
	}

	try
	{
		int value = std::stoi(strValue);
		m_value = value;
	}
	catch(std::invalid_argument&) {}
	catch(std::out_of_range& e)
	{
		std::stringstream ss;
		ss << "Out of range exception \"" << e.what() << " in token " << strValue;
		throw ValueException(ss.str().c_str());
	}

	try
	{
		float value = std::stof(strValue);
		m_value = value;
	}
	catch (std::invalid_argument&) {}
	catch (std::out_of_range& e)
	{
		std::stringstream ss;
		ss << "Out of range exception \"" << e.what() << "\" in token " << strValue;
		throw ValueException(ss.str().c_str());
	}
}

Value ValueExpression::Calculate()
{
	return m_value;
}

Value IdentifierExpression::Calculate()
{
	if (m_pScope == nullptr)
	{
		throw Runtime::RuntimeException("no scope found");
	}

	return m_pScope->GetVariableValue(m_strIdentifier);
}


