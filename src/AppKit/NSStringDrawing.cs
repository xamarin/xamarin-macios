#if !__MACCATALYST__
using System;
using System.Runtime.Versioning;
using AppKit;
using Foundation;
using CoreGraphics;

#nullable enable

namespace AppKit {

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	// Manual bindings, easier than make the generator support extension methods on non-NSObject-derived types (string in this case).
	public unsafe static partial class NSStringDrawing {
		public static void DrawAtPoint (this string This, CGPoint point, NSDictionary? attributes)
		{
			using (var self = ((NSString) This))
				self.DrawAtPoint (point, attributes);
		}

		public static void DrawAtPoint (this string This, CGPoint point, NSStringAttributes? attributes)
		{
			This.DrawAtPoint (point, attributes is null ? null : attributes.Dictionary);
		}

		public static void DrawInRect (this string This, CGRect rect, NSDictionary? attributes)
		{
			using (var self = ((NSString) This))
				self.DrawInRect (rect, attributes);
		}

		public static void DrawInRect (this string This, CGRect rect, NSStringAttributes? attributes)
		{
			This.DrawInRect (rect, attributes is null ? null : attributes.Dictionary);
		}

		public static CGSize StringSize (this string This, NSDictionary? attributes)
		{
			using (var self = ((NSString) This))
				return self.StringSize (attributes);
		}

		public static CGSize StringSize (this string This, NSStringAttributes? attributes)
		{
			return This.StringSize (attributes is null ? null : attributes.Dictionary);
		}
	}
}
#endif // !__MACCATALYST__
