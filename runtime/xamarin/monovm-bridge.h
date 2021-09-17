/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
*  Authors: Rolf Bjarne Kvinge
*
*  Copyright (C) 2021 Microsoft Corp.
*
*/

/* Support code for using MonoVM */

#if !defined (CORECLR_RUNTIME)

#ifndef __MONOVM_BRIDGE__
#define __MONOVM_BRIDGE__

#include "mono-runtime.h"

#define LOG_MONOVM(...)
//#define LOG_MONOVM(...) fprintf (__VA_ARGS__)

#ifdef __cplusplus
extern "C" {
#endif

struct _MonoObject {
	MonoVTable *vtable;
	MonoThreadsSync *synchronisation;
};

struct Managed_NSObject {
	MonoObject obj;
	id handle;
	void *class_handle;
	uint8_t flags;
};

typedef struct {
	MonoObject object;
	MonoMethod *method;
	MonoString *name;
	MonoReflectionType *reftype;
} PublicMonoReflectionMethod;

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __MONOVM_BRIDGE__ */

#endif // !CORECLR_RUNTIME
