#pragma once
#include "Logger.h"

namespace runtime
{
	class RuntimeContext : public Logger
	{
	public:
		void SetLog(LogDelegate log);
		void LogRuntimeMessage(std::string const& msg) const;

		static RuntimeContext& GetInstance();
	private:
		static RuntimeContext* g_currentInstance;
		RuntimeContext()
		{
			SetLogName("Runtime");
		}
	};

	class ContextLogScopedKeeper
	{
		RESTRICT_COPY(ContextLogScopedKeeper)
	public:
		ContextLogScopedKeeper(LogDelegate log)
		{
			RuntimeContext::GetInstance().SetLog(log);
		}
		~ContextLogScopedKeeper()
		{
			RuntimeContext::GetInstance().SetLog(nullptr);
		}
	};
}
