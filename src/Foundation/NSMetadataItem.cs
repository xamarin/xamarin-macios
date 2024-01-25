//
// Convenience methods for NSMetadataItem
//
// Copyright 2014, 2016 Xamarin Inc
//
// Author:
//   Miguel de Icaza
//
using System;

using CoreFoundation;

using ObjCRuntime;

#nullable enable

namespace Foundation {
	public partial class NSMetadataItem {
#if !NET
		bool GetBool (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n is null ? false : n.BoolValue;
		}
#endif

		bool? GetNullableBool (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n?.BoolValue;
		}

		double GetDouble (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n is null ? 0 : n.DoubleValue;
		}

		double? GetNullableDouble (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n?.DoubleValue;
		}

		nint? GetNInt (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n?.NIntValue;
		}

		// same order as NSMetadataAttributes.h

		public NSString? FileSystemName {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ItemFSNameKey));
			}
		}

		public NSString? DisplayName {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ItemDisplayNameKey));
			}
		}

		public NSUrl? Url {
			get {
				return Runtime.GetNSObject<NSUrl> (GetHandle (NSMetadataQuery.ItemURLKey));
			}
		}

		public NSString? Path {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ItemPathKey));
			}
		}

		public NSNumber? FileSystemSize {
			get {
				return Runtime.GetNSObject<NSNumber> (GetHandle (NSMetadataQuery.ItemFSSizeKey));
			}
		}

		public NSDate? FileSystemCreationDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.ItemFSCreationDateKey));
			}
		}

		public NSDate? FileSystemContentChangeDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.ItemFSContentChangeDateKey));
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSString? ContentType {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ContentTypeKey));
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSString? []? ContentTypeTree {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.ContentTypeTreeKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

#if NET
		public bool? IsUbiquitous {
#else
		public bool IsUbiquitous {
#endif
			get {
#if NET
				return GetNullableBool (NSMetadataQuery.ItemIsUbiquitousKey);
#else
				return GetBool (NSMetadataQuery.ItemIsUbiquitousKey);
#endif
			}
		}

#if NET
		public bool? UbiquitousItemHasUnresolvedConflicts {
#else
		public bool UbiquitousItemHasUnresolvedConflicts {
#endif
			get {
#if NET
				return GetNullableBool (NSMetadataQuery.UbiquitousItemHasUnresolvedConflictsKey);
#else
				return GetBool (NSMetadataQuery.UbiquitousItemHasUnresolvedConflictsKey);
#endif
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
#if NET
		public NSItemDownloadingStatus UbiquitousItemDownloadingStatus {
#else
		public NSItemDownloadingStatus DownloadingStatus {
#endif
			get {
				return NSItemDownloadingStatusExtensions.GetValue (Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.UbiquitousItemDownloadingStatusKey)));
			}
		}

#if NET
		public bool? UbiquitousItemIsDownloading {
#else
		public bool UbiquitousItemIsDownloading {
#endif
			get {
#if NET
				return GetNullableBool (NSMetadataQuery.UbiquitousItemIsDownloadingKey);
#else
				return GetBool (NSMetadataQuery.UbiquitousItemIsDownloadingKey);
#endif
			}
		}

#if NET
		public bool? UbiquitousItemIsUploaded {
#else
		public bool UbiquitousItemIsUploaded {
#endif
			get {
#if NET
				return GetNullableBool (NSMetadataQuery.UbiquitousItemIsUploadedKey);
#else
				return GetBool (NSMetadataQuery.UbiquitousItemIsUploadedKey);
#endif
			}
		}

#if NET
		public bool? UbiquitousItemIsUploading {
#else
		public bool UbiquitousItemIsUploading {
#endif
			get {
#if NET
				return GetNullableBool (NSMetadataQuery.UbiquitousItemIsUploadingKey);
#else
				return GetBool (NSMetadataQuery.UbiquitousItemIsUploadingKey);
#endif
			}
		}

#if NET
		public double? UbiquitousItemPercentDownloaded {
#else
		public double UbiquitousItemPercentDownloaded {
#endif
			get {
#if NET
				return GetNullableDouble (NSMetadataQuery.UbiquitousItemPercentDownloadedKey);
#else
				return GetDouble (NSMetadataQuery.UbiquitousItemPercentDownloadedKey);
#endif
			}
		}

#if NET
		public double? UbiquitousItemPercentUploaded {
#else
		public double UbiquitousItemPercentUploaded {
#endif
			get {
#if NET
				return GetNullableDouble (NSMetadataQuery.UbiquitousItemPercentUploadedKey);
#else
				return GetDouble (NSMetadataQuery.UbiquitousItemPercentUploadedKey);
#endif
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSError? UbiquitousItemDownloadingError {
			get {
				return Runtime.GetNSObject<NSError> (GetHandle (NSMetadataQuery.UbiquitousItemDownloadingErrorKey));
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSError? UbiquitousItemUploadingError {
			get {
				return Runtime.GetNSObject<NSError> (GetHandle (NSMetadataQuery.UbiquitousItemUploadingErrorKey));
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
#if NET
		public bool? UbiquitousItemDownloadRequested {
#else
		public bool UbiquitousItemDownloadRequested {
#endif
			get {
#if NET
				return GetNullableBool (NSMetadataQuery.UbiquitousItemDownloadRequestedKey);
#else
				return GetBool (NSMetadataQuery.UbiquitousItemDownloadRequestedKey);
#endif
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
#if NET
		public bool? UbiquitousItemIsExternalDocument {
#else
		public bool UbiquitousItemIsExternalDocument {
#endif
			get {
#if NET
				return GetNullableBool (NSMetadataQuery.UbiquitousItemIsExternalDocumentKey);
#else
				return GetBool (NSMetadataQuery.UbiquitousItemIsExternalDocumentKey);
#endif
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSString? UbiquitousItemContainerDisplayName {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.UbiquitousItemContainerDisplayNameKey));
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSUrl? UbiquitousItemUrlInLocalContainer {
			get {
				return Runtime.GetNSObject<NSUrl> (GetHandle (NSMetadataQuery.UbiquitousItemURLInLocalContainerKey));
			}
		}

#if MONOMAC
#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Keywords {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.KeywordsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Title {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.TitleKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Authors {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.AuthorsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Editors {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.EditorsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Participants {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ParticipantsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Projects {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ProjectsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public NSDate? DownloadedDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.DownloadedDateKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? WhereFroms {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.WhereFromsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Comment {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.CommentKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Copyright {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.CopyrightKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public NSDate? LastUsedDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.LastUsedDateKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public NSDate? ContentCreationDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.ContentCreationDateKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public NSDate? ContentModificationDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.ContentModificationDateKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public NSDate? DateAdded {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.DateAddedKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? DurationSeconds {
			get {
				return GetNullableDouble (NSMetadataQuery.DurationSecondsKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? ContactKeywords {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ContactKeywordsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Version {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.VersionKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? PixelHeight {
			get {
				return GetNInt (NSMetadataQuery.PixelHeightKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? PixelWidth {
			get {
				return GetNInt (NSMetadataQuery.PixelWidthKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? PixelCount {
			get {
				return GetNInt (NSMetadataQuery.PixelCountKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? ColorSpace {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.ColorSpaceKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? BitsPerSample {
			get {
				return GetNInt (NSMetadataQuery.BitsPerSampleKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public bool? FlashOnOff {
			get {
				return GetNullableBool (NSMetadataQuery.FlashOnOffKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? FocalLength {
			get {
				return GetNullableDouble (NSMetadataQuery.FocalLengthKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? AcquisitionMake {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.AcquisitionMakeKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? AcquisitionModel {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.AcquisitionModelKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? IsoSpeed {
			get {
				return GetNullableDouble (NSMetadataQuery.IsoSpeedKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? Orientation {
			get {
				return GetNInt (NSMetadataQuery.OrientationKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? LayerNames {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.LayerNamesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? WhiteBalance {
			get {
				return GetNullableDouble (NSMetadataQuery.WhiteBalanceKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? Aperture {
			get {
				return GetNullableDouble (NSMetadataQuery.ApertureKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? ProfileName {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.ProfileNameKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? ResolutionWidthDpi {
			get {
				return GetNInt (NSMetadataQuery.ResolutionWidthDpiKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? ResolutionHeightDpi {
			get {
				return GetNInt (NSMetadataQuery.ResolutionHeightDpiKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? ExposureMode {
			get {
				return GetNInt (NSMetadataQuery.ExposureModeKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? ExposureTimeSeconds {
			get {
				return GetNullableDouble (NSMetadataQuery.ExposureTimeSecondsKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? ExifVersion {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.ExifVersionKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? CameraOwner {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.CameraOwnerKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? FocalLength35mm {
			get {
				return GetNInt (NSMetadataQuery.FocalLength35mmKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? LensModel {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.LensModelKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? ExifGpsVersion {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.ExifGpsVersionKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? Altitude {
			get {
				return GetNullableDouble (NSMetadataQuery.AltitudeKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? Latitude {
			get {
				return GetNullableDouble (NSMetadataQuery.LatitudeKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? Longitude {
			get {
				return GetNullableDouble (NSMetadataQuery.LongitudeKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? Speed {
			get {
				return GetNullableDouble (NSMetadataQuery.SpeedKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public NSDate? Timestamp {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.TimestampKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? GpsTrack {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsTrackKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? ImageDirection {
			get {
				return GetNullableDouble (NSMetadataQuery.ImageDirectionKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? NamedLocation {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.NamedLocationKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? GpsStatus {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.GpsStatusKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? GpsMeasureMode {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.GpsMeasureModeKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? GpsDop {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDopKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? GpsMapDatum {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.GpsMapDatumKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? GpsDestLatitude {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDestLatitudeKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? GpsDestLongitude {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDestLongitudeKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? GpsDestBearing {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDestBearingKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? GpsDestDistance {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDestDistanceKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? GpsProcessingMethod {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.GpsProcessingMethodKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? GpsAreaInformation {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.GpsAreaInformationKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public NSDate? GpsDateStamp {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.GpsDateStampKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? GpsDifferental {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDifferentalKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Codecs {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.CodecsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? MediaTypes {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.MediaTypesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public bool? Streamable {
			get {
				return GetNullableBool (NSMetadataQuery.StreamableKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? TotalBitRate {
			get {
				return GetNInt (NSMetadataQuery.TotalBitRateKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? VideoBitRate {
			get {
				return GetNInt (NSMetadataQuery.VideoBitRateKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? AudioBitRate {
			get {
				return GetNInt (NSMetadataQuery.AudioBitRateKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? DeliveryType {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.DeliveryTypeKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Album {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.AlbumKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public bool? HasAlphaChannel {
			get {
				return GetNullableBool (NSMetadataQuery.HasAlphaChannelKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public bool? RedEyeOnOff {
			get {
				return GetNullableBool (NSMetadataQuery.RedEyeOnOffKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? MeteringMode {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.MeteringModeKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? MaxAperture {
			get {
				return GetNullableDouble (NSMetadataQuery.MaxApertureKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? FNumber {
			get {
				return GetNInt (NSMetadataQuery.FNumberKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? ExposureProgram {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.ExposureProgramKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? ExposureTimeString {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.ExposureTimeStringKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Headline {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.HeadlineKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Instructions {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.InstructionsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? City {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.CityKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? StateOrProvince {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.StateOrProvinceKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Country {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.CountryKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? TextContent {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.TextContentKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? AudioSampleRate {
			get {
				return GetNInt (NSMetadataQuery.AudioSampleRateKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? AudioChannelCount {
			get {
				return GetNInt (NSMetadataQuery.AudioChannelCountKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? Tempo {
			get {
				return GetNullableDouble (NSMetadataQuery.TempoKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? KeySignature {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.KeySignatureKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? TimeSignature {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.TimeSignatureKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? AudioEncodingApplication {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.AudioEncodingApplicationKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Composer {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.ComposerKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Lyricist {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.LyricistKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? AudioTrackNumber {
			get {
				return GetNInt (NSMetadataQuery.AudioTrackNumberKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public NSDate? RecordingDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.RecordingDateKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? MusicalGenre {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.MusicalGenreKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public bool? IsGeneralMidiSequence {
			get {
				return GetNullableBool (NSMetadataQuery.IsGeneralMidiSequenceKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? RecordingYear {
			get {
				return GetNInt (NSMetadataQuery.RecordingYearKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Organizations {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.OrganizationsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Languages {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.LanguagesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Rights {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.RightsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Publishers {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.PublishersKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Contributors {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ContributorsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Coverage {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.CoverageKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Subject {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.SubjectKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Theme {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ThemeKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Description {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.DescriptionKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Identifier {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.IdentifierKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Audiences {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.AudiencesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public nint? NumberOfPages {
			get {
				return GetNInt (NSMetadataQuery.NumberOfPagesKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? PageWidth {
			get {
				return GetNullableDouble (NSMetadataQuery.PageWidthKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? PageHeight {
			get {
				return GetNullableDouble (NSMetadataQuery.PageHeightKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? SecurityMethod {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.SecurityMethodKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Creator {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.CreatorKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? EncodingApplications {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.EncodingApplicationsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public NSDate? DueDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.DueDateKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public double? StarRating {
			get {
				return GetNullableDouble (NSMetadataQuery.StarRatingKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? PhoneNumbers {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.PhoneNumbersKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? EmailAddresses {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.EmailAddressesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? InstantMessageAddresses {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.InstantMessageAddressesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Kind {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.KindKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Recipients {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.RecipientsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? FinderComment {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.FinderCommentKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Fonts {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.FontsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? AppleLoopsRoot {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.AppleLoopsRootKeyKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? AppleLoopsKeyFilterType {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.AppleLoopsKeyFilterTypeKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? AppleLoopsLoopMode {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.AppleLoopsLoopModeKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? AppleLoopDescriptors {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.AppleLoopDescriptorsKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? MusicalInstrumentCategory {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.MusicalInstrumentCategoryKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? MusicalInstrumentName {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.MusicalInstrumentNameKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? CFBundleIdentifier {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.CFBundleIdentifierKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Information {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.InformationKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Director {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.DirectorKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Producer {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.ProducerKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? Genre {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.GenreKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? Performers {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.PerformersKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? OriginalFormat {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.OriginalFormatKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? OriginalSource {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.OriginalSourceKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? AuthorEmailAddresses {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.AuthorEmailAddressesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? RecipientEmailAddresses {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.RecipientEmailAddressesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? AuthorAddresses {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.AuthorAddressesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? RecipientAddresses {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.RecipientAddressesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public bool? IsLikelyJunk {
			get {
				return GetNullableBool (NSMetadataQuery.IsLikelyJunkKey);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? ExecutableArchitectures {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ExecutableArchitecturesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? ExecutablePlatform {
			get {
				return CFString.FromHandle (GetHandle (NSMetadataQuery.ExecutablePlatformKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public string? []? ApplicationCategories {
			get {
				return CFArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ApplicationCategoriesKey));
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
#else
		[NoWatch]
		[NoTV]
		[NoiOS]
#endif
		public bool? IsApplicationManaged {
			get {
				return GetNullableBool (NSMetadataQuery.IsApplicationManagedKey);
			}
		}
#endif
	}
}
