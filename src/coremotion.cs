//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2010, Novell, Inc.
// Copyright 2013-2015 Xamarin Inc.
//
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using UIKit;
using System;

namespace CoreMotion {
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMAccelerometerData : NSSecureCoding {
		[Export ("acceleration")]
		CMAcceleration Acceleration { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof (CMAccelerometerData))]
	[DisableDefaultCtor]
	interface CMRecordedAccelerometerData {
		[Export ("identifier")]
		ulong Identifier { get; }

		[Export ("startDate")]
		NSDate StartDate { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMLogItem : NSSecureCoding, NSCopying {
		[Export ("timestamp")]
		double Timestamp { get; }
	}

	delegate void CMAccelerometerHandler (CMAccelerometerData data, NSError error);
	delegate void CMGyroHandler (CMGyroData gyroData, NSError error);
	delegate void CMDeviceMotionHandler (CMDeviceMotion motion, NSError error);

	[BaseType (typeof (NSObject))]
	interface CMMotionManager {
		[Export ("accelerometerAvailable")]
		bool AccelerometerAvailable { [Bind ("isAccelerometerAvailable")] get;  }

		[Export ("accelerometerActive")]
		bool AccelerometerActive { [Bind ("isAccelerometerActive")] get;  }

		[Export ("accelerometerData")]
		CMAccelerometerData AccelerometerData { get;  }

		[Export ("accelerometerUpdateInterval")]
		double AccelerometerUpdateInterval { get; set; }

		[Export ("startAccelerometerUpdates")]
		void StartAccelerometerUpdates ();

		[Export ("startAccelerometerUpdatesToQueue:withHandler:")]
		void StartAccelerometerUpdates (NSOperationQueue queue, CMAccelerometerHandler handler);

		[Export ("stopAccelerometerUpdates")]
		void StopAccelerometerUpdates ();

		[Export ("deviceMotionUpdateInterval")]
		double DeviceMotionUpdateInterval { get; set;  }

		[Export ("deviceMotionAvailable")]
		bool DeviceMotionAvailable { [Bind ("isDeviceMotionAvailable")] get;  }

		[Export ("deviceMotionActive")]
		bool DeviceMotionActive { [Bind ("isDeviceMotionActive")] get;  }

		[Export ("deviceMotion")]
		CMDeviceMotion DeviceMotion { get;  }

		[Export ("gyroUpdateInterval")]
		double GyroUpdateInterval { get; set;  }

		[Export ("gyroAvailable")]
		bool GyroAvailable { [Bind ("isGyroAvailable")] get;  }

		[Export ("gyroActive")]
		bool GyroActive { [Bind ("isGyroActive")] get;  }

		[Export ("gyroData")]
		CMGyroData GyroData { get;  }

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

	[BaseType (typeof (NSObject))]
	//<quote>You access CMAttitude objects through the attitude property of each CMDeviceMotion objects passed to an application.</quote>
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMAttitude : NSSecureCoding, NSCopying {
		[Export ("pitch")]
		double Pitch { get;  }

		[Export ("yaw")]
		double Yaw { get;  }

		[Export ("rotationMatrix")]
		CMRotationMatrix RotationMatrix { get;  }

		[Export ("quaternion")]
		CMQuaternion Quaternion { get;  }

		[Export ("roll")]
		double Roll { get; }

		[Export ("multiplyByInverseOfAttitude:")]
		void MultiplyByInverseOfAttitude (CMAttitude attitude);
	}

	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMDeviceMotion : NSSecureCoding {
		[Export ("rotationRate")]
		CMRotationRate RotationRate { get;  }

		[Export ("gravity")]
		CMAcceleration Gravity { get;  }

		[Export ("userAcceleration")]
		CMAcceleration UserAcceleration { get;  }

		[Export ("attitude")]
		CMAttitude Attitude { get; }

		[Export ("magneticField")]
		CMCalibratedMagneticField MagneticField { get; }

		[Watch (4,0), iOS (11,0)]
		[Export ("heading")]
		double Heading { get; }
	}

	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMGyroData : NSSecureCoding {
		[Export ("rotationRate")]
		CMRotationRate RotationRate { get; }
	}

	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMMagnetometerData : NSSecureCoding {
		[Export ("magneticField")]
		CMMagneticField MagneticField { get; }
	}

	delegate void CMMagnetometerHandler (CMMagnetometerData magnetometerData, NSError error);

	[NoWatch]
	[iOS (7,0)]
	delegate void CMStepQueryHandler (nint numberOfSteps, NSError error);

	[NoWatch]
	[iOS (7,0)]
	delegate void CMStepUpdateHandler (nint numberOfSteps, NSDate timestamp, NSError error);

	[NoWatch]
	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[Availability (Deprecated = Platform.iOS_8_0, Message = "Use 'CMPedometer' instead.")]
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

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface CMPedometerData : NSSecureCoding, NSCopying {

		[Export ("startDate")]
		NSDate StartDate { get; }

		[Export ("endDate")]
		NSDate EndDate { get; }

		[Export ("numberOfSteps")]
		NSNumber NumberOfSteps { get; }

		[Export ("distance")]
		NSNumber Distance { get; }

		[Export ("floorsAscended")]
		NSNumber FloorsAscended { get; }

		[Export ("floorsDescended")]
		NSNumber FloorsDescended { get; }

		[iOS (9,0)]
		[NullAllowed, Export ("currentPace")]
		NSNumber CurrentPace { get; }

		[iOS (9,0)]
		[NullAllowed]
		[Export ("currentCadence")]
		NSNumber CurrentCadence { get; }

		[iOS (10,0)]
		[NullAllowed, Export ("averageActivePace")]
		NSNumber AverageActivePace { get; }
	}

	[iOS (8,0)]
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
		void QueryPedometerData (NSDate start, NSDate end, Action<CMPedometerData,NSError> handler);

		[Export ("startPedometerUpdatesFromDate:withHandler:")]
		[Async]
		void StartPedometerUpdates (NSDate start, Action<CMPedometerData,NSError> handler);

		[Export ("stopPedometerUpdates")]
		void StopPedometerUpdates ();

		[iOS (9,0)]
		[Static]
		[Export ("isPaceAvailable")]
		bool IsPaceAvailable { get; }

		[iOS (9,0)]
		[Static]
		[Export ("isCadenceAvailable")]
		bool IsCadenceAvailable { get; }

		[Watch (3,0)][iOS (10,0)]
		[Static]
		[Export ("isPedometerEventTrackingAvailable")]
		bool IsPedometerEventTrackingAvailable { get; }

		[Watch (3,0)][iOS (10,0)]
		[Async]
		[Export ("startPedometerEventUpdatesWithHandler:")]
		void StartPedometerEventUpdates (Action<CMPedometerEvent,NSError> handler);

		[Watch (3,0)][iOS (10,0)]
		[Export ("stopPedometerEventUpdates")]
		void StopPedometerEventUpdates ();

		[Watch (4,0), iOS (11,0)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }
	}

	[iOS (7,0)]
	delegate void CMMotionActivityHandler (CMMotionActivity activity);

	[iOS (7,0)]
	delegate void CMMotionActivityQueryHandler (CMMotionActivity[] activities, NSError error);

	[iOS (7,0)]
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

		[Watch (4,0), iOS (11,0)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }
	}

	[iOS (7,0)]
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

		[iOS (8,0)]
		[Export ("cycling")]
		bool Cycling { get; }
	}

	[iOS (8,0)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // this does not look to be meant to be user created (and crash when description is called)
	interface CMAltitudeData {
		[Export ("relativeAltitude")]
		NSNumber RelativeAltitude { get; }

		[Export ("pressure")]
		NSNumber Pressure { get; }
	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface CMAltimeter {
		[Static]
		[Export ("isRelativeAltitudeAvailable")]
		bool IsRelativeAltitudeAvailable { get; }

		[Export ("startRelativeAltitudeUpdatesToQueue:withHandler:")]
		[Async]
		void StartRelativeAltitudeUpdates (NSOperationQueue queue, Action<CMAltitudeData,NSError> handler);

		[Export ("stopRelativeAltitudeUpdates")]
		void StopRelativeAltitudeUpdates ();

		[Watch (4,0), iOS (11,0)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }
	}

	[Watch (4,0), iOS (11,0)]
	[Native]
	public enum CMAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
	}

	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface CMSensorDataList /* NSFastEnumeration */
	{
	}
		
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface CMSensorRecorder
	{
		[Static]
		[iOS (9,3)] // Apple changed the selector in 9.3 and removed the old one
		[Export ("isAccelerometerRecordingAvailable")]
		bool IsAccelerometerRecordingAvailable { get; }

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CMSensorRecorder.AuthorizationStatus' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CMSensorRecorder.AuthorizationStatus' instead.")]
		[Static]
		[Export ("isAuthorizedForRecording")]
		bool IsAuthorizedForRecording { get; }

		[iOS (9,3)] // Apple changed the selector in 9.3 and removed the old one
		[Export ("accelerometerDataFromDate:toDate:")]
		[return: NullAllowed]
		CMSensorDataList GetAccelerometerData (NSDate fromDate, NSDate toDate);

		[iOS (9,3)] // Apple changed the selector in 9.3 and removed the old one
		[Export ("recordAccelerometerForDuration:")]
		void RecordAccelerometer (double duration);

		[Watch (4, 0), iOS (11, 0)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }
	}

	[Watch (3,0)][NoTV][iOS (10,0)]
	[Native]
	public enum CMPedometerEventType : long {
		Pause,
		Resume
	}

	[Watch (3,0)][NoTV][iOS (10,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // instances exposed from delegate
	interface CMPedometerEvent : NSSecureCoding, NSCopying {
		[Export ("date")]
		NSDate Date { get; }

		[Export ("type")]
		CMPedometerEventType Type { get; }
	}
}
