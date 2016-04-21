//
// Convenience methods for NSMetadataItem
//
// Copyright 2014 Xamarin Inc
//
// Author:
//   Miguel de Icaza
//
using System;

namespace XamCore.Foundation {
	public partial class NSMetadataItem {
		public NSString FileSystemName {
			get {
				return ValueForAttribute (NSMetadataQuery.ItemFSNameKey) as NSString;
			}
		}
		public NSString DisplayName {
			get {
				return ValueForAttribute (NSMetadataQuery.ItemDisplayNameKey) as NSString;
			}
		}

		public NSUrl Url {
			get {
				return ValueForAttribute (NSMetadataQuery.ItemURLKey) as NSUrl;
			}
		}

		public NSString Path {
			get {
				return ValueForAttribute (NSMetadataQuery.ItemPathKey) as NSString;
			}
		}

		public NSNumber FileSystemSize {
			get {
				return ValueForAttribute (NSMetadataQuery.ItemFSSizeKey) as NSNumber;
			}
		}

		public NSDate FileSystemCreationDate {
			get {
				return ValueForAttribute (NSMetadataQuery.ItemFSCreationDateKey) as NSDate;
			}			
		}

		public NSDate FileSystemContentChangeDate {
			get {
				return ValueForAttribute (NSMetadataQuery.ItemFSContentChangeDateKey) as NSDate;
			}
		}
		public bool IsUbiquitous {
			get {
				var b =  ValueForAttribute (NSMetadataQuery.ItemIsUbiquitousKey) as NSNumber;
				return b != null && b.Int32Value != 0;
			}
		}

		public bool UbiquitousItemHasUnresolvedConflicts {
			get {
				var b =  ValueForAttribute (NSMetadataQuery.UbiquitousItemHasUnresolvedConflictsKey) as NSNumber;
				return b != null && b.Int32Value != 0;
			}
		}
		
		public bool UbiquitousItemIsDownloading {
			get {
				var b =  ValueForAttribute (NSMetadataQuery.UbiquitousItemIsDownloadingKey) as NSNumber;
				return b != null && b.Int32Value != 0;
			}
		}

		public bool UbiquitousItemIsUploaded {
			get {
				var b =  ValueForAttribute (NSMetadataQuery.UbiquitousItemIsUploadedKey) as NSNumber;
				return b != null && b.Int32Value != 0;
			}
		}

		public bool UbiquitousItemIsUploading {
			get {
				var b =  ValueForAttribute (NSMetadataQuery.UbiquitousItemIsUploadingKey) as NSNumber;
				return b != null && b.Int32Value != 0;
			}
		}

		public double UbiquitousItemPercentDownloaded {
			get {
				var b =  ValueForAttribute (NSMetadataQuery.UbiquitousItemPercentDownloadedKey) as NSNumber;
				if (b == null)
					return 0;
				return b.DoubleValue;
			}
		}

		public double UbiquitousItemPercentUploaded {
			get {
				var b =  ValueForAttribute (NSMetadataQuery.UbiquitousItemPercentUploadedKey) as NSNumber;
				if (b == null)
					return 0;
				return b.DoubleValue;
			}
		}

		public NSItemDownloadingStatus DownloadingStatus {
			get {
				var b = ValueForAttribute (NSMetadataQuery.UbiquitousItemDownloadingStatusKey) as NSString;
				if (b == _StatusDownloaded)
					return NSItemDownloadingStatus.Downloaded;
				if (b == _StatusCurrent)
					return NSItemDownloadingStatus.Current;
				if (b == _NotDownloaded)
					return NSItemDownloadingStatus.NotDownloaded;
				return NSItemDownloadingStatus.Unknown;
			}
		}

		public NSError UbiquitousItemDownloadingError {
			get {
				return ValueForAttribute (NSMetadataQuery.UbiquitousItemDownloadingErrorKey) as NSError;
			}
		}
		
		public NSError UbiquitousItemUploadingError {
			get {
				return ValueForAttribute (NSMetadataQuery.UbiquitousItemUploadingErrorKey) as NSError;
			}
		}
	}
}
