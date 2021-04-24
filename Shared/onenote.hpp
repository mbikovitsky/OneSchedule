#pragma once

#include <Unknwn.h>
#include <OAIdl.h>

namespace OneNote {

//
// Forward references and typedefs
//

struct __declspec(uuid("0ea692ee-bb50-4e3c-aef0-356d91732725"))
/* LIBID */ __OneNote;

//
// Type library items
//

enum __declspec(uuid("552e0e02-b287-4ec6-9cc0-4ba019ee5ea1"))
HierarchyScope
{
    hsSelf = 0,
    hsChildren = 1,
    hsNotebooks = 2,
    hsSections = 3,
    hsPages = 4
};

enum __declspec(uuid("41c8f6ea-0af0-4a4f-99e9-5eb01ebfc9a3"))
HierarchyElement
{
    heNone = 0,
    heNotebooks = 1,
    heSectionGroups = 2,
    heSections = 4,
    hePages = 8
};

enum __declspec(uuid("4db67b4f-cc7d-45b5-88fe-569ae5798ff2"))
RecentResultType
{
    rrtNone = 0,
    rrtFiling = 1,
    rrtSearch = 2,
    rrtLinks = 3
};

enum __declspec(uuid("b5eb9d34-5278-4d8a-ae1f-2f88ea56bbce"))
CreateFileType
{
    cftNone = 0,
    cftNotebook = 1,
    cftFolder = 2,
    cftSection = 3
};

enum __declspec(uuid("d6e78e55-7ee7-4a31-bf3e-b01e819599ba"))
PageInfo
{
    piBasic = 0,
    piBinaryData = 1,
    piSelection = 2,
    piFileType = 4,
    piBinaryDataSelection = 3,
    piBinaryDataFileType = 5,
    piSelectionFileType = 6,
    piAll = 7
};

enum __declspec(uuid("d6166973-3665-4edb-94b0-77c65c34b51c"))
PublishFormat
{
    pfOneNote = 0,
    pfOneNotePackage = 1,
    pfMHTML = 2,
    pfPDF = 3,
    pfXPS = 4,
    pfWord = 5,
    pfEMF = 6,
    pfHTML = 7,
    pfOneNote2007 = 8
};

enum __declspec(uuid("e195f3e3-8ec3-4a67-81fe-ddbec2b42d3f"))
SpecialLocation
{
    slBackUpFolder = 0,
    slUnfiledNotesSection = 1,
    slDefaultNotebookFolder = 2
};

enum __declspec(uuid("452d048e-7f61-4258-94b9-a39e19c290da"))
FilingLocation
{
    flEMail = 0,
    flContacts = 1,
    flTasks = 2,
    flMeetings = 3,
    flWebContent = 4,
    flPrintOuts = 5
};

enum __declspec(uuid("82fc5a95-feb7-4242-95e1-369c5dfe3f49"))
FilingLocationType
{
    fltNamedSectionNewPage = 0,
    fltCurrentSectionNewPage = 1,
    fltCurrentPage = 2,
    fltNamedPage = 4
};

enum __declspec(uuid("226cc8e6-1ed0-4770-a7f1-a80bb4ddf07b"))
NewPageStyle
{
    npsDefault = 0,
    npsBlankPageWithTitle = 1,
    npsBlankPageNoTitle = 2
};

enum __declspec(uuid("b67bc7e1-91b9-4f50-8471-77c76f8d63d6"))
DockLocation
{
    dlDefault = -1,
    dlNone = 0,
    dlLeft = 1,
    dlRight = 2,
    dlTop = 3,
    dlBottom = 4
};

enum __declspec(uuid("68555133-b62f-4490-9d66-9e9bfc68f6c6"))
XMLSchema
{
    xs2007 = 0,
    xs2010 = 1,
    xs2013 = 2,
    xsCurrent = 2
};

enum __declspec(uuid("1ecc88b3-6d2b-4edd-8dd5-bb11e5d34c09"))
TreeCollapsedStateType
{
    tcsExpanded = 0,
    tcsCollapsed = 1
};

enum __declspec(uuid("13f18b04-e76f-42e0-97e6-8b6abf38b484"))
NotebookFilterOutType
{
    nfoLocal = 1,
    nfoNetwork = 2,
    nfoWeb = 4,
    nfoNoWacUrl = 8
};

struct __declspec(uuid("e2e1511d-502d-4bd0-8b3a-8a89a05cdcae"))
IOneNoteEvents : IDispatch
{};

struct __declspec(uuid("d7fac39e-7ff1-49aa-98cf-a1ddd316337e"))
Application;
    // [ default ] interface IApplication
    // [ default, source ] dispinterface IOneNoteEvents

enum __declspec(uuid("d3f5a756-4bac-4d3d-9baf-90935121aaa6"))
Error
{
    hrMalformedXML = -2147213312,
    hrInvalidXML = -2147213311,
    hrCreatingSection = -2147213310,
    hrOpeningSection = -2147213309,
    hrSectionDoesNotExist = -2147213308,
    hrPageDoesNotExist = -2147213307,
    hrFileDoesNotExist = -2147213306,
    hrInsertingImage = -2147213305,
    hrInsertingInk = -2147213304,
    hrInsertingHtml = -2147213303,
    hrNavigatingToPage = -2147213302,
    hrSectionReadOnly = -2147213301,
    hrPageReadOnly = -2147213300,
    hrInsertingOutlineText = -2147213299,
    hrPageObjectDoesNotExist = -2147213298,
    hrBinaryObjectDoesNotExist = -2147213297,
    hrLastModifiedDateDidNotMatch = -2147213296,
    hrGroupDoesNotExist = -2147213295,
    hrPageDoesNotExistInGroup = -2147213294,
    hrNoActiveSelection = -2147213293,
    hrObjectDoesNotExist = -2147213292,
    hrNotebookDoesNotExist = -2147213291,
    hrInsertingFile = -2147213290,
    hrInvalidName = -2147213289,
    hrFolderDoesNotExist = -2147213288,
    hrInvalidQuery = -2147213287,
    hrFileAlreadyExists = -2147213286,
    hrSectionEncryptedAndLocked = -2147213285,
    hrDisabledByPolicy = -2147213284,
    hrNotYetSynchronized = -2147213283,
    hrLegacySection = -2147213282,
    hrMergeFailed = -2147213281,
    hrInvalidXMLSchema = -2147213280,
    hrFutureContentLoss = -2147213278,
    hrTimeOut = -2147213277,
    hrRecordingInProgress = -2147213276,
    hrUnknownLinkedNoteState = -2147213275,
    hrNoShortNameForLinkedNote = -2147213274,
    hrNoFriendlyNameForLinkedNote = -2147213273,
    hrInvalidLinkedNoteUri = -2147213272,
    hrInvalidLinkedNoteThumbnail = -2147213271,
    hrImportLNTThumbnailFailed = -2147213270,
    hrUnreadDisabledForNotebook = -2147213269,
    hrInvalidSelection = -2147213268,
    hrConvertFailed = -2147213267,
    hrRecycleBinEditFailed = -2147213266,
    hrAppInModalUI = -2147213264
};

struct __declspec(uuid("dc67e480-c3cb-49f8-8232-60b0c2056c8e"))
Application2;
    // [ default ] interface IApplication
    // [ default, source ] dispinterface IOneNoteEvents

struct __declspec(uuid("1d12bd3f-89b6-4077-aa2c-c9dc2bca42f9"))
IQuickFilingDialog : IDispatch
{
    //
    // Raw methods provided by interface
    //

      virtual HRESULT __stdcall get_Title (
        /*[out,retval]*/ BSTR * bstrTitle ) = 0;
      virtual HRESULT __stdcall put_Title (
        /*[in]*/ BSTR bstrTitle ) = 0;
      virtual HRESULT __stdcall get_Description (
        /*[out,retval]*/ BSTR * bstrDescription ) = 0;
      virtual HRESULT __stdcall put_Description (
        /*[in]*/ BSTR bstrDescription ) = 0;
      virtual HRESULT __stdcall get_CheckboxText (
        /*[out,retval]*/ BSTR * bstrText ) = 0;
      virtual HRESULT __stdcall put_CheckboxText (
        /*[in]*/ BSTR bstrText ) = 0;
      virtual HRESULT __stdcall get_CheckboxState (
        /*[out,retval]*/ VARIANT_BOOL * pfChecked ) = 0;
      virtual HRESULT __stdcall put_CheckboxState (
        /*[in]*/ VARIANT_BOOL pfChecked ) = 0;
      virtual HRESULT __stdcall get_WindowHandle (
        /*[out,retval]*/ unsigned __int64 * pHWNDWindow ) = 0;
      virtual HRESULT __stdcall get_TreeDepth (
        /*[out,retval]*/ enum HierarchyElement * pTreeDepth ) = 0;
      virtual HRESULT __stdcall put_TreeDepth (
        /*[in]*/ enum HierarchyElement pTreeDepth ) = 0;
      virtual HRESULT __stdcall get_ParentWindowHandle (
        /*[out,retval]*/ unsigned __int64 * pHWNDParentWindow ) = 0;
      virtual HRESULT __stdcall put_ParentWindowHandle (
        /*[in]*/ unsigned __int64 pHWNDParentWindow ) = 0;
      virtual HRESULT __stdcall get_Position (
        /*[out,retval]*/ struct tagPOINT * pPoint ) = 0;
      virtual HRESULT __stdcall put_Position (
        /*[in]*/ struct tagPOINT pPoint ) = 0;
      virtual HRESULT __stdcall SetRecentResults (
        /*[in]*/ enum RecentResultType recentResults,
        /*[in]*/ VARIANT_BOOL fShowCurrentSection,
        /*[in]*/ VARIANT_BOOL fShowCurrentPage,
        /*[in]*/ VARIANT_BOOL fShowUnfiledNotes ) = 0;
      virtual HRESULT __stdcall AddButton (
        /*[in]*/ BSTR bstrText,
        /*[in]*/ enum HierarchyElement allowedElements,
        /*[in]*/ enum HierarchyElement allowedReadOnlyElements,
        /*[in]*/ VARIANT_BOOL fDefault ) = 0;
      virtual HRESULT __stdcall Run (
        /*[in]*/ struct IQuickFilingDialogCallback * piCallback ) = 0;
      virtual HRESULT __stdcall get_SelectedItem (
        /*[out,retval]*/ BSTR * pbstrSelectedNodeID ) = 0;
      virtual HRESULT __stdcall get_PressedButton (
        /*[out,retval]*/ unsigned long * pButtonIndex ) = 0;
      virtual HRESULT __stdcall put_TreeCollapsedState (
        /*[in]*/ enum TreeCollapsedStateType _arg1 ) = 0;
      virtual HRESULT __stdcall put_NotebookFilterOut (
        /*[in]*/ enum NotebookFilterOutType _arg1 ) = 0;
      virtual HRESULT __stdcall ShowCreateNewNotebook ( ) = 0;
      virtual HRESULT __stdcall AddInitialEditor (
        BSTR initialEditor ) = 0;
      virtual HRESULT __stdcall ClearInitialEditors ( ) = 0;
      virtual HRESULT __stdcall ShowSharingHyperlink ( ) = 0;
};

struct __declspec(uuid("627ea7b4-95b5-4980-84c1-9d20da4460b1"))
IQuickFilingDialogCallback : IDispatch
{
    //
    // Raw methods provided by interface
    //

      virtual HRESULT __stdcall OnDialogClosed (
        /*[in]*/ struct IQuickFilingDialog * dialog ) = 0;
};

struct __declspec(uuid("452ac71a-b655-4967-a208-a4cc39dd7949"))
IApplication : IDispatch
{
    //
    // Raw methods provided by interface
    //

      virtual HRESULT __stdcall GetHierarchy (
        /*[in]*/ BSTR bstrStartNodeID,
        /*[in]*/ enum HierarchyScope hsScope,
        /*[out]*/ BSTR * pbstrHierarchyXmlOut,
        /*[in]*/ enum XMLSchema xsSchema ) = 0;
      virtual HRESULT __stdcall UpdateHierarchy (
        /*[in]*/ BSTR bstrChangesXmlIn,
        /*[in]*/ enum XMLSchema xsSchema ) = 0;
      virtual HRESULT __stdcall OpenHierarchy (
        /*[in]*/ BSTR bstrPath,
        /*[in]*/ BSTR bstrRelativeToObjectID,
        /*[out]*/ BSTR * pbstrObjectID,
        /*[in]*/ enum CreateFileType cftIfNotExist ) = 0;
      virtual HRESULT __stdcall DeleteHierarchy (
        /*[in]*/ BSTR bstrObjectID,
        /*[in]*/ DATE dateExpectedLastModified,
        /*[in]*/ VARIANT_BOOL deletePermanently ) = 0;
      virtual HRESULT __stdcall CreateNewPage (
        /*[in]*/ BSTR bstrSectionID,
        /*[out]*/ BSTR * pbstrPageID,
        /*[in]*/ enum NewPageStyle npsNewPageStyle ) = 0;
      virtual HRESULT __stdcall CloseNotebook (
        /*[in]*/ BSTR bstrNotebookID,
        /*[in]*/ VARIANT_BOOL force ) = 0;
      virtual HRESULT __stdcall GetHierarchyParent (
        /*[in]*/ BSTR bstrObjectID,
        /*[out]*/ BSTR * pbstrParentID ) = 0;
      virtual HRESULT __stdcall GetPageContent (
        /*[in]*/ BSTR bstrPageID,
        /*[out]*/ BSTR * pbstrPageXmlOut,
        /*[in]*/ enum PageInfo pageInfoToExport,
        /*[in]*/ enum XMLSchema xsSchema ) = 0;
      virtual HRESULT __stdcall UpdatePageContent (
        /*[in]*/ BSTR bstrPageChangesXmlIn,
        /*[in]*/ DATE dateExpectedLastModified,
        /*[in]*/ enum XMLSchema xsSchema,
        /*[in]*/ VARIANT_BOOL force ) = 0;
      virtual HRESULT __stdcall GetBinaryPageContent (
        /*[in]*/ BSTR bstrPageID,
        /*[in]*/ BSTR bstrCallbackID,
        /*[out]*/ BSTR * pbstrBinaryObjectB64Out ) = 0;
      virtual HRESULT __stdcall DeletePageContent (
        /*[in]*/ BSTR bstrPageID,
        /*[in]*/ BSTR bstrObjectID,
        /*[in]*/ DATE dateExpectedLastModified,
        /*[in]*/ VARIANT_BOOL force ) = 0;
      virtual HRESULT __stdcall NavigateTo (
        /*[in]*/ BSTR bstrHierarchyObjectID,
        /*[in]*/ BSTR bstrObjectID,
        /*[in]*/ VARIANT_BOOL fNewWindow ) = 0;
      virtual HRESULT __stdcall NavigateToUrl (
        /*[in]*/ BSTR bstrUrl,
        /*[in]*/ VARIANT_BOOL fNewWindow ) = 0;
      virtual HRESULT __stdcall Publish (
        /*[in]*/ BSTR bstrHierarchyID,
        /*[in]*/ BSTR bstrTargetFilePath,
        /*[in]*/ enum PublishFormat pfPublishFormat,
        /*[in]*/ BSTR bstrCLSIDofExporter ) = 0;
      virtual HRESULT __stdcall OpenPackage (
        /*[in]*/ BSTR bstrPathPackage,
        /*[in]*/ BSTR bstrPathDest,
        /*[out]*/ BSTR * pbstrPathOut ) = 0;
      virtual HRESULT __stdcall GetHyperlinkToObject (
        /*[in]*/ BSTR bstrHierarchyID,
        /*[in]*/ BSTR bstrPageContentObjectID,
        /*[out]*/ BSTR * pbstrHyperlinkOut ) = 0;
      virtual HRESULT __stdcall FindPages (
        /*[in]*/ BSTR bstrStartNodeID,
        /*[in]*/ BSTR bstrSearchString,
        /*[out]*/ BSTR * pbstrHierarchyXmlOut,
        /*[in]*/ VARIANT_BOOL fIncludeUnindexedPages,
        /*[in]*/ VARIANT_BOOL fDisplay,
        /*[in]*/ enum XMLSchema xsSchema ) = 0;
      virtual HRESULT __stdcall FindMeta (
        /*[in]*/ BSTR bstrStartNodeID,
        /*[in]*/ BSTR bstrSearchStringName,
        /*[out]*/ BSTR * pbstrHierarchyXmlOut,
        /*[in]*/ VARIANT_BOOL fIncludeUnindexedPages,
        /*[in]*/ enum XMLSchema xsSchema ) = 0;
      virtual HRESULT __stdcall GetSpecialLocation (
        /*[in]*/ enum SpecialLocation slToGet,
        /*[out]*/ BSTR * pbstrSpecialLocationPath ) = 0;
      virtual HRESULT __stdcall MergeFiles (
        /*[in]*/ BSTR bstrBaseFile,
        /*[in]*/ BSTR bstrClientFile,
        /*[in]*/ BSTR bstrServerFile,
        /*[in]*/ BSTR bstrTargetFile ) = 0;
      virtual HRESULT __stdcall QuickFiling (
        /*[out,retval]*/ struct IQuickFilingDialog * * ppiDialog ) = 0;
      virtual HRESULT __stdcall SyncHierarchy (
        /*[in]*/ BSTR bstrHierarchyID ) = 0;
      virtual HRESULT __stdcall SetFilingLocation (
        /*[in]*/ enum FilingLocation flToSet,
        /*[in]*/ enum FilingLocationType fltToSet,
        /*[in]*/ BSTR bstrFilingSectionID ) = 0;
      virtual HRESULT __stdcall get_Windows (
        /*[out,retval]*/ struct Windows * * ppONWindows ) = 0;
      virtual HRESULT __stdcall get_Dummy1 (
        /*[out,retval]*/ VARIANT_BOOL * pBool ) = 0;
      virtual HRESULT __stdcall MergeSections (
        /*[in]*/ BSTR bstrSectionSourceId,
        /*[in]*/ BSTR bstrSectionDestinationId ) = 0;
      virtual HRESULT __stdcall get_COMAddIns (
        /*[out,retval]*/ IDispatch * * ppiComAddins ) = 0;
      virtual HRESULT __stdcall get_LanguageSettings (
        /*[out,retval]*/ IDispatch * * ppiLanguageSettings ) = 0;
      virtual HRESULT __stdcall GetWebHyperlinkToObject (
        /*[in]*/ BSTR bstrHierarchyID,
        /*[in]*/ BSTR bstrPageContentObjectID,
        /*[out]*/ BSTR * pbstrHyperlinkOut ) = 0;
};

struct __declspec(uuid("8e8304b8-cbd1-44f8-b0e8-89c625b2002e"))
Window : IDispatch
{
    //
    // Raw methods provided by interface
    //

      virtual HRESULT __stdcall get_WindowHandle (
        /*[out,retval]*/ unsigned __int64 * pHWNDWindow ) = 0;
      virtual HRESULT __stdcall get_CurrentPageId (
        /*[out,retval]*/ BSTR * pbstrPageObjectId ) = 0;
      virtual HRESULT __stdcall get_CurrentSectionId (
        /*[out,retval]*/ BSTR * pbstrSectionObjectId ) = 0;
      virtual HRESULT __stdcall get_CurrentSectionGroupId (
        /*[out,retval]*/ BSTR * pbstrSectionObjectId ) = 0;
      virtual HRESULT __stdcall get_CurrentNotebookId (
        /*[out,retval]*/ BSTR * pbstrNotebookObjectId ) = 0;
      virtual HRESULT __stdcall NavigateTo (
        /*[in]*/ BSTR bstrHierarchyObjectID,
        /*[in]*/ BSTR bstrObjectID ) = 0;
      virtual HRESULT __stdcall get_FullPageView (
        /*[out,retval]*/ VARIANT_BOOL * pIsFullPageView ) = 0;
      virtual HRESULT __stdcall put_FullPageView (
        VARIANT_BOOL pIsFullPageView ) = 0;
      virtual HRESULT __stdcall get_Active (
        /*[out,retval]*/ VARIANT_BOOL * pIsActive ) = 0;
      virtual HRESULT __stdcall put_Active (
        VARIANT_BOOL pIsActive ) = 0;
      virtual HRESULT __stdcall get_DockedLocation (
        /*[out,retval]*/ enum DockLocation * pDockLocation ) = 0;
      virtual HRESULT __stdcall put_DockedLocation (
        enum DockLocation pDockLocation ) = 0;
      virtual HRESULT __stdcall get_Application (
        /*[out,retval]*/ struct IApplication * * ppiApp ) = 0;
      virtual HRESULT __stdcall get_SideNote (
        /*[out,retval]*/ VARIANT_BOOL * pIsSideNote ) = 0;
      virtual HRESULT __stdcall NavigateToUrl (
        /*[in]*/ BSTR bstrUrl ) = 0;
      virtual HRESULT __stdcall SetDockedLocation (
        /*[in]*/ enum DockLocation DockLocation,
        /*[in]*/ struct tagPOINT ptMonitor ) = 0;
};

struct __declspec(uuid("6d4b9c3e-cc05-493f-85e2-43d1006df96a"))
Windows : IDispatch
{
    //
    // Raw methods provided by interface
    //

      virtual HRESULT __stdcall get_Item (
        /*[in]*/ unsigned long Index,
        /*[out,retval]*/ struct Window * * Item ) = 0;
      virtual HRESULT __stdcall get_Count (
        /*[out,retval]*/ unsigned long * Count ) = 0;
      virtual HRESULT __stdcall get__NewEnum (
        /*[out,retval]*/ IUnknown * * _NewEnum ) = 0;
      virtual HRESULT __stdcall get_CurrentWindow (
        /*[out,retval]*/ struct Window * * ppCurrentWindow ) = 0;
};

} // namespace OneNote
