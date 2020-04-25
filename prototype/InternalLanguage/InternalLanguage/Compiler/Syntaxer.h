#pragma once

#include "stdafx.h"
#include "Definitions.h"
#include "Logger.h"
#include "Operations.h"

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

	void Run();
	
private:
	typedef Tokens::TokenList::iterator ItToken;

	enum OperatorPriority
	{
		Undefined = -1,
		Lowest = 0,
		Low = 1,
		Medium = 5,
		High = 10
	};
	
	void prepare_tokens();
	void extend_expression(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope);
	void extend_scope(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope);

	static ItToken find_high_priority_operator(ItToken itBegin, ItToken itEnd);
	static ItToken find_if_throw(ItToken itBegin, ItToken itEnd, bool(*predicate)(Tokens::Token& t));
	static ItToken find_token_type(ItToken itBegin, ItToken itEnd, Tokens::TokenType t);
	static ItToken find_token_type_throw(ItToken itBegin, ItToken itEnd, Tokens::TokenType t);
	static inline OperatorPriority get_operator_priority(ItToken t);
	
	std::shared_ptr<Tokens::TokenList> m_pTokens;
};


class CompilationError : public Utils::BaseException
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
		m_msg = msg + " source: " + std::string(itBegin, itEnd);
	}
	virtual ~CompilationError() = default;
};


