; The initial code for this file was generated using clang:
;
;    clang -c testfile -otestfile.s -arch arm64 -S -O -fverbose-asm
;
; Relevant and helpful documentation:
; * https://github.com/ARM-software/abi-aa/blob/main/aapcs64/aapcs64.rst
; * https://github.com/ARM-software/abi-aa/blob/main/cppabi64/cppabi64.rst
;

	.section	__TEXT,__text,regular,pure_instructions
	.globl	OBJCMSGFUNC       ; -- Begin function xamarin_dyn_objc_msgSend[Super]
	.p2align	2
OBJCMSGFUNC:              ; @xamarin_dyn_objc_msgSend[Super]
Lfunc_begin2:
	.cfi_startproc
	.cfi_personality 155, ___objc_personality_v0
	.cfi_lsda 16, Lexception0

	stp	x20, x19, [sp, #-32]!           ; 16-byte Folded Spill
	stp	x29, x30, [sp, #16]             ; 16-byte Folded Spill
	add	x29, sp, #16
	.cfi_def_cfa w29, 16
	.cfi_offset w30, -8
	.cfi_offset w29, -16
	.cfi_offset w19, -24
	.cfi_offset w20, -32
Lbeforeinvoke:

	; calculate the amount of stack space we need
	sub sp, sp, #400

	; we need to preserve:
	;   x0-x8, x16, x19
	;   q0-q7

	stp  x0, x1, [sp, #0x00]
	stp  x2, x3, [sp, #0x10]
	stp  x4, x5, [sp, #0x20]
	stp  x6, x7, [sp, #0x30]
	stp  x8,x16, [sp, #0x40]
	stp x21,x22, [sp, #0x50]

	stp  q0, q1, [sp, #0x60]
	stp  q2, q3, [sp, #0x80]
	stp  q4, q5, [sp, #0xa0]
	stp  q6, q7, [sp, #0xc0]

	; potentially resolve objc_super* [handle, class_handle] to the actual instance
	RESOLVESUPER

	; figure out how much stack space we need
	bl	_xamarin_get_frame_length

	; x0 holds the amount of stack space we need
	; first align stack requirement to 16 bytes
	add x0, x0, #15
	lsr x0, x0, #4
	lsl x0, x0, #4

	; save the current stack location
	mov x22, sp

	; save how much space we need
	mov x21, x0

	; then make space for the arguments
	sub sp, sp, x0

	; copy arguments from old location in the stack to new location in the stack
    ; x2 will hold the amount of bytes left to copy. This will be a multiple of 8.
    ; x1 the current src location
    ; x0 the current dst location

    mov x2, x0        ; x2 = frame_length
    add x1, x29, #16  ; x1 = address of first argument we got
    mov x0, sp        ; x0 = address of the bottom of the stack

L_start:
    cmp  x2, #0                ;
    b.eq L_end                 ; while (left != 0) {
    sub  x2, x2, #8            ;    len -= 8                 x2 -= 8
    ldr  x3, [x1, x2]          ;    tmp = src [len]          x3 = x1 [x2]
    str  x3, [x0, x2]          ;    dst [len] = tmp          x0 [x2] = x3
    b    L_start               ; }
L_end:

	; restore original input registers, except x21 and x22, which we're still using
	ldp  x0, x1, [x22, #0x00]
	ldp  x2, x3, [x22, #0x10]
	ldp  x4, x5, [x22, #0x20]
	ldp  x6, x7, [x22, #0x30]
	ldp  x8,x16, [x22, #0x40]

	ldp  q0, q1, [x22, #0x60]
	ldp  q2, q3, [x22, #0x80]
	ldp  q4, q5, [x22, #0xa0]
	ldp  q6, q7, [x22, #0xc0]

	bl	OBJCMSGCALL

Lafterinvoke:
	; restore the stack to it's previous value
	add sp, sp, x21

	; now restore x21 and x22
	ldp x21, x22, [sp, #0x50]

	add sp, sp, #400

	ldp	x29, x30, [sp, #16]             ; 16-byte Folded Reload
	ldp	x20, x19, [sp], #32             ; 16-byte Folded Reload
	ret
Lcatchhandler:
	mov	x19, x0
	cmp	w1, #1                          ; =1
	b.ne	Lnomatchexception

	; check if xamarin_marshal_objectivec_exception_mode == disable, if so, just don't handle the exception
Lloh0:
	adrp	x8, _xamarin_marshal_objectivec_exception_mode@GOTPAGE
Lloh1:
	ldr	x8, [x8, _xamarin_marshal_objectivec_exception_mode@GOTPAGEOFF]
Lloh2:
	ldr	w0, [x8]
	cmp	w0, #4
	b.eq	Lnomatchexception

	mov	x0, x19
	bl	_objc_begin_catch
Lcatchbegin:
	mov	w1, #0
	mov	x2, #0
	bl	_xamarin_process_nsexception_using_mode
Lcatchend:
	bl	_objc_end_catch
	; xamarin_process_nsexception_using_mode might have set a pending managed exception, which means we end up executing more code here
	; in theory, any pending managed exceptions should be thrown automatically upon returning to managed code.
	; however, things may go wrong, and if the pending managed exception isn't thrown, then make sure we don't return some random value.
	; returning a constant NULL/0 is at least consistent behavior, and is less likely to crash the process than any other value.
	mov x0, #0
	b	Lafterinvoke
Lcatchcatchhandler:
	mov	x19, x0
	bl	_objc_end_catch
Lnomatchexception:
	mov	x0, x19
	bl	__Unwind_Resume
	brk	#0x1
Lfunc_end2:
	.loh AdrpLdrGotLdr	Lloh0, Lloh1, Lloh2
	.cfi_endproc



	.section	__TEXT,__gcc_except_tab
	.p2align	2
GCC_except_table0:
Lexception0:
	.byte	255                             ; @LPStart Encoding = omit
	.byte	155                             ; @TType Encoding = indirect pcrel sdata4
	.uleb128 Lttbase0-Lttbaseref0
Lttbaseref0:
	.byte	1                               ; Call site Encoding = uleb128
	.uleb128 Lcst_end0-Lcst_begin0
Lcst_begin0:
	.uleb128 Lbeforeinvoke-Lfunc_begin2             ; >> Call Site 1 <<
	.uleb128 Lafterinvoke-Lbeforeinvoke                    ;   Call between Lbeforeinvoke and Lafterinvoke
	.uleb128 Lcatchhandler-Lfunc_begin2             ;     jumps to Lcatchhandler
	.byte	3                               ;   On action: 2
	.uleb128 Lafterinvoke-Lfunc_begin2             ; >> Call Site 2 <<
	.uleb128 Lcatchbegin-Lafterinvoke                    ;   Call between Lafterinvoke and Lcatchbegin
	.byte	0                               ;     has no landing pad
	.byte	0                               ;   On action: cleanup
	.uleb128 Lcatchbegin-Lfunc_begin2             ; >> Call Site 3 <<
	.uleb128 Lcatchend-Lcatchbegin                    ;   Call between Lcatchbegin and Lcatchend
	.uleb128 Lcatchcatchhandler-Lfunc_begin2             ;     jumps to Lcatchcatchhandler
	.byte	0                               ;   On action: cleanup
	.uleb128 Lcatchend-Lfunc_begin2             ; >> Call Site 4 <<
	.uleb128 Lfunc_end2-Lcatchend               ;   Call between Lcatchend and Lfunc_end2
	.byte	0                               ;     has no landing pad
	.byte	0                               ;   On action: cleanup
Lcst_end0:
	.byte	0                               ; >> Action Record 1 <<
                                        ;   Cleanup
	.byte	0                               ;   No further actions
	.byte	1                               ; >> Action Record 2 <<
                                        ;   Catch TypeInfo 1
	.byte	125                             ;   Continue to action 1
	.p2align	2
                                        ; >> Catch TypeInfos <<
Ltmp8:                                  ; TypeInfo 1
	.long	_OBJC_EHTYPE_$_NSException@GOT-Ltmp8
Lttbase0:
	.p2align	2
                                        ; -- End function


	.section	__DATA,__objc_imageinfo,regular,no_dead_strip
L_OBJC_IMAGE_INFO:
	.long	0
	.long	64

.subsections_via_symbols
