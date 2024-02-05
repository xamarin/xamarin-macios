using System;
using Foundation;
using ObjCRuntime;
using CoreMedia;
using Network;

namespace GHIssue6994 {


	public struct MTLRegion {
		public nint Origin;
		public nint Size;

		public MTLRegion (nint origin, nint size)
		{
			Origin = origin;
			Size = size;
		}
	}

	public enum MTLSparseTextureRegionAlignmentMode {
		Horizontal,
		Vertical,
	}

	[BaseType (typeof (NSObject))]
	interface Foo {
		[Export ("convertSparsePixelRegions:toTileRegions:alignmentMode:numRegions:")]
		void ConvertSparsePixelRegions (MTLRegion [] pixelRegions, MTLRegion [] tileRegions, MTLSparseTextureRegionAlignmentMode mode, nuint numRegions);

		[Export ("addProxies:")]
		void AddProxies (IntPtr [] proxies);

		[return: BindAs (typeof (int[]))]
		[Export ("getProxies")]
		NSArray<NSNumber> GetProxies ();

		[return: BindAs (typeof (NWProxyConfig[]))]
		[Export ("getConfigs")]
		NSArray<NWProxyConfig> GetConfigs ();

		[Export ("initWithRegion:")]
		IntPtr Constructor (MTLRegion [] regions);
	}
}
