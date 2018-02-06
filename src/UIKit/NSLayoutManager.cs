//
// NSLayoutManager.cs: 
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013, Xamarin Inc
//

#if !WATCH

using System;
#if IOS
using System.Drawing;
#endif
using System.Runtime.InteropServices;
using System.Collections;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace UIKit {
	partial class NSLayoutManager {
		public unsafe nuint GetGlyphs (
			NSRange glyphRange, 
			short[] /* CGGlyph* = CGFontIndex* = unsigned short* */ glyphBuffer,
			NSGlyphProperty[] /* NSGlyphProperty* = nint* */ props,
			nuint[] /* NSUInteger */ charIndexBuffer,
			byte[] /* (unsigned char *) */ bidiLevelBuffer)
		{
			if (glyphBuffer != null && glyphBuffer.Length < glyphRange.Length)
				throw new ArgumentOutOfRangeException (string.Format ("glyphBuffer must have at least {0} elements", glyphRange.Length));

			if (props != null && props.Length < glyphRange.Length)
				throw new ArgumentOutOfRangeException (string.Format ("props must have at least {0} elements", glyphRange.Length));
			
			if (charIndexBuffer != null && charIndexBuffer.Length < glyphRange.Length)
				throw new ArgumentOutOfRangeException (string.Format ("props must have at least {0} elements", glyphRange.Length));

			if (bidiLevelBuffer != null && bidiLevelBuffer.Length < glyphRange.Length)
				throw new ArgumentOutOfRangeException (string.Format ("bidiLevelBuffer must have at least {0} elements", glyphRange.Length));

			fixed (short* glyphs = glyphBuffer) {
				nuint rv;
#if XAMCORE_2_0 && ARCH_32
				// Unified/32: the output array is not the correct size, it needs to be int[], and it's an array of NSGlyphProperty (which is long)
				var tmpArray = new nint [props.Length];
				fixed (nint *properties = tmpArray) {
#else
				// Unified/64 + Classic: the input array is the correct size
				fixed (NSGlyphProperty *properties = props) {
#endif
					fixed (nuint* charIBuffer = charIndexBuffer) {
						fixed (byte* bidi = bidiLevelBuffer) {
							rv = GetGlyphsInternal (glyphRange, (IntPtr) glyphs, (IntPtr) properties, (IntPtr) charIBuffer, (IntPtr) bidi);
						}
					}
				}
#if XAMCORE_2_0 && ARCH_32
				// Marshal back from the tmpArray.
				for (int i = 0; i < props.Length; i++)
					props [i] = (NSGlyphProperty) (long) tmpArray [i];
#endif

				return rv;
			}
		}

		public unsafe void ShowCGGlyphs (
			short[] /* const CGGlyph* = CGFontIndex* = unsigned short* */ glyphs,
			CGPoint[] /* const CGPoint* */ positions,
			nuint /* NSUInteger */ glyphCount,
			UIFont font,
			CGAffineTransform textMatrix,
			NSDictionary attributes,
			CGContext graphicsContext)
		{
			fixed (short* gl = glyphs) {
				fixed (CGPoint* pos = positions) {
					ShowCGGlyphsInternal ((IntPtr) gl, (IntPtr) pos, glyphCount, font, textMatrix, attributes, graphicsContext);
				}
			}
		}

		// TextContainerForGlyphAtIndex
		public NSTextContainer TextContainerForGlyphAtIndex (nuint glyphIndex)
		{
			return TextContainerForGlyphAtIndexInternal (glyphIndex, IntPtr.Zero);
		}
		
		public unsafe NSTextContainer TextContainerForGlyphAtIndex (nuint glyphIndex, ref NSRange effectiveGlyphRange)
		{
			fixed (NSRange* ptr = &effectiveGlyphRange)
				return TextContainerForGlyphAtIndexInternal (glyphIndex, (IntPtr) ptr);
		}

		// LineFragmentRectForGlyphAtIndex
		public CGRect LineFragmentRectForGlyphAtIndex (nuint glyphIndex)
		{
			return LineFragmentRectForGlyphAtIndexInternal (glyphIndex, IntPtr.Zero);
		}

		public unsafe CGRect LineFragmentRectForGlyphAtIndex (nuint glyphIndex, ref NSRange effectiveGlyphRange)
		{
			fixed (NSRange* ptr = &effectiveGlyphRange)
				return LineFragmentRectForGlyphAtIndexInternal (glyphIndex, (IntPtr) ptr);
		}

		// LineFragmentUsedRectForGlyphAtIndex
		public CGRect LineFragmentUsedRectForGlyphAtIndex (nuint glyphIndex)
		{
			return LineFragmentUsedRectForGlyphAtIndexInternal (glyphIndex, IntPtr.Zero);
		}

		public unsafe CGRect LineFragmentUsedRectForGlyphAtIndex (nuint glyphIndex, ref NSRange effectiveGlyphRange)
		{
			fixed (NSRange* ptr = &effectiveGlyphRange)
				return LineFragmentUsedRectForGlyphAtIndexInternal (glyphIndex, (IntPtr) ptr);
		}

		// GlyphRangeForCharacterRange
		public NSRange GlyphRangeForCharacterRange (NSRange charRange)
		{
			return GlyphRangeForCharacterRangeInternal (charRange, IntPtr.Zero);
		}

		public unsafe NSRange GlyphRangeForCharacterRange (NSRange charRange, ref NSRange actualCharRange)
		{
			fixed (NSRange* ptr = &actualCharRange)
				return GlyphRangeForCharacterRangeInternal (charRange, (IntPtr) ptr);
		}
		
		// CharacterRangeForGlyphRange
		public NSRange CharacterRangeForGlyphRange (NSRange charRange)
		{
			return CharacterRangeForGlyphRangeInternal (charRange, IntPtr.Zero);
		}

		public unsafe NSRange CharacterRangeForGlyphRange (NSRange charRange, ref NSRange actualCharRange)
		{
			fixed (NSRange* ptr = &actualCharRange)
				return CharacterRangeForGlyphRangeInternal (charRange, (IntPtr) ptr);
		}

		public unsafe nuint GetLineFragmentInsertionPoints (
			nuint /* NSUInteger */ charIndex, 
			bool /* BOOL */ alternatePosition,
			bool /* BOOL */ inDisplayOrder,
			nfloat [] /* CGFloat* */ positions,
			nint [] /* NSUInteger* */ charIndexes)
		{
			fixed (nfloat* p = positions) {
				fixed (nint* c = charIndexes) {
					var rv = GetLineFragmentInsertionPoints (charIndex, alternatePosition, inDisplayOrder, (IntPtr) p, (IntPtr) c);

					// I can't find an API to check this before the call :(

					if (positions != null && (ulong) positions.Length < (ulong) rv)
						throw new ArgumentException (string.Format ("Memory corruption: the 'positions' array was not big enough to hold the number of insertion points. {0} insertion points were returned, while the array's Length is only {1}", rv, positions.Length));

					if (charIndexes != null && (ulong) charIndexes.Length < (ulong) rv)
						throw new ArgumentException (string.Format ("Memory corruption: the 'charIndexes' array was not big enough to hold the number of insertion points. {0} insertion points were returned, while the array's Length is only {1}", rv, charIndexes.Length));

					return rv;
				}
			}
		}
	}
}

#endif // !WATCH
