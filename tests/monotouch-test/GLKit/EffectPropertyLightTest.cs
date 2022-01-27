// Copyright 2012 Xamarin Inc. All rights reserved

#if HAS_GLKIT

using System;
using System.Drawing;
using Foundation;
using GLKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.GLKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EffectPropertyLightTest {
		
		[Test]
		public void Properties ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			var light = new GLKEffectPropertyLight ();
#if NET
			Assert.That (light.AmbientColor.ToString (), Is.EqualTo ("<0, 0, 0, 0>"), "AmbientColor");
			Assert.That (light.DiffuseColor.ToString (), Is.EqualTo ("<0, 0, 0, 0>"), "DiffuseColor");
			Assert.That (light.SpecularColor.ToString (), Is.EqualTo ("<0, 0, 0, 0>"), "SpecularColor");
			Assert.That (light.Position.ToString (), Is.EqualTo ("<0, 0, 0, 0>"), "Position");
#else
			Assert.That (light.AmbientColor.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "AmbientColor");
			Assert.That (light.DiffuseColor.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "DiffuseColor");
			Assert.That (light.SpecularColor.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "SpecularColor");
			Assert.That (light.Position.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "Position");
#endif

			light = new GLKBaseEffect ().Light0;
#if NET
			Assert.That (light.AmbientColor.ToString (), Is.EqualTo ("<0, 0, 0, 1>"), "AmbientColor");
			Assert.That (light.DiffuseColor.ToString (), Is.EqualTo ("<1, 1, 1, 1>"), "DiffuseColor");
			Assert.That (light.SpecularColor.ToString (), Is.EqualTo ("<1, 1, 1, 1>"), "SpecularColor");
			Assert.That (light.Position.ToString (), Is.EqualTo ("<0, 0, 1, 0>"), "Position");
#else
			Assert.That (light.AmbientColor.ToString (), Is.EqualTo ("(0, 0, 0, 1)"), "AmbientColor");
			Assert.That (light.DiffuseColor.ToString (), Is.EqualTo ("(1, 1, 1, 1)"), "DiffuseColor");
			Assert.That (light.SpecularColor.ToString (), Is.EqualTo ("(1, 1, 1, 1)"), "SpecularColor");
			Assert.That (light.Position.ToString (), Is.EqualTo ("(0, 0, 1, 0)"), "Position");
#endif
		}
	}
}

#endif // HAS_GLKIT
