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
int			xamarin_get_gchandle_trampoline (id self, SEL sel);
void		xamarin_set_gchandle_trampoline (id self, SEL sel, int gc_handle);

int			xamarin_get_frame_length (id self, SEL sel);

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

// Function to convert from id to managed. If `ptr` is passed, the value type
// will be stored in this pointer. Otherwise a memory is allocated, and the
// return value must be freed using xamarin_free.
// Returns: a pointer to the value type. In case of an exception, 'ptr' is returned (and no memory allocated in any circumstances).
typedef void *	(*xamarin_id_to_managed_func) (id value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
// Function to convert from managed to id.
typedef id		(*xamarin_managed_to_id_func) (MonoObject *value, guint32 context, guint32 *exception_gchandle);

id              xamarin_generate_conversion_to_native (MonoObject *value, MonoType *inputType, MonoType *outputType, MonoMethod *method, guint32 context, guint32 *exception_gchandle);
void *          xamarin_generate_conversion_to_managed (id value, MonoType *inputType, MonoType *outputType, MonoMethod *method, guint32 *exception_gchandle, guint32 context, /*SList*/ void **free_list);
NSNumber *      xamarin_convert_managed_to_nsnumber (MonoObject *value, MonoType *managedType, MonoType *nativeType, MonoMethod *method, guint32 *exception_gchandle);
NSValue *       xamarin_convert_managed_to_nsvalue (MonoObject *value, MonoType *managedType, MonoType *nativeType, MonoMethod *method, guint32 *exception_gchandle);
NSString *      xamarin_convert_managed_to_nsstring (MonoObject *value, MonoType *managedType, MonoType *nativeType, MonoMethod *method, guint32 *exception_gchandle);
MonoObject *    xamarin_convert_nsnumber_to_managed (NSNumber *value, MonoType *nativeType, MonoType *managedType, MonoMethod *method, guint32 *exception_gchandle);
MonoObject *    xamarin_convert_nsvalue_to_managed (NSValue *value, MonoType *nativeType, MonoType *managedType, MonoMethod *method, guint32 *exception_gchandle);
MonoObject *    xamarin_convert_nsstring_to_managed (NSString *value, MonoType *nativeType, MonoType *managedType, MonoMethod *method, guint32 *exception_gchandle);
guint32         xamarin_create_bindas_exception (MonoType *inputType, MonoType *outputType, MonoMethod *method);

xamarin_id_to_managed_func xamarin_get_nsnumber_to_managed_func (MonoClass *managedType, MonoMethod *method, guint32 *exception_gchandle);
xamarin_managed_to_id_func xamarin_get_managed_to_nsnumber_func (MonoClass *managedType, MonoMethod *method, guint32 *exception_gchandle);

xamarin_id_to_managed_func xamarin_get_nsvalue_to_managed_func (MonoClass *managedType, MonoMethod *method, guint32 *exception_gchandle);
xamarin_managed_to_id_func xamarin_get_managed_to_nsvalue_func (MonoClass *managedType, MonoMethod *method, guint32 *exception_gchandle);

xamarin_id_to_managed_func xamarin_get_nsstring_to_smart_enum_func (MonoClass *managedType, MonoMethod *method, bool static_registrar, guint32 *exception_gchandle);
xamarin_managed_to_id_func xamarin_get_smart_enum_to_nsstring_func (MonoClass *managedType, MonoMethod *method, bool static_registrar, guint32 *exception_gchandle);

NSArray *   xamarin_convert_managed_to_nsarray_with_func (MonoArray *array, xamarin_managed_to_id_func convert, guint32 context, guint32 *exception_gchandle);
MonoArray * xamarin_convert_nsarray_to_managed_with_func (NSArray *array, MonoClass *managedElementType, xamarin_id_to_managed_func convert, guint32 context, guint32 *exception_gchandle);

// Returns a pointer to the value type, which must be freed using xamarin_free.
void *xamarin_nsnumber_to_bool   (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_sbyte  (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_byte   (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_short  (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_ushort (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_int    (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_uint   (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_long   (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_ulong  (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_nint   (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_nuint  (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_float  (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_double (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);
void *xamarin_nsnumber_to_nfloat (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle);

// Returns a pointer to the value type, which must be freed using xamarin_free
void *xamarin_nsvalue_to_nsrange                (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_cgaffinetransform      (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_cgpoint                (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_cgrect                 (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_cgsize                 (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_cgvector               (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_catransform3d          (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_cllocationcoordinate2d (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_cmtime                 (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_cmtimemapping          (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_cmtimerange            (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_mkcoordinatespan       (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_scnmatrix4             (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_scnvector3             (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_scnvector4             (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_uiedgeinsets           (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_uioffset               (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);
void *xamarin_nsvalue_to_nsdirectionaledgeinsets(NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_ghandle);

id xamarin_bool_to_nsnumber   (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_sbyte_to_nsnumber  (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_byte_to_nsnumber   (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_short_to_nsnumber  (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_ushort_to_nsnumber (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_int_to_nsnumber    (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_uint_to_nsnumber   (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_long_to_nsnumber   (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_ulong_to_nsnumber  (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_nint_to_nsnumber   (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_nuint_to_nsnumber  (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_float_to_nsnumber  (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_double_to_nsnumber (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_nfloat_to_nsnumber (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);
id xamarin_nfloat_to_nsnumber (MonoObject *value, guint32 contxt, guint32 *exception_gchandle);

id xamarin_nsrange_to_nsvalue                (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_cgaffinetransform_to_nsvalue      (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_cgpoint_to_nsvalue                (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_cgrect_to_nsvalue                 (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_cgsize_to_nsvalue                 (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_cgvector_to_nsvalue               (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_catransform3d_to_nsvalue          (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_cllocationcoordinate2d_to_nsvalue (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_cmtime_to_nsvalue                 (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_cmtimemapping_to_nsvalue          (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_cmtimerange_to_nsvalue            (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_mkcoordinatespan_to_nsvalue       (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_scnmatrix4_to_nsvalue             (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_scnvector3_to_nsvalue             (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_scnvector4_to_nsvalue             (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_uiedgeinsets_to_nsvalue           (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_uioffset_to_nsvalue               (MonoObject *value, guint32 context, guint32 *exception_gchandle);
id xamarin_nsdirectionaledgeinsets_to_nsvalue(MonoObject *value, guint32 context, guint32 *exception_gchandle);

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
