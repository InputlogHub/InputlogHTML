/***************************************************************************
 * DSOFAUTO.CPP
 *
 * CDsoFramerControl: Automation interface for Binder Control
 *
 *  Copyright (c)1999-2001 Microsoft Corporation, All Rights Reserved
 *  Written by DSO Office Integration, Microsoft Developer Support
 *
 *  THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 *  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
 *  WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 *
 ***************************************************************************/
#include "dsoframer.h"


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl - _FramerControl Implementation
//

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::Activate
//
//  Activate the current embedded document (i.e, forward focus).
//
STDMETHODIMP CDsoFramerControl::Activate()
{
	HWND hwnd;

 // Activate this control in multi-control list (if applicable)...
    if ((!m_fComponentActive) && (m_pFrameHook))
	    m_pFrameHook->SetActiveComponent(this);

  // All we need to do is grab focus. This will tell the host to
  // UI activate our OCX (if not done already)...
	SetFocus(m_hwnd);

  // Then if we have an activedocument, ensure it is activated
  // and forward focus...
	if ((m_pDocObjFrame) && (hwnd = m_pDocObjFrame->GetActiveWindow()))
		SetFocus(hwnd);

    m_fComponentActive = TRUE;

	return S_OK;
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::get_ActiveDocument
//
//  Returns the automation object currently embedded.
//
//  Since we only support a single instance at a time, it might have been
//  better to call this property Object or simply Document, but VB reserves
//  the first name for use by the control extender, and IE reserves the second
//  in its extender, so we decided on the "Office sounding" name. ;-)
//
STDMETHODIMP CDsoFramerControl::get_ActiveDocument(IDispatch** ppdisp)
{
	IUnknown* punk;
	ODS("CDsoFramerControl::get_ActiveDocument\n");
	if (ppdisp) *ppdisp = NULL;
	if ((m_pDocObjFrame) && (punk = (IUnknown*)(m_pDocObjFrame->GetActiveObject())))
	{
		// QI will AddRef for us...
		punk->QueryInterface(IID_IDispatch, (void**)ppdisp);
	}
	return S_OK;
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::CreateNew
//
//  Creates a new document object based on the passed in ProgID. The 
//  ProgID should map to the "document" object, not the application per
//  se. In other words, use "Word.Document", "Excel.Sheet", etc.
//
STDMETHODIMP CDsoFramerControl::CreateNew(BSTR ProgId)
{
	HRESULT hr;
	CLSID clsid;
	RECT rcPlace;
	HCURSOR		hCur;

 // Check the string, and get the CLSID...
	if (!(ProgId) || (SysStringLen(ProgId) == 0))
		return E_INVALIDARG;

	if (FAILED(CLSIDFromProgID(ProgId, &clsid)))
		return ProvideErrorInfo(DSO_E_INVALIDPROGID);

 // Make sure any open document is closed first...
	if ((m_pDocObjFrame) && FAILED(hr = Close()))
		return hr;

 // Let's make a doc frame for ourselves...
	m_pDocObjFrame = new CDsoDocObject();
	if (!(m_pDocObjFrame)) return E_OUTOFMEMORY;

 // Start a wait operation to notify user...
	hCur = SetCursor(LoadCursor(NULL, IDC_WAIT));
	GetSizeRectAfterBorder(&rcPlace);

 // Init the doc site...
	if (SUCCEEDED(hr = m_pDocObjFrame->InitializeNewInstance(m_hwnd, 
			&rcPlace, (IOleCommandTarget*)&(m_xOleCommandTarget))))
	{
		__try
		{
		  // Create a new doc object and IP activate...
			hr = m_pDocObjFrame->CreateDocObject(clsid);
			if (SUCCEEDED(hr))
			{
				if (!m_fShowToolbars)
					m_pDocObjFrame->OnNotifyChangeToolState(FALSE);

				hr = m_pDocObjFrame->IPActivateView();
			}
		}
		__except(EvalException(GetExceptionCode()))
		{
			hr = E_WIN32_ACCESSVIOLATION;
		}
	}

 // Force a close if an error occurred...
	if (FAILED(hr))
	{
		m_fFreezeEvents = TRUE;
		Close();
		m_fFreezeEvents = FALSE;

		hr = ProvideErrorInfo(hr);
	}
	else if ((m_dispEvents) && !(m_fFreezeEvents))
	{
		VARIANT rgargs[2]; 
		DISPPARAMS dparms = {rgargs, NULL, 2, 0};
		memset(rgargs, 0, sizeof(VARIANT) * 2);

	 // Fire the OnDocumentOpened event...
		rgargs[0].vt = VT_DISPATCH;
		get_ActiveDocument(&(rgargs[0].pdispVal));
		rgargs[1].vt = VT_BSTR; rgargs[1].bstrVal = NULL;
		AutoDispInvoke(m_dispEvents, NULL, DSOF_DISPID_DOCOPEN, 0, 2, rgargs, NULL);

		VariantClear(&rgargs[0]);
    
     // Ensure we are active control...
        Activate();

     // Redraw the caption as needed...
        RedrawCaption();
    }

	SetCursor(hCur);
	return hr;
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::Open
//
//  Creates a document object based on a file or URL. This simulates an
//  "open", but preserves the correct OLE embedding activation required
//  by ActiveX Documents. Opening directly from a file is not recommended.
//  We keep a lock on the original file (unless opened read-only) so the
//  user cannot tell we don't have the file "open".
//
//  The alternate ProgID allows us to open a file that is not associated 
//  with an DocObject server (like *.asp) with the server specified. Also
//  the username/password are for web access (if Document is a URL).
//
STDMETHODIMP CDsoFramerControl::Open(VARIANT Document, VARIANT ReadOnly, VARIANT ProgId, VARIANT WebUsername, VARIANT WebPassword)
{
	HRESULT   hr;
	LPWSTR    pwszDocument  = LPWSTR_FROM_VARIANT(Document);
	LPWSTR    pwszAltProgId = LPWSTR_FROM_VARIANT(ProgId);
	LPWSTR    pwszUserName  = LPWSTR_FROM_VARIANT(WebUsername);
	LPWSTR    pwszPassword  = LPWSTR_FROM_VARIANT(WebPassword);
	CLSID     clsidAlt      = GUID_NULL;
	RECT      rcPlace;
	HCURSOR	  hCur;

 // We must have either a string (file path or URL) or an object to open from...
	if (!(pwszDocument) || (*pwszDocument == L'\0'))
		return E_INVALIDARG;

 // If the user passed the ProgId, find the alternative CLSID for server...
	if ((pwszAltProgId) && FAILED(CLSIDFromProgID(pwszAltProgId, &clsidAlt)))
		return E_INVALIDARG;

 // OK. If here, all the parameters look good and it is time to try and open
 // the document object. Start by closing any existing document first...
	if ((m_pDocObjFrame) && FAILED(hr = Close()))
		return hr;

 // Let's make a doc frame for ourselves...
	if (!(m_pDocObjFrame = new CDsoDocObject()))
		return E_OUTOFMEMORY;

 // Start a wait operation to notify user...
	hCur = SetCursor(LoadCursor(NULL, IDC_WAIT));
	GetSizeRectAfterBorder(&rcPlace);

	if (SUCCEEDED(hr = m_pDocObjFrame->InitializeNewInstance(m_hwnd, 
			&rcPlace, (IOleCommandTarget*)&(m_xOleCommandTarget))))
	{
		__try
		{
		 // Check if we are opening from a URL, and load that way...
			if (LooksLikeHTTP(pwszDocument))
			{
				hr = m_pDocObjFrame->LoadStorageFromURL(pwszDocument, clsidAlt,
						pwszUserName, pwszPassword, !(BOOL_FROM_VARIANT(ReadOnly, TRUE)));
			}
		 // Otherwise, check if it is a local file and open that...
			else if (FFileExists(pwszDocument))
			{
				hr = m_pDocObjFrame->LoadStorageFromFile(pwszDocument,
					clsidAlt, !(BOOL_FROM_VARIANT(ReadOnly, FALSE)));
			}
			else hr = E_INVALIDARG;

			if (SUCCEEDED(hr))
			{
				if (!m_fShowToolbars)
					m_pDocObjFrame->OnNotifyChangeToolState(FALSE);

				hr = m_pDocObjFrame->IPActivateView();
			}
		}
		__except(EvalException(GetExceptionCode()))
		{
			hr = E_WIN32_ACCESSVIOLATION;
		}

	}

  // Force a close if an error occurred...
	if (FAILED(hr))
	{
		m_fFreezeEvents = TRUE;
		Close();
		m_fFreezeEvents = FALSE;

		hr = ProvideErrorInfo(hr);
	}
	else if ((m_dispEvents) && !(m_fFreezeEvents))
	{
		VARIANT rgargs[2]; 
		memset(rgargs, 0, sizeof(VARIANT) * 2);

	 // Fire the OnDocumentOpened event...
		rgargs[0].vt = VT_DISPATCH;
		get_ActiveDocument(&(rgargs[0].pdispVal));
		rgargs[1].vt = VT_BSTR;
		rgargs[1].bstrVal = SysAllocString(pwszDocument);

		AutoDispInvoke(m_dispEvents, NULL, DSOF_DISPID_DOCOPEN, 0, 2, rgargs, NULL);

		VariantClear(&rgargs[1]);
		VariantClear(&rgargs[0]);
    
     // Ensure we are active control...
        Activate();

     // Redraw the caption as needed...
        RedrawCaption();

	}

	SetCursor(hCur);
	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::Save
//
//  Saves the current object. The optional SaveAs paramter lets the same
//  function act as a SaveAs method.
//
STDMETHODIMP CDsoFramerControl::Save(VARIANT SaveAsDocument, VARIANT OverwriteExisting, VARIANT WebUsername, VARIANT WebPassword)
{
	HCURSOR	  hCur;
	HRESULT	  hr = S_FALSE;
	LPWSTR    pwszDocument = LPWSTR_FROM_VARIANT(SaveAsDocument);
	LPWSTR    pwszUserName = LPWSTR_FROM_VARIANT(WebUsername);
	LPWSTR    pwszPassword = LPWSTR_FROM_VARIANT(WebPassword);
    BOOL      fOverwrite   = BOOL_FROM_VARIANT(OverwriteExisting, FALSE);

	if (!m_pDocObjFrame) return S_FALSE;

 // If user passed a value for SaveAs, it must be a valid string...
	if (!(IsVarParamMissing(SaveAsDocument)) && 
        (!(pwszDocument) || (*pwszDocument == L'\0')))
		return E_INVALIDARG;

	hCur = SetCursor(LoadCursor(NULL, IDC_WAIT));

	__try
	{
	 // Determine if they want to save to a URL...
		if ((pwszDocument) && LooksLikeHTTP(pwszDocument))
		{
			hr = m_pDocObjFrame->SaveStorageToURL(pwszDocument, fOverwrite, pwszUserName, pwszPassword);
		}
		else if (pwszDocument)
		{
		 // Save to file (local or UNC)...
			hr = m_pDocObjFrame->SaveStorageToFile(pwszDocument, fOverwrite);
		}
		else
		{
		 // Save back to open location...
			hr = m_pDocObjFrame->SaveDefault();
		}
	}
    __except(EvalException(GetExceptionCode()))
    {
        hr = E_WIN32_ACCESSVIOLATION;
    }

 // Redraw the caption as needed...
    if (SUCCEEDED(hr)) RedrawCaption();

	SetCursor(hCur);
	return ProvideErrorInfo(hr);
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::PrintOut
//
//  Prints the current object.
//
//  This method ends up calling IOleCommandTarget for Print. A more 
//  complete (but complex) approach would be to use IPrint. However, 
//  not all Office apps support IPrint, and for the sake of brevity we
//  decided to just use IOleCommandTarget for this sample.
//
STDMETHODIMP CDsoFramerControl::PrintOut(VARIANT PromptToSelectPrinter)
{
	BOOL  fPrompt = BOOL_FROM_VARIANT(PromptToSelectPrinter, FALSE);

 // We must have an open document to do this, but we don't fail per se...
	CHECK_NULL_RETURN(m_pDocObjFrame, S_FALSE);

	return m_pDocObjFrame->DoOleCommand(OLECMDID_PRINT, fPrompt);
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::Close
//
//  Closes the current object.
//
STDMETHODIMP CDsoFramerControl::Close()
{
	ODS("CDsoFramerControl::Close\n");

 // Fire the OnDocumentClosed event...
	if ((m_pDocObjFrame) && (m_dispEvents) && !(m_fFreezeEvents))
		AutoDispInvoke(m_dispEvents, NULL, DSOF_DISPID_DOCCLOSE, 0, 0, NULL, NULL);

 // Close the object...
	CDsoDocObject* pdframe = m_pDocObjFrame;
	m_pDocObjFrame = NULL;

	if (pdframe)
	{
		__try
		{
			pdframe->Close();
			delete pdframe;
		}
		__except(EvalException(GetExceptionCode())){}
	}

 // Redraw the caption as needed...
    RedrawCaption();

	return S_OK;
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::(put_Caption/get_Caption)
//
//  Allows you to set a custom cation for the titlebar.
//
STDMETHODIMP CDsoFramerControl::put_Caption(BSTR bstr)
{
 // Free exsiting caption (if any) and save new one (always dirties control)...
	if (m_bstrCustomCaption)
		{ SysFreeString(m_bstrCustomCaption); m_bstrCustomCaption = NULL;}

	if ((bstr) && (SysStringLen(bstr) > 0))
		m_bstrCustomCaption = SysAllocString(bstr);

	ViewChanged();
	m_fDirty = TRUE;

	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_Caption(BSTR* pbstr)
{
	if (pbstr) *pbstr = (m_bstrCustomCaption ? SysAllocString(m_bstrCustomCaption) : NULL);
	return S_OK;
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::(put_Titlebar/get_Titlebar)
//
//  True/False. Should we display the titlebar or not?
//
STDMETHODIMP CDsoFramerControl::put_Titlebar(VARIANT_BOOL vbool)
{
	TRACE1("CDsoFramerControl::put_Titlebar(%d)\n", vbool);
	if (m_fShowTitlebar != (WORD)(BOOL)vbool)
	{
		m_fShowTitlebar = (BOOL)vbool;
		m_fDirty = TRUE;
		OnResize();
		ViewChanged();
	}
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_Titlebar(VARIANT_BOOL* pbool)
{
	ODS("CDsoFramerControl::get_Titlebar\n");
	if (pbool) *pbool = (m_fShowTitlebar ? VARIANT_TRUE : VARIANT_FALSE);
	return S_OK;
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::(put_Toolbars/get_Toolbars)
//
//  True/False. Should we display toolbars or not?
//
STDMETHODIMP CDsoFramerControl::put_Toolbars(VARIANT_BOOL vbool)
{
	TRACE1("CDsoFramerControl::put_Toolbars(%d)\n", vbool);

 // If the control is in modal state, we can't do things that
 // will call the server directly, like toggle toolbars...
  	if (m_fModalState) return E_ACCESSDENIED;

	if (m_fShowToolbars != (WORD)(BOOL)vbool)
	{
		m_fShowToolbars = (BOOL)vbool;
		m_fDirty = TRUE;

		if (m_pDocObjFrame)
			m_pDocObjFrame->OnNotifyChangeToolState(m_fShowToolbars);

		ViewChanged();
	}
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_Toolbars(VARIANT_BOOL* pbool)
{
	ODS("CDsoFramerControl::get_Toolbars\n");
	if (pbool) *pbool = (m_fShowToolbars ? VARIANT_TRUE : VARIANT_FALSE);
	return S_OK;
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::(put_ModalState/get_ModalState)
//
//  True/False. Disables the active object (if any) thereby setting it 
//  up to behave "modal". Any time a dialog or other blocking window 
//  on the same thread is called, the developer should set this to True
//  to let the IP object know it should stay modal in the background. 
//  Set it back to False when the dialog is removed.
//
//  Technically, this should be a counter to allow for nested modal states.
//  However, we thought that might be confusing to some VB/Web developers
//  and since this is only a sample, made it a Boolean property.
//
STDMETHODIMP CDsoFramerControl::put_ModalState(VARIANT_BOOL vbool)
{
	TRACE1("CDsoFramerControl::put_ModalState(%d)\n", vbool);

	if (m_fModalState != (WORD)(BOOL)vbool)
		UpdateModalState((BOOL)vbool, TRUE);

	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_ModalState(VARIANT_BOOL* pbool)
{
	ODS("CDsoFramerControl::get_ModalState\n");
	if (pbool) *pbool = ((m_fModalState) ? VARIANT_TRUE : VARIANT_FALSE);
	return S_OK;
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::ShowDialog
//
//  Uses IOleCommandTarget to get the embedded object to display one of
//  these standard dialogs for the user.
//
STDMETHODIMP CDsoFramerControl::ShowDialog(dsoShowDialogType DlgType)
{
	HRESULT hr = E_ACCESSDENIED;

	TRACE1("CDsoFramerControl::ShowDialog(%d)\n", DlgType);
	if ((DlgType < dsoFileNew) || (DlgType > dsoDialogProperties))
		return E_INVALIDARG;

 // The first three dialog types we handle...
	if (DlgType < dsoDialogSaveCopy)
	{
		hr = DoDialogAction(DlgType);
	}
 // The others are provided by the server via IOleCommandTarget...
	else if (m_pDocObjFrame)
	{
		DWORD dwOleCmd;
		switch (DlgType)
		{
		case dsoDialogSaveCopy:   dwOleCmd = OLECMDID_SAVECOPYAS; break;
		case dsoDialogPageSetup:  dwOleCmd = OLECMDID_PAGESETUP;  break;
		case dsoDialogProperties: dwOleCmd = OLECMDID_PROPERTIES; break;
		default:                  dwOleCmd = OLECMDID_PRINT;
		}
		hr = m_pDocObjFrame->DoOleCommand(dwOleCmd, TRUE);
	}

	return ProvideErrorInfo(hr);
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::(put_EnableFileCommand/get_EnableFileCommand)
//
//  True/False. This allows the developer to disable certain menu/toolbar
//  items that are considered "file-level" -- New, Save, Print, etc.
//
//  We use the Item parameter to set a bit flag which is used when
//  displaying the menu to enable/disable the item. The OnFileCommand
//  event will not fire for disabled commands.
//
STDMETHODIMP CDsoFramerControl::put_EnableFileCommand(dsoFileCommandType Item, VARIANT_BOOL vbool)
{
	TRACE2("CDsoFramerControl::put_EnableFileCommand(%d, %d)\n", Item, vbool);

	if ((Item < dsoFileNew) || (Item > dsoFileProperties))
		return E_INVALIDARG;

	UINT code = (1 << Item);
	if (vbool == 0)	m_wMenuItems &= ~(code);
	else 		    m_wMenuItems |= code;

	if (m_pDocObjFrame)
		m_pDocObjFrame->DoOleCommand(OLECMDID_UPDATECOMMANDS, FALSE);

	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_EnableFileCommand(dsoFileCommandType Item, VARIANT_BOOL* pbool)
{
	TRACE1("CDsoFramerControl::get_EnableFileCommand(%d)\n", Item);

	if ((Item < dsoFileNew) || (Item > dsoFileProperties))
		return E_INVALIDARG;

	UINT code = (1 << Item);
	if (pbool) *pbool = ((m_wMenuItems & code) ? VARIANT_TRUE : VARIANT_FALSE);

	return S_OK;
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::(put_BorderStyle/get_BorderStyle)
//
//  Change the border style for the control.
//
STDMETHODIMP CDsoFramerControl::put_BorderStyle(dsoBorderStyle style)
{
	ODS("CDsoFramerControl::put_BorderStyle\n");
	if ((style < dsoBorderNone) || (style > dsoBorder3DThin))
		return E_INVALIDARG;

	if (m_fBorderStyle != (DWORD)style)
	{
		m_fBorderStyle = style;
		m_fDirty = TRUE;
		OnResize();
		ViewChanged();
	}
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_BorderStyle(dsoBorderStyle* pstyle)
{
	ODS("CDsoFramerControl::get_BorderStyle\n");
	if (pstyle)	*pstyle = (dsoBorderStyle)m_fBorderStyle;
	return S_OK;
}


////////////////////////////////////////////////////////////////////////
// Control Color Properties...
//
//
STDMETHODIMP CDsoFramerControl::put_BorderColor(OLE_COLOR clr)
{
	ODS("CDsoFramerControl::put_BorderColor\n");
	if (m_clrBorderColor != clr)
	{
		m_clrBorderColor = clr;
		m_fDirty = TRUE;
		ViewChanged();
	}
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_BorderColor(OLE_COLOR* pclr)
{if (pclr) *pclr = m_clrBorderColor; return S_OK;}

STDMETHODIMP CDsoFramerControl::put_BackColor(OLE_COLOR clr)
{
	ODS("CDsoFramerControl::put_BackColor\n");
	if (m_clrBackColor != clr)
	{
		m_clrBackColor = clr;
		m_fDirty = TRUE;
		ViewChanged();
	}
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_BackColor(OLE_COLOR* pclr)
{if (pclr) *pclr = m_clrBackColor; return S_OK;}

STDMETHODIMP CDsoFramerControl::put_ForeColor(OLE_COLOR clr)
{
	ODS("CDsoFramerControl::put_ForeColor\n");
	if (m_clrForeColor != clr)
	{
		m_clrForeColor = clr;
		m_fDirty = TRUE;
		ViewChanged();
	}
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_ForeColor(OLE_COLOR* pclr)
{if (pclr) *pclr = m_clrForeColor; return S_OK;}

STDMETHODIMP CDsoFramerControl::put_TitlebarColor(OLE_COLOR clr)
{
	ODS("CDsoFramerControl::put_TitlebarColor\n");
	if (m_clrTBarColor != clr)
	{
		m_clrTBarColor = clr;
		m_fDirty = TRUE;
		ViewChanged();
	}
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_TitlebarColor(OLE_COLOR* pclr)
{if (pclr) *pclr = m_clrTBarColor; return S_OK;}


STDMETHODIMP CDsoFramerControl::put_TitlebarTextColor(OLE_COLOR clr)
{
	ODS("CDsoFramerControl::put_TitlebarTextColor\n");
	if (m_clrTBarTextColor != clr)
	{
		m_clrTBarTextColor = clr;
		m_fDirty = TRUE;
		ViewChanged();
	}
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::get_TitlebarTextColor(OLE_COLOR* pclr)
{if (pclr) *pclr = m_clrTBarTextColor; return S_OK;}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl - IDispatch Implementation
//

////////////////////////////////////////////////////////////////////////
// Control's IDispatch Functions
//
//  These are largely standard and just forward calls to the functions
//  above. The only interesting thing here is the "hack" in Invoke to 
//  tell VB/IE that the control is always "Enabled".
//
STDMETHODIMP CDsoFramerControl::GetTypeInfoCount(UINT* pctinfo)
{if (pctinfo) *pctinfo = 1; return S_OK;}

STDMETHODIMP CDsoFramerControl::GetTypeInfo(UINT iTInfo, LCID lcid, ITypeInfo** ppTInfo)
{
	HRESULT hr = S_OK;

    ODS("CDsoFramerControl::GetTypeInfo\n");
	CHECK_NULL_RETURN(ppTInfo, E_POINTER); *ppTInfo = NULL;

    if (0 != iTInfo) return DISP_E_BADINDEX;

 // Load the type lib if we don't have the information already.
	if (NULL == m_pITypeInfo)
	{
		hr = GetTypeInfoEx(LIBID_DSOFramer, 0, 
            DSOFRAMERCTL_VERSION_MAJOR, DSOFRAMERCTL_VERSION_MINOR,
			v_hModule, IID__FramerControl, &m_pITypeInfo);

		RETURN_ON_FAILURE(hr);
	}

    (*ppTInfo = m_pITypeInfo)->AddRef();
    return hr;
}

STDMETHODIMP CDsoFramerControl::GetIDsOfNames(REFIID riid, LPOLESTR* rgszNames, UINT cNames, LCID lcid, DISPID* rgDispId)
{
	ODS("CDsoFramerControl::GetIDsOfNames\n");
    if (IID_NULL != riid) return DISP_E_UNKNOWNINTERFACE;

    ITypeInfo* pTI;
    HRESULT hr = GetTypeInfo(0, lcid, &pTI);
	RETURN_ON_FAILURE(hr);

    hr = pTI->GetIDsOfNames(rgszNames, cNames, rgDispId);
    RELEASE_INTERFACE(pTI);
    return hr;
}

STDMETHODIMP CDsoFramerControl::Invoke(DISPID dispIdMember, REFIID riid, LCID lcid, WORD wFlags, DISPPARAMS* pDispParams, VARIANT* pVarResult, EXCEPINFO* pExcepInfo, UINT* puArgErr)
{
    ITypeInfo* pTI;

	//TRACE1("CDsoFramerControl::Invoke (dispid = %d)\n", dispIdMember);
    if (IID_NULL != riid) return DISP_E_UNKNOWNINTERFACE;

 // VB/VBA love to check for this property during design time.
 // We don't implement it for this control, but we'll pretend
 // to make it happy and keep the code fast...
	if ((dispIdMember == DISPID_ENABLED) && (wFlags & DISPATCH_PROPERTYGET))
	{
		if (pVarResult) // We are always enabled...
			{pVarResult->vt = VT_BOOL;	pVarResult->boolVal = VARIANT_TRUE;	}
		return S_OK;
	}

    HRESULT hr = GetTypeInfo(0, lcid, &pTI);
	RETURN_ON_FAILURE(hr);

    hr = pTI->Invoke((IDispatch*)this, dispIdMember, wFlags, pDispParams, pVarResult, pExcepInfo, puArgErr);
    RELEASE_INTERFACE(pTI);
    return hr;
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::ProvideErrorInfo
//
//  Functions sets ErrorInfo in case of a custom error. Returns the
//  same HRESULT passed to it so it can be used like:
//
//    return ProvideErrorInfo(hr);
//
STDMETHODIMP CDsoFramerControl::ProvideErrorInfo(HRESULT hres)
{
 // Don't need to do anything on success...
	if ((hres == S_OK) || SUCCEEDED(hres))
		return  hres;

 // Check for one of our custom errors, and set as needed...
	if ((hres & DSO_E_ERR_BASE) == DSO_E_ERR_BASE)
	{
		ICreateErrorInfo *pcerrinfo;
		IErrorInfo *perrinfo;
		LPWSTR pwszSource, pwszError = NULL;
		CHAR szError[MAX_PATH];

		pwszSource = ConvertToLPWSTR("DsoFramerControl");

		if (LoadString(v_hModule, (hres & 0xFF), szError, MAX_PATH))
			pwszError = ConvertToLPWSTR(szError);

		if (SUCCEEDED(CreateErrorInfo(&pcerrinfo)))
		{
			pcerrinfo->SetGUID(IID__FramerControl);
			if (pwszSource)	pcerrinfo->SetSource(pwszSource);
			if (pwszError) pcerrinfo->SetDescription(pwszError);

			if (SUCCEEDED(pcerrinfo->QueryInterface(IID_IErrorInfo, (void**) &perrinfo)))
			{
				SetErrorInfo(0, perrinfo);
				perrinfo->Release();
			}
			pcerrinfo->Release();
		}

		SAFE_FREESTRING(pwszSource);
		SAFE_FREESTRING(pwszError);
	}

 // Whatever the error is, it is returned to host...
	return hres;
}
