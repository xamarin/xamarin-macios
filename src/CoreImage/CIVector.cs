//
// CIVector.cs: Extra methods for CIVector
//
// Copyright 2010, Novell, Inc.
// Copyright 2011, 2012 Xamarin Inc
//
// Author:
//   Miguel de Icaza
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

namespace XamCore.CoreImage {
	public partial class CIVector {
		nfloat this [nint index] {
			get {
				return ValueAtIndex (index);
			}
		}
		
		static IntPtr GetPtr (nfloat [] values)
		{
			if (values == null)
				throw new ArgumentNullException ("values");
			unsafe {
				fixed (nfloat *ptr = values)
					return (IntPtr) ptr;
			}
		}
		
		public CIVector (nfloat [] values) : this (GetPtr (values), values.Length)
		{
		}
	
		public static CIVector FromValues (nfloat [] values)
		{
			return _FromValues (GetPtr (values), values.Length);
		}
		
		public override string ToString ()
		{
			return StringRepresentation ();
		}
	}
}
