//
// API for the Metal framework
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2014-2015, Xamarin Inc.
//

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace Metal {
#if MONOMAC
	[Advice ("The 'NSString' argument will match a property of 'MTLDeviceNotificationHandler'.")]
	public delegate void MTLDeviceNotificationHandler (IMTLDevice device, NSString notifyName);
#endif

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#else
	[iOS (8, 0)]
	[Mac (10, 11)]
#endif
	public static partial class MTLDevice {
		[DllImport (Constants.MetalLibrary)]
		extern static IntPtr MTLCreateSystemDefaultDevice ();

		static IMTLDevice? system_default;

		public static IMTLDevice? SystemDefault {
			get {
				// Metal could be unavailable on the hardware (and we don't want to return an invalid instance)
				// also the instance could be disposed (by mistake) which would make the app unusable
				if ((system_default is null) || (system_default.Handle == IntPtr.Zero)) {
					try {
						var h = MTLCreateSystemDefaultDevice ();
						if (h != IntPtr.Zero)
							system_default = new MTLDeviceWrapper (h, false);
					} catch (EntryPointNotFoundException) {
					} catch (DllNotFoundException) {
					}
				}
				return system_default;
			}
		}

#if MONOMAC || __MACCATALYST__

#if NET
		[SupportedOSPlatform ("maccatalyst15.0")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[MacCatalyst (15,0)]
#endif
		[DllImport (Constants.MetalLibrary)]
		unsafe static extern IntPtr MTLCopyAllDevices ();

#if NET
		[SupportedOSPlatform ("maccatalyst15.0")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[MacCatalyst (15,0)]
#endif
		public static IMTLDevice [] GetAllDevices ()
		{
			var rv = MTLCopyAllDevices ();
			var devices = NSArray.ArrayFromHandle<IMTLDevice> (rv);
			NSObject.DangerousRelease (rv);
			return devices;
		}

#endif

#if MONOMAC

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 13)]
#endif
		[DllImport (Constants.MetalLibrary)]
		unsafe static extern IntPtr MTLCopyAllDevicesWithObserver (out IntPtr observer, BlockLiteral* handler);

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 13)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static IMTLDevice [] GetAllDevices (MTLDeviceNotificationHandler handler, out NSObject? observer)
		{
			IntPtr rv;
			IntPtr observer_handle;

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> trampoline = &TrampolineNotificationHandler;
				using var block = new BlockLiteral (trampoline, handler, typeof (MTLDevice), nameof (TrampolineNotificationHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_notificationHandler, handler);
#endif

				rv = MTLCopyAllDevicesWithObserver (out observer_handle, &block);
			}

			var obj = NSArray.ArrayFromHandle<IMTLDevice> (rv);
			NSObject.DangerousRelease (rv);

			observer = Runtime.GetNSObject (observer_handle);
			NSObject.DangerousRelease (observer_handle); // Apple's documentation says "The observer out parameter is returned with a +1 retain count [...]."

			return obj;
		}

#if !NET
		[Mac (10, 13)]
		[Obsolete ("Use the overload that takes an 'out NSObject' instead.")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static IMTLDevice [] GetAllDevices (ref NSObject? observer, MTLDeviceNotificationHandler handler)
		{
			var rv = GetAllDevices (handler, out var obs);
			observer = obs;
			return rv;
		}
#endif // !NET

#if !NET
		internal delegate void InnerNotification (IntPtr block, IntPtr device, IntPtr notifyName);
		static readonly InnerNotification static_notificationHandler = TrampolineNotificationHandler;
		[MonoPInvokeCallback (typeof (InnerNotification))]
#else
		[UnmanagedCallersOnly]
#endif
		public static unsafe void TrampolineNotificationHandler (IntPtr block, IntPtr device, IntPtr notifyName)
		{
			var descriptor = (BlockLiteral*) block;
			var del = (MTLDeviceNotificationHandler) (descriptor->Target);
			if (del is not null)
				del ((IMTLDevice) Runtime.GetNSObject (device)!, (Foundation.NSString) Runtime.GetNSObject (notifyName)!);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 13)]
#endif
		[DllImport (Constants.MetalLibrary)]
		static extern void MTLRemoveDeviceObserver (IntPtr observer);

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10, 13)]
		[NoiOS]
		[NoWatch]
		[NoTV]
#endif
		public static void RemoveObserver (NSObject observer)
		{
			if (observer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (observer));

			MTLRemoveDeviceObserver (observer.Handle);
		}
#endif
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static partial class MTLDevice_Extensions {
		public static IMTLBuffer? CreateBuffer<T> (this IMTLDevice This, T [] data, MTLResourceOptions options) where T : struct
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			var handle = GCHandle.Alloc (data, GCHandleType.Pinned); // This requires a pinned GCHandle, since it's not possible to use unsafe code to get the address of a generic object.
			try {
				IntPtr ptr = handle.AddrOfPinnedObject ();
				return This.CreateBuffer (ptr, (nuint) (data.Length * Marshal.SizeOf<T> ()), options);
			} finally {
				handle.Free ();
			}
		}

#if !NET
		[Obsolete ("Use the overload that takes an IntPtr instead. The 'data' parameter must be page-aligned and allocated using vm_allocate or mmap, which won't be the case for managed arrays, so this method will always fail.")]
		public static IMTLBuffer? CreateBufferNoCopy<T> (this IMTLDevice This, T [] data, MTLResourceOptions options, MTLDeallocator deallocator) where T : struct
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			var handle = GCHandle.Alloc (data, GCHandleType.Pinned); // This requires a pinned GCHandle, since it's not possible to use unsafe code to get the address of a generic object.
			IntPtr ptr = handle.AddrOfPinnedObject ();
			return This.CreateBufferNoCopy (ptr, (nuint) (data.Length * Marshal.SizeOf<T> ()), options, (pointer, length) => {
				handle.Free ();
				deallocator (pointer, length);
			});
		}
#endif

		public unsafe static void GetDefaultSamplePositions (this IMTLDevice This, MTLSamplePosition [] positions, nuint count)
		{
			if (positions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (positions));

			if (positions.Length < (nint) count)
				throw new ArgumentException ("Length of 'positions' cannot be less than 'count'.");
			fixed (void* handle = positions)
#if NET
				This.GetDefaultSamplePositions ((IntPtr) handle, count);
#else
				GetDefaultSamplePositions (This, (IntPtr) handle, count);
#endif
		}
#if IOS

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoMac]
		[NoTV]
		[iOS (13,0)]
#endif
		public static void ConvertSparseTileRegions (this IMTLDevice This, MTLRegion [] tileRegions, MTLRegion [] pixelRegions, MTLSize tileSize, nuint numRegions)
		{
			if (tileRegions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (tileRegions));
			if (pixelRegions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (pixelRegions));

			var tileRegionsHandle = GCHandle.Alloc (tileRegions, GCHandleType.Pinned);
			var pixelRegionsHandle = GCHandle.Alloc (pixelRegions, GCHandleType.Pinned);
			try {
				IntPtr tilePtr = tileRegionsHandle.AddrOfPinnedObject ();
				IntPtr pixelPtr = pixelRegionsHandle.AddrOfPinnedObject ();
				This.ConvertSparseTileRegions (tilePtr, pixelPtr, tileSize, numRegions);
			} finally {
				tileRegionsHandle.Free ();
				pixelRegionsHandle.Free ();
			}
		}

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoMac]
		[NoTV]
		[iOS (13,0)]
#endif
		public static void ConvertSparsePixelRegions (this IMTLDevice This, MTLRegion [] pixelRegions, MTLRegion [] tileRegions, MTLSize tileSize, MTLSparseTextureRegionAlignmentMode mode, nuint numRegions)
		{
			if (tileRegions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (tileRegions));
			if (pixelRegions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (pixelRegions));

			var tileRegionsHandle = GCHandle.Alloc (tileRegions, GCHandleType.Pinned);
			var pixelRegionsHandle = GCHandle.Alloc (pixelRegions, GCHandleType.Pinned);
			try {
				IntPtr tilePtr = tileRegionsHandle.AddrOfPinnedObject ();
				IntPtr pixelPtr = pixelRegionsHandle.AddrOfPinnedObject ();
				This.ConvertSparsePixelRegions (pixelPtr, tilePtr, tileSize, mode, numRegions);
			} finally {
				tileRegionsHandle.Free ();
				pixelRegionsHandle.Free ();
			}
		}
#endif

#if !NET
		[return: Release]
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		public unsafe static IMTLLibrary? CreateLibrary (this IMTLDevice This, global::CoreFoundation.DispatchData data, out NSError? error)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			var errorValue = NativeHandle.Zero;

			var ret = Runtime.GetINativeObject<IMTLLibrary> (global::ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_ref_IntPtr (This.Handle, Selector.GetHandle ("newLibraryWithData:error:"), data.Handle, &errorValue), true);
			error = Runtime.GetNSObject<NSError> (errorValue);
			return ret;
		}

		public static IMTLLibrary? CreateDefaultLibrary (this IMTLDevice This, NSBundle bundle, out NSError error)
		{
			return MTLDevice_Extensions.CreateLibrary (This, bundle, out error);
		}
#endif
	}
}
