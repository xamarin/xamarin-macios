//
// simlauncher.m: Defaults for the simlauncher binaries
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012-2015 Xamarin Inc. 
//

#include "xamarin/xamarin.h"

#ifdef __cplusplus
extern "C" {
#endif

void xamarin_create_classes_Xamarin_iOS ();

#ifdef __cplusplus
}
#endif

void xamarin_setup_impl ()
{
#if DEBUG
	xamarin_debug_mode = TRUE;
#endif
	xamarin_create_classes_Xamarin_iOS ();
	xamarin_marshal_managed_exception_mode = MarshalManagedExceptionModeDisable;
#if DEBUG
	xamarin_marshal_objectivec_exception_mode = MarshalObjectiveCExceptionModeUnwindManagedCode;
#else
	xamarin_marshal_objectivec_exception_mode = MarshalObjectiveCExceptionModeDisabled;
#endif

	mono_dllmap_insert (NULL, "System.Native", NULL, "mono-native", NULL);
	mono_dllmap_insert (NULL, "System.Security.Cryptography.Native.Apple", NULL, "mono-native", NULL);
}

int
main (int argc, char** argv)
{
	@autoreleasepool { return xamarin_main (argc, argv, XamarinLaunchModeApp); }
}


void xamarin_initialize_callbacks () __attribute__ ((constructor));
void xamarin_initialize_callbacks ()
{
	xamarin_setup = xamarin_setup_impl;
}
