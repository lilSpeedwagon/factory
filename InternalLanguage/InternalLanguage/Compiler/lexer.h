#pragma once

#include "Definitions.h"
#include "Logger.h"

class Lexer : Logger
{
	RESTRICT_COPY(Lexer)
public:
	Lexer(std::string const& code, LogDelegate& delegate) :
		m_strCode(code), m_tokensFound(0), m_undefinedTokensFound(0)
	{
		SetLogDelegate(delegate);
		SetLogName("Lexer");
	}
	~Lexer() = default;

	bool Run();
	const Tokens::TokenList& Result() const { return m_result; }

private:
	typedef std::string::const_iterator StrIter;
	void tokenize();
	inline void addToken(StrIter& begin, StrIter& end, Tokens::TokenType type);

	enum CharType
	{
		Undefined,
		Operator,
		AlNum, // number or letter
		Bracket,
		QBracket,
		CBracket,
		Quote,
		Semicolon,
		Delimiter
	};
	
	static CharType getType(char c);
	static Tokens::TokenType getTokenType(StrIter begin, StrIter end);

	static const int TOKEN_DENSITY;
	
	std::string m_strCode;
	Tokens::TokenList m_result;
	int m_tokensFound;
	int m_undefinedTokensFound;
};
