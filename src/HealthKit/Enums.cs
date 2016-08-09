using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using System;

namespace XamCore.HealthKit
{
	// NSInteger -> HKDefines.h
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	public enum HKUpdateFrequency : nint {
		Immediate = 1,
		Hourly,
		Daily,
		Weekly
	}

	// NSInteger -> HKDefines.h
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	public enum HKAuthorizationStatus : nint {
		NotDetermined = 0,
		SharingDenied,
		SharingAuthorized
	}

	// NSInteger -> HKDefines.h
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	public enum HKBiologicalSex : nint {
		NotSet = 0,
		Female,
		Male,
		[iOS (8,2)]
		Other
	}

	// NSInteger -> HKDefines.h
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	public enum HKBloodType : nint {
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
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	public enum HKBodyTemperatureSensorLocation : nint {
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
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	public enum HKHeartRateSensorLocation : nint {
		Other = 0,
		Chest,
		Wrist,
		Finger,
		Hand,
		EarLobe,
		Foot
	}

	// NSInteger -> HKObjectType.h
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	public enum HKQuantityAggregationStyle : nint {
		Cumulative = 0,
		Discrete
	}

	// NSInteger -> HKObjectType.h
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	public enum HKCategoryValueSleepAnalysis : nint {
		InBed,
		Asleep,
		[Watch (3,0), iOS (10,0)]
		Awake,
	}

	// NSUInteger -> HKQuery.h
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	[Flags]
	public enum HKQueryOptions : nuint {
		None               = 0,
		StrictStartDate    = 1 << 0,
		StrictEndDate      = 1 << 1
	}

	// NSUInteger -> HKStatistics.h
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	[Flags]
	public enum HKStatisticsOptions : nuint {
		None              	  = 0,
		SeparateBySource          = 1 << 0,
		DiscreteAverage           = 1 << 1,
		DiscreteMin               = 1 << 2,
		DiscreteMax               = 1 << 3,
		CumulativeSum             = 1 << 4
	}

	// NSInteger -> HKUnit.h
	[Watch (2,0)]
	[iOS (8,0)]
	[Native]
	public enum HKMetricPrefix : nint {
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
		Tera
	}

	// Convenience enum, ObjC uses NSString
	[Watch (2,0)]
	[iOS (8,0)]
	public enum HKQuantityTypeIdentifier {
		BodyMassIndex,
		BodyFatPercentage,
		Height,
		BodyMass,
		LeanBodyMass,
		HeartRate,
		StepCount,
		DistanceWalkingRunning,
		DistanceCycling,
		BasalEnergyBurned,
		ActiveEnergyBurned,
		FlightsClimbed,
		NikeFuel,
		// Blood
		OxygenSaturation,
		BloodGlucose,
		BloodPressureSystolic,
		BloodPressureDiastolic,
		BloodAlcoholContent,
		PeripheralPerfusionIndex,
		// Miscellaneous
		ForcedVitalCapacity,
		ForcedExpiratoryVolume1,
		PeakExpiratoryFlowRate,
		NumberOfTimesFallen,
		InhalerUsage,
		RespiratoryRate,
		BodyTemperature,
		// Nutrition
		DietaryFatTotal,
		DietaryFatPolyunsaturated,
		DietaryFatMonounsaturated,
		DietaryFatSaturated,
		DietaryCholesterol,
		DietarySodium,
		DietaryCarbohydrates,
		DietaryFiber,
		DietarySugar,
		DietaryEnergyConsumed,
		DietaryProtein,
		DietaryVitaminA,
		DietaryVitaminB6,
		DietaryVitaminB12,
		DietaryVitaminC,
		DietaryVitaminD,
		DietaryVitaminE,
		DietaryVitaminK,
		DietaryCalcium,
		DietaryIron,
		DietaryThiamin,
		DietaryRiboflavin,
		DietaryNiacin,
		DietaryFolate,
		DietaryBiotin,
		DietaryPantothenicAcid,
		DietaryPhosphorus,
		DietaryIodine,
		DietaryMagnesium,
		DietaryZinc,
		DietarySelenium,
		DietaryCopper,
		DietaryManganese,
		DietaryChromium,
		DietaryMolybdenum,
		DietaryChloride,
		DietaryPotassium,
		DietaryCaffeine,
		[iOS (9,0)]
		BasalBodyTemperature,
		[iOS (9,0)]
		DietaryWater,
		[iOS (9,0)]
		UVExposure,
		ElectrodermalActivity,
		[iOS (9,3), Watch (2,2)]
		AppleExerciseTime,
		[iOS (10,0), Watch (3,0)]
		DistanceWheelchair,
		[iOS (10,0), Watch (3,0)]
		PushCount,
	}

	// Convenience enum, ObjC uses NSString
	[Watch (2,0)]
	[iOS (8,0)]
	public enum HKCategoryTypeIdentifier {
		SleepAnalysis,
		[iOS (9,0)]
		AppleStandHour,
		[iOS (9,0)]
		CervicalMucusQuality,
		[iOS (9,0)]
		OvulationTestResult,
		[iOS (9,0)]
		MenstrualFlow,
		[iOS (9,0)]
		IntermenstrualBleeding,
		[iOS (9,0)]
		SexualActivity,
		[iOS (10,0), Watch (3,0)]
		MindfulSession,
	}

	// Convenience enum, ObjC uses NSString
	[Watch (2,0)]
	[iOS (8,0)]
	public enum HKCharacteristicTypeIdentifier {
		BiologicalSex,
		BloodType,
		DateOfBirth,
		[iOS (9,0)]
		FitzpatrickSkinType,
		[iOS (10,0), Watch (3,0)]
		WheelchairUse,
	}

	// Convenience enum, ObjC uses NSString
	[Watch (2,0), iOS (8,0)]
	public enum HKCorrelationTypeIdentifier {
		BloodPressure,
		Food,
	}

	[Native]
	[Watch (2,0)]
	[iOS (8,0)]
	public enum HKWorkoutActivityType : nuint {
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
		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use HKWorkoutActivityType.Dance, HKWorkoutActivityType.Barre, or HKWorkoutActivityType.Pilates")]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use HKWorkoutActivityType.Dance, HKWorkoutActivityType.Barre, or HKWorkoutActivityType.Pilates")]
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
		[iOS (10,0), Watch (3,0)]
		Barre,
		[iOS (10,0), Watch (3,0)]
		CoreTraining,
		[iOS (10,0), Watch (3,0)]
		CrossCountrySkiing,
		[iOS (10,0), Watch (3,0)]
		DownhillSkiing,
		[iOS (10,0), Watch (3,0)]
		Flexibility,
		[iOS (10,0), Watch (3,0)]
		HighIntensityIntervalTraining,
		[iOS (10,0), Watch (3,0)]
		JumpRope,
		[iOS (10,0), Watch (3,0)]
		Kickboxing,
		[iOS (10,0), Watch (3,0)]
		Pilates,
		[iOS (10,0), Watch (3,0)]
		Snowboarding,
		[iOS (10,0), Watch (3,0)]
		Stairs,
		[iOS (10,0), Watch (3,0)]
		StepTraining,
		[iOS (10,0), Watch (3,0)]
		WheelchairWalkPace,
		[iOS (10,0), Watch (3,0)]
		WheelchairRunPace,
		[iOS (8,2)]
		Other = 3000
	}

	[Native]
	[Watch (2,0)]
	[iOS (8,0)]
	public enum HKWorkoutEventType : nint {
		Pause = 1,
		Resume,
		[iOS (10,0), Watch (3,0)]
		Lap,
		[iOS (10,0), Watch (3,0)]
		Marker,
		[iOS (10,0), Watch (3,0)]
		MotionPaused,
		[iOS (10,0), Watch (3,0)]
		MotionResumed,
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[Native]
	public enum HKCategoryValue : nint {
		NotApplicable = 0
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[Native]
	public enum HKCategoryValueCervicalMucusQuality : nint {
		NotApplicable = 0,
		Dry = 1,
		Sticky,
		Creamy,
		Watery,
		EggWhite
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[Native]
	public enum HKCategoryValueMenstrualFlow : nint {
		NotApplicable = 0,
		Unspecified = 1,
		Light,
		Medium,
		Heavy
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[Native]
	public enum HKCategoryValueOvulationTestResult : nint {
		NotApplicable = 0,
		Negative = 1,
		Positive,
		Indeterminate
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[Native]
	public enum HKCategoryValueAppleStandHour : nint {
		Stood = 0,
		Idle
	}

	[Watch (2,0)]
	[iOS (9,0)]
	[Native]
	public enum HKFitzpatrickSkinType : nint {
		NotSet = 0,
		I,
		II,
		III,
		IV,
		V,
		VI
	}

	[Watch (3,0), iOS (10,0)]
	[Native]
	public enum HKWheelchairUse : nint {
		NotSet = 0,
		No,
		Yes,
	}

	[Watch (3,0), iOS (10,0)]
	[Native]
	public enum HKWeatherCondition : nint {
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
}
