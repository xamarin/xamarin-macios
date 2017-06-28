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

		[iOS (8, 0)]
		[Mac (10, 9)]
		public NSString ContentType {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ContentTypeKey));
			}
		}

		[iOS (8, 0)]
		[Mac (10, 9)]
		public NSString [] ContentTypeTree {
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

		[iOS (7, 0)]
		[Mac (10, 9)]
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

		[iOS (7, 0)]
		[Mac (10, 9)]
		public NSError UbiquitousItemDownloadingError {
			get {
				return Runtime.GetNSObject<NSError> (GetHandle (NSMetadataQuery.UbiquitousItemDownloadingErrorKey));
			}
		}

		[iOS (7, 0)]
		[Mac (10, 9)]
		public NSError UbiquitousItemUploadingError {
			get {
				return Runtime.GetNSObject<NSError> (GetHandle (NSMetadataQuery.UbiquitousItemUploadingErrorKey));
			}
		}

		[iOS (8, 0)]
		[Mac (10, 10)]
		public bool UbiquitousItemDownloadRequested {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemDownloadRequestedKey);
			}
		}

		[iOS (8, 0)]
		[Mac (10, 10)]
		public bool UbiquitousItemIsExternalDocument {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemIsExternalDocumentKey);
			}
		}

		[iOS (8, 0)]
		[Mac (10, 9)]
		public NSString UbiquitousItemContainerDisplayName {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.UbiquitousItemContainerDisplayNameKey));
			}
		}

		[iOS (8, 0)]
		[Mac (10, 9)]
		public NSUrl UbiquitousItemUrlInLocalContainer {
			get {
				return Runtime.GetNSObject<NSUrl> (GetHandle (NSMetadataQuery.UbiquitousItemURLInLocalContainerKey));
			}
		}

#if MONOMAC
		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Keywords {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.KeywordsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Title {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.TitleKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Authors {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.AuthorsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Editors {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.EditorsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Participants {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.ParticipantsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Projects {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.ProjectsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate DownloadedDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.DownloadedDateKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] WhereFroms {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.WhereFromsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Comment {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.CommentKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Copyright {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.CopyrightKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate LastUsedDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.LastUsedDateKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate ContentCreationDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.ContentCreationDateKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate ContentModificationDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.ContentModificationDateKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate DateAdded {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.DateAddedKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemDurationSecondsKey")]
		NSString DurationSecondsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] ContactKeywords {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.ContactKeywordsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Version {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.VersionKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPixelHeightKey")]
		NSString PixelHeightKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPixelWidthKey")]
		NSString PixelWidthKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPixelCountKey")]
		NSString PixelCountKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString ColorSpace {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ColorSpaceKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemBitsPerSampleKey")]
		NSString BitsPerSampleKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool FlashOnOff {
			get {
				return GetBool (NSMetadataQuery.FlashOnOffKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemFocalLengthKey")]
		NSString FocalLengthKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString AcquisitionMake {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.AcquisitionMakeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString AcquisitionModel {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.AcquisitionModelKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemISOSpeedKey")]
		NSString IsoSpeedKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemOrientationKey")]
		NSString OrientationKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] LayerNames {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.LayerNamesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemWhiteBalanceKey")]
		NSString WhiteBalanceKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemApertureKey")]
		NSString ApertureKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString ProfileName {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ProfileNameKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemResolutionWidthDPIKey")]
		NSString ResolutionWidthDpiKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemResolutionHeightDPIKey")]
		NSString ResolutionHeightDpiKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemExposureModeKey")]
		NSString ExposureModeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemExposureTimeSecondsKey")]
		NSString ExposureTimeSecondsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString ExifVersion {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ExifVersionKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString CameraOwner {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.CameraOwnerKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemFocalLength35mmKey")]
		NSString FocalLength35mmKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString LensModel {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.LensModelKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString ExifGpsVersion {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ExifGpsVersionKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAltitudeKey")]
		NSString AltitudeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemLatitudeKey")]
		NSString LatitudeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemLongitudeKey")]
		NSString LongitudeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemSpeedKey")]
		NSString SpeedKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate Timestamp {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.TimestampKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSTrackKey")]
		NSString GpsTrackKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemImageDirectionKey")]
		NSString ImageDirectionKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString NamedLocation {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.NamedLocationKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString GpsStatus {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.GpsStatusKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString GpsMeasureMode {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.GpsMeasureModeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDOPKey")]
		NSString GpsDopKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString GpsMapDatum {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.GpsMapDatumKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDestLatitudeKey")]
		NSString GpsDestLatitudeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDestLongitudeKey")]
		NSString GpsDestLongitudeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDestBearingKey")]
		NSString GpsDestBearingKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDestDistanceKey")]
		NSString GpsDestDistanceKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString GpsProcessingMethod {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.GpsProcessingMethodKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString GpsAreaInformation {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.GpsAreaInformationKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate GpsDateStamp {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.GpsDateStampKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDifferentalKey")]
		NSString GpsDifferentalKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Codecs {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.CodecsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] MediaTypes {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.MediaTypesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool Streamable {
			get {
				return GetBool (NSMetadataQuery.StreamableKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemTotalBitRateKey")]
		NSString TotalBitRateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemVideoBitRateKey")]
		NSString VideoBitRateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAudioBitRateKey")]
		NSString AudioBitRateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString DeliveryType {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.DeliveryTypeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Album {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.AlbumKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool HasAlphaChannel {
			get {
				return GetBool (NSMetadataQuery.HasAlphaChannelKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool RedEyeOnOff {
			get {
				return GetBool (NSMetadataQuery.RedEyeOnOffKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString MeteringMode {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.MeteringModeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemMaxApertureKey")]
		NSString MaxApertureKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemFNumberKey")]
		NSString FNumberKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString ExposureProgram {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ExposureProgramKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString ExposureTimeString {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ExposureTimeStringKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Headline {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.HeadlineKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Instructions {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.InstructionsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString City {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.CityKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString StateOrProvince {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.StateOrProvinceKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Country {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.CountryKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString TextContent {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.TextContentKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAudioSampleRateKey")]
		NSString AudioSampleRateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAudioChannelCountKey")]
		NSString AudioChannelCountKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemTempoKey")]
		NSString TempoKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString KeySignature {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.KeySignatureKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString TimeSignature {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.TimeSignatureKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString AudioEncodingApplication {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.AudioEncodingApplicationKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Composer {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ComposerKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Lyricist {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.LyricistKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAudioTrackNumberKey")]
		NSString AudioTrackNumberKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate RecordingDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.RecordingDateKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString MusicalGenre {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.MusicalGenreKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool IsGeneralMidiSequence {
			get {
				return GetBool (NSMetadataQuery.IsGeneralMidiSequenceKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemRecordingYearKey")]
		NSString RecordingYearKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Organizations {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.OrganizationsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Languages {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.LanguagesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Rights {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.RightsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Publishers {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.PublishersKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Contributors {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.ContributorsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Coverage {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.CoverageKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Subject {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.SubjectKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Theme {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ThemeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Description {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.DescriptionKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Identifier {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.IdentifierKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Audiences {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.AudiencesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemNumberOfPagesKey")]
		NSString NumberOfPagesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPageWidthKey")]
		NSString PageWidthKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPageHeightKey")]
		NSString PageHeightKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemSecurityMethodKey")]
		NSString SecurityMethodKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Creator {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.CreatorKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] EncodingApplications {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.EncodingApplicationsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate DueDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.DueDateKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemStarRatingKey")]
		NSString StarRatingKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] PhoneNumbers {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.PhoneNumbersKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] EmailAddresses {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.EmailAddressesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] InstantMessageAddresses {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.InstantMessageAddressesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Kind {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.KindKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Recipients {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.RecipientsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString FinderComment {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.FinderCommentKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Fonts {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.FontsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString AppleLoopsRoot {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.AppleLoopsRootKeyKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString AppleLoopsKeyFilterType {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.AppleLoopsKeyFilterTypeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString AppleLoopsLoopMode {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.AppleLoopsLoopModeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] AppleLoopDescriptors {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.AppleLoopDescriptorsKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString MusicalInstrumentCategory {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.MusicalInstrumentCategoryKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString MusicalInstrumentName {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.MusicalInstrumentNameKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString CFBundleIdentifier {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.CFBundleIdentifierKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Information {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.InformationKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Director {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.DirectorKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Producer {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ProducerKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString Genre {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.GenreKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] Performers {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.PerformersKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString OriginalFormat {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.OriginalFormatKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString OriginalSource {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.OriginalSourceKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] AuthorEmailAddresses {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.AuthorEmailAddressesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] RecipientEmailAddresses {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.RecipientEmailAddressesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] AuthorAddresses {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.AuthorAddressesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] RecipientAddresses {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.RecipientAddressesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool IsLikelyJunk {
			get {
				return GetBool (NSMetadataQuery.IsLikelyJunkKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] ExecutableArchitectures {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.ExecutableArchitecturesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString ExecutablePlatform {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ExecutablePlatformKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSString [] ApplicationCategories {
			get {
				using (var a = Runtime.GetNSObject<NSArray> (GetHandle (NSMetadataQuery.ApplicationCategoriesKey)))
					return NSArray.FromArray<NSString> (a);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool IsApplicationManaged {
			get {
				return GetBool (NSMetadataQuery.IsApplicationManagedKey);
			}
		}
#endif
	}
}
