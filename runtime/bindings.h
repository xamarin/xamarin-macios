#ifndef __BINDINGS_H__
#define __BINDINGS_H__

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>
#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>

#include "delegates.h"

#ifdef __cplusplus
extern "C" {
#endif

// The inclusion of "_Nullable" in the signature for xamarin_UIApplicationMain makes clang complain about missing nullability info for other methods in this file.
// We don't want to fix that right now (but feel free to do so if you're reading this), so ignore the missing nullability info warning for the methods in question.
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wnullability-completeness"

void * xamarin_IntPtr_objc_msgSend_IntPtr (id self, SEL sel, void *a);
void * xamarin_IntPtr_objc_msgSendSuper_IntPtr (struct objc_super *super, SEL sel, void *a);

typedef CGFloat xm_nfloat_t;
typedef NSInteger xm_nint_t;
typedef NSUInteger xm_nuint_t;

typedef float (*float_send) (id self, SEL sel);
typedef float (*float_sendsuper) (struct objc_super *super, SEL sel);

typedef xm_nfloat_t (*nfloat_send) (id self, SEL sel);
typedef xm_nfloat_t (*nfloat_sendsuper) (struct objc_super *super, SEL sel);

xm_nfloat_t xamarin_nfloat_objc_msgSend_exception (id self, SEL sel, GCHandle *exception_gchandle);
xm_nfloat_t xamarin_nfloat_objc_msgSendSuper_exception (struct objc_super *super, SEL sel, GCHandle *exception_gchandle);

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

#pragma clang diagnostic push

int xamarin_UIApplicationMain (int argc, char * _Nullable argv[_Nonnull], NSString * _Nullable principalClassName, NSString * _Nullable delegateClassName, GCHandle *exception_gchandle);

/* Types copied from headers */
/* We need to do this for now, since we must be able to build XM on older OSXs */

typedef __attribute__((__ext_vector_type__(2))) int vector_int2;
typedef __attribute__((__ext_vector_type__(3))) int vector_int3;
typedef __attribute__((__ext_vector_type__(4))) int vector_int4;

typedef __attribute__((__ext_vector_type__(2))) double vector_double2;
typedef __attribute__((__ext_vector_type__(3))) double vector_double3;
typedef __attribute__((__ext_vector_type__(4))) double vector_double4;

typedef __attribute__((__ext_vector_type__(2))) float vector_float2;
typedef __attribute__((__ext_vector_type__(3))) float vector_float3;
typedef __attribute__((__ext_vector_type__(4))) float vector_float4;

typedef struct { vector_float2 columns[2]; } matrix_float2x2;
typedef struct { vector_float3 columns[3]; } matrix_float3x3;
typedef struct { vector_float4 columns[4]; } matrix_float4x4;
typedef struct { vector_float3 columns[4]; } matrix_float4x3;

typedef struct { vector_double4 columns[4]; } matrix_double4x4;

typedef struct { vector_float4 vector; } simd_quatf;
typedef struct { vector_double4 vector; } simd_quatd;

typedef struct {
    vector_float3 maxBounds;
    vector_float3 minBounds;
} MDLAxisAlignedBoundingBox;

typedef struct {
    vector_float3 maxBounds;
    vector_float3 minBounds;
} GKBox;

typedef struct {
    vector_float2 maxBounds;
    vector_float2 minBounds;
} GKQuad;

typedef struct {
    vector_float3 points[3];
} GKTriangle;

typedef struct
{
    NSUInteger      numberOfHistogramEntries;
    BOOL            histogramForAlpha;
    vector_float4   minPixelValue;
    vector_float4   maxPixelValue;
} MPSImageHistogramInfo;

typedef vector_int4 MDLVoxelIndex;
typedef struct {
    MDLVoxelIndex minimumExtent;
    MDLVoxelIndex maximumExtent;
} MDLVoxelIndexExtent;

typedef struct {
    vector_float3 min;
    vector_float3 max;
} MPSAxisAlignedBoundingBox;

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

struct Vector4d {
	double a, b, c, d;
};
struct Vector3d {
	double a, b, c;
};
struct Vector2d {
	double a, b;
};

struct Matrix2f {
	Vector2f columns [2];
};

struct NMatrix2 {
	Vector2f columns [2];
};

struct Matrix3f {
	Vector3f columns [3];
};

struct NMatrix3 {
	Vector4f columns [3];
};

struct Matrix4f {
	Vector4f columns [4];
};

struct NMatrix4 {
	Vector4f columns [4];
};

typedef struct NMatrix4 NMatrix4;

struct NMatrix4d {
	Vector4d columns [4];
};

struct NMatrix4x3 {
	// Use Vector4f here, since the managed version has padding to match accordingly.
	Vector4f columns [4];
};

struct QuatF {
	Vector4f vector;
};

struct QuatD {
	Vector4d vector;
};

struct MDLAxisAlignedBoundingBoxWrapper {
	Vector3f maxBounds;
    Vector3f minBounds;
};

struct GKBoxWrapper {
	Vector3f maxBounds;
    Vector3f minBounds;
};

struct GKQuadWrapper {
    Vector2f maxBounds;
    Vector2f minBounds;
};

struct GKTriangleWrapper {
	Vector3f points [3];
};

struct MPSImageHistogramInfoWrapper {
	NSUInteger numberOfHistogramEntries;
	BOOL histogramForAlpha;
	// The minPixelValue field starts at offset 16, but we can't use
	// __attribute__ ((aligned (16))), because that will make clang align the
	// entire struct on a 16-byte boundary, which doesn't match how we've
	// defined it in managed code (explicit layout, but no specific alignment).
	// So we need to manually pad the struct to match the managed definition.
#if !defined(__ILP32__)
	uint8_t dummy[7];
#else
	uint8_t dummy[11];
#endif
	Vector4f minPixelValue;
	Vector4f maxPixelValue;
};

typedef Vector4i MDLVoxelIndexWrapper;

struct MDLVoxelIndexExtentWrapper {
    MDLVoxelIndexWrapper minimumExtent;
    MDLVoxelIndexWrapper maximumExtent;
};

struct MPSAxisAlignedBoundingBoxWrapper {
    Vector3f min;
    Vector3f max;
};

static_assert (sizeof (MPSImageHistogramInfoWrapper) == sizeof (MPSImageHistogramInfo), "Sizes aren't equal");

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
CGPoint          xamarin_CGPoint__VNNormalizedFaceBoundingBoxPointForLandmarkPoint_Vector2_CGRect_nuint_nuint_string (struct Vector2f faceLandmarkPoint, CGRect faceBoundingBox, xm_nuint_t imageWidth, xm_nuint_t imageHeight, const char **error_msg);
CGPoint          xamarin_CGPoint__VNImagePointForFaceLandmarkPoint_Vector2_CGRect_nuint_nuint_string (struct Vector2f faceLandmarkPoint, CGRect faceBoundingBox, xm_nuint_t imageWidth, xm_nuint_t imageHeight, const char **error_msg);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
