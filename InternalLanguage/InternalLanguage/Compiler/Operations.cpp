#include "stdafx.h"
#include "Operations.h"
#include "Runtime.h"
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
	m_pParentScope->SetVariableValue(m_pIdentifier->GetName(), m_pExpression->Calculate());
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
		// TODO
	}
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

// OperationFunctionCall
void OperationFunctionCall::Execute()
{
	m_pFunction->Calculate();
}

void OperationFunctionCall::ExtendView(std::stringstream& ss, int nLevel)
{
	make_indent(ss, nLevel);
	ss << "function call\n";
	m_pFunction->ExtendView(ss, ++nLevel);
}
// OperationFunctionCall end

// UnaryExpression
runtime::Value UnaryExpression::Calculate()
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
runtime::Value BinaryExpression::Calculate()
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
	if (KeyWords::isStringLiteral(strValue))
	{
		m_value = strValue;
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

	try
	{
		int value = std::stoi(strValue);
		m_value = value;
		return;
	}
	catch(std::invalid_argument&) {}
	catch(std::out_of_range& e)
	{
		std::stringstream ss;
		ss << "Out of range exception \"" << e.what() << " in token " << strValue;
	}

	try
	{
		float value = std::stof(strValue);
		m_value = value;
		return;
	}
	catch (std::invalid_argument&) {}
	catch (std::out_of_range& e)
	{
		std::stringstream ss;
		ss << "Out of range exception \"" << e.what() << "\" in token " << strValue;
	}

	m_value = runtime::Value();
}

runtime::Value ValueExpression::Calculate()
{
	return m_value;
}

void ValueExpression::ExtendView(std::stringstream& ss, int nLevel)
{
	make_indent(ss, nLevel);
	ss << "value: " << m_value.toString().getValue<std::string>() << '\n';
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

void IdentifierExpression::ExtendView(std::stringstream& ss, int nLevel)
{
	make_indent(ss, nLevel);
	ss << "identifier: " << m_strIdentifier << '\n';
}
// IdentifierExpression end