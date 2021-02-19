#include "pch.h"
#include "RuntimeStorage.h"

RuntimeStorage* RuntimeStorage::g_instance = nullptr;

RuntimeStorage& RuntimeStorage::GetInstance()
{
	if (g_instance == nullptr)
	{
		g_instance = new RuntimeStorage();
	}
	return *g_instance;
}


OperationScopePtr RuntimeStorage::GetTree(std::string const& fileName)
{
	const auto tree = m_storage.find(fileName);
	if (tree == m_storage.end())
	{
		return nullptr;
	}
	return tree->second;
}

void RuntimeStorage::StoreTree(std::string const& fileName, OperationScopePtr operationTree)
{
	m_storage[fileName] = operationTree;
}


