#include "pch.h"
#include "RuntimeExecutor.h"


bool runtime::RuntimeExecutor::run(Inputs const& in, size_t expectedOutputs, Outputs& out)
{
	bool result = false;

	try
	{
		DebugLog("Reset operations tree");
		m_pOperationTree->Reset();
		
		DebugLog("Executing...");
		for (size_t i = 0; i < in.size(); i++)
		{
			const IOType val = in[i];
			std::stringstream ss;
			ss << "in" << i;
			m_pOperationTree->SetVariableValue(ss.str(), Value(val));
		}
		
		m_pOperationTree->Execute();

		for (size_t i = 0; i < expectedOutputs; i++)
		{
			std::stringstream ss;
			ss << "out" << i;
			if (m_pOperationTree->IsVariableExist(ss.str()))
			{
				const Value val = m_pOperationTree->GetVariableValue(ss.str());
				out.push_back(val.toFloat().getValue<float>());
			}
			else
			{
				static const Value default_value = 0.0f;
				out.push_back(default_value.toFloat().getValue<float>());
			}
		}
		
		result = true;
		DebugLog("Done");
	}
	catch (RuntimeException const& e)
	{
		Log("Runtime exception: " + e.Message());
	}
	catch (std::exception const& e)
	{
		Log("Exception: " + std::string(e.what()));
	}

	return result;
}

