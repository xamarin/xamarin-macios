//
// CGColorConverter.cs: Implements the managed CGColorConverter
//
// Author: Vincent Dondain
//
// Copyright 2016 Xamarin Inc.
//

#nullable enable

#if !MONOMAC && !WATCH

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;

#if !COREBUILD
using CoreFoundation;
using Foundation;
#endif

namespace CoreGraphics {

#if NET
	[SupportedOSPlatform ("tvos9.2")]
	[SupportedOSPlatform ("ios9.3")]
#if TVOS
	[Obsolete ("Starting with tvos10.0 replaced by 'CGColorConversionInfoTriple'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios10.0 replaced by 'CGColorConversionInfoTriple'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[TV (9,2)]
	[iOS (9,3)]
	[Obsoleted (PlatformName.TvOS, 10,0, message: "Replaced by 'CGColorConversionInfoTriple'.")]
	[Obsoleted (PlatformName.iOS, 10,0, message: "Replaced by 'CGColorConversionInfoTriple'.")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CGColorConverterTriple {
		public CGColorSpace Space;
		public CGColorConverterTransformType Transform;
		public CGColorRenderingIntent Intent;
	}

	// CGColorConverter.h
#if NET
	[SupportedOSPlatform ("tvos9.2")]
	[SupportedOSPlatform ("ios9.3")]
#if TVOS
	[Obsolete ("Starting with tvos10.0 replaced by 'CGColorConversionInfo'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios10.0 replaced by 'CGColorConversionInfo'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[TV (9,2)]
	[iOS (9,3)]
	[Obsoleted (PlatformName.TvOS, 10,0, message: "Replaced by 'CGColorConversionInfo'.")]
	[Obsoleted (PlatformName.iOS, 10,0, message: "Replaced by 'CGColorConversionInfo'.")]
#endif

	public class CGColorConverter : NativeObject
	{
		public CGColorConverter (NSDictionary options, params CGColorConverterTriple [] triples)
		{
		}
	}
}

#endif // !MONOMAC && !WATCH
