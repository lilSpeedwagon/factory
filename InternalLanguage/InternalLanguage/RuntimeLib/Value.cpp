#include "pch.h"
#include "Value.h"
#include "Utils.h"
#include "FileHelpers.h"

runtime::Value::Value()
{
	m_type = Integer;
	m_value = reinterpret_cast<_Value>(0);
}

runtime::Value::Value(Value const& v)
{
	m_type = v.m_type;
	if (m_type == String)
	{
		const size_t len = strlen(reinterpret_cast<char*>(v.m_value)) + 1;
		char* buffer = new char[len];
		strcpy_s(buffer, len, reinterpret_cast<char*>(v.m_value));
		m_value = reinterpret_cast<_Value>(buffer);
	}
	else
	{
		m_value = v.m_value;
	}
}

runtime::Value::Value(int v)
{
	operator=(v);
}

runtime::Value::Value(float v)
{
	operator=(v);
}

runtime::Value::Value(bool v)
{
	operator=(v);
}

runtime::Value::Value(char* v)
{
	operator=(v);
}

runtime::Value::Value(std::string const& v)
{
	operator=(v);
}

runtime::Value::~Value()
{
	clear();
}

serializer::BinaryFile& runtime::Value::operator>>(serializer::BinaryFile& file) const
{
	const ValueType type = m_type;
	const uint8_t raw_type = type;

	file << raw_type;
	
	if (type == String)
	{
		file << getValue<std::string>();
	}
	else
	{
		file << m_value;
	}
	
	return file;
}

serializer::BinaryFile& runtime::Value::operator<<(serializer::BinaryFile& file)
{
	uint8_t raw_type;
	file >> raw_type;
	m_type = static_cast<ValueType>(raw_type);

	if (m_type == String)
	{
		std::string str;
		file >> str;
		
		const size_t size = str.size();
		char* buffer = new char[size + 1];
		
		const errno_t err = strcpy_s(buffer, size + 1, str.c_str());
		if (err != 0)
		{
			delete[] buffer;
			throw serializer::SerializationError("Cannot read value from the file. Error code: " + Utils::toString(err));
		}
		
		m_value = reinterpret_cast<_Value>(buffer);
	}
	else
	{
		file >> m_value;
	}
	
	return file;
}

void runtime::Value::clear()
{
	if (m_type == String && reinterpret_cast<char*>(m_value) != nullptr)
	{
		delete[] reinterpret_cast<char*>(m_value);
		m_value = nullptr;
	}
}

runtime::Value& runtime::Value::operator=(Value const& v)
{
	if (this == &v)
		return *this;

	clear();

	m_type = v.m_type;
	if (m_type == String)
	{
		const size_t len = strlen(reinterpret_cast<char*>(v.m_value)) + 1;
		char* buffer = new char[len];
		strcpy_s(buffer, len, reinterpret_cast<char*>(v.m_value));
		m_value = reinterpret_cast<_Value>(buffer);
	}
	else
	{
		m_value = v.m_value;
	}

	return *this;
}

runtime::Value& runtime::Value::operator=(int v)
{
	clear();

	m_type = Integer;
	m_value = reinterpret_cast<_Value>(v);

	return *this;
}

runtime::Value& runtime::Value::operator=(float v)
{
	clear();

	m_type = Float;
	static_assert(sizeof(_Value) == sizeof(float));
	memcpy_s(static_cast<_Value>(&m_value), sizeof(_Value), &v, sizeof(float));

	return *this;
}

runtime::Value& runtime::Value::operator=(bool v)
{
	clear();

	m_type = Boolean;
	m_value = reinterpret_cast<_Value>(v == true ? 1 : 0);

	return *this;
}

runtime::Value& runtime::Value::operator=(char* v)
{
	operator=(std::string(v));
	return *this;
}

runtime::Value& runtime::Value::operator=(std::string const& v)
{
	clear();

	m_type = String;
	const size_t len = v.size() + 1; // including '\0'
	char* buffer = new char[len];
	strcpy_s(buffer, len, v.c_str());
	m_value = reinterpret_cast<_Value>(buffer);

	return *this;
}

runtime::Value runtime::Value::operator!() const
{
	Value val;

	switch (m_type)
	{
	case Integer:
		val = (getValue<int>() == 0);
		break;
	case Float:
		val = (getValue<float>() == 0.0);
		break;
	case Boolean:
		val = !getValue<bool>();
		break;
	case String:
		val = (getValue<std::string>().empty());
		break;
	default:
		throw UndefinedValueException();
	}

	return val;
}

runtime::Value runtime::Value::operator+(Value const& r_val) const
{
	Value val;

	const ValueType resultType = getHighestPriorityType(m_type, r_val.m_type);
	switch (resultType)
	{
	case Integer:
		val = toInt().getValue<int>() + r_val.toInt().getValue<int>();
		break;
	case Float:
		val = toFloat().getValue<float>() + r_val.toFloat().getValue<float>();
		break;
	case Boolean:
		val = toBool().getValue<bool>() || r_val.toBool().getValue<bool>();
		break;
	case String:
		val = toString().getValue<std::string>() + r_val.toString().getValue<std::string>();
		break;
	default:
		throw UndefinedValueException();
	}
	
	return val;
}

runtime::Value runtime::Value::operator-(Value const& r_val) const
{
	Value val;

	const ValueType resultType = getHighestPriorityType(m_type, r_val.m_type);
	switch (resultType)
	{
	case Boolean:	// cast true to 1 and false to 0
	case Integer:
		val = toInt().getValue<int>() - r_val.toInt().getValue<int>();
		break;
	case Float:
		val = toFloat().getValue<float>() - r_val.toFloat().getValue<float>();
		break;
	case String:
		throw InvalidOperationException("-", TYPENAME(String));
	default:
		throw UndefinedValueException();
	}

	return val;
}

runtime::Value runtime::Value::operator*(Value const& r_val) const
{
	Value val;

	const ValueType resultType = getHighestPriorityType(m_type, r_val.m_type);
	switch (resultType)
	{
	case Integer:
		val = toInt().getValue<int>() * r_val.toInt().getValue<int>();
		break;
	case Float:
		val = toFloat().getValue<float>() * r_val.toFloat().getValue<float>();
		break;
	case Boolean:
		val = toBool().getValue<bool>() && r_val.toBool().getValue<bool>();
		break;
	case String:
	{
		if (m_type == Integer || r_val.m_type == Integer)
		{
			std::string str;
			if (m_type == Integer)
			{
				const std::string r_strVal = r_val.getValue<std::string>();
				for (int i = 0; i < getValue<int>(); i++)
					str += r_strVal;
			}
			else
			{
				const std::string l_strVal = getValue<std::string>();
				for (int i = 0; i < r_val.getValue<int>(); i++)
					str += l_strVal;
			}
			val = str;
		}
		else
		{
			throw InvalidOperationException("*", TYPENAME(String));
		}
		break;
	}
	default:
		throw UndefinedValueException();
	}

	return val;
}

runtime::Value runtime::Value::operator/(Value const& r_val) const
{
	Value val;

	const ValueType resultType = getHighestPriorityType(m_type, r_val.m_type);
	switch (resultType)
	{
	case Integer:
	case Boolean:
	case Float:
	{
		const float r_floatValue = r_val.toFloat().getValue<float>();
		if (r_floatValue == 0.0f)
			throw DivisionByZeroException();
		val = toFloat().getValue<float>() / r_floatValue;
		break;
	}
	case String:
		throw InvalidOperationException("*", TYPENAME(String));
	default:
		throw UndefinedValueException();
	}

	return val;
}

runtime::Value runtime::Value::operator%(Value const& r_val) const
{
	Value val;

	const ValueType resultType = getHighestPriorityType(m_type, r_val.m_type);
	switch (resultType)
	{
	case Integer:
		val = toInt().getValue<int>() % r_val.toInt().getValue<int>();
		break;
	case Float:
		throw InvalidOperationException("%", TYPENAME(Float));
	case Boolean:
		throw InvalidOperationException("%", TYPENAME(Boolean));
	case String:
		throw InvalidOperationException("%", TYPENAME(String));
	default:
		throw UndefinedValueException();
	}

	return val;
}

runtime::Value runtime::Value::operator==(Value const& r_val) const
{
	return internalEqual(r_val);
}

runtime::Value runtime::Value::operator!=(Value const& r_val) const
{
	return !internalEqual(r_val);
}

runtime::Value runtime::Value::operator>(Value const& r_val) const
{
	return !internalLess(r_val) && !internalEqual(r_val);
}

runtime::Value runtime::Value::operator<(Value const& r_val) const
{
	return internalLess(r_val);
}

runtime::Value runtime::Value::operator>=(Value const& r_val) const
{
	return !internalLess(r_val);
}

runtime::Value runtime::Value::operator<=(Value const& r_val) const
{
	return internalLess(r_val) || internalEqual(r_val);
}

runtime::Value runtime::Value::operator&&(Value const& r_val) const
{
	return toBool().getValue<bool>() && r_val.toBool().getValue<bool>();
}

runtime::Value runtime::Value::operator||(Value const& r_val) const
{
	return toBool().getValue<bool>() || r_val.toBool().getValue<bool>();
}

bool runtime::Value::internalEqual(Value const& r_val) const
{
	bool isEqual;

	const ValueType resultType = getHighestPriorityType(m_type, r_val.m_type);
	switch (resultType)
	{
	case Integer:
		isEqual = toInt().getValue<int>() == r_val.toInt().getValue<int>();
		break;
	case Float:
		isEqual = toFloat().getValue<float>() == r_val.toFloat().getValue<float>();
		break;
	case Boolean:
		isEqual = toBool().getValue<bool>() == r_val.toBool().getValue<bool>();
		break;
	case String:
		isEqual = toString().getValue<std::string>() == r_val.toString().getValue<std::string>();
		break;
	default:
		throw UndefinedValueException();
	}

	return isEqual;
}

bool runtime::Value::internalLess(Value const& r_val) const
{
	bool isMore;

	const ValueType resultType = getHighestPriorityType(m_type, r_val.m_type);
	switch (resultType)
	{
	case Integer:
		isMore = toInt().getValue<int>() < r_val.toInt().getValue<int>();
		break;
	case Float:
		isMore = toFloat().getValue<float>() < r_val.toFloat().getValue<float>();
		break;
	case Boolean:
		isMore = toBool().getValue<bool>() < r_val.toBool().getValue<bool>();
		break;
	case String:
		isMore = toString().getValue<std::string>() < r_val.toString().getValue<std::string>();
		break;
	default:
		throw UndefinedValueException();
	}

	return isMore;
}

runtime::Value runtime::Value::toInt() const
{
	Value val;
	switch(m_type)
	{
	case Integer:
		val = *this;
		break;
	case Float:
		val = static_cast<int>(getValue<float>());
		break;
	case Boolean:
		val = getValue<bool>() ? 1 : 0;
		break;
	case String:
		val = Utils::stringToInt(getValue<std::string>());
		break;
	default:
		throw UndefinedValueException();
	}
	return val;
}

runtime::Value runtime::Value::toFloat() const
{
	Value val;
	switch (m_type)
	{
	case Integer:
		val = static_cast<float>(getValue<int>());
		break;
	case Float:
		val = *this;
		break;
	case Boolean:
		val = getValue<bool>() ? 1.0f : 0.0f;
		break;
	case String:
		val = Utils::stringToFloat(getValue<std::string>());
		break;
	default:
		throw UndefinedValueException();
	}
	return val;
}

runtime::Value runtime::Value::toBool() const
{
	Value val;
	switch (m_type)
	{
	case Integer:
		val = getValue<int>() != 0;
		break;
	case Float:
		val = getValue<float>() != 0.0f;
		break;
	case Boolean:
		val = getValue<bool>();
		break;
	case String:
		val = !getValue<std::string>().empty();
		break;
	default:
		throw UndefinedValueException();
	}
	return val;
}

runtime::Value runtime::Value::toString() const
{
	Value val;
	switch (m_type)
	{
	case Integer:
		val = Utils::toString(getValue<int>());
		break;
	case Float:
		val = Utils::toString(getValue<float>());
		break;
	case Boolean:
		val = Utils::toString(getValue<bool>());
		break;
	case String:
		val = getValue<std::string>();
		break;
	default:
		throw UndefinedValueException();
	}
	return val;
}

runtime::Value::ValueType runtime::Value::getHighestPriorityType(ValueType lt, ValueType rt)
{
	if (lt == String || rt == String)
		return String;

	if (lt == Float || rt == Float)
		return Float;

	if (lt == Integer || rt == Integer)
		return Integer;

	if (lt == Boolean || rt == Boolean)
		return Boolean;
	
	return Undefined;
}

