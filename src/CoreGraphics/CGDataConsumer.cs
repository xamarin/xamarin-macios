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
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace CoreGraphics {

	// CGDataConsumer.h
	public partial class CGDataConsumer : INativeObject, IDisposable {
		internal IntPtr handle;

		// invoked by marshallers
		public CGDataConsumer (IntPtr handle)
			: this (handle, false)
		{
			this.handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CGDataConsumer (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CGDataConsumerRetain (handle);
		}

		~CGDataConsumer ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGDataConsumerRelease (/* CGDataConsumerRef */ IntPtr consumer);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataConsumerRef */ IntPtr CGDataConsumerRetain (/* CGDataConsumerRef */ IntPtr consumer);

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGDataConsumerRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataConsumerRef */ IntPtr CGDataConsumerCreateWithCFData (/* CFMutableDataRef __nullable */ IntPtr data);

		public CGDataConsumer (NSMutableData data)
		{
			// not it's a __nullable parameter but it would return nil (see unit tests) and create an invalid instance
			if (data == null)
				throw new ArgumentNullException ("data");
			handle = CGDataConsumerCreateWithCFData (data.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataConsumerRef */ IntPtr CGDataConsumerCreateWithURL (/* CFURLRef __nullable */ IntPtr url);

		public CGDataConsumer (NSUrl url)
		{
			// not it's a __nullable parameter but it would return nil (see unit tests) and create an invalid instance
			if (url == null)
				throw new ArgumentNullException ("url");
			handle = CGDataConsumerCreateWithURL (url.Handle);
		}
	}
}
