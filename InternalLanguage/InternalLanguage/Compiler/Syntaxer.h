#pragma once

#include "stdafx.h"
#include "Definitions.h"
#include "Logger.h"
#include "Operations.h"
#include "BaseException.h"

class Syntaxer : public Logger
{
	RESTRICT_COPY(Syntaxer)
public:
	Syntaxer(Tokens::TokenList const& tokens, LogDelegate log)
	{
		m_pTokens = std::make_shared<Tokens::TokenList>(tokens);
		SetLogName("Syntaxer");
		SetLogDelegate(log);
	}
	~Syntaxer() = default;

	bool Run();
	
private:
	typedef Tokens::TokenList::iterator ItToken;

	
	
	void prepare_tokens();
	
	void extend_operation(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope);
	void extend_scope(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope);
	OperationAssignPtr extend_assignment(ItToken itBegin, ItToken itEnd, ItToken itAssign) const;
	ExpressionPtr extend_expression(ItToken itBegin, ItToken itEnd) const;

	static bool reduce_brackets(ItToken& itBegin, ItToken& itEnd);
	static bool is_bracketed_expr(ItToken itBegin, ItToken itEnd);
	static ItToken find_lowest_priority_operator(ItToken itBegin, ItToken itEnd);

	static Runtime::FunctionUnary get_function_for_unary_operator(std::string const& op);
	static Runtime::FunctionBinary get_function_for_binary_operator(std::string const& op);
	
	static ItToken find_if_throw(ItToken itBegin, ItToken itEnd, bool(*predicate)(Tokens::Token& t));
	static ItToken find_token_type(ItToken itBegin, ItToken itEnd, Tokens::TokenType t);
	static ItToken find_token_type_throw(ItToken itBegin, ItToken itEnd, Tokens::TokenType t);
	static ItToken find_token(ItToken itBegin, ItToken itEnd, Tokens::TokenType t, std::string const& value);
	static ItToken find_token_throw(ItToken itBegin, ItToken itEnd, Tokens::TokenType t, std::string const& value);
	
	std::shared_ptr<Tokens::TokenList> m_pTokens;
};


class CompilationError : public BaseException
{
public:
	CompilationError() {}
	CompilationError(std::string msg)
	{
		m_msg = msg;
	}
	CompilationError(std::string msg, std::string src)
	{
		m_msg = msg + " source: " + src;
	}
	CompilationError(std::string msg, Tokens::TokenList::iterator itBegin, Tokens::TokenList::iterator itEnd)
	{
		m_msg = msg + " source: " + Tokens::make_string_from_tokens(itBegin, itEnd);
	}
	virtual ~CompilationError() = default;
};


