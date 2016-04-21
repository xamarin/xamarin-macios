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

#include "delegates.h"
#include "xamarin/xamarin.h"
#include "slinked-list.h"
#include "trampolines-internal.h"
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
xamarin_marshal_return_value (MonoType *mtype, const char *type, MonoObject *retval, bool retain, MonoMethod *method)
{
	/* Any changes in this method probably need to be reflected in the static registrar as well */
	switch (type [0]) {
		case _C_CLASS:
			return xamarin_get_class_handle (retval);

		case _C_SEL:
			return xamarin_get_selector_handle (retval);

		case _C_PTR: {
			MonoClass *klass = mono_class_from_mono_type (mtype);
			if (mono_class_is_delegate (klass)) {
				return xamarin_get_block_for_delegate (method, retval);
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
						v = xamarin_get_handle (value);
					}
					buf[i] = v;
				}

				arr = [[NSArray alloc] initWithObjects: buf count: length];

				free (buf);

				if (!retain)
					[arr autorelease];
				
				return (void *) arr;
			} else if (xamarin_is_class_nsobject (r_klass)) {
				id i = xamarin_get_handle (retval);

				xamarin_framework_peer_lock ();
				[i retain];
				xamarin_framework_peer_unlock ();
				if (!retain)
					[i autorelease];

				mt_dummy_use (retval);
				return i;
			} else if (xamarin_is_class_inativeobject (r_klass)) {
				return xamarin_get_handle_for_inativeobject (retval);
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

#if defined (__i386__)
int
xamarin_get_frame_length (id self, SEL sel)
{
	NSMethodSignature *sig = [self methodSignatureForSelector: sel];

	return [sig frameLength];
}
#endif

static inline void
find_objc_method_implementation (struct objc_super *sup, id self, SEL sel, IMP xamarin_impl)
{
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
	struct objc_super sup;
	find_objc_method_implementation (&sup, self, sel, xamarin_impl);
	return objc_msgSendSuper (&sup, sel);
}

#if MONOMAC
id
xamarin_copyWithZone_trampoline1 (id self, SEL sel, NSZone *zone)
{
	// This is for subclasses that themselves do not implement Copy (NSZone)

	id rv;
	int gchandle;
	struct objc_super sup;

#if defined (DEBUG_REF_COUNTING)
	NSLog (@"xamarin_copyWithZone_trampoline1 (%p, %s, %p)\n", self, sel_getName (sel), zone);
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
	// This is for subclasses that already implement Copy (NSZone)

	id rv;
	int gchandle;

#if defined (DEBUG_REF_COUNTING)
	NSLog (@"xamarin_copyWithZone_trampoline2 (%p, %s, %p)\n", self, sel_getName (sel), zone);
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
	int ref_count;
	bool detach = false;

	pthread_mutex_lock (&refcount_mutex);

	ref_count = [self retainCount];

#if defined(DEBUG_REF_COUNTING)
	NSLog (@"xamarin_release_trampoline (%s Handle=%p) retainCount=%d; HasManagedRef=%i GCHandle=%i\n", 
		class_getName ([self class]), self, ref_count, xamarin_has_managed_ref (self), xamarin_get_gchandle (self));
#endif
	
	/*
	 * We need to decide if the gchandle should become a weak one.
	 * This happens if managed code will end up holding the only ref.
	 */

	if (ref_count == 2 && xamarin_has_managed_ref (self)) {
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
	if (mono_domain_get () == NULL) {
		/* This is needed because we call into managed code below (xamarin_unregister_nsobject) */
		mono_jit_thread_attach (NULL);
	}
	/* Object is about to die. Unregister it and free any gchandles we may have */
	MonoObject *mobj = mono_gchandle_get_target (gchandle);
#if defined(DEBUG_REF_COUNTING)
	NSLog (@"xamarin_notify_dealloc (%p, %i) target: %p\n", self, gchandle, mobj);
#endif
	xamarin_free_gchandle (self, gchandle);
	xamarin_unregister_nsobject (self, mobj);
	mono_thread_detach_if_exiting ();
}

id
xamarin_retain_trampoline (id self, SEL sel)
{
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
	NSLog (@"xamarin_retain_trampoline  (%s Handle=%p) initial retainCount=%d; new retainCount=%d HadManagedRef=%i HasManagedRef=%i old GCHandle=%i new GCHandle=%i\n", 
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
	/* This is for types registered using the dynamic registrar */
	int gc_handle = 0;
	pthread_mutex_lock (&gchandle_hash_lock);
	if (gchandle_hash != NULL)
		gc_handle = GPOINTER_TO_INT (CFDictionaryGetValue (gchandle_hash, self));
	pthread_mutex_unlock (&gchandle_hash_lock);
	return gc_handle;
}
