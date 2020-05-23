#pragma once
#include "../Compiler/Operations.h"
#include "Logger.h"

namespace runtime
{
	class Runtime : public Logger
	{
	public:
		Runtime(Runtime const&) = delete;
		void operator=(Runtime const&) = delete;
		bool run(OperationScopePtr operation_tree);

		static Runtime& getInstance();
		void setOutput(LogDelegate log);
		void print(std::string const& str) const;
	private:
		static std::shared_ptr<Runtime> g_pInstance;
		Runtime() = default;
	};
}

