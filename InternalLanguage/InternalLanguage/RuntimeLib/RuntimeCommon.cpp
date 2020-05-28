#include "pch.h"
#include "RuntimeCommon.h"
#include "RuntimeExecutor.h"

runtime::Value runtime::func_print(Value const& val)
{
	RuntimeExecutor::getCurrentInstance().print(val.toString().getValue<std::string>());
	return true;
}

