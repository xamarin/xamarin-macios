//
// Unit tests for NSManagedObjectContext
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using CoreData;
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
	public class ManagedObjectContextTest {

		void Default (NSManagedObjectContext moc)
		{
			Assert.That (moc.DeletedObjects.Count, Is.EqualTo ((nuint) 0), "DeletedObjects");
			Assert.False (moc.HasChanges, "HasChanges");
			Assert.That (moc.InsertedObjects.Count, Is.EqualTo ((nuint) 0), "InsertedObjects");
			Assert.That (moc.MergePolicy, Is.Not.EqualTo (IntPtr.Zero), "MergePolicy");
			Assert.Null (moc.ParentContext, "ParentContext");
			Assert.Null (moc.PersistentStoreCoordinator, "PersistentStoreCoordinator");
			Assert.That (moc.RegisteredObjects.Count, Is.EqualTo ((nuint) 0), "RegisteredObjects");
			Assert.False (moc.RetainsRegisteredObjects, "RetainsRegisteredObjects");
			Assert.That (moc.StalenessInterval, Is.EqualTo (-1), "StalenessInterval");
			if (TestRuntime.CheckMacSystemVersion (10, 12, throwIfOtherPlatform: false))
				Assert.Null (moc.UndoManager, "UndoManager");
			else
				Assert.NotNull (moc.UndoManager, "UndoManager");
			Assert.That (moc.UpdatedObjects.Count, Is.EqualTo ((nuint) 0), "UpdatedObjects");
			Assert.That (moc.UserInfo.Count, Is.EqualTo ((nuint) 0), "UserInfo");
		}

		[Test]
		public void Default ()
		{
			using (var moc = new NSManagedObjectContext ()) {
				Assert.That (moc.ConcurrencyType, Is.EqualTo (NSManagedObjectContextConcurrencyType.Confinement), "ConcurrencyType");
				Default (moc);
			}
		}

		[Test]
		public void Main ()
		{
			using (var moc = new NSManagedObjectContext (NSManagedObjectContextConcurrencyType.MainQueue)) {
				Assert.That (moc.ConcurrencyType, Is.EqualTo (NSManagedObjectContextConcurrencyType.MainQueue), "ConcurrencyType");
				Default (moc);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Perform_Null ()
		{
			using (var moc = new NSManagedObjectContext (NSManagedObjectContextConcurrencyType.MainQueue)) {
				// a NULL results in a native crash - but not immediate
				moc.Perform (null);
			}
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void PerformAndWait_Null ()
		{
			using (var moc = new NSManagedObjectContext (NSManagedObjectContextConcurrencyType.MainQueue)) {
				// a NULL results in a *immediate* native crash
				moc.PerformAndWait (null);
			}
		}

		[Test]
		public void UndoManager_Null ()
		{
			using (var moc = new NSManagedObjectContext (NSManagedObjectContextConcurrencyType.MainQueue)) {
				moc.UndoManager = null;
			}
		}
	}
}
