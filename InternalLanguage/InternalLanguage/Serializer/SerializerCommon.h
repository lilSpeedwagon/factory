#pragma once
#include "BaseException.h"
#include <fstream>

namespace serializer
{
	typedef std::basic_fstream<uint8_t, std::char_traits<uint8_t>> BinaryFile;
	
	const std::string msg_corruptedData = "File is corrupted";
	
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

	class Serializable
	{
	public:
		virtual ~Serializable() = default;
		virtual BinaryFile& operator<<(BinaryFile&) = 0;
		virtual BinaryFile& operator>>(BinaryFile&) = 0;
	};

	// forward declaration for avoiding of loop dependencies in Operations.h and Serializer.h
	class Serializer;
}
