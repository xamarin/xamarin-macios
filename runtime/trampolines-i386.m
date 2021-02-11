
#if defined(__i386__)

#include <stdint.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>

#include "trampolines-internal.h"
#include "xamarin/runtime.h"
#include "runtime-internal.h"
#include "trampolines-i386.h"

#ifdef TRACE
static void
dump_state (struct XamarinCallState *state)
{
	fprintf (stderr, "type: %u is_stret: %i self: %p SEL: %s eax: 0x%x edx: 0x%x esp: 0x%x -- double_ret: %f float_ret: %f\n",
		state->type, state->is_stret (), state->self (), sel_getName (state->sel ()), state->eax, state->edx, state->esp,
		state->double_ret, state->float_ret);
}
#else
#define dump_state(...)
#endif

static void
param_iter_next (enum IteratorAction action, void *context, const char *type, size_t size, void *target, GCHandle *exception_gchandle)
{
	struct ParamIterator *it = (struct ParamIterator *) context;
	
	if (action == IteratorStart) {
		bool is_stret = (it->state->type & Tramp_Stret) == Tramp_Stret;
		// skip past the pointer to the previous function, and the two (three in case of stret) first arguments (id + SEL).
		it->stack_next = (uint8_t *) (it->state->esp + (is_stret ? 16 : 12));
		if (is_stret) {
			it->stret = *(uint8_t **) (it->state->esp + 4);
			it->state->eax = (uint32_t) it->stret;
		} else {
			it->stret = NULL;
		}
		LOGZ("initialized parameter iterator to %p stret to %p\n", it->stack_next, it->stret);
		return;
	} else if (action == IteratorEnd) {
		return;
	}

	// target must be at least pointer sized, and we need to zero it out first.
	if (target != NULL)
		*(uint32_t *) target = 0;

	// passed on the stack
	if (target != NULL) {
		LOGZ("read %lu bytes from stack pointer at %p\n", size, it->stack_next);
		memcpy (target, it->stack_next, size);
	} else {
		LOGZ("skipped over %lu bytes from stack pointer at %p\n", size, it->stack_next);
	}
	// increment stack pointer
	it->stack_next += size;
	// and round up to 4 bytes.
	if (size % 4 != 0)
		it->stack_next += 4 - size % 4;
}

static void
marshal_return_value (void *context, const char *type, size_t size, void *vvalue, MonoType *mtype, bool retain, MonoMethod *method, MethodDescription *desc, GCHandle *exception_gchandle)
{
	MonoObject *value = (MonoObject *) vvalue;
	struct ParamIterator *it = (struct ParamIterator *) context;

	LOGZ (" marshalling return value %p as %s\n", value, type);

	it->state->double_ret = 0;

	switch (type [0]) {
	case _C_FLT:
		// single floating point return value
		it->state->float_ret = *(float *) mono_object_unbox (value);
		break;
	case _C_DBL:
		// double floating point return value
		it->state->double_ret = *(double *) mono_object_unbox (value);
		break;
	case _C_STRUCT_B:
		if ((it->state->type & Tramp_Stret) == Tramp_Stret) {
			memcpy ((void *) it->stret, mono_object_unbox (value), size);
			break;
		}
	
		if (size > 4 && size <= 8) {
			type = xamarin_skip_type_name (type);
			if (size == 8 && !strncmp (type, "d}", 2)) {
				it->state->double_ret = *(double *) mono_object_unbox (value);
				// structures containing a single float/double value use
				// objc_msgSend (not objc_msgSend_fpret), but they still
				// behave like objc_msgSend_fpret (they return the value using
				// the floating point stack). Here we fake that behavior by
				// overring the trampoline type, so that the assembler code
				// that handles the return value knows to push the return
				// value on the floating point stack.
				it->state->type = Tramp_FpretDouble | (it->state->type & Tramp_Static);
			} else {
				// returned in %eax and %edx
				void *unboxed = mono_object_unbox (value);

				// read the struct into 2 32bit values.
				uint32_t v[2];
				v[0] = *(uint32_t *) unboxed;
				// read as much as we can of the second value
				unboxed = 1 + (uint32_t *) unboxed;
				if (size == 8) {
					v[1] = *(uint32_t *) unboxed;
				} else if (size == 6) {
					v[1] = *(uint16_t *) unboxed;
				} else if (size == 5) {
					v[1] = *(uint8_t *) unboxed;
				} else {
					v[1] = 0; // theoretically impossible, but it silences static analysis, and if the real world proves the theory wrong, then we still get consistent behavior.
				}
				it->state->eax = v[0];
				it->state->edx = v[1];
			}
		} else if (size == 4) {
			type = xamarin_skip_type_name (type);
			if (!strncmp (type, "f}", 2)) {
				it->state->float_ret = *(float *) mono_object_unbox (value);
				// structures containing a single float/double value use
				// objc_msgSend (not objc_msgSend_fpret), but they still
				// behave like objc_msgSend_fpret (they return the value using
				// the floating point stack). Here we fake that behavior by
				// overring the trampoline type, so that the assembler code
				// that handles the return value knows to push the return
				// value on the floating point stack.
				it->state->type = Tramp_FpretSingle | (it->state->type & Tramp_Static);
			} else {
				it->state->eax = *(uint32_t *) mono_object_unbox (value);
			}
		} else if (size == 2) {
			it->state->eax = *(uint16_t *) mono_object_unbox (value);
		} else if (size == 1) {
			it->state->eax = *(uint8_t *) mono_object_unbox (value);
		} else {
			*exception_gchandle = xamarin_create_mt_exception (xamarin_strdup_printf ("Xamarin.iOS: Cannot marshal struct return type %s (size: %i)\n", type, (int) size));
		}
		break;
	// For primitive types we get a pointer to the actual value
	case _C_BOOL: // signed char
	case _C_CHR:
	case _C_UCHR:
		it->state->eax = *(uint8_t *) mono_object_unbox (value);
		break;
	case _C_SHT:
	case _C_USHT:
		it->state->eax = *(uint16_t *) mono_object_unbox (value);
		break;
	case _C_INT:
	case _C_UINT:
		it->state->eax = *(uint32_t *) mono_object_unbox (value);
		break;
	case _C_LNG:
	case _C_ULNG:
	case _C_LNG_LNG:
	case _C_ULNG_LNG:
		*(uint64_t *) &it->state->eax = *(uint64_t *) mono_object_unbox (value);
		break;
	
	// For pointer types we get the value itself.
	case _C_CLASS:
	case _C_SEL:
	case _C_ID:
	case _C_CHARPTR:
	case _C_PTR:
		if (value == NULL) {
			it->state->eax = 0;
			break;
		}

		it->state->eax = (uint32_t) xamarin_marshal_return_value (it->state->sel (), mtype, type, value, retain, method, desc, exception_gchandle);
		break;
	case _C_VOID:
		break;
	case '|': // direct pointer value
	default:
		if (size == 4) {
			it->state->eax = (uint32_t) value;
		} else {
			*exception_gchandle = xamarin_create_mt_exception (xamarin_strdup_printf ("Xamarin.iOS: Cannot marshal return type %s (size: %i)\n", type, (int) size));
		}
		break;
	}
	
}

void
xamarin_arch_trampoline (struct XamarinCallState *state)
{
	dump_state (state);
	struct ParamIterator iter;
	iter.state = state;
	xamarin_invoke_trampoline ((enum TrampolineType) state->type, state->self (), state->sel (), param_iter_next, marshal_return_value, &iter);
	dump_state (state);
}

#endif /* __i386__ */