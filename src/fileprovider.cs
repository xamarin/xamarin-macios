﻿﻿//
// FileProvider C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using ObjCRuntime;
using CoreGraphics;
using Foundation;

namespace FileProvider {

	[iOS (11,0)]
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
	}

	[iOS (11,0)]
	[Static]
	interface NSFileProviderErrorKeys {

		[Field ("NSFileProviderErrorCollidingItemKey")]
		NSString CollidingItemKey { get; }

		[Field ("NSFileProviderErrorNonExistentItemIdentifierKey")]
		NSString NonExistentItemIdentifierKey { get; }
	}

	[iOS (11,0)]
	[Static]
	interface NSFileProviderFavoriteRank {

		[Field ("NSFileProviderFavoriteRankUnranked")]
		ulong Unranked { get; }
	}

	[iOS (11,0)]
	[Static]
	interface NSFileProviderItemIdentifier {

		[Field ("NSFileProviderRootContainerItemIdentifier")]
		NSString RootContainer { get; }

		[Field ("NSFileProviderWorkingSetContainerItemIdentifier")]
		NSString WorkingSetContainer { get; }
	}

	[iOS (11,0)]
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
	}

	interface INSFileProviderEnumerationObserver { }

	[iOS (11,0)]
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

		[return: NullAllowed]
		[Export ("versionIdentifier")]
		NSData GetVersionIdentifier ();

		[return: NullAllowed]
		[Export ("userInfo")]
		NSDictionary GetUserInfo ();
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSFileProviderManager {

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
	}

	interface INSFileProviderServiceSource {}

	[iOS (11,0)]
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
}
#endif
