using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using Metal;

using Vector4 = global::OpenTK.Vector4;
using OpenTK;

namespace MetalPerformanceShaders {

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[iOS (9,0)]
	[Mac (10, 13)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
#endif
	[Native] // NSUInteger
	[Flags] // NS_OPTIONS
	public enum MPSKernelOptions : ulong {
		None = 0,
		SkipApiValidation = 1 << 0,
		AllowReducedPrecision = 1 << 1,
#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (10,0)]
		[TV (10,0)]
#endif
		DisableInternalTiling = 1 << 2,
#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (10,0)]
		[TV (10,0)]
#endif
		InsertDebugGroups = 1 << 3,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (11,0)]
		[TV (11,0)]
#endif
		Verbose = 1 << 4,
#if !NET
		[Obsolete ("Use 'AllowReducedPrecision' instead.")]
		MPSKernelOptionsAllowReducedPrecision = AllowReducedPrecision,
#endif
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[iOS (9,0)]
	[Mac (10, 13)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
#endif
	[Native] // NSUInteger
	public enum MPSImageEdgeMode : ulong {
		Zero,
		Clamp = 1,
#if NET
		[SupportedOSPlatform ("ios12.1")]
		[SupportedOSPlatform ("tvos12.1")]
		[SupportedOSPlatform ("macos10.14.1")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (12,1)]
		[TV (12,1)]
		[Mac (10,14,1)]
#endif
		Mirror,
#if NET
		[SupportedOSPlatform ("ios12.1")]
		[SupportedOSPlatform ("tvos12.1")]
		[SupportedOSPlatform ("macos10.14.1")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (12,1)]
		[TV (12,1)]
		[Mac (10,14,1)]
#endif
		MirrorWithEdge,
#if NET
		[SupportedOSPlatform ("ios12.1")]
		[SupportedOSPlatform ("tvos12.1")]
		[SupportedOSPlatform ("macos10.14.1")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (12,1)]
		[TV (12,1)]
		[Mac (10,14,1)]
#endif
		Constant,
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.13")]
#else
	[iOS (10,0)]
	[TV (10,0)]
	[Mac (10, 13)]
#endif
	[Native]
	public enum MPSAlphaType : ulong {
		NonPremultiplied = 0,
		AlphaIsOne = 1,
		Premultiplied = 2,
	}
	 
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[iOS (10,0)]
	[TV (10,0)]
	[Mac (10, 13)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
#endif
	public enum MPSDataType : uint { // uint32_t
		Invalid = 0,

		FloatBit = 0x10000000,
		Float16 = FloatBit | 16,
		Float32 = FloatBit | 32,

		SignedBit = 0x20000000,
		Int8 = SignedBit | 8,
		Int16 = SignedBit | 16,
		Int32 = SignedBit | 32,
#if NET
		[SupportedOSPlatform ("ios14.1")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (14,1)]
		[TV (14,2)]
		[Mac (11,0)]
#endif
		Int64 = SignedBit | 64,

		UInt8 = 8,
		UInt16 = 16,
		UInt32 = 32,
#if NET
		[SupportedOSPlatform ("ios14.1")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (14,1)]
		[TV (14,2)]
		[Mac (11,0)]
#endif
		UInt64 = 64,

#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (11,0)]
		[TV (11,0)]
#endif
		NormalizedBit = 0x40000000,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (11,0)]
		[TV (11,0)]
#endif
		Unorm1 = NormalizedBit | 1,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (11,0)]
		[TV (11,0)]
#endif
		Unorm8 = NormalizedBit | 8,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
#endif
	[Flags]
	[Native]
	public enum MPSAliasingStrategy : ulong
	{
		Default = 0x0,
		DontCare = Default,
		ShallAlias = 1uL << 0,
		ShallNotAlias = 1uL << 1,
		AliasingReserved = ShallAlias | ShallNotAlias,
		PreferTemporaryMemory = 1uL << 2,
		PreferNonTemporaryMemory = 1uL << 3,
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[iOS (10,0)]
	[TV (10,0)]
	[Mac (10,13)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
#endif
	[Native]
	public enum MPSImageFeatureChannelFormat : ulong {
		Invalid = 0,
		Unorm8 = 1,
		Unorm16 = 2,
		Float16 = 3,
		Float32 = 4,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("maccatalyst13.0")]
#else
		[iOS (13,0)]
		[TV (13,0)]
		[Mac (10,15)]
#endif
		Reserved0 = 5,

		//[iOS (12,0), TV (12,0), Mac (10,14)]
		//Count, // must always be last, and because of this it will cause breaking changes.
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[TV (11, 0)]
	[Mac (10, 13)]
	[iOS (11, 0)]
#endif
	public enum MPSMatrixDecompositionStatus {
		Success = 0,
		Failure = -1,
		Singular = -2,
		NonPositiveDefinite = -3,
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
#endif
	[Flags]
	[Native]
	public enum MPSMatrixRandomDistribution : ulong
	{
		Default = 0x1,
		Uniform = 0x2,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,0)]
		[TV (14,0)]
		[Mac (11,0)]
		[Introduced (PlatformName.MacCatalyst, 14,0)]
#endif
		Normal = Default | Uniform,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[TV (11, 0)]
	[Mac (10, 13)]
	[iOS (11, 0)]
#endif
	[Native]
	public enum MPSRnnSequenceDirection : ulong {
		Forward = 0,
		Backward,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[TV (11, 0)]
	[Mac (10, 13)]
	[iOS (11, 0)]
#endif
	[Native]
	public enum MPSRnnBidirectionalCombineMode : ulong {
		None = 0,
		Add,
		Concatenate,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[TV (11, 0)]
	[Mac (10, 13)]
	[iOS (11, 0)]
#endif
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
#if NET
		[SupportedOSPlatform ("tvos11.3")]
		[SupportedOSPlatform ("macos10.13.4")]
		[SupportedOSPlatform ("ios11.3")]
#else
		[TV (11,3)]
		[Mac (10,13,4)]
		[iOS (11,3)]
#endif
		Power,
#if NET
		[SupportedOSPlatform ("tvos11.3")]
		[SupportedOSPlatform ("macos10.13.4")]
		[SupportedOSPlatform ("ios11.3")]
#else
		[TV (11,3)]
		[Mac (10,13,4)]
		[iOS (11,3)]
#endif
		Exponential,
#if NET
		[SupportedOSPlatform ("tvos11.3")]
		[SupportedOSPlatform ("macos10.13.4")]
		[SupportedOSPlatform ("ios11.3")]
#else
		[TV (11,3)]
		[Mac (10,13,4)]
		[iOS (11,3)]
#endif
		Logarithm,
#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		GeLU,
		[Obsolete ("The value changes when newer versions are released. It will be removed in the future.")]
		Count, // must always be last
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[TV (11, 0)]
	[Mac (10, 13)]
	[iOS (11, 0)]
#endif
	[Native]
	public enum MPSCnnBinaryConvolutionFlags : ulong {
		None = 0,
		UseBetaScaling = 1 << 0,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[TV (11, 0)]
	[Mac (10, 13)]
	[iOS (11, 0)]
#endif
	[Native]
	public enum MPSCnnBinaryConvolutionType : ulong {
		BinaryWeights = 0,
		Xnor,
		And,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[TV (11, 0)]
	[Mac (10, 13)]
	[iOS (11, 0)]
#endif
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

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[TV (11, 0)]
	[Mac (10, 13)]
	[iOS (11, 0)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
#endif
	[Native]
	public enum MPSDataLayout : ulong {
		HeightPerWidthPerFeatureChannels = 0,
		FeatureChannelsPerHeightPerWidth = 1,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("ios11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
#else
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[TV (11, 3)]
	[iOS (11, 3)]
	[Mac (10, 13, 4)]
#endif
	[Native]
	public enum MPSStateResourceType : ulong {
		None = 0,
		Buffer = 1,
		Texture = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12, 0)]
	[Mac (10, 14)]
	[iOS (12, 0)]
#endif
	[Native]
	public enum MPSIntersectionType : ulong {
		Nearest = 0,
		Any = 1,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12, 0)]
	[Mac (10, 14)]
	[iOS (12, 0)]
#endif
	[Native]
	public enum MPSTriangleIntersectionTestType : ulong {
		Default = 0,
		Watertight = 1,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12, 0)]
	[Mac (10, 14)]
	[iOS (12, 0)]
#endif
	[Native]
	public enum MPSBoundingBoxIntersectionTestType : ulong {
		Default = 0,
		AxisAligned = 1,
#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		Fast = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12, 0)]
	[Mac (10, 14)]
	[iOS (12, 0)]
#endif
	[Flags]
	[Native]
	public enum MPSRayMaskOptions : ulong {
		None = 0,
		Primitive = 1,
		Instance = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12, 0)]
	[Mac (10, 14)]
	[iOS (12, 0)]
#endif
	[Native]
	public enum MPSRayDataType : ulong {
		OriginDirection = 0,
		OriginMinDistanceDirectionMaxDistance = 1,
		OriginMaskDirectionMaxDistance = 2,
#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		PackedOriginDirection = 3,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12, 0)]
	[Mac (10, 14)]
	[iOS (12, 0)]
#endif
	[Native]
	public enum MPSIntersectionDataType : ulong {
		Distance = 0,
		PrimitiveIndex = 1,
		PrimitiveIndexCoordinates = 2,
		PrimitiveIndexInstanceIndex = 3,
		PrimitiveIndexInstanceIndexCoordinates = 4,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12, 0)]
	[Mac (10, 14)]
	[iOS (12, 0)]
#endif
	[Native]
	public enum MPSTransformType : ulong {
		Float4x4 = 0,
		Identity = 1,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12, 0)]
	[Mac (10, 14)]
	[iOS (12, 0)]
#endif
	[Flags]
	[Native]
	public enum MPSAccelerationStructureUsage : ulong {
		None = 0,
		Refit = 1,
		FrequentRebuild = 2,
#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		PreferGpuBuild = 4,
#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		PreferCpuBuild = 8,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12, 0)]
	[Mac (10, 14)]
	[iOS (12, 0)]
#endif
	[Native]
	public enum MPSAccelerationStructureStatus : ulong {
		Unbuilt = 0,
		Built = 1,
	}

#if NET
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
	[SupportedOSPlatform ("ios11.3")]
#else
	[TV (11, 3)]
	[Mac (10, 13, 4)]
	[iOS (11, 3)]
#endif
	public enum MPSCnnWeightsQuantizationType : uint {
		None = 0,
		Linear = 1,
		LookupTable = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
	[SupportedOSPlatform ("ios11.3")]
#else
	[TV (11, 3)]
	[Mac (10, 13, 4)]
	[iOS (11, 3)]
#endif
	[Flags]
	[Native]
	public enum MPSCnnConvolutionGradientOption : ulong {
		GradientWithData = 0x1,
		GradientWithWeightsAndBias = 0x2,
		All = GradientWithData | GradientWithWeightsAndBias,
	}

#if NET
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
	[SupportedOSPlatform ("ios11.3")]
#else
	[TV (11, 3)]
	[Mac (10, 13, 4)]
	[iOS (11, 3)]
#endif
	[Flags]
	[Native]
	public enum MPSNNComparisonType : ulong {
		Equal,
		NotEqual,
		Less,
		LessOrEqual,
		Greater,
		GreaterOrEqual,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
	[SupportedOSPlatform ("ios11.3")]
#else
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[TV (11, 3)]
	[Mac (10, 13, 4)]
	[iOS (11, 3)]
#endif
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

#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
	[SupportedOSPlatform ("ios11.3")]
#else
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[TV (11, 3)]
	[Mac (10, 13, 4)]
	[iOS (11, 3)]
#endif
	public enum MPSCnnReductionType {
		None = 0,
		Sum,
		Mean,
		SumByNonZeroWeights,
		//Count, // must always be last, and because of this it will cause breaking changes.
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
#endif
	[Flags]
	[Native]
	public enum MPSNNConvolutionAccumulatorPrecisionOption : ulong {
		Half = 0x0,
		Float = 1uL << 0,
	}

#if NET
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
	[SupportedOSPlatform ("ios11.3")]
#else
	[TV (11, 3)]
	[Mac (10, 13, 4)]
	[iOS (11, 3)]
#endif
	[Flags]
	[Native]
	public enum MPSCnnBatchNormalizationFlags : ulong {
		Default = 0x0,
		CalculateStatisticsAutomatic = Default,
		CalculateStatisticsAlways = 0x1,
		CalculateStatisticsNever = 0x2,
		CalculateStatisticsMask = 0x3,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12, 0)]
	[Mac (10, 14)]
	[iOS (12, 0)]
#endif
	[Native]
	public enum MPSNNRegularizationType : ulong {
		None = 0,
		L1 = 1,
		L2 = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
	[SupportedOSPlatform ("ios11.3")]
#else
	[TV (11, 3)]
	[Mac (10, 13, 4)]
	[iOS (11, 3)]
#endif
	[Flags]
	[Native]
	public enum MPSNNTrainingStyle : ulong {
		None = 0x0,
		Cpu = 0x1,
		Gpu = 0x2,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
#endif
	[Native]
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

#if NET
	[SupportedOSPlatform ("ios11.3")]
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
	[SupportedOSPlatform ("maccatalyst13.0")]
#else
	[iOS (11,3)]
	[TV (11,3)]
	[Mac (10,13,4)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
#endif
	public enum MPSCustomKernelIndex : uint
	{
		DestIndex = 0,
		Src0Index = 0,
		Src1Index = 1,
		Src2Index = 2,
		Src3Index = 3,
		Src4Index = 4,
		UserDataIndex = 30,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
	[SupportedOSPlatform ("tvos11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
	[SupportedOSPlatform ("ios11.3")]
#else
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[TV (11,3)]
	[Mac (10,13,4)]
	[iOS (11,3)]
#endif
	public enum MPSImageType : uint
	{
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
		TexelFormatStandard = 0u << (int)TexelFormatShift,
		TexelFormatUnorm8 = 1u << (int)TexelFormatShift,
		TexelFormatFloat16 = 2u << (int)TexelFormatShift,
		TexelFormatBFloat16 = 3u << (int)TexelFormatShift,
		BitCount = 6,
		Mask = (1u << (int)BitCount) - 1,
		Type2dNoAlpha = Type2d | NoAlpha,
		Type2dArrayNoAlpha = Type2dArray | NoAlpha,
		Array2dNoAlpha = Type2d | BatchMask | NoAlpha,
		Array2dArrayNoAlpha = Type2dArray | BatchMask | NoAlpha,

		DestTextureType = (MPSConstants.FunctionConstantIndex >> (int)(0*BitCount)) & Mask,
		Src0TextureType = (MPSConstants.FunctionConstantIndex >> (int)(0*BitCount)) & Mask,
		Src1TextureType = (MPSConstants.FunctionConstantIndex >> (int)(1*BitCount)) & Mask,
		Src2TextureType = (MPSConstants.FunctionConstantIndex >> (int)(2*BitCount)) & Mask,
		Src3TextureType = (MPSConstants.FunctionConstantIndex >> (int)(3*BitCount)) & Mask,
		Src4TextureType = (MPSConstants.FunctionConstantIndex >> (int)(4*BitCount)) & Mask,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
	[SupportedOSPlatform ("tvos12.2")]
	[SupportedOSPlatform ("macos10.14.4")]
	[SupportedOSPlatform ("ios12.2")]
#else
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[TV (12,2)]
	[Mac (10,14,4)]
	[iOS (12,2)]
#endif
	[Flags]
	[Native]
	public enum MPSDeviceOptions : ulong
	{
		Default = 0x0,
		LowPower = 0x1,
		SkipRemovable = 0x2,
	}
}
