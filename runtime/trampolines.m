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

void *
xamarin_marshal_return_value (MonoType *mtype, const char *type, MonoObject *retval, bool retain, MonoMethod *method, MethodDescription *desc, guint32 *exception_gchandle)
{
	// COOP: accesses managed memory: unsafe mode.
	MONO_ASSERT_GC_UNSAFE;
	
	/* Any changes in this method probably need to be reflected in the static registrar as well */
	switch (type [0]) {
		case _C_CLASS:
			return xamarin_get_handle_for_inativeobject (retval, exception_gchandle);

		case _C_SEL:
			return xamarin_get_handle_for_inativeobject (retval, exception_gchandle);

		case _C_PTR: {
			MonoClass *klass = mono_class_from_mono_type (mtype);
			if (mono_class_is_delegate (klass)) {
				return xamarin_get_block_for_delegate (method, retval, NULL, exception_gchandle);
			} else {
				return *(void **) mono_object_unbox (retval);
			}
		}
		case _C_ID: {
			MonoClass *r_klass = mono_object_get_class ((MonoObject *) retval);

			if (desc && desc->bindas [0].original_type != NULL) {
				return xamarin_generate_conversion_to_native (retval, mono_class_get_type (r_klass), mono_reflection_type_get_type (desc->bindas [0].original_type), method, INVALID_TOKEN_REF, exception_gchandle);
			} else if (r_klass == mono_get_string_class ()) {
				char *str = mono_string_to_utf8 ((MonoString *) retval);
				NSString *rv = [[NSString alloc] initWithUTF8String:str];

				if (!retain)
					[rv autorelease];
				mono_free (str);
				return (void *) rv;
			} else if (xamarin_is_class_array (r_klass)) {
				MonoClass *e_klass = mono_class_get_element_class (r_klass);
				bool is_string = e_klass == mono_get_string_class ();
				MonoArray *m_arr = (MonoArray *) retval;
				int length = mono_array_length (m_arr);
				id *buf = (id *) malloc (sizeof (id) * length);
				NSArray *arr;
				int i;
				id v;

				for (i = 0; i < length; i++) {
					MonoObject *value = mono_array_get (m_arr, MonoObject *, i);
					
					if (is_string) {
						char *str = mono_string_to_utf8 ((MonoString *) value);
						NSString *sv = [[NSString alloc] initWithUTF8String:str];

						[sv autorelease];
						mono_free (str);

						v = sv;
					} else {
						v = xamarin_get_handle (value, exception_gchandle);
						if (*exception_gchandle != 0) {
							free (buf);
							 return NULL;
						}
					}
					buf[i] = v;
				}

				arr = [[NSArray alloc] initWithObjects: buf count: length];

				free (buf);

				if (!retain)
					[arr autorelease];
				
				return (void *) arr;
			} else if (xamarin_is_class_nsobject (r_klass)) {
				id i = xamarin_get_handle (retval, exception_gchandle);
				if (*exception_gchandle != 0)
					return NULL;

				xamarin_framework_peer_lock ();
				[i retain];
				xamarin_framework_peer_unlock ();
				if (!retain)
					[i autorelease];

				mt_dummy_use (retval);
				return i;
			} else if (xamarin_is_class_inativeobject (r_klass)) {
				return xamarin_get_handle_for_inativeobject (retval, exception_gchandle);
			} else {
				xamarin_assertion_message ("Don't know how to marshal a return value of type '%s.%s'. Please file a bug with a test case at http://bugzilla.xamarin.com\n", mono_class_get_namespace (r_klass), mono_class_get_name (r_klass)); 
			}
		}
		case _C_CHARPTR:
			return (void *) mono_string_to_utf8 ((MonoString *) retval);
		case _C_VOID:
			return (void *) 0x0;
		default:
			return *(void **) mono_object_unbox (retval);
	}
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

int
xamarin_get_frame_length (id self, SEL sel)
{
	if (self == NULL)
		return sizeof (void *) * 3; // we might be in objc_msgStret, in which case we'll need to copy three arguments.

	// [NSDecimalNumber initWithDecimal:] has this descriptor: "@36@0:8{?=b8b4b1b1b18[8S]}16"
	// which NSMethodSignature chokes on: NSInvalidArgumentException Reason: +[NSMethodSignature signatureWithObjCTypes:]: unsupported type encoding spec '{?}'
	// So instead parse the description ourselves.

	int length = 0;
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
			fprintf (stderr, PRODUCT ": Failed to calculate the frame size for the method [%s %s] (%s). Using a value of %i instead.\n", class_getName (cls), sel_getName (sel), [[ex description] UTF8String], length);
		}
	} else {
		// The format of the method type encoding is described here: http://stackoverflow.com/a/11492151/183422
		// the return type might have a number after it, which is the size of the argument frame
		// first get this number (if it's there), and use it as a minimum value for the frame length
		int rvlength = get_type_description_length (desc);
		int min_length = 0;
		if (rvlength > 0) {
			const char *min_start = desc + rvlength;
			// the number is at the end of the return type encoding, so find any numbers
			// at the end of the type encoding.
			while (min_start > desc && min_start [-1] >= '0' && min_start [-1] <= '9')
				min_start--;
			if (min_start < desc + rvlength) {
				for (int i = 0; i < desc + rvlength - min_start; i++)
					min_length = min_length * 10 + (min_start [i] - '0');
			}
		}

		// fprintf (stderr, "Found desc '%s' for [%s %s] with min frame length %i\n", desc, class_getName (cls), sel_getName (sel), min_length);

		// skip the return value.
		desc += rvlength;
		while (*desc) {
			int tl = xamarin_objc_type_size (desc);
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

	return length;
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
	return objc_msgSendSuper (&sup, sel);
}

#if MONOMAC
id
xamarin_copyWithZone_trampoline1 (id self, SEL sel, NSZone *zone)
{
	// COOP: does not access managed memory: any mode
	// This is for subclasses that themselves do not implement Copy (NSZone)

	id rv;
	int gchandle;
	struct objc_super sup;

#if defined (DEBUG_REF_COUNTING)
	PRINT ("xamarin_copyWithZone_trampoline1 (%p, %s, %p)\n", self, sel_getName (sel), zone);
#endif

	// Clear out our own GCHandle
	gchandle = xamarin_get_gchandle_with_flags (self);
	if (gchandle != 0)
		xamarin_set_gchandle (self, 0);

	// Call the base class implementation
	id (*invoke) (struct objc_super *, SEL, NSZone*) = (id (*)(struct objc_super *, SEL, NSZone*)) objc_msgSendSuper;
	find_objc_method_implementation (&sup, self, sel, (IMP) xamarin_copyWithZone_trampoline1);
	rv = invoke (&sup, sel, zone);

	// Restore our GCHandle
	if (gchandle != 0)
		xamarin_set_gchandle (self, gchandle);

	return rv;
}

id
xamarin_copyWithZone_trampoline2 (id self, SEL sel, NSZone *zone)
{
	// COOP: does not access managed memory: any mode
	// This is for subclasses that already implement Copy (NSZone)

	id rv;
	int gchandle;

#if defined (DEBUG_REF_COUNTING)
	PRINT ("xamarin_copyWithZone_trampoline2 (%p, %s, %p)\n", self, sel_getName (sel), zone);
#endif

	// Clear out our own GCHandle
	gchandle = xamarin_get_gchandle_with_flags (self);
	if (gchandle != 0)
		xamarin_set_gchandle (self, 0);

	// Call the managed implementation
	id (*invoke) (id, SEL, NSZone*) = (id (*)(id, SEL, NSZone*)) xamarin_trampoline;
	rv = invoke (self, sel, zone);

	// Restore our GCHandle
	if (gchandle != 0)
		xamarin_set_gchandle (self, gchandle);

	return rv;
}
#endif

void
xamarin_release_trampoline (id self, SEL sel)
{
	// COOP: does not access managed memory: any mode, but it assumes safe mode upon entry (it takes locks, and doesn't switch to safe mode).
	MONO_ASSERT_GC_SAFE;
	
	int ref_count;
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

	/* Invoke the real retain method */
	xamarin_invoke_objc_method_implementation (self, sel, (IMP) xamarin_release_trampoline);

	if (detach)
		mono_thread_detach_if_exiting ();
}

void
xamarin_notify_dealloc (id self, int gchandle)
{
	guint32 exception_gchandle = 0;

	// COOP: safe mode upon entry, switches to unsafe when acccessing managed memory.
	MONO_ASSERT_GC_SAFE_OR_DETACHED;
	
	/* This is needed because we call into managed code below (xamarin_unregister_nsobject) */
	MONO_THREAD_ATTACH; // COOP: This will swith to GC_UNSAFE

	/* Object is about to die. Unregister it and free any gchandles we may have */
	MonoObject *mobj = mono_gchandle_get_target (gchandle);
#if defined(DEBUG_REF_COUNTING)
	PRINT ("xamarin_notify_dealloc (%p, %i) target: %p\n", self, gchandle, mobj);
#endif
	xamarin_free_gchandle (self, gchandle);
	xamarin_unregister_nsobject (self, mobj, &exception_gchandle);

	MONO_THREAD_DETACH; // COOP: This will switch to GC_SAFE

	xamarin_process_managed_exception_gchandle (exception_gchandle);

	mono_thread_detach_if_exiting ();
}

id
xamarin_retain_trampoline (id self, SEL sel)
{
	// COOP: safe mode upon entry, switches to unsafe when acccessing managed memory.
	MONO_ASSERT_GC_SAFE;

	pthread_mutex_lock (&refcount_mutex);

#if defined(DEBUG_REF_COUNTING)
	int ref_count = [self retainCount];
	bool had_managed_ref = xamarin_has_managed_ref (self);
	int pre_gchandle = xamarin_get_gchandle (self);
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

static const char *associated_key = "x"; // the string value doesn't matter, only the pointer value.
void
xamarin_set_gchandle_trampoline (id self, SEL sel, int gc_handle)
{
	// COOP: Called by ObjC (when the setGCHandle: selector is called on an object).
	// COOP: Safe mode upon entry, and doesn't access managed memory, so no need to change.
	MONO_ASSERT_GC_SAFE;
	
	/* This is for types registered using the dynamic registrar */
	XamarinAssociatedObject *obj;
	obj = objc_getAssociatedObject (self, associated_key);
	if (obj == NULL && gc_handle != 0) {
		obj = [[XamarinAssociatedObject alloc] init];
		obj->gc_handle = gc_handle;
		obj->native_object = self;
		objc_setAssociatedObject (self, associated_key, obj, OBJC_ASSOCIATION_RETAIN_NONATOMIC);
		[obj release];
	}

	if (obj != NULL)
		obj->gc_handle = gc_handle;
	
	pthread_mutex_lock (&gchandle_hash_lock);
	if (gchandle_hash == NULL)
		gchandle_hash = CFDictionaryCreateMutable (kCFAllocatorDefault, 0, NULL, NULL);
	if (gc_handle == 0) {
		CFDictionaryRemoveValue (gchandle_hash, self);
	} else {
		CFDictionarySetValue (gchandle_hash, self, GINT_TO_POINTER (gc_handle));
	}
	pthread_mutex_unlock (&gchandle_hash_lock);
}

int
xamarin_get_gchandle_trampoline (id self, SEL sel)
{
	// COOP: Called by ObjC (when the getGCHandle selector is called on an object).
	// COOP: Safe mode upon entry, and doesn't access managed memory, so no need to switch.
	MONO_ASSERT_GC_SAFE;
	
	/* This is for types registered using the dynamic registrar */
	int gc_handle = 0;
	pthread_mutex_lock (&gchandle_hash_lock);
	if (gchandle_hash != NULL)
		gc_handle = GPOINTER_TO_INT (CFDictionaryGetValue (gchandle_hash, self));
	pthread_mutex_unlock (&gchandle_hash_lock);
	return gc_handle;
}

id
xamarin_generate_conversion_to_native (MonoObject *value, MonoType *inputType, MonoType *outputType, MonoMethod *method, guint32 context, guint32 *exception_gchandle)
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

	bool isManagedArray = xamarin_is_class_array (managedType);
	bool isNativeArray = xamarin_is_class_array (nativeType);

	MonoClass *nullableManagedType = NULL;
	bool isManagedNullable = xamarin_is_class_nullable (managedType, &nullableManagedType, exception_gchandle);
	if (*exception_gchandle != 0)
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
		underlyingNativeType = mono_class_get_element_class (nativeType);
		underlyingManagedType = mono_class_get_element_class (managedType);
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
		if (*exception_gchandle != 0)
			goto exception_handling;

		if (isManagedArray) {
			convertedValue = xamarin_convert_managed_to_nsarray_with_func ((MonoArray *) value, func, context, exception_gchandle);
			if (*exception_gchandle != 0)
				goto exception_handling;
		} else {
			convertedValue = func (value, context, exception_gchandle);
			if (*exception_gchandle != 0)
				goto exception_handling;
		}
	}

exception_handling:

	return convertedValue;
}


void *
xamarin_generate_conversion_to_managed (id value, MonoType *inputType, MonoType *outputType, MonoMethod *method, guint32 *exception_gchandle, guint32 context, /*SList*/ void **free_list)
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

	bool isManagedArray = xamarin_is_class_array (managedType);
	bool isNativeArray = xamarin_is_class_array (nativeType);

	MonoClass *nullableManagedType = NULL;
	bool isManagedNullable = xamarin_is_class_nullable (managedType, &nullableManagedType, exception_gchandle);
	if (*exception_gchandle != 0)
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
		underlyingNativeType = mono_class_get_element_class (nativeType);
		underlyingManagedType = mono_class_get_element_class (managedType);
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
		if (*exception_gchandle != 0)
			goto exception_handling;

		if (isManagedArray) {
			convertedValue = xamarin_convert_nsarray_to_managed_with_func (value, underlyingManagedType, func, context, exception_gchandle);
			if (*exception_gchandle != 0)
				goto exception_handling;
		} else {
			convertedValue = func (value, NULL, underlyingManagedType, context, exception_gchandle);
			if (*exception_gchandle != 0)
				goto exception_handling;
			*(SList **) free_list = s_list_prepend (*(SList **) free_list, convertedValue);

			if (isManagedNullable)
				convertedValue = mono_value_box (mono_domain_get (), underlyingManagedType, convertedValue);
		}
	}

exception_handling:

	return convertedValue;
}

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wunguarded-availability-new"

// Returns a pointer to the value type, which must be freed using xamarin_free.
// If called multiple times in succession, the returned pointer can be passed as the second ptr argument, and it need only be freed once done iterating.
void *xamarin_nsnumber_to_bool   (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {       BOOL *valueptr =       (BOOL *) (ptr ? ptr : xamarin_calloc (sizeof (BOOL)));       *valueptr = [number boolValue];             return valueptr; }
void *xamarin_nsnumber_to_sbyte  (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {     int8_t *valueptr =     (int8_t *) (ptr ? ptr : xamarin_calloc (sizeof (int8_t)));     *valueptr = [number charValue];             return valueptr; }
void *xamarin_nsnumber_to_byte   (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {    uint8_t *valueptr =    (uint8_t *) (ptr ? ptr : xamarin_calloc (sizeof (uint8_t)));    *valueptr = [number unsignedCharValue];     return valueptr; }
void *xamarin_nsnumber_to_short  (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {    int16_t *valueptr =    (int16_t *) (ptr ? ptr : xamarin_calloc (sizeof (int16_t)));    *valueptr = [number shortValue];            return valueptr; }
void *xamarin_nsnumber_to_ushort (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {   uint16_t *valueptr =   (uint16_t *) (ptr ? ptr : xamarin_calloc (sizeof (uint16_t)));   *valueptr = [number unsignedShortValue];    return valueptr; }
void *xamarin_nsnumber_to_int    (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {    int32_t *valueptr =    (int32_t *) (ptr ? ptr : xamarin_calloc (sizeof (int32_t)));    *valueptr = [number intValue];              return valueptr; }
void *xamarin_nsnumber_to_uint   (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {   uint32_t *valueptr =   (uint32_t *) (ptr ? ptr : xamarin_calloc (sizeof (uint32_t)));   *valueptr = [number unsignedIntValue];      return valueptr; }
void *xamarin_nsnumber_to_long   (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {    int64_t *valueptr =    (int64_t *) (ptr ? ptr : xamarin_calloc (sizeof (int64_t)));    *valueptr = [number longLongValue];         return valueptr; }
void *xamarin_nsnumber_to_ulong  (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {   uint64_t *valueptr =   (uint64_t *) (ptr ? ptr : xamarin_calloc (sizeof (uint64_t)));   *valueptr = [number unsignedLongLongValue]; return valueptr; }
void *xamarin_nsnumber_to_nint   (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {  NSInteger *valueptr =  (NSInteger *) (ptr ? ptr : xamarin_calloc (sizeof (NSInteger)));  *valueptr = [number integerValue];          return valueptr; }
void *xamarin_nsnumber_to_nuint  (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) { NSUInteger *valueptr = (NSUInteger *) (ptr ? ptr : xamarin_calloc (sizeof (NSUInteger))); *valueptr = [number unsignedIntegerValue];  return valueptr; }
void *xamarin_nsnumber_to_float  (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {      float *valueptr =      (float *) (ptr ? ptr : xamarin_calloc (sizeof (float)));      *valueptr = [number floatValue];            return valueptr; }
void *xamarin_nsnumber_to_double (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {     double *valueptr =     (double *) (ptr ? ptr : xamarin_calloc (sizeof (double)));     *valueptr = [number doubleValue];           return valueptr; }
#if __POINTER_WIDTH__ == 32
void *xamarin_nsnumber_to_nfloat (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {      float *valueptr =      (float *) (ptr ? ptr : xamarin_calloc (sizeof (float)));      *valueptr = [number floatValue];            return valueptr; }
#elif __POINTER_WIDTH__ == 64
void *xamarin_nsnumber_to_nfloat (NSNumber *number, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {     double *valueptr =     (double *) (ptr ? ptr : xamarin_calloc (sizeof (double)));     *valueptr = [number doubleValue];           return valueptr; }
#else
	#error Invalid pointer size.
#endif

// Returns a pointer to the value type, which must be freed using xamarin_free.
// If called multiple times in succession, the returned pointer can be passed as the second ptr argument, and it need only be freed once done iterating.
void *xamarin_nsvalue_to_nsrange                (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {                NSRange *valueptr =                (NSRange *) (ptr ? ptr : xamarin_calloc (sizeof (NSRange)));                *valueptr = [value rangeValue];             return valueptr; }
#if HAVE_UIKIT // Yep, these CoreGraphics-looking category method is defined in UIKit.
void *xamarin_nsvalue_to_cgaffinetransform      (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {      CGAffineTransform *valueptr =      (CGAffineTransform *) (ptr ? ptr : xamarin_calloc (sizeof (CGAffineTransform)));      *valueptr = [value CGAffineTransformValue]; return valueptr; }
void *xamarin_nsvalue_to_cgpoint                (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {                CGPoint *valueptr =                (CGPoint *) (ptr ? ptr : xamarin_calloc (sizeof (CGPoint)));                *valueptr = [value CGPointValue];           return valueptr; }
void *xamarin_nsvalue_to_cgrect                 (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {                 CGRect *valueptr =                 (CGRect *) (ptr ? ptr : xamarin_calloc (sizeof (CGRect)));                 *valueptr = [value CGRectValue];            return valueptr; }
void *xamarin_nsvalue_to_cgsize                 (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {                 CGSize *valueptr =                 (CGSize *) (ptr ? ptr : xamarin_calloc (sizeof (CGSize)));                 *valueptr = [value CGSizeValue];            return valueptr; }
void *xamarin_nsvalue_to_cgvector               (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {               CGVector *valueptr =               (CGVector *) (ptr ? ptr : xamarin_calloc (sizeof (CGVector)));               *valueptr = [value CGVectorValue];          return valueptr; }
void *xamarin_nsvalue_to_nsdirectionaledgeinsets(NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {NSDirectionalEdgeInsets *valueptr =(NSDirectionalEdgeInsets *) (ptr ? ptr : xamarin_calloc (sizeof (NSDirectionalEdgeInsets)));*valueptr = [value directionalEdgeInsetsValue];return valueptr; }
#endif
#if HAVE_COREANIMATION
void *xamarin_nsvalue_to_catransform3d          (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {          CATransform3D *valueptr =          (CATransform3D *) (ptr ? ptr : xamarin_calloc (sizeof (CATransform3D)));          *valueptr = [value CATransform3DValue];     return valueptr; }
#endif
#if HAVE_MAPKIT // Yep, this is defined in MapKit.
void *xamarin_nsvalue_to_cllocationcoordinate2d (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) { CLLocationCoordinate2D *valueptr = (CLLocationCoordinate2D *) (ptr ? ptr : xamarin_calloc (sizeof (CLLocationCoordinate2D))); *valueptr = [value MKCoordinateValue];      return valueptr; }
#endif
#if HAVE_COREMEDIA
void *xamarin_nsvalue_to_cmtime                 (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {                 CMTime *valueptr =                 (CMTime *) (ptr ? ptr : xamarin_calloc (sizeof (CMTime)));                 *valueptr = [value CMTimeValue];            return valueptr; }
void *xamarin_nsvalue_to_cmtimemapping          (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {          CMTimeMapping *valueptr =          (CMTimeMapping *) (ptr ? ptr : xamarin_calloc (sizeof (CMTimeMapping)));          *valueptr = [value CMTimeMappingValue];     return valueptr; }
void *xamarin_nsvalue_to_cmtimerange            (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {            CMTimeRange *valueptr =            (CMTimeRange *) (ptr ? ptr : xamarin_calloc (sizeof (CMTimeRange)));            *valueptr = [value CMTimeRangeValue];       return valueptr; }
#endif
#if HAVE_MAPKIT
void *xamarin_nsvalue_to_mkcoordinatespan       (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {       MKCoordinateSpan *valueptr =       (MKCoordinateSpan *) (ptr ? ptr : xamarin_calloc (sizeof (MKCoordinateSpan)));       *valueptr = [value MKCoordinateSpanValue];  return valueptr; }
#endif
void *xamarin_nsvalue_to_scnmatrix4             (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {             SCNMatrix4 *valueptr =             (SCNMatrix4 *) (ptr ? ptr : xamarin_calloc (sizeof (SCNMatrix4)));             *valueptr = [value SCNMatrix4Value];        return valueptr; }
void *
xamarin_nsvalue_to_scnvector3 (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle)
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
void *xamarin_nsvalue_to_scnvector4             (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {             SCNVector4 *valueptr =             (SCNVector4 *) (ptr ? ptr : xamarin_calloc (sizeof (SCNVector4)));             *valueptr = [value SCNVector4Value];        return valueptr; }
#if HAVE_UIKIT
void *xamarin_nsvalue_to_uiedgeinsets           (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {           UIEdgeInsets *valueptr =           (UIEdgeInsets *) (ptr ? ptr : xamarin_calloc (sizeof (UIEdgeInsets)));           *valueptr = [value UIEdgeInsetsValue];      return valueptr; }
void *xamarin_nsvalue_to_uioffset               (NSValue *value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle) {               UIOffset *valueptr =               (UIOffset *) (ptr ? ptr : xamarin_calloc (sizeof (UIOffset)));               *valueptr = [value UIOffsetValue];          return valueptr; }
#endif

id xamarin_bool_to_nsnumber   (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithBool:                  *(BOOL *) mono_object_unbox (value)]; }
id xamarin_sbyte_to_nsnumber  (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithChar:                *(int8_t *) mono_object_unbox (value)]; }
id xamarin_byte_to_nsnumber   (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithUnsignedChar:       *(uint8_t *) mono_object_unbox (value)]; }
id xamarin_short_to_nsnumber  (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithShort:              *(int16_t *) mono_object_unbox (value)]; }
id xamarin_ushort_to_nsnumber (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithUnsignedShort:     *(uint16_t *) mono_object_unbox (value)]; }
id xamarin_int_to_nsnumber    (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithInt:                *(int32_t *) mono_object_unbox (value)]; }
id xamarin_uint_to_nsnumber   (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithUnsignedInt:       *(uint32_t *) mono_object_unbox (value)]; }
id xamarin_long_to_nsnumber   (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithLongLong:           *(int64_t *) mono_object_unbox (value)]; }
id xamarin_ulong_to_nsnumber  (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithUnsignedLongLong:  *(uint64_t *) mono_object_unbox (value)]; }
id xamarin_nint_to_nsnumber   (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithInteger:          *(NSInteger *) mono_object_unbox (value)]; }
id xamarin_nuint_to_nsnumber  (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithUnsignedInteger: *(NSUInteger *) mono_object_unbox (value)]; }
id xamarin_float_to_nsnumber  (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithFloat:                *(float *) mono_object_unbox (value)]; }
id xamarin_double_to_nsnumber (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithDouble:              *(double *) mono_object_unbox (value)]; }
#if __POINTER_WIDTH__ == 32
id xamarin_nfloat_to_nsnumber (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithFloat:                *(float *) mono_object_unbox (value)]; }
#elif __POINTER_WIDTH__ == 64
id xamarin_nfloat_to_nsnumber (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSNumber numberWithDouble:              *(double *) mono_object_unbox (value)]; }
#else
	#error Invalid pointer size.
#endif

id xamarin_nsrange_to_nsvalue                (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithRange:               *(NSRange *)                mono_object_unbox (value)]; }
#if HAVE_UIKIT // yep, these CoreGraphics-looking category methods are defined in UIKit
id xamarin_cgaffinetransform_to_nsvalue      (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithCGAffineTransform:   *(CGAffineTransform *)      mono_object_unbox (value)]; }
id xamarin_cgpoint_to_nsvalue                (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithCGPoint:             *(CGPoint *)                mono_object_unbox (value)]; }
id xamarin_cgrect_to_nsvalue                 (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithCGRect:              *(CGRect *)                 mono_object_unbox (value)]; }
id xamarin_cgsize_to_nsvalue                 (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithCGSize:              *(CGSize *)                 mono_object_unbox (value)]; }
id xamarin_cgvector_to_nsvalue               (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithCGVector:            *(CGVector *)               mono_object_unbox (value)]; }
id xamarin_nsdirectionaledgeinsets_to_nsvalue(MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithDirectionalEdgeInsets:*(NSDirectionalEdgeInsets *)mono_object_unbox (value)]; }
#endif
#if HAVE_COREANIMATION
id xamarin_catransform3d_to_nsvalue          (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithCATransform3D:       *(CATransform3D *)          mono_object_unbox (value)]; }
#endif
#if HAVE_MAPKIT // Yep, this is defined in MapKit.
id xamarin_cllocationcoordinate2d_to_nsvalue (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithMKCoordinate:        *(CLLocationCoordinate2D *) mono_object_unbox (value)]; }
#endif
#if HAVE_COREMEDIA
id xamarin_cmtime_to_nsvalue                 (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithCMTime:              *(CMTime *)                 mono_object_unbox (value)]; }
id xamarin_cmtimemapping_to_nsvalue          (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithCMTimeMapping:       *(CMTimeMapping *)          mono_object_unbox (value)]; }
id xamarin_cmtimerange_to_nsvalue            (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithCMTimeRange:         *(CMTimeRange *)            mono_object_unbox (value)]; }
#endif
#if HAVE_MAPKIT
id xamarin_mkcoordinatespan_to_nsvalue       (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithMKCoordinateSpan:    *(MKCoordinateSpan *)       mono_object_unbox (value)]; }
#endif
id xamarin_scnmatrix4_to_nsvalue             (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithSCNMatrix4:          *(SCNMatrix4 *)             mono_object_unbox (value)]; }
id xamarin_scnvector3_to_nsvalue             (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithSCNVector3:          *(SCNVector3 *)             mono_object_unbox (value)]; }
id xamarin_scnvector4_to_nsvalue             (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithSCNVector4:          *(SCNVector4 *)             mono_object_unbox (value)]; }
#if HAVE_UIKIT
id xamarin_uiedgeinsets_to_nsvalue           (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithUIEdgeInsets:        *(UIEdgeInsets *)           mono_object_unbox (value)]; }
id xamarin_uioffset_to_nsvalue               (MonoObject *value, guint32 context, guint32 *exception_gchandle) { return [NSValue valueWithUIOffset:            *(UIOffset *)               mono_object_unbox (value)]; }
#endif

#pragma clang diagnostic pop

static void *
xamarin_get_nsnumber_converter (MonoClass *managedType, MonoMethod *method, bool to_managed, guint32 *exception_gchandle)
{
	int type;
	void * func = NULL;
	char *fullname = xamarin_class_get_full_name (managedType, exception_gchandle);
	if (*exception_gchandle != 0)
		goto exception_handling;

	type = mono_type_get_type (mono_class_get_type (managedType));

	switch (type) {
	case MONO_TYPE_I1:
		func = to_managed ? (void *) xamarin_nsnumber_to_sbyte : (void *) xamarin_sbyte_to_nsnumber;
		break;
	case MONO_TYPE_U1:
		func = to_managed ? (void *) xamarin_nsnumber_to_byte : (void *) xamarin_byte_to_nsnumber;
		break;
	case MONO_TYPE_I2:
		func = to_managed ? (void *) xamarin_nsnumber_to_short : (void *) xamarin_short_to_nsnumber;
		break;
	case MONO_TYPE_U2:
		func = to_managed ? (void *) xamarin_nsnumber_to_ushort : (void *) xamarin_ushort_to_nsnumber;
		break;
	case MONO_TYPE_I4:
		func = to_managed ? (void *) xamarin_nsnumber_to_int : (void *) xamarin_int_to_nsnumber;
		break;
	case MONO_TYPE_U4:
		func = to_managed ? (void *) xamarin_nsnumber_to_uint : (void *) xamarin_uint_to_nsnumber;
		break;
	case MONO_TYPE_I8:
		func = to_managed ? (void *) xamarin_nsnumber_to_long : (void *) xamarin_long_to_nsnumber;
		break;
	case MONO_TYPE_U8:
		func = to_managed ? (void *) xamarin_nsnumber_to_ulong : (void *) xamarin_ulong_to_nsnumber;
		break;
	case MONO_TYPE_R4:
		func = to_managed ? (void *) xamarin_nsnumber_to_float : (void *) xamarin_float_to_nsnumber;
		break;
	case MONO_TYPE_R8:
		func = to_managed ? (void *) xamarin_nsnumber_to_double : (void *) xamarin_double_to_nsnumber;
		break;
	case MONO_TYPE_BOOLEAN:
		func = to_managed ? (void *) xamarin_nsnumber_to_bool : (void *) xamarin_bool_to_nsnumber;
		break;
	default:
		if (!strcmp (fullname, "System.nint")) {
			func = to_managed ? (void *) xamarin_nsnumber_to_nint : (void *) xamarin_nint_to_nsnumber;
		} else if (!strcmp (fullname, "System.nuint")) {
			func = to_managed ? (void *) xamarin_nsnumber_to_nuint : (void *) xamarin_nuint_to_nsnumber;
		} else if (!strcmp (fullname, "System.nfloat")) {
			func = to_managed ? (void *) xamarin_nsnumber_to_nfloat : (void *) xamarin_nfloat_to_nsnumber;
		} else if (mono_class_is_enum (managedType)) {
			func = xamarin_get_nsnumber_converter (mono_class_from_mono_type (mono_class_enum_basetype (managedType)), method, to_managed, exception_gchandle);
		} else {
			*exception_gchandle = xamarin_create_bindas_exception (mono_class_get_type (managedType), mono_class_get_type (xamarin_get_nsnumber_class ()), method);
			goto exception_handling;
		}
	}

exception_handling:
	xamarin_free (fullname);

	return func;
}

static void *
xamarin_get_nsvalue_converter (MonoClass *managedType, MonoMethod *method, bool to_managed, guint32 *exception_gchandle)
{
	void * func = NULL;
	char *fullname = xamarin_class_get_full_name (managedType, exception_gchandle);
	if (*exception_gchandle != 0)
		goto exception_handling;

#if MONOMAC
	if (xamarin_use_new_assemblies && !strncmp (fullname, "MonoMac.", 8))
		memmove (fullname, fullname + 8, strlen (fullname) - 7 /* also copy the null char */);
#endif

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
		*exception_gchandle = xamarin_create_bindas_exception (mono_class_get_type (managedType), mono_class_get_type (xamarin_get_nsvalue_class ()), method);
		goto exception_handling;
	}

exception_handling:
	xamarin_free (fullname);

	return func;
}

xamarin_id_to_managed_func
xamarin_get_nsnumber_to_managed_func (MonoClass *managedType, MonoMethod *method, guint32 *exception_gchandle)
{
	return (xamarin_id_to_managed_func) xamarin_get_nsnumber_converter (managedType, method, true, exception_gchandle);
}

xamarin_managed_to_id_func
xamarin_get_managed_to_nsnumber_func (MonoClass *managedType, MonoMethod *method, guint32 *exception_gchandle)
{
	return (xamarin_managed_to_id_func) xamarin_get_nsnumber_converter (managedType, method, false, exception_gchandle);
}

xamarin_id_to_managed_func
xamarin_get_nsvalue_to_managed_func (MonoClass *managedType, MonoMethod *method, guint32 *exception_gchandle)
{
	return (xamarin_id_to_managed_func) xamarin_get_nsvalue_converter (managedType, method, true, exception_gchandle);
}

xamarin_managed_to_id_func
xamarin_get_managed_to_nsvalue_func (MonoClass *managedType, MonoMethod *method, guint32 *exception_gchandle)
{
	return (xamarin_managed_to_id_func) xamarin_get_nsvalue_converter (managedType, method, false, exception_gchandle);
}

void *
xamarin_smart_enum_to_nsstring (MonoObject *value, guint32 context /* token ref */, guint32 *exception_gchandle)
{
	if (context == INVALID_TOKEN_REF) {
		// This requires the dynamic registrar to invoke the correct conversion function
		int handle = mono_gchandle_new (value, FALSE);
		NSString *rv = xamarin_convert_smart_enum_to_nsstring (GINT_TO_POINTER (handle), exception_gchandle);
		mono_gchandle_free (handle);
		return rv;
	} else {
		// The static registrar found the correct conversion function, and provided a token ref we can use
		// to find it (and invoke it), without needing the dynamic registrar.
		MonoMethod *managed_method;
		MonoObject *exception = NULL;
		MonoObject *retval;
		void *arg_ptrs [1];

		managed_method = xamarin_get_managed_method_for_token (context /* token ref */, exception_gchandle);
		if (*exception_gchandle != 0) return NULL;

		arg_ptrs [0] = mono_object_unbox (value);

		retval = mono_runtime_invoke (managed_method, NULL, arg_ptrs, &exception);

		if (exception) {
			*exception_gchandle = mono_gchandle_new (exception, FALSE);
			return NULL;
		}

		if (retval == NULL)
			return NULL;
		return xamarin_get_nsobject_handle (retval);

	}
}

void *
xamarin_nsstring_to_smart_enum (id value, void *ptr, MonoClass *managedType, guint32 context, guint32 *exception_gchandle)
{
	int gc_handle = 0;
	MonoObject *obj;

	if (context == INVALID_TOKEN_REF) {
		// This requires the dynamic registrar to invoke the correct conversion function
		void *rv = xamarin_convert_nsstring_to_smart_enum (value, mono_type_get_object (mono_domain_get (), mono_class_get_type (managedType)), exception_gchandle);
		if (*exception_gchandle != 0)
			return ptr;
		gc_handle = GPOINTER_TO_INT (rv);
		obj = mono_gchandle_get_target (gc_handle);
	} else {
		// The static registrar found the correct conversion function, and provided a token ref we can use
		// to find it (and invoke it), without needing the dynamic registrar.
		MonoMethod *managed_method;
		void *arg_ptrs [1];
		MonoObject *exception = NULL;

		managed_method = xamarin_get_managed_method_for_token (context /* token ref */, exception_gchandle);
		if (*exception_gchandle != 0) return NULL;

		arg_ptrs [0] = xamarin_get_nsobject_with_type_for_ptr (value, false, xamarin_get_parameter_type (managed_method, 0), exception_gchandle);
		if (*exception_gchandle != 0) return NULL;

		obj = mono_runtime_invoke (managed_method, NULL, arg_ptrs, &exception);

		if (exception) {
			*exception_gchandle = mono_gchandle_new (exception, FALSE);
			return NULL;
		}
	}

	int size = mono_class_value_size (managedType, NULL);
	if (!ptr)
		ptr = xamarin_calloc (size);
	void *value_ptr = mono_object_unbox (obj);
	memcpy (ptr, value_ptr, size);
	if (context == INVALID_TOKEN_REF)
		mono_gchandle_free (gc_handle);
	return ptr;
}

xamarin_id_to_managed_func
xamarin_get_nsstring_to_smart_enum_func (MonoClass *managedType, MonoMethod *method, guint32 *exception_gchandle)
{
	return xamarin_nsstring_to_smart_enum;
}

xamarin_managed_to_id_func
xamarin_get_smart_enum_to_nsstring_func (MonoClass *managedType, MonoMethod *method, guint32 *exception_gchandle)
{
	return (xamarin_managed_to_id_func) xamarin_smart_enum_to_nsstring;
}

NSArray *
xamarin_convert_managed_to_nsarray_with_func (MonoArray *array, xamarin_managed_to_id_func convert, guint32 context, guint32 *exception_gchandle)
{
	id *buf = NULL;
	NSArray *rv = NULL;

	if (array == NULL)
		return NULL;

	int length = mono_array_length (array);
	if (length == 0)
		return [NSArray array];

	buf = (id *) malloc (sizeof (id) * length);
	MonoClass *element_class = mono_class_get_element_class (mono_object_get_class ((MonoObject *) array));
	int element_size = mono_class_value_size (element_class, NULL);
	char *ptr = (char *) mono_array_addr_with_size (array, element_size, 0);
	for (int i = 0; i < length; i++) {
		MonoObject *value = mono_value_box (mono_domain_get (), element_class, ptr + element_size * i);
		buf [i] = convert (value, context, exception_gchandle);
		if (*exception_gchandle != 0)
			goto exception_handling;
	}
	rv = [NSArray arrayWithObjects: buf count: length];

exception_handling:
	free (buf);

	return rv;
}

MonoArray *
xamarin_convert_nsarray_to_managed_with_func (NSArray *array, MonoClass *managedElementType, xamarin_id_to_managed_func convert, guint32 context, guint32 *exception_gchandle)
{
	if (array == NULL)
		return NULL;

	int length = [array count];
	MonoArray *rv = mono_array_new (mono_domain_get (), managedElementType, length);

	if (length == 0)
		return rv;

	void *valueptr = NULL;
	int element_size = mono_class_value_size (managedElementType, NULL);
	char *ptr = (char *) mono_array_addr_with_size (rv, element_size, 0);
	for (int i = 0; i < length; i++) {
		valueptr = convert ([array objectAtIndex: i], valueptr, managedElementType, context, exception_gchandle);
		if (*exception_gchandle != 0)
			goto exception_handling;
		memcpy (ptr, valueptr, element_size);
		ptr += element_size;
	}

exception_handling:
	xamarin_free (valueptr);

	return rv;
}

NSNumber *
xamarin_convert_managed_to_nsnumber (MonoObject *value, MonoClass *managedType, MonoMethod *method, guint32 context, guint32 *exception_gchandle)
{
	xamarin_managed_to_id_func convert = xamarin_get_managed_to_nsnumber_func (managedType, method, exception_gchandle);
	if (*exception_gchandle != 0)
		return NULL;

	return convert (value, context, exception_gchandle);
}

NSValue *
xamarin_convert_managed_to_nsvalue (MonoObject *value, MonoClass *managedType, MonoMethod *method, guint32 context, guint32 *exception_gchandle)
{
	xamarin_managed_to_id_func convert = xamarin_get_managed_to_nsvalue_func (managedType, method, exception_gchandle);
	if (*exception_gchandle != 0)
		return NULL;

	return convert (value, context, exception_gchandle);
}

guint32
xamarin_create_bindas_exception (MonoType *inputType, MonoType *outputType, MonoMethod *method)
{
	guint32 exception_gchandle;
	char *to_name = NULL;
	char *from_name = NULL;
	char *method_full_name = NULL;
	char *msg = NULL;

	from_name = xamarin_type_get_full_name (inputType, &exception_gchandle);
	if (exception_gchandle != 0)
		goto exception_handling;
	to_name = xamarin_type_get_full_name (outputType, &exception_gchandle);
	if (exception_gchandle != 0)
		goto exception_handling;

	method_full_name = mono_method_full_name (method, TRUE);
	msg = xamarin_strdup_printf ("Internal error: can't convert from '%s' to '%s' in %s. Please file a bug report with a test case (https://bugzilla.xamarin.com).",
										from_name, to_name, method_full_name);
	exception_gchandle = mono_gchandle_new ((MonoObject *) xamarin_create_exception (msg), false);

exception_handling:
	xamarin_free (to_name);
	xamarin_free (from_name);
	xamarin_free (method_full_name);
	xamarin_free (msg);
	return exception_gchandle;
}
