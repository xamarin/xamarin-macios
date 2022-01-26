using CoreFoundation;
using ObjCRuntime;
using Foundation;
using System;

namespace HealthKit
{
	// NSInteger -> HKDefines.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKUpdateFrequency : long {
		Immediate = 1,
		Hourly,
		Daily,
		Weekly
	}

	// NSInteger -> HKDefines.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKAuthorizationStatus : long {
		NotDetermined = 0,
		SharingDenied,
		SharingAuthorized
	}

	// NSInteger -> HKDefines.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKBiologicalSex : long {
		NotSet = 0,
		Female,
		Male,
#if NET
		[SupportedOSPlatform ("ios8.2")]
#else
		[iOS (8,2)]
#endif
		Other
	}

	// NSInteger -> HKDefines.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKBloodType : long {
		NotSet = 0,
		APositive,
		ANegative,
		BPositive,
		BNegative,
		ABPositive,
		ABNegative,
		OPositive,
		ONegative
	}

	// NSInteger -> HKMetadata.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKBodyTemperatureSensorLocation : long {
		Other = 0,
		Armpit,
		Body,
		Ear,
		Finger,
		GastroIntestinal,
		Mouth,
		Rectum,
		Toe,
		EarDrum,
		TemporalArtery,
		Forehead
	}

	// NSInteger -> HKMetadata.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKHeartRateSensorLocation : long {
		Other = 0,
		Chest,
		Wrist,
		Finger,
		Hand,
		EarLobe,
		Foot
	}

	// NSInteger -> HKObjectType.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKQuantityAggregationStyle : long {
		Cumulative = 0,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		DiscreteArithmetic,
#if NET
		[SupportedOSPlatform ("ios8.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if IOS
		[Obsolete ("Starting with ios13.0 use 'HKQuantityAggregationStyle.DiscreteArithmetic'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'HKQuantityAggregationStyle.DiscreteArithmetic'.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'HKQuantityAggregationStyle.DiscreteArithmetic'.")]
#endif
		Discrete = DiscreteArithmetic,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		DiscreteTemporallyWeighted,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		DiscreteEquivalentContinuousLevel,
	}

	// NSInteger -> HKObjectType.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKCategoryValueSleepAnalysis : long {
		InBed,
		Asleep,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[Watch (3,0)]
		[iOS (10,0)]
#endif
		Awake,
	}

	// NSUInteger -> HKQuery.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	[Flags]
	public enum HKQueryOptions : ulong {
		None               = 0,
		StrictStartDate    = 1 << 0,
		StrictEndDate      = 1 << 1
	}

	// NSUInteger -> HKStatistics.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	[Flags]
	public enum HKStatisticsOptions : ulong {
		None              	  = 0,
		SeparateBySource          = 1 << 0,
		DiscreteAverage           = 1 << 1,
		DiscreteMin               = 1 << 2,
		DiscreteMax               = 1 << 3,
		CumulativeSum             = 1 << 4,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		MostRecent                = 1 << 5,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		Duration                  = 1 << 6,
	}

	// NSInteger -> HKUnit.h
#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKMetricPrefix : long {
		None = 0,
		Pico,
		Nano,
		Micro,
		Milli,
		Centi,
		Deci,
		Deca,
		Hecto,
		Kilo,
		Mega,
		Giga,
		Tera,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		Femto,
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKWorkoutActivityType : ulong {
		AmericanFootball = 1,
		Archery,
		AustralianFootball,
		Badminton,
		Baseball,
		Basketball,
		Bowling,
		Boxing,
		Climbing,
		Cricket,
		CrossTraining,
		Curling,
		Cycling,
		Dance,
#if NET
		[SupportedOSPlatform ("ios8.0")]
		[UnsupportedOSPlatform ("ios10.0")]
#if IOS
		[Obsolete ("Starting with ios10.0 use 'HKWorkoutActivityType.Dance', 'HKWorkoutActivityType.Barre', or 'HKWorkoutActivityType.Pilates'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'HKWorkoutActivityType.Dance', 'HKWorkoutActivityType.Barre', or 'HKWorkoutActivityType.Pilates'.")]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'HKWorkoutActivityType.Dance', 'HKWorkoutActivityType.Barre', or 'HKWorkoutActivityType.Pilates'.")]
#endif
		DanceInspiredTraining,
		Elliptical,
		EquestrianSports,
		Fencing,
		Fishing,
		FunctionalStrengthTraining,
		Golf,
		Gymnastics,
		Handball,
		Hiking,
		Hockey,
		Hunting,
		Lacrosse,
		MartialArts,
		MindAndBody,
#if NET
		[SupportedOSPlatform ("ios8.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if IOS
		[Obsolete ("Starting with ios11.0 use 'MixedCardio' or 'HighIntensityIntervalTraining' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'MixedCardio' or 'HighIntensityIntervalTraining' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'MixedCardio' or 'HighIntensityIntervalTraining' instead.")]
#endif
		MixedMetabolicCardioTraining,
		PaddleSports,
		Play,
		PreparationAndRecovery,
		Racquetball,
		Rowing,
		Rugby,
		Running,
		Sailing,
		SkatingSports,
		SnowSports,
		Soccer,
		Softball,
		Squash,
		StairClimbing,
		SurfingSports,
		Swimming,
		TableTennis,
		Tennis,
		TrackAndField,
		TraditionalStrengthTraining,
		Volleyball,
		Walking,
		WaterFitness,
		WaterPolo,
		WaterSports,
		Wrestling,
		Yoga,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		Barre,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		CoreTraining,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		CrossCountrySkiing,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		DownhillSkiing,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		Flexibility,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		HighIntensityIntervalTraining,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		JumpRope,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		Kickboxing,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		Pilates,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		Snowboarding,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		Stairs,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		StepTraining,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		WheelchairWalkPace,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		WheelchairRunPace,
#if NET
		[SupportedOSPlatform ("ios11.0")]
#else
		[iOS (11,0)]
		[Watch (4,0)]
#endif
		TaiChi,
#if NET
		[SupportedOSPlatform ("ios11.0")]
#else
		[iOS (11, 0)]
		[Watch (4, 0)]
#endif
		MixedCardio,
#if NET
		[SupportedOSPlatform ("ios11.0")]
#else
		[iOS (11, 0)]
		[Watch (4, 0)]
#endif
		HandCycling,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		DiscSports,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		FitnessGaming,
#if NET
		[SupportedOSPlatform ("ios14.0")]
#else
		[iOS (14,0)]
		[Watch (7,0)]
#endif
		CardioDance = 77,
#if NET
		[SupportedOSPlatform ("ios14.0")]
#else
		[iOS (14,0)]
		[Watch (7,0)]
#endif
		SocialDance = 78,
#if NET
		[SupportedOSPlatform ("ios14.0")]
#else
		[iOS (14,0)]
		[Watch (7,0)]
#endif
		Pickleball = 79,
#if NET
		[SupportedOSPlatform ("ios14.0")]
#else
		[iOS (14,0)]
		[Watch (7,0)]
#endif
		Cooldown = 80,
#if NET
		[SupportedOSPlatform ("ios8.2")]
#else
		[iOS (8,2)]
#endif
		Other = 3000
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (2,0)]
	[iOS (8,0)]
#endif
	[Native]
	public enum HKWorkoutEventType : long {
		Pause = 1,
		Resume,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		Lap,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		Marker,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		MotionPaused,
#if NET
		[SupportedOSPlatform ("ios10.0")]
#else
		[iOS (10,0)]
		[Watch (3,0)]
#endif
		MotionResumed,
#if NET
		[SupportedOSPlatform ("ios11.0")]
#else
		[iOS (11, 0)]
		[Watch (4, 0)]
#endif
		Segment,
#if NET
		[SupportedOSPlatform ("ios11.0")]
#else
		[iOS (11, 0)]
		[Watch (4, 0)]
#endif
		PauseOrResumeRequest,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[Watch (2,0)]
	[iOS (9,0)]
#endif
	[Native]
	public enum HKCategoryValue : long {
		NotApplicable = 0
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[Watch (2,0)]
	[iOS (9,0)]
#endif
	[Native]
	public enum HKCategoryValueCervicalMucusQuality : long {
		NotApplicable = 0,
		Dry = 1,
		Sticky,
		Creamy,
		Watery,
		EggWhite
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[Watch (2,0)]
	[iOS (9,0)]
#endif
	[Native]
	public enum HKCategoryValueMenstrualFlow : long {
		NotApplicable = 0,
		Unspecified = 1,
		Light,
		Medium,
		Heavy,
#if NET
		[SupportedOSPlatform ("ios12.0")]
#else
		[iOS (12,0)]
		[Watch (5,0)]
#endif
		None,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[Watch (2,0)]
	[iOS (9,0)]
#endif
	[Native]
	public enum HKCategoryValueOvulationTestResult : long {
		NotApplicable = 0,
		Negative = 1,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		LuteinizingHormoneSurge = 2,
#if NET
		[SupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if IOS
		[Obsolete ("Starting with ios13.0 use 'HKCategoryValueOvulationTestResult.LuteinizingHormoneSurge' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'HKCategoryValueOvulationTestResult.LuteinizingHormoneSurge' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'HKCategoryValueOvulationTestResult.LuteinizingHormoneSurge' instead.")]
#endif
		Positive = LuteinizingHormoneSurge,
		Indeterminate = 3,
#if NET
		[SupportedOSPlatform ("ios13.0")]
#else
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		EstrogenSurge = 4,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[Watch (2,0)]
	[iOS (9,0)]
#endif
	[Native]
	public enum HKCategoryValueAppleStandHour : long {
		Stood = 0,
		Idle
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
#else
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	[Native]
	public enum HKCategoryValueAudioExposureEvent : long {
		LoudEnvironment = 1,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#else
	[Watch (2,0)]
	[iOS (9,0)]
#endif
	[Native]
	public enum HKFitzpatrickSkinType : long {
		NotSet = 0,
		I,
		II,
		III,
		IV,
		V,
		VI
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[Watch (3,0)]
	[iOS (10,0)]
#endif
	[Native]
	public enum HKWheelchairUse : long {
		NotSet = 0,
		No,
		Yes,
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[Watch (3,0)]
	[iOS (10,0)]
#endif
	[Native]
	public enum HKWeatherCondition : long {
		None = 0,
		Clear,
		Fair,
		PartlyCloudy,
		MostlyCloudy,
		Cloudy,
		Foggy,
		Haze,
		Windy,
		Blustery,
		Smoky,
		Dust,
		Snow,
		Hail,
		Sleet,
		FreezingDrizzle,
		FreezingRain,
		MixedRainAndHail,
		MixedRainAndSnow,
		MixedRainAndSleet,
		MixedSnowAndSleet,
		Drizzle,
		ScatteredShowers,
		Showers,
		Thunderstorms,
		TropicalStorm,
		Hurricane,
		Tornado,
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[Watch (3,0)]
	[iOS (10,0)]
#endif
	[Native]
	public enum HKWorkoutSwimmingLocationType : long {
		Unknown = 0,
		Pool,
		OpenWater,
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
#else
	[Watch (3,0)]
	[iOS (10,0)]
#endif
	[Native]
	public enum HKSwimmingStrokeStyle : long {
		Unknown = 0,
		Mixed,
		Freestyle,
		Backstroke,
		Breaststroke,
		Butterfly,
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
#else
	[Watch (4, 0)]
	[iOS (11, 0)]
#endif
	[Native]
	public enum HKInsulinDeliveryReason : long {
		Basal = 1,
		Bolus,
#if !XAMCORE_4_0
		[Obsolete ("Use 'Basal' instead.")]
		Asal = Basal,
		[Obsolete ("Use 'Bolus' instead.")]
		Olus = Bolus,
#endif
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
#else
	[Watch (4, 0)]
	[iOS (11, 0)]
#endif
	[Native]
	public enum HKBloodGlucoseMealTime : long {
		Preprandial = 1,
		Postprandial,
#if !XAMCORE_4_0
		[Obsolete ("Use 'Preprandial' instead.")]
		Reprandial = Preprandial,
		[Obsolete ("Use 'Postprandial' instead.")]
		Ostprandial = Postprandial,
#endif
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
#else
	[Watch (4, 0)]
	[iOS (11, 0)]
#endif
	[Native]
	public enum HKVO2MaxTestType : long {
		MaxExercise = 1,
		PredictionSubMaxExercise,
		PredictionNonExercise,
	}
 
#if NET
	[SupportedOSPlatform ("ios12.0")]
#else
	[NoWatch]
	[iOS (12, 0)]
#endif
	public enum HKFhirResourceType {
		[Field ("HKFHIRResourceTypeAllergyIntolerance")]
		AllergyIntolerance,
		[Field ("HKFHIRResourceTypeCondition")]
		Condition,
		[Field ("HKFHIRResourceTypeImmunization")]
		Immunization,
		[Field ("HKFHIRResourceTypeMedicationDispense")]
		MedicationDispense,
		[Field ("HKFHIRResourceTypeMedicationOrder")]
		MedicationOrder,
		[Field ("HKFHIRResourceTypeMedicationStatement")]
		MedicationStatement,
		[Field ("HKFHIRResourceTypeObservation")]
		Observation,
		[Field ("HKFHIRResourceTypeProcedure")]
		Procedure,
#if NET
		[SupportedOSPlatform ("ios14.0")]
#else
		[iOS (14, 0)]
#endif
		[Field ("HKFHIRResourceTypeMedicationRequest")]
		MedicationRequest,
#if NET
		[SupportedOSPlatform ("ios14.0")]
#else
		[iOS (14, 0)]
#endif
		[Field ("HKFHIRResourceTypeCoverage")]
		Coverage,
	}

#if NET
	[SupportedOSPlatform ("ios12.0")]
#else
	[Watch (5, 0)]
	[iOS (12, 0)]
#endif
	public enum HKClinicalTypeIdentifier {

		[Field ("HKClinicalTypeIdentifierAllergyRecord")]
		AllergyRecord,
		[Field ("HKClinicalTypeIdentifierConditionRecord")]
		ConditionRecord,
		[Field ("HKClinicalTypeIdentifierImmunizationRecord")]
		ImmunizationRecord,
		[Field ("HKClinicalTypeIdentifierLabResultRecord")]
		LabResultRecord,
		[Field ("HKClinicalTypeIdentifierMedicationRecord")]
		MedicationRecord,
		[Field ("HKClinicalTypeIdentifierProcedureRecord")]
		ProcedureRecord,
		[Field ("HKClinicalTypeIdentifierVitalSignRecord")]
		VitalSignRecord,
#if NET
		[SupportedOSPlatform ("ios14.0")]
#else
		[Watch (7, 0)]
		[iOS (14, 0)]
#endif
		[Field ("HKClinicalTypeIdentifierCoverageRecord")]
		CoverageRecord,
	}

#if NET
	[SupportedOSPlatform ("ios12.0")]
#else
	[Watch (5,0)]
	[iOS (12,0)]
#endif
	[Native]
	public enum HKAuthorizationRequestStatus : long 
	{
		Unknown = 0,
		ShouldRequest,
		Unnecessary,
	}

#if NET
	[SupportedOSPlatform ("ios13.6")]
#else
	[Watch (7,0)]
	[iOS (13,6)]
#endif
	[Native]
	public enum HKCategoryValueAppetiteChanges : long {
		Unspecified = 0,
		NoChange,
		Decreased,
		Increased,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
#else
	[Watch (7,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum HKAppleEcgAlgorithmVersion : long {
		Version1 = 1,
		Version2 = 2,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
#else
	[Watch (7,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum HKCategoryValueEnvironmentalAudioExposureEvent : long {
		MomentaryLimit = 1,
	}

#if NET
	[SupportedOSPlatform ("ios13.6")]
#else
	[Watch (7,0)]
	[iOS (13,6)]
#endif
	[Native]
	public enum HKCategoryValuePresence : long {
		Present = 0,
		NotPresent,
	}

#if NET
	[SupportedOSPlatform ("ios13.6")]
#else
	[Watch (7,0)]
	[iOS (13,6)]
#endif
	[Native]
	public enum HKCategoryValueSeverity : long {
		Unspecified = 0,
		NotPresent,
		Mild,
		Moderate,
		Severe,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
#else
	[Watch (7,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum HKDevicePlacementSide : long {
		Unknown = 0,
		Left,
		Right,
		Central,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
#else
	[Watch (7,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum HKElectrocardiogramClassification : long {
		NotSet = 0,
		SinusRhythm,
		AtrialFibrillation,
		InconclusiveLowHeartRate,
		InconclusiveHighHeartRate,
		InconclusivePoorReading,
		InconclusiveOther,
		Unrecognized = 100,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
#else
	[Watch (7,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum HKElectrocardiogramLead : long {
		AppleWatchSimilarToLeadI = 1,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
#else
	[Watch (7,0)]
	[iOS (14,0)]
#endif
	[Native]
	public enum HKElectrocardiogramSymptomsStatus : long {
		NotSet = 0,
		None = 1,
		Present = 2,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
#else
	[NoWatch]
	[iOS (14, 0)]
#endif
	public enum HKFhirRelease {
		[Field ("HKFHIRReleaseDSTU2")]
		Dstu2,
		[Field ("HKFHIRReleaseR4")]
		R4,
		[Field ("HKFHIRReleaseUnknown")]
		Unknown,
	}
}
