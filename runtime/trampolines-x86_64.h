#ifndef __TRAMPOLINES_X86_64_H__
#define __TRAMPOLINES_X86_64_H__

#ifdef __cplusplus
extern "C" {
#endif

struct CallState {
	uint64_t type;
	uint64_t rdi;                        // 1st argument
	union {
		uint64_t rsi; // rsi upon entry. // 2nd argument
		uint64_t rax; // rax upon exit.
	};
	uint64_t rdx; // 3rd argument
	uint64_t rcx; // 4th argument
	uint64_t r8; // 5th argument
	uint64_t r9; // 6th argument
	uint64_t rbp;
	long double xmm0;
	long double xmm1;
	long double xmm2;
	long double xmm3;
	long double xmm4;
	long double xmm5;
	long double xmm6;
	long double xmm7;
};

struct ParamIterator {
	struct CallState *state;
	int byte_count;
	int float_count;
	uint8_t *stack_next;
};

void xamarin_arch_trampoline (struct CallState *state);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __TRAMPOLINES_X86_64_H__ */

