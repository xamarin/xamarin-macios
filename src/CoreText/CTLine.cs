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
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

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
		ExcludeTypographicLeading  = 1 << 0,
		ExcludeTypographicShifts   = 1 << 1,
		UseHangingPunctuation      = 1 << 2,
		UseGlyphPathBounds         = 1 << 3,
		UseOpticalBounds           = 1 << 4,
		IncludeLanguageExtents     = 1 << 5, // iOS8 and Mac 10.11
    }
	
	public class CTLine : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CTLine (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				throw ConstructorError.ArgumentNull (this, "handle");

			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}
		
		public IntPtr Handle {
			get { return handle; }
		}

		~CTLine ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

#region Line Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineCreateWithAttributedString (IntPtr @string);
		public CTLine (NSAttributedString value)
		{
			if (value == null)
				throw ConstructorError.ArgumentNull (this, "value");

			handle = CTLineCreateWithAttributedString (value.Handle);

			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineCreateTruncatedLine (IntPtr line, double width, CTLineTruncation truncationType, IntPtr truncationToken);
		public CTLine GetTruncatedLine (double width, CTLineTruncation truncationType, CTLine truncationToken)
		{
			var h = CTLineCreateTruncatedLine (handle, width, truncationType,
					truncationToken == null ? IntPtr.Zero : truncationToken.Handle);
			if (h == IntPtr.Zero)
				return null;
			return new CTLine (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineCreateJustifiedLine (IntPtr line, nfloat justificationFactor, double justificationWidth);
		public CTLine GetJustifiedLine (nfloat justificationFactor, double justificationWidth)
		{
			var h = CTLineCreateJustifiedLine (handle, justificationFactor, justificationWidth);
			if (h == IntPtr.Zero)
				return null;
			return new CTLine (h, true);
		}
#endregion

#region Line Access
		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTLineGetGlyphCount (IntPtr line);
		public nint GlyphCount {
			get {return CTLineGetGlyphCount (handle);}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTLineGetGlyphRuns (IntPtr line);
		public CTRun[] GetGlyphRuns ()
		{
			var cfArrayRef = CTLineGetGlyphRuns (handle);
			if (cfArrayRef == IntPtr.Zero)
				return new CTRun [0];

			return NSArray.ArrayFromHandle (cfArrayRef, 
					v => new CTRun (v, false));
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern NSRange CTLineGetStringRange (IntPtr line);
		public NSRange StringRange {
			get {return CTLineGetStringRange (handle);}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTLineGetPenOffsetForFlush (IntPtr line, nfloat flushFactor, double flushWidth);
		public double GetPenOffsetForFlush (nfloat flushFactor, double flushWidth)
		{
			return CTLineGetPenOffsetForFlush (handle, flushFactor, flushWidth);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern void CTLineDraw (IntPtr line, IntPtr context);
		public void Draw (CGContext context)
		{
			if (context == null)
				throw new ArgumentNullException ("context");
			CTLineDraw (handle, context.Handle);
		}
#endregion

#region Line Measurement
		[DllImport (Constants.CoreTextLibrary)]
		static extern CGRect CTLineGetImageBounds (/* CTLineRef __nonnull */ IntPtr line,
			/* CGContextRef __nullable */ IntPtr context);

		public CGRect GetImageBounds (CGContext context)
		{
			return CTLineGetImageBounds (handle, context.GetHandle ());
		}

		[iOS (6,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern CGRect CTLineGetBoundsWithOptions (IntPtr line, nuint options);

		[iOS (6,0)]
		public CGRect GetBounds (CTLineBoundsOptions options)
		{
			return CTLineGetBoundsWithOptions (handle, (nuint) (ulong) options);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTLineGetTypographicBounds (IntPtr line, out nfloat ascent, out nfloat descent, out nfloat leading);
		public double GetTypographicBounds (out nfloat ascent, out nfloat descent, out nfloat leading)
		{
			return CTLineGetTypographicBounds (handle, out ascent, out descent, out leading);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTLineGetTypographicBounds (IntPtr line, IntPtr ascent, IntPtr descent, IntPtr leading);
		public double GetTypographicBounds ()
		{
			return CTLineGetTypographicBounds (handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTLineGetTrailingWhitespaceWidth (IntPtr line);
		public double TrailingWhitespaceWidth {
			get {return CTLineGetTrailingWhitespaceWidth (handle);}
		}
#endregion

#region Line Caret Positioning and Highlighting
		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTLineGetStringIndexForPosition (IntPtr line, CGPoint position);
		public nint GetStringIndexForPosition (CGPoint position)
		{
			return CTLineGetStringIndexForPosition (handle, position);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTLineGetOffsetForStringIndex (IntPtr line, nint charIndex, out nfloat secondaryOffset);
		public nfloat GetOffsetForStringIndex (nint charIndex, out nfloat secondaryOffset)
		{
			return CTLineGetOffsetForStringIndex (handle, charIndex, out secondaryOffset);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTLineGetOffsetForStringIndex (IntPtr line, nint charIndex, IntPtr secondaryOffset);
		public nfloat GetOffsetForStringIndex (nint charIndex)
		{
			return CTLineGetOffsetForStringIndex (handle, charIndex, IntPtr.Zero);
		}

		public delegate void CaretEdgeEnumerator (double offset, nint charIndex, bool leadingEdge, ref bool stop);
		unsafe delegate void CaretEdgeEnumeratorProxy (IntPtr block, double offset, nint charIndex, [MarshalAs (UnmanagedType.I1)] bool leadingEdge, [MarshalAs (UnmanagedType.I1)] ref bool stop);
		
		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreTextLibrary)]
		static unsafe extern void CTLineEnumerateCaretOffsets (IntPtr line, BlockLiteral *blockEnumerator);

		static unsafe readonly CaretEdgeEnumeratorProxy static_enumerate = TrampolineEnumerate;

		[MonoPInvokeCallback (typeof (CaretEdgeEnumeratorProxy))]
		static unsafe void TrampolineEnumerate (IntPtr blockPtr, double offset, nint charIndex, bool leadingEdge, ref bool stop)
		{
			var block = (BlockLiteral *) blockPtr;
                        var del = (CaretEdgeEnumerator) (block->Target);
                        if (del != null)
                                del (offset, charIndex, leadingEdge, ref stop);
 		}
		
		[iOS (9,0)][Mac (10,11)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void EnumerateCaretOffsets (CaretEdgeEnumerator enumerator)
		{
			if (enumerator == null)
				throw new ArgumentNullException ("enumerator");
                        unsafe {
                                BlockLiteral *block_ptr_handler;
                                BlockLiteral block_handler;
                                block_handler = new BlockLiteral ();
                                block_ptr_handler = &block_handler;
                                block_handler.SetupBlockUnsafe (static_enumerate, enumerator);

                                CTLineEnumerateCaretOffsets (handle, block_ptr_handler);
                                block_ptr_handler->CleanupBlock ();
                        }
		}
#endregion
	}
}

