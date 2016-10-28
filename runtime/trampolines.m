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

#include <pthread.h>

#include "product.h"
#include "delegates.h"
#include "xamarin/xamarin.h"
#include "slinked-list.h"
#include "trampolines-internal.h"
#include "runtime-internal.h"
//#define DEBUG_REF_COUNTING

static pthread_mutex_t refcount_mutex;
static void
x_init_mutex () __attribute__ ((constructor));

static void
x_init_mutex ()
{
	pthread_mutexattr_t attr;
	pthread_mutexattr_init (&attr);
	pthread_mutexattr_settype (&attr, PTHREAD_MUTEX_RECURSIVE);
	pthread_mutex_init (&refcount_mutex, &attr);
}

void *
xamarin_marshal_return_value (MonoType *mtype, const char *type, MonoObject *retval, bool retain, MonoMethod *method, guint32 *exception_gchandle)
{
	// COOP: accesses managed memory: unsafe mode.
	MONO_ASSERT_GC_UNSAFE;
	
	/* Any changes in this method probably need to be reflected in the static registrar as well */
	switch (type [0]) {
		case _C_CLASS:
			return xamarin_get_class_handle (retval, exception_gchandle);

		case _C_SEL:
			return xamarin_get_selector_handle (retval, exception_gchandle);

		case _C_PTR: {
			MonoClass *klass = mono_class_from_mono_type (mtype);
			if (mono_class_is_delegate (klass)) {
				return xamarin_get_block_for_delegate (method, retval, exception_gchandle);
			} else {
				return *(void **) mono_object_unbox (retval);
			}
		}
		case _C_ID: {
			MonoClass *r_klass = mono_object_get_class ((MonoObject *) retval);
			if (r_klass == mono_get_string_class ()) {
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
				id *buf = (id *) malloc (sizeof (void *) * length);
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
						if (*exception_gchandle != 0)
							 return NULL;
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
