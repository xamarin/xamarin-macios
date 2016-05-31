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

#ifdef __cplusplus
extern "C" {
#endif

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

typedef struct {
	const char *name;
	const char *_typename;
	void *handle;
} MTClassMap;

struct MTRegistrationMap;

struct MTRegistrationMap {
	struct MTRegistrationMap *next;
	const char **assembly;
	MTClassMap *map;
	int assembly_count;
	int map_count;
	int custom_type_count;
};

typedef struct {
	MonoReflectionMethod *method;
	int32_t semantic;
} MethodDescription;

// This has a managed equivalent in NSObject2.cs
enum NSObjectFlags {
	NSObjectFlagsDisposed = 1,
	NSObjectFlagsNativeRef = 2,
	NSObjectFlagsIsDirectBinding = 4,
	NSObjectFlagsRegisteredToggleRef = 8,
	NSObjectFlagsInFinalizerQueue = 16,
	NSObjectFlagsHasManagedRef = 32,
};

void xamarin_initialize ();

void			xamarin_assertion_message (const char *msg, ...) __attribute__((__noreturn__));
const char *	xamarin_get_bundle_path (); /* Public API */
// Sets the bundle path (where the managed executable is). By default APP/Contents/MonoBundle.
void			xamarin_set_bundle_path (const char *path); /* Public API */
MonoObject *	xamarin_get_managed_object_for_ptr (id self);
MonoObject *	xamarin_get_managed_object_for_ptr_fast (id self);
void			xamarin_check_for_gced_object (MonoObject *obj, SEL sel, id self, MonoMethod *method);
int				xamarin_objc_type_size (const char *type);
bool			xamarin_is_class_nsobject (MonoClass *cls);
bool			xamarin_is_class_inativeobject (MonoClass *cls);
bool			xamarin_is_class_array (MonoClass *cls);
MonoType *		xamarin_get_parameter_type (MonoMethod *managed_method, int index);
MonoObject *	xamarin_get_nsobject_with_type_for_ptr (id self, bool owns, MonoType* type);
MonoObject *	xamarin_get_nsobject_with_type_for_ptr_created (id self, bool owns, MonoType *type, int32_t *created);
int *			xamarin_get_delegate_for_block_parameter (MonoMethod *method, int par, void *nativeBlock);
id              xamarin_get_block_for_delegate (MonoMethod *method, MonoObject *delegate);          
id				xamarin_get_nsobject_handle (MonoObject *obj);
void			xamarin_set_nsobject_handle (MonoObject *obj, id handle);
uint8_t         xamarin_get_nsobject_flags (MonoObject *obj);
void			xamarin_set_nsobject_flags (MonoObject *obj, uint8_t flags);
void			xamarin_throw_nsexception (MonoException *exc);
MonoException *	xamarin_create_exception (const char *msg);
id				xamarin_get_handle (MonoObject *obj);
char *			xamarin_strdup_printf (const char *msg, ...);
void			xamarin_free (void *ptr);
MonoMethod *	xamarin_get_reflection_method_method (MonoReflectionMethod *method);
void			xamarin_framework_peer_lock ();
void			xamarin_framework_peer_unlock ();
bool			xamarin_file_exists (const char *path);
MonoAssembly *	xamarin_open_and_register (const char *path);
void			xamarin_unhandled_exception_handler (MonoObject *exc, gpointer user_data);
void			xamarin_ftnptr_exception_handler (guint32 gchandle);
void			xamarin_create_classes ();
const char *	xamarin_skip_encoding_flags (const char *encoding);
void			xamarin_add_registration_map (struct MTRegistrationMap *map);

bool			xamarin_has_managed_ref (id self);
bool			xamarin_has_managed_ref_safe (id self);
void			xamarin_switch_gchandle (id self, bool to_weak);
int				xamarin_get_gchandle (id self);
void			xamarin_free_gchandle (id self, int gchandle);
void			xamarin_clear_gchandle (id self);
int				xamarin_get_gchandle_with_flags (id self);
void			xamarin_set_gchandle (id self, int gchandle);
void			xamarin_create_managed_ref (id self, void * managed_object, bool retain);
void            xamarin_release_managed_ref (id self, MonoObject *managed_obj);
void			xamarin_notify_dealloc (id self, int gchandle);

int				xamarin_main (int argc, char *argv[], bool is_extension);

char *			xamarin_type_get_full_name (MonoType *type); // return value must be freed with 'mono_free'
char *			xamarin_class_get_full_name (MonoClass *klass); // return value must be freed with 'mono_free'

#if DEBUG
void			xamarin_verify_parameter (MonoObject *obj, SEL sel, id self, id arg, int index, MonoClass *expected, MonoMethod *method);
void			xamarin_check_objc_type (id obj, Class expected_class, SEL sel, id self, int index, MonoMethod *method);
#endif

void			xamarin_set_gc_pump_enabled (bool value);

typedef void  	(*XamarinUnhandledExceptionFunc)         (MonoObject *exc, const char *type_name, const char *message, const char *trace);
void          	xamarin_install_unhandled_exception_hook (XamarinUnhandledExceptionFunc func);
void			xamarin_process_nsexception (NSException *exc);
void			xamarin_process_nsexception_using_mode (NSException *ns_exception, bool throwManagedAsDefault);
void			xamarin_process_managed_exception (MonoObject *exc);

id				xamarin_invoke_objc_method_implementation (id self, SEL sel, IMP xamarin_impl);

#if defined(__arm__) || defined(__aarch64__)
void mono_aot_register_module (void *aot_info);
#endif

typedef void (*xamarin_register_module_callback) ();
typedef void (*xamarin_register_assemblies_callback) ();
extern xamarin_register_module_callback xamarin_register_modules;
extern xamarin_register_assemblies_callback xamarin_register_assemblies;

/* Functions calling into ObjCRuntime.Runtime */

void        				xamarin_register_nsobject					(MonoObject *managed_obj, id native_obj);
void        				xamarin_register_assembly					(MonoReflectionAssembly *assembly);
MonoObject* 				xamarin_create_product_exception			(NSException *exc);
id          				xamarin_get_inative_handle					(MonoObject *managed_obj);
MonoObject* 				xamarin_get_block_wrapper_creator			(MonoObject *method, int parameter);
MonoObject*		 			xamarin_create_block_proxy					(MonoObject *method, void* block);
void						xamarin_register_assembly_path 				(const char *path);
MonoObject*					xamarin_get_class							(Class ptr);
MonoObject*					xamarin_get_selector						(SEL ptr);
Class						xamarin_get_class_handle					(MonoObject *obj);
SEL							xamarin_get_selector_handle					(MonoObject *obj);
MethodDescription			xamarin_get_method_for_selector				(Class cls, SEL sel);
bool						xamarin_has_nsobject 						(id obj);
MonoObject*					xamarin_get_nsobject 						(id obj);
id							xamarin_get_handle_for_inativeobject		(MonoObject *obj);
void						xamarin_unregister_nsobject					(id native_obj, MonoObject *managed_obj);
MonoReflectionMethod*		xamarin_get_method_direct					(const char *typeptr, const char *methodptr, int paramCount, const char **parameters);
MonoReflectionMethod*		xamarin_get_generic_method_direct			(MonoObject *self, const char *typeptr, const char *methodptr, int paramCount, const char **parameters);
MonoObject*					xamarin_try_get_or_construct_nsobject 		(id obj);
MonoObject*					xamarin_get_inative_object_dynamic			(id obj, bool owns, void *type);
MonoObject*					xamarin_get_inative_object_static			(id obj, bool owns, const char *type_name, const char *iface_name);
MonoObject*					xamarin_get_nsobject_with_type				(id obj, void *type, int32_t *created);
void						xamarin_dispose								(MonoObject *mobj);
bool	 					xamarin_is_parameter_transient				(MonoReflectionMethod *method, int parameter /* 0-based */);
bool						xamarin_is_parameter_out                    (MonoReflectionMethod *method, int parameter /* 0-based */);
MethodDescription			xamarin_get_method_and_object_for_selector	(Class cls, SEL sel, id self, MonoObject **mthis);
void 						xamarin_throw_product_exception				(int code, const char *message);

class XamarinObject {
public:
	id native_object;
	int gc_handle;

	~XamarinObject ();
};

@interface XamarinAssociatedObject : NSObject {
@public
	id native_object;
	int gc_handle;
}
-(void) dealloc;
@end

@interface NSObject (NonXamarinObject)
-(int) xamarinGetGCHandle;
@end

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

// Once we have one mono clone again the TARGET_OS_WATCH
// condition should be removed (DYNAMIC_MONO_RUNTIME should still
// be here though).
#if TARGET_OS_WATCH && !defined (DYNAMIC_MONO_RUNTIME)
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

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __XAMARIN_RUNTIME__ */
