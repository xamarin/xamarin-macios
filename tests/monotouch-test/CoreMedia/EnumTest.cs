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
#if XAMCORE_2_0
using Foundation;
using CoreMedia;
#else
using MonoTouch.CoreMedia;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMedia {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EnumTest {

		string FourCC (int value)
		{
			return new string (new char [] { 
				(char) (byte) (value >> 24), 
				(char) (byte) (value >> 16), 
				(char) (byte) (value >> 8), 
				(char) (byte) value });
		}

		[Test]
		public void MediaType ()
		{
			Assert.That (FourCC ((int) CMMediaType.Audio), Is.EqualTo ("soun"), "Audio");
			Assert.That (FourCC ((int) CMMediaType.ClosedCaption), Is.EqualTo ("clcp"), "ClosedCaption");
			Assert.That (FourCC ((int) CMMediaType.Metadata), Is.EqualTo ("meta"), "Metadata");
			Assert.That (FourCC ((int) CMMediaType.Muxed), Is.EqualTo ("muxx"), "Muxed");
			Assert.That (FourCC ((int) CMMediaType.Subtitle), Is.EqualTo ("sbtl"), "Subtitle");
			Assert.That (FourCC ((int) CMMediaType.Text), Is.EqualTo ("text"), "Text");
			Assert.That (FourCC ((int) CMMediaType.TimeCode), Is.EqualTo ("tmcd"), "TimeCode");
#if !XAMCORE_2_0
			Assert.That (FourCC ((int) CMMediaType.TimedMetadata), Is.EqualTo ("tmet"), "TimedMetadata");
#endif
			Assert.That (FourCC ((int) CMMediaType.Video), Is.EqualTo ("vide"), "Video");
		}

		[Test]
		public void ClosedCaptionFormatType ()
		{
			Assert.That (FourCC ((int) CMClosedCaptionFormatType.ATSC), Is.EqualTo ("atcc"), "ATSC");
			Assert.That (FourCC ((int) CMClosedCaptionFormatType.CEA608), Is.EqualTo ("c608"), "CEA608");
			Assert.That (FourCC ((int) CMClosedCaptionFormatType.CEA708), Is.EqualTo ("c708"), "CEA708");
		}

		[Test]
		public void MuxedStreamType ()
		{
			Assert.That (FourCC ((int) CMMuxedStreamType.DV), Is.EqualTo ("dv  "), "DV");
			Assert.That (FourCC ((int) CMMuxedStreamType.MPEG1System), Is.EqualTo ("mp1s"), "MPEG1System");
			Assert.That (FourCC ((int) CMMuxedStreamType.MPEG2Program), Is.EqualTo ("mp2p"), "MPEG2Program");
			Assert.That (FourCC ((int) CMMuxedStreamType.MPEG2Transport), Is.EqualTo ("mp2t"), "MPEG2Transport");
		}

		[Test]
		public void SubtitleFormatType ()
		{
			Assert.That (FourCC ((int) CMSubtitleFormatType.Text3G), Is.EqualTo ("tx3g"), "Text3G");
			Assert.That (FourCC ((int) CMSubtitleFormatType.WebVTT), Is.EqualTo ("wvtt"), "WebVTT");
		}

		[Test]
		public void MetadataFormatType ()
		{
			Assert.That (FourCC ((int) CMMetadataFormatType.Boxed), Is.EqualTo ("mebx"), "Boxed");
			Assert.That (FourCC ((int) CMMetadataFormatType.ICY), Is.EqualTo ("icy "), "ICY");
			Assert.That (FourCC ((int) CMMetadataFormatType.ID3), Is.EqualTo ("id3 "), "ID3");
			Assert.That (FourCC ((int) CMMetadataFormatType.Emsg), Is.EqualTo ("emsg"), "EMSG");
		}

		[Test]
		public void TimeCodeFormatType ()
		{
			Assert.That (FourCC ((int) CMTimeCodeFormatType.Counter32), Is.EqualTo ("cn32"), "Counter32");
			Assert.That (FourCC ((int) CMTimeCodeFormatType.Counter64), Is.EqualTo ("cn64"), "Counter64");
			Assert.That (FourCC ((int) CMTimeCodeFormatType.TimeCode32), Is.EqualTo ("tmcd"), "TimeCode32");
			Assert.That (FourCC ((int) CMTimeCodeFormatType.TimeCode64), Is.EqualTo ("tc64"), "TimeCode64");
		}

		[Test]
		public void VideoCodecType ()
		{
			Assert.That (FourCC ((int) CMVideoCodecType.Animation), Is.EqualTo ("rle "), "Animation");
			Assert.That (FourCC ((int) CMVideoCodecType.AppleProRes422), Is.EqualTo ("apcn"), "AppleProRes422");
			Assert.That (FourCC ((int) CMVideoCodecType.AppleProRes422HQ), Is.EqualTo ("apch"), "AppleProRes422HQ");
			Assert.That (FourCC ((int) CMVideoCodecType.AppleProRes422LT), Is.EqualTo ("apcs"), "AppleProRes422LT");
			Assert.That (FourCC ((int) CMVideoCodecType.AppleProRes422Proxy), Is.EqualTo ("apco"), "AppleProRes422Proxy");
			Assert.That (FourCC ((int) CMVideoCodecType.AppleProRes4444), Is.EqualTo ("ap4h"), "AppleProRes4444");
			Assert.That (FourCC ((int) CMVideoCodecType.Cinepak), Is.EqualTo ("cvid"), "Cinepak");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcNtsc), Is.EqualTo ("dvc "), "DvcNtsc");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcPal), Is.EqualTo ("dvcp"), "DvcPal");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcPro50NTSC), Is.EqualTo ("dv5n"), "DvcPro50NTSC");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcPro50PAL), Is.EqualTo ("dv5p"), "DvcPro50PAL");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcProHD1080i50), Is.EqualTo ("dvh5"), "DvcProHD1080i50");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcProHD1080i60), Is.EqualTo ("dvh6"), "DvcProHD1080i60");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcProHD1080p25), Is.EqualTo ("dvh2"), "DvcProHD1080p25");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcProHD1080p30), Is.EqualTo ("dvh3"), "DvcProHD1080p30");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcProHD720p50), Is.EqualTo ("dvhq"), "DvcProHD720p50");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcProHD720p60), Is.EqualTo ("dvhp"), "DvcProHD720p60");
			Assert.That (FourCC ((int) CMVideoCodecType.DvcProPal), Is.EqualTo ("dvpp"), "DvcProPal");
			Assert.That (FourCC ((int) CMVideoCodecType.H263), Is.EqualTo ("h263"), "H263");
			Assert.That (FourCC ((int) CMVideoCodecType.H264), Is.EqualTo ("avc1"), "H264");
			Assert.That (FourCC ((int) CMVideoCodecType.JPEG), Is.EqualTo ("jpeg"), "JPEG");
			Assert.That (FourCC ((int) CMVideoCodecType.JPEG_OpenDML), Is.EqualTo ("dmb1"), "JPEG_OpenDML");
			Assert.That (FourCC ((int) CMVideoCodecType.Mpeg1Video), Is.EqualTo ("mp1v"), "Mpeg1Video");
			Assert.That (FourCC ((int) CMVideoCodecType.Mpeg2Video), Is.EqualTo ("mp2v"), "Mpeg2Video");
			Assert.That (FourCC ((int) CMVideoCodecType.Mpeg4Video), Is.EqualTo ("mp4v"), "Mpeg4Video");
			Assert.That (FourCC ((int) CMVideoCodecType.SorensonVideo), Is.EqualTo ("SVQ1"), "SorensonVideo");
			Assert.That (FourCC ((int) CMVideoCodecType.SorensonVideo3), Is.EqualTo ("SVQ3"), "SorensonVideo3");
			Assert.That (FourCC ((int) CMVideoCodecType.YUV422YpCbCr8), Is.EqualTo ("2vuy"), "YUV422YpCbCr8");
		}

		[Test]
		public void PixelFormat ()
		{
			Assert.That ((int) CMPixelFormat.AlphaRedGreenBlue32bits, Is.EqualTo (32), "AlphaRedGreenBlue32bits");
			Assert.That (FourCC ((int) CMPixelFormat.BlueGreenRedAlpha32bits), Is.EqualTo ("BGRA"), "BlueGreenRedAlpha32bits");
			Assert.That ((int) CMPixelFormat.RedGreenBlue24bits, Is.EqualTo (24), "RedGreenBlue24bits");
			Assert.That ((int) CMPixelFormat.BigEndian555_16bits, Is.EqualTo (16), "BigEndian555_16bits");
			Assert.That (FourCC ((int) CMPixelFormat.BigEndian565_16bits), Is.EqualTo ("B565"), "BigEndian565_16bits");
			Assert.That (FourCC ((int) CMPixelFormat.LittleEndian555_16bits), Is.EqualTo ("L555"), "LittleEndian555_16bits");
			Assert.That (FourCC ((int) CMPixelFormat.LittleEndian565_16bits), Is.EqualTo ("L565"), "LittleEndian565_16bits");
			Assert.That (FourCC ((int) CMPixelFormat.LittleEndian5551_16bits), Is.EqualTo ("5551"), "LittleEndian5551_16bits");
			Assert.That (FourCC ((int) CMPixelFormat.YpCbCr422_8bits), Is.EqualTo ("2vuy"), "YpCbCr422_8bits");
			Assert.That (FourCC ((int) CMPixelFormat.YpCbCr422yuvs_8bits), Is.EqualTo ("yuvs"), "YpCbCr422yuvs_8bits");
			Assert.That (FourCC ((int) CMPixelFormat.YpCbCr444_8bits), Is.EqualTo ("v308"), "YpCbCr444_8bits");
			Assert.That (FourCC ((int) CMPixelFormat.YpCbCrA4444_8bits), Is.EqualTo ("v408"), "YpCbCrA4444_8bits");
			Assert.That (FourCC ((int) CMPixelFormat.YpCbCr422_16bits), Is.EqualTo ("v216"), "YpCbCr422_16bits");
			Assert.That (FourCC ((int) CMPixelFormat.YpCbCr422_10bits), Is.EqualTo ("v210"), "YpCbCr422_10bits");
			Assert.That (FourCC ((int) CMPixelFormat.YpCbCr444_10bits), Is.EqualTo ("v410"), "YpCbCr444_10bits");
			Assert.That ((int) CMPixelFormat.IndexedGrayWhiteIsZero_8bits, Is.EqualTo (40), "YpCbCr444_10bits");
		}
	}
}

#endif // !__WATCHOS__
