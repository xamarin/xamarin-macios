//
// VNFeaturePrintObservation.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2019 Microsoft Corporation. All rights reserved.
//

#nullable enable

using System;
using Foundation;
using ObjCRuntime;

namespace Vision {
	public partial class VNFeaturePrintObservation {

		public bool ComputeDistance (out float [] distance, VNFeaturePrintObservation featurePrint, out NSError? error)
		{
			distance = new float [ElementCount];
			unsafe {
				fixed (float* outdistance = distance)
					return _ComputeDistance ((IntPtr) outdistance, featurePrint, out error);
			}
		}
	}
}
