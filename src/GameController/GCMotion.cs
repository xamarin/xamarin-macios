//
// GCMotion.cs: extensions to GCMotion iOS API
//
// Authors:
//   TJ Lambert (t-anlamb@microsoft.com)
//
// Copyright 2019 Microsoft Corporation.

using System;

using ObjCRuntime;
using Foundation;

namespace GameController {

	[iOS (13,0), Mac (10,15), TV (13,0)]
	public struct GCAcceleration {
	
		public double x;

		public double y;

		public double z;
	}

	[iOS (13,0), Mac (10,15), TV (13,0)]
	public struct GCRotationRate {

		public double x;

		public double y;

		public double z;
	}

	[iOS (13,0), Mac (10,15), TV (13,0)]
	public struct GCQuaternion {
		
		public double x;

		public double y;

		public double z;

		public double w;
	}
}
