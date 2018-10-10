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
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#endif
#if !WATCH
using CoreSpotlight;
#endif

namespace CoreData
{
	[StrongDictionary ("UserInfoKeys")]
	interface UserInfo {
		NSError [] DetailedErrors { get; set; }
		NSManagedObject ObjectForValidationError { get; set; }
		NSString KeyForValidationError { get; set; }
		NSPredicate PredicateForValidationError { get; set; }
		NSValue ValueForValidationError { get; set; }
		NSMergeConflict [] PersistentStoreSaveConflicts { get; set; }
		NSPersistentStore [] AffectedStoresForError { get; set; }
	}

	[Static]
	interface UserInfoKeys {
		[Field ("NSDetailedErrorsKey")]
		NSString DetailedErrorsKey { get; }

		[Field ("NSValidationObjectErrorKey")]
		NSString ObjectForValidationErrorKey { get; }

		[Field ("NSValidationKeyErrorKey")]
		NSString KeyForValidationErrorKey { get; }

		[Field ("NSValidationPredicateErrorKey")]
		NSString PredicateForValidationErrorKey { get; }

		[Field ("NSValidationValueErrorKey")]
		NSString ValueForValidationErrorKey { get; }

		[Field ("NSPersistentStoreSaveConflictsErrorKey")]
		NSString PersistentStoreSaveConflictsKey { get; }

		[Field ("NSAffectedStoresErrorKey")]
		NSString AffectedStoresForErrorKey { get; }
	}

	[NoWatch][NoTV]
	[Native] // NUInteger -> NSPersistentStoreCoordinator.h
	[iOS (7, 0)]
	[Deprecated (PlatformName.iOS, 10, 0, message : "Please see the release notes and Core Data documentation.")]
	[Mac (10, 9)]
	[Deprecated (PlatformName.MacOSX, 10, 12, message : "Please see the release notes and Core Data documentation.")]
	public enum NSPersistentStoreUbiquitousTransitionType : ulong {
		AccountAdded = 1,
		AccountRemoved,
		ContentRemoved,
		InitialImportCompleted
	}

	[Native]
	public enum NSSnapshotEventType : ulong {
		UndoInsertion = 1 << 1,
		UndoDeletion = 1 << 2,
		UndoUpdate = 1 << 3,
		Rollback = 1 << 4,
		Refresh = 1 << 5,
		MergePolicy = 1 << 6
	}

	[BaseType (typeof (NSPersistentStore))]
	// Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: NSMappedObjectStore must be initialized with initWithPersistentStoreCoordinator:configurationName:URL:options
	[DisableDefaultCtor]
	interface NSAtomicStore {

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
#if XAMCORE_4_0
		NSSet<NSAtomicStoreCacheNode> CacheNodes { get; }
#else
		NSSet CacheNodes { get; }
#endif

		[Export ("addCacheNodes:")]
		
#if XAMCORE_4_0
		void AddCacheNodes (NSSet<NSAtomicStoreCacheNode> cacheNodes);
#else
		void AddCacheNodes (NSSet cacheNodes);
#endif

		[Export ("willRemoveCacheNodes:")]
#if XAMCORE_4_0
		void WillRemoveCacheNodes (NSSet<NSAtomicStoreCacheNode> cacheNodes);
#else
		void WillRemoveCacheNodes (NSSet cacheNodes);
#endif

		[Export ("cacheNodeForObjectID:")]
		[return: NullAllowed]
		NSAtomicStoreCacheNode CacheNodeForObjectID (NSManagedObjectID objectID);

		[Export ("objectIDForEntity:referenceObject:")]
		NSManagedObjectID ObjectIDForEntity (NSEntityDescription entity, NSObject data);

		[Export ("newReferenceObjectForManagedObject:")]
		NSAtomicStore NewReferenceObjectForManagedObject (NSManagedObject managedObject);

		[Export ("referenceObjectForObjectID:")]
		NSAtomicStore ReferenceObjectForObjectID (NSManagedObjectID objectID);
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface NSFetchIndexElementDescription : NSCoding
	{
		[Export ("initWithProperty:collationType:")]
		IntPtr Constructor (NSPropertyDescription property, NSFetchIndexElementType collationType);

		[NullAllowed, Export ("property", ArgumentSemantic.Retain)]
		NSPropertyDescription Property { get; }

		[NullAllowed, Export ("propertyName", ArgumentSemantic.Retain)]
		string PropertyName { get; }

		[Export ("collationType", ArgumentSemantic.Assign)]
		NSFetchIndexElementType CollationType { get; set; }

		[Export ("ascending")]
		bool IsAscending { [Bind ("isAscending")] get; set; }

		[NullAllowed, Export ("indexDescription", ArgumentSemantic.Assign)]
		NSFetchIndexDescription IndexDescription { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface NSFetchIndexDescription : NSCoding
	{
		[Export ("initWithName:elements:")]
		IntPtr Constructor (string name, [NullAllowed] NSFetchIndexElementDescription[] elements);

		[Export ("name")]
		string Name { get; set; }

		[Export ("elements", ArgumentSemantic.Copy)]
		NSFetchIndexElementDescription[] Elements { get; set; }

		[NullAllowed, Export ("entity", ArgumentSemantic.Assign)]
		NSEntityDescription Entity { get; }

		[NullAllowed, Export ("partialIndexPredicate", ArgumentSemantic.Copy)]
		NSPredicate PartialIndexPredicate { get; set; }
	}

	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: NSAtomicStoreCacheNodes must be initialized using initWithObjectID:(NSManagedObjectID *)
	[DisableDefaultCtor]
	interface NSAtomicStoreCacheNode {

		[Export ("initWithObjectID:")]
		IntPtr Constructor (NSManagedObjectID moid);

		[Export ("objectID", ArgumentSemantic.Strong)]
		NSManagedObjectID ObjectID { get; }

		[NullAllowed, Export ("propertyCache", ArgumentSemantic.Retain)]
#if XAMCORE_4_0
		NSMutableDictionary<NSString, NSObject> PropertyCache { get; set; }
#else
		NSDictionary PropertyCache { get; set; }
#endif

		[Export ("valueForKey:")]
		[return: NullAllowed]
		NSAtomicStoreCacheNode ValueForKey (string key);

		[Export ("setValue:forKey:")]
		void SetValue ([NullAllowed] NSObject value, string key);

	}
	[BaseType (typeof (NSPropertyDescription))]
	interface NSAttributeDescription {

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

		[Export ("allowsExternalBinaryDataStorage")]
		bool AllowsExternalBinaryDataStorage { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSEntityDescription : NSCoding, NSCopying {

		[Static, Export ("entityForName:inManagedObjectContext:")]
		[return: NullAllowed]
		NSEntityDescription EntityForName (string entityName, NSManagedObjectContext context);

		[Static, Export ("insertNewObjectForEntityForName:inManagedObjectContext:")]
#if !XAMCORE_4_0
		NSObject InsertNewObjectForEntityForName (string entityName, NSManagedObjectContext context);
#else
		NSManagedObject InsertNewObject (string entityName, NSManagedObjectContext context);
#endif

		[Export ("managedObjectModel")]
		NSManagedObjectModel ManagedObjectModel { get; }

		[Export ("managedObjectClassName")]
		string ManagedObjectClassName { get; set; }

		[NullAllowed, Export ("renamingIdentifier")]
		string RenamingIdentifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[Export ("abstract")]
		bool Abstract { [Bind("isAbstract")] get; set; }

		[Export ("subentitiesByName")]
#if XAMCORE_4_0
		NSDictionary<NSString, NSEntityDescription> SubentitiesByName { get; }
#else
		NSDictionary SubentitiesByName { get; }
#endif

		[Export ("subentities", ArgumentSemantic.Retain)]
		NSEntityDescription[] Subentities { get; set; }

		[NullAllowed, Export ("superentity")]
		NSEntityDescription Superentity { get; }

		[Export ("propertiesByName")]
#if XAMCORE_4_0
		NSDictionary<NSString, NSPropertyDescription> PropertiesByName { get; }
#else
		NSDictionary PropertiesByName { get; }
#endif

		[Export ("properties", ArgumentSemantic.Retain)]
		NSPropertyDescription[] Properties { get; set; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Retain)]
		NSDictionary UserInfo { get; set; }

		[Export ("attributesByName")]
#if XAMCORE_4_0
		NSDictionary<NSString, NSAttributeDescription> AttributesByName { get; }
#else
		NSDictionary AttributesByName { get; }
#endif

		[Export ("relationshipsByName")]
#if XAMCORE_4_0
		NSDictionary<NSString, NSRelationshipDescription> RelationshipsByName { get; }
#else
		NSDictionary RelationshipsByName { get; }
#endif

		[Export ("relationshipsWithDestinationEntity:")]
		NSRelationshipDescription[] RelationshipsWithDestinationEntity (NSEntityDescription entity);

		[Export ("isKindOfEntity:")]
		bool IsKindOfEntity (NSEntityDescription entity);

		[Export ("versionHash")]
		NSData VersionHash { get; }

		[NullAllowed] // by default this property is null
		[Export ("versionHashModifier")]
		string VersionHashModifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("compoundIndexes", ArgumentSemantic.Retain)]
		[Mac (10, 7)]
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message : "Use 'NSEntityDescription.Indexes' instead.")]
		NSPropertyDescription [] CompoundIndexes { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed] // by default this property is null
		[Export ("indexes", ArgumentSemantic.Copy)]
		NSFetchIndexDescription[] Indexes { get; set; }

		// @property (strong) NSArray<NSArray<id __nonnull> * __nonnull> * __nonnull uniquenessConstraints __attribute__((availability(ios, introduced=9.0)));
		[iOS (9,0), Mac (10,11)]
		[Internal, Export ("uniquenessConstraints", ArgumentSemantic.Strong)]
		NSArray _UniquenessConstraints { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("coreSpotlightDisplayNameExpression", ArgumentSemantic.Retain)]
		NSExpression CoreSpotlightDisplayNameExpression { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSEntityMapping {

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
	interface NSEntityMigrationPolicy {

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
	interface NSExpressionDescription {

		[NullAllowed, Export ("expression", ArgumentSemantic.Strong)]
		NSExpression Expression { get; set; }

		[Export ("expressionResultType")]
		NSAttributeType ResultType { get; set; }
	}

	[BaseType (typeof (NSPropertyDescription))]
	interface NSFetchedPropertyDescription {

		[NullAllowed] // by default this property is null
		[Export ("fetchRequest", ArgumentSemantic.Retain)]
		NSFetchRequest FetchRequest { get; set; }
	}

	[DisableDefaultCtor]
	[BaseType (typeof (NSExpression))]
	interface NSFetchRequestExpression {

		[Internal]
		[DesignatedInitializer]
		[Export ("initWithExpressionType:")]
		IntPtr Constructor (NSExpressionType type);

		[Static, Export ("expressionForFetch:context:countOnly:")]
		NSFetchRequestExpression FromFetch (NSExpression fetch, NSExpression context, bool countOnly);

		[Export ("requestExpression")]
		NSExpression Request { get; }

		[Export ("contextExpression")]
		NSExpression Context { get; }

		[Export ("countOnlyRequest")]
		bool IsCountOnly { [Bind ("isCountOnlyRequest")] get;}
	}

	interface INSFetchRequestResult {}

	[Watch (3,0), TV (10,0), iOS (10,0), Mac (10,12)]
	[Protocol]
	interface NSFetchRequestResult {}

	[DisableDefaultCtor] // designated
	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSFetchRequest : NSCoding {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

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

		[Static]
		[Export ("fetchRequestWithEntityName:")]
		// note: Xcode 6.3 changed the return value type from `NSFetchRequest*` to `instancetype`
		NSFetchRequest FromEntityName (string entityName);

		[Export ("initWithEntityName:")]
		IntPtr Constructor (string entityName);

		[NullAllowed, Export ("entityName", ArgumentSemantic.Strong)]
		string EntityName { get; }

		[Export ("fetchBatchSize")]
		nint FetchBatchSize { get; set; }

		[Export ("shouldRefreshRefetchedObjects")]
		bool ShouldRefreshRefetchedObjects { get; set; }

		[Export ("havingPredicate", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSPredicate HavingPredicate { get; set; }

		[Export ("propertiesToGroupBy", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSPropertyDescription [] PropertiesToGroupBy { get; set; }

		[Watch (3,0), TV (10,0), iOS (10,0), Mac (10,12)]
		[Export ("execute:")]
		[return: NullAllowed]
		INSFetchRequestResult[] Execute (out NSError error);
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

		[NullAllowed, Export ("cacheName")]
		string CacheName { get; }

		[Export ("fetchedObjects")]
		NSObject[] FetchedObjects { get; }

		[NullAllowed, Export ("fetchRequest")]
		NSFetchRequest FetchRequest { get; }

		[Export ("managedObjectContext")]
		NSManagedObjectContext ManagedObjectContext { get; }

		[NullAllowed, Export ("sectionNameKeyPath")]
		string SectionNameKeyPath { get; }

		[NullAllowed, Export ("sections")]
		INSFetchedResultsSectionInfo[] Sections { get; }

		[Export ("performFetch:")]
		bool PerformFetch (out NSError error);

		[Export ("indexPathForObject:")]
		[return: NullAllowed]
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
		[return: NullAllowed]
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
		void DidChangeObject (NSFetchedResultsController controller, NSObject anObject, [NullAllowed] NSIndexPath indexPath, NSFetchedResultsChangeType type, [NullAllowed] NSIndexPath newIndexPath);

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

		[NullAllowed, Export ("objects")]
		[Abstract]
		NSObject[] Objects { get; }

		[Export ("name")]
		[Abstract]
		string Name { get; }

		[NullAllowed, Export ("indexTitle")]
		[Abstract]
		string IndexTitle { get; }
	}

	interface INSFetchedResultsSectionInfo {}
#endif
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
		[return: NullAllowed]
		NSObject ExecuteRequest (NSPersistentStoreRequest request, [NullAllowed] NSManagedObjectContext context, out NSError error);

		[Export ("newValuesForObjectWithID:withContext:error:")]
		[return: NullAllowed]
		NSIncrementalStoreNode NewValues (NSManagedObjectID forObjectId, NSManagedObjectContext context, out NSError error);

		[Export ("newValueForRelationship:forObjectWithID:withContext:error:")]
		[return: NullAllowed]
		NSObject NewValue (NSRelationshipDescription forRelationship, NSManagedObjectID forObjectI, [NullAllowed] NSManagedObjectContext context, out NSError error);

		[Static]
		[Export ("identifierForNewStoreAtURL:")]
#if XAMCORE_4_0
		NSObject IdentifierForNewStore (NSUrl storeUrl);
#else
		NSObject IdentifierForNewStoreAtURL (NSUrl storeUrl);
#endif

		[Export ("obtainPermanentIDsForObjects:error:")]
		[return: NullAllowed]
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

	[BaseType (typeof (NSObject))]
	interface NSIncrementalStoreNode {
		[Export ("initWithObjectID:withValues:version:")]
#if XAMCORE_4_0
		IntPtr Constructor (NSManagedObjectID objectID, NSDictionary<NSString, NSObject> values, ulong version);
#else 
		IntPtr Constructor (NSManagedObjectID objectId, NSDictionary values, ulong version);
#endif

		[Export ("updateWithValues:version:")]
#if XAMCORE_4_0
		void Update (NSDictionary<NSString, NSObject> values, ulong version);
#else
		void Update (NSDictionary values, ulong version);
#endif

		[Export ("objectID")]
		NSManagedObjectID ObjectId { get; }

		[Export ("version")]
		long Version { get; }

		[Export ("valueForPropertyDescription:")]
		[return: NullAllowed]
		NSObject ValueForPropertyDescription (NSPropertyDescription prop);
	}

	[BaseType (typeof (NSObject))]
	// 'init' issues a warning: CoreData: error: Failed to call designated initializer on NSManagedObject class 'NSManagedObject' 
	// then crash while disposing the instance
	[DisableDefaultCtor]
	interface NSManagedObject : NSFetchRequestResult {
		[DesignatedInitializer]
		[Export ("initWithEntity:insertIntoManagedObjectContext:")]
		IntPtr Constructor (NSEntityDescription entity, [NullAllowed] NSManagedObjectContext context);

		[Watch (3,0), TV (10,0), iOS (10,0), Mac (10,12)]
		[Export ("initWithContext:")]
		IntPtr Constructor (NSManagedObjectContext moc);

		[Watch (3, 0), TV (10, 0), iOS (10, 0), Mac (10,12)]
		[Static]
		[Export ("entity")]
		NSEntityDescription GetEntityDescription ();

		[Watch (3, 0), TV (10, 0), iOS (10, 0), Mac (10,12)]
		[Static]
		[Export ("fetchRequest")]
		NSFetchRequest CreateFetchRequest ();

		[NullAllowed, Export ("managedObjectContext")]
		NSManagedObjectContext ManagedObjectContext { get; }

		[Export ("entity", ArgumentSemantic.Strong)]
		NSEntityDescription Entity { get; }

		[Export ("objectID", ArgumentSemantic.Strong)]
		NSManagedObjectID ObjectID { get; }
		
		[Static, Export ("contextShouldIgnoreUnmodeledPropertyChanges")]
		bool ContextShouldIgnoreUnModeledPropertyChanges { get; }

		[Export ("inserted")]
		bool IsInserted { [Bind ("isInserted")] get; }

		[Export ("updated")]
		bool IsUpdated { [Bind ("isUpdated")] get; }

		[Export ("deleted")]
		bool IsDeleted { [Bind ("isDeleted")] get; }

		[Export ("fault")]
		bool IsFault { [Bind ("isFault")] get; }

		[Export ("faultingState")]
		nuint FaultingState { get; }

		[Export ("hasFaultForRelationshipNamed:")]
		bool HasFaultForRelationshipNamed (string key);

		[Export ("willAccessValueForKey:")]
		void WillAccessValueForKey ([NullAllowed] string key);

		[Export ("didAccessValueForKey:")]
		void DidAccessValueForKey ([NullAllowed] string key);

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

		[Export ("awakeFromSnapshotEvents:")]
		void AwakeFromSnapshotEvents (NSSnapshotEventType flags);

		[Export ("willSave")]
		void WillSave ();

		[Export ("didSave")]
		void DidSave ();

		[Export ("willTurnIntoFault")]
		void WillTurnIntoFault ();

		[Export ("didTurnIntoFault")]
		void DidTurnIntoFault ();

		[Export ("valueForKey:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		NSObject GetValue (string key);
#else
		IntPtr ValueForKey (string key);
#endif

		[Export ("setValue:forKey:")]
#if XAMCORE_4_0
		void SetValue ([NullAllowed] NSObject value, string key);
#else
		void SetValue (IntPtr value, string key);
#endif

		[Export ("primitiveValueForKey:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		NSObject GetPrimitiveValue (string key);
#else
		IntPtr PrimitiveValueForKey (string key);
#endif

		[Export ("setPrimitiveValue:forKey:")]
#if XAMCORE_4_0
		void SetPrimitiveValue ([NullAllowed] NSObject value, string key);
#else
		void SetPrimitiveValue (IntPtr value, string key);
#endif

		[Export ("committedValuesForKeys:")]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> GetCommittedValues ([NullAllowed] string[] keys);
#else
		NSDictionary CommittedValuesForKeys ([NullAllowed] string[] keys);
#endif

		[Export ("changedValues")]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> ChangedValues { get; }
#else
		NSDictionary ChangedValues { get; }
#endif

		[Export ("validateValue:forKey:error:")]
		bool ValidateValue (ref NSObject value, string key, out NSError error);

		[Export ("validateForDelete:")]
		bool ValidateForDelete (out NSError error);

		[Export ("validateForInsert:")]
		bool ValidateForInsert (out NSError error);

		[Export ("validateForUpdate:")]
		bool ValidateForUpdate (out NSError error);

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

	[Watch (3,0), TV (10,0), iOS (10,0), Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface NSQueryGenerationToken : NSSecureCoding, NSCopying
	{
		[Static, Export ("currentQueryGenerationToken", ArgumentSemantic.Strong)]
		NSQueryGenerationToken CurrentToken { get; }
	}
	
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSManagedObjectContext : NSCoding
#if !WATCH && !TVOS
	, NSLocking
#endif
#if MONOMAC
	, NSEditor, NSEditorRegistration
#endif
	{
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSManagedObjectContext (NSManagedObjectContextConcurrencyType)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSManagedObjectContext (NSManagedObjectContextConcurrencyType)' instead.")]
		[Export ("init")]
		IntPtr Constructor ();
		
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
		[return: NullAllowed]
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
#if XAMCORE_4_0
		void ObserveValue (string keyPath, [NullAllowed] NSObject object1, [NullAllowed] NSDictionary<NSString, NSObject> change, IntPtr context);
#else
		void ObserveValueForKeyPath (string keyPath, IntPtr object1, [NullAllowed] NSDictionary change, IntPtr context);
#endif

		[Export ("processPendingChanges")]
		void ProcessPendingChanges ();

		[Export ("assignObject:toPersistentStore:")]
#if XAMCORE_4_0
		void AssignObject (NSObject object1, NSPersistentStore store);
#else
		void AssignObject (IntPtr object1, NSPersistentStore store);
#endif

		[Export ("insertedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		NSSet<NSManagedObject> InsertedObjects { get; }
#else
		NSSet InsertedObjects { get; }
#endif

		[Export ("updatedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		NSSet<NSManagedObject> UpdatedObjects { get; }
#else
		NSSet UpdatedObjects { get; }
#endif

		[Export ("deletedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		NSSet<NSManagedObject> DeletedObjects { get; }
#else
		NSSet DeletedObjects { get; }
#endif

		[Export ("registeredObjects", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		NSSet<NSManagedObject> RegisteredObjects { get; }
#else
		NSSet RegisteredObjects { get; }
#endif

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
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use a queue style context and 'PerformAndWait' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use a queue style context and 'PerformAndWait' instead.")]
		[Export ("lock")]
		new void Lock ();

		[Deprecated (PlatformName.iOS, 8, 0, message : "Use a queue style context and 'PerformAndWait' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use a queue style context and 'PerformAndWait' instead.")]
		[Export ("unlock")]
		new void Unlock ();

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use a queue style context and 'Perform' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use a queue style context and 'Perform' instead.")]
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
		[Export ("initWithConcurrencyType:")]
		IntPtr Constructor (NSManagedObjectContextConcurrencyType ct);

		[Export ("performBlock:")]
		void Perform (/* non null */ Action action);

		[Export ("performBlockAndWait:")]
		void PerformAndWait (/* non null */ Action action);

		[Export ("userInfo", ArgumentSemantic.Strong)]
		NSMutableDictionary UserInfo { get; }

		[Export ("concurrencyType")]
		NSManagedObjectContextConcurrencyType ConcurrencyType { get; }

		//Detected properties
		// default is null, but setting it to null again would crash the app
		[NullAllowed, Export ("parentContext", ArgumentSemantic.Retain)]
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

		[Watch (3, 0), TV (10, 0), iOS (10, 0), Mac (10,12)]
		[NullAllowed, Export ("queryGenerationToken", ArgumentSemantic.Strong)]
		NSQueryGenerationToken QueryGenerationToken { get; }

		[Watch (3,0), TV (10,0), iOS (10,0), Mac (10,12)]
		[Export ("setQueryGenerationFromToken:error:")]
		bool SetQueryGenerationFromToken ([NullAllowed] NSQueryGenerationToken generation, out NSError error);

		[Watch (3, 0), TV (10, 0), iOS (10, 0), Mac (10,12)]
		[Export ("automaticallyMergesChangesFromParent")]
		bool AutomaticallyMergesChangesFromParent { get; set; }

		[iOS (8,3), Mac (10,11)]
		[Export ("refreshAllObjects")]
		void RefreshAllObjects ();

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("transactionAuthor")]
		string TransactionAuthor { get; set; }
	}

	interface NSManagedObjectChangeEventArgs {
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
	interface NSManagedObjectID : NSCopying, NSFetchRequestResult {

		[Export ("entity", ArgumentSemantic.Strong)]
		NSEntityDescription Entity { get; }

		[NullAllowed, Export ("persistentStore", ArgumentSemantic.Weak)]
		NSPersistentStore PersistentStore { get; }

		[Export ("temporaryID")]
		bool IsTemporaryID { [Bind ("isTemporaryID")] get; }

		[Export ("URIRepresentation")]
		NSUrl URIRepresentation { get; }

	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface NSManagedObjectModel : NSCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Static, Export ("mergedModelFromBundles:")]
		[return: NullAllowed]
		NSManagedObjectModel MergedModelFromBundles ([NullAllowed] NSBundle[] bundles);

		[Static, Export ("modelByMergingModels:")]
		[return: NullAllowed]
		NSManagedObjectModel ModelByMergingModels ([NullAllowed] NSManagedObjectModel[] models);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl url);

		[Export ("entitiesByName", ArgumentSemantic.Copy)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSEntityDescription> EntitiesByName { get; }
#else
		NSDictionary EntitiesByName { get; }
#endif

		[Export ("entities", ArgumentSemantic.Retain)]
		NSEntityDescription[] Entities { get; set; }

		[Export ("configurations", ArgumentSemantic.Strong)]
		string[] Configurations { get; }

		[Export ("entitiesForConfiguration:")]
		[return: NullAllowed]
		string[] EntitiesForConfiguration ([NullAllowed] string configuration);

		[Export ("setEntities:forConfiguration:")]
		void SetEntities (NSEntityDescription[] entities, string configuration);

		[Export ("setFetchRequestTemplate:forName:")]
		void SetFetchRequestTemplate ([NullAllowed] NSFetchRequest fetchRequestTemplate, string name);

		[Export ("fetchRequestTemplateForName:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		NSFetchRequest GetFetchRequestTemplate (string name);
#else
		NSFetchRequest FetchRequestTemplateForName (string name);
#endif

		[Export ("fetchRequestFromTemplateWithName:substitutionVariables:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		NSFetchRequest GetFetchRequestFromTemplate (string name, NSDictionary<NSString, NSObject> variables); 
#else
		NSFetchRequest FetchRequestFromTemplateWithName (string name, NSDictionary variables);
#endif

		[NullAllowed] // by default this property is null
		[Export ("localizationDictionary", ArgumentSemantic.Retain)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSString> LocalizationDictionary { get; set; }
#else
		NSDictionary LocalizationDictionary { get; set; }
#endif

		[Static, Export ("mergedModelFromBundles:forStoreMetadata:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		NSManagedObjectModel GetMergedModel ([NullAllowed] NSBundle[] bundles, NSDictionary<NSString, NSObject> metadata);
#else
		NSManagedObjectModel MergedModelFromBundles ([NullAllowed] NSBundle[] bundles, NSDictionary metadata);
#endif

		[Static, Export ("modelByMergingModels:forStoreMetadata:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		NSManagedObjectModel GetModelByMerging (NSManagedObjectModel[] models, NSDictionary<NSString, NSObject> metadata);
#else
		NSManagedObjectModel ModelByMergingModels (NSManagedObjectModel[] models, NSDictionary metadata);
#endif

		[Export ("fetchRequestTemplatesByName", ArgumentSemantic.Copy)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSFetchRequest> FetchRequestTemplatesByName { get; }
#else
		NSDictionary FetchRequestTemplatesByName { get; }
#endif

		[Export ("versionIdentifiers", ArgumentSemantic.Copy)]
		NSSet VersionIdentifiers { get; set; }

		[Export ("isConfiguration:compatibleWithStoreMetadata:")]
#if XAMCORE_4_0
		bool IsConfiguration ([NullAllowed] string configuration, NSDictionary<NSString, NSObject> metadata);
#else
		bool IsConfiguration ([NullAllowed] string configuration, NSDictionary metadata);
#endif

		[Export ("entityVersionHashesByName", ArgumentSemantic.Copy)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSData> EntityVersionHashesByName { get; }
#else
		NSDictionary EntityVersionHashesByName { get; }
#endif
	}

	[BaseType (typeof (NSObject))]
	interface NSMappingModel {

		[Static, Export ("mappingModelFromBundles:forSourceModel:destinationModel:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		NSMappingModel FromBundles (NSBundle[] bundles, NSManagedObjectModel sourceModel, NSManagedObjectModel destinationModel);
#else
		NSMappingModel MappingModelFromBundles (NSBundle[] bundles, NSManagedObjectModel sourceModel, NSManagedObjectModel destinationModel);
#endif

		[Static, Export ("inferredMappingModelForSourceModel:destinationModel:error:")]
		[return: NullAllowed]
		NSMappingModel GetInferredMappingModel (NSManagedObjectModel source, NSManagedObjectModel destination, out NSError error);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl url);

		[Export ("entityMappings", ArgumentSemantic.Retain)]
		NSEntityMapping[] EntityMappings { get; set; }

		[Export ("entityMappingsByName", ArgumentSemantic.Copy)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSEntityMapping> EntityMappingsByName { get; }
#else
		NSDictionary EntityMappingsByName { get; }
#endif

	}

	[Mac (10, 7)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSMergeConflict {
		[Export ("sourceObject", ArgumentSemantic.Retain)]
		NSManagedObject SourceObject { get;  }

		[Export ("objectSnapshot", ArgumentSemantic.Retain)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> ObjectSnapshot { get; }
#else
		NSDictionary ObjectSnapshot { get;  }
#endif

		[Export ("cachedSnapshot", ArgumentSemantic.Retain)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> CachedSnapshot { get; }
#else
		NSDictionary CachedSnapshot { get;  }
#endif

		[Export ("persistedSnapshot", ArgumentSemantic.Retain)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> PersistedSnapshot { get; }
#else
		NSDictionary PersistedSnapshot { get;  }
#endif

		[Export ("newVersionNumber")]
		nuint NewVersionNumber { get;  }

		[Export ("oldVersionNumber")]
		nuint OldVersionNumber { get;  }

		[DesignatedInitializer]
		[Export ("initWithSource:newVersion:oldVersion:cachedSnapshot:persistedSnapshot:")]
#if XAMCORE_4_0
		IntPtr Constructor (NSManagedObject srcObject, nuint newvers, nuint oldvers, [NullAllowed] NSDictionary<NSString, NSObject> cachesnap, [NullAllowed] NSDictionary<NSString, NSObject> persnap);
#else
		IntPtr Constructor (NSManagedObject srcObject, nuint newvers, nuint oldvers, [NullAllowed] NSDictionary cachesnap, [NullAllowed] NSDictionary persnap);
#endif
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSMergePolicy {
		[Export ("mergeType")]
		NSMergePolicyType MergeType { get;  }

		[DesignatedInitializer]
		[Export ("initWithMergeType:")]
		IntPtr Constructor (NSMergePolicyType ty);

		[Export ("resolveConflicts:error:")]
#if XAMCORE_4_0
		bool ResolveConflicts (NSMergeConflict [] list, out NSError error);
#else
		bool ResolveConflictserror (NSMergeConflict [] list, out NSError error);
#endif

		[iOS (9,0), Mac (10,11)]
		[Export ("resolveOptimisticLockingVersionConflicts:error:")]
		bool ResolveOptimisticLockingVersionConflicts (NSMergeConflict[] list, out NSError error);

		[iOS (9,0), Mac (10,11)]
		[Export ("resolveConstraintConflicts:error:")]
		bool ResolveConstraintConflicts (NSConstraintConflict[] list, out NSError error);

		[Watch (3, 0), TV (10, 0), iOS (10, 0), Mac (10,12)]
		[Static, Export ("errorMergePolicy", ArgumentSemantic.Strong)]
		NSMergePolicy ErrorPolicy { get; }

		[Watch (3, 0), TV (10, 0), iOS (10, 0), Mac (10,12)]
		[Static, Export ("rollbackMergePolicy", ArgumentSemantic.Strong)]
		NSMergePolicy RollbackPolicy { get; }

		[Watch (3, 0), TV (10, 0), iOS (10, 0), Mac (10,12)]
		[Static, Export ("overwriteMergePolicy", ArgumentSemantic.Strong)]
		NSMergePolicy OverwritePolicy { get; }

		[Watch (3, 0), TV (10, 0), iOS (10, 0), Mac (10,12)]
		[Static, Export ("mergeByPropertyObjectTrumpMergePolicy", ArgumentSemantic.Strong)]
		NSMergePolicy MergeByPropertyObjectTrumpPolicy { get; }

		[Watch (3, 0), TV (10, 0), iOS (10, 0), Mac (10,12)]
		[Static, Export ("mergeByPropertyStoreTrumpMergePolicy", ArgumentSemantic.Strong)]
		NSMergePolicy MergeByPropertyStoreTrumpPolicy { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSMigrationManager {

		[Export ("initWithSourceModel:destinationModel:")]
		IntPtr Constructor (NSManagedObjectModel sourceModel, NSManagedObjectModel destinationModel);

		[Export ("migrateStoreFromURL:type:options:withMappingModel:toDestinationURL:destinationType:destinationOptions:error:")]
		bool MigrateStoreFromUrl (NSUrl sourceUrl, string sStoreType, [NullAllowed] NSDictionary sOptions, [NullAllowed] NSMappingModel mappings, NSUrl dUrl, string dStoreType, [NullAllowed] NSDictionary dOptions, out NSError error);

		[Export ("reset")]
		void Reset ();

		[Export ("mappingModel", ArgumentSemantic.Strong)]
		NSMappingModel MappingModel { get; }

		[Export ("sourceModel", ArgumentSemantic.Strong)]
		NSManagedObjectModel SourceModel { get; }

		[Export ("destinationModel", ArgumentSemantic.Strong)]
		NSManagedObjectModel DestinationModel { get; }

		[Export ("sourceContext", ArgumentSemantic.Strong)]
		NSManagedObjectContext SourceContext { get; }

		[Export ("destinationContext", ArgumentSemantic.Strong)]
		NSManagedObjectContext DestinationContext { get; }

		[Export ("sourceEntityForEntityMapping:")]
		[return: NullAllowed]
		NSEntityDescription SourceEntityForEntityMapping (NSEntityMapping mEntity);

		[Export ("destinationEntityForEntityMapping:")]
		[return: NullAllowed]
		NSEntityDescription DestinationEntityForEntityMapping (NSEntityMapping mEntity);

		[Export ("associateSourceInstance:withDestinationInstance:forEntityMapping:")]
		void AssociateSourceInstance (NSManagedObject sourceInstance, NSManagedObject destinationInstance, NSEntityMapping entityMapping);

		[Export ("destinationInstancesForEntityMappingNamed:sourceInstances:")]
		NSManagedObject[] DestinationInstancesForEntityMappingNamed (string mappingName, [NullAllowed] NSManagedObject[] sourceInstances);

		[Export ("sourceInstancesForEntityMappingNamed:destinationInstances:")]
		NSManagedObject[] SourceInstancesForEntityMappingNamed (string mappingName, [NullAllowed] NSManagedObject[] destinationInstances);

		[Export ("currentEntityMapping", ArgumentSemantic.Strong)]
		NSEntityMapping CurrentEntityMapping { get; }

		[Export ("migrationProgress")]
		float MigrationProgress { get; }  /* float, not CGFloat */

		[NullAllowed] // by default this property is null
		[Export ("userInfo", ArgumentSemantic.Retain)]
		NSDictionary UserInfo { get; set; }

		[Export ("cancelMigrationWithError:")]
		void CancelMigrationWithError (NSError error);

		// 5.0
		[Export ("usesStoreSpecificMigrationManager")]
		bool UsesStoreSpecificMigrationManager { get; set; }
	}

	[Abstract]
	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface NSPersistentHistoryChange : NSCopying
	{
		[Export ("changeID")]
		long ChangeId { get; }

		[Export ("changedObjectID", ArgumentSemantic.Copy)]
		NSManagedObjectID ChangedObjectId { get; }

		[Export ("changeType")]
		NSPersistentHistoryChangeType ChangeType { get; }

		[NullAllowed, Export ("tombstone", ArgumentSemantic.Copy)]
		NSDictionary Tombstone { get; }

		[NullAllowed, Export ("transaction", ArgumentSemantic.Strong)]
		NSPersistentHistoryTransaction Transaction { get; }

		[NullAllowed, Export ("updatedProperties", ArgumentSemantic.Copy)]
		NSSet<NSPropertyDescription> UpdatedProperties { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface NSPersistentHistoryToken : NSCopying //, NSSecureCoding TODO: The class does state that it supports the NSSecureCoding YET SupportsSecureCoding returns false, radar 32761925
	{
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSPersistentStoreRequest))]
	[DisableDefaultCtor]
	interface NSPersistentHistoryChangeRequest
	{
		[Static]
		[Export ("fetchHistoryAfterDate:")]
		NSPersistentHistoryChangeRequest FetchHistoryAfter (NSDate date);

		[Static]
		[Export ("fetchHistoryAfterToken:")]
		NSPersistentHistoryChangeRequest FetchHistoryAfter ([NullAllowed] NSPersistentHistoryToken token);

		[Static]
		[Export ("fetchHistoryAfterTransaction:")]
		NSPersistentHistoryChangeRequest FetchHistoryAfter ([NullAllowed] NSPersistentHistoryTransaction transaction);

		[Static]
		[Export ("deleteHistoryBeforeDate:")]
		NSPersistentHistoryChangeRequest DeleteHistoryBefore (NSDate date);

		[Static]
		[Export ("deleteHistoryBeforeToken:")]
		NSPersistentHistoryChangeRequest DeleteHistoryBefore ([NullAllowed] NSPersistentHistoryToken token);

		[Static]
		[Export ("deleteHistoryBeforeTransaction:")]
		NSPersistentHistoryChangeRequest DeleteHistoryBefore ([NullAllowed] NSPersistentHistoryTransaction transaction);

		[Export ("resultType", ArgumentSemantic.Assign)]
		NSPersistentHistoryResultType ResultType { get; set; }

		[NullAllowed, Export ("token", ArgumentSemantic.Strong)]
		NSPersistentHistoryToken Token { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSPersistentStoreResult))]
	interface NSPersistentHistoryResult
	{
		[NullAllowed]
		[Export ("result", ArgumentSemantic.Strong)]
		NSObject Result { get; }

		[Export ("resultType")]
		NSPersistentHistoryResultType ResultType { get; }
	}

	[Abstract]
	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface NSPersistentHistoryTransaction : NSCopying
	{
		[Export ("timestamp", ArgumentSemantic.Copy)]
		NSDate Timestamp { get; }

		[NullAllowed, Export ("changes", ArgumentSemantic.Copy)]
		NSPersistentHistoryChange[] Changes { get; }

		[Export ("transactionNumber")]
		long TransactionNumber { get; }

		[Export ("storeID")]
		string StoreId { get; }

		[Export ("bundleID")]
		string BundleId { get; }

		[Export ("processID")]
		string ProcessId { get; }

		[NullAllowed, Export ("contextName")]
		string ContextName { get; }

		[NullAllowed, Export ("author")]
		string Author { get; }

		[Export ("token", ArgumentSemantic.Strong)]
		NSPersistentHistoryToken Token { get; }

		[Export ("objectIDNotification")]
		NSNotification ObjectIdNotification { get; }
	}

#if !WATCH
	[NoWatch, NoTV, Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface NSCoreDataCoreSpotlightDelegate
	{
		[Export ("domainIdentifier")]
		string DomainIdentifier { get; }

		[NullAllowed, Export ("indexName")]
		string IndexName { get; }

		[Export ("initForStoreWithDescription:model:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSPersistentStoreDescription description, NSManagedObjectModel model);

		[Export ("attributeSetForObject:")]
		[return: NullAllowed]
		CSSearchableItemAttributeSet GetAttributeSet (NSManagedObject @object);

		[Export ("searchableIndex:reindexAllSearchableItemsWithAcknowledgementHandler:")]
		void ReindexAllSearchableItems (CSSearchableIndex searchableIndex, Action acknowledgementHandler);

		[Export ("searchableIndex:reindexSearchableItemsWithIdentifiers:acknowledgementHandler:")]
		void ReindexSearchableItems (CSSearchableIndex searchableIndex, string[] identifiers, Action acknowledgementHandler);
	}
#endif 

	// NSPersistentStore is an abstract type according to Apple's documentation, but Apple
	// also have internal subclasses of NSPersistentStore, and in those cases our closest
	// type is NSPersistentStore, which means we must be able to create managed wrappers
	// for such native classes using the managed NSPersistentStore. This means we can't
	// make our managed version [Abstract].
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPersistentStore {

		[Static]
		[Export ("migrationManagerClass")]
		Class MigrationManagerClass { get; }

		[Static, Export ("metadataForPersistentStoreWithURL:error:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> GetMetadataForPersistentStore (NSUrl url, out NSError error);
#else
		NSDictionary MetadataForPersistentStoreWithUrl (NSUrl url, out NSError error);
#endif

		[Static, Export ("setMetadata:forPersistentStoreWithURL:error:")]
#if XAMCORE_4_0
		bool SetMetadata ([NullAllowed] NSDictionary<NSString, NSObject> metadata, NSUrl url, out NSError error);
#else
		bool SetMetadata ([NullAllowed] NSDictionary metadata, NSUrl url, out NSError error);
#endif

#if XAMCORE_4_0
		[Protected]
#endif
		[DesignatedInitializer]
		[Export ("initWithPersistentStoreCoordinator:configurationName:URL:options:")]
		IntPtr Constructor ([NullAllowed] NSPersistentStoreCoordinator root, [NullAllowed] string name, NSUrl url, [NullAllowed] NSDictionary options);
		
		[Export ("loadMetadata:")]
		bool LoadMetadata (out NSError error);

		[NullAllowed, Export ("persistentStoreCoordinator", ArgumentSemantic.Weak)]
		NSPersistentStoreCoordinator PersistentStoreCoordinator { get; }

		[Export ("configurationName")]
		string ConfigurationName { get; }

		[NullAllowed, Export ("options", ArgumentSemantic.Strong)]
		NSDictionary Options { get; }

		[NullAllowed] // by default this property is null
		[Export ("URL", ArgumentSemantic.Retain)]
		NSUrl Url { get; set; }

		[Export ("identifier")]
		string Identifier { get; set; }

		[Export ("type")]
		string Type { get; }

		[Export ("readOnly")]
		bool ReadOnly { [Bind ("isReadOnly")] get; set; }

		[Export ("metadata", ArgumentSemantic.Retain)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> Metadata { get; set; }
#else
		NSDictionary Metadata { get; set; }
#endif

		[Export ("didAddToPersistentStoreCoordinator:")]
		void DidAddToPersistentStoreCoordinator (NSPersistentStoreCoordinator coordinator);

		[Export ("willRemoveFromPersistentStoreCoordinator:")]
		void WillRemoveFromPersistentStoreCoordinator ([NullAllowed] NSPersistentStoreCoordinator coordinator);

		[Field ("NSPersistentStoreSaveConflictsErrorKey")]
		NSString SaveConflictsErrorKey { get; }

#if !WATCH
		[NoWatch, NoTV, Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("coreSpotlightExporter")]
		NSCoreDataCoreSpotlightDelegate CoreSpotlightExporter { get; }
#endif

	}
	
	[Watch (3,0), TV (10,0), iOS (10,0), Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSPersistentStoreDescription : NSCopying
	{
		[Static]
		[Export ("persistentStoreDescriptionWithURL:")]
		NSPersistentStoreDescription GetPersistentStoreDescription (NSUrl Url);

		[Export ("type")]
		string Type { get; set; }

		[NullAllowed, Export ("configuration")]
		string Configuration { get; set; }

		[NullAllowed, Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[Export ("options", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Options { get; }

		[Export ("setOption:forKey:")]
		void SetOption ([NullAllowed] NSObject option, string key);

		[Export ("readOnly")]
		bool IsReadOnly { [Bind ("isReadOnly")] get; set; }

		[Export ("timeout")]
		double Timeout { get; set; }

		[Export ("sqlitePragmas", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> SqlitePragmas { get; }

		[Export ("setValue:forPragmaNamed:")]
		void SetValue ([NullAllowed] NSObject value, string name);

		[Export ("shouldAddStoreAsynchronously")]
		bool ShouldAddStoreAsynchronously { get; set; }

		[Export ("shouldMigrateStoreAutomatically")]
		bool ShouldMigrateStoreAutomatically { get; set; }

		[Export ("shouldInferMappingModelAutomatically")]
		bool ShouldInferMappingModelAutomatically { get; set; }

		[Export ("initWithURL:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl url);
	}
	
	[Watch (3,0), TV (10,0), iOS (10,0), Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSPersistentContainer
	{
		[Static]
		[Export ("persistentContainerWithName:")]
		NSPersistentContainer GetPersistentContainer (string name);

		[Static]
		[Export ("persistentContainerWithName:managedObjectModel:")]
		NSPersistentContainer GetPersistentContainer (string name, NSManagedObjectModel model);

		[Static]
		[Export ("defaultDirectoryURL")]
		NSUrl DefaultDirectoryUrl { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("viewContext", ArgumentSemantic.Strong)]
		NSManagedObjectContext ViewContext { get; }

		[Export ("managedObjectModel", ArgumentSemantic.Strong)]
		NSManagedObjectModel ManagedObjectModel { get; }

		[Export ("persistentStoreCoordinator", ArgumentSemantic.Strong)]
		NSPersistentStoreCoordinator PersistentStoreCoordinator { get; }

		[Export ("persistentStoreDescriptions", ArgumentSemantic.Copy)]
		NSPersistentStoreDescription[] PersistentStoreDescriptions { get; set; }

		[Export ("initWithName:")]
		IntPtr Constructor (string name);

		[Export ("initWithName:managedObjectModel:")]
		[DesignatedInitializer]
		IntPtr Constructor (string name, NSManagedObjectModel model);

		[Export ("loadPersistentStoresWithCompletionHandler:")]
		[Async]
		void LoadPersistentStores (Action<NSPersistentStoreDescription, NSError> block);

		[Export ("newBackgroundContext")]
		NSManagedObjectContext NewBackgroundContext { get; }

		[Export ("performBackgroundTask:")]
		void Perform (Action<NSManagedObjectContext> block);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // iOS8 -> Core Data: warning: client failed to call designated initializer on NSPersistentStoreCoordinator
	partial interface NSPersistentStoreCoordinator
#if !WATCH && !TVOS
		: NSLocking
#endif // !WATCH
	{

		[Static, Export ("registeredStoreTypes", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSValue> RegisteredStoreTypes { get; }
#else
		NSDictionary RegisteredStoreTypes { get; }
#endif

		[Static, Export ("registerStoreClass:forStoreType:")]
		void RegisterStoreClass (Class storeClass, NSString storeType);

		[Deprecated (PlatformName.iOS, 9, 0, message : "Use the method that takes an out NSError parameter.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use the method that takes an out NSError parameter.")]
		[Static, Export ("metadataForPersistentStoreOfType:URL:error:")]
		[return: NullAllowed]
		NSDictionary MetadataForPersistentStoreOfType (NSString storeType, NSUrl url, out NSError error);
		
		[iOS (7,0)]
		[Mac (10, 9)]
		[Static, Export ("metadataForPersistentStoreOfType:URL:options:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> GetMetadata (string storeType, NSUrl url, [NullAllowed] NSDictionary options, out NSError error);

		[Deprecated (PlatformName.iOS, 9, 0, message:"Use the method that takes an out NSError parameter.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message:"Use the method that takes an out NSError parameter.")]
		[Static, Export ("setMetadata:forPersistentStoreOfType:URL:error:")]
		bool SetMetadata (NSDictionary metadata, NSString storeType, NSUrl url, out NSError error);
		
		[iOS (7,0)]
		[Mac (10,9)]
		[Static, Export ("setMetadata:forPersistentStoreOfType:URL:options:error:")]
		bool SetMetadata ([NullAllowed] NSDictionary<NSString, NSObject> metadata, string storeType, NSUrl url, [NullAllowed] NSDictionary options, out NSError error);

		[Export ("setMetadata:forPersistentStore:")]
#if XAMCORE_4_0
		void SetMetadata ([NullAllowed] NSDictionary<NSString, NSObject> metadata, NSPersistentStore store);
#else
		void SetMetadata ([NullAllowed] NSDictionary metadata, NSPersistentStore store);
#endif

		[Export ("metadataForPersistentStore:")]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> GetMetadata (NSPersistentStore store);
#else
		NSDictionary MetadataForPersistentStore (NSPersistentStore store);
#endif

		[DesignatedInitializer]
		[Export ("initWithManagedObjectModel:")]
		IntPtr Constructor (NSManagedObjectModel model);

		[Export ("managedObjectModel", ArgumentSemantic.Strong)]
		NSManagedObjectModel ManagedObjectModel { get; }

		[Export ("persistentStores", ArgumentSemantic.Strong)]
		NSPersistentStore[] PersistentStores { get; }

		[Export ("persistentStoreForURL:")]
		NSPersistentStore PersistentStoreForUrl (NSUrl url);

		[Export ("URLForPersistentStore:")]
		NSUrl UrlForPersistentStore (NSPersistentStore store);

		[Export ("setURL:forPersistentStore:")]
		bool SetUrl (NSUrl url, NSPersistentStore store);

		[Export ("addPersistentStoreWithType:configuration:URL:options:error:")]
#if XAMCORE_4_0
		NSPersistentStore AddPersistentStore (NSString storeType, [NullAllowed] string configuration, [NullAllowed] NSUrl storeUrl, [NullAllowed] NSDictionary options, out NSError error);
#else
		NSPersistentStore AddPersistentStoreWithType (NSString storeType, [NullAllowed] string configuration, [NullAllowed] NSUrl storeUrl, [NullAllowed] NSDictionary options, out NSError error);
#endif
		
		[Watch (3,0), TV (10,0), iOS (10,0), Mac (10,12)]
		[Export ("addPersistentStoreWithDescription:completionHandler:")]
		[Async]
		void AddPersistentStore (NSPersistentStoreDescription storeDescription, Action<NSPersistentStoreDescription, NSError> block);

		[Export ("removePersistentStore:error:")]
		bool RemovePersistentStore (NSPersistentStore store, out NSError error);

		[Export ("migratePersistentStore:toURL:options:withType:error:")]
		[return: NullAllowed]
		NSPersistentStore MigratePersistentStore (NSPersistentStore store, NSUrl url, [NullAllowed] NSDictionary options, NSString storeType, out NSError error);

		[Export ("managedObjectIDForURIRepresentation:")]
		[return: NullAllowed]
		NSManagedObjectID ManagedObjectIDForURIRepresentation (NSUrl url);

#if !WATCH && !TVOS
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'PerformAndWait' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'PerformAndWait' instead.")]
		[Export ("lock")]
		new void Lock ();

		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'PerformAndWait' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'PerformAndWait' instead.")]
		[Export ("unlock")]
		new void Unlock ();

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'Perform' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'Perform' instead.")]
		[Export ("tryLock")]
		bool TryLock { get; }
#endif // !WATCH && !TVOS

#if MONOMAC
		[Availability (Deprecated = Platform.Mac_10_5)]
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

		[Watch (4,0)][TV (11,0)][Mac (10,13)][iOS (11,0)]
		[Field ("NSBinaryStoreSecureDecodingClasses")]
		NSString BinaryStoreSecureDecodingClasses { get; }

		[Watch (4,0)][TV (11,0)][Mac (10,13)][iOS (11,0)]
		[Field ("NSBinaryStoreInsecureDecodingCompatibilityOption")]
		NSString BinaryStoreInsecureDecodingCompatibilityOption { get; }

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
		[Export ("executeRequest:withContext:error:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		NSObject Execute (NSPersistentStoreRequest request, NSManagedObjectContext context, out NSError error);
#else
		NSObject ExecuteRequestwithContexterror (NSPersistentStoreRequest request, NSManagedObjectContext context, out NSError error);
#endif

		[NoWatch][NoTV]
		[Notification]
		[Availability (Deprecated = Platform.iOS_10_0, Message = "Please see the release notes and Core Data documentation.")]
		[Field ("NSPersistentStoreDidImportUbiquitousContentChangesNotification")]
		NSString DidImportUbiquitousContentChangesNotification { get; }

		[NoWatch][NoTV]
		[Availability (Deprecated = Platform.iOS_10_0, Message = "Please see the release notes and Core Data documentation.")]
		[Field ("NSPersistentStoreUbiquitousContentNameKey")]
		NSString PersistentStoreUbiquitousContentNameKey { get; }

		[NoWatch][NoTV]
		[Availability (Deprecated = Platform.iOS_10_0, Message = "Please see the release notes and Core Data documentation.")]
		[Field ("NSPersistentStoreUbiquitousContentURLKey")]
#if XAMCORE_4_0
		NSString PersistentStoreUbiquitousContentUrlKey { get; }
#else
		NSString PersistentStoreUbiquitousContentUrlLKey { get; }
#endif

#if !MONOMAC
		[Field ("NSPersistentStoreFileProtectionKey")]
		NSString PersistentStoreFileProtectionKey { get; }
#endif

		// 7.0

		[NoWatch][NoTV]
		[iOS (7,0), Mac (10, 9)]
		[Field ("NSPersistentStoreUbiquitousPeerTokenOption")]
		NSString PersistentStoreUbiquitousPeerTokenOption { get; }

		[NoWatch][NoTV]
		[iOS (7,0), Mac (10, 9)]
		[Static]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Please see the release notes and Core Data documentation.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Please see the release notes and Core Data documentation.")]
		[Export ("removeUbiquitousContentAndPersistentStoreAtURL:options:error:")]
		bool RemoveUbiquitousContentAndPersistentStore (NSUrl storeUrl, NSDictionary options, out NSError error);

		[iOS (7,0), Mac (10, 9)]
		[Notification (typeof (NSPersistentStoreCoordinatorStoreChangeEventArgs))]
		[Field ("NSPersistentStoreCoordinatorStoresWillChangeNotification")]
		NSString StoresWillChangeNotification { get; }

		[NoWatch][NoTV]
		[iOS (7,0), Mac (10, 9)]
		[Field ("NSPersistentStoreRebuildFromUbiquitousContentOption")]
		NSString RebuildFromUbiquitousContentOption { get; }

		[NoWatch][NoTV]
		[iOS (7,0), Mac (10, 9)]
		[Field ("NSPersistentStoreRemoveUbiquitousMetadataOption")]
		NSString RemoveUbiquitousMetadataOption { get; }

		[NoWatch][NoTV]
		[iOS (7,0), Mac (10, 9)]
		[Field ("NSPersistentStoreUbiquitousContainerIdentifierKey")]
		[Obsolete ("Use 'UbiquitousContainerIdentifierKey' instead.")]
		NSString eUbiquitousContainerIdentifierKey { get; }

		[NoWatch][NoTV]
		[iOS (7,0), Mac (10, 9)]
		[Field ("NSPersistentStoreUbiquitousContainerIdentifierKey")]
		NSString UbiquitousContainerIdentifierKey { get; }

		// 11.0

		[NoWatch, NoTV, Mac (10, 13), iOS (11, 0)]
		[Field ("NSCoreDataCoreSpotlightExporter")]
		NSString CoreSpotlightExporter { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Field ("NSPersistentHistoryTrackingKey")]
		NSString HistoryTrackingKey { get; }

		[iOS (8,0), Mac (10,10)]
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		[iOS (8,0), Mac (10,10)]
		[Export ("performBlock:")]
		void Perform (Action code);

		[iOS (8,0), Mac (10,10)]
		[Export ("performBlockAndWait:")]
		void PerformAndWait (Action code);

		[iOS (9,0), Mac (10,11)]
		[Export ("destroyPersistentStoreAtURL:withType:options:error:")]
		bool DestroyPersistentStore (NSUrl url, string storeType, [NullAllowed] NSDictionary options, out NSError error);

		[iOS (9,0), Mac (10,11)]
		[Export ("replacePersistentStoreAtURL:destinationOptions:withPersistentStoreFromURL:sourceOptions:storeType:error:")]
		bool ReplacePersistentStore (NSUrl destinationUrl, [NullAllowed] NSDictionary destinationOptions, NSUrl sourceUrl, [NullAllowed] NSDictionary sourceOptions, string storeType, out NSError error);
	}

	interface NSPersistentStoreCoordinatorStoreChangeEventArgs {
		[NoWatch][NoTV]
		[Export ("NSPersistentStoreUbiquitousTransitionTypeKey")]
		[Availability (Introduced = Platform.iOS_7_0, Deprecated = Platform.iOS_10_0, Message = "Please see the release notes and Core Data documentation.")]
		NSPersistentStoreUbiquitousTransitionType EventType { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSPersistentStoreRequest : NSCopying {
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

		[Export ("finalResult", ArgumentSemantic.Retain)]
#if XAMCORE_4_0
		INSFetchRequestResult[] FinalResult { get; }
#else
		NSObject [] FinalResult { get; }
#endif
	}

	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface NSPersistentStoreResult {

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
		IntPtr Constructor (NSFetchRequest request, [NullAllowed] Action<NSAsynchronousFetchResult> completion);

		[Export ("fetchRequest", ArgumentSemantic.Retain)]
		NSFetchRequest FetchRequest { get; }

		[Export ("estimatedResultCount")]
		nint EstimatedResultCount { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSPropertyDescription : NSCoding, NSCopying {

		[Export ("entity")]
		NSEntityDescription Entity { get; }

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[Export ("optional")]
		bool Optional { [Bind ("isOptional")] get; set; }

		[Export ("transient")]
		bool Transient { [Bind ("isTransient")] get; set; }

		[Export ("validationPredicates")]
		NSPredicate[] ValidationPredicates { get; }

		[Export ("validationWarnings")]
		string[] ValidationWarnings { get; }

		[Export ("setValidationPredicates:withValidationWarnings:")]
		void SetValidationPredicates ([NullAllowed] NSPredicate[] validationPredicates, [NullAllowed] string[] validationWarnings);

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Retain)]
		NSDictionary UserInfo { get; set; }

		[Export ("indexed")]
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message : "Use 'NSEntityDescription.Indexes' instead.")]
		bool Indexed { [Bind ("isIndexed")] get; set; }

		[Export ("versionHash")]
		NSData VersionHash { get; }

		[NullAllowed] // by default this property is null
		[Export ("versionHashModifier")]
		string VersionHashModifier { get; set; }

		[Export ("renamingIdentifier")]
		string RenamingIdentifier { get; set; }

		// 5.0
		[Export ("indexedBySpotlight")]
		bool IndexedBySpotlight { [Bind ("isIndexedBySpotlight")]get; set; }

		[Export ("storedInExternalRecord")]
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'CoreSpotlight' integration instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'CoreSpotlight' integration instead.")]
		bool StoredInExternalRecord { [Bind ("isStoredInExternalRecord")]get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSPropertyMapping {

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
	interface NSRelationshipDescription {

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

		[Export ("toMany")]
		bool IsToMany { [Bind ("isToMany")] get; }

		[Export ("versionHash")]
		NSData VersionHash { get; }

		// 5.0
		[Export ("ordered")]
		bool Ordered { [Bind ("isOrdered")]get; set; }
	}

	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSSaveChangesRequest {
		[Export ("initWithInsertedObjects:updatedObjects:deletedObjects:lockedObjects:")]
#if XAMCORE_4_0
		IntPtr Constructor ([NullAllowed] NSSet<NSManagedObject> insertedObjects, [NullAllowed] NSSet<NSManagedObject> updatedObjects, [NullAllowed] NSSet<NSManagedObject> deletedObjects, [NullAllowed] NSSet<NSManagedObject> lockedObjects);
#else
		IntPtr Constructor ([NullAllowed] NSSet insertedObjects, [NullAllowed] NSSet updatedObjects, [NullAllowed] NSSet deletedObjects, [NullAllowed] NSSet lockedObjects);
#endif

		[NullAllowed, Export ("insertedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		NSSet<NSManagedObject> InsertedObjects { get; }
#else
		NSSet InsertedObjects { get; }
#endif

		[NullAllowed, Export ("updatedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		NSSet<NSManagedObject> UpdatedObjects { get; }
#else
		NSSet UpdatedObjects { get; }
#endif

		[NullAllowed, Export ("deletedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		NSSet<NSManagedObject> DeletedObjects { get; }
#else
		NSSet DeletedObjects { get; }
#endif

		[NullAllowed, Export ("lockedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		NSSet<NSManagedObject> LockedObjects { get; }
#else
		NSSet LockedObjects { get; }
#endif
	}

	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSBatchUpdateRequest {
		[Export ("initWithEntityName:")]
		[DesignatedInitializer]
		IntPtr Constructor (string entityName);

		[Export ("initWithEntity:")]
		[DesignatedInitializer]
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

		[Export ("resultType", ArgumentSemantic.Assign)]
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
