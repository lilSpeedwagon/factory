#include "catch.hpp"
#include "Runtime.h"

TEST_CASE("Value default constructor", "[Value]")
{
	Runtime::Value val;
	REQUIRE(val.getType() == Runtime::Value::Integer);
	REQUIRE(val.getValue<int>() == 0);
	REQUIRE(val.getValue<char*>() == nullptr);
	REQUIRE(val.getValue<float>() == 0.0f);
	REQUIRE(val.getValue<bool>() == false);
}
