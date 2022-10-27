//
// debug.h: Debugging code for MonoTouch
// 
// Authors:
//   Geoff Norton
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2009 Novell, Inc.
// Copyright 2011-2012 Xamarin Inc. 
//

#ifndef __MONOTOUCH_DEBUG_H__
#define __MONOTOUCH_DEBUG_H__

#include "runtime-internal.h"

#ifdef __cplusplus
extern "C" {
#endif

void monotouch_configure_debugging ();
void monotouch_start_debugging ();
void monotouch_start_profiling ();

void monotouch_set_connection_mode (const char *mode);
void monotouch_set_monodevelop_port (int port);

bool xamarin_is_native_debugger_attached ();

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __MONOTOUCH_DEBUG_H__ */
