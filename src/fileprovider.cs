﻿﻿//
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

#if IOS && !XAMCORE_4_0 && !__MACCATALYST__
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
	[iOS (8,0)]
	[NoMac]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[ThreadSafe]
	[BaseType (typeof (NSObject))]
	partial interface NSFileProviderExtension {

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager' instead.")]
		[Static, Export ("writePlaceholderAtURL:withMetadata:error:")]
		bool WritePlaceholder (NSUrl placeholderUrl, NSDictionary metadata, ref NSError error);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager.GetPlaceholderUrl (NSUrl)' instead.")]
		[Static, Export ("placeholderURLForURL:")]
		NSUrl GetPlaceholderUrl (NSUrl url);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager.ProviderIdentifier' instead.")]
		[Export ("providerIdentifier")]
		string ProviderIdentifier { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager.DocumentStorageUrl' instead.")]
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

		[iOS (11,0)]
		[Export ("itemForIdentifier:error:")]
		[return: NullAllowed]
		INSFileProviderItem GetItem (NSString identifier, out NSError error);

		// Inlining NSFileProviderExtension (NSFileProviderActions) so we get asyncs

		[iOS (11,0)]
		[Async]
		[Export ("importDocumentAtURL:toParentItemIdentifier:completionHandler:")]
		void ImportDocument (NSUrl fileUrl, string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("createDirectoryWithName:inParentItemIdentifier:completionHandler:")]
		void CreateDirectory (string directoryName, string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("renameItemWithIdentifier:toName:completionHandler:")]
		void RenameItem (string itemIdentifier, string itemName, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("reparentItemWithIdentifier:toParentItemWithIdentifier:newName:completionHandler:")]
		void ReparentItem (string itemIdentifier, string parentItemIdentifier, [NullAllowed] string newName, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("trashItemWithIdentifier:completionHandler:")]
		void TrashItem (string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("untrashItemWithIdentifier:toParentItemIdentifier:completionHandler:")]
		void UntrashItem (string itemIdentifier, [NullAllowed] string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("deleteItemWithIdentifier:completionHandler:")]
		void DeleteItem (string itemIdentifier, Action<NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("setLastUsedDate:forItemIdentifier:completionHandler:")]
		void SetLastUsedDate ([NullAllowed] NSDate lastUsedDate, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("setTagData:forItemIdentifier:completionHandler:")]
		void SetTagData ([NullAllowed] NSData tagData, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("setFavoriteRank:forItemIdentifier:completionHandler:")]
		void SetFavoriteRank ([NullAllowed] NSNumber favoriteRank, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

#region NSFileProviderEnumeration (NSFileProviderExtension)
		[iOS (11,0)]
		[Export ("enumeratorForContainerItemIdentifier:error:")]
		[return: NullAllowed]
		INSFileProviderEnumerator GetEnumerator (string containerItemIdentifier, out NSError error);
#endregion

		// From NSFileProviderExtension (NSFileProviderThumbnailing)

		[iOS (11,0)]
		[Export ("fetchThumbnailsForItemIdentifiers:requestedSize:perThumbnailCompletionHandler:completionHandler:")]
		[Async]
		NSProgress FetchThumbnails (NSString [] itemIdentifiers, CGSize size, NSFileProviderExtensionFetchThumbnailsHandler perThumbnailCompletionHandler, Action<NSError> completionHandler);

		// From NSFileProviderExtension (NSFileProviderService)

		[iOS (11,0)]
		[Export ("supportedServiceSourcesForItemIdentifier:error:")]
		[return: NullAllowed]
		INSFileProviderServiceSource [] GetSupportedServiceSources (string itemIdentifier, out NSError error);

		// From NSFileProviderExtension (NSFileProviderDomain)

		[iOS (11,0)]
		[NullAllowed, Export ("domain")]
		NSFileProviderDomain Domain { get; }
	}
}

namespace FileProvider {

	[iOS (11,0)]
	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[ErrorDomain ("NSFileProviderErrorDomain")]
	[Native]
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
	}

	[iOS (11,0)]
	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[Static]
	interface NSFileProviderErrorKeys {

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSFileProviderErrorItemKey' instead.")]
		[Field ("NSFileProviderErrorCollidingItemKey")]
		NSString CollidingItemKey { get; }

		[Field ("NSFileProviderErrorNonExistentItemIdentifierKey")]
		NSString NonExistentItemIdentifierKey { get; }

		[NoiOS]
		[Field ("NSFileProviderErrorItemKey")]
		NSString ItemKey { get; }
	}

	[iOS (11,0)]
	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[Static]
	interface NSFileProviderFavoriteRank {

		[Field ("NSFileProviderFavoriteRankUnranked")]
		ulong Unranked { get; }
	}

	[iOS (11,0)]
	[Mac (11,0)]
	[MacCatalyst (13, 0)]
	[Static]
	interface NSFileProviderItemIdentifier {

		[Field ("NSFileProviderRootContainerItemIdentifier")]
		NSString RootContainer { get; }

		[Field ("NSFileProviderWorkingSetContainerItemIdentifier")]
		NSString WorkingSetContainer { get; }

		[NoiOS]
		[Field ("NSFileProviderTrashContainerItemIdentifier")]
		NSString TrashContainer { get; }
	}

	[iOS (11,0)]
	[Mac (10,15)]
	[MacCatalyst (13, 0)]
	[Native]
	[Flags]
	enum NSFileProviderItemCapabilities : ulong {
		Reading = 1 << 0,
		Writing = 1 << 1,
		Reparenting = 1 << 2,
		Renaming = 1 << 3,
		Trashing = 1 << 4,
		Deleting = 1 << 5,
		[NoiOS][NoTV][NoWatch]
		Evicting = 1 << 6,
		AddingSubItems = Writing,
		ContentEnumerating = Reading,
#if !XAMCORE_4_0
		[Obsolete ("This enum value is not constant across OS and versions.")]
	#if MONOMAC
		All = Reading | Writing | Reparenting | Renaming | Trashing | Deleting | Evicting,
	#else
		All = Reading | Writing | Reparenting | Renaming | Trashing | Deleting,
	#endif
#endif
	}

	[iOS (11,0)]
	[Mac (11,0)]
	[MacCatalyst (13, 0)]
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

	[iOS (11,0)]
	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NSFileProviderDomain {

		[NoMac]
		[Export ("initWithIdentifier:displayName:pathRelativeToDocumentStorage:")]
		IntPtr Constructor (string identifier, string displayName, string pathRelativeToDocumentStorage);

		[NoiOS]
		[Export ("initWithIdentifier:displayName:")]
		IntPtr Constructor (string identifier, string displayName);

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

		[NoiOS]
		[Export ("userEnabled")]
		bool UserEnabled { get; }

		[NoiOS]
		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[NoiOS]
		[Notification]
		[Field ("NSFileProviderDomainDidChange")]
		NSString DidChange { get; }
	}

	interface INSFileProviderEnumerationObserver { }

	[iOS (11,0)]
	[Mac (10,15)]
	[MacCatalyst (13, 0)]
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

		[Mac (11,0)]
		[NoiOS]
		[Export ("suggestedPageSize")]
		nint GetSuggestedPageSize ();
	}

	interface INSFileProviderChangeObserver { }

	[iOS (11,0)]
	[Mac (10,15)]
	[MacCatalyst (13, 0)]
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

		[Mac (11,0)]
		[NoiOS]
		[Export ("suggestedBatchSize")]
		nint GetSuggestedBatchSize ();
	}

	interface INSFileProviderEnumerator { }

	[iOS (11,0)]
	[Mac (10,15)]
	[MacCatalyst (13, 0)]
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

	[iOS (11,0)]
	[Mac (10,15)]
	[MacCatalyst (13, 0)]
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

#if !XAMCORE_4_0
		// became optional when deprecated
		[Abstract]
#endif
		[Deprecated (PlatformName.iOS, 14,0, message: "Use 'GetContentType' instead.")]
		[Deprecated (PlatformName.MacOSX, 11,0, message: "Use 'GetContentType' instead.")]
		[Export ("typeIdentifier")]
		string TypeIdentifier { get; }

		[iOS (14,0)]
		[Mac (11,0)]
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

#if XAMCORE_4_0 // Not available in mac
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

		[NoiOS]
		[Export ("fileSystemFlags")]
		NSFileProviderFileSystemFlags FileSystemFlags { get; }

		[NoiOS]
		[Export ("extendedAttributes", ArgumentSemantic.Strong)]
		NSDictionary<NSString, NSData> ExtendedAttributes { get; }

		[NoiOS]
		[NullAllowed, Export ("itemVersion", ArgumentSemantic.Strong)]
		NSFileProviderItemVersion ItemVersion { get; }

		[NoiOS]
		[NullAllowed, Export ("symlinkTargetPath")]
		string SymlinkTargetPath { get; }
	}

	[iOS (11,0)]
	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
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

		[NoiOS]
		[Static]
		[Async (ResultTypeName = "NSFileProviderGetIdentifierResult")]
		[Export ("getIdentifierForUserVisibleFileAtURL:completionHandler:")]
		void GetIdentifierForUserVisibleFile (NSUrl url, NSFileProviderGetIdentifierHandler completionHandler);

		[NoiOS]
		[Async]
		[Export ("getUserVisibleURLForItemIdentifier:completionHandler:")]
		void GetUserVisibleUrl (NSString itemIdentifier, Action<NSUrl, NSError> completionHandler);

		[NoiOS]
		[Export ("temporaryDirectoryURLWithError:")]
		[return: NullAllowed]
		NSUrl GetTemporaryDirectoryUrl ([NullAllowed] out NSError error);

		[NoiOS]
		[Async]
		[Export ("signalErrorResolved:completionHandler:")]
		void SignalErrorResolved (NSError error, Action<NSError> completionHandler);

#region Import (NSFileProviderManager)
		[NoiOS]
		[Static]
		[Async]
		[Export ("importDomain:fromDirectoryAtURL:completionHandler:")]
		void Import (NSFileProviderDomain domain, NSUrl url, Action<NSError> completionHandler);

		[NoiOS]
		[Async]
		[Export ("reimportItemsBelowItemWithIdentifier:completionHandler:")]
		void ReimportItemsBelowItem (NSString itemIdentifier, Action<NSError> completionHandler);
#endregion

#region MaterializedSet (NSFileProviderManager)
		[NoiOS]
		[Export ("enumeratorForMaterializedItems")]
		INSFileProviderEnumerator GetMaterializedItemsEnumerator ();
#endregion

#region Eviction (NSFileProviderManager)
		[NoiOS]
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
		[NoiOS]
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
	}

	// typedef NSString *NSFileProviderDomainIdentifier NS_EXTENSIBLE_STRING_ENUM
	delegate void NSFileProviderGetIdentifierHandler (/* /NSFileProviderItemIdentifier */ NSString itemIdentifier, /* NSFileProviderDomainIdentifier */ NSString domainIdentifier, NSError error);

	interface INSFileProviderServiceSource {}

	[iOS (11,0)]
	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[Protocol]
	interface NSFileProviderServiceSource {

		[Abstract]
		[Export ("serviceName")]
		string ServiceName { get; }

		[Abstract]
		[Export ("makeListenerEndpointAndReturnError:")]
		[return: NullAllowed]
		NSXpcListenerEndpoint MakeListenerEndpoint (out NSError error);
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[BaseType (typeof(NSObject))]
	interface NSFileProviderItemVersion {

		[Export ("initWithContentVersion:metadataVersion:")]
		IntPtr Constructor (NSData contentVersion, NSData metadataVersion);

		[Export ("contentVersion")]
		NSData ContentVersion { get; }

		[Export ("metadataVersion")]
		NSData MetadataVersion { get; }
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Native][Flags]
	enum NSFileProviderCreateItemOptions : ulong {
		None = 0,
		MayAlreadyExist = 1,
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Native][Flags]
	enum NSFileProviderDeleteItemOptions : ulong {
		None = 0,
		Recursive = 1,
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Native][Flags]
	enum NSFileProviderModifyItemOptions : ulong {
		None = 0,
		MayAlreadyExist = 1,
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Native][Flags]
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
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Native][Flags]
	enum NSFileProviderManagerDisconnectionOptions : ulong {
		None = 0,
		Temporary = 1,
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Native][Flags]
	enum NSFileProviderFileSystemFlags : ulong
	{
		UserExecutable = 1uL << 0,
		UserReadable = 1uL << 1,
		UserWritable = 1uL << 2,
		Hidden = 1uL << 3,
		PathExtensionHidden = 1uL << 4,
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[BaseType (typeof (NSObject))]
	interface NSFileProviderRequest {

		[Export ("isSystemRequest")]
		bool IsSystemRequest { get; }

		[Export ("isFileViewerRequest")]
		bool IsFileViewerRequest { get; }

		[NullAllowed]
		[Export ("requestingExecutable", ArgumentSemantic.Copy)]
		NSUrl RequestingExecutable { get; }
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Protocol]
	interface NSFileProviderCustomAction {

		[Abstract]
		[Export ("performActionWithIdentifier:onItemsWithIdentifiers:completionHandler:")]
		NSProgress PerformAction (string actionIdentifier, string[] itemIdentifiers, Action<NSError> completionHandler);
	}

	interface INSFileProviderEnumerating {}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Protocol]
	interface NSFileProviderEnumerating {

		[Abstract]
		[Export ("enumeratorForContainerItemIdentifier:request:error:")]
		[return: NullAllowed]
		INSFileProviderEnumerator GetEnumerator (string containerItemIdentifier, NSFileProviderRequest request, [NullAllowed] out NSError error);
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Protocol]
	interface NSFileProviderIncrementalContentFetching {

		[Abstract]
		[Export ("fetchContentsForItemWithIdentifier:version:usingExistingContentsAtURL:existingVersion:request:completionHandler:")]
		NSProgress FetchContents (string itemIdentifier, [NullAllowed] NSFileProviderItemVersion requestedVersion, NSUrl existingContents, NSFileProviderItemVersion existingVersion, NSFileProviderRequest request, NSFileProviderFetchContentsCompletionHandler completionHandler);
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Protocol]
	interface NSFileProviderServicing {

		[Abstract]
		[Export ("supportedServiceSourcesForItemIdentifier:completionHandler:")]
		NSProgress GetSupportedServiceSources (string itemIdentifier, Action<INSFileProviderServiceSource[], NSError> completionHandler);
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	[Protocol]
	interface NSFileProviderThumbnailing {

		[Abstract]
		[Export ("fetchThumbnailsForItemIdentifiers:requestedSize:perThumbnailCompletionHandler:completionHandler:")]
		NSProgress FetchThumbnails (string[] itemIdentifiers, CGSize size, NSFileProviderPerThumbnailCompletionHandler perThumbnailCompletionHandler, Action<NSError> completionHandler);
	}

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	delegate void NSFileProviderPerThumbnailCompletionHandler (NSString identifier, [NullAllowed] NSData imageData, [NullAllowed] NSError error);

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	delegate void NSFileProviderFetchContentsCompletionHandler ([NullAllowed] NSUrl fileContents, [NullAllowed] INSFileProviderItem item, [NullAllowed] NSError error);

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using Catalyst on macOS.")]
	[NoiOS]
	delegate void NSFileProviderCreateOrModifyItemCompletionHandler ([NullAllowed] INSFileProviderItem item, NSFileProviderItemFields stillPendingFields, bool shouldFetchContent, [NullAllowed] NSError error);

	[Mac (11,0)]
	[Unavailable (PlatformName.MacCatalyst)]
	[NoiOS]
	[Protocol]
	[Advice ("Implementation must expose selector 'initWithDomain:' with '.ctor (NSFileProviderDomain)'.")]
	interface NSFileProviderReplicatedExtension : NSFileProviderEnumerating {

		/* see Advice above
		[Abstract]
		[Export ("initWithDomain:")]
		IntPtr Constructor (NSFileProviderDomain domain);
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
	}
}
