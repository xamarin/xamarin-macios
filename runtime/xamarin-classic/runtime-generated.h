// vim: set filetype=c :
//
// delegates.h:
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2018 Microsoft Inc.
//

/* Functions calling into ObjCRuntime.Runtime */


#ifndef __RUNTIME_GENERATED_H__
#define __RUNTIME_GENERATED_H__

#include "xamarin.h"

#ifdef __cplusplus
extern "C" {
#endif

void
xamarin_throw_ns_exception (NSException * exc);

void
xamarin_rethrow_managed_exception (guint32 original_exception_gchandle, guint32 *exception_gchandle);

int
xamarin_create_ns_exception (NSException * exc, guint32 *exception_gchandle);

NSException *
xamarin_unwrap_ns_exception (int exc_handle, guint32 *exception_gchandle);

MonoObject *
xamarin_create_block_proxy (MonoObject * method, void * block, guint32 *exception_gchandle);

id
xamarin_create_delegate_proxy (MonoObject * method, MonoObject * block, const char * signature, unsigned int token_ref, guint32 *exception_gchandle);

MonoObject *
xamarin_get_class (Class ptr, guint32 *exception_gchandle);

MonoObject *
xamarin_get_selector (SEL ptr, guint32 *exception_gchandle);

bool
xamarin_has_nsobject (id obj, guint32 *exception_gchandle);

id
xamarin_get_handle_for_inativeobject (MonoObject * obj, guint32 *exception_gchandle);

void
xamarin_unregister_nsobject (id native_obj, MonoObject * managed_obj, guint32 *exception_gchandle);

MonoObject *
xamarin_try_get_or_construct_nsobject (id obj, guint32 *exception_gchandle);

MonoObject *
xamarin_get_inative_object_dynamic (id obj, bool owns, void * type, guint32 *exception_gchandle);

MonoReflectionMethod *
xamarin_get_method_from_token (unsigned int token_ref, guint32 *exception_gchandle);

MonoReflectionMethod *
xamarin_get_generic_method_from_token (MonoObject * obj, unsigned int token_ref, guint32 *exception_gchandle);

MonoObject *
xamarin_get_inative_object_static (id obj, bool owns, unsigned int iface_token_ref, unsigned int implementation_token_ref, guint32 *exception_gchandle);

MonoObject *
xamarin_get_nsobject_with_type (id obj, void * type, int32_t * created, SEL selector, MonoReflectionMethod * method, guint32 *exception_gchandle);

void
xamarin_dispose (MonoObject * mobj, guint32 *exception_gchandle);

guint32
xamarin_create_product_exception_for_error (int code, const char * message, guint32 *exception_gchandle);

char *
xamarin_reflection_type_get_full_name (MonoReflectionType * type, guint32 *exception_gchandle);

char *
xamarin_lookup_managed_type_name (Class klass, guint32 *exception_gchandle);

enum MarshalManagedExceptionMode
xamarin_on_marshal_managed_exception (int exception, guint32 *exception_gchandle);

enum MarshalObjectiveCExceptionMode
xamarin_on_marshal_objectivec_exception (id exception, bool throwManagedAsDefault, guint32 *exception_gchandle);

int32_t
xamarin_create_runtime_exception (int32_t code, const char * message, guint32 *exception_gchandle);


#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __RUNTIME_GENERATED_H__ */
