using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using Metal;
using ObjCRuntime;

namespace MetalPerformanceShaders {
	public partial class MPSNNGraph {
		[Introduced (PlatformName.MacCatalyst, 13, 0)]
		[TV (13,0), Mac (10,15), iOS (13,0)]
		public unsafe static MPSNNGraph Create (IMTLDevice device, MPSNNImageNode[] resultImages, bool[] resultsAreNeeded)
		{
			fixed (void *resultsAreNeededHandle = resultsAreNeeded)
				return Create (device, resultImages, (IntPtr) resultsAreNeededHandle);
		}
	}
}
