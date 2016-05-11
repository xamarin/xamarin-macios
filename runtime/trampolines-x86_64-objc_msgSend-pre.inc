
# function start
# preserve some registers

Lfunc_begin2:
.cfi_startproc
.cfi_personality 155, ___objc_personality_v0
.cfi_lsda 16, Lexception2

	pushq	%rbp
.cfi_def_cfa_offset 16
.cfi_offset %rbp, -16
	movq	%rsp, %rbp
.cfi_def_cfa_register %rbp

	# calculate the amount of stack space we need
	subq    $208,%rsp
	movq	%r12,  -8(%rbp)
	movq	%rdi, -16(%rbp)
	movq	%rsi, -24(%rbp)
	movq	%rdx, -32(%rbp)
	movq	%rcx, -40(%rbp)
	movq	%r8,  -48(%rbp)
	movq	%r9,  -56(%rbp)
	movq	%rax, -64(%rbp)
	/* skip 8 bytes so that this is 16-byte aligned */
	movaps	%xmm0, -80(%rbp)
	movaps	%xmm1, -96(%rbp)
	movaps	%xmm2, -112(%rbp)
	movaps	%xmm3, -128(%rbp)
	movaps	%xmm4, -144(%rbp)
	movaps	%xmm5, -160(%rbp)
	movaps	%xmm6, -176(%rbp)
	movaps	%xmm7, -192(%rbp)
