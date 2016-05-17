
Lafterinvoke:
	addq	$208, %rsp
	addq	%r12, %rsp
	movq	-8(%rbp), %r12
	popq	%rbp
	retq
Lcatchhandler:
	cmpl	$1, %edx
	movq	%rax, %rdi
	jne	Lnomatchexception

	# check if xamarin_marshal_objectivec_exception_mode == disable, if so, just don't handle the exception
	leal	_xamarin_marshal_objectivec_exception_mode(%rip), %edx
	cmpl	$4, %edx
	je	Lnomatchexception

	callq	_objc_begin_catch
Lcatchbegin:
	movq	%rax, %rdi
	callq	_xamarin_process_nsexception
Lcatchend:
	callq	_objc_end_catch
	jmp	Lafterinvoke
Lcatchcatchhandler:
	callq	_objc_end_catch
Lnomatchexception:
	callq	__Unwind_Resume
Lfunc_end2:
	.cfi_endproc

# exception table
	.section	__TEXT,__gcc_except_tab
	.align	2
GCC_except_table2:
Lexception2:
	.byte	255                     ## @LPStart Encoding = omit
	.byte	155                     ## @TType Encoding = indirect pcrel sdata4
	.asciz	"\274"                  ## @TType base offset
	.byte	3                       ## Call site Encoding = udata4
	.byte	52                      ## Call site table length
Lset0 = Lbeforeinvoke-Lfunc_begin2              ## >> Call Site 1 <<
	.long	Lset0
Lset1 = Lafterinvoke-Lbeforeinvoke                     ##   Call between Lbeforeinvoke and Lafterinvoke
	.long	Lset1
Lset2 = Lcatchhandler-Lfunc_begin2              ##     jumps to Lcatchhandler
	.long	Lset2
	.byte	1                       ##   On action: 1
Lset3 = Lafterinvoke-Lfunc_begin2              ## >> Call Site 2 <<
	.long	Lset3
Lset4 = Lcatchbegin-Lafterinvoke                     ##   Call between Lafterinvoke and Lcatchbegin
	.long	Lset4
	.long	0                       ##     has no landing pad
	.byte	0                       ##   On action: cleanup
Lset5 = Lcatchbegin-Lfunc_begin2              ## >> Call Site 3 <<
	.long	Lset5
Lset6 = Lcatchend-Lcatchbegin                    ##   Call between Lcatchbegin and Lcatchend
	.long	Lset6
Lset7 = Lcatchcatchhandler-Lfunc_begin2             ##     jumps to Lcatchcatchhandler
	.long	Lset7
	.byte	0                       ##   On action: cleanup
Lset8 = Lcatchend-Lfunc_begin2             ## >> Call Site 4 <<
	.long	Lset8
Lset9 = Lfunc_end2-Lcatchend               ##   Call between Lcatchend and Lfunc_end2
	.long	Lset9
	.long	0                       ##     has no landing pad
	.byte	0                       ##   On action: cleanup
	.byte	1                       ## >> Action Record 1 <<
                                        ##   Catch TypeInfo 1
	.byte	0                       ##   No further actions
                                        ## >> Catch TypeInfos <<
	.long	_OBJC_EHTYPE_$_NSException@GOTPCREL+4 ## TypeInfo 1
	.align	2
