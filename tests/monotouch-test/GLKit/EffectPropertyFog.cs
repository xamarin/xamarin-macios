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
#if NET
			Assert.That (fog.Color.ToString (), Is.EqualTo ("<0, 0, 0, 0>"), "Color");
#else
			Assert.That (fog.Color.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "Color");
#endif
			
			fog = new GLKBaseEffect ().Fog;
#if NET
			Assert.That (fog.Color.ToString (), Is.EqualTo ("<0, 0, 0, 0>"), "Color");
#else
			Assert.That (fog.Color.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "Color");
#endif
		}
	}
}

#endif // HAS_GLKIT
