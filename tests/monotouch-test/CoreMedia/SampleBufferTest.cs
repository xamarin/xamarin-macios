//
// Unit tests for CMSampleBuffer
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
#if !__TVOS__
using EventKit;
#endif
using ObjCRuntime;
using CoreVideo;
using CoreMedia;
#else
using MonoTouch.EventKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreVideo;
using MonoTouch.CoreMedia;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMedia {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SampleBufferTest
	{
		[Test]
		public void CreateForImageBuffer ()
		{
			var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB);

			CMFormatDescriptionError fde;
			var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out fde);

			var sampleTiming = new CMSampleTimingInfo ();

			CMSampleBufferError sbe;
			var sb = CMSampleBuffer.CreateForImageBuffer (pixelBuffer, true, desc, sampleTiming, out sbe);
			Assert.IsNotNull (sb, "#1");
			Assert.AreEqual (CMSampleBufferError.None, sbe, "#2");
		}

		[Test]
		public void CreateReadyWithPacketDescriptions ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);

			CMBlockBufferError bbe;
			using (var bb = CMBlockBuffer.CreateEmpty (0, CMBlockBufferFlags.AlwaysCopyData, out bbe)) {
				CMFormatDescriptionError fde;
				using (var fd = CMFormatDescription.Create (CMMediaType.ClosedCaption, (uint) CMClosedCaptionFormatType.CEA608, out fde)) {
					CMSampleBufferError sbe;
					using (var sb = CMSampleBuffer.CreateReadyWithPacketDescriptions (bb, fd, 1, CMTime.Indefinite, null, out sbe)) {
						Assert.Null (sb, "CMSampleBuffer");
						// the `null` does not match format description (but I lack a better test, at least it's callable)
						Assert.That (sbe, Is.EqualTo (CMSampleBufferError.RequiredParameterMissing), "CMSampleBufferError");
					}
				}
			}
		}

		[Test]
		public void CreateReady ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);

			CMBlockBufferError bbe;
			using (var bb = CMBlockBuffer.CreateEmpty (0, CMBlockBufferFlags.AlwaysCopyData, out bbe)) {
				CMSampleBufferError sbe;
				using (var sb = CMSampleBuffer.CreateReady (bb, null, 0, null, null, out sbe)) {
					Assert.That (sb.Handle, Is.Not.EqualTo (IntPtr.Zero), "CMSampleBuffer");
					Assert.That (sbe, Is.EqualTo (CMSampleBufferError.None), "CMSampleBufferError");
				}
			}
		}

#if !XAMCORE_4_0
		[Test]
		public void CreateReadyWithImageBuffer_ArrayValidations ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);

			CMFormatDescriptionError fde;
			using (var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB))
			using (var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out fde)) {
				CMSampleBufferError sbe;
				Assert.Throws<ArgumentNullException> (() => CMSampleBuffer.CreateReadyWithImageBuffer (pixelBuffer, desc, null, out sbe), "null");

				var stia = new CMSampleTimingInfo [0];
				Assert.Throws<ArgumentException> (() => CMSampleBuffer.CreateReadyWithImageBuffer (pixelBuffer, desc, stia, out sbe), "empty");
			}
		}
#endif

		[Test]
		public void CreateReadyWithImageBuffer ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);

			CMFormatDescriptionError fde;
			using (var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB))
			using (var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out fde)) {
				CMSampleBufferError sbe;
				var sti = new CMSampleTimingInfo ();
				using (var sb = CMSampleBuffer.CreateReadyWithImageBuffer (pixelBuffer, desc, sti, out sbe)) {
					Assert.That (sb.Handle, Is.Not.EqualTo (IntPtr.Zero), "CMSampleBuffer");
					Assert.That (sbe, Is.EqualTo (CMSampleBufferError.None), "CMSampleBufferError");
				}
			}
		}

		[Test]
		public void SetInvalidateCallback_Replace ()
		{
			using (var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB)) {
				CMFormatDescriptionError fde;
				using (var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out fde)) {
					var sampleTiming = new CMSampleTimingInfo ();
					CMSampleBufferError sbe;
					using (var sb = CMSampleBuffer.CreateForImageBuffer (pixelBuffer, true, desc, sampleTiming, out sbe)) {
						int i = 0;
						var result = sb.SetInvalidateCallback (delegate (CMSampleBuffer buffer) {
							i++;
						});

						// we cannot replace the (native) callback without getting an error (so we should not replace
						// the managed one either, that would be confusing and make it hard to port code)
						result = sb.SetInvalidateCallback (delegate (CMSampleBuffer buffer) {
							i--;
							Assert.AreSame (buffer, sb, "same");
						});
						Assert.That (result, Is.EqualTo (CMSampleBufferError.RequiredParameterMissing), "RequiredParameterMissing");

						sb.Invalidate ();
						Assert.That (i, Is.EqualTo (1), "1");
					}
				}
			}
		}

		[Test]
		public void SetInvalidateCallback ()
		{
			using (var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB)) {
				CMFormatDescriptionError fde;
				using (var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out fde)) {
					var sampleTiming = new CMSampleTimingInfo ();
					CMSampleBufferError sbe;
					using (var sb = CMSampleBuffer.CreateForImageBuffer (pixelBuffer, true, desc, sampleTiming, out sbe)) {
						int i = 0;
						var result = sb.SetInvalidateCallback (delegate (CMSampleBuffer buffer) {
							i++;
							Assert.AreSame (buffer, sb, "same");
						});
						Assert.That (result, Is.EqualTo (CMSampleBufferError.None), "SetInvalidateCallback/None");

						result = (CMSampleBufferError) sb.Invalidate ();
						Assert.That (result, Is.EqualTo (CMSampleBufferError.None), "Invalidate/None");
						Assert.That (i, Is.EqualTo (1), "1");

						// a second call to Invalidate returns Invalidated
						result = (CMSampleBufferError) sb.Invalidate ();
						Assert.That (result, Is.EqualTo (CMSampleBufferError.Invalidated), "Invalidated");
					}
				}
			}
		}

		[Test]
		public void SetInvalidateCallback_Null ()
		{
			using (var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB)) {
				CMFormatDescriptionError fde;
				using (var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out fde)) {
					var sampleTiming = new CMSampleTimingInfo ();
					CMSampleBufferError sbe;
					using (var sb = CMSampleBuffer.CreateForImageBuffer (pixelBuffer, true, desc, sampleTiming, out sbe)) {
						// ignore `null`, i.e. no crash
						Assert.That (sb.SetInvalidateCallback (null), Is.EqualTo (CMSampleBufferError.None), "null");

						int i = 0;
						var result = sb.SetInvalidateCallback (delegate (CMSampleBuffer buffer) {
							i++;
							Assert.AreSame (buffer, sb, "same");
						});
						Assert.That (result, Is.EqualTo (CMSampleBufferError.None), "SetInvalidateCallback/None");

						// we can reset (nullify) the callback
						Assert.That (sb.SetInvalidateCallback (null), Is.EqualTo (CMSampleBufferError.None), "null-2");

						result = (CMSampleBufferError) sb.Invalidate ();
						Assert.That (result, Is.EqualTo (CMSampleBufferError.None), "Invalidate/None");
						Assert.That (i, Is.EqualTo (0), "0");
					}
				}
			}
		}

		[Test]
		public void CallForEachSample ()
		{
			using (var pixelBuffer = new CVPixelBuffer (20, 10, CVPixelFormatType.CV24RGB)) {
				CMFormatDescriptionError fde;
				using (var desc = CMVideoFormatDescription.CreateForImageBuffer (pixelBuffer, out fde)) {
					var sampleTiming = new CMSampleTimingInfo ();
					CMSampleBufferError sbe;
					using (var sb = CMSampleBuffer.CreateForImageBuffer (pixelBuffer, true, desc, sampleTiming, out sbe)) {
						int i = 0;
						var result = sb.CallForEachSample (delegate (CMSampleBuffer buffer, int index) {
							i++;
							Assert.AreSame (buffer, sb, "same-1");
							return CMSampleBufferError.CannotSubdivide;
						});
						Assert.That (result, Is.EqualTo (CMSampleBufferError.CannotSubdivide), "custom error");
						Assert.That (i, Is.EqualTo (1), "1");

						Assert.Throws<ArgumentNullException> (delegate {
							sb.CallForEachSample (null);
						}, "null");
					}
				}
			}
		}
	}
}

#endif // !__WATCHOS__
