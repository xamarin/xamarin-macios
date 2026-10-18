//
// BackgroundAssets C# bindings
//
// Authors:
//	Manuel de la Pena Saenz <mandel@microsoft.com>
//
// Copyright 2022 Microsoft Corporation All rights reserved.
//

using CoreFoundation;

namespace BackgroundAssets {
	[TV (18, 4), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum BADownloadState : long {
		Failed = -1,
		Created = 0,
		Waiting,
		Downloading,
		Finished,
	}

	[TV (18, 4), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum BAContentRequest : long {
		Install = 1,
		Update,
		Periodic,
		/// <summary>A content request resulting from a change to the application's preferred language.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		LanguageChange = 4,
	}

	[TV (18, 4), iOS (17, 0), MacCatalyst (17, 0)]
	[ErrorDomain ("BAErrorDomain")]
	[Native]
	public enum BAErrorCode : long {
		DownloadInvalid = 0,
		CallFromExtensionNotAllowed = 50,
		CallFromInactiveProcessNotAllowed = 51,
		CallerConnectionNotAccepted = 55,
		CallerConnectionInvalid = 56,
		DownloadAlreadyScheduled = 100,
		DownloadNotScheduled = 101,
		DownloadFailedToStart = 102,
		DownloadAlreadyFailed = 103,
		DownloadEssentialDownloadNotPermitted = 109,
		DownloadBackgroundActivityProhibited = 111,
		DownloadWouldExceedAllowance = 112,
		DownloadDoesNotExist = 113,
		SessionDownloadDisallowedByDomain = 202,
		SessionDownloadDisallowedByAllowance = 203,
		SessionDownloadAllowanceExceeded = 204,
		SessionDownloadNotPermittedBeforeAppLaunch = 206,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
	[Flags]
	[Native]
	public enum BAAssetPackStatus : ulong {
		DownloadAvailable = 1uL << 0,
		UpdateAvailable = 1uL << 1,
		UpToDate = 1uL << 2,
		OutOfDate = 1uL << 3,
		Obsolete = 1uL << 4,
		Downloading = 1uL << 5,
		Downloaded = 1uL << 6,
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
	[ErrorDomain ("BAManagedErrorDomain")]
	[Native]
	public enum BAManagedErrorCode : long {
		AssetPackNotFound,
		FileNotFound,
		/// <summary>The system couldn't ensure local availability for some or all of the requested asset packs.</summary>
		LocalAvailabilityFailure = 2,
	}

	/// <summary>Specifies how to calculate the size of locally installed asset files.</summary>
	[TV (27, 2), Mac (27, 2), iOS (27, 2), MacCatalyst (27, 2)]
	[Native]
	public enum BASizeCalculationMethod : long {
		/// <summary>Counts the bytes in the file contents.</summary>
		Logical = 0,
		/// <summary>Counts the bytes allocated by the file system.</summary>
		Allocated = 1,
	}

	[TV (18, 4), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BADownload : NSCoding, NSSecureCoding, NSCopying {
		[Export ("state")]
		BADownloadState State { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("uniqueIdentifier")]
		string UniqueIdentifier { get; }

		[Export ("priority")]
		nint Priority { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("isEssential")]
		bool IsEssential { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[return: Release]
		[Export ("copyAsNonEssential")]
		BADownload CopyAsNonEssential ();
	}

	[TV (18, 4), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BAAppExtensionInfo : NSSecureCoding {

		[iOS (16, 1), MacCatalyst (16, 1)]
		[NullAllowed]
		[Export ("restrictedDownloadSizeRemaining", ArgumentSemantic.Strong)]
		NSNumber RestrictedDownloadSizeRemaining { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[NullAllowed]
		[Export ("restrictedEssentialDownloadSizeRemaining", ArgumentSemantic.Strong)]
		NSNumber RestrictedEssentialDownloadSizeRemaining { get; }
	}

	[TV (18, 4), iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface BADownloaderExtension {

		[NoTV]
		[Deprecated (PlatformName.iOS, 16, 4, message: "'WillTerminate' will not be called in all applicable scenarios, do not rely on it.")]
		[Deprecated (PlatformName.MacOSX, 13, 3, message: "'WillTerminate' will not be invoked in all applicable scenarios, do not rely on it.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 4, message: "'WillTerminate' will not be invoked in all applicable scenarios, do not rely on it.")]
		[Export ("extensionWillTerminate")]
		void WillTerminate ();

		[Export ("backgroundDownload:didReceiveChallenge:completionHandler:")]
		void DidReceiveChallenge (BADownload download, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler);

		[Export ("backgroundDownload:failedWithError:")]
		void Failed (BADownload download, NSError error);

		[Export ("backgroundDownload:finishedWithFileURL:")]
		void Finished (BADownload download, NSUrl fileUrl);

		[Export ("downloadsForRequest:manifestURL:extensionInfo:")]
		NSSet<BADownload> GetDownloads (BAContentRequest contentRequest, NSUrl manifestUrl, BAAppExtensionInfo extensionInfo);
	}

	interface IBADownloadManagerDelegate { }

	[TV (18, 4), iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface BADownloadManagerDelegate {
		[Export ("downloadDidBegin:")]
		void DidBegin (BADownload download);

		[Export ("downloadDidPause:")]
		void DidPause (BADownload download);

		[Export ("download:didWriteBytes:totalBytesWritten:totalBytesExpectedToWrite:")]
		void DidWriteBytes (BADownload download, long bytesWritten, long totalBytesWritten, long totalExpectedBytes);

		[Export ("download:didReceiveChallenge:completionHandler:")]
		void DidReceiveChallenge (BADownload download, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler);

		[Export ("download:failedWithError:")]
		void Failed (BADownload download, NSError error);

		[Export ("download:finishedWithFileURL:")]
		void Finished (BADownload download, NSUrl fileUrl);
	}

	[TV (18, 4), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BADownloadManager {
		[Static]
		[Export ("sharedManager", ArgumentSemantic.Strong)]
		BADownloadManager SharedManager { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IBADownloadManagerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("fetchCurrentDownloads:")]
		[return: NullAllowed]
		BADownload [] FetchCurrentDownloads ([NullAllowed] out NSError error);

		[Async]
		[Export ("fetchCurrentDownloadsWithCompletionHandler:")]
		void FetchCurrentDownloads (Action<NSArray<BADownload>, NSError> completionHandler);

		[Export ("scheduleDownload:error:")]
		bool ScheduleDownload (BADownload download, [NullAllowed] out NSError outError);

		[Export ("performWithExclusiveControl:")]
		void PerformWithExclusiveControl (Action<NSError> performHandler);

		[Export ("startForegroundDownload:error:")]
		bool StartForegroundDownload (BADownload download, [NullAllowed] out NSError outError);

		[Export ("cancelDownload:error:")]
		bool CancelDownload (BADownload download, [NullAllowed] out NSError error);

		[MacCatalyst (16, 1), iOS (16, 1)]
		[Export ("performWithExclusiveControlBeforeDate:performHandler:")]
		void PerformWithExclusiveControlBeforeDate (NSDate date, Action<bool, NSError> performHandler);
	}

	[TV (18, 4), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (BADownload), Name = "BAURLDownload")]
	[DisableDefaultCtor]
	interface BAUrlDownload {

		[Field ("BADownloaderPriorityMin")]
		nint MinPriority { get; }

		[Field ("BADownloaderPriorityDefault")]
		nint DefaultPriority { get; }

		[Field ("BADownloaderPriorityMax")]
		nint MaxPriority { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 16, 4)]
		[Deprecated (PlatformName.MacOSX, 13, 3)]
		[Deprecated (PlatformName.MacCatalyst, 16, 4)]
		[Export ("initWithIdentifier:request:applicationGroupIdentifier:")]
		NativeHandle Constructor (string identifier, NSUrlRequest request, string applicationGroupIdentifier);

		[NoTV]
		[Deprecated (PlatformName.iOS, 16, 4)]
		[Deprecated (PlatformName.MacOSX, 13, 3)]
		[Deprecated (PlatformName.MacCatalyst, 16, 4)]
		[Export ("initWithIdentifier:request:applicationGroupIdentifier:priority:")]
		NativeHandle Constructor (string identifier, NSUrlRequest request, string applicationGroupIdentifier, nint priority);

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("initWithIdentifier:request:fileSize:applicationGroupIdentifier:")]
		NativeHandle Constructor (string identifier, NSUrlRequest request, nuint fileSize, string applicationGroupIdentifier);

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("initWithIdentifier:request:essential:fileSize:applicationGroupIdentifier:priority:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, NSUrlRequest request, bool essential, nuint fileSize, string applicationGroupIdentifier, nint priority);
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	[BaseType (typeof (NSObject))]
	interface BAManagedAssetPackDownloadDelegate {
		[Export ("downloadOfAssetPackBegan:")]
		void DownloadBegan (BAAssetPack assetPack);

		[Export ("downloadOfAssetPack:hasProgress:")]
		void DownloadProgress (BAAssetPack assetPack, NSProgress progress);

		[Export ("downloadOfAssetPackPaused:")]
		void DownloadPaused (BAAssetPack assetPack);

		[Export ("downloadOfAssetPackFinished:")]
		void DownloadFinished (BAAssetPack assetPack);

		[Export ("downloadOfAssetPack:failedWithError:")]
		void DownloadFailed (BAAssetPack assetPack, NSError error);
	}

	interface IBAManagedAssetPackDownloadDelegate { }

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface BAManagedDownloaderExtension : BADownloaderExtension {
		[Export ("shouldDownloadAssetPack:")]
		bool ShouldDownload (BAAssetPack assetPack);
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BAAssetPack {
		[Export ("identifier")]
		string Identifier { get; }

		[Export ("downloadSize")]
		nint DownloadSize { get; }

		[Export ("version")]
		nint Version { get; }

		/// <summary>Gets the BCP-47 identifier for the language in which the asset pack is localized.</summary>
		/// <value>The language identifier, or <see langword="null" /> if the asset pack isn't language-specific.</value>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[NullAllowed]
		[Export ("language")]
		string Language { get; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Copy)]
		NSData UserInfo { get; }

		[Export ("download")]
		BADownload Download ();

		[Export ("downloadForContentRequest:")]
		BADownload Download (BAContentRequest contentRequest);

		[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Field ("BAAssetPackIdentifierErrorKey")]
		NSString IdentifierErrorKey { get; }

		/// <summary>Gets the error user-info key whose value contains the asset packs made available successfully.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Field ("BASuccessesErrorKey")]
		NSString SuccessesErrorKey { get; }

		/// <summary>Gets the error user-info key whose value maps unavailable asset packs to their underlying errors.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Field ("BAFailuresErrorKey")]
		NSString FailuresErrorKey { get; }
	}

	delegate void BAAssetPackManagerGetAllAssetPacksCompletionHandler ([NullAllowed] NSSet<BAAssetPack> assetPacks, [NullAllowed] NSError error);
	delegate void BAAssetPackManagerGetAssetPackCompletionHandler ([NullAllowed] BAAssetPack assetPack, [NullAllowed] NSError error);
	delegate void BAAssetPackManagerGetStatusCompletionHandler ([NullAllowed] BAAssetPackStatus status, [NullAllowed] NSError error);
	/// <summary>Completion handler invoked with an asset-pack manifest or an error.</summary>
	/// <param name="manifest">The asset-pack manifest, or <see langword="null" /> if an error occurred.</param>
	/// <param name="error">The error, or <see langword="null" /> if the operation succeeded.</param>
	delegate void BAAssetPackManagerGetManifestCompletionHandler ([NullAllowed] BAAssetPackManifest manifest, [NullAllowed] NSError error);
	/// <summary>Completion handler invoked with the local status of an asset pack.</summary>
	/// <param name="status">The <see cref="BAAssetPackStatus" /> of the asset pack on the local device.</param>
	delegate void BAAssetPackManagerGetLocalStatusCompletionHandler (BAAssetPackStatus status);
	/// <summary>Completion handler invoked with an asset pack's local installation size or an error.</summary>
	/// <param name="size">The installation size in bytes when the operation succeeds.</param>
	/// <param name="error">The error, or <see langword="null" /> if the operation succeeded.</param>
	[TV (27, 2), Mac (27, 2), iOS (27, 2), MacCatalyst (27, 2)]
	delegate void BAAssetPackManagerGetLocalSizeCompletionHandler (long size, [NullAllowed] NSError error);
	/// <summary>Completion handler invoked with the languages used by locally available asset packs.</summary>
	/// <param name="languageIdentifiers">The BCP-47 language identifiers.</param>
	delegate void BAAssetPackManagerGetLocallyAvailableLanguagesCompletionHandler (string [] languageIdentifiers);
	/// <summary>Completion handler invoked after reconciling locally available asset packs with the preferred languages.</summary>
	/// <param name="error">The error, or <see langword="null" /> if the operation succeeded.</param>
	delegate void BAAssetPackManagerReconcilePreferredLanguagesCompletionHandler ([NullAllowed] NSError error);
	delegate void BAAssetPackManagerEnsureLocalAvailabilityCompletionHandler ([NullAllowed] NSError error);
	delegate void BAAssetPackManagerCheckForUpdatesCompletionHandler ([NullAllowed] NSSet<NSString> updatingIdentifiers, [NullAllowed] NSSet<NSString> removedIdentifiers, [NullAllowed] NSError error);
	delegate void BAAssetPackManagerRemoveAssetPackCompletionHandler ([NullAllowed] NSError error);

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BAAssetPackManager {
		[Static]
		[Export ("sharedManager", ArgumentSemantic.Strong)]
		BAAssetPackManager SharedManager { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IBAManagedAssetPackDownloadDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		/// <summary>Gets or sets the BCP-47 identifier for the language whose localized asset packs the system manages automatically.</summary>
		/// <value>The resolved language identifier, or <see langword="null" /> to use the system-wide language preference.</value>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[NullAllowed, Export ("resolvedLanguage", ArgumentSemantic.Copy)]
		string ResolvedLanguage { get; set; }

		/// <summary>Gets the manifest of asset packs that are available to download.</summary>
		/// <param name="completionHandler">A completion handler called with the manifest or an error.</param>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("getManifestWithCompletionHandler:")]
		[Async]
		void GetManifest (BAAssetPackManagerGetManifestCompletionHandler completionHandler);

		[Deprecated (PlatformName.iOS, 27, 0, message: "Use 'GetManifest' and then its 'AssetPacks' property instead.")]
		[Deprecated (PlatformName.MacOSX, 27, 0, message: "Use 'GetManifest' and then its 'AssetPacks' property instead.")]
		[Deprecated (PlatformName.TvOS, 27, 0, message: "Use 'GetManifest' and then its 'AssetPacks' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 27, 0, message: "Use 'GetManifest' and then its 'AssetPacks' property instead.")]
		[Export ("getAllAssetPacksWithCompletionHandler:")]
		[Async]
		void GetAllAssetPacks (BAAssetPackManagerGetAllAssetPacksCompletionHandler completionHandler);

		[Deprecated (PlatformName.iOS, 27, 0, message: "Use 'GetManifest' and then 'BAAssetPackManifest.GetAssetPack' instead.")]
		[Deprecated (PlatformName.MacOSX, 27, 0, message: "Use 'GetManifest' and then 'BAAssetPackManifest.GetAssetPack' instead.")]
		[Deprecated (PlatformName.TvOS, 27, 0, message: "Use 'GetManifest' and then 'BAAssetPackManifest.GetAssetPack' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 27, 0, message: "Use 'GetManifest' and then 'BAAssetPackManifest.GetAssetPack' instead.")]
		[Export ("getAssetPackWithIdentifier:completionHandler:")]
		[Async]
		void GetAssetPack (string assetPackIdentifier, BAAssetPackManagerGetAssetPackCompletionHandler completionHandler);

		[Deprecated (PlatformName.iOS, 26, 4, message: "Use 'GetRelativeStatus' or 'GetLocalStatus' instead.")]
		[Deprecated (PlatformName.MacOSX, 26, 4, message: "Use 'GetRelativeStatus' or 'GetLocalStatus' instead.")]
		[Deprecated (PlatformName.TvOS, 26, 4, message: "Use 'GetRelativeStatus' or 'GetLocalStatus' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 26, 4, message: "Use 'GetRelativeStatus' or 'GetLocalStatus' instead.")]
		[Export ("getStatusOfAssetPackWithIdentifier:completionHandler:")]
		[Async]
		void GetStatus (string assetPackIdentifier, BAAssetPackManagerGetStatusCompletionHandler completionHandler);

		[Export ("ensureLocalAvailabilityOfAssetPack:completionHandler:")]
		[Async]
		void EnsureLocalAvailability (BAAssetPack assetPack, BAAssetPackManagerEnsureLocalAvailabilityCompletionHandler completionHandler);

		[Export ("checkForUpdatesWithCompletionHandler:")]
		[Async (ResultTypeName = "BAAssetPackManagerCheckForUpdatesResult")]
		void CheckForUpdates ([NullAllowed] BAAssetPackManagerCheckForUpdatesCompletionHandler completionHandler);

		[Export ("contentsAtPath:searchingInAssetPackWithIdentifier:options:error:")]
		[return: NullAllowed]
		NSData GetContents (string path, [NullAllowed] string assetPackIdentifier, NSDataReadingOptions options, [NullAllowed] out NSError error);

		/// <summary>Gets the contents of a localized asset file at the specified relative path.</summary>
		/// <param name="path">The relative path to the asset file.</param>
		/// <param name="languageIdentifier">The BCP-47 identifier used to select localized asset packs.</param>
		/// <param name="options">The options used to read the file.</param>
		/// <param name="error">The error, or <see langword="null" /> if the operation succeeded.</param>
		/// <returns>The file contents, or <see langword="null" /> if an error occurred.</returns>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("contentsAtPath:asLocalizedForLanguage:options:error:")]
		[return: NullAllowed]
		NSData GetLocalizedContents (string path, string languageIdentifier, NSDataReadingOptions options, [NullAllowed] out NSError error);

		[Export ("fileDescriptorForPath:searchingInAssetPackWithIdentifier:error:")]
		int GetFileDescriptor (string path, [NullAllowed] string assetPackIdentifier, [NullAllowed] out NSError error);

		/// <summary>Opens a localized asset file and returns its file descriptor.</summary>
		/// <param name="path">The relative path to the asset file.</param>
		/// <param name="languageIdentifier">The BCP-47 identifier used to select localized asset packs.</param>
		/// <param name="error">The error, or <see langword="null" /> if the operation succeeded.</param>
		/// <returns>The file descriptor, or <c>-1</c> if an error occurred. The caller must close a successful descriptor.</returns>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("fileDescriptorForPath:asLocalizedForLanguage:error:")]
		int GetLocalizedFileDescriptor (string path, string languageIdentifier, [NullAllowed] out NSError error);

		[Export ("URLForPath:error:")]
		[return: NullAllowed]
		NSUrl GetUrl (string path, [NullAllowed] out NSError error);

		/// <summary>Gets a URL for an item in localized asset packs.</summary>
		/// <param name="path">The relative path to the item.</param>
		/// <param name="languageIdentifier">The BCP-47 identifier used to select localized asset packs.</param>
		/// <param name="error">The error, or <see langword="null" /> if the operation succeeded.</param>
		/// <returns>The item URL, or <see langword="null" /> if an error occurred. Don't persist the URL beyond the current process.</returns>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("URLForPath:asLocalizedForLanguage:error:")]
		[return: NullAllowed]
		NSUrl GetLocalizedUrl (string path, string languageIdentifier, [NullAllowed] out NSError error);

		[Export ("removeAssetPackWithIdentifier:completionHandler:")]
		[Async]
		void RemoveAssetPack (string assetPackIdentifier, [NullAllowed] BAAssetPackManagerRemoveAssetPackCompletionHandler completionHandler);

		/// <summary>Gets the status of an asset pack relative to the server.</summary>
		/// <param name="assetPack">The <see cref="BAAssetPack" /> to query.</param>
		/// <param name="completionHandler">A completion handler called with the <see cref="BAAssetPackStatus" /> and an optional error.</param>
		[TV (26, 4), Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
		[Export ("getStatusRelativeToAssetPack:completionHandler:")]
		[Async]
		void GetRelativeStatus (BAAssetPack assetPack, BAAssetPackManagerGetStatusCompletionHandler completionHandler);

		/// <summary>Gets the local status of an asset pack.</summary>
		/// <param name="assetPackIdentifier">The identifier of the asset pack to query.</param>
		/// <param name="completionHandler">A completion handler called with the <see cref="BAAssetPackStatus" /> of the asset pack on the local device.</param>
		[TV (26, 4), Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
		[Export ("getLocalStatusOfAssetPackWithIdentifier:completionHandler:")]
		[Async]
		void GetLocalStatus (string assetPackIdentifier, BAAssetPackManagerGetLocalStatusCompletionHandler completionHandler);

		/// <summary>Synchronously checks whether an asset pack is available on the local device.</summary>
		/// <param name="assetPackIdentifier">The identifier of the asset pack to check.</param>
		/// <returns><see langword="true" /> if the asset pack is available locally; otherwise, <see langword="false" />.</returns>
		[TV (26, 4), Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
		[Export ("assetPackIsAvailableLocallyWithIdentifier:")]
		bool IsAssetPackAvailableLocally (string assetPackIdentifier);

		/// <summary>Gets the version number of a locally available asset pack.</summary>
		/// <param name="assetPackIdentifier">The identifier of the asset pack to query.</param>
		/// <param name="error">The error, or <see langword="null" /> if the operation succeeded.</param>
		/// <returns>The asset pack's version number, or <c>-1</c> if an error occurred.</returns>
		[TV (27, 2), Mac (27, 2), iOS (27, 2), MacCatalyst (27, 2)]
		[Export ("localVersionOfAssetPackWithIdentifier:error:")]
		nint GetLocalVersion (string assetPackIdentifier, [NullAllowed] out NSError error);

		/// <summary>Calculates the installation size of a locally available asset pack.</summary>
		/// <param name="assetPackIdentifier">The identifier of the asset pack to query.</param>
		/// <param name="calculationMethod">How to calculate the installation size.</param>
		/// <param name="completionHandler">A completion handler called with the size in bytes or an error.</param>
		[TV (27, 2), Mac (27, 2), iOS (27, 2), MacCatalyst (27, 2)]
		[Export ("getLocalSizeOfAssetPackWithIdentifier:calculationMethod:completionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously calculates the installation size of a locally available asset pack.</summary>
			<param name="assetPackIdentifier">The identifier of the asset pack to query.</param>
			<param name="calculationMethod">How to calculate the installation size.</param>
			<returns>A task that returns the installation size in bytes.</returns>
			""")]
		void GetLocalSize (string assetPackIdentifier, BASizeCalculationMethod calculationMethod, BAAssetPackManagerGetLocalSizeCompletionHandler completionHandler);

		/// <summary>Gets the languages used by localized asset packs that are available locally.</summary>
		/// <param name="completionHandler">A completion handler called with the BCP-47 language identifiers.</param>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("getLocallyAvailableLanguagesWithCompletionHandler:")]
		[Async]
		void GetLocallyAvailableLanguages (BAAssetPackManagerGetLocallyAvailableLanguagesCompletionHandler completionHandler);

		/// <summary>Reconciles locally available asset packs with the current preferred languages.</summary>
		/// <param name="completionHandler">A completion handler called when reconciliation finishes or an error occurs.</param>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("reconcilePreferredLanguagesWithCompletionHandler:")]
		[Async]
		void ReconcilePreferredLanguages (BAAssetPackManagerReconcilePreferredLanguagesCompletionHandler completionHandler);

		/// <summary>Ensures that an asset pack is available locally, optionally requiring the latest version.</summary>
		/// <param name="assetPack">The <see cref="BAAssetPack" /> to make available.</param>
		/// <param name="requireLatestVersion">If <see langword="true" />, checks for updates before making the asset pack available.</param>
		/// <param name="completionHandler">A completion handler called with an optional error when the operation completes.</param>
		[TV (26, 4), Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
		[Export ("ensureLocalAvailabilityOfAssetPack:requireLatestVersion:completionHandler:")]
		[Async]
		void EnsureLocalAvailability (BAAssetPack assetPack, bool requireLatestVersion, BAAssetPackManagerEnsureLocalAvailabilityCompletionHandler completionHandler);

		/// <summary>Ensures that the specified asset packs are available locally.</summary>
		/// <param name="assetPacks">The asset packs to make available.</param>
		/// <param name="completionHandler">A completion handler called when all requested asset packs are available or an error occurs.</param>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("ensureLocalAvailabilityOfAssetPacks:completionHandler:")]
		[Async]
		void EnsureLocalAvailability (NSSet<BAAssetPack> assetPacks, BAAssetPackManagerEnsureLocalAvailabilityCompletionHandler completionHandler);

		/// <summary>Ensures that the latest versions of the specified asset packs are available locally.</summary>
		/// <param name="assetPacks">The asset packs to make available.</param>
		/// <param name="requireLatestVersions">If <see langword="true" />, checks for updates before making the asset packs available.</param>
		/// <param name="completionHandler">A completion handler called when all requested asset packs are available or an error occurs.</param>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("ensureLocalAvailabilityOfAssetPacks:requireLatestVersions:completionHandler:")]
		[Async]
		void EnsureLocalAvailability (NSSet<BAAssetPack> assetPacks, bool requireLatestVersions, BAAssetPackManagerEnsureLocalAvailabilityCompletionHandler completionHandler);
	}

	[TV (26, 0), iOS (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BAAssetPackManifest {
		[Export ("assetPacks", ArgumentSemantic.Copy)]
		NSSet<BAAssetPack> AssetPacks { get; }

		/// <summary>Gets the application's primary language as a BCP-47 identifier.</summary>
		/// <value>The primary language identifier, or <see langword="null" /> if one isn't configured.</value>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[NullAllowed]
		[Export ("primaryLanguage")]
		string PrimaryLanguage { get; }

		/// <summary>Gets the BCP-47 identifiers for languages with localized asset packs in the manifest.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("availableLanguages", ArgumentSemantic.Copy)]
		string [] AvailableLanguages { get; }

		/// <summary>Gets the language whose localized asset packs the system manages automatically.</summary>
		/// <value>The resolved BCP-47 language identifier, or <see langword="null" /> if no localized asset packs are available.</value>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[NullAllowed]
		[Export ("resolvedLanguage")]
		string ResolvedLanguage { get; }

		/// <summary>Gets the asset packs that best match the current preferred languages.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("localizedAssetPacks", ArgumentSemantic.Copy)]
		NSSet<BAAssetPack> LocalizedAssetPacks { get; }

		/// <summary>Create a new <see cref="BAAssetPackManifest" /> for the specified file on disk.</summary>
		/// <param name="url">The url of the file on disk. The file is expected to be formatted as json.</param>
		/// <param name="applicationGroupIdentifier">The identifier for the application group where the downloaded assets will be stored.</param>
		/// <param name="error">The error if an error occurred.</param>
		/// <returns>A new <see cref="BAAssetPackManifest" /> if the operation succeeded, <see langword="null" /> otherwise.</returns>
		[FactoryMethod]
		[Export ("initWithContentsOfURL:applicationGroupIdentifier:error:")]
		[return: NullAllowed]
		NativeHandle Constructor (NSUrl url, string applicationGroupIdentifier, [NullAllowed] out NSError error);

		/// <summary>Create a new <see cref="BAAssetPackManifest" /> for the specified json data in memory.</summary>
		/// <param name="data">The json data to use.</param>
		/// <param name="applicationGroupIdentifier">The identifier for the application group where the downloaded assets will be stored.</param>
		/// <param name="error">The error if an error occurred.</param>
		/// <returns>A new <see cref="BAAssetPackManifest" /> if the operation succeeded, <see langword="null" /> otherwise.</returns>
		[FactoryMethod]
		[Export ("initFromData:applicationGroupIdentifier:error:")]
		[return: NullAllowed]
		NativeHandle Constructor (NSData data, string applicationGroupIdentifier, [NullAllowed] out NSError error);

		/// <summary>Gets the asset pack with the specified identifier.</summary>
		/// <param name="assetPackIdentifier">The asset-pack identifier.</param>
		/// <returns>The matching asset pack, or <see langword="null" /> if the manifest doesn't contain it.</returns>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("assetPackWithIdentifier:")]
		[return: NullAllowed]
		BAAssetPack GetAssetPack (string assetPackIdentifier);

		/// <summary>Gets the asset packs that best match the specified language.</summary>
		/// <param name="languageIdentifier">The BCP-47 language identifier.</param>
		/// <returns>The localized asset packs.</returns>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("localizedAssetPacksForLanguage:")]
		NSSet<BAAssetPack> GetLocalizedAssetPacks (string languageIdentifier);

		[Export ("allDownloads")]
		NSSet<BADownload> GetAllDownloads ();

		// -(NSSet<BADownload *> * _Nonnull)allDownloadsForContentRequest:(BAContentRequest)contentRequest;
		[Export ("allDownloadsForContentRequest:")]
		NSSet<BADownload> GetAllDownloads (BAContentRequest contentRequest);
	}

}
