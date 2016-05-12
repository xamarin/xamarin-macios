
# function start
# preserve some registers
# create stack space to call get_frame_length

Lfunc_begin0:
.cfi_startproc
.cfi_personality 155, L___objc_personality_v0$non_lazy_ptr
.cfi_lsda 16, Lexception0

	pushl	%ebp
.cfi_def_cfa_offset 8
.cfi_offset %ebp, -8
	movl	%esp, %ebp
.cfi_def_cfa_register %ebp

	pushl	%esi		# %ebp-4
	pushl	%edi		# %ebp-8
	pushl	%ebx		# %ebp-12
	# we need 64 bytes to store xmm0-3, and those 64 bytes need to be 16-byte aligned.
	# we need 4 bytes to store the result for get_frame_length, which we store in %ebp-16
	# then 4 more bytes for stack space for the call to get_frame_length (which takes 2 arguments)
	subl	$108, %esp # to store xmm0-3
	movaps	%xmm0, -40(%ebp)
	movaps	%xmm1, -56(%ebp)
	movaps	%xmm2, -72(%ebp)
	movaps	%xmm3, -88(%ebp)
