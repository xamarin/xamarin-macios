//
// API for the Metal framework
//
// Authors:
//   Manuel de la Pena <mandel@microsoft.com>
//
#if IOS
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace Metal {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
#endif
	public static partial class MTLResourceStateCommandEncoder_Extensions {

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoMac]
		[NoTV]
#endif
		public static void Update (this IMTLResourceStateCommandEncoder This, IMTLTexture texture, MTLSparseTextureMappingMode mode, MTLRegion[] regions, nuint[] mipLevels, nuint[] slices)
		{
			if (texture is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (texture));
			if (regions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (regions));
			if (mipLevels is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mipLevels));
			if (slices is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (slices));

			var regionsHandle = GCHandle.Alloc (regions, GCHandleType.Pinned); 
			var mipLevelsHandle = GCHandle.Alloc (mipLevels, GCHandleType.Pinned); 
			var slicesHandle = GCHandle.Alloc (slices, GCHandleType.Pinned); 
			try {
				var regionsPtr = regionsHandle.AddrOfPinnedObject ();
				var mipLevelsPtr = mipLevelsHandle.AddrOfPinnedObject ();
				var slicesPtr = slicesHandle.AddrOfPinnedObject ();
				This.Update (texture, mode, regionsPtr, mipLevelsPtr, slicesPtr, (nuint)regions.Length);
			} finally {
				regionsHandle.Free ();
				mipLevelsHandle.Free ();
				slicesHandle.Free ();
			}
		}
	}
}
#endif
