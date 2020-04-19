#include "stdafx.h"
#include "Syntaxer.h"


void Syntaxer::Init(Tokens::TokenList const& tokens)
{
	m_tokens = std::make_shared<Tokens::TokenList>(tokens);
}

void Syntaxer::Run()
{
	
}


