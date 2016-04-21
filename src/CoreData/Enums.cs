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

using XamCore.ObjCRuntime;

namespace XamCore.CoreData {

	// NUInteger -> NSEntityMapping.h
	[Native]
	public enum NSEntityMappingType : nuint {
		Undefined = 0x00,
		Custom = 0x01,
		Add = 0x02,
		Remove = 0x03,
		Copy = 0x05,
		Transform = 0x06
	}

	// NUInteger -> NSAttributeDescription.h
	[Native]
	public enum NSAttributeType : nuint {
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
		Transformable = 1800,
		ObjectID = 2000
	}

	// NUInteger -> NSFetchRequest.h
	[Flags]
	[Native]
	public enum NSFetchRequestResultType : nuint {
		ManagedObject = 0x00,
		ManagedObjectID = 0x01,
		[Availability (Introduced = Platform.iOS_3_0 | Platform.Mac_10_6)]
		DictionaryResultType = 0x02,
		[Availability (Introduced = Platform.iOS_3_0 | Platform.Mac_10_6)]
		NSCountResultType = 0x04
	}

#if !XAMCORE_2_0
	// NUInteger -> NSKeyValueObserving.h in Foundation.framework (and it already exists there)
	[Native]
	public enum NSKeyValueSetMutationKind : nuint {
		Union = 1,
		Minus = 2,
		Intersect = 3,
		NSKeyValueSet = 4 // misnamed
	}
#endif

	// NUInteger -> NSRelationshipDescription.h
	[Native]
	public enum NSDeleteRule : nuint {
		NoAction,
		Nullify,
		Cascade,
		Deny
	}

	// NUInteger -> NSPersistentStoreRequest.h
	[Native]
	public enum NSPersistentStoreRequestType : nuint_compat_int {
		Fetch = 1,
		Save,
		BatchUpdate = 6,
		BatchDelete = 7
	}

	// NUInteger -> NSManagedObjectContext.h
	[Native]
	public enum NSManagedObjectContextConcurrencyType : nuint_compat_int {
		Confinement, PrivateQueue, MainQueue
	}

	// NUInteger -> NSManagedObjectContext.h
	[Native]
	public enum NSMergePolicyType : nuint_compat_int {
		Error, PropertyStoreTrump, PropertyObjectTrump, Overwrite, RollbackMerge
	}

	// NUInteger -> NSFetchedResultsController.h
	[Native]
	public enum NSFetchedResultsChangeType : nuint_compat_int {
		Insert = 1,
		Delete = 2,
		Move = 3,
		Update = 4
	}

	[Native]
	public enum NSBatchUpdateRequestResultType : nuint {
		StatusOnly = 0,
		UpdatedObjectIDs = 1,
		UpdatedObjectsCount = 2
	}

	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum NSBatchDeleteRequestResultType : nuint {
		StatusOnly = 0,
		ObjectIDs = 1,
		Count = 2
	}
}
