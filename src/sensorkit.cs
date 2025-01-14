using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using CoreMedia;
using Speech;
using SoundAnalysis;

#if __MACCATALYST__
using ARFaceAnchor = Foundation.NSObject;
#else
using ARKit;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace SensorKit {

	// helpers for code generation
	interface NSUnitDuration : NSUnit { }
	interface NSUnitIlluminance : NSUnit { }
	interface NSUnitLength : NSUnit { }
	interface NSUnitElectricPotentialDifference : NSUnit { }
	interface NSUnitFrequency : NSUnit { }
	interface NSUnitAcceleration : NSUnit { }
	interface NSUnitTemperature : NSUnit { }

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum SRAmbientLightSensorPlacement : long {
		Unknown,
		FrontTop,
		FrontBottom,
		FrontRight,
		FrontLeft,
		FrontTopRight,
		FrontTopLeft,
		FrontBottomRight,
		FrontBottomLeft,
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum SRAuthorizationStatus : long {
		NotDetermined = 0,
		Authorized,
		Denied,
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum SRCrownOrientation : long {
		Left,
		Right,
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum SRDeletionReason : long {
		UserInitiated,
		LowDiskSpace,
		AgeLimit,
		NoInterestedClients,
		SystemInitiated,
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	[ErrorDomain ("SRErrorDomain")]
	enum SRErrorCode : long {
		InvalidEntitlement,
		NoAuthorization,
		DataInaccessible,
		FetchRequestInvalid,
		PromptDeclined,
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum SRLocationCategory : long {
		Unknown,
		Home,
		Work,
		School,
		Gym,
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum SRNotificationEvent : long {
		Unknown,
		Received,
		DefaultAction,
		SupplementaryAction,
		Clear,
		NotificationCenterClearAll,
		Removed,
		Hide,
		LongLook,
		Silence,
		AppLaunch,
		Expired,
		BannerPulldown,
		TapCoalesce,
		Deduped,
		DeviceActivated,
		DeviceUnlocked,
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum SRWristLocation : long {
		Left,
		Right,
	}

	[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum SRKeyboardMetricsSentimentCategory : long {
		Absolutist,
		Down,
		Death,
		Anxiety,
		Anger,
		Health,
		Positive,
		Sad,
		LowEnergy,
		Confused,
	}

	[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum SRTextInputSessionType : long {
		Keyboard = 1,
		ThirdPartyKeyboard,
		Pencil,
		Dictation,
	}

	[NoTV, NoMac, iOS (16, 4), MacCatalyst (16, 4)]
	[Native]
	public enum SRMediaEventType : long {
		OnScreen = 1,
		OffScreen,
	}

	[Flags, NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	[Native]
	public enum SRElectrocardiogramDataFlags : ulong {
		None = 0x0,
		SignalInvalid = 1uL << 0,
		CrownTouched = 1uL << 1,
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	[Native]
	public enum SRElectrocardiogramLead : long {
		RightArmMinusLeftArm = 1,
		LeftArmMinusRightArm,
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	[Native]
	public enum SRElectrocardiogramSessionState : long {
		Begin = 1,
		Active,
		End,
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	[Native]
	public enum SRElectrocardiogramSessionGuidance : long {
		Guided = 1,
		Unguided,
	}

	[Flags, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum SRFaceMetricsContext : ulong {
		DeviceUnlock = 1uL << 0,
		MessagingAppUsage = 1uL << 1,
	}

	[Flags, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum SRSpeechMetricsSessionFlags : ulong {
		Default = 0x0,
		BypassVoiceProcessing = (1uL << 0),
	}

	[Flags, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum SRWristTemperatureCondition : ulong {
		None = 0x0,
		OffWrist = 1uL << 0,
		OnCharger = 1uL << 1,
		InMotion = 1uL << 2,
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	enum SRDeviceUsageCategory {
		[Field ("SRDeviceUsageCategoryGames")]
		Games,

		[Field ("SRDeviceUsageCategoryBusiness")]
		Business,

		[Field ("SRDeviceUsageCategoryWeather")]
		Weather,

		[Field ("SRDeviceUsageCategoryUtilities")]
		Utilities,

		[Field ("SRDeviceUsageCategoryTravel")]
		Travel,

		[Field ("SRDeviceUsageCategorySports")]
		Sports,

		[Field ("SRDeviceUsageCategorySocialNetworking")]
		SocialNetworking,

		[Field ("SRDeviceUsageCategoryReference")]
		Reference,

		[Field ("SRDeviceUsageCategoryProductivity")]
		Productivity,

		[Field ("SRDeviceUsageCategoryPhotoAndVideo")]
		PhotoAndVideo,

		[Field ("SRDeviceUsageCategoryNews")]
		News,

		[Field ("SRDeviceUsageCategoryNavigation")]
		Navigation,

		[Field ("SRDeviceUsageCategoryMusic")]
		Music,

		[Field ("SRDeviceUsageCategoryLifestyle")]
		Lifestyle,

		[Field ("SRDeviceUsageCategoryHealthAndFitness")]
		HealthAndFitness,

		[Field ("SRDeviceUsageCategoryFinance")]
		Finance,

		[Field ("SRDeviceUsageCategoryEntertainment")]
		Entertainment,

		[Field ("SRDeviceUsageCategoryEducation")]
		Education,

		[Field ("SRDeviceUsageCategoryBooks")]
		Books,

		[Field ("SRDeviceUsageCategoryMedical")]
		Medical,

		[Field ("SRDeviceUsageCategoryNewsstand")]
		Newsstand,

		[Field ("SRDeviceUsageCategoryCatalogs")]
		Catalogs,

		[Field ("SRDeviceUsageCategoryKids")]
		Kids,

		[Field ("SRDeviceUsageCategoryMiscellaneous")]
		Miscellaneous,

		[Field ("SRDeviceUsageCategoryFoodAndDrink")]
		FoodAndDrink,

		[Field ("SRDeviceUsageCategoryDeveloperTools")]
		DeveloperTools,

		[Field ("SRDeviceUsageCategoryGraphicsAndDesign")]
		GraphicsAndDesign,

		[Field ("SRDeviceUsageCategoryShopping")]
		Shopping,

		[Field ("SRDeviceUsageCategoryStickers")]
		Stickers,
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRAmbientLightSample {

		[Export ("placement")]
		SRAmbientLightSensorPlacement Placement { get; }

		[Export ("chromaticity")]
		SRAmbientLightChromaticity Chromaticity { get; }

		[Export ("lux", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitIlluminance> Lux { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRApplicationUsage {

		[NullAllowed, Export ("bundleIdentifier")]
		string BundleIdentifier { get; }

		[Export ("usageTime")]
		double /* NSTimeInterval */ UsageTime { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("reportApplicationIdentifier")]
		string ReportApplicationIdentifier { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("textInputSessions", ArgumentSemantic.Copy)]
		SRTextInputSession [] TextInputSessions { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("supplementalCategories", ArgumentSemantic.Copy)]
		SRSupplementalCategory [] SupplementalCategories { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("relativeStartTime")]
		double RelativeStartTime { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRDeletionRecord : NSSecureCoding {

		[Export ("startTime")]
		double /* SRAbsoluteTime */ StartTime { get; }

		[Export ("endTime")]
		double /* SRAbsoluteTime */ EndTime { get; }

		[Export ("reason")]
		SRDeletionReason Reason { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRDevice : NSSecureCoding, NSCopying {
		[Static]
		[Export ("currentDevice")]
		SRDevice CurrentDevice { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("model")]
		string Model { get; }

		[Export ("systemName")]
		string SystemName { get; }

		[Export ("systemVersion")]
		string SystemVersion { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("productType")]
		string ProductType { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRDeviceUsageReport {

		[Export ("duration")]
		double /* NSTimeInterval */ Duration { get; }

		[Export ("applicationUsageByCategory", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSArray<SRApplicationUsage>> ApplicationUsageByCategory { get; }

		[Export ("notificationUsageByCategory", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSArray<SRNotificationUsage>> NotificationUsageByCategory { get; }

		[Export ("webUsageByCategory", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSArray<SRWebUsage>> WebUsageByCategory { get; }

		[Export ("totalScreenWakes")]
		nint TotalScreenWakes { get; }

		[Export ("totalUnlocks")]
		nint TotalUnlocks { get; }

		[Export ("totalUnlockDuration")]
		double /* NSTimeInterval */ TotalUnlockDuration { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("version")]
		string Version { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface SRFetchRequest {

		[Export ("from")]
		double /* SRAbsoluteTime */ From { get; set; }

		[Export ("to")]
		double /* SRAbsoluteTime */ To { get; set; }

		[Export ("device", ArgumentSemantic.Strong)]
		SRDevice Device { get; set; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRFetchResult<SampleType> : NSCopying where SampleType : NSObject {

		[Export ("sample", ArgumentSemantic.Copy)]
		SampleType Sample { get; }

		[Export ("timestamp")]
		double /* SRAbsoluteTime */ Timestamp { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRKeyboardProbabilityMetric<UnitType> where UnitType : NSUnit {
		[Export ("distributionSampleValues", ArgumentSemantic.Copy)]
		NSMeasurement<UnitType> [] DistributionSampleValues { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRKeyboardMetrics {

		[Export ("duration")]
		double /* NSTimeInterval */ Duration { get; }

		[Export ("keyboardIdentifier")]
		string KeyboardIdentifier { get; }

		[Export ("version")]
		string Version { get; }

		[Export ("width")]
		NSMeasurement<NSUnitLength> Width { get; }

		[Export ("height")]
		NSMeasurement<NSUnitLength> Height { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("inputModes", ArgumentSemantic.Copy)]
		string [] InputModes { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("sessionIdentifiers", ArgumentSemantic.Copy)]
		string [] SessionIdentifiers { get; }

		// SRKeyboardMetrics_ScalarMetrics

		[Export ("totalWords")]
		nint TotalWords { get; }

		[Export ("totalAlteredWords")]
		nint TotalAlteredWords { get; }

		[Export ("totalTaps")]
		nint TotalTaps { get; }

		[Export ("totalDrags")]
		nint TotalDrags { get; }

		[Export ("totalDeletes")]
		nint TotalDeletes { get; }

		[Export ("totalEmojis")]
		nint TotalEmojis { get; }

		[Export ("totalPaths")]
		nint TotalPaths { get; }

		[Export ("totalPathTime")]
		double TotalPathTime { get; }

		[Export ("totalPathLength")]
		NSMeasurement<NSUnitLength> TotalPathLength { get; }

		[Export ("totalAutoCorrections")]
		nint TotalAutoCorrections { get; }

		[Export ("totalSpaceCorrections")]
		nint TotalSpaceCorrections { get; }

		[Export ("totalRetroCorrections")]
		nint TotalRetroCorrections { get; }

		[Export ("totalTranspositionCorrections")]
		nint TotalTranspositionCorrections { get; }

		[Export ("totalInsertKeyCorrections")]
		nint TotalInsertKeyCorrections { get; }

		[Export ("totalSkipTouchCorrections")]
		nint TotalSkipTouchCorrections { get; }

		[Export ("totalNearKeyCorrections")]
		nint TotalNearKeyCorrections { get; }

		[Export ("totalSubstitutionCorrections")]
		nint TotalSubstitutionCorrections { get; }

		[Export ("totalHitTestCorrections")]
		nint TotalHitTestCorrections { get; }

		[Export ("totalTypingDuration")]
		double TotalTypingDuration { get; }

		// SRKeyboardMetrics_ProbabilityMetrics

		[Export ("upErrorDistance", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitLength> UpErrorDistance { get; }

		[Export ("downErrorDistance", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitLength> DownErrorDistance { get; }

		[Export ("spaceUpErrorDistance", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitLength> SpaceUpErrorDistance { get; }

		[Export ("spaceDownErrorDistance", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitLength> SpaceDownErrorDistance { get; }

		[Export ("deleteUpErrorDistance", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitLength> DeleteUpErrorDistance { get; }

		[Export ("deleteDownErrorDistance", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitLength> DeleteDownErrorDistance { get; }

		[Export ("shortWordCharKeyUpErrorDistance", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitLength> ShortWordCharKeyUpErrorDistance { get; }

		[Export ("shortWordCharKeyDownErrorDistance", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitLength> ShortWordCharKeyDownErrorDistance { get; }

		[Export ("touchDownUp", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> TouchDownUp { get; }

		[Export ("spaceTouchDownUp", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> SpaceTouchDownUp { get; }

		[Export ("deleteTouchDownUp", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> DeleteTouchDownUp { get; }

		[Export ("shortWordCharKeyTouchDownUp", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> ShortWordCharKeyTouchDownUp { get; }

		[Export ("touchDownDown", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> TouchDownDown { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("touchUpDown", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> TouchUpDown { get; }

		[Export ("charKeyToPrediction", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> CharKeyToPrediction { get; }

		[Export ("shortWordCharKeyToCharKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> ShortWordCharKeyToCharKey { get; }

		[Export ("charKeyToAnyTapKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> CharKeyToAnyTapKey { get; }

		[Export ("anyTapToCharKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> AnyTapToCharKey { get; }

		[Export ("spaceToCharKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> SpaceToCharKey { get; }

		[Export ("charKeyToSpaceKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> CharKeyToSpaceKey { get; }

		[Export ("spaceToDeleteKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> SpaceToDeleteKey { get; }

		[Export ("deleteToSpaceKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> DeleteToSpaceKey { get; }

		[Export ("spaceToSpaceKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> SpaceToSpaceKey { get; }

		[Export ("spaceToShiftKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> SpaceToShiftKey { get; }

		[Export ("spaceToPlaneChangeKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> SpaceToPlaneChangeKey { get; }

		[Export ("spaceToPredictionKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> SpaceToPredictionKey { get; }

		[Export ("deleteToCharKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> DeleteToCharKey { get; }

		[Export ("charKeyToDelete", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> CharKeyToDelete { get; }

		[Export ("deleteToDelete", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> DeleteToDelete { get; }

		[Export ("deleteToShiftKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> DeleteToShiftKey { get; }

		[Export ("deleteToPlaneChangeKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> DeleteToPlaneChangeKey { get; }

		[Export ("anyTapToPlaneChangeKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> AnyTapToPlaneChangeKey { get; }

		[Export ("planeChangeToAnyTap", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> PlaneChangeToAnyTap { get; }

		[Export ("charKeyToPlaneChangeKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> CharKeyToPlaneChangeKey { get; }

		[Export ("planeChangeKeyToCharKey", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> PlaneChangeKeyToCharKey { get; }

		[Export ("pathErrorDistanceRatio", ArgumentSemantic.Strong)]
		NSNumber [] PathErrorDistanceRatio { get; }

		[Export ("deleteToPath", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> DeleteToPath { get; }

		[Export ("pathToDelete", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> PathToDelete { get; }

		[Export ("spaceToPath", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> SpaceToPath { get; }

		[Export ("pathToSpace", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> PathToSpace { get; }

		[Export ("pathToPath", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> PathToPath { get; }

		// SRKeyboardMetrics_PositionalMetrics

		[Export ("longWordUpErrorDistance", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitLength> [] LongWordUpErrorDistance { get; }

		[Export ("longWordDownErrorDistance", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitLength> [] LongWordDownErrorDistance { get; }

		[Export ("longWordTouchDownUp", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> [] LongWordTouchDownUp { get; }

		[Export ("longWordTouchDownDown", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> [] LongWordTouchDownDown { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("longWordTouchUpDown", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> [] LongWordTouchUpDown { get; }

		[Export ("deleteToDeletes", ArgumentSemantic.Strong)]
		SRKeyboardProbabilityMetric<NSUnitDuration> [] DeleteToDeletes { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("pathTypingSpeed")]
		double PathTypingSpeed { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("totalPathPauses")]
		nint TotalPathPauses { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("totalPauses")]
		nint TotalPauses { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("totalTypingEpisodes")]
		nint TotalTypingEpisodes { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("typingSpeed")]
		double TypingSpeed { get; }

		// SRKeyboardMetrics_SentimentCounts

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("wordCountForSentimentCategory:")]
		nint WordCount (SRKeyboardMetricsSentimentCategory category);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("emojiCountForSentimentCategory:")]
		nint EmojiCount (SRKeyboardMetricsSentimentCategory category);
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRMessagesUsageReport {

		[Export ("duration")]
		double /* NSTimeInterval */ Duration { get; }

		[Export ("totalOutgoingMessages")]
		nint TotalOutgoingMessages { get; }

		[Export ("totalIncomingMessages")]
		nint TotalIncomingMessages { get; }

		[Export ("totalUniqueContacts")]
		nint TotalUniqueContacts { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRNotificationUsage {

		[NullAllowed, Export ("bundleIdentifier")]
		string BundleIdentifier { get; }

		[Export ("event")]
		SRNotificationEvent Event { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRPhoneUsageReport {

		[Export ("duration")]
		double /* NSTimeInterval */ Duration { get; }

		[Export ("totalOutgoingCalls")]
		nint TotalOutgoingCalls { get; }

		[Export ("totalIncomingCalls")]
		nint TotalIncomingCalls { get; }

		[Export ("totalUniqueContacts")]
		nint TotalUniqueContacts { get; }

		[Export ("totalPhoneCallDuration")]
		double /* NSTimeInterval */ TotalPhoneCallDuration { get; }
	}

	interface ISRSensorReaderDelegate { }

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface SRSensorReaderDelegate {

		[Export ("sensorReader:fetchingRequest:didFetchResult:")]
		bool DidFetchResult (SRSensorReader reader, SRFetchRequest fetchRequest, SRFetchResult<NSObject> result);

		[Export ("sensorReader:didCompleteFetch:")]
		void DidCompleteFetch (SRSensorReader reader, SRFetchRequest fetchRequest);

		[Export ("sensorReader:fetchingRequest:failedWithError:")]
		void FetchingRequestFailed (SRSensorReader reader, SRFetchRequest fetchRequest, NSError error);

		[Export ("sensorReader:didChangeAuthorizationStatus:")]
		void DidChangeAuthorizationStatus (SRSensorReader reader, SRAuthorizationStatus authorizationStatus);

		[Export ("sensorReaderWillStartRecording:")]
		void WillStartRecording (SRSensorReader reader);

		[Export ("sensorReader:startRecordingFailedWithError:")]
		void StartRecordingFailed (SRSensorReader reader, NSError error);

		[Export ("sensorReaderDidStopRecording:")]
		void DidStopRecording (SRSensorReader reader);

		[Export ("sensorReader:stopRecordingFailedWithError:")]
		void StopRecordingFailed (SRSensorReader reader, NSError error);

		[Export ("sensorReader:didFetchDevices:")]
		void DidFetchDevices (SRSensorReader reader, SRDevice [] devices);

		[Export ("sensorReader:fetchDevicesDidFailWithError:")]
		void FetchDevicesFailed (SRSensorReader reader, NSError error);
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	enum SRSensor {
		[Field (null)]
		Invalid = -1,

		[Field ("SRSensorAmbientLightSensor")]
		AmbientLightSensor,

		[Field ("SRSensorAccelerometer")]
		Accelerometer,

		[Field ("SRSensorRotationRate")]
		RotationRate,

		[Field ("SRSensorVisits")]
		Visits,

		[Field ("SRSensorPedometerData")]
		PedometerData,

		[Field ("SRSensorDeviceUsageReport")]
		DeviceUsageReport,

		[Field ("SRSensorMessagesUsageReport")]
		MessagesUsageReport,

		[Field ("SRSensorPhoneUsageReport")]
		PhoneUsageReport,

		[Field ("SRSensorOnWristState")]
		OnWristState,

		[Field ("SRSensorKeyboardMetrics")]
		KeyboardMetrics,

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("SRSensorSiriSpeechMetrics")]
		SiriSpeechMetrics,

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("SRSensorTelephonySpeechMetrics")]
		TelephonySpeechMetrics,

		[iOS (15, 4), MacCatalyst (15, 4)]
		[Field ("SRSensorAmbientPressure")]
		AmbientPressure,

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Field ("SRSensorMediaEvents")]
		MediaEvents,

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("SRSensorFaceMetrics")]
		FaceMetrics,

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("SRSensorHeartRate")]
		HeartRate,

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("SRSensorOdometer")]
		Odometer,

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("SRSensorWristTemperature")]
		WristTemperature,

		[iOS (17, 4), MacCatalyst (17, 4)]
		[Field ("SRSensorElectrocardiogram")]
		Electrocardiogram,

		[iOS (17, 4), MacCatalyst (17, 4)]
		[Field ("SRSensorPhotoplethysmogram")]
		Photoplethysmogram,
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRSensorReader {

		[Export ("initWithSensor:")]
		NativeHandle Constructor (NSString sensor);

		[Wrap ("this (sensor.GetConstant ()!)")]
		NativeHandle Constructor (SRSensor sensor);

		[Export ("startRecording")]
		void StartRecording ();

		[Export ("stopRecording")]
		void StopRecording ();

		[Export ("fetchDevices")]
		void FetchDevices ();

		[Export ("fetch:")]
		void Fetch (SRFetchRequest request);

		[Export ("authorizationStatus")]
		SRAuthorizationStatus AuthorizationStatus { get; }

		[Export ("sensor")]
		NSString WeakSensor { get; }

		SRSensor Sensor {
			[Wrap ("SRSensorExtensions.GetValue (WeakSensor)")]
			get;
		}

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ISRSensorReaderDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Async]
		[Static]
		[Export ("requestAuthorizationForSensors:completion:")]
		void RequestAuthorization (NSSet<NSString> sensors, Action<NSError> completion);
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRVisit {

		[Export ("distanceFromHome")]
		double /* CLLocationDistance */DistanceFromHome { get; }

		[Export ("arrivalDateInterval", ArgumentSemantic.Strong)]
		NSDateInterval ArrivalDateInterval { get; }

		[Export ("departureDateInterval", ArgumentSemantic.Strong)]
		NSDateInterval DepartureDateInterval { get; }

		[Export ("locationCategory")]
		SRLocationCategory LocationCategory { get; }

		[Export ("identifier", ArgumentSemantic.Strong)]
		NSUuid Identifier { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRWebUsage {

		[Export ("totalUsageTime")]
		double /* NSTimeInterval */ TotalUsageTime { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRWristDetection {

		[Export ("onWrist")]
		bool OnWrist { get; }

		[Export ("wristLocation")]
		SRWristLocation WristLocation { get; }

		[Export ("crownOrientation")]
		SRCrownOrientation CrownOrientation { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[NullAllowed]
		[Export ("onWristDate", ArgumentSemantic.Strong)]
		NSDate OnWristDate { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[NullAllowed]
		[Export ("offWristDate", ArgumentSemantic.Strong)]
		NSDate OffWristDate { get; }
	}

	[NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Category]
	[BaseType (typeof (NSString))]
	[Internal] // exposed thru SRSensor
	interface NSString_SRDeletionRecord {
		[return: NullAllowed]
		[Export ("sr_sensorForDeletionRecordsFromSensor")]
		NSString _GetSensorForDeletionRecordsFromSensor ();
	}

	[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface SRTextInputSession /* privately conforms to NSCoding and NSSecureCoding */
	{
		[Export ("duration")]
		double Duration { get; }

		[Export ("sessionType")]
		SRTextInputSessionType SessionType { get; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("sessionIdentifier")]
		string SessionIdentifier { get; }
	}

	[NoTV, NoMac, iOS (16, 4), MacCatalyst (16, 4)]
	[BaseType (typeof (NSObject))]
	interface SRMediaEvent : NSCopying, NSSecureCoding {

		[Export ("mediaIdentifier", ArgumentSemantic.Strong)]
		string MediaIdentifier { get; }

		[Export ("eventType", ArgumentSemantic.Assign)]
		SRMediaEventType EventType { get; }
	}

	[NoTV, NoMac, iOS (16, 4), MacCatalyst (16, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRSupplementalCategory : NSCopying, NSSecureCoding {

		[BindAs (typeof (SRDeviceUsageCategory))]
		[Export ("identifier")]
		NSString Identifier { get; }
	}

	[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRAudioLevel : NSCopying, NSSecureCoding {

		[Export ("timeRange", ArgumentSemantic.Assign)]
		CMTimeRange TimeRange { get; }

		[Export ("loudness")]
		double Loudness { get; }
	}

	[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRFaceMetricsExpression : NSCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("value")]
		double Value { get; }
	}

	[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRSpeechExpression : NSCopying, NSSecureCoding {

		[Export ("version")]
		string Version { get; }

		[Export ("timeRange", ArgumentSemantic.Assign)]
		CMTimeRange TimeRange { get; }

		[Export ("confidence")]
		double Confidence { get; }

		[Export ("mood")]
		double Mood { get; }

		[Export ("valence")]
		double Valence { get; }

		[Export ("activation")]
		double Activation { get; }

		[Export ("dominance")]
		double Dominance { get; }
	}

	[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRSpeechMetrics : NSCopying, NSSecureCoding {

		[Export ("sessionIdentifier")]
		string SessionIdentifier { get; }

		[Export ("sessionFlags", ArgumentSemantic.Assign)]
		SRSpeechMetricsSessionFlags SessionFlags { get; }

		[Export ("timestamp", ArgumentSemantic.Strong)]
		NSDate Timestamp { get; }

		[iOS (17, 2), MacCatalyst (17, 2)]
		[Export ("timeSinceAudioStart")]
		double TimeSinceAudioStart { get; }

		[NullAllowed, Export ("audioLevel", ArgumentSemantic.Strong)]
		SRAudioLevel AudioLevel { get; }

		[NullAllowed, Export ("speechRecognition", ArgumentSemantic.Strong)]
		SFSpeechRecognitionResult SpeechRecognition { get; }

		[NullAllowed, Export ("soundClassification", ArgumentSemantic.Strong)]
		SNClassificationResult SoundClassification { get; }

		[NullAllowed, Export ("speechExpression", ArgumentSemantic.Strong)]
		SRSpeechExpression SpeechExpression { get; }
	}

	[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRWristTemperature : NSCopying, NSSecureCoding {

		[Export ("timestamp", ArgumentSemantic.Strong)]
		NSDate Timestamp { get; }

		[Export ("value", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitTemperature> Value { get; }

		[Export ("condition")]
		SRWristTemperatureCondition Condition { get; }

		[Export ("errorEstimate", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitTemperature> ErrorEstimate { get; }
	}

	[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRWristTemperatureSession : NSCopying, NSSecureCoding {

		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[Export ("duration")]
		double Duration { get; }

		[Export ("version")]
		string Version { get; }

		[Export ("temperatures", ArgumentSemantic.Copy)]
		NSEnumerator<SRWristTemperature> Temperatures { get; }
	}

	[NoTV, NoMac, iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRFaceMetrics : NSCopying, NSSecureCoding {

		[Export ("version")]
		string Version { get; }

		[Export ("sessionIdentifier")]
		string SessionIdentifier { get; }

		[Export ("context", ArgumentSemantic.Assign)]
		SRFaceMetricsContext Context { get; }

		[Export ("faceAnchor", ArgumentSemantic.Copy)]
		ARFaceAnchor FaceAnchor { get; }

		[Export ("wholeFaceExpressions", ArgumentSemantic.Copy)]
		SRFaceMetricsExpression [] WholeFaceExpressions { get; }

		[Export ("partialFaceExpressions", ArgumentSemantic.Copy)]
		SRFaceMetricsExpression [] PartialFaceExpressions { get; }
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRElectrocardiogramData : NSCopying, NSSecureCoding {

		[Export ("flags", ArgumentSemantic.Assign)]
		SRElectrocardiogramDataFlags Flags { get; }

		[Export ("value", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitElectricPotentialDifference> Value { get; }
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRElectrocardiogramSample : NSCopying, NSSecureCoding {

		[Export ("date", ArgumentSemantic.Strong)]
		NSDate Date { get; }

		[Export ("frequency", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitFrequency> Frequency { get; }

		[Export ("session", ArgumentSemantic.Strong)]
		SRElectrocardiogramSession Session { get; }

		[Export ("lead", ArgumentSemantic.Assign)]
		SRElectrocardiogramLead Lead { get; }

		[Export ("data", ArgumentSemantic.Copy)]
		SRElectrocardiogramData [] Data { get; }
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRElectrocardiogramSession : NSCopying, NSSecureCoding {

		[Export ("state", ArgumentSemantic.Assign)]
		SRElectrocardiogramSessionState State { get; }

		[Export ("sessionGuidance", ArgumentSemantic.Assign)]
		SRElectrocardiogramSessionGuidance SessionGuidance { get; }

		[Export ("identifier")]
		string Identifier { get; }
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	public enum SRPhotoplethysmogramOpticalSampleCondition {
		[Field ("SRPhotoplethysmogramOpticalSampleConditionSignalSaturation")]
		SignalSaturation,
		[Field ("SRPhotoplethysmogramOpticalSampleConditionUnreliableNoise")]
		UnreliableNoise,
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRPhotoplethysmogramOpticalSample : NSCopying, NSSecureCoding {

		[Export ("emitter")]
		nint Emitter { get; }

		[Export ("activePhotodiodeIndexes", ArgumentSemantic.Strong)]
		NSIndexSet ActivePhotodiodeIndexes { get; }

		[Export ("signalIdentifier")]
		nint SignalIdentifier { get; }

		[Export ("nominalWavelength", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitLength> NominalWavelength { get; }

		[Export ("effectiveWavelength", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitLength> EffectiveWavelength { get; }

		[Export ("samplingFrequency", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitFrequency> SamplingFrequency { get; }

		[Export ("nanosecondsSinceStart")]
		long NanosecondsSinceStart { get; }

		[NullAllowed]
		[BindAs (typeof (double?))]
		[Export ("normalizedReflectance", ArgumentSemantic.Strong)]
		NSNumber NormalizedReflectance { get; }

		[NullAllowed]
		[BindAs (typeof (double?))]
		[Export ("whiteNoise", ArgumentSemantic.Strong)]
		NSNumber WhiteNoise { get; }

		[NullAllowed]
		[BindAs (typeof (double?))]
		[Export ("pinkNoise", ArgumentSemantic.Strong)]
		NSNumber PinkNoise { get; }

		[NullAllowed]
		[BindAs (typeof (double?))]
		[Export ("backgroundNoise", ArgumentSemantic.Strong)]
		NSNumber BackgroundNoise { get; }

		[NullAllowed]
		[BindAs (typeof (double?))]
		[Export ("backgroundNoiseOffset", ArgumentSemantic.Strong)]
		NSNumber BackgroundNoiseOffset { get; }

		[Export ("conditions", ArgumentSemantic.Copy)]
		NSString [] Conditions { get; }
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRPhotoplethysmogramAccelerometerSample : NSCopying, NSSecureCoding {

		[Export ("nanosecondsSinceStart")]
		long NanosecondsSinceStart { get; }

		[Export ("samplingFrequency", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitFrequency> SamplingFrequency { get; }

		[Export ("x", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitAcceleration> X { get; }

		[Export ("y", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitAcceleration> Y { get; }

		[Export ("z", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitAcceleration> Z { get; }
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	public enum SRPhotoplethysmogramSampleUsage {
		[Field ("SRPhotoplethysmogramSampleUsageForegroundHeartRate")]
		ForegroundHeartRate,
		[Field ("SRPhotoplethysmogramSampleUsageDeepBreathing")]
		DeepBreathing,
		[Field ("SRPhotoplethysmogramSampleUsageForegroundBloodOxygen")]
		ForegroundBloodOxygen,
		[Field ("SRPhotoplethysmogramSampleUsageBackgroundSystem")]
		BackgroundSystem,
	}

	[NoTV, NoMac, iOS (17, 4), MacCatalyst (17, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SRPhotoplethysmogramSample : NSCopying, NSSecureCoding {

		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[Export ("nanosecondsSinceStart")]
		long NanosecondsSinceStart { get; }

		[Export ("usage", ArgumentSemantic.Copy)]
		NSString [] Usage { get; }

		[Export ("opticalSamples", ArgumentSemantic.Copy)]
		SRPhotoplethysmogramOpticalSample [] OpticalSamples { get; }

		[Export ("accelerometerSamples", ArgumentSemantic.Copy)]
		SRPhotoplethysmogramAccelerometerSample [] AccelerometerSamples { get; }

		[NullAllowed, Export ("temperature", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitTemperature> Temperature { get; }
	}
}
