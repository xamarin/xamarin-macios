using System;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace OSLog {

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[Native]
	enum OSLogEntryLogLevel : long {
		Undefined,
		Debug,
		Info,
		Notice,
		Error,
		Fault,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[Native]
	enum OSLogEntrySignpostType : long {
		Undefined,
		IntervalBegin,
		IntervalEnd,
		Event,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[Native]
	enum OSLogEntryStoreCategory : long {
		Undefined,
		Metadata,
		ShortTerm,
		LongTermAuto,
		LongTerm1,
		LongTerm3,
		LongTerm7,
		LongTerm14,
		LongTerm30,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[Flags]
	[Native]
	enum OSLogEnumeratorOptions : ulong {
		Reverse = 0x1,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[Native]
	enum OSLogMessageComponentArgumentCategory : long {
		Undefined,
		Data,
		Double,
		Int64,
		String,
		UInt64,
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	enum OSLogStoreScope : long {
		System = 0,
		CurrentProcessIdentifier = 1,
	}


	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface OSLogEntryFromProcess {

		[Abstract]
		[Export ("activityIdentifier")]
		ulong ActivityIdentifier { get; }

		[Abstract]
		[Export ("process")]
		string Process { get; }

		[Abstract]
		[Export ("processIdentifier")]
		int ProcessIdentifier { get; }

		[Abstract]
		[Export ("sender")]
		string Sender { get; }

		[Abstract]
		[Export ("threadIdentifier")]
		ulong ThreadIdentifier { get; }
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface OSLogEntryWithPayload {

		[Abstract]
		[Export ("category")]
		string Category { get; }

		[Abstract]
		[Export ("components")]
		OSLogMessageComponent [] Components { get; }

		[Abstract]
		[Export ("formatString")]
		string FormatString { get; }

		[Abstract]
		[Export ("subsystem")]
		string Subsystem { get; }
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface OSLogEntry {

		[Export ("composedMessage")]
		string ComposedMessage { get; }

		[Export ("date")]
		NSDate Date { get; }

		[Export ("storeCategory")]
		OSLogEntryStoreCategory StoreCategory { get; }
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (OSLogEntry))]
	[DisableDefaultCtor]
	interface OSLogEntryActivity : OSLogEntryFromProcess {

		[Export ("parentActivityIdentifier")]
		ulong ParentActivityIdentifier { get; }
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (OSLogEntry))]
	[DisableDefaultCtor]
	interface OSLogEntryBoundary {
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (OSLogEntry))]
	[DisableDefaultCtor]
	interface OSLogEntryLog : OSLogEntryFromProcess, OSLogEntryWithPayload {

		[Export ("level")]
		OSLogEntryLogLevel Level { get; }
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (OSLogEntry))]
	[DisableDefaultCtor]
	interface OSLogEntrySignpost : OSLogEntryFromProcess, OSLogEntryWithPayload {

		[Export ("signpostIdentifier")]
		ulong SignpostIdentifier { get; }

		[Export ("signpostName")]
		string SignpostName { get; }

		[Export ("signpostType")]
		OSLogEntrySignpostType SignpostType { get; }
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSEnumerator))]
	[DisableDefaultCtor]
	interface OSLogEnumerator {
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface OSLogMessageComponent : NSSecureCoding {

		[Export ("formatSubstring")]
		string FormatSubstring { get; }

		[Export ("placeholder")]
		string Placeholder { get; }

		[Export ("argumentCategory")]
		OSLogMessageComponentArgumentCategory ArgumentCategory { get; }

		[NullAllowed, Export ("argumentDataValue")]
		NSData ArgumentDataValue { get; }

		[Export ("argumentDoubleValue")]
		double ArgumentDoubleValue { get; }

		[Export ("argumentInt64Value")]
		long ArgumentInt64Value { get; }

		[NullAllowed, Export ("argumentNumberValue")]
		NSNumber ArgumentNumberValue { get; }

		[NullAllowed, Export ("argumentStringValue")]
		string ArgumentStringValue { get; }

		[Export ("argumentUInt64Value")]
		ulong ArgumentUInt64Value { get; }
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface OSLogPosition {
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), Mac (11, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // init was added only on macos 12
	interface OSLogStore {

		[NoWatch, NoTV, NoiOS, Mac (12, 0), MacCatalyst (15, 0)]
		[Export ("init")]
		NativeHandle Constructor ();

		[NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		[Static]
		[Export ("localStoreAndReturnError:")]
		[return: NullAllowed]
		OSLogStore CreateLocalStore ([NullAllowed] out NSError error);

		[Static]
		[Export ("storeWithURL:error:")]
		[return: NullAllowed]
		OSLogStore CreateStore (NSUrl url, [NullAllowed] out NSError error);

		[Mac (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("storeWithScope:error:")]
		[return: NullAllowed]
		OSLogStore CreateStore (OSLogStoreScope scope, [NullAllowed] out NSError error);

		[Export ("entriesEnumeratorWithOptions:position:predicate:error:")]
		[return: NullAllowed]
		OSLogEnumerator GetEntriesEnumerator (OSLogEnumeratorOptions options, [NullAllowed] OSLogPosition position, [NullAllowed] NSPredicate predicate, [NullAllowed] out NSError error);

		[Export ("entriesEnumeratorAndReturnError:")]
		[return: NullAllowed]
		OSLogEnumerator GetEntriesEnumerator ([NullAllowed] out NSError error);

		[Export ("positionWithDate:")]
		OSLogPosition GetPosition (NSDate date);

		[Export ("positionWithTimeIntervalSinceEnd:")]
		OSLogPosition GetPositionWithTimeIntervalSinceEnd (double seconds);

		[Export ("positionWithTimeIntervalSinceLatestBoot:")]
		OSLogPosition GetPositionWithTimeIntervalSinceLatestBoot (double seconds);
	}
}
