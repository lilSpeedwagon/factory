#pragma once

#include "stdafx.h"
#include "Utils.h"


typedef void(__stdcall *LogDelegate)(const char*);

namespace Tokens
{
	static const std::string Delimiters = " \r\n\t";
	static const std::string AdditionToIdentifiers = "-_.";
	static const std::string Operators = "+-/*%><=";
	static const std::string Brackets = "()";
	static const std::string QBrackets = "[]";
	static const std::string CBrackets = "{}";
	static const std::vector<std::string> ComplexOperators = { "==", "!=", "<=", ">=" };
	static const std::vector<std::string> CommentOperator = { "//", "///" };
	
	enum TokenType
	{
		Undefined,
		Operator,
		Identifier,
		Number,
		Assignment,
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
		IF_TYPE(Assignment, t)
		IF_TYPE(Bracket, t)
		IF_TYPE(QBracket, t)
		IF_TYPE(CBracket, t)
		IF_TYPE(Quote, t)
		IF_TYPE(Semicolon, t)
		IF_TYPE(Comment, t)
		IF_TYPE(Delimiter, t)
		return TYPENAME(Undefined);
	}

	struct Token
	{
		std::string value;
		TokenType type;
	};
	typedef std::vector<Token> TokenList;

	inline std::string make_string_from_tokens(TokenList::const_iterator from, TokenList::const_iterator to)
	{
		std::string str;
		for (; from != to; ++from)
		{
			str.append(from->value);
		}
		return str;
	}
}



typedef std::variant<int, float, std::string, bool> Value;
struct Variable
{
	std::string identifier;
	Value value;
};

class ValueException : public Utils::BaseException
{
public:
	ValueException() {}
	ValueException(const char* msg)
	{
		m_msg.assign(msg);
	}
	ValueException(std::string const& str)
	{
		m_msg = str;
	}
	virtual ~ValueException() = default;
};
