#include "pch.h"
#include "Serializer.h"

using namespace serializer;
using namespace runtime;

bool Serializer::Load()
{
	DebugLog("Loading operations tree...");
	try
	{
		m_isReady = false;
		BinaryFile file = openFile(m_fileName, true);
		m_pTree = std::make_shared<OperationScope>();

		uint8_t raw_type;
		file >> raw_type;
		if (static_cast<OperationType>(raw_type) != OperationType::OTScope)
		{
			throw serializer::SerializationError(serializer::msg_corruptedData);
		}
		m_pTree->operator<<(file);
		m_pTree->SetParentScope(m_pTree.get());

		closeFile(file);
		m_isReady = true;
	}
	catch(SerializationError const& e)
	{
		Log(e.Message());
		return false;
	}
	DebugLog("Operations tree is ready.");
	return true;
}

bool Serializer::Store(OperationScopePtr pOperationTree) const
{
	DebugLog("Storing operation tree to a file...");
	try
	{
		BinaryFile file = openFile(m_fileName, false);
		pOperationTree->operator>>(file);
		closeFile(file);
	}
	catch (SerializationError const& e)
	{
		Log(e.Message());
		return false;
	}
	DebugLog("Storing completed.");
	return true;
}

BinaryFile Serializer::openFile(std::string const& fileName, bool isReadOnly) const
{
	DebugLog("Opening file " + fileName + "...");
	const std::ios_base::openmode mode = std::_Iosb<int>::binary | (isReadOnly ? std::_Iosb<int>::in : (std::_Iosb<int>::out | std::_Iosb<int>::trunc));
	BinaryFile file(fileName, mode);

	if (!file.is_open())
	{
		throw SerializationError("Cannot open file " + fileName + ".");
	}
	DebugLog(std::string("File opened for ") + (isReadOnly ? "reading." : "writing."));
	
	return file;
}

void Serializer::closeFile(BinaryFile& file) const
{
	DebugLog("Closing file...");
	file.close();
}