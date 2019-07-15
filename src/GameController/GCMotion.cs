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

	// Removed the Headers for these structs because I could not compile with them.
	// [iOS (13,0), Mac (10,15, onlyOn64: true), TV (13,0)]
	public struct GCAcceleration {

		public double X;

		public double Y;

		public double Z;
	}

	// [iOS (13,0), Mac (10,15, onlyOn64: true), TV (13,0)]
	public struct GCRotationRate {

		public double X;

		public double Y;

		public double Z;
	}

	// [iOS (13,0), Mac (10,15, onlyOn64: true), TV (13,0)]
	public struct GCQuaternion {

		public double X;

		public double Y;

		public double Z;

		public double W;
	}
}
