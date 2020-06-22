#pragma once
#include <string>

#define TYPENAME(t) (#t)
#define DEFINE_PTR(T) typedef std::shared_ptr<T> T##Ptr;
#define RESTRICT_COPY(T) private: T(T const&); void operator=(T const&);

namespace Utils
{
	std::string makeBltFileName(std::string const& fileName);
	
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
	
	std::string toString(int n);

	std::string toString(float f);

	std::string toString(bool b);

	int stringToInt(std::string const& s, bool* result = nullptr);

	float stringToFloat(std::string const& s, bool* result = nullptr);

	bool stringToBool(std::string const& s, bool* result = nullptr);

	template<typename Base, typename T>
	bool isInstanceOf(const T*) {
		return std::is_base_of<Base, T>::value;
	}
}
