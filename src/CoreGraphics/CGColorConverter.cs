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

using ObjCRuntime;

#if !COREBUILD
using CoreFoundation;
using Foundation;
#endif

namespace CoreGraphics {

#if !NET
	[Obsoleted (PlatformName.TvOS, 10, 0, message: "Replaced by 'CGColorConversionInfoTriple'.")]
	[Obsoleted (PlatformName.iOS, 10, 0, message: "Replaced by 'CGColorConversionInfoTriple'.")]
	[StructLayout (LayoutKind.Sequential)]
	public struct CGColorConverterTriple {
		public CGColorSpace Space;
		public CGColorConverterTransformType Transform;
		public CGColorRenderingIntent Intent;
	}
#endif // !NET

	// CGColorConverter.h
#if !NET
	[Obsoleted (PlatformName.TvOS, 10, 0, message: "Replaced by 'CGColorConversionInfo'.")]
	[Obsoleted (PlatformName.iOS, 10, 0, message: "Replaced by 'CGColorConversionInfo'.")]
	public class CGColorConverter : NativeObject {
		public CGColorConverter (NSDictionary options, params CGColorConverterTriple [] triples)
		{
		}
	}
#endif // !NET
}

#endif // !MONOMAC && !WATCH
