//
// Unit tests for NSManagedObjectModel
//
// Authors:
//	Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;

using CoreData;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreData {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ManagedObjectModelTest {

		void Default (NSManagedObjectModel moc)
		{
			Assert.That (moc.EntitiesByName.Count, Is.EqualTo ((nuint) 0), "EntitiesByName");
			Assert.That (moc.Configurations.Length, Is.EqualTo (0), "Configurations");
			Assert.Null (moc.LocalizationDictionary, "LocalizationDictionary");
			Assert.That (moc.FetchRequestTemplatesByName.Count, Is.EqualTo ((nuint) 0), "FetchRequestTemplatesByName");
			Assert.That (moc.VersionIdentifiers.Count, Is.EqualTo ((nuint) 0), "VersionIdentifiers");
			Assert.That (moc.EntityVersionHashesByName.Count, Is.EqualTo ((nuint) 0), "EntityVersionHashesByName");
		}

		[Test]
		public void IsConfiguration_Null ()
		{
			using (var moc = new NSManagedObjectModel ()) {
#if NET
				Assert.IsFalse (moc.IsConfigurationCompatibleWithStoreMetadata (null, new NSDictionary ()), "IsConfiguration");
#else
				Assert.IsFalse (moc.IsConfiguration (null, new NSDictionary ()), "IsConfiguration");
#endif
				Default (moc);
			}
		}
	}
}
