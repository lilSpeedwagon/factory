#pragma once

#include "stdafx.h"
#include "Utils.h"

namespace DataTypes
{
	const static std::string BoolLiteralTrue = "true";
	const static std::string BoolLiteralFalse = "false";
	
	inline bool isStringLiteral(std::string const& str)
	{
		return Utils::isQuotedString(str);
	}
	
	inline bool isBoolLiteral(std::string const& str)
	{
		return str == BoolLiteralTrue || str == BoolLiteralFalse;
	}
	

}
