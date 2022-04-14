#if !__MACCATALYST__
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

#if NET
		[SupportedOSPlatform ("macos10.13")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,13)]
#endif
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

#if NET
		[SupportedOSPlatform ("macos10.13")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,13)]
#endif
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

		public static NSFont FromFontName (string fontName, nfloat fontSize)
		{
			var ptr = _FromFontName (fontName, fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont FromDescription (NSFontDescriptor fontDescriptor, nfloat fontSize)
		{
			var ptr = _FromDescription (fontDescriptor, fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont FromDescription (NSFontDescriptor fontDescriptor, NSAffineTransform textTransform)
		{
			var ptr = _FromDescription (fontDescriptor, textTransform);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont UserFontOfSize (nfloat fontSize)
		{
			var ptr = _UserFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont UserFixedPitchFontOfSize (nfloat fontSize)
		{
			var ptr = _UserFixedPitchFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont SystemFontOfSize (nfloat fontSize)
		{
			var ptr = _SystemFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont BoldSystemFontOfSize (nfloat fontSize)
		{
			var ptr = _BoldSystemFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont LabelFontOfSize (nfloat fontSize)
		{
			var ptr = _LabelFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont TitleBarFontOfSize (nfloat fontSize)
		{
			var ptr = _TitleBarFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont MenuFontOfSize (nfloat fontSize)
		{
			var ptr = _MenuFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont MenuBarFontOfSize (nfloat fontSize)
		{
			var ptr = _MenuBarFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont MessageFontOfSize (nfloat fontSize)
		{
			var ptr = _MessageFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont PaletteFontOfSize (nfloat fontSize)
		{
			var ptr = _PaletteFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont ToolTipsFontOfSize (nfloat fontSize)
		{
			var ptr = _ToolTipsFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public static NSFont ControlContentFontOfSize (nfloat fontSize)
		{
			var ptr = _ControlContentFontOfSize (fontSize);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("macos10.13")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 13)]
#endif
		public virtual NSFont PrinterFont { 
			get {
				var ptr = _PrinterFont;
				return ptr == IntPtr.Zero ? null : new NSFont (ptr);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("macos10.13")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 13)]
#endif
		public virtual NSFont ScreenFont {
			get {
				var ptr = _ScreenFont;
				return ptr == IntPtr.Zero ? null : new NSFont (ptr);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("macos10.13")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 13)]
#endif
		public virtual NSFont ScreenFontWithRenderingMode (NSFontRenderingMode renderingMode)
		{
			var ptr = _ScreenFontWithRenderingMode (renderingMode);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
 
		public virtual NSFont GetVerticalFont ()
		{
			var ptr = _GetVerticalFont ();
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}

#if NET
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,11)]
#endif
		public static NSFont SystemFontOfSize (nfloat fontSize, nfloat weight)
		{
			var ptr = _SystemFontOfSize (fontSize, weight);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}

#if NET
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,11)]
#endif
		public static NSFont MonospacedDigitSystemFontOfSize (nfloat fontSize, nfloat weight)
		{
			var ptr = _MonospacedDigitSystemFontOfSize (fontSize, weight);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}

#if NET
		[SupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,15)]
#endif
		public static NSFont MonospacedSystemFont (nfloat fontSize, nfloat weight)
		{
			var ptr = _MonospacedSystemFont (fontSize, weight);
			return ptr == IntPtr.Zero ? null : new NSFont (ptr);
		}
	}
}
#endif // !__MACCATALYST__
