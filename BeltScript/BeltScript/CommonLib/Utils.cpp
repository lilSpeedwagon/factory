#include "Utils.h"
#include <algorithm>

std::string Utils::makeBltFileName(std::string const& fileName)
{
	const auto lastSlash = std::max(fileName.find_last_of('/'), fileName.find_last_of('\\'));
	const auto lastDot = fileName.find_last_of('.', lastSlash);	// dot after slashes (to avoid ./ and ../)
	return ((lastDot != std::string::npos) ? fileName.substr(0, lastDot) : fileName) + ".blt";
}

std::string Utils::toString(int n)
{
	char buffer[intBufferSize];
	sprintf_s(buffer, intBufferSize, "%d", n);
	return std::string(buffer);
}

std::string Utils::toString(float f)
{
	char buffer[floatBufferSize];
	sprintf_s(buffer, floatBufferSize, "%f", f);
	return buffer;
}

std::string Utils::toString(bool b)
{
	return b ? "true" : "false";
}

int Utils::stringToInt(std::string const& s, bool* result)
{
	int n = 0;
	try
	{
		n = std::stoi(s);
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
	return n;
}

float Utils::stringToFloat(std::string const& s, bool* result)
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

bool Utils::stringToBool(std::string const& s, bool* result)
{
	if (result != nullptr)
		*result = true;
	return s == "true" || s == "True";
}
