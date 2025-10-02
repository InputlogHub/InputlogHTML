/***************************************************************************
 * DSOFDOCOBJ.CPP
 *
 * CDsoDocObject: ActiveX Document Single Instance Frame/Site Object
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
// CDsoDocObject - The DocObject Site Class
//
//  This class wraps the functionality for DocObject hosting. Right now
//  we are setup for one active site at a time, but this could be changed
//  to allow multiple sites (although only one could be UI active at any
//  given time).
//

CDsoDocObject::CDsoDocObject()
{
	ODS("CDsoDocObject::CDsoDocObject\n");
	m_cRef = 1;
	m_fDisplayTools = TRUE;
}

CDsoDocObject::~CDsoDocObject(void)
{
	ODS("CDsoDocObject::~CDsoDocObject\n");
	if (m_pole)	Close();
	if (m_hwnd) DestroyWindow(m_hwnd);

	SAFE_FREESTRING(m_pwszUsername);
	SAFE_FREESTRING(m_pwszPassword);

	RELEASE_INTERFACE(m_punkRosebud);
	RELEASE_INTERFACE(m_pcmdCtl);
	RELEASE_INTERFACE(m_pstgroot);
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::InitializeNewInstance
//
//  Sets up the docobject site. We must a control site to attach to and
//  a bounding rect. The IOleCommandTarget is used to forward toolbar
//  commands back to the host if a user selects one.
//
STDMETHODIMP CDsoDocObject::InitializeNewInstance(HWND hwndCtl, LPRECT prcPlace, IOleCommandTarget* pcmdCtl)
{
	HRESULT hr = E_UNEXPECTED;
	WNDCLASS wndclass;

 // As an AxDoc site, we need a valid parent window...
	if ((!hwndCtl) || (!IsWindow(hwndCtl)))
		return hr;

 // Create a temp storage for this docobj site (if one already exists, bomb out)...
	if ((m_pstgroot) || FAILED(hr = StgCreateDocfile(NULL,	STGM_TRANSACTED | STGM_READWRITE |
			STGM_SHARE_EXCLUSIVE | STGM_CREATE | STGM_DELETEONRELEASE, 0, &m_pstgroot)))
		return hr;

 // If our site window class has not been registered before, we should register it...

 // This is protected by a critical section just for fun. The fact we had to single
 // instance the OCX because of the host hook makes having multiple instances conflict here
 // very unlikely. However, that could change sometime, so better to be safe than sorry.
	EnterCriticalSection(&v_csecThreadSynch);

	if (GetClassInfo(v_hModule, "DSOFramerDocWnd", &wndclass) == 0)
	{
		memset(&wndclass, 0, sizeof(WNDCLASS));
		wndclass.style          = CS_VREDRAW | CS_HREDRAW | CS_DBLCLKS;
		wndclass.lpfnWndProc    = CDsoDocObject::FrameWindowProc;
		wndclass.hInstance      = v_hModule;
		wndclass.hCursor        = LoadCursor(NULL, IDC_ARROW);
		wndclass.lpszClassName  = "DSOFramerDocWnd";
		if (RegisterClass(&wndclass) == 0)
			hr = HRESULT_FROM_WIN32(GetLastError());
	}

	LeaveCriticalSection(&v_csecThreadSynch);
	if (FAILED(hr)) return hr;
	
 // Save the place RECT (and validate as needed)...
	CopyRect(&m_rcViewRect, prcPlace);
	if (m_rcViewRect.top > m_rcViewRect.bottom)	{m_rcViewRect.top = 0; m_rcViewRect.bottom = 0;}
	if (m_rcViewRect.left > m_rcViewRect.right)	{m_rcViewRect.left = 0; m_rcViewRect.right = 0;}

 // Create our site window at the give location (we are child of the control window)...
	m_hwnd = CreateWindowEx(0, "DSOFramerDocWnd", NULL, WS_CHILD | WS_VISIBLE,
                    m_rcViewRect.left, m_rcViewRect.top,
					(m_rcViewRect.right - m_rcViewRect.left),
					(m_rcViewRect.bottom - m_rcViewRect.top),
                    hwndCtl, NULL, v_hModule, NULL);


	if (!m_hwnd) return E_OUTOFMEMORY;

	SetWindowLong(m_hwnd, GWL_USERDATA, (LONG)this);

	m_hwndCtl = hwndCtl;

	if (pcmdCtl)
		(m_pcmdCtl = pcmdCtl)->AddRef();

	return S_OK;
}


////////////////////////////////////////////////////////////////////////
// CDsoDocObject::CreateDocObject
//
//  This does the actual embedding. It is called no matter how you load
//  an object, and does some checking to make sure the CLSID is for a
//  a docobj server. If the function succeeds, we have an embedded object.
//  To activate and show the object, you must call IPActivateView().
//
STDMETHODIMP CDsoDocObject::CreateDocObject(REFCLSID rclsid)
{
	HRESULT             hr;
	BOOL                fInitNew;
	CLSID               clsid;
	DWORD               dwMiscStatus = 0;
    IOleObject*         pole    = NULL;
    IPersistStorage*    pipstg  = NULL;

    ODS("CDsoDocObject::CreateDocObject()\n");

	ASSERT(!(m_pole));

 // Don't load if an object has already been loaded...
    if (m_pole) return E_UNEXPECTED;

 // First, check the server to make sure it is AxDoc server...
	if (FAILED(hr = ValidateDocObjectServer(rclsid)))
		return hr;

 // If we haven't loaded a storage, create a new one and remember to
 // call InitNew (instead of Load) later on...
	if ((fInitNew = (!m_pstgfile)) && FAILED(hr = CreateObjectStorage(rclsid)))
		return hr;

 // It is possible that someone picked an older ProgId/CLSID that
 // will AutoConvert on CoCreate, so fix up the storage with the
 // new CLSID info. We we actually call CoCreate on the new CLSID...
	if (fInitNew && SUCCEEDED(OleGetAutoConvert(rclsid, &clsid)))
	{
		OleDoAutoConvert(m_pstgfile, &clsid);
	}
	else clsid = rclsid;

 // We are ready to create an instance. Call CoCreate to make an
 // inproc handler and ask for IOleObject (all docobjs must support this)...
    if (FAILED(hr = CoCreateInstance(clsid, NULL, CLSCTX_INPROC, IID_IOleObject, (void**)&pole)))
		return hr;

 // Do a quick check to see if server wants us to set client site before the load...
	if (SUCCEEDED(hr = pole->GetMiscStatus(DVASPECT_CONTENT, &dwMiscStatus)) &&
		(dwMiscStatus & OLEMISC_SETCLIENTSITEFIRST))
		pole->SetClientSite((IOleClientSite*)&m_xOleClientSite);

 // Load up the bloody thing...
	if (SUCCEEDED(hr = pole->QueryInterface(IID_IPersistStorage, (void**)&pipstg)))
	{
     // Remember to InitNew if this is a new storage...			
		hr = ((fInitNew) ? pipstg->InitNew(m_pstgfile) : pipstg->Load(m_pstgfile));
		pipstg->Release();
	}

 // Assuming all the above worked we should have an OLE Embeddable
 // object and should finish the initialization (set object running)...
	if (SUCCEEDED(hr))
	{
	 // Save the IOleObject* and do a disconnect on quit...
		m_fDisconnectOnQuit = TRUE;
		m_pole = pole;

	 // Make sure the object is running (uses IRunnableObject)...
		OleRun(m_pole);

	 // If we didn't do so already, set our client site...
		if (!(dwMiscStatus & OLEMISC_SETCLIENTSITEFIRST))
			m_pole->SetClientSite((IOleClientSite*)&m_xOleClientSite);

	 // Set the host names and then lock running...
		m_pole->SetHostNames(L"DSOFramer", L"DSOFramerControl");
		OleLockRunning(m_pole, TRUE, FALSE);

	 // Keep server CLSID for this object
		m_clsidObject = clsid;
	}
	else
	{
	 // We hit an error so cleanup (release should free the obj)...
		if (dwMiscStatus & OLEMISC_SETCLIENTSITEFIRST)
			pole->SetClientSite(NULL);

		pole->Release();
	}

    return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::LoadStorageFromFile
//
//  Loads the internal IStorage from a file (local or UNC).
//
//  We handle three types of files: (1) BIFF files, which will just copy
//  their storage into our own; (2) non-BIFF files that are associated
//  with Office and can be loaded by IMoniker; and (3) non-BIFF files 
//  that are not associated with Office but may be loaded in Office using 
//  IPersistFile and then IPersistStorage (if an alternate CLSID is given), 
//
//  We do a special check for HTML/TXT files because they are associated
//  with IE, and we don't support IE files per se. Instead, you should pass
//  an alternate CLSID for the Office app you want to open that file in.
//
STDMETHODIMP CDsoDocObject::LoadStorageFromFile(LPWSTR pwszFile, REFCLSID rclsid, BOOL fKeepLock)
{
	HRESULT			hr;
	CLSID           clsid;
	CLSID           clsidConv;
	DWORD           dwBindFlgs;
	IStorage        *pstg    = NULL;
	IBindCtx		*pbctx   = NULL;
	IMoniker		*pmkfile = NULL;
	IPersistStorage *pipstg  = NULL;
	BOOL fLoadFromAltCLSID   = (rclsid != GUID_NULL);

	if (!(pwszFile) || ((*pwszFile) == L'\0'))
		return E_INVALIDARG;

	TRACE1("CDsoDocObject::LoadStorageFromFile(%S)\n", pwszFile);

 // First. we'll try to find the associated CLSID for the given file,
 // and then set it to the alternate if not found. If we don't have a
 // CLSID by the end of this, because user didn't specify alternate
 // and GetClassFile failed, then we error out...
    if (FAILED(GetClassFile(pwszFile, &clsid)) && !(fLoadFromAltCLSID))
		return DSO_E_INVALIDSERVER;

 // We should try to load from alternate CLSID if provided one...
    if (fLoadFromAltCLSID) clsid = rclsid;

 // We should also handle auto-convert to start "newest" server...
	if (SUCCEEDED(OleGetAutoConvert(clsid, &clsidConv)))
		clsid = clsidConv;

 // Validate that we have a DocObject server...
	if ((clsid == GUID_NULL) || FAILED(ValidateDocObjectServer(clsid)))
		return DSO_E_INVALIDSERVER;

 // We should have a CLSID, and can now create our substorage...
	if (FAILED(hr = CreateObjectStorage(clsid)))
		return hr;

 // Check for IE cache items since these are read-only as far as user is concerned...
    if (IsIECacheFile(pwszFile)) fKeepLock = FALSE;

	dwBindFlgs = (STGM_TRANSACTED | STGM_SHARE_DENY_WRITE | (fKeepLock ? STGM_READWRITE : STGM_READ));

 // If these are native Office BiFF files, we can streamline the process to
 // copy the main storage into our sub-storage, without any involvement from
 // the server to fill our substorage. This will account for most Office loads...
	if (SUCCEEDED(hr = StgOpenStorage(pwszFile, NULL, dwBindFlgs, NULL, 0, &pstg)))
	{
		hr = pstg->CopyTo(0, NULL, NULL, m_pstgfile);
		if (SUCCEEDED(hr)) m_pstgfile->Commit(STGC_OVERWRITE);

 // Should that fail, we have to do things the more "formal" way (asking server
 // to properly save itself into our substorage)...
	}
	else if (fLoadFromAltCLSID)
	{
	 // If we are loading using an alternate CLSID that is either associated
     // with another app or is a non-Structured Storage doc not belonging to
     // to Office, then we have to explictily create an instance of the
     // alternate server (not the inproc handler), and load using IPersistFile,
     // then ask the server to save itself as OLE object using IPersistStorage.

	 // This is an expensive way to copy a storage, so we only do this when
	 // we are forced to use the alternate CLSID. This will typically be 
	 // the case for files like *.htm or *.asp that are not associated with
	 // Office, but can be opened in Office...
		IPersistFile *pipf;

		if (SUCCEEDED(hr = CoCreateInstance(clsid, NULL, CLSCTX_SERVER, 
			    IID_IPersistFile, (void**)&pipf)))
		{
			if (SUCCEEDED(hr = pipf->Load(pwszFile, dwBindFlgs)) && 
				SUCCEEDED(hr = pipf->QueryInterface(IID_IPersistStorage, (void**)&pipstg)))
			{
				if (SUCCEEDED(hr = pipstg->Save(m_pstgfile, FALSE)))
					hr = pipstg->SaveCompleted(NULL);
			
				pipstg->Release();
			}
			pipf->Release();
		}
	
	}
	else
	{
	 // Other non-BIFF files that are associated with Office (like *.rtf/*.csv)
	 // we can open based on a moniker. This is a more traditional (i.e., OLE
	 // "Insert From File") way of binding, which uses an inproc handler...
		if (SUCCEEDED(hr = CreateFileMoniker(pwszFile, &pmkfile)))
		{
			if (SUCCEEDED(hr = CreateBindCtx(0, &pbctx)))
			{
			 // We ask for IPersistStorage and do a formal Save to our IStorage...
				if (SUCCEEDED(hr = pmkfile->BindToObject(pbctx, NULL, IID_IPersistStorage, (void**)&pipstg))) 
				{
					if (SUCCEEDED(hr = pipstg->Save(m_pstgfile, FALSE)))
						hr = pipstg->SaveCompleted(NULL);
				
					pipstg->Release();
				}
				else if (hr == E_NOINTERFACE) // Somehow we got a server that doesn't handle OLE embedding...
					hr = STG_E_NOTFILEBASEDSTORAGE;

				if (SUCCEEDED(hr) && (fKeepLock))
				{
				 // If we have our copy and user wants to keep a lock on original source,
				 // we'll try to get the lock by asking for the file's IStorage...
					BIND_OPTS bopts = {sizeof(BIND_OPTS), 1, dwBindFlgs, 0};
					pbctx->SetBindOptions(&bopts);
					pmkfile->BindToStorage(pbctx, NULL, IID_IStorage, (void**)&pstg);
				}
				pbctx->Release();
			}
			pmkfile->Release();
		}
	}

 // Assuming everything above worked...
	if (SUCCEEDED(hr))
	{ 
	  // Let's go ahead and create the object, and keep the lock pointers...
		if (SUCCEEDED(hr = CreateDocObject(clsid)) && (fKeepLock))
		{
			m_fOpenReadOnly = FALSE;
			m_pwszSourceFile = CopyString(pwszFile);
			if (pstg) (m_pstgSourceFile = pstg)->AddRef();
		}
	}

 // If we aren't locking, this will free the file...
	if (pstg) pstg->Release();

	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::LoadStorageFromURL
//
//  Loads the internal IStorage from a URL (http: or https:).
//
//  The idea here is we do a download from the URL and open the result
//  as any other file using LoadStorageFromFile. This requires MSDAIPP
//  which ships with MDAC 2.5, Office 2000/XP, or Windows 2000/XP.
//
//
STDMETHODIMP CDsoDocObject::LoadStorageFromURL(LPWSTR pwszURL, REFCLSID rclsid, LPWSTR pwszUserName, LPWSTR pwszPassword, BOOL fKeepLock)
{
	HRESULT	   hr;
	IStream   *pstmWebResource = NULL;
	LPWSTR     pwszTempFile;

	if (!(m_punkRosebud) && !(m_punkRosebud = CreateRosebudIPP()))
		return DSO_E_REQUIRESMSDAIPP;

	if (!GetTempPathForURLDownload(pwszURL, &pwszTempFile))
		return E_INVALIDARG;

	if (SUCCEEDED(hr = DownloadWebResource(pwszURL, pwszTempFile,
		pwszUserName, pwszPassword, ((fKeepLock) ? &pstmWebResource : NULL))))
	{
		if (SUCCEEDED(hr = LoadStorageFromFile(pwszTempFile, rclsid, fKeepLock)) && 
			((fKeepLock) && (pstmWebResource)))
		{
			m_pwszWebResource = CopyString(pwszURL);
			(m_pstmWebResource = pstmWebResource)->AddRef();
			m_fOpenReadOnly = FALSE;
		}

		if (pstmWebResource)
			pstmWebResource->Release();
	}

	MemFree(pwszTempFile);
	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::IPActivateView
//
//  Activates the object for viewing. If we already have an IOleDocumentView
//  we do this by calling Show, otherwise we'll do an IOleObject::DoVerb.
//
STDMETHODIMP CDsoDocObject::IPActivateView()
{
    HRESULT hr = E_UNEXPECTED;
    ODS("CDsoDocObject::IPActivateView()\n");
    ASSERT(m_pole);

    if ((m_pole) && (!m_pdocv))
    {
		RECT rcView; GetClientRect(m_hwnd, &rcView);		
		hr = m_pole->DoVerb(OLEIVERB_SHOW, NULL, 
				(IOleClientSite*)&m_xOleClientSite, (UINT)-1, m_hwnd, &rcView);
    }
	else if (m_pdocv)
	{
		if (SUCCEEDED(hr = m_pdocv->Show(TRUE)))
			m_pdocv->UIActivate(TRUE);
	}

 // Forward focus as needed...
	if (SUCCEEDED(hr) && (m_hwndIPObject))
		SetFocus(m_hwndIPObject);

    return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::IPDeactivateView
//
//  Deactivates the object.
//
STDMETHODIMP CDsoDocObject::IPDeactivateView()
{
    HRESULT hr = S_OK;
    ODS("CDsoDocObject::IPDeactivateView()\n");

 // If we still have a UI active object, tell it to UI deactivate...
	if (m_pipactive)
		UIActivateView(FALSE);

 // Next hide the active object...
	if (m_pdocv)
        m_pdocv->Show(FALSE);

 // Notify object our intention to IP deactivate...
    if (m_pipobj)
        m_pipobj->InPlaceDeactivate();

 // Close the object down and release pointers...
	if (m_pdocv)
	{
        hr = m_pdocv->CloseView(0);
        m_pdocv->SetInPlaceSite(NULL);
	}

    RELEASE_INTERFACE(m_pcmdt);
    RELEASE_INTERFACE(m_pdocv);
    RELEASE_INTERFACE(m_pipobj);

    return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::UIActivateView
//
//  UI Activates/Deactivates the object as needed.
//
STDMETHODIMP CDsoDocObject::UIActivateView(BOOL fFocus)
{
    HRESULT hr = S_FALSE;
    TRACE1("CDsoDocObject::UIActivateView(%d)\n", fFocus);

	if (m_pdocv)
        hr = m_pdocv->UIActivate(fFocus);

	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::SaveDefault
//
//  Saves the open object back to the original open location (unless it
//  was opened read-only, or is a new object).
//
STDMETHODIMP CDsoDocObject::SaveDefault()
{
	HRESULT	hr = DSO_E_DOCUMENTREADONLY;

	if (m_pstmWebResource)
	{
		hr = SaveStorageToURL(NULL, TRUE, NULL, NULL);
	}
	else if (m_pwszSourceFile)
	{
		hr = SaveStorageToFile(NULL, TRUE);
	}

	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::SaveStorageToFile
//
//  Saves the open object to a file. If you pass NULL for the file, we'll
//  save back to the original open location.
//
STDMETHODIMP CDsoDocObject::SaveStorageToFile(LPWSTR pwszFile, BOOL fOverwriteFile)
{
	HRESULT		  hr = E_UNEXPECTED;
	IPersistFile *pipfile;
	IStorage     *pstg;
	LPWSTR        pwszFullName = NULL;
	LPWSTR		  pwszRename = NULL;
	BOOL          fDoNormalSave = FALSE;
	BOOL          fDoOverwriteOps = FALSE;
	BOOL          fFileOpSuccess = FALSE;

 // Make sure we have the most current state for the file...
	if ((!m_pole) || FAILED(hr = SaveObjectStorage()))
		return hr;

 // If they passed no file, use the default. If none, then current file is read-only...
	if ((!pwszFile) && !(fDoNormalSave = (!!(pwszFile = m_pwszSourceFile))))
		return DSO_E_DOCUMENTREADONLY;

 // Make sure a file extension is given (add one if not)...
	if (ValidateFileExtension(pwszFile, &pwszFullName))
		pwszFile = pwszFullName;

 // See if we will be overwriting, and error unless given permission to do so...
    if ((fDoOverwriteOps = FFileExists(pwszFile)) && !(fOverwriteFile))
        return STG_E_FILEALREADYEXISTS;

 // If we had a previous lock, we have to free it...
	SAFE_RELEASE_INTERFACE(m_pstgSourceFile);

 // If we are overwriting, we do a little Shell Operation here. This is done
 // for two reasons: (1) it keeps the server from asking us to overwrite the
 //  file as it normally would in case of normal save; and (2) it lets us
 // restore the original if the save fails...
	if (fDoOverwriteOps)
	{
		pwszRename = CopyStringCat(pwszFile, L".dstmp");
		fFileOpSuccess = ((pwszRename) && FPerformShellOp(FO_RENAME, pwszFile, pwszRename));
	}

 // Let's do it. First ask for server to save to file if it supports IPersistFile.
 // This gives us a "real" file as output. This will work with almost all Office servers...
	if (SUCCEEDED(hr = m_pole->QueryInterface(IID_IPersistFile, (void**)&pipfile)))
	{
		hr = pipfile->Save(pwszFile, FALSE);
		pipfile->Release();
	}
	else
	{
	 // If that doesn't work, save out the storage to OLE file. This may not produce
	 // the same type of file as you would get by the UI, but it would give a file
	 // that can be opened here again and in any "good" OLE server.
		if (SUCCEEDED(hr = StgCreateDocfile(pwszFile, STGM_TRANSACTED | 
				STGM_READWRITE | STGM_SHARE_EXCLUSIVE | STGM_CREATE, 0, &pstg)))
		{
			WriteClassStg(pstg, m_clsidObject);

			if (SUCCEEDED(hr = m_pstgfile->CopyTo(0, NULL, NULL, pstg)))
				hr = pstg->Commit(STGC_OVERWRITE);

			pstg->Release();
		}
	}

 // If we made a copy to protect on overwrite, either restore or delete it as needed...
	if ((fDoOverwriteOps) && (fFileOpSuccess) && (pwszRename))
	{
		FPerformShellOp((FAILED(hr) ? FO_RENAME : FO_DELETE), pwszRename, pwszFile);
	}

 // If this is an exisitng file save, or the operation failed, relock the
 // the original file save source...
    if (((fDoNormalSave) || (FAILED(hr))) && (m_pwszSourceFile))
    {
		StgOpenStorage(m_pwszSourceFile, NULL, 
				(STGM_TRANSACTED | STGM_SHARE_DENY_WRITE | STGM_READWRITE), NULL, 0, &m_pstgSourceFile);
    }
    else if (SUCCEEDED(hr))
	{
     // Otherwise if we succeeded, free any existing file info we have it and 
     // save the new file info for later re-saves (and lock)...
		SAFE_FREESTRING(m_pwszSourceFile);
		SAFE_FREESTRING(m_pwszWebResource);
		SAFE_RELEASE_INTERFACE(m_pstmWebResource);

	 // Save the name, and try to lock the file for editing...
		if (m_pwszSourceFile = CopyString(pwszFile))
			StgOpenStorage(m_pwszSourceFile, NULL, 
				(STGM_TRANSACTED | STGM_SHARE_DENY_WRITE | STGM_READWRITE), NULL, 0, &m_pstgSourceFile);
	}

	if (pwszRename)
		MemFree(pwszRename);

	if (pwszFullName)
		MemFree(pwszFullName);

	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::SaveStorageToURL
//
//  Saves the open object to a URL. If you pass NULL, we'll save back to
//  the original open location.
//
//  This works very similar to the LoadStorageFromURL in that we save to
//  a local file first using the normal SaveStorageToFile and then push an
//  upload to the server.
//
STDMETHODIMP CDsoDocObject::SaveStorageToURL(LPWSTR pwszURL, BOOL fOverwriteFile, LPWSTR pwszUserName, LPWSTR pwszPassword)
{
	HRESULT	 hr = DSO_E_DOCUMENTREADONLY;

 // If we have no URL to save to and no previously open web stream, fail...
	if ((!pwszURL) && (!m_pstmWebResource))
		return hr;

	if (!(m_punkRosebud) && !(m_punkRosebud = CreateRosebudIPP()))
		return DSO_E_REQUIRESMSDAIPP;

	if ((pwszURL) && (m_pwszWebResource) && 
		AreStringsEqual(pwszURL, -1, m_pwszWebResource, -1))
		pwszURL = NULL;

	if (pwszURL)
	{
		IStream  *pstmT = NULL;
		LPWSTR    pwszFullUrl = NULL;
		LPWSTR    pwszTempFile;

		IStream  *pstmBkupStm;
		IStorage *pstgBkupStg;
		LPWSTR    pwszBkupFile, pwszBkupUrl;

		if (!GetTempPathForURLDownload(pwszURL, &pwszTempFile))
			return E_INVALIDARG;

		if (ValidateFileExtension(pwszURL, &pwszFullUrl))
			pwszURL = pwszFullUrl;

	 // We are going to save out the current file info in case of 
	 // an error we can restore it to do native saves back to open location...
		pstmBkupStm  = m_pstmWebResource;  m_pstmWebResource  = NULL;
		pwszBkupUrl  = m_pwszWebResource;  m_pwszWebResource = NULL;
		pwszBkupFile = m_pwszSourceFile;   m_pwszSourceFile   = NULL;
		pstgBkupStg  = m_pstgSourceFile;   m_pstgSourceFile   = NULL;

	 // Save the object to a new (temp) file on the local drive...
		if (SUCCEEDED(hr = SaveStorageToFile(pwszTempFile, TRUE)))
		{
		 // Then upload from that file...
			hr = UploadWebResource(pwszTempFile, &pstmT, pwszURL, fOverwriteFile, pwszUserName, pwszPassword);
		}

	 // If both calls succeed, we can free the old file/url location info
	 // and save the new information, otherwise restore the old info from backup...
		if (SUCCEEDED(hr))
		{
			RELEASE_INTERFACE(pstgBkupStg);

			if ((pstmBkupStm) && (pwszBkupFile))
				FPerformShellOp(FO_DELETE, pwszBkupFile, NULL);

			RELEASE_INTERFACE(pstmBkupStm);
			SAFE_FREESTRING(pwszBkupUrl);
			SAFE_FREESTRING(pwszBkupFile);

			m_pstmWebResource = pstmT;
			m_pwszWebResource = CopyString(pwszURL);
			//m_pwszSourceFile already saved in SaveStorageToFile
			//m_pstgSourceFile already saved in SaveStorageToFile
		}
		else
		{
			if (m_pstgSourceFile)
				m_pstgSourceFile->Release();

			if (m_pwszSourceFile)
			{
				FPerformShellOp(FO_DELETE, m_pwszSourceFile, NULL);
				MemFree(m_pwszSourceFile);
			}

			m_pstmWebResource  = pstmBkupStm;
			m_pwszWebResource  = pwszBkupUrl;
			m_pwszSourceFile   = pwszBkupFile;
			m_pstgSourceFile   = pstgBkupStg;
		}

		if (pwszFullUrl)
			MemFree(pwszFullUrl);

		MemFree(pwszTempFile);

	}
	else if ((m_pstmWebResource) && (m_pwszSourceFile))
	{
        if (SUCCEEDED(hr = SaveStorageToFile(NULL, TRUE)))
		    hr = UploadWebResource(m_pwszSourceFile, &m_pstmWebResource,
                    NULL, TRUE, pwszUserName, pwszPassword);
	}

	return hr;
}


////////////////////////////////////////////////////////////////////////
// CDsoDocObject::DoOleCommand
//
//  Calls IOleCommandTarget::Exec on the active object to do a specific
//  command (like Print, SaveCopy, Zoom, etc.).
//
STDMETHODIMP CDsoDocObject::DoOleCommand(DWORD dwOleCmdId, BOOL fPrompt)
{
	HRESULT hr;
	OLECMD cmd = {dwOleCmdId, 0};

	TRACE2("CDsoDocObject::DoOleCommand(cmd=%d, Prompt=%d\n", dwOleCmdId, fPrompt);

 // The server must support IOleCommandTarget, the CmdID being requested, and
 // the command should be enabled. If this is the case, do the command...
	if ((m_pcmdt) && SUCCEEDED(m_pcmdt->QueryStatus(NULL, 1, &cmd, NULL)) && 
		((cmd.cmdf & OLECMDF_SUPPORTED) && (cmd.cmdf & OLECMDF_ENABLED)))
	{
		TRACE1("QueryStatus say supported = 0x%X\n", cmd.cmdf);

		hr = m_pcmdt->Exec(NULL, cmd.cmdID, 
			(fPrompt ? OLECMDEXECOPT_PROMPTUSER : OLECMDEXECOPT_DODEFAULT), NULL, NULL);

		TRACE1("CMT called = 0x%X\n", hr);

	 // If user canceled an Office dialog, that's OK.
		if ((fPrompt) && (hr == 0x80040103))
			hr = S_FALSE; 
	}
	else
	{
		TRACE1("Command Not supportted (%d)\n", cmd.cmdf);
		hr = DSO_E_COMMANDNOTSUPPORTED;
	}

	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::Close
//
//  Close down the object and disconnect us from any handlers/proxies.
//
STDMETHODIMP_(void) CDsoDocObject::Close()
{
	ODS("CDsoDocObject::Close\n");
	HRESULT hr;

	hr = IPDeactivateView();

    if (m_pole)
	{
        hr = m_pole->Close(OLECLOSE_NOSAVE);
		m_pole->SetClientSite(NULL);

		OleLockRunning(m_pole, FALSE, TRUE);

		RELEASE_INTERFACE(m_pole);
	}

	RELEASE_INTERFACE(m_pstgSourceFile);

 // Free any temp file we might have...
	if ((m_pstmWebResource) && (m_pwszSourceFile))
		FPerformShellOp(FO_DELETE, m_pwszSourceFile, NULL);

	RELEASE_INTERFACE(m_pstmWebResource);
	SAFE_FREESTRING(m_pwszWebResource);
	SAFE_FREESTRING(m_pwszSourceFile);

	if (m_fDisconnectOnQuit)
	{
		CoDisconnectObject((IUnknown*)this, 0);
		m_fDisconnectOnQuit = FALSE;
	}

	RELEASE_INTERFACE(m_pstmview);
	RELEASE_INTERFACE(m_pstgfile);

	ClearMergedMenu();
    return;
}


////////////////////////////////////////////////////////////////////////
// CDsoDocObject Notification Functions - The OCX should call these to
//  let the doc site update the object as needed.
//  

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::OnNotifySizeChange
//
//  Resets the size of the site window and tells UI active object to 
//  resize as well. If we are UI active, we'll call ResizeBorder to 
//  re-negotiate toolspace (allow toolbars to shrink and grow), otherwise
//  we'll just set the IP active view rect (minus any toolspace, which
//  should be none since object is not UI active!). 
//
STDMETHODIMP_(void) CDsoDocObject::OnNotifySizeChange(LPRECT prc)
{
	RECT rc;

	SetRect(&rc, 0, 0, (prc->right - prc->left), (prc->bottom - prc->top));
	if (rc.right < 0) rc.right = 0;
	if (rc.top < 0) rc.top = 0;

 // First, resize our frame site window tot he new size (don't change focus)...
	if (m_hwnd)
	{
		m_rcViewRect = *prc;

		SetWindowPos(m_hwnd, NULL, m_rcViewRect.left, m_rcViewRect.top,
			rc.right, rc.bottom, SWP_NOACTIVATE | SWP_NOZORDER);

		UpdateWindow(m_hwnd);
	}

 // If we have an active object (i.e., Document is still UI active) we should
 // tell it of the resize so it can re-negotiate border space...
	if ((m_fObjectUIActive) && (m_pipactive))
	{
		m_pipactive->ResizeBorder(&rc, (IOleInPlaceUIWindow*)&m_xOleInPlaceFrame, TRUE);
	}
	else if ((m_fObjectIPActive) && (m_pdocv))
	{
        rc.left   += m_bwToolSpace.left;   rc.right  -= m_bwToolSpace.right;
        rc.top    += m_bwToolSpace.top;    rc.bottom -= m_bwToolSpace.bottom;
		m_pdocv->SetRect(&rc);
	}

	return;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::OnNotifyAppActivate
//
//  Notify doc object when the top-level frame window goes active and 
//  deactive so it can handle window focs and paiting correctly. Failure
//  to not forward this notification leads to bad behavior.
// 
STDMETHODIMP_(void) CDsoDocObject::OnNotifyAppActivate(BOOL fActive, DWORD dwThreadID)
{
 // This is critical for DocObject servers, so forward these messages
 // when the object is UI active...
	if (m_pipactive)
	{
	 // We should always tell obj server when our frame activates, but
	 // don't tell it to go deactive if the thread gaining focus is 
	 // the server's since our frame may have lost focus because of
	 // a top-level modeless dialog (ex., the RefEdit dialog of Excel)...
		if ((fActive) || (dwThreadID != m_dwObjectThreadID))
			m_pipactive->OnFrameWindowActivate(fActive);
	}

	m_fAppWindowActive = fActive;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::OnNotifyPaletteChanged
//
//  Give the object first chance at realizing a palette. Important on
//  256 color machines, but not so critical these days when everyone is
//  running full 32-bit True Color graphic cards.
// 
STDMETHODIMP_(void) CDsoDocObject::OnNotifyPaletteChanged(HWND hwndPalChg)
{
	if ((m_fObjectUIActive) && (m_hwndUIActiveObj))
		SendMessage(m_hwndUIActiveObj, WM_PALETTECHANGED, (WPARAM)hwndPalChg, 0L);
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::OnNotifyChangeToolState
//
//  This should be called to get object to show/hide toolbars as needed.
//
STDMETHODIMP_(void) CDsoDocObject::OnNotifyChangeToolState(BOOL fShowTools)
{
 // If we want to show/hide toolbars, we can do the following...
	if (fShowTools != (BOOL)m_fDisplayTools)
	{
		OLECMD cmd;
		cmd.cmdID = OLECMDID_HIDETOOLBARS;

		m_fDisplayTools = fShowTools;

	 // Use IOleCommandTarget(OLECMDID_HIDETOOLBARS) to toggle on/off. We have
	 // to check that server supports it and if its state matches our own so
	 // when toggle, we do the correct thing by the user...
		if ((m_pcmdt) && SUCCEEDED(m_pcmdt->QueryStatus(NULL, 1, &cmd, NULL)) && 
			((cmd.cmdf & OLECMDF_SUPPORTED) || (cmd.cmdf & OLECMDF_ENABLED)))
		{
			if (((fShowTools) && (cmd.cmdf & OLECMDF_LATCHED)) ||
				(!(fShowTools) && !(cmd.cmdf & OLECMDF_LATCHED)))
			{
				m_pcmdt->Exec(NULL, OLECMDID_HIDETOOLBARS, OLECMDEXECOPT_PROMPTUSER, NULL, NULL);
			}

		 // There can be focus issues when turning them off, so make sure
		 // the object is on top of the z-order...
			if ((!m_fDisplayTools) && (m_hwndIPObject))
				BringWindowToTop(m_hwndIPObject);

		 // If user toggles off the toolbar while the object is UI active, and
		 // we are not still in activation process, we need to explictly tell Office
		 // apps to also hide the "Web" toolbar. For Office, OLECMDID_HIDETOOLBARS puts
		 // the app into a "web view" which (in some apps) brings up the web toolbar.
		 // Since we intend to have no tools, we have to turn it off by code...
			if ((!m_fDisplayTools) && (m_fObjectUIActive) && (m_fObjectActivateComplete))
				TurnOffWebToolbar();

		}
		else if (m_pdocv)
		{
		 // If we have a DocObj server, but no IOleCommandTarget, do things the hard
		 // way and resize. When server attempts to resize window it will have to
		 // re-negotiate BorderSpace and we fail there, so server "should" not
		 // display its tools (at least that is the idea!<g>)...
			RECT rc; GetClientRect(m_hwnd, &rc);
			MapWindowPoints(m_hwnd, m_hwndCtl, (LPPOINT)&rc, 2);
			OnNotifySizeChange(&rc);
		}
	}
	return;
}


////////////////////////////////////////////////////////////////////////
// CDsoDocObject Protected Functions -- Helpers
//

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::CreateObjectStorage (protected)
//
//  Makes the internal IStorage to host the object and assigns the CLSID.
//
STDMETHODIMP CDsoDocObject::CreateObjectStorage(REFCLSID rclsid)
{
	HRESULT hr;
	LPWSTR pwszName;
	DWORD dwid;
	CHAR szbuf[256];

	if ((!m_pstgroot)) return E_UNEXPECTED;

 // Next, create a new object storage (with unique name) in our
 // temp root storage "file" (this keeps an OLE integrity some servers
 // need to function correctly instead of IP activating from file directly).

 // We make a fake object storage name...
	dwid = ((rclsid.Data1)|GetTickCount());
	wsprintf(szbuf, "OLEDocument%X", dwid);

	if (!(pwszName = ConvertToLPWSTR(szbuf)))
		return E_OUTOFMEMORY;

 // Create the sub-storage...
	hr = m_pstgroot->CreateStorage(pwszName,
		STGM_TRANSACTED | STGM_READWRITE | STGM_SHARE_EXCLUSIVE, 0, 0, &m_pstgfile);

	MemFree(pwszName);

	if (FAILED(hr)) return hr;

 // We'll also create a stream for OLE view settings (non-critical)...
	if (pwszName = ConvertToLPWSTR(szbuf))
	{
		m_pstgroot->CreateStream(pwszName,
			STGM_DIRECT | STGM_READWRITE | STGM_SHARE_EXCLUSIVE, 0, 0, &m_pstmview);
		MemFree(pwszName);
	}

 // Finally, write out the CLSID for the new substorage...
	hr = WriteClassStg(m_pstgfile, rclsid);

 // We are read-only until told otherwise...
	m_fOpenReadOnly = TRUE;
	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::SaveObjectStorage (protected)
//
//  Saves the object back to the internal IStorage.
//
STDMETHODIMP CDsoDocObject::SaveObjectStorage()
{
	HRESULT hr;
	IPersistStorage *pipstg = NULL;

 // Got to have stg and object...
	if ((!m_pstgfile) || (!m_pole))	return E_UNEXPECTED;

 // Ask for IPersistStg and Save (commit changes regardless of result)...
	if (SUCCEEDED(hr = m_pole->QueryInterface(IID_IPersistStorage, (void**)&pipstg)))
	{
		if (SUCCEEDED(hr = pipstg->Save(m_pstgfile, TRUE)))
			hr = pipstg->SaveCompleted(NULL);

		m_pstgfile->Commit(STGC_DEFAULT);
		pipstg->Release();
	}

 // Go ahead and save the view state if view still active (non-critical)...
	if ((m_pdocv) && (m_pstmview))
	{
		m_pdocv->SaveViewState(m_pstmview);
		m_pstmview->Commit(STGC_DEFAULT);
	}

	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::ValidateDocObjectServer (protected)
//
//  Quick validation check to see if CLSID is for DocObject server.
//
//  Officially, the only way to determine if a server supports ActiveX
//  Document embedding is to IP activate it and ask for IOleDocument, 
//  but that means going through the IP process just to fail if IOleDoc
//  is not supported. Therefore, we are going to rely on the server's 
//  honesty in setting its reg keys to include the "DocObject" sub key 
//  under their CLSID.
//
//  This is 99% accurate. For those servers that fail, too bad charlie!
//
STDMETHODIMP CDsoDocObject::ValidateDocObjectServer(REFCLSID rclsid)
{
	HRESULT hr = DSO_E_INVALIDSERVER;
	CHAR  szKeyCheck[256];
	LPSTR pszClsid;
	HKEY  hkey;

 // We don't handle MSHTML even though it is DocObject server...
    const GUID CLSID_MSHTMLDOC = {0x25336920,0x03F9,0x11CF,{0x8F,0xD0,0x00,0xAA,0x00,0x68,0x6F,0x13}};
    if (rclsid == CLSID_MSHTMLDOC) return hr;

 // Convert the CLSID to a string and check for DocObject sub key...
	if (pszClsid = CLSIDtoLPSTR(rclsid))
	{
		wsprintf(szKeyCheck, "CLSID\\%s\\DocObject", pszClsid);

		if (RegOpenKeyEx(HKEY_CLASSES_ROOT, szKeyCheck, 0, KEY_READ, &hkey) == ERROR_SUCCESS)
		{
			hr = S_OK;
			RegCloseKey(hkey);
		}

		MemFree(pszClsid);
	}
	else hr = E_OUTOFMEMORY;

	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::ValidateFileExtension (protected)
//
//  Adds a default extension to save file path if user didn't provide
//  one (uses CLSID and registry to determine default extension).
//
STDMETHODIMP_(BOOL) CDsoDocObject::ValidateFileExtension(WCHAR* pwszFile, WCHAR** ppwszOut)
{
	BOOL   fHasExt = FALSE;
	BOOL   fChangedExt = FALSE;
	LPWSTR pwszT;
	LPSTR  pszClsid;
	DWORD  dw;

	if ((pwszFile) && (dw = lstrlenW(pwszFile)) && (ppwszOut))
	{
		*ppwszOut = NULL;

		pwszT = (pwszFile + dw);
		while ((pwszT != pwszFile) && 
			   (*(--pwszT)) && ((*pwszT != L'\\') && (*pwszT != L'/')))
		{
			if (*pwszT == L'.') fHasExt = TRUE;
		}
		
		if (!(fHasExt) && (pszClsid = CLSIDtoLPSTR(m_clsidObject)))
		{
			HKEY hk;
			DWORD dwType, dwSize;
			LPWSTR pwszExt;
			CHAR szkey[255];
			CHAR szbuf[128];

			wsprintf(szkey, "CLSID\\%s\\DefaultExtension", pszClsid);
			if (RegOpenKeyEx(HKEY_CLASSES_ROOT, szkey, 0, KEY_READ, &hk) == ERROR_SUCCESS)
			{
				LPSTR pszT = szbuf;
				dwSize = 128;

				if (RegQueryValueEx(hk, NULL, 0, &dwType, (BYTE*)pszT, &dwSize) == ERROR_SUCCESS)
				{
					while (*(pszT++) && (*pszT != ','))
						(void)(0);
					*pszT = '\0';
				}
				else lstrcpy(szbuf, ".ole");

				RegCloseKey(hk);
			}
			else lstrcpy(szbuf, ".ole");

			if (pwszExt = ConvertToLPWSTR(szbuf))
				*ppwszOut = CopyStringCat(pwszFile, pwszExt);

			fChangedExt = ((*ppwszOut) != NULL);
			MemFree(pszClsid);
		}
	}

	return fChangedExt;
}


////////////////////////////////////////////////////////////////////////
// CDsoDocObject::CreateRosebudIPP (protected)
//
//  Returns an instance of MSDAIPP (a.k.a., "Rosebud") which is used by
//  Office/Windows for Web Folders (HTTP with DAV/FPSE). 
//
//  This is used to open web resources, lock them for editing, and save
//  changes back up to the server as needed. If the provider is not 
//  installed or cannot be initialized, the functions returns NULL.
//
STDMETHODIMP_(IUnknown*) CDsoDocObject::CreateRosebudIPP()
{
	HRESULT           hr;
	IDBProperties*    pdbprops = NULL;
	IBindResource*    pres    = NULL;
	DBPROPSET         rdbpset;
	DBPROP            rdbp[4];
	BSTR              bstrLock;
	DWORD             dw = 256;
	CHAR              szUserName[256];

	if (FAILED(CoCreateInstance(CLSID_MSDAIPP_BINDER, NULL,
			CLSCTX_INPROC, IID_IDBProperties, (void**)&pdbprops)))
		return NULL;

	bstrLock = (GetUserName(szUserName, &dw) ? ConvertToBSTR(szUserName) : NULL);

	memset(rdbp, 0, sizeof(4 * sizeof(DBPROP)));

	rdbpset.cProperties = 4; 
	rdbpset.guidPropertySet = DBPROPSET_DBINIT;
	rdbpset.rgProperties = rdbp;

	rdbp[0].dwPropertyID = DBPROP_INIT_BINDFLAGS;
	rdbp[0].vValue.vt = VT_I4;
	rdbp[0].vValue.lVal = DBBINDURLFLAG_OUTPUT;

	rdbp[1].dwPropertyID = DBPROP_INIT_LOCKOWNER;
	rdbp[1].vValue.vt = VT_BSTR;
	rdbp[1].vValue.bstrVal = bstrLock;

	rdbp[2].dwPropertyID = DBPROP_INIT_LCID;
	rdbp[2].vValue.vt = VT_I4;
	rdbp[2].vValue.lVal = GetThreadLocale();

	rdbp[3].dwPropertyID = DBPROP_INIT_PROMPT;
	rdbp[3].vValue.vt = VT_I2;
	rdbp[3].vValue.iVal = DBPROMPT_COMPLETE;

	if (pdbprops->SetProperties(1, &rdbpset) == S_OK)
	{
		hr = pdbprops->QueryInterface(IID_IBindResource, (void**)&pres);
	}

	if (bstrLock)
		SysFreeString(bstrLock);

	pdbprops->Release();

	return (IUnknown*)pres;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::DownloadWebResource (protected)
//
//  Downloads the file specified by the URL to the given temp file. Locks
//  the web resource for editing if ppstmKeepForSave is requested.
// 
STDMETHODIMP CDsoDocObject::DownloadWebResource(LPWSTR pwszURL, LPWSTR pwszFile, LPWSTR pwszUsername, LPWSTR pwszPassword, IStream** ppstmKeepForSave)
{
	HRESULT     hr     = E_UNEXPECTED;
	IStream    *pstm   = NULL;
	BYTE       *rgbBuf;
	HANDLE      hFile;
	ULONG       cbRead, cbWritten;
	DWORD       dwStatus, dwBindFlags;

	IBindResource*    pres  = NULL;

	ASSERT(m_punkRosebud);
	if (!m_punkRosebud)	return E_FAIL;

	rgbBuf = new BYTE[10240]; //a 10-k buffer for reading
	if (!rgbBuf) return E_OUTOFMEMORY;

 // Save out the user name and password (if provided) for IAuthenticate...
	if (pwszUsername)
	{
		SAFE_FREESTRING(m_pwszUsername);
		m_pwszUsername = CopyString(pwszUsername);

		SAFE_FREESTRING(m_pwszPassword);
		m_pwszPassword = CopyString(pwszPassword);
	}

 // Use IBindResource::Bind to open an IStream and copy out the date to
 // the file given. This will then be used to load the object from the file...
	if (SUCCEEDED(m_punkRosebud->QueryInterface(IID_IBindResource, (void**)&pres)))
	{
		dwBindFlags = (DBBINDURLFLAG_READ | DBBINDURLFLAG_OUTPUT);
		if (ppstmKeepForSave)
			dwBindFlags |= (DBBINDURLFLAG_WRITE | DBBINDURLFLAG_SHARE_DENY_WRITE);

		if (SUCCEEDED(hr = pres->Bind(NULL, pwszURL, dwBindFlags, DBGUID_STREAM, 
				IID_IStream, (IAuthenticate*)&m_xAuthenticate, NULL, &dwStatus, (IUnknown**)&pstm)))
		{
			LARGE_INTEGER lintStart; lintStart.QuadPart = 0;
			pstm->Seek(lintStart, STREAM_SEEK_SET, NULL);

			if (FOpenLocalFile(pwszFile, GENERIC_WRITE, 0, CREATE_ALWAYS, &hFile))
			{
				while (TRUE)
				{
					if (FAILED(hr = pstm->Read((void*)rgbBuf, 10240, &cbRead)) ||
						(cbRead == 0))
						break;

					if (FALSE == WriteFile(hFile, rgbBuf, cbRead, &cbWritten, NULL))
					{
						hr = HRESULT_FROM_WIN32(GetLastError());
						break;
					}
				}

				CloseHandle(hFile);
			}

			if (ppstmKeepForSave)
				(*ppstmKeepForSave = pstm)->AddRef();

			pstm->Release();
		}

  		pres->Release();
	}

 // Map an OLEDB error to a common "file" error so a user
 // (and VB/VBScript) would better understand... 
	if (FAILED(hr))
	{
		switch (hr)
		{
		case DB_E_NOTFOUND:             hr = STG_E_FILENOTFOUND; break;
		case DB_E_READONLY:
		case DB_E_RESOURCELOCKED:       hr = STG_E_LOCKVIOLATION; break;
		case DB_SEC_E_PERMISSIONDENIED:
		case DB_SEC_E_SAFEMODE_DENIED:  hr = E_ACCESSDENIED; break;
		case DB_E_CANNOTCONNECT:
		case DB_E_TIMEOUT:              hr = E_VBA_NOREMOTESERVER; break;
		case E_NOINTERFACE:             hr = E_UNEXPECTED; break;
		}
	}

	delete [] rgbBuf;
	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::UploadWebResource (protected)
//
//  Uploads the file to a URL. The code can be used two ways:
//
//  1.) If ppstmSave contains a pointer to an existing IStream*, then we 
//      just upload to the existing stream. This allows for normal "Save"
//      on an open web resource.
//
//  2.) If ppstmSave is NULL (or contains a NULL IStream*), we create a new
//      web resource at the location given by pwszURLSaveTo and save to its
//      stream. If ppstmSave is passed, we return the new IStream* to the
//      caller who can then use it to do the other type of save next time.
//
STDMETHODIMP CDsoDocObject::UploadWebResource(LPWSTR pwszFile, IStream** ppstmSave, LPWSTR pwszURLSaveTo, BOOL fOverwriteFile, LPWSTR pwszUsername, LPWSTR pwszPassword)
{
	HRESULT     hr     = E_UNEXPECTED;
	ICreateRow *pcrow  = NULL;
	IStream    *pstm   = NULL;
	BYTE       *rgbBuf;
	HANDLE      hFile;
	BOOL        fstmIn = FALSE;
	ULONG       cbRead, cbWritten;
	DWORD       dwStatus, dwBindFlags;

	ASSERT(m_punkRosebud);
	if (!m_punkRosebud)	return E_FAIL;

 // Check if this a "Save" on existing IStream* and jump to loop...
	if ((ppstmSave) && (fstmIn = (BOOL)(pstm = *ppstmSave)))
		goto uploadfrominstm;

 // Save the user name and password (if provided) for IAuthenticate...
	if (pwszUsername)
	{
		SAFE_FREESTRING(m_pwszUsername);
		m_pwszUsername = CopyString(pwszUsername);

		SAFE_FREESTRING(m_pwszPassword);
		m_pwszPassword = CopyString(pwszPassword);
	}

 // Check the URL string and ask for ICreateRow (to make new web resource)... 
	if (!(pwszURLSaveTo) || !LooksLikeHTTP(pwszURLSaveTo) ||
		FAILED(m_punkRosebud->QueryInterface(IID_ICreateRow, (void**)&pcrow)))
		return hr;

	dwBindFlags = ( DBBINDURLFLAG_READ | 
					DBBINDURLFLAG_WRITE | 
					DBBINDURLFLAG_SHARE_DENY_WRITE | 
                    (fOverwriteFile ? DBBINDURLFLAG_OVERWRITE : 0));

	if (SUCCEEDED(hr = pcrow->CreateRow(NULL, pwszURLSaveTo, dwBindFlags, DBGUID_STREAM,
			IID_IStream, (IAuthenticate*)&m_xAuthenticate, NULL, &dwStatus, NULL, (IUnknown**)&pstm)))
	{

	 // Once we are here, we have a stream (either handed in or opened from above).
	 // We just loop through and read from the file to the stream...
uploadfrominstm:
		if (rgbBuf = new BYTE[10240]) //a 10-k buffer for reading
		{
			if (FOpenLocalFile(pwszFile, GENERIC_READ, FILE_SHARE_READ, OPEN_EXISTING, &hFile))
			{
				LARGE_INTEGER lintStart; lintStart.QuadPart = 0;
				pstm->Seek(lintStart, STREAM_SEEK_SET, NULL);

				while (TRUE)
				{
					if (FALSE == ReadFile(hFile, rgbBuf, 10240, &cbRead, NULL))
					{
						hr = HRESULT_FROM_WIN32(GetLastError());
						break;
					}

					if (0 == cbRead) break;

					if (FAILED(hr = pstm->Write((void*)rgbBuf, cbRead, &cbWritten)))
						break;
				}

			 // Need to commit the changes to make it official...
				if (SUCCEEDED(hr))
					hr = pstm->Commit(STGC_DEFAULT);

				CloseHandle(hFile);
			}
            else hr = HRESULT_FROM_WIN32(GetLastError());

			delete [] rgbBuf;
		}
        else hr = E_OUTOFMEMORY;

	 // If we are not using a passed in IStream (and therefore created one), we
	 // should AddRef and pass back (if caller asked us to)...
		if (!fstmIn)
		{
			if (SUCCEEDED(hr) && (ppstmSave) && (!(*ppstmSave)))
			{
				(*ppstmSave = pstm)->AddRef();
			}
			pstm->Release();
		}

	}

 // Map an OLEDB error to a common "file" error so a user
 // (and VB/VBScript) would better understand... 
    if (FAILED(hr))
	{
		switch (hr)
		{
		case DB_E_RESOURCEEXISTS:       hr = STG_E_FILEALREADYEXISTS; break;
		case DB_E_NOTFOUND:             hr = STG_E_PATHNOTFOUND; break;
		case DB_E_READONLY:
		case DB_E_RESOURCELOCKED:       hr = STG_E_LOCKVIOLATION; break;
		case DB_SEC_E_PERMISSIONDENIED:
		case DB_SEC_E_SAFEMODE_DENIED:  hr = E_ACCESSDENIED; break;
		case DB_E_CANNOTCONNECT:
		case DB_E_TIMEOUT:              hr = E_VBA_NOREMOTESERVER; break;
		case DB_E_OUTOFSPACE:           hr = STG_E_MEDIUMFULL; break;
		case E_NOINTERFACE:             hr = E_UNEXPECTED; break;
		}
	}

	if (pcrow)
		pcrow->Release();

	return hr;
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::TurnOffWebToolbar (protected)
//
//  This function "turns off" the Web toolbar used by Office apps to 
//  do in-site navigation. The problem is when toggling tools off the
//  bar may still be visible, so we have to explicitly turn it off if
//  we want a true "no tool" state.
//
STDMETHODIMP_(void) CDsoDocObject::TurnOffWebToolbar()
{
	IDispatch *pdisp;
	VARIANT    vtT[5];

	if ((m_pipactive) && 
		(SUCCEEDED(m_pipactive->QueryInterface(IID_IDispatch, (void**)&pdisp))))
	{
		if (SUCCEEDED(AutoDispInvoke(pdisp, 
				L"CommandBars", 0, DISPATCH_PROPERTYGET, 0, NULL, &vtT[0])))
		{
			ASSERT(vtT[0].vt == VT_DISPATCH);
			vtT[1].vt = VT_BSTR; vtT[1].bstrVal = SysAllocString(L"Web");

			if (SUCCEEDED(AutoDispInvoke(vtT[0].pdispVal, 
				L"Item", 0, DISPATCH_PROPERTYGET, 1, &vtT[1], &vtT[2])))
			{
				ASSERT(vtT[2].vt == VT_DISPATCH);
				vtT[3].vt = VT_BOOL; vtT[3].boolVal = 0;
				AutoDispInvoke(vtT[2].pdispVal, 
					L"Visible", 0, DISPATCH_PROPERTYPUT, 1, &vtT[3], &vtT[2]);
				VariantClear(&vtT[2]);
			}

			VariantClear(&vtT[1]);
			VariantClear(&vtT[0]);
		}

		pdisp->Release();
	}

}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::ClearMergedMenu (protected)
//
//  Frees the merged menu set by host.
//
STDMETHODIMP_(void) CDsoDocObject::ClearMergedMenu()
{	
	if (m_hMenuMerged)
	{
		RemoveMenu(m_hMenuMerged, 0, MF_BYPOSITION);
		DestroyMenu(m_hMenuMerged);
		m_hMenuMerged = NULL;
	}
}

////////////////////////////////////////////////////////////////////////
// CDsoDocObject::OnDraw (protected)
//
//  Site drawing (does nothing in this version).
//
STDMETHODIMP_(void) CDsoDocObject::OnDraw(DWORD dvAspect, HDC hdcDraw, LPRECT prcBounds, LPRECT prcWBounds, HDC hicTargetDev, BOOL fOptimize)
{
	// Don't have to draw anything, object does all this because we are
	// always UI active. If we allowed for multiple objects and had some
	// non-UI active, we would have to do some drawing, but that will not
	// happen in this sample.
}


////////////////////////////////////////////////////////////////////////
//
// ActiveX Document Site Interfaces
//

////////////////////////////////////////////////////////////////////////
//
// CDsoDocObject IUnknown Interface Methods
//
//   STDMETHODIMP         QueryInterface(REFIID riid, void ** ppv);
//   STDMETHODIMP_(ULONG) AddRef(void);
//   STDMETHODIMP_(ULONG) Release(void);
//
STDMETHODIMP CDsoDocObject::QueryInterface(REFIID riid, void** ppv)
{
	ODS("CDsoDocObject::QueryInterface\n");
	CHECK_NULL_RETURN(ppv, E_POINTER);
	
	HRESULT hr = S_OK;

	if (IID_IUnknown == riid)
	{
		*ppv = (IUnknown*)this;
	}
	else if (IID_IOleClientSite == riid)
	{
		*ppv = (IOleClientSite*)&m_xOleClientSite;
	}
	else if ((IID_IOleInPlaceSite == riid) || (IID_IOleWindow == riid))
	{
		*ppv = (IOleInPlaceSite*)&m_xOleInPlaceSite;
	}
	else if (IID_IOleDocumentSite == riid)
	{
		*ppv = (IOleDocumentSite*)&m_xOleDocumentSite;
	}
	else if ((IID_IOleInPlaceFrame == riid) || (IID_IOleInPlaceUIWindow == riid))
	{
		*ppv = (IOleInPlaceFrame*)&m_xOleInPlaceFrame;
	}
	else if (IID_IOleCommandTarget == riid)
	{
		*ppv = (IOleCommandTarget*)&m_xOleCommandTarget;
	}
	else if (IID_IAuthenticate == riid)
	{
		*ppv = (IAuthenticate*)&m_xAuthenticate;
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

STDMETHODIMP_(ULONG) CDsoDocObject::AddRef(void)
{
	TRACE1("CDsoDocObject::AddRef - %d\n", m_cRef + 1);
    return ++m_cRef;
}

STDMETHODIMP_(ULONG) CDsoDocObject::Release(void)
{
	TRACE1("CDsoDocObject::Release - %d\n", m_cRef - 1);
	return --m_cRef;
}


////////////////////////////////////////////////////////////////////////
//
// CDsoDocObject::XOleClientSite -- IOleClientSite Implementation
//
//	 STDMETHODIMP SaveObject(void);
//	 STDMETHODIMP GetMoniker(DWORD dwAssign, DWORD dwWhich, LPMONIKER* ppmk);
//	 STDMETHODIMP GetContainer(LPOLECONTAINER* ppContainer);
//	 STDMETHODIMP ShowObject(void);
//	 STDMETHODIMP OnShowWindow(BOOL fShow);
//	 STDMETHODIMP RequestNewObjectLayout(void);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoDocObject, OleClientSite)

STDMETHODIMP CDsoDocObject::XOleClientSite::SaveObject(void)
{
    ODS("CDsoDocObject::XOleClientSite::SaveObject\n");
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleClientSite::GetMoniker(DWORD dwAssign, DWORD dwWhichMoniker, IMoniker** ppmk)
{
    ODS("CDsoDocObject::XOleClientSite::GetMoniker\n");
	if (ppmk) *ppmk = NULL;
	return E_NOTIMPL;
}

STDMETHODIMP CDsoDocObject::XOleClientSite::GetContainer(IOleContainer** ppContainer)
{
    ODS("CDsoDocObject::XOleClientSite::GetContainer\n");
	if (ppContainer) *ppContainer = NULL;
	return E_NOINTERFACE;
}

STDMETHODIMP CDsoDocObject::XOleClientSite::ShowObject(void)
{
    ODS("CDsoDocObject::XOleClientSite::ShowObject\n");
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleClientSite::OnShowWindow(BOOL fShow)
{
    ODS("CDsoDocObject::XOleClientSite::OnShowWindow\n");
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleClientSite::RequestNewObjectLayout(void)
{
    ODS("CDsoDocObject::XOleClientSite::RequestNewObjectLayout\n");
	return E_NOTIMPL;
}

////////////////////////////////////////////////////////////////////////
//
// CDsoDocObject::XOleInPlaceSite -- IOleInPlaceSite Implementation
//
//	 STDMETHODIMP GetWindow(HWND* phWnd);
//	 STDMETHODIMP ContextSensitiveHelp(BOOL fEnterMode);
//	 STDMETHODIMP CanInPlaceActivate(void);
//	 STDMETHODIMP OnInPlaceActivate(void);
//	 STDMETHODIMP OnUIActivate(void);
//	 STDMETHODIMP GetWindowContext(LPOLEINPLACEFRAME* ppIIPFrame, LPOLEINPLACEUIWINDOW* ppIIPUIWindow, LPRECT prcPos, LPRECT prcClip, LPOLEINPLACEFRAMEINFO pFI);
//	 STDMETHODIMP Scroll(SIZE sz);
//	 STDMETHODIMP OnUIDeactivate(BOOL fUndoable);
//	 STDMETHODIMP OnInPlaceDeactivate(void);
//	 STDMETHODIMP DiscardUndoState(void);
//	 STDMETHODIMP DeactivateAndUndo(void);
//	 STDMETHODIMP OnPosRectChange(LPCRECT prcPos);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoDocObject, OleInPlaceSite)

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::GetWindow(HWND* phwnd)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceSite);
    ODS("CDsoDocObject::XOleInPlaceSite::GetWindow\n");
	if (phwnd) *phwnd = pThis->m_hwnd;
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::ContextSensitiveHelp(BOOL fEnterMode)
{
    ODS("CDsoDocObject::XOleInPlaceSite::ContextSensitiveHelp\n");
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::CanInPlaceActivate(void)
{
    ODS("CDsoDocObject::XOleInPlaceSite::CanInPlaceActivate\n");
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::OnInPlaceActivate(void)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceSite);
    ODS("CDsoDocObject::XOleInPlaceSite::OnInPlaceActivate\n");

	if ((!pThis->m_pole) || 
		FAILED(pThis->m_pole->QueryInterface(IID_IOleInPlaceObject, (void **)&(pThis->m_pipobj))))
		return E_UNEXPECTED;

    pThis->m_fObjectIPActive = TRUE;
    return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::OnUIActivate(void)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceSite);
    ODS("CDsoDocObject::XOleInPlaceSite::OnUIActivate\n");
    pThis->m_fObjectUIActive = TRUE;
	pThis->m_pipobj->GetWindow(&(pThis->m_hwndIPObject));
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::GetWindowContext(IOleInPlaceFrame** ppFrame,
	IOleInPlaceUIWindow** ppDoc, LPRECT lprcPosRect, LPRECT lprcClipRect, LPOLEINPLACEFRAMEINFO lpFrameInfo)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceSite);
    ODS("CDsoDocObject::XOleInPlaceSite::GetWindowContext\n");

	if (ppFrame)
	{
		*ppFrame = &(pThis->m_xOleInPlaceFrame);
		(*ppFrame)->AddRef();
	}

    if (ppDoc)
		*ppDoc = NULL;
    
    if (lprcPosRect)
		*lprcPosRect = pThis->m_rcViewRect;

    if (lprcClipRect)
		*lprcClipRect = *lprcPosRect;

	memset(lpFrameInfo, 0, sizeof(OLEINPLACEFRAMEINFO));
    lpFrameInfo->cb = sizeof(OLEINPLACEFRAMEINFO);
    lpFrameInfo->hwndFrame = pThis->m_hwnd;

    return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::Scroll(SIZE sz)
{
    ODS("CDsoDocObject::XOleInPlaceSite::Scroll\n");
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::OnUIDeactivate(BOOL fUndoable)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceSite);
    ODS("CDsoDocObject::XOleInPlaceSite::OnUIDeactivate\n");

    pThis->m_fObjectUIActive = FALSE;
	pThis->m_xOleInPlaceFrame.SetMenu(NULL, NULL, NULL);
    SetFocus(pThis->m_hwnd);

    return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::OnInPlaceDeactivate(void)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceSite);
    ODS("CDsoDocObject::XOleInPlaceSite::OnInPlaceDeactivate\n");

	pThis->m_fObjectIPActive = FALSE;
	pThis->m_hwndIPObject = NULL;
	RELEASE_INTERFACE((pThis->m_pipobj));

	pThis->m_fObjectActivateComplete = FALSE;
    return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::DiscardUndoState(void)
{
    ODS("CDsoDocObject::XOleInPlaceSite::DiscardUndoState\n");
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::DeactivateAndUndo(void)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceSite);
    ODS("CDsoDocObject::XOleInPlaceSite::DeactivateAndUndo\n");
    if (pThis->m_pipobj) pThis->m_pipobj->InPlaceDeactivate();
    return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceSite::OnPosRectChange(LPCRECT lprcPosRect)
{
    ODS("CDsoDocObject::XOleInPlaceSite::OnPosRectChange\n");
	return S_OK;
}

////////////////////////////////////////////////////////////////////////
//
// CDsoDocObject::XOleDocumentSite -- IOleDocumentSite Implementation
//
//	 STDMETHODIMP ActivateMe(IOleDocumentView* pView);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoDocObject, OleDocumentSite)

STDMETHODIMP CDsoDocObject::XOleDocumentSite::ActivateMe(IOleDocumentView* pView)
{
	METHOD_PROLOGUE(CDsoDocObject, OleDocumentSite);
    ODS("CDsoDocObject::XOleDocumentSite::ActivateMe\n");

	HRESULT             hr;
    IOleDocument*       pmsodoc;
    
 // If we're passed a NULL view pointer, then try to get one from
 // the document object (the object within us).
    if (pView)
	{
	 // Make sure that the view has our client site
		pView->SetInPlaceSite((IOleInPlaceSite*)&(pThis->m_xOleInPlaceSite));
		pView->AddRef();
	}
	else
    {
        if ((!(pThis->m_pole)) || 
			FAILED(pThis->m_pole->QueryInterface(IID_IOleDocument, (void **)&pmsodoc)))
            return E_FAIL;

		hr = pmsodoc->CreateView((IOleInPlaceSite*)&(pThis->m_xOleInPlaceSite),
							pThis->m_pstmview, 0, &pView);

		pmsodoc->Release();

		if (FAILED(hr)) return hr;
	}

    pThis->m_pdocv = pView;

 // Get a command target (if available)...
    pView->QueryInterface(IID_IOleCommandTarget, (void**)&(pThis->m_pcmdt));

 // This sets up toolbars and menus first    
	if (SUCCEEDED(hr = pView->UIActivate(TRUE)))
	{

	 // Set the window size sensitive to new toolbars
		pView->SetRect(&(pThis->m_rcViewRect));

	 // Makes it all active
		pView->Show(TRUE);

		pThis->m_fAppWindowActive = TRUE;

	 // Toogle tools off if that's what user wants...
		if (!(pThis->m_fDisplayTools))
		{
			pThis->m_fDisplayTools = TRUE;
			pThis->OnNotifyChangeToolState(FALSE);
		}

		pThis->m_fObjectActivateComplete = TRUE;
	}

    return hr;
}

////////////////////////////////////////////////////////////////////////
//
// CDsoDocObject::XOleInPlaceFrame -- IOleInPlaceFrame Implementation
//
//   STDMETHODIMP GetWindow(HWND* phWnd);
//   STDMETHODIMP ContextSensitiveHelp(BOOL fEnterMode);
//   STDMETHODIMP GetBorder(LPRECT prcBorder);
//   STDMETHODIMP RequestBorderSpace(LPCBORDERWIDTHS pBW);
//   STDMETHODIMP SetBorderSpace(LPCBORDERWIDTHS pBW);
//   STDMETHODIMP SetActiveObject(LPOLEINPLACEACTIVEOBJECT pIIPActiveObj, LPCOLESTR pszObj);
//   STDMETHODIMP InsertMenus(HMENU hMenu, LPOLEMENUGROUPWIDTHS pMGW);
//   STDMETHODIMP SetMenu(HMENU hMenu, HOLEMENU hOLEMenu, HWND hWndObj);
//   STDMETHODIMP RemoveMenus(HMENU hMenu);
//   STDMETHODIMP SetStatusText(LPCOLESTR pszText);
//   STDMETHODIMP EnableModeless(BOOL fEnable);
//   STDMETHODIMP TranslateAccelerator(LPMSG pMSG, WORD wID);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoDocObject, OleInPlaceFrame)

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::GetWindow(HWND* phWnd)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceFrame);
    ODS("CDsoDocObject::XOleInPlaceFrame::GetWindow\n");
	return pThis->m_xOleInPlaceSite.GetWindow(phWnd);
}

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::ContextSensitiveHelp(BOOL fEnterMode)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceFrame);
    ODS("CDsoDocObject::XOleInPlaceFrame::ContextSensitiveHelp\n");
	return pThis->m_xOleInPlaceSite.ContextSensitiveHelp(fEnterMode);
}


STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::GetBorder(LPRECT prcBorder)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceFrame);
    ODS("CDsoDocObject::XOleInPlaceFrame::GetBorder\n");
	CHECK_NULL_RETURN(prcBorder, E_POINTER);

 // If we don't allow Toolspace, and we are already active, give
 // no space for tools (ie, hide toolabrs), otherwise give as much we can...
	if (!(pThis->m_fDisplayTools) && (pThis->m_pipactive))
		SetRectEmpty(prcBorder);
	else
		GetClientRect(pThis->m_hwnd, prcBorder);

	TRACE_LPRECT("prcBorder", prcBorder);
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::RequestBorderSpace(LPCBORDERWIDTHS pBW)
{
    ODS("CDsoDocObject::XOleInPlaceFrame::RequestBorderSpace\n");
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::SetBorderSpace(LPCBORDERWIDTHS pBW)
{
    RECT rc;

	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceFrame);
    ODS("CDsoDocObject::XOleInPlaceFrame::SetBorderSpace\n");
	
	GetClientRect(pThis->m_hwnd, &rc);
	SetRectEmpty((RECT*)&(pThis->m_bwToolSpace));

    if ((pThis->m_fDisplayTools) && (pBW))
    {
		pThis->m_bwToolSpace = *pBW;

        rc.left   += pBW->left;   rc.right  -= pBW->right;
        rc.top    += pBW->top;    rc.bottom -= pBW->bottom;
    }

 // Save the current view RECT (space minus tools)...
    pThis->m_rcViewRect = rc;

 // Update the active document (if alive)...
    if (pThis->m_pdocv)
        pThis->m_pdocv->SetRect(&(pThis->m_rcViewRect));

    return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::SetActiveObject(LPOLEINPLACEACTIVEOBJECT pIIPActiveObj, LPCOLESTR pszObj)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceFrame);
    ODS("CDsoDocObject::XOleInPlaceFrame::SetActiveObject\n");

	RELEASE_INTERFACE((pThis->m_pipactive));
	pThis->m_hwndUIActiveObj = NULL;
	pThis->m_dwObjectThreadID = 0;

    if (pIIPActiveObj)
	{
		pThis->m_pipactive = pIIPActiveObj;
		pIIPActiveObj->AddRef();
		pIIPActiveObj->GetWindow(&(pThis->m_hwndUIActiveObj));
		pThis->m_dwObjectThreadID = GetWindowThreadProcessId(pThis->m_hwndUIActiveObj, NULL);
	}

    return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::InsertMenus(HMENU hMenu, LPOLEMENUGROUPWIDTHS pMGW)
{
    ODS("CDsoDocObject::XOleInPlaceFrame::InsertMenus\n");
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::SetMenu(HMENU hMenu, HOLEMENU hOLEMenu, HWND hWndObj)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceFrame);
    ODS("CDsoDocObject::XOleInPlaceFrame::SetMenu\n");

    if (hMenu)
    {
		pThis->m_hMenuActive = hMenu;
		pThis->m_holeMenu = hOLEMenu;
		pThis->m_hwndMenuObj = hWndObj;
    }
    else
    {
		pThis->m_hMenuActive = NULL;
		pThis->m_holeMenu = NULL;
		pThis->m_hwndMenuObj = NULL;
    }

	pThis->ClearMergedMenu();
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::RemoveMenus(HMENU hMenu)
{
    ODS("CDsoDocObject::XOleInPlaceFrame::RemoveMenus\n");
	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::SetStatusText(LPCOLESTR pszText)
{
	//METHOD_PROLOGUE(CDsoDocObject, OleInPlaceFrame);
    ODS("CDsoDocObject::XOleInPlaceFrame::SetStatusText\n");

	/*HWND hT, hTP;
	hT = GetFocus();
	while (hTP = GetParent(hT))
	{
		hT = hTP;
		if (hT == pThis->m_hwndCtl)
		{
			PostMessage(pThis->m_hwndCtl, WM_APP+302, 0, 0);
			break;
		}
	}*/

	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::EnableModeless(BOOL fEnable)
{
	METHOD_PROLOGUE(CDsoDocObject, OleInPlaceFrame);
    ODS("CDsoDocObject::XOleInPlaceFrame::EnableModeless\n");

	pThis->m_fObjectInModalCondition = !fEnable;
	SendMessage(pThis->m_hwndCtl, WM_APP+301, fEnable, 0);

	return S_OK;
}

STDMETHODIMP CDsoDocObject::XOleInPlaceFrame::TranslateAccelerator(LPMSG pMSG, WORD wID)
{
    ODS("CDsoDocObject::XOleInPlaceFrame::TranslateAccelerator\n");
	return S_FALSE;
}

////////////////////////////////////////////////////////////////////////
//
// CDsoDocObject::XOleCommandTarget -- IOleCommandTarget Implementation
//
//   STDMETHODIMP QueryStatus(const GUID *pguidCmdGroup, ULONG cCmds, OLECMD prgCmds[], OLECMDTEXT *pCmdText);
//   STDMETHODIMP Exec(const GUID *pguidCmdGroup, DWORD nCmdID, DWORD nCmdexecopt, VARIANTARG *pvaIn, VARIANTARG *pvaOut);            
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoDocObject, OleCommandTarget)

STDMETHODIMP CDsoDocObject::XOleCommandTarget::QueryStatus(const GUID *pguidCmdGroup, ULONG cCmds, OLECMD prgCmds[], OLECMDTEXT *pCmdText)
{
	HRESULT hr = OLECMDERR_E_UNKNOWNGROUP;
	METHOD_PROLOGUE(CDsoDocObject, OleCommandTarget);
	if (pThis->m_pcmdCtl)
		hr = pThis->m_pcmdCtl->QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
	return hr;
}

STDMETHODIMP CDsoDocObject::XOleCommandTarget::Exec(const GUID *pguidCmdGroup, DWORD nCmdID, DWORD nCmdexecopt, VARIANTARG *pvaIn, VARIANTARG *pvaOut)
{
	HRESULT hr = OLECMDERR_E_NOTSUPPORTED;
	METHOD_PROLOGUE(CDsoDocObject, OleCommandTarget);
	if (pThis->m_pcmdCtl)
		hr = pThis->m_pcmdCtl->Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
	return hr;
}

////////////////////////////////////////////////////////////////////////
//
// CDsoDocObject::XAuthenticate -- IAuthenticate Implementation
//
//   STDMETHODIMP Authenticate(HWND *phwnd, LPWSTR *pszUsername, LPWSTR *pszPassword);
//
IMPLEMENT_INTERFACE_UNKNOWN(CDsoDocObject, Authenticate)

STDMETHODIMP CDsoDocObject::XAuthenticate::Authenticate(HWND *phwnd, LPWSTR *pszUsername, LPWSTR *pszPassword)
{
	METHOD_PROLOGUE(CDsoDocObject, Authenticate);
    ODS("CDsoDocObject::XAuthenticate::Authenticate\n");
	if (phwnd) *phwnd = ((pThis->m_pwszUsername) ? (HWND)INVALID_HANDLE_VALUE : pThis->m_hwndCtl);
	if (pszUsername) *pszUsername = pThis->m_pwszUsername;
	if (pszPassword) *pszPassword = pThis->m_pwszPassword;
    return S_OK;
}


////////////////////////////////////////////////////////////////////////
// CDsoDocObject::FrameWindowProc
//
//  Site window procedure. Not much to do here except forward focus.
//
STDMETHODIMP_(LRESULT) CDsoDocObject::FrameWindowProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	CDsoDocObject* pbndr = (CDsoDocObject*)GetWindowLong(hwnd, GWL_USERDATA);
	if (pbndr)
	{
		switch (msg)
		{
		case WM_PAINT:
			{
				PAINTSTRUCT ps;
				RECT rc; GetClientRect(hwnd, &rc);
				BeginPaint(hwnd, &ps);
				pbndr->OnDraw(DVASPECT_CONTENT, ps.hdc, (RECT*)&rc, NULL, NULL, TRUE);
				EndPaint(hwnd, &ps);
			}
			break;

		case WM_NCDESTROY:
			SetWindowLong(hwnd, GWL_USERDATA, 0);
			pbndr->m_hwnd = NULL;
			break;

		case WM_SETFOCUS:
			if (pbndr->m_hwndUIActiveObj)
				SetFocus(pbndr->m_hwndUIActiveObj);
			break;

		}

	}

	return DefWindowProc(hwnd, msg, wParam, lParam);
}



