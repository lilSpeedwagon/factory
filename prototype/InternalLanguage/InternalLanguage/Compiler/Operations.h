#pragma once

#include "stdafx.h"
#include "Definitions.h"
#include "Runtime.h"

class Operation
{
public:
	virtual ~Operation() = 0;
	virtual void Execute() = 0;
};
DEFINE_PTR(Operation)


class OperationScope : public Operation
{
	RESTRICT_COPY(OperationScope)
public:
	OperationScope() {}
	virtual ~OperationScope() = default;
	void Execute() override;
	Runtime::Value GetVariableValue(std::string const& varName) const;

protected:
	std::map<std::string, Runtime::Value> m_mapVariables;
	std::queue<OperationPtr> m_qOperations;
};
DEFINE_PTR(OperationScope)


class Expression
{
public:
	virtual ~Expression() = 0;
	virtual Runtime::Value Calculate() = 0;

	void SetScope(OperationScopePtr pScope) { m_pScope = pScope; }
protected:
	OperationScopePtr m_pScope = nullptr;
};
DEFINE_PTR(Expression)


class UnaryExpression : public Expression
{
	RESTRICT_COPY(UnaryExpression)
public:
	typedef Runtime::Value(*UnaryFunction)(Runtime::Value val);

	UnaryExpression(UnaryFunction func, ExpressionPtr operand) :
		m_function(func), m_pOperand(operand) {}
	virtual ~UnaryExpression() = default;
	Runtime::Value Calculate() override;

protected:
	UnaryFunction m_function;
	ExpressionPtr m_pOperand;
};


class BinaryExpression : public Expression
{
	RESTRICT_COPY(BinaryExpression)
public:
	typedef Runtime::Value(*BinaryFunction)(Runtime::Value val1, Runtime::Value val2);

	BinaryExpression(BinaryFunction func, ExpressionPtr leftOp, ExpressionPtr rightOp) :
		m_function(func), m_pLeftOperand(leftOp), m_pRightOperand(rightOp) {}
	virtual ~BinaryExpression() = default;
	Runtime::Value Calculate() override;
protected:
	BinaryFunction m_function;
	ExpressionPtr m_pLeftOperand;
	ExpressionPtr m_pRightOperand;
};


class ValueExpression : public Expression
{
	RESTRICT_COPY(ValueExpression)
public:
	ValueExpression(std::string strValue);
	virtual ~ValueExpression() = default;
	Runtime::Value Calculate() override;
protected:
	Runtime::Value m_value;
};


class IdentifierExpression : public Expression
{
	RESTRICT_COPY(IdentifierExpression)
public:
	virtual ~IdentifierExpression() = default;
	Runtime::Value Calculate() override;
protected:
	std::string m_identifier;
};
