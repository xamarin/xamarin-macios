#ifndef __TRAMPOLINES_I386_H__
#define __TRAMPOLINES_I386_H__

#ifdef __cplusplus
extern "C" {
#endif

struct XamarinCallState {
	uint32_t type; // the type of trampoline
	uint32_t eax; // undefined on entry, return value upon exit
	uint32_t edx; // undefined on entry, return value upon exit
	uint32_t esp; // pointer to the stack
	union {  // floating point return value
		double double_ret;
		float float_ret;
	};
	bool is_stret () { return (type & Tramp_Stret) == Tramp_Stret; }
	id self () { return ((id *) esp) [(is_stret () ? 2 : 1)]; }
	SEL sel () { return ((SEL *) esp) [(is_stret () ? 3 : 2)]; }
};

struct ParamIterator {
	struct XamarinCallState *state;
	uint8_t *stack_next;
	uint8_t *stret;
};

void xamarin_arch_trampoline (struct XamarinCallState *state);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __TRAMPOLINES_I386_H__ */

