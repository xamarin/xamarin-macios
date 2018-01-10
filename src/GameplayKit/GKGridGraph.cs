//
// GKGridGraph.cs: Implements some nicer methods for GKGridGraph
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 || !MONOMAC
using System;
using ObjCRuntime;
using Vector2i = global::OpenTK.Vector2i;

namespace GameplayKit {
	public partial class GKGridGraph {
		
#if !XAMCORE_4_0
		public virtual GKGridGraphNode GetNodeAt (Vector2i position)
		{
			return GetNodeAt<GKGridGraphNode> (position);
		}
#endif
		public NodeType GetNodeAt<NodeType> (Vector2i position) where NodeType : GKGridGraphNode
		{
			return Runtime.GetNSObject<NodeType> (_GetNodeAt (position));
		}
	}
}
#endif