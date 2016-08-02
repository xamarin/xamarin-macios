//
// This file describes the API that the generator will produce
//
// Authors:
//   Alex Soto alex.soto@xamarin.com
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2014-2015 Xamarin Inc.
//

using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using System;
using System.ComponentModel;

namespace XamCore.HealthKit {

	// NSInteger -> HKDefines.h
	[iOS (8,0)]
	[ErrorDomain ("HKErrorDomain")]
	[Native]
	public enum HKErrorCode : nint {
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
	public enum HKWorkoutSessionLocationType : nint {
		Unknown = 1,
		Indoor,
		Outdoor
	}

	[NoiOS]
	[Watch (2,0)]
	[Native]
	public enum HKWorkoutSessionState : nint {
		NotStarted = 1,
		Running,
		Ended
	}

	public delegate void HKAnchoredObjectResultHandler2 (HKAnchoredObjectQuery query, HKSample[] results, nuint newAnchor, NSError error);
#if XAMCORE_2_0
	[Obsolete ("Use HKAnchoredObjectResultHandler2 instead")]
	public delegate void HKAnchoredObjectResultHandler (HKAnchoredObjectQuery query, HKSampleType[] results, nuint newAnchor, NSError error);
#else
	public delegate void HKAnchoredObjectResultHandler (HKAnchoredObjectQuery query, NSObject[] results, nuint newAnchor, NSError error);
#endif

	public delegate void HKAnchoredObjectUpdateHandler (HKAnchoredObjectQuery query, HKSample[] addedObjects, HKDeletedObject[] deletedObjects, HKQueryAnchor newAnchor, NSError error);

	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException: The -init method is not available on HKAnchoredObjectQuery
	public interface HKAnchoredObjectQuery {

#if XAMCORE_2_0
		[Obsolete ("Use the overload that takes HKAnchoredObjectResultHandler2 instead")]
#endif
		[Availability (Introduced = Platform.iOS_8_0, Deprecated = Platform.iOS_9_0)]
		[Export ("initWithType:predicate:anchor:limit:completionHandler:")]
		IntPtr Constructor (HKSampleType type, [NullAllowed] NSPredicate predicate, nuint anchor, nuint limit, HKAnchoredObjectResultHandler completion);

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

	[iOS (8,0)]
	[Static]
	public interface HKPredicateKeyPath {
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

		[iOS (9,0)]
		[Field ("HKPredicateKeyPathDevice")]
		NSString Device { get; }

		[iOS (9,0)]
		[Field ("HKPredicateKeyPathSourceRevision")]
		NSString SourceRevision { get; }

		[iOS (9,3), Watch (2,2)]
		[Field ("HKPredicateKeyPathDateComponents")]
		NSString DateComponents { get; }
	}
	
	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (HKSample))]
	public interface HKCategorySample {
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

	[iOS (8,0)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor] // NSInvalidArgumentException: The -init method is not available on HKCorrelation
	public interface HKCorrelation : NSSecureCoding {

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

	public delegate void HKCorrelationQueryResultHandler (HKCorrelationQuery query, HKCorrelation[] correlations, NSError error);

	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCorrelationQuery
	public interface HKCorrelationQuery {
		[Export ("initWithType:predicate:samplePredicates:completion:")]
		IntPtr Constructor (HKCorrelationType correlationType, [NullAllowed] NSPredicate predicate, [NullAllowed] NSDictionary samplePredicates, HKCorrelationQueryResultHandler completion);

		[Export ("correlationType", ArgumentSemantic.Copy)]
		HKCorrelationType CorrelationType { get; }

		[Export ("samplePredicates", ArgumentSemantic.Copy)]
		NSDictionary SamplePredicates { get; }
	}

	[iOS (8,0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCorrelationType
	public interface HKCorrelationType {

	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	public interface HKHealthStore {
		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Static]
		[Export ("isHealthDataAvailable")]
		bool IsHealthDataAvailable { get; }

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

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("stopQuery:")]
		void StopQuery (HKQuery query);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("dateOfBirthWithError:")]
		NSDate GetDateOfBirth (out NSError error);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("biologicalSexWithError:")]
		HKBiologicalSexObject GetBiologicalSex (out NSError error);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("bloodTypeWithError:")]
		HKBloodTypeObject GetBloodType (out NSError error);

		[NoWatch]
		[Export ("enableBackgroundDeliveryForType:frequency:withCompletion:")]
		void EnableBackgroundDelivery (HKObjectType type, HKUpdateFrequency frequency, Action<bool, NSError> completion);

		[NoWatch]
		[Export ("disableBackgroundDeliveryForType:withCompletion:")]
		void DisableBackgroundDelivery (HKObjectType type, Action<bool, NSError> completion);

		[NoWatch]
		[Export ("disableAllBackgroundDeliveryWithCompletion:")]
		void DisableAllBackgroundDelivery (Action<bool, NSError> completion);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[NoWatch]
		[iOS (9,0)]
		[Export ("handleAuthorizationForExtensionWithCompletion:")]
		void HandleAuthorizationForExtension (Action<bool, NSError> completion);

		[iOS (9,0)]
		[Export ("splitTotalEnergy:startDate:endDate:resultsHandler:")]
		void SplitTotalEnergy (HKQuantity totalEnergy, NSDate startDate, NSDate endDate, Action<HKQuantity, HKQuantity, NSError> resultsHandler);

		// HKHealthStore category

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

		// HKUserPreferences category

		[iOS (8,2)]
		[Async]
		[Export ("preferredUnitsForQuantityTypes:completion:")]
		void GetPreferredUnits (NSSet quantityTypes, Action<NSDictionary, NSError> completion);

		[iOS (8,2)]
		[Notification]
		[Field ("HKUserPreferencesDidChangeNotification")]
		NSString UserPreferencesDidChangeNotification { get; }
	}

	public delegate void HKStoreSampleAddedCallback (bool success, NSError error);
	
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	public interface HKBiologicalSexObject : NSCopying, NSSecureCoding {
		[Export ("biologicalSex")]
		HKBiologicalSex BiologicalSex { get; }
	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	public interface HKBloodTypeObject : NSCopying, NSSecureCoding {
		[Export ("bloodType")]
		HKBloodType BloodType { get; }
	}

	[StrongDictionary ("HKMetadataKey")]
	public interface HKMetadata {
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
		bool WasTakenInLab { get; set; 
}
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
	}
		
	[iOS (8,0)]
	[Static]
	public interface HKMetadataKey {
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
	}

	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	public interface HKObject : NSSecureCoding {
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

	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	public interface HKObjectType : NSSecureCoding, NSCopying {
		// These identifiers come from HKTypeIdentifiers
		[Export ("identifier")]
		NSString Identifier { get; }

		// TODO: introduce an enum? Allows HKTypeIdentifiers's 
		[Static]
		[Export ("quantityTypeForIdentifier:")]
		HKQuantityType GetQuantityType (NSString hkTypeIdentifier);

		// TODO: introduce an enum?  Allows Hkcategorytypeidentifier's
		[Static]
		[Export ("categoryTypeForIdentifier:")]
		HKCategoryType GetCategoryType (NSString hkCategoryTypeIdentifier);

		// TODO: introduce an enum?  Allows hkCharacteristicTypeIdentifier cosntats
		[Static]
		[Export ("characteristicTypeForIdentifier:")]
		HKCharacteristicType GetCharacteristicType (NSString hkCharacteristicTypeIdentifier);

		[Static, Export ("correlationTypeForIdentifier:")]
		HKCorrelationType GetCorrelationType (NSString hkCorrelationTypeIdentifier);

		[Static, Export ("workoutType")]
		HKWorkoutType GetWorkoutType ();

		[Watch (2,2)]
		[iOS (9,3)]
		[Static]
		[Export ("activitySummaryType")]
		HKActivitySummaryType ActivitySummaryType { get; }
	}

	[iOS (8,0)]
	[BaseType (typeof (HKObjectType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCharacteristicType
	public interface HKCharacteristicType {

	}

	[iOS (8,0)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSampleType
	[BaseType (typeof (HKObjectType))]
#if XAMCORE_2_0
	[Abstract] // The HKSampleType class is an abstract subclass of the HKObjectType class, used to represent data samples. Never instantiate an HKSampleType object directly. Instead, you should always work with one of its concrete subclasses [...]
#endif
	public interface HKSampleType {

	}

	[iOS (8,0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCategoryType
	public interface HKCategoryType {

	}

	[iOS (8,0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKQuantityType
	public interface HKQuantityType {
		[Export ("aggregationStyle")]
		HKQuantityAggregationStyle AggregationStyle { get; }

		[Export ("isCompatibleWithUnit:")]
		bool IsCompatible (HKUnit unit);
	}

#if XAMCORE_2_0
	public delegate void HKObserverQueryUpdateHandler (HKObserverQuery query, [BlockCallback] Action completion, NSError error);
#else
	public delegate void HKObserverQueryCompletionHandler ();
	public delegate void HKObserverQueryUpdateHandler (HKObserverQuery query, [BlockCallback] HKObserverQueryCompletionHandler completion, NSError error);
#endif

	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKObserverQuery
	public interface HKObserverQuery {
		[Export ("initWithSampleType:predicate:updateHandler:")]
		IntPtr Constructor (HKSampleType sampleType, [NullAllowed] NSPredicate predicate, HKObserverQueryUpdateHandler updateHandler);
	}

	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	public interface HKQuantity : NSSecureCoding, NSCopying {
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

	[iOS (8,0)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKQuantitySample
	public interface HKQuantitySample {
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
	}

	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	public interface HKQuery {
		[iOS (9,3), Watch (2,2)]
		[NullAllowed, Export ("objectType", ArgumentSemantic.Strong)]
		HKObjectType ObjectType { get; }

		[Deprecated (PlatformName.WatchOS, 2,2, message: "Use ObjectType property")]
		[Deprecated (PlatformName.iOS, 9,3, message: "Use ObjectType property")]
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

		// HKActivitySummaryPredicates

		[iOS (9,3), Watch (2,2)]
		[Static]
		[Export ("predicateForActivitySummaryWithDateComponents:")]
		NSPredicate GetPredicateForActivitySummary (NSDateComponents dateComponents);

		[iOS (9,3), Watch (2,2)]
		[Static]
		[Export ("predicateForActivitySummariesBetweenStartDateComponents:endDateComponents:")]
		NSPredicate GetPredicateForActivitySummariesBetween (NSDateComponents startDateComponents, NSDateComponents endDateComponents);
	}

	[iOS (8,0)]
	[BaseType (typeof (HKObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSample
	public interface HKSample {

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

	public delegate void HKSampleQueryResultsHandler (HKSampleQuery query, HKSample [] results, NSError error);

	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSampleQuery
	public interface HKSampleQuery {

		[Export ("limit")]
		nuint Limit { get; }

		[Export ("sortDescriptors")]
		NSSortDescriptor[] SortDescriptors { get; }

		[Export ("initWithSampleType:predicate:limit:sortDescriptors:resultsHandler:")]
		IntPtr Constructor (HKSampleType sampleType, [NullAllowed] NSPredicate predicate, nuint limit, [NullAllowed] NSSortDescriptor[] sortDescriptors, HKSampleQueryResultsHandler resultsHandler);
	}

	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	public interface HKSource : NSSecureCoding, NSCopying {
		[Export ("name")]
		string Name { get; }

		[Export ("bundleIdentifier")]
		string BundleIdentifier { get; }

		[Static]
		[Export ("defaultSource")]
		HKSource GetDefaultSource { get; }
	}

	public delegate void HKSourceQueryCompletionHandler (HKSourceQuery query, NSSet sources, NSError error);

	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSourceQuery
	public interface HKSourceQuery {

		[Export ("initWithSampleType:samplePredicate:completionHandler:")]
		IntPtr Constructor (HKSampleType sampleType, [NullAllowed] NSPredicate objectPredicate, HKSourceQueryCompletionHandler completionHandler);
	}

	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	public interface HKStatistics : NSSecureCoding, NSCopying {
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
	}

	public delegate void HKStatisticsCollectionEnumerator (HKStatistics result, bool stop);

	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	public interface HKStatisticsCollection {

		[Export ("statisticsForDate:")]
		HKStatistics GetStatistics (NSDate date);

		[Export ("enumerateStatisticsFromDate:toDate:withBlock:")]
		void EnumerateStatistics (NSDate startDate, NSDate endDate, HKStatisticsCollectionEnumerator handler);

		[Export ("statistics")]
		HKStatistics[] Statistics { get; }

		[Export ("sources")]
		NSSet Sources { get; }
	}

	public delegate void HKStatisticsCollectionQueryInitialResultsHandler (HKStatisticsCollectionQuery query, HKStatisticsCollection result, NSError error);
	public delegate void HKStatisticsCollectionQueryStatisticsUpdateHandler (HKStatisticsCollectionQuery query, HKStatistics statistics, HKStatisticsCollection collection, NSError error);


	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKStatisticsCollectionQuery
	public interface HKStatisticsCollectionQuery {

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

	public delegate void HKStatisticsQueryHandler (HKStatisticsQuery query, HKStatistics result, NSError error);

	[iOS (8,0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKStatisticsQuery
	public interface HKStatisticsQuery {

		[Export ("initWithQuantityType:quantitySamplePredicate:options:completionHandler:")]
		IntPtr Constructor (HKQuantityType quantityType, [NullAllowed] NSPredicate quantitySamplePredicate, HKStatisticsOptions options, HKStatisticsQueryHandler handler);
	}

	[iOS (8,0)]
	[Static]
	public interface HKQuantityTypeIdentifierKey {

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

		// If you add field, add them to the enum too.
	}

	[iOS (8,0)]
	[Static]
	public interface HKCorrelationTypeKey {
		[Field ("HKCorrelationTypeIdentifierBloodPressure")]
		NSString IdentifierBloodPressure { get; }
		
		[Field ("HKCorrelationTypeIdentifierFood")]
		NSString IdentifierFood { get; }
	}
	

	[iOS (8,0)]
	[Static]
	public interface HKCategoryTypeIdentifierKey
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
	}

	[iOS (8,0)]
	[Static]
	public interface HKCharacteristicTypeIdentifierKey
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
	}

	[iOS (8,0)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	public interface HKUnit : NSCopying, NSSecureCoding {

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

		[Static]
		[Export ("calorieUnit")]
		HKUnit Calorie { get; }

		[Static]
		[Export ("kilocalorieUnit")]
		HKUnit Kilocalorie { get; }

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
	}

	[iOS (8,0)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKWorkout
	public interface HKWorkout {
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

		// TODO: where is this thing used?
		[Field ("HKWorkoutSortIdentifierDuration")]
		NSString SortIdentifierDuration { get; }

		// TODO: where is this thing used?
		[Field ("HKWorkoutSortIdentifierTotalDistance")]
		NSString SortIdentifierTotalDistance { get; }

		// TODO: where is this thing used?
		[Field ("HKWorkoutSortIdentifierTotalEnergyBurned")]
		NSString SortIdentifierTotalEnergyBurned { get; }
	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface HKWorkoutEvent : NSSecureCoding, NSCopying {
		[Export ("type")]
		HKWorkoutEventType Type { get; }

		[Export ("date", ArgumentSemantic.Copy)]
		NSDate Date { get; }

		[Static, Export ("workoutEventWithType:date:")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDate date);
	}

	[iOS (8,0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKWorkoutType
	public interface HKWorkoutType {
		[Field ("HKWorkoutTypeIdentifier")]
		NSString Identifier { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface HKDeletedObject : NSSecureCoding {
		[Export ("UUID", ArgumentSemantic.Strong)]
		NSUuid Uuid { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface HKDevice : NSSecureCoding, NSCopying {
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

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	public interface HKFitzpatrickSkinTypeObject : NSCopying, NSSecureCoding {
		[Export ("skinType")]
		HKFitzpatrickSkinType SkinType { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface HKSourceRevision : NSSecureCoding, NSCopying {
		[Export ("source")]
		HKSource Source { get; }

		[NullAllowed, Export ("version")]
		string Version { get; }

		[Export ("initWithSource:version:")]
		IntPtr Constructor (HKSource source, string version);
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface HKQueryAnchor : NSSecureCoding, NSCopying {
		[Static]
		[Export ("anchorFromValue:")]
		HKQueryAnchor Create (nuint value);
	}


	[NoiOS]
	[Watch (2,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	public interface HKWorkoutSession : NSSecureCoding {
		[Export ("activityType")]
		HKWorkoutActivityType ActivityType { get; }

		[Export ("locationType")]
		HKWorkoutSessionLocationType LocationType { get; }

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

		[Export ("initWithActivityType:locationType:")]
		IntPtr Constructor (HKWorkoutActivityType activityType, HKWorkoutSessionLocationType locationType);
	}

	[NoiOS]
	[Watch (2,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	public interface HKWorkoutSessionDelegate {
		[Abstract]
		[Export ("workoutSession:didChangeToState:fromState:date:")]
		void DidChangeToState (HKWorkoutSession workoutSession, HKWorkoutSessionState toState, HKWorkoutSessionState fromState, NSDate date);

		[Abstract]
		[Export ("workoutSession:didFailWithError:")]
		void DidFail (HKWorkoutSession workoutSession, NSError error);
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
	public interface HKActivitySummaryType {
	}

	[Watch (3,0)][iOS (10,0)]
	[BaseType (typeof (NSObject))]
	interface HKWorkoutConfiguration : NSCopying, NSSecureCoding {

		[Export ("activityType", ArgumentSemantic.Assign)]
		HKWorkoutActivityType ActivityType { get; set; }

		[Export ("locationType", ArgumentSemantic.Assign)]
		HKWorkoutSessionLocationType LocationType { get; set; }
	}
}
