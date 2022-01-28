// Compatibility stubs

using System;
using Foundation;
using ObjCRuntime;
using OpenTK;
using System.Runtime.Versioning;

#if !XAMCORE_4_0 && !MONOMAC && !__MACCATALYST__

namespace GameplayKit {

	public partial class GKQuadTree {

#if !NET
		[Obsolete ("Use the constructor with the same signature.")]
#else
		[Obsolete ("Use the constructor with the same signature.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static GKQuadTree QuadTreeWithMinPosition (Vector2 min, Vector2 max, float minCellSize)
		{
			return new GKQuadTree (min, max, minCellSize);
		}

#if !NET
		[Deprecated (PlatformName.iOS, 10,0, message: "Empty stub (always return 'false') as this API is now rejected).")]
		[Deprecated (PlatformName.TvOS, 10,0, message: "Empty stub (always return 'false') as this API is now rejected).")]
#else
		[UnsupportedOSPlatform ("ios10.0")]
		[UnsupportedOSPlatform ("tvos10.0")]
#if IOS
		[Obsolete ("Starting with ios10.0 empty stub (always return 'false') as this API is now rejected).", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos10.0 empty stub (always return 'false') as this API is now rejected).", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public virtual bool RemoveData (NSObject data)
		{
			return false;
		}
	}

	public partial class GKQuadTreeNode {

#if !NET
		[Obsolete ("A valid instance of this type cannot be directly created.")]
#else
		[Obsolete ("A valid instance of this type cannot be directly created.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public GKQuadTreeNode ()
		{
		}
	}
}

#endif
