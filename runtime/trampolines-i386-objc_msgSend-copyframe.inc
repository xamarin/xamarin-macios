
	#
	# input stack layout:
	# %esp+20: ...
	# %esp+16: second arg
	# %esp+12: first arg
	# %esp+8:  sel
	# %esp+4:  this
	# %esp:    buffer
	# and %ebp+8 = %esp
	#
	# We extend the stack (big enough for all the arguments again),
	# and copy all the arguments as-is there, before
	# calling objc_msgSend with those copied arguments.
	# The only difference is that this method has an exception handler.
	#

	call	_xamarin_get_frame_length # get_frame_length (this, sel)
	
	# use eax to extend the stack, but it needs to be aligned to 16 bytes first
	addl	$15,%eax
	shrl	$4,%eax
	sall	$4,%eax
	subl	%eax,%esp
	# store the number somewhere so we can restore the stack pointer later
	movl	%eax,-16(%ebp)
	
	# copy arguments from old location in the stack to new location in the stack
	# %ecx will hold the amount of bytes left to copy
	# %esi the current src location
	# %edi the current dst location
	
	# %ecx will already be a multiple of 4, since the abi requires it
	# (arguments smaller than 4 bytes are extended to 4 bytes according to
	# http://developer.apple.com/library/mac/#documentation/DeveloperTools/Conceptual/LowLevelABI/130-IA-32_Function_Calling_Conventions/IA32.html#//apple_ref/doc/uid/TP40002492-SW4)
	
	movl	-16(%ebp),%ecx # ecx = frame_length
	leal	8(%ebp),%esi   # esi = address of first argument we got (buffer)
	movl	%esp,%edi      # edi = address of the bottom of the stack

L_start:
	cmpl $0,%ecx              # 
	je L_end                  # while (left != 0) {
	subl $4,%ecx              #    len -= 4
	movl (%esi,%ecx),%eax     #    tmp = src [len]
	movl %eax,(%edi,%ecx)     #    dst [len] = tmp
	jmp L_start               # }
L_end:

	movaps	-40(%ebp), %xmm0
	movaps	-56(%ebp), %xmm1
	movaps	-72(%ebp), %xmm2
	movaps	-88(%ebp), %xmm3
