//
// GKObstacleGraph.cs: Implements Generic variant of GKObstacleGraph
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace GameplayKit {

	public partial class GKObstacleGraph {
#if !NET
		public virtual GKGraphNode2D [] GetNodes (GKPolygonObstacle obstacle)
#else
		public GKGraphNode2D [] GetNodes (GKPolygonObstacle obstacle)
#endif
		{
			return NSArray.ArrayFromHandle<GKGraphNode2D> (_GetNodes (obstacle));
		}
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[iOS (10,0)]
	[TV (10,0)]
	[Mac (10,12)]
#endif
	[Register ("GKObstacleGraph", SkipRegistration = true)]
	public partial class GKObstacleGraph<NodeType> : GKObstacleGraph where NodeType : GKGraphNode2D {

		[Preserve (Conditional = true)]
		internal GKObstacleGraph (NativeHandle handle) : base (handle)
		{
		}

		public GKObstacleGraph (NSCoder coder) : base (coder)
		{
		}

		public GKObstacleGraph (GKPolygonObstacle [] obstacles, float bufferRadius) : base (obstacles, bufferRadius, new Class (typeof (NodeType)))
		{
		}

		public static GKObstacleGraph<NodeType> FromObstacles (GKPolygonObstacle [] obstacles, float bufferRadius)
		{
			return Runtime.GetNSObject <GKObstacleGraph<NodeType>> (GraphWithObstacles (obstacles, bufferRadius, new Class (typeof (NodeType))));
		}

		public NodeType [] GetNodes (GKPolygonObstacle obstacle)
		{
			return NSArray.ArrayFromHandle<NodeType> (_GetNodes (obstacle));
		}
	}
}
