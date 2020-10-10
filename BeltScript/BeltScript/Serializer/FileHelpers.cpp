#include "pch.h"
#include "FileHelpers.h"
#include "Utils.h"

#ifdef _WIN64
	typedef uint64_t PtrSizeInteger;
#else
	typedef uint32_t PtrSizeInteger;
#endif

static_assert(sizeof(PtrSizeInteger) == sizeof(void*));

serializer::BinaryFile& operator<<(serializer::BinaryFile& file, std::string const& str)
{
	const uint16_t size = static_cast<uint16_t>(str.size());
	file << size;
	
	uint8_t* buffer = new uint8_t[size];
	const errno_t err = memcpy_s(buffer, size, str.c_str(), size);
	if (err == 0)
	{
		file.write(buffer, size);
		delete[] buffer;
	}
	else
	{
		delete[] buffer;
		throw serializer::SerializationError("Cannot write string to the file. Error code: " + Utils::toString(err));
	}
	return file;
}

serializer::BinaryFile& operator>>(serializer::BinaryFile& file, std::string& str)
{
	uint16_t size;
	file >> size;
	
	str.resize(size);

	uint8_t* buffer = new uint8_t[size];
	file.read(buffer, size);
	const errno_t err = memcpy_s(str.data(), size, buffer, size);
	delete[] buffer;

	if (err != 0)
	{
		throw serializer::SerializationError("Cannot read string from the file. Error code: " + Utils::toString(err));
	}

	return file;
}

serializer::BinaryFile& operator<<(serializer::BinaryFile& file, void* const& pRaw)
{
	const PtrSizeInteger n = reinterpret_cast<PtrSizeInteger>(pRaw);
	for (size_t i = 0; i < sizeof(void*); i++)
	{
		const uint8_t rawByte = static_cast<uint8_t>(n >> (i * 8));
		file << rawByte;
	}
	return file;
}

serializer::BinaryFile& operator>>(serializer::BinaryFile& file, void*& pRaw)
{
	PtrSizeInteger n = 0;
	for (size_t i = 0; i < sizeof(void*); i++)
	{
		uint8_t rawByte = 0;
		file >> rawByte;
		n += static_cast<PtrSizeInteger>(rawByte) << (i * 8);
	}
	pRaw = reinterpret_cast<void*>(n);
	return file;
}
