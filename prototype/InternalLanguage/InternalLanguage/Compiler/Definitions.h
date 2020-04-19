#pragma once

#include "stdafx.h"
#include "Utils.h"

#define DEFINE_PTR(T) typedef std::shared_ptr<T> T##Ptr;
#define RESTRICT_COPY(T) private: T(T const&); void operator=(T const&);

typedef void(__stdcall *LogDelegate)(const char*);

namespace Tokens
{
	static const std::string Delimiters = " \n\t";
	static const std::string AdditionToIdentifiers = "-_.";
	static const std::string Operators = "+-/*%^";
	static const std::string Brackets = "()";
	static const std::string QBrackets = "[]";
	static const std::string CBrackets = "{}";
	
	enum TokenType
	{
		Undefined,
		Operator,
		Identifier,
		Number,
		Equality,
		Bracket,
		QBracket,
		CBracket,
		Quote,
		Semicolon,
		Comment,
		Delimiter
	};

#define TYPENAME(t) (#t)
#define IF_TYPE(type_name, t) { if ((type_name) == (t)) { return TYPENAME(type_name); }}
	
	inline std::string GetTypeName(TokenType t)
	{
		IF_TYPE(Operator, t)
		IF_TYPE(Identifier, t)
		IF_TYPE(Number, t)
		IF_TYPE(Equality, t)
		IF_TYPE(Bracket, t)
		IF_TYPE(QBracket, t)
		IF_TYPE(CBracket, t)
		IF_TYPE(Quote, t)
		IF_TYPE(Semicolon, t)
		IF_TYPE(Comment, t)
		IF_TYPE(Delimiter, t)
		return TYPENAME(Undefined);
	}

	typedef std::pair<std::string, TokenType> Token;
	typedef std::vector<Token> TokenList;
}

class CompilationError : public Utils::BaseException
{
public:
	CompilationError() { m_pMessage = nullptr; }
	CompilationError(const char* msg)
	{
		m_pMessage = new char[strlen(msg) + 1];
		strcpy(m_pMessage, msg);
	}
	virtual ~CompilationError() = default;
};
