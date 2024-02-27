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
	public class EffectPropertyFogTest {

		[Test]
		public void Properties ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			var fog = new GLKEffectPropertyFog ();
			Asserts.AreEqual (0, 0, 0, 0, fog.Color, "Color");

			fog = new GLKBaseEffect ().Fog;
			Asserts.AreEqual (0, 0, 0, 0, fog.Color, "Color 2");
		}
	}
}

#endif // HAS_GLKIT
