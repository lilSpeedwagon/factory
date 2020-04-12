#pragma once

#include "Definitions.h"

class Logger
{
public:
	void SetLogName(std::string const& name)
	{
		m_logName = name;
	}
	void SetLogDelegate(LogDelegate const& delegate)
	{
		m_logDelegate = delegate;
	}
	void Log(std::string const& message)
	{
		std::string msg = m_logName + ": " + message;
		m_logDelegate(msg.c_str());
	}

protected:
	std::string m_logName;
	LogDelegate m_logDelegate;
};