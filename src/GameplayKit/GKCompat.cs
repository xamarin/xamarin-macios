// Compatibility stubs

#nullable enable

using System;
using Foundation;
using ObjCRuntime;

#if !NET && !MONOMAC && !__MACCATALYST__

using Vector2 = global::OpenTK.Vector2;

namespace GameplayKit {

	public partial class GKQuadTree {

		[Obsolete ("Use the constructor with the same signature.")]
		public static GKQuadTree QuadTreeWithMinPosition (Vector2 min, Vector2 max, float minCellSize)
		{
			return new GKQuadTree (min, max, minCellSize);
		}

		[Deprecated (PlatformName.iOS, 10, 0, message: "Empty stub (always return 'false') as this API is now rejected).")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Empty stub (always return 'false') as this API is now rejected).")]
		public virtual bool RemoveData (NSObject data)
		{
			return false;
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
