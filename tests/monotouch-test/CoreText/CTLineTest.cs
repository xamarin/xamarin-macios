//
// Unit tests for CTStringAttributes
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
#else
using UIKit;
#endif
using CoreGraphics;
using CoreText;
using System.Drawing;

namespace MonoTouchFixtures.CoreText {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CTLineTests {
		[TestCase (false)]
		[TestCase (true)]
		public void EnumerateCaretOffsetsKeepsLineAlive (bool stopAfterFirst)
		{
			TestRuntime.AssertXcodeVersion (7, 0);
			using var text = new NSAttributedString ("Hello");
			var line = new CTLine (text);
			var reference = new WeakReference (line);
			var handle = line.Handle;
			var alive = true;
			var count = 0;

			// An independent native retain makes a lifetime regression fail without crashing.
			TestRuntime.CFRetain (handle);
			try {
				line.EnumerateCaretOffsets ((double offset, nint charIndex, bool leadingEdge, ref bool stop) => {
					GC.Collect ();
					GC.WaitForPendingFinalizers ();
					alive &= reference.IsAlive;
					count++;
					stop = stopAfterFirst;
				});
				Assert.That (alive, Is.True, "Line must remain alive during enumeration");
				Assert.That (count, stopAfterFirst ? Is.EqualTo (1) : Is.GreaterThan (1), "Callbacks");
			} finally {
				TestRuntime.CFRelease (handle);
			}
		}

		[Test]
		public void EnumerateCaretOffsets ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Requires iOS9+ or macOS 10.11+");

			var sa = new CTStringAttributes ();
			sa.ForegroundColor = TestRuntime.GetCGColor (UIColor.Blue);
			sa.Font = new CTFont ("Georgia-BoldItalic", 24);
			sa.UnderlineStyle = CTUnderlineStyle.Double; // It does not seem to do anything
			sa.UnderlineColor = TestRuntime.GetCGColor (UIColor.Blue);
			sa.UnderlineStyleModifiers = CTUnderlineStyleModifiers.PatternDashDotDot;

			var attributedString = new NSAttributedString ("Hello world.\nWoohooo!\nThere", sa);

			var line = new CTLine (attributedString);
			bool executed = false;
			line.EnumerateCaretOffsets ((double o, nint charIndex, bool leadingEdge, ref bool stop) => {
				executed = true;
			});
			Assert.That (executed, Is.True);
		}

		[Test]
		public void GetImageBounds ()
		{
			using (var a = new NSAttributedString ())
			using (var l = new CTLine (a)) {
				Assert.That (l.GetImageBounds (null).IsEmpty, Is.True, "GetImageBounds");
			}
		}
	}
}
