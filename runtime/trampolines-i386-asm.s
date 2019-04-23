#
# https://developer.apple.com/library/mac/documentation/DeveloperTools/Conceptual/LowLevelABI/130-IA-32_Function_Calling_Conventions/IA32.html#//apple_ref/doc/uid/TP40002492-SW4
#
# store all parameters in a consistent way, and send it off to managed code.
# we need to store:
#   %esp
#
# our struct is:
#      4 bytes for the type of trampoline (we use %edx temporarily for this purpose)
#   3x 4 bytes for the registers
#      8 bytes for the floating point return value
# total: 24 bytes
#
# upon return we may need to write to:
#   %eax, %edx
#   %st0 (the floating point stack)
#

#if __i386__

.subsections_via_symbols
.text

_xamarin_i386_common_trampoline:
.cfi_startproc
	pushl	%ebp
.cfi_def_cfa_offset 8
.cfi_offset %ebp, -8
	movl	%esp, %ebp
.cfi_def_cfa_register %ebp

	pushl	%esi # we use %esi as a pointer to our XamarinCallState struct. It's a preserved register, so we need to save it.

	subl	$0x24,	%esp	# allocate 32 bytes from the stack. 24 bytes for our XamarinCallState struct + 4 bytes for the parameters to xamarin_arch_trampoline + alignment.

	# %esi points our XamarinCallState structure (on the stack)
	movl	%esp,	%esi
	addl	$0x4,	%esi

	movl	%edx,	  (%esi) # type
	movl	%eax,	 4(%esi) # eax
	movl	%edx,	 8(%esi) # edx
	movl	%ebp,	   %ecx
	addl	$0x4,	   %ecx
	movl	%ecx,	12(%esi) # esp (at entry to this function)

	movl	%esi,	  (%esp) # 1st argument to xamarin_arch_trampoline, a pointer to the XamarinCallState structure.
	call	_xamarin_arch_trampoline

	# get return value(s)

	movl	4(%esi),	%eax
	movl	8(%esi),	%edx

	# check if we need to push a floating point value to the floating point stack
	# use %ecx as scratch register (not preserved, and not used to return values)
	movl	(%esi), %ecx # fetch the trampoline type

	cmpl	$0x4, %ecx           # if (type == _xamarin_fpret_single_trampoline)
	je L_single_floating_point   #     goto single floating point;
	cmpl	$0x5, %ecx           # else if (type == _xamarin_static_fpret_single_trampoline)
	je L_single_floating_point   #     goto single floating point;
	cmpl	$0x8, %ecx           # else if (type == _xamarin_fpret_double_trampoline)
	je L_double_floating_point   #     goto doublefloating point;
	cmpl	$0x9, %ecx           # else if (type == _xamarin_static_fpret_double_trampoline)
	je L_double_floating_point   #     goto double floating point;

	jmp     L_no_floating_point

L_single_floating_point:
	flds	16(%esi)
	jmp L_no_floating_point

L_double_floating_point:
	fldl	16(%esi)
	# fall through

L_no_floating_point:

	addl	$0x24, %esp	# deallocate the stack space we used

	popl	%esi
	popl	%ebp

	cmpl $0x50, %ecx
	je L_double_stret_return
	cmpl $0x51, %ecx
	je L_double_stret_return

	retl

L_double_stret_return:
	retl $0x4

.cfi_endproc

#
# trampolines
#

.globl _xamarin_trampoline
_xamarin_trampoline:
	movl	$0x0, %edx
	jmp	_xamarin_i386_common_trampoline
	
.globl _xamarin_static_trampoline
_xamarin_static_trampoline:
	movl	$0x1, %edx
	jmp	_xamarin_i386_common_trampoline

.globl _xamarin_ctor_trampoline
_xamarin_ctor_trampoline:
	movl	$0x2, %edx
	jmp _xamarin_i386_common_trampoline

.globl _xamarin_fpret_single_trampoline
_xamarin_fpret_single_trampoline:
	movl	$0x4, %edx
	jmp _xamarin_i386_common_trampoline

.globl _xamarin_static_fpret_single_trampoline
_xamarin_static_fpret_single_trampoline:
	movl	$0x5, %edx
	jmp _xamarin_i386_common_trampoline

.globl _xamarin_fpret_double_trampoline
_xamarin_fpret_double_trampoline:
	movl	$0x8, %edx
	jmp _xamarin_i386_common_trampoline
	
.globl _xamarin_static_fpret_double_trampoline
_xamarin_static_fpret_double_trampoline:
	movl	$0x9, %edx
	jmp _xamarin_i386_common_trampoline

.globl _xamarin_stret_trampoline
_xamarin_stret_trampoline:
	movl	$0x10, %edx
	jmp _xamarin_i386_common_trampoline

.globl _xamarin_static_stret_trampoline
_xamarin_static_stret_trampoline:
	movl	$0x11, %edx
	jmp _xamarin_i386_common_trampoline

.globl _xamarin_longret_trampoline
_xamarin_longret_trampoline:
	movl	$0x20, %edx
	jmp _xamarin_i386_common_trampoline

.globl _xamarin_static_longret_trampoline
_xamarin_static_longret_trampoline:
	movl	$0x21, %edx
	jmp _xamarin_i386_common_trampoline

.globl _xamarin_x86_double_abi_stret_trampoline
_xamarin_x86_double_abi_stret_trampoline:
	movl	$0x50, %edx
	jmp _xamarin_i386_common_trampoline

.globl _xamarin_static_x86_double_abi_stret_trampoline
_xamarin_static_x86_double_abi_stret_trampoline:
	movl	$0x51, %edx
	jmp _xamarin_i386_common_trampoline

# etc...

#endif
