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
bool xamarin_use_new_assemblies = false;
#if MONOTOUCH && DEBUG && (defined (__i386__) || defined (__x86_64__))
bool xamarin_gc_pump = true;
#else
bool xamarin_gc_pump = false;
#endif
#if MONOMAC
// FIXME: implement release mode for monomac.
bool xamarin_debug_mode = true;
#else
bool xamarin_debug_mode = false;
#endif
// true if either OldDynamic or OldStatic (since the static registrar still needs
// a dynamic registrar available too).
bool xamarin_use_old_dynamic_registrar = false;
bool xamarin_use_il_registrar = false;
#if DEBUG
bool xamarin_init_mono_debug = true;
#else
bool xamarin_init_mono_debug = false;
#endif
#if DEBUG && (defined (__i386__) || defined (__x86_64__))
bool xamarin_compact_seq_points = false;
#else
bool xamarin_compact_seq_points = true;
#endif
int xamarin_log_level = 0;
const char *xamarin_executable_name = NULL;
#if MONOMAC
NSString * xamarin_custom_bundle_name = nil;
bool xamarin_is_mkbundle = false;
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

/* Callbacks */

xamarin_setup_callback xamarin_setup = NULL;
xamarin_register_module_callback xamarin_register_modules = NULL;
xamarin_register_assemblies_callback xamarin_register_assemblies = NULL;
xamarin_extension_main_callback xamarin_extension_main = NULL;

/* Local variable */

typedef struct  {
	struct MTRegistrationMap   *map;
	int total_count; // SUM (registration_map->map_count)
} RegistrationData;

static RegistrationData registration_data;

static MonoClass      *inativeobject_class;
static MonoClass      *nsobject_class;

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
	/* unused									= 0x01,*/
	InitializationFlagsUseOldDynamicRegistrar	= 0x02,
	InitializationFlagsDynamicRegistrar			= 0x04,
	InitializationFlagsILRegistrar				= 0x08,
	InitializationFlagsIsSimulator				= 0x10,
};

struct InitializationOptions {
	int size; // the size of this structure. This is used for version checking.
	enum InitializationFlags flags;
	struct Delegates Delegates;
	struct Trampolines Trampolines;
	RegistrationData* RegistrationData;
	enum MarshalObjectiveCExceptionMode MarshalObjectiveCExceptionMode;
	enum MarshalManagedExceptionMode MarshalManagedExceptionMode;
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

struct Managed_NSObject {
	MonoObject obj;
	id handle;
	void *class_handle;
	uint8_t flags;
};

id
xamarin_get_nsobject_handle (MonoObject *obj)
{
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	return mobj->handle;
}

void
xamarin_set_nsobject_handle (MonoObject *obj, id handle)
{
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	mobj->handle  = handle;
}

uint8_t
xamarin_get_nsobject_flags (MonoObject *obj)
{
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	return mobj->flags;
}

void
xamarin_set_nsobject_flags (MonoObject *obj, uint8_t flags)
{
	struct Managed_NSObject *mobj = (struct Managed_NSObject *) obj;
	mobj->flags = flags;
}

MonoType *
xamarin_get_parameter_type (MonoMethod *managed_method, int index)
{
	MonoMethodSignature *msig = mono_method_signature (managed_method);
	void *iter = NULL;
	MonoType *p = NULL;
	
	for (int i = 0; i < index + 1; i++)
		p = mono_signature_get_params (msig, &iter);
	
	return p;
}

MonoObject *
xamarin_get_nsobject_with_type_for_ptr (id self, bool owns, MonoType* type)
{
	int32_t created;
	return xamarin_get_nsobject_with_type_for_ptr_created (self, owns, type, &created);
}

MonoObject *
xamarin_get_nsobject_with_type_for_ptr_created (id self, bool owns, MonoType *type, int32_t *created)
{
	MonoObject *mobj = NULL;
	uint32_t gchandle = 0;

	*created = false;

	if (self == NULL)
		return NULL;
	
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);

	gchandle = xamarin_get_gchandle (self);

	if (gchandle != 0) {
		mobj = mono_gchandle_get_target (gchandle);
		if (mono_object_isinst (mobj, mono_class_from_mono_type (type)) != NULL)
			return mobj;
	}

	return xamarin_get_nsobject_with_type (self, mono_type_get_object (mono_domain_get (), type), created);
}

MonoObject *
xamarin_get_managed_object_for_ptr (id self)
{
	MonoObject *mobj = NULL;
	uint32_t gchandle = 0;

	if (self == NULL)
		return NULL;

	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);

	gchandle = xamarin_get_gchandle (self);

	if (gchandle == 0) {
		mobj = (MonoObject *) xamarin_try_get_or_construct_nsobject (self);
	} else {
		mobj = mono_gchandle_get_target (gchandle);
	}

	return mobj;
}

MonoObject *
xamarin_get_managed_object_for_ptr_fast (id self)
{
	MonoObject *mobj = NULL;
	uint32_t gchandle = 0;

	gchandle = xamarin_get_gchandle (self);

	if (gchandle == 0) {
		mobj = (MonoObject *) xamarin_try_get_or_construct_nsobject (self);
	} else {
		mobj = mono_gchandle_get_target (gchandle);
#if DEBUG
		if (self != xamarin_get_nsobject_handle (mobj)) {
			xamarin_assertion_message ("Internal consistency error, please file a bug (http://bugzilla.xamarin.com). Additional data: found managed object %p=%p (%s) in native object %p (%s).\n",
				mobj, xamarin_get_nsobject_handle (mobj), xamarin_class_get_full_name (mono_object_get_class (mobj)), self, object_getClassName (self));
		}
#endif
	}

	return mobj;
}

void xamarin_framework_peer_lock ()
{
	pthread_mutex_lock (&framework_peer_release_lock);
}

void xamarin_framework_peer_unlock ()
{
	pthread_mutex_unlock (&framework_peer_release_lock);
}

bool
xamarin_is_class_nsobject (MonoClass *cls)
{
	return mono_class_is_subclass_of (cls, nsobject_class, false);
}

bool
xamarin_is_class_inativeobject (MonoClass *cls)
{
	return mono_class_is_subclass_of (cls, inativeobject_class, true);
}

bool
xamarin_is_class_array (MonoClass *cls)
{
	// return cls->type == MONO_TYPE_SZARRAY;
	return mono_class_is_subclass_of (cls, mono_get_array_class (), false);
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
get_raw_gchandle (id self)
{
	id<XamarinExtendedObject> xself = self;
	return (int) [xself xamarinGetGCHandle];
}

static inline void
set_raw_gchandle (id self, int gc_handle)
{
	id<XamarinExtendedObject> xself = self;
	[xself xamarinSetGCHandle: gc_handle];
}

static inline int
get_gchandle (id self)
{
	return get_raw_gchandle (self) & ~GCHANDLE_MASK;
}

int
xamarin_get_gchandle (id self)
{
	return get_gchandle (self);
}

int
xamarin_get_gchandle_with_flags (id self)
{
	return get_raw_gchandle (self);
}

bool
xamarin_has_managed_ref (id self)
{
	return get_raw_gchandle (self) & MANAGED_REF_BIT;
}

MonoException *
xamarin_create_exception (const char *msg)
{
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
	PublicMonoReflectionMethod *rm = (PublicMonoReflectionMethod *) method;
	return rm->method;
}

id
xamarin_get_handle (MonoObject *obj)
{
	MonoClass *klass;
	id rv = nil;

	if (obj == NULL)
		return nil;

	klass = mono_object_get_class (obj);

	if (xamarin_is_class_nsobject (klass)) {
		rv = xamarin_get_nsobject_handle (obj);
	} else if (xamarin_is_class_inativeobject (klass)) {
		rv = xamarin_get_handle_for_inativeobject (obj);
	} else {
		char *msg = xamarin_strdup_printf ("Unable to marshal from %s.%s to an Objective-C object. "
									"The managed class must either inherit from NSObject or implement INativeObject.",
									mono_class_get_namespace (klass), mono_class_get_name (klass));
		MonoException *exc = mono_get_exception_execution_engine (msg);
		xamarin_free (msg);
		mono_raise_exception (exc);
	}
	
	return rv;
}

#if DEBUG
static void 
verify_cast (MonoClass *to, MonoObject *obj, Class from_class, SEL sel, MonoMethod *method)
{
	if (!to)
		return;

	if (mono_object_isinst (obj, to) == NULL) {
		MonoClass *from = mono_object_get_class (obj);
		char *method_full_name = mono_method_full_name (method, TRUE);
		char *from_name = xamarin_class_get_full_name (from);
		char *to_name = xamarin_class_get_full_name (to);
		char *msg = xamarin_strdup_printf ("Unable to cast object of type '%s' (Objective-C type: '%s') to type '%s'.\n"
		"Additional information:\n"
		"\tSelector: %s\n"
		"\tMethod: %s\n", from_name, class_getName(from_class), to_name, sel_getName (sel), method_full_name);
		MonoException *mono_ex = mono_exception_from_name_msg (mono_get_corlib (), "System", "InvalidCastException", msg);
		mono_free (from_name);
		mono_free (to_name);
		xamarin_free (msg);
		mono_free (method_full_name);
		mono_raise_exception (mono_ex);
	}
}
#endif

void
xamarin_check_for_gced_object (MonoObject *obj, SEL sel, id self, MonoMethod *method)
{
	if (obj != NULL) {
#if DEBUG
		verify_cast (mono_method_get_class (method), obj, [self class], sel, method);
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
	char *type_name = xamarin_lookup_managed_type_name ([self class]);
	char *msg = xamarin_strdup_printf (m, self, object_getClassName (self), type_name, sel_getName (sel), method_full_name);
	MonoException *mex = xamarin_create_exception (msg);
	xamarin_free (msg);
	mono_free (type_name);
	mono_free (method_full_name);
	mono_raise_exception (mex);
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
//	MonoException *mono_ex = mono_exception_from_name_msg (mono_get_corlib (), "System", "InvalidCastException", msg);
//	xamarin_free (msg);
//	mono_free (method_full_name);
//	mono_raise_exception (mono_ex);
}
#endif

char *
xamarin_class_get_full_name (MonoClass *klass)
{
	return xamarin_type_get_full_name (mono_class_get_type (klass));
}

char *
xamarin_type_get_full_name (MonoType *type)
{
	return xamarin_reflection_type_get_full_name (mono_type_get_object (mono_domain_get (), type));
}

/*
 * ToggleRef support
 */
// #define DEBUG_TOGGLEREF 1
static void
gc_register_toggleref (MonoObject *obj, id self, bool isCustomType)
{
#ifdef DEBUG_TOGGLEREF
	id handle = xamarin_get_nsobject_handle (obj);

	NSLog (@"**Registering object %p handle %p RC %d flags: %i",
		obj,
		handle,
		(int) (handle ? [handle retainCount] : 0),
		xamarin_get_nsobject_flags (obj));
#endif
	mono_gc_toggleref_add (obj, TRUE);

	// Make sure the GCHandle we have is a weak one for custom types.
	if (isCustomType)
		xamarin_switch_gchandle (self, true);
}

static MonoToggleRefStatus
gc_toggleref_callback (MonoObject *object)
{
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
	NSLog (@"\tinspecting %p handle:%p %s flags: %i RC %d -> %s\n", object, handle, cn, (int) flags, (int) (handle ? [handle retainCount] : 0), rv);
#endif

	return res;
}

typedef struct {
	int dummy;
} NRCProfiler;

static void
gc_event_callback (MonoProfiler *prof, MonoGCEvent event, int generation)
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

static void
gc_enable_new_refcount (void)
{
	pthread_mutexattr_t attr;
	pthread_mutexattr_init (&attr);
	pthread_mutexattr_settype (&attr, PTHREAD_MUTEX_RECURSIVE);
	pthread_mutex_init (&framework_peer_release_lock, &attr);
	pthread_mutexattr_destroy (&attr);

	NRCProfiler *prof = (NRCProfiler *) malloc (sizeof (NRCProfiler));

	mono_gc_toggleref_register_callback (gc_toggleref_callback);

	mono_add_internal_call (xamarin_use_new_assemblies ? "Foundation.NSObject::RegisterToggleRef" : PRODUCT_COMPAT_NAMESPACE ".Foundation.NSObject::RegisterToggleRef", (const void *) gc_register_toggleref);
	mono_profiler_install ((MonoProfiler *) prof, NULL);
	mono_profiler_install_gc (gc_event_callback, NULL);
}

static MonoClass *
get_class_from_name (MonoImage* image, const char *nmspace, const char *name)
{
	MonoClass *rv = mono_class_from_name (image, nmspace, name);
	if (!rv)
		xamarin_assertion_message ("Fatal error: failed to load the class '%s.%s'\n.", nmspace, name);
	return rv;
}

bool
xamarin_file_exists (const char *path)
{
	struct stat buffer;
	return stat (path, &buffer) == 0;
}

static MonoAssembly *
open_assembly (const char *name)
{
	char path [1024];
	MonoAssembly *assembly;
	bool exists = false;

#if MONOMAC
	if (xamarin_get_is_mkbundle ()) {
		assembly = mono_assembly_open (name, NULL);
		if (assembly == NULL) {
			NSLog (@ PRODUCT ": Could not find the required assembly '%s' in the app. This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: http://bugzilla.xamarin.com", name);
			exit (1);
		}
		return assembly;
	}
#endif

	if (xamarin_use_new_assemblies) {
		snprintf (path, sizeof (path), "%s/" ARCH_SUBDIR "/%s", xamarin_get_bundle_path (), name);
		exists = xamarin_file_exists (path);
	}
	if (!exists)
		snprintf (path, sizeof (path), "%s/%s", xamarin_get_bundle_path (), name);
	
#if MONOMAC && DYLIB
	if (!xamarin_file_exists (path)) {
		// Check if we already have the assembly in memory
		char path2 [1024];
		snprintf (path2, sizeof (path2), "%s", name);
		// strip off the extension
		char *dot = strrchr (path2, '.');
		if (strncmp (dot, ".dll", 4) == 0)
			*dot = 0;
		MonoAssemblyName *aname = mono_assembly_name_new (path2);
		assembly = mono_assembly_loaded (aname);
		mono_assembly_name_free (aname);
		if (assembly)
			return assembly;
		
		NSLog (@ PRODUCT ": Could not find the assembly '%s' in the app nor as an already loaded assembly. This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: http://bugzilla.xamarin.com", name);
		exit (1);
	}
#endif

	if (!xamarin_file_exists (path)) {
		NSLog (@ PRODUCT ": Could not find the assembly '%s' in the app. This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: http://bugzilla.xamarin.com", name);
		exit (1);
	}

	assembly = mono_assembly_open (path, NULL);
	if (assembly == NULL) {
		NSLog (@ PRODUCT ": Could not find the required assembly '%s' in the app. This is usually fixed by cleaning and rebuilding your project; if that doesn't work, please file a bug report: http://bugzilla.xamarin.com", name);
		exit (1);
	}
		
	return assembly;
}

static void
register_assembly (MonoAssembly *assembly)
{
	xamarin_register_assembly (mono_assembly_get_object (mono_domain_get (), assembly));
}

MonoAssembly *
xamarin_open_and_register (const char *aname)
{
	MonoAssembly *assembly;

	assembly = open_assembly (aname);

	register_assembly (assembly);
	
	return assembly;
}

static gboolean 
is_class_finalization_aware (MonoClass *cls)
{
	gboolean rv = false;

	if (nsobject_class)
		rv = cls == nsobject_class || mono_class_is_assignable_from (nsobject_class, cls);

	//NSLog (@"IsClass %s.%s finalization aware: %i\n", mono_class_get_namespace (cls), mono_class_get_name (cls), rv);

	return rv;
}

static void 
object_queued_for_finalization (MonoObject *object)
{
	/* This is called with the GC lock held, so it can only use signal-safe code */
	struct Managed_NSObject *obj = (struct Managed_NSObject *) object;
	//NSLog (@"In finalization response for %s.%s %p (handle: %p class_handle: %p flags: %i)\n", 
	obj->flags |= NSObjectFlagsInFinalizerQueue;
}

/*
 * Registration map
 */ 

void
xamarin_add_registration_map (struct MTRegistrationMap *map)
{
	map->next = registration_data.map;
	registration_data.map = map;
	registration_data.total_count += map->map_count;
}

/*
 * Exception handling
 */

static XamarinUnhandledExceptionFunc unhandled_exception_func;

void 
xamarin_install_unhandled_exception_hook (XamarinUnhandledExceptionFunc func)
{
	unhandled_exception_func = func;	
}

static MonoObject *
fetch_exception_property (MonoObject *obj, const char *name, bool is_virtual)
{
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
		NSLog (@"Could not find the property System.Exception.%s", name);
	}
	
	return NULL;
}

static char *
fetch_exception_property_string (MonoObject *obj, const char *name, bool is_virtual)
{
	MonoString *str = (MonoString *) fetch_exception_property (obj, name, is_virtual);
	return str ? mono_string_to_utf8 (str) : NULL;
}

static void
print_exception (MonoObject *exc, bool is_inner, NSMutableString *msg)
{
	MonoClass *type = mono_object_get_class (exc);
	char *type_name = xamarin_strdup_printf ("%s.%s", mono_class_get_namespace (type), mono_class_get_name (type));
	char *trace = fetch_exception_property_string (exc, "get_StackTrace", true);
	char *message = fetch_exception_property_string (exc, "get_Message", true);

	if (!is_inner) {
		[msg appendString:@"Unhandled managed exception:\n"];
	} else {
		[msg appendString:@" --- inner exception ---\n"];
	}
	[msg appendFormat: @"%s (%s)\n%s\n", message, type_name, trace];

	if (unhandled_exception_func && !is_inner)
		unhandled_exception_func (exc, type_name, message, trace);

	mono_free (trace);
	mono_free (message);
	xamarin_free (type_name);
}

static NSMutableString *
print_all_exceptions (MonoObject *exc)
{
	NSMutableString *str = [[NSMutableString alloc] init];
	// fetch the field, since the property might have been linked away.
	int counter = 0;
	MonoClassField *inner_exception = mono_class_get_field_from_name (mono_object_get_class (exc), "inner_exception");

	do {
		print_exception (exc, counter > 0, str);
		if (inner_exception) {
			mono_field_get_value (exc, inner_exception, &exc);
		} else {
			LOG ("Could not find the field inner_exception in System.Exception\n");
			break;
		}
	} while (counter++ < 10 && exc);

	[str autorelease];
	return str;
}

void
xamarin_unhandled_exception_handler (MonoObject *exc, gpointer user_data)
{
	NSLog (@"%@", print_all_exceptions (exc));

	abort ();
}

static void
exception_handler (NSException *exc)
{
	LOG (PRODUCT ": Received unhandled ObjectiveC exception: %@ %@", [exc name], [exc reason]);
	
	// This might happen on a thread we haven't heard about before
	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);

	xamarin_throw_ns_exception (exc);
}

#if defined (DEBUG)
static void *
pump_gc (void *context)
{
	mono_thread_attach (mono_get_root_domain ());

	while (xamarin_gc_pump) {
		mono_gc_collect (mono_gc_max_generation ());
		usleep (1000000);
	}
	return NULL;
}
#endif /* DEBUG */

static void
detect_product_assembly ()
{
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

static void
log_callback (const char *log_domain, const char *log_level, const char *message, mono_bool fatal, void *user_data)
{
	NSLog (@"%s: %s", log_level, message);

	if (fatal)
		abort ();
}

static void
print_callback (const char *string, mono_bool is_stdout)
{
	NSLog (@"%s", string);
}

void
xamarin_initialize ()
{
	MonoClass *runtime_class;
	MonoAssembly *assembly = NULL;
	MonoImage *image;
	MonoMethod *runtime_initialize;
	struct InitializationOptions options;
	void* params[2];
	const char *product_dll = NULL;

	initialize_started = TRUE;

#ifdef DYNAMIC_MONO_RUNTIME
	// We might be called from the managed Runtime.EnsureInitialized method,
	// in which case xamarin_initialize_dynamic_runtime has not been called yet.
	xamarin_initialize_dynamic_runtime (NULL);
#endif

	xamarin_insert_dllmap ();

	mono_trace_set_log_handler (log_callback, NULL);
	mono_trace_set_print_handler (print_callback);
	mono_trace_set_printerr_handler (print_callback);

	detect_product_assembly ();

	MonoGCFinalizerCallbacks gc_callbacks;
	gc_callbacks.version = MONO_GC_FINALIZER_EXTENSION_VERSION;
	gc_callbacks.is_class_finalization_aware = is_class_finalization_aware;
	gc_callbacks.object_queued_for_finalization = object_queued_for_finalization;
	mono_gc_register_finalizer_callbacks (&gc_callbacks);

	NSSetUncaughtExceptionHandler (exception_handler);

	product_dll = xamarin_use_new_assemblies ? PRODUCT_DUAL_ASSEMBLY : PRODUCT_COMPAT_ASSEMBLY;

	assembly = open_assembly (product_dll);

	if (!assembly)
		xamarin_assertion_message ("Failed to load %s.", product_dll);
	image = mono_assembly_get_image (assembly);

	const char *objcruntime = xamarin_use_new_assemblies ? "ObjCRuntime" : PRODUCT_COMPAT_NAMESPACE ".ObjCRuntime";
	const char *foundation = xamarin_use_new_assemblies ? "Foundation" : PRODUCT_COMPAT_NAMESPACE ".Foundation";

	runtime_class = get_class_from_name (image, objcruntime, "Runtime");
	inativeobject_class = get_class_from_name (image, objcruntime, "INativeObject");
	nsobject_class = get_class_from_name (image, foundation, "NSObject");

	runtime_initialize = mono_class_get_method_from_name (runtime_class, "Initialize", 1);

	memset (&options, 0, sizeof (options));
	options.size = sizeof (options);
	if (xamarin_use_new_assemblies && xamarin_use_old_dynamic_registrar)
		options.flags = (enum InitializationFlags) (options.flags | InitializationFlagsUseOldDynamicRegistrar);
	if (xamarin_use_il_registrar)
		options.flags = (enum InitializationFlags) (options.flags | InitializationFlagsILRegistrar);
#if MONOTOUCH && (defined(__i386__) || defined (__x86_64__))
	options.flags = (enum InitializationFlags) (options.flags | InitializationFlagsIsSimulator);
#endif

	options.Trampolines = trampolines;
	options.RegistrationData = &registration_data;
	options.MarshalObjectiveCExceptionMode = xamarin_marshal_objectivec_exception_mode;
	options.MarshalManagedExceptionMode = xamarin_marshal_managed_exception_mode;

	params [0] = &options;

	mono_runtime_invoke (runtime_initialize, NULL, params, NULL);

	delegates = options.Delegates;
			
	register_assembly (assembly);
	install_nsautoreleasepool_hooks ();

#if defined (DEBUG)
// Disable this for watchOS for now, since we still have known bugs with the COOP GC causing crashes.
#if !TARGET_OS_WATCH
	if (xamarin_gc_pump) {
		pthread_t gc_thread;
		pthread_create (&gc_thread, NULL, pump_gc, NULL);
	}
#endif // !TARGET_OS_WATCH
#endif

	gc_enable_new_refcount ();
}

static char *x_bundle_path = NULL;
const char *
xamarin_get_bundle_path ()
{
	if (x_bundle_path != NULL)
		return x_bundle_path;

	NSBundle *main_bundle = [NSBundle mainBundle];
	NSString *bundle_path;
	char *result;

#if MONOMAC
	if (xamarin_custom_bundle_name != nil)
		bundle_path = [[main_bundle bundlePath] stringByAppendingPathComponent:[@"Contents/" stringByAppendingString:xamarin_custom_bundle_name]];
	else
		bundle_path = [[main_bundle bundlePath] stringByAppendingPathComponent:@"Contents/MonoBundle"];
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
	free (x_bundle_path);
	x_bundle_path = strdup (path);
}

void
xamarin_free (void *ptr)
{
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
		NSLog (@ PRODUCT ": %s", formatted);
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

			++type;

			do {
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
			} while (*type != '=');

			type++;

			while (*type != _C_STRUCT_E) {
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
	
	xamarin_assertion_message ("Unsupported type encoding: %s", type);
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
	NSLog (@"\tGCHandle created for %p: %d (flags: %p) = %d %s\n", self, gchandle, GINT_TO_POINTER (flags), get_raw_gchandle (self), weak ? "weak" : "strong");
#endif
}

void
xamarin_switch_gchandle (id self, bool to_weak)
{
	int new_gchandle;
	int old_gchandle;
	int old_gchandle_raw;
	MonoObject *managed_object;
	int flags = MANAGED_REF_BIT;

	old_gchandle_raw = get_raw_gchandle (self);
	old_gchandle = old_gchandle_raw & ~GCHANDLE_MASK;
	if (old_gchandle) {
		bool is_weak = (old_gchandle_raw & GCHANDLE_WEAK) == GCHANDLE_WEAK;
		if (to_weak == is_weak) {
			// we already have the GCHandle we need
#if defined(DEBUG_REF_COUNTING)
			NSLog (@"Object %p already has a %s GCHandle = %d\n", self, to_weak ? "weak" : "strong", old_gchandle);
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
		NSLog (@"Object %p has no managed object to create a %s GCHandle for\n", self, to_weak ? "weak" : "strong");
#endif
		return;
	}

	if (mono_domain_get () == NULL)
		mono_jit_thread_attach (NULL);
	
	if (old_gchandle) {
		managed_object = mono_gchandle_get_target (old_gchandle);
	} else {
		managed_object = xamarin_get_managed_object_for_ptr (self);
	}
	
	if (to_weak) {
		new_gchandle = mono_gchandle_new_weakref (managed_object, TRUE);
		flags |= GCHANDLE_WEAK;
	} else {
		new_gchandle = mono_gchandle_new (managed_object, FALSE);
	}
	
	if (old_gchandle)
		mono_gchandle_free (old_gchandle);
	
	xamarin_set_nsobject_flags (managed_object, xamarin_get_nsobject_flags (managed_object) | NSObjectFlagsHasManagedRef);
	set_raw_gchandle (self, new_gchandle | flags);

#if defined(DEBUG_REF_COUNTING)
	NSLog (@"Switched object %p to %s GCHandle = %d\n", self, to_weak ? "weak" : "strong", new_gchandle);
#endif
}

void
xamarin_free_gchandle (id self, int gchandle)
{
	if (gchandle) {
#if defined(DEBUG_REF_COUNTING)
		NSLog (@"\tGCHandle %i destroyed for object %p\n", gchandle, self);
#endif
		mono_gchandle_free (gchandle);

		set_raw_gchandle (self, 0);
	} else {
#if defined(DEBUG_REF_COUNTING)
		NSLog (@"\tNo GCHandle for the object %p\n", self);
#endif
	}
}

void
xamarin_clear_gchandle (id self)
{
	set_raw_gchandle (self, 0);
}

void
xamarin_set_gchandle (id self, int gchandle)
{
	set_raw_gchandle (self, gchandle);
}

static inline bool
is_user_type (id self)
{
	return class_getInstanceMethod (object_getClass (self), @selector (xamarinSetGCHandle:)) != NULL;
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
		return [self retainCount];
	}
}
#endif

extern "C" void
xamarin_release_managed_ref (id self, MonoObject *managed_obj)
{
	bool user_type = is_user_type (self);
	
#if defined(DEBUG_REF_COUNTING)
	NSLog (@"monotouch_release_managed_ref (%s Handle=%p) retainCount=%d; HasManagedRef=%i GCHandle=%i IsUserType=%i\n", 
		class_getName (object_getClass (self)), self, (int32_t) [self retainCount], user_type ? xamarin_has_managed_ref (self) : 666, user_type ? get_gchandle (self) : 666, user_type);
#endif

	xamarin_set_nsobject_flags (managed_obj, xamarin_get_nsobject_flags (managed_obj) & ~NSObjectFlagsHasManagedRef);

	if (user_type) {
		/* clear MANAGED_REF_BIT */
		set_raw_gchandle (self, get_raw_gchandle (self) & ~MANAGED_REF_BIT);
		[self release];
	} else {
		// This lock is needed so that we can safely call retainCount in the toggleref callback.
		xamarin_framework_peer_lock ();
		/* If we're a wrapper type, we need to unregister here, since we won't enter the release trampoline */
		xamarin_unregister_nsobject (self, managed_obj);
		[self release];
		xamarin_framework_peer_unlock ();
	}

}

void
xamarin_create_managed_ref (id self, gpointer managed_object, bool retain)
{
	int gchandle;
	bool user_type = is_user_type (self);
	
#if defined(DEBUG_REF_COUNTING)
	NSLog (@"monotouch_create_managed_ref (%s Handle=%p) retainCount=%d; HasManagedRef=%i GCHandle=%i IsUserType=%i\n", 
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

	if (retain)
		[self retain];
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
get_method_block_wrapper_creator (MonoMethod *method, int par)
{
	MonoObject *res = NULL;
	MethodAndPar mp, *nmp;
	mp.method = method;
	mp.par = par;

	// NSLog (@"Looking up method and par (%x and %d)", (int) method, par);
	pthread_mutex_lock (&wrapper_hash_lock);
	if (block_wrapper_queue == NULL)
		block_wrapper_queue = mono_gc_reference_queue_new ((void(*)(void*))_Block_release);

	if (xamarin_wrapper_hash == NULL) {
		xamarin_wrapper_hash = mono_g_hash_table_new_type (method_and_par_hash, method_and_par_compare, MONO_HASH_VALUE_GC);
	}

	res = (MonoObject *) mono_g_hash_table_lookup (xamarin_wrapper_hash, &mp);
	pthread_mutex_unlock (&wrapper_hash_lock);
	if (res != NULL){
		// NSLog (@"Found match: %x", (int) res);
		return res;
	}

	res = xamarin_get_block_wrapper_creator ((MonoObject *) mono_method_get_object (mono_domain_get (), method, NULL), par);
	// NSLog (@"New value: %x", (int) res);

	nmp = (MethodAndPar *) malloc (sizeof (MethodAndPar));
	*nmp = mp;

	pthread_mutex_lock (&wrapper_hash_lock);
	mono_g_hash_table_insert (xamarin_wrapper_hash, nmp, res);
	pthread_mutex_unlock (&wrapper_hash_lock);
	return res;
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
xamarin_get_delegate_for_block_parameter (MonoMethod *method, int par, void *nativeBlock)
{
	MonoObject *delegate;

	if (nativeBlock == NULL)
		return NULL;

	/* retain or copy (if it's a stack block) the block */
	nativeBlock = _Block_copy (nativeBlock);

	delegate = delegates.create_block_proxy (get_method_block_wrapper_creator (method, par), nativeBlock);

	pthread_mutex_lock (&wrapper_hash_lock);
	mono_gc_reference_queue_add (block_wrapper_queue, delegate, nativeBlock);
	pthread_mutex_unlock (&wrapper_hash_lock);

	return (int *) delegate;
}

id
xamarin_get_block_for_delegate (MonoMethod *method, MonoObject *delegate)
{
	return delegates.create_delegate_proxy ((MonoObject *) mono_method_get_object (mono_domain_get (), method, NULL), delegate);
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

void
xamarin_set_is_unified (bool value)
{
	if (initialize_started)
		xamarin_assertion_message ("Fatal error: xamarin_set_is_unified called after xamarin_initialize.\n");

	xamarin_use_new_assemblies = value;
	xamarin_detect_unified_build = false;
}

bool
xamarin_get_is_unified ()
{
	return xamarin_use_new_assemblies;
}

void
xamarin_set_gc_pump_enabled (bool value)
{
	xamarin_gc_pump = value;
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

void
xamarin_process_nsexception (NSException *ns_exception)
{
	MarshalObjectiveCExceptionMode mode;
	XamarinGCHandle *exc_handle;

	mode = xamarin_on_marshal_objectivec_exception (ns_exception);

	if (mode == MarshalObjectiveCExceptionModeDefault)
		mode = xamarin_is_gc_coop ? MarshalObjectiveCExceptionModeThrowManagedException : MarshalObjectiveCExceptionModeUnwindManagedCode;

	switch (mode) {
	case MarshalObjectiveCExceptionModeUnwindManagedCode:
		if (xamarin_is_gc_coop)
			xamarin_assertion_message ("Cannot unwind managed frames for Objective-C exceptions when using the COOP GC.");
		@throw ns_exception;
		break;
	case MarshalObjectiveCExceptionModeThrowManagedException:
		exc_handle = [[ns_exception userInfo] objectForKey: @"XamarinManagedExceptionHandle"];
		if (exc_handle != NULL) {
			int handle = [exc_handle getHandle];
			MonoObject *exc = mono_gchandle_get_target (handle);
			mono_set_pending_exception ((MonoException *) exc);
		} else {
			int handle = xamarin_create_ns_exception (ns_exception);
			MonoObject *exc = mono_gchandle_get_target (handle);
			mono_set_pending_exception ((MonoException *) exc);
			mono_gchandle_free (handle);
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

	int handle = mono_gchandle_new (exception, false);
	mode = xamarin_on_marshal_managed_exception (handle);
	mono_gchandle_free (handle);

	if (mode == MarshalManagedExceptionModeDefault)
		mode = xamarin_is_gc_coop ? MarshalManagedExceptionModeThrowObjectiveCException : MarshalManagedExceptionModeUnwindNativeCode;

	switch (mode) {
	case MarshalManagedExceptionModeUnwindNativeCode:
		if (xamarin_is_gc_coop)
			xamarin_assertion_message ("Cannot unwind native frames for managed exceptions when using the COOP GC.");
		mono_raise_exception ((MonoException *) exception);
		break;
	case MarshalManagedExceptionModeThrowObjectiveCException: {
		int handle = mono_gchandle_new (exception, false);
		NSException *ns_exc = xamarin_unwrap_ns_exception (handle);
		
		if (ns_exc != NULL) {
			mono_gchandle_free (handle);
			@throw ns_exc;
		} else {
			NSString *name = [NSString stringWithUTF8String: xamarin_type_get_full_name (mono_class_get_type (mono_object_get_class (exception)))];
			char *message = fetch_exception_property_string (exception, "get_Message", true);
			NSString *reason = [NSString stringWithUTF8String: message];
			mono_free (message);
			NSDictionary *userInfo = [NSDictionary dictionaryWithObject: [XamarinGCHandle createWithHandle: handle] forKey: @"XamarinManagedExceptionHandle"];
			@throw [[NSException alloc] initWithName: name reason: reason userInfo: userInfo];
		}
		break;
	}
	case MarshalManagedExceptionModeAbort:
	default:
		xamarin_assertion_message ("Aborting due to:\n%s\n", [print_all_exceptions (exception) UTF8String]);
		break;
	}
}

void
xamarin_insert_dllmap ()
{
#if defined (__i386__) || defined (__x86_64__)
	if (xamarin_marshal_objectivec_exception_mode == MarshalObjectiveCExceptionModeDisable)
		return;
#if DYNAMIC_MONO_RUNTIME
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
