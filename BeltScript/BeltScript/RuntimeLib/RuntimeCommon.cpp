#include "pch.h"
#include <ctime>
#include "RuntimeCommon.h"
#include "RuntimeContext.h"

namespace
{

	int fibonacci_impl(int num)
	{
		if (num <= 0) return 0;
		if (num == 1) return 1;
		return fibonacci_impl(num - 2) + fibonacci_impl(num - 1);
	}
	
}

// seeding random number generator
struct RandInit
{
	RandInit()
	{
		std::srand(static_cast<unsigned>(std::time(0)));
	}
};
static RandInit rand_init;


runtime::Value runtime::func_print(Value const& val)
{
	RuntimeContext::GetInstance().LogRuntimeMessage(val.toString().getValue<std::string>());
	return true;
}

runtime::Value runtime::fibonacci(Value const& val)
{
	return Value(fibonacci_impl(val.toInt().getValue<int>()));
}



