//
// main.h: Basic startup code for Xamarin.iOS/Xamarin.Mac
//         This header contains definitions used by Xamarin when building applications.
//         Do not consider anything here stable API (unless otherwise specified),
//         it will change between releases.
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc. 
//

#include <Foundation/Foundation.h>

#ifndef __XAMARIN_MAIN_H__
#define __XAMARIN_MAIN_H__

#ifdef __cplusplus
extern "C" {
#endif

/* This enum must always match the identical enum in src/ObjCRuntime/ExceptionMode.cs */
enum MarshalObjectiveCExceptionMode : int {
	MarshalObjectiveCExceptionModeDefault               = 0,
	MarshalObjectiveCExceptionModeUnwindManagedCode     = 1,
	MarshalObjectiveCExceptionModeThrowManagedException = 2,
	MarshalObjectiveCExceptionModeAbort                 = 3,
	MarshalObjectiveCExceptionModeDisable               = 4,
};

/* This enum must always match the identical enum in src/ObjCRuntime/ExceptionMode.cs */
enum MarshalManagedExceptionMode : int {
	MarshalManagedExceptionModeDefault                  = 0,
	MarshalManagedExceptionModeUnwindNativeCode         = 1,
	MarshalManagedExceptionModeThrowObjectiveCException = 2,
	MarshalManagedExceptionModeAbort                    = 3,
	MarshalManagedExceptionModeDisable                  = 4,
};

/* This enum must always match the identical enum in src/ObjCRuntime/Runtime.cs */
enum XamarinLaunchMode {
	XamarinLaunchModeApp = 0,
	XamarinLaunchModeExtension = 1,
	XamarinLaunchModeEmbedded = 2,
};

extern bool mono_use_llvm; // this is defined inside mono

#if MONOMAC
extern bool xamarin_use_new_assemblies;
#else
	#define xamarin_use_new_assemblies 1
#endif
extern bool xamarin_gc_pump;
extern bool xamarin_debug_mode;
extern bool xamarin_disable_lldb_attach;
#if MONOMAC
extern bool xamarin_mac_hybrid_aot;
extern bool xamarin_mac_modern;
extern char *xamarin_entry_assembly_path;
#endif
extern bool xamarin_init_mono_debug;
extern int xamarin_log_level;
extern const char *xamarin_executable_name;
extern const char *xamarin_arch_name;
extern bool xamarin_is_gc_coop;
extern enum MarshalObjectiveCExceptionMode xamarin_marshal_objectivec_exception_mode;
extern enum MarshalManagedExceptionMode xamarin_marshal_managed_exception_mode;
extern enum XamarinLaunchMode xamarin_launch_mode;

typedef void (*xamarin_setup_callback) ();
typedef int (*xamarin_extension_main_callback) (int argc, char** argv);

extern xamarin_setup_callback xamarin_setup;
extern xamarin_extension_main_callback xamarin_extension_main;

void xamarin_set_use_sgen (bool value); /* Public API, but a no-op (only SGen is supported, so it defaults to true) */
bool xamarin_get_use_sgen (); /* Public API, always returns true */

void xamarin_set_is_unified (bool value);  /* Public API */
bool xamarin_get_is_unified ();  /* Public API */

int xamarin_get_launch_mode ();

int xamarin_watchextension_main (int argc, char **argv);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __XAMARIN_MAIN_H__ */

#ifndef MONOTOUCH
void xamarin_set_is_mkbundle (bool value); /* Not Public API, exact semantics is not defined yet */
bool xamarin_get_is_mkbundle (); /* Not Public API, exact semantics is not defined yet */
#endif

void xamarin_set_is_debug (bool value); /* Public API */
bool xamarin_get_is_debug (); /* Public API */
