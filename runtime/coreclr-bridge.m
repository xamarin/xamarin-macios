/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
*  Authors: Rolf Bjarne Kvinge
*
*  Copyright (C) 2021 Microsoft Corp.
*
*/

#if defined (CORECLR_RUNTIME)

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
xamarin_handle_bridge_exception (GCHandle gchandle, const char *method)
{
	if (gchandle == INVALID_GCHANDLE)
		return;

	if (method == NULL)
		method = "<unknown method";

	fprintf (stderr, "%s threw an exception: %p\n", method, gchandle);
	xamarin_assertion_message ("%s threw an exception: %p", method, gchandle);
}

#endif // CORECLR_RUNTIME
