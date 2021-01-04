#pragma once
#include "pch.h"
#include "BaseException.h"
#include "Value.h"
#define _USE_MATH_DEFINES 
#include <math.h>
#include "Definitions.h"

namespace runtime
{
	typedef float IOType;
	typedef std::vector<IOType> Inputs;
	typedef std::vector<IOType> Outputs;

	struct Variable
	{
		Variable(std::string const& name) : identifier(name) {}
		std::string identifier;
		Value value;
	};

	typedef Value(*FunctionZeroArgs)();
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

	const std::map<std::string, FunctionZeroArgs> mapZeroArgsFunctions = {
		{ "pi", []() { return Value(static_cast<float>(M_PI)); } },
	};

	const std::map<std::string, FunctionUnary> mapUnaryFunctions = {
		{ "print", func_print },
		{ "sin", [](Value const& arg) { return Value(sin(arg.toFloat().getValue<float>())); } },
		{ "cos", [](Value const& arg) { return Value(cos(arg.toFloat().getValue<float>())); } },
		{ "tan", [](Value const& arg) { return Value(tan(arg.toFloat().getValue<float>())); } },
		{ "abs", [](Value const& arg) { return Value(abs(arg.toFloat().getValue<float>())); } },
		{ "sqrt", [](Value const& arg) { return Value(sqrt(arg.toFloat().getValue<float>())); } },
	};

	const std::map<std::string, FunctionBinary> mapBinaryFunctions = {
		{ "pow", [](Value const& l_arg, Value const& r_arg) { return Value(static_cast<float>(pow(l_arg.toFloat().getValue<float>(), r_arg.toInt().getValue<int>()))); } },
	};

	template<typename T>
	T findFunction(std::map<std::string, T> const& funcMap, std::string const& funcName)
	{
		T func = nullptr;
		auto it = funcMap.find(funcName);
		if (it != funcMap.end())
		{
			func = it->second;
		}
		return func;
	}

	inline FunctionZeroArgs findZeroArgsFunction(std::string const& funcName)
	{
		auto it = mapZeroArgsFunctions.find(funcName);
		if (it == mapZeroArgsFunctions.end())
			return nullptr;
		return it->second;
	}

	inline FunctionUnary findUnaryFunction(std::string const& funcName)
	{
		auto it = mapUnaryFunctions.find(funcName);
		if (it == mapUnaryFunctions.end())
		{
			it = mapUnaryOperators.find(funcName);
			if (it == mapUnaryOperators.end())
				return nullptr;
		}
		return it->second;
	}

	inline FunctionBinary findBinaryFunction(std::string const& funcName)
	{
		auto it = mapBinaryFunctions.find(funcName);
		if (it == mapBinaryFunctions.end())
		{
			it = mapBinaryOperators.find(funcName);
			if (it == mapBinaryOperators.end())
				return nullptr;
		}
		return it->second;
	}
}
