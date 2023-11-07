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

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// CGFunction.h
	public class CGFunction : NativeObject {
		CGFunctionEvaluate? evaluate;

		static CGFunctionCallbacks cbacks;

		unsafe static CGFunction ()
		{
			cbacks.version = 0;
#if NET
			cbacks.evaluate = &EvaluateCallback;
			cbacks.release = &ReleaseCallback;
#else
			cbacks.evaluate = new CGFunctionEvaluateCallback (EvaluateCallback);
			cbacks.release = new CGFunctionReleaseCallback (ReleaseCallback);
#endif
		}

#if !NET
		internal CGFunction (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGFunction (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		public CGFunctionEvaluate? EvaluateFunction {
			get {
				return evaluate;
			}
			set {
				evaluate = value;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGFunctionRelease (/* CGFunctionRef */ IntPtr function);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGFunctionRef */ IntPtr CGFunctionRetain (/* CGFunctionRef */ IntPtr function);

		protected internal override void Retain ()
		{
			CGFunctionRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGFunctionRelease (GetCheckedHandle ());
		}

		// Apple's documentation says 'float', the header files say 'CGFloat'
		unsafe delegate void CGFunctionEvaluateCallback (/* void* */ IntPtr info, /* CGFloat* */ nfloat* data, /* CGFloat* */ nfloat* outData);
		delegate void CGFunctionReleaseCallback (IntPtr info);

		[StructLayout (LayoutKind.Sequential)]
		struct CGFunctionCallbacks {
			public /* unsigned int */ uint version;
#if NET
			public unsafe delegate* unmanaged<IntPtr, nfloat*, nfloat*, void> evaluate;
			public unsafe delegate* unmanaged<IntPtr, void> release;
#else
			public CGFunctionEvaluateCallback? evaluate;
			public CGFunctionReleaseCallback? release;
#endif
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe IntPtr CGFunctionCreate (/* void* */ IntPtr data, /* size_t */ nint domainDimension, /* CGFloat* */ nfloat* domain, nint rangeDimension, /* CGFloat* */ nfloat* range, ref CGFunctionCallbacks callbacks);

		unsafe public delegate void CGFunctionEvaluate (nfloat* data, nfloat* outData);


		public unsafe CGFunction (nfloat []? domain, nfloat []? range, CGFunctionEvaluate callback)
		{
			if (domain is not null) {
				if ((domain.Length % 2) != 0)
					throw new ArgumentException ("The domain array must consist of pairs of values", nameof (domain));
			}
			if (range is not null) {
				if ((range.Length % 2) != 0)
					throw new ArgumentException ("The range array must consist of pairs of values", nameof (range));
			}
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			this.evaluate = callback;

			var gch = GCHandle.Alloc (this);
			unsafe {
				fixed (nfloat* domainPtr = domain, rangePtr = range) {
					var handle = CGFunctionCreate (GCHandle.ToIntPtr (gch), domain is not null ? domain.Length / 2 : 0, domainPtr, range is not null ? range.Length / 2 : 0, rangePtr, ref cbacks);
					InitializeHandle (handle);
				}
			}
		}
#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (CGFunctionReleaseCallback))]
#endif
#endif
		static void ReleaseCallback (IntPtr info)
		{
			GCHandle.FromIntPtr (info).Free ();
		}

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (CGFunctionEvaluateCallback))]
#endif
#endif
		unsafe static void EvaluateCallback (IntPtr info, nfloat* input, nfloat* output)
		{
			GCHandle lgc = GCHandle.FromIntPtr (info);
			var container = lgc.Target as CGFunction;

			container?.evaluate?.Invoke (input, output);
		}
	}
}
