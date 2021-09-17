//
// SCNEnums.cs: enumerations
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;

#nullable enable

namespace SceneKit {

	[Mac (10, 10)]
	[iOS (8, 0)]
	public enum SCNPhysicsShapeType {
		ConvexHull,
		BoundingBox,
		ConcavePolyhedron,
	}
}
