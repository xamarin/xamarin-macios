// 
// CTRun.cs: Implements the managed CTFrame
//
// Authors: Mono Team
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2010 Novell, Inc
// Copyright 2014 Xamarin Inc (http://www.xamarin.com)
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

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreText {

	// defined as uint32_t - System/Library/Frameworks/CoreText.framework/Headers/CTRun.h
	public enum CTRunStatus {
		NoStatus = 0,
		RightToLeft = (1 << 0),
		NonMonotonic = (1 << 1),
		HasNonIdentityMatrix = (1 << 2)
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTRun : NativeObject {
		[Preserve (Conditional = true)]
		internal CTRun (NativeHandle handle, bool owns)
			: base (handle, owns, true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static void CTRunDraw (IntPtr h, IntPtr context, NSRange range);
		public void Draw (CGContext context, NSRange range)
		{
			CTRunDraw (Handle, context.Handle, range);
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static void CTRunGetAdvances (IntPtr h, NSRange range, [In, Out] CGSize []? buffer);
		public CGSize [] GetAdvances (NSRange range, CGSize []? buffer)
		{
			buffer = GetBuffer (range, buffer);

			CTRunGetAdvances (Handle, range, buffer);

			return buffer;
		}

		T [] GetBuffer<T> (NSRange range, T []? buffer)
		{
			var glyphCount = GlyphCount;

			if (buffer is not null && range.Length != 0 && buffer.Length < range.Length)
				throw new ArgumentException ("buffer.Length must be >= range.Length.", nameof (buffer));
			if (buffer is not null && range.Length == 0 && buffer.Length < glyphCount)
				throw new ArgumentException ("buffer.Length must be >= GlyphCount.", nameof (buffer));

			return buffer ?? new T [range.Length == 0 ? glyphCount : range.Length];
		}

		public CGSize [] GetAdvances (NSRange range)
		{
			return GetAdvances (range, null);
		}

		public CGSize [] GetAdvances ()
		{
			return GetAdvances (new NSRange (0, 0), null);
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static IntPtr CTRunGetAttributes (IntPtr handle);

		public CTStringAttributes? GetAttributes ()
		{
			var d = Runtime.GetNSObject<NSDictionary> (CTRunGetAttributes (Handle));
			return d is null ? null : new CTStringAttributes (d);
		}


		[DllImport (Constants.CoreTextLibrary)]
		extern static nint CTRunGetGlyphCount (IntPtr handle);
		public nint GlyphCount {
			get {
				return CTRunGetGlyphCount (Handle);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static void CTRunGetGlyphs (IntPtr h, NSRange range, [In, Out] ushort []? buffer);
		public ushort [] GetGlyphs (NSRange range, ushort []? buffer)
		{
			buffer = GetBuffer (range, buffer);

			CTRunGetGlyphs (Handle, range, buffer);

			return buffer;
		}

		public ushort [] GetGlyphs (NSRange range)
		{
			return GetGlyphs (range, null);
		}

		public ushort [] GetGlyphs ()
		{
			return GetGlyphs (new NSRange (0, 0), null);
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static CGRect CTRunGetImageBounds (IntPtr h, IntPtr context, NSRange range);
		public CGRect GetImageBounds (CGContext context, NSRange range)
		{
			return CTRunGetImageBounds (Handle, context.Handle, range);
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static void CTRunGetPositions (IntPtr h, NSRange range, [In, Out] CGPoint []? buffer);
		public CGPoint [] GetPositions (NSRange range, CGPoint []? buffer)
		{
			buffer = GetBuffer (range, buffer);

			CTRunGetPositions (Handle, range, buffer);

			return buffer;
		}

		public CGPoint [] GetPositions (NSRange range)
		{
			return GetPositions (range, null);
		}

		public CGPoint [] GetPositions ()
		{
			return GetPositions (new NSRange (0, 0), null);
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static CTRunStatus CTRunGetStatus (IntPtr handle);
		public CTRunStatus Status {
			get {
				return CTRunGetStatus (Handle);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static void CTRunGetStringIndices (IntPtr h, NSRange range, [In, Out] nint /* CFIndex */ []? buffer);
		public nint [] GetStringIndices (NSRange range, nint []? buffer)
		{
			buffer = GetBuffer (range, buffer);

			CTRunGetStringIndices (Handle, range, buffer);

			return buffer;
		}

		public nint [] GetStringIndices (NSRange range)
		{
			return GetStringIndices (range, null);
		}

		public nint [] GetStringIndices ()
		{
			return GetStringIndices (new NSRange (0, 0), null);
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static NSRange CTRunGetStringRange (IntPtr handle);
		public NSRange StringRange {
			get {
				return CTRunGetStringRange (Handle);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static CGAffineTransform CTRunGetTextMatrix (IntPtr handle);
		public CGAffineTransform TextMatrix {
			get {
				return CTRunGetTextMatrix (Handle);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static double CTRunGetTypographicBounds (IntPtr h, NSRange range, out nfloat ascent, out nfloat descent, out nfloat leading);

		[DllImport (Constants.CoreTextLibrary)]
		extern static double CTRunGetTypographicBounds (IntPtr h, NSRange range, IntPtr ascent, IntPtr descent, IntPtr leading);
		public double GetTypographicBounds (NSRange range, out nfloat ascent, out nfloat descent, out nfloat leading)
		{
			return CTRunGetTypographicBounds (Handle, range, out ascent, out descent, out leading);
		}

		public double GetTypographicBounds ()
		{
			NSRange range = new NSRange () { Location = 0, Length = 0 };
			return CTRunGetTypographicBounds (Handle, range, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6, 0)]
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern void CTRunGetBaseAdvancesAndOrigins (/* CTRunRef */ IntPtr runRef, /* CFRange */ NSRange range, CGSize [] advancesBuffer, CGPoint [] originsBuffer);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6, 0)]
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		public void GetBaseAdvancesAndOrigins (NSRange range, out CGSize [] advancesBuffer, out CGPoint [] originsBuffer)
		{
			advancesBuffer = GetBuffer<CGSize> (range, null);
			originsBuffer = GetBuffer<CGPoint> (range, null);
			CTRunGetBaseAdvancesAndOrigins (Handle, range, advancesBuffer, originsBuffer);
		}
	}
}
