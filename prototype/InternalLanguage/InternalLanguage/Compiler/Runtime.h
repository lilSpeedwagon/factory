#pragma once

#include "stdafx.h"
#include "Utils.h"

namespace Runtime
{
	typedef std::variant<int, float, std::string, bool> Value;
	struct Variable
	{
		std::string identifier;
		Value value;
	};

	class RuntimeException : public Utils::BaseException
	{
	public:
		RuntimeException() { m_pMessage = nullptr; }
		RuntimeException(const char* msg)
		{
			m_pMessage = new char[strlen(msg) + 1];
			strcpy(m_pMessage, msg);
		}
		virtual ~RuntimeException() = default;
	};
}