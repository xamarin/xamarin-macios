using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using Metal;

namespace MetalPerformanceShadersGraph {

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	public enum MPSGraphOptions : ulong {
		None = 0,
		SynchronizeResults = 1,
		Verbose = 2,
		Default = SynchronizeResults
	}
}
