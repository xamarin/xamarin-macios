
#ifndef __TRAMPOLINES_INTERNAL_H__
#define __TRAMPOLINES_INTERNAL_H__

#include <stdbool.h>
#include "xamarin/mono-runtime.h"
#include "xamarin/runtime.h"

//#define TRACE
#ifdef TRACE
#define LOGZ(...) fprintf (stderr, __VA_ARGS__);
#else
#define LOGZ(...) ;
#endif

#ifdef __cplusplus
extern "C" {
#endif

// state: instance methods have the first bit cleared.

enum TrampolineType {
	Tramp_Default = 0x0,
	Tramp_Static = 0x1,
	Tramp_Ctor = 0x2,
	Tramp_FpretSingle = 0x4,
	Tramp_StaticFpretSingle = Tramp_FpretSingle |  Tramp_Static, // 0x5
	Tramp_FpretDouble = 0x8,
	Tramp_StaticFpretDouble = Tramp_FpretDouble | Tramp_Static, // 0x9
	Tramp_Stret = 0x10,
	Tramp_StaticStret = Tramp_Stret | Tramp_Static, // 0x11
	Tramp_LongRet = 0x20,
	Tramp_StaticLongRet = Tramp_Static | Tramp_LongRet, // 0x21
	Tramp_DoubleStret = 0x40 | Tramp_Stret, // 0x50
	Tramp_StaticDoubleStret = Tramp_DoubleStret | Tramp_Static, // 0x51
};

enum IteratorAction {
	IteratorStart,
	IteratorIterate,
	IteratorEnd,
};

// type: pass NULL to start iterating.
// target: can be null if not interested in the value.
typedef void (*iterator_func) (enum IteratorAction action, void *context, const char *type, size_t size, void *target, GCHandle *exception_gchandle);
typedef void (*marshal_return_value_func) (void *context, const char *type, size_t size, void *value, MonoType *mtype, bool retain, MonoMethod *method, MethodDescription *desc, GCHandle *exception_gchandle);

void xamarin_invoke_trampoline (enum TrampolineType type, id self, SEL sel, iterator_func iterator, marshal_return_value_func marshal_return_value, void *context);

int xamarin_get_frame_length (id self, SEL sel);
const char * xamarin_skip_type_name (const char *ptr);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __TRAMPOLINES_INTERNAL_H__ */
