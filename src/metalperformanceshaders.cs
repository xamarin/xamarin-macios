using System;
using XamCore.CoreGraphics;
using XamCore.Foundation;
using XamCore.Metal;
using XamCore.ObjCRuntime;

namespace XamCore.MetalPerformanceShaders {

	// MPSImageConvolution.h

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageConvolution {
		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("bias")]
		float Bias { get; set; }

		[Export ("initWithDevice:kernelWidth:kernelHeight:weights:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, IntPtr kernelWeights);

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageLaplacian {

		[Export ("bias")]
		float Bias { get; set; }

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageBox {
		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("initWithDevice:kernelWidth:kernelHeight:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - You must use initWithDevice:kernelWidth:kernelHeight: instead.
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSImageBox))]
	[DisableDefaultCtor]
	interface MPSImageTent {

		// inlining .ctor from base class

		[Export ("initWithDevice:kernelWidth:kernelHeight:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight);
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageGaussianBlur {
		[Export ("initWithDevice:sigma:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, float sigma);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - You must use initWithDevice:sigma: instead.

		[Export ("sigma")]
		float Sigma { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageSobel {
		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);

		[Export ("initWithDevice:linearGrayColorTransform:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, IntPtr /* float* */ transform);

		[Export ("colorTransform")]
		[Internal]
		IntPtr /* float* */ _ColorTransform { get; }
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImagePyramid {

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);

		[Export ("initWithDevice:centerWeight:")]
		IntPtr Constructor (IMTLDevice device, float centerWeight);

		[Export ("initWithDevice:kernelWidth:kernelHeight:weights:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, /* float* */ IntPtr kernelWeights);

		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSImagePyramid))]
	[DisableDefaultCtor]
	interface MPSImageGaussianPyramid {

		// inlining .ctor from base class

		[Export ("initWithDevice:kernelWidth:kernelHeight:weights:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, /* float* */ IntPtr kernelWeights);
	}

	// MPSImageHistogram.h

	[iOS (9,0)]
	[BaseType (typeof (MPSKernel))]
	[DisableDefaultCtor]
	interface MPSImageHistogram {
		[Export ("clipRectSource", ArgumentSemantic.Assign)]
		MTLRegion ClipRectSource { get; set; }

		[Export ("zeroHistogram")]
		bool ZeroHistogram { get; set; }

		[Export ("histogramInfo")]
		MPSImageHistogramInfo HistogramInfo {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")] get;
		}

		// Could not initialize an instance of the type 'MetalPerformanceShaders.MPSImageHistogram': the native 'initWithDevice:' method returned nil.
//		[Export ("initWithDevice:")]
//		IntPtr Constructor (IMTLDevice device);

		[Export ("initWithDevice:histogramInfo:")]
		[DesignatedInitializer]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr Constructor (IMTLDevice device, ref MPSImageHistogramInfo histogramInfo);

		[Export ("encodeToCommandBuffer:sourceTexture:histogram:histogramOffset:")]
		void EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, IMTLTexture source, IMTLBuffer histogram, nuint histogramOffset);

		[Export ("histogramSizeForSourceFormat:")]
		nuint HistogramSizeForSourceFormat (MTLPixelFormat sourceFormat);		
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageHistogramEqualization {
		[Export ("initWithDevice:histogramInfo:")]
		[DesignatedInitializer]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr Constructor (IMTLDevice device, ref MPSImageHistogramInfo histogramInfo);

		[Export ("histogramInfo")]
		MPSImageHistogramInfo HistogramInfo {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")] get;
		}

		[Export ("encodeTransformToCommandBuffer:sourceTexture:histogram:histogramOffset:")]
		void EncodeTransformToCommandBuffer (IMTLCommandBuffer commandBuffer, IMTLTexture source, IMTLBuffer histogram, nuint histogramOffset);
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageHistogramSpecification {
		[Export ("initWithDevice:histogramInfo:")]
		[DesignatedInitializer]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr Constructor (IMTLDevice device, ref MPSImageHistogramInfo histogramInfo);

		[Export ("histogramInfo")]
		MPSImageHistogramInfo HistogramInfo {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")] get;
		}

		[Export ("encodeTransformToCommandBuffer:sourceTexture:sourceHistogram:sourceHistogramOffset:desiredHistogram:desiredHistogramOffset:")]
		void EncodeTransformToCommandBuffer (IMTLCommandBuffer commandBuffer, IMTLTexture source, IMTLBuffer sourceHistogram, nuint sourceHistogramOffset, IMTLBuffer desiredHistogram, nuint desiredHistogramOffset);
	}

	// MPSImageIntegral.h

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageIntegral {
		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageIntegralOfSquares {
		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);
	}

	// MPSImageKernel.h

	[iOS (9,0)]
	[BaseType (typeof (MPSKernel))]
	[DisableDefaultCtor]
	interface MPSUnaryImageKernel {
		[Export ("offset", ArgumentSemantic.Assign)]
		MPSOffset Offset { get; set; }

		[Export ("clipRect", ArgumentSemantic.Assign)]
		MTLRegion ClipRect { get; set; }

		[Export ("edgeMode", ArgumentSemantic.Assign)]
		MPSImageEdgeMode EdgeMode { get; set; }

		[Export ("encodeToCommandBuffer:inPlaceTexture:fallbackCopyAllocator:")]
		bool EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, out NSObject /* IMTLTexture */ texture, [NullAllowed] MPSCopyAllocator copyAllocator);
		// FIXME: can't use IMTLTexture now

		[Export ("encodeToCommandBuffer:sourceTexture:destinationTexture:")]
		void EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, IMTLTexture sourceTexture, IMTLTexture destinationTexture);

		[Export ("sourceRegionForDestinationSize:")]
		MPSRegion SourceRegionForDestinationSize (MTLSize destinationSize);

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSKernel))]
	[DisableDefaultCtor]
	interface MPSBinaryImageKernel {
		[Export ("primaryOffset", ArgumentSemantic.Assign)]
		MPSOffset PrimaryOffset { get; set; }

		[Export ("secondaryOffset", ArgumentSemantic.Assign)]
		MPSOffset SecondaryOffset { get; set; }

		[Export ("primaryEdgeMode", ArgumentSemantic.Assign)]
		MPSImageEdgeMode PrimaryEdgeMode { get; set; }

		[Export ("secondaryEdgeMode", ArgumentSemantic.Assign)]
		MPSImageEdgeMode SecondaryEdgeMode { get; set; }

		[Export ("clipRect", ArgumentSemantic.Assign)]
		MTLRegion ClipRect { get; set; }

		[Export ("encodeToCommandBuffer:primaryTexture:inPlaceSecondaryTexture:fallbackCopyAllocator:")]
		bool EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, IMTLTexture primaryTexture, out NSObject /* IMTLTexture */ inPlaceSecondaryTexture, [NullAllowed] MPSCopyAllocator copyAllocator);
		// FIXME: can't use IMTLTexture now

		[Export ("encodeToCommandBuffer:inPlacePrimaryTexture:secondaryTexture:fallbackCopyAllocator:")]
		bool EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, out NSObject /* MTLTexture */ inPlacePrimaryTexture, IMTLTexture secondaryTexture, [NullAllowed] MPSCopyAllocator copyAllocator);
		// FIXME: can't use IMTLTexture now

		[Export ("encodeToCommandBuffer:primaryTexture:secondaryTexture:destinationTexture:")]
		void EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, IMTLTexture primaryTexture, IMTLTexture secondaryTexture, IMTLTexture destinationTexture);

		[Export ("primarySourceRegionForDestinationSize:")]
		MPSRegion PrimarySourceRegionForDestinationSize (MTLSize destinationSize);

		[Export ("secondarySourceRegionForDestinationSize:")]
		MPSRegion SecondarySourceRegionForDestinationSize (MTLSize destinationSize);

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);
	}

	// MPSImageMedian.h

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageMedian {
		[Export ("kernelDiameter")]
		nuint KernelDiameter { get; }

		[Export ("initWithDevice:kernelDiameter:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelDiameter);

		// [Export ("initWithDevice:")] is NS_UNAVAILABLE - You must use initWithDevice:kernelDiameter: instead.

		[Static]
		[Export ("maxKernelDiameter")]
		nuint MaxKernelDiameter { get; }

		[Static]
		[Export ("minKernelDiameter")]
		nuint MinKernelDiameter { get; }
	}

	// MPSImageMorphology.h

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageAreaMax {
		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("initWithDevice:kernelWidth:kernelHeight:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight);

		// [Export ("initWithDevice:")] is NS_UNAVAILABLE - You must use initWithDevice:kernelWidth:kernelHeight: instead.
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSImageAreaMax))]
	[DisableDefaultCtor]
	interface MPSImageAreaMin {
		// inlining ctor from base class

		[Export ("initWithDevice:kernelWidth:kernelHeight:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight);
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageDilate {
		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("initWithDevice:kernelWidth:kernelHeight:values:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, IntPtr values);

		// [Export ("initWithDevice:")] is NS_UNAVAILABLE - You must use initWithDevice:kernelWidth:kernelHeight:values: instead. instead.
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSImageDilate))]
	[DisableDefaultCtor]
	interface MPSImageErode {

		// inlining ctor from base class -> done in manual bindings (wrt float* argument)
	}

	// MPSImageResampling.h

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageLanczosScale {
		// scaleTransform property should be like:
		// unsafe MPSScaleTransform* ScaleTransform { get; set; }
		// which is both ugly and not supported by the generator
		[Export ("scaleTransform")]
		[Internal]
		IntPtr _GetScaleTransform ();

		[Export ("setScaleTransform:")]
		[Internal]
		void _SetScaleTransform (IntPtr value);

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);
	}

	// MPSImageThreshold.h

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageThresholdBinary {
		[Export ("initWithDevice:thresholdValue:maximumValue:linearGrayColorTransform:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, float thresholdValue, float maximumValue, /* [NullAllowed] float* */ IntPtr transform);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - You must use initWithDevice:thresholdValue:maximumValue:linearGrayColorTransform: instead

		[Export ("thresholdValue")]
		float ThresholdValue { get; }

		[Export ("maximumValue")]
		float MaximumValue { get; }

		[Export ("transform")]
		[Internal]
		IntPtr _Transform { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageThresholdBinaryInverse {
		[Export ("initWithDevice:thresholdValue:maximumValue:linearGrayColorTransform:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, float thresholdValue, float maximumValue, /* [NullAllowed] float* */ IntPtr transform);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - You must use initWithDevice:thresholdValue:maximumValue:linearGrayColorTransform: instead

		[Export ("thresholdValue")]
		float ThresholdValue { get; }

		[Export ("maximumValue")]
		float MaximumValue { get; }

		[Export ("transform")]
		[Internal]
		IntPtr _Transform { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageThresholdTruncate {
		[Export ("initWithDevice:thresholdValue:linearGrayColorTransform:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, float thresholdValue, /* [NullAllowed] float* */ IntPtr transform);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - You must use initWithDevice:thresholdValue:linearGrayColorTransform: instead

		[Export ("thresholdValue")]
		float ThresholdValue { get; }

		[Export ("transform")]
		[Internal]
		IntPtr _Transform { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageThresholdToZero {
		[Export ("initWithDevice:thresholdValue:linearGrayColorTransform:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, float thresholdValue, /* [NullAllowed] float* */ IntPtr transform);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - You must use initWithDevice:thresholdValue:linearGrayColorTransform: instead

		[Export ("thresholdValue")]
		float ThresholdValue { get; }

		[Export ("transform")]
		[Internal]
		IntPtr _Transform { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageThresholdToZeroInverse {
		[Export ("initWithDevice:thresholdValue:linearGrayColorTransform:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, float thresholdValue, /* [NullAllowed] float* */ IntPtr transform);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - You must use initWithDevice:thresholdValue:linearGrayColorTransform: instead

		[Export ("thresholdValue")]
		float ThresholdValue { get; }

		[Export ("transform")]
		[Internal]
		IntPtr _Transform { get; }
	}

	// MPSKernel.h

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSKernel : NSCopying, NSSecureCoding {
		[Export ("options", ArgumentSemantic.Assign)]
		MPSKernelOptions Options { get; set; }

		[Export ("device", ArgumentSemantic.Retain)]
		IMTLDevice Device { get; }

		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[Export ("copyWithZone:device:")]
		MPSKernel CopyWithZone ([NullAllowed] NSZone zone, [NullAllowed] IMTLDevice device);
	}

	// MPSImageTranspose.h

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageTranspose {

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);
	}

	// MPSCNN.h

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSKernel), Name = "MPSCNNKernel")]
	[Abstract]
	[DisableDefaultCtor]
	interface MPSCnnKernel {
		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);

		[Export ("offset", ArgumentSemantic.Assign)]
		MPSOffset Offset { get; set; }

		[Export ("clipRect", ArgumentSemantic.Assign)]
		MTLRegion ClipRect { get; set; }

		[Export ("destinationFeatureChannelOffset")]
		nuint DestinationFeatureChannelOffset { get; set; }

		[Export ("edgeMode", ArgumentSemantic.Assign)]
		MPSImageEdgeMode EdgeMode { get; set; }

		[Export ("encodeToCommandBuffer:sourceImage:destinationImage:")]
		void EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, MPSImage sourceImage, MPSImage destinationImage);

		[Export ("sourceRegionForDestinationSize:")]
		MPSRegion GetSourceRegion (MTLSize destinationSize);
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNNeuron")]
	[Abstract] // You must use one of the sub-classes of MPSCNNNeuron
	[DisableDefaultCtor]
	interface MPSCnnNeuron {
		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnNeuron), Name = "MPSCNNNeuronLinear")]
	[DisableDefaultCtor]
	interface MPSCnnNeuronLinear {

		[Export ("a")]
		float A { get; }

		[Export ("b")]
		float B { get; }

		[Export ("initWithDevice:a:b:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, float a, float b);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - You must use initWithDevice:a:b instead
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnNeuron), Name = "MPSCNNNeuronReLU")]
	[DisableDefaultCtor]
	interface MPSCnnNeuronReLU {

		[Export ("a")]
		float A { get; }

		[Export ("initWithDevice:a:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, float a);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - Use initWithDevice:a: instead
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnNeuron), Name = "MPSCNNNeuronSigmoid")]
	[DisableDefaultCtor]
	interface MPSCnnNeuronSigmoid {

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnNeuron), Name = "MPSCNNNeuronTanH")]
	[DisableDefaultCtor]
	interface MPSCnnNeuronTanH {

		[Export ("a")]
		float A { get; }

		[Export ("b")]
		float B { get; }

		[Export ("initWithDevice:a:b:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, float a, float b);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - Use initWithDevice:a:b: instead
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnNeuron), Name = "MPSCNNNeuronAbsolute")]
	[DisableDefaultCtor]
	interface MPSCnnNeuronAbsolute {

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (NSObject), Name = "MPSCNNConvolutionDescriptor")]
	[DisableDefaultCtor]
	interface MPSCnnConvolutionDescriptor : NSCopying, NSSecureCoding {

		[Export ("kernelWidth")]
		nuint KernelWidth { get; set; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; set; }

		[Export ("inputFeatureChannels")]
		nuint InputFeatureChannels { get; set; }

		[Export ("outputFeatureChannels")]
		nuint OutputFeatureChannels { get; set; }

		[Export ("strideInPixelsX")]
		nuint StrideInPixelsX { get; set; }

		[Export ("strideInPixelsY")]
		nuint StrideInPixelsY { get; set; }

		[Export ("groups")]
		nuint Groups { get; set; }

		[NullAllowed, Export ("neuron", ArgumentSemantic.Retain)]
		MPSCnnNeuron Neuron { get; set; }

		[Static]
		[Export ("cnnConvolutionDescriptorWithKernelWidth:kernelHeight:inputFeatureChannels:outputFeatureChannels:neuronFilter:")]
		MPSCnnConvolutionDescriptor GetConvolutionDescriptor (nuint kernelWidth, nuint kernelHeight, nuint inputFeatureChannels, nuint outputFeatureChannels, [NullAllowed] MPSCnnNeuron neuronFilter);
	}

	[iOS (10,0)][TV (10,0)]
	[Native][Flags]
	public enum MPSCnnConvolutionFlags : nuint {
		None = 0
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNConvolution")]
	[DisableDefaultCtor]
	interface MPSCnnConvolution {

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("inputFeatureChannels")]
		nuint InputFeatureChannels { get; }

		[Export ("outputFeatureChannels")]
		nuint OutputFeatureChannels { get; }

		[Export ("strideInPixelsX")]
		nuint StrideInPixelsX { get; }

		[Export ("strideInPixelsY")]
		nuint StrideInPixelsY { get; }

		[Export ("groups")]
		nuint Groups { get; }

		[NullAllowed, Export ("neuron")]
		MPSCnnNeuron Neuron { get; }

		[Export ("initWithDevice:convolutionDescriptor:kernelWeights:biasTerms:flags:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, MPSCnnConvolutionDescriptor convolutionDescriptor, /* float* */ IntPtr kernelWeights, [NullAllowed] /* float* */ IntPtr biasTerms, MPSCnnConvolutionFlags flags);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - Use initWithDevice:convolutionDescriptor:kernelWeights:biasTerms instead
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnConvolution), Name = "MPSCNNFullyConnected")]
	[DisableDefaultCtor]
	interface MPSCnnFullyConnected {
		
		[Export ("initWithDevice:convolutionDescriptor:kernelWeights:biasTerms:flags:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, MPSCnnConvolutionDescriptor fullyConnectedDescriptor, /* float* */ IntPtr kernelWeights, [NullAllowed] /* float* */ IntPtr biasTerms, MPSCnnConvolutionFlags flags);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - Use initWithDevice:convolutionDescriptor:kernelWeights:biasTerms instead
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNPooling")]
	[DisableDefaultCtor]
	interface MPSCnnPooling {

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("strideInPixelsX")]
		nuint StrideInPixelsX { get; }

		[Export ("strideInPixelsY")]
		nuint StrideInPixelsY { get; }

		[Export ("initWithDevice:kernelWidth:kernelHeight:")]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight);

		[Export ("initWithDevice:kernelWidth:kernelHeight:strideInPixelsX:strideInPixelsY:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, nuint strideInPixelsX, nuint strideInPixelsY);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - Use initWithDevice:kernelWidth:kernelHeight:strideInPixelsX:strideInPixelsY: instead
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnPooling), Name = "MPSCNNPoolingMax")]
	[DisableDefaultCtor]
	interface MPSCnnPoolingMax {

		// inlining .ctor from base class

		[Export ("initWithDevice:kernelWidth:kernelHeight:strideInPixelsX:strideInPixelsY:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, nuint strideInPixelsX, nuint strideInPixelsY);
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnPooling), Name = "MPSCNNPoolingAverage")]
	[DisableDefaultCtor]
	interface MPSCnnPoolingAverage {

		// inlining .ctor from base class

		[Export ("initWithDevice:kernelWidth:kernelHeight:strideInPixelsX:strideInPixelsY:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, nuint strideInPixelsX, nuint strideInPixelsY);
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNSpatialNormalization")]
	[DisableDefaultCtor]
	interface MPSCnnSpatialNormalization {

		[Export ("alpha")]
		float Alpha { get; set; }

		[Export ("beta")]
		float Beta { get; set; }

		[Export ("delta")]
		float Delta { get; set; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("initWithDevice:kernelWidth:kernelHeight:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - Use initWithDevice:kernelWidth:kernelHeight instead
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNLocalContrastNormalization")]
	[DisableDefaultCtor]
	interface MPSCnnLocalContrastNormalization {

		[Export ("alpha")]
		float Alpha { get; set; }

		[Export ("beta")]
		float Beta { get; set; }

		[Export ("delta")]
		float Delta { get; set; }

		[Export ("p0")]
		float P0 { get; set; }

		[Export ("pm")]
		float Pm { get; set; }

		[Export ("ps")]
		float Ps { get; set; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("initWithDevice:kernelWidth:kernelHeight:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - Use initWithDevice:kernelWidth:kernelHeight instead
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNCrossChannelNormalization")]
	[DisableDefaultCtor]
	interface MPSCnnCrossChannelNormalization {

		[Export ("alpha")]
		float Alpha { get; set; }

		[Export ("beta")]
		float Beta { get; set; }

		[Export ("delta")]
		float Delta { get; set; }

		[Export ("kernelSize")]
		nuint KernelSize { get; }

		[Export ("initWithDevice:kernelSize:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelSize);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - Use initWithDevice:kernelSize: instead
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNSoftMax")]
	[DisableDefaultCtor]
	interface MPSCnnSoftMax {

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNLogSoftMax")]
	[DisableDefaultCtor]
	interface MPSCnnLogSoftMax {

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	// MPSImage.h

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSImageDescriptor {

		[Export ("width")]
		nuint Width { get; set; }

		[Export ("height")]
		nuint Height { get; set; }

		[Export ("featureChannels")]
		nuint FeatureChannels { get; set; }

		[Export ("numberOfImages")]
		nuint NumberOfImages { get; set; }

		[Export ("pixelFormat")]
		MTLPixelFormat PixelFormat { get; }

		[Export ("channelFormat", ArgumentSemantic.Assign)]
		MPSImageFeatureChannelFormat ChannelFormat { get; set; }

		[Export ("cpuCacheMode", ArgumentSemantic.Assign)]
		MTLCpuCacheMode CpuCacheMode { get; set; }

		[Export ("storageMode", ArgumentSemantic.Assign)]
		MTLStorageMode StorageMode { get; set; }

		[Export ("usage", ArgumentSemantic.Assign)]
		MTLTextureUsage Usage { get; set; }

		[Static]
		[Export ("imageDescriptorWithChannelFormat:width:height:featureChannels:")]
		MPSImageDescriptor GetImageDescriptor (MPSImageFeatureChannelFormat channelFormat, nuint width, nuint height, nuint featureChannels);

		[Static]
		[Export ("imageDescriptorWithChannelFormat:width:height:featureChannels:numberOfImages:usage:")]
		MPSImageDescriptor GetImageDescriptor (MPSImageFeatureChannelFormat channelFormat, nuint width, nuint height, nuint featureChannels, nuint numberOfImages, MTLTextureUsage usage);
	}

	[iOS (10,0)][TV (10,0)]
	[Native][Flags]
	public enum MPSPurgeableState : nuint {
		AllocationDeferred = 0,
		KeepCurrent = MTLPurgeableState.KeepCurrent,
		NonVolatile = MTLPurgeableState.NonVolatile,
		Volatile = MTLPurgeableState.Volatile,
		Empty = MTLPurgeableState.Empty
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSImage {
		[Export ("device", ArgumentSemantic.Retain)]
		IMTLDevice Device { get; }

		[Export ("width")]
		nuint Width { get; }

		[Export ("height")]
		nuint Height { get; }

		[Export ("featureChannels")]
		nuint FeatureChannels { get; }

		[Export ("numberOfImages")]
		nuint NumberOfImages { get; }

		[Export ("textureType")]
		MTLTextureType TextureType { get; }

		[Export ("pixelFormat")]
		MTLPixelFormat PixelFormat { get; }

		[Export ("precision")]
		nuint Precision { get; }

		[Export ("usage")]
		MTLTextureUsage Usage { get; }

		[Export ("pixelSize")]
		nuint PixelSize { get; }

		[Export ("texture")]
		IMTLTexture Texture { get; }

		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[Export ("initWithDevice:imageDescriptor:")]
		IntPtr Constructor (IMTLDevice device, MPSImageDescriptor imageDescriptor);

		[Export ("initWithTexture:featureChannels:")]
		IntPtr Constructor (IMTLTexture texture, nuint featureChannels);

		[Export ("setPurgeableState:")]
		MPSPurgeableState SetPurgeableState (MPSPurgeableState state);
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSImage))]
	[DisableDefaultCtor]
	interface MPSTemporaryImage {

		[Static]
		[Export ("temporaryImageWithCommandBuffer:imageDescriptor:")]
		MPSTemporaryImage GetTemporaryImage (IMTLCommandBuffer commandBuffer, MPSImageDescriptor imageDescriptor);

		[Static]
		[Export ("temporaryImageWithCommandBuffer:textureDescriptor:")]
		MPSTemporaryImage GetTemporaryImage (IMTLCommandBuffer commandBuffer, MTLTextureDescriptor textureDescriptor);

		[Static]
		[Export ("prefetchStorageWithCommandBuffer:imageDescriptorList:")]
		void PrefetchStorage (IMTLCommandBuffer commandBuffer, MPSImageDescriptor[] descriptorList);

		[Export ("readCount")]
		nuint ReadCount { get; set; }
	}

	// MPSImageConversion.h

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageConversion {

		[Export ("sourceAlpha")]
		MPSAlphaType SourceAlpha { get; }

		[Export ("destinationAlpha")]
		MPSAlphaType DestinationAlpha { get; }

		[Export ("initWithDevice:srcAlpha:destAlpha:backgroundColor:conversionInfo:")]
		[Internal]
		IntPtr Constructor (IMTLDevice device, MPSAlphaType srcAlpha, MPSAlphaType destAlpha, [NullAllowed] /* nfloat* */ IntPtr backgroundColor, [NullAllowed] CGColorConversionInfo conversionInfo);
	}

	// MPSMatrix.h

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSMatrixDescriptor {

		[Export ("rows")]
		nuint Rows { get; set; }

		[Export ("columns")]
		nuint Columns { get; set; }

		[Export ("dataType", ArgumentSemantic.Assign)]
		MPSDataType DataType { get; set; }

		[Export ("rowBytes")]
		nuint RowBytes { get; set; }

		[Static]
		[Export ("matrixDescriptorWithDimensions:columns:rowBytes:dataType:")]
		MPSMatrixDescriptor Create (nuint rows, nuint columns, nuint rowBytes, MPSDataType dataType);

		[Static]
		[Export ("rowBytesFromColumns:dataType:")]
		nuint GetRowBytes (nuint columns, MPSDataType dataType);
	}

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // init NS_UNAVAILABLE;
	interface MPSMatrix {

		[Export ("device", ArgumentSemantic.Retain)]
		IMTLDevice Device { get; }

		[Export ("rows")]
		nuint Rows { get; }

		[Export ("columns")]
		nuint Columns { get; }

		[Export ("dataType")]
		MPSDataType DataType { get; }

		[Export ("rowBytes")]
		nuint RowBytes { get; }

		[Export ("data")]
		IMTLBuffer Data { get; }

		[Export ("initWithBuffer:descriptor:")]
		IntPtr Constructor (IMTLBuffer buffer, MPSMatrixDescriptor descriptor);
	}

	// MPSMatrixMultiplication.h

	[iOS (10,0)][TV (10,0)]
	[BaseType (typeof (MPSKernel))]
	[DisableDefaultCtor]
	interface MPSMatrixMultiplication {

		[Export ("resultMatrixOrigin", ArgumentSemantic.Assign)]
		MTLOrigin ResultMatrixOrigin { get; set; }

		[Export ("leftMatrixOrigin", ArgumentSemantic.Assign)]
		MTLOrigin LeftMatrixOrigin { get; set; }

		[Export ("rightMatrixOrigin", ArgumentSemantic.Assign)]
		MTLOrigin RightMatrixOrigin { get; set; }

		[Export ("initWithDevice:transposeLeft:transposeRight:resultRows:resultColumns:interiorColumns:alpha:beta:")]
		IntPtr Constructor (IMTLDevice device, bool transposeLeft, bool transposeRight, nuint resultRows, nuint resultColumns, nuint interiorColumns, double alpha, double beta);

		[Export ("encodeToCommandBuffer:leftMatrix:rightMatrix:resultMatrix:")]
		void EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, MPSMatrix leftMatrix, MPSMatrix rightMatrix, MPSMatrix resultMatrix);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - Use the above initialization method instead.
	}
}
