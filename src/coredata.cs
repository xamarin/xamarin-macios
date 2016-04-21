// This file describes the API that the generator will produce
//
// Authors:
//   MonoMac community
//   Miguel de Icaza
//
// Copyright 2009, 2011, MonoMac community
// Copyright 2011, 2015 Xamarin Inc.
//
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.CoreData
{
	[NoWatch][NoTV]
	[Native] // NUInteger -> NSPersistentStoreCoordinator.h
	public enum NSPersistentStoreUbiquitousTransitionType : nuint_compat_int {
		AccountAdded = 1,
		AccountRemoved,
		ContentRemoved,
		InitialImportCompleted
	}

	[BaseType (typeof (NSPersistentStore))]
	// Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: NSMappedObjectStore must be initialized with initWithPersistentStoreCoordinator:configurationName:URL:options
	[DisableDefaultCtor]
	public interface NSAtomicStore {

		[Export ("initWithPersistentStoreCoordinator:configurationName:URL:options:")]
		IntPtr Constructor (NSPersistentStoreCoordinator coordinator, string configurationName, NSUrl url, [NullAllowed] NSDictionary options);

		[Export ("load:")]
		bool Load (out NSError error);

		[Export ("save:")]
		bool Save (out NSError error);

		[Export ("newCacheNodeForManagedObject:")]
		NSAtomicStoreCacheNode NewCacheNodeForManagedObject (NSManagedObject managedObject);

		[Export ("updateCacheNode:fromManagedObject:")]
		void UpdateCacheNode (NSAtomicStoreCacheNode node, NSManagedObject managedObject);

		[Export ("cacheNodes")]
		NSSet CacheNodes { get; }

		[Export ("addCacheNodes:")]
		void AddCacheNodes (NSSet cacheNodes);

		[Export ("willRemoveCacheNodes:")]
		void WillRemoveCacheNodes (NSSet cacheNodes);

		[Export ("cacheNodeForObjectID:")]
		NSAtomicStoreCacheNode CacheNodeForObjectID (NSManagedObjectID objectID);

		[Export ("objectIDForEntity:referenceObject:")]
		NSManagedObjectID ObjectIDForEntity (NSEntityDescription entity, NSObject data);

		[Export ("newReferenceObjectForManagedObject:")]
		NSAtomicStore NewReferenceObjectForManagedObject (NSManagedObject managedObject);

		[Export ("referenceObjectForObjectID:")]
		NSAtomicStore ReferenceObjectForObjectID (NSManagedObjectID objectID);
	}

	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: NSAtomicStoreCacheNodes must be initialized using initWithObjectID:(NSManagedObjectID *)
	[DisableDefaultCtor]
	public interface NSAtomicStoreCacheNode {

		[Export ("initWithObjectID:")]
		IntPtr Constructor (NSManagedObjectID moid);

		[Export ("objectID")]
		NSManagedObjectID ObjectID { get; }

		[Export ("propertyCache", ArgumentSemantic.Retain)]
		NSDictionary PropertyCache { get; set; }

		[Export ("valueForKey:")]
		NSAtomicStoreCacheNode ValueForKey (string key);

		[Export ("setValue:forKey:")]
		void SetValue (NSObject value, string key);

	}
	[BaseType (typeof (NSPropertyDescription))]
	public interface NSAttributeDescription {

		[Export ("attributeType")]
		NSAttributeType AttributeType { get; set; }

		// Default property value is null but it cannot be set to that value
		// NSInternalInconsistencyException Reason: Can't set attributeValueClassName to nil for a non-transient attribute.
		[Export ("attributeValueClassName")]
		string AttributeValueClassName { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("defaultValue", ArgumentSemantic.Retain)]
		NSObject DefaultValue { get; set; }

		[Export ("versionHash")]
		NSData VersionHash { get; }

		[NullAllowed] // by default this property is null
		[Export ("valueTransformerName")]
		string ValueTransformerName { get; set; }

		[Since(5,0)]
		[Export ("allowsExternalBinaryDataStorage")]
		bool AllowsExternalBinaryDataStorage { get; set; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSEntityDescription : NSCoding, NSCopying {

		[Static, Export ("entityForName:inManagedObjectContext:")]
		NSEntityDescription EntityForName (string entityName, NSManagedObjectContext context);

		[Static, Export ("insertNewObjectForEntityForName:inManagedObjectContext:")]
		NSObject InsertNewObjectForEntityForName (string entityName, NSManagedObjectContext context);

		[Export ("managedObjectModel")]
		NSManagedObjectModel ManagedObjectModel { get; }

		[Export ("managedObjectClassName")]
		string ManagedObjectClassName { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[Export ("Abstract")]
		bool Abstract { [Bind("isAbstract")] get; set; }

		[Export ("subentitiesByName")]
		NSDictionary SubentitiesByName { get; }

		[Export ("subentities", ArgumentSemantic.Retain)]
		NSEntityDescription[] Subentities { get; set; }

		[Export ("superentity")]
		NSEntityDescription Superentity { get; }

		[Export ("propertiesByName")]
		NSDictionary PropertiesByName { get; }

		[Export ("properties", ArgumentSemantic.Retain)]
		NSPropertyDescription[] Properties { get; set; }

		[Export ("userInfo", ArgumentSemantic.Retain)]
		NSDictionary UserInfo { get; set; }

		[Export ("attributesByName")]
		NSDictionary AttributesByName { get; }

		[Export ("relationshipsByName")]
		NSDictionary RelationshipsByName { get; }

		[Export ("relationshipsWithDestinationEntity:")]
		NSRelationshipDescription[] RelationshipsWithDestinationEntity (NSEntityDescription entity);

		[Export ("isKindOfEntity:")]
		bool IsKindOfEntity (NSEntityDescription entity);

		[NullAllowed] // by default this property is null
		[Export ("versionHash")]
		NSData VersionHash { get; }

		[NullAllowed] // by default this property is null
		[Export ("versionHashModifier")]
		string VersionHashModifier { get; set; }

		[Since(5,0)]
		[NullAllowed] // by default this property is null
		[Export ("compoundIndexes", ArgumentSemantic.Retain)]
		NSPropertyDescription [] CompoundIndexes { get; set; }

		// @property (strong) NSArray<NSArray<id __nonnull> * __nonnull> * __nonnull uniquenessConstraints __attribute__((availability(ios, introduced=9.0)));
		[iOS (9,0), Mac (10,11)]
		[Internal, Export ("uniquenessConstraints", ArgumentSemantic.Strong)]
		NSArray _UniquenessConstraints { get; set; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSEntityMapping {

		[Export ("name")]
		string Name { get; set; }

		[Export ("mappingType")]
		NSEntityMappingType MappingType { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("sourceEntityName")]
		string SourceEntityName { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("sourceEntityVersionHash", ArgumentSemantic.Copy)]
		NSData SourceEntityVersionHash { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("destinationEntityName", ArgumentSemantic.Copy)]
		string DestinationEntityName { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("destinationEntityVersionHash", ArgumentSemantic.Copy)]
		NSData DestinationEntityVersionHash { get; set; }

		[Export ("attributeMappings", ArgumentSemantic.Retain)]
		NSPropertyMapping[] AttributeMappings { get; set; }

		[Export ("relationshipMappings", ArgumentSemantic.Retain)]
		NSPropertyMapping[] RelationshipMappings { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("sourceExpression", ArgumentSemantic.Retain)]
		NSExpression SourceExpression { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("userInfo", ArgumentSemantic.Retain)]
		NSDictionary UserInfo { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("entityMigrationPolicyClassName")]
		string EntityMigrationPolicyClassName { get; set; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSEntityMigrationPolicy {

		[Export ("beginEntityMapping:manager:error:")]
		bool BeginEntityMapping (NSEntityMapping mapping, NSMigrationManager manager, out NSError error);

		[Export ("createDestinationInstancesForSourceInstance:entityMapping:manager:error:")]
		bool CreateDestinationInstancesForSourceInstance (NSManagedObject sInstance, NSEntityMapping mapping, NSMigrationManager manager, out NSError error);

		[Export ("endInstanceCreationForEntityMapping:manager:error:")]
		bool EndInstanceCreationForEntityMapping (NSEntityMapping mapping, NSMigrationManager manager, out NSError error);

		[Export ("createRelationshipsForDestinationInstance:entityMapping:manager:error:")]
		bool CreateRelationshipsForDestinationInstance (NSManagedObject dInstance, NSEntityMapping mapping, NSMigrationManager manager, out NSError error);

		[Export ("endRelationshipCreationForEntityMapping:manager:error:")]
		bool EndRelationshipCreationForEntityMapping (NSEntityMapping mapping, NSMigrationManager manager, out NSError error);

		[Export ("performCustomValidationForEntityMapping:manager:error:")]
		bool PerformCustomValidationForEntityMapping (NSEntityMapping mapping, NSMigrationManager manager, out NSError error);

		[Export ("endEntityMapping:manager:error:")]
		bool EndEntityMapping (NSEntityMapping mapping, NSMigrationManager manager, out NSError error);
	}

	[BaseType (typeof (NSPropertyDescription))]
	public interface NSFetchedPropertyDescription {

		[NullAllowed] // by default this property is null
		[Export ("fetchRequest", ArgumentSemantic.Retain)]
		NSFetchRequest FetchRequest { get; set; }
	}

	[BaseType (typeof (NSPersistentStoreRequest))]
	public interface NSFetchRequest : NSCoding {

		[Export ("entity", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSEntityDescription Entity { get; set; }

		[Export ("predicate", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSPredicate Predicate { get; set; }

		[Export ("sortDescriptors", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSSortDescriptor[] SortDescriptors { get; set; }

		[Export ("fetchLimit")]
		nuint FetchLimit { get; set; }

		[Export ("fetchOffset")]
		nuint FetchOffset { get; set; }

		[Export ("affectedStores", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSPersistentStore[] AffectedStores { get; set; }

		[Export ("resultType")]
		NSFetchRequestResultType ResultType { get; set; }

		[Export ("returnsDistinctResults")]
		bool ReturnsDistinctResults { get; set; }

		[Export ("includesSubentities")]
		bool IncludesSubentities { get; set; }

		[Export ("includesPropertyValues")]
		bool IncludesPropertyValues { get; set; }

		[Export ("includesPendingChanges")]
		bool IncludesPendingChanges { get; set; }

		[Export ("returnsObjectsAsFaults")]
		bool ReturnsObjectsAsFaults { get; set; }

		[Export ("relationshipKeyPathsForPrefetching")]
		[NullAllowed]
		string[] RelationshipKeyPathsForPrefetching { get; set; }

		[Export ("propertiesToFetch", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSPropertyDescription [] PropertiesToFetch { get; set; }

		[Since(5,0)]
		[Static]
		[Export ("fetchRequestWithEntityName:")]
		// note: Xcode 6.3 changed the return value type from `NSFetchRequest*` to `instancetype`
		NSFetchRequest FromEntityName (string entityName);

		[Since(5,0)]
		[Export ("initWithEntityName:")]
		IntPtr Constructor (string entityName);

		[Since(5,0)]
		[Export ("entityName")]
		string EntityName { get; }

		[Since(5,0)]
		[Export ("fetchBatchSize")]
		nint FetchBatchSize { get; set; }

		[Since(5,0)]
		[Export ("shouldRefreshRefetchedObjects")]
		bool ShouldRefreshRefetchedObjects { get; set; }

		[Since(5,0)]
		[Export ("havingPredicate", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSPredicate HavingPredicate { get; set; }

		[Since(5,0)]
		[Export ("propertiesToGroupBy", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSPropertyDescription [] PropertiesToGroupBy { get; set; }
	}
#if !MONOMAC
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" })]
	interface NSFetchedResultsController {

		[Export ("initWithFetchRequest:managedObjectContext:sectionNameKeyPath:cacheName:")]
		IntPtr Constructor (NSFetchRequest fetchRequest, NSManagedObjectContext context, [NullAllowed] string sectionNameKeyPath, [NullAllowed] string name);

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSFetchedResultsControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("cacheName")]
		string CacheName { get; }

		[Export ("fetchedObjects")]
		NSObject[] FetchedObjects { get; }

		[Export ("fetchRequest")]
		NSFetchRequest FetchRequest { get; }

		[Export ("managedObjectContext")]
		NSManagedObjectContext ManagedObjectContext { get; }

		[Export ("sectionNameKeyPath")]
		string SectionNameKeyPath { get; }

		[Export ("sections")]
		INSFetchedResultsSectionInfo[] Sections { get; }

		[Export ("performFetch:")]
		bool PerformFetch (out NSError error);

		[Export ("indexPathForObject:")]
		NSIndexPath FromObject (NSObject obj);

		[Export ("objectAtIndexPath:")]
		NSObject ObjectAt (NSIndexPath path);

		[Export ("sectionForSectionIndexTitle:atIndex:")]
		// name like UITableViewSource's similar (and linked) selector
		nint SectionFor (string title, nint atIndex);

#if !XAMCORE_4_0
		// badly named and conflict with the property
		[Export ("sectionIndexTitleForSectionName:")]
		string SectionIndexTitles (string sectionName);

		// expose a method as the property name is taken
		[Export ("sectionIndexTitles")]
		string[] GetSectionIndexTitles ();
#else
		[Export ("sectionIndexTitleForSectionName:")]
		string GetSectionIndexTitle (string sectionName);

		[Export ("sectionIndexTitles")]
		string[] SectionIndexTitles { get; }
#endif
		
		[Static]
		[Export ("deleteCacheWithName:")]
		void DeleteCache ([NullAllowed] string name);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSFetchedResultsControllerDelegate {
		[Export ("controllerWillChangeContent:")]
		void WillChangeContent (NSFetchedResultsController controller);

		[Export ("controller:didChangeObject:atIndexPath:forChangeType:newIndexPath:")]
		void DidChangeObject (NSFetchedResultsController controller, NSObject anObject, NSIndexPath indexPath, NSFetchedResultsChangeType type, NSIndexPath newIndexPath);

		[Export ("controller:didChangeSection:atIndex:forChangeType:")]
		void DidChangeSection (NSFetchedResultsController controller, INSFetchedResultsSectionInfo sectionInfo, nuint sectionIndex, NSFetchedResultsChangeType type);

		[Export ("controllerDidChangeContent:")]
		void DidChangeContent (NSFetchedResultsController controller);

		[Export ("controller:sectionIndexTitleForSectionName:")]
		string SectionFor (NSFetchedResultsController controller, string sectionName);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSFetchedResultsSectionInfo {
		[Export ("numberOfObjects")]
		[Abstract]
		nint Count { get; }

		[Export ("objects")]
		[Abstract]
		NSObject[] Objects { get; }

		[Export ("name")]
		[Abstract]
		string Name { get; }

		[Export ("indexTitle")]
		[Abstract]
		string IndexTitle { get; }
	}

	interface INSFetchedResultsSectionInfo {}
#endif
	[Since(5,0)]
	// 	NSInvalidArgumentException *** -loadMetadata: cannot be sent to an abstract object of class NSIncrementalStore: Create a concrete instance!
	//	Apple doc quote: "NSIncrementalStore is an abstract superclass..."
#if XAMCORE_4_0 // bad API -> should be an abstract type (breaking change)
	[Abstract]
#endif
	[BaseType (typeof (NSPersistentStore))]
	interface NSIncrementalStore {
#if XAMCORE_4_0
		[Protected]
#endif
		[Export ("initWithPersistentStoreCoordinator:configurationName:URL:options:")]
		IntPtr Constructor (NSPersistentStoreCoordinator root, string name, NSUrl url, NSDictionary options);

		[Export ("loadMetadata:")]
		bool LoadMetadata (out NSError error);

		[Export ("executeRequest:withContext:error:")]
		NSObject ExecuteRequest (NSPersistentStoreRequest request, NSManagedObjectContext context, out NSError error);

		[Export ("newValuesForObjectWithID:withContext:error:")]
		NSIncrementalStoreNode NewValues (NSManagedObjectID forObjectId, NSManagedObjectContext context, out NSError error);

		[Export ("newValueForRelationship:forObjectWithID:withContext:error:")]
		NSObject NewValue (NSRelationshipDescription forRelationship, NSManagedObjectID forObjectI, NSManagedObjectContext context, out NSError error);

		[Static]
		[Export ("identifierForNewStoreAtURL:")]
		NSObject IdentifierForNewStoreAtURL (NSUrl storeURL);

		[Export ("obtainPermanentIDsForObjects:error:")]
		NSObject [] ObtainPermanentIds (NSObject [] array, out NSError error);

		[Export ("managedObjectContextDidRegisterObjectsWithIDs:")]
		void ManagedObjectContextDidRegisterObjectsWithIds (NSObject [] objectIds);

		[Export ("managedObjectContextDidUnregisterObjectsWithIDs:")]
		void ManagedObjectContextDidUnregisterObjectsWithIds (NSObject [] objectIds);

		[Export ("newObjectIDForEntity:referenceObject:")]
		NSManagedObjectID NewObjectIdFor (NSEntityDescription forEntity, NSObject referenceObject);

		[Export ("referenceObjectForObjectID:")]
		NSObject ReferenceObjectForObject (NSManagedObjectID objectId);

	}

	[Since(5,0)]
	[BaseType (typeof (NSObject))]
	interface NSIncrementalStoreNode {
		[Export ("initWithObjectID:withValues:version:")]
		IntPtr Constructor (NSManagedObjectID objectId, NSDictionary values, ulong version);

		[Export ("updateWithValues:version:")]
		void Update (NSDictionary values, ulong version);

		[Export ("objectID")]
		NSManagedObjectID ObjectId { get; }

		[Export ("version")]
		long Version { get; }

		[Export ("valueForPropertyDescription:")]
		NSObject ValueForPropertyDescription (NSPropertyDescription prop);
	}

	[BaseType (typeof (NSObject))]
	// 'init' issues a warning: CoreData: error: Failed to call designated initializer on NSManagedObject class 'NSManagedObject' 
	// then crash while disposing the instance
	[DisableDefaultCtor]
	public interface NSManagedObject {
		[DesignatedInitializer]
		[Export ("initWithEntity:insertIntoManagedObjectContext:")]
		IntPtr Constructor (NSEntityDescription entity, NSManagedObjectContext context);

		[Export ("managedObjectContext")]
		NSManagedObjectContext ManagedObjectContext { get; }

		[Export ("entity")]
		NSEntityDescription Entity { get; }

		[Export ("objectID")]
		NSManagedObjectID ObjectID { get; }

		[Export ("isInserted")]
		bool IsInserted { get; }

		[Export ("isUpdated")]
		bool IsUpdated { get; }

		[Export ("isDeleted")]
		bool IsDeleted { get; }

		[Export ("isFault")]
		bool IsFault { get; }

		[Export ("hasFaultForRelationshipNamed:")]
		bool HasFaultForRelationshipNamed (string key);

		[Export ("willAccessValueForKey:")]
		void WillAccessValueForKey (string key);

		[Export ("didAccessValueForKey:")]
		void DidAccessValueForKey (string key);

		[Export ("willChangeValueForKey:")]
		void WillChangeValueForKey (string key);

		[Export ("didChangeValueForKey:")]
		void DidChangeValueForKey (string key);

		[Export ("willChangeValueForKey:withSetMutation:usingObjects:")]
		void WillChangeValueForKey (string inKey, NSKeyValueSetMutationKind inMutationKind, NSSet inObjects);

		[Export ("didChangeValueForKey:withSetMutation:usingObjects:")]
		void DidChangeValueForKey (string inKey, NSKeyValueSetMutationKind inMutationKind, NSSet inObjects);

		[NoWatch][NoTV]
		[Export ("observationInfo")]
		IntPtr ObservationInfo { get; set; }

		[Export ("awakeFromFetch")]
		void AwakeFromFetch ();

		[Export ("awakeFromInsert")]
		void AwakeFromInsert ();

		[Export ("willSave")]
		void WillSave ();

		[Export ("didSave")]
		void DidSave ();

		[Export ("willTurnIntoFault")]
		void WillTurnIntoFault ();

		[Export ("didTurnIntoFault")]
		void DidTurnIntoFault ();

		[Export ("valueForKey:")]
		IntPtr ValueForKey (string key);

		[Export ("setValue:forKey:")]
		void SetValue (IntPtr value, string key);

		[Export ("primitiveValueForKey:")]
		IntPtr PrimitiveValueForKey (string key);

		[Export ("setPrimitiveValue:forKey:")]
		void SetPrimitiveValue (IntPtr value, string key);

		[Export ("committedValuesForKeys:")]
		NSDictionary CommittedValuesForKeys (string[] keys);

		[Export ("changedValues")]
		NSDictionary ChangedValues { get; }

		[Export ("validateValue:forKey:error:")]
		bool ValidateValue (ref NSObject value, string key, out NSError error);

		[Export ("validateForDelete:")]
		bool ValidateForDelete (out NSError error);

		[Export ("validateForInsert:")]
		bool ValidateForInsert (out NSError error);

		[Export ("validateForUpdate:")]
		bool ValidateForUpdate (out NSError error);

		[Since(5,0)]
		[Export ("hasChanges")]
		bool HasChanges { get; }

		[Export ("changedValuesForCurrentEvent")]
		NSDictionary ChangedValuesForCurrentEvent { get; }

		[Export ("prepareForDeletion")]
		void PrepareForDeletion ();

		// headers say this is introduced in 7.0,10.9 but Xcode 7 API diff
		// indicates it's new in 9.0,10.11... going by the header value...
		[iOS (7,0), Mac (10,9)]
		[Export ("hasPersistentChangedValues")]
		bool HasPersistentChangedValues { get; }

		[iOS (8,3), Mac (10,11)]
		[Export ("objectIDsForRelationshipNamed:")]
		NSManagedObjectID [] GetObjectIDs (string relationshipName);
	}
	
	[BaseType (typeof (NSObject))]
	public interface NSManagedObjectContext : NSCoding
#if !WATCH && !TVOS
	, NSLocking
#endif // !WATCH
	{

		[NullAllowed] // by default this property is null
		[Export ("persistentStoreCoordinator", ArgumentSemantic.Retain)]
		NSPersistentStoreCoordinator PersistentStoreCoordinator { get; set; }

		[Export ("undoManager", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSUndoManager UndoManager { get; set; }

		[Export ("hasChanges")]
		bool HasChanges { get; }

		[Export ("objectRegisteredForID:")]
		NSManagedObject ObjectRegisteredForID (NSManagedObjectID objectID);

		[Export ("objectWithID:")]
		NSManagedObject ObjectWithID (NSManagedObjectID objectID);

		[Export ("executeFetchRequest:error:")]
		NSObject[] ExecuteFetchRequest (NSFetchRequest request, out NSError error);

		[Export ("countForFetchRequest:error:")]
		nuint CountForFetchRequest (NSFetchRequest request, out NSError error);

		[Export ("insertObject:")]
		void InsertObject (NSManagedObject object1);

		[Export ("deleteObject:")]
		void DeleteObject (NSManagedObject object1);

		[Export ("refreshObject:mergeChanges:")]
		void RefreshObject (NSManagedObject object1, bool flag);

		[Export ("detectConflictsForObject:")]
		void DetectConflictsForObject (NSManagedObject object1);

		[Export ("observeValueForKeyPath:ofObject:change:context:")]
		void ObserveValueForKeyPath (string keyPath, IntPtr object1, NSDictionary change, IntPtr context);

		[Export ("processPendingChanges")]
		void ProcessPendingChanges ();

		[Export ("assignObject:toPersistentStore:")]
		void AssignObject (IntPtr object1, NSPersistentStore store);

		[Export ("insertedObjects")]
		NSSet InsertedObjects { get; }

		[Export ("updatedObjects")]
		NSSet UpdatedObjects { get; }

		[Export ("deletedObjects")]
		NSSet DeletedObjects { get; }

		[Export ("registeredObjects")]
		NSSet RegisteredObjects { get; }

		[Export ("undo")]
		void Undo ();

		[Export ("redo")]
		void Redo ();

		[Export ("reset")]
		void Reset ();

		[Export ("rollback")]
		void Rollback ();

		[Export ("save:")]
		bool Save (out NSError error);

#if !WATCH && !TVOS
		[Availability (Introduced = Platform.iOS_3_0 | Platform.Mac_10_4, Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use a queue style context and PerformAndWait instead")]
		[Export ("lock")]
		new void Lock ();

		[Availability (Introduced = Platform.iOS_3_0 | Platform.Mac_10_4, Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use a queue style context and PerformAndWait instead")]
		[Export ("unlock")]
		new void Unlock ();

		[NoTV]
		[Availability (Introduced = Platform.iOS_3_0 | Platform.Mac_10_4, Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use a queue style context and Perform instead")]
		[Export ("tryLock")]
		bool TryLock { get; }
#endif // !WATCH && !TVOS

		[Export ("propagatesDeletesAtEndOfEvent")]
		bool PropagatesDeletesAtEndOfEvent { get; set; }

		[Export ("retainsRegisteredObjects")]
		bool RetainsRegisteredObjects { get; set; }

		[Export ("stalenessInterval")]
		double StalenessInterval { get; set; }

		[Export ("mergePolicy", ArgumentSemantic.Retain)]
		IntPtr MergePolicy { get; set; }

		[Export ("obtainPermanentIDsForObjects:error:")]
		bool ObtainPermanentIDsForObjects (NSManagedObject[] objects, out NSError error);

		[Export ("mergeChangesFromContextDidSaveNotification:")]
		void MergeChangesFromContextDidSaveNotification (NSNotification notification);

		[DesignatedInitializer]
		[Since (5,0)]
		[Export ("initWithConcurrencyType:")]
		IntPtr Constructor (NSManagedObjectContextConcurrencyType ct);

		[Since (5,0)]
		[Export ("performBlock:")]
		void Perform (/* non null */ NSAction action);

		[Since (5,0)]
		[Export ("performBlockAndWait:")]
		void PerformAndWait (/* non null */ NSAction action);

		[Since (5,0)]
		[Export ("userInfo")]
		NSMutableDictionary UserInfo { get; }

		[Since (5,0)]
		[Export ("concurrencyType")]
		NSManagedObjectContextConcurrencyType ConcurrencyType { get; }

		//Detected properties
		[Since (5,0)]
		// default is null, but setting it to null again would crash the app
		[Export ("parentContext", ArgumentSemantic.Retain)]
		NSManagedObjectContext ParentContext { get; set; }

		[Field ("NSManagedObjectContextObjectsDidChangeNotification")]
		[Notification (typeof (NSManagedObjectChangeEventArgs))]
		NSString ObjectsDidChangeNotification { get; }

		[Field ("NSManagedObjectContextDidSaveNotification")]
		[Notification (typeof (NSManagedObjectChangeEventArgs))]
		NSString DidSaveNotification { get; }

		[Field ("NSManagedObjectContextWillSaveNotification")]
		[Notification ()]
		NSString WillSaveNotification { get; }

		[iOS (8,0), Mac(10,10)]
		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[iOS (8,0), Mac(10,10)]
		[Export ("executeRequest:error:")]
		NSPersistentStoreResult ExecuteRequest (NSPersistentStoreRequest request, out NSError error);

		[Export ("existingObjectWithID:error:")]
		NSManagedObject GetExistingObject (NSManagedObjectID objectID, out NSError error);

		[iOS (9,0), Mac (10,11)]
		[Export ("shouldDeleteInaccessibleFaults")]
		bool ShouldDeleteInaccessibleFaults { get; set; }

		[iOS (9,0), Mac (10,11)]
		[Export ("shouldHandleInaccessibleFault:forObjectID:triggeredByProperty:")]
		bool ShouldHandleInaccessibleFault (NSManagedObject fault, NSManagedObjectID oid, [NullAllowed] NSPropertyDescription property);

		[iOS (9,0), Mac (10,11)]
		[Static]
		[Export ("mergeChangesFromRemoteContextSave:intoContexts:")]
		void MergeChangesFromRemoteContextSave (NSDictionary changeNotificationData, NSManagedObjectContext[] contexts);

		[iOS (8,3), Mac (10,11)]
		[Export ("refreshAllObjects")]
		void RefreshAllObjects ();
	}

	public interface NSManagedObjectChangeEventArgs {
		[Export ("NSInsertedObjectsKey")]
		NSSet InsertedObjects { get; }

		[Export ("NSUpdatedObjectsKey")]
		NSSet UpdatedObjects { get; }

		[Export ("NSDeletedObjectsKey")]
		NSSet DeletedObjects { get; }

		[Export ("NSRefreshedObjectsKey")]
		NSSet RefreshedObjects { get; }

		[Export ("NSInvalidatedObjectsKey")]
		NSSet InvalidatedObjects { get; }

		[ProbePresence]
		[Export ("NSInvalidatedAllObjectsKey")]
		bool InvalidatedAllObjects { get; }
	}
	
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -URIRepresentation cannot be sent to an abstract object of class NSManagedObjectID: Create a concrete instance!
	[DisableDefaultCtor]
	public interface NSManagedObjectID : NSCopying {

		[Export ("entity")]
		NSEntityDescription Entity { get; }

		[Export ("persistentStore")]
		NSPersistentStore PersistentStore { get; }

		[Export ("isTemporaryID")]
		bool IsTemporaryID { get; }

		[Export ("URIRepresentation")]
		NSUrl URIRepresentation { get; }

	}

	[BaseType (typeof (NSObject))]
	public interface NSManagedObjectModel : NSCoding, NSCopying {

		[Static, Export ("mergedModelFromBundles:")]
		NSManagedObjectModel MergedModelFromBundles (NSBundle[] bundles);

		[Static, Export ("modelByMergingModels:")]
		NSManagedObjectModel ModelByMergingModels (NSManagedObjectModel[] models);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl url);

		[Export ("entitiesByName")]
		NSDictionary EntitiesByName { get; }

		[Export ("entities", ArgumentSemantic.Retain)]
		NSEntityDescription[] Entities { get; set; }

		[Export ("configurations")]
		string[] Configurations { get; }

		[Export ("entitiesForConfiguration:")]
		string[] EntitiesForConfiguration (string configuration);

		[Export ("setEntities:forConfiguration:")]
		void SetEntities (NSEntityDescription[] entities, string configuration);

		[Export ("setFetchRequestTemplate:forName:")]
		void SetFetchRequestTemplate (NSFetchRequest fetchRequestTemplate, string name);

		[Export ("fetchRequestTemplateForName:")]
		NSFetchRequest FetchRequestTemplateForName (string name);

		[Export ("fetchRequestFromTemplateWithName:substitutionVariables:")]
		NSFetchRequest FetchRequestFromTemplateWithName (string name, NSDictionary variables);

		[NullAllowed] // by default this property is null
		[Export ("localizationDictionary", ArgumentSemantic.Retain)]
		NSDictionary LocalizationDictionary { get; set; }

		[Static, Export ("mergedModelFromBundles:forStoreMetadata:")]
		NSManagedObjectModel MergedModelFromBundles (NSBundle[] bundles, NSDictionary metadata);

		[Static, Export ("modelByMergingModels:forStoreMetadata:")]
		NSManagedObjectModel ModelByMergingModels (NSManagedObjectModel[] models, NSDictionary metadata);

		[Export ("fetchRequestTemplatesByName")]
		NSDictionary FetchRequestTemplatesByName { get; }

		[Export ("versionIdentifiers", ArgumentSemantic.Copy)]
		NSSet VersionIdentifiers { get; set; }

		[Export ("isConfiguration:compatibleWithStoreMetadata:")]
		bool IsConfiguration ([NullAllowed] string configuration, NSDictionary metadata);

		[Export ("entityVersionHashesByName")]
		NSDictionary EntityVersionHashesByName { get; }
	}
	[BaseType (typeof (NSObject))]
	public interface NSMappingModel {

		[Static, Export ("mappingModelFromBundles:forSourceModel:destinationModel:")]
		NSMappingModel MappingModelFromBundles (NSBundle[] bundles, NSManagedObjectModel sourceModel, NSManagedObjectModel destinationModel);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl url);

		[Export ("entityMappings", ArgumentSemantic.Retain)]
		NSEntityMapping[] EntityMappings { get; set; }

		[Export ("entityMappingsByName")]
		NSDictionary EntityMappingsByName { get; }

	}

	[Since(5,0)][Lion]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSMergeConflict {
		[Export ("sourceObject", ArgumentSemantic.Retain)]
		NSManagedObject SourceObject { get;  }

		[Export ("objectSnapshot", ArgumentSemantic.Retain)]
		NSDictionary ObjectSnapshot { get;  }

		[Export ("cachedSnapshot", ArgumentSemantic.Retain)]
		NSDictionary CachedSnapshot { get;  }

		[Export ("persistedSnapshot", ArgumentSemantic.Retain)]
		NSDictionary PersistedSnapshot { get;  }

		[Export ("newVersionNumber")]
		nuint NewVersionNumber { get;  }

		[Export ("oldVersionNumber")]
		nuint OldVersionNumber { get;  }

		[DesignatedInitializer]
		[Export ("initWithSource:newVersion:oldVersion:cachedSnapshot:persistedSnapshot:")]
		IntPtr Constructor (NSManagedObject srcObject, nuint newvers, nuint oldvers, NSDictionary cachesnap, NSDictionary persnap);
	}

	[Since(5,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSMergePolicy {
		[Export ("mergeType")]
		NSMergePolicyType MergeType { get;  }

		[DesignatedInitializer]
		[Export ("initWithMergeType:")]
		IntPtr Constructor (NSMergePolicyType ty);

		[Export ("resolveConflicts:error:")]
		bool ResolveConflictserror (NSMergeConflict [] list, out NSError error);

		[iOS (9,0), Mac (10,11)]
		[Export ("resolveOptimisticLockingVersionConflicts:error:")]
		bool ResolveOptimisticLockingVersionConflicts (NSMergeConflict[] list, [NullAllowed] out NSError error);

		[iOS (9,0), Mac (10,11)]
		[Export ("resolveConstraintConflicts:error:")]
		bool ResolveConstraintConflicts (NSConstraintConflict[] list, [NullAllowed] out NSError error);
	}

	[BaseType (typeof (NSObject))]
	public interface NSMigrationManager {

		[Export ("initWithSourceModel:destinationModel:")]
		IntPtr Constructor (NSManagedObjectModel sourceModel, NSManagedObjectModel destinationModel);

		[Export ("migrateStoreFromURL:type:options:withMappingModel:toDestinationURL:destinationType:destinationOptions:error:")]
		bool MigrateStoreFromUrl (NSUrl sourceURL, string sStoreType, NSDictionary sOptions, NSMappingModel mappings, NSUrl dURL, string dStoreType, NSDictionary dOptions, out NSError error);

		[Export ("reset")]
		void Reset ();

		[Export ("mappingModel")]
		NSMappingModel MappingModel { get; }

		[Export ("sourceModel")]
		NSManagedObjectModel SourceModel { get; }

		[Export ("destinationModel")]
		NSManagedObjectModel DestinationModel { get; }

		[Export ("sourceContext")]
		NSManagedObjectContext SourceContext { get; }

		[Export ("destinationContext")]
		NSManagedObjectContext DestinationContext { get; }

		[Export ("sourceEntityForEntityMapping:")]
		NSEntityDescription SourceEntityForEntityMapping (NSEntityMapping mEntity);

		[Export ("destinationEntityForEntityMapping:")]
		NSEntityDescription DestinationEntityForEntityMapping (NSEntityMapping mEntity);

		[Export ("associateSourceInstance:withDestinationInstance:forEntityMapping:")]
		void AssociateSourceInstance (NSManagedObject sourceInstance, NSManagedObject destinationInstance, NSEntityMapping entityMapping);

		[Export ("destinationInstancesForEntityMappingNamed:sourceInstances:")]
		NSManagedObject[] DestinationInstancesForEntityMappingNamed (string mappingName, NSManagedObject[] sourceInstances);

		[Export ("sourceInstancesForEntityMappingNamed:destinationInstances:")]
		NSManagedObject[] SourceInstancesForEntityMappingNamed (string mappingName, NSManagedObject[] destinationInstances);

		[Export ("currentEntityMapping")]
		NSEntityMapping CurrentEntityMapping { get; }

		[Export ("migrationProgress")]
		float MigrationProgress { get; }  /* float, not CGFloat */

		[NullAllowed] // by default this property is null
		[Export ("userInfo", ArgumentSemantic.Retain)]
		NSDictionary UserInfo { get; set; }

		[Export ("cancelMigrationWithError:")]
		void CancelMigrationWithError (NSError error);

		// 5.0
		[Since(5,0)]
		[Export ("usesStoreSpecificMigrationManager")]
		bool UsesStoreSpecificMigrationManager { get; set; }
	}

	// NSPersistentStore is an abstract type according to Apple's documentation, but Apple
	// also have internal subclasses of NSPersistentStore, and in those cases our closest
	// type is NSPersistentStore, which means we must be able to create managed wrappers
	// for such native classes using the managed NSPersistentStore. This means we can't
	// make our managed version [Abstract].
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface NSPersistentStore {

		[Static, Export ("metadataForPersistentStoreWithURL:error:")]
		NSDictionary MetadataForPersistentStoreWithUrl (NSUrl url, out NSError error);

		[Static, Export ("setMetadata:forPersistentStoreWithURL:error:")]
		bool SetMetadata (NSDictionary metadata, NSUrl url, out NSError error);

#if XAMCORE_4_0
		[Protected]
#endif
		[DesignatedInitializer]
		[Export ("initWithPersistentStoreCoordinator:configurationName:URL:options:")]
		IntPtr Constructor (NSPersistentStoreCoordinator root, string name, NSUrl url, NSDictionary options);

		[Export ("persistentStoreCoordinator")]
		NSPersistentStoreCoordinator PersistentStoreCoordinator { get; }

		[Export ("configurationName")]
		string ConfigurationName { get; }

		[Export ("options")]
		NSDictionary Options { get; }

		[NullAllowed] // by default this property is null
		[Export ("URL", ArgumentSemantic.Retain)]
		NSUrl Url { get; set; }

		[Export ("identifier")]
		string Identifier { get; set; }

		[Export ("type")]
		string Type { get; }

		[Export ("isReadOnly")]
		bool ReadOnly { get; [Bind("setReadOnly:")] set; }

		[Export ("metadata", ArgumentSemantic.Retain)]
		NSDictionary Metadata { get; set; }

		[Export ("didAddToPersistentStoreCoordinator:")]
		void DidAddToPersistentStoreCoordinator (NSPersistentStoreCoordinator coordinator);

		[Export ("willRemoveFromPersistentStoreCoordinator:")]
		void WillRemoveFromPersistentStoreCoordinator (NSPersistentStoreCoordinator coordinator);

		[Since (5,0)]
		[Field ("NSPersistentStoreSaveConflictsErrorKey")]
		NSString SaveConflictsErrorKey { get; }

	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // iOS8 -> Core Data: warning: client failed to call designated initializer on NSPersistentStoreCoordinator
	public partial interface NSPersistentStoreCoordinator
#if !WATCH && !TVOS
		: NSLocking
#endif // !WATCH
	{

		[Static, Export ("registeredStoreTypes")]
		NSDictionary RegisteredStoreTypes { get; }

		[Static, Export ("registerStoreClass:forStoreType:")]
		void RegisterStoreClass (Class storeClass, NSString storeType);

		[Static, Export ("metadataForPersistentStoreOfType:URL:error:")]
		NSDictionary MetadataForPersistentStoreOfType (NSString storeType, NSUrl url, out NSError error);

		[Static, Export ("setMetadata:forPersistentStoreOfType:URL:error:")]
		bool SetMetadata (NSDictionary metadata, NSString storeType, NSUrl url, out NSError error);

		[Export ("setMetadata:forPersistentStore:")]
		void SetMetadata (NSDictionary metadata, NSPersistentStore store);

		[Export ("metadataForPersistentStore:")]
		NSDictionary MetadataForPersistentStore (NSPersistentStore store);

		[DesignatedInitializer]
		[Export ("initWithManagedObjectModel:")]
		IntPtr Constructor (NSManagedObjectModel model);

		[Export ("managedObjectModel")]
		NSManagedObjectModel ManagedObjectModel { get; }

		[Export ("persistentStores")]
		NSPersistentStore[] PersistentStores { get; }

		[Export ("persistentStoreForURL:")]
		NSPersistentStore PersistentStoreForUrl (NSUrl URL);

		[Export ("URLForPersistentStore:")]
		NSUrl UrlForPersistentStore (NSPersistentStore store);

		[Export ("setURL:forPersistentStore:")]
		bool SetUrl (NSUrl url, NSPersistentStore store);

		[Export ("addPersistentStoreWithType:configuration:URL:options:error:")]
		NSPersistentStore AddPersistentStoreWithType (NSString storeType, [NullAllowed] string configuration, NSUrl storeURL, [NullAllowed] NSDictionary options, out NSError error);

		[Export ("removePersistentStore:error:")]
		bool RemovePersistentStore (NSPersistentStore store, out NSError error);

		[Export ("migratePersistentStore:toURL:options:withType:error:")]
		NSPersistentStore MigratePersistentStore (NSPersistentStore store, NSUrl URL, NSDictionary options, NSString storeType, out NSError error);

		[Export ("managedObjectIDForURIRepresentation:")]
		NSManagedObjectID ManagedObjectIDForURIRepresentation (NSUrl url);

#if !WATCH && !TVOS
		[Availability (Introduced = Platform.iOS_3_0, Deprecated = Platform.iOS_8_0, Message="Use PerformAndWait instead")]
		[Export ("lock")]
		new void Lock ();

		[Availability (Introduced = Platform.iOS_3_0, Deprecated = Platform.iOS_8_0, Message="Use PerformAndWait instead")]
		[Export ("unlock")]
		new void Unlock ();

		[NoTV]
		[Availability (Introduced = Platform.iOS_3_0, Deprecated = Platform.iOS_8_0, Message="Use Perform instead")]
		[Export ("tryLock")]
		bool TryLock { get; }
#endif // !WATCH && !TVOS

#if MONOMAC
		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_5)]
		[Static, Export ("metadataForPersistentStoreWithURL:error:")]
		NSDictionary MetadataForPersistentStoreWithUrl (NSUrl url, out NSError error);
#endif
		[Field ("NSSQLiteStoreType")]
		NSString SQLiteStoreType { get; }
#if MONOMAC
		[Field ("NSXMLStoreType")]
		NSString XMLStoreType { get; }
#endif	
		[Field ("NSBinaryStoreType")]
		NSString BinaryStoreType { get; }
		
		[Field ("NSInMemoryStoreType")]
		NSString InMemoryStoreType { get; }

		[Field ("NSStoreUUIDKey")]
		NSString StoreUUIDKey { get; }

		[Field ("NSAddedPersistentStoresKey")]
		NSString AddedPersistentStoresKey { get; }

		[Field ("NSRemovedPersistentStoresKey")]
		NSString RemovedPersistentStoresKey { get; }

		[Field ("NSUUIDChangedPersistentStoresKey")]
		NSString UUIDChangedPersistentStoresKey { get; }

		[Field ("NSReadOnlyPersistentStoreOption")]
		NSString ReadOnlyPersistentStoreOption { get; }
#if MONOMAC
		[Field ("NSValidateXMLStoreOption")]
		NSString ValidateXMLStoreOption { get; }
#endif
		[Field ("NSPersistentStoreTimeoutOption")]
		NSString PersistentStoreTimeoutOption { get; }

		[Field ("NSSQLitePragmasOption")]
		NSString SQLitePragmasOption { get; }

		[Field ("NSSQLiteAnalyzeOption")]
		NSString SQLiteAnalyzeOption { get; }

		[Field ("NSSQLiteManualVacuumOption")]
		NSString SQLiteManualVacuumOption { get; }

		[Field ("NSIgnorePersistentStoreVersioningOption")]
		NSString IgnorePersistentStoreVersioningOption { get; }

		[Field ("NSMigratePersistentStoresAutomaticallyOption")]
		NSString MigratePersistentStoresAutomaticallyOption { get; }

		[Field ("NSInferMappingModelAutomaticallyOption")]
		NSString InferMappingModelAutomaticallyOption { get; }

		[Field ("NSStoreModelVersionHashesKey")]
		NSString StoreModelVersionHashesKey { get; }

		[Field ("NSStoreModelVersionIdentifiersKey")]
		NSString StoreModelVersionIdentifiersKey { get; }

		[Field ("NSPersistentStoreOSCompatibility")]
		NSString PersistentStoreOSCompatibility { get; }

		[Field ("NSStoreTypeKey")]
		NSString StoreTypeKey { get; }

		[Notification]
		[Field ("NSPersistentStoreCoordinatorStoresDidChangeNotification")]
		NSString StoresDidChangeNotification { get; }
		
		[Notification]
		[Field ("NSPersistentStoreCoordinatorWillRemoveStoreNotification")]
		NSString WillRemoveStoreNotification { get; }
		
		// 5.0
		[Since(5,0)]
		[Export ("executeRequest:withContext:error:")]
		NSObject ExecuteRequestwithContexterror (NSPersistentStoreRequest request, NSManagedObjectContext context, out NSError error);

		[NoWatch][NoTV]
		[Notification]
		[Since (5,0)]
		[Field ("NSPersistentStoreDidImportUbiquitousContentChangesNotification")]
		NSString DidImportUbiquitousContentChangesNotification { get; }

		[NoWatch][NoTV]
		[Since (5,0)]
		[Field ("NSPersistentStoreUbiquitousContentNameKey")]
		NSString PersistentStoreUbiquitousContentNameKey { get; }

		[NoWatch][NoTV]
		[Since (5,0)]
		[Field ("NSPersistentStoreUbiquitousContentURLKey")]
#if XAMCORE_4_0
		NSString PersistentStoreUbiquitousContentUrlKey { get; }
#else
		NSString PersistentStoreUbiquitousContentUrlLKey { get; }
#endif

#if !MONOMAC
		[Since (5,0)]
		[Field ("NSPersistentStoreFileProtectionKey")]
		NSString PersistentStoreFileProtectionKey { get; }
#endif

		// 7.0

		[NoWatch][NoTV]
		[Since (7,0), Mavericks]
		[Field ("NSPersistentStoreUbiquitousPeerTokenOption")]
		NSString PersistentStoreUbiquitousPeerTokenOption { get; }

		[NoWatch][NoTV]
		[Since (7,0), Mavericks]
		[Static]
		[Export ("removeUbiquitousContentAndPersistentStoreAtURL:options:error:")]
		bool RemoveUbiquitousContentAndPersistentStore (NSUrl storeUrl, NSDictionary options, out NSError error);

		[Since (7,0), Mavericks]
		[Notification (typeof (NSPersistentStoreCoordinatorStoreChangeEventArgs))]
		[Field ("NSPersistentStoreCoordinatorStoresWillChangeNotification")]
		NSString StoresWillChangeNotification { get; }

		[NoWatch][NoTV]
		[Since (7,0), Mavericks]
		[Field ("NSPersistentStoreRebuildFromUbiquitousContentOption")]
		NSString RebuildFromUbiquitousContentOption { get; }

		[NoWatch][NoTV]
		[Since (7,0), Mavericks]
		[Field ("NSPersistentStoreRemoveUbiquitousMetadataOption")]
		NSString RemoveUbiquitousMetadataOption { get; }

		[NoWatch][NoTV]
		[Since (7,0), Mavericks]
		[Field ("NSPersistentStoreUbiquitousContainerIdentifierKey")]
		NSString eUbiquitousContainerIdentifierKey { get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("name")]
		string Name { get; set; }

		[iOS (8,0), Mac (10,10)]
		[Export ("performBlock:")]
		void Perform (Action code);

		[iOS (8,0), Mac (10,10)]
		[Export ("performBlockAndWait:")]
		void PerformAndWait (Action code);

		[iOS (9,0), Mac (10,11)]
		[Export ("destroyPersistentStoreAtURL:withType:options:error:")]
		bool DestroyPersistentStore (NSUrl url, string storeType, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);

		[iOS (9,0), Mac (10,11)]
		[Export ("replacePersistentStoreAtURL:destinationOptions:withPersistentStoreFromURL:sourceOptions:storeType:error:")]
		bool ReplacePersistentStore (NSUrl destinationURL, [NullAllowed] NSDictionary destinationOptions, NSUrl sourceURL, [NullAllowed] NSDictionary sourceOptions, string storeType, [NullAllowed] out NSError error);
	}

	interface NSPersistentStoreCoordinatorStoreChangeEventArgs {
		[NoWatch][NoTV]
		[Export ("NSPersistentStoreUbiquitousTransitionTypeKey")]
		NSPersistentStoreUbiquitousTransitionType EventType { get; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSPersistentStoreRequest : NSCopying {
		[Export ("requestType")]
		NSPersistentStoreRequestType RequestType { get; }

		//Detected properties
		[NullAllowed] // by default this property is null
		[Export ("affectedStores", ArgumentSemantic.Retain)]
		NSPersistentStore [] AffectedStores { get; set; }
	}

	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSPersistentStoreAsynchronousResult))]
	interface NSAsynchronousFetchResult {
		[Export ("fetchRequest", ArgumentSemantic.Retain)]
		NSAsynchronousFetchRequest FetchRequest { get; }

		// TODO: strong type
		[Export ("finalResult", ArgumentSemantic.Retain)]
		NSObject [] FinalResult { get; }
	}

	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSObject))]
	public interface NSPersistentStoreResult {

	}

	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSPersistentStoreResult))]
	interface NSBatchUpdateResult {
		[Export ("result", ArgumentSemantic.Retain)]
		NSObject Result { get; }

		[Export ("resultType")]
		NSBatchUpdateRequestResultType ResultType { get; }
	}
	
	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSPersistentStoreResult))]
	interface NSPersistentStoreAsynchronousResult {
		[Export ("managedObjectContext", ArgumentSemantic.Retain)]
		NSManagedObjectContext ManagedObjectContext { get; }

		[Export ("operationError", ArgumentSemantic.Retain)]
		NSError OperationError { get; }

		[Export ("progress", ArgumentSemantic.Retain)]
		NSProgress Progress { get; }

		[Export ("cancel")]
		void Cancel ();
	}
	
	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSAsynchronousFetchRequest {
		[Export ("initWithFetchRequest:completionBlock:")]
		IntPtr Constructor (NSFetchRequest request, Action<NSAsynchronousFetchResult> completion);

		[Export ("fetchRequest", ArgumentSemantic.Retain)]
		NSFetchRequest FetchRequest { get; }

		[Export ("estimatedResultCount")]
		nint EstimatedResultCount { get; set; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSPropertyDescription : NSCoding, NSCopying {

		[Export ("entity")]
		NSEntityDescription Entity { get; }

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[Export ("isOptional")]
		bool Optional { get; [Bind("setOptional:")] set; }

		[Export ("isTransient")]
		bool Transient { get; [Bind("setTransient:")] set; }

		[Export ("validationPredicates")]
		NSPredicate[] ValidationPredicates { get; }

		[Export ("validationWarnings")]
		string[] ValidationWarnings { get; }

		[Export ("setValidationPredicates:withValidationWarnings:")]
		void SetValidationPredicates (NSPredicate[] validationPredicates, string[] validationWarnings);

		[Export ("userInfo", ArgumentSemantic.Retain)]
		NSDictionary UserInfo { get; set; }

		[Export ("isIndexed")]
		bool Indexed { get; [Bind("setIndexed:")] set; }

		[Export ("versionHash")]
		NSData VersionHash { get; }

		[NullAllowed] // by default this property is null
		[Export ("versionHashModifier")]
		string VersionHashModifier { get; set; }

		// 5.0
		[Since (5,0)]
		[Export ("indexedBySpotlight")]
		bool IndexedBySpotlight { [Bind ("isIndexedBySpotlight")]get; set; }

		[Since (5,0)]
		[Export ("storedInExternalRecord")]
		bool StoredInExternalRecord { [Bind ("isStoredInExternalRecord")]get; set; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSPropertyMapping {

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("valueExpression", ArgumentSemantic.Retain)]
		NSExpression ValueExpression { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("userInfo", ArgumentSemantic.Retain)]
		NSDictionary UserInfo { get; set; }
	}

	[BaseType (typeof (NSPropertyDescription))]
	public interface NSRelationshipDescription {

		[NullAllowed] // by default this property is null
		[Export ("destinationEntity")]
		NSEntityDescription DestinationEntity { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("inverseRelationship")]
		NSRelationshipDescription InverseRelationship { get; set; }

		[Export ("maxCount")]
		nuint MaxCount { get; set; }

		[Export ("minCount")]
		nuint MinCount { get; set; }

		[Export ("deleteRule")]
		NSDeleteRule DeleteRule { get; set; }

		[Export ("isToMany")]
		bool IsToMany { get; }

		[Export ("versionHash")]
		NSData VersionHash { get; }

		// 5.0
		[Since (5,0)]
		[Export ("ordered")]
		bool Ordered { [Bind ("isOrdered")]get; set; }
	}

	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSSaveChangesRequest {
		[Export ("initWithInsertedObjects:updatedObjects:deletedObjects:lockedObjects:")]
		IntPtr Constructor (NSSet insertedObjects, NSSet updatedObjects, NSSet deletedObjects, NSSet lockedObjects);

		[Export ("insertedObjects")]
		NSSet InsertedObjects { get; }

		[Export ("updatedObjects")]
		NSSet UpdatedObjects { get; }

		[Export ("deletedObjects")]
		NSSet DeletedObjects { get; }

		[Export ("lockedObjects")]
		NSSet LockedObjects { get; }
	}

	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSBatchUpdateRequest {
		[Export ("initWithEntityName:")]
		IntPtr Constructor (string entityName);

		[Export ("initWithEntity:")]
		IntPtr Constructor (NSEntityDescription entity);

		[Export ("entityName")]
		string EntityName { get; }

		[Export ("entity", ArgumentSemantic.Retain)]
		NSEntityDescription Entity { get; }

		[NullAllowed] // by default this property is null
		[Export ("predicate", ArgumentSemantic.Retain)]
		NSPredicate Predicate { get; set; }

		[Export ("includesSubentities")]
		bool IncludesSubentities { get; set; }

		[Export ("resultType")]
		NSBatchUpdateRequestResultType ResultType { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("propertiesToUpdate", ArgumentSemantic.Copy)]
		NSDictionary PropertiesToUpdate { get; set; }

		[Static, Export ("batchUpdateRequestWithEntityName:")]
		NSBatchUpdateRequest BatchUpdateRequestWithEntityName (string entityName);
	}

	[iOS (9,0), Mac (10,11)]
	[BaseType (typeof(NSPersistentStoreRequest))]
	[DisableDefaultCtor]
	interface NSBatchDeleteRequest
	{
		[Export ("initWithFetchRequest:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSFetchRequest fetch);

		[Export ("initWithObjectIDs:")]
		IntPtr Constructor (NSManagedObjectID[] objects);

		[Export ("resultType", ArgumentSemantic.Assign)]
		NSBatchDeleteRequestResultType ResultType { get; set; }

		[Export ("fetchRequest", ArgumentSemantic.Copy)]
		NSFetchRequest FetchRequest { get; }
	}

	[iOS (9,0), Mac (10,11)]
	[BaseType (typeof(NSPersistentStoreResult))]
	interface NSBatchDeleteResult
	{
		[NullAllowed, Export ("result", ArgumentSemantic.Strong)]
		NSObject Result { get; }

		[Export ("resultType")]
		NSBatchDeleteRequestResultType ResultType { get; }
	}

	[iOS (9,0), Mac (10,11)]
	[BaseType (typeof(NSObject))]
	interface NSConstraintConflict
	{
		[Export ("initWithConstraint:databaseObject:databaseSnapshot:conflictingObjects:conflictingSnapshots:")]
		[DesignatedInitializer]
		IntPtr Constructor (string[] contraint, [NullAllowed] NSManagedObject databaseObject, [NullAllowed] NSDictionary databaseSnapshot, NSManagedObject[] conflictingObjects, NSObject[] conflictingSnapshots);

#if MONOMAC
		[Export ("constraint", ArgumentSemantic.Copy)]
#else
		[Export ("constraint", ArgumentSemantic.Retain)]
#endif
		string[] Constraint { get; }

#if MONOMAC
		[Export ("constraintValues", ArgumentSemantic.Copy)]
#else
		[Export ("constraintValues", ArgumentSemantic.Retain)]
#endif
		NSDictionary<NSString, NSObject> ConstraintValues { get; }

		[NullAllowed, Export ("databaseObject", ArgumentSemantic.Retain)]
		NSManagedObject DatabaseObject { get; }

		[NullAllowed, Export ("databaseSnapshot", ArgumentSemantic.Retain)]
		NSDictionary<NSString, NSObject> DatabaseSnapshot { get; }

#if MONOMAC
		[Export ("conflictingObjects", ArgumentSemantic.Copy)]
#else
		[Export ("conflictingObjects", ArgumentSemantic.Retain)]
#endif
		NSManagedObject[] ConflictingObjects { get; }

#if MONOMAC
		[Export ("conflictingSnapshots", ArgumentSemantic.Copy)]
#else
		[Export ("conflictingSnapshots", ArgumentSemantic.Retain)]
#endif
		NSDictionary[] ConflictingSnapshots { get; }
	}
}
