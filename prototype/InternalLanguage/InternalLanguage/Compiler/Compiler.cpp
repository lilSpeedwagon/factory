// Compiler.cpp: определяет экспортированные функции для приложения DLL.
//

#include "stdafx.h"
#include "Definitions.h"
#include "Compiler.h"
#include "lexer.h"
#include "Syntaxer.h"

bool compileInternal(const char* code, LogDelegate log)
{
	std::string strCode(code);
	
	Lexer lex(strCode, log);
	bool lextResult = lex.Run();
	if (!lextResult)
	{
		log("Cannot continue compilation with undefined tokens. Abort.");
		return false;
	}
	const Tokens::TokenList& tokens = lex.Result();
	
	Syntaxer syntaxer(tokens, log);
	syntaxer.Run();

	return true;
}

bool __declspec(dllexport) __stdcall Compile(const char * code, LogDelegate log)
{
	return compileInternal(code, log);
}

