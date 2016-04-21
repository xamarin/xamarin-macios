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


typedef struct {
	const char *name;
	void (*connect) (const char *address);
	void (*close1) (void);
	void (*close2) (void);
	gboolean (*send) (void *buf, int len);
	int (*recv) (void *buf, int len);
} DebuggerTransport;

void mono_debugger_agent_parse_options (const char *options); 
gboolean mono_debugger_agent_transport_handshake (void);
void mono_debugger_agent_register_transport (DebuggerTransport *trans);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __MONOTOUCH_DEBUG_H__ */
