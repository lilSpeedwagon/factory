#include "stdafx.h"
#include "Syntaxer.h"


void Syntaxer::prepare_tokens()
{
	Log("Preparing tokens.");
	
	// remove delimiters
	m_pTokens->erase(std::remove_if(m_pTokens->begin(), m_pTokens->end(),
		[](Tokens::Token& t) {	return t.type == Tokens::Delimiter;	}),
		m_pTokens->end());

	// remove comments
	
	Log("Tokens are ready.");
}

bool Syntaxer::Run()
{
	Log("Running...");
	prepare_tokens();

	OperationScopePtr operation_tree = std::make_shared<OperationScope>();
	try
	{
		extend_scope(m_pTokens->begin(), m_pTokens->end(), operation_tree);
	}
	catch(CompilationError& e)
	{
		Log("Compilation error: " + std::string(e.Message()));
		return false;
	}

	Log("Done.");
	return true;
}

void Syntaxer::extend_scope(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope)
{
	Log("Extending scope for: " + make_string_from_tokens(itBegin, itEnd));
	
	auto it = itBegin;
	auto expr_begin = it, expr_end = it;;
	while (it != itEnd)
	{
		// expression in scope
		if (it->type == Tokens::Identifier)
		{
			ItToken itExprEnd = find_if_throw(it, itEnd, [](Tokens::Token& t) { return t.type == Tokens::Semicolon; });
			extend_operation(it, itExprEnd, pCurrentScope);
			it = itExprEnd + 1;
			continue;
		}

		// scope
		if (it->type == Tokens::QBracket)
		{
			
		}
	}
}

void Syntaxer::extend_operation(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope)
{
	Log("Extending operation: " + make_string_from_tokens(itBegin, itEnd));
	
	// assignment
	ItToken itAssign = find_token_type(itBegin, itEnd, Tokens::Assignment);
	if (itAssign != itEnd)
	{
		OperationAssignPtr pAssignOp = extend_assignment(itBegin, itEnd, itAssign);
		pCurrentScope->AddOperation(pAssignOp);
		return;
	}

	// find operator
	ItToken itOperator = find_token_type_throw(itBegin, itEnd, Tokens::Operator);
	if (std::distance(itBegin, itOperator) < 1 || std::distance(itOperator, itEnd) < 1)
	{
		throw CompilationError("");
	}

	
	// define left operand
	

	// define right operand
	/*if (it->type == Tokens::Bracket && it->value == "(")
	{
		ItToken itBracketEnd = find_if_throw(it, itEnd, [](Tokens::Token& t) { return t.type == Tokens::Bracket && t.value == ")"; });

	}*/
}

OperationAssignPtr Syntaxer::extend_assignment(ItToken itBegin, ItToken itEnd, ItToken itAssign) const
{
	Log("Extending assignment: " + Tokens::make_string_from_tokens(itBegin, itEnd));
	
	if (std::distance(itBegin, itAssign) != 1 || itBegin->type != Tokens::Identifier)
		throw CompilationError("Cannot assign value to non identifier", itBegin, itEnd);

	Log("Left operand: " + itBegin->value);
	IdentifierExpressionPtr pIdentifier = std::make_shared<IdentifierExpression>(itBegin->value);
	ItToken itRightOperand = itAssign + 1;
	Log("Right operand: " + Tokens::make_string_from_tokens(itRightOperand, itEnd));
	ExpressionPtr pExpr = extend_expression(itRightOperand, itEnd);

	return std::make_shared<OperationAssign>(pIdentifier, pExpr);
}


ExpressionPtr Syntaxer::extend_expression(ItToken itBegin, ItToken itEnd) const
{
	Log("Extending expression: " + Tokens::make_string_from_tokens(itBegin, itEnd));
	ExpressionPtr pExpr;
	
	const size_t length = std::distance(itBegin, itEnd);
	
	// expression is a single value or an identifier
	if (length == 1)
	{
		const Tokens::TokenType argType = (itBegin)->type;
		std::string const& strArg = itBegin->value;
		switch (argType)
		{
		case Tokens::Identifier:
			Log("Expression is an identifier: " + strArg);
			pExpr = std::make_shared<IdentifierExpression>(strArg);
			break;
		case Tokens::Number:
			Log("Expression is a value: " + strArg);
			pExpr = std::make_shared<ValueExpression>(strArg);
			break;
		case Tokens::Quote:
			Log("Expression is a string literal: " + strArg);
			pExpr = std::make_shared<ValueExpression>(strArg);
			break;
		default:
			throw CompilationError("Wrong argument in the right side of assignment.");
		}
	}
	else // expression is complex
	{
		// 1. find lowest prior operator
		ItToken itLowestPriority = find_lowest_priority_operator(itBegin, itEnd);
		if (itLowestPriority == itEnd)
		{
			throw CompilationError("Error in expression: " + make_string_from_tokens(itBegin, itEnd));
		}
		Log("Extend arguments of operator: " + itLowestPriority->value);

		// 2. extend its operands
		bool isUnaryOperation = Operators::isUnaryOperator(itLowestPriority->value);
		if (isUnaryOperation)
		{
			Log("Unary operand: " + Tokens::make_string_from_tokens(itLowestPriority + 1, itEnd));
			ExpressionPtr pOperand = extend_expression(itLowestPriority + 1, itEnd);
			
			UnaryExpression::UnaryFunction func = [](Value v) -> Value { return Value(); };
			pExpr = std::make_shared<UnaryExpression>(func, pOperand);
		}
		else
		{
			Log("Left operand: " + Tokens::make_string_from_tokens(itBegin, itLowestPriority));
			ExpressionPtr pLeftOperand = extend_expression(itBegin, itLowestPriority);
			Log("Right operand: " + Tokens::make_string_from_tokens(itLowestPriority + 1, itEnd));
			ExpressionPtr pRightOperand = extend_expression(itLowestPriority + 1, itEnd);

			BinaryExpression::BinaryFunction func = [](Value v, Value v2) -> Value { return Value(); };
			pExpr = std::make_shared<BinaryExpression>(func, pLeftOperand, pRightOperand);
		}
	}
	
	return pExpr;
}


Syntaxer::ItToken Syntaxer::find_lowest_priority_operator(ItToken itBegin, ItToken itEnd)
{
	ItToken it = itBegin, itLowestPriorityOperator = itEnd;
	Operators::Priority lowestPriority = Operators::max_priority;

	do
	{
		it = find_token_type(it + 1, itEnd, Tokens::Operator);
		const Operators::Priority priority = Operators::operatorPriority(it->value);

		if (priority > 0 && priority < lowestPriority)
		{
			lowestPriority = priority;
			itLowestPriorityOperator = it;
		}
	}
	while (it != itEnd);
	
	return itLowestPriorityOperator;
}

Syntaxer::ItToken Syntaxer::find_if_throw(ItToken itBegin, ItToken itEnd, bool(*predicate)(Tokens::Token& t))
{
	auto itRes = std::find_if(itBegin, itEnd, predicate);
	if (itRes == itEnd)
		throw CompilationError("Cannot find end token");
	
	return itRes;
}

Syntaxer::ItToken Syntaxer::find_token_type(ItToken itBegin, ItToken itEnd, Tokens::TokenType type)
{
	return std::find_if(itBegin, itEnd, [&type](Tokens::Token& t) { return t.type == type; });
}

Syntaxer::ItToken Syntaxer::find_token_type_throw(ItToken itBegin, ItToken itEnd, Tokens::TokenType type)
{
	ItToken it = find_token_type(itBegin, itEnd, type);
	if (it == itEnd)
		throw CompilationError("Token with type " + Tokens::GetTypeName(type) + " expected, but not found");
		
	return it;
}

