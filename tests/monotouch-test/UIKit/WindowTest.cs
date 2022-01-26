// Copyright 2011-2013 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class WindowTest {
		
		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UIWindow w = new UIWindow (frame)) {
				Assert.That (w.Frame, Is.EqualTo (frame), "Frame");
			}
		}
		
		[Test]
		public void Convert_Null ()
		{
			using (UIWindow w = new UIWindow ()) {
				Assert.That (w.ConvertPointFromWindow (CGPoint.Empty, null), Is.EqualTo (CGPoint.Empty), "ConvertPointFromWindow");
				Assert.That (w.ConvertPointToWindow (CGPoint.Empty, null), Is.EqualTo (CGPoint.Empty), "ConvertPointToWindow");
				Assert.That (w.ConvertRectFromWindow (CGRect.Empty, null), Is.EqualTo (CGRect.Empty), "ConvertRectFromWindow");
				Assert.That (w.ConvertRectToWindow (CGRect.Empty, null), Is.EqualTo (CGRect.Empty), "ConvertRectToWindow");
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
#if NO_NFLOAT_OPERATORS
				Assert.That (w.WindowLevel, Is.EqualTo (new NFloat (0f)), "default");
#else
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 0f), "default");
#endif
				w.WindowLevel = UIWindowLevel.Normal;
#if NO_NFLOAT_OPERATORS
				Assert.That (w.WindowLevel, Is.EqualTo (new NFloat (0f)), "Normal");
#else
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 0f), "Normal");
#endif
				w.WindowLevel = UIWindowLevel.Alert;
#if NO_NFLOAT_OPERATORS
				Assert.That (w.WindowLevel, Is.EqualTo (new NFloat (2000f)), "Alert");
#else
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 2000f), "Alert");
#endif
#if !__TVOS__
				w.WindowLevel = UIWindowLevel.StatusBar;
#if NO_NFLOAT_OPERATORS
				Assert.That (w.WindowLevel, Is.EqualTo (new NFloat (1000f)), "StatusBar");
#else
				Assert.That (w.WindowLevel, Is.EqualTo ((nfloat) 1000f), "StatusBar");
#endif
#endif // !__TVOS__
			}
		}
	}
}

#endif // !__WATCHOS__
