/***************************************************************************
 * DSOFDOCOBJ.H
 *
 * Developer Support Office ActiveX Document Framer Control Sample
 *
 *  Copyright (c)1999-2001 Microsoft Corporation, All Rights Reserved
 *  Microsoft Product Support Services, Developer Support
 *
 *  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 *  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
 *  WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 *
 ***************************************************************************/
#ifndef DS_DSOFDOCOBJ_H 
#define DS_DSOFDOCOBJ_H

#include <docobj.h>
#include "rbbinder.h" 

class CDsoDocObject : public IUnknown
{
public:
    CDsoDocObject();
    ~CDsoDocObject();

 // IUnknown Implementation
    STDMETHODIMP QueryInterface(REFIID riid, void** ppv);
    STDMETHODIMP_(ULONG) AddRef(void);
    STDMETHODIMP_(ULONG) Release(void);

 // IOleClientSite Implementation
    BEGIN_INTERFACE_PART(OleClientSite, IOleClientSite)
        STDMETHODIMP SaveObject(void);
        STDMETHODIMP GetMoniker(DWORD dwAssign, DWORD dwWhichMoniker, IMoniker** ppmk);
        STDMETHODIMP GetContainer(IOleContainer** ppContainer);
        STDMETHODIMP ShowObject(void);
        STDMETHODIMP OnShowWindow(BOOL fShow);
        STDMETHODIMP RequestNewObjectLayout(void);
    END_INTERFACE_PART(OleClientSite)

 // IOleInPlaceSite Implementation
    BEGIN_INTERFACE_PART(OleInPlaceSite, IOleInPlaceSite)
        STDMETHODIMP GetWindow(HWND* phwnd);
        STDMETHODIMP ContextSensitiveHelp(BOOL fEnterMode);
        STDMETHODIMP CanInPlaceActivate(void);
        STDMETHODIMP OnInPlaceActivate(void);
        STDMETHODIMP OnUIActivate(void);
        STDMETHODIMP GetWindowContext(IOleInPlaceFrame** ppFrame, IOleInPlaceUIWindow** ppDoc, LPRECT lprcPosRect, LPRECT lprcClipRect, LPOLEINPLACEFRAMEINFO lpFrameInfo);
        STDMETHODIMP Scroll(SIZE sz);
        STDMETHODIMP OnUIDeactivate(BOOL fUndoable);
        STDMETHODIMP OnInPlaceDeactivate(void);
        STDMETHODIMP DiscardUndoState(void);
        STDMETHODIMP DeactivateAndUndo(void);
        STDMETHODIMP OnPosRectChange(LPCRECT lprcPosRect);
    END_INTERFACE_PART(OleInPlaceSite)

 // IOleDocumentSite Implementation
    BEGIN_INTERFACE_PART(OleDocumentSite, IOleDocumentSite)
        STDMETHODIMP ActivateMe(IOleDocumentView* pView);
    END_INTERFACE_PART(OleDocumentSite)

 // IOleInPlaceFrame Implementation
    BEGIN_INTERFACE_PART(OleInPlaceFrame, IOleInPlaceFrame)
        STDMETHODIMP GetWindow(HWND* phWnd);
        STDMETHODIMP ContextSensitiveHelp(BOOL fEnterMode);
        STDMETHODIMP GetBorder(LPRECT prcBorder);
        STDMETHODIMP RequestBorderSpace(LPCBORDERWIDTHS pBW);
        STDMETHODIMP SetBorderSpace(LPCBORDERWIDTHS pBW);
        STDMETHODIMP SetActiveObject(LPOLEINPLACEACTIVEOBJECT pIIPActiveObj, LPCOLESTR pszObj);
        STDMETHODIMP InsertMenus(HMENU hMenu, LPOLEMENUGROUPWIDTHS pMGW);
        STDMETHODIMP SetMenu(HMENU hMenu, HOLEMENU hOLEMenu, HWND hWndObj);
        STDMETHODIMP RemoveMenus(HMENU hMenu);
        STDMETHODIMP SetStatusText(LPCOLESTR pszText);
        STDMETHODIMP EnableModeless(BOOL fEnable);
        STDMETHODIMP TranslateAccelerator(LPMSG pMSG, WORD wID);
    END_INTERFACE_PART(OleInPlaceFrame)

 // IOleCommandTarget  Implementation
    BEGIN_INTERFACE_PART(OleCommandTarget , IOleCommandTarget)
        STDMETHODIMP QueryStatus(const GUID *pguidCmdGroup, ULONG cCmds, OLECMD prgCmds[], OLECMDTEXT *pCmdText);
        STDMETHODIMP Exec(const GUID *pguidCmdGroup, DWORD nCmdID, DWORD nCmdexecopt, VARIANTARG *pvaIn, VARIANTARG *pvaOut);            
    END_INTERFACE_PART(OleCommandTarget)

 // IAuthenticate  Implementation
    BEGIN_INTERFACE_PART(Authenticate , IAuthenticate)
        STDMETHODIMP Authenticate(HWND *phwnd, LPWSTR *pszUsername, LPWSTR *pszPassword);
    END_INTERFACE_PART(Authenticate)

    STDMETHODIMP        InitializeNewInstance(HWND hwndCtl, LPRECT prcPlace, IOleCommandTarget* pcmdCtl);

    STDMETHODIMP        CreateDocObject(REFCLSID rclsid);
    STDMETHODIMP        LoadStorageFromFile(LPWSTR pwszFile, REFCLSID rclsid, BOOL fKeepLock);
    STDMETHODIMP        LoadStorageFromURL(LPWSTR pwszURL, REFCLSID rclsid, LPWSTR pwszUserName, LPWSTR pwszPassword, BOOL fKeepLock);

    STDMETHODIMP        IPActivateView();
    STDMETHODIMP        IPDeactivateView();
    STDMETHODIMP        UIActivateView(BOOL fFocus);

    STDMETHODIMP        SaveDefault();
    STDMETHODIMP        SaveStorageToFile(LPWSTR pwszFile, BOOL fOverwriteFile);
    STDMETHODIMP        SaveStorageToURL(LPWSTR pwszURL, BOOL fOverwriteFile, LPWSTR pwszUserName, LPWSTR pwszPassword);

    STDMETHODIMP        DoOleCommand(DWORD dwOleCmdId, BOOL fPrompt);
    STDMETHODIMP_(void) Close();

 // Control should notify us on these conditions (so we can pass to IP object)...
    STDMETHODIMP_(void) OnNotifySizeChange(LPRECT prc);
    STDMETHODIMP_(void) OnNotifyAppActivate(BOOL fActive, DWORD dwThreadID);
    STDMETHODIMP_(void) OnNotifyPaletteChanged(HWND hwndPalChg);
    STDMETHODIMP_(void) OnNotifyChangeToolState(BOOL fShowTools);

 // Inline accessors for control to get IP object info...
    inline IOleInPlaceActiveObject*  GetActiveObject(){return m_pipactive;}
    inline BOOL         IsReadOnly(){return (m_pwszSourceFile == NULL);}
	inline HWND         GetDocWindow(){return m_hwnd;}
    inline HWND         GetActiveWindow(){return m_hwndUIActiveObj;}
    inline HWND         GetMenuHWND(){return m_hwndMenuObj;}
    inline HMENU        GetActiveMenu(){return m_hMenuActive;}
	inline HMENU        GetMergedMenu(){return m_hMenuMerged;}
	inline void         SetMergedMenu(HMENU h){m_hMenuMerged = h;}

protected:
 // Internal helper methods...
    STDMETHODIMP             CreateObjectStorage(REFCLSID rclsid);
    STDMETHODIMP             SaveObjectStorage();
    STDMETHODIMP             ValidateDocObjectServer(REFCLSID rclsid);
    STDMETHODIMP_(BOOL)      ValidateFileExtension(WCHAR* pwszFile, WCHAR** ppwszOut);

    STDMETHODIMP_(IUnknown*) CreateRosebudIPP();
    STDMETHODIMP             DownloadWebResource(LPWSTR pwszURL, LPWSTR pwszFile, LPWSTR pwszUsername, LPWSTR pwszPassword, IStream** ppstmKeepForSave);
    STDMETHODIMP             UploadWebResource(LPWSTR pwszFile, IStream** ppstmSave, LPWSTR pwszURLSaveTo, BOOL fOverwriteFile, LPWSTR pwszUsername, LPWSTR pwszPassword);

    STDMETHODIMP_(void)      TurnOffWebToolbar();
    STDMETHODIMP_(void)      ClearMergedMenu();

    STDMETHODIMP_(void)      OnDraw(DWORD dvAspect, HDC hdcDraw, LPRECT prcBounds, LPRECT prcWBounds, HDC hicTargetDev, BOOL fOptimize);

    static
    STDMETHODIMP_(LRESULT)  FrameWindowProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

private:
    ULONG                m_cRef;				// Reference count
    HWND                 m_hwnd;                // our window
    HWND                 m_hwndCtl;             // The control's window
    IOleCommandTarget   *m_pcmdCtl;             // IOCT of host (for frame msgs)
    RECT                 m_rcViewRect;          // Viewable area (set by host)

    LPWSTR               m_pwszSourceFile;      // Path to Source File (on Open)
    IStorage            *m_pstgSourceFile;      // Original File Storage (if open/save file)

    IStorage            *m_pstgroot;            // Root temp storage
    IStorage            *m_pstgfile;            // In-memory file storage
    IStream             *m_pstmview;            // In-memory view info stream

    LPWSTR               m_pwszWebResource;     // The full URL to the web resource
    IStream             *m_pstmWebResource;     // Original Download Stream (if open/save URL)
    IUnknown            *m_punkRosebud;         // MSDAIPP pointer (for URL downloads)
    LPWSTR               m_pwszUsername;        // Username and password used by MSDAIPP
    LPWSTR               m_pwszPassword;        // for Authentication (see IAuthenticate)

    CLSID                m_clsidObject;         // CLSID of the embedded object

    IOleObject              *m_pole;            // Embedded OLE Object (OLE)
    IOleInPlaceObject       *m_pipobj;          // The IP object methods (OLE)
    IOleInPlaceActiveObject *m_pipactive;       // The UI Active object methods (OLE)
    IOleDocumentView        *m_pdocv;           // MSO Document View (DocObj)
    IOleCommandTarget       *m_pcmdt;           // MSO Command Target (DocObj)

    HMENU                m_hMenuActive;         // The menu supplied by embedded object
    HMENU                m_hMenuMerged;         // The merged menu (set by control host)
    HOLEMENU             m_holeMenu;            // The OLE Menu Descriptor
    HWND                 m_hwndMenuObj;         // The window for menu commands
    HWND                 m_hwndIPObject;        // IP active object window
    HWND                 m_hwndUIActiveObj;     // UI Active object window
    DWORD                m_dwObjectThreadID;    // Thread Id of UI server
    BORDERWIDTHS         m_bwToolSpace;         // Toolspace...

 // Bitflags (state info)...
    unsigned int         m_fDisplayTools:1;
    unsigned int         m_fDisconnectOnQuit:1;
    unsigned int         m_fAppWindowActive:1;
    unsigned int         m_fOpenReadOnly:1;
    unsigned int         m_fObjectInModalCondition:1;
    unsigned int         m_fObjectIPActive:1;
    unsigned int         m_fObjectUIActive:1;
    unsigned int         m_fObjectActivateComplete:1;

};


#endif //DS_DSOFDOCOBJ_H