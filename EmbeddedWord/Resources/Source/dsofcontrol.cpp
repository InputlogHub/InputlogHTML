/***************************************************************************
 * DSOFCONTROL.CPP
 *
 * CDsoFramerControl: The Base Control
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
// CDsoFramerControl - The Main Control Class
//
//  Serves as a base for the ActiveX Document site object which will do
//  the actual embedding.
//

CDsoFramerControl::CDsoFramerControl(LPUNKNOWN punk)
{
	ODS("CDsoFramerControl::CDsoFramerControl\n");
	m_pOuterUnknown = ((punk) ? punk : (IUnknown*)&m_xInternalUnknown);
}

CDsoFramerControl::~CDsoFramerControl(void)
{
	ODS("CDsoFramerControl::~CDsoFramerControl\n");

	RELEASE_INTERFACE(m_pITypeInfo);
	RELEASE_INTERFACE(m_pClientSite);
	RELEASE_INTERFACE(m_pControlSite);
	RELEASE_INTERFACE(m_pInPlaceSite);
	RELEASE_INTERFACE(m_pInPlaceFrame);
	RELEASE_INTERFACE(m_pInPlaceUIWindow);
	RELEASE_INTERFACE(m_pViewAdviseSink);
	RELEASE_INTERFACE(m_pOleAdviseHolder);
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::InitializeNewInstance
//
//  Sets up the inital state for the control. Any other member variable
//  not set here will be NULL/FALSE thanks to zero-init in MemAlloc.
//
STDMETHODIMP CDsoFramerControl::InitializeNewInstance()
{
	m_clrBorderColor	= 0x80000010; // System Button Shadow Color
	m_clrBackColor		= 0x80000005; // System Window Background Color
	m_clrForeColor		= 0x80000008; // System Window Text Color
	m_clrTBarColor		= 0x80000010; // System Button Shadow Color
	m_clrTBarTextColor	= 0x80000014; // System Button Highlight Color
	m_fBorderStyle		= dsoBorderFlat;
	m_fShowTitlebar		= TRUE;
	m_fShowToolbars		= TRUE;
	m_wMenuItems        = 0xFFFF;     // All items are "enabled" by default
	return S_OK;
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::InPlaceActivate
//
//  Does all the work in-place activating and/or ui activating the
//  control when asked to do so. Sets up the main control window, the 
//  needed interfaces, and (if not in design) subclasses the main top-level
//  window we can monitor messages that must be forwarded to an ip active
//  DocObject when we have one loaded.
//
STDMETHODIMP CDsoFramerControl::InPlaceActivate(LONG lVerb)
{
    HRESULT hr;
    SIZEL sizel;

	TRACE1("CDsoFramerControl::InPlaceActivate - %d\n", lVerb);

 // if we don't have a client site, then there's not much to do.
    if (!m_pClientSite) return S_OK;

 // get an InPlace site pointer.
    if (NULL == m_pInPlaceSite)
	{
        hr = m_pClientSite->QueryInterface(IID_IOleInPlaceSite, (void **)&m_pInPlaceSite);
        RETURN_ON_FAILURE(hr);
    }

 // if we're not already active, go and do it.
    if (!m_fInPlaceActive)
	{
        OLEINPLACEFRAMEINFO InPlaceFrameInfo;
        RECT rcPos, rcClip;

        hr = m_pInPlaceSite->CanInPlaceActivate();
        if (hr != S_OK){ if (!FAILED(hr)) hr = E_FAIL; goto cleanup;}

     // if we are here, then we have permission to go in-place active.
     // now, announce our intentions to actually go ahead and do this.
        hr = m_pInPlaceSite->OnInPlaceActivate();
        GOTO_ON_FAILURE(hr, cleanup);

     // if we're here, we're ready to go in-place active.  we just need
     // to set up some flags, and then create the window [if we have one]
        m_fInPlaceActive = TRUE;

     // we need to get some information about our location in the parent
     // window, as well as some information about the parent
        hr = m_pInPlaceSite->GetWindow(&m_hwndParent);
		if (FAILED(hr) || !IsWindow(m_hwndParent))
		{
			hr = OLE_E_INVALIDHWND;
			goto cleanup;
		}

        InPlaceFrameInfo.cb = sizeof(OLEINPLACEFRAMEINFO);
        hr = m_pInPlaceSite->GetWindowContext(&m_pInPlaceFrame, &m_pInPlaceUIWindow, &rcPos, &rcClip, &InPlaceFrameInfo);
        GOTO_ON_FAILURE(hr, cleanup);

     // make sure we'll display ourselves in the correct location with the correct size
        sizel.cx = rcPos.right - rcPos.left;
        sizel.cy = rcPos.bottom - rcPos.top;
        m_Size = sizel;

        m_xOleInplaceObject.SetObjectRects(&rcPos, &rcClip);

     // finally, create our window if we have to!
		if (NULL == m_hwnd)
		{
			WNDCLASS wndclass;

			EnterCriticalSection(&v_csecThreadSynch);

			if (GetClassInfo(v_hModule, "DSOFramerOCXWnd", &wndclass) == 0)
			{
				memset(&wndclass, 0, sizeof(WNDCLASS));
				wndclass.style          = CS_VREDRAW | CS_HREDRAW | CS_DBLCLKS;
				wndclass.lpfnWndProc    = CDsoFramerControl::ControlWindowProc;
				wndclass.hInstance      = v_hModule;
				wndclass.hCursor        = LoadCursor(NULL, IDC_ARROW);
				wndclass.lpszClassName  = "DSOFramerOCXWnd";
				RegisterClass(&wndclass);
			}

			m_hwnd = CreateWindowEx(0, "DSOFramerOCXWnd", NULL,
                            WS_CHILD | WS_CLIPSIBLINGS | WS_VISIBLE,
                            rcPos.left, rcPos.top, m_Size.cx, m_Size.cy,
                            m_hwndParent, NULL,	v_hModule, NULL);

			if (m_hwnd) SetWindowLong(m_hwnd, GWL_USERDATA, (LONG)this);

			LeaveCriticalSection(&v_csecThreadSynch);

			if (!m_hwnd) {hr = E_FAIL; goto cleanup;}
		}

     // finally, tell the host of this
        m_pClientSite->ShowObject();
    }

 // if we're not inplace visible yet, do so now.
    if (!m_fInPlaceVisible)
        SetInPlaceVisible(TRUE);

 // Hook top-level parent window if in RunMode. This let's us
 // track WM_ACTIVATEAPP messages, which are critical for DocObjs...
	if (!m_pFrameHook)
		m_pFrameHook = CDsoFrameWindowHook::AttachToFrameWindow(m_hwnd, this);

 // if we were asked to UIActivate, and we currently are not, do so!
    if ((lVerb == OLEIVERB_PRIMARY || 
		 lVerb == OLEIVERB_UIACTIVATE) && (!m_fUIActive))
	{
        m_fUIActive = TRUE;

     // inform the container of our intent
        m_pInPlaceSite->OnUIActivate();
	
     // notify the hook we have taked focus...
		if (m_pFrameHook)
			m_pFrameHook->SetActiveComponent(this);

     // take the focus  [which is what UI Activation is all about !]
        SetFocus(m_hwnd);

     // set ourselves up in the host.
		if (m_pInPlaceFrame)
	        m_pInPlaceFrame->SetActiveObject((IOleInPlaceActiveObject*)&m_xOleInplaceActiveObject, NULL);

        if (m_pInPlaceUIWindow)
            m_pInPlaceUIWindow->SetActiveObject((IOleInPlaceActiveObject*)&m_xOleInplaceActiveObject, NULL);

     // we have to explicitly say we don't wany any border space.
		if (m_pInPlaceFrame)
			m_pInPlaceFrame->SetBorderSpace(NULL);
			
        if (m_pInPlaceUIWindow)
            m_pInPlaceUIWindow->SetBorderSpace(NULL);

    }

    return S_OK; // be-de-be-de-be-de that's all folks!

cleanup:
 // something catastrophic happened [or, at least something bad].
 // die a horrible fiery mangled painful death.
    m_fInPlaceActive = FALSE;
    return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::SetInPlaceVisible
//
//  Make sure our control is visible or not (does not actually take ui
//  focus, that should be done in InPlaceActivate).
//
STDMETHODIMP_(void) CDsoFramerControl::SetInPlaceVisible(BOOL fShow)
{
    BOOL fVisible;
    m_fInPlaceVisible = fShow;

    // don't do anything if we don't have a window.  otherwise, set it
    if (m_hwnd)
	{
        fVisible = IsWindowVisible(m_hwnd);

        if (fVisible && !fShow)
			ShowWindow(m_hwnd, SW_HIDE);
        else if (!fVisible && fShow)
			ShowWindow(m_hwnd, SW_SHOWNA);
    }
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::UpdateModalState
//
//  Called either by developer (ModalState property) or by the embedded
//  DocObject to notify host (VB/IE) that we are modal.
//
STDMETHODIMP_(void) CDsoFramerControl::UpdateModalState(BOOL fModeless, BOOL fNotifyIPObject)
{
	TRACE2("CDsoFramerControl::UpdateModalState(AllowModeless=%d, NotifyIP=%d)\n", fModeless, fNotifyIPObject);
	if (fModeless == (int)m_fModalState)
	{
		IOleInPlaceActiveObject* pipao;

		m_fModalState = !(fModeless);
		ODS("Modal state changed\n");

	  // Excel doesn't like us to notify the host of changes in modality
	  // if it is the one who initialied the call. So, we check the
	  // NotifyIPObj flag and only notify host when the IPObj is not the caller...
		if ((fNotifyIPObject) && (m_pInPlaceFrame))
			m_pInPlaceFrame->EnableModeless(fModeless);

	  // Again, if IPObj is not the caller and we have Ipobj, let it know 
	  // of the change in modal state...
		if ((fNotifyIPObject) && (m_pDocObjFrame) && 
			(pipao = m_pDocObjFrame->GetActiveObject()))
			pipao->EnableModeless(fModeless);
	}
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::OnDraw
//
//  Drawing is largely limited to border and titlebar, except when in
//  design mode. We don't do any background painting to avoid paint problems
//  when we have an ui active DocObject. It should do any painting for us.
//
STDMETHODIMP_(void) CDsoFramerControl::OnDraw(DWORD dvAspect, HDC hdcDraw, LPRECT prcBounds, LPRECT prcWBounds, HDC hicTargetDev, BOOL fOptimize)
{
	COLORREF clrBackground, clrForeground, clrBorder, clrT;
	HBRUSH   hbshBackground, hbshBorder;
	HFONT    hfontText, hfontOld;
	HPALETTE hpal, hpalOld;
    LOGFONT  lfText = {-11,0,0,0,FW_NORMAL,0,0,0,0,0,0,0,FF_SWISS, "Tahoma"};
	RECT     rc, rcT;
	int      iTxtMode;

 // Copy out the bounding rect...
	CopyRect(&rc, prcBounds);

 // On 256 color systems, we may need a palette...
    if ((GetDeviceCaps(hdcDraw, RASTERCAPS) & RC_PALETTE) &&
		((GetDeviceCaps(hdcDraw, BITSPIXEL) * GetDeviceCaps(hdcDraw, PLANES)) == 8))
    {
        hpal = CreateHalftonePalette(hdcDraw);
		hpalOld = SelectPalette(hdcDraw, hpal, TRUE);
		RealizePalette(hdcDraw);
    }
	else hpal = NULL;

 // Translate the OLE_COLOR values to a matching COLORREF in palette...
	OleTranslateColor(m_clrBackColor,   hpal, &clrBackground);
	OleTranslateColor(m_clrForeColor,   hpal, &clrForeground);
	OleTranslateColor(m_clrBorderColor, hpal, &clrBorder);

 // Create the resources we need for drawing and setting up the DC...
	hbshBackground = CreateSolidBrush(clrBackground);
	hbshBorder     = CreateSolidBrush(clrBorder);

	hfontText = CreateFontIndirect(&lfText);
	hfontOld = (HFONT)SelectObject(hdcDraw, hfontText);
	iTxtMode = SetBkMode(hdcDraw, TRANSPARENT);


 // Fill in the background (but only if there is no active object)...
	if (!m_pDocObjFrame)
		FillRect(hdcDraw, &rc, hbshBackground);

 // Based on the choosen border style, draw the border and deflate the
 // drawing rect accordingly...
	switch (m_fBorderStyle)
	{
	case 1:
		{
		HBRUSH hbT = GetSysColorBrush(COLOR_BTNFACE);
		FrameRect(hdcDraw, &rc, hbshBorder);	InflateRect(&rc, -1, -1);
		FrameRect(hdcDraw, &rc, hbT);			InflateRect(&rc, -1, -1);
		FrameRect(hdcDraw, &rc, hbshBorder);	InflateRect(&rc, -1, -1);
		}
		break;

	case 2:
		DrawEdge(hdcDraw, &rc, EDGE_SUNKEN, BF_RECT);	
		InflateRect(&rc, -2, -2);
		break;

	case 3:
		DrawEdge(hdcDraw, &rc, BDR_SUNKENOUTER, BF_RECT);
		InflateRect(&rc, -1, -1);
		break;

	//default: no border...
	}

 // Draw the titlebar if visible...
	if (m_fShowTitlebar)
	{
		COLORREF clrTitlebar, clrTitlebarText;
		HBRUSH   hbshTitlebar, hbshT;
        LPWSTR   pwszROCaption = NULL;
        LPWSTR   pwszCaption = ((m_bstrCustomCaption) ? 
                    m_bstrCustomCaption : L"Office Framer Control Sample");

		CopyRect(&rcT, &rc);
		rcT.bottom = rcT.top + 21;

		OleTranslateColor(m_clrTBarColor,     hpal, &clrTitlebar);
		OleTranslateColor(m_clrTBarTextColor, hpal, &clrTitlebarText);

		hbshTitlebar = CreateSolidBrush(clrTitlebar);
		FillRect(hdcDraw, &rcT, hbshTitlebar);

		DrawIconEx(hdcDraw, rcT.left+2, rcT.top+2, v_icoOffDocIcon, 16, 16, 0, NULL, DI_NORMAL);

		clrT = SetTextColor(hdcDraw, clrTitlebarText);
		rcT.left += 22;

        if ((m_pDocObjFrame) && (m_pDocObjFrame->IsReadOnly()))
            pwszROCaption = CopyStringCat(pwszCaption, L" (Read-Only)");

	 // Draw out the custom titlebar caption in Unicode if on NT/XP OS, since
	 // a custom caption may have non-ANSI characters...
        FDrawText(hdcDraw, ((pwszROCaption) ? pwszROCaption : pwszCaption),
            &rcT, DT_VCENTER|DT_SINGLELINE);

        if (pwszROCaption)
            MemFree(pwszROCaption);

		CopyRect(&rcT, &rc);
		rcT.bottom = rcT.top + 21;
		rcT.top = rcT.bottom - 1;

		hbshT = GetSysColorBrush(COLOR_BTNSHADOW);
		FillRect(hdcDraw, &rcT, hbshT);

		SetTextColor(hdcDraw, clrT);
		DeleteObject(hbshTitlebar);
	}

 // When in design mode, we do a little extra stuff to help the 
 // developer visualize some of the control settings (toolbars,
 // foreground color, etc.)...
	if ((!m_pDocObjFrame) && FRunningInDesignMode())
	{
		UINT pxHeight = (rc.bottom - rc.top);

		if (pxHeight > 46)
		{
			GetSizeRectAfterBorder(&rcT);
			rcT.top = (pxHeight / 2);

			clrT = SetTextColor(hdcDraw, clrForeground);
			DrawText(hdcDraw, "DSO ActiveX Document Framer Control (Sample)", -1, &rcT, DT_WORDBREAK | DT_CENTER);
			SetTextColor(hdcDraw, clrT);
		}

		if ((m_fShowToolbars) && (pxHeight > 46))
		{
			GetSizeRectAfterBorder(&rcT);
			rcT.bottom = rcT.top + 25;

			FillRect(hdcDraw, &rcT, GetSysColorBrush(COLOR_BTNFACE));

			clrT = SetTextColor(hdcDraw, clrBackground);
			DrawText(hdcDraw, "< Toolbar Space >", -1, &rcT, DT_SINGLELINE|DT_VCENTER|DT_CENTER);
			SetTextColor(hdcDraw, clrT);
		}

	}
	else if ((m_pDocObjFrame) && (!m_fComponentActive))
	{
	 // In the case we have more than one component, and this component is
	 // has an object but is not ui active we draw a bitmap representation
	 // of the obect to fill the area. This gives the user a sense the object
	 // is there but in background, and can click to reactivate...

		GetSizeRectAfterBorder(&rcT);

		if (m_hbmDeactive)
		{
			HDC hdcT = CreateCompatibleDC(hdcDraw);
			HBITMAP bmpOld = (HBITMAP)SelectObject(hdcT, m_hbmDeactive);
			BITMAP bm;

			if (GetObject(m_hbmDeactive, sizeof(BITMAP), &bm))
			{
				StretchBlt(hdcDraw, rcT.left, rcT.top, 
					(rcT.right - rcT.left), (rcT.bottom - rcT.top),
					 hdcT, 0, 0, bm.bmWidth, bm.bmHeight, SRCCOPY);
			}
			else
			{
				BitBlt(hdcDraw, rcT.left, rcT.top, 
					(rcT.right - rcT.left), (rcT.bottom - rcT.top), hdcT, 0,0, SRCCOPY);
			}

			SelectObject(hdcT, bmpOld);
			DeleteDC(hdcT);
		}
		else
		{
			FillRect(hdcDraw, &rcT, hbshBackground);
			rcT.top = ((rc.bottom - rc.top) / 2);

			clrT = SetTextColor(hdcDraw, clrForeground);
			DrawText(hdcDraw, "Unable to display document while inactive. Click here to reactivate object.", -1, &rcT, DT_WORDBREAK | DT_CENTER);
			SetTextColor(hdcDraw, clrT);
		}
	}

 // Cleanup the hDC as required...
	DeleteObject(hbshBackground);
	DeleteObject(hbshBorder);

	SelectObject(hdcDraw, hfontOld);
	DeleteObject(hfontText);

	if (hpal)
	{
		SelectPalette(hdcDraw, hpalOld, TRUE);
		DeleteObject(hpal);
	}

	SetBkMode(hdcDraw, iTxtMode);
	return;
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::OnFocusChange
//
//  Selectively handle focus changes to make sure both host and ui 
//  active docobject are in synch.
//
STDMETHODIMP_(void) CDsoFramerControl::OnFocusChange(BOOL fGotFocus, HWND hFocusWnd)
{
	HWND hwndT;
	TRACE1("CDsoFramerControl::OnFocusChange(%d)\n", fGotFocus);

  // When IP active, notify the control host of focus change...
	if ((m_fInPlaceActive) && (m_pControlSite))
	{
		if (fGotFocus) // If we got focus...
		{

		 // Notify host we have focus (we only need
		 // to do this if we are not already UI Active)...
			if (!(m_fUIActive))
				m_pControlSite->OnFocus(fGotFocus);

		 // If we have an active document, forward the focus...
			if ((m_pDocObjFrame) && (hwndT=m_pDocObjFrame->GetActiveWindow()))
				SetFocus(hwndT);

		}
		else // else we lost focus...
		{

		 // When we lose focus, only notify host if we lost to window
		 // that does not belong to us...
			hwndT = hFocusWnd;
			while (hwndT = GetParent(hwndT))
				if (hwndT == m_hwnd) return;

			ODS("Notify host of lost focus (ie, UI deactivate us)\n");
			m_pControlSite->OnFocus(fGotFocus);
		}
	}

	return;
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::OnDestroyWindow
//
//  When the window closes make sure the object closes and release the
//  subclass we have on the parent's top-level window.
//
STDMETHODIMP_(void) CDsoFramerControl::OnDestroyWindow()
{
	ODS("CDsoFramerControl::OnDestroyWindow\n");
	m_hwnd = NULL;

	Close();
	
	if (m_pFrameHook)
	{
		m_pFrameHook->Detach(this);
		m_pFrameHook = NULL;
	}

	if (m_hbmDeactive)
	{
		DeleteObject(m_hbmDeactive);
		m_hbmDeactive = NULL;
	}

	if (m_hmenuFilePopup)
	{
		DestroyMenu(m_hmenuFilePopup);
		m_hmenuFilePopup = NULL;
	}

}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::OnResize
//
//  Resize the window and notify the doc site.
//
STDMETHODIMP_(void) CDsoFramerControl::OnResize()
{
	RECT rcPlace;
	ODS("CDsoFramerControl::OnResize\n");
	if (m_pDocObjFrame)
	{
		GetSizeRectAfterBorder(&rcPlace);
		m_pDocObjFrame->OnNotifySizeChange(&rcPlace);
	}
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::OnButtonDown
//
//  We watch for mouse button down events when the titlebar is visible
//  so we can drop down our popup menu if the user clicks around the 
//  area of the file icon. This is our workaround for menu merging.
//
STDMETHODIMP_(void) CDsoFramerControl::OnButtonDown(UINT x, UINT y)
{
	HMENU hTest = NULL;
	RECT rc;

 // If we don't have UI focus, take it...
	if (!m_fUIActive) Activate();

 // If the caption is showing, we are not modal, and user clicked in the 
 // area around the office doc icon, show the popup menu...
	if ((m_fShowTitlebar) && (!m_fModalState) && ((x < 22) && (y < 22)))
	{
	 // This will get the File menu or the merged menu based on current state...
		hTest = GetActivePopupMenu();
		if (hTest)
		{
		 // We'll place it right below the titlebar just like sys menu...
			GetSizeRectAfterBorder(&rc);
			MapWindowPoints(m_hwnd, HWND_DESKTOP, (POINT*)&rc, 2);
			TrackPopupMenu(hTest, 0, rc.left, rc.top - 1, 0, m_hwnd, NULL);
		}
	}
	return;
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::OnMenuMessage
//
//  We should get these messages when we are displaying the popup menu.
//  The "File" menu is ours, but the popup may have other menus copied
//  from the merged menu set by the ui active docobject, so we need to
//  forward those messages (as needed) to the right window handler.
//
//  In addition, code is added to enable/disable File menu items based
//  on flags set by developer in EnableFileCommand, and/or call the
//  correct function if one of those commands was selected by user.
//
STDMETHODIMP_(void) CDsoFramerControl::OnMenuMessage(UINT msg, WPARAM wParam, LPARAM lParam)
{
	HMENU hmenu, hmenuMerged = NULL;
	BOOL  fAlwaysSendMessage = FALSE;
	HWND  hwndObjectMenu = NULL;

	if (m_pDocObjFrame)
	{
		hwndObjectMenu = m_pDocObjFrame->GetMenuHWND();
		hmenuMerged = m_pDocObjFrame->GetMergedMenu();
	}

	switch (msg)
	{
	case WM_INITMENU:  m_fObjectMenu = FALSE;      //fall through...
	case WM_ENTERIDLE: fAlwaysSendMessage = TRUE;
		break;

	case WM_MENUSELECT:
		if ((lParam == 0) && (HIWORD(wParam) == 0xFFFF))
			fAlwaysSendMessage = TRUE;
		break;

	case WM_INITMENUPOPUP:
		hmenu = (HMENU)wParam;
		m_fObjectMenu = ((hmenu != m_hmenuFilePopup) && (hmenu != hmenuMerged));

		if ((!m_fObjectMenu) && (hmenu == m_hmenuFilePopup))
		{
			EnableMenuItem(hmenu, MNU_NEW,     MF_BYCOMMAND | ((m_wMenuItems & 1) ? MF_ENABLED : (MF_DISABLED | MF_GRAYED)));
			EnableMenuItem(hmenu, MNU_OPEN,    MF_BYCOMMAND | ((m_wMenuItems & 2) ? MF_ENABLED : (MF_DISABLED | MF_GRAYED)));
			EnableMenuItem(hmenu, MNU_CLOSE,   MF_BYCOMMAND | (((m_wMenuItems &   4) && (m_pDocObjFrame)) ? MF_ENABLED : (MF_DISABLED | MF_GRAYED)));
			EnableMenuItem(hmenu, MNU_SAVE,    MF_BYCOMMAND | (((m_wMenuItems &   8) && (m_pDocObjFrame)) ? MF_ENABLED : (MF_DISABLED | MF_GRAYED)));
			EnableMenuItem(hmenu, MNU_SAVEAS,  MF_BYCOMMAND | (((m_wMenuItems &  16) && (m_pDocObjFrame)) ? MF_ENABLED : (MF_DISABLED | MF_GRAYED)));
			EnableMenuItem(hmenu, MNU_PGSETUP, MF_BYCOMMAND | (((m_wMenuItems &  64) && (m_pDocObjFrame)) ? MF_ENABLED : (MF_DISABLED | MF_GRAYED)));
			EnableMenuItem(hmenu, MNU_PRINT,   MF_BYCOMMAND | (((m_wMenuItems &  32) && (m_pDocObjFrame)) ? MF_ENABLED : (MF_DISABLED | MF_GRAYED)));
			EnableMenuItem(hmenu, MNU_PROPS,   MF_BYCOMMAND | (((m_wMenuItems & 128) && (m_pDocObjFrame)) ? MF_ENABLED : (MF_DISABLED | MF_GRAYED)));
		}
		break;

	case WM_COMMAND:

		if ((m_fObjectMenu) && (hwndObjectMenu))
			PostMessage(hwndObjectMenu, msg, wParam, lParam);
		else
		{
			DWORD dwCmd = 0;
			switch ((int)LOWORD(wParam))
			{
			case MNU_NEW:     dwCmd = OLECMDID_NEW;  break;
			case MNU_OPEN:    dwCmd = OLECMDID_OPEN;  break;
			case MNU_SAVE:    dwCmd = OLECMDID_SAVE;  break;
			case MNU_SAVEAS:  dwCmd = OLECMDID_SAVEAS;  break;
			case MNU_PGSETUP: dwCmd = OLECMDID_PAGESETUP;  break;
			case MNU_PRINT:   dwCmd = OLECMDID_PRINT;  break;
			case MNU_PROPS:   dwCmd = OLECMDID_PROPERTIES;  break;
			}
				
			PostMessage(m_hwnd, WM_APP+300, dwCmd, 0);
		}
		return; // we just return

	}

	if ((hwndObjectMenu) && ((m_fObjectMenu) || (fAlwaysSendMessage)))
		SendMessage(hwndObjectMenu, msg, wParam, lParam);

	return;
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::OnToolbarAction
//
//  Does an associated OLECMDID action and raises the OnFileCommand event
//  for developer to override as needed. The name comes from the fact that
//  most Office servers will call this (via IOleCommandTarget) when user
//  clicks on a toolbar button for file-command item. It also serves for
//  our menu commands to keep the code consistant. Both menu and toolbar
//  call this method asynchronously (via PostMessage) to avoid blocking the
//  remote docobject server.
//
STDMETHODIMP_(void) CDsoFramerControl::OnToolbarAction(DWORD cmd)
{
	ODS("CDsoFramerControl::OnToolbarAction\n");
	HRESULT hr = S_OK;
	WORD wCancelAction = 0;
	VARIANT rgargs[2];
	VARIANT vtT = {0};

 // We will do a default action by default, but give developer a chance
 // to override in OnFileCommand event...
	rgargs[0].vt = VT_BOOL|VT_BYREF;
	rgargs[0].pboolVal = (short*)&wCancelAction;

 // Map the OLECMDID to one of our values...
	rgargs[1].vt = VT_I4;
	switch (cmd)
	{
	case OLECMDID_NEW:           rgargs[1].lVal = dsoFileNew; break;
	case OLECMDID_OPEN:          rgargs[1].lVal = dsoFileOpen; break;
	case OLECMDID_SAVE:          rgargs[1].lVal = dsoFileSave; break;
	case OLECMDID_SAVEAS:        rgargs[1].lVal = dsoFileSaveAs; break;
	case OLECMDID_PRINT:         rgargs[1].lVal = dsoFilePrint; break;
	case OLECMDID_PAGESETUP:     rgargs[1].lVal = dsoFilePageSetup; break;
	case OLECMDID_PROPERTIES:    rgargs[1].lVal = dsoFileProperties; break;
	default:    rgargs[1].lVal = dsoFileClose;
	}
	
 // Only do action if item is enabled...
	if ((1 << rgargs[1].lVal) & m_wMenuItems)
	{
	 // Let control developer handle the event first...
		if ((m_dispEvents)  && !(m_fFreezeEvents))
		{
			hr = AutoDispInvoke(m_dispEvents, NULL, DSOF_DISPID_FILECMD, 0, 2, rgargs, NULL);
			TRACE1("Disp event returned 0x%X \n", hr);
		}

     // If the event was canceled (or event handler threw an
     // unhandled error from user code), bail out now...
        if ((wCancelAction) || (hr == DISP_E_EXCEPTION))
            return;

     // Do the action based on the event...
        switch (rgargs[1].lVal)
        {
        case dsoFileClose:		hr = Close();	break;
        case dsoFilePrint:		hr = ShowDialog(dsoDialogPrint); break;
        case dsoFilePageSetup:	hr = ShowDialog(dsoDialogPageSetup); break;
        case dsoFileProperties:	hr = ShowDialog(dsoDialogProperties); break;

        case dsoFileSave:
	        hr = Save(vtT, vtT, vtT, vtT);
	        if (hr != DSO_E_DOCUMENTREADONLY)
		        break; // fall through to SaveAs if file is read-only...

        default:
	        hr = DoDialogAction(
		        (rgargs[1].lVal == dsoFileNew) ? dsoDialogNew : 
		        (rgargs[1].lVal == dsoFileOpen) ? dsoDialogOpen : dsoDialogSave);
        }

     // Display error information to user if we failed...
        if (FAILED(hr) && (hr != E_ABORT))
        {
            if (hr == DSO_E_COMMANDNOTSUPPORTED)
            {
	            MessageBox(m_hwnd, 
		            "This operation could not be performed because the document server "
		            "does not support the command, or it is disabled when embedded.",
		            "Command Not Supported", MB_ICONINFORMATION|MB_SETFOREGROUND);
            }
            else if (hr == DSO_E_DOCUMENTREADONLY)
            {
	            MessageBox(m_hwnd, 
		            "The operation could not be performed because the document was opened read-only.",
		            "Unable to Update Document", MB_ICONINFORMATION|MB_SETFOREGROUND);
            }
            else if ((hr == DSO_E_INVALIDSERVER) || (hr == DSO_E_INVALIDPROGID))
            {
	            MessageBox(m_hwnd, "The item selected is not associated with an ActiveX "					
		            "Document server. It cannot be loaded into the control.",
		            "Cannot Open Item", MB_ICONINFORMATION|MB_SETFOREGROUND);
            }
            else if (hr == DSO_E_REQUIRESMSDAIPP)
            {
	            MessageBox(m_hwnd, 
		            "You cannot open/save to a web folder unless MDAC 2.5 is installed. The operation cannot be performed.",
		            "Unable to Open/Save Document", MB_ICONINFORMATION|MB_SETFOREGROUND);
            }
            else 
            {
	            MessageBox(m_hwnd, "The program encountered an error trying to perform the command. "					
		            "If the problem persists, contact the program vendor for a possible solution.",
		            "Command Error", MB_ICONHAND|MB_SETFOREGROUND);
            }
        }
	}

	return;
}



////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::OnComponentActivationChange
//
//  Handles showing/hiding doc object during activation state changes.
//  This is needed to properly handle focus changes when more than one
//  instance of the control is running on the same top-level window.
//
STDMETHODIMP_(void) CDsoFramerControl::OnComponentActivationChange(BOOL fActivate)
{
	TRACE1("CDsoFramerControl::OnComponentActivationChange(%d)\n", fActivate);

	if ((fActivate) && (!m_fComponentActive))
	{
		m_fComponentActive = TRUE;

		if (m_pDocObjFrame)
			ShowWindow(m_pDocObjFrame->GetDocWindow(), SW_SHOW);

		OnAppActivation(TRUE, NULL);
	}
	else if ((!fActivate) && (m_fComponentActive))
	{

 		OnAppActivation(FALSE, NULL);

		if (m_pDocObjFrame)
		{
		    if (m_hbmDeactive) DeleteObject(m_hbmDeactive);
            m_hbmDeactive = GetDeactiveBitmapFromWindow(m_pDocObjFrame->GetDocWindow());
			ShowWindow(m_pDocObjFrame->GetDocWindow(), SW_HIDE);
		}

		m_fComponentActive = FALSE;
	}
}




////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::OnAppActivation
//
//  This method is called from the subclassed parent to notify us of
//  WM_ACTIVATEAPP messages which we pass on to ui active docobject. This
//  notification is required by docobject spec.
//
STDMETHODIMP_(void) CDsoFramerControl::OnAppActivation(BOOL fActive, DWORD dwThreadID)
{
	TRACE1("CDsoFramerControl::OnAppActivation(Active=%d)\n", fActive);
	if (m_pDocObjFrame) m_pDocObjFrame->OnNotifyAppActivate(fActive, dwThreadID);
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::OnPaletteChanged
//
//  This method is also called from the subclassed parent. Per docobject
//  spec, a ui active object should get first chance of any palette updates.
//
STDMETHODIMP_(void) CDsoFramerControl::OnPaletteChanged(HWND hwndPalChg)
{
	ODS("CDsoFramerControl::OnPaletteChanged\n");
	if (m_pDocObjFrame)	m_pDocObjFrame->OnNotifyPaletteChanged(hwndPalChg);
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::GetActivePopupMenu
//
//  Constructs our popup menu and returns the handle. The menu can be
//  either the File menu by itself (no docobj is open) or the merged
//  menu as a popup.
//
STDMETHODIMP_(HMENU) CDsoFramerControl::GetActivePopupMenu()
{
	HMENU hPopup, hMergedMenu, hServerMenu;

 // If we haven't made a File menu yet, make it. Here is where you might
 // want to add custom items if you have other file-level commands you would
 // the control to support. These are the basics...
	if (!m_hmenuFilePopup)
	{
		m_hmenuFilePopup = CreatePopupMenu();
		if (!m_hmenuFilePopup) return NULL;

		AppendMenu(m_hmenuFilePopup, MF_STRING,    MNU_NEW,     "&New...\tCtrl+N");
		AppendMenu(m_hmenuFilePopup, MF_STRING,    MNU_OPEN,    "&Open...\tCtrl+O");
		AppendMenu(m_hmenuFilePopup, MF_STRING,    MNU_CLOSE,   "&Close");
		AppendMenu(m_hmenuFilePopup, MF_SEPARATOR, 0,            NULL);
		AppendMenu(m_hmenuFilePopup, MF_STRING,    MNU_SAVE,    "&Save\tCtrl+S");
		AppendMenu(m_hmenuFilePopup, MF_STRING,    MNU_SAVEAS,  "Save &As...");
		AppendMenu(m_hmenuFilePopup, MF_SEPARATOR, 0,            NULL);
		AppendMenu(m_hmenuFilePopup, MF_STRING,    MNU_PGSETUP, "Page Set&up...");
		AppendMenu(m_hmenuFilePopup, MF_STRING,    MNU_PRINT,   "&Print...");
		AppendMenu(m_hmenuFilePopup, MF_SEPARATOR, 0,            NULL);
		AppendMenu(m_hmenuFilePopup, MF_STRING,    MNU_PROPS,   "Propert&ies");
	}

 // If we have a docobj and it has a merged menu, then lets return a 
 // merged menu between the docobj and our file menu...
	if ((m_pDocObjFrame) && (m_pDocObjFrame->GetMenuHWND()))
	{
	 // We only need to create the merged copy once.
		if (!(hMergedMenu = m_pDocObjFrame->GetMergedMenu()))
		{
			hMergedMenu = CreatePopupMenu();
			if ((hMergedMenu) && (hServerMenu = (m_pDocObjFrame->GetActiveMenu())))
			{
				char szbuf[MAX_PATH];
				HMENU hT;

				int cbMenuCnt = GetMenuItemCount(hServerMenu);
				for (int i = 0; i < cbMenuCnt; i++)
				{
					hT = GetSubMenu(hServerMenu, i);
					if (hT)
					{
						szbuf[0] = '\0';
						GetMenuString(hServerMenu, i, szbuf, MAX_PATH, MF_BYPOSITION);
						InsertMenu(hMergedMenu, i, MF_BYPOSITION|MF_POPUP, (UINT)hT, szbuf);
					}
				}

				InsertMenu(hMergedMenu, 0, MF_BYPOSITION|MF_POPUP, (UINT)m_hmenuFilePopup, "&File");
				m_pDocObjFrame->SetMergedMenu(hMergedMenu);
			}
		}

		hPopup = hMergedMenu;
	}
	else hPopup = m_hmenuFilePopup;

	return hPopup;
}


////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::FRunningInDesignMode
//
//  Determines if we are in design mode. Called when painting the control
//  and when deciding whether to subclass the parent.
//
STDMETHODIMP_(BOOL) CDsoFramerControl::FRunningInDesignMode()
{
	IDispatch *pdisp;

 // We must have a control site.
	CHECK_NULL_RETURN(m_pClientSite, FALSE);

 // If we have done this before, we don't need to keep doing it unless
 // the host has notified us of the state change (see our code in 
 // XOleControl::OnAmbientPropertyChange)...
	if ((!m_fModeFlagValid) && 
		SUCCEEDED(m_pClientSite->QueryInterface(IID_IDispatch, (void **)&pdisp)))
	{
		VARIANT vtUserMode;
		m_fDesignMode = FALSE; // assume run mode

		if (SUCCEEDED(AutoDispInvoke(pdisp, NULL, 
				DISPID_AMBIENT_USERMODE, DISPATCH_PROPERTYGET, 0, NULL, &vtUserMode)))
		{
		 // UserMode is True when control is in Run mode, False when in design.
		 // We assume run mode, so we only care to set the flag if in design.
			m_fDesignMode = !(BOOL_FROM_VARIANT(vtUserMode, TRUE));
			VariantClear(&vtUserMode);
		}

		m_fModeFlagValid = TRUE;
	}

	return m_fDesignMode;
}

////////////////////////////////////////////////////////////////////////
// CDsoFramerControl::ControlWindowProc
//
//  The window proc for our control.
//
STDMETHODIMP_(LRESULT) CDsoFramerControl::ControlWindowProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	CDsoFramerControl* pCtl = (CDsoFramerControl*)GetWindowLong(hwnd, GWL_USERDATA);
	if (pCtl)
	{
		switch (msg)
		{
			case WM_PAINT:
			{
				// call the OnDraw routine.
				PAINTSTRUCT ps;
				RECT        rc;
				HDC         hdc;
				hdc = BeginPaint(hwnd, &ps);
				GetClientRect(hwnd, &rc);
				pCtl->OnDraw(DVASPECT_CONTENT, hdc, (RECT*)&rc, NULL, NULL, TRUE);
				EndPaint(hwnd, &ps);
			}
			break;

			case WM_SIZE:
				pCtl->OnResize();
				break;

			case WM_DESTROY:
				pCtl->OnDestroyWindow();
				break;

			case WM_NCDESTROY:
				SetWindowLong(hwnd, GWL_USERDATA, 0);
				break;

			case WM_SETFOCUS:
			case WM_KILLFOCUS:
				pCtl->OnFocusChange((msg == WM_SETFOCUS), (HWND)wParam);
				break;

			case WM_LBUTTONDOWN:
				pCtl->OnButtonDown(LOWORD(lParam), HIWORD(lParam));
				break;

			case (WM_APP+300):
				pCtl->OnToolbarAction(wParam);
				break;

			case (WM_APP+301):
				pCtl->UpdateModalState((BOOL)wParam, FALSE);
				break;

			case WM_MENUSELECT:
			case WM_DRAWITEM:
			case WM_MEASUREITEM:
			case WM_ENTERIDLE:
			case WM_INITMENU:
			case WM_INITMENUPOPUP:
			case WM_COMMAND:
				pCtl->OnMenuMessage(msg, wParam, lParam);
				break;
		}

	}

	return DefWindowProc(hwnd, msg, wParam, lParam);
}


////////////////////////////////////////////////////////////////////////
//
// OCX Interfaces for CDsoFramerControl
//
//  NOTE: The automation interfaces are implemented in dsofauto.cpp.
//  The interfaces that follow are largely the interfaces used by the
//  OCX host for normal control containment.
//

////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XInternalUnknown -- Internal IUnknown interface
//     !!! Controls lifetime of CDsoFramerControl !!!
//
//   STDMETHODIMP         QueryInterface(REFIID riid, void ** ppv);
//   STDMETHODIMP_(ULONG) AddRef(void);
//   STDMETHODIMP_(ULONG) Release(void);
//
STDMETHODIMP CDsoFramerControl::XInternalUnknown::QueryInterface(REFIID riid, void** ppv)
{
	ODS("CDsoFramerControl::InternalQueryInterface\n");
	CHECK_NULL_RETURN(ppv, E_POINTER);
	
	HRESULT hr = S_OK;
	METHOD_PROLOGUE(CDsoFramerControl, InternalUnknown);

	if (IID_IUnknown == riid)
	{
		*ppv = (IUnknown*)this;
	}
	else if ((IID_IDispatch == riid) || (IID__FramerControl == riid))
	{
		*ppv = (_FramerControl*)pThis;
	}
	else if (IID_IOleObject == riid)
	{
		*ppv = (IOleObject*)&(pThis->m_xOleObject);
	}
	else if (IID_IOleControl == riid)
	{
		*ppv = (IOleControl*)&(pThis->m_xOleControl);
	}
	else if (IID_IPersistPropertyBag == riid)
	{
		*ppv = (IPersistPropertyBag*)&(pThis->m_xPersistPropertyBag);
	}
	else if ((IID_IPersistStreamInit == riid) || (IID_IPersistStream == riid) || (IID_IPersist == riid))
	{
		*ppv = (IPersistStreamInit*)&(pThis->m_xPersistStreamInit);
	}
	else if ((IID_IOleInPlaceObject == riid) || (IID_IOleWindow == riid))
	{
		*ppv = (IOleInPlaceObject*)&(pThis->m_xOleInplaceObject);
	}	
	else if (IID_IOleInPlaceActiveObject == riid)
	{
		*ppv = (IOleInPlaceActiveObject*)&(pThis->m_xOleInplaceActiveObject);
	}
	else if ((IID_IViewObjectEx == riid) || (IID_IViewObject == riid) || (IID_IViewObject2 == riid))
	{
		*ppv = (IViewObjectEx*)&(pThis->m_xViewObjectEx);
	}	
	else if (IID_ISupportErrorInfo == riid)
	{
		*ppv = (ISupportErrorInfo*)&(pThis->m_xSupportErrorInfo);
	}
	else if (IID_IProvideClassInfo == riid)
	{
		*ppv = (IProvideClassInfo*)&(pThis->m_xProvideClassInfo);
	}
	else if (IID_IConnectionPointContainer == riid)
	{
		*ppv = (IConnectionPointContainer*)&(pThis->m_xConnectionPointContainer);
	}
	else if (IID_IConnectionPoint == riid)
	{
		*ppv = (IConnectionPoint*)&(pThis->m_xConnectionPoint);
	}
	else if (IID_IEnumConnectionPoints == riid)
	{
		*ppv = (IEnumConnectionPoints*)&(pThis->m_xEnumConnectionPoints);
	}
	else if (IID_IObjectSafety == riid)
	{
		*ppv = (IObjectSafety*)&(pThis->m_xObjectSafety);
	}
	else 
	{
		*ppv = NULL;
		hr = E_NOINTERFACE;
	}

	if (NULL != *ppv)
		((IUnknown*)(*ppv))->AddRef();
	return hr;
}

STDMETHODIMP_(ULONG) CDsoFramerControl::XInternalUnknown::AddRef(void)
{
	METHOD_PROLOGUE(CDsoFramerControl, InternalUnknown);
	//TRACE1("CDsoFramerControl::InternalAddRef - %d\n", pThis->m_cRef + 1);
    return ++(pThis->m_cRef);
}

STDMETHODIMP_(ULONG) CDsoFramerControl::XInternalUnknown::Release(void)
{
	METHOD_PROLOGUE(CDsoFramerControl, InternalUnknown);
	//TRACE1("CDsoFramerControl::InternalRelease - %d\n", (pThis->m_cRef) - 1);
    if (0 != --(pThis->m_cRef)) return (pThis->m_cRef);

	ODS("CDsoFramerControl delete\n");
	InterlockedDecrement((LPLONG)&v_cLocks);
	delete pThis; return 0;
}


////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XPersistStreamInit -- IPersistStreamInit Implementation
//
//	 STDMETHODIMP GetClassID(CLSID *pClassID);
//	 STDMETHODIMP IsDirty(void);
//	 STDMETHODIMP Load(LPSTREAM pStm);
//	 STDMETHODIMP Save(LPSTREAM pStm, BOOL fClearDirty);
//	 STDMETHODIMP GetSizeMax(ULARGE_INTEGER* pcbSize);
//	 STDMETHODIMP InitNew(void);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, PersistStreamInit)

STDMETHODIMP CDsoFramerControl::XPersistStreamInit::GetClassID(CLSID *pClassID)
{
	ODS("CDsoFramerControl::XPersistStreamInit::GetClassID\n");
	if (pClassID) *pClassID = CLSID_FramerControl;
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XPersistStreamInit::IsDirty(void)
{
	METHOD_PROLOGUE(CDsoFramerControl, PersistStreamInit);
    ODS("CDsoFramerControl::XPersistStreamInit::IsDirty\n");
    return (pThis->m_fDirty) ? S_OK : S_FALSE;
}


#define STREAMHDR_SIGNATURE 0x1234ABCD  // Signature to identify our format (avoid crashes!)

STDMETHODIMP CDsoFramerControl::XPersistStreamInit::Load(LPSTREAM pStm)
{
    HRESULT hr;
	DWORD dwSig, dwT;

	METHOD_PROLOGUE(CDsoFramerControl, PersistStreamInit);
    ODS("CDsoFramerControl::XPersistStreamInit::Load\n");

 // look for our header structure, so we can verify stream validity.
    hr = pStm->Read(&dwSig, sizeof(DWORD), NULL);
    RETURN_ON_FAILURE(hr);

    if (dwSig != STREAMHDR_SIGNATURE)
        return E_UNEXPECTED;

 // we like the stream.  let's go load in our two properties.
    hr = pStm->Read(&(pThis->m_Size), sizeof(SIZEL), NULL);
    RETURN_ON_FAILURE(hr);

	HimetricToPixels(&(pThis->m_Size.cx), &(pThis->m_Size.cy));

    hr = pStm->Read(&(pThis->m_clrBorderColor), sizeof(OLE_COLOR), NULL);
    RETURN_ON_FAILURE(hr);

    hr = pStm->Read(&(pThis->m_clrBackColor), sizeof(OLE_COLOR), NULL);
    RETURN_ON_FAILURE(hr);

    hr = pStm->Read(&(pThis->m_clrForeColor), sizeof(OLE_COLOR), NULL);
    RETURN_ON_FAILURE(hr);

    hr = pStm->Read(&(pThis->m_clrTBarColor), sizeof(OLE_COLOR), NULL);
    RETURN_ON_FAILURE(hr);

    hr = pStm->Read(&(pThis->m_clrTBarTextColor), sizeof(OLE_COLOR), NULL);
    RETURN_ON_FAILURE(hr);

    hr = pStm->Read(&dwT, sizeof(DWORD), NULL);
    RETURN_ON_FAILURE(hr);

	pThis->m_fBorderStyle = LOWORD(dwT);
	pThis->m_fShowTitlebar = HIBYTE(HIWORD(dwT));
	pThis->m_fShowToolbars = LOBYTE(HIWORD(dwT));

    hr = pStm->Read(&dwT, sizeof(DWORD), NULL);
    RETURN_ON_FAILURE(hr);

	if (dwT)
	{
		LPWSTR pwsz = (LPWSTR)MemAlloc(dwT+2);
		if (pwsz)
		{
			hr = pStm->Read(pwsz, dwT, NULL);
			if (SUCCEEDED(hr))
				pThis->m_bstrCustomCaption = SysAllocString(pwsz);
		}
		MemFree(pwsz);
	}

    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XPersistStreamInit::Save(LPSTREAM pStm, BOOL fClearDirty)
{
    HRESULT hr;
	DWORD   dwT, dwSig = STREAMHDR_SIGNATURE;
	SIZEL   slSize;

	METHOD_PROLOGUE(CDsoFramerControl, PersistStreamInit);
    ODS("CDsoFramerControl::XPersistStreamInit::Save\n");

 // first thing to do is write out our stream sig...
    hr = pStm->Write(&dwSig, sizeof(DWORD), NULL);
    RETURN_ON_FAILURE(hr);

 // the only properties we're currently persisting here are the size
 // properties for this control.  make sure we do that in HiMetric
	slSize = pThis->m_Size;
	PixelsToHimetric(&(slSize.cx), &(slSize.cy));

    hr = pStm->Write(&slSize, sizeof(SIZEL), NULL);
    RETURN_ON_FAILURE(hr);

    hr = pStm->Write(&(pThis->m_clrBorderColor), sizeof(OLE_COLOR), NULL);
    RETURN_ON_FAILURE(hr);

    hr = pStm->Write(&(pThis->m_clrBackColor), sizeof(OLE_COLOR), NULL);
    RETURN_ON_FAILURE(hr);

    hr = pStm->Write(&(pThis->m_clrForeColor), sizeof(OLE_COLOR), NULL);
    RETURN_ON_FAILURE(hr);

    hr = pStm->Write(&(pThis->m_clrTBarColor), sizeof(OLE_COLOR), NULL);
    RETURN_ON_FAILURE(hr);

    hr = pStm->Write(&(pThis->m_clrTBarTextColor), sizeof(OLE_COLOR), NULL);
    RETURN_ON_FAILURE(hr);

	dwT = (DWORD)MAKELONG((WORD)(pThis->m_fBorderStyle), 
		MAKEWORD((BYTE)(pThis->m_fShowToolbars), (BYTE)(pThis->m_fShowTitlebar)));

    hr = pStm->Write(&dwT, sizeof(DWORD), NULL);
    RETURN_ON_FAILURE(hr);

	dwT = 0;
	if (pThis->m_bstrCustomCaption)
		dwT = SysStringByteLen(pThis->m_bstrCustomCaption);

	hr = pStm->Write(&dwT, sizeof(DWORD), NULL);
	RETURN_ON_FAILURE(hr);

	if (dwT)
	{
		hr = pStm->Write(pThis->m_bstrCustomCaption, dwT, NULL);
		RETURN_ON_FAILURE(hr);
	}

 // clear out dirty flag and notify that we're done with save.
    if (fClearDirty)
        pThis->m_fDirty = FALSE;

    if (pThis->m_pOleAdviseHolder)
        pThis->m_pOleAdviseHolder->SendOnSave();

    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XPersistStreamInit::GetSizeMax(ULARGE_INTEGER *pcbSize)
{
    ODS("CDsoFramerControl::XPersistStreamInit::GetSizeMax\n");
    return E_NOTIMPL;
}

STDMETHODIMP CDsoFramerControl::XPersistStreamInit::InitNew(void)
{
    METHOD_PROLOGUE(CDsoFramerControl, PersistStreamInit);
    ODS("CDsoFramerControl::XPersistStreamInit::InitNew\n");
 // If we are new control, we are dirty by default...
	pThis->m_fDirty	= TRUE;  
    return S_OK;
}

////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XPersistPropertyBag -- IPersistPropertyBag Implementation
//
//	 STDMETHODIMP GetClassID(CLSID *pClassID);
//	 STDMETHODIMP InitNew();
//	 STDMETHODIMP Load(IPropertyBag* pPropBag, IErrorLog* pErrorLog);
//	 STDMETHODIMP Save(IPropertyBag* pPropBag, BOOL fClearDirty, BOOL fSaveAllProperties);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, PersistPropertyBag)

STDMETHODIMP CDsoFramerControl::XPersistPropertyBag::GetClassID(CLSID *pClassID)
{
    ODS("CDsoFramerControl::XPersistPropertyBag::GetClassID\n");
	if (pClassID) *pClassID = CLSID_FramerControl;
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XPersistPropertyBag::InitNew()
{
	METHOD_PROLOGUE(CDsoFramerControl, PersistPropertyBag);
    ODS("CDsoFramerControl::XPersistPropertyBag::InitNew\n");
	return pThis->m_xPersistStreamInit.InitNew();
}

STDMETHODIMP CDsoFramerControl::XPersistPropertyBag::Load(IPropertyBag* pPropBag, IErrorLog* pErrorLog)
{
    VARIANT v;
    HRESULT hr;
	SIZEL sl = {5000, 3000};

	METHOD_PROLOGUE(CDsoFramerControl, PersistPropertyBag);
    ODS("CDsoFramerControl::XPersistPropertyBag::Load\n");

    v.vt = VT_I4;
	v.lVal = 0;

    hr = pPropBag->Read(L"_ExtentX", &v, pErrorLog);
    if (SUCCEEDED(hr)){sl.cx = v.lVal;}
    
    v.lVal = 0;
    hr = pPropBag->Read(L"_ExtentY", &v, pErrorLog);
    if (SUCCEEDED(hr)){sl.cy = v.lVal;}

	HimetricToPixels(&(sl.cx), &(sl.cy));
	pThis->m_Size = sl;

    hr = pPropBag->Read(L"Titlebar", &v, pErrorLog);
	if (SUCCEEDED(hr)){pThis->m_fShowTitlebar = v.boolVal;}

    hr = pPropBag->Read(L"Toolbars", &v, pErrorLog);
	if (SUCCEEDED(hr)){pThis->m_fShowToolbars = v.boolVal;}

    hr = pPropBag->Read(L"BorderStyle", &v, pErrorLog);
	if (SUCCEEDED(hr)){pThis->m_fBorderStyle = v.lVal;}

    hr = pPropBag->Read(L"BorderColor", &v, pErrorLog);
	if (SUCCEEDED(hr)){pThis->m_clrBorderColor = v.lVal;}

    hr = pPropBag->Read(L"BackColor", &v, pErrorLog);
	if (SUCCEEDED(hr)){pThis->m_clrBackColor = v.lVal;}

    hr = pPropBag->Read(L"ForeColor", &v, pErrorLog);
	if (SUCCEEDED(hr)){pThis->m_clrForeColor = v.lVal;}

    hr = pPropBag->Read(L"TitlebarColor", &v, pErrorLog);
	if (SUCCEEDED(hr)){pThis->m_clrTBarColor = v.lVal;}

    hr = pPropBag->Read(L"TitlebarTextColor", &v, pErrorLog);
	if (SUCCEEDED(hr)){pThis->m_clrTBarTextColor = v.lVal;}

    hr = pPropBag->Read(L"Caption", &v, pErrorLog);
	if (SUCCEEDED(hr))
	{
		LPWSTR pwsz = LPWSTR_FROM_VARIANT(v);
		if (pwsz) pThis->m_bstrCustomCaption = SysAllocString(pwsz);
		VariantClear(&v);
	}

    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XPersistPropertyBag::Save(IPropertyBag* pPropBag, BOOL fClearDirty, BOOL fSaveAllProperties)
{
    HRESULT hr;
    VARIANT v;
    SIZEL   sl;

	METHOD_PROLOGUE(CDsoFramerControl, PersistPropertyBag);
    ODS("CDsoFramerControl::XPersistPropertyBag::Save\n");

	sl = pThis->m_Size;
    PixelsToHimetric(&(sl.cx), &(sl.cy));

    v.vt = VT_I4; v.lVal = sl.cx;
    hr = pPropBag->Write(L"_ExtentX", &v);
    RETURN_ON_FAILURE(hr);

    v.lVal = sl.cy;
    hr = pPropBag->Write(L"_ExtentY", &v);
    RETURN_ON_FAILURE(hr);

    v.lVal = pThis->m_clrBorderColor;
    hr = pPropBag->Write(L"BorderColor", &v);
    RETURN_ON_FAILURE(hr);

    v.lVal = pThis->m_clrBackColor;
    hr = pPropBag->Write(L"BackColor", &v);
    RETURN_ON_FAILURE(hr);

    v.lVal = pThis->m_clrForeColor;
    hr = pPropBag->Write(L"ForeColor", &v);
    RETURN_ON_FAILURE(hr);

    v.lVal = pThis->m_clrTBarColor;
    hr = pPropBag->Write(L"TitlebarColor", &v);
    RETURN_ON_FAILURE(hr);

    v.lVal = pThis->m_clrTBarTextColor;
    hr = pPropBag->Write(L"TitlebarTextColor", &v);
    RETURN_ON_FAILURE(hr);

    v.lVal = pThis->m_fBorderStyle;
    hr = pPropBag->Write(L"BorderStyle", &v);
    RETURN_ON_FAILURE(hr);

    v.vt = VT_BOOL;
	v.boolVal = pThis->m_fShowTitlebar;
    hr = pPropBag->Write(L"Titlebar", &v);
    RETURN_ON_FAILURE(hr);

	v.boolVal = pThis->m_fShowToolbars;
    hr = pPropBag->Write(L"Toolbars", &v);
    RETURN_ON_FAILURE(hr);

	if (pThis->m_bstrCustomCaption)
	{
		v.vt = VT_BSTR;
		v.bstrVal = pThis->m_bstrCustomCaption;
		hr = pPropBag->Write(L"Toolbars", &v);
		RETURN_ON_FAILURE(hr);
	}
	
 // now clear the dirty flag and send out notification 
 // that we're done.  
    if (fClearDirty)
        pThis->m_fDirty = FALSE;

    if (pThis->m_pOleAdviseHolder)
        pThis->m_pOleAdviseHolder->SendOnSave();

    return S_OK;
}

////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XOleObject -- IOleObject Implementation
//
//	 STDMETHODIMP SetClientSite(IOleClientSite *pClientSite);
//	 STDMETHODIMP GetClientSite(IOleClientSite **ppClientSite);
//	 STDMETHODIMP SetHostNames(LPCOLESTR szContainerApp, LPCOLESTR szContainerObj);
//	 STDMETHODIMP Close(DWORD dwSaveOption);
//	 STDMETHODIMP SetMoniker(DWORD dwWhichMoniker, IMoniker *pmk);
//	 STDMETHODIMP GetMoniker(DWORD dwAssign, DWORD dwWhichMoniker, IMoniker **ppmk);
//	 STDMETHODIMP InitFromData(IDataObject *pDataObject, BOOL fCreation, DWORD dwReserved);
//	 STDMETHODIMP GetClipboardData(DWORD dwReserved, IDataObject **ppDataObject);
//	 STDMETHODIMP DoVerb(LONG iVerb, LPMSG lpmsg, IOleClientSite *pActiveSite, LONG lindex, HWND hwndParent, LPCRECT lprcPosRect);
//	 STDMETHODIMP EnumVerbs(IEnumOleVerb **ppEnumOleVerb);
//	 STDMETHODIMP Update();
//	 STDMETHODIMP IsUpToDate();
//	 STDMETHODIMP GetUserClassID(CLSID *pClsid);
//	 STDMETHODIMP GetUserType(DWORD dwFormOfType, LPOLESTR *pszUserType);
//	 STDMETHODIMP SetExtent(DWORD dwDrawAspect, SIZEL *psizel);
//	 STDMETHODIMP GetExtent(DWORD dwDrawAspect, SIZEL *psizel);
//	 STDMETHODIMP Advise(IAdviseSink *pAdvSink, DWORD *pdwConnection);
//	 STDMETHODIMP Unadvise(DWORD dwConnection);
//	 STDMETHODIMP EnumAdvise(IEnumSTATDATA **ppenumAdvise);
//	 STDMETHODIMP GetMiscStatus(DWORD dwAspect, DWORD *pdwStatus);
//	 STDMETHODIMP SetColorScheme(LOGPALETTE *pLogpal);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, OleObject)

STDMETHODIMP CDsoFramerControl::XOleObject::SetClientSite(IOleClientSite *pClientSite)
{

	METHOD_PROLOGUE(CDsoFramerControl, OleObject);
    ODS("CDsoFramerControl::XOleObject::SetClientSite\n");

    RELEASE_INTERFACE(pThis->m_pClientSite);
    RELEASE_INTERFACE(pThis->m_pControlSite);

 // store away the new client site
    pThis->m_pClientSite = pClientSite;

  // if we've actually got one, then get some other interfaces we want to keep
  // around, and keep a handle on it
    if (pClientSite)
	{
        pClientSite->AddRef();
        pClientSite->QueryInterface(IID_IOleControlSite, (void **)&(pThis->m_pControlSite));
    }
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleObject::GetClientSite(IOleClientSite **ppClientSite)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleObject);
    ODS("CDsoFramerControl::XOleObject::GetClientSite\n");
	if ((ppClientSite) && (pThis->m_pClientSite))
	{
		(*ppClientSite = pThis->m_pClientSite)->AddRef();
	}
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleObject::SetHostNames(LPCOLESTR /*szContainerApp*/, LPCOLESTR /*szContainerObj*/)
{
    ODS("CDsoFramerControl::XOleObject::SetHostNames\n");
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleObject::Close(DWORD dwSaveOption)
{
	HRESULT hr;

	METHOD_PROLOGUE(CDsoFramerControl, OleObject);
    ODS("CDsoFramerControl::XOleObject::Close\n");

    if (pThis->m_fInPlaceActive)
	{
        hr = pThis->m_xOleInplaceObject.InPlaceDeactivate();
        RETURN_ON_FAILURE(hr);
    }

 // handle the save flag.
    if ((pThis->m_fDirty) && (pThis->m_pClientSite) &&
		(dwSaveOption == OLECLOSE_SAVEIFDIRTY || 
		 dwSaveOption == OLECLOSE_PROMPTSAVE))
	{
		pThis->m_pClientSite->SaveObject();
    }

    if (pThis->m_pOleAdviseHolder)
        pThis->m_pOleAdviseHolder->SendOnClose();

    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleObject::SetMoniker(DWORD dwWhichMoniker, IMoniker *pmk)
{
    ODS("CDsoFramerControl::XOleObject::SetMoniker\n");
    return E_NOTIMPL;
}

STDMETHODIMP CDsoFramerControl::XOleObject::GetMoniker(DWORD dwAssign, DWORD dwWhichMoniker, IMoniker **ppmk)
{
    ODS("CDsoFramerControl::XOleObject::GetMoniker\n");
	if (ppmk) *ppmk = NULL;
    return E_NOTIMPL;
}

STDMETHODIMP CDsoFramerControl::XOleObject::InitFromData(IDataObject *pDataObject, BOOL fCreation, DWORD dwReserved)
{
    ODS("CDsoFramerControl::XOleObject::InitFromData\n");
    return E_NOTIMPL;
}

STDMETHODIMP CDsoFramerControl::XOleObject::GetClipboardData(DWORD /*dwReserved*/, IDataObject **ppDataObject)
{
    ODS("CDsoFramerControl::XOleObject::GetClipboardData\n");
	if (ppDataObject) *ppDataObject = NULL;
    return E_NOTIMPL;
}

STDMETHODIMP CDsoFramerControl::XOleObject::DoVerb(LONG iVerb, LPMSG lpmsg, IOleClientSite *pActiveSite, LONG lindex, HWND hwndParent, LPCRECT lprcPosRect)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleObject);
    ODS("CDsoFramerControl::XOleObject::DoVerb\n");

    switch (iVerb)
	{
    case OLEIVERB_PRIMARY:
    case OLEIVERB_SHOW:
    case OLEIVERB_INPLACEACTIVATE:
    case OLEIVERB_UIACTIVATE:
        return pThis->InPlaceActivate(iVerb);

    case OLEIVERB_HIDE:
        pThis->m_xOleInplaceObject.UIDeactivate();
        if (pThis->m_fInPlaceVisible)
            pThis->SetInPlaceVisible(FALSE);
        return S_OK;
    }

    return OLEOBJ_S_INVALIDVERB;
}

STDMETHODIMP CDsoFramerControl::XOleObject::EnumVerbs(IEnumOLEVERB **ppEnumOleVerb)
{
    ODS("CDsoFramerControl::XOleObject::EnumVerbs\n");
	return OleRegEnumVerbs(CLSID_FramerControl, ppEnumOleVerb);
}

STDMETHODIMP CDsoFramerControl::XOleObject::Update()
{
    ODS("CDsoFramerControl::XOleObject::Update\n");
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleObject::IsUpToDate()
{
    ODS("CDsoFramerControl::XOleObject::IsUpToDate\n");
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleObject::GetUserClassID(CLSID *pClsid)
{
    ODS("CDsoFramerControl::XOleObject::GetUserClassID\n");
	if (pClsid) *pClsid = CLSID_FramerControl;
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleObject::GetUserType(DWORD dwFormOfType, LPOLESTR *pszUserType)
{
    ODS("CDsoFramerControl::XOleObject::GetUserType\n");
    return OleRegGetUserType(CLSID_FramerControl, dwFormOfType, pszUserType);
}

STDMETHODIMP CDsoFramerControl::XOleObject::SetExtent(DWORD dwDrawAspect, SIZEL *psizel)
{
    SIZEL sl;

	METHOD_PROLOGUE(CDsoFramerControl, OleObject);
    ODS("CDsoFramerControl::XOleObject::SetExtent\n");

	CHECK_NULL_RETURN(psizel, E_POINTER);
	sl = *psizel;

 // we don't support any other aspects.
    if (!(dwDrawAspect & DVASPECT_CONTENT))
        return DV_E_DVASPECT;

 // change the units to pixels, and resize the control.
    HimetricToPixels(&(sl.cx), &(sl.cy));

 // if the size already matches the extents, then don't do anything
 // and don't mark the control dirty.
	if (sl.cx != pThis->m_Size.cx || sl.cy != pThis->m_Size.cy)
	{
		pThis->m_Size = sl;
		pThis->m_fDirty = TRUE; // changing size should dirty

		if (pThis->m_fInPlaceActive)
		{
			RECT rect;  

		 // Use OnPosRectChange for older OCX containters and Access
		 // who just refuses to get with the times (like me <g>)...
			GetWindowRect(pThis->m_hwnd, &rect);
			MapWindowPoints(NULL, pThis->m_hwndParent, (LPPOINT)&rect, 2);
			rect.right = rect.left + sl.cx;
			rect.bottom = rect.top + sl.cy;
			pThis->m_pInPlaceSite->OnPosRectChange(&rect);

		 // Resize the window based on the new size...
			SetWindowPos(pThis->m_hwnd, NULL, 0, 0, sl.cx, sl.cy,
					SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE);
		}
		else pThis->ViewChanged();
	}
    
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleObject::GetExtent(DWORD dwDrawAspect, SIZEL *psizel)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleObject);
    ODS("CDsoFramerControl::XOleObject::GetExtent\n");

	CHECK_NULL_RETURN(psizel, E_POINTER);

    if (!(dwDrawAspect & DVASPECT_CONTENT))
        return DV_E_DVASPECT;

	*psizel = pThis->m_Size;
	PixelsToHimetric(&(psizel->cx), &(psizel->cy));
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleObject::Advise(IAdviseSink *pAdvSink, DWORD *pdwConnection)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleObject);
    ODS("CDsoFramerControl::XOleObject::Advise\n");

	if (NULL == pThis->m_pOleAdviseHolder)
	{
		if (FAILED(CreateOleAdviseHolder(&(pThis->m_pOleAdviseHolder))))
			return E_FAIL;
	}
	return pThis->m_pOleAdviseHolder->Advise(pAdvSink, pdwConnection);
}

STDMETHODIMP CDsoFramerControl::XOleObject::Unadvise(DWORD dwConnection)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleObject);
    ODS("CDsoFramerControl::XOleObject::Unadvise\n");

	CHECK_NULL_RETURN(pThis->m_pOleAdviseHolder, CONNECT_E_NOCONNECTION);
    return pThis->m_pOleAdviseHolder->Unadvise(dwConnection);
}

STDMETHODIMP CDsoFramerControl::XOleObject::EnumAdvise(IEnumSTATDATA **ppenumAdvise)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleObject);
    ODS("CDsoFramerControl::XOleObject::EnumAdvise\n");

	CHECK_NULL_RETURN(ppenumAdvise, E_POINTER);
	*ppenumAdvise = NULL;

	CHECK_NULL_RETURN(pThis->m_pOleAdviseHolder, E_FAIL);
    return pThis->m_pOleAdviseHolder->EnumAdvise(ppenumAdvise);
}

STDMETHODIMP CDsoFramerControl::XOleObject::GetMiscStatus(DWORD dwAspect, DWORD *pdwStatus)
{
    ODS("CDsoFramerControl::XOleObject::GetMiscStatus\n");

    if (dwAspect != DVASPECT_CONTENT)
        return DV_E_DVASPECT;
    
    if (pdwStatus)
		*pdwStatus = 	OLEMISC_SETCLIENTSITEFIRST |
						OLEMISC_ACTIVATEWHENVISIBLE | 
						OLEMISC_RECOMPOSEONRESIZE |
						OLEMISC_CANTLINKINSIDE | 
						OLEMISC_INSIDEOUT;
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleObject::SetColorScheme(LOGPALETTE *pLogpal)
{
    ODS("CDsoFramerControl::XOleObject::SetColorScheme\n");
    return S_OK;
}

////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XOleControl -- IOleControl Implementation
//
//	 STDMETHODIMP GetControlInfo(CONTROLINFO* pCI);
//	 STDMETHODIMP OnMnemonic(LPMSG pMsg);
//	 STDMETHODIMP OnAmbientPropertyChange(DISPID dispID);
//	 STDMETHODIMP FreezeEvents(BOOL bFreeze);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, OleControl)

STDMETHODIMP CDsoFramerControl::XOleControl::GetControlInfo(CONTROLINFO* pCI)
{
	ODS("CDsoFramerControl::XOleControl::GetControlInfo\n");
	CHECK_NULL_RETURN(pCI, E_POINTER);
	pCI->cAccel = 0;
	pCI->hAccel = NULL;
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleControl::OnMnemonic(LPMSG pMsg)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleControl);
	ODS("CDsoFramerControl::XOleControl::OnMnemonic\n");

	return pThis->InPlaceActivate(OLEIVERB_UIACTIVATE);
}

STDMETHODIMP CDsoFramerControl::XOleControl::OnAmbientPropertyChange(DISPID dispID)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleControl);
	ODS("CDsoFramerControl::XOleControl::OnAmbientPropertyChange\n");

 // We keep track of state changes in design/run modes...
    if (dispID == DISPID_AMBIENT_USERMODE || dispID == DISPID_UNKNOWN)
        pThis->m_fModeFlagValid = FALSE;

	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleControl::FreezeEvents(BOOL bFreeze)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleControl);
	TRACE1("CDsoFramerControl::XOleControl::FreezeEvents = %s\n", (bFreeze ? "TRUE" : "FALSE"));
    pThis->m_fFreezeEvents = bFreeze;
	return S_OK;
}


////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XOleInplaceObject -- IOleInplaceObject Implementation
//
//	 STDMETHODIMP GetWindow(HWND *phwnd);
//	 STDMETHODIMP ContextSensitiveHelp(BOOL fEnterMode);
//	 STDMETHODIMP InPlaceDeactivate();
//	 STDMETHODIMP UIDeactivate();
//	 STDMETHODIMP SetObjectRects(LPCRECT lprcPosRect, LPCRECT lprcClipRect);
//	 STDMETHODIMP ReactivateAndUndo();
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, OleInplaceObject)

STDMETHODIMP CDsoFramerControl::XOleInplaceObject::GetWindow(HWND *phwnd)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleInplaceObject);
	ODS("CDsoFramerControl::XOleInplaceObject::GetWindow\n");
	if (!(pThis->m_fInPlaceActive)) return E_FAIL;
	if (phwnd) *phwnd = pThis->m_hwnd;
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleInplaceObject::ContextSensitiveHelp(BOOL /*fEnterMode*/)
{
	ODS("CDsoFramerControl::XOleInplaceObject::ContextSensitiveHelp\n");
    return E_NOTIMPL;
}

STDMETHODIMP CDsoFramerControl::XOleInplaceObject::InPlaceDeactivate()
{
	METHOD_PROLOGUE(CDsoFramerControl, OleInplaceObject);
	ODS("CDsoFramerControl::XOleInplaceObject::InPlaceDeactivate\n");

 // if we're not in-place active yet, then this is easy.
    if (!(pThis->m_fInPlaceActive))
        return S_OK;

 // transition from UIActive back to active
    if (pThis->m_fUIActive)
        UIDeactivate();

 // tell the host we're going away
    pThis->m_pInPlaceSite->OnInPlaceDeactivate();

    pThis->m_fInPlaceActive = FALSE;
    pThis->m_fInPlaceVisible = FALSE;

 // if we have a window, tell it to go away.
    if (pThis->m_hwnd)
		{DestroyWindow(pThis->m_hwnd); pThis->m_hwnd = NULL;}
    
    RELEASE_INTERFACE(pThis->m_pInPlaceFrame);
    RELEASE_INTERFACE(pThis->m_pInPlaceUIWindow);

    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleInplaceObject::UIDeactivate()
{
	METHOD_PROLOGUE(CDsoFramerControl, OleInplaceObject);
	ODS("CDsoFramerControl::XOleInplaceObject::UIDeactivate\n");

 // if we're not UIActive, not much to do.
    if (!(pThis->m_fUIActive))
        return S_OK;

    pThis->m_fUIActive = FALSE;

 // notify frame windows, if appropriate, that we're no longer ui-active.
    if (pThis->m_pInPlaceUIWindow)
		pThis->m_pInPlaceUIWindow->SetActiveObject(NULL, NULL);

    if (pThis->m_pInPlaceFrame)
		pThis->m_pInPlaceFrame->SetActiveObject(NULL, NULL);

 // we don't need to explicitly release the focus here since somebody
 // else grabbing the focus is what is likely to cause us to get lose it
    pThis->m_pInPlaceSite->OnUIDeactivate(FALSE);

    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleInplaceObject::SetObjectRects(LPCRECT lprcPosRect, LPCRECT lprcClipRect)
{
    RECT rcIXect;

	METHOD_PROLOGUE(CDsoFramerControl, OleInplaceObject);
	ODS("CDsoFramerControl::XOleInplaceObject::SetObjectRects\n");

  // move our window to the new location and handle clipping.
    if (pThis->m_hwnd)
	{
	 // if asked to clip the window, we'll use a clip region...
		if ((lprcClipRect) && (IntersectRect(&rcIXect, lprcPosRect, lprcClipRect)) &&
			(!EqualRect(&rcIXect, lprcPosRect)))
		{
			OffsetRect(&rcIXect, -(lprcPosRect->left), -(lprcPosRect->top));
			SetWindowRgn(pThis->m_hwnd, CreateRectRgnIndirect(&rcIXect), TRUE);
			pThis->m_fUsingWindowRgn = TRUE;
		}
		else if (pThis->m_fUsingWindowRgn)
		{
            SetWindowRgn(pThis->m_hwnd, NULL, TRUE);
            pThis->m_fUsingWindowRgn = FALSE;
		}

        // set our control's location, but don't change it's size at all
        // [people for whom zooming is important should set that up here]
        SetWindowPos(pThis->m_hwnd, NULL, lprcPosRect->left, lprcPosRect->top, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
    }

 // save out our current location.
    pThis->m_rcLocation = *lprcPosRect;
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleInplaceObject::ReactivateAndUndo()
{
	ODS("CDsoFramerControl::XOleInplaceObject::ReactivateAndUndo\n");
	return E_NOTIMPL;
}


////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XOleInplaceActiveObject -- IOleInplaceActiveObject Implementation
//
//	 STDMETHODIMP GetWindow(HWND *phwnd);
//	 STDMETHODIMP ContextSensitiveHelp(BOOL fEnterMode);
//	 STDMETHODIMP TranslateAccelerator(LPMSG lpmsg);
//	 STDMETHODIMP OnFrameWindowActivate(BOOL fActivate);
//	 STDMETHODIMP OnDocWindowActivate(BOOL fActivate);
//	 STDMETHODIMP ResizeBorder(LPCRECT prcBorder, IOleInPlaceUIWindow *pUIWindow, BOOL fFrameWindow);
//	 STDMETHODIMP EnableModeless(BOOL fEnable);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, OleInplaceActiveObject)

STDMETHODIMP CDsoFramerControl::XOleInplaceActiveObject::GetWindow(HWND *phwnd)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleInplaceActiveObject);
	ODS("CDsoFramerControl::XOleInplaceActiveObject::GetWindow\n");
	if (phwnd) *phwnd = pThis->m_hwnd;
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleInplaceActiveObject::ContextSensitiveHelp(BOOL /*fEnterMode*/)
{
	ODS("CDsoFramerControl::XOleInplaceActiveObject::ContextSensitiveHelp\n");
    return E_NOTIMPL;
}

static short _SpecialKeyState()
{
    BOOL bShift = (GetKeyState(VK_SHIFT) < 0);
    BOOL bCtrl  = (GetKeyState(VK_CONTROL) < 0);
    BOOL bAlt   = (GetKeyState(VK_MENU) < 0);
    return (short)(bShift + (bCtrl << 1) + (bAlt << 2));
}

STDMETHODIMP CDsoFramerControl::XOleInplaceActiveObject::TranslateAccelerator(LPMSG lpmsg)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleInplaceActiveObject);
	ODS("CDsoFramerControl::XOleInplaceActiveObject::TranslateAccelerator\n");

 // forward it back to the site for further processing
    if (pThis->m_pControlSite)
        return pThis->m_pControlSite->TranslateAccelerator(lpmsg, _SpecialKeyState());

 // we didn't want it.
    return S_FALSE;
}

STDMETHODIMP CDsoFramerControl::XOleInplaceActiveObject::OnFrameWindowActivate(BOOL fActivate)
{
	TRACE1("CDsoFramerControl::XOleInplaceActiveObject::OnFrameWindowActivate - %d\n", fActivate);
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleInplaceActiveObject::OnDocWindowActivate(BOOL fActivate)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleInplaceActiveObject);
	TRACE1("CDsoFramerControl::XOleInplaceActiveObject::OnDocWindowActivate - %d\n", fActivate);
    
	if (pThis->m_fUIActive && fActivate && pThis->m_pInPlaceFrame)
        pThis->m_pInPlaceFrame->SetBorderSpace(NULL);

	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleInplaceActiveObject::ResizeBorder(LPCRECT prcBorder, IOleInPlaceUIWindow *pUIWindow, BOOL fFrameWindow)
{
	ODS("CDsoFramerControl::XOleInplaceActiveObject::ResizeBorder\n");
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleInplaceActiveObject::EnableModeless(BOOL fEnable)
{
	ODS("CDsoFramerControl::XOleInplaceActiveObject::EnableModeless\n");
	return S_OK;
}

////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XViewObjectEx -- IViewObjectEx Implementation
//
//	 STDMETHODIMP Draw(DWORD dwAspect, LONG lindex, void *pvAspect, DVTARGETDEVICE * ptd, HDC hicTargetDev, HDC hdcDraw, const LPRECTL lprcBounds, const LPRECTL lprcWBounds, BOOL(*)(DWORD)pfnContinue, DWORD dwContinue); 
//	 STDMETHODIMP GetColorSet(DWORD dwAspect, LONG lindex, void* pvAspect, DVTARGETDEVICE *ptd, HDC hicTargetDev, LOGPALETTE** ppColorSet);
//	 STDMETHODIMP Freeze(DWORD dwAspect, LONG lindex, void* pvAspect, DWORD* pdwFreeze);
//	 STDMETHODIMP Unfreeze(DWORD dwFreeze);
//	 STDMETHODIMP SetAdvise(DWORD dwAspect, DWORD advf, IAdviseSink* pAdvSink);
//	 STDMETHODIMP GetAdvise(DWORD* pdwAspect, DWORD* padvf, IAdviseSink** ppAdvSink);
//	 STDMETHODIMP GetExtent(DWORD dwAspect, DWORD lindex, DVTARGETDEVICE ptd, LPSIZEL lpsizel);
//	 STDMETHODIMP GetRect(DWORD dwAspect, LPRECTL pRect);
//	 STDMETHODIMP GetViewStatus(DWORD* pdwStatus);
//	 STDMETHODIMP QueryHitPoint(DWORD dwAspect, LPCRECT pRectBounds, POINT ptlLoc, LONG lCloseHint, DWORD *pHitResult);
//	 STDMETHODIMP QueryHitRect(DWORD dwAspect, LPCRECT pRectBounds, LPCRECT pRectLoc, LONG lCloseHint, DWORD *pHitResult);
//	 STDMETHODIMP GetNaturalExtent(DWORD dwAspect, LONG lindex, DVTARGETDEVICE *ptd, HDC hicTargetDev, DVEXTENTINFO *pExtentInfo, LPSIZEL pSizel);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, ViewObjectEx)

STDMETHODIMP CDsoFramerControl::XViewObjectEx::Draw(DWORD dwDrawAspect, LONG lIndex, void *pvAspect, DVTARGETDEVICE *ptd, HDC hicTargetDevice, HDC hdcDraw, LPCRECTL prcBounds, LPCRECTL prcWBounds, BOOL (__stdcall *pfnContinue)(DWORD dwContinue), DWORD dwContinue)
{
    RECT rc;
    int iMode = 0;
    POINT pVp = {0,0};
	POINT pW = {0,0};
    BOOL fOptimize = FALSE;
    BOOL fMetafile = FALSE;

	METHOD_PROLOGUE(CDsoFramerControl, ViewObjectEx);
	ODS("CDsoFramerControl::XViewObjectEx::Draw\n");

	if (!((dwDrawAspect == DVASPECT_CONTENT) || 
		  (dwDrawAspect == DVASPECT_OPAQUE)))
            return DV_E_DVASPECT;

	SaveDC(hdcDraw);
	
 // first, have to do a little bit to support printing.
    if (GetDeviceCaps(hdcDraw, TECHNOLOGY) == DT_METAFILE)
	{

     // We are dealing with a metafile.
        fMetafile = TRUE;
    }

  // check to see if we have any flags passed in the pvAspect parameter.
    if (pvAspect && ((DVASPECTINFO *)pvAspect)->cb == sizeof(DVASPECTINFO))
        fOptimize = (((DVASPECTINFO *)pvAspect)->dwFlags & DVASPECTINFOFLAG_CANOPTIMIZE) ? TRUE : FALSE;

  // Get the draw rect, if they didn't give us one, just copy over ours
	memcpy(&rc, ((prcBounds) ? (RECT*)prcBounds : &(pThis->m_rcLocation)), sizeof(RECT));

    if (!fMetafile)
	{
        LPtoDP(hdcDraw, (POINT*)&rc, 2);
        SetViewportOrgEx(hdcDraw, 0, 0, &pVp);
        SetWindowOrgEx(hdcDraw, 0, 0, &pW);
        iMode = SetMapMode(hdcDraw, MM_TEXT);
    }

  // prcWBounds is NULL and not used if we are not dealing with a metafile.
  // For metafiles, we pass on rc as *prcBounds, we should also include prcWBounds
    pThis->OnDraw(dwDrawAspect, hdcDraw, &rc, (RECT*)prcWBounds, hicTargetDevice, fOptimize);

  // clean up the DC when we're done with it, if appropriate.
    if (!fMetafile)
	{
        SetViewportOrgEx(hdcDraw, pVp.x, pVp.y, NULL);
        SetWindowOrgEx(hdcDraw, pW.x, pW.y, NULL);
        SetMapMode(hdcDraw, iMode);
    }

	RestoreDC(hdcDraw, -1);
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::GetColorSet(DWORD dwAspect, LONG lindex, void* pvAspect, DVTARGETDEVICE *ptd, HDC hicTargetDev, LOGPALETTE** ppColorSet)
{
	ODS("CDsoFramerControl::XViewObjectEx::GetColorSet\n");
    return E_NOTIMPL;
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::Freeze(DWORD dwAspect, LONG lindex, void* pvAspect, DWORD* pdwFreeze)
{
	ODS("CDsoFramerControl::XViewObjectEx::Freeze\n");
    return E_NOTIMPL;
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::Unfreeze(DWORD dwFreeze)
{
	ODS("CDsoFramerControl::XViewObjectEx::Unfreeze\n");
    return E_NOTIMPL;
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::SetAdvise(DWORD dwAspect, DWORD advf, IAdviseSink* pAdviseSink)
{
	METHOD_PROLOGUE(CDsoFramerControl, ViewObjectEx);
	ODS("CDsoFramerControl::XViewObjectEx::SetAdvise\n");

 // if it's not a content aspect, we don't support it.
    if (!(dwAspect & DVASPECT_CONTENT)) 
        return DV_E_DVASPECT;

 // set up some flags  [we gotta stash for GetAdvise ...]
    pThis->m_fViewAdvisePrimeFirst = (advf & ADVF_PRIMEFIRST) ? TRUE : FALSE;
    pThis->m_fViewAdviseOnlyOnce = (advf & ADVF_ONLYONCE) ? TRUE : FALSE;

    RELEASE_INTERFACE(pThis->m_pViewAdviseSink);

    pThis->m_pViewAdviseSink = pAdviseSink;
	if (pThis->m_pViewAdviseSink)
		(pThis->m_pViewAdviseSink)->AddRef();

 // prime them if they want it [we need to store this so they can get flags later]
    if (pThis->m_fViewAdvisePrimeFirst)
        pThis->ViewChanged();

    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::GetAdvise(DWORD* pdwAspect, DWORD* padvf, IAdviseSink** ppAdviseSink)
{
	METHOD_PROLOGUE(CDsoFramerControl, ViewObjectEx);
	ODS("CDsoFramerControl::XViewObjectEx::GetAdvise\n");

    if (pdwAspect)
        *pdwAspect = DVASPECT_CONTENT;

    if (padvf)
	{
        *padvf = 0;
        if (pThis->m_fViewAdviseOnlyOnce) *padvf |= ADVF_ONLYONCE;
        if (pThis->m_fViewAdvisePrimeFirst) *padvf |= ADVF_PRIMEFIRST;
    }

    if (ppAdviseSink)
	{
        *ppAdviseSink = pThis->m_pViewAdviseSink;
		if (*ppAdviseSink) (*ppAdviseSink)->AddRef();
    }
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::GetExtent(DWORD dwDrawAspect, LONG lindex, DVTARGETDEVICE *ptd, LPSIZEL psizel)
{
	METHOD_PROLOGUE(CDsoFramerControl, ViewObjectEx);
	ODS("CDsoFramerControl::XViewObjectEx::GetExtent\n");
	return pThis->m_xOleObject.GetExtent(dwDrawAspect, psizel);
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::GetRect(DWORD dwAspect, LPRECTL pRect)
{
	METHOD_PROLOGUE(CDsoFramerControl, ViewObjectEx);
	ODS("CDsoFramerControl::XViewObjectEx::GetRect\n");

    if (dwAspect != DVASPECT_CONTENT)
        return DV_E_DVASPECT;

	if (pRect)
	{
		CopyRect((LPRECT)pRect, &(pThis->m_rcLocation));

	 // Convert to himetric (according to docs, this is required)
		PixelsToHimetric(&(pRect->left), &(pRect->top));
		PixelsToHimetric(&(pRect->right), &(pRect->bottom));
	}

	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::GetViewStatus(DWORD* pdwStatus)
{
	ODS("CDsoFramerControl::XViewObjectEx::GetViewStatus\n");
	if (pdwStatus) *pdwStatus = VIEWSTATUS_OPAQUE;
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::QueryHitPoint(DWORD dwAspect, LPCRECT pRectBounds, POINT ptlLoc, LONG lCloseHint, DWORD *pHitResult)
{
	ODS("CDsoFramerControl::XViewObjectEx::QueryHitPoint\n");

    if (dwAspect != DVASPECT_CONTENT)
        return DV_E_DVASPECT;

 // OVERRIDE: override me if you want to provide additional
 // non-opaque functionality...
    *pHitResult = PtInRect(pRectBounds, ptlLoc) ? HITRESULT_HIT : HITRESULT_OUTSIDE;
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::QueryHitRect(DWORD dwAspect, LPCRECT pRectBounds, LPCRECT pRectLoc, LONG lCloseHint, DWORD *pHitResult)
{
    RECT rc;
	ODS("CDsoFramerControl::XViewObjectEx::QueryHitRect\n");

    if (dwAspect != DVASPECT_CONTENT)
        return DV_E_DVASPECT;

    *pHitResult = IntersectRect(&rc, pRectBounds, pRectLoc) ? HITRESULT_HIT : HITRESULT_OUTSIDE;
    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XViewObjectEx::GetNaturalExtent(DWORD dwAspect, LONG lindex, DVTARGETDEVICE *ptd, HDC hicTargetDev, DVEXTENTINFO *pExtentInfo, LPSIZEL pSizel)
{
	ODS("CDsoFramerControl::XViewObjectEx::GetNaturalExtent\n");
	return E_NOTIMPL;
}


////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XProvideClassInfo -- IProvideClassInfo Implementation
//
//	 STDMETHODIMP GetClassInfo(ITypeInfo** ppTI);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, ProvideClassInfo)

STDMETHODIMP CDsoFramerControl::XProvideClassInfo::GetClassInfo(ITypeInfo** ppTI)
{
	ODS("CDsoFramerControl::XProvideClassInfo::GetClassInfo\n");
	return GetTypeInfoEx(LIBID_DSOFramer, 0,
		DSOFRAMERCTL_VERSION_MAJOR, DSOFRAMERCTL_VERSION_MINOR, v_hModule, CLSID_FramerControl, ppTI);
}

////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XConnectionPointContainer -- IConnectionPointContainer Implementation
//
//	 STDMETHODIMP EnumConnectionPoints(IEnumConnectionPoints **ppEnum);
//	 STDMETHODIMP FindConnectionPoint(REFIID riid, IConnectionPoint **ppCP);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, ConnectionPointContainer)

STDMETHODIMP CDsoFramerControl::XConnectionPointContainer::EnumConnectionPoints(IEnumConnectionPoints **ppEnum)
{
	METHOD_PROLOGUE(CDsoFramerControl, ConnectionPointContainer);
    ODS("CDsoFramerControl::XConnectionPointContainer::EnumConnectionPoints\n");
	if (ppEnum)	(*ppEnum = &(pThis->m_xEnumConnectionPoints))->AddRef();
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XConnectionPointContainer::FindConnectionPoint(REFIID riid, IConnectionPoint **ppCP)
{
	METHOD_PROLOGUE(CDsoFramerControl, ConnectionPointContainer);
    ODS("CDsoFramerControl::XConnectionPointContainer::FindConnectionPoint\n");
	if (riid != DIID__DFramerCtlEvents) return CONNECT_E_NOCONNECTION;
	if (ppCP) (*ppCP = &(pThis->m_xConnectionPoint))->AddRef();
	return S_OK;
}


////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XEnumConnectionPoints -- IEnumConnectionPoints Implementation
//
//	 STDMETHODIMP Next(ULONG cConnections, IConnectionPoint **rgpcn, ULONG *pcFetched);
//	 STDMETHODIMP Skip(ULONG cConnections);
//	 STDMETHODIMP Reset(void);
//	 STDMETHODIMP Clone(IEnumConnectionPoints **ppEnum);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, EnumConnectionPoints)

STDMETHODIMP CDsoFramerControl::XEnumConnectionPoints::Next(ULONG cConnections, IConnectionPoint **rgpcn, ULONG *pcFetched)
{
	HRESULT hr = S_FALSE;

	METHOD_PROLOGUE(CDsoFramerControl, EnumConnectionPoints);
    ODS("CDsoFramerControl::XEnumConnectionPoints::Next\n");

	CHECK_NULL_RETURN(rgpcn, E_POINTER);
	*rgpcn = NULL;

	CHECK_NULL_RETURN(pcFetched, E_POINTER);
	*pcFetched = 0;

	if (pThis->m_fConCntDone)
		pThis->m_fConCntDone = FALSE;
	else
	{
		pThis->m_fConCntDone = TRUE;
		(*rgpcn = &(pThis->m_xConnectionPoint))->AddRef();
		*pcFetched = 1;
		hr = S_OK;
	}

	return hr;
}

STDMETHODIMP CDsoFramerControl::XEnumConnectionPoints::Skip(ULONG cConnections)
{return S_OK;}

STDMETHODIMP CDsoFramerControl::XEnumConnectionPoints::Reset(void)
{return S_OK;}

STDMETHODIMP CDsoFramerControl::XEnumConnectionPoints::Clone(IEnumConnectionPoints **ppEnum)
{if (ppEnum) *ppEnum = this; return S_OK;}


////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XConnectionPoint -- IConnectionPoint Implementation
//
//	 STDMETHODIMP GetConnectionInterface(IID *pIID);
//	 STDMETHODIMP GetConnectionPointContainer(IConnectionPointContainer **ppCPC);
//	 STDMETHODIMP Advise(IUnknown *pUnk, DWORD *pdwCookie);
//	 STDMETHODIMP Unadvise(DWORD dwCookie);
//	 STDMETHODIMP EnumConnections(IEnumConnections **ppEnum);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, ConnectionPoint)

STDMETHODIMP CDsoFramerControl::XConnectionPoint::GetConnectionInterface(IID *pIID)
{
    ODS("CDsoFramerControl::XConnectionPoint::GetConnectionInterface\n");
	if (pIID) *pIID = DIID__DFramerCtlEvents;
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XConnectionPoint::GetConnectionPointContainer(IConnectionPointContainer **ppCPC)
{
	METHOD_PROLOGUE(CDsoFramerControl, ConnectionPoint);
    ODS("CDsoFramerControl::XConnectionPoint::GetConnectionPointContainer\n");
	if (ppCPC) (*ppCPC = &(pThis->m_xConnectionPointContainer))->AddRef();
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XConnectionPoint::Advise(IUnknown *pUnk, DWORD *pdwCookie)
{
	METHOD_PROLOGUE(CDsoFramerControl, ConnectionPoint);
    ODS("CDsoFramerControl::XConnectionPoint::Advise\n");

	if (!pUnk) return E_POINTER;
	RELEASE_INTERFACE(pThis->m_dispEvents);

	pUnk->QueryInterface(IID_IDispatch, (void**)&(pThis->m_dispEvents));
	if (pdwCookie) *pdwCookie = 123;
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XConnectionPoint::Unadvise(DWORD dwCookie)
{
	METHOD_PROLOGUE(CDsoFramerControl, ConnectionPoint);
    ODS("CDsoFramerControl::XConnectionPoint::Unadvise\n");
	RELEASE_INTERFACE(pThis->m_dispEvents);
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XConnectionPoint::EnumConnections(IEnumConnections **ppEnum)
{if (ppEnum) *ppEnum = NULL; return E_NOTIMPL;}


////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XOleCommandTarget -- IOleCommandTarget Implementation
//
//   STDMETHODIMP QueryStatus(const GUID *pguidCmdGroup, ULONG cCmds, OLECMD prgCmds[], OLECMDTEXT *pCmdText);
//   STDMETHODIMP Exec(const GUID *pguidCmdGroup, DWORD nCmdID, DWORD nCmdexecopt, VARIANTARG *pvaIn, VARIANTARG *pvaOut);            
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, OleCommandTarget)

STDMETHODIMP CDsoFramerControl::XOleCommandTarget::QueryStatus(const GUID *pguidCmdGroup, ULONG cCmds, OLECMD prgCmds[], OLECMDTEXT *pCmdText)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleCommandTarget);
    //ODS("CDsoFramerControl::XOleCommandTarget::QueryStatus\n");
	if (pguidCmdGroup != NULL)
		return OLECMDERR_E_UNKNOWNGROUP;
			
	if (prgCmds == NULL)
		return E_INVALIDARG;

    for (int i = 0; i < (int)cCmds; i++) 
	{
		switch (prgCmds[i].cmdID) 
		{
		case OLECMDID_NEW:			prgCmds[i].cmdf = (((pThis->m_wMenuItems &   1) ? OLECMDF_ENABLED : 0) | OLECMDF_SUPPORTED); break;
		case OLECMDID_OPEN:			prgCmds[i].cmdf = (((pThis->m_wMenuItems &   2) ? OLECMDF_ENABLED : 0) | OLECMDF_SUPPORTED); break;
		case OLECMDID_SAVE:			prgCmds[i].cmdf = (((pThis->m_wMenuItems &   8) ? OLECMDF_ENABLED : 0) | OLECMDF_SUPPORTED); break;
		case OLECMDID_SAVEAS:		prgCmds[i].cmdf = (((pThis->m_wMenuItems &  16) ? OLECMDF_ENABLED : 0) | OLECMDF_SUPPORTED); break;
		case OLECMDID_PRINT:		prgCmds[i].cmdf = (((pThis->m_wMenuItems &  32) ? OLECMDF_ENABLED : 0) | OLECMDF_SUPPORTED); break;
		case OLECMDID_PAGESETUP:	prgCmds[i].cmdf = (((pThis->m_wMenuItems & 128) ? OLECMDF_ENABLED : 0) | OLECMDF_SUPPORTED); break;
		case OLECMDID_PROPERTIES:	prgCmds[i].cmdf = (((pThis->m_wMenuItems & 256) ? OLECMDF_ENABLED : 0) | OLECMDF_SUPPORTED); break;
		default: prgCmds[i].cmdf = 0;
		}
	}
	
	if (pCmdText)
    {
        pCmdText->cmdtextf = MSOCMDTEXTF_NONE;
        pCmdText->cwActual = 0;
    }

    return S_OK;
}

STDMETHODIMP CDsoFramerControl::XOleCommandTarget::Exec(const GUID *pguidCmdGroup, DWORD nCmdID, DWORD nCmdexecopt, VARIANTARG *pvaIn, VARIANTARG *pvaOut)
{
	METHOD_PROLOGUE(CDsoFramerControl, OleCommandTarget);
    ODS("CDsoFramerControl::XOleCommandTarget::Exec\n");

    if (pguidCmdGroup != NULL)
        return OLECMDERR_E_UNKNOWNGROUP;
	    
    if (nCmdexecopt == MSOCMDEXECOPT_SHOWHELP)
        return OLECMDERR_E_NOHELP;

	switch (nCmdID)
	{
	case OLECMDID_NEW:
	case OLECMDID_OPEN:
	case OLECMDID_SAVE:
	case OLECMDID_SAVEAS:
	case OLECMDID_PRINT:
	case OLECMDID_PAGESETUP:
	case OLECMDID_PROPERTIES:
		PostMessage(pThis->m_hwnd, WM_APP+300, nCmdID, nCmdexecopt);
		return S_OK;
	}

	return OLECMDERR_E_NOTSUPPORTED;
}

////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XSupportErrorInfo -- ISupportErrorInfo Implementation
//
//	 STDMETHODIMP InterfaceSupportsErrorInfo(REFIID riid);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, SupportErrorInfo)

STDMETHODIMP CDsoFramerControl::XSupportErrorInfo::InterfaceSupportsErrorInfo(REFIID riid)
{
	ODS("CDsoFramerControl::XSupportErrorInfo::InterfaceSupportsErrorInfo\n");
	return ((riid == IID__FramerControl) ? S_OK : E_FAIL);
}

////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::XObjectSafety -- IObjectSafety Implementation
//
//  STDMETHODIMP GetInterfaceSafetyOptions(REFIID riid, DWORD *pdwSupportedOptions,DWORD *pdwEnabledOptions);
//  STDMETHODIMP SetInterfaceSafetyOptions(REFIID riid, DWORD dwOptionSetMask, DWORD dwEnabledOptions);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoFramerControl, ObjectSafety)

STDMETHODIMP CDsoFramerControl::XObjectSafety::GetInterfaceSafetyOptions(REFIID riid, DWORD *pdwSupportedOptions,DWORD *pdwEnabledOptions)
{
	ODS("CDsoFramerControl::XObjectSafety::GetInterfaceSafetyOptions\n");
	CHECK_NULL_RETURN(pdwEnabledOptions, E_POINTER);

	if (pdwSupportedOptions)
		*pdwSupportedOptions = (INTERFACESAFE_FOR_UNTRUSTED_DATA|INTERFACESAFE_FOR_UNTRUSTED_CALLER);

 // We are safe for initialization, but not necessarily for scripting. Since script
 // can open documents with macros, and get access to Office OM via ActiveDocument,
 // we cannot be "safe" for malicious script. But this allows the control to load
 // without prompt (just not scripted without prompt)...
	*pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_DATA;
	return S_OK;
}

STDMETHODIMP CDsoFramerControl::XObjectSafety::SetInterfaceSafetyOptions(REFIID riid, DWORD dwOptionSetMask, DWORD dwEnabledOptions)
{
	ODS("CDsoFramerControl::XObjectSafety::SetInterfaceSafetyOptions\n");
	return ((((riid == IID_IPersist) || (riid == IID_IPersistPropertyBag)) &&
		(dwEnabledOptions == INTERFACESAFE_FOR_UNTRUSTED_DATA)) ? S_OK : E_FAIL);
}

////////////////////////////////////////////////////////////////////////
//
// CDsoFramerControl::DoDialogAction
//
//  A very simple implementation of a New/Open/Save dialogs to serve as
//  default actions in case developer does not override these "File"
//  commands in OnFileCommand.
//     
#include <commdlg.h>
#include <oledlg.h>

static const CHAR v_szFileFilter[] =
 "Office Files\0*.doc;*.xls;*.ppt;*.vsd;*.rtf;*.csv\0All Files (*.*)\0*.*\0\0";

STDMETHODIMP CDsoFramerControl::DoDialogAction(dsoShowDialogType item)
{
	HRESULT    hr = S_FALSE;
	CHAR       szFile[MAX_PATH]; szFile[0] = 0;
	CHAR       szCustFlt[MAX_PATH]; szCustFlt[0] = 0;
	VARIANT    vT[3]; memset(&vT[0], 0, (sizeof(VARIANT) * 3));

	if ((item == dsoDialogOpen) || (item == dsoDialogSave))
	{
		OPENFILENAME    ofn;

		memset(&ofn, 0, sizeof(OPENFILENAME));

		ofn.lStructSize        = sizeof(OPENFILENAME);
		ofn.hwndOwner          = m_hwnd;
		ofn.lpstrFilter        = v_szFileFilter;
		ofn.nFilterIndex       = 1;
		ofn.lpstrFile          = szFile;
		ofn.lpstrCustomFilter  = szCustFlt;
		ofn.nMaxFile           = MAX_PATH;
		ofn.nMaxCustFilter     = MAX_PATH;

		if (item == dsoDialogOpen)
		{
			ofn.Flags = OFN_FILEMUSTEXIST | OFN_EXPLORER;
			if ((GetOpenFileName(&ofn)) && 
				(NULL != (vT[0].bstrVal = ConvertToBSTR(szFile))))
			{
				vT[0].vt = VT_BSTR;
				vT[1].vt = VT_I4;	vT[1].lVal = (ofn.Flags & OFN_READONLY);
				hr = Open(vT[0], vT[1], vT[2], vT[2], vT[2]);
				VariantClear(&vT[0]);
			}
		}
		else
		{
			ofn.Flags  = OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT | OFN_EXPLORER; // | OFN_PATHMUSTEXIST;
			if ((GetSaveFileName(&ofn)) && 
				(NULL != (vT[0].bstrVal = ConvertToBSTR(szFile))))
			{
				vT[0].vt = VT_BSTR;
                vT[1].vt = VT_BOOL;
                vT[1].boolVal = 1;

				hr = Save(vT[0], vT[1], vT[2], vT[2]);
				VariantClear(&vT[0]);
			}
		}
	}
	else if (item == dsoDialogNew)
	{
		OLEUIINSERTOBJECT oidlg = {0};
		LPCLSID lpNewExcludeList = NULL;
		int nNewExcludeCount = 0;
		int nNewExcludeLen = 0;

		oidlg.cbStruct = sizeof(OLEUIINSERTOBJECT);
		oidlg.dwFlags = IOF_SELECTCREATENEW | IOF_DISABLELINK | IOF_DISABLEDISPLAYASICON | IOF_HIDECHANGEICON;
		oidlg.hWndOwner = m_hwnd;
		oidlg.lpszCaption = "Insert New Document Object";
		oidlg.lpszFile = szFile;
		oidlg.cchFile = MAX_PATH;

		HKEY hkCLSID;
		HKEY hkItem;
		HKEY hkDocObject;
		DWORD dwIndex = 0;
		CHAR szName[MAX_PATH+1];

		if(RegOpenKeyEx(HKEY_CLASSES_ROOT, "CLSID", 0, KEY_READ|KEY_ENUMERATE_SUB_KEYS, &hkCLSID) == ERROR_SUCCESS)
		{
			while(RegEnumKey(hkCLSID, dwIndex++, szName, MAX_PATH) == ERROR_SUCCESS)
			{
				if (RegOpenKeyEx(hkCLSID, szName, 0, KEY_READ, &hkItem) == ERROR_SUCCESS)
				{
					if ((RegOpenKeyEx(hkItem, "Insertable", 0, KEY_READ, &hkDocObject) == ERROR_SUCCESS))
					{
						RegCloseKey(hkDocObject);
						if ((RegOpenKeyEx(hkItem, "DocObject", 0, KEY_READ, &hkDocObject) != ERROR_SUCCESS))
						{
							CLSID clsid;
							LPWSTR pwszClsid = ConvertToLPWSTR(szName);
							if ((pwszClsid) && SUCCEEDED(CLSIDFromString(pwszClsid, &clsid)))
							{
								if (lpNewExcludeList == NULL)
								{
									nNewExcludeCount = 0;
									nNewExcludeLen = 16;
									lpNewExcludeList = new CLSID[nNewExcludeLen];
								}
								if (nNewExcludeCount == nNewExcludeLen)
								{
									LPCLSID lpOldList = lpNewExcludeList;
									nNewExcludeLen <<= 2;
									lpNewExcludeList = new CLSID[nNewExcludeLen];
									memcpy(lpNewExcludeList, lpOldList, sizeof(CLSID) * nNewExcludeCount);
									delete [] lpOldList;
								}

								lpNewExcludeList[nNewExcludeCount] = clsid;
								nNewExcludeCount++;
							}
							MemFree(pwszClsid);

						} else RegCloseKey(hkDocObject);
					}
					RegCloseKey(hkItem);
				}
			}
			RegCloseKey(hkCLSID);
		}

		oidlg.lpClsidExclude = lpNewExcludeList;
		oidlg.cClsidExclude = nNewExcludeCount;

		if (OleUIInsertObject(&oidlg) == OLEUI_OK)
		{
			if ((oidlg.dwFlags & IOF_SELECTCREATENEW) && (oidlg.clsid != GUID_NULL))
			{
				LPOLESTR posz;
				BSTR bstr;
				if (SUCCEEDED(ProgIDFromCLSID(oidlg.clsid, &posz)))
				{
					bstr = SysAllocString(posz);
					hr = CreateNew(bstr);
					SysFreeString(bstr);
					CoTaskMemFree(posz);
				}
			}
			else if ((oidlg.dwFlags & IOF_SELECTCREATEFROMFILE) && (szFile[0] != 0))
			{
				vT[0].vt = VT_BSTR;
				vT[0].bstrVal = ConvertToBSTR(szFile);
				vT[1].vt = VT_I4; vT[1].lVal = 1;

				hr = Open(vT[0], vT[1], vT[2], vT[2], vT[2]);

				VariantClear(&vT[0]);
			}
			else
			{
				MessageBeep(0);
			}

		}

		if (lpNewExcludeList)
			delete [] lpNewExcludeList;

	}
	return hr;
}


////////////////////////////////////////////////////////////////////////
// CDsoFrameWindowHook
//

////////////////////////////////////////////////////////////////////////
// CDsoFrameWindowHook::AttachToFrameWindow
//
//  Finds an existing hook or makes a new one and registers the control
//  as a component in the list. This does not make the component active.
//  Callers should use SetActiveComponent to activate.
//
STDMETHODIMP_(CDsoFrameWindowHook*) CDsoFrameWindowHook::AttachToFrameWindow(HWND hwndCtl, CDsoFramerControl* pocx)
{
	CDsoFrameWindowHook* phook = NULL;
	HWND hwndNext, hwndHost = hwndCtl;

 // Find the top-level window this control is sited on...
	while (hwndNext = GetParent(hwndHost))
		hwndHost = hwndNext;

 // We have to be in-thread for a subclass to work...
	if (!IsWindow(hwndHost) || 
		(GetWindowThreadProcessId(hwndHost, NULL) != GetCurrentThreadId()))
		return NULL;

	EnterCriticalSection(&v_csecThreadSynch);

 // We will try to get the active hook for this host window (if one was already created)
 // or create an initialize a new one...
	if (((phook = (CDsoFrameWindowHook*)GetProp(hwndHost, "DSOFramerWndHook")) == NULL) &&
		(phook = new CDsoFrameWindowHook()))
	{
		phook->m_hwndTopLevelHost = hwndHost;
		if (!SetProp(hwndHost, "DSOFramerWndHook", (HANDLE)phook) ||
			(!(phook->m_pfnOrigWndProc = (WNDPROC)SetWindowLong(hwndHost, GWL_WNDPROC, (LONG)(WNDPROC)HostWindowProcHook))))
		{
			delete phook;
			phook = NULL;
		}
	}

 // If we have the hook and we aren't past the limit, add this control...
	if ((phook) && ((phook->m_cControls) < DSOF_MAX_CONTROLS))
	{
		phook->m_pControls[(phook->m_cControls)++] = pocx;
	}
	else phook = NULL;

	LeaveCriticalSection(&v_csecThreadSynch);

	return phook;
}

////////////////////////////////////////////////////////////////////////
// CDsoFrameWindowHook::Detach
//
//  Unregisters the control from the hook list. This call can remove the
//  subclass and delete the hook, so callers should not access a cached
//  pointer after calling this method.
//
STDMETHODIMP CDsoFrameWindowHook::Detach(CDsoFramerControl* pocx)
{
	DWORD dwIndex;
	CHECK_NULL_RETURN(m_cControls, E_FAIL);

 // Find this control in the list...
	for (dwIndex = 0; dwIndex < m_cControls; dwIndex++)
		if (m_pControls[dwIndex] == pocx) break;

	if (dwIndex > m_cControls)
		return E_INVALIDARG;

 // If we found the index, remove the item and shift remaining
 // items down the list. Change active index as needed...
	EnterCriticalSection(&v_csecThreadSynch);

	m_pControls[dwIndex] = NULL;

	while (++dwIndex < m_cControls)
	{
		m_pControls[dwIndex-1] = m_pControls[dwIndex];
		m_pControls[dwIndex] = NULL;
	}

	if (--m_cControls)
	{
		while (m_idxActive >= m_cControls)
			--m_idxActive;
	}
	else
	{
	 // If this is the last control, we can remove the hook!
		SetWindowLong(m_hwndTopLevelHost, GWL_WNDPROC, (LONG)(m_pfnOrigWndProc));
		RemoveProp(m_hwndTopLevelHost, "DSOFramerWndHook");
		delete this;
	}

	LeaveCriticalSection(&v_csecThreadSynch);

	return S_OK;
}

////////////////////////////////////////////////////////////////////////
// CDsoFrameWindowHook::SetActiveComponent
//
//  Notifies the hook when a component "takes" activation so it can 
//  forward this message to other registered components. Called by the
//  control when it goes UI active.
//
STDMETHODIMP CDsoFrameWindowHook::SetActiveComponent(CDsoFramerControl* pocx)
{
	ODS("!! CDsoFrameWindowHook::SetActiveComponent !!\n");
	CHECK_NULL_RETURN(m_cControls, E_FAIL);
	CDsoFramerControl* pnext;
	DWORD dwIndex;

 // Find the index of the control...
	for (dwIndex = 0; dwIndex < m_cControls; dwIndex++)
		if (m_pControls[dwIndex] == pocx) break;

	if (dwIndex > m_cControls)
		return E_INVALIDARG;

 // If it is not already the active item, notify old component it is
 // losing activation, and notify new component that it is gaining...
	if (dwIndex != m_idxActive)
	{
		pnext = m_pControls[m_idxActive];
		if (pnext) pnext->OnComponentActivationChange(FALSE);

		m_idxActive = dwIndex;

		pnext = m_pControls[m_idxActive];
		if (pnext) pnext->OnComponentActivationChange(TRUE);
	}

	return S_OK;
}


////////////////////////////////////////////////////////////////////////
// CDsoFrameWindowHook::HostWindowProcHook
//
//  The window proc for the subclassed host window.
//
STDMETHODIMP_(LRESULT) CDsoFrameWindowHook::HostWindowProcHook(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	CDsoFramerControl* pocx = NULL;
	CDsoFrameWindowHook* phook = (CDsoFrameWindowHook*)GetProp(hwnd, "DSOFramerWndHook");

	if (phook)
	{
		switch (msg)
		{
	 // If our hook is running in IE, IE gives bad data on WM_GETTEXT messages. 
     // The default window proc returns the correct data, so we use it...
		case WM_GETTEXT:
            {
                CHAR szbuf[255];
                if (GetClassName(hwnd, szbuf, 255) && (lstrcmp(szbuf, "IEFrame") == 0))
			        return DefWindowProc(hwnd, msg, wParam, lParam);
            }
            break;

		case WM_ACTIVATEAPP:
			pocx = phook->GetActiveComponent();
			if (pocx)
				pocx->OnAppActivation((BOOL)wParam, lParam);
			break;

		case WM_PALETTECHANGED:
			pocx = phook->GetActiveComponent();
			if (pocx)
				pocx->OnPaletteChanged((HWND)wParam);
			break;
		}

		return CallWindowProc(phook->m_pfnOrigWndProc, hwnd, msg, wParam, lParam);
	}
	return DefWindowProc(hwnd, msg, wParam, lParam);
}
