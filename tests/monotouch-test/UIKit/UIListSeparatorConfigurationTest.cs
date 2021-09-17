//
// UIListSeparatorConfiguration.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

#if __IOS__

using System;
using System.Drawing;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UIListSeparatorConfigurationTest {

		[Test]
		public void AutomaticInsetsTest ()
		{
			TestRuntime.AssertXcodeVersion (12, 5);

			var insets = UIListSeparatorConfiguration.AutomaticInsets;
			Assert.That (insets.Leading, Is.Not.Zero, "leading");
			Assert.That (insets.Trailing, Is.Not.Zero, "Trailing");
		}
	}
}

#endif // __WATCHOS__ || __IOS__
