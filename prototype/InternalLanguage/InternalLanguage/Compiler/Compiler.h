#pragma once

void __declspec(dllexport) __stdcall Compile(const char * code, void(__stdcall* log)(const char*));