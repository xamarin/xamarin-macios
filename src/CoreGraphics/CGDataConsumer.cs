//
// CGDataConsumer.cs: Implements the managed CGDataConsumer
//
// Authors: Ademar Gonzalez
//
// Copyright 2009 Novell, Inc
// Copyright 2011-2014 Xamarin Inc
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

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {

	// CGDataConsumer.h
	public partial class CGDataConsumer : NativeObject {
#if !NET
		public CGDataConsumer (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGDataConsumer (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGDataConsumerRelease (/* CGDataConsumerRef */ IntPtr consumer);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataConsumerRef */ IntPtr CGDataConsumerRetain (/* CGDataConsumerRef */ IntPtr consumer);

		protected internal override void Retain ()
		{
			CGDataConsumerRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGDataConsumerRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataConsumerRef */ IntPtr CGDataConsumerCreateWithCFData (/* CFMutableDataRef __nullable */ IntPtr data);

		static IntPtr Create (NSMutableData data)
		{
			// not it's a __nullable parameter but it would return nil (see unit tests) and create an invalid instance
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			return CGDataConsumerCreateWithCFData (data.Handle);
		}

		public CGDataConsumer (NSMutableData data)
			: base (Create (data), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataConsumerRef */ IntPtr CGDataConsumerCreateWithURL (/* CFURLRef __nullable */ IntPtr url);

		static IntPtr Create (NSUrl url)
		{
			// not it's a __nullable parameter but it would return nil (see unit tests) and create an invalid instance
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			return CGDataConsumerCreateWithURL (url.Handle);
		}

		public CGDataConsumer (NSUrl url)
			: base (Create (url), true)
		{
		}
	}
}
