//
// MetricKit C# bindings
//
// Authors:
//	Manuel de la Pena Saenz <mandel@microsoft.com>
//
// Copyright 2019 Microsoft Corporation All rights reserved.
//

using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace MetricKit {

	interface NSUnitDuration : NSUnit { }
	interface NSUnitInformationStorage : NSUnit { }

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MXMetric : NSSecureCoding {
		[Export ("JSONRepresentation")]
		NSData JsonRepresentation { get; }

		[Export ("DictionaryRepresentation")]
		NSDictionary DictionaryRepresentation { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric), Name = "MXCPUMetric")]
	interface MXCpuMetric {
		[Export ("cumulativeCPUTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeCpuTime { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric), Name = "MXGPUMetric")]
	interface MXGpuMetric {
		[Export ("cumulativeGPUTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeGpuTime { get; }
	}

	// NSUnit is added as a parent to ensure that the intermediate tmp dll can be comppiled
	// since at this stage the compiler does not know about the inheritance of NSDimension.
	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(NSDimension))]
	[DisableDefaultCtor]
	interface MXUnitSignalBars : NSUnit {

		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("bars", ArgumentSemantic.Copy)]
		MXUnitSignalBars Bars { get; }
	}

	// NSUnit is added as a parent to ensure that the intermediate tmp dll can be comppiled
	// since at this stage the compiler does not know about the inheritance of NSDimension.
	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(NSDimension))]
	[DisableDefaultCtor]
	interface MXUnitAveragePixelLuminance : NSUnit {

		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("apl", ArgumentSemantic.Copy)]
		MXUnitAveragePixelLuminance Apl { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MXHistogramBucket<UnitType> : NSSecureCoding
		where UnitType : NSUnit {
		[Export ("bucketStart", ArgumentSemantic.Strong)]
		NSMeasurement<UnitType> BucketStart { get; }

		[Export ("bucketEnd", ArgumentSemantic.Strong)]
		NSMeasurement<UnitType> BucketEnd { get; }

		[Export ("bucketCount")]
		nuint BucketCount { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MXHistogram<UnitType> : NSSecureCoding
		where UnitType : NSUnit {
		[Export ("totalBucketCount")]
		nuint TotalBucketCount { get; }

		[Export ("bucketEnumerator", ArgumentSemantic.Strong)]
		NSEnumerator<MXHistogramBucket<UnitType>> BucketEnumerator { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric))]
	interface MXCellularConditionMetric {
		[Export ("histogrammedCellularConditionTime", ArgumentSemantic.Strong)]
		MXHistogram<MXUnitSignalBars> HistogrammedCellularConditionTime { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MXMetaData : NSSecureCoding {
		[Export ("regionFormat", ArgumentSemantic.Strong)]
		string RegionFormat { get; }

		[Export ("osVersion", ArgumentSemantic.Strong)]
		string OSVersion { get; }

		[Export ("deviceType", ArgumentSemantic.Strong)]
		string DeviceType { get; }

		[Export ("applicationBuildVersion", ArgumentSemantic.Strong)]
		string ApplicationBuildVersion { get; }

		[Export ("JSONRepresentation")]
		NSData JsonRepresentation { get; }

		[Export ("DictionaryRepresentation")]
		NSDictionary DictionaryRepresentation { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric))]
	interface MXAppRunTimeMetric {
		[Export ("cumulativeForegroundTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeForegroundTime { get; }

		[Export ("cumulativeBackgroundTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeBackgroundTime { get; }

		[Export ("cumulativeBackgroundAudioTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeBackgroundAudioTime { get; }

		[Export ("cumulativeBackgroundLocationTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeBackgroundLocationTime { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric))]
	interface MXLocationActivityMetric {
		[Export ("cumulativeBestAccuracyTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeBestAccuracyTime { get; }

		[Export ("cumulativeBestAccuracyForNavigationTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeBestAccuracyForNavigationTime { get; }

		[Export ("cumulativeNearestTenMetersAccuracyTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeNearestTenMetersAccuracyTime { get; }

		[Export ("cumulativeHundredMetersAccuracyTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeHundredMetersAccuracyTime { get; }

		[Export ("cumulativeKilometerAccuracyTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeKilometerAccuracyTime { get; }

		[Export ("cumulativeThreeKilometersAccuracyTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeThreeKilometersAccuracyTime { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric))]
	interface MXNetworkTransferMetric {
		[Export ("cumulativeWifiUpload", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> CumulativeWifiUpload { get; }

		[Export ("cumulativeWifiDownload", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> CumulativeWifiDownload { get; }

		[Export ("cumulativeCellularUpload", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> CumulativeCellularUpload { get; }

		[Export ("cumulativeCellularDownload", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> CumulativeCellularDownload { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric))]
	interface MXAppLaunchMetric {
		[Export ("histogrammedTimeToFirstDraw", ArgumentSemantic.Strong)]
		MXHistogram<NSUnitDuration> HistogrammedTimeToFirstDraw { get; }

		[Export ("histogrammedApplicationResumeTime", ArgumentSemantic.Strong)]
		MXHistogram<NSUnitDuration> HistogrammedApplicationResumeTime { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric))]
	interface MXAppResponsivenessMetric {
		[Export ("histogrammedApplicationHangTime", ArgumentSemantic.Strong)]
		MXHistogram<NSUnitDuration> HistogrammedApplicationHangTime { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric))]
	interface MXDiskIOMetric {
		[Export ("cumulativeLogicalWrites", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> CumulativeLogicalWrites { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MXAverage<UnitType> : NSSecureCoding
		where UnitType : NSUnit {
		[Export ("averageMeasurement", ArgumentSemantic.Strong)]
		NSMeasurement<UnitType> AverageMeasurement { get; }

		[Export ("sampleCount")]
		nint SampleCount { get; }

		[Export ("standardDeviation")]
		double StandardDeviation { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric))]
	interface MXMemoryMetric {
		[Export ("peakMemoryUsage", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> PeakMemoryUsage { get; }

		[Export ("averageSuspendedMemory", ArgumentSemantic.Strong)]
		MXAverage<NSUnitInformationStorage> AverageSuspendedMemory { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric))]
	interface MXDisplayMetric {
		[NullAllowed, Export ("averagePixelLuminance", ArgumentSemantic.Strong)]
		MXAverage<MXUnitAveragePixelLuminance> AveragePixelLuminance { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MXSignpostIntervalData : NSSecureCoding {
		[Export ("histogrammedSignpostDuration", ArgumentSemantic.Strong)]
		MXHistogram<NSUnitDuration> HistogrammedSignpostDuration { get; }

		[NullAllowed, Export ("cumulativeCPUTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeCpuTime { get; }

		[NullAllowed, Export ("averageMemory", ArgumentSemantic.Strong)]
		MXAverage<NSUnitInformationStorage> AverageMemory { get; }

		[NullAllowed, Export ("cumulativeLogicalWrites", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> CumulativeLogicalWrites { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(MXMetric))]
	interface MXSignpostMetric {
		[Export ("signpostName", ArgumentSemantic.Strong)]
		string SignpostName { get; }

		[Export ("signpostCategory", ArgumentSemantic.Strong)]
		string SignpostCategory { get; }

		[NullAllowed, Export ("signpostIntervalData", ArgumentSemantic.Strong)]
		MXSignpostIntervalData SignpostIntervalData { get; }

		[Export ("totalCount")]
		nuint TotalCount { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface MXMetricPayload : NSSecureCoding {
		[Export ("latestApplicationVersion", ArgumentSemantic.Strong)]
		string LatestApplicationVersion { get; }

		[Export ("includesMultipleApplicationVersions")]
		bool IncludesMultipleApplicationVersions { get; }

		[Export ("timeStampBegin", ArgumentSemantic.Strong)]
		NSDate TimeStampBegin { get; }

		[Export ("timeStampEnd", ArgumentSemantic.Strong)]
		NSDate TimeStampEnd { get; }

		[NullAllowed, Export ("cpuMetrics", ArgumentSemantic.Strong)]
		MXCpuMetric CpuMetrics { get; }

		[NullAllowed, Export ("gpuMetrics", ArgumentSemantic.Strong)]
		MXGpuMetric GpuMetrics { get; }

		[NullAllowed, Export ("cellularConditionMetrics", ArgumentSemantic.Strong)]
		MXCellularConditionMetric CellularConditionMetrics { get; }

		[NullAllowed, Export ("applicationTimeMetrics", ArgumentSemantic.Strong)]
		MXAppRunTimeMetric ApplicationTimeMetrics { get; }

		[NullAllowed, Export ("locationActivityMetrics", ArgumentSemantic.Strong)]
		MXLocationActivityMetric LocationActivityMetrics { get; }

		[NullAllowed, Export ("networkTransferMetrics", ArgumentSemantic.Strong)]
		MXNetworkTransferMetric NetworkTransferMetrics { get; }

		[NullAllowed, Export ("applicationLaunchMetrics", ArgumentSemantic.Strong)]
		MXAppLaunchMetric ApplicationLaunchMetrics { get; }

		[NullAllowed, Export ("applicationResponsivenessMetrics", ArgumentSemantic.Strong)]
		MXAppResponsivenessMetric ApplicationResponsivenessMetrics { get; }

		[NullAllowed, Export ("diskIOMetrics", ArgumentSemantic.Strong)]
		MXDiskIOMetric DiskIOMetrics { get; }

		[NullAllowed, Export ("memoryMetrics", ArgumentSemantic.Strong)]
		MXMemoryMetric MemoryMetrics { get; }

		[NullAllowed, Export ("displayMetrics", ArgumentSemantic.Strong)]
		MXDisplayMetric DisplayMetrics { get; }

		[NullAllowed, Export ("signpostMetrics", ArgumentSemantic.Strong)]
		MXSignpostMetric[] SignpostMetrics { get; }

		[NullAllowed, Export ("metaData", ArgumentSemantic.Strong)]
		MXMetaData MetaData { get; }

		[Export ("JSONRepresentation")]
		NSData JsonRepresentation { get; }

		[Export ("DictionaryRepresentation")]
		NSDictionary DictionaryRepresentation { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MXMetricManager {
		[Export ("pastPayloads", ArgumentSemantic.Strong)]
		MXMetricPayload[] PastPayloads { get; }

		[Static]
		[Export ("sharedManager", ArgumentSemantic.Strong)]
		MXMetricManager SharedManager { get; }

		[Export ("addSubscriber:")]
		void Add (IMXMetricManagerSubscriber subscriber);

		[Export ("removeSubscriber:")]
		void Remove (IMXMetricManagerSubscriber subscriber);
	}

	interface IMXMetricManagerSubscriber { }

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[Protocol]
	interface MXMetricManagerSubscriber {
		[Abstract]
		[Export ("didReceiveMetricPayloads:")]
		void DidReceiveMetricPayloads (MXMetricPayload[] payloads);
	}
}