#pragma once

#include "stdafx.h"

namespace DataTypes
{
	inline bool isStringLiteral(std::string const& str);
	inline bool isBoolLiteral(std::string const& str);

	
	const static std::string BoolLiteralTrue = "true";
	const static std::string BoolLiteralFalse = "false";

}