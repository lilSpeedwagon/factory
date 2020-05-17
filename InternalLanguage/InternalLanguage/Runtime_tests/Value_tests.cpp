#include "catch.hpp"
#include "Runtime.h"

using namespace runtime;

TEST_CASE("Value default constructor", "[Value]")
{
	Value val;
	REQUIRE(val.getType() == Value::Integer);
	REQUIRE(val.getValue<int>() == 0);
	REQUIRE(val.getValue<char*>() == nullptr);
	REQUIRE(val.getValue<float>() == 0.0f);
	REQUIRE(val.getValue<bool>() == false);
}

TEST_CASE("Value assign operator", "[Value]")
{
	SECTION("Integer")
	{
		Value val = 10;
		REQUIRE(val.getType() == Value::Integer);
		CHECK(val.getValue<int>() == 10);
	}

	SECTION("Float")
	{
		Value val = 10.0f;
		REQUIRE(val.getType() == Value::Float);
		CHECK(val.getValue<float>() == 10.0f);
	}

	SECTION("Boolean")
	{
		Value val = true;
		REQUIRE(val.getType() == Value::Boolean);
		CHECK(val.getValue<bool>() == true);
	}

	SECTION("String")
	{
		Value val(std::string("String"));
		REQUIRE(val.getType() == Value::String);
		CHECK(val.getValue<std::string>() == "String");
	}
}

TEST_CASE("Value copy constructors", "[Value]")
{
	SECTION("Integer")
	{
		Value valInt = 10;
		
		Value val = valInt;
		REQUIRE(val.getType() == Value::Integer);
		CHECK(val.getValue<int>() == 10);
	}
	
	SECTION("Float")
	{
		Value valFloat = 10.0f;

		Value val = valFloat;
		REQUIRE(val.getType() == Value::Float);
		CHECK(val.getValue<float>() == 10.0f);
	}

	SECTION("Boolean")
	{
		Value valBool = true;

		Value val = valBool;
		REQUIRE(val.getType() == Value::Boolean);
		CHECK(val.getValue<bool>() == true);
	}

	SECTION("String")
	{
		Value valStr(std::string("String"));
		
		Value val = valStr;
		REQUIRE(val.getType() == Value::String);
		CHECK(val.getValue<std::string>() == "String");
	}
}

TEST_CASE("Value operator!", "[Value]")
{
	CHECK(Value().operator!().getType() == Value::Boolean);
	CHECK(Value().operator!().getValue<bool>() == true);
	CHECK(Value(true).operator!().getValue<bool>() == false);
	CHECK(Value(0).operator!().getValue<bool>() == true);
	CHECK(Value(1).operator!().getValue<bool>() == false);
	CHECK(Value(0.0f).operator!().getValue<bool>() == true);
	CHECK(Value(-1.0f).operator!().getValue<bool>() == false);
	CHECK(Value(std::string("hallo")).operator!().getValue<bool>() == false);
	CHECK(Value(std::string("")).operator!().getValue<bool>() == true);
}

TEST_CASE("Value cast", "[Value]")
{
	SECTION("Integer")
	{
		Value val = 10;
		
		Value intVal = val.toInt();
		REQUIRE(intVal.getType() == Value::Integer);
		CHECK(intVal.getValue<int>() == 10);
		
		Value floatVal = val.toFloat();
		REQUIRE(floatVal.getType() == Value::Float);
		CHECK(floatVal.getValue<float>() == 10.0f);
		
		Value boolVal = val.toBool();
		REQUIRE(boolVal.getType() == Value::Boolean);
		CHECK(boolVal.getValue<bool>() == true);
		
		Value stringVal = val.toString();
		REQUIRE(stringVal.getType() == Value::String);
		CHECK(stringVal.getValue<std::string>() == "10");
	}

	SECTION("Float")
	{
		Value val = 10.0f;

		Value intVal = val.toInt();
		REQUIRE(intVal.getType() == Value::Integer);
		CHECK(intVal.getValue<int>() == 10);

		Value floatVal = val.toFloat();
		REQUIRE(floatVal.getType() == Value::Float);
		CHECK(floatVal.getValue<float>() == 10.0f);

		Value boolVal = val.toBool();
		REQUIRE(boolVal.getType() == Value::Boolean);
		CHECK(boolVal.getValue<bool>() == true);

		Value stringVal = val.toString();
		REQUIRE(stringVal.getType() == Value::String);
		CHECK(stringVal.getValue<std::string>() == "10.000000");
	}

	SECTION("Boolean")
	{
		Value val = true;

		Value intVal = val.toInt();
		REQUIRE(intVal.getType() == Value::Integer);
		CHECK(intVal.getValue<int>() == 1);

		Value floatVal = val.toFloat();
		REQUIRE(floatVal.getType() == Value::Float);
		CHECK(floatVal.getValue<float>() == 1.0f);

		Value boolVal = val.toBool();
		REQUIRE(boolVal.getType() == Value::Boolean);
		CHECK(boolVal.getValue<bool>() == true);

		Value stringVal = val.toString();
		REQUIRE(stringVal.getType() == Value::String);
		CHECK(stringVal.getValue<std::string>() == "true");
	}

	SECTION("String")
	{
		SECTION("empty")
		{
			Value val(std::string(""));

			Value intVal = val.toInt();
			REQUIRE(intVal.getType() == Value::Integer);
			CHECK(intVal.getValue<int>() == 0);

			Value floatVal = val.toFloat();
			REQUIRE(floatVal.getType() == Value::Float);
			CHECK(floatVal.getValue<float>() == 0.0f);

			Value boolVal = val.toBool();
			REQUIRE(boolVal.getType() == Value::Boolean);
			CHECK(boolVal.getValue<bool>() == false);

			Value stringVal = val.toString();
			REQUIRE(stringVal.getType() == Value::String);
			CHECK(stringVal.getValue<std::string>() == "");
		}
		
		SECTION("string")
		{
			Value val(std::string("string"));

			Value intVal = val.toInt();
			REQUIRE(intVal.getType() == Value::Integer);
			CHECK(intVal.getValue<int>() == 0);

			Value floatVal = val.toFloat();
			REQUIRE(floatVal.getType() == Value::Float);
			CHECK(floatVal.getValue<float>() == 0.0f);

			Value boolVal = val.toBool();
			REQUIRE(boolVal.getType() == Value::Boolean);
			CHECK(boolVal.getValue<bool>() == true);

			Value stringVal = val.toString();
			REQUIRE(stringVal.getType() == Value::String);
			CHECK(stringVal.getValue<std::string>() == "string");
		}
		
		SECTION("numeric")
		{
			Value val(std::string("10.5"));
			
			Value intVal = val.toInt();
			REQUIRE(intVal.getType() == Value::Integer);
			CHECK(intVal.getValue<int>() == 10);

			Value floatVal = val.toFloat();
			REQUIRE(floatVal.getType() == Value::Float);
			CHECK(floatVal.getValue<float>() == 10.5f);

			Value boolVal = val.toBool();
			REQUIRE(boolVal.getType() == Value::Boolean);
			CHECK(boolVal.getValue<bool>() == true);

			Value stringVal = val.toString();
			REQUIRE(stringVal.getType() == Value::String);
			CHECK(stringVal.getValue<std::string>() == "10.5");
		}
		
	}
}

TEST_CASE("Value operator+", "[Value]")
{
	SECTION("Integer")
	{
		Value v1 = 10;
		Value v2 = 5;
		Value res = v1 + v2;
		REQUIRE(res.getType() == Value::Integer);
		CHECK(res.getValue<int>() == 15);
	}

	SECTION("Float")
	{
		Value v1 = 10.0f;
		Value v2 = 5.5f;
		Value res = v1 + v2;
		REQUIRE(res.getType() == Value::Float);
		CHECK(res.getValue<float>() == 15.5f);
	}

	SECTION("Boolean")
	{
		Value v1 = true;
		Value v2 = false;
		Value res = v1 + v2;
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
	}

	SECTION("String")
	{
		Value v1(std::string("Hello"));
		Value v2(std::string("world"));
		Value res = v1 + v2;
		REQUIRE(res.getType() == Value::String);
		CHECK(res.getValue<std::string>() == "Helloworld");
	}

	SECTION("Mixed")
	{
		Value intVal = 10;
		Value floatVal = 5.5f;
		Value boolVal = true;
		Value stringVal(std::string("str"));

		Value v1 = intVal + floatVal;
		REQUIRE(v1.getType() == Value::Float);
		CHECK(v1.getValue<float>() == 15.5f);

		Value v2 = intVal + boolVal;
		REQUIRE(v2.getType() == Value::Integer);
		CHECK(v2.getValue<int>() == 11);

		Value v3 = intVal + stringVal;
		REQUIRE(v3.getType() == Value::String);
		CHECK(v3.getValue<std::string>() == "10str");

		Value v4 = floatVal + boolVal;
		REQUIRE(v4.getType() == Value::Float);
		CHECK(v4.getValue<float>() == 6.5f);

		Value v5 = floatVal + stringVal;
		REQUIRE(v5.getType() == Value::String);
		CHECK(v5.getValue<std::string>() == "5.500000str");

		Value v6 = boolVal + stringVal;
		REQUIRE(v6.getType() == Value::String);
		CHECK(v6.getValue<std::string>() == "truestr");
	}	
}