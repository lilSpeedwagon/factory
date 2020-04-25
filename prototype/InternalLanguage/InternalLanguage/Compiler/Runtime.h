#pragma once

#include "stdafx.h"
#include "Utils.h"

namespace Runtime
{

	class RuntimeException : public Utils::BaseException
	{
	public:
		RuntimeException() {}
		RuntimeException(const char* msg)
		{
			m_msg.assign(msg);
		}
		virtual ~RuntimeException() = default;
	};
}