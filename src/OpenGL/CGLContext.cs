// 
// CGLContext.cs: Implements the managed CGLContext
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
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

namespace OpenGL {
	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use 'Metal' Framework instead.")]
	public class CGLContext : INativeObject, IDisposable {
		IntPtr handle;

		public CGLContext (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				throw new Exception ("Invalid parameters to context creation");

			CGLRetainContext (handle);
			this.handle = handle;
		}

		internal CGLContext ()
		{
		}

		[Preserve (Conditional=true)]
		internal CGLContext (IntPtr handle, bool owns)
		{
			if (!owns)
				CGLRetainContext (handle);

			this.handle = handle;
		}

		~CGLContext ()
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
	
		[DllImport (Constants.OpenGLLibrary)]
		extern static void CGLRetainContext (IntPtr handle);

		[DllImport (Constants.OpenGLLibrary)]
		extern static void CGLReleaseContext (IntPtr handle);

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGLReleaseContext (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.OpenGLLibrary)]
		extern static CGLErrorCode CGLLockContext (IntPtr ctx);
		public CGLErrorCode Lock ()
		{
			return CGLLockContext (this.handle);
		}

		[DllImport (Constants.OpenGLLibrary)]
		extern static CGLErrorCode CGLUnlockContext (IntPtr ctx);
		public CGLErrorCode Unlock ()
		{
			return CGLUnlockContext (this.handle);
		}
	
		[DllImport (Constants.OpenGLLibrary)]
		extern static CGLErrorCode CGLSetCurrentContext (IntPtr ctx);

		[DllImport (Constants.OpenGLLibrary)]
		extern static IntPtr CGLGetCurrentContext ();

		public static CGLContext CurrentContext {
			get {
				IntPtr ctx = CGLGetCurrentContext ();
				if (ctx != IntPtr.Zero)
					return new CGLContext (ctx);
				else
					return null;
			} 

			set {

				CGLErrorCode retValue = CGLSetCurrentContext (value?.Handle ?? IntPtr.Zero);
				if (retValue != CGLErrorCode.NoError)
					throw new Exception ("Error setting the Current Context");
			}
		}
	}
}
