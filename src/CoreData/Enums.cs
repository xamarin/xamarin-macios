//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright 2011-2014 Xamarin Inc.

using System;

using ObjCRuntime;

namespace CoreData {

	// NUInteger -> NSEntityMapping.h
	[Native]
	public enum NSEntityMappingType : ulong {
		Undefined = 0x00,
		Custom = 0x01,
		Add = 0x02,
		Remove = 0x03,
		Copy = 0x04,
		Transform = 0x05
	}

	// NUInteger -> NSAttributeDescription.h
	[Native]
	public enum NSAttributeType : ulong {
		Undefined = 0,
		Integer16 = 100,
		Integer32 = 200,
		Integer64 = 300,
		Decimal = 400,
		Double = 500,
		Float = 600,
		String = 700,
		Boolean = 800,
		Date = 900,
		Binary = 1000,
		[iOS (11,0), TV (11,0), Mac (10,13), Watch (4,0)]
		Uuid = 1100,
		[iOS (11,0), TV (11,0), Mac (10,13), Watch (4,0)]
		Uri = 1200,
		Transformable = 1800,
		ObjectID = 2000
	}

	// NUInteger -> NSFetchRequest.h
	[Flags]
	[Native]
	public enum NSFetchRequestResultType : ulong {
		ManagedObject = 0x00,
		ManagedObjectID = 0x01,
		DictionaryResultType = 0x02,
		NSCountResultType = 0x04
	}

#if !XAMCORE_2_0
	// NUInteger -> NSKeyValueObserving.h in Foundation.framework (and it already exists there)
	[Native]
	public enum NSKeyValueSetMutationKind : ulong {
		Union = 1,
		Minus = 2,
		Intersect = 3,
		NSKeyValueSet = 4 // misnamed
	}
#endif

	// NUInteger -> NSRelationshipDescription.h
	[Native]
	public enum NSDeleteRule : ulong {
		NoAction,
		Nullify,
		Cascade,
		Deny
	}

	// NUInteger -> NSPersistentStoreRequest.h
	[Native]
	public enum NSPersistentStoreRequestType : ulong {
		Fetch = 1,
		Save,
		BatchUpdate = 6,
		BatchDelete = 7
	}

	// NUInteger -> NSManagedObjectContext.h
	[Native]
	public enum NSManagedObjectContextConcurrencyType : ulong {
		Confinement, PrivateQueue, MainQueue
	}

	// NUInteger -> NSManagedObjectContext.h
	[Native]
	public enum NSMergePolicyType : ulong {
		Error, PropertyStoreTrump, PropertyObjectTrump, Overwrite, RollbackMerge
	}

	// NUInteger -> NSFetchedResultsController.h
	[Native]
	public enum NSFetchedResultsChangeType : ulong {
		Insert = 1,
		Delete = 2,
		Move = 3,
		Update = 4
	}

	[Native]
	public enum NSBatchUpdateRequestResultType : ulong {
		StatusOnly = 0,
		UpdatedObjectIDs = 1,
		UpdatedObjectsCount = 2
	}

	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum NSBatchDeleteRequestResultType : ulong {
		StatusOnly = 0,
		ObjectIDs = 1,
		Count = 2
	}

	[Native]
	public enum ValidationErrorType : ulong {
		ManagedObjectValidation = 1550,
		MultipleErrors = 1560,
		MissingMandatoryProperty = 1570,
		RelationshipLacksMinimumCount = 1580,
		RelationshipExceedsMaximumCount = 1590,
		RelationshipDeniedDelete = 1600,
		NumberTooLarge = 1610,
		NumberTooSmall = 1620,
		DateTooLate = 1630,
		DateTooSoon = 1640,
		InvalidDate = 1650,
		StringTooLong = 1660,
		StringTooShort = 1670,
		StringPatternMatching = 1680,
		[iOS (11,0), TV (11,0), Mac (10,13), Watch (4,0)]
		InvalidUri = 1690,
	}

	[Native]
	public enum ObjectGraphManagementErrorType : ulong {
		ManagedObjectContextLocking = 132000,
		PersistentStoreCoordinatorLocking = 132010,
		ManagedObjectReferentialIntegrity = 133000,
		ManagedObjectExternalRelationship = 133010,
		ManagedObjectMerge = 133020
	}

	[Native]
	public enum PersistentStoreErrorType : ulong {
		InvalidType = 134000,
		TypeMismatch = 134010,
		IncompatibleSchema = 134020,
		Save = 134030,
		IncompleteSave = 134040,
		SaveConflicts = 134050,
		Operation = 134070,
		Open = 134080,
		Timeout = 134090,
		IncompatibleVersionHash = 134100
	}
	
	[Native]
	public enum MigrationErrorType {
		Migration = 134110,
		MigrationCancelled = 134120,
		MigrationMissingSourceModel = 134130,
		MigrationMissingMappingModel = 134140,
		MigrationManagerSourceStore = 134150,
		MigrationManagerDestinationStore = 134160,
		EntityMigrationPolicy = 134170,
		InferredMappingModel = 134190,
		ExternalRecordImport = 134200,
		[iOS (11,0), TV (11,0), Mac (10,13), Watch (4,0)]
		HistoryTokenExpired = 134301,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum NSFetchIndexElementType : ulong
	{
		Binary,
		RTree
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum NSPersistentHistoryChangeType : long
	{
		Insert,
		Update,
		Delete
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum NSPersistentHistoryResultType : long
	{
		StatusOnly = 0,
		ObjectIds = 1,
		Count = 2,
		TransactionsOnly = 3,
		ChangesOnly = 4,
		TransactionsAndChanges = 5
	}
}
