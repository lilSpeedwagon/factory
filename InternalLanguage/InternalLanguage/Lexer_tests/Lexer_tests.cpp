#include "catch.hpp"
#include "Lexer.h"

LogDelegate logMock = [](const char*) {};

using namespace Tokens;

typedef std::vector<std::pair<std::string, TokenList>> InputsOuputs;

void testLexerIO(InputsOuputs const& ios)
{
	for (auto& io : ios)
	{
		Lexer lex(io.first, logMock);
		bool result = lex.Run();
		REQUIRE(result == true);
		auto tokens = lex.Result();
		REQUIRE(tokens.size() == io.second.size());
		for (size_t i = 0; i < tokens.size(); i++)
		{
			const Token& tExpected = tokens.at(i);
			const Token& tInFact = io.second.at(i);
			INFO("Matching tokens: \"" << tExpected.value << "\" == \"" << io.second.at(i).value << "\"");
			REQUIRE(tExpected.value == tInFact.value);
			INFO("Expected type: " << GetTypeName(tExpected.type) << "; In fact: " << GetTypeName(tInFact.type));
			REQUIRE(tExpected.type == tInFact.type);
		}
	}
}

TEST_CASE("Empty input", "[Lexer]")
{
	Lexer lex("", logMock);
	bool result = lex.Run();
	REQUIRE_FALSE(result);
}

TEST_CASE("One string input", "[Lexer]")
{
	const InputsOuputs inputs_outputs = {
		{ "token", {{ "token", Identifier }} },
		{ "a+b", {{ "a", Identifier }, {"+", Operator}, {"b", Identifier}} },
		{ "id;", {{ "id", Identifier }, {";", Semicolon}} },
		{ " ",  {{ " ", Delimiter }} },
		{ "a=1;", {{ "a", Identifier }, {"=", Assignment}, {"1", Number}, {";", Semicolon}} },
	};
	testLexerIO(inputs_outputs);
}
