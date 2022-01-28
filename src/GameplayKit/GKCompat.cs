// Compatibility stubs

using System;
using Foundation;
using ObjCRuntime;
using OpenTK;
using System.Runtime.Versioning;

#if !XAMCORE_4_0 && !MONOMAC && !__MACCATALYST__

namespace GameplayKit {

	public partial class GKQuadTree {

		[Obsolete ("Use the constructor with the same signature.")]
		public static GKQuadTree QuadTreeWithMinPosition (Vector2 min, Vector2 max, float minCellSize)
		{
			return new GKQuadTree (min, max, minCellSize);
		}

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[UnsupportedOSPlatform ("tvos10.0")]
		[UnsupportedOSPlatform ("ios10.0")]
#if TVOS
		[Obsolete ("Starting with tvos10.0 empty stub (always return 'false') as this API is now rejected).", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios10.0 empty stub (always return 'false') as this API is now rejected).", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 10,0, message: "Empty stub (always return 'false') as this API is now rejected).")]
		[Deprecated (PlatformName.TvOS, 10,0, message: "Empty stub (always return 'false') as this API is now rejected).")]
#endif
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
