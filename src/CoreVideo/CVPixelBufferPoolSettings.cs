// 
// CVPixelBufferPoolSettings.cs: Implements settings for CVPixelBufferPool
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012-2014, Xamarin Inc.
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
using CoreFoundation;
using ObjCRuntime;

#nullable enable

namespace CoreVideo {

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CVPixelBufferPoolSettings : DictionaryContainer {
#if !COREBUILD
		public CVPixelBufferPoolSettings ()
			: base (new NSMutableDictionary ())
		{
		}

		public CVPixelBufferPoolSettings (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>The minimum number of buffers allowed in the pixel buffer pool.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kCVPixelBufferPoolMinimumBufferCountKey value to access the underlying dictionary.</remarks>
		public int? MinimumBufferCount {
			set {
				SetNumberValue (CVPixelBufferPool.MinimumBufferCountKey, value);
			}
			get {
				return GetInt32Value (CVPixelBufferPool.MinimumBufferCountKey);
			}
		}

		/// <summary>The maximum allowable age in seconds for a buffer in the pixel buffer pool.</summary>
		///         <value>Using zero value will disable the age out procedure completely.</value>
		///         <remarks>The property uses constant kCVPixelBufferPoolMaximumBufferAgeKey value to access the underlying dictionary.</remarks>
		public double? MaximumBufferAgeInSeconds {
			set {
				SetNumberValue (CVPixelBufferPool.MaximumBufferAgeKey, value);
			}
			get {
				return GetDoubleValue (CVPixelBufferPool.MaximumBufferAgeKey);
			}
		}
#endif
	}

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public partial class CVPixelBufferPoolAllocationSettings : DictionaryContainer {
#if !COREBUILD
		public CVPixelBufferPoolAllocationSettings ()
			: base (new NSMutableDictionary ())
		{
		}

		public CVPixelBufferPoolAllocationSettings (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>The maximum allowed pixel buffer allocations in the pixel buffer pool.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant kCVPixelBufferPoolAllocationThresholdKey value to access the underlying dictionary.</remarks>
		public int? Threshold {
			set {
				SetNumberValue (ThresholdKey, value);
			}
			get {
				return GetInt32Value (ThresholdKey);
			}
		}
#endif
	}
}
