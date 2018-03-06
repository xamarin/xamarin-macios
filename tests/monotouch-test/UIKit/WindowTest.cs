// Copyright 2011-2013 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class WindowTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UIWindow w = new UIWindow (frame)) {
				Assert.That (w.Frame, Is.EqualTo (frame), "Frame");
			}
		}
		
		[Test]
		public void Convert_Null ()
		{
			using (UIWindow w = new UIWindow ()) {
				Assert.That (w.ConvertPointFromWindow (PointF.Empty, null), Is.EqualTo (PointF.Empty), "ConvertPointFromWindow");
				Assert.That (w.ConvertPointToWindow (PointF.Empty, null), Is.EqualTo (PointF.Empty), "ConvertPointToWindow");
				Assert.That (w.ConvertRectFromWindow (RectangleF.Empty, null), Is.EqualTo (RectangleF.Empty), "ConvertRectFromWindow");
				Assert.That (w.ConvertRectToWindow (RectangleF.Empty, null), Is.EqualTo (RectangleF.Empty), "ConvertRectToWindow");
			}
		}
		
		[Test]
		public void IsKeyWindow_5199 ()
		{
			using (UIWindow w = new UIWindow ()) {
				Assert.False (w.IsKeyWindow, "IsKeyWindow");
			}
		}

		[Test]
		public void Level ()
		{
			using (UIWindow w = new UIWindow ()) {
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 0f), "default");
				w.WindowLevel = UIWindowLevel.Normal;
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 0f), "Normal");
				w.WindowLevel = UIWindowLevel.Alert;
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 2000f), "Alert");
#if !__TVOS__
				w.WindowLevel = UIWindowLevel.StatusBar;
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 1000f), "StatusBar");
#endif // !__TVOS__
			}
		}
	}
}

#endif // !__WATCHOS__
