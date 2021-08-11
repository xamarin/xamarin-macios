//
// Unit tests for NSDiffableDataSourceSnapshot
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2019 Microsoft Corporation.
//

#if !__WATCHOS__ && !MONOMAC
using System;
using NUnit.Framework;
using Foundation;
using UIKit;
using ObjCRuntime;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSDiffableDataSourceSnapshotTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
		}

		// https://github.com/xamarin/xamarin-macios/issues/6567
		[Test]
		public void GHIssue6567Test ()
		{
			var type = typeof (UICollectionViewDiffableDataSource<,>);
			Assert.NotNull (type, $"{nameof (type)} was null;");

			Class cls = null;
			Assert.DoesNotThrow (() => cls = new Class (type), "Should not throw");
			Assert.NotNull (cls, $"{nameof (cls)} was null");
		}

		[Test]
		public void ObjectUsageTest ()
		{
			var diff = new NSDiffableDataSourceSnapshot<NSNumber, NSUuid> ();
			diff.AppendSections (new NSNumber [] { NSNumber.FromNInt (1) });
			diff.AppendItems (new NSUuid [] { new NSUuid () }, NSNumber.FromNInt (1));
			Assert.That (diff.NumberOfSections, Is.GreaterThan ((nint) 0), "Sections");
			Assert.That (diff.GetNumberOfItems (NSNumber.FromNInt (1)), Is.GreaterThan ((nint) 0), "Items");
		}
	}
}
#endif // !__WATCHOS__ && !MONOMAC
