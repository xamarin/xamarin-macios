/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 *  Authors: Rolf Bjarne Kvinge
 *
 *  Copyright (C) 2014 Xamarin Inc. (www.xamarin.com)
 *
 */

#ifndef __TRAMPOLINES_H__
#define __TRAMPOLINES_H__

#ifdef __cplusplus
extern "C" {
#endif

void *		xamarin_trampoline (id self, SEL sel, ...);
void		xamarin_stret_trampoline (void *buffer, id self, SEL sel, ...);
float		xamarin_fpret_single_trampoline (id self, SEL sel, ...);
double		xamarin_fpret_double_trampoline (id self, SEL sel, ...);
void		xamarin_release_trampoline (id self, SEL sel);
void		xamarin_calayer_release_trampoline (id self, SEL sel);
id			xamarin_retain_trampoline (id self, SEL sel);
void		xamarin_dealloc_trampoline (id self, SEL sel);
void *		xamarin_static_trampoline (id self, SEL sel, ...);
void *		xamarin_ctor_trampoline (id self, SEL sel, ...);
void		xamarin_x86_double_abi_stret_trampoline ();
float		xamarin_static_fpret_single_trampoline (id self, SEL sel, ...);
double		xamarin_static_fpret_double_trampoline (id self, SEL sel, ...);
void		xamarin_static_stret_trampoline (void *buffer, id self, SEL sel, ...);
void		xamarin_static_x86_double_abi_stret_trampoline ();
long long	xamarin_longret_trampoline (id self, SEL sel, ...);
long long	xamarin_static_longret_trampoline (id self, SEL sel, ...);
id			xamarin_copyWithZone_trampoline1 (id self, SEL sel, NSZone *zone);
id			xamarin_copyWithZone_trampoline2 (id self, SEL sel, NSZone *zone);
int			xamarin_get_gchandle_trampoline (id self, SEL sel);
void		xamarin_set_gchandle_trampoline (id self, SEL sel, int gc_handle);

int			xamarin_get_frame_length (id self, SEL sel);

enum ArgumentSemantic /* Xcode 4.4 doesn't like this ': int' */ {
	ArgumentSemanticNone   = -1,
	ArgumentSemanticAssign = 0,
	ArgumentSemanticCopy   = 1,
	ArgumentSemanticRetain = 2,
	ArgumentSemanticMask = ArgumentSemanticAssign | ArgumentSemanticCopy | ArgumentSemanticRetain,
	ArgumentSemanticRetainReturnValue = 1 << 10,
	ArgumentSemanticCategoryInstance = 1 << 11,
};

/* Copied from SGen */

static inline void
mt_dummy_use (void *v) {
#if defined(__GNUC__)
	__asm__ volatile ("" : "=r"(v) : "r"(v));
#elif defined(_MSC_VER)
	__asm {
		mov eax, v;
		and eax, eax;
	};
#else
#error "Implement mt_dummy_use for your compiler"
#endif
}

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __TRAMPOLINES_H__ */
