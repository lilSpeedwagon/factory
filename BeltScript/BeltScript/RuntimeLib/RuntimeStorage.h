#pragma once
#include <map>
#include "Operations.h"

class RuntimeStorage
{
public:
	static RuntimeStorage& GetInstance();
	
	~RuntimeStorage() = default;

	OperationScopePtr GetTree(std::string const& fileName);
	void StoreTree(std::string const& fileName, OperationScopePtr operationTree);

private:
	RuntimeStorage() = default;

	static RuntimeStorage* g_instance;
	std::map<std::string, OperationScopePtr> m_storage;
};

