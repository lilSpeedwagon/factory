#pragma once

#include "stdafx.h"
#include "Definitions.h"
#include "OperationTreeViewHelper.h"

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
class Operation : public TreeHelper
{
public:
	virtual ~Operation() = default;
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
	void ExtendView(std::stringstream& ss, int nLevel) override;

protected:
	std::map<std::string, Value> m_mapVariables;
	std::list<OperationPtr> m_listOperations;
};
DEFINE_PTR(OperationScope)


class OperationAssign : public Operation
{
	RESTRICT_COPY(OperationAssign)
public:
	OperationAssign(IdentifierExpressionPtr pIdentifier, ExpressionPtr pExpr) :
		m_pIdentifier(pIdentifier), m_pExpression(pExpr) {}
	virtual ~OperationAssign() = default;
	
	void Execute() override;
	void ExtendView(std::stringstream& ss, int nLevel) override;
private:
	IdentifierExpressionPtr m_pIdentifier;
	ExpressionPtr m_pExpression;
};
DEFINE_PTR(OperationAssign)


class OperationControlFlow : public Operation
{
	RESTRICT_COPY(OperationControlFlow)
public:
	OperationControlFlow(ExpressionPtr pExpr, OperationScopePtr pScopeIfTrue, bool isLoop = false) :
		m_pCondition(pExpr), m_pScopeIfTrue(pScopeIfTrue), m_pScopeElse(nullptr), m_isLoop(isLoop) {}
	OperationControlFlow(ExpressionPtr pExpr, OperationScopePtr pScopeIfTrue, OperationScopePtr pScopeElse, bool isLoop = false) :
		m_pCondition(pExpr), m_pScopeIfTrue(pScopeIfTrue), m_pScopeElse(pScopeElse), m_isLoop(isLoop) {}
	virtual ~OperationControlFlow() = default;
	void Execute() override;
	void ExtendView(std::stringstream& ss, int nLevel) override;
private:
	ExpressionPtr m_pCondition;
	OperationScopePtr m_pScopeIfTrue;
	OperationScopePtr m_pScopeElse;
	bool m_isLoop;
};
DEFINE_PTR(OperationControlFlow)

class Expression : public TreeHelper
{
public:
	virtual ~Expression() = default;
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
	void ExtendView(std::stringstream& ss, int nLevel) override;

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
	void ExtendView(std::stringstream& ss, int nLevel) override;
	
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
	void ExtendView(std::stringstream& ss, int nLevel) override;
	
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
	void ExtendView(std::stringstream& ss, int nLevel) override;
	
protected:
	std::string m_strIdentifier;
};
