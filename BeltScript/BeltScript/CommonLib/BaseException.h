#pragma once
#include <string>

class BaseException
{
public:
	BaseException() {}
	BaseException(std::string const& msg) : m_msg(msg) {}
	BaseException(const char* msg) : m_msg(msg) {}
	virtual ~BaseException() = default;
	virtual std::string Message() const { return m_msg; }
protected:
	std::string m_msg;
};