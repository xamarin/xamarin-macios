// Copyright 2016 Xamarin Inc. All rights reserved.

#if !XAMCORE_3_0

using System;

namespace XamCore.SceneKit {

	partial class SCNAction {

		[Obsolete ("Use TimingFunction property")]
		public virtual void SetTimingFunction (Action<float> timingFunction)
		{
			TimingFunction = timingFunction;
		}
	}
}

#endif
