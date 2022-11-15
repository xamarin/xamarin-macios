// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
using CoreGraphics;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NavigationBarTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UINavigationBar nb = new UINavigationBar (frame)) {
				Assert.That (nb.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void BackgroundImage ()
		{
			// http://stackoverflow.com/q/10504966/220643
			using (UINavigationBar nb = new UINavigationBar ()) {
				Assert.Null (nb.GetBackgroundImage (UIBarMetrics.Default), "Get");
				nb.SetBackgroundImage (null, UIBarMetrics.Default);
			}
		}
	}
}

#endif // !__WATCHOS__
