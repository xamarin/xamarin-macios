// 
// CGBitmapContext.cs:
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2014 Xamarin Inc.
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
	public class CGBitmapContext : CGContext {
#if !COREBUILD
		// If allocated, this points to the byte array buffer that is passed.
		GCHandle buffer;

		[Preserve (Conditional = true)]
		internal CGBitmapContext (NativeHandle handle, bool owns) : base (handle, owns)
		{
		}

		// CGBitmapInfo -> uint32_t -> CGImage.h
		// CGImageAlphaInfo -> uint32_t -> CGImage.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGBitmapContextCreate (/* void* */ IntPtr data, /* size_t */ nint width, /* size_t */ nint height, /* size_t */ nint bitsPerComponent,
			/* size_t */ nint bytesPerRow, /* CGColorSpaceRef */ IntPtr colorSpace, /* CGBitmapInfo = uint32_t */ uint bitmapInfo);

		public CGBitmapContext (IntPtr data, nint width, nint height, nint bitsPerComponent, nint bytesPerRow, CGColorSpace? colorSpace, CGImageAlphaInfo bitmapInfo)
			: base (CGBitmapContextCreate (data, width, height, bitsPerComponent, bytesPerRow, colorSpace.GetHandle (), (uint) bitmapInfo), true)
		{
		}

		public CGBitmapContext (IntPtr data, nint width, nint height, nint bitsPerComponent, nint bytesPerRow, CGColorSpace? colorSpace, CGBitmapFlags bitmapInfo)
			: base (CGBitmapContextCreate (data, width, height, bitsPerComponent, bytesPerRow, colorSpace.GetHandle (), (uint) bitmapInfo), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGBitmapContextCreate (/* void* */ byte []? data, /* size_t */ nint width, /* size_t */ nint height, /* size_t */ nint bitsPerComponent,
			/* size_t */ nint bytesPerRow, /* CGColorSpaceRef */ IntPtr colorSpace, /* CGBitmapInfo = uint32_t */ uint bitmapInfo);

		static IntPtr Create (byte []? data, nint width, nint height, nint bitsPerComponent, nint bytesPerRow, CGColorSpace? colorSpace, CGImageAlphaInfo bitmapInfo, out GCHandle buffer)
		{
			buffer = default (GCHandle);
			if (data is not null)
				buffer = GCHandle.Alloc (data, GCHandleType.Pinned); // This requires a pinned GCHandle, because unsafe code is scoped to the current block, and the address of the byte array will be used after this function returns.
			return CGBitmapContextCreate (data, width, height, bitsPerComponent, bytesPerRow, colorSpace.GetHandle (), (uint) bitmapInfo);
		}

		public CGBitmapContext (byte []? data, nint width, nint height, nint bitsPerComponent, nint bytesPerRow, CGColorSpace? colorSpace, CGImageAlphaInfo bitmapInfo)
			: base (Create (data, width, height, bitsPerComponent, bytesPerRow, colorSpace, bitmapInfo, out var buffer), true)
		{
			this.buffer = buffer;
		}

		static IntPtr Create (byte []? data, nint width, nint height, nint bitsPerComponent, nint bytesPerRow, CGColorSpace? colorSpace, CGBitmapFlags bitmapInfo, out GCHandle buffer)
		{
			buffer = default (GCHandle);
			if (data is not null)
				buffer = GCHandle.Alloc (data, GCHandleType.Pinned); // This requires a pinned GCHandle, because unsafe code is scoped to the current block, and the address of the byte array will be used after this function returns.
			return CGBitmapContextCreate (data, width, height, bitsPerComponent, bytesPerRow, colorSpace.GetHandle (), (uint) bitmapInfo);
		}

		public CGBitmapContext (byte []? data, nint width, nint height, nint bitsPerComponent, nint bytesPerRow, CGColorSpace? colorSpace, CGBitmapFlags bitmapInfo)
			: base (Create (data, width, height, bitsPerComponent, bytesPerRow, colorSpace, bitmapInfo, out var buffer), true)
		{
			this.buffer = buffer;
		}

		protected override void Dispose (bool disposing)
		{
			if (buffer.IsAllocated)
				buffer.Free ();
			base.Dispose (disposing);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* void* */ IntPtr CGBitmapContextGetData (/* CGContextRef */ IntPtr context);

		public IntPtr Data {
			get { return CGBitmapContextGetData (Handle); }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGBitmapContextGetWidth (/* CGContextRef */ IntPtr context);

		public nint Width {
			get { return CGBitmapContextGetWidth (Handle); }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGBitmapContextGetHeight (/* CGContextRef */ IntPtr context);

		public nint Height {
			get { return CGBitmapContextGetHeight (Handle); }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGBitmapContextGetBitsPerComponent (/* CGContextRef */ IntPtr context);

		public nint BitsPerComponent {
			get { return CGBitmapContextGetBitsPerComponent (Handle); }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGBitmapContextGetBitsPerPixel (/* CGContextRef */ IntPtr context);

		public nint BitsPerPixel {
			get { return (nint) CGBitmapContextGetBitsPerPixel (Handle); }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGBitmapContextGetBytesPerRow (/* CGContextRef */ IntPtr context);

		public nint BytesPerRow {
			get { return CGBitmapContextGetBytesPerRow (Handle); }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGBitmapContextGetColorSpace (/* CGContextRef */ IntPtr context);

		public CGColorSpace? ColorSpace {
			get {
				var ptr = CGBitmapContextGetColorSpace (Handle);
				return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, false);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGImageAlphaInfo CGBitmapContextGetAlphaInfo (/* CGContextRef */ IntPtr context);

		public CGImageAlphaInfo AlphaInfo {
			get { return CGBitmapContextGetAlphaInfo (Handle); }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGBitmapInfo */ uint CGBitmapContextGetBitmapInfo (/* CGContextRef */ IntPtr context);

		public CGBitmapFlags BitmapInfo {
			get { return (CGBitmapFlags) CGBitmapContextGetBitmapInfo (Handle); }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGBitmapContextCreateImage (/* CGContextRef */ IntPtr context);

		public CGImage? ToImage ()
		{
			var h = CGBitmapContextCreateImage (Handle);
			// do not return an invalid instance (null handle) if something went wrong
			return h == IntPtr.Zero ? null : new CGImage (h, true);
		}
#endif // !COREBUILD
	}
}
