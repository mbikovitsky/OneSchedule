#include "onenote_events_unmarshal.hpp"

#include "onenote_events_proxy.hpp"


wil::com_ptr<OneNoteEventsUnmarshal> OneNoteEventsUnmarshal::create_instance()
{
    wil::com_ptr<OneNoteEventsUnmarshal> result{};
    result.attach(new OneNoteEventsUnmarshal());
    return result;
}

HRESULT OneNoteEventsUnmarshal::QueryInterface(IID const & riid, void ** ppvObject) noexcept
{
    if (nullptr == ppvObject)
    {
        return E_POINTER;
    }

    if (riid == __uuidof(IUnknown))
    {
        *ppvObject = static_cast<IUnknown *>(this);
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

ULONG OneNoteEventsUnmarshal::AddRef() noexcept
{
    return ++_reference_count;
}

ULONG OneNoteEventsUnmarshal::Release() noexcept
{
    auto const new_refcount = --_reference_count;
    if (0 == new_refcount)
    {
        delete this;
    }
    return new_refcount;
}

HRESULT OneNoteEventsUnmarshal::GetUnmarshalClass(IID const & /*riid*/,
                                                  void * /*pv*/,
                                                  DWORD /*dwDestContext*/,
                                                  void * /*pvDestContext*/,
                                                  DWORD /*mshlflags*/,
                                                  CLSID * /*pCid*/) noexcept
{
    return E_NOTIMPL;
}

HRESULT OneNoteEventsUnmarshal::GetMarshalSizeMax(IID const & /*riid*/,
                                                  void * /*pv*/,
                                                  DWORD /*dwDestContext*/,
                                                  void * /*pvDestContext*/,
                                                  DWORD /*mshlflags*/,
                                                  DWORD * /*pSize*/) noexcept
{
    return E_NOTIMPL;
}

HRESULT OneNoteEventsUnmarshal::MarshalInterface(IStream * /*pStm*/,
                                                 IID const & /*riid*/,
                                                 void * /*pv*/,
                                                 DWORD /*dwDestContext*/,
                                                 void * /*pvDestContext*/,
                                                 DWORD /*mshlflags*/) noexcept
{
    return E_NOTIMPL;
}

HRESULT OneNoteEventsUnmarshal::UnmarshalInterface(IStream * pStm,
                                                   IID const & riid,
                                                   void ** ppv) noexcept
{
    wil::com_ptr<IDispatch> remote_object{};
    if (auto const result =
            CoUnmarshalInterface(pStm, __uuidof(IDispatch), remote_object.put_void());
        FAILED(result))
    {
        RETURN_HR(result);
    }

    try
    {
        auto proxy = OneNoteEventsProxy::create_instance(remote_object);
        return proxy->QueryInterface(riid, ppv);
    }
    CATCH_RETURN()
}

HRESULT OneNoteEventsUnmarshal::ReleaseMarshalData(IStream * pStm) noexcept
{
    return CoReleaseMarshalData(pStm);
}

HRESULT OneNoteEventsUnmarshal::DisconnectObject(DWORD /*dwReserved*/) noexcept
{
    return E_NOTIMPL;
}
