#pragma once
#include "BaseException.h"
#include "Logger.h"
#include "Operations.h"

class SerializationError : public BaseException
{
public:
	SerializationError() = default;
	SerializationError(std::string const& msg)
	{
		m_msg = msg;
	}
	virtual ~SerializationError() = default;
};


class Serializer : public Logger
{
public:
	Serializer()
	{
		SetLogName("Serializer");
	}

	OperationScopePtr Load(std::string const& fileName) const;
	void Store(OperationScopePtr pOperationTree, std::string const& fileName) const;
private:
	
};


