// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
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
}
