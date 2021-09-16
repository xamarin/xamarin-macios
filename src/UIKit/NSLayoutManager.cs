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
using System.Runtime.Versioning;
using System.Collections;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

#if MONOMAC
using UIFont=AppKit.NSFont;
#endif

#if MONOMAC
namespace AppKit {
#else
namespace UIKit {
#endif
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
#if ARCH_32
				// Unified/32: the output array is not the correct size, it needs to be int[], and it's an array of NSGlyphProperty (which is long)
				nint[] tmpArray = null;
				if (props != null)
					tmpArray = new nint [props.Length];
#else
				// Unified/64 + Classic: the input array is the correct size
				var tmpArray = props;
#endif
				fixed (void *properties = tmpArray) {
					fixed (nuint* charIBuffer = charIndexBuffer) {
						fixed (byte* bidi = bidiLevelBuffer) {
							rv = GetGlyphs (glyphRange, (IntPtr) glyphs, (IntPtr) properties, (IntPtr) charIBuffer, (IntPtr) bidi);
						}
					}
				}
#if ARCH_32
				// Marshal back from the tmpArray.
				if (tmpArray != null) {
					for (int i = 0; i < props.Length; i++)
						props [i] = (NSGlyphProperty) (long) tmpArray [i];
				}
#endif

				return rv;
			}
		}

#if !XAMCORE_4_0 && !__MACCATALYST__
#if MONOMAC
#if !NET
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use the overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the overload that takes 'nint glyphCount' instead.")]
#else
		[UnsupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("macos10.15")]
#if IOS
		[Obsolete ("Starting with ios13.0 use the overload that takes 'nint glyphCount' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use the overload that takes 'nint glyphCount' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
		[Obsolete ("Starting with macos10.15 use the overload that takes 'nint glyphCount' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif // !NET
		public unsafe void ShowGlyphs (
#else
#if !NET
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the 'ShowGlyphs' overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the 'ShowGlyphs' overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use the 'ShowGlyphs' overload that takes 'nint glyphCount' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the 'ShowGlyphs' overload that takes 'nint glyphCount' instead.")]
#else
		[UnsupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("macos10.15")]
#if IOS
		[Obsolete ("Starting with ios13.0 use the 'ShowGlyphs' overload that takes 'nint glyphCount' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use the 'ShowGlyphs' overload that takes 'nint glyphCount' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
		[Obsolete ("Starting with macos10.15 use the 'ShowGlyphs' overload that takes 'nint glyphCount' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif // !NET
		public unsafe void ShowCGGlyphs (
#endif // MONOMAC
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
					ShowGlyphs ((IntPtr) gl, (IntPtr) pos, glyphCount, font, textMatrix, attributes, graphicsContext);
				}
			}
		}
#endif // !XAMCORE_4_0

#if !NET
		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
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

#if !XAMCORE_4_0 && !MONOMAC
		// TextContainerForGlyphAtIndex
#if !NET
		[Obsolete ("Use 'GetTextContainer' instead.")]
#else
		[Obsolete ("Use 'GetTextContainer' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public NSTextContainer TextContainerForGlyphAtIndex (nuint glyphIndex)
		{
			return GetTextContainer (glyphIndex);
		}
		
#if !NET
		[Obsolete ("Use 'GetTextContainer' instead.")]
#else
		[Obsolete ("Use 'GetTextContainer' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public NSTextContainer TextContainerForGlyphAtIndex (nuint glyphIndex, ref NSRange effectiveGlyphRange)
		{
			return GetTextContainer (glyphIndex, out effectiveGlyphRange);
		}

		// LineFragmentRectForGlyphAtIndex
#if !NET
		[Obsolete ("Use 'GetLineFragmentRect' instead.")]
#else
		[Obsolete ("Use 'GetLineFragmentRect' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public CGRect LineFragmentRectForGlyphAtIndex (nuint glyphIndex)
		{
			return GetLineFragmentRect (glyphIndex);
		}

#if !NET
		[Obsolete ("Use 'GetLineFragmentRect' instead.")]
#else
		[Obsolete ("Use 'GetLineFragmentRect' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public CGRect LineFragmentRectForGlyphAtIndex (nuint glyphIndex, ref NSRange effectiveGlyphRange)
		{
			return GetLineFragmentRect (glyphIndex, out effectiveGlyphRange);
		}

		// LineFragmentUsedRectForGlyphAtIndex
#if !NET
		[Obsolete ("Use 'GetLineFragmentUsedRect' instead.")]
#else
		[Obsolete ("Use 'GetLineFragmentUsedRect' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public CGRect LineFragmentUsedRectForGlyphAtIndex (nuint glyphIndex)
		{
			return GetLineFragmentUsedRect (glyphIndex);
		}

#if !NET
		[Obsolete ("Use 'GetLineFragmentUsedRect' instead.")]
#else
		[Obsolete ("Use 'GetLineFragmentUsedRect' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public CGRect LineFragmentUsedRectForGlyphAtIndex (nuint glyphIndex, ref NSRange effectiveGlyphRange)
		{
			return GetLineFragmentUsedRect (glyphIndex, out effectiveGlyphRange);
		}

		// GlyphRangeForCharacterRange
#if !NET
		[Obsolete ("Use 'GetGlyphRange' instead.")]
#else
		[Obsolete ("Use 'GetGlyphRange' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public NSRange GlyphRangeForCharacterRange (NSRange charRange)
		{
			return GetGlyphRange (charRange);
		}

#if !NET
		[Obsolete ("Use 'GetGlyphRange' instead.")]
#else
		[Obsolete ("Use 'GetGlyphRange' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public NSRange GlyphRangeForCharacterRange (NSRange charRange, ref NSRange actualCharRange)
		{
			return GetGlyphRange (charRange, out actualCharRange);
		}
		
		// CharacterRangeForGlyphRange
#if !NET
		[Obsolete ("Use 'GetCharacterRange' instead.")]
#else
		[Obsolete ("Use 'GetCharacterRange' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public NSRange CharacterRangeForGlyphRange (NSRange charRange)
		{
			return GetCharacterRange (charRange);
		}

		public NSRange CharacterRangeForGlyphRange (NSRange charRange, ref NSRange actualCharRange)
		{
			return GetCharacterRange (charRange, out actualCharRange);
		}
#endif // !XAMCORE_4_0 && !MONOMAC

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
