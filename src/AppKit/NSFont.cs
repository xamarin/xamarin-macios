using System;
using System.Runtime.InteropServices;
using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreGraphics;
using XamCore.CoreAnimation;

using CGGlyph = System.UInt16;

namespace XamCore.AppKit {
	public partial class NSFont
	{
		[Mac (10,13)]
		public unsafe void GetBoundingRects (CGRect [] bounds, CGGlyph [] glyphs, nuint glyphCount)
		{
			if (bounds == null)
				throw new ArgumentNullException ("bounds");
			if (bounds.Length < 1)
				throw new ArgumentException ("bounds array is empty");

			if (glyphs == null)
				throw new ArgumentNullException ("glyphs");
			if (glyphs.Length < 1)
				throw new ArgumentException ("glyphs array is empty");

			fixed (CGRect* boundsPtr = &bounds [0])
				fixed (CGGlyph* glyphsPtr = &glyphs [0])
					_GetBoundingRects ((IntPtr)boundsPtr, (IntPtr)glyphsPtr, (nuint)glyphs.Length);
		}

		[Mac (10,13)]
		public unsafe void GetAdvancements (CGSize [] advancements, CGGlyph [] glyphs, nuint glyphCount)
		{
			if (advancements == null)
				throw new ArgumentNullException ("advancements");
			if (advancements.Length < 1)
				throw new ArgumentException ("advancements array is empty");

			if (glyphs == null)
				throw new ArgumentNullException ("glyphs");
			if (glyphs.Length < 1)
				throw new ArgumentException ("glyphs array is empty");

			fixed (CGSize* advancementsPtr = &advancements [0])
				fixed (CGGlyph* glyphsPtr = &glyphs [0])
					_GetAdvancements ((IntPtr)advancementsPtr, (IntPtr)glyphsPtr, (nuint)glyphs.Length);
		}
	}
}
