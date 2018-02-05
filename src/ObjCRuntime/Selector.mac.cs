//
// Copyright 2010, Novell, Inc.
// Copyright 2013, Xamarin Inc.
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

#if MONOMAC

using System;
using System.Runtime.InteropServices;

namespace ObjCRuntime {
#if !XAMCORE_2_0
	[StructLayout(LayoutKind.Sequential)]
#endif
	public partial class Selector {
#if !XAMCORE_2_0
		public static readonly IntPtr Init = Selector.GetHandle ("init");
		public static readonly IntPtr InitWithCoder = Selector.GetHandle ("initWithCoder:");
#else
		internal static readonly IntPtr Init = Selector.GetHandle ("init");
		internal static readonly IntPtr InitWithCoder = Selector.GetHandle ("initWithCoder:");
#endif

		internal static IntPtr AllocHandle = Selector.GetHandle (Alloc);
		internal static IntPtr ReleaseHandle = Selector.GetHandle (Release);
		internal static IntPtr RetainHandle = Selector.GetHandle (Retain);
		internal static IntPtr AutoreleaseHandle = Selector.GetHandle (Autorelease);
		internal static IntPtr DoesNotRecognizeSelectorHandle = Selector.GetHandle (DoesNotRecognizeSelector);
		internal static IntPtr PerformSelectorOnMainThreadWithObjectWaitUntilDoneHandle = GetHandle (PerformSelectorOnMainThreadWithObjectWaitUntilDone);
		internal static IntPtr PerformSelectorWithObjectAfterDelayHandle = GetHandle (PerformSelectorWithObjectAfterDelay);
	}
}

#endif // MONOMAC
