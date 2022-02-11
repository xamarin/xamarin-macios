#if !__MACCATALYST__
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

namespace AppKit {

#if NET
	[SupportedOSPlatform ("macos10.15")]
#else
	[Mac (10, 15)]
#endif
	public enum NSCollectionLayoutAnchorOffsetType {
		Absolute,
		Fractional,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("maccatalyst13.0")]
#endif
	public partial class NSCollectionLayoutAnchor {
		public static NSCollectionLayoutAnchor Create (NSDirectionalRectEdge edges, NSCollectionLayoutAnchorOffsetType offsetType, CGPoint offset) =>
		    offsetType switch
		    {
			    NSCollectionLayoutAnchorOffsetType.Absolute   => CreateFromAbsoluteOffset (edges, offset),
			    NSCollectionLayoutAnchorOffsetType.Fractional => CreateFromFractionalOffset (edges, offset),
			    _                                             => throw new ArgumentException (message: "Invalid enum value", paramName: nameof (offsetType)),
		    };
	}
}
#endif // !__MACCATALYST__
