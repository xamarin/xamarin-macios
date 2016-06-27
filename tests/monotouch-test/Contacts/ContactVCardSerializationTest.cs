//
// Unit tests for CNContactVCardSerialization
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__TVOS__
#if XAMCORE_2_0 // The Contacts framework is Unified only

using System;
#if XAMCORE_2_0
using Contacts;
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.Contacts;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Contacts {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ContactVCardSerializationTest {

		[SetUp]
		public void MinimumSdkCheck ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void GetDescriptorFromRequiredKeys ()
		{
			var keys = CNContactVCardSerialization.GetDescriptorFromRequiredKeys ();
			// while most input for ICNKeyDescriptor are done with NSString
			// the output is opaque and an internal type
			// note: this is not very robust - but I want to know if this changes during the next betas
			Assert.True (keys.Description.StartsWith ("<CNAggregateKeyDescriptor:", StringComparison.Ordinal), "type");
			Assert.True (keys.Description.Contains ("kind=+[CNContactVCardSerialization descriptorForRequiredKeys]"), "kind");
		}
	}
}

#endif // XAMCORE_2_0
#endif // !__TVOS__
