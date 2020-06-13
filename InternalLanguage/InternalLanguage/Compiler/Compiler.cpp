#include "stdafx.h"
#include "Compiler.h"
#include "lexer.h"
#include "Syntaxer.h"
#include "Serializer.h"


bool compileInternal(const char * fileName, const char* code, LogDelegate log)
{
	const std::string strFileName(fileName);
	const std::string strCode(code);
	
	Lexer lex(strCode, log);
	const bool lextResult = lex.Run();
	if (!lextResult)
	{
		log("Cannot start compilation due founded undefined tokens. Abort.");
		return false;
	}
	const Tokens::TokenList& tokens = lex.Result();
	
	Syntaxer syntaxer(tokens, log);
	const bool synResult = syntaxer.Run();
	if (!synResult)
	{
		log("Compilation failed.");
		return false;
	}
	OperationScopePtr pTree = syntaxer.GetResult();

	serializer::Serializer serializer(fileName, log);
	const bool serResult = serializer.Store(pTree);
	if (!serResult)
	{
		log("Storing to file failed.");
		return false;
	}
	
	log("Compilation is finished.");

	return true;
}

bool __declspec(dllexport) __stdcall Compile(const char * fileName, const char * code, LogDelegate log)
{
	try
	{
		return compileInternal(fileName, code, log);
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
