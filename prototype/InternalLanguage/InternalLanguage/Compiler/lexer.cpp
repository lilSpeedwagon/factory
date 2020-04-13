#include "stdafx.h"
#include "lexer.h"

#include <algorithm>

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

		// skip EOF
		if (*tokenEnd < 0)
			continue;
		
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
		case CBracket:
		{
			addToken(tokenBegin, tokenEnd, Tokens::CBracket);
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
	int numOfDots = std::count<StrIter>(begin, end, '.');
	int numOfAlphas = std::count_if<StrIter>(begin, end, isalpha);
	
	if (isLetterFirst && numOfDots == 0)
	{
		return Tokens::Identifier;
	}

	if (numOfAlphas == 0 && (numOfDots == 1 || numOfDots == 0))
	{
		return Tokens::Number;
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
	
	if (Tokens::CBrackets.find(c) != std::string::npos)
		return CBracket;
	
	if (c == '\"')
		return Quote;

	if (c == ';')
		return Semicolon;

	if (c == '/')
		return Slash;
	
	return Undefined;
}
