//
// Unit tests for NSAttributeDescriptionTest
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2012 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using CoreData;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreData {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AttributeDescription {

		[Test]
		public void WeakFramework ()
		{
			NSAttributeDescription ad = new NSAttributeDescription ();
			Assert.That (ad.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			// if CoreData is not linked then all related objects handle will be null
		}

		[Test]
		public void DefaultValue ()
		{
			using (var ad = new NSAttributeDescription ())
			using (var o = new NSObject ()) {
				ad.DefaultValue = o;
				Assert.AreSame (o, ad.DefaultValue, "DefaultValue");
			}
		}

		[Test]
		public void GetSetRenamingIdentifier ()
		{
			using (var ad = new NSAttributeDescription ()) {
				Assert.IsNull (ad.RenamingIdentifier, "An unset RenamingIdentifier should be null.");
				ad.RenamingIdentifier = "Foo";
				Assert.AreEqual ("Foo", ad.RenamingIdentifier,
								 "RenamingIndentifier was not corrently set.");
			}
		}
	}
}
