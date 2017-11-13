//
// Unit tests for EKUIBundle
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 && !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using EventKitUI;
using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures.EventKitUI {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EKUIBundleTest {

		[Test]
		public void BundleTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (9, 0))
				Assert.Ignore ("Ignoring tests: Requires iOS11+");

			var bundle = EKUIBundle.UIBundle;
			Assert.NotNull (bundle, "Was Null");
			Assert.AreEqual ("com.apple.eventkitui", bundle.BundleIdentifier, "BundleIdentifier");
		}
	}
}
#endif
