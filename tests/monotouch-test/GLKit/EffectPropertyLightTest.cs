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
			Asserts.AreEqual (0, 0, 0, 0, light.AmbientColor, "AmbientColor");
			Asserts.AreEqual (0, 0, 0, 0, light.DiffuseColor, "DiffuseColor");
			Asserts.AreEqual (0, 0, 0, 0, light.SpecularColor, "SpecularColor");
			Asserts.AreEqual (0, 0, 0, 0, light.Position, "Position");

			light = new GLKBaseEffect ().Light0;
			Asserts.AreEqual (0, 0, 0, 1, light.AmbientColor, "AmbientColor");
			Asserts.AreEqual (1, 1, 1, 1, light.DiffuseColor, "DiffuseColor");
			Asserts.AreEqual (1, 1, 1, 1, light.SpecularColor, "SpecularColor");
			Asserts.AreEqual (0, 0, 1, 0, light.Position, "Position");
		}
	}
}

#endif // HAS_GLKIT
