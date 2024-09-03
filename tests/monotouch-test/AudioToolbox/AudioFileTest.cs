// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__

using System;
using System.IO;
using System.Runtime.InteropServices;

using Foundation;
using AudioToolbox;
using CoreFoundation;
using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioFileTest {

		[Test]
		public void ReadTest ()
		{
			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");
			var af = AudioFile.Open (CFUrl.FromFile (path), AudioFilePermission.Read, AudioFileType.CAF);

			int len;
			long current = 0;
			long size = 1024;
			byte [] buffer = new byte [size];
			while ((len = af.Read (current, buffer, 0, buffer.Length, false)) != -1) {
				current += len;
			}

			var full_len = new FileInfo (path).Length;
			int header = 4096;
			Assert.That (header + current == full_len, "#1");
		}

		[Test]
		public void ApiTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");
			using var af = AudioFile.Open (CFUrl.FromFile (path), AudioFilePermission.Read, AudioFileType.CAF);
			var chunkIds = af.ChunkIDs;
			Assert.That (chunkIds.Length, Is.GreaterThan (0), "ChunkIDs");

			var memorySize = 1024;
			IntPtr memory = Marshal.AllocHGlobal (memorySize); ;
			int size = 0;
			int offset;
			byte [] buffer;
			var expectedData = new byte [] { 0x40, 0xc5, 0x7c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x69, 0x6d, 0x61, 0x34, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x22, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };

			try {
				var chunkType = AudioFileChunkType.CAFStreamDescription;
				Assert.Multiple (() => {
					Assert.AreEqual (1, af.CountUserData (chunkType), "CountUserData #1");
					Assert.AreEqual (1, af.CountUserData ((uint) chunkType), "CountUserData #2");

					Assert.AreEqual (32, af.GetUserDataSize (chunkType, 0), "GetUserDataSize #1");
					Assert.AreEqual (32, af.GetUserDataSize ((uint) chunkType, 0), "GetUserDataSize #2");

					Assert.AreEqual (AudioFileError.Success, af.GetUserDataSize (chunkType, 0, out var userDataSize64), "GetUserDataSize64 #1");
					Assert.AreEqual (32, userDataSize64, "GetUserDataSize64 #2");

					Assert.AreEqual (AudioFileError.Success, af.GetUserDataSize ((uint) chunkType, 0, out userDataSize64), "GetUserDataSize64 #3");
					Assert.AreEqual (32, userDataSize64, "GetUserDataSize64 #4");

					size = memorySize;
					Assert.AreEqual (AudioFileError.Success, af.GetUserData (chunkType, 0, ref size, memory), "GetUserData #1");
					Assert.AreEqual (32, size, "GetUserData #2");
					Assert.AreEqual (size, expectedData.Length, "GetUserData #3");
					for (var i = 0; i < expectedData.Length; i++) {
						Assert.AreEqual (expectedData [i], Marshal.ReadByte (memory, i), $"GetUserData #4[{i}]");
						Marshal.WriteByte (memory, i, 0);
					}

					size = memorySize;
					Assert.AreEqual (0, af.GetUserData ((int) chunkType, 0, ref size, memory), "GetUserData/B #1");
					Assert.AreEqual (32, size, "GetUserData/B #2");
					Assert.AreEqual (size, expectedData.Length, "GetUserData/B #3");
					for (var i = 0; i < expectedData.Length; i++) {
						Assert.AreEqual (expectedData [i], Marshal.ReadByte (memory, i), $"GetUserData/B #4[{i}]");
						Marshal.WriteByte (memory, i, 0);
					}

					size = memorySize;
					offset = 16;
					Assert.AreEqual (AudioFileError.Success, af.GetUserData (chunkType, 0, offset, ref size, memory), "GetUserDataAtOffset/A #1");
					Assert.AreEqual (32 - offset, size, "GetUserDataAtOffset/A #2");
					Assert.AreEqual (size, expectedData.Length - offset, "GetUserDataAtOffset/A #3");
					for (var i = offset; i < expectedData.Length; i++) {
						Assert.AreEqual (expectedData [i], Marshal.ReadByte (memory, i - offset), $"GetUserDataAtOffset/A #4[{i}]");
						Marshal.WriteByte (memory, i - offset, 0);
					}

					size = memorySize;
					offset = 12;
					Assert.AreEqual (AudioFileError.Success, af.GetUserData ((uint) chunkType, 0, offset, ref size, memory), "GetUserDataAtOffset/B #1");
					Assert.AreEqual (32 - offset, size, "GetUserDataAtOffset/B #2");
					Assert.AreEqual (size, expectedData.Length - offset, "GetUserDataAtOffset/B #3");
					for (var i = offset; i < expectedData.Length; i++) {
						Assert.AreEqual (expectedData [i], Marshal.ReadByte (memory, i - offset), $"GetUserDataAtOffset/B #4[{i}]");
						Marshal.WriteByte (memory, i - offset, 0);
					}

					size = memorySize;
					offset = 24;
					buffer = new byte [memorySize];
					Assert.AreEqual (AudioFileError.Success, af.GetUserData (chunkType, 0, offset, buffer, out size), "GetUserDataAtOffset/C #1");
					Assert.AreEqual (32 - offset, size, "GetUserDataAtOffset/C #2");
					Assert.AreEqual (size, expectedData.Length - offset, "GetUserDataAtOffset/C #3");
					for (var i = offset; i < expectedData.Length; i++)
						Assert.AreEqual (expectedData [i], buffer [i - offset], $"GetUserDataAtOffset/C #4[{i}]");

					size = memorySize;
					offset = 8;
					Assert.AreEqual (AudioFileError.Success, af.GetUserData ((uint) chunkType, 0, offset, buffer, out size), "GetUserDataAtOffset/D #1");
					Assert.AreEqual (32 - offset, size, "GetUserDataAtOffset/D #2");
					Assert.AreEqual (size, expectedData.Length - offset, "GetUserDataAtOffset/D #3");
					for (var i = offset; i < expectedData.Length; i++)
						Assert.AreEqual (expectedData [i], buffer [i - offset], $"GetUserDataAtOffset/D #4[{i}]");
				});
			} finally {
				Marshal.FreeHGlobal (memory);
			}
		}
	}
}

#endif // !__WATCHOS__
