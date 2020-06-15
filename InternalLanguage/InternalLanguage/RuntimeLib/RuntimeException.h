#pragma once

#include "BaseException.h"

namespace runtime
{
	class RuntimeException : public BaseException
	{
	public:
		RuntimeException() = default;
		RuntimeException(std::string const& msg)
		{
			m_msg = msg;
		}
		virtual ~RuntimeException() = default;
	};
}