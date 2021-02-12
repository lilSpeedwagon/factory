#include "pch.h"
#include <ctime>
#include "RuntimeCommon.h"
#include "RuntimeContext.h"


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

