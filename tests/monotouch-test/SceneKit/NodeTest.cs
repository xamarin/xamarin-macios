//
// Unit tests for SCNNode
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using CoreAnimation;
using Foundation;
using SceneKit;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NodeTest {

		[Test]
		public void AddAnimation ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			using (var a = CAAnimation.CreateAnimation ())
			using (var n = SCNNode.Create ()) {
				n.AddAnimation (a, (NSString) null);
				n.AddAnimation (a, (string) null);
				string key = "key";
				n.AddAnimation (a, key);
				using (var s = new NSString (key))
					n.AddAnimation (a, s);
			}
		}

		[Test]
		public void AddAnimation_Overload ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			using (var ca = CAAnimation.CreateAnimation ())
			using (var a = SCNAnimation.FromCAAnimation (ca))
			using (var n = SCNNode.Create ()) {
				n.AddAnimation (a, "key");
				n.RemoveAllAnimations ();

				n.AddAnimation (a, null);
				n.RemoveAllAnimations ();
			}
		}
	}
}

#endif // !__WATCHOS__
