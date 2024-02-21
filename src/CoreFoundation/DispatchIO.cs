//
// DispatchIO.cs: IO routines for Dispatch
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010 Novell, Inc.
// Copyright 2011-2014 Xamarin Inc
//
// this contains DispatchObject, Group and Queue
//
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
using System.Threading;
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

	public delegate void DispatchIOHandler (DispatchData? data, int error);

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class DispatchIO : DispatchObject {
		[Preserve (Conditional = true)]
		internal DispatchIO (NativeHandle handle, bool owns) : base (handle, owns)
		{
		}

#if !NET
		[Preserve (Conditional = true)]
		internal DispatchIO (NativeHandle handle) : this (handle, false)
		{
		}
#endif

#if !NET
		delegate void DispatchReadWrite (IntPtr block, IntPtr dispatchData, int error);
		static DispatchReadWrite static_DispatchReadWriteHandler = Trampoline_DispatchReadWriteHandler;

		[MonoPInvokeCallback (typeof (DispatchReadWrite))]
#else
		[UnmanagedCallersOnly]
#endif
		static void Trampoline_DispatchReadWriteHandler (IntPtr block, IntPtr dispatchData, int error)
		{
			var del = BlockLiteral.GetTarget<DispatchIOHandler> (block);
			if (del is not null) {
				var dd = dispatchData == IntPtr.Zero ? null : new DispatchData (dispatchData, owns: false);
				del (dd, error);
			}
		}

		[DllImport (Constants.libcLibrary)]
		unsafe extern static void dispatch_read (int fd, nuint length, IntPtr dispatchQueue, BlockLiteral* block);

		//
		// if size == nuint.MaxValue, reads as much data as is available
		//
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void Read (int fd, nuint size, DispatchQueue dispatchQueue, DispatchIOHandler handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));
			if (dispatchQueue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dispatchQueue));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, int, void> trampoline = &Trampoline_DispatchReadWriteHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (DispatchIO), nameof (Trampoline_DispatchReadWriteHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_DispatchReadWriteHandler, handler);
#endif
				dispatch_read (fd, size, dispatchQueue.Handle, &block);
			}
		}

		[DllImport (Constants.libcLibrary)]
		unsafe extern static void dispatch_write (int fd, IntPtr dispatchData, IntPtr dispatchQueue, BlockLiteral* handler);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void Write (int fd, DispatchData dispatchData, DispatchQueue dispatchQueue, DispatchIOHandler handler)
		{
			if (dispatchData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dispatchData));
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));
			if (dispatchQueue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dispatchQueue));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, int, void> trampoline = &Trampoline_DispatchReadWriteHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (DispatchIO), nameof (Trampoline_DispatchReadWriteHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_DispatchReadWriteHandler, handler);
#endif
				dispatch_write (fd, dispatchData.Handle, dispatchQueue.Handle, &block);
			}
		}
	}
}
