#include "onenote_events_unmarshal.hpp"

#include "onenote_events_proxy.hpp"


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
        auto proxy = Microsoft::WRL::Make<OneNoteEventsProxy>(remote_object);
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
