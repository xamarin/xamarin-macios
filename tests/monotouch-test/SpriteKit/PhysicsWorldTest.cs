
#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using SpriteKit;
#else
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;
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
	public class PhysicsWorldTest {
		[SetUp]
		public void VersionCheck ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);
		}

		[Test]
		public void SampleFields ()
		{
			using (var scene = new SKScene ()) {
				using (var world = scene.PhysicsWorld) {
					var v = world.SampleFields (new Vector3 (1, 2, 3));
					Assert.AreEqual (0, v.X, "#x1");
					Assert.AreEqual (0, v.Y, "#y1");
					Assert.AreEqual (0, v.Z, "#z1");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
