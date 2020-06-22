#include "Logger.h"

void Logger::Log(std::string const& message) const
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

void Logger::SetLogName(std::string const& name)
{
	m_logName = name;
}

void Logger::SetLogDelegate(LogDelegate const& delegate)
{
	m_logDelegate = delegate;
}
