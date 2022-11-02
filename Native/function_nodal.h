//#ifdef WINDOWS
//#include <Windows.h>
#ifdef _MSC_VER

#define DLL_EXPORT __declspec(dllexport)

#ifndef STDMETHODCALLTYPE
#define STDMETHODCALLTYPE __stdcall
#endif

#else // !WINDOWS

#ifndef STDMETHODCALLTYPE
#define STDMETHODCALLTYPE
#endif

#if __GNUC__ >= 4
#define DLL_EXPORT __attribute__ ((visibility ("default")))
#else
#define DLL_EXPORT
#endif

#endif

extern "C" DLL_EXPORT int STDMETHODCALLTYPE sumi(int a, int b);
extern "C" DLL_EXPORT void STDMETHODCALLTYPE sumouti(int a, int b, int* c);
extern "C" DLL_EXPORT void STDMETHODCALLTYPE sumrefi(int a, int* b);