#if __x86_64__

.subsections_via_symbols
.text

.align	4
.globl	_xamarin_dyn_objc_msgSendSuper
_xamarin_dyn_objc_msgSendSuper:

#include "trampolines-x86_64-objc_msgSend-pre.inc"

	# resolve objc_super* [handle, class_handle] to the actual instance
	movq	(%rdi), %rdi

#include "trampolines-x86_64-objc_msgSend-copyframe.inc"

Lbeforeinvoke:
call	_objc_msgSendSuper

#include "trampolines-x86_64-objc_msgSend-post.inc"

#endif
