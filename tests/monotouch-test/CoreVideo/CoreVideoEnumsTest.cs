//
// Unit tests for 4cc-based enums
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;

using CoreVideo;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CoreVideoEnumTest {

		string FourCC (int value)
		{
			return new string (new char [] {
				(char) (byte) (value >> 24),
				(char) (byte) (value >> 16),
				(char) (byte) (value >> 8),
				(char) (byte) value });
		}

		[Test]
		public void PixelFormatType ()
		{
			Assert.That (FourCC ((int) CVPixelFormatType.CV16LE555), Is.EqualTo ("L555"), "CV16LE555");
			Assert.That (FourCC ((int) CVPixelFormatType.CV16LE5551), Is.EqualTo ("5551"), "CV16LE5551");
			Assert.That (FourCC ((int) CVPixelFormatType.CV16BE565), Is.EqualTo ("B565"), "CV16BE565");
			Assert.That (FourCC ((int) CVPixelFormatType.CV16LE565), Is.EqualTo ("L565"), "CV16LE565");
			Assert.That (FourCC ((int) CVPixelFormatType.CV24BGR), Is.EqualTo ("24BG"), "CV24BGR");
			Assert.That (FourCC ((int) CVPixelFormatType.CV32BGRA), Is.EqualTo ("BGRA"), "CV32BGRA");
			Assert.That (FourCC ((int) CVPixelFormatType.CV32ABGR), Is.EqualTo ("ABGR"), "CV32ABGR");
			Assert.That (FourCC ((int) CVPixelFormatType.CV32RGBA), Is.EqualTo ("RGBA"), "CV32RGBA");
			Assert.That (FourCC ((int) CVPixelFormatType.CV64ARGB), Is.EqualTo ("b64a"), "CV64ARGB");
			Assert.That (FourCC ((int) CVPixelFormatType.CV48RGB), Is.EqualTo ("b48r"), "CV48RGB");
			Assert.That (FourCC ((int) CVPixelFormatType.CV32AlphaGray), Is.EqualTo ("b32a"), "CV32AlphaGray");
			Assert.That (FourCC ((int) CVPixelFormatType.CV16Gray), Is.EqualTo ("b16g"), "CV16Gray");
			Assert.That (FourCC ((int) CVPixelFormatType.CV422YpCbCr8), Is.EqualTo ("2vuy"), "CV422YpCbCr8");
			Assert.That (FourCC ((int) CVPixelFormatType.CV4444YpCbCrA8), Is.EqualTo ("v408"), "CV4444YpCbCrA8");
			Assert.That (FourCC ((int) CVPixelFormatType.CV4444YpCbCrA8R), Is.EqualTo ("r408"), "CV4444YpCbCrA8R");
			Assert.That (FourCC ((int) CVPixelFormatType.CV444YpCbCr8), Is.EqualTo ("v308"), "CV444YpCbCr8");
			Assert.That (FourCC ((int) CVPixelFormatType.CV422YpCbCr16), Is.EqualTo ("v216"), "CV422YpCbCr16");
			Assert.That (FourCC ((int) CVPixelFormatType.CV422YpCbCr10), Is.EqualTo ("v210"), "CV422YpCbCr10");
			Assert.That (FourCC ((int) CVPixelFormatType.CV444YpCbCr10), Is.EqualTo ("v410"), "CV444YpCbCr10");
			Assert.That (FourCC ((int) CVPixelFormatType.CV420YpCbCr8Planar), Is.EqualTo ("y420"), "CV420YpCbCr8Planar");
			Assert.That (FourCC ((int) CVPixelFormatType.CV420YpCbCr8PlanarFullRange), Is.EqualTo ("f420"), "CV420YpCbCr8PlanarFullRange");
			Assert.That (FourCC ((int) CVPixelFormatType.CV422YpCbCr_4A_8BiPlanar), Is.EqualTo ("a2vy"), "CV422YpCbCr_4A_8BiPlanar");
			Assert.That (FourCC ((int) CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange), Is.EqualTo ("420v"), "CV420YpCbCr8BiPlanarVideoRange");
			Assert.That (FourCC ((int) CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange), Is.EqualTo ("420f"), "CV420YpCbCr8BiPlanarFullRange");
			Assert.That (FourCC ((int) CVPixelFormatType.CV422YpCbCr8_yuvs), Is.EqualTo ("yuvs"), "CV422YpCbCr8_yuvs");
			Assert.That (FourCC ((int) CVPixelFormatType.CV422YpCbCr8FullRange), Is.EqualTo ("yuvf"), "CV422YpCbCr8FullRange");
			Assert.That (FourCC ((int) CVPixelFormatType.CV30RGB), Is.EqualTo ("R10k"), "CV30RGB");
			Assert.That (FourCC ((int) CVPixelFormatType.CV4444AYpCbCr8), Is.EqualTo ("y408"), "CV4444AYpCbCr8");
			Assert.That (FourCC ((int) CVPixelFormatType.CV4444AYpCbCr16), Is.EqualTo ("y416"), "CV4444AYpCbCr16");
			Assert.That (FourCC ((int) CVPixelFormatType.OneComponent8), Is.EqualTo ("L008"), "OneComponent8");
			Assert.That (FourCC ((int) CVPixelFormatType.TwoComponent8), Is.EqualTo ("2C08"), "TwoComponent8");
			Assert.That (FourCC ((int) CVPixelFormatType.OneComponent16Half), Is.EqualTo ("L00h"), "OneComponent16Half");
			Assert.That (FourCC ((int) CVPixelFormatType.OneComponent32Float), Is.EqualTo ("L00f"), "OneComponent32Float");
			Assert.That (FourCC ((int) CVPixelFormatType.TwoComponent16Half), Is.EqualTo ("2C0h"), "TwoComponent16Half");
			Assert.That (FourCC ((int) CVPixelFormatType.TwoComponent32Float), Is.EqualTo ("2C0f"), "TwoComponent32Float");
			Assert.That (FourCC ((int) CVPixelFormatType.CV64RGBAHalf), Is.EqualTo ("RGhA"), "CV64RGBAHalf");
			Assert.That (FourCC ((int) CVPixelFormatType.CV128RGBAFloat), Is.EqualTo ("RGfA"), "CV128RGBAFloat");

			Assert.That (FourCC ((int) CVPixelFormatType.CV30RgbLePackedWideGamut), Is.EqualTo ("w30r"), "CV30RbgaLePackedWideGamut");
			Assert.That (FourCC ((int) CVPixelFormatType.CV14BayerGrbg), Is.EqualTo ("grb4"), "CV14Bayer_Grbg");
			Assert.That (FourCC ((int) CVPixelFormatType.CV14BayerRggb), Is.EqualTo ("rgg4"), "CV14Bayer_Rggb");
			Assert.That (FourCC ((int) CVPixelFormatType.CV14BayerBggr), Is.EqualTo ("bgg4"), "CV14Bayer_Bggr");
			Assert.That (FourCC ((int) CVPixelFormatType.CV14BayerGbrg), Is.EqualTo ("gbr4"), "CV14Bayer_Gbrg");
			Assert.That (FourCC ((int) CVPixelFormatType.Argb2101010LEPacked), Is.EqualTo ("l10r"), "Argb2101010LEPacked");
			Assert.That (FourCC ((int) CVPixelFormatType.CV64RgbaLE), Is.EqualTo ("l64r"), "CV64RgbaLE");
		}
	}
}

#endif // !__WATCHOS__
