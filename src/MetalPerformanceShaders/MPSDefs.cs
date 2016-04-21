using System;
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
	public struct MPSImageHistogramInfo {
		public nuint NumberOfHistogramEntries;
		public bool HistogramForAlpha;
		public Vector4 MinPixelValue;
		public Vector4 MaxPixelValue;
	}

	// MPSTypes.h
	// FIXME: public delegate IMTLTexture MPSCopyAllocator (MPSKernel filter, IMTLCommandBuffer commandBuffer, IMTLTexture sourceTexture);
	public delegate NSObject MPSCopyAllocator (MPSKernel filter, NSObject commandBuffer, NSObject sourceTexture);
	// https://trello.com/c/GqtNId1C/517-generator-our-block-delegates-needs-to-use-wrapper-for-protocols
}