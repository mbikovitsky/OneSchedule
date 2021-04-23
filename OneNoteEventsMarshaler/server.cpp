#include "server.hpp"

#include <string>
#include <atomic>
#include <cassert>

#include <Windows.h>

#include <wil/resource.h>
#include <wil/stl.h>
#include <wil/win32_helpers.h>
#include <wil/result.h>

#include "onenote_events_unmarshal.hpp"
#include "onenote_events_unmarshal_factory.hpp"


#define ONENOTE_EVENTS_UNMARSHAL_SUBKEY L"SOFTWARE\\Classes\\CLSID\\{" ONENOTE_EVENTS_UNMARSHAL_CLSID L"}"


namespace {

constexpr wchar_t kBothThreadingModel[] = L"Both";

std::atomic<ULONG> g_lock_count = 0;
std::atomic<ULONG> g_object_count = 0;

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


void Server::lock() noexcept
{
    ++g_lock_count;
}

void Server::unlock() noexcept
{
    ULONG count = 0;
    do
    {
        count = g_lock_count.load();
        if (0 == count)
        {
            break;
        }
    }
    while (!g_lock_count.compare_exchange_weak(count, count - 1));
}

void Server::notify_object_created() noexcept
{
    ++g_object_count;
}

void Server::notify_object_destroyed() noexcept
{
    auto const old_object_count = g_object_count--;
    assert(0 != old_object_count);
}


extern "C" HRESULT __stdcall DllRegisterServer() noexcept
try
{
    auto module_filename = get_module_filename();

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
                            (module_filename.size() + 1) * sizeof(module_filename[0]));
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
try
{
    if (auto const result = RegDeleteTreeW(HKEY_CURRENT_USER, ONENOTE_EVENTS_UNMARSHAL_SUBKEY);
        ERROR_SUCCESS != result)
    {
        RETURN_HR(HRESULT_FROM_WIN32(result));
    }

    return S_OK;
}
CATCH_RETURN()

extern "C" HRESULT __stdcall DllGetClassObject(CLSID const & clsid,
                                               IID const & iid,
                                               void ** ppv) /*noexcept*/
try
{
    if (nullptr == ppv)
    {
        return E_POINTER;
    }
    *ppv = nullptr;

    if (clsid == __uuidof(OneNoteEventsUnmarshal))
    {
        return OneNoteEventsUnmarshalFactory::create_instance()->QueryInterface(iid, ppv);
    }
    else
    {
        return CLASS_E_CLASSNOTAVAILABLE;
    }
}
CATCH_RETURN()

extern "C" HRESULT __stdcall DllCanUnloadNow() /*noexcept*/
{
    return (0 == g_lock_count) && (0 == g_object_count);
}
