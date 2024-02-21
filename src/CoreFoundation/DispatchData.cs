//
// DispatchData.cs: Support for dispatch_data_t APIs
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2018 Microsoft, Inc.
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

	public partial class DispatchData : DispatchObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
#if NET
		internal DispatchData (NativeHandle handle, bool owns) : base (handle, owns)
#else
		public DispatchData (NativeHandle handle, bool owns) : base (handle, owns)
#endif
		{
		}

#if !NET
		public DispatchData (NativeHandle handle) : base (handle, false)
		{
		}
#endif

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_data_create (IntPtr buffer, nuint size, IntPtr dispatchQueue, IntPtr destructor);

		//
		// This constructor will do it for now, but we should support a constructor
		// that allows custom releasing of the buffer
		//
		public static DispatchData FromByteBuffer (byte [] buffer)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			var b = Marshal.AllocHGlobal (buffer.Length);
			Marshal.Copy (buffer, 0, b, buffer.Length);
			var dd = dispatch_data_create (b, (nuint) buffer.Length, IntPtr.Zero, destructor: free);
			return new DispatchData (dd, owns: true);
		}

		public static DispatchData FromByteBuffer (byte [] buffer, int start, int length)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			if (start < 0 || start >= buffer.Length)
				throw new ArgumentException (nameof (start));
			if (length < 0)
				throw new ArgumentException (nameof (length));
			if (start > buffer.Length + length)
				throw new ArgumentException ("Start+Length go beyond the buffer.Length");

			var b = Marshal.AllocHGlobal (length);
			Marshal.Copy (buffer, start, b, length);
			var dd = dispatch_data_create (b, (nuint) length, IntPtr.Zero, destructor: free);
			return new DispatchData (dd, owns: true);
		}

		//
		// This will create a DispatchData by making a copy of the provided buffer
		//
		public static DispatchData FromBuffer (IntPtr buffer, nuint size)
		{
			if (buffer == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			var dd = dispatch_data_create (buffer, (nuint) size, IntPtr.Zero, destructor: IntPtr.Zero);
			return new DispatchData (dd, owns: true);
		}

		// create a dd using the data of the span 
		public static DispatchData FromReadOnlySpan (ReadOnlySpan<byte> content)
		{
			unsafe {
				fixed (byte* ptr = content) {
					// As per the documentation for dispatch_data_create, seems the data the pointer points to is copied 
					// internally when specifying DISPATCH_DATA_DESTRUCTOR_DEFAULT as the destructor,
					// and DISPATCH_DATA_DESTRUCTOR_DEFAULT=NULL, which is what you're passing, which means the code is OK. 
					var dd = dispatch_data_create ((IntPtr) ptr, (nuint) content.Length, IntPtr.Zero, destructor: IntPtr.Zero);
					return new DispatchData (dd, owns: true);
				}
			}
		}

		[DllImport (Constants.libcLibrary)]
		extern static nuint dispatch_data_get_size (IntPtr handle);

		public nuint Size => dispatch_data_get_size (Handle);

		[DllImport (Constants.libcLibrary)]
		unsafe extern static IntPtr dispatch_data_create_map (IntPtr handle, IntPtr* bufferPtr, nuint* size);

		public unsafe DispatchData CreateMap (out IntPtr bufferPtr, out nuint size)
		{
			bufferPtr = default;
			size = default;
			var nh = dispatch_data_create_map (Handle,
												(IntPtr*) Unsafe.AsPointer<IntPtr> (ref bufferPtr),
												(nuint*) Unsafe.AsPointer<nuint> (ref size));
			return new DispatchData (nh, owns: true);
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_data_create_concat (IntPtr h1, IntPtr h2);

		public static DispatchData Concat (DispatchData data1, DispatchData data2)
		{
			if (data1 is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data1));
			if (data2 is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data2));

			return new DispatchData (dispatch_data_create_concat (data1.Handle, data2.Handle), owns: true);
		}

		[DllImport (Constants.libcLibrary)]
		extern static IntPtr dispatch_data_create_subrange (IntPtr handle, nuint offset, nuint size);

		public DispatchData CreateSubrange (nuint offset, nuint size)
		{
			return new DispatchData (dispatch_data_create_subrange (Handle, offset, size), owns: true);
		}

		// copies the dispatch data to a managed array
		public byte [] ToArray ()
		{
			IntPtr bufferAddress = IntPtr.Zero;
			nuint bufferSize = 0;
			using DispatchData dataCopy = CreateMap (out bufferAddress, out bufferSize);

			byte [] managedArray = new byte [(int) bufferSize];
			Marshal.Copy (bufferAddress, managedArray, 0, (int) bufferSize);
			return managedArray;
		}

#endif
	}
}
