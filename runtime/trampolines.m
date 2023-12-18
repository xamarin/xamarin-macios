/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 *  Authors: Rolf Bjarne Kvinge
 *
 *  Copyright (C) 2014 Xamarin Inc. (www.xamarin.com)
 *
 */

#include <stdio.h>
#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>
#include <Foundation/Foundation.h>

#include "frameworks.h"

#include <AVFoundation/AVFoundation.h>
#include <CoreGraphics/CoreGraphics.h>
#include <CoreLocation/CoreLocation.h>
#if HAVE_COREMEDIA
#include <CoreMedia/CoreMedia.h>
#endif
#if HAVE_MAPKIT
#include <MapKit/MapKit.h>
#endif
#include <SceneKit/SceneKit.h>
#if HAVE_COREANIMATION
#include <QuartzCore/QuartzCore.h>
#endif
#if HAVE_UIKIT
#include <UIKit/UIKit.h>
#endif

#include <pthread.h>

#include "product.h"
#include "delegates.h"
#include "xamarin/xamarin.h"
#include "slinked-list.h"
#include "trampolines-internal.h"
#include "runtime-internal.h"
//#define DEBUG_REF_COUNTING

static pthread_mutex_t refcount_mutex = PTHREAD_RECURSIVE_MUTEX_INITIALIZER;

size_t
xamarin_get_primitive_size (char type)
{
	switch (type) {
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
	default:
		return 0;
	}
}

static void *
xamarin_marshal_return_value_impl (MonoType *mtype, const char *type, MonoObject *retval, bool retain, MonoMethod *method, MethodDescription *desc, GCHandle *exception_gchandle)
{
	// COOP: accesses managed memory: unsafe mode.
	MONO_ASSERT_GC_UNSAFE;
	
	/* Any changes in this method probably need to be reflected in the static registrar as well */
	switch (type [0]) {
		case _C_CLASS:
		case _C_SEL:
			return xamarin_get_handle_for_inativeobject (retval, exception_gchandle);

		case _C_PTR: {
			MonoClass *klass = mono_class_from_mono_type (mtype);
			bool is_delegate = mono_class_is_delegate (klass);
			xamarin_mono_object_release (&klass);
			if (is_delegate) {
				return xamarin_get_block_for_delegate (method, retval, NULL, INVALID_TOKEN_REF, exception_gchandle);
			} else {
				return *(void **) mono_object_unbox (retval);
			}
		}
		case _C_ID: {
			MonoClass *r_klass = mono_object_get_class ((MonoObject *) retval);

			void *returnValue;
			if (desc && desc->bindas [0].original_type_handle != INVALID_GCHANDLE) {
				MonoReflectionType *original_type = (MonoReflectionType *) xamarin_gchandle_get_target (desc->bindas [0].original_type_handle);
				MonoType *original_tp = mono_reflection_type_get_type (original_type);
				xamarin_mono_object_release (&original_type);
				MonoType *r_type = mono_class_get_type (r_klass);
				returnValue = xamarin_generate_conversion_to_native (retval, r_type, original_tp, method, (void *) INVALID_TOKEN_REF, exception_gchandle);
				xamarin_mono_object_release (&original_tp);
				xamarin_mono_object_release (&r_type);
			} else if (xamarin_is_class_string (r_klass)) {
				returnValue = xamarin_string_to_nsstring ((MonoString *) retval, retain);
			} else if (xamarin_is_class_array (r_klass)) {
				NSArray *rv = xamarin_managed_array_to_nsarray ((MonoArray *) retval, NULL, r_klass, exception_gchandle);
				if (retain && rv)
					objc_retain (rv);
				returnValue = rv;
			} else if (xamarin_is_class_nsobject (r_klass)) {
				id i = xamarin_get_handle (retval, exception_gchandle);
				if (*exception_gchandle != INVALID_GCHANDLE) {
					returnValue = NULL;
				} else {
					//
					// This waypoint (lock+unlock) is needed so that we can reliably call retainCount in the
					// toggleref callback (by making sure the toggle ref callback sees the retain).
					//
					// The race is between the following actions (given a managed object Z):
					//
					// a1) Thread A reads retainCount = 1 for Z
					// a2) Thread A stops the world, and runs a GC (potentially collecting Z)
					// b1) Thread B retains Z
					// b2) Thread B exits the last frame that has a reference to the managed peer for Z
					//
					// Possible execution orders:
					//
					//   1) a1-a2-*: all such orders are safe, because Z will be referenced somewhere on thread B's stack during the GC
					//   2) b1-*: safe; thread A will see the retained object
					//   3) a1-b1-a2-b2: safe; because Z will be referenced somewhere on thread B's stack during the GC
					//   4) a1-b1-b2-a2: unsafe; thread A will read retainCount = 1, and think no managed code has a reference to the object, thus collect it.
					//
					// Order 4 would look like this:
					//
					//   * Thread A reads retainCount = 1 for Z
					//   * Thread B retains Z
					//   * Thread B exits the last frame that has a reference to the managed peer for Z
					//   * Thread A stops the world, and runs a GC (potentially collecting Z)
					//
					// Solution: lock/unlock the framework peer lock here. This looks
					// weird (since nothing happens inside the lock), but it works:
					//
					//   * Thread A starts a GC, locks the framework peer lock, and starts
					//     calling toggleref callbacks.
					//   * Thread A fetches the handle (H) for object Z in a toggleref
					//     callback.
					//   * Thread B calls retain for object Z
					//   * Thread B tries to lock the framework peer lock, and blocks (before returning up the stack)
					//   * Thread A finishes processing all toggleref callbacks, runs
					//     the GC, and won't free Z because it's referenced somewhere on B's stack.
					//
					// Q) Why not just unlock after calling retain, to avoid the strange-
					//    looking empty lock?
					// A) Because calling retain on an object might end up calling
					//    our xamarin_retain_trampoline method, which calls managed code, which might want to run a GC,
					//    which won't happen if we have the framework peer lock locked here.
					//    1) Thread T calls retain on a native object.
					//    2) Thread T calls xamarin_retain_trampoline, which executes managed code,
					//       which blocks on the GC (by trying to allocate memory for instance).
					//       that's supposed to happen on another thread U.
					//    3) Thread U runs the GC, and tries to lock the framework peer lock, and
					//       deadlocks because thread T already has the framework peer lock.
					//
					//    This is https://github.com/xamarin/xamarin-macios/issues/13066
					//
					// See also comment in xamarin_release_managed_ref

					xamarin_framework_peer_waypoint ();

					objc_retain (i);
					if (!retain)
						objc_autorelease (i);

					mt_dummy_use (retval);
					returnValue = i;
				}
			} else if (xamarin_is_class_inativeobject (r_klass)) {
				returnValue = xamarin_get_handle_for_inativeobject (retval, exception_gchandle);
				if (*exception_gchandle != INVALID_GCHANDLE)
					return returnValue;
				if (returnValue != NULL) {
					if (retain) {
						xamarin_retain_nativeobject (retval, exception_gchandle);
						if (*exception_gchandle != INVALID_GCHANDLE)
							return returnValue;
					} else {
						// This will try to retain the object if and only if it's an NSObject -
						// in which case we known it's 'id' here and we can call autorelease on it.
						bool retained = xamarin_attempt_retain_nsobject (retval, exception_gchandle);
						if (*exception_gchandle != INVALID_GCHANDLE)
							return returnValue;
						if (retained) {
							id i = (id) returnValue;
							objc_autorelease (i);
						}
					}
				}
			} else {
#if DOTNET
				if (xamarin_is_class_intptr (r_klass) || xamarin_is_class_nativehandle (r_klass)) {
#else
				if (xamarin_is_class_intptr (r_klass)) {
#endif
					returnValue = *(void **) mono_object_unbox (retval);
				} else {
					xamarin_assertion_message ("Don't know how to marshal a return value of type '%s.%s'. Please file a bug with a test case at https://github.com/xamarin/xamarin-macios/issues/new\n", mono_class_get_namespace (r_klass), mono_class_get_name (r_klass));
				}
			}

			xamarin_mono_object_release (&r_klass);

			return returnValue;
		}
		case _C_CHARPTR:
			return (void *) mono_string_to_utf8 ((MonoString *) retval);
		case _C_VOID:
			return (void *) 0x0;
		default:
			return *(void **) mono_object_unbox (retval);
	}
}

static GCHandle
xamarin_get_exception_for_element_conversion_failure (GCHandle inner_exception_gchandle, unsigned long index)
{
	GCHandle exception_gchandle = INVALID_GCHANDLE;
	char *msg = xamarin_strdup_printf ("Failed to marshal the value at index %lu.", index);
	exception_gchandle = xamarin_create_product_exception_with_inner_exception (8036, inner_exception_gchandle, msg);
	xamarin_free (msg);
	return exception_gchandle;
}

static GCHandle
xamarin_get_exception_for_return_value (int code, GCHandle inner_exception_gchandle, SEL sel, MonoMethod *method, MonoType *returnType)
{
	GCHandle exception_gchandle = INVALID_GCHANDLE;
	char *to_name = xamarin_type_get_full_name (returnType, &exception_gchandle);
	if (exception_gchandle != INVALID_GCHANDLE)
		return exception_gchandle;
	char *method_full_name = mono_method_full_name (method, TRUE);
	char *msg = xamarin_strdup_printf ("Unable to marshal the return value of type '%s' to Objective-C.\n"
		"Additional information:\n"
		"\tSelector: %s\n"
		"\tMethod: %s\n", to_name, sel_getName (sel), method_full_name);
	exception_gchandle = xamarin_create_product_exception_with_inner_exception (code, inner_exception_gchandle, msg);
	xamarin_free (msg);
	xamarin_free (to_name);
	xamarin_free (method_full_name);
	return exception_gchandle;
}

void *
xamarin_marshal_return_value (SEL sel, MonoType *mtype, const char *type, MonoObject *retval, bool retain, MonoMethod *method, MethodDescription *desc, GCHandle *exception_gchandle)
{
	void *rv;
	rv = xamarin_marshal_return_value_impl (mtype, type, retval, retain, method, desc, exception_gchandle);
	if (*exception_gchandle != INVALID_GCHANDLE) {
		*exception_gchandle = xamarin_get_exception_for_return_value (8033, *exception_gchandle, sel, method, mtype);
		return NULL;
	}
	return rv;
}

static const char *
get_method_description (Class cls, SEL sel)
{
	Protocol **protocols;
	unsigned int p_count;
	Class p_cls = cls;
	struct objc_method_description desc;

	while (p_cls) {
		protocols = class_copyProtocolList (p_cls, &p_count);
		for (unsigned int i = 0; i < p_count; i++) {
			desc = protocol_getMethodDescription (protocols [i], sel, YES, !class_isMetaClass (p_cls));
			if (desc.types != NULL) {
				free (protocols);
				return desc.types;
			}
		}
		free (protocols);
		p_cls = class_getSuperclass (p_cls);
	}

	Method method = class_getInstanceMethod (cls, sel);
	if (!method)
		return NULL;
	struct objc_method_description* m_desc;
	m_desc = method_getDescription (method);
	return m_desc ? m_desc->types : NULL;
}

static int
count_until (const char *desc, char start, char end)
{
	// Counts the number of characters until a certain character is found: 'end'
	// If the 'start' character is found, nesting is assumed, and an additional
	// 'end' character must be found before the function returns.
	int i = 1;
	int sub = 0;
	while (*desc) {
		if (start == *desc) {
			sub++;
		} else if (end == *desc) {
			sub--;
			if (sub == 0)
				return i;
		}
		i++;
		// This is not multi-byte safe...
		desc++;
	}

	fprintf (stderr, PRODUCT ": Unexpected type encoding, did not find end character '%c' in '%s'.", end, desc);

	return i;
}

static int
get_type_description_length (const char *desc)
{
	int length = 0;
	// This function returns the length of the first encoded type string in desc.
	switch (desc [0]) {
		case _C_ID:
			if (desc [1] == '?') {
				// Example: [AVAssetImageGenerator generateCGImagesAsynchronouslyForTimes:completionHandler:] = 'v16@0:4@8@?12'
				length = 2;
			} else {
				length = 1;
			}
			break;
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
		case _C_BOOL:
		case _C_VOID:
		case _C_CHARPTR:
			length = 1;
			break;
		case _C_PTR:
			length = 1;

			// handle broken encoding where simd types don't show up at all
			// Example: [GKPath pathWithPoints:count:radius:cyclical:] = '@24@0:4^8L12f16c20'
			// Here we assume that we're pointing to a simd type if we find
			// a number (i.e. only get the size of what we're pointing to
			// if the next character isn't a number).
			if (desc [1] < '0' || desc [1] > '9')
				length += get_type_description_length (desc + 1);
			
			break;
		case _C_ARY_B:
			length = count_until (desc, _C_ARY_B, _C_ARY_E);
			break;
		case _C_UNION_B:
			length = count_until (desc, _C_UNION_B, _C_UNION_E);
			break;
		case _C_STRUCT_B:
			length = count_until (desc, _C_STRUCT_B, _C_STRUCT_E);
			break;
		// The following are from table 6-2 here: https://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/ObjCRuntimeGuide/Articles/ocrtTypeEncodings.html
		case 'r': // _C_CONST
		case 'n':
		case 'N':
		case 'o':
		case 'O':
		case 'R':
		case 'V':
			 length = 1 + get_type_description_length (desc + 1);
			 break;
		case _C_BFLD:
			length = 1;
			break;
		case _C_UNDEF:
			// Example: [NSView sortSubviewsUsingFunction:context:] = '^?16^v24'
			length = 1;
			break;
		case _C_ATOM:
		case _C_VECTOR:
			xamarin_assertion_message ("Unhandled type encoding: %s", desc);
			break;
		default:
			xamarin_assertion_message ("Unsupported type encoding: %s", desc);
			break;
	}

	// Every type encoding _may_ be followed by the stack frame offset for that type
	while (desc [length] >= '0' && desc [length] <= '9')
		length++;

	return length;
}

// The input string will be freed (so that the caller can use xamarin_strdup_printf easily).
GCHandle
xamarin_create_mt_exception (char *msg)
{
	MonoException *ex = xamarin_create_exception (msg);
	xamarin_free (msg);
	return xamarin_gchandle_new ((MonoObject *) ex, FALSE);
}

// Skips any brace and the content within. Supports nested braced content.
// Example:
//    {bc}d => d
//    {a{b}cd}e => e
static const char *
skip_nested_brace (const char *type)
{
	if (*type != '{')
		return type;
	while (*++type) {
		switch (*type) {
		case '{':
			return skip_nested_brace (type);
		case '}':
			return type++;
		default:
			break;
		}
	}
	return NULL;
}

// Takes a struct type name and collapses it into just the types of the
// fields. The purpose of this function is to get a string where each
// character represents the type + size of a field in the struct.
//
// Examples:
//     {MKCoordinateRegion={CLLocationCoordinate2D=dd}{MKCoordinateSpan=dd}} => dddd
//     {CGRect=dddd} => dddd
//     ^q => ^
//	   @? => @ (this is a block)
//
// type: the input type name
// struct_name: where to write the collapsed struct name. Returns an empty string if the array isn't big enough.
// max_char: the maximum number of characters to write to struct_name
// return value: false if something went wrong (an exception thrown, or struct_name wasn't big enough).
bool
xamarin_collapse_struct_name (const char *type, char struct_name[], int max_char, GCHandle *exception_gchandle)
{
	const char *input = type;
	int c = 0;
	struct_name [0] = 0;

	while (*type) {
		switch (*type) {
		case _C_STRUCT_B:
			// Skip until '='
			while (type [1] != 0 && type [0] != '=')
				type++;
			break;
		case _C_CONST:
		case _C_STRUCT_E:
			// don't care about these
			break;
		case _C_PTR:
			if (c == max_char) {
				LOGZ ("    xamarin_collapse_struct_name (%s, %i) => failed!\n", input, max_char);
				struct_name [0] = 0; // return an empty string
				return false;
			}
			struct_name [c++] = *type;
			type++;
			// this might be a pointer to a pointer to a pointer to a labrador!
			while (*type == _C_PTR)
				type++;
			// it might be a pointer to some nested stuff. skip that.
			if (*type == '{')
				type = skip_nested_brace (type);
			else
				type++;
			continue;
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
		case _C_BOOL:
		case _C_CHARPTR:
			if (c == max_char) {
				LOGZ ("    xamarin_collapse_struct_name (%s, %i) => failed!\n", input, max_char);
				struct_name [0] = 0; // return an empty string
				return false;
			}
			struct_name [c++] = *type;
			break;
		case _C_UNDEF:
			// unknown. this could be a block in which case the previous character is a '@'.
			break;
		case _C_ATOM:
		case _C_ARY_B:
		case _C_ARY_E:
		case _C_UNION_B:
		case _C_UNION_E:
		case _C_VOID:
		case _C_BFLD:
		default:
			// don't understand.
			struct_name [0] = 0; // return an empty string
			*exception_gchandle = xamarin_create_mt_exception (xamarin_strdup_printf (PRODUCT ": Cannot marshal type %s (unexpected type encoding: %c)\n", input, *type));
			return false;
		}
		type++;
	}

	if (c == max_char) {
		LOGZ ("    xamarin_collapse_struct_name (%s, %i) => failed (too long)!\n", input, max_char);
		struct_name [0] = 0; // return an empty string
		return false;
	}

	struct_name [c] = 0; // Zero-terminate.
	LOGZ ("    xamarin_collapse_struct_name (%s, %i) => %s (succeeded)\n", input, max_char, struct_name);
	return true;
}

int 
xamarin_get_frame_length (id self, SEL sel)
{
	if (self == NULL)
		return sizeof (void *) * 3; // we might be in objc_msgStret, in which case we'll need to copy three arguments.

	// [NSDecimalNumber initWithDecimal:] has this descriptor: "@36@0:8{?=b8b4b1b1b18[8S]}16"
	// which NSMethodSignature chokes on: NSInvalidArgumentException Reason: +[NSMethodSignature signatureWithObjCTypes:]: unsupported type encoding spec '{?}'
	// So instead parse the description ourselves.

	unsigned long length = 0;
	[self class]; // There's a bug in the ObjC runtime where we might get an uninitialized Class instance from object_getClass. See #6258. Calling the 'class' selector first makes sure the Class instance is initialized.
	Class cls = object_getClass (self);
	const char *method_description = get_method_description (cls, sel);
	const char *desc = method_description;
	if (desc == NULL) {
		// This happens with [[UITableViewCell appearance] backgroundColor]
		@try {
			NSMethodSignature *sig = [self methodSignatureForSelector: sel];
			length = [sig frameLength];
		} @catch (NSException *ex) {
			length = sizeof (void *) * 64; // some high-ish number.
			fprintf (stderr, PRODUCT ": Failed to calculate the frame size for the method [%s %s] (%s). Using a value of %lu instead.\n", class_getName (cls), sel_getName (sel), [[ex description] UTF8String], length);
		}
	} else {
		// The format of the method type encoding is described here: http://stackoverflow.com/a/11492151/183422
		// the return type might have a number after it, which is the size of the argument frame
		// first get this number (if it's there), and use it as a minimum value for the frame length
		int rvlength = get_type_description_length (desc);
		unsigned long min_length = 0;
		if (rvlength > 0) {
			const char *min_start = desc + rvlength;
			// the number is at the end of the return type encoding, so find any numbers
			// at the end of the type encoding.
			while (min_start > desc && min_start [-1] >= '0' && min_start [-1] <= '9')
				min_start--;
			if (min_start < desc + rvlength) {
				for (int i = 0; i < desc + rvlength - min_start; i++)
					min_length = min_length * 10ul + (unsigned long)(min_start [i] - '0');
			}
		}

		// fprintf (stderr, "Found desc '%s' for [%s %s] with min frame length %i\n", desc, class_getName (cls), sel_getName (sel), min_length);

		// skip the return value.
		desc += rvlength;
		while (*desc) {
			unsigned long tl = xamarin_objc_type_size (desc);
			// round up to pointer size
			if (tl % sizeof (void *) != 0)
				tl += sizeof (void *) - (tl % sizeof (void *));
			length += tl;
			// fprintf (stderr, " argument=%s length=%i totallength=%i\n", desc, tl, length);
			desc += get_type_description_length (desc);
		}

		if (min_length > length) {
			// this might happen for methods that take simd types, since those arguments don't show up in the
			// method signature encoding at all, but they're still added to the frame size.
			// fprintf (stderr, " min length: %i is higher than calculated length: %i for [%s %s] with description %s\n", min_length, length, class_getName (cls), sel_getName (sel), method_description);
			length = min_length;
		}
	}

	// we can't detect varargs, so just add 16 more pointer sized arguments to be on the safe-ish side.
	length += sizeof (void *) * 16;

	return (int) length;
}

static inline void
find_objc_method_implementation (struct objc_super *sup, id self, SEL sel, IMP xamarin_impl)
{
	// COOP: does not access managed memory: any mode
	Class klass = object_getClass (self);
	Class sklass = class_getSuperclass (klass);

	IMP imp = class_getMethodImplementation (klass, sel);
	IMP simp = class_getMethodImplementation (sklass, sel);
	while (imp == simp || simp == xamarin_impl) {
		sklass = class_getSuperclass (sklass);
		simp = class_getMethodImplementation (sklass, sel);
	}

	sup->receiver = self;
#if !defined(__cplusplus) && !__OBJC2__
	sup->class = sklass;
#else
	sup->super_class = sklass;
#endif
}

id
xamarin_invoke_objc_method_implementation (id self, SEL sel, IMP xamarin_impl)
{
	// COOP: does not access managed memory: any mode
	struct objc_super sup;
	find_objc_method_implementation (&sup, self, sel, xamarin_impl);
	typedef id (*func_objc_msgSendSuper) (struct objc_super *sup, SEL sel);
	return ((func_objc_msgSendSuper) objc_msgSendSuper) (&sup, sel);
}

#if MONOMAC
id
xamarin_copyWithZone_trampoline1 (id self, SEL sel, NSZone *zone)
{
	// COOP: does not access managed memory: any mode
	// This is for subclasses that themselves do not implement Copy (NSZone)

	id rv;
	GCHandle gchandle;
	enum XamarinGCHandleFlags flags = XamarinGCHandleFlags_None;
	struct objc_super sup;

#if defined (DEBUG_REF_COUNTING)
	PRINT ("xamarin_copyWithZone_trampoline1 (%p, %s, %p)\n", self, sel_getName (sel), zone);
#endif

	// Clear out our own GCHandle
	gchandle = xamarin_get_gchandle_with_flags (self, &flags);
	if (gchandle != INVALID_GCHANDLE)
		xamarin_set_gchandle_with_flags (self, INVALID_GCHANDLE, XamarinGCHandleFlags_None);

	// Call the base class implementation
	id (*invoke) (struct objc_super *, SEL, NSZone*) = (id (*)(struct objc_super *, SEL, NSZone*)) objc_msgSendSuper;
	find_objc_method_implementation (&sup, self, sel, (IMP) xamarin_copyWithZone_trampoline1);
	rv = invoke (&sup, sel, zone);

	// Restore our GCHandle
	if (gchandle != INVALID_GCHANDLE)
		xamarin_set_gchandle_with_flags (self, gchandle, flags);

	return rv;
}

id
xamarin_copyWithZone_trampoline2 (id self, SEL sel, NSZone *zone)
{
	// COOP: does not access managed memory: any mode
	// This is for subclasses that already implement Copy (NSZone)

	id rv;
	GCHandle gchandle;
	enum XamarinGCHandleFlags flags = XamarinGCHandleFlags_None;

#if defined (DEBUG_REF_COUNTING)
	PRINT ("xamarin_copyWithZone_trampoline2 (%p, %s, %p)\n", self, sel_getName (sel), zone);
#endif

	// Clear out our own GCHandle
	gchandle = xamarin_get_gchandle_with_flags (self, &flags);
	if (gchandle != INVALID_GCHANDLE)
		xamarin_set_gchandle_with_flags (self, INVALID_GCHANDLE, XamarinGCHandleFlags_None);

	// Call the managed implementation
	id (*invoke) (id, SEL, NSZone*) = (id (*)(id, SEL, NSZone*)) xamarin_trampoline;
	rv = invoke (self, sel, zone);

	// Restore our GCHandle
	if (gchandle != INVALID_GCHANDLE)
		xamarin_set_gchandle_with_flags (self, gchandle, flags);

	return rv;
}
#endif

void
xamarin_release_trampoline (id self, SEL sel)
{
	// COOP: does not access managed memory: any mode, but it assumes safe mode upon entry (it takes locks, and doesn't switch to safe mode).
	MONO_ASSERT_GC_SAFE_OR_DETACHED;
	
	unsigned long ref_count;
	bool detach = false;

	pthread_mutex_lock (&refcount_mutex);

	ref_count = [self retainCount];

#if defined(DEBUG_REF_COUNTING)
	PRINT ("xamarin_release_trampoline (%s Handle=%p) retainCount=%d; HasManagedRef=%i GCHandle=%i\n", 
		class_getName ([self class]), self, ref_count, xamarin_has_managed_ref (self), xamarin_get_gchandle (self));
#endif
	
	/*
	 * We need to decide if the gchandle should become a weak one.
	 * This happens if managed code will end up holding the only ref.
	 */

	if (ref_count == 2 && xamarin_has_managed_ref_safe (self)) {
		xamarin_switch_gchandle (self, true /* weak */);
		detach = true;
	}

	pthread_mutex_unlock (&refcount_mutex);

	/* Invoke the real release method */
	xamarin_invoke_objc_method_implementation (self, sel, (IMP) xamarin_release_trampoline);

	if (detach)
		mono_thread_detach_if_exiting ();
}

void
xamarin_notify_dealloc (id self, GCHandle gchandle)
{
	GCHandle exception_gchandle = INVALID_GCHANDLE;

	// COOP: safe mode upon entry, switches to unsafe when acccessing managed memory.
	MONO_ASSERT_GC_SAFE_OR_DETACHED;
	
	/* This is needed because we call into managed code below (xamarin_unregister_nsobject) */
	MONO_THREAD_ATTACH; // COOP: This will swith to GC_UNSAFE

	/* Object is about to die. Unregister it and free any gchandles we may have */
#if defined(DEBUG_REF_COUNTING)
	PRINT ("xamarin_notify_dealloc (%p, %i)\n", self, gchandle);
#endif
	xamarin_unregister_nsobject (self, GINT_TO_POINTER (gchandle), &exception_gchandle);
	xamarin_free_gchandle (self, gchandle);

	MONO_THREAD_DETACH; // COOP: This will switch to GC_SAFE

	xamarin_process_managed_exception_gchandle (exception_gchandle);

	mono_thread_detach_if_exiting ();
}

id
xamarin_retain_trampoline (id self, SEL sel)
{
	// COOP: safe mode upon entry, switches to unsafe when acccessing managed memory.
	MONO_ASSERT_GC_SAFE_OR_DETACHED;

	pthread_mutex_lock (&refcount_mutex);

#if defined(DEBUG_REF_COUNTING)
	int ref_count = (int) [self retainCount];
	bool had_managed_ref = xamarin_has_managed_ref (self);
	GCHandle pre_gchandle = xamarin_get_gchandle (self);
#endif

	/*
	 * We need to make sure we have a strong GCHandle.
	 * We can not rely the retainCount changing from 1 to 2, since
	 * we can not monitor all retains (see bug #26532).
	 * So just always make sure we have a strong GCHandle after a retain.
	 */
	xamarin_switch_gchandle (self, false /* strong */);

	pthread_mutex_unlock (&refcount_mutex);

	/* Invoke the real retain method */
	self = xamarin_invoke_objc_method_implementation (self, sel, (IMP) xamarin_retain_trampoline);
	
#if defined(DEBUG_REF_COUNTING)
	PRINT ("xamarin_retain_trampoline  (%s Handle=%p) initial retainCount=%d; new retainCount=%d HadManagedRef=%i HasManagedRef=%i old GCHandle=%i new GCHandle=%i\n", 
		class_getName ([self class]), self, ref_count, (int) [self retainCount], had_managed_ref, xamarin_has_managed_ref (self), pre_gchandle, xamarin_get_gchandle (self));
#endif

	return self;
}


// We try to use the associated object API as little as possible, because the API does
// not like recursion (see bug #35017), and it calls retain/release, which we might
// have overridden with our own code that calls these functions. So in addition to
// keeping the gchandle inside the associated object, we also keep it in a hash
// table, so that xamarin_get_gchandle_trampoline does not have to call any
// associated object API to get the gchandle.
static CFMutableDictionaryRef gchandle_hash = NULL;
static pthread_mutex_t gchandle_hash_lock = PTHREAD_MUTEX_INITIALIZER;

struct gchandle_dictionary_entry {
	GCHandle gc_handle;
	enum XamarinGCHandleFlags flags;
};

static void
release_gchandle_dictionary_entry (CFAllocatorRef allocator, const void *value)
{
	free ((void *) value);
}

static const char *associated_key = "x"; // the string value doesn't matter, only the pointer value.
bool
xamarin_set_gchandle_trampoline (id self, SEL sel, GCHandle gc_handle, enum XamarinGCHandleFlags flags)
{
	// COOP: Called by ObjC (when the setGCHandle:flags: selector is called on an object).
	// COOP: Safe mode upon entry, and doesn't access managed memory, so no need to change.
	MONO_ASSERT_GC_SAFE;
	
	/* This is for types registered using the dynamic registrar */
	XamarinAssociatedObject *obj;
	obj = objc_getAssociatedObject (self, associated_key);

	// Check if we're setting the initial value, in which case we don't want to overwrite
	if (obj != NULL && obj->gc_handle != INVALID_GCHANDLE && ((flags & XamarinGCHandleFlags_InitialSet) == XamarinGCHandleFlags_InitialSet))
		return false;

	flags = (enum XamarinGCHandleFlags) (flags & ~XamarinGCHandleFlags_InitialSet); // Remove the InitialSet flag, we don't want to store it.

	if (obj == NULL && gc_handle != INVALID_GCHANDLE) {
		obj = [[XamarinAssociatedObject alloc] init];
		obj->gc_handle = gc_handle;
		obj->flags = flags;
		obj->native_object = self;
		objc_setAssociatedObject (self, associated_key, obj, OBJC_ASSOCIATION_RETAIN_NONATOMIC);
		objc_release (obj);
	}

	if (obj != NULL) {
		obj->gc_handle = gc_handle;
		obj->flags = flags;
	}
	
	pthread_mutex_lock (&gchandle_hash_lock);
	if (gchandle_hash == NULL) {
		CFDictionaryValueCallBacks value_callbacks = { 0 };
		value_callbacks.release = release_gchandle_dictionary_entry;
		gchandle_hash = CFDictionaryCreateMutable (kCFAllocatorDefault, 0, NULL, &value_callbacks);
	}
	if (gc_handle == INVALID_GCHANDLE) {
		CFDictionaryRemoveValue (gchandle_hash, self);
	} else {
		struct gchandle_dictionary_entry *entry = (struct gchandle_dictionary_entry *) malloc (sizeof (struct gchandle_dictionary_entry));
		entry->gc_handle = gc_handle;
		entry->flags = flags;
		CFDictionarySetValue (gchandle_hash, self, entry);
	}
	pthread_mutex_unlock (&gchandle_hash_lock);

	return true;
}

GCHandle
xamarin_get_gchandle_trampoline (id self, SEL sel)
{
	// COOP: Called by ObjC (when the getGCHandle selector is called on an object).
	// COOP: Safe mode upon entry, and doesn't access managed memory, so no need to switch.
	MONO_ASSERT_GC_SAFE;
	
	/* This is for types registered using the dynamic registrar */
	GCHandle gc_handle = INVALID_GCHANDLE;
	pthread_mutex_lock (&gchandle_hash_lock);
	if (gchandle_hash != NULL) {
		struct gchandle_dictionary_entry *entry;
		entry = (struct gchandle_dictionary_entry *) CFDictionaryGetValue (gchandle_hash, self);
		if (entry != NULL)
			gc_handle = entry->gc_handle;
	}
	pthread_mutex_unlock (&gchandle_hash_lock);
	return gc_handle;
}

enum XamarinGCHandleFlags
xamarin_get_flags_trampoline (id self, SEL sel)
{
	// COOP: Called by ObjC (when the getFlags selector is called on an object).
	// COOP: Safe mode upon entry, and doesn't access managed memory, so no need to switch.
	MONO_ASSERT_GC_SAFE;

	/* This is for types registered using the dynamic registrar */
	enum XamarinGCHandleFlags flags = XamarinGCHandleFlags_None;
	pthread_mutex_lock (&gchandle_hash_lock);
	if (gchandle_hash != NULL) {
		struct gchandle_dictionary_entry *entry;
		entry = (struct gchandle_dictionary_entry *) CFDictionaryGetValue (gchandle_hash, self);
		if (entry != NULL)
			flags = entry->flags;
	}
	pthread_mutex_unlock (&gchandle_hash_lock);
	return flags;
}

void
xamarin_set_flags_trampoline (id self, SEL sel, enum XamarinGCHandleFlags flags)
{
	// COOP: Called by ObjC (when the setFlags: selector is called on an object).
	// COOP: Safe mode upon entry, and doesn't access managed memory, so no need to switch.
	MONO_ASSERT_GC_SAFE;

	/* This is for types registered using the dynamic registrar */
	pthread_mutex_lock (&gchandle_hash_lock);
	if (gchandle_hash != NULL) {
		struct gchandle_dictionary_entry *entry;
		entry = (struct gchandle_dictionary_entry *) CFDictionaryGetValue (gchandle_hash, self);
		if (entry != NULL)
			entry->flags = flags;
	}
	pthread_mutex_unlock (&gchandle_hash_lock);
}

id
xamarin_generate_conversion_to_native (MonoObject *value, MonoType *inputType, MonoType *outputType, MonoMethod *method, void *context, GCHandle *exception_gchandle)
{
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;

	// This method is a mirror of StaticRegistrar.GenerateConversionToNative
	// These methods must be kept in sync.

	id convertedValue = NULL;
	MonoClass *managedType = mono_class_from_mono_type (inputType);
	MonoClass *nativeType = mono_class_from_mono_type (outputType);

	MonoClass *underlyingManagedType = managedType;
	MonoClass *underlyingNativeType = nativeType;
	MonoClass *nativeElementType = NULL;
	MonoClass *managedElementType = NULL;

	bool isManagedArray = xamarin_is_class_array (managedType);
	bool isNativeArray = xamarin_is_class_array (nativeType);

	MonoClass *nullableManagedType = NULL;
	bool isManagedNullable = xamarin_is_class_nullable (managedType, &nullableManagedType, exception_gchandle);
	if (*exception_gchandle != INVALID_GCHANDLE)
		goto exception_handling;

	if (isManagedArray != isNativeArray) {
		*exception_gchandle = xamarin_create_bindas_exception (inputType, outputType, method);
		goto exception_handling;
	}

	if (isManagedArray) {
		if (isManagedNullable) {
			*exception_gchandle = xamarin_create_bindas_exception (inputType, outputType, method);
			goto exception_handling;
		}
		nativeElementType = mono_class_get_element_class (nativeType);
		managedElementType = mono_class_get_element_class (managedType);
		underlyingNativeType = nativeElementType;
		underlyingManagedType = managedElementType;
	} else if (isManagedNullable) {
		underlyingManagedType = nullableManagedType;
	}

	if (value) {
		xamarin_managed_to_id_func func;
		if (xamarin_is_class_nsnumber (underlyingNativeType)) {
			func = xamarin_get_managed_to_nsnumber_func (underlyingManagedType, method, exception_gchandle);
		} else if (xamarin_is_class_nsvalue (underlyingNativeType)) {
			func = xamarin_get_managed_to_nsvalue_func (underlyingManagedType, method, exception_gchandle);
		} else if (xamarin_is_class_nsstring (underlyingNativeType)) {
			func = xamarin_get_smart_enum_to_nsstring_func (underlyingManagedType, method, exception_gchandle);
		} else {
			*exception_gchandle = xamarin_create_bindas_exception (inputType, outputType, method);
			goto exception_handling;
		}
		if (*exception_gchandle != INVALID_GCHANDLE)
			goto exception_handling;

		if (isManagedArray) {
			convertedValue = xamarin_convert_managed_to_nsarray_with_func ((MonoArray *) value, func, context, exception_gchandle);
			if (*exception_gchandle != INVALID_GCHANDLE)
				goto exception_handling;
		} else {
			convertedValue = func (value, context, exception_gchandle);
			if (*exception_gchandle != INVALID_GCHANDLE)
				goto exception_handling;
		}
	}

exception_handling:

	xamarin_mono_object_release (&managedType);
	xamarin_mono_object_release (&nativeType);
	xamarin_mono_object_release (&nativeElementType);
	xamarin_mono_object_release (&managedElementType);
	xamarin_mono_object_release (&nullableManagedType);

	return convertedValue;
}


void *
xamarin_generate_conversion_to_managed (id value, MonoType *inputType, MonoType *outputType, MonoMethod *method, GCHandle *exception_gchandle, void *context, /*SList*/ void **free_list, /*SList*/ void**release_list_ptr)
{
	// COOP: Reads managed memory, needs to be in UNSAFE mode
	MONO_ASSERT_GC_UNSAFE;

	// This method is a mirror of StaticRegistrar.GenerateConversionToManaged
	// These methods must be kept in sync.

	void *convertedValue = NULL;
	MonoClass *managedType = mono_class_from_mono_type (outputType);
	MonoClass *nativeType = mono_class_from_mono_type (inputType);

	MonoClass *underlyingManagedType = managedType;
	MonoClass *underlyingNativeType = nativeType;
	MonoClass *nativeElementType = NULL;
	MonoClass *managedElementType = NULL;

	bool isManagedArray = xamarin_is_class_array (managedType);
	bool isNativeArray = xamarin_is_class_array (nativeType);

	MonoClass *nullableManagedType = NULL;
	bool isManagedNullable = xamarin_is_class_nullable (managedType, &nullableManagedType, exception_gchandle);
	if (*exception_gchandle != INVALID_GCHANDLE)
		goto exception_handling;

	if (isManagedArray != isNativeArray) {
		*exception_gchandle = xamarin_create_bindas_exception (inputType, outputType, method);
		goto exception_handling;
	}

	if (isManagedArray) {
		if (isManagedNullable) {
			*exception_gchandle = xamarin_create_bindas_exception (inputType, outputType, method);
			goto exception_handling;
		}
		nativeElementType = mono_class_get_element_class (nativeType);
		managedElementType = mono_class_get_element_class (managedType);
		underlyingNativeType = nativeElementType;
		underlyingManagedType = managedElementType;
	} else if (isManagedNullable) {
		underlyingManagedType = nullableManagedType;
	}

	if (value) {
		xamarin_id_to_managed_func func;
		if (xamarin_is_class_nsnumber (underlyingNativeType)) {
			func = xamarin_get_nsnumber_to_managed_func (underlyingManagedType, method, exception_gchandle);
		} else if (xamarin_is_class_nsvalue (underlyingNativeType)) {
			func = xamarin_get_nsvalue_to_managed_func (underlyingManagedType, method, exception_gchandle);
		} else if (xamarin_is_class_nsstring (underlyingNativeType)) {
			func = xamarin_get_nsstring_to_smart_enum_func (underlyingManagedType, method, exception_gchandle);
		} else {
			*exception_gchandle = xamarin_create_bindas_exception (inputType, outputType, method);
			goto exception_handling;
		}
		if (*exception_gchandle != INVALID_GCHANDLE)
			goto exception_handling;

		if (isManagedArray) {
			convertedValue = xamarin_convert_nsarray_to_managed_with_func (value, underlyingManagedType, func, context, exception_gchandle);
			if (*exception_gchandle != INVALID_GCHANDLE)
				goto exception_handling;
			SList* release_list = *(SList**) release_list_ptr;
			if (release_list != NULL)
				*release_list_ptr = s_list_prepend (release_list, convertedValue);
		} else {
			convertedValue = func (value, NULL, underlyingManagedType, context, exception_gchandle);
			if (*exception_gchandle != INVALID_GCHANDLE)
				goto exception_handling;
			*(SList **) free_list = s_list_prepend (*(SList **) free_list, convertedValue);

			if (isManagedNullable) {
				convertedValue = mono_value_box (mono_domain_get (), underlyingManagedType, convertedValue);
				SList* release_list = *(SList**) release_list_ptr;
				if (release_list != NULL)
					*release_list_ptr = s_list_prepend (release_list, convertedValue);
			}
		}
	}

exception_handling:

	xamarin_mono_object_release (&managedType);
	xamarin_mono_object_release (&nativeType);
	xamarin_mono_object_release (&nativeElementType);
	xamarin_mono_object_release (&managedElementType);
	xamarin_mono_object_release (&nullableManagedType);

	return convertedValue;
}

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wunguarded-availability-new"

// Returns a pointer to the value type, which must be freed using xamarin_free.
// If called multiple times in succession, the returned pointer can be passed as the second ptr argument, and it need only be freed once done iterating.
void *xamarin_nsnumber_to_bool   (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {       BOOL *valueptr =       (BOOL *) (ptr ? ptr : xamarin_calloc (sizeof (BOOL)));       *valueptr = [number boolValue];             return valueptr; }
void *xamarin_nsnumber_to_sbyte  (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {     int8_t *valueptr =     (int8_t *) (ptr ? ptr : xamarin_calloc (sizeof (int8_t)));     *valueptr = [number charValue];             return valueptr; }
void *xamarin_nsnumber_to_byte   (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {    uint8_t *valueptr =    (uint8_t *) (ptr ? ptr : xamarin_calloc (sizeof (uint8_t)));    *valueptr = [number unsignedCharValue];     return valueptr; }
void *xamarin_nsnumber_to_short  (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {    int16_t *valueptr =    (int16_t *) (ptr ? ptr : xamarin_calloc (sizeof (int16_t)));    *valueptr = [number shortValue];            return valueptr; }
void *xamarin_nsnumber_to_ushort (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {   uint16_t *valueptr =   (uint16_t *) (ptr ? ptr : xamarin_calloc (sizeof (uint16_t)));   *valueptr = [number unsignedShortValue];    return valueptr; }
void *xamarin_nsnumber_to_int    (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {    int32_t *valueptr =    (int32_t *) (ptr ? ptr : xamarin_calloc (sizeof (int32_t)));    *valueptr = [number intValue];              return valueptr; }
void *xamarin_nsnumber_to_uint   (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {   uint32_t *valueptr =   (uint32_t *) (ptr ? ptr : xamarin_calloc (sizeof (uint32_t)));   *valueptr = [number unsignedIntValue];      return valueptr; }
void *xamarin_nsnumber_to_long   (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {    int64_t *valueptr =    (int64_t *) (ptr ? ptr : xamarin_calloc (sizeof (int64_t)));    *valueptr = [number longLongValue];         return valueptr; }
void *xamarin_nsnumber_to_ulong  (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {   uint64_t *valueptr =   (uint64_t *) (ptr ? ptr : xamarin_calloc (sizeof (uint64_t)));   *valueptr = [number unsignedLongLongValue]; return valueptr; }
void *xamarin_nsnumber_to_nint   (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {  NSInteger *valueptr =  (NSInteger *) (ptr ? ptr : xamarin_calloc (sizeof (NSInteger)));  *valueptr = [number integerValue];          return valueptr; }
void *xamarin_nsnumber_to_nuint  (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) { NSUInteger *valueptr = (NSUInteger *) (ptr ? ptr : xamarin_calloc (sizeof (NSUInteger))); *valueptr = [number unsignedIntegerValue];  return valueptr; }
void *xamarin_nsnumber_to_float  (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {      float *valueptr =      (float *) (ptr ? ptr : xamarin_calloc (sizeof (float)));      *valueptr = [number floatValue];            return valueptr; }
void *xamarin_nsnumber_to_double (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {     double *valueptr =     (double *) (ptr ? ptr : xamarin_calloc (sizeof (double)));     *valueptr = [number doubleValue];           return valueptr; }
#if __POINTER_WIDTH__ == 32
void *xamarin_nsnumber_to_nfloat (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {      float *valueptr =      (float *) (ptr ? ptr : xamarin_calloc (sizeof (float)));      *valueptr = [number floatValue];            return valueptr; }
#elif __POINTER_WIDTH__ == 64
void *xamarin_nsnumber_to_nfloat (NSNumber *number, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {     double *valueptr =     (double *) (ptr ? ptr : xamarin_calloc (sizeof (double)));     *valueptr = [number doubleValue];           return valueptr; }
#else
	#error Invalid pointer size.
#endif

// Returns a pointer to the value type, which must be freed using xamarin_free.
// If called multiple times in succession, the returned pointer can be passed as the second ptr argument, and it need only be freed once done iterating.
void *xamarin_nsvalue_to_nsrange                (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {                NSRange *valueptr =                (NSRange *) (ptr ? ptr : xamarin_calloc (sizeof (NSRange)));                *valueptr = [value rangeValue];             return valueptr; }
#if HAVE_UIKIT // Yep, these CoreGraphics-looking category method is defined in UIKit.
void *xamarin_nsvalue_to_cgaffinetransform      (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {      CGAffineTransform *valueptr =      (CGAffineTransform *) (ptr ? ptr : xamarin_calloc (sizeof (CGAffineTransform)));      *valueptr = [value CGAffineTransformValue]; return valueptr; }
void *xamarin_nsvalue_to_cgpoint                (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {                CGPoint *valueptr =                (CGPoint *) (ptr ? ptr : xamarin_calloc (sizeof (CGPoint)));                *valueptr = [value CGPointValue];           return valueptr; }
void *xamarin_nsvalue_to_cgrect                 (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {                 CGRect *valueptr =                 (CGRect *) (ptr ? ptr : xamarin_calloc (sizeof (CGRect)));                 *valueptr = [value CGRectValue];            return valueptr; }
void *xamarin_nsvalue_to_cgsize                 (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {                 CGSize *valueptr =                 (CGSize *) (ptr ? ptr : xamarin_calloc (sizeof (CGSize)));                 *valueptr = [value CGSizeValue];            return valueptr; }
void *xamarin_nsvalue_to_cgvector               (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {               CGVector *valueptr =               (CGVector *) (ptr ? ptr : xamarin_calloc (sizeof (CGVector)));               *valueptr = [value CGVectorValue];          return valueptr; }
void *xamarin_nsvalue_to_nsdirectionaledgeinsets(NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {NSDirectionalEdgeInsets *valueptr =(NSDirectionalEdgeInsets *) (ptr ? ptr : xamarin_calloc (sizeof (NSDirectionalEdgeInsets)));*valueptr = [value directionalEdgeInsetsValue];return valueptr; }
#endif
#if HAVE_COREANIMATION
void *xamarin_nsvalue_to_catransform3d          (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {          CATransform3D *valueptr =          (CATransform3D *) (ptr ? ptr : xamarin_calloc (sizeof (CATransform3D)));          *valueptr = [value CATransform3DValue];     return valueptr; }
#endif
#if HAVE_MAPKIT // Yep, this is defined in MapKit.
void *xamarin_nsvalue_to_cllocationcoordinate2d (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) { CLLocationCoordinate2D *valueptr = (CLLocationCoordinate2D *) (ptr ? ptr : xamarin_calloc (sizeof (CLLocationCoordinate2D))); *valueptr = [value MKCoordinateValue];      return valueptr; }
#endif
#if HAVE_COREMEDIA
void *xamarin_nsvalue_to_cmtime                 (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {                 CMTime *valueptr =                 (CMTime *) (ptr ? ptr : xamarin_calloc (sizeof (CMTime)));                 *valueptr = [value CMTimeValue];            return valueptr; }
void *xamarin_nsvalue_to_cmtimemapping          (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {          CMTimeMapping *valueptr =          (CMTimeMapping *) (ptr ? ptr : xamarin_calloc (sizeof (CMTimeMapping)));          *valueptr = [value CMTimeMappingValue];     return valueptr; }
void *xamarin_nsvalue_to_cmtimerange            (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {            CMTimeRange *valueptr =            (CMTimeRange *) (ptr ? ptr : xamarin_calloc (sizeof (CMTimeRange)));            *valueptr = [value CMTimeRangeValue];       return valueptr; }
void *xamarin_nsvalue_to_cmvideodimensions      (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {      CMVideoDimensions *valueptr =      (CMVideoDimensions *) (ptr ? ptr : xamarin_calloc (sizeof (CMVideoDimensions)));      *valueptr = [value CMVideoDimensionsValue]; return valueptr; }
#endif
#if HAVE_MAPKIT
void *xamarin_nsvalue_to_mkcoordinatespan       (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {       MKCoordinateSpan *valueptr =       (MKCoordinateSpan *) (ptr ? ptr : xamarin_calloc (sizeof (MKCoordinateSpan)));       *valueptr = [value MKCoordinateSpanValue];  return valueptr; }
#endif
void *xamarin_nsvalue_to_scnmatrix4             (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {             SCNMatrix4 *valueptr =             (SCNMatrix4 *) (ptr ? ptr : xamarin_calloc (sizeof (SCNMatrix4)));             *valueptr = [value SCNMatrix4Value];        return valueptr; }
void *
xamarin_nsvalue_to_scnvector3 (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle)
{
#if TARGET_OS_IOS && defined (__arm__)
	// In earlier versions of iOS [NSValue SCNVector3Value] would return 4
	// floats. This does not cause problems on 64-bit architectures, because
	// the 4 floats end up in floating point registers that doesn't need to be
	// preserved. On 32-bit architectures it becomes a real problem though,
	// since objc_msgSend_stret will be called, and the return value will be
	// written to the stack. Writing 4 floats to the stack, when clang
	// allocates 3 bytes, is a bad idea. There's no radar since this has
	// already been fixed in iOS, it only affects older versions.

	// So we have to avoid the SCNVector3Value selector on 32-bit
	// architectures, since we can't influence how clang generates the call.
	// Instead use [NSValue getValue:]. Interestingly enough this function has
	// the same bug: it will write 4 floats on 32-bit architectures (and
	// amazingly 4 *doubles* on 64-bit architectures - this has been filed as
	// radar 33104111), but since we control the input buffer, we can just
	// allocate the necessary bytes. And for good measure allocate 32 bytes,
	// just to be sure.

	// Just to complicate matters, everything works fine on watchOS because
	// armv7k does not use objc_msgSend_stret for this signature, this only
	// happens on iOS.
	SCNVector3 *valueptr = (SCNVector3 *) xamarin_calloc (32);
	[value getValue: valueptr];
	if (ptr) {
		memcpy (ptr, valueptr, sizeof (SCNVector3));
		xamarin_free (valueptr);
		valueptr = (SCNVector3 *) ptr;
	}
#else
	SCNVector3 *valueptr = (SCNVector3 *) (ptr ? ptr : xamarin_calloc (sizeof (SCNVector3)));
	*valueptr = [value SCNVector3Value];
#endif

	return valueptr;
}
void *xamarin_nsvalue_to_scnvector4             (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {             SCNVector4 *valueptr =             (SCNVector4 *) (ptr ? ptr : xamarin_calloc (sizeof (SCNVector4)));             *valueptr = [value SCNVector4Value];        return valueptr; }
#if HAVE_UIKIT
void *xamarin_nsvalue_to_uiedgeinsets           (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {           UIEdgeInsets *valueptr =           (UIEdgeInsets *) (ptr ? ptr : xamarin_calloc (sizeof (UIEdgeInsets)));           *valueptr = [value UIEdgeInsetsValue];      return valueptr; }
void *xamarin_nsvalue_to_uioffset               (NSValue *value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle) {               UIOffset *valueptr =               (UIOffset *) (ptr ? ptr : xamarin_calloc (sizeof (UIOffset)));               *valueptr = [value UIOffsetValue];          return valueptr; }
#endif

id xamarin_bool_to_nsnumber   (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithBool:                  *(BOOL *) mono_object_unbox (value)]; }
id xamarin_sbyte_to_nsnumber  (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithChar:                *(int8_t *) mono_object_unbox (value)]; }
id xamarin_byte_to_nsnumber   (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithUnsignedChar:       *(uint8_t *) mono_object_unbox (value)]; }
id xamarin_short_to_nsnumber  (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithShort:              *(int16_t *) mono_object_unbox (value)]; }
id xamarin_ushort_to_nsnumber (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithUnsignedShort:     *(uint16_t *) mono_object_unbox (value)]; }
id xamarin_int_to_nsnumber    (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithInt:                *(int32_t *) mono_object_unbox (value)]; }
id xamarin_uint_to_nsnumber   (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithUnsignedInt:       *(uint32_t *) mono_object_unbox (value)]; }
id xamarin_long_to_nsnumber   (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithLongLong:           *(int64_t *) mono_object_unbox (value)]; }
id xamarin_ulong_to_nsnumber  (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithUnsignedLongLong:  *(uint64_t *) mono_object_unbox (value)]; }
id xamarin_nint_to_nsnumber   (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithInteger:          *(NSInteger *) mono_object_unbox (value)]; }
id xamarin_nuint_to_nsnumber  (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithUnsignedInteger: *(NSUInteger *) mono_object_unbox (value)]; }
id xamarin_float_to_nsnumber  (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithFloat:                *(float *) mono_object_unbox (value)]; }
id xamarin_double_to_nsnumber (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithDouble:              *(double *) mono_object_unbox (value)]; }
#if __POINTER_WIDTH__ == 32
id xamarin_nfloat_to_nsnumber (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithFloat:                *(float *) mono_object_unbox (value)]; }
#elif __POINTER_WIDTH__ == 64
id xamarin_nfloat_to_nsnumber (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSNumber numberWithDouble:              *(double *) mono_object_unbox (value)]; }
#else
	#error Invalid pointer size.
#endif

id xamarin_nsrange_to_nsvalue                (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithRange:               *(NSRange *)                mono_object_unbox (value)]; }
#if HAVE_UIKIT // yep, these CoreGraphics-looking category methods are defined in UIKit
id xamarin_cgaffinetransform_to_nsvalue      (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithCGAffineTransform:   *(CGAffineTransform *)      mono_object_unbox (value)]; }
id xamarin_cgpoint_to_nsvalue                (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithCGPoint:             *(CGPoint *)                mono_object_unbox (value)]; }
id xamarin_cgrect_to_nsvalue                 (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithCGRect:              *(CGRect *)                 mono_object_unbox (value)]; }
id xamarin_cgsize_to_nsvalue                 (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithCGSize:              *(CGSize *)                 mono_object_unbox (value)]; }
id xamarin_cgvector_to_nsvalue               (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithCGVector:            *(CGVector *)               mono_object_unbox (value)]; }
id xamarin_nsdirectionaledgeinsets_to_nsvalue(MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithDirectionalEdgeInsets:*(NSDirectionalEdgeInsets *)mono_object_unbox (value)]; }
#endif
#if HAVE_COREANIMATION
id xamarin_catransform3d_to_nsvalue          (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithCATransform3D:       *(CATransform3D *)          mono_object_unbox (value)]; }
#endif
#if HAVE_MAPKIT // Yep, this is defined in MapKit.
id xamarin_cllocationcoordinate2d_to_nsvalue (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithMKCoordinate:        *(CLLocationCoordinate2D *) mono_object_unbox (value)]; }
#endif
#if HAVE_COREMEDIA
id xamarin_cmtime_to_nsvalue                 (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithCMTime:              *(CMTime *)                 mono_object_unbox (value)]; }
id xamarin_cmtimemapping_to_nsvalue          (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithCMTimeMapping:       *(CMTimeMapping *)          mono_object_unbox (value)]; }
id xamarin_cmtimerange_to_nsvalue            (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithCMTimeRange:         *(CMTimeRange *)            mono_object_unbox (value)]; }
id xamarin_cmvideodimensions_to_nsvalue      (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithCMVideoDimensions:   *(CMVideoDimensions *)      mono_object_unbox (value)]; }
#endif
#if HAVE_MAPKIT
id xamarin_mkcoordinatespan_to_nsvalue       (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithMKCoordinateSpan:    *(MKCoordinateSpan *)       mono_object_unbox (value)]; }
#endif
id xamarin_scnmatrix4_to_nsvalue             (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithSCNMatrix4:          *(SCNMatrix4 *)             mono_object_unbox (value)]; }
id xamarin_scnvector3_to_nsvalue             (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithSCNVector3:          *(SCNVector3 *)             mono_object_unbox (value)]; }
id xamarin_scnvector4_to_nsvalue             (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithSCNVector4:          *(SCNVector4 *)             mono_object_unbox (value)]; }
#if HAVE_UIKIT
id xamarin_uiedgeinsets_to_nsvalue           (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithUIEdgeInsets:        *(UIEdgeInsets *)           mono_object_unbox (value)]; }
id xamarin_uioffset_to_nsvalue               (MonoObject *value, void *context, GCHandle *exception_gchandle) { return [NSValue valueWithUIOffset:            *(UIOffset *)               mono_object_unbox (value)]; }
#endif

#pragma clang diagnostic pop

struct conversion_data {
	MonoDomain *domain;
	MonoType *element_type;
	MonoClass *element_class;
	MonoReflectionType *element_reflection_type;
	uint32_t iface_token_ref;
	uint32_t implementation_token_ref;
};

id
xamarin_convert_string_to_nsstring (MonoObject *obj, void *context, GCHandle *exception_gchandle)
{
	return xamarin_string_to_nsstring ((MonoString *) obj, false);
}

void *
xamarin_convert_nsstring_to_string (id value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle)
{
	return xamarin_nsstring_to_string (NULL, (NSString *) value);
}

id
xamarin_object_to_nsobject (MonoObject *object, void *context, GCHandle *exception_gchandle)
{
	return xamarin_get_nsobject_handle (object);
}

id
xamarin_inativeobject_to_nsobject (MonoObject *object, void *context, GCHandle *exception_gchandle)
{
	return xamarin_get_handle_for_inativeobject (object, exception_gchandle);
}

void *
xamarin_nsobject_to_object (id object, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle)
{
	struct conversion_data * data = (struct conversion_data *) context;
	return xamarin_get_nsobject_with_type_for_ptr (object, false, data->element_type, exception_gchandle);
}

void *
xamarin_nsobject_to_inativeobject (id object, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle)
{
	struct conversion_data * data = (struct conversion_data *) context;
	return xamarin_get_inative_object_dynamic (object, false, data->element_reflection_type, exception_gchandle);
}

void *
xamarin_nsobject_to_inativeobject_static (id object, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle)
{
	struct conversion_data * data = (struct conversion_data *) context;
	return xamarin_get_inative_object_static (object, false, data->iface_token_ref, data->implementation_token_ref, exception_gchandle);
}

NSArray *
xamarin_managed_string_array_to_nsarray (MonoArray *array, GCHandle *exception_gchandle)
{
	return xamarin_convert_managed_to_nsarray_with_func (array, xamarin_convert_string_to_nsstring, 0, exception_gchandle);
}

NSArray *
xamarin_managed_nsobject_array_to_nsarray (MonoArray *array, GCHandle *exception_gchandle)
{
	return xamarin_convert_managed_to_nsarray_with_func (array, xamarin_object_to_nsobject, 0, exception_gchandle);
}

NSArray *
xamarin_managed_inativeobject_array_to_nsarray (MonoArray *array, GCHandle *exception_gchandle)
{
	return xamarin_convert_managed_to_nsarray_with_func (array, xamarin_inativeobject_to_nsobject, 0, exception_gchandle);
}

NSArray *
xamarin_managed_array_to_nsarray (MonoArray *array, MonoType *managed_type, MonoClass *managed_class, GCHandle *exception_gchandle)
{
	NSArray *rv = NULL;

	if (array == NULL)
		return NULL;

	MonoClass *mclass = NULL;
	if (managed_class == NULL) {
		mclass = mono_class_from_mono_type (managed_type);
		managed_class = mclass;
	}

	MonoClass *e_klass = mono_class_get_element_class (managed_class);

	xamarin_mono_object_release (&mclass);

	if (xamarin_is_class_string (e_klass)) {
		rv = xamarin_managed_string_array_to_nsarray (array, exception_gchandle);
	} else if (xamarin_is_class_nsobject (e_klass)) {
		rv = xamarin_managed_nsobject_array_to_nsarray (array, exception_gchandle);
	} else if (xamarin_is_class_inativeobject (e_klass)) {
		rv = xamarin_managed_inativeobject_array_to_nsarray (array, exception_gchandle);
	} else {
		// Don't know how to convert: show an exception.
		MonoType *e_type = mono_class_get_type (e_klass);
		char *element_name = xamarin_type_get_full_name (e_type, exception_gchandle);
		xamarin_mono_object_release (&e_type);
		if (*exception_gchandle == INVALID_GCHANDLE) {
			char *msg = xamarin_strdup_printf ("Unable to convert from a managed array of %s to an NSArray.", element_name);
			*exception_gchandle = xamarin_create_product_exception_with_inner_exception (8032, *exception_gchandle, msg);
			xamarin_free (msg);
			xamarin_free (element_name);
		}
	}

	xamarin_mono_object_release (&e_klass);

	return rv;
}

MonoArray *
xamarin_nsarray_to_managed_string_array (NSArray *array, GCHandle *exception_gchandle)
{
	MonoClass *stringClass = mono_get_string_class ();
	MonoArray *rv = xamarin_convert_nsarray_to_managed_with_func (array, stringClass, xamarin_convert_nsstring_to_string, 0, exception_gchandle);
	xamarin_mono_object_release (&stringClass);
	return rv;
}

MonoArray *
xamarin_nsarray_to_managed_nsobject_array (NSArray *array, MonoType *array_type, MonoClass *element_class, GCHandle *exception_gchandle)
{
	MonoClass *e_class = NULL;
	if (element_class == NULL) {
		MonoClass *mclass = mono_class_from_mono_type (array_type);
		e_class = mono_class_get_element_class (mclass);
		xamarin_mono_object_release (&mclass);

		element_class = e_class;
	}

	struct conversion_data data = { 0 };
	data.domain = mono_domain_get ();
	data.element_class = element_class;
	data.element_type = mono_class_get_type (data.element_class);
	data.element_reflection_type = mono_type_get_object (data.domain, data.element_type);
	MonoArray *rv = xamarin_convert_nsarray_to_managed_with_func (array, data.element_class, xamarin_nsobject_to_object, &data, exception_gchandle);

	xamarin_mono_object_release (&data.element_type);
	xamarin_mono_object_release (&data.element_reflection_type);
	xamarin_mono_object_release (&e_class);

	return rv;
}

MonoArray *
xamarin_nsarray_to_managed_inativeobject_array (NSArray *array, MonoType *array_type, MonoClass *element_class, GCHandle *exception_gchandle)
{
	MonoClass *e_class = NULL;
	if (element_class == NULL) {
		MonoClass *mclass = mono_class_from_mono_type (array_type);
		e_class = mono_class_get_element_class (mclass);
		xamarin_mono_object_release (&mclass);

		element_class = e_class;
	}

	struct conversion_data data = { 0 };
	data.domain = mono_domain_get ();
	data.element_class = element_class;
	data.element_type = mono_class_get_type (data.element_class);
	data.element_reflection_type = mono_type_get_object (data.domain, data.element_type);
	MonoArray *rv = xamarin_convert_nsarray_to_managed_with_func (array, data.element_class, xamarin_nsobject_to_inativeobject, &data, exception_gchandle);

	xamarin_mono_object_release (&data.element_type);
	xamarin_mono_object_release (&data.element_reflection_type);
	xamarin_mono_object_release (&e_class);

	return rv;
}

MonoArray *
xamarin_nsarray_to_managed_inativeobject_array_static (NSArray *array, MonoType *array_type, MonoClass *element_class, uint32_t iface_token_ref, uint32_t implementation_token_ref, GCHandle *exception_gchandle)
{
	MonoClass *e_class = NULL;
	if (element_class == NULL) {
		MonoClass *mclass = mono_class_from_mono_type (array_type);
		e_class = mono_class_get_element_class (mclass);
		xamarin_mono_object_release (&mclass);

		element_class = e_class;
	}

	struct conversion_data data = { 0 };
	data.element_class = element_class;
	data.element_type = mono_class_get_type (data.element_class);
	data.iface_token_ref = iface_token_ref;
	data.implementation_token_ref = implementation_token_ref;

	MonoArray *rv = xamarin_convert_nsarray_to_managed_with_func (array, data.element_class, xamarin_nsobject_to_inativeobject_static, &data, exception_gchandle);

	xamarin_mono_object_release (&data.element_type);
	xamarin_mono_object_release (&e_class);

	return rv;
}

MonoArray *
xamarin_nsarray_to_managed_array (NSArray *array, MonoType *managed_type, MonoClass *managed_class, GCHandle *exception_gchandle)
{
	MonoArray *rv = NULL;

	if (array == NULL)
		return NULL;

	MonoClass *mclass = NULL;
	if (managed_class == NULL) {
		mclass = mono_class_from_mono_type (managed_type);
		managed_class = mclass;
	}

	xamarin_mono_object_release (&mclass);

	MonoClass *e_klass = mono_class_get_element_class (managed_class);
	if (xamarin_is_class_string (e_klass)) {
		rv = xamarin_nsarray_to_managed_string_array (array, exception_gchandle);
	} else if (xamarin_is_class_nsobject (e_klass)) {
		rv = xamarin_nsarray_to_managed_nsobject_array (array, managed_type, e_klass, exception_gchandle);
	} else if (xamarin_is_class_inativeobject (e_klass)) {
		rv = xamarin_nsarray_to_managed_inativeobject_array (array, managed_type, e_klass, exception_gchandle);
	} else {
		// Don't know how to convert: show an exception.
		MonoType *e_type = mono_class_get_type (e_klass);
		char *element_name = xamarin_type_get_full_name (e_type, exception_gchandle);
		xamarin_mono_object_release (&e_type);
		if (*exception_gchandle == INVALID_GCHANDLE) {
			char *msg = xamarin_strdup_printf ("Unable to convert from an NSArray to a managed array of %s.", element_name);
			*exception_gchandle = xamarin_create_product_exception_with_inner_exception (8031, *exception_gchandle, msg);
			xamarin_free (msg);
			xamarin_free (element_name);
		}
	}

	xamarin_mono_object_release (&e_klass);

	return rv;
}

static void *
xamarin_get_nsnumber_converter (MonoClass *managedType, MonoMethod *method, bool to_managed, GCHandle *exception_gchandle)
{
	void * func = NULL;
	MonoType *mtype = NULL;
	char *fullname = xamarin_class_get_full_name (managedType, exception_gchandle);
	if (*exception_gchandle != INVALID_GCHANDLE)
		goto exception_handling;

	mtype = mono_class_get_type (managedType);

	if (!strcmp (fullname, "System.SByte")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_sbyte : (void *) xamarin_sbyte_to_nsnumber;
	} else if (!strcmp (fullname, "System.Byte")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_byte : (void *) xamarin_byte_to_nsnumber;
	} else if (!strcmp (fullname, "System.Int16")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_short : (void *) xamarin_short_to_nsnumber;
	} else if (!strcmp (fullname, "System.UInt16")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_ushort : (void *) xamarin_ushort_to_nsnumber;
	} else if (!strcmp (fullname, "System.Int32")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_int : (void *) xamarin_int_to_nsnumber;
	} else if (!strcmp (fullname, "System.UInt32")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_uint : (void *) xamarin_uint_to_nsnumber;
	} else if (!strcmp (fullname, "System.Int64")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_long : (void *) xamarin_long_to_nsnumber;
	} else if (!strcmp (fullname, "System.UInt64")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_ulong : (void *) xamarin_ulong_to_nsnumber;
	} else if (!strcmp (fullname, "System.Single")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_float : (void *) xamarin_float_to_nsnumber;
	} else if (!strcmp (fullname, "System.Double")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_double : (void *) xamarin_double_to_nsnumber;
	} else if (!strcmp (fullname, "System.Boolean")) {
		func = to_managed ? (void *) xamarin_nsnumber_to_bool : (void *) xamarin_bool_to_nsnumber;
#if DOTNET
	} else if (!strcmp (fullname, "System.IntPtr")) {
#else
	} else if (!strcmp (fullname, "System.nint")) {
#endif
		func = to_managed ? (void *) xamarin_nsnumber_to_nint : (void *) xamarin_nint_to_nsnumber;
#if DOTNET
	} else if (!strcmp (fullname, "System.UIntPtr")) {
#else
	} else if (!strcmp (fullname, "System.nuint")) {
#endif
		func = to_managed ? (void *) xamarin_nsnumber_to_nuint : (void *) xamarin_nuint_to_nsnumber;
#if DOTNET
	} else if (!strcmp (fullname, "System.Runtime.InteropServices.NFloat")) {
#else
	} else if (!strcmp (fullname, "System.nfloat")) {
#endif
		func = to_managed ? (void *) xamarin_nsnumber_to_nfloat : (void *) xamarin_nfloat_to_nsnumber;
	} else if (mono_class_is_enum (managedType)) {
		MonoType *baseType = mono_class_enum_basetype (managedType);
		MonoClass *baseClass = mono_class_from_mono_type (baseType);
		func = xamarin_get_nsnumber_converter (baseClass, method, to_managed, exception_gchandle);
		xamarin_mono_object_release (&baseClass);
		xamarin_mono_object_release (&baseType);
	} else {
		MonoType *nsnumberType = xamarin_get_nsnumber_type ();
		*exception_gchandle = xamarin_create_bindas_exception (mtype, nsnumberType, method);
		xamarin_mono_object_release (&nsnumberType);
		goto exception_handling;
	}

exception_handling:
	xamarin_free (fullname);
	xamarin_mono_object_release (&mtype);

	return func;
}

static void *
xamarin_get_nsvalue_converter (MonoClass *managedType, MonoMethod *method, bool to_managed, GCHandle *exception_gchandle)
{
	void * func = NULL;
	char *fullname = xamarin_class_get_full_name (managedType, exception_gchandle);
	if (*exception_gchandle != INVALID_GCHANDLE)
		goto exception_handling;

	if (!strcmp (fullname, "Foundation.NSRange")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_nsrange : (void *) xamarin_nsrange_to_nsvalue;
#if HAVE_UIKIT // yep, these CoreGraphics-looking category methods are defined in UIKit
	} else if (!strcmp (fullname, "CoreGraphics.CGAffineTransform")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_cgaffinetransform : (void *) xamarin_cgaffinetransform_to_nsvalue;
	} else if (!strcmp (fullname, "CoreGraphics.CGPoint")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_cgpoint : (void *) xamarin_cgpoint_to_nsvalue;
	} else if (!strcmp (fullname, "CoreGraphics.CGRect")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_cgrect : (void *) xamarin_cgrect_to_nsvalue;
	} else if (!strcmp (fullname, "CoreGraphics.CGSize")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_cgsize : (void *) xamarin_cgsize_to_nsvalue;
	} else if (!strcmp (fullname, "CoreGraphics.CGVector")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_cgvector : (void *) xamarin_cgvector_to_nsvalue;
	} else if (!strcmp (fullname, "UIKit.NSDirectionalEdgeInsets")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_nsdirectionaledgeinsets : (void *) xamarin_nsdirectionaledgeinsets_to_nsvalue;
#endif
#if HAVE_COREANIMATION
	} else if (!strcmp (fullname, "CoreAnimation.CATransform3D")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_catransform3d : (void *) xamarin_catransform3d_to_nsvalue;
#endif
#if HAVE_MAPKIT // Yep, this is defined in MapKit.
	} else if (!strcmp (fullname, "CoreLocation.CLLocationCoordinate2D")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_cllocationcoordinate2d : (void *) xamarin_cllocationcoordinate2d_to_nsvalue;
#endif
#if HAVE_COREMEDIA
	} else if (!strcmp (fullname, "CoreMedia.CMTime")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_cmtime : (void *) xamarin_cmtime_to_nsvalue;
	} else if (!strcmp (fullname, "CoreMedia.CMTimeMapping")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_cmtimemapping : (void *) xamarin_cmtimemapping_to_nsvalue;
	} else if (!strcmp (fullname, "CoreMedia.CMTimeRange")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_cmtimerange : (void *) xamarin_cmtimerange_to_nsvalue;
	} else if (!strcmp (fullname, "CoreMedia.CMVideoDimensions")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_cmvideodimensions : (void *) xamarin_cmvideodimensions_to_nsvalue;
#endif
#if HAVE_MAPKIT
	} else if (!strcmp (fullname, "MapKit.MKCoordinateSpan")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_mkcoordinatespan : (void *) xamarin_mkcoordinatespan_to_nsvalue;
#endif
	} else if (!strcmp (fullname, "SceneKit.SCNMatrix4")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_scnmatrix4 : (void *) xamarin_scnmatrix4_to_nsvalue;
	} else if (!strcmp (fullname, "SceneKit.SCNVector3")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_scnvector3 : (void *) xamarin_scnvector3_to_nsvalue;
	} else if (!strcmp (fullname, "SceneKit.SCNVector4")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_scnvector4 : (void *) xamarin_scnvector4_to_nsvalue;
#if HAVE_UIKIT
	} else if (!strcmp (fullname, "UIKit.UIEdgeInsets")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_uiedgeinsets : (void *) xamarin_uiedgeinsets_to_nsvalue;
	} else if (!strcmp (fullname, "UIKit.UIOffset")) {
		func = to_managed ? (void *) xamarin_nsvalue_to_uioffset : (void *) xamarin_uioffset_to_nsvalue;
#endif
	} else {
		MonoType *mType = mono_class_get_type (managedType);
		MonoType *nsvalueType = xamarin_get_nsvalue_type ();
		*exception_gchandle = xamarin_create_bindas_exception (mType, nsvalueType, method);
		xamarin_mono_object_release (&mType);
		xamarin_mono_object_release (&nsvalueType);
		goto exception_handling;
	}

exception_handling:
	xamarin_free (fullname);

	return func;
}

xamarin_id_to_managed_func
xamarin_get_nsnumber_to_managed_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle)
{
	return (xamarin_id_to_managed_func) xamarin_get_nsnumber_converter (managedType, method, true, exception_gchandle);
}

xamarin_managed_to_id_func
xamarin_get_managed_to_nsnumber_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle)
{
	return (xamarin_managed_to_id_func) xamarin_get_nsnumber_converter (managedType, method, false, exception_gchandle);
}

xamarin_id_to_managed_func
xamarin_get_nsvalue_to_managed_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle)
{
	return (xamarin_id_to_managed_func) xamarin_get_nsvalue_converter (managedType, method, true, exception_gchandle);
}

xamarin_managed_to_id_func
xamarin_get_managed_to_nsvalue_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle)
{
	return (xamarin_managed_to_id_func) xamarin_get_nsvalue_converter (managedType, method, false, exception_gchandle);
}

void *
xamarin_smart_enum_to_nsstring (MonoObject *value, void *context /* token ref */, GCHandle *exception_gchandle)
{
	guint32 context_ref = GPOINTER_TO_UINT (context);
	if (context_ref == INVALID_TOKEN_REF) {
		// This requires the dynamic registrar to invoke the correct conversion function
		NSString *rv = xamarin_convert_smart_enum_to_nsstring (value, exception_gchandle);
		return rv;
	} else {
		// The static registrar found the correct conversion function, and provided a token ref we can use
		// to find it (and invoke it), without needing the dynamic registrar.
		MonoMethod *managed_method;
		MonoObject *exception = NULL;
		MonoObject *retval;
		void *arg_ptrs [1];

		managed_method = xamarin_get_managed_method_for_token (context_ref /* token ref */, exception_gchandle);
		if (*exception_gchandle != INVALID_GCHANDLE) return NULL;

		arg_ptrs [0] = mono_object_unbox (value);

		retval = mono_runtime_invoke (managed_method, NULL, arg_ptrs, &exception);

		xamarin_mono_object_release (&managed_method);

		if (exception) {
			*exception_gchandle = xamarin_gchandle_new (exception, FALSE);
			return NULL;
		}

		if (retval == NULL)
			return NULL;

		id retval_handle = xamarin_get_nsobject_handle (retval);
		xamarin_mono_object_release (&retval);
		return retval_handle;
	}
}

void *
xamarin_nsstring_to_smart_enum (id value, void *ptr, MonoClass *managedType, void *context, GCHandle *exception_gchandle)
{
	guint32 context_ref = GPOINTER_TO_UINT (context);
	MonoObject *obj;
	MonoType *parameterType = NULL;

	if (context_ref == INVALID_TOKEN_REF) {
		// This requires the dynamic registrar to invoke the correct conversion function
		MonoType *mType = mono_class_get_type (managedType);
		MonoReflectionType *rManagedType = mono_type_get_object (mono_domain_get (), mType);
		xamarin_mono_object_release (&mType);
		obj = xamarin_convert_nsstring_to_smart_enum (value, rManagedType, exception_gchandle);
		xamarin_mono_object_release (&rManagedType);
		if (*exception_gchandle != INVALID_GCHANDLE)
			return ptr;
	} else {
		// The static registrar found the correct conversion function, and provided a token ref we can use
		// to find it (and invoke it), without needing the dynamic registrar.
		MonoMethod *managed_method;
		void *arg_ptrs [1];
		MonoObject *exception = NULL;
		MonoObject *arg0 = NULL;

		managed_method = xamarin_get_managed_method_for_token (context_ref /* token ref */, exception_gchandle);
		if (*exception_gchandle != INVALID_GCHANDLE) return NULL;

		parameterType = xamarin_get_parameter_type (managed_method, 0);
		arg0 = xamarin_get_nsobject_with_type_for_ptr (value, false, parameterType, exception_gchandle);
		xamarin_mono_object_release (&parameterType);
		if (*exception_gchandle != INVALID_GCHANDLE) {
			xamarin_mono_object_release (&managed_method);
			return NULL;
		}

		arg_ptrs [0] = arg0;

		obj = mono_runtime_invoke (managed_method, NULL, arg_ptrs, &exception);

		xamarin_mono_object_release (&arg0);
		xamarin_mono_object_release (&managed_method);

		if (exception) {
			*exception_gchandle = xamarin_gchandle_new (exception, FALSE);
			return NULL;
		}
	}

	size_t size = (size_t) mono_class_value_size (managedType, NULL);
	if (!ptr)
		ptr = xamarin_calloc (size);
	void *value_ptr = mono_object_unbox (obj);
	memcpy (ptr, value_ptr, size);

	xamarin_mono_object_release (&obj);

	return ptr;
}

xamarin_id_to_managed_func
xamarin_get_nsstring_to_smart_enum_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle)
{
	return xamarin_nsstring_to_smart_enum;
}

xamarin_managed_to_id_func
xamarin_get_smart_enum_to_nsstring_func (MonoClass *managedType, MonoMethod *method, GCHandle *exception_gchandle)
{
	return (xamarin_managed_to_id_func) xamarin_smart_enum_to_nsstring;
}

NSArray *
xamarin_convert_managed_to_nsarray_with_func (MonoArray *array, xamarin_managed_to_id_func convert, void *context, GCHandle *exception_gchandle)
{
	id *buf = NULL;
	NSArray *rv = NULL;
#if !defined (CORECLR_RUNTIME)
	size_t element_size = 0;
	char *ptr = NULL;
#endif

	if (array == NULL)
		return NULL;

	unsigned long length = mono_array_length (array);
	if (length == 0)
		return [NSArray array];

	buf = (id *) malloc (sizeof (id) * length);

#if !defined (CORECLR_RUNTIME)
	MonoClass *object_class = mono_object_get_class ((MonoObject *) array);
	MonoClass *element_class = mono_class_get_element_class (object_class);
	xamarin_mono_object_release (&object_class);

	bool is_value_type = mono_class_is_valuetype (element_class);
	if (is_value_type) {
		element_size = (size_t) mono_class_value_size (element_class, NULL);
		ptr = (char *) mono_array_addr_with_size (array, (int) element_size, 0);
	}
#endif

	for (unsigned long i = 0; i < length; i++) {
		MonoObject *value = NULL;
#if defined (CORECLR_RUNTIME)
		value = mono_array_get (array, i, exception_gchandle);
#else
		if (is_value_type) {
			value = mono_value_box (mono_domain_get (), element_class, ptr + element_size * i);
		} else {
			value = mono_array_get (array, MonoObject *, i);
		}
#endif
		if (*exception_gchandle != INVALID_GCHANDLE) {
			*exception_gchandle = xamarin_get_exception_for_element_conversion_failure (*exception_gchandle, i);
			goto exception_handling;
		}

		buf [i] = convert (value, context, exception_gchandle);
		xamarin_mono_object_release (&value);

		if (*exception_gchandle != INVALID_GCHANDLE) {
			*exception_gchandle = xamarin_get_exception_for_element_conversion_failure (*exception_gchandle, i);
			goto exception_handling;
		}
	}
	rv = [NSArray arrayWithObjects: buf count: length];

exception_handling:
	free (buf);
#if !defined (CORECLR_RUNTIME)
	xamarin_mono_object_release (&element_class);
#endif

	return rv;
}

MonoArray *
xamarin_convert_nsarray_to_managed_with_func (NSArray *array, MonoClass *managedElementType, xamarin_id_to_managed_func convert, void *context, GCHandle *exception_gchandle)
{
	if (array == NULL)
		return NULL;

	unsigned long length = [array count];
	MonoArray *rv = mono_array_new (mono_domain_get (), managedElementType, length);

	if (length == 0)
		return rv;

	bool is_value_type = mono_class_is_valuetype (managedElementType);
	MonoObject *mobj;
	void *valueptr = NULL;
#if !defined (CORECLR_RUNTIME)
	size_t element_size = 0;
	char *ptr = NULL;
#endif

#if !defined (CORECLR_RUNTIME)
	if (is_value_type) {
		element_size = (size_t) mono_class_value_size (managedElementType, NULL);
		ptr = (char *) mono_array_addr_with_size (rv, (int) element_size, 0);
	}
#endif

	for (unsigned long i = 0; i < length; i++) {
		if (is_value_type) {
			valueptr = convert ([array objectAtIndex: i], valueptr, managedElementType, context, exception_gchandle);
#if defined (CORECLR_RUNTIME)
			xamarin_bridge_set_array_struct_value (rv, i, managedElementType, valueptr, exception_gchandle);
#else
			memcpy (ptr, valueptr, element_size);
			ptr += element_size;
#endif
		} else {
			mobj = (MonoObject *) convert ([array objectAtIndex: i], NULL, managedElementType, context, exception_gchandle);
			if (*exception_gchandle == INVALID_GCHANDLE) {
				mono_array_setref (rv, i, mobj, exception_gchandle);
				xamarin_mono_object_release (&mobj);
			}
		}
		if (*exception_gchandle != INVALID_GCHANDLE) {
			*exception_gchandle = xamarin_get_exception_for_element_conversion_failure (*exception_gchandle, i);
			goto exception_handling;
		}
	}

exception_handling:
	xamarin_free (valueptr);

	return rv;
}

NSNumber *
xamarin_convert_managed_to_nsnumber (MonoObject *value, MonoClass *managedType, MonoMethod *method, void *context, GCHandle *exception_gchandle)
{
	xamarin_managed_to_id_func convert = xamarin_get_managed_to_nsnumber_func (managedType, method, exception_gchandle);
	if (*exception_gchandle != INVALID_GCHANDLE)
		return NULL;

	return convert (value, context, exception_gchandle);
}

NSValue *
xamarin_convert_managed_to_nsvalue (MonoObject *value, MonoClass *managedType, MonoMethod *method, void *context, GCHandle *exception_gchandle)
{
	xamarin_managed_to_id_func convert = xamarin_get_managed_to_nsvalue_func (managedType, method, exception_gchandle);
	if (*exception_gchandle != INVALID_GCHANDLE)
		return NULL;

	return convert (value, context, exception_gchandle);
}

GCHandle
xamarin_create_bindas_exception (MonoType *inputType, MonoType *outputType, MonoMethod *method)
{
	GCHandle exception_gchandle;
	char *to_name = NULL;
	char *from_name = NULL;
	char *method_full_name = NULL;
	char *msg = NULL;

	from_name = xamarin_type_get_full_name (inputType, &exception_gchandle);
	if (exception_gchandle != INVALID_GCHANDLE)
		goto exception_handling;
	to_name = xamarin_type_get_full_name (outputType, &exception_gchandle);
	if (exception_gchandle != INVALID_GCHANDLE)
		goto exception_handling;

	method_full_name = mono_method_full_name (method, TRUE);
	msg = xamarin_strdup_printf ("Internal error: can't convert from '%s' to '%s' in %s. Please file a bug report with a test case (https://github.com/xamarin/xamarin-macios/issues/new).",
										from_name, to_name, method_full_name);
	exception_gchandle = xamarin_gchandle_new ((MonoObject *) xamarin_create_exception (msg), false);

exception_handling:
	xamarin_free (to_name);
	xamarin_free (from_name);
	xamarin_free (method_full_name);
	xamarin_free (msg);
	return exception_gchandle;
}

const char *
xamarin_skip_type_name (const char *ptr)
{
	const char *t = ptr;
	do {
		if (*t == '=') {
			t++;
			return t;
		}
		t++;
	} while (*t != 0);

	return ptr;
}
