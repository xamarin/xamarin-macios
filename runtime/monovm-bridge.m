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
#include <sys/mman.h>
#include <sys/stat.h>

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
#if DOTNET
static MonoClass* nativehandle_class = NULL;
#endif

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
	if (xamarin_register_modules != NULL)
		xamarin_register_modules ();
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

void
xamarin_bridge_shutdown ()
{
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
#if DOTNET
	nativehandle_class = get_class_from_name (platform_image, objcruntime, "NativeHandle");
#endif
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

#if DOTNET
MonoClass *
xamarin_get_nativehandle_class ()
{
	if (nativehandle_class == NULL)
		xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: can't get the %s class because it's been linked away.\n", "NativeHandle");
	return nativehandle_class;
}
#endif

MonoClass *
xamarin_get_nsobject_class ()
{
	if (nsobject_class == NULL)
		xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: can't get the %s class because it's been linked away.\n", "NSObject");
	return nsobject_class;
}

MonoType *
xamarin_get_nsvalue_type ()
{
	if (nsvalue_class == NULL)
		xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: can't get the %s class because it's been linked away.\n", "NSValue");
	return mono_class_get_type (nsvalue_class);
}

MonoType *
xamarin_get_nsnumber_type ()
{
	if (nsnumber_class == NULL)
		xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: can't get the %s class because it's been linked away.\n", "NSNumber");
	return mono_class_get_type (nsnumber_class);
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

#if !DOTNET
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
#endif // !DOTNET

void
xamarin_install_nsautoreleasepool_hooks ()
{
	// No need to do anything here for CoreCLR.
#if !DOTNET
	// COOP: executed at startup (and no managed memory access): any mode.
	xamarin_thread_hash = CFDictionaryCreateMutable (kCFAllocatorDefault, 0, NULL, NULL);

	mono_profiler_install_thread (thread_start, thread_end);
#endif // !DOTNET
}

void
xamarin_bridge_free_mono_signature (MonoMethodSignature **psig)
{
	// nothing to free here
	*psig = NULL;
}

bool
xamarin_is_class_nsobject (MonoClass *cls)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;

	return mono_class_is_subclass_of (cls, xamarin_get_nsobject_class (), false);
}

bool
xamarin_is_class_inativeobject (MonoClass *cls)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;

	return mono_class_is_subclass_of (cls, xamarin_get_inativeobject_class (), true);
}

#if DOTNET
bool
xamarin_is_class_nativehandle (MonoClass *cls)
{
	return cls == xamarin_get_nativehandle_class ();
}
#endif

bool
xamarin_is_class_array (MonoClass *cls)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;

	return mono_class_is_subclass_of (cls, mono_get_array_class (), false);
}

bool
xamarin_is_class_nsnumber (MonoClass *cls)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;

	if (nsnumber_class == NULL)
		return false;

	return mono_class_is_subclass_of (cls, nsnumber_class, false);
}

bool
xamarin_is_class_nsvalue (MonoClass *cls)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;

	if (nsvalue_class == NULL)
		return false;

	return mono_class_is_subclass_of (cls, nsvalue_class, false);
}

bool
xamarin_is_class_nsstring (MonoClass *cls)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;

	MonoClass *nsstring_class = xamarin_get_nsstring_class ();
	if (nsstring_class == NULL)
		return false;

	return mono_class_is_subclass_of (cls, nsstring_class, false);
}

bool
xamarin_is_class_intptr (MonoClass *cls)
{
	return cls == mono_get_intptr_class ();
}

bool
xamarin_is_class_string (MonoClass *cls)
{
	return cls == mono_get_string_class ();
}

MonoException *
xamarin_create_system_invalid_cast_exception (const char *message)
{
	return (MonoException *) mono_exception_from_name_msg (mono_get_corlib (), "System", "InvalidCastException", message);
}

MonoException *
xamarin_create_system_exception (const char *message)
{
	return (MonoException *) mono_exception_from_name_msg (mono_get_corlib (), "System", "Exception", message);
}

MonoException *
xamarin_create_system_entry_point_not_found_exception (const char *entrypoint)
{
	return (MonoException *) mono_exception_from_name_msg (mono_get_corlib (), "System", "EntryPointNotFoundException", entrypoint);
}

#if DOTNET

static void
xamarin_runtime_config_cleanup (MonovmRuntimeConfigArguments *args, void *user_data)
{
	free ((char *) args->runtimeconfig.name.path);
	free (args);
}

static void
xamarin_initialize_runtime_config ()
{
	if (xamarin_runtime_configuration_name == NULL) {
		LOG (PRODUCT ": No runtime config file provided at build time.\n");
		return;
	}

	char path [1024];
	if (!xamarin_locate_app_resource (xamarin_runtime_configuration_name, path, sizeof (path))) {
		LOG (PRODUCT ": Could not locate the runtime config file '%s' in the app bundle.\n", xamarin_runtime_configuration_name);
		return;
	}

	MonovmRuntimeConfigArguments *args = (MonovmRuntimeConfigArguments *) calloc (sizeof (MonovmRuntimeConfigArguments), 1);
	args->kind = 0; // Path of runtimeconfig.blob
	args->runtimeconfig.name.path = strdup (path);

	int rv = monovm_runtimeconfig_initialize (args, xamarin_runtime_config_cleanup, NULL);
	if (rv != 0) {
		LOG_MONOVM (PRODUCT ": Failed to load the runtime config file %s: %i\n", path, rv);
		return;
	}

	LOG_MONOVM (PRODUCT ": Loaded the runtime config file %s\n", path);
}

bool
xamarin_bridge_vm_initialize (int propertyCount, const char **propertyKeys, const char **propertyValues)
{
	int rv;

	xamarin_initialize_runtime_config ();

	rv = monovm_initialize (propertyCount, propertyKeys, propertyValues);

	LOG_MONOVM (stderr, "xamarin_vm_initialize (%i, %p, %p): rv: %i\n", propertyCount, propertyKeys, propertyValues, rv);

	return rv == 0;
}

// We have a P/Invoke to xamarin_mono_object_retain in managed code, but the
// corresponding native method only really exists when using CoreCLR. However,
// the P/Invoke might not always be linked away (if the linker isn't enabled
// for instance), in which case we must still have a native function. So
// provide an empty implementation of xamarin_mono_object_retain (since it
// doesn't have to do anything when using MonoVM). We still keep the #define
// that does nothing, so that all the native code that calls
// xamarin_mono_object_retain, will completely disappear when using MonoVM.
#undef xamarin_mono_object_retain
extern "C" {
	void xamarin_mono_object_retain (MonoObject *mobj);
}
void
xamarin_mono_object_retain (MonoObject *mobj)
{
	// Nothing to do here
}

#if defined (TRACK_MONOOBJECTS)
// This function is needed for the corresponding managed P/Invoke to not make
// the native linker fail due to an unresolved symbol. This method should
// never end up being called (it'll be linked away by the native linker if the
// managed linker removes the P/Invoke, and never called from managed code
// otherwise).
void
xamarin_bridge_log_monoobject (MonoObject *mobj, const char *stacktrace)
{
	xamarin_assertion_message ("%s is not available on MonoVM", __func__);
}
#endif // defined (TRACK_MONOOBJECTS)

#endif // DOTNET

/*
 * ToggleRef support
 */
// #define DEBUG_TOGGLEREF 1

static void
gc_register_toggleref (MonoObject *obj, id self, bool isCustomType)
{
	// COOP: This is an icall, at entry we're in unsafe mode. Managed memory is accessed, so we stay in unsafe mode.
	MONO_ASSERT_GC_UNSAFE;

#ifdef DEBUG_TOGGLEREF
	id handle = xamarin_get_nsobject_handle (obj);

	PRINT ("**Registering object %p handle %p RC %d flags: %i isCustomType: %i",
		obj,
		handle,
		(int) (handle ? [handle retainCount] : 0),
		xamarin_get_nsobject_flags (obj),
		isCustomType
		);
#endif
	mono_gc_toggleref_add (obj, TRUE);

	// Make sure the GCHandle we have is a weak one for custom types.
	if (isCustomType) {
		MONO_ENTER_GC_SAFE;
		xamarin_switch_gchandle (self, true);
		MONO_EXIT_GC_SAFE;
	}
}

static MonoToggleRefStatus
gc_toggleref_callback (MonoObject *object)
{
	// COOP: this is a callback called by the GC, so I assume the mode here doesn't matter
	MonoToggleRefStatus res;
	uint8_t flags = xamarin_get_nsobject_flags (object);

	res = xamarin_gc_toggleref_callback (flags, NULL, xamarin_get_nsobject_handle, object);

	return res;
}

static void
gc_event_callback (MonoProfiler *prof, MonoGCEvent event, int generation)
{
	// COOP: this is a callback called by the GC, I believe the mode here doesn't matter.
	xamarin_gc_event (event);
}

void
xamarin_enable_new_refcount ()
{
	mono_gc_toggleref_register_callback (gc_toggleref_callback);

	xamarin_add_internal_call ("Foundation.NSObject::RegisterToggleRef", (const void *) gc_register_toggleref);
	mono_profiler_install_gc (gc_event_callback, NULL);
}

#endif // !CORECLR_RUNTIME
