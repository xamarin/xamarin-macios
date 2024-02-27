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
    #if DOTNET
      #define PRODUCT    "Microsoft.watchOS"
    #else
      #define PRODUCT    "Xamarin.WatchOS"
    #endif
  #elif TARGET_OS_MACCATALYST
    #if DOTNET
      #define PRODUCT    "Microsoft.MacCatalyst"
    #else
      #define PRODUCT    "Xamarin.MacCatalyst"
    #endif
  #elif TARGET_OS_IOS
    #if DOTNET
      #define PRODUCT                  "Microsoft.iOS"
    #else
      #define PRODUCT                  "Xamarin.iOS"
    #endif
  #elif TARGET_OS_TV
    #if DOTNET
      #define PRODUCT                  "Microsoft.tvOS"
    #else
      #define PRODUCT                  "Xamarin.TVOS"
    #endif
  #else
    #error Unknown MONOTOUCH product
  #endif
  #if !DOTNET
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
  #endif
  #if DOTNET
    #define PRODUCT_EXCEPTION_TYPE   "ObjCException"
  #else
    #define PRODUCT_EXCEPTION_TYPE   "MonoTouchException"
  #endif
  #ifdef __LP64__
    #define ARCH_SUBDIR ".monotouch-64"
  #else
    #define ARCH_SUBDIR ".monotouch-32"
  #endif
#elif MONOMAC
  #if DOTNET
    #define PRODUCT                  "Microsoft.macOS"
  #else
    #define PRODUCT                  "Xamarin.Mac"
    #define PRODUCT_DUAL_ASSEMBLY    "Xamarin.Mac.dll"
  #endif
  #define PRODUCT_EXCEPTION_TYPE   "ObjCException"
  #define ARCH_SUBDIR        
#else
    #error Either MONOTOUCH or MONOMAC must be defined.
#endif

#if DOTNET
  #define PRODUCT_DUAL_ASSEMBLY PRODUCT ".dll"
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
#define PRODUCT_HASH "@PRODUCT_HASH@"
