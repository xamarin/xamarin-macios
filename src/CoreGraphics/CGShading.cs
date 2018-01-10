// 
// CGShading.cs: Implements the managed CGShading
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

	// CGShading.h
	public class CGShading : INativeObject
#if !COREBUILD
		, IDisposable
#endif
	{
#if !COREBUILD
		internal IntPtr handle;

		/* invoked by marshallers */
		public CGShading (IntPtr handle)
		{
			this.handle = handle;
			CGShadingRetain (handle);
		}

		[Preserve (Conditional=true)]
		internal CGShading (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CGShadingRetain (handle);
		}
		

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGShadingRef */ IntPtr CGShadingCreateAxial (/* CGColorSpaceRef */ IntPtr space, 
			CGPoint start, CGPoint end, /* CGFunctionRef */ IntPtr functionHandle, bool extendStart, bool extendEnd);

		public static CGShading CreateAxial (CGColorSpace colorspace, CGPoint start, CGPoint end, CGFunction function, bool extendStart, bool extendEnd)
		{
			if (colorspace == null)
				throw new ArgumentNullException ("colorspace");
			if (colorspace.Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("colorspace");
			if (function == null)
				throw new ArgumentNullException ("function");
			if (function.Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("function");

			return new CGShading (CGShadingCreateAxial (colorspace.Handle, start, end, function.Handle, extendStart, extendEnd), true);
		}
		
		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGShadingRef */ IntPtr CGShadingCreateRadial (/* CGColorSpaceRef */ IntPtr space, 
			CGPoint start, /* CGFloat */ nfloat startRadius, CGPoint end, /* CGFloat */ nfloat endRadius,
			/* CGFunctionRef */ IntPtr function, bool extendStart, bool extendEnd);

		public static CGShading CreateRadial (CGColorSpace colorspace, CGPoint start, nfloat startRadius, CGPoint end, nfloat endRadius,
						      CGFunction function, bool extendStart, bool extendEnd)
		{
			if (colorspace == null)
				throw new ArgumentNullException ("colorspace");
			if (colorspace.Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("colorspace");
			if (function == null)
				throw new ArgumentNullException ("function");
			if (function.Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("function");

			return new CGShading (CGShadingCreateRadial (colorspace.Handle, start, startRadius, end, endRadius,
								     function.Handle, extendStart, extendEnd), true);
		}

		~CGShading ()
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
		extern static /* CGShadingRef */ IntPtr CGShadingRelease (/* CGShadingRef */ IntPtr shading);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGShadingRetain (/* CGShadingRef */ IntPtr shading);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGShadingRelease (handle);
				handle = IntPtr.Zero;
			}
		}
#endif // !COREBUILD
	}
}
