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

#ifdef __cplusplus
extern "C" {
#endif

void *xamarin_marshal_return_value (MonoType *mtype, const char *type, MonoObject *retval, bool retain, MonoMethod *method, MethodDescription *desc, guint32 *exception_gchandle);

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
	int handle;
}
+(XamarinGCHandle *) createWithHandle: (int) handle;
-(void) dealloc;
-(int) getHandle;
@end

#endif /* __RUNTIME_INTERNAL_H__ */
