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

namespace Metal {

	public static partial class MTLResourceStateCommandEncoder_Extensions {

		[NoMac, NoTV, iOS (13,0)]
		public static void Update (this IMTLResourceStateCommandEncoder This, IMTLTexture texture, MTLSparseTextureMappingMode mode, MTLRegion[] regions, nuint[] mipLevels, nuint[] slices, nuint numRegions)
		{
			var regionsHandle = GCHandle.Alloc (regions, GCHandleType.Pinned); 
			var mipLevelsHandle = GCHandle.Alloc (mipLevels, GCHandleType.Pinned); 
			var slicesHandle = GCHandle.Alloc (slices, GCHandleType.Pinned); 
			try {
				var regionsPtr = regionsHandle.AddrOfPinnedObject ();
				var mipLevelsPtr = mipLevelsHandle.AddrOfPinnedObject ();
				var slicesPtr = slicesHandle.AddrOfPinnedObject ();
				This.Update (texture, mode, regionsPtr, mipLevelsPtr, slicesPtr, numRegions);
			} finally {
				regionsHandle.Free ();
				mipLevelsHandle.Free ();
				slicesHandle.Free ();
			}
		}
	}
}
#endif