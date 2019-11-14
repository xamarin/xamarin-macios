#ifndef __TRAMPOLINES_VARARGS_H__
#define __TRAMPOLINES_VARARGS_H__

#include <stdarg.h>

#ifdef __cplusplus
extern "C" {
#endif

struct XamarinCallState {
	enum TrampolineType type; // the type of trampoline
	id self;
	SEL sel;
	void *buffer;
	va_list ap;
	union {  // floating point return value
		double double_ret;
		float float_ret;
		long long longlong_ret;
		void *ptr_ret;
	};
};

struct ParamIterator {
	struct XamarinCallState *state;
	va_list ap;
};

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __TRAMPOLINES_VARARGS_H__ */

