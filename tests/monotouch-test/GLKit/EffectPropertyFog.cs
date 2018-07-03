// Copyright 2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using GLKit;
#else
using MonoTouch.Foundation;
using MonoTouch.GLKit;
#endif
using OpenTK;
using NUnit.Framework;

namespace MonoTouchFixtures.GLKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EffectPropertyFogTest {
		
		[Test]
		public void Properties ()
		{
			TestRuntime.AssertMacSystemVersion (10, 8, throwIfOtherPlatform: false);

			var fog = new GLKEffectPropertyFog ();
			Assert.That (fog.Color.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "Color");
			
			fog = new GLKBaseEffect ().Fog;
			Assert.That (fog.Color.ToString (), Is.EqualTo ("(0, 0, 0, 0)"), "Color");
		}
	}
}

#endif // !__WATCHOS__
