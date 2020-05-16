#pragma once
#include "BaseException.h"

namespace runtime
{
	class UndefinedValueException : public BaseException {};
	
	class Value
	{
	public:
		/* Sizes of void*, int and float depend of the OS but the are always equal between each other.
		 * Let's assume bool as int and store 0 for false and 1 for true.
		 * We can also store char* in void*.
		 * So this way void* is appropriate for storing different data types. */
		typedef void* _Value;

		enum ValueType
		{
			Undefined = -1,
			Integer = 0,
			Float = 1,
			Boolean = 2,
			String = 3,
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
		float getValue<float>() const
		{
			// DANGER ZONE
			static_assert(sizeof(_Value) == sizeof(float));
			float fValue;
			memcpy_s(static_cast<void*>(&fValue), sizeof(float), &m_value, sizeof(_Value));
			return fValue;
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

	private:
		void clear();
		static ValueType getHighestPriorityType(ValueType lt, ValueType rt);

		ValueType m_type;
		_Value m_value;
	};
}

