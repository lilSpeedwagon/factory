#include "stdafx.h"
#include "lexer.h"

void Lexer::Init(std::string const& code, LogDelegate& delegate)
{
	m_strCode = code;
	SetLogDelegate(delegate);
	SetLogName("Lexer");
}

bool Lexer::Run()
{
	bool result = false;

	Log("Running lexical analysis");
	m_result.clear();
	if (m_strCode.empty())
	{
		Log("Nothing to analyze");
		return result;
	}

	Log("Code: " + m_strCode);
	Log("Tokens:");
	tokenize();

	result = true;
	Log("done");
	
	return result;
}

void Lexer::tokenize()
{
	size_t length = m_strCode.length();

	StrIter tokenBegin = m_strCode.begin(), tokenEnd = m_strCode.begin();
	CharType prevType = getType(*tokenBegin);
	for (; tokenBegin != m_strCode.end();)
	{
		++tokenEnd;
		if (tokenEnd == m_strCode.end())
			return;
		
		CharType currentType = getType(*tokenEnd);

		switch (prevType)
		{
		case Operator:
		{
			addToken(tokenBegin, tokenEnd, *tokenBegin == '=' ? Tokens::Equality : Tokens::Operator);
			break;
		}
		case Bracket:
		{
			addToken(tokenBegin, tokenEnd, Tokens::Bracket);
			break;
		}
		case QBracket:
		{
			addToken(tokenBegin, tokenEnd, Tokens::QBracket);
			break;
		}
		case Quote:
		{
			addToken(tokenBegin, tokenEnd, Tokens::Quote);
			break;
		}
		case Semicolon:
		{
			addToken(tokenBegin, tokenEnd, Tokens::Semicolon);
			break;
		}
		default:
		{
			if (currentType != prevType)
			{
				switch (prevType)
				{
				case Delimiter:
				{
					addToken(tokenBegin, tokenEnd, Tokens::Delimiter);
					break;
				}
				case AlNum:
				{
					Tokens::TokenType type = getTokenType(tokenBegin, tokenEnd);
					addToken(tokenBegin, tokenEnd, type);
					break;
				}
				case Slash:
				{
					// skip comments
					if (std::distance(tokenEnd, tokenBegin) > 1)
					{
						tokenBegin = std::find<StrIter>(tokenEnd, m_strCode.end(), '\n');
						tokenEnd = tokenEnd;
					}
					break;
				}
				}
			}
		}
		}

		prevType = currentType;
	}
}

void Lexer::addToken(StrIter& begin, StrIter& end, Tokens::TokenType type)
{
	m_result.push_back({ std::string(begin, end), type});
	Log(("\"" + std::string(begin, end) + "\" type: " + Tokens::GetTypeName(type)).c_str());
	begin = end;
}

Tokens::TokenType Lexer::getTokenType(StrIter begin, StrIter end)
{
	std::string token(begin, end);
	
	bool isLetterFirst = isalpha(*begin) || *begin == '_';
	if (isLetterFirst)
	{
		if (token.find('.') == std::string::npos)
		{
			return Tokens::Identifier;
		}
		else
		{
			return Tokens::Undefined;
		}
	}

	try
	{
		std::stof(token.c_str());
		return Tokens::Number;
	}
	catch(std::invalid_argument&)
	{
		return Tokens::Undefined;
	}

	return Tokens::Undefined;
}

Lexer::CharType Lexer::getType(char c)
{
	if (isalnum(c) || Tokens::AdditionToIdentifiers.find(c) != std::string::npos)
		return AlNum;

	if (Tokens::Delimiters.find(c) != std::string::npos)
		return Delimiter;

	if (Tokens::Operators.find(c) != std::string::npos || c == '=')
		return Operator;

	if (Tokens::Brackets.find(c) != std::string::npos)
		return Bracket;

	if (Tokens::QBrackets.find(c) != std::string::npos)
		return QBracket;
	
	if (c == '\"')
		return Quote;

	if (c == ';')
		return Semicolon;

	if (c == '/')
		return Slash;
	
	return Undefined;
}
