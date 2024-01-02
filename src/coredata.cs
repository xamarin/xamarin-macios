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
using CloudKit;
#if MONOMAC
using AppKit;
#endif
#if !WATCH
using CoreSpotlight;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreData {
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

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSPersistentStoreStagedMigrationManagerOptionKey")]
		NSString StagedMigrationManagerOptionKey { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0), MacCatalyst (17, 0)]
		[Field ("NSPersistentStoreDeferredLightweightMigrationOptionKey")]
		NSString DeferredLightweightMigrationOptionKey { get; }
	}

	[NoWatch]
	[NoTV]
	[Native] // NUInteger -> NSPersistentStoreCoordinator.h
	[Deprecated (PlatformName.iOS, 10, 0, message: "Please see the release notes and Core Data documentation.")]
	[Deprecated (PlatformName.MacOSX, 10, 12, message: "Please see the release notes and Core Data documentation.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please see the release notes and Core Data documentation.")]
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
		NativeHandle Constructor ([NullAllowed] NSPersistentStoreCoordinator coordinator, [NullAllowed] string configurationName, NSUrl url, [NullAllowed] NSDictionary options);

		[Export ("load:")]
		bool Load (out NSError error);

		[Export ("save:")]
		bool Save (out NSError error);

		[Export ("newCacheNodeForManagedObject:")]
		NSAtomicStoreCacheNode NewCacheNodeForManagedObject (NSManagedObject managedObject);

		[Export ("updateCacheNode:fromManagedObject:")]
		void UpdateCacheNode (NSAtomicStoreCacheNode node, NSManagedObject managedObject);

		[Export ("cacheNodes")]
#if XAMCORE_5_0
		NSSet<NSAtomicStoreCacheNode> CacheNodes { get; }
#else
		NSSet CacheNodes { get; }
#endif

		[Export ("addCacheNodes:")]

#if XAMCORE_5_0
		void AddCacheNodes (NSSet<NSAtomicStoreCacheNode> cacheNodes);
#else
		void AddCacheNodes (NSSet cacheNodes);
#endif

		[Export ("willRemoveCacheNodes:")]
#if XAMCORE_5_0
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSFetchIndexElementDescription : NSCoding, NSCopying {
		[Export ("initWithProperty:collationType:")]
		NativeHandle Constructor (NSPropertyDescription property, NSFetchIndexElementType collationType);

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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSFetchIndexDescription : NSCoding, NSCopying {
		[Export ("initWithName:elements:")]
		NativeHandle Constructor (string name, [NullAllowed] NSFetchIndexElementDescription [] elements);

		[Export ("name")]
		string Name { get; set; }

		[Export ("elements", ArgumentSemantic.Copy)]
		NSFetchIndexElementDescription [] Elements { get; set; }

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
		NativeHandle Constructor (NSManagedObjectID moid);

		[Export ("objectID", ArgumentSemantic.Strong)]
		NSManagedObjectID ObjectID { get; }

		[NullAllowed, Export ("propertyCache", ArgumentSemantic.Retain)]
#if XAMCORE_5_0
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
		[NullAllowed]
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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("preservesValueInHistoryOnDeletion")]
		bool PreservesValueInHistoryOnDeletion { get; set; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("allowsCloudEncryption")]
		bool AllowsCloudEncryption { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSEntityDescription : NSCoding, NSCopying {

		[Static, Export ("entityForName:inManagedObjectContext:")]
		[return: NullAllowed]
		NSEntityDescription EntityForName (string entityName, NSManagedObjectContext context);

		[Static, Export ("insertNewObjectForEntityForName:inManagedObjectContext:")]
#if !NET
		NSObject InsertNewObjectForEntityForName (string entityName, NSManagedObjectContext context);
#else
		NSManagedObject InsertNewObject (string entityName, NSManagedObjectContext context);
#endif

		[Export ("managedObjectModel")]
		NSManagedObjectModel ManagedObjectModel { get; }

		[Export ("managedObjectClassName")]
		[NullAllowed]
		string ManagedObjectClassName { get; set; }

		[NullAllowed, Export ("renamingIdentifier")]
		string RenamingIdentifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[Export ("abstract")]
		bool Abstract { [Bind ("isAbstract")] get; set; }

		[Export ("subentitiesByName")]
#if XAMCORE_5_0
		NSDictionary<NSString, NSEntityDescription> SubentitiesByName { get; }
#else
		NSDictionary SubentitiesByName { get; }
#endif

		[Export ("subentities", ArgumentSemantic.Retain)]
		NSEntityDescription [] Subentities { get; set; }

		[NullAllowed, Export ("superentity")]
		NSEntityDescription Superentity { get; }

		[Export ("propertiesByName")]
#if XAMCORE_5_0
		NSDictionary<NSString, NSPropertyDescription> PropertiesByName { get; }
#else
		NSDictionary PropertiesByName { get; }
#endif

		[Export ("properties", ArgumentSemantic.Retain)]
		NSPropertyDescription [] Properties { get; set; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Retain)]
		NSDictionary UserInfo { get; set; }

		[Export ("attributesByName")]
#if XAMCORE_5_0
		NSDictionary<NSString, NSAttributeDescription> AttributesByName { get; }
#else
		NSDictionary AttributesByName { get; }
#endif

		[Export ("relationshipsByName")]
#if XAMCORE_5_0
		NSDictionary<NSString, NSRelationshipDescription> RelationshipsByName { get; }
#else
		NSDictionary RelationshipsByName { get; }
#endif

		[Export ("relationshipsWithDestinationEntity:")]
		NSRelationshipDescription [] RelationshipsWithDestinationEntity (NSEntityDescription entity);

		[Export ("isKindOfEntity:")]
		bool IsKindOfEntity (NSEntityDescription entity);

		[Export ("versionHash")]
		NSData VersionHash { get; }

		[NullAllowed] // by default this property is null
		[Export ("versionHashModifier")]
		string VersionHashModifier { get; set; }

		[Export ("compoundIndexes", ArgumentSemantic.Retain)]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSEntityDescription.Indexes' instead.")]
		NSPropertyDescription [] CompoundIndexes { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("indexes", ArgumentSemantic.Copy)]
		NSFetchIndexDescription [] Indexes { get; set; }

		// @property (strong) NSArray<NSArray<id __nonnull> * __nonnull> * __nonnull uniquenessConstraints __attribute__((availability(ios, introduced=9.0)));
		[MacCatalyst (13, 1)]
		[Internal, Export ("uniquenessConstraints", ArgumentSemantic.Strong)]
		NSArray _UniquenessConstraints { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("coreSpotlightDisplayNameExpression", ArgumentSemantic.Retain)]
		NSExpression CoreSpotlightDisplayNameExpression { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSEntityMapping {

		[Export ("name")]
		[NullAllowed]
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
		[NullAllowed]
		NSPropertyMapping [] AttributeMappings { get; set; }

		[Export ("relationshipMappings", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSPropertyMapping [] RelationshipMappings { get; set; }

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
		NativeHandle Constructor (NSExpressionType type);

		[Static, Export ("expressionForFetch:context:countOnly:")]
		NSFetchRequestExpression FromFetch (NSExpression fetch, NSExpression context, bool countOnly);

		[Export ("requestExpression")]
		NSExpression Request { get; }

		[Export ("contextExpression")]
		NSExpression Context { get; }

		[Export ("countOnlyRequest")]
		bool IsCountOnly { [Bind ("isCountOnlyRequest")] get; }
	}

	interface INSFetchRequestResult { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface NSFetchRequestResult { }

	[DisableDefaultCtor] // designated
	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSFetchRequest : NSCoding {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("entity", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSEntityDescription Entity { get; set; }

		[Export ("predicate", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSPredicate Predicate { get; set; }

		[Export ("sortDescriptors", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSSortDescriptor [] SortDescriptors { get; set; }

		[Export ("fetchLimit")]
		nuint FetchLimit { get; set; }

		[Export ("fetchOffset")]
		nuint FetchOffset { get; set; }

		[Export ("affectedStores", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSPersistentStore [] AffectedStores { get; set; }

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
		string [] RelationshipKeyPathsForPrefetching { get; set; }

		[Export ("propertiesToFetch", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSPropertyDescription [] PropertiesToFetch { get; set; }

		[Static]
		[Export ("fetchRequestWithEntityName:")]
		// note: Xcode 6.3 changed the return value type from `NSFetchRequest*` to `instancetype`
		NSFetchRequest FromEntityName (string entityName);

		[Export ("initWithEntityName:")]
		NativeHandle Constructor (string entityName);

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

		[MacCatalyst (13, 1)]
		[Export ("execute:")]
		[return: NullAllowed]
		INSFetchRequestResult [] Execute (out NSError error);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" })]
	interface NSFetchedResultsController {

		[Export ("initWithFetchRequest:managedObjectContext:sectionNameKeyPath:cacheName:")]
		NativeHandle Constructor (NSFetchRequest fetchRequest, NSManagedObjectContext context, [NullAllowed] string sectionNameKeyPath, [NullAllowed] string name);

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSFetchedResultsControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("cacheName")]
		string CacheName { get; }

		[Export ("fetchedObjects")]
		[NullAllowed]
		NSObject [] FetchedObjects { get; }

		[Export ("fetchRequest")]
		NSFetchRequest FetchRequest { get; }

		[Export ("managedObjectContext")]
		NSManagedObjectContext ManagedObjectContext { get; }

		[NullAllowed, Export ("sectionNameKeyPath")]
		string SectionNameKeyPath { get; }

		[NullAllowed, Export ("sections")]
		INSFetchedResultsSectionInfo [] Sections { get; }

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

#if !NET
		// badly named and conflict with the property
		[Export ("sectionIndexTitleForSectionName:")]
		[return: NullAllowed]
		string SectionIndexTitles (string sectionName);

		// expose a method as the property name is taken
		[Export ("sectionIndexTitles")]
		string [] GetSectionIndexTitles ();
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

	[NoMac]
	[MacCatalyst (13, 1)]
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
		[return: NullAllowed]
		string SectionFor (NSFetchedResultsController controller, string sectionName);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSFetchedResultsSectionInfo {
		[Export ("numberOfObjects")]
		[Abstract]
		nint Count { get; }

		[NullAllowed, Export ("objects")]
		[Abstract]
		NSObject [] Objects { get; }

		[Export ("name")]
		[Abstract]
		string Name { get; }

		[NullAllowed, Export ("indexTitle")]
		[Abstract]
		string IndexTitle { get; }
	}

	interface INSFetchedResultsSectionInfo { }

	// 	NSInvalidArgumentException *** -loadMetadata: cannot be sent to an abstract object of class NSIncrementalStore: Create a concrete instance!
	//	Apple doc quote: "NSIncrementalStore is an abstract superclass..."
#if NET
	// Making a class abstract has problems: https://github.com/xamarin/xamarin-macios/issues/4969, so we're not doing this yet
	// [Abstract] // Abstract superclass.
#endif
	[BaseType (typeof (NSPersistentStore))]
	interface NSIncrementalStore {
#if NET
		[Protected]
#endif
		[Export ("initWithPersistentStoreCoordinator:configurationName:URL:options:")]
		NativeHandle Constructor (NSPersistentStoreCoordinator root, string name, NSUrl url, NSDictionary options);

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
#if NET
		NSObject GetIdentifierForNewStore (NSUrl storeUrl);
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
#if XAMCORE_5_0
		NativeHandle Constructor (NSManagedObjectID objectId, NSDictionary<NSString, NSObject> values, ulong version);
#else
		NativeHandle Constructor (NSManagedObjectID objectId, NSDictionary values, ulong version);
#endif

		[Export ("updateWithValues:version:")]
#if XAMCORE_5_0
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
		NativeHandle Constructor (NSEntityDescription entity, [NullAllowed] NSManagedObjectContext context);

		[MacCatalyst (13, 1)]
		[Export ("initWithContext:")]
		NativeHandle Constructor (NSManagedObjectContext moc);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("entity")]
		NSEntityDescription GetEntityDescription ();

		[MacCatalyst (13, 1)]
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

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
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
#if NET
		NSObject GetValue (string key);
#else
		IntPtr ValueForKey (string key);
#endif

		[Export ("setValue:forKey:")]
#if NET
		void SetValue ([NullAllowed] NSObject value, string key);
#else
		void SetValue (IntPtr value, string key);
#endif

		[Export ("primitiveValueForKey:")]
		[return: NullAllowed]
#if NET
		NSObject GetPrimitiveValue (string key);
#else
		IntPtr PrimitiveValueForKey (string key);
#endif

		[Export ("setPrimitiveValue:forKey:")]
#if NET
		void SetPrimitiveValue ([NullAllowed] NSObject value, string key);
#else
		void SetPrimitiveValue (IntPtr value, string key);
#endif

		[Export ("committedValuesForKeys:")]
#if XAMCORE_5_0
		NSDictionary<NSString, NSObject> GetCommittedValues ([NullAllowed] string[] keys);
#elif NET
		NSDictionary GetCommittedValues ([NullAllowed] string[] keys);
#else
		NSDictionary CommittedValuesForKeys ([NullAllowed] string [] keys);
#endif

		[Export ("changedValues")]
#if XAMCORE_5_0
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
		[MacCatalyst (13, 1)]
		[Export ("hasPersistentChangedValues")]
		bool HasPersistentChangedValues { get; }

		[MacCatalyst (13, 1)]
		[Export ("objectIDsForRelationshipNamed:")]
		NSManagedObjectID [] GetObjectIDs (string relationshipName);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSQueryGenerationToken : NSSecureCoding, NSCopying {
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
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSManagedObjectContext (NSManagedObjectContextConcurrencyType)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSManagedObjectContext (NSManagedObjectContextConcurrencyType)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSManagedObjectContext (NSManagedObjectContextConcurrencyType)' instead.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[NullAllowed] // by default this property is null
		[Export ("persistentStoreCoordinator", ArgumentSemantic.Retain)]
		NSPersistentStoreCoordinator PersistentStoreCoordinator { get; set; }

		[Export ("undoManager", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSUndoManager UndoManager { get; set; }

		[Export ("hasChanges")]
		bool HasChanges { get; }

		[Export ("objectRegisteredForID:")]
		[return: NullAllowed]
		NSManagedObject ObjectRegisteredForID (NSManagedObjectID objectID);

		[Export ("objectWithID:")]
		NSManagedObject ObjectWithID (NSManagedObjectID objectID);

		[Export ("executeFetchRequest:error:")]
		[return: NullAllowed]
		NSObject [] ExecuteFetchRequest (NSFetchRequest request, out NSError error);

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
#if XAMCORE_5_0
		void ObserveValue ([NullAllowed] string keyPath, [NullAllowed] NSObject object1, [NullAllowed] NSDictionary<NSString, NSObject> change, IntPtr context);
#elif NET
		void ObserveValue ([NullAllowed] string keyPath, [NullAllowed] NSObject object1, [NullAllowed] NSDictionary change, IntPtr context);
#else
		void ObserveValueForKeyPath ([NullAllowed] string keyPath, IntPtr object1, [NullAllowed] NSDictionary change, IntPtr context);
#endif

		[Export ("processPendingChanges")]
		void ProcessPendingChanges ();

		[Export ("assignObject:toPersistentStore:")]
#if NET
		void AssignObject (NSObject object1, NSPersistentStore store);
#else
		void AssignObject (IntPtr object1, NSPersistentStore store);
#endif

		[Export ("insertedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_5_0
		NSSet<NSManagedObject> InsertedObjects { get; }
#else
		NSSet InsertedObjects { get; }
#endif

		[Export ("updatedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_5_0
		NSSet<NSManagedObject> UpdatedObjects { get; }
#else
		NSSet UpdatedObjects { get; }
#endif

		[Export ("deletedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_5_0
		NSSet<NSManagedObject> DeletedObjects { get; }
#else
		NSSet DeletedObjects { get; }
#endif

		[Export ("registeredObjects", ArgumentSemantic.Strong)]
#if XAMCORE_5_0
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

#pragma warning disable 0109 // warning CS0109: The member 'NSManagedObjectContext.Lock()' does not hide an accessible member. The new keyword is not required.
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use a queue style context and 'PerformAndWait' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use a queue style context and 'PerformAndWait' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use a queue style context and 'PerformAndWait' instead.")]
		[Export ("lock")]
		new void Lock ();
#pragma warning restore

#pragma warning disable 0109 // warning CS0109: The member 'NSManagedObjectContext.Unlock()' does not hide an accessible member. The new keyword is not required.
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use a queue style context and 'PerformAndWait' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use a queue style context and 'PerformAndWait' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use a queue style context and 'PerformAndWait' instead.")]
		[Export ("unlock")]
		new void Unlock ();
#pragma warning restore

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use a queue style context and 'Perform' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use a queue style context and 'Perform' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use a queue style context and 'Perform' instead.")]
		[Export ("tryLock")]
		bool TryLock { get; }

		[Export ("propagatesDeletesAtEndOfEvent")]
		bool PropagatesDeletesAtEndOfEvent { get; set; }

		[Export ("retainsRegisteredObjects")]
		bool RetainsRegisteredObjects { get; set; }

		[Export ("stalenessInterval")]
		double StalenessInterval { get; set; }

		[Export ("mergePolicy", ArgumentSemantic.Retain)]
		IntPtr MergePolicy { get; set; }

		[Export ("obtainPermanentIDsForObjects:error:")]
		bool ObtainPermanentIDsForObjects (NSManagedObject [] objects, out NSError error);

		[Export ("mergeChangesFromContextDidSaveNotification:")]
		void MergeChangesFromContextDidSaveNotification (NSNotification notification);

		[DesignatedInitializer]
		[Export ("initWithConcurrencyType:")]
		NativeHandle Constructor (NSManagedObjectContextConcurrencyType ct);

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

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("executeRequest:error:")]
		[return: NullAllowed]
		NSPersistentStoreResult ExecuteRequest (NSPersistentStoreRequest request, out NSError error);

		[Export ("existingObjectWithID:error:")]
		[return: NullAllowed]
		NSManagedObject GetExistingObject (NSManagedObjectID objectID, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("shouldDeleteInaccessibleFaults")]
		bool ShouldDeleteInaccessibleFaults { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("shouldHandleInaccessibleFault:forObjectID:triggeredByProperty:")]
		bool ShouldHandleInaccessibleFault (NSManagedObject fault, NSManagedObjectID oid, [NullAllowed] NSPropertyDescription property);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("mergeChangesFromRemoteContextSave:intoContexts:")]
		void MergeChangesFromRemoteContextSave (NSDictionary changeNotificationData, NSManagedObjectContext [] contexts);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("queryGenerationToken", ArgumentSemantic.Strong)]
		NSQueryGenerationToken QueryGenerationToken { get; }

		[MacCatalyst (13, 1)]
		[Export ("setQueryGenerationFromToken:error:")]
		bool SetQueryGenerationFromToken ([NullAllowed] NSQueryGenerationToken generation, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("automaticallyMergesChangesFromParent")]
		bool AutomaticallyMergesChangesFromParent { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("refreshAllObjects")]
		void RefreshAllObjects ();

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("transactionAuthor")]
		string TransactionAuthor { get; set; }

		[Notification (typeof (NSManagedObjectsIdsChangedEventArgs))]
		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSManagedObjectContextDidMergeChangesObjectIDsNotification")]
		NSString DidMergeChangesObjectIdsNotification { get; }

		[Notification (typeof (NSManagedObjectsIdsChangedEventArgs))]
		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSManagedObjectContextDidSaveObjectIDsNotification")]
		NSString DidSaveObjectIdsNotification { get; }
	}

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	interface NSManagedObjectsIdsChangedEventArgs {
		[Export ("NSDeletedObjectIDsKey")]
		NSSet DeletedObjectIds { get; }

		[Export ("NSInsertedObjectIDsKey")]
		NSSet InsertedObjectIdsKey { get; }

		[Export ("NSInvalidatedObjectIDsKey")]
		NSSet InvalidatedObjectIdsKey { get; }

		[Export ("NSRefreshedObjectIDsKey")]
		NSSet RefreshedObjectIdsKey { get; }

		[Export ("NSUpdatedObjectIDsKey")]
		NSSet UpdatedObjectIdsKey { get; }
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
		NativeHandle Constructor ();

		[Static, Export ("mergedModelFromBundles:")]
		[return: NullAllowed]
#if NET
		NSManagedObjectModel GetMergedModel ([NullAllowed] NSBundle[] bundles);
#else
		NSManagedObjectModel MergedModelFromBundles ([NullAllowed] NSBundle [] bundles);
#endif

		[Static, Export ("modelByMergingModels:")]
		[return: NullAllowed]
		NSManagedObjectModel ModelByMergingModels ([NullAllowed] NSManagedObjectModel [] models);

		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor (NSUrl url);

		[Export ("entitiesByName", ArgumentSemantic.Copy)]
#if XAMCORE_5_0
		NSDictionary<NSString, NSEntityDescription> EntitiesByName { get; }
#else
		NSDictionary EntitiesByName { get; }
#endif

		[Export ("entities", ArgumentSemantic.Retain)]
		NSEntityDescription [] Entities { get; set; }

		[Export ("configurations", ArgumentSemantic.Strong)]
		string [] Configurations { get; }

		[Export ("entitiesForConfiguration:")]
		[return: NullAllowed]
		string [] EntitiesForConfiguration ([NullAllowed] string configuration);

		[Export ("setEntities:forConfiguration:")]
		void SetEntities (NSEntityDescription [] entities, string configuration);

		[Export ("setFetchRequestTemplate:forName:")]
		void SetFetchRequestTemplate ([NullAllowed] NSFetchRequest fetchRequestTemplate, string name);

		[Export ("fetchRequestTemplateForName:")]
		[return: NullAllowed]
#if NET
		NSFetchRequest GetFetchRequestTemplate (string name);
#else
		NSFetchRequest FetchRequestTemplateForName (string name);
#endif

		[Export ("fetchRequestFromTemplateWithName:substitutionVariables:")]
		[return: NullAllowed]
#if XAMCORE_5_0
		NSFetchRequest GetFetchRequestFromTemplate (string name, NSDictionary<NSString, NSObject> variables);
#elif NET
		NSFetchRequest GetFetchRequestFromTemplate (string name, NSDictionary variables);
#else
		NSFetchRequest FetchRequestFromTemplateWithName (string name, NSDictionary variables);
#endif

		[NullAllowed] // by default this property is null
		[Export ("localizationDictionary", ArgumentSemantic.Retain)]
#if XAMCORE_5_0
		NSDictionary<NSString, NSString> LocalizationDictionary { get; set; }
#else
		NSDictionary LocalizationDictionary { get; set; }
#endif

		[Static, Export ("mergedModelFromBundles:forStoreMetadata:")]
		[return: NullAllowed]
#if XAMCORE_5_0
		NSManagedObjectModel GetMergedModel ([NullAllowed] NSBundle[] bundles, NSDictionary<NSString, NSObject> metadata);
#elif NET
		NSManagedObjectModel GetMergedModel ([NullAllowed] NSBundle[] bundles, NSDictionary metadata);
#else
		NSManagedObjectModel MergedModelFromBundles ([NullAllowed] NSBundle [] bundles, NSDictionary metadata);
#endif

		[Static, Export ("modelByMergingModels:forStoreMetadata:")]
		[return: NullAllowed]
#if XAMCORE_5_0
		NSManagedObjectModel GetModelByMerging (NSManagedObjectModel[] models, NSDictionary<NSString, NSObject> metadata);
#elif NET
		NSManagedObjectModel GetModelByMerging (NSManagedObjectModel[] models, NSDictionary metadata);
#else
		NSManagedObjectModel ModelByMergingModels (NSManagedObjectModel [] models, NSDictionary metadata);
#endif

		[Export ("fetchRequestTemplatesByName", ArgumentSemantic.Copy)]
#if XAMCORE_5_0
		NSDictionary<NSString, NSFetchRequest> FetchRequestTemplatesByName { get; }
#else
		NSDictionary FetchRequestTemplatesByName { get; }
#endif

		[Export ("versionIdentifiers", ArgumentSemantic.Copy)]
		NSSet VersionIdentifiers { get; set; }

		[Export ("isConfiguration:compatibleWithStoreMetadata:")]
#if XAMCORE_5_0
		bool IsConfigurationCompatibleWithStoreMetadata ([NullAllowed] string configuration, NSDictionary<NSString, NSObject> metadata);
#elif NET
		bool IsConfigurationCompatibleWithStoreMetadata ([NullAllowed] string configuration, NSDictionary metadata);
#else
		bool IsConfiguration ([NullAllowed] string configuration, NSDictionary metadata);
#endif

		[Export ("entityVersionHashesByName", ArgumentSemantic.Copy)]
#if XAMCORE_5_0
		NSDictionary<NSString, NSData> EntityVersionHashesByName { get; }
#else
		NSDictionary EntityVersionHashesByName { get; }
#endif

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("checksumsForVersionedModelAtURL:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSString> ChecksumsForVersionedModel (NSUrl modelUrl, [NullAllowed] out NSError error);

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("versionChecksum")]
		string VersionChecksum { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSMappingModel {

		[Static, Export ("mappingModelFromBundles:forSourceModel:destinationModel:")]
		[return: NullAllowed]
#if NET
		NSMappingModel GetMappingModel ([NullAllowed] NSBundle[] bundles, [NullAllowed] NSManagedObjectModel sourceModel, [NullAllowed] NSManagedObjectModel destinationModel);
#else
		NSMappingModel MappingModelFromBundles ([NullAllowed] NSBundle [] bundles, [NullAllowed] NSManagedObjectModel sourceModel, [NullAllowed] NSManagedObjectModel destinationModel);
#endif

		[Static, Export ("inferredMappingModelForSourceModel:destinationModel:error:")]
		[return: NullAllowed]
		NSMappingModel GetInferredMappingModel (NSManagedObjectModel source, NSManagedObjectModel destination, out NSError error);

		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor ([NullAllowed] NSUrl url);

		[Export ("entityMappings", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSEntityMapping [] EntityMappings { get; set; }

		[Export ("entityMappingsByName", ArgumentSemantic.Copy)]
#if XAMCORE_5_0
		NSDictionary<NSString, NSEntityMapping> EntityMappingsByName { get; }
#else
		NSDictionary EntityMappingsByName { get; }
#endif

	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSMergeConflict {
		[Export ("sourceObject", ArgumentSemantic.Retain)]
		NSManagedObject SourceObject { get; }

		[Export ("objectSnapshot", ArgumentSemantic.Retain)]
		[NullAllowed]
#if XAMCORE_5_0
		NSDictionary<NSString, NSObject> ObjectSnapshot { get; }
#else
		NSDictionary ObjectSnapshot { get; }
#endif

		[Export ("cachedSnapshot", ArgumentSemantic.Retain)]
		[NullAllowed]
#if XAMCORE_5_0
		NSDictionary<NSString, NSObject> CachedSnapshot { get; }
#else
		NSDictionary CachedSnapshot { get; }
#endif

		[Export ("persistedSnapshot", ArgumentSemantic.Retain)]
		[NullAllowed]
#if XAMCORE_5_0
		NSDictionary<NSString, NSObject> PersistedSnapshot { get; }
#else
		NSDictionary PersistedSnapshot { get; }
#endif

		[Export ("newVersionNumber")]
		nuint NewVersionNumber { get; }

		[Export ("oldVersionNumber")]
		nuint OldVersionNumber { get; }

		[DesignatedInitializer]
		[Export ("initWithSource:newVersion:oldVersion:cachedSnapshot:persistedSnapshot:")]
#if XAMCORE_5_0
		NativeHandle Constructor (NSManagedObject sourceObject, nuint newVersion, nuint oldVersion, [NullAllowed] NSDictionary<NSString, NSObject> cachedSnapshot, [NullAllowed] NSDictionary<NSString, NSObject> persistedSnapshot);
#elif NET
		NativeHandle Constructor (NSManagedObject sourceObject, nuint newVersion, nuint oldVersion, [NullAllowed] NSDictionary cachedSnapshot, [NullAllowed] NSDictionary persistedSnapshot);
#else
		NativeHandle Constructor (NSManagedObject srcObject, nuint newvers, nuint oldvers, [NullAllowed] NSDictionary cachesnap, [NullAllowed] NSDictionary persnap);
#endif
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSMergePolicy {
		[Export ("mergeType")]
		NSMergePolicyType MergeType { get; }

		[DesignatedInitializer]
		[Export ("initWithMergeType:")]
		NativeHandle Constructor (NSMergePolicyType ty);

		[Export ("resolveConflicts:error:")]
#if NET
		bool ResolveConflicts (NSMergeConflict [] list, out NSError error);
#else
		bool ResolveConflictserror (NSMergeConflict [] list, out NSError error);
#endif

		[MacCatalyst (13, 1)]
		[Export ("resolveOptimisticLockingVersionConflicts:error:")]
		bool ResolveOptimisticLockingVersionConflicts (NSMergeConflict [] list, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("resolveConstraintConflicts:error:")]
		bool ResolveConstraintConflicts (NSConstraintConflict [] list, out NSError error);

		[MacCatalyst (13, 1)]
		[Static, Export ("errorMergePolicy", ArgumentSemantic.Strong)]
		NSMergePolicy ErrorPolicy { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("rollbackMergePolicy", ArgumentSemantic.Strong)]
		NSMergePolicy RollbackPolicy { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("overwriteMergePolicy", ArgumentSemantic.Strong)]
		NSMergePolicy OverwritePolicy { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("mergeByPropertyObjectTrumpMergePolicy", ArgumentSemantic.Strong)]
		NSMergePolicy MergeByPropertyObjectTrumpPolicy { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("mergeByPropertyStoreTrumpMergePolicy", ArgumentSemantic.Strong)]
		NSMergePolicy MergeByPropertyStoreTrumpPolicy { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSMigrationManager {

		[Export ("initWithSourceModel:destinationModel:")]
		NativeHandle Constructor (NSManagedObjectModel sourceModel, NSManagedObjectModel destinationModel);

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
		NSManagedObject [] DestinationInstancesForEntityMappingNamed (string mappingName, [NullAllowed] NSManagedObject [] sourceInstances);

		[Export ("sourceInstancesForEntityMappingNamed:destinationInstances:")]
		NSManagedObject [] SourceInstancesForEntityMappingNamed (string mappingName, [NullAllowed] NSManagedObject [] destinationInstances);

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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSPersistentHistoryChange : NSCopying {
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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[NullAllowed, Export ("entityDescription")]
		NSEntityDescription EntityDescription { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[NullAllowed, Export ("fetchRequest")]
		NSFetchRequest FetchRequest { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("entityDescriptionWithContext:")]
		[return: NullAllowed]
		NSEntityDescription GetEntityDescription (NSManagedObjectContext context);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSPersistentHistoryToken : NSCopying //, NSSecureCoding TODO: The class does state that it supports the NSSecureCoding YET SupportsSecureCoding returns false, radar 32761925
	{
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreRequest))]
	[DisableDefaultCtor]
	interface NSPersistentHistoryChangeRequest {
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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("fetchHistoryWithFetchRequest:")]
		NSPersistentHistoryChangeRequest FetchHistory (NSFetchRequest fetchRequest);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("fetchRequest", ArgumentSemantic.Strong)]
		NSFetchRequest FetchRequest { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreResult))]
	interface NSPersistentHistoryResult {
		[NullAllowed]
		[Export ("result", ArgumentSemantic.Strong)]
		NSObject Result { get; }

		[Export ("resultType")]
		NSPersistentHistoryResultType ResultType { get; }
	}

	[Abstract]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSPersistentHistoryTransaction : NSCopying {
		[Export ("timestamp", ArgumentSemantic.Copy)]
		NSDate Timestamp { get; }

		[NullAllowed, Export ("changes", ArgumentSemantic.Copy)]
		NSPersistentHistoryChange [] Changes { get; }

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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("entityDescriptionWithContext:")]
		[return: NullAllowed]
		NSEntityDescription GetEntityDescription (NSManagedObjectContext context);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[NullAllowed, Export ("entityDescription")]
		NSEntityDescription EntityDescription { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[NullAllowed, Export ("fetchRequest")]
		NSFetchRequest FetchRequest { get; }
	}

#if !WATCH
	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: NSCoreDataCoreSpotlightDelegate requires the use of the initializer initForStoreWithDescription:model: 
	interface NSCoreDataCoreSpotlightDelegate {

		[Notification]
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("NSCoreDataCoreSpotlightDelegateIndexDidUpdateNotification")]
		NSString IndexDidUpdateNotification { get; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initForStoreWithDescription:coordinator:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSPersistentStoreDescription description, NSPersistentStoreCoordinator psc);

		[Export ("domainIdentifier")]
		string DomainIdentifier { get; }

		[NullAllowed, Export ("indexName")]
		string IndexName { get; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use the constructor that takes a NSPersistentStoreCoordinator instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the constructor that takes a NSPersistentStoreCoordinator instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the constructor that takes a NSPersistentStoreCoordinator instead.")]
		[Export ("initForStoreWithDescription:model:")]
		NativeHandle Constructor (NSPersistentStoreDescription description, NSManagedObjectModel model);

		[Export ("attributeSetForObject:")]
		[return: NullAllowed]
		CSSearchableItemAttributeSet GetAttributeSet (NSManagedObject @object);

		[Export ("searchableIndex:reindexAllSearchableItemsWithAcknowledgementHandler:")]
		void ReindexAllSearchableItems (CSSearchableIndex searchableIndex, Action acknowledgementHandler);

		[Export ("searchableIndex:reindexSearchableItemsWithIdentifiers:acknowledgementHandler:")]
		void ReindexSearchableItems (CSSearchableIndex searchableIndex, string [] identifiers, Action acknowledgementHandler);

		[Async]
		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("deleteSpotlightIndexWithCompletionHandler:")]
		void DeleteSpotlightIndex (Action<NSError> completionHandler);

		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("indexingEnabled")]
		bool IndexingEnabled { [Bind ("isIndexingEnabled")] get; }

		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("startSpotlightIndexing")]
		void StartSpotlightIndexing ();

		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("stopSpotlightIndexing")]
		void StopSpotlightIndexing ();
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
#if XAMCORE_5_0
		NSDictionary<NSString, NSObject> GetMetadataForPersistentStore (NSUrl url, out NSError error);
#else
		NSDictionary MetadataForPersistentStoreWithUrl (NSUrl url, out NSError error);
#endif

		[Static, Export ("setMetadata:forPersistentStoreWithURL:error:")]
#if XAMCORE_5_0
		bool SetMetadata ([NullAllowed] NSDictionary<NSString, NSObject> metadata, NSUrl url, out NSError error);
#else
		bool SetMetadata ([NullAllowed] NSDictionary metadata, NSUrl url, out NSError error);
#endif

#if NET
		[Protected]
#endif
		[DesignatedInitializer]
		[Export ("initWithPersistentStoreCoordinator:configurationName:URL:options:")]
		NativeHandle Constructor ([NullAllowed] NSPersistentStoreCoordinator root, [NullAllowed] string name, NSUrl url, [NullAllowed] NSDictionary options);

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
		[NullAllowed]
		string Identifier { get; set; }

		[Export ("type")]
		string Type { get; }

		[Export ("readOnly")]
		bool ReadOnly { [Bind ("isReadOnly")] get; set; }

		[Export ("metadata", ArgumentSemantic.Retain)]
		[NullAllowed]
#if XAMCORE_5_0
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
		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("coreSpotlightExporter")]
		NSCoreDataCoreSpotlightDelegate CoreSpotlightExporter { get; }
#endif

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSPersistentStoreRemoteChangeNotificationPostOptionKey")]
		NSString RemoteChangeNotificationPostOptionKey { get; }

		[Notification (typeof (NSPersistentStoreRemoteChangeEventArgs))]
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSPersistentStoreRemoteChangeNotification")]
		NSString StoreRemoteChangeNotification { get; }

	}

	interface NSPersistentStoreRemoteChangeEventArgs {
		[Export ("NSStoreUUIDKey")]
		NSUuid Uuid { get; }

		[Export ("NSPersistentStoreURLKey")]
		string Url { get; }

		[Export ("NSPersistentHistoryTokenKey")]
		NSPersistentHistoryToken PersistentHistoryTracking { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPersistentStoreDescription : NSCopying {
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
		NativeHandle Constructor (NSUrl url);

		// NSPersistentStoreDescription_NSPersistentCloudKitContainerAdditions category
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("cloudKitContainerOptions", ArgumentSemantic.Strong)]
		NSPersistentCloudKitContainerOptions CloudKitContainerOptions { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPersistentContainer {
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
		NSPersistentStoreDescription [] PersistentStoreDescriptions { get; set; }

		[Export ("initWithName:")]
		NativeHandle Constructor (string name);

		[Export ("initWithName:managedObjectModel:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, NSManagedObjectModel model);

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
#if XAMCORE_5_0
		NSDictionary<NSString, NSValue> RegisteredStoreTypes { get; }
#else
		NSDictionary RegisteredStoreTypes { get; }
#endif

		[Static, Export ("registerStoreClass:forStoreType:")]
		void RegisterStoreClass ([NullAllowed] Class storeClass, NSString storeType);

		[Deprecated (PlatformName.iOS, 9, 0, message: "Use the method that takes an out NSError parameter.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use the method that takes an out NSError parameter.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use the method that takes an out NSError parameter.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the method that takes an out NSError parameter.")]
		[Static, Export ("metadataForPersistentStoreOfType:URL:error:")]
		[return: NullAllowed]
		NSDictionary MetadataForPersistentStoreOfType ([NullAllowed] NSString storeType, NSUrl url, out NSError error);

		[MacCatalyst (13, 1)]
		[Static, Export ("metadataForPersistentStoreOfType:URL:options:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> GetMetadata (string storeType, NSUrl url, [NullAllowed] NSDictionary options, out NSError error);

		[Deprecated (PlatformName.iOS, 9, 0, message: "Use the method that takes an 'out NSError' parameter.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use the method that takes an 'out NSError' parameter.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use the method that takes an 'out NSError' parameter.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the method that takes an 'out NSError' parameter.")]
		[Static, Export ("setMetadata:forPersistentStoreOfType:URL:error:")]
		bool SetMetadata ([NullAllowed] NSDictionary metadata, [NullAllowed] NSString storeType, NSUrl url, out NSError error);

		[MacCatalyst (13, 1)]
		[Static, Export ("setMetadata:forPersistentStoreOfType:URL:options:error:")]
		bool SetMetadata ([NullAllowed] NSDictionary<NSString, NSObject> metadata, string storeType, NSUrl url, [NullAllowed] NSDictionary options, out NSError error);

		[Export ("setMetadata:forPersistentStore:")]
#if XAMCORE_5_0
		void SetMetadata ([NullAllowed] NSDictionary<NSString, NSObject> metadata, NSPersistentStore store);
#else
		void SetMetadata ([NullAllowed] NSDictionary metadata, NSPersistentStore store);
#endif

		[Export ("metadataForPersistentStore:")]
#if XAMCORE_5_0
		NSDictionary<NSString, NSObject> GetMetadata (NSPersistentStore store);
#elif NET
		NSDictionary GetMetadata (NSPersistentStore store);
#else
		NSDictionary MetadataForPersistentStore (NSPersistentStore store);
#endif

		[DesignatedInitializer]
		[Export ("initWithManagedObjectModel:")]
		NativeHandle Constructor (NSManagedObjectModel model);

		[Export ("managedObjectModel", ArgumentSemantic.Strong)]
		NSManagedObjectModel ManagedObjectModel { get; }

		[Export ("persistentStores", ArgumentSemantic.Strong)]
		NSPersistentStore [] PersistentStores { get; }

		[Export ("persistentStoreForURL:")]
		[return: NullAllowed]
		NSPersistentStore PersistentStoreForUrl (NSUrl url);

		[Export ("URLForPersistentStore:")]
		NSUrl UrlForPersistentStore (NSPersistentStore store);

		[Export ("setURL:forPersistentStore:")]
		bool SetUrl (NSUrl url, NSPersistentStore store);

		[Export ("addPersistentStoreWithType:configuration:URL:options:error:")]
		[return: NullAllowed]
#if NET
		NSPersistentStore AddPersistentStore (NSString storeType, [NullAllowed] string configuration, [NullAllowed] NSUrl storeUrl, [NullAllowed] NSDictionary options, out NSError error);
#else
		NSPersistentStore AddPersistentStoreWithType (NSString storeType, [NullAllowed] string configuration, [NullAllowed] NSUrl storeUrl, [NullAllowed] NSDictionary options, out NSError error);
#endif

		[MacCatalyst (13, 1)]
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

#pragma warning disable 0109 // warning CS0109: The member 'NSManagedObjectContext.Lock()' does not hide an accessible member. The new keyword is not required.
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'PerformAndWait' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'PerformAndWait' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PerformAndWait' instead.")]
		[Export ("lock")]
		new void Lock ();
#pragma warning restore

#pragma warning disable 0109 // warning CS0109: The member 'NSManagedObjectContext.Unlock()' does not hide an accessible member. The new keyword is not required.
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'PerformAndWait' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'PerformAndWait' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PerformAndWait' instead.")]
		[Export ("unlock")]
		new void Unlock ();
#pragma warning restore

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'Perform' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'Perform' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Perform' instead.")]
		[Export ("tryLock")]
		bool TryLock { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 5)]
		[Static, Export ("metadataForPersistentStoreWithURL:error:")]
		[return: NullAllowed]
		NSDictionary MetadataForPersistentStoreWithUrl (NSUrl url, out NSError error);

		[Field ("NSSQLiteStoreType")]
		NSString SQLiteStoreType { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("NSXMLStoreType")]
		NSString XMLStoreType { get; }

		[Field ("NSBinaryStoreType")]
		NSString BinaryStoreType { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSBinaryStoreSecureDecodingClasses")]
		NSString BinaryStoreSecureDecodingClasses { get; }

		[MacCatalyst (13, 1)]
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

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("NSValidateXMLStoreOption")]
		NSString ValidateXMLStoreOption { get; }

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
#if NET
		NSObject Execute (NSPersistentStoreRequest request, NSManagedObjectContext context, out NSError error);
#else
		NSObject ExecuteRequestwithContexterror (NSPersistentStoreRequest request, NSManagedObjectContext context, out NSError error);
#endif

		[NoWatch]
		[NoTV]
		[Notification]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Please see the release notes and Core Data documentation.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please see the release notes and Core Data documentation.")]
		[Field ("NSPersistentStoreDidImportUbiquitousContentChangesNotification")]
		NSString DidImportUbiquitousContentChangesNotification { get; }

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Please see the release notes and Core Data documentation.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please see the release notes and Core Data documentation.")]
		[Field ("NSPersistentStoreUbiquitousContentNameKey")]
		NSString PersistentStoreUbiquitousContentNameKey { get; }

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Please see the release notes and Core Data documentation.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please see the release notes and Core Data documentation.")]
		[Field ("NSPersistentStoreUbiquitousContentURLKey")]
#if NET
		NSString PersistentStoreUbiquitousContentUrlKey { get; }
#else
		NSString PersistentStoreUbiquitousContentUrlLKey { get; }
#endif

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("NSPersistentStoreFileProtectionKey")]
		NSString PersistentStoreFileProtectionKey { get; }

		// 7.0

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSPersistentStoreUbiquitousPeerTokenOption")]
		NSString PersistentStoreUbiquitousPeerTokenOption { get; }

		[NoWatch]
		[NoTV]
		[Static]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Please see the release notes and Core Data documentation.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Please see the release notes and Core Data documentation.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please see the release notes and Core Data documentation.")]
		[Export ("removeUbiquitousContentAndPersistentStoreAtURL:options:error:")]
		bool RemoveUbiquitousContentAndPersistentStore (NSUrl storeUrl, [NullAllowed] NSDictionary options, out NSError error);

		[MacCatalyst (13, 1)]
		[Notification (typeof (NSPersistentStoreCoordinatorStoreChangeEventArgs))]
		[Field ("NSPersistentStoreCoordinatorStoresWillChangeNotification")]
		NSString StoresWillChangeNotification { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSPersistentStoreRebuildFromUbiquitousContentOption")]
		NSString RebuildFromUbiquitousContentOption { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSPersistentStoreRemoveUbiquitousMetadataOption")]
		NSString RemoveUbiquitousMetadataOption { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSPersistentStoreUbiquitousContainerIdentifierKey")]
		[Obsolete ("Use 'UbiquitousContainerIdentifierKey' instead.")]
		NSString eUbiquitousContainerIdentifierKey { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSPersistentStoreUbiquitousContainerIdentifierKey")]
		NSString UbiquitousContainerIdentifierKey { get; }

		// 11.0

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSCoreDataCoreSpotlightExporter")]
		NSString CoreSpotlightExporter { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSPersistentHistoryTrackingKey")]
		NSString HistoryTrackingKey { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("performBlock:")]
		void Perform (Action code);

		[MacCatalyst (13, 1)]
		[Export ("performBlockAndWait:")]
		void PerformAndWait (Action code);

		[MacCatalyst (13, 1)]
		[Export ("destroyPersistentStoreAtURL:withType:options:error:")]
		bool DestroyPersistentStore (NSUrl url, string storeType, [NullAllowed] NSDictionary options, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("replacePersistentStoreAtURL:destinationOptions:withPersistentStoreFromURL:sourceOptions:storeType:error:")]
		bool ReplacePersistentStore (NSUrl destinationUrl, [NullAllowed] NSDictionary destinationOptions, NSUrl sourceUrl, [NullAllowed] NSDictionary sourceOptions, string storeType, out NSError error);


		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("currentPersistentHistoryTokenFromStores:")]
		[return: NullAllowed]
		NSPersistentHistoryToken GetCurrentPersistentHistoryToken ([NullAllowed] NSObject [] stores);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0), MacCatalyst (14, 0)]
		[Export ("finishDeferredLightweightMigration:")]
		bool FinishDeferredLightweightMigration ([NullAllowed] out NSError error);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0), MacCatalyst (14, 0)]
		[Export ("finishDeferredLightweightMigrationTask:")]
		bool FinishDeferredLightweightMigrationTask ([NullAllowed] out NSError error);
	}

	interface NSPersistentStoreCoordinatorStoreChangeEventArgs {
		[NoWatch]
		[NoTV]
		[Export ("NSPersistentStoreUbiquitousTransitionTypeKey")]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Please see the release notes and Core Data documentation.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please see the release notes and Core Data documentation.")]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreAsynchronousResult))]
	interface NSAsynchronousFetchResult {
		[Export ("fetchRequest", ArgumentSemantic.Retain)]
		NSAsynchronousFetchRequest FetchRequest { get; }

		[Export ("finalResult", ArgumentSemantic.Retain)]
		[NullAllowed]
#if NET
		INSFetchRequestResult[] FinalResult { get; }
#else
		NSObject [] FinalResult { get; }
#endif
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSPersistentStoreResult {

	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreResult))]
	interface NSBatchUpdateResult {
		[Export ("result", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSObject Result { get; }

		[Export ("resultType")]
		NSBatchUpdateRequestResultType ResultType { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreResult))]
	interface NSPersistentStoreAsynchronousResult {
		[Export ("managedObjectContext", ArgumentSemantic.Retain)]
		NSManagedObjectContext ManagedObjectContext { get; }

		[Export ("operationError", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSError OperationError { get; }

		[Export ("progress", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSProgress Progress { get; }

		[Export ("cancel")]
		void Cancel ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSAsynchronousFetchRequest {
		[Export ("initWithFetchRequest:completionBlock:")]
		NativeHandle Constructor (NSFetchRequest request, [NullAllowed] Action<NSAsynchronousFetchResult> completion);

		[Export ("fetchRequest", ArgumentSemantic.Retain)]
		NSFetchRequest FetchRequest { get; }

		[Export ("estimatedResultCount")]
		nint EstimatedResultCount { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSPropertyDescription : NSCoding, NSCopying {

		[Export ("entity")]
		NSEntityDescription Entity { get; }

		// by default this property is null, but docs mention possible 
		// exception on setter so allowing a null is not a good idea
		[Export ("name")]
		string Name { get; set; }

		[Export ("optional")]
		bool Optional { [Bind ("isOptional")] get; set; }

		[Export ("transient")]
		bool Transient { [Bind ("isTransient")] get; set; }

		[Export ("validationPredicates")]
		NSPredicate [] ValidationPredicates { get; }

		[Export ("validationWarnings")]
		string [] ValidationWarnings { get; }

		[Export ("setValidationPredicates:withValidationWarnings:")]
		void SetValidationPredicates ([NullAllowed] NSPredicate [] validationPredicates, [NullAllowed] string [] validationWarnings);

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Retain)]
		NSDictionary UserInfo { get; set; }

		[Export ("indexed")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'NSEntityDescription.Indexes' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSEntityDescription.Indexes' instead.")]
		bool Indexed { [Bind ("isIndexed")] get; set; }

		[Export ("versionHash")]
		NSData VersionHash { get; }

		[NullAllowed] // by default this property is null
		[Export ("versionHashModifier")]
		string VersionHashModifier { get; set; }

		[Export ("renamingIdentifier")]
		[NullAllowed]
		string RenamingIdentifier { get; set; }

		// 5.0
		[Export ("indexedBySpotlight")]
		bool IndexedBySpotlight { [Bind ("isIndexedBySpotlight")] get; set; }

		[Export ("storedInExternalRecord")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CoreSpotlight' integration instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CoreSpotlight' integration instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CoreSpotlight' integration instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CoreSpotlight' integration instead.")]
		bool StoredInExternalRecord { [Bind ("isStoredInExternalRecord")] get; set; }
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
		bool Ordered { [Bind ("isOrdered")] get; set; }
	}

	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSSaveChangesRequest {
		[Export ("initWithInsertedObjects:updatedObjects:deletedObjects:lockedObjects:")]
#if XAMCORE_5_0
		NativeHandle Constructor ([NullAllowed] NSSet<NSManagedObject> insertedObjects, [NullAllowed] NSSet<NSManagedObject> updatedObjects, [NullAllowed] NSSet<NSManagedObject> deletedObjects, [NullAllowed] NSSet<NSManagedObject> lockedObjects);
#else
		NativeHandle Constructor ([NullAllowed] NSSet insertedObjects, [NullAllowed] NSSet updatedObjects, [NullAllowed] NSSet deletedObjects, [NullAllowed] NSSet lockedObjects);
#endif

		[NullAllowed, Export ("insertedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_5_0
		NSSet<NSManagedObject> InsertedObjects { get; }
#else
		NSSet InsertedObjects { get; }
#endif

		[NullAllowed, Export ("updatedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_5_0
		NSSet<NSManagedObject> UpdatedObjects { get; }
#else
		NSSet UpdatedObjects { get; }
#endif

		[NullAllowed, Export ("deletedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_5_0
		NSSet<NSManagedObject> DeletedObjects { get; }
#else
		NSSet DeletedObjects { get; }
#endif

		[NullAllowed, Export ("lockedObjects", ArgumentSemantic.Strong)]
#if XAMCORE_5_0
		NSSet<NSManagedObject> LockedObjects { get; }
#else
		NSSet LockedObjects { get; }
#endif
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSBatchUpdateRequest {
		[Export ("initWithEntityName:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string entityName);

		[Export ("initWithEntity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSEntityDescription entity);

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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreRequest))]
	[DisableDefaultCtor]
	interface NSBatchDeleteRequest {
		[Export ("initWithFetchRequest:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSFetchRequest fetch);

		[Export ("initWithObjectIDs:")]
		NativeHandle Constructor (NSManagedObjectID [] objects);

		[Export ("resultType", ArgumentSemantic.Assign)]
		NSBatchDeleteRequestResultType ResultType { get; set; }

		[Export ("fetchRequest", ArgumentSemantic.Copy)]
		NSFetchRequest FetchRequest { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreResult))]
	interface NSBatchDeleteResult {
		[NullAllowed, Export ("result", ArgumentSemantic.Strong)]
		NSObject Result { get; }

		[Export ("resultType")]
		NSBatchDeleteRequestResultType ResultType { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSConstraintConflict {
		[Export ("initWithConstraint:databaseObject:databaseSnapshot:conflictingObjects:conflictingSnapshots:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string [] contraint, [NullAllowed] NSManagedObject databaseObject, [NullAllowed] NSDictionary databaseSnapshot, NSManagedObject [] conflictingObjects, NSObject [] conflictingSnapshots);

#if MONOMAC
		[Export ("constraint", ArgumentSemantic.Copy)]
#else
		[Export ("constraint", ArgumentSemantic.Retain)]
#endif
		string [] Constraint { get; }

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
		NSManagedObject [] ConflictingObjects { get; }

#if MONOMAC
		[Export ("conflictingSnapshots", ArgumentSemantic.Copy)]
#else
		[Export ("conflictingSnapshots", ArgumentSemantic.Retain)]
#endif
		NSDictionary [] ConflictingSnapshots { get; }
	}

#if XAMCORE_5_0
	delegate bool NSBatchInsertRequestDictionaryHandler (NSMutableDictionary<NSString, NSObject> dictionary);
#else
	delegate bool NSBatchInsertRequestDictionaryHandler (NSMutableDictionary dictionary);
#endif
	delegate bool NSBatchInsertRequestManagedObjectHandler (NSManagedObject managedObject);

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreRequest))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: -init results in undefined behavior for NSBatchInsertRequest
	interface NSBatchInsertRequest {

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use another constructor instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use another constructor instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use another constructor instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 16, message: "Use another constructor instead.")]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use another constructor instead.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithEntity:dictionaryHandler:")]
		NativeHandle Constructor (NSEntityDescription entity, NSBatchInsertRequestDictionaryHandler handler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithEntity:managedObjectHandler:")]
		NativeHandle Constructor (NSEntityDescription entity, NSBatchInsertRequestManagedObjectHandler handler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithEntityName:dictionaryHandler:")]
		NativeHandle Constructor (string entityName, NSBatchInsertRequestDictionaryHandler handler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithEntityName:managedObjectHandler:")]
		NativeHandle Constructor (string entityName, NSBatchInsertRequestManagedObjectHandler handler);

		[Export ("entityName")]
		string EntityName { get; }

		[NullAllowed, Export ("entity", ArgumentSemantic.Strong)]
		NSEntityDescription Entity { get; }

		[NullAllowed, Export ("objectsToInsert", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> [] ObjectsToInsert { get; set; }

		[Export ("resultType", ArgumentSemantic.Assign)]
		NSBatchInsertRequestResultType ResultType { get; set; }

		[Static]
		[Export ("batchInsertRequestWithEntityName:objects:")]
		NSBatchInsertRequest BatchInsertRequest (string entityName, NSDictionary<NSString, NSObject> [] dictionaries);

		[DesignatedInitializer]
		[Export ("initWithEntityName:objects:")]
		NativeHandle Constructor (string entityName, NSDictionary<NSString, NSObject> [] dictionaries);

		[DesignatedInitializer]
		[Export ("initWithEntity:objects:")]
		NativeHandle Constructor (NSEntityDescription entity, NSDictionary<NSString, NSObject> [] dictionaries);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("dictionaryHandler", ArgumentSemantic.Copy)]
		NSBatchInsertRequestDictionaryHandler DictionaryHandler { get; set; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("managedObjectHandler", ArgumentSemantic.Copy)]
		NSBatchInsertRequestManagedObjectHandler ManagedObjectHandler { get; set; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("batchInsertRequestWithEntityName:dictionaryHandler:")]
		NSBatchInsertRequest CreateBatchInsertRequest (string entityName, NSBatchInsertRequestDictionaryHandler handler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("batchInsertRequestWithEntityName:managedObjectHandler:")]
		NSBatchInsertRequest CreateBatchInsertRequest (string entityName, NSBatchInsertRequestManagedObjectHandler handler);
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentStoreResult))]
	interface NSBatchInsertResult {
		[NullAllowed, Export ("result", ArgumentSemantic.Strong)]
		NSObject Result { get; }

		[Export ("resultType")]
		NSBatchInsertRequestResultType ResultType { get; }
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSAttributeDescription))]
	interface NSDerivedAttributeDescription : NSSecureCoding {
		[NullAllowed, Export ("derivationExpression", ArgumentSemantic.Strong)]
		NSExpression DerivationExpression { get; set; }
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void NSPersistentCloudKitContainerShareManagedObjectsHandler (NSSet<NSManagedObjectID> sharedObjectIds, CKShare share, CKContainer container, NSError error);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void NSPersistentCloudKitContainerFetchParticipantsMatchingLookupInfosHandler (NSArray<CKShareParticipant> fetchedParticipants, NSError error);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void NSPersistentCloudKitContainerPersistUpdatedShareHandler (CKShare persistedShare, NSError error);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void NSPersistentCloudKitContainerPurgeObjectsAndRecordsInZoneHandler (CKRecordZoneID purgedZoneId, NSError error);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void NSPersistentCloudKitContainerAcceptShareInvitationsHandler (NSArray<CKShareMetadata> acceptedShareMetadatas, NSError error);

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSPersistentContainer))]
	[DisableDefaultCtor]
	interface NSPersistentCloudKitContainer {

		[Export ("initWithName:managedObjectModel:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, NSManagedObjectModel model);

		[Export ("initializeCloudKitSchemaWithOptions:error:")]
		bool Initialize (NSPersistentCloudKitContainerSchemaInitializationOptions options, [NullAllowed] out NSError error);

		[Export ("recordForManagedObjectID:")]
		[return: NullAllowed]
		CKRecord GetRecord (NSManagedObjectID managedObjectId);

		[Export ("recordsForManagedObjectIDs:")]
		NSDictionary<NSManagedObjectID, CKRecord> GetRecords (NSManagedObjectID [] managedObjectIds);

		[Export ("recordIDForManagedObjectID:")]
		[return: NullAllowed]
		CKRecordID GetRecordId (NSManagedObjectID managedObjectId);

		[Export ("recordIDsForManagedObjectIDs:")]
		NSDictionary<NSManagedObjectID, CKRecordID> GetRecordIds (NSManagedObjectID [] managedObjectIds);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("canUpdateRecordForManagedObjectWithID:")]
		bool CanUpdateRecord (NSManagedObjectID objectID);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("canDeleteRecordForManagedObjectWithID:")]
		bool CanDeleteRecord (NSManagedObjectID objectID);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("canModifyManagedObjectsInStore:")]
		bool CanModifyManagedObjects (NSPersistentStore store);

		// NSPersistentCloudKitContainer_Sharing
		[Async (ResultTypeName = "NSPersistentCloudKitContainerAcceptShareInvitationsResult")]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("acceptShareInvitationsFromMetadata:intoPersistentStore:completion:")]
		void AcceptShareInvitations (CKShareMetadata [] metadata, NSPersistentStore persistentStore, [NullAllowed] NSPersistentCloudKitContainerAcceptShareInvitationsHandler handler);

		[Async (ResultTypeName = "NSPersistentCloudKitContainerPurgeObjectsAndRecordsInZone")]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("purgeObjectsAndRecordsInZoneWithID:inPersistentStore:completion:")]
		void PurgeObjectsAndRecordsInZone (CKRecordZoneID zoneId, [NullAllowed] NSPersistentStore persistentStore, [NullAllowed] NSPersistentCloudKitContainerPurgeObjectsAndRecordsInZoneHandler handler);

		[Async (ResultTypeName = "NSPersistentCloudKitContainerPersistUpdatedShareResult")]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("persistUpdatedShare:inPersistentStore:completion:")]
		void PersistUpdatedShare (CKShare share, NSPersistentStore persistentStore, [NullAllowed] NSPersistentCloudKitContainerPersistUpdatedShareHandler handler);

		[Async (ResultTypeName = "NSPersistentCloudKitContainerFetchParticipantsMatchingLookupInfosResult")]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("fetchParticipantsMatchingLookupInfos:intoPersistentStore:completion:")]
		void FetchParticipantsMatchingLookupInfos (CKUserIdentityLookupInfo [] lookupInfos, NSPersistentStore persistentStore, NSPersistentCloudKitContainerFetchParticipantsMatchingLookupInfosHandler handler);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("fetchSharesMatchingObjectIDs:error:")]
		[return: NullAllowed]
		NSDictionary<NSManagedObjectID, CKShare> FetchSharesMatchingObjectIds (NSManagedObjectID [] objectIDs, [NullAllowed] out NSError error);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("fetchSharesInPersistentStore:error:")]
		[return: NullAllowed]
		CKShare [] FetchSharesInPersistentStore ([NullAllowed] NSPersistentStore persistentStore, [NullAllowed] out NSError error);

		[Async (ResultTypeName = "NSPersistentCloudKitContainerShareManagedObjectsResult")]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("shareManagedObjects:toShare:completion:")]
		void ShareManagedObjects (NSManagedObject [] managedObjects, [NullAllowed] CKShare share, NSPersistentCloudKitContainerShareManagedObjectsHandler handler);

	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPersistentCloudKitContainerOptions {
		[Export ("containerIdentifier")]
		string ContainerIdentifier { get; }

		[Export ("initWithContainerIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string containerIdentifier);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("databaseScope", ArgumentSemantic.Assign)]
		CKDatabaseScope DatabaseScope { get; set; }
	}

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPersistentCloudKitContainerEvent : NSCopying {
		[Export ("identifier", ArgumentSemantic.Strong)]
		NSUuid Identifier { get; }

		[Export ("storeIdentifier", ArgumentSemantic.Strong)]
		string StoreIdentifier { get; }

		[Export ("type")]
		NSPersistentCloudKitContainerEventType Type { get; }

		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[NullAllowed, Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; }

		[Export ("succeeded")]
		bool Succeeded { get; }

		[NullAllowed, Export ("error", ArgumentSemantic.Strong)]
		NSError Error { get; }

		[Notification]
		[Field ("NSPersistentCloudKitContainerEventChangedNotification")]
		NSString ChangedNotification { get; }

		[Field ("NSPersistentCloudKitContainerEventUserInfoKey")]
		NSString UserInfoKey { get; }
	}

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSPersistentStoreRequest))]
	interface NSPersistentCloudKitContainerEventRequest {
		[Export ("resultType", ArgumentSemantic.Assign)]
		NSPersistentCloudKitContainerEventResultType ResultType { get; set; }

		[Static]
		[Export ("fetchEventsAfterDate:")]
		NSPersistentCloudKitContainerEventRequest FetchEventsAfter (NSDate date);

		[Static]
		[Export ("fetchEventsAfterEvent:")]
		NSPersistentCloudKitContainerEventRequest FetchEventsAfter ([NullAllowed] NSPersistentCloudKitContainerEvent @event);

		[Static]
		[Export ("fetchEventsMatchingFetchRequest:")]
		NSPersistentCloudKitContainerEventRequest FetchEvents (NSFetchRequest fetchRequest);

		[Static]
		[Export ("fetchRequestForEvents")]
		NSFetchRequest FetchRequest ();
	}

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSPersistentStoreResult))]
	[DisableDefaultCtor]
	interface NSPersistentCloudKitContainerEventResult {
		[NullAllowed, Export ("result", ArgumentSemantic.Strong)]
		NSObject Result { get; }

		[Export ("resultType")]
		NSPersistentCloudKitContainerEventResultType ResultType { get; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSStagedMigrationManager {
		[Export ("stages", ArgumentSemantic.Strong)]
		NSMigrationStage [] Stages { get; }

		[NullAllowed, Export ("container", ArgumentSemantic.Strong)]
		NSPersistentContainer Container { get; }

		[Export ("initWithMigrationStages:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSMigrationStage [] stages);
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface NSMigrationStage {
		[NullAllowed, Export ("label")]
		string Label { get; set; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSManagedObjectModelReference {
		[Export ("resolvedModel", ArgumentSemantic.Strong)]
		NSManagedObjectModel ResolvedModel { get; }

		[Export ("versionChecksum", ArgumentSemantic.Strong)]
		string VersionChecksum { get; }

		[Export ("initWithModel:versionChecksum:")]
		NativeHandle Constructor (NSManagedObjectModel model, string versionChecksum);

		[Export ("initWithFileURL:versionChecksum:")]
		NativeHandle Constructor (NSUrl fileUrl, string versionChecksum);

		[Export ("initWithEntityVersionHashes:inBundle:versionChecksum:")]
		NativeHandle Constructor (NSDictionary versionHash, [NullAllowed] NSBundle bundle, string versionChecksum);

		[Export ("initWithName:inBundle:versionChecksum:")]
		NativeHandle Constructor (string modelName, [NullAllowed] NSBundle bundle, string versionChecksum);
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSMigrationStage))]
	[DisableDefaultCtor]
	interface NSLightweightMigrationStage {
		[Export ("versionChecksums", ArgumentSemantic.Strong)]
		string [] VersionChecksums { get; }

		[Export ("initWithVersionChecksums:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string [] versionChecksums);
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSAttributeDescription))]
	interface NSCompositeAttributeDescription {
		[Export ("elements", ArgumentSemantic.Copy)]
		NSAttributeDescription [] Elements { get; set; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSMigrationStage))]
	[DisableDefaultCtor]
	interface NSCustomMigrationStage {
		[Export ("currentModel", ArgumentSemantic.Strong)]
		NSManagedObjectModelReference CurrentModel { get; }

		[Export ("nextModel", ArgumentSemantic.Strong)]
		NSManagedObjectModelReference NextModel { get; }

		[NullAllowed]
		[Export ("willMigrateHandler", ArgumentSemantic.Copy)]
		Func<NSStagedMigrationManager, NSCustomMigrationStage, NSError, bool> WillMigrateHandler { get; set; }

		[NullAllowed]
		[Export ("didMigrateHandler", ArgumentSemantic.Copy)]
		Func<NSStagedMigrationManager, NSCustomMigrationStage, NSError, bool> DidMigrateHandler { get; set; }

		[Export ("initWithCurrentModelReference:nextModelReference:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSManagedObjectModelReference currentModel, NSManagedObjectModelReference nextModel);
	}
}

