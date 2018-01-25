// Copyright 2016 Xamarin Inc. All rights reserved.

#if !XAMCORE_3_0

using System;
using System.Runtime.InteropServices;

namespace CoreMotion {

	public partial class CMSensorRecorder {

		[Obsolete ("Apple removed this API in iOS 9.3.")]
		public virtual CMSensorDataList GetAccelerometerDataSince (ulong identifier)
		{
			return null;
		}
	}
}

#endif
