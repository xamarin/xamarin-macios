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
		public static HKQuantityType Create (HKQuantityTypeIdentifier kind)
		{
			return HKObjectType.GetQuantityType (HKQuantityTypeIdentifierExtensions.GetConstant (kind));
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

