#nullable enable

#if !NET && (IOS || WATCHOS)

using System;
using Foundation;
using ObjCRuntime;

namespace HealthKit {
	[Obsolete ("Use the 'HKQuantityTypeIdentifier' enum instead.")]
	public unsafe static partial class HKQuantityTypeIdentifierKey  {
		public static NSString? ActiveEnergyBurned {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.ActiveEnergyBurned);
			}
		}

		public static NSString? AppleExerciseTime {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.AppleExerciseTime);
			}
		}

		public static NSString? BasalBodyTemperature {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BasalBodyTemperature);
			}
		}

		public static NSString? BasalEnergyBurned {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BasalEnergyBurned);
			}
		}

		public static NSString? BloodAlcoholContent {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BloodAlcoholContent);
			}
		}

		public static NSString? BloodGlucose {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BloodGlucose);
			}
		}

		public static NSString? BloodPressureDiastolic {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BloodPressureDiastolic);
			}
		}

		public static NSString? BloodPressureSystolic {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BloodPressureSystolic);
			}
		}

		public static NSString? BodyFatPercentage {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BodyFatPercentage);
			}
		}

		public static NSString? BodyMass {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BodyMass);
			}
		}

		public static NSString? BodyMassIndex {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BodyMassIndex);
			}
		}

		public static NSString? BodyTemperature {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.BodyTemperature);
			}
		}

		public static NSString? DietaryBiotin {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryBiotin);
			}
		}

		public static NSString? DietaryCaffeine {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryCaffeine);
			}
		}

		public static NSString? DietaryCalcium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryCalcium);
			}
		}

		public static NSString? DietaryCarbohydrates {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryCarbohydrates);
			}
		}

		public static NSString? DietaryChloride {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryChloride);
			}
		}

		public static NSString? DietaryCholesterol {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryCholesterol);
			}
		}

		public static NSString? DietaryChromium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryChromium);
			}
		}

		public static NSString? DietaryCopper {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryCopper);
			}
		}

		public static NSString? DietaryEnergyConsumed {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryEnergyConsumed);
			}
		}

		public static NSString? DietaryFatMonounsaturated {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFatMonounsaturated);
			}
		}

		public static NSString? DietaryFatPolyunsaturated {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFatPolyunsaturated);
			}
		}

		public static NSString? DietaryFatSaturated {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFatSaturated);
			}
		}

		public static NSString? DietaryFatTotal {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFatTotal);
			}
		}

		public static NSString? DietaryFiber {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFiber);
			}
		}

		public static NSString? DietaryFolate {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryFolate);
			}
		}

		public static NSString? DietaryIodine {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryIodine);
			}
		}

		public static NSString? DietaryIron {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryIron);
			}
		}

		public static NSString? DietaryMagnesium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryMagnesium);
			}
		}

		public static NSString? DietaryManganese {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryManganese);
			}
		}

		public static NSString? DietaryMolybdenum {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryMolybdenum);
			}
		}

		public static NSString? DietaryNiacin {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryNiacin);
			}
		}

		public static NSString? DietaryPantothenicAcid {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryPantothenicAcid);
			}
		}

		public static NSString? DietaryPhosphorus {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryPhosphorus);
			}
		}

		public static NSString? DietaryPotassium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryPotassium);
			}
		}

		public static NSString? DietaryProtein {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryProtein);
			}
		}

		public static NSString? DietaryRiboflavin {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryRiboflavin);
			}
		}

		public static NSString? DietarySelenium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietarySelenium);
			}
		}

		public static NSString? DietarySodium {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietarySodium);
			}
		}

		public static NSString? DietarySugar {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietarySugar);
			}
		}

		public static NSString? DietaryThiamin {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryThiamin);
			}
		}

		public static NSString? DietaryVitaminA {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminA);
			}
		}

		public static NSString? DietaryVitaminB12 {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminB12);
			}
		}

		public static NSString? DietaryVitaminB6 {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminB6);
			}
		}

		public static NSString? DietaryVitaminC {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminC);
			}
		}

		public static NSString? DietaryVitaminD {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminD);
			}
		}

		public static NSString? DietaryVitaminE {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminE);
			}
		}

		public static NSString? DietaryVitaminK {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryVitaminK);
			}
		}

		public static NSString? DietaryWater {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryWater);
			}
		}

		public static NSString? DietaryZinc {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DietaryZinc);
			}
		}

		public static NSString? DistanceCycling {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DistanceCycling);
			}
		}

		[Introduced (PlatformName.WatchOS, 4,2, PlatformArchitecture.All)]
		[Introduced (PlatformName.iOS, 11,2, PlatformArchitecture.All)]
		public static NSString? DistanceDownhillSnowSports {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DistanceDownhillSnowSports);
			}
		}

		public static NSString? DistanceSwimming {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DistanceSwimming);
			}
		}

		public static NSString? DistanceWalkingRunning {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DistanceWalkingRunning);
			}
		}

		public static NSString? DistanceWheelchair {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.DistanceWheelchair);
			}
		}

		public static NSString? ElectrodermalActivity {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.ElectrodermalActivity);
			}
		}

		public static NSString? FlightsClimbed {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.FlightsClimbed);
			}
		}

		public static NSString? ForcedExpiratoryVolume1 {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.ForcedExpiratoryVolume1);
			}
		}

		public static NSString? ForcedVitalCapacity {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.ForcedVitalCapacity);
			}
		}

		public static NSString? HeartRate {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.HeartRate);
			}
		}

		public static NSString? HeartRateVariabilitySdnn {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.HeartRateVariabilitySdnn);
			}
		}

		public static NSString? Height {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.Height);
			}
		}

		public static NSString? InhalerUsage {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.InhalerUsage);
			}
		}

		public static NSString? InsulinDelivery {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.InsulinDelivery);
			}
		}

		public static NSString? LeanBodyMass {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.LeanBodyMass);
			}
		}

		public static NSString? NikeFuel {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.NikeFuel);
			}
		}

		public static NSString? NumberOfTimesFallen {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.NumberOfTimesFallen);
			}
		}

		public static NSString? OxygenSaturation {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.OxygenSaturation);
			}
		}

		public static NSString? PeakExpiratoryFlowRate {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.PeakExpiratoryFlowRate);
			}
		}

		public static NSString? PeripheralPerfusionIndex {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.PeripheralPerfusionIndex);
			}
		}

		public static NSString? PushCount {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.PushCount);
			}
		}

		public static NSString? RespiratoryRate {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.RespiratoryRate);
			}
		}

		public static NSString? RestingHeartRate {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.RestingHeartRate);
			}
		}

		public static NSString? StepCount {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.StepCount);
			}
		}

		public static NSString? SwimmingStrokeCount {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.SwimmingStrokeCount);
			}
		}

		public static NSString? UVExposure {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.UVExposure);
			}
		}

		public static NSString? VO2Max {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.VO2Max);
			}
		}

		public static NSString? WaistCircumference {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.WaistCircumference);
			}
		}

		public static NSString? WalkingHeartRateAverage {
			get {
				return HKQuantityTypeIdentifierExtensions.GetConstant (HKQuantityTypeIdentifier.WalkingHeartRateAverage);
			}
		}
	} /* class HKQuantityTypeIdentifierKey */

	[Obsolete ("Use the 'HKCategoryTypeIdentifier' enum instead.")]
	public unsafe static partial class HKCategoryTypeIdentifierKey  {
		public static NSString? AppleStandHour {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.AppleStandHour);
			}
		}

		public static NSString? CervicalMucusQuality {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.CervicalMucusQuality);
			}
		}

		public static NSString? IntermenstrualBleeding {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.IntermenstrualBleeding);
			}
		}

		public static NSString? MenstrualFlow {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.MenstrualFlow);
			}
		}

		public static NSString? MindfulSession {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.MindfulSession);
			}
		}

		public static NSString? OvulationTestResult {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.OvulationTestResult);
			}
		}

		public static NSString? SexualActivity {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.SexualActivity);
			}
		}

		public static NSString? SleepAnalysis {
			get {
				return HKCategoryTypeIdentifierExtensions.GetConstant (HKCategoryTypeIdentifier.SleepAnalysis);
			}
		}
	} /* class HKCategoryTypeIdentifierKey */

	[Obsolete ("Use the 'HKCharacteristicTypeIdentifier' enum instead.")]
	public unsafe static partial class HKCharacteristicTypeIdentifierKey  {
		public static NSString? BiologicalSex {
			get {
				return HKCharacteristicTypeIdentifierExtensions.GetConstant (HKCharacteristicTypeIdentifier.BiologicalSex);
			}
		}

		public static NSString? BloodType {
			get {
				return HKCharacteristicTypeIdentifierExtensions.GetConstant (HKCharacteristicTypeIdentifier.BloodType);
			}
		}

		public static NSString? DateOfBirth {
			get {
				return HKCharacteristicTypeIdentifierExtensions.GetConstant (HKCharacteristicTypeIdentifier.DateOfBirth);
			}
		}

		public static NSString? FitzpatrickSkinType {
			get {
				return HKCharacteristicTypeIdentifierExtensions.GetConstant (HKCharacteristicTypeIdentifier.FitzpatrickSkinType);
			}
		}

		public static NSString? WheelchairUse {
			get {
				return HKCharacteristicTypeIdentifierExtensions.GetConstant (HKCharacteristicTypeIdentifier.WheelchairUse);
			}
		}
	} /* class HKCharacteristicTypeIdentifierKey */

	[Obsolete ("Use the 'HKCorrelationType' enum instead.")]
	public unsafe static partial class HKCorrelationTypeKey  {
		public static NSString? IdentifierBloodPressure {
			get {
				return HKCorrelationTypeIdentifierExtensions.GetConstant (HKCorrelationTypeIdentifier.BloodPressure);
			}
		}

		public static NSString? IdentifierFood {
			get {
				return HKCorrelationTypeIdentifierExtensions.GetConstant (HKCorrelationTypeIdentifier.Food);
			}
		}
	} /* class HKCorrelationTypeKey */
}

#endif // !NET
