// 
// CTLine.cs: Implements the managed CTLine
//
// Authors: Mono Team
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2010 Novell, Inc
// Copyright 2011 - 2014 Xamarin Inc
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

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTLine.h
	public enum CTLineTruncation : uint {
		Start = 0,
		End = 1,
		Middle = 2
	}

	// defined as CFOptionFlags (unsigned long [long] = nuint) - /System/Library/Frameworks/CoreText.framework/Headers/CTLine.h
	[Native]
	[Flags]
	public enum CTLineBoundsOptions : ulong {
		ExcludeTypographicLeading = 1 << 0,
		ExcludeTypographicShifts = 1 << 1,
		UseHangingPunctuation = 1 << 2,
		UseGlyphPathBounds = 1 << 3,
		UseOpticalBounds = 1 << 4,
		IncludeLanguageExtents = 1 << 5, // iOS8 and Mac 10.11
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTLine : NativeObject {
		[Preserve (Conditional = true)]
		internal CTLine (NativeHandle handle, bool owns)
			: base (handle, owns, true)
		{
		}

		#region Line Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineCreateWithAttributedString (IntPtr @string);
		public CTLine (NSAttributedString value)
			: base (CTLineCreateWithAttributedString (Runtime.ThrowOnNull (value, nameof (value)).Handle), true, true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineCreateTruncatedLine (IntPtr line, double width, CTLineTruncation truncationType, IntPtr truncationToken);
		public CTLine? GetTruncatedLine (double width, CTLineTruncation truncationType, CTLine? truncationToken)
		{
			var h = CTLineCreateTruncatedLine (Handle, width, truncationType, truncationToken.GetHandle ());
			if (h == IntPtr.Zero)
				return null;
			return new CTLine (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineCreateJustifiedLine (IntPtr line, nfloat justificationFactor, double justificationWidth);
		public CTLine? GetJustifiedLine (nfloat justificationFactor, double justificationWidth)
		{
			var h = CTLineCreateJustifiedLine (Handle, justificationFactor, justificationWidth);
			if (h == IntPtr.Zero)
				return null;
			return new CTLine (h, true);
		}
		#endregion

		#region Line Access
		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTLineGetGlyphCount (IntPtr line);
		public nint GlyphCount {
			get { return CTLineGetGlyphCount (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineGetGlyphRuns (IntPtr line);
		public CTRun [] GetGlyphRuns ()
		{
			var cfArrayRef = CTLineGetGlyphRuns (Handle);
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTRun> ();

			return NSArray.ArrayFromHandle (cfArrayRef,
					v => new CTRun (v, false))!;
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern NSRange CTLineGetStringRange (IntPtr line);
		public NSRange StringRange {
			get { return CTLineGetStringRange (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTLineGetPenOffsetForFlush (IntPtr line, nfloat flushFactor, double flushWidth);
		public double GetPenOffsetForFlush (nfloat flushFactor, double flushWidth)
		{
			return CTLineGetPenOffsetForFlush (Handle, flushFactor, flushWidth);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern void CTLineDraw (IntPtr line, IntPtr context);
		public void Draw (CGContext context)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			CTLineDraw (Handle, context.Handle);
		}
		#endregion

		#region Line Measurement
		[DllImport (Constants.CoreTextLibrary)]
		static extern CGRect CTLineGetImageBounds (/* CTLineRef __nonnull */ IntPtr line,
			/* CGContextRef __nullable */ IntPtr context);

		public CGRect GetImageBounds (CGContext? context)
		{
			return CTLineGetImageBounds (Handle, context.GetHandle ());
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CGRect CTLineGetBoundsWithOptions (IntPtr line, nuint options);

		public CGRect GetBounds (CTLineBoundsOptions options)
		{
			return CTLineGetBoundsWithOptions (Handle, (nuint) (ulong) options);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTLineGetTypographicBounds (IntPtr line, out nfloat ascent, out nfloat descent, out nfloat leading);
		public double GetTypographicBounds (out nfloat ascent, out nfloat descent, out nfloat leading)
		{
			return CTLineGetTypographicBounds (Handle, out ascent, out descent, out leading);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTLineGetTypographicBounds (IntPtr line, IntPtr ascent, IntPtr descent, IntPtr leading);
		public double GetTypographicBounds ()
		{
			return CTLineGetTypographicBounds (Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTLineGetTrailingWhitespaceWidth (IntPtr line);
		public double TrailingWhitespaceWidth {
			get { return CTLineGetTrailingWhitespaceWidth (Handle); }
		}
		#endregion

		#region Line Caret Positioning and Highlighting
		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTLineGetStringIndexForPosition (IntPtr line, CGPoint position);
		public nint GetStringIndexForPosition (CGPoint position)
		{
			return CTLineGetStringIndexForPosition (Handle, position);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTLineGetOffsetForStringIndex (IntPtr line, nint charIndex, out nfloat secondaryOffset);
		public nfloat GetOffsetForStringIndex (nint charIndex, out nfloat secondaryOffset)
		{
			return CTLineGetOffsetForStringIndex (Handle, charIndex, out secondaryOffset);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTLineGetOffsetForStringIndex (IntPtr line, nint charIndex, IntPtr secondaryOffset);
		public nfloat GetOffsetForStringIndex (nint charIndex)
		{
			return CTLineGetOffsetForStringIndex (Handle, charIndex, IntPtr.Zero);
		}

		public delegate void CaretEdgeEnumerator (double offset, nint charIndex, bool leadingEdge, ref bool stop);
#if !NET
		unsafe delegate void CaretEdgeEnumeratorProxy (IntPtr block, double offset, nint charIndex, byte leadingEdge, byte* stop);
#endif

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern void CTLineEnumerateCaretOffsets (IntPtr line, BlockLiteral* blockEnumerator);

#if !NET
		static unsafe readonly CaretEdgeEnumeratorProxy static_enumerate = TrampolineEnumerate;

		[MonoPInvokeCallback (typeof (CaretEdgeEnumeratorProxy))]
#else
		[UnmanagedCallersOnly]
#endif
		unsafe static void TrampolineEnumerate (IntPtr blockPtr, double offset, nint charIndex, byte leadingEdge, byte* stopPointer)
		{
			var del = BlockLiteral.GetTarget<CaretEdgeEnumerator> (blockPtr);
			if (del is not null) {
				bool stop = *stopPointer != 0;
				del (offset, charIndex, leadingEdge != 0, ref stop);
				*stopPointer = stop ? (byte) 1 : (byte) 0;
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateCaretOffsets (CaretEdgeEnumerator enumerator)
		{
			if (enumerator is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (enumerator));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, double, nint, byte, byte*, void> trampoline = &TrampolineEnumerate;
				using var block = new BlockLiteral (trampoline, enumerator, typeof (CTLine), nameof (TrampolineEnumerate));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_enumerate, enumerator);
#endif
				CTLineEnumerateCaretOffsets (Handle, &block);
			}
		}
		#endregion
	}
}

