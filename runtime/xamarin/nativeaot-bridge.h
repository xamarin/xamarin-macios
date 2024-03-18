
/* Support for using NativeAOT */

#if defined (NATIVEAOT)

#ifndef __NATIVEAOT_BRIDGE__
#define __NATIVEAOT_BRIDGE__

#include <stdatomic.h>

#include "runtime.h"

#ifdef __cplusplus
extern "C" {
#endif

void xamarin_objcruntime_runtime_nativeaotinitialize (struct InitializationOptions* options, GCHandle* exception_gchandle);
int __managed__Main (int argc, const char** argv);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __NATIVEAOT_BRIDGE__ */

#endif // NATIVEAOT
