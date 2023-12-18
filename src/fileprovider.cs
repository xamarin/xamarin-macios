//
// FileProvider C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//	Sebastien Pouliot  <sebastien.pouliot@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
// Copyright 2019 Microsoft Corporation
//

using System;
using ObjCRuntime;
using CoreGraphics;
using Foundation;
using UniformTypeIdentifiers;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if IOS && !NET && !__MACCATALYST__
using FileProvider;

// This is the original (iOS 8) location of `NSFileProviderExtension`
// but it moved into it's own framework later (iOS 11) and it's now
// shared with macOS...
namespace UIKit {
#else
namespace FileProvider {
#endif
	delegate void NSFileProviderExtensionFetchThumbnailsHandler (NSString identifier, [NullAllowed] NSData imageData, [NullAllowed] NSError error);

	[NoWatch]
	[NoTV]
	[NoMac]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[ThreadSafe]
	[BaseType (typeof (NSObject))]
	partial interface NSFileProviderExtension {

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSFileProviderManager' instead.")]
		[Static, Export ("writePlaceholderAtURL:withMetadata:error:")]
		bool WritePlaceholder (NSUrl placeholderUrl, NSDictionary metadata, ref NSError error);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager.GetPlaceholderUrl (NSUrl)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSFileProviderManager.GetPlaceholderUrl (NSUrl)' instead.")]
		[Static, Export ("placeholderURLForURL:")]
		NSUrl GetPlaceholderUrl (NSUrl url);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager.ProviderIdentifier' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSFileProviderManager.ProviderIdentifier' instead.")]
		[Export ("providerIdentifier")]
		string ProviderIdentifier { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager.DocumentStorageUrl' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSFileProviderManager.DocumentStorageUrl' instead.")]
		[Export ("documentStorageURL")]
		NSUrl DocumentStorageUrl { get; }

		[return: NullAllowed]
		[Export ("URLForItemWithPersistentIdentifier:")]
		NSUrl GetUrlForItem (string persistentIdentifier);

		[return: NullAllowed]
		[Export ("persistentIdentifierForItemAtURL:")]
		string GetPersistentIdentifier (NSUrl itemUrl);

		[Export ("providePlaceholderAtURL:completionHandler:")]
		[Async]
		void ProvidePlaceholderAtUrl (NSUrl url, Action<NSError> completionHandler);

		[Export ("startProvidingItemAtURL:completionHandler:")]
		[Async]
		void StartProvidingItemAtUrl (NSUrl url, Action<NSError> completionHandler);

		[Export ("itemChangedAtURL:")]
		void ItemChangedAtUrl (NSUrl url);

		[Export ("stopProvidingItemAtURL:")]
		void StopProvidingItemAtUrl (NSUrl url);

		[Export ("itemForIdentifier:error:")]
		[return: NullAllowed]
		INSFileProviderItem GetItem (NSString identifier, out NSError error);

		// Inlining NSFileProviderExtension (NSFileProviderActions) so we get asyncs

		[Async]
		[Export ("importDocumentAtURL:toParentItemIdentifier:completionHandler:")]
		void ImportDocument (NSUrl fileUrl, string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[Async]
		[Export ("createDirectoryWithName:inParentItemIdentifier:completionHandler:")]
		void CreateDirectory (string directoryName, string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[Async]
		[Export ("renameItemWithIdentifier:toName:completionHandler:")]
		void RenameItem (string itemIdentifier, string itemName, Action<INSFileProviderItem, NSError> completionHandler);

		[Async]
		[Export ("reparentItemWithIdentifier:toParentItemWithIdentifier:newName:completionHandler:")]
		void ReparentItem (string itemIdentifier, string parentItemIdentifier, [NullAllowed] string newName, Action<INSFileProviderItem, NSError> completionHandler);

		[Async]
		[Export ("trashItemWithIdentifier:completionHandler:")]
		void TrashItem (string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[Async]
		[Export ("untrashItemWithIdentifier:toParentItemIdentifier:completionHandler:")]
		void UntrashItem (string itemIdentifier, [NullAllowed] string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[Async]
		[Export ("deleteItemWithIdentifier:completionHandler:")]
		void DeleteItem (string itemIdentifier, Action<NSError> completionHandler);

		[Async]
		[Export ("setLastUsedDate:forItemIdentifier:completionHandler:")]
		void SetLastUsedDate ([NullAllowed] NSDate lastUsedDate, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[Async]
		[Export ("setTagData:forItemIdentifier:completionHandler:")]
		void SetTagData ([NullAllowed] NSData tagData, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[Async]
		[Export ("setFavoriteRank:forItemIdentifier:completionHandler:")]
		void SetFavoriteRank ([NullAllowed] NSNumber favoriteRank, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		#region NSFileProviderEnumeration (NSFileProviderExtension)
		[Export ("enumeratorForContainerItemIdentifier:error:")]
		[return: NullAllowed]
		INSFileProviderEnumerator GetEnumerator (string containerItemIdentifier, out NSError error);
		#endregion

		// From NSFileProviderExtension (NSFileProviderThumbnailing)

		[Export ("fetchThumbnailsForItemIdentifiers:requestedSize:perThumbnailCompletionHandler:completionHandler:")]
		[Async]
		NSProgress FetchThumbnails (NSString [] itemIdentifiers, CGSize size, NSFileProviderExtensionFetchThumbnailsHandler perThumbnailCompletionHandler, Action<NSError> completionHandler);

		// From NSFileProviderExtension (NSFileProviderService)

		[Export ("supportedServiceSourcesForItemIdentifier:error:")]
		[return: NullAllowed]
		INSFileProviderServiceSource [] GetSupportedServiceSources (string itemIdentifier, out NSError error);

		// From NSFileProviderExtension (NSFileProviderDomain)

		[NullAllowed, Export ("domain")]
		NSFileProviderDomain Domain { get; }
	}
}

namespace FileProvider {

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[ErrorDomain ("NSFileProviderErrorDomain")]
	[Native ("NSFileProviderErrorCode")]
	enum NSFileProviderError : long {
		NotAuthenticated = -1000,
		FilenameCollision = -1001,
		SyncAnchorExpired = -1002,
		PageExpired = SyncAnchorExpired,
		InsufficientQuota = -1003,
		ServerUnreachable = -1004,
		NoSuchItem = -1005,
		VersionOutOfDate = -1006,
		DirectoryNotEmpty = -1007,
		ProviderNotFound = -2001,
		ProviderTranslocated = -2002,
		OlderExtensionVersionRunning = -2003,
		NewerExtensionVersionFound = -2004,
		CannotSynchronize = -2005,
		NonEvictableChildren = -2006,
		UnsyncedEdits = -2007,
		NonEvictable = -2008,
		VersionNoLongerAvailable = -2009,
		ExcludedFromSync = -2010,
		DomainDisabled = -2011,
	}

	[iOS (16, 0), Mac (12, 0), NoMacCatalyst]
	[Native]
	public enum NSFileProviderDomainRemovalMode : long {
		RemoveAll = 0,
		[NoiOS, NoMacCatalyst]
		PreserveDirtyUserData = 1,
		[NoiOS, NoMacCatalyst]
		PreserveDownloadedUserData = 2,
	}

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[Static]
	interface NSFileProviderErrorKeys {

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSFileProviderErrorItemKey' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSFileProviderErrorItemKey' instead.")]
		[Field ("NSFileProviderErrorCollidingItemKey")]
		NSString CollidingItemKey { get; }

		[Field ("NSFileProviderErrorNonExistentItemIdentifierKey")]
		NSString NonExistentItemIdentifierKey { get; }

		[iOS (15, 0)]
		[Field ("NSFileProviderErrorItemKey")]
		NSString ItemKey { get; }
	}

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[Static]
	interface NSFileProviderFavoriteRank {

		[Field ("NSFileProviderFavoriteRankUnranked")]
		ulong Unranked { get; }
	}

	[Mac (11, 0)]
	[NoMacCatalyst]
	[Static]
	interface NSFileProviderItemIdentifier {

		[Field ("NSFileProviderRootContainerItemIdentifier")]
		NSString RootContainer { get; }

		[Field ("NSFileProviderWorkingSetContainerItemIdentifier")]
		NSString WorkingSetContainer { get; }

		[iOS (16, 0)]
		[Field ("NSFileProviderTrashContainerItemIdentifier")]
		NSString TrashContainer { get; }
	}

	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	enum NSFileProviderItemCapabilities : ulong {
		Reading = 1 << 0,
		Writing = 1 << 1,
		Reparenting = 1 << 2,
		Renaming = 1 << 3,
		Trashing = 1 << 4,
		Deleting = 1 << 5,
		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		Evicting = 1 << 6,
		[NoiOS]
		[NoTV]
		[NoWatch]
		[Mac (11, 3)]
		[NoMacCatalyst]
		ExcludingFromSync = 1 << 7,
		AddingSubItems = Writing,
		ContentEnumerating = Reading,
#if !NET
		[Obsolete ("This enum value is not constant across OS and versions.")]
#if MONOMAC
		All = Reading | Writing | Reparenting | Renaming | Trashing | Deleting | Evicting,
#else
		All = Reading | Writing | Reparenting | Renaming | Trashing | Deleting,
#endif
#endif
	}

	[Flags, NoWatch, NoTV, NoMacCatalyst, NoiOS, Mac (12, 3)]
	[Native]
	public enum NSFileProviderMaterializationFlags : ulong {
		KnownSparseRanges = 1uL << 0,
	}

	[Flags, NoWatch, NoTV, NoMacCatalyst, NoiOS, Mac (12, 3)]
	[Native]
	public enum NSFileProviderFetchContentsOptions : ulong {
		StrictVersioning = 1uL << 0,
	}

	[Native]
	[iOS (16, 0), Mac (13, 0), NoWatch, NoTV, NoMacCatalyst]
	public enum NSFileProviderContentPolicy : long {
		Inherited,
		[NoiOS, NoMacCatalyst]
		DownloadLazily,
		DownloadLazilyAndEvictOnRemoteUpdate,
		[NoiOS, NoMacCatalyst]
		DownloadEagerlyAndKeepDownloaded,
	}

	[Mac (11, 0)]
	[NoMacCatalyst]
	[Static]
	interface NSFileProviderPage {

		[Internal]
		[Field ("NSFileProviderInitialPageSortedByName")]
		IntPtr _InitialPageSortedByName { get; }

		[Static]
		[Wrap ("Runtime.GetNSObject<NSData> (_InitialPageSortedByName)")]
		NSData InitialPageSortedByName { get; }

		[Internal]
		[Field ("NSFileProviderInitialPageSortedByDate")]
		IntPtr _InitialPageSortedByDate { get; }

		[Static]
		[Wrap ("Runtime.GetNSObject<NSData> (_InitialPageSortedByDate)")]
		NSData InitialPageSortedByDate { get; }
	}

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NSFileProviderDomain {

		[NoMac]
		[Export ("initWithIdentifier:displayName:pathRelativeToDocumentStorage:")]
		NativeHandle Constructor (string identifier, string displayName, string pathRelativeToDocumentStorage);

		[iOS (16, 0)]
		[Export ("initWithIdentifier:displayName:")]
		NativeHandle Constructor (string identifier, string displayName);

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[NoMac]
		[Export ("pathRelativeToDocumentStorage")]
		string PathRelativeToDocumentStorage { get; }

		[NoiOS]
		[Export ("disconnected")]
		bool Disconnected { [Bind ("isDisconnected")] get; }

		[iOS (16, 0)]
		[Export ("userEnabled")]
		bool UserEnabled { get; }

		[NoiOS]
		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[NoMacCatalyst]
		[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
		[Export ("testingModes", ArgumentSemantic.Assign)]
		NSFileProviderDomainTestingModes TestingModes { get; set; }

		[iOS (16, 0)]
		[Notification]
		[Field ("NSFileProviderDomainDidChange")]
		NSString DidChange { get; }

		[NoWatch, NoTV, iOS (16, 0), Mac (12, 0), NoMacCatalyst]
		[NullAllowed, Export ("backingStoreIdentity")]
		NSData BackingStoreIdentity { get; }

		[NoWatch, NoTV, NoMacCatalyst, Mac (13, 0), iOS (16, 0)]
		[Export ("replicated")]
		bool Replicated { [Bind ("isReplicated")] get; }

		[NoWatch, NoTV, NoMacCatalyst, NoiOS, Mac (13, 0)]
		[Export ("supportsSyncingTrash")]
		bool SupportsSyncingTrash { get; set; }

		[NoWatch, NoTV, NoMacCatalyst, Mac (13, 3), iOS (16, 4)]
		[NullAllowed, Export ("volumeUUID")]
		NSUuid VolumeUuid { get; }
	}

	interface INSFileProviderEnumerationObserver { }

	[NoMacCatalyst]
	[Protocol]
	interface NSFileProviderEnumerationObserver {

		[Abstract]
		[Export ("didEnumerateItems:")]
		void DidEnumerateItems (INSFileProviderItem [] updatedItems);

		[Abstract]
		[Export ("finishEnumeratingUpToPage:")]
		void FinishEnumerating ([NullAllowed] NSData upToPage);

		[Abstract]
		[Export ("finishEnumeratingWithError:")]
		void FinishEnumerating (NSError error);

		[Mac (11, 0)]
		[iOS (16, 0)]
		[Export ("suggestedPageSize")]
		nint GetSuggestedPageSize ();
	}

	interface INSFileProviderChangeObserver { }

	[NoMacCatalyst]
	[Protocol]
	interface NSFileProviderChangeObserver {

		[Abstract]
		[Export ("didUpdateItems:")]
		void DidUpdateItems (INSFileProviderItem [] updatedItems);

		[Abstract]
		[Export ("didDeleteItemsWithIdentifiers:")]
		void DidDeleteItems (string [] deletedItemIdentifiers);

		[Abstract]
		[Export ("finishEnumeratingChangesUpToSyncAnchor:moreComing:")]
		void FinishEnumeratingChanges (NSData anchor, bool moreComing);

		[Abstract]
		[Export ("finishEnumeratingWithError:")]
		void FinishEnumerating (NSError error);

		[Mac (11, 0)]
		[iOS (16, 0)]
		[Export ("suggestedBatchSize")]
		nint GetSuggestedBatchSize ();
	}

	interface INSFileProviderEnumerator { }

	[NoMacCatalyst]
	[Protocol]
	interface NSFileProviderEnumerator {

		[Abstract]
		[Export ("invalidate")]
		void Invalidate ();

		[Abstract]
		[Export ("enumerateItemsForObserver:startingAtPage:")]
		void EnumerateItems (INSFileProviderEnumerationObserver observer, NSData startPage);

		[Export ("enumerateChangesForObserver:fromSyncAnchor:")]
		void EnumerateChanges (INSFileProviderChangeObserver observer, NSData syncAnchor);

		[Export ("currentSyncAnchorWithCompletionHandler:")]
		void CurrentSyncAnchor (Action<NSData> completionHandler);
	}

	interface INSFileProviderItem { }

	[NoMacCatalyst]
	[Protocol]
	interface NSFileProviderItem {

		[Abstract]
		[Export ("itemIdentifier")]
		string Identifier { get; }

		[Abstract]
		[Export ("parentItemIdentifier")]
		string ParentIdentifier { get; }

		[Abstract]
		[Export ("filename")]
		string Filename { get; }

#if !NET
		// became optional when deprecated
		[Abstract]
#endif
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'GetContentType' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'GetContentType' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'GetContentType' instead.")]
		[Export ("typeIdentifier")]
		string TypeIdentifier { get; }

		[iOS (14, 0)]
		[Mac (11, 0)]
		[Export ("contentType", ArgumentSemantic.Copy)]
		UTType GetContentType ();

		[Export ("capabilities")]
		NSFileProviderItemCapabilities GetCapabilities ();

		[return: NullAllowed]
		[Export ("documentSize", ArgumentSemantic.Copy)]
		NSNumber GetDocumentSize ();

		[return: NullAllowed]
		[Export ("childItemCount", ArgumentSemantic.Copy)]
		NSNumber GetChildItemCount ();

		[return: NullAllowed]
		[Export ("creationDate", ArgumentSemantic.Copy)]
		NSDate GetCreationDate ();

		[return: NullAllowed]
		[Export ("contentModificationDate", ArgumentSemantic.Copy)]
		NSDate GetContentModificationDate ();

		[return: NullAllowed]
		[Export ("lastUsedDate", ArgumentSemantic.Copy)]
		NSDate GetLastUsedDate ();

		[return: NullAllowed]
		[Export ("tagData", ArgumentSemantic.Copy)]
		NSData GetTagData ();

		[NoMac]
		[return: NullAllowed]
		[Export ("favoriteRank", ArgumentSemantic.Copy)]
		NSNumber GetFavoriteRank ();

#if NET // Not available in mac
		[NoMac]
#elif MONOMAC
		[Obsolete ("'IsTrashed' is not available in macOS and will be removed in the future.")]
#endif
		[Export ("isTrashed")]
		bool IsTrashed ();

		[Export ("isUploaded")]
		bool IsUploaded ();

		[Export ("isUploading")]
		bool IsUploading ();

		[return: NullAllowed]
		[Export ("uploadingError", ArgumentSemantic.Copy)]
		NSError GetUploadingError ();

		[Export ("isDownloaded")]
		bool IsDownloaded ();

		[Export ("isDownloading")]
		bool IsDownloading ();

		[return: NullAllowed]
		[Export ("downloadingError", ArgumentSemantic.Copy)]
		NSError GetDownloadingError ();

		[Export ("isMostRecentVersionDownloaded")]
		bool IsMostRecentVersionDownloaded ();

		[Export ("isShared")]
		bool IsShared ();

		[Export ("isSharedByCurrentUser")]
		bool IsSharedByCurrentUser ();

		[return: NullAllowed]
		[Export ("ownerNameComponents")]
		NSPersonNameComponents GetOwnerNameComponents ();

		[return: NullAllowed]
		[Export ("mostRecentEditorNameComponents")]
		NSPersonNameComponents GetMostRecentEditorNameComponents ();

		[NoMac]
		[return: NullAllowed]
		[Export ("versionIdentifier")]
		NSData GetVersionIdentifier ();

		[return: NullAllowed]
		[Export ("userInfo")]
		NSDictionary GetUserInfo ();

		[iOS (16, 0), NoMacCatalyst]
		[Export ("fileSystemFlags")]
		NSFileProviderFileSystemFlags FileSystemFlags { get; }

		[iOS (16, 0)]
		[Export ("extendedAttributes", ArgumentSemantic.Strong)]
		NSDictionary<NSString, NSData> ExtendedAttributes { get; }

		[iOS (16, 0)]
		[NullAllowed, Export ("itemVersion", ArgumentSemantic.Strong)]
		NSFileProviderItemVersion ItemVersion { get; }

		[iOS (16, 0)]
		[NullAllowed, Export ("symlinkTargetPath")]
		string SymlinkTargetPath { get; }

		[iOS (16, 0), Mac (12, 0), NoMacCatalyst]
		[Export ("typeAndCreator")]
		NSFileProviderTypeAndCreator TypeAndCreator { get; }

		[NoWatch, NoTV, NoMacCatalyst, Mac (13, 0), iOS (16, 0)]
		[Export ("contentPolicy")]
		NSFileProviderContentPolicy ContentPolicy { get; }
	}

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSFileProviderManager {

		[NoMac]
		[Static]
		[Export ("defaultManager", ArgumentSemantic.Strong)]
		NSFileProviderManager DefaultManager { get; }

		[Export ("signalEnumeratorForContainerItemIdentifier:completionHandler:")]
		// Not Async'ified on purpose, because this can switch from app to extension.
		void SignalEnumerator (string containerItemIdentifier, Action<NSError> completion);

		// Not Async'ified on purpose, because the task must be accesed while the completion action is performing...
		[Export ("registerURLSessionTask:forItemWithIdentifier:completionHandler:")]
		void Register (NSUrlSessionTask task, string identifier, Action<NSError> completion);

		[NoMac]
		[Export ("providerIdentifier")]
		string ProviderIdentifier { get; }

		[NoMac]
		[Export ("documentStorageURL")]
		NSUrl DocumentStorageUrl { get; }

		[NoMac]
		[Static]
		[Export ("writePlaceholderAtURL:withMetadata:error:")]
		bool WritePlaceholder (NSUrl placeholderUrl, INSFileProviderItem metadata, out NSError error);

		[NoMac]
		[Static]
		[Export ("placeholderURLForURL:")]
		NSUrl GetPlaceholderUrl (NSUrl url);

		[Static]
		[Async]
		[Export ("addDomain:completionHandler:")]
		void AddDomain (NSFileProviderDomain domain, Action<NSError> completionHandler);

		[Static]
		[Async]
		[Export ("removeDomain:completionHandler:")]
		void RemoveDomain (NSFileProviderDomain domain, Action<NSError> completionHandler);

		[Static]
		[Async]
		[Export ("getDomainsWithCompletionHandler:")]
		void GetDomains (Action<NSFileProviderDomain [], NSError> completionHandler);

		[Static]
		[Async]
		[Export ("removeAllDomainsWithCompletionHandler:")]
		void RemoveAllDomains (Action<NSError> completionHandler);

		[Static]
		[Export ("managerForDomain:")]
		[return: NullAllowed]
		NSFileProviderManager FromDomain (NSFileProviderDomain domain);

		[iOS (16, 0)]
		[Static]
		[Async (ResultTypeName = "NSFileProviderGetIdentifierResult")]
		[Export ("getIdentifierForUserVisibleFileAtURL:completionHandler:")]
		void GetIdentifierForUserVisibleFile (NSUrl url, NSFileProviderGetIdentifierHandler completionHandler);

		[iOS (16, 0)]
		[Async]
		[Export ("getUserVisibleURLForItemIdentifier:completionHandler:")]
		void GetUserVisibleUrl (NSString itemIdentifier, Action<NSUrl, NSError> completionHandler);

		[iOS (16, 0)]
		[Export ("temporaryDirectoryURLWithError:")]
		[return: NullAllowed]
		NSUrl GetTemporaryDirectoryUrl ([NullAllowed] out NSError error);

		[iOS (16, 0)]
		[Async]
		[Export ("signalErrorResolved:completionHandler:")]
		void SignalErrorResolved (NSError error, Action<NSError> completionHandler);

		[Unavailable (PlatformName.MacCatalyst)]
		[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
		[Export ("globalProgressForKind:")]
		NSProgress GetGlobalProgress (NSString kind); // NSString intended.

		[Unavailable (PlatformName.MacCatalyst)]
		[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
		[Field ("NSFileProviderMaterializedSetDidChange")]
		[Notification]
		NSString MaterializedSetDidChange { get; }

		[Unavailable (PlatformName.MacCatalyst)]
		[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
		[Field ("NSFileProviderPendingSetDidChange")]
		[Notification]
		NSString PendingSetDidChange { get; }

		#region Import (NSFileProviderManager)
		[iOS (16, 0)]
		[Static]
		[Async]
		[Export ("importDomain:fromDirectoryAtURL:completionHandler:")]
		void Import (NSFileProviderDomain domain, NSUrl url, Action<NSError> completionHandler);

		[iOS (16, 0)]
		[Async]
		[Export ("reimportItemsBelowItemWithIdentifier:completionHandler:")]
		void ReimportItemsBelowItem (NSString itemIdentifier, Action<NSError> completionHandler);
		#endregion

		#region MaterializedSet (NSFileProviderManager)
		[iOS (16, 0)]
		[Export ("enumeratorForMaterializedItems")]
		INSFileProviderEnumerator GetMaterializedItemsEnumerator ();
		#endregion

		#region Eviction (NSFileProviderManager)
		[iOS (16, 0)]
		[Export ("evictItemWithIdentifier:completionHandler:")]
		[Async]
		void EvictItem (NSString itemIdentifier, Action<NSError> completionHandler);
		#endregion

		#region Disconnection (NSFileProviderManager)
		[NoiOS]
		[Async]
		[Export ("disconnectWithReason:options:completionHandler:")]
		void Disconnect (string localizedReason, NSFileProviderManagerDisconnectionOptions options, Action<NSError> completionHandler);

		[NoiOS]
		[Async]
		[Export ("reconnectWithCompletionHandler:")]
		void Reconnect (Action<NSError> completionHandler);
		#endregion

		#region Barrier (NSFileProviderManager)
		[iOS (16, 0)]
		[Async]
		[Export ("waitForChangesOnItemsBelowItemWithIdentifier:completionHandler:")]
		void WaitForChangesOnItemsBelowItem (string itemIdentifier, Action<NSError> completionHandler);
		#endregion

		#region Stabilization (NSFileProviderManager)
		[NoiOS]
		[Async]
		[Export ("waitForStabilizationWithCompletionHandler:")]
		void WaitForStabilization (Action<NSError> completionHandler);
		#endregion

		[Unavailable (PlatformName.MacCatalyst)]
		[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
		[Export ("enumeratorForPendingItems")]
		INSFileProviderPendingSetEnumerator GetEnumeratorForPendingItems ();

		// From NSFileProviderManager (TestingModeInteractive) Category

		[Unavailable (PlatformName.MacCatalyst)]
		[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
		[Export ("listAvailableTestingOperationsWithError:")]
		[return: NullAllowed]
		INSFileProviderTestingOperation [] ListAvailableTestingOperations ([NullAllowed] out NSError error);

		[Unavailable (PlatformName.MacCatalyst)]
		[NoWatch, NoTV, Mac (11, 3), iOS (16, 0)]
		[Export ("runTestingOperations:error:")]
		[return: NullAllowed]
		NSDictionary<INSFileProviderTestingOperation, NSError> GetRunTestingOperations (INSFileProviderTestingOperation [] operations, [NullAllowed] out NSError error);

		[iOS (16, 0), Mac (12, 0), NoMacCatalyst]
		[Async (ResultTypeName = "NSFileProviderRemoveDomainResult")]
		[Static]
		[Export ("removeDomain:mode:completionHandler:")]
		void RemoveDomain (NSFileProviderDomain domain, NSFileProviderDomainRemovalMode mode, Action<NSUrl, NSError> completionHandler);

		[Async]
		[iOS (16, 0), Mac (13, 0), NoWatch, NoTV, NoMacCatalyst]
		[Export ("getServiceWithName:itemIdentifier:completionHandler:")]
		void GetService (string serviceName, string itemIdentifier, Action<NSFileProviderService, NSError> completionHandler);

		[Async]
		[NoWatch, NoTV, NoMacCatalyst, Mac (13, 0), iOS (16, 0)]
		[Export ("requestModificationOfFields:forItemWithIdentifier:options:completionHandler:")]
		void RequestModification (NSFileProviderItemFields fields, string itemIdentifier, NSFileProviderModifyItemOptions options, Action<NSError> completionHandler);

		[Async]
		[NoWatch, NoTV, NoMacCatalyst, NoiOS, Mac (13, 0)]
		[Export ("requestDownloadForItemWithIdentifier:requestedRange:completionHandler:")]
		void RequestDownload (string itemIdentifier, NSRange rangeToMaterialize, Action<NSError> completionHandler);
	}

	interface INSFileProviderPendingSetEnumerator { }

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Protocol]
	interface NSFileProviderPendingSetEnumerator : NSFileProviderEnumerator {

		[Abstract]
		[NullAllowed, Export ("domainVersion")]
		NSFileProviderDomainVersion DomainVersion { get; }

		[Abstract]
		[Export ("refreshInterval")]
		double RefreshInterval { get; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[NoWatch, NoTV, NoMacCatalyst, Mac (13, 0), iOS (16, 0)]
		[Export ("maximumSizeReached")]
		bool MaximumSizeReached { [Bind ("isMaximumSizeReached")] get; }
	}

	// typedef NSString *NSFileProviderDomainIdentifier NS_EXTENSIBLE_STRING_ENUM
	delegate void NSFileProviderGetIdentifierHandler (/* /NSFileProviderItemIdentifier */ NSString itemIdentifier, /* NSFileProviderDomainIdentifier */ NSString domainIdentifier, NSError error);

	interface INSFileProviderServiceSource { }

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[Protocol]
	interface NSFileProviderServiceSource {

		[Abstract]
		[Export ("serviceName")]
		string ServiceName { get; }

		[Abstract]
		[Export ("makeListenerEndpointAndReturnError:")]
		[return: NullAllowed]
		NSXpcListenerEndpoint MakeListenerEndpoint (out NSError error);

		[NoWatch, NoTV, NoMacCatalyst, Mac (13, 0), iOS (16, 0)]
		[Export ("restricted")]
		bool Restricted { [Bind ("isRestricted")] get; }
	}

	[Mac (11, 0), iOS (16, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[BaseType (typeof (NSObject))]
	interface NSFileProviderItemVersion {

		[Export ("initWithContentVersion:metadataVersion:")]
		NativeHandle Constructor (NSData contentVersion, NSData metadataVersion);

		[Export ("contentVersion")]
		NSData ContentVersion { get; }

		[Export ("metadataVersion")]
		NSData MetadataVersion { get; }

		[NoWatch, NoTV, NoMacCatalyst, iOS (16, 0), Mac (12, 0)]
		[Static]
		[Export ("beforeFirstSyncComponent")]
		NSData BeforeFirstSyncComponent { get; }
	}

	[Mac (11, 0), iOS (16, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[Native]
	[Flags]
	enum NSFileProviderCreateItemOptions : ulong {
		None = 0,
		MayAlreadyExist = 1,
		DeletionConflicted = 2,
	}

	[Mac (11, 0), iOS (16, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[Native]
	[Flags]
	enum NSFileProviderDeleteItemOptions : ulong {
		None = 0,
		Recursive = 1,
	}

	[Mac (11, 0), iOS (16, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[Native]
	[Flags]
	enum NSFileProviderModifyItemOptions : ulong {
		None = 0,
		MayAlreadyExist = 1,
	}

	[Mac (11, 0), iOS (16, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[Native]
	[Flags]
	enum NSFileProviderItemFields : ulong {
		Contents = 1uL << 0,
		Filename = 1uL << 1,
		ParentItemIdentifier = 1uL << 2,
		LastUsedDate = 1uL << 3,
		TagData = 1uL << 4,
		FavoriteRank = 1uL << 5,
		CreationDate = 1uL << 6,
		ContentModificationDate = 1uL << 7,
		FileSystemFlags = 1uL << 8,
		ExtendedAttributes = 1uL << 9,
		[Mac (12, 0)]
		TypeAndCreator = 1uL << 10,
	}

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Native]
	[Flags]
	enum NSFileProviderManagerDisconnectionOptions : ulong {
		None = 0,
		Temporary = 1,
	}

	[iOS (15, 0), Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[Native]
	[Flags]
	enum NSFileProviderFileSystemFlags : ulong {
		UserExecutable = 1uL << 0,
		UserReadable = 1uL << 1,
		UserWritable = 1uL << 2,
		Hidden = 1uL << 3,
		PathExtensionHidden = 1uL << 4,
	}

	[Mac (11, 0), iOS (16, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[BaseType (typeof (NSObject))]
	interface NSFileProviderRequest {

		[Export ("isSystemRequest")]
		bool IsSystemRequest { get; }

		[Export ("isFileViewerRequest")]
		bool IsFileViewerRequest { get; }

		[NullAllowed]
		[Export ("requestingExecutable", ArgumentSemantic.Copy)]
		NSUrl RequestingExecutable { get; }

		[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
		[NoMacCatalyst]
		[NullAllowed, Export ("domainVersion")]
		NSFileProviderDomainVersion DomainVersion { get; }
	}

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[iOS (16, 0)]
	[Protocol]
	interface NSFileProviderCustomAction {

		[Abstract]
		[Export ("performActionWithIdentifier:onItemsWithIdentifiers:completionHandler:")]
		NSProgress PerformAction (string actionIdentifier, string [] itemIdentifiers, Action<NSError> completionHandler);
	}

	interface INSFileProviderEnumerating { }

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[iOS (16, 0)]
	[Protocol]
	interface NSFileProviderEnumerating {

		[Abstract]
		[Export ("enumeratorForContainerItemIdentifier:request:error:")]
		[return: NullAllowed]
		INSFileProviderEnumerator GetEnumerator (string containerItemIdentifier, NSFileProviderRequest request, [NullAllowed] out NSError error);
	}

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[iOS (16, 0)]
	[Protocol]
	interface NSFileProviderIncrementalContentFetching {

		[Abstract]
		[Export ("fetchContentsForItemWithIdentifier:version:usingExistingContentsAtURL:existingVersion:request:completionHandler:")]
		NSProgress FetchContents (string itemIdentifier, [NullAllowed] NSFileProviderItemVersion requestedVersion, NSUrl existingContents, NSFileProviderItemVersion existingVersion, NSFileProviderRequest request, NSFileProviderFetchContentsCompletionHandler completionHandler);
	}

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[iOS (16, 0)]
	[Protocol]
	interface NSFileProviderServicing {

		[Abstract]
		[Export ("supportedServiceSourcesForItemIdentifier:completionHandler:")]
		NSProgress GetSupportedServiceSources (string itemIdentifier, Action<INSFileProviderServiceSource [], NSError> completionHandler);
	}

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[iOS (16, 0)]
	[Protocol]
	interface NSFileProviderThumbnailing {

		[Abstract]
		[Export ("fetchThumbnailsForItemIdentifiers:requestedSize:perThumbnailCompletionHandler:completionHandler:")]
		NSProgress FetchThumbnails (string [] itemIdentifiers, CGSize size, NSFileProviderPerThumbnailCompletionHandler perThumbnailCompletionHandler, Action<NSError> completionHandler);
	}

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	delegate void NSFileProviderPerThumbnailCompletionHandler (NSString identifier, [NullAllowed] NSData imageData, [NullAllowed] NSError error);

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	delegate void NSFileProviderFetchContentsCompletionHandler ([NullAllowed] NSUrl fileContents, [NullAllowed] INSFileProviderItem item, [NullAllowed] NSError error);

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	delegate void NSFileProviderCreateOrModifyItemCompletionHandler ([NullAllowed] INSFileProviderItem item, NSFileProviderItemFields stillPendingFields, bool shouldFetchContent, [NullAllowed] NSError error);

	[Mac (11, 0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[iOS (16, 0)]
	[Protocol]
	[Advice ("Implementation must expose selector 'initWithDomain:' with '.ctor (NSFileProviderDomain)'.")]
	interface NSFileProviderReplicatedExtension : NSFileProviderEnumerating {

		/* see Advice above
		[Abstract]
		[Export ("initWithDomain:")]
		NativeHandle Constructor (NSFileProviderDomain domain);
		*/

		[Abstract]
		[Export ("invalidate")]
		void Invalidate ();

		[Abstract]
		[Export ("itemForIdentifier:request:completionHandler:")]
		NSProgress GetItem (string identifier, NSFileProviderRequest request, Action<INSFileProviderItem, NSError> completionHandler);

		[Abstract]
		[Export ("createItemBasedOnTemplate:fields:contents:options:request:completionHandler:")]
		NSProgress CreateItem (INSFileProviderItem itemTemplate, NSFileProviderItemFields fields, [NullAllowed] NSUrl url, NSFileProviderCreateItemOptions options, NSFileProviderRequest request, NSFileProviderCreateOrModifyItemCompletionHandler completionHandler);

		[Abstract]
		[Export ("fetchContentsForItemWithIdentifier:version:request:completionHandler:")]
		NSProgress FetchContents (string itemIdentifier, [NullAllowed] NSFileProviderItemVersion requestedVersion, NSFileProviderRequest request, NSFileProviderFetchContentsCompletionHandler completionHandler);

		[Abstract]
		[Export ("modifyItem:baseVersion:changedFields:contents:options:request:completionHandler:")]
		NSProgress ModifyItem (INSFileProviderItem item, NSFileProviderItemVersion version, NSFileProviderItemFields changedFields, [NullAllowed] NSUrl newContents, NSFileProviderModifyItemOptions options, NSFileProviderRequest request, NSFileProviderCreateOrModifyItemCompletionHandler completionHandler);

		[Abstract]
		[Export ("deleteItemWithIdentifier:baseVersion:options:request:completionHandler:")]
		NSProgress DeleteItem (string identifier, NSFileProviderItemVersion version, NSFileProviderDeleteItemOptions options, NSFileProviderRequest request, Action<NSError> completionHandler);

		[Export ("importDidFinishWithCompletionHandler:")]
		void ImportDidFinish (Action completionHandler);

		[Export ("materializedItemsDidChangeWithCompletionHandler:")]
		void MaterializedItemsDidChange (Action completionHandler);

		[NoMacCatalyst]
		[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
		[Export ("pendingItemsDidChangeWithCompletionHandler:")]
		void PendingItemsDidChange (Action completionHandler);
	}

	interface INSFileProviderDomainState { }

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Protocol]
	interface NSFileProviderDomainState {

		[Abstract]
		[Export ("domainVersion")]
		NSFileProviderDomainVersion DomainVersion { get; }

		[Abstract]
		[Export ("userInfo", ArgumentSemantic.Strong)]
		NSDictionary UserInfo { get; }
	}

	[NoWatch, NoTV, iOS (15, 0), Mac (12, 0), NoMacCatalyst]
	[Flags]
	[Native]
	public enum NSFileProviderDomainTestingModes : ulong {
		AlwaysEnabled = 1uL << 0,
		Interactive = 1uL << 1,
	}

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSFileProviderDomainVersion : NSSecureCoding {

		[Export ("next")]
		NSFileProviderDomainVersion Next { get; }

		[Export ("compare:")]
		NSComparisonResult Compare (NSFileProviderDomainVersion otherVersion);
	}

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Native]
	public enum NSFileProviderTestingOperationType : long {
		Ingestion = 0,
		Lookup = 1,
		Creation = 2,
		Modification = 3,
		Deletion = 4,
		ContentFetch = 5,
		ChildrenEnumeration = 6,
		CollisionResolution = 7,
	}

	interface INSFileProviderTestingOperation : global::ObjCRuntime.INativeObject { }

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Protocol]
	interface NSFileProviderTestingOperation {

		[Abstract]
		[Export ("type")]
		NSFileProviderTestingOperationType Type { get; }

		[Abstract]
		[return: NullAllowed]
		[Export ("asIngestion")]
		INSFileProviderTestingIngestion GetAsIngestion ();

		[Abstract]
		[return: NullAllowed]
		[Export ("asLookup")]
		INSFileProviderTestingLookup GetAsLookup ();

		[Abstract]
		[return: NullAllowed]
		[Export ("asCreation")]
		INSFileProviderTestingCreation GetAsCreation ();

		[Abstract]
		[return: NullAllowed]
		[Export ("asModification")]
		INSFileProviderTestingModification GetAsModification ();

		[Abstract]
		[return: NullAllowed]
		[Export ("asDeletion")]
		INSFileProviderTestingDeletion GetAsDeletion ();

		[Abstract]
		[return: NullAllowed]
		[Export ("asContentFetch")]
		INSFileProviderTestingContentFetch GetAsContentFetch ();

		[Abstract]
		[return: NullAllowed]
		[Export ("asChildrenEnumeration")]
		INSFileProviderTestingChildrenEnumeration GetAsChildrenEnumeration ();

		[Abstract]
		[return: NullAllowed]
		[Export ("asCollisionResolution")]
		INSFileProviderTestingCollisionResolution GetAsCollisionResolution ();
	}

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Native]
	public enum NSFileProviderTestingOperationSide : ulong {
		Disk = 0,
		FileProvider = 1,
	}

	interface INSFileProviderTestingIngestion { }

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Protocol]
	interface NSFileProviderTestingIngestion : NSFileProviderTestingOperation {

		[Abstract]
		[Export ("side")]
		NSFileProviderTestingOperationSide Side { get; }

		[Abstract]
		[Export ("itemIdentifier")]
		string ItemIdentifier { get; }

		[Abstract]
		[NullAllowed, Export ("item")]
		INSFileProviderItem Item { get; }
	}

	interface INSFileProviderTestingLookup { }

	[NoWatch, NoTV, iOS (16, 0), NoMacCatalyst, Mac (11, 3)]
	[Protocol]
	interface NSFileProviderTestingLookup : NSFileProviderTestingOperation {

		[Abstract]
		[Export ("side")]
		NSFileProviderTestingOperationSide Side { get; }

		[Abstract]
		[Export ("itemIdentifier")]
		string ItemIdentifier { get; }
	}

	interface INSFileProviderTestingCreation { }

	[NoWatch, NoTV, iOS (16, 0), NoMacCatalyst, Mac (11, 3)]
	[Protocol]
	interface NSFileProviderTestingCreation : NSFileProviderTestingOperation {

		[Abstract]
		[Export ("targetSide")]
		NSFileProviderTestingOperationSide TargetSide { get; }

		[Abstract]
		[Export ("sourceItem")]
		INSFileProviderItem SourceItem { get; }

		[Abstract]
		[NullAllowed, Export ("domainVersion")]
		NSFileProviderDomainVersion DomainVersion { get; }
	}

	interface INSFileProviderTestingModification { }

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Protocol]
	interface NSFileProviderTestingModification : NSFileProviderTestingOperation {

		[Abstract]
		[Export ("targetSide")]
		NSFileProviderTestingOperationSide TargetSide { get; }

		[Abstract]
		[Export ("sourceItem")]
		INSFileProviderItem SourceItem { get; }

		[Abstract]
		[Export ("targetItemIdentifier")]
		string TargetItemIdentifier { get; }

		[Abstract]
		[Export ("targetItemBaseVersion")]
		NSFileProviderItemVersion TargetItemBaseVersion { get; }

		[Abstract]
		[Export ("changedFields")]
		NSFileProviderItemFields ChangedFields { get; }

		[Abstract]
		[NullAllowed, Export ("domainVersion")]
		NSFileProviderDomainVersion DomainVersion { get; }
	}

	interface INSFileProviderTestingDeletion { }

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Protocol]
	interface NSFileProviderTestingDeletion : NSFileProviderTestingOperation {

		[Abstract]
		[Export ("targetSide")]
		NSFileProviderTestingOperationSide TargetSide { get; }

		[Abstract]
		[Export ("sourceItemIdentifier")]
		string SourceItemIdentifier { get; }

		[Abstract]
		[Export ("targetItemIdentifier")]
		string TargetItemIdentifier { get; }

		[Abstract]
		[Export ("targetItemBaseVersion")]
		NSFileProviderItemVersion TargetItemBaseVersion { get; }

		[Abstract]
		[NullAllowed, Export ("domainVersion")]
		NSFileProviderDomainVersion DomainVersion { get; }
	}

	interface INSFileProviderTestingContentFetch { }

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Protocol]
	interface NSFileProviderTestingContentFetch : NSFileProviderTestingOperation {

		[Abstract]
		[Export ("side")]
		NSFileProviderTestingOperationSide Side { get; }

		[Abstract]
		[Export ("itemIdentifier")]
		string ItemIdentifier { get; }
	}

	interface INSFileProviderTestingChildrenEnumeration { }

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Protocol]
	interface NSFileProviderTestingChildrenEnumeration : NSFileProviderTestingOperation {

		[Abstract]
		[Export ("side")]
		NSFileProviderTestingOperationSide Side { get; }

		[Abstract]
		[Export ("itemIdentifier")]
		string ItemIdentifier { get; }
	}

	interface INSFileProviderTestingCollisionResolution { }

	[NoMacCatalyst]
	[NoWatch, NoTV, iOS (16, 0), Mac (11, 3)]
	[Protocol]
	interface NSFileProviderTestingCollisionResolution : NSFileProviderTestingOperation {

		[Abstract]
		[Export ("side")]
		NSFileProviderTestingOperationSide Side { get; }

		[Abstract]
		[Export ("renamedItem")]
		INSFileProviderItem RenamedItem { get; }
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[Protocol]
	interface NSFileProviderUserInteractionSuppressing {
		[Abstract]
		[Export ("setInteractionSuppressed:forIdentifier:")]
		void SetInteractionSuppressed (bool suppression, string suppressionIdentifier);

		[Abstract]
		[Export ("isInteractionSuppressedForIdentifier:")]
		bool IsInteractionSuppressed (string suppressionIdentifier);
	}

	interface INSFileProviderPartialContentFetching { }
	delegate void NSFileProviderPartialContentFetchingCompletionHandler (NSUrl fileContents, INSFileProviderItem item, NSRange retrievedRange, NSFileProviderMaterializationFlags flags, NSError error);

	[NoWatch, NoTV, NoMacCatalyst, NoiOS, Mac (12, 3)]
	[Protocol]
	interface NSFileProviderPartialContentFetching {

		[Abstract]
		[Export ("fetchPartialContentsForItemWithIdentifier:version:request:minimalRange:aligningTo:options:completionHandler:")]
		NSProgress FetchPartialContents (string itemIdentifier, NSFileProviderItemVersion requestedVersion, NSFileProviderRequest request, NSRange requestedRange, nuint alignment, NSFileProviderFetchContentsOptions options, NSFileProviderPartialContentFetchingCompletionHandler completionHandler);
	}
}
