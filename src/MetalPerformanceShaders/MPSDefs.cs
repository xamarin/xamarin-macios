#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

using Vector4 = global::OpenTK.Vector4;

namespace MetalPerformanceShaders {

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[Native] // NSUInteger
	[Flags]	// NS_OPTIONS
	public enum MPSKernelOptions : ulong {
		None									= 0,
		SkipApiValidation						= 1 << 0,
		MPSKernelOptionsAllowReducedPrecision	= 1 << 1,
		Verbose = 1 << 4,
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[Native] // NSUInteger
	public enum MPSImageEdgeMode : ulong {
		Zero,
		Clamp = 1
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[Native]
	public enum MPSAlphaType : ulong {
		NonPremultiplied = 0,
		AlphaIsOne = 1,
		Premultiplied = 2,
	}
	 
	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	public enum MPSDataType : uint { // uint32_t
		FloatBit = 0x10000000,
		Float16 = FloatBit | 16,
		Float32 = FloatBit | 32,
		NormalizedBit = 0x40000000,
		Unorm1 = NormalizedBit | 1,
		Unorm8 = NormalizedBit | 8,
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[Native]
	public enum MPSImageFeatureChannelFormat : ulong {
		Invalid = 0,
		Unorm8 = 1,
		Unorm16 = 2,
		Float16 = 3,
		Float32 = 4,
	}

	// uses NSInteger
	[Mac (10, 13, onlyOn64: true)]
	public struct MPSOffset {
		public nint X;
		public nint Y;
		public nint Z;
	}

	// really use double, not CGFloat
	[Mac (10, 13, onlyOn64: true)]
	public struct MPSOrigin {
		public double X;
		public double Y;
		public double Z;
	}

	// really use double, not CGFloat
	[Mac (10, 13, onlyOn64: true)]
	public struct MPSSize {
		public double Width;
		public double Height;
		public double Depth;
	}

	[Mac (10, 13, onlyOn64: true)]
	public struct MPSRegion {
		public MPSOrigin Origin;
		public MPSSize Size;
	}

	// really use double, not CGFloat
	[Mac (10, 13, onlyOn64: true)]
	public struct MPSScaleTransform {
		public double ScaleX;
		public double ScaleY;
		public double TranslateX;
		public double TranslateY;
	}

	// MPSImageHistogram.h
	[Mac (10, 13, onlyOn64: true)]
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

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	public enum MPSMatrixDecompositionStatus {
		Success = 0,
		Failure = -1,
		Singular = -2,
		NonPositiveDefinite = -3,
	}

	// MPSTypes.h
	// FIXME: public delegate IMTLTexture MPSCopyAllocator (MPSKernel filter, IMTLCommandBuffer commandBuffer, IMTLTexture sourceTexture);
	public delegate NSObject MPSCopyAllocator (MPSKernel filter, NSObject commandBuffer, NSObject sourceTexture);
	// https://trello.com/c/GqtNId1C/517-generator-our-block-delegates-needs-to-use-wrapper-for-protocols

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	[Native]
	public enum MPSRnnSequenceDirection : ulong {
		Forward = 0,
		Backward,
	}

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	[Native]
	public enum MPSRnnBidirectionalCombineMode : ulong {
		None = 0,
		Add,
		Concatenate,
	}

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	public enum MPSCnnNeuronType {
		None = 0,
		ReLU,
		Linear,
		Sigmoid,
		HardSigmoid,
		TanH,
		Absolute,
		SoftPlus,
		SoftSign,
		Elu,
		PReLU,
		ReLun,
		Count,
	}

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	[Native]
	public enum MPSCnnBinaryConvolutionFlags : ulong {
		None = 0,
		UseBetaScaling = 1 << 0,
	}

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	[Native]
	public enum MPSCnnBinaryConvolutionType : ulong {
		BinaryWeights = 0,
		Xnor,
		And,
	}

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	[Native]
	public enum MPSNNPaddingMethod : ulong {
		AlignCentered = 0,
		AlignTopLeft = 1,
		AlignBottomRight = 2,
		AlignReserved = 3,
		AddRemainderToTopLeft = 0 << 2,
		AddRemainderToTopRight = 1 << 2,
		AddRemainderToBottomLeft = 2 << 2,
		AddRemainderToBottomRight = 3 << 2,
		SizeValidOnly = 0,
		SizeSame = 1 << 4,
		SizeFull = 2 << 4,
		SizeReserved = 3 << 4,
		Custom = (1 << 14),
		SizeMask = 2032,
		ExcludeEdges = (1 << 15),
	}

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	[Native]
	public enum MPSDataLayout : ulong {
		HeightPerWidthPerFeatureChannels = 0,
		FeatureChannelsPerHeightPerWidth = 1,
	}

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	public struct MPSMatrixCopyOffsets {
		public uint SourceRowOffset;
		public uint SourceColumnOffset;
		public uint DestinationRowOffset;
		public uint DestinationColumnOffset;
	}

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	public struct MPSImageReadWriteParams {
		public nuint FeatureChannelOffset;
		public nuint NumberOfFeatureChannelsToReadWrite;
	}

	[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
	public struct MPSImageKeypointRangeInfo {
		public nuint MaximumKeypoints;
		public float MinimumThresholdValue;
	}
}
#endif