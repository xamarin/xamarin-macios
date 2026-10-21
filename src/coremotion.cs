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
using CoreGraphics;
using CoreLocation;

namespace CoreMotion {
	/// <summary>An accelerometer event, encapsulates an instance of <see cref="CoreMotion.CMAcceleration" /></summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMAccelerometerData_Class/index.html">Apple documentation for <c>CMAccelerometerData</c></related>
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	[MacCatalyst (13, 1)]
	interface CMAccelerometerData : NSSecureCoding {
		/// <summary>The acceleration data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("acceleration")]
		CMAcceleration Acceleration { get; }
	}

	/// <summary>Accelerometer data stored by a <see cref="CoreMotion.CMSensorRecorder" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMRecordedAccelerometerData_class/index.html">Apple documentation for <c>CMRecordedAccelerometerData</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMAccelerometerData))]
	[DisableDefaultCtor]
	interface CMRecordedAccelerometerData {
		/// <summary>An identifier of the batch in which these data were recorded.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("identifier")]
		ulong Identifier { get; }

		/// <summary>The wall-time at which the accelerometer data was recorded.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("startDate")]
		NSDate StartDate { get; }
	}

	/// <summary>Base class for CMAccelerometerData, CMDeviceMotion and CMGyroData, it is used to attach a timestamp to these events.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMLogItem_Class/index.html">Apple documentation for <c>CMLogItem</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMLogItem : NSSecureCoding, NSCopying {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("timestamp")]
		double Timestamp { get; }
	}

	/// <param name="data">The accelerometer data for this event.</param>
	///     <param name="error">An error, or null if there have been no errors.</param>
	///     <summary>A delegate signature used to receive events from <see cref="CoreMotion.CMMotionManager" /></summary>
	///     <remarks>
	///       <para>
	/// This signature is used as an argument to the <see cref="CoreMotion.CMMotionManager" />'s StartAccelerometerUpdates.  
	/// </para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	///      var motionManager = new CMMotionManager ();
	///      motionManager.AccelerometerUpdateInterval = 0.01; // 100Hz
	///      motionManager.StartAccelerometerUpdates (NSOperationQueue.CurrentQueue, (data, error) => {
	///           Console.WriteLine ("Got an acceleration event {0}", data.Acceleration.X);
	///      });
	///   ]]></code>
	///       </example>
	///     </remarks>
	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void CMAccelerometerHandler ([NullAllowed] CMAccelerometerData data, [NullAllowed] NSError error);
	/// <param name="gyroData">The gyroscope data for this event.</param>
	///     <param name="error">Error or null if there are no errors.</param>
	///     <summary>A delegate signature used to receive events from <see cref="CoreMotion.CMMotionManager" /></summary>
	///     <remarks>
	///       <para>
	/// This signature is used as an argument to the <see cref="CoreMotion.CMMotionManager" />'s StartGyroUpdates.  
	/// </para>
	///       <para>
	/// For example:</para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	///      var motionManager = new CMMotionManager ();
	///      motionManager.AccelerometerUpdateInterval = 0.01; // 100Hz
	///      motionManager.StartDeviceMotionUpdates (NSOperationQueue.CurrentQueue, (gyroData, error) => {
	///           Console.WriteLine ("RotationRate={0}", gyroData.RotationRate);
	///      });
	///   ]]></code>
	///       </example>
	///     </remarks>
	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void CMGyroHandler ([NullAllowed] CMGyroData gyroData, [NullAllowed] NSError error);
	/// <include file="../docs/api/CoreMotion/CMDeviceMotionHandler.xml" path="/Documentation/Docs[@DocId='T:CoreMotion.CMDeviceMotionHandler']/*" />
	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void CMDeviceMotionHandler ([NullAllowed] CMDeviceMotion motion, [NullAllowed] NSError error);

	/// <include file="../docs/api/CoreMotion/CMMotionManager.xml" path="/Documentation/Docs[@DocId='T:CoreMotion.CMMotionManager']/*" />
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMMotionManager {
		/// <summary>Gets the current authorization status for access to motion sensor data.</summary>
		[NoTV, NoMac, iOS (27, 2), MacCatalyst (27, 2)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }

		/// <summary>Whether the device has an accelerometer.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("accelerometerAvailable")]
		bool AccelerometerAvailable { [Bind ("isAccelerometerAvailable")] get; }

		/// <summary>Whether the accelerometer is currently sending updates.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("accelerometerActive")]
		bool AccelerometerActive { [Bind ("isAccelerometerActive")] get; }

		/// <summary>The latest accelerometer data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("accelerometerData")]
		CMAccelerometerData AccelerometerData { get; }

		/// <summary>The duration, in seconds, between accelerometer data updates.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("accelerometerUpdateInterval")]
		double AccelerometerUpdateInterval { get; set; }

		/// <summary>Requests that the accelerometer begin delivering data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startAccelerometerUpdates")]
		void StartAccelerometerUpdates ();

		/// <param name="queue">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Requests that the accelerometer begin delivering data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startAccelerometerUpdatesToQueue:withHandler:")]
		void StartAccelerometerUpdates (NSOperationQueue queue, CMAccelerometerHandler handler);

		/// <summary>Requests that the accelerometer stop delivering data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("stopAccelerometerUpdates")]
		void StopAccelerometerUpdates ();

		/// <summary>The interval, in seconds, between device-motion data updates.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("deviceMotionUpdateInterval")]
		double DeviceMotionUpdateInterval { get; set; }

		/// <summary>Whether the device supports device-motion updates.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("deviceMotionAvailable")]
		bool DeviceMotionAvailable { [Bind ("isDeviceMotionAvailable")] get; }

		/// <summary>Whether the device is delivering device-motion updates.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("deviceMotionActive")]
		bool DeviceMotionActive { [Bind ("isDeviceMotionActive")] get; }

		/// <summary>The latest device-motion data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("deviceMotion")]
		CMDeviceMotion DeviceMotion { get; }

		/// <summary>The interval, in seconds, between gyroscopic data updates.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("gyroUpdateInterval")]
		double GyroUpdateInterval { get; set; }

		/// <summary>Whether the device has a gyroscope.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("gyroAvailable")]
		bool GyroAvailable { [Bind ("isGyroAvailable")] get; }

		/// <summary>Whether the device gyroscope is delivering updates.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("gyroActive")]
		bool GyroActive { [Bind ("isGyroActive")] get; }

		/// <summary>The latest gyroscopic data from the device.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("gyroData")]
		CMGyroData GyroData { get; }

		/// <summary>Requests that the gyroscope begin delivering data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startGyroUpdates")]
		void StartGyroUpdates ();

		/// <param name="toQueue">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Requests that the gyroscope begin delivering data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startGyroUpdatesToQueue:withHandler:")]
		void StartGyroUpdates (NSOperationQueue toQueue, CMGyroHandler handler);

		/// <summary>Requests that the gyroscope stop delivering data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("stopGyroUpdates")]
		void StopGyroUpdates ();

		/// <summary>Requests that the device begin delivering device-motion data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startDeviceMotionUpdates")]
		void StartDeviceMotionUpdates ();

		/// <param name="toQueue">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Requests that the device begin delivering device-motion data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startDeviceMotionUpdatesToQueue:withHandler:")]
		void StartDeviceMotionUpdates (NSOperationQueue toQueue, CMDeviceMotionHandler handler);

		/// <summary>Requests that the device stop delivering device-motion updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("stopDeviceMotionUpdates")]
		void StopDeviceMotionUpdates ();

		/// <summary>The interval, in seconds, between magnetometer updates.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("magnetometerUpdateInterval")]
		double MagnetometerUpdateInterval { get; set; }

		/// <summary>Whether the device has a magnetometer.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("magnetometerAvailable")]
		bool MagnetometerAvailable { [Bind ("isMagnetometerAvailable")] get; }

		/// <summary>Whether the magnetometer is delivering data updates.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("magnetometerActive")]
		bool MagnetometerActive { [Bind ("isMagnetometerActive")] get; }

		/// <summary>The latest magnetometer data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("magnetometerData")]
		CMMagnetometerData MagnetometerData { get; }

		/// <summary>Requests that the magnetometer begin delivering data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startMagnetometerUpdates")]
		void StartMagnetometerUpdates ();

		/// <param name="queue">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Requests that the gyroscope begin delivering data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startMagnetometerUpdatesToQueue:withHandler:")]
		void StartMagnetometerUpdates (NSOperationQueue queue, CMMagnetometerHandler handler);

		/// <summary>Requests that the magnetometer begin delivering data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("stopMagnetometerUpdates")]
		void StopMagnetometerUpdates ();

		/// <summary>Returns a bit-mask of all the available reference frames.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("availableAttitudeReferenceFrames"), Static]
		CMAttitudeReferenceFrame AvailableAttitudeReferenceFrames { get; }

		/// <summary>The current attitude reference frame.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("attitudeReferenceFrame")]
		CMAttitudeReferenceFrame AttitudeReferenceFrame { get; }

		/// <summary>Gets or sets the body used as the reference for device-motion updates.</summary>
		[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
		[NullAllowed, Export ("deviceMotionBody", ArgumentSemantic.Retain)]
		ICMBodyIdentifiable DeviceMotionBody { get; set; }

		/// <param name="referenceFrame">To be added.</param>
		///         <summary>Requests that the device begin delivering device-motion data updates, using <paramref name="referenceFrame" />.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startDeviceMotionUpdatesUsingReferenceFrame:")]
		void StartDeviceMotionUpdates (CMAttitudeReferenceFrame referenceFrame);

		/// <param name="referenceFrame">To be added.</param>
		///         <param name="queue">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Requests that the device begin delivering device-motion data updates.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startDeviceMotionUpdatesUsingReferenceFrame:toQueue:withHandler:")]
		void StartDeviceMotionUpdates (CMAttitudeReferenceFrame referenceFrame, NSOperationQueue queue, CMDeviceMotionHandler handler);

		/// <summary>Whether the device-movement display is shown.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("showsDeviceMovementDisplay")]
		bool ShowsDeviceMovementDisplay { get; set; }
	}

	/// <include file="../docs/api/CoreMotion/CMAttitude.xml" path="/Documentation/Docs[@DocId='T:CoreMotion.CMAttitude']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	//<quote>You access CMAttitude objects through the attitude property of each CMDeviceMotion objects passed to an application.</quote>
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMAttitude : NSSecureCoding, NSCopying {
		/// <summary>Device pitch in radians.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("pitch")]
		double Pitch { get; }

		/// <summary>Device yaw, in Radians.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("yaw")]
		double Yaw { get; }

		/// <summary>The device attitude as a rotation matrix.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("rotationMatrix")]
		CMRotationMatrix RotationMatrix { get; }

		/// <summary>Device attitude represented as a quaternion.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("quaternion")]
		CMQuaternion Quaternion { get; }

		/// <summary>Device roll in radians.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("roll")]
		double Roll { get; }

		/// <param name="attitude">To be added.</param>
		///         <summary>Multiplies the attitude by the specified attitude.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("multiplyByInverseOfAttitude:")]
		void MultiplyByInverseOfAttitude (CMAttitude attitude);
	}

	/// <summary>Encapsulates Attitude, Rotation Rate and Acceleration of the device.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMDeviceMotion_Class/index.html">Apple documentation for <c>CMDeviceMotion</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMDeviceMotion : NSSecureCoding {
		/// <summary>Returns the device's current rotation rate.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("rotationRate")]
		CMRotationRate RotationRate { get; }

		/// <summary>The gravity vector, expressed in the device's reference frame.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("gravity")]
		CMAcceleration Gravity { get; }

		/// <summary>Returns the current user-generated acceleration on the device (subtracting the gravity vector).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("userAcceleration")]
		CMAcceleration UserAcceleration { get; }

		/// <summary>The current <see cref="CoreMotion.CMAttitude" /> of the device.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("attitude")]
		CMAttitude Attitude { get; }

		/// <summary>Returns the device's current <see cref="CoreMotion.CMCalibratedMagneticField" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("magneticField")]
		CMCalibratedMagneticField MagneticField { get; }

		/// <summary>Gets the heading angle, in degrees in [0,360), relative to the reference frame.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("heading")]
		double Heading { get; }

		/// <summary>Gets the maximum deviation, in degrees, between the estimated heading and the actual heading.</summary>
		/// <value>A negative value indicates that the heading is invalid.</value>
		[iOS (27, 0)]
		[MacCatalyst (27, 0)]
		[Export ("headingAccuracy")]
		double HeadingAccuracy { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sensorLocation")]
		CMDeviceMotionSensorLocation SensorLocation { get; }
	}

	/// <summary>Historical device-motion data recorded for later retrieval.</summary>
	[NoTV, Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (CMDeviceMotion))]
	[DisableDefaultCtor]
	interface CMRecordedDeviceMotion {
		/// <summary>Gets the identifier of the sensor batch that contains this sample.</summary>
		[Export ("identifier")]
		ulong Identifier { get; }

		/// <summary>Gets the date and time when this sample was observed.</summary>
		[Export ("startDate")]
		NSDate StartDate { get; }
	}

	/// <summary>A single measurement of the device rotation rate.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMGyroData_Class/index.html">Apple documentation for <c>CMGyroData</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMGyroData : NSSecureCoding {
		/// <summary>The rotation rate of the device.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("rotationRate")]
		CMRotationRate RotationRate { get; }
	}

	/// <summary>A reading from the device magnetometer.</summary>
	///     <remarks>Instances containing the magnetometer data are delivered to the method provided to the <see cref="CoreMotion.CMMotionManager.StartMagnetometerUpdates(Foundation.NSOperationQueue,CoreMotion.CMMagnetometerHandler)" /> method.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMMagnetometerData_Class/index.html">Apple documentation for <c>CMMagnetometerData</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // will crash, see Extra.cs for compatibility stubs
	interface CMMagnetometerData : NSSecureCoding {
		/// <summary>Raw, uncalibrated magnetic field reading.</summary>
		///         <value>
		///
		/// Magnetic field information for x, y, z coordinates.
		///      </value>
		///         <remarks>This method returns the raw magnetic field measured by the device.  If you want a version of the magnetic field that has been filtered from the magnetic field introduced by the device, use the <see cref="CoreMotion.CMDeviceMotion.MagneticField" /> property instead.</remarks>
		[Export ("magneticField")]
		CMMagneticField MagneticField { get; }
	}

	/// <param name="magnetometerData">
	/// 
	/// 
	/// The magnetometer data being provider
	///   	   
	///   	   </param>
	///     <param name="error">
	/// 
	/// 
	/// Error, if any.
	///   	   
	///   	   </param>
	///     <summary>A delegate that is the <c>handler</c> for the <see cref="CoreMotion.CMMotionManager.StartMagnetometerUpdates(Foundation.NSOperationQueue,CoreMotion.CMMagnetometerHandler)" /> method.</summary>
	///     <remarks>Signature for a method that can receive magnetometer data from the CMMotionManager.</remarks>
	delegate void CMMagnetometerHandler ([NullAllowed] CMMagnetometerData magnetometerData, [NullAllowed] NSError error);

	/// <summary>The delegate for use with <see cref="CoreMotion.CMStepCounter.QueryStepCount(Foundation.NSDate,Foundation.NSDate,Foundation.NSOperationQueue,CoreMotion.CMStepQueryHandler)" />.</summary>
	[MacCatalyst (13, 1)]
	delegate void CMStepQueryHandler (nint numberOfSteps, [NullAllowed] NSError error);

	/// <summary>The delegate used with <see cref="CoreMotion.CMStepCounter.StartStepCountingUpdates(Foundation.NSOperationQueue,System.IntPtr,CoreMotion.CMStepUpdateHandler)" />.</summary>
	[MacCatalyst (13, 1)]
	delegate void CMStepUpdateHandler (nint numberOfSteps, NSDate timestamp, [NullAllowed] NSError error);

	/// <summary>On supported devices, stores the number of steps the user has taken with the device and allows step-counting to be dynamically tracked.</summary>
	///     <remarks>
	///       <para>Not all devices support step counting. Application developers should check <see cref="CoreMotion.CMStepCounter.IsStepCountingAvailable" />.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMStepCounter_class/index.html">Apple documentation for <c>CMStepCounter</c></related>
	[NoMac]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'CMPedometer' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CMPedometer' instead.")]
	interface CMStepCounter {

		/// <summary>Whether step counting is available.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("isStepCountingAvailable")]
		bool IsStepCountingAvailable { get; }

		/// <param name="start">To be added.</param>
		///         <param name="end">To be added.</param>
		///         <param name="queue">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Queries for step-counting data.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("queryStepCountStartingFrom:to:toQueue:withHandler:")]
		[Async (XmlDocs = """
			<param name="start">To be added.</param>
			<param name="end">To be added.</param>
			<param name="queue">To be added.</param>
			<summary>Queries for step-counting data.</summary>
			<returns>
			          <para>A task that represents the asynchronous QueryStepCount operation.   The value of the TResult parameter is a CoreMotion.CMStepQueryHandler.</para>
			        </returns>
			<remarks>
			          <para copied="true">The QueryStepCountAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void QueryStepCount (NSDate start, NSDate end, NSOperationQueue queue, CMStepQueryHandler handler);

		/// <param name="queue">To be added.</param>
		/// <param name="stepCounts">To be added.</param>
		/// <param name="handler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("startStepCountingUpdatesToQueue:updateOn:withHandler:")]
		void StartStepCountingUpdates (NSOperationQueue queue, nint stepCounts, CMStepUpdateHandler handler);

		/// <summary>End periodic updates of step-counting information.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("stopStepCountingUpdates")]
		void StopStepCountingUpdates ();
	}

	/// <summary>Holds data relating to walking and floor-climbing. Generated by <see cref="CoreMotion.CMPedometer" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMPedometerData_class/index.html">Apple documentation for <c>CMPedometerData</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMPedometerData : NSSecureCoding, NSCopying {

		/// <summary>The beginning time of the data interval.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("startDate")]
		NSDate StartDate { get; }

		/// <summary>The ending time of the data interval.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("endDate")]
		NSDate EndDate { get; }

		/// <summary>The number of steps taken.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("numberOfSteps")]
		NSNumber NumberOfSteps { get; }

		/// <summary>The estimated distance, in meters, traveled by the user.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("distance")]
		NSNumber Distance { get; }

		/// <summary>The number of floors walked up.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("floorsAscended")]
		NSNumber FloorsAscended { get; }

		/// <summary>The number of floors walked down.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("floorsDescended")]
		NSNumber FloorsDescended { get; }

		/// <summary>The user's current speed, in meters per second.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("currentPace")]
		NSNumber CurrentPace { get; }

		/// <summary>The user's current cadence, in steps per second.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("currentCadence")]
		NSNumber CurrentCadence { get; }

		/// <summary>Gets the average pace of the user when the user is moving, in seconds per meter.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("averageActivePace")]
		NSNumber AverageActivePace { get; }
	}

	/// <summary>Generates step-counting data, including number of steps and floors climbed/descended.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMPedometer_class/index.html">Apple documentation for <c>CMPedometer</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMPedometer {

		/// <summary>Whether the device has step-counting capabilities.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("isStepCountingAvailable")]
		bool IsStepCountingAvailable { get; }

		/// <summary>Whether the device can estimate walking distance.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("isDistanceAvailable")]
		bool IsDistanceAvailable { get; }

		/// <summary>Whether the device can track floors climbed throughout the day.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("isFloorCountingAvailable")]
		bool IsFloorCountingAvailable { get; }

		/// <param name="start">To be added.</param>
		///         <param name="end">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Requests pedometer data for the specified range.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("queryPedometerDataFromDate:toDate:withHandler:")]
		[Async (XmlDocs = """
			<param name="start">To be added.</param>
			<param name="end">To be added.</param>
			<summary>Requests pedometer data for the specified range.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous QueryPedometerData operation.  The value of the TResult parameter is of type System.Action&lt;CoreMotion.CMPedometerData,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void QueryPedometerData (NSDate start, NSDate end, Action<CMPedometerData, NSError> handler);

		/// <param name="start">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Requests that the pedometer begin sending periodic updates to the app.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startPedometerUpdatesFromDate:withHandler:")]
		[Async (XmlDocs = """
			<param name="start">To be added.</param>
			<summary>Requests that the pedometer begin sending periodic updates to the app.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous StartPedometerUpdates operation.  The value of the TResult parameter is of type System.Action&lt;CoreMotion.CMPedometerData,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void StartPedometerUpdates (NSDate start, Action<CMPedometerData, NSError> handler);

		/// <summary>Requests that the pedometer stop sending periodic updates to the app.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("stopPedometerUpdates")]
		void StopPedometerUpdates ();

		/// <summary>Whether the device can determine the user's pace.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("isPaceAvailable")]
		bool IsPaceAvailable { get; }

		/// <summary>Whether the device can deliver cadence data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("isCadenceAvailable")]
		bool IsCadenceAvailable { get; }

		/// <summary>Gets a value that tells whether pedometer tracking data is available on the device.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("isPedometerEventTrackingAvailable")]
		bool IsPedometerEventTrackingAvailable { get; }

		/// <param name="handler">An event handler for pedometer update events.</param>
		///         <summary>Starts handling updates to pedestrian data.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<summary>Starts handling updates to pedestrian data.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous StartPedometerEventUpdates operation.  The value of the TResult parameter is of type System.Action&lt;CoreMotion.CMPedometerEvent,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The StartPedometerEventUpdatesAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("startPedometerEventUpdatesWithHandler:")]
		void StartPedometerEventUpdates (Action<CMPedometerEvent, NSError> handler);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("stopPedometerEventUpdates")]
		void StopPedometerEventUpdates ();

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }
	}

	/// <summary>The delegate that acts as the handler for <see cref="CoreMotion.CMMotionActivityManager.QueryActivity(Foundation.NSDate,Foundation.NSDate,Foundation.NSOperationQueue,CoreMotion.CMMotionActivityQueryHandler)" />.</summary>
	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void CMMotionActivityHandler ([NullAllowed] CMMotionActivity activity);

	/// <summary>The delegate that is the handler for <see cref="CoreMotion.CMMotionActivityManager.StartActivityUpdates(Foundation.NSOperationQueue,CoreMotion.CMMotionActivityHandler)" />.</summary>
	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void CMMotionActivityQueryHandler ([NullAllowed] CMMotionActivity [] activities, [NullAllowed] NSError error);

	/// <summary>On supported devices, accesses stored motion data and allows monitoring of motion updates.</summary>
	///     <remarks>
	///       <para>Not everty device supports stored motion data. Application developers should check <see cref="CoreMotion.CMMotionActivityManager.IsActivityAvailable" />.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMMotionActivityManager_class/index.html">Apple documentation for <c>CMMotionActivityManager</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMMotionActivityManager {

		/// <summary>Whether stored motion data is available.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("isActivityAvailable")]
		bool IsActivityAvailable { get; }

		/// <param name="start">To be added.</param>
		///         <param name="end">To be added.</param>
		///         <param name="queue">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Queries the device for stored motion activity.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("queryActivityStartingFromDate:toDate:toQueue:withHandler:")]
		[Async (XmlDocs = """
			<param name="start">To be added.</param>
			<param name="end">To be added.</param>
			<param name="queue">To be added.</param>
			<summary>Queries the device for stored motion activity.</summary>
			<returns>
			          <para>A task that represents the asynchronous QueryActivity operation.   The value of the TResult parameter is a <see cref="CoreMotion.CMMotionActivityQueryHandler" />.</para>
			        </returns>
			<remarks>
			          <para copied="true">The QueryActivityAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void QueryActivity (NSDate start, NSDate end, NSOperationQueue queue, CMMotionActivityQueryHandler handler);

		/// <param name="queue">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Begins periodically sending motion activity data to the app.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startActivityUpdatesToQueue:withHandler:")]
		void StartActivityUpdates (NSOperationQueue queue, CMMotionActivityHandler handler);

		/// <summary>Stops periodically sending motion activity data to the app.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("stopActivityUpdates")]
		void StopActivityUpdates ();

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }
	}

	/// <summary>A motion update event, including characterization of the likely form (walking, running, automotive).</summary>
	///     
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMMotionActivity_class/index.html">Apple documentation for <c>CMMotionActivity</c></related>
	[Mac (15, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // <quote>You do not create instances of this class yourself.</quote>
	interface CMMotionActivity : NSCopying, NSSecureCoding {
		/// <summary>The confidence in the assessement of the motion type.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("confidence")]
		CMMotionActivityConfidence Confidence { get; }

		/// <summary>The beginning of the data interval.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		/// <summary>The device's motion cannot be characterized.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("unknown")]
		bool Unknown { get; }

		/// <summary>The user is not moving a significant amount.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("stationary")]
		bool Stationary { get; }

		/// <summary>The user is walking with the device.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("walking")]
		bool Walking { get; }

		/// <summary>The user is running with the device.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("running")]
		bool Running { get; }

		/// <summary>Whether the device is in an automobile.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("automotive")]
		bool Automotive { get; }

		/// <summary>The device is on a bicycle.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("cycling")]
		bool Cycling { get; }
	}

	/// <summary>Holds relative-altitude data, generated by the <see cref="CoreMotion.CMAltitudeData" /> class.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMAltitudeData_class/index.html">Apple documentation for <c>CMAltitudeData</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor] // this does not look to be meant to be user created (and crash when description is called)
	interface CMAltitudeData {
		/// <summary>Delta, in meters, since the last time this method was called.</summary>
		///         <value>The first time this method is called, the value will be 0.</value>
		///         <remarks>To be added.</remarks>
		[Export ("relativeAltitude")]
		NSNumber RelativeAltitude { get; }

		/// <summary>The current pressure, in kilopascals.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("pressure")]
		NSNumber Pressure { get; }
	}

	/// <summary>Interface to the relative-altitude pressure sensor.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMAltimeter_class/index.html">Apple documentation for <c>CMAltimeter</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMAltimeter {
		/// <summary>Whether the device has a barometer.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("isRelativeAltitudeAvailable")]
		bool IsRelativeAltitudeAvailable { get; }

		/// <param name="queue">To be added.</param>
		///         <param name="handler">To be added.</param>
		///         <summary>Requests periodic updates of altitude-adjustment data.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("startRelativeAltitudeUpdatesToQueue:withHandler:")]
		[Async (XmlDocs = """
			<param name="queue">To be added.</param>
			<summary>Requests periodic updates of altitude-adjustment data.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous StartRelativeAltitudeUpdates operation.  The value of the TResult parameter is of type System.Action&lt;CoreMotion.CMAltitudeData,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The StartRelativeAltitudeUpdatesAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void StartRelativeAltitudeUpdates (NSOperationQueue queue, Action<CMAltitudeData, NSError> handler);

		/// <summary>Stops periodic updates of altitude-adjustment data.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("stopRelativeAltitudeUpdates")]
		void StopRelativeAltitudeUpdates ();

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }

		[NoTV, NoMac, iOS (15, 0)]
		[NoMacCatalyst]
		[Static]
		[Export ("isAbsoluteAltitudeAvailable")]
		bool IsAbsoluteAltitudeAvailable { get; }

		[NoTV, NoMac, iOS (15, 0)]
		[NoMacCatalyst]
		[Export ("startAbsoluteAltitudeUpdatesToQueue:withHandler:")]
		void StartAbsoluteAltitudeUpdates (NSOperationQueue queue, Action<CMAbsoluteAltitudeData, NSError> handler);

		[NoTV, NoMac, iOS (15, 0)]
		[NoMacCatalyst]
		[Export ("stopAbsoluteAltitudeUpdates")]
		void StopAbsoluteAltitudeUpdates ();
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum CMAuthorizationStatus : long {
		/// <summary>To be added.</summary>
		NotDetermined = 0,
		/// <summary>To be added.</summary>
		Restricted,
		/// <summary>To be added.</summary>
		Denied,
		/// <summary>To be added.</summary>
		Authorized,
	}

	[iOS (15, 4), MacCatalyst (15, 4), TV (15, 4)]
	[Native]
	public enum CMOdometerOriginDevice : long {
		Unknown = 0,
		Local,
		Remote,
	}


	/// <summary>A system-provided list of objects that contain recorded Core Motion accelerometer data.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMSensorDataList_class/index.html">Apple documentation for <c>CMSensorDataList</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMSensorDataList /* NSFastEnumeration */
	{
	}

	/// <summary>Gathers and retrieves accelerometer data.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreMotion/Reference/CMSensorRecorder_class/index.html">Apple documentation for <c>CMSensorRecorder</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMSensorRecorder {
		/// <summary>Whether accelerometer recording is available on the device.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[MacCatalyst (13, 1)]
		[Export ("isAccelerometerRecordingAvailable")]
		bool IsAccelerometerRecordingAvailable { get; }

		/// <summary>Developers should not use this deprecated property. Developers should use 'CMSensorRecorder.AuthorizationStatus' instead.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CMSensorRecorder.AuthorizationStatus' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CMSensorRecorder.AuthorizationStatus' instead.")]
		[Static]
		[Export ("isAuthorizedForRecording")]
		bool IsAuthorizedForRecording { get; }

		/// <param name="fromDate">To be added.</param>
		///         <param name="toDate">To be added.</param>
		///         <summary>Retrieves accelerometer data for the specified time interval.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("accelerometerDataFromDate:toDate:")]
		[return: NullAllowed]
		CMSensorDataList GetAccelerometerData (NSDate fromDate, NSDate toDate);

		/// <param name="duration">To be added.</param>
		///         <summary>Begins recording sensor data for <paramref name="duration" /> seconds.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("recordAccelerometerForDuration:")]
		void RecordAccelerometer (double duration);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CMPedometerEventType : long {
		/// <summary>To be added.</summary>
		Pause,
		/// <summary>To be added.</summary>
		Resume,
	}

	/// <related type="externalDocumentation" href="https://developer.apple.com/reference/CoreMotion/CMPedometerEvent">Apple documentation for <c>CMPedometerEvent</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // instances exposed from delegate
	interface CMPedometerEvent : NSSecureCoding, NSCopying {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("date")]
		NSDate Date { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("type")]
		CMPedometerEventType Type { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CMDyskineticSymptomResult : NSCopying, NSSecureCoding {

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("percentUnlikely")]
		float PercentUnlikely { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("percentLikely")]
		float PercentLikely { get; }
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CMTremorResult : NSCopying, NSSecureCoding {

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("percentUnknown")]
		float PercentUnknown { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("percentNone")]
		float PercentNone { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("percentSlight")]
		float PercentSlight { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("percentMild")]
		float PercentMild { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("percentModerate")]
		float PercentModerate { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("percentStrong")]
		float PercentStrong { get; }
	}

	[NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	delegate void CMDyskineticSymptomResultHandler (CMDyskineticSymptomResult [] dyskineticSymptomResult, NSError error);

	[NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	delegate void CMTremorResultHandler (CMTremorResult [] tremorResults, NSError error);

	[NoTV, NoMac, NoiOS]
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

	/// <summary>Type for CoreMotion errors.</summary>
	[MacCatalyst (13, 1)]
	[ErrorDomain ("CMErrorDomain")]
	// untyped enum -> CMError.h
	public enum CMError {
		/// <summary>No error.</summary>
		Null = 100,
		/// <summary>The device must move to generate motion data.</summary>
		DeviceRequiresMovement,
		/// <summary>The device does not know the direction to true north.</summary>
		TrueNorthNotAvailable,
		/// <summary>An error of unknown type.</summary>
		Unknown,
		/// <summary>Motion data is not available on this device.</summary>
		MotionActivityNotAvailable,
		/// <summary>The user has not authorized the app to access motion data.</summary>
		MotionActivityNotAuthorized,
		/// <summary>The app does not have a properly-configured entitlements file.</summary>
		MotionActivityNotEntitled,
		/// <summary>An invalid parameter was specified.</summary>
		InvalidParameter,
		/// <summary>An invalid action was specified.</summary>
		InvalidAction,
		/// <summary>The requested data are not available.</summary>
		NotAvailable,
		/// <summary>The app does not have the proper entitlements file.</summary>
		NotEntitled,
		/// <summary>The app is not currently authorized to access motion data.</summary>
		NotAuthorized,
		NilData,
		Size,
	}

	/// <summary>An enumeration whose values specify the reference frame from which <see cref="CoreMotion.CMAttitude" /> readings are referenced.</summary>
	[NoMac]
	[MacCatalyst (13, 1)]
	// untyped enum -> CMAttitude.h
	// in Xcode 6.3 SDK is became an NSUInteger
	[Flags]
	[Native]
	public enum CMAttitudeReferenceFrame : ulong {
		/// <summary>Z axis is vertical; X axis is in any direction. </summary>
		XArbitraryZVertical = 1 << 0,
		/// <summary>Z axis is vertical; X axis is in any direction. Magnetometer is used to correct long-term yaw. A reference frame in which the magnetometer (if available) is used to improve the long-term yaw.</summary>
		XArbitraryCorrectedZVertical = 1 << 1,
		/// <summary>Z axis is vertical; X axis is magnetic north. Magnetometer is used to correct long-term yaw.</summary>
		XMagneticNorthZVertical = 1 << 2,
		/// <summary>Z axis is vertical; X axis is true north. Magnetometer is used to correct long-term yaw.</summary>
		XTrueNorthZVertical = 1 << 3,
	}

	/// <summary>An enumeration whose values specify the confidence in a <see cref="CoreMotion.CMMotionActivity" />.</summary>
	[Mac (15, 0)]
	[MacCatalyst (13, 1)]
	// NSInteger -> CMMotionActivity.h
	[Native]
	public enum CMMotionActivityConfidence : long {
		/// <summary>Confidence is low.</summary>
		Low = 0,
		/// <summary>Confidence is moderate.</summary>
		Medium,
		/// <summary>Confidence is high.</summary>
		High,
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum CMDeviceMotionSensorLocation : long {
		Default,
		HeadphoneLeft,
		HeadphoneRight,
	}

	[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum CMHighFrequencyHeartRateDataConfidence : long {
		Low = 0,
		Medium,
		High,
		Highest,
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[NoMac]
	delegate void CMHeadphoneDeviceMotionHandler ([NullAllowed] CMDeviceMotion motion, [NullAllowed] NSError error);

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
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

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("connectionStatusActive")]
		bool ConnectionStatusActive { [Bind ("isConnectionStatusActive")] get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("startConnectionStatusUpdates")]
		void StartConnectionStatusUpdates ();

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("stopConnectionStatusUpdates")]
		void StopConnectionStatusUpdates ();
	}

	interface ICMHeadphoneMotionManagerDelegate { }

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CMHeadphoneMotionManagerDelegate {

		[Export ("headphoneMotionManagerDidConnect:")]
		void DidConnect (CMHeadphoneMotionManager manager);

		[Export ("headphoneMotionManagerDidDisconnect:")]
		void DidDisconnect (CMHeadphoneMotionManager manager);
	}

	[NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor]
	interface CMRotationRateData {

		[Export ("rotationRate")]
		CMRotationRate RotationRate { get; }
	}

	[NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (CMRotationRateData))]
	[DisableDefaultCtor]
	interface CMRecordedRotationRateData {

		[Export ("startDate")]
		NSDate StartDate { get; }
	}

	[NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	[Native]
	enum CMFallDetectionEventUserResolution : long {
		Confirmed,
		Dismissed,
		Rejected,
		Unresponsive,
	}

	[NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CMFallDetectionEvent {

		[Export ("date")]
		NSDate Date { get; }

		[Export ("resolution")]
		CMFallDetectionEventUserResolution Resolution { get; }
	}

	[NoTV, NoMac, NoiOS]
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

	[NoTV, NoMac, NoiOS]
	[NoMacCatalyst]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CMFallDetectionDelegate {

		[Export ("fallDetectionManager:didDetectEvent:completionHandler:")]
		void DidDetectEvent (CMFallDetectionManager fallDetectionManager, CMFallDetectionEvent @event, Action handler);

		[Export ("fallDetectionManagerDidChangeAuthorization:")]
		void DidChangeAuthorization (CMFallDetectionManager fallDetectionManager);
	}

	[DisableDefaultCtor] // will crash
	[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
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

	[NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
	[BaseType (typeof (CMLogItem))]
	[DisableDefaultCtor]
	interface CMAmbientPressureData {

		[Export ("pressure", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitPressure> Pressure { get; }

		[Export ("temperature", ArgumentSemantic.Strong)]
		NSMeasurement<NSUnitTemperature> Temperature { get; }
	}

	[NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
	[BaseType (typeof (CMAmbientPressureData))]
	[DisableDefaultCtor]
	interface CMRecordedPressureData {

		[Export ("identifier")]
		ulong Identifier { get; }

		[Export ("startDate")]
		NSDate StartDate { get; }
	}

	[iOS (16, 0), NoMacCatalyst, NoTV]
	[Native]
	public enum CMWaterSubmersionState : long {
		Unknown = 0,
		NotSubmerged,
		Submerged,
	}

	[iOS (16, 0), NoMacCatalyst, NoTV]
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

	[NoMac, iOS (16, 0), NoMacCatalyst, NoTV]
	[BaseType (typeof (NSObject))]
	interface CMWaterSubmersionEvent : NSSecureCoding, NSCopying {
		[Export ("date")]
		NSDate Date { get; }

		[Export ("state")]
		CMWaterSubmersionState State { get; }
	}

	[NoMac, iOS (16, 0), NoMacCatalyst, NoTV]
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

	[NoMac, iOS (16, 0), NoMacCatalyst, NoTV]
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

	[NoMac, iOS (16, 0), NoMacCatalyst, NoTV]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CMWaterSubmersionManagerDelegate {
		/// <summary>Notifies the delegate that a new submersion event is available.</summary>
		[Export ("manager:didUpdateEvent:")]
		void DidUpdateEvent (CMWaterSubmersionManager manager, CMWaterSubmersionEvent @event);

		/// <summary>Notifies the delegate that a new depth or pressure measurement is available.</summary>
		[Export ("manager:didUpdateMeasurement:")]
		void DidUpdateMeasurement (CMWaterSubmersionManager manager, CMWaterSubmersionMeasurement measurement);

		/// <summary>Notifies the delegate that a new temperature measurement is available.</summary>
		[Export ("manager:didUpdateTemperature:")]
		void DidUpdateTemperature (CMWaterSubmersionManager manager, CMWaterTemperature measurement);

		/// <summary>Notifies the delegate that an error occurred.</summary>
		[Export ("manager:errorOccurred:")]
		void ErrorOccurred (CMWaterSubmersionManager manager, NSError error);
	}

	[NoMac, iOS (16, 0), NoMacCatalyst, NoTV]
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

	[NoMac, iOS (17, 0), MacCatalyst (17, 0)]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CMOdometerData : NSSecureCoding, NSCopying {
		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("deltaDistance")]
		double DeltaDistance { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("deltaDistanceAccuracy")]
		double DeltaDistanceAccuracy { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("speed")]
		double Speed { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("speedAccuracy")]
		double SpeedAccuracy { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("gpsDate", ArgumentSemantic.Strong)]
		NSDate GpsDate { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("deltaAltitude")]
		double DeltaAltitude { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("verticalAccuracy")]
		double VerticalAccuracy { get; }

		[iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("originDevice", ArgumentSemantic.Assign)]
		CMOdometerOriginDevice OriginDevice { get; }

		[iOS (15, 4), MacCatalyst (15, 4)]
		[NullAllowed]
		[Export ("slope", ArgumentSemantic.Strong)]
		NSNumber Slope { get; }

		[iOS (15, 4), MacCatalyst (15, 4)]
		[NullAllowed]
		[Export ("maxAbsSlope", ArgumentSemantic.Strong)]
		NSNumber MaxAbsSlope { get; }
	}

	[NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CMLogItem))]
	interface CMHighFrequencyHeartRateData {
		[Export ("heartRate")]
		double HeartRate { get; }

		[Export ("confidence")]
		CMHighFrequencyHeartRateDataConfidence Confidence { get; }

		[NullAllowed, Export ("date")]
		NSDate Date { get; }
	}

	[Native]
	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	enum CMHeadphoneActivityStatus : long {
		Disconnected = 0,
		Connected,
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	delegate void CMHeadphoneActivityStatusHandler (CMHeadphoneActivityStatus status, [NullAllowed] NSError error);

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	delegate void CMHeadphoneActivityHandler ([NullAllowed] CMMotionActivity activity, [NullAllowed] NSError error);

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	interface CMHeadphoneActivityManager {
		[Static]
		[Export ("authorizationStatus")]
		CMAuthorizationStatus AuthorizationStatus { get; }

		[Export ("isActivityAvailable")]
		bool IsActivityAvailable { get; }

		[Export ("isActivityActive")]
		bool IsActivityActive { get; }

		[Export ("isStatusAvailable")]
		bool IsStatusAvailable { get; }

		[Export ("isStatusActive")]
		bool IsStatusActive { get; }

		[Export ("startActivityUpdatesToQueue:withHandler:")]
		void StartActivityUpdates (NSOperationQueue queue, CMHeadphoneActivityHandler handler);

		[Export ("stopActivityUpdates")]
		void StopActivityUpdates ();

		[Export ("startStatusUpdatesToQueue:withHandler:")]
		void StartStatusUpdates (NSOperationQueue queue, CMHeadphoneActivityStatusHandler handler);

		[Export ("stopStatusUpdates")]
		void StopStatusUpdates ();
	}

	interface ICMBodyIdentifiable { }

	[NoTV, Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
	[Protocol]
	interface CMBodyIdentifiable {
	}
}
