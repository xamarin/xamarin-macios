//
// A few complementary classes for CoreAnimation
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2014 Xamarin Inc.
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

using ObjCRuntime;
using Foundation;
using CoreGraphics;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace CoreAnimation {

	partial class CAAnimation {
		[DllImport (Constants.QuartzLibrary, EntryPoint = "CACurrentMediaTime")]
		public extern static /* CFTimeInterval */ double CurrentMediaTime ();
	}

	public partial class CAGradientLayer {
#if NET
		CGColor CreateColor (NativeHandle p)
#else
		public CGColor CreateColor (IntPtr p)
#endif
		{
			return new CGColor (p, false);
		}

		public CGColor [] Colors {
			get {
				return NSArray.ArrayFromHandle<CGColor> (_Colors, CreateColor);
			}

			set {
				if (value is null) {
					_Colors = IntPtr.Zero;
					return;
				}

				var ptrs = new NativeHandle [value.Length];
				for (int i = 0; i < ptrs.Length; i++)
					ptrs [i] = value [i].Handle;

				using (NSArray array = NSArray.FromIntPtrs (ptrs)) {
					_Colors = array.Handle;
				}
			}
		}
	}

	public partial class CAKeyFrameAnimation {

		// For compatibility, as we told users to explicitly use this method before, or get a warning
		public static CAKeyFrameAnimation GetFromKeyPath (string path)
		{
			return FromKeyPath (path);
		}
	}
}
