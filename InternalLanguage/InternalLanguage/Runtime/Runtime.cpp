#include "pch.h"
#include "Runtime.h"
#include "Operations.h"
#include "../Serializer/Serializer.h"
#include "RuntimeExecutor.h"


bool runInternal(const char* codeFileName, void(__stdcall* log)(const char*), int inputsCount, float inputs[], int outputsCount, float* outputs[])
{
	bool result = false;

	try
	{
		Serializer s;
		OperationScopePtr pOperationTree = s.Load(codeFileName);

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

		runtime::RuntimeExecutor runtime(pOperationTree, log);
		const bool runtimeResult = runtime.run(vIn, static_cast<size_t>(outputsCount), out);
		if (!runtimeResult)
		{
			return false;
		}

		if (outputs != nullptr)
		{
			for (size_t i = 0; i < out.size() && i < static_cast<size_t>(outputsCount); i++)
			{
				*outputs[i] = out[i];
			}
		}
		
		result = true;
	}
	catch(SerializationError const& e)
	{
		std::string msg = "Cannot read file: " + e.Message();
		log(msg.c_str());
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

bool __declspec(dllexport) __stdcall Run(const char * codeFileName, void(__stdcall* log)(const char*))
{
	return runInternal(codeFileName, log, 0, nullptr, 0, nullptr);
}


bool __declspec(dllexport) __stdcall RunIO(const char * codeFileName, void(__stdcall* log)(const char*), int inputsCount, float inputs[], int outputsCount, float* outputs[])
{
	return runInternal(codeFileName, log, inputsCount, inputs, outputsCount, outputs);
}