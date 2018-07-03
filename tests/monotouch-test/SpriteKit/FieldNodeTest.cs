
#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
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
	public class FieldNodeTest {
		[SetUp]
		public void VersionCheck ()
		{
			TestRuntime.AssertiOSSystemVersion (8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertMacSystemVersion (10, 10, throwIfOtherPlatform: false);
		}

		[Test]
		public void CreateRadialGravityField ()
		{
			using (SKFieldNode node = SKFieldNode.CreateRadialGravityField ()) {

				var v = new Vector4 (1, 2, 3, 4);
				node.Direction = v;
				Assert.AreEqual (node.Direction.X, 1, "#x1");
				Assert.AreEqual (node.Direction.Y, 2, "#y1");
				Assert.AreEqual (node.Direction.Z, 3, "#z1");
				Assert.AreEqual (node.Direction.W, 0, "#w1");

				v = node.Direction;
				Assert.AreEqual (v.X, 1, "#x2");
				Assert.AreEqual (v.Y, 2, "#y2");
				Assert.AreEqual (v.Z, 3, "#z2");
				Assert.AreEqual (v.W, 0, "#w2");
			}
		}

		[Test]
		public void CreateLinearGravityField ()
		{
			using (var node = SKFieldNode.CreateLinearGravityField (new Vector4 (1, 2, 3, 4))) {

				Assert.AreEqual (0.00457763672f, node.MinimumRadius, "#minimum radius");
			}
		}

		[Test]
		public void CreateVelocityField ()
		{
			using (var node = SKFieldNode.CreateVelocityField (new Vector4 (1, 2, 3, 4))) {

				Assert.AreEqual (0.00457763672f, node.MinimumRadius, "#minimum radius");
			}
		}

		[Test]
		public void CreateCustomField ()
		{
			using (var node = SKFieldNode.CreateCustomField ((Vector4 position, Vector4 velocity, float mass, float charge, double time) => {
				return new Vector3 (3, 1, 4);
			})) {

				// FIXME: the code below doesn't end up calling the anonymous delegate above.
				var v = node.Direction;
				Assert.AreEqual (0, v.X, "#x2");
				Assert.AreEqual (0, v.Y, "#y2");
				Assert.AreEqual (0, v.Z, "#z2");
				Assert.AreEqual (0, v.W, "#w2");
			}
		}
	}
}

#endif // !__WATCHOS__
