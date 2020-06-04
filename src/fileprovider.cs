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

#if IOS && !XAMCORE_4_0
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
	[ThreadSafe]
	[BaseType (typeof (NSObject))]
	partial interface NSFileProviderExtension {
		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager' instead.")]
		[Static, Export ("writePlaceholderAtURL:withMetadata:error:")]
		bool WritePlaceholder (NSUrl placeholderUrl, NSDictionary metadata, ref NSError error);

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager.GetPlaceholderUrl (NSUrl)' instead.")]
		[Static, Export ("placeholderURLForURL:")]
		NSUrl GetPlaceholderUrl (NSUrl url);

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager.ProviderIdentifier' instead.")]
		[Export ("providerIdentifier")]
		string ProviderIdentifier { get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager.DocumentStorageUrl' instead.")]
		[Export ("documentStorageURL")]
		NSUrl DocumentStorageUrl { get; }

		[NoMac]
		[return: NullAllowed]
		[Export ("URLForItemWithPersistentIdentifier:")]
		NSUrl GetUrlForItem (string persistentIdentifier);

		[NoMac]
		[return: NullAllowed]
		[Export ("persistentIdentifierForItemAtURL:")]
		string GetPersistentIdentifier (NSUrl itemUrl);

		[NoMac]
		[Export ("providePlaceholderAtURL:completionHandler:")]
		[Async]
		void ProvidePlaceholderAtUrl (NSUrl url, Action<NSError> completionHandler);

		[NoMac]
		[Export ("startProvidingItemAtURL:completionHandler:")]
		[Async]
		void StartProvidingItemAtUrl (NSUrl url, Action<NSError> completionHandler);

		[NoMac]
		[Export ("itemChangedAtURL:")]
		void ItemChangedAtUrl (NSUrl url);

		[NoMac]
		[Export ("stopProvidingItemAtURL:")]
		void StopProvidingItemAtUrl (NSUrl url);

		[iOS (11,0)]
		[Export ("itemForIdentifier:error:")]
		[return: NullAllowed]
		INSFileProviderItem GetItem (NSString identifier, out NSError error);

		// Inlining NSFileProviderExtension (NSFileProviderActions) so we get asyncs

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Import' instead.")]
		[iOS (11,0)]
		[Async]
		[Export ("importDocumentAtURL:toParentItemIdentifier:completionHandler:")]
		void ImportDocument (NSUrl fileUrl, string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'CreateItem' instead.")]
		[iOS (11,0)]
		[Async]
		[Export ("createDirectoryWithName:inParentItemIdentifier:completionHandler:")]
		void CreateDirectory (string directoryName, string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ItemChanged' instead.")]
		[iOS (11,0)]
		[Async]
		[Export ("renameItemWithIdentifier:toName:completionHandler:")]
		void RenameItem (string itemIdentifier, string itemName, Action<INSFileProviderItem, NSError> completionHandler);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ItemChanged' instead.")]
		[iOS (11,0)]
		[Async]
		[Export ("reparentItemWithIdentifier:toParentItemWithIdentifier:newName:completionHandler:")]
		void ReparentItem (string itemIdentifier, string parentItemIdentifier, [NullAllowed] string newName, Action<INSFileProviderItem, NSError> completionHandler);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ItemChanged' instead.")]
		[iOS (11,0)]
		[Async]
		[Export ("trashItemWithIdentifier:completionHandler:")]
		void TrashItem (string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ItemChanged' instead.")]
		[iOS (11,0)]
		[Async]
		[Export ("untrashItemWithIdentifier:toParentItemIdentifier:completionHandler:")]
		void UntrashItem (string itemIdentifier, [NullAllowed] string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'DeleteItem (NSString, NSFileProviderItemVersion, NSFileProviderDeleteItemOptions, Action<NSError>)' instead.")]
		[iOS (11,0)]
		[Async]
		[Export ("deleteItemWithIdentifier:completionHandler:")]
		void DeleteItem (string itemIdentifier, Action<NSError> completionHandler);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ItemChanged' instead.")]
		[iOS (11,0)]
		[Async]
		[Export ("setLastUsedDate:forItemIdentifier:completionHandler:")]
		void SetLastUsedDate ([NullAllowed] NSDate lastUsedDate, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ItemChanged' instead.")]
		[iOS (11,0)]
		[Async]
		[Export ("setTagData:forItemIdentifier:completionHandler:")]
		void SetTagData ([NullAllowed] NSData tagData, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ItemChanged' instead.")]
		[iOS (11,0)]
		[Async]
		[Export ("setFavoriteRank:forItemIdentifier:completionHandler:")]
		void SetFavoriteRank ([NullAllowed] NSNumber favoriteRank, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[NoiOS]
		[Async]
		[Export ("performActionWithIdentifier:onItemsWithIdentifiers:completionHandler:")]
		NSProgress PerformAction (NSString actionIdentifier, NSString[] itemIdentifiers, Action<NSError> completionHandler);

		[NoiOS]
		[Async (ResultTypeName = "NSFileProviderExtensionFetchResult")]
		[Export ("fetchContentsForItemWithIdentifier:version:completionHandler:")]
		NSProgress FetchContents (NSString itemIdentifier, [NullAllowed] NSFileProviderItemVersion requestedVersion, NSFileProviderExtensionFetchHandler completionHandler);

		[NoiOS]
		[Async (ResultTypeName = "NSFileProviderExtensionFetchResult")]
		[Export ("fetchContentsForItemWithIdentifier:version:usingExistingContentsAtURL:existingVersion:completionHandler:")]
		NSProgress FetchContents (NSString itemIdentifier, [NullAllowed] NSFileProviderItemVersion requestedVersion, NSUrl existingContents, NSFileProviderItemVersion existingVersion, NSFileProviderExtensionFetchHandler completionHandler);

		[NoiOS]
		[Async]
		[Export ("itemChanged:baseVersion:changedFields:contents:completionHandler:")]
		void ItemChanged (INSFileProviderItem item, NSFileProviderItemVersion version, NSFileProviderItemField changedFields, [NullAllowed] NSUrl newContents, Action<INSFileProviderItem, NSError> completionHandler);

#region NSFileProviderEnumeration (NSFileProviderExtension)
		[iOS (11,0)]
		[Export ("enumeratorForContainerItemIdentifier:error:")]
		[return: NullAllowed]
		INSFileProviderEnumerator GetEnumerator (string containerItemIdentifier, out NSError error);

		[NoiOS]
		[Export ("enumeratorForSearchQuery:error:")]
		[return: NullAllowed]
		INSFileProviderEnumerator GetEnumerator (NSFileProviderSearchQuery searchQuery, [NullAllowed] out NSError error);
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

#region CreateItem (NSFileProviderExtension)
		[NoiOS]
		[Export ("createItemBasedOnTemplate:fields:contents:options:completionHandler:")]
		[Async]
		void CreateItem (INSFileProviderItem itemTemplate, NSFileProviderItemField fields, [NullAllowed] NSUrl url, NSFileProviderCreateItemOptions options, Action<INSFileProviderItem, NSError> completionHandler);
#endregion

#region DeleteItem (NSFileProviderExtension)
		[NoiOS]
		[Export ("deleteItemWithIdentifier:baseVersion:options:completionHandler:")]
		[Async]
		void DeleteItem (NSString itemIdentifier, NSFileProviderItemVersion version, NSFileProviderDeleteItemOptions options, Action<NSError> completionHandler);
#endregion

#region Import (NSFileProviderExtension)
		[NoiOS]
		[Export ("importDidFinishWithCompletionHandler:")]
		[Async]
		void ImportDidFinish (Action completionHandler);
#endregion

#region MaterializedSet (NSFileProviderExtension)
		[NoiOS]
		[Export ("materializedItemsDidChangeWithCompletionHandler:")]
		[Async]
		void MaterializedItemsDidChange (Action completionHandler);
#endregion

#region Request (NSFileProviderExtension)
		[NoiOS]
		[NullAllowed, Export ("currentRequest")]
		NSFileProviderRequest CurrentRequest { get; }
#endregion
	}

	delegate void NSFileProviderExtensionFetchHandler (NSUrl fileContents, INSFileProviderItem item, NSError error);
}

namespace FileProvider {

	[iOS (11,0)]
	[NoMac]
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
		[NoiOS]
		VersionOutOfDate = -1006,
		[NoiOS]
		DirectoryNotEmpty = -1007,
	}

	[iOS (11,0)]
	[NoMac]
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
	[NoMac]
	[Static]
	interface NSFileProviderFavoriteRank {

		[Field ("NSFileProviderFavoriteRankUnranked")]
		ulong Unranked { get; }
	}

	[iOS (11,0)]
	[NoMac]
	[Static]
	interface NSFileProviderItemIdentifier {

		[Field ("NSFileProviderRootContainerItemIdentifier")]
		NSString RootContainer { get; }

		[Field ("NSFileProviderWorkingSetContainerItemIdentifier")]
		NSString WorkingSetContainer { get; }
	}

	[iOS (11,0)]
	[Mac (10,15)]
	[Native]
	[Flags]
	enum NSFileProviderItemCapabilities : ulong {
		Reading = 1 << 0,
		Writing = 1 << 1,
		Reparenting = 1 << 2,
		Renaming = 1 << 3,
		Trashing = 1 << 4,
		Deleting = 1 << 5,
		AddingSubItems = Writing,
		ContentEnumerating = Reading,
		All = Reading | Writing | Reparenting | Renaming | Trashing | Deleting,
	}

	[iOS (11,0)]
	[NoMac]
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
	[NoMac]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NSFileProviderDomain {

		[Export ("initWithIdentifier:displayName:pathRelativeToDocumentStorage:")]
		IntPtr Constructor (string identifier, string displayName, string pathRelativeToDocumentStorage);

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("pathRelativeToDocumentStorage")]
		string PathRelativeToDocumentStorage { get; }

		[NoiOS]
		[Export ("disconnected")]
		bool Disconnected { [Bind ("isDisconnected")] get; set; }
	}

	interface INSFileProviderEnumerationObserver { }

	[iOS (11,0)]
	[Mac (10,15)]
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
	}

	interface INSFileProviderChangeObserver { }

	[iOS (11,0)]
	[Mac (10,15)]
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
	}

	interface INSFileProviderEnumerator { }

	[iOS (11,0)]
	[Mac (10,15)]
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

		[Abstract]
		[Export ("typeIdentifier")]
		string TypeIdentifier { get; }

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
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'ItemVersion' instead.")]
		[return: NullAllowed]
		[Export ("versionIdentifier")]
		NSData GetVersionIdentifier ();

		[return: NullAllowed]
		[Export ("userInfo")]
		NSDictionary GetUserInfo ();

		[NoiOS]
		[NoMac]
		[Export ("excludedFromSync")]
		bool ExcludedFromSync { [Bind ("isExcludedFromSync")] get; }

		[NoiOS]
		[NoMac]
		[Export ("flags", ArgumentSemantic.Strong)]
		[NullAllowed]
		INSFileProviderItemFlags Flags { get; }

		[NoiOS]
		[NoMac]
		[NullAllowed, Export ("extendedAttributes", ArgumentSemantic.Strong)]
		NSDictionary<NSString, NSData> ExtendedAttributes { get; }

		[NoiOS]
		[NoMac]
		[NullAllowed, Export ("itemVersion", ArgumentSemantic.Strong)]
		NSFileProviderItemVersion ItemVersion { get; }
	}

	[iOS (11,0)]
	[NoMac]
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

		[Export ("providerIdentifier")]
		string ProviderIdentifier { get; }

		[Export ("documentStorageURL")]
		NSUrl DocumentStorageUrl { get; }

		[Static]
		[Export ("writePlaceholderAtURL:withMetadata:error:")]
		bool WritePlaceholder (NSUrl placeholderUrl, INSFileProviderItem metadata, out NSError error);

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

#region DownloadAndEviction (NSFileProviderManager)
		[NoiOS]
		[Export ("evictItemWithIdentifier:completionHandler:")]
		[Async]
		void EvictItem (NSString itemIdentifier, Action<NSError> completionHandler);

		[NoiOS]
		[Export ("setDownloadPolicy:forItemWithIdentifier:completionHandler:")]
		[Async]
		void SetDownloadPolicy (NSFileProviderDownloadPolicy downloadPolicy, NSString itemIdentifier, Action<NSError> completionHandler);
#endregion
	}

	// typedef NSString *NSFileProviderDomainIdentifier NS_EXTENSIBLE_STRING_ENUM
	delegate void NSFileProviderGetIdentifierHandler (/* /NSFileProviderItemIdentifier */ NSString itemIdentifier, /* NSFileProviderDomainIdentifier */ NSString domainIdentifier, NSError error);

	interface INSFileProviderServiceSource {}

	[iOS (11,0)]
	[NoMac]
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

	[NoiOS]
	[NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // the `init*` and properties don't allow null
	interface NSFileProviderItemVersion {

		[Export ("initWithContentVersion:metadataVersion:")]
		IntPtr Constructor (NSData contentVersion, NSData metadataVersion);

		[Export ("contentVersion")]
		NSData ContentVersion { get; }

		[Export ("metadataVersion")]
		NSData MetadataVersion { get; }
	}

	[NoiOS]
	[NoMac]
	[BaseType (typeof (NSObject))]
	interface NSFileProviderRequest {

		[Export ("requestingApplicationIdentifier", ArgumentSemantic.Strong)]
		NSUuid RequestingApplicationIdentifier { get; }
	}

	[NoiOS]
	[NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSFileProviderSearchQuery {

		[NullAllowed, Export ("filename")]
		string Filename { get; }

		[NullAllowed, Export ("allowedContentTypes", ArgumentSemantic.Copy)]
		NSSet<NSString> AllowedContentTypes { get; }

		[NullAllowed, Export ("allowedPathExtensions", ArgumentSemantic.Copy)]
		NSSet<NSString> AllowedPathExtensions { get; }

		[Export ("scopedToItemIdentifier")]
		NSString ScopedToItemIdentifier { get; }

		[Export ("searchContainerItemIdentifier")]
		NSString SearchContainerItemIdentifier { get; }
	}

	[NoiOS]
	[NoMac]
	[Flags]
	[Native]
	enum NSFileProviderCreateItemOptions : ulong {
		None = 0,
		ItemMayAlreadyExist = 1,
	}

	[NoiOS]
	[NoMac]
	[Flags]
	[Native]
	enum NSFileProviderDeleteItemOptions : ulong {
		None = 0,
		Recursive = 1,
	}

	[NoiOS]
	[NoMac]
	[Flags]
	[Native]
	enum NSFileProviderDownloadPolicy : ulong {
		Default = 0,
		Speculative = 1,
		KeepDownloaded = 2,
	}

	[NoiOS]
	[NoMac]
	[Flags]
	[Native]
	enum NSFileProviderItemField : ulong {
		Contents = 1 << 0,
		Filename = 1 << 1,
		ParentItemIdentifier = 1 << 2,
		LastUsedDate = 1 << 3,
		TagData = 1 << 4,
		FavoriteRank = 1 << 5,
		CreationDate = 1 << 6,
		ContentModificationDate = 1 << 7,
		Flags = 1 << 8,
		Trashed = 1 << 9,
		ExtendedAttributes = 1 << 10,
	}

	[iOS (13,0)]
	[NoMac]
	[Protocol]
	interface NSFileProviderItemDecorating : NSFileProviderItem {

		[NoiOS]
		[Abstract]
		[NullAllowed, Export ("decorations", ArgumentSemantic.Strong)]
		string[] Decorations { get; }
	}

	interface INSFileProviderItemFlags {}

	[iOS (13,0)]
	[Mac (10,15)]
	[Protocol]
	interface NSFileProviderItemFlags {

		[Abstract]
		[Export ("userExecutable")]
		bool UserExecutable { [Bind ("isUserExecutable")] get; }

		[Abstract]
		[Export ("userReadable")]
		bool UserReadable { [Bind ("isUserReadable")] get; }

		[Abstract]
		[Export ("userWritable")]
		bool UserWritable { [Bind ("isUserWritable")] get; }

		[Abstract]
		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; }

		[Abstract]
		[Export ("pathExtensionHidden")]
		bool PathExtensionHidden { [Bind ("isPathExtensionHidden")] get; }
	}
}
