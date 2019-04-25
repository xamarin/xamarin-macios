// vim: set filetype=c :
//
// mono-runtime.h: Header file to allow dynamic loading of
// Mono to be transparent from code including this header.
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc.
//

#ifndef __MONO_RUNTIME__
#define __MONO_RUNTIME__

#ifdef __cplusplus
extern "C" {
#endif

/* This is copied from eglib's header files */
typedef unsigned int   guint;
typedef int32_t        gboolean;
typedef int32_t        gint32;
typedef uint32_t       guint32;
typedef char           gchar;
typedef void *         gpointer;
typedef const void *   gconstpointer;
typedef guint    (*GHashFunc)      (gconstpointer key);
typedef gboolean (*GEqualFunc)     (gconstpointer a, gconstpointer b);

#ifndef GPOINTER_TO_INT
#define GPOINTER_TO_INT(p) ((int)  (long) (p))
#endif

#ifndef GINT_TO_POINTER
#define GINT_TO_POINTER(i) ((void *) (long) (i))
#endif

#include <stdint.h>

const char * xamarin_initialize_dynamic_runtime (const char *mono_runtime_prefix);
char *xamarin_get_mono_runtime_build_info (); // returns NULL if libmono couldn't be found (mono_get_runtime_build_info doesn't exist).


#ifdef DYNAMIC_MONO_RUNTIME
#define mono_class_from_name mono_class_from_name_impl
#define mono_class_get_method_from_name mono_class_get_method_from_name_impl
#define mono_class_get_field_from_name mono_class_get_field_from_name_impl
#define mono_class_is_assignable_from mono_class_is_assignable_from_impl
#define mono_class_from_mono_type mono_class_from_mono_type_impl
#define mono_class_is_delegate mono_class_is_delegate_impl
#define mono_class_get_element_class mono_class_get_element_class_impl
#define mono_class_get_namespace mono_class_get_namespace_impl
#define mono_class_get_name mono_class_get_name_impl
#define mono_class_get_parent mono_class_get_parent_impl
#define mono_class_is_subclass_of mono_class_is_subclass_of_impl
#define mono_class_is_valuetype mono_class_is_valuetype_impl
#define mono_class_is_enum mono_class_is_enum_impl
#define mono_class_enum_basetype mono_class_enum_basetype_impl
#define mono_class_value_size mono_class_value_size_impl
#define mono_class_get_type mono_class_get_type_impl
#define mono_class_is_nullable mono_class_is_nullable_impl
#define mono_class_get_nullable_param mono_class_get_nullable_param_impl
#define mono_method_full_name mono_method_full_name_impl
#define mono_runtime_invoke mono_runtime_invoke_impl
#define mono_gchandle_new mono_gchandle_new_impl
#define mono_gchandle_get_target mono_gchandle_get_target_impl
#define mono_gchandle_free mono_gchandle_free_impl
#define mono_gchandle_new_weakref mono_gchandle_new_weakref_impl
#define mono_raise_exception mono_raise_exception_impl
#define mono_array_addr_with_size mono_array_addr_with_size_impl
#define mono_string_new mono_string_new_impl
#define mono_array_new mono_array_new_impl
#define mono_object_unbox mono_object_unbox_impl
#define mono_string_to_utf8 mono_string_to_utf8_impl
#define mono_object_new mono_object_new_impl
#define mono_array_length mono_array_length_impl
#define mono_object_isinst mono_object_isinst_impl
#define mono_object_get_class mono_object_get_class_impl
#define mono_object_get_virtual_method mono_object_get_virtual_method_impl
#define mono_field_get_value mono_field_get_value_impl
#define mono_value_box mono_value_box_impl
#define mono_gc_wbarrier_set_arrayref mono_gc_wbarrier_set_arrayref_impl
#define mono_profiler_install mono_profiler_install_impl
#define mono_profiler_install_thread mono_profiler_install_thread_impl
#define mono_profiler_install_gc mono_profiler_install_gc_impl
#define mono_profiler_load mono_profiler_load_impl
#define mono_thread_is_foreign mono_thread_is_foreign_impl
#define mono_thread_current mono_thread_current_impl
#define mono_thread_attach mono_thread_attach_impl
#define mono_thread_detach_if_exiting mono_thread_detach_if_exiting_impl
#define mono_runtime_set_pending_exception mono_runtime_set_pending_exception_impl
#define mono_set_assemblies_path mono_set_assemblies_path_impl
#define mono_assembly_open mono_assembly_open_impl
#define mono_assembly_get_image mono_assembly_get_image_impl
#define mono_assembly_name_new mono_assembly_name_new_impl
#define mono_assembly_name_free mono_assembly_name_free_impl
#define mono_assembly_loaded mono_assembly_loaded_impl
#define mono_register_machine_config mono_register_machine_config_impl
#define mono_set_dirs mono_set_dirs_impl
#define mono_assembly_name_get_name mono_assembly_name_get_name_impl
#define mono_assembly_name_get_culture mono_assembly_name_get_culture_impl
#define mono_install_assembly_preload_hook mono_install_assembly_preload_hook_impl
#define mono_assembly_get_name mono_assembly_get_name_impl
#define mono_add_internal_call mono_add_internal_call_impl
#define mono_dangerous_add_raw_internal_call mono_dangerous_add_raw_internal_call_impl
#define mono_method_signature mono_method_signature_impl
#define mono_method_get_class mono_method_get_class_impl
#define mono_dllmap_insert mono_dllmap_insert_impl
#define mono_domain_get mono_domain_get_impl
#define mono_get_intptr_class mono_get_intptr_class_impl
#define mono_get_string_class mono_get_string_class_impl
#define mono_get_corlib mono_get_corlib_impl
#define mono_get_array_class mono_get_array_class_impl
#define mono_get_exception_class mono_get_exception_class_impl
#define mono_get_root_domain mono_get_root_domain_impl
#define mono_domain_set_config mono_domain_set_config_impl
#define mono_assembly_get_object mono_assembly_get_object_impl
#define mono_method_get_object mono_method_get_object_impl
#define mono_type_get_object mono_type_get_object_impl
#define mono_reflection_type_get_type mono_reflection_type_get_type_impl
#define mono_signature_get_params mono_signature_get_params_impl
#define mono_type_is_byref mono_type_is_byref_impl
#define mono_signature_get_return_type mono_signature_get_return_type_impl
#define mono_type_get_type mono_type_get_type_impl
#define mono_debug_init mono_debug_init_impl
#define mono_gc_collect mono_gc_collect_impl
#define mono_is_debugger_attached mono_is_debugger_attached_impl
#define mono_config_parse_memory mono_config_parse_memory_impl
#define mono_gc_max_generation mono_gc_max_generation_impl
#define mono_g_hash_table_new_type mono_g_hash_table_new_type_impl
#define mono_g_hash_table_lookup mono_g_hash_table_lookup_impl
#define mono_g_hash_table_insert mono_g_hash_table_insert_impl
#define mono_get_exception_execution_engine mono_get_exception_execution_engine_impl
#define mono_exception_from_name_msg mono_exception_from_name_msg_impl
#define mono_gc_reference_queue_new mono_gc_reference_queue_new_impl
#define mono_gc_reference_queue_free mono_gc_reference_queue_free_impl
#define mono_gc_reference_queue_add mono_gc_reference_queue_add_impl
#define mono_gc_register_finalizer_callbacks mono_gc_register_finalizer_callbacks_impl
#define mono_gc_toggleref_add mono_gc_toggleref_add_impl
#define mono_gc_toggleref_register_callback mono_gc_toggleref_register_callback_impl
#define mono_path_resolve_symlinks mono_path_resolve_symlinks_impl
#define mono_free mono_free_impl
#define mono_get_runtime_build_info mono_get_runtime_build_info_impl
#define mono_jit_init_version mono_jit_init_version_impl
#define mono_jit_init mono_jit_init_impl
#define mono_jit_exec mono_jit_exec_impl
#define mono_jit_parse_options mono_jit_parse_options_impl
#define mono_jit_set_aot_mode mono_jit_set_aot_mode_impl
#define mono_set_signal_chaining mono_set_signal_chaining_impl
#define mono_set_crash_chaining mono_set_crash_chaining_impl
#define mono_jit_set_trace_options mono_jit_set_trace_options_impl
#define mono_jit_thread_attach mono_jit_thread_attach_impl
#define mono_exception_walk_trace mono_exception_walk_trace_impl
#define mono_install_unhandled_exception_hook mono_install_unhandled_exception_hook_impl
#define mono_main mono_main_impl
#define mono_install_load_aot_data_hook mono_install_load_aot_data_hook_impl
#define mini_parse_debug_option mini_parse_debug_option_impl
#define mono_trace_set_log_handler mono_trace_set_log_handler_impl
#define mono_trace_set_print_handler mono_trace_set_print_handler_impl
#define mono_trace_set_printerr_handler mono_trace_set_printerr_handler_impl
#define mono_threads_enter_gc_unsafe_region mono_threads_enter_gc_unsafe_region_impl
#define mono_threads_exit_gc_unsafe_region mono_threads_exit_gc_unsafe_region_impl
#define mono_threads_enter_gc_safe_region mono_threads_enter_gc_safe_region_impl
#define mono_threads_exit_gc_safe_region mono_threads_exit_gc_safe_region_impl
#define mono_threads_assert_gc_safe_region mono_threads_assert_gc_safe_region_impl
#define mono_threads_assert_gc_unsafe_region mono_threads_assert_gc_unsafe_region_impl
#define mono_threads_assert_gc_starting_region mono_threads_assert_gc_starting_region_impl
#define mono_thread_info_current_unchecked mono_thread_info_current_unchecked_impl
#define mono_threads_attach_coop mono_threads_attach_coop_impl
#define mono_threads_detach_coop mono_threads_detach_coop_impl
#define mono_install_ftnptr_eh_callback mono_install_ftnptr_eh_callback_impl
#endif

/* This is copied from mono's header files */

/* utils/mono-publib.h */
#define MONO_API
typedef int32_t	mono_bool;

/* metadata/image.h */
typedef struct _MonoAssembly MonoAssembly;
typedef struct _MonoAssemblyName MonoAssemblyName;
typedef struct _MonoImage MonoImage;

typedef enum {
	MONO_IMAGE_OK,
	MONO_IMAGE_ERROR_ERRNO,
	MONO_IMAGE_MISSING_ASSEMBLYREF,
	MONO_IMAGE_IMAGE_INVALID
} MonoImageOpenStatus;

/* metadata/metadata.h */
typedef struct _MonoClass MonoClass;
typedef struct _MonoDomain MonoDomain;
typedef struct _MonoMethod MonoMethod;
typedef struct _MonoMethodSignature MonoMethodSignature;
typedef struct _MonoType MonoType;

/* metadata/class.h */
typedef struct MonoVTable MonoVTable;

typedef struct _MonoClassField MonoClassField;

/* metadata/object.h */
typedef struct _MonoString MonoString;
typedef struct _MonoArray MonoArray;
typedef struct _MonoReflectionMethod MonoReflectionMethod;
typedef struct _MonoReflectionAssembly MonoReflectionAssembly;
typedef struct _MonoReflectionType MonoReflectionType;
typedef struct _MonoException MonoException;
typedef struct _MonoThread MonoThread;
typedef struct _MonoThreadsSync MonoThreadsSync;
typedef struct {
	MonoVTable *vtable;
	MonoThreadsSync *synchronisation;
} MonoObject;

typedef struct _MonoReferenceQueue MonoReferenceQueue;
typedef void (*mono_reference_queue_callback) (void *user_data);

#define mono_array_addr(array,type,index) ((type*)(void*) mono_array_addr_with_size (array, sizeof (type), index))
#define mono_array_get(array,type,index) ( *(type*)mono_array_addr ((array), type, (index)) )
#define mono_array_setref(array,index,value)	\
	do {	\
		void **__p = (void **) mono_array_addr ((array), void*, (index));	\
		mono_gc_wbarrier_set_arrayref ((array), __p, (MonoObject*)(value));	\
		/* *__p = (value);*/	\
	} while (0)

/* metadata/assembly.h */

typedef MonoAssembly * (*MonoAssemblyPreLoadFunc) (MonoAssemblyName *aname, char **assemblies_path, void* user_data);

/* metadata/profiler.h */
typedef struct _MonoProfiler MonoProfiler;

typedef enum {
	MONO_PROFILE_NONE = 0,
	MONO_PROFILE_APPDOMAIN_EVENTS = 1 << 0,
	MONO_PROFILE_ASSEMBLY_EVENTS  = 1 << 1,
	MONO_PROFILE_MODULE_EVENTS    = 1 << 2,
	MONO_PROFILE_CLASS_EVENTS     = 1 << 3,
	MONO_PROFILE_JIT_COMPILATION  = 1 << 4,
	MONO_PROFILE_INLINING         = 1 << 5,
	MONO_PROFILE_EXCEPTIONS       = 1 << 6,
	MONO_PROFILE_ALLOCATIONS      = 1 << 7,
	MONO_PROFILE_GC               = 1 << 8,
	MONO_PROFILE_THREADS          = 1 << 9,
	MONO_PROFILE_REMOTING         = 1 << 10,
	MONO_PROFILE_TRANSITIONS      = 1 << 11,
	MONO_PROFILE_ENTER_LEAVE      = 1 << 12,
	MONO_PROFILE_COVERAGE         = 1 << 13,
	MONO_PROFILE_INS_COVERAGE     = 1 << 14,
	MONO_PROFILE_STATISTICAL      = 1 << 15,
	MONO_PROFILE_METHOD_EVENTS    = 1 << 16,
	MONO_PROFILE_MONITOR_EVENTS   = 1 << 17,
	MONO_PROFILE_IOMAP_EVENTS     = 1 << 18, /* this should likely be removed, too */
	MONO_PROFILE_GC_MOVES         = 1 << 19,
	MONO_PROFILE_GC_ROOTS         = 1 << 20
} MonoProfileFlags;

typedef enum {
	MONO_GC_EVENT_START,
	MONO_GC_EVENT_MARK_START,
	MONO_GC_EVENT_MARK_END,
	MONO_GC_EVENT_RECLAIM_START,
	MONO_GC_EVENT_RECLAIM_END,
	MONO_GC_EVENT_END,
	MONO_GC_EVENT_PRE_STOP_WORLD,
	MONO_GC_EVENT_POST_STOP_WORLD,
	MONO_GC_EVENT_PRE_START_WORLD,
	MONO_GC_EVENT_POST_START_WORLD
} MonoGCEvent;

typedef void (*MonoProfileFunc) (MonoProfiler *prof);
typedef void (*MonoProfileThreadFunc) (MonoProfiler *prof, uintptr_t tid);
typedef void (*MonoProfileGCFunc)         (MonoProfiler *prof, MonoGCEvent event, int generation);
typedef void (*MonoProfileGCResizeFunc)   (MonoProfiler *prof, int64_t new_size);

/* metadata/mono-debug.h */

typedef enum {
	MONO_DEBUG_FORMAT_NONE,
	MONO_DEBUG_FORMAT_MONO,
	/* Deprecated, the mdb debugger is not longer supported. */
	MONO_DEBUG_FORMAT_DEBUGGER
} MonoDebugFormat;

/* metadata/mini.h */
typedef gboolean (*MonoExceptionFrameWalk)      (MonoMethod *method, gpointer ip, size_t native_offset, gboolean managed, gpointer user_data);
typedef void  (*MonoUnhandledExceptionFunc)         (MonoObject *exc, gpointer user_data);

typedef unsigned char* (*MonoLoadAotDataFunc)          (MonoAssembly *assembly, int size, gpointer user_data, void **out_handle);
typedef void  (*MonoFreeAotDataFunc)          (MonoAssembly *assembly, int size, gpointer user_data, void *handle);

/* metadata/blob.h */

/*
 * Encoding for type signatures used in the Metadata
 */
typedef enum {
	MONO_TYPE_END        = 0x00,       /* End of List */
	MONO_TYPE_VOID       = 0x01,
	MONO_TYPE_BOOLEAN    = 0x02,
	MONO_TYPE_CHAR       = 0x03,
	MONO_TYPE_I1         = 0x04,
	MONO_TYPE_U1         = 0x05,
	MONO_TYPE_I2         = 0x06,
	MONO_TYPE_U2         = 0x07,
	MONO_TYPE_I4         = 0x08,
	MONO_TYPE_U4         = 0x09,
	MONO_TYPE_I8         = 0x0a,
	MONO_TYPE_U8         = 0x0b,
	MONO_TYPE_R4         = 0x0c,
	MONO_TYPE_R8         = 0x0d,
	MONO_TYPE_STRING     = 0x0e,
	MONO_TYPE_PTR        = 0x0f,       /* arg: <type> token */
	MONO_TYPE_BYREF      = 0x10,       /* arg: <type> token */
	MONO_TYPE_VALUETYPE  = 0x11,       /* arg: <type> token */
	MONO_TYPE_CLASS      = 0x12,       /* arg: <type> token */
	MONO_TYPE_VAR	     = 0x13,	   /* number */
	MONO_TYPE_ARRAY      = 0x14,       /* type, rank, boundsCount, bound1, loCount, lo1 */
	MONO_TYPE_GENERICINST= 0x15,	   /* <type> <type-arg-count> <type-1> \x{2026} <type-n> */
	MONO_TYPE_TYPEDBYREF = 0x16,
	MONO_TYPE_I          = 0x18,
	MONO_TYPE_U          = 0x19,
	MONO_TYPE_FNPTR      = 0x1b,	      /* arg: full method signature */
	MONO_TYPE_OBJECT     = 0x1c,
	MONO_TYPE_SZARRAY    = 0x1d,       /* 0-based one-dim-array */
	MONO_TYPE_MVAR	     = 0x1e,       /* number */
	MONO_TYPE_CMOD_REQD  = 0x1f,       /* arg: typedef or typeref token */
	MONO_TYPE_CMOD_OPT   = 0x20,       /* optional arg: typedef or typref token */
	MONO_TYPE_INTERNAL   = 0x21,       /* CLR internal type */

	MONO_TYPE_MODIFIER   = 0x40,       /* Or with the following types */
	MONO_TYPE_SENTINEL   = 0x41,       /* Sentinel for varargs method signature */
	MONO_TYPE_PINNED     = 0x45,       /* Local var that points to pinned object */

	MONO_TYPE_ENUM       = 0x55        /* an enumeration */
} MonoTypeEnum;

/*
 * From internal headers
 */

/* metadata/gc-internal.h */

enum {
   MONO_GC_FINALIZER_EXTENSION_VERSION = 1,
};

typedef struct {
	int version;
	gboolean (*is_class_finalization_aware) (MonoClass *klass);
	void (*object_queued_for_finalization) (MonoObject *object);
} MonoGCFinalizerCallbacks;

/* metadata/sgen-toggleref.h */

typedef enum {
	MONO_TOGGLE_REF_DROP,
	MONO_TOGGLE_REF_STRONG,
	MONO_TOGGLE_REF_WEAK
} MonoToggleRefStatus;

typedef MonoToggleRefStatus (*MonoToggleRefCallback) (MonoObject *obj);

/* metadata/mono-hash.h */

typedef enum {
	MONO_HASH_KEY_GC = 1,
	MONO_HASH_VALUE_GC = 2,
	MONO_HASH_KEY_VALUE_GC = MONO_HASH_KEY_GC | MONO_HASH_VALUE_GC,
} MonoGHashGCType;

typedef struct _MonoGHashTable MonoGHashTable;

/* utils/mono-logger.h */

typedef void (*MonoLogCallback) (const char *log_domain, const char *log_level, const char *message, mono_bool fatal, void *user_data);
typedef void (*MonoPrintCallback) (const char *string, mono_bool is_stdout);

/* mini/jit.h */
typedef enum {
	MONO_AOT_MODE_NONE,
	MONO_AOT_MODE_NORMAL,
	MONO_AOT_MODE_HYBRID,
	MONO_AOT_MODE_FULL,
	MONO_AOT_MODE_LLVMONLY,
	MONO_AOT_MODE_INTERP,
	MONO_AOT_MODE_INTERP_LLVMONLY,
} MonoAotMode;

/* metadata/marshal.h */

typedef void (*MonoFtnPtrEHCallback) (guint32 gchandle);

/* not in any header */

void mono_gc_init_finalizer_thread ();

/*
 * The functions we want to expose
 */


MONO_API MonoClass *
mono_class_from_name (MonoImage * image, const char * name_space, const char * name);

MONO_API MonoMethod *
mono_class_get_method_from_name (MonoClass * klass, const char * name, int param_count);

MONO_API MonoClassField *
mono_class_get_field_from_name (MonoClass * klass, const char * name);

MONO_API mono_bool
mono_class_is_assignable_from (MonoClass * klass, MonoClass * oklass);

MONO_API MonoClass *
mono_class_from_mono_type (MonoType * type);

MONO_API mono_bool
mono_class_is_delegate (MonoClass * klass);

MONO_API MonoClass *
mono_class_get_element_class (MonoClass * klass);

MONO_API const char *
mono_class_get_namespace (MonoClass * klass);

MONO_API const char *
mono_class_get_name (MonoClass * klass);

MONO_API MonoClass *
mono_class_get_parent (MonoClass * klass);

MONO_API mono_bool
mono_class_is_subclass_of (MonoClass * klass, MonoClass * klassc, mono_bool check_interfaces);

MONO_API mono_bool
mono_class_is_valuetype (MonoClass * klass);

MONO_API mono_bool
mono_class_is_enum (MonoClass * klass);

MONO_API MonoType *
mono_class_enum_basetype (MonoClass * klass);

MONO_API int32_t
mono_class_value_size (MonoClass * klass, uint32_t * align);

MONO_API MonoType *
mono_class_get_type (MonoClass * klass);

MONO_API gboolean
mono_class_is_nullable (MonoClass * klass);

MONO_API MonoClass *
mono_class_get_nullable_param (MonoClass * klass);

MONO_API char *
mono_method_full_name (MonoMethod * method, mono_bool signature);

MONO_API MonoObject *
mono_runtime_invoke (MonoMethod * method, void * obj, void ** params, MonoObject ** exc);

MONO_API uint32_t
mono_gchandle_new (MonoObject * obj, mono_bool pinned);

MONO_API MonoObject *
mono_gchandle_get_target (uint32_t gchandle);

MONO_API void
mono_gchandle_free (uint32_t gchandle);

MONO_API uint32_t
mono_gchandle_new_weakref (MonoObject * obj, mono_bool track_resurrection);

MONO_API void
mono_raise_exception (MonoException * ex);

MONO_API char*
mono_array_addr_with_size (MonoArray * array, int size, uintptr_t idx);

MONO_API MonoString *
mono_string_new (MonoDomain * domain, const char * text);

MONO_API MonoArray *
mono_array_new (MonoDomain * domain, MonoClass * eclass, uintptr_t n);

MONO_API void *
mono_object_unbox (MonoObject * obj);

MONO_API char *
mono_string_to_utf8 (MonoString * string_obj);

MONO_API MonoObject *
mono_object_new (MonoDomain * domain, MonoClass * klass);

MONO_API uintptr_t
mono_array_length (MonoArray * array);

MONO_API MonoObject *
mono_object_isinst (MonoObject * obj, MonoClass * klass);

MONO_API MonoClass *
mono_object_get_class (MonoObject * obj);

MONO_API MonoMethod *
mono_object_get_virtual_method (MonoObject * obj, MonoMethod * method);

MONO_API void
mono_field_get_value (MonoObject * obj, MonoClassField * field, void * value);

MONO_API MonoObject *
mono_value_box (MonoDomain * domain, MonoClass * klass, void * val);

MONO_API void
mono_gc_wbarrier_set_arrayref (MonoArray * arr, void * slot_ptr, MonoObject * value);

MONO_API void
mono_profiler_install (MonoProfiler * prof, MonoProfileFunc shutdown_callback);

MONO_API void
mono_profiler_install_thread (MonoProfileThreadFunc start, MonoProfileThreadFunc end);

MONO_API void
mono_profiler_install_gc (MonoProfileGCFunc callback, MonoProfileGCResizeFunc heap_resize_callback);

MONO_API void
mono_profiler_load (const char * desc);

MONO_API mono_bool
mono_thread_is_foreign (MonoThread * thread);

MONO_API MonoThread * 
mono_thread_current (void);

MONO_API MonoThread *
mono_thread_attach (MonoDomain * domain);

MONO_API mono_bool
mono_thread_detach_if_exiting (void);

MONO_API void
mono_runtime_set_pending_exception (MonoException * exc, mono_bool overwrite);

MONO_API void
mono_set_assemblies_path (const char * path);

MONO_API MonoAssembly *
mono_assembly_open (const char * filename, MonoImageOpenStatus * status);

MONO_API MonoImage *
mono_assembly_get_image (MonoAssembly * assembly);

MONO_API MonoAssemblyName *
mono_assembly_name_new (const char * name);

MONO_API void
mono_assembly_name_free (MonoAssemblyName * aname);

MONO_API MonoAssembly *
mono_assembly_loaded (MonoAssemblyName * aname);

MONO_API void
mono_register_machine_config (const char * config_xml);

MONO_API void
mono_set_dirs (const char * assembly_dir, const char * config_dir);

MONO_API const char *
mono_assembly_name_get_name (MonoAssemblyName * aname);

MONO_API const char *
mono_assembly_name_get_culture (MonoAssemblyName * aname);

MONO_API void
mono_install_assembly_preload_hook (MonoAssemblyPreLoadFunc func, void * user_data);

MONO_API MonoAssemblyName *
mono_assembly_get_name (MonoAssembly * assembly);

MONO_API void
mono_add_internal_call (const char * name, const void * method);

MONO_API void
mono_dangerous_add_raw_internal_call (const char * name, const void * method);

MONO_API MonoMethodSignature *
mono_method_signature (MonoMethod * method);

MONO_API MonoClass *
mono_method_get_class (MonoMethod * method);

MONO_API void
mono_dllmap_insert (MonoImage * assembly, const char * dll, const char * func, const char * tdll, const char * tfunc);

MONO_API MonoDomain *
mono_domain_get (void);

MONO_API MonoClass *
mono_get_intptr_class (void);

MONO_API MonoClass *
mono_get_string_class (void);

MONO_API MonoImage *
mono_get_corlib (void);

MONO_API MonoClass *
mono_get_array_class (void);

MONO_API MonoClass *
mono_get_exception_class (void);

MONO_API MonoDomain *
mono_get_root_domain (void);

MONO_API void
mono_domain_set_config (MonoDomain * domain, const char * base_dir, const char * config_file_name);

MONO_API MonoReflectionAssembly *
mono_assembly_get_object (MonoDomain * domain, MonoAssembly * assembly);

MONO_API MonoReflectionMethod *
mono_method_get_object (MonoDomain * domain, MonoMethod * method, MonoClass * refclass);

MONO_API MonoReflectionType *
mono_type_get_object (MonoDomain * domain, MonoType * type);

MONO_API MonoType *
mono_reflection_type_get_type (MonoReflectionType * reftype);

MONO_API MonoType *
mono_signature_get_params (MonoMethodSignature * sig, void ** iter);

MONO_API mono_bool
mono_type_is_byref (MonoType * type);

MONO_API MonoType *
mono_signature_get_return_type (MonoMethodSignature * sig);

MONO_API int
mono_type_get_type (MonoType * type);

MONO_API void
mono_debug_init (MonoDebugFormat format);

MONO_API void
mono_gc_collect (int generation);

MONO_API mono_bool
mono_is_debugger_attached (void);

MONO_API void
mono_config_parse_memory (const char * buffer);

MONO_API int
mono_gc_max_generation (void);

MONO_API MonoGHashTable *
mono_g_hash_table_new_type (GHashFunc hash_func, GEqualFunc key_equal_func, MonoGHashGCType type);

MONO_API gpointer
mono_g_hash_table_lookup (MonoGHashTable * hash, gconstpointer key);

MONO_API void
mono_g_hash_table_insert (MonoGHashTable * hash, gpointer k, gpointer v);

MONO_API MonoException *
mono_get_exception_execution_engine (const char * msg);

MONO_API MonoException *
mono_exception_from_name_msg (MonoImage * image, const char * name_space, const char * name, const char * msg);

MONO_API MonoReferenceQueue *
mono_gc_reference_queue_new (mono_reference_queue_callback callback);

MONO_API void
mono_gc_reference_queue_free (MonoReferenceQueue * queue);

MONO_API gboolean
mono_gc_reference_queue_add (MonoReferenceQueue * queue, MonoObject * obj, void * user_data);

MONO_API void
mono_gc_register_finalizer_callbacks (MonoGCFinalizerCallbacks * callbacks);

MONO_API void
mono_gc_toggleref_add (MonoObject * object, mono_bool strong_ref);

MONO_API void
mono_gc_toggleref_register_callback (MonoToggleRefCallback process_toggleref);

MONO_API gchar *
mono_path_resolve_symlinks (const char * path);

MONO_API void
mono_free (void * ptr);

MONO_API char *
mono_get_runtime_build_info (void);

MONO_API MonoDomain *
mono_jit_init_version (const char * root_domain_name, const char * runtime_version);

MONO_API MonoDomain *
mono_jit_init (const char * file);

MONO_API int
mono_jit_exec (MonoDomain * domain, MonoAssembly * assembly, int argc, const char** argv);

MONO_API void
mono_jit_parse_options (int argc, char** argv);

MONO_API void
mono_jit_set_aot_mode (MonoAotMode mode);

MONO_API void
mono_set_signal_chaining (mono_bool chain_signals);

MONO_API void
mono_set_crash_chaining (mono_bool chain_signals);

MONO_API void
mono_jit_set_trace_options (const char * option);

MONO_API void*
mono_jit_thread_attach (MonoDomain * domain);

MONO_API gboolean
mono_exception_walk_trace (MonoException * exc, MonoExceptionFrameWalk func, gpointer user_data);

MONO_API void
mono_install_unhandled_exception_hook (MonoUnhandledExceptionFunc func, gpointer user_data);

MONO_API int
mono_main (int argc, char ** argv);

MONO_API void
mono_install_load_aot_data_hook (MonoLoadAotDataFunc load_func, MonoFreeAotDataFunc free_func, gpointer user_data);

MONO_API gboolean
mini_parse_debug_option (const char * option);

MONO_API void
mono_trace_set_log_handler (MonoLogCallback callback, void * user_data);

MONO_API void
mono_trace_set_print_handler (MonoPrintCallback callback);

MONO_API void
mono_trace_set_printerr_handler (MonoPrintCallback callback);

MONO_API void*
mono_threads_enter_gc_unsafe_region (void ** stackdata);

MONO_API void
mono_threads_exit_gc_unsafe_region (void * cookie, void ** stackdata);

MONO_API void*
mono_threads_enter_gc_safe_region (void ** stackdata);

MONO_API void
mono_threads_exit_gc_safe_region (void * cookie, void ** stackdata);

MONO_API void
mono_threads_assert_gc_safe_region (void);

MONO_API void
mono_threads_assert_gc_unsafe_region (void);

MONO_API void
mono_threads_assert_gc_starting_region (void);

MONO_API void*
mono_thread_info_current_unchecked (void);

MONO_API void *
mono_threads_attach_coop (MonoDomain * domain, gpointer* dummy);

MONO_API void *
mono_threads_detach_coop (gpointer cookie, gpointer* dummy);

MONO_API void
mono_install_ftnptr_eh_callback (MonoFtnPtrEHCallback callback);

bool
mono_class_is_nullable_exists ();

bool
mono_class_get_nullable_param_exists ();

bool
mono_runtime_set_pending_exception_exists ();

bool
mono_dangerous_add_raw_internal_call_exists ();

bool
mono_install_load_aot_data_hook_exists ();

bool
mini_parse_debug_option_exists ();

bool
mono_threads_enter_gc_unsafe_region_exists ();

bool
mono_threads_exit_gc_unsafe_region_exists ();

bool
mono_threads_enter_gc_safe_region_exists ();

bool
mono_threads_exit_gc_safe_region_exists ();

bool
mono_threads_assert_gc_safe_region_exists ();

bool
mono_threads_assert_gc_unsafe_region_exists ();

bool
mono_threads_assert_gc_starting_region_exists ();

bool
mono_thread_info_current_unchecked_exists ();

bool
mono_threads_attach_coop_exists ();

bool
mono_threads_detach_coop_exists ();

bool
mono_install_ftnptr_eh_callback_exists ();

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __MONO_RUNTIME__ */
