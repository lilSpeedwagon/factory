#pragma once
#include "SerializerCommon.h"
#include "Logger.h"
#include "Operations.h"

namespace serializer
{
	class Serializer : public Logger
	{
	public:
		Serializer(std::string const& fileName, LogDelegate log) :
			m_fileName(fileName), m_pTree(nullptr), m_isReady(false)
		{
			SetLogDelegate(log);
			SetLogName("Serializer");
		}

		bool Load();
		bool Store(OperationScopePtr pOperationTree) const;
		OperationScopePtr GetTree() const { return m_pTree; };
		bool isLoaded() const { return m_isReady; }
	private:
		BinaryFile openFile(std::string const& fileName, bool isReadOnly = true) const;
		void closeFile(BinaryFile& file) const;

		std::string m_fileName;
		OperationScopePtr m_pTree;
		bool m_isReady;
	};
}

