// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using System.Threading.Tasks;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.SceneKit {

#if !XAMCORE_3_0
	partial class SCNAction {

		[Obsolete ("Use TimingFunction property")]
		public virtual void SetTimingFunction (Action<float> timingFunction)
		{
			TimingFunction = timingFunction;
		}
	}
#elif TVOS && !XAMCORE_4_0
	partial class SCNMaterialProperty {
	[Introduced (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Deprecated")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Deprecated in iOS 10 but removed from tvOS 10")]
		public virtual NSObject BorderColor { get; set; }
	}

	partial class SCNRenderer {
		[Introduced (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Deprecated")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Deprecated in iOS 9 but removed from tvOS 10")]
		public virtual void Render ()
		{
		}
	}
#endif

#if MONOMAC && !XAMCORE_4_0
	partial class SCNScene {
		[Obsolete ("Use the ISCNSceneExportDelegate overload instead")]
		[Mac (10, 9)]
		public virtual bool WriteToUrl (NSUrl url, SCNSceneLoadingOptions options, SCNSceneExportDelegate handler, SCNSceneExportProgressHandler exportProgressHandler)
		{
			return WriteToUrl (url: url, options: options == null ? null : options.Dictionary, aDelegate: handler, exportProgressHandler: exportProgressHandler);
		}

		[Obsolete ("Use the ISCNSceneExportDelegate overload instead")]
		[Mac (10, 9)]
		public virtual bool WriteToUrl (NSUrl url, NSDictionary options, SCNSceneExportDelegate handler, SCNSceneExportProgressHandler exportProgressHandler)
		{
			return WriteToUrl (url: url, options: options, aDelegate: handler, exportProgressHandler: exportProgressHandler);
		}
	}
#endif

#if !XAMCORE_4_0
#if XAMCORE_2_0 || !MONOMAC
	public abstract partial class SCNSceneRenderer : NSObject {
		[Introduced (PlatformName.MacOSX, 10, 10)]
		[Obsolete ("Use 'SCNSceneRenderer_Extensions.PrepareAsync' instead")]
		public unsafe virtual Task<bool> PrepareAsync (NSObject[] objects)
		{
			return SCNSceneRenderer_Extensions.PrepareAsync (this, objects);
		}

		[Introduced (PlatformName.iOS, 9, 0)]
		[Introduced (PlatformName.MacOSX, 10, 11, PlatformArchitecture.Arch64)]
		[Obsolete ("Use 'SCNSceneRenderer_Extensions.PresentSceneAsync' instead")]
		public unsafe virtual Task PresentSceneAsync (SCNScene scene, global::XamCore.SpriteKit.SKTransition transition, SCNNode pointOfView)
		{
			return SCNSceneRenderer_Extensions.PresentSceneAsync (this, scene, transition, pointOfView);
		}
	}
#endif
#endif
}
