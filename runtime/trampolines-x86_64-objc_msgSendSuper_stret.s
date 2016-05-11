#if __x86_64__

.subsections_via_symbols
.text

.align	4
.globl	_xamarin_dyn_objc_msgSendSuper_stret
_xamarin_dyn_objc_msgSendSuper_stret:

#include "trampolines-x86_64-objc_msgSend-pre.inc"

	# put (id, SEL) where xamarin_get_frame_length expects it
	movq	(%rsi), %rdi
	movq	%rdx, %rsi

#include "trampolines-x86_64-objc_msgSend-copyframe.inc"

Lbeforeinvoke:
call	_objc_msgSendSuper_stret

#include "trampolines-x86_64-objc_msgSend-post.inc"

#endif
