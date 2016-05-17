#if __i386__ && MONOTOUCH

.subsections_via_symbols
.text

.align	4
.globl	_xamarin_dyn_objc_msgSendSuper
_xamarin_dyn_objc_msgSendSuper:

#include "trampolines-i386-objc_msgSend-pre.inc"

	# int frame_length = get_frame_length (this, sel)
	movl	12(%ebp), %eax
	movl	%eax, 4(%esp)   # push sel
	movl	8(%ebp), %eax
	movl	(%eax), %eax
	movl	%eax, (%esp)    # push this

#include "trampolines-i386-objc_msgSend-copyframe.inc"

Lbeforeinvoke:
	calll	_objc_msgSendSuper

#include "trampolines-i386-objc_msgSend-post.inc"

#endif
