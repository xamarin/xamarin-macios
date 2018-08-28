//
// This file describes the API that the generator will produce
//
// Authors:
//   Alex Soto alex.soto@xamarin.com
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2014-2015 Xamarin Inc.
//

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using System;
using System.ComponentModel;
using CoreLocation;

namespace HealthKit {

	[Watch (3,0), iOS (10,0)]
	public enum HKDocumentTypeIdentifier {
		[Field ("HKDocumentTypeIdentifierCDA")]
		Cda,
	}

	// NSInteger -> HKDefines.h
	[Watch (2,0)]
	[iOS (8,0)]
	[ErrorDomain ("HKErrorDomain")]
	[Native]
	public enum HKErrorCode : long {
		NoError = 0,
		HealthDataUnavailable,
		HealthDataRestricted,
		InvalidArgument,
		AuthorizationDenied,
		AuthorizationNotDetermined,
		DatabaseInaccessible,
		UserCanceled,
		AnotherWorkoutSessionStarted,
		UserExitedWorkoutSession,
	}

	[iOS (10,0)]
	[Watch (2,0)]
	[Native]
	public enum HKWorkoutSessionLocationType : long {
		Unknown = 1,
		Indoor,
		Outdoor
	}

	[NoiOS]
	[Watch (2,0)]
	[Native]
	public enum HKWorkoutSessionState : long {
		NotStarted = 1,
		Running,
		Ended,
		[Watch (3,0)]
		Paused,
	}

	[iOS (11,0)]
	[Watch (4,0)]
	[Native]
	public enum HKHeartRateMotionContext : long {
		NotSet = 0,
		Sedentary,
		Active,
	}

	delegate void HKAnchoredObjectResultHandler2 (HKAnchoredObjectQuery query, HKSample[] results, nuint newAnchor, NSError error);
#if XAMCORE_2_0
	[Obsolete ("Use HKAnchoredObjectResultHandler2 instead")]
	delegate void HKAnchoredObjectResultHandler (HKAnchoredObjectQuery query, HKSampleType[] results, nuint newAnchor, NSError error);
#else
	delegate void HKAnchoredObjectResultHandler (HKAnchoredObjectQuery query, NSObject[] results, nuint newAnchor, NSError error);
#endif

	delegate void HKAnchoredObjectUpdateHandler (HKAnchoredObjectQuery query, HKSample[] addedObjects, HKDeletedObject[] deletedObjects, HKQueryAnchor newAnchor, NSError error);

	delegate void HKWorkoutRouteBuilderDataHandler (HKWorkoutRouteQuery query, CLLocation [] routeData, bool done, NSError error);

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException: The -init method is not available on HKAnchoredObjectQuery
	interface HKAnchoredObjectQuery {

		[NoWatch]
#if XAMCORE_2_0
		[Obsolete ("Use the overload that takes HKAnchoredObjectResultHandler2 instead")]
#endif
		[Availability (Introduced = Platform.iOS_8_0, Deprecated = Platform.iOS_9_0)]
		[Export ("initWithType:predicate:anchor:limit:completionHandler:")]
		IntPtr Constructor (HKSampleType type, [NullAllowed] NSPredicate predicate, nuint anchor, nuint limit, HKAnchoredObjectResultHandler completion);

		[NoWatch]
		[Sealed]
		[Availability (Introduced = Platform.iOS_8_0, Deprecated = Platform.iOS_9_0)]
		[Export ("initWithType:predicate:anchor:limit:completionHandler:")]
		IntPtr Constructor (HKSampleType type, [NullAllowed] NSPredicate predicate, nuint anchor, nuint limit, HKAnchoredObjectResultHandler2 completion);

		[iOS (9,0)]
		[Export ("initWithType:predicate:anchor:limit:resultsHandler:")]
		IntPtr Constructor (HKSampleType type, [NullAllowed] NSPredicate predicate, [NullAllowed] HKQueryAnchor anchor, nuint limit, HKAnchoredObjectUpdateHandler handler);

		[iOS (9,0)]
		[NullAllowed, Export ("updateHandler", ArgumentSemantic.Copy)]
		HKAnchoredObjectUpdateHandler UpdateHandler { get; set; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[Static]
	interface HKPredicateKeyPath {
		[Field ("HKPredicateKeyPathCategoryValue")]
		NSString CategoryValue { get; }

		[Field ("HKPredicateKeyPathSource")]
		NSString Source { get; }

		[Field ("HKPredicateKeyPathMetadata")]
		NSString Metadata { get; }
		
		[Field ("HKPredicateKeyPathQuantity")]
		NSString Quantity { get; }

		[Field ("HKPredicateKeyPathStartDate")]
		NSString StartDate { get; }

		[Field ("HKPredicateKeyPathEndDate")]
		NSString EndDate { get; }

		[Field ("HKPredicateKeyPathUUID")]
		NSString Uuid { get; }

		[Field ("HKPredicateKeyPathCorrelation")]
		NSString Correlation { get; }

		[Field ("HKPredicateKeyPathWorkout")]
		NSString Workout { get; }

		[Field ("HKPredicateKeyPathWorkoutDuration")]
		NSString WorkoutDuration { get; }
		
		[Field ("HKPredicateKeyPathWorkoutTotalDistance")]
		NSString WorkoutTotalDistance { get; }
		
		[Field ("HKPredicateKeyPathWorkoutTotalEnergyBurned")]
		NSString WorkoutTotalEnergyBurned { get; }
		
		[Field ("HKPredicateKeyPathWorkoutType")]
		NSString WorkoutType { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKPredicateKeyPathWorkoutTotalSwimmingStrokeCount")]
		NSString WorkoutTotalSwimmingStrokeCount { get; }

		[iOS (9,0)]
		[Field ("HKPredicateKeyPathDevice")]
		NSString Device { get; }

		[iOS (9,0)]
		[Field ("HKPredicateKeyPathSourceRevision")]
		NSString SourceRevision { get; }

		[iOS (9,3), Watch (2,2)]
		[Field ("HKPredicateKeyPathDateComponents")]
		NSString DateComponents { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKPredicateKeyPathCDATitle")]
		NSString CdaTitle { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKPredicateKeyPathCDAPatientName")]
		NSString CdaPatientName { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKPredicateKeyPathCDAAuthorName")]
		NSString CdaAuthorName { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKPredicateKeyPathCDACustodianName")]
		NSString CdaCustodianName { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Field ("HKPredicateKeyPathWorkoutTotalFlightsClimbed")]
		NSString TotalFlightsClimbed { get; }

		[Watch (5, 0), iOS (12, 0)]
		[Field ("HKPredicateKeyPathSum")]
		NSString PathSum { get; }

		[NoWatch, iOS (12, 0)]
		[Field ("HKPredicateKeyPathClinicalRecordFHIRResourceIdentifier")]
		NSString ClinicalRecordFhirResourceIdentifier { get; }

		[NoWatch, iOS (12, 0)]
		[Field ("HKPredicateKeyPathClinicalRecordFHIRResourceType")]
		NSString ClinicalRecordFhirResourceType { get; }
	}

	[NoWatch] // headers says it's available but it's only usable from another, unavailable, type
	[iOS (10,0)]
	[Static]
	[Internal]
	interface HKDetailedCdaErrorKeys {
		[Field ("HKDetailedCDAValidationErrorKey")]
		NSString ValidationErrorKey { get; }
	}

	[NoWatch]
	[iOS (10,0)]
	[StrongDictionary ("HKDetailedCdaErrorKeys")]
	[Internal]
	interface HKDetailedCdaErrors {
		NSString ValidationError { get; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (HKSample))]
	interface HKCategorySample {
		[Export ("categoryType")]
		HKCategoryType CategoryType { get; }

		[Export ("value")]
		nint Value { get; }

		[Static]
		[Export ("categorySampleWithType:value:startDate:endDate:metadata:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		HKCategorySample FromType (HKCategoryType type, nint value, NSDate startDate, NSDate endDate, [NullAllowed] NSDictionary metadata);

		[Static]
		[Wrap ("FromType (type, value, startDate, endDate, metadata != null ? metadata.Dictionary : null)")]
		HKCategorySample FromType (HKCategoryType type, nint value, NSDate startDate, NSDate endDate, HKMetadata metadata);

		[Static]
		[Export ("categorySampleWithType:value:startDate:endDate:")]
		HKCategorySample FromType (HKCategoryType type, nint value, NSDate startDate, NSDate endDate);

		[iOS (9,0)]
		[Static]
		[Export ("categorySampleWithType:value:startDate:endDate:device:metadata:")]
		HKCategorySample FromType (HKCategoryType type, nint value, NSDate startDate, NSDate endDate, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary<NSString,NSObject> metadata);
	}

	[Watch (3,0), iOS (10,0)]
	[BaseType (typeof(HKSample))]
	[Abstract] // as per docs
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKDocumentSample
	interface HKDocumentSample
	{
		[NoWatch] // HKDocumentType is iOS only, rdar #27865614
		[Export ("documentType", ArgumentSemantic.Strong)]
		HKDocumentType DocumentType { get; }
	}

	[NoWatch, iOS (10,0)]
	[BaseType (typeof(HKDocumentSample), Name = "HKCDADocumentSample")]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCDADocumentSample
	interface HKCdaDocumentSample
	{
		[NullAllowed, Export ("document")]
		HKCdaDocument Document { get; }

		[NoWatch]
		[Static]
		[Export ("CDADocumentSampleWithData:startDate:endDate:metadata:validationError:")]
		[return: NullAllowed]
		HKCdaDocumentSample Create (NSData documentData, NSDate startDate, NSDate endDate, [NullAllowed] NSDictionary metadata, out NSError validationError);

		[Static, Wrap ("Create (documentData, startDate, endDate, metadata != null ? metadata.Dictionary : null, out validationError)")]
		HKCdaDocumentSample Create (NSData documentData, NSDate startDate, NSDate endDate, HKMetadata metadata, out NSError validationError);
	}

	[Watch (3,0), iOS (10,0)]
	[BaseType (typeof(NSObject), Name = "HKCDADocument")]
	[DisableDefaultCtor] // as per docs
	interface HKCdaDocument
	{
		[NullAllowed, Export ("documentData", ArgumentSemantic.Copy)]
		NSData DocumentData { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("patientName")]
		string PatientName { get; }

		[Export ("authorName")]
		string AuthorName { get; }

		[Export ("custodianName")]
		string CustodianName { get; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor] // NSInvalidArgumentException: The -init method is not available on HKCorrelation
	interface HKCorrelation : NSSecureCoding {

		[Export ("objects")]
		NSSet Objects { get; }

		[Export ("objectsForType:")]
		NSSet GetObjects (HKObjectType objectType);
		
		[Export ("correlationType")]
		HKCorrelationType CorrelationType { get; }

		[Static, Export ("correlationWithType:startDate:endDate:objects:metadata:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		HKCorrelation Create (HKCorrelationType correlationType, NSDate startDate, NSDate endDate, NSSet objects, [NullAllowed] NSDictionary metadata);

		[Static, Wrap ("Create (correlationType, startDate, endDate, objects, metadata != null ? metadata.Dictionary : null)")]
		HKCorrelation Create (HKCorrelationType correlationType, NSDate startDate, NSDate endDate, NSSet objects, HKMetadata metadata);

		[Static, Export ("correlationWithType:startDate:endDate:objects:")]
		HKCorrelation Create (HKCorrelationType correlationType, NSDate startDate, NSDate endDate, NSSet objects);

		[iOS (9,0)]
		[Static]
		[Export ("correlationWithType:startDate:endDate:objects:device:metadata:")]
		HKCorrelation Create (HKCorrelationType correlationType, NSDate startDate, NSDate endDate, NSSet<HKSample> objects, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary<NSString,NSObject> metadata);
	}

	delegate void HKCorrelationQueryResultHandler (HKCorrelationQuery query, HKCorrelation[] correlations, NSError error);

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCorrelationQuery
	interface HKCorrelationQuery {
		[Export ("initWithType:predicate:samplePredicates:completion:")]
		IntPtr Constructor (HKCorrelationType correlationType, [NullAllowed] NSPredicate predicate, [NullAllowed] NSDictionary samplePredicates, HKCorrelationQueryResultHandler completion);

		[Export ("correlationType", ArgumentSemantic.Copy)]
		HKCorrelationType CorrelationType { get; }

		[Export ("samplePredicates", ArgumentSemantic.Copy)]
		NSDictionary SamplePredicates { get; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCorrelationType
	interface HKCorrelationType {

	}

	delegate void HKHealthStoreGetRequestStatusForAuthorizationToShareHandler (HKAuthorizationRequestStatus requestStatus, NSError error);
	delegate void HKHealthStoreRecoverActiveWorkoutSessionHandler (HKWorkoutSession session, NSError error);

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface HKHealthStore {
		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Static]
		[Export ("isHealthDataAvailable")]
		bool IsHealthDataAvailable { get; }

		[NoWatch, iOS (12, 0)]
		[Export ("supportsHealthRecords")]
		bool SupportsHealthRecords { get; }

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("authorizationStatusForType:")]
		HKAuthorizationStatus GetAuthorizationStatus (HKObjectType type);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Async]
		[Export ("requestAuthorizationToShareTypes:readTypes:completion:")]
		void RequestAuthorizationToShare (NSSet typesToShare, NSSet typesToRead, Action<bool, NSError> completion);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Async]
		[Export ("saveObject:withCompletion:")]
		void SaveObject (HKObject obj, Action<bool, NSError> completion);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Async]
		[Export ("saveObjects:withCompletion:")]
		void SaveObjects (HKObject[] objects, Action<bool, NSError> completion);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Async]
		[Export ("deleteObject:withCompletion:")]
		void DeleteObject (HKObject obj, Action<bool, NSError> completion);

		[iOS (9,0)]
		[Async]
		[Export ("deleteObjects:withCompletion:")]
		void DeleteObjects (HKObject[] objects, Action<bool, NSError> completion);

		[iOS (9,0)]
		[Export ("deleteObjectsOfType:predicate:withCompletion:")]
		void DeleteObjects (HKObjectType objectType, NSPredicate predicate, Action<bool, nuint, NSError> completion);

		[iOS (9,0)]
		[Export ("earliestPermittedSampleDate")]
		NSDate EarliestPermittedSampleDate { get; }

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("executeQuery:")]
		void ExecuteQuery (HKQuery query);

		[iOS (9,0)]
		[Export ("fitzpatrickSkinTypeWithError:")]
		[return: NullAllowed]
		HKFitzpatrickSkinTypeObject GetFitzpatrickSkinType (out NSError error);

		[Watch (3,0), iOS (10,0)]
		[Export ("wheelchairUseWithError:")]
		[return: NullAllowed]
		HKWheelchairUseObject GetWheelchairUse (out NSError error);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("stopQuery:")]
		void StopQuery (HKQuery query);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'GetDateOfBirthComponents' instead.")]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'GetDateOfBirthComponents' instead.")]
		[Export ("dateOfBirthWithError:")]
		NSDate GetDateOfBirth (out NSError error);

		[Watch (3,0), iOS (10,0)]
		[Export ("dateOfBirthComponentsWithError:")]
		[return: NullAllowed]
		NSDateComponents GetDateOfBirthComponents (out NSError error);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("biologicalSexWithError:")]
		HKBiologicalSexObject GetBiologicalSex (out NSError error);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("bloodTypeWithError:")]
		HKBloodTypeObject GetBloodType (out NSError error);

		[NoWatch]
		[Async]
		[Export ("enableBackgroundDeliveryForType:frequency:withCompletion:")]
		void EnableBackgroundDelivery (HKObjectType type, HKUpdateFrequency frequency, Action<bool, NSError> completion);

		[NoWatch]
		[Async]
		[Export ("disableBackgroundDeliveryForType:withCompletion:")]
		void DisableBackgroundDelivery (HKObjectType type, Action<bool, NSError> completion);

		[NoWatch]
		[Async]
		[Export ("disableAllBackgroundDeliveryWithCompletion:")]
		void DisableAllBackgroundDelivery (Action<bool, NSError> completion);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[NoWatch]
		[Async]
		[iOS (9,0)]
		[Export ("handleAuthorizationForExtensionWithCompletion:")]
		void HandleAuthorizationForExtension (Action<bool, NSError> completion);

		[iOS (9,0)]
		[Export ("splitTotalEnergy:startDate:endDate:resultsHandler:")]
		void SplitTotalEnergy (HKQuantity totalEnergy, NSDate startDate, NSDate endDate, Action<HKQuantity, HKQuantity, NSError> resultsHandler);

		// HKWorkout category

		[Export ("addSamples:toWorkout:completion:")]
		void AddSamples (HKSample [] samples, HKWorkout workout, HKStoreSampleAddedCallback callback);

		[NoiOS]
		[Watch (2,0)]
		[Export ("startWorkoutSession:")]
		void StartWorkoutSession (HKWorkoutSession workoutSession);

		[NoiOS]
		[Watch (2,0)]
		[Export ("endWorkoutSession:")]
		void EndWorkoutSession (HKWorkoutSession workoutSession);

		[Watch (3,0), NoiOS]
		[Export ("pauseWorkoutSession:")]
		void PauseWorkoutSession (HKWorkoutSession workoutSession);

		[Watch (3,0), NoiOS]
		[Export ("resumeWorkoutSession:")]
		void ResumeWorkoutSession (HKWorkoutSession workoutSession);

		[NoWatch, iOS (10,0)]
		[Async]
		[Export ("startWatchAppWithWorkoutConfiguration:completion:")]
		void StartWatchApp (HKWorkoutConfiguration workoutConfiguration, Action<bool, NSError> completion);

		// HKUserPreferences category

		[iOS (8,2)]
		[Async]
		[Export ("preferredUnitsForQuantityTypes:completion:")]
		void GetPreferredUnits (NSSet quantityTypes, Action<NSDictionary, NSError> completion);

		[iOS (8,2)]
		[Notification]
		[Field ("HKUserPreferencesDidChangeNotification")]
		NSString UserPreferencesDidChangeNotification { get; }

		[Async]
		[Watch (5,0), iOS (12,0)]
		[Export ("getRequestStatusForAuthorizationToShareTypes:readTypes:completion:")]
		void GetRequestStatusForAuthorizationToShare (NSSet<HKSampleType> typesToShare, NSSet<HKObjectType> typesToRead, HKHealthStoreGetRequestStatusForAuthorizationToShareHandler completion);

		[Async]
		[Watch (5,0), NoiOS]
		[Export ("recoverActiveWorkoutSessionWithCompletion:")]
		void RecoverActiveWorkoutSession (HKHealthStoreRecoverActiveWorkoutSessionHandler completion);
	}

	delegate void HKStoreSampleAddedCallback (bool success, NSError error);
	
	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface HKBiologicalSexObject : NSCopying, NSSecureCoding {
		[Export ("biologicalSex")]
		HKBiologicalSex BiologicalSex { get; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface HKBloodTypeObject : NSCopying, NSSecureCoding {
		[Export ("bloodType")]
		HKBloodType BloodType { get; }
	}

	[StrongDictionary ("HKMetadataKey")]
	interface HKMetadata {
		[Export ("FoodType")]
		string FoodType { get; set; }

		[Export ("UdiDeviceIdentifier")]
		string UdiDeviceIdentifier { get; set; }

		[Export ("UdiProductionIdentifier")]
		string UdiProductionIdentifier { get; set; }

		[Export ("DigitalSignature")]
		string DigitalSignature { get; set; }

		[Export ("ExternalUuid")]
		string ExternalUuid { get; set; }

		[Export ("DeviceSerialNumber")]
		string DeviceSerialNumber { get; set; }

		[Export ("BodyTemperatureSensorLocation")]
		HKBodyTemperatureSensorLocation BodyTemperatureSensorLocation { get; set; }

		[Export ("HeartRateSensorLocation")]
		HKHeartRateSensorLocation HeartRateSensorLocation { get; set; }

		[Export ("TimeZone")]
		NSTimeZone TimeZone { get; set; }

		[Export ("DeviceName")]
		string DeviceName { get; set; }

		[Export ("DeviceManufacturerName")]
		string DeviceManufacturerName { get; set; }
		
		[Export ("WasTakenInLab")]
		bool WasTakenInLab { get; set; }

		[Export ("ReferenceRangeLowerLimit")]
		NSNumber ReferenceRangeLowerLimit { get; set; }
		
		[Export ("ReferenceRangeUpperLimit")]
		NSNumber ReferenceRangeUpperLimit { get; set; }
		
		[Export ("WasUserEntered")]
		bool WasUserEntered { get; set; }
		
		[Export ("WorkoutBrandName")]
		string WorkoutBrandName { get; set; }
		
		[Export ("GroupFitness")]
		bool GroupFitness { get; set; }
		
		[Export ("IndoorWorkout")]
		bool IndoorWorkout { get; set; }
		
		[Export ("CoachedWorkout")]
		bool CoachedWorkout { get; set; }

		[iOS (9,0)]
		[Export ("SexualActivityProtectionUsed")]
		bool SexualActivityProtectionUsed { get; set; }

		[iOS (9,0)]
		[Export ("MenstrualCycleStart")]
		bool MenstrualCycleStart { get; set; }

		[Watch (3,0), iOS (10,0)]
		[Export ("WeatherCondition")]
		HKWeatherCondition WeatherCondition { get; }

		[Watch (3,0), iOS (10,0)]
		[Export ("WeatherTemperature")]
		HKQuantity WeatherTemperature { get; }

		[Watch (3,0), iOS (10,0)]
		[Export ("WeatherHumidity")]
		HKQuantity WeatherHumidity { get; }

		[Watch (3,0), iOS (10,0)]
		[Export ("LapLength")]
		NSString LapLength { get; }

		[Watch (3,0), iOS (10,0)]
		[Export ("SwimmingLocationType")]
		NSString SwimmingLocationType { get; }

		[Watch (3,0), iOS (10,0)]
		[Export ("SwimmingStrokeStyle")]
		NSString SwimmingStrokeStyle { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Export ("SyncIdentifier")]
		string SyncIdentifier { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Export ("SyncVersion")]
		int SyncVersion { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Export ("InsulinDeliveryReason")]
		HKInsulinDeliveryReason InsulinDeliveryReason { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Export ("BloodGlucoseMealTime")]
		HKBloodGlucoseMealTime BloodGlucoseMealTime { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Export ("VO2MaxTestType")]
		HKVO2MaxTestType VO2MaxTestType { get; }
        
		[Watch (4,0), iOS (11,0)]
		[Export ("HeartRateMotionContext")]
		HKHeartRateMotionContext HeartRateMotionContext { get; }

		[Watch (4,2), iOS (11,2)]
		[Export ("AverageSpeed")]
		HKQuantity AverageSpeed { get; set; }

		[Watch (4,2), iOS (11,2)]
		[Export ("MaximumSpeed")]
		HKQuantity MaximumSpeed { get; set; }

		[Watch (4,2), iOS (11,2)]
		[Export ("AlpineSlopeGrade")]
		HKQuantity AlpineSlopeGrade { get; set; }

		[Watch (4,2), iOS (11,2)]
		[Export ("ElevationAscended")]
		HKQuantity ElevationAscended { get; set; }

		[Watch (4,2), iOS (11,2)]
		[Export ("ElevationDescended")]
		HKQuantity ElevationDescended { get; set; }

		[Watch (5,0), iOS (12,0)]
		[Export ("FitnessMachineDuration")]
		HKQuantity FitnessMachineDuration { get; set; }

		[Watch (5, 0), iOS (12, 0)]
		[Export ("IndoorBikeDistance")]
		HKQuantity IndoorBikeDistance { get; set; }

		[Watch (5, 0), iOS (12, 0)]
		[Export ("CrossTrainerDistance")]
		HKQuantity CrossTrainerDistance { get; set; }
	}
		
	[Watch (2,0)]
	[iOS (8,0)]
	[Static]
	interface HKMetadataKey {
		[Field ("HKMetadataKeyDeviceSerialNumber")]
		NSString DeviceSerialNumber { get; }

		[Field ("HKMetadataKeyBodyTemperatureSensorLocation")]
		NSString BodyTemperatureSensorLocation { get; }

		[Field ("HKMetadataKeyHeartRateSensorLocation")]
		NSString HeartRateSensorLocation { get; }

		[Field ("HKMetadataKeyFoodType")]
		NSString FoodType { get; }

		[Field ("HKMetadataKeyUDIDeviceIdentifier")]
		NSString UdiDeviceIdentifier { get; }

		[Field ("HKMetadataKeyUDIProductionIdentifier")]
		NSString UdiProductionIdentifier { get; }

		[Field ("HKMetadataKeyDigitalSignature")]
		NSString DigitalSignature { get; }

		[Field ("HKMetadataKeyExternalUUID")]
		NSString ExternalUuid { get; }

		[Field ("HKMetadataKeyTimeZone")]
		NSString TimeZone { get; }

		[Field ("HKMetadataKeyDeviceName")]
		NSString DeviceName { get; }

		[Field ("HKMetadataKeyDeviceManufacturerName")]
		NSString DeviceManufacturerName { get; }
		
		[Field ("HKMetadataKeyWasTakenInLab")]
		NSString WasTakenInLab { get; }

		[Field ("HKMetadataKeyReferenceRangeLowerLimit")]
		NSString ReferenceRangeLowerLimit { get; }

		[Field ("HKMetadataKeyReferenceRangeUpperLimit")]
		NSString ReferenceRangeUpperLimit { get; }
		
		[Field ("HKMetadataKeyWasUserEntered")]
		NSString WasUserEntered { get; }
		
		[Field ("HKMetadataKeyWorkoutBrandName")]
		NSString WorkoutBrandName { get; }
		
		[Field ("HKMetadataKeyGroupFitness")]
		NSString GroupFitness { get; }
		
		[Field ("HKMetadataKeyIndoorWorkout")]
		NSString IndoorWorkout { get; }
		
		[Field ("HKMetadataKeyCoachedWorkout")]
		NSString CoachedWorkout { get; }

		[iOS (9,0)]
		[Field ("HKMetadataKeySexualActivityProtectionUsed")]
		NSString SexualActivityProtectionUsed { get; }

		[iOS (9,0)]
		[Field ("HKMetadataKeyMenstrualCycleStart")]
		NSString MenstrualCycleStart { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKMetadataKeyWeatherCondition")]
		NSString WeatherCondition { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKMetadataKeyWeatherTemperature")]
		NSString WeatherTemperature { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKMetadataKeyWeatherHumidity")]
		NSString WeatherHumidity { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKMetadataKeyLapLength")]
		NSString LapLength { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKMetadataKeySwimmingLocationType")]
		NSString SwimmingLocationType { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKMetadataKeySwimmingStrokeStyle")]
		NSString SwimmingStrokeStyle { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Field ("HKMetadataKeySyncIdentifier")]
		NSString SyncIdentifier { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Field ("HKMetadataKeySyncVersion")]
		NSString SyncVersion { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Field ("HKMetadataKeyInsulinDeliveryReason")]
		NSString InsulinDeliveryReason { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Field ("HKMetadataKeyBloodGlucoseMealTime")]
		NSString BloodGlucoseMealTime { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Field ("HKMetadataKeyVO2MaxTestType")]
		NSString VO2MaxTestType { get; }
        
		[Watch (4,0), iOS (11,0)]
		[Field ("HKMetadataKeyHeartRateMotionContext")]
		NSString HeartRateMotionContext { get; }

		[Watch (4,2), iOS (11,2)]
		[Field ("HKMetadataKeyAverageSpeed")]
		NSString AverageSpeed { get; }

		[Watch (4,2), iOS (11,2)]
		[Field ("HKMetadataKeyMaximumSpeed")]
		NSString MaximumSpeed { get; }

		[Watch (4,2), iOS (11,2)]
		[Field ("HKMetadataKeyAlpineSlopeGrade")]
		NSString AlpineSlopeGrade { get; }

		[Watch (4,2), iOS (11,2)]
		[Field ("HKMetadataKeyElevationAscended")]
		NSString ElevationAscended { get; }

		[Watch (4,2), iOS (11,2)]
		[Field ("HKMetadataKeyElevationDescended")]
		NSString ElevationDescended { get; }

		[Watch (5,0), iOS (12,0)]
		[Field ("HKMetadataKeyFitnessMachineDuration")]
		NSString FitnessMachineDuration { get; }

		[Watch (5, 0), iOS (12, 0)]
		[Field ("HKMetadataKeyIndoorBikeDistance")]
		NSString IndoorBikeDistance { get; }

		[Watch (5, 0), iOS (12, 0)]
		[Field ("HKMetadataKeyCrossTrainerDistance")]
		NSString CrossTrainerDistance { get; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
#if XAMCORE_4_0
	[Abstract] // as per docs
#endif
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKObject : NSSecureCoding {
		[Export ("UUID", ArgumentSemantic.Strong)]
		NSUuid Uuid { get; }

		[Availability (Introduced = Platform.iOS_8_0, Deprecated = Platform.iOS_9_0)]
		[Export ("source", ArgumentSemantic.Strong)]
		HKSource Source { get; }

		[Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary WeakMetadata { get; }

		[Wrap ("WeakMetadata")]
		HKMetadata Metadata { get; }

		[iOS (9,0)]
		[Export ("sourceRevision", ArgumentSemantic.Strong)]
		HKSourceRevision SourceRevision { get; }

		[iOS (9,0)]
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		HKDevice Device { get; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
#if XAMCORE_4_0
	[Abstract]
#endif
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKObjectType : NSSecureCoding, NSCopying {
		// These identifiers come from HKTypeIdentifiers
		[Export ("identifier")]
		NSString Identifier { get; }

#if XAMCORE_4_0 || WATCH
		[Internal]
#else
		[Obsolete ("Use 'HKQuantityType.Create (HKQuantityTypeIdentifier)'.")]
#endif
		[Static]
		[Export ("quantityTypeForIdentifier:")]
		[return: NullAllowed]
		HKQuantityType GetQuantityType (NSString hkTypeIdentifier);

#if XAMCORE_4_0 || WATCH
		[Internal]
#else
		[Obsolete ("Use 'HKCategoryType.Create (HKCategoryTypeIdentifier)'.")]
#endif
		[Static]
		[Export ("categoryTypeForIdentifier:")]
		[return: NullAllowed]
		HKCategoryType GetCategoryType (NSString hkCategoryTypeIdentifier);

#if XAMCORE_4_0 || WATCH
		[Internal]
#else
		[Obsolete ("Use 'HKCharacteristicType.Create (HKCharacteristicTypeIdentifier)'.")]
#endif
		[Static]
		[Export ("characteristicTypeForIdentifier:")]
		[return: NullAllowed]
		HKCharacteristicType GetCharacteristicType (NSString hkCharacteristicTypeIdentifier);

#if XAMCORE_4_0 || WATCH
		[Internal]
#else
		[Obsolete ("Use 'HKCorrelationType.Create (HKCorrelationTypeIdentifier)'.")]
#endif
		[Static, Export ("correlationTypeForIdentifier:")]
		[return: NullAllowed]
		HKCorrelationType GetCorrelationType (NSString hkCorrelationTypeIdentifier);

		[NoWatch] // HKDocumentType is iOS only, rdar #27865614
		[iOS (10,0)]
		[Internal]
		[Static]
		[Export ("documentTypeForIdentifier:")]
		[return: NullAllowed]
		HKDocumentType _GetDocumentType (NSString hkDocumentTypeIdentifier);

		[Static, Export ("workoutType")]
#if XAMCORE_4_0
		HKWorkoutType WorkoutType { get; }
#else
		HKWorkoutType GetWorkoutType ();
#endif

		[Watch (2,2)]
		[iOS (9,3)]
		[Static]
		[Export ("activitySummaryType")]
		HKActivitySummaryType ActivitySummaryType { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Static]
		[Export ("seriesTypeForIdentifier:")]
		[return: NullAllowed]
		HKSeriesType GetSeriesType (string identifier);

		[Watch (5,0), iOS (12,0)]
		[Static, Internal]
		[Export ("clinicalTypeForIdentifier:")]
		[return: NullAllowed]
		HKClinicalType GetClinicalType (NSString identifier);

		[Watch (5,0), iOS (12,0)]
		[Static]
		[Wrap ("GetClinicalType (identifier.GetConstant ())")]
		HKClinicalType GetClinicalType (HKClinicalTypeIdentifier identifier);
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKObjectType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCharacteristicType
	interface HKCharacteristicType {

	}

	[Watch (2,0)]
	[iOS (8,0)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSampleType
	[BaseType (typeof (HKObjectType))]
#if XAMCORE_2_0
	[Abstract] // The HKSampleType class is an abstract subclass of the HKObjectType class, used to represent data samples. Never instantiate an HKSampleType object directly. Instead, you should always work with one of its concrete subclasses [...]
#endif
	interface HKSampleType {

	}

	[Watch (5,0)]
	[iOS (12,0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKClinicalType 
	interface HKClinicalType {

	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCategoryType
	interface HKCategoryType {

	}

	[NoWatch] // marked as iOS-only (confirmed by Apple) even if some watchOS 3 API returns this type, rdar #27865614
	[iOS (10,0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKDocumentType
	interface HKDocumentType {

	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKQuantityType
	interface HKQuantityType {
		[Export ("aggregationStyle")]
		HKQuantityAggregationStyle AggregationStyle { get; }

		[Export ("isCompatibleWithUnit:")]
		bool IsCompatible (HKUnit unit);
	}

#if XAMCORE_2_0
	delegate void HKObserverQueryUpdateHandler (HKObserverQuery query, [BlockCallback] Action completion, NSError error);
#else
	delegate void HKObserverQueryCompletionHandler ();
	delegate void HKObserverQueryUpdateHandler (HKObserverQuery query, [BlockCallback] HKObserverQueryCompletionHandler completion, NSError error);
#endif

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
#if XAMCORE_4_0
	[Abstract]
#endif
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKObserverQuery
	interface HKObserverQuery {
		[Export ("initWithSampleType:predicate:updateHandler:")]
		IntPtr Constructor (HKSampleType sampleType, [NullAllowed] NSPredicate predicate, HKObserverQueryUpdateHandler updateHandler);
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKQuantity : NSSecureCoding, NSCopying {
		[Static]
		[Export ("quantityWithUnit:doubleValue:")]
		HKQuantity FromQuantity (HKUnit unit, double value);

		[Export ("isCompatibleWithUnit:")]
		bool IsCompatible (HKUnit unit);

		[Export ("doubleValueForUnit:")]
		double GetDoubleValue (HKUnit unit);

		[Export ("compare:")]
		NSComparisonResult Compare (HKQuantity quantity);
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKQuantitySample
	interface HKQuantitySample {
		[Export ("quantityType", ArgumentSemantic.Strong)]
		HKQuantityType QuantityType { get; }

		[Export ("quantity", ArgumentSemantic.Strong)]
		HKQuantity Quantity { get; }

		[Static]
		[Export ("quantitySampleWithType:quantity:startDate:endDate:")]
		HKQuantitySample FromType (HKQuantityType quantityType, HKQuantity quantity, NSDate startDate, NSDate endDate);

		[Static]
		[Export ("quantitySampleWithType:quantity:startDate:endDate:metadata:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		HKQuantitySample FromType (HKQuantityType quantityType, HKQuantity quantity, NSDate startDate, NSDate endDate, [NullAllowed] NSDictionary metadata);

		[Static]
		[Wrap ("FromType (quantityType, quantity, startDate, endDate, metadata != null ? metadata.Dictionary : null)")]
		HKQuantitySample FromType (HKQuantityType quantityType, HKQuantity quantity, NSDate startDate, NSDate endDate, HKMetadata metadata);

		[iOS (9,0)]
		[Static]
		[Export ("quantitySampleWithType:quantity:startDate:endDate:device:metadata:")]
		HKQuantitySample FromType (HKQuantityType quantityType, HKQuantity quantity, NSDate startDate, NSDate endDate, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary<NSString,NSObject> metadata);

		[Watch (5, 0), iOS (12, 0)]
		[Export ("count")]
		nint Count { get; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKQuery {
		[iOS (9,3), Watch (2,2)]
		[NullAllowed, Export ("objectType", ArgumentSemantic.Strong)]
		HKObjectType ObjectType { get; }

		[Deprecated (PlatformName.WatchOS, 2,2, message: "Use 'ObjectType' property.")]
		[Deprecated (PlatformName.iOS, 9,3, message: "Use 'ObjectType' property.")]
		[Watch (2,0)]
		[NullAllowed, Export ("sampleType", ArgumentSemantic.Strong)]
		HKSampleType SampleType { get; }

		[Export ("predicate", ArgumentSemantic.Strong)]
		NSPredicate Predicate { get; }

		// HKQuery (HKObjectPredicates) Category

		[Static]
		[Export ("predicateForObjectsWithMetadataKey:")]
		NSPredicate GetPredicateForMetadataKey (NSString metadataKey);

		[Static]
		[Export ("predicateForObjectsWithMetadataKey:allowedValues:")]
		NSPredicate GetPredicateForMetadataKey (NSString metadataKey, NSObject[] allowedValues);

		[Static]
		[Export ("predicateForObjectsWithMetadataKey:operatorType:value:")]
		NSPredicate GetPredicateForMetadataKey (NSString metadataKey, NSPredicateOperatorType operatorType, NSObject value);

		[Static]
		[Export ("predicateForObjectsFromSource:")]
		NSPredicate GetPredicateForObjectsFromSource (HKSource source);

		[Static]
		[Export ("predicateForObjectsFromSources:")]
		NSPredicate GetPredicateForObjectsFromSources (NSSet sources);

		[Static]
		[Export ("predicateForObjectWithUUID:")]
		NSPredicate GetPredicateForObject (NSUuid objectUuid);

		[Static]
		[Export ("predicateForObjectsWithUUIDs:")]
		NSPredicate GetPredicateForObjects (NSSet objectUuids);

		[iOS (9,0)]
		[Static]
		[Export ("predicateForObjectsFromDevices:")]
		NSPredicate GetPredicateForObjectsFromDevices (NSSet<HKDevice> devices);

		[iOS (9,0)]
		[Static]
		[Export ("predicateForObjectsWithDeviceProperty:allowedValues:")]
		NSPredicate GetPredicateForObjectsWithDeviceProperty (string key, NSSet<NSString> allowedValues);

		[iOS (9,0)]
		[Static]
		[Export ("predicateForObjectsFromSourceRevisions:")]
		NSPredicate GetPredicateForObjectsFromSourceRevisions (NSSet<HKSourceRevision> sourceRevisions);

		// HKQuery (HKQuantitySamplePredicates) Category

		[Static]
		[Export ("predicateForQuantitySamplesWithOperatorType:quantity:")]
		NSPredicate GetPredicateForQuantitySamples (NSPredicateOperatorType operatorType, HKQuantity quantity);

		// HKQuery (HKCategorySamplePredicates) Category

		[Static]
		[Export ("predicateForCategorySamplesWithOperatorType:value:")]
		NSPredicate GetPredicateForCategorySamples (NSPredicateOperatorType operatorType, nint value);

		[Static]
		[Export ("predicateForSamplesWithStartDate:endDate:options:")]
		NSPredicate GetPredicateForSamples ([NullAllowed] NSDate startDate, [NullAllowed] NSDate endDate, HKQueryOptions options);

		[Static]
		[Export ("predicateForObjectsWithNoCorrelation")]
		NSPredicate PredicateForObjectsWithNoCorrelation ();

		[Static]
		[Export ("predicateForObjectsFromWorkout:")]
		NSPredicate GetPredicateForObjectsFromWorkout (HKWorkout workout);

		[Static]
		[Export ("predicateForWorkoutsWithWorkoutActivityType:")]
		NSPredicate GetPredicateForWorkouts (HKWorkoutActivityType workoutActivityType);

		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:duration:")]
		NSPredicate GetPredicateForDuration (NSPredicateOperatorType operatorType, double duration);

		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:totalEnergyBurned:")]
		NSPredicate GetPredicateForTotalEnergyBurned (NSPredicateOperatorType operatorType, HKQuantity totalEnergyBurned);

		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:totalDistance:")]
		NSPredicate GetPredicateForTotalDistance (NSPredicateOperatorType operatorType, HKQuantity totalDistance);

		[iOS (10,0), Watch (3,0)]
		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:totalSwimmingStrokeCount:")]
		NSPredicate GetPredicateForTotalSwimmingStrokeCount (NSPredicateOperatorType operatorType, HKQuantity totalSwimmingStrokeCount);

		[Watch (4, 0), iOS (11, 0)]
		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:totalFlightsClimbed:")]
		NSPredicate GetPredicateForTotalFlightsClimbed (NSPredicateOperatorType operatorType, HKQuantity totalFlightsClimbed);

		// HKActivitySummaryPredicates

		[iOS (9,3), Watch (2,2)]
		[Static]
		[Export ("predicateForActivitySummaryWithDateComponents:")]
		NSPredicate GetPredicateForActivitySummary (NSDateComponents dateComponents);

		[iOS (9,3), Watch (2,2)]
		[Static]
		[Export ("predicateForActivitySummariesBetweenStartDateComponents:endDateComponents:")]
		NSPredicate GetPredicateForActivitySummariesBetween (NSDateComponents startDateComponents, NSDateComponents endDateComponents);


		// @interface HKClinicalRecordPredicates (HKQuery)
		[NoWatch, iOS (12,0)]
		[Static, Internal]
		[Export ("predicateForClinicalRecordsWithFHIRResourceType:")]
		NSPredicate GetPredicateForClinicalRecords (NSString resourceType);

		[NoWatch, iOS (12,0)]
		[Static]
		[Wrap ("GetPredicateForClinicalRecords (resourceType.GetConstant ())")]
		NSPredicate GetPredicateForClinicalRecords (HKFhirResourceType resourceType);

		[NoWatch, iOS (12,0)]
		[Static, Internal]
		[Export ("predicateForClinicalRecordsFromSource:FHIRResourceType:identifier:")]
		NSPredicate GetPredicateForClinicalRecords (HKSource source, string resourceType, string identifier);

		[NoWatch, iOS (12,0)]
		[Static]
		[Wrap ("GetPredicateForClinicalRecords (source, resourceType.GetConstant (), identifier)")]
		NSPredicate GetPredicateForClinicalRecords (HKSource source, HKFhirResourceType resourceType, string identifier);
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKObject))]
#if XAMCORE_4_0
	[Abstract]
#endif
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSample
	interface HKSample {

		[Export ("sampleType", ArgumentSemantic.Strong)]
		HKSampleType SampleType { get; }

		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; }

		// TODO: where is this thing used?
		[Field ("HKSampleSortIdentifierStartDate")]
		NSString SortIdentifierStartDate { get; }

		// TODO: where is this thing used?
		[Field ("HKSampleSortIdentifierEndDate")]
		NSString SortIdentifierEndDate { get; }

	}

	delegate void HKSampleQueryResultsHandler (HKSampleQuery query, HKSample [] results, NSError error);

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSampleQuery
	interface HKSampleQuery {

		[Export ("limit")]
		nuint Limit { get; }

		[Export ("sortDescriptors")]
		NSSortDescriptor[] SortDescriptors { get; }

		[Export ("initWithSampleType:predicate:limit:sortDescriptors:resultsHandler:")]
		IntPtr Constructor (HKSampleType sampleType, [NullAllowed] NSPredicate predicate, nuint limit, [NullAllowed] NSSortDescriptor[] sortDescriptors, HKSampleQueryResultsHandler resultsHandler);
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKSource : NSSecureCoding, NSCopying {
		[Export ("name")]
		string Name { get; }

		[Export ("bundleIdentifier")]
		string BundleIdentifier { get; }

		[Static]
		[Export ("defaultSource")]
		HKSource GetDefaultSource { get; }
	}

	delegate void HKSourceQueryCompletionHandler (HKSourceQuery query, NSSet sources, NSError error);

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSourceQuery
	interface HKSourceQuery {

		[Export ("initWithSampleType:samplePredicate:completionHandler:")]
		IntPtr Constructor (HKSampleType sampleType, [NullAllowed] NSPredicate objectPredicate, HKSourceQueryCompletionHandler completionHandler);
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKStatistics : NSSecureCoding, NSCopying {
		[Export ("quantityType", ArgumentSemantic.Strong)]
		HKQuantityType QuantityType { get; }

		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; }

		[Export ("sources")]
		HKSource [] Sources { get; }

		[Export ("averageQuantityForSource:")]
		HKQuantity AverageQuantity (HKSource source);

		[Export ("averageQuantity")]
		HKQuantity AverageQuantity ();

		[Export ("minimumQuantityForSource:")]
		HKQuantity MinimumQuantity (HKSource source);

		[Export ("minimumQuantity")]
		HKQuantity MinimumQuantity ();

		[Export ("maximumQuantityForSource:")]
		HKQuantity MaximumQuantity (HKSource source);

		[Export ("maximumQuantity")]
		HKQuantity MaximumQuantity ();

		[Export ("sumQuantityForSource:")]
		HKQuantity SumQuantity (HKSource source);

		[Export ("sumQuantity")]
		HKQuantity SumQuantity ();

		[Watch (5,0), iOS (12,0)]
		[Export ("mostRecentQuantityForSource:")]
		[return: NullAllowed]
		HKQuantity GetMostRecentQuantity (HKSource source);

		[Watch (5, 0), iOS (12, 0)]
		[NullAllowed, Export ("mostRecentQuantity")]
		HKQuantity MostRecentQuantity { get; }

		[Watch (5,0), iOS (12,0)]
		[Export ("mostRecentQuantityDateIntervalForSource:")]
		[return: NullAllowed]
		NSDateInterval GetMostRecentQuantityDateInterval (HKSource source);

		[Watch (5, 0), iOS (12, 0)]
		[NullAllowed, Export ("mostRecentQuantityDateInterval")]
		NSDateInterval MostRecentQuantityDateInterval { get; }
	}

	delegate void HKStatisticsCollectionEnumerator (HKStatistics result, bool stop);

	[Watch (2,0)]
	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKStatisticsCollection {

		[Export ("statisticsForDate:")]
		HKStatistics GetStatistics (NSDate date);

		[Export ("enumerateStatisticsFromDate:toDate:withBlock:")]
		void EnumerateStatistics (NSDate startDate, NSDate endDate, HKStatisticsCollectionEnumerator handler);

		[Export ("statistics")]
		HKStatistics[] Statistics { get; }

		[Export ("sources")]
		NSSet Sources { get; }
	}

	delegate void HKStatisticsCollectionQueryInitialResultsHandler (HKStatisticsCollectionQuery query, HKStatisticsCollection result, NSError error);
	delegate void HKStatisticsCollectionQueryStatisticsUpdateHandler (HKStatisticsCollectionQuery query, HKStatistics statistics, HKStatisticsCollection collection, NSError error);


	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKStatisticsCollectionQuery
	interface HKStatisticsCollectionQuery {

		[Export ("anchorDate", ArgumentSemantic.Strong)]
		NSDate AnchorDate { get; }

		[Export ("options")]
		HKStatisticsOptions Options { get; }

		[Export ("intervalComponents", ArgumentSemantic.Copy)]
		NSDateComponents IntervalComponents { get; }

		[NullAllowed, Export ("initialResultsHandler", ArgumentSemantic.Copy)]
		HKStatisticsCollectionQueryInitialResultsHandler InitialResultsHandler { get; set; }

		[NullAllowed, Export ("statisticsUpdateHandler", ArgumentSemantic.Copy)]
		HKStatisticsCollectionQueryStatisticsUpdateHandler StatisticsUpdated { get; set; }

		[Export ("initWithQuantityType:quantitySamplePredicate:options:anchorDate:intervalComponents:")]
		IntPtr Constructor (HKQuantityType quantityType, [NullAllowed] NSPredicate quantitySamplePredicate, HKStatisticsOptions options, NSDate anchorDate, NSDateComponents intervalComponents);
	}

	delegate void HKStatisticsQueryHandler (HKStatisticsQuery query, HKStatistics result, NSError error);

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKStatisticsQuery
	interface HKStatisticsQuery {

		[Export ("initWithQuantityType:quantitySamplePredicate:options:completionHandler:")]
		IntPtr Constructor (HKQuantityType quantityType, [NullAllowed] NSPredicate quantitySamplePredicate, HKStatisticsOptions options, HKStatisticsQueryHandler handler);
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[Static]
	interface HKQuantityTypeIdentifierKey {

		[Field ("HKQuantityTypeIdentifierBodyMassIndex")]
		NSString BodyMassIndex { get; }

		[Field ("HKQuantityTypeIdentifierBodyFatPercentage")]
		NSString BodyFatPercentage { get; }

		[Field ("HKQuantityTypeIdentifierHeight")]
		NSString Height { get; }

		[Field ("HKQuantityTypeIdentifierBodyMass")]
		NSString BodyMass { get; }

		[Field ("HKQuantityTypeIdentifierLeanBodyMass")]
		NSString LeanBodyMass { get; }

		[Field ("HKQuantityTypeIdentifierHeartRate")]
		NSString HeartRate { get; }

		[Field ("HKQuantityTypeIdentifierStepCount")]
		NSString StepCount { get; }

		[Field ("HKQuantityTypeIdentifierDistanceWalkingRunning")]
		NSString DistanceWalkingRunning { get; }

		[Field ("HKQuantityTypeIdentifierDistanceCycling")]
		NSString DistanceCycling { get; }

		[iOS (10,0), Watch (3,0)]
		[Field ("HKQuantityTypeIdentifierDistanceWheelchair")]
		NSString DistanceWheelchair { get; }

		[Field ("HKQuantityTypeIdentifierBasalEnergyBurned")]
		NSString BasalEnergyBurned { get; }

		[Field ("HKQuantityTypeIdentifierActiveEnergyBurned")]
		NSString ActiveEnergyBurned { get; }

		[Field ("HKQuantityTypeIdentifierFlightsClimbed")]
		NSString FlightsClimbed { get; }

		[Field ("HKQuantityTypeIdentifierNikeFuel")]
		NSString NikeFuel { get; }

		[iOS (9,3), Watch (2,2)]
		[Field ("HKQuantityTypeIdentifierAppleExerciseTime")]
		NSString AppleExerciseTime { get; }

		[iOS (10,0), Watch (3,0)]
		[Field ("HKQuantityTypeIdentifierPushCount")]
		NSString PushCount { get; }

		[iOS (10,0), Watch (3,0)]
		[Field ("HKQuantityTypeIdentifierDistanceSwimming")]
		NSString DistanceSwimming { get; }

		[iOS (10,0), Watch (3,0)]
		[Field ("HKQuantityTypeIdentifierSwimmingStrokeCount")]
		NSString SwimmingStrokeCount { get; }

		// Blood
		[Field ("HKQuantityTypeIdentifierOxygenSaturation")]
		NSString OxygenSaturation { get; }

		[Field ("HKQuantityTypeIdentifierBloodGlucose")]
		NSString BloodGlucose { get; }

		[Field ("HKQuantityTypeIdentifierBloodPressureSystolic")]
		NSString BloodPressureSystolic { get; }

		[Field ("HKQuantityTypeIdentifierBloodPressureDiastolic")]
		NSString BloodPressureDiastolic { get; }

		[Field ("HKQuantityTypeIdentifierBloodAlcoholContent")]
		NSString BloodAlcoholContent { get; }

		[Field ("HKQuantityTypeIdentifierForcedVitalCapacity")]
		NSString ForcedVitalCapacity { get; }

		[Field ("HKQuantityTypeIdentifierForcedExpiratoryVolume1")]
		NSString ForcedExpiratoryVolume1 { get; }

		[Field ("HKQuantityTypeIdentifierPeakExpiratoryFlowRate")]
		NSString PeakExpiratoryFlowRate { get; }

		[Field ("HKQuantityTypeIdentifierPeripheralPerfusionIndex")]
		NSString PeripheralPerfusionIndex { get; }

		// Miscellaneous
		[Field ("HKQuantityTypeIdentifierNumberOfTimesFallen")]
		NSString NumberOfTimesFallen { get; }

		[Field ("HKQuantityTypeIdentifierElectrodermalActivity")]
		NSString ElectrodermalActivity { get; }

		[Field ("HKQuantityTypeIdentifierInhalerUsage")]
		NSString InhalerUsage { get; }

		[Field ("HKQuantityTypeIdentifierRespiratoryRate")]
		NSString RespiratoryRate { get; }

		[iOS (11,0), Watch (4,0)]
		[Field ("HKQuantityTypeIdentifierInsulinDelivery")]
		NSString InsulinDelivery { get; }

		[iOS (11,0), Watch (4,0)]
		[Field ("HKQuantityTypeIdentifierRestingHeartRate")]
		NSString RestingHeartRate { get; }

		[iOS (11,0), Watch (4,0)]
		[Field ("HKQuantityTypeIdentifierWalkingHeartRateAverage")]
		NSString WalkingHeartRateAverage { get; }

		[iOS (11,0), Watch (4,0)]
		[Field ("HKQuantityTypeIdentifierHeartRateVariabilitySDNN")]
		NSString HeartRateVariabilitySdnn { get; }

		[Field ("HKQuantityTypeIdentifierBodyTemperature")]
		NSString BodyTemperature { get; }

		// Nutrition
		[Field ("HKQuantityTypeIdentifierDietaryFatTotal")]
		NSString DietaryFatTotal { get; }

		[Field ("HKQuantityTypeIdentifierDietaryFatPolyunsaturated")]
		NSString DietaryFatPolyunsaturated { get; }

		[Field ("HKQuantityTypeIdentifierDietaryFatMonounsaturated")]
		NSString DietaryFatMonounsaturated { get; }

		[Field ("HKQuantityTypeIdentifierDietaryFatSaturated")]
		NSString DietaryFatSaturated { get; }

		[Field ("HKQuantityTypeIdentifierDietaryCholesterol")]
		NSString DietaryCholesterol { get; }

		[Field ("HKQuantityTypeIdentifierDietarySodium")]
		NSString DietarySodium { get; }

		[Field ("HKQuantityTypeIdentifierDietaryCarbohydrates")]
		NSString DietaryCarbohydrates { get; }

		[Field ("HKQuantityTypeIdentifierDietaryFiber")]
		NSString DietaryFiber { get; }

		[Field ("HKQuantityTypeIdentifierDietarySugar")]
		NSString DietarySugar { get; }

		[Field ("HKQuantityTypeIdentifierDietaryEnergyConsumed")]
		NSString DietaryEnergyConsumed { get; }

		[Field ("HKQuantityTypeIdentifierDietaryProtein")]
		NSString DietaryProtein { get; }

		[Field ("HKQuantityTypeIdentifierDietaryVitaminA")]
		NSString DietaryVitaminA { get; }

		[Field ("HKQuantityTypeIdentifierDietaryVitaminB6")]
		NSString DietaryVitaminB6 { get; }

		[Field ("HKQuantityTypeIdentifierDietaryVitaminB12")]
		NSString DietaryVitaminB12 { get; }

		[Field ("HKQuantityTypeIdentifierDietaryVitaminC")]
		NSString DietaryVitaminC { get; }

		[Field ("HKQuantityTypeIdentifierDietaryVitaminD")]
		NSString DietaryVitaminD { get; }

		[Field ("HKQuantityTypeIdentifierDietaryVitaminE")]
		NSString DietaryVitaminE { get; }

		[Field ("HKQuantityTypeIdentifierDietaryVitaminK")]
		NSString DietaryVitaminK { get; }

		[Field ("HKQuantityTypeIdentifierDietaryCalcium")]
		NSString DietaryCalcium { get; }

		[Field ("HKQuantityTypeIdentifierDietaryIron")]
		NSString DietaryIron { get; }

		[Field ("HKQuantityTypeIdentifierDietaryThiamin")]
		NSString DietaryThiamin { get; }

		[Field ("HKQuantityTypeIdentifierDietaryRiboflavin")]
		NSString DietaryRiboflavin { get; }

		[Field ("HKQuantityTypeIdentifierDietaryNiacin")]
		NSString DietaryNiacin { get; }

		[Field ("HKQuantityTypeIdentifierDietaryFolate")]
		NSString DietaryFolate { get; }

		[Field ("HKQuantityTypeIdentifierDietaryBiotin")]
		NSString DietaryBiotin { get; }

		[Field ("HKQuantityTypeIdentifierDietaryPantothenicAcid")]
		NSString DietaryPantothenicAcid { get; }

		[Field ("HKQuantityTypeIdentifierDietaryPhosphorus")]
		NSString DietaryPhosphorus { get; }

		[Field ("HKQuantityTypeIdentifierDietaryIodine")]
		NSString DietaryIodine { get; }

		[Field ("HKQuantityTypeIdentifierDietaryMagnesium")]
		NSString DietaryMagnesium { get; }

		[Field ("HKQuantityTypeIdentifierDietaryZinc")]
		NSString DietaryZinc { get; }

		[Field ("HKQuantityTypeIdentifierDietarySelenium")]
		NSString DietarySelenium { get; }

		[Field ("HKQuantityTypeIdentifierDietaryCopper")]
		NSString DietaryCopper { get; }

		[Field ("HKQuantityTypeIdentifierDietaryManganese")]
		NSString DietaryManganese { get; }

		[Field ("HKQuantityTypeIdentifierDietaryChromium")]
		NSString DietaryChromium { get; }

		[Field ("HKQuantityTypeIdentifierDietaryMolybdenum")]
		NSString DietaryMolybdenum { get; }

		[Field ("HKQuantityTypeIdentifierDietaryChloride")]
		NSString DietaryChloride { get; }

		[Field ("HKQuantityTypeIdentifierDietaryPotassium")]
		NSString DietaryPotassium { get; }

		[Field ("HKQuantityTypeIdentifierDietaryCaffeine")]
		NSString DietaryCaffeine { get; }

		[iOS (9,0)]
		[Field ("HKQuantityTypeIdentifierBasalBodyTemperature")]
		NSString BasalBodyTemperature { get; }

		[iOS (9,0)]
		[Field ("HKQuantityTypeIdentifierDietaryWater")]
		NSString DietaryWater { get; }

		[iOS (9,0)]
		[Field ("HKQuantityTypeIdentifierUVExposure")]
		NSString UVExposure { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Field ("HKQuantityTypeIdentifierWaistCircumference")]
		NSString WaistCircumference { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Field ("HKQuantityTypeIdentifierVO2Max")]
		NSString VO2Max { get; }

		[Watch (4,2), iOS (11,2)]
		[Field ("HKQuantityTypeIdentifierDistanceDownhillSnowSports")]
		NSString DistanceDownhillSnowSports { get; }

        // If you add field, add them to the enum too.
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[Static]
	interface HKCorrelationTypeKey {
		[Field ("HKCorrelationTypeIdentifierBloodPressure")]
		NSString IdentifierBloodPressure { get; }
		
		[Field ("HKCorrelationTypeIdentifierFood")]
		NSString IdentifierFood { get; }

		// If you add fields, add them to the enum too.
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[Static]
	interface HKCategoryTypeIdentifierKey
	{
		/**** HKCategoryType Identifiers ****/

		[Field ("HKCategoryTypeIdentifierSleepAnalysis")]
		NSString SleepAnalysis { get; }

		[iOS (9,0)]
		[Field ("HKCategoryTypeIdentifierAppleStandHour")]
		NSString AppleStandHour { get; }

		[iOS (9,0)]
		[Field ("HKCategoryTypeIdentifierCervicalMucusQuality")]
		NSString CervicalMucusQuality { get; }

		[iOS (9,0)]
		[Field ("HKCategoryTypeIdentifierOvulationTestResult")]
		NSString OvulationTestResult { get; }

		[iOS (9,0)]
		[Field ("HKCategoryTypeIdentifierMenstrualFlow")]
		NSString MenstrualFlow { get; }

		[iOS (9,0)]
		[Field ("HKCategoryTypeIdentifierIntermenstrualBleeding")]
		NSString IntermenstrualBleeding { get; }

		[iOS (9,0)]
		[Field ("HKCategoryTypeIdentifierSexualActivity")]
		NSString SexualActivity { get; }

		[iOS (10,0), Watch (3,0)]
		[Field ("HKCategoryTypeIdentifierMindfulSession")]
		NSString MindfulSession { get; }

		// If you add fields, add them to the enum too.
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[Static]
	interface HKCharacteristicTypeIdentifierKey
	{
		/**** HKCharacteristicType Identifiers ****/

		[Field ("HKCharacteristicTypeIdentifierBiologicalSex")]
		NSString BiologicalSex { get; }

		[Field ("HKCharacteristicTypeIdentifierBloodType")]
		NSString BloodType { get; }

		[Field ("HKCharacteristicTypeIdentifierDateOfBirth")]
		NSString DateOfBirth { get; }

		[iOS (9,0)]
		[Field ("HKCharacteristicTypeIdentifierFitzpatrickSkinType")]
		NSString FitzpatrickSkinType { get; }

		[iOS (10,0), Watch (3,0)]
		[Field ("HKCharacteristicTypeIdentifierWheelchairUse")]
		NSString WheelchairUse { get; }

		// If you add fields, add them to the enum too.
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKUnit : NSCopying, NSSecureCoding {

		[Export ("unitString")]
		string UnitString { get; }

		[Static]
		[Export ("unitFromString:")]
		HKUnit FromString (string aString);


		[Static, Export ("unitFromMassFormatterUnit:")]
		HKUnit FromMassFormatterUnit (NSMassFormatterUnit massFormatterUnit);

		[Static, Export ("massFormatterUnitFromUnit:")]
		NSMassFormatterUnit GetMassFormatterUnit (HKUnit unit);

		[Static, Export ("unitFromLengthFormatterUnit:")]
		HKUnit FromLengthFormatterUnit (NSLengthFormatterUnit lengthFormatterUnit);

		[Static, Export ("lengthFormatterUnitFromUnit:")]
		NSLengthFormatterUnit GetLengthFormatterUnit (HKUnit unit);

		[Static, Export ("unitFromEnergyFormatterUnit:")]
		HKUnit FromEnergyFormatterUnit (NSEnergyFormatterUnit energyFormatterUnit);

		[Static, Export ("energyFormatterUnitFromUnit:")]
		NSEnergyFormatterUnit GetEnergyFormatterUnit (HKUnit unit);

		[Export ("isNull")]
		bool IsNull { get; }

		// HKUnit (Mass) Category

		[Static]
		[Export ("gramUnitWithMetricPrefix:")]
		HKUnit FromGramUnit (HKMetricPrefix prefix);

		[Static]
		[Export ("gramUnit")]
		HKUnit Gram { get; }

		[Static]
		[Export ("ounceUnit")]
		HKUnit Ounce { get; }

		[Static]
		[Export ("poundUnit")]
		HKUnit Pound { get; }

		[Static]
		[Export ("stoneUnit")]
		HKUnit Stone { get; }

		[Static]
		[Export ("moleUnitWithMetricPrefix:molarMass:")]
		HKUnit CreateMoleUnit (HKMetricPrefix prefix, double gramsPerMole);

		[Static]
		[Export ("moleUnitWithMolarMass:")]
		HKUnit CreateMoleUnit (double gramsPerMole);

		// HKUnit (Length) Category

		[Static]
		[Export ("meterUnitWithMetricPrefix:")]
		HKUnit CreateMeterUnit (HKMetricPrefix prefix);

		[Static]
		[Export ("meterUnit")]
		HKUnit Meter { get; }

		[Static]
		[Export ("inchUnit")]
		HKUnit Inch { get; }

		[Static]
		[Export ("footUnit")]
		HKUnit Foot { get; }

		[Static]
		[Export ("mileUnit")]
		HKUnit Mile { get; }

		[iOS (9,0)]
		[Static]
		[Export ("yardUnit")]
		HKUnit Yard { get; }

		// HKUnit (Volume) Category

		[Static]
		[Export ("literUnitWithMetricPrefix:")]
		HKUnit CreateLiterUnit (HKMetricPrefix prefix);

		[Static]
		[Export ("literUnit")]
		HKUnit Liter { get; }

		[Static]
		[Export ("fluidOunceUSUnit")]
		HKUnit FluidOunceUSUnit { get; }
		
		[Static]
		[Export ("fluidOunceImperialUnit")]
		HKUnit FluidOunceImperialUnit { get; }	

		[Static]
		[Export ("pintUSUnit")]
		HKUnit PintUSUnit { get; }

		[Static]
		[Export ("pintImperialUnit")]
		HKUnit PintImperialUnit { get; }

		[Static]
		[iOS (9,0)]
		[Export ("cupUSUnit")]
		HKUnit CupUSUnit { get; }

		[Static]
		[iOS (9,0)]
		[Export ("cupImperialUnit")]
		HKUnit CupImperialUnit { get; }

		// HKUnit (Pressure) Category

		[Static]
		[Export ("pascalUnitWithMetricPrefix:")]
		HKUnit CreatePascalUnit (HKMetricPrefix prefix);

		[Static]
		[Export ("pascalUnit")]
		HKUnit Pascal { get; }

		[Static]
		[Export ("millimeterOfMercuryUnit")]
		HKUnit MillimeterOfMercury { get; }

		[Static]
		[Export ("centimeterOfWaterUnit")]
		HKUnit CentimeterOfWater { get; }

		[Static]
		[Export ("atmosphereUnit")]
		HKUnit Atmosphere { get; }

		// HKUnit (Time) Category

		[Static]
		[Export ("secondUnitWithMetricPrefix:")]
		HKUnit CreateSecondUnit (HKMetricPrefix prefix);

		[Static]
		[Export ("secondUnit")]
		HKUnit Second { get; }

		[Static]
		[Export ("minuteUnit")]
		HKUnit Minute { get; }

		[Static]
		[Export ("hourUnit")]
		HKUnit Hour { get; }

		[Static]
		[Export ("dayUnit")]
		HKUnit Day { get; }

		// HKUnit (Energy) Category

		[Static]
		[Export ("jouleUnitWithMetricPrefix:")]
		HKUnit CreateJouleUnit (HKMetricPrefix prefix);

		[Static]
		[Export ("jouleUnit")]
		HKUnit Joule { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SmallCalorie' or 'LargeCalorie' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SmallCalorie' or 'LargeCalorie' instead.")]
		[Static]
		[Export ("calorieUnit")]
		HKUnit Calorie { get; }

		[Static]
		[Export ("kilocalorieUnit")]
		HKUnit Kilocalorie { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Static]
		[Export ("smallCalorieUnit")]
		HKUnit SmallCalorie { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Static]
		[Export ("largeCalorieUnit")]
		HKUnit LargeCalorie { get; }

		// HKUnit (Temperature) Category

		[Static]
		[Export ("degreeCelsiusUnit")]
		HKUnit DegreeCelsius { get; }

		[Static]
		[Export ("degreeFahrenheitUnit")]
		HKUnit DegreeFahrenheit { get; }

		[Static]
		[Export ("kelvinUnit")]
		HKUnit Kelvin { get; }

		// HKUnit(Conductance) Category

		[Static]
		[Export ("siemenUnitWithMetricPrefix:")]
		HKUnit CreateSiemenUnit (HKMetricPrefix prefix);

		[Static]
		[Export ("siemenUnit")]
		HKUnit Siemen { get; }

		// HKUnit (Scalar) Category

		[Static]
		[Export ("countUnit")]
		HKUnit Count { get; }

		[Static]
		[Export ("percentUnit")]
		HKUnit Percent { get; }

		// HKUnit (Math) Category

		[Export ("unitMultipliedByUnit:")]
		HKUnit UnitMultipliedBy (HKUnit unit);

		[Export ("unitDividedByUnit:")]
		HKUnit UnitDividedBy (HKUnit unit);

		[Export ("unitRaisedToPower:")]
		HKUnit UnitRaisedToPower (nint power);

		[Export ("reciprocalUnit")]
		HKUnit ReciprocalUnit ();

		// HKUnit (Pharmacology) Category
		[Watch (4, 0), iOS (11, 0)]
		[Static]
		[Export ("internationalUnit")]
		HKUnit InternationalUnit { get; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKWorkout
	interface HKWorkout {
		[Export ("workoutActivityType")]
		HKWorkoutActivityType WorkoutActivityType { get; }

		[Export ("workoutEvents")]
		HKWorkoutEvent [] WorkoutEvents { get; }

		[Export ("duration", ArgumentSemantic.UnsafeUnretained)]
		double Duration { get; }

		[Export ("totalEnergyBurned", ArgumentSemantic.Retain)]
		HKQuantity TotalEnergyBurned { get; }

		[Export ("totalDistance", ArgumentSemantic.Retain)]
		HKQuantity TotalDistance { get; }

		[Watch (3,0), iOS (10,0)]
		[NullAllowed, Export ("totalSwimmingStrokeCount", ArgumentSemantic.Strong)]
		HKQuantity TotalSwimmingStrokeCount { get; }

		[Static, Export ("workoutWithActivityType:startDate:endDate:")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate);

		[Static, Export ("workoutWithActivityType:startDate:endDate:workoutEvents:totalEnergyBurned:totalDistance:metadata:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, [NullAllowed] HKWorkoutEvent [] workoutEvents, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] NSDictionary metadata);

		[Static, Wrap ("Create (workoutActivityType, startDate, endDate, workoutEvents, totalEnergyBurned, totalDistance, metadata == null ? null : metadata.Dictionary)")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, HKWorkoutEvent [] workoutEvents, HKQuantity totalEnergyBurned, HKQuantity totalDistance, HKMetadata metadata);
		
		[Static, Export ("workoutWithActivityType:startDate:endDate:duration:totalEnergyBurned:totalDistance:metadata:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, double duration, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] NSDictionary metadata);

		[Static, Wrap ("Create (workoutActivityType, startDate, endDate, duration, totalEnergyBurned, totalDistance, metadata == null ? null : metadata.Dictionary)")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, double duration, HKQuantity totalEnergyBurned, HKQuantity totalDistance, HKMetadata metadata);

		[iOS (9,0)]
		[Static]
		[Export ("workoutWithActivityType:startDate:endDate:workoutEvents:totalEnergyBurned:totalDistance:device:metadata:")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, [NullAllowed] HKWorkoutEvent[] workoutEvents, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary metadata);

		[iOS (9,0)]
		[Static]
		[Wrap ("Create (workoutActivityType, startDate, endDate, workoutEvents, totalEnergyBurned, totalDistance, device, metadata == null ? null : metadata.Dictionary)")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, HKWorkoutEvent[] workoutEvents, HKQuantity totalEnergyBurned, HKQuantity totalDistance, HKDevice device, HKMetadata metadata);

		[iOS (9,0)]
		[Static]
		[Export ("workoutWithActivityType:startDate:endDate:duration:totalEnergyBurned:totalDistance:device:metadata:")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, double duration, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary metadata);

		[iOS (9,0)]
		[Static]
		[Wrap ("Create (workoutActivityType, startDate, endDate, duration, totalEnergyBurned, totalDistance, device, metadata == null ? null : metadata.Dictionary)")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, double duration, HKQuantity totalEnergyBurned, HKQuantity totalDistance, HKDevice device, HKMetadata metadata);

		[Watch (3,0), iOS (10,0)]
		[Static]
		[Export ("workoutWithActivityType:startDate:endDate:workoutEvents:totalEnergyBurned:totalDistance:totalSwimmingStrokeCount:device:metadata:")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, [NullAllowed] HKWorkoutEvent[] workoutEvents, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] HKQuantity totalSwimmingStrokeCount, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary metadata);

		[Watch (3,0), iOS (10,0)]
		[Static]
		[Wrap ("Create (workoutActivityType, startDate, endDate, workoutEvents, totalEnergyBurned, totalDistance, totalSwimmingStrokeCount, device, metadata == null ? null : metadata.Dictionary)")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, HKWorkoutEvent[] workoutEvents, HKQuantity totalEnergyBurned, HKQuantity totalDistance, HKQuantity totalSwimmingStrokeCount, HKDevice device, HKMetadata metadata);

		[Watch (4, 0), iOS (11, 0)]
		[Static]
		[Export ("workoutWithActivityType:startDate:endDate:workoutEvents:totalEnergyBurned:totalDistance:totalFlightsClimbed:device:metadata:")]
		HKWorkout CreateFlightsClimbedWorkout (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, [NullAllowed] HKWorkoutEvent [] workoutEvents, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] HKQuantity totalFlightsClimbed, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary metadata);

		[Watch (4, 0), iOS (11, 0)]
		[Static]
		[Wrap ("CreateFlightsClimbedWorkout (workoutActivityType, startDate, endDate, workoutEvents, totalEnergyBurned, totalDistance, totalFlightsClimbed, device, metadata == null ? null : metadata.Dictionary)")]
		HKWorkout CreateFlightsClimbedWorkout (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, [NullAllowed] HKWorkoutEvent [] workoutEvents, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] HKQuantity totalFlightsClimbed, [NullAllowed] HKDevice device, [NullAllowed] HKMetadata metadata);

		// TODO: where is this thing used?
		[Field ("HKWorkoutSortIdentifierDuration")]
		NSString SortIdentifierDuration { get; }

		// TODO: where is this thing used?
		[Field ("HKWorkoutSortIdentifierTotalDistance")]
		NSString SortIdentifierTotalDistance { get; }

		// TODO: where is this thing used?
		[Field ("HKWorkoutSortIdentifierTotalEnergyBurned")]
		NSString SortIdentifierTotalEnergyBurned { get; }

		[Watch (3,0), iOS (10,0)]
		[Field ("HKWorkoutSortIdentifierTotalSwimmingStrokeCount")]
		NSString SortIdentifierTotalSwimmingStrokeCount { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Field ("HKWorkoutSortIdentifierTotalFlightsClimbed")]
		NSString SortIdentifierTotalFlightsClimbed { get; }

		[Watch (4, 0), iOS (11, 0)]
		[NullAllowed, Export ("totalFlightsClimbed", ArgumentSemantic.Strong)]
		HKQuantity TotalFlightsClimbed { get; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKWorkoutEvent : NSSecureCoding, NSCopying {
		[Export ("type")]
		HKWorkoutEventType Type { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'DateInterval' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'DateInterval' instead.")]
		[Export ("date", ArgumentSemantic.Copy)]
		NSDate Date { get; }

		[Watch (3,0), iOS (10,0)]
		[NullAllowed, Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary WeakMetadata { get; }

		[Watch (3,0), iOS (10,0)]
		[Wrap ("WeakMetadata")]
		HKMetadata Metadata { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[Static, Export ("workoutEventWithType:date:")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDate date);

		[Watch (3,0), iOS (10,0)]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		[Export ("workoutEventWithType:date:metadata:")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDate date, NSDictionary metadata);

		[Watch (3,0), iOS (10,0)]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[Static]
		[Wrap ("Create (type, date, metadata != null ? metadata.Dictionary : null)")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDate date, HKMetadata metadata);

		[Watch (4, 0), iOS (11, 0)]
		[Static]
		[Export ("workoutEventWithType:dateInterval:metadata:")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDateInterval dateInterval, [NullAllowed] NSDictionary metadata);

		[Watch (4, 0), iOS (11, 0)]
		[Static]
		[Wrap ("Create (type, dateInterval, metadata != null ? metadata.Dictionary : null)")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDateInterval dateInterval, HKMetadata metadata);

		[Watch (4, 0), iOS (11, 0)]
		[Export ("dateInterval", ArgumentSemantic.Copy)]
		NSDateInterval DateInterval { get; }
	}

	[Watch (2,0)]
	[iOS (8,0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKWorkoutType
	interface HKWorkoutType {
		[Field ("HKWorkoutTypeIdentifier")]
		NSString Identifier { get; }
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKDeletedObject : NSSecureCoding {
		[Export ("UUID", ArgumentSemantic.Strong)]
		NSUuid Uuid { get; }

		[Watch (4, 0), iOS (11, 0)]
		[NullAllowed, Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary WeakMetadata { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Wrap ("WeakMetadata")]
		HKMetadata Metadata { get; }
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKDevice : NSSecureCoding, NSCopying {
		[NullAllowed]
		[Export ("name")]
		string Name { get; }

		[NullAllowed, Export ("manufacturer")]
		string Manufacturer { get; }

		[NullAllowed, Export ("model")]
		string Model { get; }

		[NullAllowed, Export ("hardwareVersion")]
		string HardwareVersion { get; }

		[NullAllowed, Export ("firmwareVersion")]
		string FirmwareVersion { get; }

		[NullAllowed, Export ("softwareVersion")]
		string SoftwareVersion { get; }

		[NullAllowed, Export ("localIdentifier")]
		string LocalIdentifier { get; }

		[NullAllowed, Export ("UDIDeviceIdentifier")]
		string UdiDeviceIdentifier { get; }

		[Export ("initWithName:manufacturer:model:hardwareVersion:firmwareVersion:softwareVersion:localIdentifier:UDIDeviceIdentifier:")]
		IntPtr Constructor ([NullAllowed] string name, [NullAllowed] string manufacturer, [NullAllowed] string model, [NullAllowed] string hardwareVersion, [NullAllowed] string firmwareVersion, [NullAllowed] string softwareVersion, [NullAllowed] string localIdentifier, [NullAllowed] string udiDeviceIdentifier);

		[Static]
		[Export ("localDevice")]
		HKDevice LocalDevice { get; }
	}

	[NoWatch, iOS (10,0)]
	[BaseType (typeof(HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKDocumentQuery
	interface HKDocumentQuery
	{
		[Export ("limit")]
		nuint Limit { get; }

		[NullAllowed, Export ("sortDescriptors", ArgumentSemantic.Copy)]
		NSSortDescriptor[] SortDescriptors { get; }

		[Export ("includeDocumentData")]
		bool IncludeDocumentData { get; }

		[Export ("initWithDocumentType:predicate:limit:sortDescriptors:includeDocumentData:resultsHandler:")]
		IntPtr Constructor (HKDocumentType documentType, [NullAllowed] NSPredicate predicate, nuint limit, [NullAllowed] NSSortDescriptor[] sortDescriptors, bool includeDocumentData, Action<HKDocumentQuery, HKDocumentSample [], bool, NSError> resultsHandler);
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[Static]
	interface HKDevicePropertyKey {
		[Field ("HKDevicePropertyKeyName")]
		NSString Name { get; }

		[Field ("HKDevicePropertyKeyManufacturer")]
		NSString Manufacturer { get; }

		[Field ("HKDevicePropertyKeyModel")]
		NSString Model { get; }

		[Field ("HKDevicePropertyKeyHardwareVersion")]
		NSString HardwareVersion { get; }

		[Field ("HKDevicePropertyKeyFirmwareVersion")]
		NSString FirmwareVersion { get; }

		[Field ("HKDevicePropertyKeySoftwareVersion")]
		NSString SoftwareVersion { get; }

		[Field ("HKDevicePropertyKeyLocalIdentifier")]
		NSString LocalIdentifier { get; }

		[Field ("HKDevicePropertyKeyUDIDeviceIdentifier")]
		NSString UdiDeviceIdentifier { get; }
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface HKFitzpatrickSkinTypeObject : NSCopying, NSSecureCoding {
		[Export ("skinType")]
		HKFitzpatrickSkinType SkinType { get; }
	}

	[Watch (3,0), iOS (10,0)]
	[BaseType (typeof(NSObject))]
	interface HKWheelchairUseObject : NSCopying, NSSecureCoding {
		[Export ("wheelchairUse")]
		HKWheelchairUse WheelchairUse { get; }
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKSourceRevision : NSSecureCoding, NSCopying {
		[Export ("source")]
		HKSource Source { get; }

		[NullAllowed, Export ("version")]
		string Version { get; }

		[Export ("initWithSource:version:")]
		IntPtr Constructor (HKSource source, [NullAllowed] string version);

		[Watch (4, 0), iOS (11, 0)]
		[NullAllowed, Export ("productType")]
		string ProductType { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Export ("operatingSystemVersion", ArgumentSemantic.Assign)]
		NSOperatingSystemVersion OperatingSystemVersion { get; }

		[Watch (4, 0), iOS (11, 0)]
		[Export ("initWithSource:version:productType:operatingSystemVersion:")]
		IntPtr Constructor (HKSource source, [NullAllowed] string version, [NullAllowed] string productType, NSOperatingSystemVersion operatingSystemVersion);
	}

	[Watch (4,0), iOS (11,0)]
	[Static]
	interface HKSourceRevisionInfo {

		[Field ("HKSourceRevisionAnyVersion")]
		NSString AnyVersion { get; }

		[Field ("HKSourceRevisionAnyProductType")]
		NSString AnyProductType { get; }

		// This key seems broken even in Objc returns a weird value
		//[Internal]
		//[Field ("HKSourceRevisionAnyOperatingSystem")]
		//IntPtr _AnyOperatingSystem { get; }

		//[Static]
		//[Wrap ("System.Runtime.InteropServices.Marshal.PtrToStructure<NSOperatingSystemVersion> (_AnyOperatingSystem)")]
		//NSOperatingSystemVersion AnyOperatingSystem { get; }
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKQueryAnchor : NSSecureCoding, NSCopying {
		[Static]
		[Export ("anchorFromValue:")]
		HKQueryAnchor Create (nuint value);
	}


	[NoiOS]
	[Watch (2,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface HKWorkoutSession : NSSecureCoding {
		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use WorkoutConfiguration")]
		[Export ("activityType")]
		HKWorkoutActivityType ActivityType { get; }

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use WorkoutConfiguration")]
		[Export ("locationType")]
		HKWorkoutSessionLocationType LocationType { get; }

		[Watch (3,0)]
		[Export ("workoutConfiguration", ArgumentSemantic.Copy)]
		HKWorkoutConfiguration WorkoutConfiguration { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		HKWorkoutSessionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("state")]
		HKWorkoutSessionState State { get; }

		[NullAllowed, Export ("startDate")]
		NSDate StartDate { get; }

		[NullAllowed, Export ("endDate")]
		NSDate EndDate { get; }

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use HKWorkoutSession (HKHealthStore, HKWorkoutConfiguration, out NSError) instead.")]
		[Export ("initWithActivityType:locationType:")]
		IntPtr Constructor (HKWorkoutActivityType activityType, HKWorkoutSessionLocationType locationType);

		[Watch (3,0)]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use HKWorkoutSession (HKHealthStore, HKWorkoutConfiguration, out NSError) instead.")]
		[Export ("initWithConfiguration:error:")]
		IntPtr Constructor (HKWorkoutConfiguration workoutConfiguration, out NSError error);

		[Watch (5,0)]
		[Export ("initWithHealthStore:configuration:error:")]
		IntPtr Constructor (HKHealthStore healthStore, HKWorkoutConfiguration workoutConfiguration, [NullAllowed] out NSError error);

		[Watch (5,0)]
		[Export ("prepare")]
		void Prepare ();

		[Watch (5,0)]
		[Export ("startActivityWithDate:")]
		void StartActivity ([NullAllowed] NSDate date);

		[Watch (5,0)]
		[Export ("stopActivityWithDate:")]
		void StopActivity ([NullAllowed] NSDate date);

		[Watch (5,0)]
		[Export ("end")]
		void End ();

		[Watch (5,0)]
		[Export ("pause")]
		void Pause ();

		[Watch (5,0)]
		[Export ("resume")]
		void Resume ();

		[Watch (5,0)]
		[Export ("associatedWorkoutBuilder")]
		HKLiveWorkoutBuilder AssociatedWorkoutBuilder { get; }
	}

	[NoiOS]
	[Watch (2,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface HKWorkoutSessionDelegate {
		[Abstract]
		[Export ("workoutSession:didChangeToState:fromState:date:")]
		void DidChangeToState (HKWorkoutSession workoutSession, HKWorkoutSessionState toState, HKWorkoutSessionState fromState, NSDate date);

		[Abstract]
		[Export ("workoutSession:didFailWithError:")]
		void DidFail (HKWorkoutSession workoutSession, NSError error);

		[Watch (3,0), iOS (10,0)]
		[Export ("workoutSession:didGenerateEvent:")]
		void DidGenerateEvent (HKWorkoutSession workoutSession, HKWorkoutEvent @event);
	}

	[iOS (9,3), Watch (2,2)]
	[BaseType (typeof (NSObject))]
	interface HKActivitySummary : NSSecureCoding, NSCopying {
		[Export ("dateComponentsForCalendar:")]
		NSDateComponents DateComponentsForCalendar (NSCalendar calendar);

		[Export ("activeEnergyBurned", ArgumentSemantic.Strong)]
		HKQuantity ActiveEnergyBurned { get; set; }

		[Export ("appleExerciseTime", ArgumentSemantic.Strong)]
		HKQuantity AppleExerciseTime { get; set; }

		[Export ("appleStandHours", ArgumentSemantic.Strong)]
		HKQuantity AppleStandHours { get; set; }

		[Export ("activeEnergyBurnedGoal", ArgumentSemantic.Strong)]
		HKQuantity ActiveEnergyBurnedGoal { get; set; }

		[Export ("appleExerciseTimeGoal", ArgumentSemantic.Strong)]
		HKQuantity AppleExerciseTimeGoal { get; set; }

		[Export ("appleStandHoursGoal", ArgumentSemantic.Strong)]
		HKQuantity AppleStandHoursGoal { get; set; }
	}

	[iOS (9,3), Watch (2,2)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKActivitySummaryQuery
	interface HKActivitySummaryQuery {
		[NullAllowed, Export ("updateHandler", ArgumentSemantic.Copy)]
		Action<HKActivitySummaryQuery, HKActivitySummary[], NSError> UpdateHandler { get; set; }

		[Export ("initWithPredicate:resultsHandler:")]
		IntPtr Constructor ([NullAllowed] NSPredicate predicate, Action<HKActivitySummaryQuery, HKActivitySummary[], NSError> handler);
	}

	[iOS (9,3), Watch (2,2)]
	[BaseType (typeof (HKObjectType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKActivitySummaryType
	interface HKActivitySummaryType {
	}

	[Watch (3,0)][iOS (10,0)]
	[BaseType (typeof (NSObject))]
	interface HKWorkoutConfiguration : NSCopying, NSSecureCoding {

		[Export ("activityType", ArgumentSemantic.Assign)]
		HKWorkoutActivityType ActivityType { get; set; }

		[Export ("locationType", ArgumentSemantic.Assign)]
		HKWorkoutSessionLocationType LocationType { get; set; }

		[Export ("swimmingLocationType", ArgumentSemantic.Assign)]
		HKWorkoutSwimmingLocationType SwimmingLocationType { get; set; }

		[NullAllowed, Export ("lapLength", ArgumentSemantic.Copy)]
		HKQuantity LapLength { get; set; }
	}

	[Watch (4, 0), iOS (11, 0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor]
	interface HKSeriesType {
		[Static]
		[Export ("workoutRouteType")]
		HKSeriesType WorkoutRouteType { get; }
	}

	[iOS (11,0)]
	[Watch (4,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKSeriesBuilder : NSSecureCoding {
		[Export ("discard")]
		void Discard ();
	}

	[iOS (11,0)]
	[Watch (4,0)]
	[BaseType (typeof(HKSample))]
	[DisableDefaultCtor]
	interface HKSeriesSample : NSCopying {
		[Export ("count")]
		nuint Count { get; }
	}

	[Watch (4, 0), iOS (11, 0)]
	[BaseType (typeof(HKSeriesSample))]
	[DisableDefaultCtor]
	interface HKWorkoutRoute : NSCopying {

		[Field ("HKWorkoutRouteTypeIdentifier")]
		NSString TypeIdentifier { get; }
	}

	delegate void HKWorkoutRouteBuilderAddMetadataHandler (bool success, NSError error);
	[Watch (4, 0), iOS (11, 0)]
	[BaseType (typeof (HKSeriesBuilder))]
	[DisableDefaultCtor]
	interface HKWorkoutRouteBuilder {
		[Export ("initWithHealthStore:device:")]
		IntPtr Constructor (HKHealthStore healthStore, [NullAllowed] HKDevice device);

		[Async, Export ("insertRouteData:completion:")]
		void InsertRouteData (CLLocation [] routeData, Action<bool, NSError> completion);

		[Async, Protected, Export ("finishRouteWithWorkout:metadata:completion:")]
		void FinishRoute (HKWorkout workout, [NullAllowed] NSDictionary metadata, Action<HKWorkoutRoute, NSError> completion);

		[Async, Wrap ("FinishRoute (workout, metadata != null ? metadata.Dictionary : null, completion)")]
		void FinishRoute (HKWorkout workout, HKMetadata metadata, Action<HKWorkoutRoute, NSError> completion);

		[Watch (5, 0), iOS (12, 0)]
		[Async, Protected]
		[Export ("addMetadata:completion:")]
		void AddMetadata (NSDictionary metadata, HKWorkoutRouteBuilderAddMetadataHandler completion);

		[Watch (5, 0), iOS (12, 0)]
		[Async, Wrap ("AddMetadata (metadata != null ? metadata.Dictionary : null, completion)")]
		void AddMetadata (HKMetadata metadata, HKWorkoutRouteBuilderAddMetadataHandler completion);
	}

	[Watch (4,0), iOS (11,0)]
	[BaseType (typeof(HKQuery))]
	interface HKWorkoutRouteQuery {
		[Export ("initWithRoute:dataHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (HKWorkoutRoute workoutRoute, HKWorkoutRouteBuilderDataHandler dataHandler);
	}

	delegate void HKWorkoutBuilderCompletionHandler (bool success, NSError error);
	[Watch (5,0), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface HKWorkoutBuilder
	{
		[NullAllowed, Export ("device", ArgumentSemantic.Copy)]
		HKDevice Device { get; }

		[NullAllowed, Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		[NullAllowed, Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; }

		[Export ("workoutConfiguration", ArgumentSemantic.Copy)]
		HKWorkoutConfiguration WorkoutConfiguration { get; }

		[Protected]
		[Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary NativeMetadata { get; }

		[Wrap ("NativeMetadata")]
		HKMetadata Metadata { get; }

		[Export ("workoutEvents", ArgumentSemantic.Copy)]
		HKWorkoutEvent[] WorkoutEvents { get; }

		[Export ("initWithHealthStore:configuration:device:")]
		IntPtr Constructor (HKHealthStore healthStore, HKWorkoutConfiguration configuration, [NullAllowed] HKDevice device);

		[Async]
		[Export ("beginCollectionWithStartDate:completion:")]
		void BeginCollection (NSDate startDate, HKWorkoutBuilderCompletionHandler completionHandler);

		[Async]
		[Export ("addSamples:completion:")]
		void Add (HKSample[] samples, HKWorkoutBuilderCompletionHandler completionHandler);

		[Async]
		[Export ("addWorkoutEvents:completion:")]
		void Add (HKWorkoutEvent[] workoutEvents, HKWorkoutBuilderCompletionHandler completionHandler);

		[Async, Protected]
		[Export ("addMetadata:completion:")]
		void Add (NSDictionary metadata, HKWorkoutBuilderCompletionHandler completionHandler);

		[Async]
		[Wrap ("Add (metadata.Dictionary, completionHandler)")]
		void Add (HKMetadata metadata, HKWorkoutBuilderCompletionHandler completionHandler);

		[Async]
		[Export ("endCollectionWithEndDate:completion:")]
		void EndCollection (NSDate endDate, HKWorkoutBuilderCompletionHandler completionHandler);

		[Async]
		[Export ("finishWorkoutWithCompletion:")]
		void FinishWorkout (HKWorkoutBuilderCompletionHandler completionHandler);

		[Export ("discardWorkout")]
		void DiscardWorkout ();

		[Export ("elapsedTimeAtDate:")]
		double GetElapsedTime (NSDate date);

		[Export ("statisticsForType:")]
		[return: NullAllowed]
		HKStatistics GetStatistics (HKQuantityType quantityType);

		[Export ("seriesBuilderForType:")]
		HKSeriesBuilder GetSeriesBuilder (HKSeriesType seriesType);
	}

	delegate void HKQuantitySeriesSampleQueryQuantityDelegate (HKQuantitySeriesSampleQuery query, HKQuantity quantity, NSDate date, bool done, NSError error);

	[Watch (5,0), iOS (12,0)]
	[BaseType (typeof(HKQuery))]
	interface HKQuantitySeriesSampleQuery
	{
		[Export ("initWithSample:quantityHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (HKQuantitySample quantitySample, HKQuantitySeriesSampleQueryQuantityDelegate quantityHandler);
	}

	delegate void HKQuantitySeriesSampleBuilderFinishSeriesDelegate (HKQuantitySample [] samples, NSError error);

	[Watch (5,0), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface HKQuantitySeriesSampleBuilder
	{
		[Export ("initWithHealthStore:quantityType:startDate:device:")]
		IntPtr Constructor (HKHealthStore healthStore, HKQuantityType quantityType, NSDate startDate, [NullAllowed] HKDevice device);

		[Export ("quantityType", ArgumentSemantic.Copy)]
		HKQuantityType QuantityType { get; }

		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		[NullAllowed, Export ("device", ArgumentSemantic.Copy)]
		HKDevice Device { get; }

		[Export ("insertQuantity:date:error:")]
		bool Insert (HKQuantity quantity, NSDate date, [NullAllowed] out NSError error);

		[Async, Protected]
		[Export ("finishSeriesWithMetadata:completion:")]
		void FinishSeries ([NullAllowed] NSDictionary metadata, HKQuantitySeriesSampleBuilderFinishSeriesDelegate completionHandler);

		[Async]
		[Wrap ("FinishSeries (metadata?.Dictionary, completionHandler)")]
		void FinishSeries ([NullAllowed] HKMetadata metadata, HKQuantitySeriesSampleBuilderFinishSeriesDelegate completionHandler);

		[Export ("discard")]
		void Discard ();
	}

	[Watch (5,0), NoiOS]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface HKLiveWorkoutDataSource
	{
		[Export ("typesToCollect", ArgumentSemantic.Copy)]
		NSSet<HKQuantityType> TypesToCollect { get; }

		[Export ("initWithHealthStore:workoutConfiguration:")]
		[DesignatedInitializer]
		IntPtr Constructor (HKHealthStore healthStore, [NullAllowed] HKWorkoutConfiguration configuration);

		[Export ("enableCollectionForType:predicate:")]
		void EnableCollection (HKQuantityType quantityType, [NullAllowed] NSPredicate predicate);

		[Export ("disableCollectionForType:")]
		void DisableCollection (HKQuantityType quantityType);
	}

	[NoWatch, iOS (12,0)]
	[BaseType (typeof (NSObject), Name = "HKFHIRResource")]
	[DisableDefaultCtor]
	interface HKFhirResource : NSSecureCoding, NSCopying
	{
		[Internal]
		[Export ("resourceType")]
		NSString _ResourceType { get; }

		HKFhirResourceType ResourceType { [Wrap ("HKFhirResourceTypeExtensions.GetValue (_ResourceType)")] get; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }

		[NullAllowed, Export ("sourceURL", ArgumentSemantic.Copy)]
		NSUrl SourceUrl { get; }
	}

	[Watch (5,0), iOS (12,0)]
	[DisableDefaultCtor]
	[BaseType (typeof(HKQuantitySample))]
	interface HKCumulativeQuantitySeriesSample
	{
		[Export ("sum", ArgumentSemantic.Copy)]
		HKQuantity Sum { get; }
	}

	[NoWatch, iOS (12,0)]
	[DisableDefaultCtor]
	[BaseType (typeof(HKSample))]
	interface HKClinicalRecord : NSSecureCoding, NSCopying
	{
		[Export ("clinicalType", ArgumentSemantic.Copy)]
		HKClinicalType ClinicalType { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[NullAllowed, Export ("FHIRResource", ArgumentSemantic.Copy)]
		HKFhirResource FhirResource { get; }
	}

	interface IHKLiveWorkoutBuilderDelegate {}
	[Watch (5,0), NoiOS]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface HKLiveWorkoutBuilderDelegate
	{
		[Abstract]
		[Export ("workoutBuilder:didCollectDataOfTypes:")]
		void DidCollectData (HKLiveWorkoutBuilder workoutBuilder, NSSet<HKSampleType> collectedTypes);

		[Abstract]
		[Export ("workoutBuilderDidCollectEvent:")]
		void DidCollectEvent (HKLiveWorkoutBuilder workoutBuilder);
	}

	[Watch (5,0), NoiOS]
	[DisableDefaultCtor]
	[BaseType (typeof(HKWorkoutBuilder))]
	interface HKLiveWorkoutBuilder
	{
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IHKLiveWorkoutBuilderDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("workoutSession", ArgumentSemantic.Weak)]
		HKWorkoutSession WorkoutSession { get; }

		[Export ("shouldCollectWorkoutEvents")]
		bool ShouldCollectWorkoutEvents { get; set; }

		[NullAllowed, Export ("dataSource", ArgumentSemantic.Strong)]
		HKLiveWorkoutDataSource DataSource { get; set; }

		[Export ("elapsedTime")]
		double ElapsedTime { get; }
	}
}
