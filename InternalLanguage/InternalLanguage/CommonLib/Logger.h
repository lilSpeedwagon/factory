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
		if (m_logDelegate != nullptr)
		{
			std::string msg;
			if (!m_logName.empty())
			{
				msg += m_logName + ": ";
			}
			msg += message;
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