//
// Unit tests for GKNoiseMap
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
using Vector2d = global::CoreGraphics.NVector2d;
using Vector2i = global::CoreGraphics.NVector2i;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.GamePlayKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKNoiseMapTests {

		[Test]
		public void Vector2dTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (8, 0))
				Assert.Ignore ("Ignoring GameplayKit tests: Requires iOS10+");

			var size = new Vector2d (1.0, 1.0);
			var origin = new Vector2d (0, 0);
			var sample = new Vector2i (100, 100);

			using (var noise = new GKNoise (GKCylindersNoiseSource.Create (1)))
			using (var baseMap = new GKNoiseMap (noise))
			using (var map = new GKNoiseMap (noise, size, origin, sample, false)) {
				Assert.NotNull (baseMap, "baseMap is null");
				Assert.NotNull (map, "baseMap is null");

				Assert.AreEqual (size, map.Size, "map size is different");
				Assert.AreEqual (size, baseMap.Size, "baseMap size is different");
				Assert.AreEqual (origin, map.Origin, "map origin is different");
				Assert.AreEqual (origin, baseMap.Origin, "baseMap origin is different");
				Assert.AreEqual (sample, map.SampleCount, "map sample is different");
				Assert.AreEqual (sample, baseMap.SampleCount, "baseMap sample is different");
			}
		}
	}
}
#endif
