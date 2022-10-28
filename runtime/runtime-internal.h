/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * runtime-internal.h: This header is not shipped with Xamarin.iOS/Mac.
 *
 *  Authors: Rolf Bjarne Kvinge
 *
 *  Copyright (C) 2014 Xamarin Inc. (www.xamarin.com)
 *
 */

#ifndef __RUNTIME_INTERNAL_H__
#define __RUNTIME_INTERNAL_H__

#include "xamarin/xamarin.h"

#define PRINT(...) do { xamarin_printf (__VA_ARGS__); } while (0);
#define LOG(...) do { if (xamarin_log_level > 0) xamarin_printf (__VA_ARGS__); } while (0);

// #define DEBUG_LAUNCH_TIME

#ifdef DEBUG_LAUNCH_TIME
#define DEBUG_LAUNCH_TIME_PRINT(msg) \
	debug_launch_time_print (msg);
#else
#define DEBUG_LAUNCH_TIME_PRINT(...)
#endif

// Uncomment the TRACK_MONOOBJECTS define to show a summary at process exit of
// the MonoObjects that were created, and if any were not freed. If there are
// leaked MonoObjects, a list of them will be printed.
// This has an equivalent variable in src/ObjCRuntime/Runtime.CoreCLR.cs,
// which must be set for tracking to work.
//#define TRACK_MONOOBJECTS

#ifdef __cplusplus
extern "C" {
#endif

void *xamarin_marshal_return_value (SEL sel, MonoType *mtype, const char *type, MonoObject *retval, bool retain, MonoMethod *method, MethodDescription *desc, GCHandle *exception_gchandle);

void xamarin_dyn_objc_msgSend ();
void xamarin_dyn_objc_msgSendSuper ();
void xamarin_dyn_objc_msgSend_stret ();
void xamarin_dyn_objc_msgSendSuper_stret ();
void xamarin_add_internal_call (const char *name, const void *method);

#ifdef __cplusplus
}
#endif
/*
 * XamarinGCHandle
 *
 * This is an ObjC type that ties the lifetime of a GCHandle to the lifetime of itself.
 * It stores a GCHandle, and frees the GCHandle in dealloc.
 */
@interface XamarinGCHandle : NSObject {
@public
	GCHandle handle;
}
+(XamarinGCHandle *) createWithHandle: (GCHandle) handle;
-(void) dealloc;
-(GCHandle) getHandle;
@end

#endif /* __RUNTIME_INTERNAL_H__ */
