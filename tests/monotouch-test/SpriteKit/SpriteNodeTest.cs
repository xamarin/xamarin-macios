//
// Unit tests for SKSpriteNode
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using SpriteKit;
#else
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
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

namespace MonoTouchFixtures.SpriteKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SpriteNodeTest {

		void CheckEmpty (SKSpriteNode n)
		{
			Assert.IsNotNull (n.Color, "Color");
			Assert.Null (n.Name, "Name");
			Assert.True (n.Size.IsEmpty, "Size");
			Assert.Null (n.Texture, "Texture");
		}

		[SetUp]
		public void VersionCheck ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Inconclusive ("requires iOS7+");
		}

		[Test]
		public void Ctor ()
		{
			using (var n = new SKSpriteNode ()) {
				CheckEmpty (n);
			}
		}

		[Test]
		public void CtorColor ()
		{
			using (var n = new SKSpriteNode ((UIColor) null, SizeF.Empty)) {
				CheckEmpty (n);
			}
		}

		[Test]
		public void CtorName ()
		{
			Assert.Throws<ArgumentNullException> (delegate { new SKSpriteNode ((string) null); }, "string");
		}

		[Test]
		public void CtorTexture ()
		{
			using (var n = new SKSpriteNode ((SKTexture) null)) {
				CheckEmpty (n);
			}
		}

		[Test]
		public void CtorTextureColor ()
		{
			using (var n = new SKSpriteNode (null, null, SizeF.Empty)) {
				CheckEmpty (n);
			}
		}

		[Test]
		public void Color ()
		{
			using (var n = new SKSpriteNode (UIColor.Blue, SizeF.Empty)) {
				Assert.That (n.Color.ToString (), Is.EqualTo ("UIColor [A=255, R=0, G=0, B=255]"), "Color-1");
				n.Color = null;
				Assert.That (n.Color.ToString (), Is.EqualTo ("UIColor [A=0, R=0, G=0, B=0]"), "Color-2");
			}
		}

		[Test]
		public void Texture ()
		{
			using (var t = SKTexture.FromImageNamed ("basn3p08.png"))
			using (var n = new SKSpriteNode (t)) {
				Assert.AreSame (n.Texture, t, "Texture-1");
				n.Texture = null;
				Assert.IsNull (n.Texture, "Texture-2");
			}
		}
	}
}

#endif // !__WATCHOS__
