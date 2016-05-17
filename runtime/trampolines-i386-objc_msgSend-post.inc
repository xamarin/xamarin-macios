Lafterinvoke:
	addl	-16(%ebp), %esp
	addl	$108, %esp
	popl	%ebx
	popl	%edi
	popl	%esi
	popl	%ebp
	retl
Lcatchhandler:
	cmpl	$1, %edx
	movl	%eax, (%esp)
	jne	Lnomatchexception

	# check if xamarin_marshal_objectivec_exception_mode == disable, if so, just don't handle the exception
	call	Lloadpcrelative1
Lloadpcrelative1:
	popl	%ecx
	cmpl	$4, L_xamarin_marshal_objectivec_exception_mode$non_lazy_ptr-Lloadpcrelative1(%ecx)
	je	Lnomatchexception

	calll	_objc_begin_catch
Lcatchbegin:
	movl	%eax, (%esp)
	calll	_xamarin_process_nsexception
Lcatchend:
	calll	_objc_end_catch
	jmp	Lafterinvoke
Lcatchcatchhandler:
	calll	_objc_end_catch
Lnomatchexception:
	calll	__Unwind_Resume
Lfunc_end0:
.cfi_endproc

# exception table
	.section	__TEXT,__gcc_except_tab
	.align	2
GCC_except_table0:
Lexception0:
	.byte	255                     ## @LPStart Encoding = omit
	.byte	155                     ## @TType Encoding = indirect pcrel sdata4
	.asciz	"\274"                  ## @TType base offset
	.byte	3                       ## Call site Encoding = udata4
	.byte	52                      ## Call site table length
Lset0 = Lbeforeinvoke-Lfunc_begin0              ## >> Call Site 1 <<
	.long	Lset0
Lset1 = Lafterinvoke-Lbeforeinvoke                     ##   Call between Lbeforeinvoke and Lafterinvoke
	.long	Lset1
Lset2 = Lcatchhandler-Lfunc_begin0              ##     jumps to Lcatchhandler
	.long	Lset2
	.byte	1                       ##   On action: 1
Lset3 = Lafterinvoke-Lfunc_begin0              ## >> Call Site 2 <<
	.long	Lset3
Lset4 = Lcatchbegin-Lafterinvoke                     ##   Call between Lafterinvoke and Lcatchbegin
	.long	Lset4
	.long	0                       ##     has no landing pad
	.byte	0                       ##   On action: cleanup
Lset5 = Lcatchbegin-Lfunc_begin0              ## >> Call Site 3 <<
	.long	Lset5
Lset6 = Lcatchend-Lcatchbegin                     ##   Call between Lcatchbegin and Lcatchend
	.long	Lset6
Lset7 = Lcatchcatchhandler-Lfunc_begin0              ##     jumps to Lcatchcatchhandler
	.long	Lset7
	.byte	0                       ##   On action: cleanup
Lset8 = Lcatchend-Lfunc_begin0              ## >> Call Site 4 <<
	.long	Lset8
Lset9 = Lfunc_end0-Lcatchend                ##   Call between Lcatchend and Lfunc_end0
	.long	Lset9
	.long	0                       ##     has no landing pad
	.byte	0                       ##   On action: cleanup
	.byte	1                       ## >> Action Record 1 <<
                                        ##   Catch TypeInfo 1
	.byte	0                       ##   No further actions
                                        ## >> Catch TypeInfos <<
Ltmp9:                                  ## TypeInfo 1
	.long	L_OBJC_EHTYPE_$_NSException$non_lazy_ptr-Ltmp9
	.align	2

	.section	__DATA,__objc_imageinfo,regular,no_dead_strip
L_OBJC_IMAGE_INFO:
	.long	0
	.long	0

	.section	__IMPORT,__pointers,non_lazy_symbol_pointers
L_OBJC_EHTYPE_$_NSException$non_lazy_ptr:
	.indirect_symbol	_OBJC_EHTYPE_$_NSException
	.long	0
L___objc_personality_v0$non_lazy_ptr:
	.indirect_symbol	___objc_personality_v0
	.long	0
L_xamarin_marshal_objectivec_exception_mode$non_lazy_ptr:
	.indirect_symbol	_xamarin_marshal_objectivec_exception_mode
	.long	0
