// 
// CGFont.cs: Implements the managed CGFont
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2014 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;
using CoreFoundation;

using CoreText;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// CGFont.h
	public class CGFont : NativeObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal CGFont (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		static CGFont Create (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handle));
			return new CGFont (handle, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGFontRef */ IntPtr CGFontRetain (/* CGFontRef */ IntPtr font);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGFontRelease (/* CGFontRef */ IntPtr font);

		protected internal override void Retain ()
		{
			CGFontRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGFontRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGFontRef */ IntPtr CGFontCreateWithDataProvider (/* CGDataProviderRef __nullable */ IntPtr provider);

		public static CGFont? CreateFromProvider (CGDataProvider provider)
		{
			// the API accept a `nil` argument but returns `nil`, we take a shortcut (no native call)
			// and have a unit tests to make sure this behavior does not change over time
			if (provider is null)
				return null;
			return Create (CGFontCreateWithDataProvider (provider.Handle));
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGFontRef */ IntPtr CGFontCreateWithFontName (/* CFStringRef __nullable */ IntPtr name);

		public static CGFont? CreateWithFontName (string name)
		{
			// the API accept a `nil` argument but returns `nil`, we take a shortcut (no native call)
			// and have a unit tests to make sure this behavior does not change over time
			if (name is null)
				return null;
			var nameHandle = CFString.CreateNative (name);
			try {
				return Create (CGFontCreateWithFontName (nameHandle));
			} finally {
				CFString.ReleaseNative (nameHandle);
			}
		}

		//[DllImport (Constants.CoreGraphicsLibrary)]
		//extern static IntPtr CGFontCreateCopyWithVariations(IntPtr font, IntPtr cf_dictionaryref_variations);
		//public static CGFont CreateCopyWithVariations ()
		//{
		//	throw new NotImplementedException ();
		//}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGFontGetNumberOfGlyphs (/* CGFontRef */ IntPtr font);

		public nint NumberOfGlyphs {
			get {
				return CGFontGetNumberOfGlyphs (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* int */ int CGFontGetUnitsPerEm (/* CGFontRef */ IntPtr font);

		public int UnitsPerEm {
			get {
				return CGFontGetUnitsPerEm (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGFontCopyPostScriptName (/* CGFontRef __nullable */ IntPtr font);

		public string? PostScriptName {
			get {
				return CFString.FromHandle (CGFontCopyPostScriptName (Handle), releaseHandle: true);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGFontCopyFullName (/* CGFontRef __nullable */ IntPtr font);

		public string? FullName {
			get {
				return CFString.FromHandle (CGFontCopyFullName (Handle), releaseHandle: true);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* int */ int CGFontGetAscent (/* CGFontRef */ IntPtr font);

		public int Ascent {
			get {
				return CGFontGetAscent (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* int */ int CGFontGetDescent (/* CGFontRef */ IntPtr font);

		public int Descent {
			get {
				return CGFontGetDescent (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* int */ int CGFontGetLeading (/* CGFontRef */ IntPtr font);

		public int Leading {
			get {
				return CGFontGetLeading (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* int */ int CGFontGetCapHeight (/* CGFontRef */ IntPtr font);

		public int CapHeight {
			get {
				return CGFontGetCapHeight (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* int */ int CGFontGetXHeight (/* CGFontRef */ IntPtr font);

		public int XHeight {
			get {
				return CGFontGetXHeight (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGFontGetFontBBox (/* CGFontRef */ IntPtr font);

		public CGRect FontBBox {
			get {
				return CGFontGetFontBBox (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGFloat */ nfloat CGFontGetItalicAngle (/* CGFontRef */ IntPtr font);

		public nfloat ItalicAngle {
			get {
				return CGFontGetItalicAngle (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGFloat */ nfloat CGFontGetStemV (/* CGFontRef */ IntPtr font);

		public nfloat StemV {
			get {
				return CGFontGetStemV (Handle);
			}
		}

		//[DllImport (Constants.CoreGraphicsLibrary)]
		//extern static CFArrayRef CGFontCopyVariationAxes(IntPtr font);

		//[DllImport (Constants.CoreGraphicsLibrary)]
		//extern static CFDictionaryRef CGFontCopyVariations(IntPtr font);
		//[DllImport (Constants.CoreGraphicsLibrary)]
		//extern static bool CGFontGetGlyphAdvances(IntPtr font, ushort [] glyphs, int size_t_count, int [] advances);

		//[DllImport (Constants.CoreGraphicsLibrary)]
		//extern static bool CGFontGetGlyphBBoxes(IntPtr font, ushort [] glyphs, int size_t_count, CGRect [] bboxes);

		// CGGlyph -> CGFontIndex -> unsigned short -> CGFont.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGGlyph */ ushort CGFontGetGlyphWithGlyphName (/* CGFontRef __nullable */ IntPtr font, /* CFStringRef __nullable */ IntPtr name);

		public ushort GetGlyphWithGlyphName (string s)
		{
			// note: the API is marked to accept a null CFStringRef but it currently (iOS9 beta 4) crash when provided one
			if (s is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (s));
			var sHandle = CFString.CreateNative (s);
			try {
				return CGFontGetGlyphWithGlyphName (Handle, sHandle);
			} finally {
				CFString.ReleaseNative (sHandle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGFontCopyGlyphNameForGlyph (/* CGFontRef __nullable */ IntPtr font, /* CGGlyph */ ushort glyph);

		public string? GlyphNameForGlyph (ushort glyph)
		{
			return CFString.FromHandle (CGFontCopyGlyphNameForGlyph (Handle, glyph), releaseHandle: true);
		}

		//[DllImport (Constants.CoreGraphicsLibrary)]
		//extern static bool CGFontCanCreatePostScriptSubset(IntPtr font, CGFontPostScriptFormat format);
		//[DllImport (Constants.CoreGraphicsLibrary)]
		//extern static CFDataRef CGFontCreatePostScriptSubset(IntPtr font, CFStringRef subsetName, CGFontPostScriptFormat format, ushort [] glyphs, size_t count, ushort [256] glyph_encodins);
		//[DllImport (Constants.CoreGraphicsLibrary)]
		//extern static CFDataRef CGFontCreatePostScriptEncoding(IntPtr font, ushort [256] CGGlyph_encoding);
		//[DllImport (Constants.CoreGraphicsLibrary)]
		//extern static /* CFArrayRef */ IntPtr CGFontCopyTableTags(IntPtr font);
		//[DllImport (Constants.CoreGraphicsLibrary)]
		//extern static /* CFDataRef */ IntPtr CGFontCopyTableForTag(IntPtr font, int tag);

		// CFStringRef kCGFontVariationAxisName;
		// CFStringRef kCGFontVariationAxisMinValue;
		// CFStringRef kCGFontVariationAxisMaxValue;
		// CFStringRef kCGFontVariationAxisDefaultValue;

		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern /* CTFontRef */ IntPtr CTFontCreateWithGraphicsFont (/* CGFontRef */ IntPtr graphicsFont, /* CGFloat */ nfloat size, CGAffineTransform* matrix, /* CTFontDescriptorRef */ IntPtr attributes);

		public unsafe CTFont ToCTFont (nfloat size)
		{
			return new CTFont (CTFontCreateWithGraphicsFont (Handle, size, null, IntPtr.Zero), true);
		}

		public unsafe CTFont ToCTFont (nfloat size, CGAffineTransform matrix)
		{
			return new CTFont (CTFontCreateWithGraphicsFont (Handle, size, &matrix, IntPtr.Zero), true);
		}

#if TODO
		ToCTFont() overloads where attributes is CTFontDescriptorRef
#endif // TODO

		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGFontGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();
#endif // !COREBUILD
	}
}
