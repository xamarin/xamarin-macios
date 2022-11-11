// Unit test for AVAudioFormat
// Authors: 
// 		Whitney Schmidt (whschm@microsoft.com)
// Copyright 2020 Microsoft Corp.

using System;

using AudioToolbox;
using Foundation;
using AVFoundation;
using NUnit.Framework;
using ObjCRuntime;
using Xamarin.Utils;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVAudioFormatTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
		}

		[Test]
		public void TestEqualOperatorSameInstace ()
		{
			using (var format = new AVAudioFormat ())
				Assert.IsTrue (format == format, "format == format");
		}

		[Test]
		public void TestEqualOperatorNull ()
		{
			using (var format = new AVAudioFormat ()) {
				Assert.IsFalse (format == null, "format == null");
				Assert.IsFalse (null == format, "null == format");
			}
			using (AVAudioFormat nullFormat = null) {
				Assert.IsTrue (nullFormat == null, "nullFormat == null");
				Assert.IsTrue (null == nullFormat, "null == nullFormat");
			}
		}

		[Test]
		public void TestNotEqualOperatorNull ()
		{
			using (var format = new AVAudioFormat ()) {
				Assert.IsTrue (format != null, "format != null");
				Assert.IsTrue (null != format, "null != format");
			}
			using (AVAudioFormat nullFormat = null) {
				Assert.IsFalse (nullFormat != null, "nullFormat != null");
				Assert.IsFalse (null != nullFormat, "null != nullFormat");
			}

		}

		[Test]
		public void StreamDescription ()
		{
			var format = new AVAudioFormat (AVAudioCommonFormat.PCMFloat32, 44100.0, 2, true);
			var desc = format.StreamDescription;
			Assert.AreEqual (AudioFormatType.LinearPCM, desc.Format, "Format");
			Assert.AreEqual (AudioFormatFlags.LinearPCMIsFloat | AudioFormatFlags.LinearPCMIsPacked, desc.FormatFlags, "FormatFlags");
			Assert.AreEqual (8, desc.BytesPerPacket, "BytesPerPacket");
			Assert.AreEqual (1, desc.FramesPerPacket, "FramesPerPacket");
			Assert.AreEqual (8, desc.BytesPerFrame, "BytesPerFrame");
			Assert.AreEqual (2, desc.ChannelsPerFrame, "ChannelsPerFrame");
			Assert.AreEqual (32, desc.BitsPerChannel, "BitsPerChannel");
			Assert.AreEqual (0, desc.Reserved, "Reserved");
		}
	}
}
