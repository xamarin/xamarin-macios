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
// this contains DispathcObject, Group and Queue
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
using System;
using System.Runtime.InteropServices;
using System.Threading;
using ObjCRuntime;
using Foundation;

namespace CoreFoundation {
	
	public delegate void DispatchIOHandler (DispatchData data, int error);
	
	public class DispatchIO : DispatchObject {
		[Preserve (Conditional = true)]
		internal DispatchIO (IntPtr handle, bool owns) : base (handle, owns)
		{
		}
		[Preserve (Conditional = true)]
		internal DispatchIO (IntPtr handle) : this (handle, false)
		{
		}

		delegate void DispatchReadWrite (IntPtr block, IntPtr dispatchData, int error);
		static DispatchReadWrite static_DispatchReadWriteHandler = Trampoline_DispatchReadWriteHandler;

		[MonoPInvokeCallback(typeof(DispatchReadWrite))]
		static unsafe void Trampoline_DispatchReadWriteHandler (IntPtr block, IntPtr dispatchData, int error)
		{
                        var descriptor = (BlockLiteral *) block;
                        var del = (DispatchIOHandler) (descriptor->Target);
                        if (del != null){
				var dd = dispatchData == IntPtr.Zero ? null : new DispatchData (dispatchData, owns: false);
                                del (dd, error);
			}
		}
			

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_read (int fd, ulong length, IntPtr dispatchQueue, IntPtr block);

		//
		// if size == ULong.MaxValue, reads as much data as is available
		//
		public static void Read (int fd, ulong size, DispatchQueue dispatchQueue, DispatchIOHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException (nameof(handler));
			if (dispatchQueue == null)
				throw new ArgumentNullException (nameof(dispatchQueue));
                        unsafe {
                                BlockLiteral *block_ptr_handler;
                                BlockLiteral block_handler;
                                block_handler = new BlockLiteral ();
                                block_ptr_handler = &block_handler;
                                block_handler.SetupBlockUnsafe (static_DispatchReadWriteHandler, handler);

                                dispatch_read (fd, size, dispatchQueue.Handle, (IntPtr) block_ptr_handler);
                                block_ptr_handler->CleanupBlock ();
                        }
		}

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_write (int fd, IntPtr dispatchData, IntPtr dispatchQueue, IntPtr handler);

		public static void Write (int fd, DispatchData dispatchData, DispatchQueue dispatchQueue, DispatchIOHandler handler)
		{
			if (dispatchData == null)
				throw new ArgumentNullException (nameof(dispatchData));				
			if (handler == null)
				throw new ArgumentNullException (nameof(handler));
			if (dispatchQueue == null)
				throw new ArgumentNullException (nameof(dispatchQueue));
                        unsafe {
                                BlockLiteral *block_ptr_handler;
                                BlockLiteral block_handler;
                                block_handler = new BlockLiteral ();
                                block_ptr_handler = &block_handler;
                                block_handler.SetupBlockUnsafe (static_DispatchReadWriteHandler, handler);

                                dispatch_write (fd, dispatchData.Handle, dispatchQueue.Handle, (IntPtr) block_ptr_handler);
                                block_ptr_handler->CleanupBlock ();
                        }
		}
	}
}
