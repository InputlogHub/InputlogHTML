/***************************************************************************
 * DSOFRAMER.H
 *
 * Developer Support Office ActiveX Document Framer Control Sample
 *
 *  Copyright (c)1999-2001 Microsoft Corporation, All Rights Reserved
 *  Microsoft Product Support Services, Developer Support
 *
 *  You have a royalty-free right to use, modify, reproduce and distribute
 *  this sample application, and/or any modified version, in any way you
 *  find useful, provided that you agree that Microsoft has no warranty,
 *  obligations or liability for the code or information provided herein.
 *
 *  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 *  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
 *  WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 *
 ***************************************************************************/
#ifndef DS_DSOFRAMER_H 
#define DS_DSOFRAMER_H

////////////////////////////////////////////////////////////////////
// Standard include files (turn off some warnings)
//
#pragma warning(disable: 4100) // unreferenced formal parameter (in OLE this is common)
#pragma warning(disable: 4103) // pragma pack
#pragma warning(disable: 4127) // constant expression
#pragma warning(disable: 4146) // unary minus operator applied to unsigned type, result still unsigned
#pragma warning(disable: 4201) // nameless unions are part of C++
#pragma warning(disable: 4310) // cast truncates constant value
#pragma warning(disable: 4505) // unreferenced local function has been removed
#pragma warning(disable: 4710) // function couldn't be inlined
#pragma warning(disable: 4786) // identifier was truncated in the debug information
#pragma warning(disable: 4706) // assignment within conditional expression

#include <windows.h>
#include <ole2.h>
#include <olectl.h>
#include <oleidl.h>
#include <objsafe.h>

#include "version.h"
#include "utilities.h"
#include "dsofdocobj.h"
#include ".\lib\dsoframerlib.h"
#include ".\res\resource.h"

extern HINSTANCE        v_hModule;
extern CRITICAL_SECTION v_csecThreadSynch;
extern HICON            v_icoOffDocIcon;
extern HBITMAP          v_bmpFakeToolbar;
extern ULONG            v_cLocks;


////////////////////////////////////////////////////////////////////
// Custom Errors - we support a very limited set of custom error messages
//
#define DSO_E_ERR_BASE              0x80041100
#define DSO_E_INVALIDPROGID         0x80041102   // "The ProgID is incorrect, or the associated server is not installed."
#define DSO_E_INVALIDSERVER         0x80041103   // "The associated program is not an ActiveX Document server."
#define DSO_E_COMMANDNOTSUPPORTED   0x80041104   // "The command is not supported by the document server."
#define DSO_E_DOCUMENTREADONLY      0x80041105   // "Unable to perform action because document was opened in read-only mode."
#define DSO_E_REQUIRESMSDAIPP       0x80041106   // "Opening/saving a document from a URL requires the Microsoft Internet Publishing Provider installed with MDAC 2.5 or Office 2000/XP."


////////////////////////////////////////////////////////////////////
// Control Class Factory
//

class CDsoFramerClassFactory : public IClassFactory
{
public:
    CDsoFramerClassFactory(): m_cRef(0){}
    ~CDsoFramerClassFactory(void){}

 // IUnknown Implementation
    STDMETHODIMP         QueryInterface(REFIID riid, void ** ppv);
    STDMETHODIMP_(ULONG) AddRef(void);
    STDMETHODIMP_(ULONG) Release(void);

 // IClassFactory Implementation
    STDMETHODIMP  CreateInstance(LPUNKNOWN punk, REFIID riid, void** ppv);
    STDMETHODIMP  LockServer(BOOL fLock);

private:
    ULONG          m_cRef;          // Reference count

};


////////////////////////////////////////////////////////////////////
// Control Object (OCX)
//

class CDsoFramerControl : public _FramerControl
{
public:
    CDsoFramerControl(LPUNKNOWN punk);
    ~CDsoFramerControl(void);

 // IUnknown Implementation -- Always delgates to outer unknown...
    STDMETHODIMP         QueryInterface(REFIID riid, void ** ppv){return m_pOuterUnknown->QueryInterface(riid, ppv);}
    STDMETHODIMP_(ULONG) AddRef(void){return m_pOuterUnknown->AddRef();}
    STDMETHODIMP_(ULONG) Release(void){return m_pOuterUnknown->Release();}

 // IDispatch Implementation
    STDMETHODIMP GetTypeInfoCount(UINT* pctinfo); 
    STDMETHODIMP GetTypeInfo(UINT iTInfo, LCID lcid, ITypeInfo** ppTInfo);
    STDMETHODIMP GetIDsOfNames(REFIID riid, LPOLESTR* rgszNames, UINT cNames, LCID lcid, DISPID* rgDispId);
    STDMETHODIMP Invoke(DISPID dispIdMember, REFIID riid, LCID lcid, WORD wFlags, DISPPARAMS* pDispParams, VARIANT* pVarResult, EXCEPINFO* pExcepInfo, UINT* puArgErr);

 // _FramerControl Implementation
    STDMETHODIMP Activate();
    STDMETHODIMP get_ActiveDocument(IDispatch** ppdisp);
    STDMETHODIMP CreateNew(BSTR ProgId);
    STDMETHODIMP Open(VARIANT Document, VARIANT ReadOnly, VARIANT ProgId, VARIANT WebUsername, VARIANT WebPassword);
    STDMETHODIMP Save(VARIANT SaveAsDocument, VARIANT OverwriteExisting, VARIANT WebUsername, VARIANT WebPassword);
    STDMETHODIMP PrintOut(VARIANT PromptToSelectPrinter);
    STDMETHODIMP Close();
    STDMETHODIMP put_Caption(BSTR bstr);
    STDMETHODIMP get_Caption(BSTR* pbstr);
    STDMETHODIMP put_Titlebar(VARIANT_BOOL vbool);
    STDMETHODIMP get_Titlebar(VARIANT_BOOL* pbool);
    STDMETHODIMP put_Toolbars(VARIANT_BOOL vbool);
    STDMETHODIMP get_Toolbars(VARIANT_BOOL* pbool);
    STDMETHODIMP put_ModalState(VARIANT_BOOL vbool);
    STDMETHODIMP get_ModalState(VARIANT_BOOL* pbool);
    STDMETHODIMP ShowDialog(dsoShowDialogType DlgType);
    STDMETHODIMP put_EnableFileCommand(dsoFileCommandType Item, VARIANT_BOOL vbool);
    STDMETHODIMP get_EnableFileCommand(dsoFileCommandType Item, VARIANT_BOOL* pbool);
    STDMETHODIMP put_BorderStyle(dsoBorderStyle style);
    STDMETHODIMP get_BorderStyle(dsoBorderStyle* pstyle);
    STDMETHODIMP put_BorderColor(OLE_COLOR clr);
    STDMETHODIMP get_BorderColor(OLE_COLOR* pclr);
    STDMETHODIMP put_BackColor(OLE_COLOR clr);
    STDMETHODIMP get_BackColor(OLE_COLOR* pclr);
    STDMETHODIMP put_ForeColor(OLE_COLOR clr);
    STDMETHODIMP get_ForeColor(OLE_COLOR* pclr);
    STDMETHODIMP put_TitlebarColor(OLE_COLOR clr);
    STDMETHODIMP get_TitlebarColor(OLE_COLOR* pclr);
    STDMETHODIMP put_TitlebarTextColor(OLE_COLOR clr);
    STDMETHODIMP get_TitlebarTextColor(OLE_COLOR* pclr);


 // IInternalUnknown Implementation
    BEGIN_INTERFACE_PART(InternalUnknown, IUnknown)
    END_INTERFACE_PART(InternalUnknown)

 // IPersistStreamInit Implementation
    BEGIN_INTERFACE_PART(PersistStreamInit, IPersistStreamInit)
        STDMETHODIMP GetClassID(CLSID *pClassID);
        STDMETHODIMP IsDirty(void);
        STDMETHODIMP Load(LPSTREAM pStm);
        STDMETHODIMP Save(LPSTREAM pStm, BOOL fClearDirty);
        STDMETHODIMP GetSizeMax(ULARGE_INTEGER* pcbSize);
        STDMETHODIMP InitNew(void);
    END_INTERFACE_PART(PersistStreamInit)

 // IPersistPropertyBag Implementation
    BEGIN_INTERFACE_PART(PersistPropertyBag, IPersistPropertyBag)
        STDMETHODIMP GetClassID(CLSID *pClassID);
        STDMETHODIMP InitNew(void);
        STDMETHODIMP Load(IPropertyBag* pPropBag, IErrorLog* pErrorLog);
        STDMETHODIMP Save(IPropertyBag* pPropBag, BOOL fClearDirty, BOOL fSaveAllProperties);
    END_INTERFACE_PART(PersistPropertyBag)

 // IOleObject Implementation
    BEGIN_INTERFACE_PART(OleObject, IOleObject)
        STDMETHODIMP SetClientSite(IOleClientSite *pClientSite);
        STDMETHODIMP GetClientSite(IOleClientSite **ppClientSite);
        STDMETHODIMP SetHostNames(LPCOLESTR szContainerApp, LPCOLESTR szContainerObj);
        STDMETHODIMP Close(DWORD dwSaveOption);
        STDMETHODIMP SetMoniker(DWORD dwWhichMoniker, IMoniker *pmk);
        STDMETHODIMP GetMoniker(DWORD dwAssign, DWORD dwWhichMoniker, IMoniker **ppmk);
        STDMETHODIMP InitFromData(IDataObject *pDataObject, BOOL fCreation, DWORD dwReserved);
        STDMETHODIMP GetClipboardData(DWORD dwReserved, IDataObject **ppDataObject);
        STDMETHODIMP DoVerb(LONG iVerb, LPMSG lpmsg, IOleClientSite *pActiveSite, LONG lindex, HWND hwndParent, LPCRECT lprcPosRect);
        STDMETHODIMP EnumVerbs(IEnumOLEVERB **ppEnumOleVerb);
        STDMETHODIMP Update();
        STDMETHODIMP IsUpToDate();
        STDMETHODIMP GetUserClassID(CLSID *pClsid);
        STDMETHODIMP GetUserType(DWORD dwFormOfType, LPOLESTR *pszUserType);
        STDMETHODIMP SetExtent(DWORD dwDrawAspect, SIZEL *psizel);
        STDMETHODIMP GetExtent(DWORD dwDrawAspect, SIZEL *psizel);
        STDMETHODIMP Advise(IAdviseSink *pAdvSink, DWORD *pdwConnection);
        STDMETHODIMP Unadvise(DWORD dwConnection);
        STDMETHODIMP EnumAdvise(IEnumSTATDATA **ppenumAdvise);
        STDMETHODIMP GetMiscStatus(DWORD dwAspect, DWORD *pdwStatus);
        STDMETHODIMP SetColorScheme(LOGPALETTE *pLogpal);
    END_INTERFACE_PART(OleObject)
 
 // IOleControl Implementation
    BEGIN_INTERFACE_PART(OleControl, IOleControl)
        STDMETHODIMP GetControlInfo(CONTROLINFO* pCI);
        STDMETHODIMP OnMnemonic(LPMSG pMsg);
        STDMETHODIMP OnAmbientPropertyChange(DISPID dispID);
        STDMETHODIMP FreezeEvents(BOOL bFreeze);
    END_INTERFACE_PART(OleControl)

 // IOleInplaceObject Implementation 
    BEGIN_INTERFACE_PART(OleInplaceObject, IOleInPlaceObject)
        STDMETHODIMP GetWindow(HWND *phwnd);
        STDMETHODIMP ContextSensitiveHelp(BOOL fEnterMode);
        STDMETHODIMP InPlaceDeactivate();
        STDMETHODIMP UIDeactivate();
        STDMETHODIMP SetObjectRects(LPCRECT lprcPosRect, LPCRECT lprcClipRect);
        STDMETHODIMP ReactivateAndUndo();
    END_INTERFACE_PART(OleInplaceObject)

 // IOleInplaceActiveObject Implementation 
    BEGIN_INTERFACE_PART(OleInplaceActiveObject, IOleInPlaceActiveObject)
        STDMETHODIMP GetWindow(HWND *phwnd);
        STDMETHODIMP ContextSensitiveHelp(BOOL fEnterMode);
        STDMETHODIMP TranslateAccelerator(LPMSG lpmsg);
        STDMETHODIMP OnFrameWindowActivate(BOOL fActivate);
        STDMETHODIMP OnDocWindowActivate(BOOL fActivate);
        STDMETHODIMP ResizeBorder(LPCRECT prcBorder, IOleInPlaceUIWindow *pUIWindow, BOOL fFrameWindow);
        STDMETHODIMP EnableModeless(BOOL fEnable);
    END_INTERFACE_PART(OleInplaceActiveObject)
 
 // IViewObjectEx Implementation 
    BEGIN_INTERFACE_PART(ViewObjectEx, IViewObjectEx)
        STDMETHODIMP Draw(DWORD dwDrawAspect, LONG lIndex, void *pvAspect, DVTARGETDEVICE *ptd, HDC hicTargetDevice, HDC hdcDraw, LPCRECTL prcBounds, LPCRECTL prcWBounds, BOOL (__stdcall *pfnContinue)(DWORD dwContinue), DWORD dwContinue);
        STDMETHODIMP GetColorSet(DWORD dwAspect, LONG lindex, void* pvAspect, DVTARGETDEVICE *ptd, HDC hicTargetDev, LOGPALETTE** ppColorSet);
        STDMETHODIMP Freeze(DWORD dwAspect, LONG lindex, void* pvAspect, DWORD* pdwFreeze);
        STDMETHODIMP Unfreeze(DWORD dwFreeze);
        STDMETHODIMP SetAdvise(DWORD dwAspect, DWORD advf, IAdviseSink* pAdviseSink);
        STDMETHODIMP GetAdvise(DWORD* pdwAspect, DWORD* padvf, IAdviseSink** ppAdviseSink);
        STDMETHODIMP GetExtent(DWORD  dwDrawAspect, LONG lindex, DVTARGETDEVICE *ptd, LPSIZEL psizel);
        STDMETHODIMP GetRect(DWORD dwAspect, LPRECTL pRect);
        STDMETHODIMP GetViewStatus(DWORD* pdwStatus);
        STDMETHODIMP QueryHitPoint(DWORD dwAspect, LPCRECT pRectBounds, POINT ptlLoc, LONG lCloseHint, DWORD *pHitResult);
        STDMETHODIMP QueryHitRect(DWORD dwAspect, LPCRECT pRectBounds, LPCRECT pRectLoc, LONG lCloseHint, DWORD *pHitResult);
        STDMETHODIMP GetNaturalExtent(DWORD dwAspect, LONG lindex, DVTARGETDEVICE *ptd, HDC hicTargetDev, DVEXTENTINFO *pExtentInfo, LPSIZEL pSizel);
    END_INTERFACE_PART(ViewObjectEx)

 // IProvideClassInfo Implementation
    BEGIN_INTERFACE_PART(ProvideClassInfo, IProvideClassInfo)
        STDMETHODIMP GetClassInfo(ITypeInfo** ppTI);
    END_INTERFACE_PART(ProvideClassInfo)

 // IConnectionPointContainer Implementation
    BEGIN_INTERFACE_PART(ConnectionPointContainer, IConnectionPointContainer)
        STDMETHODIMP EnumConnectionPoints(IEnumConnectionPoints **ppEnum);
        STDMETHODIMP FindConnectionPoint(REFIID riid, IConnectionPoint **ppCP);
    END_INTERFACE_PART(ConnectionPointContainer)

 // IEnumConnectionPoints Implementation
    BEGIN_INTERFACE_PART(EnumConnectionPoints, IEnumConnectionPoints)
        STDMETHODIMP Next(ULONG cConnections, IConnectionPoint **rgpcn, ULONG *pcFetched);
        STDMETHODIMP Skip(ULONG cConnections);
        STDMETHODIMP Reset(void);
        STDMETHODIMP Clone(IEnumConnectionPoints **ppEnum);
    END_INTERFACE_PART(EnumConnectionPoints)
 
 // IConnectionPoint Implementation
    BEGIN_INTERFACE_PART(ConnectionPoint, IConnectionPoint)
        STDMETHODIMP GetConnectionInterface(IID *pIID);
        STDMETHODIMP GetConnectionPointContainer(IConnectionPointContainer **ppCPC);
        STDMETHODIMP Advise(IUnknown *pUnk, DWORD *pdwCookie);
        STDMETHODIMP Unadvise(DWORD dwCookie);
        STDMETHODIMP EnumConnections(IEnumConnections **ppEnum);
    END_INTERFACE_PART(ConnectionPoint)

 // IOleCommandTarget  Implementation
    BEGIN_INTERFACE_PART(OleCommandTarget , IOleCommandTarget)
        STDMETHODIMP QueryStatus(const GUID *pguidCmdGroup, ULONG cCmds, OLECMD prgCmds[], OLECMDTEXT *pCmdText);
        STDMETHODIMP Exec(const GUID *pguidCmdGroup, DWORD nCmdID, DWORD nCmdexecopt, VARIANTARG *pvaIn, VARIANTARG *pvaOut);            
    END_INTERFACE_PART(OleCommandTarget)

 // ISupportErrorInfo Implementation
    BEGIN_INTERFACE_PART(SupportErrorInfo, ISupportErrorInfo)
        STDMETHODIMP InterfaceSupportsErrorInfo(REFIID riid);
    END_INTERFACE_PART(SupportErrorInfo)

 // IObjectSafety Implementation
    BEGIN_INTERFACE_PART(ObjectSafety, IObjectSafety)
        STDMETHODIMP GetInterfaceSafetyOptions(REFIID riid, DWORD *pdwSupportedOptions,DWORD *pdwEnabledOptions);
        STDMETHODIMP SetInterfaceSafetyOptions(REFIID riid, DWORD dwOptionSetMask, DWORD dwEnabledOptions);
    END_INTERFACE_PART(ObjectSafety)

    STDMETHODIMP           InitializeNewInstance();

    STDMETHODIMP           InPlaceActivate(LONG lVerb);
    STDMETHODIMP_(void)    SetInPlaceVisible(BOOL fShow);
    STDMETHODIMP_(void)    UpdateModalState(BOOL fModeless, BOOL fNotifyIPObject);

    STDMETHODIMP_(void)    OnDraw(DWORD dvAspect, HDC hdcDraw, LPRECT prcBounds, LPRECT prcWBounds, HDC hicTargetDev, BOOL fOptimize);
    STDMETHODIMP_(void)    OnFocusChange(BOOL fGotFocus, HWND hFocusWnd);
    STDMETHODIMP_(void)    OnDestroyWindow();
    STDMETHODIMP_(void)    OnResize();
    STDMETHODIMP_(void)    OnButtonDown(UINT x, UINT y);
    STDMETHODIMP_(void)    OnMenuMessage(UINT msg, WPARAM wParam, LPARAM lParam);
    STDMETHODIMP_(void)    OnToolbarAction(DWORD cmd);

    STDMETHODIMP_(void)    OnComponentActivationChange(BOOL fActivate);
    STDMETHODIMP_(void)    OnAppActivation(BOOL fActive, DWORD dwThreadID);
    STDMETHODIMP_(void)    OnPaletteChanged(HWND hwndPalChg);

    STDMETHODIMP_(HMENU)   GetActivePopupMenu();
    STDMETHODIMP_(BOOL)    FRunningInDesignMode();
    STDMETHODIMP           DoDialogAction(dsoShowDialogType item);

    STDMETHODIMP           ProvideErrorInfo(HRESULT hres);

    inline void ViewChanged()
    {
        InvalidateRect(m_hwnd, NULL, TRUE);
        if (m_pViewAdviseSink) // send the view change notification to anybody listening.
        {
            m_pViewAdviseSink->OnViewChange(DVASPECT_CONTENT, -1);
            if (m_fViewAdviseOnlyOnce) // if they asked to be advised once, kill the connection
            m_xViewObjectEx.SetAdvise(DVASPECT_CONTENT, 0, NULL);
        }
    }

    inline void GetSizeRectAfterBorder(LPRECT lprc)
    {
        SetRect(lprc, 0, 0, m_Size.cx, m_Size.cy);
        if (m_fBorderStyle)	InflateRect(lprc, -(4-m_fBorderStyle), -(4-m_fBorderStyle));
        if (m_fShowTitlebar) lprc->top += 21;
    }

    inline void RedrawCaption()
    {
        if ((m_hwnd) && (m_fShowTitlebar))
        {   RECT rcT; GetClientRect(m_hwnd, &rcT); rcT.bottom = 21;
            InvalidateRect(m_hwnd, &rcT, FALSE);
        }
    }

    static
    STDMETHODIMP_(LRESULT) ControlWindowProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

private:
    ULONG                   m_cRef;				   // Reference count
    IUnknown               *m_pOuterUnknown;       // Outer IUnknown (points to m_xInternalUnknown if not agg)
    ITypeInfo              *m_pITypeInfo;          // Saved ITypeInfo for this object (IDispatch)

    HWND                    m_hwnd;                // our window
    HWND                    m_hwndParent;          // immediate parent window
    SIZEL                   m_Size;                // the size of this control  
    RECT                    m_rcLocation;          // where we at

    IOleClientSite         *m_pClientSite;         // active client site of host containter
    IOleControlSite        *m_pControlSite;        // control site
    IOleInPlaceSite        *m_pInPlaceSite;        // inplace site
    IOleInPlaceFrame       *m_pInPlaceFrame;       // inplace frame
    IOleInPlaceUIWindow    *m_pInPlaceUIWindow;    // inplace ui window

    IAdviseSink            *m_pViewAdviseSink;     // advise sink for view (only 1 allowed)
    IOleAdviseHolder       *m_pOleAdviseHolder;    // OLE advise holder (for data advise sinks)
    IDispatch              *m_dispEvents;          // event sink (we only support 1 at a time)

    CDsoDocObject          *m_pDocObjFrame;

    OLE_COLOR               m_clrBorderColor;      // Control Colors
    OLE_COLOR               m_clrBackColor;        // "
    OLE_COLOR               m_clrForeColor;        // "
    OLE_COLOR               m_clrTBarColor;        // "
    OLE_COLOR               m_clrTBarTextColor;    // "

    BSTR                    m_bstrCustomCaption;   // A custom caption (if provided)
    HMENU                   m_hmenuFilePopup;      // The File menu popup
    WORD                    m_wMenuItems;          // Bitflags of menu items enabled.

	class CDsoFrameWindowHook*  m_pFrameHook;
	HBITMAP                     m_hbmDeactive;

    unsigned int        m_fDirty:1;                // does the control need to be resaved?
    unsigned int        m_fInPlaceActive:1;        // are we in place active or not?
    unsigned int        m_fInPlaceVisible:1;       // we are in place visible or not?
    unsigned int        m_fUIActive:1;             // are we UI active or not.
    unsigned int        m_fViewAdvisePrimeFirst: 1;// for IViewobject2::setadvise
    unsigned int        m_fViewAdviseOnlyOnce: 1;  // for IViewobject2::setadvise
    unsigned int        m_fUsingWindowRgn:1;       // for SetObjectRects and clipping
    unsigned int        m_fFreezeEvents:1;         // should events be frozen?
    unsigned int        m_fDesignMode:1;           // are we in design mode?
    unsigned int        m_fModeFlagValid:1;        // has mode changed since last check?
    unsigned int        m_fBorderStyle:2;          // the border style
    unsigned int        m_fShowTitlebar:1;         // should we show titlebar?
    unsigned int        m_fShowToolbars:1;         // should we show toolbars?
    unsigned int        m_fModalState:1;           // are we modal?
    unsigned int        m_fObjectMenu:1;           // are we over obj menu item?
    unsigned int        m_fConCntDone:1;           // for enum connectpts
    unsigned int        m_fComponentActive:1;

};


////////////////////////////////////////////////////////////////////
// Top-Level Frame Window Hook
//
//  Used by the control to allow for proper host notification of 
//  focus and activation events occurring at top-level window frame.
//  Because this DocObject host is an OCX, we don't own these notifications
//  and have to "steal" them from our parent using a subclass.
//
//  The hook allows for more than one instance of the control by 
//  serializing activation notifications so only one instance of the
//  control can be "UI active" at a time. This is required for proper
//  ActiveX Document containment.
//
#define DSOF_MAX_CONTROLS      10

class CDsoFrameWindowHook
{
public:
	CDsoFrameWindowHook(){}
	~CDsoFrameWindowHook(){}

	static STDMETHODIMP_(CDsoFrameWindowHook*)
		AttachToFrameWindow(HWND hwndCtl, CDsoFramerControl* pocx);

	STDMETHODIMP Detach(CDsoFramerControl* pocx);
	STDMETHODIMP SetActiveComponent(CDsoFramerControl* pocx);

	inline STDMETHODIMP_(CDsoFramerControl*)
		GetActiveComponent(){return m_pControls[m_idxActive];}

    static STDMETHODIMP_(LRESULT) 
		HostWindowProcHook(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

protected:
	HWND                    m_hwndTopLevelHost;    // Top-level host window (hooked)
    WNDPROC                 m_pfnOrigWndProc;
	DWORD                   m_idxActive;
	DWORD                   m_cControls;
    CDsoFramerControl*      m_pControls[DSOF_MAX_CONTROLS];
};


#endif //DS_DSOFRAMER_H