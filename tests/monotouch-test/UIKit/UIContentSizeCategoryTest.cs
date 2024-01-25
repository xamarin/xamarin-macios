//
// Unit tests for UIContentSizeCategory
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;

using Foundation;

using NUnit.Framework;

using UIKit;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UIContentSizeCategoryTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

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
			Assert.Throws<ArgumentException> (() => UIContentSizeCategoryExtensions.Compare ((UIContentSizeCategory) 31415, large));
			Assert.Throws<ArgumentException> (() => UIContentSizeCategoryExtensions.Compare (small, (UIContentSizeCategory) 271828));
			Assert.Throws<ArgumentException> (() => ((UIContentSizeCategory) 1234).IsAccessibilityCategory ());
		}

		[Test]
		public void GetPreferredContentSizeCategoryTest ()
		{
			var sizeNSString = UIApplication.SharedApplication.PreferredContentSizeCategory;
			var sizeEnum = UIContentSizeCategoryExtensions.GetValue (sizeNSString);
			var size = UIApplication.SharedApplication.GetPreferredContentSizeCategory ();
			Assert.AreEqual (sizeEnum, size, "String");
			var sizeReverse = size.GetConstant ();
			Assert.AreEqual (sizeNSString, sizeReverse, "NSString");
		}
	}
}

#endif // !__WATCHOS__
