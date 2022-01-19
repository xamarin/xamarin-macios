//
// Unit tests for CGContext
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

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
#if NO_NFLOAT_OPERATORS
			var lengths = new nfloat [] { new NFloat (1.0f), new NFloat (2.0f), new NFloat (3.0f) };
#else
			var lengths = new nfloat [] { 1.0f, 2.0f, 3.0f };
#endif
			using (var c = Create ()) {
#if NO_NFLOAT_OPERATORS
				c.SetLineDash (new NFloat (1.0f), lengths);
#else
				c.SetLineDash (1.0f, lengths);
#endif
				c.SetLineDash (2.0f, null);
#if NO_NFLOAT_OPERATORS
				c.SetLineDash (new NFloat (3.0f), lengths, 2);
#else
				c.SetLineDash (3.0f, lengths, 2);
#endif
				c.SetLineDash (4.0f, null, -1);
#if NO_NFLOAT_OPERATORS
				Assert.Throws<ArgumentException> (delegate { c.SetLineDash (new NFloat (5.0f), lengths, -1); }, "negative");
				Assert.Throws<ArgumentException> (delegate { c.SetLineDash (new NFloat (6.0f), lengths, Int32.MaxValue); }, "max");
#else
				Assert.Throws<ArgumentException> (delegate { c.SetLineDash (5.0f, lengths, -1); }, "negative");
				Assert.Throws<ArgumentException> (delegate { c.SetLineDash (6.0f, lengths, Int32.MaxValue); }, "max");
#endif
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
	}
}
