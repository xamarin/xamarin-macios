//
// Unit tests for NSFetchRequest
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
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
	public class FetchRequestTest {

		[Test]
		public void DefaultValues ()
		{
			using (var fr = new NSFetchRequest ()) {
				Assert.Null (fr.AffectedStores, "AffectedStores");
				Assert.Null (fr.Entity, "Entity");
				Assert.Null (fr.EntityName, "EntityName");
				Assert.That (fr.FetchBatchSize, Is.EqualTo ((nint) 0), "FetchBatchSize");
				Assert.That (fr.FetchLimit, Is.EqualTo ((nuint) 0), "FetchLimit");
				Assert.That (fr.FetchOffset, Is.EqualTo ((nuint) 0), "FetchOffset");
				Assert.Null (fr.HavingPredicate, "HavingPredicate");
				Assert.True (fr.IncludesPendingChanges, "IncludesPendingChanges");
				Assert.True (fr.IncludesPropertyValues, "IncludesPropertyValues");
				Assert.True (fr.IncludesSubentities, "IncludesSubentities");
				Assert.Null (fr.Predicate, "Predicate");
				Assert.Null (fr.PropertiesToFetch, "PropertiesToFetch");
				Assert.Null (fr.PropertiesToGroupBy, "PropertiesToGroupBy");
				Assert.Null (fr.RelationshipKeyPathsForPrefetching, "RelationshipKeyPathsForPrefetching");
				Assert.That (fr.ResultType, Is.EqualTo (NSFetchRequestResultType.ManagedObject), "ResultType");
				Assert.False (fr.ReturnsDistinctResults, "ReturnsDistinctResults");
				Assert.True (fr.ReturnsObjectsAsFaults, "ReturnsObjectsAsFaults");
				Assert.False (fr.ShouldRefreshRefetchedObjects, "ShouldRefreshRefetchedObjects");
				Assert.Null (fr.SortDescriptors, "SortDescriptors");
			}
		}

		[Test]
		public void CtorString ()
		{
			using (var fr = new NSFetchRequest ("entityName")) {
				Assert.That (fr.EntityName, Is.EqualTo ("entityName"), "EntityName");
				// Entity is invalid (and throws) so we do not check it - except to see if we can set it to null
				fr.Entity = null;
				Assert.Null (fr.Entity, "Entity");
			}
		}

		[Test]
		public void SettersNull ()
		{
			using (var fr = new NSFetchRequest ()) {
				// bug #18153
				fr.Predicate = null;
				// bug #18152
				fr.SortDescriptors = null;
				// other properties that are null (by default) are likely accepting being set to null
				fr.AffectedStores = fr.AffectedStores;
				fr.HavingPredicate = fr.HavingPredicate;
				fr.PropertiesToGroupBy = fr.PropertiesToGroupBy;
				fr.RelationshipKeyPathsForPrefetching = fr.RelationshipKeyPathsForPrefetching;
			}
		}
	}
}
