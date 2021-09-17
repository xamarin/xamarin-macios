// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using System.Threading.Tasks;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

#if WATCH
using AnimationType = global::SceneKit.ISCNAnimationProtocol;
#else
using CoreAnimation;
using AnimationType = global::CoreAnimation.CAAnimation;
#endif

#nullable enable

namespace SceneKit {

	partial class SCNAction {

#if !XAMCORE_4_0
		[Obsolete ("Use 'TimingFunction2' property.")]
		public virtual Action<float>? TimingFunction {
			get {
				if (TimingFunction2 == null)
					return null;
				else
					return (f) => {
						TimingFunction2 (f);
					};
			}
			set {
				if (value == null)
					TimingFunction2 = null;
				else
					TimingFunction2 = (f) => {
						value (f);
						return float.NaN;
					};
			}

		}
#endif

#if !XAMCORE_3_0
		[Obsolete ("Use 'TimingFunction2' property.")]
		public virtual void SetTimingFunction (Action<float> timingFunction)
		{
			TimingFunction = timingFunction;
		}
#endif
	}
#if TVOS && !XAMCORE_4_0
	partial class SCNMaterialProperty {
#if !NET
	[iOS (8, 0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "This API has been totally removed on iOS.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "This API has been totally removed on tvOS.")]
#else
		[UnsupportedOSPlatform ("ios10.0")]
		[UnsupportedOSPlatform ("tvos10.0")]
#if TVOS
		[Obsolete ("Starting with tvos10.0 this API has been totally removed on tvOS.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios10.0 this API has been totally removed on iOS.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public virtual NSObject? BorderColor { get; set; }
	}

	partial class SCNRenderer {
#if !NET
		[iOS (8, 0)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "This API has been totally removed on iOS.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "This API has been totally removed on tvOS.")]
#else
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("tvos10.0")]
#if TVOS
		[Obsolete ("Starting with tvos10.0 this API has been totally removed on tvOS.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios9.0 this API has been totally removed on iOS.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public virtual void Render ()
		{
		}
	}
#endif

#if MONOMAC && !XAMCORE_4_0
	partial class SCNScene {
#if !NET
		[Obsolete ("Use the 'ISCNSceneExportDelegate' overload instead.")]
		[Mac (10, 9)]
#else
		[Obsolete ("Use the 'ISCNSceneExportDelegate' overload instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public virtual bool WriteToUrl (NSUrl url, SCNSceneLoadingOptions options, SCNSceneExportDelegate handler, SCNSceneExportProgressHandler exportProgressHandler)
		{
			return WriteToUrl (url: url, options: options == null ? null : options.Dictionary, aDelegate: handler, exportProgressHandler: exportProgressHandler);
		}

#if !NET
		[Obsolete ("Use the 'ISCNSceneExportDelegate' overload instead.")]
		[Mac (10, 9)]
#else
		[Obsolete ("Use the 'ISCNSceneExportDelegate' overload instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public virtual bool WriteToUrl (NSUrl url, NSDictionary options, SCNSceneExportDelegate handler, SCNSceneExportProgressHandler exportProgressHandler)
		{
			return WriteToUrl (url: url, options: options, aDelegate: handler, exportProgressHandler: exportProgressHandler);
		}
	}
#endif

#if !XAMCORE_4_0
	public abstract partial class SCNSceneRenderer : NSObject {
#if !NET
		[Mac (10, 10)]
		[Obsolete ("Use 'SCNSceneRenderer_Extensions.PrepareAsync' instead.")]
#else
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[Obsolete ("Use 'SCNSceneRenderer_Extensions.PrepareAsync' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public unsafe virtual Task<bool> PrepareAsync (NSObject[] objects)
		{
			return SCNSceneRenderer_Extensions.PrepareAsync (this, objects);
		}

#if !NET
		[iOS (9, 0)]
		[Mac (10, 11, 0, PlatformArchitecture.Arch64)]
		[Obsolete ("Use 'SCNSceneRenderer_Extensions.PresentSceneAsync' instead.")]
#else
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[Obsolete ("Use 'SCNSceneRenderer_Extensions.PresentSceneAsync' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public unsafe virtual Task PresentSceneAsync (SCNScene scene, global::SpriteKit.SKTransition transition, SCNNode pointOfView)
		{
			return SCNSceneRenderer_Extensions.PresentSceneAsync (this, scene, transition, pointOfView);
		}

	}
#endif


#if !XAMCORE_4_0
#if !NET
	[Mac (10,9), iOS (8,0), Watch (4,0)]
#endif
	public delegate void SCNAnimationEventHandler (AnimationType animation, NSObject animatedObject, bool playingBackward);

	public partial class SCNAnimationEvent : NSObject
	{
		public static SCNAnimationEvent Create (nfloat keyTime, SCNAnimationEventHandler eventHandler)
		{
			var handler = new Action<IntPtr, NSObject, bool> ((animationPtr, animatedObject, playingBackward) => {
				var animation = Runtime.GetINativeObject<AnimationType> (animationPtr, true);
				eventHandler (animation, animatedObject, playingBackward);
			});
			return Create (keyTime, handler);
		}
	}
#endif

#if !WATCH && !XAMCORE_4_0
#if !NET
	[iOS (11,0)]
	[TV (11,0)]
	[Mac (10,13,0, PlatformArchitecture.Arch64)]
#else
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
#endif
	static public partial class SCNAnimatableExtensions {
		static public void AddAnimation (this ISCNAnimatable self, SCNAnimation animation, string key)
		{
			using (var ca = CAAnimation.FromSCNAnimation (animation))
			using (var st = key != null ? new NSString (key) : null)
				self.AddAnimation (ca, st);
		}
	}
#endif

#if !XAMCORE_4_0
#if !NET
	[Watch (3,0)]
#endif
	public partial class SCNHitTestOptions {
		[Obsolete ("Use 'SearchMode' instead.")]
		public SCNHitTestSearchMode? OptionSearchMode {
			get {
				return SearchMode;
			}
		}
	}

#if !MONOMAC && !WATCH && !__MACCATALYST__
	public partial class SCNView {
#if !NET
		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Obsolete ("Empty stub. (not a public API).")]
#else
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[Obsolete ("Empty stub. (not a public API).", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public virtual bool DrawableResizesAsynchronously { get; set; } 
	}
#endif
#endif
}
