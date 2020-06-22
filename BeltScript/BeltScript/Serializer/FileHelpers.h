#pragma once
#include "SerializerCommon.h"

inline uint8_t boolToU8(bool b)
{
	return static_cast<uint8_t>(b ? 1u : 0u);
}
inline bool U8toBool(uint8_t v)
{
	return v == 1u;
}

serializer::BinaryFile& operator<<(serializer::BinaryFile& file, std::string const& str);
serializer::BinaryFile& operator>>(serializer::BinaryFile& file, std::string& str);

serializer::BinaryFile& operator<<(serializer::BinaryFile& file, void* const& pRaw);
serializer::BinaryFile& operator>>(serializer::BinaryFile& file, void* & pRaw);


// only for unsigned arithmetic types
template<typename T, typename = typename std::enable_if<std::is_arithmetic<T>::value && std::is_unsigned<T>::value, T>::type>
serializer::BinaryFile& operator<<(serializer::BinaryFile& file, T const& num)
{
	constexpr size_t buffer_size = sizeof(num);
	uint8_t buffer[buffer_size];

	for (size_t i = 0; i < buffer_size; i++)
	{
		buffer[i] = num >> (i * 8);
	}
	file.write(buffer, buffer_size);
	
	return file;
}

template<typename T, typename = typename std::enable_if<std::is_arithmetic<T>::value && std::is_unsigned<T>::value, T>::type>
serializer::BinaryFile& operator>>(serializer::BinaryFile& file, T & num)
{
	constexpr size_t buffer_size = sizeof(num);
	uint8_t buffer[buffer_size];

	file.read(buffer, buffer_size);
	
	T n = 0;
	for (size_t i = 0; i < buffer_size; i++)
	{
		n += static_cast<T>(buffer[i]) << (i * 8);
	}
	num = n;
	
	return file;
}
