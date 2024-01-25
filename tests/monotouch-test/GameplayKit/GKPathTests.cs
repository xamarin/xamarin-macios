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

using Foundation;

using GameplayKit;

using NUnit.Framework;
#if NET
using System.Numerics;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.GamePlayKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKPathTests {

		Vector2 [] points = new Vector2 [] {
			new Vector2 (0,0), new Vector2 (0,1), new Vector2 (0,2), new Vector2 (0,3),
			new Vector2 (1,3), new Vector2 (1,2), new Vector2 (1,1), new Vector2 (1,0)
		};

		static Vector3 [] test_vectors3 = new [] {
			new Vector3 (0.1532144f, 0.5451511f, 0.2004739f),
			new Vector3 (0.7717745f, 0.559364f, 0.00918373f),
			new Vector3 (0.2023053f, 0.4701468f, 0.6618567f),
			new Vector3 (0.4904693f, 0.841727f, 0.2294401f),
			new Vector3 (0.1252193f, 0.08986127f, 0.3407605f),
			new Vector3 (0.006755914f, 0.07464754f, 0.287938f),
			new Vector3 (9.799572E+08f, 1.64794E+09f, 1.117296E+09f),
			new Vector3 (1.102396E+09f, 3.082477E+08f, 1.126484E+09f),
			new Vector3 (2.263112E+08f, 8.79644E+08f, 1.303282E+09f),
			new Vector3 (8.176959E+08f, 1.386156E+09f, 5.956444E+08f),
		};

		[Test]
		public void FromPointsTest ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			var path = GKPath.FromPoints (points, 1, false);
			Assert.NotNull (path, "GKPath.FromPoints should not be null");
		}

		[Test]
		public void InitWithPointsTest ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			var path = new GKPath (points, 1, false);
			Assert.NotNull (path, "GKPath.FromPoints should not be null");
		}

		[Test]
		public void FromPointsVector3Test ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var path = GKPath.FromPoints (test_vectors3, 1, false);
			Assert.NotNull (path, "GKPath.FromPoints should not be null");

			for (int i = 0; i < test_vectors3.Length; i++)
				Asserts.AreEqual (path.GetVector3Point ((nuint) i), test_vectors3 [i], $"FromPointsVector3 iter {i}");
		}

		[Test]
		public void InitWithPointsVector3Test ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			var path = new GKPath (test_vectors3, 1, false);
			Assert.NotNull (path, "GKPath.FromPoints should not be null");

			for (int i = 0; i < test_vectors3.Length; i++)
				Asserts.AreEqual (path.GetVector3Point ((nuint) i), test_vectors3 [i], $"InitWithVector3 iter {i}");
		}
	}
}

#endif // __WATCHOS__
