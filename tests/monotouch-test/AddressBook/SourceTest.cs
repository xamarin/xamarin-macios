//
// Unit tests for ABSource
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC && !__MACCATALYST__

using System;
using Foundation;
using UIKit;
using AddressBook;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.AddressBook {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SourceTest {

		[SetUp]
		public void Setup ()
		{
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
			if (TestRuntime.CheckXcodeVersion (15, 0)) {
				Assert.Ignore ("The addressbook framework is deprecated in Xcode 15.0 and always returns null");
			}
		}

		[Test]
		public void Default ()
		{
			if (Runtime.Arch != Arch.SIMULATOR)
				return;

			// we assume the simulator defaults (e.g. after a reset)
			ABSource source = new ABAddressBook ().GetDefaultSource ();
			Assert.Null (source.Name, "Name");
			Assert.That (source.SourceType, Is.EqualTo (ABSourceType.Local), "SourceType");

			// ABRecord
			// some bots returns -1 (invalid) and I get 0 after a reset (maybe permission related?)
			Assert.That (source.Id, Is.LessThanOrEqualTo (0), "Id");
			// iOS [9,11.2[ returned ABRecordType.Person, otherwise ABRecordType.Source
			Assert.That (source.Type, Is.Not.EqualTo (ABRecordType.Group), "Type");
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__ && !__MACCATALYST__
