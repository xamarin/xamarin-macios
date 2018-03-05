//
// Unit tests for NSFetchedResultsController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !MONOMAC
using System;
using System.Linq;
#if XAMCORE_2_0
using Foundation;
using CoreData;
#else
using MonoTouch.CoreData;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreData {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FetchedResultsControllerTest {

		[TestFixtureSetUp]
		public void Cache ()
		{
			// null -> delete all cache
			NSFetchedResultsController.DeleteCache (null);
		}
		
		[Test]
		public void Default ()
		{
			using (NSFetchedResultsController frc = new NSFetchedResultsController ()) {
				NSError e;
				Assert.False (frc.PerformFetch (out e), "PerformFetch");
				Assert.Null (e, "NSError");
			}
		}

		[Test]
		public void PerformFetch_Minimal ()
		{
			using (NSManagedObjectContext c = new NSManagedObjectContext (NSManagedObjectContextConcurrencyType.PrivateQueue))
			using (NSFetchRequest r = new NSFetchRequest ()) {
				r.SortDescriptors = new NSSortDescriptor[] {
					new NSSortDescriptor ("key", true)
				};
				r.Entity = new NSEntityDescription ();
				using (NSFetchedResultsController frc = new NSFetchedResultsController (r, c, null, null)) {
					NSError e;
					Assert.False (frc.PerformFetch (out e), "PerformFetch");
				}
			}
		}

		[Test]
		public void Sections ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=13785

			// use Caches directory so this works with tvOS on devices (as well as existing iOS devices)
			string applicationDocumentsDirectory = NSSearchPath.GetDirectories (NSSearchPathDirectory.CachesDirectory, NSSearchPathDomain.User, true).LastOrDefault ();

			using (var ManagedObjectModel = new NSManagedObjectModel ()) {
				{
					// create an entity description
					NSEntityDescription entity = new NSEntityDescription ();
					entity.Name = "Earthquake";

					// create an attribute for the entity
					NSAttributeDescription date = new NSAttributeDescription ();
					date.AttributeType = NSAttributeType.Date;
					date.Name = "date";
					date.Optional = false;

					NSAttributeDescription latitude = new NSAttributeDescription ();
					latitude.AttributeType = NSAttributeType.Double;
					latitude.Name = "latitude";
					latitude.Optional = false;

					NSAttributeDescription location = new NSAttributeDescription ();
					location.AttributeType = NSAttributeType.String;
					location.Name = "location";
					location.Optional = false;

					NSAttributeDescription longitude = new NSAttributeDescription ();
					longitude.AttributeType = NSAttributeType.Double;
					longitude.Name = "longitude";
					longitude.Optional = false;

					NSAttributeDescription magnitude = new NSAttributeDescription ();
					magnitude.AttributeType = NSAttributeType.Float;
					magnitude.Name = "magnitude";
					magnitude.Optional = false;

					NSAttributeDescription USGSWebLink = new NSAttributeDescription ();
					USGSWebLink.AttributeType = NSAttributeType.String;
					USGSWebLink.Name = "USGSWebLink";
					USGSWebLink.Optional = false;

					// assign the properties to the entity
					entity.Properties = new NSPropertyDescription[] { 
						date,
						latitude,
						location,
						longitude,
						magnitude,
						USGSWebLink
					};

					// add the entity to the model, and then add a configuration that
					// contains the entities
					ManagedObjectModel.Entities = new NSEntityDescription[] { entity };
					ManagedObjectModel.SetEntities (ManagedObjectModel.Entities, String.Empty);
				}

				using (var PersistentStoreCoordinator = new NSPersistentStoreCoordinator (ManagedObjectModel)) {
					{
						var storePath = applicationDocumentsDirectory + "/Earthquakes.sqlite";
						var storeUrl = new NSUrl (storePath, false);
						NSError error;

						if (PersistentStoreCoordinator.AddPersistentStoreWithType (NSPersistentStoreCoordinator.SQLiteStoreType, null, storeUrl, null, out error) == null) {
							Assert.Fail ("Unresolved error " + error + ", " + error.UserInfo);
						}
					}

					using (var ManagedObjectContext = new NSManagedObjectContext ()) {
						ManagedObjectContext.PersistentStoreCoordinator = PersistentStoreCoordinator;
					
	//					NSNotificationCenter.DefaultCenter.AddObserver (
	//						this, new MonoTouch.ObjCRuntime.Selector ("mergeChanges"), 
	//						"NSManagedObjectContextDidSaveNotification", null);


						NSFetchRequest fetchRequest = new NSFetchRequest ();
						NSEntityDescription entity = NSEntityDescription.EntityForName ("Earthquake", ManagedObjectContext);
						fetchRequest.Entity = entity;

						NSSortDescriptor sortDescriptor = new NSSortDescriptor ("date", false);
						fetchRequest.SortDescriptors = new [] { sortDescriptor };

						NSFetchedResultsController fetchedResultsController = new NSFetchedResultsController (
							fetchRequest, ManagedObjectContext, null, null);

						NSError error;

						if (!fetchedResultsController.PerformFetch (out error)) {
							Assert.Fail ("Unresolved error: " + error + ", " + error.UserInfo);
						}

						var sections = fetchedResultsController.Sections;
						Assert.That (sections [0].GetType ().FullName, Is.StringEnding ("CoreData.NSFetchedResultsSectionInfoWrapper"), "Wrapper");
					}
				}
			}
		}
	}
}
#endif