// Compiler.cpp: определяет экспортированные функции для приложения DLL.
//

#include "stdafx.h"
#include "Definitions.h"
#include "Compiler.h"
#include "lexer.h"
#include "Syntaxer.h"
#include "RuntimeLauncher.h"

//to remove  !!!
#include <iostream>


bool compileInternal(const char* code, LogDelegate log)
{
	std::string strCode(code);
	
	Lexer lex(strCode, log);
	const bool lextResult = lex.Run();
	if (!lextResult)
	{
		log("Cannot start compilation due founded undefined tokens. Abort.");
		return false;
	}
	const Tokens::TokenList& tokens = lex.Result();

	char c;  // to remove !!!
	std::cin >> c;
	
	Syntaxer syntaxer(tokens, log);
	const bool synResult = syntaxer.Run();
	if (!synResult)
	{
		log("Compilation failed.");
		return false;
	}
	log("Compilation is finished.");

	OperationScopePtr pOperationTree = syntaxer.GetResult();
	runtime::Runtime& runtime = runtime::Runtime::getInstance();
	runtime.setOutput(log);
	const bool runResult = runtime.run(pOperationTree);
	if (!runResult)
	{
		log("Running error.");
		return false;
	}
	log("Successfully executed.");

	return true;
}

bool __declspec(dllexport) __stdcall Compile(const char * code, LogDelegate log)
{
	try
	{
		return compileInternal(code, log);
	}
	catch(std::exception& e)
	{
		log(e.what());
	}
	catch(...)
	{
		log("Unexpected error occured.");
	}
	return false;
}

