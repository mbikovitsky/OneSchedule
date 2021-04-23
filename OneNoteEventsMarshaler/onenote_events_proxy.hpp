#pragma once

#include "pch.hpp"


class OneNoteEventsProxy final : public OneNote::IOneNoteEvents, public IMarshal
{
private:
    std::atomic<ULONG> _reference_count = 1;
    wil::com_ptr<IDispatch> _remote_object;

public:
    static wil::com_ptr<OneNoteEventsProxy> create_instance(
        wil::com_ptr<IDispatch> const & remote_object);

    OneNoteEventsProxy(OneNoteEventsProxy const &) = delete;
    OneNoteEventsProxy & operator=(OneNoteEventsProxy const &) = delete;

    OneNoteEventsProxy(OneNoteEventsProxy &&) = delete;
    OneNoteEventsProxy & operator=(OneNoteEventsProxy &&) = delete;

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

    HRESULT GetTypeInfoCount(UINT * pctinfo) noexcept override;
    HRESULT GetTypeInfo(UINT iTInfo, LCID lcid, ITypeInfo ** ppTInfo) noexcept override;
    HRESULT GetIDsOfNames(IID const & riid,
                          LPOLESTR * rgszNames,
                          UINT cNames,
                          LCID lcid,
                          DISPID * rgDispId) noexcept override;
    HRESULT Invoke(DISPID dispIdMember,
                   IID const & riid,
                   LCID lcid,
                   WORD wFlags,
                   DISPPARAMS * pDispParams,
                   VARIANT * pVarResult,
                   EXCEPINFO * pExcepInfo,
                   UINT * puArgErr) noexcept override;

private:
    explicit OneNoteEventsProxy(wil::com_ptr<IDispatch> remote_object);

    ~OneNoteEventsProxy() = default;
};
