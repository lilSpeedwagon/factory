// Compiler.cpp: определяет экспортированные функции для приложения DLL.
//

#include "stdafx.h"
#include "Definitions.h"
#include "Compiler.h"
#include "lexer.h"

void compileInternal(const char* code, LogDelegate log)
{
	std::string strCode(code);
	
	Lexer lex(strCode, log);
	lex.Run();
	const Tokens::TokenList& tokens = lex.Result();

	
}

void __declspec(dllexport) __stdcall Compile(const char * code, LogDelegate log)
{
	compileInternal(code, log);
}

