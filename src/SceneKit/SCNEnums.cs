//
// SCNPhysicsShape.cs: extensions to SCNPhysicsShape
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

#nullable enable

namespace SceneKit {

	[Mac (10, 10)]
	[iOS (8, 0)]
	public enum SCNPhysicsShapeType
	{
		ConvexHull,
		BoundingBox,
		ConcavePolyhedron
	}
}
