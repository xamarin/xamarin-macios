using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using Metal;

namespace MetalPerformanceShaders {

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[Native] // NSUInteger
	[Flags] // NS_OPTIONS
	public enum MPSKernelOptions : ulong {
		None = 0,
		SkipApiValidation = 1 << 0,
		AllowReducedPrecision = 1 << 1,
		[MacCatalyst (13, 1)]
		DisableInternalTiling = 1 << 2,
		[MacCatalyst (13, 1)]
		InsertDebugGroups = 1 << 3,
		[MacCatalyst (13, 1)]
		Verbose = 1 << 4,
#if !NET
		[Obsolete ("Use 'AllowReducedPrecision' instead.")]
		MPSKernelOptionsAllowReducedPrecision = AllowReducedPrecision,
#endif
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[Native] // NSUInteger
	public enum MPSImageEdgeMode : ulong {
		Zero,
		Clamp = 1,
		[MacCatalyst (13, 1)]
		Mirror,
		[MacCatalyst (13, 1)]
		MirrorWithEdge,
		[MacCatalyst (13, 1)]
		Constant,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSAlphaType : ulong {
		NonPremultiplied = 0,
		AlphaIsOne = 1,
		Premultiplied = 2,
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	public enum MPSDataType : uint { // uint32_t
		Invalid = 0,

		FloatBit = 0x10000000,
		Float16 = FloatBit | 16,
		Float32 = FloatBit | 32,

		SignedBit = 0x20000000,
		Int8 = SignedBit | 8,
		Int16 = SignedBit | 16,
		Int32 = SignedBit | 32,
		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[MacCatalyst (14, 1)]
		Int64 = SignedBit | 64,

		UInt8 = 8,
		UInt16 = 16,
		UInt32 = 32,
		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[MacCatalyst (14, 1)]
		UInt64 = 64,

		[MacCatalyst (13, 1)]
		NormalizedBit = 0x40000000,
		[MacCatalyst (13, 1)]
		Unorm1 = NormalizedBit | 1,
		[MacCatalyst (13, 1)]
		Unorm8 = NormalizedBit | 8,
	}

	[Flags]
	[Native]
	[iOS (13, 0), TV (13, 0)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	public enum MPSAliasingStrategy : ulong {
		Default = 0x0,
		DontCare = Default,
		ShallAlias = 1uL << 0,
		ShallNotAlias = 1uL << 1,
		AliasingReserved = ShallAlias | ShallNotAlias,
		PreferTemporaryMemory = 1uL << 2,
		PreferNonTemporaryMemory = 1uL << 3,
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[Native]
	public enum MPSImageFeatureChannelFormat : ulong {
		Invalid = 0,
		Unorm8 = 1,
		Unorm16 = 2,
		Float16 = 3,
		Float32 = 4,
		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		Reserved0 = 5,

		//Count, // must always be last, and because of this it will cause breaking changes.
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	public enum MPSMatrixDecompositionStatus {
		Success = 0,
		Failure = -1,
		Singular = -2,
		NonPositiveDefinite = -3,
	}

	[iOS (13, 0), TV (13, 0)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[Flags]
	[Native]
	public enum MPSMatrixRandomDistribution : ulong {
		Default = 0x1,
		Uniform = 0x2,
		[iOS (14, 0), TV (14, 0), Mac (11, 0)]
		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		Normal = Default | Uniform,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSRnnSequenceDirection : ulong {
		Forward = 0,
		Backward,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSRnnBidirectionalCombineMode : ulong {
		None = 0,
		Add,
		Concatenate,
	}

	[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		Power,
		[MacCatalyst (13, 1)]
		Exponential,
		[MacCatalyst (13, 1)]
		Logarithm,
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		GeLU,
#if !NET
		[Obsolete ("The value changes when newer versions are released. It will be removed in the future.")]
		Count, // must always be last
#endif
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSCnnBinaryConvolutionFlags : ulong {
		None = 0,
		UseBetaScaling = 1 << 0,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSCnnBinaryConvolutionType : ulong {
		BinaryWeights = 0,
		Xnor,
		And,
	}

	[MacCatalyst (13, 1)]
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
		CustomWhitelistForNodeFusion = (1 << 13),
		Custom = (1 << 14),
		SizeMask = 2032,
		ExcludeEdges = (1 << 15),
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[Native]
	public enum MPSDataLayout : ulong {
		HeightPerWidthPerFeatureChannels = 0,
		FeatureChannelsPerHeightPerWidth = 1,
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[Native]
	public enum MPSStateResourceType : ulong {
		None = 0,
		Buffer = 1,
		Texture = 2,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSIntersectionType : ulong {
		Nearest = 0,
		Any = 1,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSTriangleIntersectionTestType : ulong {
		Default = 0,
		Watertight = 1,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSBoundingBoxIntersectionTestType : ulong {
		Default = 0,
		AxisAligned = 1,
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Fast = 2,
	}

	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	public enum MPSRayMaskOptions : ulong {
		None = 0,
		Primitive = 1,
		Instance = 2,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSRayDataType : ulong {
		OriginDirection = 0,
		OriginMinDistanceDirectionMaxDistance = 1,
		OriginMaskDirectionMaxDistance = 2,
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		PackedOriginDirection = 3,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSIntersectionDataType : ulong {
		Distance = 0,
		PrimitiveIndex = 1,
		PrimitiveIndexCoordinates = 2,
		PrimitiveIndexInstanceIndex = 3,
		PrimitiveIndexInstanceIndexCoordinates = 4,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSTransformType : ulong {
		Float4x4 = 0,
		Identity = 1,
	}

	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	public enum MPSAccelerationStructureUsage : ulong {
		None = 0,
		Refit = 1,
		FrequentRebuild = 2,
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		PreferGpuBuild = 4,
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		PreferCpuBuild = 8,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSAccelerationStructureStatus : ulong {
		Unbuilt = 0,
		Built = 1,
	}

	[MacCatalyst (13, 1)]
	public enum MPSCnnWeightsQuantizationType : uint {
		None = 0,
		Linear = 1,
		LookupTable = 2,
	}

	[Flags]
	[Native]
	[MacCatalyst (13, 1)]
	public enum MPSCnnConvolutionGradientOption : ulong {
		GradientWithData = 0x1,
		GradientWithWeightsAndBias = 0x2,
		All = GradientWithData | GradientWithWeightsAndBias,
	}

	[Flags]
	[Native]
	[MacCatalyst (13, 1)]
	public enum MPSNNComparisonType : ulong {
		Equal,
		NotEqual,
		Less,
		LessOrEqual,
		Greater,
		GreaterOrEqual,
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	public enum MPSCnnLossType : uint {
		MeanAbsoluteError = 0,
		MeanSquaredError,
		SoftMaxCrossEntropy,
		SigmoidCrossEntropy,
		CategoricalCrossEntropy,
		Hinge,
		Huber,
		CosineDistance,
		Log,
		KullbackLeiblerDivergence,
		//Count, // must always be last, and because of this it will cause breaking changes.
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	public enum MPSCnnReductionType {
		None = 0,
		Sum,
		Mean,
		SumByNonZeroWeights,
		//Count, // must always be last, and because of this it will cause breaking changes.
	}

	[Flags]
	[Native]
	[MacCatalyst (13, 1)]
	public enum MPSNNConvolutionAccumulatorPrecisionOption : ulong {
		Half = 0x0,
		Float = 1uL << 0,
	}

	[Flags]
	[Native]
	[MacCatalyst (13, 1)]
	public enum MPSCnnBatchNormalizationFlags : ulong {
		Default = 0x0,
		CalculateStatisticsAutomatic = Default,
		CalculateStatisticsAlways = 0x1,
		CalculateStatisticsNever = 0x2,
		CalculateStatisticsMask = 0x3,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum MPSNNRegularizationType : ulong {
		None = 0,
		L1 = 1,
		L2 = 2,
	}

	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	public enum MPSNNTrainingStyle : ulong {
		None = 0x0,
		Cpu = 0x1,
		Gpu = 0x2,
	}

	[Native]
	[MacCatalyst (13, 1)]
	public enum MPSRnnMatrixId : ulong {
		SingleGateInputWeights = 0,
		SingleGateRecurrentWeights,
		SingleGateBiasTerms,
		LstmInputGateInputWeights,
		LstmInputGateRecurrentWeights,
		LstmInputGateMemoryWeights,
		LstmInputGateBiasTerms,
		LstmForgetGateInputWeights,
		LstmForgetGateRecurrentWeights,
		LstmForgetGateMemoryWeights,
		LstmForgetGateBiasTerms,
		LstmMemoryGateInputWeights,
		LstmMemoryGateRecurrentWeights,
		LstmMemoryGateMemoryWeights,
		LstmMemoryGateBiasTerms,
		LstmOutputGateInputWeights,
		LstmOutputGateRecurrentWeights,
		LstmOutputGateMemoryWeights,
		LstmOutputGateBiasTerms,
		GruInputGateInputWeights,
		GruInputGateRecurrentWeights,
		GruInputGateBiasTerms,
		GruRecurrentGateInputWeights,
		GruRecurrentGateRecurrentWeights,
		GruRecurrentGateBiasTerms,
		GruOutputGateInputWeights,
		GruOutputGateRecurrentWeights,
		GruOutputGateInputGateWeights,
		GruOutputGateBiasTerms,
		//Count, // must always be last, and because of this it will cause breaking changes.
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	public enum MPSCustomKernelIndex : uint {
		DestIndex = 0,
		Src0Index = 0,
		Src1Index = 1,
		Src2Index = 2,
		Src3Index = 3,
		Src4Index = 4,
		UserDataIndex = 30,
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	public enum MPSImageType : uint {
		Type2d = 0,
		Type2dArray = 1,
		Array2d = 2,
		Array2dArray = 3,

		ArrayMask = 1,
		BatchMask = 2,
		TypeMask = 3,
		NoAlpha = 4,
		TexelFormatMask = 56,
		TexelFormatShift = 3,
		TexelFormatStandard = 0u << (int) TexelFormatShift,
		TexelFormatUnorm8 = 1u << (int) TexelFormatShift,
		TexelFormatFloat16 = 2u << (int) TexelFormatShift,
		TexelFormatBFloat16 = 3u << (int) TexelFormatShift,
		BitCount = 6,
		Mask = (1u << (int) BitCount) - 1,
		Type2dNoAlpha = Type2d | NoAlpha,
		Type2dArrayNoAlpha = Type2dArray | NoAlpha,
		Array2dNoAlpha = Type2d | BatchMask | NoAlpha,
		Array2dArrayNoAlpha = Type2dArray | BatchMask | NoAlpha,

		DestTextureType = (MPSConstants.FunctionConstantIndex >> (int) (0 * BitCount)) & Mask,
		Src0TextureType = (MPSConstants.FunctionConstantIndex >> (int) (0 * BitCount)) & Mask,
		Src1TextureType = (MPSConstants.FunctionConstantIndex >> (int) (1 * BitCount)) & Mask,
		Src2TextureType = (MPSConstants.FunctionConstantIndex >> (int) (2 * BitCount)) & Mask,
		Src3TextureType = (MPSConstants.FunctionConstantIndex >> (int) (3 * BitCount)) & Mask,
		Src4TextureType = (MPSConstants.FunctionConstantIndex >> (int) (4 * BitCount)) & Mask,
	}

	[Flags]
	[Native]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	public enum MPSDeviceOptions : ulong {
		Default = 0x0,
		LowPower = 0x1,
		SkipRemovable = 0x2,
	}
}
