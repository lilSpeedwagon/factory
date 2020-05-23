#include "pch.h"
#include "RuntimeLauncher.h"

std::shared_ptr<runtime::Runtime> runtime::Runtime::g_pInstance = nullptr;

runtime::Runtime& runtime::Runtime::getInstance()
{
	if (g_pInstance == nullptr)
	{
		g_pInstance = std::shared_ptr<Runtime>(new Runtime());
		g_pInstance->SetLogName("Runtime");
		g_pInstance->print("Init...");
	}
	return *g_pInstance;
}

void runtime::Runtime::setOutput(LogDelegate log)
{
	SetLogDelegate(log);
}

void runtime::Runtime::print(std::string const& str) const
{
	Log(str);
}

bool runtime::Runtime::run(OperationScopePtr operation_tree)
{
	bool result = false;

	try
	{
		print("Executing...");
		operation_tree->Execute();
		result = true;
		print("Done");
	}
	catch (RuntimeException const& e)
	{
		print("Runtime exception: " + e.Message());
	}
	catch (std::exception const& e)
	{
		print("Exception: " + std::string(e.what()));
	}
	catch (...)
	{
		print("Something went wrong. Termination...");
	}

	return result;
}
