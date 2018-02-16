// 
// CGFunction.cs: Implements the managed CGFuntion
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
// Copyright 2014 Xamarin Inc
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

	// CGFunction.h
	public class CGFunction : INativeObject, IDisposable {
		internal IntPtr handle;
		GCHandle gch;
		CGFunctionEvaluate evaluate;
		
		// invoked by marshallers
		internal CGFunction (IntPtr handle)
			: this (handle, false)
		{
			this.handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CGFunction (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CGFunctionRetain (handle);
		}
		
		~CGFunction ()
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
		extern static void CGFunctionRelease (/* CGFunctionRef */ IntPtr function);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGFunctionRef */ IntPtr CGFunctionRetain (/* CGFunctionRef */ IntPtr function);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGFunctionRelease (handle);
				handle = IntPtr.Zero;
				gch.Free ();
				evaluate = null;
			}
		}

		// Apple's documentation says 'float', the header files say 'CGFloat'
		unsafe delegate void CGFunctionEvaluateCallback (/* void* */ IntPtr info, /* CGFloat* */ nfloat *data, /* CGFloat* */ nfloat *outData); 
			
		[StructLayout (LayoutKind.Sequential)]
		struct CGFunctionCallbacks {
			public /* unsigned int */ uint version;
			public CGFunctionEvaluateCallback evaluate;
			public /* CGFunctionReleaseInfoCallback */ IntPtr release;
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGFunctionCreate (/* void* */ IntPtr data, /* size_t */ nint domainDimension, /* CGFloat* */ nfloat [] domain, nint rangeDimension, /* CGFloat* */ nfloat [] range, ref CGFunctionCallbacks callbacks);
		
		unsafe public delegate void CGFunctionEvaluate (nfloat *data, nfloat *outData);

		public unsafe CGFunction (nfloat [] domain, nfloat [] range, CGFunctionEvaluate callback)
		{
			if (domain != null){
				if ((domain.Length % 2) != 0)
					throw new ArgumentException ("The domain array must consist of pairs of values", "domain");
			}
			if (range != null) {
				if ((range.Length % 2) != 0)
					throw new ArgumentException ("The range array must consist of pairs of values", "range");
			}
			if (callback == null)
				throw new ArgumentNullException ("callback");

			this.evaluate = callback;

			CGFunctionCallbacks cbacks;
			cbacks.version = 0;
			cbacks.evaluate = new CGFunctionEvaluateCallback (EvaluateCallback);
			cbacks.release = IntPtr.Zero;

			gch = GCHandle.Alloc (this);
			handle = CGFunctionCreate (GCHandle.ToIntPtr (gch), domain != null ? domain.Length/2 : 0, domain, range != null ? range.Length/2 : 0, range, ref cbacks);
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (CGFunctionEvaluateCallback))]
#endif
		unsafe static void EvaluateCallback (IntPtr info, nfloat *input, nfloat *output)
		{
			GCHandle lgc = GCHandle.FromIntPtr (info);
			CGFunction container = (CGFunction) lgc.Target;

			container.evaluate (input, output);
		}
	}
}
