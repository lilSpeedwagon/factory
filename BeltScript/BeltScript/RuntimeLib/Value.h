#pragma once
#include "RuntimeException.h"
#include "SerializerCommon.h"
#include <sstream>

namespace runtime
{
	class UndefinedValueException : public RuntimeException
	{
	public:
		virtual ~UndefinedValueException() = default;
	};
	
	class InvalidOperationException : public RuntimeException
	{
	public:
		virtual ~InvalidOperationException() = default;
		InvalidOperationException(std::string const& strOperation, std::string const& strLType, std::string const& strRType = "")
		{
			std::stringstream ss;
			ss << "Invalid operation \"" << strOperation << "\" with type \"" << strLType << "\"";
			if (!strRType.empty())
				ss << " and \"" << strRType << "\"";
			m_msg = ss.str();
		}
	};
	
	class DivisionByZeroException : public RuntimeException
	{
	public:
		DivisionByZeroException() { m_msg = "Division by zero"; }
		virtual ~DivisionByZeroException() = default;
	};
	
	class Value
	{
	public:
		/* Sizes of void*, int and float depend of the OS but the are always equal between each other.
		 * Let's assume bool as int and store 0 for false and 1 for true.
		 * We can also store char* in void*.
		 * So this way void* is appropriate for storing different data types. */
		typedef void* _Value;

#ifdef _WIN64
		typedef double decimal;
		typedef int64_t integer;
#else
		typedef float decimal;
		typedef int32_t integer;
#endif

		enum ValueType : uint8_t
		{
			Undefined = 0,
			Integer = 1,
			Float = 2,
			Boolean = 3,
			String = 4,
		};

		Value();
		Value(Value const&);
		Value(int v);
		Value(float v);
		Value(bool v);
		Value(char* v);					// require explicit constructor call instead of:
		Value(std::string const& v);	// Value v = "str"
		~Value();
		Value& operator=(Value const& v);
		Value& operator=(int v);
		Value& operator=(bool v);
		Value& operator=(float v);
		Value& operator=(char* v);
		Value& operator=(std::string const& v);
		Value operator!() const;
		Value operator+(Value const& r_val) const;
		Value operator-(Value const& r_val) const;
		Value operator/(Value const& r_val) const;
		Value operator*(Value const& r_val) const;
		Value operator%(Value const& r_val) const;
		Value operator==(Value const& r_val) const;
		Value operator!=(Value const& r_val) const;
		Value operator>(Value const& r_val) const;
		Value operator<(Value const& r_val) const;
		Value operator>=(Value const& r_val) const;
		Value operator<=(Value const& r_val) const;
		Value operator&&(Value const& r_val) const;
		Value operator||(Value const& r_val) const;

		Value toInt() const;
		Value toFloat() const;
		Value toBool() const;
		Value toString() const;

		ValueType getType() const
		{
			return m_type;
		}

		template<typename T>
		T getValue() const
		{
			return reinterpret_cast<T>(m_value);
		}

		template<>
		int getValue<int>() const
		{
			return static_cast<int>(reinterpret_cast<integer>(m_value));
		}
		
		template<>
		float getValue<float>() const
		{
			// DANGER ZONE
			static_assert(sizeof(_Value) == sizeof(decimal));
			decimal fValue;
			memcpy_s(static_cast<void*>(&fValue), sizeof(decimal), &m_value, sizeof(_Value));
			return static_cast<float>(fValue);
		}
		
		template<>
		bool getValue<bool>() const
		{
			return m_value != NULL;
		}
		
		template<>
		std::string getValue<std::string>() const
		{
			return std::string(reinterpret_cast<char*>(m_value));
		}

		serializer::BinaryFile& operator>>(serializer::BinaryFile& file) const;
		serializer::BinaryFile& operator<<(serializer::BinaryFile& file);

	private:
		void clear();
		static ValueType getHighestPriorityType(ValueType lt, ValueType rt);
		bool internalEqual(Value const& r_val) const;
		bool internalLess(Value const& r_val) const;

		ValueType m_type;
		_Value m_value;
	};
}

