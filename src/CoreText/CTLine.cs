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

using CoreFoundation;
using CoreGraphics;

namespace CoreText {

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTLine.h
	/// <summary>An enumeration whose values specify valid options for line truncation.</summary>
	///     <remarks>To be added.</remarks>
	///     <altmember cref="CoreText.CTLine.GetTruncatedLine" />
	[NativeName ("CTLineTruncationType")]
	public enum CTLineTruncation : uint {
		/// <summary>To be added.</summary>
		Start = 0,
		/// <summary>To be added.</summary>
		End = 1,
		/// <summary>To be added.</summary>
		Middle = 2,
	}

	// defined as CFOptionFlags (unsigned long [long] = nuint) - /System/Library/Frameworks/CoreText.framework/Headers/CTLine.h
	/// <summary>The kind of bounds computation that we want to perform on a CTLine.</summary>
	///     <remarks>
	///       <para>
	/// 	These options can be combined.   In the graphic below, you can see the different bounds that are computed based on this flag.
	///       </para>
	///       <para>
	/// 	The following image shows the effect that the options have on measuring text.
	///       </para>
	///       <para>
	///         <img href="~/xml/CoreText/_images/CoreTextBoundOptions.png" alt="Illustration of the area defined by the various bounds options" />
	///       </para>
	///     </remarks>
	[Native]
	[Flags]
	public enum CTLineBoundsOptions : ulong {
		/// <summary>Use this option to exclude the typographic leading from the bounds computation (the space between baselines of different lines of text).</summary>
		ExcludeTypographicLeading = 1 << 0,
		/// <summary>Does not take into account kerning or leading information when computing bounds for</summary>
		ExcludeTypographicShifts = 1 << 1,
		/// <summary>
		/// 	    Hanging Punctuation is a way of typesetting
		/// 	    punctuation marks and bullet points, most commonly quotation
		/// 	    marks and hyphens, so that they do not disrupt the "flow" of
		/// 	    a body of text or "break" the margin of alignment.  It is so
		/// 	    called because the punctuation appears to ‘hang’ in the
		/// 	    margin of the text, and is not incorporated into the block
		/// 	    or column of text. It is commonly used when text is fully justified.
		/// 	  </summary>
		UseHangingPunctuation = 1 << 2,
		/// <summary>The bounds of every glyph.   These are typographically not very interesting as they do not take into account the finer details of typography, this returns the bounding box for the actual text rendered.</summary>
		UseGlyphPathBounds = 1 << 3,
		/// <summary>
		///
		/// 	  This uses the optical bounds.  Some fonts include
		/// 	  information about the optical perception of the font, and it
		/// 	  might not align perfectly with the bounding box of the text.
		///
		/// 	</summary>
		UseOpticalBounds = 1 << 4,
		/// <summary>To be added.</summary>
		IncludeLanguageExtents = 1 << 5, // iOS8 and Mac 10.11
	}

	/// <summary>A line of text, comprising an array of <see cref="CoreText.CTRun" />s.</summary>
	///     <remarks>To be added.</remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/SimpleTextInput/">SimpleTextInput</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTLine : NativeObject {
		[Preserve (Conditional = true)]
		internal CTLine (NativeHandle handle, bool owns)
			: base (handle, owns, true)
		{
		}

		#region Line Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineCreateWithAttributedString (IntPtr @string);
		/// <param name="value">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTLine (NSAttributedString value)
			: base (CTLineCreateWithAttributedString (value.GetNonNullHandle (nameof (value))), true, true)
		{
			GC.KeepAlive (value);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineCreateTruncatedLine (IntPtr line, double width, CTLineTruncation truncationType, IntPtr truncationToken);
		/// <param name="width">To be added.</param>
		///         <param name="truncationType">To be added.</param>
		///         <param name="truncationToken">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CTLine? GetTruncatedLine (double width, CTLineTruncation truncationType, CTLine? truncationToken)
		{
			var h = CTLineCreateTruncatedLine (Handle, width, truncationType, truncationToken.GetHandle ());
			GC.KeepAlive (truncationToken);
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
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nint GlyphCount {
			get { return CTLineGetGlyphCount (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineGetGlyphRuns (IntPtr line);
		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		/// <param name="context">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void Draw (CGContext context)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			CTLineDraw (Handle, context.Handle);
			GC.KeepAlive (context);
		}
		#endregion

		#region Line Measurement
		[DllImport (Constants.CoreTextLibrary)]
		static extern CGRect CTLineGetImageBounds (/* CTLineRef __nonnull */ IntPtr line,
			/* CGContextRef __nullable */ IntPtr context);

		/// <param name="context">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CGRect GetImageBounds (CGContext? context)
		{
			CGRect bounds = CTLineGetImageBounds (Handle, context.GetHandle ());
			GC.KeepAlive (context);
			return bounds;
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CGRect CTLineGetBoundsWithOptions (IntPtr line, nuint options);

		/// <param name="options">Determines the kind of typographical information to return.</param>
		///         <summary>Returns the bounds of the line as a rectangle, based on the specified <see cref="CoreText.CTLineBoundsOptions" />.</summary>
		///         <returns>The bounding rectangle based on the parameter you pass on the options.</returns>
		///         <remarks>
		///           <para>
		/// 	    This function can return different bounds based on the options passed.    
		/// 	  </para>
		///           <para>
		///             <img href="~/xml/CoreText/_images/CoreTextBoundOptions.png" alt="Illustration of the area defined by the various bounds options" />
		///           </para>
		///         </remarks>
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
		/// <summary>Returns the typorgraphic width of the line.</summary>
		///         <returns>The width of the line, or zero if there are any errors.</returns>
		///         <remarks>
		///           <para>
		/// 	    Use the <see cref="CoreText.CTLine.GetTypographicBounds(out nfloat, out nfloat, out nfloat)" /> method to retrieve more information about the typographical features of the line.
		/// 	  </para>
		///           <para>
		/// 	    Starting with iOS 6.0, the <see cref="CoreText.CTLine.GetBounds(CoreText.CTLineBoundsOptions)" /> provides finer typorgraphical information than this method.
		/// 	  </para>
		///         </remarks>
		public double GetTypographicBounds ()
		{
			return CTLineGetTypographicBounds (Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTLineGetTrailingWhitespaceWidth (IntPtr line);
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public double TrailingWhitespaceWidth {
			get { return CTLineGetTrailingWhitespaceWidth (Handle); }
		}
		#endregion

		#region Line Caret Positioning and Highlighting
		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTLineGetStringIndexForPosition (IntPtr line, CGPoint position);
		/// <param name="position">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

		/// <param name="offset">To be added.</param>
		///     <param name="charIndex">To be added.</param>
		///     <param name="leadingEdge">To be added.</param>
		///     <param name="stop">To be added.</param>
		///     <summary>To be added.</summary>
		///     <remarks>To be added.</remarks>
		public delegate void CaretEdgeEnumerator (double offset, nint charIndex, bool leadingEdge, ref bool stop);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern void CTLineEnumerateCaretOffsets (IntPtr line, BlockLiteral* blockEnumerator);

		[UnmanagedCallersOnly]
		unsafe static void TrampolineEnumerate (IntPtr blockPtr, double offset, nint charIndex, byte leadingEdge, byte* stopPointer)
		{
			var del = BlockLiteral.GetTarget<CaretEdgeEnumerator> (blockPtr);
			if (del is not null) {
				bool stop = *stopPointer != 0;
				del (offset, charIndex, leadingEdge != 0, ref stop);
				*stopPointer = stop ? (byte) 1 : (byte) 0;
			}
		}

		/// <param name="enumerator">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateCaretOffsets (CaretEdgeEnumerator enumerator)
		{
			if (enumerator is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (enumerator));

			unsafe {
				delegate* unmanaged<IntPtr, double, nint, byte, byte*, void> trampoline = &TrampolineEnumerate;
				using var block = new BlockLiteral (trampoline, enumerator, typeof (CTLine), nameof (TrampolineEnumerate));
				CTLineEnumerateCaretOffsets (Handle, &block);
			}
		}
		#endregion
	}
}
