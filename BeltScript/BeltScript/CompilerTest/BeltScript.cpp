// CompilerTest.cpp: определяет точку входа для консольного приложения.
//

#include <iostream>
#include <fstream>
#include <vector>
#include <cstdlib>
#include <chrono>

#include "Compiler.h"
#include "Runtime.h"
#include "../CommonLib/Utils.h"

const int retCodeSuccess = 0;
const int retCodeFail = -1;

void showHelp()
{
	std::cout << "Using of BeltScript.exe\n"
		"\tBeltScript.exe compile [file name] - compile BeltScript source code;\n"
		"\tBeltScript.exe run [compiled file name] - run compiled BeltScript program.\n"
		"\tBeltScript.exe runio [compiled file name] [inputs count] <inputs> [outputs count] - run compiled BeltScript program with inputs and outputs.\n";
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
	const std::string bltFileName = Utils::makeBltFileName(fileName);

	bool result;
	try
	{
		const auto start_time = std::chrono::system_clock::now();
		
		if (mode == "compile")
		{
			std::string code = readFile(fileName);
			result = Compile(bltFileName.c_str(), code.c_str(), &log);
		}
		else if (mode == "run")
		{
			result = Run(bltFileName.c_str(), log);
		}
		else if (mode == "runio")
		{
			const int inputsCount = std::atoi(args[2].c_str());			
			float* inputs = new float[inputsCount];

			int currentArg = 3;
			for (int i = 0; currentArg < inputsCount + 3 && currentArg < args.size(); currentArg++, i++)
			{
				inputs[i] = static_cast<float>(std::atof(args[currentArg].c_str()));
			}
			
			const int outputsCount = std::atoi(args[currentArg].c_str());
			float* outputs = new float[outputsCount];
			for (int i = 0; i < outputsCount; i++)
			{
				outputs[i] = 0.0f;
			}

			result = RunIO(bltFileName.c_str(), log, inputsCount, inputs, outputsCount, &outputs);
			
			if (result)
			{
				for (int i = 0; i < outputsCount; i++)
				{
					std::cout << "out" << i << ": " << outputs[i];
				}
			}

			delete[] inputs;
			delete[] outputs;
		}
		else
		{
			throw std::exception("Invalid arguments. Type 'help' to show usage tips.");
		}

		const auto end_time = std::chrono::system_clock::now();
		const std::chrono::duration<double> duration = end_time - start_time;
		std::cout << "execution time: " << duration.count() << '\n';
	}
	catch (std::exception& e)
	{
		std::cout << "Exception was captured in the main(): " << e.what() << '\n';
		return retCodeFail;
	}
	
    return retCodeSuccess;
}

