using ObjCRuntime;
using Foundation;
using CoreFoundation;
using System;

namespace OSLog {
	[Mac (10,15)]
	[BaseType (typeof(NSObject))]
	interface OSLogEntry
	{
		[Export ("composedMessage")]
		string ComposedMessage { get; }

		[Export ("date")]
		NSDate Date { get; }

		[Export ("storeCategory")]
		EntryStoreCategory StoreCategory { get; }
	}

	[Mac (10,15)]
	[Protocol]
	interface OSLogEntryFromProcess
	{
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

	[Mac (10,15)]
	[Protocol]
	interface OSLogEntryWithPayload
	{
		[Abstract]
		[Export ("category")]
		string Category { get; }

		[Abstract]
		[Export ("components")]
		OSLogMessageComponent[] Components { get; }

		[Abstract]
		[Export ("formatString")]
		string FormatString { get; }

		[Abstract]
		[Export ("subsystem")]
		string Subsystem { get; }
	}

	[Mac (10,15)]
	[BaseType (typeof(OSLogEntry))]
	interface OSLogEntryActivity : IOSLogEntryFromProcess
	{
		[Export ("parentActivityIdentifier")]
		ulong ParentActivityIdentifier { get; }
	}


	[Mac (10,15)]
	[BaseType (typeof(OSLogEntry))]
	interface OSLogEntryLog : IOSLogEntryFromProcess, IOSLogEntryWithPayload
	{
		[Export ("level")]
		EntryLogLevel Level { get; }
	}

	[Mac (10,15)]
	[BaseType (typeof(OSLogEntry))]
	interface OSLogEntrySignpost : IOSLogEntryFromProcess, IOSLogEntryWithPayload
	{
		[Export ("signpostIdentifier")]
		ulong SignpostIdentifier { get; }

		[Export ("signpostName")]
		string SignpostName { get; }

		[Export ("signpostType")]
		EntrySignpostType SignpostType { get; }
	}



	[Mac (10,15)]
	[BaseType (typeof(NSObject))]
	interface OSLogMessageComponent
	{
		[Export ("formatSubstring")]
		string FormatSubstring { get; }

		[Export ("placeholder")]
		string Placeholder { get; }

		[Export ("argumentCategory")]
		MessageComponentArgumentCategory ArgumentCategory { get; }

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

	[Mac (10,15)]
	[BaseType (typeof(NSObject))]
	interface OSLogStore
	{
		[Static]
		[Export ("localStoreAndReturnError:")]
		[return: NullAllowed]
		OSLogStore LocalStoreAndReturnError ([NullAllowed] out NSError error);

		[Static]
		[Export ("storeWithURL:error:")]
		[return: NullAllowed]
		OSLogStore StoreWithURL (NSUrl url, [NullAllowed] out NSError error);

		[Export ("entriesEnumeratorWithOptions:position:predicate:error:")]
		[return: NullAllowed]
		OSLogEnumerator EntriesEnumeratorWithOptions (EnumeratorOptions options, [NullAllowed] OSLogPosition position, [NullAllowed] NSPredicate predicate, [NullAllowed] out NSError error);

		[Unavailable (PlatformName.Swift)]
		[Export ("entriesEnumeratorAndReturnError:")]
		[return: NullAllowed]
		OSLogEnumerator EntriesEnumeratorAndReturnError ([NullAllowed] out NSError error);

		[Export ("positionWithDate:")]
		OSLogPosition PositionWithDate (NSDate date);

		[Export ("positionWithTimeIntervalSinceEnd:")]
		OSLogPosition PositionWithTimeIntervalSinceEnd (double seconds);

		[Export ("positionWithTimeIntervalSinceLatestBoot:")]
		OSLogPosition PositionWithTimeIntervalSinceLatestBoot (double seconds);
	}
}

