#pragma once
#include "pch.h"
#include "BaseException.h"
#include "Value.h"

namespace runtime
{
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
	
	

	struct Variable
	{
		Variable(std::string const& name) : identifier(name) {}
		std::string identifier;
		Value value;
	};

	typedef Value(*FunctionUnary)(Value const& arg);
	typedef Value(*FunctionBinary)(Value const& l_arg, Value const& r_arg);

	const std::map<std::string, FunctionUnary> mapUnaryOperators = {
		{ "!", [](Value const& arg) { return !arg; } },
		{ "-", [](Value const& arg) { return Value() - arg; } } //TODO
	};
	
	const std::map<std::string, FunctionBinary> mapBinaryOperators = {
		{ "+", [](Value const& l_arg, Value const& r_arg) { return l_arg + r_arg; } },
		{ "-", [](Value const& l_arg, Value const& r_arg) { return l_arg - r_arg; } },
		{ "/", [](Value const& l_arg, Value const& r_arg) { return l_arg / r_arg; } },
		{ "*", [](Value const& l_arg, Value const& r_arg) { return l_arg * r_arg; } },
		{ "%", [](Value const& l_arg, Value const& r_arg) { return l_arg % r_arg; } },
		{ "==", [](Value const& l_arg, Value const& r_arg) { return l_arg == r_arg; } },
		{ "!=", [](Value const& l_arg, Value const& r_arg) { return l_arg != r_arg; } },
		{ ">", [](Value const& l_arg, Value const& r_arg) { return l_arg > r_arg; } },
		{ "<", [](Value const& l_arg, Value const& r_arg) { return l_arg < r_arg; } },
		{ ">=", [](Value const& l_arg, Value const& r_arg) { return l_arg >= r_arg; } },
		{ "<=", [](Value const& l_arg, Value const& r_arg) { return l_arg <= r_arg; } },
	};

	Value func_print(Value const& val);
	
	const std::map<std::string, FunctionUnary> mapUnaryFunctions = {
		{ "print", func_print },
	};

	const std::map<std::string, FunctionBinary> mapBinaryFunctions = {
		
	};
	
}
