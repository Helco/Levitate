// based on https://raw.githubusercontent.com/SohaibJundi/Inject.Net/main/Inject.Net.Core.Loader/dllmain.cpp

#define STRINGIFY_MACRO(x) STR(x)
#define STR(x) #x
#define EXPAND(x) x
#define COMBINE_PATH(path, file) STRINGIFY_MACRO(EXPAND(path)##file)

#ifdef WIN32
#define ARCH x86
#else
#define ARCH x64
#endif
#define RUNTIME 7.0.1 // Change this value depending on the version(s) available in "C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Host.win-{ARCH}"
#define NETHOST_USE_AS_STATIC
#define LIB_PATH C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Host.win-ARCH\\RUNTIME\\runtimes\\win-ARCH\\native/
#include COMBINE_PATH(LIB_PATH, nethost.h)
#include COMBINE_PATH(LIB_PATH, coreclr_delegates.h)
#include COMBINE_PATH(LIB_PATH, hostfxr.h)
#pragma comment(lib, COMBINE_PATH(LIB_PATH, libnethost.lib))
#include <Windows.h>
#include <string>
#include "../DetoursSource/detours.h"
#include <thread>

int LoadDll(LPCWSTR dll, LPCWSTR config, LPCWSTR type, LPCWSTR method, LPCWSTR arg)
{
    wchar_t buf[MAX_PATH];
    auto buf_len = sizeof(buf) / sizeof(wchar_t);
    get_hostfxr_path(buf, &buf_len, nullptr);
    const auto lib = LoadLibraryW(buf);
    hostfxr_handle cxt;
    const auto init = reinterpret_cast<hostfxr_initialize_for_runtime_config_fn>(GetProcAddress(lib, "hostfxr_initialize_for_runtime_config"));
    init(config, nullptr, &cxt);
    load_assembly_and_get_function_pointer_fn get_managed_function;
    const auto get_delegate = reinterpret_cast<hostfxr_get_runtime_delegate_fn>(GetProcAddress(lib, "hostfxr_get_runtime_delegate"));
    get_delegate(cxt, hdt_load_assembly_and_get_function_pointer, reinterpret_cast<void**>(&get_managed_function));
    const auto close = reinterpret_cast<hostfxr_close_fn>(GetProcAddress(lib, "hostfxr_close"));
    close(cxt);
    component_entry_point_fn method_fn;
    get_managed_function(dll, type, method, nullptr, nullptr, reinterpret_cast<void**>(&method_fn));

    return method_fn(const_cast<LPWSTR>(arg), wcslen(arg));
}

void LoadHost(HMODULE hModule)
{
    wchar_t modulePath[MAX_PATH];
    DWORD modulePathLen = GetModuleFileNameW(hModule, modulePath, MAX_PATH);
    std::wstring basePath(modulePath, modulePathLen - strlen("LevitateLoader.dll"));
    std::wstring dllPath = basePath + L"Levitate.dll";
    std::wstring configPath = basePath + L"StartLevitate.runtimeconfig.json";

    LoadDll(dllPath.c_str(), configPath.c_str(), L"Levitate.Injector, Levitate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", L"Inject", L"");
}

typedef int(__stdcall* WinMainFunc)(int, int, int, int);
WinMainFunc originalWinMain = reinterpret_cast<WinMainFunc>(0x45021e);
HMODULE myModule;

int __stdcall NewWinMain(int a, int b, int c, int d)
{
    // We delay loading CLR to main as starting up is much much faster
    LoadHost(myModule);
    return originalWinMain(a, b, c, d);
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    if (ul_reason_for_call == DLL_PROCESS_ATTACH)
    {
        myModule = hModule;
        DetourRestoreAfterWith();
        DetourTransactionBegin();
        DetourAttach((void**)&originalWinMain, NewWinMain);
        DetourTransactionCommit();
    }

    return TRUE;
}
