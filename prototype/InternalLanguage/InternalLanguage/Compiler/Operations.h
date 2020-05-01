#pragma once

#include "stdafx.h"
#include "Definitions.h"

// predefined classes
class Expression;
DEFINE_PTR(Expression)
class UnaryExpression;
DEFINE_PTR(UnaryExpression)
class BinaryExpression;
DEFINE_PTR(BinaryExpression)
class ValueExpression;
DEFINE_PTR(ValueExpression)
class IdentifierExpression;
DEFINE_PTR(IdentifierExpression)


// classes declaration
class Operation
{
public:
	virtual ~Operation() {}
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
	Value GetVariableValue(std::string const& varName) const;
	void AddOperation(OperationPtr pOperation);

protected:
	std::map<std::string, Value> m_mapVariables;
	std::queue<OperationPtr> m_qOperations;
};
DEFINE_PTR(OperationScope)


class OperationAssign : public Operation
{
	RESTRICT_COPY(OperationAssign)
public:
	OperationAssign(IdentifierExpressionPtr pIdentifier, ExpressionPtr pExpr) :
		m_pIdentifier(pIdentifier), m_pExpression(pExpr) {}
	virtual ~OperationAssign() = default;
	virtual void Execute();
private:
	IdentifierExpressionPtr m_pIdentifier;
	ExpressionPtr m_pExpression;
};
DEFINE_PTR(OperationAssign)


class Expression
{
public:
	virtual ~Expression() {}
	virtual Value Calculate() = 0;

	void SetScope(OperationScopePtr pScope) { m_pScope = pScope; }
protected:
	OperationScopePtr m_pScope = nullptr;
};


class UnaryExpression : public Expression
{
	RESTRICT_COPY(UnaryExpression)
public:
	typedef Value(*UnaryFunction)(Value val);

	UnaryExpression(UnaryFunction func, ExpressionPtr operand) :
		m_function(func), m_pOperand(operand) {}
	virtual ~UnaryExpression() = default;
	Value Calculate() override;

protected:
	UnaryFunction m_function;
	ExpressionPtr m_pOperand;
};


class BinaryExpression : public Expression
{
	RESTRICT_COPY(BinaryExpression)
public:
	typedef Value(*BinaryFunction)(Value val1, Value val2);

	BinaryExpression(BinaryFunction func, ExpressionPtr leftOp, ExpressionPtr rightOp) :
		m_function(func), m_pLeftOperand(leftOp), m_pRightOperand(rightOp) {}
	virtual ~BinaryExpression() = default;
	Value Calculate() override;
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
	Value Calculate() override;
protected:
	Value m_value;
};


class IdentifierExpression : public Expression
{
	RESTRICT_COPY(IdentifierExpression)
public:
	IdentifierExpression(std::string const& name) :
		m_strIdentifier(name) {}
	virtual ~IdentifierExpression() = default;
	Value Calculate() override;
protected:
	std::string m_strIdentifier;
};
