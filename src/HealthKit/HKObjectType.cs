//
// HKObjectType.cs: methods that live technically in HKObjectType, but we are going to put
// in classes where they make more sense (they are factory methods on HKObejctType, taking
// strings, we are going to add factory methods in the relevant classes, that tkae the kind
// you want
//
// Authors:
//  Miguel de Icaza (miguel@xamarin.com
//
// Copyright 2014-2015 Xamarin
//
using System;
using Foundation;
namespace HealthKit
{
	public partial class HKQuantityType {
		static internal NSString ToKey (HKQuantityTypeIdentifier kind)
		{
			switch (kind){
			case HKQuantityTypeIdentifier.BodyMassIndex:
				return HKQuantityTypeIdentifierKey.BodyMassIndex;

			case HKQuantityTypeIdentifier.BodyFatPercentage:
				return HKQuantityTypeIdentifierKey.BodyFatPercentage;				

			case HKQuantityTypeIdentifier.Height:
				return HKQuantityTypeIdentifierKey.Height;				

			case HKQuantityTypeIdentifier.BodyMass:
				return HKQuantityTypeIdentifierKey.BodyMass;				

			case HKQuantityTypeIdentifier.LeanBodyMass:
				return HKQuantityTypeIdentifierKey.LeanBodyMass;				

			case HKQuantityTypeIdentifier.HeartRate:
				return HKQuantityTypeIdentifierKey.HeartRate;				

			case HKQuantityTypeIdentifier.StepCount:
				return HKQuantityTypeIdentifierKey.StepCount;				

			case HKQuantityTypeIdentifier.DistanceWalkingRunning:
				return HKQuantityTypeIdentifierKey.DistanceWalkingRunning;

			case HKQuantityTypeIdentifier.DistanceCycling:
				return HKQuantityTypeIdentifierKey.DistanceCycling;

			case HKQuantityTypeIdentifier.DistanceWheelchair:
				return HKQuantityTypeIdentifierKey.DistanceWheelchair;

			case HKQuantityTypeIdentifier.BasalEnergyBurned:
				return HKQuantityTypeIdentifierKey.BasalEnergyBurned;				

			case HKQuantityTypeIdentifier.ActiveEnergyBurned:
				return HKQuantityTypeIdentifierKey.ActiveEnergyBurned;				

			case HKQuantityTypeIdentifier.FlightsClimbed:
				return HKQuantityTypeIdentifierKey.FlightsClimbed;				

			case HKQuantityTypeIdentifier.NikeFuel:
				return HKQuantityTypeIdentifierKey.NikeFuel;				

			case HKQuantityTypeIdentifier.AppleExerciseTime:
				return HKQuantityTypeIdentifierKey.AppleExerciseTime;

			case HKQuantityTypeIdentifier.PushCount:
				return HKQuantityTypeIdentifierKey.PushCount;

			case HKQuantityTypeIdentifier.DistanceSwimming:
				return HKQuantityTypeIdentifierKey.DistanceSwimming;

			case HKQuantityTypeIdentifier.SwimmingStrokeCount:
				return HKQuantityTypeIdentifierKey.SwimmingStrokeCount;

		// Blood
			case HKQuantityTypeIdentifier.OxygenSaturation:
				return HKQuantityTypeIdentifierKey.OxygenSaturation;				

			case HKQuantityTypeIdentifier.BloodGlucose:
				return HKQuantityTypeIdentifierKey.BloodGlucose;				

			case HKQuantityTypeIdentifier.BloodPressureSystolic:
				return HKQuantityTypeIdentifierKey.BloodPressureSystolic;				

			case HKQuantityTypeIdentifier.BloodPressureDiastolic:
				return HKQuantityTypeIdentifierKey.BloodPressureDiastolic;				

			case HKQuantityTypeIdentifier.BloodAlcoholContent:
				return HKQuantityTypeIdentifierKey.BloodAlcoholContent;				

			case HKQuantityTypeIdentifier.PeripheralPerfusionIndex:
				return HKQuantityTypeIdentifierKey.PeripheralPerfusionIndex;				

		// Miscellaneous
			case HKQuantityTypeIdentifier.ForcedVitalCapacity:
				return HKQuantityTypeIdentifierKey.ForcedVitalCapacity;

			case HKQuantityTypeIdentifier.ForcedExpiratoryVolume1:
				return HKQuantityTypeIdentifierKey.ForcedExpiratoryVolume1;

			case HKQuantityTypeIdentifier.PeakExpiratoryFlowRate:
				return HKQuantityTypeIdentifierKey.PeakExpiratoryFlowRate;

			case HKQuantityTypeIdentifier.NumberOfTimesFallen:
				return HKQuantityTypeIdentifierKey.NumberOfTimesFallen;				

			case HKQuantityTypeIdentifier.ElectrodermalActivity:
				return HKQuantityTypeIdentifierKey.ElectrodermalActivity;

			case HKQuantityTypeIdentifier.InhalerUsage:
				return HKQuantityTypeIdentifierKey.InhalerUsage;				

			case HKQuantityTypeIdentifier.RespiratoryRate:
				return HKQuantityTypeIdentifierKey.RespiratoryRate;				

			case HKQuantityTypeIdentifier.BodyTemperature:
				return HKQuantityTypeIdentifierKey.BodyTemperature;				

		// Nutrition
			case HKQuantityTypeIdentifier.DietaryFatTotal:
				return HKQuantityTypeIdentifierKey.DietaryFatTotal;				

			case HKQuantityTypeIdentifier.DietaryFatPolyunsaturated:
				return HKQuantityTypeIdentifierKey.DietaryFatPolyunsaturated;				

			case HKQuantityTypeIdentifier.DietaryFatMonounsaturated:
				return HKQuantityTypeIdentifierKey.DietaryFatMonounsaturated;				

			case HKQuantityTypeIdentifier.DietaryFatSaturated:
				return HKQuantityTypeIdentifierKey.DietaryFatSaturated;				

			case HKQuantityTypeIdentifier.DietaryCholesterol:
				return HKQuantityTypeIdentifierKey.DietaryCholesterol;				

			case HKQuantityTypeIdentifier.DietarySodium:
				return HKQuantityTypeIdentifierKey.DietarySodium;				

			case HKQuantityTypeIdentifier.DietaryCarbohydrates:
				return HKQuantityTypeIdentifierKey.DietaryCarbohydrates;				

			case HKQuantityTypeIdentifier.DietaryFiber:
				return HKQuantityTypeIdentifierKey.DietaryFiber;				

			case HKQuantityTypeIdentifier.DietarySugar:
				return HKQuantityTypeIdentifierKey.DietarySugar;				

			case HKQuantityTypeIdentifier.DietaryEnergyConsumed:
				return HKQuantityTypeIdentifierKey.DietaryEnergyConsumed;				

			case HKQuantityTypeIdentifier.DietaryProtein:
				return HKQuantityTypeIdentifierKey.DietaryProtein;				

			case HKQuantityTypeIdentifier.DietaryVitaminA:
				return HKQuantityTypeIdentifierKey.DietaryVitaminA;				

			case HKQuantityTypeIdentifier.DietaryVitaminB6:
				return HKQuantityTypeIdentifierKey.DietaryVitaminB6;				

			case HKQuantityTypeIdentifier.DietaryVitaminB12:
				return HKQuantityTypeIdentifierKey.DietaryVitaminB12;				

			case HKQuantityTypeIdentifier.DietaryVitaminC:
				return HKQuantityTypeIdentifierKey.DietaryVitaminC;				

			case HKQuantityTypeIdentifier.DietaryVitaminD:
				return HKQuantityTypeIdentifierKey.DietaryVitaminD;				

			case HKQuantityTypeIdentifier.DietaryVitaminE:
				return HKQuantityTypeIdentifierKey.DietaryVitaminE;				

			case HKQuantityTypeIdentifier.DietaryVitaminK:
				return HKQuantityTypeIdentifierKey.DietaryVitaminK;				

			case HKQuantityTypeIdentifier.DietaryCalcium:
				return HKQuantityTypeIdentifierKey.DietaryCalcium;				

			case HKQuantityTypeIdentifier.DietaryIron:
				return HKQuantityTypeIdentifierKey.DietaryIron;				

			case HKQuantityTypeIdentifier.DietaryThiamin:
				return HKQuantityTypeIdentifierKey.DietaryThiamin;				

			case HKQuantityTypeIdentifier.DietaryRiboflavin:
				return HKQuantityTypeIdentifierKey.DietaryRiboflavin;				

			case HKQuantityTypeIdentifier.DietaryNiacin:
				return HKQuantityTypeIdentifierKey.DietaryNiacin;				

			case HKQuantityTypeIdentifier.DietaryFolate:
				return HKQuantityTypeIdentifierKey.DietaryFolate;				

			case HKQuantityTypeIdentifier.DietaryBiotin:
				return HKQuantityTypeIdentifierKey.DietaryBiotin;				

			case HKQuantityTypeIdentifier.DietaryPantothenicAcid:
				return HKQuantityTypeIdentifierKey.DietaryPantothenicAcid;				

			case HKQuantityTypeIdentifier.DietaryPhosphorus:
				return HKQuantityTypeIdentifierKey.DietaryPhosphorus;				

			case HKQuantityTypeIdentifier.DietaryIodine:
				return HKQuantityTypeIdentifierKey.DietaryIodine;				

			case HKQuantityTypeIdentifier.DietaryMagnesium:
				return HKQuantityTypeIdentifierKey.DietaryMagnesium;				

			case HKQuantityTypeIdentifier.DietaryZinc:
				return HKQuantityTypeIdentifierKey.DietaryZinc;				

			case HKQuantityTypeIdentifier.DietarySelenium:
				return HKQuantityTypeIdentifierKey.DietarySelenium;				

			case HKQuantityTypeIdentifier.DietaryCopper:
				return HKQuantityTypeIdentifierKey.DietaryCopper;				

			case HKQuantityTypeIdentifier.DietaryManganese:
				return HKQuantityTypeIdentifierKey.DietaryManganese;				

			case HKQuantityTypeIdentifier.DietaryChromium:
				return HKQuantityTypeIdentifierKey.DietaryChromium;				

			case HKQuantityTypeIdentifier.DietaryMolybdenum:
				return HKQuantityTypeIdentifierKey.DietaryMolybdenum;				

			case HKQuantityTypeIdentifier.DietaryChloride:
				return HKQuantityTypeIdentifierKey.DietaryChloride;				

			case HKQuantityTypeIdentifier.DietaryPotassium:
				return HKQuantityTypeIdentifierKey.DietaryPotassium;

			case HKQuantityTypeIdentifier.DietaryCaffeine:
				return HKQuantityTypeIdentifierKey.DietaryCaffeine;

			case HKQuantityTypeIdentifier.BasalBodyTemperature:
				return HKQuantityTypeIdentifierKey.BasalBodyTemperature;

			case HKQuantityTypeIdentifier.DietaryWater:
				return HKQuantityTypeIdentifierKey.DietaryWater;

			case HKQuantityTypeIdentifier.UVExposure:
				return HKQuantityTypeIdentifierKey.UVExposure;

			case HKQuantityTypeIdentifier.WaistCircumference:
				return HKQuantityTypeIdentifierKey.WaistCircumference;

			case HKQuantityTypeIdentifier.VO2Max:
				return HKQuantityTypeIdentifierKey.VO2Max;
			}
			return null;
		}
		
		public static HKQuantityType Create (HKQuantityTypeIdentifier kind)
		{
			return HKObjectType.GetQuantityType (ToKey (kind));
		}
	}

	public partial class HKCategoryType {
		static internal NSString ToKey (HKCategoryTypeIdentifier kind)
		{
			switch (kind){
			case HKCategoryTypeIdentifier.SleepAnalysis:
				return HKCategoryTypeIdentifierKey.SleepAnalysis;
			case HKCategoryTypeIdentifier.AppleStandHour:
				return HKCategoryTypeIdentifierKey.AppleStandHour;
			case HKCategoryTypeIdentifier.CervicalMucusQuality:
				return HKCategoryTypeIdentifierKey.CervicalMucusQuality;
			case HKCategoryTypeIdentifier.OvulationTestResult:
				return HKCategoryTypeIdentifierKey.OvulationTestResult;
			case HKCategoryTypeIdentifier.MenstrualFlow:
				return HKCategoryTypeIdentifierKey.MenstrualFlow;
			case HKCategoryTypeIdentifier.IntermenstrualBleeding:
				return HKCategoryTypeIdentifierKey.IntermenstrualBleeding;
			case HKCategoryTypeIdentifier.SexualActivity:
				return HKCategoryTypeIdentifierKey.SexualActivity;
			case HKCategoryTypeIdentifier.MindfulSession:
				return HKCategoryTypeIdentifierKey.MindfulSession;
			}
			return null;
		}

		public static HKCategoryType Create (HKCategoryTypeIdentifier kind)
		{
			return HKObjectType.GetCategoryType (ToKey (kind));
		}
	}

	public partial class HKCharacteristicType {
		static internal NSString ToKey (HKCharacteristicTypeIdentifier kind)
		{
			switch (kind){
			case HKCharacteristicTypeIdentifier.BiologicalSex:
				return HKCharacteristicTypeIdentifierKey.BiologicalSex;
			case HKCharacteristicTypeIdentifier.BloodType:
				return HKCharacteristicTypeIdentifierKey.BloodType;
			case HKCharacteristicTypeIdentifier.DateOfBirth:
				return HKCharacteristicTypeIdentifierKey.DateOfBirth;
			case HKCharacteristicTypeIdentifier.FitzpatrickSkinType:
				return HKCharacteristicTypeIdentifierKey.FitzpatrickSkinType;
			case HKCharacteristicTypeIdentifier.WheelchairUse:
				return HKCharacteristicTypeIdentifierKey.WheelchairUse;
			}
			return null;
		}
		
		public static HKCharacteristicType Create (HKCharacteristicTypeIdentifier kind)
		{
			return HKObjectType.GetCharacteristicType (ToKey (kind));
		}
	}

	public partial class HKCorrelationType {
		static internal NSString ToKey (HKCorrelationTypeIdentifier kind)
		{
			switch (kind) {
			case HKCorrelationTypeIdentifier.BloodPressure:
				return HKCorrelationTypeKey.IdentifierBloodPressure;
			case HKCorrelationTypeIdentifier.Food:
				return HKCorrelationTypeKey.IdentifierFood;
			}
			return null;
		}

		public static HKCorrelationType Create (HKCorrelationTypeIdentifier kind)
		{
			return HKObjectType.GetCorrelationType (ToKey (kind));
		}
	}

#if !WATCH
	public partial class HKDocumentType {
		public static HKDocumentType Create (HKDocumentTypeIdentifier kind)
		{
			return HKObjectType._GetDocumentType (kind.GetConstant ());
		}
	}
#endif
}

