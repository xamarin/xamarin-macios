/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
*  Authors: Rolf Bjarne Kvinge
*
*  Copyright (C) 2014 - 2015 Xamarin Inc. (www.xamarin.com)
*
*/

#ifdef MONOTOUCH
  #if TARGET_OS_WATCH
	#define PRODUCT                  "Xamarin.WatchOS"
  #elif TARGET_OS_MACCATALYST
  #define PRODUCT                  "Xamarin.MacCatalyst"
  #elif TARGET_OS_IOS
	#define PRODUCT                  "Xamarin.iOS"
  #elif TARGET_OS_TV
    #define PRODUCT                  "Xamarin.TVOS"
  #else
    #error Unknown MONOTOUCH product
  #endif
  #if TARGET_OS_WATCH
	#define PRODUCT_DUAL_ASSEMBLY    "Xamarin.WatchOS.dll"
  #elif TARGET_OS_MACCATALYST
  #define PRODUCT_DUAL_ASSEMBLY    "Xamarin.MacCatalyst.dll"
  #elif TARGET_OS_IOS
	#define PRODUCT_DUAL_ASSEMBLY    "Xamarin.iOS.dll"
  #elif TARGET_OS_TV
    #define PRODUCT_DUAL_ASSEMBLY    "Xamarin.TVOS.dll"
  #else
    #error Unknown MONOTOUCH product for dual assembly
  #endif
	#define PRODUCT_EXCEPTION_TYPE   "MonoTouchException"
	#ifdef __LP64__
		#define ARCH_SUBDIR ".monotouch-64"
	#else
		#define ARCH_SUBDIR ".monotouch-32"
	#endif

#elif MONOMAC
	#define PRODUCT                  "Xamarin.Mac"
	#define PRODUCT_DUAL_ASSEMBLY    "Xamarin.Mac.dll"
	#define PRODUCT_EXCEPTION_TYPE   "ObjCException"
	#define ARCH_SUBDIR				
#else
    #error Either MONOTOUCH or MONOMAC must be defined.
#endif