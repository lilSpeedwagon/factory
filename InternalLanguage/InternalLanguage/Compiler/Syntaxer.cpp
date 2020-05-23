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

	m_result.reset();
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

	std::stringstream ss;
	ss << "Operations tree:\n";
	operation_tree->ExtendView(ss, 0);
	Log(ss.str());

	m_result = operation_tree;
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
		// expression or operation in scope
		if (it->type == Tokens::Identifier)
		{
			if (it->value == KeyWords::IfElse.first)
			{
				Log("Extending if-else block.");
				// find condition
				ItToken itOpenBracket = it + 1;
				if (itOpenBracket == itEnd || itOpenBracket->type != Tokens::Bracket || itOpenBracket->value != "(")
					throw CompilationError("Missing condition expression.");

				ItToken itCloseBracket = find_token_throw(itOpenBracket + 1, itEnd, Tokens::Bracket, ")");
				Log("Condition: " + Tokens::make_string_from_tokens(itOpenBracket + 1, itCloseBracket));
				ExpressionPtr pExprCondition = extend_expression(itOpenBracket + 1, itCloseBracket, pCurrentScope);
				pExprCondition->SetScope(pCurrentScope);

				// find true scope
				ItToken itOpenTrueBracket = itCloseBracket + 1;
				if (itOpenTrueBracket == itEnd || itOpenTrueBracket->type != Tokens::CBracket || itOpenTrueBracket->value != "{")
					throw CompilationError("Missing brackets for if scope.");

				ItToken itCloseTrueBracket = find_token_throw(itOpenTrueBracket + 1, itEnd, Tokens::CBracket, "}");
				Log("If scope: " + Tokens::make_string_from_tokens(itOpenTrueBracket + 1, itCloseTrueBracket));
				OperationScopePtr pIfTrueScope = std::make_shared<OperationScope>();
				extend_scope(itOpenTrueBracket + 1, itCloseTrueBracket, pIfTrueScope);
				pIfTrueScope->SetParentScope(pCurrentScope);

				ItToken itLastBracket = itCloseTrueBracket;
				
				// find else scope
				OperationScopePtr pElseScope = nullptr;
				
				ItToken itPossibleElse = itCloseTrueBracket + 1;
				if (itPossibleElse != itEnd && itPossibleElse->type == Tokens::Identifier && itPossibleElse->value == KeyWords::IfElse.second)
				{
					ItToken itOpenElseBracket = itPossibleElse + 1;
					if (itOpenElseBracket == itEnd || itOpenElseBracket->type != Tokens::CBracket || itOpenElseBracket->value != "{")
						throw CompilationError("Missing brackets for else scope.");

					ItToken itCloseElseBracket = find_token_throw(itOpenElseBracket + 1, itEnd, Tokens::CBracket, "}");
					Log("Else scope: " + Tokens::make_string_from_tokens(itOpenElseBracket + 1, itCloseElseBracket));
					pElseScope = std::make_shared<OperationScope>();
					extend_scope(itOpenElseBracket + 1, itCloseElseBracket, pElseScope);
					pElseScope->SetParentScope(pCurrentScope);

					itLastBracket = itCloseElseBracket;
				}

				OperationControlFlowPtr pIfElse = std::make_shared<OperationControlFlow>(pExprCondition, pIfTrueScope, pElseScope);
				pIfElse->SetParentScope(pCurrentScope);
				pCurrentScope->AddOperation(pIfElse);

				it = itLastBracket + 1;
			}
			else if (it->value == KeyWords::Loop)
			{
				
			}
			else
			{
				ItToken itExprEnd = find_token_type_throw(it, itEnd, Tokens::Semicolon);
				extend_operation(it, itExprEnd, pCurrentScope);
				it = itExprEnd + 1;
			}
			continue;
		}

		// scope
		if (it->type == Tokens::CBracket)
		{
			if (it->value != "{")
				throw CompilationError("Unexpected bracket");
			
			ItToken itScopeEnd = find_token(it + 1, itEnd, Tokens::CBracket, "}");
			OperationScopePtr pInternalScope = std::make_shared<OperationScope>();
			extend_scope(it + 1, itScopeEnd, pInternalScope);
			pInternalScope->SetParentScope(pCurrentScope);
			pCurrentScope->AddOperation(pInternalScope);
			it = itScopeEnd + 1;
			continue;
		}

		throw CompilationError("Unexpected token: " + it->value);
	}
}

void Syntaxer::extend_operation(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope)
{
	Log("Extending operation: " + make_string_from_tokens(itBegin, itEnd));
	
	// assignment
	ItToken itAssign = find_token_type(itBegin, itEnd, Tokens::Assignment);
	if (itAssign != itEnd)
	{
		OperationAssignPtr pAssignOp = extend_assignment(itBegin, itEnd, itAssign, pCurrentScope);
		pAssignOp->SetParentScope(pCurrentScope);
		pCurrentScope->AddOperation(pAssignOp);
		return;
	}

	// function call
	const ItToken itOpenBracket = itBegin + 1;
	if (itOpenBracket->type == Tokens::Bracket && itOpenBracket->value == "(")
	{
		const ItToken itCloseBracket = find_close_bracket(itOpenBracket, itEnd);
		const ItToken itFunction = itBegin;
		ExpressionPtr pExpr = extend_function(itFunction, itCloseBracket, pCurrentScope);
		OperationFunctionCallPtr pFunc = std::make_shared<OperationFunctionCall>(pExpr);
		pCurrentScope->AddOperation(pFunc);
		return;
	}
}

OperationAssignPtr Syntaxer::extend_assignment(ItToken itBegin, ItToken itEnd, ItToken itAssign, OperationScopePtr pCurrentScope) const
{
	Log("Extending assignment: " + Tokens::make_string_from_tokens(itBegin, itEnd));
	
	if (std::distance(itBegin, itAssign) != 1 || itBegin->type != Tokens::Identifier)
		throw CompilationError("Cannot assign value to non identifier", itBegin, itEnd);

	if (std::find(KeyWords::KeyWords.cbegin(), KeyWords::KeyWords.cend(), itBegin->value) != KeyWords::KeyWords.cend())
		throw CompilationError("Unexpected keyword in the left part of assignment", itBegin, itEnd);
	
	Log("Left operand: " + itBegin->value);
	IdentifierExpressionPtr pIdentifier = std::make_shared<IdentifierExpression>(itBegin->value);
	ItToken itRightOperand = itAssign + 1;
	Log("Right operand: " + Tokens::make_string_from_tokens(itRightOperand, itEnd));
	ExpressionPtr pExpr = extend_expression(itRightOperand, itEnd, pCurrentScope);
	pExpr->SetScope(pCurrentScope);

	return std::make_shared<OperationAssign>(pIdentifier, pExpr);
}


ExpressionPtr Syntaxer::extend_expression(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope) const
{
	Log("Extending expression: " + Tokens::make_string_from_tokens(itBegin, itEnd));
	ExpressionPtr pExpr;

	if (reduce_brackets(itBegin, itEnd))
		Log("Brackets reduced.");
	
	const size_t length = std::distance(itBegin, itEnd);

	if (length == 0)
		throw CompilationError("Unexpected empty expression.");
	
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
			pExpr->SetScope(pCurrentScope);
			break;
		case Tokens::Number:
			Log("Expression is a value: " + strArg);
			pExpr = std::make_shared<ValueExpression>(strArg);
			pExpr->SetScope(pCurrentScope);
			break;
		case Tokens::Quote:
			Log("Expression is a string literal: " + strArg);
			pExpr = std::make_shared<ValueExpression>(strArg);
			pExpr->SetScope(pCurrentScope);
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
		const std::string strOperator = itLowestPriority->value;
		Log("Extend arguments of operator: " + strOperator);

		// 2. extend its operands
		const bool hasLeftOperand = std::distance(itBegin, itLowestPriority) > 0;
		const bool isUnaryOperation = Operators::isUnaryOperator(strOperator) && !hasLeftOperand;
		if (isUnaryOperation)
		{
			Log("Unary operand: " + Tokens::make_string_from_tokens(itLowestPriority + 1, itEnd));
			ExpressionPtr pOperand = extend_expression(itLowestPriority + 1, itEnd, pCurrentScope);
			pOperand->SetScope(pCurrentScope);
			
			runtime::FunctionUnary func = get_function_for_unary_operator(strOperator);
			pExpr = std::make_shared<UnaryExpression>(func, pOperand);
			pExpr->SetScope(pCurrentScope);
		}
		else
		{
			Log("Left operand: " + Tokens::make_string_from_tokens(itBegin, itLowestPriority));
			ExpressionPtr pLeftOperand = extend_expression(itBegin, itLowestPriority, pCurrentScope);
			pLeftOperand->SetScope(pCurrentScope);
			
			Log("Right operand: " + Tokens::make_string_from_tokens(itLowestPriority + 1, itEnd));
			ExpressionPtr pRightOperand = extend_expression(itLowestPriority + 1, itEnd, pCurrentScope);
			pRightOperand->SetScope(pCurrentScope);

			runtime::FunctionBinary func = get_function_for_binary_operator(strOperator);
			pExpr = std::make_shared<BinaryExpression>(func, pLeftOperand, pRightOperand);
			pExpr->SetScope(pCurrentScope);
		}
	}
	
	return pExpr;
}

ExpressionPtr Syntaxer::extend_function(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope) const
{
	Log("Extending function: " + Tokens::make_string_from_tokens(itBegin, itEnd));
	const ItToken itFunc = itBegin;
	const ItToken itLeftBracket = itFunc + 1;

	ExpressionPtr pExpr;
	
	if (std::distance(itLeftBracket, itEnd) == 0)
	{
		// functions without args
		// TODO
	}
	else
	{
		const size_t args_number = std::count <ItToken, Tokens::Token>(itLeftBracket + 1, itEnd, { ",", Tokens::Comma }) + 1;

		if (args_number == 1)
		{
			runtime::FunctionUnary func = get_unary_function(itFunc->value);
			ExpressionPtr pArg = extend_expression(itLeftBracket + 1, itEnd, pCurrentScope);
			pExpr = std::make_shared<UnaryExpression>(func, pArg);
			pExpr->SetScope(pCurrentScope);
		}
		else if (args_number == 2)
		{
			runtime::FunctionBinary func = get_binary_function(itFunc->value);
			ItToken itComma = find_token(itLeftBracket + 1, itEnd, Tokens::Comma, ",");
			ExpressionPtr pLeftArg = extend_expression(itLeftBracket + 1, itComma, pCurrentScope);
			ExpressionPtr pRightArg = extend_expression(itComma, itEnd, pCurrentScope);
			pExpr = std::make_shared<BinaryExpression>(func, pLeftArg, pRightArg);
			pExpr->SetScope(pCurrentScope);
		}
		else
		{
			// functions with many args
			// TODO
		}
	}
	
	return pExpr;
}


bool Syntaxer::reduce_brackets(ItToken& itBegin, ItToken& itEnd)
{
	bool isBracketed = false;
	while (is_bracketed_expr(itBegin, itEnd))
	{
		++itBegin;
		--itEnd;
		isBracketed = true;
	}
	return isBracketed;
}

bool Syntaxer::is_bracketed_expr(ItToken itBegin, ItToken itEnd)
{
	bool isBracketed = false;
	
	const bool hasBrackets = std::distance(itBegin, itEnd) >= 2 &&
		(itBegin->type == Tokens::Bracket && itBegin->value == "(") &&
		((itEnd - 1)->type == Tokens::Bracket && (itEnd - 1)->value == ")");
	
	if (hasBrackets)
	{
		const ItToken it = find_close_bracket(itBegin, itEnd);

		// if very right bracket closes very left bracket then the expression is bracketed
		isBracketed = it == itEnd - 1;
	}

	return isBracketed;
}

Syntaxer::ItToken Syntaxer::find_lowest_priority_operator(ItToken itBegin, ItToken itEnd)
{
	ItToken it = itBegin, itLowestPriorityOperator = itEnd;
	Operators::Priority lowestPriority = Operators::max_priority;
	Operators::Priority bracketsPriority = 0;
	
	do
	{
		if (it->type == Tokens::Operator)
		{
			const Operators::Priority priority = Operators::operatorPriority(it->value) + bracketsPriority;

			if (priority > 0 && priority < lowestPriority)
			{
				lowestPriority = priority;
				itLowestPriorityOperator = it;
			}
		}
		else if (it->type == Tokens::Bracket)
		{
			if (it->value == ")")
			{
				bracketsPriority -= Operators::brackets_priority;
			}
			if (it->value == "(")
			{
				bracketsPriority += Operators::brackets_priority;
			}
		}
		++it;
	}
	while (it != itEnd);
	
	return itLowestPriorityOperator;
}

runtime::FunctionUnary Syntaxer::get_function_for_unary_operator(std::string const& op)
{
	auto it = runtime::mapUnaryOperators.find(op);
	if (it == runtime::mapUnaryOperators.cend())
	{
		throw CompilationError("Cannot find runtime operation for unary operator \"" + op + "\"");
	}
	return it->second;
}

runtime::FunctionBinary Syntaxer::get_function_for_binary_operator(std::string const& op)
{
	auto it = runtime::mapBinaryOperators.find(op);
	if (it == runtime::mapBinaryOperators.cend())
	{
		throw CompilationError("Cannot find runtime operation for binary operator \"" + op + "\"");
	}
	return it->second;
}

runtime::FunctionUnary Syntaxer::get_unary_function(std::string const& f)
{
	auto it = runtime::mapUnaryFunctions.find(f);
	if (it == runtime::mapUnaryFunctions.end())
	{
		throw CompilationError("Cannot find runtime operation for unary function \"" + f + "\"");
	}
	return it->second;
}

runtime::FunctionBinary Syntaxer::get_binary_function(std::string const& f)
{
	auto it = runtime::mapBinaryFunctions.find(f);
	if (it == runtime::mapBinaryFunctions.end())
	{
		throw CompilationError("Cannot find runtime operation for binary function \"" + f + "\"");
	}
	return it->second;
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

Syntaxer::ItToken Syntaxer::find_token(ItToken itBegin, ItToken itEnd, Tokens::TokenType t,
	std::string const& value)
{
	ItToken it = find_token_type(itBegin, itEnd, t);
	while (it != itEnd)
	{
		if (it->value == value)
			return it;
		it = find_token_type(it + 1, itEnd, t);
	}
	return itEnd;
}

Syntaxer::ItToken Syntaxer::find_token_throw(ItToken itBegin, ItToken itEnd, Tokens::TokenType t,
	std::string const& value)
{
	ItToken it = find_token(itBegin, itEnd, t, value);
	if (it == itEnd)
		throw CompilationError("Token \"" + value + "\" expected, but not found");
	return it;
}

Syntaxer::ItToken Syntaxer::find_close_bracket(ItToken itLeftBracket, ItToken itEnd)
{
	ItToken it = itLeftBracket + 1;

	// looking for close bracket for very left bracket
	for (int nOpenBrackets = 1; it != itEnd; ++it)
	{
		if (it->type == Tokens::Bracket)
		{
			if (it->value == ")")
				--nOpenBrackets;
			else if (it->value == "(")
				++nOpenBrackets;

			if (nOpenBrackets <= 0)
				break;
		}
	}

	return it;
}