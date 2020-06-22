#include "catch.hpp"
#include "lexer.h"
#include <sstream>

LogDelegate logMock = [](const char*) {};

using namespace Tokens;

struct TestData
{
	std::string input;
	bool isSuccessExpected;
	TokenList expectedOutput;
};
typedef std::vector<TestData> TestParams;

std::string getTestInfo(TestData const& td, TokenList const& actual)
{
	std::stringstream ss;
	ss << "Expected output: ";
	for (auto t : td.expectedOutput)
	{
		ss << "\"" << t.value << "\" [" << GetTypeName(t.type) << "], ";
	}
	ss << ";\nActual output: ";
	for (auto t : actual)
	{
		ss << "\"" << t.value << "\" [" << GetTypeName(t.type) << "], ";
	}
	ss << ";";
	return ss.str();
}

void testLexerIO(TestParams const& tp)
{
	for (auto& td : tp)
	{
		INFO("Lexer input: " << td.input);
		Lexer lex(td.input, logMock);
		bool result = lex.Run();
		REQUIRE(result == td.isSuccessExpected);
		if (td.isSuccessExpected)
		{
			auto tokens = lex.Result();
			INFO(getTestInfo(td, tokens));
			REQUIRE(tokens.size() == td.expectedOutput.size());
			for (size_t i = 0; i < tokens.size(); i++)
			{
				const Token& tExpected = td.expectedOutput.at(i);
				const Token& tActual = tokens.at(i);
				REQUIRE(tExpected.value == tActual.value);
				REQUIRE(tExpected.type == tActual.type);
			}
		}
	}
}

TEST_CASE("Empty input", "[Lexer]")
{
	const TestParams params = {{ "", false, {}} };
	testLexerIO(params);
}

TEST_CASE("One string input", "[Lexer]")
{
	const TestParams params = {
		{ "token", true, {{ "token", Identifier }} },
		{ "a+b", true, {{ "a", Identifier }, {"+", Operator}, {"b", Identifier}} },
		{ "id;", true, {{ "id", Identifier }, {";", Semicolon}} },
		{ " ", true, {{ " ", Delimiter }} },
		{ "a=1;", true, {{ "a", Identifier }, {"=", Assignment}, {"1", Number}, {";", Semicolon}} },
	};
	testLexerIO(params);
}

TEST_CASE("Comment removing", "[Lexer]")
{
	const TestParams params = {
		{ "//", true, {} },
		{ "//a+b", true, {} },
		{ "a//a", true, {{ "a", Identifier }} },
		{ "a//b\nc", true, {{ "a", Identifier }, {"c", Identifier}} },
		{ "//a\nb\n//\n", true, {{ "b", Identifier }, {"\n", Delimiter}} },
	};
	testLexerIO(params);
}

TEST_CASE("Delimeters", "[Lexer]")
{
	const TestParams params = {
		{ "   ", true, {{"   ", Delimiter}} },
		{ " \t\n", true, {{" \t\n", Delimiter }} },
		{ "abc\nabc", true, {{ "abc", Identifier }, {"\n", Delimiter}, {"abc", Identifier}} },
		{ "1 2", true, {{ "1", Number}, {" ", Delimiter}, {"2", Number}} },
	};
	testLexerIO(params);
}

TEST_CASE("Numbers in identifiers", "[Lexer]")
{
	const TestParams params = {
		{ "id 123", true, {{"id", Identifier}, {" ", Delimiter}, {"123", Number}} },
		{ "id123", true, {{"id123", Identifier }} },
		{ "123id", false, {}}
	};
	testLexerIO(params);
}

TEST_CASE("Arithmetic", "[Lexer]")
{
	const TestParams params = {
		{ "a=1", true, {{"a", Identifier}, {"=", Assignment}, {"1", Number}} },
		{ "a==1", true, {{"a", Identifier}, {"==", Operator}, {"1", Number}} },
		{ "b=a/5", true, {{ "b", Identifier }, {"=", Assignment}, {"a", Identifier}, {"/", Operator}, {"5", Number}} },
		{ ">= <= !=", true, { {">=", Operator}, {" ", Delimiter}, {"<=", Operator}, {" ", Delimiter}, {"!=", Operator}}},
		{ "+- /* %/", false, {} },
		{ "+-*/%", false, {} },
	};
	testLexerIO(params);
}

TEST_CASE("Brackets", "[Lexer]")
{
	const TestParams params = {
		{ "()", true, {{"(", Bracket}, {")", Bracket}} },
		{ "(((", true, {{"(", Bracket }, {"(", Bracket}, {"(", Bracket}} },
		{ "(a)(b)", true, {{"(", Bracket }, {"a", Identifier}, {")", Bracket}, {"(", Bracket}, {"b", Identifier}, {")", Bracket}}},
		{ "a[i]", true, {{"a", Identifier}, {"[", QBracket}, {"i", Identifier}, {"]", QBracket}} },
		{ "([{", true, {{"(", Bracket }, {"[", QBracket}, {"{", CBracket}} },
	};
	testLexerIO(params);
}

TEST_CASE("Quotes", "[Lexer]")
{
	const TestParams params = {
		{ "\"quoted\"", true, {{"\"quoted\"", Quote}} },
		{ "\"\na123+-=({[_,.!\"", true, {{"\"\na123+-=({[_,.!\"", Quote}} },
		{ "\"\"", true, {{"\"\"", Quote }} },
		{ "\"", false, {} },
		{ "\"q\" \"", false, {} },
	};
	testLexerIO(params);
}

TEST_CASE("Common", "[Lexer]")
{
	const TestParams params = {
		{ "a = 1 + (b == false);\n print(a);", true, {{"a", Identifier}, {" ", Delimiter}, {"=", Assignment},
			{" ", Delimiter}, {"1", Number}, {" ", Delimiter}, {"+", Operator}, {" ", Delimiter}, {"(", Bracket},
			{"b", Identifier}, {" ", Delimiter}, {"==", Operator}, {" ", Delimiter}, {"false", Identifier}, {")", Bracket},
			{";", Semicolon}, {"\n ", Delimiter}, {"print", Identifier}, {"(", Bracket}, {"a", Identifier}, {")", Bracket},
			{";", Semicolon}} },
		{ "while(true)\n{\npoop();\n}", true, { {"while", Identifier}, {"(", Bracket}, {"true", Identifier}, {")", Bracket},
			{"\n", Delimiter}, {"{", CBracket}, {"\n", Delimiter}, {"poop", Identifier}, {"(", Bracket}, {")", Bracket},
			{";", Semicolon}, {"\n", Delimiter}, {"}", CBracket}} },
	};
	testLexerIO(params);
}