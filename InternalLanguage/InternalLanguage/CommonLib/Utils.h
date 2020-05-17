#pragma once
#include "pch.h"

#define TYPENAME(t) (#t)
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

	constexpr size_t count_symbols(int n)
	{
		size_t count = 0;
		while (n != 0) { n /= 10; count++; }
		return count;
	}

	constexpr size_t intBufferSize =	count_symbols((std::numeric_limits<int>::max)()) + 1;		// + sign
	constexpr size_t floatBufferSize =	3 + DBL_MANT_DIG + (-DBL_MIN_EXP);
	
	inline std::string toString(int n)
	{
		char buffer[intBufferSize];
		sprintf_s(buffer, intBufferSize, "%d", n);
		return std::string(buffer);
	}

	inline std::string toString(float f)
	{
		char buffer[floatBufferSize];
		sprintf_s(buffer, floatBufferSize, "%f", f);
		return buffer;
	}

	inline std::string toString(bool b)
	{
		return b ? "true" : "false";
	}

	inline int stringToInt(std::string const& s, bool* result = nullptr)
	{
		int n = 0;
		try
		{
			n = std::stoi(s);
			if (result != nullptr)
				*result = true;
		}
		catch(std::invalid_argument const&)
		{
			if (result != nullptr)
				*result = false;
		}
		catch (std::out_of_range const&)
		{
			if (result != nullptr)
				*result = false;
		}
		return n;
	}

	inline float stringToFloat(std::string const& s, bool* result = nullptr)
	{
		float f = 0;
		try
		{
			f = std::stof(s);
			if (result != nullptr)
				*result = true;
		}
		catch (std::invalid_argument const&)
		{
			if (result != nullptr)
				*result = false;
		}
		catch (std::out_of_range const&)
		{
			if (result != nullptr)
				*result = false;
		}
		return f;
	}

	inline bool stringToBool(std::string const& s, bool* result = nullptr)
	{
		if (result != nullptr)
			*result = true;
		return s == "true" || s == "True";
	}
}
