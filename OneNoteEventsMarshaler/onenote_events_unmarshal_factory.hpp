#pragma once

#include <atomic>

#include <Windows.h>

#include <wil/com.h>


class OneNoteEventsUnmarshalFactory final : public IClassFactory
{
private:
    std::atomic<ULONG> _reference_count = 1;
    wil::com_ptr<IUnknown> _ftm;

public:
    static wil::com_ptr<OneNoteEventsUnmarshalFactory> create_instance();

    OneNoteEventsUnmarshalFactory(OneNoteEventsUnmarshalFactory const &) = delete;
    OneNoteEventsUnmarshalFactory & operator=(OneNoteEventsUnmarshalFactory const &) = delete;

    OneNoteEventsUnmarshalFactory(OneNoteEventsUnmarshalFactory &&) = delete;
    OneNoteEventsUnmarshalFactory & operator=(OneNoteEventsUnmarshalFactory &&) = delete;

    HRESULT QueryInterface(IID const & riid, void ** ppvObject) noexcept override;
    ULONG AddRef() noexcept override;
    ULONG Release() noexcept override;

    HRESULT CreateInstance(IUnknown * pUnkOuter,
                           IID const & riid,
                           void ** ppvObject) noexcept override;
    HRESULT LockServer(BOOL fLock) noexcept override;

private:
    OneNoteEventsUnmarshalFactory();
    ~OneNoteEventsUnmarshalFactory() = default;
};
