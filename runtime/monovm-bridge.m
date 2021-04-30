/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
*  Authors: Rolf Bjarne Kvinge
*
*  Copyright (C) 2021 Microsoft Corp.
*
*/

/* Support code for using MonoVM */

#if !defined (CORECLR_RUNTIME)

#include <TargetConditionals.h>
#include <pthread.h>

#if !DOTNET && TARGET_OS_OSX
#define LEGACY_XAMARIN_MAC 1
#else
#define LEGACY_XAMARIN_MAC 0
#endif

#include "product.h"
#include "monotouch-debug.h"
#include "runtime-internal.h"
#include "xamarin/xamarin.h"
#include "xamarin/monovm-bridge.h"

static MonoAssembly* entry_assembly = NULL;
static MonoClass* inativeobject_class = NULL;
static MonoClass* nsobject_class = NULL;
static MonoClass* nsvalue_class = NULL;
static MonoClass* nsnumber_class = NULL;
static MonoClass* nsstring_class = NULL;
static MonoClass* runtime_class = NULL;

#if !LEGACY_XAMARIN_MAC
void
xamarin_bridge_setup ()
{
	const char *c_bundle_path = xamarin_get_bundle_path ();

	setenv ("MONO_PATH", c_bundle_path, 1);

	setenv ("MONO_XMLSERIALIZER_THS", "no", 1);
	setenv ("MONO_REFLECTION_SERIALIZER", "yes", 1);

#if TARGET_OS_WATCH || TARGET_OS_TV
	mini_parse_debug_option ("explicit-null-checks");
#endif
	// see http://bugzilla.xamarin.com/show_bug.cgi?id=820
	// take this line out once the bug is fixed
	mini_parse_debug_option ("no-gdb-backtrace");
}

void
xamarin_bridge_initialize ()
{
#if defined (__arm__) || defined(__aarch64__)
	xamarin_register_modules ();
#endif
	DEBUG_LAUNCH_TIME_PRINT ("\tAOT register time");

#ifdef DEBUG
	monotouch_start_debugging ();
	DEBUG_LAUNCH_TIME_PRINT ("\tDebug init time");
#endif
	
	if (xamarin_init_mono_debug)
		mono_debug_init (MONO_DEBUG_FORMAT_MONO);
	
	mono_install_assembly_preload_hook (xamarin_assembly_preload_hook, NULL);
	mono_install_load_aot_data_hook (xamarin_load_aot_data, xamarin_free_aot_data, NULL);

#ifdef DEBUG
	monotouch_start_profiling ();
	DEBUG_LAUNCH_TIME_PRINT ("\tProfiler config time");
#endif

	mono_set_signal_chaining (TRUE);
	mono_set_crash_chaining (TRUE);
	mono_install_unhandled_exception_hook (xamarin_unhandled_exception_handler, NULL);
	mono_install_ftnptr_eh_callback (xamarin_ftnptr_exception_handler);

	mono_jit_init_version ("MonoTouch", "mobile");
	/*
	  As part of mono initialization a preload hook is added that overrides ours, so we need to re-instate it here.
	  This is wasteful, but there's no way to manipulate the preload hook list except by adding to it.
	*/
	mono_install_assembly_preload_hook (xamarin_assembly_preload_hook, NULL);
	DEBUG_LAUNCH_TIME_PRINT ("\tJIT init time");
}
#endif // !LEGACY_XAMARIN_MAC

static MonoClass *
get_class_from_name (MonoImage* image, const char *nmspace, const char *name, bool optional = false)
{
	// COOP: this is a convenience function executed only at startup, I believe the mode here doesn't matter.	
	MonoClass *rv = mono_class_from_name (image, nmspace, name);
	if (!rv && !optional)
		xamarin_assertion_message ("Fatal error: failed to load the class '%s.%s'\n.", nmspace, name);
	return rv;
}

void
xamarin_bridge_call_runtime_initialize (struct InitializationOptions* options, GCHandle* exception_gchandle)
{
	MonoMethod *runtime_initialize;
	void* params[2];
	MonoObject *exc = NULL;
	MonoImage* platform_image = NULL;

	entry_assembly = xamarin_open_assembly (PRODUCT_DUAL_ASSEMBLY);

	if (!entry_assembly)
		xamarin_assertion_message ("Failed to load %s.", PRODUCT_DUAL_ASSEMBLY);
	platform_image = mono_assembly_get_image (entry_assembly);

	const char *objcruntime = "ObjCRuntime";
	const char *foundation = "Foundation";

	runtime_class = get_class_from_name (platform_image, objcruntime, "Runtime");
	inativeobject_class = get_class_from_name (platform_image, objcruntime, "INativeObject");
	nsobject_class = get_class_from_name (platform_image, foundation, "NSObject");
	nsnumber_class = get_class_from_name (platform_image, foundation, "NSNumber", true);
	nsvalue_class = get_class_from_name (platform_image, foundation, "NSValue", true);
	nsstring_class = get_class_from_name (platform_image, foundation, "NSString", true);

	runtime_initialize = mono_class_get_method_from_name (runtime_class, "Initialize", 1);

	if (runtime_initialize == NULL)
		xamarin_assertion_message ("Fatal error: failed to load the %s.%s method", "Runtime", "Initialize");

	params [0] = options;

	mono_runtime_invoke (runtime_initialize, NULL, params, &exc);

	if (exc)
		*exception_gchandle = xamarin_gchandle_new (exc, false);
}

void
xamarin_bridge_register_product_assembly (GCHandle* exception_gchandle)
{
	xamarin_register_monoassembly (entry_assembly, exception_gchandle);
	// We don't need the entry_assembly around anymore, so release it.
	xamarin_mono_object_release (&entry_assembly);
}

MonoMethod *
xamarin_bridge_get_mono_method (MonoReflectionMethod *method)
{
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;

	PublicMonoReflectionMethod *rm = (PublicMonoReflectionMethod *) method;
	return rm->method;
}

MonoClass *
xamarin_get_inativeobject_class ()
{
	if (inativeobject_class == NULL)
		xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: can't get the %s class because it's been linked away.\n", "INativeObject");
	return inativeobject_class;
}

MonoClass *
xamarin_get_nsobject_class ()
{
	if (nsobject_class == NULL)
		xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: can't get the %s class because it's been linked away.\n", "NSObject");
	return nsobject_class;
}

MonoClass *
xamarin_get_nsvalue_class ()
{
	if (nsvalue_class == NULL)
		xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: can't get the %s class because it's been linked away.\n", "NSValue");
	return nsvalue_class;
}

MonoClass *
xamarin_get_nsnumber_class ()
{
	if (nsnumber_class == NULL)
		xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: can't get the %s class because it's been linked away.\n", "NSNumber");
	return nsnumber_class;
}

MonoClass *
xamarin_get_nsstring_class ()
{
	if (nsstring_class == NULL)
		xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: can't get the %s class because it's been linked away.\n", "NSString");
	return nsstring_class;
}

MonoClass *
xamarin_get_runtime_class ()
{
	if (runtime_class == NULL)
		xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: can't get the %s class because it's been linked away.\n", "Runtime");
	return runtime_class;
}

/* Wrapping threads with NSAutoreleasePool
 *
 * We must create an NSAutoreleasePool for each thread, so users
 * don't have to do it manually.
 *
 * Use mono's profiling API to get notified for thread start/stop,
 * and create a pool that spans the thread's entire lifetime.
 */

static CFMutableDictionaryRef xamarin_thread_hash = NULL;
static pthread_mutex_t thread_hash_lock = PTHREAD_MUTEX_INITIALIZER;

static void
xamarin_thread_start (void *user_data)
{
	// COOP: no managed memory access: any mode. Switching to safe mode since we're locking a mutex.
	NSAutoreleasePool *pool;

	if (mono_thread_is_foreign (mono_thread_current ()))
		return;

	MONO_ENTER_GC_SAFE;

	pool = [[NSAutoreleasePool alloc] init];

	pthread_mutex_lock (&thread_hash_lock);

	CFDictionarySetValue (xamarin_thread_hash, GINT_TO_POINTER (pthread_self ()), pool);

	pthread_mutex_unlock (&thread_hash_lock);

	MONO_EXIT_GC_SAFE;
}

static void
xamarin_thread_finish (void *user_data)
{
	// COOP: no managed memory access: any mode. Switching to safe mode since we're locking a mutex.
	NSAutoreleasePool *pool;

	MONO_ENTER_GC_SAFE;

	/* Don't drain the pool while holding the thread hash lock. */
	pthread_mutex_lock (&thread_hash_lock);

	pool = (NSAutoreleasePool *) CFDictionaryGetValue (xamarin_thread_hash, GINT_TO_POINTER (pthread_self ()));
	if (pool)
		CFDictionaryRemoveValue (xamarin_thread_hash, GINT_TO_POINTER (pthread_self ()));

	pthread_mutex_unlock (&thread_hash_lock);

	if (pool)
		[pool drain];

	MONO_EXIT_GC_SAFE;
}

static void
thread_start (MonoProfiler *prof, uintptr_t tid)
{
	// COOP: no managed memory access: any mode.
	xamarin_thread_start (NULL);
}

static void
thread_end (MonoProfiler *prof, uintptr_t tid)
{
	// COOP: no managed memory access: any mode.
	xamarin_thread_finish (NULL);
}

void
xamarin_install_nsautoreleasepool_hooks ()
{
	// COOP: executed at startup (and no managed memory access): any mode.
	xamarin_thread_hash = CFDictionaryCreateMutable (kCFAllocatorDefault, 0, NULL, NULL);

	mono_profiler_install_thread (thread_start, thread_end);
}

#if DOTNET

bool
xamarin_bridge_vm_initialize (int propertyCount, const char **propertyKeys, const char **propertyValues)
{
	int rv;

	rv = monovm_initialize (propertyCount, propertyKeys, propertyValues);

	LOG_MONOVM (stderr, "xamarin_vm_initialize (%i, %p, %p): rv: %i\n", propertyCount, propertyKeys, propertyValues, rv);

	return rv == 0;
}

#endif // DOTNET

#endif // !CORECLR_RUNTIME
