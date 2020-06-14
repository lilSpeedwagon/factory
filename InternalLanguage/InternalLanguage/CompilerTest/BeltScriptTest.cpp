// CompilerTest.cpp: определяет точку входа для консольного приложения.
//

#include <iostream>
#include <fstream>
#include "Compiler.h"
#include "Runtime.h"

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
		char* buffer = new char[length + 1];

		f.seekg(0, std::ios::beg);
		f.read(buffer, length);
		buffer[length] = '\0';	// instead of EOF char
		code.assign(buffer);

		delete[] buffer;
		f.close();
	}
	else
	{
		throw std::exception("cannot open code file");
	}

	return code;
}

int main(int argc, char *argv[])
{
	char c;
	std::cin >> c;	//TODO delete
	
	if (argc < 3)
	{
		log("Incorrect number of args");
		return 0;
	}

	const std::string mode = argv[1];
	char* fileName = argv[2];

	bool result;
	try
	{
		if (mode == "compile")
		{
			std::string code = readFile(fileName);
			result = Compile(fileName, code.c_str(), &log);

		}
		else if (mode == "run")
		{
			result = Run(fileName, log);
		}
		else
		{
			throw std::exception("wrong mode");
		}

		if (!result)
		{
			throw std::exception("false result");
		}
	}
	catch (std::exception& e)
	{
		log(e.what());
	}
	
    return 0;
}

