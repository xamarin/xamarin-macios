//
// This file describes the API that the generator will produce
//
// Authors:
//   Alex Soto alex.soto@xamarin.com
//   Miguel de Icaza (miguel@xamarin.com)
//   Whitney Schmidt (whschm@microsoft.com)
//
// Copyright 2014-2015 Xamarin Inc.
// Copyright 2019, 2020 Microsoft Corporation. All rights reserved.
//

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using System;
using System.ComponentModel;
using CoreLocation;
using UniformTypeIdentifiers;
#if MONOMAC
using AppKit;
using UIViewController = AppKit.NSViewController;
#else
using UIKit;
using NSViewController = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace HealthKit {

	/// <summary>Enumerates HealthKit document types.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	public enum HKDocumentTypeIdentifier {
		[Field ("HKDocumentTypeIdentifierCDA")]
		Cda,
	}

	// NSInteger -> HKDefines.h
	/// <summary>Enumerates common errors made when accessing health data.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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
		RequiredAuthorizationDenied,
		NoData,
		WorkoutActivityNotAllowed,
		DataSizeExceeded,
		BackgroundWorkoutSessionNotAllowed,
		NotPermissibleForGuestUserMode,
	}

	/// <summary>Enumerates workout locations.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKWorkoutSessionLocationType : long {
		Unknown = 1,
		Indoor,
		Outdoor
	}

	[iOS (17, 0)]
	[Mac (13, 0)]
	[MacCatalyst (17, 0)]
	[Native]
	public enum HKWorkoutSessionState : long {
		NotStarted = 1,
		Running,
		Ended,
		[NoMacCatalyst]
		Paused,
		[NoMacCatalyst]
		Prepared,
		[NoMacCatalyst]
		Stopped,
	}

	/// <summary>Enumerates the possible activity-levels associated with a heart-rate sample.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKHeartRateMotionContext : long {
		NotSet = 0,
		Sedentary,
		Active,
	}

	[iOS (14, 0), Mac (13, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum HKActivityMoveMode : long {
		ActiveEnergy = 1,
		AppleMoveTime = 2,
	}

	[iOS (14, 2), Mac (13, 0)]
	[MacCatalyst (14, 2)]
	[Native]
	public enum HKCategoryValueHeadphoneAudioExposureEvent : long {
		SevenDayLimit = 1,
	}

	[iOS (15, 0), Mac (13, 0)]
	[MacCatalyst (15, 0)]
	[Native]
	public enum HKAppleWalkingSteadinessClassification : long {
		Ok = 1,
		Low,
		VeryLow,
	}

	[iOS (15, 0), Mac (13, 0)]
	[MacCatalyst (15, 0)]
	[Native]
	public enum HKCategoryValueAppleWalkingSteadinessEvent : long {
		InitialLow = 1,
		InitialVeryLow = 2,
		RepeatLow = 3,
		RepeatVeryLow = 4,
	}

	[iOS (15, 0), Mac (13, 0)]
	[MacCatalyst (15, 0)]
	[Native]
	public enum HKCategoryValuePregnancyTestResult : long {
		Negative = 1,
		Positive,
		Indeterminate,
	}

	[iOS (15, 0), Mac (13, 0)]
	[MacCatalyst (15, 0)]
	[Native]
	public enum HKCategoryValueProgesteroneTestResult : long {
		Negative = 1,
		Positive,
		Indeterminate,
	}

	[iOS (15, 4), MacCatalyst (15, 4), Mac (13, 0)]
	public enum HKVerifiableClinicalRecordSourceType {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("HKVerifiableClinicalRecordSourceTypeSMARTHealthCard")]
		SmartHealthCard,

		[Field ("HKVerifiableClinicalRecordSourceTypeEUDigitalCOVIDCertificate")]
		EuDigitalCovidCertificate,
	}

	[iOS (15, 4), MacCatalyst (15, 4), Mac (13, 0)]
	public enum HKVerifiableClinicalRecordCredentialType {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("HKVerifiableClinicalRecordCredentialTypeCOVID19")]
		Covid19,

		[Field ("HKVerifiableClinicalRecordCredentialTypeImmunization")]
		Immunization,

		[Field ("HKVerifiableClinicalRecordCredentialTypeLaboratory")]
		Laboratory,

		[Field ("HKVerifiableClinicalRecordCredentialTypeRecovery")]
		Recovery,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	[NativeName ("HKGAD7AssessmentRisk")]
	public enum HKGad7AssessmentRisk : long {
		NoneToMinimal = 1,
		Mild,
		Moderate,
		Severe,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	[NativeName ("HKGAD7AssessmentAnswer")]
	public enum HKGad7AssessmentAnswer : long {
		NotAtAll = 0,
		SeveralDays,
		MoreThanHalfTheDays,
		NearlyEveryDay,
	}

	[MacCatalyst (18, 1), Mac (15, 1), iOS (18, 1)]
	[Native]
	public enum HKAudiogramConductionType : long {
		Air = 0,
	}

	[MacCatalyst (18, 1), Mac (15, 1), iOS (18, 1)]
	[Native]
	public enum HKAudiogramSensitivityTestSide : long {
		Left = 0,
		Right = 1,
	}

#if NET
	/// <summary>The completion handler for <see cref="M:HealthKit.HKAnchoredObjectQuery.#ctor(HealthKit.HKSampleType,Foundation.NSPredicate,System.nuint,System.nuint,HealthKit.HKAnchoredObjectResultHandler2)" />.</summary>
	/// <summary>Completion handler for anchored object queries.</summary>
	delegate void HKAnchoredObjectResultHandler (HKAnchoredObjectQuery query, HKSample [] results, nuint newAnchor, NSError error);
#else
	delegate void HKAnchoredObjectResultHandler2 (HKAnchoredObjectQuery query, HKSample [] results, nuint newAnchor, NSError error);

	[Obsolete ("Use HKAnchoredObjectResultHandler2 instead")]
	delegate void HKAnchoredObjectResultHandler (HKAnchoredObjectQuery query, HKSampleType [] results, nuint newAnchor, NSError error);
#endif

	delegate void HKAnchoredObjectUpdateHandler (HKAnchoredObjectQuery query, HKSample [] addedObjects, HKDeletedObject [] deletedObjects, HKQueryAnchor newAnchor, NSError error);

	delegate void HKWorkoutRouteBuilderDataHandler (HKWorkoutRouteQuery query, CLLocation [] routeData, bool done, NSError error);

	/// <summary>An <see cref="T:HealthKit.HKQuery" /> that on its initial call returns the most recent result and in subsequent calls returns only data added after the initial call.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKAnchoredObjectQuery_Class/index.html">Apple documentation for <c>HKAnchoredObjectQuery</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException: The -init method is not available on HKAnchoredObjectQuery
	interface HKAnchoredObjectQuery {

#if !NET
		[Obsolete ("Use the overload that takes HKAnchoredObjectResultHandler2 instead")]
#endif
		[Deprecated (PlatformName.iOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("initWithType:predicate:anchor:limit:completionHandler:")]
		NativeHandle Constructor (HKSampleType type, [NullAllowed] NSPredicate predicate, nuint anchor, nuint limit, HKAnchoredObjectResultHandler completion);

#if !NET
		[Sealed]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Export ("initWithType:predicate:anchor:limit:completionHandler:")]
		NativeHandle Constructor (HKSampleType type, [NullAllowed] NSPredicate predicate, nuint anchor, nuint limit, HKAnchoredObjectResultHandler2 completion);
#endif

		[MacCatalyst (13, 1)]
		[Export ("initWithType:predicate:anchor:limit:resultsHandler:")]
		NativeHandle Constructor (HKSampleType type, [NullAllowed] NSPredicate predicate, [NullAllowed] HKQueryAnchor anchor, nuint limit, HKAnchoredObjectUpdateHandler handler);

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Export ("initWithQueryDescriptors:anchor:limit:resultsHandler:")]
		NativeHandle Constructor (HKQueryDescriptor [] queryDescriptors, [NullAllowed] HKQueryAnchor anchor, nint limit, HKAnchoredObjectUpdateHandler resultsHandler);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("updateHandler", ArgumentSemantic.Copy)]
		HKAnchoredObjectUpdateHandler UpdateHandler { get; set; }
	}

	/// <summary>Contains constants that represent keys that identify predicate key paths for selecting HealthKit values.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for the desired distance type.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for the desired distance type.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for the desired distance type.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for the desired distance type.")]
		[Field ("HKPredicateKeyPathWorkoutTotalDistance")]
		NSString WorkoutTotalDistance { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.ActiveEnergyBurned.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.ActiveEnergyBurned.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.ActiveEnergyBurned.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.ActiveEnergyBurned.")]
		[Field ("HKPredicateKeyPathWorkoutTotalEnergyBurned")]
		NSString WorkoutTotalEnergyBurned { get; }

		[Field ("HKPredicateKeyPathWorkoutType")]
		NSString WorkoutType { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.SwimmingStrokeCount.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.SwimmingStrokeCount.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.SwimmingStrokeCount.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.SwimmingStrokeCount.")]
		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathWorkoutTotalSwimmingStrokeCount")]
		NSString WorkoutTotalSwimmingStrokeCount { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathDevice")]
		NSString Device { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathSourceRevision")]
		NSString SourceRevision { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathDateComponents")]
		NSString DateComponents { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathCDATitle")]
		NSString CdaTitle { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathCDAPatientName")]
		NSString CdaPatientName { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathCDAAuthorName")]
		NSString CdaAuthorName { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathCDACustodianName")]
		NSString CdaCustodianName { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.FlightsClimbed.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.FlightsClimbed.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.FlightsClimbed.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'HKQuery.GetSumQuantityPredicateForWorkoutActivities' instead, passing the HKQuantityType for HKQuantityTypeIdentifier.FlightsClimbed.")]
		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathWorkoutTotalFlightsClimbed")]
		NSString TotalFlightsClimbed { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathSum")]
		NSString PathSum { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathClinicalRecordFHIRResourceIdentifier")]
		NSString ClinicalRecordFhirResourceIdentifier { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathClinicalRecordFHIRResourceType")]
		NSString ClinicalRecordFhirResourceType { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathMin")]
		NSString Min { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathAverage")]
		NSString Average { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathMax")]
		NSString Max { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathMostRecent")]
		NSString MostRecent { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathMostRecentStartDate")]
		NSString MostRecentStartDate { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathMostRecentEndDate")]
		NSString MostRecentEndDate { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathMostRecentDuration")]
		NSString MostRecentDuration { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKPredicateKeyPathCount")]
		NSString PathCount { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKPredicateKeyPathAverageHeartRate")]
		NSString AverageHeartRate { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKPredicateKeyPathECGClassification")]
		NSString EcgClassification { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKPredicateKeyPathECGSymptomsStatus")]
		NSString EcgSymptomsStatus { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutActivityType")]
		NSString WorkoutActivityType { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutActivityDuration")]
		NSString WorkoutActivityDuration { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutActivityStartDate")]
		NSString WorkoutActivityStartDate { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutActivityEndDate")]
		NSString WorkoutActivityEndDate { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutActivitySumQuantity")]
		NSString WorkoutActivitySumQuantity { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutActivityMinimumQuantity")]
		NSString WorkoutActivityMinimumQuantity { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutActivityMaximumQuantity")]
		NSString WorkoutActivityMaximumQuantity { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutActivityAverageQuantity")]
		NSString WorkoutActivityAverageQuantity { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutSumQuantity")]
		NSString WorkoutSumQuantity { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutMinimumQuantity")]
		NSString WorkoutMinimumQuantity { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutMaximumQuantity")]
		NSString WorkoutMaximumQuantity { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutAverageQuantity")]
		NSString WorkoutAverageQuantity { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKPredicateKeyPathWorkoutActivity")]
		NSString WorkoutActivity { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HKPredicateKeyPathWorkoutEffortRelationship")]
		NSString WorkoutEffortRelationship { get; }
	}

	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface HKDetailedCdaErrorKeys {
		[Field ("HKDetailedCDAValidationErrorKey")]
		NSString ValidationErrorKey { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("HKDetailedCdaErrorKeys")]
	[Internal]
	interface HKDetailedCdaErrors {
		NSString ValidationError { get; }
	}

	/// <summary>An <see cref="T:HealthKit.HKSample" /> whose value is one of an enumerated type.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKCategorySample_Class/index.html">Apple documentation for <c>HKCategorySample</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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
		[Wrap ("FromType (type, value, startDate, endDate, metadata.GetDictionary ())")]
		HKCategorySample FromType (HKCategoryType type, nint value, NSDate startDate, NSDate endDate, HKMetadata metadata);

		[Static]
		[Export ("categorySampleWithType:value:startDate:endDate:")]
		HKCategorySample FromType (HKCategoryType type, nint value, NSDate startDate, NSDate endDate);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("categorySampleWithType:value:startDate:endDate:device:metadata:")]
		HKCategorySample FromType (HKCategoryType type, nint value, NSDate startDate, NSDate endDate, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary<NSString, NSObject> metadata);
	}

	/// <summary>Abstract HealthKit store health document.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKDocumentSample">Apple documentation for <c>HKDocumentSample</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSample))]
	[Abstract] // as per docs
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKDocumentSample
	interface HKDocumentSample {
		[MacCatalyst (13, 1)]
		[Export ("documentType", ArgumentSemantic.Strong)]
		HKDocumentType DocumentType { get; }
	}

	/// <summary>Contains the information that is represented in XML in a HealthKit store health document.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKCDADocumentSample">Apple documentation for <c>HKCDADocumentSample</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKDocumentSample), Name = "HKCDADocumentSample")]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCDADocumentSample
	interface HKCdaDocumentSample {
		[NullAllowed, Export ("document")]
		HKCdaDocument Document { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("CDADocumentSampleWithData:startDate:endDate:metadata:validationError:")]
		[return: NullAllowed]
		HKCdaDocumentSample Create (NSData documentData, NSDate startDate, NSDate endDate, [NullAllowed] NSDictionary metadata, out NSError validationError);

		[Static, Wrap ("Create (documentData, startDate, endDate, metadata.GetDictionary (), out validationError)")]
		[return: NullAllowed]
		HKCdaDocumentSample Create (NSData documentData, NSDate startDate, NSDate endDate, HKMetadata metadata, out NSError validationError);
	}

	/// <summary>Contains the information that is represented in XML in a HealthKit store health document.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKCDADocument">Apple documentation for <c>HKCDADocument</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "HKCDADocument")]
	[DisableDefaultCtor] // as per docs
	interface HKCdaDocument {
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

	/// <summary>A correlation between two pieces of health data (for instance, blood pressure).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKCorrelation_Class/index.html">Apple documentation for <c>HKCorrelation</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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

		[Static, Wrap ("Create (correlationType, startDate, endDate, objects, metadata.GetDictionary ())")]
		HKCorrelation Create (HKCorrelationType correlationType, NSDate startDate, NSDate endDate, NSSet objects, HKMetadata metadata);

		[Static, Export ("correlationWithType:startDate:endDate:objects:")]
		HKCorrelation Create (HKCorrelationType correlationType, NSDate startDate, NSDate endDate, NSSet objects);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("correlationWithType:startDate:endDate:objects:device:metadata:")]
		HKCorrelation Create (HKCorrelationType correlationType, NSDate startDate, NSDate endDate, NSSet<HKSample> objects, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary<NSString, NSObject> metadata);
	}

	/// <summary>Completion handler for <see cref="T:HealthKit.HKCorrelationQuery" />.</summary>
	delegate void HKCorrelationQueryResultHandler (HKCorrelationQuery query, HKCorrelation [] correlations, NSError error);

	/// <summary>An <see cref="T:HealthKit.HKQuery" /> that returns only data that had been stored with correlations. (Note: Systolic and diastolic blood pressure readings are not correlated.)</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKCorrelationQuery_Class/index.html">Apple documentation for <c>HKCorrelationQuery</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCorrelationQuery
	interface HKCorrelationQuery {
		[Export ("initWithType:predicate:samplePredicates:completion:")]
		NativeHandle Constructor (HKCorrelationType correlationType, [NullAllowed] NSPredicate predicate, [NullAllowed] NSDictionary samplePredicates, HKCorrelationQueryResultHandler completion);

		[Export ("correlationType", ArgumentSemantic.Copy)]
		HKCorrelationType CorrelationType { get; }

		[NullAllowed, Export ("samplePredicates", ArgumentSemantic.Copy)]
		NSDictionary SamplePredicates { get; }
	}

	/// <summary>An <see cref="T:HealthKit.HKSampleType" /> that specifies a correlation between two types of data (for instance, blood pressure).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKCorrelationType_Class/index.html">Apple documentation for <c>HKCorrelationType</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCorrelationType
	interface HKCorrelationType {

	}

	/// <param name="requestStatus">The resulting request status.</param>
	///     <param name="error">The error, if one occurred..</param>
	///     <summary>Handler to pass to <see cref="M:HealthKit.HKHealthStore.GetRequestStatusForAuthorizationToShare(Foundation.NSSet{HealthKit.HKSampleType},Foundation.NSSet{HealthKit.HKObjectType},HealthKit.HKHealthStoreGetRequestStatusForAuthorizationToShareHandler)" />.</summary>
	delegate void HKHealthStoreGetRequestStatusForAuthorizationToShareHandler (HKAuthorizationRequestStatus requestStatus, NSError error);
	delegate void HKHealthStoreRecoverActiveWorkoutSessionHandler (HKWorkoutSession session, NSError error);
	delegate void HKHealthStoreCompletionHandler (bool success, NSError error);

	/// <include file="../docs/api/HealthKit/HKHealthStore.xml" path="/Documentation/Docs[@DocId='T:HealthKit.HKHealthStore']/*" />
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface HKHealthStore {
		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Static]
		[Export ("isHealthDataAvailable")]
		bool IsHealthDataAvailable { get; }

		[MacCatalyst (13, 1)]
		[Export ("supportsHealthRecords")]
		bool SupportsHealthRecords { get; }

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("authorizationStatusForType:")]
		HKAuthorizationStatus GetAuthorizationStatus (HKObjectType type);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Async]
		[Export ("requestAuthorizationToShareTypes:readTypes:completion:")]
		void RequestAuthorizationToShare ([NullAllowed] NSSet typesToShare, [NullAllowed] NSSet typesToRead, Action<bool, NSError> completion);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Async]
		[Export ("saveObject:withCompletion:")]
		void SaveObject (HKObject obj, Action<bool, NSError> completion);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Async]
		[Export ("saveObjects:withCompletion:")]
		void SaveObjects (HKObject [] objects, Action<bool, NSError> completion);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Async]
		[Export ("deleteObject:withCompletion:")]
		void DeleteObject (HKObject obj, Action<bool, NSError> completion);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("deleteObjects:withCompletion:")]
		void DeleteObjects (HKObject [] objects, Action<bool, NSError> completion);

		[MacCatalyst (13, 1)]
		[Export ("deleteObjectsOfType:predicate:withCompletion:")]
		void DeleteObjects (HKObjectType objectType, NSPredicate predicate, Action<bool, nuint, NSError> completion);

		[MacCatalyst (13, 1)]
		[Export ("earliestPermittedSampleDate")]
		NSDate EarliestPermittedSampleDate { get; }

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("executeQuery:")]
		void ExecuteQuery (HKQuery query);

		[MacCatalyst (13, 1)]
		[Export ("fitzpatrickSkinTypeWithError:")]
		[return: NullAllowed]
		HKFitzpatrickSkinTypeObject GetFitzpatrickSkinType (out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("wheelchairUseWithError:")]
		[return: NullAllowed]
		HKWheelchairUseObject GetWheelchairUse (out NSError error);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("activityMoveModeWithError:")]
		[return: NullAllowed]
		HKActivityMoveModeObject GetActivityMoveMode ([NullAllowed] out NSError error);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("stopQuery:")]
		void StopQuery (HKQuery query);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'GetDateOfBirthComponents' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetDateOfBirthComponents' instead.")]
		[Export ("dateOfBirthWithError:")]
		[return: NullAllowed]
		NSDate GetDateOfBirth (out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("dateOfBirthComponentsWithError:")]
		[return: NullAllowed]
		NSDateComponents GetDateOfBirthComponents (out NSError error);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("biologicalSexWithError:")]
		[return: NullAllowed]
		HKBiologicalSexObject GetBiologicalSex (out NSError error);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Export ("bloodTypeWithError:")]
		[return: NullAllowed]
		HKBloodTypeObject GetBloodType (out NSError error);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("enableBackgroundDeliveryForType:frequency:withCompletion:")]
		void EnableBackgroundDelivery (HKObjectType type, HKUpdateFrequency frequency, Action<bool, NSError> completion);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("disableBackgroundDeliveryForType:withCompletion:")]
		void DisableBackgroundDelivery (HKObjectType type, Action<bool, NSError> completion);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("disableAllBackgroundDeliveryWithCompletion:")]
		void DisableAllBackgroundDelivery (Action<bool, NSError> completion);

		// FIXME NS_EXTENSION_UNAVAILABLE("Not available to extensions") ;
		[Async]
		[MacCatalyst (13, 1)]
		[Export ("handleAuthorizationForExtensionWithCompletion:")]
		void HandleAuthorizationForExtension (Action<bool, NSError> completion);

		[Deprecated (PlatformName.iOS, 11, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("splitTotalEnergy:startDate:endDate:resultsHandler:")]
		void SplitTotalEnergy (HKQuantity totalEnergy, NSDate startDate, NSDate endDate, Action<HKQuantity, HKQuantity, NSError> resultsHandler);

		// HKWorkout category

		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'HKWorkoutBuilder.Add (HKSample [] samples, HKWorkoutBuilderCompletionHandler completionHandler)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use 'HKWorkoutBuilder.Add (HKSample [] samples, HKWorkoutBuilderCompletionHandler completionHandler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'HKWorkoutBuilder.Add (HKSample [] samples, HKWorkoutBuilderCompletionHandler completionHandler)' instead.")]
		[Export ("addSamples:toWorkout:completion:")]
		void AddSamples (HKSample [] samples, HKWorkout workout, HKStoreSampleAddedCallback callback);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("startWorkoutSession:")]
		void StartWorkoutSession (HKWorkoutSession workoutSession);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("endWorkoutSession:")]
		void EndWorkoutSession (HKWorkoutSession workoutSession);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("pauseWorkoutSession:")]
		void PauseWorkoutSession (HKWorkoutSession workoutSession);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("resumeWorkoutSession:")]
		void ResumeWorkoutSession (HKWorkoutSession workoutSession);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("startWatchAppWithWorkoutConfiguration:completion:")]
		void StartWatchApp (HKWorkoutConfiguration workoutConfiguration, Action<bool, NSError> completion);

		// HKUserPreferences category

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("preferredUnitsForQuantityTypes:completion:")]
		void GetPreferredUnits (NSSet quantityTypes, Action<NSDictionary, NSError> completion);

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("HKUserPreferencesDidChangeNotification")]
		NSString UserPreferencesDidChangeNotification { get; }

		[Async]
		[MacCatalyst (13, 1)]
		[Export ("getRequestStatusForAuthorizationToShareTypes:readTypes:completion:")]
		void GetRequestStatusForAuthorizationToShare (NSSet<HKSampleType> typesToShare, NSSet<HKObjectType> typesToRead, HKHealthStoreGetRequestStatusForAuthorizationToShareHandler completion);

		[Async]
		[NoiOS]
		[NoMacCatalyst]
		[Export ("recoverActiveWorkoutSessionWithCompletion:")]
		void RecoverActiveWorkoutSession (HKHealthStoreRecoverActiveWorkoutSessionHandler completion);

		[Async]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Export ("recalibrateEstimatesForSampleType:atDate:completion:")]
		void RecalibrateEstimates (HKSampleType sampleType, NSDate date, Action<bool, NSError> completion);

		[iOS (16, 0), Mac (13, 0), NoTV, MacCatalyst (16, 0)]
		[Async]
		[Export ("requestPerObjectReadAuthorizationForType:predicate:completion:")]
		void RequestPerObjectReadAuthorization (HKObjectType objectType, [NullAllowed] NSPredicate predicate, HKHealthStoreCompletionHandler completion);

		[NullAllowed]
		// xtro says this exists on macOS, introspection disagrees.
		// Headers doesn't say neither that it's available nor that it's not on macOS, which is probably why xtro picks it up.
		// Assuming that the lack of unavailability in the headers is a mistake, so remove from macOS.
		[iOS (17, 0), NoMac, NoTV, MacCatalyst (17, 0)]
		[Export ("workoutSessionMirroringStartHandler", ArgumentSemantic.Copy)]
		Action<HKWorkoutSession> WorkoutSessionMirroringStartHandler { get; set; }

		[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("authorizationViewControllerPresenter")]
		UIViewController AuthorizationViewControllerPresenter { get; set; }
	}

	/// <summary>Completion handler for <see cref="M:HealthKit.HKHealthStore.AddSamples(HealthKit.HKSample[],HealthKit.HKWorkout,HealthKit.HKStoreSampleAddedCallback)" />.</summary>
	delegate void HKStoreSampleAddedCallback (bool success, NSError error);

	/// <summary>Returned by <see cref="M:HealthKit.HKHealthStore.GetBiologicalSex(Foundation.NSError@)" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKBiologicalSexObject_Class/index.html">Apple documentation for <c>HKBiologicalSexObject</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface HKBiologicalSexObject : NSCopying, NSSecureCoding {
		[Export ("biologicalSex")]
		HKBiologicalSex BiologicalSex { get; }
	}

	/// <summary>Returned by <see cref="M:HealthKit.HKHealthStore.GetBloodType(Foundation.NSError@)" /></summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/help/HKBloodTypeObject_Class/index.html">Apple documentation for <c>HKBloodTypeObject</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface HKBloodTypeObject : NSCopying, NSSecureCoding {
		[Export ("bloodType")]
		HKBloodType BloodType { get; }
	}

	[iOS (13, 0)]
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor]
	interface HKAudiogramSampleType { }

	/// <summary>A key-value store for various types of health-related metadata.</summary>
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

		[MacCatalyst (13, 1)]
		[Export ("SexualActivityProtectionUsed")]
		bool SexualActivityProtectionUsed { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("MenstrualCycleStart")]
		bool MenstrualCycleStart { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("WeatherCondition")]
		HKWeatherCondition WeatherCondition { get; }

		[MacCatalyst (13, 1)]
		[Export ("WeatherTemperature")]
		HKQuantity WeatherTemperature { get; }

		[MacCatalyst (13, 1)]
		[Export ("WeatherHumidity")]
		HKQuantity WeatherHumidity { get; }

		[MacCatalyst (13, 1)]
		[Export ("LapLength")]
		NSString LapLength { get; }

		[MacCatalyst (13, 1)]
		[Export ("SwimmingLocationType")]
		NSString SwimmingLocationType { get; }

		[MacCatalyst (13, 1)]
		[Export ("SwimmingStrokeStyle")]
		NSString SwimmingStrokeStyle { get; }

		[MacCatalyst (13, 1)]
		[Export ("SyncIdentifier")]
		string SyncIdentifier { get; }

		[MacCatalyst (13, 1)]
		[Export ("SyncVersion")]
		int SyncVersion { get; }

		[MacCatalyst (13, 1)]
		[Export ("InsulinDeliveryReason")]
		HKInsulinDeliveryReason InsulinDeliveryReason { get; }

		[MacCatalyst (13, 1)]
		[Export ("BloodGlucoseMealTime")]
		HKBloodGlucoseMealTime BloodGlucoseMealTime { get; }

		[MacCatalyst (13, 1)]
		[Export ("VO2MaxTestType")]
		HKVO2MaxTestType VO2MaxTestType { get; }

		[MacCatalyst (13, 1)]
		[Export ("HeartRateMotionContext")]
		HKHeartRateMotionContext HeartRateMotionContext { get; }

		[MacCatalyst (13, 1)]
		[Export ("AverageSpeed")]
		HKQuantity AverageSpeed { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("MaximumSpeed")]
		HKQuantity MaximumSpeed { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("AlpineSlopeGrade")]
		HKQuantity AlpineSlopeGrade { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("ElevationAscended")]
		HKQuantity ElevationAscended { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("ElevationDescended")]
		HKQuantity ElevationDescended { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("FitnessMachineDuration")]
		HKQuantity FitnessMachineDuration { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("IndoorBikeDistance")]
		HKQuantity IndoorBikeDistance { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("CrossTrainerDistance")]
		HKQuantity CrossTrainerDistance { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("HeartRateEventThreshold")]
		HKQuantity HeartRateEventThreshold { get; set; }

		[MacCatalyst (18, 2), Mac (15, 2), iOS (18, 2)]
		[Export ("AppleFitnessPlusCatalogIdentifier")]
		string AppleFitnessPlusCatalogIdentifier { get; set; }
	}

	/// <summary>Defines the keys in the <see cref="T:HealthKit.HKMetadata" /> key-value dictionary.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeySexualActivityProtectionUsed")]
		NSString SexualActivityProtectionUsed { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyMenstrualCycleStart")]
		NSString MenstrualCycleStart { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyWeatherCondition")]
		NSString WeatherCondition { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyWeatherTemperature")]
		NSString WeatherTemperature { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyWeatherHumidity")]
		NSString WeatherHumidity { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyLapLength")]
		NSString LapLength { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeySwimmingLocationType")]
		NSString SwimmingLocationType { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeySwimmingStrokeStyle")]
		NSString SwimmingStrokeStyle { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeySyncIdentifier")]
		NSString SyncIdentifier { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeySyncVersion")]
		NSString SyncVersion { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyInsulinDeliveryReason")]
		NSString InsulinDeliveryReason { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyBloodGlucoseMealTime")]
		NSString BloodGlucoseMealTime { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyVO2MaxTestType")]
		NSString VO2MaxTestType { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyHeartRateMotionContext")]
		NSString HeartRateMotionContext { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyAverageSpeed")]
		NSString AverageSpeed { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyMaximumSpeed")]
		NSString MaximumSpeed { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyAlpineSlopeGrade")]
		NSString AlpineSlopeGrade { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyElevationAscended")]
		NSString ElevationAscended { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyElevationDescended")]
		NSString ElevationDescended { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyFitnessMachineDuration")]
		NSString FitnessMachineDuration { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyIndoorBikeDistance")]
		NSString IndoorBikeDistance { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyCrossTrainerDistance")]
		NSString CrossTrainerDistance { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyHeartRateEventThreshold")]
		NSString HeartRateEventThreshold { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyAverageMETs")]
		NSString AverageMets { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKMetadataKeyAudioExposureLevel")]
		NSString AudioExposureLevel { get; }

		[iOS (14, 2)]
		[MacCatalyst (14, 2)]
		[Field ("HKMetadataKeyAudioExposureDuration")]
		NSString AudioExposureDuration { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKMetadataKeyDevicePlacementSide")]
		NSString DevicePlacementSide { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKMetadataKeyBarometricPressure")]
		NSString BarometricPressure { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKMetadataKeyAppleECGAlgorithmVersion")]
		NSString AppleEcgAlgorithmVersion { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKMetadataKeyAppleDeviceCalibrated")]
		NSString AppleDeviceCalibrated { get; }

		[iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("HKMetadataKeyVO2MaxValue")]
		NSString VO2MaxValue { get; }

		[iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("HKMetadataKeyLowCardioFitnessEventThreshold")]
		NSString LowCardioFitnessEventThreshold { get; }

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Field ("HKMetadataKeyDateOfEarliestDataUsedForEstimate")]
		NSString DateOfEarliestDataUsedForEstimate { get; }

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Field ("HKMetadataKeyAlgorithmVersion")]
		NSString AlgorithmVersion { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKMetadataKeySWOLFScore")]
		NSString SwolfScore { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKMetadataKeyQuantityClampedToLowerBound")]
		NSString QuantityClampedToLowerBound { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKMetadataKeyQuantityClampedToUpperBound")]
		NSString QuantityClampedToUpperBound { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKMetadataKeyGlassesPrescriptionDescription")]
		NSString GlassesPrescriptionDescription { get; }

		[MacCatalyst (16, 4), Mac (13, 3), iOS (16, 4)]
		[Field ("HKMetadataKeyHeadphoneGain")]
		NSString HeadphoneGain { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKMetadataKeyHeartRateRecoveryTestType")]
		NSString HeartRateRecoveryTestType { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKMetadataKeyHeartRateRecoveryActivityType")]
		NSString HeartRateRecoveryActivityType { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKMetadataKeyHeartRateRecoveryActivityDuration")]
		NSString HeartRateRecoveryActivityDuration { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKMetadataKeyHeartRateRecoveryMaxObservedRecoveryHeartRate")]
		NSString HeartRateRecoveryMaxObservedRecoveryHeartRate { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKMetadataKeySessionEstimate")]
		NSString SessionEstimate { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKMetadataKeyUserMotionContext")]
		NSString UserMotionContext { get; }

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKMetadataKeyActivityType")]
		NSString KeyActivityType { get; }

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKMetadataKeyPhysicalEffortEstimationType")]
		NSString PhysicalEffortEstimationType { get; }

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKMetadataKeyAppleFitnessPlusSession")]
		NSString AppleFitnessPlusSession { get; }

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKMetadataKeyCyclingFunctionalThresholdPowerTestType")]
		NSString CyclingFunctionalThresholdPowerTestType { get; }

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKMetadataKeyMaximumLightIntensity")]
		NSString MaximumLightIntensity { get; }

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKMetadataKeyWaterSalinity")]
		NSString WaterSalinity { get; }

		[MacCatalyst (18, 2), Mac (15, 2), iOS (18, 2)]
		[Field ("HKMetadataKeyAppleFitnessPlusCatalogIdentifier")]
		NSString AppleFitnessPlusCatalogIdentifier { get; }
	}

	/// <summary>Base class to <see cref="T:HealthKit.HKSample" />, which defines sampling data.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKObject_Class/index.html">Apple documentation for <c>HKObject</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Abstract] // as per docs
#endif
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKObject : NSSecureCoding {
		[Export ("UUID", ArgumentSemantic.Strong)]
		NSUuid Uuid { get; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("source", ArgumentSemantic.Strong)]
		HKSource Source { get; }

		[NullAllowed, Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary WeakMetadata { get; }

		[Wrap ("WeakMetadata")]
		HKMetadata Metadata { get; }

		[MacCatalyst (13, 1)]
		[Export ("sourceRevision", ArgumentSemantic.Strong)]
		HKSourceRevision SourceRevision { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("device", ArgumentSemantic.Strong)]
		HKDevice Device { get; }
	}

	/// <summary>Base class for types of data storable in the Health Kit database.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKObjectType_Class/index.html">Apple documentation for <c>HKObjectType</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Abstract]
#endif
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKObjectType : NSSecureCoding, NSCopying {
		// These identifiers come from HKTypeIdentifiers
		[Export ("identifier")]
		NSString Identifier { get; }

#if NET
		[Internal]
#else
		[Obsolete ("Use 'HKQuantityType.Create (HKQuantityTypeIdentifier)'.")]
#endif
		[Static]
		[Export ("quantityTypeForIdentifier:")]
		[return: NullAllowed]
		HKQuantityType GetQuantityType ([NullAllowed] NSString hkTypeIdentifier);

#if NET
		[Internal]
#else
		[Obsolete ("Use 'HKCategoryType.Create (HKCategoryTypeIdentifier)'.")]
#endif
		[Static]
		[Export ("categoryTypeForIdentifier:")]
		[return: NullAllowed]
		HKCategoryType GetCategoryType ([NullAllowed] NSString hkCategoryTypeIdentifier);

#if NET
		[Internal]
#else
		[Obsolete ("Use 'HKCharacteristicType.Create (HKCharacteristicTypeIdentifier)'.")]
#endif
		[Static]
		[Export ("characteristicTypeForIdentifier:")]
		[return: NullAllowed]
		HKCharacteristicType GetCharacteristicType ([NullAllowed] NSString hkCharacteristicTypeIdentifier);

#if NET
		[Internal]
#else
		[Obsolete ("Use 'HKCorrelationType.Create (HKCorrelationTypeIdentifier)'.")]
#endif
		[Static, Export ("correlationTypeForIdentifier:")]
		[return: NullAllowed]
		HKCorrelationType GetCorrelationType ([NullAllowed] NSString hkCorrelationTypeIdentifier);

		[MacCatalyst (13, 1)]
		[Internal]
		[Static]
		[Export ("documentTypeForIdentifier:")]
		[return: NullAllowed]
		HKDocumentType _GetDocumentType (NSString hkDocumentTypeIdentifier);

		[Static, Export ("workoutType")]
#if NET
		HKWorkoutType WorkoutType { get; }
#else
		HKWorkoutType GetWorkoutType ();
#endif

		[Mac (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("activitySummaryType")]
		HKActivitySummaryType ActivitySummaryType { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("seriesTypeForIdentifier:")]
		[return: NullAllowed]
		HKSeriesType GetSeriesType (string identifier);

		[MacCatalyst (13, 1)]
		[Static, Internal]
		[Export ("clinicalTypeForIdentifier:")]
		[return: NullAllowed]
		HKClinicalType GetClinicalType (NSString identifier);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetClinicalType (identifier.GetConstant ()!)")]
		[return: NullAllowed]
		HKClinicalType GetClinicalType (HKClinicalTypeIdentifier identifier);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("audiogramSampleType")]
		HKAudiogramSampleType AudiogramSampleType { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("electrocardiogramType")]
		HKElectrocardiogramType ElectrocardiogramType { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("visionPrescriptionType")]
		HKPrescriptionType VisionPrescriptionType { get; }

		[iOS (16, 0), Mac (13, 0), NoTV, MacCatalyst (16, 0)]
		[Export ("requiresPerObjectAuthorization")]
		bool RequiresPerObjectAuthorization { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Static]
		[return: NullAllowed]
		[Export ("scoredAssessmentTypeForIdentifier:")]
		HKScoredAssessmentType GetScoredAssessmentType ([BindAs (typeof (HKScoredAssessmentTypeIdentifier))] NSString identifier);

		[Static]
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("stateOfMindType")]
		HKStateOfMindType StateOfMindType { get; }
	}

	[iOS (14, 0), Mac (13, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKElectrocardiogram
	interface HKElectrocardiogramType {

	}

	/// <summary>An <see cref="T:HealthKit.HKObjectType" /> that specifies a permanent aspect of the user.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKCharacteristicType_Class/index.html">Apple documentation for <c>HKCharacteristicType</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKObjectType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCharacteristicType
	interface HKCharacteristicType {

	}

	/// <summary>An <see cref="T:HealthKit.HKObject" /> that represents data that is sampled at a specific time or sampled over a time period.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKSampleType_Class/index.html">Apple documentation for <c>HKSampleType</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSampleType
	[BaseType (typeof (HKObjectType))]
	[Abstract] // The HKSampleType class is an abstract subclass of the HKObjectType class, used to represent data samples. Never instantiate an HKSampleType object directly. Instead, you should always work with one of its concrete subclasses [...]
	interface HKSampleType {
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("isMaximumDurationRestricted")]
		bool IsMaximumDurationRestricted { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("maximumAllowedDuration")]
		double MaximumAllowedDuration { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("isMinimumDurationRestricted")]
		bool IsMinimumDurationRestricted { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("minimumAllowedDuration")]
		double MinimumAllowedDuration { get; }

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Export ("allowsRecalibrationForEstimates")]
		bool AllowsRecalibrationForEstimates { get; }
	}

	/// <summary>A sample type for a clinical record.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKClinicalType
	interface HKClinicalType {

	}

	/// <summary>An <see cref="T:HealthKit.HKSampleType" /> that currently has only one form: sleep analysis.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKCategoryType_Class/index.html">Apple documentation for <c>HKCategoryType</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKCategoryType
	interface HKCategoryType {

	}

	/// <summary>Contains a constant that identifies the CDA document type.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKDocumentType">Apple documentation for <c>HKDocumentType</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKDocumentType
	interface HKDocumentType {

	}

	/// <include file="../docs/api/HealthKit/HKQuantityType.xml" path="/Documentation/Docs[@DocId='T:HealthKit.HKQuantityType']/*" />
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKQuantityType
	interface HKQuantityType {
		[Export ("aggregationStyle")]
		HKQuantityAggregationStyle AggregationStyle { get; }

		[Export ("isCompatibleWithUnit:")]
		bool IsCompatible (HKUnit unit);
	}

	/// <summary>Update handler for <see cref="T:HealthKit.HKObserverQuery" /> objects.</summary>
	delegate void HKObserverQueryUpdateHandler (HKObserverQuery query, [BlockCallback] Action completion, NSError error);

	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
	delegate void HKObserverQueryDescriptorUpdateHandler (HKObserverQuery query, NSSet<HKSampleType> samples, [BlockCallback] Action completion, NSError error);

	/// <summary>An <see cref="T:HealthKit.HKQuery" /> that runs once initially and then is automatically executed when relevant data is added to the database .</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKObserverQuery_Class/index.html">Apple documentation for <c>HKObserverQuery</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
#if NET
	[Abstract]
#endif
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKObserverQuery
	interface HKObserverQuery {
		[Export ("initWithSampleType:predicate:updateHandler:")]
		NativeHandle Constructor (HKSampleType sampleType, [NullAllowed] NSPredicate predicate, HKObserverQueryUpdateHandler updateHandler);

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Export ("initWithQueryDescriptors:updateHandler:")]
		NativeHandle Constructor (HKQueryDescriptor [] queryDescriptors, HKObserverQueryDescriptorUpdateHandler updateHandler);
	}

	/// <summary>Represents a measurable quantity of a certain type of unit, with a <see langword="double" /> value and a <see cref="T:HealthKit.HKUnit" /> type.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKQuantity_Class/index.html">Apple documentation for <c>HKQuantity</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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

	/// <include file="../docs/api/HealthKit/HKQuantitySample.xml" path="/Documentation/Docs[@DocId='T:HealthKit.HKQuantitySample']/*" />
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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
		[Wrap ("FromType (quantityType, quantity, startDate, endDate, metadata.GetDictionary ())")]
		HKQuantitySample FromType (HKQuantityType quantityType, HKQuantity quantity, NSDate startDate, NSDate endDate, HKMetadata metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("quantitySampleWithType:quantity:startDate:endDate:device:metadata:")]
		HKQuantitySample FromType (HKQuantityType quantityType, HKQuantity quantity, NSDate startDate, NSDate endDate, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary<NSString, NSObject> metadata);

		[MacCatalyst (13, 1)]
		[Export ("count")]
		nint Count { get; }
	}

	/// <summary>Base class for querying Health Kit databases.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKQuery_Class/index.html">Apple documentation for <c>HKQuery</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKQuery {
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("objectType", ArgumentSemantic.Strong)]
		HKObjectType ObjectType { get; }

		[Deprecated (PlatformName.iOS, 9, 3, message: "Use 'ObjectType' property.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ObjectType' property.")]
		[NullAllowed, Export ("sampleType", ArgumentSemantic.Strong)]
		HKSampleType SampleType { get; }

		[NullAllowed, Export ("predicate", ArgumentSemantic.Strong)]
		NSPredicate Predicate { get; }

		// HKQuery (HKObjectPredicates) Category

		[Static]
		[Export ("predicateForObjectsWithMetadataKey:")]
		NSPredicate GetPredicateForMetadataKey (NSString metadataKey);

		[Static]
		[Export ("predicateForObjectsWithMetadataKey:allowedValues:")]
		NSPredicate GetPredicateForMetadataKey (NSString metadataKey, NSObject [] allowedValues);

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

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("predicateForObjectsFromDevices:")]
		NSPredicate GetPredicateForObjectsFromDevices (NSSet<HKDevice> devices);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("predicateForObjectsWithDeviceProperty:allowedValues:")]
		NSPredicate GetPredicateForObjectsWithDeviceProperty (string key, NSSet<NSString> allowedValues);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("predicateForObjectsFromSourceRevisions:")]
		NSPredicate GetPredicateForObjectsFromSourceRevisions (NSSet<HKSourceRevision> sourceRevisions);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("predicateForObjectsAssociatedWithElectrocardiogram:")]
		NSPredicate GetPredicateForObjects (HKElectrocardiogram electrocardiogram);

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

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:totalSwimmingStrokeCount:")]
		NSPredicate GetPredicateForTotalSwimmingStrokeCount (NSPredicateOperatorType operatorType, HKQuantity totalSwimmingStrokeCount);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:totalFlightsClimbed:")]
		NSPredicate GetPredicateForTotalFlightsClimbed (NSPredicateOperatorType operatorType, HKQuantity totalFlightsClimbed);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:quantityType:sumQuantity:")]
		NSPredicate GetSumQuantityPredicateForWorkouts (NSPredicateOperatorType operatorType, HKQuantityType quantityType, HKQuantity sumQuantity);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:quantityType:minimumQuantity:")]
		NSPredicate GetMinimumQuantityPredicateForWorkouts (NSPredicateOperatorType operatorType, HKQuantityType quantityType, HKQuantity minimumQuantity);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:quantityType:maximumQuantity:")]
		NSPredicate GetMaximumQuantityPredicateForWorkouts (NSPredicateOperatorType operatorType, HKQuantityType quantityType, HKQuantity maximumQuantity);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutsWithOperatorType:quantityType:averageQuantity:")]
		NSPredicate GetAverageQuantityPredicateForWorkouts (NSPredicateOperatorType operatorType, HKQuantityType quantityType, HKQuantity averageQuantity);

		// HKActivitySummaryPredicates

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("predicateForActivitySummaryWithDateComponents:")]
		NSPredicate GetPredicateForActivitySummary (NSDateComponents dateComponents);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("predicateForActivitySummariesBetweenStartDateComponents:endDateComponents:")]
		NSPredicate GetPredicateForActivitySummariesBetween (NSDateComponents startDateComponents, NSDateComponents endDateComponents);


		// @interface HKClinicalRecordPredicates (HKQuery)
		[MacCatalyst (13, 1)]
		[Static, Internal]
		[Export ("predicateForClinicalRecordsWithFHIRResourceType:")]
		NSPredicate GetPredicateForClinicalRecords (NSString resourceType);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetPredicateForClinicalRecords (resourceType.GetConstant ()!)")]
		NSPredicate GetPredicateForClinicalRecords (HKFhirResourceType resourceType);

		[MacCatalyst (13, 1)]
		[Static, Internal]
		[Export ("predicateForClinicalRecordsFromSource:FHIRResourceType:identifier:")]
		NSPredicate GetPredicateForClinicalRecords (HKSource source, string resourceType, string identifier);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetPredicateForClinicalRecords (source, resourceType.GetConstant (), identifier)")]
		NSPredicate GetPredicateForClinicalRecords (HKSource source, HKFhirResourceType resourceType, string identifier);

		// @interface HKElectrocardiogramPredicates (HKQuery)

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("predicateForElectrocardiogramsWithClassification:")]
		NSPredicate GetPredicateForElectrocardiograms (HKElectrocardiogramClassification classification);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("predicateForElectrocardiogramsWithSymptomsStatus:")]
		NSPredicate GetPredicateForElectrocardiograms (HKElectrocardiogramSymptomsStatus symptomsStatus);

		// @interface HKVerifiableClinicalRecordPredicates (HKQuery)
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("predicateForVerifiableClinicalRecordsWithRelevantDateWithinDateInterval:")]
		NSPredicate GetPredicateForVerifiableClinicalRecords (NSDateInterval dateInterval);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForCategorySamplesEqualToValues:")]
		NSPredicate GetPredicateForCategorySamples (NSSet<NSNumber> values);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutActivitiesWithWorkoutActivityType:")]
		NSPredicate GetPredicateForWorkoutActivities (HKWorkoutActivityType workoutActivityType);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutActivitiesWithOperatorType:duration:")]
		NSPredicate GetPredicateForWorkoutActivities (NSPredicateOperatorType operatorType, double duration);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutActivitiesWithStartDate:endDate:options:")]
		NSPredicate GetPredicateForWorkoutActivities ([NullAllowed] NSDate startDate, [NullAllowed] NSDate endDate, HKQueryOptions options);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutActivitiesWithOperatorType:quantityType:sumQuantity:")]
		NSPredicate GetSumQuantityPredicateForWorkoutActivities (NSPredicateOperatorType operatorType, HKQuantityType quantityType, HKQuantity sumQuantity);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutActivitiesWithOperatorType:quantityType:minimumQuantity:")]
		NSPredicate GetMinimumQuantityPredicateForWorkoutActivities (NSPredicateOperatorType operatorType, HKQuantityType quantityType, HKQuantity minimumQuantity);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutActivitiesWithOperatorType:quantityType:maximumQuantity:")]
		NSPredicate GetMaximumQuantityPredicateForWorkoutActivities (NSPredicateOperatorType operatorType, HKQuantityType quantityType, HKQuantity maximumQuantity);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutActivitiesWithOperatorType:quantityType:averageQuantity:")]
		NSPredicate GetAverageQuantityPredicateForWorkoutActivities (NSPredicateOperatorType operatorType, HKQuantityType quantityType, HKQuantity averageQuantity);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("predicateForWorkoutsWithActivityPredicate:")]
		NSPredicate GetPredicateForWorkouts (NSPredicate activityPredicate);

		[Static]
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("predicateForWorkoutEffortSamplesRelatedToWorkout:activity:")]
		NSPredicate GetPredicateForWorkoutEffortSamplesRelatedToWorkout (HKWorkout workout, [NullAllowed] HKWorkoutActivity activity);

		// Category HKQuery (HKStateOfMind)
		[Static]
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("predicateForStatesOfMindWithValence:operatorType:")]
		NSPredicate GetPredicateForStatesOfMind (double valence, NSPredicateOperatorType operatorType);

		// Category HKQuery (HKStateOfMind)
		[Static]
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("predicateForStatesOfMindWithKind:")]
		NSPredicate GetPredicateForStatesOfMind (HKStateOfMindKind kind);

		// Category HKQuery (HKStateOfMind)
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Static]
		[Export ("predicateForStatesOfMindWithLabel:")]
		NSPredicate GetPredicateForStatesOfMind (HKStateOfMindLabel label);

		// Category HKQuery (HKStateOfMind)
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Static]
		[Export ("predicateForStatesOfMindWithAssociation:")]
		NSPredicate GetPredicateForStatesOfMind (HKStateOfMindAssociation association);
	}

	/// <summary>A measurement of health information. Base class for <see cref="T:HealthKit.HKQuantitySample" /> and <see cref="T:HealthKit.HKCategorySample" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKSample_Class/index.html">Apple documentation for <c>HKSample</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKObject))]
#if NET
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

		[iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Export ("hasUndeterminedDuration")]
		bool HasUndeterminedDuration { get; }
	}

	/// <summary>Result handler for <see cref="C:HealthKit.HKSampleQuery" />.</summary>
	delegate void HKSampleQueryResultsHandler (HKSampleQuery query, [NullAllowed] HKSample [] results, [NullAllowed] NSError error);

	/// <summary>An <see cref="T:HealthKit.HKQuery" /> that retrieves <see cref="T:HealthKit.HKSampleType" /> data from the database.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKSampleQuery_Class/index.html">Apple documentation for <c>HKSampleQuery</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSampleQuery
	interface HKSampleQuery {

		[Export ("limit")]
		nuint Limit { get; }

		[NullAllowed, Export ("sortDescriptors")]
		NSSortDescriptor [] SortDescriptors { get; }

		[Export ("initWithSampleType:predicate:limit:sortDescriptors:resultsHandler:")]
		NativeHandle Constructor (HKSampleType sampleType, [NullAllowed] NSPredicate predicate, nuint limit, [NullAllowed] NSSortDescriptor [] sortDescriptors, HKSampleQueryResultsHandler resultsHandler);

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Export ("initWithQueryDescriptors:limit:resultsHandler:")]
		NativeHandle Constructor (HKQueryDescriptor [] queryDescriptors, nint limit, HKSampleQueryResultsHandler resultsHandler);

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Export ("initWithQueryDescriptors:limit:sortDescriptors:resultsHandler:")]
		NativeHandle Constructor (HKQueryDescriptor [] queryDescriptors, nint limit, NSSortDescriptor [] sortDescriptors, HKSampleQueryResultsHandler resultsHandler);
	}

	/// <summary>A provider of health data, such as a particular sensor or application.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKSource_Class/index.html">Apple documentation for <c>HKSource</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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

	/// <summary>Completion handler for <see cref="C:HealthKit.HKSourceQuery" />.</summary>
	delegate void HKSourceQueryCompletionHandler (HKSourceQuery query, NSSet sources, NSError error);

	/// <summary>Class that represents a query for HealthKit data.</summary>
	///     
	///     <!-- Apple undocumented 2014-08-27 -->
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKSourceQuery_Class/index.html">Apple documentation for <c>HKSourceQuery</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKSourceQuery
	interface HKSourceQuery {

		[Export ("initWithSampleType:samplePredicate:completionHandler:")]
		NativeHandle Constructor (HKSampleType sampleType, [NullAllowed] NSPredicate objectPredicate, HKSourceQueryCompletionHandler completionHandler);
	}

	/// <summary>Provides basic statistical operations on health information.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKStatistics_Class/index.html">Apple documentation for <c>HKStatistics</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKStatistics : NSSecureCoding, NSCopying {
		[Export ("quantityType", ArgumentSemantic.Strong)]
		HKQuantityType QuantityType { get; }

		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; }

		[NullAllowed, Export ("sources")]
		HKSource [] Sources { get; }

		[Export ("averageQuantityForSource:")]
		[return: NullAllowed]
		HKQuantity AverageQuantity (HKSource source);

		[Export ("averageQuantity")]
		[return: NullAllowed]
		HKQuantity AverageQuantity ();

		[Export ("minimumQuantityForSource:")]
		[return: NullAllowed]
		HKQuantity MinimumQuantity (HKSource source);

		[Export ("minimumQuantity")]
		[return: NullAllowed]
		HKQuantity MinimumQuantity ();

		[Export ("maximumQuantityForSource:")]
		[return: NullAllowed]
		HKQuantity MaximumQuantity (HKSource source);

		[Export ("maximumQuantity")]
		[return: NullAllowed]
		HKQuantity MaximumQuantity ();

		[Export ("sumQuantityForSource:")]
		[return: NullAllowed]
		HKQuantity SumQuantity (HKSource source);

		[Export ("sumQuantity")]
		[return: NullAllowed]
		HKQuantity SumQuantity ();

		[MacCatalyst (13, 1)]
		[Export ("mostRecentQuantityForSource:")]
		[return: NullAllowed]
		HKQuantity GetMostRecentQuantity (HKSource source);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("mostRecentQuantity")]
		HKQuantity MostRecentQuantity { get; }

		[MacCatalyst (13, 1)]
		[Export ("mostRecentQuantityDateIntervalForSource:")]
		[return: NullAllowed]
		NSDateInterval GetMostRecentQuantityDateInterval (HKSource source);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("mostRecentQuantityDateInterval")]
		NSDateInterval MostRecentQuantityDateInterval { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("duration")]
		HKQuantity Duration { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("durationForSource:")]
		[return: NullAllowed]
		HKQuantity GetDuration (HKSource source);
	}

	/// <summary>Delegate handler for <see cref="M:HealthKit.HKStatisticsCollection.EnumerateStatistics(Foundation.NSDate,Foundation.NSDate,HealthKit.HKStatisticsCollectionEnumerator)" />.</summary>
	delegate void HKStatisticsCollectionEnumerator (HKStatistics result, bool stop);

	/// <summary>A group of related statistics (generally representing a time series).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKStatisticsCollection_Class/index.html">Apple documentation for <c>HKStatisticsCollection</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	[BaseType (typeof (NSObject))]
	interface HKStatisticsCollection {

		[Export ("statisticsForDate:")]
		[return: NullAllowed]
		HKStatistics GetStatistics (NSDate date);

		[Export ("enumerateStatisticsFromDate:toDate:withBlock:")]
		void EnumerateStatistics (NSDate startDate, NSDate endDate, HKStatisticsCollectionEnumerator handler);

		[Export ("statistics")]
		HKStatistics [] Statistics { get; }

		[Export ("sources")]
		NSSet Sources { get; }
	}

	/// <summary>Results handler for <see cref="M:HealthKit.HKStatisticsCollectionQuery.SetInitialResultsHandler(HealthKit.HKStatisticsCollectionQueryInitialResultsHandler)" /> and <see cref="M:HealthKit.HKStatisticsCollectionQuery.SetStatisticsUpdateHandler(HealthKit.HKStatisticsCollectionQueryInitialResultsHandler)" />.</summary>
	delegate void HKStatisticsCollectionQueryInitialResultsHandler (HKStatisticsCollectionQuery query, HKStatisticsCollection result, NSError error);
	delegate void HKStatisticsCollectionQueryStatisticsUpdateHandler (HKStatisticsCollectionQuery query, HKStatistics statistics, HKStatisticsCollection collection, NSError error);


	/// <summary>An <see cref="T:HealthKit.HKQuery" /> that produces a collection of statistics (for instance, number of steps per day for the previous month).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKStatisticsCollectionQuery_Class/index.html">Apple documentation for <c>HKStatisticsCollectionQuery</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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
		NativeHandle Constructor (HKQuantityType quantityType, [NullAllowed] NSPredicate quantitySamplePredicate, HKStatisticsOptions options, NSDate anchorDate, NSDateComponents intervalComponents);
	}

	/// <summary>Results handler for <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=C:HKStatisticsQuery&amp;scope=Xamarin" title="C:HKStatisticsQuery">C:HKStatisticsQuery</a></format>.</summary>
	delegate void HKStatisticsQueryHandler (HKStatisticsQuery query, HKStatistics result, NSError error);

	/// <summary>An <see cref="T:HealthKit.HKQuery" /> that can calculate basic statistics (such as the sum and mean) on its constituent data.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKStatisticsQuery_Class/index.html">Apple documentation for <c>HKStatisticsQuery</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKStatisticsQuery
	interface HKStatisticsQuery {

		[Export ("initWithQuantityType:quantitySamplePredicate:options:completionHandler:")]
		NativeHandle Constructor (HKQuantityType quantityType, [NullAllowed] NSPredicate quantitySamplePredicate, HKStatisticsOptions options, HKStatisticsQueryHandler handler);
	}

	/// <summary>Enumerates the types of <see cref="T:HealthKit.HKQuantityType" />.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	enum HKQuantityTypeIdentifier {

		[Field ("HKQuantityTypeIdentifierBodyMassIndex")]
		BodyMassIndex,

		[Field ("HKQuantityTypeIdentifierBodyFatPercentage")]
		BodyFatPercentage,

		[Field ("HKQuantityTypeIdentifierHeight")]
		Height,

		[Field ("HKQuantityTypeIdentifierBodyMass")]
		BodyMass,

		[Field ("HKQuantityTypeIdentifierLeanBodyMass")]
		LeanBodyMass,

		[Field ("HKQuantityTypeIdentifierHeartRate")]
		HeartRate,

		[Field ("HKQuantityTypeIdentifierStepCount")]
		StepCount,

		[Field ("HKQuantityTypeIdentifierDistanceWalkingRunning")]
		DistanceWalkingRunning,

		[Field ("HKQuantityTypeIdentifierDistanceCycling")]
		DistanceCycling,

		[Field ("HKQuantityTypeIdentifierBasalEnergyBurned")]
		BasalEnergyBurned,

		[Field ("HKQuantityTypeIdentifierActiveEnergyBurned")]
		ActiveEnergyBurned,

		[Field ("HKQuantityTypeIdentifierFlightsClimbed")]
		FlightsClimbed,

		[Field ("HKQuantityTypeIdentifierNikeFuel")]
		NikeFuel,

		// Blood
		[Field ("HKQuantityTypeIdentifierOxygenSaturation")]
		OxygenSaturation,

		[Field ("HKQuantityTypeIdentifierBloodGlucose")]
		BloodGlucose,

		[Field ("HKQuantityTypeIdentifierBloodPressureSystolic")]
		BloodPressureSystolic,

		[Field ("HKQuantityTypeIdentifierBloodPressureDiastolic")]
		BloodPressureDiastolic,

		[Field ("HKQuantityTypeIdentifierBloodAlcoholContent")]
		BloodAlcoholContent,

		[Field ("HKQuantityTypeIdentifierPeripheralPerfusionIndex")]
		PeripheralPerfusionIndex,

		[Field ("HKQuantityTypeIdentifierForcedVitalCapacity")]
		ForcedVitalCapacity,

		[Field ("HKQuantityTypeIdentifierForcedExpiratoryVolume1")]
		ForcedExpiratoryVolume1,

		[Field ("HKQuantityTypeIdentifierPeakExpiratoryFlowRate")]
		PeakExpiratoryFlowRate,

		// Miscellaneous
		[Field ("HKQuantityTypeIdentifierNumberOfTimesFallen")]
		NumberOfTimesFallen,

		[Field ("HKQuantityTypeIdentifierInhalerUsage")]
		InhalerUsage,

		[Field ("HKQuantityTypeIdentifierRespiratoryRate")]
		RespiratoryRate,

		[Field ("HKQuantityTypeIdentifierBodyTemperature")]
		BodyTemperature,

		// Nutrition
		[Field ("HKQuantityTypeIdentifierDietaryFatTotal")]
		DietaryFatTotal,

		[Field ("HKQuantityTypeIdentifierDietaryFatPolyunsaturated")]
		DietaryFatPolyunsaturated,

		[Field ("HKQuantityTypeIdentifierDietaryFatMonounsaturated")]
		DietaryFatMonounsaturated,

		[Field ("HKQuantityTypeIdentifierDietaryFatSaturated")]
		DietaryFatSaturated,

		[Field ("HKQuantityTypeIdentifierDietaryCholesterol")]
		DietaryCholesterol,

		[Field ("HKQuantityTypeIdentifierDietarySodium")]
		DietarySodium,

		[Field ("HKQuantityTypeIdentifierDietaryCarbohydrates")]
		DietaryCarbohydrates,

		[Field ("HKQuantityTypeIdentifierDietaryFiber")]
		DietaryFiber,

		[Field ("HKQuantityTypeIdentifierDietarySugar")]
		DietarySugar,

		[Field ("HKQuantityTypeIdentifierDietaryEnergyConsumed")]
		DietaryEnergyConsumed,

		[Field ("HKQuantityTypeIdentifierDietaryProtein")]
		DietaryProtein,

		[Field ("HKQuantityTypeIdentifierDietaryVitaminA")]
		DietaryVitaminA,

		[Field ("HKQuantityTypeIdentifierDietaryVitaminB6")]
		DietaryVitaminB6,

		[Field ("HKQuantityTypeIdentifierDietaryVitaminB12")]
		DietaryVitaminB12,

		[Field ("HKQuantityTypeIdentifierDietaryVitaminC")]
		DietaryVitaminC,

		[Field ("HKQuantityTypeIdentifierDietaryVitaminD")]
		DietaryVitaminD,

		[Field ("HKQuantityTypeIdentifierDietaryVitaminE")]
		DietaryVitaminE,

		[Field ("HKQuantityTypeIdentifierDietaryVitaminK")]
		DietaryVitaminK,

		[Field ("HKQuantityTypeIdentifierDietaryCalcium")]
		DietaryCalcium,

		[Field ("HKQuantityTypeIdentifierDietaryIron")]
		DietaryIron,

		[Field ("HKQuantityTypeIdentifierDietaryThiamin")]
		DietaryThiamin,

		[Field ("HKQuantityTypeIdentifierDietaryRiboflavin")]
		DietaryRiboflavin,

		[Field ("HKQuantityTypeIdentifierDietaryNiacin")]
		DietaryNiacin,

		[Field ("HKQuantityTypeIdentifierDietaryFolate")]
		DietaryFolate,

		[Field ("HKQuantityTypeIdentifierDietaryBiotin")]
		DietaryBiotin,

		[Field ("HKQuantityTypeIdentifierDietaryPantothenicAcid")]
		DietaryPantothenicAcid,

		[Field ("HKQuantityTypeIdentifierDietaryPhosphorus")]
		DietaryPhosphorus,

		[Field ("HKQuantityTypeIdentifierDietaryIodine")]
		DietaryIodine,

		[Field ("HKQuantityTypeIdentifierDietaryMagnesium")]
		DietaryMagnesium,

		[Field ("HKQuantityTypeIdentifierDietaryZinc")]
		DietaryZinc,

		[Field ("HKQuantityTypeIdentifierDietarySelenium")]
		DietarySelenium,

		[Field ("HKQuantityTypeIdentifierDietaryCopper")]
		DietaryCopper,

		[Field ("HKQuantityTypeIdentifierDietaryManganese")]
		DietaryManganese,

		[Field ("HKQuantityTypeIdentifierDietaryChromium")]
		DietaryChromium,

		[Field ("HKQuantityTypeIdentifierDietaryMolybdenum")]
		DietaryMolybdenum,

		[Field ("HKQuantityTypeIdentifierDietaryChloride")]
		DietaryChloride,

		[Field ("HKQuantityTypeIdentifierDietaryPotassium")]
		DietaryPotassium,

		[Field ("HKQuantityTypeIdentifierDietaryCaffeine")]
		DietaryCaffeine,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierBasalBodyTemperature")]
		BasalBodyTemperature,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierDietaryWater")]
		DietaryWater,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierUVExposure")]
		UVExposure,

		[Field ("HKQuantityTypeIdentifierElectrodermalActivity")]
		ElectrodermalActivity,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierAppleExerciseTime")]
		AppleExerciseTime,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierDistanceWheelchair")]
		DistanceWheelchair,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierPushCount")]
		PushCount,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierDistanceSwimming")]
		DistanceSwimming,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierSwimmingStrokeCount")]
		SwimmingStrokeCount,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierWaistCircumference")]
		WaistCircumference,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierVO2Max")]
		VO2Max,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierDistanceDownhillSnowSports")]
		DistanceDownhillSnowSports,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierInsulinDelivery")]
		InsulinDelivery,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierRestingHeartRate")]
		RestingHeartRate,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierWalkingHeartRateAverage")]
		WalkingHeartRateAverage,

		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierHeartRateVariabilitySDNN")]
		HeartRateVariabilitySdnn,

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierAppleStandTime")]
		AppleStandTime,

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierEnvironmentalAudioExposure")]
		EnvironmentalAudioExposure,

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKQuantityTypeIdentifierHeadphoneAudioExposure")]
		HeadphoneAudioExposure,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKQuantityTypeIdentifierSixMinuteWalkTestDistance")]
		SixMinuteWalkTestDistance,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKQuantityTypeIdentifierStairAscentSpeed")]
		StairAscentSpeed,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKQuantityTypeIdentifierStairDescentSpeed")]
		StairDescentSpeed,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKQuantityTypeIdentifierWalkingAsymmetryPercentage")]
		WalkingAsymmetryPercentage,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKQuantityTypeIdentifierWalkingDoubleSupportPercentage")]
		WalkingDoubleSupportPercentage,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKQuantityTypeIdentifierWalkingSpeed")]
		WalkingSpeed,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKQuantityTypeIdentifierWalkingStepLength")]
		WalkingStepLength,

		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("HKQuantityTypeIdentifierAppleMoveTime")]
		AppleMoveTime,

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Field ("HKQuantityTypeIdentifierAppleWalkingSteadiness")]
		AppleWalkingSteadiness,

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Field ("HKQuantityTypeIdentifierNumberOfAlcoholicBeverages")]
		NumberOfAlcoholicBeverages,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKQuantityTypeIdentifierHeartRateRecoveryOneMinute")]
		HeartRateRecoveryOneMinute,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKQuantityTypeIdentifierRunningGroundContactTime")]
		RunningGroundContactTime,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKQuantityTypeIdentifierRunningStrideLength")]
		RunningStrideLength,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKQuantityTypeIdentifierRunningVerticalOscillation")]
		RunningVerticalOscillation,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKQuantityTypeIdentifierRunningPower")]
		RunningPower,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKQuantityTypeIdentifierRunningSpeed")]
		RunningSpeed,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Field ("HKQuantityTypeIdentifierAtrialFibrillationBurden")]
		AtrialFibrillationBurden,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKQuantityTypeIdentifierAppleSleepingWristTemperature")]
		AppleSleepingWristTemperature,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKQuantityTypeIdentifierUnderwaterDepth")]
		UnderwaterDepth,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKQuantityTypeIdentifierWaterTemperature")]
		WaterTemperature,

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKQuantityTypeIdentifierCyclingCadence")]
		CyclingCadence,

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKQuantityTypeIdentifierCyclingFunctionalThresholdPower")]
		CyclingFunctionalThresholdPower,

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKQuantityTypeIdentifierCyclingPower")]
		CyclingPower,

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKQuantityTypeIdentifierCyclingSpeed")]
		CyclingSpeed,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKQuantityTypeIdentifierEnvironmentalSoundReduction")]
		EnvironmentalSoundReduction,

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKQuantityTypeIdentifierPhysicalEffort")]
		PhysicalEffort,

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("HKQuantityTypeIdentifierTimeInDaylight")]
		TimeInDaylight,

		[Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HKQuantityTypeIdentifierWorkoutEffortScore")]
		WorkoutEffortScore,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKQuantityTypeIdentifierCrossCountrySkiingSpeed")]
		CrossCountrySkiingSpeed,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKQuantityTypeIdentifierDistanceCrossCountrySkiing")]
		DistanceCrossCountrySkiing,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKQuantityTypeIdentifierDistancePaddleSports")]
		DistancePaddleSports,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKQuantityTypeIdentifierDistanceRowing")]
		DistanceRowing,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKQuantityTypeIdentifierDistanceSkatingSports")]
		DistanceSkatingSports,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKQuantityTypeIdentifierEstimatedWorkoutEffortScore")]
		EstimatedWorkoutEffortScore,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKQuantityTypeIdentifierPaddleSportsSpeed")]
		PaddleSportsSpeed,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKQuantityTypeIdentifierRowingSpeed")]
		RowingSpeed,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKQuantityTypeIdentifierAppleSleepingBreathingDisturbances")]
		AppleSleepingBreathingDisturbances,
	}

	/// <summary>Contains constants that identify HealthKit correlation types.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	enum HKCorrelationTypeIdentifier {
		[Field ("HKCorrelationTypeIdentifierBloodPressure")]
		BloodPressure,

		[Field ("HKCorrelationTypeIdentifierFood")]
		Food,
	}

	[iOS (13, 0), Mac (13, 0)]
	[MacCatalyst (13, 1)]
	enum HKDataTypeIdentifier {
		[Field ("HKDataTypeIdentifierHeartbeatSeries")]
		HeartbeatSeries,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("HKDataTypeIdentifierStateOfMind")]
		StateOfMind,
	}

	/// <summary>Enumerates the types of <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Health%20Kit%20HKCategory&amp;scope=Xamarin" title="T:HealthKit.HKCategory">T:HealthKit.HKCategory</a></format>; currently there is only the one form (Sleep Analysis).</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	enum HKCategoryTypeIdentifier {
		[Field ("HKCategoryTypeIdentifierSleepAnalysis")]
		SleepAnalysis,

		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierAppleStandHour")]
		AppleStandHour,

		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierCervicalMucusQuality")]
		CervicalMucusQuality,

		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierOvulationTestResult")]
		OvulationTestResult,

		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierMenstrualFlow")]
		MenstrualFlow,

		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierIntermenstrualBleeding")]
		IntermenstrualBleeding,

		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierSexualActivity")]
		SexualActivity,

		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierMindfulSession")]
		MindfulSession,

		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierHighHeartRateEvent")]
		HighHeartRateEvent,

		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierLowHeartRateEvent")]
		LowHeartRateEvent,

		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierIrregularHeartRhythmEvent")]
		IrregularHeartRhythmEvent,

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierAudioExposureEvent")]
		AudioExposureEvent,

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierToothbrushingEvent")]
		ToothbrushingEvent,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierAbdominalCramps")]
		AbdominalCramps,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierAcne")]
		Acne,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierAppetiteChanges")]
		AppetiteChanges,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierGeneralizedBodyAche")]
		GeneralizedBodyAche,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierBloating")]
		Bloating,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierBreastPain")]
		BreastPain,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierChestTightnessOrPain")]
		ChestTightnessOrPain,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierChills")]
		Chills,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierConstipation")]
		Constipation,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierCoughing")]
		Coughing,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierDiarrhea")]
		Diarrhea,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierDizziness")]
		Dizziness,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierFainting")]
		Fainting,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierFatigue")]
		Fatigue,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierFever")]
		Fever,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierHeadache")]
		Headache,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierHeartburn")]
		Heartburn,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierHotFlashes")]
		HotFlashes,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierLowerBackPain")]
		LowerBackPain,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierLossOfSmell")]
		LossOfSmell,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierLossOfTaste")]
		LossOfTaste,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierMoodChanges")]
		MoodChanges,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierNausea")]
		Nausea,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierPelvicPain")]
		PelvicPain,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierRapidPoundingOrFlutteringHeartbeat")]
		RapidPoundingOrFlutteringHeartbeat,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierRunnyNose")]
		RunnyNose,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierShortnessOfBreath")]
		ShortnessOfBreath,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierSinusCongestion")]
		SinusCongestion,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierSkippedHeartbeat")]
		SkippedHeartbeat,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierSleepChanges")]
		SleepChanges,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierSoreThroat")]
		SoreThroat,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierVomiting")]
		Vomiting,

		[iOS (13, 6)]
		[MacCatalyst (13, 1)]
		[Field ("HKCategoryTypeIdentifierWheezing")]
		Wheezing,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKCategoryTypeIdentifierBladderIncontinence")]
		BladderIncontinence,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKCategoryTypeIdentifierDrySkin")]
		DrySkin,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKCategoryTypeIdentifierHairLoss")]
		HairLoss,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKCategoryTypeIdentifierVaginalDryness")]
		VaginalDryness,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKCategoryTypeIdentifierMemoryLapse")]
		MemoryLapse,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKCategoryTypeIdentifierNightSweats")]
		NightSweats,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKCategoryTypeIdentifierEnvironmentalAudioExposureEvent")]
		EnvironmentalAudioExposureEvent,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKCategoryTypeIdentifierHandwashingEvent")]
		HandwashingEvent,

		[iOS (14, 2)]
		[MacCatalyst (14, 2)]
		[Field ("HKCategoryTypeIdentifierHeadphoneAudioExposureEvent")]
		HeadphoneAudioExposureEvent,

		[iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("HKCategoryTypeIdentifierPregnancy")]
		Pregnancy,

		[iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("HKCategoryTypeIdentifierLactation")]
		Lactation,

		[iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("HKCategoryTypeIdentifierContraceptive")]
		Contraceptive,

		[iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("HKCategoryTypeIdentifierLowCardioFitnessEvent")]
		LowCardioFitnessEvent,

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Field ("HKCategoryTypeIdentifierAppleWalkingSteadinessEvent")]
		AppleWalkingSteadinessEvent,

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Field ("HKCategoryTypeIdentifierPregnancyTestResult")]
		PregnancyTestResult,

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Field ("HKCategoryTypeIdentifierProgesteroneTestResult")]
		ProgesteroneTestResult,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKCategoryTypeIdentifierInfrequentMenstrualCycles")]
		InfrequentMenstrualCycles,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKCategoryTypeIdentifierIrregularMenstrualCycles")]
		IrregularMenstrualCycles,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKCategoryTypeIdentifierPersistentIntermenstrualBleeding")]
		PersistentIntermenstrualBleeding,

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0)]
		[Field ("HKCategoryTypeIdentifierProlongedMenstrualPeriods")]
		ProlongedMenstrualPeriods,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKCategoryTypeIdentifierBleedingAfterPregnancy")]
		BleedingAfterPregnancy,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKCategoryTypeIdentifierBleedingDuringPregnancy")]
		BleedingDuringPregnancy,

		[iOS (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("HKCategoryTypeIdentifierSleepApneaEvent")]
		SleepApneaEvent,
	}

	/// <summary>Enumerates the forms of <see cref="T:HealthKit.HKCharacteristicType" />.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	enum HKCharacteristicTypeIdentifier {
		[Field ("HKCharacteristicTypeIdentifierBiologicalSex")]
		BiologicalSex,

		[Field ("HKCharacteristicTypeIdentifierBloodType")]
		BloodType,

		[Field ("HKCharacteristicTypeIdentifierDateOfBirth")]
		DateOfBirth,

		[MacCatalyst (13, 1)]
		[Field ("HKCharacteristicTypeIdentifierFitzpatrickSkinType")]
		FitzpatrickSkinType,

		[MacCatalyst (13, 1)]
		[Field ("HKCharacteristicTypeIdentifierWheelchairUse")]
		WheelchairUse,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKCharacteristicTypeIdentifierActivityMoveMode")]
		ActivityMoveMode,
	}

	/// <summary>Definitions and utility methods for manipulating measurements of mass, length, volume, and energy.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKUnit_Class/index.html">Apple documentation for <c>HKUnit</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		[Export ("cupUSUnit")]
		HKUnit CupUSUnit { get; }

		[Static]
		[MacCatalyst (13, 1)]
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

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("inchesOfMercuryUnit")]
		HKUnit InchesOfMercury { get; }

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
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SmallCalorie' or 'LargeCalorie' instead.")]
		[Static]
		[Export ("calorieUnit")]
		HKUnit Calorie { get; }

		[Static]
		[Export ("kilocalorieUnit")]
		HKUnit Kilocalorie { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("smallCalorieUnit")]
		HKUnit SmallCalorie { get; }

		[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("internationalUnit")]
		HKUnit InternationalUnit { get; }

		// HKUnit (DecibelAWeightedSoundPressureLevel) Category
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("decibelAWeightedSoundPressureLevelUnit")]
		HKUnit DecibelAWeightedSoundPressureLevelUnit { get; }

		// HKUnit (HearingSensitivity) Category
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("decibelHearingLevelUnit")]
		HKUnit DecibelHearingLevelUnit { get; }

		// HKUnit (Frequency) Category

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("hertzUnitWithMetricPrefix:")]
		HKUnit GetHertzUnit (HKMetricPrefix prefix);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("hertzUnit")]
		HKUnit HertzUnit { get; }

		// HKUnit (ElectricPotentialDifference) Category

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("voltUnitWithMetricPrefix:")]
		HKUnit GetVolt (HKMetricPrefix prefix);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("voltUnit")]
		HKUnit Volt { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("diopterUnit")]
		HKUnit Diopter { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("prismDiopterUnit")]
		HKUnit PrismDiopter { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("wattUnitWithMetricPrefix:")]
		HKUnit CreateWatt (HKMetricPrefix prefix);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("wattUnit")]
		HKUnit Watt { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("radianAngleUnitWithMetricPrefix:")]
		HKUnit CreateRadianAngle (HKMetricPrefix prefix);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("radianAngleUnit")]
		HKUnit RadianAngle { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Static]
		[Export ("degreeAngleUnit")]
		HKUnit DegreeAngle { get; }

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0), NoTV]
		[Static]
		[Export ("luxUnitWithMetricPrefix:")]
		HKUnit CreateLux (HKMetricPrefix prefix);

		[MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0), NoTV]
		[Static]
		[Export ("luxUnit")]
		HKUnit Lux { get; }

		// HKUnit (UnitLess)
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Static]
		[Export ("appleEffortScoreUnit")]
		HKUnit AppleEffortScoreUnit { get; }
	}

	/// <summary>An <see cref="T:HealthKit.HKSample" /> that represents a physical workout.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKWorkout_Class/index.html">Apple documentation for <c>HKWorkout</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKWorkout
	interface HKWorkout {
		[Export ("workoutActivityType")]
		HKWorkoutActivityType WorkoutActivityType { get; }

		[NullAllowed, Export ("workoutEvents")]
		HKWorkoutEvent [] WorkoutEvents { get; }

		[Export ("duration", ArgumentSemantic.UnsafeUnretained)]
		double Duration { get; }

		[Deprecated (PlatformName.MacOSX, 15, 0)]
		[Deprecated (PlatformName.iOS, 18, 0)]
		[Deprecated (PlatformName.MacCatalyst, 18, 0)]
		[NullAllowed, Export ("totalEnergyBurned", ArgumentSemantic.Retain)]
		HKQuantity TotalEnergyBurned { get; }

		[Deprecated (PlatformName.MacOSX, 15, 0)]
		[Deprecated (PlatformName.iOS, 18, 0)]
		[Deprecated (PlatformName.MacCatalyst, 18, 0)]
		[NullAllowed, Export ("totalDistance", ArgumentSemantic.Retain)]
		HKQuantity TotalDistance { get; }

		[Deprecated (PlatformName.MacOSX, 15, 0)]
		[Deprecated (PlatformName.iOS, 18, 0)]
		[Deprecated (PlatformName.MacCatalyst, 18, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("totalSwimmingStrokeCount", ArgumentSemantic.Strong)]
		HKQuantity TotalSwimmingStrokeCount { get; }

		[Static, Export ("workoutWithActivityType:startDate:endDate:")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate);

		[Static, Export ("workoutWithActivityType:startDate:endDate:workoutEvents:totalEnergyBurned:totalDistance:metadata:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, [NullAllowed] HKWorkoutEvent [] workoutEvents, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] NSDictionary metadata);

		[Static, Wrap ("Create (workoutActivityType, startDate, endDate, workoutEvents, totalEnergyBurned, totalDistance, metadata.GetDictionary ())")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, HKWorkoutEvent [] workoutEvents, HKQuantity totalEnergyBurned, HKQuantity totalDistance, HKMetadata metadata);

		[Static, Export ("workoutWithActivityType:startDate:endDate:duration:totalEnergyBurned:totalDistance:metadata:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, double duration, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] NSDictionary metadata);

		[Static, Wrap ("Create (workoutActivityType, startDate, endDate, duration, totalEnergyBurned, totalDistance, metadata.GetDictionary ())")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, double duration, HKQuantity totalEnergyBurned, HKQuantity totalDistance, HKMetadata metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("workoutWithActivityType:startDate:endDate:workoutEvents:totalEnergyBurned:totalDistance:device:metadata:")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, [NullAllowed] HKWorkoutEvent [] workoutEvents, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("Create (workoutActivityType, startDate, endDate, workoutEvents, totalEnergyBurned, totalDistance, device, metadata.GetDictionary ())")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, HKWorkoutEvent [] workoutEvents, HKQuantity totalEnergyBurned, HKQuantity totalDistance, HKDevice device, HKMetadata metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("workoutWithActivityType:startDate:endDate:duration:totalEnergyBurned:totalDistance:device:metadata:")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, double duration, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("Create (workoutActivityType, startDate, endDate, duration, totalEnergyBurned, totalDistance, device, metadata.GetDictionary ())")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, double duration, HKQuantity totalEnergyBurned, HKQuantity totalDistance, HKDevice device, HKMetadata metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("workoutWithActivityType:startDate:endDate:workoutEvents:totalEnergyBurned:totalDistance:totalSwimmingStrokeCount:device:metadata:")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, [NullAllowed] HKWorkoutEvent [] workoutEvents, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] HKQuantity totalSwimmingStrokeCount, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("Create (workoutActivityType, startDate, endDate, workoutEvents, totalEnergyBurned, totalDistance, totalSwimmingStrokeCount, device, metadata.GetDictionary ())")]
		HKWorkout Create (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, HKWorkoutEvent [] workoutEvents, HKQuantity totalEnergyBurned, HKQuantity totalDistance, HKQuantity totalSwimmingStrokeCount, HKDevice device, HKMetadata metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("workoutWithActivityType:startDate:endDate:workoutEvents:totalEnergyBurned:totalDistance:totalFlightsClimbed:device:metadata:")]
		HKWorkout CreateFlightsClimbedWorkout (HKWorkoutActivityType workoutActivityType, NSDate startDate, NSDate endDate, [NullAllowed] HKWorkoutEvent [] workoutEvents, [NullAllowed] HKQuantity totalEnergyBurned, [NullAllowed] HKQuantity totalDistance, [NullAllowed] HKQuantity totalFlightsClimbed, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("CreateFlightsClimbedWorkout (workoutActivityType, startDate, endDate, workoutEvents, totalEnergyBurned, totalDistance, totalFlightsClimbed, device, metadata.GetDictionary ())")]
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

		[MacCatalyst (13, 1)]
		[Field ("HKWorkoutSortIdentifierTotalSwimmingStrokeCount")]
		NSString SortIdentifierTotalSwimmingStrokeCount { get; }

		[MacCatalyst (13, 1)]
		[Field ("HKWorkoutSortIdentifierTotalFlightsClimbed")]
		NSString SortIdentifierTotalFlightsClimbed { get; }

		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("totalFlightsClimbed", ArgumentSemantic.Strong)]
		HKQuantity TotalFlightsClimbed { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Export ("workoutActivities", ArgumentSemantic.Copy)]
		HKWorkoutActivity [] WorkoutActivities { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Export ("allStatistics", ArgumentSemantic.Copy)]
		NSDictionary<HKQuantityType, HKStatistics> AllStatistics { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Export ("statisticsForType:")]
		[return: NullAllowed]
		HKStatistics GetStatistics (HKQuantityType quantityType);
	}

	/// <summary>A pause or resumption of a workout.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKWorkoutEvent_Class/index.html">Apple documentation for <c>HKWorkoutEvent</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKWorkoutEvent : NSSecureCoding, NSCopying {
		[Export ("type")]
		HKWorkoutEventType Type { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'DateInterval' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DateInterval' instead.")]
		[Export ("date", ArgumentSemantic.Copy)]
		NSDate Date { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary WeakMetadata { get; }

		[MacCatalyst (13, 1)]
		[Wrap ("WeakMetadata")]
		HKMetadata Metadata { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[Static, Export ("workoutEventWithType:date:")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDate date);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		[Export ("workoutEventWithType:date:metadata:")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDate date, NSDictionary metadata);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Create (HKWorkoutEventType, NSDateInterval, HKMetadata)' instead.")]
		[Static]
		[Wrap ("Create (type, date, metadata.GetDictionary ()!)")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDate date, HKMetadata metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("workoutEventWithType:dateInterval:metadata:")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDateInterval dateInterval, [NullAllowed] NSDictionary metadata);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("Create (type, dateInterval, metadata.GetDictionary ())")]
		HKWorkoutEvent Create (HKWorkoutEventType type, NSDateInterval dateInterval, HKMetadata metadata);

		[MacCatalyst (13, 1)]
		[Export ("dateInterval", ArgumentSemantic.Copy)]
		NSDateInterval DateInterval { get; }
	}

	/// <summary>An <see cref="T:HealthKit.HKSampleType" /> representing a workout.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKWorkoutType_Class/index.html">Apple documentation for <c>HKWorkoutType</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKWorkoutType
	interface HKWorkoutType {
		[Field ("HKWorkoutTypeIdentifier")]
		NSString Identifier { get; }
	}

	/// <summary>Represents samples that have been deleted from the store.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKDeletedObject_ClassReference/index.html">Apple documentation for <c>HKDeletedObject</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKDeletedObject : NSSecureCoding {
		[Export ("UUID", ArgumentSemantic.Strong)]
		NSUuid Uuid { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary WeakMetadata { get; }

		[MacCatalyst (13, 1)]
		[Wrap ("WeakMetadata")]
		HKMetadata Metadata { get; }
	}

	/// <summary>Hardware that generates or consumes HealthKit data.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKDevice_ClassReference/index.html">Apple documentation for <c>HKDevice</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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
		NativeHandle Constructor ([NullAllowed] string name, [NullAllowed] string manufacturer, [NullAllowed] string model, [NullAllowed] string hardwareVersion, [NullAllowed] string firmwareVersion, [NullAllowed] string softwareVersion, [NullAllowed] string localIdentifier, [NullAllowed] string udiDeviceIdentifier);

		[Static]
		[Export ("localDevice")]
		HKDevice LocalDevice { get; }
	}

	/// <summary>Queries for documents in the HealthKit store.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKDocumentQuery">Apple documentation for <c>HKDocumentQuery</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKDocumentQuery
	interface HKDocumentQuery {
		[Export ("limit")]
		nuint Limit { get; }

		[NullAllowed, Export ("sortDescriptors", ArgumentSemantic.Copy)]
		NSSortDescriptor [] SortDescriptors { get; }

		[Export ("includeDocumentData")]
		bool IncludeDocumentData { get; }

		[Export ("initWithDocumentType:predicate:limit:sortDescriptors:includeDocumentData:resultsHandler:")]
		NativeHandle Constructor (HKDocumentType documentType, [NullAllowed] NSPredicate predicate, nuint limit, [NullAllowed] NSSortDescriptor [] sortDescriptors, bool includeDocumentData, Action<HKDocumentQuery, HKDocumentSample [], bool, NSError> resultsHandler);
	}

	/// <summary>Holds keys whose constant values relate to properties of a <see cref="T:HealthKit.HKDevice" />.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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

	/// <summary>Holds skin-type data.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKFitzpatrickSkinTypeObject_ClassReference/index.html">Apple documentation for <c>HKFitzpatrickSkinTypeObject</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface HKFitzpatrickSkinTypeObject : NSCopying, NSSecureCoding {
		[Export ("skinType")]
		HKFitzpatrickSkinType SkinType { get; }
	}

	/// <summary>Contains an enumeration that describes wheelchair use.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKWheelchairUseObject">Apple documentation for <c>HKWheelchairUseObject</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface HKWheelchairUseObject : NSCopying, NSSecureCoding {
		[Export ("wheelchairUse")]
		HKWheelchairUse WheelchairUse { get; }
	}

	/// <summary>Wraps <see cref="T:HealthKit.HKSource" />, adding version information.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/HealthKit/Reference/HKSourceRevision_ClassReference/index.html">Apple documentation for <c>HKSourceRevision</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKSourceRevision : NSSecureCoding, NSCopying {
		[Export ("source")]
		HKSource Source { get; }

		[NullAllowed, Export ("version")]
		string Version { get; }

		[Export ("initWithSource:version:")]
		NativeHandle Constructor (HKSource source, [NullAllowed] string version);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("productType")]
		string ProductType { get; }

		[MacCatalyst (13, 1)]
		[Export ("operatingSystemVersion", ArgumentSemantic.Assign)]
		NSOperatingSystemVersion OperatingSystemVersion { get; }

		[MacCatalyst (13, 1)]
		[Export ("initWithSource:version:productType:operatingSystemVersion:")]
		NativeHandle Constructor (HKSource source, [NullAllowed] string version, [NullAllowed] string productType, NSOperatingSystemVersion operatingSystemVersion);
	}

	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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

	/// <summary>Represents the most recent sample that was returned by a previous anchored object query.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKQueryAnchor">Apple documentation for <c>HKQueryAnchor</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKQueryAnchor : NSSecureCoding, NSCopying {
		[Static]
		[Export ("anchorFromValue:")]
		HKQueryAnchor Create (nuint value);
	}

	[Mac (13, 0)]
	[iOS (17, 0)]
	[MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKWorkoutSession : NSSecureCoding {
		[Export ("activityType")]
		HKWorkoutActivityType ActivityType { get; }

		[Export ("locationType")]
		HKWorkoutSessionLocationType LocationType { get; }

		[Export ("workoutConfiguration", ArgumentSemantic.Copy)]
		HKWorkoutConfiguration WorkoutConfiguration { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IHKWorkoutSessionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("state")]
		HKWorkoutSessionState State { get; }

		[NullAllowed, Export ("startDate")]
		NSDate StartDate { get; }

		[NullAllowed, Export ("endDate")]
		NSDate EndDate { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("initWithActivityType:locationType:")]
		NativeHandle Constructor (HKWorkoutActivityType activityType, HKWorkoutSessionLocationType locationType);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("initWithConfiguration:error:")]
		NativeHandle Constructor (HKWorkoutConfiguration workoutConfiguration, out NSError error);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("initWithHealthStore:configuration:error:")]
		NativeHandle Constructor (HKHealthStore healthStore, HKWorkoutConfiguration workoutConfiguration, [NullAllowed] out NSError error);

		[Export ("prepare")]
		void Prepare ();

		[Export ("startActivityWithDate:")]
		void StartActivity ([NullAllowed] NSDate date);

		[Export ("stopActivityWithDate:")]
		void StopActivity ([NullAllowed] NSDate date);

		[Export ("end")]
		void End ();

		[Export ("pause")]
		void Pause ();

		[Export ("resume")]
		void Resume ();

		[NoiOS]
		[NoMacCatalyst]
		[Export ("associatedWorkoutBuilder")]
		HKLiveWorkoutBuilder AssociatedWorkoutBuilder { get; }

		[NoTV]
		[Export ("beginNewActivityWithConfiguration:date:metadata:")]
		void BeginNewActivity (HKWorkoutConfiguration workoutConfiguration, NSDate date, [NullAllowed] NSDictionary<NSString, NSObject> metadata);

		[NoTV]
		[Export ("endCurrentActivityOnDate:")]
		void EndCurrentActivity (NSDate date);

		[NoTV]
		[Export ("currentActivity", ArgumentSemantic.Copy)]
		HKWorkoutActivity CurrentActivity { get; }

		[NoTV, Mac (14, 0)]
		[Export ("type")]
		HKWorkoutSessionType Type { get; }

		[NoTV, NoMacCatalyst, NoMac, iOS (17, 0)]
		[Export ("sendDataToRemoteWorkoutSession:completion:")]
		[Async]
		void SendDataToRemoteWorkoutSession (NSData data, Action<bool, NSError> completion);

		[NoTV, NoMacCatalyst, NoMac, NoiOS]
		[Export ("startMirroringToCompanionDeviceWithCompletion:")]
		[Async]
		void StartMirroringToCompanionDevice (Action<bool, NSError> completion);

		[NoTV, NoMacCatalyst, NoMac, NoiOS]
		[Export ("stopMirroringToCompanionDeviceWithCompletion:")]
		[Async]
		void StopMirroringToCompanionDevice (Action<bool, NSError> completion);
	}

	interface IHKWorkoutSessionDelegate { }

	[Mac (13, 0)]
	[iOS (17, 0)]
	[MacCatalyst (17, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface HKWorkoutSessionDelegate {
		[Abstract]
		[Export ("workoutSession:didChangeToState:fromState:date:")]
		void DidChangeToState (HKWorkoutSession workoutSession, HKWorkoutSessionState toState, HKWorkoutSessionState fromState, NSDate date);

		[Abstract]
		[Export ("workoutSession:didFailWithError:")]
		void DidFail (HKWorkoutSession workoutSession, NSError error);

		// // Issue filed at: https://github.com/xamarin/maccore/issues/2609
		[Export ("workoutSession:didGenerateEvent:")]
		void DidGenerateEvent (HKWorkoutSession workoutSession, HKWorkoutEvent @event);

		[NoTV, Mac (13, 0)]
		[Export ("workoutSession:didBeginActivityWithConfiguration:date:")]
		void DidBeginActivity (HKWorkoutSession workoutSession, HKWorkoutConfiguration workoutConfiguration, NSDate date);

		[NoTV, Mac (13, 0)]
		[Export ("workoutSession:didEndActivityWithConfiguration:date:")]
		void DidEndActivity (HKWorkoutSession workoutSession, HKWorkoutConfiguration workoutConfiguration, NSDate date);

		[iOS (17, 0), MacCatalyst (17, 0), NoTV, Mac (14, 0)]
		[Export ("workoutSession:didReceiveDataFromRemoteWorkoutSession:")]
		void DidReceiveData (HKWorkoutSession workoutSession, NSData [] data);

		[iOS (17, 0), MacCatalyst (17, 0), NoTV, Mac (14, 0)]
		[Export ("workoutSession:didDisconnectFromRemoteDeviceWithError:")]
		void DidDisconnect (HKWorkoutSession workoutSession, [NullAllowed] NSError error);
	}

	/// <summary>Summarizes user activity for a specific day.</summary>
	///     <remarks>Developers use a <see cref="T:HealthKit.HKActivitySummaryQuery" /> object to get a <see cref="T:HealthKit.HKActivitySummary" /> instance for a specific day. While developers can create <see cref="T:HealthKit.HKActivitySummary" /> themselves, they cannot save these to the store. Developers can display an active summary on iOS with the <see cref="T:HealthKitUI.HKActivityRingView" /> class.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKActivitySummary">Apple documentation for <c>HKActivitySummary</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface HKActivitySummary : NSSecureCoding, NSCopying {
		[Export ("dateComponentsForCalendar:")]
		NSDateComponents DateComponentsForCalendar (NSCalendar calendar);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("activityMoveMode", ArgumentSemantic.Assign)]
		HKActivityMoveMode ActivityMoveMode { get; set; }

		[Export ("activeEnergyBurned", ArgumentSemantic.Strong)]
		HKQuantity ActiveEnergyBurned { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("appleMoveTime", ArgumentSemantic.Strong)]
		HKQuantity AppleMoveTime { get; set; }

		[Export ("appleExerciseTime", ArgumentSemantic.Strong)]
		HKQuantity AppleExerciseTime { get; set; }

		[Export ("appleStandHours", ArgumentSemantic.Strong)]
		HKQuantity AppleStandHours { get; set; }

		[Export ("activeEnergyBurnedGoal", ArgumentSemantic.Strong)]
		HKQuantity ActiveEnergyBurnedGoal { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("appleMoveTimeGoal", ArgumentSemantic.Strong)]
		HKQuantity AppleMoveTimeGoal { get; set; }

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Mac (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("appleExerciseTimeGoal", ArgumentSemantic.Strong)]
		HKQuantity AppleExerciseTimeGoal { get; set; }

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("appleStandHoursGoal", ArgumentSemantic.Strong)]
		HKQuantity AppleStandHoursGoal { get; set; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[NullAllowed, Export ("exerciseTimeGoal", ArgumentSemantic.Strong)]
		HKQuantity ExerciseTimeGoal { get; set; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[NullAllowed, Export ("standHoursGoal", ArgumentSemantic.Strong)]
		HKQuantity StandHoursGoal { get; set; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("paused", ArgumentSemantic.Assign)]
		bool Paused { [Bind ("isPaused")] get; set; }

	}

	/// <summary>Gets <see cref="T:HealthKit.HKActivitySummary" /> instances that match an <see cref="T:Foundation.NSPredicate" />.</summary>
	///     <remarks>Developers can use the methods of the <see cref="T:HealthKit.HKQuery" /> class to create predicates that will call the handler in the <see cref="P:HealthKit.HKActivitySummaryQuery.UpdateHandler" /> property when a summary matches the query.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKActivitySummaryQuery">Apple documentation for <c>HKActivitySummaryQuery</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKActivitySummaryQuery
	interface HKActivitySummaryQuery {
		[NullAllowed, Export ("updateHandler", ArgumentSemantic.Copy)]
		Action<HKActivitySummaryQuery, HKActivitySummary [], NSError> UpdateHandler { get; set; }

		[Export ("initWithPredicate:resultsHandler:")]
		NativeHandle Constructor ([NullAllowed] NSPredicate predicate, Action<HKActivitySummaryQuery, HKActivitySummary [], NSError> handler);
	}

	/// <summary>Obect that is used to request permission to read <see cref="T:HealthKit.HKActivitySummary" /> objects.</summary>
	///     <remarks>Developers use the <see cref="P:HealthKit.HKObjectType.ActivitySummaryType" /> method.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKActivitySummaryType">Apple documentation for <c>HKActivitySummaryType</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKObjectType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKActivitySummaryType
	interface HKActivitySummaryType {
	}

	/// <summary>Contains workout configuration information.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/HealthKit/HKWorkoutConfiguration">Apple documentation for <c>HKWorkoutConfiguration</c></related>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
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

	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor]
	interface HKSeriesType {
		[Static]
		[Export ("workoutRouteType")]
		HKSeriesType WorkoutRouteType { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("heartbeatSeriesType")]
		HKSeriesType HeartbeatSeriesType { get; }
	}

	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKSeriesBuilder
#if !MONOMAC && !XAMCORE_5_0
		: NSSecureCoding
#endif
{
		[Export ("discard")]
		void Discard ();
	}

	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor]
	interface HKSeriesSample : NSCopying {
		[Export ("count")]
		nuint Count { get; }
	}

	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSeriesSample))]
	[DisableDefaultCtor]
	interface HKWorkoutRoute : NSCopying {

		[Field ("HKWorkoutRouteTypeIdentifier")]
		NSString TypeIdentifier { get; }
	}

	/// <param name="success">Whether the operation succeeded.</param>
	///     <param name="error">The error that occurred, if <paramref name="success" /> was <see langword="false" />.</param>
	///     <summary>Completion handler for adding metadata with <see cref="M:HealthKit.HKWorkoutRouteQuery.#ctor(HealthKit.HKWorkoutRoute,HealthKit.HKWorkoutRouteBuilderDataHandler)" />.</summary>
	delegate void HKWorkoutRouteBuilderAddMetadataHandler (bool success, NSError error);
	/// <summary>A class for adding geographical data to a workout as the user's location changes.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSeriesBuilder))]
	[DisableDefaultCtor]
	interface HKWorkoutRouteBuilder {
		[Export ("initWithHealthStore:device:")]
		NativeHandle Constructor (HKHealthStore healthStore, [NullAllowed] HKDevice device);

		[Async, Export ("insertRouteData:completion:")]
		void InsertRouteData (CLLocation [] routeData, Action<bool, NSError> completion);

		[Async, Protected, Export ("finishRouteWithWorkout:metadata:completion:")]
		void FinishRoute (HKWorkout workout, [NullAllowed] NSDictionary metadata, Action<HKWorkoutRoute, NSError> completion);

		[Async, Wrap ("FinishRoute (workout, metadata.GetDictionary (), completion)")]
		void FinishRoute (HKWorkout workout, HKMetadata metadata, Action<HKWorkoutRoute, NSError> completion);

		[MacCatalyst (13, 1)]
		[Async, Protected]
		[Export ("addMetadata:completion:")]
		void AddMetadata (NSDictionary metadata, HKWorkoutRouteBuilderAddMetadataHandler completion);

		[MacCatalyst (13, 1)]
		[Async, Wrap ("AddMetadata (metadata.GetDictionary ()!, completion)")]
		void AddMetadata (HKMetadata metadata, HKWorkoutRouteBuilderAddMetadataHandler completion);
	}

	delegate void HKWorkoutRouteQueryDataHandler (HKWorkoutRouteQuery query, [NullAllowed] CLLocation [] routeData, bool done, [NullAllowed] NSError error);

	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
	interface HKWorkoutRouteQuery {
		[Export ("initWithRoute:dataHandler:")]
		NativeHandle Constructor (HKWorkoutRoute workoutRoute, HKWorkoutRouteBuilderDataHandler dataHandler);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Export ("initWithRoute:dateInterval:dataHandler:")]
		NativeHandle Constructor (HKWorkoutRoute workoutRoute, NSDateInterval dateInterval, HKWorkoutRouteQueryDataHandler dataHandler);
	}

	/// <param name="success">Whether the operation succeeded.</param>
	///     <param name="error">The error that occurred, if <paramref name="success" /> was <see langword="false" />.</param>
	///     <summary>Completion handler for adding metadata with <see cref="M:HealthKit.HKWorkoutRouteQuery.#ctor(HealthKit.HKWorkoutRoute,HealthKit.HKWorkoutRouteBuilderDataHandler)" />.</summary>
	delegate void HKWorkoutBuilderCompletionHandler (bool success, NSError error);
	/// <summary>Builds a workout from workout data as it is added.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKWorkoutBuilder {
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
		HKWorkoutEvent [] WorkoutEvents { get; }

		[Export ("initWithHealthStore:configuration:device:")]
		NativeHandle Constructor (HKHealthStore healthStore, HKWorkoutConfiguration configuration, [NullAllowed] HKDevice device);

		[Async]
		[Export ("beginCollectionWithStartDate:completion:")]
		void BeginCollection (NSDate startDate, HKWorkoutBuilderCompletionHandler completionHandler);

		[Async]
		[Export ("addSamples:completion:")]
		void Add (HKSample [] samples, HKWorkoutBuilderCompletionHandler completionHandler);

		[Async]
		[Export ("addWorkoutEvents:completion:")]
		void Add (HKWorkoutEvent [] workoutEvents, HKWorkoutBuilderCompletionHandler completionHandler);

		[Async, Protected]
		[Export ("addMetadata:completion:")]
		void Add (NSDictionary metadata, HKWorkoutBuilderCompletionHandler completionHandler);

		[Async]
		[Wrap ("Add (metadata.GetDictionary ()!, completionHandler)")]
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
		[return: NullAllowed]
		HKSeriesBuilder GetSeriesBuilder (HKSeriesType seriesType);

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Export ("workoutActivities", ArgumentSemantic.Copy)]
		HKWorkoutActivity [] WorkoutActivities { get; }

		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Export ("allStatistics", ArgumentSemantic.Copy)]
		NSDictionary<HKQuantityType, HKStatistics> AllStatistics { get; }

		[Async]
		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Export ("addWorkoutActivity:completion:")]
		void AddWorkoutActivity (HKWorkoutActivity workoutActivity, HKWorkoutBuilderCompletionHandler completion);

		[Async]
		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Export ("updateActivityWithUUID:endDate:completion:")]
		void UpdateActivity (NSUuid uuid, NSDate endDate, HKWorkoutBuilderCompletionHandler completion);

		[Async]
		[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
		[Export ("updateActivityWithUUID:addMedatata:completion:")]
		void UpdateActivity (NSUuid uuid, NSDictionary<NSString, NSObject> metadata, HKWorkoutBuilderCompletionHandler completion);
	}

	/// <summary>A handler to pass to <see cref="M:HealthKit.HKQuantitySeriesSampleQuery.#ctor(HealthKit.HKQuantitySample,HealthKit.HKQuantitySeriesSampleQueryQuantityDelegate)" />.</summary>
	delegate void HKQuantitySeriesSampleQueryQuantityDelegate (HKQuantitySeriesSampleQuery query, HKQuantity quantity, NSDate date, bool done, NSError error);
	delegate void HKQuantitySeriesSampleQueryQuantityHandler (HKQuantitySeriesSampleQuery query, HKQuantity quantity, NSDateInterval date, bool done, NSError error);

	/// <summary>Queries series data in a quantity sample.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
	interface HKQuantitySeriesSampleQuery {
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("includeSample")]
		bool IncludeSample { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("orderByQuantitySampleStartDate")]
		bool OrderByQuantitySampleStartDate { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithQuantityType:predicate:quantityHandler:")]
		NativeHandle Constructor (HKQuantityType quantityType, [NullAllowed] NSPredicate predicate, HKQuantitySeriesSampleQueryQuantityHandler quantityHandler);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use Constructor that takes 'NSDateInterval' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Constructor that takes 'NSDateInterval' instead.")]
		[Export ("initWithSample:quantityHandler:")]
		NativeHandle Constructor (HKQuantitySample quantitySample, HKQuantitySeriesSampleQueryQuantityDelegate quantityHandler);
	}

	/// <param name="samples">The samples that were added.</param>
	///     <param name="error">The error, if one occurred.</param>
	///     <summary>Completion handler for <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Health%20Kit%20HKQuantity%20Series%20Sample%20Builder%20Finish%20Series&amp;scope=Xamarin" title="T:HealthKit.HKQuantitySeriesSampleBuilder.FinishSeries">T:HealthKit.HKQuantitySeriesSampleBuilder.FinishSeries</a></format>.</summary>
	delegate void HKQuantitySeriesSampleBuilderFinishSeriesDelegate (HKQuantitySample [] samples, NSError error);

	/// <summary>Builds quantity sample series.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKQuantitySeriesSampleBuilder {
		[Export ("initWithHealthStore:quantityType:startDate:device:")]
		NativeHandle Constructor (HKHealthStore healthStore, HKQuantityType quantityType, NSDate startDate, [NullAllowed] HKDevice device);

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
		[Wrap ("FinishSeries (metadata.GetDictionary (), completionHandler)")]
		void FinishSeries ([NullAllowed] HKMetadata metadata, HKQuantitySeriesSampleBuilderFinishSeriesDelegate completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("finishSeriesWithMetadata:endDate:completion:")]
		void FinishSeries ([NullAllowed] NSDictionary metadata, [NullAllowed] NSDate endDate, HKQuantitySeriesSampleBuilderFinishSeriesDelegate completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Async]
		[Wrap ("FinishSeries (metadata.GetDictionary (), endDate, completionHandler)")]
		void FinishSeries ([NullAllowed] HKMetadata metadata, [NullAllowed] NSDate endDate, HKQuantitySeriesSampleBuilderFinishSeriesDelegate completionHandler);


		[Export ("discard")]
		void Discard ();

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("insertQuantity:dateInterval:error:")]
		bool Insert (HKQuantity quantity, NSDateInterval dateInterval, [NullAllowed] out NSError error);
	}

	[NoiOS, Mac (13, 0)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKLiveWorkoutDataSource {
		[Export ("typesToCollect", ArgumentSemantic.Copy)]
		NSSet<HKQuantityType> TypesToCollect { get; }

		[Export ("initWithHealthStore:workoutConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (HKHealthStore healthStore, [NullAllowed] HKWorkoutConfiguration configuration);

		[Export ("enableCollectionForType:predicate:")]
		void EnableCollection (HKQuantityType quantityType, [NullAllowed] NSPredicate predicate);

		[Export ("disableCollectionForType:")]
		void DisableCollection (HKQuantityType quantityType);
	}

	/// <summary>Represents a Fast Healthcare Interoperability Resources (FHIR) resource.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "HKFHIRResource")]
	[DisableDefaultCtor]
	interface HKFhirResource : NSSecureCoding, NSCopying {
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

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("FHIRVersion", ArgumentSemantic.Copy)]
		HKFhirVersion FhirVersion { get; }
	}

	/// <summary>A cumulative data series.</summary>
	[Mac (13, 0)]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use HKCumulativeQuantitySample instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use HKCumulativeQuantitySample instead.")]
	[Deprecated (PlatformName.MacOSX, 13, 0, message: "Use HKCumulativeQuantitySample instead.")]
	[DisableDefaultCtor]
	[BaseType (typeof (HKCumulativeQuantitySample))]
	interface HKCumulativeQuantitySeriesSample {
		[Export ("sum", ArgumentSemantic.Copy)]
		HKQuantity Sum { get; }
	}

	[iOS (13, 0), Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuantitySample))]
	[DisableDefaultCtor]
	interface HKCumulativeQuantitySample {
		[Export ("sumQuantity", ArgumentSemantic.Copy)]
		HKQuantity SumQuantity { get; }
	}

	/// <summary>A sample for clinical records.</summary>
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (HKSample))]
	interface HKClinicalRecord : NSSecureCoding, NSCopying {
		[Export ("clinicalType", ArgumentSemantic.Copy)]
		HKClinicalType ClinicalType { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[NullAllowed, Export ("FHIRResource", ArgumentSemantic.Copy)]
		HKFhirResource FhirResource { get; }
	}

	interface IHKLiveWorkoutBuilderDelegate { }
	[NoiOS]
	[NoMacCatalyst]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface HKLiveWorkoutBuilderDelegate {
		[Abstract]
		[Export ("workoutBuilder:didCollectDataOfTypes:")]
		void DidCollectData (HKLiveWorkoutBuilder workoutBuilder, NSSet<HKSampleType> collectedTypes);

		[Abstract]
		[Export ("workoutBuilderDidCollectEvent:")]
		void DidCollectEvent (HKLiveWorkoutBuilder workoutBuilder);

		[NoiOS, Mac (13, 0), NoMacCatalyst, NoTV]
		[Export ("workoutBuilder:didBeginActivity:")]
		void DidBeginActivity (HKLiveWorkoutBuilder workoutBuilder, HKWorkoutActivity workoutActivity);

		[NoiOS, Mac (13, 0), NoMacCatalyst, NoTV]
		[Export ("workoutBuilder:didEndActivity:")]
		void DidEndActivity (HKLiveWorkoutBuilder workoutBuilder, HKWorkoutActivity workoutActivity);
	}

	[NoiOS, Mac (13, 0)]
	[NoMacCatalyst]
	[DisableDefaultCtor]
	[BaseType (typeof (HKWorkoutBuilder))]
	interface HKLiveWorkoutBuilder {
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

		[NoMacCatalyst]
		[NullAllowed, Export ("currentWorkoutActivity", ArgumentSemantic.Copy)]
		HKWorkoutActivity CurrentWorkoutActivity { get; }
	}

	[iOS (13, 0), Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKAudiogramSensitivityPoint : NSSecureCoding {
		[Export ("frequency", ArgumentSemantic.Copy)]
		HKQuantity Frequency { get; }

		[Deprecated (PlatformName.iOS, 18, 1)]
		[Deprecated (PlatformName.MacCatalyst, 18, 1)]
		[Deprecated (PlatformName.MacOSX, 15, 1)]
		[NullAllowed, Export ("leftEarSensitivity", ArgumentSemantic.Copy)]
		HKQuantity LeftEarSensitivity { get; }

		[Deprecated (PlatformName.iOS, 18, 1)]
		[Deprecated (PlatformName.MacCatalyst, 18, 1)]
		[Deprecated (PlatformName.MacOSX, 15, 1)]
		[NullAllowed, Export ("rightEarSensitivity", ArgumentSemantic.Copy)]
		HKQuantity RightEarSensitivity { get; }

		[Deprecated (PlatformName.iOS, 18, 1, message: "Use the 'HKAudiogramSensitivityTest' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 1, message: "Use the 'HKAudiogramSensitivityTest' overload instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 1, message: "Use the 'HKAudiogramSensitivityTest' overload instead.")]
		[Static]
		[Export ("sensitivityPointWithFrequency:leftEarSensitivity:rightEarSensitivity:error:")]
		[return: NullAllowed]
		HKAudiogramSensitivityPoint GetSensitivityPoint (HKQuantity frequency, [NullAllowed] HKQuantity leftEarSensitivity, [NullAllowed] HKQuantity rightEarSensitivity, [NullAllowed] out NSError error);

		[MacCatalyst (18, 1), Mac (15, 1), iOS (18, 1)]
		[Static]
		[Export ("sensitivityPointWithFrequency:tests:error:")]
		[return: NullAllowed]
		HKAudiogramSensitivityPoint GetSensitivityPoint (HKQuantity frequency, HKAudiogramSensitivityTest [] tests, [NullAllowed] out NSError error);

		[MacCatalyst (18, 1), Mac (15, 1), iOS (18, 1)]
		[Export ("tests", ArgumentSemantic.Copy)]
		HKAudiogramSensitivityTest [] Tests { get; }
	}

	[iOS (13, 0), Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor]
	interface HKAudiogramSample {
		[Export ("sensitivityPoints", ArgumentSemantic.Copy)]
		HKAudiogramSensitivityPoint [] SensitivityPoints { get; }

		[Deprecated (PlatformName.iOS, 18, 1, message: "Use the 'HKDevice' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 1, message: "Use the 'HKDevice' overload instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 1, message: "Use the 'HKDevice' overload instead.")]
		[Static]
		[Export ("audiogramSampleWithSensitivityPoints:startDate:endDate:metadata:")]
		HKAudiogramSample GetAudiogramSample (HKAudiogramSensitivityPoint [] sensitivityPoints, NSDate startDate, NSDate endDate, [NullAllowed] NSDictionary<NSString, NSObject> metadata);

		[MacCatalyst (18, 1), Mac (15, 1), iOS (18, 1)]
		[Static]
		[Export ("audiogramSampleWithSensitivityPoints:startDate:endDate:device:metadata:")]
		HKAudiogramSample GetAudiogramSample (HKAudiogramSensitivityPoint [] sensitivityPoints, NSDate startDate, NSDate endDate, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary<NSString, NSObject> metadata);
	}

	[iOS (13, 0), Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuantitySample))]
	[DisableDefaultCtor]
	interface HKDiscreteQuantitySample {
		[Export ("minimumQuantity", ArgumentSemantic.Copy)]
		HKQuantity Minimum { get; }

		[Export ("averageQuantity", ArgumentSemantic.Copy)]
		HKQuantity Average { get; }

		[Export ("maximumQuantity", ArgumentSemantic.Copy)]
		HKQuantity Maximum { get; }

		[Export ("mostRecentQuantity", ArgumentSemantic.Copy)]
		HKQuantity MostRecent { get; }

		[Export ("mostRecentQuantityDateInterval", ArgumentSemantic.Copy)]
		NSDateInterval MostRecentDateInterval { get; }
	}

	[iOS (13, 0)]
	[Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSeriesSample))]
	[DisableDefaultCtor]
	interface HKHeartbeatSeriesSample : NSSecureCoding { }

	delegate void HKHeartbeatSeriesBuilderCompletionHandler (bool success, NSError error);

	[iOS (13, 0), Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKSeriesBuilder))]
	[DisableDefaultCtor]
	interface HKHeartbeatSeriesBuilder {
		[Static]
		[Export ("maximumCount")]
		nuint MaximumCount { get; }

		[Export ("initWithHealthStore:device:startDate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (HKHealthStore healthStore, [NullAllowed] HKDevice device, NSDate startDate);

		[Export ("addHeartbeatWithTimeIntervalSinceSeriesStartDate:precededByGap:completion:")]
		[Async]
		void AddHeartbeat (double timeInterval, bool precededByGap, HKHeartbeatSeriesBuilderCompletionHandler completion);

		[Export ("addMetadata:completion:")]
		[Async]
		void AddMetadata (NSDictionary<NSString, NSObject> metadata, HKHeartbeatSeriesBuilderCompletionHandler completion);

		[Export ("finishSeriesWithCompletion:")]
		[Async]
		void FinishSeries (Action<HKHeartbeatSeriesSample, NSError> completion);
	}

	delegate void HKHeartbeatSeriesQueryDataHandler (HKHeartbeatSeriesQuery query, double timeSinceSeriesStart, bool precededByGap, bool done, NSError error);

	[iOS (13, 0), Mac (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (HKQuery))]
	interface HKHeartbeatSeriesQuery {
		[Export ("initWithHeartbeatSeries:dataHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (HKHeartbeatSeriesSample heartbeatSeries, HKHeartbeatSeriesQueryDataHandler dataHandler);
	}

	[iOS (14, 0), Mac (13, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKElectrocardiogram
	interface HKElectrocardiogram {
		[Export ("numberOfVoltageMeasurements")]
		nint NumberOfVoltageMeasurements { get; }

		[NullAllowed, Export ("samplingFrequency", ArgumentSemantic.Copy)]
		HKQuantity SamplingFrequency { get; }

		[Export ("classification", ArgumentSemantic.Assign)]
		HKElectrocardiogramClassification Classification { get; }

		[NullAllowed, Export ("averageHeartRate", ArgumentSemantic.Copy)]
		HKQuantity AverageHeartRate { get; }

		[Export ("symptomsStatus", ArgumentSemantic.Assign)]
		HKElectrocardiogramSymptomsStatus SymptomsStatus { get; }
	}

	delegate void HKElectrocardiogramQueryDataHandler (HKElectrocardiogramQuery query, HKElectrocardiogramVoltageMeasurement voltageMeasurement, bool done, NSError error);

	[iOS (14, 0), Mac (13, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor]
	interface HKElectrocardiogramQuery {

		[Export ("initWithElectrocardiogram:dataHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (HKElectrocardiogram electrocardiogram, HKElectrocardiogramQueryDataHandler dataHandler);
	}

	[iOS (14, 0), Mac (13, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKElectrocardiogramVoltageMeasurement : NSCopying {
		[Export ("timeSinceSampleStart")]
		double TimeSinceSampleStart { get; }

		[Export ("quantityForLead:")]
		[return: NullAllowed]
		HKQuantity GetQuantity (HKElectrocardiogramLead lead);
	}

	[iOS (14, 0), Mac (13, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "HKFHIRVersion")]
	[DisableDefaultCtor]
	interface HKFhirVersion : NSCopying, NSSecureCoding {
		[Export ("majorVersion")]
		nint MajorVersion { get; }

		[Export ("minorVersion")]
		nint MinorVersion { get; }

		[Export ("patchVersion")]
		nint PatchVersion { get; }

		[Export ("FHIRRelease", ArgumentSemantic.Strong)]
		string FhirRelease { get; }

		[Export ("stringRepresentation")]
		string StringRepresentation { get; }

		[Static]
		[Export ("versionFromVersionString:error:")]
		[return: NullAllowed]
		HKFhirVersion GetVersion (string versionString, [NullAllowed] out NSError errorOut);

		[Static]
		[Export ("primaryDSTU2Version")]
		HKFhirVersion PrimaryDstu2Version { get; }

		[Static]
		[Export ("primaryR4Version")]
		HKFhirVersion PrimaryR4Version { get; }
	}

	[iOS (14, 0), Mac (13, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKActivityMoveModeObject : NSCopying, NSSecureCoding {

		[Export ("activityMoveMode")]
		HKActivityMoveMode ActivityMoveMode { get; }
	}

	[iOS (14, 3), Mac (13, 0)]
	[MacCatalyst (14, 3)]
	[Native]
	enum HKCategoryValueContraceptive : long {
		Unspecified = 1,
		Implant,
		Injection,
		IntrauterineDevice,
		IntravaginalRing,
		Oral,
		Patch,
	}

	[iOS (14, 3), Mac (13, 0)]
	[MacCatalyst (14, 3)]
	[Native]
	enum HKCategoryValueLowCardioFitnessEvent : long {
		LowFitness = 1,
	}

	[iOS (15, 0), Mac (13, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKQueryDescriptor : NSCopying, NSSecureCoding {
		[Export ("sampleType", ArgumentSemantic.Copy)]
		HKSampleType SampleType { get; }

		[NullAllowed, Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; }

		[Export ("initWithSampleType:predicate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (HKSampleType sampleType, [NullAllowed] NSPredicate predicate);
	}

	[iOS (15, 0), Mac (13, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor]
	interface HKVerifiableClinicalRecord {
		[Export ("recordTypes", ArgumentSemantic.Copy)]
		string [] RecordTypes { get; }

		[Export ("issuerIdentifier")]
		string IssuerIdentifier { get; }

		[Export ("subject", ArgumentSemantic.Copy)]
		HKVerifiableClinicalRecordSubject Subject { get; }

		[Export ("issuedDate", ArgumentSemantic.Copy)]
		NSDate IssuedDate { get; }

		[Export ("relevantDate", ArgumentSemantic.Copy)]
		NSDate RelevantDate { get; }

		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; }

		[Export ("itemNames", ArgumentSemantic.Copy)]
		string [] ItemNames { get; }

		[NullAllowed, iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("sourceType")]
		string SourceType { get; }

		[iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("dataRepresentation", ArgumentSemantic.Copy)]
		NSData DataRepresentation { get; }

		[Deprecated (PlatformName.iOS, 15, 4)]
		[Deprecated (PlatformName.MacCatalyst, 15, 4)]
		[Export ("JWSRepresentation", ArgumentSemantic.Copy)]
		NSData JwsRepresentation { get; }
	}

	delegate void HKVerifiableClinicalRecordQueryResultHandler (HKVerifiableClinicalRecordQuery query, NSArray<HKVerifiableClinicalRecord> records, NSError error);

	[iOS (15, 0), Mac (13, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor]
	interface HKVerifiableClinicalRecordQuery {
		[Export ("recordTypes", ArgumentSemantic.Copy)]
		string [] RecordTypes { get; }

		[iOS (15, 4), MacCatalyst (15, 4)]
		[BindAs (typeof (HKVerifiableClinicalRecordSourceType []))]
		[Export ("sourceTypes", ArgumentSemantic.Copy)]
		NSString [] SourceTypes { get; }

		[Export ("initWithRecordTypes:predicate:resultsHandler:")]
		NativeHandle Constructor (string [] recordTypes, [NullAllowed] NSPredicate predicate, HKVerifiableClinicalRecordQueryResultHandler handler);

		[iOS (15, 4)]
		[MacCatalyst (15, 4)]
		[Export ("initWithRecordTypes:sourceTypes:predicate:resultsHandler:")]
#pragma warning disable 8632 // warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
		IntPtr Constructor (string [] recordTypes, [BindAs (typeof (HKVerifiableClinicalRecordSourceType []))] NSString [] sourceTypes, [NullAllowed] NSPredicate predicate, Action<HKVerifiableClinicalRecordQuery, HKVerifiableClinicalRecord []?, NSError?> resultsHandler);
#pragma warning restore
	}

	[iOS (15, 0), Mac (13, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKVerifiableClinicalRecordSubject : NSSecureCoding, NSCopying {
		[Export ("fullName")]
		string FullName { get; }

		[NullAllowed, Export ("dateOfBirthComponents", ArgumentSemantic.Copy)]
		NSDateComponents DateOfBirthComponents { get; }
	}

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKAttachment : NSSecureCoding, NSCopying {
		[Export ("identifier", ArgumentSemantic.Copy)]
		NSUuid Identifier { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("contentType", ArgumentSemantic.Copy)]
		UTType ContentType { get; }

		[Export ("size")]
		nint Size { get; }

		[Export ("creationDate", ArgumentSemantic.Copy)]
		NSDate CreationDate { get; }

		[NullAllowed, Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Metadata { get; }
	}

	delegate void HKAttachmentStoreCompletionHandler (bool success, NSError error);
	delegate void HKAttachmentStoreDataHandler ([NullAllowed] NSData dataChunk, [NullAllowed] NSError error, bool done);
	delegate void HKAttachmentStoreGetAttachmentCompletionHandler ([NullAllowed] HKAttachment [] attachments, [NullAllowed] NSError error);

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (NSObject))]
	interface HKAttachmentStore {
		[Export ("initWithHealthStore:")]
		NativeHandle Constructor (HKHealthStore healthStore);

		[Async]
		[Export ("addAttachmentToObject:name:contentType:URL:metadata:completion:")]
		void AddAttachment (HKObject @object, string name, UTType contentType, NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> metadata, Action<HKAttachment, NSError> completion);

		[Async]
		[Export ("removeAttachment:fromObject:completion:")]
		void RemoveAttachment (HKAttachment attachment, HKObject @object, HKAttachmentStoreCompletionHandler completion);

		[Async]
		[Export ("getAttachmentsForObject:completion:")]
		void GetAttachments (HKObject @object, HKAttachmentStoreGetAttachmentCompletionHandler completion);

		[Async]
		[Export ("getDataForAttachment:completion:")]
		NSProgress GetData (HKAttachment attachment, Action<NSData, NSError> completion);

		[Export ("streamDataForAttachment:dataHandler:")]
		NSProgress StreamData (HKAttachment attachment, HKAttachmentStoreDataHandler dataHandler);
	}

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (HKLensSpecification))]
	[DisableDefaultCtor]
	interface HKContactsLensSpecification : NSSecureCoding, NSCopying {
		[Export ("initWithSphere:cylinder:axis:addPower:baseCurve:diameter:")]
		NativeHandle Constructor (HKQuantity sphere, [NullAllowed] HKQuantity cylinder, [NullAllowed] HKQuantity axis, [NullAllowed] HKQuantity addPower, [NullAllowed] HKQuantity baseCurve, [NullAllowed] HKQuantity diameter);

		[NullAllowed, Export ("baseCurve", ArgumentSemantic.Copy)]
		HKQuantity BaseCurve { get; }

		[NullAllowed, Export ("diameter", ArgumentSemantic.Copy)]
		HKQuantity Diameter { get; }
	}

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (HKVisionPrescription))]
	[DisableDefaultCtor]
	interface HKContactsPrescription : NSSecureCoding, NSCopying {
		[NullAllowed, Export ("rightEye", ArgumentSemantic.Copy)]
		HKContactsLensSpecification RightEye { get; }

		[NullAllowed, Export ("leftEye", ArgumentSemantic.Copy)]
		HKContactsLensSpecification LeftEye { get; }

		[Export ("brand")]
		string Brand { get; }

		[Static]
		[Export ("prescriptionWithRightEyeSpecification:leftEyeSpecification:brand:dateIssued:expirationDate:device:metadata:")]
		HKContactsPrescription GetPrescription ([NullAllowed] HKContactsLensSpecification rightEyeSpecification, [NullAllowed] HKContactsLensSpecification leftEyeSpecification, string brand, NSDate dateIssued, [NullAllowed] NSDate expirationDate, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary<NSString, NSObject> metadata);
	}

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (HKLensSpecification))]
	[DisableDefaultCtor]
	interface HKGlassesLensSpecification : NSSecureCoding, NSCopying {
		[Export ("initWithSphere:cylinder:axis:addPower:vertexDistance:prism:farPupillaryDistance:nearPupillaryDistance:")]
		NativeHandle Constructor (HKQuantity sphere, [NullAllowed] HKQuantity cylinder, [NullAllowed] HKQuantity axis, [NullAllowed] HKQuantity addPower, [NullAllowed] HKQuantity vertexDistance, [NullAllowed] HKVisionPrism prism, [NullAllowed] HKQuantity farPupillaryDistance, [NullAllowed] HKQuantity nearPupillaryDistance);

		[NullAllowed, Export ("vertexDistance", ArgumentSemantic.Copy)]
		HKQuantity VertexDistance { get; }

		[NullAllowed, Export ("prism", ArgumentSemantic.Copy)]
		HKVisionPrism Prism { get; }

		[NullAllowed, Export ("farPupillaryDistance", ArgumentSemantic.Copy)]
		HKQuantity FarPupillaryDistance { get; }

		[NullAllowed, Export ("nearPupillaryDistance", ArgumentSemantic.Copy)]
		HKQuantity NearPupillaryDistance { get; }
	}

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (HKVisionPrescription))]
	[DisableDefaultCtor]
	interface HKGlassesPrescription : NSSecureCoding, NSCopying {
		[NullAllowed, Export ("rightEye", ArgumentSemantic.Copy)]
		HKGlassesLensSpecification RightEye { get; }

		[NullAllowed, Export ("leftEye", ArgumentSemantic.Copy)]
		HKGlassesLensSpecification LeftEye { get; }

		[Static]
		[Export ("prescriptionWithRightEyeSpecification:leftEyeSpecification:dateIssued:expirationDate:device:metadata:")]
		HKGlassesPrescription GetPrescription ([NullAllowed] HKGlassesLensSpecification rightEyeSpecification, [NullAllowed] HKGlassesLensSpecification leftEyeSpecification, NSDate dateIssued, [NullAllowed] NSDate expirationDate, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary<NSString, NSObject> metadata);
	}

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKLensSpecification {
		[Export ("sphere", ArgumentSemantic.Copy)]
		HKQuantity Sphere { get; }

		[NullAllowed, Export ("cylinder", ArgumentSemantic.Copy)]
		HKQuantity Cylinder { get; }

		[NullAllowed, Export ("axis", ArgumentSemantic.Copy)]
		HKQuantity Axis { get; }

		[NullAllowed, Export ("addPower", ArgumentSemantic.Copy)]
		HKQuantity AddPower { get; }
	}

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor]
	interface HKVisionPrescription : NSSecureCoding, NSCopying {
		[Export ("prescriptionType", ArgumentSemantic.Assign)]
		HKVisionPrescriptionType PrescriptionType { get; }

		[Export ("dateIssued", ArgumentSemantic.Copy)]
		NSDate DateIssued { get; }

		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; }

		[Static]
		[Export ("prescriptionWithType:dateIssued:expirationDate:device:metadata:")]
		HKVisionPrescription GetPrescription (HKVisionPrescriptionType type, NSDate dateIssued, [NullAllowed] NSDate expirationDate, [NullAllowed] HKDevice device, [NullAllowed] NSDictionary<NSString, NSObject> metadata);

		[iOS (16, 0), Mac (13, 0), NoTV, MacCatalyst (16, 0)]
		[Field ("HKVisionPrescriptionTypeIdentifier")]
		NSString TypeIdentifier { get; }
	}

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKVisionPrism : NSSecureCoding, NSCopying {
		[Export ("initWithAmount:angle:eye:")]
		NativeHandle Constructor (HKQuantity amount, HKQuantity angle, HKVisionEye eye);

		[Export ("initWithVerticalAmount:verticalBase:horizontalAmount:horizontalBase:eye:")]
		NativeHandle Constructor (HKQuantity verticalAmount, HKPrismBase verticalBase, HKQuantity horizontalAmount, HKPrismBase horizontalBase, HKVisionEye eye);

		[Export ("amount", ArgumentSemantic.Copy)]
		HKQuantity Amount { get; }

		[Export ("angle", ArgumentSemantic.Copy)]
		HKQuantity Angle { get; }

		[Export ("verticalAmount", ArgumentSemantic.Copy)]
		HKQuantity VerticalAmount { get; }

		[Export ("horizontalAmount", ArgumentSemantic.Copy)]
		HKQuantity HorizontalAmount { get; }

		[Export ("verticalBase")]
		HKPrismBase VerticalBase { get; }

		[Export ("horizontalBase")]
		HKPrismBase HorizontalBase { get; }

		[Export ("eye", ArgumentSemantic.Assign)]
		HKVisionEye Eye { get; }
	}

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKWorkoutActivity : NSSecureCoding, NSCopying {
		[Export ("initWithWorkoutConfiguration:startDate:endDate:metadata:")]
		NativeHandle Constructor (HKWorkoutConfiguration workoutConfiguration, NSDate startDate, [NullAllowed] NSDate endDate, [NullAllowed] NSDictionary<NSString, NSObject> metadata);

		[Export ("UUID", ArgumentSemantic.Copy)]
		NSUuid Uuid { get; }

		[Export ("workoutConfiguration", ArgumentSemantic.Copy)]
		HKWorkoutConfiguration WorkoutConfiguration { get; }

		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		[NullAllowed, Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; }

		[NullAllowed, Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Metadata { get; }

		[Export ("duration")]
		double Duration { get; }

		[Export ("workoutEvents", ArgumentSemantic.Copy)]
		HKWorkoutEvent [] WorkoutEvents { get; }

		[Export ("allStatistics", ArgumentSemantic.Copy)]
		NSDictionary<HKQuantityType, HKStatistics> AllStatistics { get; }

		[Export ("statisticsForType:")]
		[return: NullAllowed]
		HKStatistics GetStatistics (HKQuantityType quantityType);
	}

	[MacCatalyst (16, 0), Mac (13, 0), iOS (16, 0), NoTV]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: The -init method is not available on HKPrescriptionType
	interface HKPrescriptionType {
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (HKScoredAssessment), Name = "HKGAD7Assessment")]
	[DisableDefaultCtor]
	interface HKGad7Assessment {
		[Export ("answers", ArgumentSemantic.Copy)]
		[BindAs (typeof (HKGad7AssessmentAnswer []))]
		NSNumber [] Answers { get; }

		[Export ("risk", ArgumentSemantic.Assign)]
		HKGad7AssessmentRisk Risk { get; }

		[Static]
		[Export ("assessmentWithDate:answers:")]
		HKGad7Assessment Create (NSDate date, [BindAs (typeof (HKGad7AssessmentAnswer []))] NSNumber [] answers);

		[Static]
		[Export ("assessmentWithDate:answers:metadata:")]
		HKGad7Assessment Create (NSDate date, [BindAs (typeof (HKGad7AssessmentAnswer []))] NSNumber [] answers, [NullAllowed] NSDictionary<NSString, NSObject> metadata);
	}

	delegate void HKWorkoutRelationshipCallback (bool success, [NullAllowed] NSError error);

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Category]
	[BaseType (typeof (HKHealthStore))]
	interface HKHealthStore_HKWorkoutRelationship {
		[Export ("relateWorkoutEffortSample:withWorkout:activity:completion:")]
		void RelateWorkoutEffortSample (HKSample sample, HKWorkout workout, [NullAllowed] HKWorkoutActivity activity, HKWorkoutRelationshipCallback completion);

		[Export ("unrelateWorkoutEffortSample:fromWorkout:activity:completion:")]
		void UnrelateWorkoutEffortSample (HKSample sample, HKWorkout workout, [NullAllowed] HKWorkoutActivity activity, HKWorkoutRelationshipCallback completion);
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor]
	interface HKScoredAssessmentType {
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (HKSampleType))]
	[DisableDefaultCtor]
	interface HKStateOfMindType {
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	[NativeName ("HKPHQ9AssessmentRisk")]
	public enum HKPhq9AssessmentRisk : long {
		NoneToMinimal = 1,
		Mild,
		Moderate,
		ModeratelySevere,
		Severe,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	[NativeName ("HKPHQ9AssessmentAnswer")]
	public enum HKPhq9AssessmentAnswer : long {
		NotAtAll = 0,
		SeveralDays,
		MoreThanHalfTheDays,
		NearlyEveryDay,
		PreferNotToAnswer,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (HKScoredAssessment), Name = "HKPHQ9Assessment")]
	[DisableDefaultCtor]
	interface HKPhq9Assessment {
		[Export ("answers", ArgumentSemantic.Copy)]
		[BindAs (typeof (HKPhq9AssessmentAnswer []))]
		NSNumber [] Answers { get; }

		[Export ("risk", ArgumentSemantic.Assign)]
		HKPhq9AssessmentRisk Risk { get; }

		[Static]
		[Export ("assessmentWithDate:answers:")]
		HKPhq9Assessment Create (NSDate date, [BindAs (typeof (HKPhq9AssessmentAnswer []))] NSNumber [] answers);

		[Static]
		[Export ("assessmentWithDate:answers:metadata:")]
		HKPhq9Assessment Create (NSDate date, [BindAs (typeof (HKPhq9AssessmentAnswer []))] NSNumber [] answers, [NullAllowed] NSDictionary<NSString, NSObject> metadata);
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor]
	interface HKScoredAssessment : NSSecureCoding, NSCopying {
		[Export ("score", ArgumentSemantic.Assign)]
		nint Score { get; }
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HKStateOfMindValenceClassification : long {
		VeryUnpleasant = 1,
		Unpleasant,
		SlightlyUnpleasant,
		Neutral,
		SlightlyPleasant,
		Pleasant,
		VeryPleasant,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HKStateOfMindLabel : long {
		Amazed = 1,
		Amused,
		Angry,
		Anxious,
		Ashamed,
		Brave,
		Calm,
		Content,
		Disappointed,
		Discouraged,
		Disgusted,
		Embarrassed,
		Excited,
		Frustrated,
		Grateful,
		Guilty,
		Happy,
		Hopeless,
		Irritated,
		Jealous,
		Joyful,
		Lonely,
		Passionate,
		Peaceful,
		Proud,
		Relieved,
		Sad,
		Scared,
		Stressed,
		Surprised,
		Worried,
		Annoyed,
		Confident,
		Drained,
		Hopeful,
		Indifferent,
		Overwhelmed,
		Satisfied,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HKStateOfMindAssociation : long {
		Community = 1,
		CurrentEvents,
		Dating,
		Education,
		Family,
		Fitness,
		Friends,
		Health,
		Hobbies,
		Identity,
		Money,
		Partner,
		SelfCare,
		Spirituality,
		Tasks,
		Travel,
		Work,
		Weather,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HKStateOfMindKind : long {
		MomentaryEmotion = 1,
		DailyMood = 2,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (HKSample))]
	[DisableDefaultCtor]
	interface HKStateOfMind : NSSecureCoding, NSCopying {
		[Export ("kind", ArgumentSemantic.Assign)]
		HKStateOfMindKind Kind { get; }

		[Export ("valence", ArgumentSemantic.Assign)]
		double Valence { get; }

		[Export ("valenceClassification", ArgumentSemantic.Assign)]
		HKStateOfMindValenceClassification ValenceClassification { get; }

		[Export ("labels", ArgumentSemantic.Copy)]
		[BindAs (typeof (HKStateOfMindLabel []))]
		NSNumber [] Labels { get; }

		[Export ("associations", ArgumentSemantic.Copy)]
		[BindAs (typeof (HKStateOfMindAssociation []))]
		NSNumber [] Associations { get; }

		[Static]
		[Export ("stateOfMindWithDate:kind:valence:labels:associations:")]
		HKStateOfMind Create (NSDate date, HKStateOfMindKind kind, double valence, [BindAs (typeof (HKStateOfMindLabel []))] NSNumber [] labels, [BindAs (typeof (HKStateOfMindAssociation []))] NSNumber [] associations);

		[Static]
		[Export ("stateOfMindWithDate:kind:valence:labels:associations:metadata:")]
		HKStateOfMind Create (NSDate date, HKStateOfMindKind kind, double valence, [BindAs (typeof (HKStateOfMindLabel []))] NSNumber [] labels, [BindAs (typeof (HKStateOfMindAssociation []))] NSNumber [] associations, [NullAllowed] NSDictionary<NSString, NSObject> metadata);
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	enum HKScoredAssessmentTypeIdentifier {
		[Field ("HKScoredAssessmentTypeIdentifierGAD7")]
		Gad7,
		[Field ("HKScoredAssessmentTypeIdentifierPHQ9")]
		Phq9,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKWorkoutEffortRelationship : NSSecureCoding, NSCopying {
		[Export ("workout", ArgumentSemantic.Copy)]
		HKWorkout Workout { get; }

		[Export ("activity", ArgumentSemantic.Copy), NullAllowed]
		HKWorkoutActivity Activity { get; }

		[Export ("samples", ArgumentSemantic.Copy), NullAllowed]
		HKSample [] Samples { get; }
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HKWorkoutEffortRelationshipQueryOptions : long {
		Default = 0,
		MostRelevant = 1 << 0,
	}

	delegate void HKWorkoutEffortRelationshipQueryResultsHandler (HKWorkoutEffortRelationshipQuery query, [NullAllowed] HKWorkoutEffortRelationship [] relationships, [NullAllowed] HKQueryAnchor newAnchor, [NullAllowed] NSError error);

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (HKQuery))]
	[DisableDefaultCtor]
	interface HKWorkoutEffortRelationshipQuery {
		[Export ("initWithPredicate:anchor:options:resultsHandler:")]
		NativeHandle Constructor ([NullAllowed] NSPredicate predicate, [NullAllowed] HKQueryAnchor anchor, HKWorkoutEffortRelationshipQueryOptions options, HKWorkoutEffortRelationshipQueryResultsHandler resultsHandler);
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HKAppleSleepingBreathingDisturbancesClassification : long {
		NotElevated,
		Elevated,
	}

	[MacCatalyst (18, 1), Mac (15, 1), iOS (18, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKAudiogramSensitivityPointClampingRange : NSSecureCoding, NSCopying {

		[NullAllowed, Export ("lowerBound", ArgumentSemantic.Copy)]
		HKQuantity LowerBound { get; }

		[NullAllowed, Export ("upperBound", ArgumentSemantic.Copy)]
		HKQuantity UpperBound { get; }

		[Static]
		[Export ("clampingRangeWithLowerBound:upperBound:error:")]
		[return: NullAllowed]
		HKAudiogramSensitivityPointClampingRange Create ([NullAllowed][BindAs (typeof (double?))] NSNumber lowerBound, [NullAllowed][BindAs (typeof (double?))] NSNumber upperBound, [NullAllowed] out NSError error);
	}

	[MacCatalyst (18, 1), Mac (15, 1), iOS (18, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface HKAudiogramSensitivityTest : NSSecureCoding, NSCopying {

		[Export ("sensitivity", ArgumentSemantic.Copy)]
		HKQuantity Sensitivity { get; }

		[Export ("type", ArgumentSemantic.Assign)]
		HKAudiogramConductionType Type { get; }

		[Export ("masked")]
		bool Masked { get; }

		[Export ("side", ArgumentSemantic.Assign)]
		HKAudiogramSensitivityTestSide Side { get; }

		[NullAllowed, Export ("clampingRange", ArgumentSemantic.Copy)]
		HKAudiogramSensitivityPointClampingRange ClampingRange { get; }

		[Export ("initWithSensitivity:type:masked:side:clampingRange:error:")]
		NativeHandle Constructor (HKQuantity sensitivity, HKAudiogramConductionType type, bool masked, HKAudiogramSensitivityTestSide side, [NullAllowed] HKAudiogramSensitivityPointClampingRange clampingRange, [NullAllowed] out NSError error);
	}

}
