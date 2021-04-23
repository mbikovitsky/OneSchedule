#pragma once

#include <Windows.h>
#include <wrl.h>

#include <wil/com.h>


#define ONENOTE_EVENTS_UNMARSHAL_CLSID "28F3F297-33A1-4881-888B-D0784B2D258A"


class __declspec(uuid(ONENOTE_EVENTS_UNMARSHAL_CLSID)) OneNoteEventsUnmarshal final
    : public Microsoft::WRL::RuntimeClass<
        Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::ClassicCom |
                                          Microsoft::WRL::InhibitFtmBase>,
        IMarshal>
{
public:
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
};

CoCreatableClass(OneNoteEventsUnmarshal);
