//
// NSLayoutManager.cs: 
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013, Xamarin Inc
//

using System;
#if IOS
using System.Drawing;
#endif
using System.Runtime.InteropServices;
using System.Collections;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

#if MONOMAC
using UIFont = AppKit.NSFont;
#endif

#if MONOMAC
namespace AppKit {
#else
namespace UIKit {
#endif
	partial class NSLayoutManager {
		public unsafe nuint GetGlyphs (
			NSRange glyphRange,
			short [] /* CGGlyph* = CGFontIndex* = unsigned short* */ glyphBuffer,
			NSGlyphProperty [] /* NSGlyphProperty* = nint* */ props,
			nuint [] /* NSUInteger */ charIndexBuffer,
			byte [] /* (unsigned char *) */ bidiLevelBuffer)
		{
			if (glyphBuffer is not null && glyphBuffer.Length < glyphRange.Length)
				throw new ArgumentOutOfRangeException (string.Format ("glyphBuffer must have at least {0} elements", glyphRange.Length));

			if (props is not null && props.Length < glyphRange.Length)
				throw new ArgumentOutOfRangeException (string.Format ("props must have at least {0} elements", glyphRange.Length));

			if (charIndexBuffer is not null && charIndexBuffer.Length < glyphRange.Length)
				throw new ArgumentOutOfRangeException (string.Format ("props must have at least {0} elements", glyphRange.Length));

			if (bidiLevelBuffer is not null && bidiLevelBuffer.Length < glyphRange.Length)
				throw new ArgumentOutOfRangeException (string.Format ("bidiLevelBuffer must have at least {0} elements", glyphRange.Length));

			fixed (short* glyphs = glyphBuffer) {
				nuint rv;
				// Unified/64 + Classic: the input array is the correct size
				var tmpArray = props;
				fixed (void* properties = tmpArray) {
					fixed (nuint* charIBuffer = charIndexBuffer) {
						fixed (byte* bidi = bidiLevelBuffer) {
							rv = GetGlyphs (glyphRange, (IntPtr) glyphs, (IntPtr) properties, (IntPtr) charIBuffer, (IntPtr) bidi);
						}
					}
				}

				return rv;
			}
		}

#if !NET && !__MACCATALYST__
#if MONOMAC
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the overload that takes 'nint glyphCount' instead.")]
		public unsafe void ShowGlyphs (
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the 'ShowGlyphs' overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the 'ShowGlyphs' overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the 'ShowGlyphs' overload that takes 'nint glyphCount' instead.")]
		public unsafe void ShowCGGlyphs (
#endif // MONOMAC
			short [] /* const CGGlyph* = CGFontIndex* = unsigned short* */ glyphs,
			CGPoint [] /* const CGPoint* */ positions,
			nuint /* NSUInteger */ glyphCount,
			UIFont font,
			CGAffineTransform textMatrix,
			NSDictionary attributes,
			CGContext graphicsContext)
		{
			fixed (short* gl = glyphs) {
				fixed (CGPoint* pos = positions) {
					ShowGlyphs ((IntPtr) gl, (IntPtr) pos, glyphCount, font, textMatrix, attributes, graphicsContext);
				}
			}
		}
#endif // !NET

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		public unsafe void ShowGlyphs (
			short [] /* const CGGlyph* = CGFontIndex* = unsigned short* */ glyphs,
			CGPoint [] /* const CGPoint* */ positions,
			nint /* NSInteger */ glyphCount,
			UIFont font,
			CGAffineTransform textMatrix,
			NSDictionary attributes,
			CGContext graphicsContext)
		{
			fixed (short* gl = glyphs) {
				fixed (CGPoint* pos = positions) {
					ShowGlyphs ((IntPtr) gl, (IntPtr) pos, glyphCount, font, textMatrix, attributes, graphicsContext);
				}
			}
		}

#if !NET && !MONOMAC
		// TextContainerForGlyphAtIndex
		[Obsolete ("Use 'GetTextContainer' instead.")]
		public NSTextContainer TextContainerForGlyphAtIndex (nuint glyphIndex)
		{
			return GetTextContainer (glyphIndex);
		}

		[Obsolete ("Use 'GetTextContainer' instead.")]
		public NSTextContainer TextContainerForGlyphAtIndex (nuint glyphIndex, ref NSRange effectiveGlyphRange)
		{
			return GetTextContainer (glyphIndex, out effectiveGlyphRange);
		}

		// LineFragmentRectForGlyphAtIndex
		[Obsolete ("Use 'GetLineFragmentRect' instead.")]
		public CGRect LineFragmentRectForGlyphAtIndex (nuint glyphIndex)
		{
			return GetLineFragmentRect (glyphIndex);
		}

		[Obsolete ("Use 'GetLineFragmentRect' instead.")]
		public CGRect LineFragmentRectForGlyphAtIndex (nuint glyphIndex, ref NSRange effectiveGlyphRange)
		{
			return GetLineFragmentRect (glyphIndex, out effectiveGlyphRange);
		}

		// LineFragmentUsedRectForGlyphAtIndex
		[Obsolete ("Use 'GetLineFragmentUsedRect' instead.")]
		public CGRect LineFragmentUsedRectForGlyphAtIndex (nuint glyphIndex)
		{
			return GetLineFragmentUsedRect (glyphIndex);
		}

		[Obsolete ("Use 'GetLineFragmentUsedRect' instead.")]
		public CGRect LineFragmentUsedRectForGlyphAtIndex (nuint glyphIndex, ref NSRange effectiveGlyphRange)
		{
			return GetLineFragmentUsedRect (glyphIndex, out effectiveGlyphRange);
		}

		// GlyphRangeForCharacterRange
		[Obsolete ("Use 'GetGlyphRange' instead.")]
		public NSRange GlyphRangeForCharacterRange (NSRange charRange)
		{
			return GetGlyphRange (charRange);
		}

		[Obsolete ("Use 'GetGlyphRange' instead.")]
		public NSRange GlyphRangeForCharacterRange (NSRange charRange, ref NSRange actualCharRange)
		{
			return GetGlyphRange (charRange, out actualCharRange);
		}

		// CharacterRangeForGlyphRange
		[Obsolete ("Use 'GetCharacterRange' instead.")]
		public NSRange CharacterRangeForGlyphRange (NSRange charRange)
		{
			return GetCharacterRange (charRange);
		}

		[Obsolete ("Use 'GetCharacterRange' instead.")]
		public NSRange CharacterRangeForGlyphRange (NSRange charRange, ref NSRange actualCharRange)
		{
			return GetCharacterRange (charRange, out actualCharRange);
		}
#endif // !NET && !MONOMAC

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

					if (positions is not null && (ulong) positions.Length < (ulong) rv)
						throw new ArgumentException (string.Format ("Memory corruption: the 'positions' array was not big enough to hold the number of insertion points. {0} insertion points were returned, while the array's Length is only {1}", rv, positions.Length));

					if (charIndexes is not null && (ulong) charIndexes.Length < (ulong) rv)
						throw new ArgumentException (string.Format ("Memory corruption: the 'charIndexes' array was not big enough to hold the number of insertion points. {0} insertion points were returned, while the array's Length is only {1}", rv, charIndexes.Length));

					return rv;
				}
			}
		}
	}
}
