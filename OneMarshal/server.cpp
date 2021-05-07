#include <string>

#include <Windows.h>
#include <wrl.h>

#include <wil/resource.h>
#include <wil/stl.h>
#include <wil/win32_helpers.h>
#include <wil/result.h>

#include "onenote_events_unmarshal.hpp"


#define ONENOTE_EVENTS_UNMARSHAL_SUBKEY L"SOFTWARE\\Classes\\CLSID\\{" ONENOTE_EVENTS_UNMARSHAL_CLSID L"}"


namespace {

constexpr wchar_t kBothThreadingModel[] = L"Both";

std::wstring get_module_filename()
{
    std::wstring module_filename{};
    if (auto const result = wil::GetModuleFileNameW(wil::GetModuleInstanceHandle(), module_filename)
        ; FAILED(result))
    {
        THROW_HR(result);
    }
    return module_filename;
}

}


extern "C" HRESULT __stdcall DllRegisterServer() noexcept
try
{
    auto module_filename = get_module_filename();

    auto const module_filename_size = (module_filename.size() + 1) * sizeof(module_filename[0]);
    if (module_filename_size > MAXDWORD)
    {
        RETURN_HR(HRESULT_FROM_WIN32(ERROR_BUFFER_OVERFLOW));
    }

    wil::unique_hkey key{};
    auto result = RegCreateKeyExW(HKEY_CURRENT_USER,
                                  ONENOTE_EVENTS_UNMARSHAL_SUBKEY L"\\InProcServer32",
                                  0,
                                  nullptr,
                                  REG_OPTION_NON_VOLATILE,
                                  KEY_SET_VALUE,
                                  nullptr,
                                  &key,
                                  nullptr);
    if (ERROR_SUCCESS != result)
    {
        RETURN_HR(HRESULT_FROM_WIN32(result));
    }

    result = RegSetValueExW(key.get(),
                            nullptr,
                            0,
                            REG_SZ,
                            reinterpret_cast<BYTE const *>(module_filename.c_str()),
                            static_cast<DWORD>(module_filename_size));
    if (ERROR_SUCCESS != result)
    {
        RETURN_HR(HRESULT_FROM_WIN32(result));
    }

    result = RegSetValueExW(key.get(),
                            L"ThreadingModel",
                            0,
                            REG_SZ,
                            reinterpret_cast<BYTE const *>(&kBothThreadingModel[0]),
                            sizeof(kBothThreadingModel));
    if (ERROR_SUCCESS != result)
    {
        RETURN_HR(HRESULT_FROM_WIN32(result));
    }

    return S_OK;
}
CATCH_RETURN()

extern "C" HRESULT __stdcall DllUnregisterServer() noexcept
{
    if (auto const result = RegDeleteTreeW(HKEY_CURRENT_USER, ONENOTE_EVENTS_UNMARSHAL_SUBKEY);
        ERROR_SUCCESS != result)
    {
        RETURN_HR(HRESULT_FROM_WIN32(result));
    }

    return S_OK;
}

extern "C" HRESULT __stdcall DllGetClassObject(CLSID const & clsid,
                                               IID const & iid,
                                               void ** ppv) /*noexcept*/
{
    return Microsoft::WRL::Module<Microsoft::WRL::InProc>::GetModule().GetClassObject(
        clsid,
        iid,
        ppv);
}

extern "C" HRESULT __stdcall DllCanUnloadNow() /*noexcept*/
{
    return Microsoft::WRL::Module<Microsoft::WRL::InProc>::GetModule().Terminate() ? S_OK : S_FALSE;
}
