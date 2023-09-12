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
using System;
using ObjCRuntime;

namespace Foundation {

	[Native]
	public enum NSStringEncoding : ulong {
		ASCIIStringEncoding = 1,
		NEXTSTEP = 2,
		JapaneseEUC = 3,
		UTF8 = 4,
		ISOLatin1 = 5,
		Symbol = 6,
		NonLossyASCII = 7,
		ShiftJIS = 8,
		ISOLatin2 = 9,
		Unicode = 10,
		WindowsCP1251 = 11,
		WindowsCP1252 = 12,
		WindowsCP1253 = 13,
		WindowsCP1254 = 14,
		WindowsCP1250 = 15,
		ISO2022JP = 21,
		MacOSRoman = 30,
		UTF16BigEndian = 0x90000100,
		UTF16LittleEndian = 0x94000100,
		UTF32 = 0x8c000100,
		UTF32BigEndian = 0x98000100,
		UTF32LittleEndian = 0x9c000100,
	};

	[Native]
	public enum NSStringCompareOptions : ulong {
		CaseInsensitiveSearch = 1,
		LiteralSearch = 2,
		BackwardsSearch = 4,
		AnchoredSearch = 8,
		NumericSearch = 64,
		DiacriticInsensitiveSearch = 128,
		WidthInsensitiveSearch = 256,
		ForcedOrderingSearch = 512,
		RegularExpressionSearch = 1024,
	}

	[Native]
	public enum NSUrlCredentialPersistence : ulong {
		None,
		ForSession,
		Permanent,
		Synchronizable
	}

#if MONOMAC || !XAMCORE_3_0

#if !NET
	[Native]
	public enum NSBundleExecutableArchitecture : long {
#else
	[NoiOS][NoTV][NoMacCatalyst]
	public enum NSBundleExecutableArchitecture {
#endif
		I386 = 0x00000007,
		PPC = 0x00000012,
		X86_64 = 0x01000007,
		PPC64 = 0x01000012,
		[Mac (11, 0)]
#if !XAMCORE_3_0
		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
#endif
		ARM64 = 0x0100000c,
	}
#endif

	[Native]
	public enum NSComparisonResult : long {
		Ascending = -1,
		Same,
		Descending
	}

	[Native]
	public enum NSUrlRequestCachePolicy : ulong {
		UseProtocolCachePolicy = 0,
		ReloadIgnoringLocalCacheData = 1,
		ReloadIgnoringLocalAndRemoteCacheData = 4, // Unimplemented
		ReloadIgnoringCacheData = ReloadIgnoringLocalCacheData,

		ReturnCacheDataElseLoad = 2,
		ReturnCacheDataDoNotLoad = 3,

		ReloadRevalidatingCacheData = 5, // Unimplemented
	}

	[Native]
	public enum NSUrlCacheStoragePolicy : ulong {
		Allowed, AllowedInMemoryOnly, NotAllowed
	}

	[Native]
	public enum NSStreamStatus : ulong {
		NotOpen = 0,
		Opening = 1,
		Open = 2,
		Reading = 3,
		Writing = 4,
		AtEnd = 5,
		Closed = 6,
		Error = 7
	}

	[Native]
	public enum NSPropertyListFormat : ulong {
		OpenStep = 1,
		Xml = 100,
		Binary = 200
	}

	[Native]
	public enum NSPropertyListMutabilityOptions : ulong {
		Immutable = 0,
		MutableContainers = 1,
		MutableContainersAndLeaves = 2
	}

	// Should mirror NSPropertyListMutabilityOptions
	[Native]
	public enum NSPropertyListWriteOptions : ulong {
		Immutable = 0,
		MutableContainers = 1,
		MutableContainersAndLeaves = 2
	}

	// Should mirror NSPropertyListMutabilityOptions, but currently
	// not implemented (always use Immutable/0)
	[Native]
	public enum NSPropertyListReadOptions : ulong {
		Immutable = 0,
		MutableContainers = 1,
		MutableContainersAndLeaves = 2
	}

	[Native]
	[Flags]
	public enum NSMachPortRights : ulong {
		None = 0,
		SendRight = (1 << 0),
		ReceiveRight = (1 << 1)
	}

	[Native]
	public enum NSNetServicesStatus : long {
		UnknownError = -72000,
		CollisionError = -72001,
		NotFoundError = -72002,
		ActivityInProgress = -72003,
		BadArgumentError = -72004,
		CancelledError = -72005,
		InvalidError = -72006,
		TimeoutError = -72007,
		MissingRequiredConfigurationError = -72008,
	}

#if XAMCORE_5_0
	[NoWatch]
#endif
	[Flags]
	[Native]
	public enum NSNetServiceOptions : ulong {
		NoAutoRename = 1 << 0,
		ListenForConnections = 1 << 1
	}

	[Native]
	public enum NSDateFormatterStyle : ulong {
		None,
		Short,
		Medium,
		Long,
		Full
	}

	[Native]
	public enum NSDateFormatterBehavior : ulong {
		Default = 0,
		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		Mode_10_0 = 1000,
		Mode_10_4 = 1040,
	}

	[Native]
	public enum NSHttpCookieAcceptPolicy : ulong {
		Always, Never, OnlyFromMainDocumentDomain
	}

	[Flags]
	[Native]
	public enum NSCalendarUnit : ulong {
		Era = 2,
		Year = 4,
		Month = 8,
		Day = 16,
		Hour = 32,
		Minute = 64,
		Second = 128,
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		Week = 256,
		Weekday = 512,
		WeekdayOrdinal = 1024,
		Quarter = 2048,

		WeekOfMonth = (1 << 12),
		WeekOfYear = (1 << 13),
		YearForWeakOfYear = (1 << 14),

		Nanosecond = (1 << 15),

		Calendar = (1 << 20),
		TimeZone = (1 << 21),
	}

	[Flags]
	[Native]
	public enum NSDataReadingOptions : ulong {
		Mapped = 1 << 0,
		Uncached = 1 << 1,
#if !NET
		[Obsolete ("This option is unavailable.")]
		Coordinated = 1 << 2,
#endif
		MappedAlways = 1 << 3
	}

	[Flags]
	[Native]
	public enum NSDataWritingOptions : ulong {
		Atomic = 1,

		WithoutOverwriting = 2,
		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		FileProtectionNone = 0x10000000,
		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		FileProtectionComplete = 0x20000000,
		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		FileProtectionMask = 0xf0000000,
		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		FileProtectionCompleteUnlessOpen = 0x30000000,
		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		FileProtectionCompleteUntilFirstUserAuthentication = 0x40000000,
		[iOS (17, 0), NoMac, MacCatalyst (17, 0), TV (17, 0), Watch (10, 0)]
		FileProtectionCompleteWhenUserInactive = 0x50000000,
	}

	public delegate void NSSetEnumerator (NSObject obj, ref bool stop);

	[Native]
	public enum NSOperationQueuePriority : long {
		VeryLow = -8, Low = -4, Normal = 0, High = 4, VeryHigh = 8
	}

	[Flags]
	[Native]
	public enum NSNotificationCoalescing : ulong {
		NoCoalescing = 0,
		CoalescingOnName = 1,
		CoalescingOnSender = 2
	}

	[Native]
	public enum NSPostingStyle : ulong {
		PostWhenIdle = 1, PostASAP = 2, Now = 3
	}

	[Flags]
	[Native]
	public enum NSDataSearchOptions : ulong {
		SearchBackwards = 1,
		SearchAnchored = 2
	}

	[Native]
	public enum NSExpressionType : ulong {
		ConstantValue = 0,
		EvaluatedObject,
		Variable,
		KeyPath,
		Function,
		UnionSet,
		IntersectSet,
		MinusSet,
		Subquery = 13,
		NSAggregate,
		AnyKey = 15,
		Block = 19,
		Conditional = 20
	}

	public enum NSCocoaError : int {
		None,

		FileNoSuchFile = 4,
		FileLocking = 255,
		FileReadUnknown = 256,
		FileReadNoPermission = 257,
		FileReadInvalidFileName = 258,
		FileReadCorruptFile = 259,
		FileReadNoSuchFile = 260,
		FileReadInapplicableStringEncoding = 261,
		FileReadUnsupportedScheme = 262,
		FileReadTooLarge = 263,
		FileReadUnknownStringEncoding = 264,
		FileWriteUnknown = 512,
		FileWriteNoPermission = 513,
		FileWriteInvalidFileName = 514,
		FileWriteFileExists = 516,
		FileWriteInapplicableStringEncoding = 517,
		FileWriteUnsupportedScheme = 518,
		FileWriteOutOfSpace = 640,
		FileWriteVolumeReadOnly = 642,

#if MONOMAC
		FileManagerUnmountUnknownError = 768,
		FileManagerUnmountBusyError = 769,
#endif

		KeyValueValidation = 1024,
		Formatting = 2048,
		UserCancelled = 3072,
		FeatureUnsupported = 3328,
		ExecutableNotLoadable = 3584,
		ExecutableArchitectureMismatch = 3585,
		ExecutableRuntimeMismatch = 3586,
		ExecutableLoad = 3587,
		ExecutableLink = 3588,
		FileErrorMinimum = 0,
		FileErrorMaximum = 1023,
		ValidationErrorMinimum = 1024,
		ValidationErrorMaximum = 2047,
		ExecutableErrorMinimum = 3584,
		ExecutableErrorMaximum = 3839,
		FormattingErrorMinimum = 2048,
		FormattingErrorMaximum = 2559,

		PropertyListReadCorrupt = 3840,
		PropertyListReadUnknownVersion = 3841,
		PropertyListReadStream = 3842,
		PropertyListWriteStream = 3851,
		PropertyListWriteInvalid = 3852,
		PropertyListErrorMinimum = 3840,
		PropertyListErrorMaximum = 4095,

		XpcConnectionInterrupted = 4097,
		XpcConnectionInvalid = 4099,
		XpcConnectionReplyInvalid = 4101,
		XpcConnectionCodeSigningRequirementFailure = 4102,
		XpcConnectionErrorMinimum = 4096,
		XpcConnectionErrorMaximum = 4224,

		UbiquitousFileUnavailable = 4353,
		UbiquitousFileNotUploadedDueToQuota = 4354,
		UbiquitousFileUbiquityServerNotAvailable = 4355,
		UbiquitousFileErrorMinimum = 4352,
		UbiquitousFileErrorMaximum = 4607,

		UserActivityHandoffFailedError = 4608,
		UserActivityConnectionUnavailableError = 4609,
		UserActivityRemoteApplicationTimedOutError = 4610,
		UserActivityHandoffUserInfoTooLargeError = 4611,

		UserActivityErrorMinimum = 4608,
		UserActivityErrorMaximum = 4863,

		CoderReadCorruptError = 4864,
		CoderValueNotFoundError = 4865,
		CoderInvalidValueError = 4866,
		CoderErrorMinimum = 4864,
		CoderErrorMaximum = 4991,

		BundleErrorMinimum = 4992,
		BundleErrorMaximum = 5119,

		BundleOnDemandResourceOutOfSpaceError = 4992,
		BundleOnDemandResourceExceededMaximumSizeError = 4993,
		BundleOnDemandResourceInvalidTagError = 4994,

		CloudSharingNetworkFailureError = 5120,
		CloudSharingQuotaExceededError = 5121,
		CloudSharingTooManyParticipantsError = 5122,
		CloudSharingConflictError = 5123,
		CloudSharingNoPermissionError = 5124,
		CloudSharingOtherError = 5375,
		CloudSharingErrorMinimum = 5120,
		CloudSharingErrorMaximum = 5375,

		CompressionFailedError = 5376,
		DecompressionFailedError = 5377,
		CompressionErrorMinimum = 5376,
		CompressionErrorMaximum = 5503,
	}

	// note: Make sure names are identical/consistent with CFNetworkErrors.*
	// they share the same values but there's more entries in CFNetworkErrors
	// so anything new probably already exists over there
	public enum NSUrlError : int {
		Unknown = -1,

		BackgroundSessionRequiresSharedContainer = -995,
		BackgroundSessionInUseByAnotherProcess = -996,
		BackgroundSessionWasDisconnected = -997,

		Cancelled = -999,
		BadURL = -1000,
		TimedOut = -1001,
		UnsupportedURL = -1002,
		CannotFindHost = -1003,
		CannotConnectToHost = -1004,
		NetworkConnectionLost = -1005,
		DNSLookupFailed = -1006,
		HTTPTooManyRedirects = -1007,
		ResourceUnavailable = -1008,
		NotConnectedToInternet = -1009,
		RedirectToNonExistentLocation = -1010,
		BadServerResponse = -1011,
		UserCancelledAuthentication = -1012,
		UserAuthenticationRequired = -1013,
		ZeroByteResource = -1014,
		CannotDecodeRawData = -1015,
		CannotDecodeContentData = -1016,
		CannotParseResponse = -1017,
		InternationalRoamingOff = -1018,
		CallIsActive = -1019,
		DataNotAllowed = -1020,
		RequestBodyStreamExhausted = -1021,
		AppTransportSecurityRequiresSecureConnection = -1022,

		FileDoesNotExist = -1100,
		FileIsDirectory = -1101,
		NoPermissionsToReadFile = -1102,
		DataLengthExceedsMaximum = -1103,
		FileOutsideSafeArea = -1104,

		SecureConnectionFailed = -1200,
		ServerCertificateHasBadDate = -1201,
		ServerCertificateUntrusted = -1202,
		ServerCertificateHasUnknownRoot = -1203,
		ServerCertificateNotYetValid = -1204,
		ClientCertificateRejected = -1205,
		ClientCertificateRequired = -1206,

		CannotLoadFromNetwork = -2000,

		// Download and file I/O errors
		CannotCreateFile = -3000,
		CannotOpenFile = -3001,
		CannotCloseFile = -3002,
		CannotWriteToFile = -3003,
		CannotRemoveFile = -3004,
		CannotMoveFile = -3005,
		DownloadDecodingFailedMidStream = -3006,
		DownloadDecodingFailedToComplete = -3007,
	}

	[Flags]
	[Native]
	public enum NSKeyValueObservingOptions : ulong {
		None = 0,
		New = 1,
		Old = 2,
		OldNew = 3,
		Initial = 4,
		Prior = 8,
	}

	[Native]
	public enum NSKeyValueChange : ulong {
		Setting = 1, Insertion, Removal, Replacement
	}

	[Native]
	public enum NSKeyValueSetMutationKind : ulong {
		UnionSet = 1, MinusSet, IntersectSet, SetSet
	}

	[Flags]
	[Native]
	public enum NSEnumerationOptions : ulong {
		SortConcurrent = 1,
		Reverse = 2
	}

	[Flags]
	[Native]
	public enum NSStreamEvent : ulong {
		None = 0,
		OpenCompleted = 1 << 0,
		HasBytesAvailable = 1 << 1,
		HasSpaceAvailable = 1 << 2,
		ErrorOccurred = 1 << 3,
		EndEncountered = 1 << 4
	}

	[Native]
	public enum NSComparisonPredicateModifier : ulong {
		Direct,
		All,
		Any
	}

	[Native]
	public enum NSPredicateOperatorType : ulong {
		LessThan,
		LessThanOrEqualTo,
		GreaterThan,
		GreaterThanOrEqualTo,
		EqualTo,
		NotEqualTo,
		Matches,
		Like,
		BeginsWith,
		EndsWith,
		In,
		CustomSelector,
		Contains = 99,
		Between
	}

	[Flags]
	[Native]
	public enum NSComparisonPredicateOptions : ulong {
		None = 0x00,
		CaseInsensitive = 1 << 0,
		DiacriticInsensitive = 1 << 1,
		Normalized = 1 << 2
	}

	[Native]
	public enum NSCompoundPredicateType : ulong {
		Not,
		And,
		Or
	}

	[Flags]
	[Native]
	public enum NSVolumeEnumerationOptions : ulong {
		None = 0,
		// skip                  = 1 << 0,
		SkipHiddenVolumes = 1 << 1,
		ProduceFileReferenceUrls = 1 << 2,
	}

	[Flags]
	[Native]
	public enum NSDirectoryEnumerationOptions : ulong {
#if !NET
		[Obsolete ("Use 'None' instead.")]
		SkipsNone = 0,
#endif
		None = 0,
		SkipsSubdirectoryDescendants = 1 << 0,
		SkipsPackageDescendants = 1 << 1,
		SkipsHiddenFiles = 1 << 2,
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		IncludesDirectoriesPostOrder = 1 << 3,
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		ProducesRelativePathUrls = 1 << 4,
	}

	[Flags]
	[Native]
	public enum NSFileManagerItemReplacementOptions : ulong {
		None = 0,
		UsingNewMetadataOnly = 1 << 0,
		WithoutDeletingBackupItem = 1 << 1,
	}

	[Native]
	public enum NSSearchPathDirectory : ulong {
		ApplicationDirectory = 1,
		DemoApplicationDirectory,
		DeveloperApplicationDirectory,
		AdminApplicationDirectory,
		LibraryDirectory,
		DeveloperDirectory,
		UserDirectory,
		DocumentationDirectory,
		DocumentDirectory,
		CoreServiceDirectory,
		AutosavedInformationDirectory = 11,
		DesktopDirectory = 12,
		CachesDirectory = 13,
		ApplicationSupportDirectory = 14,
		DownloadsDirectory = 15,
		InputMethodsDirectory = 16,
		MoviesDirectory = 17,
		MusicDirectory = 18,
		PicturesDirectory = 19,
		PrinterDescriptionDirectory = 20,
		SharedPublicDirectory = 21,
		PreferencePanesDirectory = 22,
		[NoWatch]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		ApplicationScriptsDirectory = 23,
		ItemReplacementDirectory = 99,
		AllApplicationsDirectory = 100,
		AllLibrariesDirectory = 101,
		[NoTV, NoWatch]
		[MacCatalyst (13, 1)]
		TrashDirectory = 102,
	}

	[Flags]
	[Native]
	public enum NSSearchPathDomain : ulong {
		None = 0,
		User = 1 << 0,
		Local = 1 << 1,
		Network = 1 << 2,
		System = 1 << 3,
		All = 0x0ffff,
	}

	[Native]
	public enum NSRoundingMode : ulong {
		Plain, Down, Up, Bankers
	}

	[Native]
	public enum NSCalculationError : ulong {
		None, PrecisionLoss, Underflow, Overflow, DivideByZero
	}

	[Flags]
	[Native]
	public enum NSStringDrawingOptions : ulong {
		UsesLineFragmentOrigin = (1 << 0),
		UsesFontLeading = (1 << 1),
		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		DisableScreenFontSubstitution = (1 << 2),
		UsesDeviceMetrics = (1 << 3),
		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		OneShot = (1 << 4),
		TruncatesLastVisibleLine = (1 << 5)
	}

	[Native]
	public enum NSNumberFormatterStyle : ulong {
		None = 0,
		Decimal = 1,
		Currency = 2,
		Percent = 3,
		Scientific = 4,
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

	[Native]
	public enum NSNumberFormatterBehavior : ulong {
		Default = 0,
		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		Version_10_0 = 1000,
		Version_10_4 = 1040
	}

	[Native]
	public enum NSNumberFormatterPadPosition : ulong {
		BeforePrefix, AfterPrefix, BeforeSuffix, AfterSuffix
	}

	[Native]
	public enum NSNumberFormatterRoundingMode : ulong {
		Ceiling, Floor, Down, Up, HalfEven, HalfDown, HalfUp
	}

	[Flags]
	[Native]
	public enum NSFileVersionReplacingOptions : ulong {
		ByMoving = 1 << 0
	}

	[Flags]
	[Native]
	public enum NSFileVersionAddingOptions : ulong {
		ByMoving = 1 << 0
	}

	[Flags]
	[Native]
	public enum NSFileCoordinatorReadingOptions : ulong {
		WithoutChanges = 1,
		ResolvesSymbolicLink = 1 << 1,
		[MacCatalyst (13, 1)]
		ImmediatelyAvailableMetadataOnly = 1 << 2,
		[MacCatalyst (13, 1)]
		ForUploading = 1 << 3
	}

	[Flags]
	[Native]
	public enum NSFileCoordinatorWritingOptions : ulong {
		ForDeleting = 1,
		ForMoving = 2,
		ForMerging = 4,
		ForReplacing = 8,
		[MacCatalyst (13, 1)]
		ContentIndependentMetadataOnly = 16,
	}

	[Flags]
	[Native]
	public enum NSLinguisticTaggerOptions : ulong {
		OmitWords = 1,
		OmitPunctuation = 2,
		OmitWhitespace = 4,
		OmitOther = 8,
		JoinNames = 16
	}

	[Native]
	public enum NSUbiquitousKeyValueStoreChangeReason : long {
		ServerChange, InitialSyncChange, QuotaViolationChange, AccountChange
	}

	[Flags]
	[Native]
	public enum NSJsonReadingOptions : ulong {
		MutableContainers = 1,
		MutableLeaves = 2,
		FragmentsAllowed = 4,
		[Mac (12, 0), iOS (15, 0), TV (15, 0), Watch (8, 0), MacCatalyst (15, 0)]
		Json5Allowed = 8,
		[Mac (12, 0), iOS (15, 0), TV (15, 0), Watch (8, 0), MacCatalyst (15, 0)]
		TopLevelDictionaryAssumed = 16,
#if !NET
		[Obsolete ("Use 'FragmentsAllowed. instead.")]
		AllowFragments = FragmentsAllowed,
#endif
	}

	[Flags]
	[Native]
	public enum NSJsonWritingOptions : ulong {
		PrettyPrinted = 1,
		[MacCatalyst (13, 1)]
		SortedKeys = (1 << 1),
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		FragmentsAllowed = (1 << 2),
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		WithoutEscapingSlashes = (1 << 3),
	}

	[Native]
	public enum NSLocaleLanguageDirection : ulong {
		Unknown, LeftToRight, RightToLeft, TopToBottom, BottomToTop,
	}

	[Flags]
	public enum NSAlignmentOptions : long {
		MinXInward = 1 << 0,
		MinYInward = 1 << 1,
		MaxXInward = 1 << 2,
		MaxYInward = 1 << 3,
		WidthInward = 1 << 4,
		HeightInward = 1 << 5,

		MinXOutward = 1 << 8,
		MinYOutward = 1 << 9,
		MaxXOutward = 1 << 10,
		MaxYOutward = 1 << 11,
		WidthOutward = 1 << 12,
		HeightOutward = 1 << 13,

		MinXNearest = 1 << 16,
		MinYNearest = 1 << 17,
		MaxXNearest = 1 << 18,
		MaxYNearest = 1 << 19,
		WidthNearest = 1 << 20,
		HeightNearest = 1 << 21,

		RectFlipped = unchecked((long) (1UL << 63)),

		AllEdgesInward = MinXInward | MaxXInward | MinYInward | MaxYInward,
		AllEdgesOutward = MinXOutward | MaxXOutward | MinYOutward | MaxYOutward,
		AllEdgesNearest = MinXNearest | MaxXNearest | MinYNearest | MaxYNearest,
	}

	[Flags]
	[Native]
	public enum NSFileWrapperReadingOptions : ulong {
		Immediate = 1 << 0,
		WithoutMapping = 1 << 1
	}

	[Flags]
	[Native]
	public enum NSFileWrapperWritingOptions : ulong {
		Atomic = 1 << 0,
		WithNameUpdating = 1 << 1
	}

	[Flags]
	[Native ("NSAttributedStringEnumerationOptions")]
	public enum NSAttributedStringEnumeration : ulong {
		None = 0,
		Reverse = 1 << 1,
		LongestEffectiveRangeNotRequired = 1 << 20
	}

#if NET || !MONOMAC
	// macOS has defined this in AppKit as well, but starting with .NET we're going
	// to use this one only.
	[Native]
	public enum NSUnderlineStyle : long {
		None = 0x00,
		Single = 0x01,
		Thick = 0x02,
		Double = 0x09,
		PatternSolid = 0x0000,
		PatternDot = 0x0100,
		PatternDash = 0x0200,
		PatternDashDot = 0x0300,
		PatternDashDotDot = 0x0400,
		ByWord = 0x8000
	}
#endif

	// There's an AppKit.NSWritingDirection, which is deprecated.
	// There's also an UIKit.UITextWritingDirection, which is deprecated too.
	// This is the enum we should be using.
	// See https://github.com/xamarin/xamarin-macios/issues/6573
	[Native]
	public enum NSWritingDirection : long {
		Natural = -1, LeftToRight = 0, RightToLeft = 1,
	}

	[Flags]
	[Native]
	public enum NSByteCountFormatterUnits : ulong {
		UseDefault = 0,
		UseBytes = 1 << 0,
		UseKB = 1 << 1,
		UseMB = 1 << 2,
		UseGB = 1 << 3,
		UseTB = 1 << 4,
		UsePB = 1 << 5,
		UseEB = 1 << 6,
		UseZB = 1 << 7,
		UseYBOrHigher = 0x0FF << 8,
		UseAll = 0x0FFFF
	}

	[Native]
	public enum NSByteCountFormatterCountStyle : long {
		File, Memory, Decimal, Binary
	}

	[Flags]
	[Native]
	public enum NSUrlBookmarkCreationOptions : ulong {
		PreferFileIDResolution = 1 << 8,
		MinimalBookmark = 1 << 9,
		SuitableForBookmarkFile = 1 << 10,
		[NoiOS, NoTV, NoWatch]
		[NoMacCatalyst]
		WithSecurityScope = 1 << 11,
		[NoiOS, NoTV, NoWatch]
		[NoMacCatalyst]
		SecurityScopeAllowOnlyReadAccess = 1 << 12,
		[Mac (12, 0), iOS (15, 0), TV (15, 0), Watch (8, 0), MacCatalyst (15, 0)]
		CreationWithoutImplicitSecurityScope = 1 << 29,
	}

	[Flags]
	[Native]
	public enum NSUrlBookmarkResolutionOptions : ulong {
		WithoutUI = 1 << 8,
		WithoutMounting = 1 << 9,
		[NoiOS, NoTV, NoWatch]
		[NoMacCatalyst]
		WithSecurityScope = 1 << 10,
		[Mac (12, 0), iOS (15, 0), TV (15, 0), Watch (8, 0), MacCatalyst (15, 0)]
		WithoutImplicitStartAccessing = 1 << 15,
	}

	[Native]
	public enum NSLigatureType : long {
		None, Default, All
	}

#if !NET
	[Flags]
	[Native]
	public enum NSDateComponentsWrappingBehavior : ulong {
		None = 0,
		WrapCalendarComponents = 1 << 0,

		// Did not add the new enums here, we moved them elsewhere, and provided overloads.
	}
#endif

	[Flags]
	[Native]
	public enum NSCalendarOptions : ulong {
		None = 0,
		WrapCalendarComponents = 1 << 0,

		[MacCatalyst (13, 1)]
		MatchStrictly = 1 << 1,
		[MacCatalyst (13, 1)]
		SearchBackwards = 1 << 2,

		[MacCatalyst (13, 1)]
		MatchPreviousTimePreservingSmallerUnits = 1 << 8,
		[MacCatalyst (13, 1)]
		MatchNextTimePreservingSmallerUnits = 1 << 9,
		[MacCatalyst (13, 1)]
		MatchNextTime = 1 << 10,

		[MacCatalyst (13, 1)]
		MatchFirst = 1 << 12,
		[MacCatalyst (13, 1)]
		MatchLast = 1 << 13,
	}

	[Native]
	public enum NSUrlRequestNetworkServiceType : ulong {
		Default,
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'PushKit' framework instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'PushKit' framework instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'PushKit' framework instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'PushKit' framework instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PushKit' framework instead.")]
		VoIP,
		Video,
		Background,
		Voice,
		[iOS (12, 0)]
		[Watch (5, 0)]
		[TV (12, 0)]
		[MacCatalyst (13, 1)]
		ResponsiveData = 6,
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		AVStreaming = 8,
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		ResponsiveAV = 9,
		[MacCatalyst (13, 1)]
		CallSignaling = 11,
	}

	[Flags]
	[Native]
	public enum NSSortOptions : ulong {
		Concurrent = 1 << 0,
		Stable = 1 << 4
	}

	[Flags]
	[Native]
	public enum NSDataBase64DecodingOptions : ulong {
		None = 0,
		IgnoreUnknownCharacters = 1
	}

	[Flags]
	[Native]
	public enum NSDataBase64EncodingOptions : ulong {
		None = 0,
		SixtyFourCharacterLineLength = 1,
		SeventySixCharacterLineLength = 1 << 1,
		EndLineWithCarriageReturn = 1 << 4,
		EndLineWithLineFeed = 1 << 5
	}

#if !XAMCORE_3_0
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSWritingDirectionFormatType'.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSWritingDirectionFormatType'.")]
	[Flags]
	[Native]
	public enum NSTextWritingDirection : long {
		Embedding = 0, Override = 2
	}
#endif

	[Native]
	public enum NSUrlSessionAuthChallengeDisposition : long {
		UseCredential = 0,
		PerformDefaultHandling = 1,
		CancelAuthenticationChallenge = 2,
		RejectProtectionSpace = 3
	}

	[Native]
	public enum NSUrlSessionTaskState : long {
		Running = 0,
		Suspended = 1,
		Canceling = 2,
		Completed = 3
	}

	[Native]
	public enum NSUrlSessionResponseDisposition : long {
		Cancel = 0,
		Allow = 1,
		BecomeDownload = 2,
		BecomeStream = 3
	}

	[Native]
	public enum NSUrlErrorCancelledReason : long {
		UserForceQuitApplication,
		BackgroundUpdatesDisabled,
		InsufficientSystemResources
	}

	[Flags]
	public enum NSActivityOptions : ulong {
		IdleDisplaySleepDisabled = 1UL << 40,
		IdleSystemSleepDisabled = 1UL << 20,
		SuddenTerminationDisabled = 1UL << 14,
		AutomaticTerminationDisabled = 1UL << 15,
		AnimationTrackingEnabled = 1uL << 45,
		TrackingEnabled = 1uL << 46,
		UserInteractive = (UserInitiated | LatencyCritical),
		UserInitiated = 0x00FFFFFFUL | IdleSystemSleepDisabled,
		Background = 0x000000ffUL,
		LatencyCritical = 0xFF00000000UL,
		InitiatedAllowingIdleSystemSleep = UserInitiated & ~IdleSystemSleepDisabled,
	}

	[Native]
	public enum NSTimeZoneNameStyle : long {
		Standard,
		ShortStandard,
		DaylightSaving,
		ShortDaylightSaving,
		Generic,
		ShortGeneric
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSItemProviderErrorCode : long {
		Unknown = -1,
		None = 0,
		ItemUnavailable = -1000,
		UnexpectedValueClass = -1100,
		UnavailableCoercion = -1200
	}

	[Native]
	[MacCatalyst (13, 1)]
	public enum NSDateComponentsFormatterUnitsStyle : long {
		Positional = 0,
		Abbreviated,
		Short,
		Full,
		SpellOut,
		[MacCatalyst (13, 1)]
		Brief,
	}

	[Flags]
	[Native]
	[MacCatalyst (13, 1)]
	public enum NSDateComponentsFormatterZeroFormattingBehavior : ulong {
		None = (0),
		Default = (1 << 0),
		DropLeading = (1 << 1),
		DropMiddle = (1 << 2),
		DropTrailing = (1 << 3),
		DropAll = (DropLeading | DropMiddle | DropTrailing),
		Pad = (1 << 16)
	}

	[Native]
	[MacCatalyst (13, 1)]
	public enum NSFormattingContext : long {
		Unknown = 0,
		Dynamic = 1,
		Standalone = 2,
		ListItem = 3,
		BeginningOfSentence = 4,
		MiddleOfSentence = 5
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSDateIntervalFormatterStyle : ulong {
		None = 0,
		Short = 1,
		Medium = 2,
		Long = 3,
		Full = 4
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSEnergyFormatterUnit : long {
		Joule = 11,
		Kilojoule = 14,
		Calorie = (7 << 8) + 1,
		Kilocalorie = (7 << 8) + 2
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSFormattingUnitStyle : long {
		Short = 1,
		Medium,
		Long
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSMassFormatterUnit : long {
		Gram = 11,
		Kilogram = 14,
		Ounce = (6 << 8) + 1,
		Pound = (6 << 8) + 2,
		Stone = (6 << 8) + 3
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSLengthFormatterUnit : long {
		Millimeter = 8,
		Centimeter = 9,
		Meter = 11,
		Kilometer = 14,
		Inch = (5 << 8) + 1,
		Foot = (5 << 8) + 2,
		Yard = (5 << 8) + 3,
		Mile = (5 << 8) + 4
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSQualityOfService : long {
		UserInteractive = 33,
		UserInitiated = 25,
		Utility = 17,
		Background = 9,
		Default = -1
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSProcessInfoThermalState : long {
		Nominal, Fair, Serious, Critical
	}

	[Native]
	public enum NSUrlRelationship : long {
		Contains, Same, Other
	}

	// NSTextCheckingResult.h:typedef NS_OPTIONS(uint64_t, NSTextCheckingType)
	[Flags]
	public enum NSTextCheckingType : ulong {
		Orthography = 1 << 0,
		Spelling = 1 << 1,
		Grammar = 1 << 2,
		Date = 1 << 3,
		Address = 1 << 4,
		Link = 1 << 5,
		Quote = 1 << 6,
		Dash = 1 << 7,
		Replacement = 1 << 8,
		Correction = 1 << 9,
		RegularExpression = 1 << 10,
		PhoneNumber = 1 << 11,
		TransitInformation = 1 << 12,
	}

	// NSTextCheckingResult.h:typedef uint64_t NSTextCheckingTypes;
	public enum NSTextCheckingTypes : ulong {
		AllSystemTypes = 0xffffffff,
		AllCustomTypes = 0xffffffff00000000,
		AllTypes = 0xffffffffffffffff
	}

	[Native]
	[Flags]
	public enum NSRegularExpressionOptions : ulong {
		CaseInsensitive = 1 << 0,
		AllowCommentsAndWhitespace = 1 << 1,
		IgnoreMetacharacters = 1 << 2,
		DotMatchesLineSeparators = 1 << 3,
		AnchorsMatchLines = 1 << 4,
		UseUnixLineSeparators = 1 << 5,
		UseUnicodeWordBoundaries = 1 << 6
	}

	[Native]
	[Flags]
	public enum NSMatchingOptions : ulong {
		ReportProgress = 1 << 0,
		ReportCompletion = 1 << 1,
		Anchored = 1 << 2,
		WithTransparentBounds = 1 << 3,
		WithoutAnchoringBounds = 1 << 4
	}

	[Native]
	[Flags]
	public enum NSMatchingFlags : ulong {
		Progress = 1 << 0,
		Completed = 1 << 1,
		HitEnd = 1 << 2,
		RequiredEnd = 1 << 3,
		InternalError = 1 << 4
	}

	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum NSPersonNameComponentsFormatterOptions : ulong {
		Phonetic = (1 << 1)
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSPersonNameComponentsFormatterStyle : long {
		Default = 0,
		Short,
		Medium,
		Long,
		Abbreviated
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSDecodingFailurePolicy : long {
		RaiseException,
		SetErrorAndReturn
	}

	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum NSIso8601DateFormatOptions : ulong {
		Year = 1 << 0,
		Month = 1 << 1,
		WeekOfYear = 1 << 2,
		Day = 1 << 4,
		Time = 1 << 5,
		TimeZone = 1 << 6,
		SpaceBetweenDateAndTime = 1 << 7,
		DashSeparatorInDate = 1 << 8,
		ColonSeparatorInTime = 1 << 9,
		ColonSeparatorInTimeZone = 1 << 10,
		[MacCatalyst (13, 1)]
		FractionalSeconds = 1 << 11,
		FullDate = Year | Month | Day | DashSeparatorInDate,
		FullTime = Time | ColonSeparatorInTime | TimeZone | ColonSeparatorInTimeZone,
		InternetDateTime = FullDate | FullTime,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSUrlSessionTaskMetricsResourceFetchType : long {
		Unknown,
		NetworkLoad,
		ServerPush,
		LocalCache
	}

	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum NSMeasurementFormatterUnitOptions : ulong {
		ProvidedUnit = (1 << 0),
		NaturalScale = (1 << 1),
		TemperatureWithoutUnit = (1 << 2)
	}


	[MacCatalyst (13, 1)]
	[Native]
	public enum NSItemProviderRepresentationVisibility : long {
		All = 0,
		[NoMac]
		[MacCatalyst (13, 1)]
		Team = 1,
		[NoiOS, NoTV, NoWatch]
		[NoMacCatalyst]
		Group = 2,
		OwnProcess = 3,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSItemProviderFileOptions : long {
		OpenInPlace = 1,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSLinguisticTaggerUnit : long {
		Word,
		Sentence,
		Paragraph,
		Document,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NSUrlSessionDelayedRequestDisposition : long {
		ContinueLoading = 0,
		UseNewRequest = 1,
		Cancel = 2,
	}

	[Native]
	public enum NSXpcConnectionOptions : ulong {
		Privileged = (1 << 12),
	}

	[Mac (11, 0), MacCatalyst (13, 1)]
	public enum NSFileProtectionType {
		[Field ("NSFileProtectionComplete")]
		Complete,
		[Field ("NSFileProtectionCompleteUnlessOpen")]
		CompleteUnlessOpen,
		[Field ("NSFileProtectionCompleteUntilFirstUserAuthentication")]
		CompleteUntilFirstUserAuthentication,
		[Watch (10, 0), TV (17, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSFileProtectionCompleteWhenUserInactive")]
		CompleteWhenUserInactive,
		[Field ("NSFileProtectionNone")]
		None,
	}
}
