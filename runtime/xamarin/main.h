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

// This has a managed equivalent in NSObject2.cs
enum NSObjectFlags {
	NSObjectFlagsDisposed = 1,
	NSObjectFlagsNativeRef = 2,
	NSObjectFlagsIsDirectBinding = 4,
	NSObjectFlagsRegisteredToggleRef = 8,
	NSObjectFlagsInFinalizerQueue = 16,
	NSObjectFlagsHasManagedRef = 32,
	// 64, // Used by SoM
	NSObjectFlagsIsCustomType = 128,
};

enum XamarinGCHandleType : int {
	XamarinGCHandleTypeWeak = 0,
	XamarinGCHandleTypeWeakTrackResurrection = 1,
	XamarinGCHandleTypeNormal = 2,
	XamarinGCHandleTypePinned = 3,
};

// Keep in sync with Runtime.LookupTypes in Runtime.CoreCLR.cs
enum XamarinLookupTypes : int {
	XamarinLookupTypes_System_Array,
	XamarinLookupTypes_System_String,
	XamarinLookupTypes_System_IntPtr,
	XamarinLookupTypes_Foundation_NSNumber,
	XamarinLookupTypes_Foundation_NSObject,
	XamarinLookupTypes_Foundation_NSString,
	XamarinLookupTypes_Foundation_NSValue,
	XamarinLookupTypes_ObjCRuntime_INativeObject,
	XamarinLookupTypes_ObjCRuntime_NativeHandle,
};

// Keep in sync with Runtime.ExceptionType in Runtime.CoreCLR.cs
enum XamarinExceptionTypes : int {
	XamarinExceptionTypes_System_Exception,
	XamarinExceptionTypes_System_InvalidCastException,
	XamarinExceptionTypes_System_EntryPointNotFoundException,
	XamarinExceptionTypes_System_OutOfMemoryException,
};

// Keep in sync with AssemblyBuildTarget in AssemblyBuildTarget.cs
enum XamarinNativeLinkMode : int {
	XamarinNativeLinkModeStaticObject,
	XamarinNativeLinkModeDynamicLibrary,
	XamarinNativeLinkModeFramework,
};

enum XamarinTriState : int {
	XamarinTriStateNone,
	XamarinTriStateEnabled,
	XamarinTriStateDisabled,
};

extern bool mono_use_llvm; // this is defined inside mono

#if defined (NATIVEAOT)
#define SUPPORTS_DYNAMIC_REGISTRATION 0
#else
#define SUPPORTS_DYNAMIC_REGISTRATION 1
#endif

#if DEBUG
extern bool xamarin_gc_pump;
#endif
extern bool xamarin_debug_mode;
extern bool xamarin_disable_lldb_attach;
extern bool xamarin_disable_omit_fp;
#if MONOMAC
extern bool xamarin_mac_hybrid_aot;
extern bool xamarin_mac_modern;
extern char *xamarin_entry_assembly_path;
#endif
extern bool xamarin_init_mono_debug;
extern int xamarin_log_level;
extern const char *xamarin_executable_name;
#if MONOMAC || TARGET_OS_MACCATALYST
extern NSString *xamarin_custom_bundle_name;
#endif
extern const char *xamarin_arch_name;
extern bool xamarin_is_gc_coop;
extern enum MarshalObjectiveCExceptionMode xamarin_marshal_objectivec_exception_mode;
extern enum MarshalManagedExceptionMode xamarin_marshal_managed_exception_mode;
extern enum XamarinLaunchMode xamarin_launch_mode;
#if SUPPORTS_DYNAMIC_REGISTRATION
extern bool xamarin_supports_dynamic_registration;
#endif
extern const char *xamarin_runtime_configuration_name;
extern enum XamarinNativeLinkMode xamarin_libmono_native_link_mode;
extern const char** xamarin_runtime_libraries;

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

#ifndef MONOTOUCH
void xamarin_set_is_mkbundle (bool value); /* Not Public API, exact semantics is not defined yet */
bool xamarin_get_is_mkbundle (); /* Not Public API, exact semantics is not defined yet */
#endif

void xamarin_set_is_debug (bool value); /* Public API */
bool xamarin_get_is_debug (); /* Public API */

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __XAMARIN_MAIN_H__ */
