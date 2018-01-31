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
#if MONOMAC
	[Advice ("The 'NSString' argument will match a property of 'MTLDeviceNotificationHandler'.")]
	public delegate void MTLDeviceNotificationHandler (IMTLDevice device, NSString notifyName);
#endif

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

#if MONOMAC
		[Mac (10,11, onlyOn64: true), NoiOS, NoWatch, NoTV]
		[DllImport (Constants.MetalLibrary)]
		unsafe static extern IntPtr MTLCopyAllDevices ();

		[Mac (10,11, onlyOn64: true), NoiOS, NoWatch, NoTV]
		public static IMTLDevice [] GetAllDevices ()
		{
			var rv = MTLCopyAllDevices ();
			return NSArray.ArrayFromHandle<IMTLDevice> (rv);
		}

		[Mac (10, 13, onlyOn64: true), NoiOS, NoWatch, NoTV]
		[DllImport (Constants.MetalLibrary)]
		unsafe static extern IntPtr MTLCopyAllDevicesWithObserver (ref IntPtr observer, void* handler);

		[Mac (10, 13, onlyOn64: true), NoiOS, NoWatch, NoTV]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static IMTLDevice [] GetAllDevices (ref NSObject observer, MTLDeviceNotificationHandler handler)
		{
			if (observer == null)
				throw new ArgumentNullException ("observer");

			IntPtr handle = observer.Handle;

			unsafe
			{
				BlockLiteral* block_ptr_handler;
				BlockLiteral block_handler;
				block_handler = new BlockLiteral ();
				block_ptr_handler = &block_handler;
				block_handler.SetupBlockUnsafe (static_notificationHandler, handler);

				var rv = MTLCopyAllDevicesWithObserver (ref handle, (void*) block_ptr_handler);
				var obj = NSArray.ArrayFromHandle<IMTLDevice> (rv);

				if (handle != observer.Handle)
					observer = Runtime.GetNSObject (handle);

				return obj;
			}
		}

		internal delegate void InnerNotification (IntPtr block, IntPtr device, IntPtr notifyName);
		static readonly InnerNotification static_notificationHandler = TrampolineNotificationHandler;
		[MonoPInvokeCallback (typeof (InnerNotification))]
		public static unsafe void TrampolineNotificationHandler (IntPtr block, IntPtr device, IntPtr notifyName)
		{
			var descriptor = (BlockLiteral*) block;
			var del = (MTLDeviceNotificationHandler) (descriptor->Target);
			if (del != null)
				del ((IMTLDevice) Runtime.GetNSObject (device), (XamCore.Foundation.NSString) Runtime.GetNSObject (notifyName));
		}

		[Mac (10, 13, onlyOn64: true), NoiOS, NoWatch, NoTV]
		[DllImport (Constants.MetalLibrary)]
		static extern void MTLRemoveDeviceObserver (IntPtr observer);

		[Mac (10, 13, onlyOn64: true), NoiOS, NoWatch, NoTV]
		public static void RemoveObserver (NSObject observer)
		{
			if (observer == null)
				throw new ArgumentNullException ("observer");

			MTLRemoveDeviceObserver (observer.Handle);
		}
#endif
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
				return This.CreateBufferNoCopy (ptr, (nuint)(data.Length * Marshal.SizeOf (typeof (T))), options, deallocator);
			} finally {
				handle.Free ();
			}
		}

		public unsafe static void GetDefaultSamplePositions (this IMTLDevice This, MTLSamplePosition [] positions, nuint count)
		{
			if (positions.Length < (nint)count)
				throw new ArgumentException ("Length of 'positions' cannot be less than 'count'.");
			fixed (void * handle = positions)
				GetDefaultSamplePositions (This, (IntPtr)handle, count);
		}
	}
}
#endif