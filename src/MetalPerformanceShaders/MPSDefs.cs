#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using Metal;

using Vector4 = global::OpenTK.Vector4;
using OpenTK;

namespace MetalPerformanceShaders {

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[Native] // NSUInteger
	[Flags]	// NS_OPTIONS
	public enum MPSKernelOptions : ulong {
		None									= 0,
		SkipApiValidation						= 1 << 0,
		AllowReducedPrecision = 1 << 1,
		[iOS (10,0), TV(10,0)]
		DisableInternalTiling = 1 << 2,
		[iOS (10,0), TV (10,0)]
		InsertDebugGroups = 1 << 3,
		[iOS (11,0), TV (11,0)]
		Verbose = 1 << 4,
#if !XAMCORE_4_0
		[Obsolete ("Use 'AllowReducedPrecision' instead.")]
		MPSKernelOptionsAllowReducedPrecision = AllowReducedPrecision,
#endif
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[Native] // NSUInteger
	public enum MPSImageEdgeMode : ulong {
		Zero,
		Clamp = 1,
		[iOS (12,1), TV (12,1), Mac (10,14,1, onlyOn64: true)]
		Mirror,
		[iOS (12,1), TV (12,1), Mac (10,14,1, onlyOn64: true)]
		MirrorWithEdge,
		[iOS (12,1), TV (12,1), Mac (10,14,1, onlyOn64: true)]
		Constant,
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
		Invalid = 0,

		FloatBit = 0x10000000,
		Float16 = FloatBit | 16,
		Float32 = FloatBit | 32,

		SignedBit = 0x20000000,
		Int8 = SignedBit | 8,
		Int16 = SignedBit | 16,

		UInt8 = 8,
		UInt16 = 16,
		UInt32 = 32,

		[iOS (11,0), TV (11,0)]
		NormalizedBit = 0x40000000,
		[iOS (11,0), TV (11,0)]
		Unorm1 = NormalizedBit | 1,
		[iOS (11,0), TV (11,0)]
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
		[iOS (12,0), TV (12,0), Mac (10,14, onlyOn64: true)]
		Count,
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

	[iOS (11,3), TV (11,3), Mac (10,13,4, onlyOn64: true)]
	public struct MPSImageCoordinate {
		public nuint X;
		public nuint Y;
		public nuint Channel;
	}

	[iOS (11,3), TV (11,3), Mac (10,13,4, onlyOn64: true)]
	public struct MPSImageRegion {
		public MPSImageCoordinate Offset;
		public MPSImageCoordinate Size;
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

	[TV (11, 3), iOS (11, 3), Mac (10, 13, 4, onlyOn64: true)]
	public struct MPSStateTextureInfo {
		public nuint Width;
		public nuint Height;
		public nuint Depth;
		public nuint ArrayLength;

#pragma warning disable 0169 // Avoid warning when building core.dll and the unused reserved fields
		nuint _PixelFormat;
		nuint _TextureType;
		nuint _TextureUsage;

		//NSUInteger _reserved [4];
		nuint Reserved0;
		nuint Reserved1;
		nuint Reserved2;
		nuint Reserved3;
#pragma warning restore 0169
#if !COREBUILD
		public MTLPixelFormat PixelFormat {
			get => (MTLPixelFormat) (ulong) _PixelFormat;
			set => _PixelFormat = (nuint) (ulong) value;
		}

		public MTLTextureType TextureType {
			get => (MTLTextureType) (ulong) _TextureType;
			set => _TextureType = (nuint) (ulong) value;
		}

		public MTLTextureUsage TextureUsage {
			get => (MTLTextureUsage) (ulong) _TextureUsage;
			set => _TextureUsage = (nuint) (ulong) value;
		}
#endif
	}
	[TV (11, 3), iOS (11, 3), Mac (10, 13, 4, onlyOn64: true)]
	[Native]
	public enum MPSStateResourceType : ulong {
		None = 0,
		Buffer = 1,
		Texture = 2,
	}

	[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
	[Native]
	public enum MPSIntersectionType : ulong {
		Nearest = 0,
		Any = 1,
	}

	[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
	[Native]
	public enum MPSTriangleIntersectionTestType : ulong {
		Default = 0,
		Watertight = 1,
	}

	[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
	[Native]
	public enum MPSBoundingBoxIntersectionTestType : ulong {
		Default = 0,
		AxisAligned = 1,
	}

	[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
	[Flags]
	[Native]
	public enum MPSRayMaskOptions : ulong {
		None = 0,
		Primitive = 1,
		Instance = 2,
	}

	[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
	[Native]
	public enum MPSRayDataType : ulong {
		OriginDirection = 0,
		OriginMinDistanceDirectionMaxDistance = 1,
		OriginMaskDirectionMaxDistance = 2,
	}

	[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
	[Native]
	public enum MPSIntersectionDataType : ulong {
		Distance = 0,
		PrimitiveIndex = 1,
		PrimitiveIndexCoordinates = 2,
		PrimitiveIndexInstanceIndex = 3,
		PrimitiveIndexInstanceIndexCoordinates = 4,
	}

	[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
	[Native]
	public enum MPSTransformType : ulong {
		Float4x4 = 0,
		Identity = 1,
	}

	[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
	[Flags]
	[Native]
	public enum MPSAccelerationStructureUsage : ulong {
		None = 0,
		Refit = 1,
		FrequentRebuild = 2,
	}

	[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
	[Native]
	public enum MPSAccelerationStructureStatus : ulong {
		Unbuilt = 0,
		Built = 1,
	}

	[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
	[StructLayout (LayoutKind.Sequential)]
	public struct MPSAxisAlignedBoundingBox {
		public Vector3 Min;
		public Vector3 Max;
	}

	[Flags]
	[Native]
	[TV (12, 2), Mac (10, 14, 4, onlyOn64: true), iOS (12, 2)]
	public enum MPSDeviceOptions : ulong {
		Default = 0,
		LowPower = 1,
		SkipRemovable = 2,
	}
}
#endif