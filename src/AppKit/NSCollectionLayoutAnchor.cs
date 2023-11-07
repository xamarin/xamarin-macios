#if !__MACCATALYST__
using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

#nullable enable

namespace AppKit {

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public enum NSCollectionLayoutAnchorOffsetType {
		Absolute,
		Fractional,
	}

	public partial class NSCollectionLayoutAnchor {
		public static NSCollectionLayoutAnchor Create (NSDirectionalRectEdge edges, NSCollectionLayoutAnchorOffsetType offsetType, CGPoint offset) =>
			offsetType switch {
				NSCollectionLayoutAnchorOffsetType.Absolute => CreateFromAbsoluteOffset (edges, offset),
				NSCollectionLayoutAnchorOffsetType.Fractional => CreateFromFractionalOffset (edges, offset),
				_ => throw new ArgumentException (message: "Invalid enum value", paramName: nameof (offsetType)),
			};
	}
}
#endif // !__MACCATALYST__
