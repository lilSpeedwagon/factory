#pragma once
#include "pch.h"

#define DEFINE_PTR(T) typedef std::shared_ptr<T> T##Ptr;
#define RESTRICT_COPY(T) private: T(T const&); void operator=(T const&);

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
}
