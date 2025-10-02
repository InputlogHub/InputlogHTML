/***************************************************************************
 * UTILITIES.CPP
 *
 * Shared helper functions and routines.
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
// Win32 Unicode API wrappers
//
//  This project is compiled using ANSI in order for it to run on Win9x
//  machines that don't support Unicode. However, in order to try to keep
//  the code language/locale neutral we need to use Unicode in places where
//  we pass file names and paths. So we are using these wrappers to convert
//  Unicode file info to ANSI only when needed (i.e., running on Win9x).
//
//  To keep it simple we'll be using the following static members to hold
//  on to the function pointers (all are functions in Kernel32 which should
//  always be loaded, hence no need to call LoadLibrary).
//
typedef DWORD  (WINAPI *PFN_GETFILEATTRW)(LPCWSTR);
typedef HANDLE (WINAPI *PFN_CREATEFILEW)(LPCWSTR, DWORD, DWORD, LPSECURITY_ATTRIBUTES, DWORD, DWORD, HANDLE);
typedef BOOL   (WINAPI *PFN_MOVEFILEW)(LPCWSTR, LPCWSTR);
typedef BOOL   (WINAPI *PFN_DELETEFILEW)(LPCWSTR);
typedef DWORD  (WINAPI *PFN_GETMODENAMEW)(HMODULE, LPCWSTR, DWORD);
typedef INT    (WINAPI *PFN_CMPSTRINGW)(LCID, DWORD, LPCWSTR, INT, LPCWSTR, INT);

typedef INT    (WINAPI *PFN_DRAWTEXTW)(HDC, LPCWSTR, INT, LPRECT, UINT);
typedef LONG   (WINAPI *PFN_REGSETVALEXW)(HKEY, LPCWSTR, DWORD, DWORD, CONST BYTE*, DWORD);

static BOOL             v_bRunningOnWin9x  = FALSE;
static PFN_GETFILEATTRW v_pfnGetFileAttrib = NULL;
static PFN_CREATEFILEW  v_pfnCreateFile    = NULL;
static PFN_MOVEFILEW    v_pfnMoveFile      = NULL;
static PFN_DELETEFILEW  v_pfnDeleteFile    = NULL;
static PFN_GETMODENAMEW v_pfnGetModuleName = NULL;
static PFN_CMPSTRINGW   v_pfnCompareString = NULL;
static PFN_DRAWTEXTW    v_pfnDrawText      = NULL;
static PFN_REGSETVALEXW v_pfnRegSetValueEx = NULL;


////////////////////////////////////////////////////////////////////////
// FEnsureProcAddresses
//
//  Gets the proc addresses we'll use. Note we are using the same variable
//  for both the ANSI and Unicode version of a function, and then cast the
//  ANSI string as a LPWSTR to trick the compiler. This is not the best coding
//  practice, but is efficient since a LPWSTR and LPSTR are both 32 bits, so 
//  stack frame is the same. The v_bRunningOnWin9x flag tells us which version
//  we are about to call.
//
static BOOL FEnsureProcAddresses()
{
    HMODULE hDll;

 // If any are NULL, try and get the function pointers...
    if (!(v_pfnGetFileAttrib) ||  !(v_pfnCreateFile) || 
        !(v_pfnMoveFile)      ||  !(v_pfnDeleteFile) || !(v_pfnGetModuleName))
    {
        hDll = GetModuleHandle("kernel32.dll");
        CHECK_NULL_RETURN(hDll, FALSE);

     // Check for Win9x system...
        v_bRunningOnWin9x = (GetVersion() & 0x80000000);

     // GetFileAttributes...
        v_pfnGetFileAttrib = (PFN_GETFILEATTRW)GetProcAddress(hDll, 
                        (v_bRunningOnWin9x ? "GetFileAttributesA" : "GetFileAttributesW"));
        CHECK_NULL_RETURN(v_pfnGetFileAttrib, FALSE);

     // CreateFile...
        v_pfnCreateFile = (PFN_CREATEFILEW)GetProcAddress(hDll, 
                        (v_bRunningOnWin9x ? "CreateFileA" : "CreateFileW"));
        CHECK_NULL_RETURN(v_pfnCreateFile, FALSE);

     // MoveFile...
        v_pfnMoveFile = (PFN_MOVEFILEW)GetProcAddress(hDll, 
                        (v_bRunningOnWin9x ? "MoveFileA" : "MoveFileW"));
        CHECK_NULL_RETURN(v_pfnMoveFile, FALSE);

     // DeleteFile...
        v_pfnDeleteFile = (PFN_DELETEFILEW)GetProcAddress(hDll, 
                        (v_bRunningOnWin9x ? "DeleteFileA" : "DeleteFileW"));
        CHECK_NULL_RETURN(v_pfnDeleteFile, FALSE);

     // GetModuleFileName
        v_pfnGetModuleName = (PFN_GETMODENAMEW)GetProcAddress(hDll, 
                        (v_bRunningOnWin9x ? "GetModuleFileNameA" : "GetModuleFileNameW"));
        CHECK_NULL_RETURN(v_pfnGetModuleName, FALSE);
       
     // CompareString
        v_pfnCompareString = (PFN_CMPSTRINGW)GetProcAddress(hDll, 
                        (v_bRunningOnWin9x ? "CompareStringA" : "CompareStringW"));
        CHECK_NULL_RETURN(v_pfnCompareString, FALSE);
       
    }

    if (!(v_pfnDrawText))
    {
        hDll = GetModuleHandle("user32.dll");
        CHECK_NULL_RETURN(hDll, FALSE);

     // DrawText
        v_pfnDrawText = (PFN_DRAWTEXTW)GetProcAddress(hDll, 
                        (v_bRunningOnWin9x ? "DrawTextA" : "DrawTextW"));
        CHECK_NULL_RETURN(v_pfnDrawText, FALSE);
	}

	if (!(v_pfnRegSetValueEx))
    {
        hDll = GetModuleHandle("advapi32.dll");
        CHECK_NULL_RETURN(hDll, FALSE);

     // DrawText
        v_pfnRegSetValueEx = (PFN_REGSETVALEXW)GetProcAddress(hDll, 
                        (v_bRunningOnWin9x ? "RegSetValueExA" : "RegSetValueExW"));
        CHECK_NULL_RETURN(v_pfnRegSetValueEx, FALSE);
	}

    return TRUE;
}

////////////////////////////////////////////////////////////////////////
// FFileExists
//
//  Returns TRUE if the given file exists. Does not handle URLs.
//
STDAPI_(BOOL) FFileExists(WCHAR* wzPath)
{
    DWORD dw = 0xFFFFFFFF;

    if (!FEnsureProcAddresses())
        return FALSE;

 // On Win9x you have to use ANSI...
    if (v_bRunningOnWin9x)
	{
		LPSTR psz = ConvertToANSI(wzPath);
        if (psz)
            dw = (v_pfnGetFileAttrib)((LPWSTR)psz);
		MemFree(psz);
	}
    else
    {
        dw = (v_pfnGetFileAttrib)(wzPath);
    }

    return (dw != 0xFFFFFFFF);
}

////////////////////////////////////////////////////////////////////////
// FOpenLocalFile
//
//  Returns TRUE if the file can be opened with the access required.
//  Use the handle for ReadFile/WriteFile operations as normal.
//
STDAPI_(BOOL) FOpenLocalFile(WCHAR* wzFilePath, DWORD dwAccess, DWORD dwShareMode, DWORD dwCreate, HANDLE* phFile)
{
    CHECK_NULL_RETURN(phFile, FALSE);
    *phFile = INVALID_HANDLE_VALUE;

    if (!FEnsureProcAddresses())
        return FALSE;

 // On Win9x you have to use ANSI...
    if (v_bRunningOnWin9x)
    {
        LPSTR psz = ConvertToANSI(wzFilePath);
        if (psz)
	        *phFile = (v_pfnCreateFile)((LPWSTR)psz, dwAccess, dwShareMode, NULL, dwCreate, FILE_ATTRIBUTE_NORMAL, NULL);
        MemFree(psz);
    }
    else
    {
	    *phFile = (v_pfnCreateFile)(wzFilePath, dwAccess, dwShareMode, NULL, dwCreate, FILE_ATTRIBUTE_NORMAL, NULL);
    }

    return (*phFile != INVALID_HANDLE_VALUE);
}


////////////////////////////////////////////////////////////////////////
// FPerformShellOp
//
//  This function started as a wrapper for SHFileOperation, but that 
//  shell function had an enormous performance hit, especially on Win9x
//  and NT4. To speed things up we removed the shell32 call and are
//  using the kernel32 APIs instead. The only drawback is we can't
//  handle multiple files, but that is not critical for this project.
//
STDAPI_(BOOL) FPerformShellOp(DWORD dwOp, WCHAR* wzFrom, WCHAR* wzTo)
{
	BOOL f = FALSE;

    if (!FEnsureProcAddresses())
        return FALSE;

 // On Win9x you have to use ANSI...
    if (v_bRunningOnWin9x)
    {
	    LPSTR pszFrom = ConvertToANSI(wzFrom);
	    LPSTR pszTo = ConvertToANSI(wzTo);

	    if (dwOp == FO_RENAME)
	    {
		    f = (v_pfnMoveFile)((LPWSTR)pszFrom, (LPWSTR)pszTo);
	    }
	    else if (dwOp == FO_DELETE)
	    {
		    f = (v_pfnDeleteFile)((LPWSTR)pszFrom);
	    }

	    if (pszFrom) MemFree(pszFrom);
	    if (pszTo) MemFree(pszTo);
    }
    else
    {
	    if (dwOp == FO_RENAME)
	    {
		    f = (v_pfnMoveFile)(wzFrom, wzTo);
	    }
	    else if (dwOp == FO_DELETE)
	    {
		    f = (v_pfnDeleteFile)(wzFrom);
	    }
    }

	return f;
}

////////////////////////////////////////////////////////////////////////
// FGetModuleFileName
//
//  Handles Unicode/ANSI paths from a module handle.
//
STDAPI_(BOOL) FGetModuleFileName(HMODULE hModule, WCHAR** wzFileName)
{
    LPWSTR pwsz;
    DWORD dw;

    CHECK_NULL_RETURN(wzFileName, FALSE);
    *wzFileName = NULL;

    if (!FEnsureProcAddresses())
        return FALSE;

    pwsz = (LPWSTR)MemAlloc(MAX_PATH*2);
    CHECK_NULL_RETURN(pwsz, FALSE);

    dw = (v_pfnGetModuleName)(hModule, pwsz, MAX_PATH);
    if (dw == 0)
    {
        MemFree(pwsz);
        return FALSE;
    }

 // On Win9x we got an ANSI string, so we should
 // convert it to UNICODE for OLE...
    if (v_bRunningOnWin9x)
    {
        LPWSTR pwsz2 = ConvertToLPWSTR((LPSTR)pwsz);
        if (pwsz2 == NULL)
        {
            MemFree(pwsz);
            return FALSE;
        }

        MemFree(pwsz);
        pwsz = pwsz2;
    }

    *wzFileName = pwsz;
    return TRUE;
}


////////////////////////////////////////////////////////////////////////
// AreStringsEqual
//
//  This function compares 2 strings using the OS supplied API and
//  returns TRUE if they are lexically equal in value.
//
//  NOTE: This serves as a replacement for lstrcmpiW which will not
//  work on Win95 -- the strings must be in ANSI for that OS!!
//
STDAPI_(BOOL) AreStringsEqual(LPCWSTR pwsz1, INT cch1, LPCWSTR pwsz2, INT cch2)
{
	INT iret;
	LCID lcid = GetThreadLocale();

    if (!FEnsureProcAddresses())
        return FALSE;

 // On Win9x you have to use ANSI...
    if (v_bRunningOnWin9x)
    {
		LPSTR psz1 = ConvertToANSI(pwsz1);
		LPSTR psz2 = ConvertToANSI(pwsz2);


		iret = (v_pfnCompareString)(lcid, NORM_IGNORECASE,
				(LPWSTR)psz1, cch1, (LPWSTR)psz2, cch2);

		MemFree(psz2);
		MemFree(psz1);
	}
	else
	{
		iret = (v_pfnCompareString)(lcid, NORM_IGNORECASE | NORM_IGNOREWIDTH,
				pwsz1, cch1, pwsz2, cch2);
	}

	return (iret == CSTR_EQUAL);
}


////////////////////////////////////////////////////////////////////////
// IsIECacheFile
//
//  Determines if file came from IE Cache (treat as read-only).
//
STDAPI_(BOOL) IsIECacheFile(LPWSTR pwszFile)
{
    HKEY hk;
    LPWSTR pwszTmpCache;
    BOOL fIsCacheFile = FALSE;
    CHAR szbuffer[MAX_PATH];

    if (RegOpenKey(HKEY_CURRENT_USER, 
        "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders", &hk) == ERROR_SUCCESS)
    {
        DWORD dwT, dwS;
        dwT = MAX_PATH; memset(szbuffer, 0, dwT);

        if ((RegQueryValueEx(hk, "Cache", 0, &dwT, (BYTE*)szbuffer, &dwS) == ERROR_SUCCESS) && 
            (dwT == REG_SZ) && (dwS-- > 1) && (pwszTmpCache = ConvertToLPWSTR(szbuffer)))
        {
            fIsCacheFile = AreStringsEqual(pwszTmpCache, dwS, pwszFile, dwS);
            MemFree(pwszTmpCache);
        }

        RegCloseKey(hk);
    }

    return fIsCacheFile;
}

////////////////////////////////////////////////////////////////////////
// FDrawText
//
//  This is used by the control for drawing the caption in the titlebar.
//  Since a custom caption could contain Unicode characters only printable
//  on Unicode OS, we should try to use the Unicode version when available.
//
STDAPI_(BOOL) FDrawText(HDC hdc, WCHAR* pwsz, LPRECT prc, UINT fmt)
{
	BOOL f;

    if (!FEnsureProcAddresses())
        return FALSE;

    if (v_bRunningOnWin9x)
	{
		LPSTR psz = ConvertToANSI(pwsz);
		f = (BOOL)(v_pfnDrawText)(hdc, (LPWSTR)psz, -1, prc, fmt);
		MemFree(psz);
	}
	else f = (BOOL)(v_pfnDrawText)(hdc, pwsz, -1, prc, fmt);

	return f;
}


////////////////////////////////////////////////////////////////////////
// FSetRegKeyValue
//
//  We use this for registration when dealing with the file path, since
//  that path may have Unicode characters on some systems. Win9x boxes
//  will have to be converted to ANSI.
//
STDAPI_(BOOL) FSetRegKeyValue(HKEY hk, WCHAR* pwsz)
{
	LONG lret;

    if (!FEnsureProcAddresses())
        return FALSE;

    if (v_bRunningOnWin9x)
	{
		LPSTR psz = ConvertToANSI(pwsz);
		lret = (v_pfnRegSetValueEx)(hk, NULL, 0, REG_SZ, (BYTE*)psz, lstrlen(psz));
		MemFree(psz);
	}
	else lret = (v_pfnRegSetValueEx)(hk, NULL, 0, REG_SZ, (BYTE*)pwsz, (lstrlenW(pwsz) * sizeof(WCHAR)));

	return (lret == ERROR_SUCCESS);
}



////////////////////////////////////////////////////////////////////////
// URL Helpers
//
//
STDAPI_(BOOL) LooksLikeHTTP(LPCWSTR pwsz)
{
	return ((pwsz) && ((*pwsz == L'H') || (*pwsz == L'h')) &&
		((*(pwsz + 1) == L'T') || (*(pwsz + 1) == L't')) &&
		((*(pwsz + 2) == L'T') || (*(pwsz + 2) == L't')) &&
		((*(pwsz + 3) == L'P') || (*(pwsz + 3) == L'p')) &&
		((*(pwsz + 4) == L':') || (((*(pwsz + 4) == L'S') || (*(pwsz + 4) == L's')) && (*(pwsz + 5) == L':'))));
}

////////////////////////////////////////////////////////////////////////
// GetTempPathForURLDownload
//
//  Constructs a temp path for a downloaded file. Note we scan the URL 
//  to find the name of the file so we can use its exention for server 
//  association (in case it is a non-docfile -- like RTF) and also 
//  create our own subfolder to try and avoid name conflicts in temp
//  folder itself. 
//
STDAPI_(BOOL) GetTempPathForURLDownload(WCHAR* pwszURL, WCHAR** ppwszLocalFile)
{
	DWORD  dw;
	LPWSTR pwszTPath = NULL;
	LPWSTR pwszTFile = NULL;
	CHAR   szTmpPath[MAX_PATH];

 // Do a little parameter checking and find length of the URL...
	if (!(pwszURL) || ((dw = lstrlenW(pwszURL)) < 6) ||
		!(LooksLikeHTTP(pwszURL)) || !(ppwszLocalFile))
		return FALSE;

	*ppwszLocalFile = NULL;

	if (GetTempPath(MAX_PATH, szTmpPath))
	{
		DWORD dwtlen = lstrlen(szTmpPath);
		if (dwtlen > 0 && szTmpPath[dwtlen-1] != '\\')
			lstrcat(szTmpPath, "\\");

		lstrcat(szTmpPath, "DsoFramer");

		if (CreateDirectory(szTmpPath, NULL) || GetLastError() == ERROR_ALREADY_EXISTS)
		{
			lstrcat(szTmpPath, "\\");
			pwszTPath = ConvertToLPWSTR(szTmpPath);
		}
	}

	if (pwszTPath)
	{
		if (pwszTFile = CopyString(pwszURL))
		{
			LPWSTR pwszT = pwszTFile;
			while (*(++pwszT))
				if (*pwszT == L'?'){*pwszT = L'\0'; break;}

			while (*(--pwszT))
				if (*pwszT == L'/'){++pwszT; break;}

			*ppwszLocalFile = CopyStringCat(pwszTPath, pwszT);

			MemFree(pwszTFile);
		}

		MemFree(pwszTPath);
	}

	return (BOOL)(*ppwszLocalFile);
}


////////////////////////////////////////////////////////////////////////
// Memory Allocation -- We use our own heap
// 
//
HANDLE v_hAllocHeap   = NULL;   // Global Heap handle - use instead of malloc/free, new/delete

STDAPI_(LPVOID) MemAlloc(DWORD cbSize)
{
	if (NULL == v_hAllocHeap)
	{
		if (NULL == (v_hAllocHeap = HeapCreate(0, 1024, 0)))
			return NULL;
	}
	return HeapAlloc(v_hAllocHeap, HEAP_ZERO_MEMORY, cbSize);
}

STDAPI_(void) MemFree(LPVOID ptr)
{
	ASSERT(v_hAllocHeap);
	if (ptr) HeapFree(v_hAllocHeap, 0, ptr);
}

void* _cdecl operator new(size_t size){return MemAlloc(size);}
void _cdecl operator delete(void *lpv){MemFree(lpv);}
int __cdecl _purecall(){__asm{int 3}; return 0;}

/////////////////////////////////////////////////////////////////////////////
//  EvalException -- Use to evaluate thrown NT exceptions.
//
//  __try{}
//  __except(EvalException(GetExceptionCode())){}
//
int EvalException(int n_except)
{
  // Trap a few standard errors usually caused by bad
  // passed in data or internal corruption. Other items should 
  // always be passed down the chain to NT or a waiting debugger...
    if ((n_except == STATUS_ACCESS_VIOLATION) ||
		(n_except == STATUS_INTEGER_OVERFLOW) ||
        (n_except == STATUS_INTEGER_DIVIDE_BY_ZERO))   
        return EXCEPTION_EXECUTE_HANDLER;

    return EXCEPTION_CONTINUE_SEARCH;
}


////////////////////////////////////////////////////////////////////////
//
// String conversion routines
//

STDAPI_(LPWSTR) ConvertToLPWSTR(LPCSTR pszString)
{
	LPWSTR pwsz;
	UINT cblen;

	CHECK_NULL_RETURN(pszString, NULL);
	cblen = lstrlen(pszString);

	if ((pwsz = (LPWSTR)MemAlloc((cblen+1)*sizeof(WCHAR))) && 
		(0 == MultiByteToWideChar(CP_ACP, MB_PRECOMPOSED, pszString, cblen, pwsz, cblen + 1)))
	{
		MemFree(pwsz); pwsz = NULL;
	}

	return pwsz;
}

STDAPI_(BSTR) ConvertToBSTR(LPCSTR pszString)
{
	BSTR bstr = NULL;
	LPWSTR pwsz;
	
	if (pwsz = ConvertToLPWSTR(pszString))
	{
		bstr = SysAllocString(pwsz);
		MemFree(pwsz);
	}

	return bstr;
}

STDAPI_(LPSTR) ConvertToANSI(LPCWSTR pwszString)
{
	LPSTR psz;
	UINT cblen;

	CHECK_NULL_RETURN(pwszString, NULL);
	cblen = WideCharToMultiByte(CP_ACP, 0, pwszString, -1, NULL, 0, NULL, NULL);

	if ((psz = (LPSTR)MemAlloc(cblen + 1)) && 
		(0 == WideCharToMultiByte(CP_ACP, 0, pwszString, -1, psz, cblen, NULL, NULL)))
	{
		MemFree(psz); psz = NULL;
	}

	return psz;
}

STDAPI_(LPSTR) CLSIDtoLPSTR(REFCLSID clsid)
{
	LPSTR psz = NULL;
	LPWSTR pwsz;

	if (SUCCEEDED(StringFromCLSID(clsid, &pwsz)))
	{
		psz = ConvertToANSI(pwsz);
		CoTaskMemFree(pwsz);
	}

    return psz;
}

STDAPI_(LPWSTR) CopyString(LPWSTR pwszString)
{
	LPWSTR pwsz;
	UINT cblen;

	CHECK_NULL_RETURN(pwszString, NULL);
	cblen = (2 * lstrlenW(pwszString));

	pwsz = (LPWSTR)MemAlloc(cblen + 2);
	CHECK_NULL_RETURN(pwsz, NULL);

	memcpy(pwsz, pwszString, cblen);
	return pwsz;
}

STDAPI_(LPWSTR) CopyStringCat(LPWSTR pwszString1, LPWSTR pwszString2)
{
	LPWSTR pwsz;
	UINT cblen1 = 0;
	UINT cblen2 = 0;

	if (pwszString1) cblen1 = lstrlenW(pwszString1);
	if (pwszString2) cblen2 = lstrlenW(pwszString2);

	if ((cblen1 + cblen2) == 0)
		return NULL;

	pwsz = (LPWSTR)MemAlloc((cblen1 + cblen2 + 2) * 2);
	CHECK_NULL_RETURN(pwsz, NULL);

	if (pwszString1)
		memcpy(pwsz, pwszString1, (cblen1 * 2));
	if (pwszString2)
		memcpy((pwsz + cblen1), pwszString2, (cblen2 * 2));

	return pwsz;
}


////////////////////////////////////////////////////////////////////////
// GetTypeInfoEx
//
//  Gets an ITypeInfo from the LIBID specified. Optionally can load and
//  register the typelib from a module resource (if specified). Used to
//  load our typelib on demand.
//
STDAPI GetTypeInfoEx(REFGUID rlibid, LCID lcid, WORD wVerMaj, WORD wVerMin, 
	HMODULE hResource, REFGUID rguid, ITypeInfo** ppti)
{
	HRESULT     hr;
	ITypeLib*   ptlib;

	CHECK_NULL_RETURN(ppti, E_POINTER);
    *ppti = NULL;

 // Try to pull information from the registry...
    hr = LoadRegTypeLib(rlibid, wVerMaj, wVerMin, lcid, &ptlib);

 // If that failed, and we have a resource module to load from,
 // try loading it from the module...
    if (FAILED(hr) && (hResource))
    {
		LPWSTR pwszPath;

        if (FGetModuleFileName(hResource, &pwszPath))
        {
         // Now, load the type library from module resource file...
			hr = LoadTypeLib(pwszPath, &ptlib);

		 // Register library to make things easier next time...
			if (SUCCEEDED(hr))
                RegisterTypeLib(ptlib, pwszPath, NULL);

			MemFree(pwszPath);
		}
    }

 // Can't go any further if there is an error...
	if (FAILED(hr)) return hr;

 // We have the typelib. Now get the requested typeinfo...
    hr = ptlib->GetTypeInfoOfGuid(rguid, ppti);

 // Release the type library interface.
    RELEASE_INTERFACE(ptlib);
	return hr;
}


////////////////////////////////////////////////////////////////////////
// AutoDispInvoke
//
//  Wrapper for IDispatch::Invoke calls.
//
STDAPI AutoDispInvoke(LPDISPATCH pdisp, LPOLESTR pwszname, DISPID dspid,
	WORD wflags, DWORD cargs, VARIANT* rgargs, VARIANT* pvtret)
{
    HRESULT    hr;
    DISPID     dspidPut = DISPID_PROPERTYPUT;
    DISPPARAMS dspparm = {NULL, NULL, 0, 0};

	CHECK_NULL_RETURN(pdisp, E_POINTER);

    dspparm.rgvarg = rgargs;
    dspparm.cArgs = cargs;

	if ((wflags & DISPATCH_PROPERTYPUT) || (wflags & DISPATCH_PROPERTYPUTREF))
	{
		dspparm.rgdispidNamedArgs = &dspidPut;
		dspparm.cNamedArgs = 1;
	}

	__try
	{
		if (pwszname)
		{
			hr = pdisp->GetIDsOfNames(IID_NULL, &pwszname, 1, LOCALE_USER_DEFAULT, &dspid);
			RETURN_ON_FAILURE(hr);
		}

        hr = pdisp->Invoke(dspid, IID_NULL, LOCALE_USER_DEFAULT,
            (WORD)(DISPATCH_METHOD | wflags), &dspparm, pvtret, NULL, NULL);

    }
    __except(EvalException(GetExceptionCode()))
    {
        hr = E_WIN32_ACCESSVIOLATION;
    }

    return hr;
}


#define HIMETRIC_PER_INCH   2540      // number HIMETRIC units per inch
#define PTS_PER_INCH        72        // number points (font size) per inch

#define MAP_PIX_TO_LOGHIM(x,ppli)   MulDiv(HIMETRIC_PER_INCH, (x), (ppli))
#define MAP_LOGHIM_TO_PIX(x,ppli)   MulDiv((ppli), (x), HIMETRIC_PER_INCH)

STDAPI_(void) HimetricToPixels(LONG* px, LONG* py)
{
    HDC hdc = GetDC(NULL);
    if (px) *px = MAP_LOGHIM_TO_PIX(*px, GetDeviceCaps(hdc, LOGPIXELSX));
    if (py) *py = MAP_LOGHIM_TO_PIX(*py, GetDeviceCaps(hdc, LOGPIXELSY));
    ReleaseDC(NULL, hdc);
}

STDAPI_(void) PixelsToHimetric(LONG* px, LONG* py)
{
    HDC hdc = GetDC(NULL);
    if (px) *px = MAP_PIX_TO_LOGHIM(*px, GetDeviceCaps(hdc, LOGPIXELSX));
    if (py) *py = MAP_PIX_TO_LOGHIM(*py, GetDeviceCaps(hdc, LOGPIXELSY));
    ReleaseDC(NULL, hdc);
}

STDAPI_(HBITMAP) GetDeactiveBitmapFromWindow(HWND hwnd)
{
    HBITMAP hbmpold, hbmp = NULL;
    HDC hdcWin, hdcMem;
    RECT rc;
    INT x, y;

    if (!GetWindowRect(hwnd, &rc))
        return NULL;

    x = (rc.right - rc.left); if (x < 0) x = 1;
    y = (rc.bottom - rc.top); if (y < 0) y = 1;

	hdcWin = GetDC(hwnd);
	hdcMem = CreateCompatibleDC(hdcWin);

	hbmp = CreateCompatibleBitmap(hdcWin, x, y);
	hbmpold = (HBITMAP)SelectObject(hdcMem, hbmp);

	BitBlt(hdcMem, 0,0, x, y, hdcWin, 0,0, SRCCOPY);

	SelectObject(hdcMem, hbmpold);
	DeleteDC(hdcMem);
	ReleaseDC(hwnd, hdcWin);

    return hbmp;
}