using System;
using System.Runtime.InteropServices;

using Foundation;
using Metal;
using ObjCRuntime;

namespace MLCompute {

	// compilation helper for async custom types
	interface MLCExecutionResult { }

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCActivationType {
		None = 0,
		ReLU = 1,
		Linear = 2,
		Sigmoid = 3,
		HardSigmoid = 4,
		Tanh = 5,
		Absolute = 6,
		SoftPlus = 7,
		SoftSign = 8,
		Elu = 9,
		ReLun = 10,
		LogSigmoid = 11,
		Selu = 12,
		Celu = 13,
		HardShrink = 14,
		SoftShrink = 15,
		TanhShrink = 16,
		Threshold = 17,
		Gelu = 18,
		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		HardSwish = 19,
		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		Clamp = 20,
		// Count, // must be last, not available in swift
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCArithmeticOperation {
		Add = 0,
		Subtract = 1,
		Multiply = 2,
		Divide = 3,
		Floor = 4,
		Round = 5,
		Ceil = 6,
		Sqrt = 7,
		Rsqrt = 8,
		Sin = 9,
		Cos = 10,
		Tan = 11,
		Asin = 12,
		Acos = 13,
		Atan = 14,
		Sinh = 15,
		Cosh = 16,
		Tanh = 17,
		Asinh = 18,
		Acosh = 19,
		Atanh = 20,
		Pow = 21,
		Exp = 22,
		Exp2 = 23,
		Log = 24,
		Log2 = 25,
		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		MultiplyNoNaN = 26,
		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		DivideNoNaN = 27,
		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		Min = 28,
		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		Max = 29,
		// Count, // must be last, not available in swift
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCConvolutionType {
		Standard = 0,
		Transposed = 1,
		Depthwise = 2,
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCDataType {
		Invalid = 0,
		Float32 = 1,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		Float16 = 3,
		Boolean = 4,
		Int64 = 5,
		Inot32 = 7,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		Int8 = 8,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		UInt8 = 9,
		// Count, // must be last, not available in swift
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCDeviceType /* int32_t */ {
		Cpu = 0,
		Gpu = 1,
		Any = 2,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		Ane = 3, // Apple neural engine
				 // Count, // must be last, not available in swift
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[Flags]
	enum MLCExecutionOptions : ulong {
		None = 0x0,
		SkipWritingInputDataToDevice = 0x1,
		Synchronous = 0x2,
		Profiling = 0x4,
		ForwardForInference = 0x8,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		PerLayerProfiling = 0x10,
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[Flags]
	enum MLCGraphCompilationOptions : ulong {
		None = 0x0,
		DebugLayers = 0x1,
		DisableLayerFusion = 0x2,
		LinkGraphs = 0x4,
		ComputeAllGradients = 0x8,
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCLossType {
		MeanAbsoluteError = 0,
		MeanSquaredError = 1,
		SoftmaxCrossEntropy = 2,
		SigmoidCrossEntropy = 3,
		CategoricalCrossEntropy = 4,
		Hinge = 5,
		Huber = 6,
		CosineDistance = 7,
		Log = 8,
		// Count, // must be last, not available in swift
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCLstmResultMode : ulong /* uint64_t */ {
		Output = 0,
		OutputAndStates = 1,
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCPaddingPolicy {
		Same = 0,
		Valid = 1,
		UsePaddingSize = 2,
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCPaddingType {
		Zero = 0,
		Reflect = 1,
		Symmetric = 2,
		Constant = 3,
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCPoolingType {
		Max = 1,
		Average = 2,
		L2Norm = 3,
		// Count, // must be last, not available in swift
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCRandomInitializerType {
		Invalid = 0,
		Uniform = 1,
		GlorotUniform = 2,
		Xavier = 3,
		// Count, // must be last, not available in swift
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCReductionType {
		None = 0,
		Sum = 1,
		Mean = 2,
		Max = 3,
		Min = 4,
		ArgMax = 5,
		ArgMin = 6,
		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		L1Norm = 7,
		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		Any = 8,
		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		All = 9,
		// Count, // must be last, not available in swift
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCRegularizationType {
		None = 0,
		L1 = 1,
		L2 = 2,
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCSampleMode {
		Nearest = 0,
		Linear = 1,
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	enum MLCSoftmaxOperation {
		Softmax = 0,
		LogSoftmax = 1,
	}

	[iOS (15, 0), TV (15, 0), NoWatch, MacCatalyst (15, 0)]
	public enum MLCGradientClippingType {
		Value = 0,
		Norm = 1,
		GlobalNorm = 2,
	}


	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCActivationDescriptor : NSCopying {

		[Export ("activationType")]
		MLCActivationType ActivationType { get; }

		[Export ("a")]
		float A { get; }

		[Export ("b")]
		float B { get; }

		[Export ("c")]
		float C { get; }

		[Static]
		[Export ("descriptorWithType:")]
		[return: NullAllowed]
		MLCActivationDescriptor Create (MLCActivationType activationType);

		[Static]
		[Export ("descriptorWithType:a:")]
		[return: NullAllowed]
		MLCActivationDescriptor Create (MLCActivationType activationType, float a);

		[Static]
		[Export ("descriptorWithType:a:b:")]
		[return: NullAllowed]
		MLCActivationDescriptor Create (MLCActivationType activationType, float a, float b);

		[Static]
		[Export ("descriptorWithType:a:b:c:")]
		[return: NullAllowed]
		MLCActivationDescriptor Create (MLCActivationType activationType, float a, float b, float c);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCLayer {

		[Export ("layerID")]
		nuint LayerId { get; }

		[Export ("label")]
		string Label { get; set; }

		[Export ("isDebuggingEnabled")]
		bool IsDebuggingEnabled { get; set; }

		[Static]
		[Export ("supportsDataType:onDevice:")]
		bool SupportsDataType (MLCDataType dataType, MLCDeviceType device);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("deviceType")]
		MLCDeviceType DeviceType { get; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCActivationLayer {

		[Export ("descriptor", ArgumentSemantic.Copy)]
		MLCActivationDescriptor Descriptor { get; }

		[Static]
		[Export ("layerWithDescriptor:")]
		MLCActivationLayer Create (MLCActivationDescriptor descriptor);

		[Static]
		[Export ("reluLayer")]
		MLCActivationLayer ReluLayer { get; }

		[Static]
		[Export ("relu6Layer")]
		MLCActivationLayer Relu6Layer { get; }

		[Static]
		[Export ("leakyReLULayer")]
		MLCActivationLayer LeakyReLULayer { get; }

		[Static]
		[Export ("leakyReLULayerWithNegativeSlope:")]
		MLCActivationLayer CreateLeakyReLULayer (float negativeSlope);

		[Static]
		[Export ("linearLayerWithScale:bias:")]
		MLCActivationLayer CreateLinearLayer (float scale, float bias);

		[Static]
		[Export ("sigmoidLayer")]
		MLCActivationLayer SigmoidLayer { get; }

		[Static]
		[Export ("hardSigmoidLayer")]
		MLCActivationLayer HardSigmoidLayer { get; }

		[Static]
		[Export ("tanhLayer")]
		MLCActivationLayer TanhLayer { get; }

		[Static]
		[Export ("absoluteLayer")]
		MLCActivationLayer AbsoluteLayer { get; }

		[Static]
		[Export ("softPlusLayer")]
		MLCActivationLayer SoftPlusLayer { get; }

		[Static]
		[Export ("softPlusLayerWithBeta:")]
		MLCActivationLayer CreateSoftPlusLayer (float beta);

		[Static]
		[Export ("softSignLayer")]
		MLCActivationLayer SoftSignLayer { get; }

		[Static]
		[Export ("eluLayer")]
		MLCActivationLayer EluLayer { get; }

		[Static]
		[Export ("eluLayerWithA:")]
		MLCActivationLayer CreateEluLayer (float a);

		[Static]
		[Export ("relunLayerWithA:b:")]
		MLCActivationLayer CreateRelunLayer (float a, float b);

		[Static]
		[Export ("logSigmoidLayer")]
		MLCActivationLayer LogSigmoidLayer { get; }

		[Static]
		[Export ("seluLayer")]
		MLCActivationLayer SeluLayer { get; }

		[Static]
		[Export ("celuLayer")]
		MLCActivationLayer CeluLayer { get; }

		[Static]
		[Export ("celuLayerWithA:")]
		MLCActivationLayer CreateCeluLayer (float a);

		[Static]
		[Export ("hardShrinkLayer")]
		MLCActivationLayer HardShrinkLayer { get; }

		[Static]
		[Export ("hardShrinkLayerWithA:")]
		MLCActivationLayer CreateHardShrinkLayer (float a);

		[Static]
		[Export ("softShrinkLayer")]
		MLCActivationLayer SoftShrinkLayer { get; }

		[Static]
		[Export ("softShrinkLayerWithA:")]
		MLCActivationLayer CreateSoftShrinkLayer (float a);

		[Static]
		[Export ("tanhShrinkLayer")]
		MLCActivationLayer TanhShrinkLayer { get; }

		[Static]
		[Export ("thresholdLayerWithThreshold:replacement:")]
		MLCActivationLayer CreateThresholdLayer (float threshold, float replacement);

		[Static]
		[Export ("geluLayer")]
		MLCActivationLayer GeluLayer { get; }

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Static]
		[Export ("hardSwishLayer")]
		MLCActivationLayer CreateHardSwishLayer ();

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Static]
		[Export ("clampLayerWithMinValue:maxValue:")]
		MLCActivationLayer CreateClampLayer (float minValue, float maxValue);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCOptimizer : NSCopying {

		[Export ("learningRate")]
		float LearningRate { get; set; }

		[Export ("gradientRescale")]
		float GradientRescale { get; }

		[Export ("appliesGradientClipping")]
		bool AppliesGradientClipping { get; set; }

		[Export ("gradientClipMax")]
		float GradientClipMax { get; }

		[Export ("gradientClipMin")]
		float GradientClipMin { get; }

		[Export ("regularizationScale")]
		float RegularizationScale { get; }

		[Export ("regularizationType")]
		MLCRegularizationType RegularizationType { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("gradientClippingType")]
		MLCGradientClippingType GradientClippingType { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("maximumClippingNorm")]
		float MaximumClippingNorm { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("customGlobalNorm")]
		float CustomGlobalNorm { get; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCOptimizerDescriptor : NSCopying {

		[Export ("learningRate")]
		float LearningRate { get; }

		[Export ("gradientRescale")]
		float GradientRescale { get; }

		[Export ("appliesGradientClipping")]
		bool AppliesGradientClipping { get; }

		[Export ("gradientClipMax")]
		float GradientClipMax { get; }

		[Export ("gradientClipMin")]
		float GradientClipMin { get; }

		[Export ("regularizationScale")]
		float RegularizationScale { get; }

		[Export ("regularizationType")]
		MLCRegularizationType RegularizationType { get; }

		[Static]
		[Export ("descriptorWithLearningRate:gradientRescale:regularizationType:regularizationScale:")]
		MLCOptimizerDescriptor Create (float learningRate, float gradientRescale, MLCRegularizationType regularizationType, float regularizationScale);

		[Static]
		[Export ("descriptorWithLearningRate:gradientRescale:appliesGradientClipping:gradientClipMax:gradientClipMin:regularizationType:regularizationScale:")]
		MLCOptimizerDescriptor Create (float learningRate, float gradientRescale, bool appliesGradientClipping, float gradientClipMax, float gradientClipMin, MLCRegularizationType regularizationType, float regularizationScale);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("descriptorWithLearningRate:gradientRescale:appliesGradientClipping:gradientClippingType:gradientClipMax:gradientClipMin:maximumClippingNorm:customGlobalNorm:regularizationType:regularizationScale:")]
		MLCOptimizerDescriptor Create (float learningRate, float gradientRescale, bool appliesGradientClipping, MLCGradientClippingType gradientClippingType, float gradientClipMax, float gradientClipMin, float maximumClippingNorm, float customGlobalNorm, MLCRegularizationType regularizationType, float regularizationScale);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("gradientClippingType")]
		MLCGradientClippingType GradientClippingType { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("maximumClippingNorm")]
		float MaximumClippingNorm { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("customGlobalNorm")]
		float CustomGlobalNorm { get; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCOptimizer))]
	[DisableDefaultCtor]
	interface MLCAdamOptimizer : NSCopying {

		[Export ("beta1")]
		float Beta1 { get; }

		[Export ("beta2")]
		float Beta2 { get; }

		[Export ("epsilon")]
		float Epsilon { get; }

		[Export ("timeStep")]
		nuint TimeStep { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("usesAMSGrad")]
		bool UsesAmsGrad { get; }

		[Static]
		[Export ("optimizerWithDescriptor:")]
		MLCAdamOptimizer Create (MLCOptimizerDescriptor optimizerDescriptor);

		[Static]
		[Export ("optimizerWithDescriptor:beta1:beta2:epsilon:timeStep:")]
		MLCAdamOptimizer Create (MLCOptimizerDescriptor optimizerDescriptor, float beta1, float beta2, float epsilon, nuint timeStep);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("optimizerWithDescriptor:beta1:beta2:epsilon:usesAMSGrad:timeStep:")]
		MLCAdamOptimizer Create (MLCOptimizerDescriptor optimizerDescriptor, float beta1, float beta2, float epsilon, bool usesAmsGrad, nuint timeStep);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCDevice : NSCopying {

		[Export ("type")]
		MLCDeviceType Type { get; }

		[Export ("gpuDevices")]
		IMTLDevice [] GpuDevices { get; }

		[Static]
		[Export ("cpuDevice")]
		MLCDevice GetCpu ();

		[Static]
		[Export ("gpuDevice")]
		[return: NullAllowed]
		MLCDevice GetGpu ();

		[Static]
		[Export ("deviceWithType:")]
		[return: NullAllowed]
		MLCDevice GetDevice (MLCDeviceType type);

		[Static]
		[Export ("deviceWithGPUDevices:")]
		[return: NullAllowed]
		MLCDevice GetDevice (IMTLDevice [] gpus);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("aneDevice")]
		[return: NullAllowed]
		MLCDevice GetAneDevice ();

		[iOS (14, 2)]
		[TV (14, 2)]
		[MacCatalyst (14, 2)]
		[Static]
		[Export ("deviceWithType:selectsMultipleComputeDevices:")]
		[return: NullAllowed]
		MLCDevice GetDevice (MLCDeviceType type, bool selectsMultipleComputeDevices);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("actualDeviceType")]
		MLCDeviceType ActualDeviceType { get; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCTensor : NSCopying {

		[Export ("tensorID")]
		nuint TensorId { get; }

		[Export ("descriptor", ArgumentSemantic.Copy)]
		MLCTensorDescriptor Descriptor { get; }

		[NullAllowed, Export ("data", ArgumentSemantic.Retain)]
		NSData Data { get; }

		[Export ("label")]
		string Label { get; set; }

		[NullAllowed, Export ("device", ArgumentSemantic.Retain)]
		MLCDevice Device { get; }

		[Export ("optimizerData", ArgumentSemantic.Copy)]
		MLCTensorData [] OptimizerData {
			get;
#if !NET
			[NotImplemented]
			set;
#endif
		}

		[Export ("optimizerDeviceData", ArgumentSemantic.Copy)]
		MLCTensorOptimizerDeviceData [] OptimizerDeviceData { get; }

		[Static]
		[Export ("tensorWithDescriptor:")]
		MLCTensor Create (MLCTensorDescriptor tensorDescriptor);

		[Static]
		[Export ("tensorWithDescriptor:randomInitializerType:")]
		MLCTensor Create (MLCTensorDescriptor tensorDescriptor, MLCRandomInitializerType randomInitializerType);

		[Static]
		[Export ("tensorWithDescriptor:data:")]
		MLCTensor Create (MLCTensorDescriptor tensorDescriptor, MLCTensorData data);

		[Static]
		[Export ("tensorWithShape:")]
		MLCTensor Create ([BindAs (typeof (nint []))] NSNumber [] shape);

		[Static]
		[Export ("tensorWithShape:randomInitializerType:")]
		MLCTensor Create ([BindAs (typeof (nint []))] NSNumber [] shape, MLCRandomInitializerType randomInitializerType);

		[Static]
		[Export ("tensorWithShape:dataType:")]
		MLCTensor Create ([BindAs (typeof (nint []))] NSNumber [] shape, MLCDataType dataType);

		[Static]
		[Export ("tensorWithShape:data:dataType:")]
		MLCTensor Create ([BindAs (typeof (nint []))] NSNumber [] shape, MLCTensorData data, MLCDataType dataType);

		[Static]
		[Export ("tensorWithWidth:height:featureChannelCount:batchSize:")]
		MLCTensor Create (nuint width, nuint height, nuint featureChannelCount, nuint batchSize);

		[Static]
		[Export ("tensorWithWidth:height:featureChannelCount:batchSize:fillWithData:dataType:")]
		MLCTensor Create (nuint width, nuint height, nuint featureChannelCount, nuint batchSize, float fillWithData, MLCDataType dataType);

		[Static]
		[Export ("tensorWithWidth:height:featureChannelCount:batchSize:randomInitializerType:")]
		MLCTensor Create (nuint width, nuint height, nuint featureChannelCount, nuint batchSize, MLCRandomInitializerType randomInitializerType);

		[Static]
		[Export ("tensorWithWidth:height:featureChannelCount:batchSize:data:")]
		MLCTensor Create (nuint width, nuint height, nuint featureChannelCount, nuint batchSize, MLCTensorData data);

		[Static]
		[Export ("tensorWithWidth:height:featureChannelCount:batchSize:data:dataType:")]
		MLCTensor Create (nuint width, nuint height, nuint featureChannelCount, nuint batchSize, MLCTensorData data, MLCDataType dataType);

		[Static]
		[Export ("tensorWithSequenceLength:featureChannelCount:batchSize:")]
		MLCTensor Create (nuint sequenceLength, nuint featureChannelCount, nuint batchSize);

		[Static]
		[Export ("tensorWithSequenceLength:featureChannelCount:batchSize:randomInitializerType:")]
		MLCTensor Create (nuint sequenceLength, nuint featureChannelCount, nuint batchSize, MLCRandomInitializerType randomInitializerType);

		[Static]
		[Export ("tensorWithSequenceLength:featureChannelCount:batchSize:data:")]
		MLCTensor Create (nuint sequenceLength, nuint featureChannelCount, nuint batchSize, [NullAllowed] MLCTensorData data);

		[Static]
		[Export ("tensorWithSequenceLengths:sortedSequences:featureChannelCount:batchSize:randomInitializerType:")]
		[return: NullAllowed]
		MLCTensor Create ([BindAs (typeof (nint []))] NSNumber [] sequenceLengths, bool sortedSequences, nuint featureChannelCount, nuint batchSize, MLCRandomInitializerType randomInitializerType);

		[Static]
		[Export ("tensorWithShape:fillWithData:dataType:")]
		MLCTensor Create ([BindAs (typeof (nint []))] NSNumber [] shape, NSNumber fillData, MLCDataType dataType);

		[Static]
		[Export ("tensorWithDescriptor:fillWithData:")]
		MLCTensor Create (MLCTensorDescriptor tensorDescriptor, NSNumber fillData);

		[Static]
		[Export ("tensorWithSequenceLengths:sortedSequences:featureChannelCount:batchSize:data:")]
		[return: NullAllowed]
		MLCTensor Create ([BindAs (typeof (nint []))] NSNumber [] sequenceLengths, bool sortedSequences, nuint featureChannelCount, nuint batchSize, [NullAllowed] MLCTensorData data);

		[Export ("hasValidNumerics")]
		bool HasValidNumerics { get; }

		[Export ("synchronizeData")]
		bool SynchronizeData ();

		[Export ("synchronizeOptimizerData")]
		bool SynchronizeOptimizerData ();

		[Export ("copyDataFromDeviceMemoryToBytes:length:synchronizeWithDevice:")]
		bool CopyDataFromDeviceMemory (IntPtr bytes, nuint length, bool synchronizeWithDevice);

		[Export ("bindAndWriteData:toDevice:")]
		bool BindAndWrite (MLCTensorData data, MLCDevice device);

		[Export ("bindOptimizerData:deviceData:")]
		bool BindOptimizer (MLCTensorData [] data, [NullAllowed] MLCTensorOptimizerDeviceData [] deviceData);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("tensorByQuantizingToType:scale:bias:")]
		[return: NullAllowed]
		MLCTensor CreateByQuantizing (MLCDataType type, float scale, nint bias);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("tensorByDequantizingToType:scale:bias:")]
		[return: NullAllowed]
		MLCTensor CreateByDequantizing (MLCDataType type, MLCTensor scale, MLCTensor bias);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("tensorByQuantizingToType:scale:bias:axis:")]
		[return: NullAllowed]
		MLCTensor CreateByQuantizing (MLCDataType type, MLCTensor scale, MLCTensor bias, nint axis);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("tensorByDequantizingToType:scale:bias:axis:")]
		[return: NullAllowed]
		MLCTensor CreateByDequantizing (MLCDataType type, MLCTensor scale, MLCTensor bias, nint axis);

		[Static]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("tensorWithShape:randomInitializerType:dataType:")]
		MLCTensor Create ([BindAs (typeof (nint []))] NSNumber [] shape, MLCRandomInitializerType randomInitializerType, MLCDataType dataType);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCTensorData {

		[Export ("bytes")]
		IntPtr Bytes { get; }

		[Export ("length")]
		nuint Length { get; }

		[Static]
		[Export ("dataWithBytesNoCopy:length:")]
		MLCTensorData CreateFromBytesNoCopy (IntPtr bytes, nuint length);

		[Static]
		[Export ("dataWithImmutableBytesNoCopy:length:")]
		MLCTensorData CreateFromImmutableBytesNoCopy (IntPtr bytes, nuint length);

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Static]
		[Export ("dataWithBytesNoCopy:length:deallocator:")]
		MLCTensorData CreateFromBytesNoCopy (IntPtr bytes, nuint length, Action<IntPtr, nuint> deallocator);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCTensorDescriptor : NSCopying {

		[Export ("dataType")]
		MLCDataType DataType { get; }

		[Export ("dimensionCount")]
		nuint DimensionCount { get; }

		[Export ("shape", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]`
		NSNumber [] Shape { get; }

		[Export ("stride", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]`
		NSNumber [] Stride { get; }

		[Export ("tensorAllocationSizeInBytes")]
		nuint TensorAllocationSizeInBytes { get; }

		[NullAllowed]
		[Export ("sequenceLengths", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]?`
		NSNumber [] SequenceLengths { get; }

		[Export ("sortedSequences")]
		bool SortedSequences { get; }

		[NullAllowed]
		[Export ("batchSizePerSequenceStep", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]?`
		NSNumber [] BatchSizePerSequenceStep { get; }

		[Static]
		[Export ("maxTensorDimensions")]
		nuint MaxTensorDimensions { get; }

		[Static]
		[Export ("descriptorWithShape:dataType:")]
		[return: NullAllowed]
		MLCTensorDescriptor Create ([BindAs (typeof (nint []))] NSNumber [] shape, MLCDataType dataType);

		[Static]
		[Export ("descriptorWithShape:sequenceLengths:sortedSequences:dataType:")]
		[return: NullAllowed]
		MLCTensorDescriptor Create ([BindAs (typeof (nint []))] NSNumber [] shape, [BindAs (typeof (nint []))] NSNumber [] sequenceLengths, bool sortedSequences, MLCDataType dataType);

		[Static]
		[Export ("descriptorWithWidth:height:featureChannelCount:batchSize:")]
		[return: NullAllowed]
		MLCTensorDescriptor Create (nuint width, nuint height, nuint featureChannels, nuint batchSize);

		[Static]
		[Export ("descriptorWithWidth:height:featureChannelCount:batchSize:dataType:")]
		[return: NullAllowed]
		MLCTensorDescriptor Create (nuint width, nuint height, nuint featureChannelCount, nuint batchSize, MLCDataType dataType);

		[Static]
		[Export ("convolutionWeightsDescriptorWithWidth:height:inputFeatureChannelCount:outputFeatureChannelCount:dataType:")]
		[return: NullAllowed]
		MLCTensorDescriptor CreateConvolutionWeights (nuint width, nuint height, nuint inputFeatureChannelCount, nuint outputFeatureChannelCount, MLCDataType dataType);

		[Static]
		[Export ("convolutionWeightsDescriptorWithInputFeatureChannelCount:outputFeatureChannelCount:dataType:")]
		[return: NullAllowed]
		MLCTensorDescriptor CreateConvolutionWeights (nuint inputFeatureChannelCount, nuint outputFeatureChannelCount, MLCDataType dataType);

		[Static]
		[Export ("convolutionBiasesDescriptorWithFeatureChannelCount:dataType:")]
		[return: NullAllowed]
		MLCTensorDescriptor CreateConvolutionBiases (nuint featureChannelCount, MLCDataType dataType);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCTensorParameter {

		[Export ("tensor", ArgumentSemantic.Retain)]
		MLCTensor Tensor { get; }

		[Export ("isUpdatable")]
		bool IsUpdatable { get; set; }

		[Static]
		[Export ("parameterWithTensor:")]
		MLCTensorParameter Create (MLCTensor tensor);

		[Static]
		[Export ("parameterWithTensor:optimizerData:")]
		MLCTensorParameter Create (MLCTensor tensor, [NullAllowed] MLCTensorData [] optimizerData);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCArithmeticLayer {

		[Export ("operation")]
		MLCArithmeticOperation Operation { get; }

		[Static]
		[Export ("layerWithOperation:")]
		MLCArithmeticLayer Create (MLCArithmeticOperation operation);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCBatchNormalizationLayer {

		[Export ("featureChannelCount")]
		nuint FeatureChannelCount { get; }

		[Export ("mean", ArgumentSemantic.Retain)]
		MLCTensor Mean { get; }

		[Export ("variance", ArgumentSemantic.Retain)]
		MLCTensor Variance { get; }

		[NullAllowed, Export ("beta", ArgumentSemantic.Retain)]
		MLCTensor Beta { get; }

		[NullAllowed, Export ("gamma", ArgumentSemantic.Retain)]
		MLCTensor Gamma { get; }

		[NullAllowed, Export ("betaParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter BetaParameter { get; }

		[NullAllowed, Export ("gammaParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter GammaParameter { get; }

		[Export ("varianceEpsilon")]
		float VarianceEpsilon { get; }

		[Export ("momentum")]
		float Momentum { get; }

		[Static]
		[Export ("layerWithFeatureChannelCount:mean:variance:beta:gamma:varianceEpsilon:")]
		[return: NullAllowed]
		MLCBatchNormalizationLayer Create (nuint featureChannelCount, MLCTensor mean, MLCTensor variance, [NullAllowed] MLCTensor beta, [NullAllowed] MLCTensor gamma, float varianceEpsilon);

		[Static]
		[Export ("layerWithFeatureChannelCount:mean:variance:beta:gamma:varianceEpsilon:momentum:")]
		[return: NullAllowed]
		MLCBatchNormalizationLayer Create (nuint featureChannelCount, MLCTensor mean, MLCTensor variance, [NullAllowed] MLCTensor beta, [NullAllowed] MLCTensor gamma, float varianceEpsilon, float momentum);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCConcatenationLayer {

		[Export ("dimension")]
		nuint Dimension { get; }

		[Static]
		[Export ("layer")]
		MLCConcatenationLayer Create ();

		[Static]
		[Export ("layerWithDimension:")]
		MLCConcatenationLayer Create (nuint dimension);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCConvolutionDescriptor : NSCopying {

		[Export ("convolutionType")]
		MLCConvolutionType ConvolutionType { get; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("inputFeatureChannelCount")]
		nuint InputFeatureChannelCount { get; }

		[Export ("outputFeatureChannelCount")]
		nuint OutputFeatureChannelCount { get; }

		[Export ("strideInX")]
		nuint StrideInX { get; }

		[Export ("strideInY")]
		nuint StrideInY { get; }

		[Export ("dilationRateInX")]
		nuint DilationRateInX { get; }

		[Export ("dilationRateInY")]
		nuint DilationRateInY { get; }

		[Export ("groupCount")]
		nuint GroupCount { get; }

		[Export ("paddingPolicy")]
		MLCPaddingPolicy PaddingPolicy { get; }

		[Export ("paddingSizeInX")]
		nuint PaddingSizeInX { get; }

		[Export ("paddingSizeInY")]
		nuint PaddingSizeInY { get; }

		[Export ("isConvolutionTranspose")]
		bool IsConvolutionTranspose { get; }

		[Export ("usesDepthwiseConvolution")]
		bool UsesDepthwiseConvolution { get; }

		[Static]
		[Export ("descriptorWithType:kernelSizes:inputFeatureChannelCount:outputFeatureChannelCount:groupCount:strides:dilationRates:paddingPolicy:paddingSizes:")]
		MLCConvolutionDescriptor Create (MLCConvolutionType convolutionType, [BindAs (typeof (nuint []))] NSNumber [] kernelSizes, nuint inputFeatureChannelCount, nuint outputFeatureChannelCount, nuint groupCount, [BindAs (typeof (nuint []))] NSNumber [] strides, [BindAs (typeof (nuint []))] NSNumber [] dilationRates, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);

		[Static]
		[Export ("descriptorWithKernelWidth:kernelHeight:inputFeatureChannelCount:outputFeatureChannelCount:")]
		MLCConvolutionDescriptor Create (nuint kernelWidth, nuint kernelHeight, nuint inputFeatureChannelCount, nuint outputFeatureChannelCount);

		[Static]
		[Export ("descriptorWithKernelSizes:inputFeatureChannelCount:outputFeatureChannelCount:strides:paddingPolicy:paddingSizes:")]
		MLCConvolutionDescriptor Create ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, nuint inputFeatureChannelCount, nuint outputFeatureChannelCount, [BindAs (typeof (nuint []))] NSNumber [] strides, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);

		[Static]
		[Export ("descriptorWithKernelSizes:inputFeatureChannelCount:outputFeatureChannelCount:groupCount:strides:dilationRates:paddingPolicy:paddingSizes:")]
		MLCConvolutionDescriptor Create ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, nuint inputFeatureChannelCount, nuint outputFeatureChannelCount, nuint groupCount, [BindAs (typeof (nuint []))] NSNumber [] strides, [BindAs (typeof (nuint []))] NSNumber [] dilationRates, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);

		[Static]
		[Export ("convolutionTransposeDescriptorWithKernelWidth:kernelHeight:inputFeatureChannelCount:outputFeatureChannelCount:")]
		MLCConvolutionDescriptor CreateConvolutionTranspose (nuint kernelWidth, nuint kernelHeight, nuint inputFeatureChannelCount, nuint outputFeatureChannelCount);

		[Static]
		[Export ("convolutionTransposeDescriptorWithKernelSizes:inputFeatureChannelCount:outputFeatureChannelCount:strides:paddingPolicy:paddingSizes:")]
		MLCConvolutionDescriptor CreateConvolutionTranspose ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, nuint inputFeatureChannelCount, nuint outputFeatureChannelCount, [BindAs (typeof (nuint []))] NSNumber [] strides, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);

		[Static]
		[Export ("convolutionTransposeDescriptorWithKernelSizes:inputFeatureChannelCount:outputFeatureChannelCount:groupCount:strides:dilationRates:paddingPolicy:paddingSizes:")]
		MLCConvolutionDescriptor CreateConvolutionTranspose ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, nuint inputFeatureChannelCount, nuint outputFeatureChannelCount, nuint groupCount, [BindAs (typeof (nuint []))] NSNumber [] strides, [BindAs (typeof (nuint []))] NSNumber [] dilationRates, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);

		[Static]
		[Export ("depthwiseConvolutionDescriptorWithKernelWidth:kernelHeight:inputFeatureChannelCount:channelMultiplier:")]
		MLCConvolutionDescriptor CreateDepthwiseConvolution (nuint kernelWidth, nuint kernelHeight, nuint inputFeatureChannelCount, nuint channelMultiplier);

		[Static]
		[Export ("depthwiseConvolutionDescriptorWithKernelSizes:inputFeatureChannelCount:channelMultiplier:strides:paddingPolicy:paddingSizes:")]
		MLCConvolutionDescriptor CreateDepthwiseConvolution ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, nuint inputFeatureChannelCount, nuint channelMultiplier, [BindAs (typeof (nuint []))] NSNumber [] strides, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);

		[Static]
		[Export ("depthwiseConvolutionDescriptorWithKernelSizes:inputFeatureChannelCount:channelMultiplier:strides:dilationRates:paddingPolicy:paddingSizes:")]
		MLCConvolutionDescriptor CreateDepthwiseConvolution ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, nuint inputFeatureChannelCount, nuint channelMultiplier, [BindAs (typeof (nuint []))] NSNumber [] strides, [BindAs (typeof (nuint []))] NSNumber [] dilationRates, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCConvolutionLayer {

		[Export ("descriptor", ArgumentSemantic.Copy)]
		MLCConvolutionDescriptor Descriptor { get; }

		[Export ("weights", ArgumentSemantic.Retain)]
		MLCTensor Weights { get; }

		[NullAllowed, Export ("biases", ArgumentSemantic.Retain)]
		MLCTensor Biases { get; }

		[Export ("weightsParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter WeightsParameter { get; }

		[NullAllowed, Export ("biasesParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter BiasesParameter { get; }

		[Static]
		[Export ("layerWithWeights:biases:descriptor:")]
		[return: NullAllowed]
		MLCConvolutionLayer Create (MLCTensor weights, [NullAllowed] MLCTensor biases, MLCConvolutionDescriptor descriptor);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCDropoutLayer {

		[Export ("rate")]
		float Rate { get; }

		[Export ("seed")]
		nuint Seed { get; }

		[Static]
		[Export ("layerWithRate:seed:")]
		MLCDropoutLayer Create (float rate, nuint seed);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCEmbeddingDescriptor : NSCopying {

		[Export ("embeddingCount")]
		[BindAs (typeof (nint))] // swift `Int`
		NSNumber EmbeddingCount { get; }

		[Export ("embeddingDimension")]
		[BindAs (typeof (nint))] // swift `Int`
		NSNumber EmbeddingDimension { get; }

		[NullAllowed]
		[Export ("paddingIndex")]
		[BindAs (typeof (nint?))] // swift `Int?`
		NSNumber PaddingIndex { get; }

		[NullAllowed]
		[Export ("maximumNorm")]
		[BindAs (typeof (float?))] // swift `Float?`
		NSNumber MaximumNorm { get; }

		[NullAllowed]
		[Export ("pNorm")]
		[BindAs (typeof (float?))] // swift `Float?`
		NSNumber PNorm { get; }

		[Export ("scalesGradientByFrequency")]
		bool ScalesGradientByFrequency { get; }

		[Static]
		[Export ("descriptorWithEmbeddingCount:embeddingDimension:")]
		[return: NullAllowed]
		MLCEmbeddingDescriptor Create ([BindAs (typeof (nint))] NSNumber embeddingCount, [BindAs (typeof (nint))] NSNumber embeddingDimension);

		[Static]
		[Export ("descriptorWithEmbeddingCount:embeddingDimension:paddingIndex:maximumNorm:pNorm:scalesGradientByFrequency:")]
		[return: NullAllowed]
		MLCEmbeddingDescriptor Create ([BindAs (typeof (nint))] NSNumber embeddingCount, [BindAs (typeof (nint))] NSNumber embeddingDimension, [BindAs (typeof (nint?))] NSNumber paddingIndex, [BindAs (typeof (float?))] NSNumber maximumNorm, [BindAs (typeof (float?))] NSNumber pNorm, bool scalesGradientByFrequency);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCEmbeddingLayer {

		[Export ("descriptor", ArgumentSemantic.Copy)]
		MLCEmbeddingDescriptor Descriptor { get; }

		[Export ("weights", ArgumentSemantic.Retain)]
		MLCTensor Weights { get; }

		[Export ("weightsParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter WeightsParameter { get; }

		[Static]
		[Export ("layerWithDescriptor:weights:")]
		MLCEmbeddingLayer Create (MLCEmbeddingDescriptor descriptor, MLCTensor weights);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCFullyConnectedLayer {

		[Export ("descriptor", ArgumentSemantic.Copy)]
		MLCConvolutionDescriptor Descriptor { get; }

		[Export ("weights", ArgumentSemantic.Retain)]
		MLCTensor Weights { get; }

		[NullAllowed, Export ("biases", ArgumentSemantic.Retain)]
		MLCTensor Biases { get; }

		[Export ("weightsParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter WeightsParameter { get; }

		[NullAllowed, Export ("biasesParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter BiasesParameter { get; }

		[Static]
		[Export ("layerWithWeights:biases:descriptor:")]
		[return: NullAllowed]
		MLCFullyConnectedLayer Create (MLCTensor weights, [NullAllowed] MLCTensor biases, MLCConvolutionDescriptor descriptor);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCGramMatrixLayer {

		[Export ("scale")]
		float Scale { get; }

		[Static]
		[Export ("layerWithScale:")]
		MLCGramMatrixLayer Create (float scale);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCGroupNormalizationLayer {

		[Export ("featureChannelCount")]
		nuint FeatureChannelCount { get; }

		[Export ("groupCount")]
		nuint GroupCount { get; }

		[NullAllowed, Export ("beta", ArgumentSemantic.Retain)]
		MLCTensor Beta { get; }

		[NullAllowed, Export ("gamma", ArgumentSemantic.Retain)]
		MLCTensor Gamma { get; }

		[NullAllowed, Export ("betaParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter BetaParameter { get; }

		[NullAllowed, Export ("gammaParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter GammaParameter { get; }

		[Export ("varianceEpsilon")]
		float VarianceEpsilon { get; }

		[Static]
		[Export ("layerWithFeatureChannelCount:groupCount:beta:gamma:varianceEpsilon:")]
		[return: NullAllowed]
		MLCGroupNormalizationLayer Create (nuint featureChannelCount, nuint groupCount, [NullAllowed] MLCTensor beta, [NullAllowed] MLCTensor gamma, float varianceEpsilon);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCInstanceNormalizationLayer {

		[Export ("featureChannelCount")]
		nuint FeatureChannelCount { get; }

		[NullAllowed, Export ("beta", ArgumentSemantic.Retain)]
		MLCTensor Beta { get; }

		[NullAllowed, Export ("gamma", ArgumentSemantic.Retain)]
		MLCTensor Gamma { get; }

		[NullAllowed, Export ("betaParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter BetaParameter { get; }

		[NullAllowed, Export ("gammaParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter GammaParameter { get; }

		[Export ("varianceEpsilon")]
		float VarianceEpsilon { get; }

		[Export ("momentum")]
		float Momentum { get; }

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[NullAllowed, Export ("mean", ArgumentSemantic.Retain)]
		MLCTensor Mean { get; }

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[NullAllowed, Export ("variance", ArgumentSemantic.Retain)]
		MLCTensor Variance { get; }

		[Static]
		[Export ("layerWithFeatureChannelCount:beta:gamma:varianceEpsilon:")]
		[return: NullAllowed]
		MLCInstanceNormalizationLayer Create (nuint featureChannelCount, [NullAllowed] MLCTensor beta, [NullAllowed] MLCTensor gamma, float varianceEpsilon);

		[Static]
		[Export ("layerWithFeatureChannelCount:beta:gamma:varianceEpsilon:momentum:")]
		[return: NullAllowed]
		MLCInstanceNormalizationLayer Create (nuint featureChannelCount, [NullAllowed] MLCTensor beta, [NullAllowed] MLCTensor gamma, float varianceEpsilon, float momentum);

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Static]
		[Export ("layerWithFeatureChannelCount:mean:variance:beta:gamma:varianceEpsilon:momentum:")]
		[return: NullAllowed]
		MLCInstanceNormalizationLayer Create (nuint featureChannelCount, MLCTensor mean, MLCTensor variance, [NullAllowed] MLCTensor beta, [NullAllowed] MLCTensor gamma, float varianceEpsilon, float momentum);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCLayerNormalizationLayer {

		[Export ("normalizedShape", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]`
		NSNumber [] NormalizedShape { get; }

		[NullAllowed, Export ("beta", ArgumentSemantic.Retain)]
		MLCTensor Beta { get; }

		[NullAllowed, Export ("gamma", ArgumentSemantic.Retain)]
		MLCTensor Gamma { get; }

		[NullAllowed, Export ("betaParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter BetaParameter { get; }

		[NullAllowed, Export ("gammaParameter", ArgumentSemantic.Retain)]
		MLCTensorParameter GammaParameter { get; }

		[Export ("varianceEpsilon")]
		float VarianceEpsilon { get; }

		[Static]
		[Export ("layerWithNormalizedShape:beta:gamma:varianceEpsilon:")]
		[return: NullAllowed]
		MLCLayerNormalizationLayer Create ([BindAs (typeof (nint []))] NSNumber [] normalizedShape, [NullAllowed] MLCTensor beta, [NullAllowed] MLCTensor gamma, float varianceEpsilon);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCLossDescriptor : NSCopying {

		[Export ("lossType")]
		MLCLossType LossType { get; }

		[Export ("reductionType")]
		MLCReductionType ReductionType { get; }

		[Export ("weight")]
		float Weight { get; }

		[Export ("labelSmoothing")]
		float LabelSmoothing { get; }

		[Export ("classCount")]
		nuint ClassCount { get; }

		[Export ("epsilon")]
		float Epsilon { get; }

		[Export ("delta")]
		float Delta { get; }

		[Static]
		[Export ("descriptorWithType:reductionType:")]
		MLCLossDescriptor Create (MLCLossType lossType, MLCReductionType reductionType);

		[Static]
		[Export ("descriptorWithType:reductionType:weight:")]
		MLCLossDescriptor Create (MLCLossType lossType, MLCReductionType reductionType, float weight);

		[Static]
		[Export ("descriptorWithType:reductionType:weight:labelSmoothing:classCount:")]
		MLCLossDescriptor Create (MLCLossType lossType, MLCReductionType reductionType, float weight, float labelSmoothing, nuint classCount);

		[Static]
		[Export ("descriptorWithType:reductionType:weight:labelSmoothing:classCount:epsilon:delta:")]
		MLCLossDescriptor Create (MLCLossType lossType, MLCReductionType reductionType, float weight, float labelSmoothing, nuint classCount, float epsilon, float delta);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCLossLayer {

		[Export ("descriptor", ArgumentSemantic.Copy)]
		MLCLossDescriptor Descriptor { get; }

		[NullAllowed, Export ("weights", ArgumentSemantic.Retain)]
		MLCTensor Weights { get; }

		[Static]
		[Export ("layerWithDescriptor:")]
		MLCLossLayer Create (MLCLossDescriptor lossDescriptor);

		[Static]
		[Export ("layerWithDescriptor:weights:")]
		MLCLossLayer Create (MLCLossDescriptor lossDescriptor, MLCTensor weights);

		[Static]
		[Export ("softmaxCrossEntropyLossWithReductionType:labelSmoothing:classCount:weight:")]
		MLCLossLayer CreateSoftmaxCrossEntropyLoss (MLCReductionType reductionType, float labelSmoothing, nuint classCount, float weight);

		[Static]
		[Export ("softmaxCrossEntropyLossWithReductionType:labelSmoothing:classCount:weights:")]
		MLCLossLayer CreateSoftmaxCrossEntropyLoss (MLCReductionType reductionType, float labelSmoothing, nuint classCount, [NullAllowed] MLCTensor weights);

		[Static]
		[Export ("categoricalCrossEntropyLossWithReductionType:labelSmoothing:classCount:weight:")]
		MLCLossLayer CreateCategoricalCrossEntropyLoss (MLCReductionType reductionType, float labelSmoothing, nuint classCount, float weight);

		[Static]
		[Export ("categoricalCrossEntropyLossWithReductionType:labelSmoothing:classCount:weights:")]
		MLCLossLayer CreateCategoricalCrossEntropyLoss (MLCReductionType reductionType, float labelSmoothing, nuint classCount, [NullAllowed] MLCTensor weights);

		[Static]
		[Export ("sigmoidCrossEntropyLossWithReductionType:labelSmoothing:weight:")]
		MLCLossLayer CreateSigmoidCrossEntropyLoss (MLCReductionType reductionType, float labelSmoothing, float weight);

		[Static]
		[Export ("sigmoidCrossEntropyLossWithReductionType:labelSmoothing:weights:")]
		MLCLossLayer CreateSigmoidCrossEntropyLoss (MLCReductionType reductionType, float labelSmoothing, [NullAllowed] MLCTensor weights);

		[Static]
		[Export ("logLossWithReductionType:epsilon:weight:")]
		MLCLossLayer CreateLogLoss (MLCReductionType reductionType, float epsilon, float weight);

		[Static]
		[Export ("logLossWithReductionType:epsilon:weights:")]
		MLCLossLayer CreateLogLoss (MLCReductionType reductionType, float epsilon, [NullAllowed] MLCTensor weights);

		[Static]
		[Export ("huberLossWithReductionType:delta:weight:")]
		MLCLossLayer CreateHuberLoss (MLCReductionType reductionType, float delta, float weight);

		[Static]
		[Export ("huberLossWithReductionType:delta:weights:")]
		MLCLossLayer CreateHuberLoss (MLCReductionType reductionType, float delta, [NullAllowed] MLCTensor weights);

		[Static]
		[Export ("meanAbsoluteErrorLossWithReductionType:weight:")]
		MLCLossLayer CreateMeanAbsoluteErrorLoss (MLCReductionType reductionType, float weight);

		[Static]
		[Export ("meanAbsoluteErrorLossWithReductionType:weights:")]
		MLCLossLayer CreateMeanAbsoluteErrorLoss (MLCReductionType reductionType, [NullAllowed] MLCTensor weights);

		[Static]
		[Export ("meanSquaredErrorLossWithReductionType:weight:")]
		MLCLossLayer CreateMeanSquaredErrorLoss (MLCReductionType reductionType, float weight);

		[Static]
		[Export ("meanSquaredErrorLossWithReductionType:weights:")]
		MLCLossLayer CreateMeanSquaredErrorLoss (MLCReductionType reductionType, [NullAllowed] MLCTensor weights);

		[Static]
		[Export ("hingeLossWithReductionType:weight:")]
		MLCLossLayer CreateHingeLoss (MLCReductionType reductionType, float weight);

		[Static]
		[Export ("hingeLossWithReductionType:weights:")]
		MLCLossLayer CreateHingeLoss (MLCReductionType reductionType, [NullAllowed] MLCTensor weights);

		[Static]
		[Export ("cosineDistanceLossWithReductionType:weight:")]
		MLCLossLayer CreateCosineDistanceLoss (MLCReductionType reductionType, float weight);

		[Static]
		[Export ("cosineDistanceLossWithReductionType:weights:")]
		MLCLossLayer CreateCosineDistanceLoss (MLCReductionType reductionType, [NullAllowed] MLCTensor weights);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "MLCLSTMDescriptor")]
	[DisableDefaultCtor]
	interface MLCLstmDescriptor : NSCopying {

		[Export ("inputSize")]
		nuint InputSize { get; }

		[Export ("hiddenSize")]
		nuint HiddenSize { get; }

		[Export ("layerCount")]
		nuint LayerCount { get; }

		[Export ("usesBiases")]
		bool UsesBiases { get; }

		[Export ("batchFirst")]
		bool BatchFirst { get; }

		[Export ("isBidirectional")]
		bool IsBidirectional { get; }

		[Export ("returnsSequences")]
		bool ReturnsSequences { get; }

		[Export ("dropout")]
		float Dropout { get; }

		[Export ("resultMode")]
		MLCLstmResultMode ResultMode { get; }

		[Static]
		[Export ("descriptorWithInputSize:hiddenSize:layerCount:")]
		MLCLstmDescriptor Create (nuint inputSize, nuint hiddenSize, nuint layerCount);

		[Static]
		[Export ("descriptorWithInputSize:hiddenSize:layerCount:usesBiases:isBidirectional:dropout:")]
		MLCLstmDescriptor Create (nuint inputSize, nuint hiddenSize, nuint layerCount, bool usesBiases, bool isBidirectional, float dropout);

		[Static]
		[Export ("descriptorWithInputSize:hiddenSize:layerCount:usesBiases:batchFirst:isBidirectional:dropout:")]
		MLCLstmDescriptor Create (nuint inputSize, nuint hiddenSize, nuint layerCount, bool usesBiases, bool batchFirst, bool isBidirectional, float dropout);

		[Static]
		[Export ("descriptorWithInputSize:hiddenSize:layerCount:usesBiases:batchFirst:isBidirectional:returnsSequences:dropout:")]
		MLCLstmDescriptor Create (nuint inputSize, nuint hiddenSize, nuint layerCount, bool usesBiases, bool batchFirst, bool isBidirectional, bool returnsSequences, float dropout);

		[Static]
		[Export ("descriptorWithInputSize:hiddenSize:layerCount:usesBiases:batchFirst:isBidirectional:returnsSequences:dropout:resultMode:")]
		MLCLstmDescriptor Create (nuint inputSize, nuint hiddenSize, nuint layerCount, bool usesBiases, bool batchFirst, bool isBidirectional, bool returnsSequences, float dropout, MLCLstmResultMode resultMode);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer), Name = "MLCLSTMLayer")]
	[DisableDefaultCtor]
	interface MLCLstmLayer {

		[Export ("descriptor", ArgumentSemantic.Copy)]
		MLCLstmDescriptor Descriptor { get; }

		[Export ("gateActivations", ArgumentSemantic.Copy)]
		MLCActivationDescriptor [] GateActivations { get; }

		[Export ("outputResultActivation", ArgumentSemantic.Copy)]
		MLCActivationDescriptor OutputResultActivation { get; }

		[Export ("inputWeights", ArgumentSemantic.Retain)]
		MLCTensor [] InputWeights { get; }

		[Export ("hiddenWeights", ArgumentSemantic.Retain)]
		MLCTensor [] HiddenWeights { get; }

		[NullAllowed, Export ("peepholeWeights", ArgumentSemantic.Retain)]
		MLCTensor [] PeepholeWeights { get; }

		[NullAllowed, Export ("biases", ArgumentSemantic.Retain)]
		MLCTensor [] Biases { get; }

		[Export ("inputWeightsParameters", ArgumentSemantic.Retain)]
		MLCTensorParameter [] InputWeightsParameters { get; }

		[Export ("hiddenWeightsParameters", ArgumentSemantic.Retain)]
		MLCTensorParameter [] HiddenWeightsParameters { get; }

		[NullAllowed, Export ("peepholeWeightsParameters", ArgumentSemantic.Retain)]
		MLCTensorParameter [] PeepholeWeightsParameters { get; }

		[NullAllowed, Export ("biasesParameters", ArgumentSemantic.Retain)]
		MLCTensorParameter [] BiasesParameters { get; }

		[Static]
		[Export ("layerWithDescriptor:inputWeights:hiddenWeights:biases:")]
		[return: NullAllowed]
		MLCLstmLayer Create (MLCLstmDescriptor descriptor, MLCTensor [] inputWeights, MLCTensor [] hiddenWeights, [NullAllowed] MLCTensor [] biases);

		[Static]
		[Export ("layerWithDescriptor:inputWeights:hiddenWeights:peepholeWeights:biases:")]
		[return: NullAllowed]
		MLCLstmLayer Create (MLCLstmDescriptor descriptor, MLCTensor [] inputWeights, MLCTensor [] hiddenWeights, [NullAllowed] MLCTensor [] peepholeWeights, [NullAllowed] MLCTensor [] biases);

		[Static]
		[Export ("layerWithDescriptor:inputWeights:hiddenWeights:peepholeWeights:biases:gateActivations:outputResultActivation:")]
		[return: NullAllowed]
		MLCLstmLayer Create (MLCLstmDescriptor descriptor, MLCTensor [] inputWeights, MLCTensor [] hiddenWeights, [NullAllowed] MLCTensor [] peepholeWeights, [NullAllowed] MLCTensor [] biases, MLCActivationDescriptor [] gateActivations, MLCActivationDescriptor outputResultActivation);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCMatMulDescriptor : NSCopying {

		[Export ("alpha")]
		float Alpha { get; }

		[Export ("transposesX")]
		bool TransposesX { get; }

		[Export ("transposesY")]
		bool TransposesY { get; }

		[Static]
		[Export ("descriptorWithAlpha:transposesX:transposesY:")]
		[return: NullAllowed]
		MLCMatMulDescriptor Create (float alpha, bool transposesX, bool transposesY);

		[Static]
		[Export ("descriptor")]
		MLCMatMulDescriptor Create ();
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCMatMulLayer {

		[Export ("descriptor", ArgumentSemantic.Copy)]
		MLCMatMulDescriptor Descriptor { get; }

		[Static]
		[Export ("layerWithDescriptor:")]
		[return: NullAllowed]
		MLCMatMulLayer Create (MLCMatMulDescriptor descriptor);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCMultiheadAttentionDescriptor : NSCopying {

		[Export ("modelDimension")]
		nuint ModelDimension { get; }

		[Export ("keyDimension")]
		nuint KeyDimension { get; }

		[Export ("valueDimension")]
		nuint ValueDimension { get; }

		[Export ("headCount")]
		nuint HeadCount { get; }

		[Export ("dropout")]
		float Dropout { get; }

		[Export ("hasBiases")]
		bool HasBiases { get; }

		[Export ("hasAttentionBiases")]
		bool HasAttentionBiases { get; }

		[Export ("addsZeroAttention")]
		bool AddsZeroAttention { get; }

		[Static]
		[Export ("descriptorWithModelDimension:keyDimension:valueDimension:headCount:dropout:hasBiases:hasAttentionBiases:addsZeroAttention:")]
		[return: NullAllowed]
		MLCMultiheadAttentionDescriptor Create (nuint modelDimension, nuint keyDimension, nuint valueDimension, nuint headCount, float dropout, bool hasBiases, bool hasAttentionBiases, bool addsZeroAttention);

		[Static]
		[Export ("descriptorWithModelDimension:headCount:")]
		MLCMultiheadAttentionDescriptor Create (nuint modelDimension, nuint headCount);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCMultiheadAttentionLayer {

		[Export ("descriptor", ArgumentSemantic.Copy)]
		MLCMultiheadAttentionDescriptor Descriptor { get; }

		[Export ("weights", ArgumentSemantic.Retain)]
		MLCTensor [] Weights { get; }

		[NullAllowed, Export ("biases", ArgumentSemantic.Retain)]
		MLCTensor [] Biases { get; }

		[NullAllowed, Export ("attentionBiases", ArgumentSemantic.Retain)]
		MLCTensor [] AttentionBiases { get; }

		[Export ("weightsParameters", ArgumentSemantic.Retain)]
		MLCTensorParameter [] WeightsParameters { get; }

		[NullAllowed, Export ("biasesParameters", ArgumentSemantic.Retain)]
		MLCTensorParameter [] BiasesParameters { get; }

		[Static]
		[Export ("layerWithDescriptor:weights:biases:attentionBiases:")]
		[return: NullAllowed]
		MLCMultiheadAttentionLayer Create (MLCMultiheadAttentionDescriptor descriptor, MLCTensor [] weights, [NullAllowed] MLCTensor [] biases, [NullAllowed] MLCTensor [] attentionBiases);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCPaddingLayer : NSCopying {

		[Export ("paddingType")]
		MLCPaddingType PaddingType { get; }

		[Export ("paddingLeft")]
		nuint PaddingLeft { get; }

		[Export ("paddingRight")]
		nuint PaddingRight { get; }

		[Export ("paddingTop")]
		nuint PaddingTop { get; }

		[Export ("paddingBottom")]
		nuint PaddingBottom { get; }

		[Export ("constantValue")]
		float ConstantValue { get; }

		[Static]
		[Export ("layerWithReflectionPadding:")]
		MLCPaddingLayer CreateReflectionPadding ([BindAs (typeof (nuint []))] NSNumber [] padding);

		[Static]
		[Export ("layerWithSymmetricPadding:")]
		MLCPaddingLayer CreateSymmetricPadding ([BindAs (typeof (nuint []))] NSNumber [] padding);

		[Static]
		[Export ("layerWithZeroPadding:")]
		MLCPaddingLayer CreateZeroPadding ([BindAs (typeof (nuint []))] NSNumber [] padding);

		[Static]
		[Export ("layerWithConstantPadding:constantValue:")]
		MLCPaddingLayer CreateConstantPadding ([BindAs (typeof (nuint []))] NSNumber [] padding, float constantValue);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCPoolingDescriptor : NSCopying {

		[Export ("poolingType")]
		MLCPoolingType PoolingType { get; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("strideInX")]
		nuint StrideInX { get; }

		[Export ("strideInY")]
		nuint StrideInY { get; }

		[Export ("dilationRateInX")]
		nuint DilationRateInX { get; }

		[Export ("dilationRateInY")]
		nuint DilationRateInY { get; }

		[Export ("paddingPolicy")]
		MLCPaddingPolicy PaddingPolicy { get; }

		[Export ("paddingSizeInX")]
		nuint PaddingSizeInX { get; }

		[Export ("paddingSizeInY")]
		nuint PaddingSizeInY { get; }

		[Export ("countIncludesPadding")]
		bool CountIncludesPadding { get; }

		[Static]
		[Export ("poolingDescriptorWithType:kernelSize:stride:")]
		MLCPoolingDescriptor Create (MLCPoolingType poolingType, nuint kernelSize, nuint stride);

		[Static]
		[Export ("maxPoolingDescriptorWithKernelSizes:strides:paddingPolicy:paddingSizes:")]
		MLCPoolingDescriptor CreateMaxPooling ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, [BindAs (typeof (nuint []))] NSNumber [] strides, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);

		[Static]
		[Export ("maxPoolingDescriptorWithKernelSizes:strides:dilationRates:paddingPolicy:paddingSizes:")]
		MLCPoolingDescriptor CreateMaxPooling ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, [BindAs (typeof (nuint []))] NSNumber [] strides, [BindAs (typeof (nuint []))] NSNumber [] dilationRates, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);

		[Static]
		[Export ("averagePoolingDescriptorWithKernelSizes:strides:paddingPolicy:paddingSizes:countIncludesPadding:")]
		MLCPoolingDescriptor CreateAveragePooling ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, [BindAs (typeof (nuint []))] NSNumber [] strides, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes, bool countIncludesPadding);

		[Static]
		[Export ("averagePoolingDescriptorWithKernelSizes:strides:dilationRates:paddingPolicy:paddingSizes:countIncludesPadding:")]
		MLCPoolingDescriptor CreateAveragePooling ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, [BindAs (typeof (nuint []))] NSNumber [] strides, [BindAs (typeof (nuint []))] NSNumber [] dilationRates, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes, bool countIncludesPadding);

		[Static]
		[Export ("l2NormPoolingDescriptorWithKernelSizes:strides:paddingPolicy:paddingSizes:")]
		MLCPoolingDescriptor CreateL2NormPooling ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, [BindAs (typeof (nuint []))] NSNumber [] strides, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);

		[Static]
		[Export ("l2NormPoolingDescriptorWithKernelSizes:strides:dilationRates:paddingPolicy:paddingSizes:")]
		MLCPoolingDescriptor CreateL2NormPooling ([BindAs (typeof (nuint []))] NSNumber [] kernelSizes, [BindAs (typeof (nuint []))] NSNumber [] strides, [BindAs (typeof (nuint []))] NSNumber [] dilationRates, MLCPaddingPolicy paddingPolicy, [BindAs (typeof (nuint []))][NullAllowed] NSNumber [] paddingSizes);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCPoolingLayer {

		[Export ("descriptor", ArgumentSemantic.Copy)]
		MLCPoolingDescriptor Descriptor { get; }

		[Static]
		[Export ("layerWithDescriptor:")]
		MLCPoolingLayer Create (MLCPoolingDescriptor descriptor);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCReductionLayer {

		[Export ("reductionType")]
		MLCReductionType ReductionType { get; }

		[Export ("dimension")]
		nuint Dimension { get; }

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("dimensions")]
		[BindAs (typeof (nuint []))]
		NSNumber [] Dimensions { get; }

		[Static]
		[Export ("layerWithReductionType:dimension:")]
		[return: NullAllowed]
		MLCReductionLayer Create (MLCReductionType reductionType, nuint dimension);

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Static]
		[Export ("layerWithReductionType:dimensions:")]
		[return: NullAllowed]
		MLCReductionLayer Create (MLCReductionType reductionType, [BindAs (typeof (nuint []))] NSNumber [] dimensions);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCReshapeLayer {

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("shape", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))]
		NSNumber [] Shape { get; }

		[Static]
		[Export ("layerWithShape:")]
		[return: NullAllowed]
		// swift uses `[Int]`
		MLCReshapeLayer Create ([BindAs (typeof (nint []))] NSNumber [] shape);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCOptimizer), Name = "MLCRMSPropOptimizer")]
	interface MLCRmsPropOptimizer : NSCopying {

		[Export ("momentumScale")]
		float MomentumScale { get; }

		[Export ("alpha")]
		float Alpha { get; }

		[Export ("epsilon")]
		float Epsilon { get; }

		[Export ("isCentered")]
		bool IsCentered { get; }

		[Static]
		[Export ("optimizerWithDescriptor:")]
		MLCRmsPropOptimizer Create (MLCOptimizerDescriptor optimizerDescriptor);

		[Static]
		[Export ("optimizerWithDescriptor:momentumScale:alpha:epsilon:isCentered:")]
		MLCRmsPropOptimizer Create (MLCOptimizerDescriptor optimizerDescriptor, float momentumScale, float alpha, float epsilon, bool isCentered);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCOptimizer), Name = "MLCSGDOptimizer")]
	[DisableDefaultCtor]
	interface MLCSgdOptimizer : NSCopying {

		[Export ("momentumScale")]
		float MomentumScale { get; }

		[Export ("usesNesterovMomentum")]
		bool UsesNesterovMomentum { get; }

		[Static]
		[Export ("optimizerWithDescriptor:")]
		MLCSgdOptimizer Create (MLCOptimizerDescriptor optimizerDescriptor);

		[Static]
		[Export ("optimizerWithDescriptor:momentumScale:usesNesterovMomentum:")]
		MLCSgdOptimizer Create (MLCOptimizerDescriptor optimizerDescriptor, float momentumScale, bool usesNesterovMomentum);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCSliceLayer {

		[Export ("start", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]`
		NSNumber [] Start { get; }

		[Export ("end", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]`
		NSNumber [] End { get; }

		[NullAllowed]
		[Export ("stride", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]?`
		NSNumber [] Stride { get; }

		[Static]
		[Export ("sliceLayerWithStart:end:stride:")]
		[return: NullAllowed]
		MLCSliceLayer Create ([BindAs (typeof (nint []))] NSNumber [] start, [BindAs (typeof (nint []))] NSNumber [] end, [BindAs (typeof (nint []))][NullAllowed] NSNumber [] stride);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCSoftmaxLayer {

		[Export ("operation")]
		MLCSoftmaxOperation Operation { get; }

		[Export ("dimension")]
		nuint Dimension { get; }

		[Static]
		[Export ("layerWithOperation:")]
		MLCSoftmaxLayer Create (MLCSoftmaxOperation operation);

		[Static]
		[Export ("layerWithOperation:dimension:")]
		MLCSoftmaxLayer Create (MLCSoftmaxOperation operation, nuint dimension);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCSplitLayer {

		[Export ("dimension")]
		nuint Dimension { get; }

		[Export ("splitCount")]
		nuint SplitCount { get; }

		[NullAllowed]
		[Export ("splitSectionLengths", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]`
		NSNumber [] SplitSectionLengths { get; }

		[Static]
		[Export ("layerWithSplitCount:dimension:")]
		MLCSplitLayer Create (nuint splitCount, nuint dimension);

		[Static]
		[Export ("layerWithSplitSectionLengths:dimension:")]
		MLCSplitLayer Create ([BindAs (typeof (nint []))] NSNumber [] splitSectionLengths, nuint dimension);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCTransposeLayer {

		[Export ("dimensions", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]`
		NSNumber [] Dimensions { get; }

		[Static]
		[Export ("layerWithDimensions:")]
		[return: NullAllowed]
		MLCTransposeLayer Create ([BindAs (typeof (nint []))] NSNumber [] dimensions);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCUpsampleLayer {

		[Export ("shape", ArgumentSemantic.Copy)]
		[BindAs (typeof (nint []))] // swift `[Int]`
		NSNumber [] Shape { get; }

		[Export ("sampleMode")]
		MLCSampleMode SampleMode { get; }

		[Export ("alignsCorners")]
		bool AlignsCorners { get; }

		[Static]
		[Export ("layerWithShape:")]
		[return: NullAllowed]
		MLCUpsampleLayer Create ([BindAs (typeof (nint []))] NSNumber [] shape);

		[Static]
		[Export ("layerWithShape:sampleMode:alignsCorners:")]
		[return: NullAllowed]
		MLCUpsampleLayer Create ([BindAs (typeof (nint []))] NSNumber [] shape, MLCSampleMode sampleMode, bool alignsCorners);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "MLCYOLOLossDescriptor")]
	[DisableDefaultCtor]
	interface MLCYoloLossDescriptor : NSCopying {

		[Export ("anchorBoxCount")]
		nuint AnchorBoxCount { get; }

		[Export ("anchorBoxes", ArgumentSemantic.Copy)]
		NSData AnchorBoxes { get; }

		[Export ("shouldRescore")]
		bool ShouldRescore { get; set; }

		[Export ("scaleSpatialPositionLoss")]
		float ScaleSpatialPositionLoss { get; set; }

		[Export ("scaleSpatialSizeLoss")]
		float ScaleSpatialSizeLoss { get; set; }

		[Export ("scaleNoObjectConfidenceLoss")]
		float ScaleNoObjectConfidenceLoss { get; set; }

		[Export ("scaleObjectConfidenceLoss")]
		float ScaleObjectConfidenceLoss { get; set; }

		[Export ("scaleClassLoss")]
		float ScaleClassLoss { get; set; }

		[Export ("minimumIOUForObjectPresence")]
		float MinimumIouForObjectPresence { get; set; }

		[Export ("maximumIOUForObjectAbsence")]
		float MaximumIouForObjectAbsence { get; set; }

		[Static]
		[Export ("descriptorWithAnchorBoxes:anchorBoxCount:")]
		MLCYoloLossDescriptor Create (NSData anchorBoxes, nuint anchorBoxCount);
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCLossLayer), Name = "MLCYOLOLossLayer")]
	[DisableDefaultCtor]
	interface MLCYoloLossLayer {

		[Export ("yoloLossDescriptor", ArgumentSemantic.Copy)]
		MLCYoloLossDescriptor YoloLossDescriptor { get; }

		[Static]
		[Export ("layerWithDescriptor:")]
		MLCYoloLossLayer Create (MLCYoloLossDescriptor lossDescriptor);
	}

	delegate void MLCGraphCompletionHandler ([NullAllowed] MLCTensor resultTensor, [NullAllowed] NSError error, /* NSTimeInterval */ double executionTime);

	[TV (14, 0), iOS (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCTensorOptimizerDeviceData : NSCopying {
	}

	[TV (14, 0), iOS (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCGraph {

		[NullAllowed, Export ("device", ArgumentSemantic.Retain)]
		MLCDevice Device { get; }

		[Export ("layers")]
		MLCLayer [] Layers { get; }

		[Static]
		[Export ("graph")]
		MLCGraph Create ();

		[Export ("summarizedDOTDescription")]
		string SummarizedDotDescription { get; }

		[Export ("nodeWithLayer:source:")]
		[return: NullAllowed]
		MLCTensor CreateNode (MLCLayer layer, MLCTensor source);

		[Export ("nodeWithLayer:sources:")]
		[return: NullAllowed]
		MLCTensor CreateNode (MLCLayer layer, MLCTensor [] sources);

		[Export ("nodeWithLayer:sources:disableUpdate:")]
		[return: NullAllowed]
		MLCTensor CreateNode (MLCLayer layer, MLCTensor [] sources, bool disableUpdate);

		[Export ("nodeWithLayer:sources:lossLabels:")]
		[return: NullAllowed]
		MLCTensor CreateNode (MLCLayer layer, MLCTensor [] sources, MLCTensor [] lossLabels);

		[Export ("splitWithSource:splitCount:dimension:")]
		[return: NullAllowed]
		MLCTensor [] Split (MLCTensor source, nuint splitCount, nuint dimension);

		[Export ("splitWithSource:splitSectionLengths:dimension:")]
		[return: NullAllowed]
		MLCTensor [] Split (MLCTensor source, [BindAs (typeof (nuint []))] NSNumber [] splitSectionLengths, nuint dimension);

		[Export ("concatenateWithSources:dimension:")]
		[return: NullAllowed]
		MLCTensor Concatenate (MLCTensor [] sources, nuint dimension);

		[Export ("reshapeWithShape:source:")]
		[return: NullAllowed]
		// swift `[Int]`
		MLCTensor Reshape ([BindAs (typeof (nint []))] NSNumber [] shape, MLCTensor source);

		[Export ("transposeWithDimensions:source:")]
		[return: NullAllowed]
		// swift `[Int]`
		MLCTensor Transpose ([BindAs (typeof (nint []))] NSNumber [] dimensions, MLCTensor source);

		[Export ("bindAndWriteData:forInputs:toDevice:batchSize:synchronous:")]
		bool BindAndWrite (NSDictionary<NSString, MLCTensorData> inputsData, NSDictionary<NSString, MLCTensor> inputTensors, MLCDevice device, nuint batchSize, bool synchronous);

		[Export ("bindAndWriteData:forInputs:toDevice:synchronous:")]
		bool BindAndWrite (NSDictionary<NSString, MLCTensorData> inputsData, NSDictionary<NSString, MLCTensor> inputTensors, MLCDevice device, bool synchronous);

		[Export ("sourceTensorsForLayer:")]
		MLCTensor [] GetSourceTensors (MLCLayer layer);

		[Export ("resultTensorsForLayer:")]
		MLCTensor [] GetResultTensors (MLCLayer layer);

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("gatherWithDimension:source:indices:")]
		[return: NullAllowed]
		MLCTensor Gather (nuint dimension, MLCTensor source, MLCTensor indices);

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("scatterWithDimension:source:indices:copyFrom:reductionType:")]
		[return: NullAllowed]
		MLCTensor Scatter (nuint dimension, MLCTensor source, MLCTensor indices, MLCTensor copyFrom, MLCReductionType reductionType);

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("selectWithSources:condition:")]
		[return: NullAllowed]
		MLCTensor Select (MLCTensor [] sources, MLCTensor condition);
	}

	[TV (14, 0), iOS (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCGraph))]
	[DisableDefaultCtor]
	interface MLCTrainingGraph {

		[NullAllowed, Export ("optimizer", ArgumentSemantic.Retain)]
		MLCOptimizer Optimizer { get; }

		[Export ("deviceMemorySize")]
		nuint DeviceMemorySize { get; }

		[Static]
		[Export ("graphWithGraphObjects:lossLayer:optimizer:")]
		MLCTrainingGraph Create (MLCGraph [] graphObjects, [NullAllowed] MLCLayer lossLayer, [NullAllowed] MLCOptimizer optimizer);

		[Export ("addInputs:lossLabels:")]
		bool AddInputs (NSDictionary<NSString, MLCTensor> inputs, [NullAllowed] NSDictionary<NSString, MLCTensor> lossLabels);

		[Export ("addInputs:lossLabels:lossLabelWeights:")]
		bool AddInputs (NSDictionary<NSString, MLCTensor> inputs, [NullAllowed] NSDictionary<NSString, MLCTensor> lossLabels, [NullAllowed] NSDictionary<NSString, MLCTensor> lossLabelWeights);

		[Export ("addOutputs:")]
		bool AddOutputs (NSDictionary<NSString, MLCTensor> outputs);

		[Export ("stopGradientForTensors:")]
		bool StopGradient (MLCTensor [] tensors);

		[Export ("compileWithOptions:device:")]
		bool Compile (MLCGraphCompilationOptions options, MLCDevice device);

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("compileWithOptions:device:inputTensors:inputTensorsData:")]
		bool Compile (MLCGraphCompilationOptions options, MLCDevice device, [NullAllowed] NSDictionary<NSString, MLCTensor> inputTensors, [NullAllowed] NSDictionary<NSString, MLCTensorData> inputTensorsData);

		[Export ("compileOptimizer:")]
		bool Compile (MLCOptimizer optimizer);

		[Export ("linkWithGraphs:")]
		bool Link (MLCTrainingGraph [] graphs);

		[Export ("sourceGradientTensorsForLayer:")]
		MLCTensor [] GetSourceGradientTensors (MLCLayer layer);

		[Export ("resultGradientTensorsForLayer:")]
		MLCTensor [] GetResultGradientTensors (MLCLayer layer);

		[Export ("gradientDataForParameter:layer:")]
		[return: NullAllowed]
		NSData GetGradientData (MLCTensor parameter, MLCLayer layer);

		[Export ("allocateUserGradientForTensor:")]
		[return: NullAllowed]
		MLCTensor AllocateUserGradient (MLCTensor tensor);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeWithInputsData:lossLabelsData:lossLabelWeightsData:batchSize:options:completionHandler:")]
		bool Execute (NSDictionary<NSString, MLCTensorData> inputsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> lossLabelsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> lossLabelWeightsData, nuint batchSize, MLCExecutionOptions options, [NullAllowed] MLCGraphCompletionHandler completionHandler);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeWithInputsData:lossLabelsData:lossLabelWeightsData:outputsData:batchSize:options:completionHandler:")]
		bool Execute (NSDictionary<NSString, MLCTensorData> inputsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> lossLabelsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> lossLabelWeightsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> outputsData, nuint batchSize, MLCExecutionOptions options, [NullAllowed] MLCGraphCompletionHandler completionHandler);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeForwardWithBatchSize:options:completionHandler:")]
		bool ExecuteForward (nuint batchSize, MLCExecutionOptions options, [NullAllowed] MLCGraphCompletionHandler completionHandler);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeForwardWithBatchSize:options:outputsData:completionHandler:")]
		bool ExecuteForward (nuint batchSize, MLCExecutionOptions options, [NullAllowed] NSDictionary<NSString, MLCTensorData> outputsData, [NullAllowed] MLCGraphCompletionHandler completionHandler);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeGradientWithBatchSize:options:completionHandler:")]
		bool ExecuteGradient (nuint batchSize, MLCExecutionOptions options, [NullAllowed] MLCGraphCompletionHandler completionHandler);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeGradientWithBatchSize:options:outputsData:completionHandler:")]
		bool ExecuteGradient (nuint batchSize, MLCExecutionOptions options, [NullAllowed] NSDictionary<NSString, MLCTensorData> outputsData, [NullAllowed] MLCGraphCompletionHandler completionHandler);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeOptimizerUpdateWithOptions:completionHandler:")]
		bool ExecuteOptimizerUpdate (MLCExecutionOptions options, [NullAllowed] MLCGraphCompletionHandler completionHandler);

		[Export ("synchronizeUpdates")]
		void SynchronizeUpdates ();

		[Export ("setTrainingTensorParameters:")]
		bool SetTrainingTensorParameters (MLCTensorParameter [] parameters);

		[Export ("gradientTensorForInput:")]
		[return: NullAllowed]
		MLCTensor GetGradientTensor (MLCTensor input);

		[iOS (14, 2)]
		[TV (14, 2)]
		[MacCatalyst (14, 2)]
		[Export ("bindOptimizerData:deviceData:withTensor:")]
		bool BindOptimizer (MLCTensorData [] data, [NullAllowed] MLCTensorOptimizerDeviceData [] deviceData, MLCTensor tensor);
	}

	[TV (14, 0), iOS (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (MLCGraph))]
	[DisableDefaultCtor]
	interface MLCInferenceGraph {

		[Export ("deviceMemorySize")]
		nuint DeviceMemorySize { get; }

		[Static]
		[Export ("graphWithGraphObjects:")]
		MLCInferenceGraph Create (MLCGraph [] graphObjects);

		[Export ("addInputs:")]
		bool AddInputs (NSDictionary<NSString, MLCTensor> inputs);

		[Export ("addInputs:lossLabels:lossLabelWeights:")]
		bool AddInputs (NSDictionary<NSString, MLCTensor> inputs, [NullAllowed] NSDictionary<NSString, MLCTensor> lossLabels, [NullAllowed] NSDictionary<NSString, MLCTensor> lossLabelWeights);

		[Export ("addOutputs:")]
		bool AddOutputs (NSDictionary<NSString, MLCTensor> outputs);

		[Export ("compileWithOptions:device:")]
		bool Compile (MLCGraphCompilationOptions options, MLCDevice device);

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("compileWithOptions:device:inputTensors:inputTensorsData:")]
		bool Compile (MLCGraphCompilationOptions options, MLCDevice device, [NullAllowed] NSDictionary<NSString, MLCTensor> inputTensors, [NullAllowed] NSDictionary<NSString, MLCTensorData> inputTensorsData);

		[Export ("linkWithGraphs:")]
		bool Link (MLCInferenceGraph [] graphs);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeWithInputsData:batchSize:options:completionHandler:")]
		bool Execute (NSDictionary<NSString, MLCTensorData> inputsData, nuint batchSize, MLCExecutionOptions options, [NullAllowed] MLCGraphCompletionHandler completionHandler);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeWithInputsData:outputsData:batchSize:options:completionHandler:")]
		bool Execute (NSDictionary<NSString, MLCTensorData> inputsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> outputsData, nuint batchSize, MLCExecutionOptions options, [NullAllowed] MLCGraphCompletionHandler completionHandler);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeWithInputsData:lossLabelsData:lossLabelWeightsData:batchSize:options:completionHandler:")]
		bool Execute (NSDictionary<NSString, MLCTensorData> inputsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> lossLabelsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> lossLabelWeightsData, nuint batchSize, MLCExecutionOptions options, [NullAllowed] MLCGraphCompletionHandler completionHandler);

		[Async (ResultTypeName = "MLCGraphCompletionResult")]
		[Export ("executeWithInputsData:lossLabelsData:lossLabelWeightsData:outputsData:batchSize:options:completionHandler:")]
		bool Execute (NSDictionary<NSString, MLCTensorData> inputsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> lossLabelsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> lossLabelWeightsData, [NullAllowed] NSDictionary<NSString, MLCTensorData> outputsData, nuint batchSize, MLCExecutionOptions options, [NullAllowed] MLCGraphCompletionHandler completionHandler);
	}

	[TV (14, 5)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	enum MLCComparisonOperation {
		Equal = 0,
		NotEqual = 1,
		Less = 2,
		Greater = 3,
		LessOrEqual = 4,
		GreaterOrEqual = 5,
		LogicalAnd = 6,
		LogicalOr = 7,
		LogicalNot = 8,
		LogicalNand = 9,
		LogicalNor = 10,
		LogicalXor = 11,
	}

	[TV (14, 5)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCComparisonLayer {

		[Export ("operation")]
		MLCComparisonOperation Operation { get; }

		[Static]
		[Export ("layerWithOperation:")]
		MLCComparisonLayer Create (MLCComparisonOperation operation);
	}

	[TV (14, 5)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCGatherLayer {

		[Export ("dimension")]
		nuint Dimension { get; }

		[Static]
		[Export ("layerWithDimension:")]
		MLCGatherLayer Create (nuint dimension);
	}

	[TV (14, 5)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCScatterLayer {

		[Export ("dimension")]
		nuint Dimension { get; }

		[Export ("reductionType")]
		MLCReductionType ReductionType { get; }

		[Static]
		[Export ("layerWithDimension:reductionType:")]
		[return: NullAllowed]
		MLCScatterLayer Create (nuint dimension, MLCReductionType reductionType);
	}

	[TV (14, 5)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (MLCLayer))]
	[DisableDefaultCtor]
	interface MLCSelectionLayer {

		[Static]
		[Export ("layer")]
		MLCSelectionLayer Create ();
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLCPlatform {

		[Static]
		[Export ("setRNGSeedTo:")]
		void SetRngSeed ([BindAs (typeof (nuint))] NSNumber seed);

		[return: BindAs (typeof (nuint)), NullAllowed]
		[Static]
		[Export ("getRNGseed")]
		NSNumber GetRngSeed ();
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (MLCOptimizer))]
	[DisableDefaultCtor]
	interface MLCAdamWOptimizer : NSCopying {
		[Export ("beta1")]
		float Beta1 { get; }

		[Export ("beta2")]
		float Beta2 { get; }

		[Export ("epsilon")]
		float Epsilon { get; }

		[Export ("usesAMSGrad")]
		bool UsesAmsGrad { get; }

		[Export ("timeStep")]
		nuint TimeStep { get; }

		[Static]
		[Export ("optimizerWithDescriptor:")]
		MLCAdamWOptimizer GetOptimizer (MLCOptimizerDescriptor optimizerDescriptor);

		[Static]
		[Export ("optimizerWithDescriptor:beta1:beta2:epsilon:usesAMSGrad:timeStep:")]
		MLCAdamWOptimizer GetOptimizer (MLCOptimizerDescriptor optimizerDescriptor, float beta1, float beta2, float epsilon, bool usesAmsGrad, nuint timeStep);
	}

}
