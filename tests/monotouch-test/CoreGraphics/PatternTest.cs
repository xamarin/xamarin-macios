//
// Unit tests for CGPattern
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2019 Microsoft Corp. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
#if __MACOS__
using AppKit;
using UIImage = AppKit.NSImage;
using UIColor = AppKit.NSColor;
#else
using UIKit;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PatternTest {
		[Test]
		public void DrawTest ()
		{
			// Render an image with a pattern, and show it briefly on screen.
			// It's supposed to show a blue oval with green plus signs inside.
			TestRuntime.RunAsync (TimeSpan.FromSeconds (0.1), () => { }, () => true, GetRenderedPattern ());
		}

		public UIImage GetRenderedPattern ()
		{
			var patternBounds = new CGRect (0, 0, 12, 12);
			var bounds = new CGRect (0, 0, patternBounds.Width * patternBounds.Width, patternBounds.Height * patternBounds.Height);
#if __MACOS__
			var img = new NSImage (bounds.Size);
			img.LockFocus ();
			var context = NSGraphicsContext.CurrentContext.GraphicsPort;
#else
			UIGraphics.BeginImageContextWithOptions (bounds.Size, false, 0);
			var context = UIGraphics.GetCurrentContext ();
#endif
			try {
				context.SetStrokeColor (UIColor.Blue.CGColor);
				context.AddEllipseInRect (bounds);

				using (var patternColorSpace = CGColorSpace.CreatePattern (null)) {
					context.SetFillColorSpace (patternColorSpace);

					using (var pattern = new CGPattern (patternBounds, CGAffineTransform.MakeIdentity (), patternBounds.Width, patternBounds.Height, CGPatternTiling.ConstantSpacing, true, ctx => {
						ctx.SetStrokeColor (UIColor.Green.CGColor);
						ctx.SetLineWidth (1);
						ctx.MoveTo (2, 6);
						ctx.AddLineToPoint (10, 6);
						ctx.MoveTo (6, 2);
						ctx.AddLineToPoint (6, 10);
						ctx.StrokePath ();
					})) {
						context.SetFillPattern (pattern, new nfloat [] { 1 });
					}
				}

				context.DrawPath (CGPathDrawingMode.FillStroke);

#if __MACOS__
				return img;
#else
				return UIGraphics.GetImageFromCurrentImageContext ();
#endif
			} finally {
#if __MACOS__
				img.UnlockFocus ();
#else
				UIGraphics.EndImageContext ();
#endif
			}
		}
	}
}
