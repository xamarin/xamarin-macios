/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 *  Authors: Rolf Bjarne Kvinge
 *
 *  Copyright (C) 2014 Xamarin Inc. (www.xamarin.com)
 *
 */

#ifndef __TRAMPOLINES_H__
#define __TRAMPOLINES_H__

#ifdef __cplusplus
extern "C" {
#endif

// Must be kept in sync with the same enum in NSObject2.cs
enum XamarinGCHandleFlags : uint32_t {
	XamarinGCHandleFlags_None = 0,
	XamarinGCHandleFlags_WeakGCHandle = 1,
	XamarinGCHandleFlags_HasManagedRef = 2,
	XamarinGCHandleFlags_InitialSet = 4,
};

void *		xamarin_trampoline (id self, SEL sel, ...);
void		xamarin_stret_trampoline (void *buffer, id self, SEL sel, ...);
float		xamarin_fpret_single_trampoline (id self, SEL sel, ...);
double		xamarin_fpret_double_trampoline (id self, SEL sel, ...);
void		xamarin_release_trampoline (id self, SEL sel);
void		xamarin_calayer_release_trampoline (id self, SEL sel);
id			xamarin_retain_trampoline (id self, SEL sel);
void		xamarin_dealloc_trampoline (id self, SEL sel);
void *		xamarin_static_trampoline (id self, SEL sel, ...);
void *		xamarin_ctor_trampoline (id self, SEL sel, ...);
void		xamarin_x86_double_abi_stret_trampoline ();
float		xamarin_static_fpret_single_trampoline (id self, SEL sel, ...);
double		xamarin_static_fpret_double_trampoline (id self, SEL sel, ...);
void		xamarin_static_stret_trampoline (void *buffer, id self, SEL sel, ...);
void		xamarin_static_x86_double_abi_stret_trampoline ();
long long	xamarin_longret_trampoline (id self, SEL sel, ...);
long long	xamarin_static_longret_trampoline (id self, SEL sel, ...);
id			xamarin_copyWithZone_trampoline1 (id self, SEL sel, NSZone *zone);
id			xamarin_copyWithZone_trampoline2 (id self, SEL sel, NSZone *zone);
GCHandle	xamarin_get_gchandle_trampoline (id self, SEL sel);
bool		xamarin_set_gchandle_trampoline (id self, SEL sel, GCHandle gc_handle, enum XamarinGCHandleFlags flags);
enum XamarinGCHandleFlags xamarin_get_flags_trampoline (id self, SEL sel);
void		xamarin_set_flags_trampoline (id self, SEL sel, enum XamarinGCHandleFlags flags);

int 		xamarin_get_frame_length (id self, SEL sel);
bool		xamarin_collapse_struct_name (const char *type, char struct_name[], int max_char, GCHandle *exception_gchandle);
GCHandle	xamarin_create_mt_exception (char *msg);
size_t		xamarin_get_primitive_size (char type);

enum ArgumentSemantic /* Xcode 4.4 doesn't like this ': int' */ {
	ArgumentSemanticNone   = -1,
	ArgumentSemanticAssign = 0,
	ArgumentSemanticCopy   = 1,
	ArgumentSemanticRetain = 2,
	ArgumentSemanticMask = ArgumentSemanticAssign | ArgumentSemanticCopy | ArgumentSemanticRetain,
	ArgumentSemanticRetainReturnValue = 1 << 10,
	ArgumentSemanticCategoryInstance = 1 << 11,
};

/* Conversion functions */

// The xamarin_id_to_managed_func and xamarin_managed_to_id_func typedefs
// represents functions to convert to/from id and managed types.
//
// The `value` parameter is the value to convert.
//
// The `ptr` parameter must not be passed if the managed type is a class. If
// the managed type is a value type, `ptr` is optional, and if passed the
// resulting value will be stored here. If NULL, memory is allocated and
// returned, and the return value must be freed using `xamarin_free`.
//
// The `managedType` parameter is the managed type to convert to.
//
// The `context` parameter is a conversion-specific value that may or may not
// be provided:
// * Smart enum conversions: The `context` parameter represents a token ref to
//   the conversion method. The static registrar bakes those token refs in to
//   the generated code, thus avoiding the need for finding the conversion
//   method at runtime).
// * Other conversions: The `context` parameter is not used in other
//   conversions at the moment.
//
// The `exception_gchandle` parameter is required, and will contain a GCHandle
// to any exceptions that occur.
//
// The return value is:
// * xamarin_id_to_managed_func: a pointer to the resulting value. If
//   `ptr` was passed, this value is also returned, otherwise newly allocated
//   memory is returned (which must be freed with `xamarin_free`). If an
//   exception occurs, 'ptr' is returned (and no memory allocated).
//   If the return value is a MonoObject*, then it's a retained MonoObject*.
// * xamarin_managed_to_id_func: the resulting Objective-C object.

typedef void *	(*xamarin_id_to_managed_func) (id value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
typedef id		(*xamarin_managed_to_id_func) (MonoObject *value, void *context, GCHandle *exception_gchandle);

id              xamarin_generate_conversion_to_native (MonoObject *value, MonoType *inputType, MonoType *outputType, MonoMethod *method, void *context, GCHandle *exception_gchandle);
void *          xamarin_generate_conversion_to_managed (id value, MonoType *inputType, MonoType *outputType, MonoMethod *method, GCHandle *exception_gchandle, void *context, /*SList*/ void **free_list, /*SList*/ void **release_list_ptr);
NSNumber *      xamarin_convert_managed_to_nsnumber (MonoObject *value, MonoClass *managedType, MonoMethod *method, void *context, GCHandle *exception_gchandle);
NSValue *       xamarin_convert_managed_to_nsvalue (MonoObject *value, MonoClass *managedType, MonoMethod *method, void *context, GCHandle *exception_gchandle);
NSString *      xamarin_convert_managed_to_nsstring (MonoObject *value, MonoType *managedType, MonoType *nativeType, MonoMethod *method, GCHandle *exception_gchandle);
MonoObject *    xamarin_convert_nsnumber_to_managed (NSNumber *value, MonoType *nativeType, MonoType *managedType, MonoMethod *method, GCHandle *exception_gchandle);
MonoObject *    xamarin_convert_nsvalue_to_managed (NSValue *value, MonoType *nativeType, MonoType *managedType, MonoMethod *method, GCHandle *exception_gchandle);
MonoObject *    xamarin_convert_nsstring_to_managed (NSString *value, MonoType *nativeType, MonoType *managedType, MonoMethod *method, GCHandle *exception_gchandle);
GCHandle        xamarin_create_bindas_exception (MonoType *inputType, MonoType *outputType, MonoMethod *method);
GCHandle        xamarin_get_exception_for_parameter (int code, GCHandle inner_exception_gchandle, const char *reason, SEL sel, MonoMethod *method, MonoType *p, int i, bool to_managed);

xamarin_id_to_managed_func xamarin_get_nsnumber_to_managed_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle);
xamarin_managed_to_id_func xamarin_get_managed_to_nsnumber_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle);

xamarin_id_to_managed_func xamarin_get_nsvalue_to_managed_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle);
xamarin_managed_to_id_func xamarin_get_managed_to_nsvalue_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle);

xamarin_id_to_managed_func xamarin_get_nsstring_to_smart_enum_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle);
xamarin_managed_to_id_func xamarin_get_smart_enum_to_nsstring_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle);

NSArray *   xamarin_convert_managed_to_nsarray_with_func (MonoArray *array, xamarin_managed_to_id_func convert, void *context, GCHandle *exception_gchandle);
MonoArray * xamarin_convert_nsarray_to_managed_with_func (NSArray *array, MonoClass *managedElementType, xamarin_id_to_managed_func convert, void *context, GCHandle *exception_gchandle);

void * xamarin_nsstring_to_smart_enum (id value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void * xamarin_smart_enum_to_nsstring (MonoObject *value, void *context /* token ref */, GCHandle *exception_gchandle);

// Returns a pointer to the value type, which must be freed using xamarin_free.
void *xamarin_nsnumber_to_bool   (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_sbyte  (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_byte   (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_short  (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_ushort (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_int    (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_uint   (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_long   (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_ulong  (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_nint   (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_nuint  (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_float  (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_double (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsnumber_to_nfloat (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);

// Returns a pointer to the value type, which must be freed using xamarin_free
void *xamarin_nsvalue_to_nsrange                (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_cgaffinetransform      (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_cgpoint                (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_cgrect                 (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_cgsize                 (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_cgvector               (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_catransform3d          (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_cllocationcoordinate2d (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_cmtime                 (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_cmtimemapping          (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_cmtimerange            (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_cmvideodimensions      (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_mkcoordinatespan       (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_scnmatrix4             (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_scnvector3             (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_scnvector4             (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_uiedgeinsets           (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_uioffset               (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void *xamarin_nsvalue_to_nsdirectionaledgeinsets(NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);

id xamarin_bool_to_nsnumber   (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_sbyte_to_nsnumber  (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_byte_to_nsnumber   (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_short_to_nsnumber  (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_ushort_to_nsnumber (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_int_to_nsnumber    (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_uint_to_nsnumber   (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_long_to_nsnumber   (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_ulong_to_nsnumber  (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_nint_to_nsnumber   (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_nuint_to_nsnumber  (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_float_to_nsnumber  (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_double_to_nsnumber (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_nfloat_to_nsnumber (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_nfloat_to_nsnumber (MonoObject *value, void *context, GCHandle *exception_gchandle);

id xamarin_nsrange_to_nsvalue                (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_cgaffinetransform_to_nsvalue      (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_cgpoint_to_nsvalue                (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_cgrect_to_nsvalue                 (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_cgsize_to_nsvalue                 (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_cgvector_to_nsvalue               (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_catransform3d_to_nsvalue          (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_cllocationcoordinate2d_to_nsvalue (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_cmtime_to_nsvalue                 (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_cmtimemapping_to_nsvalue          (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_cmtimerange_to_nsvalue            (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_cmvideodimensions_to_nsvalue      (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_mkcoordinatespan_to_nsvalue       (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_scnmatrix4_to_nsvalue             (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_scnvector3_to_nsvalue             (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_scnvector4_to_nsvalue             (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_uiedgeinsets_to_nsvalue           (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_uioffset_to_nsvalue               (MonoObject *value, void *context, GCHandle *exception_gchandle);
id xamarin_nsdirectionaledgeinsets_to_nsvalue(MonoObject *value, void *context, GCHandle *exception_gchandle);

// These functions can be passed as xamarin_id_to_managed_func/xamarin_managed_to_id_func parameters
id           xamarin_convert_string_to_nsstring (MonoObject *obj, void *context, GCHandle *exception_gchandle);
void *       xamarin_convert_nsstring_to_string (id value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);

// These are simpler versions of the above string<->nsstring conversion functions.
NSString *   xamarin_string_to_nsstring (MonoString *obj, bool retain);
// domain is optional, if NULL the function will call mono_get_domain.
MonoString * xamarin_nsstring_to_string (MonoDomain *domain, NSString *obj);

// Either managed_type or managed_class has to be provided
NSArray *   xamarin_managed_array_to_nsarray (MonoArray *array, MonoType *managed_type, MonoClass *managed_class, GCHandle *exception_gchandle);
// Either managed_type or managed_class has to be provided
MonoArray * xamarin_nsarray_to_managed_array (NSArray *array, MonoType *managed_type, MonoClass *managed_class, GCHandle *exception_gchandle);

NSArray *   xamarin_managed_string_array_to_nsarray (MonoArray *array, GCHandle *exception_gchandle);
NSArray *   xamarin_managed_nsobject_array_to_nsarray (MonoArray *array, GCHandle *exception_gchandle);
NSArray *   xamarin_managed_inativeobject_array_to_nsarray (MonoArray *array, GCHandle *exception_gchandle);

MonoArray * xamarin_nsarray_to_managed_string_array (NSArray *array, GCHandle *exception_gchandle);
MonoArray * xamarin_nsarray_to_managed_nsobject_array (NSArray *array, MonoType *array_type, MonoClass *element_class, GCHandle *exception_gchandle);
MonoArray * xamarin_nsarray_to_managed_inativeobject_array (NSArray *array, MonoType *array_type, MonoClass *element_class, GCHandle *exception_gchandle);
MonoArray * xamarin_nsarray_to_managed_inativeobject_array_static (NSArray *array, MonoType *array_type, MonoClass *element_class, uint32_t iface_token_ref, uint32_t implementation_token_ref, GCHandle *exception_gchandle);

void * xamarin_nsobject_to_object (id object, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
id     xamarin_object_to_nsobject (MonoObject *object, void *context, GCHandle *exception_gchandle);

id     xamarin_inativeobject_to_nsobject (MonoObject *object, void *context, GCHandle *exception_gchandle);

void * xamarin_nsobject_to_inativeobject (id object, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);
void * xamarin_nsobject_to_inativeobject_static (id object, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle);

/* Copied from SGen */

static inline void
mt_dummy_use (void *v) {
#if defined(__GNUC__)
	__asm__ volatile ("" : "=r"(v) : "r"(v));
#elif defined(_MSC_VER)
	__asm {
		mov eax, v;
		and eax, eax;
	};
#else
#error "Implement mt_dummy_use for your compiler"
#endif
}

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __TRAMPOLINES_H__ */
