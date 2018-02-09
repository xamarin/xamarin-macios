// Copyright 2009, Novell, Inc.
// Copyright 2010, Novell, Inc.
// Copyright 2011, 2012, 2014-2015 Xamarin Inc.
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
//
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace AVFoundation {

	[StructLayout (LayoutKind.Sequential)]
	public struct AVPixelAspectRatio {
		public nint /* NSInteger */ HorizontalSpacing;
		public nint /* NSInteger */ VerticalSpacing;

		public AVPixelAspectRatio (nint horizontalSpacing, nint verticalSpacing)
		{
			HorizontalSpacing = horizontalSpacing;
			VerticalSpacing = verticalSpacing;
		}

		public override string ToString ()
		{
			return String.Format ("(horizontalSpacing={0}, verticalSpacing={1})", HorizontalSpacing, VerticalSpacing);
		}

		public static bool operator == (AVPixelAspectRatio left, AVPixelAspectRatio right)
		{
			return left.HorizontalSpacing == right.HorizontalSpacing && left.VerticalSpacing == right.VerticalSpacing;
		}

		public static bool operator != (AVPixelAspectRatio left, AVPixelAspectRatio right)
		{
			return left.HorizontalSpacing != right.HorizontalSpacing || left.VerticalSpacing != right.VerticalSpacing;
		}

		public override int GetHashCode ()
		{
			return (int) HorizontalSpacing ^ (int) VerticalSpacing;
		}

		public override bool Equals (object other)
		{
			if (other is AVPixelAspectRatio){
				var o = (AVPixelAspectRatio) other;
				return o.HorizontalSpacing == HorizontalSpacing && o.VerticalSpacing == VerticalSpacing;
			}
			return false;
		}
	}
}
