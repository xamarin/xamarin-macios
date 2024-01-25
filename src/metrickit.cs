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

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MetricKit {

	interface NSUnitDuration : NSUnit { }
	interface NSUnitInformationStorage : NSUnit { }

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MXMetric : NSSecureCoding {
		[Export ("JSONRepresentation")]
		NSData JsonRepresentation { get; }

		[Internal]
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("DictionaryRepresentation")]
		NSDictionary _DictionaryRepresentation13 { get; }

		[Internal]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("dictionaryRepresentation")]
		NSDictionary _DictionaryRepresentation14 { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric), Name = "MXCPUMetric")]
	interface MXCpuMetric {
		[Export ("cumulativeCPUTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeCpuTime { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("cumulativeCPUInstructions", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnit> CumulativeCpuInstructions { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric), Name = "MXGPUMetric")]
	interface MXGpuMetric {
		[Export ("cumulativeGPUTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeGpuTime { get; }
	}

	// NSUnit is added as a parent to ensure that the intermediate tmp dll can be compiled
	// since at this stage the compiler does not know about the inheritance of NSDimension.
	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	[DisableDefaultCtor]
	interface MXUnitSignalBars : NSUnit {

		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("bars", ArgumentSemantic.Copy)]
		MXUnitSignalBars Bars { get; }
	}

	// NSUnit is added as a parent to ensure that the intermediate tmp dll can be compiled
	// since at this stage the compiler does not know about the inheritance of NSDimension.
	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	[DisableDefaultCtor]
	interface MXUnitAveragePixelLuminance : NSUnit {

		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("apl", ArgumentSemantic.Copy)]
		MXUnitAveragePixelLuminance Apl { get; }
	}

	[NoWatch, NoTV, Mac (12, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MXHistogramBucket<UnitType> : NSSecureCoding
		where UnitType : NSUnit {
		[Export ("bucketStart", ArgumentSemantic.Strong)]
		NSMeasurement<UnitType> BucketStart { get; }

		[Export ("bucketEnd", ArgumentSemantic.Strong)]
		NSMeasurement<UnitType> BucketEnd { get; }

		[Export ("bucketCount")]
		nuint BucketCount { get; }
	}

	[NoWatch, NoTV, Mac (12, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MXHistogram<UnitType> : NSSecureCoding
		where UnitType : NSUnit {
		[Export ("totalBucketCount")]
		nuint TotalBucketCount { get; }

		[Export ("bucketEnumerator", ArgumentSemantic.Strong)]
		NSEnumerator<MXHistogramBucket<UnitType>> BucketEnumerator { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric))]
	interface MXCellularConditionMetric {
		[Export ("histogrammedCellularConditionTime", ArgumentSemantic.Strong)]
		MXHistogram<MXUnitSignalBars> HistogrammedCellularConditionTime { get; }
	}

	[NoWatch, NoTV, Mac (12, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
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

		[NoMac]
		[Internal]
		[Deprecated (PlatformName.iOS, 14, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("DictionaryRepresentation")]
		NSDictionary _DictionaryRepresentation13 { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("dictionaryRepresentation")]
#if MONOMAC
 		NSDictionary DictionaryRepresentation { get; }
#else
		[Internal]
		NSDictionary _DictionaryRepresentation14 { get; }
#endif

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("platformArchitecture", ArgumentSemantic.Strong)]
		string PlatformArchitecture { get; }

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("lowPowerModeEnabled")]
		bool LowPowerModeEnabled { get; }

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("isTestFlightApp")]
		bool IsTestFlightApp { get; }

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("pid")]
		int Pid { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric))]
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

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric))]
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

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric))]
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

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric))]
	interface MXAppLaunchMetric {
		[Export ("histogrammedTimeToFirstDraw", ArgumentSemantic.Strong)]
		MXHistogram<NSUnitDuration> HistogrammedTimeToFirstDraw { get; }

		[Export ("histogrammedApplicationResumeTime", ArgumentSemantic.Strong)]
		MXHistogram<NSUnitDuration> HistogrammedApplicationResumeTime { get; }

		[NoWatch, NoTV, NoMac, iOS (15, 2), MacCatalyst (15, 2)]
		[Export ("histogrammedOptimizedTimeToFirstDraw", ArgumentSemantic.Strong)]
		MXHistogram<NSUnitDuration> HistogrammedOptimizedTimeToFirstDraw { get; }

		[NoMac, iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Export ("histogrammedExtendedLaunch", ArgumentSemantic.Strong)]
		MXHistogram<NSUnitDuration> HistogrammedExtendedLaunch { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric))]
	interface MXAppResponsivenessMetric {
		[Export ("histogrammedApplicationHangTime", ArgumentSemantic.Strong)]
		MXHistogram<NSUnitDuration> HistogrammedApplicationHangTime { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric))]
	interface MXDiskIOMetric {
		[Export ("cumulativeLogicalWrites", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> CumulativeLogicalWrites { get; }
	}

	[NoWatch, NoTV, Mac (12, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
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

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric))]
	interface MXMemoryMetric {
		[Export ("peakMemoryUsage", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> PeakMemoryUsage { get; }

		[Export ("averageSuspendedMemory", ArgumentSemantic.Strong)]
		MXAverage<NSUnitInformationStorage> AverageSuspendedMemory { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric))]
	interface MXDisplayMetric {
		[NullAllowed, Export ("averagePixelLuminance", ArgumentSemantic.Strong)]
		MXAverage<MXUnitAveragePixelLuminance> AveragePixelLuminance { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface MXSignpostIntervalData : NSSecureCoding {
		[Export ("histogrammedSignpostDuration", ArgumentSemantic.Strong)]
		MXHistogram<NSUnitDuration> HistogrammedSignpostDuration { get; }

		[NullAllowed, Export ("cumulativeCPUTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> CumulativeCpuTime { get; }

		[NullAllowed, Export ("averageMemory", ArgumentSemantic.Strong)]
		MXAverage<NSUnitInformationStorage> AverageMemory { get; }

		[NullAllowed, Export ("cumulativeLogicalWrites", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> CumulativeLogicalWrites { get; }

		[NullAllowed]
		[NoWatch, NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("cumulativeHitchTimeRatio", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnit> CumulativeHitchTimeRatio { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (MXMetric))]
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

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
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
		MXSignpostMetric [] SignpostMetrics { get; }

		[NullAllowed, Export ("metaData", ArgumentSemantic.Strong)]
		MXMetaData MetaData { get; }

		[Export ("JSONRepresentation")]
		NSData JsonRepresentation { get; }

		[Internal]
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("DictionaryRepresentation")]
		NSDictionary _DictionaryRepresentation13 { get; }

		[Internal]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("dictionaryRepresentation")]
		NSDictionary _DictionaryRepresentation14 { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("animationMetrics", ArgumentSemantic.Strong)]
		MXAnimationMetric AnimationMetrics { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("applicationExitMetrics", ArgumentSemantic.Strong)]
		MXAppExitMetric ApplicationExitMetrics { get; }
	}

	[NoWatch, NoTV, Mac (12, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MXMetricManager {
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("pastPayloads", ArgumentSemantic.Strong)]
		MXMetricPayload [] PastPayloads { get; }

		[Static]
		[Export ("sharedManager", ArgumentSemantic.Strong)]
		MXMetricManager SharedManager { get; }

		[Export ("addSubscriber:")]
		void Add (IMXMetricManagerSubscriber subscriber);

		[Export ("removeSubscriber:")]
		void Remove (IMXMetricManagerSubscriber subscriber);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("pastDiagnosticPayloads", ArgumentSemantic.Strong)]
		MXDiagnosticPayload [] PastDiagnosticPayloads { get; }

		[Static]
		[Internal]
		[Export ("makeLogHandleWithCategory:")]
		IntPtr /* os_log_t */ _MakeLogHandle (NSString category);

		[NoMac, iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Static]
		[Export ("extendLaunchMeasurementForTaskID:error:")]
		bool ExtendLaunchMeasurement (string taskId, [NullAllowed] out NSError error);

		[NoMac, iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Static]
		[Export ("finishExtendedLaunchMeasurementForTaskID:error:")]
		bool FinishExtendedLaunchMeasurement (string taskId, [NullAllowed] out NSError error);
	}

	interface IMXMetricManagerSubscriber { }

	[NoWatch, NoTV, Mac (12, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface MXMetricManagerSubscriber {
#if !NET
		[Abstract]
#endif
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("didReceiveMetricPayloads:")]
		void DidReceiveMetricPayloads (MXMetricPayload [] payloads);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("didReceiveDiagnosticPayloads:")]
		void DidReceiveDiagnosticPayloads (MXDiagnosticPayload [] payloads);
	}

	[NoWatch, NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MXMetric))]
	[DisableDefaultCtor]
	interface MXAnimationMetric {

		[Export ("scrollHitchTimeRatio", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnit> ScrollHitchTimeRatio { get; }
	}

	[NoWatch, NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MXMetric))]
	[DisableDefaultCtor]
	interface MXAppExitMetric {

		[Export ("foregroundExitData", ArgumentSemantic.Strong)]
		MXForegroundExitData ForegroundExitData { get; }

		[Export ("backgroundExitData", ArgumentSemantic.Strong)]
		MXBackgroundExitData BackgroundExitData { get; }
	}

	[NoWatch, NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MXBackgroundExitData : NSSecureCoding {

		[Export ("cumulativeNormalAppExitCount")]
		nuint CumulativeNormalAppExitCount { get; }

		[Export ("cumulativeMemoryResourceLimitExitCount")]
		nuint CumulativeMemoryResourceLimitExitCount { get; }

		[Export ("cumulativeCPUResourceLimitExitCount")]
		nuint CumulativeCpuResourceLimitExitCount { get; }

		[Export ("cumulativeMemoryPressureExitCount")]
		nuint CumulativeMemoryPressureExitCount { get; }

		[Export ("cumulativeBadAccessExitCount")]
		nuint CumulativeBadAccessExitCount { get; }

		[Export ("cumulativeAbnormalExitCount")]
		nuint CumulativeAbnormalExitCount { get; }

		[Export ("cumulativeIllegalInstructionExitCount")]
		nuint CumulativeIllegalInstructionExitCount { get; }

		[Export ("cumulativeAppWatchdogExitCount")]
		nuint CumulativeAppWatchdogExitCount { get; }

		[Export ("cumulativeSuspendedWithLockedFileExitCount")]
		nuint CumulativeSuspendedWithLockedFileExitCount { get; }

		[Export ("cumulativeBackgroundTaskAssertionTimeoutExitCount")]
		nuint CumulativeBackgroundTaskAssertionTimeoutExitCount { get; }
	}

	[NoWatch, NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MXForegroundExitData : NSSecureCoding {

		[Export ("cumulativeNormalAppExitCount")]
		nuint CumulativeNormalAppExitCount { get; }

		[Export ("cumulativeMemoryResourceLimitExitCount")]
		nuint CumulativeMemoryResourceLimitExitCount { get; }

		[Export ("cumulativeBadAccessExitCount")]
		nuint CumulativeBadAccessExitCount { get; }

		[Export ("cumulativeAbnormalExitCount")]
		nuint CumulativeAbnormalExitCount { get; }

		[Export ("cumulativeIllegalInstructionExitCount")]
		nuint CumulativeIllegalInstructionExitCount { get; }

		[Export ("cumulativeAppWatchdogExitCount")]
		nuint CumulativeAppWatchdogExitCount { get; }
	}

	[NoWatch, NoTV, Mac (12, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MXCallStackTree : NSSecureCoding {

		[Export ("JSONRepresentation")]
		NSData JsonRepresentation { get; }
	}

	[NoWatch, NoTV, Mac (12, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MXDiagnostic), Name = "MXCPUExceptionDiagnostic")]
	[DisableDefaultCtor]
	interface MXCpuExceptionDiagnostic {

		[Export ("callStackTree", ArgumentSemantic.Strong)]
		MXCallStackTree CallStackTree { get; }

		[Export ("totalCPUTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> TotalCpuTime { get; }

		[Export ("totalSampledTime", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> TotalSampledTime { get; }
	}

	[NoWatch, NoTV, Mac (12, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MXDiagnostic))]
	[DisableDefaultCtor]
	interface MXCrashDiagnostic {

		[Export ("callStackTree", ArgumentSemantic.Strong)]
		MXCallStackTree CallStackTree { get; }

		[NullAllowed, Export ("terminationReason", ArgumentSemantic.Strong)]
		string TerminationReason { get; }

		[NullAllowed, Export ("virtualMemoryRegionInfo", ArgumentSemantic.Strong)]
		string VirtualMemoryRegionInfo { get; }

		// exception_type_t -> int (exception_types.h)
		[BindAs (typeof (int?))]
		[NullAllowed, Export ("exceptionType", ArgumentSemantic.Strong)]
		NSNumber ExceptionType { get; }

		// mach_exception_code_t -> mach_exception_data_type_t -> int64_t (exception_types.h)
		[BindAs (typeof (long?))]
		[NullAllowed, Export ("exceptionCode", ArgumentSemantic.Strong)]
		NSNumber ExceptionCode { get; }

		// signal number (various structs) always an `int` (signal.h)
		[BindAs (typeof (int?))]
		[NullAllowed, Export ("signal", ArgumentSemantic.Strong)]
		NSNumber Signal { get; }

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("exceptionReason", ArgumentSemantic.Strong)]
		MXCrashDiagnosticObjectiveCExceptionReason ExceptionReason { get; }
	}

	[NoWatch, NoTV, Mac (12, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MXDiagnostic : NSSecureCoding {

		[Export ("metaData", ArgumentSemantic.Strong)]
		MXMetaData MetaData { get; }

		[Export ("applicationVersion", ArgumentSemantic.Strong)]
		string ApplicationVersion { get; }

		[Export ("JSONRepresentation")]
		NSData JsonRepresentation { get; }

		[Export ("dictionaryRepresentation")]
		NSDictionary DictionaryRepresentation { get; }

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("signpostData", ArgumentSemantic.Strong)]
		MXSignpostRecord [] SignpostData { get; }
	}

	// @interface MXAppLaunchDiagnostic : MXDiagnostic
	[NoMac, iOS (16, 0), Mac (16, 0), NoWatch, NoTV]
	[MacCatalyst (16, 0)]
	[BaseType (typeof (MXDiagnostic))]
	[DisableDefaultCtor]
	interface MXAppLaunchDiagnostic {
		[Export ("callStackTree", ArgumentSemantic.Strong)]
		MXCallStackTree CallStackTree { get; }

		[Export ("launchDuration", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> LaunchDuration { get; }
	}

	[NoWatch, NoTV, Mac (12, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MXDiagnosticPayload : NSSecureCoding {

		[NullAllowed, Export ("cpuExceptionDiagnostics", ArgumentSemantic.Strong)]
		MXCpuExceptionDiagnostic [] CpuExceptionDiagnostics { get; }

		[NullAllowed, Export ("diskWriteExceptionDiagnostics", ArgumentSemantic.Strong)]
		MXDiskWriteExceptionDiagnostic [] DiskWriteExceptionDiagnostics { get; }

		[NullAllowed, Export ("hangDiagnostics", ArgumentSemantic.Strong)]
		MXHangDiagnostic [] HangDiagnostics { get; }

		[NoMac, iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[NullAllowed, Export ("appLaunchDiagnostics", ArgumentSemantic.Strong)]
		MXAppLaunchDiagnostic [] AppLaunchDiagnostics { get; }

		[NullAllowed, Export ("crashDiagnostics", ArgumentSemantic.Strong)]
		MXCrashDiagnostic [] CrashDiagnostics { get; }

		[Export ("timeStampBegin", ArgumentSemantic.Strong)]
		NSDate TimeStampBegin { get; }

		[Export ("timeStampEnd", ArgumentSemantic.Strong)]
		NSDate TimeStampEnd { get; }

		[Export ("JSONRepresentation")]
		NSData JsonRepresentation { get; }

		[Export ("dictionaryRepresentation")]
		NSDictionary DictionaryRepresentation { get; }
	}

	[NoWatch, NoTV, Mac (12, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MXDiagnostic))]
	[DisableDefaultCtor]
	interface MXDiskWriteExceptionDiagnostic {

		[Export ("callStackTree", ArgumentSemantic.Strong)]
		MXCallStackTree CallStackTree { get; }

		[Export ("totalWritesCaused", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitInformationStorage> TotalWritesCaused { get; }
	}

	[NoWatch, NoTV, Mac (12, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MXDiagnostic))]
	[DisableDefaultCtor]
	interface MXHangDiagnostic {

		[Export ("callStackTree", ArgumentSemantic.Strong)]
		MXCallStackTree CallStackTree { get; }

		[Export ("hangDuration", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitDuration> HangDuration { get; }
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface MXSignpostRecord : NSSecureCoding {
		[Export ("subsystem")]
		string Subsystem { get; }

		[Export ("category")]
		string Category { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("beginTimeStamp", ArgumentSemantic.Copy)]
		NSDate BeginTimeStamp { get; }

		[NullAllowed, Export ("endTimeStamp", ArgumentSemantic.Copy)]
		NSDate EndTimeStamp { get; }

		[NullAllowed, Export ("duration", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitDuration> Duration { get; }

		[Export ("isInterval")]
		bool IsInterval { get; }

		[Export ("JSONRepresentation")]
		NSData JsonRepresentation { get; }

		[Export ("dictionaryRepresentation")]
		NSDictionary DictionaryRepresentation { get; }
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface MXCrashDiagnosticObjectiveCExceptionReason : NSSecureCoding {
		[Export ("composedMessage")]
		string ComposedMessage { get; }

		[Export ("formatString")]
		string FormatString { get; }

		[Export ("arguments", ArgumentSemantic.Copy)]
		string [] Arguments { get; }

		[Export ("exceptionType")]
		string ExceptionType { get; }

		[Export ("className")]
		string ClassName { get; }

		[Export ("exceptionName")]
		string ExceptionName { get; }

		[Export ("JSONRepresentation")]
		NSData JsonRepresentation { get; }

		[Export ("dictionaryRepresentation")]
		NSDictionary DictionaryRepresentation { get; }
	}
}
