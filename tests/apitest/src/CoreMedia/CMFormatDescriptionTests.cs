using System;
using System.Net;

#if XAMCORE_2_0
using Foundation;
using CoreFoundation;
using CoreMedia;
using AudioToolbox;
using CoreGraphics;
#else
#if MONOMAC
using MonoMac.CoreFoundation;
using MonoMac.Foundation;
using MonoMac.CoreMedia;
using MonoMac.AudioToolbox;
using MonoMac.CoreGraphics;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.CoreMedia;
using MonoTouch.AudioToolbox;
#endif
#endif
using NUnit.Framework;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class CMFormatDescriptionTests
	{
		[Test]
		public void CMAudioFormatDescriptionTest ()
		{
			var desc = CMAudioFormatDescription.Create (AudioFormatType.Audible);

			Assert.IsNotNull (desc);

			AudioStreamBasicDescription description = new AudioStreamBasicDescription (AudioFormatType.LinearPCM) {
				SampleRate = 44100,
				FormatFlags = AudioFormatFlags.IsSignedInteger | AudioFormatFlags.IsPacked,
				FramesPerPacket = 1,
				ChannelsPerFrame = 2,
				BitsPerChannel = 16,
				BytesPerPacket = 4,
				BytesPerFrame = 4
			};

			var audioDescription = new CMAudioFormatDescription (ref description);

			Assert.IsNotNull (audioDescription);
			Assert.AreEqual (audioDescription.MediaType, CMMediaType.Audio);
			Assert.AreEqual ((AudioFormatType)audioDescription.MediaSubType, AudioFormatType.LinearPCM);

			Assert.IsNotNull (audioDescription.MostCompatibleFormat);
			Assert.IsNotNull (audioDescription.RichestDecodableFormat);
			Assert.AreEqual (audioDescription.Formats.Length, 1);
			Assert.AreEqual (audioDescription.StreamBasicDescription, description);
			Assert.AreEqual (audioDescription.MostCompatibleFormat, audioDescription.Formats [0]);
			Assert.AreEqual (audioDescription.RichestDecodableFormat, audioDescription.Formats [0]);

			Assert.AreEqual (audioDescription.StreamBasicDescription.Value.BitsPerChannel, 16);
			Assert.AreEqual (audioDescription.StreamBasicDescription.Value.BytesPerFrame, 4);
			Assert.AreEqual (audioDescription.StreamBasicDescription.Value.BytesPerPacket, 4);
			Assert.AreEqual (audioDescription.StreamBasicDescription.Value.ChannelsPerFrame, 2);
			Assert.AreEqual (audioDescription.StreamBasicDescription.Value.Format, AudioFormatType.LinearPCM);
			Assert.AreEqual (audioDescription.StreamBasicDescription.Value.FormatName, "Linear PCM, 16 bit little-endian signed integer, 2 channels, 44100 Hz");
			Assert.AreEqual (audioDescription.StreamBasicDescription.Value.FormatFlags, AudioFormatFlags.IsSignedInteger | AudioFormatFlags.IsPacked);
			Assert.AreEqual (audioDescription.StreamBasicDescription.Value.FramesPerPacket, 1);
			Assert.AreEqual (audioDescription.StreamBasicDescription.Value.SampleRate, 44100);
			Assert.IsFalse (audioDescription.StreamBasicDescription.Value.IsEncrypted);
			Assert.IsFalse (audioDescription.StreamBasicDescription.Value.IsVariableBitrate);
			Assert.IsFalse (audioDescription.StreamBasicDescription.Value.IsExternallyFramed);
		}

		[Test]
		public void CMVideoFormatDescriptionTest ()
		{
			CMFormatDescriptionError error;
			var videoDesc = CMFormatDescription.Create (CMMediaType.Video, 61766331, out error);
			var videoDesc2 = CMVideoFormatDescription.Create (CMVideoCodecType.Mpeg4Video, out error);
			var videoDesc3 = CMVideoFormatDescription.Create (CMVideoCodecType.H264);
			var videoDesc4 = new CMVideoFormatDescription (CMVideoCodecType.Cinepak, new CMVideoDimensions (640, 480));

			Assert.IsNotNull (videoDesc as CMVideoFormatDescription);
			Assert.IsNotNull (videoDesc2);
			Assert.IsNotNull (videoDesc3);
			Assert.IsNotNull (videoDesc4);

			Assert.AreEqual (videoDesc2.Dimensions, new CMVideoDimensions (0, 0));
			Assert.AreEqual (videoDesc4.Dimensions, new CMVideoDimensions (640, 480));

			Assert.AreEqual (videoDesc2.GetCleanAperture (true), new CGRect (0, 0, 0, 0));
			Assert.AreEqual (videoDesc4.GetCleanAperture (true), new CGRect (0, 0, 640, 480));

			Assert.AreEqual (videoDesc4.GetPresentationDimensions (false, false), new CGSize (640, 480));

			Assert.AreEqual (videoDesc2.CodecType, CMVideoCodecType.Mpeg4Video);
		}

		[Test]
		public void CMTimeCodeFormatDescriptionTest ()
		{
			var timeDescription = new CMTimeCodeFormatDescription (CMTimeCodeFormatType.TimeCode32, CMTime.FromSeconds (30, 1), 1, CMTimeCodeFlag.DropFrame | CMTimeCodeFlag.NegTimesOK);

			Assert.IsNotNull (timeDescription);
			Assert.AreEqual (timeDescription.FormatType, CMTimeCodeFormatType.TimeCode32);
			Assert.AreEqual (timeDescription.FrameQuanta, 1);
			Assert.AreEqual (timeDescription.FrameDuration, CMTime.FromSeconds (30, 1));
			Assert.AreEqual (timeDescription.TimeCodeFlags, CMTimeCodeFlag.DropFrame | CMTimeCodeFlag.NegTimesOK);
		}

		[Test]
		public void CMMuxedFormatDescriptionTest ()
		{
			var muxedDescription = new CMMuxedFormatDescription (CMMuxedStreamType.MPEG2Program);
			Assert.IsNotNull (muxedDescription);
			Assert.AreEqual (muxedDescription.StreamType, CMMuxedStreamType.MPEG2Program);
		}

		[Test]
		public void CMMetadataFormatDescriptionTest ()
		{
			var metadataDescription = new CMMetadataFormatDescription (CMMetadataFormatType.ID3);

			Assert.IsNotNull (metadataDescription);
			Assert.AreEqual (metadataDescription.FormatType, CMMetadataFormatType.ID3);
		}
	}
}
