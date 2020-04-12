// CompilerTest.cpp: определяет точку входа для консольного приложения.
//

#include <iostream>
#include "Compiler.h"

void __stdcall log(const char* msg)
{
	std::cout << msg << std::endl;
}

int main()
{
	log("start");
	char c;
	std::cin >> c;
	Compile("name = 123;a=1;                 biglongnumber_with_additional-shit;not.a.number=1.23;var=5.5.5;////////////// hallo", &log);
	log("end");
    return 0;
}

