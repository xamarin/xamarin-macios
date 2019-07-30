#
# store all parameters in a consistent way, and send it off to managed code.
# we need to store:
#   %rdi, %rsi, %rdx, %rcx, %r8, %r9
#   %xmm0-%xmm7
#   an unknown amount of stack space, but we can pass a pointer to the start of this area.
# in total we need 7*64bits registers + 8*128bits registers + 1*64bit ptr = 192 bytes.
# additionally we'll use %r11 to specify the type of trampoline was called, so 200 bytes.
# and finally we need to align the long double fields on a 16-byte boundary, so 208 bytes.
#
#
# upon return we may need to write to:
#   %rax, %rdx 
#   %xmm0-%xmm1
#   (the spec says %st0 and %st1 are used to return 'long double' arguments, but we won't encounter those)
#

#if __x86_64__

.subsections_via_symbols
.text

_xamarin_x86_64_common_trampoline:
.cfi_startproc
	pushq	%rbp
.cfi_def_cfa_offset 16
.cfi_offset %rbp, -16
	movq	%rsp, %rbp
.cfi_def_cfa_register %rbp
	subq	$0xD0, %rsp	# allocate 208 bytes from the stack
	# todo: verify alignment.
	movq	%r11,   (%rsp)
	movq	%rdi,  8(%rsp)
	movq	%rsi, 16(%rsp)
	movq	%rdx, 24(%rsp)
	movq	%rdx, 72(%rsp) # store a copy of rdx where we read the rdx return value from.
	movq	%rcx, 32(%rsp)
	movq	%r8,  40(%rsp)
	movq	%r9,  48(%rsp)
	movq	%rbp, 56(%rsp)
	movaps	%xmm0, 80(%rsp)
	movaps	%xmm1, 96(%rsp)
	movaps	%xmm2, 112(%rsp)
	movaps	%xmm3, 128(%rsp)
	movaps	%xmm4, 144(%rsp)
	movaps	%xmm5, 160(%rsp)
	movaps	%xmm6, 176(%rsp)
	movaps	%xmm7, 192(%rsp)

	movq	%rsp, %rdi
	callq	_xamarin_arch_trampoline

	# get return value(s)

	movq	64(%rsp), %rax
	movq	72(%rsp), %rdx
	movaps	80(%rsp), %xmm0
	movaps	96(%rsp), %xmm1
	
	addq	$0xD0, %rsp	# deallocate 208 bytes from the stack
	popq	%rbp
		
	ret
.cfi_endproc

#
# trampolines
#

.globl _xamarin_trampoline
_xamarin_trampoline:
	movq	$0x0, %r11
	jmp	_xamarin_x86_64_common_trampoline
	
.globl _xamarin_static_trampoline
_xamarin_static_trampoline:
	movq	$0x1, %r11
	jmp	_xamarin_x86_64_common_trampoline

.globl _xamarin_ctor_trampoline
_xamarin_ctor_trampoline:
	movq	$0x2, %r11
	jmp _xamarin_x86_64_common_trampoline

.globl _xamarin_fpret_single_trampoline
_xamarin_fpret_single_trampoline:
	movq	$0x4, %r11
	jmp _xamarin_x86_64_common_trampoline

.globl _xamarin_static_fpret_single_trampoline
_xamarin_static_fpret_single_trampoline:
	movq	$0x5, %r11
	jmp _xamarin_x86_64_common_trampoline

.globl _xamarin_fpret_double_trampoline
_xamarin_fpret_double_trampoline:
	movq	$0x8, %r11
	jmp _xamarin_x86_64_common_trampoline
	
.globl _xamarin_static_fpret_double_trampoline
_xamarin_static_fpret_double_trampoline:
	movq	$0x9, %r11
	jmp _xamarin_x86_64_common_trampoline

.globl _xamarin_stret_trampoline
_xamarin_stret_trampoline:
	movq	$0x10, %r11
	jmp _xamarin_x86_64_common_trampoline

.globl _xamarin_static_stret_trampoline
_xamarin_static_stret_trampoline:
	movq	$0x11, %r11
	jmp _xamarin_x86_64_common_trampoline

.globl _xamarin_longret_trampoline
_xamarin_longret_trampoline:
	movq	$0x20, %r11
	jmp _xamarin_x86_64_common_trampoline

.globl _xamarin_static_longret_trampoline
_xamarin_static_longret_trampoline:
	movq	$0x21, %r11
	jmp _xamarin_x86_64_common_trampoline

# etc...

#endif
