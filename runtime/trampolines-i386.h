#ifndef __TRAMPOLINES_I386_H__
#define __TRAMPOLINES_I386_H__

#ifdef __cplusplus
extern "C" {
#endif

struct CallState {
	uint32_t type; // the type of trampoline
	uint32_t eax; // undefined on entry, return value upon exit
	uint32_t edx; // undefined on entry, return value upon exit
	uint32_t esp; // pointer to the stack
	union {  // floating point return value
		double double_ret;
		float float_ret;
	};
};

struct ParamIterator {
	struct CallState *state;
	uint8_t *stack_next;
	uint8_t *stret;
};

void xamarin_arch_trampoline (struct CallState *state);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __TRAMPOLINES_I386_H__ */

