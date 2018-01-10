// 
// CGDataProvider.cs: Implements the managed CGDataProvider
//
// Authors: Miguel de Icaza
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

	// CGDataProvider.h
	public partial class CGDataProvider : INativeObject, IDisposable {
		IntPtr handle;

		// invoked by marshallers
		public CGDataProvider (IntPtr handle)
			: this (handle, false)
		{
		}

		[Preserve (Conditional=true)]
		internal CGDataProvider (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CGDataProviderRetain (handle);
		}
		
		~CGDataProvider ()
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
		extern static void CGDataProviderRelease (/* CGDataProviderRef */ IntPtr provider);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataProviderRef */ IntPtr CGDataProviderRetain (/* CGDataProviderRef */ IntPtr provider);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGDataProviderRelease (handle);
				handle = IntPtr.Zero;
			}
		}
#if !COREBUILD
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataProviderRef */ IntPtr CGDataProviderCreateWithFilename (/* const char* */ string filename);

		static public CGDataProvider FromFile (string file)
		{
			if (file == null)
				throw new ArgumentNullException ("file");

			var handle = CGDataProviderCreateWithFilename (file);
			if (handle == IntPtr.Zero)
				return null;

			return new CGDataProvider (handle, true);
		}

		public CGDataProvider (string file)
		{
			if (file == null)
				throw new ArgumentNullException ("file");

			handle = CGDataProviderCreateWithFilename (file);
			if (handle == IntPtr.Zero)
				throw new ArgumentException ("Could not create provider from the specified file");
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGDataProviderRef */ IntPtr CGDataProviderCreateWithURL (/* CFURLRef __nullable */ IntPtr url);

		public CGDataProvider (NSUrl url)
		{
			// not it's a __nullable parameter but it would return nil (see unit tests) and create an invalid instance
			if (url == null)
				throw new ArgumentNullException ("url");
			handle = CGDataProviderCreateWithURL (url.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGDataProviderRef */ IntPtr CGDataProviderCreateWithCFData (/* CFDataRef __nullable */ IntPtr data);

		public CGDataProvider (NSData data)
		{
			// not it's a __nullable parameter but it would return nil (see unit tests) and create an invalid instance
			if (data == null)
				throw new ArgumentNullException ("data");
			handle = CGDataProviderCreateWithCFData (data.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGDataProviderCreateWithData (/* void* */ IntPtr info, /* const void* */ IntPtr data, /* size_t */ nint size, /* CGDataProviderReleaseDataCallback */ CGDataProviderReleaseDataCallback releaseData);

		delegate void CGDataProviderReleaseDataCallback (IntPtr info, IntPtr data, nint size);
		static CGDataProviderReleaseDataCallback release_gchandle_callback = ReleaseGCHandle;
		static CGDataProviderReleaseDataCallback release_buffer_callback = ReleaseBuffer;
		static CGDataProviderReleaseDataCallback release_func_callback = ReleaseFunc;

		[MonoPInvokeCallback (typeof (CGDataProviderReleaseDataCallback))]
		private static void ReleaseGCHandle (IntPtr info, IntPtr data, nint size)
		{
			var gch = GCHandle.FromIntPtr (info);
			gch.Free ();
		}

		[MonoPInvokeCallback (typeof (CGDataProviderReleaseDataCallback))]
		private static void ReleaseBuffer (IntPtr info, IntPtr data, nint size)
		{
			if (data != IntPtr.Zero)
				Marshal.FreeHGlobal (data);
		}

		[MonoPInvokeCallback (typeof (CGDataProviderReleaseDataCallback))]
		private static void ReleaseFunc (IntPtr info, IntPtr data, nint size)
		{
			var gch = GCHandle.FromIntPtr (info);
			var target = (Action<IntPtr>) gch.Target;
			try {
				target (data);
			} finally {
				gch.Free ();
			}
		}

		public CGDataProvider (IntPtr memoryBlock, int size)
			: this (memoryBlock, size, false)
		{
		}

		public CGDataProvider (IntPtr memoryBlock, int size, bool ownBuffer)
		{
			if (!ownBuffer)
				memoryBlock = Runtime.CloneMemory (memoryBlock, size);
			handle = CGDataProviderCreateWithData (IntPtr.Zero, memoryBlock, size, release_buffer_callback);
		}

		public CGDataProvider (IntPtr memoryBlock, int size, Action<IntPtr> releaseMemoryBlockCallback)
		{
			if (releaseMemoryBlockCallback == null)
				throw new ArgumentNullException (nameof (releaseMemoryBlockCallback));
			
			var gch = GCHandle.Alloc (releaseMemoryBlockCallback);
			handle = CGDataProviderCreateWithData (GCHandle.ToIntPtr (gch), memoryBlock, size, release_func_callback);
		}

		public CGDataProvider (byte [] buffer, int offset, int count)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			if (offset < 0 || offset > buffer.Length)
				throw new ArgumentException ("offset");
			if (offset + count > buffer.Length)
				throw new ArgumentException ("offset");

			var gch = GCHandle.Alloc (buffer, GCHandleType.Pinned); // This requires a pinned GCHandle, because unsafe code is scoped to the current block, and the address of the byte array will be used after this function returns.
			var ptr = gch.AddrOfPinnedObject () + offset;
			handle = CGDataProviderCreateWithData (GCHandle.ToIntPtr (gch), ptr, count, release_gchandle_callback);
		}

		public CGDataProvider (byte [] buffer)
			: this (buffer, 0, buffer.Length)
		{
		}
#endif
	}
}
