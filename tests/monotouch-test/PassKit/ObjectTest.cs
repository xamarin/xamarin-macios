//
// Unit tests for PKObject
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !MONOMAC

using System;
using Foundation;
using UIKit;
using PassKit;
using NUnit.Framework;

namespace MonoTouchFixtures.PassKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ObjectTest {
		[Test]
		public void Constructor ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			if (TestRuntime.CheckXcodeVersion (6, 0)) {
				Assert.DoesNotThrow (() => new PKObject ());
			} else {
				Assert.Throws<Exception> (() => new PKObject ());
			}
		}
	}
}

#endif // !__TVOS__
