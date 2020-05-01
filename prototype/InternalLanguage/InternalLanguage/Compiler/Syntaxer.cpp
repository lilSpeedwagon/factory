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

	const size_t distToEnd = std::distance(itAssign, itEnd);
	if (distToEnd < 2)
		throw CompilationError("Missing assignment argument", itBegin, itEnd);

	Log("Left operand: " + itBegin->value);
	IdentifierExpressionPtr pIdentifier = std::make_shared<IdentifierExpression>(itBegin->value);
	ExpressionPtr pExpr;

	// right operand is single value or identifier
	if (distToEnd == 2)
	{
		const ItToken itRightArg = itAssign + 1;
		const Tokens::TokenType rightArgType = (itRightArg)->type;
		switch (rightArgType)
		{
		case Tokens::Identifier:
			Log("Right operand is an identifier: " + itRightArg->value);
			pExpr = std::make_shared<IdentifierExpression>(itRightArg->value);
			break;
		case Tokens::Number:
			Log("Right operand is a value: " + itRightArg->value);
			pExpr = std::make_shared<ValueExpression>(itRightArg->value);
			break;
		case Tokens::Quote:
			Log("Right operand is a string literal: " + itRightArg->value);
			pExpr = std::make_shared<ValueExpression>(itRightArg->value);
			break;
		default:
			throw CompilationError("Wrong argument in the right side of assignment.");
		}
	}
	else // right operand is expression
	{
		
	}

	return std::make_shared<OperationAssign>(pIdentifier, pExpr);
}


ExpressionPtr Syntaxer::extend_expression(ItToken itBegin, ItToken itEnd) const
{
	return nullptr;
}

Syntaxer::ItToken Syntaxer::find_high_priority_operator(ItToken itBegin, ItToken itEnd)
{
	/*ItToken itMaxPriority = itEnd;
	ItToken it = itBegin;
	while (it != itEnd)
	{
		it = std::find_if(it, itEnd, [](Tokens::Token& t) { return t.type == Tokens::Operator; });

	}*/
	return itBegin;
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

Syntaxer::OperatorPriority Syntaxer::get_operator_priority(ItToken t)
{
	char op = t->value[0];

	if (op == '+' || op == '-')
		return Medium;

	if (op == '*' || op == '/' || op == '%')
		return High;

	return Undefined;
}


