//
// Convenience methods for NSMetadataItem
//
// Copyright 2014, 2016 Xamarin Inc
//
// Author:
//   Miguel de Icaza
//
using System;
using ObjCRuntime;

namespace Foundation {
	public partial class NSMetadataItem {

		bool GetBool (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n == null ? false : n.BoolValue;
		}

		bool? GetNullableBool (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n?.BoolValue;
		}

		double GetDouble (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n == null ? 0 : n.DoubleValue;
		}

		double? GetNullableDouble (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
			return n?.DoubleValue;
		}

		nint? GetNInt (NSString key)
		{
			var n = Runtime.GetNSObject<NSNumber> (GetHandle (key));
#if XAMCORE_2_0
			return n?.NIntValue;
#else
			return n?.IntValue;
#endif
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

		// XAMCORE_4_0 FIXME return nullable
		public bool IsUbiquitous {
			get {
				return GetBool (NSMetadataQuery.ItemIsUbiquitousKey);
			}
		}

		// XAMCORE_4_0 FIXME return nullable
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

		// XAMCORE_4_0 FIXME return nullable
		public bool UbiquitousItemIsDownloading {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemIsDownloadingKey);
			}
		}

		// XAMCORE_4_0 FIXME return nullable
		public bool UbiquitousItemIsUploaded {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemIsUploadedKey);
			}
		}

		// XAMCORE_4_0 FIXME return nullable
		public bool UbiquitousItemIsUploading {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemIsUploadingKey);
			}
		}

		// XAMCORE_4_0 FIXME return nullable
		public double UbiquitousItemPercentDownloaded {
			get {
				return GetDouble (NSMetadataQuery.UbiquitousItemPercentDownloadedKey);
			}
		}

		// XAMCORE_4_0 FIXME return nullable
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

		// XAMCORE_4_0 FIXME return nullable
		[iOS (8,0)][Mac (10,10)]
		public bool UbiquitousItemDownloadRequested {
			get {
				return GetBool (NSMetadataQuery.UbiquitousItemDownloadRequestedKey);
			}
		}

		// XAMCORE_4_0 FIXME return nullable
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

#if MONOMAC
		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Keywords {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.KeywordsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Title {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.TitleKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Authors {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.AuthorsKey)); 
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Editors {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.EditorsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Participants {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ParticipantsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Projects {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ProjectsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate DownloadedDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.DownloadedDateKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] WhereFroms {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.WhereFromsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Comment {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.CommentKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Copyright {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.CopyrightKey));
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
		public double? DurationSeconds {
			get {
				return GetNullableDouble (NSMetadataQuery.DurationSecondsKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] ContactKeywords {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ContactKeywordsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Version {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.VersionKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? PixelHeight {
			get {
				return GetNInt (NSMetadataQuery.PixelHeightKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? PixelWidth {
			get {
				return GetNInt (NSMetadataQuery.PixelWidthKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? PixelCount {
			get {
				return GetNInt (NSMetadataQuery.PixelCountKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string ColorSpace {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.ColorSpaceKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? BitsPerSample {
			get {
				return GetNInt (NSMetadataQuery.BitsPerSampleKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool? FlashOnOff {
			get {
				return GetNullableBool (NSMetadataQuery.FlashOnOffKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? FocalLength {
			get {
				return GetNullableDouble (NSMetadataQuery.FocalLengthKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string AcquisitionMake {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.AcquisitionMakeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string AcquisitionModel {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.AcquisitionModelKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? IsoSpeed {
			get {
				return GetNullableDouble (NSMetadataQuery.IsoSpeedKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? Orientation {
			get {
				return GetNInt (NSMetadataQuery.OrientationKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] LayerNames {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.LayerNamesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? WhiteBalance {
			get {
				return GetNullableDouble (NSMetadataQuery.WhiteBalanceKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? Aperture {
			get {
				return GetNullableDouble (NSMetadataQuery.ApertureKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string ProfileName {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.ProfileNameKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? ResolutionWidthDpi {
			get {
				return GetNInt (NSMetadataQuery.ResolutionWidthDpiKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? ResolutionHeightDpi {
			get {
				return GetNInt (NSMetadataQuery.ResolutionHeightDpiKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? ExposureMode {
			get {
				return GetNInt (NSMetadataQuery.ExposureModeKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? ExposureTimeSeconds {
			get {
				return GetNullableDouble (NSMetadataQuery.ExposureTimeSecondsKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string ExifVersion {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.ExifVersionKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string CameraOwner {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.CameraOwnerKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? FocalLength35mm {
			get {
				return GetNInt (NSMetadataQuery.FocalLength35mmKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string LensModel {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.LensModelKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string ExifGpsVersion {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.ExifGpsVersionKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? Altitude {
			get {
				return GetNullableDouble (NSMetadataQuery.AltitudeKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? Latitude {
			get {
				return GetNullableDouble (NSMetadataQuery.LatitudeKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? Longitude {
			get {
				return GetNullableDouble (NSMetadataQuery.LongitudeKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? Speed {
			get {
				return GetNullableDouble (NSMetadataQuery.SpeedKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate Timestamp {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.TimestampKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? GpsTrack {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsTrackKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? ImageDirection {
			get {
				return GetNullableDouble (NSMetadataQuery.ImageDirectionKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string NamedLocation {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.NamedLocationKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string GpsStatus {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.GpsStatusKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string GpsMeasureMode {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.GpsMeasureModeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? GpsDop {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDopKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string GpsMapDatum {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.GpsMapDatumKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? GpsDestLatitude {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDestLatitudeKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? GpsDestLongitude {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDestLongitudeKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? GpsDestBearing {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDestBearingKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? GpsDestDistance {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDestDistanceKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string GpsProcessingMethod {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.GpsProcessingMethodKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string GpsAreaInformation {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.GpsAreaInformationKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate GpsDateStamp {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.GpsDateStampKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? GpsDifferental {
			get {
				return GetNullableDouble (NSMetadataQuery.GpsDifferentalKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Codecs {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.CodecsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] MediaTypes {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.MediaTypesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool? Streamable {
			get {
				return GetNullableBool (NSMetadataQuery.StreamableKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? TotalBitRate {
			get {
				return GetNInt (NSMetadataQuery.TotalBitRateKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? VideoBitRate {
			get {
				return GetNInt (NSMetadataQuery.VideoBitRateKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? AudioBitRate {
			get {
				return GetNInt (NSMetadataQuery.AudioBitRateKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string DeliveryType {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.DeliveryTypeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Album {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.AlbumKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool? HasAlphaChannel {
			get {
				return GetNullableBool (NSMetadataQuery.HasAlphaChannelKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool? RedEyeOnOff {
			get {
				return GetNullableBool (NSMetadataQuery.RedEyeOnOffKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string MeteringMode {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.MeteringModeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? MaxAperture {
			get {
				return GetNullableDouble (NSMetadataQuery.MaxApertureKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? FNumber {
			get {
				return GetNInt (NSMetadataQuery.FNumberKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string ExposureProgram {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.ExposureProgramKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string ExposureTimeString {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.ExposureTimeStringKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Headline {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.HeadlineKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Instructions {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.InstructionsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string City {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.CityKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string StateOrProvince {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.StateOrProvinceKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Country {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.CountryKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string TextContent {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.TextContentKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? AudioSampleRate {
			get {
				return GetNInt (NSMetadataQuery.AudioSampleRateKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? AudioChannelCount {
			get {
				return GetNInt (NSMetadataQuery.AudioChannelCountKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? Tempo {
			get {
				return GetNullableDouble (NSMetadataQuery.TempoKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string KeySignature {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.KeySignatureKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string TimeSignature {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.TimeSignatureKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string AudioEncodingApplication {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.AudioEncodingApplicationKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Composer {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.ComposerKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Lyricist {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.LyricistKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? AudioTrackNumber {
			get {
				return GetNInt (NSMetadataQuery.AudioTrackNumberKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate RecordingDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.RecordingDateKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string MusicalGenre {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.MusicalGenreKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool? IsGeneralMidiSequence {
			get {
				return GetNullableBool (NSMetadataQuery.IsGeneralMidiSequenceKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? RecordingYear {
			get {
				return GetNInt (NSMetadataQuery.RecordingYearKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Organizations {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.OrganizationsKey));			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Languages {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.LanguagesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Rights {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.RightsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Publishers {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.PublishersKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Contributors {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ContributorsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Coverage {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.CoverageKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Subject {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.SubjectKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Theme {
			get {
				return Runtime.GetNSObject<NSString> (GetHandle (NSMetadataQuery.ThemeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Description {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.DescriptionKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Identifier {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.IdentifierKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Audiences {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.AudiencesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public nint? NumberOfPages {
			get {
				return GetNInt (NSMetadataQuery.NumberOfPagesKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? PageWidth {
			get {
				return GetNullableDouble (NSMetadataQuery.PageWidthKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? PageHeight {
			get {
				return GetNullableDouble (NSMetadataQuery.PageHeightKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string SecurityMethod {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.SecurityMethodKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Creator {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.CreatorKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] EncodingApplications {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.EncodingApplicationsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public NSDate DueDate {
			get {
				return Runtime.GetNSObject<NSDate> (GetHandle (NSMetadataQuery.DueDateKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public double? StarRating {
			get {
				return GetNullableDouble (NSMetadataQuery.StarRatingKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] PhoneNumbers {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.PhoneNumbersKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] EmailAddresses {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.EmailAddressesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] InstantMessageAddresses {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.InstantMessageAddressesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Kind {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.KindKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Recipients {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.RecipientsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string FinderComment {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.FinderCommentKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Fonts {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.FontsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string AppleLoopsRoot {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.AppleLoopsRootKeyKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string AppleLoopsKeyFilterType {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.AppleLoopsKeyFilterTypeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string AppleLoopsLoopMode {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.AppleLoopsLoopModeKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] AppleLoopDescriptors {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.AppleLoopDescriptorsKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string MusicalInstrumentCategory {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.MusicalInstrumentCategoryKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string MusicalInstrumentName {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.MusicalInstrumentNameKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string CFBundleIdentifier {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.CFBundleIdentifierKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Information {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.InformationKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Director {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.DirectorKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Producer {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.ProducerKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string Genre {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.GenreKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] Performers {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.PerformersKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string OriginalFormat {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.OriginalFormatKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string OriginalSource {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.OriginalSourceKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] AuthorEmailAddresses {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.AuthorEmailAddressesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] RecipientEmailAddresses {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.RecipientEmailAddressesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] AuthorAddresses {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.AuthorAddressesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] RecipientAddresses {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.RecipientAddressesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool? IsLikelyJunk {
			get {
				return GetNullableBool (NSMetadataQuery.IsLikelyJunkKey);
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] ExecutableArchitectures {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ExecutableArchitecturesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string ExecutablePlatform {
			get {
				return NSString.FromHandle (GetHandle (NSMetadataQuery.ExecutablePlatformKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public string [] ApplicationCategories {
			get {
				return NSArray.StringArrayFromHandle (GetHandle (NSMetadataQuery.ApplicationCategoriesKey));
			}
		}

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		public bool? IsApplicationManaged {
			get {
				return GetNullableBool (NSMetadataQuery.IsApplicationManagedKey);
			}
		}
#endif
	}
}
