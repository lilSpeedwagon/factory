#pragma once
#include "Operations.h"
#include "Logger.h"

namespace runtime
{
	class RuntimeExecutor : public Logger
	{
	public:
		static const RuntimeExecutor& getCurrentInstance();
		
		RuntimeExecutor(OperationScopePtr operation_tree, LogDelegate log) : m_pOperationTree(operation_tree)
		{
			SetLogName("Runtime");
			SetLogDelegate(log);
		}
		RuntimeExecutor(RuntimeExecutor const&) = delete;
		void operator=(RuntimeExecutor const&) = delete;

		bool run(Inputs const& in, size_t expectedOutputs, Outputs& out);

		void print(std::string const& str) const;
	private:
		OperationScopePtr m_pOperationTree;
		
		static RuntimeExecutor* g_pInstance;
		static void setCurrentInstance(RuntimeExecutor* current);
	};
}

