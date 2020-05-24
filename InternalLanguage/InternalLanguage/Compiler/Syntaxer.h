#pragma once

#include "stdafx.h"
#include "Definitions.h"
#include "Logger.h"
#include "Operations.h"
#include "BaseException.h"

class CompilationError : public BaseException
{
public:
	CompilationError() = default;
	CompilationError(std::string msg) { m_msg = msg; }
	CompilationError(std::string msg, std::string src) { m_msg = msg + " source: " + src; }
	CompilationError(std::string msg, Tokens::TokenList::iterator itBegin, Tokens::TokenList::iterator itEnd)
	{
		m_msg = msg + " source: " + Tokens::make_string_from_tokens(itBegin, itEnd);
	}
	virtual ~CompilationError() = default;
};

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
	OperationScopePtr GetResult() const { return m_result; }
	
private:
	typedef Tokens::TokenList::iterator ItToken;
	OperationScopePtr m_result;
	
	
	void prepare_tokens();
	
	void extend_operation(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope);
	void extend_scope(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope);
	OperationAssignPtr extend_assignment(ItToken itBegin, ItToken itEnd, ItToken itAssign, OperationScopePtr pCurrentScope) const;
	ExpressionPtr extend_expression(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope) const;
	ExpressionPtr extend_function(ItToken itBegin, ItToken itEnd, OperationScopePtr pCurrentScope) const;

	static bool reduce_brackets(ItToken& itBegin, ItToken& itEnd);
	static bool is_bracketed_expr(ItToken itBegin, ItToken itEnd);
	static ItToken find_lowest_priority_operator(ItToken itBegin, ItToken itEnd);

	static runtime::FunctionUnary get_function_for_unary_operator(std::string const& op);
	static runtime::FunctionBinary get_function_for_binary_operator(std::string const& op);

	static runtime::FunctionUnary get_unary_function(std::string const& f);
	static runtime::FunctionBinary get_binary_function(std::string const& f);
	
	static ItToken find_if_throw(ItToken itBegin, ItToken itEnd, bool(*predicate)(Tokens::Token& t));
	static ItToken find_token_type(ItToken itBegin, ItToken itEnd, Tokens::TokenType t);
	static ItToken find_token_type_throw(ItToken itBegin, ItToken itEnd, Tokens::TokenType t);
	static ItToken find_token(ItToken itBegin, ItToken itEnd, Tokens::TokenType t, std::string const& value);
	static ItToken find_token_throw(ItToken itBegin, ItToken itEnd, Tokens::TokenType t, std::string const& value);

	template<Tokens::TokenType type>
	static ItToken find_close_bracket(ItToken itLeftBracket, ItToken itEnd)
	{
		static_assert(type == Tokens::Bracket || type == Tokens::CBracket || type == Tokens::QBracket);
		
		ItToken it = itLeftBracket + 1;

		std::string openBracket;
		std::string closeBracket;

		switch (type)
		{
		case Tokens::Bracket:
			openBracket = "("; closeBracket = ")";
			break;
		case Tokens::CBracket:
			openBracket = "{"; closeBracket = "}";
			break;
		case Tokens::QBracket:
			openBracket = "["; closeBracket = "]";
			break;
		default: {}
		}

		// looking for close bracket for very left bracket
		for (int nOpenBrackets = 1; it != itEnd; ++it)
		{
			if (it->type == type)
			{
				if (it->value == closeBracket)
					--nOpenBrackets;
				else if (it->value == openBracket)
					++nOpenBrackets;

				if (nOpenBrackets <= 0)
					return it;
			}
		}

		throw CompilationError("\'" + closeBracket + "\' is missing", itLeftBracket, itEnd);
	}
	
	std::shared_ptr<Tokens::TokenList> m_pTokens;
};




