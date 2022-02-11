//
// GKGridGraph.cs: Implements some nicer methods for GKGridGraph
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;
using System.Runtime.Versioning;

#if NET
using Vector2i = global::CoreGraphics.NVector2i;
#else
using Vector2i = global::OpenTK.Vector2i;
#endif // NET

namespace GameplayKit {
#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class GKGridGraph {
		
#if !NET
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
