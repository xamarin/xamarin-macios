//
// Unit tests for NSPersistentStoreCoordinator
//

#if !TVOS

using System;
using System.Linq;

using CoreData;
using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreData {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSPersistentStoreCoordinatorTest {
#if NET
		[Test]
		public void GetManagedObjectId ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var managedObjectModel = new NSManagedObjectModel ();
			// create an entity description
			var entity = new NSEntityDescription ();
			entity.Name = "Earthquake";

			// create an attribute for the entity
			using var date = new NSAttributeDescription ();
			date.AttributeType = NSAttributeType.Date;
			date.Name = "date";
			date.Optional = false;

			using var latitude = new NSAttributeDescription ();
			latitude.AttributeType = NSAttributeType.Double;
			latitude.Name = "latitude";
			latitude.Optional = false;

			using var location = new NSAttributeDescription ();
			location.AttributeType = NSAttributeType.String;
			location.Name = "location";
			location.Optional = false;

			using var longitude = new NSAttributeDescription ();
			longitude.AttributeType = NSAttributeType.Double;
			longitude.Name = "longitude";
			longitude.Optional = false;

			using var magnitude = new NSAttributeDescription ();
			magnitude.AttributeType = NSAttributeType.Float;
			magnitude.Name = "magnitude";
			magnitude.Optional = false;

			using var USGSWebLink = new NSAttributeDescription ();
			USGSWebLink.AttributeType = NSAttributeType.String;
			USGSWebLink.Name = "USGSWebLink";
			USGSWebLink.Optional = false;

			// assign the properties to the entity
			entity.Properties = new NSPropertyDescription [] {
				date,
				latitude,
				location,
				longitude,
				magnitude,
				USGSWebLink
			};

			// add the entity to the model, and then add a configuration that
			// contains the entities
			managedObjectModel.Entities = new NSEntityDescription [] { entity };
			managedObjectModel.SetEntities (managedObjectModel.Entities, String.Empty);

			using var psc = new NSPersistentStoreCoordinator (managedObjectModel);
			Assert.IsNull (psc.GetManagedObjectId ("magnitude"), "GetManagedObjectId");
		}
#endif // NET
	}
}
#endif // !TVOS
