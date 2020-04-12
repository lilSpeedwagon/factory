#pragma once

#include "Definitions.h"
#include "Logger.h"

class Lexer : Logger
{
public:
	Lexer()	{}
	~Lexer() = default;

	void Init(std::string const& code, LogDelegate& delegate);
	bool Run();
	Tokens::TokenList Result() const { return m_result; }

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
		Quote,
		Semicolon,
		Slash,
		Delimiter
	};
	
	static CharType getType(char c);
	static Tokens::TokenType getTokenType(StrIter begin, StrIter end);
	
	std::string m_strCode;
	Tokens::TokenList m_result;
};
