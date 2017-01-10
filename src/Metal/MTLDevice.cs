//
// API for the Metal framework
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2014-2015, Xamarin Inc.
//
#if XAMCORE_2_0 || !MONOMAC

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Metal {

	[iOS (8,0)][Mac (10,11)]
	public static partial class MTLDevice {
		[DllImport (Constants.MetalLibrary)]
		extern static IntPtr MTLCreateSystemDefaultDevice ();

		static IMTLDevice system_default;
		
		public static IMTLDevice SystemDefault {
			get {
				// Metal could be unavailable on the hardware (and we don't want to return an invalid instance)
				// also the instance could be disposed (by mistake) which would make the app unusable
				if ((system_default == null) || (system_default.Handle == IntPtr.Zero)) {
					try {
						var h = MTLCreateSystemDefaultDevice ();
						if (h != IntPtr.Zero)
							system_default = new MTLDeviceWrapper (h, false);
					}
					catch (EntryPointNotFoundException) {
					}
					catch (DllNotFoundException) {
					}
				}
				return system_default;
			}
		}
	}

	public static partial class MTLDevice_Extensions {
		public static IMTLBuffer CreateBuffer<T> (this IMTLDevice This, T [] data, MTLResourceOptions options) where T : struct
		{
			var handle = GCHandle.Alloc (data, GCHandleType.Pinned); // This requires a pinned GCHandle, since it's not possible to use unsafe code to get the address of a generic object.
			try {
				IntPtr ptr = handle.AddrOfPinnedObject ();
				return This.CreateBuffer (ptr, (nuint)(data.Length * Marshal.SizeOf (typeof (T))) , options);
			} finally {
				handle.Free ();
			}
		}

		public static IMTLBuffer CreateBufferNoCopy<T> (this IMTLDevice This, T [] data, MTLResourceOptions options, MTLDeallocator deallocator) where T : struct
		{
			var handle = GCHandle.Alloc (data, GCHandleType.Pinned); // This requires a pinned GCHandle, since it's not possible to use unsafe code to get the address of a generic object.
			try {
				IntPtr ptr = handle.AddrOfPinnedObject ();
				return This.CreateBufferNoCopy (ptr, (nuint)(data.Length * Marshal.SizeOf (typeof (T))) , options, deallocator);
			} finally {
				handle.Free ();
			}
		}
	}
}
#endif