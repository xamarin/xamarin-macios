#ifndef __TRAMPOLINES_X86_64_H__
#define __TRAMPOLINES_X86_64_H__

#ifdef __cplusplus
extern "C" {
#endif

struct XamarinCallState {
	uint64_t type;
	uint64_t rdi;                        // 1st argument
	uint64_t rsi; // 2nd argument
	uint64_t rdx; // 3rd argument
	uint64_t rcx; // 4th argument
	uint64_t r8; // 5th argument
	uint64_t r9; // 6th argument
	uint64_t rbp;
	uint64_t rax;
	uint64_t rdx_out; // use this field as a temporary rdx field to store output. It makes the marshalling code a bit easier to have this field just after the rax field (so we can treat rax+rdx as a continuous block of 32 bytes of memory)
	long double xmm0;
	long double xmm1;
	long double xmm2;
	long double xmm3;
	long double xmm4;
	long double xmm5;
	long double xmm6;
	long double xmm7;

	bool is_stret () { return (type & Tramp_Stret) == Tramp_Stret; }
	id self ()  { return is_stret () ? (id) rsi : (id) rdi; }
	SEL sel () { return is_stret () ? (SEL) rdx : (SEL) rsi; }
};

struct ParamIterator {
	struct XamarinCallState *state;
	int byte_count;
	int float_count;
	uint8_t *stack_next;
};

void xamarin_arch_trampoline (struct XamarinCallState *state);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __TRAMPOLINES_X86_64_H__ */

