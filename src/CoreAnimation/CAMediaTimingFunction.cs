//
// Extra methods for CAMediaTimingFunction
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc
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
using CoreGraphics;

namespace CoreAnimation {
	public unsafe partial class CAMediaTimingFunction {

#if !XAMCORE_2_0
#if !MONOMAC
		[Obsolete ("This type is not meant to be created by application code")]
		public CAMediaTimingFunction () : base (IntPtr.Zero)
		{
			IsDirectBinding = GetType () == typeof (CAMediaTimingFunction);
		}
#endif

		[Advice ("Use 'FromName(NSString)' with one of the CAMediaTimingFunction fields.")]
		static public CAMediaTimingFunction FromName (string name)
		{
			using (NSString s = new NSString (name))
				return FromName (s);
		}
#endif

		public CGPoint GetControlPoint (nint index)
		{
			if ((index < 0) || (index > 3))
				throw new ArgumentOutOfRangeException ("index");

			float [] values = new float [2];
			fixed (float *p = &values [0])
				GetControlPointAtIndex (index, (IntPtr) p /* float[2] */);
			return new CGPoint (values [0], values [1]);
		}
	}
}
