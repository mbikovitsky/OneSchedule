#include <onemarshal.hpp>

#include <Windows.h>
#include <wrl.h>

#include <wil/result.h>

#include "onenote_events_proxy.hpp"


HRESULT onemarshal_create_marshalable_wrapper(OneNote::IOneNoteEvents * event_sink,
                                              OneNote::IOneNoteEvents ** wrapper_out_ptr) noexcept
try
{
    if (nullptr == wrapper_out_ptr)
    {
        return E_POINTER;
    }

    auto wrapper = Microsoft::WRL::Make<OneNoteEventsProxy>(event_sink);

    *wrapper_out_ptr = wrapper.Detach();
    return S_OK;
}
CATCH_RETURN()
