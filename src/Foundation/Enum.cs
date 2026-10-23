//
// Enums.cs: enumeration definitions for Foundation
//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2011, 2012 Xamarin Inc
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

namespace Foundation {

	/// <summary>Encodings supported by NSString.Encode.</summary>
	/// <remarks>The encodings supported by NSData and NSString.</remarks>
	[Native]
	public enum NSStringEncoding : ulong {
		/// <summary>ASCII encoding contains, 7-bit of information stored in 8 bits.</summary>
		ASCIIStringEncoding = 1,
		/// <summary>To be added.</summary>
		NEXTSTEP = 2,
		/// <summary>To be added.</summary>
		JapaneseEUC = 3,
		/// <summary>8-bit based variable-length character encoding for Unicode. </summary>
		UTF8 = 4,
		/// <summary>8-bit ISO/IEC 8859-1, also knows as Latin1 encoding.</summary>
		ISOLatin1 = 5,
		/// <summary>To be added.</summary>
		Symbol = 6,
		/// <summary>To be added.</summary>
		NonLossyASCII = 7,
		/// <summary>To be added.</summary>
		ShiftJIS = 8,
		/// <summary>8-bit ISO/IEC 8859-2, also knows as Latin2 encoding.</summary>
		ISOLatin2 = 9,
		/// <summary>To be added.</summary>
		Unicode = 10,
		/// <summary>To be added.</summary>
		WindowsCP1251 = 11,
		/// <summary>To be added.</summary>
		WindowsCP1252 = 12,
		/// <summary>To be added.</summary>
		WindowsCP1253 = 13,
		/// <summary>To be added.</summary>
		WindowsCP1254 = 14,
		/// <summary>To be added.</summary>
		WindowsCP1250 = 15,
		/// <summary>To be added.</summary>
		ISO2022JP = 21,
		/// <summary>To be added.</summary>
		MacOSRoman = 30,
		/// <summary>16 bit-based variable length encoding, blocks are interpreted as big endian.</summary>
		UTF16BigEndian = 0x90000100,
		/// <summary>16 bit-based variable length encoding, blocks are interpreted as litle endian. </summary>
		UTF16LittleEndian = 0x94000100,
		/// <summary>32-bit unicode encoding.</summary>
		UTF32 = 0x8c000100,
		/// <summary>32 bit encoding, blocks are interpreted as big endian.</summary>
		UTF32BigEndian = 0x98000100,
		/// <summary>32 bit encoding, blocks are interpreted as little endian.</summary>
		UTF32LittleEndian = 0x9c000100,
	};

	/// <summary>An enumeration of options available to NSString search and comparison methods.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSStringCompareOptions : ulong {
		/// <summary>To be added.</summary>
		CaseInsensitiveSearch = 1,
		/// <summary>To be added.</summary>
		LiteralSearch = 2,
		/// <summary>To be added.</summary>
		BackwardsSearch = 4,
		/// <summary>To be added.</summary>
		AnchoredSearch = 8,
		/// <summary>To be added.</summary>
		NumericSearch = 64,
		/// <summary>To be added.</summary>
		DiacriticInsensitiveSearch = 128,
		/// <summary>To be added.</summary>
		WidthInsensitiveSearch = 256,
		/// <summary>To be added.</summary>
		ForcedOrderingSearch = 512,
		/// <summary>To be added.</summary>
		RegularExpressionSearch = 1024,
	}

	/// <summary>Determines how credentials are persisted.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSUrlCredentialPersistence : ulong {
		/// <summary>Not persisted.</summary>
		None,
		/// <summary>Persisted for the session.</summary>
		ForSession,
		/// <summary>Permanently.</summary>
		Permanent,
		/// <summary>To be added.</summary>
		Synchronizable,
	}

#if MONOMAC

	/// <summary>The bundle's architecture.</summary>
	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	public enum NSBundleExecutableArchitecture {
		/// <summary>x86</summary>
		I386 = 0x00000007,
		/// <summary>PPC</summary>
		PPC = 0x00000012,
		/// <summary>x86-64</summary>
		X86_64 = 0x01000007,
		/// <summary>PPC64</summary>
		PPC64 = 0x01000012,
		ARM64 = 0x0100000c,
	}
#endif

	/// <summary>Comparison result in the Foundation Framework.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSComparisonResult : long {
		/// <summary>To be added.</summary>
		Ascending = -1,
		/// <summary>To be added.</summary>
		Same,
		/// <summary>To be added.</summary>
		Descending,
	}

	/// <summary>NSUrlRequest caching policy.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSUrlRequestCachePolicy : ulong {
		/// <summary>To be added.</summary>
		UseProtocolCachePolicy = 0,
		/// <summary>To be added.</summary>
		ReloadIgnoringLocalCacheData = 1,
		/// <summary>To be added.</summary>
		ReloadIgnoringLocalAndRemoteCacheData = 4, // Unimplemented
		/// <summary>To be added.</summary>
		ReloadIgnoringCacheData = ReloadIgnoringLocalCacheData,

		/// <summary>To be added.</summary>
		ReturnCacheDataElseLoad = 2,
		/// <summary>To be added.</summary>
		ReturnCacheDataDoNotLoad = 3,

		/// <summary>To be added.</summary>
		ReloadRevalidatingCacheData = 5, // Unimplemented
	}

	/// <summary>An enumeration of values representing valid caching strategies for use with NSUrls.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSUrlCacheStoragePolicy : ulong {
		/// <summary>To be added.</summary>
		Allowed,
		/// <summary>To be added.</summary>
		AllowedInMemoryOnly,
		/// <summary>To be added.</summary>
		NotAllowed,
	}

	/// <summary>The current status of an NSStream.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSStreamStatus : ulong {
		/// <summary>The stream is not yet open.</summary>
		NotOpen = 0,
		/// <summary>The stream is in the opening state.</summary>
		Opening = 1,
		/// <summary>The stream has been opened.</summary>
		Open = 2,
		/// <summary>The stream is reading.</summary>
		Reading = 3,
		/// <summary>The stream is writing.</summary>
		Writing = 4,
		/// <summary>At the end.</summary>
		AtEnd = 5,
		/// <summary>The stream is closed.</summary>
		Closed = 6,
		/// <summary>Error</summary>
		Error = 7,
	}

	/// <summary>The format to use during serialization using NSKeyedArchiver.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSPropertyListFormat : ulong {
		/// <summary>Store in the old OpenStep format.</summary>
		OpenStep = 1,
		/// <summary>Store in XML format.</summary>
		Xml = 100,
		/// <summary>Store in the binary file format.</summary>
		Binary = 200,
	}

	/// <summary>An enumeration of values specifying mutability options for property lists.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSPropertyListMutabilityOptions : ulong {
		/// <summary>To be added.</summary>
		Immutable = 0,
		/// <summary>To be added.</summary>
		MutableContainers = 1,
		/// <summary>To be added.</summary>
		MutableContainersAndLeaves = 2,
	}

	// Should mirror NSPropertyListMutabilityOptions
	/// <summary>An enumeration of mutability options for use with property lists.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSPropertyListWriteOptions : ulong {
		/// <summary>To be added.</summary>
		Immutable = 0,
		/// <summary>To be added.</summary>
		MutableContainers = 1,
		/// <summary>To be added.</summary>
		MutableContainersAndLeaves = 2,
	}

	// Should mirror NSPropertyListMutabilityOptions, but currently
	// not implemented (always use Immutable/0)
	/// <summary>Apple states that functionality related to this class is not implemented.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSPropertyListReadOptions : ulong {
		/// <summary>To be added.</summary>
		Immutable = 0,
		/// <summary>To be added.</summary>
		MutableContainers = 1,
		/// <summary>To be added.</summary>
		MutableContainersAndLeaves = 2,
	}

	/// <summary>A flagging enumeration whose values specify options in calls to <see cref="Foundation.NSMachPort.FromMachPort(System.UInt32,Foundation.NSMachPortRights)" />.</summary>
	/// <remarks>To be added.</remarks>
	[NativeName ("NSMachPortOptions")]
	[Native]
	[Flags]
	public enum NSMachPortRights : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		SendRight = (1 << 0),
		/// <summary>To be added.</summary>
		ReceiveRight = (1 << 1),
	}

	/// <summary>Status codes for the NSNetService.</summary>
	/// <remarks>To be added.</remarks>
	[NativeName ("NSNetServicesError")]
	[Native]
	public enum NSNetServicesStatus : long {
		/// <summary>To be added.</summary>
		UnknownError = -72000,
		/// <summary>To be added.</summary>
		CollisionError = -72001,
		/// <summary>To be added.</summary>
		NotFoundError = -72002,
		/// <summary>To be added.</summary>
		ActivityInProgress = -72003,
		/// <summary>To be added.</summary>
		BadArgumentError = -72004,
		/// <summary>To be added.</summary>
		CancelledError = -72005,
		/// <summary>To be added.</summary>
		InvalidError = -72006,
		/// <summary>To be added.</summary>
		TimeoutError = -72007,
		MissingRequiredConfigurationError = -72008,
	}

	/// <summary>NSNetService options.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSNetServiceOptions : ulong {
		/// <summary>To be added.</summary>
		NoAutoRename = 1 << 0,
		/// <summary>To be added.</summary>
		ListenForConnections = 1 << 1,
	}

	/// <summary>An enumeration of values that specify different date-format styles.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSDateFormatterStyle : ulong {
		/// <summary>To be added.</summary>
		None,
		/// <summary>To be added.</summary>
		Short,
		/// <summary>To be added.</summary>
		Medium,
		/// <summary>To be added.</summary>
		Long,
		/// <summary>To be added.</summary>
		Full,
	}

	/// <summary>An enumeration that can specify whether the <see cref="Foundation.NSDateFormatter" /> should behave as it did prior to OS x v10.4</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSDateFormatterBehavior : ulong {
		/// <summary>To be added.</summary>
		Default = 0,
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		Mode_10_0 = 1000,
		/// <summary>To be added.</summary>
		Mode_10_4 = 1040,
	}

	/// <summary>An enumeration whose values specify valid strategies for accepting <see cref="Foundation.NSHttpCookie" />s.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSHttpCookieAcceptPolicy : ulong {
		/// <summary>To be added.</summary>
		Always,
		/// <summary>To be added.</summary>
		Never,
		/// <summary>To be added.</summary>
		OnlyFromMainDocumentDomain,
	}

	/// <summary>An enumeration whose values specify calendrical units (e.g., seconds, months, eras).</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSCalendarUnit : ulong {
		/// <summary>To be added.</summary>
		Era = 2,
		/// <summary>To be added.</summary>
		Year = 4,
		/// <summary>To be added.</summary>
		Month = 8,
		/// <summary>To be added.</summary>
		Day = 16,
		/// <summary>To be added.</summary>
		Hour = 32,
		/// <summary>To be added.</summary>
		Minute = 64,
		/// <summary>To be added.</summary>
		Second = 128,
		/// <summary>Developers should not use this deprecated field. </summary>
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		Week = 256,
		/// <summary>To be added.</summary>
		Weekday = 512,
		/// <summary>To be added.</summary>
		WeekdayOrdinal = 1024,
		/// <summary>To be added.</summary>
		Quarter = 2048,

		/// <summary>To be added.</summary>
		WeekOfMonth = (1 << 12),
		/// <summary>To be added.</summary>
		WeekOfYear = (1 << 13),
		/// <summary>To be added.</summary>
		YearForWeakOfYear = (1 << 14),

		/// <summary>To be added.</summary>
		Nanosecond = (1 << 15),
		DayOfYear = (1 << 16),

		/// <summary>To be added.</summary>
		Calendar = (1 << 20),
		/// <summary>To be added.</summary>
		TimeZone = (1 << 21),

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		IsLeapMonth = (1 << 30),

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		IsRepeatedDay = (1UL << 31),
	}

	/// <summary>Flags that determine how NSData loads files.</summary>
	/// <remarks>
	///       <para>
	/// 	By default NSData will loads the contents of the file in memory
	/// 	by allocating a block of memory and then reading the contents of
	/// 	the file into it.
	///       </para>
	///       <para>
	/// 	The Mapped and MappedAlways parameter instruct NSData to use
	/// 	the kernel's interface to map the file into the process
	/// 	address space.  This has a few advantages: instead of
	/// 	allocating read/write memory for the process, that becomes
	/// 	real memory usage, the mapped versions map the file into
	/// 	memory which means that the data is loaded on demand instead
	/// 	of being loaded upfront.  This also allows the kernel to
	/// 	discard the data loaded from memory when the system is running
	/// 	low on memory. 
	///       </para>
	///     </remarks>
	[Flags]
	[Native]
	public enum NSDataReadingOptions : ulong {
		/// <summary>Use the kernel's virtual memory map to load the file, if possible. If sucessful, this replaces read/write memory that can be very expensive with discardable memory that is backed by a file.</summary>
		Mapped = 1 << 0,
		/// <summary>Notify the kernel that it should not try to cache the contents of this file in its buffer cache.</summary>
		Uncached = 1 << 1,
		/// <summary>Force NSData to try to use the kernel's mapping support to load the file.   If sucessful, this replaces read/write memory that can be very expensive with discardable memory that is backed by a file.</summary>
		MappedAlways = 1 << 3,
	}

	/// <summary>An enumeration of options to be used when writing <see cref="Foundation.NSData" /> objects.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSDataWritingOptions : ulong {
		/// <summary>To be added.</summary>
		Atomic = 1,

		/// <summary>To be added.</summary>
		WithoutOverwriting = 2,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		FileProtectionNone = 0x10000000,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		FileProtectionComplete = 0x20000000,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		FileProtectionMask = 0xf0000000,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		FileProtectionCompleteUnlessOpen = 0x30000000,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		FileProtectionCompleteUntilFirstUserAuthentication = 0x40000000,
		[iOS (17, 0), NoMac, MacCatalyst (17, 0), TV (17, 0)]
		FileProtectionCompleteWhenUserInactive = 0x50000000,
	}

	/// <param name="obj">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>A delegate that specifies the enumerator used by <see cref="Foundation.NSSet.Enumerate(Foundation.NSSetEnumerator)" />.</summary>
	/// <remarks>To be added.</remarks>
	public delegate void NSSetEnumerator (NSObject obj, ref bool stop);

	/// <summary>An enumeration of values that specify the priority of an operation, relative to others, in a <see cref="Foundation.NSOperationQueue" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSOperationQueuePriority : long {
		/// <summary>To be added.</summary>
		VeryLow = -8,
		/// <summary>To be added.</summary>
		Low = -4,
		/// <summary>To be added.</summary>
		Normal = 0,
		/// <summary>To be added.</summary>
		High = 4,
		/// <summary>To be added.</summary>
		VeryHigh = 8,
	}

	/// <summary>An enumeration of ways in which <see cref="Foundation.NSNotification" />s can be coalesced.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSNotificationCoalescing : ulong {
		/// <summary>To be added.</summary>
		NoCoalescing = 0,
		/// <summary>To be added.</summary>
		CoalescingOnName = 1,
		/// <summary>To be added.</summary>
		CoalescingOnSender = 2,
	}

	/// <summary>An enumeration of values that specify when a notification shouldbe posted.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSPostingStyle : ulong {
		/// <summary>To be added.</summary>
		PostWhenIdle = 1,
		/// <summary>To be added.</summary>
		PostASAP = 2,
		/// <summary>To be added.</summary>
		Now = 3,
	}

	/// <summary>Flags controling the search in NSData's Find method.</summary>
	/// <remarks>
	///     </remarks>
	[Flags]
	[Native]
	public enum NSDataSearchOptions : ulong {
		/// <summary>Starts search from the end, instead of the start.</summary>
		SearchBackwards = 1,
		/// <summary>Limits the search to the start (or end if SearchBackwards is specified)</summary>
		SearchAnchored = 2,
	}

	/// <summary>An enumeration of valid types for a <see cref="Foundation.NSExpression" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSExpressionType : ulong {
		/// <summary>To be added.</summary>
		ConstantValue = 0,
		/// <summary>To be added.</summary>
		EvaluatedObject,
		/// <summary>To be added.</summary>
		Variable,
		/// <summary>To be added.</summary>
		KeyPath,
		/// <summary>To be added.</summary>
		Function,
		/// <summary>To be added.</summary>
		UnionSet,
		/// <summary>To be added.</summary>
		IntersectSet,
		/// <summary>To be added.</summary>
		MinusSet,
		/// <summary>To be added.</summary>
		Subquery = 13,
		/// <summary>To be added.</summary>
		NSAggregate,
		/// <summary>To be added.</summary>
		AnyKey = 15,
		/// <summary>To be added.</summary>
		Block = 19,
		/// <summary>To be added.</summary>
		Conditional = 20,
	}

	/// <summary>Enumeration of various errors relating to Cocoa development.</summary>
	/// <remarks>To be added.</remarks>
	public enum NSCocoaError : int {
		/// <summary>To be added.</summary>
		None,

		/// <summary>To be added.</summary>
		FileNoSuchFile = 4,
		/// <summary>To be added.</summary>
		FileLocking = 255,
		/// <summary>To be added.</summary>
		FileReadUnknown = 256,
		/// <summary>To be added.</summary>
		FileReadNoPermission = 257,
		/// <summary>To be added.</summary>
		FileReadInvalidFileName = 258,
		/// <summary>To be added.</summary>
		FileReadCorruptFile = 259,
		/// <summary>To be added.</summary>
		FileReadNoSuchFile = 260,
		/// <summary>To be added.</summary>
		FileReadInapplicableStringEncoding = 261,
		/// <summary>To be added.</summary>
		FileReadUnsupportedScheme = 262,
		/// <summary>To be added.</summary>
		FileReadTooLarge = 263,
		/// <summary>To be added.</summary>
		FileReadUnknownStringEncoding = 264,
		/// <summary>To be added.</summary>
		FileWriteUnknown = 512,
		/// <summary>To be added.</summary>
		FileWriteNoPermission = 513,
		/// <summary>To be added.</summary>
		FileWriteInvalidFileName = 514,
		/// <summary>To be added.</summary>
		FileWriteFileExists = 516,
		/// <summary>To be added.</summary>
		FileWriteInapplicableStringEncoding = 517,
		/// <summary>To be added.</summary>
		FileWriteUnsupportedScheme = 518,
		/// <summary>To be added.</summary>
		FileWriteOutOfSpace = 640,
		/// <summary>To be added.</summary>
		FileWriteVolumeReadOnly = 642,

#if MONOMAC
		/// <summary>To be added.</summary>
		FileManagerUnmountUnknownError = 768,
		/// <summary>To be added.</summary>
		FileManagerUnmountBusyError = 769,
#endif

		/// <summary>To be added.</summary>
		KeyValueValidation = 1024,
		/// <summary>To be added.</summary>
		Formatting = 2048,
		/// <summary>To be added.</summary>
		UserCancelled = 3072,
		/// <summary>To be added.</summary>
		FeatureUnsupported = 3328,
		/// <summary>To be added.</summary>
		ExecutableNotLoadable = 3584,
		/// <summary>To be added.</summary>
		ExecutableArchitectureMismatch = 3585,
		/// <summary>To be added.</summary>
		ExecutableRuntimeMismatch = 3586,
		/// <summary>To be added.</summary>
		ExecutableLoad = 3587,
		/// <summary>To be added.</summary>
		ExecutableLink = 3588,
		/// <summary>To be added.</summary>
		FileErrorMinimum = 0,
		/// <summary>To be added.</summary>
		FileErrorMaximum = 1023,
		/// <summary>To be added.</summary>
		ValidationErrorMinimum = 1024,
		/// <summary>To be added.</summary>
		ValidationErrorMaximum = 2047,
		/// <summary>To be added.</summary>
		ExecutableErrorMinimum = 3584,
		/// <summary>To be added.</summary>
		ExecutableErrorMaximum = 3839,
		/// <summary>To be added.</summary>
		FormattingErrorMinimum = 2048,
		/// <summary>To be added.</summary>
		FormattingErrorMaximum = 2559,

		/// <summary>To be added.</summary>
		PropertyListReadCorrupt = 3840,
		/// <summary>To be added.</summary>
		PropertyListReadUnknownVersion = 3841,
		/// <summary>To be added.</summary>
		PropertyListReadStream = 3842,
		/// <summary>To be added.</summary>
		PropertyListWriteStream = 3851,
		/// <summary>To be added.</summary>
		PropertyListWriteInvalid = 3852,
		/// <summary>To be added.</summary>
		PropertyListErrorMinimum = 3840,
		/// <summary>To be added.</summary>
		PropertyListErrorMaximum = 4095,

		/// <summary>To be added.</summary>
		XpcConnectionInterrupted = 4097,
		/// <summary>To be added.</summary>
		XpcConnectionInvalid = 4099,
		/// <summary>To be added.</summary>
		XpcConnectionReplyInvalid = 4101,
		XpcConnectionCodeSigningRequirementFailure = 4102,
		/// <summary>To be added.</summary>
		XpcConnectionErrorMinimum = 4096,
		/// <summary>To be added.</summary>
		XpcConnectionErrorMaximum = 4224,

		/// <summary>To be added.</summary>
		UbiquitousFileUnavailable = 4353,
		/// <summary>To be added.</summary>
		UbiquitousFileNotUploadedDueToQuota = 4354,
		/// <summary>To be added.</summary>
		UbiquitousFileUbiquityServerNotAvailable = 4355,
		/// <summary>To be added.</summary>
		UbiquitousFileErrorMinimum = 4352,
		/// <summary>To be added.</summary>
		UbiquitousFileErrorMaximum = 4607,

		/// <summary>To be added.</summary>
		UserActivityHandoffFailedError = 4608,
		/// <summary>To be added.</summary>
		UserActivityConnectionUnavailableError = 4609,
		/// <summary>To be added.</summary>
		UserActivityRemoteApplicationTimedOutError = 4610,
		/// <summary>To be added.</summary>
		UserActivityHandoffUserInfoTooLargeError = 4611,

		/// <summary>To be added.</summary>
		UserActivityErrorMinimum = 4608,
		/// <summary>To be added.</summary>
		UserActivityErrorMaximum = 4863,

		/// <summary>To be added.</summary>
		CoderReadCorruptError = 4864,
		/// <summary>To be added.</summary>
		CoderValueNotFoundError = 4865,
		/// <summary>To be added.</summary>
		CoderInvalidValueError = 4866,
		/// <summary>To be added.</summary>
		CoderErrorMinimum = 4864,
		/// <summary>To be added.</summary>
		CoderErrorMaximum = 4991,

		/// <summary>To be added.</summary>
		BundleErrorMinimum = 4992,
		/// <summary>To be added.</summary>
		BundleErrorMaximum = 5119,

		/// <summary>To be added.</summary>
		BundleOnDemandResourceOutOfSpaceError = 4992,
		/// <summary>To be added.</summary>
		BundleOnDemandResourceExceededMaximumSizeError = 4993,
		/// <summary>To be added.</summary>
		BundleOnDemandResourceInvalidTagError = 4994,

		/// <summary>To be added.</summary>
		CloudSharingNetworkFailureError = 5120,
		/// <summary>To be added.</summary>
		CloudSharingQuotaExceededError = 5121,
		/// <summary>To be added.</summary>
		CloudSharingTooManyParticipantsError = 5122,
		/// <summary>To be added.</summary>
		CloudSharingConflictError = 5123,
		/// <summary>To be added.</summary>
		CloudSharingNoPermissionError = 5124,
		/// <summary>To be added.</summary>
		CloudSharingOtherError = 5375,
		/// <summary>To be added.</summary>
		CloudSharingErrorMinimum = 5120,
		/// <summary>To be added.</summary>
		CloudSharingErrorMaximum = 5375,

		CompressionFailedError = 5376,
		DecompressionFailedError = 5377,
		CompressionErrorMinimum = 5376,
		CompressionErrorMaximum = 5503,
	}

	// note: Make sure names are identical/consistent with CFNetworkErrors.*
	// they share the same values but there's more entries in CFNetworkErrors
	// so anything new probably already exists over there
	/// <summary>An enumeration of errors associated with creating or loading a <see cref="Foundation.NSUrl" />.</summary>
	/// <remarks>To be added.</remarks>
	public enum NSUrlError : int {
		/// <summary>To be added.</summary>
		Unknown = -1,

		/// <summary>To be added.</summary>
		BackgroundSessionRequiresSharedContainer = -995,
		/// <summary>To be added.</summary>
		BackgroundSessionInUseByAnotherProcess = -996,
		/// <summary>To be added.</summary>
		BackgroundSessionWasDisconnected = -997,

		/// <summary>To be added.</summary>
		Cancelled = -999,
		/// <summary>To be added.</summary>
		BadURL = -1000,
		/// <summary>To be added.</summary>
		TimedOut = -1001,
		/// <summary>To be added.</summary>
		UnsupportedURL = -1002,
		/// <summary>To be added.</summary>
		CannotFindHost = -1003,
		/// <summary>To be added.</summary>
		CannotConnectToHost = -1004,
		/// <summary>To be added.</summary>
		NetworkConnectionLost = -1005,
		/// <summary>To be added.</summary>
		DNSLookupFailed = -1006,
		/// <summary>To be added.</summary>
		HTTPTooManyRedirects = -1007,
		/// <summary>To be added.</summary>
		ResourceUnavailable = -1008,
		/// <summary>To be added.</summary>
		NotConnectedToInternet = -1009,
		/// <summary>To be added.</summary>
		RedirectToNonExistentLocation = -1010,
		/// <summary>To be added.</summary>
		BadServerResponse = -1011,
		/// <summary>To be added.</summary>
		UserCancelledAuthentication = -1012,
		/// <summary>To be added.</summary>
		UserAuthenticationRequired = -1013,
		/// <summary>To be added.</summary>
		ZeroByteResource = -1014,
		/// <summary>To be added.</summary>
		CannotDecodeRawData = -1015,
		/// <summary>To be added.</summary>
		CannotDecodeContentData = -1016,
		/// <summary>To be added.</summary>
		CannotParseResponse = -1017,
		/// <summary>To be added.</summary>
		InternationalRoamingOff = -1018,
		/// <summary>To be added.</summary>
		CallIsActive = -1019,
		/// <summary>To be added.</summary>
		DataNotAllowed = -1020,
		/// <summary>To be added.</summary>
		RequestBodyStreamExhausted = -1021,
		/// <summary>To be added.</summary>
		AppTransportSecurityRequiresSecureConnection = -1022,

		/// <summary>To be added.</summary>
		FileDoesNotExist = -1100,
		/// <summary>To be added.</summary>
		FileIsDirectory = -1101,
		/// <summary>To be added.</summary>
		NoPermissionsToReadFile = -1102,
		/// <summary>To be added.</summary>
		DataLengthExceedsMaximum = -1103,
		/// <summary>To be added.</summary>
		FileOutsideSafeArea = -1104,

		/// <summary>To be added.</summary>
		SecureConnectionFailed = -1200,
		/// <summary>To be added.</summary>
		ServerCertificateHasBadDate = -1201,
		/// <summary>To be added.</summary>
		ServerCertificateUntrusted = -1202,
		/// <summary>To be added.</summary>
		ServerCertificateHasUnknownRoot = -1203,
		/// <summary>To be added.</summary>
		ServerCertificateNotYetValid = -1204,
		/// <summary>To be added.</summary>
		ClientCertificateRejected = -1205,
		/// <summary>To be added.</summary>
		ClientCertificateRequired = -1206,

		/// <summary>To be added.</summary>
		CannotLoadFromNetwork = -2000,

		// Download and file I/O errors
		/// <summary>To be added.</summary>
		CannotCreateFile = -3000,
		/// <summary>To be added.</summary>
		CannotOpenFile = -3001,
		/// <summary>To be added.</summary>
		CannotCloseFile = -3002,
		/// <summary>To be added.</summary>
		CannotWriteToFile = -3003,
		/// <summary>To be added.</summary>
		CannotRemoveFile = -3004,
		/// <summary>To be added.</summary>
		CannotMoveFile = -3005,
		/// <summary>To be added.</summary>
		DownloadDecodingFailedMidStream = -3006,
		/// <summary>To be added.</summary>
		DownloadDecodingFailedToComplete = -3007,
	}

	/// <summary>An enumeration of values specifying options to be used with the <see cref="Foundation.NSObject.AddObserver(Foundation.NSObject,System.String,Foundation.NSKeyValueObservingOptions,System.IntPtr)" /> method.</summary>
	/// <remarks>
	///       <para>
	///       </para>
	///     </remarks>
	[Flags]
	[Native]
	public enum NSKeyValueObservingOptions : ulong {
		None = 0,
		/// <summary>Request that new values be delivered to the observer.</summary>
		New = 1,
		/// <summary>Requests that old values be delivered to the observer.</summary>
		Old = 2,
		/// <summary>Requests that both old and new values be delivered to the observer.</summary>
		OldNew = 3,
		/// <summary>Use this to send a notification on first use, without waiting for an actual change to take place.</summary>
		Initial = 4,
		/// <summary>If set, notifications are sent before and after the change to the property is made.   Otherwise the notifications are only sent after the property has been set.</summary>
		Prior = 8,
	}

	/// <summary>An enumeration indicating the type of change occurring in the <see cref="Foundation.NSObject.WillChangeValue(System.String)" /> and <see cref="Foundation.NSObject.DidChangeValue(System.String)" /> methods.</summary>
	/// <remarks>
	///       <para />
	///     </remarks>
	[Native]
	public enum NSKeyValueChange : ulong {
		/// <summary>The change is reported for setting a value in a property.</summary>
		Setting = 1,
		/// <summary>The change being reported is an insertion into a collection.</summary>
		Insertion,
		/// <summary>The change being reported is a removal from a collection.</summary>
		Removal,
		/// <summary>The change being reported is a set of changes in a collection.</summary>
		Replacement,
	}

	/// <summary>An enumeration of values indicating the operation being performed on a mutable key-value store.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSKeyValueSetMutationKind : ulong {
		/// <summary>To be added.</summary>
		UnionSet = 1,
		/// <summary>To be added.</summary>
		MinusSet,
		/// <summary>To be added.</summary>
		IntersectSet,
		/// <summary>To be added.</summary>
		SetSet,
	}

	/// <summary>An enumeration of valid options for use when enumerating over Blocks.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSEnumerationOptions : ulong {
		/// <summary>To be added.</summary>
		SortConcurrent = 1,
		/// <summary>To be added.</summary>
		Reverse = 2,
	}

	/// <summary>An enumeration of values that may be sent to <see cref="Foundation.NSStreamDelegate.HandleEvent(Foundation.NSStream,Foundation.NSStreamEvent)" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSStreamEvent : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		OpenCompleted = 1 << 0,
		/// <summary>To be added.</summary>
		HasBytesAvailable = 1 << 1,
		/// <summary>To be added.</summary>
		HasSpaceAvailable = 1 << 2,
		/// <summary>To be added.</summary>
		ErrorOccurred = 1 << 3,
		/// <summary>To be added.</summary>
		EndEncountered = 1 << 4,
	}

	/// <summary>An enumeration whose values specify how a <see cref="Foundation.NSComparisonPredicate" /> should apply to an n-to-many relationship.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSComparisonPredicateModifier : ulong {
		/// <summary>To be added.</summary>
		Direct,
		/// <summary>To be added.</summary>
		All,
		/// <summary>To be added.</summary>
		Any,
	}

	/// <summary>An enumeration of values that specify comparison types for use with <see cref="Foundation.NSComparisonPredicate" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSPredicateOperatorType : ulong {
		/// <summary>To be added.</summary>
		LessThan,
		/// <summary>To be added.</summary>
		LessThanOrEqualTo,
		/// <summary>To be added.</summary>
		GreaterThan,
		/// <summary>To be added.</summary>
		GreaterThanOrEqualTo,
		/// <summary>To be added.</summary>
		EqualTo,
		/// <summary>To be added.</summary>
		NotEqualTo,
		/// <summary>To be added.</summary>
		Matches,
		/// <summary>To be added.</summary>
		Like,
		/// <summary>To be added.</summary>
		BeginsWith,
		/// <summary>To be added.</summary>
		EndsWith,
		/// <summary>To be added.</summary>
		In,
		/// <summary>To be added.</summary>
		CustomSelector,
		/// <summary>To be added.</summary>
		Contains = 99,
		/// <summary>To be added.</summary>
		Between,
	}

	/// <summary>An enumeration whose values specify the type of string comparison to be used in a <see cref="Foundation.NSComparisonPredicate" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSComparisonPredicateOptions : ulong {
		/// <summary>To be added.</summary>
		None = 0x00,
		/// <summary>To be added.</summary>
		CaseInsensitive = 1 << 0,
		/// <summary>To be added.</summary>
		DiacriticInsensitive = 1 << 1,
		/// <summary>To be added.</summary>
		Normalized = 1 << 2,
	}

	/// <summary>An enumeration whose values specify the Boolean logical operator to be applied to a <see cref="Foundation.NSCompoundPredicate" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSCompoundPredicateType : ulong {
		/// <summary>To be added.</summary>
		Not,
		/// <summary>To be added.</summary>
		And,
		/// <summary>To be added.</summary>
		Or,
	}

	/// <summary>An enumeration of options for use when enumerating mounted volumes.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSVolumeEnumerationOptions : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		// skip                  = 1 << 0,
		/// <summary>To be added.</summary>
		SkipHiddenVolumes = 1 << 1,
		/// <summary>To be added.</summary>
		ProduceFileReferenceUrls = 1 << 2,
	}

	/// <summary>An enumeration of options for use with <see cref="Foundation.NSDirectoryEnumerator" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSDirectoryEnumerationOptions : ulong {
		None = 0,
		/// <summary>To be added.</summary>
		SkipsSubdirectoryDescendants = 1 << 0,
		/// <summary>To be added.</summary>
		SkipsPackageDescendants = 1 << 1,
		/// <summary>To be added.</summary>
		SkipsHiddenFiles = 1 << 2,
		[MacCatalyst (13, 1)]
		IncludesDirectoriesPostOrder = 1 << 3,
		[MacCatalyst (13, 1)]
		ProducesRelativePathUrls = 1 << 4,
	}

	/// <summary>An enumeration of options for use with <see cref="Foundation.NSFileManager.Replace(Foundation.NSUrl,Foundation.NSUrl,System.String,Foundation.NSFileManagerItemReplacementOptions,out Foundation.NSUrl,out Foundation.NSError)" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSFileManagerItemReplacementOptions : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		UsingNewMetadataOnly = 1 << 0,
		/// <summary>To be added.</summary>
		WithoutDeletingBackupItem = 1 << 1,
	}

	/// <summary>An enumeration of special directories for use with <see cref="Foundation.NSFileManager.GetUrls" />.</summary>
	/// <remarks>Some of these constants when used can return more than one value (for example AllApplicationsDirectory).</remarks>
	[Native]
	public enum NSSearchPathDirectory : ulong {
		/// <summary>Applications directory (/Applications).</summary>
		ApplicationDirectory = 1,
		/// <summary>Demo applications directory</summary>
		DemoApplicationDirectory,
		/// <summary>Deprecated, used to be /Developer/Applications.</summary>
		DeveloperApplicationDirectory,
		/// <summary>Directory for admin applications (Application/Utilities)</summary>
		AdminApplicationDirectory,
		/// <summary>Library directory contains documentation, configuration files and support files (Library)</summary>
		LibraryDirectory,
		/// <summary>Deprecated, used to be /Developer</summary>
		DeveloperDirectory,
		/// <summary>User directory (for all users, not the currently logged in user, /Users, /Network/Users for example)</summary>
		UserDirectory,
		/// <summary>Documentation directory</summary>
		DocumentationDirectory,
		/// <summary>Document directory (this is where an application can store its documents)</summary>
		DocumentDirectory,
		/// <summary>Directory for CoreServices (System/Library/CoreServices)</summary>
		CoreServiceDirectory,
		/// <summary>User autosave directory (Library/Autosave Information)</summary>
		AutosavedInformationDirectory = 11,
		/// <summary>The user’s desktop directory.</summary>
		DesktopDirectory = 12,
		/// <summary>Cache directory (Library/Caches)</summary>
		CachesDirectory = 13,
		/// <summary>Application support directory (Library/Application Support)</summary>
		ApplicationSupportDirectory = 14,
		/// <summary>Downloads directory (only available when the domain specified includes the User value)</summary>
		DownloadsDirectory = 15,
		/// <summary>Input methods directory (Library/Input Methods)</summary>
		InputMethodsDirectory = 16,
		/// <summary>User’s movies directory (~/Movies)</summary>
		MoviesDirectory = 17,
		/// <summary>User’s music directory (~/Music)</summary>
		MusicDirectory = 18,
		/// <summary>User’s picture directory (~/Pictures)</summary>
		PicturesDirectory = 19,
		/// <summary>Printer descriptions directory, the directory that contains Postcript Printer Description files (Library/Printers/PPDS)</summary>
		PrinterDescriptionDirectory = 20,
		/// <summary>Shared public directory, when enabled (~/Public)</summary>
		SharedPublicDirectory = 21,
		/// <summary>Preference Panes directory, the directory that contains the *.prefPane bundles, (Library/PreferencePanes)</summary>
		PreferencePanesDirectory = 22,
		/// <summary>User scripts directory (Library/Application Scripts/app)</summary>
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		ApplicationScriptsDirectory = 23,
		/// <summary>Item replacement directory, used for implementing safe-save features.</summary>
		ItemReplacementDirectory = 99,
		/// <summary>Combined directories where applications can appear.</summary>
		AllApplicationsDirectory = 100,
		/// <summary>Combined directories where resources can be appear.</summary>
		AllLibrariesDirectory = 101,
		/// <summary>Trash directory</summary>
		[NoTV]
		[MacCatalyst (13, 1)]
		TrashDirectory = 102,
	}

	/// <summary>An enumeration of values specifying search path domain constants for use with <see cref="Foundation.NSSearchPathDirectory" />.</summary>
	/// <remarks>The domain is used to specify the kind of directory you want to get from the <see cref="Foundation.NSFileManager.GetUrls(Foundation.NSSearchPathDirectory,Foundation.NSSearchPathDomain)" /> method.</remarks>
	[Flags]
	[NativeName ("NSSearchPathDomainMask")]
	[Native]
	public enum NSSearchPathDomain : ulong {
		/// <summary>Do not return anything.</summary>
		None = 0,
		/// <summary>Return user directories.</summary>
		User = 1 << 0,
		/// <summary>Return local directories.</summary>
		Local = 1 << 1,
		/// <summary>Return network directories.</summary>
		Network = 1 << 2,
		/// <summary>Return the system directories.</summary>
		System = 1 << 3,
		/// <summary>Return items for all domains.</summary>
		All = 0x0ffff,
	}

	/// <summary>An enumeration of values that specify rounding behaviors for <see cref="Foundation.NSDecimal" />s.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSRoundingMode : ulong {
		/// <summary>To be added.</summary>
		Plain,
		/// <summary>To be added.</summary>
		Down,
		/// <summary>To be added.</summary>
		Up,
		/// <summary>To be added.</summary>
		Bankers,
	}

	/// <summary>An enumeration whose values indicate a specific calculation error (e.g., underflow, division by zero, loss of precision).</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSCalculationError : ulong {
		/// <summary>To be added.</summary>
		None,
		/// <summary>To be added.</summary>
		PrecisionLoss,
		/// <summary>To be added.</summary>
		Underflow,
		/// <summary>To be added.</summary>
		Overflow,
		/// <summary>To be added.</summary>
		DivideByZero,
	}

	/// <summary>An enumeration of options for use when drawing strings.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSStringDrawingOptions : ulong {
		/// <summary>To be added.</summary>
		UsesLineFragmentOrigin = (1 << 0),
		/// <summary>To be added.</summary>
		UsesFontLeading = (1 << 1),
		/// <summary>This value is OSX-specific (not available in iOS)</summary>
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		DisableScreenFontSubstitution = (1 << 2),
		/// <summary>To be added.</summary>
		UsesDeviceMetrics = (1 << 3),
		/// <summary>This value is OSX-specific (not available in iOS)</summary>
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		OneShot = (1 << 4),
		/// <summary>To be added.</summary>
		TruncatesLastVisibleLine = (1 << 5),

		[iOS (26, 0), TV (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		OptionsResolvesNaturalAlignmentWithBaseWritingDirection = (1L << 9),
	}

	/// <summary>An enumeration of formats that can be used with numbers.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSNumberFormatterStyle : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		Decimal = 1,
		/// <summary>To be added.</summary>
		Currency = 2,
		/// <summary>To be added.</summary>
		Percent = 3,
		/// <summary>To be added.</summary>
		Scientific = 4,
		/// <summary>To be added.</summary>
		SpellOut = 5,
		[MacCatalyst (13, 1)]
		OrdinalStyle = 6,
		[MacCatalyst (13, 1)]
		CurrencyIsoCodeStyle = 8,
		[MacCatalyst (13, 1)]
		CurrencyPluralStyle = 9,
		[MacCatalyst (13, 1)]
		CurrencyAccountingStyle = 10,
	}

	/// <summary>An enumeration whose values specify whether the number formatter should behave as it did before OS X v10.4</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSNumberFormatterBehavior : ulong {
		/// <summary>To be added.</summary>
		Default = 0,
		/// <summary>To be added.</summary>
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		Version_10_0 = 1000,
		/// <summary>To be added.</summary>
		Version_10_4 = 1040,
	}

	/// <summary>An enumeration whose values indicates where padding should be applied to numbers.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSNumberFormatterPadPosition : ulong {
		/// <summary>To be added.</summary>
		BeforePrefix,
		/// <summary>To be added.</summary>
		AfterPrefix,
		/// <summary>To be added.</summary>
		BeforeSuffix,
		/// <summary>To be added.</summary>
		AfterSuffix,
	}

	/// <summary>An enumeration of rounding modes that can be applied to numbers.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSNumberFormatterRoundingMode : ulong {
		/// <summary>To be added.</summary>
		Ceiling,
		/// <summary>To be added.</summary>
		Floor,
		/// <summary>To be added.</summary>
		Down,
		/// <summary>To be added.</summary>
		Up,
		/// <summary>To be added.</summary>
		HalfEven,
		/// <summary>To be added.</summary>
		HalfDown,
		/// <summary>To be added.</summary>
		HalfUp,
	}

	/// <summary>Allows the application developer to specify that the old version of the file should be removed from the version store.</summary>
	[Flags]
	[Native]
	public enum NSFileVersionReplacingOptions : ulong {
		/// <summary>Moves the replacement file instead of copying it.</summary>
		ByMoving = 1 << 0,
	}

	/// <summary>Allows the application developer to specify that a new file version should be created by moving the source file.</summary>
	[Flags]
	[Native]
	public enum NSFileVersionAddingOptions : ulong {
		/// <summary>Moves the source file instead of copying it.</summary>
		ByMoving = 1 << 0,
	}

	/// <summary>An enumeration of options relating to reading the contents or attributes of a file or directory.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSFileCoordinatorReadingOptions : ulong {
		/// <summary>To be added.</summary>
		WithoutChanges = 1,
		/// <summary>To be added.</summary>
		ResolvesSymbolicLink = 1 << 1,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		ImmediatelyAvailableMetadataOnly = 1 << 2,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		ForUploading = 1 << 3,
	}

	/// <summary>An enumeration of options valid when changing the contents or attributes of a file or directory.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSFileCoordinatorWritingOptions : ulong {
		/// <summary>To be added.</summary>
		ForDeleting = 1,
		/// <summary>To be added.</summary>
		ForMoving = 2,
		/// <summary>To be added.</summary>
		ForMerging = 4,
		/// <summary>To be added.</summary>
		ForReplacing = 8,
		[MacCatalyst (13, 1)]
		ContentIndependentMetadataOnly = 16,
	}

	/// <summary>An enumeration of options for use with <see cref="Foundation.NSLinguisticTagger" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSLinguisticTaggerOptions : ulong {
		/// <summary>To be added.</summary>
		OmitWords = 1,
		/// <summary>To be added.</summary>
		OmitPunctuation = 2,
		/// <summary>To be added.</summary>
		OmitWhitespace = 4,
		/// <summary>To be added.</summary>
		OmitOther = 8,
		/// <summary>To be added.</summary>
		JoinNames = 16,
	}

	/// <summary>An enumeration of valid reasons for modifying the iCloud ubiquitous key store.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSUbiquitousKeyValueStoreChangeReason : long {
		/// <summary>To be added.</summary>
		ServerChange,
		/// <summary>To be added.</summary>
		InitialSyncChange,
		/// <summary>To be added.</summary>
		QuotaViolationChange,
		/// <summary>To be added.</summary>
		AccountChange,
	}

	/// <summary>Options for use when converting JSON data to instances of Foundation types.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSJsonReadingOptions : ulong {
		/// <summary>To be added.</summary>
		MutableContainers = 1,
		/// <summary>To be added.</summary>
		MutableLeaves = 2,
		FragmentsAllowed = 4,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		Json5Allowed = 8,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		TopLevelDictionaryAssumed = 16,
	}

	/// <summary>An enumeration specifying printing options (compact vs. pretty-printed) for JSON data.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSJsonWritingOptions : ulong {
		/// <summary>To be added.</summary>
		PrettyPrinted = 1,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		SortedKeys = (1 << 1),
		[MacCatalyst (13, 1)]
		FragmentsAllowed = (1 << 2),
		[MacCatalyst (13, 1)]
		WithoutEscapingSlashes = (1 << 3),
	}

	/// <summary>An enumeration of values that specify the direction of text for a language.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSLocaleLanguageDirection : ulong {
		/// <summary>To be added.</summary>
		Unknown,
		/// <summary>To be added.</summary>
		LeftToRight,
		/// <summary>To be added.</summary>
		RightToLeft,
		/// <summary>To be added.</summary>
		TopToBottom,
		/// <summary>To be added.</summary>
		BottomToTop,
	}

	/// <summary>An enumeration of values used by alignment functions.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	public enum NSAlignmentOptions : long {
		/// <summary>To be added.</summary>
		MinXInward = 1 << 0,
		/// <summary>To be added.</summary>
		MinYInward = 1 << 1,
		/// <summary>To be added.</summary>
		MaxXInward = 1 << 2,
		/// <summary>To be added.</summary>
		MaxYInward = 1 << 3,
		/// <summary>To be added.</summary>
		WidthInward = 1 << 4,
		/// <summary>To be added.</summary>
		HeightInward = 1 << 5,

		/// <summary>To be added.</summary>
		MinXOutward = 1 << 8,
		/// <summary>To be added.</summary>
		MinYOutward = 1 << 9,
		/// <summary>To be added.</summary>
		MaxXOutward = 1 << 10,
		/// <summary>To be added.</summary>
		MaxYOutward = 1 << 11,
		/// <summary>To be added.</summary>
		WidthOutward = 1 << 12,
		/// <summary>To be added.</summary>
		HeightOutward = 1 << 13,

		/// <summary>To be added.</summary>
		MinXNearest = 1 << 16,
		/// <summary>To be added.</summary>
		MinYNearest = 1 << 17,
		/// <summary>To be added.</summary>
		MaxXNearest = 1 << 18,
		/// <summary>To be added.</summary>
		MaxYNearest = 1 << 19,
		/// <summary>To be added.</summary>
		WidthNearest = 1 << 20,
		/// <summary>To be added.</summary>
		HeightNearest = 1 << 21,

		/// <summary>To be added.</summary>
		RectFlipped = unchecked((long) (1UL << 63)),

		/// <summary>To be added.</summary>
		AllEdgesInward = MinXInward | MaxXInward | MinYInward | MaxYInward,
		/// <summary>To be added.</summary>
		AllEdgesOutward = MinXOutward | MaxXOutward | MinYOutward | MaxYOutward,
		/// <summary>To be added.</summary>
		AllEdgesNearest = MinXNearest | MaxXNearest | MinYNearest | MaxYNearest,
	}

	/// <summary>An enumeration of options to be used when reading a file-system node.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSFileWrapperReadingOptions : ulong {
		/// <summary>To be added.</summary>
		Immediate = 1 << 0,
		/// <summary>To be added.</summary>
		WithoutMapping = 1 << 1,
	}

	/// <summary>An enumeration of options to be used when writing a file-system node.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSFileWrapperWritingOptions : ulong {
		/// <summary>To be added.</summary>
		Atomic = 1 << 0,
		/// <summary>To be added.</summary>
		WithNameUpdating = 1 << 1,
	}

	/// <summary>An enumeration whose values specify the options to be used in the <see cref="Foundation.NSAttributedString.EnumerateAttribute(Foundation.NSString,Foundation.NSRange,Foundation.NSAttributedStringEnumeration,Foundation.NSAttributedStringCallback)" /> and <see cref="Foundation.NSAttributedString.EnumerateAttributes(Foundation.NSRange,Foundation.NSAttributedStringEnumeration,Foundation.NSAttributedRangeCallback)" /> methods.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native ("NSAttributedStringEnumerationOptions")]
	public enum NSAttributedStringEnumeration : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		Reverse = 1 << 1,
		/// <summary>To be added.</summary>
		LongestEffectiveRangeNotRequired = 1 << 20,
	}

	// macOS has defined this in AppKit as well, but starting with .NET we're going
	// to use this one only.
	/// <summary>An enumeration of valid styles for underlines or strikethroughs.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSUnderlineStyle : long {
		/// <summary>To be added.</summary>
		None = 0x00,
		/// <summary>To be added.</summary>
		Single = 0x01,
		/// <summary>To be added.</summary>
		Thick = 0x02,
		/// <summary>To be added.</summary>
		Double = 0x09,
		/// <summary>To be added.</summary>
		PatternSolid = 0x0000,
		/// <summary>To be added.</summary>
		PatternDot = 0x0100,
		/// <summary>To be added.</summary>
		PatternDash = 0x0200,
		/// <summary>To be added.</summary>
		PatternDashDot = 0x0300,
		/// <summary>To be added.</summary>
		PatternDashDotDot = 0x0400,
		/// <summary>To be added.</summary>
		ByWord = 0x8000,
	}

	// There's an AppKit.NSWritingDirection, which is deprecated.
	// There's also an UIKit.UITextWritingDirection, which is deprecated too.
	// This is the enum we should be using.
	// See https://github.com/dotnet/macios/issues/6573
	/// <summary>An enumeration of valid writing directions.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSWritingDirection : long {
		/// <summary>To be added.</summary>
		Natural = -1,
		/// <summary>To be added.</summary>
		LeftToRight = 0,
		/// <summary>To be added.</summary>
		RightToLeft = 1,
	}

	/// <summary>An enumeration whose values specify the units to be displayed by a <see cref="Foundation.NSByteCountFormatter" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSByteCountFormatterUnits : ulong {
		/// <summary>To be added.</summary>
		UseDefault = 0,
		/// <summary>To be added.</summary>
		UseBytes = 1 << 0,
		/// <summary>To be added.</summary>
		UseKB = 1 << 1,
		/// <summary>To be added.</summary>
		UseMB = 1 << 2,
		/// <summary>To be added.</summary>
		UseGB = 1 << 3,
		/// <summary>To be added.</summary>
		UseTB = 1 << 4,
		/// <summary>To be added.</summary>
		UsePB = 1 << 5,
		/// <summary>To be added.</summary>
		UseEB = 1 << 6,
		/// <summary>To be added.</summary>
		UseZB = 1 << 7,
		/// <summary>To be added.</summary>
		UseYBOrHigher = 0x0FF << 8,
		/// <summary>To be added.</summary>
		UseAll = 0x0FFFF,
	}

	/// <summary>An enumeration whose values specify how byte units are calculated (e.g., if "KB" indicates 1000 or 1024 bytes).</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSByteCountFormatterCountStyle : long {
		/// <summary>To be added.</summary>
		File,
		/// <summary>To be added.</summary>
		Memory,
		/// <summary>To be added.</summary>
		Decimal,
		/// <summary>To be added.</summary>
		Binary,
	}

	/// <summary>An enumeration of options ot be used when creating a bookmark.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSUrlBookmarkCreationOptions : ulong {
		/// <summary>To be added.</summary>
		PreferFileIDResolution = 1 << 8,
		/// <summary>To be added.</summary>
		MinimalBookmark = 1 << 9,
		/// <summary>To be added.</summary>
		SuitableForBookmarkFile = 1 << 10,
		/// <summary>To be added.</summary>
		[NoiOS, NoTV]
		[NoMacCatalyst]
		WithSecurityScope = 1 << 11,
		/// <summary>To be added.</summary>
		[NoiOS, NoTV]
		[NoMacCatalyst]
		SecurityScopeAllowOnlyReadAccess = 1 << 12,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		CreationWithoutImplicitSecurityScope = 1 << 29,
	}

	/// <summary>An enumeration of options to be used when creating an NSUrl by resolving a bookmark.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSUrlBookmarkResolutionOptions : ulong {
		/// <summary>To be added.</summary>
		WithoutUI = 1 << 8,
		/// <summary>To be added.</summary>
		WithoutMounting = 1 << 9,
		/// <summary>To be added.</summary>
		[NoiOS, NoTV]
		[NoMacCatalyst]
		WithSecurityScope = 1 << 10,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		WithoutImplicitStartAccessing = 1 << 15,
	}

	/// <summary>An enumeration that defines the valid ligature types of an <see cref="Foundation.NSAttributedString" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSLigatureType : long {
		/// <summary>To be added.</summary>
		None,
		/// <summary>To be added.</summary>
		Default,
		/// <summary>To be added.</summary>
		All,
	}

	/// <summary>A flagging enumeration whose values specify options in calls to <see cref="NSCalendar.Components" /> and <see cref="Foundation.NSCalendar.DateByAddingComponents(Foundation.NSDateComponents,Foundation.NSDate,Foundation.NSCalendarOptions)" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSCalendarOptions : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		WrapCalendarComponents = 1 << 0,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		MatchStrictly = 1 << 1,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		SearchBackwards = 1 << 2,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		MatchPreviousTimePreservingSmallerUnits = 1 << 8,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		MatchNextTimePreservingSmallerUnits = 1 << 9,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		MatchNextTime = 1 << 10,

		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		MatchFirst = 1 << 12,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		MatchLast = 1 << 13,
	}

	/// <summary>Network service types for <see cref="Foundation.NSUrlRequest" />.</summary>
	/// <remarks>
	///     </remarks>
	[Native]
	public enum NSUrlRequestNetworkServiceType : ulong {
		/// <summary>Default traffic</summary>
		Default,
		/// <summary>Voice over IP traffic.</summary>
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'PushKit' framework instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'PushKit' framework instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'PushKit' framework instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PushKit' framework instead.")]
		VoIP,
		/// <summary>Video traffic.</summary>
		Video,
		/// <summary>Background traffic.</summary>
		Background,
		/// <summary>Voice traffic.</summary>
		Voice,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		ResponsiveData = 6,
		[MacCatalyst (13, 1)]
		AVStreaming = 8,
		[MacCatalyst (13, 1)]
		ResponsiveAV = 9,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		CallSignaling = 11,
	}

	/// <summary>A flagging enumeration that specifies sorting options in calls to <see cref="NSMutableOrderedSet.Sort" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSSortOptions : ulong {
		/// <summary>To be added.</summary>
		Concurrent = 1 << 0,
		/// <summary>To be added.</summary>
		Stable = 1 << 4,
	}

	/// <summary>A flagging enumeration that can be used with the <see cref="Foundation.NSData(Foundation.NSData,Foundation.NSDataBase64DecodingOptions)" /> constructor.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSDataBase64DecodingOptions : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		IgnoreUnknownCharacters = 1,
	}

	/// <summary>A flagging enumeration that can be used to specify options for <see cref="Foundation.NSData.GetBase64EncodedData(Foundation.NSDataBase64EncodingOptions)" /> and <see cref="Foundation.NSData.GetBase64EncodedString(Foundation.NSDataBase64EncodingOptions)" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum NSDataBase64EncodingOptions : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		SixtyFourCharacterLineLength = 1,
		/// <summary>To be added.</summary>
		SeventySixCharacterLineLength = 1 << 1,
		/// <summary>To be added.</summary>
		EndLineWithCarriageReturn = 1 << 4,
		/// <summary>To be added.</summary>
		EndLineWithLineFeed = 1 << 5,
	}

	/// <summary>An enumeration whose values specify the state of an authorization challenge.</summary>
	/// <remarks>
	///       <para>Instances of this type are passed as an argument to the completion handler callback in <see cref="Foundation.NSUrlSessionDelegate.DidReceiveChallenge(Foundation.NSUrlSession,Foundation.NSUrlAuthenticationChallenge,System.Action{Foundation.NSUrlSessionAuthChallengeDisposition,Foundation.NSUrlCredential})" /> and its overrides.</para>
	///     </remarks>
	[Native]
	public enum NSUrlSessionAuthChallengeDisposition : long {
		/// <summary>To be added.</summary>
		UseCredential = 0,
		/// <summary>To be added.</summary>
		PerformDefaultHandling = 1,
		/// <summary>To be added.</summary>
		CancelAuthenticationChallenge = 2,
		/// <summary>To be added.</summary>
		RejectProtectionSpace = 3,
	}

	/// <summary>An enumeration whose values specify the state of a <see cref="NSUrlSessionTask" />.</summary>
	[Native]
	public enum NSUrlSessionTaskState : long {
		/// <summary>To be added.</summary>
		Running = 0,
		/// <summary>To be added.</summary>
		Suspended = 1,
		/// <summary>To be added.</summary>
		Canceling = 2,
		/// <summary>To be added.</summary>
		Completed = 3,
	}

	/// <summary>An enumeration whose values specify the state of a response.</summary>
	/// <remarks>An instance of this class is passed as an argument to the completion handler of the <see cref="Foundation.NSUrlSessionDataDelegate.DidReceiveResponse(Foundation.NSUrlSession,Foundation.NSUrlSessionDataTask,Foundation.NSUrlResponse,System.Action{Foundation.NSUrlSessionResponseDisposition})" /> method.</remarks>
	[Native]
	public enum NSUrlSessionResponseDisposition : long {
		/// <summary>To be added.</summary>
		Cancel = 0,
		/// <summary>To be added.</summary>
		Allow = 1,
		/// <summary>To be added.</summary>
		BecomeDownload = 2,
		/// <summary>To be added.</summary>
		BecomeStream = 3,
	}

	/// <summary>An enumeration whose values specify why a data transfer was cancelled.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSUrlErrorCancelledReason : long {
		/// <summary>To be added.</summary>
		UserForceQuitApplication,
		/// <summary>To be added.</summary>
		BackgroundUpdatesDisabled,
		/// <summary>To be added.</summary>
		InsufficientSystemResources,
	}

	/// <summary>A flagging enumeration whose values can be used with <see cref="Foundation.NSProcessInfo.BeginActivity(Foundation.NSActivityOptions,System.String)" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	public enum NSActivityOptions : ulong {
		/// <summary>To be added.</summary>
		IdleDisplaySleepDisabled = 1UL << 40,
		/// <summary>To be added.</summary>
		IdleSystemSleepDisabled = 1UL << 20,
		/// <summary>To be added.</summary>
		SuddenTerminationDisabled = 1UL << 14,
		/// <summary>To be added.</summary>
		AutomaticTerminationDisabled = 1UL << 15,
		AnimationTrackingEnabled = 1uL << 45,
		TrackingEnabled = 1uL << 46,
		UserInteractive = (UserInitiated | LatencyCritical),
		/// <summary>To be added.</summary>
		UserInitiated = 0x00FFFFFFUL | IdleSystemSleepDisabled,
		/// <summary>To be added.</summary>
		Background = 0x000000ffUL,
		/// <summary>To be added.</summary>
		LatencyCritical = 0xFF00000000UL,
		InitiatedAllowingIdleSystemSleep = UserInitiated & ~IdleSystemSleepDisabled,
	}

	/// <summary>Specifies styles for time-zone names.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSTimeZoneNameStyle : long {
		/// <summary>To be added.</summary>
		Standard,
		/// <summary>To be added.</summary>
		ShortStandard,
		/// <summary>To be added.</summary>
		DaylightSaving,
		/// <summary>To be added.</summary>
		ShortDaylightSaving,
		/// <summary>To be added.</summary>
		Generic,
		/// <summary>To be added.</summary>
		ShortGeneric,
	}

	/// <summary>Enumerates errors relating to <see cref="Foundation.NSItemProvider" /> methods.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSItemProviderErrorCode : long {
		/// <summary>To be added.</summary>
		Unknown = -1,
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		ItemUnavailable = -1000,
		/// <summary>To be added.</summary>
		UnexpectedValueClass = -1100,
		/// <summary>To be added.</summary>
		UnavailableCoercion = -1200,
	}

	/// <summary>Enumerates <see cref="Foundation.NSDateComponentsFormatter" /> output styles.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum NSDateComponentsFormatterUnitsStyle : long {
		/// <summary>To be added.</summary>
		Positional = 0,
		/// <summary>To be added.</summary>
		Abbreviated,
		/// <summary>To be added.</summary>
		Short,
		/// <summary>To be added.</summary>
		Full,
		/// <summary>To be added.</summary>
		SpellOut,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Brief,
	}

	/// <summary>Enumerates how zero values should be dealt with by a <see cref="Foundation.NSDateComponentsFormatter" />.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	[MacCatalyst (13, 1)]
	public enum NSDateComponentsFormatterZeroFormattingBehavior : ulong {
		/// <summary>To be added.</summary>
		None = (0),
		/// <summary>To be added.</summary>
		Default = (1 << 0),
		/// <summary>To be added.</summary>
		DropLeading = (1 << 1),
		/// <summary>To be added.</summary>
		DropMiddle = (1 << 2),
		/// <summary>To be added.</summary>
		DropTrailing = (1 << 3),
		/// <summary>To be added.</summary>
		DropAll = (DropLeading | DropMiddle | DropTrailing),
		/// <summary>To be added.</summary>
		Pad = (1 << 16),
	}

	/// <summary>Enumerates the position of the data being formatted. Used with <see cref="Foundation.NSByteCountFormatter" /> and <see cref="Foundation.NSDateComponentsFormatter" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[MacCatalyst (13, 1)]
	public enum NSFormattingContext : long {
		/// <summary>To be added.</summary>
		Unknown = 0,
		/// <summary>To be added.</summary>
		Dynamic = 1,
		/// <summary>To be added.</summary>
		Standalone = 2,
		/// <summary>To be added.</summary>
		ListItem = 3,
		/// <summary>To be added.</summary>
		BeginningOfSentence = 4,
		/// <summary>To be added.</summary>
		MiddleOfSentence = 5,
	}

	/// <summary>Enumerates the output styles of a <see cref="Foundation.NSDateIntervalFormatter" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSDateIntervalFormatterStyle : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		Short = 1,
		/// <summary>To be added.</summary>
		Medium = 2,
		/// <summary>To be added.</summary>
		Long = 3,
		/// <summary>To be added.</summary>
		Full = 4,
	}

	/// <summary>The unit to be used by a <see cref="Foundation.NSEnergyFormatter" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSEnergyFormatterUnit : long {
		/// <summary>To be added.</summary>
		Joule = 11,
		/// <summary>To be added.</summary>
		Kilojoule = 14,
		/// <summary>To be added.</summary>
		Calorie = (7 << 8) + 1,
		/// <summary>To be added.</summary>
		Kilocalorie = (7 << 8) + 2,
	}

	/// <summary>Enumerates the style (desired length) of an <see cref="Foundation.NSLengthFormatter" />, <see cref="Foundation.NSEnergyFormatter" />, or <see cref="Foundation.NSMassFormatter" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSFormattingUnitStyle : long {
		/// <summary>To be added.</summary>
		Short = 1,
		/// <summary>To be added.</summary>
		Medium,
		/// <summary>To be added.</summary>
		Long,
	}

	/// <summary>Enumerates mass units (lb, kg, stone).</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSMassFormatterUnit : long {
		/// <summary>To be added.</summary>
		Gram = 11,
		/// <summary>To be added.</summary>
		Kilogram = 14,
		/// <summary>To be added.</summary>
		Ounce = (6 << 8) + 1,
		/// <summary>To be added.</summary>
		Pound = (6 << 8) + 2,
		/// <summary>To be added.</summary>
		Stone = (6 << 8) + 3,
	}

	/// <summary>Enumerates units of length (foot, meter, etc.) for use with <see cref="Foundation.NSLengthFormatter" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSLengthFormatterUnit : long {
		/// <summary>To be added.</summary>
		Millimeter = 8,
		/// <summary>To be added.</summary>
		Centimeter = 9,
		/// <summary>To be added.</summary>
		Meter = 11,
		/// <summary>To be added.</summary>
		Kilometer = 14,
		/// <summary>To be added.</summary>
		Inch = (5 << 8) + 1,
		/// <summary>To be added.</summary>
		Foot = (5 << 8) + 2,
		/// <summary>To be added.</summary>
		Yard = (5 << 8) + 3,
		/// <summary>To be added.</summary>
		Mile = (5 << 8) + 4,
	}

	/// <summary>Enumerates QoS values for use with <see cref="Foundation.NSOperation" /> objects and <see cref="Foundation.NSOperationQueue" /> objects.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSQualityOfService : long {
		/// <summary>To be added.</summary>
		UserInteractive = 33,
		/// <summary>To be added.</summary>
		UserInitiated = 25,
		/// <summary>To be added.</summary>
		Utility = 17,
		/// <summary>To be added.</summary>
		Background = 9,
		/// <summary>To be added.</summary>
		Default = -1,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSProcessInfoThermalState : long {
		/// <summary>To be added.</summary>
		Nominal,
		/// <summary>To be added.</summary>
		Fair,
		/// <summary>To be added.</summary>
		Serious,
		/// <summary>To be added.</summary>
		Critical,
	}

	/// <summary>Defines constants defining the relationship between a directory and an item.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum NSUrlRelationship : long {
		/// <summary>To be added.</summary>
		Contains,
		/// <summary>To be added.</summary>
		Same,
		/// <summary>To be added.</summary>
		Other,
	}

	// NSTextCheckingResult.h:typedef NS_OPTIONS(uint64_t, NSTextCheckingType)
	/// <summary>Enumerates available predefined regular expressions for checking text.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	public enum NSTextCheckingType : ulong {
		/// <summary>To be added.</summary>
		Orthography = 1 << 0,
		/// <summary>To be added.</summary>
		Spelling = 1 << 1,
		/// <summary>To be added.</summary>
		Grammar = 1 << 2,
		/// <summary>To be added.</summary>
		Date = 1 << 3,
		/// <summary>To be added.</summary>
		Address = 1 << 4,
		/// <summary>To be added.</summary>
		Link = 1 << 5,
		/// <summary>To be added.</summary>
		Quote = 1 << 6,
		/// <summary>To be added.</summary>
		Dash = 1 << 7,
		/// <summary>To be added.</summary>
		Replacement = 1 << 8,
		/// <summary>To be added.</summary>
		Correction = 1 << 9,
		/// <summary>To be added.</summary>
		RegularExpression = 1 << 10,
		/// <summary>To be added.</summary>
		PhoneNumber = 1 << 11,
		/// <summary>To be added.</summary>
		TransitInformation = 1 << 12,
	}

	// NSTextCheckingResult.h:typedef uint64_t NSTextCheckingTypes;
	/// <summary>Enumerates available predefined classes of regular expressions for checking text.</summary>
	/// <remarks>To be added.</remarks>
	public enum NSTextCheckingTypes : ulong {
		/// <summary>To be added.</summary>
		AllSystemTypes = 0xffffffff,
		/// <summary>To be added.</summary>
		AllCustomTypes = 0xffffffff00000000,
		/// <summary>To be added.</summary>
		AllTypes = 0xffffffffffffffff,
	}

	/// <summary>Defines options for use with <see cref="Foundation.NSRegularExpression" /> objects.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[Flags]
	public enum NSRegularExpressionOptions : ulong {
		/// <summary>To be added.</summary>
		CaseInsensitive = 1 << 0,
		/// <summary>To be added.</summary>
		AllowCommentsAndWhitespace = 1 << 1,
		/// <summary>To be added.</summary>
		IgnoreMetacharacters = 1 << 2,
		/// <summary>To be added.</summary>
		DotMatchesLineSeparators = 1 << 3,
		/// <summary>To be added.</summary>
		AnchorsMatchLines = 1 << 4,
		UseUnixLineSeparators = 1 << 5,
		/// <summary>To be added.</summary>
		UseUnicodeWordBoundaries = 1 << 6,
	}

	/// <summary>Enumerates options for use with <see cref="Foundation.NSRegularExpression" /> regular expression objects.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[Flags]
	public enum NSMatchingOptions : ulong {
		/// <summary>To be added.</summary>
		ReportProgress = 1 << 0,
		/// <summary>To be added.</summary>
		ReportCompletion = 1 << 1,
		/// <summary>To be added.</summary>
		Anchored = 1 << 2,
		/// <summary>To be added.</summary>
		WithTransparentBounds = 1 << 3,
		/// <summary>To be added.</summary>
		WithoutAnchoringBounds = 1 << 4,
	}

	/// <summary>Enumerates flags for use with the <see cref="Foundation.NSMatchEnumerator" /> delegate.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	[Flags]
	public enum NSMatchingFlags : ulong {
		/// <summary>To be added.</summary>
		Progress = 1 << 0,
		/// <summary>To be added.</summary>
		Completed = 1 << 1,
		/// <summary>To be added.</summary>
		HitEnd = 1 << 2,
		/// <summary>To be added.</summary>
		RequiredEnd = 1 << 3,
		/// <summary>To be added.</summary>
		InternalError = 1 << 4,
	}

	/// <summary>Contains a constant that, if specified, indicates that the phonetic representation of a name should be formatted, rather than the name object's own components.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum NSPersonNameComponentsFormatterOptions : ulong {
		/// <summary>Value that indicates that the phonetic representation of a name should be formatted, rather than the name object's own components.</summary>
		Phonetic = (1 << 1),
	}

	/// <summary>Enumerates values that control the way that names are displayed.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSPersonNameComponentsFormatterStyle : long {
		/// <summary>Display a medium-length representation of the name. Equivalent to <c>Medium</c>.</summary>
		Default = 0,
		/// <summary>Display a shortened form of the name by abbreviating or omitting various, user-specified, components.</summary>
		Short,
		/// <summary>Display a medium-length representation of the name. Equivalent to <c>Default</c>.</summary>
		Medium,
		/// <summary>Use all available name components, except for the nickname, to display the name.</summary>
		Long,
		/// <summary>Display the most abbreviated form of the name.</summary>
		Abbreviated,
	}

	/// <summary>Enumerates the manner in which a <see cref="Foundation.NSCoder" /> fails. (See <see cref="Foundation.NSCoder.DecodingFailurePolicy" />)</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSDecodingFailurePolicy : long {
		/// <summary>To be added.</summary>
		RaiseException,
		/// <summary>To be added.</summary>
		SetErrorAndReturn,
	}

	/// <summary>A flagging enumeration of formatting options for use with <see cref="Foundation.NSIso8601DateFormatter" />.</summary>
	/// <remarks>
	///       <para>It is often easier to code a format by removing flags from <see cref="Foundation.NSIso8601DateFormatOptions.InternetDateTime" /> such as:</para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// fmt.FormatOptions = fmt.FormatOptions & ~NSIso8601DateFormatOptions.Day
	/// var s = fmt.StringFor(d); //"2016-08T23:15:36Z"          
	///           ]]></code>
	///       </example>
	///     </remarks>
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum NSIso8601DateFormatOptions : ulong {
		/// <summary>Includes the year in the string.</summary>
		Year = 1 << 0,
		/// <summary>Includes the month in the string.</summary>
		Month = 1 << 1,
		/// <summary>Includes the week of the year in the string (e.g., "W33").</summary>
		WeekOfYear = 1 << 2,
		/// <summary>Includes the day in the string.</summary>
		Day = 1 << 4,
		/// <summary>Includes the time in the string.</summary>
		Time = 1 << 5,
		/// <summary>Includes the timezone in the string.</summary>
		TimeZone = 1 << 6,
		/// <summary>Puts a space between the date and the time.</summary>
		SpaceBetweenDateAndTime = 1 << 7,
		/// <summary>Separates date components with a dash.</summary>
		DashSeparatorInDate = 1 << 8,
		/// <summary>Puts a colon between time components</summary>
		ColonSeparatorInTime = 1 << 9,
		/// <summary>Puts a colon in the time.</summary>
		ColonSeparatorInTimeZone = 1 << 10,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		FractionalSeconds = 1 << 11,
		/// <summary>Includes the date in the string.</summary>
		FullDate = Year | Month | Day | DashSeparatorInDate,
		/// <summary>Includes the time in the string.</summary>
		FullTime = Time | ColonSeparatorInTime | TimeZone | ColonSeparatorInTimeZone,
		/// <summary>The standard ISO-8601 format (<c>yyyy-MM-ddTHH:mm:ssZ</c>).</summary>
		InternetDateTime = FullDate | FullTime,
	}

	/// <summary>Enumerates the way a network resource might be loaded.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSUrlSessionTaskMetricsResourceFetchType : long {
		/// <summary>The manner of the resource loading is not known.</summary>
		Unknown,
		/// <summary>The resource was retrieved via a network connection.</summary>
		NetworkLoad,
		/// <summary>The resource was pushed from a server.</summary>
		[Deprecated (PlatformName.iOS, 18, 4, message: "Not supported in iOS 17+.")] // message mentions one OS version and the actual deprecation OS version don't match, but that's what the headers do.
		[Deprecated (PlatformName.TvOS, 18, 4, message: "Not supported in tvOS 17+.")] // message mentions one OS version and the actual deprecation OS version don't match, but that's what the headers do.
		[Deprecated (PlatformName.MacCatalyst, 18, 4, message: "Not supported in Mac Catalyst 17+.")] // message mentions one OS version and the actual deprecation OS version don't match, but that's what the headers do.
		[Deprecated (PlatformName.MacOSX, 15, 4, message: "Not supported in macOS 14+.")] // message mentions one OS version and the actual deprecation OS version don't match, but that's what the headers do.
		ServerPush,
		/// <summary>The resource was retrieved from a local cache.</summary>
		LocalCache,
	}

	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum NSMeasurementFormatterUnitOptions : ulong {
		/// <summary>To be added.</summary>
		ProvidedUnit = (1 << 0),
		/// <summary>To be added.</summary>
		NaturalScale = (1 << 1),
		/// <summary>To be added.</summary>
		TemperatureWithoutUnit = (1 << 2),
	}


	[MacCatalyst (13, 1)]
	[Native]
	public enum NSItemProviderRepresentationVisibility : long {
		/// <summary>To be added.</summary>
		All = 0,
		/// <summary>To be added.</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		Team = 1,
		/// <summary>To be added.</summary>
		[NoiOS, NoTV]
		[NoMacCatalyst]
		Group = 2,
		/// <summary>To be added.</summary>
		OwnProcess = 3,
	}

	/// <summary>Specifies how an item provider supplies a file.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSItemProviderFileOptions : long {
		/// <summary>Opens the file in place instead of copying it.</summary>
		OpenInPlace = 1,
	}

	/// <summary>Enumerate the linguistic units recognized by the <see cref="Foundation.NSLinguisticTagger" /> class.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum NSLinguisticTaggerUnit : long {
		/// <summary>To be added.</summary>
		Word,
		/// <summary>To be added.</summary>
		Sentence,
		/// <summary>To be added.</summary>
		Paragraph,
		/// <summary>To be added.</summary>
		Document,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSUrlSessionDelayedRequestDisposition : long {
		/// <summary>To be added.</summary>
		ContinueLoading = 0,
		/// <summary>To be added.</summary>
		UseNewRequest = 1,
		/// <summary>To be added.</summary>
		Cancel = 2,
	}

	[Native]
	public enum NSXpcConnectionOptions : ulong {
		Privileged = (1 << 12),
	}

	[MacCatalyst (13, 1)]
	public enum NSFileProtectionType {
		[Field ("NSFileProtectionComplete")]
		Complete,
		[Field ("NSFileProtectionCompleteUnlessOpen")]
		CompleteUnlessOpen,
		[Field ("NSFileProtectionCompleteUntilFirstUserAuthentication")]
		CompleteUntilFirstUserAuthentication,
		[Field ("NSFileProtectionNone")]
		None,
		[TV (17, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSFileProtectionCompleteWhenUserInactive")]
		CompleteWhenUserInactive,
	}
}
