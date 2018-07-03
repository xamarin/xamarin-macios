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
#if XAMCORE_2_0
using CoreAnimation;
using Foundation;
using SceneKit;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.SceneKit;
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

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NodeTest {

		[Test]
		public void AddAnimation ()
		{
			TestRuntime.AssertiOSSystemVersion (8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertMacSystemVersion (10, 8, throwIfOtherPlatform: false);

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
			TestRuntime.AssertXcodeVersion (9,0);

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
