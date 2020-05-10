#pragma once

#include "pch.h"
#include "BaseException.h"

namespace Runtime
{
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
		~Value();
		Value& operator=(Value const& v);
		Value& operator=(int const& v);
		Value& operator=(bool const& v);
		Value& operator=(float const& v);
		Value& operator=(std::string const& v);
		Value operator!();
		Value operator+(Value const& r_val);
		Value operator-(Value const& r_val);
		Value operator/(Value const& r_val);
		Value operator*(Value const& r_val);
		Value operator%(Value const& r_val);
		Value operator==(Value const& r_val);
		Value operator!=(Value const& r_val);
		Value operator>(Value const& r_val);
		Value operator<(Value const& r_val);
		Value operator>=(Value const& r_val);
		Value operator<=(Value const& r_val);
		Value operator&&(Value const& r_val);
		Value operator||(Value const& r_val);
		

		ValueType getType() const
		{
			return m_type;
		}
		template<typename T>
		T getValue() const
		{
			return reinterpret_cast<T>(m_value);
		}

	private:
		ValueType m_type;
		_Value m_value;
	};

	struct Variable
	{
		Variable(std::string const& name) : identifier(name) {}
		std::string identifier;
		Value value;
	};

	typedef void(*Procedure)();
	typedef void(*ProcedureUnary)(Value arg);
	typedef void(*ProcedureBinary)(Value l_arg, Value r_arg);
	typedef Value(*Function)();
	typedef Value(*FunctionUnary)(Value arg);
	typedef Value(*FunctionBinary)(Value l_arg, Value r_arg);

	const std::map<std::string, FunctionUnary> mapUnaryFunctions = {
		{ "!", [](Value arg) { return !arg; } }
	};
	
	const std::map<std::string, FunctionBinary> mapBinaryFunctions = {
		{ "+", [](Value l_arg, Value r_arg) { return l_arg + r_arg; } },
		{ "-", [](Value l_arg, Value r_arg) { return l_arg - r_arg; } },
		{ "/", [](Value l_arg, Value r_arg) { return l_arg / r_arg; } },
		{ "*", [](Value l_arg, Value r_arg) { return l_arg * r_arg; } },
		{ "%", [](Value l_arg, Value r_arg) { return l_arg % r_arg; } },
		{ "==", [](Value l_arg, Value r_arg) { return l_arg == r_arg; } },
		{ "!=", [](Value l_arg, Value r_arg) { return l_arg != r_arg; } },
		{ ">", [](Value l_arg, Value r_arg) { return l_arg > r_arg; } },
		{ "<", [](Value l_arg, Value r_arg) { return l_arg < r_arg; } },
		{ ">=", [](Value l_arg, Value r_arg) { return l_arg >= r_arg; } },
		{ "<=", [](Value l_arg, Value r_arg) { return l_arg <= r_arg; } },		
	};
	
	class RuntimeException : public BaseException
	{
	public:
		RuntimeException() = default;
		RuntimeException(const char* msg)
		{
			m_msg.assign(msg);
		}
		virtual ~RuntimeException() = default;
	};
}
