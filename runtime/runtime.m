/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
*  Authors: Rolf Bjarne Kvinge
*
*  Copyright (C) 2014 Xamarin Inc. (www.xamarin.com)
*
*/

#include <pthread.h>
#include <objc/runtime.h>
#include <sys/stat.h>
#include <dlfcn.h>
#include <mach/mach.h>
#include <mach-o/dyld.h>
#include <mach-o/loader.h>
#include <Security/Security.h>

#include "product.h"
#include "shared.h"
#include "delegates.h"
#include "runtime-internal.h"
#include "xamarin/xamarin.h"
#include "host_runtime_contract.h"

#if !defined (CORECLR_RUNTIME)
#include "xamarin/monovm-bridge.h"
#else
#include "xamarin/coreclr-bridge.h"
#endif

#if defined (DEBUG)
//extern BOOL NSZombieEnabled;
#endif

/*
 * These are values that can be configured in xamarin_setup.
 *
 * The defaults values here have been chosen to minimize
 * the configuration required for the simulator (in particular
 * the simlauncher binaries).
 */

#if MONOMAC
// FIXME: implement release mode for monomac.
bool xamarin_debug_mode = true;
bool xamarin_mac_hybrid_aot = false;
bool xamarin_mac_modern = false;
#else
bool xamarin_debug_mode = false;
#endif
bool xamarin_disable_lldb_attach = false;
bool xamarin_disable_omit_fp = false;
#if DEBUG
bool xamarin_init_mono_debug = true;
#else
bool xamarin_init_mono_debug = false;
#endif
int xamarin_log_level = 0;
const char *xamarin_executable_name = NULL;
#if MONOMAC || TARGET_OS_MACCATALYST
NSString * xamarin_custom_bundle_name = @"MonoBundle";
#endif
#if MONOMAC
bool xamarin_is_mkbundle = false;
char *xamarin_entry_assembly_path = NULL;
#endif
#if defined (__x86_64__)
const char *xamarin_arch_name = "x86_64";
#else
const char *xamarin_arch_name = NULL;
#endif

enum MarshalObjectiveCExceptionMode xamarin_marshal_objectivec_exception_mode = MarshalObjectiveCExceptionModeDefault;
enum MarshalManagedExceptionMode xamarin_marshal_managed_exception_mode = MarshalManagedExceptionModeDefault;
enum XamarinTriState xamarin_log_exceptions = XamarinTriStateNone;
enum XamarinLaunchMode xamarin_launch_mode = XamarinLaunchModeApp;
#if SUPPORTS_DYNAMIC_REGISTRATION
bool xamarin_supports_dynamic_registration = true;
#endif
const char *xamarin_runtime_configuration_name = NULL;

// The runtimeconfig.json 'configProperties' baked into the app as C arrays (CoreCLR only).
// These are set from the generated main (xamarin_setup_impl) and consumed by
// xamarin_bridge_compute_properties when initializing CoreCLR.
int xamarin_runtime_config_property_count = 0;
const char **xamarin_runtime_config_property_keys = NULL;
const char **xamarin_runtime_config_property_values = NULL;

enum XamarinNativeLinkMode xamarin_libmono_native_link_mode = XamarinNativeLinkModeStaticObject;
const char **xamarin_runtime_libraries = NULL;
struct xamarin_r2r_module *xamarin_r2r_modules = NULL;
int xamarin_r2r_module_count = 0;
const char * const *xamarin_trusted_platform_assembly_names = NULL;
#if defined (SUPPORTS_UNIVERSAL_BUILDS)
bool xamarin_is_multi_rid_build = false;
#endif

/* Callbacks */

xamarin_setup_callback xamarin_setup = NULL;
xamarin_register_module_callback xamarin_register_modules = NULL;
xamarin_register_assemblies_callback xamarin_register_assemblies = NULL;
xamarin_extension_main_callback xamarin_extension_main = NULL;

/* Local variable */

static pthread_mutex_t framework_peer_release_lock;
static MonoGHashTable *xamarin_wrapper_hash;

// Hash table mapping native object pointers (id) to strong GCHandles.
// Used by the dual-gchandle scheme: the object's gc_handle is always a weak
// handle used for lookups, while this table holds an optional strong handle
// that keeps the managed object alive. See xamarin_switch_gchandle.
static CFMutableDictionaryRef strong_gchandle_hash = NULL;
static pthread_mutex_t strong_gchandle_hash_lock = PTHREAD_MUTEX_INITIALIZER;

static bool initialize_started = FALSE;

#include "delegates.inc"

/* Keep Trampolines, InitializationFlags and InitializationOptions in sync with Runtime.cs */

struct Trampolines {
	void* tramp;
	void* stret_tramp;
	void* fpret_single_tramp;
	void* fpret_double_tramp;
	void* release_tramp;
	void* retain_tramp;
	void* static_tramp;
	void* ctor_tramp;
	void* static_fpret_single_tramp;
	void* static_fpret_double_tramp;
	void* static_stret_tramp;
	void* long_tramp;
	void* static_long_tramp;
#if MONOMAC
	void* copy_with_zone1;
	void* copy_with_zone2;
#endif
	void* get_gchandle_tramp;
	void* set_gchandle_tramp;
	void* get_flags_tramp;
	void* set_flags_tramp;
	void* retainWeakReference_tramp;
	void* get_nsobject_data_tramp;
};

enum InitializationFlags : int {
	InitializationFlagsIsPartialStaticRegistrar = 0x01,
	InitializationFlagsIsManagedStaticRegistrar = 0x02,
	InitializationFlagsIsTrimmableStaticRegistrar = 0x04,
	/* unused									= 0x08,*/
	InitializationFlagsIsSimulator				= 0x10,
	InitializationFlagsIsCoreCLR                = 0x20,
	InitializationFlagsIsNativeAOT              = 0x40,
};

struct InitializationOptions {
	int size; // the size of this structure. This is used for version checking.
	enum InitializationFlags flags;
	struct Delegates* Delegates;
	struct Trampolines* Trampolines;
	struct MTRegistrationMap* RegistrationData;
	enum MarshalObjectiveCExceptionMode MarshalObjectiveCExceptionMode;
	enum MarshalManagedExceptionMode MarshalManagedExceptionMode;
	struct AssemblyLocations* AssemblyLocations;
	// This struct must be kept in sync with the corresponding struct in Runtime.cs, and since we use the same managed code for both MonoVM and CoreCLR,
	// we can't restrict the following fields to CORECLR_RUNTIME only, we can only exclude it from legacy Xamarin.
	void *xamarin_objc_msgsend;
	void *xamarin_objc_msgsend_super;
	void *xamarin_objc_msgsend_stret;
	void *xamarin_objc_msgsend_super_stret;
	void *unhandled_exception_handler;
	void *reference_tracking_begin_end_callback;
	void *reference_tracking_is_referenced_callback;
	void *reference_tracking_tracked_object_entered_finalization;
};

static struct Trampolines trampolines = {
	(void *) &xamarin_trampoline,
	(void *) &xamarin_stret_trampoline,
	(void *) &xamarin_fpret_single_trampoline,
	(void *) &xamarin_fpret_double_trampoline,
	(void *) &xamarin_release_trampoline,
	(void *) &xamarin_retain_trampoline,
	(void *) &xamarin_static_trampoline,
	(void *) &xamarin_ctor_trampoline,
	(void *) &xamarin_static_fpret_single_trampoline,
	(void *) &xamarin_static_fpret_double_trampoline,
	(void *) &xamarin_static_stret_trampoline,
	(void *) &xamarin_longret_trampoline,
	(void *) &xamarin_static_longret_trampoline,
#if MONOMAC
	(void *) &xamarin_copyWithZone_trampoline1,
	(void *) &xamarin_copyWithZone_trampoline2,
#endif
	(void *) &xamarin_get_gchandle_trampoline,
	(void *) &xamarin_set_gchandle_trampoline,
	(void *) &xamarin_get_flags_trampoline,
	(void *) &xamarin_set_flags_trampoline,
	(void *) &xamarin_retainWeakReference_trampoline,
	(void *) &xamarin_get_nsobject_data_trampoline,
};

static struct InitializationOptions options = { };

#if !defined (CORECLR_RUNTIME)
void
xamarin_add_internal_call (const char *name, const void *method)
{
	mono_add_internal_call (name, method);
}
#endif // !CORECLR_RUNTIME

id
xamarin_get_nsobject_handle (MonoObject *obj)
{
#if defined (CORECLR_RUNTIME)
	id rv = xamarin_get_handle_for_inativeobject (obj);
	LOG_CORECLR (stderr, "xamarin_get_nsobject_handle (%p) => %p\n", obj, rv);
	return rv;
#else
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	return mobj->data->handle;
#endif
}

uint32_t
xamarin_get_nsobject_flags (MonoObject *obj)
{
#if defined (CORECLR_RUNTIME)
	return xamarin_get_flags_for_nsobject (obj->gchandle);
#else
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	if (mobj->data)
		return mobj->data->flags;
	return NSObjectFlagsDisposed;
#endif
}

void
xamarin_set_nsobject_flags (MonoObject *obj, uint32_t flags)
{
#if defined (CORECLR_RUNTIME)
	xamarin_set_flags_for_nsobject (obj->gchandle, flags);
#else
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	mobj->data->flags = flags;
#endif
}

uint32_t
xamarin_get_nsobject_id_flags (id obj)
{
	NSObjectData *data = xamarin_get_nsobject_data (obj);
	if (data)
		return data->flags;
	return 0;
}

MonoType *
xamarin_get_parameter_type (MonoMethod *managed_method, int index)
{
	MonoMethodSignature *msig = mono_method_signature (managed_method);
	void *iter = NULL;
	MonoType *p = NULL;
	
	if (index == -1) {
		p = mono_signature_get_return_type (msig);
	} else {
		for (int i = 0; i < index + 1; i++) {
			xamarin_mono_object_release (&p);
			p = mono_signature_get_params (msig, &iter);
		}
	}
	
	xamarin_bridge_free_mono_signature (&msig);

	return p;
}

MonoObject *
xamarin_get_nsobject_with_type_for_ptr (id self, bool owns, MonoType* type, GCHandle *exception_gchandle)
{
	int32_t created;
	return xamarin_get_nsobject_with_type_for_ptr_created (self, owns, type, &created, exception_gchandle);
}

MonoObject *
xamarin_get_nsobject_with_type_for_ptr_created (id self, bool owns, MonoType *type, int32_t *created, GCHandle *exception_gchandle)
{
	MonoObject *mobj = NULL;
	GCHandle gchandle = INVALID_GCHANDLE;

	*created = false;

	if (self == NULL)
		return NULL;

	gchandle = xamarin_get_gchandle (self);

	if (gchandle != INVALID_GCHANDLE) {
		mobj = xamarin_gchandle_get_target (gchandle);
		MonoClass *klass = mono_class_from_mono_type (type);
		bool isinst = mono_object_isinst (mobj, klass) != NULL;
		xamarin_mono_object_release (&klass);

		if (isinst) {
			return mobj;
		} else {
			xamarin_mono_object_release (&mobj);
		}
	}

	MonoReflectionType *rtype = mono_type_get_object (mono_domain_get (), type);
	MonoObject *rv = xamarin_get_nsobject_with_type (self, rtype, created, exception_gchandle);
	xamarin_mono_object_release (&rtype);
	return rv;
}

MonoObject *
xamarin_get_managed_object_for_ptr_fast (id self, GCHandle *exception_gchandle)
{
	MonoObject *mobj = NULL;
	GCHandle gchandle = INVALID_GCHANDLE;

	gchandle = xamarin_get_gchandle (self);

	if (gchandle == INVALID_GCHANDLE) {
		mobj = xamarin_try_get_or_construct_nsobject (self, exception_gchandle);
	} else {
		mobj = xamarin_gchandle_get_target (gchandle);
#if DEBUG
		if (self != xamarin_get_nsobject_handle (mobj)) {
			xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/dotnet/macios/issues/new). Additional data: found managed object %p=%p (%s) in native object %p (%s).\n",
				mobj, xamarin_get_nsobject_handle (mobj), xamarin_class_get_full_name (mono_object_get_class (mobj), exception_gchandle), self, object_getClassName (self));
		}
#endif
	}

	return mobj;
}

// See comments in the following methods to explain the logic here:
// xamarin_marshal_return_value_impl in trampolines.m
// xamarin_release_managed_ref in runtime.m
void xamarin_framework_peer_waypoint ()
{
	pthread_mutex_lock (&framework_peer_release_lock);
	pthread_mutex_unlock (&framework_peer_release_lock);
}

// Same as xamarin_framework_peer_waypoint, except the current mode should be GC Safe.
void xamarin_framework_peer_waypoint_safe ()
{
	pthread_mutex_lock (&framework_peer_release_lock);
	pthread_mutex_unlock (&framework_peer_release_lock);
}

MonoObject *
xamarin_new_nsobject (id self, MonoClass *klass, GCHandle *exception_gchandle)
{
	MonoType *type = mono_class_get_type (klass);
	MonoReflectionType *rtype = mono_type_get_object (mono_domain_get (), type);
	xamarin_mono_object_release (&type);

	GCHandle obj = xamarin_create_nsobject (rtype, self, NSObjectFlagsNativeRef, exception_gchandle);
	xamarin_mono_object_release (&rtype);
	return xamarin_gchandle_unwrap (obj);
}

// Returns if a MonoClass is nullable.
// Will also return the element type (it the type is nullable, and if out pointer is not NULL).
bool
xamarin_is_class_nullable (MonoClass *cls, MonoClass **element_type, GCHandle *exception_gchandle)
{
#ifdef DYNAMIC_MONO_RUNTIME
	// mono_class_is_nullable/mono_class_get_nullable_param are private
	// functions, and as such we can't call find them in libmono.dylib. In
	// this case we manually call a managed function to do the work for us (we
	// don't use the normal delegate mechanism, because how it's currently
	// implemented it would inflict size costs on all platforms, not just
	// Xamarin.Mac).
	if (!mono_class_is_nullable_exists () || !mono_class_get_nullable_param_exists ()) {
		static MonoMethod *get_nullable_type = NULL;

		if (get_nullable_type == NULL)
			get_nullable_type = mono_class_get_method_from_name (xamarin_get_runtime_class (), "GetNullableType", 1);

		GCHandle type_handle = xamarin_gchandle_new ((MonoObject *) mono_type_get_object (mono_domain_get (), mono_class_get_type (cls)), false);
		void *args [1] { type_handle };
		MonoObject *exc = NULL;
		MonoObject *nullable_type_handle = mono_runtime_invoke (get_nullable_type, NULL, args, &exc);
		xamarin_gchandle_free (type_handle);
		if (exc != NULL) {
			*exception_gchandle = xamarin_gchandle_new (exc, FALSE);
			return false;
		}

		MonoReflectionType *nullable_type = (MonoReflectionType *) xamarin_gchandle_unwrap (nullable_type_handle);

		if (element_type != NULL && nullable_type != NULL) {
			MonoType *mono_type = mono_reflection_type_get_type (nullable_type);
			*element_type = mono_class_from_mono_type (mono_type);
			xamarin_mono_object_release (&mono_type);
		}

		bool is_nullable = nullable_type != NULL;
		xamarin_mono_object_release (&nullable_type);
		return is_nullable;
	}
#endif

	bool rv = mono_class_is_nullable (cls);
	if (rv && element_type)
		*element_type = mono_class_get_nullable_param (cls);
	return rv;
}

MonoClass *
xamarin_get_nullable_type (MonoClass *cls, GCHandle *exception_gchandle)
{
	MonoClass *rv = NULL;
	xamarin_is_class_nullable (cls, &rv, exception_gchandle);
	return rv;
}

// The XamarinExtendedObject protocol is just to avoid a 
// compiler warning (no 'xamarinGetGChandle' selector found).
@protocol XamarinExtendedObject
-(GCHandle) xamarinGetGCHandle;
-(bool) xamarinSetGCHandle: (GCHandle) gc_handle flags: (enum XamarinGCHandleFlags) flags data: (struct NSObjectData *) data;
-(enum XamarinGCHandleFlags) xamarinGetGCHandleFlags;
-(void) xamarinSetGCHandleFlags: (enum XamarinGCHandleFlags) gchandle_flags;
-(struct NSObjectData*) xamarinGetNSObjectData;
@end

static inline bool
set_gchandle (id self, GCHandle gc_handle, enum XamarinGCHandleFlags flags, struct NSObjectData *data)
{
	bool rv;

	id<XamarinExtendedObject> xself = self;
	rv = [xself xamarinSetGCHandle: gc_handle flags: flags data: data];

	return rv;
}

static inline bool
set_gchandle_safe (id self, GCHandle gc_handle, enum XamarinGCHandleFlags flags, struct NSObjectData *data)
{
	bool rv;

	id<XamarinExtendedObject> xself = self;
	rv = [xself xamarinSetGCHandle: gc_handle flags: flags data: data];

	return rv;
}

static inline GCHandle
get_gchandle_without_flags (id self)
{
	GCHandle rv;
	id<XamarinExtendedObject> xself = self;
	rv = (GCHandle) [xself xamarinGetGCHandle];
	return rv;
}

static inline GCHandle
get_gchandle_with_flags (id self, enum XamarinGCHandleFlags* gchandle_flags)
{
	GCHandle rv;
	id<XamarinExtendedObject> xself = self;
	rv = (GCHandle) [xself xamarinGetGCHandle];
	if (gchandle_flags != NULL)
		*gchandle_flags = [xself xamarinGetGCHandleFlags];
	return rv;
}

static inline enum XamarinGCHandleFlags
get_flags (id self)
{
	enum XamarinGCHandleFlags rv;
	id<XamarinExtendedObject> xself = self;
	rv = [xself xamarinGetGCHandleFlags];

	return rv;
}

static inline void
set_gchandle_flags_safe (id self, enum XamarinGCHandleFlags flags)
{
	id<XamarinExtendedObject> xself = self;
	[xself xamarinSetGCHandleFlags: flags];
}

static inline enum XamarinGCHandleFlags
get_flags_safe (id self)
{
	enum XamarinGCHandleFlags rv;
	id<XamarinExtendedObject> xself = self;
	rv = [xself xamarinGetGCHandleFlags];

	return rv;
}

struct NSObjectData *
xamarin_get_nsobject_data (id self)
{
	struct NSObjectData * rv;
	id<XamarinExtendedObject> xself = self;
	rv = [xself xamarinGetNSObjectData];

	return rv;
}

GCHandle
xamarin_get_gchandle (id self)
{
	return get_gchandle_without_flags (self);
}

GCHandle
xamarin_get_gchandle_with_flags (id self, enum XamarinGCHandleFlags* flags)
{
	return get_gchandle_with_flags (self, flags);
}

bool
xamarin_has_managed_ref (id self)
{
	return (get_flags (self) & XamarinGCHandleFlags_HasManagedRef) == XamarinGCHandleFlags_HasManagedRef;
}

bool
xamarin_has_managed_ref_safe (id self)
{
	return (get_flags_safe (self) & XamarinGCHandleFlags_HasManagedRef) == XamarinGCHandleFlags_HasManagedRef;
}

MonoException *
xamarin_create_exception (const char *msg)
{
	return xamarin_create_system_exception (msg);
}

MonoMethod *
xamarin_get_reflection_method_method (MonoReflectionMethod *method)
{
	return xamarin_bridge_get_mono_method (method);
}

id
xamarin_get_handle (MonoObject *obj, GCHandle *exception_gchandle)
{
	MonoClass *klass;
	id rv = nil;

	if (obj == NULL)
		return nil;

	klass = mono_object_get_class (obj);

	if (xamarin_is_class_nsobject (klass)) {
		rv = xamarin_get_nsobject_handle (obj);
	} else if (xamarin_is_class_inativeobject (klass)) {
		rv = xamarin_get_handle_for_inativeobject (obj, exception_gchandle);
	} else {
		char *msg = xamarin_strdup_printf ("Unable to marshal from %s.%s to an Objective-C object. "
									"The managed class must either inherit from NSObject or implement INativeObject.",
									mono_class_get_namespace (klass), mono_class_get_name (klass));
		GCHandle ex_handle = xamarin_create_runtime_exception (8039, msg, exception_gchandle);
		xamarin_free (msg);
		if (*exception_gchandle == INVALID_GCHANDLE)
			*exception_gchandle = ex_handle;
	}

	xamarin_mono_object_release (&klass);
	
	return rv;
}

#if DEBUG
static void 
verify_cast (MonoClass *to, MonoObject *obj, Class from_class, SEL sel, MonoMethod *method, GCHandle *exception_gchandle)
{
	if (!to)
		return;

	if (mono_object_isinst (obj, to) == NULL) {
		MonoClass *from = mono_object_get_class (obj);
		char *method_full_name = mono_method_full_name (method, TRUE);
		char *from_name = xamarin_class_get_full_name (from, exception_gchandle);
		char *to_name = xamarin_class_get_full_name (to, exception_gchandle);
		char *msg = xamarin_strdup_printf ("Unable to cast object of type '%s' (Objective-C type: '%s') to type '%s'.\n"
		"Additional information:\n"
		"\tSelector: %s\n"
		"\tMethod: %s\n", from_name, class_getName(from_class), to_name, sel_getName (sel), method_full_name);
		MonoException *mono_ex = xamarin_create_system_invalid_cast_exception (msg);
		mono_free (from_name);
		mono_free (to_name);
		xamarin_free (msg);
		mono_free (method_full_name);
		xamarin_mono_object_release (&from);
		*exception_gchandle = xamarin_gchandle_new ((MonoObject *) mono_ex, FALSE);
	}
}
#endif

void
xamarin_check_for_gced_object (MonoObject *obj, SEL sel, id self, MonoMethod *method, GCHandle *exception_gchandle)
{
	if (obj != NULL) {
#if DEBUG
		MonoClass *declaring_type = mono_method_get_class (method);
		verify_cast (declaring_type, obj, [self class], sel, method, exception_gchandle);
		xamarin_mono_object_release (&declaring_type);
#endif
		return;
	}
	
	char *method_full_name = mono_method_full_name (method, TRUE);
	char *type_name = xamarin_lookup_managed_type_name ([self class], exception_gchandle);
	if (*exception_gchandle == INVALID_GCHANDLE) {
		char *msg = xamarin_strdup_printf ("Failed to marshal the Objective-C object %p (type: %s). "
		"Could not find an existing managed instance for this object, "
		"nor was it possible to create a new managed instance "
		"(because the type '%s' does not have a constructor that takes one NativeHandle argument).\n"
		"Additional information:\n"
		"\tSelector: %s\n"
		"\tMethod: %s\n", self, object_getClassName (self), type_name, sel_getName (sel), method_full_name);
		GCHandle ex_handle = xamarin_create_runtime_exception (8027, msg, exception_gchandle);
		xamarin_free (msg);
		if (*exception_gchandle == INVALID_GCHANDLE)
			*exception_gchandle = ex_handle;
	}
	mono_free (type_name);
	mono_free (method_full_name);
}

#if DEBUG
//
// We can't do the type-checks below correctly until we support multiple managed peers for each
// native objects. The problem is that Objective-C can fake multiple inheritance by
// overriding isKindOfClass: A test case can be seen in bug #23421: A parameter in a
// callback from Objective-C is typed as 'NSUrlSessionDownloadTask'. Objective-C may give us an instance
// of an internal class __NSCFBackgroundDownloadTask, which doesn't inherit from NSUrlSessionDownloadTask
// (it inherits from NSUrlSessionTask, which is a superclass of NSUrlSessionDownloadTask). In Objective-C
// the __NSCFBackgroundDownloadTask class gets away with this because it overrides isKindOfClass: to return
// YES for NSUrlSessionDownloadTask. We can't rely on isKindOfClass: when creating a managed peer for
// the native object, because we may already have an instance of the "correct" type (if we
// had support for multiple managed peers, we could just create a new instance of the expected type).
// 

void
xamarin_verify_parameter (MonoObject *obj, SEL sel, id self, id arg, unsigned long index, MonoClass *expected, MonoMethod *method)
{
//	if (arg == NULL)
//		return;
//
//	if (obj != NULL) {
//		verify_cast (expected, obj, [arg class], sel, method);
//		return;
//	}
//	
//	const char *m = "Failed to marshal the Objective-C object 0x%x (type: %s). "
//	"Could not find an existing managed instance for this object, "
//	"nor was it possible to create a new managed instance "
//	"(because the type '%s' does not have a constructor that takes one IntPtr argument).\n"
//	"Additional information:\n"
//	"\tSelector: %s\n"
//	"\tMethod: %s\n"
//	"\tParameter: %i\n";
//	
//	char *method_full_name = mono_method_full_name (method, TRUE);
//	char *type_name = xamarin_lookup_managed_type_name ([arg class]);
//	char *msg = xamarin_strdup_printf (m, arg, object_getClassName (arg), type_name, sel, method_full_name, index);
//	MonoException *mex = xamarin_create_exception (msg);
//	xamarin_free (msg);
//	mono_free (method_full_name);
//	mono_free (type_name);
//	mono_raise_exception (mex);
}

void
xamarin_check_objc_type (id obj, Class expected_class, SEL sel, id self, int index, MonoMethod *method)
{
//	if ([obj isKindOfClass:expected_class])
//		return;
//	
//	const char *m = "Failed to marshal the Objective-C object 0x%x (type: %s), expected an object of type %s.\n"
//	"Additional information:\n"
//	"\tSelector: %s\n"
//	"\tMethod: %s\n"
//	"\tParameter: %i\n";
//
//	char *method_full_name = mono_method_full_name (method, TRUE);
//	char *msg = xamarin_strdup_printf (m, obj, object_getClassName (obj), class_getName (expected_class), sel, method_full_name, index);
//	MonoException *mono_ex = xamarin_create_system_invalid_cast_exception (msg);
//	xamarin_free (msg);
//	mono_free (method_full_name);
//	mono_raise_exception (mono_ex);
}
#endif

char *
xamarin_class_get_full_name (MonoClass *klass, GCHandle *exception_gchandle)
{
	MonoType *type = mono_class_get_type (klass);
	char * rv = xamarin_type_get_full_name (type, exception_gchandle);
	xamarin_mono_object_release (&type);

	return rv;
}

char *
xamarin_type_get_full_name (MonoType *type, GCHandle *exception_gchandle)
{
	MonoReflectionType *rtype = mono_type_get_object (mono_domain_get (), type);
	char *rv = xamarin_reflection_type_get_full_name (rtype, exception_gchandle);
	xamarin_mono_object_release (&rtype);
	return rv;
}

/*
 * ToggleRef support
 */
// #define DEBUG_TOGGLEREF 1

MonoToggleRefStatus
xamarin_gc_toggleref_callback (uint32_t flags, id handle, xamarin_get_handle_func get_handle, MonoObject *info)
{
	MonoToggleRefStatus res;

	bool disposed = (flags & NSObjectFlagsDisposed) == NSObjectFlagsDisposed;
	bool has_managed_ref = (flags & NSObjectFlagsHasManagedRef) == NSObjectFlagsHasManagedRef;

	if (disposed || !has_managed_ref) {
		res = MONO_TOGGLE_REF_DROP; /* Already disposed, we don't need the managed object around */
	} else {
		if (handle == NULL)
			handle = get_handle (info);

		if (handle == NULL) { /* This shouldn't really happen */
			res = MONO_TOGGLE_REF_DROP;
		} else {
			if ([handle retainCount] == 1)
				res = MONO_TOGGLE_REF_WEAK;
			else
				res = MONO_TOGGLE_REF_STRONG;
		}
	}

#ifdef DEBUG_TOGGLEREF
	const char *rv;
	if (res == MONO_TOGGLE_REF_DROP) {
		rv = "DROP";
	} else if (res == MONO_TOGGLE_REF_STRONG) {
		rv = "STRONG";
	} else if (res == MONO_TOGGLE_REF_WEAK) {
		rv = "WEAK";
	} else {
		rv = "UNKNOWN";
	}
	const char *cn = NULL;
	if (handle == NULL)
		handle = get_handle (info);
	cn = object_getClassName (handle);
	PRINT ("\tinspecting %p handle:%p %s flags: %i RC %d -> %s\n", object, handle, cn, (int) flags, (int) (handle ? [handle retainCount] : 0), rv);
#endif

	return res;
}

void
xamarin_gc_event (MonoGCEvent event)
{
	switch (event) {
	case MONO_GC_EVENT_PRE_STOP_WORLD:
		pthread_mutex_lock (&framework_peer_release_lock);
		break;

	case MONO_GC_EVENT_POST_START_WORLD:
		pthread_mutex_unlock (&framework_peer_release_lock);
		break;
	
	default: // silences a compiler warning.
		break;
	}
}

#if !defined (CORECLR_RUNTIME)
struct _MonoProfiler {
	int dummy;
};

static void
xamarin_install_mono_profiler ()
{
	static _MonoProfiler profiler = { 0 };
	// This must be done before any other mono_profiler_install_* functions are called
	// (currently xamarin_enable_new_refcount and xamarin_install_nsautoreleasepool_hooks).
	mono_profiler_install (&profiler, NULL);
}
#endif

bool
xamarin_file_exists (const char *path)
{
	struct stat buffer;
	return stat (path, &buffer) == 0;
}

static MonoAssembly *
xamarin_open_assembly_or_assert (const char *name)
{
	MonoImageOpenStatus status = MONO_IMAGE_OK;
	MonoAssembly *assembly = mono_assembly_open (name, &status);
	if (assembly == NULL)
		xamarin_assertion_message ("Failed to open the assembly '%s' from the app: %i (errno: %i). This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: https://github.com/dotnet/macios/issues/new", name, (int) status, errno);
	return assembly;
}

// Returns a retained MonoObject. Caller must release.
MonoAssembly *
xamarin_open_assembly (const char *name)
{
	char path [1024];
	bool exists = false;

#if MONOMAC
	if (xamarin_get_is_mkbundle ())
		return xamarin_open_assembly_or_assert (name);
#endif

	exists = xamarin_locate_assembly_resource (name, NULL, name, path, sizeof (path));

#if MONOMAC && DYLIB
	if (!exists) {
		// Check if we already have the assembly in memory
		xamarin_get_assembly_name_without_extension (name, path, sizeof (path));
		MonoAssemblyName *aname = mono_assembly_name_new (path);
		MonoAssembly *assembly = mono_assembly_loaded (aname);
		mono_assembly_name_free (aname);
		if (assembly)
			return assembly;

		xamarin_assertion_message ("Could not find the assembly '%s' in the app nor as an already loaded assembly. This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: https://github.com/dotnet/macios/issues/new", name);
	}
#endif

	if (!exists)
		xamarin_assertion_message ("Could not find the assembly '%s' in the app. This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: https://github.com/dotnet/macios/issues/new", name);

	return xamarin_open_assembly_or_assert (path);
}

bool
xamarin_register_monoassembly (MonoAssembly *assembly, GCHandle *exception_gchandle)
{
#if SUPPORTS_DYNAMIC_REGISTRATION
	if (!xamarin_supports_dynamic_registration) {
#endif
#if defined (CORECLR_RUNTIME)
		if (xamarin_log_level > 0) {
			MonoReflectionAssembly *rassembly = mono_assembly_get_object (mono_domain_get (), assembly);
			GCHandle assembly_gchandle = xamarin_gchandle_new ((MonoObject *) rassembly, false);
			xamarin_mono_object_release (&rassembly);

			char *assembly_name = xamarin_bridge_get_assembly_name (assembly_gchandle);
			xamarin_gchandle_free (assembly_gchandle);

			LOG (PRODUCT ": Skipping assembly registration for %s since it's not needed (dynamic registration is not supported)", assembly_name);
			mono_free (assembly_name);
		}
#else
		LOG (PRODUCT ": Skipping assembly registration for %s since it's not needed (dynamic registration is not supported)", mono_assembly_name_get_name (mono_assembly_get_name (assembly)));
#endif
		return true;
#if SUPPORTS_DYNAMIC_REGISTRATION
	}
#endif

#if SUPPORTS_DYNAMIC_REGISTRATION
	MonoReflectionAssembly *rassembly = mono_assembly_get_object (mono_domain_get (), assembly);
	xamarin_register_assembly (rassembly, exception_gchandle);
	xamarin_mono_object_release (&rassembly);
	return *exception_gchandle == INVALID_GCHANDLE;
#endif // SUPPORTS_DYNAMIC_REGISTRATION
}

// Returns a retained MonoObject. Caller must release.
MonoAssembly *
xamarin_open_and_register (const char *aname, GCHandle *exception_gchandle)
{
	MonoAssembly *assembly;

	assembly = xamarin_open_assembly (aname);

	xamarin_register_monoassembly (assembly, exception_gchandle);
	
	return assembly;
}

#if !defined (CORECLR_RUNTIME)
static gboolean 
is_class_finalization_aware (MonoClass *cls)
{
	gboolean rv = false;

	MonoClass *nsobject_class = xamarin_get_nsobject_class (true);
	if (nsobject_class)
		rv = cls == nsobject_class || mono_class_is_assignable_from (nsobject_class, cls);

	//PRINT ("IsClass %s.%s finalization aware: %i\n", mono_class_get_namespace (cls), mono_class_get_name (cls), rv);

	return rv;
}

static void 
object_queued_for_finalization (MonoObject *object)
{
	/* This is called with the GC lock held, so it can only use signal-safe code */
	struct Managed_NSObject *obj = (struct Managed_NSObject *) object;
#if defined(DEBUG_REF_COUNTING)
	// Don't use PRINT/NSLog/os_log here, because the process can end up aborting due to recursive locks inside the ObjC runtime. Luckily fprintf works fine.
	fprintf (stderr, "object_queued_for_finalization (%p): data=%p flags=%x handle=%p\n", object, obj->data, (unsigned int) (obj->data ? obj->data->flags : 0), obj->data ? obj->data->handle : NULL);
#endif
	if (obj->data)
		obj->data->flags |= NSObjectFlagsInFinalizerQueue;
}
#endif // !defined (CORECLR_RUNTIME)

/*
 * Registration map
 */ 

static int
compare_mtclassmap (const void *a, const void *b)
{
	MTClassMap *mapa = (MTClassMap *) a;
	MTClassMap *mapb = (MTClassMap *) b;

	intptr_t diff = (intptr_t)mapa->handle - (intptr_t)mapb->handle;
	const int shift = (sizeof(intptr_t) * 8) - 1;

	return (diff >> shift) | !!diff;
}

void
xamarin_add_registration_map (struct MTRegistrationMap *map, bool partial)
{
	if (strcmp (map->product_hash, PRODUCT_HASH) != 0) {
		fprintf (stderr, PRODUCT ": The static registrar map for %s (and %i other assemblies) is invalid. It was built using a runtime with hash %s, but the current runtime was built with hash %s.\n", map->assemblies [0].name, map->assembly_count - 1, map->product_hash, PRODUCT_HASH);
		return;
	}

	options.RegistrationData = map;
	if (partial)
		options.flags = (InitializationFlags) (options.flags | InitializationFlagsIsPartialStaticRegistrar);

	// Sort the type map according to Class
	qsort (map->map, (size_t) map->map_count, sizeof (MTClassMap), compare_mtclassmap);
}

/*
 * Exception handling
 */

NSString *
xamarin_print_all_exceptions (GCHandle gchandle)
{
	GCHandle exception_gchandle = INVALID_GCHANDLE;

	char *msg = xamarin_print_all_exceptions_wrapper (gchandle, &exception_gchandle);
	if (exception_gchandle != INVALID_GCHANDLE) {
		// Not much we can do here but returning a very generic message, since we failed to print one exception, it's reasonable to assume that printing another won't work either.
		xamarin_gchandle_free (exception_gchandle);
		exception_gchandle = INVALID_GCHANDLE;
		return [NSString stringWithFormat: @"An exception occurred while trying to get a string representation for the exception %p (%p)", gchandle, exception_gchandle];
	}

	NSString *rv = [NSString stringWithUTF8String: msg];
	xamarin_free (msg);
	return rv;
}

void
xamarin_ftnptr_exception_handler (GCHandle gchandle)
{
	xamarin_process_managed_exception_gchandle (gchandle);
}

void
xamarin_process_fatal_exception_gchandle (GCHandle gchandle, const char *message)
{
	if (gchandle == INVALID_GCHANDLE)
		return;

	NSString *fatal_message = [NSString stringWithFormat:@"%s\n%@", message, xamarin_print_all_exceptions (gchandle)];
	NSLog (@PRODUCT ": %@", fatal_message);
	xamarin_assertion_message ("%s", [fatal_message UTF8String]);
}

// Because this function won't always return, it will take ownership of the GCHandle and free it.
void
xamarin_process_managed_exception_gchandle (GCHandle gchandle)
{
	if (gchandle == INVALID_GCHANDLE)
		return;

	MonoObject *exc = xamarin_gchandle_get_target (gchandle);
	xamarin_gchandle_free (gchandle);
	xamarin_process_managed_exception (exc);	
}

void
xamarin_unhandled_exception_handler (MonoObject *exc, gpointer user_data)
{
	GCHandle exception_gchandle = xamarin_gchandle_new (exc, false);
	PRINT ("Unhandled managed exception: %@", xamarin_print_all_exceptions (exception_gchandle));
	xamarin_gchandle_free (exception_gchandle);

	abort ();
}

extern "C" {
	static thread_local int xamarin_handling_unhandled_exceptions = 0;
}

static void
exception_handler (NSException *exc)
{
	LOG (PRODUCT ": Received unhandled ObjectiveC exception: %@ %@", [exc name], [exc reason]);

	XamarinGCHandle* exc_handle = [[exc userInfo] objectForKey: @"XamarinManagedExceptionHandle"];
	if (exc_handle != NULL) {
		GCHandle exception_gchandle = [exc_handle getHandle];
		if (exception_gchandle != INVALID_GCHANDLE) {
			xamarin_bridge_raise_unhandled_exception_event (exception_gchandle);
			PRINT ("Received unhandled Objective-C exception that was marshalled from a managed exception: %@", exc);
			abort ();
		}
	}

	if (xamarin_handling_unhandled_exceptions == 1) {
		PRINT ("Detected recursion when handling uncaught Objective-C exception: %@", exc);
		abort ();
	}

	xamarin_handling_unhandled_exceptions = 1;
	xamarin_throw_ns_exception (exc);
	xamarin_handling_unhandled_exceptions = 0;
}

#if !defined (CORECLR_RUNTIME)
static void
log_callback (const char *log_domain, const char *log_level, const char *message, mono_bool fatal, void *user_data)
{
	PRINT ("%s: %s", log_level, message);

	if (fatal)
		abort ();
}

static void
print_callback (const char *string, mono_bool is_stdout)
{
	PRINT ("%s", string);
}
#endif // !defined (CORECLR_RUNTIME)

static int
xamarin_compare_ints (const void *a, const void *b)
{
	uint32_t x = *(uint32_t *) a;
	uint32_t y = *(uint32_t *) b;
	return x < y ? -1 : (x == y ? 0 : 1);
}

uint32_t
xamarin_find_protocol_wrapper_type (uint32_t token_ref)
{
	if (options.RegistrationData == NULL || options.RegistrationData->protocol_wrappers == NULL)
		return INVALID_TOKEN_REF;

	void* ptr = bsearch (&token_ref, options.RegistrationData->protocol_wrappers, (size_t) options.RegistrationData->protocol_wrapper_count, sizeof (MTProtocolWrapperMap), xamarin_compare_ints);
	if (ptr == NULL)
		return INVALID_TOKEN_REF;

	MTProtocolWrapperMap *entry = (MTProtocolWrapperMap *) ptr;
	return entry->wrapper_token;
}

/* Installs g_print/g_error handlers that will redirect output to the system Console */
void
xamarin_install_log_callbacks ()
{
#if !defined (CORECLR_RUNTIME)
	mono_trace_set_log_handler (log_callback, NULL);
	mono_trace_set_print_handler (print_callback);
	mono_trace_set_printerr_handler (print_callback);
#endif
}

void
xamarin_initialize_dynamic_registrar ()
{
	options.Trampolines = &trampolines;
}

void
xamarin_initialize ()
{
	GCHandle exception_gchandle = INVALID_GCHANDLE;

	initialize_started = TRUE;

#ifdef DYNAMIC_MONO_RUNTIME
	// We might be called from the managed Runtime.EnsureInitialized method,
	// in which case xamarin_initialize_dynamic_runtime has not been called yet.
	xamarin_initialize_dynamic_runtime (NULL);
#endif

	xamarin_install_log_callbacks ();

#if !defined (CORECLR_RUNTIME)
	MonoGCFinalizerCallbacks gc_callbacks;
	gc_callbacks.version = MONO_GC_FINALIZER_EXTENSION_VERSION;
	gc_callbacks.is_class_finalization_aware = is_class_finalization_aware;
	gc_callbacks.object_queued_for_finalization = object_queued_for_finalization;
	mono_gc_register_finalizer_callbacks (&gc_callbacks);
#endif

	NSSetUncaughtExceptionHandler (exception_handler);

	options.size = sizeof (options);
#if TARGET_OS_SIMULATOR
	options.flags = (enum InitializationFlags) (options.flags | InitializationFlagsIsSimulator);
#endif

#if defined (CORECLR_RUNTIME)
	options.flags = (enum InitializationFlags) (options.flags | InitializationFlagsIsCoreCLR);
#endif
#if defined (NATIVEAOT)
	options.flags = (enum InitializationFlags) (options.flags | InitializationFlagsIsNativeAOT);
#endif

	options.Delegates = &delegates;
	options.MarshalObjectiveCExceptionMode = xamarin_marshal_objectivec_exception_mode;
	options.MarshalManagedExceptionMode = xamarin_marshal_managed_exception_mode;

#if defined (CORECLR_RUNTIME)
	options.xamarin_objc_msgsend = (void *) xamarin_dyn_objc_msgSend;
	options.xamarin_objc_msgsend_super = (void *) xamarin_dyn_objc_msgSendSuper;
#if !defined(__aarch64__)
	options.xamarin_objc_msgsend_stret = (void *) xamarin_dyn_objc_msgSend_stret;
	options.xamarin_objc_msgsend_super_stret = (void *) xamarin_dyn_objc_msgSendSuper_stret;
#endif // !defined(__aarch64__)
	options.unhandled_exception_handler = (void *) &xamarin_coreclr_unhandled_exception_handler;
	options.reference_tracking_begin_end_callback = (void *) &xamarin_coreclr_reference_tracking_begin_end_callback;
	options.reference_tracking_is_referenced_callback = (void *) &xamarin_coreclr_reference_tracking_is_referenced_callback;
	options.reference_tracking_tracked_object_entered_finalization = (void *) &xamarin_coreclr_reference_tracking_tracked_object_entered_finalization;
#endif // defined(CORECLR_RUNTIME)

	xamarin_bridge_call_runtime_initialize (&options, &exception_gchandle);
	xamarin_process_fatal_exception_gchandle (exception_gchandle, "An exception occurred while calling Runtime.Initialize");

	xamarin_bridge_register_product_assembly (&exception_gchandle);
	xamarin_process_fatal_exception_gchandle (exception_gchandle, "An exception occurred while registering the product assembly");

#if !defined (CORECLR_RUNTIME)
	xamarin_install_mono_profiler (); // must be called before xamarin_install_nsautoreleasepool_hooks or xamarin_enable_new_refcount
#endif

	xamarin_install_nsautoreleasepool_hooks ();

	pthread_mutexattr_t attr;
	pthread_mutexattr_init (&attr);
	pthread_mutexattr_settype (&attr, PTHREAD_MUTEX_RECURSIVE);
	pthread_mutex_init (&framework_peer_release_lock, &attr);
	pthread_mutexattr_destroy (&attr);

	xamarin_enable_new_refcount ();
}

static char *x_app_bundle_path = NULL;
const char *
xamarin_get_app_bundle_path ()
{
	if (x_app_bundle_path != NULL)
		return x_app_bundle_path;

	NSBundle *main_bundle = [NSBundle mainBundle];

	if (main_bundle == NULL)
		xamarin_assertion_message ("Could not find the main bundle in the app ([NSBundle mainBundle] returned nil)");

	x_app_bundle_path = strdup ([[[main_bundle bundlePath] stringByStandardizingPath] UTF8String]);

	return x_app_bundle_path;
}

static char *x_bundle_path = NULL;
const char *
xamarin_get_bundle_path ()
{
	if (x_bundle_path != NULL)
		return x_bundle_path;

	NSBundle *main_bundle = [NSBundle mainBundle];
	NSString *bundle_path;

	if (main_bundle == NULL)
		xamarin_assertion_message ("Could not find the main bundle in the app ([NSBundle mainBundle] returned nil)");

#if TARGET_OS_MACCATALYST || TARGET_OS_OSX
	bundle_path = [[main_bundle bundlePath] stringByAppendingPathComponent:@"Contents"];
	bundle_path = [bundle_path stringByAppendingPathComponent: xamarin_custom_bundle_name];
#else
	bundle_path = [main_bundle bundlePath];
#endif

	x_bundle_path = strdup ([[bundle_path stringByStandardizingPath] UTF8String]);

	return x_bundle_path;
}

void
xamarin_set_bundle_path (const char *path)
{
	free (x_bundle_path);
	x_bundle_path = strdup (path);
}

void *
xamarin_calloc (size_t size)
{
	return calloc (size, 1);
}

void
xamarin_free (void *ptr)
{
	// We use this method to free memory returned by mono,
	// which means we have to use the free function mono expects.
	if (ptr)
		free (ptr);
}

char *
xamarin_strdup_printf (const char *msg, ...)
{
	va_list args;
	char *formatted = NULL;

	va_start (args, msg);

	vasprintf (&formatted, msg, args);

	va_end (args);

	return formatted;
}

void
xamarin_assertion_message (const char *msg, ...)
{
	va_list args;
	char *formatted = NULL;

	va_start (args, msg);

	vasprintf (&formatted, msg, args);

	if (formatted) {
		PRINT ( PRODUCT ": %s", formatted);
		free (formatted);
	}
	va_end (args);
	abort ();
}

static const char *
objc_skip_type (const char *type)
{
	switch (type [0]) {
		case _C_ID:
			type++;
			if (*type == '"') {
				// https://github.com/dotnet/macios/issues/18562
				// @"..." is an object with the class name inside the quotes.
				// https://github.com/llvm/llvm-project/blob/24a082878f7baec3651de56d54e5aa2b75a21b5f/clang/lib/AST/ASTContext.cpp#L8505-L8516
				type++;
				while (*type && *type != '"')
					type++;
				type++;
			} else if (*type == '?' && type [1] == '<') {
				// https://github.com/dotnet/macios/issues/18562
				// @?<...> is a block pointer
				// https://github.com/llvm/llvm-project/blob/24a082878f7baec3651de56d54e5aa2b75a21b5f/clang/lib/AST/ASTContext.cpp#L8405-L8426
				type += 2;
				do {
					type = objc_skip_type (type);
				} while (*type && *type != '>');
				if (*type)
					type++;
			}
			return type;
		case _C_CLASS:
		case _C_SEL:
		case _C_CHR:
		case _C_UCHR:
		case _C_SHT:
		case _C_USHT:
		case _C_INT:
		case _C_UINT:
		case _C_LNG:
		case _C_ULNG:
		case _C_LNG_LNG:
		case _C_ULNG_LNG:
		case _C_FLT:
		case _C_DBL:
		case _C_CHARPTR:
		case _C_BOOL:
		case _C_VOID:
		case _C_UNDEF:
			return ++type;
		case _C_PTR:
			return objc_skip_type (++type);
		case _C_BFLD:
			type++;
			while (*type && *type >= '0' && *type <= '9')
				type++;
			return type;
		case _C_ATOM:
		case _C_VECTOR:
		case _C_CONST:
		case _C_ARY_E:
		case _C_UNION_E:
		case _C_STRUCT_E:
			xamarin_assertion_message ("Unhandled type encoding: %s", type);
			break;
		case _C_ARY_B: {
			do {
				type++;
			} while (isdigit (*type));

			type = objc_skip_type (type);
			
			return ++type;
		}
		case _C_UNION_B: {
			do {
				type++;
			} while (*type != '=');

			type ++;
			do {
				type = objc_skip_type (type);
			} while (*type != _C_UNION_E);
			
			return ++type;
		}
		case _C_STRUCT_B: {
			do {
				type++;
			} while (*type != '=');

			type++;

			do {
				type = objc_skip_type (type);
			} while (*type != _C_STRUCT_E);

			return ++type;
		}
		default:
			xamarin_assertion_message ("Unsupported type encoding: %s", type);
			break;
	}
}

unsigned long
xamarin_objc_type_size (const char *type)
{
	const char *original_type = type;

	switch (type [0]) {
		case _C_ID: return sizeof (id);
		case _C_CLASS: return sizeof (Class);
		case _C_SEL: return sizeof (SEL);
		case _C_CHR: return sizeof (char);
		case _C_UCHR: return sizeof (unsigned char);
		case _C_SHT: return sizeof (short);
		case _C_USHT: return sizeof (unsigned short);
		case _C_INT: return sizeof (int);
		case _C_UINT: return sizeof (unsigned int);
		case _C_LNG: return sizeof (long);
		case _C_ULNG: return sizeof (unsigned long);
		case _C_LNG_LNG: return sizeof (long long);
		case _C_ULNG_LNG: return sizeof (unsigned long long);
		case _C_FLT: return sizeof (float);
		case _C_DBL: return sizeof (double);
		case _C_BOOL: return sizeof (BOOL);
		case _C_VOID: return 0;
		case _C_PTR: return sizeof (void *);
		case _C_CHARPTR: return sizeof (char *);
		case _C_BFLD: {
			// Example: [NSDecimalNumberPlaceholder initWithDecimal:] = @28@0:4{?=b8b4b1b1b18[8S]}8
			unsigned long bits = 0;
			int bc = 1;
			while (type [bc] >= '0' && type [bc] <= '9') {
				bits = bits * 10ul + (unsigned long) (type [bc] - '0');
				bc++;
			}
			if (bits % sizeof (void *) == 0)
				return bits / sizeof (void *);
			return 1 + (bits / sizeof (void *));
		}
		case _C_UNDEF:
		case _C_ATOM:
		case _C_VECTOR:
			xamarin_assertion_message ("Unhandled type encoding: %s", type);
			break;
		case _C_ARY_B: {
			unsigned long size = 0;
			unsigned long len = (unsigned long) atol (type+1);
			do {
				type++;
			} while (isdigit (*type));

			size = xamarin_objc_type_size (type);

			size = (size + (sizeof (void *) - 1)) & ~((sizeof (void *) - 1));

			return len * size;
		}
		case _C_UNION_B: {
			unsigned long size = 0;

			do {
				type++;

				if (*type == 0)
					xamarin_assertion_message ("Unsupported union type: %s", original_type);
			} while (*type != '=');

			++type;

			do {
				if (*type == 0)
					xamarin_assertion_message ("Unsupported union type: %s", original_type);

				unsigned long tsize = xamarin_objc_type_size (type);
				type = objc_skip_type (type);

				tsize = (tsize + (sizeof (void *) - 1)) & ~((sizeof (void *) - 1));
				size = size > tsize ? size : tsize;
			} while (*type != _C_UNION_E);

			return size;
		}
		case _C_STRUCT_B: {
			unsigned long size = 0;

			do {
				type++;

				if (*type == 0)
					xamarin_assertion_message ("Unsupported struct type: %s", original_type);
			} while (*type != '=');

			type++;

			while (*type != _C_STRUCT_E) {
				if (*type == 0)
					xamarin_assertion_message ("Unsupported struct type: %s", original_type);
				unsigned long item_size = xamarin_objc_type_size (type);
				
				// first align the accumulated size to the current item's size (this would be any padding between struct elements)
				size = align_ulong (size, item_size);
				// accumulate this item's size
				size += item_size;

				type = objc_skip_type (type);
			}

			return size;
		}
		// The following are from table 6-2 here: https://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/ObjCRuntimeGuide/Articles/ocrtTypeEncodings.html
		case 'r': // _C_CONST
		case 'n':
		case 'N':
		case 'o':
		case 'O':
		case 'R':
		case 'V':
			return xamarin_objc_type_size (type + 1);
	}
	
	xamarin_assertion_message ("Unsupported type encoding: %s", original_type);
}

/*
 * Reference counting
 * ==================
 * 
 * There are two types of managed types:
 *  - Wrapper types for existing ObjectiveC types (such as UIView).
 *  - Other types derived from wrapper types (henceforth called User types), 
 *    that do not have a corresponding ObjectiveC type.
 *
 * Wrapper types
 * -------------
 * 
 * They are simple. The managed peer's lifetime is not linked to the native object's lifetime. 
 * The managed peer can be freed by the GC whenever it so determines, and if a managed object 
 * is needed at a later stage we just recreate it.
 * 
 * User types
 * ----------
 * These are not that simple. We can't free managed objects at will, since they may contain
 * user state. Therefore we must ensure the managed object stays alive as long as needed 
 * (this is done by using a strong GCHandle) - the problem is determining when it's no longer
 * needed.
 * 
 * Historically we've had two cases:
 * 1) User calls Dispose, we free the managed ref and break the link between the native and
 *    managed object, thus allowing the GC to free it (if nothing else is keeping it alive
 *    of course).
 * 2) Refcount reaches 1, in which case we know that the managed ref is the only ref and we
 *    can safely assume that native code will not use the object again. We break the link,
 *    release the native object (which will be freed now) and allow the GC to free the
 *    managed object.
 * 
 * Problem arises in case 1), when users call Dispose and then native code tries to use
 * that object for whatever reason. MonoTouch will detect that no managed peer exists,
 * and try to (re)create one. This may fail, and an exception is thrown which may kill
 * the process (there may be no managed frame / exception handler on the stack at this point).
 *
 * This solution will do a couple of things when user calls Dispose:
 * - Free the managed ref.
 * - Not break the link between the native and managed object until the native object's
 *   refcount reaches 0.
 * This will allow us to still lookup the managed object as long as the native object is
 * alive.
 * 
 * Implementation details (for user types only)
 * ============================================
 * 
 * Managed code releases its ref when either of the following conditions occur:
 *  - Dispose is called (manually) on the object.
 *  - The Handle property on the managed object changes.
 *  - The GC frees the object. This can only happen if refCount == 1 and that ref is a managed ref 
 *    (since otherwise there will be a strong gchandle to the managed object preventing the GC from freeing it).
 * 
 * Objects are removed from the native<->managed dictionary when either of the following conditions occur:
 *  - The native object is dealloc'ed (refcount reaches 0).
 *  - The managed object's Handle property changes (the link between the previous Handle 
 *    value and the managed object is then removed from the dictionary).
 *
 * We need to keep track of two pieces of information in native land:
 *  - The GCHandle to the managed object.
 *  - If there is a managed ref or not.
 * We already have an ObjectiveC ivar to store the GCHandle, so to not create another ivar
 * we use one bit of the GCHandle to store whether there is a managed ref or not (MANAGED_REF_BIT).
 * 
 */
//#define DEBUG_REF_COUNTING

// Free the strong gchandle for a given native object.
static void
free_strong_gchandle (id self)
{
	GCHandle strong_gchandle = INVALID_GCHANDLE;
	pthread_mutex_lock (&strong_gchandle_hash_lock);
	if (strong_gchandle_hash != NULL) {
		const void *value;
		if (CFDictionaryGetValueIfPresent (strong_gchandle_hash, self, &value)) {
			strong_gchandle = (GCHandle) value;
			CFDictionaryRemoveValue (strong_gchandle_hash, self);
		}
	}
	pthread_mutex_unlock (&strong_gchandle_hash_lock);
	if (strong_gchandle != INVALID_GCHANDLE) {
#if defined(DEBUG_REF_COUNTING)
		PRINT ("Cleared strong gchandle %d for %p\n", strong_gchandle, self);
#endif
		xamarin_gchandle_free (strong_gchandle);
	} else {
#if defined(DEBUG_REF_COUNTING)
		PRINT ("Did not clear a strong gchandle for %p, because none was found\n", self);
#endif
	}
}

// Creates a strong gchandle for the given managed object and associates it
// with the native object. If a strong gchandle already exists for this
// native object, the newly created one is freed and the existing one is kept.
// Returns true if a new strong gchandle was set, false if one already existed.
static bool
create_strong_gchandle (id self, MonoObject *managed_object)
{
	GCHandle new_gchandle = xamarin_gchandle_new (managed_object, FALSE);

	GCHandle existing = INVALID_GCHANDLE;
	pthread_mutex_lock (&strong_gchandle_hash_lock);
	if (strong_gchandle_hash == NULL)
		strong_gchandle_hash = CFDictionaryCreateMutable (kCFAllocatorDefault, 0, NULL, NULL);
	const void *value;
	if (CFDictionaryGetValueIfPresent (strong_gchandle_hash, self, &value))
		existing = (GCHandle) value;
	if (existing == INVALID_GCHANDLE)
		CFDictionarySetValue (strong_gchandle_hash, self, (const void *) new_gchandle);
	pthread_mutex_unlock (&strong_gchandle_hash_lock);

	if (existing != INVALID_GCHANDLE) {
		// Another thread already set a strong gchandle, free ours.
		xamarin_gchandle_free (new_gchandle);
		return false;
	}
	return true;
}

void
xamarin_switch_gchandle (id self, bool to_weak)
{
	GCHandle old_gchandle;
	MonoObject *managed_object = NULL;

	old_gchandle = get_gchandle_without_flags (self);
	if (!old_gchandle) {
		// We don't have a GCHandle. This means there's no managed instance for this
		// native object.
		// If to_weak is true, then there's obviously nothing to do
		// (why create a managed object which can immediately be freed by the GC?).
		// If to_weak is false, the question is if we want to create the
		// managed object. Bug #30420 says no (previously we didn't, and
		// if we do, managed ctors end up being executed at a different moment,
		// which breaks implicit assumptions in people's code.)
#if defined(DEBUG_REF_COUNTING)
		PRINT ("Object %p has no managed object to create a %s GCHandle for\n", self, to_weak ? "weak" : "strong");
#endif
		return;
	}

	// The object's gc_handle is always a weak handle used for lookups. It is
	// never modified here. A separate strong gchandle (stored in a global hash
	// table) is created/freed as needed to keep the managed object alive.
	// Since gc_handle is never modified, there is no race with concurrent
	// readers (fixing https://github.com/dotnet/macios/issues/24702).

	if (to_weak) {
		free_strong_gchandle (self);
	} else {
		MONO_THREAD_ATTACH;

		managed_object = xamarin_gchandle_get_target (old_gchandle);
		if (managed_object) {
			if (create_strong_gchandle (self, managed_object)) {
				xamarin_set_nsobject_flags (managed_object, xamarin_get_nsobject_flags (managed_object) | NSObjectFlagsHasManagedRef);
			}
		}

		MONO_THREAD_DETACH;
		xamarin_mono_object_release (&managed_object);

#if defined(DEBUG_REF_COUNTING)
		PRINT ("Object %p switched to strong mode\n", self);
#endif
	}
}

void
xamarin_free_gchandle (id self, GCHandle gchandle)
{
	if (gchandle) {
#if defined(DEBUG_REF_COUNTING)
		PRINT ("\tGCHandle %i destroyed for object %p\n", gchandle, self);
#endif
		xamarin_gchandle_free (gchandle);

		set_gchandle (self, INVALID_GCHANDLE, XamarinGCHandleFlags_None, NULL);
	} else {
#if defined(DEBUG_REF_COUNTING)
		PRINT ("\tNo GCHandle for the object %p\n", self);
#endif
	}

	// Also free any strong gchandle from the dual-gchandle scheme.
	free_strong_gchandle (self);
}

void
xamarin_clear_gchandle (id self)
{
	set_gchandle (self, INVALID_GCHANDLE, XamarinGCHandleFlags_None, NULL);
}

bool
xamarin_set_gchandle_with_flags (id self, GCHandle gchandle, enum XamarinGCHandleFlags flags)
{
	return set_gchandle (self, gchandle, flags, NULL);
}

bool
xamarin_set_gchandle_with_flags_safe (id self, GCHandle gchandle, enum XamarinGCHandleFlags flags, struct NSObjectData *data)
{
	return set_gchandle_safe (self, gchandle, flags, data);
}

#if defined(DEBUG_REF_COUNTING)
int
get_safe_retainCount (id self)
{
	if ([self isKindOfClass: [NSCalendar class]] ||
		[self isKindOfClass: [NSInputStream class]] ||
		[self isKindOfClass: [NSOutputStream class]]) {
		// NSCalendar/NSInputStream may end up with a stack overflow where CFGetRetainCount calls itself
		return 666;
	} else {
		return (int) [self retainCount];
	}
}
#endif

// It's fairly frequent (due to various types of coding errors) to have the
// call to '[self release]' in xamarin_release_managed_ref crash. These
// crashes are typically very hard to diagnose, because it can be hard to
// figure out which object caused the crash. So here we store the native
// object in a static variable, so that it can be read using lldb from a core
// dump. The variable is declared as volatile so that the compiler doesn't
// optimize away anything (we want both writes in
// xamarin_release_managed_ref), and it's attributed with 'unused' because
// otherwise the compiler complains that the variable is never read.
//
// Admittedly the variable should also be thread-local, but that's not
// supported on all platforms we build for (in particular it requires min
// iOS 10), and it's also slower than a straight forward write to a static
// variable. Also while xamarin_release_managed_ref can be called on
// multiple threads, the vast majority of the calls occur on the main
// thread, so let's keep the variable global for now and if it turns out
// to be a problem we can make it thread-local later.
extern "C" {
	volatile id xamarin_handle_to_be_released __attribute__((unused));
}

void
xamarin_release_managed_ref (id self, bool user_type)
{
#if defined(DEBUG_REF_COUNTING)
	PRINT ("monotouch_release_managed_ref (%s Handle=%p) retainCount=%d; HasManagedRef=%i GCHandle=%p IsUserType=%i managed_obj=%p\n", 
		class_getName (object_getClass (self)), self, (int32_t) [self retainCount], user_type ? xamarin_has_managed_ref (self) : 666, user_type ? get_gchandle_without_flags (self) : (void*) 666, user_type, managed_obj);
#endif

	if (user_type) {
		/* clear MANAGED_REF_BIT */
		set_gchandle_flags_safe (self, (enum XamarinGCHandleFlags) (get_flags_safe (self) & ~XamarinGCHandleFlags_HasManagedRef));
	} else {
		//
		// This waypoint (lock+unlock) is needed so that we can safely call retainCount in the
		// toggleref callback.
		//
		// The race is between the following actions (given a managed object Z):
		//
		//   a1) Thread A nulls out the handle for Z
		//   a2) Thread A calls release on Z's original handle
		//   b1) Thread B fetches the handle for Z
		//   b2) Thread B calls retainCount on Z's handle
		//
		// Possible execution orders:
		//
		//   1) a1-*: all such orders are safe, because b1 will read NULL and
		//      b2 won't do anything
		//   2) b1-b2-a1-a2: retainCount before release is safe.
		//   3) b1-a1-b2-a2: retainCount before release is safe.
		//   4) b1-a1-a2-b2: unsafe; this tries to call retainCount after
		//      release.
		//
		// Order 4 would look like this:
		//
		//   * Thread B runs a GC, and starts calling toggleref callbacks.
		//   * Thread B fetches the handle (H) for object Z in a toggleref
		//     callback.
		//   * Thread A calls xamarin_release_managed_ref for object Z, and
		//     calls release on H, deallocating it.
		//   * Thread B tries to call retainCount on H, which is now a
		//     deallocated object.
		//
		// Solution: lock/unlock the framework peer lock here. This looks
		// weird (since nothing happens inside the lock), but it works:
		//
		//   * Thread B runs a GC, locks the framework peer lock, and starts
		//     calling toggleref callbacks.
		//   * Thread B fetches the handle (H) for object Z in a toggleref
		//     callback.
		//   * Thread A calls xamarin_release_managed_ref for object Z (and
		//     also nulls out the handle for Z)
		//   * Thread A tries to lock the framework peer lock, and blocks
		//     (before calling release on H)
		//   * Thread B successfully calls retainCount on H
		//   * Thread B finishes processing all toggleref callbacks, completes
		//     the GC, and unlocks the framework peer lock.
		//   * Thread A wakes up, and calls release on H.
		//
		// An alternative phrasing would be to say that the lock prevents both
		// a1 and a2 from happening between b1 and b2 from above, thus making
		// order 4 impossible.
		//
		// Q) Why not just unlock after calling release, to avoid the strange-
		//    looking empty lock?
		// A) Because calling release on an object might end up calling
		//    managed code (the native object can override dealloc and do all
		//    sorts of strange things, any of which may end up invoking
		//    managed code), and we can deadlock:
		//    1) Thread T calls release on a native object.
		//    2) Thread T executes managed code, which blocks on something
		//       that's supposed to happen on another thread U.
		//    3) Thread U causes a garbage collection.
		//    4) Thread U tries to lock the framework peer lock before running
		//       the GC, and deadlocks because thread T already has the
		//       framework peer lock.
		//
		//    This is https://github.com/dotnet/macios/issues/3943
		//
		// See also comment in xamarin_marshal_return_value_impl
		xamarin_framework_peer_waypoint_safe ();
	}

	xamarin_handle_to_be_released = self;

	objc_release (self);

	xamarin_handle_to_be_released = NULL;
}

/*
 * Block support
 */

typedef struct {
	MonoMethod *method;
	int par;
} MethodAndPar;

static gboolean
method_and_par_compare (gconstpointer l, gconstpointer r)
{
	MethodAndPar *f = (MethodAndPar *)l;
	MethodAndPar *g = (MethodAndPar *) r;

	return f->method == g->method && f->par == g->par;
}

static unsigned int
method_and_par_hash (gconstpointer l)
{
	MethodAndPar *x = (MethodAndPar *) l;

	return (unsigned int) (intptr_t) x->method;
}

static pthread_mutex_t wrapper_hash_lock = PTHREAD_MUTEX_INITIALIZER;
/*
 * Given a MonoMethod and a parameter, lookup the MethodInfo (MonoReflectionMethod)
 * that can be used to create a new delegate, this returns the method that can
 * create the method
 */
static GCHandle
get_method_block_wrapper_creator (MonoMethod *method, int par, GCHandle *exception_gchandle)
{
	GCHandle rv = INVALID_GCHANDLE;
	MonoObject *res = NULL;
	MethodAndPar mp, *nmp;
	mp.method = method;
	mp.par = par;

	// PRINT ("Looking up method and par (%x and %d)", (int) method, par);
	pthread_mutex_lock (&wrapper_hash_lock);
	
	if (xamarin_wrapper_hash == NULL) {
		xamarin_wrapper_hash = mono_g_hash_table_new_type (method_and_par_hash, method_and_par_compare, MONO_HASH_VALUE_GC);
	}

	res = (MonoObject *) mono_g_hash_table_lookup (xamarin_wrapper_hash, &mp);
	pthread_mutex_unlock (&wrapper_hash_lock);
	if (res != NULL){
		rv = xamarin_gchandle_new (res, false);
		xamarin_mono_object_release (&res);
		// PRINT ("Found match: %x", (int) res);
		return rv;
	}

	MonoReflectionMethod *reflection_method = mono_method_get_object (mono_domain_get (), method, NULL);
	res = xamarin_get_block_wrapper_creator (reflection_method, (int) par, exception_gchandle);
	xamarin_mono_object_release (&reflection_method);

	if (*exception_gchandle != INVALID_GCHANDLE)
		return INVALID_GCHANDLE;
	// PRINT ("New value: %x", (int) res);

	nmp = (MethodAndPar *) calloc (1, sizeof (MethodAndPar));
	*nmp = mp;

	pthread_mutex_lock (&wrapper_hash_lock);
	mono_g_hash_table_insert (xamarin_wrapper_hash, nmp, res);
	pthread_mutex_unlock (&wrapper_hash_lock);

	rv = xamarin_gchandle_new (res, false);
	xamarin_mono_object_release (&res);
	return rv;
}

void
xamarin_release_block_on_main_thread (void *obj)
{
	if ([NSThread isMainThread]) {
		_Block_release (obj);
	} else {
		dispatch_async_f (dispatch_get_main_queue (), obj, (dispatch_function_t) _Block_release);
	}
}

/*
 * Creates a System.Delegate to wrap an Objective-C proxy when surfacing parameters from Objective-C to C#.
 * @method: method where the parameter is found
 * @par: index of the parameter that is a delegate
 *
 * Given a method, and a parameter index that we have previously probed to be a Delegate,
 * this method returns a strongly typed System.Delegate that wraps the underlying
 * Objective-C block.
 *
 * This works by enlisting the help of the C# runtime to find a [BlockProxy] attrbute
 * on the parameter of the function, or in one of the base definitions.   That attribute
 * contains a link to a proxy type that can create the delegate, which we in turn invoke
 *
 * Returns: the instantiated delegate.
 */
MonoObject *
xamarin_get_delegate_for_block_parameter (MonoMethod *method, guint32 token_ref, int par, void *nativeBlock, GCHandle *exception_gchandle)
{
	MonoObject *delegate = NULL;
	GCHandle obj_handle = INVALID_GCHANDLE;

	if (nativeBlock == NULL)
		return NULL;

	if (token_ref != INVALID_TOKEN_REF) {
		obj_handle = xamarin_get_method_from_token (token_ref, exception_gchandle);
	} else {
		obj_handle = get_method_block_wrapper_creator (method, par, exception_gchandle);
	}
	if (*exception_gchandle != INVALID_GCHANDLE)
		goto cleanup;

	/* retain or copy (if it's a stack block) the block */
	nativeBlock = _Block_copy (nativeBlock);

	delegate = xamarin_create_block_proxy (obj_handle, nativeBlock, exception_gchandle);
	if (*exception_gchandle != INVALID_GCHANDLE) {
		_Block_release (nativeBlock);
		delegate = NULL;
		goto cleanup;
	}

cleanup:
	xamarin_gchandle_free (obj_handle);
	return delegate;
}

id
xamarin_get_block_for_delegate (MonoMethod *method, MonoObject *delegate, const char *signature, guint32 token_ref, GCHandle *exception_gchandle)
{
	MonoReflectionMethod *reflection_method = mono_method_get_object (mono_domain_get (), method, NULL);
	id rv = xamarin_create_delegate_proxy (reflection_method, delegate, signature, token_ref, exception_gchandle);
	xamarin_mono_object_release (&reflection_method);
	return rv;
}

void
xamarin_release_static_dictionaries ()
{
#if defined (CORECLR_RUNTIME)
	// Release static dictionaries of cached objects. If we end up trying to
	// add objects to these dictionaries after this point (on a background
	// thread), the dictionaries will be re-created (and leak) - which
	// shouldn't be a problem, because at this point the process is about to
	// exit anyway.
	pthread_mutex_lock (&wrapper_hash_lock);
	xamarin_mono_object_release (&xamarin_wrapper_hash);
	pthread_mutex_unlock (&wrapper_hash_lock);
#endif

	pthread_mutex_lock (&strong_gchandle_hash_lock);
	if (strong_gchandle_hash != NULL) {
		CFRelease (strong_gchandle_hash);
		strong_gchandle_hash = NULL;
	}
	pthread_mutex_unlock (&strong_gchandle_hash_lock);
}

void
xamarin_set_use_sgen (bool value)
{
}

bool
xamarin_get_use_sgen ()
{
	return true;
}

const char *
xamarin_skip_encoding_flags (const char *encoding)
{
	while (true) {
		switch (*encoding) {
		case 'r': // const
		case 'n': // in
		case 'N': // inout
		case 'o': // out
		case 'O': // bycopy
		case 'R': // byref
		case 'V': // oneway
			encoding++;
			continue;
		default:
			return encoding;
		}
	}
}

bool
xamarin_log_marshalled_exceptions ()
{
	if (xamarin_log_exceptions == XamarinTriStateNone) {
		const char *var = getenv ("XAMARIN_LOG_MARSHALLED_EXCEPTIONS");
		xamarin_log_exceptions = (var && *var) ? XamarinTriStateEnabled : XamarinTriStateDisabled;
	}
	return xamarin_log_exceptions == XamarinTriStateEnabled;
}

void
xamarin_log_managed_exception (MonoObject *exception, MarshalManagedExceptionMode mode)
{
	if (!xamarin_log_marshalled_exceptions ())
		return;

	GCHandle handle = xamarin_gchandle_new (exception, false);
	NSLog (@PRODUCT ": Processing managed exception for exception marshalling (mode: %i):\n%@", mode, xamarin_print_all_exceptions (handle));
	xamarin_gchandle_free (handle);
}

void
xamarin_log_objectivec_exception (NSException *exception, MarshalObjectiveCExceptionMode mode)
{
	if (!xamarin_log_marshalled_exceptions ())
		return;

	NSLog (@PRODUCT ": Processing Objective-C exception for exception marshalling (mode: %i):\n%@", mode, [exception debugDescription]);
}

void
xamarin_process_nsexception (NSException *ns_exception)
{
	xamarin_process_nsexception_using_mode (ns_exception, false, NULL);
}

void
xamarin_process_nsexception_using_mode (NSException *ns_exception, bool throwManagedAsDefault, GCHandle *output_exception)
{
	XamarinGCHandle *exc_handle;
	GCHandle exception_gchandle = INVALID_GCHANDLE;
	MarshalObjectiveCExceptionMode mode;

	mode = xamarin_on_marshal_objectivec_exception (ns_exception, throwManagedAsDefault, &exception_gchandle);

	if (exception_gchandle != INVALID_GCHANDLE) {
		PRINT (PRODUCT ": Got an exception while executing the MarshalObjectiveCException event (this exception will be ignored):");
		PRINT ("%@", xamarin_print_all_exceptions (exception_gchandle));
		xamarin_gchandle_free (exception_gchandle);
		exception_gchandle = INVALID_GCHANDLE;
	}

	if (mode == MarshalObjectiveCExceptionModeDefault)
		mode = MarshalObjectiveCExceptionModeThrowManagedException;
	
	xamarin_log_objectivec_exception (ns_exception, mode);

	switch (mode) {
	case MarshalObjectiveCExceptionModeUnwindManagedCode:
		@throw ns_exception;
		break;
	case MarshalObjectiveCExceptionModeThrowManagedException:
		exc_handle = [[ns_exception userInfo] objectForKey: @"XamarinManagedExceptionHandle"];
		GCHandle handle;
		if (exc_handle != NULL) {
			GCHandle e_handle = [exc_handle getHandle];
			GCHandle rethrow_exception_gchandle;

			//
			// We want to maintain the original stack trace of the exception, but unfortunately
			// calling mono_runtime_set_pending_exception directly with the original exception will overwrite
			// the original stack trace.
			//
			// The good news is that the managed ExceptionDispatchInfo class is able to capture
			// a stack trace for an exception and show it later.
			//
			// The xamarin_rethrow_managed_exception method will use ExceptionDispatchInfo
			// to throw an exception that contains the original stack trace, we will then capture that
			// exception, and pass it to mono_runtime_set_pending_exception.
			//
			xamarin_rethrow_managed_exception (e_handle, &rethrow_exception_gchandle);
			if (rethrow_exception_gchandle == INVALID_GCHANDLE) {
				PRINT (PRODUCT ": Did not get a rethrow exception, will throw the original exception. The original stack trace will be lost.");
				MonoObject *exc = xamarin_gchandle_get_target (e_handle);
				handle = xamarin_gchandle_new (exc, false);
				xamarin_mono_object_release (&exc);
			} else {
				handle = rethrow_exception_gchandle;
			}
		} else {
			handle = xamarin_create_ns_exception (ns_exception, &exception_gchandle);
			if (exception_gchandle != INVALID_GCHANDLE) {
				PRINT (PRODUCT ": Got an exception while creating a managed NSException wrapper (will throw this exception instead):");
				PRINT ("%@", xamarin_print_all_exceptions (exception_gchandle));
				handle = exception_gchandle;
				exception_gchandle = INVALID_GCHANDLE;
			}
		}

		if (output_exception == NULL) {
			MonoObject *exc = xamarin_gchandle_get_target (handle);
			mono_runtime_set_pending_exception ((MonoException *) exc, false);
			xamarin_mono_object_release (&exc);
			xamarin_gchandle_free (handle);
		} else {
			*output_exception = handle;
		}
		break;
	case MarshalObjectiveCExceptionModeAbort:
	default:
		xamarin_assertion_message ("Aborting due to unhandled Objective-C exception:\n%s\n", [[ns_exception description] UTF8String]);
		break;
	}
}

// Since this method may not return, it will release the given exception object.
void
xamarin_process_managed_exception (MonoObject *exception)
{
	if (exception == NULL)
		return;

#if !defined (CORECLR_RUNTIME)
	if (mono_is_debugger_attached ())
		mono_debugger_agent_unhandled_exception ((MonoException *) exception);
#endif

	MarshalManagedExceptionMode mode;
	GCHandle exception_gchandle = INVALID_GCHANDLE;

	{
		GCHandle handle = xamarin_gchandle_new (exception, false);
		mode = xamarin_on_marshal_managed_exception (handle, &exception_gchandle);
		xamarin_gchandle_free (handle);
	}

	if (exception_gchandle != INVALID_GCHANDLE) {
		PRINT (PRODUCT ": Got an exception while executing the MarshalManagedException event (this exception will be ignored):");
		PRINT ("%@", xamarin_print_all_exceptions (exception_gchandle));
		xamarin_gchandle_free (exception_gchandle);
		exception_gchandle = INVALID_GCHANDLE;
		mode = MarshalManagedExceptionModeDefault;
	}

	if (mode == MarshalManagedExceptionModeDefault) {
		mode = MarshalManagedExceptionModeThrowObjectiveCException;
	}

	xamarin_log_managed_exception (exception, mode);

	switch (mode) {
#if !defined (CORECLR_RUNTIME) // CoreCLR won't unwind through native frames, so we'll have to abort (in the default case statement)
	case MarshalManagedExceptionModeDisable:
	case MarshalManagedExceptionModeUnwindNativeCode: {
		//
		// We want to maintain the original stack trace of the exception, but unfortunately
		// calling mono_raise_exception directly with the original exception will overwrite
		// the original stack trace.
		//
		// The good news is that the managed ExceptionDispatchInfo class is able to capture
		// a stack trace for an exception and show it later.
		//
		// The xamarin_rethrow_managed_exception method will use ExceptionDispatchInfo
		// to throw an exception that contains the original stack trace.
		//

		GCHandle handle = xamarin_gchandle_new (exception, false);
		xamarin_rethrow_managed_exception (handle, &exception_gchandle);
		xamarin_gchandle_free (handle);

		if (exception_gchandle == INVALID_GCHANDLE) {
			PRINT (PRODUCT ": Did not get a rethrow exception, will throw the original exception. The original stack trace will be lost.");
		} else {
			xamarin_mono_object_release (&exception);
			exception = xamarin_gchandle_get_target (exception_gchandle);
			xamarin_gchandle_free (exception_gchandle);
		}

		// If we end up here as part of an unhandled Objective-C exception, we're also trying to detect an infinite loop by
		// setting the xamarin_handling_unhandled_exceptions variable to 1 before processing the unhandled Objective-C exception,
		// and clearing it afterwards. However, the clearing of the variable will never happen if Mono unwinds native
		// stack frames, and then on the next unhandled Objective-C exception we'll think we're recursing when we're really not.
		// (FWIW this is yet another reason why letting Mono unhandled native frames is a really bad idea).
		// So here we work around that by clearing the variable before letting Mono unwind native frames.
		xamarin_handling_unhandled_exceptions = 0;

		mono_raise_exception ((MonoException *) exception);
		break;
	}
#endif
	case MarshalManagedExceptionModeThrowObjectiveCException: {
		GCHandle handle = xamarin_gchandle_new (exception, false);
		NSException *ns_exc = xamarin_unwrap_ns_exception (handle, &exception_gchandle);
		
		if (exception_gchandle != INVALID_GCHANDLE) {
			PRINT (PRODUCT ": Got an exception while unwrapping a managed NSException wrapper (this exception will be ignored):");
			PRINT ("%@", xamarin_print_all_exceptions (exception_gchandle));
			xamarin_gchandle_free (exception_gchandle);
			exception_gchandle = INVALID_GCHANDLE;
			ns_exc = NULL;
		}

		if (ns_exc != NULL) {
			xamarin_gchandle_free (handle);
		} else {
			// Strangely enough the thread might be detached, if xamarin_process_managed_exception was called from
			// xamarin_ftnptr_exception_handler for an exception that occurred in a reverse delegate that
			// was called on a detached thread, since in that case the native-to-managed wrapper will have
			// returned the thread to a detached state after calling the managed function.
			NSString *name;
			NSString *reason;
			NSDictionary *userInfo;
			char *fullname;
			MONO_THREAD_ATTACH;
			
			fullname = xamarin_get_object_type_fullname (handle, &exception_gchandle);
			if (exception_gchandle != INVALID_GCHANDLE) {
				PRINT (PRODUCT ": Got an exception when trying to get the typename for an exception (this exception will be ignored):");
				PRINT ("%@", xamarin_print_all_exceptions (exception_gchandle));
				xamarin_gchandle_free (exception_gchandle);
				exception_gchandle = INVALID_GCHANDLE;
				name = @"Unknown type";
			} else {
				name = [NSString stringWithUTF8String: fullname];
				xamarin_free (fullname);
			}

			reason = xamarin_print_all_exceptions (handle);
			if (exception_gchandle != INVALID_GCHANDLE) {
				PRINT (PRODUCT ": Got an exception when trying to get the message for an exception (this exception will be ignored):");
				PRINT ("%@", xamarin_print_all_exceptions (exception_gchandle));
				xamarin_gchandle_free (exception_gchandle);
				exception_gchandle = INVALID_GCHANDLE;
				reason = @"Unknown message";
			}

			userInfo = [NSDictionary dictionaryWithObject: [XamarinGCHandle createWithHandle: handle] forKey: @"XamarinManagedExceptionHandle"];
			
			MONO_THREAD_DETACH;
			
			ns_exc = [[NSException alloc] initWithName: name reason: reason userInfo: userInfo];
		}

		xamarin_mono_object_release (&exception);

		@throw ns_exc;
	}
#if defined (CORECLR_RUNTIME)
	case MarshalManagedExceptionModeDisable:
	case MarshalManagedExceptionModeUnwindNativeCode:
#endif
	case MarshalManagedExceptionModeAbort:
	default: {
		GCHandle handle = xamarin_gchandle_new (exception, false);
		const char *msg = [xamarin_print_all_exceptions (handle) UTF8String];
		xamarin_gchandle_free (handle);
		xamarin_assertion_message ("Aborting due to trying to marshal managed exception:\n%s\n", msg);
		break;
	}
	}
}

void
xamarin_throw_product_exception (int code, const char *message)
{
	xamarin_process_managed_exception_gchandle (xamarin_create_product_exception (code, message));
}

GCHandle
xamarin_create_product_exception (int code, const char *message)
{
	return xamarin_create_product_exception_with_inner_exception (code, 0, message);
}

GCHandle
xamarin_create_product_exception_with_inner_exception (int code, GCHandle inner_exception_gchandle, const char *message)
{
	GCHandle exception_gchandle = INVALID_GCHANDLE;
	GCHandle handle = xamarin_create_product_exception_for_error (code, inner_exception_gchandle, message, &exception_gchandle);
	if (exception_gchandle != INVALID_GCHANDLE)
		return exception_gchandle;
	return handle;
}

// List all the assemblies that we can find in the app bundle in:
// - The bundle directory
// - The runtimeidentifier-specific subdirectory
NSArray<NSString *> *
xamarin_compute_trusted_platform_assemblies_at_runtime ()
{
	const char *bundle_path = xamarin_get_bundle_path ();

	NSMutableArray<NSString *> *files = [NSMutableArray array];
	NSMutableArray<NSString *> *exes = [NSMutableArray array];
	NSMutableArray<NSString *> *directories = [NSMutableArray array];
	[directories addObject: [NSString stringWithUTF8String: bundle_path]];
	[directories addObject: [NSString stringWithFormat: @"%s/.xamarin/%s", bundle_path, RUNTIMEIDENTIFIER]];

	NSFileManager *manager = [NSFileManager defaultManager];
	for (NSString *dir in directories) {
		NSDirectoryEnumerator *enumerator = [manager enumeratorAtURL:[NSURL fileURLWithPath: dir]
		                                  includingPropertiesForKeys:@[NSURLNameKey, NSURLIsDirectoryKey]
		                                                     options:NSDirectoryEnumerationSkipsSubdirectoryDescendants
		                                                     errorHandler:nil];
		for (NSURL *file in enumerator) {
			// skip subdirectories
			NSNumber *isDirectory = nil;
			if (![file getResourceValue:&isDirectory forKey:NSURLIsDirectoryKey error:nil] || [isDirectory boolValue])
				continue;

			NSString *name = nil;
			if (![file getResourceValue:&name forKey:NSURLNameKey error:nil])
				continue;

			if ([name length] < 4)
				continue;

			// We want dlls and exes
			if ([name compare: @".dll" options: NSCaseInsensitiveSearch range: NSMakeRange ([name length] - 4, 4)] == NSOrderedSame) {
				[files addObject: [dir stringByAppendingPathComponent: name]];
			} else if ([name compare: @".exe" options: NSCaseInsensitiveSearch range: NSMakeRange ([name length] - 4, 4)] == NSOrderedSame) {
				[exes addObject: [dir stringByAppendingPathComponent: name]];
			}
		}
	}

	// Any .exe files must be at the end, due to https://github.com/dotnet/runtime/issues/62735
	[files addObjectsFromArray: exes];

	return files;
}

NSArray<NSString *> *
xamarin_compute_trusted_platform_assemblies ()
{
	if (xamarin_trusted_platform_assembly_names == NULL || xamarin_trusted_platform_assembly_names [0] == NULL)
		return xamarin_compute_trusted_platform_assemblies_at_runtime ();

	const char *bundle_path = xamarin_get_bundle_path ();
	NSMutableArray<NSString *> *files = [NSMutableArray array];
#if defined (SUPPORTS_UNIVERSAL_BUILDS)
	NSFileManager *manager = xamarin_is_multi_rid_build ? [NSFileManager defaultManager] : nil;
#endif

	for (const char * const *assembly = xamarin_trusted_platform_assembly_names; *assembly != NULL; assembly++) {
		NSString *path = [NSString stringWithFormat: @"%s/%s", bundle_path, *assembly];
#if defined (SUPPORTS_UNIVERSAL_BUILDS)
		if (xamarin_is_multi_rid_build && ![manager fileExistsAtPath: path])
			path = [NSString stringWithFormat: @"%s/.xamarin/%s/%s", bundle_path, RUNTIMEIDENTIFIER, *assembly];
#endif
		[files addObject: path];
	}

	return files;
}

struct xamarin_host_assembly_path {
	const char *directory;
	const char *file_name;
};

static const char **xamarin_host_assembly_names;
static struct xamarin_host_assembly_path *xamarin_host_assembly_paths;
static size_t xamarin_host_assembly_count;

static void
xamarin_initialize_trusted_platform_assemblies ()
{
	NSArray<NSString *> *files = xamarin_compute_trusted_platform_assemblies ();
	xamarin_host_assembly_names = (const char **) xamarin_calloc (sizeof (char *) * ([files count] + 1));
	xamarin_host_assembly_paths = (struct xamarin_host_assembly_path *) xamarin_calloc (sizeof (*xamarin_host_assembly_paths) * ([files count] + 1));
	NSMutableSet<NSString *> *names = [NSMutableSet set];
	for (NSString *path in files) {
		NSString *file_name = [path lastPathComponent];
		NSString *name = [file_name stringByDeletingPathExtension];
		NSString *key = [name lowercaseString];
		if ([names containsObject: key])
			continue;
		[names addObject: key];

		// The runtime may request these strings after startup, so keep them for the process lifetime.
		size_t i = xamarin_host_assembly_count++;
		xamarin_host_assembly_names [i] = strdup ([name UTF8String]);
		xamarin_host_assembly_paths [i].directory = strdup ([[path stringByDeletingLastPathComponent] UTF8String]);
		xamarin_host_assembly_paths [i].file_name = strdup ([file_name UTF8String]);
	}
}

static bool
xamarin_get_assembly_names (const char * const **names, size_t *count, void *contract_context)
{
	*names = xamarin_host_assembly_names;
	*count = xamarin_host_assembly_count;
	return true;
}

static bool
xamarin_resolve_assembly_to_path (const char *simple_name, const char **directory, const char **file_name, void *contract_context)
{
	for (size_t i = 0; i < xamarin_host_assembly_count; i++) {
		if (strcasecmp (simple_name, xamarin_host_assembly_names [i]) != 0)
			continue;

		*directory = xamarin_host_assembly_paths [i].directory;
		*file_name = xamarin_host_assembly_paths [i].file_name;
		return true;
	}
	return false;
}

// Find the directory that contains System.Private.CoreLib.dll, looking in:
// - The bundle directory
// - The runtimeidentifier-specific subdirectory
// Returns an empty string if the file can't be found in any of those directories
// (an empty value is treated by the runtime as if the property wasn't set).
// Caller must free the return value using xamarin_free.
char *
xamarin_compute_system_corelib_directory ()
{
	const char *bundle_path = xamarin_get_bundle_path ();

	NSMutableArray<NSString *> *directories = [NSMutableArray array];
	[directories addObject: [NSString stringWithUTF8String: bundle_path]];
	[directories addObject: [NSString stringWithFormat: @"%s/.xamarin/%s", bundle_path, RUNTIMEIDENTIFIER]];

	NSFileManager *manager = [NSFileManager defaultManager];
	for (NSString *dir in directories) {
		NSString *corelib = [dir stringByAppendingPathComponent: @"System.Private.CoreLib.dll"];
		if ([manager fileExistsAtPath: corelib])
			return xamarin_strdup_printf ("%s", [dir UTF8String]);
	}

	return xamarin_strdup_printf ("%s", "");
}

char *
xamarin_compute_native_dll_search_directories ()
{
	const char *bundle_path = xamarin_get_bundle_path ();

	NSMutableArray<NSString *> *directories = [NSMutableArray array];

	// Always check in the root directory first.
	[directories addObject: @"/"];

	// Native libraries might be in the app bundle
	[directories addObject: [NSString stringWithUTF8String: bundle_path]];
	// They won't be in the runtimeidentifier-specific directory (because they get lipo'ed into a fat file instead)
	// However, might also be in the Resources/lib directory
	[directories addObject: [[[NSBundle mainBundle] resourcePath] stringByAppendingPathComponent: @"lib"]];

	// Missing:
	// * The parent app bundle if launched from an app extension. This requires adding app extension tests, which we currently don't have.

	// Remove the ones that don't exist
	NSFileManager *manager = [NSFileManager defaultManager];
	for (int i = (int) [directories count] - 1; i >= 0; i--) {
		NSString *dir = [directories objectAtIndex: (NSUInteger) i];
		BOOL isDirectory;
		if ([manager fileExistsAtPath: dir isDirectory: &isDirectory] && isDirectory)
			continue;

		[directories removeObjectAtIndex: (NSUInteger) i];
	}

	// Join them all together with a colon separating them
	NSString *joined = [directories componentsJoinedByString: @":"];
	char *rv = xamarin_strdup_printf ("%s", [joined UTF8String]);
	return rv;
}

#if defined (CORECLR_RUNTIME)
size_t
xamarin_get_dyld_image_size (void* base_address)
{
	if (base_address == NULL)
		return 0;

	const struct mach_header_64* header = (const struct mach_header_64 *) base_address;

	// Only support 64-bit Mach-O images.
	if (header->magic != MH_MAGIC_64 && header->magic != MH_CIGAM_64) {
		// Not a 64-bit Mach-O image. Return 0 or handle as appropriate.
		return 0;
	}
	const struct load_command* cmd = (const struct load_command*) ((const char *) header + sizeof (struct mach_header_64));

	size_t image_size = 0;
	for (uint32_t j = 0; j < header->ncmds; ++j) {
		if (cmd->cmd == LC_SEGMENT_64) {
			const struct segment_command_64* seg = (const struct segment_command_64 *) cmd;
			size_t end_addr = (size_t) (seg->vmaddr + seg->vmsize);
			if (end_addr > image_size)
				image_size = end_addr;
		}

		cmd = (const struct load_command *) ((const char *) cmd + cmd->cmdsize);
	}

	return image_size;
}

bool
xamarin_get_native_code_data (const struct host_runtime_contract_native_code_context* context, struct host_runtime_contract_native_code_data* data)
{
	if (!context || !data || !context->assembly_path || !context->owner_composite_name)
		return false;

	void* r2r_header = NULL;

	for (int i = 0; i < xamarin_r2r_module_count; i++) {
		if (strcmp (xamarin_r2r_modules [i].name, context->owner_composite_name) == 0) {
			r2r_header = xamarin_r2r_modules [i].header;
			break;
		}
	}

	if (r2r_header == NULL)
		return false;

	Dl_info info;
	if (dladdr (r2r_header, &info) == 0)
		xamarin_assertion_message ("Failed to get dladdr info for the RTR_HEADER symbol.");

	data->size = sizeof (struct host_runtime_contract_native_code_data);
	data->r2r_header_ptr = r2r_header;
	data->image_size = xamarin_get_dyld_image_size (info.dli_fbase);
	data->image_base = info.dli_fbase;

	return true;
}

// dotnet/runtime#128278 (.NET 11 preview 5) gates the BindToSystem CoreLib fallback on
// Bundle::AppIsBundle () && Bundle::AppBundle->HasExtractedFiles (). Register a no-op
// bundle_probe so AppBundle is non-null, and answer BUNDLE_EXTRACTION_PATH with the
// directory containing System.Private.CoreLib.dll so the fallback finds it.
static bool xamarin_coreclr_bundle_probe (const char *path, int64_t *offset, int64_t *size, int64_t *compressed_size)
{
	return false;
}

static const char *xamarin_compute_corelib_directory (void)
{
	static char *cached = NULL;

	if (cached)
		return cached;

	const char *bundle_path = xamarin_get_bundle_path ();
	NSFileManager *manager = [NSFileManager defaultManager];
	NSString *candidates [] = {
		[NSString stringWithUTF8String: bundle_path],
		[NSString stringWithFormat: @"%s/.xamarin/%s", bundle_path, RUNTIMEIDENTIFIER],
	};
	for (size_t i = 0; i < sizeof (candidates) / sizeof (candidates [0]); i++) {
		NSString *probe = [candidates [i] stringByAppendingPathComponent: @"System.Private.CoreLib.dll"];
		if ([manager fileExistsAtPath: probe]) {
			cached = strdup ([candidates [i] UTF8String]);
			break;
		}
	}

	return cached;
}

static size_t xamarin_coreclr_get_runtime_property (const char *key, char *value_buffer, size_t value_buffer_size, void *contract_context)
{
	if (strcmp (key, HOST_PROPERTY_BUNDLE_EXTRACTION_PATH) != 0)
		return (size_t) -1;

	const char *dir = xamarin_compute_corelib_directory ();
	if (dir == NULL)
		return (size_t) -1;

	size_t len = strlen (dir);
	size_t required = len + 1; // include null terminator
	if (value_buffer != NULL && value_buffer_size >= required)
		memcpy (value_buffer, dir, required);
	return required;
}
#endif // defined (CORECLR_RUNTIME)

static bool
xamarin_is_sandboxed ()
{
#if TARGET_OS_OSX || TARGET_OS_MACCATALYST
	SecTaskRef task = SecTaskCreateFromSelf (NULL);
	if (task == NULL)
		return false;

	CFTypeRef entitlement = SecTaskCopyValueForEntitlement (task, CFSTR ("com.apple.security.app-sandbox"), NULL);
	bool is_sandboxed = false;
	if (entitlement != NULL && CFGetTypeID (entitlement) == CFBooleanGetTypeID ())
		is_sandboxed = CFBooleanGetValue ((CFBooleanRef) entitlement);

	if (entitlement != NULL)
		CFRelease (entitlement);
	CFRelease (task);

	return is_sandboxed;
#elif TARGET_OS_IOS || TARGET_OS_TV
	// TARGET_OS_IOS is set for Mac Catalyst too, so we check for TARGET_OS_MACCATALYST first.
	return true;
#else
	#error Unknown platform
#endif
}

static void
xamarin_initialize_crash_report_directory ()
{
	// Set DOTNET_CrashReportRootPath before loading CoreCLR so that crash reports
	// are written to a known, writable location that isn't backed up by iCloud.
	// We use NSCachesDirectory for this purpose.
	// Ref: https://github.com/dotnet/runtime/pull/128738

	// Only do this if crash reports are enabled (DOTNET_EnableCrashReport=1),
	// otherwise there's no point in setting the crash report directory.
	const char *enableCrashReport = getenv ("DOTNET_EnableCrashReport");
	if (enableCrashReport == NULL || strcmp (enableCrashReport, "1") != 0)
		return;

	// If DOTNET_CrashReportRootPath is already set, respect it and don't override it
	// (nor create any directory), since the caller is then responsible for the location.
	if (getenv ("DOTNET_CrashReportRootPath") != NULL)
		return;

	NSArray *paths = NSSearchPathForDirectoriesInDomains (NSCachesDirectory, NSUserDomainMask, YES);
	if (paths == nil || [paths count] == 0) {
		LOG (PRODUCT ": Could not find the caches directory for crash reports.\n");
		return;
	}

	NSString *cachesDir = paths [0];

	// For non-sandboxed desktop apps, NSCachesDirectory is shared between all apps,
	// so we need to use a subdirectory specific to this app (using the bundle identifier).
	// Sandboxed apps already get an app-specific directory.
	if (!xamarin_is_sandboxed ()) {
		NSString *bundleId = [[NSBundle mainBundle] bundleIdentifier];
		if (bundleId != nil) {
			cachesDir = [cachesDir stringByAppendingPathComponent: bundleId];
		}
	}

	// The .NET runtime's in-process crash reporter won't write any crash report files
	// unless DOTNET_CrashReportRootPath points to an already-existing directory, so make
	// sure the directory exists before setting the environment variable.
	NSError *error = nil;
	if (![[NSFileManager defaultManager] createDirectoryAtPath: cachesDir withIntermediateDirectories: YES attributes: nil error: &error]) {
		LOG (PRODUCT ": Could not create the crash report directory '%s': %s\n", [cachesDir UTF8String], [[error description] UTF8String]);
		return;
	}

	setenv ("DOTNET_CrashReportRootPath", [cachesDir UTF8String], 0 /* don't overwrite */);
}

void
xamarin_vm_initialize ()
{
	xamarin_initialize_crash_report_directory ();

	xamarin_initialize_trusted_platform_assemblies ();
	static const struct host_runtime_contract host_contract = {
		.size = sizeof (struct host_runtime_contract),
#if defined (CORECLR_RUNTIME)
		.get_runtime_property = &xamarin_coreclr_get_runtime_property,
		.bundle_probe = &xamarin_coreclr_bundle_probe,
#endif
		.pinvoke_override = &xamarin_pinvoke_override,
#if defined (CORECLR_RUNTIME)
		.get_native_code_data = &xamarin_get_native_code_data,
#endif
		.get_assembly_names = &xamarin_get_assembly_names,
		.resolve_assembly_to_path = &xamarin_resolve_assembly_to_path,
	};

	char contract_str [19]; // 0x + 16 hex digits + '\0'
	snprintf (contract_str, sizeof (contract_str), "0x%zx", (size_t) (&host_contract));
	char *native_dll_search_directories = xamarin_compute_native_dll_search_directories ();
	char *system_corelib_directory = xamarin_compute_system_corelib_directory ();
	const char *startupHooks = getenv ("DOTNET_STARTUP_HOOKS");
	const char *app_paths = xamarin_get_bundle_path ();
#if defined (CORECLR_RUNTIME)
	// APP_PATHS is separator-delimited; use only the callbacks when it cannot represent the bundle path.
	if (strchr (app_paths, ':') != NULL)
		app_paths = "";
#endif

	// All the properties we pass here must also be listed in the _RuntimeConfigReservedProperties item group
	// for the _CreateRuntimeConfiguration target in dotnet/targets/Xamarin.Shared.Sdk.targets.
	const char *propertyKeys[] = {
		"APP_CONTEXT_BASE_DIRECTORY", // path to where the managed assemblies are (usually at least - RID-specific assemblies will be in subfolders)
		"APP_PATHS",
		"HOST_RUNTIME_CONTRACT",
		"NATIVE_DLL_SEARCH_DIRECTORIES",
		"RUNTIME_IDENTIFIER",
		"SYSTEM_CORELIB_DIRECTORY", // the directory that contains System.Private.CoreLib.dll (must come before STARTUP_HOOKS, because we might not pass STARTUP_HOOKS)
		"STARTUP_HOOKS", // must be last entry (because we just decrement propertyCount to not pass it if it's not set)
	};
	const char *propertyValues[] = {
		xamarin_get_bundle_path (),
		app_paths,
		contract_str,
		native_dll_search_directories,
		RUNTIMEIDENTIFIER,
		system_corelib_directory,
		startupHooks,
	};
	static_assert (sizeof (propertyKeys) == sizeof (propertyValues), "The number of keys and values must be the same.");

	int propertyCount = (int) (sizeof (propertyValues) / sizeof (propertyValues [0]));
	if (startupHooks == NULL || startupHooks [0] == 0)
		propertyCount--;

	bool rv = xamarin_bridge_vm_initialize (propertyCount, propertyKeys, propertyValues);

	xamarin_free (native_dll_search_directories);
	xamarin_free (system_corelib_directory);

	if (!rv)
		xamarin_assertion_message ("Failed to initialize the VM");
}

static bool
xamarin_is_native_library (const char *libraryName)
{
	if (xamarin_runtime_libraries == NULL)
		return false;

	size_t libraryNameLength = strnlen (libraryName, 255 /* max length of file name across all relevant file systems currently in use */);
	// The libraries in xamarin_runtime_libraries are extension-less, so we need to
	// remove any .dylib extension for the library name we're comparing with too.
	if (libraryNameLength > 6 && strcmp (libraryName + libraryNameLength - 6, ".dylib") == 0)
		libraryNameLength -= 6;

	bool rv = false;
	for (int i = 0; xamarin_runtime_libraries [i] != NULL; i++) {
		// Check if the start of the current xamarin_runtime_libraries entry matches libraryName
		if (!strncmp (xamarin_runtime_libraries [i], libraryName, libraryNameLength)) {
			// The start matches, now check if that's all there is
			if (xamarin_runtime_libraries [i] [libraryNameLength] == 0) {
				// If so, we've got a match
				rv = true;
				break;
			}
		}
	}

	return rv;
}

const void*
xamarin_pinvoke_override (const char *libraryName, const char *entrypointName)
{

	void* symbol = NULL;

	if (!strcmp (libraryName, "__Internal")) {
		symbol = dlsym (RTLD_DEFAULT, entrypointName);
#if !defined (CORECLR_RUNTIME) // we're intercepting objc_msgSend calls using the managed System.Runtime.InteropServices.ObjectiveC.Bridge.SetMessageSendCallback instead.
#if defined (__x86_64__) || defined (__arm64__)
	} else if (!strcmp (libraryName, "/usr/lib/libobjc.dylib")) {
		if (xamarin_marshal_objectivec_exception_mode != MarshalObjectiveCExceptionModeDisable) {
			if (!strcmp (entrypointName, "objc_msgSend")) {
				symbol = (void *) &xamarin_dyn_objc_msgSend;
			} else if (!strcmp (entrypointName, "objc_msgSendSuper")) {
				symbol = (void *) &xamarin_dyn_objc_msgSendSuper;
#if !defined (__arm64__)
			} else if (!strcmp (entrypointName, "objc_msgSend_stret")) {
				symbol = (void *) &xamarin_dyn_objc_msgSend_stret;
			} else if (!strcmp (entrypointName, "objc_msgSendSuper_stret")) {
				symbol = (void *) &xamarin_dyn_objc_msgSendSuper_stret;
#endif // !defined (__arm64__)
			} else {
				return NULL;
			}
		} else {
			return NULL;
		}
#endif // defined (__x86_64__) || defined (__arm64__)
#endif // !defined (CORECLR_RUNTIME)
	} else if (xamarin_is_native_library (libraryName)) {
		switch (xamarin_libmono_native_link_mode) {
		case XamarinNativeLinkModeStaticObject:
			// lookup the symbol in loaded memory, like __Internal does.
			symbol = dlsym (RTLD_DEFAULT, entrypointName);
			break;
		case XamarinNativeLinkModeDynamicLibrary:
			// if we're not linking statically, then don't do anything at all, let mono handle whatever needs to be done
			return NULL;
		case XamarinNativeLinkModeFramework:
		default:
			// handle this as "DynamicLibrary" for now - do nothing.
			LOG (PRODUCT ": Unhandled libmono link mode: %i when looking up %s in %s", xamarin_libmono_native_link_mode, entrypointName, libraryName);
			return NULL;
		}
	} else {
		return NULL;
	}

	if (symbol == NULL) {
		LOG (PRODUCT ": Unable to resolve P/Invoke '%s' in the library '%s'", entrypointName, libraryName);
	}

	return symbol;
}

void
xamarin_printf (const char *format, ...)
{
	va_list list;
	va_start (list, format);
	xamarin_vprintf (format, list);
	va_end (list);
}

void
xamarin_vprintf (const char *format, va_list args)
{
	NSString *message = [[NSString alloc] initWithFormat: [NSString stringWithUTF8String: format] arguments: args];
	
	NSLog (@"%@", message);	

	objc_release (message);
}

void
xamarin_registrar_dlsym (void **function_pointer, const char *assembly, const char *symbol, int32_t id, const char* objcClassName)
{
	if (*function_pointer != NULL)
		return;

	*function_pointer = dlsym (RTLD_MAIN_ONLY, symbol);
	if (*function_pointer != NULL)
		return;

	GCHandle exception_gchandle = INVALID_GCHANDLE;
	*function_pointer = xamarin_lookup_unmanaged_function (assembly, symbol, id, objcClassName, &exception_gchandle);
	if (*function_pointer != NULL)
		return;

	if (exception_gchandle != INVALID_GCHANDLE)
		xamarin_process_managed_exception_gchandle (exception_gchandle);

	// This shouldn't really happen
	NSString *msg = [NSString stringWithFormat: @"Unable to load the symbol '%s' to call managed code: %@", symbol, xamarin_print_all_exceptions (exception_gchandle)];
	NSLog (@"%@", msg);
	@throw [[NSException alloc] initWithName: @"SymbolNotFoundException" reason: msg userInfo: NULL];
}

/*
 * File/resource lookup for assemblies
 *
 * Assemblies can be found in several locations:
 * 1. If the assembly is compiled to a framework, in the framework's MonoBundle directory.
 *    For extensions the framework may be in the containing app's Frameworks directory.
 *    If an extension is sharing code with the main app, then the assemblies whose build target isn't 'framework' will be located in the container app's root directory.
 *    A framework may contain multiple assemblies, so it's not possible to deduce the framework name from the assembly name.
 * 2. If the assembly is not a framework, in the app's root directory.
 *
 * The platform assembly (Microsoft.[iOS|tvOS|MacCatalyst|macOS].dll) and any assemblies
 * the platform assembly references (mscorlib.dll, System.dll) may be in a
 * pointer-size subdirectory (ARCH_SUBDIR), or an RID-specific subdirectory.
 * 
 * AOT data files will have an arch-specific infix.
 */

void
xamarin_get_assembly_name_without_extension (const char *aname, char *name, size_t namelen)
{
	size_t len = strnlen (aname, namelen);
	if (len == namelen)
		return;
	strlcpy (name, aname, namelen);
	if (namelen <= 4 || len <= 4)
		return;
	const char *ext = name + (len - 4);
	if (!strncmp (".exe", ext, 4) || !strncmp (".dll", ext, 4))
		name [len - 4] = 0; // strip off any extensions.
}

bool
xamarin_locate_app_resource (const char *resource, char *path, size_t pathlen)
{
	const char *app_path = xamarin_get_bundle_path ();
	return xamarin_locate_assembly_resource_for_root (app_path, NULL, resource, path, pathlen);
}

static bool
xamarin_locate_assembly_resource_for_root (const char *root, const char *culture, const char *resource, char *path, size_t pathlen)
{
	if (culture != NULL && *culture != 0) {
		// culture-specific directory
		if (snprintf (path, pathlen, "%s/%s/%s", root, culture, resource) < 0) {
			LOG (PRODUCT ": Failed to construct path for assembly resource (root directory: '%s', culture: '%s', resource: '%s'): %s", root, culture, resource, strerror (errno));
			return false;
		} else if (xamarin_file_exists (path)) {
			return true;
		}
	}

	// arch-specific extension
	if (snprintf (path, pathlen, "%s/%s.%s", root, resource, xamarin_arch_name) < 0) {
		LOG (PRODUCT ": Failed to construct path for resource: %s (4): %s", resource, strerror (errno));
		return false;
	} else if (xamarin_file_exists (path)) {
		return true;
	}

#if !MONOMAC
	// pointer-size subdirectory with arch-specific extension
	if (snprintf (path, pathlen, "%s/%s/%s.%s", root, ARCH_SUBDIR, resource, xamarin_arch_name) < 0) {
		LOG (PRODUCT ": Failed to construct path for resource: %s (5): %s", resource, strerror (errno));
		return false;
	} else if (xamarin_file_exists (path)) {
		return true;
	}

	// pointer-size subdirectory
	if (snprintf (path, pathlen, "%s/%s/%s", root, ARCH_SUBDIR, resource) < 0) {
		LOG (PRODUCT ": Failed to construct path for resource: %s (5): %s", resource, strerror (errno));
		return false;
	} else if (xamarin_file_exists (path)) {
		return true;
	}
#endif // !MONOMAC

	// RID-specific subdirectory
	if (snprintf (path, pathlen, "%s/.xamarin/%s/%s", root, RUNTIMEIDENTIFIER, resource) < 0) {
		LOG (PRODUCT ": Failed to construct path for resource: %s (5): %s", resource, strerror (errno));
		return false;
	} else if (xamarin_file_exists (path)) {
		return true;
	}

	if (culture != NULL && *culture != 0) {
		// culture-specific directory
		if (snprintf (path, pathlen, "%s/%s/.xamarin/%s/%s", root, culture, RUNTIMEIDENTIFIER, resource) < 0) {
			LOG (PRODUCT ": Failed to construct path for assembly resource (root directory: '%s', culture: '%s', resource: '%s', runtimeidentifier: %s): %s", root, culture, resource, RUNTIMEIDENTIFIER, strerror (errno));
			return false;
		} else if (xamarin_file_exists (path)) {
			return true;
		}
	}

	// just the file, no extensions, etc.
	if (snprintf (path, pathlen, "%s/%s", root, resource) < 0) {
		LOG (PRODUCT ": Failed to construct path for resource: %s (6): %s", resource, strerror (errno));
		return false;
	} else if (xamarin_file_exists (path)) {
		return true;
	}

	return false;
}

#if !defined (CORECLR_RUNTIME)
bool
xamarin_locate_assembly_resource_for_name (MonoAssemblyName *assembly_name, const char *resource, char *path, size_t pathlen)
{
	const char *culture = mono_assembly_name_get_culture (assembly_name);
	const char *aname = mono_assembly_name_get_name (assembly_name);
	return xamarin_locate_assembly_resource (aname, culture, resource, path, pathlen);
}
#endif

// #define LOG_RESOURCELOOKUP(...) do { NSLog (@ __VA_ARGS__); } while (0);
#define LOG_RESOURCELOOKUP(...)


bool
xamarin_locate_assembly_resource (const char *assembly_name, const char *culture, const char *resource, char *path, size_t pathlen)
{
	char root [1024];
	char aname [256];
	const char *app_path;

	LOG_RESOURCELOOKUP (PRODUCT ": Locating the resource '%s' for the assembly '%s' (culture: '%s').", resource, assembly_name, culture);

	app_path = xamarin_get_bundle_path ();

	xamarin_get_assembly_name_without_extension (assembly_name, aname, sizeof (aname));

	// First check if the directory is explicitly set. This directory is relative to the app bundle.
	// For extensions this might be a relative path that points to container app (../../Frameworks/...).
	const char *explicit_location = xamarin_find_assembly_directory (aname);
	if (explicit_location) {
		snprintf (root, sizeof (root), "%s/%s", app_path, explicit_location);
		if (xamarin_locate_assembly_resource_for_root (root, culture, resource, path, pathlen)) {
			LOG_RESOURCELOOKUP (PRODUCT ": Located resource '%s' from explicit path '%s': %s\n", resource, explicit_location, path);
			return true;
		}
		// If we have an explicit location, then that's where the assembly must be.
		LOG_RESOURCELOOKUP (PRODUCT ": Could not find the resource '%s' for the assembly '%s' (culture: '%s') in the explicit path '%s'.", resource, assembly_name, culture, explicit_location);
		return false;
	}

	// The root app directory
	if (xamarin_locate_assembly_resource_for_root (app_path, culture, resource, path, pathlen)) {
		LOG_RESOURCELOOKUP (PRODUCT ": Located resource '%s' from app bundle: %s\n", resource, path);
		return true;
	}

#if !MONOMAC && defined (__x86_64__)
	// In the simulator we also check in a 'simulator' subdirectory. This is
	// so that we can create a framework that works for both simulator and
	// device, without affecting device builds in any way (device-specific
	// files just go in the MonoBundle directory)
	snprintf (root, sizeof (root), "%s/Frameworks/%s.framework/MonoBundle/simulator", app_path, aname);
	if (xamarin_locate_assembly_resource_for_root (root, culture, resource, path, pathlen)) {
		LOG_RESOURCELOOKUP (PRODUCT ": Located resource '%s' from framework '%s': %s\n", resource, aname, path);
		return true;
	}
#endif // !MONOMAC

	// Then in a framework named as the assembly
	snprintf (root, sizeof (root), "%s/Frameworks/%s.framework/MonoBundle", app_path, aname);
	if (xamarin_locate_assembly_resource_for_root (root, culture, resource, path, pathlen)) {
		LOG_RESOURCELOOKUP (PRODUCT ": Located resource '%s' from framework '%s': %s\n", resource, aname, path);
		return true;
	}

	// Then in the container app's root directory (for extensions)
	if (xamarin_launch_mode == XamarinLaunchModeExtension) {
		snprintf (root, sizeof (root), "../..");
		if (xamarin_locate_assembly_resource_for_root (root, culture, resource, path, pathlen)) {
			LOG_RESOURCELOOKUP (PRODUCT ": Located resource '%s' from container app bundle '%s': %s\n", resource, aname, path);
			return true;
		}
	}

#if TARGET_OS_MACCATALYST
	snprintf (root, sizeof (root), "%s/Contents/%s", app_path, [xamarin_custom_bundle_name UTF8String]);
	if (xamarin_locate_assembly_resource_for_root (root, culture, resource, path, pathlen)) {
		LOG_RESOURCELOOKUP (PRODUCT ": Located resource '%s' from macOS content bundle '%s': %s\n", resource, aname, path);
		return true;
	}
#endif

	return false;
}

void
xamarin_set_assembly_directories (struct AssemblyLocations *directories)
{
	if (options.AssemblyLocations)
		xamarin_assertion_message ("Assembly directories already set.");

	options.AssemblyLocations = directories;
}

static int
compare_assembly_location (const void *key, const void *value)
{
	return strcmp ((const char *) key, ((struct AssemblyLocation *) value)->assembly_name);
}

const char *
xamarin_find_assembly_directory (const char *assembly_name)
{
	if (options.AssemblyLocations == NULL)
		return NULL;

	struct AssemblyLocation *entry;

	entry = (struct AssemblyLocation *) bsearch (assembly_name, options.AssemblyLocations->locations, options.AssemblyLocations->length, sizeof (struct AssemblyLocation), compare_assembly_location);

	return entry ? entry->location : NULL;
}

MonoMethod *
xamarin_get_managed_method_for_token (guint32 token_ref, GCHandle *exception_gchandle)
{
	MonoReflectionMethod *reflection_method;

	reflection_method = (MonoReflectionMethod *) xamarin_gchandle_unwrap (xamarin_get_method_from_token (token_ref, exception_gchandle));
	if (*exception_gchandle != INVALID_GCHANDLE) return NULL;

	MonoMethod *rv = xamarin_get_reflection_method_method (reflection_method);
	xamarin_mono_object_release (&reflection_method);
	return rv;
}

GCHandle
xamarin_gchandle_new (MonoObject *obj, bool pinned)
{
#if defined (CORECLR_RUNTIME)
	return xamarin_bridge_create_gchandle (obj == NULL ? INVALID_GCHANDLE : obj->gchandle, pinned ? XamarinGCHandleTypePinned : XamarinGCHandleTypeNormal);
#else
	return mono_gchandle_new_v2 (obj, pinned);
#endif
}

GCHandle
xamarin_gchandle_new_weakref (MonoObject *obj, bool track_resurrection)
{
#if defined (CORECLR_RUNTIME)
	return xamarin_bridge_create_gchandle (obj == NULL ? INVALID_GCHANDLE : obj->gchandle, track_resurrection ? XamarinGCHandleTypeWeakTrackResurrection : XamarinGCHandleTypeWeak);
#else
	return mono_gchandle_new_weakref_v2 (obj, track_resurrection);
#endif
}

MonoObject *
xamarin_gchandle_get_target (GCHandle handle)
{
	if (handle == INVALID_GCHANDLE)
		return NULL;

#if defined (CORECLR_RUNTIME)
	return xamarin_bridge_get_monoobject (handle);
#else
	return mono_gchandle_get_target_v2 (handle);
#endif
}

void
xamarin_gchandle_free (GCHandle handle)
{
	if (handle == INVALID_GCHANDLE)
		return;
#if defined (CORECLR_RUNTIME)
	xamarin_bridge_free_gchandle (handle);
#else
	mono_gchandle_free_v2 (handle);
#endif
}

MonoObject *
xamarin_gchandle_unwrap (GCHandle handle)
{
	if (handle == INVALID_GCHANDLE)
		return NULL;
	MonoObject *rv = xamarin_gchandle_get_target (handle);
	xamarin_gchandle_free (handle);
	return rv;
}

// Use custom native method to detect if a type is a user type.
// This way might produce better diagnostic info when running in lldb.
bool
xamarin_is_user_type (Class cls)
{
	Method setGCHandle = class_getInstanceMethod (cls, @selector(xamarinSetGCHandle:flags:data:));
	return setGCHandle != NULL;
}

/*
 * Object unregistration:
 *
 * This is a difficult problem, because we need to be able
 * to lookup the managed object (given the native object)
 * from inside the dealloc implementation in super classes.
 * 
 * Example:
 * 
 * MyManagedObject : MyNativeObject : NSObject
 * 
 * -[MyNativeObject dealloc] may call selectors, and in
 * that case we need to be able to look up the managed
 * object to invoke the corresponding managed methods.
 *
 * See 02474ac1dd and bug #24609.
 *
 * Note: it is not recommmended to invoke selectors from the
 * dealloc method, but iOS still does it (and third-party
 * libraries as well), so we have to cope with it.
 *
 * At the same time we must also not unregister after
 * the native memory has been freed, because then there's
 * a race condition where another thread can create a 
 * new native object with the same pointer and end up in
 * managed code, in which case we might find the managed
 * object for the first native object (which has been freed,
 * but not unregistered yet), and random InvalidCastExceptions
 * occur.
 *
 * See bug #29801.
 *
 * Solution:
 * 
 * Fortunately the complete deallocation sequence is documented:

01 // The documented Deallocation Timeline (WWDC 2011, Session 322, 36:22).
02 // 1. -release to zero
03 //     * Object is now deallocating and will die.
04 //     * New __weak references are not allowed, and will get nil.
05 //     * [self dealloc] is called
06 // 2. Subclass -dealloc
07 //     * bottom-most subclass -dealloc is called
08 //     * Non-ARC code manually releases iVars
09 //     * Walk the super-class chain calling -dealloc
10 // 3. NSObject -dealloc
11 //     * Simply calls the ObjC runtime object_dispose()
12 // 4. object_dispose()
13 //     * Call destructors for C++ iVars
14 //     * Call -release for ARC iVars
15 //     * Erase associated references
16 //     * Erase __weak references
17 //     * Call free()

 * and our solution to the problem is:
 *
 * a) For the static registrar, we execute code at line #13 above, by
 *    adding a C++ iVar (with a destructor). This is the XamarinObject
 *    type.
 *
 * b) For the dynamic registrar we have to use another solution, because
 *    it's not possible to add C++ iVars dynamically. Instead we create
 *    a new native object (XamarinAssociatedObject), which we associate
 *    with the native object we're tracking, and which will be deallocated
 *    when the tracked native object is erasing its associated references
 *    (line #15) from above.
 * 
 * Associated references is a lot slower and more memory hungry than the C++
 * iVar, which is why we're not using associated references in all cases.
 * 
 */

/*
 * XamarinObject
 *
 * With the static registrar we add a C++ ivar, and
 * in the destructor we unregister the managed instance.
 */

XamarinObject::~XamarinObject ()
{
	@try {
		xamarin_notify_dealloc (native_object, gc_handle);
	} @catch (NSException *ex) {
		NSLog (@"%@", ex);
	}
	native_object = NULL;
	gc_handle = INVALID_GCHANDLE;
}

/*
 * XamarinAssociatedObject
 * 
 * With the dynamic registrar we associate an instance
 * of this object to every user type, and in this object's
 * dealloc method we unregister the managed instance.
 */

@implementation XamarinAssociatedObject 
-(void) dealloc
{
	xamarin_notify_dealloc (native_object, gc_handle);
	native_object = NULL;
	gc_handle = INVALID_GCHANDLE;
	[super dealloc];
}
@end

/*
 * NonXamarinObject category
 *
 * Inject a default xamarinGetGCHandle implementation into every object,
 * so that we don't have to check if a type is a user type before
 * calling xamarinGetGCHandle. TODO: verify if this is really faster than
 * checking the type first.
 *
 * Do not add a xamarinSetGCHandle:flags:data: method, since we use the presence
 * of it to detect whether a particular type is a user type or not
 * (see Runtime.IsUserType).
 */

@implementation NSObject (NonXamarinObject)
-(GCHandle) xamarinGetGCHandle
{
	return INVALID_GCHANDLE;
}

-(enum XamarinGCHandleFlags) xamarinGetGCHandleFlags
{
	return XamarinGCHandleFlags_None;
}

-(struct NSObjectData*) xamarinGetNSObjectData
{
	return NULL;
}
@end

#if MONOMAC
void
xamarin_set_is_mkbundle (bool value)
{
	if (initialize_started)
		xamarin_assertion_message ("Fatal error: xamarin_set_is_mkbundle called after xamarin_initialize.\n");
	
	xamarin_is_mkbundle = value;
}

bool
xamarin_get_is_mkbundle ()
{
	return xamarin_is_mkbundle;
}
#endif

void
xamarin_set_is_debug (bool value)
{
	if (initialize_started)
		xamarin_assertion_message ("Fatal error: xamarin_set_is_debug called after xamarin_initialize.\n");
	
	xamarin_debug_mode = value;
}

bool
xamarin_get_is_debug ()
{
	return xamarin_debug_mode;
}

void
xamarin_set_is_managed_static_registrar (bool value)
{
	if (value) {
		options.flags = (InitializationFlags) (options.flags | InitializationFlagsIsManagedStaticRegistrar);
	} else {
		options.flags = (InitializationFlags) (options.flags & ~InitializationFlagsIsManagedStaticRegistrar);
	}
}

void
xamarin_set_is_trimmable_static_registrar (bool value)
{
	if (value) {
		options.flags = (InitializationFlags) (options.flags | InitializationFlagsIsTrimmableStaticRegistrar);
	} else {
		options.flags = (InitializationFlags) (options.flags & ~InitializationFlagsIsTrimmableStaticRegistrar);
	}
}

bool
xamarin_is_managed_exception_marshaling_disabled ()
{
#if defined (CORECLR_RUNTIME)
	return false; // never disable exception marshalling for CoreCLR.
#elif DEBUG
	switch (xamarin_marshal_managed_exception_mode) {
	case MarshalManagedExceptionModeDefault:
		// If all of the following are true:
		// * In debug mode
		// * Using the default exception marshaling mode
		// * The debugger is attached
		// Then disable managed exception marshaling.
		return mono_is_debugger_attached ();
	case MarshalManagedExceptionModeDisable:
		return true;
	default:
		return false;
	}
#else
	return false;
#endif
}

/* This is a function to validate that a pointer is pointing to an actual object (id). It's used for providing better diagnostics/crashes */
bool
xamarin_is_object_valid (id obj, char** error_message)
{
	kern_return_t rv;

	vm_size_t vmsize;
	vm_address_t address = (vm_address_t) obj;
	vm_region_basic_info_data_t info;
	mach_msg_type_number_t info_count = VM_REGION_BASIC_INFO_COUNT_64;

	memory_object_name_t object;

	*error_message = NULL;

	// A tagged object is a valid object, even though it's not an actual pointer
	// Note that tagged pointers only happens on 64-bit.
#if __LP64__
#if (TARGET_OS_OSX || TARGET_OS_MACCATALYST) && __x86_64__
#   define _OBJC_TAG_MASK 1UL
#else
#   define _OBJC_TAG_MASK (1UL<<63)
#endif

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-objc-pointer-introspection"
	if (((uintptr_t) obj & _OBJC_TAG_MASK) == _OBJC_TAG_MASK)
		return true;
#pragma clang diagnostic pop
#endif

	rv = vm_region_64 (mach_task_self (), &address, &vmsize, VM_REGION_BASIC_INFO, (vm_region_info_t) &info, &info_count, &object);
	if (rv != KERN_SUCCESS) {
		*error_message = xamarin_strdup_printf ("vm_region_64 returned kern error %i", (int) rv);
		return false;
	}

	if ((info.protection & VM_PROT_READ) != VM_PROT_READ) {
		*error_message = xamarin_strdup_printf ("vm_region_64 said memory isn't readable (protection: %i)", (int) info.protection);
		return false;
	}

	// Read the memory
	void* readMem = NULL;
	vm_size_t size = 0;
	rv = vm_read_overwrite (mach_task_self (), (vm_address_t) obj, sizeof (readMem), (vm_address_t) &readMem, &size);

	if (rv != KERN_SUCCESS)	{
		*error_message = xamarin_strdup_printf ("vm_read returned kern error %i", rv);
		return false;
	}

	return true;
}

#if TARGET_OS_IOS || TARGET_OS_TV
int
xamarin_get_runtime_arch ()
{
	#if TARGET_OS_SIMULATOR
		return 1;
	#else
		return 0;
	#endif
}
#endif // TARGET_OS_IOS || TARGET_OS_TV

/*
 * XamarinGCHandle
 */
@implementation XamarinGCHandle
+(XamarinGCHandle *) createWithHandle: (GCHandle) h
{
	XamarinGCHandle *rv = [[XamarinGCHandle alloc] init];
	rv->handle = h;
	objc_autorelease (rv);
	return rv;
}

-(void) dealloc
{
	xamarin_gchandle_free (handle);
	[super dealloc];
}

-(GCHandle) getHandle
{
	return handle;
}
@end
