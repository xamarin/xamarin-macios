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

/*
 * Hand-written bindings to support ObjectiveC exceptions.
 * Reference:
 * 		https://bugzilla.xamarin.com/show_bug.cgi?id=7696
 *		https://bugzilla.xamarin.com/show_bug.cgi?id=11900
 */

float
xamarin_float_objc_msgSend (id self, SEL sel)
{
	@try {
#if	defined(__i386__)
		return ((float_send) objc_msgSend_fpret) (self, sel);
#else
		return ((float_send) objc_msgSend) (self, sel);
#endif
	} @catch (NSException *e) {
		xamarin_throw_ns_exception (e);
	}
}

float
xamarin_float_objc_msgSendSuper (struct objc_super *super, SEL sel)
{
	@try {
		// there is no objc_msgSendSuper_fpret: http://lists.apple.com/archives/objc-language/2006/Jun/msg00012.html
		return ((float_sendsuper) objc_msgSendSuper) (super, sel);
	} @catch (NSException *e) {
		xamarin_throw_ns_exception (e);
	}
}

xm_nfloat_t
xamarin_nfloat_objc_msgSend (id self, SEL sel)
{
	@try {
#if defined(__i386__)
		return ((nfloat_send) objc_msgSend_fpret) (self, sel);
#else
		return ((nfloat_send) objc_msgSend) (self, sel);
#endif
	} @catch (NSException *e) {
		xamarin_throw_ns_exception (e);
	}
}

xm_nfloat_t
xamarin_nfloat_objc_msgSendSuper (struct objc_super *super, SEL sel)
{
	@try {
		// there is no objc_msgSendSuper_fpret: http://lists.apple.com/archives/objc-language/2006/Jun/msg00012.html
		return ((nfloat_sendsuper) objc_msgSendSuper) (super, sel);
	} @catch (NSException *e) {
		xamarin_throw_ns_exception (e);
	}
}

/*
 * Compatibility wrappers.
 */

#ifndef XAMCORE_2_0
#ifdef MONOMAC
void *monomac_IntPtr_objc_msgSend_IntPtr (id self, SEL sel, void *a) { return xamarin_IntPtr_objc_msgSend_IntPtr (self, sel, a); }
void *monomac_IntPtr_objc_msgSendSuper_IntPtr (struct objc_super *super, SEL sel, void *a) { return xamarin_IntPtr_objc_msgSendSuper_IntPtr (super, sel, a); }
float monomac_float_objc_msgSend (id self, SEL sel) { return xamarin_float_objc_msgSend (self, sel); }
float monomac_float_objc_msgSendSuper (struct objc_super *super, SEL sel) { return xamarin_float_objc_msgSendSuper (super, sel); }
xm_nfloat_t monomac_nfloat_objc_msgSend (id self, SEL sel) { return xamarin_nfloat_objc_msgSend (self, sel); }
xm_nfloat_t monomac_nfloat_objc_msgSendSuper (struct objc_super *super, SEL sel) { return xamarin_nfloat_objc_msgSendSuper (super, sel); }
#endif
#ifdef MONOTOUCH
void * monotouch_IntPtr_objc_msgSend_IntPtr (id self, SEL sel, void *a) { return xamarin_IntPtr_objc_msgSend_IntPtr (self, sel, a); }
void * monotouch_IntPtr_objc_msgSendSuper_IntPtr (struct objc_super *super, SEL sel, void *a) { return xamarin_IntPtr_objc_msgSendSuper_IntPtr (super, sel, a); }
#endif
#endif

