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

namespace MonoTouchFixtures.GLKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EffectPropertytMaterialTest {
		
		[Test]
		[Culture ("en")]
		public void Properties ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);

			var material = new GLKEffectPropertyMaterial ();
			Assert.That (material.AmbientColor.ToString (), Is.EqualTo ("(0.2, 0.2, 0.2, 1)"), "AmbientColor");
			Assert.That (material.DiffuseColor.ToString (), Is.EqualTo ("(0.8, 0.8, 0.8, 1)"), "DiffuseColor");
			Assert.That (material.SpecularColor.ToString (), Is.EqualTo ("(0, 0, 0, 1)"), "SpecularColor");
			Assert.That (material.EmissiveColor.ToString (), Is.EqualTo ("(0, 0, 0, 1)"), "EmissiveColor");

			material = new GLKBaseEffect ().Material;
			Assert.That (material.AmbientColor.ToString (), Is.EqualTo ("(0.2, 0.2, 0.2, 1)"), "AmbientColor");
			Assert.That (material.DiffuseColor.ToString (), Is.EqualTo ("(0.8, 0.8, 0.8, 1)"), "DiffuseColor");
			Assert.That (material.SpecularColor.ToString (), Is.EqualTo ("(0, 0, 0, 1)"), "SpecularColor");
			Assert.That (material.EmissiveColor.ToString (), Is.EqualTo ("(0, 0, 0, 1)"), "EmissiveColor");
		}
	}
}

#endif // !__WATCHOS__
