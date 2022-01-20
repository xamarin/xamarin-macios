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

	[TV (9,2)]
	[iOS (9,3)]
	[Obsoleted (PlatformName.TvOS, 10,0, message: "Replaced by 'GColorConversionInfoTriple'.")]
	[Obsoleted (PlatformName.iOS, 10,0, message: "Replaced by 'GColorConversionInfoTriple'.")]
	[StructLayout (LayoutKind.Sequential)]
	public struct CGColorConverterTriple {
		public CGColorSpace Space;
		public CGColorConverterTransformType Transform;
		public CGColorRenderingIntent Intent;
	}

	// CGColorConverter.h
	[TV (9,2)]
	[iOS (9,3)]
	[Obsoleted (PlatformName.TvOS, 10,0, message: "Replaced by 'CGColorConversionInfo'.")]
	[Obsoleted (PlatformName.iOS, 10,0, message: "Replaced by 'CGColorConversionInfo'.")]

	public class CGColorConverter : NativeObject
	{
		public CGColorConverter (NSDictionary options, params CGColorConverterTriple [] triples)
		{
		}
	}
}

#endif // !MONOMAC && !WATCH
