//
// Unit tests for NSDirectionalEdgeInsets
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft. All rights reserved.
//

#if XAMCORE_2_0 && !MONOMAC

using System;
using System.Drawing;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DirectionalEdgeInsetsTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

		[Test]
		public void FromString_Null ()
		{
			var e = NSDirectionalEdgeInsets.FromString (null);
			Assert.That (e, Is.EqualTo (NSDirectionalEdgeInsets.Zero), "roundtrip");
		}

		[Test]
		public void ToFromString_Zero ()
		{
			string s = NSDirectionalEdgeInsets.Zero.ToString ();
			var e = NSDirectionalEdgeInsets.FromString (s);
			Assert.That (e, Is.EqualTo (NSDirectionalEdgeInsets.Zero), "roundtrip");
		}

		[Test]
		public void Operators ()
		{
			var i1 = new NSDirectionalEdgeInsets (10, 20, 30, 40);
			var i2 = new NSDirectionalEdgeInsets (10, 10, 10, 10);

			Assert.True (i1 == i1, "i1 == i1");
			Assert.True (i2 == i2, "i1 == i1");
			Assert.True (i1 != i2, "i1 != i2");
			Assert.True (i2 != i1, "i2 != i1");
		}
	}
}

#endif // XAMCORE_2_0