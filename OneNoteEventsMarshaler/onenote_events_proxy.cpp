#include "onenote_events_proxy.hpp"

#include <marshal.hpp>


wil::com_ptr<OneNoteEventsProxy> OneNoteEventsProxy::create_instance(
    wil::com_ptr<IDispatch> const & remote_object)
{
    wil::com_ptr<OneNoteEventsProxy> result{};
    result.attach(new OneNoteEventsProxy(remote_object));
    return result;
}

HRESULT OneNoteEventsProxy::QueryInterface(IID const & riid, void ** ppvObject) noexcept
{
    if (nullptr == ppvObject)
    {
        return E_POINTER;
    }

    if (riid == __uuidof(IUnknown))
    {
        *ppvObject = static_cast<IUnknown *>(static_cast<IDispatch *>(this));
    }
    else if (riid == __uuidof(IDispatch))
    {
        *ppvObject = static_cast<IDispatch *>(this);
    }
    else if (riid == __uuidof(IOneNoteEvents))
    {
        *ppvObject = static_cast<IOneNoteEvents *>(this);
    }
    else if (riid == __uuidof(IMarshal))
    {
        *ppvObject = static_cast<IMarshal *>(this);
    }
    else
    {
        *ppvObject = nullptr;
        return E_NOINTERFACE;
    }

    static_cast<IUnknown *>(*ppvObject)->AddRef();

    return S_OK;
}

ULONG OneNoteEventsProxy::AddRef() noexcept
{
    return ++_reference_count;
}

ULONG OneNoteEventsProxy::Release() noexcept
{
    auto const new_refcount = --_reference_count;
    if (0 == new_refcount)
    {
        delete this;
    }
    return new_refcount;
}

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
