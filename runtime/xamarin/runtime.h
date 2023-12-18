/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * runtime.h: This header contains definitions used by Xamarin when building applications.
 *            Do not consider anything here stable API (unless otherwise specified),
 *            it will change between releases.
 *
 *  Authors: Rolf Bjarne Kvinge
 *
 *  Copyright (C) 2014 Xamarin Inc. (www.xamarin.com)
 *
 */

#ifndef __XAMARIN_RUNTIME__
#define __XAMARIN_RUNTIME__

#include <stdbool.h>
#include <Foundation/Foundation.h>

#include "main.h"
#include "mono-runtime.h"
#include "runtime-generated.h"
#include "trampolines.h"

#ifdef __cplusplus
extern "C" {
#endif

struct InitializationOptions;

typedef struct {
	const char *name;
	const char *type;
	int size;
	int align;
} MTIvar;

typedef struct {
	const char *selector;
	const char *signature;
	int isstatic;
	void *trampoline;
} MTMethod;

typedef struct {
	const char *name;
	const char *supername;
	int ivar_count;
	int method_count;
	int prop_count;
} MTClass;

typedef struct {
	const char *name;
	const char *type;
	const char *argument_semantic;
} MTProperty;

// This structure completely describes everything required to resolve a metadata token
typedef struct __attribute__((packed)) {
	uint32_t assembly_index; /* index into the 'assemblies' array in the registration map */
	uint32_t module_token;
	uint32_t token;
} MTFullTokenReference;

// This structure is packed to be exactly 32 bits
// If 'is_full_reference' is 1, then the remaining bits are an index into a table of MTFullTokenReference.
typedef struct __attribute__((packed)) {
	uint8_t is_full_reference:1;
	uint8_t assembly_index:7; /* 0-based index into the '__xamarin_registration_assemblies' array. Max 127 (registered) assemblies before a full token reference has to be used */
	uint32_t token:24; /* RID of the corresponding metadata token. The exact type of metadata token depends on the context where the token reference is used. */
} MTTokenReference;
static const uint32_t INVALID_TOKEN_REF = 0xFFFFFFFF;

enum MTTypeFlags {
	MTTypeFlagsNone = 0,
	MTTypeFlagsCustomType = 1, // not a platform type
	MTTypeFlagsUserType = 2, // not a wrapped type
};

typedef struct __attribute__((packed)) {
	void *handle;
	uint32_t /* MTTokenReference */ type_reference;
	uint32_t /* MTTypeFlags */ flags;
} MTClassMap;

typedef struct __attribute__((packed)) {
	uint32_t /* MTTokenReference */ skipped_reference;
	uint32_t /* MTTokenReference */ actual_reference;
} MTManagedClassMap;

typedef struct __attribute__((packed)) {
	uint32_t protocol_token;
	uint32_t wrapper_token;
} MTProtocolWrapperMap;

typedef struct __attribute__((packed)) {
	const uint32_t *protocol_tokens; // an array of token references to managed interfaces that represent protocols
	// __unsafe_unretained needed to prevent "error: pointer to non-const type 'const Protocol *' with no explicit ownership" in Embeddinator
	const __unsafe_unretained Protocol * const * protocols; // the corresponding native protocols
} MTProtocolMap;

typedef struct __attribute__((packed)) {
	const char *name;
	const char *mvid;
} MTAssembly;

struct MTRegistrationMap;

struct MTRegistrationMap {
	const char *product_hash;
	const MTAssembly *assemblies;
	MTClassMap *map;
	const MTFullTokenReference *full_token_references;
	// There are some managed types that are not registered because their ObjC
	// class is already registered for a different managed type. For instance:
	// The managed type "Foundation.NSArray<T>"" is not registered, because
	// its ObjC class would be NSArray, which is already registered to
	// "Foundation.NSArray". In order to be able to map all managed types to
	// ObjC types we need to know which other managed type is the main type
	// for the ObjC type (an alternative would be to map it directly to the
	// ObjC class, but this is not a constant known at compile time, which
	// means it can't be stored in read-only memory).
	const MTManagedClassMap *skipped_map;
	const MTProtocolWrapperMap *protocol_wrappers; // array of MTProtocolWrapperMap, sorted ascending by protocol_token
	const MTProtocolMap protocols;
	int assembly_count;
	int map_count;
	int full_token_reference_count;
	int skipped_map_count;
	int protocol_wrapper_count;
	int protocol_count;
	void **classHandles;
};

typedef struct {
	GCHandle original_type_handle;
} BindAsData;

typedef struct {
	GCHandle method_handle;
	int32_t semantic;
	int32_t bindas_count; // The number of elements available in the bindas_types array.
	// An array of BindAs original types. Element 0 is for the return value,
	// the rest are for parameters (parameters start at 1 even for void methods).
	// The array must contain space for the return value and all the parameters,
	// even for those that don't have BindAs attributes (the original_type entry will be NULL).
	BindAsData bindas[];
} MethodDescription;

struct AssemblyLocation {
	const char *assembly_name; // base name (without extension) of the assembly
	const char *location; // the directory where the assembly is
};

struct AssemblyLocations {
	size_t length;
	struct AssemblyLocation *locations;
};

void xamarin_initialize ();
void xamarin_initialize_embedded (); /* Public API, must not change - this is used by the embeddinator */

void			xamarin_assertion_message (const char *msg, ...) __attribute__((__noreturn__));
// Gets the bundle path (where the managed executable is). This is *not* the path of the app bundle (.app/.appex).
const char *	xamarin_get_bundle_path (); /* Public API */
// Sets the bundle path (where the managed executable is). By default APP/Contents/MonoBundle.
void			xamarin_set_bundle_path (const char *path); /* Public API */
// Gets the app bundle path (.app/.appex).
const char *	xamarin_get_app_bundle_path ();
MonoObject *	xamarin_get_managed_object_for_ptr_fast (id self, GCHandle *exception_gchandle);
void			xamarin_check_for_gced_object (MonoObject *obj, SEL sel, id self, MonoMethod *method, GCHandle *exception_gchandle);
unsigned long 	xamarin_objc_type_size (const char *type);
bool			xamarin_is_class_nsobject (MonoClass *cls);
bool			xamarin_is_class_inativeobject (MonoClass *cls);
bool			xamarin_is_class_nativehandle (MonoClass *cls);
bool			xamarin_is_class_array (MonoClass *cls);
bool			xamarin_is_class_nsnumber (MonoClass *cls);
bool			xamarin_is_class_nsvalue (MonoClass *cls);
bool			xamarin_is_class_nsstring (MonoClass *cls);
bool			xamarin_is_class_nullable (MonoClass *cls, MonoClass **element_type, GCHandle *exception_gchandle);
bool			xamarin_is_class_intptr (MonoClass *cls);
bool			xamarin_is_class_string (MonoClass *cls);
MonoClass *		xamarin_get_nullable_type (MonoClass *cls, GCHandle *exception_gchandle);
MonoType *		xamarin_get_parameter_type (MonoMethod *managed_method, int index);
MonoObject *	xamarin_get_nsobject_with_type_for_ptr (id self, bool owns, MonoType* type, GCHandle *exception_gchandle);
MonoObject *	xamarin_get_nsobject_with_type_for_ptr_created (id self, bool owns, MonoType *type, int32_t *created, GCHandle *exception_gchandle);
MonoObject *	xamarin_get_delegate_for_block_parameter (MonoMethod *method, guint32 token_ref, int par, void *nativeBlock, GCHandle *exception_gchandle);
id              xamarin_get_block_for_delegate (MonoMethod *method, MonoObject *delegate, const char *signature /* NULL allowed, but requires the dynamic registrar at runtime to compute */, guint32 token_ref /* INVALID_TOKEN_REF allowed, but requires the dynamic registrar at runtime */, GCHandle *exception_gchandle);
id				xamarin_get_nsobject_handle (MonoObject *obj);
uint8_t         xamarin_get_nsobject_flags (MonoObject *obj);
void			xamarin_set_nsobject_flags (MonoObject *obj, uint8_t flags);
void			xamarin_throw_nsexception (MonoException *exc);
void			xamarin_rethrow_managed_exception (GCHandle original_gchandle, GCHandle *exception_gchandle);
MonoException *	xamarin_create_exception (const char *msg);
id				xamarin_get_handle (MonoObject *obj, GCHandle *exception_gchandle);
char *			xamarin_strdup_printf (const char *msg, ...);
void *			xamarin_calloc (size_t size);
void			xamarin_free (void *ptr);
MonoMethod *	xamarin_get_reflection_method_method (MonoReflectionMethod *method);
MonoMethod *	xamarin_get_managed_method_for_token (guint32 token_ref, GCHandle *exception_gchandle);
void			xamarin_framework_peer_waypoint ();
void			xamarin_framework_peer_waypoint_safe ();
bool			xamarin_file_exists (const char *path);
MonoAssembly *	xamarin_open_and_register (const char *path, GCHandle *exception_gchandle);
void			xamarin_unhandled_exception_handler (MonoObject *exc, gpointer user_data);
void			xamarin_ftnptr_exception_handler (GCHandle gchandle);
void			xamarin_create_classes ();
const char *	xamarin_skip_encoding_flags (const char *encoding);
void			xamarin_add_registration_map (struct MTRegistrationMap *map, bool partial);
uint32_t		xamarin_find_protocol_wrapper_type (uint32_t token_ref);
void			xamarin_release_block_on_main_thread (void *obj);
void			xamarin_bridge_setup (); // this is called very early, before parsing the command line arguments
void			xamarin_bridge_initialize (); // this is called a bit later, after parsing the command line arguments.
void			xamarin_bridge_shutdown (); // this is called just before returning from xamarin_main
unsigned char *	xamarin_load_aot_data (MonoAssembly *assembly, int size, gpointer user_data, void **out_handle);
void			xamarin_free_aot_data (MonoAssembly *assembly, int size, gpointer user_data, void *handle);
MonoAssembly*	xamarin_assembly_preload_hook (MonoAssemblyName *aname, char **assemblies_path, void* user_data);
void			xamarin_handle_bridge_exception (GCHandle gchandle, const char *method);
void			xamarin_vm_initialize ();
bool			xamarin_bridge_vm_initialize (int propertyCount, const char **propertyKeys, const char **propertyValues);
void*			xamarin_pinvoke_override (const char *libraryName, const char *entrypointName);
void			xamarin_bridge_call_runtime_initialize (struct InitializationOptions* options, GCHandle* exception_gchandle);
void			xamarin_bridge_register_product_assembly (GCHandle* exception_gchandle);
MonoMethod *	xamarin_bridge_get_mono_method (MonoReflectionMethod *method);
void			xamarin_bridge_free_mono_signature (MonoMethodSignature **signature);
bool			xamarin_register_monoassembly (MonoAssembly *assembly, GCHandle *exception_gchandle);
void			xamarin_install_nsautoreleasepool_hooks ();
void			xamarin_enable_new_refcount ();
const char * const	xamarin_get_original_working_directory_path ();
int				xamarin_get_runtime_arch ();

MonoObject *	xamarin_new_nsobject (id self, MonoClass *klass, GCHandle *exception_gchandle);
bool			xamarin_has_managed_ref (id self);
bool			xamarin_has_managed_ref_safe (id self);
void			xamarin_switch_gchandle (id self, bool to_weak);
GCHandle		xamarin_get_gchandle (id self);
void			xamarin_free_gchandle (id self, GCHandle gchandle);
void			xamarin_clear_gchandle (id self);
GCHandle		xamarin_get_gchandle_with_flags (id self, enum XamarinGCHandleFlags *flags);
bool			xamarin_set_gchandle_with_flags (id self, GCHandle gchandle, enum XamarinGCHandleFlags flags);
bool			xamarin_set_gchandle_with_flags_safe (id self, GCHandle gchandle, enum XamarinGCHandleFlags flags);
void			xamarin_create_gchandle (id self, void *managed_object, enum XamarinGCHandleFlags flags, bool force_weak);
void            xamarin_release_managed_ref (id self, bool user_type);
void			xamarin_notify_dealloc (id self, GCHandle gchandle);
void			xamarin_release_static_dictionaries ();

int				xamarin_main (int argc, char *argv[], enum XamarinLaunchMode launch_mode);

char *			xamarin_type_get_full_name (MonoType *type, GCHandle *exception_gchandle); // return value must be freed with 'mono_free'
char *			xamarin_class_get_full_name (MonoClass *klass, GCHandle *exception_gchandle); // return value must be freed with 'mono_free'

#if DEBUG
void			xamarin_verify_parameter (MonoObject *obj, SEL sel, id self, id arg, unsigned long index, MonoClass *expected, MonoMethod *method);
void			xamarin_check_objc_type (id obj, Class expected_class, SEL sel, id self, int index, MonoMethod *method);
#endif

void			xamarin_set_gc_pump_enabled (bool value);
void			xamarin_set_is_managed_static_registrar (bool value);

void			xamarin_process_nsexception (NSException *exc);
void			xamarin_process_nsexception_using_mode (NSException *ns_exception, bool throwManagedAsDefault, GCHandle *output_exception);
void			xamarin_process_managed_exception (MonoObject *exc);
void			xamarin_process_managed_exception_gchandle (GCHandle gchandle);
void			xamarin_process_fatal_exception_gchandle (GCHandle gchandle, const char *message);
void			xamarin_throw_product_exception (int code, const char *message);
GCHandle		xamarin_create_product_exception (int code, const char *message);
GCHandle		xamarin_create_product_exception_with_inner_exception (int code, GCHandle inner_exception_gchandle /* will be freed */, const char *message);
MonoException *	xamarin_create_system_exception (const char *message);
MonoException *	xamarin_create_system_invalid_cast_exception (const char *message);
MonoException *	xamarin_create_system_entry_point_not_found_exception (const char *entrypoint);
NSString *		xamarin_print_all_exceptions (GCHandle handle);
bool			xamarin_log_marshalled_exceptions ();
void			xamarin_log_managed_exception (MonoObject *exception, MarshalManagedExceptionMode mode);
void			xamarin_log_objectivec_exception (NSException *exception, MarshalObjectiveCExceptionMode mode);

id				xamarin_invoke_objc_method_implementation (id self, SEL sel, IMP xamarin_impl);
MonoType *		xamarin_get_nsnumber_type ();
MonoType *		xamarin_get_nsvalue_type ();
MonoClass *		xamarin_get_inativeobject_class ();
MonoClass *		xamarin_get_nativehandle_class ();
MonoClass *		xamarin_get_nsobject_class ();
MonoClass *		xamarin_get_nsstring_class ();
MonoClass *		xamarin_get_runtime_class ();

bool			xamarin_is_managed_exception_marshaling_disabled ();

const char *	xamarin_find_assembly_directory (const char *assembly_name);
void			xamarin_set_assembly_directories (struct AssemblyLocations *directories);
void			xamarin_get_assembly_name_without_extension (const char *aname, char *name, size_t namelen);
bool			xamarin_locate_assembly_resource_for_name (MonoAssemblyName *assembly_name, const char *resource, char *path, size_t pathlen);
bool			xamarin_locate_assembly_resource_for_root (const char *root, const char *culture, const char *resource, char *path, size_t pathlen);
bool			xamarin_locate_assembly_resource (const char *assembly_name, const char *culture, const char *resource, char *path, size_t pathlen);
bool			xamarin_locate_app_resource (const char *resource, char *path, size_t pathlen);

// this functions support NSLog/NSString-style format specifiers.
void			xamarin_printf (const char *format, ...);
void			xamarin_vprintf (const char *format, va_list args);
void			xamarin_install_log_callbacks ();

/*
 * Looks up a native function pointer for a managed [UnmanagedCallersOnly] method.
 *     function_pointer: the return value, lookup will only be performed if this points to NULL.
 *     assembly: the assembly to look in. Might be NULL if the app was not built with support for loading additional assemblies at runtime.
 *     symbol: the symbol to look up. Can be NULL to save space (this value isn't used except in error messages).
 *     id: a numerical id for faster lookup (than doing string comparisons on the symbol name).
 */
void			xamarin_registrar_dlsym (void **function_pointer, const char *assembly, const char *symbol, int32_t id);

/*
 * Wrapper GCHandle functions that takes pointer sized handles instead of ints,
 * so that we can adapt our code incrementally to use pointers instead of ints
 * until Mono's pointer-sized API is available for us.
 * Ref: https://github.com/dotnet/runtime/commit/3886a63841434af716292172737a42757a15c6a6
 */
GCHandle		xamarin_gchandle_new (MonoObject *obj, bool pinned);
GCHandle		xamarin_gchandle_new_weakref (MonoObject *obj, bool track_resurrection);
MonoObject *	xamarin_gchandle_get_target (GCHandle handle);
void			xamarin_gchandle_free (GCHandle handle);
MonoObject *	xamarin_gchandle_unwrap (GCHandle handle); // Will get the target and free the GCHandle

typedef id (*xamarin_get_handle_func) (MonoObject *info);
MonoToggleRefStatus	xamarin_gc_toggleref_callback (uint8_t flags, id handle, xamarin_get_handle_func get_handle, MonoObject *info);
void				xamarin_gc_event (MonoGCEvent event);

void			xamarin_bridge_log_monoobject (MonoObject *obj, const char *stacktrace);
/*
 * In MonoVM MonoObjects are tracked in memory/the stack directly by the GC, but that doesn't
 * work for CoreCLR, so we make it ref-counted. All code must use the functions below to retain/release
 * MonoObjects, although these functions do nothing when using MonoVM.
 *
 * The release function take a pointer to the variable that contains the MonoObject, and clears out the value,
 * to avoid running into use-after-free problems.
 */
#if defined(CORECLR_RUNTIME)
void			xamarin_mono_object_retain (MonoObject *mobj);
// Use C++ linking to be able to use method overloading, so that callers don't have to cast their variables to 'MonoObject**' (which improves type safety a lot).
extern "C++" void	xamarin_mono_object_release (MonoObject **mobj);
extern "C++" void	xamarin_mono_object_release (MonoString **mobj);
void			xamarin_mono_object_release_at_process_exit (MonoObject *mobj);
#else
// Nothing to do here.
#define			xamarin_mono_object_retain(x)
#define			xamarin_mono_object_release(x) do { *x = NULL; } while (0);
#define			xamarin_mono_object_release_at_process_exit(x)
#endif


/*
 * Look for an assembly in the app and open it.
 *
 * Stable API.
 */
MonoAssembly * xamarin_open_assembly (const char *name);

void mono_aot_register_module (void *aot_info);

typedef void (*xamarin_register_module_callback) ();
typedef void (*xamarin_register_assemblies_callback) ();
extern xamarin_register_module_callback xamarin_register_modules;
extern xamarin_register_assemblies_callback xamarin_register_assemblies;

#ifdef __cplusplus
class XamarinObject {
public:
	id native_object;
	GCHandle gc_handle;
	enum XamarinGCHandleFlags flags;

	~XamarinObject ();
};
#endif

#ifdef __OBJC__
@interface XamarinAssociatedObject : NSObject {
@public
	id native_object;
	GCHandle gc_handle;
	enum XamarinGCHandleFlags flags;
}
-(void) dealloc;
@end

@interface NSObject (NonXamarinObject)
-(GCHandle) xamarinGetGCHandle;
-(enum XamarinGCHandleFlags) xamarinGetFlags;
@end
#endif

// Coop GC helper API
#if !TARGET_OS_WATCH

#define MONO_ENTER_GC_UNSAFE
#define MONO_EXIT_GC_UNSAFE
#define MONO_ENTER_GC_SAFE
#define MONO_EXIT_GC_SAFE
#define MONO_ASSERT_GC_SAFE
#define MONO_ASSERT_GC_SAFE_OR_DETACHED
#define MONO_ASSERT_GC_UNSAFE
#define MONO_ASSERT_GC_STARTING

#else

#define MONO_ENTER_GC_UNSAFE	\
	do {	\
		gpointer __dummy;	\
		gpointer __gc_unsafe_cookie = mono_threads_enter_gc_unsafe_region (&__dummy)	\

#define MONO_EXIT_GC_UNSAFE	\
		mono_threads_exit_gc_unsafe_region	(__gc_unsafe_cookie, &__dummy);	\
	} while (0)

#define MONO_ENTER_GC_SAFE	\
	do {	\
		gpointer __dummy;	\
		gpointer __gc_safe_cookie = mono_threads_enter_gc_safe_region (&__dummy)	\

#define MONO_EXIT_GC_SAFE	\
		mono_threads_exit_gc_safe_region (__gc_safe_cookie, &__dummy);	\
	} while (0)

//#if DEBUG
	#define MONO_ASSERT_GC_SAFE      mono_threads_assert_gc_safe_region ()
	#define MONO_ASSERT_GC_SAFE_OR_DETACHED \
	do { \
		if (mono_thread_info_current_unchecked ()) \
			mono_threads_assert_gc_safe_region (); \
	} while (0)
	#define MONO_ASSERT_GC_UNSAFE    mono_threads_assert_gc_unsafe_region ()
	#define MONO_ASSERT_GC_STARTING
	// There's no way to assert STARTING, tls values inside mono aren't initialized so mono's API end up accessing random memory, and thus randomly asserting //  mono_threads_assert_gc_starting_region ()
//#else
//	#define MONO_ASSERT_GC_SAFE
//	#define MONO_ASSERT_GC_UNSAFE
//#endif /* DEBUG */

#endif /* !TARGET_OS_WATCH */

#if defined(CORECLR_RUNTIME)
// this is not needed for CoreCLR
#define MONO_THREAD_ATTACH
#define MONO_THREAD_DETACH
// Once we have one mono clone again the TARGET_OS_WATCH
// condition should be removed (DYNAMIC_MONO_RUNTIME should still
// be here though).
#elif TARGET_OS_WATCH && !defined (DYNAMIC_MONO_RUNTIME)
#define MONO_THREAD_ATTACH \
	do { \
		gpointer __thread_dummy; \
		gpointer __thread_cookie = mono_threads_attach_coop (NULL, &__thread_dummy) \

#define MONO_THREAD_DETACH \
		mono_threads_detach_coop (__thread_cookie, &__thread_dummy); \
	} while (0)
#else
#define MONO_THREAD_ATTACH \
	do { \
		mono_jit_thread_attach (NULL) \

#define MONO_THREAD_DETACH \
	} while (0)
#endif

void objc_release (id value);
id objc_retain (id value);
id objc_autorelease (id value);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __XAMARIN_RUNTIME__ */
