//
// shared.m: Shared native code between Xamarin.iOS and Xamarin.Mac.
// 
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. 
//

#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>
#include <pthread.h>
#include <libkern/OSAtomic.h>

#import <Foundation/Foundation.h>

#include "xamarin/xamarin.h"
#include "shared.h"
#include "runtime-internal.h"

/*
 * XamarinNSThreadObject and xamarin_init_nsthread is a fix for a problem that
 * occurs because NSThread will retain the 'object' argument to the
 * initWithTarget:selector:object: selector, and release it using a tls dtor.
 * The issue is that calling release on that object may end up in managed code,
 * but calling the mono runtime from a tls dtor is not allowed.
 *
 * So we create a native wrapper object which is the object that ends up released
 * on the tls dtor, and then it forwards the release of the managed object to
 * the main thread.
 * 
 * Bug: https://bugzilla.xamarin.com/show_bug.cgi?id=13612
 */

@interface XamarinNSThreadObject : NSObject
{
	void *target;
	SEL selector;
	id argument;
	bool is_direct_binding;
}
-(id) initWithData: (void *) targ selector:(SEL) sel argument:(id) arg is_direct_binding:(bool) is_direct;
-(void) start: (id) arg;
-(void) dealloc;
@end

@implementation XamarinNSThreadObject
{
}
-(id) initWithData: (void *) targ selector:(SEL) sel argument:(id) arg is_direct_binding:(bool) is_direct;
{
	// COOP: no managed memory access: any mode.
	target = targ;
	if (is_direct)
		[((id) targ) retain];
	selector = sel;
	argument = [arg retain];
	is_direct_binding = is_direct;
	return self;
}

-(void) start: (id) arg;
{
	// COOP: no managed memory access: any mode.
	if (is_direct_binding) {
		id (*invoke) (id, SEL, id) = (id (*)(id, SEL, id)) objc_msgSend;
		invoke ((id) target, selector, argument);
	} else {
		id (*invoke) (struct objc_super *, SEL, id) = (id (*)(struct objc_super *, SEL, id)) objc_msgSendSuper;
		invoke ((struct objc_super *) target, selector, argument);
	}
}

-(void) dealloc;
{
	// COOP: no managed memory access: any mode.
	// Cast to void* so that ObjC doesn't do any funky retain/release
	// on these objects when capturing them for the block, since we
	// may be on a thread where we cannot end up in the mono runtime,
	// and retain/release may do exactly that.
	void *targ = (void *) (is_direct_binding ? target : nil);
	void *arg = (void *) argument;
	dispatch_async (dispatch_get_main_queue (), ^{
		[((id) targ) release];
		[((id) arg) release];
	});
	[super dealloc];
}
@end

id
xamarin_init_nsthread (id obj, bool is_direct, id target, SEL sel, id arg)
{
	// COOP: no managed memory access: any mode.
	XamarinNSThreadObject *wrap = [[[XamarinNSThreadObject alloc] initWithData: target selector: sel argument: arg is_direct_binding: is_direct] autorelease];
	id (*invoke) (id, SEL, id, SEL, id) = (id (*)(id, SEL, id, SEL, id)) objc_msgSend;
	return invoke (obj, @selector(initWithTarget:selector:object:), wrap, @selector(start:), nil);
}

/* Protecting the Cocoa Frameworks
 *
 * To let Cocoa know that you intend to use multiple threads, all you have
 * to do is spawn a single thread using the NSThread class and let that
 * thread immediately exit. Your thread entry point need not do anything.
 * Just the act of spawning a thread using NSThread is enough to ensure that
 * the locks needed by the Cocoa frameworks are put in place.
 *
 * Source: https://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/Multithreading/CreatingThreads/CreatingThreads.html#//apple_ref/doc/uid/20000738-125024
 *
 * Ref: https://bugzilla.xamarin.com/show_bug.cgi?id=798
 */

@interface XamarinCocoaThreadInitializer : NSObject
{
	init_cocoa_func *the_func;
}
-(void) entryPoint: (NSObject *) obj;
-(id) initWithFunc: (init_cocoa_func *) func;
@end

@implementation XamarinCocoaThreadInitializer
{
}
-(void) entryPoint: (NSObject *) obj
{
	// COOP: no managed memory access: any mode.
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	if (the_func != NULL)
		the_func ();
	[pool drain];
}
-(id) initWithFunc: (init_cocoa_func *) func;
{
	// COOP: no managed memory access: any mode.
	self = [super init];
	if (self != nil) {
		the_func = func;
		[NSThread detachNewThreadSelector:@selector(entryPoint:) toTarget:self withObject:nil];
	}
	return self;
}
@end

void
xamarin_initialize_cocoa_threads (init_cocoa_func *func)
{
	// COOP: no managed memory access: any mode.
	[[[XamarinCocoaThreadInitializer alloc] initWithFunc: func] autorelease];
}

/* Wrapping threads with NSAutoreleasePool
 *
 * We must create an NSAutoreleasePool for each thread, so users
 * don't have to do it manually.
 * 
 * Use mono's profiling API to get notified for thread start/stop,
 * and create a pool that spans the thread's entire lifetime.
 */

static CFMutableDictionaryRef xamarin_thread_hash = NULL;
static pthread_mutex_t thread_hash_lock = PTHREAD_MUTEX_INITIALIZER;

static void
xamarin_thread_start (void *user_data)
{
	// COOP: no managed memory access: any mode. Switching to safe mode since we're locking a mutex.
	NSAutoreleasePool *pool;

	if (mono_thread_is_foreign (mono_thread_current ()))
		return;

	MONO_ENTER_GC_SAFE;

	pool = [[NSAutoreleasePool alloc] init];

	pthread_mutex_lock (&thread_hash_lock);

	CFDictionarySetValue (xamarin_thread_hash, GINT_TO_POINTER (pthread_self ()), pool);

	pthread_mutex_unlock (&thread_hash_lock);

	MONO_EXIT_GC_SAFE;
}
	
static void
xamarin_thread_finish (void *user_data)
{
	// COOP: no managed memory access: any mode. Switching to safe mode since we're locking a mutex.
	NSAutoreleasePool *pool;

	MONO_ENTER_GC_SAFE;

	/* Don't drain the pool while holding the thread hash lock. */
	pthread_mutex_lock (&thread_hash_lock);

	pool = (NSAutoreleasePool *) CFDictionaryGetValue (xamarin_thread_hash, GINT_TO_POINTER (pthread_self ()));
	if (pool)
		CFDictionaryRemoveValue (xamarin_thread_hash, GINT_TO_POINTER (pthread_self ()));

	pthread_mutex_unlock (&thread_hash_lock);

	if (pool)
		[pool drain];
		
	MONO_EXIT_GC_SAFE;
}

static void
thread_start (MonoProfiler *prof, uintptr_t tid)
{
	// COOP: no managed memory access: any mode.
	xamarin_thread_start (NULL);
}

static void
thread_end (MonoProfiler *prof, uintptr_t tid)
{
	// COOP: no managed memory access: any mode.
	xamarin_thread_finish (NULL);
}

void
xamarin_install_nsautoreleasepool_hooks ()
{
	// COOP: executed at startup (and no managed memory access): any mode.
	xamarin_thread_hash = CFDictionaryCreateMutable (kCFAllocatorDefault, 0, NULL, NULL);

	mono_profiler_install_thread (thread_start, thread_end);
}
	
/* Threads & Blocks
 * 
 * At the moment we can't execute managed code in the dispose method for a block (the process may deadlock,
 * depending on the thread the dispose method is called on).
 * 
 * ref: https://bugzilla.xamarin.com/show_bug.cgi?id=11286
 * ref: https://bugzilla.xamarin.com/show_bug.cgi?id=14954
 * 
 */ 

static void
xamarin_dispose_helper (void *a)
{
	// COOP: this method is executed by the ObjC runtime when a block must be freed.
	// COOP: it does not touch any managed memory (except to free a gchandle), so any mode goes.
	struct Block_literal *bl = (struct Block_literal *) a;
	int handle = GPOINTER_TO_INT (bl->global_handle);
	mono_gchandle_free (handle);
	bl->global_handle = GINT_TO_POINTER (-1);
	if (OSAtomicDecrement32Barrier (&bl->descriptor->ref_count) == 0) {
		free (bl->descriptor); // allocated using Marshal.AllocHGlobal.
	}
	bl->descriptor = NULL;
}

static void
xamarin_copy_helper (void *dst, void *src)
{
	// COOP: this method is executed by the ObjC runtime when a block must be copied.
	// COOP: it does not touch any managed memory (except to allocate a gchandle), so any mode goes.
	struct Block_literal *source = (struct Block_literal *) src;
	struct Block_literal *target = (struct Block_literal *) dst;
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wint-to-void-pointer-cast"
	target->global_handle = GINT_TO_POINTER (mono_gchandle_new (mono_gchandle_get_target (GPOINTER_TO_INT (source->local_handle)), FALSE));
#pragma clang diagnostic pop

	OSAtomicIncrement32 (&source->descriptor->ref_count);
	target->descriptor = source->descriptor;
}

struct Xamarin_block_descriptor xamarin_block_descriptor = 
{
	0,
	sizeof (struct Block_literal),
	xamarin_copy_helper,
	xamarin_dispose_helper,
	NULL,
	0,
};

struct Xamarin_block_descriptor *
xamarin_get_block_descriptor ()
{
	// COOP: no managed memory access: any mode.
	return &xamarin_block_descriptor;
}
