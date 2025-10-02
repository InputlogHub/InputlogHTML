/***************************************************************************
 * UTILITIES.H
 *
 * DSOFramer: Common Utilities and Macros (Shared)
 *
 *  Copyright (c)1999-2001 Microsoft Corporation, All Rights Reserved
 *  Written by DSO Office Integration, Microsoft Developer Support
 *
 *  THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 *  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
 *  WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 *
 ***************************************************************************/
#ifndef DS_UTILITIES_H
#define DS_UTILITIES_H

////////////////////////////////////////////////////////////////////////
// Fixed Win32 Errors as HRESULTs
//
#define E_WIN32_BUFFERTOOSMALL    0x8007007A   //HRESULT_FROM_WIN32(ERROR_INSUFFICIENT_BUFFER)
#define E_WIN32_ACCESSVIOLATION   0x800701E7   //HRESULT_FROM_WIN32(ERROR_INVALID_ADDRESS)
#define E_VBA_NOREMOTESERVER      0x800A01CE

int EvalException(int n_except);

////////////////////////////////////////////////////////////////////////
// Unicode Win32 API wrappers (handles Win9x ANSI)
//
STDAPI_(BOOL) FFileExists(WCHAR* wzPath);
STDAPI_(BOOL) FOpenLocalFile(WCHAR* wzFilePath, DWORD dwAccess, DWORD dwShareMode, DWORD dwCreate, HANDLE* phFile);
STDAPI_(BOOL) FPerformShellOp(DWORD dwOp, WCHAR* wzFrom, WCHAR* wzTo);
STDAPI_(BOOL) FGetModuleFileName(HMODULE hModule, WCHAR** wzFileName);
STDAPI_(BOOL) AreStringsEqual(LPCWSTR pwsz1, INT cch1, LPCWSTR pwsz2, INT cch2);
STDAPI_(BOOL) IsIECacheFile(LPWSTR pwszFile);

STDAPI_(BOOL) FDrawText(HDC hdc, WCHAR* pwsz, LPRECT prc, UINT fmt);
STDAPI_(BOOL) FSetRegKeyValue(HKEY hk, WCHAR* pwsz);


////////////////////////////////////////////////////////////////////////
// URL Helpers
//
STDAPI_(BOOL) LooksLikeHTTP(LPCWSTR pwsz);
STDAPI_(BOOL) GetTempPathForURLDownload(WCHAR* pwszURL, WCHAR** ppwszLocalFile);


////////////////////////////////////////////////////////////////////////
// String Functions
//
STDAPI_(LPWSTR)  ConvertToLPWSTR(LPCSTR pszString);
STDAPI_(BSTR)    ConvertToBSTR(LPCSTR pszString);
STDAPI_(LPSTR)   ConvertToANSI(LPCWSTR pwszString);

STDAPI_(LPWSTR)  CopyString(LPWSTR pwszString);
STDAPI_(LPWSTR)  CopyStringCat(LPWSTR pwszString1, LPWSTR pwszString2);

STDAPI_(LPSTR)   CLSIDtoLPSTR(REFCLSID clsid);

////////////////////////////////////////////////////////////////////////
// Custom LoadTypeLib Function (for getting access to typeinfo)
//
STDAPI GetTypeInfoEx(REFGUID rlibid, LCID lcid, WORD wVerMaj, WORD wVerMin, 
	HMODULE hResource, REFGUID rguid, ITypeInfo** ppti);

STDAPI AutoDispInvoke(LPDISPATCH pdisp, LPOLESTR pwszname, DISPID dspid,
	WORD wflags, DWORD cargs, VARIANT* rgargs, VARIANT* pvtret);

////////////////////////////////////////////////////////////////////////
// OLE Conversion Functions
//
STDAPI_(void) HimetricToPixels(LONG* px, LONG* py);
STDAPI_(void) PixelsToHimetric(LONG* px, LONG* py);


////////////////////////////////////////////////////////////////////////
// GetDeactiveBitmapFromWindow Function -- helper for AlphaBlend
//
STDAPI_(HBITMAP) GetDeactiveBitmapFromWindow(HWND hwnd);


////////////////////////////////////////////////////////////////////////
// Heap Allocation
//
STDAPI_(LPVOID) MemAlloc(DWORD cbSize);
STDAPI_(void)   MemFree(LPVOID ptr);

// Override new/delete to use our task allocator
// (removing CRT dependency will improve code performance and size)...
void * _cdecl operator new(size_t size);
void  _cdecl operator delete(void *ptr);


////////////////////////////////////////////////////////////////////////
// Common macros
//

#define RETURN_ON_FAILURE(x)    if (FAILED(x)) return (x)
#define GOTO_ON_FAILURE(x, lbl) if (FAILED(x)) goto lbl

#define RELEASE_INTERFACE(x) if (x) (x)->Release(); (x) = NULL;
#define SAFE_RELEASE_INTERFACE(x) __try{if (x) x->Release(); x = NULL;}__except(1){}

#define SAFE_FREESTRING(s) if(s){MemFree(s); s = NULL;}
#define CHECK_NULL_RETURN(var, err) if(NULL==(var)) return (err)

inline VARIANT* PVarFromPVarRef(VARIANT* px)
{return ((px->vt == (VT_VARIANT|VT_BYREF)) ? (px->pvarVal) : px);}

inline BOOL IsVarParamMissing(VARIANT x)
{return (((PVarFromPVarRef(&x))->vt & ~VT_ERROR) == VT_EMPTY);}

inline LPWSTR PWStrFromPVar(VARIANT* px)
{return ((px->vt == VT_BSTR) ? px->bstrVal : ((px->vt == (VT_BSTR|VT_BYREF)) ? *(px->pbstrVal) : NULL));}

inline IUnknown* PUnkFromPVar(VARIANT* px)
{return (((px->vt == VT_DISPATCH) || (px->vt == VT_UNKNOWN)) ? px->punkVal : (((px->vt == (VT_DISPATCH|VT_BYREF)) || (px->vt == (VT_UNKNOWN|VT_BYREF))) ? *(px->ppunkVal) : NULL));}

inline BOOL BoolFromPVar(VARIANT* px, BOOL fdef)
{return ((((px->vt & 0xFF) != VT_BOOL) && ((px->vt & 0xFF) != VT_I4) && ((px->vt & 0xFF) != VT_I2)) ? fdef : ((px->vt & VT_ARRAY) == VT_ARRAY) ? (BOOL)(*(px->pboolVal)) : (BOOL)(px->boolVal));}

#define LPWSTR_FROM_VARIANT(x)   PWStrFromPVar(PVarFromPVarRef(&(x)))
#define PUNK_FROM_VARIANT(x)     PUnkFromPVar(PVarFromPVarRef(&(x)))
#define BOOL_FROM_VARIANT(x, y)  BoolFromPVar(PVarFromPVarRef(&(x)), (y))


////////////////////////////////////////////////////////////////////////
// Debug macros
//
#ifdef _DEBUG

#define ASSERT(x)  if(!(x)) DebugBreak()
#define ODS(x)	OutputDebugString(x)

#define TRACE1(sz, arg1) { \
	CHAR ach[1024]; \
	wsprintf(ach, (sz), (arg1)); \
	ODS(ach); }

#define TRACE2(sz, arg1, arg2) { \
	CHAR ach[1024]; \
	wsprintf(ach, (sz), (arg1), (arg2)); \
	ODS(ach); }

#define TRACE3(sz, arg1, arg2, arg3) { \
	CHAR ach[1024]; \
	wsprintf(ach, (sz), (arg1), (arg2), (arg3)); \
	ODS(ach); }

#define TRACE_LPRECT(sz, lprc) { \
	CHAR ach[1024]; \
	wsprintf(ach, "RECT %s - left=%d, top=%d, right=%d, bottom=%d\n", \
		(sz), (lprc)->left, (lprc)->top, (lprc)->right, (lprc)->bottom); \
	ODS(ach); }

#else // !defined(_DEBUG)

#define ASSERT(x)
#define ODS(x) 
#define TRACE1(sz, arg1)
#define TRACE2(sz, arg1, arg2)
#define TRACE3(sz, arg1, arg2, arg3)
#define TRACE_LPRECT(sz, lprc)

#endif // (_DEBUG)

////////////////////////////////////////////////////////////////////////
// Macros for Nested COM Interfaces 
//

#ifdef _DEBUG
#define DEFINE_REFCOUNT \
	ULONG m_cRef;
#define IMPLEMENT_DEBUG_ADDREF \
	m_cRef++;
#define IMPLEMENT_DEBUG_RELEASE \
	ASSERT(m_cRef > 0); \
	m_cRef--;
#else
#define DEFINE_REFCOUNT
#define IMPLEMENT_DEBUG_ADDREF
#define IMPLEMENT_DEBUG_RELEASE
#endif /* !_DEBUG */

#define BEGIN_INTERFACE_PART(localClass, baseClass) \
	class X##localClass : public baseClass \
	{ public: \
		STDMETHOD(QueryInterface)(REFIID iid, PVOID* ppvObj); \
		STDMETHOD_(ULONG, AddRef)(); \
		STDMETHOD_(ULONG, Release)(); \
    DEFINE_REFCOUNT

#define END_INTERFACE_PART(localClass) \
	} m_x##localClass; \
	friend class X##localClass;

#define METHOD_PROLOGUE(theClass, localClass) \
	theClass* pThis = \
		((theClass*)(((BYTE*)this) - (size_t)&(((theClass*)0)->m_x##localClass)));

#define IMPLEMENT_INTERFACE_UNKNOWN(theClass, localClass) \
	ULONG theClass::X##localClass::AddRef() { \
		METHOD_PROLOGUE(theClass, localClass) \
		IMPLEMENT_DEBUG_ADDREF \
		return pThis->AddRef(); \
	} \
	ULONG theClass::X##localClass::Release() { \
		METHOD_PROLOGUE(theClass, localClass) \
		IMPLEMENT_DEBUG_RELEASE \
		return pThis->Release(); \
	} \
	STDMETHODIMP theClass::X##localClass::QueryInterface(REFIID iid, void **ppvObj) { \
		METHOD_PROLOGUE(theClass, localClass) \
		return pThis->QueryInterface(iid, ppvObj); \
	}


#endif //DS_UTILITIES_H