// CompilerTest.cpp: определяет точку входа для консольного приложения.
//

#include <iostream>
#include "Compiler.h"
#include "fstream"

void __stdcall log(const char* msg)
{
	std::cout << msg << std::endl;
}

std::string readFile(const char* fileName)
{
	std::string code;
	
	std::fstream f;
	f.open(fileName);

	if (f.is_open())
	{
		f.seekg(0, std::ios::end);
		int length = static_cast<int>(f.tellg());
		char* buffer = new char[length];

		f.seekg(0, std::ios::beg);
		f.read(buffer, length);
		buffer[length - 1] = '\0';	// instead of EOF char
		code.assign(buffer);

		delete[] buffer;
		f.close();
	}
	else
	{
		throw new std::exception("cannot open code file");
	}

	return code;
}

int main(int argc, char *argv[])
{
	if (argc < 2)
	{
		log("Nothing to compile");
		return 0;
	}

	char* fileName = argv[1];
	
	try
	{
		std::string code = readFile(fileName);
		Compile(code.c_str(), &log);
	}
	catch (std::exception& e)
	{
		log(e.what());
	}
	
    return 0;
}

