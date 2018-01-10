using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreAnimation;
using CoreText;

using CGGlyph = System.UInt16;

namespace AppKit {
	public partial class NSFont
	{
		public static NSFont FromCTFont (CTFont font)
		{
			if (font == null)
				return null;
			return new NSFont (font.Handle);
		}

		[Mac (10,13)]
		public unsafe CGRect [] GetBoundingRects (CGGlyph [] glyphs)
		{
			if (glyphs == null)
				throw new ArgumentNullException ("glyphs");
			if (glyphs.Length < 1)
				throw new ArgumentException ("glyphs array is empty");

			CGRect [] bounds = new CGRect [glyphs.Length]; 
			fixed (CGRect* boundsPtr = &bounds [0]) {
				fixed (CGGlyph* glyphsPtr = &glyphs [0]) {
					_GetBoundingRects ((IntPtr)boundsPtr, (IntPtr)glyphsPtr, (nuint)glyphs.Length);
				}
			}
			return bounds;
		}

		[Mac (10,13)]
		public unsafe CGSize [] GetAdvancements (CGGlyph [] glyphs)
		{
			if (glyphs == null)
				throw new ArgumentNullException ("glyphs");
			if (glyphs.Length < 1)
				throw new ArgumentException ("glyphs array is empty");

			CGSize [] advancements = new CGSize [glyphs.Length]; 
			fixed (CGSize* advancementsPtr = &advancements [0]) {
				fixed (CGGlyph* glyphsPtr = &glyphs [0]) {
					_GetAdvancements ((IntPtr)advancementsPtr, (IntPtr)glyphsPtr, (nuint)glyphs.Length);
				}
			}
			return advancements;
		}
	}
}
