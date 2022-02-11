//
// VNFeaturePrintObservation.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2019 Microsoft Corporation. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace Vision {
#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class VNFeaturePrintObservation {

		public bool ComputeDistance (out float [] distance, VNFeaturePrintObservation featurePrint, out NSError error)
		{
			distance = new float [ElementCount];
			unsafe {
				fixed (float* outdistance = distance)
					return _ComputeDistance ((IntPtr) outdistance, featurePrint, out error);
			}
		}
	}
}
