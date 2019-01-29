// Copyright 2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using GLKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.GLKit;
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

namespace MonoTouchFixtures.GLKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BaseEffectTest {
		
		[Test]
		[Culture ("en")]
		public void Properties ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);

			var effect = new GLKBaseEffect ();
			Assert.That (effect.LightModelAmbientColor.ToString (), Is.EqualTo ("(0.2, 0.2, 0.2, 1)"), "LightModelAmbientColor");
			Assert.That (effect.ConstantColor.ToString (), Is.EqualTo ("(1, 1, 1, 1)"), "ConstantColor");

			effect.Light0.Enabled = true;
			effect.Light0.DiffuseColor = new Vector4 (1.0f, 0.4f, 0.4f, 1.0f);
			Assert.That (effect.Light0.DiffuseColor.ToString (), Is.EqualTo ("(1, 0.4, 0.4, 1)"), "Light0");
		}
	}
}
#endif // !__WATCHOS__
