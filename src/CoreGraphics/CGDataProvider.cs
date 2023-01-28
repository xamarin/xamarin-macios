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
	// CGDataProvider.h
	public partial class CGDataProvider : NativeObject {
#if !NET
		public CGDataProvider (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGDataProvider (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGDataProviderRelease (/* CGDataProviderRef */ IntPtr provider);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataProviderRef */ IntPtr CGDataProviderRetain (/* CGDataProviderRef */ IntPtr provider);

		protected internal override void Retain ()
		{
			CGDataProviderRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGDataProviderRelease (GetCheckedHandle ());
		}

#if !COREBUILD
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataProviderRef */ IntPtr CGDataProviderCreateWithFilename (/* const char* */ IntPtr filename);

		static public CGDataProvider? FromFile (string file)
		{
			if (file is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (file));

			using var filePtr = new TransientString (file);
			var handle = CGDataProviderCreateWithFilename (filePtr);
			if (handle == IntPtr.Zero)
				return null;

			return new CGDataProvider (handle, true);
		}

		static IntPtr Create (string file)
		{
			if (file is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (file));

			using var filePtr = new TransientString (file);
			var handle = CGDataProviderCreateWithFilename (filePtr);
			if (handle == IntPtr.Zero)
				throw new ArgumentException ("Could not create provider from the specified file");
			return handle;
		}

		public CGDataProvider (string file)
			: base (Create (file), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGDataProviderRef */ IntPtr CGDataProviderCreateWithURL (/* CFURLRef __nullable */ IntPtr url);

		static IntPtr Create (NSUrl url)
		{
			// not it's a __nullable parameter but it would return nil (see unit tests) and create an invalid instance
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			return CGDataProviderCreateWithURL (url.Handle);
		}

		public CGDataProvider (NSUrl url)
			: base (Create (url), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGDataProviderRef */ IntPtr CGDataProviderCreateWithCFData (/* CFDataRef __nullable */ IntPtr data);

		static IntPtr Create (NSData data)
		{
			// not it's a __nullable parameter but it would return nil (see unit tests) and create an invalid instance
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			return CGDataProviderCreateWithCFData (data.Handle);
		}

		public CGDataProvider (NSData data)
			: base (Create (data), true)
		{
		}

#if NET
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe IntPtr CGDataProviderCreateWithData (/* void* */ IntPtr info, /* const void* */ IntPtr data, /* size_t */ nint size, /* CGDataProviderReleaseDataCallback */ delegate* unmanaged<IntPtr, IntPtr, nint, void> releaseData);
#else
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGDataProviderCreateWithData (/* void* */ IntPtr info, /* const void* */ IntPtr data, /* size_t */ nint size, /* CGDataProviderReleaseDataCallback */ CGDataProviderReleaseDataCallback releaseData);
#endif

#if !NET
		delegate void CGDataProviderReleaseDataCallback (IntPtr info, IntPtr data, nint size);
		static CGDataProviderReleaseDataCallback release_gchandle_callback = ReleaseGCHandle;
		static CGDataProviderReleaseDataCallback release_buffer_callback = ReleaseBuffer;
		static CGDataProviderReleaseDataCallback release_func_callback = ReleaseFunc;
#endif

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CGDataProviderReleaseDataCallback))]
#endif
		private static void ReleaseGCHandle (IntPtr info, IntPtr data, nint size)
		{
			var gch = GCHandle.FromIntPtr (info);
			gch.Free ();
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CGDataProviderReleaseDataCallback))]
#endif
		private static void ReleaseBuffer (IntPtr info, IntPtr data, nint size)
		{
			if (data != IntPtr.Zero)
				Marshal.FreeHGlobal (data);
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CGDataProviderReleaseDataCallback))]
#endif
		private static void ReleaseFunc (IntPtr info, IntPtr data, nint size)
		{
			var gch = GCHandle.FromIntPtr (info);
			var target = gch.Target as Action<IntPtr>;
			try {
				if (target is not null)
					target (data);
			} finally {
				gch.Free ();
			}
		}

		public CGDataProvider (IntPtr memoryBlock, int size)
			: this (memoryBlock, size, false)
		{
		}

		static IntPtr Create (IntPtr memoryBlock, int size, bool ownBuffer)
		{
			if (!ownBuffer)
				memoryBlock = Runtime.CloneMemory (memoryBlock, size);
#if NET
			unsafe {
				return CGDataProviderCreateWithData (IntPtr.Zero, memoryBlock, size, &ReleaseBuffer);
			}
#else
			return CGDataProviderCreateWithData (IntPtr.Zero, memoryBlock, size, release_buffer_callback);
#endif
		}

		public CGDataProvider (IntPtr memoryBlock, int size, bool ownBuffer)
			: base (Create (memoryBlock, size, ownBuffer), true)
		{
		}

		static IntPtr Create (IntPtr memoryBlock, int size, Action<IntPtr> releaseMemoryBlockCallback)
		{
			if (releaseMemoryBlockCallback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (releaseMemoryBlockCallback));

			var gch = GCHandle.Alloc (releaseMemoryBlockCallback);
#if NET
			unsafe {
				return CGDataProviderCreateWithData (GCHandle.ToIntPtr (gch), memoryBlock, size, &ReleaseFunc);
			}
#else
			return CGDataProviderCreateWithData (GCHandle.ToIntPtr (gch), memoryBlock, size, release_func_callback);
#endif
		}

		public CGDataProvider (IntPtr memoryBlock, int size, Action<IntPtr> releaseMemoryBlockCallback)
			: base (Create (memoryBlock, size, releaseMemoryBlockCallback), true)
		{
		}

		static IntPtr Create (byte [] buffer, int offset, int count)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			if (offset < 0 || offset > buffer.Length)
				throw new ArgumentException (nameof (offset));
			if (offset + count > buffer.Length)
				throw new ArgumentException (nameof (offset));

			var gch = GCHandle.Alloc (buffer, GCHandleType.Pinned); // This requires a pinned GCHandle, because unsafe code is scoped to the current block, and the address of the byte array will be used after this function returns.
			var ptr = gch.AddrOfPinnedObject () + offset;
#if NET
			unsafe {
				return CGDataProviderCreateWithData (GCHandle.ToIntPtr (gch), ptr, count, &ReleaseGCHandle);
			}
#else
			return CGDataProviderCreateWithData (GCHandle.ToIntPtr (gch), ptr, count, release_gchandle_callback);
#endif
		}

		public CGDataProvider (byte [] buffer, int offset, int count)
			: base (Create (buffer, offset, count), true)
		{
		}

		public CGDataProvider (byte [] buffer)
			: this (buffer, 0, buffer.Length)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CFDataRef */ IntPtr CGDataProviderCopyData (/* CGDataProviderRef */ IntPtr provider);

		public NSData? CopyData ()
		{
			return Runtime.GetNSObject<NSData> (CGDataProviderCopyData (Handle), true);
		}
#endif
	}
}
