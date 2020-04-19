#pragma once

#include "stdafx.h"

namespace Utils
{
	inline bool isQuotedString(std::string const& str)
	{
		return str.front() == '\"' && str.back() == '\"';
	}

	inline std::string removeQuotes(std::string str)
	{
		if (isQuotedString(str))
		{
			str.erase(0, 1);
			str.erase(str.size() - 2, 1);
		}

		return str;
	}

	class BaseException
	{
	public:
		BaseException()
		{
			m_pMessage = nullptr;
		}
		BaseException(const char* msg)
		{
			m_pMessage = new char[strlen(msg) + 1];
			strcpy(m_pMessage, msg);
		}
		virtual ~BaseException()
		{
			delete[] m_pMessage; // standard says its ok to delete nullptr (?)
		}
		const char* Message() const
		{
			if (m_pMessage == nullptr)
				return "";
			
			return m_pMessage;
		}
	protected:
		char* m_pMessage;
	};
}
