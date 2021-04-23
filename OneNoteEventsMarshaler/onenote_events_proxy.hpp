#pragma once

#include <Windows.h>
#include <wrl.h>

#include <wil/com.h>

#include <onenote.hpp>


class OneNoteEventsProxy final
    : public Microsoft::WRL::RuntimeClass<
        Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::ClassicCom |
                                          Microsoft::WRL::InhibitFtmBase>,
        OneNote::IOneNoteEvents,
        IDispatch,
        IMarshal>
{
private:
    wil::com_ptr<IDispatch> _remote_object;

public:
    explicit OneNoteEventsProxy(wil::com_ptr<IDispatch> remote_object);

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
};
