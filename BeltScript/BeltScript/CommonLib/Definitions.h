#pragma once
#include <string>
#include <vector>
#include <array>

#include "Utils.h"

typedef void(__stdcall *LogDelegate)(const char*);

namespace Tokens
{
	static const std::string Delimiters = " \r\n\t";
	static const std::string AdditionToIdentifiers = "_.";
	static const std::string Operators = "+-/*%><=!";
	static const std::string Commas = ",";
	static const std::string Brackets = "()";
	static const std::string QBrackets = "[]";
	static const std::string CBrackets = "{}";
	static const std::vector<std::string> ComplexOperators = { "==", "!=", "<=", ">=", "&&", "||" };
	static const std::vector<std::string> CommentOperator = { "//", "///" };
	
	enum TokenType
	{
		Undefined,
		Operator,
		Identifier,
		Number,
		Assignment,
		Comma,
		Bracket,
		QBracket,
		CBracket,
		Quote,
		Semicolon,
		Comment,
		Delimiter
	};

#define IF_TYPE(type_name, t) { if ((type_name) == (t)) { return TYPENAME(type_name); }}
	
	inline std::string GetTypeName(TokenType t)
	{
		IF_TYPE(Operator, t)
		IF_TYPE(Identifier, t)
		IF_TYPE(Number, t)
		IF_TYPE(Assignment, t)
		IF_TYPE(Comma, t)
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
	inline bool operator==(Token const& l_val, Token const& r_val) { return l_val.type == r_val.type && l_val.value == r_val.value; }
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

namespace Operators
{
	typedef int Priority;
	static const Priority max_priority = INT32_MAX;
	static const Priority min_priority = 0;
	static const Priority brackets_priority = 50;
	
	template<int Size>
	bool isOperator(std::array<const char*, Size> const& vars, std::string const& str)
	{
		return std::find(vars.begin(), vars.end(), str) != vars.end();
	}

	constexpr std::pair LogicalOr =			{ std::array{ "||" },											5 };
	constexpr std::pair LogicalAnd =		{ std::array{ "&&" },											10 };
	constexpr std::pair Comparison =		{ std::array{ "==", "!=" },								15 };
	constexpr std::pair MoreLess =			{ std::array{ ">=", "<=", ">", "<" },	20 };
	constexpr std::pair ArithmeticLow =		{ std::array{ "+", "-" },								25 };
	constexpr std::pair ArithmeticHigh =	{ std::array{ "*", "/", "%" },					30 };
	constexpr std::pair Not =				{ std::array{ "!", "-" },								35 };

	inline Priority operatorPriority(std::string const& str)
	{
		if (isOperator(LogicalOr.first, str))		{ return LogicalOr.second; }
		if (isOperator(LogicalAnd.first, str))		{ return LogicalAnd.second; }
		if (isOperator(Comparison.first, str))		{ return Comparison.second; }
		if (isOperator(MoreLess.first, str))		{ return MoreLess.second; }
		if (isOperator(ArithmeticLow.first, str))	{ return ArithmeticLow.second; }
		if (isOperator(ArithmeticHigh.first, str))	{ return ArithmeticHigh.second; }
		if (isOperator(Not.first, str))				{ return Not.second; }
		
		return -1;
	}

	constexpr std::array UnaryOperators = { "!", "-" };
	inline bool isUnaryOperator(std::string const& str)
	{
		return std::find(UnaryOperators.begin(), UnaryOperators.end(), str) != UnaryOperators.end();
	}
}

namespace KeyWords
{
	static const std::pair<std::string, std::string> IfElse = { "if", "else" };
	static const std::string Loop = "while";
	static const std::string Static = "static";
	constexpr std::array KeyWords = { "if", "else", "for", "true", "false", "static" };

	const static std::string BoolLiteralTrue = "true";
	const static std::string BoolLiteralFalse = "false";

	inline bool isStringLiteral(std::string const& str)
	{
		return Utils::isQuotedString(str);
	}

	inline bool isBoolLiteral(std::string const& str)
	{
		return str == BoolLiteralTrue || str == BoolLiteralFalse;
	}
}


