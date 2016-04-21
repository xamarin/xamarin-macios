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

extern bool mono_use_llvm; // this is defined inside mono

extern bool xamarin_use_new_assemblies;
extern bool xamarin_gc_pump;
extern bool xamarin_debug_mode;
extern bool xamarin_use_old_dynamic_registrar;
extern bool xamarin_use_il_registrar;
extern bool xamarin_init_mono_debug;
extern bool xamarin_compact_seq_points;
extern int xamarin_log_level;
extern const char *xamarin_executable_name;
extern const char *xamarin_arch_name;

#ifdef MONOTOUCH
extern NSString* xamarin_crashlytics_api_key;
extern NSTimeInterval xamarin_crashlytics_delay;
extern int32_t xamarin_console_poll_interval;
#endif

typedef void (*xamarin_setup_callback) ();
typedef int (*xamarin_extension_main_callback) (int argc, char** argv);

extern xamarin_setup_callback xamarin_setup;
extern xamarin_extension_main_callback xamarin_extension_main;

void xamarin_set_use_sgen (bool value); /* Public API, but a no-op (only SGen is supported, so it defaults to true) */
bool xamarin_get_use_sgen (); /* Public API, always returns true */

void xamarin_set_is_unified (bool value);  /* Public API */
bool xamarin_get_is_unified ();  /* Public API */

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
