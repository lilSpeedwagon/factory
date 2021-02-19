#pragma once

#include <list>
#include "Utils.h"
#include "RuntimeCommon.h"
#include "SerializerCommon.h"

// predefined classes
class Expression;
DEFINE_PTR(Expression)
class ZeroArgsExpression;
DEFINE_PTR(ZeroArgsExpression)
class UnaryExpression;
DEFINE_PTR(UnaryExpression)
class BinaryExpression;
DEFINE_PTR(BinaryExpression)
class ValueExpression;
DEFINE_PTR(ValueExpression)
class IdentifierExpression;
DEFINE_PTR(IdentifierExpression)

class OperationScope;
DEFINE_PTR(OperationScope)


// types
enum OperationType : uint8_t
{
	OTUndefined = 0,
	OTScope = 1,
	OTAssign = 2,
	OTControlFlow = 3,
	OTOpExpression = 4,
};

enum ExpressionType : uint8_t
{
	ETUndefined = 0,
	ETZeroArgExpression = 11,
	ETUnaryExpression = 12,
	ETBinaryExpression = 13,
	ETValueExpression = 14,
	ETIdentifierExpression = 15,
};


inline ExpressionPtr getExpressionByType(ExpressionType t);


// classes declaration
class Operation : public serializer::Serializable
{
public:
	virtual ~Operation() = default;
	virtual void Execute() = 0;
	virtual void Reset() {}
	virtual void SetParentScope(OperationScope* pScope) { m_pParentScope = pScope; }
	OperationScope* GetParentScope() const { return m_pParentScope; }
	
protected:
	OperationScope* m_pParentScope = nullptr;
	inline virtual uint8_t raw_type() const = 0;
};
DEFINE_PTR(Operation)


class OperationScope : public Operation
{
	RESTRICT_COPY(OperationScope)
public:
	OperationScope() = default;
	virtual ~OperationScope() = default;
	
	void Execute() override;
	runtime::Value GetVariableValue(std::string const& varName) const;
	void SetVariableValue(std::string const& varName, runtime::Value const& val);
	bool IsVariableExist(std::string const& varName) const;
	void AddStaticVariable(std::string const& name);
	void AddOperation(OperationPtr pOperation);
	void Reset() override;
	bool IsRoot() const { return m_pParentScope == nullptr; }

	void SetParentScope(OperationScope* pScope) override;

	virtual serializer::BinaryFile& operator<<(serializer::BinaryFile&) override;
	virtual serializer::BinaryFile& operator>>(serializer::BinaryFile&) override;
	
protected:
	virtual inline uint8_t raw_type() const override { return static_cast<uint8_t>(OTScope); }
	
private:
	std::map<std::string, runtime::Value> m_mapVariables;
	std::map<std::string, runtime::Value> m_mapStaticVariables;
	std::list<OperationPtr> m_listOperations;
};


class OperationAssign : public Operation
{
	RESTRICT_COPY(OperationAssign)
public:
	OperationAssign() = default;
	OperationAssign(IdentifierExpressionPtr pIdentifier, ExpressionPtr pExpr) :
		m_pIdentifier(pIdentifier), m_pExpression(pExpr) {}
	virtual ~OperationAssign() = default;
	
	void Execute() override;

	virtual serializer::BinaryFile& operator<<(serializer::BinaryFile&) override;
	virtual serializer::BinaryFile& operator>>(serializer::BinaryFile&) override;

	void SetParentScope(OperationScope* pScope) override;

protected:
	virtual inline uint8_t raw_type() const override { return static_cast<uint8_t>(OTAssign); }
	
private:
	IdentifierExpressionPtr m_pIdentifier;
	ExpressionPtr m_pExpression;
};
DEFINE_PTR(OperationAssign)


class OperationControlFlow : public Operation
{
	RESTRICT_COPY(OperationControlFlow)
public:
	OperationControlFlow() = default;
	OperationControlFlow(ExpressionPtr pExpr, OperationScopePtr pScopeIfTrue, bool isLoop = false) :
		m_pCondition(pExpr), m_pScopeIfTrue(pScopeIfTrue), m_pScopeElse(nullptr), m_isLoop(isLoop) {}
	OperationControlFlow(ExpressionPtr pExpr, OperationScopePtr pScopeIfTrue, OperationScopePtr pScopeElse, bool isLoop = false) :
		m_pCondition(pExpr), m_pScopeIfTrue(pScopeIfTrue), m_pScopeElse(pScopeElse), m_isLoop(isLoop) {}
	virtual ~OperationControlFlow() = default;
	
	void Execute() override;

	virtual serializer::BinaryFile& operator<<(serializer::BinaryFile&) override;
	virtual serializer::BinaryFile& operator>>(serializer::BinaryFile&) override;

	void SetParentScope(OperationScope* pScope) override;
	
protected:
	virtual inline uint8_t raw_type() const override { return static_cast<uint8_t>(OTControlFlow); }

private:
	ExpressionPtr m_pCondition;
	OperationScopePtr m_pScopeIfTrue;
	OperationScopePtr m_pScopeElse;
	bool m_isLoop;
};
DEFINE_PTR(OperationControlFlow)


class OperationExpression : public Operation
{
	RESTRICT_COPY(OperationExpression)
public:
	OperationExpression() = default;
	OperationExpression(ExpressionPtr pFuncExpr) : m_pExpr(pFuncExpr) {}
	virtual ~OperationExpression() = default;

	void Execute() override;
	
	virtual serializer::BinaryFile& operator<<(serializer::BinaryFile&) override;
	virtual serializer::BinaryFile& operator>>(serializer::BinaryFile&) override;

	void SetParentScope(OperationScope* pScope) override;

protected:
	virtual inline uint8_t raw_type() const override { return static_cast<uint8_t>(OTOpExpression); }
	
private:
	ExpressionPtr m_pExpr;
};
DEFINE_PTR(OperationExpression)


// Expressions

class Expression : public serializer::Serializable
{
public:
	virtual ~Expression() = default;
	virtual runtime::Value Calculate() = 0;

	virtual void SetScope(OperationScope* pScope) { m_pScope = pScope; }
protected:
	OperationScope* m_pScope = nullptr;
	virtual inline uint8_t raw_type() const = 0;
};


class ZeroArgsExpression : public Expression
{
	RESTRICT_COPY(ZeroArgsExpression)
public:
	ZeroArgsExpression() {}
	ZeroArgsExpression(std::string const& funcName, runtime::FunctionZeroArgs func) :
		m_funcName(funcName), m_function(func) {}
	virtual ~ZeroArgsExpression() = default;

	runtime::Value Calculate() override;

	virtual serializer::BinaryFile& operator<<(serializer::BinaryFile&) override;
	virtual serializer::BinaryFile& operator>>(serializer::BinaryFile&) override;
	
protected:
	virtual inline uint8_t raw_type() const override { return static_cast<uint8_t>(ETZeroArgExpression); }
	
private:
	std::string m_funcName;
	runtime::FunctionZeroArgs m_function;
};


class UnaryExpression : public Expression
{
	RESTRICT_COPY(UnaryExpression)
public:
	UnaryExpression() {}
	UnaryExpression(std::string const& funcName, runtime::FunctionUnary func, ExpressionPtr operand) :
		m_funcName(funcName), m_pOperand(operand) {}
	virtual ~UnaryExpression() = default;
	
	runtime::Value Calculate() override;

	virtual serializer::BinaryFile& operator<<(serializer::BinaryFile&) override;
	virtual serializer::BinaryFile& operator>>(serializer::BinaryFile&) override;

	void SetScope(OperationScope* pScope) override;
	
protected:
	virtual inline uint8_t raw_type() const override { return static_cast<uint8_t>(ETUnaryExpression); }

private:
	std::string m_funcName;
	runtime::FunctionUnary m_function;
	ExpressionPtr m_pOperand;
};


class BinaryExpression : public Expression
{
	RESTRICT_COPY(BinaryExpression)
public:
	BinaryExpression() {}
	BinaryExpression(std::string const& funcName, runtime::FunctionBinary func, ExpressionPtr leftOp, ExpressionPtr rightOp) :
		m_funcName(funcName), m_function(func), m_pLeftOperand(leftOp), m_pRightOperand(rightOp) {}
	virtual ~BinaryExpression() = default;
	
	runtime::Value Calculate() override;

	virtual serializer::BinaryFile& operator<<(serializer::BinaryFile&) override;
	virtual serializer::BinaryFile& operator>>(serializer::BinaryFile&) override;

	void SetScope(OperationScope* pScope) override;
	
protected:
	virtual inline uint8_t raw_type() const override { return static_cast<uint8_t>(ETBinaryExpression); }

private:
	std::string m_funcName;
	runtime::FunctionBinary m_function;
	ExpressionPtr m_pLeftOperand;
	ExpressionPtr m_pRightOperand;
};


class ValueExpression : public Expression
{
	RESTRICT_COPY(ValueExpression)
public:
	ValueExpression() {}
	ValueExpression(std::string strValue);
	virtual ~ValueExpression() = default;
	
	runtime::Value Calculate() override;

	virtual serializer::BinaryFile& operator<<(serializer::BinaryFile&) override;
	virtual serializer::BinaryFile& operator>>(serializer::BinaryFile&) override;
	
protected:
	virtual inline uint8_t raw_type() const override { return static_cast<uint8_t>(ETValueExpression); }

private:
	runtime::Value m_value;
	inline static const uint8_t type = ExpressionType::ETValueExpression;
};


class IdentifierExpression : public Expression
{
	RESTRICT_COPY(IdentifierExpression)
public:
	IdentifierExpression() {}
	IdentifierExpression(std::string const& name) :
		m_strIdentifier(name) {}
	virtual ~IdentifierExpression() = default;
	
	runtime::Value Calculate() override;
	std::string GetName() const { return m_strIdentifier; }

	virtual serializer::BinaryFile& operator<<(serializer::BinaryFile&) override;
	virtual serializer::BinaryFile& operator>>(serializer::BinaryFile&) override;
	
protected:
	virtual inline uint8_t raw_type() const override { return static_cast<uint8_t>(ETValueExpression); }

private:
	std::string m_strIdentifier;
	inline static const uint8_t type = ExpressionType::ETIdentifierExpression;
};
