#pragma once

extern "C"
{
	bool __declspec(dllexport) __stdcall Compile(const char * fileName, const char * code, void(__stdcall* log)(const char*));
}