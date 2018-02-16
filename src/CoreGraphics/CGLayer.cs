// 
// CGLayer.cs: Implements the managed CGLayer
//
// Authors: Mono Team
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

	// CGLayer.h
	public class CGLayer : INativeObject
#if !COREBUILD
		, IDisposable
#endif
	{
#if !COREBUILD
		IntPtr handle;

		internal CGLayer (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				throw new Exception ("Invalid parameters to layer creation");
					
			this.handle = handle;
			CGLayerRetain (handle);
		}

		[Preserve (Conditional=true)]
		internal CGLayer (IntPtr handle, bool owns)
		{
			if (!owns)
				CGLayerRetain (handle);

			this.handle = handle;
		}

		~CGLayer ()
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
		extern static void CGLayerRelease (/* CGLayerRef */ IntPtr layer);
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGLayerRef */ IntPtr CGLayerRetain (/* CGLayerRef */ IntPtr layer);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGLayerRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGSize CGLayerGetSize (/* CGLayerRef */ IntPtr layer);

		public CGSize Size {
			get {
				return CGLayerGetSize (handle);
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGContextRef */ IntPtr CGLayerGetContext (/* CGLayerRef */ IntPtr layer);

		public CGContext Context {
			get {
				return new CGContext (CGLayerGetContext (handle));
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGLayerRef */ IntPtr CGLayerCreateWithContext (/* CGContextRef */ IntPtr context, CGSize size, /* CFDictionaryRef */ IntPtr auxiliaryInfo);

		public static CGLayer Create (CGContext context, CGSize size)
		{
			// note: auxiliaryInfo is reserved and should be null
			return new CGLayer (CGLayerCreateWithContext (context == null ? IntPtr.Zero : context.Handle, size, IntPtr.Zero), true);
		}
#endif
	}
}
