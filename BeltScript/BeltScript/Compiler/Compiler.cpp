#include "stdafx.h"
#include "Compiler.h"
#include "lexer.h"
#include "Syntaxer.h"
#include "Serializer.h"


bool compileInternal(const char * fileName, const char* code, LogDelegate log)
{	
	Lexer lex(code, log);
	const bool lexResult = lex.Run();
	if (!lexResult)
	{
		log("Lexical analysis failed.");
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

extern "C"
{
	bool __declspec(dllexport) __stdcall Compile(const char * fileName, const char * code, LogDelegate log)
	{
		// don't let LogDelegate be empty
		if (log == nullptr)
		{
			log = [](const char*) {};
		}
		
		try
		{
			return compileInternal(fileName, code, log);
		}
		catch (std::exception& e)
		{
			log(e.what());
		}
		catch (...)
		{
			log("Unexpected error occured.");
		}
		return false;
	}
}