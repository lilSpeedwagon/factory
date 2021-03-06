#pragma once

extern "C"
{
	bool __declspec(dllexport) __stdcall Run(const char * codeFileName, void(__stdcall* log)(const char*));
	bool __declspec(dllexport) __stdcall RunIO(const char * codeFileName, void(__stdcall* log)(const char*), int inputsCount, float inputs[], int outputsCount, float** outputs);
	bool __declspec(dllexport) __stdcall RemoveFromCash(const char* codeFileName);
}