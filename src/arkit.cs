//
// ARKit bindings
//
// Authors:
//	Vincent Dondain  <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//

using System;
using System.ComponentModel;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreLocation;
using CoreVideo;
using Foundation;
using ObjCRuntime;
using ImageIO;
using Metal;
using QuickLook;
using SpriteKit;
using SceneKit;
using UIKit;

#if NET
using Vector2 = global::System.Numerics.Vector2;
using Vector3 = global::CoreGraphics.NVector3;
using Matrix3 = global::CoreGraphics.NMatrix3;
using Matrix4 = global::CoreGraphics.NMatrix4;
#else
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.NVector3;
using Matrix3 = global::OpenTK.NMatrix3;
using Matrix4 = global::OpenTK.NMatrix4;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ARKit {

	[NoWatch, NoTV, NoMac]
	[Native]
	public enum ARTrackingState : long {
		NotAvailable,
		Limited,
		Normal,
	}

	[NoWatch, NoTV, NoMac]
	[Native]
	public enum ARTrackingStateReason : long {
		None,
		Initializing,
		ExcessiveMotion,
		InsufficientFeatures,
		[iOS (11, 3)]
		Relocalizing,
	}

	[NoWatch, NoTV, NoMac]
	[ErrorDomain ("ARErrorDomain")]
	[Native]
	public enum ARErrorCode : long {
		UnsupportedConfiguration = 100,
		SensorUnavailable = 101,
		SensorFailed = 102,
		CameraUnauthorized = 103,
		MicrophoneUnauthorized = 104,
		LocationUnauthorized = 105,
		HighResolutionFrameCaptureInProgress = 106,
		HighResolutionFrameCaptureFailed = 107,
		WorldTrackingFailed = 200,
		GeoTrackingNotAvailableAtLocation = 201,
		GeoTrackingFailed = 202,
		InvalidReferenceImage = 300,
		InvalidReferenceObject = 301,
		InvalidWorldMap = 302,
		InvalidConfiguration = 303,
#if !NET
		[Obsolete ("Please use the 'InvalidCollaborationData' value instead.")]
		CollaborationDataUnavailable = InvalidCollaborationData,
#endif
		InvalidCollaborationData = 304,
		InsufficientFeatures = 400,
		ObjectMergeFailed = 401,
		FileIOFailed = 500,
		RequestFailed = 501,
	}

	[NoWatch, NoTV, NoMac]
	[Flags]
	[Native]
	public enum ARHitTestResultType : ulong {
		FeaturePoint = 1 << 0,
		EstimatedHorizontalPlane = 1 << 1,
		[iOS (11, 3)]
		EstimatedVerticalPlane = 1 << 2,
		ExistingPlane = 1 << 3,
		ExistingPlaneUsingExtent = 1 << 4,
		[iOS (11, 3)]
		ExistingPlaneUsingGeometry = 1 << 5,
	}

	[NoWatch, NoTV, NoMac]
	[Native]
	public enum ARPlaneAnchorAlignment : long {
		Horizontal,
		[iOS (11, 3)]
		Vertical,
	}

	[NoWatch, NoTV, NoMac]
	[Flags]
	[Native]
	public enum ARSessionRunOptions : ulong {
		None = 0,
		ResetTracking = 1 << 0,
		RemoveExistingAnchors = 1 << 1,
		StopTrackedRaycasts = 1 << 2,
		[iOS (13, 4)]
		ResetSceneReconstruction = (1 << 3),
	}

	[NoWatch, NoTV, NoMac]
	[Native]
	public enum ARWorldAlignment : long {
		Gravity,
		GravityAndHeading,
		Camera,
	}

	[NoWatch, NoTV, NoMac]
	[Flags]
	[Native]
	public enum ARPlaneDetection : ulong {
		None = 0,
		Horizontal = 1 << 0,
		[iOS (11, 3)]
		Vertical = 1 << 1,
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[Native]
	public enum AREnvironmentTexturing : long {
		None,
		Manual,
		Automatic,
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[Native]
	public enum ARWorldMappingStatus : long {
		NotAvailable,
		Limited,
		Extending,
		Mapped,
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[Native]
	public enum ARPlaneClassificationStatus : long {
		NotAvailable = 0,
		Undetermined,
		Unknown,
		Known,
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[Native]
	public enum ARPlaneClassification : long {
		None = 0,
		Wall,
		Floor,
		Ceiling,
		Table,
		Seat,
		[iOS (13, 0)]
		Window,
		[iOS (13, 0)]
		Door,
	}

	[iOS (13, 0)]
	[Native]
	public enum ARCoachingGoal : long {
		Tracking,
		HorizontalPlane,
		VerticalPlane,
		AnyPlane,
		[iOS (15, 0)]
		GeoTracking,
	}

	[iOS (13, 0)]
	[Flags]
	[Native]
	public enum ARFrameSemantics : long {
		None = 0x0,
		PersonSegmentation = 1 << 0,
		PersonSegmentationWithDepth = (1 << 1) | (1 << 0),
		BodyDetection = 1 << 2,
		[iOS (14, 0)]
		SceneDepth = (1 << 3),
		[iOS (14, 0)]
		SmoothedSceneDepth = (1 << 4),
	}

	[iOS (13, 0)]
	[Native]
	public enum ARMatteResolution : long {
		Full = 0,
		Half = 1,
	}

	[iOS (13, 0)]
	[Native]
	public enum ARRaycastTarget : long {
		ExistingPlaneGeometry,
		ExistingPlaneInfinite,
		EstimatedPlane,
	}

	[iOS (13, 0)]
	[Native]
	public enum ARRaycastTargetAlignment : long {
		Horizontal,
		Vertical,
		Any,
	}

	[iOS (13, 0)]
	public enum ARSegmentationClass : byte {
		None = 0,
		Person = 255,
	}

	[iOS (13, 0)]
	[Native]
	public enum ARCollaborationDataPriority : long {
		Critical,
		Optional,
	}


	[iOS (14, 0)]
	[Native]
	public enum ARAltitudeSource : long {
		Unknown,
		Coarse,
		Precise,
		UserDefined,
	}

	[iOS (14, 0)]
	[Native]
	public enum ARConfidenceLevel : long {
		Low,
		Medium,
		High,
	}

	[iOS (14, 0)]
	[Native]
	public enum ARGeoTrackingAccuracy : long {
		Undetermined,
		Low,
		Medium,
		High,
	}

	[iOS (14, 0)]
	[Native]
	public enum ARGeoTrackingState : long {
		NotAvailable,
		Initializing,
		Localizing,
		Localized,
	}

	[iOS (14, 0)]
	[Native]
	public enum ARGeoTrackingStateReason : long {
		None,
		NotAvailableAtLocation,
		NeedLocationPermissions,
		WorldTrackingUnstable,
		WaitingForLocation,
		WaitingForAvailabilityCheck,
		GeoDataNotLoaded,
		DevicePointedTooLow,
		VisualLocalizationFailed,
	}

	[iOS (14, 3)]
	[Native]
	public enum ARAppClipCodeUrlDecodingState : long {
		Decoding,
		Failed,
		Decoded,
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[Protocol]
	[Advice ("To conform to 'ARAnchorCopying' you need to implement:\n'[Export (\"initWithAnchor:\")]'\n'public YourConstructor (ARAnchor anchor)'")]
	interface ARAnchorCopying : NSCopying {
		// Constructors in interfaces are not possible in C#
		// @required -(instancetype _Nonnull)initWithAnchor:(ARAnchor * _Nonnull)anchor;
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARAnchor : ARAnchorCopying, NSSecureCoding {

		[Export ("identifier")]
		NSUuid Identifier { get; }

		[iOS (12, 0)]
		[NullAllowed, Export ("name")]
		string Name { get; }

		[iOS (13, 0)]
		[NullAllowed, Export ("sessionIdentifier")]
		NSUuid SessionIdentifier { get; }

		[Export ("transform")]
		Matrix4 Transform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("initWithTransform:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (Matrix4 transform);

		[iOS (12, 0)]
		[Export ("initWithName:transform:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (string name, Matrix4 transform);

		// Inlined from 'ARAnchorCopying' protocol (we can't have constructors in interfaces)
		[iOS (12, 0)]
		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARCamera : NSCopying {

		[Export ("transform")]
		Matrix4 Transform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("eulerAngles")]
		Vector3 EulerAngles {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("trackingState")]
		ARTrackingState TrackingState { get; }

		[Export ("trackingStateReason")]
		ARTrackingStateReason TrackingStateReason { get; }

		[Export ("intrinsics")]
		Matrix3 Intrinsics {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("imageResolution")]
		CGSize ImageResolution { get; }

		[iOS (13, 0)]
		[Export ("exposureDuration")]
		double ExposureDuration { get; }

		[iOS (13, 0)]
		[Export ("exposureOffset")]
		float ExposureOffset { get; }

		[Export ("projectionMatrix")]
		Matrix4 ProjectionMatrix {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

#if !NET
		[Obsolete ("Use 'Project' instead.")]
		[Wrap ("Project (point, orientation, viewportSize)", IsVirtual = true)]
		CGPoint GetProjectPoint (Vector3 point, UIInterfaceOrientation orientation, CGSize viewportSize);
#endif

		[Export ("projectPoint:orientation:viewportSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		CGPoint Project (Vector3 point, UIInterfaceOrientation orientation, CGSize viewportSize);

		[iOS (12, 0)]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[Export ("unprojectPoint:ontoPlaneWithTransform:orientation:viewportSize:")]
		Vector3 Unproject (CGPoint point, Matrix4 planeTransform, UIInterfaceOrientation orientation, CGSize viewportSize);

		[Export ("projectionMatrixForOrientation:viewportSize:zNear:zFar:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Matrix4 GetProjectionMatrix (UIInterfaceOrientation orientation, CGSize viewportSize, nfloat zNear, nfloat zFar);

		[Export ("viewMatrixForOrientation:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Matrix4 GetViewMatrix (UIInterfaceOrientation orientation);
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARFrame : NSCopying {

		[Export ("timestamp")]
		double Timestamp { get; }

		[Export ("capturedImage")]
		CVPixelBuffer CapturedImage { get; }

		[iOS (13, 0)]
		[NullAllowed, Export ("cameraGrainTexture")]
		IMTLTexture CameraGrainTexture { get; }

		[iOS (13, 0)]
		[Export ("cameraGrainIntensity")]
		float CameraGrainIntensity { get; }

		[NullAllowed, Export ("capturedDepthData", ArgumentSemantic.Strong)]
		AVDepthData CapturedDepthData { get; }

		[Export ("capturedDepthDataTimestamp")]
		double CapturedDepthDataTimestamp { get; }

		[Export ("camera", ArgumentSemantic.Copy)]
		ARCamera Camera { get; }

		[Export ("anchors", ArgumentSemantic.Copy)]
		ARAnchor [] Anchors { get; }

		[NullAllowed, Export ("lightEstimate", ArgumentSemantic.Strong)]
		ARLightEstimate LightEstimate { get; }

		[NullAllowed, Export ("rawFeaturePoints", ArgumentSemantic.Strong)]
		ARPointCloud RawFeaturePoints { get; }

		[iOS (12, 0)]
		[Export ("worldMappingStatus")]
		ARWorldMappingStatus WorldMappingStatus { get; }

		[iOS (13, 0)]
		[NullAllowed, Export ("segmentationBuffer")]
		CVPixelBuffer SegmentationBuffer { get; }

		[iOS (13, 0)]
		[NullAllowed, Export ("estimatedDepthData")]
		CVPixelBuffer EstimatedDepthData { get; }

		[iOS (13, 0)]
		[NullAllowed, Export ("detectedBody")]
		ARBody2D DetectedBody { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'ARSession.Raycast' instead.")]
		[Export ("hitTest:types:")]
		ARHitTestResult [] HitTest (CGPoint point, ARHitTestResultType types);

		[iOS (13, 0)]
		[Export ("raycastQueryFromPoint:allowingTarget:alignment:")]
		ARRaycastQuery CreateRaycastQuery (CGPoint point, ARRaycastTarget target, ARRaycastTargetAlignment alignment);

		[Export ("displayTransformForOrientation:viewportSize:")]
		CGAffineTransform GetDisplayTransform (UIInterfaceOrientation orientation, CGSize viewportSize);

		[iOS (14, 0)]
		[NullAllowed, Export ("geoTrackingStatus", ArgumentSemantic.Strong)]
		ARGeoTrackingStatus GeoTrackingStatus { get; }

		[iOS (14, 0)]
		[NullAllowed, Export ("sceneDepth", ArgumentSemantic.Strong)]
		ARDepthData SceneDepth { get; }

		[iOS (14, 0)]
		[NullAllowed]
		[Export ("smoothedSceneDepth", ArgumentSemantic.Strong)]
		ARDepthData SmoothedSceneDepth { get; }

		[iOS (16, 0)]
		[Export ("exifData", ArgumentSemantic.Strong)]
		NSDictionary<NSString, NSObject> ExifData { get; }
	}

	[Deprecated (PlatformName.iOS, 14, 0, message: "Use Raycasting methods over HitTestResult ones.")]
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARHitTestResult {

		[Export ("type")]
		ARHitTestResultType Type { get; }

		[Export ("distance")]
		nfloat Distance { get; }

		[Export ("localTransform")]
		Matrix4 LocalTransform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("worldTransform")]
		Matrix4 WorldTransform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[NullAllowed, Export ("anchor", ArgumentSemantic.Strong)]
		ARAnchor Anchor { get; }
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARLightEstimate {

		[Export ("ambientIntensity")]
		nfloat AmbientIntensity { get; }

		[Export ("ambientColorTemperature")]
		nfloat AmbientColorTemperature { get; }
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARAnchor))]
	[DisableDefaultCtor]
	interface ARPlaneAnchor {
		// Inlined from 'ARAnchorCopying' protocol (we can't have constructors in interfaces)
		[iOS (12, 0)]
		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);

		// [Export ("initWithTransform:")] marked as NS_UNAVAILABLE

		[iOS (12, 0)]
		[Static]
		[Export ("classificationSupported")]
		bool ClassificationSupported { [Bind ("isClassificationSupported")] get; }

		[Export ("alignment")]
		ARPlaneAnchorAlignment Alignment { get; }

		[Export ("center")]
		Vector3 Center {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Deprecated (PlatformName.iOS, 16, 0, message: "Use 'PlaneExtent' instead.")]
		[Export ("extent")]
		Vector3 Extent {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[iOS (11, 3)]
		[Export ("geometry", ArgumentSemantic.Strong)]
		ARPlaneGeometry Geometry { get; }

		[iOS (12, 0)]
		[Export ("classificationStatus", ArgumentSemantic.Assign)]
		ARPlaneClassificationStatus ClassificationStatus { get; }

		[iOS (12, 0)]
		[Export ("classification", ArgumentSemantic.Assign)]
		ARPlaneClassification Classification { get; }

		[iOS (16, 0)]
		[Export ("planeExtent")]
		ARPlaneExtent PlaneExtent { get; }
	}

	[iOS (11, 3)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARPlaneGeometry : NSSecureCoding {
		[Export ("vertexCount")]
		nuint VertexCount { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("vertices")]
		IntPtr GetRawVertices ();

		[Export ("textureCoordinateCount")]
		nuint TextureCoordinateCount { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("textureCoordinates")]
		IntPtr GetRawTextureCoordinates ();

		[Export ("triangleCount")]
		nuint TriangleCount { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("triangleIndices")]
		IntPtr GetRawTriangleIndices ();

		[Export ("boundaryVertexCount")]
		nuint BoundaryVertexCount { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("boundaryVertices")]
		IntPtr GetRawBoundaryVertices ();
	}

	[iOS (11, 3)]
	[BaseType (typeof (SCNGeometry))]
	[DisableDefaultCtor]
	interface ARSCNPlaneGeometry {
		[Static]
		[Export ("planeGeometryWithDevice:")]
		[return: NullAllowed]
		ARSCNPlaneGeometry Create (IMTLDevice device);

		[Export ("updateFromPlaneGeometry:")]
		void Update (ARPlaneGeometry planeGeometry);
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARPointCloud : NSSecureCoding {

		[Export ("count")]
		nuint Count { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Protected, Export ("points")]
		IntPtr GetRawPoints ();

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Protected, Export ("identifiers")]
		IntPtr GetRawIdentifiers ();
	}

	[iOS (11, 3)]
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARReferenceImage : NSCopying {
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		[Export ("physicalSize")]
		CGSize PhysicalSize { get; }

		[iOS (13, 0)]
		[NullAllowed, Export ("resourceGroupName", ArgumentSemantic.Strong)]
		string ResourceGroupName { get; }

		[iOS (13, 0)]
		[Async]
		[Export ("validateWithCompletionHandler:")]
		void Validate (Action<NSError> completionHandler);

		[Export ("initWithCGImage:orientation:physicalWidth:")]
		NativeHandle Constructor (CGImage image, CGImagePropertyOrientation orientation, nfloat physicalWidth);

		[Export ("initWithPixelBuffer:orientation:physicalWidth:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, nfloat physicalWidth);

		[Static]
		[Export ("referenceImagesInGroupNamed:bundle:")]
		[return: NullAllowed]
		NSSet<ARReferenceImage> GetReferenceImagesInGroup (string name, [NullAllowed] NSBundle bundle);
	}

	[iOS (11, 3)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARVideoFormat : NSCopying {

		[iOS (13, 0)]
		[Export ("captureDevicePosition")]
		AVCaptureDevicePosition CaptureDevicePosition { get; }

		[Export ("imageResolution")]
		CGSize ImageResolution { get; }

		[Export ("framesPerSecond")]
		nint FramesPerSecond { get; }

		[iOS (14, 5)]
		[Export ("captureDeviceType")]
#if NET
		[BindAs (typeof (AVCaptureDeviceType))]
#endif
		NSString CaptureDeviceType { get; }

		[iOS (16, 0)]
		[Export ("isRecommendedForHighResolutionFrameCapturing")]
		bool IsRecommendedForHighResolutionFrameCapturing { get; }

		[iOS (16, 0)]
		[Export ("videoHDRSupported")]
		bool IsVideoHdrSupported { [Bind ("isVideoHDRSupported")] get; }
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (SCNView))]
	interface ARSCNView : ARSessionProviding {

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IARSCNViewDelegate Delegate { get; set; }

		// We get the Session property from ARSessionProviding, but only the getter, so we must redefine the property here.
		[Export ("session", ArgumentSemantic.Strong)]
		new ARSession Session { get; set; }

		[Export ("scene", ArgumentSemantic.Strong)]
		SCNScene Scene { get; set; }

		[Export ("automaticallyUpdatesLighting")]
		bool AutomaticallyUpdatesLighting { get; set; }

		[iOS (13, 0)]
		[Export ("rendersCameraGrain")]
		bool RendersCameraGrain { get; set; }

		[iOS (13, 0)]
		[Export ("rendersMotionBlur")]
		bool RendersMotionBlur { get; set; }

		[Export ("anchorForNode:")]
		[return: NullAllowed]
		ARAnchor GetAnchor (SCNNode node);

		[Export ("nodeForAnchor:")]
		[return: NullAllowed]
		SCNNode GetNode (ARAnchor anchor);

		[Export ("hitTest:types:")]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'CreateRaycastQuery' instead.")]
		ARHitTestResult [] HitTest (CGPoint point, ARHitTestResultType types);

		[iOS (12, 0)]
		[Export ("unprojectPoint:ontoPlaneWithTransform:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector3 Unproject (CGPoint point, Matrix4 planeTransform);

		[iOS (13, 0)]
		[Export ("raycastQueryFromPoint:allowingTarget:alignment:")]
		[return: NullAllowed]
		ARRaycastQuery CreateRaycastQuery (CGPoint point, ARRaycastTarget target, ARRaycastTargetAlignment alignment);
	}

	interface IARSCNViewDelegate { }

	[NoWatch, NoTV, NoMac]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface ARSCNViewDelegate : SCNSceneRendererDelegate, ARSessionObserver {

		[Export ("renderer:nodeForAnchor:")]
		[return: NullAllowed]
		SCNNode GetNode (ISCNSceneRenderer renderer, ARAnchor anchor);

		[Export ("renderer:didAddNode:forAnchor:")]
		void DidAddNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor);

		[Export ("renderer:willUpdateNode:forAnchor:")]
		void WillUpdateNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor);

		[Export ("renderer:didUpdateNode:forAnchor:")]
		void DidUpdateNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor);

		[Export ("renderer:didRemoveNode:forAnchor:")]
		void DidRemoveNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor);
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (SKView))]
	interface ARSKView : ARSessionProviding {

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IARSKViewDelegate Delegate { get; set; }

		// We get the Session property from ARSessionProviding, but only the getter, so we must redefine the property here.
		[Export ("session", ArgumentSemantic.Strong)]
		new ARSession Session { get; set; }

		[Export ("anchorForNode:")]
		[return: NullAllowed]
		ARAnchor GetAnchor (SKNode node);

		[Export ("nodeForAnchor:")]
		[return: NullAllowed]
		SKNode GetNode (ARAnchor anchor);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use Raycasting methods instead.")]
		[Export ("hitTest:types:")]
		ARHitTestResult [] HitTest (CGPoint point, ARHitTestResultType types);
	}

	interface IARSKViewDelegate { }

	[NoWatch, NoTV, NoMac]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface ARSKViewDelegate : SKViewDelegate, ARSessionObserver {

		[Export ("view:nodeForAnchor:")]
		[return: NullAllowed]
		SKNode GetNode (ARSKView view, ARAnchor anchor);

		[Export ("view:didAddNode:forAnchor:")]
		void DidAddNode (ARSKView view, SKNode node, ARAnchor anchor);

		[Export ("view:willUpdateNode:forAnchor:")]
		void WillUpdateNode (ARSKView view, SKNode node, ARAnchor anchor);

		[Export ("view:didUpdateNode:forAnchor:")]
		void DidUpdateNode (ARSKView view, SKNode node, ARAnchor anchor);

		[Export ("view:didRemoveNode:forAnchor:")]
		void DidRemoveNode (ARSKView view, SKNode node, ARAnchor anchor);
	}

	delegate void GetGeolocationCallback (CLLocationCoordinate2D coordinate, double altitude, NSError error);

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	interface ARSession {

		[iOS (13, 0)]
		[Export ("identifier", ArgumentSemantic.Strong)]
		NSUuid Identifier { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IARSessionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegateQueue", ArgumentSemantic.Strong)]
		DispatchQueue DelegateQueue { get; set; }

		[NullAllowed, Export ("currentFrame", ArgumentSemantic.Copy)]
		ARFrame CurrentFrame { get; }

		[NullAllowed, Export ("configuration", ArgumentSemantic.Copy)]
		ARConfiguration Configuration { get; }

		[Export ("runWithConfiguration:")]
		void Run (ARConfiguration configuration);

		[Export ("runWithConfiguration:options:")]
		void Run (ARConfiguration configuration, ARSessionRunOptions options);

		[Export ("pause")]
		void Pause ();

		[Export ("addAnchor:")]
		void AddAnchor (ARAnchor anchor);

		[Export ("removeAnchor:")]
		void RemoveAnchor (ARAnchor anchor);

		[iOS (11, 3)]
		[Export ("setWorldOrigin:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		void SetWorldOrigin (Matrix4 relativeTransform);

		[iOS (12, 0)]
		[Async]
		[Export ("getCurrentWorldMapWithCompletionHandler:")]
		void GetCurrentWorldMap (Action<ARWorldMap, NSError> completionHandler);

		[iOS (12, 0)]
		[Async]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[Export ("createReferenceObjectWithTransform:center:extent:completionHandler:")]
		void CreateReferenceObject (Matrix4 transform, Vector3 center, Vector3 extent, Action<ARReferenceObject, NSError> completionHandler);

		[iOS (13, 0)]
		[Export ("raycast:")]
		ARRaycastResult [] Raycast (ARRaycastQuery query);

		[iOS (13, 0)]
		[Async]
		[Export ("trackedRaycast:updateHandler:")]
		[return: NullAllowed]
		ARTrackedRaycast TrackedRaycast (ARRaycastQuery query, Action<ARRaycastResult []> updateHandler);

		[iOS (13, 0)]
		[Export ("updateWithCollaborationData:")]
		void Update (ARCollaborationData collaborationData);

		[iOS (14, 0)]
		[Async (ResultTypeName = "GeoLocationForPoint")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[Export ("getGeoLocationForPoint:completionHandler:")]
		void GetGeoLocation (Vector3 position, GetGeolocationCallback completionHandler);

		[iOS (16, 0)]
		[Async]
		[Export ("captureHighResolutionFrameWithCompletion:")]
		void CaptureHighResolutionFrame (Action<ARFrame, NSError> handler);
	}

	[NoWatch, NoTV, NoMac]
	[Protocol]
	interface ARSessionObserver {

		[Export ("session:didFailWithError:")]
		void DidFail (ARSession session, NSError error);

		[Export ("session:cameraDidChangeTrackingState:")]
		void CameraDidChangeTrackingState (ARSession session, ARCamera camera);

		[Export ("sessionWasInterrupted:")]
		void WasInterrupted (ARSession session);

		[Export ("sessionInterruptionEnded:")]
		void InterruptionEnded (ARSession session);

		[iOS (11, 3)]
		[Export ("sessionShouldAttemptRelocalization:")]
		bool ShouldAttemptRelocalization (ARSession session);

		[Export ("session:didOutputAudioSampleBuffer:")]
		void DidOutputAudioSampleBuffer (ARSession session, CMSampleBuffer audioSampleBuffer);

		[iOS (13, 0)]
		[Export ("session:didOutputCollaborationData:")]
		void DidOutputCollaborationData (ARSession session, ARCollaborationData data);

		[iOS (14, 0)]
		[Export ("session:didChangeGeoTrackingStatus:")]
		void DidChangeGeoTrackingStatus (ARSession session, ARGeoTrackingStatus geoTrackingStatus);
	}

	interface IARSessionDelegate { }

	[NoWatch, NoTV, NoMac]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface ARSessionDelegate : ARSessionObserver {

		[Export ("session:didUpdateFrame:")]
		void DidUpdateFrame (ARSession session, ARFrame frame);

		[Export ("session:didAddAnchors:")]
		void DidAddAnchors (ARSession session, ARAnchor [] anchors);

		[Export ("session:didUpdateAnchors:")]
		void DidUpdateAnchors (ARSession session, ARAnchor [] anchors);

		[Export ("session:didRemoveAnchors:")]
		void DidRemoveAnchors (ARSession session, ARAnchor [] anchors);
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[DisableDefaultCtor]
	interface ARConfiguration : NSCopying {

		[Static]
		[Export ("isSupported")]
		bool IsSupported { get; }

#if !NET
		// even if static - it's abstract
		[iOS (11, 3)]
		[Static]
		[Obsolete ("This is an abstract static method. You need to call 'GetSupportedVideoFormats ()' from a subclass to get results.")]
		ARVideoFormat [] SupportedVideoFormats {
			// avoid the native exception leading to a crash
			[Wrap ("Array.Empty<ARVideoFormat> ()")]
			get;
		}
#endif

		[iOS (11, 3)]
		[Export ("videoFormat", ArgumentSemantic.Strong)]
		ARVideoFormat VideoFormat { get; set; }

		[Export ("worldAlignment", ArgumentSemantic.Assign)]
		ARWorldAlignment WorldAlignment { get; set; }

		[Export ("lightEstimationEnabled")]
		bool LightEstimationEnabled { [Bind ("isLightEstimationEnabled")] get; set; }

		[Export ("providesAudioData")]
		bool ProvidesAudioData { get; set; }

		[iOS (13, 0)]
		[Export ("frameSemantics", ArgumentSemantic.Assign)]
		ARFrameSemantics FrameSemantics { get; set; }

		[iOS (13, 0)]
		[Static]
		[Export ("supportsFrameSemantics:")]
		bool SupportsFrameSemantics (ARFrameSemantics frameSemantics);

		[iOS (16, 0)]
		[Static]
		[NullAllowed, Export ("configurableCaptureDeviceForPrimaryCamera")]
		AVCaptureDevice ConfigurableCaptureDeviceForPrimaryCamera { get; }

		[iOS (16, 0)]
		[Static]
		[NullAllowed, Export ("recommendedVideoFormatFor4KResolution")]
		ARVideoFormat RecommendedVideoFormatFor4KResolution { get; }

		[iOS (16, 0)]
		[Static]
		[NullAllowed, Export ("recommendedVideoFormatForHighResolutionFrameCapturing")]
		ARVideoFormat RecommendedVideoFormatForHighResolutionFrameCapturing { get; }

		[iOS (16, 0)]
		[NullAllowed, Export ("videoHDRAllowed")]
		bool VideoHdrAllowed { get; set; }
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARConfiguration))]
	interface ARWorldTrackingConfiguration {

		[iOS (11, 3)]
		[Static]
		[Export ("supportedVideoFormats")]
		ARVideoFormat [] GetSupportedVideoFormats ();

		[iOS (11, 3)]
		[Export ("autoFocusEnabled")]
		bool AutoFocusEnabled { [Bind ("isAutoFocusEnabled")] get; set; }

		[iOS (12, 0)]
		[Export ("environmentTexturing", ArgumentSemantic.Assign)]
		AREnvironmentTexturing EnvironmentTexturing { get; set; }

		[iOS (13, 0)]
		[Export ("wantsHDREnvironmentTextures")]
		bool WantsHdrEnvironmentTextures { get; set; }

		[Export ("planeDetection", ArgumentSemantic.Assign)]
		ARPlaneDetection PlaneDetection { get; set; }

		[iOS (12, 0)]
		[NullAllowed, Export ("initialWorldMap", ArgumentSemantic.Strong)]
		ARWorldMap InitialWorldMap { get; set; }

		[iOS (11, 3)]
		[NullAllowed] //null_resettable
		[Export ("detectionImages", ArgumentSemantic.Copy)]
		NSSet<ARReferenceImage> DetectionImages { get; set; }

		[iOS (13, 0)]
		[Export ("automaticImageScaleEstimationEnabled")]
		bool AutomaticImageScaleEstimationEnabled { get; set; }

		[iOS (12, 0)]
		[Export ("maximumNumberOfTrackedImages")]
		nint MaximumNumberOfTrackedImages { get; set; }

		[iOS (12, 0)]
		[Export ("detectionObjects", ArgumentSemantic.Copy)]
		NSSet<ARReferenceObject> DetectionObjects { get; set; }

		[iOS (13, 0)]
		[Export ("collaborationEnabled")]
		bool CollaborationEnabled { [Bind ("isCollaborationEnabled")] get; set; }

		[iOS (13, 0)]
		[Static]
		[Export ("supportsUserFaceTracking")]
		bool SupportsUserFaceTracking { get; }

		[iOS (13, 0)]
		[Export ("userFaceTrackingEnabled")]
		bool UserFaceTrackingEnabled { [Bind ("userFaceTrackingEnabled")] get; set; }

		[iOS (14, 3)]
		[Export ("appClipCodeTrackingEnabled")]
		bool AppClipCodeTrackingEnabled { get; set; }

		[iOS (14, 3)]
		[Static]
		[Export ("supportsAppClipCodeTracking")]
		bool SupportsAppClipCodeTracking { get; }

		[iOS (13, 4)]
		[Static]
		[Export ("supportsSceneReconstruction:")]
		bool SupportsSceneReconstruction (ARSceneReconstruction sceneReconstruction);

		[iOS (13, 4)]
		[Export ("sceneReconstruction", ArgumentSemantic.Assign)]
		ARSceneReconstruction SceneReconstruction { get; set; }

		[iOS (13, 0)]
		[Static]
		[Export ("supportsFrameSemantics:")]
		bool SupportsFrameSemantics (ARFrameSemantics frameSemantics);
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARConfiguration))]
	interface AROrientationTrackingConfiguration {

		[iOS (11, 3)]
		[Static]
		[Export ("supportedVideoFormats")]
		ARVideoFormat [] GetSupportedVideoFormats ();

		[iOS (11, 3)]
		[Export ("autoFocusEnabled")]
		bool AutoFocusEnabled { [Bind ("isAutoFocusEnabled")] get; set; }

		[iOS (13, 0)]
		[Static]
		[Export ("supportsFrameSemantics:")]
		bool SupportsFrameSemantics (ARFrameSemantics frameSemantics);
	}

	[NoWatch, NoTV, NoMac]
	[Static]
	interface ARSCNDebugOptions {

		[Field ("ARSCNDebugOptionShowWorldOrigin")]
		SCNDebugOptions ShowWorldOrigin { get; }

		[Field ("ARSCNDebugOptionShowFeaturePoints")]
		SCNDebugOptions ShowFeaturePoints { get; }
	}

	[NoWatch, NoTV, NoMac]
	[Protocol]
	interface ARTrackable {
		[Abstract]
		[Export ("isTracked")]
		bool IsTracked { get; }
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARConfiguration))]
	interface ARFaceTrackingConfiguration {
		[iOS (11, 3)]
		[Static]
		[Export ("supportedVideoFormats")]
		ARVideoFormat [] GetSupportedVideoFormats ();

		[iOS (13, 0)]
		[Static]
		[Export ("supportedNumberOfTrackedFaces")]
		nint SupportedNumberOfTrackedFaces { get; }

		[iOS (13, 0)]
		[Export ("maximumNumberOfTrackedFaces")]
		nint MaximumNumberOfTrackedFaces { get; set; }

		[iOS (13, 0)]
		[Static]
		[Export ("supportsWorldTracking")]
		bool SupportsWorldTracking { get; }

		[iOS (13, 0)]
		[Export ("worldTrackingEnabled")]
		bool WorldTrackingEnabled { [Bind ("isWorldTrackingEnabled")] get; set; }

		[iOS (13, 0)]
		[Static]
		[Export ("supportsFrameSemantics:")]
		bool SupportsFrameSemantics (ARFrameSemantics frameSemantics);
	}

	[NoWatch, NoTV, NoMac]
	[StrongDictionary ("ARBlendShapeLocationKeys")]
	interface ARBlendShapeLocationOptions {

		float BrowDownLeft { get; set; }

		float BrowDownRight { get; set; }

		float BrowInnerUp { get; set; }

		float BrowOuterUpLeft { get; set; }

		float BrowOuterUpRight { get; set; }

		float CheekPuff { get; set; }

		float CheekSquintLeft { get; set; }

		float CheekSquintRight { get; set; }

		float EyeBlinkLeft { get; set; }

		float EyeBlinkRight { get; set; }

		float EyeLookDownLeft { get; set; }

		float EyeLookDownRight { get; set; }

		float EyeLookInLeft { get; set; }

		float EyeLookInRight { get; set; }

		float EyeLookOutLeft { get; set; }

		float EyeLookOutRight { get; set; }

		float EyeLookUpLeft { get; set; }

		float EyeLookUpRight { get; set; }

		float EyeSquintLeft { get; set; }

		float EyeSquintRight { get; set; }

		float EyeWideLeft { get; set; }

		float EyeWideRight { get; set; }

		float JawForward { get; set; }

		float JawLeft { get; set; }

		float JawOpen { get; set; }

		float JawRight { get; set; }

		float MouthClose { get; set; }

		float MouthDimpleLeft { get; set; }

		float MouthDimpleRight { get; set; }

		float MouthFrownLeft { get; set; }

		float MouthFrownRight { get; set; }

		float MouthFunnel { get; set; }

		float MouthLeft { get; set; }

		float MouthLowerDownLeft { get; set; }

		float MouthLowerDownRight { get; set; }

		float MouthPressLeft { get; set; }

		float MouthPressRight { get; set; }

		float MouthPucker { get; set; }

		float MouthRight { get; set; }

		float MouthRollLower { get; set; }

		float MouthRollUpper { get; set; }

		float MouthShrugLower { get; set; }

		float MouthShrugUpper { get; set; }

		float MouthSmileLeft { get; set; }

		float MouthSmileRight { get; set; }

		float MouthStretchLeft { get; set; }

		float MouthStretchRight { get; set; }

		float MouthUpperUpLeft { get; set; }

		float MouthUpperUpRight { get; set; }

		float NoseSneerLeft { get; set; }

		float NoseSneerRight { get; set; }

		[iOS (12, 0)]
		float TongueOut { get; set; }
	}

	[NoWatch, NoTV, NoMac]
	[Static]
	[Internal]
	interface ARBlendShapeLocationKeys {

		[Field ("ARBlendShapeLocationBrowDownLeft")]
		NSString BrowDownLeftKey { get; }

		[Field ("ARBlendShapeLocationBrowDownRight")]
		NSString BrowDownRightKey { get; }

		[Field ("ARBlendShapeLocationBrowInnerUp")]
		NSString BrowInnerUpKey { get; }

		[Field ("ARBlendShapeLocationBrowOuterUpLeft")]
		NSString BrowOuterUpLeftKey { get; }

		[Field ("ARBlendShapeLocationBrowOuterUpRight")]
		NSString BrowOuterUpRightKey { get; }

		[Field ("ARBlendShapeLocationCheekPuff")]
		NSString CheekPuffKey { get; }

		[Field ("ARBlendShapeLocationCheekSquintLeft")]
		NSString CheekSquintLeftKey { get; }

		[Field ("ARBlendShapeLocationCheekSquintRight")]
		NSString CheekSquintRightKey { get; }

		[Field ("ARBlendShapeLocationEyeBlinkLeft")]
		NSString EyeBlinkLeftKey { get; }

		[Field ("ARBlendShapeLocationEyeBlinkRight")]
		NSString EyeBlinkRightKey { get; }

		[Field ("ARBlendShapeLocationEyeLookDownLeft")]
		NSString EyeLookDownLeftKey { get; }

		[Field ("ARBlendShapeLocationEyeLookDownRight")]
		NSString EyeLookDownRightKey { get; }

		[Field ("ARBlendShapeLocationEyeLookInLeft")]
		NSString EyeLookInLeftKey { get; }

		[Field ("ARBlendShapeLocationEyeLookInRight")]
		NSString EyeLookInRightKey { get; }

		[Field ("ARBlendShapeLocationEyeLookOutLeft")]
		NSString EyeLookOutLeftKey { get; }

		[Field ("ARBlendShapeLocationEyeLookOutRight")]
		NSString EyeLookOutRightKey { get; }

		[Field ("ARBlendShapeLocationEyeLookUpLeft")]
		NSString EyeLookUpLeftKey { get; }

		[Field ("ARBlendShapeLocationEyeLookUpRight")]
		NSString EyeLookUpRightKey { get; }

		[Field ("ARBlendShapeLocationEyeSquintLeft")]
		NSString EyeSquintLeftKey { get; }

		[Field ("ARBlendShapeLocationEyeSquintRight")]
		NSString EyeSquintRightKey { get; }

		[Field ("ARBlendShapeLocationEyeWideLeft")]
		NSString EyeWideLeftKey { get; }

		[Field ("ARBlendShapeLocationEyeWideRight")]
		NSString EyeWideRightKey { get; }

		[Field ("ARBlendShapeLocationJawForward")]
		NSString JawForwardKey { get; }

		[Field ("ARBlendShapeLocationJawLeft")]
		NSString JawLeftKey { get; }

		[Field ("ARBlendShapeLocationJawOpen")]
		NSString JawOpenKey { get; }

		[Field ("ARBlendShapeLocationJawRight")]
		NSString JawRightKey { get; }

		[Field ("ARBlendShapeLocationMouthClose")]
		NSString MouthCloseKey { get; }

		[Field ("ARBlendShapeLocationMouthDimpleLeft")]
		NSString MouthDimpleLeftKey { get; }

		[Field ("ARBlendShapeLocationMouthDimpleRight")]
		NSString MouthDimpleRightKey { get; }

		[Field ("ARBlendShapeLocationMouthFrownLeft")]
		NSString MouthFrownLeftKey { get; }

		[Field ("ARBlendShapeLocationMouthFrownRight")]
		NSString MouthFrownRightKey { get; }

		[Field ("ARBlendShapeLocationMouthFunnel")]
		NSString MouthFunnelKey { get; }

		[Field ("ARBlendShapeLocationMouthLeft")]
		NSString MouthLeftKey { get; }

		[Field ("ARBlendShapeLocationMouthLowerDownLeft")]
		NSString MouthLowerDownLeftKey { get; }

		[Field ("ARBlendShapeLocationMouthLowerDownRight")]
		NSString MouthLowerDownRightKey { get; }

		[Field ("ARBlendShapeLocationMouthPressLeft")]
		NSString MouthPressLeftKey { get; }

		[Field ("ARBlendShapeLocationMouthPressRight")]
		NSString MouthPressRightKey { get; }

		[Field ("ARBlendShapeLocationMouthPucker")]
		NSString MouthPuckerKey { get; }

		[Field ("ARBlendShapeLocationMouthRight")]
		NSString MouthRightKey { get; }

		[Field ("ARBlendShapeLocationMouthRollLower")]
		NSString MouthRollLowerKey { get; }

		[Field ("ARBlendShapeLocationMouthRollUpper")]
		NSString MouthRollUpperKey { get; }

		[Field ("ARBlendShapeLocationMouthShrugLower")]
		NSString MouthShrugLowerKey { get; }

		[Field ("ARBlendShapeLocationMouthShrugUpper")]
		NSString MouthShrugUpperKey { get; }

		[Field ("ARBlendShapeLocationMouthSmileLeft")]
		NSString MouthSmileLeftKey { get; }

		[Field ("ARBlendShapeLocationMouthSmileRight")]
		NSString MouthSmileRightKey { get; }

		[Field ("ARBlendShapeLocationMouthStretchLeft")]
		NSString MouthStretchLeftKey { get; }

		[Field ("ARBlendShapeLocationMouthStretchRight")]
		NSString MouthStretchRightKey { get; }

		[Field ("ARBlendShapeLocationMouthUpperUpLeft")]
		NSString MouthUpperUpLeftKey { get; }

		[Field ("ARBlendShapeLocationMouthUpperUpRight")]
		NSString MouthUpperUpRightKey { get; }

		[Field ("ARBlendShapeLocationNoseSneerLeft")]
		NSString NoseSneerLeftKey { get; }

		[Field ("ARBlendShapeLocationNoseSneerRight")]
		NSString NoseSneerRightKey { get; }

		[iOS (12, 0)]
		[Field ("ARBlendShapeLocationTongueOut")]
		NSString TongueOutKey { get; }
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARAnchor))]
	[DisableDefaultCtor]
	interface ARFaceAnchor : ARTrackable {
		// Inlined from 'ARAnchorCopying' protocol (we can't have constructors in interfaces)
		[iOS (12, 0)]
		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);

#if !NET
		[Obsolete ("Constructor marked as unavailable.")]
		[Export ("init")]
		NativeHandle Constructor ();
#endif

		[Export ("geometry")]
		ARFaceGeometry Geometry { get; }

		[iOS (12, 0)]
		[Export ("leftEyeTransform")]
		Matrix4 LeftEyeTransform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[iOS (12, 0)]
		[Export ("rightEyeTransform")]
		Matrix4 RightEyeTransform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[iOS (12, 0)]
		[Export ("lookAtPoint")]
		Vector3 LookAtPoint {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("blendShapes")]
		NSDictionary WeakBlendShapes { get; }

		[Wrap ("WeakBlendShapes")]
		ARBlendShapeLocationOptions BlendShapes { get; }
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARFaceGeometry : NSCopying, NSSecureCoding {
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithBlendShapes:")]
		NativeHandle Constructor (NSDictionary blendShapes);

		[Wrap ("this (blendShapes.GetDictionary ()!)")]
		NativeHandle Constructor (ARBlendShapeLocationOptions blendShapes);

		[Export ("vertexCount")]
		nuint VertexCount { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("vertices")]
		IntPtr GetRawVertices ();

		[Export ("textureCoordinateCount")]
		nuint TextureCoordinateCount { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("textureCoordinates")]
		IntPtr GetRawTextureCoordinates ();

		[Export ("triangleCount")]
		nuint TriangleCount { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("triangleIndices")]
		IntPtr GetRawTriangleIndices ();
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (SCNGeometry))]
	[DisableDefaultCtor]
	interface ARSCNFaceGeometry {
#if !NET
		[Obsolete ("Use the 'Create' static constructor instead.")]
		[Static]
		[Wrap ("Create (device)")]
		[return: NullAllowed]
		ARSCNFaceGeometry CreateFaceGeometry (IMTLDevice device);
#endif

		[Static]
		[Export ("faceGeometryWithDevice:")]
		[return: NullAllowed]
		ARSCNFaceGeometry Create (IMTLDevice device);

#if !NET
		[Obsolete ("Use the 'Create' static constructor instead.")]
		[Static]
		[Wrap ("Create (device, fillMesh)")]
		[return: NullAllowed]
		ARSCNFaceGeometry CreateFaceGeometry (IMTLDevice device, bool fillMesh);
#endif

		[Static]
		[Export ("faceGeometryWithDevice:fillMesh:")]
		[return: NullAllowed]
		ARSCNFaceGeometry Create (IMTLDevice device, bool fillMesh);

		[Export ("updateFromFaceGeometry:")]
		void Update (ARFaceGeometry faceGeometry);
	}

	[iOS (11, 3)]
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARAnchor))]
	[DisableDefaultCtor]
	interface ARImageAnchor : ARTrackable {
		// Inlined from 'ARAnchorCopying' protocol (we can't have constructors in interfaces)
		[iOS (12, 0)]
		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);

		[Export ("referenceImage", ArgumentSemantic.Strong)]
		ARReferenceImage ReferenceImage { get; }

		[iOS (13, 0)]
		[Export ("estimatedScaleFactor")]
		nfloat EstimatedScaleFactor { get; }
	}

	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARLightEstimate))]
	[DisableDefaultCtor]
	interface ARDirectionalLightEstimate {
		[Export ("sphericalHarmonicsCoefficients", ArgumentSemantic.Copy)]
		NSData SphericalHarmonicsCoefficients { get; }

		[Export ("primaryLightDirection")]
		Vector3 PrimaryLightDirection {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("primaryLightIntensity")]
		nfloat PrimaryLightIntensity { get; }
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARConfiguration))]
	interface ARImageTrackingConfiguration {
		[Static]
		[Export ("supportedVideoFormats")]
		ARVideoFormat [] GetSupportedVideoFormats ();

		[Export ("autoFocusEnabled")]
		bool AutoFocusEnabled { [Bind ("isAutoFocusEnabled")] get; set; }

		[Export ("trackingImages", ArgumentSemantic.Copy)]
		NSSet<ARReferenceImage> TrackingImages { get; set; }

		[Export ("maximumNumberOfTrackedImages")]
		nint MaximumNumberOfTrackedImages { get; set; }

		[iOS (13, 0)]
		[Static]
		[Export ("supportsFrameSemantics:")]
		bool SupportsFrameSemantics (ARFrameSemantics frameSemantics);
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARConfiguration))]
	interface ARObjectScanningConfiguration {
		[Static]
		[Export ("supportedVideoFormats")]
		ARVideoFormat [] GetSupportedVideoFormats ();

		[Export ("autoFocusEnabled")]
		bool AutoFocusEnabled { [Bind ("isAutoFocusEnabled")] get; set; }

		[Export ("planeDetection", ArgumentSemantic.Assign)]
		ARPlaneDetection PlaneDetection { get; set; }

		[iOS (13, 0)]
		[Static]
		[Export ("supportsFrameSemantics:")]
		bool SupportsFrameSemantics (ARFrameSemantics frameSemantics);
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARAnchor))]
	[DisableDefaultCtor]
	interface AREnvironmentProbeAnchor {
		// Inlined from 'ARAnchorCopying' protocol (we can't have constructors in interfaces)
		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);

		[Export ("initWithTransform:extent:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (Matrix4 transform, Vector3 extent);

		[Export ("initWithName:transform:extent:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (string name, Matrix4 transform, Vector3 extent);

		[NullAllowed, Export ("environmentTexture", ArgumentSemantic.Strong)]
		IMTLTexture EnvironmentTexture { get; }

		[Export ("extent")]
		Vector3 Extent {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARReferenceObject : NSSecureCoding {
		[Export ("initWithArchiveURL:error:")]
		NativeHandle Constructor (NSUrl archiveUrl, [NullAllowed] out NSError error);

		[NullAllowed, Export ("name")]
		string Name { get; set; }

		[Export ("center")]
		Vector3 Center {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("extent")]
		Vector3 Extent {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("scale")]
		Vector3 Scale {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[iOS (13, 0)]
		[NullAllowed, Export ("resourceGroupName", ArgumentSemantic.Strong)]
		string ResourceGroupName { get; }

		[Export ("rawFeaturePoints", ArgumentSemantic.Strong)]
		ARPointCloud RawFeaturePoints { get; }

		[Static]
		[Export ("referenceObjectsInGroupNamed:bundle:")]
		[return: NullAllowed]
		NSSet<ARReferenceObject> GetReferenceObjects (string resourceGroupName, [NullAllowed] NSBundle bundle);

		[Export ("exportObjectToURL:previewImage:error:")]
		bool Export (NSUrl url, [NullAllowed] UIImage previewImage, [NullAllowed] out NSError error);

		[Export ("referenceObjectByApplyingTransform:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		ARReferenceObject ApplyTransform (Matrix4 transform);

		[Export ("referenceObjectByMergingObject:error:")]
		[return: NullAllowed]
		ARReferenceObject Merge (ARReferenceObject @object, [NullAllowed] out NSError error);

		[Field ("ARReferenceObjectArchiveExtension")]
		NSString ArchiveExtension { get; }
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (ARAnchor))]
	[DisableDefaultCtor]
	interface ARObjectAnchor {
		// Inlined from 'ARAnchorCopying' protocol (we can't have constructors in interfaces)
		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);

		[Export ("referenceObject", ArgumentSemantic.Strong)]
		ARReferenceObject ReferenceObject { get; }
	}

	[iOS (12, 0)]
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARWorldMap : NSCopying, NSSecureCoding {
		[Export ("center")]
		Vector3 Center {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("extent")]
		Vector3 Extent {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("anchors", ArgumentSemantic.Copy)]
		ARAnchor [] Anchors { get; set; }

		[Export ("rawFeaturePoints", ArgumentSemantic.Strong)]
		ARPointCloud RawFeaturePoints { get; }
	}

	[iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARBody2D {

		[Export ("skeleton")]
		ARSkeleton2D Skeleton { get; }
	}

	[iOS (13, 0)]
	[BaseType (typeof (ARAnchor))]
	[DisableDefaultCtor]
	interface ARBodyAnchor : ARTrackable {

		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);

		// [Export ("initWithTransform:")] marked as NS_UNAVAILABLE
		// [Export ("initWithName:")] marked as NS_UNAVAILABLE

		[Export ("skeleton", ArgumentSemantic.Strong)]
		ARSkeleton3D Skeleton { get; }

		[Export ("estimatedScaleFactor")]
		nfloat EstimatedScaleFactor { get; }
	}

	[iOS (13, 0)]
	[BaseType (typeof (UIView))]
	interface ARCoachingOverlayView {

		// inherited from UIView
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IARCoachingOverlayViewDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("sessionProvider", ArgumentSemantic.Weak)]
		IARSessionProviding SessionProvider { get; set; }

		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		ARSession Session { get; set; }

		[Export ("goal", ArgumentSemantic.Assign)]
		ARCoachingGoal Goal { get; set; }

		[Export ("activatesAutomatically")]
		bool ActivatesAutomatically { get; set; }

		[Export ("isActive")]
		bool IsActive { get; }

		[Export ("setActive:animated:")]
		void SetActive (bool active, bool animated);
	}

	interface IARCoachingOverlayViewDelegate { }

	[iOS (13, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface ARCoachingOverlayViewDelegate {

		[Export ("coachingOverlayViewDidRequestSessionReset:")]
		void DidRequestSessionReset (ARCoachingOverlayView coachingOverlayView);

		[Export ("coachingOverlayViewWillActivate:")]
		void WillActivate (ARCoachingOverlayView coachingOverlayView);

		[Export ("coachingOverlayViewDidDeactivate:")]
		void DidDeactivate (ARCoachingOverlayView coachingOverlayView);
	}

	[iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARCollaborationData : NSSecureCoding {

		[Export ("priority")]
		ARCollaborationDataPriority Priority { get; }
	}

	[iOS (13, 0)]
	[BaseType (typeof (ARConfiguration))]
	interface ARBodyTrackingConfiguration {

		// From the parent, needed in all subclasses
		[Static]
		[Export ("supportedVideoFormats")]
		ARVideoFormat [] GetSupportedVideoFormats ();

		[Export ("autoFocusEnabled")]
		bool AutoFocusEnabled { [Bind ("isAutoFocusEnabled")] get; set; }

		[NullAllowed, Export ("initialWorldMap", ArgumentSemantic.Strong)]
		ARWorldMap InitialWorldMap { get; set; }

		[Export ("environmentTexturing", ArgumentSemantic.Assign)]
		AREnvironmentTexturing EnvironmentTexturing { get; set; }

		[Export ("wantsHDREnvironmentTextures")]
		bool WantsHdrEnvironmentTextures { get; set; }

		[Export ("planeDetection", ArgumentSemantic.Assign)]
		ARPlaneDetection PlaneDetection { get; set; }

		[Export ("detectionImages", ArgumentSemantic.Copy)]
		NSSet<ARReferenceImage> DetectionImages { get; set; }

		[Export ("automaticImageScaleEstimationEnabled")]
		bool AutomaticImageScaleEstimationEnabled { get; set; }

		[Export ("automaticSkeletonScaleEstimationEnabled")]
		bool AutomaticSkeletonScaleEstimationEnabled { get; set; }

		[Export ("maximumNumberOfTrackedImages")]
		nint MaximumNumberOfTrackedImages { get; set; }

		[Static]
		[Export ("supportsFrameSemantics:")]
		bool SupportsFrameSemantics (ARFrameSemantics frameSemantics);

		[iOS (14, 3)]
		[Export ("appClipCodeTrackingEnabled")]
		bool AppClipCodeTrackingEnabled { get; set; }

		[iOS (14, 3)]
		[Static]
		[Export ("supportsAppClipCodeTracking")]
		bool SupportsAppClipCodeTracking { get; }
	}

	[iOS (13, 0)]
	[BaseType (typeof (ARConfiguration))]
	interface ARPositionalTrackingConfiguration {

		// From the parent, needed in all subclasses
		[Static]
		[Export ("supportedVideoFormats")]
		ARVideoFormat [] GetSupportedVideoFormats ();

		[Export ("planeDetection", ArgumentSemantic.Assign)]
		ARPlaneDetection PlaneDetection { get; set; }

		[NullAllowed, Export ("initialWorldMap", ArgumentSemantic.Strong)]
		ARWorldMap InitialWorldMap { get; set; }

		[Static]
		[Export ("supportsFrameSemantics:")]
		bool SupportsFrameSemantics (ARFrameSemantics frameSemantics);
	}

	[iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARMatteGenerator {

		[DesignatedInitializer]
		[Export ("initWithDevice:matteResolution:")]
		NativeHandle Constructor (IMTLDevice device, ARMatteResolution matteResolution);

		[Export ("generateMatteFromFrame:commandBuffer:")]
		IMTLTexture GenerateMatte (ARFrame frame, IMTLCommandBuffer commandBuffer);

		[Export ("generateDilatedDepthFromFrame:commandBuffer:")]
		IMTLTexture GenerateDilatedDepth (ARFrame frame, IMTLCommandBuffer commandBuffer);
	}

	[iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARQuickLookPreviewItem : QLPreviewItem {

		[Export ("initWithFileAtURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl url);

		[NullAllowed, Export ("canonicalWebPageURL", ArgumentSemantic.Strong)]
		NSUrl CanonicalWebPageUrl { get; set; }

		[Export ("allowsContentScaling")]
		bool AllowsContentScaling { get; set; }
	}

	[iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARRaycastQuery {

		[Export ("initWithOrigin:direction:allowingTarget:alignment:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (Vector3 origin, Vector3 direction, ARRaycastTarget target, ARRaycastTargetAlignment alignment);

		[Export ("origin")]
		Vector3 Origin {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("direction")]
		Vector3 Direction {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("target")]
		ARRaycastTarget Target { get; }

		[Export ("targetAlignment")]
		ARRaycastTargetAlignment TargetAlignment { get; }
	}

	[iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARRaycastResult {

		[Export ("worldTransform")]
		Matrix4 WorldTransform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("target", ArgumentSemantic.Assign)]
		ARRaycastTarget Target { get; }

		[Export ("targetAlignment", ArgumentSemantic.Assign)]
		ARRaycastTargetAlignment TargetAlignment { get; }

		[NullAllowed, Export ("anchor", ArgumentSemantic.Strong)]
		ARAnchor Anchor { get; }
	}

	interface IARSessionProviding { }

	[iOS (13, 0)]
	[Protocol]
	interface ARSessionProviding {

		[Abstract]
		[Export ("session")]
		ARSession Session { get; }
	}

	[iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARSkeleton {

		[Export ("definition")]
		ARSkeletonDefinition Definition { get; }

		[Export ("jointCount")]
		nuint JointCount { get; }

		[Export ("isJointTracked:")]
		bool IsJointTracked (nint jointIndex);
	}

	[iOS (13, 0)]
	[BaseType (typeof (ARSkeleton))]
	[DisableDefaultCtor]
	interface ARSkeleton3D {

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Protected, Export ("jointModelTransforms")]
		IntPtr RawJointModelTransforms { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Protected, Export ("jointLocalTransforms")]
		IntPtr RawJointLocalTransforms { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("modelTransformForJointName:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Matrix4 GetModelTransform (NSString jointName);

		[Wrap ("GetModelTransform (jointName.GetConstant()!)", IsVirtual = true)]
		Matrix4 GetModelTransform (ARSkeletonJointName jointName);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("localTransformForJointName:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Matrix4 GetLocalTransform (NSString jointName);

		[Wrap ("GetLocalTransform (jointName.GetConstant()!)", IsVirtual = true)]
		Matrix4 GetLocalTransform (ARSkeletonJointName jointName);
	}

	[iOS (13, 0)]
	[BaseType (typeof (ARSkeleton))]
	[DisableDefaultCtor]
	interface ARSkeleton2D {

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Protected, Export ("jointLandmarks")]
		IntPtr RawJointLandmarks { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("landmarkForJointNamed:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector2 GetLandmarkPoint (NSString jointName);

		[Wrap ("GetLandmarkPoint (jointName.GetConstant()!)", IsVirtual = true)]
		Vector2 GetLandmarkPoint (ARSkeletonJointName jointName);
	}

	[iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARSkeletonDefinition {

		[Static]
		[Export ("defaultBody3DSkeletonDefinition")]
		ARSkeletonDefinition DefaultBody3DSkeletonDefinition { get; }

		[Static]
		[Export ("defaultBody2DSkeletonDefinition")]
		ARSkeletonDefinition DefaultBody2DSkeletonDefinition { get; }

		[Export ("jointCount")]
		nuint JointCount { get; }

		[Export ("jointNames")]
		string [] JointNames { get; }

		[Export ("parentIndices")]
		NSNumber [] ParentIndices { get; }

		[NullAllowed, Export ("neutralBodySkeleton3D")]
		ARSkeleton3D NeutralBodySkeleton3D { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("indexForJointName:")]
		nuint GetJointIndex (NSString jointName);

		[Wrap ("GetJointIndex (jointName.GetConstant()!)")]
		nuint GetJointIndex (ARSkeletonJointName jointName);
	}

	[iOS (13, 0)]
	enum ARSkeletonJointName {

		[Field ("ARSkeletonJointNameRoot")]
		Root,

		[Field ("ARSkeletonJointNameHead")]
		Head,

		[Field ("ARSkeletonJointNameLeftHand")]
		LeftHand,

		[Field ("ARSkeletonJointNameRightHand")]
		RightHand,

		[Field ("ARSkeletonJointNameLeftFoot")]
		LeftFoot,

		[Field ("ARSkeletonJointNameRightFoot")]
		RightFoot,

		[Field ("ARSkeletonJointNameLeftShoulder")]
		LeftShoulder,

		[Field ("ARSkeletonJointNameRightShoulder")]
		RightShoulder,
	}

	[iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARTrackedRaycast {

		[Export ("stopTracking")]
		void StopTracking ();
	}

	[iOS (13, 0)]
	[BaseType (typeof (ARAnchor))]
	[DisableDefaultCtor]
	interface ARParticipantAnchor {

		// Inlined from 'ARAnchorCopying' protocol (we can't have constructors in interfaces)
		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);

		// [Export ("initWithTransform:")] marked as NS_UNAVAILABLE
		// [Export ("initWithName:")] marked as NS_UNAVAILABLE
	}

	[iOS (13, 4)]
	[Native]
	[Flags]
	enum ARSceneReconstruction : ulong {
		None = 0,
		Mesh = 1,
		MeshWithClassification = (1 << 1) | (1 << 0),
	}

	[iOS (13, 4)]
	[BaseType (typeof (ARAnchor))]
	[DisableDefaultCtor]
	interface ARMeshAnchor {

		// Inlined from 'ARAnchorCopying' protocol (we can't have constructors in interfaces)
		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);

		// [Export ("initWithTransform:")] marked as NS_UNAVAILABLE
		// [Export ("initWithName:")] marked as NS_UNAVAILABLE

		[Export ("geometry")]
		ARMeshGeometry Geometry { get; }
	}

	[iOS (13, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARGeometrySource : NSSecureCoding {

		[Export ("buffer", ArgumentSemantic.Strong)]
		IMTLBuffer Buffer { get; }

		[Export ("count")]
		nint Count { get; }

		[Export ("format", ArgumentSemantic.Assign)]
		MTLVertexFormat Format { get; }

		[Export ("componentsPerVector")]
		nint ComponentsPerVector { get; }

		[Export ("offset")]
		nint Offset { get; }

		[Export ("stride")]
		nint Stride { get; }
	}

	[iOS (13, 4)]
	[Native]
	enum ARGeometryPrimitiveType : long {
		Line,
		Triangle,
	}

	[iOS (13, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARGeometryElement : NSSecureCoding {

		[Export ("buffer", ArgumentSemantic.Strong)]
		IMTLBuffer Buffer { get; }

		[Export ("count")]
		nint Count { get; }

		[Export ("bytesPerIndex")]
		nint BytesPerIndex { get; }

		[Export ("indexCountPerPrimitive")]
		nint IndexCountPerPrimitive { get; }

		[Export ("primitiveType", ArgumentSemantic.Assign)]
		ARGeometryPrimitiveType PrimitiveType { get; }
	}

	[iOS (13, 4)]
	[Native]
	enum ARMeshClassification : long {
		None,
		Wall,
		Floor,
		Ceiling,
		Table,
		Seat,
		Window,
		Door,
	}

	[iOS (13, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARMeshGeometry : NSSecureCoding {

		[Export ("vertices", ArgumentSemantic.Strong)]
		ARGeometrySource Vertices { get; }

		[Export ("normals", ArgumentSemantic.Strong)]
		ARGeometrySource Normals { get; }

		[Export ("faces", ArgumentSemantic.Strong)]
		ARGeometryElement Faces { get; }

		[Export ("classification", ArgumentSemantic.Strong)]
		[NullAllowed]
		ARGeometrySource Classification { get; }
	}

	[iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARDepthData {
		[Export ("depthMap", ArgumentSemantic.Assign)]
		CVPixelBuffer DepthMap { get; }

		[NullAllowed, Export ("confidenceMap", ArgumentSemantic.Assign)]
		CVPixelBuffer ConfidenceMap { get; }
	}

	[iOS (14, 0)]
	[BaseType (typeof (ARAnchor))]
	interface ARGeoAnchor : ARTrackable {
		// Inlined from 'ARAnchorCopying' protocol (we can't have constructors in interfaces)
		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);

		[Export ("coordinate")]
		CLLocationCoordinate2D Coordinate { get; }

		[Export ("altitude")]
		double Altitude { get; }

		[Export ("altitudeSource", ArgumentSemantic.Assign)]
		ARAltitudeSource AltitudeSource { get; }

		[Export ("initWithCoordinate:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate);

		[Export ("initWithCoordinate:altitude:")]
		NativeHandle Constructor (CLLocationCoordinate2D coordinate, double altitude);

		[Export ("initWithName:coordinate:")]
		NativeHandle Constructor (string name, CLLocationCoordinate2D coordinate);

		[Export ("initWithName:coordinate:altitude:")]
		NativeHandle Constructor (string name, CLLocationCoordinate2D coordinate, double altitude);
	}

	[iOS (14, 0)]
	[BaseType (typeof (ARConfiguration))]
	interface ARGeoTrackingConfiguration {
		[Static]
		[Export ("supportedVideoFormats")]
		ARVideoFormat [] GetSupportedVideoFormats ();

		[Export ("environmentTexturing", ArgumentSemantic.Assign)]
		AREnvironmentTexturing EnvironmentTexturing { get; set; }

		[Export ("wantsHDREnvironmentTextures")]
		bool WantsHdrEnvironmentTextures { get; set; }

		[Export ("planeDetection", ArgumentSemantic.Assign)]
		ARPlaneDetection PlaneDetection { get; set; }

		[NullAllowed, Export ("detectionImages", ArgumentSemantic.Copy)]
		NSSet<ARReferenceImage> DetectionImages { get; set; }

		[Export ("automaticImageScaleEstimationEnabled")]
		bool AutomaticImageScaleEstimationEnabled { get; set; }

		[Export ("maximumNumberOfTrackedImages")]
		nint MaximumNumberOfTrackedImages { get; set; }

		[iOS (14, 3)]
		[Export ("appClipCodeTrackingEnabled")]
		bool AppClipCodeTrackingEnabled { get; set; }

		[iOS (14, 3)]
		[Static]
		[Export ("supportsAppClipCodeTracking")]
		bool SupportsAppClipCodeTracking { get; }

		[Export ("detectionObjects", ArgumentSemantic.Copy)]
		NSSet<ARReferenceObject> DetectionObjects { get; set; }

		[Async]
		[Static]
		[Export ("checkAvailabilityWithCompletionHandler:")]
		void CheckAvailability (Action<bool, NSError> completionHandler);

		[Async]
		[Static]
		[Export ("checkAvailabilityAtCoordinate:completionHandler:")]
		void CheckAvailability (CLLocationCoordinate2D coordinate, Action<bool, NSError> completionHandler);

		[Static]
		[Export ("new")]
		[return: Release]
		ARGeoTrackingConfiguration Create ();

		[Static]
		[Export ("supportsFrameSemantics:")]
		bool SupportsFrameSemantics (ARFrameSemantics frameSemantics);
	}

	[iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ARGeoTrackingStatus : NSCopying, NSSecureCoding {
		[Export ("state")]
		ARGeoTrackingState State { get; }

		[Export ("accuracy")]
		ARGeoTrackingAccuracy Accuracy { get; }

		[Export ("stateReason")]
		ARGeoTrackingStateReason StateReason { get; }
	}

	[iOS (14, 3)]
	[BaseType (typeof (ARAnchor))]
	[DisableDefaultCtor]
	interface ARAppClipCodeAnchor : ARTrackable {

		// Inlined from 'ARAnchorCopying' protocol (we can't have constructors in interfaces)
		[Export ("initWithAnchor:")]
		NativeHandle Constructor (ARAnchor anchor);

		[NullAllowed, Export ("url", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("urlDecodingState", ArgumentSemantic.Assign)]
		ARAppClipCodeUrlDecodingState UrlDecodingState { get; }

		[Export ("radius")]
		float Radius { get; }
	}

	[iOS (16, 0)]
	[BaseType (typeof (NSObject))]
	interface ARPlaneExtent : NSSecureCoding {
		[Export ("rotationOnYAxis")]
		float RotationOnYAxis { get; }

		[Export ("width")]
		float Width { get; }

		[Export ("height")]
		float Height { get; }
	}


}
