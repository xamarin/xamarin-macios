// 
// CGGradient.cs: Implements the managed CGGradient
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2012-2014 Xamarin Inc.
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
using CoreFoundation;
using Foundation;

namespace CoreGraphics {

	// uint32_t -> CGGradient.h
	[Flags]
	public enum CGGradientDrawingOptions : uint {
		None = 0,
		DrawsBeforeStartLocation = (1 << 0),
		DrawsAfterEndLocation = (1 << 1)
	}
	
	public class CGGradient : INativeObject
#if !COREBUILD
		, IDisposable
#endif
	{
#if !COREBUILD
		internal IntPtr handle;

		[Preserve (Conditional=true)]
		internal CGGradient (IntPtr handle, bool owns)
		{
			if (!owns)
				CGGradientRetain (handle);

			this.handle = handle;
		}

		~CGGradient ()
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
		extern static /* CGGradientRef */ IntPtr CGGradientRetain (/* CGGradientRef */ IntPtr gradient);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGGradientRelease (/* CGGradientRef */ IntPtr gradient);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGGradientRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGGradientRef __nullable */ IntPtr CGGradientCreateWithColorComponents (
			/* CGColorSpaceRef __nullable */ IntPtr colorspace, /* const CGFloat* __nullable */ nfloat [] components, 
			/* const CGFloat* __nullable */ nfloat [] locations, /* size_t */ nint count);

		public CGGradient (CGColorSpace colorspace, nfloat [] components, nfloat [] locations)
		{
			// those parameters are __nullable but would return a `nil` instance back,
			// which is not something we can handle nicely from a .NET constructor
			if (colorspace == null)
				throw new ArgumentNullException ("colorspace");
			if (components == null)
				throw new ArgumentNullException ("components");

			handle = CGGradientCreateWithColorComponents (colorspace.handle, components, locations, components.Length / (colorspace.Components+1));
		}

		public CGGradient (CGColorSpace colorspace, nfloat [] components)
		{
			// those parameters are __nullable but would return a `nil` instance back,
			// which is not something we can handle nicely from a .NET constructor
			if (colorspace == null)
				throw new ArgumentNullException ("colorspace");
			if (components == null)
				throw new ArgumentNullException ("components");

			handle = CGGradientCreateWithColorComponents (colorspace.handle, components, null, components.Length / (colorspace.Components+1));
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGGradientRef __nullable */ IntPtr CGGradientCreateWithColors (
			/* CGColorSpaceRef __nullable */ IntPtr space, /* CFArrayRef __nullable */ IntPtr colors, 
			/* const CGFloat* __nullable */ nfloat [] locations);

		public CGGradient (CGColorSpace colorspace, CGColor [] colors, nfloat [] locations)
		{
			// colors is __nullable but would return a `nil` instance back,
			// which is not something we can handle nicely from a .NET constructor
			if (colors == null)
				throw new ArgumentNullException ("colors");
			
			IntPtr csh = colorspace == null ? IntPtr.Zero : colorspace.handle;
			using (var array = CFArray.FromNativeObjects (colors))
				handle = CGGradientCreateWithColors (csh, array.Handle, locations);
		}

		public CGGradient (CGColorSpace colorspace, CGColor [] colors)
		{
			if (colors == null)
				throw new ArgumentNullException ("colors");
			
			IntPtr csh = colorspace == null ? IntPtr.Zero : colorspace.handle;
			using (var array = CFArray.FromNativeObjects (colors))
				handle = CGGradientCreateWithColors (csh, array.Handle, null);
		}
#endif // !COREBUILD
	}
}
