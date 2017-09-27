//
// Unit tests for UIContentSizeCategory
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft. All rights reserved.
//

using System;
using Foundation;
using NUnit.Framework;
using UIKit;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UIContentSizeCategoryTest {

		[Test]
		public void IsAccessibilityCategory ()
		{
			var isAccessible = UIContentSizeCategory.AccessibilityMedium.IsAccessibilityCategory ();
			Assert.IsTrue (isAccessible, "AccessibilityMedium");
			isAccessible = UIContentSizeCategory.Medium.IsAccessibilityCategory ();
			Assert.IsFalse (isAccessible, "Medium");
		}

		[Test]
		public void Compare ()
		{
			var small = UIContentSizeCategory.Small;
			var large = UIContentSizeCategory.Large;
			Assert.True (UIContentSizeCategoryExtensions.Compare (small, large) == NSComparisonResult.Ascending, "small < large");
		}
	}
}
