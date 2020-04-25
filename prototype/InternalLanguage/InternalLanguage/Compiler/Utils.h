#pragma once

#include "stdafx.h"

namespace Utils
{
	inline bool isQuotedString(std::string const& str)
	{
		return str.front() == '\"' && str.back() == '\"';
	}

	inline std::string removeQuotes(std::string str)
	{
		if (isQuotedString(str))
		{
			str.erase(0, 1);
			str.erase(str.size() - 2, 1);
		}

		return str;
	}

	class BaseException
	{
	public:
		BaseException() {}
		BaseException(std::string const& msg) : m_msg(msg) {}
		BaseException(const char* msg) : m_msg(msg) {}
		virtual ~BaseException() = default;
		virtual std::string Message() const	{ return m_msg;	}
	protected:
		std::string m_msg;
	};
}
