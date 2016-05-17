#if __x86_64__

.subsections_via_symbols
.text

.align	4
.globl	_xamarin_dyn_objc_msgSend
_xamarin_dyn_objc_msgSend:

#include "trampolines-x86_64-objc_msgSend-pre.inc"

#include "trampolines-x86_64-objc_msgSend-copyframe.inc"

Lbeforeinvoke:
call	_objc_msgSend

#include "trampolines-x86_64-objc_msgSend-post.inc"

#endif
