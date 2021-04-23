#pragma once

#include <atomic>

#include <Windows.h>

#include <wil/com.h>


class OneNoteEventsUnmarshal final : public IMarshal
{
private:
    std::atomic<ULONG> _reference_count = 1;

public:
    static wil::com_ptr<OneNoteEventsUnmarshal> create_instance();

    OneNoteEventsUnmarshal(OneNoteEventsUnmarshal const &) = delete;
    OneNoteEventsUnmarshal & operator=(OneNoteEventsUnmarshal const &) = delete;

    OneNoteEventsUnmarshal(OneNoteEventsUnmarshal &&) = delete;
    OneNoteEventsUnmarshal & operator=(OneNoteEventsUnmarshal &&) = delete;

    HRESULT QueryInterface(IID const & riid, void ** ppvObject) noexcept override;
    ULONG AddRef() noexcept override;
    ULONG Release() noexcept override;

    HRESULT GetUnmarshalClass(IID const & riid,
                              void * pv,
                              DWORD dwDestContext,
                              void * pvDestContext,
                              DWORD mshlflags,
                              CLSID * pCid) noexcept override;
    HRESULT GetMarshalSizeMax(IID const & riid,
                              void * pv,
                              DWORD dwDestContext,
                              void * pvDestContext,
                              DWORD mshlflags,
                              DWORD * pSize) noexcept override;
    HRESULT MarshalInterface(IStream * pStm,
                             IID const & riid,
                             void * pv,
                             DWORD dwDestContext,
                             void * pvDestContext,
                             DWORD mshlflags) noexcept override;
    HRESULT UnmarshalInterface(IStream * pStm, IID const & riid, void ** ppv) noexcept override;
    HRESULT ReleaseMarshalData(IStream * pStm) noexcept override;
    HRESULT DisconnectObject(DWORD dwReserved) noexcept override;

private:
    OneNoteEventsUnmarshal() = default;
    ~OneNoteEventsUnmarshal() = default;
};
