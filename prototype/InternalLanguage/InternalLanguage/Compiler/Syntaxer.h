#pragma once

#include "stdafx.h"
#include "Definitions.h"
#include "Logger.h"

class Syntaxer : public Logger
{
public:
	Syntaxer() {}
	~Syntaxer() = default;

	void Init(Tokens::TokenList const& tokens);
	void Run();
	
private:
	std::shared_ptr<Tokens::TokenList> m_tokens;
};

