//
// Unit tests for CMTag
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2024 Microsoft Corp. All rights reserved.
//
using System;

using CoreFoundation;
using CoreMedia;
using CoreVideo;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

using Xamarin.Utils;

using MonoTouchFixtures.AVFoundation;

namespace MonoTouchFixtures.CoreMedia {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CMTaggedBufferGroupTests {
		[Test]
		public void GetTypeIdTest ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			Assert.AreNotEqual (0, CMTaggedBufferGroup.GetTypeId (), "GetTypeId");
		}

		[Test]
		public void Create_PixelBuffers ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { pixelBuffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status");
				Assert.AreEqual (1, (int) group.Count, "Count A");

				Assert.Throws<ArgumentException> (() => CMTaggedBufferGroup.Create (
						new [] { tagCollection },
						new [] { pixelBuffer, pixelBuffer },
						out var _), "AE 1 Different element count");

				Assert.Throws<ArgumentNullException> (() => CMTaggedBufferGroup.Create (
						null,
						new [] { pixelBuffer },
						out var _), "ANE 1");

				Assert.Throws<ArgumentNullException> (() => CMTaggedBufferGroup.Create (
						new [] { tagCollection },
						(CVPixelBuffer []) null,
						out var _), "ANE 2");
			}
		}

		static CMSampleBuffer CreateCMSampleBuffer ()
		{
			var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
			var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out var fde);
			var sampleTiming = new CMSampleTimingInfo ();
			return CMSampleBuffer.CreateForImageBuffer (pixelBuffer, true, desc, sampleTiming, out var sbe);
		}

		[Test]
		public void Create_MediaBuffers ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var sampleBuffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { sampleBuffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status");
				Assert.AreEqual (1, (int) group.Count, "Count A");

				Assert.Throws<ArgumentException> (() => CMTaggedBufferGroup.Create (
						new [] { tagCollection },
						new [] { sampleBuffer, sampleBuffer },
						out var _), "AE 1 Different element count");

				Assert.Throws<ArgumentNullException> (() => CMTaggedBufferGroup.Create (
						null,
						new [] { sampleBuffer },
						out var _), "ANE 1");

				Assert.Throws<ArgumentNullException> (() => CMTaggedBufferGroup.Create (
						new [] { tagCollection },
						(CMSampleBuffer []) null,
						out var _), "ANE 2");
			}
		}

		[Test]
		public void Create_MixedBuffers ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var sampleBuffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection1, tagCollection2 },
					new NativeObject [] { pixelBuffer, sampleBuffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status");
				Assert.AreEqual (2, (int) group.Count, "Count A");

				Assert.Throws<ArgumentException> (() => CMTaggedBufferGroup.Create (
						new [] { tagCollection1 },
						new NativeObject [] { pixelBuffer, sampleBuffer },
						out var _), "AE 1 Different element count");

				Assert.Throws<ArgumentNullException> (() => CMTaggedBufferGroup.Create (
						null,
						new NativeObject [] { pixelBuffer, sampleBuffer },
						out var _), "ANE 1");

				Assert.Throws<ArgumentNullException> (() => CMTaggedBufferGroup.Create (
						new [] { tagCollection1 },
						(NativeObject []) null,
						out var _), "ANE 2");

				Assert.Throws<ArgumentException> (() => CMTaggedBufferGroup.Create (
						new [] { tagCollection1 },
						new NativeObject [] { tagCollection2 },
						out var _), "AE 2 Wrong element type");
			}
		}

		[Test]
		public void Combine ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer1 = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group1 = CMTaggedBufferGroup.Create (
					new [] { tagCollection1 },
					new [] { buffer1 },
					out var status1);

				using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer2 = CreateCMSampleBuffer (); // new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group2 = CMTaggedBufferGroup.Create (
					new [] { tagCollection2 },
					new [] { buffer2 },
					out var status2);

				using var group = CMTaggedBufferGroup.Combine (out var status, group1, group2);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status");
				Assert.AreEqual (2, (int) group.Count, "Count A");
			}
		}

		[Test]
		public void GetTagCollection ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { pixelBuffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				Assert.AreEqual (tagCollection.Handle, group.GetTagCollection (0).Handle, "#0");

				Assert.Throws<ArgumentOutOfRangeException> (() => group.GetTagCollection (-1), "AOORE: -1");
				Assert.Throws<ArgumentOutOfRangeException> (() => group.GetTagCollection (1), "AOORE: 1");
			}
		}

		[Test]
		public void GetPixelBuffer ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				Assert.IsNotNull (group.GetPixelBuffer (0), "#0 A");

				Assert.Throws<ArgumentOutOfRangeException> (() => group.GetPixelBuffer (-1), "AOORE: -1");
				Assert.Throws<ArgumentOutOfRangeException> (() => group.GetPixelBuffer (1), "AOORE: 1");
			}

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status B");
				Assert.AreEqual (1, (int) group.Count, "Count B");
				Assert.IsNull (group.GetPixelBuffer (0), "#0 B");
			}
		}

		[Test]
		public void GetSampleBuffer ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				Assert.AreEqual (buffer.Handle, group.GetSampleBuffer (0).Handle, "#0 A");

				Assert.Throws<ArgumentOutOfRangeException> (() => group.GetSampleBuffer (-1), "AOORE: -1");
				Assert.Throws<ArgumentOutOfRangeException> (() => group.GetSampleBuffer (1), "AOORE: 1");
			}

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status B");
				Assert.AreEqual (1, (int) group.Count, "Count B");
				Assert.IsNull (group.GetSampleBuffer (0), "#0 B");
			}
		}

		[Test]
		public void GetMixedBuffers ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var sampleBuffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection1, tagCollection2 },
					new NativeObject [] { pixelBuffer, sampleBuffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status C");
				Assert.AreEqual (2, (int) group.Count, "Count C");

				Assert.IsNotNull (group.GetPixelBuffer (0), "#0 C1");
				Assert.IsNull (group.GetPixelBuffer (1), "#1 C1");

				Assert.IsNull (group.GetSampleBuffer (0), "#0 C2");
				Assert.IsNotNull (group.GetSampleBuffer (1), "#1 C2");
			}
		}

		[Test]
		public void GetPixelBuffer_Tag ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				Assert.AreEqual (buffer.Handle, group.GetPixelBuffer (CMTag.MediaTypeVideo, out var index).Handle, "Video A");
				Assert.AreEqual (0, (int) index, "Index A");
				Assert.IsNull (group.GetPixelBuffer (CMTag.MediaTypeAudio, out index), "Audio A");
			}

			{
				// buffer of wrong type
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				Assert.IsNull (group.GetPixelBuffer (CMTag.MediaTypeVideo, out var index), "Video A");
			}

			{
				// more than one of the buffers match
				using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer1 = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var buffer2 = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection1, tagCollection2 },
					new [] { buffer1, buffer2 },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (2, (int) group.Count, "Count A");
				Assert.IsNull (group.GetPixelBuffer (CMTag.MediaTypeVideo, out var index), "Video A");
			}
		}

		[Test]
		public void GetPixelBuffer_TagCollection ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				Assert.AreEqual (buffer.Handle, group.GetPixelBuffer (tagCollection, out var index).Handle, "Video A");
				Assert.AreEqual (0, (int) index, "Index A");
				using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio);
				Assert.IsNull (group.GetPixelBuffer (tagCollection2, out index), "Audio A");
			}

			{
				// buffer of wrong type
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				Assert.IsNull (group.GetPixelBuffer (tagCollection, out var index), "Video A");
			}

			{
				// more than one of the buffers match
				using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer1 = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var buffer2 = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection1, tagCollection2 },
					new [] { buffer1, buffer2 },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (2, (int) group.Count, "Count A");
				Assert.IsNull (group.GetPixelBuffer (tagCollection1, out var index), "Video A");
			}
		}

		[Test]
		public void GetSampleBuffer_Tag ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				Assert.AreEqual (buffer.Handle, group.GetSampleBuffer (CMTag.MediaTypeVideo, out var index).Handle, "Video A");
				Assert.AreEqual (0, (int) index, "Index A");
				Assert.IsNull (group.GetSampleBuffer (CMTag.MediaTypeAudio, out index), "Audio A");
			}

			{
				// buffer of wrong type
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				Assert.IsNull (group.GetSampleBuffer (CMTag.MediaTypeVideo, out var index), "Video A");
			}

			{
				// more than one of the buffers match
				using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer1 = CreateCMSampleBuffer ();
				using var buffer2 = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection1, tagCollection2 },
					new [] { buffer1, buffer2 },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (2, (int) group.Count, "Count A");
				Assert.IsNull (group.GetSampleBuffer (CMTag.MediaTypeVideo, out var index), "Video A");
			}
		}

		[Test]
		public void GetSampleBuffer_TagCollection ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				Assert.IsNotNull (group.GetSampleBuffer (tagCollection, out var index), "Video A");
				Assert.AreEqual (0, (int) index, "Index A");
				using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeAudio);
				Assert.IsNull (group.GetSampleBuffer (tagCollection2, out index), "Audio A");
			}

			{
				// buffer of wrong type
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status B");
				Assert.AreEqual (1, (int) group.Count, "Count B");
				Assert.IsNull (group.GetSampleBuffer (tagCollection, out var index), "Video B");
			}

			{
				// more than one of the buffers match
				using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer1 = CreateCMSampleBuffer ();
				using var buffer2 = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection1, tagCollection2 },
					new [] { buffer1, buffer2 },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status C");
				Assert.AreEqual (2, (int) group.Count, "Count C");
				Assert.IsNull (group.GetSampleBuffer (tagCollection1, out var index), "Video C");
			}
		}

		[Test]
		public void GetNumberOfMatches ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				// more than one of the buffers match
				using var tagCollection1 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var tagCollection2 = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer1 = CreateCMSampleBuffer ();
				using var buffer2 = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection1, tagCollection2 },
					new [] { buffer1, buffer2 },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (2, (int) group.Count, "Count A");
				Assert.AreEqual (2, (int) group.GetNumberOfMatches (tagCollection1), "Matches 1 A");
				using var tagCollection3 = CMTagCollection.Create (CMTag.MediaTypeAudio);
				Assert.AreEqual (0, (int) group.GetNumberOfMatches (tagCollection3), "Matches 2 A");
			}
		}

		[Test]
		public void CreateFormatDescription ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				using var desc = group.CreateFormatDescription (out status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status Desc A");
				Assert.AreEqual (CMMediaType.TaggedBufferGroup, desc.MediaType, $"Desc.MediaType: {AVFoundationEnumTest.FourCC ((int) desc.MediaType)}");
				Assert.AreEqual (CMTaggedBufferGroupFormatType.TaggedBufferGroup, desc.TaggedBufferGroupFormatType, "Desc.TaggedBufferGroupFormatType");
			}
		}

		[Test]
		public void Matches ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				using var desc = group.CreateFormatDescription (out status);
				Assert.IsTrue (group.Matches (desc), "Matches A");
				using var desc2 = CMFormatDescription.Create (CMMediaType.ClosedCaption, (uint) CMClosedCaptionFormatType.CEA608, out var fde);
				Assert.AreEqual (CMFormatDescriptionError.None, fde, "FDE");
				Assert.IsFalse (group.Matches (desc2), "Matches B");
			}
		}

		[Test]
		public void CreateSampleBuffer ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				using var formatDescription = CMFormatDescription.Create (CMMediaType.TaggedBufferGroup, (uint) CMTaggedBufferGroupFormatType.TaggedBufferGroup, out var fde);
				Assert.AreEqual (CMFormatDescriptionError.None, fde, "FDE A");
				using var sampleBuffer = group.CreateSampleBuffer (CMTime.Zero, CMTime.FromSeconds (1, 1), formatDescription, out status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status CSB A");
			}
		}

		[Test]
		public void GetTaggedBufferGroup ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			{
				using var tagCollection = CMTagCollection.Create (CMTag.MediaTypeVideo);
				using var buffer = CreateCMSampleBuffer ();
				using var group = CMTaggedBufferGroup.Create (
					new [] { tagCollection },
					new [] { buffer },
					out var status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status A");
				Assert.AreEqual (1, (int) group.Count, "Count A");
				using var formatDescription = CMFormatDescription.Create (CMMediaType.TaggedBufferGroup, (uint) CMTaggedBufferGroupFormatType.TaggedBufferGroup, out var fde);
				Assert.AreEqual (CMFormatDescriptionError.None, fde, "FDE A");
				using var sampleBuffer = group.CreateSampleBuffer (CMTime.Zero, CMTime.FromSeconds (1, 1), formatDescription, out status);
				Assert.AreEqual (CMTaggedBufferGroupError.Success, status, "Status CSB A");

				Assert.IsNotNull (sampleBuffer.TaggedBufferGroup, "CMSampleBuffer.GetTaggedBufferGroup A");
				Assert.IsNotNull (CMTaggedBufferGroup.GetTaggedBufferGroup (sampleBuffer), "CMTaggedBufferGroup.GetTaggedBufferGroup A");

				Assert.IsNull (buffer.TaggedBufferGroup, "CMSampleBuffer.GetTaggedBufferGroup B");
				Assert.IsNull (CMTaggedBufferGroup.GetTaggedBufferGroup (buffer), "CMTaggedBufferGroup.GetTaggedBufferGroup B");

				Assert.Throws<ArgumentNullException> (() => CMTaggedBufferGroup.GetTaggedBufferGroup (null), "ANE");
			}
		}
	}
}
