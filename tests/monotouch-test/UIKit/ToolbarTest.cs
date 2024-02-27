// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ToolbarTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UIToolbar tb = new UIToolbar (frame)) {
				Assert.That (tb.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void BackgroundImage ()
		{
			using (UIToolbar tb = new UIToolbar ()) {
				Assert.Null (tb.GetBackgroundImage (UIToolbarPosition.Any, UIBarMetrics.Default), "Get");
				tb.SetBackgroundImage (null, UIToolbarPosition.Any, UIBarMetrics.Default);
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
