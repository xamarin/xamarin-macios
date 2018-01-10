// 
// CVPixelFormatType.cs
//
// Authors: Mono Team
//     
// Copyright 2011 Novell, Inc
// Copyright 2011-2014, 2016 Xamarin Inc
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

namespace CoreVideo {

	// Note: CoreVideo is not supported in watchOS except for this enum
	// for which ObjC API uses `int` instead of the enum
	
	// untyped enum, some are 4CC -> CVPixelBuffer.h
	[Watch (3,0)]
	public enum CVPixelFormatType : uint {
		// FIXME: These all start with integers; what should we do here?
		CV1Monochrome    = 0x00000001,
		CV2Indexed       = 0x00000002,
		CV4Indexed       = 0x00000004,
		CV8Indexed       = 0x00000008,
		CV1IndexedGray_WhiteIsZero = 0x00000021,
		CV2IndexedGray_WhiteIsZero = 0x00000022,
		CV4IndexedGray_WhiteIsZero = 0x00000024,
		CV8IndexedGray_WhiteIsZero = 0x00000028,
		CV16BE555        = 0x00000010,
		CV24RGB          = 0x00000018,
		CV32ARGB         = 0x00000020,
		CV16LE555        = 0x4c353535,
		CV16LE5551       = 0x35353531,
		CV16BE565        = 0x42353635,
		CV16LE565        = 0x4c353635,
		CV24BGR          = 0x32344247,
		CV32BGRA         = 0x42475241,
		CV32ABGR         = 0x41424752,
		CV32RGBA         = 0x52474241,
		CV64ARGB         = 0x62363461,
		CV48RGB          = 0x62343872,
		CV32AlphaGray    = 0x62333261,
		CV16Gray         = 0x62313667,
		CV422YpCbCr8     = 0x32767579,
		CV4444YpCbCrA8   = 0x76343038,
		CV4444YpCbCrA8R  = 0x72343038,
		CV444YpCbCr8     = 0x76333038,
		CV422YpCbCr16    = 0x76323136,
		CV422YpCbCr10    = 0x76323130,
		CV444YpCbCr10    = 0x76343130,
		CV420YpCbCr8Planar = 0x79343230,
		CV420YpCbCr8PlanarFullRange    = 0x66343230,
		CV422YpCbCr_4A_8BiPlanar = 0x61327679,
		CV420YpCbCr8BiPlanarVideoRange = 0x34323076,
		CV420YpCbCr8BiPlanarFullRange  = 0x34323066,
		CV422YpCbCr8_yuvs = 0x79757673,
		CV422YpCbCr8FullRange = 0x79757666,
		CV30RGB = 0x5231306b,
		CV4444AYpCbCr8   = 0x79343038,
		CV4444AYpCbCr16  = 0x79343136,
		// Since 5.1
		OneComponent8 = 0x4C303038,
		TwoComponent8 = 0x32433038,
		// Since 6.0
		OneComponent16Half	= 0x4C303068, // 'L00h'
 		OneComponent32Float = 0x4C303066, // 'L00f'
		TwoComponent16Half  = 0x32433068, // '2C0h'
		TwoComponent32Float = 0x32433066, // '2C0f'
		CV64RGBAHalf		= 0x52476841, // 'RGhA'
		CV128RGBAFloat		= 0x52476641, // 'RGfA'
		// iOS 10
		CV30RgbLePackedWideGamut = 0x77333072, // 'w30r'
		CV14BayerGrbg = 0x67726234, // 'grb4',
		CV14BayerRggb = 0x72676734, // 'rgg4',
		CV14BayerBggr = 0x62676734, // 'bgg4',
		CV14BayerGbrg = 0x67627234, // 'gbr4',
		// iOS 10.3
		Argb2101010LEPacked = 0x6C313072, // 'l10r'
		// iOS 11.0
		DisparityFloat16 = 0x68646973, // hdis
		DisparityFloat32 = 0x66646973, // fdis
		DepthFloat16 = 0x68646570, // hdep
		DepthFloat32 = 0x66646570, // fdep
		CV420YpCbCr10BiPlanarVideoRange = 0x78343230, // x420
		CV422YpCbCr10BiPlanarVideoRange = 0x78343232, // x422
		CV444YpCbCr10BiPlanarVideoRange = 0x78343434, // x444
		CV420YpCbCr10BiPlanarFullRange = 0x78663230, // xf20
		CV422YpCbCr10BiPlanarFullRange = 0x78663232, // xf22
		CV444YpCbCr10BiPlanarFullRange = 0x78663434, // xf44
	}
}
