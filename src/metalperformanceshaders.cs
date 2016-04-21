using System;
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
		MPSImageHistogramInfo HistogramInfo { get; }

		// Could not initialize an instance of the type 'MetalPerformanceShaders.MPSImageHistogram': the native 'initWithDevice:' method returned nil.
//		[Export ("initWithDevice:")]
//		IntPtr Constructor (IMTLDevice device);

		[Export ("initWithDevice:histogramInfo:")]
		[DesignatedInitializer]
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
		IntPtr Constructor (IMTLDevice device, ref MPSImageHistogramInfo histogramInfo);

		[Export ("histogramInfo")]
		MPSImageHistogramInfo HistogramInfo { get; }

		[Export ("encodeTransformToCommandBuffer:sourceTexture:histogram:histogramOffset:")]
		void EncodeTransformToCommandBuffer (IMTLCommandBuffer commandBuffer, IMTLTexture source, IMTLBuffer histogram, nuint histogramOffset);
	}

	[iOS (9,0)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageHistogramSpecification {
		[Export ("initWithDevice:histogramInfo:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, ref MPSImageHistogramInfo histogramInfo);

		[Export ("histogramInfo")]
		MPSImageHistogramInfo HistogramInfo { get; }

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
	interface MPSKernel : NSCopying {
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
}
