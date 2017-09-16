//
// Unit tests for GKPath
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using OpenTK;

#if XAMCORE_2_0
using Foundation;
using GameplayKit;
#else
using MonoTouch.Foundation;
using MonoTouch.GameplayKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.GamePlayKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKPathTests {
		
		Vector2[] points = new Vector2[] { 
			new Vector2 (0,0), new Vector2 (0,1), new Vector2 (0,2), new Vector2 (0,3),
			new Vector2 (1,3), new Vector2 (1,2), new Vector2 (1,1), new Vector2 (1,0)
		};

		[Test]
		public void FromPointsTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (9, 0))
				Assert.Ignore ("Ignoring GameplayKit tests: Requires iOS9+");
			
			var path = GKPath.FromPoints (points, 1, false);
			Assert.NotNull (path, "GKPath.FromPoints should not be null");
		}

		[Test]
		public void InitWithPointsTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (9, 0))
				Assert.Ignore ("Ignoring GameplayKit tests: Requires iOS9+");

			var path = new GKPath (points, 1, false);
			Assert.NotNull (path, "GKPath.FromPoints should not be null");
		}
	}
}
	
#endif // __WATCHOS__
