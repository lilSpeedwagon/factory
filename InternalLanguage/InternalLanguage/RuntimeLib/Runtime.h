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

	typedef void(*Procedure)();
	typedef void(*ProcedureUnary)(Value const& arg);
	typedef void(*ProcedureBinary)(Value const& l_arg, Value const& r_arg);
	typedef Value(*Function)();
	typedef Value(*FunctionUnary)(Value const& arg);
	typedef Value(*FunctionBinary)(Value const& l_arg, Value const& r_arg);

	const std::map<std::string, FunctionUnary> mapUnaryFunctions = {
		{ "!", [](Value const& arg) { return !arg; } }
	};
	
	const std::map<std::string, FunctionBinary> mapBinaryFunctions = {
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
	
	
}
