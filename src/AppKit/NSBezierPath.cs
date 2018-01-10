//
// NSBezierPath.cs: Helper methods to expose strongly typed functions
//
// Author:
//   Regan Sarwas <rsarwas@gmail.com>
//
// Copyright 2010, Regan Sarwas.
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

using Foundation;
using ObjCRuntime;
using CoreGraphics;

namespace AppKit {
	public partial class NSBezierPath {

		public unsafe void GetLineDash (out nfloat [] pattern, out nfloat phase)
		{
			nint length;

			//Call the internal method with null to get the length of the pattern array
			_GetLineDash (IntPtr.Zero, out length, out phase);

			pattern = new nfloat [length];
			fixed (nfloat* ptr = &pattern [0])
				_GetLineDash ((IntPtr) ptr, out length, out phase);
		}

		public unsafe void SetLineDash (nfloat [] pattern, nfloat phase)
		{
			if (pattern == null)
				throw new ArgumentNullException ("pattern");

			fixed (nfloat* ptr = &pattern [0])
				_SetLineDash ((IntPtr) ptr, pattern.Length, phase);
		}

		public unsafe NSBezierPathElement ElementAt (nint index, out CGPoint[] points)
		{
			NSBezierPathElement bpe;

			// Return array will be 1 or 3 points, depending on type.
			// Create output with 3 elements at first
			points = new CGPoint [3];

			fixed (CGPoint* ptr = &points [0])
				bpe = _ElementAt (index, (IntPtr) ptr);

			if (bpe != NSBezierPathElement.CurveTo) {
				// we only got one element, downsize array
				Array.Resize (ref points, 1);
			}

			return bpe;
		}

		public unsafe void SetAssociatedPointsAtIndex (CGPoint[] points, nint index)
		{
		    if (points == null)
		        throw new ArgumentNullException ("points");

			if (points.Length < 1)
				throw new ArgumentException ("points array is empty");

			fixed (CGPoint* ptr = &points [0])
				_SetAssociatedPointsAtIndex ((IntPtr)ptr, index);
		}

		public unsafe void Append (CGPoint[] points)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			if (points.Length < 1)
				throw new ArgumentException ("points array is empty");

			fixed (CGPoint* ptr = &points [0])
				_AppendPathWithPoints ((IntPtr)ptr, points.Length);
		}

#if !XAMCORE_4_0
		[Obsolete ("Use 'Append (CGPoint[])' instead.")]
		public unsafe void AppendPathWithPoints (CGPoint[] points)
		{
			Append (points);
		}

		[Obsolete ("Use 'Append (uint[], NSFont)' instead.")]
		public unsafe void AppendPathWithGlyphs (uint[] glyphs, NSFont font)
		{
			if (glyphs == null)
				throw new ArgumentNullException ("glyphs");
			if (glyphs.Length < 1)
				throw new ArgumentException ("glyphs array is empty");

			fixed (uint* ptr = &glyphs [0])
				_AppendPathWithGlyphs ((IntPtr)ptr, glyphs.Length, font);
		}
#endif

		[Mac (10,13)]
		public unsafe void Append (uint[] glyphs, NSFont font)
		{
			if (glyphs == null)
				throw new ArgumentNullException ("glyphs");
			if (glyphs.Length < 1)
				throw new ArgumentException ("glyphs array is empty");

			fixed (uint* ptr = &glyphs [0])
				_AppendBezierPathWithCGGlyphs ((IntPtr)ptr, glyphs.Length, font);
		}
	}
}
