//
// CGColorConverter.cs: Implements the managed CGColorConverter
//
// Author: Vincent Dondain
//
// Copyright 2016 Xamarin Inc.
//

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
	[TV (9,2)]
	[iOS (9,3)]
#endif
	[Obsoleted (PlatformName.TvOS, 10,0, message: "Replaced by 'GColorConversionInfoTriple'.")]
	[Obsoleted (PlatformName.iOS, 10,0, message: "Replaced by 'GColorConversionInfoTriple'.")]
	[StructLayout (LayoutKind.Sequential)]
	public struct CGColorConverterTriple {
		public CGColorSpace Space;
		public CGColorConverterTransformType Transform;
		public CGColorRenderingIntent Intent;
	}

	// CGColorConverter.h
#if !NET
	[TV (9,2)]
	[iOS (9,3)]
#endif
	[Obsoleted (PlatformName.TvOS, 10,0, message: "Replaced by 'CGColorConversionInfo'.")]
	[Obsoleted (PlatformName.iOS, 10,0, message: "Replaced by 'CGColorConversionInfo'.")]
	public class CGColorConverter : INativeObject, IDisposable
	{
		/* invoked by marshallers */
		internal CGColorConverter (IntPtr handle)
		{
		}

		[Preserve (Conditional=true)]
		internal CGColorConverter (IntPtr handle, bool owns)
		{
		}

		public CGColorConverter (NSDictionary options, params CGColorConverterTriple [] triples)
		{
		}

		~CGColorConverter ()
		{
		}

		public void Dispose ()
		{
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return IntPtr.Zero; }
		}

		protected virtual void Dispose (bool disposing)
		{
		}
	}
}

#endif // !MONOMAC && !WATCH
