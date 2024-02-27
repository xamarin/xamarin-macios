// Copyright 2022 Xamarin Inc. All rights reserved

#if __IOS__ || __MACOS__
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using AudioToolbox;
using AudioUnit;
using AVFoundation;
using CoreFoundation;
using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioConverterTest {

		[Test]
		public void Formats ()
		{
			var decodeFormats = AudioConverter.DecodeFormats;
			Assert.NotNull (decodeFormats, "Decode #1");
			Assert.That (decodeFormats.Length, Is.GreaterThan (10), "Decode Length #1");

			var encodeFormats = AudioConverter.EncodeFormats;
			Assert.NotNull (encodeFormats, "Encode #1");
			Assert.That (encodeFormats.Length, Is.GreaterThan (10), "Encode Length #1");
		}

		[Test]
		public void Convert ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			var sourcePath = Path.Combine (NSBundle.MainBundle.ResourcePath, "Hand.wav");
			var paths = NSSearchPath.GetDirectories (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);

			// Convert once
			var output1 = Path.Combine (paths [0], "output1.caf");
			Convert (sourcePath, output1, AudioFormatType.AppleLossless);

			// Convert converted output
			var output2 = Path.Combine (paths [0], "output2.wav");
			Convert (output1, output2, AudioFormatType.LinearPCM);
		}

		void Convert (string sourceFilePath, string destinationFilePath, AudioFormatType outputFormatType, int? sampleRate = null)
		{
			var destinationUrl = NSUrl.FromFilename (destinationFilePath);
			var sourceUrl = NSUrl.FromFilename (sourceFilePath);

			// get the source file
			var name = Path.GetFileNameWithoutExtension (destinationFilePath);
			using var sourceFile = AudioFile.Open (sourceUrl, AudioFilePermission.Read);

			var srcFormat = (AudioStreamBasicDescription) sourceFile.DataFormat;
			var dstFormat = new AudioStreamBasicDescription ();

			// setup the output file format
			dstFormat.SampleRate = sampleRate ?? srcFormat.SampleRate;
			if (outputFormatType == AudioFormatType.LinearPCM) {
				// if the output format is PCM create a 16 - bit int PCM file format
				dstFormat.Format = AudioFormatType.LinearPCM;
				dstFormat.ChannelsPerFrame = srcFormat.ChannelsPerFrame;
				dstFormat.BitsPerChannel = 16;
				dstFormat.BytesPerPacket = dstFormat.BytesPerFrame = 2 * dstFormat.ChannelsPerFrame;
				dstFormat.FramesPerPacket = 1;
				dstFormat.FormatFlags = AudioFormatFlags.LinearPCMIsPacked | AudioFormatFlags.LinearPCMIsSignedInteger;
			} else {
				// compressed format - need to set at least format, sample rate and channel fields for kAudioFormatProperty_FormatInfo
				dstFormat.Format = outputFormatType;
				dstFormat.ChannelsPerFrame = srcFormat.ChannelsPerFrame; // for iLBC num channels must be 1

				// use AudioFormat API to fill out the rest of the description
				var afe = AudioStreamBasicDescription.GetFormatInfo (ref dstFormat);
				Assert.AreEqual (AudioFormatError.None, afe, $"GetFormatInfo: {name}");
			}

			// create the AudioConverter
			using var converter = AudioConverter.Create (srcFormat, dstFormat, out var ce);
			Assert.AreEqual (AudioConverterError.None, ce, $"AudioConverterCreate: {name}");

			// set up source buffers and data proc info struct
			var afio = new AudioFileIO (32 * 1024); // 32Kb

			converter.InputData += (ref int numberDataPackets, AudioBuffers data, ref AudioStreamPacketDescription [] dataPacketDescription) => {
				return EncoderDataProc (afio, ref numberDataPackets, data, ref dataPacketDescription);
			};

			// Some audio formats have a magic cookie associated with them which is required to decompress audio data
			// When converting audio data you must check to see if the format of the data has a magic cookie
			// If the audio data format has a magic cookie associated with it, you must add this information to anAudio Converter
			// using AudioConverterSetProperty and kAudioConverterDecompressionMagicCookie to appropriately decompress the data
			// http://developer.apple.com/mac/library/qa/qa2001/qa1318.html
			var cookie = sourceFile.MagicCookie;

			// if there is an error here, then the format doesn't have a cookie - this is perfectly fine as some formats do not
			if (cookie?.Length > 0)
				converter.DecompressionMagicCookie = cookie;

			// get the actual formats back from the Audio Converter
			srcFormat = converter.CurrentInputStreamDescription;
			dstFormat = converter.CurrentOutputStreamDescription;

			// create the destination file
			using var destinationFile = AudioFile.Create (destinationUrl, AudioFileType.CAF, dstFormat, AudioFileFlags.EraseFlags);

			// set up source buffers and data proc info struct
			afio.SourceFile = sourceFile;
			afio.SrcFormat = srcFormat;

			if (srcFormat.BytesPerPacket == 0) {
				// if the source format is VBR, we need to get the maximum packet size
				// use kAudioFilePropertyPacketSizeUpperBound which returns the theoretical maximum packet size
				// in the file (without actually scanning the whole file to find the largest packet,
				// as may happen with kAudioFilePropertyMaximumPacketSize)
				afio.SrcSizePerPacket = sourceFile.PacketSizeUpperBound;

				// how many packets can we read for our buffer size?
				afio.NumPacketsPerRead = afio.SrcBufferSize / afio.SrcSizePerPacket;

				// allocate memory for the PacketDescription structures describing the layout of each packet
				afio.PacketDescriptions = new AudioStreamPacketDescription [afio.NumPacketsPerRead];
			} else {
				// CBR source format
				afio.SrcSizePerPacket = srcFormat.BytesPerPacket;
				afio.NumPacketsPerRead = afio.SrcBufferSize / afio.SrcSizePerPacket;
			}

			// set up output buffers
			int outputSizePerPacket = dstFormat.BytesPerPacket; // this will be non-zero if the format is CBR
			const int theOutputBufSize = 32 * 1024; // 32Kb
			var outputBuffer = Marshal.AllocHGlobal (theOutputBufSize);
			AudioStreamPacketDescription [] outputPacketDescriptions = null;

			if (outputSizePerPacket == 0) {
				// if the destination format is VBR, we need to get max size per packet from the converter
				outputSizePerPacket = (int) converter.MaximumOutputPacketSize;

				// allocate memory for the PacketDescription structures describing the layout of each packet
				outputPacketDescriptions = new AudioStreamPacketDescription [theOutputBufSize / outputSizePerPacket];
			}
			int numOutputPackets = theOutputBufSize / outputSizePerPacket;

			// if the destination format has a cookie, get it and set it on the output file
			WriteCookie (converter, destinationFile);

			long totalOutputFrames = 0; // used for debugging
			long outputFilePos = 0;
			AudioBuffers fillBufList = new AudioBuffers (1);

			// loop to convert data
			while (true) {
				// set up output buffer list
				fillBufList [0] = new AudioBuffer () {
					NumberChannels = dstFormat.ChannelsPerFrame,
					DataByteSize = theOutputBufSize,
					Data = outputBuffer
				};

				// convert data
				int ioOutputDataPackets = numOutputPackets;
				var fe = converter.FillComplexBuffer (ref ioOutputDataPackets, fillBufList, outputPacketDescriptions);
				// if interrupted in the process of the conversion call, we must handle the error appropriately
				Assert.AreEqual (AudioConverterError.None, fe, $"FillComplexBuffer: {name}");

				if (ioOutputDataPackets == 0) {
					// this is the EOF conditon
					break;
				}

				// write to output file
				var inNumBytes = fillBufList [0].DataByteSize;

				var we = destinationFile.WritePackets (false, inNumBytes, outputPacketDescriptions, outputFilePos, ref ioOutputDataPackets, outputBuffer);
				Assert.AreEqual (AudioFileError.Success, we, $"WritePackets: {name}");

				// advance output file packet position
				outputFilePos += ioOutputDataPackets;

				// the format has constant frames per packet
				totalOutputFrames += (ioOutputDataPackets * dstFormat.FramesPerPacket);
			}

			Marshal.FreeHGlobal (outputBuffer);

			// write out any of the leading and trailing frames for compressed formats only
			if (dstFormat.BitsPerChannel == 0)
				WritePacketTableInfo (converter, destinationFile);

			// write the cookie again - sometimes codecs will update cookies at the end of a conversion
			WriteCookie (converter, destinationFile);
		}

		// Input data proc callback
		AudioConverterError EncoderDataProc (AudioFileIO afio, ref int numberDataPackets, AudioBuffers data, ref AudioStreamPacketDescription [] dataPacketDescription)
		{
			// figure out how much to read
			int maxPackets = afio.SrcBufferSize / afio.SrcSizePerPacket;
			if (numberDataPackets > maxPackets)
				numberDataPackets = maxPackets;

			// read from the file
			int outNumBytes = 16384;

			// modified for iOS7 (ReadPackets depricated)
			afio.PacketDescriptions = afio.SourceFile.ReadPacketData (false, afio.SrcFilePos, ref numberDataPackets, afio.SrcBuffer, ref outNumBytes);

			if (afio.PacketDescriptions.Length == 0 && numberDataPackets > 0)
				throw new ApplicationException (afio.PacketDescriptions.ToString ());

			// advance input file packet position
			afio.SrcFilePos += numberDataPackets;

			// put the data pointer into the buffer list
			data.SetData (0, afio.SrcBuffer, outNumBytes);

			// don't forget the packet descriptions if required
			if (dataPacketDescription is not null)
				dataPacketDescription = afio.PacketDescriptions;

			return AudioConverterError.None;
		}

		// Some audio formats have a magic cookie associated with them which is required to decompress audio data
		// When converting audio, a magic cookie may be returned by the Audio Converter so that it may be stored along with
		// the output data -- This is done so that it may then be passed back to the Audio Converter at a later time as required
		static void WriteCookie (AudioConverter converter, AudioFile destinationFile)
		{
			var cookie = converter.CompressionMagicCookie;
			if (cookie is not null && cookie.Length != 0) {
				destinationFile.MagicCookie = cookie;
			}
		}

		// Sets the packet table containing information about the number of valid frames in a file and where they begin and end
		// for the file types that support this information.
		// Calling this function makes sure we write out the priming and remainder details to the destination file
		static void WritePacketTableInfo (AudioConverter converter, AudioFile destinationFile)
		{
			if (!destinationFile.IsPropertyWritable (AudioFileProperty.PacketTableInfo))
				return;

			// retrieve the leadingFrames and trailingFrames information from the converter,
			AudioConverterPrimeInfo primeInfo = converter.PrimeInfo;

			// we have some priming information to write out to the destination file
			// The total number of packets in the file times the frames per packet (or counting each packet's
			// frames individually for a variable frames per packet format) minus mPrimingFrames, minus
			// mRemainderFrames, should equal mNumberValidFrames.

			AudioFilePacketTableInfo? pti_n = destinationFile.PacketTableInfo;
			if (pti_n is null)
				return;

			AudioFilePacketTableInfo pti = pti_n.Value;

			// there's priming to write out to the file
			// get the total number of frames from the output file
			long totalFrames = pti.ValidFrames + pti.PrimingFrames + pti.RemainderFrames;

			pti.PrimingFrames = primeInfo.LeadingFrames;
			pti.RemainderFrames = primeInfo.TrailingFrames;
			pti.ValidFrames = totalFrames - pti.PrimingFrames - pti.RemainderFrames;

			destinationFile.PacketTableInfo = pti;
		}

		class AudioFileIO {
			public AudioFileIO (int bufferSize)
			{
				this.SrcBufferSize = bufferSize;
				this.SrcBuffer = Marshal.AllocHGlobal (bufferSize);
			}

			~AudioFileIO ()
			{
				Marshal.FreeHGlobal (SrcBuffer);
			}

			public AudioFile SourceFile { get; set; }

			public int SrcBufferSize { get; private set; }

			public IntPtr SrcBuffer { get; private set; }

			public int SrcFilePos { get; set; }

			public AudioStreamBasicDescription SrcFormat { get; set; }

			public int SrcSizePerPacket { get; set; }

			public int NumPacketsPerRead { get; set; }

			public AudioStreamPacketDescription [] PacketDescriptions { get; set; }
		}
	}
}

#endif
