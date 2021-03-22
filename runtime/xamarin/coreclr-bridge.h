
/* Support for using CoreCLR */

#if defined (CORECLR_RUNTIME)

#ifndef __CORECLR_BRIDGE__
#define __CORECLR_BRIDGE__

//#define LOG_CORECLR(...)
#define LOG_CORECLR(...) fprintf (__VA_ARGS__)

#ifdef __cplusplus
extern "C" {
#endif

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __CORECLR_BRIDGE__ */

#endif // CORECLR_RUNTIME
