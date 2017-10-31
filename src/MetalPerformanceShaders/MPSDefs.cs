using System;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

using Vector4 = global::OpenTK.Vector4;

namespace XamCore.MetalPerformanceShaders {

	[iOS (9,0)]
	[Native] // NSUInteger
	[Flags]	// NS_OPTIONS
	public enum MPSKernelOptions : nuint {
		None									= 0,
		SkipApiValidation						= 1 << 0,
		MPSKernelOptionsAllowReducedPrecision	= 1 << 1,
	}

	[iOS (9,0)]
	[Native] // NSUInteger
	public enum MPSImageEdgeMode : nuint {
		Zero,
		Clamp = 1
	}

	[iOS (10,0)][TV (10,0)]
	[Native]
	public enum MPSAlphaType : nuint {
		NonPremultiplied = 0,
		AlphaIsOne = 1,
		Premultiplied = 2,
	}
	 
	[iOS (10,0)][TV (10,0)]
	public enum MPSDataType : uint { // uint32_t
		FloatBit = 0x10000000,
		Float32 = FloatBit | 32,
	}

	[iOS (10,0)][TV (10,0)]
	[Native]
	public enum MPSImageFeatureChannelFormat : nuint {
		Invalid = 0,
		Unorm8 = 1,
		Unorm16 = 2,
		Float16 = 3,
		Float32 = 4,
	}

	// uses NSInteger
	public struct MPSOffset {
		public nint X;
		public nint Y;
		public nint Z;
	}

	// really use double, not CGFloat
	public struct MPSOrigin {
		public double X;
		public double Y;
		public double Z;
	}

	// really use double, not CGFloat
	public struct MPSSize {
		public double Width;
		public double Height;
		public double Depth;
	}

	public struct MPSRegion {
		public MPSOrigin Origin;
		public MPSSize Size;
	}

	// really use double, not CGFloat
	public struct MPSScaleTransform {
		public double ScaleX;
		public double ScaleY;
		public double TranslateX;
		public double TranslateY;
	}

	// MPSImageHistogram.h
	[StructLayout (LayoutKind.Explicit)]
	public struct MPSImageHistogramInfo {
		[FieldOffset (0)]
		public nuint NumberOfHistogramEntries;
#if ARCH_64
		[FieldOffset (8)]
#else
		[FieldOffset (4)]
#endif
		public bool HistogramForAlpha;
		[FieldOffset (16)]
		public Vector4 MinPixelValue;
		[FieldOffset (32)]
		public Vector4 MaxPixelValue;
	}

	// MPSTypes.h
	// FIXME: public delegate IMTLTexture MPSCopyAllocator (MPSKernel filter, IMTLCommandBuffer commandBuffer, IMTLTexture sourceTexture);
	public delegate NSObject MPSCopyAllocator (MPSKernel filter, NSObject commandBuffer, NSObject sourceTexture);
	// https://trello.com/c/GqtNId1C/517-generator-our-block-delegates-needs-to-use-wrapper-for-protocols
}