//
// Copyright 2011-2014, Xamarin Inc.
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
using ObjCRuntime;

#nullable enable

namespace CoreImage {

	// convenience enum for values used with kCGImagePropertyOrientation (key) as NSNumber
	// values are part of the header file (CGImageProperties.h) as comments (not constants or fields)
	public enum CIImageOrientation {
		TopLeft = 1,
		TopRight = 2,
		BottomRight = 3,
		BottomLeft = 4,
		LeftTop = 5,
		RightTop = 6,
		RightBottom = 7,
		LeftBottom = 8
	}

	// convenience enum (fields are used) but also a `typedef int` -> CIImage.h
	public enum CIFormat {
		ARGB8 = 0,
		RGBAh = 1,
#if MONOMAC
		RGBA16 = 2,
		[Obsolete ("This value can not be shared across Mac/iOS binaries, future proof with kRGBAf instead.")]
		RGBAf  = 3,

		// Please, do not add values into MonoMac/iOS without adding an explicit value
#elif !XAMCORE_3_0
		[Obsolete ("This value can not be shared across Mac/iOS binaries, future proof with kBGRA8 instead.")]
		BGRA8 = 2,
		[Obsolete ("This value can not be shared across Mac/iOS binaries, future proof with kRGBA8 instead.")]
		RGBA8 = 3,
		// Please, do not add values into MonoMac/iOS without adding an explicit value
#endif
		kRGBAf = 4,
		kBGRA8 = 5,
		kRGBA8 = 6,
		ABGR8 = 7,
		A8 = 11,
		A16 = 12,
		Ah = 13,
		Af = 14,
		R8 = 15,
		R16 = 16,
		Rh = 17,
		Rf = 18,
		RG8 = 19,
		RG16 = 20,
		RGh = 21,
		RGf = 22
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum CIQRCodeErrorCorrectionLevel : long {
		L = 76,
		M = 77,
		Q = 81,
		H = 72,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum CIDataMatrixCodeEccVersion : long {
		V000 = 0,
		V050 = 50,
		V080 = 80,
		V100 = 100,
		V140 = 140,
		V200 = 200,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum CIRenderDestinationAlphaMode : ulong {
		None = 0,
		Premultiplied = 1,
		Unpremultiplied = 2,
	}
}
