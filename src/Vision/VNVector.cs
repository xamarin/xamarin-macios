//
// VNVector.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

using System;
using Foundation;
using ObjCRuntime;

namespace Vision {
	public partial class VNVector {

		public static VNVector Create (double r, double theta)
		{
			var vector = new VNVector (NSObjectFlag.Empty);
			vector.InitializeHandle (vector.InitWithRTheta (r, theta));
			return vector;
		}
	}
}