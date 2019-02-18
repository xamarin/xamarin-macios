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

#include "product.h"
#include "shared.h"
#include "delegates.h"
#include "runtime-internal.h"
#include "xamarin/xamarin.h"

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
bool xamarin_detect_unified_build = true;
#else
// no automatic detection for XI, mtouch should do the right thing in the generated main.
bool xamarin_detect_unified_build = false;
#endif
#if MONOMAC
bool xamarin_use_new_assemblies = false;
#endif
#if DEBUG
bool xamarin_gc_pump = false;
#endif
#if MONOMAC
// FIXME: implement release mode for monomac.
bool xamarin_debug_mode = true;
bool xamarin_mac_hybrid_aot = false;
bool xamarin_mac_modern = false;
#else
bool xamarin_debug_mode = false;
#endif
bool xamarin_disable_lldb_attach = false;
#if DEBUG
bool xamarin_init_mono_debug = true;
#else
bool xamarin_init_mono_debug = false;
#endif
int xamarin_log_level = 0;
const char *xamarin_executable_name = NULL;
#if MONOMAC
NSString * xamarin_custom_bundle_name = nil;
bool xamarin_is_mkbundle = false;
char *xamarin_entry_assembly_path = NULL;
#endif
#if defined (__i386__)
const char *xamarin_arch_name = "i386";
#elif defined (__x86_64__)
const char *xamarin_arch_name = "x86_64";
#else
const char *xamarin_arch_name = NULL;
#endif

#if TARGET_OS_WATCH
bool xamarin_is_gc_coop = true;
#else
bool xamarin_is_gc_coop = false;
#endif
enum MarshalObjectiveCExceptionMode xamarin_marshal_objectivec_exception_mode = MarshalObjectiveCExceptionModeDefault;
enum MarshalManagedExceptionMode xamarin_marshal_managed_exception_mode = MarshalManagedExceptionModeDefault;
enum XamarinLaunchMode xamarin_launch_mode = XamarinLaunchModeApp;
bool xamarin_supports_dynamic_registration = true;

/* Callbacks */

xamarin_setup_callback xamarin_setup = NULL;
xamarin_register_module_callback xamarin_register_modules = NULL;
xamarin_register_assemblies_callback xamarin_register_assemblies = NULL;
xamarin_extension_main_callback xamarin_extension_main = NULL;

/* Local variable */

static MonoImage      *platform_image;
static MonoClass      *inativeobject_class;
static MonoClass      *nsobject_class;
static MonoClass      *nsvalue_class;
static MonoClass      *nsnumber_class;
static MonoClass      *nsstring_class;
static MonoClass      *runtime_class;

static pthread_mutex_t framework_peer_release_lock;
static MonoGHashTable *xamarin_wrapper_hash;

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
	void* x86_double_abi_stret_tramp;
	void* static_fpret_single_tramp;
	void* static_fpret_double_tramp;
	void* static_stret_tramp;
	void* x86_double_abi_static_stret_tramp;
	void* long_tramp;
	void* static_long_tramp;
#if MONOMAC
	void* copy_with_zone1;
	void* copy_with_zone2;
#endif
	void* get_gchandle_tramp;
	void* set_gchandle_tramp;
};

enum InitializationFlags : int {
	InitializationFlagsIsPartialStaticRegistrar = 0x01,
	/* unused									= 0x02,*/
	InitializationFlagsDynamicRegistrar			= 0x04,
	/* unused									= 0x08,*/
	InitializationFlagsIsSimulator				= 0x10,
};

struct InitializationOptions {
	int size; // the size of this structure. This is used for version checking.
	enum InitializationFlags flags;
	struct Delegates* Delegates;
	struct Trampolines* Trampolines;
	struct MTRegistrationMap* RegistrationData;
	enum MarshalObjectiveCExceptionMode MarshalObjectiveCExceptionMode;
	enum MarshalManagedExceptionMode MarshalManagedExceptionMode;
#if MONOMAC
	enum XamarinLaunchMode LaunchMode;
	const char *EntryAssemblyPath;
#endif
	struct AssemblyLocations* AssemblyLocations;
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
#if defined (__i386__)
	(void *) &xamarin_x86_double_abi_stret_trampoline,
#else
	NULL,
#endif
	(void *) &xamarin_static_fpret_single_trampoline,
	(void *) &xamarin_static_fpret_double_trampoline,
	(void *) &xamarin_static_stret_trampoline,
#if defined (__i386__)
	(void *) &xamarin_static_x86_double_abi_stret_trampoline,
#else
	NULL,
#endif
	(void *) &xamarin_longret_trampoline,
	(void *) &xamarin_static_longret_trampoline,
#if MONOMAC
	(void *) &xamarin_copyWithZone_trampoline1,
	(void *) &xamarin_copyWithZone_trampoline2,
#endif
	(void *) &xamarin_get_gchandle_trampoline,
	(void *) &xamarin_set_gchandle_trampoline,
};

static struct InitializationOptions options = { 0 };

struct Managed_NSObject {
	MonoObject obj;
	id handle;
	void *class_handle;
	uint8_t flags;
};

static void
xamarin_add_internal_call (const char *name, const void *method)
{
	/* COOP: With cooperative GC, icalls will run, like manageed methods,
	 * in GC Unsafe mode, avoiding a thread state trandition.  In return
	 * the icalls must guarantee that they won't block, or run indefinitely
	 * without a safepoint, by manually performing a transition to GC Safe
	 * mode.  With backward-compatible hybrid GC, icalls run in GC Safe
	 * mode and the Mono API functions take care of thread state
	 * transitions, so don't need to perform GC thread state transitions
	 * themselves.
	 *
	 */
	if (xamarin_is_gc_coop)
		mono_dangerous_add_raw_internal_call (name, method);
	else
		mono_add_internal_call (name, method);
}

id
xamarin_get_nsobject_handle (MonoObject *obj)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	return mobj->handle;
}

void
xamarin_set_nsobject_handle (MonoObject *obj, id handle)
{
	// COOP: Writing managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	mobj->handle  = handle;
}

uint8_t
xamarin_get_nsobject_flags (MonoObject *obj)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	return mobj->flags;
}

void
xamarin_set_nsobject_flags (MonoObject *obj, uint8_t flags)
{
	// COOP: Writing managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	mobj->flags = flags;
}

MonoType *
xamarin_get_parameter_type (MonoMethod *managed_method, int index)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	MonoMethodSignature *msig = mono_method_signature (managed_method);
	void *iter = NULL;
	MonoType *p = NULL;
	
	if (index == -1) {
		p = mono_signature_get_return_type (msig);
	} else {
		for (int i = 0; i < index + 1; i++)
			p = mono_signature_get_params (msig, &iter);
	}
	
	return p;
}

MonoObject *
xamarin_get_nsobject_with_type_for_ptr (id self, bool owns, MonoType* type, SEL selector, MonoMethod *managed_method, guint32 *exception_gchandle)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;

	int32_t created;
	return xamarin_get_nsobject_with_type_for_ptr_created (self, owns, type, &created, selector, managed_method, exception_gchandle);
}

MonoObject *
xamarin_get_nsobject_with_type_for_ptr_created (id self, bool owns, MonoType *type, int32_t *created, SEL selector, MonoMethod *managed_method, guint32 *exception_gchandle)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	MonoObject *mobj = NULL;
	uint32_t gchandle = 0;

	*created = false;

	if (self == NULL)
		return NULL;

	gchandle = xamarin_get_gchandle (self);

	if (gchandle != 0) {
		mobj = mono_gchandle_get_target (gchandle);
		if (mono_object_isinst (mobj, mono_class_from_mono_type (type)) != NULL)
			return mobj;
	}

	return xamarin_get_nsobject_with_type (self, mono_type_get_object (mono_domain_get (), type), created, selector, managed_method == NULL ? NULL : mono_method_get_object (mono_domain_get (), managed_method, NULL), exception_gchandle);
}

MonoObject *
xamarin_get_managed_object_for_ptr (id self, guint32 *exception_gchandle)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	MonoObject *mobj = NULL;
	uint32_t gchandle = 0;

	if (self == NULL)
		return NULL;

	gchandle = xamarin_get_gchandle (self);

	if (gchandle == 0) {
		mobj = (MonoObject *) xamarin_try_get_or_construct_nsobject (self, exception_gchandle);
	} else {
		mobj = mono_gchandle_get_target (gchandle);
	}

	return mobj;
}

MonoObject *
xamarin_get_managed_object_for_ptr_fast (id self, guint32 *exception_gchandle)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	MonoObject *mobj = NULL;
	uint32_t gchandle = 0;

	gchandle = xamarin_get_gchandle (self);

	if (gchandle == 0) {
		mobj = (MonoObject *) xamarin_try_get_or_construct_nsobject (self, exception_gchandle);
	} else {
		mobj = mono_gchandle_get_target (gchandle);
#if DEBUG
		if (self != xamarin_get_nsobject_handle (mobj)) {
			xamarin_assertion_message ("Internal consistency error, please file a bug (https://github.com/xamarin/xamarin-macios/issues/new). Additional data: found managed object %p=%p (%s) in native object %p (%s).\n",
				mobj, xamarin_get_nsobject_handle (mobj), xamarin_class_get_full_name (mono_object_get_class (mobj), exception_gchandle), self, object_getClassName (self));
		}
#endif
	}

	return mobj;
}

void xamarin_framework_peer_lock ()
{
	// COOP: CHECK
	MONO_ASSERT_GC_UNSAFE;
	
	MONO_ENTER_GC_SAFE;
	pthread_mutex_lock (&framework_peer_release_lock);
	MONO_EXIT_GC_SAFE;
}

void xamarin_framework_peer_unlock ()
{
	pthread_mutex_unlock (&framework_peer_release_lock);
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

bool
xamarin_is_class_nsobject (MonoClass *cls)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	return mono_class_is_subclass_of (cls, nsobject_class, false);
}

bool
xamarin_is_class_inativeobject (MonoClass *cls)
{
	// COOP: Reading managed data, must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	return mono_class_is_subclass_of (cls, inativeobject_class, true);
}

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

	if (nsstring_class == NULL)
		return false;

	return mono_class_is_subclass_of (cls, nsstring_class, false);
}

// Returns if a MonoClass is nullable.
// Will also return the element type (it the type is nullable, and if out pointer is not NULL).
bool
xamarin_is_class_nullable (MonoClass *cls, MonoClass **element_type, guint32 *exception_gchandle)
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
			get_nullable_type = mono_class_get_method_from_name (runtime_class, "GetNullableType", 1);

		void *args [1] { mono_type_get_object (mono_domain_get (), mono_class_get_type (cls)) };
		MonoObject *exc = NULL;
		MonoReflectionType *nullable_type = (MonoReflectionType *) mono_runtime_invoke (get_nullable_type, NULL, args, &exc);
		if (exc != NULL) {
			*exception_gchandle = mono_gchandle_new (exc, FALSE);
			return false;
		}

		if (element_type != NULL && nullable_type != NULL)
			*element_type = mono_class_from_mono_type (mono_reflection_type_get_type (nullable_type));
		return nullable_type != NULL;
	}
#endif

	bool rv = mono_class_is_nullable (cls);
	if (rv && element_type)
		*element_type = mono_class_get_nullable_param (cls);
	return rv;
}

MonoClass *
xamarin_get_nullable_type (MonoClass *cls, guint32 *exception_gchandle)
{
	MonoClass *rv = NULL;
	xamarin_is_class_nullable (cls, &rv, exception_gchandle);
	return rv;
}

#define MANAGED_REF_BIT (1 << 31)
#define GCHANDLE_WEAK (1 << 30)
#define GCHANDLE_MASK (MANAGED_REF_BIT | GCHANDLE_WEAK)

// The XamarinExtendedObject protocol is just to avoid a 
// compiler warning (no 'xamarinGetGChandle' selector found).
@protocol XamarinExtendedObject
-(int) xamarinGetGCHandle;
-(void) xamarinSetGCHandle: (int) gc_handle;
@end

static inline int
get_raw_gchandle_safe (id self)
{
	// COOP: we call a selector, and that must only be done in SAFE mode.
	MONO_ASSERT_GC_SAFE;
	id<XamarinExtendedObject> xself = self;
	return (int) [xself xamarinGetGCHandle];
}

static inline int
get_raw_gchandle (id self)
{
	// COOP: we call a selector, and that must only be done in SAFE mode.
	MONO_ASSERT_GC_UNSAFE;
	
	int rv;
	MONO_ENTER_GC_SAFE;
	id<XamarinExtendedObject> xself = self;
	rv = (int) [xself xamarinGetGCHandle];
	MONO_EXIT_GC_SAFE;
	
	return rv;
}

static inline void
set_raw_gchandle (id self, int gc_handle)
{
	// COOP: we call a selector, and that must only be done in SAFE mode.
	MONO_ASSERT_GC_UNSAFE;
	
	MONO_ENTER_GC_SAFE;
	id<XamarinExtendedObject> xself = self;
	[xself xamarinSetGCHandle: gc_handle];
	MONO_EXIT_GC_SAFE;
}

static inline int
get_gchandle (id self)
{
	// COOP: does not access managed memory: any mode
	return get_raw_gchandle (self) & ~GCHANDLE_MASK;
}

int
xamarin_get_gchandle (id self)
{
	// COOP: does not access managed memory: any mode
	return get_gchandle (self);
}

int
xamarin_get_gchandle_with_flags (id self)
{
	// COOP: does not access managed memory: any mode
	return get_raw_gchandle (self);
}

bool
xamarin_has_managed_ref (id self)
{
	// COOP: get_raw_gchandle requires UNSAFE mode, so this function requires it too.
	return get_raw_gchandle (self) & MANAGED_REF_BIT;
}

bool
xamarin_has_managed_ref_safe (id self)
{
	// COOP: variation of xamarin_has_managed_ref for SAFE mode.
	return get_raw_gchandle_safe (self) & MANAGED_REF_BIT;
}

MonoException *
xamarin_create_exception (const char *msg)
{
	// COOP: calls mono, needs to be in UNSAFE mode.
	MONO_ASSERT_GC_UNSAFE;
	
	return (MonoException *) mono_exception_from_name_msg (mono_get_corlib (), "System", "Exception", msg);
}

typedef struct {
	MonoObject object;
	MonoMethod *method;
	MonoString *name;
	MonoReflectionType *reftype;
} PublicMonoReflectionMethod;

MonoMethod *
xamarin_get_reflection_method_method (MonoReflectionMethod *method)
{
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	PublicMonoReflectionMethod *rm = (PublicMonoReflectionMethod *) method;
	return rm->method;
}

id
xamarin_get_handle (MonoObject *obj, guint32 *exception_gchandle)
{
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
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
		MonoException *exc = mono_get_exception_execution_engine (msg);
		xamarin_free (msg);
		*exception_gchandle = mono_gchandle_new ((MonoObject *) exc, FALSE);
	}
	
	return rv;
}

#if DEBUG
static void 
verify_cast (MonoClass *to, MonoObject *obj, Class from_class, SEL sel, MonoMethod *method, guint32 *exception_gchandle)
{
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
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
		MonoException *mono_ex = mono_exception_from_name_msg (mono_get_corlib (), "System", "InvalidCastException", msg);
		mono_free (from_name);
		mono_free (to_name);
		xamarin_free (msg);
		mono_free (method_full_name);
		*exception_gchandle = mono_gchandle_new ((MonoObject *) mono_ex, FALSE);
	}
}
#endif

void
xamarin_check_for_gced_object (MonoObject *obj, SEL sel, id self, MonoMethod *method, guint32 *exception_gchandle)
{
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	if (obj != NULL) {
#if DEBUG
		verify_cast (mono_method_get_class (method), obj, [self class], sel, method, exception_gchandle);
#endif
		return;
	}
	
	const char *m = "Failed to marshal the Objective-C object %p (type: %s). "
	"Could not find an existing managed instance for this object, "
	"nor was it possible to create a new managed instance "
	"(because the type '%s' does not have a constructor that takes one IntPtr argument).\n"
	"Additional information:\n"
	"\tSelector: %s\n"
	"\tMethod: %s\n";
	
	char *method_full_name = mono_method_full_name (method, TRUE);
	char *type_name = xamarin_lookup_managed_type_name ([self class], exception_gchandle);
	if (*exception_gchandle == 0) {
		char *msg = xamarin_strdup_printf (m, self, object_getClassName (self), type_name, sel_getName (sel), method_full_name);
		guint32 ex_handle = xamarin_create_runtime_exception (8027, msg, exception_gchandle);
		xamarin_free (msg);
		if (*exception_gchandle == 0)
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
xamarin_verify_parameter (MonoObject *obj, SEL sel, id self, id arg, int index, MonoClass *expected, MonoMethod *method)
{
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
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
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
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
//	MonoException *mono_ex = mono_exception_from_name_msg (mono_get_corlib (), "System", "InvalidCastException", msg);
//	xamarin_free (msg);
//	mono_free (method_full_name);
//	mono_raise_exception (mono_ex);
}
#endif

char *
xamarin_class_get_full_name (MonoClass *klass, guint32 *exception_gchandle)
{
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	return xamarin_type_get_full_name (mono_class_get_type (klass), exception_gchandle);
}

char *
xamarin_type_get_full_name (MonoType *type, guint32 *exception_gchandle)
{
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	return xamarin_reflection_type_get_full_name (mono_type_get_object (mono_domain_get (), type), exception_gchandle);
}

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

	PRINT ("**Registering object %p handle %p RC %d flags: %i",
		obj,
		handle,
		(int) (handle ? [handle retainCount] : 0),
		xamarin_get_nsobject_flags (obj));
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
	id handle = NULL;
	MonoToggleRefStatus res;

	uint8_t flags = xamarin_get_nsobject_flags (object);
	bool disposed = (flags & NSObjectFlagsDisposed) == NSObjectFlagsDisposed;
	bool has_managed_ref = (flags & NSObjectFlagsHasManagedRef) == NSObjectFlagsHasManagedRef;

	if (disposed || !has_managed_ref) {
		res = MONO_TOGGLE_REF_DROP; /* Already disposed, we don't need the managed object around */
	} else {
		handle = xamarin_get_nsobject_handle (object);
		if (handle == NULL) { /* This shouldn't really happen */
			return MONO_TOGGLE_REF_DROP;
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
	if (handle == NULL) {
		cn = object_getClassName (xamarin_get_nsobject_handle (object));
	} else {
		cn = object_getClassName (handle);
	}
	PRINT ("\tinspecting %p handle:%p %s flags: %i RC %d -> %s\n", object, handle, cn, (int) flags, (int) (handle ? [handle retainCount] : 0), rv);
#endif

	return res;
}

static void
gc_event_callback (MonoProfiler *prof, MonoGCEvent event, int generation)
{
	// COOP: this is a callback called by the GC, I believe the mode here doesn't matter.
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

static void
gc_enable_new_refcount (void)
{
	// COOP: this is executed at startup, I believe the mode here doesn't matter.
	pthread_mutexattr_t attr;
	pthread_mutexattr_init (&attr);
	pthread_mutexattr_settype (&attr, PTHREAD_MUTEX_RECURSIVE);
	pthread_mutex_init (&framework_peer_release_lock, &attr);
	pthread_mutexattr_destroy (&attr);

	mono_gc_toggleref_register_callback (gc_toggleref_callback);

	xamarin_add_internal_call (xamarin_use_new_assemblies ? "Foundation.NSObject::RegisterToggleRef" : PRODUCT_COMPAT_NAMESPACE ".Foundation.NSObject::RegisterToggleRef", (const void *) gc_register_toggleref);
	mono_profiler_install_gc (gc_event_callback, NULL);
}

static MonoClass *
get_class_from_name (MonoImage* image, const char *nmspace, const char *name, bool optional = false)
{
	// COOP: this is a convenience function executed only at startup, I believe the mode here doesn't matter.	
	MonoClass *rv = mono_class_from_name (image, nmspace, name);
	if (!rv && !optional)
		xamarin_assertion_message ("Fatal error: failed to load the class '%s.%s'\n.", nmspace, name);
	return rv;
}

struct _MonoProfiler {
	int dummy;
};

static void
xamarin_install_mono_profiler ()
{
	static _MonoProfiler profiler = { 0 };
	// This must be done before any other mono_profiler_install_* functions are called
	// (currently gc_enable_new_refcount and xamarin_install_nsautoreleasepool_hooks).
	mono_profiler_install (&profiler, NULL);
}

bool
xamarin_file_exists (const char *path)
{
	// COOP: no managed access: any mode
	struct stat buffer;
	return stat (path, &buffer) == 0;
}

MonoAssembly *
xamarin_open_assembly (const char *name)
{
	// COOP: this is a function executed only at startup, I believe the mode here doesn't matter.
	char path [1024];
	MonoAssembly *assembly;
	bool exists = false;

#if MONOMAC
	if (xamarin_get_is_mkbundle ()) {
		assembly = mono_assembly_open (name, NULL);
		if (assembly == NULL)
			xamarin_assertion_message ("Could not find the required assembly '%s' in the app. This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: https://github.com/xamarin/xamarin-macios/issues/new", name);
		return assembly;
	}
#endif

	exists = xamarin_locate_assembly_resource (name, NULL, name, path, sizeof (path));

#if MONOMAC && DYLIB
	if (!exists) {
		// Check if we already have the assembly in memory
		xamarin_get_assembly_name_without_extension (name, path, sizeof (path));
		MonoAssemblyName *aname = mono_assembly_name_new (path);
		assembly = mono_assembly_loaded (aname);
		mono_assembly_name_free (aname);
		if (assembly)
			return assembly;

		xamarin_assertion_message ("Could not find the assembly '%s' in the app nor as an already loaded assembly. This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: https://github.com/xamarin/xamarin-macios/issues/new", name);
	}
#endif

	if (!exists)
		xamarin_assertion_message ("Could not find the assembly '%s' in the app. This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: https://github.com/xamarin/xamarin-macios/issues/new", name);

	assembly = mono_assembly_open (path, NULL);
	if (assembly == NULL)
		xamarin_assertion_message ("Could not find the required assembly '%s' in the app. This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: https://github.com/xamarin/xamarin-macios/issues/new", name);

	return assembly;
}

static bool
register_assembly (MonoAssembly *assembly, guint32 *exception_gchandle)
{
	// COOP: this is a function executed only at startup, I believe the mode here doesn't matter.
	if (!xamarin_supports_dynamic_registration) {
		LOG (PRODUCT ": Skipping assembly registration for %s since it's not needed (dynamic registration is not supported)", mono_assembly_name_get_name (mono_assembly_get_name (assembly)));
		return true;
	}
	xamarin_register_assembly (mono_assembly_get_object (mono_domain_get (), assembly), exception_gchandle);
	return *exception_gchandle == 0;
}

MonoAssembly *
xamarin_open_and_register (const char *aname, guint32 *exception_gchandle)
{
	// COOP: this is a function executed only at startup, I believe the mode here doesn't matter.
	MonoAssembly *assembly;

	assembly = xamarin_open_assembly (aname);

	register_assembly (assembly, exception_gchandle);
	
	return assembly;
}

static gboolean 
is_class_finalization_aware (MonoClass *cls)
{
	// COOP: This is a callback called by the GC, I believe the mode here doesn't matter.
	gboolean rv = false;

	if (nsobject_class)
		rv = cls == nsobject_class || mono_class_is_assignable_from (nsobject_class, cls);

	//PRINT ("IsClass %s.%s finalization aware: %i\n", mono_class_get_namespace (cls), mono_class_get_name (cls), rv);

	return rv;
}

static void 
object_queued_for_finalization (MonoObject *object)
{
	// COOP: Although this is reading managed memory, it is a callback called by the GC, so I believe the mode here doesn't matter.
	
	/* This is called with the GC lock held, so it can only use signal-safe code */
	struct Managed_NSObject *obj = (struct Managed_NSObject *) object;
	//PRINT ("In finalization response for %s.%s %p (handle: %p class_handle: %p flags: %i)\n", 
	obj->flags |= NSObjectFlagsInFinalizerQueue;
}

/*
 * Registration map
 */ 

static int
compare_mtclassmap (const void *a, const void *b)
{
	MTClassMap *mapa = (MTClassMap *) a;
	MTClassMap *mapb = (MTClassMap *) b;
	if (mapa->handle == mapb->handle)
		return 0;
	if ((intptr_t) mapa->handle < (intptr_t) mapb->handle)
		return -1;
	return 1;
}

void
xamarin_add_registration_map (struct MTRegistrationMap *map, bool partial)
{
	// COOP: no managed memory access: any mode
	options.RegistrationData = map;
	if (partial)
		options.flags = (InitializationFlags) (options.flags | InitializationFlagsIsPartialStaticRegistrar);

	// Sort the type map according to Class
	qsort (map->map, map->map_count, sizeof (MTClassMap), compare_mtclassmap);
}

/*
 * Exception handling
 */

static XamarinUnhandledExceptionFunc unhandled_exception_func;

void 
xamarin_install_unhandled_exception_hook (XamarinUnhandledExceptionFunc func)
{
	// COOP: no managed memory access: any mode
	unhandled_exception_func = func;	
}

static MonoObject *
fetch_exception_property (MonoObject *obj, const char *name, bool is_virtual)
{
	// COOP: reading managed memory and executing managed code: must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	MonoMethod *get = NULL;
	MonoMethod *get_virt = NULL;
	MonoObject *exc = NULL;

	get = mono_class_get_method_from_name (mono_get_exception_class (), name, 0);
	if (get) {
		if (is_virtual) {
			get_virt = mono_object_get_virtual_method (obj, get);
			if (get_virt)
				get = get_virt;
		}

		return (MonoObject *) mono_runtime_invoke (get, obj, NULL, &exc);
	} else {
		PRINT ("Could not find the property System.Exception.%s", name);
	}
	
	return NULL;
}

static char *
fetch_exception_property_string (MonoObject *obj, const char *name, bool is_virtual)
{
	// COOP: reading managed memory and executing managed code: must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	MonoString *str = (MonoString *) fetch_exception_property (obj, name, is_virtual);
	return str ? mono_string_to_utf8 (str) : NULL;
}

static void
print_exception (MonoObject *exc, bool is_inner, NSMutableString *msg)
{
	// COOP: reading managed memory and executing managed code: must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	MonoClass *type = mono_object_get_class (exc);
	char *type_name = xamarin_strdup_printf ("%s.%s", mono_class_get_namespace (type), mono_class_get_name (type));
	char *trace = fetch_exception_property_string (exc, "get_StackTrace", true);
	char *message = fetch_exception_property_string (exc, "get_Message", true);

	if (is_inner)
		[msg appendString:@" --- inner exception ---\n"];
	[msg appendFormat: @"%s (%s)\n", message, type_name];
	if (trace)
		[msg appendFormat: @"%s\n", trace];

	if (unhandled_exception_func && !is_inner)
		unhandled_exception_func (exc, type_name, message, trace);

	mono_free (trace);
	mono_free (message);
	xamarin_free (type_name);
}

static NSMutableString *
print_all_exceptions (MonoObject *exc)
{
	// COOP: reading managed memory and executing managed code: must be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	NSMutableString *str = [[NSMutableString alloc] init];
	// fetch the field, since the property might have been linked away.
	int counter = 0;
	MonoClassField *inner_exception = mono_class_get_field_from_name (mono_object_get_class (exc), "_innerException");

	do {
		print_exception (exc, counter > 0, str);
		if (inner_exception) {
			mono_field_get_value (exc, inner_exception, &exc);
		} else {
			LOG ("Could not find the field _innerException in System.Exception\n");
			break;
		}
	} while (counter++ < 10 && exc);

	[str autorelease];
	return str;
}

NSString *
xamarin_print_all_exceptions (MonoObject *exc)
{
	return print_all_exceptions (exc);
}

void
xamarin_ftnptr_exception_handler (guint32 gchandle)
{
	xamarin_process_managed_exception_gchandle (gchandle);
}

void
xamarin_process_managed_exception_gchandle (guint32 gchandle)
{
	if (gchandle == 0)
		return;

	MonoObject *exc = mono_gchandle_get_target (gchandle);
	mono_gchandle_free (gchandle);
	xamarin_process_managed_exception (exc);	
}

void
xamarin_unhandled_exception_handler (MonoObject *exc, gpointer user_data)
{
	PRINT ("Unhandled managed exception: %@", print_all_exceptions (exc));

	abort ();
}

static void
exception_handler (NSException *exc)
{
	// COOP: We won't get here in coop-mode, because we don't set the uncaught objc exception handler in that case.
	LOG (PRODUCT ": Received unhandled ObjectiveC exception: %@ %@", [exc name], [exc reason]);

	if (xamarin_is_gc_coop) {
		PRINT ("Uncaught Objective-C exception: %@", exc);
		assert (false); // Re-throwing the Objective-C exception will probably just end up with infinite recursion
	}

	xamarin_throw_ns_exception (exc);
}

#if defined (DEBUG)
static void *
pump_gc (void *context)
{
	// COOP: this runs on a separate thread, so I'm not sure what happens here.
	//       We can make sure we're in safe mode while sleeping though.
	mono_thread_attach (mono_get_root_domain ());

	while (xamarin_gc_pump) {
		mono_gc_collect (mono_gc_max_generation ());
		MONO_ENTER_GC_SAFE;
		usleep (1000000);
		MONO_EXIT_GC_SAFE;
	}
	return NULL;
}
#endif /* DEBUG */

#if MONOMAC
static void
detect_product_assembly ()
{
	// COOP: Function only called at startup, I believe the mode here doesn't matter
	if (!xamarin_detect_unified_build)
		return;

	char path [1024];
	bool unified;
	bool compat;

	snprintf (path, sizeof (path), "%s/" ARCH_SUBDIR "/%s", xamarin_get_bundle_path (), PRODUCT_DUAL_ASSEMBLY);
	unified = xamarin_file_exists (path);
	snprintf (path, sizeof (path), "%s/" ARCH_SUBDIR "/%s", xamarin_get_bundle_path (), PRODUCT_COMPAT_ASSEMBLY);
	compat = xamarin_file_exists (path);

	if (unified && compat) {
		xamarin_assertion_message ("Found both " PRODUCT_COMPAT_ASSEMBLY " and " PRODUCT_DUAL_ASSEMBLY " in the app. Only one can be present");
	} else if (!unified && !compat) {
		xamarin_assertion_message ("Found neither " PRODUCT_COMPAT_ASSEMBLY " nor " PRODUCT_DUAL_ASSEMBLY " in the app.");
	} else if (unified) {
		xamarin_use_new_assemblies = true;
	} else {
		xamarin_use_new_assemblies = false;
	}
}
#endif

static void
log_callback (const char *log_domain, const char *log_level, const char *message, mono_bool fatal, void *user_data)
{
	// COOP: Not accessing managed memory: any mode
	PRINT ("%s: %s", log_level, message);

	if (fatal)
		abort ();
}

static void
print_callback (const char *string, mono_bool is_stdout)
{
	// COOP: Not accessing managed memory: any mode
	PRINT ("%s", string);
}

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

	void* ptr = bsearch (&token_ref, options.RegistrationData->protocol_wrappers, options.RegistrationData->protocol_wrapper_count, sizeof (MTProtocolWrapperMap), xamarin_compare_ints);
	if (ptr == NULL)
		return INVALID_TOKEN_REF;

	MTProtocolWrapperMap *entry = (MTProtocolWrapperMap *) ptr;
	return entry->wrapper_token;
}

void
xamarin_initialize_embedded ()
{
	static bool initialized = false;
	if (initialized)
		return;
	initialized = true;

	char *argv[] = { NULL };
	char *libname = NULL;

	Dl_info info;
	if (dladdr ((void *) xamarin_initialize_embedded, &info) != 0) {
		const char *last_sep = strrchr (info.dli_fname, '/');
		if (last_sep == NULL) {
			libname = strdup (info.dli_fname);
		} else {
			libname = strdup (last_sep + 1);
		}
		argv [0] = libname;
	}

	if (argv [0] == NULL)
		argv [0] = (char *) "embedded";

	xamarin_main (1, argv, XamarinLaunchModeEmbedded);

	if (libname != NULL)
		free (libname);
}

/* Installs g_print/g_error handlers that will redirect output to the system Console */
void
xamarin_install_log_callbacks ()
{
	mono_trace_set_log_handler (log_callback, NULL);
	mono_trace_set_print_handler (print_callback);
	mono_trace_set_printerr_handler (print_callback);
}

void
xamarin_initialize ()
{
	// COOP: accessing managed memory: UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;
	
	MonoAssembly *assembly = NULL;
	MonoMethod *runtime_initialize;
	void* params[2];
	const char *product_dll = NULL;
	guint32 exception_gchandle = 0;
	MonoObject *exc = NULL;

	initialize_started = TRUE;

#ifdef DYNAMIC_MONO_RUNTIME
	// We might be called from the managed Runtime.EnsureInitialized method,
	// in which case xamarin_initialize_dynamic_runtime has not been called yet.
	xamarin_initialize_dynamic_runtime (NULL);
#endif

	xamarin_insert_dllmap ();

	MONO_ENTER_GC_UNSAFE;

	xamarin_install_log_callbacks ();

#if MONOMAC
	detect_product_assembly ();
#endif

	MonoGCFinalizerCallbacks gc_callbacks;
	gc_callbacks.version = MONO_GC_FINALIZER_EXTENSION_VERSION;
	gc_callbacks.is_class_finalization_aware = is_class_finalization_aware;
	gc_callbacks.object_queued_for_finalization = object_queued_for_finalization;
	mono_gc_register_finalizer_callbacks (&gc_callbacks);

	if (xamarin_is_gc_coop) {
		// There should be no such thing as an unhandled ObjC exception
		// when running the GC in cooperative mode, and if we run into it,
		// it's a bug somewhere, in which case we must fix it.
	} else {
		NSSetUncaughtExceptionHandler (exception_handler);
	}

	product_dll = xamarin_use_new_assemblies ? PRODUCT_DUAL_ASSEMBLY : PRODUCT_COMPAT_ASSEMBLY;

	assembly = xamarin_open_assembly (product_dll);

	if (!assembly)
		xamarin_assertion_message ("Failed to load %s.", product_dll);
	platform_image = mono_assembly_get_image (assembly);

	const char *objcruntime = xamarin_use_new_assemblies ? "ObjCRuntime" : PRODUCT_COMPAT_NAMESPACE ".ObjCRuntime";
	const char *foundation = xamarin_use_new_assemblies ? "Foundation" : PRODUCT_COMPAT_NAMESPACE ".Foundation";

	runtime_class = get_class_from_name (platform_image, objcruntime, "Runtime");
	inativeobject_class = get_class_from_name (platform_image, objcruntime, "INativeObject");
	nsobject_class = get_class_from_name (platform_image, foundation, "NSObject");
	nsnumber_class = get_class_from_name (platform_image, foundation, "NSNumber", true);
	nsvalue_class = get_class_from_name (platform_image, foundation, "NSValue", true);
	nsstring_class = get_class_from_name (platform_image, foundation, "NSString", true);

	xamarin_add_internal_call (xamarin_use_new_assemblies ? "Foundation.NSObject::xamarin_release_managed_ref" : PRODUCT_COMPAT_NAMESPACE ".Foundation.NSObject::xamarin_release_managed_ref", (const void *) xamarin_release_managed_ref);
	xamarin_add_internal_call (xamarin_use_new_assemblies ? "Foundation.NSObject::xamarin_create_managed_ref" : PRODUCT_COMPAT_NAMESPACE ".Foundation.NSObject::xamarin_create_managed_ref", (const void *) xamarin_create_managed_ref);

	runtime_initialize = mono_class_get_method_from_name (runtime_class, "Initialize", 1);

	options.size = sizeof (options);
#if MONOTOUCH && (defined(__i386__) || defined (__x86_64__))
	options.flags = (enum InitializationFlags) (options.flags | InitializationFlagsIsSimulator);
#endif

	options.Delegates = &delegates;
	options.Trampolines = &trampolines;
	options.MarshalObjectiveCExceptionMode = xamarin_marshal_objectivec_exception_mode;
	options.MarshalManagedExceptionMode = xamarin_marshal_managed_exception_mode;
#if MONOMAC
	options.LaunchMode = xamarin_launch_mode;
	options.EntryAssemblyPath = xamarin_entry_assembly_path;
#endif

	params [0] = &options;

	mono_runtime_invoke (runtime_initialize, NULL, params, &exc);

	if (exc)
		xamarin_process_managed_exception (exc);

	if (!register_assembly (assembly, &exception_gchandle))
		xamarin_process_managed_exception_gchandle (exception_gchandle);

	xamarin_install_mono_profiler (); // must be called before xamarin_install_nsautoreleasepool_hooks or gc_enable_new_refcount

	xamarin_install_nsautoreleasepool_hooks ();

#if defined (DEBUG)
	if (xamarin_gc_pump) {
		pthread_t gc_thread;
		pthread_create (&gc_thread, NULL, pump_gc, NULL);
	}
#endif

	gc_enable_new_refcount ();

	MONO_EXIT_GC_UNSAFE;
}

static char *x_bundle_path = NULL;
const char *
xamarin_get_bundle_path ()
{
	// COOP: only called at startup, so I believe the mode doesn't matter
	if (x_bundle_path != NULL)
		return x_bundle_path;

	NSBundle *main_bundle = [NSBundle mainBundle];
	NSString *bundle_path;
	char *result;

#if MONOMAC
	if (xamarin_launch_mode == XamarinLaunchModeEmbedded) {
		bundle_path = [[[NSBundle bundleForClass: [XamarinAssociatedObject class]] bundlePath] stringByAppendingPathComponent: @"Versions/Current"];
	} else {
		bundle_path = [[main_bundle bundlePath] stringByAppendingPathComponent:@"Contents"];
	}
	bundle_path = [bundle_path stringByAppendingPathComponent: xamarin_custom_bundle_name == NULL ? @"MonoBundle" : xamarin_custom_bundle_name];
#else
	bundle_path = [main_bundle bundlePath];
#endif

	if (main_bundle == NULL)
		xamarin_assertion_message ("Could not find the main bundle in the app ([NSBundle mainBundle] returned nil)");

	result = mono_path_resolve_symlinks ([bundle_path UTF8String]);

	x_bundle_path = strdup (result);
	mono_free (result);

	return x_bundle_path;
}

void
xamarin_set_bundle_path (const char *path)
{
	// COOP: no managed memory access: any mode
	free (x_bundle_path);
	x_bundle_path = strdup (path);
}

void *
xamarin_calloc (size_t size)
{
	// COOP: no managed memory access: any mode
	return calloc (size, 1);
}

void
xamarin_free (void *ptr)
{
	// COOP: no managed memory access: any mode
	// We use this method to free memory returned by mono,
	// which means we have to use the free function mono expects.
	if (ptr)
		free (ptr);
}

char *
xamarin_strdup_printf (const char *msg, ...)
{
	// COOP: no managed memory access: any mode
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
	// COOP: no managed memory access: any mode.
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
	// COOP: no managed memory access: any mode
	switch (type [0]) {
		case _C_ID:
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

int
xamarin_objc_type_size (const char *type)
{
	const char *original_type = type;

	// COOP: no managed memory access: any mode
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
			int bits = 0;
			int bc = 1;
			while (type [bc] >= '0' && type [bc] <= '9') {
				bits = bits * 10 + (type [bc] - '0');
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
			int size = 0;
			int len = atoi (type+1);
			do {
				type++;
			} while (isdigit (*type));

			size = xamarin_objc_type_size (type);

			size = (size + (sizeof (void *) - 1)) & ~((sizeof (void *) - 1));

			return len * size;
		}
		case _C_UNION_B: {
			int size = 0;

			do {
				type++;

				if (*type == 0)
					xamarin_assertion_message ("Unsupported union type: %s", original_type);
			} while (*type != '=');

			++type;

			do {
				if (*type == 0)
					xamarin_assertion_message ("Unsupported union type: %s", original_type);

				int tsize = xamarin_objc_type_size (type);
				type = objc_skip_type (type);

				tsize = (tsize + (sizeof (void *) - 1)) & ~((sizeof (void *) - 1));
				size = size > tsize ? size : tsize;
			} while (*type != _C_UNION_E);

			return size;
		}
		case _C_STRUCT_B: {
			int size = 0;

			do {
				type++;

				if (*type == 0)
					xamarin_assertion_message ("Unsupported struct type: %s", original_type);
			} while (*type != '=');

			type++;

			while (*type != _C_STRUCT_E) {
				if (*type == 0)
					xamarin_assertion_message ("Unsupported struct type: %s", original_type);
				int item_size = xamarin_objc_type_size (type);
				
				size += (item_size + (sizeof (void *) - 1)) & ~((sizeof (void *) - 1));

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
void
xamarin_create_gchandle (id self, void *managed_object, int flags, bool force_weak)
{
	// COOP: reads managed memory: unsafe mode
	MONO_ASSERT_GC_UNSAFE;
	
	// force_weak is to avoid calling retainCount unless needed, since some classes (UIWebView in iOS 5)
	// will crash if retainCount is called before init. See bug #9261.
	bool weak = force_weak || ([self retainCount] == 1);
	int gchandle;

	if (weak) {
		gchandle = mono_gchandle_new_weakref ((MonoObject *) managed_object, TRUE);
		flags |= GCHANDLE_WEAK;
	} else {
		gchandle = mono_gchandle_new ((MonoObject *) managed_object, FALSE);
		flags &= ~GCHANDLE_WEAK;
	}
	assert ((gchandle & GCHANDLE_MASK) == 0); // Make sure we don't create too many gchandles...
	set_raw_gchandle (self, gchandle | flags);
#if defined(DEBUG_REF_COUNTING)
	PRINT ("\tGCHandle created for %p: %d (flags: %p) = %d %s\n", self, gchandle, GINT_TO_POINTER (flags), get_raw_gchandle (self), weak ? "weak" : "strong");
#endif
}

void
xamarin_switch_gchandle (id self, bool to_weak)
{
	// COOP: reads managed memory: unsafe mode
	MONO_ASSERT_GC_SAFE_OR_DETACHED;
	
	guint32 exception_gchandle = 0;
	int new_gchandle;
	int old_gchandle;
	int old_gchandle_raw;
	MonoObject *managed_object;
	int flags = MANAGED_REF_BIT;

	old_gchandle_raw = get_raw_gchandle_safe (self);
	old_gchandle = old_gchandle_raw & ~GCHANDLE_MASK;
	if (old_gchandle) {
		bool is_weak = (old_gchandle_raw & GCHANDLE_WEAK) == GCHANDLE_WEAK;
		if (to_weak == is_weak) {
			// we already have the GCHandle we need
#if defined(DEBUG_REF_COUNTING)
			PRINT ("Object %p already has a %s GCHandle = %d\n", self, to_weak ? "weak" : "strong", old_gchandle);
#endif
			return;
		}
	} else {
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

	
	MONO_THREAD_ATTACH; // COOP: will switch to GC_UNSAFE

	if (old_gchandle) {
		managed_object = mono_gchandle_get_target (old_gchandle);
	} else {
		managed_object = xamarin_get_managed_object_for_ptr (self, &exception_gchandle);
	}
	
	if (exception_gchandle == 0) {
		if (to_weak) {
			new_gchandle = mono_gchandle_new_weakref (managed_object, TRUE);
			flags |= GCHANDLE_WEAK;
		} else {
			new_gchandle = mono_gchandle_new (managed_object, FALSE);
		}
		
		if (old_gchandle)
			mono_gchandle_free (old_gchandle);
		
		if (managed_object) {
			// It's possible to not have a managed object if:
			// 1. Objective-C holds a weak reference to the native object (and no other strong references)
			//    - in which case the original (old) gchandle would be a weak one.
			// 2. Managed code does not reference the managed object.
			// 3. The GC ran and collected the managed object, but the main thread has not gotten
			//    around to release the native object yet.
			// If all these conditions hold, then the original gchandle will point to
			// null, because the target would be collected.
			xamarin_set_nsobject_flags (managed_object, xamarin_get_nsobject_flags (managed_object) | NSObjectFlagsHasManagedRef);
		}
		set_raw_gchandle (self, new_gchandle | flags);
	}

	MONO_THREAD_DETACH; // COOP: this will switch to GC_SAFE

#if defined(DEBUG_REF_COUNTING)
	PRINT ("Switched object %p to %s GCHandle = %d\n", self, to_weak ? "weak" : "strong", new_gchandle);
#endif

	xamarin_process_managed_exception_gchandle (exception_gchandle);
}

void
xamarin_free_gchandle (id self, int gchandle)
{
	// COOP: no managed memory access, but calls mono function mono_gc_handle_free. Assuming that function can be called with any mode: this function can be called with any mode as well
	if (gchandle) {
#if defined(DEBUG_REF_COUNTING)
		PRINT ("\tGCHandle %i destroyed for object %p\n", gchandle, self);
#endif
		mono_gchandle_free (gchandle);

		set_raw_gchandle (self, 0);
	} else {
#if defined(DEBUG_REF_COUNTING)
		PRINT ("\tNo GCHandle for the object %p\n", self);
#endif
	}
}

void
xamarin_clear_gchandle (id self)
{
	// COOP: no managed memory access: any mode
	set_raw_gchandle (self, 0);
}

void
xamarin_set_gchandle (id self, int gchandle)
{
	// COOP: no managed memory access: any mode
	set_raw_gchandle (self, gchandle);
}

static int
find_user_type_index (MTClassMap *map, int lo, int hi, Class cls)
{
	if (hi >= lo) {
		int mid = lo + (hi - lo) / 2;

		if (map [mid].handle == cls)
			return mid;

		if ((intptr_t) map [mid].handle > (intptr_t) cls)
			return find_user_type_index (map, lo, mid - 1, cls);

		return find_user_type_index (map, mid + 1, hi, cls);
	}

	return -1;
}

static inline bool
is_user_type (id self)
{
	Class cls = object_getClass (self);

	if (options.RegistrationData != NULL && options.RegistrationData->map_count > 0) {
		MTClassMap *map = options.RegistrationData->map;
		int idx = find_user_type_index (map, 0, options.RegistrationData->map_count - 1, cls);
		if (idx >= 0)
			return (map [idx].flags & MTTypeFlagsUserType) == MTTypeFlagsUserType;
		// If using the partial static registrar, we need to continue
		// If full static registrar, we can return false
		if ((options.flags & InitializationFlagsIsPartialStaticRegistrar) != InitializationFlagsIsPartialStaticRegistrar)
			return false;
	}

	// COOP: no managed memory access: any mode
	return class_getInstanceMethod (cls, @selector (xamarinSetGCHandle:)) != NULL;
}

#if defined(DEBUG_REF_COUNTING)
int
get_safe_retainCount (id self)
{
	// COOP: no managed memory access: any mode
	if ([self isKindOfClass: [NSCalendar class]] ||
		[self isKindOfClass: [NSInputStream class]] ||
		[self isKindOfClass: [NSOutputStream class]]) {
		// NSCalendar/NSInputStream may end up with a stack overflow where CFGetRetainCount calls itself
		return 666;
	} else {
		return [self retainCount];
	}
}
#endif

void
xamarin_release_managed_ref (id self, MonoObject *managed_obj)
{
	// COOP: This is an icall, so at entry we're in unsafe mode.
	// COOP: we stay in unsafe mode (since we write to the managed memory) unless calling a selector (which must be done in safe mode)
	MONO_ASSERT_GC_UNSAFE;
	
	bool user_type = is_user_type (self);
	guint32 exception_gchandle = 0;
	
#if defined(DEBUG_REF_COUNTING)
	PRINT ("monotouch_release_managed_ref (%s Handle=%p) retainCount=%d; HasManagedRef=%i GCHandle=%i IsUserType=%i\n", 
		class_getName (object_getClass (self)), self, (int32_t) [self retainCount], user_type ? xamarin_has_managed_ref (self) : 666, user_type ? get_gchandle (self) : 666, user_type);
#endif

	xamarin_set_nsobject_flags (managed_obj, xamarin_get_nsobject_flags (managed_obj) & ~NSObjectFlagsHasManagedRef);

	if (user_type) {
		/* clear MANAGED_REF_BIT */
		set_raw_gchandle (self, get_raw_gchandle (self) & ~MANAGED_REF_BIT);
		MONO_ENTER_GC_SAFE;
		[self release];
		MONO_EXIT_GC_SAFE;
	} else {
		/* If we're a wrapper type, we need to unregister here, since we won't enter the release trampoline */
		xamarin_unregister_nsobject (self, managed_obj, &exception_gchandle);

		//
		// This lock is needed so that we can safely call retainCount in the
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
		//    This is https://github.com/xamarin/xamarin-macios/issues/3943
		//
		xamarin_framework_peer_lock ();
		xamarin_framework_peer_unlock ();
		MONO_ENTER_GC_SAFE;
		[self release];
		MONO_EXIT_GC_SAFE;
	}

	xamarin_process_managed_exception_gchandle (exception_gchandle);
}

void
xamarin_create_managed_ref (id self, gpointer managed_object, bool retain)
{
	// COOP: This is an icall, so at entry we're in unsafe mode.
	// COOP: we stay in unsafe mode (since we write to the managed memory) unless calling a selector (which must be done in safe mode)
	MONO_ASSERT_GC_UNSAFE;
	
	int gchandle;
	bool user_type = is_user_type (self);
	
#if defined(DEBUG_REF_COUNTING)
	PRINT ("monotouch_create_managed_ref (%s Handle=%p) retainCount=%d; HasManagedRef=%i GCHandle=%i IsUserType=%i\n", 
		class_getName ([self class]), self, get_safe_retainCount (self), user_type ? xamarin_has_managed_ref (self) : 666, user_type ? get_gchandle (self) : 666, user_type);
#endif
	
	xamarin_set_nsobject_flags ((MonoObject *) managed_object, xamarin_get_nsobject_flags ((MonoObject *) managed_object) | NSObjectFlagsHasManagedRef);

	if (user_type) {
		gchandle = get_gchandle (self);
		if (!gchandle) {
			xamarin_create_gchandle (self, managed_object, MANAGED_REF_BIT, !retain);
		} else {
#if defined(DEBUG_REF_COUNTING)
			xamarin_assertion_message ("GCHandle already exists for %p: %d\n", self, gchandle);
#endif
		}
	}

	if (retain) {
		MONO_ENTER_GC_SAFE;
		[self retain];
		MONO_EXIT_GC_SAFE;
	}
	mt_dummy_use (managed_object);
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
static MonoReferenceQueue *block_wrapper_queue;
/*
 * Given a MonoMethod and a parameter, lookup the MethodInfo (MonoReflectionMethod)
 * that can be used to create a new delegate, this returns the method that can
 * create the method
 */
static MonoObject *
get_method_block_wrapper_creator (MonoMethod *method, int par, guint32 *exception_gchandle)
{
	// COOP: accesses managed memory: unsafe mode.
	MONO_ASSERT_GC_UNSAFE;
	
	MonoObject *res = NULL;
	MethodAndPar mp, *nmp;
	mp.method = method;
	mp.par = par;

	// PRINT ("Looking up method and par (%x and %d)", (int) method, par);
	MONO_ENTER_GC_SAFE;
	pthread_mutex_lock (&wrapper_hash_lock);
	MONO_EXIT_GC_SAFE;
	
	if (xamarin_wrapper_hash == NULL) {
		xamarin_wrapper_hash = mono_g_hash_table_new_type (method_and_par_hash, method_and_par_compare, MONO_HASH_VALUE_GC);
	}

	res = (MonoObject *) mono_g_hash_table_lookup (xamarin_wrapper_hash, &mp);
	pthread_mutex_unlock (&wrapper_hash_lock);
	if (res != NULL){
		// PRINT ("Found match: %x", (int) res);
		return res;
	}

	res = xamarin_get_block_wrapper_creator ((MonoObject *) mono_method_get_object (mono_domain_get (), method, NULL), par, exception_gchandle);
	if (*exception_gchandle != 0)
		return NULL;
	// PRINT ("New value: %x", (int) res);

	nmp = (MethodAndPar *) malloc (sizeof (MethodAndPar));
	*nmp = mp;

	MONO_ENTER_GC_SAFE;
	pthread_mutex_lock (&wrapper_hash_lock);
	MONO_EXIT_GC_SAFE;
	mono_g_hash_table_insert (xamarin_wrapper_hash, nmp, res);
	pthread_mutex_unlock (&wrapper_hash_lock);
	return res;
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
int *
xamarin_get_delegate_for_block_parameter (MonoMethod *method, guint32 token_ref, int par, void *nativeBlock, guint32 *exception_gchandle)
{
	// COOP: accesses managed memory: unsafe mode.
	MONO_ASSERT_GC_UNSAFE;
	
	MonoObject *delegate;

	if (nativeBlock == NULL)
		return NULL;

	MonoObject *obj;
	if (token_ref != INVALID_TOKEN_REF) {
		obj = (MonoObject *) xamarin_get_method_from_token (token_ref, exception_gchandle);
	} else {
		obj = get_method_block_wrapper_creator (method, par, exception_gchandle);
	}
	if (*exception_gchandle != 0)
		return NULL;

	/* retain or copy (if it's a stack block) the block */
	nativeBlock = _Block_copy (nativeBlock);

	delegate = delegates.create_block_proxy (obj, nativeBlock, exception_gchandle);
	if (*exception_gchandle != 0) {
		_Block_release (nativeBlock);
		return NULL;
	}

	MONO_ENTER_GC_SAFE;
	pthread_mutex_lock (&wrapper_hash_lock);
	MONO_EXIT_GC_SAFE;

	if (block_wrapper_queue == NULL)
		block_wrapper_queue = mono_gc_reference_queue_new (xamarin_release_block_on_main_thread);

	mono_gc_reference_queue_add (block_wrapper_queue, delegate, nativeBlock);
	pthread_mutex_unlock (&wrapper_hash_lock);

	return (int *) delegate;
}

id
xamarin_get_block_for_delegate (MonoMethod *method, MonoObject *delegate, const char *signature, guint32 token_ref, guint32 *exception_gchandle)
{
	// COOP: accesses managed memory: unsafe mode.
	return delegates.create_delegate_proxy ((MonoObject *) mono_method_get_object (mono_domain_get (), method, NULL), delegate, signature, token_ref, exception_gchandle);
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

#if MONOMAC
void
xamarin_set_is_unified (bool value)
{
	// COOP: no managed memory access: any mode.
	if (initialize_started)
		xamarin_assertion_message ("Fatal error: xamarin_set_is_unified called after xamarin_initialize.\n");

	xamarin_use_new_assemblies = value;
	xamarin_detect_unified_build = false;
}
#endif

bool
xamarin_get_is_unified ()
{
	// COOP: no managed memory access: any mode.
	return xamarin_use_new_assemblies;
}

void
xamarin_set_gc_pump_enabled (bool value)
{
#if DEBUG
	// COOP: no managed memory access: any mode.
	xamarin_gc_pump = value;
#endif
}

const char *
xamarin_skip_encoding_flags (const char *encoding)
{
	// COOP: no managed memory access: any mode.
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

void
xamarin_process_nsexception (NSException *ns_exception)
{
	xamarin_process_nsexception_using_mode (ns_exception, false);
}

void
xamarin_process_nsexception_using_mode (NSException *ns_exception, bool throwManagedAsDefault)
{
	XamarinGCHandle *exc_handle;
	guint32 exception_gchandle = 0;
	MarshalObjectiveCExceptionMode mode;

	mode = xamarin_on_marshal_objectivec_exception (ns_exception, throwManagedAsDefault, &exception_gchandle);

	if (exception_gchandle != 0) {
		PRINT (PRODUCT ": Got an exception while executing the MarshalObjectiveCException event (this exception will be ignored):");
		PRINT ("%@", print_all_exceptions (mono_gchandle_get_target (exception_gchandle)));
		mono_gchandle_free (exception_gchandle);
		exception_gchandle = 0;
	}

	if (mode == MarshalObjectiveCExceptionModeDefault)
		mode = xamarin_is_gc_coop ? MarshalObjectiveCExceptionModeThrowManagedException : MarshalObjectiveCExceptionModeUnwindManagedCode;
	
	switch (mode) {
	case MarshalObjectiveCExceptionModeUnwindManagedCode:
		if (xamarin_is_gc_coop)
			xamarin_assertion_message ("Cannot unwind managed frames for Objective-C exceptions when the GC is in cooperative mode.");
		@throw ns_exception;
		break;
	case MarshalObjectiveCExceptionModeThrowManagedException:
		exc_handle = [[ns_exception userInfo] objectForKey: @"XamarinManagedExceptionHandle"];
		if (exc_handle != NULL) {
			int handle = [exc_handle getHandle];
			MonoObject *exc = mono_gchandle_get_target (handle);
			mono_runtime_set_pending_exception ((MonoException *) exc, false);
		} else {
			int handle = xamarin_create_ns_exception (ns_exception, &exception_gchandle);
			if (exception_gchandle != 0) {
				PRINT (PRODUCT ": Got an exception while creating a managed NSException wrapper (will throw this exception instead):");
				PRINT ("%@", print_all_exceptions (mono_gchandle_get_target (exception_gchandle)));
				handle = exception_gchandle;
				exception_gchandle = 0;
			}
			MONO_ENTER_GC_UNSAFE;
			MonoObject *exc = mono_gchandle_get_target (handle);
			mono_runtime_set_pending_exception ((MonoException *) exc, false);
			mono_gchandle_free (handle);
			MONO_EXIT_GC_UNSAFE;
		}
		break;
	case MarshalObjectiveCExceptionModeAbort:
	default:
		xamarin_assertion_message ("Aborting due to unhandled Objective-C exception:\n%s\n", [[ns_exception description] UTF8String]);
		break;
	}
}

void
xamarin_process_managed_exception (MonoObject *exception)
{
	if (exception == NULL)
		return;

	MarshalManagedExceptionMode mode;
	guint32 exception_gchandle = 0;

	int handle = mono_gchandle_new (exception, false);
	mode = xamarin_on_marshal_managed_exception (handle, &exception_gchandle);
	mono_gchandle_free (handle);

	if (exception_gchandle != 0) {
		PRINT (PRODUCT ": Got an exception while executing the MarshalManagedException event (this exception will be ignored):");
		PRINT ("%@", print_all_exceptions (mono_gchandle_get_target (exception_gchandle)));
		mono_gchandle_free (exception_gchandle);
		exception_gchandle = 0;
		mode = MarshalManagedExceptionModeDefault;
	}

	if (mode == MarshalManagedExceptionModeDefault)
		mode = xamarin_is_gc_coop ? MarshalManagedExceptionModeThrowObjectiveCException : MarshalManagedExceptionModeUnwindNativeCode;

	switch (mode) {
	case MarshalManagedExceptionModeDisable:
	case MarshalManagedExceptionModeUnwindNativeCode:
		if (xamarin_is_gc_coop)
			xamarin_assertion_message ("Cannot unwind native frames for managed exceptions when the GC is in cooperative mode.");

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

		handle = mono_gchandle_new (exception, false);
		xamarin_rethrow_managed_exception (handle, &exception_gchandle);
		mono_gchandle_free (handle);

		if (exception_gchandle == 0) {
			PRINT (PRODUCT ": Did not get a rethrow exception, will throw the original exception. The original stack trace will be lost.");
		} else {
			exception = mono_gchandle_get_target (exception_gchandle);
			mono_gchandle_free (exception_gchandle);
		}

		mono_raise_exception ((MonoException *) exception);

		break;
	case MarshalManagedExceptionModeThrowObjectiveCException: {
		int handle = mono_gchandle_new (exception, false);
		NSException *ns_exc = xamarin_unwrap_ns_exception (handle, &exception_gchandle);
		
		if (exception_gchandle != 0) {
			PRINT (PRODUCT ": Got an exception while unwrapping a managed NSException wrapper (this exception will be ignored):");
			PRINT ("%@", print_all_exceptions (mono_gchandle_get_target (exception_gchandle)));
			mono_gchandle_free (exception_gchandle);
			exception_gchandle = 0;
			ns_exc = NULL;
		}

		if (ns_exc != NULL) {
			mono_gchandle_free (handle);
			@throw ns_exc;
		} else {
			// Strangely enough the thread might be detached, if xamarin_process_managed_exception was called from
			// xamarin_ftnptr_exception_handler for an exception that occurred in a reverse delegate that
			// was called on a detached thread, since in that case the native-to-managed wrapper will have
			// returned the thread to a detached state after calling the managed function.
			NSString *name;
			NSString *reason;
			NSDictionary *userInfo;
			const char *fullname;
			MONO_THREAD_ATTACH; // COOP: will switch to GC_UNSAFE
			
			fullname = xamarin_type_get_full_name (mono_class_get_type (mono_object_get_class (exception)), &exception_gchandle);
			if (exception_gchandle != 0) {
				PRINT (PRODUCT ": Got an exception when trying to get the typename for an exception (this exception will be ignored):");
				PRINT ("%@", print_all_exceptions (mono_gchandle_get_target (exception_gchandle)));
				mono_gchandle_free (exception_gchandle);
				exception_gchandle = 0;
				fullname = "Unknown";
			}

			name = [NSString stringWithUTF8String: fullname];

			char *message = fetch_exception_property_string (exception, "get_Message", true);
			reason = [NSString stringWithUTF8String: message];
			mono_free (message);
			userInfo = [NSDictionary dictionaryWithObject: [XamarinGCHandle createWithHandle: handle] forKey: @"XamarinManagedExceptionHandle"];
			
			MONO_THREAD_DETACH; // COOP: this will switch to GC_SAFE
			
			@throw [[NSException alloc] initWithName: name reason: reason userInfo: userInfo];
		}
		break;
	}
	case MarshalManagedExceptionModeAbort:
	default:
		xamarin_assertion_message ("Aborting due to trying to marshal managed exception:\n%s\n", [print_all_exceptions (exception) UTF8String]);
		break;
	}
}

void
xamarin_throw_product_exception (int code, const char *message)
{
	xamarin_process_managed_exception_gchandle (xamarin_create_product_exception (code, message));
}

guint32
xamarin_create_product_exception (int code, const char *message)
{
	guint32 exception_gchandle = 0;
	guint32 handle = xamarin_create_product_exception_for_error (code, message, &exception_gchandle);
	if (exception_gchandle != 0)
		return exception_gchandle;
	return handle;
}

void
xamarin_insert_dllmap ()
{
#if defined (OBJC_ZEROCOST_EXCEPTIONS) && (defined (__i386__) || defined (__x86_64__))
	if (xamarin_marshal_objectivec_exception_mode == MarshalObjectiveCExceptionModeDisable)
		return;
#if DYLIB
	const char *lib = "libxammac.dylib";
#else
	const char *lib = "__Internal";
#endif
	mono_dllmap_insert (NULL, "/usr/lib/libobjc.dylib", "objc_msgSend",            lib, "xamarin_dyn_objc_msgSend");
	mono_dllmap_insert (NULL, "/usr/lib/libobjc.dylib", "objc_msgSendSuper",       lib, "xamarin_dyn_objc_msgSendSuper");
	mono_dllmap_insert (NULL, "/usr/lib/libobjc.dylib", "objc_msgSend_stret",      lib, "xamarin_dyn_objc_msgSend_stret");
	mono_dllmap_insert (NULL, "/usr/lib/libobjc.dylib", "objc_msgSendSuper_stret", lib, "xamarin_dyn_objc_msgSendSuper_stret");
	LOG (PRODUCT ": Added dllmap for objc_msgSend");
#endif // defined (__i386__) || defined (__x86_64__)
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
	
#if TARGET_OS_WATCH && defined (__arm__) // maybe make this configurable somehow?
	const char *msg = [message UTF8String];
	int len = strlen (msg);
	fwrite (msg, 1, len, stdout);
	if (len == 0 || msg [len - 1] != '\n')
		fwrite ("\n", 1, 1, stdout);
	fflush (stdout);
#else
	NSLog (@"%@", message);	
#endif

	[message release];
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
 * The platform assembly (Xamarin.[iOS|TVOS|WatchOS].dll) and any assemblies
 * the platform assembly references (mscorlib.dll, System.dll) may be in a
 * pointer-size subdirectory (ARCH_SUBDIR).
 * 
 * AOT data files will have an arch-specific infix.
 */

void
xamarin_get_assembly_name_without_extension (const char *aname, char *name, int namelen)
{
	int len = strlen (aname);
	strlcpy (name, aname, namelen);
	if (namelen <= 4 || len <= 4)
		return;
	const char *ext = name + (len - 4);
	if (!strncmp (".exe", ext, 4) || !strncmp (".dll", ext, 4))
		name [len - 4] = 0; // strip off any extensions.
}

static bool
xamarin_locate_assembly_resource_for_root (const char *root, const char *culture, const char *resource, char *path, int pathlen)
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

	// just the file, no extensions, etc.
	if (snprintf (path, pathlen, "%s/%s", root, resource) < 0) {
		LOG (PRODUCT ": Failed to construct path for resource: %s (6): %s", resource, strerror (errno));
		return false;
	} else if (xamarin_file_exists (path)) {
		return true;
	}

	return false;
}

bool
xamarin_locate_assembly_resource_for_name (MonoAssemblyName *assembly_name, const char *resource, char *path, int pathlen)
{
	const char *culture = mono_assembly_name_get_culture (assembly_name);
	const char *aname = mono_assembly_name_get_name (assembly_name);
	return xamarin_locate_assembly_resource (aname, culture, resource, path, pathlen);
}

// #define LOG_RESOURCELOOKUP(...) do { NSLog (@ __VA_ARGS__); } while (0);
#define LOG_RESOURCELOOKUP(...)


bool
xamarin_locate_assembly_resource (const char *assembly_name, const char *culture, const char *resource, char *path, int pathlen)
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

#if !MONOMAC && (defined(__i386__) || defined (__x86_64__))
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
xamarin_get_managed_method_for_token (guint32 token_ref, guint32 *exception_gchandle)
{
	MonoReflectionMethod *reflection_method;

	reflection_method = xamarin_get_method_from_token (token_ref, exception_gchandle);
	if (*exception_gchandle != 0) return NULL;

	return xamarin_get_reflection_method_method (reflection_method);
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
	// COOP: no managed memory access: any mode.
	xamarin_notify_dealloc (native_object, gc_handle & ~GCHANDLE_MASK);
	native_object = NULL;
	gc_handle = 0;
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
	// COOP: no managed memory access: any mode.
	xamarin_notify_dealloc (native_object, gc_handle & ~GCHANDLE_MASK);
	native_object = NULL;
	gc_handle = 0;
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
 * Do not add a xamarinSetGCHandle: method, since we use the presence
 * of it to detect whether a particular type is a user type or not
 * (see is_user_type).
 */

@implementation NSObject (NonXamarinObject)
-(int) xamarinGetGCHandle
{
	// COOP: no managed memory access: any mode.
	return 0;
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

bool
xamarin_is_managed_exception_marshaling_disabled ()
{
#if DEBUG
	if (xamarin_is_gc_coop)
		return false;

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

/*
 * XamarinGCHandle
 */
@implementation XamarinGCHandle
+(XamarinGCHandle *) createWithHandle: (int) h
{
	XamarinGCHandle *rv = [[XamarinGCHandle alloc] init];
	rv->handle = h;
	[rv autorelease];
	return rv;
}

-(void) dealloc
{
	mono_gchandle_free (handle);
	[super dealloc];
}

-(int) getHandle
{
	return handle;
}
@end
