#include "pch.h"
#include "RuntimeContext.h"

runtime::RuntimeContext* runtime::RuntimeContext::g_currentInstance = nullptr;

void runtime::RuntimeContext::SetLog(LogDelegate log)
{
	SetLogDelegate(log);
}

void runtime::RuntimeContext::LogRuntimeMessage(std::string const& msg) const
{
	Log(msg);
}

runtime::RuntimeContext& runtime::RuntimeContext::GetInstance()
{
	if (g_currentInstance == nullptr)
	{
		g_currentInstance = new RuntimeContext;
	}
	return *g_currentInstance;
}
