using CoreFoundation;

namespace HealthKit {
	// NSInteger -> HKDefines.h
	/// <summary>Enumerates the frequences for background delivery of data (see <see cref="HealthKit.HKHealthStore.EnableBackgroundDelivery(HealthKit.HKObjectType,HealthKit.HKUpdateFrequency,System.Action{System.Boolean,Foundation.NSError})" />).</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKUpdateFrequency : long {
		/// <summary>Updates as they become available.</summary>
		Immediate = 1,
		/// <summary>Hourly updates.</summary>
		Hourly,
		/// <summary>Daily updates.</summary>
		Daily,
		/// <summary>Weekly updates.</summary>
		Weekly,
	}

	// NSInteger -> HKDefines.h
	/// <summary>Enumerates the permission of the app to read or write health data.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKAuthorizationStatus : long {
		/// <summary>The permission has not yet been determined.</summary>
		NotDetermined = 0,
		/// <summary>Sharing is not allowed.</summary>
		SharingDenied,
		/// <summary>Sharing is allowed.</summary>
		SharingAuthorized,
	}

	// NSInteger -> HKDefines.h
	/// <summary>Enumerates the biological sexes.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKBiologicalSex : long {
		/// <summary>The user has not recorded their sex in the gender binary.</summary>
		NotSet = 0,
		/// <summary>The female sex.</summary>
		Female,
		/// <summary>The male sex.</summary>
		Male,
		/// <summary>The user is not categorized as either male or female.</summary>
		[MacCatalyst (13, 1)]
		Other,
	}

	// NSInteger -> HKDefines.h
	/// <summary>Enumerates known blood types.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKBloodType : long {
		/// <summary>The user's blood type is not recorded.</summary>
		NotSet = 0,
		/// <summary>The A+ blood type.</summary>
		APositive,
		/// <summary>The A- blood type.</summary>
		ANegative,
		/// <summary>The B+ blood type.</summary>
		BPositive,
		/// <summary>The B- blood type.</summary>
		BNegative,
		/// <summary>The AB+ blood type</summary>
		ABPositive,
		/// <summary>The AB- blood type.</summary>
		ABNegative,
		/// <summary>The O+ blood type.</summary>
		OPositive,
		/// <summary>The O- blood type.</summary>
		ONegative,
	}

	// NSInteger -> HKMetadata.h
	/// <summary>Enumerates the positions at which a thermometer takes its reading.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKBodyTemperatureSensorLocation : long {
		/// <summary>The temperature reading location is unspecified.</summary>
		Other = 0,
		/// <summary>The temperature reading is taken at the user's armpit.</summary>
		Armpit,
		/// <summary>The temperature reading is taken somewhere on the user's body.</summary>
		Body,
		/// <summary>The temperature reading is taken at the user's ear.</summary>
		Ear,
		/// <summary>The temperature reading is taken at the user's finger.</summary>
		Finger,
		/// <summary>The temperature reading is taken in the user's gastrointestinal system.</summary>
		GastroIntestinal,
		/// <summary>The temperature reading is taken in the user's mouth.</summary>
		Mouth,
		/// <summary>The temperature reading is taken at the user's rectum.</summary>
		Rectum,
		/// <summary>The temperature reading is taken at the user's toe.</summary>
		Toe,
		/// <summary>The temperature reading is taken at the user's eardrum.</summary>
		EarDrum,
		/// <summary>The temperature reading is taken at the user's temporal artery.</summary>
		TemporalArtery,
		/// <summary>The temperature reading is taken at the user's forehead.</summary>
		Forehead,
	}

	// NSInteger -> HKMetadata.h
	/// <summary>Enumerates the locations at which a heart rate monitor is attached.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKHeartRateSensorLocation : long {
		/// <summary>The monitor is attached elsewhere on the user's body.</summary>
		Other = 0,
		/// <summary>The monitor is attached to the user's chest.</summary>
		Chest,
		/// <summary>The monitor is attached to the user's wrist.</summary>
		Wrist,
		/// <summary>The monitor is attached to the user's finger.</summary>
		Finger,
		/// <summary>The monitor is attached to the user's hand.</summary>
		Hand,
		/// <summary>The monitor is attached to the user's earlobe.</summary>
		EarLobe,
		/// <summary>The monitor is attached to the user's foot.</summary>
		Foot,
	}

	// NSInteger -> HKObjectType.h
	/// <summary>Enumerates whether an <see cref="HealthKit.HKQuantityType" /> is a cumulative measure (for instance, "active energy burned") or a discrete value (such as "blood alcohol content").</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKQuantityAggregationStyle : long {
		/// <summary>The data are cumulative.</summary>
		Cumulative = 0,
		[MacCatalyst (13, 1)]
		DiscreteArithmetic,
		/// <summary>The data is a single value.</summary>
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'HKQuantityAggregationStyle.DiscreteArithmetic'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HKQuantityAggregationStyle.DiscreteArithmetic'.")]
		Discrete = DiscreteArithmetic,
		[MacCatalyst (13, 1)]
		DiscreteTemporallyWeighted,
		[MacCatalyst (13, 1)]
		DiscreteEquivalentContinuousLevel,
	}

	// NSInteger -> HKObjectType.h
	/// <summary>Enumerates the states of the slumberer: whether they are asleep or merely resting in bed.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKCategoryValueSleepAnalysis : long {
		/// <summary>The user is in bed.</summary>
		InBed,
		/// <summary>The user is asleep.</summary>
		Asleep,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Awake,
		[MacCatalyst (16, 0), iOS (16, 0), NoTV]
		AsleepCore = 3,
		[MacCatalyst (16, 0), iOS (16, 0), NoTV]
		AsleepDeep = 4,
		[MacCatalyst (16, 0), iOS (16, 0), NoTV]
		AsleepREM = 5,
	}

	// NSUInteger -> HKQuery.h
	/// <summary>Enumerates options available for use with the <see cref="HealthKit.HKQuery.GetPredicateForSamples(Foundation.NSDate,Foundation.NSDate,HealthKit.HKQueryOptions)" /> method.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum HKQueryOptions : ulong {
		/// <summary>The sample must be entirely within the date range, inclusively.</summary>
		None = 0,
		/// <summary>The sample must start at or after the specified start date.</summary>
		StrictStartDate = 1 << 0,
		/// <summary>The sample must end at or before the specified end time.</summary>
		StrictEndDate = 1 << 1,
	}

	// NSUInteger -> HKStatistics.h
	/// <summary>Enumerates options applicable to <see cref="HealthKit.HKStatisticsQuery" /> and <see cref="HealthKit.HKStatisticsCollectionQuery" /> objets.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum HKStatisticsOptions : ulong {
		/// <summary>Do nothing with the data.</summary>
		None = 0,
		/// <summary>Calculate each statistic by source.</summary>
		SeparateBySource = 1 << 0,
		/// <summary>Calculate the average of all the matching data.</summary>
		DiscreteAverage = 1 << 1,
		/// <summary>Find the minimum value in the matching data.</summary>
		DiscreteMin = 1 << 2,
		/// <summary>Find the maximum value in the matching data.</summary>
		DiscreteMax = 1 << 3,
		/// <summary>Calculate a sum of all the matching data.</summary>
		CumulativeSum = 1 << 4,
		[MacCatalyst (13, 1)]
		MostRecent = 1 << 5,
		[MacCatalyst (13, 1)]
		Duration = 1 << 6,
	}

	// NSInteger -> HKUnit.h
	/// <summary>Enumerates metric prefixes, e.g., Centi-, Deca-, Deci-. Used with factory methods of <see cref="HealthKit.HKUnit" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKMetricPrefix : long {
		/// <summary>No prefix</summary>
		None = 0,
		/// <summary>The "pico-" prefix.</summary>
		Pico,
		/// <summary>The "nano-" prefix.</summary>
		Nano,
		/// <summary>The "micro-" prefix.</summary>
		Micro,
		/// <summary>The "milli-" prefix.</summary>
		Milli,
		/// <summary>The "centi-" prefix.</summary>
		Centi,
		/// <summary>The "deci-" prefix.</summary>
		Deci,
		/// <summary>The "deca-" prefix.</summary>
		Deca,
		/// <summary>The "hecto-" prefix.</summary>
		Hecto,
		/// <summary>The "kilo-" prefix.</summary>
		Kilo,
		/// <summary>The "mega-" prefix.</summary>
		Mega,
		/// <summary>The "giga-" prefix.</summary>
		Giga,
		/// <summary>The "tera-" prefix.</summary>
		Tera,
		[MacCatalyst (13, 1)]
		Femto,
	}

	/// <summary>Enumerates various activities that are considered workouts.</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum HKWorkoutActivityType : ulong {
		/// <summary>American football.</summary>
		AmericanFootball = 1,
		/// <summary>Archery.</summary>
		Archery,
		/// <summary>Australian football.</summary>
		AustralianFootball,
		/// <summary>Badminton.</summary>
		Badminton,
		/// <summary>Baseball.</summary>
		Baseball,
		/// <summary>Basketball.</summary>
		Basketball,
		/// <summary>Bowling.</summary>
		Bowling,
		/// <summary>Boxing.</summary>
		Boxing,
		/// <summary>Climbing.</summary>
		Climbing,
		/// <summary>Cricket.</summary>
		Cricket,
		/// <summary>Cross training.</summary>
		CrossTraining,
		/// <summary>Curling.</summary>
		Curling,
		/// <summary>Cycling.</summary>
		Cycling,
		/// <summary>Dance.</summary>
		Dance,
		/// <summary>Developers should not use this deprecated field. Developers should use 'HKWorkoutActivityType.Dance', 'HKWorkoutActivityType.Barre', or 'HKWorkoutActivityType.Pilates'.</summary>
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'HKWorkoutActivityType.Dance', 'HKWorkoutActivityType.Barre', or 'HKWorkoutActivityType.Pilates'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HKWorkoutActivityType.Dance', 'HKWorkoutActivityType.Barre', or 'HKWorkoutActivityType.Pilates'.")]
		DanceInspiredTraining,
		/// <summary>Elliptical machine training.</summary>
		Elliptical,
		/// <summary>Equestrian sports.</summary>
		EquestrianSports,
		/// <summary>Fencing.</summary>
		Fencing,
		/// <summary>Fishing.</summary>
		Fishing,
		/// <summary>Functional strength training.</summary>
		FunctionalStrengthTraining,
		/// <summary>Golf.</summary>
		Golf,
		/// <summary>Gymnastics.</summary>
		Gymnastics,
		/// <summary>Handball.</summary>
		Handball,
		/// <summary>Hiking.</summary>
		Hiking,
		/// <summary>Hockey.</summary>
		Hockey,
		/// <summary>Hunting.</summary>
		Hunting,
		/// <summary>Lacrosse.</summary>
		Lacrosse,
		/// <summary>Martial arts.</summary>
		MartialArts,
		/// <summary>Mind and body training.</summary>
		MindAndBody,
		/// <summary>Mixed metabolic cardio training.</summary>
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'MixedCardio' or 'HighIntensityIntervalTraining' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MixedCardio' or 'HighIntensityIntervalTraining' instead.")]
		MixedMetabolicCardioTraining,
		/// <summary>Paddle sports.</summary>
		PaddleSports,
		/// <summary>Play.</summary>
		Play,
		/// <summary>Preparation and recovery.</summary>
		PreparationAndRecovery,
		/// <summary>Racquetball.</summary>
		Racquetball,
		/// <summary>Rowing.</summary>
		Rowing,
		/// <summary>Rugby.</summary>
		Rugby,
		/// <summary>Running.</summary>
		Running,
		/// <summary>Sailing.</summary>
		Sailing,
		/// <summary>Skating sports.</summary>
		SkatingSports,
		/// <summary>Snow sports.</summary>
		SnowSports,
		/// <summary>Soccer.</summary>
		Soccer,
		/// <summary>Softball.</summary>
		Softball,
		/// <summary>Squash.</summary>
		Squash,
		/// <summary>Stair climbing.</summary>
		StairClimbing,
		/// <summary>Surfing sports.</summary>
		SurfingSports,
		/// <summary>Swimming.</summary>
		Swimming,
		/// <summary>Table tennis.</summary>
		TableTennis,
		/// <summary>Tennis.</summary>
		Tennis,
		/// <summary>Track and field.</summary>
		TrackAndField,
		/// <summary>Traditional strength training.</summary>
		TraditionalStrengthTraining,
		/// <summary>Volleyball.</summary>
		Volleyball,
		/// <summary>Walking.</summary>
		Walking,
		/// <summary>Water fitness.</summary>
		WaterFitness,
		/// <summary>Water polo.</summary>
		WaterPolo,
		/// <summary>Water sports.</summary>
		WaterSports,
		/// <summary>Wrestling.</summary>
		Wrestling,
		/// <summary>Yoga.</summary>
		Yoga,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Barre,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		CoreTraining,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		CrossCountrySkiing,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		DownhillSkiing,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Flexibility,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		HighIntensityIntervalTraining,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		JumpRope,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Kickboxing,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Pilates,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Snowboarding,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Stairs,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		StepTraining,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		WheelchairWalkPace,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		WheelchairRunPace,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		TaiChi,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		MixedCardio,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		HandCycling,
		[MacCatalyst (13, 1)]
		DiscSports,
		[MacCatalyst (13, 1)]
		FitnessGaming,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		CardioDance = 77,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		SocialDance = 78,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Pickleball = 79,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Cooldown = 80,
		[MacCatalyst (16, 0), iOS (16, 0), NoTV]
		SwimBikeRun = 82,
		[MacCatalyst (16, 0), iOS (16, 0), NoTV]
		Transition = 83,
		[MacCatalyst (17, 0), iOS (17, 0), NoTV]
		UnderwaterDiving,
		/// <summary>Another kind of workout.</summary>
		[MacCatalyst (13, 1)]
		Other = 3000,
	}

	/// <summary>Enumerates events that can occur during a workout (Pause, Resume).</summary>
	[Native]
	[MacCatalyst (13, 1)]
	public enum HKWorkoutEventType : long {
		/// <summary>The activity was paused.</summary>
		Pause = 1,
		/// <summary>The activity was resumed.</summary>
		Resume,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Lap,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Marker,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		MotionPaused,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		MotionResumed,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Segment,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		PauseOrResumeRequest,
	}

	/// <summary>Contains a single value that indicates that a category value is not applicable to the category.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKCategoryValue : long {
		/// <summary>A category value is not applicable to the category.</summary>
		NotApplicable = 0,
	}

	/// <summary>Enumerates the user's cervical mucus quality.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKCategoryValueCervicalMucusQuality : long {
		/// <summary>Either not recorded or permissions not given.</summary>
		NotApplicable = 0,
		/// <summary>Little or no mucus.</summary>
		Dry = 1,
		/// <summary>Sticky mucus.</summary>
		Sticky,
		/// <summary>Creamy mucus.</summary>
		Creamy,
		/// <summary>Watery mucus.</summary>
		Watery,
		/// <summary>Mucus the color and consistency of egg whites.</summary>
		EggWhite,
	}

	/// <summary>Enumerates menopausal states recorded in a HealthKit category sample.</summary>
	[MacCatalyst (27, 0), Mac (27, 0), iOS (27, 0)]
	[Native]
	public enum HKCategoryValueMenopausalState : long {
		/// <summary>The user is in menopause.</summary>
		Menopause = 1,
		/// <summary>The user is in perimenopause.</summary>
		Perimenopause = 2,
		/// <summary>The user is not in menopause or perimenopause.</summary>
		None = 3,
	}

	/// <summary>Enumerates the amount of menstrual flow.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'HKCategoryValueVaginalBleeding' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'HKCategoryValueVaginalBleeding' instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'HKCategoryValueVaginalBleeding' instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'HKCategoryValueVaginalBleeding' instead.")]
	public enum HKCategoryValueMenstrualFlow : long {
		/// <summary>Categorization is not applicable.</summary>
		NotApplicable = 0,
		/// <summary>The level is unspecified.</summary>
		Unspecified = 1,
		/// <summary>Light flow.</summary>
		Light,
		/// <summary>Medium flow.</summary>
		Medium,
		/// <summary>Heavy flow.</summary>
		Heavy,
		[MacCatalyst (13, 1)]
		None,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum HKCategoryValueVaginalBleeding : long {
		Unspecified = 1,
		Light = 2,
		Medium = 3,
		Heavy = 4,
		None = 5,
	}

	/// <summary>Enumerates the results of an ovulation test.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKCategoryValueOvulationTestResult : long {
		/// <summary>The test is not applicable.</summary>
		NotApplicable = 0,
		/// <summary>The test was negative.</summary>
		Negative = 1,
		[MacCatalyst (13, 1)]
		LuteinizingHormoneSurge = 2,
		/// <summary>The test was positive.</summary>
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'HKCategoryValueOvulationTestResult.LuteinizingHormoneSurge' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HKCategoryValueOvulationTestResult.LuteinizingHormoneSurge' instead.")]
		Positive = LuteinizingHormoneSurge,
		/// <summary>The test was indeterminate.</summary>
		Indeterminate = 3,
		[MacCatalyst (13, 1)]
		EstrogenSurge = 4,
	}

	/// <summary>Enumerates whether the user stood or not during an hour.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKCategoryValueAppleStandHour : long {
		/// <summary>The user did stand for at least a few minutes.</summary>
		Stood = 0,
		/// <summary>The user did not stand.</summary>
		Idle,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum HKCategoryValueAudioExposureEvent : long {
		LoudEnvironment = 1,
	}

	/// <summary>Enumerates skin types using the Fitzpatrick scale.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKFitzpatrickSkinType : long {
		/// <summary>Either the user's skin type has not been set or permissions have not been granted.</summary>
		NotSet = 0,
		/// <summary>Pale white skin that never tans and burns easily.</summary>
		I,
		/// <summary>White skin that tans minimally and burns easily.</summary>
		II,
		/// <summary>White to light-brown skin that tans uniformly and burns moderately.</summary>
		III,
		/// <summary>Beige-olive, lightly-tanned skin that tans uniformly and burns moderately.</summary>
		IV,
		/// <summary>Brown skin that tans profusely and rarely burns.</summary>
		V,
		/// <summary>Dark brown to black skin that tans profusely and never burns.</summary>
		VI,
	}

	/// <summary>Enumerates constants that describe wheelchair use.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKWheelchairUse : long {
		/// <summary>Indicates that a wheelchair use value was not set.</summary>
		NotSet = 0,
		/// <summary>Indicates that a wheelchair was not used.</summary>
		No,
		/// <summary>Indicates that a wheelchair was used.</summary>
		Yes,
	}

	/// <summary>Enumerates weather types.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKWeatherCondition : long {
		/// <summary>The weather has not been indicated.</summary>
		None = 0,
		/// <summary>Indicates that the weather is clear.</summary>
		Clear,
		/// <summary>Indicates that the weather is fair.</summary>
		Fair,
		/// <summary>Indicates that the weather is partly cloudy.</summary>
		PartlyCloudy,
		/// <summary>Indicates that the weather is mostly cloudy.</summary>
		MostlyCloudy,
		/// <summary>Indicates that the weather is cloudy.</summary>
		Cloudy,
		/// <summary>Indicates that the weather is foggy.</summary>
		Foggy,
		/// <summary>Indicates haze.</summary>
		Haze,
		/// <summary>Indicates that it is windy outside.</summary>
		Windy,
		/// <summary>Indicates that the weather is blustery.</summary>
		Blustery,
		/// <summary>Indicates smoky weather.</summary>
		Smoky,
		/// <summary>Indicates that the weather is dusty.</summary>
		Dust,
		/// <summary>Indicates falling snow.</summary>
		Snow,
		/// <summary>Indicates hail.</summary>
		Hail,
		/// <summary>Indicates sleet.</summary>
		Sleet,
		/// <summary>Indicates freezing drizzle.</summary>
		FreezingDrizzle,
		/// <summary>Indicates freezing rain.</summary>
		FreezingRain,
		/// <summary>Indicates mixed rain and hail.</summary>
		MixedRainAndHail,
		/// <summary>Indicates mixed rain and snow.</summary>
		MixedRainAndSnow,
		/// <summary>Indicates mixed rain and sleet.</summary>
		MixedRainAndSleet,
		/// <summary>Indicates mixed sleet and snow.</summary>
		MixedSnowAndSleet,
		/// <summary>Indicates that the weather is drizzly.</summary>
		Drizzle,
		/// <summary>Indicates scattered showers.</summary>
		ScatteredShowers,
		/// <summary>Indicates showers.</summary>
		Showers,
		/// <summary>Indicates thunderstorms.</summary>
		Thunderstorms,
		/// <summary>Indicates a tropical storm.</summary>
		TropicalStorm,
		/// <summary>Indicates that there is a hurricane.</summary>
		Hurricane,
		/// <summary>Indicates one or more tornadoes.</summary>
		Tornado,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum HKWorkoutSwimmingLocationType : long {
		/// <summary>To be added.</summary>
		Unknown = 0,
		/// <summary>To be added.</summary>
		Pool,
		/// <summary>To be added.</summary>
		OpenWater,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum HKSwimmingStrokeStyle : long {
		/// <summary>To be added.</summary>
		Unknown = 0,
		/// <summary>To be added.</summary>
		Mixed,
		/// <summary>To be added.</summary>
		Freestyle,
		/// <summary>To be added.</summary>
		Backstroke,
		/// <summary>To be added.</summary>
		Breaststroke,
		/// <summary>To be added.</summary>
		Butterfly,
		[MacCatalyst (16, 0), iOS (16, 0), NoTV]
		Kickboard = 6,
	}

	/// <summary>Enumerates the reasons for why insulin was provided.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKInsulinDeliveryReason : long {
		/// <summary>Indicates that a dose was given to maintain a base level of blood chemistry.</summary>
		Basal = 1,
		/// <summary>Indicates that a dose was given in response to an event, such as a meal, that affected blood chemistry.</summary>
		Bolus,
	}

	/// <summary>Enumerates values that tell whether a blood glucose level was taken before or after a meal.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKBloodGlucoseMealTime : long {
		/// <summary>Indicates a measurement taken before a meal.</summary>
		Preprandial = 1,
		/// <summary>Indicates a measurement taken after a meal.</summary>
		Postprandial,
	}

	/// <summary>Enumerates the testing process used for establishing VO2 Max.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKVO2MaxTestType : long {
		/// <summary>To be added.</summary>
		MaxExercise = 1,
		/// <summary>To be added.</summary>
		PredictionSubMaxExercise,
		/// <summary>To be added.</summary>
		PredictionNonExercise,
		[iOS (26, 0), TV (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		PredictionStepTest,
	}

	/// <summary>Enumerates Fast Healthcare Interoperability Resources (FHIR) types.</summary>
	[MacCatalyst (13, 1)]
	public enum HKFhirResourceType {
		/// <summary>Indicates allergy or intolerance information.</summary>
		[Field ("HKFHIRResourceTypeAllergyIntolerance")]
		AllergyIntolerance,
		/// <summary>Indicates medical condition information.</summary>
		[Field ("HKFHIRResourceTypeCondition")]
		Condition,
		/// <summary>Indicates immunization information.</summary>
		[Field ("HKFHIRResourceTypeImmunization")]
		Immunization,
		/// <summary>Indicates information about dispensed or delivered medications.</summary>
		[Field ("HKFHIRResourceTypeMedicationDispense")]
		MedicationDispense,
		/// <summary>Indicates information about ordered medications</summary>
		[Field ("HKFHIRResourceTypeMedicationOrder")]
		MedicationOrder,
		/// <summary>Indicates information about patient statements of medication taken.</summary>
		[Field ("HKFHIRResourceTypeMedicationStatement")]
		MedicationStatement,
		/// <summary>Indicates information about a medical observation.</summary>
		[Field ("HKFHIRResourceTypeObservation")]
		Observation,
		/// <summary>Indicates information about a procedure performed.</summary>
		[Field ("HKFHIRResourceTypeProcedure")]
		Procedure,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKFHIRResourceTypeMedicationRequest")]
		MedicationRequest,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKFHIRResourceTypeCoverage")]
		Coverage,
		[iOS (16, 4), MacCatalyst (16, 4)]
		[Field ("HKFHIRResourceTypeDiagnosticReport")]
		DiagnosticReport,
		[iOS (16, 4), MacCatalyst (16, 4)]
		[Field ("HKFHIRResourceTypeDocumentReference")]
		DocumentReference,
	}

	/// <summary>Enumerates clinical record type identifiers.</summary>
	[MacCatalyst (13, 1)]
	public enum HKClinicalTypeIdentifier {

		/// <summary>Indicates an allergy record.</summary>
		[Field ("HKClinicalTypeIdentifierAllergyRecord")]
		AllergyRecord,
		/// <summary>Indicates a record for a medical condition.</summary>
		[Field ("HKClinicalTypeIdentifierConditionRecord")]
		ConditionRecord,
		/// <summary>Indicates an immunization record.</summary>
		[Field ("HKClinicalTypeIdentifierImmunizationRecord")]
		ImmunizationRecord,
		/// <summary>Indicates a lab result record.</summary>
		[Field ("HKClinicalTypeIdentifierLabResultRecord")]
		LabResultRecord,
		/// <summary>Indicates a record of medication</summary>
		[Field ("HKClinicalTypeIdentifierMedicationRecord")]
		MedicationRecord,
		/// <summary>Indicates a record of a procedure.</summary>
		[Field ("HKClinicalTypeIdentifierProcedureRecord")]
		ProcedureRecord,
		/// <summary>Indicates a record of a vital sign.</summary>
		[Field ("HKClinicalTypeIdentifierVitalSignRecord")]
		VitalSignRecord,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("HKClinicalTypeIdentifierCoverageRecord")]
		CoverageRecord,
		[iOS (16, 4)]
		[MacCatalyst (16, 4)]
		[Field ("HKClinicalTypeIdentifierClinicalNoteRecord")]
		ClinicalNoteRecord,
	}

	/// <summary>Enumerates values that tell when an app should request user permission for access.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKAuthorizationRequestStatus : long {
		/// <summary>Indicates that it is unknown whether the app should request user permission.</summary>
		Unknown = 0,
		/// <summary>Indicates that the app should request user permission.</summary>
		ShouldRequest,
		/// <summary>Indicates that the app does not need to request user permission.</summary>
		Unnecessary,
	}

	[iOS (13, 6)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKCategoryValueAppetiteChanges : long {
		Unspecified = 0,
		NoChange,
		Decreased,
		Increased,
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum HKAppleEcgAlgorithmVersion : long {
		Version1 = 1,
		Version2 = 2,
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum HKCategoryValueEnvironmentalAudioExposureEvent : long {
		MomentaryLimit = 1,
	}

	[iOS (13, 6)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKCategoryValuePresence : long {
		Present = 0,
		NotPresent,
	}

	[iOS (13, 6)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum HKCategoryValueSeverity : long {
		Unspecified = 0,
		NotPresent,
		Mild,
		Moderate,
		Severe,
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum HKDevicePlacementSide : long {
		Unknown = 0,
		Left,
		Right,
		Central,
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
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

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum HKElectrocardiogramLead : long {
		AppleWatchSimilarToLeadI = 1,
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum HKElectrocardiogramSymptomsStatus : long {
		NotSet = 0,
		None = 1,
		Present = 2,
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum HKFhirRelease {
		[Field ("HKFHIRReleaseDSTU2")]
		Dstu2,
		[Field ("HKFHIRReleaseR4")]
		R4,
		[Field ("HKFHIRReleaseUnknown")]
		Unknown,
	}

	[MacCatalyst (16, 0), iOS (16, 0), NoTV]
	[Native]
	public enum HKHeartRateRecoveryTestType : long {
		MaxExercise = 1,
		PredictionSubMaxExercise,
		PredictionNonExercise,
		/// <summary>Identifies a step-test protocol used to predict heart rate recovery.</summary>
		[iOS (27, 2), Mac (27, 2), MacCatalyst (27, 2)]
		StepTest = 4,
	}

	[MacCatalyst (16, 0), iOS (16, 0), NoTV]
	[Native]
	public enum HKPrismBase : long {
		None = 0,
		Up,
		Down,
		In,
		Out,
	}

	[MacCatalyst (16, 0), iOS (16, 0), NoTV]
	[Native]
	public enum HKUserMotionContext : long {
		NotSet = 0,
		Stationary,
		Active,
	}

	[MacCatalyst (16, 0), iOS (16, 0), NoTV]
	[Native]
	public enum HKVisionEye : long {
		Left = 1,
		Right,
	}

	[MacCatalyst (16, 0), iOS (16, 0), NoTV]
	[Native]
	public enum HKVisionPrescriptionType : ulong {
		Glasses = 1,
		Contacts,
	}

	[MacCatalyst (17, 0), iOS (17, 0)]
	[Native]
	public enum HKCyclingFunctionalThresholdPowerTestType : long {
		MaxExercise60Minute = 1,
		MaxExercise20Minute,
		RampTest,
		PredictionExercise,
	}

	[MacCatalyst (17, 0), iOS (17, 0)]
	[Native]
	public enum HKPhysicalEffortEstimationType : long {
		ActivityLookup = 1,
		DeviceSensed,
	}

	[MacCatalyst (17, 0), iOS (17, 0)]
	[Native]
	public enum HKWaterSalinity : long {
		FreshWater = 1,
		SaltWater,
	}

	[iOS (17, 0)]
	[Native]
	public enum HKWorkoutSessionType : long {
		Primary = 0,
		Mirrored,
	}
}
