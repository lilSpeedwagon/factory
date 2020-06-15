// CompilerTest.cpp: определяет точку входа для консольного приложения.
//

#include <iostream>
#include <fstream>
#include <vector>

#include "Compiler.h"
#include "Runtime.h"

const int retCodeSuccess = 0;
const int retCodeFail = -1;

void showHelp()
{
	std::cout << "Using of BeltScript.exe\n"
		"\tBeltScript.exe compile [file name] - compile BeltScript source code;\n"
		"\tBeltScript.exe run [compiled file name] - run compiled BeltScript program.\n";
}

void showIntro()
{
	std::cout << "BeltScript. Egor Sorokin 2020\n\n";
}

void __stdcall log(const char* msg)
{
	std::cout << msg << '\n';
}

std::string readFile(std::string const& fileName)
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
		throw std::exception("Cannot open file");
	}

	return code;
}

int main(int argc, char *argv[])
{
	showIntro();
	
#ifdef _DEBUG
	std::cout << "DEBUG MODE. Press any key to continue...\n";
	system("pause");
#endif

	std::vector<std::string> args;
	for (int i = 1; i < argc; i++)
	{
		args.emplace_back(argv[i]);
	}
	
	if (args.size() == 1 && (args[0] == "help" || args[0] == "/?"))
	{
		showHelp();
		return retCodeSuccess;
	}
	
	if (args.size() < 2)
	{
		log("Incorrect number of arguments. Type 'help' to show usage tips.");
		return retCodeFail;
	}

	const std::string mode = args[0];
	const std::string fileName = args[1];

	bool result;
	try
	{
		if (mode == "compile")
		{
			std::string code = readFile(fileName);
			result = Compile(fileName.c_str(), code.c_str(), &log);
		}
		else if (mode == "run")
		{
			result = Run(fileName.c_str(), log);
		}
		else
		{
			throw std::exception("Invalid arguments. Type 'help' to show usage tips.");
		}
	}
	catch (std::exception& e)
	{
		log(e.what());
		return retCodeFail;
	}
	
    return retCodeSuccess;
}

