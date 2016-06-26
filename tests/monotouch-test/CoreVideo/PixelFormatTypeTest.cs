//
// Unit tests for PixelFormatType
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using CoreVideo;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreVideo;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreVideo {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PixelFormatTypeTest {
		
		string FourCC (int value)
		{
			return new string (new char [] { 
				(char) (byte) (value >> 24), 
				(char) (byte) (value >> 16), 
				(char) (byte) (value >> 8), 
				(char) (byte) value });
		}
		
		[Test]
		public void FourCC ()
		{
			Assert.That (FourCC ((int) CVPixelFormatType.OneComponent8), Is.EqualTo ("L008"), "OneComponent8");
			Assert.That (FourCC ((int) CVPixelFormatType.TwoComponent8), Is.EqualTo ("2C08"), "TwoComponent8");

			Assert.That (FourCC ((int)CVPixelFormatType.CV30RbgaLePackedWideGamut), Is.EqualTo ("w30r"), "CV30RbgaLePackedWideGamut");
			Assert.That (FourCC ((int)CVPixelFormatType.CV14BayerGrbg), Is.EqualTo ("grb4"), "CV14Bayer_Grbg");
			Assert.That (FourCC ((int)CVPixelFormatType.CV14BayerRggb), Is.EqualTo ("rgg4"), "CV14Bayer_Rggb");
			Assert.That (FourCC ((int)CVPixelFormatType.CV14BayerBggr), Is.EqualTo ("bgg4"), "CV14Bayer_Bggr");
			Assert.That (FourCC ((int)CVPixelFormatType.CV14BayerGbrg), Is.EqualTo ("gbr4"), "CV14Bayer_Gbrg");
		}
	}
}

#endif // !__WATCHOS__
