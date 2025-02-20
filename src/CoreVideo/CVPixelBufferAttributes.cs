// 
// CVPixelBufferAttributes.cs: Implements strongly typed access for Pixel Buffer attributes
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012, Xamarin Inc.
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
	public class CVPixelBufferAttributes : DictionaryContainer {
#if !COREBUILD
		public CVPixelBufferAttributes ()
			: base (new NSMutableDictionary ())
		{
		}

		public CVPixelBufferAttributes (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public CVPixelBufferAttributes (CVPixelFormatType pixelFormatType, nint width, nint height)
			: this ()
		{
			PixelFormatType = pixelFormatType;
			Width = width;
			Height = height;
		}

		public CVPixelFormatType? PixelFormatType {
			set {
				SetNumberValue (CVPixelBuffer.PixelFormatTypeKey, (uint?) value);
			}
			get {
				return (CVPixelFormatType?) GetUIntValue (CVPixelBuffer.PixelFormatTypeKey);
			}
		}

		public CFAllocator? MemoryAllocator {
			get {
				return GetNativeValue<CFAllocator> (CVPixelBuffer.MemoryAllocatorKey);
			}
			set {
				SetNativeValue (CVPixelBuffer.MemoryAllocatorKey, value);
			}
		}

		public nint? Width {
			set {
				SetNumberValue (CVPixelBuffer.WidthKey, value);
			}
			get {
				return GetInt32Value (CVPixelBuffer.WidthKey);
			}
		}

		public nint? Height {
			set {
				SetNumberValue (CVPixelBuffer.HeightKey, value);
			}
			get {
				return GetInt32Value (CVPixelBuffer.HeightKey);
			}
		}

		public int? ExtendedPixelsLeft {
			set {
				SetNumberValue (CVPixelBuffer.ExtendedPixelsLeftKey, value);
			}
			get {
				return GetInt32Value (CVPixelBuffer.ExtendedPixelsLeftKey);
			}
		}

		public int? ExtendedPixelsTop {
			set {
				SetNumberValue (CVPixelBuffer.ExtendedPixelsTopKey, value);
			}
			get {
				return GetInt32Value (CVPixelBuffer.ExtendedPixelsTopKey);
			}
		}

		public int? ExtendedPixelsRight {
			set {
				SetNumberValue (CVPixelBuffer.ExtendedPixelsRightKey, value);
			}
			get {
				return GetInt32Value (CVPixelBuffer.ExtendedPixelsRightKey);
			}
		}

		public int? ExtendedPixelsBottom {
			set {
				SetNumberValue (CVPixelBuffer.ExtendedPixelsBottomKey, value);
			}
			get {
				return GetInt32Value (CVPixelBuffer.ExtendedPixelsBottomKey);
			}
		}

		public int? BytesPerRowAlignment {
			set {
				SetNumberValue (CVPixelBuffer.BytesPerRowAlignmentKey, value);
			}
			get {
				return GetInt32Value (CVPixelBuffer.BytesPerRowAlignmentKey);
			}
		}

		public bool? CGBitmapContextCompatibility {
			set {
				SetBooleanValue (CVPixelBuffer.CGBitmapContextCompatibilityKey, value);
			}
			get {
				return GetBoolValue (CVPixelBuffer.CGBitmapContextCompatibilityKey);
			}
		}

		public bool? CGImageCompatibility {
			set {
				SetBooleanValue (CVPixelBuffer.CGImageCompatibilityKey, value);
			}
			get {
				return GetBoolValue (CVPixelBuffer.CGImageCompatibilityKey);
			}
		}

		public bool? OpenGLCompatibility {
			set {
				SetBooleanValue (CVPixelBuffer.OpenGLCompatibilityKey, value);
			}
			get {
				return GetBoolValue (CVPixelBuffer.OpenGLCompatibilityKey);
			}
		}

		public int? PlaneAlignment {
			set {
				SetNumberValue (CVPixelBuffer.PlaneAlignmentKey, value);
			}
			get {
				return GetInt32Value (CVPixelBuffer.PlaneAlignmentKey);
			}
		}

		// TODO: kCVPixelBufferIOSurfacePropertiesKey
#if !MONOMAC
		// The presence of the IOSurfacePropertiesKey mandates the allocation via IOSurfaceProperty
		public bool? AllocateWithIOSurface {
			set {
				if (value.HasValue && value.Value)
					SetNativeValue (CVPixelBuffer.IOSurfacePropertiesKey, new NSDictionary ());
				else
					RemoveValue (CVPixelBuffer.IOSurfacePropertiesKey);
			}
			get {
				return GetNSDictionary (CVPixelBuffer.IOSurfacePropertiesKey) is not null;
			}
		}

#if !__MACCATALYST__
		public bool? OpenGLESCompatibility {
			set {
				SetBooleanValue (CVPixelBuffer.OpenGLESCompatibilityKey, value);
			}
			get {
				return GetBoolValue (CVPixelBuffer.OpenGLESCompatibilityKey);
			}
		}
#endif

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		public bool? MetalCompatibility {
			set {
				SetBooleanValue (CVPixelBuffer.MetalCompatibilityKey, value);
			}
			get {
				return GetBoolValue (CVPixelBuffer.MetalCompatibilityKey);
			}
		}
#endif
#endif
	}
}
