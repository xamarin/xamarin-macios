//
// Unit tests for CGContext
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;

using Foundation;
using CoreGraphics;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ContextTest {

		CGContext Create ()
		{
			using (CGColorSpace space = CGColorSpace.CreateDeviceRGB ()) {
				return new CGBitmapContext (null, 10, 10, 8, 40, space, CGBitmapFlags.PremultipliedLast);
			}
		}

		[Test]
		public void SetLineDash ()
		{
			var lengths = new nfloat [] { 1.0f, 2.0f, 3.0f };
			using (var c = Create ()) {
				c.SetLineDash (1.0f, lengths);
				c.SetLineDash (2.0f, null);
				c.SetLineDash (3.0f, lengths, 2);
				c.SetLineDash (4.0f, null, -1);
				Assert.Throws<ArgumentException> (delegate { c.SetLineDash (5.0f, lengths, -1); }, "negative");
				Assert.Throws<ArgumentException> (delegate { c.SetLineDash (6.0f, lengths, Int32.MaxValue); }, "max");
			}
		}

		[Test]
		public void ResetClip ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			// Merely tests that the P/Invoke is correct
			using (var c = Create ()) {
				var original = c.GetClipBoundingBox ();
				var rect = new CGRect (0, 0, 2, 2);
				c.ClipToRect (rect);
				Assert.That (rect, Is.EqualTo (c.GetClipBoundingBox ()));
				c.ResetClip ();
				Assert.That (original, Is.EqualTo (c.GetClipBoundingBox ()));
			}
		}

		[Test]
		public void EdrHeadroom ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var context = Create ();
			Assert.AreEqual (0.0f, context.GetEdrTargetHeadroom (), "a");
			Assert.IsTrue (context.SetEdrTargetHeadroom (2.0f), "b");
			Assert.AreEqual (2.0f, context.GetEdrTargetHeadroom (), "c");
			Assert.IsFalse (context.SetEdrTargetHeadroom (-2.0f), "d");
			Assert.AreEqual (2.0f, context.GetEdrTargetHeadroom (), "e");
		}

		[Test]
		public void DrawImageApplyingToneMapping ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			var imageFile = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using var dp = new CGDataProvider (imageFile);
			using var img = CGImage.FromPNG (dp, null, false, CGColorRenderingIntent.Default);
			var mapping = new CGToneMappingOptions () { Use100nitsHlgOotf = true, ExrToneMappingGammaExposure = 3.14f };

			using (var context = Create ()) {
				Assert.IsFalse (context.DrawImageApplyingToneMapping (new CGRect (0, 0, 10, 10), img, CGToneMapping.IturRecommended, (NSDictionary?) null), "DrawImageApplyingToneMapping A");
			}

			using (var context = Create ()) {
				Assert.IsFalse (context.DrawImageApplyingToneMapping (new CGRect (0, 0, 10, 10), img, CGToneMapping.IturRecommended, mapping), "DrawImageApplyingToneMapping B");
			}

			using (var context = Create ()) {
				Assert.IsFalse (context.DrawImageApplyingToneMapping (new CGRect (0, 0, 10, 10), img, CGToneMapping.IturRecommended, mapping.Dictionary), "DrawImageApplyingToneMapping C");
			}

		}
	}
}
