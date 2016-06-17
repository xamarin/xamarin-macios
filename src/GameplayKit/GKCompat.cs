// Compatibility stubs

using System;
using OpenTK;

#if !XAMCORE_4_0 && !MONOMAC

namespace XamCore.GameplayKit {

	public partial class GKQuadTree {

		[Obsolete ("Use constructor with the same signature")]
		public static GKQuadTree QuadTreeWithMinPosition (Vector2 min, Vector2 max, float minCellSize)
		{
			return new GKQuadTree (min, max, minCellSize);
		}
	}
}

#endif
