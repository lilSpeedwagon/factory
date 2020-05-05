#include "stdafx.h"
#include "Operations.h"
#include "DataTypes.h"
#include "Runtime.h"
#include "sstream"

// OperationScope
void OperationScope::Execute()
{
	for (auto op : m_listOperations)
	{
		op->Execute();
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

void OperationScope::ExtendView(std::stringstream& ss, int nLevel)
{
	make_indent(ss, nLevel);
	ss << "scope\n";
	for(auto op : m_listOperations)
		op->ExtendView(ss, nLevel + 1);
}

void OperationScope::AddOperation(OperationPtr pOperation)
{
	m_listOperations.push_back(pOperation);
}
// OperationScope end

// OperatorAssign
void OperationAssign::Execute()
{
	
}

void OperationAssign::ExtendView(std::stringstream& ss, int nLevel)
{
	make_indent(ss, nLevel);
	ss << "assign\n";
	m_pIdentifier->ExtendView(ss, nLevel + 1);
	m_pExpression->ExtendView(ss, nLevel + 1);
}
// OperatorAssign end

// OperationControlFlow
void OperationControlFlow::Execute()
{
	
}

void OperationControlFlow::ExtendView(std::stringstream& ss, int nLevel)
{
	make_indent(ss, nLevel);
	ss << "Control flow " << (m_isLoop ? "loop\n" : "\n");
	m_pCondition->ExtendView(ss, nLevel + 1);
	m_pScopeIfTrue->ExtendView(ss, nLevel + 1);
	if (m_pScopeElse != nullptr)
		m_pScopeElse->ExtendView(ss, nLevel + 1);
}
// OperationControlFlow end

// UnaryExpression
Value UnaryExpression::Calculate()
{
	return m_function(m_pOperand->Calculate());
}

void UnaryExpression::ExtendView(std::stringstream& ss, int nLevel)
{
	make_indent(ss, nLevel);
	ss << "unary\n";
	m_pOperand->ExtendView(ss, ++nLevel);
}
// UnaryExpression end

// BinaryExpression
Value BinaryExpression::Calculate()
{
	return m_function(m_pLeftOperand->Calculate(), m_pRightOperand->Calculate());
}

void BinaryExpression::ExtendView(std::stringstream& ss, int nLevel)
{
	make_indent(ss, nLevel);
	ss << "binary\n";
	m_pLeftOperand->ExtendView(ss, nLevel + 1);
	m_pRightOperand->ExtendView(ss, nLevel + 1);
}
// BinaryExpression end

// ValueExpression
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

void ValueExpression::ExtendView(std::stringstream& ss, int nLevel)
{
	make_indent(ss, nLevel);
	ss << "value\n";
}
// ValueExpression end

// IdentifierExpression
Value IdentifierExpression::Calculate()
{
	if (m_pScope == nullptr)
	{
		throw Runtime::RuntimeException("no scope found");
	}

	return m_pScope->GetVariableValue(m_strIdentifier);
}

void IdentifierExpression::ExtendView(std::stringstream& ss, int nLevel)
{
	make_indent(ss, nLevel);
	ss << "identifier: " << m_strIdentifier << '\n';
}
// IdentifierExpression end