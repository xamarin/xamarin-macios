//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011-2015 Xamarin Inc
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

namespace CoreImage {

	// convenience enum on kCISamplerWrap[Black|Clamp] fields -> CISampler.h (headers hidden under QuartzCore.framework)
	public enum CIWrapMode {
		Black,
		Clamp
	}

	// convenience enum on kCISamplerFilter[Nearest|Linear] fields -> CISampler.h (headers hidden under QuartzCore.framework)
	public enum CIFilterMode {
		Nearest, Linear
	}
	
	public class CISamplerOptions {
		public CISamplerOptions () {}

		public CGAffineTransform? AffineMatrix { get; set; }
		public CIWrapMode? WrapMode { get; set; }
		public CIFilterMode? FilterMode { get; set; }
		public CGColorSpace ColorSpace { get; set; }
		
		internal NSDictionary ToDictionary ()
		{
			var ret = new NSMutableDictionary ();

			if (AffineMatrix.HasValue){
				var a = AffineMatrix.Value;
				using (var array = NSArray.FromObjects (a.xx, a.yx, a.xy, a.yy, a.x0, a.y0))
					ret.SetObject (array, CISampler.AffineMatrix);
			}
			if (WrapMode.HasValue){
				var k = WrapMode.Value == CIWrapMode.Black ? CISampler.WrapBlack : CISampler.FilterNearest;
				ret.SetObject (k, CISampler.WrapMode);
			}
			if (FilterMode.HasValue){
				var k = FilterMode.Value == CIFilterMode.Nearest ? CISampler.FilterNearest : CISampler.FilterLinear;
				ret.SetObject (k, CISampler.FilterMode);
			}
			if (ColorSpace != null)
				ret.LowlevelSetObject (ColorSpace.Handle, CISampler.ColorSpace.Handle);
			return ret;
		}
	}
	
	public partial class CISampler {
#if !XAMCORE_3_0 && MONOMAC
		[Obsolete ("This default constructor does not provide a valid instance")]
		public CISampler () {}
#endif
		public CISampler FromImage (CIImage sourceImage, CISamplerOptions options)
		{
			if (options == null)
				return FromImage (sourceImage);
			return FromImage (sourceImage, options.ToDictionary ());
		}

		[DesignatedInitializer]
		public CISampler (CIImage sourceImage, CISamplerOptions options) : this (sourceImage, options == null ? null : options.ToDictionary ())
		{
		}
	}
}
