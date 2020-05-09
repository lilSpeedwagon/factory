#include "stdafx.h"
#include "Runtime.h"

Runtime::Value::Value()
{
	
}

Runtime::Value::Value(Value const&)
{
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