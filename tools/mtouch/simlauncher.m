//
// simlauncher.m: Defaults for the simlauncher binaries
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012-2015 Xamarin Inc. 
//

#include "xamarin/xamarin.h"

void xamarin_create_classes_Xamarin_iOS ();

void xamarin_setup_impl ()
{
#if DEBUG
	xamarin_debug_mode = TRUE;
#endif
#if XAMCORE_2_0
	xamarin_use_new_assemblies = TRUE;
	xamarin_create_classes_Xamarin_iOS ();
#else
	xamarin_use_new_assemblies = FALSE;
#endif
}

int
main (int argc, char** argv)
{
	@autoreleasepool { return xamarin_main (argc, argv, false); }
}


void xamarin_initialize_callbacks () __attribute__ ((constructor));
void xamarin_initialize_callbacks ()
{
	xamarin_setup = xamarin_setup_impl;
}