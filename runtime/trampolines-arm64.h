#ifndef __TRAMPOLINES_ARM64_H__
#define __TRAMPOLINES_ARM64_H__

#ifdef __cplusplus
extern "C" {
#endif

struct BigDouble {
	struct _f {
		float f1;
		float f2;
	};
	union {
		__int128 bits;
		long double d;
		_f f;
	};
};

struct XamarinCallState {
	uint64_t type;
	uint64_t sp;
	uint64_t x0;
	uint64_t x1;
	uint64_t x2;
	uint64_t x3;
	uint64_t x4;
	uint64_t x5;
	uint64_t x6;
	uint64_t x7;
	uint64_t x8;

	struct BigDouble q0;
	struct BigDouble q1;
	struct BigDouble q2;
	struct BigDouble q3;
	struct BigDouble q4;
	struct BigDouble q5;
	struct BigDouble q6;
	struct BigDouble q7;

	// Make a copy of x0/x1, so the sel() and self () functions return the correct values even after putting return values to x0/x1.
	id _self;
	SEL _sel;

	bool is_stret () { return false; }
	id self () { return _self; }
	SEL sel () { return _sel; }
};

struct ParamIterator {
	struct XamarinCallState *state;
	int ngrn; // Next General-purpose Register Number
	int nsrn; // Next SIMD and Floating-point Register Number
	uint8_t *nsaa; // Next stacked argument address.

	// computed values
	uint64_t *x;
	struct BigDouble *q;
};

void xamarin_arch_trampoline (struct XamarinCallState *state);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __TRAMPOLINES_ARM64_H__ */

