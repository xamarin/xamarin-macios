//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2010, Novell, Inc.
// Copyright 2013-2015 Xamarin Inc.
// Copyright 2019 Microsoft Corporation
//
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using System;

namespace CoreMotion {
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	[MacCatalyst (13, 1)]
	interface CMAccelerometerData : NSSecureCoding {
		[Export ("acceleration")]
		CMAcceleration Acceleration { get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMAccelerometerData))]
	[DisableDefaultCtor]
	interface CMRecordedAccelerometerData {
		[Export ("identifier")]
		ulong Identifier { get; }

		[Export ("startDate")]
		NSDate StartDate { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMLogItem : NSSecureCoding, NSCopying {
		[Export ("timestamp")]
		double Timestamp { get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void CMAccelerometerHandler (CMAccelerometerData data, NSError error);
	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void CMGyroHandler (CMGyroData gyroData, NSError error);
	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void CMDeviceMotionHandler (CMDeviceMotion motion, NSError error);

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMMotionManager {
		[Export ("accelerometerAvailable")]
		bool AccelerometerAvailable { [Bind ("isAccelerometerAvailable")] get; }

		[Export ("accelerometerActive")]
		bool AccelerometerActive { [Bind ("isAccelerometerActive")] get; }

		[NullAllowed]
		[Export ("accelerometerData")]
		CMAccelerometerData AccelerometerData { get; }

		[Export ("accelerometerUpdateInterval")]
		double AccelerometerUpdateInterval { get; set; }

		[Export ("startAccelerometerUpdates")]
		void StartAccelerometerUpdates ();

		[Export ("startAccelerometerUpdatesToQueue:withHandler:")]
		void StartAccelerometerUpdates (NSOperationQueue queue, CMAccelerometerHandler handler);

		[Export ("stopAccelerometerUpdates")]
		void StopAccelerometerUpdates ();

		[Export ("deviceMotionUpdateInterval")]
		double DeviceMotionUpdateInterval { get; set; }

		[Export ("deviceMotionAvailable")]
		bool DeviceMotionAvailable { [Bind ("isDeviceMotionAvailable")] get; }

		[Export ("deviceMotionActive")]
		bool DeviceMotionActive { [Bind ("isDeviceMotionActive")] get; }

		[NullAllowed]
		[Export ("deviceMotion")]
		CMDeviceMotion DeviceMotion { get; }

		[Export ("gyroUpdateInterval")]
		double GyroUpdateInterval { get; set; }

		[Export ("gyroAvailable")]
		bool GyroAvailable { [Bind ("isGyroAvailable")] get; }

		[Export ("gyroActive")]
		bool GyroActive { [Bind ("isGyroActive")] get; }

		[NullAllowed]
		[Export ("gyroData")]
		CMGyroData GyroData { get; }

		[Export ("startGyroUpdates")]
		void StartGyroUpdates ();

		[Export ("startGyroUpdatesToQueue:withHandler:")]
		void StartGyroUpdates (NSOperationQueue toQueue, CMGyroHandler handler);

		[Export ("stopGyroUpdates")]
		void StopGyroUpdates ();

		[Export ("startDeviceMotionUpdates")]
		void StartDeviceMotionUpdates ();

		[Export ("startDeviceMotionUpdatesToQueue:withHandler:")]
		void StartDeviceMotionUpdates (NSOperationQueue toQueue, CMDeviceMotionHandler handler);

		[Export ("stopDeviceMotionUpdates")]
		void StopDeviceMotionUpdates ();

		[Export ("magnetometerUpdateInterval")]
		double MagnetometerUpdateInterval { get; set; }

		[Export ("magnetometerAvailable")]
		bool MagnetometerAvailable { [Bind ("isMagnetometerAvailable")] get; }

		[Export ("magnetometerActive")]
		bool MagnetometerActive { [Bind ("isMagnetometerActive")] get; }

		[NullAllowed]
		[Export ("magnetometerData")]
		CMMagnetometerData MagnetometerData { get; }

		[Export ("startMagnetometerUpdates")]
		void StartMagnetometerUpdates ();

		[Export ("startMagnetometerUpdatesToQueue:withHandler:")]
		void StartMagnetometerUpdates (NSOperationQueue queue, CMMagnetometerHandler handler);

		[Export ("stopMagnetometerUpdates")]
		void StopMagnetometerUpdates ();

		[Export ("availableAttitudeReferenceFrames"), Static]
		CMAttitudeReferenceFrame AvailableAttitudeReferenceFrames { get; }

		[Export ("attitudeReferenceFrame")]
		CMAttitudeReferenceFrame AttitudeReferenceFrame { get; }

		[Export ("startDeviceMotionUpdatesUsingReferenceFrame:")]
		void StartDeviceMotionUpdates (CMAttitudeReferenceFrame referenceFrame);

		[Export ("startDeviceMotionUpdatesUsingReferenceFrame:toQueue:withHandler:")]
		void StartDeviceMotionUpdates (CMAttitudeReferenceFrame referenceFrame, NSOperationQueue queue, CMDeviceMotionHandler handler);

		[Export ("showsDeviceMovementDisplay")]
		bool ShowsDeviceMovementDisplay { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	//<quote>You access CMAttitude objects through the attitude property of each CMDeviceMotion objects passed to an application.</quote>
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMAttitude : NSSecureCoding, NSCopying {
		[Export ("pitch")]
		double Pitch { get; }

		[Export ("yaw")]
		double Yaw { get; }

		[Export ("rotationMatrix")]
		CMRotationMatrix RotationMatrix { get; }

		[Export ("quaternion")]
		CMQuaternion Quaternion { get; }

		[Export ("roll")]
		double Roll { get; }

		[Export ("multiplyByInverseOfAttitude:")]
		void MultiplyByInverseOfAttitude (CMAttitude attitude);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMDeviceMotion : NSSecureCoding {
		[Export ("rotationRate")]
		CMRotationRate RotationRate { get; }

		[Export ("gravity")]
		CMAcceleration Gravity { get; }

		[Export ("userAcceleration")]
		CMAcceleration UserAcceleration { get; }

		[Export ("attitude")]
		CMAttitude Attitude { get; }

		[Export ("magneticField")]
		CMCalibratedMagneticField MagneticField { get; }

		[MacCatalyst (13, 1)]
		[Export ("heading")]
		double Heading { get; }

		[iOS (14, 0)]
		[Watch (7, 0)]
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sensorLocation")]
		CMDeviceMotionSensorLocation SensorLocation { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMGyroData : NSSecureCoding {
		[Export ("rotationRate")]
		CMRotationRate RotationRate { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMMagnetometerData : NSSecureCoding {
		[Export ("magneticField")]
		CMMagneticField MagneticField { get; }
	}

	delegate void CMMagnetometerHandler (CMMagnetometerData magnetometerData, NSError error);

	[NoWatch]
	[MacCatalyst (13, 1)]
	delegate void CMStepQueryHandler (nint numberOfSteps, NSError error);

	[NoWatch]
	[MacCatalyst (13, 1)]
	delegate void CMStepUpdateHandler (nint numberOfSteps, NSDate timestamp, NSError error);

	[NoMac]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'CMPedometer' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CMPedometer' instead.")]
	interface CMStepCounter {

		[Static]
		[Export ("isStepCountingAvailable")]
		bool IsStepCountingAvailable { get; }

		[Export ("queryStepCountStartingFrom:to:toQueue:withHandler:")]
		[Async]
		void QueryStepCount (NSDate start, NSDate end, NSOperationQueue queue, CMStepQueryHandler handler);

		[Export ("startStepCountingUpdatesToQueue:updateOn:withHandler:")]
		void StartStepCountingUpdates (NSOperationQueue queue, nint stepCounts, CMStepUpdateHandler handler);

		[Export ("stopStepCountingUpdates")]
		void StopStepCountingUpdates ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMPedometerData : NSSecureCoding, NSCopying {

		[Export ("startDate")]
		NSDate StartDate { get; }

		[Export ("endDate")]
		NSDate EndDate { get; }

		[Export ("numberOfSteps")]
		NSNumber NumberOfSteps { get; }

		[NullAllowed]
		[Export ("distance")]
		NSNumber Distance { get; }

		[NullAllowed]
		[Export ("floorsAscended")]
		NSNumber FloorsAscended { get; }

		[NullAllowed]
		[Export ("floorsDescended")]
		NSNumber FloorsDescended { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("currentPace")]
		NSNumber CurrentPace { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("currentCadence")]
		NSNumber CurrentCadence { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("averageActivePace")]
		NSNumber AverageActivePace { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMPedometer {

		[Static]
		[Export ("isStepCountingAvailable")]
		bool IsStepCountingAvailable { get; }

		[Static]
		[Export ("isDistanceAvailable")]
		bool IsDistanceAvailable { get; }

		[Static]
		[Export ("isFloorCountingAvailable")]
		bool IsFloorCountingAvailable { get; }

		[Export ("queryPedometerDataFromDate:toDate:withHandler:")]
		[Async]
		void QueryPedometerData (NSDate start, NSDate end, Action<CMPedometerData, NSError> handler);

		[Export ("startPedometerUpdatesFromDate:withHandler:")]
		[Async]
		void StartPedometerUpdates (NSDate start, Action<CMPedometerData, NSError> handler);

		[Export ("stopPedometerUpdates")]
		void StopPedometerUpdates ();

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("isPaceAvailable")]
		bool IsPaceAvailable { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("isCadenceAvailable")]
		bool IsCadenceAvailable { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("isPedometerEventTrackingAvailable")]
		bool IsPedometerEventTrackingAvailable { get; }

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("startPedometerEventUpdatesWithHandler:")]
		void StartPedometerEventUpdates (Action<CMPedometerEvent, NSError> handler);

		[MacCatalyst (13, 1)]
		[Export ("stopPedometerEventUpdates")]
		void StopPedometerEventUpdates ();

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void CMMotionActivityHandler (CMMotionActivity activity);

	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void CMMotionActivityQueryHandler (CMMotionActivity [] activities, NSError error);

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMMotionActivityManager {

		[Static]
		[Export ("isActivityAvailable")]
		bool IsActivityAvailable { get; }

		[Export ("queryActivityStartingFromDate:toDate:toQueue:withHandler:")]
		[Async]
		void QueryActivity (NSDate start, NSDate end, NSOperationQueue queue, CMMotionActivityQueryHandler handler);

		[Export ("startActivityUpdatesToQueue:withHandler:")]
		void StartActivityUpdates (NSOperationQueue queue, CMMotionActivityHandler handler);

		[Export ("stopActivityUpdates")]
		void StopActivityUpdates ();

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // <quote>You do not create instances of this class yourself.</quote>
	interface CMMotionActivity : NSCopying, NSSecureCoding {
		[Export ("confidence")]
		CMMotionActivityConfidence Confidence { get; }

		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		[Export ("unknown")]
		bool Unknown { get; }

		[Export ("stationary")]
		bool Stationary { get; }

		[Export ("walking")]
		bool Walking { get; }

		[Export ("running")]
		bool Running { get; }

		[Export ("automotive")]
		bool Automotive { get; }

		[MacCatalyst (13, 1)]
		[Export ("cycling")]
		bool Cycling { get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // this does not look to be meant to be user created (and crash when description is called)
	interface CMAltitudeData {
		[Export ("relativeAltitude")]
		NSNumber RelativeAltitude { get; }

		[Export ("pressure")]
		NSNumber Pressure { get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMAltimeter {
		[Static]
		[Export ("isRelativeAltitudeAvailable")]
		bool IsRelativeAltitudeAvailable { get; }

		[Export ("startRelativeAltitudeUpdatesToQueue:withHandler:")]
		[Async]
		void StartRelativeAltitudeUpdates (NSOperationQueue queue, Action<CMAltitudeData, NSError> handler);

		[Export ("stopRelativeAltitudeUpdates")]
		void StopRelativeAltitudeUpdates ();

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }

		[Watch (8, 0), NoTV, NoMac, iOS (15, 0)]
		[NoMacCatalyst]
		[Static]
		[Export ("isAbsoluteAltitudeAvailable")]
		bool IsAbsoluteAltitudeAvailable { get; }

		[Watch (8, 0), NoTV, NoMac, iOS (15, 0)]
		[NoMacCatalyst]
		[Export ("startAbsoluteAltitudeUpdatesToQueue:withHandler:")]
		void StartAbsoluteAltitudeUpdates (NSOperationQueue queue, Action<CMAbsoluteAltitudeData, NSError> handler);

		[Watch (8, 0), NoTV, NoMac, iOS (15, 0)]
		[NoMacCatalyst]
		[Export ("stopAbsoluteAltitudeUpdates")]
		void StopAbsoluteAltitudeUpdates ();
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum CMAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
	}

	[Watch (8, 4), iOS (15, 4), MacCatalyst (15, 4), TV (15, 4)]
	[Native]
	public enum CMOdometerOriginDevice : long {
		Unknown = 0,
		Local,
		Remote,
	}


	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMSensorDataList /* NSFastEnumeration */
	{
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMSensorRecorder {
		[Static]
		[MacCatalyst (13, 1)]
		[Export ("isAccelerometerRecordingAvailable")]
		bool IsAccelerometerRecordingAvailable { get; }

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CMSensorRecorder.AuthorizationStatus' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CMSensorRecorder.AuthorizationStatus' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CMSensorRecorder.AuthorizationStatus' instead.")]
		[Static]
		[Export ("isAuthorizedForRecording")]
		bool IsAuthorizedForRecording { get; }

		[MacCatalyst (13, 1)]
		[Export ("accelerometerDataFromDate:toDate:")]
		[return: NullAllowed]
		CMSensorDataList GetAccelerometerData (NSDate fromDate, NSDate toDate);

		[MacCatalyst (13, 1)]
		[Export ("recordAccelerometerForDuration:")]
		void RecordAccelerometer (double duration);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CMPedometerEventType : long {
		Pause,
		Resume
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // instances exposed from delegate
	interface CMPedometerEvent : NSSecureCoding, NSCopying {
		[Export ("date")]
		NSDate Date { get; }

		[Export ("type")]
		CMPedometerEventType Type { get; }
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CMDyskineticSymptomResult : NSCopying, NSSecureCoding {

		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; }

		[Export ("percentUnlikely")]
		float PercentUnlikely { get; }

		[Export ("percentLikely")]
		float PercentLikely { get; }
	}

	[Watch (5, 0), NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CMTremorResult : NSCopying, NSSecureCoding {

		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; }

		[Export ("percentUnknown")]
		float PercentUnknown { get; }

		[Export ("percentNone")]
		float PercentNone { get; }

		[Export ("percentSlight")]
		float PercentSlight { get; }

		[Export ("percentMild")]
		float PercentMild { get; }

		[Export ("percentModerate")]
		float PercentModerate { get; }

		[Export ("percentStrong")]
		float PercentStrong { get; }
	}

	[NoMac]
	[Watch (5, 0), NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	delegate void CMDyskineticSymptomResultHandler (CMDyskineticSymptomResult [] dyskineticSymptomResult, NSError error);

	[NoMac]
	[Watch (5, 0), NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	delegate void CMTremorResultHandler (CMTremorResult [] tremorResults, NSError error);

	[NoMac]
	[Watch (5, 0), NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CMMovementDisorderManager {

		[Static]
		[Export ("isAvailable")]
		bool IsAvailable { get; }

		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }

		[Export ("monitorKinesiasForDuration:")]
		void MonitorKinesias (double durationInSeconds);

		[Async]
		[Export ("queryDyskineticSymptomFromDate:toDate:withHandler:")]
		void QueryDyskineticSymptom (NSDate fromDate, NSDate toDate, CMDyskineticSymptomResultHandler handler);

		[Async]
		[Export ("queryTremorFromDate:toDate:withHandler:")]
		void QueryTremor (NSDate fromDate, NSDate toDate, CMTremorResultHandler handler);

		[NullAllowed, Export ("lastProcessedDate")]
		NSDate LastProcessedDate { get; }

		[NullAllowed, Export ("monitorKinesiasExpirationDate")]
		NSDate MonitorKinesiasExpirationDate { get; }

		[Static]
		[NullAllowed, Export ("version")]
		string Version { get; }
	}

	[MacCatalyst (13, 1)]
	[ErrorDomain ("CMErrorDomain")]
	// untyped enum -> CMError.h
	public enum CMError {
		Null = 100,
		DeviceRequiresMovement,
		TrueNorthNotAvailable,
		Unknown,
		MotionActivityNotAvailable,
		MotionActivityNotAuthorized,
		MotionActivityNotEntitled,
		InvalidParameter,
		InvalidAction,
		NotAvailable,
		NotEntitled,
		NotAuthorized,
		NilData,
		Size,
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	// untyped enum -> CMAttitude.h
	// in Xcode 6.3 SDK is became an NSUInteger
	[Flags]
	[Native]
	public enum CMAttitudeReferenceFrame : ulong {
		XArbitraryZVertical = 1 << 0,
		XArbitraryCorrectedZVertical = 1 << 1,
		XMagneticNorthZVertical = 1 << 2,
		XTrueNorthZVertical = 1 << 3,
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	// NSInteger -> CMMotionActivity.h
	[Native]
	public enum CMMotionActivityConfidence : long {
		Low = 0,
		Medium,
		High,
	}

	[iOS (14, 0)]
	[Watch (7, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum CMDeviceMotionSensorLocation : long {
		Default,
		HeadphoneLeft,
		HeadphoneRight,
	}

	[Watch (10, 0), NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum CMHighFrequencyHeartRateDataConfidence : long {
		Low = 0,
		Medium,
		High,
		Highest,
	}

	[iOS (14, 0)]
	[Watch (7, 0)]
	[MacCatalyst (14, 0)]
	[NoMac]
	delegate void CMHeadphoneDeviceMotionHandler ([NullAllowed] CMDeviceMotion motion, [NullAllowed] NSError error);

	[iOS (14, 0)]
	[Watch (7, 0)]
	[MacCatalyst (14, 0)]
	[Mac (13, 0)]
	[BaseType (typeof (NSObject))]
	interface CMHeadphoneMotionManager {

		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICMHeadphoneMotionManagerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("deviceMotionAvailable")]
		bool DeviceMotionAvailable { [Bind ("isDeviceMotionAvailable")] get; }

		[Export ("deviceMotionActive")]
		bool DeviceMotionActive { [Bind ("isDeviceMotionActive")] get; }

		[NullAllowed, Export ("deviceMotion")]
		CMDeviceMotion DeviceMotion { get; }

		[Export ("startDeviceMotionUpdates")]
		void StartDeviceMotionUpdates ();

		[Export ("startDeviceMotionUpdatesToQueue:withHandler:")]
		void StartDeviceMotionUpdates (NSOperationQueue queue, CMHeadphoneDeviceMotionHandler handler);

		[Export ("stopDeviceMotionUpdates")]
		void StopDeviceMotionUpdates ();
	}

	interface ICMHeadphoneMotionManagerDelegate { }

	[iOS (14, 0)]
	[Watch (7, 0)]
	[MacCatalyst (14, 0)]
	[Mac (13, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CMHeadphoneMotionManagerDelegate {

		[Export ("headphoneMotionManagerDidConnect:")]
		void DidConnect (CMHeadphoneMotionManager manager);

		[Export ("headphoneMotionManagerDidDisconnect:")]
		void DidDisconnect (CMHeadphoneMotionManager manager);
	}

	[Watch (7, 0), NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor]
	interface CMRotationRateData {

		[Export ("rotationRate")]
		CMRotationRate RotationRate { get; }
	}

	[Watch (7, 0), NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (CMRotationRateData))]
	[DisableDefaultCtor]
	interface CMRecordedRotationRateData {

		[Export ("startDate")]
		NSDate StartDate { get; }
	}

	[Watch (7, 2), NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	[Native]
	enum CMFallDetectionEventUserResolution : long {
		Confirmed,
		Dismissed,
		Rejected,
		Unresponsive,
	}

	[Watch (7, 2), NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CMFallDetectionEvent {

		[Export ("date")]
		NSDate Date { get; }

		[Export ("resolution")]
		CMFallDetectionEventUserResolution Resolution { get; }
	}

	[Watch (7, 2), NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CMFallDetectionManager {

		[Static]
		[Export ("available")]
		bool Available { [Bind ("isAvailable")] get; }

		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICMFallDetectionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("requestAuthorizationWithHandler:")]
		void RequestAuthorization (Action<CMAuthorizationStatus> handler);
	}

	interface ICMFallDetectionDelegate { }

	[Watch (7, 2), NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CMFallDetectionDelegate {

		[Export ("fallDetectionManager:didDetectEvent:completionHandler:")]
		void DidDetectEvent (CMFallDetectionManager fallDetectionManager, CMFallDetectionEvent @event, Action handler);

		[Export ("fallDetectionManagerDidChangeAuthorization:")]
		void DidChangeAuthorization (CMFallDetectionManager fallDetectionManager);
	}

	[DisableDefaultCtor] // will crash
	[Watch (8, 0), NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (CMLogItem))]
	interface CMAbsoluteAltitudeData {
		[Export ("altitude")]
		double Altitude { get; }

		[Export ("accuracy")]
		double Accuracy { get; }

		[Export ("precision")]
		double Precision { get; }
	}

	// Just to please the generator that at this point does not know the hierarchy
	interface NSUnitPressure : NSUnit { }
	interface NSUnitTemperature : NSUnit { }
	interface NSUnitLength : NSUnit { }

	[Watch (8, 5), NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor]
	interface CMAmbientPressureData {

		[Export ("pressure", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitPressure> Pressure { get; }

		[Export ("temperature", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitTemperature> Temperature { get; }
	}

	[Watch (8, 5), NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
	[BaseType (typeof (CMAmbientPressureData))]
	[DisableDefaultCtor]
	interface CMRecordedPressureData {

		[Export ("identifier")]
		ulong Identifier { get; }

		[Export ("startDate")]
		NSDate StartDate { get; }
	}

	[Mac (13, 0), iOS (16, 0), NoMacCatalyst, Watch (9, 0), NoTV]
	[Native]
	public enum CMWaterSubmersionState : long {
		Unknown = 0,
		NotSubmerged,
		Submerged,
	}

	[Mac (13, 0), iOS (16, 0), NoMacCatalyst, Watch (9, 0), NoTV]
	[Native]
	public enum CMWaterSubmersionDepthState : long {
		Unknown = 0,
		NotSubmerged = 100,
		SubmergedShallow = 200,
		SubmergedDeep = 300,
		ApproachingMaxDepth = 400,
		PastMaxDepth = 500,
		SensorDepthError = 600,
	}

	[NoMac, iOS (16, 0), NoMacCatalyst, Watch (9, 0), NoTV]
	[BaseType (typeof (NSObject))]
	interface CMWaterSubmersionEvent : NSSecureCoding, NSCopying {
		[Export ("date")]
		NSDate Date { get; }

		[Export ("state")]
		CMWaterSubmersionState State { get; }
	}

	[NoMac, iOS (16, 0), NoMacCatalyst, Watch (9, 0), NoTV]
	[BaseType (typeof (NSObject))]
	interface CMWaterSubmersionMeasurement : NSSecureCoding, NSCopying {
		[Export ("date")]
		NSDate Date { get; }

		[NullAllowed, Export ("depth")]
		NSMeasurement<NSUnitLength> Depth { get; }

		[NullAllowed, Export ("pressure")]
		NSMeasurement<NSUnitPressure> Pressure { get; }

		[Export ("surfacePressure")]
		NSMeasurement<NSUnitPressure> SurfacePressure { get; }

		[Export ("submersionState")]
		CMWaterSubmersionDepthState SubmersionState { get; }
	}

	[NoMac, iOS (16, 0), NoMacCatalyst, Watch (9, 0), NoTV]
	[BaseType (typeof (NSObject))]
	interface CMWaterTemperature : NSSecureCoding, NSCopying {
		[Export ("date")]
		NSDate Date { get; }

		[Export ("temperature")]
		NSMeasurement<NSUnitTemperature> Temperature { get; }

		[Export ("temperatureUncertainty")]
		NSMeasurement<NSUnitTemperature> TemperatureUncertainty { get; }
	}

	interface ICMWaterSubmersionManagerDelegate { }

	[NoMac, iOS (16, 0), NoMacCatalyst, Watch (9, 0), NoTV]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CMWaterSubmersionManagerDelegate {
		[Abstract]
		[Export ("manager:didUpdateEvent:")]
		void DidUpdateEvent (CMWaterSubmersionManager manager, CMWaterSubmersionEvent @event);

		[Abstract]
		[Export ("manager:didUpdateMeasurement:")]
		void DidUpdateMeasurement (CMWaterSubmersionManager manager, CMWaterSubmersionMeasurement measurement);

		[Abstract]
		[Export ("manager:didUpdateTemperature:")]
		void DidUpdateTemperature (CMWaterSubmersionManager manager, CMWaterTemperature measurement);

		[Abstract]
		[Export ("manager:errorOccurred:")]
		void ErrorOccurred (CMWaterSubmersionManager manager, NSError error);
	}

	[NoMac, iOS (16, 0), NoMacCatalyst, Watch (9, 0), NoTV]
	[BaseType (typeof (NSObject))]
	interface CMWaterSubmersionManager {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICMWaterSubmersionManagerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Static]
		[Export ("waterSubmersionAvailable")]
		bool IsWaterSubmersionAvailable { get; }

		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }

		[NullAllowed, Export ("maximumDepth")]
		NSMeasurement<NSUnitLength> MaximumDepth { get; }
	}

	[Watch (10, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface CMBatchedSensorManager {
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }

		[Static]
		[Export ("accelerometerSupported")]
		bool AccelerometerSupported { [Bind ("isAccelerometerSupported")] get; }

		[Export ("accelerometerActive")]
		bool AccelerometerActive { [Bind ("isAccelerometerActive")] get; }

		[Export ("accelerometerDataFrequency")]
		nint AccelerometerDataFrequency { get; }

		[NullAllowed, Export ("accelerometerBatch")]
		CMAccelerometerData [] AccelerometerBatch { get; }

		[Export ("startAccelerometerUpdates")]
		void StartAccelerometerUpdates ();

		[Export ("startAccelerometerUpdatesWithHandler:")]
		void StartAccelerometerUpdates (Action<CMAccelerometerData [], NSError> handler);

		[Export ("stopAccelerometerUpdates")]
		void StopAccelerometerUpdates ();

		[Static]
		[Export ("deviceMotionSupported")]
		bool DeviceMotionSupported { [Bind ("isDeviceMotionSupported")] get; }

		[Export ("deviceMotionDataFrequency")]
		nint DeviceMotionDataFrequency { get; }

		[Export ("deviceMotionActive")]
		bool DeviceMotionActive { [Bind ("isDeviceMotionActive")] get; }

		[NullAllowed, Export ("deviceMotionBatch")]
		CMDeviceMotion [] DeviceMotionBatch { get; }

		[Export ("startDeviceMotionUpdates")]
		void StartDeviceMotionUpdates ();

		[Export ("startDeviceMotionUpdatesWithHandler:")]
		void StartDeviceMotionUpdates (Action<CMDeviceMotion [], NSError> handler);

		[Export ("stopDeviceMotionUpdates")]
		void StopDeviceMotionUpdates ();
	}

	[Mac (14, 0), MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMOdometerData : NSSecureCoding, NSCopying {
		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; }

		[Watch (10, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("deltaDistance")]
		double DeltaDistance { get; }

		[Watch (10, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("deltaDistanceAccuracy")]
		double DeltaDistanceAccuracy { get; }

		[Watch (10, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("speed")]
		double Speed { get; }

		[Watch (10, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("speedAccuracy")]
		double SpeedAccuracy { get; }

		[Watch (10, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("gpsDate", ArgumentSemantic.Strong)]
		NSDate GpsDate { get; }

		[Watch (10, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("deltaAltitude")]
		double DeltaAltitude { get; }

		[Watch (10, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("verticalAccuracy")]
		double VerticalAccuracy { get; }

		[Watch (8, 4), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("originDevice", ArgumentSemantic.Assign)]
		CMOdometerOriginDevice OriginDevice { get; }

		[Watch (8, 4), iOS (15, 4), MacCatalyst (15, 4)]
		[NullAllowed]
		[Export ("slope", ArgumentSemantic.Strong)]
		NSNumber Slope { get; }

		[Watch (8, 4), iOS (15, 4), MacCatalyst (15, 4)]
		[NullAllowed]
		[Export ("maxAbsSlope", ArgumentSemantic.Strong)]
		NSNumber MaxAbsSlope { get; }
	}

	[Watch (10, 0), NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CMLogItem))]
	interface CMHighFrequencyHeartRateData {
		[Export ("heartRate")]
		double HeartRate { get; }

		[Export ("confidence")]
		CMHighFrequencyHeartRateDataConfidence Confidence { get; }

		[NullAllowed, Export ("date")]
		NSDate Date { get; }
	}
}
