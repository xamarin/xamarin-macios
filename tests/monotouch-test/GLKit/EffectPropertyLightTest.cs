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
	public class EffectPropertyLightTest {
		
		[Test]
		public void Properties ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);

			var light = new GLKEffectPropertyLight ();
			Assert.That (light.AmbientColor.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "AmbientColor");
			Assert.That (light.DiffuseColor.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "DiffuseColor");
			Assert.That (light.SpecularColor.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "SpecularColor");
			Assert.That (light.Position.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "Position");

			light = new GLKBaseEffect ().Light0;
			Assert.That (light.AmbientColor.ToString (), Is.EqualTo ("(0, 0, 0, 1)"), "AmbientColor");
			Assert.That (light.DiffuseColor.ToString (), Is.EqualTo ("(1, 1, 1, 1)"), "DiffuseColor");
			Assert.That (light.SpecularColor.ToString (), Is.EqualTo ("(1, 1, 1, 1)"), "SpecularColor");
			Assert.That (light.Position.ToString (), Is.EqualTo ("(0, 0, 1, 0)"), "Position");
		}
	}
}

#endif // !__WATCHOS__
