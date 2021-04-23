#pragma once

#include <onenote.hpp>


#ifdef ONENOTEEVENTSMARSHALER_EXPORTS
#define DLLEXPORT __declspec(dllexport)
#else
#define DLLEXPORT __declspec(dllimport)
#endif


extern "C" DLLEXPORT HRESULT
    onemarshal_create_marshalable_wrapper(OneNote::IOneNoteEvents * event_sink,
                                          OneNote::IOneNoteEvents ** wrapper) noexcept;
