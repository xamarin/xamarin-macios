
	call _xamarin_get_frame_length

	# r12 holds the amount of stack space we need
	movq	%rax, %r12
	# first align stack requirement to 16 bytes
	addq    $15,%r12
	shrq    $4,%r12
	salq    $4,%r12
	# then make space for the arguments
	subq	%r12, %rsp

	# copy arguments from old location in the stack to new location in the stack
    # %rcx will hold the amount of bytes left to copy
    # %rsi the current src location
    # %rdi the current dst location

    movq	%r12, %rcx     # rcx = frame_length
    leaq    16(%rbp),%rsi  # rsi = address of first argument we got
    movq    %rsp,%rdi      # rdi = address of the bottom of the stack

L_start:
    cmpq $0,%rcx              # 
    je L_end                  # while (left != 0) {
    subq $8,%rcx              #    len -= 8
    movq (%rsi,%rcx),%rax     #    tmp = src [len]
    movq %rax,(%rdi,%rcx)     #    dst [len] = tmp
    jmp L_start               # }
L_end:

	movq	-16(%rbp), %rdi
	movq	-24(%rbp), %rsi
	movq	-32(%rbp), %rdx
	movq	-40(%rbp), %rcx
	movq	-48(%rbp), %r8
	movq	-56(%rbp), %r9
	movq	-64(%rbp), %rax
	/* skip 8 bytes so that this is 16-byte aligned */
	movaps	-80(%rbp), %xmm0
	movaps	-96(%rbp), %xmm1
	movaps	-112(%rbp), %xmm2
	movaps	-128(%rbp), %xmm3
	movaps	-144(%rbp), %xmm4
	movaps	-160(%rbp), %xmm5
	movaps	-176(%rbp), %xmm6
	movaps	-192(%rbp), %xmm7
