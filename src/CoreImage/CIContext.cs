//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011, 2012, 2015 Xamarin Inc.
// Copyright 2010, Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;

using Foundation;
using CoreGraphics;
using CoreFoundation;
using ObjCRuntime;
#if !MONOMAC
using Metal;
#endif
#if HAS_OPENGLES
using OpenGLES;
#endif

#nullable enable

namespace CoreImage {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CIContextOptions : DictionaryContainer {

		public CIContextOptions ()
		{
		}

		public CIContextOptions (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public CGColorSpace? OutputColorSpace {
			get {
				return GetNativeValue<CGColorSpace> (CIContext.OutputColorSpace);
			}
			set {
				SetNativeValue (CIContext.OutputColorSpace, value);
			}
		}

		public CGColorSpace? WorkingColorSpace {
			get {
				return GetNativeValue<CGColorSpace> (CIContext._WorkingColorSpace);
			}
			set {
				SetNativeValue (CIContext._WorkingColorSpace, value);
			}
		}

		public bool UseSoftwareRenderer {
			get {
				var b = GetBoolValue (CIContext.UseSoftwareRenderer);
				return b.HasValue ? b.Value : false;
			}
			set {
				SetBooleanValue (CIContext.UseSoftwareRenderer, value);
			}
		}

		public int? CIImageFormat {
			get {
				return GetInt32Value (CIContext.WorkingFormatField);
			}
			set {
				SetNumberValue (CIContext.WorkingFormatField, value);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public bool? PriorityRequestLow {
			get {
				return GetBoolValue (CIContext.PriorityRequestLow);
			}
			set {
				SetBooleanValue (CIContext.PriorityRequestLow, value);
			}
		}

		public bool? HighQualityDownsample {
			get {
				return GetBoolValue (CIContext.HighQualityDownsample);
			}
			set {
				SetBooleanValue (CIContext.HighQualityDownsample, value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public bool? OutputPremultiplied {
			get {
				return GetBoolValue (CIContext.OutputPremultiplied);
			}
			set {
				SetBooleanValue (CIContext.OutputPremultiplied, value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public bool? CacheIntermediates {
			get {
				return GetBoolValue (CIContext.CacheIntermediates);
			}
			set {
				SetBooleanValue (CIContext.CacheIntermediates, value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
#endif
		public bool? AllowLowPower {
			get {
				return GetBoolValue (CIContext.AllowLowPower);
			}
			set {
				SetBooleanValue (CIContext.AllowLowPower, value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (14, 0)]
		[TV (14, 0)]
#endif
		public string? Name {
			get {
				return GetStringValue (CIContext.Name);
			}
			set {
				SetStringValue (CIContext.Name, value);
			}
		}
	}

	public partial class CIContext {

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public CIContext (CIContextOptions options) :
			this (options?.Dictionary)
		{
		}

		public static CIContext FromContext (CGContext ctx, CIContextOptions? options)
		{
			return FromContext (ctx, options?.Dictionary);
		}

		public static CIContext FromContext (CGContext ctx)
		{
			return FromContext (ctx, (NSDictionary?) null);
		}

#if HAS_OPENGLES
		public static CIContext FromContext (EAGLContext eaglContext, CIContextOptions? options)
		{
			if (options is null)
				return FromContext (eaglContext);

			return FromContext (eaglContext, options.Dictionary);
		}

		public static CIContext FromMetalDevice (IMTLDevice device, CIContextOptions? options)
		{
			if (options is null)
				return FromMetalDevice (device);

			return FromMetalDevice (device, options.Dictionary);
		}
#endif

#if MONOMAC
#if NET
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.11")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 11)]
#endif
		public CGLayer? CreateCGLayer (CGSize size)
		{
			return CreateCGLayer (size, null);
		}
#else
		public static CIContext FromOptions (CIContextOptions? options)
		{
			return FromOptions (options?.Dictionary);
		}

		public CGImage? CreateCGImage (CIImage image, CGRect fromRect, CIFormat ciImageFormat, CGColorSpace? colorSpace)
		{
			return CreateCGImage (image, fromRect, CIImage.CIFormatToInt (ciImageFormat), colorSpace);
		}
#endif
	}
}
