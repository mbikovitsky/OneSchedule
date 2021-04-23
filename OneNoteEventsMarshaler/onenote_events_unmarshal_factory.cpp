#include "onenote_events_unmarshal_factory.hpp"

#include "onenote_events_unmarshal.hpp"


wil::com_ptr<OneNoteEventsUnmarshalFactory> OneNoteEventsUnmarshalFactory::create_instance()
{
    wil::com_ptr<OneNoteEventsUnmarshalFactory> result{};
    result.attach(new OneNoteEventsUnmarshalFactory());
    return result;
}

HRESULT OneNoteEventsUnmarshalFactory::QueryInterface(IID const & riid, void ** ppvObject) noexcept
{
    if (nullptr == ppvObject)
    {
        return E_POINTER;
    }

    if (riid == __uuidof(IUnknown))
    {
        *ppvObject = static_cast<IUnknown *>(this);
    }
    else if (riid == __uuidof(IClassFactory))
    {
        *ppvObject = static_cast<IClassFactory *>(this);
    }
    else if (riid == __uuidof(IMarshal))
    {
        return _ftm->QueryInterface(riid, ppvObject);
    }
    else
    {
        *ppvObject = nullptr;
        return E_NOINTERFACE;
    }

    static_cast<IUnknown *>(*ppvObject)->AddRef();

    return S_OK;
}

ULONG OneNoteEventsUnmarshalFactory::AddRef() noexcept
{
    return ++_reference_count;
}

ULONG OneNoteEventsUnmarshalFactory::Release() noexcept
{
    auto const new_refcount = --_reference_count;
    if (0 == new_refcount)
    {
        delete this;
    }
    return new_refcount;
}

HRESULT OneNoteEventsUnmarshalFactory::CreateInstance(IUnknown * pUnkOuter,
                                                      IID const & riid,
                                                      void ** ppvObject) noexcept
{
    if (nullptr != pUnkOuter)
    {
        return CLASS_E_NOAGGREGATION;
    }

    try
    {
        auto object = OneNoteEventsUnmarshal::create_instance();
        return object->QueryInterface(riid, ppvObject);
    }
    CATCH_RETURN()
}

HRESULT OneNoteEventsUnmarshalFactory::LockServer(BOOL /*fLock*/) noexcept
{
    return S_OK;
}

OneNoteEventsUnmarshalFactory::OneNoteEventsUnmarshalFactory()
{
    wil::com_ptr<IUnknown> ftm{};
    if (auto const result = CoCreateFreeThreadedMarshaler(this, &ftm); FAILED(result))
    {
        THROW_HR(result);
    }
    _ftm = ftm;
}
