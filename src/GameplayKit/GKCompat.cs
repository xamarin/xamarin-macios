// Compatibility stubs

using System;
using OpenTK;

#if !XAMCORE_4_0 && !MONOMAC

namespace GameplayKit {

	public partial class GKQuadTree {

		[Obsolete ("Use the constructor with the same signature.")]
		public static GKQuadTree QuadTreeWithMinPosition (Vector2 min, Vector2 max, float minCellSize)
		{
			return new GKQuadTree (min, max, minCellSize);
		}
	}

	public partial class GKQuadTreeNode {

		[Obsolete ("A valid instance of this type cannot be directly created.")]
		public GKQuadTreeNode ()
		{
		}
	}
}

#endif
