//
// Convenience methods for NSMetadataItem
//
// Copyright 2014, 2016 Xamarin Inc
//
// Author:
//   Miguel de Icaza
//
using System;
using XamCore.ObjCRuntime;

namespace XamCore.Foundation {
	public partial class NSMetadataItem {

		bool GetBool (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n == null ? false : n.BoolValue;
		}

		double GetDouble (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n == null ? 0 : n.DoubleValue;
		}

		// same order as NSMetadataAttributes.h

		public NSString FileSystemName {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ItemFSNameKey));
			}
		}

		public NSString DisplayName {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ItemDisplayNameKey));
			}
		}

		public NSUrl Url {
			get {
				return Runtime.GetNSObject<NSUrl> (GetHandle (NSMetadataQuery.ItemURLKey));
			}
		}

		public NSString Path {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ItemPathKey));
			}
		}

		public NSNumber FileSystemSize {
			get {
				return Runtime.GetNSObject<NSNumber> (GetHandle (NSMetadataQuery.ItemFSSizeKey));
			}
		}

		public NSDate FileSystemCreationDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.ItemFSCreationDateKey));
			}
		}

		public NSDate FileSystemContentChangeDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.ItemFSContentChangeDateKey));
			}
		}

		[iOS (8,0)][Mac (10,9)]
		public NSString ContentType {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ContentTypeKey));
			}
		}

		[iOS (8,0)][Mac (10,9)]
		public NSString[] ContentTypeTree {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.ContentTypeTreeKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		public bool IsUbiquitous {
			get {
				return GetBool (NSMetadataQuery.ItemIsUbiquitousKey);
			}
		}

		public bool UbiquitousItemHasUnresolvedConflicts {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemHasUnresolvedConflictsKey);
			}
		}

		[iOS (7,0)][Mac (10,9)]
#if XAMCORE_4_0
		public NSItemDownloadingStatus UbiquitousItemDownloadingStatus {
#else
		public NSItemDownloadingStatus DownloadingStatus {
#endif
			get {
				return NSItemDownloadingStatusExtensions.GetValue (Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.UbiquitousItemDownloadingStatusKey)));
			}
		}

		public bool UbiquitousItemIsDownloading {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemIsDownloadingKey);
			}
		}

		public bool UbiquitousItemIsUploaded {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemIsUploadedKey);
			}
		}

		public bool UbiquitousItemIsUploading {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemIsUploadingKey);
			}
		}

		public double UbiquitousItemPercentDownloaded {
			get {
				return GetDouble (NSMetadataQuery.UbiquitousItemPercentDownloadedKey);
			}
		}

		public double UbiquitousItemPercentUploaded {
			get {
				return GetDouble (NSMetadataQuery.UbiquitousItemPercentUploadedKey);
			}
		}

		[iOS (7,0)][Mac (10,9)]
		public NSError UbiquitousItemDownloadingError {
			get {
				return Runtime.GetNSObject<NSError> (GetHandle (NSMetadataQuery.UbiquitousItemDownloadingErrorKey));
			}
		}

		[iOS (7,0)][Mac (10,9)]
		public NSError UbiquitousItemUploadingError {
			get {
				return Runtime.GetNSObject<NSError> (GetHandle (NSMetadataQuery.UbiquitousItemUploadingErrorKey));
			}
		}

		[iOS (8,0)][Mac (10,10)]
		public bool UbiquitousItemDownloadRequested {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemDownloadRequestedKey);
			}
		}

		[iOS (8,0)][Mac (10,10)]
		public bool UbiquitousItemIsExternalDocument {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemIsExternalDocumentKey);
			}
		}

		[iOS (8,0)][Mac (10,9)]
		public NSString UbiquitousItemContainerDisplayName {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.UbiquitousItemContainerDisplayNameKey));
			}
		}

		[iOS (8,0)][Mac (10,9)]
		public NSUrl UbiquitousItemUrlInLocalContainer {
			get {
				return Runtime.GetNSObject<NSUrl> (GetHandle (NSMetadataQuery.UbiquitousItemURLInLocalContainerKey));
			}
		}
	}
}
