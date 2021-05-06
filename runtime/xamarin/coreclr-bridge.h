
/* Support for using CoreCLR */

#if defined (CORECLR_RUNTIME)

#ifndef __CORECLR_BRIDGE__
#define __CORECLR_BRIDGE__

#include <stdatomic.h>

//#define LOG_CORECLR(...)
#define LOG_CORECLR(...) fprintf (__VA_ARGS__)

#ifdef __cplusplus
extern "C" {
#endif

// We need a way to represent a managed object in native code, and since most
// our existing runtime code uses MonoObjects, we use the same for the CoreCLR
// bridge, just our own version of it. In Mono, the MonoObjects are tracked by
// the GC (which scans the stack), but we can't make CoreCLR scan the stack,
// so we use a reference counted version of MonoObject instead - we just put
// the GCHandle into a reference counted MonoObject, and when the MonoObject
// is freed, then we free the GCHandle as well.
//
// This struct must be kept in sync with the MonoObject struct in Runtime.CoreCLR.cs
struct _MonoObject {
	int _Atomic reference_count;
	GCHandle gchandle;
};

// This struct must be kept in sync with the MonoMethodSignature struct in Runtime.CoreCLR.cs
struct _MonoMethodSignature {
	MonoObject *method;
	int parameter_count;
	MonoObject *return_type;
	MonoObject *parameters[];
};

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __CORECLR_BRIDGE__ */

#endif // CORECLR_RUNTIME
