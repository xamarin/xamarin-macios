using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using Metal;

namespace MetalPerformanceShadersGraph
{
	public enum MPSGraphOptions : ulong
	{
		None = 0,
		SynchronizeResults = 1,
		Verbose = 2,
		Default = SynchronizeResults
	}

	[Native]
	public enum MPSGraphTensorNamedDataLayout : ulong
	{
		Nchw = 0,
		Nhwc = 1,
		Oihw = 2,
		Hwio = 3,
		Chw = 4,
		Hwc = 5,
		Hw = 6
	}

	[Native]
	public enum MPSGraphPaddingStyle : ulong
	{
		Explicit = 0,
		TfValid = 1,
		TfSame = 2,
		ExplicitOffset = 3
	}

	[Native]
	public enum MPSGraphPaddingMode : long
	{
		Constant = 0,
		Reflect = 1,
		Symmetric = 2,
		ClampToEdge = 3,
		Zero = 4,
		Periodic = 5,
		AntiPeriodic = 6
	}

	[Native]
	public enum MPSGraphReductionMode : ulong
	{
		Min = 0,
		Max = 1,
		Sum = 2,
		Product = 3,
		ArgumentMin = 4,
		ArgumentMax = 5
	}

	public enum MPSGraphDeviceType : uint
	{
		MPSGraphDeviceTypeMetal = 0
	}
}
