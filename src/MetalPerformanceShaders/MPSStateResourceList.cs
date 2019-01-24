//
// MPSStateResourceList.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
//
// Copyright 2019 Microsoft Corporation.
//

#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;
using Foundation;
using Metal;
using ObjCRuntime;

namespace MetalPerformanceShaders {
	public partial class MPSStateResourceList {
		public static MPSStateResourceList Create (params MTLTextureDescriptor [] descriptors)
		{
			if (descriptors == null)
				throw new ArgumentNullException (nameof (descriptors));
			if (descriptors.Length > 10)
				throw new ArgumentException ("Only 10 parameters are currently supported.");

			var arr = new IntPtr [10];
			for (int i = 0; i < descriptors.Length; ++i) {
				if (descriptors [i] == null)
					throw new ArgumentException ($"'{nameof (descriptors)}[{i}]' was null.");
				arr [i] = descriptors [i].Handle;
			}

			MPSStateResourceList ret;
#if !MONOMAC
			// Learned the hard way about arm64's variadic arguments calling conventions are different...
			if (IntPtr.Size == 8 && Runtime.Arch == Arch.DEVICE)
				ret = Runtime.GetNSObject<MPSStateResourceList> (IntPtr_objc_msgSend_IntPtrx3_FakeIntPtrx5_IntPtrx10 (class_ptr, Selector.GetHandle ("resourceListWithTextureDescriptors:"), arr [0], IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, arr [1], arr [2], arr [3], arr [4], arr [5], arr [6], arr [7], arr [8], arr [9], IntPtr.Zero));
			else
#endif
				ret = Runtime.GetNSObject<MPSStateResourceList> (IntPtr_objc_msgSend_IntPtrx13 (class_ptr, Selector.GetHandle ("resourceListWithTextureDescriptors:"), arr [0], arr [1], arr [2], arr [3], arr [4], arr [5], arr [6], arr [7], arr [8], arr [9], IntPtr.Zero));

			return ret;
		}

		public static MPSStateResourceList Create (params nuint [] bufferSizes)
		{
			if (bufferSizes == null)
				throw new ArgumentNullException (nameof (bufferSizes));
			if (bufferSizes.Length > 10)
				throw new ArgumentException ("Only 10 parameters are currently supported.");

			var arr = new IntPtr [10];
			for (int i = 0; i < bufferSizes.Length; ++i) {
				arr [i] = (IntPtr) bufferSizes [i];
			}

			MPSStateResourceList ret;
#if !MONOMAC
			// Learned the hard way about arm64's variadic arguments calling conventions are different...
			if (IntPtr.Size == 8 && Runtime.Arch == Arch.DEVICE)
				ret = Runtime.GetNSObject<MPSStateResourceList> (IntPtr_objc_msgSend_IntPtrx3_FakeIntPtrx5_IntPtrx10 (class_ptr, Selector.GetHandle ("resourceListWithBufferSizes:"), arr [0], IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, arr [1], arr [2], arr [3], arr [4], arr [5], arr [6], arr [7], arr [8], arr [9], IntPtr.Zero));
			else
#endif
				ret = Runtime.GetNSObject<MPSStateResourceList> (IntPtr_objc_msgSend_IntPtrx13 (class_ptr, Selector.GetHandle ("resourceListWithBufferSizes:"), arr [0], arr [1], arr [2], arr [3], arr [4], arr [5], arr [6], arr [7], arr [8], arr [9], IntPtr.Zero));

			return ret;
		}

#if !MONOMAC
		[DllImport (Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
		static extern IntPtr IntPtr_objc_msgSend_IntPtrx3_FakeIntPtrx5_IntPtrx10 (IntPtr receiver, IntPtr selector, IntPtr arg0, IntPtr argFake1, IntPtr argFake2, IntPtr argFake3, IntPtr argFake4, IntPtr argFake5, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10);
#endif
		[DllImport (Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
		static extern IntPtr IntPtr_objc_msgSend_IntPtrx13 (IntPtr receiver, IntPtr selector, IntPtr arg0, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10);
	}
}
#endif
