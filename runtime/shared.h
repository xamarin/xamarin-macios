//
// shared.h: Shared native code between Xamarin.iOS and Xamarin.Mac.
// 
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. 
//

#ifndef __SHARED_H__
#define __SHARED_H__

#include <stdbool.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void (init_cocoa_func) (void);
void initialize_cocoa_threads (init_cocoa_func *func);

void install_nsautoreleasepool_hooks ();
id xamarin_init_nsthread (id obj, bool is_direct, id target, SEL sel, id arg);

/*
 * Block support
 */

struct Xamarin_block_descriptor {
	unsigned long int reserved; // NULL
	unsigned long int size;     // sizeof(struct Block_literal_1)
    // optional helper functions
    void (*copy_helper) (void *dst, void *src);    // IFF (1<<25)
    void (*dispose_helper)(void *src);             // IFF (1<<25)
    // required ABI.2010.3.16
    const char *signature;                         // IFF (1<<30)
    int xamarin_size;
    // variable-length string
};

struct Block_literal {
	void *isa;
	int flags;
	int reserved;
	void (*invoke)(void *, ...);
	struct Xamarin_block_descriptor *descriptor;
	void *local_handle;
	void *global_handle;
};

struct Xamarin_block_descriptor *  xamarin_get_block_descriptor ();


#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __SHARED_H__ */
