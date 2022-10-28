#if !defined (__i386__) && !defined (__x86_64__) && !(defined (__arm64__) && !defined(__ILP32__))
#define __VARARGS_TRAMPOLINES__ 1
#endif

#if defined(__VARARGS_TRAMPOLINES__)

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
#include "trampolines-varargs.h"

#ifdef TRACE
static void
dump_state (struct XamarinCallState *state)
{
	PRINT ("type: %u is_stret: %i self: %p SEL: %s -- double_ret: %f float_ret: %f longlong_ret: %llu ptr_ret: %p\n",
		state->type, (state->type & Tramp_Stret) == Tramp_Stret, state->self, sel_getName (state->sel),
		state->double_ret, state->float_ret, state->longlong_ret, state->ptr_ret);
}
#else
#define dump_state(...)
#endif

static void
param_iter_next (enum IteratorAction action, void *context, const char *type, size_t size, void *target, GCHandle *exception_gchandle)
{
	struct ParamIterator *it = (struct ParamIterator *) context;
	struct XamarinCallState *state = it->state;
	
	if (action == IteratorStart) {
		va_copy (it->ap, state->ap);
		return;
	} else if (action == IteratorEnd) {
		va_end (it->ap);
		return;
	}

	if (target == NULL) {
		LOGZ("skipping over %lu bytes\n", size);
		if (size % sizeof (void *) != 0)
			xamarin_assertion_message ("Cannot marshal structure of type '%s' with size %i since it's not a multiple of %i.\n", type, size, sizeof (void *));

		do {
			va_arg (it->ap, void *);
			size -= 4;
		} while (size > 0);
		return;
	}
	
	void **target_ptr = (void **) target;
	if (size > sizeof (void *)) {
		if (size % sizeof (void *) != 0)
			xamarin_assertion_message ("Cannot marshal structure of type '%s' with size %i since it's not a multiple of %i.\n", type, size, sizeof (void *));

		size_t size_left = size;
		do {
			*target_ptr++ = va_arg (it->ap, void *);
			size_left -= sizeof (void *);
			LOGZ("read %lu bytes (%lu left) from va arg: %p\n", sizeof (void *), size_left, *target_ptr);
		} while (size_left > 0);	
	} else {
		*target_ptr = va_arg (it->ap, void *);
		LOGZ("read %lu bytes from va arg: %p\n", size, *target_ptr);
	}
}

static void
marshal_return_value (void *context, const char *type, size_t size, void *vvalue, MonoType *mtype, bool retain, MonoMethod *method, MethodDescription *desc, GCHandle *exception_gchandle)
{
	MonoObject *value = (MonoObject *) vvalue;
	struct ParamIterator *it = (struct ParamIterator *) context;
	struct XamarinCallState *state = it->state;

	LOGZ (" marshalling return value %p as %s\n", value, type);

	it->state->double_ret = 0;

	switch (type [0]) {
	case _C_FLT:
		// single floating point return value
		state->float_ret = *(float *) mono_object_unbox (value);
		break;
	case _C_DBL:
		// double floating point return value
		state->double_ret = *(double *) mono_object_unbox (value);
		break;
	case _C_STRUCT_B: {
		bool is_stret = (state->type & Tramp_Stret) == Tramp_Stret;
		if (is_stret) {
			memcpy (state->buffer, mono_object_unbox (value), size);
			break;
		}
	
		if (size <= sizeof (void *)) {
			state->ptr_ret = *(void **) mono_object_unbox (value);
		} else if (size <= 8) {
			state->longlong_ret = 0;
			memcpy (&state->longlong_ret + 8 - size, mono_object_unbox (value), size);
		} else {
			*exception_gchandle = xamarin_create_mt_exception (xamarin_strdup_printf ("Xamarin.iOS: Cannot marshal struct return type %s (size: %i)\n", type, (int) size));
		}
		break;
	}
	// For primitive types we get a pointer to the actual value
	case _C_BOOL: // signed char
	case _C_CHR:
	case _C_UCHR:
		state->ptr_ret = (void *) (intptr_t) *(uint8_t *) mono_object_unbox (value);
		break;
	case _C_SHT:
	case _C_USHT:
		state->ptr_ret = (void *) (intptr_t)  *(uint16_t *) mono_object_unbox (value);
		break;
	case _C_INT:
	case _C_UINT:
		state->ptr_ret = (void *) (intptr_t) *(uint32_t *) mono_object_unbox (value);
		break;
	case _C_LNG:
	case _C_ULNG:
	case _C_LNG_LNG:
	case _C_ULNG_LNG:
		state->longlong_ret = *(int64_t *) mono_object_unbox (value);
		break;
	
	// For pointer types we get the value itself.
	case _C_CLASS:
	case _C_SEL:
	case _C_ID:
	case _C_CHARPTR:
	case _C_PTR:
		if (value == NULL) {
			state->ptr_ret = 0;
			break;
		}

		state->ptr_ret = xamarin_marshal_return_value (it->state->sel, mtype, type, value, retain, method, desc, exception_gchandle);
		break;
	case _C_VOID:
		break;
	case '|': // direct pointer value
	default:
		if (size == sizeof (void *)) {
			state->ptr_ret = value;
		} else {
			*exception_gchandle = xamarin_create_mt_exception (xamarin_strdup_printf ("Xamarin.iOS: Cannot marshal return type %s (size: %i)\n", type, (int) size));
		}
		break;
	}
}

static void
xamarin_varargs_trampoline (struct XamarinCallState *state)
{
	dump_state (state);
	struct ParamIterator iter;
	iter.state = state;
	xamarin_invoke_trampoline (state->type, state->self, state->sel, param_iter_next, marshal_return_value, &iter);
	dump_state (state);
}

double
xamarin_fpret_double_trampoline (id self, SEL sel, ...)
{
	struct XamarinCallState state;
	state.type = Tramp_FpretDouble;
	state.self = self;
	state.sel = sel;
	state.buffer = NULL;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
	return state.double_ret;
}

float
xamarin_fpret_single_trampoline (id self, SEL sel, ...)
{
	struct XamarinCallState state;
	state.type = Tramp_FpretSingle;
	state.self = self;
	state.sel = sel;
	state.buffer = NULL;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
	return state.float_ret;
}

long long
xamarin_longret_trampoline (id self, SEL sel, ...)
{
	struct XamarinCallState state;
	state.type = Tramp_LongRet;
	state.self = self;
	state.sel = sel;
	state.buffer = NULL;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
	return state.longlong_ret;
}

void
xamarin_stret_trampoline (void *buffer, id self, SEL sel, ...)
{
	struct XamarinCallState state;
	state.type = Tramp_Stret;
	state.self = self;
	state.sel = sel;
	state.buffer = buffer;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
}

void *
xamarin_trampoline (id self, SEL sel, ...)
{
	struct XamarinCallState state;
	state.type = Tramp_Default;
	state.self = self;
	state.sel = sel;
	state.buffer = NULL;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
	return state.ptr_ret;
}

void *
xamarin_ctor_trampoline (id self, SEL sel, ...)
{
	struct XamarinCallState state;
	state.type = Tramp_Ctor;
	state.self = self;
	state.sel = sel;
	state.buffer = NULL;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
	return state.ptr_ret;
}

void *
xamarin_static_trampoline (id self, SEL sel, ...)
{
	struct XamarinCallState state;
	state.type = Tramp_Static;
	state.self = self;
	state.sel = sel;
	state.buffer = NULL;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
	return state.ptr_ret;
}

float
xamarin_static_fpret_single_trampoline (id self, SEL sel, ...)
{
	struct XamarinCallState state;
	state.type = Tramp_StaticFpretSingle;
	state.self = self;
	state.sel = sel;
	state.buffer = NULL;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
	return state.float_ret;
}

double
xamarin_static_fpret_double_trampoline (id self, SEL sel, ...)
{
	struct XamarinCallState state;
	state.type = Tramp_StaticFpretDouble;
	state.self = self;
	state.sel = sel;
	state.buffer = NULL;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
	return state.double_ret;
}

long long
xamarin_static_longret_trampoline (id self, SEL sel, ...)
{
	struct XamarinCallState state;
	state.type = Tramp_StaticLongRet;
	state.self = self;
	state.sel = sel;
	state.buffer = NULL;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
	return state.longlong_ret;
}

void
xamarin_static_stret_trampoline (void *buffer, id self, SEL sel, ...)
{
	XamarinCallState state;
	state.type = Tramp_StaticStret;
	state.self = self;
	state.sel = sel;
	state.buffer = buffer;
	va_start (state.ap, sel);
	xamarin_varargs_trampoline (&state);
	va_end (state.ap);
}

#endif /* __VARARGS_TRAMPOLINES__ */