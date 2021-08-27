#if !XAMCORE_4_0

using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace HealthKit {
#if !NET
	[Obsolete ("Use the 'HKQuantityTypeIdentifier' enum instead.")]
	[Introduced (PlatformName.iOS, 8,0, PlatformArchitecture.All)]
#else
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("tvos")]
	[Obsolete ("Use the 'HKQuantityTypeIdentifier' enum instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public unsafe static partial class HKQuantityTypeIdentifierKey  {
		public static NSString ActiveEnergyBurned {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.ActiveEnergyBurned);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 9,3, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 2,2, PlatformArchitecture.All)]
#endif
		public static NSString AppleExerciseTime {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.AppleExerciseTime);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 9,0, PlatformArchitecture.All)]
#endif
		public static NSString BasalBodyTemperature {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BasalBodyTemperature);
			}
		}

		public static NSString BasalEnergyBurned {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BasalEnergyBurned);
			}
		}

		public static NSString BloodAlcoholContent {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BloodAlcoholContent);
			}
		}

		public static NSString BloodGlucose {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BloodGlucose);
			}
		}

		public static NSString BloodPressureDiastolic {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BloodPressureDiastolic);
			}
		}

		public static NSString BloodPressureSystolic {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BloodPressureSystolic);
			}
		}

		public static NSString BodyFatPercentage {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BodyFatPercentage);
			}
		}

		public static NSString BodyMass {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BodyMass);
			}
		}

		public static NSString BodyMassIndex {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BodyMassIndex);
			}
		}

		public static NSString BodyTemperature {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BodyTemperature);
			}
		}

		public static NSString DietaryBiotin {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryBiotin);
			}
		}

		public static NSString DietaryCaffeine {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryCaffeine);
			}
		}

		public static NSString DietaryCalcium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryCalcium);
			}
		}

		public static NSString DietaryCarbohydrates {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryCarbohydrates);
			}
		}

		public static NSString DietaryChloride {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryChloride);
			}
		}

		public static NSString DietaryCholesterol {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryCholesterol);
			}
		}

		public static NSString DietaryChromium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryChromium);
			}
		}

		public static NSString DietaryCopper {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryCopper);
			}
		}

		public static NSString DietaryEnergyConsumed {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryEnergyConsumed);
			}
		}

		public static NSString DietaryFatMonounsaturated {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFatMonounsaturated);
			}
		}

		public static NSString DietaryFatPolyunsaturated {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFatPolyunsaturated);
			}
		}

		public static NSString DietaryFatSaturated {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFatSaturated);
			}
		}

		public static NSString DietaryFatTotal {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFatTotal);
			}
		}

		public static NSString DietaryFiber {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFiber);
			}
		}

		public static NSString DietaryFolate {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFolate);
			}
		}

		public static NSString DietaryIodine {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryIodine);
			}
		}

		public static NSString DietaryIron {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryIron);
			}
		}

		public static NSString DietaryMagnesium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryMagnesium);
			}
		}

		public static NSString DietaryManganese {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryManganese);
			}
		}

		public static NSString DietaryMolybdenum {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryMolybdenum);
			}
		}

		public static NSString DietaryNiacin {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryNiacin);
			}
		}

		public static NSString DietaryPantothenicAcid {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryPantothenicAcid);
			}
		}

		public static NSString DietaryPhosphorus {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryPhosphorus);
			}
		}

		public static NSString DietaryPotassium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryPotassium);
			}
		}

		public static NSString DietaryProtein {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryProtein);
			}
		}

		public static NSString DietaryRiboflavin {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryRiboflavin);
			}
		}

		public static NSString DietarySelenium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietarySelenium);
			}
		}

		public static NSString DietarySodium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietarySodium);
			}
		}

		public static NSString DietarySugar {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietarySugar);
			}
		}

		public static NSString DietaryThiamin {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryThiamin);
			}
		}

		public static NSString DietaryVitaminA {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminA);
			}
		}

		public static NSString DietaryVitaminB12 {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminB12);
			}
		}

		public static NSString DietaryVitaminB6 {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminB6);
			}
		}

		public static NSString DietaryVitaminC {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminC);
			}
		}

		public static NSString DietaryVitaminD {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminD);
			}
		}

		public static NSString DietaryVitaminE {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminE);
			}
		}

		public static NSString DietaryVitaminK {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminK);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 9,0, PlatformArchitecture.All)]
#endif
		public static NSString DietaryWater {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryWater);
			}
		}

		public static NSString DietaryZinc {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryZinc);
			}
		}

		public static NSString DistanceCycling {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DistanceCycling);
			}
		}

#if !NET
		[Introduced (PlatformName.WatchOS, 4,2, PlatformArchitecture.All)]
		[Introduced (PlatformName.iOS, 11,2, PlatformArchitecture.All)]
#else
		[SupportedOSPlatform ("ios11.2")]
#endif
		public static NSString DistanceDownhillSnowSports {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DistanceDownhillSnowSports);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 10,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 3,0, PlatformArchitecture.All)]
#endif
		public static NSString DistanceSwimming {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DistanceSwimming);
			}
		}

		public static NSString DistanceWalkingRunning {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DistanceWalkingRunning);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 10,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 3,0, PlatformArchitecture.All)]
#endif
		public static NSString DistanceWheelchair {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DistanceWheelchair);
			}
		}

		public static NSString ElectrodermalActivity {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.ElectrodermalActivity);
			}
		}

		public static NSString FlightsClimbed {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.FlightsClimbed);
			}
		}

		public static NSString ForcedExpiratoryVolume1 {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.ForcedExpiratoryVolume1);
			}
		}

		public static NSString ForcedVitalCapacity {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.ForcedVitalCapacity);
			}
		}

		public static NSString HeartRate {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.HeartRate);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 11,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 4,0, PlatformArchitecture.All)]
#else
		[SupportedOSPlatform ("ios11.0")]
#endif
		public static NSString HeartRateVariabilitySdnn {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.HeartRateVariabilitySdnn);
			}
		}

		public static NSString Height {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.Height);
			}
		}

		public static NSString InhalerUsage {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.InhalerUsage);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 11,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 4,0, PlatformArchitecture.All)]
#else
		[SupportedOSPlatform ("ios11.0")]
#endif
		public static NSString InsulinDelivery {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.InsulinDelivery);
			}
		}

		public static NSString LeanBodyMass {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.LeanBodyMass);
			}
		}

		public static NSString NikeFuel {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.NikeFuel);
			}
		}

		public static NSString NumberOfTimesFallen {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.NumberOfTimesFallen);
			}
		}

		public static NSString OxygenSaturation {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.OxygenSaturation);
			}
		}

		public static NSString PeakExpiratoryFlowRate {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.PeakExpiratoryFlowRate);
			}
		}

		public static NSString PeripheralPerfusionIndex {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.PeripheralPerfusionIndex);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 10,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 3,0, PlatformArchitecture.All)]
#endif
		public static NSString PushCount {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.PushCount);
			}
		}

		public static NSString RespiratoryRate {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.RespiratoryRate);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 11,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 4,0, PlatformArchitecture.All)]
#else
		[SupportedOSPlatform ("ios11.0")]
#endif
		public static NSString RestingHeartRate {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.RestingHeartRate);
			}
		}

		public static NSString StepCount {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.StepCount);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 10,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 3,0, PlatformArchitecture.All)]
#endif
		public static NSString SwimmingStrokeCount {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.SwimmingStrokeCount);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 9,0, PlatformArchitecture.All)]
#endif
		public static NSString UVExposure {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.UVExposure);
			}
		}

#if !NET
		[Introduced (PlatformName.WatchOS, 4,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.iOS, 11,0, PlatformArchitecture.All)]
#else
		[SupportedOSPlatform ("ios11.0")]
#endif
		public static NSString VO2Max {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.VO2Max);
			}
		}

#if !NET
		[Introduced (PlatformName.WatchOS, 4,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.iOS, 11,0, PlatformArchitecture.All)]
#else
		[SupportedOSPlatform ("ios11.0")]
#endif
		public static NSString WaistCircumference {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.WaistCircumference);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 11,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 4,0, PlatformArchitecture.All)]
#else
		[SupportedOSPlatform ("ios11.0")]
#endif
		public static NSString WalkingHeartRateAverage {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.WalkingHeartRateAverage);
			}
		}
	} /* class HKQuantityTypeIdentifierKey */

#if !NET
	[Obsolete ("Use the 'HKCategoryTypeIdentifier' enum instead.")]
	[Introduced (PlatformName.iOS, 8,0, PlatformArchitecture.All)]
#else
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[Obsolete ("Starting with ios8.0 use the 'HKCategoryTypeIdentifier' enum instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	public unsafe static partial class HKCategoryTypeIdentifierKey  {
#if !NET
		[Introduced (PlatformName.iOS, 9,0, PlatformArchitecture.All)]
#endif
		public static NSString AppleStandHour {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.AppleStandHour);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 9,0, PlatformArchitecture.All)]
#endif
		public static NSString CervicalMucusQuality {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.CervicalMucusQuality);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 9,0, PlatformArchitecture.All)]
#endif
		public static NSString IntermenstrualBleeding {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.IntermenstrualBleeding);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 9,0, PlatformArchitecture.All)]
#endif
		public static NSString MenstrualFlow {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.MenstrualFlow);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 10,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 3,0, PlatformArchitecture.All)]
#endif
		public static NSString MindfulSession {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.MindfulSession);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 9,0, PlatformArchitecture.All)]
#endif
		public static NSString OvulationTestResult {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.OvulationTestResult);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 9,0, PlatformArchitecture.All)]
#endif
		public static NSString SexualActivity {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.SexualActivity);
			}
		}

		public static NSString SleepAnalysis {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.SleepAnalysis);
			}
		}
	} /* class HKCategoryTypeIdentifierKey */

#if !NET
	[Introduced (PlatformName.iOS, 8,0, PlatformArchitecture.All)]
	[Obsolete ("Use the 'HKCharacteristicTypeIdentifier' enum instead.")]
#else
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[Obsolete ("Use the 'HKCharacteristicTypeIdentifier' enum instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	public unsafe static partial class HKCharacteristicTypeIdentifierKey  {
		public static NSString BiologicalSex {
			get {
				return HKCharacteristicTypeIdentifierExtensions.GetConstant (HKCharacteristicTypeIdentifier.BiologicalSex);
			}
		}

		public static NSString BloodType {
			get {
				return HKCharacteristicTypeIdentifierExtensions.GetConstant (HKCharacteristicTypeIdentifier.BloodType);
			}
		}

		public static NSString DateOfBirth {
			get {
				return HKCharacteristicTypeIdentifierExtensions.GetConstant (HKCharacteristicTypeIdentifier.DateOfBirth);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 9,0, PlatformArchitecture.All)]
#endif
		public static NSString FitzpatrickSkinType {
			get {
				return HKCharacteristicTypeIdentifierExtensions.GetConstant (HKCharacteristicTypeIdentifier.FitzpatrickSkinType);
			}
		}

#if !NET
		[Introduced (PlatformName.iOS, 10,0, PlatformArchitecture.All)]
		[Introduced (PlatformName.WatchOS, 3,0, PlatformArchitecture.All)]
#endif
		public static NSString WheelchairUse {
			get {
				return HKCharacteristicTypeIdentifierExtensions.GetConstant (HKCharacteristicTypeIdentifier.WheelchairUse);
			}
		}
	} /* class HKCharacteristicTypeIdentifierKey */

#if !NET
	[Obsolete ("Use the 'HKCorrelationType' enum instead.")]
	[Introduced (PlatformName.iOS, 8,0, PlatformArchitecture.All)]
#else
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[Obsolete ("Use the 'HKCorrelationType' enum instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public unsafe static partial class HKCorrelationTypeKey  {
		public static NSString IdentifierBloodPressure {
			get {
				return HKCorrelationTypeIdentifierExtensions.GetConstant (HKCorrelationTypeIdentifier.BloodPressure);
			}
		}

		public static NSString IdentifierFood {
			get {
				return HKCorrelationTypeIdentifierExtensions.GetConstant (HKCorrelationTypeIdentifier.Food);
			}
		}
	} /* class HKCorrelationTypeKey */
}

#endif // !XAMCORE_4_0
