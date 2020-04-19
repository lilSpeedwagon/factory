#include "stdafx.h"
#include "DataTypes.h"
#include "Utils.h"
#include <cctype>

bool DataTypes::isStringLiteral(std::string const& str)
{
	return Utils::isQuotedString(str);
}

bool DataTypes::isBoolLiteral(std::string const& str)
{
	return str == BoolLiteralTrue || str == BoolLiteralFalse;
}
