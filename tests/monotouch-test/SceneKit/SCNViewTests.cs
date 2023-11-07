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

using Foundation;
using SceneKit;
using CoreGraphics;

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SCNViewTests {

		[Test]
		public void NullOverlaySceneTest ()
		{
			// Issue: https://github.com/xamarin/xamarin-macios/issues/3392
			TestRuntime.AssertXcodeVersion (7, 0);
			TestRuntime.AssertNotVirtualMachine ();

			var view = new SCNView (new CGRect (), (NSDictionary) null);
			Assert.NotNull (view, "View not null");
			Assert.DoesNotThrow (() => view.OverlayScene = null, "Should not throw");
		}
	}
}
#endif
