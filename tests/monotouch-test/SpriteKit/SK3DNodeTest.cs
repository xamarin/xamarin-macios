
#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
#if !MONOMAC
using UIKit;
#endif
using SpriteKit;
using ObjCRuntime;
using SceneKit;
#else
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.SceneKit;
#endif
using OpenTK;
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

namespace MonoTouchFixtures.SpriteKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SK3DNodeTest {
		[SetUp]
		public void VersionCheck ()
		{
			TestRuntime.AssertiOSSystemVersion (8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertMacSystemVersion (10, 10, throwIfOtherPlatform: false);
		}

		[Test]
		public void ProjectPoint ()
		{
			if (TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("This doesn't seem to work properly in the iOS 9+ or macOS 10.11+");

			// SK3Node loads SCNRenderer dynamically, so make sure it's actually loaded.
			GC.KeepAlive (Class.GetHandle (typeof(SCNRenderer)));

			using (var node = new SK3DNode ()) {
#if !MONOMAC
				if (Runtime.Arch == Arch.SIMULATOR && IntPtr.Size == 4) {
					// 32-bit simulator returns 0,0,0 the first time
					// this is executed for some reason, so just
					// ignore that.
					node.ProjectPoint (new Vector3 (4, 5, 6));
				}
#endif
				var v = node.ProjectPoint (new Vector3 (1, 2, 3));
				Assert.AreEqual (1, v.X, "#x1");
				Assert.AreEqual (2, v.Y, "#y1");
				Assert.AreEqual (3, v.Z, "#z1");
			}
		}

		[Test]
		public void UnprojectPoint ()
		{
#if MONOMAC
			Assert.Ignore ("This doesn't seem to work properly in macOS 10.12");
#elif __IOS__ || __TVOS__
			if (TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("This doesn't seem to work properly in the iOS 9");
#endif

			using (var node = new SK3DNode ()) {
#if !MONOMAC
				if (Runtime.Arch == Arch.SIMULATOR && IntPtr.Size == 4) {
					// 32-bit simulator returns 0,0,0 the first time
					// this is executed for some reason, so just
					// ignore that.
					node.UnprojectPoint (new Vector3 (4, 5, 6));
				}
#endif
				var v = node.UnprojectPoint (new Vector3 (1, 2, 3));
				Assert.AreEqual (1, v.X, "#x1");
				Assert.AreEqual (2, v.Y, "#y1");
				Assert.AreEqual (3, v.Z, "#z1");
			}
		}
	}
}

#endif // !__WATCHOS__
