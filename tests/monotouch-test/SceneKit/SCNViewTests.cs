//
// SCNViewTests.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2018 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__
using System;
using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using SceneKit;
using CoreGraphics;
#else
using MonoTouch.Foundation;
using MonoTouch.SceneKit;
using CGRect = System.Drawing.RectangleF;
#endif

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SCNViewTests {

		[Test]
		public void NullOverlaySceneTest ()
		{
			// Issue: https://github.com/xamarin/xamarin-macios/issues/3392
			TestRuntime.AssertXcodeVersion (6,0);

			var view = new SCNView (new CGRect (), (NSDictionary) null);
			Assert.NotNull (view, "View not null");
			Assert.DoesNotThrow (() => view.OverlayScene = null, "Should not throw");
		}
	}
}
#endif
