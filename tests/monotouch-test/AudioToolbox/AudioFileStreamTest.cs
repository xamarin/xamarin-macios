//
// Unit tests for AudioFileStream
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using Foundation;
using AudioToolbox;
using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioFileStreamTest {

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
			Assert.That (FourCC ((int) AudioFileProperty.AudioDataByteCount), Is.EqualTo ("bcnt"), "AudioDataByteCount");
			Assert.That (FourCC ((int) AudioFileProperty.AudioDataPacketCount), Is.EqualTo ("pcnt"), "AudioDataPacketCount");
			Assert.That (FourCC ((int) AudioFileProperty.BitRate), Is.EqualTo ("brat"), "BitRate");
			Assert.That (FourCC ((int) AudioFileProperty.ByteToPacket), Is.EqualTo ("bypk"), "ByteToPacket");
			Assert.That (FourCC ((int) AudioFileProperty.ChannelLayout), Is.EqualTo ("cmap"), "ChannelLayout");
			Assert.That (FourCC ((int) AudioFileProperty.DataFormat), Is.EqualTo ("dfmt"), "DataFormat");
			Assert.That (FourCC ((int) AudioFileProperty.DataOffset), Is.EqualTo ("doff"), "DataOffset");
			Assert.That (FourCC ((int) AudioFileProperty.FileFormat), Is.EqualTo ("ffmt"), "FileFormat");
			Assert.That (FourCC ((int) AudioFileProperty.FormatList), Is.EqualTo ("flst"), "FormatList");
			Assert.That (FourCC ((int) AudioFileProperty.FrameToPacket), Is.EqualTo ("frpk"), "FrameToPacket");
			Assert.That (FourCC ((int) AudioFileProperty.MagicCookieData), Is.EqualTo ("mgic"), "MagicCookieData");
			Assert.That (FourCC ((int) AudioFileProperty.MaximumPacketSize), Is.EqualTo ("psze"), "MaximumPacketSize");
			Assert.That (FourCC ((int) AudioFileProperty.PacketSizeUpperBound), Is.EqualTo ("pkub"), "PacketSizeUpperBound");
			Assert.That (FourCC ((int) AudioFileProperty.PacketTableInfo), Is.EqualTo ("pnfo"), "PacketTableInfo");
			Assert.That (FourCC ((int) AudioFileProperty.PacketToByte), Is.EqualTo ("pkby"), "PacketToByte");
			Assert.That (FourCC ((int) AudioFileProperty.PacketToFrame), Is.EqualTo ("pkfr"), "PacketToFrame");
			// were missing - part of https://developer.apple.com/library/ios/#documentation/MusicAudio/Reference/AudioStreamReference/Reference/reference.html
			Assert.That (FourCC ((int) AudioFileProperty.ReadyToProducePackets), Is.EqualTo ("redy"), "AverageBytesPerPacket");
			Assert.That (FourCC ((int) AudioFileProperty.AverageBytesPerPacket), Is.EqualTo ("abpp"), "ReadyToProducePackets");
		}

		// not defined in https://developer.apple.com/library/ios/#documentation/MusicAudio/Reference/AudioStreamReference/Reference/reference.html
		[Test]
		public void UndocumentedFourCC ()
		{
			Assert.That (FourCC ((int) AudioFileProperty.AlbumArtwork), Is.EqualTo ("aart"), "AlbumArtwork");           // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.ChunkIDs), Is.EqualTo ("chid"), "ChunkIDs");               // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.DataFormatName), Is.EqualTo ("fnme"), "DataFormatName");           // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.DeferSizeUpdates), Is.EqualTo ("dszu"), "DeferSizeUpdates");       // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.EstimatedDuration), Is.EqualTo ("edur"), "EstimatedDuration");     // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.ID3Tag), Is.EqualTo ("id3t"), "ID3Tag");                   // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.InfoDictionary), Is.EqualTo ("info"), "InfoDictionary");           // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.IsOptimized), Is.EqualTo ("optm"), "IsOptimized");             // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.MarkerList), Is.EqualTo ("mkls"), "MarkerList");               // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.RegionList), Is.EqualTo ("rgls"), "RegionList");               // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.ReserveDuration), Is.EqualTo ("rsrv"), "ReserveDuration");         // ? reference ?
			Assert.That (FourCC ((int) AudioFileProperty.SourceBitDepth), Is.EqualTo ("sbtd"), "SourceBitDepth");           // ? reference ?
		}

		[Test]
		public void CafNoData ()
		{
			using (AudioFileStream afs = new AudioFileStream (AudioFileType.CAF)) {
				Assert.That (afs.StreamBasicDescription.BitsPerChannel, Is.EqualTo (0), "StreamBasicDescription.BitsPerChannel");
				Assert.That (afs.StreamBasicDescription.BytesPerFrame, Is.EqualTo (0), "StreamBasicDescription.BytesPerFrame");
				Assert.That (afs.StreamBasicDescription.BytesPerPacket, Is.EqualTo (0), "StreamBasicDescription.BytesPerPacket");
				Assert.That (afs.StreamBasicDescription.ChannelsPerFrame, Is.EqualTo (0), "StreamBasicDescription.ChannelsPerFrame");
				Assert.That (afs.StreamBasicDescription.Format, Is.EqualTo ((AudioFormatType) 0), "StreamBasicDescription.Format");
				Assert.That (afs.StreamBasicDescription.FormatFlags, Is.EqualTo ((AudioFormatFlags) 0), "StreamBasicDescription.FormatFlags");
				Assert.That (afs.StreamBasicDescription.FramesPerPacket, Is.EqualTo (0), "StreamBasicDescription.FramesPerPacket");
				Assert.That (afs.StreamBasicDescription.Reserved, Is.EqualTo (0), "StreamBasicDescription.Reserved");
				Assert.That (afs.StreamBasicDescription.SampleRate, Is.EqualTo (0), "StreamBasicDescription.SampleRate");

				int offset;
				var packet = afs.FrameToPacket (0, out offset);
				Assert.That (packet, Is.LessThanOrEqualTo (0), "FrameToPacket");    // -1 on first run
				Assert.That (offset, Is.EqualTo (0), "offset");

				long frame = afs.PacketToFrame (packet);
				Assert.That (frame, Is.LessThanOrEqualTo (0), "PacketToFrame");     // -1 on first run
			}
		}
	}
}

#endif // !__WATCHOS__
