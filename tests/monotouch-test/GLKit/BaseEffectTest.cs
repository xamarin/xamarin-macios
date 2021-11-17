// Copyright 2012 Xamarin Inc. All rights reserved

#if HAS_GLKIT

using System;
using System.Drawing;
using Foundation;
using GLKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

#if NET
using System.Numerics;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.GLKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BaseEffectTest {
		
		[Test]
		[Culture ("en")]
		public void Properties ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			var effect = new GLKBaseEffect ();
			Assert.That (effect.LightModelAmbientColor.ToString (), Is.EqualTo ("(0.2, 0.2, 0.2, 1)"), "LightModelAmbientColor");
			Assert.That (effect.ConstantColor.ToString (), Is.EqualTo ("(1, 1, 1, 1)"), "ConstantColor");

			effect.Light0.Enabled = true;
			effect.Light0.DiffuseColor = new Vector4 (1.0f, 0.4f, 0.4f, 1.0f);
			Assert.That (effect.Light0.DiffuseColor.ToString (), Is.EqualTo ("(1, 0.4, 0.4, 1)"), "Light0");
		}
	}
}
#endif // HAS_GLKIT
