#include "onenote_events_proxy.hpp"

#include "onenote_events_unmarshal.hpp"


HRESULT OneNoteEventsProxy::GetUnmarshalClass(IID const & /*riid*/,
                                              void * /*pv*/,
                                              DWORD /*dwDestContext*/,
                                              void * /*pvDestContext*/,
                                              DWORD /*mshlflags*/,
                                              CLSID * pCid) noexcept
{
    if (nullptr == pCid)
    {
        return E_POINTER;
    }
    *pCid = __uuidof(OneNoteEventsUnmarshal);
    return S_OK;
}

HRESULT OneNoteEventsProxy::GetMarshalSizeMax(IID const & /*riid*/,
                                              void * /*pv*/,
                                              DWORD dwDestContext,
                                              void * pvDestContext,
                                              DWORD mshlflags,
                                              DWORD * pSize) noexcept
{
    return CoGetMarshalSizeMax(pSize,
                               __uuidof(IDispatch),
                               _remote_object.get(),
                               dwDestContext,
                               pvDestContext,
                               mshlflags);
}

HRESULT OneNoteEventsProxy::MarshalInterface(IStream * pStm,
                                             IID const & /*riid*/,
                                             void * /*pv*/,
                                             DWORD dwDestContext,
                                             void * pvDestContext,
                                             DWORD mshlflags) noexcept
{
    return CoMarshalInterface(pStm,
                              __uuidof(IDispatch),
                              _remote_object.get(),
                              dwDestContext,
                              pvDestContext,
                              mshlflags);
}

HRESULT OneNoteEventsProxy::UnmarshalInterface(IStream * /*pStm*/,
                                               IID const & /*riid*/,
                                               void ** /*ppv*/) noexcept
{
    return E_NOTIMPL;
}

HRESULT OneNoteEventsProxy::ReleaseMarshalData(IStream * pStm) noexcept
{
    return CoReleaseMarshalData(pStm);
}

HRESULT OneNoteEventsProxy::DisconnectObject(DWORD /*dwReserved*/) noexcept
{
    return E_NOTIMPL;
}

HRESULT OneNoteEventsProxy::GetTypeInfoCount(UINT * pctinfo) noexcept
{
    return _remote_object->GetTypeInfoCount(pctinfo);
}

HRESULT OneNoteEventsProxy::GetTypeInfo(UINT iTInfo, LCID lcid, ITypeInfo ** ppTInfo) noexcept
{
    return _remote_object->GetTypeInfo(iTInfo, lcid, ppTInfo);
}

HRESULT OneNoteEventsProxy::GetIDsOfNames(IID const & riid,
                                          LPOLESTR * rgszNames,
                                          UINT cNames,
                                          LCID lcid,
                                          DISPID * rgDispId) noexcept
{
    return _remote_object->GetIDsOfNames(riid, rgszNames, cNames, lcid, rgDispId);
}

HRESULT OneNoteEventsProxy::Invoke(DISPID dispIdMember,
                                   IID const & riid,
                                   LCID lcid,
                                   WORD wFlags,
                                   DISPPARAMS * pDispParams,
                                   VARIANT * pVarResult,
                                   EXCEPINFO * pExcepInfo,
                                   UINT * puArgErr) noexcept
{
    return _remote_object->Invoke(dispIdMember,
                                  riid,
                                  lcid,
                                  wFlags,
                                  pDispParams,
                                  pVarResult,
                                  pExcepInfo,
                                  puArgErr);
}

OneNoteEventsProxy::OneNoteEventsProxy(wil::com_ptr<IDispatch> remote_object)
    : _remote_object(std::move(remote_object))
{
}
