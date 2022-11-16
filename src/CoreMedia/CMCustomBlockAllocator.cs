// 
// CMCustomBlockAllocator.cs: Custom allocator for CMBlockBuffer apis 
//
// Authors:
//    Alex Soto (alex.soto@xamarin.com
// 
// Copyright 2015 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace CoreMedia {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#else
	[Watch (6, 0)]
#endif
	public class CMCustomBlockAllocator : IDisposable {

		GCHandle gch;

		public CMCustomBlockAllocator ()
		{
			gch = GCHandle.Alloc (this);
			// kCMBlockBufferCustomBlockSourceVersion = 0 <- this is the only and current value
			Cblock.Version = 0;
#if NET
			unsafe {
				Cblock.Allocate = &AllocateCallback;
				Cblock.Free = &FreeCallback;
			}
#else
			Cblock.Allocate = static_AllocateCallback;
			Cblock.Free = static_FreeCallback;
#endif
			Cblock.RefCon = GCHandle.ToIntPtr (gch);
		}

		// 1:1 mapping to the real underlying structure
		[StructLayout (LayoutKind.Sequential, Pack = 4)] // it's 28 bytes (not 32) on 64 bits iOS
		internal struct CMBlockBufferCustomBlockSource {
			public uint Version;
#if NET
			public unsafe delegate* unmanaged<IntPtr, nuint, IntPtr> Allocate;
			public unsafe delegate* unmanaged<IntPtr, IntPtr, nuint, void> Free;
#else
			public CMAllocateCallback Allocate;
			public CMFreeCallback Free;
#endif
			public IntPtr RefCon;
		}
		internal CMBlockBufferCustomBlockSource Cblock;

#if !NET
		internal delegate IntPtr CMAllocateCallback (/* void* */ IntPtr refCon, /* size_t */ nuint sizeInBytes);
		internal delegate void CMFreeCallback (/* void* */ IntPtr refCon, /* void* */ IntPtr doomedMemoryBlock, /* size_t */ nuint sizeInBytes);

		static CMAllocateCallback static_AllocateCallback = AllocateCallback;
		static CMFreeCallback static_FreeCallback = FreeCallback;
#endif

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (CMAllocateCallback))]
#endif
#endif
		static IntPtr AllocateCallback (IntPtr refCon, nuint sizeInBytes)
		{
			var gch = GCHandle.FromIntPtr (refCon);
			if (gch.Target is CMCustomBlockAllocator target)
				return target.Allocate (sizeInBytes);
			return IntPtr.Zero;
		}

		public virtual IntPtr Allocate (nuint sizeInBytes)
		{
			return Marshal.AllocHGlobal ((int) sizeInBytes);
		}

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (CMFreeCallback))]
#endif
#endif
		static void FreeCallback (IntPtr refCon, IntPtr doomedMemoryBlock, nuint sizeInBytes)
		{
			var gch = GCHandle.FromIntPtr (refCon);
			if (gch.Target is CMCustomBlockAllocator allocator)
				allocator.Free (doomedMemoryBlock, sizeInBytes);
		}

		public virtual void Free (IntPtr doomedMemoryBlock, nuint sizeInBytes)
		{
			Marshal.FreeHGlobal (doomedMemoryBlock);
		}

		~CMCustomBlockAllocator ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (gch.IsAllocated)
				gch.Free ();
		}
	}

	// This class is used internally by a couple of CMBlockBuffer methods
	// that take a managed array as input parameter
#if !NET
	[Watch (6, 0)]
#endif
	internal class CMManagedArrayBlockAllocator : CMCustomBlockAllocator {

		GCHandle dataHandle;
		public CMManagedArrayBlockAllocator (byte [] data)
		{
			dataHandle = GCHandle.Alloc (data, GCHandleType.Pinned); // This requires a pinned GCHandle, because unsafe code is scoped to the current block, and the address of the byte array will be used after this function returns.
		}

		public override IntPtr Allocate (nuint sizeInBytes)
		{
			return dataHandle.AddrOfPinnedObject ();
		}

		public override void Free (IntPtr doomedMemoryBlock, nuint sizeInBytes)
		{
			if (dataHandle.IsAllocated)
				dataHandle.Free ();
		}
	}
}
