#pragma once

#include "pch.h"
#include "Definitions.h"

class Logger
{
public:
	Logger()
	{
		m_logName = "";
		m_logDelegate = nullptr;
	}

protected:
	inline void Log(std::string const& message) const
	{
		std::string msg = m_logName + ": " + message;
		if (m_logDelegate != nullptr)
		{
			m_logDelegate(msg.c_str());
		}
	}
	void SetLogName(std::string const& name)
	{
		m_logName = name;
	}
	void SetLogDelegate(LogDelegate const& delegate)
	{
		m_logDelegate = delegate;
	}
	
private:
	std::string m_logName;
	LogDelegate m_logDelegate;
};