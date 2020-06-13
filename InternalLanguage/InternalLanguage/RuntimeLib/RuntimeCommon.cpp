#include "pch.h"
#include "RuntimeCommon.h"
#include "RuntimeContext.h"


runtime::Value runtime::func_print(Value const& val)
{
	RuntimeContext::GetInstance().LogRuntimeMessage(val.toString().getValue<std::string>());
	return true;
}

