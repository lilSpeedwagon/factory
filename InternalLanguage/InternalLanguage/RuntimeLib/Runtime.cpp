#include "pch.h"
#include "Runtime.h"
#include "RuntimeLauncher.h"

runtime::Value runtime::func_print(Value const& val)
{
	Runtime::getInstance().print(val.toString().getValue<std::string>());
	return true;
}

