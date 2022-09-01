/*
 * bindings.m
 *
 * Authors:
 *   Rolf Bjarne Kvinge (rolf@xamarin.com)
 *
 * Copyright 2012 - 2014 Xamarin Inc (http://www.xamarin.com)
 *
 * See the LICENSE file included with the distribution for details.
 * 
 */

#include "bindings.h"
#include <dlfcn.h>

#if !TARGET_OS_OSX && !TARGET_OS_WATCH
#include <UIKit/UIKit.h>
#endif

/*
 * Hand-written bindings to support ObjectiveC exceptions.
 * Reference:
 * 		https://bugzilla.xamarin.com/show_bug.cgi?id=7696
 *		https://bugzilla.xamarin.com/show_bug.cgi?id=11900
 */

xm_nfloat_t
xamarin_nfloat_objc_msgSend_exception (id self, SEL sel, GCHandle *exception_gchandle)
{
	@try {
#if defined(__i386__)
		return ((nfloat_send) objc_msgSend_fpret) (self, sel);
#else
		return ((nfloat_send) objc_msgSend) (self, sel);
#endif
	} @catch (NSException *e) {
		xamarin_process_nsexception_using_mode (e, true, exception_gchandle);
		return 0;
	}
}

xm_nfloat_t
xamarin_nfloat_objc_msgSendSuper_exception (struct objc_super *super, SEL sel, GCHandle *exception_gchandle)
{
	@try {
		// there is no objc_msgSendSuper_fpret: http://lists.apple.com/archives/objc-language/2006/Jun/msg00012.html
		return ((nfloat_sendsuper) objc_msgSendSuper) (super, sel);
	} @catch (NSException *e) {
		xamarin_process_nsexception_using_mode (e, true, exception_gchandle);
		return 0;
	}
}

/*
 * Vector c bindings
 *
 * The following code uses dlsym to not have a linking dependency on Xcode 9,
 * the code can be simplified once we require to remove dlsym or xcode 9 becomes the minimum supported version
 */

typedef CGPoint (*vision_func) (vector_float2 faceLandmarkPoint, CGRect faceBoundingBox, xm_nuint_t imageWidth, xm_nuint_t imageHeight);

static vision_func
get_vision_func (const char *func_name, const char **error_msg)
{
	static void* vision_handle;
	if (vision_handle == NULL) {
		vision_handle = dlopen ("/System/Library/Frameworks/Vision.framework/Vision", RTLD_LAZY);
		if (vision_handle == NULL) {
			*error_msg = "Could not open Vision.framework";
			return NULL;
		}
	}
	return (vision_func) dlsym (vision_handle, func_name);
}

CGPoint
xamarin_CGPoint__VNNormalizedFaceBoundingBoxPointForLandmarkPoint_Vector2_CGRect_nuint_nuint_string (struct Vector2f faceLandmarkPoint, CGRect faceBoundingBox, xm_nuint_t imageWidth, xm_nuint_t imageHeight, const char **error_msg) {

	static vision_func func = NULL;
	*error_msg = NULL;

	if (func == NULL) {
		func = get_vision_func ("VNNormalizedFaceBoundingBoxPointForLandmarkPoint", error_msg);
		if (func == NULL && *error_msg == NULL)
			*error_msg = "Could not obtain the address for VNNormalizedFaceBoundingBoxPointForLandmarkPoint";
		if (func == NULL)
			return CGPointMake (0, 0);
	}

	vector_float2 flp;
	flp [0] = faceLandmarkPoint.a;
	flp [1] = faceLandmarkPoint.b;

	return func (flp, faceBoundingBox, imageWidth, imageHeight);
}

CGPoint
xamarin_CGPoint__VNImagePointForFaceLandmarkPoint_Vector2_CGRect_nuint_nuint_string (struct Vector2f faceLandmarkPoint, CGRect faceBoundingBox, xm_nuint_t imageWidth, xm_nuint_t imageHeight, const char **error_msg) {

	static vision_func func = NULL;
	*error_msg = NULL;

	if (func == NULL) {
		func = get_vision_func ("VNImagePointForFaceLandmarkPoint", error_msg);
		if (func == NULL) {
			if (*error_msg == NULL)
				*error_msg = "Could not obtain the address for VNImagePointForFaceLandmarkPoint";
			return CGPointMake (0, 0);
		}
	}

	vector_float2 flp;
	flp [0] = faceLandmarkPoint.a;
	flp [1] = faceLandmarkPoint.b;

	return func (flp, faceBoundingBox, imageWidth, imageHeight);
}

/* UIKit bindings */
#if !TARGET_OS_OSX && !TARGET_OS_WATCH
int xamarin_UIApplicationMain (int argc, char * _Nullable argv[_Nonnull], NSString * _Nullable principalClassName, NSString * _Nullable delegateClassName, GCHandle *exception_gchandle)
{
	@try {
		*exception_gchandle = INVALID_GCHANDLE;
		return UIApplicationMain (argc, argv, principalClassName, delegateClassName);
	} @catch (NSException *e) {
		xamarin_process_nsexception_using_mode (e, false, exception_gchandle);
		return 1;
	}
}
#endif // !TARGET_OS_OSX
