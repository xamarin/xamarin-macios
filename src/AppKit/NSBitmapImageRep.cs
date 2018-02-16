//
// Copyright 2010, Novell, Inc.
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
using Foundation;
using ObjCRuntime;

namespace AppKit {

	public partial class NSBitmapImageRep {
		static IntPtr selInitForIncrementalLoad = Selector.GetHandle ("initForIncrementalLoad");

		// Do not actually export because NSObjectFlag is not exportable.
		// The Objective C method already exists. This is just to allow
		// access on the managed side via the static method.
		//[Export ("initForIncrementalLoad")]
		private NSBitmapImageRep (NSObjectFlag a, NSObjectFlag b) : base (a)
		{
			if (IsDirectBinding) {
				Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, selInitForIncrementalLoad);
			} else {
				Handle = ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, selInitForIncrementalLoad);
			}
		}

		public NSData RepresentationUsingTypeProperties (NSBitmapImageFileType storageType)
		{
			return RepresentationUsingTypeProperties (storageType, null);
		}

		public static NSBitmapImageRep IncrementalLoader ()
		{
			return new NSBitmapImageRep (NSObjectFlag.Empty, NSObjectFlag.Empty);
		}
	}
}

