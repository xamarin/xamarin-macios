//
// Unit tests for CGFunction
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !MONOMAC
using System;
using Foundation;
using CoreGraphics;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FunctionTest {
#if !__WATCHOS__ // FIXME: It doesn't look like this test needs to use UIKit, so it might be possible to rewrite it to run on WatchOS as well.
		[Test]
		public void Test ()
		{
			bool tested = false;
			using (var vc = new UIViewController ()) {
				vc.View = new CustomView () {
					BackgroundColor = UIColor.Green,
					Shaded = () => tested = true,
				};
				AppDelegate.PresentModalViewController (vc, 0.1);
			}

			if (!tested)
				Assert.Inconclusive ("The Shading callback wasn't called.");
		}

		class CustomView : UIView {
			public Action Shaded;
			public unsafe override void Draw (CGRect rect)
			{
				var start = new CGPoint (rect.Left, rect.Bottom);
				var end = new CGPoint (rect.Left, rect.Top);

				var domain = new nfloat [] { 0f, 1f };
				var range = new nfloat [] { 0f, 1f, 0f, 1f };
				using (var context = UIGraphics.GetCurrentContext ())
				using (var rgb = CGColorSpace.CreateDeviceGray ())
				using (var shadingFunction = new CGFunction (domain, range, Shading))
				using (var shading = CGShading.CreateAxial (rgb, start, end, shadingFunction, true, false)) {
					context.DrawShading (shading);
				}

				base.Draw (rect);
			}

			public unsafe void Shading (nfloat* data, nfloat* outData)
			{
				var p = data [0];
				outData [0] = 0.0f;
				outData [1] = (1.0f - Slope (p, 2.0f)) * 0.5f;
				Shaded ();
			}

			public nfloat Slope (nfloat x, nfloat A)
			{
				var p = Math.Pow (x, A);
				return (nfloat) (p / (p + Math.Pow (1.0f - x, A)));
			}
		}

		[Test]
		public void CoreGraphicsStrongDictionary ()
		{
			var rect = new CGRect (10, 10, 100, 100);
			var size = new CGSize (200, 200);
			var point = new CGPoint (10, 20);
			var graphicsDict = new GraphicsDict {
				Rect = rect,
				Size = size,
				Point = point
			};

			var retrievedRect = graphicsDict.Rect;
			Assert.IsTrue (rect == retrievedRect, "CoreGraphicsStrongDictionary CGRect");

			var retrievedSize = graphicsDict.Size;
			Assert.IsTrue (size == retrievedSize, "CoreGraphicsStrongDictionary CGSize");

			var retrievedPoint = graphicsDict.Point;
			Assert.IsTrue (point == retrievedPoint, "CoreGraphicsStrongDictionary CGPoint");
		}

		class GraphicsDict : DictionaryContainer {
			static NSString RectKey = new NSString ("RectKey");
			public CGRect? Rect {
				get { return GetCGRectValue (RectKey); }
				set { SetCGRectValue (RectKey, value); }
			}

			static NSString SizeKey = new NSString ("SizeKey");
			public CGSize? Size {
				get { return GetCGSizeValue (SizeKey); }
				set { SetCGSizeValue (SizeKey, value); }
			}

			static NSString PointKey = new NSString ("PointKey");
			public CGPoint? Point {
				get { return GetCGPointValue (PointKey); }
				set { SetCGPointValue (PointKey, value); }
			}
		}

#endif // !__WATCHOS__
	}
}
#endif
