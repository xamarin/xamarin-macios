//
// CoreSpotlight bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.ComponentModel;
using ObjCRuntime;
using Foundation;

namespace CoreSpotlight {

	[NoTV] // CS_TVOS_UNAVAILABLE
	[iOS (9,0)]
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CSIndexExtensionRequestHandler : NSExtensionRequestHandling, CSSearchableIndexDelegate {

	}

	[NoTV] // CS_TVOS_UNAVAILABLE
	[iOS (9,0)]
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CSPerson : NSSecureCoding, NSCopying {

		[Export ("initWithDisplayName:handles:handleIdentifier:")]
		IntPtr Constructor ([NullAllowed] string displayName, string [] handles, NSString handleIdentifier);

		[NullAllowed]
		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("handles")]
		string [] Handles { get; }

		[Export ("handleIdentifier")]
		NSString HandleIdentifier { get; }

		[NullAllowed]
		[Export ("contactIdentifier")]
		string ContactIdentifier { get; set; }
	}

	[NoTV] // CS_TVOS_UNAVAILABLE
	[iOS (9,0)]
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CSSearchableIndex {

		[Export ("indexDelegate", ArgumentSemantic.Weak)][NullAllowed]
		ICSSearchableIndexDelegate IndexDelegate { get; set; }

		[Static]
		[Export ("isIndexingAvailable")]
		bool IsIndexingAvailable { get; }

		[Static]
		[Export ("defaultSearchableIndex")]
		CSSearchableIndex DefaultSearchableIndex { get; }

		[Export ("initWithName:")]
		IntPtr Constructor (string name);

#if !MONOMAC
		[Export ("initWithName:protectionClass:")]
		IntPtr Constructor (string name, [NullAllowed] NSString protectionClass);
#endif

		[Export ("indexSearchableItems:completionHandler:")]
		[Async]
		void Index (CSSearchableItem [] items, [NullAllowed] Action<NSError> completionHandler);

		[Export ("deleteSearchableItemsWithIdentifiers:completionHandler:")]
		[Async]
		void Delete (string [] identifiers, [NullAllowed] Action<NSError> completionHandler);

		[Export ("deleteSearchableItemsWithDomainIdentifiers:completionHandler:")]
		[Async]
		void DeleteWithDomain (string [] domainIdentifiers, [NullAllowed] Action<NSError> completionHandler);

		[Export ("deleteAllSearchableItemsWithCompletionHandler:")]
		[Async]
		void DeleteAll ([NullAllowed] Action<NSError> completionHandler);
	}

	delegate void CSSearchableIndexFetchHandler (NSData clientState, NSError error);

	[NoTV] // CS_TVOS_UNAVAILABLE
	[iOS (9,0)]
	[Mac (10,13, onlyOn64: true)]
	[Category]
	[BaseType (typeof (CSSearchableIndex))]
	interface CSSearchableIndex_CSOptionalBatchingExtension {

		[Export ("beginIndexBatch")]
		void BeginIndexBatch ();

		[Export ("endIndexBatchWithClientState:completionHandler:")]
		void EndIndexBatch (NSData clientState, [NullAllowed] Action<NSError> completionHandler);

		[Export ("fetchLastClientStateWithCompletionHandler:")]
		void FetchLastClientState (CSSearchableIndexFetchHandler completionHandler);
	}

	interface ICSSearchableIndexDelegate {}

	[NoTV] // CS_TVOS_UNAVAILABLE
	[iOS (9,0)]
	[Mac (10,13, onlyOn64: true)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CSSearchableIndexDelegate {

		[Abstract]
		[Export ("searchableIndex:reindexAllSearchableItemsWithAcknowledgementHandler:")]
		void ReindexAllSearchableItems (CSSearchableIndex searchableIndex, Action acknowledgementHandler);

		[Abstract]
		[Export ("searchableIndex:reindexSearchableItemsWithIdentifiers:acknowledgementHandler:")]
		void ReindexSearchableItems (CSSearchableIndex searchableIndex, string [] identifiers, Action acknowledgementHandler);

		[Export ("searchableIndexDidThrottle:")]
		void DidThrottle (CSSearchableIndex searchableIndex);

		[Export ("searchableIndexDidFinishThrottle:")]
		void DidFinishThrottle (CSSearchableIndex searchableIndex);

		[iOS (11,0), NoTV, Mac (10,13)]
		[Export ("dataForSearchableIndex:itemIdentifier:typeIdentifier:error:")]
		[return: NullAllowed]
		NSData GetData (CSSearchableIndex searchableIndex, string itemIdentifier, string typeIdentifier, out NSError outError);

		[iOS (11,0), NoTV, Mac (10,13)]
		[Export ("fileURLForSearchableIndex:itemIdentifier:typeIdentifier:inPlace:error:")]
		[return: NullAllowed]
		NSUrl GetFileUrl (CSSearchableIndex searchableIndex, string itemIdentifier, string typeIdentifier, bool inPlace, out NSError outError);
	}

	[NoTV] // CS_TVOS_UNAVAILABLE
	[iOS (9,0)]
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CSSearchableItem : NSSecureCoding, NSCopying {

		[Field ("CSSearchableItemActionType")]
		NSString ActionType { get; }

		[Field ("CSSearchableItemActivityIdentifier")]
		NSString ActivityIdentifier { get; }

		[iOS (10,0)]
		[Field ("CSQueryContinuationActionType")]
		NSString ContinuationActionType { get; }

		[iOS (10,0)]
		[Field ("CSSearchQueryString")]
		NSString QueryString { get; }

		[Export ("initWithUniqueIdentifier:domainIdentifier:attributeSet:")]
		IntPtr Constructor ([NullAllowed] string uniqueIdentifier, [NullAllowed] string domainIdentifier, CSSearchableItemAttributeSet attributeSet);

		[Export ("uniqueIdentifier")]
		string UniqueIdentifier { get; set; }

		[NullAllowed]
		[Export ("domainIdentifier")]
		string DomainIdentifier { get; set; }

		[NullAllowed] // <- null_resettable
		[Export ("expirationDate")]
		NSDate ExpirationDate { get; set; }

		[Export ("attributeSet", ArgumentSemantic.Strong)]
		CSSearchableItemAttributeSet AttributeSet { get; set; }
	}

	[NoTV] // CS_TVOS_UNAVAILABLE
	[iOS (9,0)]
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSString))]
	// hack: it seems that generator.cs can't track NSCoding correctly ? maybe because the type is named NSString2 at that time
	interface CSLocalizedString : NSCoding {

		[Export ("initWithLocalizedStrings:")]
		IntPtr Constructor (NSDictionary localizedStrings);

		[Export ("localizedString")]
		string GetLocalizedString ();
	}

	[NoTV] // CS_TVOS_UNAVAILABLE
	[iOS (9,0)]
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CSCustomAttributeKey initWithKeyName...]
	interface CSCustomAttributeKey : NSCopying, NSSecureCoding {

		[Export ("initWithKeyName:")]
		IntPtr Constructor (string keyName);

		[DesignatedInitializer]
		[Export ("initWithKeyName:searchable:searchableByDefault:unique:multiValued:")]
		IntPtr Constructor (string keyName, bool searchable, bool searchableByDefault, bool unique, bool multiValued);

		[Export ("keyName")]
		string KeyName { get; }

		[Export ("searchable")]
		bool Searchable { [Bind ("isSearchable")] get; }

		[Export ("searchableByDefault")]
		bool SearchableByDefault { [Bind ("isSearchableByDefault")] get; }

		[Export ("unique")]
		bool Unique { [Bind ("isUnique")] get; }

		[Export ("multiValued")]
		bool MultiValued { [Bind ("isMultiValued")] get; }
	}

#if XAMCORE_4_0
	[NoTV]
#endif
	[iOS (9,0)]
	[Mac (10,13, onlyOn64: true)]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	[Static]
	interface CSMailboxKey {

		[Field ("CSMailboxInbox")]
		NSString Inbox { get; }

		[Field ("CSMailboxDrafts")]
		NSString Drafts { get; }

		[Field ("CSMailboxSent")]
		NSString Sent { get; }

		[Field ("CSMailboxJunk")]
		NSString Junk { get; }

		[Field ("CSMailboxTrash")]
		NSString Trash { get; }

		[Field ("CSMailboxArchive")]
		NSString Archive { get; }
	}

	[NoTV]
	[iOS (9,0)]
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CSSearchableItemAttributeSet : NSCopying, NSSecureCoding {

		[Export ("initWithItemContentType:")]
		IntPtr Constructor (string itemContentType);

		// FIXME: Should we keep all the following Categories inline? or should we make them actual [Category] interfaces
		// There are no methods on any of the following categories, just properties

		// CSSearchableItemAttributeSet_Documents.h
		// CSSearchableItemAttributeSet (CSDocuments) Category
		[NullAllowed]
		[Export ("subject")]
		string Subject { get; set; }

		[NullAllowed]
		[Export ("theme")]
		string Theme { get; set; }

		[NullAllowed]
		[Export ("contentDescription")]
		string ContentDescription { get; set; }

		[NullAllowed]
		[Export ("identifier")]
		string Identifier { get; set; }

		[NullAllowed]
		[Export ("audiences")]
		string [] Audiences { get; set; }

		[NullAllowed]
		[Export ("fileSize")]
		NSNumber FileSize { get; set; }

		[NullAllowed]
		[Export ("pageCount", ArgumentSemantic.Strong)]
		NSNumber PageCount { get; set; }

		[NullAllowed]
		[Export ("pageWidth", ArgumentSemantic.Strong)]
		NSNumber PageWidth { get; set; }

		[NullAllowed]
		[Export ("pageHeight", ArgumentSemantic.Strong)]
		NSNumber PageHeight { get; set; }

		[NullAllowed]
		[Export ("securityMethod")]
		string SecurityMethod { get; set; }

		[NullAllowed]
		[Export ("creator")]
		string Creator { get; set; }

		[NullAllowed]
		[Export ("encodingApplications")]
		string [] EncodingApplications { get; set; }

		[NullAllowed]
		[Export ("kind")]
		string Kind { get; set; }

		[NullAllowed]
		[Export ("fontNames")]
		string [] FontNames { get; set; }

		// CSSearchableItemAttributeSet_Events.h
		// CSSearchableItemAttributeSet (CSEvents) Category

		[NullAllowed]
		[Export ("dueDate", ArgumentSemantic.Strong)]
		NSDate DueDate { get; set; }

		[NullAllowed]
		[Export ("completionDate", ArgumentSemantic.Strong)]
		NSDate CompletionDate { get; set; }

		[NullAllowed]
		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; set; }

		[NullAllowed]
		[Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; set; }

		[NullAllowed]
		[Export ("importantDates")]
		NSDate [] ImportantDates { get; set; }

		[Export ("allDay", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSNumber AllDay { get; set; }

		// CSSearchableItemAttributeSet_General.h
		// CSSearchableItemAttributeSet (CSGeneral) Category

		[NullAllowed]
		[Export ("displayName")]
		string DisplayName { get; set; }

		[NullAllowed]
		[Export ("alternateNames")]
		string [] AlternateNames { get; set; }

		[NullAllowed]
		[Export ("path")]
		string Path { get; set; }

		[NullAllowed]
		[Export ("contentURL", ArgumentSemantic.Strong)]
		NSUrl ContentUrl { get; set; }

		[NullAllowed]
		[Export ("thumbnailURL", ArgumentSemantic.Strong)]
		NSUrl ThumbnailUrl { get; set; }

		[NullAllowed]
		[Export ("thumbnailData", ArgumentSemantic.Copy)]
		NSData ThumbnailData { get; set; }

		[NullAllowed]
		[Export ("relatedUniqueIdentifier")]
		string RelatedUniqueIdentifier { get; set; }

		[NullAllowed]
		[Export ("metadataModificationDate", ArgumentSemantic.Strong)]
		NSDate MetadataModificationDate { get; set; }

		[NullAllowed]
		[Export ("contentType")]
		string ContentType { get; set; }

		[NullAllowed]
		[Export ("contentTypeTree")]
		string [] ContentTypeTree { get; set; }

		[NullAllowed]
		[Export ("keywords")]
		string [] Keywords { get; set; }

		[NullAllowed]
		[Export ("title")]
		string Title { get; set; }

		[NullAllowed]
		[Export ("version")]
		string Version { get; set; }

		[iOS (10,0)]
		[NullAllowed, Export ("weakRelatedUniqueIdentifier", ArgumentSemantic.Copy)]
		string WeakRelatedUniqueIdentifier { get; set; }

		[iOS (10,0)]
		[NullAllowed, Export ("domainIdentifier")]
		string DomainIdentifier { get; set; }

		// CSSearchableItemAttributeSet_Images.h
		// CSSearchableItemAttributeSet (CSImages) Category

		[NullAllowed]
		[Export ("pixelHeight", ArgumentSemantic.Strong)]
		NSNumber PixelHeight { get; set; }

		[NullAllowed]
		[Export ("pixelWidth", ArgumentSemantic.Strong)]
		NSNumber PixelWidth { get; set; }

		[NullAllowed]
		[Export ("pixelCount", ArgumentSemantic.Strong)]
		NSNumber PixelCount { get; set; }

		[NullAllowed]
		[Export ("colorSpace")]
		string ColorSpace { get; set; }

		[NullAllowed]
		[Export ("bitsPerSample", ArgumentSemantic.Strong)]
		NSNumber BitsPerSample { get; set; }

		[NullAllowed]
		[Export ("flashOn", ArgumentSemantic.Strong)]
		NSNumber FlashOn { [Bind ("isFlashOn")] get; set; }

		[NullAllowed]
		[Export ("focalLength", ArgumentSemantic.Strong)]
		NSNumber FocalLength { get; set; }

		[NullAllowed]
		[Export ("focalLength35mm", ArgumentSemantic.Strong)]
		NSNumber FocalLength35mm { [Bind ("isFocalLength35mm")] get; set; }

		[NullAllowed]
		[Export ("acquisitionMake")]
		string AcquisitionMake { get; set; }

		[NullAllowed]
		[Export ("acquisitionModel")]
		string AcquisitionModel { get; set; }

		[NullAllowed]
		[Export ("cameraOwner")]
		string CameraOwner { get; set; }

		[NullAllowed]
		[Export ("lensModel")]
		string LensModel { get; set; }

		[NullAllowed]
		[Export ("ISOSpeed", ArgumentSemantic.Strong)]
		NSNumber IsoSpeed { get; set; }

		[NullAllowed]
		[Export ("orientation", ArgumentSemantic.Strong)]
		NSNumber Orientation { get; set; }

		[NullAllowed]
		[Export ("layerNames")]
		string [] LayerNames { get; set; }

		[NullAllowed]
		[Export ("whiteBalance", ArgumentSemantic.Strong)]
		NSNumber WhiteBalance { get; set; }

		[NullAllowed]
		[Export ("aperture", ArgumentSemantic.Strong)]
		NSNumber Aperture { get; set; }

		[NullAllowed]
		[Export ("profileName")]
		string ProfileName { get; set; }

		[NullAllowed]
		[Export ("resolutionWidthDPI", ArgumentSemantic.Strong)]
		NSNumber ResolutionWidthDpi { get; set; }

		[NullAllowed]
		[Export ("resolutionHeightDPI", ArgumentSemantic.Strong)]
		NSNumber ResolutionHeightDPI { get; set; }

		[NullAllowed]
		[Export ("exposureMode", ArgumentSemantic.Strong)]
		NSNumber ExposureMode { get; set; }

		[NullAllowed]
		[Export ("exposureTime", ArgumentSemantic.Strong)]
		NSNumber ExposureTime { get; set; }

		[NullAllowed]
		[Export ("EXIFVersion")]
		string ExifVersion { get; set; }

		[NullAllowed]
		[Export ("EXIFGPSVersion")]
		string ExifGpsVersion { get; set; }

		[NullAllowed]
		[Export ("hasAlphaChannel", ArgumentSemantic.Strong)]
		NSNumber HasAlphaChannel { get; set; }

		[NullAllowed]
		[Export ("redEyeOn", ArgumentSemantic.Strong)]
		NSNumber RedEyeOn { [Bind ("isRedEyeOn")] get; set; }

		[NullAllowed]
		[Export ("meteringMode")]
		string MeteringMode { get; set; }

		[NullAllowed]
		[Export ("maxAperture", ArgumentSemantic.Strong)]
		NSNumber MaxAperture { get; set; }

		[NullAllowed]
		[Export ("fNumber", ArgumentSemantic.Strong)]
		NSNumber FNumber { get; set; }

		[NullAllowed]
		[Export ("exposureProgram")]
		string ExposureProgram { get; set; }

		[NullAllowed]
		[Export ("exposureTimeString")]
		string ExposureTimeString { get; set; }

		// CSSearchableItemAttributeSet_Media.h
		// CSSearchableItemAttributeSet (CSMedia) Category

		[NullAllowed]
		[Export ("editors")]
		string [] Editors { get; set; }

		[NullAllowed]
		[Export ("participants")]
		string [] Participants { get; set; }

		[NullAllowed]
		[Export ("projects")]
		string [] Projects { get; set; }

		[NullAllowed]
		[Export ("downloadedDate", ArgumentSemantic.Strong)]
		NSDate DownloadedDate { get; set; }

		[NullAllowed]
		[Export ("contentSources")]
		string [] ContentSources { get; set; }

		[NullAllowed]
		[Export ("comment")]
		string Comment { get; set; }

		[NullAllowed]
		[Export ("copyright")]
		string Copyright { get; set; }

		[NullAllowed]
		[Export ("lastUsedDate", ArgumentSemantic.Strong)]
		NSDate LastUsedDate { get; set; }

		[NullAllowed]
		[Export ("contentCreationDate", ArgumentSemantic.Strong)]
		NSDate ContentCreationDate { get; set; }

		[NullAllowed]
		[Export ("contentModificationDate", ArgumentSemantic.Strong)]
		NSDate ContentModificationDate { get; set; }

		[NullAllowed]
		[Export ("addedDate", ArgumentSemantic.Strong)]
		NSDate AddedDate { get; set; }

		[NullAllowed]
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; set; }

		[NullAllowed]
		[Export ("contactKeywords")]
		string [] ContactKeywords { get; set; }

		[NullAllowed]
		[Export ("codecs")]
		string [] Codecs { get; set; }

		[NullAllowed]
		[Export ("mediaTypes")]
		string [] MediaTypes { get; set; }

		[NullAllowed]
		[Export ("streamable", ArgumentSemantic.Strong)]
		NSNumber Streamable { [Bind ("isStreamable")] get; set; }

		[NullAllowed]
		[Export ("totalBitRate", ArgumentSemantic.Strong)]
		NSNumber TotalBitRate { get; set; }

		[NullAllowed]
		[Export ("videoBitRate", ArgumentSemantic.Strong)]
		NSNumber VideoBitRate { get; set; }

		[NullAllowed]
		[Export ("audioBitRate", ArgumentSemantic.Strong)]
		NSNumber AudioBitRate { get; set; }

		[NullAllowed]
		[Export ("deliveryType", ArgumentSemantic.Strong)]
		NSNumber DeliveryType { get; set; }

		[NullAllowed]
		[Export ("organizations")]
		string [] Organizations { get; set; }

		[NullAllowed]
		[Export ("role")]
		string Role { get; set; }

		[NullAllowed]
		[Export ("languages")]
		string [] Languages { get; set; }

		[NullAllowed]
		[Export ("rights")]
		string Rights { get; set; }

		[NullAllowed]
		[Export ("publishers")]
		string [] Publishers { get; set; }

		[NullAllowed]
		[Export ("contributors")]
		string [] Contributors { get; set; }

		[NullAllowed]
		[Export ("coverage")]
		string [] Coverage { get; set; }

		[NullAllowed]
		[Export ("rating", ArgumentSemantic.Strong)]
		NSNumber Rating { get; set; }

		[NullAllowed]
		[Export ("ratingDescription")]
		NSNumber RatingDescription { get; set; }
		
		[NullAllowed]
		[Export ("playCount", ArgumentSemantic.Strong)]
		NSNumber PlayCount { get; set; }

		[NullAllowed]
		[Export ("information")]
		string Information { get; set; }

		[NullAllowed]
		[Export ("director")]
		string Director { get; set; }

		[NullAllowed]
		[Export ("producer")]
		string Producer { get; set; }

		[NullAllowed]
		[Export ("genre")]
		string Genre { get; set; }

		[NullAllowed]
		[Export ("performers")]
		string [] Performers { get; set; }

		[NullAllowed]
		[Export ("originalFormat")]
		string OriginalFormat { get; set; }

		[NullAllowed]
		[Export ("originalSource")]
		string OriginalSource { get; set; }

		[NullAllowed]
		[Export ("local", ArgumentSemantic.Strong)]
		NSNumber Local { [Bind ("isLocal")] get; set; }

		[NullAllowed]
		[Export ("contentRating", ArgumentSemantic.Strong)]
		NSNumber ContentRating { get; set; }

		[NullAllowed]
		[Export ("URL", ArgumentSemantic.Strong)]
		NSUrl Url { get; set; }

		// CSSearchableItemAttributeSet_Media.h
		// CSSearchableItemAttributeSet (CSMusic) Category

		[NullAllowed]
		[Export ("audioSampleRate", ArgumentSemantic.Strong)]
		NSNumber AudioSampleRate { get; set; }

		[NullAllowed]
		[Export ("audioChannelCount", ArgumentSemantic.Strong)]
		NSNumber AudioChannelCount { get; set; }

		[NullAllowed]
		[Export ("tempo", ArgumentSemantic.Strong)]
		NSNumber Tempo { get; set; }

		[NullAllowed]
		[Export ("keySignature")]
		string KeySignature { get; set; }

		[NullAllowed]
		[Export ("timeSignature")]
		string TimeSignature { get; set; }

		[NullAllowed]
		[Export ("audioEncodingApplication")]
		string AudioEncodingApplication { get; set; }

		[NullAllowed]
		[Export ("composer")]
		string Composer { get; set; }

		[NullAllowed]
		[Export ("lyricist")]
		string Lyricist { get; set; }

		[NullAllowed]
		[Export ("album")]
		string Album { get; set; }

		[NullAllowed]
		[Export ("artist")]
		string Artist { get; set; }

		[NullAllowed]
		[Export ("audioTrackNumber", ArgumentSemantic.Strong)]
		NSNumber AudioTrackNumber { get; set; }

		[NullAllowed]
		[Export ("recordingDate", ArgumentSemantic.Strong)]
		NSDate RecordingDate { get; set; }

		[NullAllowed]
		[Export ("musicalGenre")]
		string MusicalGenre { get; set; }

		[NullAllowed]
		[Export ("generalMIDISequence", ArgumentSemantic.Strong)]
		NSNumber GeneralMidiSequence { [Bind ("isGeneralMIDISequence")] get; set; }

		[NullAllowed]
		[Export ("musicalInstrumentCategory")]
		string MusicalInstrumentCategory { get; set; }

		[NullAllowed]
		[Export ("musicalInstrumentName")]
		string MusicalInstrumentName { get; set; }

		// CSSearchableItemAttributeSet_Messaging.h
		// CSSearchableItemAttributeSet (CSMessaging) Category

		[NullAllowed]
		[Export ("accountIdentifier")]
		string AccountIdentifier { get; set; }

		[NullAllowed]
		[Export ("accountHandles")]
		string [] AccountHandles { get; set; }

		[NullAllowed]
		[Export ("HTMLContentData", ArgumentSemantic.Copy)]
		NSData HtmlContentData { get; set; }

		[NullAllowed]
		[Export ("textContent")]
		string TextContent { get; set; }

		[NullAllowed]
		[Export ("authors", ArgumentSemantic.Copy)]
		CSPerson [] Authors { get; set; }

		[NullAllowed]
		[Export ("primaryRecipients", ArgumentSemantic.Copy)]
		CSPerson [] PrimaryRecipients { get; set; }

		[NullAllowed]
		[Export ("additionalRecipients", ArgumentSemantic.Copy)]
		CSPerson [] AdditionalRecipients { get; set; }

		[NullAllowed]
		[Export ("hiddenAdditionalRecipients", ArgumentSemantic.Copy)]
		CSPerson [] HiddenAdditionalRecipients { get; set; }

		[NullAllowed]
		[Export ("emailHeaders", ArgumentSemantic.Copy)]
		NSDictionary EmailHeaders { get; set; }

		[NullAllowed]
		[Export ("mailboxIdentifiers")]
		string [] MailboxIdentifiers { get; set; }

		[NullAllowed]
		[Export ("authorNames")]
		string [] AuthorNames { get; set; }

		[NullAllowed]
		[Export ("recipientNames")]
		string [] RecipientNames { get; set; }

		[NullAllowed]
		[Export ("authorEmailAddresses")]
		string [] AuthorEmailAddresses { get; set; }

		[NullAllowed]
		[Export ("recipientEmailAddresses")]
		string [] RecipientEmailAddresses { get; set; }

		[NullAllowed]
		[Export ("authorAddresses")]
		string [] AuthorAddresses { get; set; }

		[NullAllowed]
		[Export ("recipientAddresses")]
		string [] RecipientAddresses { get; set; }

		[NullAllowed]
		[Export ("phoneNumbers")]
		string [] PhoneNumbers { get; set; }

		[NullAllowed]
		[Export ("emailAddresses")]
		string [] EmailAddresses { get; set; }

		[NullAllowed]
		[Export ("instantMessageAddresses")]
		string [] InstantMessageAddresses { get; set; }

		[NullAllowed]
		[Export ("likelyJunk", ArgumentSemantic.Strong)]
		NSNumber LikelyJunk { [Bind ("isLikelyJunk")] get; set; }

		// CSSearchableItemAttributeSet_Places.h
		// CSSearchableItemAttributeSet (CSPlaces) Category

		[NullAllowed]
		[Export ("headline")]
		string Headline { get; set; }

		[NullAllowed]
		[Export ("instructions")]
		string Instructions { get; set; }

		[NullAllowed]
		[Export ("city")]
		string City { get; set; }

		[NullAllowed]
		[Export ("stateOrProvince")]
		string StateOrProvince { get; set; }

		[NullAllowed]
		[Export ("country")]
		string Country { get; set; }

		[NullAllowed]
		[Export ("altitude", ArgumentSemantic.Strong)]
		NSNumber Altitude { get; set; }

		[NullAllowed]
		[Export ("latitude", ArgumentSemantic.Strong)]
		NSNumber Latitude { get; set; }

		[NullAllowed]
		[Export ("longitude", ArgumentSemantic.Strong)]
		NSNumber Longitude { get; set; }

		[NullAllowed]
		[Export ("speed", ArgumentSemantic.Strong)]
		NSNumber Speed { get; set; }

		[NullAllowed]
		[Export ("timestamp", ArgumentSemantic.Strong)]
		NSDate Timestamp { get; set; }

		[NullAllowed]
		[Export ("imageDirection", ArgumentSemantic.Strong)]
		NSNumber ImageDirection { get; set; }

		[NullAllowed]
		[Export ("namedLocation")]
		string NamedLocation { get; set; }

		[NullAllowed]
		[Export ("GPSTrack", ArgumentSemantic.Strong)]
		NSNumber GpsTrack { get; set; }

		[NullAllowed]
		[Export ("GPSStatus")]
		string GpsStatus { get; set; }

		[NullAllowed]
		[Export ("GPSMeasureMode")]
		string GpsMeasureMode { get; set; }

		[NullAllowed]
		[Export ("GPSDOP", ArgumentSemantic.Strong)]
		NSNumber GpsDop { get; set; }

		[NullAllowed]
		[Export ("GPSMapDatum")]
		string GpsMapDatum { get; set; }

		[NullAllowed]
		[Export ("GPSDestLatitude", ArgumentSemantic.Strong)]
		NSNumber GpsDestLatitude { get; set; }

		[NullAllowed]
		[Export ("GPSDestLongitude", ArgumentSemantic.Strong)]
		NSNumber GpsDestLongitude { get; set; }

		[NullAllowed]
		[Export ("GPSDestBearing", ArgumentSemantic.Strong)]
		NSNumber GpsDestBearing { get; set; }

		[NullAllowed]
		[Export ("GPSDestDistance", ArgumentSemantic.Strong)]
		NSNumber GpsDestDistance { get; set; }

		[NullAllowed]
		[Export ("GPSProcessingMethod")]
		string GpsProcessingMethod { get; set; }

		[NullAllowed]
		[Export ("GPSAreaInformation")]
		string GpsAreaInformation { get; set; }

		[NullAllowed]
		[Export ("GPSDateStamp", ArgumentSemantic.Strong)]
		NSDate GpsDateStamp { get; set; }

		[NullAllowed]
		[Export ("GPSDifferental", ArgumentSemantic.Strong)]
		NSNumber GpsDifferental { get; set; }

		[iOS (10,0)]
		[NullAllowed, Export ("fullyFormattedAddress")]
		string FullyFormattedAddress { get; set; }

		[iOS (10,0)]
		[NullAllowed, Export ("postalCode")]
		string PostalCode { get; set; }

		[iOS (10,0)]
		[NullAllowed, Export ("subThoroughfare")]
		string SubThoroughfare { get; set; }

		[iOS (10,0)]
		[NullAllowed, Export ("thoroughfare")]
		string Thoroughfare { get; set; }

		// CSActionExtras

		[NullAllowed, Export ("supportsPhoneCall", ArgumentSemantic.Strong)]
		NSNumber SupportsPhoneCall { get; set; }

		[NullAllowed, Export ("supportsNavigation", ArgumentSemantic.Strong)]
		NSNumber SupportsNavigation { get; set; }

		// CSContainment

		[NullAllowed, Export ("containerTitle")]
		string ContainerTitle { get; set; }

		[NullAllowed, Export ("containerDisplayName")]
		string ContainerDisplayName { get; set; }

		[NullAllowed, Export ("containerIdentifier")]
		string ContainerIdentifier { get; set; }

		[NullAllowed, Export ("containerOrder", ArgumentSemantic.Strong)]
		NSNumber ContainerOrder { get; set; }

		// CSCustomAttributes

		[Internal]
		[Export ("setValue:forCustomKey:")]
		void SetValue ([NullAllowed] INSSecureCoding value, CSCustomAttributeKey key);

		[Internal]
		[Export ("valueForCustomKey:")]
		[return: NullAllowed]
		INSSecureCoding ValueForCustomKey (CSCustomAttributeKey key);

		// CSSearchableItemAttributeSet_CSGeneral

		[iOS (11,0), NoTV, Mac (10, 11)]
		[NullAllowed, Export ("userCreated", ArgumentSemantic.Strong)]
		[Internal] // We would like to use [BindAs (typeof (bool?))]
		NSNumber _IsUserCreated { [Bind ("isUserCreated")] get; set; }

		[iOS (11, 0), NoTV, Mac (10, 11)]
		[NullAllowed, Export ("userOwned", ArgumentSemantic.Strong)]
		[Internal] // We would like to use[BindAs (typeof (bool?))]
		NSNumber _IsUserOwned { [Bind ("isUserOwned")] get; set; }

		[iOS (11, 0), NoTV, Mac (10, 11)]
		[NullAllowed, Export ("userCurated", ArgumentSemantic.Strong)]
		[Internal] // We would like to use [BindAs (typeof (bool?))]
		NSNumber _IsUserCurated { [Bind ("isUserCurated")] get; set; }

		[iOS (11, 0), NoTV, Mac (10, 11)]
		[NullAllowed, Export ("rankingHint", ArgumentSemantic.Strong)]
		NSNumber RankingHint { get; set; }

		// CSSearchableItemAttributeSet_CSItemProvider

		[iOS (11, 0), NoTV, Mac (10, 11)]
		[NullAllowed, Export ("providerDataTypeIdentifiers", ArgumentSemantic.Copy)]
		string[] ProviderDataTypeIdentifiers { get; set; }

		[iOS (11, 0), NoTV, Mac (10, 11)]
		[NullAllowed, Export ("providerFileTypeIdentifiers", ArgumentSemantic.Copy)]
		string[] ProviderFileTypeIdentifiers { get; set; }

		[iOS (11, 0), NoTV, Mac (10, 11)]
		[NullAllowed, Export ("providerInPlaceFileTypeIdentifiers", ArgumentSemantic.Copy)]
		string[] ProviderInPlaceFileTypeIdentifiers { get; set; }
	}

	[NoTV][iOS (10,0)]
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CSSearchQuery {
		[Export ("initWithQueryString:attributes:")]
		IntPtr Constructor (string queryString, [NullAllowed] string[] attributes);

		[Export ("cancelled")]
		bool Cancelled { [Bind ("isCancelled")] get; }

		[Export ("foundItemCount")]
		nuint FoundItemCount { get; }

		[NullAllowed, Export ("foundItemsHandler", ArgumentSemantic.Copy)]
		Action<CSSearchableItem[]> FoundItemsHandler { get; set; }

		[NullAllowed, Export ("completionHandler", ArgumentSemantic.Copy)]
		Action<NSError> CompletionHandler { get; set; }

		[Export ("protectionClasses", ArgumentSemantic.Copy)]
		string[] ProtectionClasses { get; set; }

		[Export ("start")]
		void Start ();

		[Export ("cancel")]
		void Cancel ();
	}
}

