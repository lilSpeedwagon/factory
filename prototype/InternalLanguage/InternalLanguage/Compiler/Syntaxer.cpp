#include "stdafx.h"
#include "Syntaxer.h"


void Syntaxer::prepare_tokens()
{
	Log("Preparing tokens.");
	
	// remove delimiters
	std::remove_if(m_pTokens->begin(), m_pTokens->end(), [](Tokens::Token& t)
	{
		return t.type == Tokens::Delimiter;
	});

	// unite complex operators

	// remove comments
	
	Log("Tokens are ready.");
}

void Syntaxer::Run()
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
	}

	Log("Done.");
}

void Syntaxer::extend_scope(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope)
{
	Log("extending scope for : " + std::string(itBegin->value.cbegin(), itEnd->value.cend()));
	
	auto it = itBegin;
	auto expr_begin = it, expr_end = it;;
	while (it != itEnd)
	{
		// expression in scope
		if (it->type == Tokens::Identifier)
		{
			ItToken itExprEnd = find_if_throw(it, itEnd, [](Tokens::Token& t) { return t.type == Tokens::Semicolon; });
			extend_expression(it, itExprEnd, pCurrentScope);
			it = itExprEnd, 1;
			continue;
		}

		// scope
		if (it->type == Tokens::QBracket)
		{
			
		}
	}
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


void Syntaxer::extend_expression(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope)
{
	Log("Extending expression: " + std::string(itBegin->value.cbegin(), itEnd->value.cend()));
	
	// assignment
	ItToken itAssign = find_token_type(itBegin, itEnd, Tokens::Assignment);
	if (itAssign != itEnd)
	{
		if (std::distance(itBegin, itAssign) != 1 || itBegin->type != Tokens::Identifier)
			throw CompilationError("Cannot assign value to non identifier", itBegin, itEnd);

		size_t distToEnd = std::distance(itAssign, itEnd);
		if (distToEnd < 1)
			throw CompilationError("Missing assignment argument", itBegin, itEnd);

		if (distToEnd == 1)
		{
			Tokens::TokenType rightArgType = (itAssign + 1)->type;
			switch(rightArgType)
			{
			case Tokens::Identifier:

				break;
			case Tokens::Number:
				
				break;
			default:
				break;
			}
		}
		
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


