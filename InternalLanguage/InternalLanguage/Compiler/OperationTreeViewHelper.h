#pragma once

#include "Definitions.h"
#include "sstream"

class TreeHelper
{
public:
	virtual ~TreeHelper() = default;
	virtual void ExtendView(std::stringstream& ss, int nLevel) = 0;
};

inline void make_indent(std::stringstream& ss, int nLevel)
{
	for (int i = 0; i < nLevel; i++) ss << "  ";
}
