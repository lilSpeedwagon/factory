#include "pch.h"
#include "lexer.h"

#include <sstream>

const int Lexer::TOKEN_DENSITY = 2;

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
	m_result.reserve(m_strCode.length() / TOKEN_DENSITY);
	tokenize();
	m_result.shrink_to_fit();
	Log("done");

	std::stringstream ss;
	ss << m_tokensFound << " tokens found. " << m_undefinedTokensFound << " undefined.";
	Log(ss.str());

	result = m_undefinedTokensFound == 0;
	
	return result;
}

void Lexer::tokenize()
{
	StrIter tokenBegin = m_strCode.cbegin(), tokenEnd = m_strCode.cbegin();
	CharType prevType = getType(*tokenBegin);
	
	for (; tokenBegin != m_strCode.cend();)
	{
		if (tokenEnd == m_strCode.cend())
			break;

		++tokenEnd;
		CharType currentType = tokenEnd != m_strCode.cend() ? getType(*tokenEnd) : Undefined;

		// add token after its type definition
		switch (prevType)
		{
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
			// looking for next quote
			tokenEnd = std::find(tokenBegin + 1, m_strCode.cend(), '\"');
			if (tokenEnd != m_strCode.end())
			{
				++tokenEnd;
				addToken(tokenBegin, tokenEnd, Tokens::Quote);
				currentType = getType(*tokenEnd);
			}
			else
			{
				addToken(tokenBegin, tokenEnd, Tokens::Undefined);
			}
			break;
		}
		case Semicolon:
		{
			addToken(tokenBegin, tokenEnd, Tokens::Semicolon);
			break;
		}
		default:
		{				
			// add token if another type token found
			if (currentType != prevType)
			{
				switch (prevType)
				{
				case Delimiter:
				{
					addToken(tokenBegin, tokenEnd, Tokens::Delimiter);
					break;
				}
				case Operator:
				{
					// one char operator
					if (std::distance(tokenBegin, tokenEnd) == 1)
					{
						addToken(tokenBegin, tokenEnd, *tokenBegin == '=' ? Tokens::Assignment : Tokens::Operator);
					}
					else
					{
						std::string tokenStr(tokenBegin, tokenEnd);
						
						// check for complex operator...
						if (std::find(Tokens::ComplexOperators.cbegin(), Tokens::ComplexOperators.cend(), tokenStr) != Tokens::ComplexOperators.cend())
						{
							addToken(tokenBegin, tokenEnd, Tokens::Operator);
						}
						// or comment
						else if (std::find(Tokens::CommentOperator.cbegin(), Tokens::CommentOperator.cend(), tokenStr) != Tokens::CommentOperator.cend())
						{
							tokenEnd = std::find(tokenEnd, m_strCode.cend(), '\n');
							if (tokenEnd != m_strCode.cend())
							{
								++tokenEnd;
								if (tokenEnd != m_strCode.cend())
								{
									tokenBegin = tokenEnd;
									currentType = getType(*tokenBegin);
								}
							}
						}
						else
						{
							addToken(tokenBegin, tokenEnd, Tokens::Undefined);
						}
					}
					break;
				}
				case AlNum:
				{
					Tokens::TokenType type = getTokenType(tokenBegin, tokenEnd);
					addToken(tokenBegin, tokenEnd, type);
					break;
				}
				default:
					addToken(tokenBegin, tokenEnd, Tokens::Undefined);
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
	m_tokensFound++;
	if (type == Tokens::Undefined)
		m_undefinedTokensFound++;
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
	if (c < 0) // EOF
		return Undefined;
	
	if (isalnum(c) || Tokens::AdditionToIdentifiers.find(c) != std::string::npos)
		return AlNum;

	if (Tokens::Delimiters.find(c) != std::string::npos)
		return Delimiter;

	if (Tokens::Operators.find(c) != std::string::npos)
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
	
	return Undefined;
}
