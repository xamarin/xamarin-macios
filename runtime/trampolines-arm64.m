
#if defined(__arm64__)

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
#include "trampolines-arm64.h"
#include "product.h"

/*
 * https://developer.apple.com/library/archive/documentation/Xcode/Conceptual/iPhoneOSABIReference/Articles/ARM64FunctionCallingConventions.html
 *
 * Standard arm64 calling convention:
 * Input:
 *   x0 - x7 (x8 too in some cases)
 *   q0 - q7 (simd/floating point)
 * Output:
 *   same as input
 *
 */

#ifdef TRACE
static void
dump_state (struct XamarinCallState *state, const char *prefix)
{
	fprintf (stderr, "%stype: %llu self: %p SEL: %s sp: 0x%llx x0: 0x%llx x1: 0x%llx x2: 0x%llx x3: 0x%llx x4: 0x%llx x5: 0x%llx x6: 0x%llx x7: 0x%llx x8: 0x%llx -- q0: %Lf q1: %Lf q2: %Lf q3: %Lf q4: %Lf q5: %Lf q6: %Lf q7: %Lf\n",
		prefix, state->type, state->self (), sel_getName (state->sel ()), state->sp, state->x0, state->x1, state->x2, state->x3, state->x4, state->x5, state->x6, state->x7, state->x8,
		state->q0.d, state->q1.d, state->q2.d, state->q3.d, state->q4.d, state->q5.d, state->q6.d, state->q7.d);
}
#else
#define dump_state(...)
#endif

static uint64_t align_uint64 (uint64_t target, uint64_t alignment)
{
	target = (target + (alignment - 1)) & ~(alignment - 1);
	return target;
}

static size_t align_size (size_t target, uint64_t alignment)
{
	target = (target + (alignment - 1)) & ~(alignment - 1);
	return target;
}

static void* align_ptr (void* target, uint64_t alignment)
{
	if (target == NULL)
		return NULL;

	uint64_t ptr = (uint64_t) target;
	ptr = (ptr + (alignment - 1)) & ~(alignment - 1);
	return (void *) ptr;
}

static int
param_read_primitive (struct ParamIterator *it, const char *type_ptr, void *target, size_t total_size, bool prohibit_fp_registers, GCHandle *exception_gchandle)
{
	// COOP: does not access managed memory: any mode.
	char type = *type_ptr;
	LOGZ ("        reading primitive %c. total size: %i nsrn: %i ngrn: %i ngrn_offset: %zu nsaa: %p\n", type, (int) total_size, it->nsrn, it->ngrn, it->ngrn_offset, it->nsaa);

	switch (type) {
	case _C_FLT: {
		size_t size = 4;
		target = align_ptr (target, size);
		if (prohibit_fp_registers && it->ngrn < 8) {
			// if we can't use fp registers, then must use the standard logic
			goto DEFAULT;
		} else if (!prohibit_fp_registers && it->nsrn < 8) {
			// if we can use fp registers, and we haven't used up all the fp registers, then read from the fp registers
			if (target != NULL) {
				*(float *) target = *(float *) &it->q [it->nsrn];
				LOGZ ("        reading float at q%i into %p: %f\n", it->nsrn, target, *(float *) target);
			}
			it->nsrn++;
		} else {
			// read from stack
			it->nsaa = (uint8_t *) align_ptr (it->nsaa, size); // align to natural alignment for the value we want to read
			if (target != NULL) {
				*(float *) target = *(float *) it->nsaa;
				LOGZ ("        reading float at stack %p into %p: %f\n", it->nsaa, target, *(float *) target);
			}
			it->nsaa += size;
		}
		return (int) size;
	}
	case _C_DBL: {
		size_t size = 8;
		target = align_ptr (target, size);
		if (prohibit_fp_registers && it->ngrn < 8) {
			// if we can't use fp registers, then must use the standard logic
			goto DEFAULT;
		} else if (!prohibit_fp_registers && it->nsrn < 8) {
			// if we can use fp registers, and we haven't used up all the fp registers, then read from the fp registers
			if (target != NULL) {
				*(double *) target = *(double *) &it->q [it->nsrn];
				LOGZ ("        reading double at q%i into %p: %f\n", it->nsrn, target, *(double *) target);
			}
			it->nsrn++;
		} else {
			// read from stack
			it->nsaa = (uint8_t *) align_ptr (it->nsaa, size); // align to natural alignment for the value we want to read
			if (target != NULL) {
				*(double *) target = *(double *) it->nsaa;
				LOGZ ("        reading double at stack %p into %p: %f\n", it->nsaa, target, *(double *) target);
			}
			it->nsaa += size;
		}
		return (int) size;
	}
DEFAULT:
	default: {
		size_t size = xamarin_get_primitive_size (type);

		if (size == 0)
			return 0;

		uint8_t *ptr;
		bool read_register = it->ngrn < 8;

		target = align_ptr (target, size);

		if (read_register && it->ngrn_offset > 0) {
			// align the offset we're supposed to read from with the size of the current read
			it->ngrn_offset = align_size (it->ngrn_offset, size);

			// if we'd read past the end of the current register, advance to the next register
			if (it->ngrn_offset + size > 8) {
				it->ngrn++;
				it->ngrn_offset = 0;
				read_register = it->ngrn < 8;
			}
		}

		if (read_register) {
			ptr = it->ngrn_offset + (uint8_t *) &it->x [it->ngrn];
			if (target != NULL) {
				LOGZ ("        reading primitive of type %c and size %i from x%i into %p: ", type, (int) size, it->ngrn, target);
			}

			it->ngrn_offset += size;
			// if we're past the current register, advance to the next register
			if (it->ngrn_offset >= 8) {
				it->ngrn++;
				it->ngrn_offset = 0;
			}
		} else {
			// align the pointer we're supposed to read from with the size of the current read
			it->nsaa = (uint8_t *) align_ptr (it->nsaa, size);
			ptr = (uint8_t *) it->nsaa;
			if (target != NULL) {
				LOGZ ("        reading primitive of type %c and size %i from %p into %p: ", type, (int) size, ptr, target);
			}
			it->nsaa += size;
		}

		if (target == NULL) {
			LOGZ (" not reading, since target is NULL.\n");
			return (int) size;
		}

		switch (size) {
		case 8:
			*(uint64_t *) target = *(uint64_t *) ptr;
			LOGZ ("0x%llx = %llu = %f\n", * (uint64_t *) target, * (uint64_t *) target, *(double *) target);
			break;
		case 4:
			*(uint32_t *) target = *(uint32_t *) ptr;
			LOGZ ("0x%x = %u = %f\n", * (uint32_t *) target, * (uint32_t *) target, *(float *) target);
			break;
		case 2:
			*(uint16_t *) target = *(uint16_t *) ptr;
			LOGZ ("0x%x = %u\n", (int) * (uint32_t *) target, (int) * (uint32_t *) target);
			break;
		case 1:
			*(uint8_t *) target = *(uint8_t *) ptr;
			LOGZ ("0x%x = %u = '%c'\n", (int) * (uint8_t *) target, (int) * (uint8_t *) target, (char) * (uint8_t *) target);
			break;
		default:
			*exception_gchandle = xamarin_create_mt_exception (xamarin_strdup_printf (PRODUCT ": Cannot marshal parameter type %c (size: %i): invalid size.\n", type, (int) size));
			return 0;
		}

		return (int) size;
	}
	}
}

static void
param_iter_next (enum IteratorAction action, void *context, const char *type, size_t size, void *target, GCHandle *exception_gchandle)
{
	// COOP: does not access managed memory: any mode.
	struct ParamIterator *it = (struct ParamIterator *) context;

	if (action == IteratorStart) {
		it->ngrn = 2; // we already have two arguments: self + SEL
		it->nsrn = 0;
		it->ngrn_offset = 0;
		it->nsaa = (uint8_t *) it->state->sp;
		it->x = &it->state->x0;
		it->q = &it->state->q0;
		LOGZ ("    initialized parameter iterator. next register: %i next fp register: %i next stack pointer: %p\n", it->ngrn, it->nsrn, it->nsaa);
		return;
	} else if (action == IteratorEnd) {
		return;
	}

	// if the previous parameter ended up reading a partial register, advance to the next register
	if (it->ngrn_offset > 0) {
		it->ngrn++;
		it->ngrn_offset = 0;
	}

	// target must be at least pointer sized, and we need to zero it out first.
	if (target != NULL)
		*(uint64_t *) target = 0;

	char struct_name [5]; // we don't care about structs with more than 4 fields.
	xamarin_collapse_struct_name (type, struct_name, sizeof (struct_name), exception_gchandle);
	if (*exception_gchandle != INVALID_GCHANDLE)
		return;

	// a struct with only 2-4 floats or 2-4 doubles (but not a 2-4 mix of floats and doubles) can be passed in floating point registers.
	size_t struct_length = strlen (struct_name);
	bool in_fp_registers = struct_length >= 2 && struct_length <= 4;
	if (in_fp_registers) {
		for (int i = 1; i < struct_length; i++)
			in_fp_registers &= struct_name [i] == struct_name [0];
	}

	if (size > 16 && !in_fp_registers) {
		LOGZ ("    reading parameter passed by reference. type: %s size: %i next register: %i next fp register: %i nsaa: %p\n", type, (int) size, it->ngrn, it->nsrn, it->nsaa);
		param_read_primitive (it, "^", target, sizeof (void *), false,  exception_gchandle);
		return;
	}

	if (*struct_name == 0) {
		*exception_gchandle = xamarin_create_mt_exception (xamarin_strdup_printf (PRODUCT ": Cannot marshal parameter type %s, collapsed type name is empty\n", type));
		return;
	}

	// passed in registers (and on the stack if not enough registers)
	LOGZ ("    reading parameter from registers/stack. type: %s (collapsed: %s) size: %i in_fp_registers: %i\n", type, struct_name, (int) size, in_fp_registers);
	const char *t = struct_name;
	uint8_t *targ = (uint8_t *) target;
	// members of structs are generally not in floating point registers (except for in_fp_registers structs, see above)
	// here we detect a struct by checking if we have more than one field to read
	bool prohibit_fp_registers = !in_fp_registers && strlen (struct_name) > 1;
	do {
		int c = param_read_primitive (it, t, targ, size, prohibit_fp_registers, exception_gchandle);
		if (*exception_gchandle != INVALID_GCHANDLE)
			return;
		if (targ != NULL)
			targ += c;
	} while (*++t);

	// align stack reads to sizeof(void*) bytes
	it->nsaa = align_uint64 (it->nsaa, 8);
}

static void
marshal_return_value (void *context, const char *type, size_t size, void *vvalue, MonoType *mtype, bool retain, MonoMethod *method, MethodDescription *desc, GCHandle *exception_gchandle)
{
	// COOP: accessing managed memory (as input), so must be in unsafe mode.
	MONO_ASSERT_GC_UNSAFE;

	char struct_name[5];
	MonoObject *value = (MonoObject *) vvalue;
	struct ParamIterator *it = (struct ParamIterator *) context;

	LOGZ ("    marshalling return value %p as %s with size %i\n", value, type, (int) size);

	switch (type [0]) {
	case _C_FLT:
		// single floating point return value
		it->state->q0.f.f1 = *(float *) mono_object_unbox (value);
		break;
	case _C_DBL:
		// double floating point return value
		it->state->q0.d = *(double *) mono_object_unbox (value);
		break;
	case _C_STRUCT_B:
		/*
		 * Structures, this is ugly, but not as bad as x86_64.
		 *
		 * A) Structures of 1 to 4 fields of the same floating point type (float or double), are passed in the q[0-3] registers.
		 * B) Any other structure > 16 bytes is passed by reference (pointer to memory in x8)
		 * C) Structures <= 16 bytes are passed in x0 and x1 as needed. Each register can contain multiple fields.
		 */

		xamarin_collapse_struct_name (type, struct_name, sizeof (struct_name), exception_gchandle);
		if (*exception_gchandle != INVALID_GCHANDLE)
			return;

		if ((size == 32 && !strncmp (struct_name, "dddd", 4)) ||
			(size == 24 && !strncmp (struct_name, "ddd", 3)) ||
			(size == 16 && !strncmp (struct_name, "dd", 2)) ||
			(size == 8 && !strncmp (struct_name, "d", 1))) {
			LOGZ ("        marshalling as %i doubles (struct name: %s)\n", (int) size / 8, struct_name);
			double* ptr = (double *) mono_object_unbox (value);
			for (int i = 0; i < size / 8; i++) {
				LOGZ ("        #%i: %f\n", i, ptr [i]);
				it->q [i].d = ptr [i];
			}
		} else if ((size == 16 && !strncmp (struct_name, "ffff", 4)) ||
				   (size == 12 && !strncmp (struct_name, "fff", 3)) ||
				   (size == 8 && !strncmp (struct_name, "ff", 2)) ||
				   (size == 4 && !strncmp (struct_name, "f", 1))) {
			LOGZ ("        marshalling as %i floats (struct name: %s)\n", (int) size / 4, struct_name);
			float* ptr = (float *) mono_object_unbox (value);
			for (int i = 0; i < size / 4; i++) {
				LOGZ ("        #%i: %f\n", i, ptr [i]);
				it->q [i].f.f1 = ptr [i];
			}
		} else if (size > 16) {
			LOGZ ("        marshalling as stret through x8\n");
			memcpy ((void *) it->state->x8, mono_object_unbox (value), size);
		} else {
			LOGZ ("        marshalling as composite values in x0 and x1\n");
			it->state->x0 = 0;
			it->state->x1 = 0;
			memcpy (&it->state->x0, mono_object_unbox (value), size);
		}
		break;
	// For primitive types we get a pointer to the actual value
	case _C_BOOL: // signed char
	case _C_CHR:
	case _C_UCHR:
		it->state->x0 = *(uint8_t *) mono_object_unbox (value);
		break;
	case _C_SHT:
	case _C_USHT:
		it->state->x0 = *(uint16_t *) mono_object_unbox (value);
		break;
	case _C_INT:
	case _C_UINT:
		it->state->x0 = *(uint32_t *) mono_object_unbox (value);
		break;
	case _C_LNG:
	case _C_ULNG:
	case _C_LNG_LNG:
	case _C_ULNG_LNG:
		it->state->x0 = *(uint64_t *) mono_object_unbox (value);
		break;

	// For pointer types we get the value itself.
	case _C_CLASS:
	case _C_SEL:
	case _C_ID:
	case _C_CHARPTR:
	case _C_PTR:
		if (value == NULL) {
			it->state->x0 = 0;
			break;
		}

		it->state->x0 = (uint64_t) xamarin_marshal_return_value (it->state->sel (), mtype, type, value, retain, method, desc, exception_gchandle);
		break;
	case _C_VOID:
		break;
	case '|': // direct pointer value
	default:
		if (size == 8) {
			it->state->x0 = (uint64_t) value;
		} else {
			*exception_gchandle = xamarin_create_mt_exception (xamarin_strdup_printf (PRODUCT ": Cannot marshal return type %s (size: %i)\n", type, (int) size));
		}
		break;
	}
}

void
xamarin_arch_trampoline (struct XamarinCallState *state)
{
	// COOP: called from ObjC, and does not access managed memory.
	MONO_ASSERT_GC_SAFE;

	state->_self = (id) state->x0;
	state->_sel = (SEL) state->x1;

	dump_state (state, "BEGIN: ");
	struct ParamIterator iter;
	iter.state = state;
	xamarin_invoke_trampoline ((enum TrampolineType) state->type, state->self (), state->sel (), param_iter_next, marshal_return_value, &iter);
	dump_state (state, "END: ");
}

bool
xamarin_arch_param_passed_by_reference (unsigned long size, const char *type, GCHandle *exception_gchandle)
{
	if (size <= 16)
		return false;

	char struct_name [5]; // we don't care about structs with more than 4 fields.
	xamarin_collapse_struct_name (type, struct_name, sizeof (struct_name), exception_gchandle);
	if (*exception_gchandle != INVALID_GCHANDLE)
		return false;

	if (strcmp (struct_name, "dddd") == 0)
		return false;

	if (strcmp (struct_name, "ddd") == 0)
		return false;

	return true;
}

#endif /* __arm64__ */
