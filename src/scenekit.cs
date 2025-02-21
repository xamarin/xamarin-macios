//
// Authors:
//  Miguel de Icaza (miguel@xamarin.com)
//  Aaron Bockover (abock@xamarin.com)
//
// Copyright 2012-2014 Xamarin, Inc.
//
// TODO:
//
//   SpriteKit needs porting to Mac (see SCNSceneRenderer.OverlayScene)
//
//   SCNSceneSource's propertyForKey: takes a series of constants,
//   perhaps we should just hide that with strongly typed values, and
//   not even expose the keys.
//
//   Strong types SCNSceneSource's init options
//
//   Review docs for "nil" to annotate the various nulls.
//
//   Strongly typed dictionary support:
//     - SCNTechnique:
//       This one requires a strong type with nested components to produce the dictionary
//       it is not very obvious.
//     - SCNPhysicsShape (VERY IMPORTANT)
//     - SCNParticleSystem (not important)
//

using System;
using System.ComponentModel;
using System.Diagnostics;

using AVFoundation;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if NET
using NMatrix4 = global::CoreGraphics.NMatrix4;
using NVector3 = global::CoreGraphics.NVector3;
using Vector3 = global::CoreGraphics.NVector3;
using Vector4 = global::System.Numerics.Vector4;
#else
using NMatrix4 = global::OpenTK.NMatrix4;
using NVector3 = global::OpenTK.NVector3;
using Vector3 = global::OpenTK.NVector3;
using Vector4 = global::OpenTK.Vector4;
#endif

using CoreAnimation;
using CoreImage;

#if NET
using AnimationType = global::SceneKit.ISCNAnimationProtocol;
#else
using AnimationType = global::CoreAnimation.CAAnimation;
#endif

using CoreGraphics;
using SpriteKit;
using ModelIO;
using Metal;
using GameplayKit;

#if MONOMAC || __MACCATALYST__
using EAGLContext = System.Object;
#endif

#if MONOMAC
using AppKit;

using GLContext = global::OpenGL.CGLContext;
#else
using UIKit;
using CAOpenGLLayer = System.Object;
using NSOpenGLContext = System.Object;
using NSOpenGLPixelFormat = System.Object;

#if HAS_OPENGLES
using OpenGLES;
using GLContext = global::OpenGLES.EAGLContext;
#else
using GLContext = global::Foundation.NSObject; // won't be used -> but must compile
#endif

using NSView = global::UIKit.UIView;

using NSColor = global::UIKit.UIColor;
using NSFont = global::UIKit.UIFont;
using NSImage = global::UIKit.UIImage;
using NSBezierPath = global::UIKit.UIBezierPath;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace SceneKit {
	/// <summary>Callback used to reflect progress during execution of <see cref="M:SceneKit.SCNSceneSource.SceneFromOptions(SceneKit.SCNSceneLoadingOptions,SceneKit.SCNSceneSourceStatusHandler)" />.</summary>
	[MacCatalyst (13, 1)]
	delegate void SCNSceneSourceStatusHandler (float /* float, not CGFloat */ totalProgress, SCNSceneSourceStatus status, NSError error, ref bool stopLoading);

#if NET
	delegate void SCNAnimationDidStartHandler (SCNAnimation animation, ISCNAnimatable receiver);
#else
	[Obsolete ("Use 'SCNAnimationDidStartHandler2' instead.")]
	delegate void SCNAnimationDidStartHandler (SCNAnimation animation, SCNAnimatable receiver);

	delegate void SCNAnimationDidStartHandler2 (SCNAnimation animation, ISCNAnimatable receiver);
#endif

#if NET
	delegate void SCNAnimationDidStopHandler (SCNAnimation animation, ISCNAnimatable receiver, bool completed);
#else
	[Obsolete ("Use 'SCNAnimationDidStopHandler2' instead.")]
	delegate void SCNAnimationDidStopHandler (SCNAnimation animation, SCNAnimatable receiver, bool completed);

	delegate void SCNAnimationDidStopHandler2 (SCNAnimation animation, ISCNAnimatable receiver, bool completed);
#endif

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:SceneKit.SCNAnimatable" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:SceneKit.SCNAnimatable" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:SceneKit.SCNAnimatable" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:SceneKit.SCNAnimatable_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	/// <summary>Extension methods to the <see cref="T:SceneKit.ISCNAnimatable" /> interface to support all the methods from the <see cref="T:SceneKit.SCNAnimatable" /> protocol.</summary>
	///     <remarks>
	///       <para>The extension methods for <see cref="T:SceneKit.ISCNAnimatable" /> allow developers to treat instances of the interface as having all the optional methods of the original <see cref="T:SceneKit.SCNAnimatable" /> protocol.   Since the interface only contains the required members, these extension methods allow developers to call the optional members of the protocol.</para>
	///     </remarks>
	interface ISCNAnimatable { }

	/// <summary>Allows animations to be applied.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNAnimatable_Protocol/index.html">Apple documentation for <c>SCNAnimatable</c></related>
	[MacCatalyst (13, 1)]
	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	interface SCNAnimatable {
		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("addAnimation:forKey:")]
#if !NET
		void AddAnimation (CAAnimation animation, [NullAllowed] NSString key);
#else
		void AddAnimation (ISCNAnimationProtocol scnAnimation, [NullAllowed] string key);
#endif

#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("addAnimationPlayer:forKey:")]
		void AddAnimation (SCNAnimationPlayer player, [NullAllowed] NSString key);

		[Abstract]
		[Export ("removeAllAnimations")]
		void RemoveAllAnimations ();

#if NET
		[Abstract]
#endif
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("removeAllAnimationsWithBlendOutDuration:")]
		void RemoveAllAnimationsWithBlendOutDuration (nfloat duration);

		[Abstract]
		[Export ("removeAnimationForKey:")]
		void RemoveAnimation (NSString key);

		[Abstract]
		[Export ("animationKeys")]
		NSString [] GetAnimationKeys ();

#if NET
		[Abstract]
#endif
		[return: NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("animationPlayerForKey:")]
		SCNAnimationPlayer GetAnimationPlayer (NSString key);

		[Abstract]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'GetAnimationPlayer' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'GetAnimationPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'GetAnimationPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetAnimationPlayer' instead.")]
		[Export ("animationForKey:")]
		[return: NullAllowed]
		CAAnimation GetAnimation (NSString key);

		[Abstract]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Export ("pauseAnimationForKey:")]
		void PauseAnimation (NSString key);

		[Abstract]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Export ("resumeAnimationForKey:")]
		void ResumeAnimation (NSString key);

		[Abstract]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Export ("isAnimationForKeyPaused:")]
		bool IsAnimationPaused (NSString key);

		[Abstract]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'RemoveAnimationUsingBlendOutDuration' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'RemoveAnimationUsingBlendOutDuration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'RemoveAnimationUsingBlendOutDuration' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RemoveAnimationUsingBlendOutDuration' instead.")]
		[Export ("removeAnimationForKey:fadeOutDuration:")]
		void RemoveAnimation (NSString key, nfloat duration);

#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("removeAnimationForKey:blendOutDuration:")]
		void RemoveAnimationUsingBlendOutDuration (NSString key, nfloat blendOutDuration);

#if NET
		[Abstract]
#endif
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'SCNAnimationPlayer.Speed' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'SCNAnimationPlayer.Speed' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'SCNAnimationPlayer.Speed' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SCNAnimationPlayer.Speed' instead.")]
		[Export ("setSpeed:forAnimationKey:")]
		void SetSpeed (nfloat speed, NSString key);
	}

	/// <summary>Holds a <see cref="T:SceneKit.SCNAudioSource" /> or <see cref="T:AVFoundation.AVAudioNode" /> for use with positional audio.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNAudioPlayer_Class/index.html">Apple documentation for <c>SCNAudioPlayer</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNAudioPlayer {
		[Export ("initWithSource:")]
		[DesignatedInitializer]
		NativeHandle Constructor (SCNAudioSource source);

		[Export ("initWithAVAudioNode:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVAudioNode audioNode);

		[Static]
		[Export ("audioPlayerWithSource:")]
		SCNAudioPlayer FromSource (SCNAudioSource source);

		[Static]
		[Export ("audioPlayerWithAVAudioNode:")]
		SCNAudioPlayer AVAudioNode (AVAudioNode audioNode);

		[Export ("willStartPlayback")]
		[NullAllowed]
		Action WillStartPlayback { get; set; }

		[Export ("didFinishPlayback")]
		[NullAllowed]
		Action DidFinishPlayback { get; set; }

		[NullAllowed, Export ("audioNode")]
		AVAudioNode AudioNode { get; }

		[NullAllowed, Export ("audioSource")]
		SCNAudioSource AudioSource { get; }
	}

	/// <summary>A reusable audio source for positional audio in SceneKit scenes.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNAudioSource_Class/index.html">Apple documentation for <c>SCNAudioSource</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNAudioSource : NSCopying, NSSecureCoding {
		[Export ("initWithFileNamed:")]
		NativeHandle Constructor (string filename);

		[Export ("initWithURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl url);

		[Static]
		[Export ("audioSourceNamed:")]
		[return: NullAllowed]
		SCNAudioSource FromFile (string fileName);

		[Export ("positional")]
		bool Positional { [Bind ("isPositional")] get; set; }


		[Export ("volume")]
		float Volume { get; set; }

		[Export ("rate")]
		float Rate { get; set; }

		[Export ("reverbBlend")]
		float ReverbBlend { get; set; }

		[Export ("loops")]
		bool Loops { get; set; }

		[Export ("shouldStream")]
		bool ShouldStream { get; set; }

		[Export ("load")]
		void Load ();
	}


	/// <summary>Functions that define a volume in 3D space.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNBoundingVolume_Protocol/index.html">Apple documentation for <c>SCNBoundingVolume</c></related>
	[MacCatalyst (13, 1)]
	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	interface SCNBoundingVolume {
		[Abstract]
		[Export ("getBoundingBoxMin:max:")]
		bool GetBoundingBox (ref SCNVector3 min, ref SCNVector3 max);

		[MacCatalyst (13, 1)]
		[Abstract]
		[Export ("setBoundingBoxMin:max:")]
		void SetBoundingBox (ref SCNVector3 min, ref SCNVector3 max);

		[Abstract]
		[Export ("getBoundingSphereCenter:radius:")]
		bool GetBoundingSphere (ref SCNVector3 center, ref nfloat radius);
	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> whose faces are all rectangles.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNBox_Class/index.html">Apple documentation for <c>SCNBox</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNBox {
		[Export ("width")]
		nfloat Width { get; set; }

		[Export ("height")]
		nfloat Height { get; set; }

		[Export ("length")]
		nfloat Length { get; set; }

		[Export ("chamferRadius")]
		nfloat ChamferRadius { get; set; }

		[Export ("widthSegmentCount")]
		nint WidthSegmentCount { get; set; }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set; }

		[Export ("lengthSegmentCount")]
		nint LengthSegmentCount { get; set; }

		[Export ("chamferSegmentCount")]
		nint ChamferSegmentCount { get; set; }

		[Static, Export ("boxWithWidth:height:length:chamferRadius:")]
		SCNBox Create (nfloat width, nfloat height, nfloat length, nfloat chamferRadius);
	}

	/// <summary>Specifies a camera, with attributes such as field-of-view and depth-of-view.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNCamera_Class/index.html">Apple documentation for <c>SCNCamera</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNCamera : SCNAnimatable, SCNTechniqueSupport, NSCopying, NSSecureCoding {
		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Export ("xFov")]
		double XFov { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Export ("yFov")]
		double YFov { get; set; }

		[Export ("zNear")]
		double ZNear { get; set; }

		[Export ("zFar")]
		double ZFar { get; set; }

		[Export ("usesOrthographicProjection")]
		bool UsesOrthographicProjection { get; set; }

		[Static, Export ("camera")]
		SCNCamera Create ();

		[MacCatalyst (13, 1)]
		[Export ("projectionTransform")]
		SCNMatrix4 ProjectionTransform { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("automaticallyAdjustsZRange")]
		bool AutomaticallyAdjustsZRange { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("orthographicScale")]
		double OrthographicScale { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FocusDistance' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'FocusDistance' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'FocusDistance' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FocusDistance' instead.")]
		[Export ("focalDistance")]
		nfloat FocalDistance { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FocusDistance' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'FocusDistance' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'FocusDistance' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FocusDistance' instead.")]
		[Export ("focalSize")]
		nfloat FocalSize { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FStop' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'FStop' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'FStop' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FStop' instead.")]
		[Export ("focalBlurRadius")]
		nfloat FocalBlurRadius { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FStop' instead with FStop = SensorHeight / Aperture.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'FStop' instead with FStop = SensorHeight / Aperture.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'FStop' instead with FStop = SensorHeight / Aperture.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FStop' instead with FStop = SensorHeight / Aperture.")]
		[Export ("aperture")]
		nfloat Aperture { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("motionBlurIntensity")]
		nfloat MotionBlurIntensity { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("wantsHDR")]
		bool WantsHdr { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("exposureOffset")]
		nfloat ExposureOffset { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("averageGray")]
		nfloat AverageGray { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("whitePoint")]
		nfloat WhitePoint { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("wantsExposureAdaptation")]
		bool WantsExposureAdaptation { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("exposureAdaptationBrighteningSpeedFactor")]
		nfloat ExposureAdaptationBrighteningSpeedFactor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("exposureAdaptationDarkeningSpeedFactor")]
		nfloat ExposureAdaptationDarkeningSpeedFactor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("minimumExposure")]
		nfloat MinimumExposure { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maximumExposure")]
		nfloat MaximumExposure { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("bloomThreshold")]
		nfloat BloomThreshold { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("bloomIntensity")]
		nfloat BloomIntensity { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("bloomBlurRadius")]
		nfloat BloomBlurRadius { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("vignettingPower")]
		nfloat VignettingPower { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("vignettingIntensity")]
		nfloat VignettingIntensity { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("colorFringeStrength")]
		nfloat ColorFringeStrength { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("colorFringeIntensity")]
		nfloat ColorFringeIntensity { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("saturation")]
		nfloat Saturation { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("contrast")]
		nfloat Contrast { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("colorGrading")]
		SCNMaterialProperty ColorGrading { get; }

		[MacCatalyst (13, 1)]
		[Export ("categoryBitMask")]
		nuint CategoryBitMask { get; set; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("cameraWithMDLCamera:")]
		SCNCamera FromModelCamera (MDLCamera modelCamera);

		[MacCatalyst (13, 1)]
		[Export ("fieldOfView")]
		nfloat FieldOfView { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("projectionDirection", ArgumentSemantic.Assign)]
		SCNCameraProjectionDirection ProjectionDirection { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("focalLength")]
		nfloat FocalLength { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("sensorHeight")]
		nfloat SensorHeight { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("wantsDepthOfField")]
		bool WantsDepthOfField { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("focusDistance")]
		nfloat FocusDistance { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("focalBlurSampleCount")]
		nint FocalBlurSampleCount { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("fStop")]
		nfloat FStop { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("apertureBladeCount")]
		nint ApertureBladeCount { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("screenSpaceAmbientOcclusionIntensity")]
		nfloat ScreenSpaceAmbientOcclusionIntensity { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("screenSpaceAmbientOcclusionRadius")]
		nfloat ScreenSpaceAmbientOcclusionRadius { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("screenSpaceAmbientOcclusionBias")]
		nfloat ScreenSpaceAmbientOcclusionBias { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("screenSpaceAmbientOcclusionDepthThreshold")]
		nfloat ScreenSpaceAmbientOcclusionDepthThreshold { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("screenSpaceAmbientOcclusionNormalThreshold")]
		nfloat ScreenSpaceAmbientOcclusionNormalThreshold { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("projectionTransformWithViewportSize:")]
		SCNMatrix4 GetProjectionTransform (CGSize viewportSize);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("bloomIterationCount")]
		nint BloomIterationCount { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("bloomIterationSpread")]
		nfloat BloomIterationSpread { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("grainIntensity")]
		nfloat GrainIntensity { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("grainScale")]
		nfloat GrainScale { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("grainIsColored")]
		bool GrainIsColored { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("whiteBalanceTemperature")]
		nfloat WhiteBalanceTemperature { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("whiteBalanceTint")]
		nfloat WhiteBalanceTint { get; set; }
	}

	interface ISCNCameraControlConfiguration { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface SCNCameraControlConfiguration {
		[Abstract]
		[Export ("autoSwitchToFreeCamera")]
		bool AutoSwitchToFreeCamera { get; set; }

		[Abstract]
		[Export ("allowsTranslation")]
		bool AllowsTranslation { get; set; }

		[Abstract]
		[Export ("flyModeVelocity")]
		nfloat FlyModeVelocity { get; set; }

		[Abstract]
		[Export ("panSensitivity")]
		nfloat PanSensitivity { get; set; }

		[Abstract]
		[Export ("truckSensitivity")]
		nfloat TruckSensitivity { get; set; }

		[Abstract]
		[Export ("rotationSensitivity")]
		nfloat RotationSensitivity { get; set; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:SceneKit.SCNCameraControllerDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:SceneKit.SCNCameraControllerDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:SceneKit.SCNCameraControllerDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:SceneKit.SCNCameraControllerDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ISCNCameraControllerDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol]
	[Model] // Figured I would keep the model for convenience, as all the methods here are optional
	[BaseType (typeof (NSObject))]
	interface SCNCameraControllerDelegate {
		[Export ("cameraInertiaWillStartForController:")]
		void CameraInertiaWillStart (SCNCameraController cameraController);

		[Export ("cameraInertiaDidEndForController:")]
		void CameraInertiaDidEnd (SCNCameraController cameraController);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNCameraController {
		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		ISCNCameraControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("pointOfView", ArgumentSemantic.Retain)]
		SCNNode PointOfView { get; set; }

		[Export ("interactionMode", ArgumentSemantic.Assign)]
		SCNInteractionMode InteractionMode { get; set; }

		[Export ("target", ArgumentSemantic.Assign)]
		SCNVector3 Target { get; set; }

		[Export ("automaticTarget")]
		bool AutomaticTarget { get; set; }

		[Export ("worldUp", ArgumentSemantic.Assign)]
		SCNVector3 WorldUp { get; set; }

		[Export ("inertiaEnabled")]
		bool InertiaEnabled { get; set; }

		[Export ("inertiaFriction")]
		float InertiaFriction { get; set; }

		[Export ("inertiaRunning")]
		bool InertiaRunning { [Bind ("isInertiaRunning")] get; }

		[Export ("minimumVerticalAngle")]
		float MinimumVerticalAngle { get; set; }

		[Export ("maximumVerticalAngle")]
		float MaximumVerticalAngle { get; set; }

		[Export ("minimumHorizontalAngle")]
		float MinimumHorizontalAngle { get; set; }

		[Export ("maximumHorizontalAngle")]
		float MaximumHorizontalAngle { get; set; }

		[Export ("translateInCameraSpaceByX:Y:Z:")]
		void TranslateInCameraSpace (float deltaX, float deltaY, float deltaZ);

		[Export ("frameNodes:")]
		void FrameNodes (SCNNode [] nodes);

		[Export ("rotateByX:Y:")]
		void Rotate (float deltaX, float deltaY);

		[Export ("rollBy:aroundScreenPoint:viewport:")]
		void Roll (float delta, CGPoint screenPoint, CGSize viewport);

		[Export ("dollyBy:onScreenPoint:viewport:")]
		void Dolly (float delta, CGPoint screenPoint, CGSize viewport);

		[Export ("rollAroundTarget:")]
		void RollAroundTarget (float delta);

		[Export ("dollyToTarget:")]
		void DollyToTarget (float delta);

		[Export ("clearRoll")]
		void ClearRoll ();

		[Export ("stopInertia")]
		void StopInertia ();

		[Export ("beginInteraction:withViewport:")]
		void BeginInteraction (CGPoint location, CGSize viewport);

		[Export ("continueInteraction:withViewport:sensitivity:")]
		void ContinueInteraction (CGPoint location, CGSize viewport, nfloat sensitivity);

		[Export ("endInteraction:withViewport:velocity:")]
		void EndInteraction (CGPoint location, CGSize viewport, CGPoint velocity);
	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> that is a cylinder with capped ends.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNCapsule_Class/index.html">Apple documentation for <c>SCNCapsule</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNCapsule {
		[Export ("capRadius")]
		nfloat CapRadius { get; set; }

		[Export ("height")]
		nfloat Height { get; set; }

		[Export ("radialSegmentCount")]
		nint RadialSegmentCount { get; set; }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set; }

		[Export ("capSegmentCount")]
		nint CapSegmentCount { get; set; }

		[Static, Export ("capsuleWithCapRadius:height:")]
		SCNCapsule Create (nfloat capRadius, nfloat height);
	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> that is a circular frustum. Or, a party hat.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNCone_Class/index.html">Apple documentation for <c>SCNCone</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNCone {
		[Export ("topRadius")]
		nfloat TopRadius { get; set; }

		[Export ("bottomRadius")]
		nfloat BottomRadius { get; set; }

		[Export ("height")]
		nfloat Height { get; set; }

		[Export ("radialSegmentCount")]
		nint RadialSegmentCount { get; set; }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set; }

		[Static, Export ("coneWithTopRadius:bottomRadius:height:")]
		SCNCone Create (nfloat topRadius, nfloat bottomRadius, nfloat height);
	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> that is a right circular cylinder.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNCylinder_Class/index.html">Apple documentation for <c>SCNCylinder</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNCylinder {
		[Export ("radius")]
		nfloat Radius { get; set; }

		[Export ("height")]
		nfloat Height { get; set; }

		[Export ("radialSegmentCount")]
		nint RadialSegmentCount { get; set; }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set; }

		[Static, Export ("cylinderWithRadius:height:")]
		SCNCylinder Create (nfloat radius, nfloat height);
	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> that is an infinite plane.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNFloor_Class/index.html">Apple documentation for <c>SCNFloor</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNFloor {
		[Export ("reflectivity")]
		nfloat Reflectivity { get; set; }

		[Export ("reflectionFalloffStart")]
		nfloat ReflectionFalloffStart { get; set; }

		[Export ("reflectionFalloffEnd")]
		nfloat ReflectionFalloffEnd { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("reflectionCategoryBitMask")]
		nuint ReflectionCategoryBitMask { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("width")]
		nfloat Width { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("length")]
		nfloat Length { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("reflectionResolutionScaleFactor")]
		nfloat ReflectionResolutionScaleFactor { get; set; }

		[Static, Export ("floor")]
		SCNFloor Create ();
	}

	/// <include file="../docs/api/SceneKit/SCNGeometry.xml" path="/Documentation/Docs[@DocId='T:SceneKit.SCNGeometry']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNGeometry : SCNAnimatable, SCNBoundingVolume, SCNShadable, NSCopying, NSSecureCoding {
		[Export ("materials", ArgumentSemantic.Copy)]
		SCNMaterial [] Materials { get; set; }

		[Export ("geometryElementCount")]
		nint GeometryElementCount { get; }

		[Export ("firstMaterial", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNMaterial FirstMaterial { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("geometryElements")]
		SCNGeometryElement [] GeometryElements { get; }

		[MacCatalyst (13, 1)]
		[Export ("geometrySources")]
		SCNGeometrySource [] GeometrySources { get; }

		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Export ("insertMaterial:atIndex:")]
		void InsertMaterial (SCNMaterial material, nint index);

		[Export ("removeMaterialAtIndex:")]
		void RemoveMaterial (nint index);

		[Export ("replaceMaterialAtIndex:withMaterial:")]
		void ReplaceMaterial (nint materialIndex, SCNMaterial newMaterial);

		[Export ("materialWithName:")]
		[return: NullAllowed]
		SCNMaterial GetMaterial (string name);

		[Static]
		[Export ("geometryWithSources:elements:")]
		SCNGeometry Create (SCNGeometrySource [] sources, [NullAllowed] SCNGeometryElement [] elements);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Static]
		[Export ("geometryWithSources:elements:sourceChannels:")]
		SCNGeometry Create (SCNGeometrySource [] sources, [NullAllowed] SCNGeometryElement [] elements, [NullAllowed][BindAs (typeof (int []))] NSNumber [] sourceChannels);

		[Export ("geometrySourcesForSemantic:")]
		SCNGeometrySource [] GetGeometrySourcesForSemantic (string semantic);

		[Export ("geometryElementAtIndex:")]
		SCNGeometryElement GetGeometryElement (nint elementIndex);

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("levelsOfDetail", ArgumentSemantic.Copy)]
		SCNLevelOfDetail [] LevelsOfDetail { get; set; }

		[MacCatalyst (13, 1)]
		[Static, Export ("geometry")]
		SCNGeometry Create ();

		[MacCatalyst (13, 1)]
		[Export ("subdivisionLevel")]
		nuint SubdivisionLevel { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("edgeCreasesElement", ArgumentSemantic.Retain)]
		SCNGeometryElement EdgeCreasesElement { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("edgeCreasesSource", ArgumentSemantic.Retain)]
		SCNGeometrySource EdgeCreasesSource { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("wantsAdaptiveSubdivision")]
		bool WantsAdaptiveSubdivision { get; set; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("geometryWithMDLMesh:")]
		SCNGeometry FromMesh (MDLMesh mesh);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("tessellator", ArgumentSemantic.Retain)]
		SCNGeometryTessellator Tessellator { get; set; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("geometrySourceChannels"), NullAllowed]
		[BindAs (typeof (int []))]
		NSNumber [] GeometrySourceChannels { get; }
	}

	/// <include file="../docs/api/SceneKit/SCNGeometrySource.xml" path="/Documentation/Docs[@DocId='T:SceneKit.SCNGeometrySource']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNGeometrySource : NSSecureCoding {
		[Export ("data")]
		NSData Data { get; }

		[Export ("semantic")]
		NSString Semantic { get; }

		[Export ("vectorCount")]
		nint VectorCount { get; }

		[Export ("floatComponents")]
		bool FloatComponents { get; }

		[Export ("componentsPerVector")]
		nint ComponentsPerVector { get; }

		[Export ("bytesPerComponent")]
		nint BytesPerComponent { get; }

		[Export ("dataOffset")]
		nint DataOffset { get; }

		[Export ("dataStride")]
		nint DataStride { get; }

		[Export ("geometrySourceWithData:semantic:vectorCount:floatComponents:componentsPerVector:bytesPerComponent:dataOffset:dataStride:")]
		[Static]
		SCNGeometrySource FromData (NSData data, NSString geometrySourceSemantic, nint vectorCount, bool floatComponents, nint componentsPerVector, nint bytesPerComponent, nint offset, nint stride);

		[Static]
		[Export ("geometrySourceWithVertices:count:"), Internal]
		SCNGeometrySource FromVertices (IntPtr vertices, nint count);

		[Static]
		[Export ("geometrySourceWithNormals:count:"), Internal]
		SCNGeometrySource FromNormals (IntPtr normals, nint count);

		[Static]
		[Export ("geometrySourceWithTextureCoordinates:count:"), Internal]
		SCNGeometrySource FromTextureCoordinates (IntPtr texcoords, nint count);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("geometrySourceWithBuffer:vertexFormat:semantic:vertexCount:dataOffset:dataStride:")]
		SCNGeometrySource FromMetalBuffer (IMTLBuffer mtlBuffer, MTLVertexFormat vertexFormat, NSString geometrySourceSemantic, nint vertexCount, nint offset, nint stride);
	}

	/// <summary>Enumerates vertex mode attributes.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface SCNGeometrySourceSemantic {
		[Field ("SCNGeometrySourceSemanticVertex")]
		NSString Vertex { get; }

		[Field ("SCNGeometrySourceSemanticNormal")]
		NSString Normal { get; }

		[Field ("SCNGeometrySourceSemanticColor")]
		NSString Color { get; }

		[Field ("SCNGeometrySourceSemanticTexcoord")]
		NSString Texcoord { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNGeometrySourceSemanticTangent")]
		NSString Tangent { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNGeometrySourceSemanticVertexCrease")]
		NSString VertexCrease { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNGeometrySourceSemanticEdgeCrease")]
		NSString EdgeCrease { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNGeometrySourceSemanticBoneWeights")]
		NSString BoneWeights { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNGeometrySourceSemanticBoneIndices")]
		NSString BoneIndices { get; }
	}

	/// <summary>Combines information about vertices and a drawing primitive. Maps to a single drawing command sent to the GPU.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNGeometryElement_Class/index.html">Apple documentation for <c>SCNGeometryElement</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNGeometryElement : NSSecureCoding {
		[Export ("data")]
		NSData Data { get; }

		[Export ("primitiveType")]
		SCNGeometryPrimitiveType PrimitiveType { get; }

		[Export ("primitiveCount")]
		nint PrimitiveCount { get; }

		[Export ("bytesPerIndex")]
		nint BytesPerIndex { get; }

		[Static]
		[Export ("geometryElementWithData:primitiveType:primitiveCount:bytesPerIndex:")]
		SCNGeometryElement FromData ([NullAllowed] NSData data, SCNGeometryPrimitiveType primitiveType, nint primitiveCount, nint bytesPerIndex);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Static]
		[Export ("geometryElementWithData:primitiveType:primitiveCount:indicesChannelCount:interleavedIndicesChannels:bytesPerIndex:")]
		SCNGeometryElement FromData ([NullAllowed] NSData data, SCNGeometryPrimitiveType primitiveType, nint primitiveCount, nint indicesChannelCount, bool interleavedIndicesChannels, nint bytesPerIndex);

		[MacCatalyst (13, 1)]
		[Export ("primitiveRange", ArgumentSemantic.Assign)]
		NSRange PrimitiveRange { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("pointSize")]
		nfloat PointSize { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("minimumPointScreenSpaceRadius")]
		nfloat MinimumPointScreenSpaceRadius { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maximumPointScreenSpaceRadius")]
		nfloat MaximumPointScreenSpaceRadius { get; set; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("geometryElementWithMDLSubmesh:")]
		SCNGeometryElement FromSubmesh (MDLSubmesh submesh);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("geometryElementWithBuffer:primitiveType:primitiveCount:bytesPerIndex:")]
		SCNGeometryElement FromBuffer (IMTLBuffer buffer, SCNGeometryPrimitiveType primitiveType, nint primitiveCount, nint bytesPerIndex);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Static]
		[Export ("geometryElementWithBuffer:primitiveType:primitiveCount:indicesChannelCount:interleavedIndicesChannels:bytesPerIndex:")]
		SCNGeometryElement FromBuffer (IMTLBuffer data, SCNGeometryPrimitiveType primitiveType, nint primitiveCount, nint indicesChannelCount, bool interleavedIndicesChannels, nint bytesPerIndex);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("interleavedIndicesChannels")]
		bool InterleavedIndicesChannels { [Bind ("hasInterleavedIndicesChannels")] get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("indicesChannelCount")]
		nint IndicesChannelCount { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNGeometryTessellator : NSCopying, NSSecureCoding {
		[Export ("tessellationFactorScale")]
		nfloat TessellationFactorScale { get; set; }

		[Export ("tessellationPartitionMode", ArgumentSemantic.Assign)]
		MTLTessellationPartitionMode TessellationPartitionMode { get; set; }

		[Export ("adaptive")]
		bool Adaptive { [Bind ("isAdaptive")] get; set; }

		[Export ("screenSpace")]
		bool ScreenSpace { [Bind ("isScreenSpace")] get; set; }

		[Export ("edgeTessellationFactor")]
		nfloat EdgeTessellationFactor { get; set; }

		[Export ("insideTessellationFactor")]
		nfloat InsideTessellationFactor { get; set; }

		[Export ("maximumEdgeLength")]
		nfloat MaximumEdgeLength { get; set; }

		[Export ("smoothingMode", ArgumentSemantic.Assign)]
		SCNTessellationSmoothingMode SmoothingMode { get; set; }
	}

	/// <summary>Constants for use with the <c>options</c> argument in <see cref="M:SceneKit.SCNNode.HitTest(SceneKit.SCNVector3,SceneKit.SCNVector3,SceneKit.SCNHitTestOptions)" />.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface SCNHitTest {
		[Field ("SCNHitTestFirstFoundOnlyKey")]
		NSString FirstFoundOnlyKey { get; }

		[Field ("SCNHitTestSortResultsKey")]
		NSString SortResultsKey { get; }

		[Field ("SCNHitTestClipToZRangeKey")]
		NSString ClipToZRangeKey { get; }

		[Field ("SCNHitTestBackFaceCullingKey")]
		NSString BackFaceCullingKey { get; }

		[Field ("SCNHitTestBoundingBoxOnlyKey")]
		NSString BoundingBoxOnlyKey { get; }

		[Field ("SCNHitTestIgnoreChildNodesKey")]
		NSString IgnoreChildNodesKey { get; }

		[Field ("SCNHitTestRootNodeKey")]
		NSString RootNodeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNHitTestIgnoreHiddenNodesKey")]
		NSString IgnoreHiddenNodesKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNHitTestOptionCategoryBitMask")]
		NSString OptionCategoryBitMaskKey { get; }

#if !NET
		[Obsolete ("Use 'SearchModeKey' instead.")]
		[Field ("SCNHitTestOptionSearchMode")]
		NSString OptionSearchModeKey { get; }
#endif

		[MacCatalyst (13, 1)]
		[Field ("SCNHitTestOptionSearchMode")]
		NSString SearchModeKey { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("SCNHitTestOptionIgnoreLightArea")]
		NSString IgnoreLightAreaKey { get; }
	}

	/// <summary>Information about the SCNNode (if any) located at a specific point or along a ray.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNHitTestResult_Class/index.html">Apple documentation for <c>SCNHitTestResult</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// quote: The SCNHitTestResult class is exposed as the return object from the hitTest:options: method, as an array of SCNHitTestResult objects.
	[DisableDefaultCtor] // crash when calling description
	interface SCNHitTestResult {
		[Export ("geometryIndex")]
		nint GeometryIndex { get; }

		[Export ("faceIndex")]
		nint FaceIndex { get; }

		[Export ("localCoordinates")]
		SCNVector3 LocalCoordinates { get; }

		[Export ("worldCoordinates")]
		SCNVector3 WorldCoordinates { get; }

		[Export ("localNormal")]
		SCNVector3 LocalNormal { get; }

		[Export ("worldNormal")]
		SCNVector3 WorldNormal { get; }

		[Export ("modelTransform")]
		SCNMatrix4 ModelTransform { get; }

		[MacCatalyst (13, 1)]
		[Export ("boneNode")]
		[NullAllowed]
		SCNNode BoneNode { get; }

		[Export ("node")]
		SCNNode Node { get; }

		[Export ("textureCoordinatesWithMappingChannel:")]
		CGPoint GetTextureCoordinatesWithMappingChannel (nint channel);
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Please use Metal instead of OpenGL API.")]
	[BaseType (typeof (CAOpenGLLayer))]
	interface SCNLayer : SCNSceneRenderer, SCNTechniqueSupport {
		//		We already pull in the Scene property from the SCNSceneRenderer protocol, no need to redefine it here.
		//		[Export ("scene", ArgumentSemantic.Retain)]
		//		SCNScene Scene { get; set;  }
	}

	/// <summary>A light source, used for all SCNLightTypes.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNLight_Class/index.html">Apple documentation for <c>SCNLight</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNLight : SCNAnimatable, SCNTechniqueSupport, NSCopying, NSSecureCoding {
		[Export ("type", ArgumentSemantic.Copy)]
		NSString LightType { get; set; }

		[Export ("color", ArgumentSemantic.Retain)]
		NSObject WeakColor { get; set; }

		[Wrap ("WeakColor")]
		NSColor Color { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("temperature")]
		nfloat Temperature { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("intensity")]
		nfloat Intensity { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Export ("castsShadow")]
		bool CastsShadow { get; set; }

		[Export ("shadowColor", ArgumentSemantic.Retain)]
		NSObject WeakShadowColor { get; set; }

		[Wrap ("WeakShadowColor")]
		NSColor ShadowColor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("shadowBias")]
		nfloat ShadowBias { get; set; }

		[Export ("shadowRadius")]
		nfloat ShadowRadius { get; set; }

		[Static, Export ("light")]
		SCNLight Create ();

		[NoiOS]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[NoMacCatalyst]
		[Export ("attributeForKey:")]
		[return: NullAllowed]
		NSObject GetAttribute (NSString lightAttribute);

		[NoiOS]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[NoMacCatalyst]
		[Export ("setAttribute:forKey:")]
		void SetAttribute ([NullAllowed] NSObject value, NSString attribuetKey);

		[MacCatalyst (13, 1)]
		[Export ("gobo")]
		[NullAllowed]
		SCNMaterialProperty Gobo { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("IESProfileURL", ArgumentSemantic.Retain)]
		NSUrl IesProfileUrl { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("shadowMapSize")]
		CGSize ShadowMapSize { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("shadowSampleCount")]
		nuint ShadowSampleCount { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("shadowMode")]
		SCNShadowMode ShadowMode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("attenuationStartDistance")]
		nfloat AttenuationStartDistance { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("attenuationEndDistance")]
		nfloat AttenuationEndDistance { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("attenuationFalloffExponent")]
		nfloat AttenuationFalloffExponent { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("spotInnerAngle")]
		nfloat SpotInnerAngle { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("spotOuterAngle")]
		nfloat SpotOuterAngle { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("orthographicScale")]
		nfloat OrthographicScale { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("zNear")]
		nfloat ZNear { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("zFar")]
		nfloat ZFar { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("categoryBitMask")]
		nuint CategoryBitMask { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("automaticallyAdjustsShadowProjection")]
		bool AutomaticallyAdjustsShadowProjection { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maximumShadowDistance")]
		nfloat MaximumShadowDistance { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("forcesBackFaceCasters")]
		bool ForcesBackFaceCasters { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("sampleDistributedShadowMaps")]
		bool SampleDistributedShadowMaps { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("shadowCascadeCount")]
		nuint ShadowCascadeCount { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("shadowCascadeSplittingFactor")]
		nfloat ShadowCascadeSplittingFactor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("sphericalHarmonicsCoefficients", ArgumentSemantic.Copy)]
		NSData SphericalHarmonicsCoefficients { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("lightWithMDLLight:")]
		SCNLight FromModelLight (MDLLight mdllight);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("probeType", ArgumentSemantic.Assign)]
		SCNLightProbeType ProbeType { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("probeUpdateType", ArgumentSemantic.Assign)]
		SCNLightProbeUpdateType ProbeUpdateType { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("probeExtents", ArgumentSemantic.Assign)]
		Vector3 ProbeExtents {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("probeOffset", ArgumentSemantic.Assign)]
		Vector3 ProbeOffset {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("parallaxCorrectionEnabled")]
		bool ParallaxCorrectionEnabled { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("parallaxExtentsFactor", ArgumentSemantic.Assign)]
		Vector3 ParallaxExtentsFactor {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("parallaxCenterOffset", ArgumentSemantic.Assign)]
		Vector3 ParallaxCenterOffset {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("probeEnvironment")]
		SCNMaterialProperty ProbeEnvironment { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("areaType", ArgumentSemantic.Assign)]
		SCNLightAreaType AreaType { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("areaExtents", ArgumentSemantic.Assign)]
		Vector3 AreaExtents {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("areaPolygonVertices", ArgumentSemantic.Copy)]
		NSValue [] AreaPolygonVertices { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("drawsArea")]
		bool DrawsArea { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("doubleSided")]
		bool DoubleSided { get; set; }
	}

	/// <summary>Enumerates the types of <see cref="T:SceneKit.SCNLight" />.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface SCNLightType {
		[Field ("SCNLightTypeAmbient")]
		NSString Ambient { get; }

		[Field ("SCNLightTypeOmni")]
		NSString Omni { get; }

		[Field ("SCNLightTypeDirectional")]
		NSString Directional { get; }

		[Field ("SCNLightTypeSpot")]
		NSString Spot { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNLightTypeIES")]
		NSString Ies { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNLightTypeProbe")]
		NSString Probe { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("SCNLightTypeArea")]
		NSString Area { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	[Static]
	interface SCNLightAttribute {
		[Field ("SCNLightAttenuationStartKey")]
		NSString AttenuationStartKey { get; }

		[Field ("SCNLightAttenuationEndKey")]
		NSString AttenuationEndKey { get; }

		[Field ("SCNLightAttenuationFalloffExponentKey")]
		NSString AttenuationFalloffExponentKey { get; }

		[Field ("SCNLightSpotInnerAngleKey")]
		NSString SpotInnerAngleKey { get; }

		[Field ("SCNLightSpotOuterAngleKey")]
		NSString SpotOuterAngleKey { get; }

		[Field ("SCNLightShadowNearClippingKey")]
		NSString ShadowNearClippingKey { get; }

		[Field ("SCNLightShadowFarClippingKey")]
		NSString ShadowFarClippingKey { get; }
	}

	/// <summary>Constants for use with the <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=P:SceneKit.SCNMaterial.LightingModel&amp;scope=Xamarin" title="P:SceneKit.SCNMaterial.LightingModel">P:SceneKit.SCNMaterial.LightingModel</a></format> property, which defines the way lights and materials combine to produce a particular rendered color.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface SCNLightingModel {
		[Field ("SCNLightingModelPhong")]
		NSString Phong { get; }

		[Field ("SCNLightingModelBlinn")]
		NSString Blinn { get; }

		[Field ("SCNLightingModelLambert")]
		NSString Lambert { get; }

		[Field ("SCNLightingModelConstant")]
		NSString Constant { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNLightingModelPhysicallyBased")]
		NSString PhysicallyBased { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("SCNLightingModelShadowOnly")]
		NSString ShadowOnly { get; }
	}

	/// <summary>Specifies the lighting and shading characteristics of the surface of an SCNGeometry.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNMaterial_Class/index.html">Apple documentation for <c>SCNMaterial</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNMaterial : SCNAnimatable, SCNShadable, NSCopying, NSSecureCoding {
		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Export ("diffuse")]
		SCNMaterialProperty Diffuse { get; }

		[Export ("ambient")]
		SCNMaterialProperty Ambient { get; }

		[Export ("specular")]
		SCNMaterialProperty Specular { get; }

		[Export ("emission")]
		SCNMaterialProperty Emission { get; }

		[Export ("transparent")]
		SCNMaterialProperty Transparent { get; }

		[Export ("reflective")]
		SCNMaterialProperty Reflective { get; }

		[Export ("multiply")]
		SCNMaterialProperty Multiply { get; }

		[Export ("normal")]
		SCNMaterialProperty Normal { get; }

		[Export ("shininess")]
		nfloat Shininess { get; set; }

		[Export ("transparency")]
		nfloat Transparency { get; set; }

		[Export ("lightingModelName", ArgumentSemantic.Copy)]
		NSString LightingModelName { get; set; }

		[Export ("litPerPixel")]
		bool LitPerPixel { [Bind ("isLitPerPixel")] get; set; }

		[Export ("doubleSided")]
		bool DoubleSided { [Bind ("isDoubleSided")] get; set; }

		[Export ("cullMode")]
		SCNCullMode CullMode { get; set; }

		[Export ("transparencyMode")]
		SCNTransparencyMode TransparencyMode { get; set; }

		[Export ("locksAmbientWithDiffuse")]
		bool LocksAmbientWithDiffuse { get; set; }

		[Export ("writesToDepthBuffer")]
		bool WritesToDepthBuffer { get; set; }

		[Static, Export ("material")]
		SCNMaterial Create ();

		[MacCatalyst (13, 1)]
		[Export ("readsFromDepthBuffer")]
		bool ReadsFromDepthBuffer { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("fresnelExponent")]
		nfloat FresnelExponent { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("blendMode", ArgumentSemantic.Assign)]
		SCNBlendMode BlendMode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("ambientOcclusion")]
		SCNMaterialProperty AmbientOcclusion { get; }

		[MacCatalyst (13, 1)]
		[Export ("selfIllumination")]
		SCNMaterialProperty SelfIllumination { get; }

		[MacCatalyst (13, 1)]
		[Export ("metalness")]
		SCNMaterialProperty Metalness { get; }

		[MacCatalyst (13, 1)]
		[Export ("roughness")]
		SCNMaterialProperty Roughness { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("materialWithMDLMaterial:")]
		SCNMaterial FromMaterial (MDLMaterial material);

		[MacCatalyst (13, 1)]
		[Export ("displacement")]
		SCNMaterialProperty Displacement { get; }

		[MacCatalyst (13, 1)]
		[Export ("fillMode", ArgumentSemantic.Assign)]
		SCNFillMode FillMode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("colorBufferWriteMask", ArgumentSemantic.Assign)]
		SCNColorMask ColorBufferWriteMask { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("clearCoat")]
		SCNMaterialProperty ClearCoat { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("clearCoatRoughness")]
		SCNMaterialProperty ClearCoatRoughness { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("clearCoatNormal")]
		SCNMaterialProperty ClearCoatNormal { get; }
	}

	/// <summary>Configures the various properties (such as color and texture) of a <see cref="T:SceneKit.SCNMaterial" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNMaterialProperty_Class/index.html">Apple documentation for <c>SCNMaterialProperty</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // runtime -> [SCNKit ERROR] Do not instantiate SCNMaterialProperty objects directly
	interface SCNMaterialProperty : SCNAnimatable, NSSecureCoding {
		[Export ("minificationFilter")]
		SCNFilterMode MinificationFilter { get; set; }

		[Export ("magnificationFilter")]
		SCNFilterMode MagnificationFilter { get; set; }

		[Export ("mipFilter")]
		SCNFilterMode MipFilter { get; set; }

		[Export ("contentsTransform")]
		SCNMatrix4 ContentsTransform { get; set; }

		[Export ("wrapS")]
		SCNWrapMode WrapS { get; set; }

		[Export ("wrapT")]
		SCNWrapMode WrapT { get; set; }

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[NullAllowed, Export ("borderColor", ArgumentSemantic.Retain)]
		NSObject BorderColor { get; set; }

		[Export ("mappingChannel")]
		nint MappingChannel { get; set; }

		[Export ("contents", ArgumentSemantic.Retain), NullAllowed]
		NSObject Contents { get; set; }

		[Wrap ("Contents")]
		NSColor ContentColor { get; set; }

		[Wrap ("Contents")]
		NSImage ContentImage { get; set; }

		[MacCatalyst (13, 1)]
		[Wrap ("Contents")]
		CALayer ContentLayer { get; set; }

		[Wrap ("Contents")]
		NSString ContentPath { get; set; }

		[Wrap ("Contents")]
		NSUrl ContentUrl { get; set; }

		[Wrap ("Contents")]
		SKScene ContentScene { get; set; }

		[Wrap ("Contents")]
		SKTexture ContentTexture { get; set; }

		[Wrap ("Contents")]
		NSImage [] ContentImageCube { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("intensity")]
		nfloat Intensity { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maxAnisotropy")]
		nfloat MaxAnisotropy { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("textureComponents", ArgumentSemantic.Assign)]
		SCNColorMask TextureComponents { get; set; }

		[MacCatalyst (13, 1)]
		[Static, Export ("materialPropertyWithContents:")]
		SCNMaterialProperty Create (NSObject contents);

		[Static]
		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[return: NullAllowed]
		[Export ("precomputedLightingEnvironmentContentsWithURL:error:")]
		NSObject GetPrecomputedLightingEnvironmentContents (NSUrl url, out NSError error);

		[Static]
		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[return: NullAllowed]
		[Export ("precomputedLightingEnvironmentContentsWithData:error:")]
		NSObject GetPrecomputedLightingEnvironmentContents (NSData url, out NSError error);

		[Static]
		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[return: NullAllowed]
		[Export ("precomputedLightingEnvironmentDataForContents:device:error:")]
		NSData GetPrecomputedLightingEnvironmentData (NSObject contents, [NullAllowed] IMTLDevice device, out NSError error);
	}

	/// <summary>Defines the <see cref="P:SceneKit.SCNProgramSemanticOptions.MappingChannel" /> for use with <see cref="M:SceneKit.SCNProgram.SetSemantic(Foundation.NSString,System.String,SceneKit.SCNProgramSemanticOptions)" />.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("SCNProgram")]
	interface SCNProgramSemanticOptions {
		nuint MappingChannel { get; set; }
	}

	/// <summary>Configuration options for hit-testing in SCNNode and SCNSceneRenderer</summary>
	///     <remarks>
	///       <para>
	/// 	You create an instance of this class to configure how hit-testing should be evaluated by the HitTest methods.
	///       </para>
	///       <para>
	/// 	Hit testing uses two 3D vectors in the case of <see cref="T:SceneKit.SCNNode" /> and a 2D point in the case of the <see cref="T:SceneKit.SCNSceneRenderer" />.
	///       </para>
	///     </remarks>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("SCNHitTest")]
	interface SCNHitTestOptions {
		bool FirstFoundOnly { get; set; }
		bool SortResults { get; set; }
		bool BackFaceCulling { get; set; }
		bool BoundingBoxOnly { get; set; }
		bool IgnoreChildNodes { get; set; }
		bool IgnoreHiddenNodes { get; set; }
		[NullAllowed]
		SCNNode RootNode { get; set; }
		SCNHitTestSearchMode SearchMode { get; set; }
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		bool IgnoreLightArea { get; set; }
	}

	/// <summary>Options for scene loading.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("SCNSceneSourceLoading")]
	interface SCNSceneLoadingOptions {
		NSUrl [] AssetDirectoryUrls { get; set; }
		bool CreateNormalsIfAbsent { get; set; }
		bool FlattenScene { get; set; }
		bool CheckConsistency { get; set; }
		bool OverrideAssetUrls { get; set; }
		bool StrictConformance { get; set; }
		bool UseSafeMode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("SCNSceneSourceLoading.OptionPreserveOriginalTopology")]
		bool PreserveOriginalTopology { get; set; }

		// note: generator's StrongDictionary does not support No* attributes yet
		[NoTV]
		[MacCatalyst (13, 1)]
		float ConvertUnitsToMeters { get; set; } /* 'floating value encapsulated in a NSNumber' probably a float since it's a graphics framework */
		[NoTV]
		[MacCatalyst (13, 1)]
		bool ConvertToYUp { get; set; }

		[Internal, Export ("SCNSceneSourceLoading.AnimationImportPolicyKey")]
		NSString _AnimationImportPolicyKey { get; set; }
	}

	/// <param name="node">The current <see cref="T:SceneKit.SCNNode" />.</param>
	///     <param name="stop">An <c>out</c> parameter that, when set to true, stops the enumeration.</param>
	///     <summary>Delegate for use with <see cref="M:SceneKit.SCNNode.EnumerateChildNodes(SceneKit.SCNNodePredicate)" />.</summary>
	[MacCatalyst (13, 1)]
	delegate bool SCNNodePredicate (SCNNode node, out bool stop);

	[MacCatalyst (13, 1)]
	delegate void SCNNodeHandler (SCNNode node, out bool stop);

	/// <summary>A node in the scene graph.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNNode_Class/index.html">Apple documentation for <c>SCNNode</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNNode : SCNAnimatable, SCNBoundingVolume, SCNActionable, NSCopying, NSSecureCoding
#if (IOS || TVOS) && !XAMCORE_5_0 // Conformance Removed in Xcode 16.1
		, UIFocusItem
#endif
	{
		[Export ("transform")]
		SCNMatrix4 Transform { get; set; }

		[Export ("position")]
		SCNVector3 Position { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("worldPosition", ArgumentSemantic.Assign)]
		SCNVector3 WorldPosition { get; set; }

		[Export ("rotation")]
		SCNVector4 Rotation { get; set; }

		[Export ("scale")]
		SCNVector3 Scale { get; set; }

		[Export ("pivot")]
		SCNMatrix4 Pivot { get; set; }

		[Export ("worldTransform")]
		SCNMatrix4 WorldTransform {
			get; [MacCatalyst (13, 1)]
			set;
		}

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[Export ("opacity")]
		nfloat Opacity { get; set; }

		[Export ("renderingOrder")]
		nint RenderingOrder { get; set; }

		[Export ("parentNode")]
		[NullAllowed]
		SCNNode ParentNode { get; }

		[Export ("childNodes")]
		SCNNode [] ChildNodes { get; }

		[Export ("light", ArgumentSemantic.Retain)]
		[NullAllowedAttribute]
		SCNLight Light { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("camera", ArgumentSemantic.Retain)]
		SCNCamera Camera { get; set; }

		[Export ("geometry", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNGeometry Geometry { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("rendererDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakRendererDelegate { get; set; }

		[MacCatalyst (13, 1)]
		[Wrap ("WeakRendererDelegate")]
		ISCNNodeRendererDelegate RendererDelegate { get; set; }

		[Static, Export ("node")]
		SCNNode Create ();

		[Static]
		[Export ("nodeWithGeometry:")]
		SCNNode FromGeometry ([NullAllowed] SCNGeometry geometry);

		[Export ("presentationNode")]
		SCNNode PresentationNode { get; }

		[Export ("insertChildNode:atIndex:")]
		void InsertChildNode (SCNNode child, nint index);

		[Export ("replaceChildNode:with:")]
		void ReplaceChildNode (SCNNode child, SCNNode child2);

		[Export ("removeFromParentNode")]
		void RemoveFromParentNode ();

		[Export ("addChildNode:")]
		void AddChildNode (SCNNode child);

		[Export ("childNodeWithName:recursively:")]
		[return: NullAllowed]
		SCNNode FindChildNode (string childName, bool recursively);

		[Export ("childNodesPassingTest:")]
		SCNNode [] FindNodes (SCNNodePredicate predicate);

		[Export ("clone")]
		SCNNode Clone ();

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("skinner", ArgumentSemantic.Retain)]
		SCNSkinner Skinner { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("morpher", ArgumentSemantic.Retain)]
		SCNMorpher Morpher { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("worldOrientation", ArgumentSemantic.Assign)]
		SCNQuaternion WorldOrientation { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("orientation")]
		SCNQuaternion Orientation { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("constraints", ArgumentSemantic.Copy)]
		[NullAllowed]
		SCNConstraint [] Constraints { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("filters", ArgumentSemantic.Copy)]
		CIFilter [] Filters { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("flattenedClone")]
		SCNNode FlattenedClone ();

		[MacCatalyst (13, 1)]
		[Export ("convertPosition:toNode:")]
		SCNVector3 ConvertPositionToNode (SCNVector3 position, [NullAllowed] SCNNode node);

		[MacCatalyst (13, 1)]
		[Export ("convertPosition:fromNode:")]
		SCNVector3 ConvertPositionFromNode (SCNVector3 position, [NullAllowed] SCNNode node);

		[MacCatalyst (13, 1)]
		[Export ("convertVector:toNode:")]
		SCNVector3 ConvertVectorToNode (SCNVector3 vector, [NullAllowed] SCNNode node);

		[MacCatalyst (13, 1)]
		[Export ("convertVector:fromNode:")]
		SCNVector3 ConvertVectorFromNode (SCNVector3 vector, [NullAllowed] SCNNode node);

		[MacCatalyst (13, 1)]
		[Export ("convertTransform:fromNode:")]
		SCNMatrix4 ConvertTransformFromNode (SCNMatrix4 transform, [NullAllowed] SCNNode node);

		[MacCatalyst (13, 1)]
		[Export ("convertTransform:toNode:")]
		SCNMatrix4 ConvertTransformToNode (SCNMatrix4 transform, [NullAllowed] SCNNode node);

		[MacCatalyst (13, 1)]
		[Export ("hitTestWithSegmentFromPoint:toPoint:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNHitTestResult [] HitTest (SCNVector3 pointA, SCNVector3 pointB, [NullAllowed] NSDictionary options);

		[Wrap ("HitTest (pointA, pointB, options.GetDictionary ())")]
		SCNHitTestResult [] HitTest (SCNVector3 pointA, SCNVector3 pointB, [NullAllowed] SCNHitTestOptions options);

		[MacCatalyst (13, 1)]
		[Export ("eulerAngles")]
		SCNVector3 EulerAngles { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("castsShadow")]
		bool CastsShadow { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("movabilityHint", ArgumentSemantic.Assign)]
		SCNMovabilityHint MovabilityHint { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("physicsBody", ArgumentSemantic.Retain), NullAllowed]
		SCNPhysicsBody PhysicsBody { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("physicsField", ArgumentSemantic.Retain)]
		SCNPhysicsField PhysicsField { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("categoryBitMask")]
		nuint CategoryBitMask { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("enumerateChildNodesUsingBlock:")]
		void EnumerateChildNodes (SCNNodeHandler handler);

		[MacCatalyst (13, 1)]
		[Export ("enumerateHierarchyUsingBlock:")]
		void EnumerateHierarchy (SCNNodeHandler handler);

		[MacCatalyst (13, 1)]
		[Export ("focusBehavior", ArgumentSemantic.Assign)]
		SCNNodeFocusBehavior FocusBehavior { get; set; }

		#region SCNParticleSystemSupport (SCNNode) category

		[MacCatalyst (13, 1)]
		[Export ("particleSystems")]
		[NullAllowed]
		SCNParticleSystem [] ParticleSystems { get; }

		[MacCatalyst (13, 1)]
		[Export ("addParticleSystem:")]
		void AddParticleSystem (SCNParticleSystem system);

		[MacCatalyst (13, 1)]
		[Export ("removeAllParticleSystems")]
		void RemoveAllParticleSystems ();

		[MacCatalyst (13, 1)]
		[Export ("removeParticleSystem:")]
		void RemoveParticleSystem (SCNParticleSystem system);

		#endregion

		#region SCNAudioSupport (SCNNode) category

		[MacCatalyst (13, 1)]
		[Export ("audioPlayers")]
		SCNAudioPlayer [] AudioPlayers { get; }

		[MacCatalyst (13, 1)]
		[Export ("addAudioPlayer:")]
		void AddAudioPlayer (SCNAudioPlayer player);

		[MacCatalyst (13, 1)]
		[Export ("removeAllAudioPlayers")]
		void RemoveAllAudioPlayers ();

		[MacCatalyst (13, 1)]
		[Export ("removeAudioPlayer:")]
		void RemoveAudioPlayer (SCNAudioPlayer player);

		#endregion

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("nodeWithMDLObject:")]
		SCNNode FromModelObject (MDLObject mdlObject);

		// From SCNNode (Transforms) Category

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localUp")]
		SCNVector3 LocalUp { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localRight")]
		SCNVector3 LocalRight { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localFront")]
		SCNVector3 LocalFront { get; }

		[MacCatalyst (13, 1)]
		[Export ("worldUp")]
		SCNVector3 WorldUp { get; }

		[MacCatalyst (13, 1)]
		[Export ("worldRight")]
		SCNVector3 WorldRight { get; }

		[MacCatalyst (13, 1)]
		[Export ("worldFront")]
		SCNVector3 WorldFront { get; }

		[MacCatalyst (13, 1)]
		[Export ("lookAt:")]
		void Look (SCNVector3 worldTarget);

		[MacCatalyst (13, 1)]
		[Export ("lookAt:up:localFront:")]
		void Look (SCNVector3 worldTarget, SCNVector3 worldUp, SCNVector3 localFront);

		[MacCatalyst (13, 1)]
		[Export ("localTranslateBy:")]
		void LocalTranslate (SCNVector3 translation);

		[MacCatalyst (13, 1)]
		[Export ("localRotateBy:")]
		void LocalRotate (SCNQuaternion rotation);

		[MacCatalyst (13, 1)]
		[Export ("rotateBy:aroundTarget:")]
		void Rotate (SCNQuaternion worldRotation, SCNVector3 worldTarget);

		// From SCNNode (SIMD) Category
		// Unfortunatelly had to prefix some props Simd due to the property name is already taken
		// by the SCN* version.

		// We took the decision to comment the following helpers since they currently do not bring
		// any benefits over the SCN* versions, actually the SIMD types could potentially be just
		// a little slower than the SCN* versions due to manual native conversion. If you really
		// need them please file a bug at https://github.com/xamarin/xamarin-macios/issues/new

		// Please add the following code to runtime/bindings-generator.cs if you reenable the following SIMD methods.

		/*
		 * Add to runtime/bindings-generator.cs
		 *

		data.Add (
				new FunctionData {
					Comment = " // void func (Quaternion, NVector3)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.QuatF },
						new ParameterData { TypeData = Types.NVector3 }
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (NVector3, NVector3, NVector3)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.NVector3 },
						new ParameterData { TypeData = Types.NVector3 },
						new ParameterData { TypeData = Types.NVector3 }
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // NMatrix4 func (NMatrix4, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.NMatrix4,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.NMatrix4 },
						new ParameterData { TypeData = Types.IntPtr }
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // NVector3 func (NVector3, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.NVector3,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.NVector3 },
						new ParameterData { TypeData = Types.IntPtr }
					},
				}
			);

		 * End of bindings-generator.cs.
		 */

		//
		//[Export ("simdTransform", ArgumentSemantic.Assign)]
		//NMatrix4 SimdTransform {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//
		//[Export ("simdPosition", ArgumentSemantic.Assign)]
		//NVector3 SimdPosition {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//
		//[Export ("simdRotation", ArgumentSemantic.Assign)]
		//Vector4 SimdRotation {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//
		//[Export ("simdOrientation", ArgumentSemantic.Assign)]
		//Quaternion SimdOrientation {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//
		//[Export ("simdEulerAngles", ArgumentSemantic.Assign)]
		//NVector3 SimdEulerAngles {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//
		//[Export ("simdScale", ArgumentSemantic.Assign)]
		//NVector3 SimdScale {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//
		//[Export ("simdPivot", ArgumentSemantic.Assign)]
		//NMatrix4 SimdPivot {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//
		//[Export ("simdWorldPosition", ArgumentSemantic.Assign)]
		//NVector3 SimdWorldPosition {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//
		//[Export ("simdWorldOrientation", ArgumentSemantic.Assign)]
		//Quaternion SimdWorldOrientation {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//
		//[Export ("simdWorldTransform", ArgumentSemantic.Assign)]
		//NMatrix4 SimdWorldTransform {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//
		//[Export ("simdConvertPosition:toNode:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//NVector3 ConvertPositionToNode (NVector3 position, [NullAllowed] SCNNode node);

		//
		//[Export ("simdConvertPosition:fromNode:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//NVector3 ConvertPositionFromNode (NVector3 position, [NullAllowed] SCNNode node);

		//
		//[Export ("simdConvertVector:toNode:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//NVector3 ConvertVectorToNode (NVector3 vector, [NullAllowed] SCNNode node);

		//
		//[Export ("simdConvertVector:fromNode:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//NVector3 ConvertVectorFromNode (NVector3 vector, [NullAllowed] SCNNode node);

		//
		//[Export ("simdConvertTransform:toNode:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//NMatrix4 ConvertTransformToNode (NMatrix4 transform, [NullAllowed] SCNNode node);

		//
		//[Export ("simdConvertTransform:fromNode:")]
		//NMatrix4 ConvertTransformFromNode (NMatrix4 transform, [NullAllowed] SCNNode node);

		//
		//[Static]
		//[Export ("simdLocalUp")]
		//NVector3 SimdLocalUp {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//
		//[Static]
		//[Export ("simdLocalRight")]
		//NVector3 SimdLocalRight {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//
		//[Static]
		//[Export ("simdLocalFront")]
		//NVector3 SimdLocalFront {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//
		//[Export ("simdWorldUp")]
		//NVector3 SimdWorldUp {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//
		//[Export ("simdWorldRight")]
		//NVector3 SimdWorldRight {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//
		//[Export ("simdWorldFront")]
		//NVector3 SimdWorldFront {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//
		//[Export ("simdLookAt:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//void Look (NVector3 worldTarget);

		//
		//[Export ("simdLookAt:up:localFront:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//void Look (NVector3 worldTarget, NVector3 worldUp, NVector3 localFront);

		//
		//[Export ("simdLocalTranslateBy:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//void LocalTranslate (NVector3 translation);

		//
		//[Export ("simdLocalRotateBy:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//void LocalRotate (Quaternion rotation);

		//
		//[Export ("simdRotateBy:aroundTarget:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//void Rotate (Quaternion worldRotation, NVector3 worldTarget);
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:SceneKit.SCNNodeRendererDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:SceneKit.SCNNodeRendererDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:SceneKit.SCNNodeRendererDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:SceneKit.SCNNodeRendererDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ISCNNodeRendererDelegate { }

	/// <summary>Interface defining delegate object functions for adding custom OpenGL drawing code.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNNodeRendererDelegate_Protocol/index.html">Apple documentation for <c>SCNNodeRendererDelegate</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model, Protocol]
	interface SCNNodeRendererDelegate {
		[Export ("renderNode:renderer:arguments:")]
		void Render (SCNNode node, SCNRenderer renderer, NSDictionary arguments);
	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> that is a rectangular single-sided finite plane.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPlane_Class/index.html">Apple documentation for <c>SCNPlane</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNPlane {
		[Export ("width")]
		nfloat Width { get; set; }

		[Export ("height")]
		nfloat Height { get; set; }

		[Export ("widthSegmentCount")]
		nint WidthSegmentCount { get; set; }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set; }

		[Static, Export ("planeWithWidth:height:")]
		SCNPlane Create (nfloat width, nfloat height);

		[MacCatalyst (13, 1)]
		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("cornerSegmentCount")]
		nint CornerSegmentCount { get; set; }
	}

#if NET
	delegate void SCNBufferBindingHandler (ISCNBufferStream buffer, SCNNode node, ISCNShadable shadable, SCNRenderer renderer);
#else
	[Obsolete ("Use 'SCNBufferBindingHandler2' instead.")]
	delegate void SCNBufferBindingHandler (ISCNBufferStream buffer, SCNNode node, SCNShadable shadable, SCNRenderer renderer);

	delegate void SCNBufferBindingHandler2 (ISCNBufferStream buffer, SCNNode node, ISCNShadable shadable, SCNRenderer renderer);
#endif // NET

	/// <summary>Performs custom rendering using shaders written in OpenGL Shading Language.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNProgram_Class/index.html">Apple documentation for <c>SCNProgram</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNProgram : NSCopying, NSSecureCoding {
		[NullAllowed]
		[Export ("vertexShader", ArgumentSemantic.Copy)]
		string VertexShader { get; set; }

		[NullAllowed]
		[Export ("fragmentShader", ArgumentSemantic.Copy)]
		string FragmentShader { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("vertexFunctionName")]
		string VertexFunctionName { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("fragmentFunctionName")]
		string FragmentFunctionName { get; set; }

#if NET
		[MacCatalyst (13, 1)]
		[Export ("handleBindingOfBufferNamed:frequency:usingBlock:")]
		void HandleBinding (string name, SCNBufferFrequency frequency, SCNBufferBindingHandler handler);
#else
		[Obsolete ("Use 'HandleBinding' overload with 'SCNBufferBindingHandler2' parameter instead.")]
		[Export ("handleBindingOfBufferNamed:frequency:usingBlock:")]
		void HandleBinding (string name, SCNBufferFrequency frequency, SCNBufferBindingHandler handler);

		[Sealed]
		[Export ("handleBindingOfBufferNamed:frequency:usingBlock:")]
		void HandleBinding (string name, SCNBufferFrequency frequency, SCNBufferBindingHandler2 handler);
#endif // NET

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		ISCNProgramDelegate Delegate { get; set; }

		[Static, Export ("program")]
		SCNProgram Create ();

		[Export ("setSemantic:forSymbol:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		void SetSemantic ([NullAllowed] NSString geometrySourceSemantic, string symbol, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("SetSemantic (geometrySourceSemantic, symbol, options.GetDictionary ())")]
		void SetSemantic (NSString geometrySourceSemantic, string symbol, SCNProgramSemanticOptions options);

		[Export ("semanticForSymbol:")]
		[return: NullAllowed]
#if NET
		NSString GetSemantic (string symbol);
#else
		NSString GetSemanticForSymbol (string symbol);
#endif

		[MacCatalyst (13, 1)]
		[Field ("SCNProgramMappingChannelKey")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString MappingChannelKey { get; }

		[MacCatalyst (13, 1)]
		[Export ("opaque")]
		bool Opaque { [Bind ("isOpaque")] get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("library", ArgumentSemantic.Retain)]
		IMTLLibrary Library { get; set; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:SceneKit.SCNProgramDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:SceneKit.SCNProgramDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:SceneKit.SCNProgramDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:SceneKit.SCNProgramDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ISCNProgramDelegate { }

	/// <summary>Delegate object for SCNProgram objects.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNProgramDelegate_Protocol/index.html">Apple documentation for <c>SCNProgramDelegate</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model, Protocol]
	interface SCNProgramDelegate {

		[Unavailable (PlatformName.iOS)]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Export ("program:bindValueForSymbol:atLocation:programID:renderer:")]
		bool BindValue (SCNProgram program, string symbol, uint /* unsigned int */ location, uint /* unsigned int */ programID, SCNRenderer renderer);

		[Unavailable (PlatformName.iOS)]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Export ("program:unbindValueForSymbol:atLocation:programID:renderer:")]
		void UnbindValue (SCNProgram program, string symbol, uint /* unsigned int */ location, uint /* unsigned int */ programID, SCNRenderer renderer);

		[Export ("program:handleError:")]
		void HandleError (SCNProgram program, NSError error);

		[NoiOS]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use the SCNProgram's Opaque property instead.")]
		[NoMacCatalyst]
		[Export ("programIsOpaque:")]
		bool IsProgramOpaque (SCNProgram program);
	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> that is a right rectangular pyramid.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPyramid_Class/index.html">Apple documentation for <c>SCNPyramid</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNPyramid {
		[Export ("width")]
		nfloat Width { get; set; }

		[Export ("height")]
		nfloat Height { get; set; }

		[Export ("length")]
		nfloat Length { get; set; }

		[Export ("widthSegmentCount")]
		nint WidthSegmentCount { get; set; }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set; }

		[Export ("lengthSegmentCount")]
		nint LengthSegmentCount { get; set; }

		[Static, Export ("pyramidWithWidth:height:length:")]
		SCNPyramid Create (nfloat width, nfloat height, nfloat length);
	}

	/// <summary>Renders an SCNScene into an OpenGL context.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNRenderer_Class/index.html">Apple documentation for <c>SCNRenderer</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[SCNRenderer init]: unrecognized selector sent to instance 0x7ce85a30
	interface SCNRenderer : SCNSceneRenderer, SCNTechniqueSupport {
		//		We already pull in the Scene property from the SCNSceneRenderer protocol, no need to redefine it here.
		//		[Export ("scene", ArgumentSemantic.Retain)]
		//		SCNScene Scene { get; set;  }

		// options: nothing today, it is reserved for future use
		[NoMacCatalyst]
		[Static, Export ("rendererWithContext:options:")]
		SCNRenderer FromContext (IntPtr context, [NullAllowed] NSDictionary options);

		[NoMacCatalyst]
		[Static]
		[Wrap ("FromContext (context.GetHandle (), options)")]
		// GetHandle will return IntPtr.Zero is context is null
		// GLContext == CGLContext on macOS and EAGLContext in iOS and tvOS (using on top of file)
		SCNRenderer FromContext (GLContext context, [NullAllowed] NSDictionary options);

		[NoTV]
		[Export ("render")]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void Render ();

		[MacCatalyst (13, 1)]
		[Export ("renderAtTime:")]
		void Render (double timeInSeconds);

		[MacCatalyst (13, 1)]
		[Export ("snapshotAtTime:withSize:antialiasingMode:")]
		NSImage GetSnapshot (double time, CGSize size, SCNAntialiasingMode antialiasingMode);

		[MacCatalyst (13, 1)]
		[Export ("nextFrameTime")]
		double NextFrameTimeInSeconds { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("rendererWithDevice:options:")]
		SCNRenderer FromDevice ([NullAllowed] IMTLDevice device, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Export ("renderAtTime:viewport:commandBuffer:passDescriptor:")]
		void Render (double timeInSeconds, CGRect viewport, IMTLCommandBuffer commandBuffer, MTLRenderPassDescriptor renderPassDescriptor);

		[MacCatalyst (13, 1)]
		[Export ("renderWithViewport:commandBuffer:passDescriptor:")]
		void Render (CGRect viewport, IMTLCommandBuffer commandBuffer, MTLRenderPassDescriptor renderPassDescriptor);

		[MacCatalyst (13, 1)]
		[Export ("updateProbes:atTime:")]
		void Update (SCNNode [] lightProbes, double time);

		[MacCatalyst (13, 1)]
		[Export ("updateAtTime:")]
		void Update (double time);

	}

	/// <summary>Defines keys for use with <see cref="M:SceneKit.SCNNodeRendererDelegate.Render(SceneKit.SCNNode,SceneKit.SCNRenderer,Foundation.NSDictionary)" /> and <see cref="M:SceneKit.SCNProgram.SetSemantic(Foundation.NSString,System.String,SceneKit.SCNProgramSemanticOptions)" />.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface SCNRenderingArguments {
		/// <summary>Represents the value associated with the constant SCNModelTransform</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("SCNModelTransform")]
		NSString ModelTransform { get; }

		[Field ("SCNViewTransform")]
		NSString ViewTransform { get; }

		[Field ("SCNProjectionTransform")]
		NSString ProjectionTransform { get; }

		[Field ("SCNNormalTransform")]
		NSString NormalTransform { get; }

		[Field ("SCNModelViewTransform")]
		NSString ModelViewTransform { get; }

		[Field ("SCNModelViewProjectionTransform")]
		NSString ModelViewProjectionTransform { get; }
	}

	/// <param name="totalProgress">A number in [0,1.0] that indicates the relative progress of the export operation.</param>
	///     <param name="error">An error that occurred.</param>
	///     <param name="stop">Developers set this to true to cancel processing.</param>
	///     <summary>Continuation handler that SceneKit repeatedly calls when exporting a scene.</summary>
	[MacCatalyst (13, 1)]
	delegate void SCNSceneExportProgressHandler (float /* float, not CGFloat */ totalProgress, NSError error, out bool stop);

	/// <summary>The highest-level description of a 3D scene.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNScene_Class/index.html">Apple documentation for <c>SCNScene</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNScene :
		GKSceneRootNodeType,
		NSSecureCoding {

		[Static]
		[Export ("scene")]
		SCNScene Create ();

		[Export ("rootNode")]
		SCNNode RootNode { get; }

		[Export ("attributeForKey:")]
		[return: NullAllowed]
		NSObject GetAttribute (NSString key);

		[Export ("setAttribute:forKey:")]
		void SetAttribute ([NullAllowed] NSObject attribute, NSString key);

		[Static]
		[Export ("sceneWithURL:options:error:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[return: NullAllowed]
		SCNScene FromUrl (NSUrl url, [NullAllowed] NSDictionary options, out NSError error);

		[Static]
		[Wrap ("FromUrl (url, options.GetDictionary (), out error)")]
		[return: NullAllowed]
		SCNScene FromUrl (NSUrl url, [NullAllowed] SCNSceneLoadingOptions options, out NSError error);



		[Field ("SCNSceneExportDestinationURL")]
		NSString ExportDestinationUrl { get; }

		[MacCatalyst (13, 1)]
		[Export ("physicsWorld")]
		SCNPhysicsWorld PhysicsWorld { get; }

		[MacCatalyst (13, 1)]
		[Export ("background")]
		SCNMaterialProperty Background { get; }

		[MacCatalyst (13, 1)]
		[Export ("lightingEnvironment")]
		SCNMaterialProperty LightingEnvironment { get; }

		[MacCatalyst (13, 1)]
		[Export ("fogStartDistance")]
		nfloat FogStartDistance { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("fogEndDistance")]
		nfloat FogEndDistance { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("fogDensityExponent")]
		nfloat FogDensityExponent { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("fogColor", ArgumentSemantic.Retain)]
		NSObject FogColor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; set; }

		[Static, Export ("sceneNamed:")]
		[return: NullAllowed]
		SCNScene FromFile (string name);

		[MacCatalyst (13, 1)]
		[Static, Export ("sceneNamed:inDirectory:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[return: NullAllowed]
		SCNScene FromFile (string name, [NullAllowed] string directory, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Static, Wrap ("FromFile (name, directory, options.GetDictionary ())")]
		[return: NullAllowed]
		SCNScene FromFile (string name, string directory, SCNSceneLoadingOptions options);

		// Keeping here the same name WriteToUrl for iOS and friends because it is how it was bound
		// initialy for macOS and having it named diferently would hurt shared code
		[MacCatalyst (13, 1)]
		[Export ("writeToURL:options:delegate:progressHandler:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		bool WriteToUrl (NSUrl url,
			[NullAllowed] NSDictionary options,
			[NullAllowed] ISCNSceneExportDelegate aDelegate,
			[NullAllowed] SCNSceneExportProgressHandler exportProgressHandler);

		[MacCatalyst (13, 1)]
		[Wrap ("WriteToUrl (url, options.GetDictionary (), handler, exportProgressHandler)")]
		bool WriteToUrl (NSUrl url, SCNSceneLoadingOptions options, ISCNSceneExportDelegate handler, SCNSceneExportProgressHandler exportProgressHandler);

		#region SCNParticleSystemSupport (SCNNode) category

		[MacCatalyst (13, 1)]
		[Export ("particleSystems")]
		[NullAllowed]
		SCNParticleSystem [] ParticleSystems { get; }

		[MacCatalyst (13, 1)]
		[Export ("addParticleSystem:withTransform:")]
		void AddParticleSystem (SCNParticleSystem system, SCNMatrix4 transform);

		[MacCatalyst (13, 1)]
		[Export ("removeAllParticleSystems")]
		void RemoveAllParticleSystems ();

		[MacCatalyst (13, 1)]
		[Export ("removeParticleSystem:")]
		void RemoveParticleSystem (SCNParticleSystem system);

		#endregion

		[Field ("SCNSceneStartTimeAttributeKey")]
		NSString StartTimeAttributeKey { get; }

		[Field ("SCNSceneEndTimeAttributeKey")]
		NSString EndTimeAttributeKey { get; }

		[Field ("SCNSceneFrameRateAttributeKey")]
		NSString FrameRateAttributeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNSceneUpAxisAttributeKey")]
		NSString UpAxisAttributeKey { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("sceneWithMDLAsset:")]
		SCNScene FromAsset (MDLAsset asset);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("wantsScreenSpaceReflection")]
		bool WantsScreenSpaceReflection { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("screenSpaceReflectionSampleCount")]
		nint ScreenSpaceReflectionSampleCount { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("screenSpaceReflectionMaximumDistance")]
		nfloat ScreenSpaceReflectionMaximumDistance { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("screenSpaceReflectionStride")]
		nfloat ScreenSpaceReflectionStride { get; set; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:SceneKit.SCNSceneExportDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:SceneKit.SCNSceneExportDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:SceneKit.SCNSceneExportDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:SceneKit.SCNSceneExportDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ISCNSceneExportDelegate { }

	/// <summary>Protocol that defines methods to handle exporting a scene to a file.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/SceneKit/SCNSceneExportDelegate">Apple documentation for <c>SCNSceneExportDelegate</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNSceneExportDelegate {

		[Export ("writeImage:withSceneDocumentURL:originalImageURL:")]
		[return: NullAllowed]
		NSUrl WriteImage (NSImage image, NSUrl documentUrl, [NullAllowed] NSUrl originalImageUrl);
	}

	/// <summary>Reads scene-defining data from a file or NSData object.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNSceneSource_Class/index.html">Apple documentation for <c>SCNSceneSource</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNSceneSource {
		[Export ("url")]
		[NullAllowed]
		NSUrl Url { get; }

		[Export ("data")]
		[NullAllowed]
		NSData Data { get; }

		[Static, Export ("sceneSourceWithURL:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[return: NullAllowed]
		SCNSceneSource FromUrl (NSUrl url, [NullAllowed] NSDictionary options);

		[Wrap ("FromUrl (url, options.GetDictionary ())")]
		[return: NullAllowed]
		SCNSceneSource FromUrl (NSUrl url, SCNSceneLoadingOptions options);

		[Static]
		[Export ("sceneSourceWithData:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[return: NullAllowed]
		SCNSceneSource FromData (NSData data, [NullAllowed] NSDictionary options);

		[Static]
		[Wrap ("FromData (data, options.GetDictionary ())")]
		[return: NullAllowed]
		SCNSceneSource FromData (NSData data, SCNSceneLoadingOptions options);

		[Export ("initWithURL:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NativeHandle Constructor (NSUrl url, [NullAllowed] NSDictionary options);

		[Wrap ("this (url, options.GetDictionary ())")]
		NativeHandle Constructor (NSUrl url, SCNSceneLoadingOptions options);

		[Export ("initWithData:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NativeHandle Constructor (NSData data, [NullAllowed] NSDictionary options);

		[Wrap ("this (data, options.GetDictionary ())")]
		NativeHandle Constructor (NSData data, SCNSceneLoadingOptions options);

		[Export ("sceneWithOptions:statusHandler:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[return: NullAllowed]
		SCNScene SceneFromOptions ([NullAllowed] NSDictionary options, [NullAllowed] SCNSceneSourceStatusHandler statusHandler);

		[Wrap ("SceneFromOptions (options?.GetDictionary (), statusHandler)")]
		[return: NullAllowed]
		SCNScene SceneFromOptions ([NullAllowed] SCNSceneLoadingOptions options, [NullAllowed] SCNSceneSourceStatusHandler statusHandler);

		[Export ("sceneWithOptions:error:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[return: NullAllowed]
		SCNScene SceneWithOption ([NullAllowed] NSDictionary options, out NSError error);

		[Wrap ("SceneWithOption (options?.GetDictionary (), out error)")]
		[return: NullAllowed]
		SCNScene SceneWithOption (SCNSceneLoadingOptions options, out NSError error);

		[Export ("propertyForKey:")]
		[return: NullAllowed]
		NSObject GetProperty (NSString key);

		[Export ("entryWithIdentifier:withClass:")]
		[return: NullAllowed]
		NSObject GetEntryWithIdentifier (string uid, Class entryClass);

		[Export ("identifiersOfEntriesWithClass:")]
		string [] GetIdentifiersOfEntries (Class entryClass);

		[MacCatalyst (13, 1)]
		[Export ("entriesPassingTest:")]
		NSObject [] EntriesPassingTest (SCNSceneSourceFilter predicate);
	}
	/// <summary>Delegate that tests objects in the scene for inclusion.</summary>
	delegate bool SCNSceneSourceFilter (NSObject entry, NSString identifier, ref bool stop);

	/// <summary>Options for creating and loading SCNScene objects.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface SCNSceneSourceLoading {
		[Field ("SCNSceneSourceAssetDirectoryURLsKey")]
		NSString AssetDirectoryUrlsKey { get; }

		[Field ("SCNSceneSourceCreateNormalsIfAbsentKey")]
		NSString CreateNormalsIfAbsentKey { get; }

		[Field ("SCNSceneSourceFlattenSceneKey")]
		NSString FlattenSceneKey { get; }

		[Field ("SCNSceneSourceCheckConsistencyKey")]
		NSString CheckConsistencyKey { get; }

		[Field ("SCNSceneSourceOverrideAssetURLsKey")]
		NSString OverrideAssetUrlsKey { get; }

		[Field ("SCNSceneSourceStrictConformanceKey")]
		NSString StrictConformanceKey { get; }

		[Deprecated (PlatformName.TvOS, 11, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("SCNSceneSourceUseSafeModeKey")]
		NSString UseSafeModeKey { get; }

		// header said NA and docs says "Available in iOS 8.0 through iOS 8.2." but it's back on iOS11
		[MacCatalyst (13, 1)]
		[Field ("SCNSceneSourceConvertUnitsToMetersKey")]
		NSString ConvertUnitsToMetersKey { get; }

		// header said NA and docs says "Available in iOS 8.0 through iOS 8.2." but it's back on iOS11
		[MacCatalyst (13, 1)]
		[Field ("SCNSceneSourceConvertToYUpKey")]
		NSString ConvertToYUpKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNSceneSourceAnimationImportPolicyKey")]
		NSString AnimationImportPolicyKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNSceneSourceAnimationImportPolicyPlay")]
		NSString AnimationImportPolicyPlay { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNSceneSourceAnimationImportPolicyPlayRepeatedly")]
		NSString AnimationImportPolicyPlayRepeatedly { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNSceneSourceAnimationImportPolicyDoNotPlay")]
		NSString AnimationImportPolicyDoNotPlay { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNSceneSourceAnimationImportPolicyPlayUsingSceneTimeBase")]
		NSString AnimationImportPolicyPlayUsingSceneTimeBase { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNSceneSourceLoadingOptionPreserveOriginalTopology")]
		NSString OptionPreserveOriginalTopology { get; }
	}

	/// <summary>Enumerates error types that can occur while loading <see cref="T:SceneKit.SCNSceneSource" />s.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface SCNSceneSourceLoadErrors {
		[Field ("SCNConsistencyElementIDErrorKey")]
		NSString ConsistencyElementIDErrorKey { get; }

		[Field ("SCNConsistencyElementTypeErrorKey")]
		NSString ConsistencyElementTypeErrorKey { get; }

		[Field ("SCNConsistencyLineNumberErrorKey")]
		NSString ConsistencyLineNumberErrorKey { get; }

		[Field ("SCNDetailedErrorsKey")]
		NSString DetailedErrorsKey { get; }
	}

	/// <summary>Metadata keys associated with a scene file. Used with <see cref="M:SceneKit.SCNSceneSource.GetProperty(Foundation.NSString)" />.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface SCNSceneSourceProperties {
		[Field ("SCNSceneSourceAssetContributorsKey")]
		NSString AssetContributorsKey { get; }

		[Field ("SCNSceneSourceAssetCreatedDateKey")]
		NSString AssetCreatedDateKey { get; }

		[Field ("SCNSceneSourceAssetModifiedDateKey")]
		NSString AssetModifiedDateKey { get; }

		[Field ("SCNSceneSourceAssetUpAxisKey")]
		NSString AssetUpAxisKey { get; }

		[Field ("SCNSceneSourceAssetUnitKey")]
		NSString AssetUnitKey { get; }

		[Field ("SCNSceneSourceAssetAuthoringToolKey")]
		NSString AssetAuthoringToolKey { get; }

		[Field ("SCNSceneSourceAssetAuthorKey")]
		NSString AssetAuthorKey { get; }

		[Field ("SCNSceneSourceAssetUnitNameKey")]
		NSString AssetUnitNameKey { get; }

		[Field ("SCNSceneSourceAssetUnitMeterKey")]
		NSString AssetUnitMeterKey { get; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:SceneKit.SCNSceneRenderer" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:SceneKit.SCNSceneRenderer" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:SceneKit.SCNSceneRenderer" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:SceneKit.SCNSceneRenderer_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ISCNSceneRenderer { }

	/// <summary>Implementation class of the <see cref="T:SceneKit.ISCNSceneRenderer" /> interface.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNSceneRenderer_Protocol/index.html">Apple documentation for <c>SCNSceneRenderer</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNSceneRenderer {
		[Abstract]
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakSceneRendererDelegate { get; set; }

		[Wrap ("WeakSceneRendererDelegate")]
		ISCNSceneRendererDelegate SceneRendererDelegate { get; set; }

		[Abstract]
		[Export ("playing")]
		bool Playing { [Bind ("isPlaying")] get; set; }

		[Abstract]
		[Export ("loops")]
		bool Loops { get; set; }

		[Abstract]
		[Export ("pointOfView", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNNode PointOfView { get; set; }

		[Abstract]
		[Export ("autoenablesDefaultLighting")]
		bool AutoenablesDefaultLighting { get; set; }

		[Abstract]
		[Export ("jitteringEnabled")]
		bool JitteringEnabled { [Bind ("isJitteringEnabled")] get; set; }

		[Abstract]
		[NoMacCatalyst]
		[Export ("context")]
		IntPtr Context { get; }

#if NET
		[Abstract]
#endif
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[NoiOS]
		[Export ("currentTime")]
		double CurrentTime { get; set; }

		[Abstract]
		[Export ("hitTest:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNHitTestResult [] HitTest (CGPoint thePoint, [NullAllowed] NSDictionary options);

		[Wrap ("HitTest (thePoint, options.GetDictionary ())")]
		SCNHitTestResult [] HitTest (CGPoint thePoint, SCNHitTestOptions options);

		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("showsStatistics")]
		bool ShowsStatistics { get; set; }

		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("sceneTime")]
		double SceneTimeInSeconds { get; set; }

		[Abstract]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("scene", ArgumentSemantic.Retain)]
		SCNScene Scene { get; set; }

		[Abstract]
		// It seems swift has this property listed as an optional[0] and an Apple sample[1] sets this to null
		// [0]: https://developer.apple.com/documentation/scenekit/scnscenerenderer/1524051-overlayskscene
		// [1]: https://github.com/xamarin/xamarin-macios/issues/3392
		[NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("overlaySKScene", ArgumentSemantic.Retain)]
		SKScene OverlayScene { get; set; }

		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("isNodeInsideFrustum:withPointOfView:")]
		bool IsNodeInsideFrustum (SCNNode node, SCNNode pointOfView);

		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("projectPoint:")]
		SCNVector3 ProjectPoint (SCNVector3 point);

		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("unprojectPoint:")]
		SCNVector3 UnprojectPoint (SCNVector3 point);

		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("prepareObject:shouldAbortBlock:")]
		bool Prepare (NSObject obj, [NullAllowed] Func<bool> abortHandler);

		[Abstract]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("prepareObjects:withCompletionHandler:")]
		void Prepare (NSObject [] objects, [NullAllowed] Action<bool> completionHandler);

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("presentScene:withTransition:incomingPointOfView:completionHandler:")]
		void PresentScene (SCNScene scene, SKTransition transition, [NullAllowed] SCNNode pointOfView, [NullAllowed] Action completionHandler);

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[Export ("nodesInsideFrustumWithPointOfView:")]
		SCNNode [] GetNodesInsideFrustum (SCNNode pointOfView);

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[Export ("debugOptions", ArgumentSemantic.Assign)]
		SCNDebugOptions DebugOptions { get; set; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[Export ("renderingAPI")]
		SCNRenderingApi RenderingApi { get; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("currentRenderCommandEncoder")]
		IMTLRenderCommandEncoder CurrentRenderCommandEncoder { get; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("device")]
		IMTLDevice Device { get; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[Export ("colorPixelFormat")]
		MTLPixelFormat ColorPixelFormat { get; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[Export ("depthPixelFormat")]
		MTLPixelFormat DepthPixelFormat { get; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[Export ("stencilPixelFormat")]
		MTLPixelFormat StencilPixelFormat { get; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("commandQueue")]
		IMTLCommandQueue CommandQueue { get; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[Export ("audioEngine")]
		AVAudioEngine AudioEngine { get; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[Export ("audioEnvironmentNode")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		AVAudioEnvironmentNode AudioEnvironmentNode { get; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("audioListener", ArgumentSemantic.Retain)]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		SCNNode AudioListener { get; set; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("temporalAntialiasingEnabled")]
		bool TemporalAntialiasingEnabled { [Bind ("isTemporalAntialiasingEnabled")] get; set; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("currentViewport")]
		CGRect CurrentViewport { get; }

#if NET
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("usesReverseZ")]
		bool UsesReverseZ { get; set; }

		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("currentRenderPassDescriptor")]
		MTLRenderPassDescriptor CurrentRenderPassDescriptor { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("workingColorSpace")]
		CGColorSpace WorkingColorSpace { get; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:SceneKit.SCNSceneRendererDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:SceneKit.SCNSceneRendererDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:SceneKit.SCNSceneRendererDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:SceneKit.SCNSceneRendererDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ISCNSceneRendererDelegate { }

	/// <summary>Delegate object for the SCNRenderer class, defines methods reflects rendering events.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNSceneRendererDelegate_Protocol/index.html">Apple documentation for <c>SCNSceneRendererDelegate</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNSceneRendererDelegate {

		[Export ("renderer:willRenderScene:atTime:")]
		void WillRenderScene (ISCNSceneRenderer renderer, SCNScene scene, double timeInSeconds);

		[Export ("renderer:didRenderScene:atTime:")]
		void DidRenderScene (ISCNSceneRenderer renderer, SCNScene scene, double timeInSeconds);

		[MacCatalyst (13, 1)]
		[Export ("renderer:updateAtTime:")]
		void Update (ISCNSceneRenderer renderer, double timeInSeconds);

		[MacCatalyst (13, 1)]
		[Export ("renderer:didApplyAnimationsAtTime:")]
		void DidApplyAnimations (ISCNSceneRenderer renderer, double timeInSeconds);

		[MacCatalyst (13, 1)]
		[Export ("renderer:didSimulatePhysicsAtTime:")]
		void DidSimulatePhysics (ISCNSceneRenderer renderer, double timeInSeconds);

		[MacCatalyst (13, 1)]
		[Export ("renderer:didApplyConstraintsAtTime:")]
		void DidApplyConstraints (ISCNSceneRenderer renderer, double atTime);

	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> that is a sphere.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNSphere_Class/index.html">Apple documentation for <c>SCNSphere</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	[DisableDefaultCtor]
	interface SCNSphere {
		[Export ("radius")]
		nfloat Radius { get; set; }

		[Export ("geodesic")]
		bool Geodesic { [Bind ("isGeodesic")] get; set; }

		[Export ("segmentCount")]
		nint SegmentCount { get; set; }

		[Static, Export ("sphereWithRadius:")]
		SCNSphere Create (nfloat radius);

	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> based on a string, optionally extruded into three dimensions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNText_Class/index.html">Apple documentation for <c>SCNText</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	[DisableDefaultCtor]
	interface SCNText {
		[Export ("extrusionDepth")]
		nfloat ExtrusionDepth { get; set; }

		[Export ("string", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSObject String { get; set; }

		[Export ("containerFrame")]
		CGRect ContainerFrame { get; set; }

		// removed in iOS8 beta 5 - but it was already existing in 10.8 ?
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("textSize")]
		CGSize TextSize { get; }

		[Export ("truncationMode", ArgumentSemantic.Copy)]
		string TruncationMode { get; set; }

		[Export ("alignmentMode", ArgumentSemantic.Copy)]
		string AlignmentMode { get; set; }

		[Export ("chamferRadius")]
		nfloat ChamferRadius { get; set; }

		[Static, Export ("textWithString:extrusionDepth:")]
		SCNText Create ([NullAllowed] NSObject str, nfloat extrusionDepth);

		[Export ("font", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSFont Font { get; set; }

		[Export ("wrapped")]
		bool Wrapped { [Bind ("isWrapped")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("chamferProfile", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSBezierPath ChamferProfile { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("flatness")]
		nfloat Flatness { get; set; }
	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> that is a ring-shaped solid.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNTorus_Class/index.html">Apple documentation for <c>SCNTorus</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	[DisableDefaultCtor]
	interface SCNTorus {
		[Export ("ringRadius")]
		nfloat RingRadius { get; set; }

		[Export ("pipeRadius")]
		nfloat PipeRadius { get; set; }

		[Export ("ringSegmentCount")]
		nint RingSegmentCount { get; set; }

		[Export ("pipeSegmentCount")]
		nint PipeSegmentCount { get; set; }

		[Static, Export ("torusWithRingRadius:pipeRadius:")]
		SCNTorus Create (nfloat ringRadius, nfloat pipeRadius);
	}

	/// <summary>Allows a set of modifications to be applied atomically.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNTransaction_Class/index.html">Apple documentation for <c>SCNTransaction</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNTransaction {
		[Static]
		[Export ("begin")]
		void Begin ();

		[Static]
		[Export ("commit")]
		void Commit ();

		[Static]
		[Export ("flush")]
		void Flush ();

		[Static]
		[Export ("lock")]
		void Lock ();

		[Static]
		[Export ("unlock")]
		void Unlock ();

		[Static]
		[Export ("setCompletionBlock:")]
		void SetCompletionBlock ([NullAllowed] Action completion);

		[Export ("valueForKey:")]
		NSObject ValueForKey (NSString key);

		[Static]
		[Export ("setValue:forKey:")]
		void SetValueForKey ([NullAllowed] NSObject value, NSString key);

		//Detected properties
		[Static]
		[Export ("animationDuration")]
		double AnimationDuration { get; set; }

		[Static]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("animationTimingFunction")]
		CAMediaTimingFunction AnimationTimingFunction { get; set; }

		[Static]
		[Export ("disableActions")]
		bool DisableActions { get; set; }
	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> that is a cylinder with a circular hole along its central axis.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNTube_Class/index.html">Apple documentation for <c>SCNTube</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNTube {
		[Export ("innerRadius")]
		nfloat InnerRadius { get; set; }

		[Export ("outerRadius")]
		nfloat OuterRadius { get; set; }

		[Export ("height")]
		nfloat Height { get; set; }

		[Export ("radialSegmentCount")]
		nint RadialSegmentCount { get; set; }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set; }

		[Static, Export ("tubeWithInnerRadius:outerRadius:height:")]
		SCNTube Create (nfloat innerRadius, nfloat outerRadius, nfloat height);
	}

	[MacCatalyst (13, 1)]
	[Static]
	[Internal] // we'll make it public if there's a need for them (beside the strong dictionary we provide)
	interface SCNRenderingOptionsKeys {

		[NoMacCatalyst]
		[Field ("SCNPreferredRenderingAPIKey")]
		NSString RenderingApiKey { get; }

		[Field ("SCNPreferredDeviceKey")]
		NSString DeviceKey { get; }

		[Field ("SCNPreferLowPowerDeviceKey")]
		NSString LowPowerDeviceKey { get; }
	}

	/// <summary>A <see cref="T:Foundation.DictionaryContainer" /> that defines <c>options</c> when instantiating a <see cref="T:SceneKit.SCNView" />.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("SCNRenderingOptionsKeys")]
	interface SCNRenderingOptions {
		IMTLDevice Device { get; set; }

		bool LowPowerDevice { get; set; }
	}

	/// <summary>A <see cref="T:UIKit.UIView" /> that renders <see cref="T:SceneKit.SCNScene" />s.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNView_Class/index.html">Apple documentation for <c>SCNView</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSView))]
	[DisableDefaultCtor]
	interface SCNView : SCNSceneRenderer, SCNTechniqueSupport {
		//		We already pull in the Scene property from the SCNSceneRenderer protocol, no need to redefine it here.
		//		[Export ("scene", ArgumentSemantic.Retain)]
		//		SCNScene Scene { get; set;  }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[Export ("allowsCameraControl")]
		bool AllowsCameraControl { get; set; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Please use Metal instead of OpenGL API.")]
		[Export ("openGLContext", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSOpenGLContext OpenGLContext { get; set; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Please use Metal instead of OpenGL API.")]
		[Export ("pixelFormat", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSOpenGLPixelFormat PixelFormat { get; set; }

		[NoMac]
		[NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Please use Metal instead of OpenGL API.")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Please use Metal instead of OpenGL API.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use Metal instead of OpenGL API.")]
		[Export ("eaglContext", ArgumentSemantic.Retain)]
		[NullAllowed]
		EAGLContext EAGLContext { get; set; }

		[MacCatalyst (13, 1)]
		[Wrap ("this (frame, options.GetDictionary ())")]
		NativeHandle Constructor (CGRect frame, [NullAllowed] SCNRenderingOptions options);

		[Export ("initWithFrame:options:")]
		NativeHandle Constructor (CGRect frame, [NullAllowed] NSDictionary options);

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("play:")]
		void Play ([NullAllowed] NSObject sender);

		[Export ("pause:")]
		void Pause ([NullAllowed] NSObject sender);

		[Export ("stop:")]
		void Stop ([NullAllowed] NSObject sender);

		[MacCatalyst (13, 1)]
		[Export ("snapshot")]
		NSImage Snapshot ();

		[MacCatalyst (13, 1)]
		[Export ("preferredFramesPerSecond")]
		nint PreferredFramesPerSecond { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("antialiasingMode")]
		SCNAntialiasingMode AntialiasingMode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("cameraControlConfiguration")]
		ISCNCameraControlConfiguration CameraControlConfiguration { get; }

		[MacCatalyst (13, 1)]
		[Export ("defaultCameraController")]
		SCNCameraController DefaultCameraController { get; }

		[MacCatalyst (13, 1)]
		[Export ("rendersContinuously")]
		bool RendersContinuously { get; set; }

		[NoTV, NoiOS]
		[NoMacCatalyst]
		[Export ("drawableResizesAsynchronously")]
		bool DrawableResizesAsynchronously { get; set; }
	}

#if NET
	/// <summary>Completion handler for use with <see cref="M:SceneKit.SCNAnimationEvent.Create(System.nfloat,SceneKit.SCNAnimationEventHandler)" />.</summary>
	[MacCatalyst (13, 1)]
	delegate void SCNAnimationEventHandler (AnimationType animation, NSObject animatedObject, bool playingBackward);
#endif

	/// <summary>Performs a function at a specific time during an animation.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNAnimationEvent_Class/index.html">Apple documentation for <c>SCNAnimationEvent</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNAnimationEvent {

#if NET
		[Static, Export ("animationEventWithKeyTime:block:")]
		SCNAnimationEvent Create (nfloat keyTime, SCNAnimationEventHandler eventHandler);
#else
		[Internal]
		[Static, Export ("animationEventWithKeyTime:block:")]
		SCNAnimationEvent Create (nfloat keyTime, Action<IntPtr, NSObject, bool> handler);
#endif
	}

	/// <summary>An <see cref="T:SceneKit.SCNGeometry" /> created from a 2D path, optionally extruded into three dimensions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNShape_Class/index.html">Apple documentation for <c>SCNShape</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNGeometry))]
	partial interface SCNShape {
		[NullAllowed] // by default this property is null
		[Export ("path", ArgumentSemantic.Copy)]
		NSBezierPath Path { get; set; }

		[Export ("extrusionDepth")]
		nfloat ExtrusionDepth { get; set; }

		[Export ("chamferMode")]
		SCNChamferMode ChamferMode { get; set; }

		[Export ("chamferRadius")]
		nfloat ChamferRadius { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("chamferProfile", ArgumentSemantic.Copy)]
		NSBezierPath ChamferProfile { get; set; }

		[Static, Export ("shapeWithPath:extrusionDepth:")]
		SCNShape Create ([NullAllowed] NSBezierPath path, nfloat extrusionDepth);
	}

	/// <summary>An animated transformation of an SCNGeometry into one or more target SCNGeometry objects.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNMorpher_Class/index.html">Apple documentation for <c>SCNMorpher</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNMorpher : SCNAnimatable, NSSecureCoding {

		[Export ("targets", ArgumentSemantic.Copy)]
		SCNGeometry [] Targets { get; set; }

		[Export ("calculationMode")]
		SCNMorpherCalculationMode CalculationMode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("weights", ArgumentSemantic.Retain)]
		NSNumber [] Weights { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("unifiesNormals")]
		bool UnifiesNormals { get; set; }

		[Export ("setWeight:forTargetAtIndex:")]
		void SetWeight (nfloat weight, nuint targetIndex);

		[MacCatalyst (13, 1)]
		[Export ("setWeight:forTargetNamed:")]
		void SetWeight (nfloat weight, string targetName);

		[Export ("weightForTargetAtIndex:")]
		nfloat GetWeight (nuint targetIndex);

		[MacCatalyst (13, 1)]
		[Export ("weightForTargetNamed:")]
		nfloat GetWeight (string targetName);
	}

	/// <summary>Accesses the scene graph in support of skeletal animation.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNSkinner_Class/index.html">Apple documentation for <c>SCNSkinner</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNSkinner : NSSecureCoding {
		[Export ("skeleton", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNNode Skeleton { get; set; }

		[Export ("baseGeometry", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNGeometry BaseGeometry { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("baseGeometryBindTransform")]
		SCNMatrix4 BaseGeometryBindTransform { get; set; }

		[MacCatalyst (13, 1)]
		[Internal, Export ("boneInverseBindTransforms")]
		[NullAllowed]
		NSArray _BoneInverseBindTransforms { get; }

		[MacCatalyst (13, 1)]
		[Export ("bones")]
		SCNNode [] Bones { get; }

		[MacCatalyst (13, 1)]
		[Export ("boneWeights")]
		SCNGeometrySource BoneWeights { get; }

		[MacCatalyst (13, 1)]
		[Export ("boneIndices")]
		SCNGeometrySource BoneIndices { get; }

		[MacCatalyst (13, 1)]
		[Static, Internal, Export ("skinnerWithBaseGeometry:bones:boneInverseBindTransforms:boneWeights:boneIndices:")]
		SCNSkinner _Create ([NullAllowed] SCNGeometry baseGeometry, SCNNode [] bones, [NullAllowed] NSArray boneInverseBindTransforms,
			SCNGeometrySource boneWeights, SCNGeometrySource boneIndices);
	}

	/// <summary>Automatically adjusts the transformation of a node.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNConstraint_Class/index.html">Apple documentation for <c>SCNConstraint</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[DisableDefaultCtor]
	interface SCNConstraint : SCNAnimatable, NSCopying, NSSecureCoding {
		[MacCatalyst (13, 1)]
		[Export ("influenceFactor")]
		nfloat InfluenceFactor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("incremental")]
		bool Incremental { [Bind ("isIncremental")] get; set; }
	}

	/// <summary>An <see cref="T:SceneKit.SCNConstraint" /> that applies inverse kinematics.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNIKConstraint_Class/index.html">Apple documentation for <c>SCNIKConstraint</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNConstraint))]
	[DisableDefaultCtor]
	interface SCNIKConstraint {

		[Export ("chainRootNode")]
		SCNNode ChainRootNode { get; }

		[Export ("targetPosition")]
		SCNVector3 TargetPosition { get; set; }

		[Static, Export ("inverseKinematicsConstraintWithChainRootNode:")]
		SCNIKConstraint Create (SCNNode chainRootNode);

		[Export ("setMaxAllowedRotationAngle:forJoint:")]
		void SetMaxAllowedRotationAnglet (nfloat angle, SCNNode node);

		[Export ("maxAllowedRotationAngleForJoint:")]
		nfloat GetMaxAllowedRotationAngle (SCNNode node);

		[MacCatalyst (13, 1)]
		[Export ("initWithChainRootNode:")]
		NativeHandle Constructor (SCNNode chainRootNode);

	}

	/// <summary>An <see cref="T:SceneKit.SCNConstraint" /> that orients a node to point towards another node.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNLookAtConstraint_Class/index.html">Apple documentation for <c>SCNLookAtConstraint</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNConstraint))]
	[DisableDefaultCtor]
	interface SCNLookAtConstraint {
		[Export ("target", ArgumentSemantic.Retain), NullAllowed]
		SCNNode Target {
			get; [MacCatalyst (13, 1)]
			set;
		}

		[Export ("gimbalLockEnabled")]
		bool GimbalLockEnabled { get; set; }

		[Static, Export ("lookAtConstraintWithTarget:")]
		SCNLookAtConstraint Create ([NullAllowed] SCNNode target);

		[MacCatalyst (13, 1)]
		[Export ("targetOffset", ArgumentSemantic.Assign)]
		SCNVector3 TargetOffset { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("localFront", ArgumentSemantic.Assign)]
		SCNVector3 LocalFront { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("worldUp", ArgumentSemantic.Assign)]
		SCNVector3 WorldUp { get; set; }
	}

	/// <summary>Delegate used in calls to <see cref="M:SceneKit.SCNTransformConstraint.Create(System.Boolean,SceneKit.SCNTransformConstraintHandler)" />.</summary>
	[MacCatalyst (13, 1)]
	delegate SCNMatrix4 SCNTransformConstraintHandler (SCNNode node, SCNMatrix4 transform);

	/// <summary>An <see cref="T:SceneKit.SCNConstraint" /> that executes a function specifying a change in transformation (position, rotation, and scale) to a node or nodes.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNTransformConstraint_Class/index.html">Apple documentation for <c>SCNTransformConstraint</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNConstraint))]
	[DisableDefaultCtor]
	interface SCNTransformConstraint {
		[Static, Export ("transformConstraintInWorldSpace:withBlock:")]
		SCNTransformConstraint Create (bool inWorldSpace, SCNTransformConstraintHandler transformHandler);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("positionConstraintInWorldSpace:withBlock:")]
		SCNTransformConstraint CreatePositionConstraint (bool inWorldSpace, Func<SCNNode, SCNVector3, SCNVector3> transformHandler);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("orientationConstraintInWorldSpace:withBlock:")]
		SCNTransformConstraint CreateOrientationConstraint (bool inWorldSpace, Func<SCNNode, SCNQuaternion, SCNQuaternion> transformHandler);

	}

	/// <summary>Controls alternate geometries rendered at thresholds in either world-space or screen-space.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNLevelOfDetail_Class/index.html">Apple documentation for <c>SCNLevelOfDetail</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNLevelOfDetail : NSCopying, NSSecureCoding {
		[Export ("geometry")]
		[NullAllowed]
		SCNGeometry Geometry { get; }

		[Export ("screenSpaceRadius")]
		nfloat ScreenSpaceRadius { get; }

		[Export ("worldSpaceDistance")]
		nfloat WorldSpaceDistance { get; }

		[Static, Export ("levelOfDetailWithGeometry:screenSpaceRadius:")]
		SCNLevelOfDetail CreateWithScreenSpaceRadius ([NullAllowed] SCNGeometry geometry, nfloat screenSpaceRadius);

		[Static, Export ("levelOfDetailWithGeometry:worldSpaceDistance:")]
		SCNLevelOfDetail CreateWithWorldSpaceDistance ([NullAllowed] SCNGeometry geometry, nfloat worldSpaceDistance);
	}

	/// <summary>A dictionary of shader snippets.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface _SCNShaderModifiers {
		[Internal, Field ("SCNShaderModifierEntryPointGeometry")]
		NSString EntryPointGeometryKey { get; }

		[Internal, Field ("SCNShaderModifierEntryPointSurface")]
		NSString EntryPointSurfaceKey { get; }

		[Internal, Field ("SCNShaderModifierEntryPointLightingModel")]
		NSString EntryPointLightingModelKey { get; }

		[Internal, Field ("SCNShaderModifierEntryPointFragment")]
		NSString EntryPointFragmentKey { get; }
	}


	/// <summary>Allows actions to be applied.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNActionable_Protocol/index.html">Apple documentation for <c>SCNActionable</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNActionable {
		[Abstract]
		[Export ("runAction:")]
		void RunAction (SCNAction action);

		[Abstract]
		[Export ("runAction:completionHandler:")]
		void RunAction (SCNAction action, [NullAllowed] Action block);

		[Abstract]
		[Export ("runAction:forKey:")]
		void RunAction (SCNAction action, [NullAllowed] string key);

		[Abstract]
		[Export ("runAction:forKey:completionHandler:")]
		void RunAction (SCNAction action, [NullAllowed] string key, [NullAllowed] Action block);

		[Abstract]
		[Export ("hasActions")]
		bool HasActions ();

		[Abstract]
		[Export ("actionForKey:")]
		[return: NullAllowed]
		SCNAction GetAction (string key);

		[Abstract]
		[Export ("removeActionForKey:")]
		void RemoveAction (string key);

		[Abstract]
		[Export ("removeAllActions")]
		void RemoveAllActions ();

#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("actionKeys")]
		string [] ActionKeys { get; }
	}

	/// <summary>Completion handler used with <see cref="M:SceneKit.SCNAction.CustomAction(System.Double,SceneKit.SCNActionNodeWithElapsedTimeHandler)" />.</summary>
	[MacCatalyst (13, 1)]
	delegate void SCNActionNodeWithElapsedTimeHandler (SCNNode node, nfloat elapsedTime);

	/// <summary>Performs actions on a <see cref="T:SceneKit.SCNNode" />, including changes to appearance, geometry, animations, or arbitrary functions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNAction_Class/index.html">Apple documentation for <c>SCNAction</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNAction : NSCopying, NSSecureCoding {

		[Export ("duration")]
		double DurationInSeconds { get; set; }

		[Export ("timingMode")]
		SCNActionTimingMode TimingMode { get; set; }

		[NullAllowed, Export ("timingFunction", ArgumentSemantic.Assign)]
#if NET
		Func<float, float> TimingFunction { get; set; }
#else
		Func<float, float> TimingFunction2 { get; set; }
#endif

		[Export ("speed")]
		nfloat Speed { get; set; }

		[Export ("reversedAction")]
		SCNAction ReversedAction ();

		[Static, Export ("moveByX:y:z:duration:")]
		SCNAction MoveBy (nfloat deltaX, nfloat deltaY, nfloat deltaZ, double durationInSeconds);

		[Static, Export ("moveBy:duration:")]
		SCNAction MoveBy (SCNVector3 delta, double durationInSeconds);

		[Static, Export ("moveTo:duration:")]
		SCNAction MoveTo (SCNVector3 location, double durationInSeconds);

		[Static, Export ("rotateByX:y:z:duration:")]
		SCNAction RotateBy (nfloat xAngle, nfloat yAngle, nfloat zAngle, double durationInSeconds);

		[Static, Export ("rotateByAngle:aroundAxis:duration:")]
		SCNAction RotateBy (nfloat angle, SCNVector3 axis, double durationInSeconds);

		[Static, Export ("rotateToX:y:z:duration:")]
		SCNAction RotateTo (nfloat xAngle, nfloat yAngle, nfloat zAngle, double durationInSeconds);

		[Static, Export ("rotateToX:y:z:duration:shortestUnitArc:")]
		SCNAction RotateTo (nfloat xAngle, nfloat yAngle, nfloat zAngle, double durationInSeconds, bool shortestUnitArc);

		[Static, Export ("rotateToAxisAngle:duration:")]
		SCNAction RotateTo (SCNVector4 axisAngle, double durationInSeconds);

		[Static, Export ("scaleBy:duration:")]
		SCNAction ScaleBy (nfloat scale, double durationInSeconds);

		[Static, Export ("scaleTo:duration:")]
		SCNAction ScaleTo (nfloat scale, double durationInSeconds);

		[Static, Export ("sequence:")]
		SCNAction Sequence (SCNAction [] actions);

		[Static, Export ("group:")]
		SCNAction Group (SCNAction [] actions);

		[Static, Export ("repeatAction:count:")]
		SCNAction RepeatAction (SCNAction action, nuint count);

		[Static, Export ("repeatActionForever:")]
		SCNAction RepeatActionForever (SCNAction action);

		[Static, Export ("fadeInWithDuration:")]
		SCNAction FadeIn (double durationInSeconds);

		[Static, Export ("fadeOutWithDuration:")]
		SCNAction FadeOut (double durationInSeconds);

		[Static, Export ("fadeOpacityBy:duration:")]
		SCNAction FadeOpacityBy (nfloat factor, double durationInSeconds);

		[Static, Export ("fadeOpacityTo:duration:")]
		SCNAction FadeOpacityTo (nfloat opacity, double durationInSeconds);

		[Static, Export ("waitForDuration:")]
		SCNAction Wait (double durationInSeconds);

		[Static, Export ("waitForDuration:withRange:")]
		SCNAction Wait (double durationInSeconds, double durationRange);

		[Static, Export ("removeFromParentNode")]
		SCNAction RemoveFromParentNode ();

		[Static, Export ("runBlock:")]
		SCNAction Run (Action<SCNNode> handler);

		[Static, Export ("runBlock:queue:")]
		SCNAction Run (Action<SCNNode> handler, DispatchQueue queue);

		[Static, Export ("javaScriptActionWithScript:duration:")]
		SCNAction FromJavascript (string script, double seconds);

		[Static, Export ("customActionWithDuration:actionBlock:")]
		SCNAction CustomAction (double seconds, SCNActionNodeWithElapsedTimeHandler handler);

		[MacCatalyst (13, 1)]
		[Static, Export ("hide")]
		SCNAction Hide ();

		[MacCatalyst (13, 1)]
		[Static, Export ("unhide")]
		SCNAction Unhide ();

		[MacCatalyst (13, 1)]
		[Static, Export ("playAudioSource:waitForCompletion:")]
		SCNAction PlayAudioSource (SCNAudioSource source, bool wait);
	}

	/// <summary>Completion handler used with <see cref="M:SceneKit.SCNShadable.HandleBinding(System.String,SceneKit.SCNBindingHandler)" />.</summary>
	[MacCatalyst (13, 1)]
	delegate void SCNBindingHandler (uint /* unsigned int */ programId, uint /* unsigned int */ location, SCNNode renderedNode, SCNRenderer renderer);

	/// <summary>A <see cref="T:Foundation.DictionaryContainer" /> containing options for shaders.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("_SCNShaderModifiers")]
	interface SCNShaderModifiers {
		string EntryPointGeometry { get; set; }
		string EntryPointSurface { get; set; }
		string EntryPointLightingModel { get; set; }
		string EntryPointFragment { get; set; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:SceneKit.SCNShadable" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:SceneKit.SCNShadable" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:SceneKit.SCNShadable" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:SceneKit.SCNShadable_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ISCNShadable { }

	/// <summary>Implementation of <see cref="T:SceneKit.ISCNShadable" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNShadable_Protocol/index.html">Apple documentation for <c>SCNShadable</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNShadable {

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("shaderModifiers", ArgumentSemantic.Copy)]
		NSDictionary WeakShaderModifiers { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Wrap ("WeakShaderModifiers")]
		SCNShaderModifiers ShaderModifiers { get; set; }

		[iOS (15, 0), TV (15, 0)]
		[MacCatalyst (15, 0)]
		[NullAllowed] // by default this property is null
		[Export ("minimumLanguageVersion", ArgumentSemantic.Retain)]
		NSNumber MinimumLanguageVersion { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("program", ArgumentSemantic.Retain)]
		SCNProgram Program { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("handleBindingOfSymbol:usingBlock:")]
		void HandleBinding (string symbol, [NullAllowed] SCNBindingHandler handler);

		[MacCatalyst (13, 1)]
		[Export ("handleUnbindingOfSymbol:usingBlock:")]
		void HandleUnbinding (string symbol, [NullAllowed] SCNBindingHandler handler);
	}

	/// <summary>Provides additional drawing passes, each of which applies a shader.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNTechnique_Class/index.html">Apple documentation for <c>SCNTechnique</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNTechnique : SCNAnimatable, NSCopying, NSSecureCoding {

		[Export ("dictionaryRepresentation")]
		NSDictionary ToDictionary ();

		[Static, Export ("techniqueWithDictionary:")]
		[return: NullAllowed]
		SCNTechnique Create (NSDictionary dictionary);

		[Static, Export ("techniqueBySequencingTechniques:")]
		[return: NullAllowed]
		SCNTechnique Create (SCNTechnique [] techniques);

		[MacCatalyst (13, 1)]
		[Export ("handleBindingOfSymbol:usingBlock:")]
		void HandleBinding (string symbol, [NullAllowed] SCNBindingHandler handler);

		[MacCatalyst (13, 1)]
		[Internal, Export ("objectForKeyedSubscript:")]
		[return: NullAllowed]
		NSObject _GetObject (NSObject key);

		[MacCatalyst (13, 1)]
		[Internal, Export ("setObject:forKeyedSubscript:")]
		void _SetObject ([NullAllowed] NSObject obj, INSCopying key);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("library", ArgumentSemantic.Strong)]
		IMTLLibrary Library { get; set; }
	}

	/// <summary>Implementation of <see cref="T:SceneKit.ISCNTechniqueSupport" /> that simply holds a reference to the <see cref="T:SceneKit.SCNTechnique" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNTechniqueSupport_Protocol/index.html">Apple documentation for <c>SCNTechniqueSupport</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNTechniqueSupport {
		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("technique", ArgumentSemantic.Copy)]
		[NullAllowed]
		SCNTechnique Technique { get; set; }
	}

	/// <summary>Contains values that control searching, backface culling, and collision testing.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface SCNPhysicsTestKeys {

		[Field ("SCNPhysicsTestCollisionBitMaskKey")]
		NSString CollisionBitMaskKey { get; }

		[Field ("SCNPhysicsTestSearchModeKey")]
		NSString SearchModeKey { get; }

		[Field ("SCNPhysicsTestBackfaceCullingKey")]
		NSString BackfaceCullingKey { get; }
	}

	/// <summary>Enumerates values that control which collision search results are returned.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface SCNPhysicsTestSearchModeKeys {

		[Field ("SCNPhysicsTestSearchModeAny")]
		NSString Any { get; }

		[Field ("SCNPhysicsTestSearchModeClosest")]
		NSString Closest { get; }

		[Field ("SCNPhysicsTestSearchModeAll")]
		NSString All { get; }
	}

	/// <summary>Adds physics simulation to an SCNNode.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsBody_Class/index.html">Apple documentation for <c>SCNPhysicsBody</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNPhysicsBody : NSCopying, NSSecureCoding {

		[Static, Export ("staticBody")]
		SCNPhysicsBody CreateStaticBody ();

		[Static, Export ("dynamicBody")]
		SCNPhysicsBody CreateDynamicBody ();

		[Static, Export ("kinematicBody")]
		SCNPhysicsBody CreateKinematicBody ();

		[Static, Export ("bodyWithType:shape:")]
		SCNPhysicsBody CreateBody (SCNPhysicsBodyType type, [NullAllowed] SCNPhysicsShape shape);

		[Export ("type")]
		SCNPhysicsBodyType Type { get; set; }

		[Export ("mass")]
		nfloat Mass { get; set; }

		[Export ("friction")]
		nfloat Friction { get; set; }

		[Export ("charge")]
		nfloat Charge { get; set; }

		[Export ("restitution")]
		nfloat Restitution { get; set; }

		[Export ("rollingFriction")]
		nfloat RollingFriction { get; set; }

		[Export ("physicsShape", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNPhysicsShape PhysicsShape { get; set; }

		[Export ("isResting")]
		bool IsResting { get; }

		[Export ("allowsResting")]
		bool AllowsResting { get; set; }

		[Export ("velocity")]
		SCNVector3 Velocity { get; set; }

		[Export ("angularVelocity")]
		SCNVector4 AngularVelocity { get; set; }

		[Export ("damping")]
		nfloat Damping { get; set; }

		[Export ("angularDamping")]
		nfloat AngularDamping { get; set; }

		[Export ("velocityFactor")]
		SCNVector3 VelocityFactor { get; set; }

		[Export ("angularVelocityFactor")]
		SCNVector3 AngularVelocityFactor { get; set; }

		[Export ("categoryBitMask", ArgumentSemantic.UnsafeUnretained)]
		nuint CategoryBitMask { get; set; }

		[Export ("collisionBitMask", ArgumentSemantic.UnsafeUnretained)]
		nuint CollisionBitMask { get; set; }

		[Export ("applyForce:impulse:")]
		void ApplyForce (SCNVector3 direction, bool impulse);

		[Export ("applyForce:atPosition:impulse:")]
		void ApplyForce (SCNVector3 direction, SCNVector3 position, bool impulse);

		[Export ("applyTorque:impulse:")]
		void ApplyTorque (SCNVector4 torque, bool impulse);

		[Export ("clearAllForces")]
		void ClearAllForces ();

		[Export ("resetTransform")]
		void ResetTransform ();

		[MacCatalyst (13, 1)]
		[Export ("momentOfInertia", ArgumentSemantic.Assign)]
		SCNVector3 MomentOfInertia { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("usesDefaultMomentOfInertia")]
		bool UsesDefaultMomentOfInertia { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("contactTestBitMask", ArgumentSemantic.Assign)]
		nuint ContactTestBitMask { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("affectedByGravity")]
		bool AffectedByGravity { [Bind ("isAffectedByGravity")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("setResting:")]
		void SetResting (bool resting);

		[MacCatalyst (13, 1)]
		[Export ("continuousCollisionDetectionThreshold")]
		nfloat ContinuousCollisionDetectionThreshold { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("centerOfMassOffset", ArgumentSemantic.Assign)]
		SCNVector3 CenterOfMassOffset { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("linearRestingThreshold")]
		nfloat LinearRestingThreshold { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("angularRestingThreshold")]
		nfloat AngularRestingThreshold { get; set; }
	}

	/// <summary>Delegate for the <see cref="M:SceneKit.SCNPhysicsField.CustomField(SceneKit.SCNFieldForceEvaluator)" /> method.</summary>
	[MacCatalyst (13, 1)]
	delegate SCNVector3 SCNFieldForceEvaluator (SCNVector3 position, SCNVector3 velocity, float /* float, not CGFloat */ mass, float /* float, not CGFloat */ charge, double timeInSeconds);

	/// <summary>Applies forces to all SCNPhysicsBody objects in a given volume.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsField_Class/index.html">Apple documentation for <c>SCNPhysicsField</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNPhysicsField : NSCopying, NSSecureCoding {

		[Static, Export ("dragField")]
		SCNPhysicsField CreateDragField ();

		[Static, Export ("vortexField")]
		SCNPhysicsField CreateVortexField ();

		[Static, Export ("radialGravityField")]
		SCNPhysicsField CreateRadialGravityField ();

		[Static, Export ("linearGravityField")]
		SCNPhysicsField CreateLinearGravityField ();

		[Static, Export ("noiseFieldWithSmoothness:animationSpeed:")]
		SCNPhysicsField CreateNoiseField (nfloat smoothness, nfloat speed);

		[Static, Export ("turbulenceFieldWithSmoothness:animationSpeed:")]
		SCNPhysicsField CreateTurbulenceField (nfloat smoothness, nfloat speed);

		[Static, Export ("springField")]
		SCNPhysicsField CreateSpringField ();

		[Static, Export ("electricField")]
		SCNPhysicsField CreateElectricField ();

		[Static, Export ("magneticField")]
		SCNPhysicsField CreateMagneticField ();

		[Static, Export ("customFieldWithEvaluationBlock:")]
		SCNPhysicsField CustomField (SCNFieldForceEvaluator evaluator);

		[Export ("strength")]
		nfloat Strength { get; set; }

		[Export ("falloffExponent")]
		nfloat FalloffExponent { get; set; }

		[Export ("minimumDistance")]
		nfloat MinimumDistance { get; set; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; set; }

		[Export ("exclusive")]
		bool Exclusive { [Bind ("isExclusive")] get; set; }

		[Export ("halfExtent")]
		SCNVector3 HalfExtent { get; set; }

		[Export ("usesEllipsoidalExtent")]
		bool UsesEllipsoidalExtent { get; set; }

		[Export ("scope")]
		SCNPhysicsFieldScope Scope { get; set; }

		[Export ("offset")]
		SCNVector3 Offset { get; set; }

		[Export ("direction")]
		SCNVector3 Direction { get; set; }

		[Export ("categoryBitMask")]
		nuint CategoryBitMask { get; set; }
	}

	/// <summary>Valid options for use with <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Scene%20Kit%20SCNPhysics%20World%20Contact%20Test&amp;scope=Xamarin" title="M:SceneKit.SCNPhysicsWorld.ContactTest*">M:SceneKit.SCNPhysicsWorld.ContactTest*</a></format>.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("SCNPhysicsTestKeys")]
	interface SCNPhysicsTest {
		nuint CollisionBitMask { get; set; }

		bool BackfaceCulling { get; set; }

		[Internal, Export ("SCNPhysicsTestKeys.SearchModeKey")]
		NSString _SearchMode { get; set; }
	}

	/// <summary>A physics simulation that models collisions and other physics-related properties in an SCNScene.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsWorld_Class/index.html">Apple documentation for <c>SCNPhysicsWorld</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject),
		Delegates = new [] { "WeakContactDelegate" },
		Events = new [] { typeof (SCNPhysicsContactDelegate) }
	)]
	[DisableDefaultCtor] // not to be allocated directly; use SCNScene.PhysicsWorld
	interface SCNPhysicsWorld : NSSecureCoding {

		[Export ("gravity")]
		SCNVector3 Gravity { get; set; }

		[Export ("speed")]
		nfloat Speed { get; set; }

		[Export ("timeStep")]
		double TimeStep { get; set; }

		[Export ("contactDelegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakContactDelegate { get; set; }

		[Wrap ("WeakContactDelegate")]
		ISCNPhysicsContactDelegate ContactDelegate { get; set; }

		[Export ("addBehavior:")]
		void AddBehavior (SCNPhysicsBehavior behavior);

		[Export ("removeBehavior:")]
		void RemoveBehavior (SCNPhysicsBehavior behavior);

		[Export ("removeAllBehaviors")]
		void RemoveAllBehaviors ();

		[Export ("allBehaviors")]
		SCNPhysicsBehavior [] AllBehaviors { get; }

		[Export ("rayTestWithSegmentFromPoint:toPoint:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNHitTestResult [] RayTestWithSegmentFromPoint (SCNVector3 origin, SCNVector3 dest, [NullAllowed] NSDictionary options);

		[Wrap ("RayTestWithSegmentFromPoint (origin, dest, options.GetDictionary ())")]
		SCNHitTestResult [] RayTestWithSegmentFromPoint (SCNVector3 origin, SCNVector3 dest, [NullAllowed] SCNPhysicsTest options);

		[Export ("contactTestBetweenBody:andBody:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNPhysicsContact [] ContactTest (SCNPhysicsBody bodyA, SCNPhysicsBody bodyB, [NullAllowed] NSDictionary options);

		[Wrap ("ContactTest (bodyA, bodyB, options.GetDictionary ())")]
		SCNPhysicsContact [] ContactTest (SCNPhysicsBody bodyA, SCNPhysicsBody bodyB, [NullAllowed] SCNPhysicsTest options);

		[Export ("contactTestWithBody:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNPhysicsContact [] ContactTest (SCNPhysicsBody body, [NullAllowed] NSDictionary options);

		[Wrap ("ContactTest (body, options.GetDictionary ())")]
		SCNPhysicsContact [] ContactTest (SCNPhysicsBody body, [NullAllowed] SCNPhysicsTest options);

		[Export ("convexSweepTestWithShape:fromTransform:toTransform:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNPhysicsContact [] ConvexSweepTest (SCNPhysicsShape shape, SCNMatrix4 from, SCNMatrix4 to, [NullAllowed] NSDictionary options);

		[Wrap ("ConvexSweepTest (shape, from, to, options.GetDictionary ())")]
		SCNPhysicsContact [] ConvexSweepTest (SCNPhysicsShape shape, SCNMatrix4 from, SCNMatrix4 to, [NullAllowed] SCNPhysicsTest options);

		[Export ("updateCollisionPairs")]
		void UpdateCollisionPairs ();
	}

	/// <summary>A solid volume for a SCNPhysicsBody. Used in collision detection.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsShape_Class/index.html">Apple documentation for <c>SCNPhysicsShape</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNPhysicsShape : NSCopying, NSSecureCoding {

		[Internal, Static, Export ("shapeWithShapes:transforms:")]
		SCNPhysicsShape Create (SCNPhysicsShape [] shapes, [NullAllowed] NSValue [] transforms);

		[Static, Export ("shapeWithGeometry:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNPhysicsShape Create (SCNGeometry geometry, [NullAllowed] NSDictionary options);

		[Static, Export ("shapeWithNode:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNPhysicsShape Create (SCNNode node, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("options"), Internal]
		NSDictionary _Options { get; }

		[MacCatalyst (13, 1)]
		[Export ("sourceObject")]
		NSObject SourceObject { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("transforms")]
		NSValue [] Transforms { get; }
	}

	/// <summary>Defines valid keys for use with the <c>options</c> dictionary passed to <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Scene%20Kit%20SCNPhysics%20Shape%20Create&amp;scope=Xamarin" title="M:SceneKit.SCNPhysicsShape.Create*">M:SceneKit.SCNPhysicsShape.Create*</a></format>.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface SCNPhysicsShapeOptionsKeys {

		[Field ("SCNPhysicsShapeScaleKey")]
		NSString Scale { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNPhysicsShapeOptionCollisionMargin")]
		NSString CollisionMargin { get; }

		[Field ("SCNPhysicsShapeKeepAsCompoundKey")]
		NSString KeepAsCompound { get; }

		[Field ("SCNPhysicsShapeTypeKey")]
		NSString Type { get; }
	}

	/// <summary>Geometrical type associated with <see cref="T:SceneKit.SCNPhysicsShape" />s.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface SCNPhysicsShapeOptionsTypes {

		[Field ("SCNPhysicsShapeTypeBoundingBox")]
		NSString BoundingBox { get; }

		[Field ("SCNPhysicsShapeTypeConvexHull")]
		NSString ConvexHull { get; }

		[Field ("SCNPhysicsShapeTypeConcavePolyhedron")]
		NSString ConcavePolyhedron { get; }
	}

	/// <summary>How two SCNPhysicsBody objects are in contact with each other.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsContact_Class/index.html">Apple documentation for <c>SCNPhysicsContact</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNPhysicsContact {

		[Export ("nodeA")]
		SCNNode NodeA { get; }

		[Export ("nodeB")]
		SCNNode NodeB { get; }

		[Export ("contactPoint")]
		SCNVector3 ContactPoint { get; }

		[Export ("contactNormal")]
		SCNVector3 ContactNormal { get; }

		[Export ("collisionImpulse")]
		nfloat CollisionImpulse { get; }

		[Export ("penetrationDistance")]
		nfloat PenetrationDistance { get; }

		[MacCatalyst (13, 1)]
		[Export ("sweepTestFraction")]
		nfloat SweepTestFraction { get; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:SceneKit.SCNPhysicsContactDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:SceneKit.SCNPhysicsContactDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:SceneKit.SCNPhysicsContactDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:SceneKit.SCNPhysicsContactDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ISCNPhysicsContactDelegate { }

	/// <summary>Delegate object functions responding to touches between <see cref="T:SceneKit.SCNPhysicsBody" /> objects..</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsContactDelegate_Protocol/index.html">Apple documentation for <c>SCNPhysicsContactDelegate</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNPhysicsContactDelegate {

		[Export ("physicsWorld:didBeginContact:"), EventArgs ("SCNPhysicsContact")]
		void DidBeginContact (SCNPhysicsWorld world, SCNPhysicsContact contact);

		[Export ("physicsWorld:didUpdateContact:"), EventArgs ("SCNPhysicsContact")]
		void DidUpdateContact (SCNPhysicsWorld world, SCNPhysicsContact contact);

		[Export ("physicsWorld:didEndContact:"), EventArgs ("SCNPhysicsContact")]
		void DidEndContact (SCNPhysicsWorld world, SCNPhysicsContact contact);
	}

	/// <summary>Controls the behavior of one or more SCNPhysicsBody objects.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsBehavior/index.html">Apple documentation for <c>SCNPhysicsBehavior</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[DisableDefaultCtor]
	interface SCNPhysicsBehavior : NSSecureCoding {

	}

	/// <summary>An SCNPhysicsBehavior that attaches two SCNPhysicsBody objects along a single axis.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsHingeJoint_Class/index.html">Apple documentation for <c>SCNPhysicsHingeJoint</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNPhysicsBehavior))]
	[DisableDefaultCtor]
	interface SCNPhysicsHingeJoint {

		[Static, Export ("jointWithBodyA:axisA:anchorA:bodyB:axisB:anchorB:")]
		SCNPhysicsHingeJoint Create (SCNPhysicsBody bodyA, SCNVector3 axisA, SCNVector3 anchorA,
			SCNPhysicsBody bodyB, SCNVector3 axisB, SCNVector3 anchorB);

		[Static, Export ("jointWithBody:axis:anchor:")]
		SCNPhysicsHingeJoint Create (SCNPhysicsBody body, SCNVector3 axis, SCNVector3 anchor);

		[Export ("bodyA")]
		SCNPhysicsBody BodyA { get; }

		[Export ("axisA")]
		SCNVector3 AxisA { get; set; }

		[Export ("anchorA")]
		SCNVector3 AnchorA { get; set; }

		[Export ("bodyB")]
		[NullAllowed]
		SCNPhysicsBody BodyB { get; }

		[Export ("axisB")]
		SCNVector3 AxisB { get; set; }

		[Export ("anchorB")]
		SCNVector3 AnchorB { get; set; }
	}

	/// <summary>An SCNPhysicsBehavior that allows two SCNPhysicsBody objects to pivot around each other.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsBallSocketJoint_Class/index.html">Apple documentation for <c>SCNPhysicsBallSocketJoint</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNPhysicsBehavior))]
	[DisableDefaultCtor]
	interface SCNPhysicsBallSocketJoint {

		[Static, Export ("jointWithBodyA:anchorA:bodyB:anchorB:")]
		SCNPhysicsBallSocketJoint Create (SCNPhysicsBody bodyA, SCNVector3 anchorA, SCNPhysicsBody bodyB, SCNVector3 anchorB);

		[Static, Export ("jointWithBody:anchor:")]
		SCNPhysicsBallSocketJoint Create (SCNPhysicsBody body, SCNVector3 anchor);

		[Export ("bodyA")]
		SCNPhysicsBody BodyA { get; }

		[Export ("anchorA")]
		SCNVector3 AnchorA { get; set; }

		[Export ("bodyB")]
		[NullAllowed]
		SCNPhysicsBody BodyB { get; }

		[Export ("anchorB")]
		SCNVector3 AnchorB { get; set; }
	}

	/// <summary>An SCNPhysicsBehavior that connects two SCNPhysicsBody objects, allowing them to slide and rotate around their connecting points.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsSliderJoint_Class/index.html">Apple documentation for <c>SCNPhysicsSliderJoint</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNPhysicsBehavior))]
	[DisableDefaultCtor]
	interface SCNPhysicsSliderJoint {

		[Static, Export ("jointWithBodyA:axisA:anchorA:bodyB:axisB:anchorB:")]
		SCNPhysicsSliderJoint Create (SCNPhysicsBody bodyA, SCNVector3 axisA,
			SCNVector3 anchorA, SCNPhysicsBody bodyB, SCNVector3 axisB, SCNVector3 anchorB);

		[Static, Export ("jointWithBody:axis:anchor:")]
		SCNPhysicsSliderJoint Create (SCNPhysicsBody body, SCNVector3 axis, SCNVector3 anchor);

		[Export ("bodyA")]
		SCNPhysicsBody BodyA { get; }

		[Export ("axisA")]
		SCNVector3 AxisA { get; set; }

		[Export ("anchorA")]
		SCNVector3 AnchorA { get; set; }

		[Export ("bodyB")]
		[NullAllowed]
		SCNPhysicsBody BodyB { get; }

		[Export ("axisB")]
		SCNVector3 AxisB { get; set; }

		[Export ("anchorB")]
		SCNVector3 AnchorB { get; set; }

		[Export ("minimumLinearLimit")]
		nfloat MinimumLinearLimit { get; set; }

		[Export ("maximumLinearLimit")]
		nfloat MaximumLinearLimit { get; set; }

		[Export ("minimumAngularLimit")]
		nfloat MinimumAngularLimit { get; set; }

		[Export ("maximumAngularLimit")]
		nfloat MaximumAngularLimit { get; set; }

		[Export ("motorTargetLinearVelocity")]
		nfloat MotorTargetLinearVelocity { get; set; }

		[Export ("motorMaximumForce")]
		nfloat MotorMaximumForce { get; set; }

		[Export ("motorTargetAngularVelocity")]
		nfloat MotorTargetAngularVelocity { get; set; }

		[Export ("motorMaximumTorque")]
		nfloat MotorMaximumTorque { get; set; }
	}

	/// <summary>An SCNPhysicsBehavior that simulates the physics of a wheeled vehicle.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsVehicle_Class/index.html">Apple documentation for <c>SCNPhysicsVehicle</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNPhysicsBehavior))]
	[DisableDefaultCtor]
	interface SCNPhysicsVehicle {

		[Static, Export ("vehicleWithChassisBody:wheels:")]
		SCNPhysicsVehicle Create (SCNPhysicsBody chassisBody, SCNPhysicsVehicleWheel [] wheels);

		[Export ("speedInKilometersPerHour")]
		nfloat SpeedInKilometersPerHour { get; }

		[Export ("wheels")]
		SCNPhysicsVehicleWheel [] Wheels { get; }

		[Export ("chassisBody")]
		SCNPhysicsBody ChassisBody { get; }

		[Export ("applyEngineForce:forWheelAtIndex:")]
		void ApplyEngineForce (nfloat value, nint index);

		[Export ("setSteeringAngle:forWheelAtIndex:")]
		void SetSteeringAngle (nfloat value, nint index);

		[Export ("applyBrakingForce:forWheelAtIndex:")]
		void ApplyBrakingForce (nfloat value, nint index);
	}

	/// <summary>The appearance and physical characteristics of a wheel on a SCNPhysicsVehicle.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNPhysicsVehicleWheel_Class/index.html">Apple documentation for <c>SCNPhysicsVehicleWheel</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNPhysicsVehicleWheel : NSCopying, NSSecureCoding {

		[Static, Export ("wheelWithNode:")]
		SCNPhysicsVehicleWheel Create (SCNNode node);

		[Export ("node")]
		SCNNode Node { get; }

		[Export ("suspensionStiffness")]
		nfloat SuspensionStiffness { get; set; }

		[Export ("suspensionCompression")]
		nfloat SuspensionCompression { get; set; }

		[Export ("suspensionDamping")]
		nfloat SuspensionDamping { get; set; }

		[Export ("maximumSuspensionTravel")]
		nfloat MaximumSuspensionTravel { get; set; }

		[Export ("frictionSlip")]
		nfloat FrictionSlip { get; set; }

		[Export ("maximumSuspensionForce")]
		nfloat MaximumSuspensionForce { get; set; }

		[Export ("connectionPosition")]
		SCNVector3 ConnectionPosition { get; set; }

		[Export ("steeringAxis")]
		SCNVector3 SteeringAxis { get; set; }

		[Export ("axle")]
		SCNVector3 Axle { get; set; }

		[Export ("radius")]
		nfloat Radius { get; set; }

		[Export ("suspensionRestLength")]
		nfloat SuspensionRestLength { get; set; }
	}

	/// <summary>Emits small sprites whose behaviors are developer-specified.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNParticleSystem_Class/index.html">Apple documentation for <c>SCNParticleSystem</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNParticleSystem : NSCopying, NSSecureCoding, SCNAnimatable {

		[Static, Export ("particleSystem")]
		SCNParticleSystem Create ();

		[Static, Export ("particleSystemNamed:inDirectory:")]
		[return: NullAllowed]
		SCNParticleSystem Create (string name, [NullAllowed] string directory);

		[Export ("emissionDuration")]
		nfloat EmissionDuration { get; set; }

		[Export ("emissionDurationVariation")]
		nfloat EmissionDurationVariation { get; set; }

		[Export ("idleDuration")]
		nfloat IdleDuration { get; set; }

		[Export ("idleDurationVariation")]
		nfloat IdleDurationVariation { get; set; }

		[Export ("loops")]
		bool Loops { get; set; }

		[Export ("birthRate")]
		nfloat BirthRate { get; set; }

		[Export ("birthRateVariation")]
		nfloat BirthRateVariation { get; set; }

		[Export ("warmupDuration")]
		nfloat WarmupDuration { get; set; }

		[Export ("emitterShape", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNGeometry EmitterShape { get; set; }

		[Export ("birthLocation")]
		SCNParticleBirthLocation BirthLocation { get; set; }

		[Export ("birthDirection")]
		SCNParticleBirthDirection BirthDirection { get; set; }

		[Export ("spreadingAngle")]
		nfloat SpreadingAngle { get; set; }

		[Export ("emittingDirection")]
		SCNVector3 EmittingDirection { get; set; }

		[Export ("acceleration")]
		SCNVector3 Acceleration { get; set; }

		[Export ("local")]
		bool Local { [Bind ("isLocal")] get; set; }

		[Export ("particleAngle")]
		nfloat ParticleAngle { get; set; }

		[Export ("particleAngleVariation")]
		nfloat ParticleAngleVariation { get; set; }

		[Export ("particleVelocity")]
		nfloat ParticleVelocity { get; set; }

		[Export ("particleVelocityVariation")]
		nfloat ParticleVelocityVariation { get; set; }

		[Export ("particleAngularVelocity")]
		nfloat ParticleAngularVelocity { get; set; }

		[Export ("particleAngularVelocityVariation")]
		nfloat ParticleAngularVelocityVariation { get; set; }

		[Export ("particleLifeSpan")]
		nfloat ParticleLifeSpan { get; set; }

		[Export ("particleLifeSpanVariation")]
		nfloat ParticleLifeSpanVariation { get; set; }

		[Export ("systemSpawnedOnDying", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNParticleSystem SystemSpawnedOnDying { get; set; }

		[Export ("systemSpawnedOnCollision", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNParticleSystem SystemSpawnedOnCollision { get; set; }

		[Export ("systemSpawnedOnLiving", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNParticleSystem SystemSpawnedOnLiving { get; set; }

		[Export ("particleImage", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSObject ParticleImage { get; set; }

		[Export ("imageSequenceColumnCount")]
		nuint ImageSequenceColumnCount { get; set; }

		[Export ("imageSequenceRowCount")]
		nuint ImageSequenceRowCount { get; set; }

		[Export ("imageSequenceInitialFrame")]
		nfloat ImageSequenceInitialFrame { get; set; }

		[Export ("imageSequenceInitialFrameVariation")]
		nfloat ImageSequenceInitialFrameVariation { get; set; }

		[Export ("imageSequenceFrameRate")]
		nfloat ImageSequenceFrameRate { get; set; }

		[Export ("imageSequenceFrameRateVariation")]
		nfloat ImageSequenceFrameRateVariation { get; set; }

		[Export ("imageSequenceAnimationMode")]
		SCNParticleImageSequenceAnimationMode ImageSequenceAnimationMode { get; set; }

		[Export ("particleColor", ArgumentSemantic.Retain)]
		NSColor ParticleColor { get; set; }

		[Export ("particleColorVariation")]
		SCNVector4 ParticleColorVariation { get; set; }

		[Export ("particleSize")]
		nfloat ParticleSize { get; set; }

		[Export ("particleSizeVariation")]
		nfloat ParticleSizeVariation { get; set; }

		[Export ("blendMode")]
		SCNParticleBlendMode BlendMode { get; set; }

		[Export ("blackPassEnabled")]
		bool BlackPassEnabled { [Bind ("isBlackPassEnabled")] get; set; }

		[Export ("orientationMode")]
		SCNParticleOrientationMode OrientationMode { get; set; }

		[Export ("sortingMode")]
		SCNParticleSortingMode SortingMode { get; set; }

		[Export ("lightingEnabled")]
		bool LightingEnabled { [Bind ("isLightingEnabled")] get; set; }

		[Export ("affectedByGravity")]
		bool AffectedByGravity { get; set; }

		[Export ("affectedByPhysicsFields")]
		bool AffectedByPhysicsFields { get; set; }

		[Export ("particleDiesOnCollision")]
		bool ParticleDiesOnCollision { get; set; }

		[Export ("colliderNodes", ArgumentSemantic.Copy)]
		[NullAllowed]
		SCNNode [] ColliderNodes { get; set; }

		[Export ("particleMass")]
		nfloat ParticleMass { get; set; }

		[Export ("particleMassVariation")]
		nfloat ParticleMassVariation { get; set; }

		[Export ("particleBounce")]
		nfloat ParticleBounce { get; set; }

		[Export ("particleBounceVariation")]
		nfloat ParticleBounceVariation { get; set; }

		[Export ("particleFriction")]
		nfloat ParticleFriction { get; set; }

		[Export ("particleFrictionVariation")]
		nfloat ParticleFrictionVariation { get; set; }

		[Export ("particleCharge")]
		nfloat ParticleCharge { get; set; }

		[Export ("particleChargeVariation")]
		nfloat ParticleChargeVariation { get; set; }

		[Export ("dampingFactor")]
		nfloat DampingFactor { get; set; }

		[Export ("speedFactor")]
		nfloat SpeedFactor { get; set; }

		[Export ("stretchFactor")]
		nfloat StretchFactor { get; set; }

		[Export ("fresnelExponent")]
		nfloat FresnelExponent { get; set; }

		[Introduced (PlatformName.MacCatalyst, 15, 0)]
		[TV (15, 0), iOS (15, 0)]
		[Export ("writesToDepthBuffer")]
		bool WritesToDepthBuffer { get; set; }

		[Export ("propertyControllers", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary WeakPropertyControllers { get; set; }

		[Export ("reset")]
		void Reset ();

		[Export ("handleEvent:forProperties:withBlock:")]
		void HandleEvent (SCNParticleEvent evnt, NSString [] particleProperties, SCNParticleEventHandler handler);

		[Export ("addModifierForProperties:atStage:withBlock:")]
		void AddModifier (NSString [] properties, SCNParticleModifierStage stage, SCNParticleModifierHandler handler);

		[Export ("removeModifiersOfStage:")]
		void RemoveModifiers (SCNParticleModifierStage stage);

		[Export ("removeAllModifiers")]
		void RemoveAllModifiers ();

		[MacCatalyst (13, 1)]
		[Export ("orientationDirection", ArgumentSemantic.Assign)]
		SCNVector3 OrientationDirection { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("particleIntensity")]
		nfloat ParticleIntensity { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("particleIntensityVariation")]
		nfloat ParticleIntensityVariation { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface SCNParticleProperty {
		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyPosition")]
		NSString Position { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyAngle")]
		NSString Angle { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyRotationAxis")]
		NSString RotationAxis { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyVelocity")]
		NSString Velocity { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyAngularVelocity")]
		NSString AngularVelocity { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyLife")]
		NSString Life { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyColor")]
		NSString Color { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyOpacity")]
		NSString Opacity { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertySize")]
		NSString Size { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyFrame")]
		NSString Frame { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyFrameRate")]
		NSString FrameRate { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyBounce")]
		NSString Bounce { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyCharge")]
		NSString Charge { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyFriction")]
		NSString Friction { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyContactPoint")]
		NSString ContactPoint { get; }

		[MacCatalyst (13, 1)]
		[Field ("SCNParticlePropertyContactNormal")]
		NSString ContactNormal { get; }
	}

	/// <summary>Completion handler for use with <see cref="M:SceneKit.SCNParticleSystem.HandleEvent(SceneKit.SCNParticleEvent,Foundation.NSString[],SceneKit.SCNParticleEventHandler)" />.</summary>
	[MacCatalyst (13, 1)]
	delegate void SCNParticleEventHandler (IntPtr data, IntPtr dataStride, IntPtr indices, nint count);

	/// <summary>Delegate used as the <c>handler</c> in <see cref="M:SceneKit.SCNParticleSystem.AddModifier(Foundation.NSString[],SceneKit.SCNParticleModifierStage,SceneKit.SCNParticleModifierHandler)" />.</summary>
	[MacCatalyst (13, 1)]
	delegate void SCNParticleModifierHandler (IntPtr data, IntPtr dataStride, nint start, nint end, float /* float, not CGFloat */ deltaTime);

	/// <summary>Animates a property of the particles emitted by a SCNParticleSystem.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNParticlePropertyController_Class/index.html">Apple documentation for <c>SCNParticlePropertyController</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNParticlePropertyController : NSSecureCoding, NSCopying {

		[MacCatalyst (13, 1)]
		[Static, Export ("controllerWithAnimation:")]
		SCNParticlePropertyController Create (CAAnimation animation);

		[MacCatalyst (13, 1)]
		[Export ("animation", ArgumentSemantic.Retain)]
		CAAnimation Animation { get; set; }

		[Export ("inputMode")]
		SCNParticleInputMode InputMode { get; set; }

		[Export ("inputScale")]
		nfloat InputScale { get; set; }

		[Export ("inputBias")]
		nfloat InputBias { get; set; }

		[Export ("inputOrigin", ArgumentSemantic.Weak)]
		[NullAllowed]
		SCNNode InputOrigin { get; set; }

		[Export ("inputProperty")]
		[NullAllowed]
		NSString InputProperty { get; set; }
	}

	/// <summary>Automatically constrains a <see cref="T:SceneKit.SCNNode" /> so that it always points to the current rendering node.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNBillboardConstraint_Class/index.html">Apple documentation for <c>SCNBillboardConstraint</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNConstraint))]
	interface SCNBillboardConstraint {
		[Static]
		[Export ("billboardConstraint")]
		SCNBillboardConstraint Create ();

		[Export ("freeAxes", ArgumentSemantic.Assign)]
		SCNBillboardAxis FreeAxes { get; set; }
	}

	/// <summary>A <see cref="T:SceneKit.SCNNode" /> that loads its content from a scene file.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/SceneKit/Reference/SCNReferenceNode_Class/index.html">Apple documentation for <c>SCNReferenceNode</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNNode))]
	[DisableDefaultCtor]
	interface SCNReferenceNode : NSCoding {
		[Export ("initWithURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl referenceUrl);

		[Static]
		[Export ("referenceNodeWithURL:")]
		[return: NullAllowed]
		SCNReferenceNode CreateFromUrl (NSUrl referenceUrl);

		[Export ("referenceURL", ArgumentSemantic.Copy)]
		NSUrl ReferenceUrl { get; set; }

		[Export ("loadingPolicy", ArgumentSemantic.Assign)]
		SCNReferenceLoadingPolicy LoadingPolicy { get; set; }

		[Export ("load")]
		void Load ();

		[Export ("unload")]
		void Unload ();

		[Export ("loaded")]
		bool Loaded { [Bind ("isLoaded")] get; }
	}

	interface ISCNBufferStream { }

	/// <summary>Interface for managing a custom shader's Metal buffer.</summary>
	[MacCatalyst (13, 1)]
	[Protocol]
	interface SCNBufferStream {
		[Abstract]
		[Export ("writeBytes:length:")]
		unsafe void Length (IntPtr bytes, nuint length);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNTimingFunction : NSSecureCoding {
		[Static]
		[Export ("functionWithTimingMode:")]
		SCNTimingFunction Create (SCNActionTimingMode timingMode);

		[Static]
		[MacCatalyst (13, 1)]
		[Export ("functionWithCAMediaTimingFunction:")]
		SCNTimingFunction Create (CAMediaTimingFunction caTimingFunction);
	}

	// Use the Swift name SCNAnimationProtocol since it conflicts with the type name
	[Protocol (Name = "SCNAnimation")]
	interface SCNAnimationProtocol {
	}

	interface ISCNAnimationProtocol { }

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNAnimation : SCNAnimationProtocol, NSCopying, NSSecureCoding {
		[Static]
		[Export ("animationWithContentsOfURL:")]
		SCNAnimation FromUrl (NSUrl animationUrl);

		[Static]
		[Export ("animationNamed:")]
		SCNAnimation FromName (string animationName);

		[Static]
		[MacCatalyst (13, 1)]
		[Export ("animationWithCAAnimation:")]
		SCNAnimation FromCAAnimation (CAAnimation caAnimation);

		[Export ("duration")]
		double Duration { get; set; }

		[NullAllowed, Export ("keyPath")]
		string KeyPath { get; set; }

		[Export ("timingFunction", ArgumentSemantic.Retain)]
		SCNTimingFunction TimingFunction { get; set; }

		[Export ("blendInDuration")]
		double BlendInDuration { get; set; }

		[Export ("blendOutDuration")]
		double BlendOutDuration { get; set; }

		[Export ("removedOnCompletion")]
		bool RemovedOnCompletion { [Bind ("isRemovedOnCompletion")] get; set; }

		[Export ("appliedOnCompletion")]
		bool AppliedOnCompletion { [Bind ("isAppliedOnCompletion")] get; set; }

		[Export ("repeatCount")]
		nfloat RepeatCount { get; set; }

		[Export ("autoreverses")]
		bool Autoreverses { get; set; }

		[Export ("startDelay")]
		double StartDelay { get; set; }

		[Export ("timeOffset")]
		double TimeOffset { get; set; }

		[Export ("fillsForward")]
		bool FillsForward { get; set; }

		[Export ("fillsBackward")]
		bool FillsBackward { get; set; }

		[Export ("usesSceneTimeBase")]
		bool UsesSceneTimeBase { get; set; }

#if !NET
		[Sealed]
		[NullAllowed, Export ("animationDidStart", ArgumentSemantic.Copy)]
		SCNAnimationDidStartHandler2 AnimationDidStart2 { get; set; }

		[Obsolete ("Use 'AnimationDidStart2' instead.")]
#endif
		[NullAllowed, Export ("animationDidStart", ArgumentSemantic.Copy)]
		SCNAnimationDidStartHandler AnimationDidStart { get; set; }

#if !NET
		[Sealed]
		[NullAllowed, Export ("animationDidStop", ArgumentSemantic.Copy)]
		SCNAnimationDidStopHandler2 AnimationDidStop2 { get; set; }

		[Obsolete ("Use 'AnimationDidStop2' instead.")]
#endif
		[NullAllowed, Export ("animationDidStop", ArgumentSemantic.Copy)]
		SCNAnimationDidStopHandler AnimationDidStop { get; set; }

		[NullAllowed, Export ("animationEvents", ArgumentSemantic.Copy)]
		SCNAnimationEvent [] AnimationEvents { get; set; }

		[Export ("additive")]
		bool Additive { [Bind ("isAdditive")] get; set; }

		[Export ("cumulative")]
		bool Cumulative { [Bind ("isCumulative")] get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SCNAnimationPlayer : SCNAnimatable, NSCopying, NSSecureCoding {
		[Static]
		[Export ("animationPlayerWithAnimation:")]
		SCNAnimationPlayer FromAnimation (SCNAnimation animation);

		[Export ("animation")]
		SCNAnimation Animation { get; }

		[Export ("speed")]
		nfloat Speed { get; set; }

		[Export ("blendFactor")]
		nfloat BlendFactor { get; set; }

		[Export ("paused")]
		bool Paused { get; set; }

		[Export ("play")]
		void Play ();

		[Export ("stop")]
		void Stop ();

		[Export ("stopWithBlendOutDuration:")]
		void StopWithBlendOutDuration (double seconds);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNConstraint))]
	interface SCNDistanceConstraint {

		[Static]
		[Export ("distanceConstraintWithTarget:")]
		SCNDistanceConstraint FromTarget ([NullAllowed] SCNNode target);

		[NullAllowed, Export ("target", ArgumentSemantic.Retain)]
		SCNNode Target { get; set; }

		[Export ("minimumDistance")]
		nfloat MinimumDistance { get; set; }

		[Export ("maximumDistance")]
		nfloat MaximumDistance { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNConstraint))]
	interface SCNReplicatorConstraint {

		[Static]
		[Export ("replicatorConstraintWithTarget:")]
		SCNReplicatorConstraint FromTarget ([NullAllowed] SCNNode target);

		[NullAllowed, Export ("target", ArgumentSemantic.Retain)]
		SCNNode Target { get; set; }

		[Export ("replicatesOrientation")]
		bool ReplicatesOrientation { get; set; }

		[Export ("replicatesPosition")]
		bool ReplicatesPosition { get; set; }

		[Export ("replicatesScale")]
		bool ReplicatesScale { get; set; }

		[Export ("orientationOffset", ArgumentSemantic.Assign)]
		SCNQuaternion OrientationOffset { get; set; }

		[Export ("positionOffset", ArgumentSemantic.Assign)]
		SCNVector3 PositionOffset { get; set; }

		[Export ("scaleOffset", ArgumentSemantic.Assign)]
		SCNVector3 ScaleOffset { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNConstraint))]
	interface SCNAccelerationConstraint {

		[Static]
		[Export ("accelerationConstraint")]
		SCNAccelerationConstraint Create ();

		[Export ("maximumLinearAcceleration")]
		nfloat MaximumLinearAcceleration { get; set; }

		[Export ("maximumLinearVelocity")]
		nfloat MaximumLinearVelocity { get; set; }

		[Export ("decelerationDistance")]
		nfloat DecelerationDistance { get; set; }

		[Export ("damping")]
		nfloat Damping { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNConstraint))]
	interface SCNSliderConstraint {

		[Static]
		[Export ("sliderConstraint")]
		SCNSliderConstraint Create ();

		[Export ("collisionCategoryBitMask")]
		nuint CollisionCategoryBitMask { get; set; }

		[Export ("radius")]
		nfloat Radius { get; set; }

		[Export ("offset", ArgumentSemantic.Assign)]
		SCNVector3 Offset { get; set; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:SceneKit.SCNAvoidOccluderConstraintDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:SceneKit.SCNAvoidOccluderConstraintDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:SceneKit.SCNAvoidOccluderConstraintDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:SceneKit.SCNAvoidOccluderConstraintDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ISCNAvoidOccluderConstraintDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNAvoidOccluderConstraintDelegate {

		[Export ("avoidOccluderConstraint:shouldAvoidOccluder:forNode:")]
		bool ShouldAvoidOccluder (SCNAvoidOccluderConstraint constraint, SCNNode occluder, SCNNode node);

		[Export ("avoidOccluderConstraint:didAvoidOccluder:forNode:")]
		void DidAvoidOccluder (SCNAvoidOccluderConstraint constraint, SCNNode occluder, SCNNode node);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNConstraint))]
	interface SCNAvoidOccluderConstraint {

		[Static]
		[Export ("avoidOccluderConstraintWithTarget:")]
		SCNAvoidOccluderConstraint FromTarget ([NullAllowed] SCNNode target);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Assign)]
		ISCNAvoidOccluderConstraintDelegate Delegate { get; set; }

		[NullAllowed, Export ("target", ArgumentSemantic.Retain)]
		SCNNode Target { get; set; }

		[Export ("occluderCategoryBitMask")]
		nuint OccluderCategoryBitMask { get; set; }

		[Export ("bias")]
		nfloat Bias { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (SCNPhysicsBehavior))]
	interface SCNPhysicsConeTwistJoint {

		[Static]
		[Export ("jointWithBodyA:frameA:bodyB:frameB:")]
		SCNPhysicsConeTwistJoint FromBodies (SCNPhysicsBody bodyA, SCNMatrix4 frameA, SCNPhysicsBody bodyB, SCNMatrix4 frameB);

		[Static]
		[Export ("jointWithBody:frame:")]
		SCNPhysicsConeTwistJoint FromBody (SCNPhysicsBody body, SCNMatrix4 frame);

		[Export ("bodyA")]
		SCNPhysicsBody BodyA { get; }

		[Export ("frameA", ArgumentSemantic.Assign)]
		SCNMatrix4 FrameA { get; set; }

		[NullAllowed, Export ("bodyB")]
		SCNPhysicsBody BodyB { get; }

		[Export ("frameB", ArgumentSemantic.Assign)]
		SCNMatrix4 FrameB { get; set; }

		[Export ("maximumAngularLimit1")]
		nfloat MaximumAngularLimit1 { get; set; }

		[Export ("maximumAngularLimit2")]
		nfloat MaximumAngularLimit2 { get; set; }

		[Export ("maximumTwistAngle")]
		nfloat MaximumTwistAngle { get; set; }
	}


}
