#ifndef __BINDINGS_H__
#define __BINDINGS_H__

#import <Foundation/Foundation.h>
#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>

#include "delegates.h"

#ifdef __cplusplus
extern "C" {
#endif

void * xamarin_IntPtr_objc_msgSend_IntPtr (id self, SEL sel, void *a);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr (struct objc_super *super, SEL sel, void *a);

#if defined(__i386__)
typedef float xm_nfloat_t;
typedef int32_t xm_nint_t;
typedef uint32_t xm_nuint_t;
#else
typedef double xm_nfloat_t;
typedef int64_t xm_nint_t;
typedef uint64_t xm_nuint_t;
#endif

typedef float (*float_send) (id self, SEL sel);
typedef float (*float_sendsuper) (struct objc_super *super, SEL sel);

typedef xm_nfloat_t (*nfloat_send) (id self, SEL sel);
typedef xm_nfloat_t (*nfloat_sendsuper) (struct objc_super *super, SEL sel);

float xamarin_float_objc_msgSend (id self, SEL sel);
float xamarin_float_objc_msgSendSuper (struct objc_super *super, SEL sel);

xm_nfloat_t xamarin_nfloat_objc_msgSend (id self, SEL sel);
xm_nfloat_t xamarin_nfloat_objc_msgSendSuper (struct objc_super *super, SEL sel);

void * xamarin_IntPtr_objc_msgSend_IntPtr_IntPtr_int (id self, SEL sel, void *a, void *b, int c);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr_IntPtr_int (struct objc_super *super, SEL sel, void *a, void *b, int c);

void * xamarin_IntPtr_objc_msgSend_IntPtr_IntPtr_UInt32 (id self, SEL sel, void *a, void *b, uint32_t c);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr_IntPtr_UInt32 (struct objc_super *super, SEL sel, void *a, void *b, uint32_t c);

void * xamarin_IntPtr_objc_msgSend_IntPtr_IntPtr_UInt64 (id self, SEL sel, void *a, void *b, uint64_t c);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr_IntPtr_UInt64 (struct objc_super *super, SEL sel, void *a, void *b, uint64_t c);

void * xamarin_IntPtr_objc_msgSend_IntPtr_int_int_int_int (id self, SEL sel, void *a, int b, int c, int d, int e);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr_int_int_int_int (struct objc_super *super, SEL sel, void *a, int b, int c, int d, int e);

void * xamarin_IntPtr_objc_msgSend_IntPtr_nint_nint_nint_UInt32 (id self, SEL sel, void *a, xm_nint_t b, xm_nint_t c, xm_nint_t d, uint32_t e);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr_nint_nint_nint_UInt32 (struct objc_super *super, SEL sel, void *a, xm_nint_t b, xm_nint_t c, xm_nint_t d, uint32_t e);

void * xamarin_IntPtr_objc_msgSend_IntPtr_nint_nint_nint_UInt64 (id self, SEL sel, void *a, xm_nint_t b, xm_nint_t c, xm_nint_t d, uint64_t e);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr_nint_nint_nint_UInt64 (struct objc_super *super, SEL sel, void *a, xm_nint_t b, xm_nint_t c, xm_nint_t d, uint64_t e);

void * xamarin_IntPtr_objc_msgSend_IntPtr_int_int_int (id self, SEL sel, void *a, int b, int c, int d);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr_int_int_int (struct objc_super *super, SEL sel, void *a, int b, int c, int d);

void * xamarin_IntPtr_objc_msgSend_IntPtr_UInt32_nint_UInt32 (id self, SEL sel, void *a, uint32_t b, xm_nint_t c, uint32_t d);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr_UInt32_nint_UInt32 (struct objc_super *super, SEL sel, void *a, uint32_t b, xm_nint_t c, uint32_t d);

void * xamarin_IntPtr_objc_msgSend_IntPtr_UInt64_nint_UInt64 (id self, SEL sel, void *a, uint64_t b, xm_nint_t c, uint64_t d);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr_UInt64_nint_UInt64 (struct objc_super *super, SEL sel, void *a, uint64_t b, xm_nint_t c, uint64_t d);

/* Types copied from headers */
/* We need to do this for now, since we must be able to build XM on older OSXs */

typedef __attribute__((__ext_vector_type__(2))) int vector_int2;
typedef __attribute__((__ext_vector_type__(3))) int vector_int3;
typedef __attribute__((__ext_vector_type__(4))) int vector_int4;

typedef __attribute__((__ext_vector_type__(2))) float vector_float2;
typedef __attribute__((__ext_vector_type__(3))) float vector_float3;
typedef __attribute__((__ext_vector_type__(4))) float vector_float4;

typedef struct { vector_float4 columns[4]; } matrix_float4x4;

typedef struct {
    vector_float3 maxBounds;
    vector_float3 minBounds;
} MDLAxisAlignedBoundingBox;

/*
 * iOS has a vector type (vector_float3) which can't be expressed
 * in P/Invoke signatures, so we need custom wrappers.
 */

struct Vector4f {
	float a, b, c, d;
};
struct Vector3f {
	float a, b, c;
};
struct Vector2f {
	float a, b;
};

struct Vector2i {
	int a, b;
};
struct Vector3i {
	int a, b, c;
};
struct Vector4i {
	int a, b, c, d;
};

struct Matrix4f {
	Vector4f columns [4];
};

struct MDLAxisAlignedBoundingBoxWrapper {
	Vector3f maxBounds;
    Vector3f minBounds;
};


struct Vector4f  xamarin_vector_float3__Vector4_objc_msgSend (id self, SEL sel);
void             xamarin_vector_float3__Vector4_objc_msgSend_stret (struct Vector4f *v4, id self, SEL sel);
void             xamarin_vector_float3__void_objc_msgSend_Vector4 (id self, SEL sel, struct Vector4f p0);
struct Vector4f  xamarin_vector_float3__Vector4_objc_msgSendSuper (struct objc_super *super, SEL sel);
void             xamarin_vector_float3__Vector4_objc_msgSendSuper_stret (struct Vector4f *v4, struct objc_super *super, SEL sel);
void             xamarin_vector_float3__void_objc_msgSendSuper_Vector4 (struct objc_super *super, SEL sel, struct Vector4f p0);

struct Vector3f  xamarin_vector_float3__Vector3_objc_msgSend (id self, SEL sel);
void             xamarin_vector_float3__Vector3_objc_msgSend_stret (struct Vector3f *v3, id self, SEL sel);
void             xamarin_vector_float3__void_objc_msgSend_Vector3 (id self, SEL sel, struct Vector3f p0);
void             xamarin_vector_float3__Vector3_objc_msgSend_stret_Vector3 (struct Vector3f *v3, id self, SEL sel, struct Vector3f p0);
struct Vector3f  xamarin_vector_float3__Vector3_objc_msgSend_Vector3 (id self, SEL sel, struct Vector3f p0);
struct Vector3f  xamarin_vector_float3__Vector3_objc_msgSendSuper (struct objc_super *super, SEL sel);
void             xamarin_vector_float3__Vector3_objc_msgSendSuper_stret (struct Vector3f *v3, struct objc_super *super, SEL sel);
void             xamarin_vector_float3__void_objc_msgSendSuper_Vector3 (struct objc_super *super, SEL sel, struct Vector3f p0);
void             xamarin_vector_float3__Vector3_objc_msgSendSuper_stret_Vector3 (struct Vector3f *v3, struct objc_super *super, SEL sel, struct Vector3f p0);
struct Vector3f  xamarin_vector_float3__Vector3_objc_msgSendSuper_Vector3 (struct objc_super *super, SEL sel, struct Vector3f p0);

#ifndef XAMCORE_2_0
#ifdef MONOMAC
void *monomac_IntPtr_objc_msgSend_IntPtr (id self, SEL sel, void *a);
void *monomac_IntPtr_objc_msgSendSuper_IntPtr (struct objc_super *super, SEL sel, void *a);
float monomac_float_objc_msgSend (id self, SEL sel);
float monomac_float_objc_msgSendSuper (struct objc_super *super, SEL sel);
xm_nfloat_t monomac_nfloat_objc_msgSend (id self, SEL sel);
xm_nfloat_t monomac_nfloat_objc_msgSendSuper (struct objc_super *super, SEL sel);
#endif
#ifdef MONOTOUCH
void * monotouch_IntPtr_objc_msgSend_IntPtr (id self, SEL sel, void *a);
void * monotouch_IntPtr_objc_msgSendSuper_IntPtr (struct objc_super *super, SEL sel, void *a);
#endif
#endif

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
