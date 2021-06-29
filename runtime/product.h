/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
*  Authors: Rolf Bjarne Kvinge
*
*  Copyright (C) 2014 - 2015 Xamarin Inc. (www.xamarin.com)
*
*/

#include <TargetConditionals.h>

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

// Set RuntimeIdentifier defines
#if TARGET_OS_MACCATALYST
  #define RUNTIMEIDENTIFIER_PLATFORM    "maccatalyst"
#elif TARGET_OS_IOS
  #if TARGET_OS_SIMULATOR
    #define RUNTIMEIDENTIFIER_PLATFORM  "iossimulator"
  #else
    #define RUNTIMEIDENTIFIER_PLATFORM  "ios"
  #endif
#elif TARGET_OS_TV
  #if TARGET_OS_SIMULATOR
    #define RUNTIMEIDENTIFIER_PLATFORM  "tvossimulator"
  #else
    #define RUNTIMEIDENTIFIER_PLATFORM  "tvos"
  #endif
#elif TARGET_OS_WATCH
  #if TARGET_OS_SIMULATOR
    #define RUNTIMEIDENTIFIER_PLATFORM  "watchossimulator"
  #else
    #define RUNTIMEIDENTIFIER_PLATFORM  "watchos"
  #endif
#elif TARGET_OS_OSX
  #define RUNTIMEIDENTIFIER_PLATFORM    "osx"
#else
  #error Unknown platform
#endif

#if defined (__aarch64__)
  #define RUNTIMEIDENTIFIER_ARCHITECTURE "arm64"
#elif defined (__x86_64__)
  #define RUNTIMEIDENTIFIER_ARCHITECTURE "x64"
#elif defined (__i386__)
  #define RUNTIMEIDENTIFIER_ARCHITECTURE "x86"
#elif defined (__arm__)
  #define RUNTIMEIDENTIFIER_ARCHITECTURE "arm"
#else
    #error Unknown architecture
#endif

#define RUNTIMEIDENTIFIER RUNTIMEIDENTIFIER_PLATFORM "-" RUNTIMEIDENTIFIER_ARCHITECTURE
