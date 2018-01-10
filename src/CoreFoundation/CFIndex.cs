//
// MonoMac.CoreFoundation.CFIndex
//
// Authors:
//      Martin Baulig (martin.baulig@gmail.com)
//
// Copyright 2012 Xamarin Inc. (http://www.xamarin.com)
//
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

namespace CoreFoundation {

#if !XAMCORE_2_0
	// This struct is redundant with Xamcore 2.0, just use nint instead.
	public struct CFIndex {
		IntPtr value;

		private CFIndex (IntPtr value)
		{
			this.value = value;
		}

		public static implicit operator int (CFIndex index)
		{
			return index.value.ToInt32 ();
		}

		public static implicit operator CFIndex (int value)
		{
			return new CFIndex (new IntPtr (value));
		}

		public static implicit operator long (CFIndex index)
		{
			return index.value.ToInt64 ();
		}
	}
#endif
}
