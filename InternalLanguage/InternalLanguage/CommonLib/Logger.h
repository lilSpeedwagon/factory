#pragma once

#include <string>
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
	void Log(std::string const& message) const;
	void SetLogName(std::string const& name);
	void SetLogDelegate(LogDelegate const& delegate);
	
private:
	std::string m_logName;
	LogDelegate m_logDelegate;
};


