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
		objc_retain ((id) targ);
	selector = sel;
	argument = objc_retain (arg);
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
		objc_release ((id) targ);
		objc_release ((id) arg);
	});
	[super dealloc];
}
@end

id
xamarin_init_nsthread (id obj, bool is_direct, id target, SEL sel, id arg)
{
	// COOP: no managed memory access: any mode.
	XamarinNSThreadObject *wrap = [[XamarinNSThreadObject alloc] initWithData: target selector: sel argument: arg is_direct_binding: is_direct];
	objc_autorelease (wrap);
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
	objc_autorelease ([[XamarinCocoaThreadInitializer alloc] initWithFunc: func]);
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
	xamarin_gchandle_free (bl->global_handle);
	bl->global_handle = INVALID_GCHANDLE;
	if (atomic_fetch_sub (&bl->descriptor->ref_count, 1) == 0) {
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
	MonoObject *mobj = xamarin_gchandle_get_target (source->local_handle);
	target->global_handle = xamarin_gchandle_new (mobj, FALSE);
	xamarin_mono_object_release (&mobj);

	atomic_fetch_add (&source->descriptor->ref_count, 1);
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
