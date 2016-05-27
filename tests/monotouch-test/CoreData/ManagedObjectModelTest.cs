//
// Unit tests for NSManagedObjectModel
//
// Authors:
//	Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using CoreData;
using Foundation;
#else
using MonoTouch.CoreData;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

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
			using (var moc = new NSManagedObjectModel()) {
				Assert.IsFalse (moc.IsConfiguration (null, new NSDictionary ()), "IsConfiguration");
				Default (moc);
			}
		}
	}
}
