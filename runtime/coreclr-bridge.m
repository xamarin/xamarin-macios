/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
*  Authors: Rolf Bjarne Kvinge
*
*  Copyright (C) 2021 Microsoft Corp.
*
*/

#if defined (CORECLR_RUNTIME)

#include "product.h"
#include "xamarin/xamarin.h"
#include "xamarin/coreclr-bridge.h"

#include "coreclrhost.h"

unsigned int coreclr_domainId = 0;
void *coreclr_handle = NULL;

void
xamarin_bridge_setup ()
{
}

void
xamarin_bridge_initialize ()
{
}

bool
xamarin_bridge_vm_initialize (int propertyCount, const char **propertyKeys, const char **propertyValues)
{
	int rv;

	const char *executablePath = [[[[NSBundle mainBundle] executableURL] path] UTF8String];
	rv = coreclr_initialize (
		executablePath,
		xamarin_executable_name,
		propertyCount,
		propertyKeys,
		propertyValues,
		&coreclr_handle,
		&coreclr_domainId
		);

	LOG_CORECLR (stderr, "xamarin_vm_initialize (%i, %p, %p): rv: %i domainId: %i handle: %p\n", propertyCount, propertyKeys, propertyValues, rv, coreclr_domainId, coreclr_handle);

	return rv == 0;
}

void
xamarin_install_nsautoreleasepool_hooks ()
{
	// https://github.com/xamarin/xamarin-macios/issues/11256
	fprintf (stderr, "TODO: add support for wrapping all threads with NSAutoreleasePools.\n");
}

void
xamarin_handle_bridge_exception (GCHandle gchandle, const char *method)
{
	if (gchandle == INVALID_GCHANDLE)
		return;

	if (method == NULL)
		method = "<unknown method";

	fprintf (stderr, "%s threw an exception: %p => %s\n", method, gchandle, [xamarin_print_all_exceptions (gchandle) UTF8String]);
	xamarin_assertion_message ("%s threw an exception: %p = %s", method, gchandle, [xamarin_print_all_exceptions (gchandle) UTF8String]);
}

typedef void (*xamarin_runtime_initialize_decl)(struct InitializationOptions* options);
void
xamarin_bridge_call_runtime_initialize (struct InitializationOptions* options, GCHandle* exception_gchandle)
{
	void *del = NULL;
	int rv = coreclr_create_delegate (coreclr_handle, coreclr_domainId, PRODUCT ", Version=0.0.0.0", "ObjCRuntime.Runtime", "Initialize", &del);
	if (rv != 0)
		xamarin_assertion_message ("xamarin_bridge_call_runtime_initialize: failed to create delegate: %i\n", rv);

	xamarin_runtime_initialize_decl runtime_initialize = (xamarin_runtime_initialize_decl) del;
	runtime_initialize (options);
}

void
xamarin_bridge_register_product_assembly (GCHandle* exception_gchandle)
{
	MonoAssembly *assembly;
	assembly = xamarin_open_and_register (PRODUCT_DUAL_ASSEMBLY, exception_gchandle);
	xamarin_mono_object_release (&assembly);
}

MonoMethod *
xamarin_bridge_get_mono_method (MonoReflectionMethod *method)
{
	// MonoMethod and MonoReflectionMethod are identical in CoreCLR (both are actually MonoObjects).
	// However, we're returning a retained object, so we need to retain here.
	xamarin_mono_object_retain (method);
	LOG_CORECLR (stderr, "%s (%p): rv: %p\n", __func__, method, method);
	return method;
}

MonoClass *
xamarin_get_nsnumber_class ()
{
	xamarin_assertion_message ("The method %s it not implemented yet for CoreCLR", __func__);
}

MonoClass *
xamarin_get_nsvalue_class ()
{
	xamarin_assertion_message ("The method %s it not implemented yet for CoreCLR", __func__);
}

MonoClass *
xamarin_get_inativeobject_class ()
{
	xamarin_assertion_message ("The method %s it not implemented yet for CoreCLR", __func__);
}

MonoClass *
xamarin_get_nsobject_class ()
{
	xamarin_assertion_message ("The method %s it not implemented yet for CoreCLR", __func__);
}

MonoClass *
xamarin_get_nsstring_class ()
{
	xamarin_assertion_message ("The method %s it not implemented yet for CoreCLR", __func__);
}

MonoClass *
xamarin_get_runtime_class ()
{
	xamarin_assertion_message ("The method %s it not implemented yet for CoreCLR", __func__);
}

void
xamarin_mono_object_retain (MonoObject *mobj)
{
	atomic_fetch_add (&mobj->reference_count, 1);
}

void
xamarin_mono_object_release (MonoObject **mobj_ref)
{
	MonoObject *mobj = *mobj_ref;

	if (mobj == NULL)
		return;

	int rc = atomic_fetch_sub (&mobj->reference_count, 1) - 1;
	if (rc == 0) {
		if (mobj->gchandle != INVALID_GCHANDLE) {
			xamarin_gchandle_free (mobj->gchandle);
			mobj->gchandle = INVALID_GCHANDLE;
		}

		xamarin_free (mobj); // allocated using Marshal.AllocHGlobal.
	}

	*mobj_ref = NULL;
}

void
xamarin_mono_object_release (MonoReflectionType **mobj)
{
	xamarin_mono_object_release ((MonoObject **) mobj);
}

void
xamarin_mono_object_release (MonoString **mobj)
{
	xamarin_mono_object_release ((MonoObject **) mobj);
}

/* Implementation of the Mono Embedding API */

// returns a retained MonoAssembly *
MonoAssembly *
mono_assembly_open (const char * filename, MonoImageOpenStatus * status)
{
	assert (status == NULL);

	MonoAssembly *rv = xamarin_find_assembly (filename);

	LOG_CORECLR (stderr, "mono_assembly_open (%s, %p) => MonoObject=%p GCHandle=%p\n", filename, status, rv, rv->gchandle);

	return rv;
}

MonoDomain *
mono_domain_get (void)
{
	// This is not needed for CoreCLR.
	return NULL;
}

// returns a retained MonoReflectionAssembly *
MonoReflectionAssembly *
mono_assembly_get_object (MonoDomain * domain, MonoAssembly * assembly)
{
	// MonoAssembly and MonoReflectionAssembly are identical in CoreCLR (both are actually MonoObjects).
	// However, we're returning a retained object, so we need to retain here.
	xamarin_mono_object_retain (assembly);
	LOG_CORECLR (stderr, "mono_assembly_get_object (%p, %p): rv: %p\n", domain, assembly, assembly);
	return assembly;
}

int
mono_jit_exec (MonoDomain * domain, MonoAssembly * assembly, int argc, const char** argv)
{
	unsigned int exitCode = 0;

	char *assemblyName = xamarin_bridge_get_assembly_name (assembly->gchandle);

	LOG_CORECLR (stderr, "mono_jit_exec (%p, %p, %i, %p) => EXECUTING %s\n", domain, assembly, argc, argv, assemblyName);
	for (int i = 0; i < argc; i++) {
		LOG_CORECLR (stderr, "    Argument #%i: %s\n", i + 1, argv [i]);
	}

	int rv = coreclr_execute_assembly (coreclr_handle, coreclr_domainId, argc, argv, assemblyName, &exitCode);

	LOG_CORECLR (stderr, "mono_jit_exec (%p, %p, %i, %p) => EXECUTING %s rv: %i exitCode: %i\n", domain, assembly, argc, argv, assemblyName, rv, exitCode);

	xamarin_free (assemblyName);

	if (rv != 0)
		xamarin_assertion_message ("mono_jit_exec failed: %i\n", rv);

	return (int) exitCode;
}

MonoClass *
mono_method_get_class (MonoMethod * method)
{
	MonoClass *rv = xamarin_bridge_get_method_declaring_type (method);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, method, rv);
	return rv;
}

MonoClass *
mono_object_get_class (MonoObject * obj)
{
	MonoClass *rv = xamarin_bridge_object_get_type (obj);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, obj, rv);
	return rv;
}

MonoObject *
mono_object_isinst (MonoObject * obj, MonoClass * klass)
{
	bool rv = xamarin_bridge_isinstance (obj, klass);
	LOG_CORECLR (stderr, "%s (%p, %p) => %i\n", __func__, obj, klass, rv);
	return rv ? obj : NULL;
}

// Return value: NULL, or a retained MonoObject* that must be freed with xamarin_mono_object_release.
// Returns NULL in case of exception.
MonoObject *
mono_runtime_invoke (MonoMethod * method, void * obj, void ** params, MonoObject ** exc)
{
	MonoObject *rv = NULL;
	GCHandle exception_gchandle = INVALID_GCHANDLE;

	LOG_CORECLR (stderr, "%s (%p, %p, %p, %p)\n", __func__, method, obj, params, exc);

	rv = xamarin_bridge_runtime_invoke_method (method, (MonoObject *) obj, params, &exception_gchandle);

	if (exc == NULL) {
		xamarin_handle_bridge_exception (exception_gchandle, __func__);
	} else {
		*exc = xamarin_gchandle_unwrap (exception_gchandle);
	}

	return rv;
}

MonoMethodSignature *
mono_method_signature (MonoMethod* method)
{
	MonoMethodSignature *rv = xamarin_bridge_method_get_signature (method);

	LOG_CORECLR (stderr, "xamarin_bridge_mono_method_signature (%p) => %p\n", method, rv);

	return rv;
}

MonoType *
mono_signature_get_params (MonoMethodSignature* sig, void ** iter)
{
	int* p = (int *) iter;
	if (*p >= sig->parameter_count) {
		LOG_CORECLR (stderr, "%s (%p, %p => %i) => DONE\n", __func__, sig, iter, *p);
		return NULL;
	}

	MonoObject *rv = sig->parameters [*p];
	xamarin_mono_object_retain (rv);

	LOG_CORECLR (stderr, "%s (%p, %p => %i) => %p NEXT\n", __func__, sig, iter, *p, rv->gchandle);

	*p = *p + 1;

	return rv;
}

MonoType *
mono_signature_get_return_type (MonoMethodSignature* sig)
{
	MonoType *rv = sig->return_type;
	xamarin_mono_object_retain (rv);

	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, sig, rv);

	return rv;
}

void
xamarin_bridge_free_mono_signature (MonoMethodSignature **psig)
{
	MonoMethodSignature *sig = *psig;

	for (int i = 0; i < sig->parameter_count; i++) {
		xamarin_mono_object_release (&sig->parameters [i]);
	}
	xamarin_mono_object_release (&sig->return_type);

	mono_free (sig);

	*psig = NULL;
}

#endif // CORECLR_RUNTIME
