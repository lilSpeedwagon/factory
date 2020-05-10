#include "pch.h"
#include "Runtime.h"


Runtime::Value::Value()
{
	m_type = Integer;
	m_value = reinterpret_cast<_Value>(0);
}

Runtime::Value::Value(Value const& v)
{
	m_type = v.m_type;
	if (m_type == String)
	{
		const size_t len = strlen(reinterpret_cast<char*>(v.m_value));
		char* buffer = new char[len + 1];
		strcpy_s(buffer, len, reinterpret_cast<char*>(v.m_value));
	}
	else
	{
		m_value = v.m_value;
	}
}

Runtime::Value::~Value()
{
	if (m_type == String && reinterpret_cast<char*>(m_value) != nullptr)
		delete[] reinterpret_cast<char*>(m_value);
}

Runtime::Value& Runtime::Value::operator=(Value const& v)
{
	return *this;
}

Runtime::Value& Runtime::Value::operator=(int const& v)
{
	return *this;
}

Runtime::Value& Runtime::Value::operator=(bool const& v)
{
	return *this;
}

Runtime::Value& Runtime::Value::operator=(float const& v)
{
	return *this;
}

Runtime::Value& Runtime::Value::operator=(std::string const& v)
{
	return *this;
}

Runtime::Value Runtime::Value::operator!()
{
	return *this;
}

Runtime::Value Runtime::Value::operator+(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator-(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator/(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator*(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator%(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator==(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator!=(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator>(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator<(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator>=(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator<=(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator&&(Value const& r_val)
{
	return *this;
}

Runtime::Value Runtime::Value::operator||(Value const& r_val)
{
	return *this;
}