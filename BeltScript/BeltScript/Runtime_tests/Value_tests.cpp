#include "catch.hpp"
#include "Value.h"

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

	SECTION("Value")
	{
		SECTION("Integer")
		{
			Value val;
			val = Value(10);
			REQUIRE(val.getType() == Value::Integer);
			CHECK(val.getValue<int>() == 10);
		}

		SECTION("Float")
		{
			Value val;
			val = Value(10.0f);
			REQUIRE(val.getType() == Value::Float);
			CHECK(val.getValue<float>() == 10.0f);
		}

		SECTION("Boolean")
		{
			Value val;
			val = Value(true);
			REQUIRE(val.getType() == Value::Boolean);
			CHECK(val.getValue<bool>() == true);
		}

		SECTION("String")
		{
			Value val;
			val = Value(std::string("String"));
			REQUIRE(val.getType() == Value::String);
			CHECK(val.getValue<std::string>() == "String");
		}
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
			CHECK(floatVal.getValue<float>() == Approx(10.5f));

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
		CHECK(res.getValue<float>() == Approx(15.5f));
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
		CHECK(v1.getValue<float>() == Approx(15.5f));

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

TEST_CASE("Value operator-", "[Value]")
{
	SECTION("Integer")
	{
		Value v1 = 10;
		Value v2 = 5;
		Value res = v1 - v2;
		REQUIRE(res.getType() == Value::Integer);
		CHECK(res.getValue<int>() == 5);
	}

	SECTION("Float")
	{
		Value v1 = 10.0f;
		Value v2 = 5.5f;
		Value res = v1 - v2;
		REQUIRE(res.getType() == Value::Float);
		CHECK(res.getValue<float>() == Approx(4.5f));
	}

	SECTION("Boolean")
	{
		Value v1 = true;
		Value v2 = false;
		Value res = v1 - v2;
		REQUIRE(res.getType() == Value::Integer);
		CHECK(res.getValue<int>() == 1);
	}

	SECTION("String")
	{
		Value v1(std::string("Hello"));
		Value v2(std::string("world"));
		CHECK_THROWS_AS(v1 - v2, InvalidOperationException);
	}

	SECTION("Mixed")
	{
		Value intVal = 10;
		Value floatVal = 5.5f;
		Value boolVal = true;
		Value stringVal(std::string("str"));

		Value v1 = intVal - floatVal;
		REQUIRE(v1.getType() == Value::Float);
		CHECK(v1.getValue<float>() == Approx(4.5f));

		Value v2 = intVal - boolVal;
		REQUIRE(v2.getType() == Value::Integer);
		CHECK(v2.getValue<int>() == 9);

		Value v3 = floatVal - boolVal;
		REQUIRE(v3.getType() == Value::Float);
		CHECK(v3.getValue<float>() == Approx(4.5f));
		
		CHECK_THROWS_AS(intVal - stringVal, InvalidOperationException);
		CHECK_THROWS_AS(stringVal - floatVal, InvalidOperationException);
		CHECK_THROWS_AS(boolVal - stringVal, InvalidOperationException);
	}
}

TEST_CASE("Value operator *", "[Value]")
{
	SECTION("Integer")
	{
		Value v1 = 10;
		Value v2 = 5;
		Value res = v1 * v2;
		REQUIRE(res.getType() == Value::Integer);
		CHECK(res.getValue<int>() == 50);
	}

	SECTION("Float")
	{
		Value v1 = 10.0f;
		Value v2 = 5.5f;
		Value res = v1 * v2;
		REQUIRE(res.getType() == Value::Float);
		CHECK(res.getValue<float>() == Approx(55.0f));
	}

	SECTION("Boolean")
	{
		Value v1 = true;
		Value v2 = false;
		Value res = v1 * v2;
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == false);
	}

	SECTION("String")
	{
		Value v1(std::string("Hello"));
		Value v2(std::string("world"));
		CHECK_THROWS_AS(v1 * v2, InvalidOperationException);
	}

	SECTION("Mixed")
	{
		Value intVal = 10;
		Value floatVal = 5.5f;
		Value boolVal = true;
		Value stringVal(std::string("str"));

		Value v1 = intVal * floatVal;
		REQUIRE(v1.getType() == Value::Float);
		CHECK(v1.getValue<float>() == Approx(55.0f));

		Value v2 = intVal * boolVal;
		REQUIRE(v2.getType() == Value::Integer);
		CHECK(v2.getValue<int>() == 10);

		Value v3 = floatVal * boolVal;
		REQUIRE(v3.getType() == Value::Float);
		CHECK(v3.getValue<float>() == Approx(5.5f));

		Value v4 = intVal * stringVal;
		REQUIRE(v4.getType() == Value::String);
		CHECK(v4.getValue<std::string>() == "strstrstrstrstrstrstrstrstrstr");

		Value v5 = stringVal * intVal;
		REQUIRE(v5.getType() == Value::String);
		CHECK(v5.getValue<std::string>() == "strstrstrstrstrstrstrstrstrstr");

		CHECK_THROWS_AS(boolVal * stringVal, InvalidOperationException);
		CHECK_THROWS_AS(stringVal * floatVal, InvalidOperationException);
	}
}

TEST_CASE("Value operator/", "[Value]")
{
	SECTION("Integer")
	{
		Value v1 = 10;
		Value v2 = 5;
		Value res = v1 / v2;
		REQUIRE(res.getType() == Value::Float);
		CHECK(res.getValue<float>() == Approx(2.0f));
	}

	SECTION("Float")
	{
		Value v1 = 10.0f;
		Value v2 = 5.5f;
		Value res = v1 / v2;
		REQUIRE(res.getType() == Value::Float);
		CHECK(res.getValue<float>() == Approx(1.81818181818f));
	}

	SECTION("Boolean")
	{
		Value v1 = true;
		Value v2 = true;
		Value res = v1 / v2;
		REQUIRE(res.getType() == Value::Float);
		CHECK(res.getValue<float>() == Approx(1.0f));
	}

	SECTION("String")
	{
		Value v1(std::string("Hello"));
		Value v2(std::string("world"));
		CHECK_THROWS_AS(v1 / v2, InvalidOperationException);
	}

	SECTION("Mixed")
	{
		Value intVal = 10;
		Value floatVal = 5.5f;
		Value boolVal = true;
		Value stringVal(std::string("str"));

		Value v1 = intVal / floatVal;
		REQUIRE(v1.getType() == Value::Float);
		CHECK(v1.getValue<float>() == Approx(1.81818181818f));

		Value v2 = intVal / boolVal;
		REQUIRE(v2.getType() == Value::Float);
		CHECK(v2.getValue<float>() == Approx(10.0f));

		Value v3 = floatVal / boolVal;
		REQUIRE(v3.getType() == Value::Float);
		CHECK(v3.getValue<float>() == Approx(5.5f));

		CHECK_THROWS_AS(stringVal / intVal, InvalidOperationException);
		CHECK_THROWS_AS(stringVal / floatVal, InvalidOperationException);
		CHECK_THROWS_AS(stringVal / boolVal, InvalidOperationException);
		CHECK_THROWS_AS(boolVal / stringVal, InvalidOperationException);
		CHECK_THROWS_AS(floatVal / stringVal, InvalidOperationException);
		CHECK_THROWS_AS(intVal / stringVal, InvalidOperationException);
	}

	SECTION("Difivsion by zero")
	{
		CHECK_THROWS_AS(Value(10) / Value(0), DivisionByZeroException);
		CHECK_THROWS_AS(Value(10) / Value(false), DivisionByZeroException);
		CHECK_THROWS_AS(Value(10) / Value(0.0f), DivisionByZeroException);
	}
}

TEST_CASE("Value operator%", "[Value]")
{
	SECTION("Integer")
	{
		Value v1 = 10;
		Value v2 = 4;
		Value res = v1 % v2;
		REQUIRE(res.getType() == Value::Integer);
		CHECK(res.getValue<int>() == 2);
	}

	SECTION("Float")
	{
		CHECK_THROWS_AS(Value(10.0f) % Value(4.0f), InvalidOperationException);
	}

	SECTION("Boolean")
	{
		CHECK_THROWS_AS(Value(true) % Value(false), InvalidOperationException);
	}

	SECTION("String")
	{
		Value v1(std::string("Hello"));
		Value v2(std::string("world"));
		CHECK_THROWS_AS(v1 % v2, InvalidOperationException);
	}

	SECTION("Mixed")
	{
		Value intVal = 10;
		Value floatVal = 5.5f;
		Value boolVal = true;
		Value stringVal(std::string("str"));

		Value v1 = intVal % boolVal;
		REQUIRE(v1.getType() == Value::Integer);
		CHECK(v1.getValue<int>() == 0);

		Value v2 = boolVal % intVal;
		REQUIRE(v1.getType() == Value::Integer);
		CHECK(v1.getValue<int>() == 0);

		CHECK_THROWS_AS(intVal % floatVal, InvalidOperationException);
		CHECK_THROWS_AS(intVal % stringVal, InvalidOperationException);
		CHECK_THROWS_AS(floatVal % intVal, InvalidOperationException);
		CHECK_THROWS_AS(floatVal % boolVal, InvalidOperationException);
		CHECK_THROWS_AS(floatVal % stringVal, InvalidOperationException);
		CHECK_THROWS_AS(boolVal % floatVal, InvalidOperationException);
		CHECK_THROWS_AS(boolVal % stringVal, InvalidOperationException);
		CHECK_THROWS_AS(stringVal % intVal, InvalidOperationException);
		CHECK_THROWS_AS(stringVal % floatVal, InvalidOperationException);
		CHECK_THROWS_AS(stringVal % boolVal, InvalidOperationException);
	}
}

TEST_CASE("Value operator==", "[Value]")
{
	SECTION("Integer")
	{
		Value res = Value(10) == Value(5);
		Value res2 = Value(1) == Value(1);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == false);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == true);
	}

	SECTION("Float")
	{
		Value res = Value(10.0f) == Value(5.0f);
		Value res2 = Value(1.0f) == Value(1.0f);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == false);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == true);
	}

	SECTION("Boolean")
	{
		Value res = Value(true) == Value(false);
		Value res2 = Value(true) == Value(true);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == false);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == true);
	}

	SECTION("String")
	{
		Value res = Value(std::string("str")) == Value(std::string("abc"));
		Value res2 = Value(std::string("str")) == Value(std::string("str"));
		Value res3 = Value(std::string("")) == Value(std::string(""));
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == false);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == true);
		REQUIRE(res3.getType() == Value::Boolean);
		CHECK(res3.getValue<bool>() == true);
	}

	SECTION("Mixed")
	{
		Value v1 = Value(10) == Value(10.0f);
		REQUIRE(v1.getType() == Value::Boolean);
		CHECK(v1.getValue<bool>() == true);

		Value v2 = Value(5) == Value(5.1f);
		REQUIRE(v2.getType() == Value::Boolean);
		CHECK(v2.getValue<bool>() == false);

		Value v3 = Value(true) == Value(1);
		REQUIRE(v3.getType() == Value::Boolean);
		CHECK(v3.getValue<bool>() == true);

		Value v4 = Value(true) == Value(1.0f);
		REQUIRE(v4.getType() == Value::Boolean);
		CHECK(v4.getValue<bool>() == true);

		Value v5 = Value(false) == Value(-1);
		REQUIRE(v5.getType() == Value::Boolean);
		CHECK(v5.getValue<bool>() == false);

		Value v6 = Value(true) == Value(1.1f);
		REQUIRE(v6.getType() == Value::Boolean);
		CHECK(v6.getValue<bool>() == false);

		Value v7 = Value(std::string("true")) == Value(true);
		REQUIRE(v7.getType() == Value::Boolean);
		CHECK(v7.getValue<bool>() == true);

		Value v8 = Value(std::string("true")) == Value(false);
		REQUIRE(v8.getType() == Value::Boolean);
		CHECK(v8.getValue<bool>() == false);

		Value v9 = Value(std::string("-10")) == Value(10);
		REQUIRE(v9.getType() == Value::Boolean);
		CHECK(v9.getValue<bool>() == false);

		Value v10 = Value(std::string("10.500000")) == Value(10.5f);
		REQUIRE(v10.getType() == Value::Boolean);
		CHECK(v10.getValue<bool>() == true);
	}
}

TEST_CASE("Value operator<", "[Value]")
{
	SECTION("Integer")
	{
		Value res = Value(10) < Value(15);
		Value res2 = Value(1) < Value(1);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == false);
	}

	SECTION("Float")
	{
		Value res = Value(10.0f) < Value(5.0f);
		Value res2 = Value(1.0f) < Value(1.0f);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == false);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == false);
	}

	SECTION("Boolean")
	{
		Value res = Value(false) < Value(true);
		Value res2 = Value(true) < Value(true);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == false);
	}

	SECTION("String")
	{
		Value res = Value(std::string("abc")) < Value(std::string("abd"));
		Value res2 = Value(std::string("str")) < Value(std::string("str"));
		Value res3 = Value(std::string("")) < Value(std::string(""));
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == false);
		REQUIRE(res3.getType() == Value::Boolean);
		CHECK(res3.getValue<bool>() == false);
	}

	SECTION("Mixed")
	{
		Value v1 = Value(10) < Value(10.1f);
		REQUIRE(v1.getType() == Value::Boolean);
		CHECK(v1.getValue<bool>() == true);

		Value v2 = Value(6) < Value(5.1f);
		REQUIRE(v2.getType() == Value::Boolean);
		CHECK(v2.getValue<bool>() == false);

		Value v3 = Value(true) < Value(2);
		REQUIRE(v3.getType() == Value::Boolean);
		CHECK(v3.getValue<bool>() == true);

		Value v4 = Value(false) < Value(-1.0f);
		REQUIRE(v4.getType() == Value::Boolean);
		CHECK(v4.getValue<bool>() == false);

		Value v7 = Value(std::string("true")) < Value(true);
		REQUIRE(v7.getType() == Value::Boolean);
		CHECK(v7.getValue<bool>() == false);

		Value v8 = Value(std::string("abc")) < Value(false);
		REQUIRE(v8.getType() == Value::Boolean);
		CHECK(v8.getValue<bool>() == true);

		Value v9 = Value(std::string("-11")) < Value(-10);
		REQUIRE(v9.getType() == Value::Boolean);
		CHECK(v9.getValue<bool>() == false);

		Value v10 = Value(std::string("10.4")) < Value(10.5f);
		REQUIRE(v10.getType() == Value::Boolean);
		CHECK(v10.getValue<bool>() == true);
	}
}

TEST_CASE("Value operator&&", "[Value]")
{
	SECTION("Integer")
	{
		Value res = Value(10) && Value(5);
		Value res2 = Value(1) && Value(0);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == false);
	}

	SECTION("Float")
	{
		Value res = Value(10.0f) && Value(5.0f);
		Value res2 = Value(1.0f) && Value(0.0f);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == false);
	}

	SECTION("Boolean")
	{
		Value res = Value(true) && Value(false);
		Value res2 = Value(true) && Value(true);
		Value res3 = Value(false) && Value(false);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == false);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == true);
		REQUIRE(res3.getType() == Value::Boolean);
		CHECK(res3.getValue<bool>() == false);
	}

	SECTION("String")
	{
		Value res = Value(std::string("str")) && Value(std::string("abc"));
		Value res2 = Value(std::string("str")) && Value(std::string(""));
		Value res3 = Value(std::string("")) && Value(std::string(""));
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == false);
		REQUIRE(res3.getType() == Value::Boolean);
		CHECK(res3.getValue<bool>() == false);
	}

	SECTION("Mixed")
	{
		Value v1 = Value(10) && Value(10.0f);
		REQUIRE(v1.getType() == Value::Boolean);
		CHECK(v1.getValue<bool>() == true);

		Value v2 = Value(true) && Value(1);
		REQUIRE(v2.getType() == Value::Boolean);
		CHECK(v2.getValue<bool>() == true);

		Value v3 = Value(true) && Value(1.0f);
		REQUIRE(v3.getType() == Value::Boolean);
		CHECK(v3.getValue<bool>() == true);

		Value v4 = Value(false) && Value(-1);
		REQUIRE(v4.getType() == Value::Boolean);
		CHECK(v4.getValue<bool>() == false);

		Value v5 = Value(true) && Value(0.0f);
		REQUIRE(v5.getType() == Value::Boolean);
		CHECK(v5.getValue<bool>() == false);

		Value v6 = Value(std::string("true")) && Value(false);
		REQUIRE(v6.getType() == Value::Boolean);
		CHECK(v6.getValue<bool>() == false);
		
		Value v7 = Value(std::string("true")) && Value(true);
		REQUIRE(v7.getType() == Value::Boolean);
		CHECK(v7.getValue<bool>() == true);
	}
}


TEST_CASE("Value operator||", "[Value]")
{
	SECTION("Integer")
	{
		Value res = Value(10) || Value(5);
		Value res2 = Value(1) || Value(0);
		Value res3 = Value(0) || Value(0);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == true);
		REQUIRE(res3.getType() == Value::Boolean);
		CHECK(res3.getValue<bool>() == false);
	}

	SECTION("Float")
	{
		Value res = Value(10.0f) || Value(5.0f);
		Value res2 = Value(1.0f) || Value(0.0f);
		Value res3 = Value(0.0f) || Value(0.0f);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == true);
		REQUIRE(res3.getType() == Value::Boolean);
		CHECK(res3.getValue<bool>() == false);
	}

	SECTION("Boolean")
	{
		Value res = Value(true) || Value(false);
		Value res2 = Value(true) || Value(true);
		Value res3 = Value(false) || Value(false);
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == true);
		REQUIRE(res3.getType() == Value::Boolean);
		CHECK(res3.getValue<bool>() == false);
	}

	SECTION("String")
	{
		Value res = Value(std::string("str")) || Value(std::string("abc"));
		Value res2 = Value(std::string("str")) || Value(std::string(""));
		Value res3 = Value(std::string("")) || Value(std::string(""));
		REQUIRE(res.getType() == Value::Boolean);
		CHECK(res.getValue<bool>() == true);
		REQUIRE(res2.getType() == Value::Boolean);
		CHECK(res2.getValue<bool>() == true);
		REQUIRE(res3.getType() == Value::Boolean);
		CHECK(res3.getValue<bool>() == false);
	}

	SECTION("Mixed")
	{
		Value v1 = Value(10) || Value(10.0f);
		REQUIRE(v1.getType() == Value::Boolean);
		CHECK(v1.getValue<bool>() == true);

		Value v2 = Value(true) || Value(1);
		REQUIRE(v2.getType() == Value::Boolean);
		CHECK(v2.getValue<bool>() == true);

		Value v3 = Value(true) || Value(1.0f);
		REQUIRE(v3.getType() == Value::Boolean);
		CHECK(v3.getValue<bool>() == true);

		Value v4 = Value(false) || Value(-1);
		REQUIRE(v4.getType() == Value::Boolean);
		CHECK(v4.getValue<bool>() == true);

		Value v5 = Value(true) || Value(0.0f);
		REQUIRE(v5.getType() == Value::Boolean);
		CHECK(v5.getValue<bool>() == true);

		Value v6 = Value(std::string("true")) || Value(false);
		REQUIRE(v6.getType() == Value::Boolean);
		CHECK(v6.getValue<bool>() == true);

		Value v7 = Value(std::string("true")) || Value(true);
		REQUIRE(v7.getType() == Value::Boolean);
		CHECK(v7.getValue<bool>() == true);

		Value v8 = Value(std::string("")) || Value(false);
		REQUIRE(v8.getType() == Value::Boolean);
		CHECK(v8.getValue<bool>() == false);

		Value v9 = Value(false) || Value(0);
		REQUIRE(v9.getType() == Value::Boolean);
		CHECK(v9.getValue<bool>() == false);
	}
}


