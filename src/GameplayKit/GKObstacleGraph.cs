//
// GKObstacleGraph.cs: Implements Generic variant of GKObstacleGraph
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 || !MONOMAC

using System;
using Foundation;
using ObjCRuntime;

namespace GameplayKit {

	public partial class GKObstacleGraph {
		public
#if !XAMCORE_4_0
		virtual
#endif
		GKGraphNode2D [] GetNodes (GKPolygonObstacle obstacle)
		{
			return NSArray.ArrayFromHandle<GKGraphNode2D> (_GetNodes (obstacle));
		}
	}

#if XAMCORE_2_0
	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	[Register ("GKObstacleGraph", SkipRegistration = true)]
	public partial class GKObstacleGraph<NodeType> : GKObstacleGraph where NodeType : GKGraphNode2D {

		[Preserve (Conditional = true)]
		internal GKObstacleGraph (IntPtr handle) : base (handle)
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
#endif
}
#endif
