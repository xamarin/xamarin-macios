///
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

using NMatrix4 = global::OpenTK.NMatrix4;
using NVector3 = global::OpenTK.NVector3;
using Vector4 = global::OpenTK.Vector4;
using Quaternion = global::OpenTK.Quaternion;

#if !WATCH
using CoreAnimation;
using CoreImage;
#endif

#if WATCH || XAMCORE_4_0
using AnimationType = global::SceneKit.ISCNAnimationProtocol;
#else
using AnimationType = global::CoreAnimation.CAAnimation;
#endif

using CoreGraphics;
using SpriteKit;
// MonoMac (classic) does not support those 64bits only frameworks
#if (XAMCORE_2_0 || !MONOMAC) && !WATCH
using ModelIO;
using Metal;
using GameplayKit;
#endif

#if MONOMAC
using AppKit;
using OpenTK;

using GLContext = global::OpenGL.CGLContext;
#else
using UIKit;

#if WATCH
using GLContext = global::Foundation.NSObject; // won't be used -> [NoWatch] but must compile
using NSView = global::Foundation.NSObject; // won't be used -> [NoWatch] but must compile
using SCNGeometryTessellator = global::Foundation.NSObject; // won't be used -> [NoWatch] but must compile
#else
using OpenGLES;

using GLContext = global::OpenGLES.EAGLContext;
using NSView = global::UIKit.UIView;
#endif

using NSColor = global::UIKit.UIColor;
using NSFont = global::UIKit.UIFont;
using NSImage = global::UIKit.UIImage;
using NSBezierPath = global::UIKit.UIBezierPath;
#endif

namespace SceneKit {

#if WATCH
	// stubs to limit the number of preprocessor directives in the source file
	interface CAAnimation {}
	interface CALayer {}
	interface CAMediaTimingFunction {}
	interface MDLAsset {}
	interface MDLCamera {}
	interface MDLLight {}
	interface MDLMaterial {}
	interface MDLMesh {}
	interface MDLObject {}
	interface MDLSubmesh {}
	enum MTLPixelFormat {}
	enum MTLVertexFormat {}
	interface IMTLBuffer {}
	interface IMTLCommandBuffer {}
	interface IMTLCommandQueue {}
	interface IMTLDevice {}
	interface IMTLLibrary {}
	interface IMTLRenderCommandEncoder {}
	interface MTLRenderPassDescriptor {}
#endif

	[iOS (8,0)]
	delegate void SCNSceneSourceStatusHandler (float /* float, not CGFloat */ totalProgress, SCNSceneSourceStatus status, NSError error, ref bool stopLoading);

	delegate void SCNAnimationDidStartHandler (SCNAnimation animation, SCNAnimatable receiver);
	delegate void SCNAnimationDidStopHandler (SCNAnimation animation, SCNAnimatable receiver, bool completed);

	[Watch (3,0)]
	[iOS (8,0)]
	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	interface SCNAnimatable {
#if XAMCORE_2_0
		[Abstract]
#endif
		[NoWatch]
		[Export ("addAnimation:forKey:")]
#if !XAMCORE_4_0
		void AddAnimation (CAAnimation animation, [NullAllowed] NSString key);
#else
		void AddAnimation (ISCNAnimationProtocol scnAnimation, [NullAllowed] string key);
#endif

#if XAMCORE_2_0
#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (11,0), TV (11,0), Watch (4,0), Mac (10,13, onlyOn64: true)]
		[Export ("addAnimationPlayer:forKey:")]
		void AddAnimation (SCNAnimationPlayer player, [NullAllowed] NSString key);
#endif

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("removeAllAnimations")]
		void RemoveAllAnimations ();

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("removeAnimationForKey:")]
		void RemoveAnimation (NSString key);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("animationKeys")]
		NSString [] GetAnimationKeys ();

#if XAMCORE_2_0
#if XAMCORE_4_0
		[Abstract]
#endif
		[return: NullAllowed]
		[iOS (11,0), TV (11,0), Watch (4,0), Mac (10,13, onlyOn64: true)]
		[Export ("animationPlayerForKey:")]
		SCNAnimationPlayer GetAnimationPlayer (NSString key);
#endif

#if XAMCORE_2_0
		[Abstract]
#endif
		[Deprecated (PlatformName.TvOS, 11, 0,   message: "Use 'GetAnimationPlayer' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0,    message: "Use 'GetAnimationPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13,message: "Use 'GetAnimationPlayer' instead.")]
		[NoWatch]
		[Export ("animationForKey:")]
		CAAnimation GetAnimation (NSString key);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0,   message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0,    message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13,message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Mac (10,9)]
		[Export ("pauseAnimationForKey:")]
		void PauseAnimation (NSString key);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0,   message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0,    message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13,message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Mac (10,9)]
		[Export ("resumeAnimationForKey:")]
		void ResumeAnimation (NSString key);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Watch (3, 0)]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[TV (9, 0)]
		[Deprecated (PlatformName.TvOS, 11, 0,   message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[iOS (8, 0)]
		[Deprecated (PlatformName.iOS, 11, 0,    message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Mac (10, 9)]
		[Deprecated (PlatformName.MacOSX, 10, 13,message: "Use 'SCNAnimationPlayer.Paused' instead.")]
		[Export ("isAnimationForKeyPaused:")]
		bool IsAnimationPaused (NSString key);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'RemoveAnimationUsingBlendOutDuration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0,   message: "Use 'RemoveAnimationUsingBlendOutDuration' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0,    message: "Use 'RemoveAnimationUsingBlendOutDuration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13,message: "Use 'RemoveAnimationUsingBlendOutDuration' instead.")]
		[Mac (10,10)]
		[Export ("removeAnimationForKey:fadeOutDuration:")]
		void RemoveAnimation (NSString key, nfloat duration);

#if XAMCORE_4_0
		[Abstract]
#endif
		[return: NullAllowed]
		[iOS (11,0), TV (11,0), Watch (4,0), Mac (10,13, onlyOn64: true)]
		[Export ("removeAnimationForKey:blendOutDuration:")]
		void RemoveAnimationUsingBlendOutDuration (NSString key, nfloat blendOutDuration);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SCNAnimationPlayer.Speed' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0,   message: "Use 'SCNAnimationPlayer.Speed' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0,    message: "Use 'SCNAnimationPlayer.Speed' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13,message: "Use 'SCNAnimationPlayer.Speed' instead.")]
		[TV (10, 0), Mac (10, 12), iOS (10, 0), Watch (3,0)]
		[Export ("setSpeed:forAnimationKey:")]
		void SetSpeed (nfloat speed, NSString key);
	}

	[Watch (3,0)]
	[iOS(9,0),Mac(10,11)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface SCNAudioPlayer
	{
		[Export ("initWithSource:")]
		[DesignatedInitializer]
		IntPtr Constructor (SCNAudioSource source);
	
		[Export ("initWithAVAudioNode:")]
		[DesignatedInitializer]
		IntPtr Constructor (AVAudioNode audioNode);
	
		[Static]
		[Export ("audioPlayerWithSource:")]
		SCNAudioPlayer FromSource (SCNAudioSource source);
	
		[Static]
		[Export ("audioPlayerWithAVAudioNode:")]
		SCNAudioPlayer AVAudioNode (AVAudioNode audioNode);
	
		[Export ("willStartPlayback")]
		Action WillStartPlayback { get; set; }
	
		[Export ("didFinishPlayback")]
		Action DidFinishPlayback { get; set; }
	
		[NullAllowed, Export ("audioNode")]
		AVAudioNode AudioNode { get; }
	
		[NullAllowed, Export ("audioSource")]
		SCNAudioSource AudioSource { get; }
	}
	
	[Watch (3,0)]
	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface SCNAudioSource : NSCopying, NSSecureCoding
	{
		[Export ("initWithFileNamed:")]
		IntPtr Constructor (string filename);
	
		[Export ("initWithURL:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl url);
	
		[Static]
		[Export ("audioSourceNamed:")]
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
		

	[Watch (3,0)]
	[iOS (8,0)]
	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	interface SCNBoundingVolume {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("getBoundingBoxMin:max:")]
		bool GetBoundingBox (ref SCNVector3 min, ref SCNVector3 max);

		[Mac (10,9)] // Yep, Apple broke backwards compatibility in 10.9 by introducing a new required member.
		[Abstract]
		[Export ("setBoundingBoxMin:max:")]
		void SetBoundingBox (ref SCNVector3 min, ref SCNVector3 max);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("getBoundingSphereCenter:radius:")]
		bool GetBoundingSphere (ref SCNVector3 center, ref nfloat radius);
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNBox {
		[Export ("width")]
		nfloat Width { get; set;  }

		[Export ("height")]
		nfloat Height { get; set;  }

		[Export ("length")]
		nfloat Length { get; set;  }

		[Export ("chamferRadius")]
		nfloat ChamferRadius { get; set;  }

		[Export ("widthSegmentCount")]
		nint WidthSegmentCount { get; set;  }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set;  }

		[Export ("lengthSegmentCount")]
		nint LengthSegmentCount { get; set;  }

		[Export ("chamferSegmentCount")]
		nint ChamferSegmentCount { get; set;  }

		[Static, Export ("boxWithWidth:height:length:chamferRadius:")]
		SCNBox Create (nfloat width, nfloat height, nfloat length, nfloat chamferRadius);
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface SCNCamera : SCNAnimatable, SCNTechniqueSupport, NSCopying, NSSecureCoding {
		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set;  }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.iOS,    11, 0,  message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.TvOS,   11, 0,  message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0,  message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Export ("xFov")]
		double XFov { get; set;  }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.iOS,    11, 0,  message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.TvOS,   11, 0,  message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0,  message: "Use 'FieldOfView' or 'FocalLength' instead.")]
		[Export ("yFov")]
		double YFov { get; set;  }

		[Export ("zNear")]
		double ZNear { get; set;  }

		[Export ("zFar")]
		double ZFar { get; set;  }

		[Export ("usesOrthographicProjection")]
		bool UsesOrthographicProjection { get; set;  }

		[Static, Export ("camera")]
		SCNCamera Create ();

		[iOS (8,0)][Mac (10,9)]
		[Export ("projectionTransform")]
		SCNMatrix4 ProjectionTransform { get; [Mac (10,9)] set; }

		[iOS (8,0)][Mac (10,9)]
		[Export ("automaticallyAdjustsZRange")]
		bool AutomaticallyAdjustsZRange { get; set; }
		
		[iOS (8,0)][Mac (10,9)]
		[Export ("orthographicScale")]
		double OrthographicScale { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FocusDistance' instead.")]
		[Deprecated (PlatformName.iOS,    11, 0,  message: "Use 'FocusDistance' instead.")]
		[Deprecated (PlatformName.TvOS,   11, 0,  message: "Use 'FocusDistance' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0,  message: "Use 'FocusDistance' instead.")]
		[iOS (8,0)][Mac (10,9)]
		[Export ("focalDistance")]
		nfloat FocalDistance { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FocusDistance' instead.")]
		[Deprecated (PlatformName.iOS,    11, 0,  message: "Use 'FocusDistance' instead.")]
		[Deprecated (PlatformName.TvOS,   11, 0,  message: "Use 'FocusDistance' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0,  message: "Use 'FocusDistance' instead.")]
		[iOS (8,0)][Mac (10,9)]
		[Export ("focalSize")]
		nfloat FocalSize { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FStop' instead.")]
		[Deprecated (PlatformName.iOS,    11, 0,  message: "Use 'FStop' instead.")]
		[Deprecated (PlatformName.TvOS,   11, 0,  message: "Use 'FStop' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0,  message: "Use 'FStop' instead.")]
		[iOS (8,0)][Mac (10,9)]
		[Export ("focalBlurRadius")]
		nfloat FocalBlurRadius { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'FStop' instead with FStop = SensorHeight / Aperture.")]
		[Deprecated (PlatformName.iOS,    11, 0,  message: "Use 'FStop' instead with FStop = SensorHeight / Aperture.")]
		[Deprecated (PlatformName.TvOS,   11, 0,  message: "Use 'FStop' instead with FStop = SensorHeight / Aperture.")]
		[Deprecated (PlatformName.WatchOS, 4, 0,  message: "Use 'FStop' instead with FStop = SensorHeight / Aperture.")]
		[iOS (8,0)][Mac (10,9)]
		[Export ("aperture")]
		nfloat Aperture { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("motionBlurIntensity")]
		nfloat MotionBlurIntensity { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("wantsHDR")]
		bool WantsHdr { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("exposureOffset")]
		nfloat ExposureOffset { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("averageGray")]
		nfloat AverageGray { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("whitePoint")]
		nfloat WhitePoint { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("wantsExposureAdaptation")]
		bool WantsExposureAdaptation { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("exposureAdaptationBrighteningSpeedFactor")]
		nfloat ExposureAdaptationBrighteningSpeedFactor { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("exposureAdaptationDarkeningSpeedFactor")]
		nfloat ExposureAdaptationDarkeningSpeedFactor { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("minimumExposure")]
		nfloat MinimumExposure { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("maximumExposure")]
		nfloat MaximumExposure { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("bloomThreshold")]
		nfloat BloomThreshold { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("bloomIntensity")]
		nfloat BloomIntensity { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("bloomBlurRadius")]
		nfloat BloomBlurRadius { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("vignettingPower")]
		nfloat VignettingPower { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("vignettingIntensity")]
		nfloat VignettingIntensity { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("colorFringeStrength")]
		nfloat ColorFringeStrength { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("colorFringeIntensity")]
		nfloat ColorFringeIntensity { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("saturation")]
		nfloat Saturation { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("contrast")]
		nfloat Contrast { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("colorGrading")]
		SCNMaterialProperty ColorGrading { get; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("categoryBitMask")]
		nuint CategoryBitMask { get; set; }

#if XAMCORE_2_0
		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("cameraWithMDLCamera:")]
		SCNCamera FromModelCamera (MDLCamera modelCamera);

		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("fieldOfView")]
		nfloat FieldOfView { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("projectionDirection", ArgumentSemantic.Assign)]
		SCNCameraProjectionDirection ProjectionDirection { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("focalLength")]
		nfloat FocalLength { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("sensorHeight")]
		nfloat SensorHeight { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("wantsDepthOfField")]
		bool WantsDepthOfField { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("focusDistance")]
		nfloat FocusDistance { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("focalBlurSampleCount")]
		nint FocalBlurSampleCount { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("fStop")]
		nfloat FStop { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("apertureBladeCount")]
		nint ApertureBladeCount { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("screenSpaceAmbientOcclusionIntensity")]
		nfloat ScreenSpaceAmbientOcclusionIntensity { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("screenSpaceAmbientOcclusionRadius")]
		nfloat ScreenSpaceAmbientOcclusionRadius { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("screenSpaceAmbientOcclusionBias")]
		nfloat ScreenSpaceAmbientOcclusionBias { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("screenSpaceAmbientOcclusionDepthThreshold")]
		nfloat ScreenSpaceAmbientOcclusionDepthThreshold { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("screenSpaceAmbientOcclusionNormalThreshold")]
		nfloat ScreenSpaceAmbientOcclusionNormalThreshold { get; set; }

#endif
	}

	interface ISCNCameraControlConfiguration {}

	[NoWatch]
	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[Protocol]
	interface SCNCameraControlConfiguration
	{
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

	interface ISCNCameraControllerDelegate {}
	
	[Watch (4,0), TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[Protocol]
	[Model] // Figured I would keep the model for convenience, as all the methods here are optional
	[BaseType (typeof(NSObject))]
	interface SCNCameraControllerDelegate
	{
		[Export ("cameraInertiaWillStartForController:")]
		void CameraInertiaWillStart (SCNCameraController cameraController);
	
		[Export ("cameraInertiaDidEndForController:")]
		void CameraInertiaDidEnd (SCNCameraController cameraController);
	}
	
	[Watch (4,0), TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface SCNCameraController
	{
		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		[Protocolize]
		SCNCameraControllerDelegate Delegate { get; set; }
	
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
		void FrameNodes (SCNNode[] nodes);
	
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

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNCapsule {
		[Export ("capRadius")]
		nfloat CapRadius { get; set;  }

		[Export ("height")]
		nfloat Height { get; set;  }

		[Export ("radialSegmentCount")]
		nint RadialSegmentCount { get; set;  }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set;  }

		[Export ("capSegmentCount")]
		nint CapSegmentCount { get; set;  }

		[Static, Export ("capsuleWithCapRadius:height:")]
		SCNCapsule Create (nfloat capRadius, nfloat height);
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNCone {
		[Export ("topRadius")]
		nfloat TopRadius { get; set;  }

		[Export ("bottomRadius")]
		nfloat BottomRadius { get; set;  }

		[Export ("height")]
		nfloat Height { get; set;  }

		[Export ("radialSegmentCount")]
		nint RadialSegmentCount { get; set;  }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set;  }

		[Static, Export ("coneWithTopRadius:bottomRadius:height:")]
		SCNCone Create (nfloat topRadius, nfloat bottomRadius, nfloat height);
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNCylinder {
		[Export ("radius")]
		nfloat Radius { get; set;  }

		[Export ("height")]
		nfloat Height { get; set;  }

		[Export ("radialSegmentCount")]
		nint RadialSegmentCount { get; set;  }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set;  }

		[Static, Export ("cylinderWithRadius:height:")]
		SCNCylinder Create (nfloat radius, nfloat height);
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNFloor {
		[Export ("reflectivity")]
		nfloat Reflectivity { get; set;  }

		[Export ("reflectionFalloffStart")]
		nfloat ReflectionFalloffStart { get; set;  }

		[Export ("reflectionFalloffEnd")]
		nfloat ReflectionFalloffEnd { get; set;  }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("reflectionCategoryBitMask")]
		nuint ReflectionCategoryBitMask { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("width")]
		nfloat Width { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("length")]
		nfloat Length { get; set; }

		[Mac (10,10)]
		[Export ("reflectionResolutionScaleFactor")]
		nfloat ReflectionResolutionScaleFactor { get; set; }

		[Static, Export ("floor")]
		SCNFloor Create ();
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] 
	interface SCNGeometry : SCNAnimatable, SCNBoundingVolume, SCNShadable, NSCopying, NSSecureCoding {
		[Export ("materials", ArgumentSemantic.Copy)]
		SCNMaterial [] Materials { get; set;  }

		[Export ("geometryElementCount")]
		nint GeometryElementCount { get;  }

		[Export ("firstMaterial", ArgumentSemantic.Retain)]
		SCNMaterial FirstMaterial { get; set;  }

		[iOS (9,0)][Mac(10,11)] // header mistake (10,10) as tests shows it does not exists
		[Export ("geometryElements")]
		SCNGeometryElement [] GeometryElements { get; }

		[iOS (9,0)][Mac(10,11)] // header mistake (10,10) as tests shows it does not exists
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
		SCNMaterial GetMaterial (string name);

		[Static]
		[Export ("geometryWithSources:elements:")]
		SCNGeometry Create (SCNGeometrySource [] sources, [NullAllowed] SCNGeometryElement [] elements);
	
		[Export ("geometrySourcesForSemantic:")]
		SCNGeometrySource [] GetGeometrySourcesForSemantic (string semantic);

		[Export ("geometryElementAtIndex:")]
		SCNGeometryElement GetGeometryElement (nint elementIndex);

		[Mac (10,9)]
		[NullAllowed] // by default this property is null
		[Export ("levelsOfDetail", ArgumentSemantic.Copy)]
		SCNLevelOfDetail [] LevelsOfDetail { get; set; }

		[Mac (10,9)]
		[Static, Export ("geometry")]
		SCNGeometry Create ();

		[Mac (10,10)]
		[Export ("subdivisionLevel")]
		nuint SubdivisionLevel { get; set; }

		[Mac (10,10)]
		[NullAllowed] // by default this property is null
		[Export ("edgeCreasesElement", ArgumentSemantic.Retain)]
		SCNGeometryElement EdgeCreasesElement { get; set; }

		[Mac (10,10)]
		[NullAllowed] // by default this property is null
		[Export ("edgeCreasesSource", ArgumentSemantic.Retain)]
		SCNGeometrySource EdgeCreasesSource { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("wantsAdaptiveSubdivision")]
		bool WantsAdaptiveSubdivision { get; set; }

#if XAMCORE_2_0
		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("geometryWithMDLMesh:")]
		SCNGeometry FromMesh (MDLMesh mesh);

		[NoWatch, Mac (10,13, onlyOn64: true), iOS (11,0)]
		[TV (12,0)]
		[NullAllowed, Export ("tessellator", ArgumentSemantic.Retain)]
		SCNGeometryTessellator Tessellator { get; set; }
#endif
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface SCNGeometrySource : NSSecureCoding {
		[Export ("data")]
		NSData Data { get;  }

		[Export ("semantic")]
		NSString Semantic { get;  }

		[Export ("vectorCount")]
		nint VectorCount { get;  }

		[Export ("floatComponents")]
		bool FloatComponents { get;  }

		[Export ("componentsPerVector")]
		nint ComponentsPerVector { get;  }

		[Export ("bytesPerComponent")]
		nint BytesPerComponent { get;  }

		[Export ("dataOffset")]
		nint DataOffset { get;  }

		[Export ("dataStride")]
		nint DataStride { get;  }

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

#if XAMCORE_2_0 || !MONOMAC
		[NoWatch]
		[iOS (9,0)][Mac (10,11)]
		[Static]
		[Export ("geometrySourceWithBuffer:vertexFormat:semantic:vertexCount:dataOffset:dataStride:")]
		SCNGeometrySource FromMetalBuffer (IMTLBuffer mtlBuffer, MTLVertexFormat vertexFormat, NSString geometrySourceSemantic, nint vertexCount, nint offset, nint stride);
#endif
	}

	[Watch (3,0)]
	[iOS (8,0)]
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

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("SCNGeometrySourceSemanticTangent")]
		NSString Tangent { get; }

		[Mac (10,10)]
		[Field ("SCNGeometrySourceSemanticVertexCrease")]
		NSString VertexCrease { get; }

		[Mac (10,10)]
		[Field ("SCNGeometrySourceSemanticEdgeCrease")]
		NSString EdgeCrease { get; }

		[Mac (10,10)]
		[Field ("SCNGeometrySourceSemanticBoneWeights")]
		NSString BoneWeights { get; }

		[Mac (10,10)]
		[Field ("SCNGeometrySourceSemanticBoneIndices")]
		NSString BoneIndices { get; }
	}
	
	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface SCNGeometryElement : NSSecureCoding {
		[Export ("data")]
		NSData Data { get;  }

		[Export ("primitiveType")]
		SCNGeometryPrimitiveType PrimitiveType { get;  }

		[Export ("primitiveCount")]
		nint PrimitiveCount { get;  }

		[Export ("bytesPerIndex")]
		nint BytesPerIndex { get;  }

		[Static]
		[Export ("geometryElementWithData:primitiveType:primitiveCount:bytesPerIndex:")]
		SCNGeometryElement FromData ([NullAllowed] NSData data, SCNGeometryPrimitiveType primitiveType, nint primitiveCount, nint bytesPerIndex);

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("primitiveRange", ArgumentSemantic.Assign)]
		NSRange PrimitiveRange { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("pointSize")]
		nfloat PointSize { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("minimumPointScreenSpaceRadius")]
		nfloat MinimumPointScreenSpaceRadius { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("maximumPointScreenSpaceRadius")]
		nfloat MaximumPointScreenSpaceRadius { get; set; }

#if XAMCORE_2_0
		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("geometryElementWithMDLSubmesh:")]
		SCNGeometryElement FromSubmesh (MDLSubmesh submesh);
#endif
	}

#if XAMCORE_2_0 && !WATCH
	[NoWatch, Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[TV (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface SCNGeometryTessellator : NSCopying, NSSecureCoding
	{
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
#endif
	
	[Watch (3,0)]
	[iOS (8,0)]
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

		[Mac (10,9), iOS (8,0)]
		[Field ("SCNHitTestIgnoreHiddenNodesKey")]
		NSString IgnoreHiddenNodesKey { get; }

#if !XAMCORE_2_0 // Less preferred name, but let's not break stable API
		[Mac (10,9), iOS (8,0)]
		[Obsolete ("Use IgnoreHiddenNodesKey")]
		[Field ("SCNHitTestIgnoreHiddenNodesKey")]
		NSString IgnoreHiddenNodes { get; }
#endif
		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("SCNHitTestOptionCategoryBitMask")]
		NSString OptionCategoryBitMaskKey { get; }

#if !XAMCORE_4_0
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Obsolete ("Use 'SearchModeKey' instead.")]
		[Field ("SCNHitTestOptionSearchMode")]
		NSString OptionSearchModeKey { get; }
#endif

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Field ("SCNHitTestOptionSearchMode")]
		NSString SearchModeKey { get; }
	}
	
	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	// quote: The SCNHitTestResult class is exposed as the return object from the hitTest:options: method, as an array of SCNHitTestResult objects.
	[DisableDefaultCtor] // crash when calling description
	interface SCNHitTestResult {
		[Export ("geometryIndex")]
		nint GeometryIndex { get;  }

		[Export ("faceIndex")]
		nint FaceIndex { get;  }

		[Export ("localCoordinates")]
		SCNVector3 LocalCoordinates { get;  }

		[Export ("worldCoordinates")]
		SCNVector3 WorldCoordinates { get;  }

		[Export ("localNormal")]
		SCNVector3 LocalNormal { get;  }

		[Export ("worldNormal")]
		SCNVector3 WorldNormal { get;  }

		[Export ("modelTransform")]
		SCNMatrix4 ModelTransform { get;  }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("boneNode")]
		SCNNode BoneNode { get; }

		[Export ("node")]
		SCNNode Node { get; }

		[Export ("textureCoordinatesWithMappingChannel:")]
		CGPoint GetTextureCoordinatesWithMappingChannel (nint channel);
	}

#if MONOMAC
	[iOS (8,0)]
	[BaseType (typeof (CAOpenGLLayer))]
	interface SCNLayer : SCNSceneRenderer, SCNTechniqueSupport {
//		We already pull in the Scene property from the SCNSceneRenderer protocol, no need to redefine it here.
//		[Export ("scene", ArgumentSemantic.Retain)]
//		SCNScene Scene { get; set;  }
	}
#endif

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface SCNLight : SCNAnimatable, SCNTechniqueSupport, NSCopying, NSSecureCoding {
		[Export ("type", ArgumentSemantic.Copy)]
		NSString LightType { get; set;  }

		[Export ("color", ArgumentSemantic.Retain)]
		NSObject WeakColor { get; set; }

		[Wrap ("WeakColor")]
		NSColor Color { get; set;  }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("temperature")]
		nfloat Temperature { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("intensity")]
		nfloat Intensity { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set;  }

		[Export ("castsShadow")]
		bool CastsShadow { get; set;  }

		[Export ("shadowColor", ArgumentSemantic.Retain)]
		NSObject WeakShadowColor { get; set;  }

		[Wrap ("WeakShadowColor")]
		NSColor ShadowColor { get; set; }

		[Mac (10,10)]
		[Export ("shadowBias")]
		nfloat ShadowBias { get; set; }

		[Export ("shadowRadius")]
		nfloat ShadowRadius { get; set;  }

		[Static, Export ("light")]
		SCNLight Create ();

#if XAMCORE_3_0
		[Availability (Unavailable = Platform.iOS_Version)]
#elif !MONOMAC
		[Obsolete ("Do not use; this method only exist in macOS, not in iOS.")]
#endif
		[NoTV]
		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("attributeForKey:")]
		NSObject GetAttribute (NSString lightAttribute);

#if XAMCORE_3_0
		[Availability (Unavailable = Platform.iOS_Version)]
#elif !MONOMAC
		[Obsolete ("Do not use; this method only exist in macOS, not in iOS.")]
#endif
		[NoTV]
		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("setAttribute:forKey:")]
		void SetAttribute (NSObject value, NSString attribuetKey);

		[Mac (10,9)]
		[Export ("gobo")]
		SCNMaterialProperty Gobo { get; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[NullAllowed, Export ("IESProfileURL", ArgumentSemantic.Retain)]
		NSUrl IesProfileUrl { get; set; }

		[Mac (10,10)]
		[Export ("shadowMapSize")]
		CGSize ShadowMapSize { get; set; }

		[Mac (10,10)]
		[Export ("shadowSampleCount")]
		nuint ShadowSampleCount { get; set; }

		[Mac (10,10)]
		[Export ("shadowMode")]
		SCNShadowMode ShadowMode { get; set; }

		[Mac (10,10)]
		[Export ("attenuationStartDistance")]
		nfloat AttenuationStartDistance { get; set; }

		[Mac (10,10)]
		[Export ("attenuationEndDistance")]
		nfloat AttenuationEndDistance { get; set; }

		[Mac (10,10)]
		[Export ("attenuationFalloffExponent")]
		nfloat AttenuationFalloffExponent { get; set; }

		[Mac (10,10)]
		[Export ("spotInnerAngle")]
		nfloat SpotInnerAngle { get; set; }

		[Mac (10,10)]
		[Export ("spotOuterAngle")]
		nfloat SpotOuterAngle { get; set; }

		[Mac (10,10)]
		[Export ("orthographicScale")]
		nfloat OrthographicScale { get; set; }

		[Mac (10,10)]
		[Export ("zNear")]
		nfloat ZNear { get; set; }

		[Mac (10,10)]
		[Export ("zFar")]
		nfloat ZFar { get; set; }

		[Mac (10,10), iOS (8,0)]
		[Export ("categoryBitMask")]
		nuint CategoryBitMask { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("automaticallyAdjustsShadowProjection")]
		bool AutomaticallyAdjustsShadowProjection { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("maximumShadowDistance")]
		nfloat MaximumShadowDistance { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("forcesBackFaceCasters")]
		bool ForcesBackFaceCasters { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("sampleDistributedShadowMaps")]
		bool SampleDistributedShadowMaps { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("shadowCascadeCount")]
		nuint ShadowCascadeCount { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("shadowCascadeSplittingFactor")]
		nfloat ShadowCascadeSplittingFactor { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("sphericalHarmonicsCoefficients", ArgumentSemantic.Copy)]
		NSData SphericalHarmonicsCoefficients { get; }

#if XAMCORE_2_0
		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("lightWithMDLLight:")]
		SCNLight FromModelLight (MDLLight mdllight);
#endif
	}

	[Watch (3,0)]
	[iOS (8,0)]
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

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("SCNLightTypeIES")]
		NSString Ies { get; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("SCNLightTypeProbe")]
		NSString Probe { get; }
	}

#if MONOMAC
	[Availability (Deprecated = Platform.Mac_10_10)]
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
#endif

	[Watch (3,0)]
	[iOS (8,0)]
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

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("SCNLightingModelPhysicallyBased")]
		NSString PhysicallyBased { get; }
	}
	
	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface SCNMaterial : SCNAnimatable, SCNShadable, NSCopying, NSSecureCoding {
		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set;  }

		[Export ("diffuse")]
		SCNMaterialProperty Diffuse { get;  }

		[Export ("ambient")]
		SCNMaterialProperty Ambient { get;  }

		[Export ("specular")]
		SCNMaterialProperty Specular { get;  }

		[Export ("emission")]
		SCNMaterialProperty Emission { get;  }

		[Export ("transparent")]
		SCNMaterialProperty Transparent { get;  }

		[Export ("reflective")]
		SCNMaterialProperty Reflective { get;  }

		[Export ("multiply")]
		SCNMaterialProperty Multiply { get;  }

		[Export ("normal")]
		SCNMaterialProperty Normal { get;  }

		[Export ("shininess")]
		nfloat Shininess { get; set;  }

		[Export ("transparency")]
		nfloat Transparency { get; set;  }

		[Export ("lightingModelName", ArgumentSemantic.Copy)]
		NSString LightingModelName { get; set;  }

		[Export ("litPerPixel")]
		bool LitPerPixel { [Bind ("isLitPerPixel")] get; set;  }

		[Export ("doubleSided")]
		bool DoubleSided { [Bind ("isDoubleSided")] get; set;  }

		[Export ("cullMode")]
		SCNCullMode CullMode { get; set;  }

		[Export ("transparencyMode")]
		SCNTransparencyMode TransparencyMode { get; set;  }

		[Export ("locksAmbientWithDiffuse")]
		bool LocksAmbientWithDiffuse { get; set;  }

		[Export ("writesToDepthBuffer")]
		bool WritesToDepthBuffer { get; set;  }

		[Static, Export ("material")]
		SCNMaterial Create ();

		[Mac (10,9)]
		[Export ("readsFromDepthBuffer")]
		bool ReadsFromDepthBuffer  { get; set; }

		[Mac (10,9)]
		[Export ("fresnelExponent")]
		nfloat FresnelExponent { get; set; }

		[iOS (9,0), Mac(10,11)]
		[Export ("blendMode", ArgumentSemantic.Assign)]
		SCNBlendMode BlendMode { get; set; }

		[iOS (9,0), Mac(10,11)]
		[Export ("ambientOcclusion")]
		SCNMaterialProperty AmbientOcclusion { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("selfIllumination")]
		SCNMaterialProperty SelfIllumination { get; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("metalness")]
		SCNMaterialProperty Metalness { get; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("roughness")]
		SCNMaterialProperty Roughness { get; }

#if XAMCORE_2_0
		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("materialWithMDLMaterial:")]
		SCNMaterial FromMaterial (MDLMaterial material);

		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("displacement")]
		SCNMaterialProperty Displacement { get; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("fillMode", ArgumentSemantic.Assign)]
		SCNFillMode FillMode { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("colorBufferWriteMask", ArgumentSemantic.Assign)]
		SCNColorMask ColorBufferWriteMask { get; set; }

#endif
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // runtime -> [SCNKit ERROR] Do not instantiate SCNMaterialProperty objects directly
	interface SCNMaterialProperty : SCNAnimatable, NSSecureCoding {
		[Export ("minificationFilter")]
		SCNFilterMode MinificationFilter { get; set;  }

		[Export ("magnificationFilter")]
		SCNFilterMode MagnificationFilter { get; set;  }

		[Export ("mipFilter")]
		SCNFilterMode MipFilter { get; set;  }

		[Export ("contentsTransform")]
		SCNMatrix4 ContentsTransform { get; set;  }

		[Export ("wrapS")]
		SCNWrapMode WrapS { get; set;  }

		[Export ("wrapT")]
		SCNWrapMode WrapT { get; set;  }

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoWatch, NoTV]
		[NullAllowed, Export ("borderColor", ArgumentSemantic.Retain)]
		NSObject BorderColor { get; set; }

		[Export ("mappingChannel")]
		nint MappingChannel { get; set;  }

		[Export ("contents", ArgumentSemantic.Retain), NullAllowed]
		NSObject Contents { get; set; }

		[Wrap ("Contents")]
		NSColor ContentColor { get; set; }

		[Wrap ("Contents")]
		NSImage ContentImage { get; set; }

		[NoWatch]
		[Wrap ("Contents")]
		CALayer ContentLayer { get; set; }

		[Wrap ("Contents")]
		NSString ContentPath { get; set; }

		[Wrap ("Contents")]
		NSUrl ContentUrl { get; set; }

#if XAMCORE_2_0 || !MONOMAC
		[Wrap ("Contents")]
		SKScene ContentScene { get; set; }

		[Wrap ("Contents")]
		SKTexture ContentTexture { get; set; }
#endif

		[Wrap ("Contents")]
		NSImage [] ContentImageCube { get; set; }

		[Mac (10,9)]
		[Export ("intensity")]
		nfloat Intensity { get; set; }

		[Mac (10,9)]
		[Export ("maxAnisotropy")]
		nfloat MaxAnisotropy { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("textureComponents", ArgumentSemantic.Assign)]
		SCNColorMask TextureComponents { get; set; }

		[Mac (10,9)]
		[Static, Export ("materialPropertyWithContents:")]
		SCNMaterialProperty Create (NSObject contents);
	}

#if !WATCH
	[NoWatch]
	[StrongDictionary ("SCNProgram")]
	interface SCNProgramSemanticOptions {
		nuint MappingChannel { get; set; }
	}
#endif

	[Watch (3,0)]
	[StrongDictionary ("SCNHitTest")]
	interface SCNHitTestOptions {
		bool FirstFoundOnly { get; set; }
		bool SortResults { get; set; }
		bool BackFaceCulling { get; set; }
		bool BoundingBoxOnly { get; set; }
		bool IgnoreChildNodes { get; set; }
		bool IgnoreHiddenNodes { get; set; }
		SCNNode RootNode { get; set; }
		SCNHitTestSearchMode SearchMode { get; set; }
	}

	[Watch (3,0)]
	[StrongDictionary ("SCNSceneSourceLoading")]
	interface SCNSceneLoadingOptions {
		NSUrl [] AssetDirectoryUrls { get; set; }
		bool CreateNormalsIfAbsent { get; set; }
		bool FlattenScene { get; set; }
		bool CheckConsistency { get; set; }
		bool OverrideAssetUrls { get; set; }
		bool StrictConformance { get; set; }
		bool UseSafeMode { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("SCNSceneSourceLoading.OptionPreserveOriginalTopology")]
		bool PreserveOriginalTopology { get; set; }

#if !TVOS && !WATCH
		// note: generator's StrongDictionary does not support No* attributes yet
		[NoTV]
		[NoWatch]
		float ConvertUnitsToMeters { get; set; } /* 'floating value encapsulated in a NSNumber' probably a float since it's a graphics framework */
		[NoTV]
		[NoWatch]
		bool ConvertToYUp { get; set; }
#endif

		[Internal, Export ("SCNSceneSourceLoading.AnimationImportPolicyKey")]
		NSString _AnimationImportPolicyKey { get; set; }
	}

	[iOS (8,0)]
	delegate bool SCNNodePredicate (SCNNode node, out bool stop);

	[iOS (8, 0)]
	delegate void SCNNodeHandler (SCNNode node, out bool stop);

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface SCNNode : SCNAnimatable, SCNBoundingVolume, SCNActionable, NSCopying, NSSecureCoding 
#if IOS || TVOS
		, UIFocusItem
#endif
	{
		[Export ("transform")]
		SCNMatrix4 Transform { get; set;  }

		[Export ("position")]
		SCNVector3 Position { get; set;  }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("worldPosition", ArgumentSemantic.Assign)]
		SCNVector3 WorldPosition { get; set; }

		[Export ("rotation")]
		SCNVector4 Rotation { get; set;  }

		[Export ("scale")]
		SCNVector3 Scale { get; set;  }

		[Export ("pivot")]
		SCNMatrix4 Pivot { get; set;  }

		[Export ("worldTransform")]
		SCNMatrix4 WorldTransform { get; [Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)] set; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set;  }

		[Export ("opacity")]
		nfloat Opacity { get; set;  }

		[Export ("renderingOrder")]
		nint RenderingOrder { get; set;  }

		[Export ("parentNode")]
		SCNNode ParentNode { get;  }

		[Export ("childNodes")]
		SCNNode [] ChildNodes { get;  }

		[Export ("light", ArgumentSemantic.Retain)]
		[NullAllowedAttribute]
		SCNLight Light { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("camera", ArgumentSemantic.Retain)]
		SCNCamera Camera { get; set;  }

		[Export ("geometry", ArgumentSemantic.Retain)]
		[NullAllowed]
		SCNGeometry Geometry { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set;  }

		[NoWatch]
		[Export ("rendererDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakRendererDelegate { get; set;  }

		[NoWatch]
		[Wrap ("WeakRendererDelegate")]
		[Protocolize]
		SCNNodeRendererDelegate RendererDelegate { get; set; }

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
		SCNNode FindChildNode (string childName, bool recursively);

		[Export ("childNodesPassingTest:")]
		SCNNode [] FindNodes (SCNNodePredicate predicate);

		[Export ("clone")]
		SCNNode Clone ();

		[Mac (10,9)]
		[NullAllowed] // by default this property is null
		[Export ("skinner", ArgumentSemantic.Retain)]
		SCNSkinner Skinner { get; set; }

		[Mac (10,9)]
		[NullAllowed] // by default this property is null
		[Export ("morpher", ArgumentSemantic.Retain)]
		SCNMorpher Morpher { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("worldOrientation", ArgumentSemantic.Assign)]
		SCNQuaternion WorldOrientation { get; set; }

		[Mac (10,10)]
		[Export ("orientation")]
		SCNQuaternion Orientation { get; set; }

		[Mac (10,9)]
		[Export ("constraints", ArgumentSemantic.Copy)]
		[NullAllowed]
		SCNConstraint [] Constraints { get; set; }

		[NoWatch, Mac (10, 9)]
		[NullAllowed] // by default this property is null
		[Export ("filters", ArgumentSemantic.Copy)]
		CIFilter [] Filters { get; set; }

		[Mac (10,9)]
		[Export ("flattenedClone")]
		SCNNode FlattenedClone ();

		[Mac (10,9)]
		[Export ("convertPosition:toNode:")]
		SCNVector3 ConvertPositionToNode (SCNVector3 position, [NullAllowed] SCNNode node);

		[Mac (10,9)]
		[Export ("convertPosition:fromNode:")]
		SCNVector3 ConvertPositionFromNode (SCNVector3 position, [NullAllowed] SCNNode node);

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("convertVector:toNode:")]
		SCNVector3 ConvertVectorToNode (SCNVector3 vector, [NullAllowed] SCNNode node);

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("convertVector:fromNode:")]
		SCNVector3 ConvertVectorFromNode (SCNVector3 vector, [NullAllowed] SCNNode node);

		[Mac (10,9)]
		[Export ("convertTransform:fromNode:")]
		SCNMatrix4 ConvertTransformFromNode (SCNMatrix4 transform, [NullAllowed] SCNNode node);

		[Mac (10,9)]
		[Export ("convertTransform:toNode:")]
		SCNMatrix4 ConvertTransformToNode (SCNMatrix4 transform, [NullAllowed] SCNNode node);

		[Mac (10,9)]
		[Export ("hitTestWithSegmentFromPoint:toPoint:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNHitTestResult [] HitTest (SCNVector3 pointA, SCNVector3 pointB, [NullAllowed] NSDictionary options);

		[Wrap ("HitTest (pointA, pointB, options == null ? null : options.Dictionary)")]
		SCNHitTestResult [] HitTest (SCNVector3 pointA, SCNVector3 pointB, SCNHitTestOptions options);

		[Mac (10,10)]
		[Export ("eulerAngles")]
		SCNVector3 EulerAngles { get; set; }

		[Mac (10,10)]
		[Export ("castsShadow")]
		bool CastsShadow { get; set; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("movabilityHint", ArgumentSemantic.Assign)]
		SCNMovabilityHint MovabilityHint { get; set; }

		[Mac (10,10, onlyOn64 : true)] // More broken 32-bit code, 17710842
		[Export ("physicsBody", ArgumentSemantic.Retain), NullAllowed]
		SCNPhysicsBody PhysicsBody { get; set; }

		[Mac (10,10)]
		[NullAllowed] // by default this property is null
		[Export ("physicsField", ArgumentSemantic.Retain)]
		SCNPhysicsField PhysicsField { get; set; }

		[Mac (10,10)]
		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; set; }

		[Mac (10,10)]
		[Export ("categoryBitMask")]
		nuint CategoryBitMask { get; set; }

		[Mac (10,10)]
		[Export ("enumerateChildNodesUsingBlock:")]
		void EnumerateChildNodes (SCNNodeHandler handler);

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("enumerateHierarchyUsingBlock:")]
		void EnumerateHierarchy (SCNNodeHandler handler);

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("focusBehavior", ArgumentSemantic.Assign)]
		SCNNodeFocusBehavior FocusBehavior { get; set; }

		#region SCNParticleSystemSupport (SCNNode) category

		[Mac (10,10)]
		[Export ("particleSystems")]
		SCNParticleSystem [] ParticleSystems { get; }

		[Mac (10,10)]
		[Export ("addParticleSystem:")]
		void AddParticleSystem (SCNParticleSystem system);

		[Mac (10,10)]
		[Export ("removeAllParticleSystems")]
		void RemoveAllParticleSystems ();

		[Mac (10,10)]
		[Export ("removeParticleSystem:")]
		void RemoveParticleSystem (SCNParticleSystem system);

		#endregion

		#region SCNAudioSupport (SCNNode) category

		[iOS (9,0), Mac(10,11)]
		[Export ("audioPlayers")]
		SCNAudioPlayer [] AudioPlayers { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("addAudioPlayer:")]
		void AddAudioPlayer (SCNAudioPlayer player);

		[iOS (9,0), Mac(10,11)]
		[Export ("removeAllAudioPlayers")]
		void RemoveAllAudioPlayers ();

		[iOS (9,0), Mac(10,11)]
		[Export ("removeAudioPlayer:")]
		void RemoveAudioPlayer (SCNAudioPlayer player);

		#endregion

#if XAMCORE_2_0
		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("nodeWithMDLObject:")]
		SCNNode FromModelObject (MDLObject mdlObject);
#endif

		// From SCNNode (Transforms) Category

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("localUp")]
		SCNVector3 LocalUp { get; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("localRight")]
		SCNVector3 LocalRight { get; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("localFront")]
		SCNVector3 LocalFront { get; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("worldUp")]
		SCNVector3 WorldUp { get; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("worldRight")]
		SCNVector3 WorldRight { get; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("worldFront")]
		SCNVector3 WorldFront { get; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("lookAt:")]
		void Look (SCNVector3 worldTarget);

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("lookAt:up:localFront:")]
		void Look (SCNVector3 worldTarget, SCNVector3 worldUp, SCNVector3 localFront);

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("localTranslateBy:")]
		void LocalTranslate (SCNVector3 translation);

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("localRotateBy:")]
		void LocalRotate (SCNQuaternion rotation);

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
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

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdTransform", ArgumentSemantic.Assign)]
		//NMatrix4 SimdTransform {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdPosition", ArgumentSemantic.Assign)]
		//NVector3 SimdPosition {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdRotation", ArgumentSemantic.Assign)]
		//Vector4 SimdRotation {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdOrientation", ArgumentSemantic.Assign)]
		//Quaternion SimdOrientation {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdEulerAngles", ArgumentSemantic.Assign)]
		//NVector3 SimdEulerAngles {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdScale", ArgumentSemantic.Assign)]
		//NVector3 SimdScale {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdPivot", ArgumentSemantic.Assign)]
		//NMatrix4 SimdPivot {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdWorldPosition", ArgumentSemantic.Assign)]
		//NVector3 SimdWorldPosition {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdWorldOrientation", ArgumentSemantic.Assign)]
		//Quaternion SimdWorldOrientation {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdWorldTransform", ArgumentSemantic.Assign)]
		//NMatrix4 SimdWorldTransform {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	set;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdConvertPosition:toNode:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//NVector3 ConvertPositionToNode (NVector3 position, [NullAllowed] SCNNode node);

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdConvertPosition:fromNode:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//NVector3 ConvertPositionFromNode (NVector3 position, [NullAllowed] SCNNode node);

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdConvertVector:toNode:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//NVector3 ConvertVectorToNode (NVector3 vector, [NullAllowed] SCNNode node);

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdConvertVector:fromNode:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//NVector3 ConvertVectorFromNode (NVector3 vector, [NullAllowed] SCNNode node);

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdConvertTransform:toNode:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//NMatrix4 ConvertTransformToNode (NMatrix4 transform, [NullAllowed] SCNNode node);

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdConvertTransform:fromNode:")]
		//NMatrix4 ConvertTransformFromNode (NMatrix4 transform, [NullAllowed] SCNNode node);

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Static]
		//[Export ("simdLocalUp")]
		//NVector3 SimdLocalUp {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Static]
		//[Export ("simdLocalRight")]
		//NVector3 SimdLocalRight {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Static]
		//[Export ("simdLocalFront")]
		//NVector3 SimdLocalFront {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdWorldUp")]
		//NVector3 SimdWorldUp {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdWorldRight")]
		//NVector3 SimdWorldRight {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdWorldFront")]
		//NVector3 SimdWorldFront {
		//	[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//	get;
		//}

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdLookAt:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//void Look (NVector3 worldTarget);

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdLookAt:up:localFront:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//void Look (NVector3 worldTarget, NVector3 worldUp, NVector3 localFront);

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdLocalTranslateBy:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//void LocalTranslate (NVector3 translation);

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdLocalRotateBy:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//void LocalRotate (Quaternion rotation);

		//[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		//[Export ("simdRotateBy:aroundTarget:")]
		//[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		//void Rotate (Quaternion worldRotation, NVector3 worldTarget);
	}

	[NoWatch]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[Model, Protocol]
	interface SCNNodeRendererDelegate {
		[Export ("renderNode:renderer:arguments:")]
		void Render (SCNNode node, SCNRenderer renderer, [NullAllowed] NSDictionary arguments);
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNPlane {
		[Export ("width")]
		nfloat Width { get; set;  }

		[Export ("height")]
		nfloat Height { get; set;  }

		[Export ("widthSegmentCount")]
		nint WidthSegmentCount { get; set;  }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set;  }

		[Static, Export ("planeWithWidth:height:")]
		SCNPlane Create (nfloat width, nfloat height);

		[Mac (10,9)]
		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }

		[Mac (10,9)]
		[Export ("cornerSegmentCount")]
		nint CornerSegmentCount { get; set; }
	}

	delegate void SCNBufferBindingHandler (ISCNBufferStream buffer, SCNNode node, SCNShadable shadable, SCNRenderer renderer);

	[iOS (8,0), NoWatch]
	[BaseType (typeof (NSObject))]
	interface SCNProgram : NSCopying, NSSecureCoding {
		[NullAllowed]
		[Export ("vertexShader", ArgumentSemantic.Copy)]
		string VertexShader { get; set;  }

		[NullAllowed]
		[Export ("fragmentShader", ArgumentSemantic.Copy)]
		string FragmentShader { get; set;  }

		[iOS (9,0)][Mac (10,11)]
		[NullAllowed]
		[Export ("vertexFunctionName")]
		string VertexFunctionName { get; set; }

		[iOS (9,0)][Mac (10,11)]
		[NullAllowed]
		[Export ("fragmentFunctionName")]
		string FragmentFunctionName { get; set; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("handleBindingOfBufferNamed:frequency:usingBlock:")]
		void HandleBinding (string name, SCNBufferFrequency frequency, SCNBufferBindingHandler handler);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set;  }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		SCNProgramDelegate Delegate { get; set; }

		[Static, Export ("program")]
		SCNProgram Create ();

		[Export ("setSemantic:forSymbol:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		void SetSemantic (NSString geometrySourceSemantic, string symbol, [NullAllowed] NSDictionary options);

#if !WATCH
		[NoWatch]
		[Wrap ("SetSemantic (geometrySourceSemantic, symbol, options == null ? null : options.Dictionary)")]
		void SetSemantic (NSString geometrySourceSemantic, string symbol, SCNProgramSemanticOptions options);
#endif

		[Export ("semanticForSymbol:")]
#if XAMCORE_4_0
		NSString GetSemantic (string symbol);
#else
		NSString GetSemanticForSymbol (string symbol);
#endif

		[NoWatch]
		[Field ("SCNProgramMappingChannelKey")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSString MappingChannelKey { get; }

		[Mac (10,10)]
		[Export ("opaque")]
		bool Opaque { [Bind ("isOpaque")] get; set; }

#if XAMCORE_2_0
		[NoWatch]
		[iOS (9,0)][Mac (10,11)]
		[NullAllowed]
		[Export ("library", ArgumentSemantic.Retain)]
		IMTLLibrary Library { get; set; }
#endif
	}

	[iOS (8,0), NoWatch]
	[BaseType (typeof (NSObject))]
	[Model, Protocol]
	interface SCNProgramDelegate {
#if MONOMAC || !XAMCORE_2_0
	#if XAMCORE_3_0
		[Availability (Unavailable = Platform.iOS_Version)]
	#endif
		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("program:bindValueForSymbol:atLocation:programID:renderer:")]
		bool BindValue (SCNProgram program, string symbol, uint /* unsigned int */ location, uint /* unsigned int */ programID, SCNRenderer renderer);

	#if XAMCORE_3_0
		[Availability (Unavailable = Platform.iOS_Version)]
	#endif
		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("program:unbindValueForSymbol:atLocation:programID:renderer:")]
		void UnbindValue (SCNProgram program, string symbol, uint /* unsigned int */ location, uint /* unsigned int */ programID, SCNRenderer renderer);
#endif

		[Export ("program:handleError:")]
		void HandleError (SCNProgram program, NSError error);

#if MONOMAC || !XAMCORE_2_0
	#if XAMCORE_3_0
		[Availability (Unavailable = Platform.iOS_Version)]
		[NoTV, NoWatch]
	#endif
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use the SCNProgram's Opaque property instead.")]
		[Export ("programIsOpaque:")]
		bool IsProgramOpaque (SCNProgram program);
#endif
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNPyramid {
		[Export ("width")]
		nfloat Width { get; set;  }

		[Export ("height")]
		nfloat Height { get; set;  }

		[Export ("length")]
		nfloat Length { get; set;  }

		[Export ("widthSegmentCount")]
		nint WidthSegmentCount { get; set;  }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set;  }

		[Export ("lengthSegmentCount")]
		nint LengthSegmentCount { get; set;  }

		[Static, Export ("pyramidWithWidth:height:length:")]
		SCNPyramid Create (nfloat width, nfloat height, nfloat length);
	}

	[iOS (8,0), NoWatch]
	[BaseType (typeof (NSObject))]
#if !MONOMAC || XAMCORE_2_0
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[SCNRenderer init]: unrecognized selector sent to instance 0x7ce85a30
#endif
	interface SCNRenderer : SCNSceneRenderer, SCNTechniqueSupport {
//		We already pull in the Scene property from the SCNSceneRenderer protocol, no need to redefine it here.
//		[Export ("scene", ArgumentSemantic.Retain)]
//		SCNScene Scene { get; set;  }

		// options: nothing today, it is reserved for future use
		[Static, Export ("rendererWithContext:options:")]
		SCNRenderer FromContext (IntPtr context, [NullAllowed] NSDictionary options);

		[NoWatch]
		[Static]
		[Wrap ("FromContext (context.GetHandle (), options)")]
		// GetHandle will return IntPtr.Zero is context is null
		// GLContext == CGLContext on macOS and EAGLContext in iOS and tvOS (using on top of file)
		SCNRenderer FromContext (GLContext context, [NullAllowed] NSDictionary options);

		[NoWatch, NoTV]
		[Export ("render")]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.iOS, 9, 0)]
		void Render ();

		[Mac (10,10)]
		[Export ("renderAtTime:")]
		void Render (double timeInSeconds);

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("snapshotAtTime:withSize:antialiasingMode:")]
		NSImage GetSnapshot (double time, CGSize size, SCNAntialiasingMode antialiasingMode);

		[Mac (10,10)]
		[Export ("nextFrameTime")]
		double NextFrameTimeInSeconds { get; }

#if XAMCORE_2_0 || !MONOMAC
		[NoWatch]
		[iOS (9,0)][Mac (10,11)]
		[Static]
		[Export ("rendererWithDevice:options:")]
		SCNRenderer FromDevice ([NullAllowed] IMTLDevice device, [NullAllowed] NSDictionary options);

		[NoWatch]
		[iOS (9,0)][Mac (10,11)]
		[Export ("renderAtTime:viewport:commandBuffer:passDescriptor:")]
		void Render (double timeInSeconds, CGRect viewport, IMTLCommandBuffer commandBuffer, MTLRenderPassDescriptor renderPassDescriptor);

		[Watch (4,0), TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("renderWithViewport:commandBuffer:passDescriptor:")]
		void Render (CGRect viewport, IMTLCommandBuffer commandBuffer, MTLRenderPassDescriptor renderPassDescriptor);
#endif
		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("updateProbes:atTime:")]
		void Update (SCNNode [] lightProbes, double time);

		[Watch (4,0), TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("updateAtTime:")]
		void Update (double time);
	
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[Static]
	interface SCNRenderingArguments {
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

	[Mac (10,9), iOS (8,0)]
	delegate void SCNSceneExportProgressHandler (float /* float, not CGFloat */ totalProgress, NSError error, out bool stop);

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface SCNScene :
#if (XAMCORE_2_0 || !MONOMAC) && !WATCH
		GKSceneRootNodeType ,
#endif
		NSSecureCoding {

		[Static]
		[Export ("scene")]
		SCNScene Create ();

		[Export ("rootNode")]
		SCNNode RootNode { get; }

		[Export ("attributeForKey:")]
		NSObject GetAttribute (NSString key);

		[Export ("setAttribute:forKey:")]
		void SetAttribute (NSObject attribute, NSString key);

		[Static]
		[Export ("sceneWithURL:options:error:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNScene FromUrl (NSUrl url, [NullAllowed] NSDictionary options, out NSError error);

		[Static]
		[Wrap ("FromUrl (url, options == null ? null : options.Dictionary, out error)")]
		SCNScene FromUrl (NSUrl url, [NullAllowed] SCNSceneLoadingOptions options, out NSError error);

		

		[Mac (10,9), iOS (8,0)]
		[Field ("SCNSceneExportDestinationURL")]
		NSString ExportDestinationUrl { get; }

		[Mac (10,10, onlyOn64 : true), iOS (8,0)] // More 32-bit brokenness - 17710842
		[Export ("physicsWorld")]
		SCNPhysicsWorld PhysicsWorld { get; }

		[Mac (10,9), iOS (8,0)]
		[Export ("background")]
		SCNMaterialProperty Background { get; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("lightingEnvironment")]
		SCNMaterialProperty LightingEnvironment { get; }

		[Mac (10,10), iOS (8,0)]
		[Export ("fogStartDistance")]
		nfloat FogStartDistance { get; set; }

		[Mac (10,10), iOS (8,0)]
		[Export ("fogEndDistance")]
		nfloat FogEndDistance { get; set; }

		[Mac (10,10), iOS (8,0)]
		[Export ("fogDensityExponent")]
		nfloat FogDensityExponent { get; set; }

		[Mac (10,10), iOS (8,0)]
		[Export ("fogColor", ArgumentSemantic.Retain)]
		NSObject FogColor { get; set; }

		[Mac (10,10), iOS (8,0)]
		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; set; }

		[Mac (10,9), iOS (8,0)]
		[Static, Export ("sceneNamed:")]
		SCNScene FromFile (string name);

		[Mac (10,10), iOS (8,0)]
		[Static, Export ("sceneNamed:inDirectory:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNScene FromFile (string name, [NullAllowed] string directory, [NullAllowed] NSDictionary options);

		[Mac (10,10), iOS (8,0)]
		[Static, Wrap ("FromFile (name, directory, options == null ? null : options.Dictionary)")]
		SCNScene FromFile (string name, string directory, SCNSceneLoadingOptions options);

		// Keeping here the same name WriteToUrl for iOS and friends because it is how it was bound
		// initialy for macOS and having it named diferently would hurt shared code
		[TV (10, 0), NoWatch, Mac (10, 9), iOS (10, 0)]
		[Export ("writeToURL:options:delegate:progressHandler:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		bool WriteToUrl (NSUrl url,
			[NullAllowed] NSDictionary options,
			[NullAllowed] ISCNSceneExportDelegate aDelegate,
			[NullAllowed] SCNSceneExportProgressHandler exportProgressHandler);

		[TV (10, 0), NoWatch, Mac (10, 9), iOS (10, 0)]
		[Wrap ("WriteToUrl (url, options == null ? null : options.Dictionary, handler, exportProgressHandler)")]
		bool WriteToUrl (NSUrl url, SCNSceneLoadingOptions options, ISCNSceneExportDelegate handler, SCNSceneExportProgressHandler exportProgressHandler);

		#region SCNParticleSystemSupport (SCNNode) category

		[Mac (10,10)]
		[Export ("particleSystems")]
		SCNParticleSystem [] ParticleSystems { get; }

		[Mac (10,10)]
		[Export ("addParticleSystem:withTransform:")]
		void AddParticleSystem (SCNParticleSystem system, SCNMatrix4 transform);

		[Mac (10,10)]
		[Export ("removeAllParticleSystems")]
		void RemoveAllParticleSystems ();

		[Mac (10,10)]
		[Export ("removeParticleSystem:")]
		void RemoveParticleSystem (SCNParticleSystem system);

		#endregion

		[Field ("SCNSceneStartTimeAttributeKey")]
		NSString StartTimeAttributeKey { get; }

		[Field ("SCNSceneEndTimeAttributeKey")]
		NSString EndTimeAttributeKey { get; }

		[Field ("SCNSceneFrameRateAttributeKey")]
		NSString FrameRateAttributeKey { get; }

		[iOS (8,0)][Mac (10,10)]
		[Field ("SCNSceneUpAxisAttributeKey")]
		NSString UpAxisAttributeKey { get; }

#if XAMCORE_2_0
		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("sceneWithMDLAsset:")]
		SCNScene FromAsset (MDLAsset asset);
#endif
	}

	interface ISCNSceneExportDelegate { }

	[TV (10, 0), NoWatch, Mac (10, 9), iOS (10, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNSceneExportDelegate {

		[Export ("writeImage:withSceneDocumentURL:originalImageURL:")]
		[return: NullAllowed]
		NSUrl WriteImage (NSImage image, NSUrl documentUrl, [NullAllowed] NSUrl originalImageUrl);
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNSceneSource {
		[Export ("url")]
		NSUrl Url { get;  }

		[Export ("data")]
		NSData Data { get;  }

		[Static, Export ("sceneSourceWithURL:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNSceneSource FromUrl (NSUrl url, [NullAllowed] NSDictionary options);

		[Wrap ("FromUrl (url, options == null ? null : options.Dictionary)")]
		SCNSceneSource FromUrl (NSUrl url, SCNSceneLoadingOptions options);

		[Static]
		[Export ("sceneSourceWithData:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNSceneSource FromData (NSData data, [NullAllowed] NSDictionary options);

		[Static]
		[Wrap ("FromData (data, options == null ? null : options.Dictionary)")]
		SCNSceneSource FromData (NSData data, SCNSceneLoadingOptions options);

		[Export ("initWithURL:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		IntPtr Constructor (NSUrl url, [NullAllowed] NSDictionary options);

		[Wrap ("this (url, options == null ? null : options.Dictionary)")]
		IntPtr Constructor (NSUrl url, SCNSceneLoadingOptions options);

		[Export ("initWithData:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		IntPtr Constructor (NSData data, [NullAllowed] NSDictionary options);

		[Wrap ("this (data, options == null ? null : options.Dictionary)")]
		IntPtr Constructor (NSData data, SCNSceneLoadingOptions options);
		
		[Export ("sceneWithOptions:statusHandler:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNScene SceneFromOptions ([NullAllowed] NSDictionary options, SCNSceneSourceStatusHandler statusHandler);

		[Wrap ("SceneFromOptions (options == null ? null : options.Dictionary, statusHandler)")]
		SCNScene SceneFromOptions (SCNSceneLoadingOptions options, SCNSceneSourceStatusHandler statusHandler);

		[Export ("sceneWithOptions:error:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNScene SceneWithOption ([NullAllowed] NSDictionary options, out NSError error);

		[Wrap ("SceneWithOption (options == null ? null : options.Dictionary, out error)")]
		SCNScene SceneWithOption (SCNSceneLoadingOptions options, out NSError error);

		[Export ("propertyForKey:")]
		NSObject GetProperty (NSString key);

		[Export ("entryWithIdentifier:withClass:")]
		NSObject GetEntryWithIdentifier (string uid, Class entryClass);

		[Export ("identifiersOfEntriesWithClass:")]
#if XAMCORE_2_0
		string [] GetIdentifiersOfEntries (Class entryClass);
#else
		NSObject [] IdentifiersOfEntriesWithClass (Class entryClass);
#endif

		[Mac (10,9)]
		[Export ("entriesPassingTest:")]
		NSObject [] EntriesPassingTest (SCNSceneSourceFilter predicate);		
	}
	delegate bool SCNSceneSourceFilter (NSObject entry, NSString identifier, ref bool stop);

	[Watch (3,0)]
	[iOS (8,0)]
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

		// Less preferred spelling, don't break stable API
		// note: was never released for XI
#if !XAMCORE_2_0 && MONOMAC
		[Obsolete ("Use AssetDirectoryUrlsKey")]
		[Field ("SCNSceneSourceAssetDirectoryURLsKey")]
		NSString AssetDirectoryURLsKey { get; }

		[Obsolete ("Use OverrideAssetUrlsKey")]
		[Field ("SCNSceneSourceOverrideAssetURLsKey")]
		NSString OverrideAssetURLsKey { get; }
#endif

		[Field ("SCNSceneSourceStrictConformanceKey")]
		NSString StrictConformanceKey { get; }
		
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.TvOS, 11, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Field ("SCNSceneSourceUseSafeModeKey")]
		NSString UseSafeModeKey	 { get; }

		[Mac(10,10)]
		[iOS (8,0)] // header said NA and docs says "Available in iOS 8.0 through iOS 8.2." but it's back on iOS11
		[TV (11,0), Watch (4,0)]
		[Field ("SCNSceneSourceConvertUnitsToMetersKey")]
		NSString ConvertUnitsToMetersKey { get; }

		[Mac(10,10)]
		[iOS (8,0)] // header said NA and docs says "Available in iOS 8.0 through iOS 8.2." but it's back on iOS11
		[TV (11,0), Watch (4,0)]
		[Field ("SCNSceneSourceConvertToYUpKey")]
		NSString ConvertToYUpKey { get; }

		[Mac(10,10), iOS(8,0)]
		[Field ("SCNSceneSourceAnimationImportPolicyKey")]
		NSString AnimationImportPolicyKey { get; }
		
		[Mac(10,10), iOS(8,0)]
		[Field ("SCNSceneSourceAnimationImportPolicyPlay")]
		NSString AnimationImportPolicyPlay { get; }
		
		[Mac(10,10), iOS(8,0)]
		[Field ("SCNSceneSourceAnimationImportPolicyPlayRepeatedly")]
		NSString AnimationImportPolicyPlayRepeatedly { get; }
		
		[Mac(10,10), iOS(8,0)]
		[Field ("SCNSceneSourceAnimationImportPolicyDoNotPlay")]
		NSString AnimationImportPolicyDoNotPlay { get; }
		
		[Mac(10,10), iOS(8,0)]
		[Field ("SCNSceneSourceAnimationImportPolicyPlayUsingSceneTimeBase")]
		NSString AnimationImportPolicyPlayUsingSceneTimeBase { get; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("SCNSceneSourceLoadingOptionPreserveOriginalTopology")]
		NSString OptionPreserveOriginalTopology { get; }
	}

	[Watch (3,0)]
	[iOS (8,0)]
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

	[Watch (3,0)]
	[iOS (8,0)]
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

	interface ISCNSceneRenderer {}

	[Watch (3,0)]
	[iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNSceneRenderer {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakSceneRendererDelegate { get; set;  }

		[Wrap ("WeakSceneRendererDelegate")]
		[Protocolize]
		SCNSceneRendererDelegate SceneRendererDelegate { get; set; }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("playing")]
		bool Playing { [Bind ("isPlaying")] get; set;  }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("loops")]
		bool Loops { get; set;  }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("pointOfView", ArgumentSemantic.Retain)]
		SCNNode PointOfView { get; set;  }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("autoenablesDefaultLighting")]
		bool AutoenablesDefaultLighting { get; set;  }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("jitteringEnabled")]
		bool JitteringEnabled { [Bind ("isJitteringEnabled")] get; set;  }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("context")]
		IntPtr Context { get;  }

#if MONOMAC
		[Availability (Deprecated = Platform.Mac_10_10, Unavailable = Platform.iOS_Version)]
		[Export ("currentTime")]
		double CurrentTime { get; set; }
#endif

		[Abstract]
		[Export ("hitTest:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNHitTestResult [] HitTest (CGPoint thePoint, [NullAllowed] NSDictionary options);

		[Wrap ("HitTest (thePoint, options == null ? null : options.Dictionary)")]
		SCNHitTestResult [] HitTest (CGPoint thePoint, SCNHitTestOptions options);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Mac (10,9)]
		[Export ("showsStatistics")]
		bool ShowsStatistics { get; set; }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Mac (10,10)]
		[Export ("sceneTime")]
		double SceneTimeInSeconds { get; set; }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Mac (10,10)]
		[Export ("scene", ArgumentSemantic.Retain)]
		SCNScene Scene { get; set; }

#if XAMCORE_2_0
		[Abstract]
#endif
#if XAMCORE_2_0 || !MONOMAC
		// It seems swift has this property listed as an optional[0] and an Apple sample[1] sets this to null
		// [0]: https://developer.apple.com/documentation/scenekit/scnscenerenderer/1524051-overlayskscene
		// [1]: https://github.com/xamarin/xamarin-macios/issues/3392
		[NullAllowed]
		[Mac (10,10)]
		[Export ("overlaySKScene", ArgumentSemantic.Retain)]
		SKScene OverlayScene { get; set; }
#endif

#if XAMCORE_2_0
		[Abstract]
#endif
		[Mac (10,9)]
		[Export ("isNodeInsideFrustum:withPointOfView:")]
		bool IsNodeInsideFrustum (SCNNode node, SCNNode pointOfView);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Mac (10,9)]
		[Export ("projectPoint:")]
		SCNVector3 ProjectPoint (SCNVector3 point);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Mac (10,9)]
		[Export ("unprojectPoint:")]
		SCNVector3 UnprojectPoint (SCNVector3 point);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Mac (10,9)]
		[Export ("prepareObject:shouldAbortBlock:")]
		bool Prepare (NSObject obj, [NullAllowed] Func<bool> abortHandler);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Mac (10,10)]
		[Async]
		[Export ("prepareObjects:withCompletionHandler:")]
		void Prepare (NSObject [] objects, [NullAllowed] Action<bool> completionHandler);

#if XAMCORE_2_0 || !MONOMAC
	#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
	#endif
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)] // SKTransition -> SpriteKit -> only on 64 bits
		[Async]
		[Export ("presentScene:withTransition:incomingPointOfView:completionHandler:")]
		void PresentScene (SCNScene scene, SKTransition transition, [NullAllowed] SCNNode pointOfView, [NullAllowed] Action completionHandler);
#endif

#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[iOS (9,0)][Mac (10,11)]
		[Export ("nodesInsideFrustumWithPointOfView:")]
		SCNNode[] GetNodesInsideFrustum (SCNNode pointOfView);

#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[iOS (9,0)][Mac (10,11)]
		[Export ("debugOptions", ArgumentSemantic.Assign)]
		SCNDebugOptions DebugOptions { get; set; }

#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[iOS (9,0)][Mac (10,11)]
		[Export ("renderingAPI")]
		SCNRenderingApi RenderingApi { get; }

		// MonoMac / XM Classic does not have 64bits frameworks, e.g. Metal, and can't have those API
#if XAMCORE_2_0 || !MONOMAC
	#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
	#endif
		[NoWatch]
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)] // IMTLRenderCommandEncoder -> Metal -> only on 64 bits
		[NullAllowed, Export ("currentRenderCommandEncoder")]
		IMTLRenderCommandEncoder CurrentRenderCommandEncoder { get; }

	#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
	#endif
		[NoWatch]
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[NullAllowed, Export ("device")]
		IMTLDevice Device { get; }

	#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
	#endif
		[NoWatch]
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("colorPixelFormat")]
		MTLPixelFormat ColorPixelFormat { get; }

	#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
	#endif
		[NoWatch]
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("depthPixelFormat")]
		MTLPixelFormat DepthPixelFormat { get; }

	#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
	#endif
		[NoWatch]
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("stencilPixelFormat")]
		MTLPixelFormat StencilPixelFormat { get; }

	#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
	#endif
		[NoWatch]
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[NullAllowed, Export ("commandQueue")]
		IMTLCommandQueue CommandQueue { get; }
#endif

#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("audioEngine")]
		AVAudioEngine AudioEngine { get; }

#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[NoWatch]
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("audioEnvironmentNode")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		AVAudioEnvironmentNode AudioEnvironmentNode { get; }

#if XAMCORE_4_0
		[Abstract] // this protocol existed before iOS 9 (or OSX 10.11) and we cannot add abstract members to it (breaking changes)
#endif
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[NullAllowed, Export ("audioListener", ArgumentSemantic.Retain)]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		SCNNode AudioListener { get; set; }
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNSceneRendererDelegate {

		[Export ("renderer:willRenderScene:atTime:")]
		void WillRenderScene ([Protocolize]SCNSceneRenderer renderer, SCNScene scene, double timeInSeconds);

		[Export ("renderer:didRenderScene:atTime:")]
		void DidRenderScene ([Protocolize]SCNSceneRenderer renderer, SCNScene scene, double timeInSeconds);

		[Mac (10,10)]
		[Export ("renderer:updateAtTime:")]
		void Update ([Protocolize]SCNSceneRenderer renderer, double timeInSeconds);

		[Mac (10,10)]
		[Export ("renderer:didApplyAnimationsAtTime:")]
		void DidApplyAnimations ([Protocolize]SCNSceneRenderer renderer, double timeInSeconds);

		[Mac (10,10)]
		[Export ("renderer:didSimulatePhysicsAtTime:")]
		void DidSimulatePhysics ([Protocolize]SCNSceneRenderer renderer, double timeInSeconds);

		[Watch (4,0), TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("renderer:didApplyConstraintsAtTime:")]
		void DidApplyConstraints ([Protocolize] SCNSceneRenderer renderer, double atTime);
		
	}	

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	[DisableDefaultCtor] 
	interface SCNSphere {
		[Export ("radius")]
		nfloat Radius { get; set;  }

		[Export ("geodesic")]
		bool Geodesic { [Bind ("isGeodesic")] get; set;  }

		[Export ("segmentCount")]
		nint SegmentCount { get; set;  }

		[Static, Export ("sphereWithRadius:")]
		SCNSphere Create (nfloat radius);

	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	[DisableDefaultCtor] 
	interface SCNText {
		[Export ("extrusionDepth")]
		nfloat ExtrusionDepth { get; set;  }

		[Export ("string", ArgumentSemantic.Copy)]
		NSObject String { get; set;  }

		[Export ("containerFrame")]
		CGRect ContainerFrame { get; set;  }

#if MONOMAC
		// removed in iOS8 beta 5 - but it was already existing in 10.8 ?
		[Export ("textSize")]
		CGSize TextSize { get;  }
#endif

		[Export ("truncationMode", ArgumentSemantic.Copy)]
		string TruncationMode { get; set;  }

		[Export ("alignmentMode", ArgumentSemantic.Copy)]
		string AlignmentMode { get; set;  }

		[Export ("chamferRadius")]
		nfloat ChamferRadius { get; set;  }

#if MONOMAC && !XAMCORE_2_0
		[Availability (Deprecated = Platform.Mac_10_9)]
		[Export ("chamferSegmentCount")]
		nint ChamferSegmentCount { get; set;  }
#endif

		[Static, Export ("textWithString:extrusionDepth:")]
		SCNText Create (NSObject str, nfloat extrusionDepth);

		[Export ("font", ArgumentSemantic.Retain)]
		NSFont Font { get; set; }

		[Export ("wrapped")]
		bool Wrapped { [Bind ("isWrapped")] get; set; }

		[Mac (10,9)]
		[Export ("chamferProfile", ArgumentSemantic.Copy)]
		NSBezierPath ChamferProfile { get; set; }

		[Mac (10,9)]
		[Export ("flatness")]
		nfloat Flatness { get; set; }
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	[DisableDefaultCtor] 
	interface SCNTorus {
		[Export ("ringRadius")]
		nfloat RingRadius { get; set;  }

		[Export ("pipeRadius")]
		nfloat PipeRadius { get; set;  }

		[Export ("ringSegmentCount")]
		nint RingSegmentCount { get; set;  }

		[Export ("pipeSegmentCount")]
		nint PipeSegmentCount { get; set;  }

		[Static, Export ("torusWithRingRadius:pipeRadius:")]
		SCNTorus Create (nfloat ringRadius, nfloat pipeRadius);
	}

	[Watch (3,0)]
	[iOS (8,0)]
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
		void SetValueForKey (NSObject value, NSString key);

		//Detected properties
		[Static]
		[Export ("animationDuration")]
		double AnimationDuration { get; set; }

		[Static]
		[NoWatch]
		[NullAllowed] // by default this property is null
		[Export ("animationTimingFunction")]
		CAMediaTimingFunction AnimationTimingFunction { get; set; }

		[Static]
		[Export ("disableActions")]
		bool DisableActions { get; set; }
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (SCNGeometry))]
	interface SCNTube {
		[Export ("innerRadius")]
		nfloat InnerRadius { get; set;  }

		[Export ("outerRadius")]
		nfloat OuterRadius { get; set;  }

		[Export ("height")]
		nfloat Height { get; set;  }

		[Export ("radialSegmentCount")]
		nint RadialSegmentCount { get; set;  }

		[Export ("heightSegmentCount")]
		nint HeightSegmentCount { get; set;  }

		[Static, Export ("tubeWithInnerRadius:outerRadius:height:")]
		SCNTube Create (nfloat innerRadius, nfloat outerRadius, nfloat height);
	}

	[NoWatch]
	[iOS (9,0)][Mac (10,11)]
	[Static]
	[Internal] // we'll make it public if there's a need for them (beside the strong dictionary we provide)
	interface SCNRenderingOptionsKeys {

		[Field ("SCNPreferredRenderingAPIKey")]
		NSString RenderingApiKey { get; }

		[Field ("SCNPreferredDeviceKey")]
		NSString DeviceKey { get; }

		[Field ("SCNPreferLowPowerDeviceKey")]
		NSString LowPowerDeviceKey { get; }
	}

#if !WATCH
	[NoWatch]
	[iOS (9,0)][Mac (10,11)]
	[StrongDictionary ("SCNRenderingOptionsKeys")]
	interface SCNRenderingOptions
	{

#if XAMCORE_2_0 || !MONOMAC
		IMTLDevice Device { get; set; }
#endif

		bool LowPowerDevice { get; set; }
	}
#endif

	[iOS (8,0), NoWatch]
	[BaseType (typeof (NSView))]
	[DisableDefaultCtor] 
	interface SCNView : SCNSceneRenderer, SCNTechniqueSupport {
//		We already pull in the Scene property from the SCNSceneRenderer protocol, no need to redefine it here.
//		[Export ("scene", ArgumentSemantic.Retain)]
//		SCNScene Scene { get; set;  }

#if MONOMAC
		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set;  }
#endif

		[Export ("allowsCameraControl")]
		bool AllowsCameraControl { get; set;  }

#if MONOMAC
		[Export ("openGLContext", ArgumentSemantic.Retain)]
		NSOpenGLContext	OpenGLContext { get; set;  }

		[Export ("pixelFormat", ArgumentSemantic.Retain)]
		NSOpenGLPixelFormat PixelFormat { get; set;  }
#elif !WATCH
		[Export ("eaglContext", ArgumentSemantic.Retain)]
#if XAMCORE_2_0
		EAGLContext EAGLContext { get; set; }
#else
		new EAGLContext Context { get; set; }
#endif
#endif

#if !WATCH
		[iOS (9,0)][Mac (10,11)]
		[Wrap ("this (frame, options != null ? options.Dictionary : null)")]
		IntPtr Constructor (CGRect frame, [NullAllowed] SCNRenderingOptions options);
#endif

		[Export ("initWithFrame:options:")]
		IntPtr Constructor (CGRect frame, [NullAllowed] NSDictionary options);

		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("play:")]
		void Play ([NullAllowed] NSObject sender);

		[Export ("pause:")]
		void Pause ([NullAllowed] NSObject sender);

		[Export ("stop:")]
		void Stop ([NullAllowed] NSObject sender);

		[iOS (8,0)][Mac (10,10)]
		[Export ("snapshot")]
		NSImage Snapshot ();

		[Mac (10,12)]
		[Export ("preferredFramesPerSecond")]
		nint PreferredFramesPerSecond { get; set; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("antialiasingMode")]
		SCNAntialiasingMode AntialiasingMode { get; set; }

#if XAMCORE_2_0
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("cameraControlConfiguration")]
		ISCNCameraControlConfiguration CameraControlConfiguration { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("defaultCameraController")]
		SCNCameraController DefaultCameraController { get; }
#endif
		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("rendersContinuously")]
		bool RendersContinuously { get; set; }
	}

#if XAMCORE_4_0
	[Mac (10,9), iOS (8,0), Watch (4,0)]
	delegate void SCNAnimationEventHandler (AnimationType animation, NSObject animatedObject, bool playingBackward);
#endif

	[Watch (4,0)]
	[Mac (10,9), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNAnimationEvent {

#if XAMCORE_4_0
		[Static, Export ("animationEventWithKeyTime:block:")]
		SCNAnimationEvent Create (nfloat keyTime, SCNAnimationEventHandler eventHandler);
#else
		[Internal]
		[Static, Export ("animationEventWithKeyTime:block:")]
		SCNAnimationEvent Create (nfloat keyTime, Action<IntPtr, NSObject, bool> handler);
#endif
	}

	[Watch (3,0)]
	[Mac (10,9), iOS (8,0)]
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
		SCNShape Create (NSBezierPath path, nfloat extrusionDepth);
	}

	[Watch (3,0)]
	[Mac (10,9), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface SCNMorpher : SCNAnimatable, NSSecureCoding {
		[NullAllowed] // by default this property is null
		[Export ("targets", ArgumentSemantic.Copy)]
		SCNGeometry [] Targets { get; set; }

		[Export ("calculationMode")]
		SCNMorpherCalculationMode CalculationMode { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("weights", ArgumentSemantic.Retain)]
		NSNumber [] Weights { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("unifiesNormals")]
		bool UnifiesNormals { get; set; }

		[Export ("setWeight:forTargetAtIndex:")]
		void SetWeight (nfloat weight, nuint targetIndex);

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("setWeight:forTargetNamed:")]
		void SetWeight (nfloat weight, string targetName);

		[Export ("weightForTargetAtIndex:")]
		nfloat GetWeight (nuint targetIndex);

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("weightForTargetNamed:")]
		nfloat GetWeight (string targetName);
	}

	[Watch (3,0)]
	[Mac (10,9), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNSkinner : NSSecureCoding {
		[Export ("skeleton", ArgumentSemantic.Retain)]
		SCNNode Skeleton { get; set; }

		[Export ("baseGeometry", ArgumentSemantic.Retain)]
		SCNGeometry BaseGeometry { get; set; }

		[Mac (10,10)]
		[Export ("baseGeometryBindTransform")]
		SCNMatrix4 BaseGeometryBindTransform { get; set; }

		[Mac (10,10)]
		[Internal, Export ("boneInverseBindTransforms")]
		NSArray _BoneInverseBindTransforms { get; }

		[Mac (10,10)]
		[Export ("bones")]
		SCNNode [] Bones { get; }

		[Mac (10,10)]
		[Export ("boneWeights")]
		SCNGeometrySource BoneWeights { get; }

		[Mac (10,10)]
		[Export ("boneIndices")]
		SCNGeometrySource BoneIndices { get; }

		[Mac (10,10)]
		[Static, Internal, Export ("skinnerWithBaseGeometry:bones:boneInverseBindTransforms:boneWeights:boneIndices:")]
		SCNSkinner _Create (SCNGeometry baseGeometry, SCNNode [] bones, NSArray boneInverseBindTransforms,
			SCNGeometrySource boneWeights, SCNGeometrySource boneIndices);
	}

	[Watch (3,0)]
	[Mac (10,9), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[DisableDefaultCtor]
	interface SCNConstraint : SCNAnimatable, NSCopying, NSSecureCoding {
		[Mac (10,10)]
		[Export ("influenceFactor")]
		nfloat InfluenceFactor { get; set; }

		[Mac (10, 10), iOS (8,0)]
		[TV (11,0)][Watch (4,0)]
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("incremental")]
		bool Incremental { [Bind ("isIncremental")] get; set; }
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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

		[iOS (9,0)][Mac (10,11)]
		[Export ("initWithChainRootNode:")]
		IntPtr Constructor (SCNNode chainRootNode);
		
	}

	[Watch (3,0)]
	[Mac (10,9), iOS (8,0)]
	[BaseType (typeof (SCNConstraint))]
#if !MONOMAC || XAMCORE_2_0
	[DisableDefaultCtor]
#endif
	interface SCNLookAtConstraint {
		[Export ("target", ArgumentSemantic.Retain), NullAllowed]
		SCNNode Target { get; [Mac (10, 12), iOS (10, 0), TV (10, 0)] set; }

		[Export ("gimbalLockEnabled")]
		bool GimbalLockEnabled { get; set; }

		[Static, Export ("lookAtConstraintWithTarget:")]
		SCNLookAtConstraint Create ([NullAllowed] SCNNode target);

		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("targetOffset", ArgumentSemantic.Assign)]
		SCNVector3 TargetOffset { get; set; }
	
		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("localFront", ArgumentSemantic.Assign)]
		SCNVector3 LocalFront { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("worldUp", ArgumentSemantic.Assign)]
		SCNVector3 WorldUp { get; set; }
	}

	[Mac (10,9), iOS (8,0)]
	delegate SCNMatrix4 SCNTransformConstraintHandler (SCNNode node, SCNMatrix4 transform);

	[Watch (3,0)]
	[Mac (10,9), iOS (8,0)]
	[BaseType (typeof (SCNConstraint))]
	[DisableDefaultCtor]
	interface SCNTransformConstraint {
		[Static, Export ("transformConstraintInWorldSpace:withBlock:")]
		SCNTransformConstraint Create (bool inWorldSpace, SCNTransformConstraintHandler transformHandler);

		[Watch (4,0), TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("positionConstraintInWorldSpace:withBlock:")]
		SCNTransformConstraint CreatePositionConstraint (bool inWorldSpace, Func<SCNNode, SCNVector3, SCNVector3> transformHandler);
	
		[Watch (4,0), TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("orientationConstraintInWorldSpace:withBlock:")]
		SCNTransformConstraint CreateOrientationConstraint (bool inWorldSpace, Func<SCNNode, SCNQuaternion, SCNQuaternion> transformHandler);
		
	}

	[Watch (3,0)]
	[Mac (10,9), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNLevelOfDetail : NSCopying, NSSecureCoding {
		[Export ("geometry")]
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

	[Watch (3,0)]
	[Mac (10,9), iOS (8,0)]
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


	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNActionable {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("runAction:")]
		void RunAction (SCNAction action);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("runAction:completionHandler:")]
		void RunAction (SCNAction action, [NullAllowed] Action block);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("runAction:forKey:")]
		void RunAction (SCNAction action, [NullAllowed] string key);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("runAction:forKey:completionHandler:")]
		void RunAction (SCNAction action, [NullAllowed] string key, [NullAllowed] Action block);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("hasActions")]
		bool HasActions ();

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("actionForKey:")]
		SCNAction GetAction (string key);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("removeActionForKey:")]
		void RemoveAction (string key);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("removeAllActions")]
		void RemoveAllActions ();

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (8,0), Mac(10,10)]
		[Export ("actionKeys")]
		string [] ActionKeys { get; }
	}

	[Mac (10,10), iOS (8,0)]
	delegate void SCNActionNodeWithElapsedTimeHandler (SCNNode node, nfloat elapsedTime);

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface SCNAction : NSCopying, NSSecureCoding {

		[Export ("duration")]
		double DurationInSeconds { get; set; }

		[Export ("timingMode")]
		SCNActionTimingMode TimingMode { get; set; }

		[NullAllowed, Export ("timingFunction", ArgumentSemantic.Assign)]
#if XAMCORE_4_0
		Func<float,float> TimingFunction { get; set; }
#else
		Func<float,float> TimingFunction2 { get; set; }
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

		[Mac (10,11), iOS (9,0)]
		[Static, Export ("hide")]
		SCNAction Hide ();

		[Mac (10,11), iOS (9,0)]
		[Static, Export ("unhide")]
		SCNAction Unhide ();

		[Mac (10,11), iOS (9,0)]
		[Static, Export ("playAudioSource:waitForCompletion:")]
		SCNAction PlayAudioSource (SCNAudioSource source, bool wait);
	}

	[iOS (8,0)]
	delegate void SCNBindingHandler (uint /* unsigned int */ programId, uint /* unsigned int */ location, SCNNode renderedNode, SCNRenderer renderer);

	[Watch (3,0)]
	[StrongDictionary ("_SCNShaderModifiers")]
	interface SCNShaderModifiers {
		string EntryPointGeometry { get; set; }
		string EntryPointSurface { get; set; }
		string EntryPointLightingModel { get; set; }
		string EntryPointFragment { get; set; }
	}
	
	[Watch (3,0)]
	[Mac (10,9), iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNShadable {

		[Mac (10,9, onlyOn64 : true)] // Not marked, but crashes 32-bit - 17695192
		[NullAllowed] // by default this property is null
		[Export ("shaderModifiers", ArgumentSemantic.Copy)]
		NSDictionary WeakShaderModifiers { get; set; }

		[Mac (10,9, onlyOn64 : true)] // Not marked, but crashes 32-bit - 17695192
		[Wrap ("WeakShaderModifiers")]
		SCNShaderModifiers ShaderModifiers { get; set; }

		[NoWatch]
		[NullAllowed] // by default this property is null
		[Export ("program", ArgumentSemantic.Retain)]
		SCNProgram Program { get; set; }

		[Mac (10,9), NoWatch]
		[Export ("handleBindingOfSymbol:usingBlock:")]
		void HandleBinding (string symbol, SCNBindingHandler handler);

		[Mac (10,9), NoWatch]
		[Export ("handleUnbindingOfSymbol:usingBlock:")]
		void HandleUnbinding (string symbol, SCNBindingHandler handler);
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNTechnique : SCNAnimatable, NSCopying, NSSecureCoding {

		[Export ("dictionaryRepresentation")]
#if XAMCORE_2_0
		NSDictionary ToDictionary ();
#else
		[Obsolete ("Use ToDictionary () instead")]
		NSDictionary DictionaryRepresentation { get; }
#endif

		[Static, Export ("techniqueWithDictionary:")]
		SCNTechnique Create (NSDictionary dictionary);

		[Static, Export ("techniqueBySequencingTechniques:")]
		SCNTechnique Create (SCNTechnique [] techniques);

		[NoWatch]
		[Export ("handleBindingOfSymbol:usingBlock:")]
		void HandleBinding (string symbol, [NullAllowed] SCNBindingHandler handler);

		[iOS (9,0)][Mac (10,11)]
		[Internal, Export ("objectForKeyedSubscript:")]
		[return: NullAllowed]
		NSObject _GetObject (NSObject key);

		[iOS (9,0)][Mac (10,11)]
		[Internal, Export ("setObject:forKeyedSubscript:")]
		void _SetObject ([NullAllowed] NSObject obj, INSCopying key);
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNTechniqueSupport {
		[Abstract]
		[Mac (10,10), iOS (8,0)]
		[Export ("technique", ArgumentSemantic.Copy)]
		[NullAllowed]
		SCNTechnique Technique { get; set; }
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[Static]
	interface SCNPhysicsTestKeys {

		[Field ("SCNPhysicsTestCollisionBitMaskKey")]
		NSString CollisionBitMaskKey { get; }

		[Field ("SCNPhysicsTestSearchModeKey")]
		NSString SearchModeKey { get; }

		[Field ("SCNPhysicsTestBackfaceCullingKey")]
		NSString BackfaceCullingKey { get; }
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[Static]
	interface SCNPhysicsTestSearchModeKeys {

		[Field ("SCNPhysicsTestSearchModeAny")]
		NSString Any { get; }

		[Field ("SCNPhysicsTestSearchModeClosest")]
		NSString Closest { get; }

		[Field ("SCNPhysicsTestSearchModeAll")]
		NSString All { get; }
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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

		[iOS (9,0), Mac(10,11)]
		[Export ("momentOfInertia", ArgumentSemantic.Assign)]
		SCNVector3 MomentOfInertia { get; set; }

		[iOS (9,0), Mac(10,11)]
		[Export ("usesDefaultMomentOfInertia")]
		bool UsesDefaultMomentOfInertia { get; set; }

		[iOS (9,0), Mac(10,11)]
		[Export ("contactTestBitMask", ArgumentSemantic.Assign)]
		nuint ContactTestBitMask { get; set; }		

		[iOS (9,0), Mac(10,11)]
		[Export ("affectedByGravity")]
		bool AffectedByGravity { [Bind ("isAffectedByGravity")] get; set; }
		
		[Watch (5,0), TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("setResting:")]
		void SetResting (bool resting);

		[Watch (5,0), TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("continuousCollisionDetectionThreshold")]
		nfloat ContinuousCollisionDetectionThreshold { get; set; }

		[Watch (5,0), TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("centerOfMassOffset", ArgumentSemantic.Assign)]
		SCNVector3 CenterOfMassOffset { get; set; }

		[Watch (5,0), TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("linearRestingThreshold")]
		nfloat LinearRestingThreshold { get; set; }

		[Watch (5,0), TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("angularRestingThreshold")]
		nfloat AngularRestingThreshold { get; set; }
	}

	[Mac (10,10), iOS (8,0)]
	delegate SCNVector3 SCNFieldForceEvaluator (SCNVector3 position, SCNVector3 velocity, float /* float, not CGFloat */ mass, float /* float, not CGFloat */ charge, double timeInSeconds);

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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

	[Watch (3,0)]
	[StrongDictionary ("SCNPhysicsTestKeys")]
	interface SCNPhysicsTest {
		nuint CollisionBitMask { get; set; }

		bool BackfaceCulling { get; set; }

		[Internal, Export ("SCNPhysicsTestKeys.SearchModeKey")]
		NSString _SearchMode { get; set; }
	}
	
	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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
		[Protocolize]
		SCNPhysicsContactDelegate ContactDelegate { get; set; }

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

		[Wrap ("RayTestWithSegmentFromPoint (origin, dest, options != null ? options.Dictionary : null)")]
		SCNHitTestResult [] RayTestWithSegmentFromPoint (SCNVector3 origin, SCNVector3 dest, [NullAllowed] SCNPhysicsTest options);

		[Export ("contactTestBetweenBody:andBody:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNPhysicsContact [] ContactTest (SCNPhysicsBody bodyA, SCNPhysicsBody bodyB, [NullAllowed] NSDictionary options);

		[Wrap ("ContactTest (bodyA, bodyB, options != null ? options.Dictionary : null)")]
		SCNPhysicsContact [] ContactTest (SCNPhysicsBody bodyA, SCNPhysicsBody bodyB, [NullAllowed] SCNPhysicsTest options);

		[Export ("contactTestWithBody:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNPhysicsContact [] ContactTest (SCNPhysicsBody body, [NullAllowed] NSDictionary options);

		[Wrap ("ContactTest (body, options != null ? options.Dictionary : null)")]
		SCNPhysicsContact [] ContactTest (SCNPhysicsBody body, [NullAllowed] SCNPhysicsTest options);

		[Export ("convexSweepTestWithShape:fromTransform:toTransform:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNPhysicsContact [] ConvexSweepTest (SCNPhysicsShape shape, SCNMatrix4 from, SCNMatrix4 to, [NullAllowed] NSDictionary options);

		[Wrap ("ConvexSweepTest (shape, from, to, options != null ? options.Dictionary : null)")]
		SCNPhysicsContact [] ConvexSweepTest (SCNPhysicsShape shape, SCNMatrix4 from, SCNMatrix4 to, [NullAllowed] SCNPhysicsTest options);

		[Export ("updateCollisionPairs")]
		void UpdateCollisionPairs ();
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNPhysicsShape : NSCopying, NSSecureCoding {

		[Internal, Static, Export ("shapeWithShapes:transforms:")]
		SCNPhysicsShape Create (SCNPhysicsShape [] shapes, NSValue [] transforms);

		[Static, Export ("shapeWithGeometry:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNPhysicsShape Create (SCNGeometry geometry, [NullAllowed] NSDictionary options);

		[Static, Export ("shapeWithNode:options:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		SCNPhysicsShape Create (SCNNode node, [NullAllowed] NSDictionary options);

		[iOS (9,0), Mac(10,11)]
		[NullAllowed, Export ("options"), Internal]
		NSDictionary _Options { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("sourceObject")]
		NSObject SourceObject { get; }

		[iOS (9,0), Mac(10,11)]
		[NullAllowed, Export ("transforms")]
		NSValue[] Transforms { get; }		
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface SCNPhysicsShapeOptionsKeys {

		[Field ("SCNPhysicsShapeScaleKey")]
		NSString Scale { get; }

		[TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("SCNPhysicsShapeOptionCollisionMargin")]
		NSString CollisionMargin { get; }

		[Field ("SCNPhysicsShapeKeepAsCompoundKey")]
		NSString KeepAsCompound { get; }

		[Field ("SCNPhysicsShapeTypeKey")]
		NSString Type { get; }
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("sweepTestFraction")]
		nfloat SweepTestFraction { get; }
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[DisableDefaultCtor]
	interface SCNPhysicsBehavior : NSSecureCoding {

	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[BaseType (typeof (SCNPhysicsBehavior))]
	[DisableDefaultCtor]
	interface SCNPhysicsHingeJoint {

		[Static, Export ("jointWithBodyA:axisA:anchorA:bodyB:axisB:anchorB:")]
		SCNPhysicsHingeJoint Create (SCNPhysicsBody bodyA, SCNVector3 axisA, SCNVector3 anchorA,
			[NullAllowed] SCNPhysicsBody bodyB, SCNVector3 axisB, SCNVector3 anchorB);

		[Static, Export ("jointWithBody:axis:anchor:")]
		SCNPhysicsHingeJoint Create (SCNPhysicsBody body, SCNVector3 axis, SCNVector3 anchor);

		[Export ("bodyA")]
		SCNPhysicsBody BodyA { get; }

		[Export ("axisA")]
		SCNVector3 AxisA { get; set; }

		[Export ("anchorA")]
		SCNVector3 AnchorA { get; set; }

		[Export ("bodyB")]
		SCNPhysicsBody BodyB { get; }

		[Export ("axisB")]
		SCNVector3 AxisB { get; set; }

		[Export ("anchorB")]
		SCNVector3 AnchorB { get; set; }
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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
		SCNPhysicsBody BodyB { get; }

		[Export ("anchorB")]
		SCNVector3 AnchorB { get; set; }
	}

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
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

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNParticleSystem : NSCopying, NSSecureCoding, SCNAnimatable {

		[Static, Export ("particleSystem")]
		SCNParticleSystem Create ();

		[Static, Export ("particleSystemNamed:inDirectory:")]
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
		SCNParticleSystem SystemSpawnedOnDying { get; set; }

		[Export ("systemSpawnedOnCollision", ArgumentSemantic.Retain)]
		SCNParticleSystem SystemSpawnedOnCollision { get; set; }

		[Export ("systemSpawnedOnLiving", ArgumentSemantic.Retain)]
		SCNParticleSystem SystemSpawnedOnLiving { get; set; }

		[Export ("particleImage", ArgumentSemantic.Retain)]
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

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("orientationDirection", ArgumentSemantic.Assign)]
		SCNVector3 OrientationDirection { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("particleIntensity")]
		nfloat ParticleIntensity { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Export ("particleIntensityVariation")]
		nfloat ParticleIntensityVariation { get; set; }
	}

	[Watch (3,0)]
	[Static]
	interface SCNParticleProperty {
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyPosition")]
		NSString Position { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyAngle")]
		NSString Angle { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyRotationAxis")]
		NSString RotationAxis { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyVelocity")]
		NSString Velocity { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyAngularVelocity")]
		NSString AngularVelocity { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyLife")]
		NSString Life { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyColor")]
		NSString Color { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyOpacity")]
		NSString Opacity { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertySize")]
		NSString Size { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyFrame")]
		NSString Frame { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyFrameRate")]
		NSString FrameRate { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyBounce")]
		NSString Bounce { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyCharge")]
		NSString Charge { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("SCNParticlePropertyFriction")]
		NSString Friction { get; }

		[Mac (10, 10), iOS (8, 0)]
		[Field ("SCNParticlePropertyContactPoint")]
		NSString ContactPoint { get; }

		[Mac (10, 10), iOS (8, 0)]
		[Field ("SCNParticlePropertyContactNormal")]
		NSString ContactNormal { get; }
	}
	
	[Mac (10,10), iOS (8,0)]
	delegate void SCNParticleEventHandler (IntPtr data, IntPtr dataStride, IntPtr indices, nint count);

	[Mac (10,10), iOS (8,0)]
	delegate void SCNParticleModifierHandler (IntPtr data, IntPtr dataStride, nint start, nint end, float /* float, not CGFloat */ deltaTime);

	[Watch (3,0)]
	[Mac (10,10), iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCNParticlePropertyController : NSSecureCoding, NSCopying {

		[NoWatch]
		[Static, Export ("controllerWithAnimation:")]
		SCNParticlePropertyController Create (CAAnimation animation);

		[NoWatch]
		[Export ("animation", ArgumentSemantic.Retain)]
		CAAnimation Animation { get; set; }

		[Export ("inputMode")]
		SCNParticleInputMode InputMode { get; set; }

		[Export ("inputScale")]
		nfloat InputScale { get; set; }

		[Export ("inputBias")]
		nfloat InputBias { get; set; }

		[Export ("inputOrigin", ArgumentSemantic.Weak)]
		SCNNode InputOrigin { get; set; }

		[Export ("inputProperty")]
		NSString InputProperty { get; set; }
	}

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[BaseType (typeof (SCNConstraint))]
	interface SCNBillboardConstraint {
		[Static]
		[Export ("billboardConstraint")]
		SCNBillboardConstraint Create ();

		[Export ("freeAxes", ArgumentSemantic.Assign)]
		SCNBillboardAxis FreeAxes { get; set; }
	}

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[BaseType (typeof (SCNNode))]
	[DisableDefaultCtor]
	interface SCNReferenceNode : NSCoding {
		[Export ("initWithURL:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl referenceUrl);

		[Static]
		[Export ("referenceNodeWithURL:")]
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

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[Protocol]
	interface SCNBufferStream {
		[Abstract]
		[Export ("writeBytes:length:")]
		unsafe void Length (IntPtr bytes, nuint length);
	}

	[Watch (4,0), TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface SCNTimingFunction : NSSecureCoding
	{
		[Static]
		[Export ("functionWithTimingMode:")]
		SCNTimingFunction Create (SCNActionTimingMode timingMode);
	
		[Static, NoWatch]
		[Export ("functionWithCAMediaTimingFunction:")]
		SCNTimingFunction Create (CAMediaTimingFunction caTimingFunction);
	}

	// Use the Swift name SCNAnimationProtocol since it conflicts with the type name
	[Protocol (Name="SCNAnimation")]
	interface SCNAnimationProtocol {
	}

	interface ISCNAnimationProtocol {}

	[Watch (4,0), TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface SCNAnimation : SCNAnimationProtocol, NSCopying, NSSecureCoding
	{
		[Static]
		[Export ("animationWithContentsOfURL:")]
		SCNAnimation FromUrl (NSUrl animationUrl);
	
		[Static]
		[Export ("animationNamed:")]
		SCNAnimation FromName (string animationName);
	
		[Static, NoWatch]
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
	
		[NullAllowed, Export ("animationDidStart", ArgumentSemantic.Copy)]
		SCNAnimationDidStartHandler AnimationDidStart { get; set; }
	
		[NullAllowed, Export ("animationDidStop", ArgumentSemantic.Copy)]
		SCNAnimationDidStopHandler AnimationDidStop { get; set; }
	
		[NullAllowed, Export ("animationEvents", ArgumentSemantic.Copy)]
		SCNAnimationEvent[] AnimationEvents { get; set; }
	
		[Export ("additive")]
		bool Additive { [Bind ("isAdditive")] get; set; }
	
		[Export ("cumulative")]
		bool Cumulative { [Bind ("isCumulative")] get; set; }
	}
	
	[Watch (4,0), TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface SCNAnimationPlayer : SCNAnimatable, NSCopying, NSSecureCoding
	{
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

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
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

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
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

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
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

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
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

	interface ISCNAvoidOccluderConstraintDelegate { }

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SCNAvoidOccluderConstraintDelegate {

		[Export ("avoidOccluderConstraint:shouldAvoidOccluder:forNode:")]
		bool ShouldAvoidOccluder (SCNAvoidOccluderConstraint constraint, SCNNode occluder, SCNNode node);

		[Export ("avoidOccluderConstraint:didAvoidOccluder:forNode:")]
		void DidAvoidOccluder (SCNAvoidOccluderConstraint constraint, SCNNode occluder, SCNNode node);
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
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

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(SCNPhysicsBehavior))]
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
