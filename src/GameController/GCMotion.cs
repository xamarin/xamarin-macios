//
// GCMotion.cs: extensions to GCMotion iOS API
//
// Authors:
//   TJ Lambert (t-anlamb@microsoft.com)
//
// Copyright 2019 Microsoft Corporation.

using System;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;

namespace GameController {

	[Introduced (PlatformName.iOS, 13, 0)]
	[Introduced (PlatformName.MacOSX, 10, 15)]
	[Introduced (PlatformName.TvOS, 13, 0)]
	public struct GCAcceleration {
		public double X;
		public double Y;
		public double Z;
	}
	[Introduced (PlatformName.iOS, 13, 0)]
	[Introduced (PlatformName.MacOSX, 10, 15)]
	[Introduced (PlatformName.TvOS, 13, 0)]
	public struct GCRotationRate {
		public double X;
		public double Y;
		public double Z;
	}

	[Introduced (PlatformName.iOS, 13, 0)]
	[Introduced (PlatformName.MacOSX, 10, 15)]
	[Introduced (PlatformName.TvOS, 13, 0)]
	public struct GCQuaternion {
		public double X;
		public double Y;
		public double Z;
		public double W;
	}
}
