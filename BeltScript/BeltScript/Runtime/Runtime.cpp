#include "pch.h"
#include "Runtime.h"
#include "Operations.h"
#include "RuntimeContext.h"
#include "Serializer.h"
#include "RuntimeExecutor.h"
#include "RuntimeStorage.h"


bool runInternal(const char* codeFileName, void(__stdcall* log)(const char*), int inputsCount, float inputs[], int outputsCount, float** outputs)
{
	bool result = false;

	// don't let LogDelegate be empty
	if (log == nullptr)
	{
		log = [](const char*) {};
	}
	
	try
	{
		// try to find cashed runtime tree
		OperationScopePtr pOperationTree = RuntimeStorage::GetInstance().GetTree(codeFileName);

		// upload from file if not found
		if (pOperationTree == nullptr)
		{
			serializer::Serializer s(codeFileName, log);
			const bool serRes = s.Load();
			if (!serRes)
			{
				return false;
			}
			pOperationTree = s.GetTree();
		}

		// set inputs and outputs
		runtime::Inputs vIn;
		if (inputs != nullptr)
		{
			vIn.reserve(inputsCount);
			for (int i = 0; i < inputsCount; i++)
			{
				vIn.push_back(inputs[i]);
			}
		}
		runtime::Outputs out;

		// set runtime context
		runtime::ContextLogScopedKeeper logKeeper(log);
		
		// run executor
		runtime::RuntimeExecutor runtime(pOperationTree, log);
		const bool runtimeResult = runtime.run(vIn, static_cast<size_t>(outputsCount), out);
		if (!runtimeResult)
		{
			return false;
		}

		// handle outputs
		if (outputs != nullptr)
		{
			for (size_t i = 0; i < out.size() && i < static_cast<size_t>(outputsCount); i++)
			{
				(*outputs)[i] = out[i];
			}
		}

		// store tree to cash
		RuntimeStorage::GetInstance().StoreTree(codeFileName, pOperationTree);
		
		result = true;
	}
	catch(std::exception const& e)
	{
		log(e.what());
	}
	catch(...)
	{
		log("Something went wrong. Terminating.");
	}
	
	return result;
}


extern "C"
{
	bool __declspec(dllexport) __stdcall Run(const char * codeFileName, void(__stdcall* log)(const char*))
	{
		return runInternal(codeFileName, log, 0, nullptr, 0, nullptr);
	}

	bool __declspec(dllexport) __stdcall RunIO(const char * codeFileName, void(__stdcall* log)(const char*), int inputsCount, float inputs[], int outputsCount, float** outputs)
	{
		return runInternal(codeFileName, log, inputsCount, inputs, outputsCount, outputs);
	}
}