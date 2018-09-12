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
			return HKObjectType.GetQuantityType (kind.GetConstant ());
		}
	}

	public partial class HKCategoryType {
		public static HKCategoryType Create (HKCategoryTypeIdentifier kind)
		{
			return HKObjectType.GetCategoryType (kind.GetConstant ());
		}
	}

	public partial class HKCharacteristicType {
		public static HKCharacteristicType Create (HKCharacteristicTypeIdentifier kind)
		{
			return HKObjectType.GetCharacteristicType (kind.GetConstant ());
		}
	}

	public partial class HKCorrelationType {
		public static HKCorrelationType Create (HKCorrelationTypeIdentifier kind)
		{
			return HKObjectType.GetCorrelationType (kind.GetConstant ());
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

