using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using AVFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ShazamKit {

	[Native]
	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[ErrorDomain ("SHErrorDomain")]
	public enum SHErrorCode : long {
		InvalidAudioFormat = 100,
		AudioDiscontinuity = 101,
		SignatureInvalid = 200,
		SignatureDurationInvalid = 201,
		MatchAttemptFailed = 202,
		CustomCatalogInvalid = 300,
		CustomCatalogInvalidURL = 301,
		MediaLibrarySyncFailed = 400,
		InternalError = 500,
		MediaItemFetchFailed = 600,
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[Static]
	enum SHMediaItemProperty {
		[Field ("SHMediaItemShazamID")]
		ShazamId,
		[Field ("SHMediaItemTitle")]
		Title,
		[Field ("SHMediaItemSubtitle")]
		Subtitle,
		[Field ("SHMediaItemArtist")]
		Artist,
		[Field ("SHMediaItemWebURL")]
		WebUrl,
		[Field ("SHMediaItemAppleMusicID")]
		AppleMusicId,
		[Field ("SHMediaItemAppleMusicURL")]
		AppleMusicUrl,
		[Field ("SHMediaItemArtworkURL")]
		ArtworkUrl,
		[Field ("SHMediaItemVideoURL")]
		VideoUrl,
		[Field ("SHMediaItemExplicitContent")]
		ExplicitContent,
		[Field ("SHMediaItemGenres")]
		Genres,
		[Field ("SHMediaItemISRC")]
		Isrc,
		[Field ("SHMediaItemMatchOffset")]
		MatchOffset,
		[Field ("SHMediaItemFrequencySkew")]
		FrequencySkew,
		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Field ("SHMediaItemTimeRanges")]
		TimeRanges,
		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Field ("SHMediaItemFrequencySkewRanges")]
		FrequencySkewRanges,
		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("SHMediaItemCreationDate")]
		CreationDate,
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SHCatalog {
		[Export ("minimumQuerySignatureDuration")]
		double MinimumQuerySignatureDuration { get; }

		[Export ("maximumQuerySignatureDuration")]
		double MaximumQuerySignatureDuration { get; }
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (SHCatalog))]
	interface SHCustomCatalog {
		[Export ("addReferenceSignature:representingMediaItems:error:")]
		bool Add (SHSignature signature, SHMediaItem [] mediaItems, [NullAllowed] out NSError error);

		[Export ("addCustomCatalogFromURL:error:")]
		bool Add (NSUrl url, [NullAllowed] out NSError error);

		[Export ("writeToURL:error:")]
		bool Write (NSUrl url, [NullAllowed] out NSError error);

		[Static]
		[Export ("new")]
		[return: Release]
		SHCustomCatalog Create ();
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SHMatch : NSSecureCoding {
		[Export ("mediaItems", ArgumentSemantic.Strong)]
		SHMatchedMediaItem [] MediaItems { get; }

		[Export ("querySignature", ArgumentSemantic.Strong)]
		SHSignature QuerySignature { get; }
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (SHMediaItem))]
	[DisableDefaultCtor]
	interface SHMatchedMediaItem : NSSecureCoding {
		[Export ("frequencySkew")]
		float FrequencySkew { get; }

		[Export ("matchOffset")]
		double MatchOffset { get; }

		[Export ("predictedCurrentMatchOffset")]
		double PredictedCurrentMatchOffset { get; }
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SHMediaItem : NSSecureCoding, NSCopying {
		[NullAllowed]
		[Export ("shazamID")]
		string ShazamId { get; }

		[NullAllowed]
		[Export ("title")]
		string Title { get; }

		[NullAllowed]
		[Export ("subtitle")]
		string Subtitle { get; }

		[NullAllowed]
		[Export ("artist")]
		string Artist { get; }

		[Export ("genres", ArgumentSemantic.Strong)]
		string [] Genres { get; }

		[NullAllowed]
		[Export ("appleMusicID")]
		string AppleMusicId { get; }

		[NullAllowed]
		[Export ("appleMusicURL", ArgumentSemantic.Strong)]
		NSUrl AppleMusicUrl { get; }

		[NullAllowed]
		[Export ("webURL", ArgumentSemantic.Strong)]
		NSUrl WebUrl { get; }

		[NullAllowed]
		[Export ("artworkURL", ArgumentSemantic.Strong)]
		NSUrl ArtworkUrl { get; }

		[NullAllowed]
		[Export ("videoURL", ArgumentSemantic.Strong)]
		NSUrl VideoUrl { get; }

		[Export ("explicitContent")]
		bool ExplicitContent { get; }

		[NullAllowed]
		[Export ("isrc")]
		string Isrc { get; }

		[Static]
		[Export ("mediaItemWithProperties:")]
		SHMediaItem Create (NSDictionary<NSString, NSObject> properties);

		[Async]
		[Static]
		[Export ("fetchMediaItemWithShazamID:completionHandler:")]
		void FetchMediaItem (string shazamId, Action<SHMediaItem, NSError> completionHandler);

		[Export ("valueForProperty:")]
		NSObject GetValue (string property);

		[Export ("objectForKeyedSubscript:")]
		NSObject GetObject (string key);

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("timeRanges", ArgumentSemantic.Strong)]
		SHRange [] TimeRanges { get; }

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("frequencySkewRanges", ArgumentSemantic.Strong)]
		SHRange [] FrequencySkewRanges { get; }

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("creationDate", ArgumentSemantic.Strong)]
		NSDate CreationDate { get; }
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use SHLibrary instead.")]
	[Deprecated (PlatformName.iOS, 17, 0, message: "Use SHLibrary instead.")]
	[Deprecated (PlatformName.TvOS, 17, 0, message: "Use SHLibrary instead.")]
	[Deprecated (PlatformName.WatchOS, 10, 0, message: "Use SHLibrary instead.")]
	[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use SHLibrary instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SHMediaLibrary {
		[Static]
		[Export ("defaultLibrary", ArgumentSemantic.Strong)]
		SHMediaLibrary DefaultLibrary { get; }

		[Async]
		[Export ("addMediaItems:completionHandler:")]
		void Add (SHMediaItem [] mediaItems, Action<NSError> completionHandler);
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface SHSession {
		[Export ("catalog", ArgumentSemantic.Strong)]
		SHCatalog Catalog { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ISHSessionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("initWithCatalog:")]
		NativeHandle Constructor (SHCatalog catalog);

		[Export ("matchStreamingBuffer:atTime:")]
		void Match (AVAudioPcmBuffer buffer, [NullAllowed] AVAudioTime time);

		[Export ("matchSignature:")]
		void Match (SHSignature signature);
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SHSignature : NSSecureCoding, NSCopying {
		[Export ("initWithDataRepresentation:error:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData dataRepresentation, [NullAllowed] out NSError error);

		[Export ("duration")]
		double Duration { get; }

		[Export ("dataRepresentation", ArgumentSemantic.Strong)]
		NSData DataRepresentation { get; }

		[Static]
		[Export ("signatureWithDataRepresentation:error:")]
		[return: NullAllowed]
		SHSignature GetSignature (NSData dataRepresentation, [NullAllowed] out NSError error);
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface SHSignatureGenerator {
		[Export ("appendBuffer:atTime:error:")]
		bool Append (AVAudioPcmBuffer buffer, [NullAllowed] AVAudioTime time, [NullAllowed] out NSError error);

		[Export ("signature")]
		SHSignature Signature { get; }

		[Static, Async]
		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("generateSignatureFromAsset:completionHandler:")]
		void GenerateSignature (AVAsset asset, Action<SHSignature, NSError> completionHandler);
	}

	interface ISHSessionDelegate { }

	[iOS (15, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface SHSessionDelegate {
		[Export ("session:didFindMatch:")]
		void DidFindMatch (SHSession session, SHMatch match);

		[Export ("session:didNotFindMatchForSignature:error:")]
		void DidNotFindMatch (SHSession session, SHSignature signature, [NullAllowed] NSError error);
	}

	[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SHRange : NSSecureCoding, NSCopying {
		[Export ("initWithLowerBound:upperBound:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double lowerBound, double upperBound);

		[Export ("lowerBound")]
		double LowerBound { get; }

		[Export ("upperBound")]
		double UpperBound { get; }

		[Static]
		[Export ("rangeWithLowerBound:upperBound:")]
		SHRange CreateRange (double lowerBound, double upperBound);
	}
}
