// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using System.Threading.Tasks;
using Foundation;
using ObjCRuntime;

#if WATCH
using AnimationType = global::SceneKit.ISCNAnimationProtocol;
#else
using CoreAnimation;
using AnimationType = global::CoreAnimation.CAAnimation;
#endif

#nullable enable

namespace SceneKit {

	partial class SCNAction {

#if !NET
		[Obsolete ("Use 'TimingFunction2' property.")]
		public virtual Action<float>? TimingFunction {
			get {
				if (TimingFunction2 is null)
					return null;
				else
					return (f) => {
						TimingFunction2 (f);
					};
			}
			set {
				if (value is null)
					TimingFunction2 = null;
				else
					TimingFunction2 = (f) => {
						value (f);
						return float.NaN;
					};
			}

		}
#endif // !NET

#if !XAMCORE_3_0
		[Obsolete ("Use 'TimingFunction2' property.")]
		public virtual void SetTimingFunction (Action<float> timingFunction)
		{
			TimingFunction = timingFunction;
		}
#endif // !XAMCORE_3_0
	}
#if TVOS && !NET
	partial class SCNMaterialProperty {
		[Deprecated (PlatformName.iOS, 10, 0, message: "This API has been totally removed on iOS.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "This API has been totally removed on tvOS.")]
		public virtual NSObject? BorderColor { get; set; }
	}
#endif // TVOS && !NET

#if TVOS && !NET
	partial class SCNRenderer {
		[Deprecated (PlatformName.iOS, 9, 0, message: "This API has been totally removed on iOS.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "This API has been totally removed on tvOS.")]
		public virtual void Render ()
		{
		}
	}
#endif // TVOS && !NET

#if MONOMAC && !NET
	partial class SCNScene {
		[Obsolete ("Use the 'ISCNSceneExportDelegate' overload instead.")]
		public virtual bool WriteToUrl (NSUrl url, SCNSceneLoadingOptions options, SCNSceneExportDelegate handler, SCNSceneExportProgressHandler exportProgressHandler)
		{
			return WriteToUrl (url: url, options: options?.Dictionary, aDelegate: handler, exportProgressHandler: exportProgressHandler);
		}

		[Obsolete ("Use the 'ISCNSceneExportDelegate' overload instead.")]
		public virtual bool WriteToUrl (NSUrl url, NSDictionary options, SCNSceneExportDelegate handler, SCNSceneExportProgressHandler exportProgressHandler)
		{
			return WriteToUrl (url: url, options: options, aDelegate: handler, exportProgressHandler: exportProgressHandler);
		}
	}
#endif // MONOMAC && !NET

#if !NET
	public abstract partial class SCNSceneRenderer : NSObject {
		[Obsolete ("Use 'SCNSceneRenderer_Extensions.PrepareAsync' instead.")]
		public unsafe virtual Task<bool> PrepareAsync (NSObject [] objects)
		{
			return SCNSceneRenderer_Extensions.PrepareAsync (this, objects);
		}

		[Obsolete ("Use 'SCNSceneRenderer_Extensions.PresentSceneAsync' instead.")]
		public unsafe virtual Task PresentSceneAsync (SCNScene scene, global::SpriteKit.SKTransition transition, SCNNode pointOfView)
		{
			return SCNSceneRenderer_Extensions.PresentSceneAsync (this, scene, transition, pointOfView);
		}

	}
#endif // !NET


#if !NET
	public delegate void SCNAnimationEventHandler (AnimationType animation, NSObject animatedObject, bool playingBackward);

	public partial class SCNAnimationEvent : NSObject {
		public static SCNAnimationEvent Create (nfloat keyTime, SCNAnimationEventHandler eventHandler)
		{
			var handler = new Action<IntPtr, NSObject, bool> ((animationPtr, animatedObject, playingBackward) => {
				var animation = Runtime.GetINativeObject<AnimationType> (animationPtr, true)!;
				eventHandler (animation, animatedObject, playingBackward);
			});
			return Create (keyTime, handler);
		}
	}
#endif // !NET

#if !WATCH && !NET
	static public partial class SCNAnimatableExtensions {
		static public void AddAnimation (this ISCNAnimatable self, SCNAnimation animation, string key)
		{
			using (var ca = CAAnimation.FromSCNAnimation (animation))
			using (var st = key is not null ? new NSString (key) : null)
				self.AddAnimation (ca, st);
		}
	}
#endif // !WATCH && !NET

#if !NET
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
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[Obsolete ("Empty stub. (not a public API).")]
		public virtual bool DrawableResizesAsynchronously { get; set; }
	}
#endif // !MONOMAC && !WATCH && !__MACCATALYST__
#endif // !NET
}
