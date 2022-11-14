//
// Unit tests for GKNoise
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//	
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using NUnit.Framework;

using Foundation;
using GameplayKit;

#if NET
using Vector3d = global::CoreGraphics.NVector3d;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.GamePlayKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKNoiseTests {

		[Test]
		public void Vector3dTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (8, 0))
				Assert.Ignore ("Ignoring GameplayKit tests: Requires iOS10+");

			using (var noise = new GKNoise (GKCheckerboardNoiseSource.Create (5))) {
				var delta = new Vector3d (1 / 8, 0, 1 / 3);
				Assert.DoesNotThrow (() => noise.Move (delta), "Move");

				var radians = new Vector3d (0, Math.PI / 3, 0);
				Assert.DoesNotThrow (() => noise.Rotate (radians), "Rotate");

				var factor = new Vector3d (2, 1, 1.5);
				Assert.DoesNotThrow (() => noise.Scale (factor), "Scale");
			}
		}
	}
}
#endif
