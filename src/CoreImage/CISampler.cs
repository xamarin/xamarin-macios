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

using CoreGraphics;
using CoreFoundation;

#nullable enable

namespace CoreImage {

	// convenience enum on kCISamplerWrap[Black|Clamp] fields -> CISampler.h (headers hidden under QuartzCore.framework)
	/// <summary>Enumerates values that control how samples from outside the source image are treated.</summary>
	///     <remarks>To be added.</remarks>
	public enum CIWrapMode {
		/// <summary>Areas outside the source image are treated as black.</summary>
		Black,
		/// <summary>Areas outside the source image are clamped to the value at the edge.</summary>
		Clamp,
	}

	// convenience enum on kCISamplerFilter[Nearest|Linear] fields -> CISampler.h (headers hidden under QuartzCore.framework)
	/// <summary>Enumerates filter modes.</summary>
	///     <remarks>To be added.</remarks>
	public enum CIFilterMode {
		/// <summary>Use the value of the nearest pixel.</summary>
		Nearest,
		/// <summary>Linearly interpolate a sample value from the neighboring pixels.</summary>
		Linear,
	}

	/// <summary>Options to control sampler operations for <see cref="CoreImage.CISampler" /> objects.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CISamplerOptions {
		/// <summary>Creates a new default sampler options argument.</summary>
		///         <remarks>To be added.</remarks>
		public CISamplerOptions () { }

		/// <summary>Gets or sets the matrix to use for affine transformations.</summary>
		///         <value>The matrix to use for affine transformations.</value>
		///         <remarks>To be added.</remarks>
		public CGAffineTransform? AffineMatrix { get; set; }
		/// <summary>Gets or sets the wrapping mode, which controls whether pixels outside of the source image boundary will be clamped to the edge value or be black.</summary>
		///         <value>The wrapping mode, which controls whether pixels outside of the source image boundary will be clamped to the edge value or be black.</value>
		///         <remarks>To be added.</remarks>
		public CIWrapMode? WrapMode { get; set; }
		/// <summary>Gets or sets the filter mode, which controls whether to do the nearest value or to linearly interpolate the value from the surrounding pixels.</summary>
		///         <value>The filter mode, which controls whether to do the nearest value or to linearly interpolate the value from the surrounding pixels.</value>
		///         <remarks>To be added.</remarks>
		public CIFilterMode? FilterMode { get; set; }
		/// <summary>Gets or sets the color space to which samples are converted before being passed to kernels.</summary>
		///         <value>The color space to which samples are converted before being passed to kernels.</value>
		///         <remarks>If the developer does not set this value, then samples are processed in the working color space of the rendering context.</remarks>
		public CGColorSpace? ColorSpace { get; set; }

		internal NSDictionary ToDictionary ()
		{
			var ret = new NSMutableDictionary ();

			if (AffineMatrix.HasValue) {
				var a = AffineMatrix.Value;
				using (var array = NSArray.FromObjects (a.A, a.B, a.C, a.D, a.Tx, a.Ty))
					ret.SetObject (array, CISampler.AffineMatrix);
			}
			if (WrapMode.HasValue) {
				var k = WrapMode.Value == CIWrapMode.Black ? CISampler.WrapBlack : CISampler.FilterNearest;
				ret.SetObject (k, CISampler.WrapMode);
			}
			if (FilterMode.HasValue) {
				var k = FilterMode.Value == CIFilterMode.Nearest ? CISampler.FilterNearest : CISampler.FilterLinear;
				ret.SetObject (k, CISampler.FilterMode);
			}
			if (ColorSpace is not null)
				ret.LowlevelSetObject (ColorSpace, CISampler.ColorSpace);
			return ret;
		}
	}

	public partial class CISampler {
		/// <param name="sourceImage">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>Creates a new <see cref="CoreImage.CISampler" /> from the <paramref name="sourceImage" /> with <paramref name="options" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CISampler FromImage (CIImage sourceImage, CISamplerOptions? options)
		{
			if (options is null)
				return FromImage (sourceImage);
			return FromImage (sourceImage, options.ToDictionary ());
		}

		/// <param name="sourceImage">The image from which to sample.</param>
		///         <param name="options">Options that specify transform matrices, wrapping and filtering modes, and the color space.</param>
		///         <summary>Creates a new sampler from a source image and a set of options.</summary>
		///         <remarks>To be added.</remarks>
		[DesignatedInitializer]
		public CISampler (CIImage sourceImage, CISamplerOptions? options) : this (sourceImage, options?.ToDictionary ())
		{
		}
	}
}
