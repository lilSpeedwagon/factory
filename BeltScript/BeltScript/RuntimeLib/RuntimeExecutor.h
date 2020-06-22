#pragma once
#include "Operations.h"
#include "Logger.h"

namespace runtime
{
	class RuntimeExecutor : public Logger
	{
	public:
		
		RuntimeExecutor(OperationScopePtr operation_tree, LogDelegate log) : m_pOperationTree(operation_tree)
		{
			SetLogName("Runtime executor");
			SetLogDelegate(log);
		}
		RuntimeExecutor(RuntimeExecutor const&) = delete;
		void operator=(RuntimeExecutor const&) = delete;

		bool run(Inputs const& in, size_t expectedOutputs, Outputs& out);
	private:
		OperationScopePtr m_pOperationTree;
	};
}

