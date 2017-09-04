#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.CoreGraphics;
using XamCore.Foundation;
using XamCore.Metal;
using XamCore.ObjCRuntime;
using Vector4 = global::OpenTK.Vector4;

namespace XamCore.MetalPerformanceShaders {
	// MPSImageConvolution.h

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageLaplacian {

		[Export ("bias")]
		float Bias { get; set; }

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageBox {
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("initWithDevice:kernelWidth:kernelHeight:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - You must use initWithDevice:kernelWidth:kernelHeight: instead.
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSImageBox))]
	[DisableDefaultCtor]
	interface MPSImageTent {

		// inlining .ctor from base class

		[Export ("initWithDevice:kernelWidth:kernelHeight:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageGaussianBlur {
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[Export ("initWithDevice:sigma:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, float sigma);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - You must use initWithDevice:sigma: instead.

		[Export ("sigma")]
		float Sigma { get; }
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageSobel {
		// inlining .ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

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

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImagePyramid {
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

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

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSImagePyramid))]
	[DisableDefaultCtor]
	interface MPSImageGaussianPyramid {

		// inlining .ctor from base class

		[Export ("initWithDevice:kernelWidth:kernelHeight:weights:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, /* float* */ IntPtr kernelWeights);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	// MPSImageHistogram.h

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSKernel))]
	[DisableDefaultCtor]
	interface MPSImageHistogram {
		[TV (11,0), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

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

		[Export ("minPixelThresholdValue", ArgumentSemantic.Assign)]
		Vector4 MinPixelThresholdValue {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")] get; 
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")] set; 
		}
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	// MPSImageIntegral.h

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageIntegral {
		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageIntegralOfSquares {
		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	// MPSImageKernel.h

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		[iOS (11,0), TV (11,0)]
		[Export ("encodeToCommandBuffer:sourceImage:destinationImage:")]
		void EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, MPSImage sourceImage, MPSImage destinationImage);

		[Export ("sourceRegionForDestinationSize:")]
		MPSRegion SourceRegionForDestinationSize (MTLSize destinationSize);

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSKernel))]
	[DisableDefaultCtor]
	interface MPSBinaryImageKernel {
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
		
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

		[iOS (11,0), TV (11,0)]
		[Export ("encodeToCommandBuffer:primaryImage:secondaryImage:destinationImage:")]
		void EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, MPSImage primaryImage, MPSImage secondaryImage, MPSImage destinationImage);

		[Export ("primarySourceRegionForDestinationSize:")]
		MPSRegion PrimarySourceRegionForDestinationSize (MTLSize destinationSize);

		[Export ("secondarySourceRegionForDestinationSize:")]
		MPSRegion SecondarySourceRegionForDestinationSize (MTLSize destinationSize);

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	// MPSImageMedian.h

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	// MPSImageMorphology.h

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		// [Export ("initWithDevice:")] is NS_UNAVAILABLE - You must use initWithDevice:kernelWidth:kernelHeight: instead.
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSImageAreaMax))]
	[DisableDefaultCtor]
	interface MPSImageAreaMin {
		// inlining ctor from base class

		[Export ("initWithDevice:kernelWidth:kernelHeight:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		// inlining ctor from base class
		[TV (11,0), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSImageDilate))]
	[DisableDefaultCtor]
	interface MPSImageErode {

		// inlining ctor from base class -> done in manual bindings (wrt float* argument)

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	// MPSImageResampling.h

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	// MPSImageThreshold.h

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
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

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	// MPSKernel.h

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MPSKernel : NSCopying, NSSecureCoding {
		[TV (11,0), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

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

	[iOS (9,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSUnaryImageKernel))]
	[DisableDefaultCtor]
	interface MPSImageTranspose {

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	// MPSCNN.h

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSKernel), Name = "MPSCNNKernel")]
	[Abstract]
	[DisableDefaultCtor]
	interface MPSCnnKernel {
		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
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

		//inlining ctor from base class
		[TV (11,0), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[TV (11, 0), iOS (11, 0)]
		[Export ("isBackwards")]
		bool IsBackwards { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("padding", ArgumentSemantic.Retain)]
		IMPSNnPadding Padding { get; set; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("destinationImageAllocator", ArgumentSemantic.Retain)]
		IMPSImageAllocator DestinationImageAllocator { get; set; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("encodeToCommandBuffer:sourceImage:")]
		MPSImage Encode (IMTLCommandBuffer commandBuffer, MPSImage sourceImage);

		[TV (11, 0), iOS (11, 0)]
		[Export ("kernelWidth")]
		nuint KernelWidth { get; set; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("kernelHeight")]
		nuint KernelHeight { get; set; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("strideInPixelsX")]
		nuint StrideInPixelsX { get; set; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("strideInPixelsY")]
		nuint StrideInPixelsY { get; set; }
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNNeuron")]
	[Abstract] // You must use one of the sub-classes of MPSCNNNeuron
	[DisableDefaultCtor]
	interface MPSCnnNeuron {
		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		IntPtr Constructor (IMTLDevice device);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSCnnNeuron), Name = "MPSCNNNeuronSigmoid")]
	[DisableDefaultCtor]
	interface MPSCnnNeuronSigmoid {

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSCnnNeuron), Name = "MPSCNNNeuronAbsolute")]
	[DisableDefaultCtor]
	interface MPSCnnNeuronAbsolute {

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), iOS (11, 0)]
		[Static]
		[Export ("supportsSecureCoding")]
		bool SupportsSecureCoding { get; }

		[TV (11, 0), iOS (11, 0)]
		[Static]
		[Export ("cnnConvolutionDescriptorWithKernelWidth:kernelHeight:inputFeatureChannels:outputFeatureChannels:")]
		MPSCnnConvolutionDescriptor CreateCnnConvolutionDescriptor (nuint kernelWidth, nuint kernelHeight, nuint inputFeatureChannels, nuint outputFeatureChannels);

		[TV (11, 0), iOS (11, 0)]
		[Internal, Export ("setBatchNormalizationParametersForInferenceWithMean:variance:gamma:beta:epsilon:")]
		void SetBatchNormalizationParameters (IntPtr /* float* */ mean, IntPtr /* float* */ variance, [NullAllowed] IntPtr /* float* */ gamma, [NullAllowed] IntPtr /* float* */ beta, float epsilon);

		[TV (11, 0), iOS (11, 0)]
		[Export ("setNeuronType:parameterA:parameterB:")]
		void SetNeuronType (MPSCnnNeuronType neuronType, float parameterA, float parameterB);

		[TV (11, 0), iOS (11, 0)]
		[Export ("neuronType")]
		MPSCnnNeuronType NeuronType { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("neuronParameterA")]
		float NeuronParameterA { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("neuronParameterB")]
		float NeuronParameterB { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("setNeuronToPReLUWithParametersA:")]
		void SetNeuronToPReLUWithParametersA (NSData A);

		[TV (11, 0), iOS (11, 0)]
		[Export ("dilationRateX")]
		nuint DilationRateX { get; set; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("dilationRateY")]
		nuint DilationRateY { get; set; }

	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[Native][Flags]
	public enum MPSCnnConvolutionFlags : nuint {
		None = 0
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), iOS (11, 0)]
		[Export ("initWithDevice:weights:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, IMPSCnnConvolutionDataSource weights);

		[TV (11, 0), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[TV (11, 0), iOS (11, 0)]
		[Export ("encodeToCommandBuffer:sourceImage:destinationImage:state:")]
		void Encode (IMTLCommandBuffer commandBuffer, MPSImage sourceImage, MPSImage destinationImage, out MPSCnnConvolutionState outState);

		[TV (11, 0), iOS (11, 0)]
		[Export ("dilationRateX")]
		nuint DilationRateX { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("dilationRateY")]
		nuint DilationRateY { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("channelMultiplier")]
		nuint ChannelMultiplier { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("neuronType")]
		MPSCnnNeuronType NeuronType { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("neuronParameterA")]
		float NeuronParameterA { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("neuronParameterB")]
		float NeuronParameterB { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("subPixelScaleFactor")]
		nuint SubPixelScaleFactor { get; }
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSCnnConvolution), Name = "MPSCNNFullyConnected")]
	[DisableDefaultCtor]
	interface MPSCnnFullyConnected {
		
		[Export ("initWithDevice:convolutionDescriptor:kernelWeights:biasTerms:flags:")]
		[DesignatedInitializer]
		[Internal]
		IntPtr Constructor (IMTLDevice device, MPSCnnConvolutionDescriptor fullyConnectedDescriptor, /* float* */ IntPtr kernelWeights, [NullAllowed] /* float* */ IntPtr biasTerms, MPSCnnConvolutionFlags flags);

		// [Export ("initWithDevice:")] marked as NS_UNAVAILABLE - Use initWithDevice:convolutionDescriptor:kernelWeights:biasTerms instead

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("initWithDevice:weights:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, IMPSCnnConvolutionDataSource weights);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), Mac (10, 13, onlyOn64: true), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSCnnPooling), Name = "MPSCNNPoolingMax")]
	[DisableDefaultCtor]
	interface MPSCnnPoolingMax {

		// inlining .ctor from base class

		[Export ("initWithDevice:kernelWidth:kernelHeight:strideInPixelsX:strideInPixelsY:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, nuint strideInPixelsX, nuint strideInPixelsY);

		[TV (11, 0), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSCnnPooling), Name = "MPSCNNPoolingAverage")]
	[DisableDefaultCtor]
	interface MPSCnnPoolingAverage {

		// inlining .ctor from base class

		[Export ("initWithDevice:kernelWidth:kernelHeight:strideInPixelsX:strideInPixelsY:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, nuint strideInPixelsX, nuint strideInPixelsY);

		[TV (11, 0), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[TV (11, 0), iOS (11, 0)]
		[Export ("zeroPadSizeX")]
		nuint ZeroPadSizeX { get; set; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("zeroPadSizeY")]
		nuint ZeroPadSizeY { get; set; }
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), iOS (11, 0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNSoftMax")]
	[DisableDefaultCtor]
	interface MPSCnnSoftMax {

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSCnnKernel), Name = "MPSCNNLogSoftMax")]
	[DisableDefaultCtor]
	interface MPSCnnLogSoftMax {

		// inlining .ctor from base class

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	// MPSImage.h

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[Native][Flags]
	public enum MPSPurgeableState : nuint {
		AllocationDeferred = 0,
		KeepCurrent = MTLPurgeableState.KeepCurrent,
		NonVolatile = MTLPurgeableState.NonVolatile,
		Volatile = MTLPurgeableState.Volatile,
		Empty = MTLPurgeableState.Empty
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11,0), iOS (11,0)]
		[Export ("readBytes:dataLayout:bytesPerRow:region:featureChannelInfo:imageIndex:")]
		void ReadBytes (IntPtr /* void* */ dataBytes, MPSDataLayout dataLayout, nuint bytesPerRow, MTLRegion region, MPSImageReadWriteParams featureChannelInfo, nuint imageIndex);

		[TV (11,0), iOS (11,0)]
		[Export ("writeBytes:dataLayout:bytesPerRow:region:featureChannelInfo:imageIndex:")]
		void WriteBytes (IntPtr /* void* */ dataBytes, MPSDataLayout dataLayout, nuint bytesPerRow, MTLRegion region, MPSImageReadWriteParams featureChannelInfo, nuint imageIndex);

		[TV (11,0), iOS (11,0)]
		[Export ("readBytes:dataLayout:imageIndex:")]
		void ReadBytes (IntPtr /* void* */ dataBytes, MPSDataLayout dataLayout, nuint imageIndex);

		[TV (11,0), iOS (11,0)]
		[Export ("writeBytes:dataLayout:imageIndex:")]
		void WriteBytes (IntPtr /* void* */ dataBytes, MPSDataLayout dataLayout, nuint imageIndex);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	// MPSMatrix.h

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), iOS (11, 0)]
		[Export ("matrices")]
		nuint Matrices { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("matrixBytes")]
		nuint MatrixBytes { get; }

		[TV (11,0), iOS (11,0)]
		[Static]
		[Export ("matrixDescriptorWithRows:columns:rowBytes:dataType:")]
		MPSMatrixDescriptor GetMatrixDescriptor (nuint rows, nuint columns, nuint rowBytes, MPSDataType dataType);

		[TV (11,0), iOS (11,0)]
		[Static]
		[Export ("matrixDescriptorWithRows:columns:matrices:rowBytes:matrixBytes:dataType:")]
		MPSMatrixDescriptor GetMatrixDescriptor (nuint rows, nuint columns, nuint matrices, nuint rowBytes, nuint matrixBytes, MPSDataType dataType);

		[TV (11,0), iOS (11,0)]
		[Static]
		[Export ("rowBytesForColumns:dataType:")]
		nuint GetRowBytesForColumns (nuint columns, MPSDataType dataType);
	}

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
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

		[TV (11, 0), iOS (11, 0)]
		[Export ("matrices")]
		nuint Matrices { get; }

		[TV (11, 0), iOS (11, 0)]
		[Export ("matrixBytes")]
		nuint MatrixBytes { get; }
	}

	// MPSMatrixMultiplication.h

	[iOS (10,0)][TV (10,0)][Mac (10, 13, onlyOn64: true)]
	[BaseType (typeof (MPSKernel))]
	[DisableDefaultCtor]
	interface MPSMatrixMultiplication {
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithDevice:resultRows:resultColumns:interiorColumns:")]
		IntPtr Constructor (IMTLDevice device, nuint resultRows, nuint resultColumns, nuint interiorColumns);

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

		// inlining ctor from base class
		[TV (11,0), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[TV (11,0), iOS (11,0)]
		[Export ("batchStart")]
		nuint BatchStart { get; set; }

		[TV (11,0), iOS (11,0)]
		[Export ("batchSize")]
		nuint BatchSize { get; set; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MPSState
	{
		[Export ("readCount")]
		nuint ReadCount { get; set; }

		[Export ("isTemporary")]
		bool IsTemporary { get; }

		[NullAllowed, Export ("label")]
		string Label { get; set; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSMatrix))]
	interface MPSTemporaryMatrix
	{
		[Static]
		[Export ("temporaryMatrixWithCommandBuffer:matrixDescriptor:")]
		MPSTemporaryMatrix CreateTemporaryMatrix (IMTLCommandBuffer commandBuffer, MPSMatrixDescriptor matrixDescriptor);

		[Static]
		[Export ("prefetchStorageWithCommandBuffer:matrixDescriptorList:")]
		void PrefetchStorage (IMTLCommandBuffer commandBuffer, MPSMatrixDescriptor[] descriptorList);

		[Export ("readCount")]
		nuint ReadCount { get; set; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MPSVector
	{
		[Export ("device", ArgumentSemantic.Retain)]
		IMTLDevice Device { get; }

		[Export ("length")]
		nuint Length { get; }

		[Export ("vectors")]
		nuint Vectors { get; }

		[Export ("dataType")]
		MPSDataType DataType { get; }

		[Export ("vectorBytes")]
		nuint VectorBytes { get; }

		[Export ("data")]
		IMTLBuffer Data { get; }

		[Export ("initWithBuffer:descriptor:")]
		IntPtr Constructor (IMTLBuffer buffer, MPSVectorDescriptor descriptor);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface MPSVectorDescriptor
	{
		[Export ("length")]
		nuint Length { get; set; }

		[Export ("vectors")]
		nuint Vectors { get; }

		[Export ("dataType", ArgumentSemantic.Assign)]
		MPSDataType DataType { get; set; }

		[Export ("vectorBytes")]
		nuint VectorBytes { get; }

		[Static]
		[Export ("vectorDescriptorWithLength:dataType:")]
		MPSVectorDescriptor GetVectorDescriptor (nuint length, MPSDataType dataType);

		[Static]
		[Export ("vectorDescriptorWithLength:vectors:vectorBytes:dataType:")]
		MPSVectorDescriptor GetVectorDescriptor (nuint length, nuint vectors, nuint vectorBytes, MPSDataType dataType);

		[Static]
		[Export ("vectorBytesForLength:dataType:")]
		nuint GetVectorBytes (nuint length, MPSDataType dataType);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSKernel))]
	interface MPSMatrixUnaryKernel
	{
		[Export ("sourceMatrixOrigin", ArgumentSemantic.Assign)]
		MTLOrigin SourceMatrixOrigin { get; set; }

		[Export ("resultMatrixOrigin", ArgumentSemantic.Assign)]
		MTLOrigin ResultMatrixOrigin { get; set; }

		[Export ("batchStart")]
		nuint BatchStart { get; set; }

		[Export ("batchSize")]
		nuint BatchSize { get; set; }

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSKernel))]
	interface MPSMatrixBinaryKernel
	{
		[Export ("primarySourceMatrixOrigin", ArgumentSemantic.Assign)]
		MTLOrigin PrimarySourceMatrixOrigin { get; set; }

		[Export ("secondarySourceMatrixOrigin", ArgumentSemantic.Assign)]
		MTLOrigin SecondarySourceMatrixOrigin { get; set; }

		[Export ("resultMatrixOrigin", ArgumentSemantic.Assign)]
		MTLOrigin ResultMatrixOrigin { get; set; }

		[Export ("batchStart")]
		nuint BatchStart { get; set; }

		[Export ("batchSize")]
		nuint BatchSize { get; set; }

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}
	
	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSMatrixBinaryKernel))]
	interface MPSMatrixVectorMultiplication
	{
		[Export ("initWithDevice:transpose:rows:columns:alpha:beta:")]
		IntPtr Constructor (IMTLDevice device, bool transpose, nuint rows, nuint columns, double alpha, double beta);

		[Export ("initWithDevice:rows:columns:")]
		IntPtr Constructor (IMTLDevice device, nuint rows, nuint columns);

		[Export ("encodeToCommandBuffer:inputMatrix:inputVector:resultVector:")]
		void Encode (IMTLCommandBuffer commandBuffer, MPSMatrix inputMatrix, MPSVector inputVector, MPSVector resultVector);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSMatrixBinaryKernel))]
	interface MPSMatrixSolveTriangular
	{
		[Export ("initWithDevice:right:upper:transpose:unit:order:numberOfRightHandSides:alpha:")]
		IntPtr Constructor (IMTLDevice device, bool right, bool upper, bool transpose, bool unit, nuint order, nuint numberOfRightHandSides, double alpha);

		[Export ("encodeToCommandBuffer:sourceMatrix:rightHandSideMatrix:solutionMatrix:")]
		void Encode (IMTLCommandBuffer commandBuffer, MPSMatrix sourceMatrix, MPSMatrix rightHandSideMatrix, MPSMatrix solutionMatrix);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSMatrixBinaryKernel))]
	interface MPSMatrixSolveLU
	{
		[Export ("initWithDevice:transpose:order:numberOfRightHandSides:")]
		IntPtr Constructor (IMTLDevice device, bool transpose, nuint order, nuint numberOfRightHandSides);

		[Export ("encodeToCommandBuffer:sourceMatrix:rightHandSideMatrix:pivotIndices:solutionMatrix:")]
		void Encode (IMTLCommandBuffer commandBuffer, MPSMatrix sourceMatrix, MPSMatrix rightHandSideMatrix, MPSMatrix pivotIndices, MPSMatrix solutionMatrix);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSMatrixBinaryKernel))]
	interface MPSMatrixSolveCholesky
	{
		[Export ("initWithDevice:upper:order:numberOfRightHandSides:")]
		IntPtr Constructor (IMTLDevice device, bool upper, nuint order, nuint numberOfRightHandSides);

		[Export ("encodeToCommandBuffer:sourceMatrix:rightHandSideMatrix:solutionMatrix:")]
		void Encode (IMTLCommandBuffer commandBuffer, MPSMatrix sourceMatrix, MPSMatrix rightHandSideMatrix, MPSMatrix solutionMatrix);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSMatrixUnaryKernel))]
	interface MPSMatrixDecompositionLU
	{
		[Export ("initWithDevice:rows:columns:")]
		IntPtr Constructor (IMTLDevice device, nuint rows, nuint columns);

		[Export ("encodeToCommandBuffer:sourceMatrix:resultMatrix:pivotIndices:status:")]
		void Encode (IMTLCommandBuffer commandBuffer, MPSMatrix sourceMatrix, MPSMatrix resultMatrix, MPSMatrix pivotIndices, [NullAllowed] IMTLBuffer status);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSMatrixUnaryKernel))]
	interface MPSMatrixDecompositionCholesky
	{
		[Export ("initWithDevice:lower:order:")]
		IntPtr Constructor (IMTLDevice device, bool lower, nuint order);

		[Export ("encodeToCommandBuffer:sourceMatrix:resultMatrix:status:")]
		void Encode (IMTLCommandBuffer commandBuffer, MPSMatrix sourceMatrix, MPSMatrix resultMatrix, [NullAllowed] IMTLBuffer status);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MPSMatrixCopyDescriptor
	{
		[Static]
		[Export ("descriptorWithSourceMatrix:destinationMatrix:offsets:")]
		MPSMatrixCopyDescriptor CreateDescriptor (MPSMatrix sourceMatrix, MPSMatrix destinationMatrix, MPSMatrixCopyOffsets offsets);

		[Export ("initWithDevice:count:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint count);

		[Export ("setCopyOperationAtIndex:sourceMatrix:destinationMatrix:offsets:")]
		void SetCopyOperation (nuint index, MPSMatrix sourceMatrix, MPSMatrix destinationMatrix, MPSMatrixCopyOffsets offsets);

		[Export ("initWithSourceMatrices:destinationMatrices:offsetVector:offset:")]
		[DesignatedInitializer]
		IntPtr Constructor (MPSMatrix[] sourceMatrices, MPSMatrix[] destinationMatrices, [NullAllowed] MPSVector offsets, nuint byteOffset);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSKernel))]
	interface MPSMatrixCopy
	{
		[Export ("initWithDevice:copyRows:copyColumns:sourcesAreTransposed:destinationsAreTransposed:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint copyRows, nuint copyColumns, bool sourcesAreTransposed, bool destinationsAreTransposed);

		[Export ("copyRows")]
		nuint CopyRows { get; }

		[Export ("copyColumns")]
		nuint CopyColumns { get; }

		[Export ("sourcesAreTransposed")]
		bool SourcesAreTransposed { get; }

		[Export ("destinationsAreTransposed")]
		bool DestinationsAreTransposed { get; }

		[Export ("encodeToCommandBuffer:copyDescriptor:")]
		void Encode (IMTLCommandBuffer cmdBuf, MPSMatrixCopyDescriptor copyDescriptor);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSKernel))]
	interface MPSImageCopyToMatrix
	{
		[Export ("destinationMatrixOrigin", ArgumentSemantic.Assign)]
		MTLOrigin DestinationMatrixOrigin { get; set; }

		[Export ("destinationMatrixBatchIndex")]
		nuint DestinationMatrixBatchIndex { get; set; }

		[Export ("dataLayout")]
		MPSDataLayout DataLayout { get; }

		[Export ("initWithDevice:dataLayout:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, MPSDataLayout dataLayout);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[Export ("encodeToCommandBuffer:sourceImage:destinationMatrix:")]
		void Encode (IMTLCommandBuffer commandBuffer, MPSImage sourceImage, MPSMatrix destinationMatrix);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSKernel))]
	interface MPSImageFindKeypoints
	{
		[Export ("keypointRangeInfo")]
		MPSImageKeypointRangeInfo KeypointRangeInfo { get; }

		[Export ("initWithDevice:info:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, MPSImageKeypointRangeInfo info);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[Export ("encodeToCommandBuffer:sourceTexture:regions:numberOfRegions:keypointCountBuffer:keypointCountBufferOffset:keypointDataBuffer:keypointDataBufferOffset:")]
		void Encode (IMTLCommandBuffer commandBuffer, IMTLTexture source, MTLRegion regions, nuint numberOfRegions, IMTLBuffer keypointCountBuffer, nuint keypointCountBufferOffset, IMTLBuffer keypointDataBuffer, nuint keypointDataBufferOffset);
	}
	
	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSBinaryImageKernel))]
	interface MPSImageArithmetic
	{
		[Export ("primaryScale")]
		float PrimaryScale { get; set; }

		[Export ("secondaryScale")]
		float SecondaryScale { get; set; }

		[Export ("bias")]
		float Bias { get; set; }

		[Export ("primaryStrideInPixels", ArgumentSemantic.Assign)]
		MTLSize PrimaryStrideInPixels { get; set; }

		[Export ("secondaryStrideInPixels", ArgumentSemantic.Assign)]
		MTLSize SecondaryStrideInPixels { get; set; }

		//inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSImageArithmetic))]
	interface MPSImageAdd
	{
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSImageArithmetic))]
	interface MPSImageSubtract
	{
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSImageArithmetic))]
	interface MPSImageMultiply
	{
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSImageArithmetic))]
	interface MPSImageDivide
	{
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSUnaryImageKernel))]
	interface MPSImageScale
	{
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[NullAllowed, Export ("scaleTransform", ArgumentSemantic.Assign)]
		MPSScaleTransform ScaleTransform { get; set; }

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSImageScale))]
	interface MPSImageBilinearScale
	{
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSUnaryImageKernel))]
	interface MPSImageStatisticsMinAndMax
	{
		[Export ("clipRectSource", ArgumentSemantic.Assign)]
		MTLRegion ClipRectSource { get; set; }

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSUnaryImageKernel))]
	interface MPSImageStatisticsMeanAndVariance
	{
		[Export ("clipRectSource", ArgumentSemantic.Assign)]
		MTLRegion ClipRectSource { get; set; }

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSUnaryImageKernel))]
	interface MPSImageStatisticsMean
	{
		[Export ("clipRectSource", ArgumentSemantic.Assign)]
		MTLRegion ClipRectSource { get; set; }

		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject), Name = "MPSNNDefaultPadding")]
	interface MPSNnDefaultPadding : MPSNnPadding
	{
		[Static]
		[Export ("paddingWithMethod:")]
		MPSNnDefaultPadding CreatePadding (MPSNnPaddingMethod method);

		[Static]
		[Export ("paddingForTensorflowAveragePooling")]
		MPSNnDefaultPadding CreatePaddingForTensorflowAveragePooling ();
	}
	
	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSKernel), Name = "MPSCNNBinaryKernel")]
	interface MPSCnnBinaryKernel
	{
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);

		[Export ("primaryOffset", ArgumentSemantic.Assign)]
		MPSOffset PrimaryOffset { get; set; }

		[Export ("secondaryOffset", ArgumentSemantic.Assign)]
		MPSOffset SecondaryOffset { get; set; }

		[Export ("clipRect", ArgumentSemantic.Assign)]
		MTLRegion ClipRect { get; set; }

		[Export ("destinationFeatureChannelOffset")]
		nuint DestinationFeatureChannelOffset { get; set; }

		[Export ("primaryEdgeMode", ArgumentSemantic.Assign)]
		MPSImageEdgeMode PrimaryEdgeMode { get; set; }

		[Export ("secondaryEdgeMode", ArgumentSemantic.Assign)]
		MPSImageEdgeMode SecondaryEdgeMode { get; set; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("primaryStrideInPixelsX")]
		nuint PrimaryStrideInPixelsX { get; }

		[Export ("primaryStrideInPixelsY")]
		nuint PrimaryStrideInPixelsY { get; }

		[Export ("secondaryStrideInPixelsX")]
		nuint SecondaryStrideInPixelsX { get; }

		[Export ("secondaryStrideInPixelsY")]
		nuint SecondaryStrideInPixelsY { get; }

		[Export ("isBackwards")]
		bool IsBackwards { get; }

		[Export ("padding", ArgumentSemantic.Retain)]
		IMPSNnPadding Padding { get; set; }

		[Export ("destinationImageAllocator", ArgumentSemantic.Retain)]
		IMPSImageAllocator DestinationImageAllocator { get; set; }

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[Export ("encodeToCommandBuffer:primaryImage:secondaryImage:destinationImage:")]
		void Encode (IMTLCommandBuffer commandBuffer, MPSImage primaryImage, MPSImage secondaryImage, MPSImage destinationImage);

		[Export ("encodeToCommandBuffer:primaryImage:secondaryImage:")]
		MPSImage EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, MPSImage primaryImage, MPSImage secondaryImage);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnNeuron), Name = "MPSCNNNeuronPReLU")]
	interface MPSCnnNeuronPReLU
	{
		[Export ("initWithDevice:a:count:")]
		[Internal]
		IntPtr InitWith (IMTLDevice device, IntPtr /* float* */ a, nuint count);

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnNeuron), Name = "MPSCNNNeuronHardSigmoid")]
	interface MPSCnnNeuronHardSigmoid
	{
		[Export ("a")]
		float A { get; }

		[Export ("b")]
		float B { get; }

		[Export ("initWithDevice:a:b:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, float a, float b);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnNeuron), Name = "MPSCNNNeuronSoftPlus")]
	interface MPSCnnNeuronSoftPlus
	{
		[Export ("a")]
		float A { get; }

		[Export ("b")]
		float B { get; }

		[Export ("initWithDevice:a:b:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, float a, float b);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnNeuron), Name = "MPSCNNNeuronSoftSign")]
	interface MPSCnnNeuronSoftSign
	{
		[Export ("initWithDevice:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnNeuron), Name = "MPSCNNNeuronELU")]
	interface MPSCnnNeuronELU
	{
		[Export ("a")]
		float A { get; }

		[Export ("initWithDevice:a:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, float a);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnNeuron), Name = "MPSCNNNeuronReLUN")]
	interface MPSCnnNeuronReLUN
	{
		[Export ("a")]
		float A { get; }

		[Export ("b")]
		float B { get; }

		[Export ("initWithDevice:a:b:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, float a, float b);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnConvolutionDescriptor), Name = "MPSCNNSubPixelConvolutionDescriptor")]
	interface MPSCnnSubPixelConvolutionDescriptor
	{
		[Export ("subPixelScaleFactor")]
		nuint SubPixelScaleFactor { get; set; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnConvolutionDescriptor), Name = "MPSCNNDepthWiseConvolutionDescriptor")]
	interface MPSCnnDepthWiseConvolutionDescriptor
	{
		[Export ("channelMultiplier")]
		nuint ChannelMultiplier { get; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSState), Name = "MPSCNNConvolutionState")]
	interface MPSCnnConvolutionState : MPSImageSizeEncodingState
	{
		[Export ("kernelWidth")]
		nuint KernelWidth { get; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; }

		[Export ("sourceOffset")]
		MPSOffset SourceOffset { get; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnKernel), Name = "MPSCNNConvolutionTranspose")]
	interface MPSCnnConvolutionTranspose
	{
		[Export ("inputFeatureChannels")]
		nuint InputFeatureChannels { get; }

		[Export ("outputFeatureChannels")]
		nuint OutputFeatureChannels { get; }

		[Export ("kernelOffsetX")]
		nint KernelOffsetX { get; set; }

		[Export ("kernelOffsetY")]
		nint KernelOffsetY { get; set; }

		[Export ("groups")]
		nuint Groups { get; }

		[Export ("initWithDevice:weights:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, IMPSCnnConvolutionDataSource weights);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[Export ("encodeToCommandBuffer:sourceImage:convolutionState:")]
		MPSImage Encode (IMTLCommandBuffer commandBuffer, MPSImage sourceImage, [NullAllowed]MPSCnnConvolutionState convolutionState);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnKernel), Name = "MPSCNNBinaryConvolution")]
	interface MPSCnnBinaryConvolution
	{
		[Export ("inputFeatureChannels")]
		nuint InputFeatureChannels { get; }

		[Export ("outputFeatureChannels")]
		nuint OutputFeatureChannels { get; }

		[Export ("initWithDevice:convolutionData:scaleValue:type:flags:")]
		IntPtr Constructor (IMTLDevice device, IMPSCnnConvolutionDataSource convolutionData, float scaleValue, MPSCnnBinaryConvolutionType type, MPSCnnBinaryConvolutionFlags flags);

		[Internal, Export ("initWithDevice:convolutionData:outputBiasTerms:outputScaleTerms:inputBiasTerms:inputScaleTerms:type:flags:")]
		IntPtr InitWith (IMTLDevice device, IMPSCnnConvolutionDataSource convolutionData, [NullAllowed] IntPtr /* float* */ outputBiasTerms, [NullAllowed] IntPtr /* float* */ outputScaleTerms, [NullAllowed] IntPtr /* float* */ inputBiasTerms, [NullAllowed] IntPtr /* float* */ inputScaleTerms,MPSCnnBinaryConvolutionType type, MPSCnnBinaryConvolutionFlags flags);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnBinaryConvolution), Name = "MPSCNNBinaryFullyConnected")]
	interface MPSCnnBinaryFullyConnected
	{
		[Export ("initWithDevice:convolutionData:scaleValue:type:flags:")]
		IntPtr Constructor (IMTLDevice device, IMPSCnnConvolutionDataSource convolutionData, float scaleValue, MPSCnnBinaryConvolutionType type, MPSCnnBinaryConvolutionFlags flags);

		[Internal, Export ("initWithDevice:convolutionData:outputBiasTerms:outputScaleTerms:inputBiasTerms:inputScaleTerms:type:flags:")]
		IntPtr InitWith (IMTLDevice device, IMPSCnnConvolutionDataSource convolutionData, [NullAllowed] IntPtr /* float* */ outputBiasTerms, [NullAllowed] IntPtr /* float* */ outputScaleTerms, [NullAllowed] IntPtr /* float* */ inputBiasTerms, [NullAllowed] IntPtr /* float* */ inputScaleTerms, MPSCnnBinaryConvolutionType type, MPSCnnBinaryConvolutionFlags flags);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnPooling), Name = "MPSCNNPoolingL2Norm")]
	interface MPSCnnPoolingL2Norm
	{
		[Export ("initWithDevice:kernelWidth:kernelHeight:strideInPixelsX:strideInPixelsY:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, nuint strideInPixelsX, nuint strideInPixelsY);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnPooling), Name = "MPSCNNDilatedPoolingMax")]
	interface MPSCnnDilatedPoolingMax
	{
		[Export ("dilationRateX")]
		nuint DilationRateX { get; }

		[Export ("dilationRateY")]
		nuint DilationRateY { get; }

		[Export ("initWithDevice:kernelWidth:kernelHeight:dilationRateX:dilationRateY:strideInPixelsX:strideInPixelsY:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint kernelWidth, nuint kernelHeight, nuint dilationRateX, nuint dilationRateY, nuint strideInPixelsX, nuint strideInPixelsY);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnKernel), Name = "MPSCNNUpsampling")]
	interface MPSCnnUpsampling
	{
		[Export ("scaleFactorX")]
		double ScaleFactorX { get; }

		[Export ("scaleFactorY")]
		double ScaleFactorY { get; }

		// inlining ctor from base class
		[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnUpsampling), Name = "MPSCNNUpsamplingNearest")]
	interface MPSCnnUpsamplingNearest
	{
		[Export ("initWithDevice:integerScaleFactorX:integerScaleFactorY:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint integerScaleFactorX, nuint integerScaleFactorY);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnUpsampling), Name = "MPSCNNUpsamplingBilinear")]
	interface MPSCnnUpsamplingBilinear
	{
		[Export ("initWithDevice:integerScaleFactorX:integerScaleFactorY:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, nuint integerScaleFactorX, nuint integerScaleFactorY);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject), Name = "MPSRNNDescriptor")]
	interface MPSRnnDescriptor
	{
		[Export ("inputFeatureChannels")]
		nuint InputFeatureChannels { get; set; }

		[Export ("outputFeatureChannels")]
		nuint OutputFeatureChannels { get; set; }

		[Export ("useLayerInputUnitTransformMode")]
		bool UseLayerInputUnitTransformMode { get; set; }

		[Export ("useFloat32Weights")]
		bool UseFloat32Weights { get; set; }

		[Export ("layerSequenceDirection", ArgumentSemantic.Assign)]
		MPSRnnSequenceDirection LayerSequenceDirection { get; set; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSRnnDescriptor), Name = "MPSRNNSingleGateDescriptor")]
	interface MPSRnnSingleGateDescriptor
	{
		[NullAllowed, Export ("inputWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource InputWeights { get; set; }

		[NullAllowed, Export ("recurrentWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource RecurrentWeights { get; set; }

		[Static]
		[Export ("createRNNSingleGateDescriptorWithInputFeatureChannels:outputFeatureChannels:")]
		MPSRnnSingleGateDescriptor CreateRNNSingleGateDescriptor (nuint inputFeatureChannels, nuint outputFeatureChannels);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSRnnDescriptor))]
	interface MPSGRUDescriptor
	{
		[NullAllowed, Export ("inputGateInputWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource InputGateInputWeights { get; set; }

		[NullAllowed, Export ("inputGateRecurrentWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource InputGateRecurrentWeights { get; set; }

		[NullAllowed, Export ("recurrentGateInputWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource RecurrentGateInputWeights { get; set; }

		[NullAllowed, Export ("recurrentGateRecurrentWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource RecurrentGateRecurrentWeights { get; set; }

		[NullAllowed, Export ("outputGateInputWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource OutputGateInputWeights { get; set; }

		[NullAllowed, Export ("outputGateRecurrentWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource OutputGateRecurrentWeights { get; set; }

		[NullAllowed, Export ("outputGateInputGateWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource OutputGateInputGateWeights { get; set; }

		[Export ("gatePnormValue")]
		float GatePnormValue { get; set; }

		[Export ("flipOutputGates")]
		bool FlipOutputGates { get; set; }

		[Static]
		[Export ("createGRUDescriptorWithInputFeatureChannels:outputFeatureChannels:")]
		MPSGRUDescriptor CreateGRUDescriptor (nuint inputFeatureChannels, nuint outputFeatureChannels);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSRnnDescriptor))]
	interface MPSLSTMDescriptor
	{
		[Export ("memoryWeightsAreDiagonal")]
		bool MemoryWeightsAreDiagonal { get; set; }

		[NullAllowed, Export ("inputGateInputWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource InputGateInputWeights { get; set; }

		[NullAllowed, Export ("inputGateRecurrentWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource InputGateRecurrentWeights { get; set; }

		[NullAllowed, Export ("inputGateMemoryWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource InputGateMemoryWeights { get; set; }

		[NullAllowed, Export ("forgetGateInputWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource ForgetGateInputWeights { get; set; }

		[NullAllowed, Export ("forgetGateRecurrentWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource ForgetGateRecurrentWeights { get; set; }

		[NullAllowed, Export ("forgetGateMemoryWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource ForgetGateMemoryWeights { get; set; }

		[NullAllowed, Export ("outputGateInputWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource OutputGateInputWeights { get; set; }

		[NullAllowed, Export ("outputGateRecurrentWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource OutputGateRecurrentWeights { get; set; }

		[NullAllowed, Export ("outputGateMemoryWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource OutputGateMemoryWeights { get; set; }

		[NullAllowed, Export ("cellGateInputWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource CellGateInputWeights { get; set; }

		[NullAllowed, Export ("cellGateRecurrentWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource CellGateRecurrentWeights { get; set; }

		[NullAllowed, Export ("cellGateMemoryWeights", ArgumentSemantic.Retain)]
		IMPSCnnConvolutionDataSource CellGateMemoryWeights { get; set; }

		[Export ("cellToOutputNeuronType", ArgumentSemantic.Assign)]
		MPSCnnNeuronType CellToOutputNeuronType { get; set; }

		[Export ("cellToOutputNeuronParamA")]
		float CellToOutputNeuronParamA { get; set; }

		[Export ("cellToOutputNeuronParamB")]
		float CellToOutputNeuronParamB { get; set; }

		[Static]
		[Export ("createLSTMDescriptorWithInputFeatureChannels:outputFeatureChannels:")]
		MPSLSTMDescriptor CreateLSTMDescriptor (nuint inputFeatureChannels, nuint outputFeatureChannels);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSState), Name = "MPSRNNRecurrentImageState")]
	interface MPSRnnRecurrentImageState
	{
		[Export ("getRecurrentOutputImageForLayerIndex:")]
		[return: NullAllowed]
		MPSImage GetRecurrentOutputImage (nuint layerIndex);

		[Export ("getMemoryCellImageForLayerIndex:")]
		[return: NullAllowed]
		MPSImage GetMemoryCellImage (nuint layerIndex);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnKernel), Name = "MPSRNNImageInferenceLayer")]
	interface MPSRnnImageInferenceLayer
	{
		[Export ("inputFeatureChannels")]
		nuint InputFeatureChannels { get; }

		[Export ("outputFeatureChannels")]
		nuint OutputFeatureChannels { get; }

		[Export ("numberOfLayers")]
		nuint NumberOfLayers { get; }

		[Export ("recurrentOutputIsTemporary")]
		bool RecurrentOutputIsTemporary { get; set; }

		[Export ("storeAllIntermediateStates")]
		bool StoreAllIntermediateStates { get; set; }

		[Export ("bidirectionalCombineMode", ArgumentSemantic.Assign)]
		MPSRnnBidirectionalCombineMode BidirectionalCombineMode { get; set; }

		[Export ("initWithDevice:rnnDescriptor:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, MPSRnnDescriptor rnnDescriptor);

		[Export ("initWithDevice:rnnDescriptors:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, MPSRnnDescriptor[] rnnDescriptors);

		[Export ("encodeSequenceToCommandBuffer:sourceImages:destinationImages:recurrentInputState:recurrentOutputStates:")]
		void EncodeSequence (IMTLCommandBuffer commandBuffer, MPSImage[] sourceImages, MPSImage[] destinationImages, [NullAllowed] MPSRnnRecurrentImageState recurrentInputState, [NullAllowed] NSMutableArray<MPSRnnRecurrentImageState> recurrentOutputStates);

		[Export ("encodeBidirectionalSequenceToCommandBuffer:sourceSequence:destinationForwardImages:destinationBackwardImages:")]
		void EncodeBidirectionalSequence (IMTLCommandBuffer commandBuffer, MPSImage[] sourceSequence, MPSImage[] destinationForwardImages, [NullAllowed] MPSImage[] destinationBackwardImages);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[Export ("copyWithZone:device:")]
		MPSRnnImageInferenceLayer Copy ([NullAllowed] NSZone zone, [NullAllowed] IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSState), Name = "MPSRNNRecurrentMatrixState")]
	interface MPSRnnRecurrentMatrixState
	{
		[Export ("getRecurrentOutputMatrixForLayerIndex:")]
		[return: NullAllowed]
		MPSMatrix GetRecurrentOutputMatrix (nuint layerIndex);

		[Export ("getMemoryCellMatrixForLayerIndex:")]
		[return: NullAllowed]
		MPSMatrix GetMemoryCellMatrix (nuint layerIndex);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSKernel), Name = "MPSRNNMatrixInferenceLayer")]
	interface MPSRnnMatrixInferenceLayer
	{
		[Export ("inputFeatureChannels")]
		nuint InputFeatureChannels { get; }

		[Export ("outputFeatureChannels")]
		nuint OutputFeatureChannels { get; }

		[Export ("numberOfLayers")]
		nuint NumberOfLayers { get; }

		[Export ("recurrentOutputIsTemporary")]
		bool RecurrentOutputIsTemporary { get; set; }

		[Export ("storeAllIntermediateStates")]
		bool StoreAllIntermediateStates { get; set; }

		[Export ("bidirectionalCombineMode", ArgumentSemantic.Assign)]
		MPSRnnBidirectionalCombineMode BidirectionalCombineMode { get; set; }

		[Export ("initWithDevice:rnnDescriptor:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, MPSRnnDescriptor rnnDescriptor);

		[Export ("initWithDevice:rnnDescriptors:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, MPSRnnDescriptor[] rnnDescriptors);

		[Export ("encodeSequenceToCommandBuffer:sourceMatrices:destinationMatrices:recurrentInputState:recurrentOutputStates:")]
		void EncodeSequence (IMTLCommandBuffer commandBuffer, MPSMatrix[] sourceMatrices, MPSMatrix[] destinationMatrices, [NullAllowed] MPSRnnRecurrentMatrixState recurrentInputState, [NullAllowed] NSMutableArray<MPSRnnRecurrentMatrixState> recurrentOutputStates);

		[Export ("encodeBidirectionalSequenceToCommandBuffer:sourceSequence:destinationForwardMatrices:destinationBackwardMatrices:")]
		void EncodeBidirectionalSequence (IMTLCommandBuffer commandBuffer, MPSMatrix[] sourceSequence, MPSMatrix[] destinationForwardMatrices, [NullAllowed] MPSMatrix[] destinationBackwardMatrices);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[Export ("copyWithZone:device:")]
		MPSRnnMatrixInferenceLayer Copy ([NullAllowed] NSZone zone, [NullAllowed] IMTLDevice device);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject), Name = "MPSNNImageNode")]
	[DisableDefaultCtor]
	interface MPSNnImageNode
	{
		[Export ("initWithHandle:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] IMPSHandle handle);

		[Static]
		[Export ("nodeWithHandle:")]
		MPSNnImageNode FromHandle ([NullAllowed] IMPSHandle handle);

		[Static]
		[Export ("exportedNodeWithHandle:")]
		MPSNnImageNode ExportedNodeWithHandle ([NullAllowed] IMPSHandle handle);

		[NullAllowed, Export ("handle", ArgumentSemantic.Retain)]
		IMPSHandle MPSHandle { get; set; }

		[Export ("format", ArgumentSemantic.Assign)]
		MPSImageFeatureChannelFormat Format { get; set; }

		[Export ("imageAllocator", ArgumentSemantic.Retain)]
		IMPSImageAllocator ImageAllocator { get; set; }

		[Export ("exportFromGraph")]
		bool ExportFromGraph { get; set; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject), Name = "MPSNNStateNode")]
	[DisableDefaultCtor]
	interface MPSNnStateNode
	{
		[NullAllowed, Export ("handle", ArgumentSemantic.Retain)]
		IMPSHandle MPSHandle { get; set; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(NSObject), Name = "MPSNNFilterNode")]
	[DisableDefaultCtor]
	interface MPSNnFilterNode
	{
		[Export ("resultImage")]
		MPSNnImageNode ResultImage { get; }

		[NullAllowed, Export ("resultState")]
		MPSNnStateNode ResultState { get; }

		[NullAllowed, Export ("resultStates")]
		MPSNnStateNode[] ResultStates { get; }

		[Export ("paddingPolicy", ArgumentSemantic.Retain)]
		IMPSNnPadding PaddingPolicy { get; set; }

		[NullAllowed, Export ("label")]
		string Label { get; set; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSCNNConvolutionNode")]
	interface MPSCnnConvolutionNode
	{
		[Static]
		[Export ("nodeWithSource:weights:")]
		MPSCnnConvolutionNode FromSource (MPSNnImageNode sourceNode, IMPSCnnConvolutionDataSource weights);

		[Export ("initWithSource:weights:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, IMPSCnnConvolutionDataSource weights);

		[NullAllowed, Export ("convolutionState")]
		MPSCnnConvolutionStateNode ConvolutionState { get; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnConvolutionNode), Name = "MPSCNNFullyConnectedNode")]
	interface MPSCnnFullyConnectedNode
	{
		[Static]
		[Export ("nodeWithSource:weights:")]
		MPSCnnFullyConnectedNode FromSource (MPSNnImageNode sourceNode, IMPSCnnConvolutionDataSource weights);

		[Export ("initWithSource:weights:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, IMPSCnnConvolutionDataSource weights);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnConvolutionNode), Name = "MPSCNNBinaryConvolutionNode")]
	interface MPSCnnBinaryConvolutionNode
	{
		[Static]
		[Export ("nodeWithSource:weights:scaleValue:type:flags:")]
		MPSCnnBinaryConvolutionNode FromSource (MPSNnImageNode sourceNode, IMPSCnnConvolutionDataSource weights, float scaleValue,MPSCnnBinaryConvolutionType type,MPSCnnBinaryConvolutionFlags flags);

		[Export ("initWithSource:weights:scaleValue:type:flags:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, IMPSCnnConvolutionDataSource weights, float scaleValue,MPSCnnBinaryConvolutionType type,MPSCnnBinaryConvolutionFlags flags);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnBinaryConvolutionNode), Name = "MPSCNNBinaryFullyConnectedNode")]
	interface MPSCnnBinaryFullyConnectedNode
	{
		[Static]
		[Export ("nodeWithSource:weights:scaleValue:type:flags:")]
		MPSCnnBinaryFullyConnectedNode FromSource (MPSNnImageNode sourceNode, IMPSCnnConvolutionDataSource weights, float scaleValue,MPSCnnBinaryConvolutionType type,MPSCnnBinaryConvolutionFlags flags);

		[Export ("initWithSource:weights:scaleValue:type:flags:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, IMPSCnnConvolutionDataSource weights, float scaleValue,MPSCnnBinaryConvolutionType type,MPSCnnBinaryConvolutionFlags flags);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnConvolutionNode), Name = "MPSCNNConvolutionTransposeNode")]
	interface MPSCnnConvolutionTransposeNode
	{
		[Static]
		[Export ("nodeWithSource:convolutionState:weights:")]
		MPSCnnConvolutionTransposeNode FromSource (MPSNnImageNode sourceNode, [NullAllowed]MPSCnnConvolutionStateNode convolutionState, IMPSCnnConvolutionDataSource weights);

		[Export ("initWithSource:convolutionState:weights:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, [NullAllowed]MPSCnnConvolutionStateNode convolutionState, IMPSCnnConvolutionDataSource weights);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSCNNNeuronNode")]
	[DisableDefaultCtor]
	interface MPSCnnNeuronNode
	{
		[Export ("a")]
		float A { get; }

		[Export ("b")]
		float B { get; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronAbsoluteNode")]
	interface MPSCnnNeuronAbsoluteNode
	{
		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNeuronAbsoluteNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronELUNode")]
	interface MPSCnnNeuronELUNode
	{
		[Static]
		[Export ("nodeWithSource:a:")]
		MPSCnnNeuronELUNode FromSource (MPSNnImageNode sourceNode, float a);

		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNeuronELUNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);

		[Export ("initWithSource:a:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, float a);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronReLUNNode")]
	interface MPSCnnNeuronReLUNNode
	{
		[Static]
		[Export ("nodeWithSource:a:b:")]
		MPSCnnNeuronReLUNNode FromSource (MPSNnImageNode sourceNode, float a, float b);

		[Export ("initWithSource:a:b:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, float a, float b);

		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNeuronReLUNNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronLinearNode")]
	interface MPSCnnNeuronLinearNode
	{
		[Static]
		[Export ("nodeWithSource:a:b:")]
		MPSCnnNeuronLinearNode FromSource (MPSNnImageNode sourceNode, float a, float b);

		[Export ("initWithSource:a:b:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, float a, float b);

		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNeuronLinearNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronReLUNode")]
	interface MPSCnnNeuronReLUNode
	{
		[Static]
		[Export ("nodeWithSource:a:")]
		MPSCnnNeuronReLUNode FromSource (MPSNnImageNode sourceNode, float a);

		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNeuronReLUNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);

		[Export ("initWithSource:a:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, float a);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronSigmoidNode")]
	interface MPSCnnNeuronSigmoidNode
	{
		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNeuronSigmoidNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronHardSigmoidNode")]
	interface MPSCnnNeuronHardSigmoidNode
	{
		[Static]
		[Export ("nodeWithSource:a:b:")]
		MPSCnnNeuronHardSigmoidNode FromSource (MPSNnImageNode sourceNode, float a, float b);

		[Export ("initWithSource:a:b:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, float a, float b);

		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNeuronHardSigmoidNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronSoftPlusNode")]
	interface MPSCnnNeuronSoftPlusNode
	{
		[Static]
		[Export ("nodeWithSource:a:b:")]
		MPSCnnNeuronSoftPlusNode FromSource (MPSNnImageNode sourceNode, float a, float b);

		[Export ("initWithSource:a:b:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, float a, float b);

		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNeuronSoftPlusNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronSoftSignNode")]
	interface MPSCnnNeuronSoftSignNode
	{
		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNeuronSoftSignNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronTanHNode")]
	interface MPSCnnNeuronTanHNode
	{
		[Static]
		[Export ("nodeWithSource:a:b:")]
		MPSCnnNeuronTanHNode FromSource (MPSNnImageNode sourceNode, float a, float b);

		[Export ("initWithSource:a:b:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, float a, float b);

		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNeuronTanHNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNeuronNode), Name = "MPSCNNNeuronPReLUNode")]
	interface MPSCnnNeuronPReLUNode
	{
		[Static]
		[Export ("nodeWithSource:aData:")]
		MPSCnnNeuronPReLUNode FromSource (MPSNnImageNode sourceNode, NSData aData);

		[Export ("initWithSource:aData:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, NSData aData);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSCNNPoolingNode")]
	interface MPSCnnPoolingNode
	{
		[Static]
		[Export ("nodeWithSource:filterSize:")]
		MPSCnnPoolingNode FromSource (MPSNnImageNode sourceNode, nuint size);

		[Static]
		[Export ("nodeWithSource:filterSize:stride:")]
		MPSCnnPoolingNode FromSource (MPSNnImageNode sourceNode, nuint size, nuint stride);

		[Export ("initWithSource:kernelWidth:kernelHeight:strideInPixelsX:strideInPixelsY:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint kernelWidth, nuint kernelHeight, nuint strideInPixelsX, nuint strideInPixelsY);

		[Export ("initWithSource:filterSize:stride:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint size, nuint stride);

		[Export ("initWithSource:filterSize:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint size);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSCNNDilatedPoolingMaxNode")]
	interface MPSCnnDilatedPoolingMaxNode
	{
		[Export ("dilationRateX")]
		nuint DilationRateX { get; }

		[Export ("dilationRateY")]
		nuint DilationRateY { get; }

		[Static]
		[Export ("nodeWithSource:filterSize:")]
		MPSCnnDilatedPoolingMaxNode FromSource (MPSNnImageNode sourceNode, nuint size);

		[Static]
		[Export ("nodeWithSource:filterSize:stride:dilationRate:")]
		MPSCnnDilatedPoolingMaxNode FromSource (MPSNnImageNode sourceNode, nuint size, nuint stride, nuint dilationRate);

		[Export ("initWithSource:kernelWidth:kernelHeight:strideInPixelsX:strideInPixelsY:dilationRateX:dilationRateY:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint kernelWidth, nuint kernelHeight, nuint strideInPixelsX, nuint strideInPixelsY, nuint dilationRateX, nuint dilationRateY);

		[Export ("initWithSource:filterSize:stride:dilationRate:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint size, nuint stride, nuint dilationRate);

		[Export ("initWithSource:filterSize:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint size);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSCNNNormalizationNode")]
	interface MPSCnnNormalizationNode
	{
		[Export ("alpha")]
		float Alpha { get; set; }

		[Export ("beta")]
		float Beta { get; set; }

		[Export ("delta")]
		float Delta { get; set; }

		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnNormalizationNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		[DesignatedInitializer]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNormalizationNode), Name = "MPSCNNSpatialNormalizationNode")]
	interface MPSCnnSpatialNormalizationNode
	{
		[Export ("kernelWidth")]
		nuint KernelWidth { get; set; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; set; }

		[Static]
		[Export ("nodeWithSource:kernelSize:")]
		MPSCnnSpatialNormalizationNode FromSource (MPSNnImageNode sourceNode, nuint kernelSize);

		[Export ("initWithSource:kernelSize:")]
		[DesignatedInitializer]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint kernelSize);

		[Export ("initWithSource:")]
		[DesignatedInitializer]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNormalizationNode), Name = "MPSCNNLocalContrastNormalizationNode")]
	interface MPSCnnLocalContrastNormalizationNode
	{
		[Export ("pm")]
		float Pm { get; set; }

		[Export ("ps")]
		float Ps { get; set; }

		[Export ("p0")]
		float P0 { get; set; }

		[Export ("kernelWidth")]
		nuint KernelWidth { get; set; }

		[Export ("kernelHeight")]
		nuint KernelHeight { get; set; }

		[Static]
		[Export ("nodeWithSource:kernelSize:")]
		MPSCnnLocalContrastNormalizationNode FromSource (MPSNnImageNode sourceNode, nuint kernelSize);

		[Export ("initWithSource:kernelSize:")]
		[DesignatedInitializer]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint kernelSize);

		[Export ("initWithSource:")]
		[DesignatedInitializer]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (MPSCnnNormalizationNode), Name = "MPSCNNCrossChannelNormalizationNode")]
	interface MPSCnnCrossChannelNormalizationNode
	{
		[Export ("kernelSizeInFeatureChannels")]
		nuint KernelSizeInFeatureChannels { get; set; }

		[Static]
		[Export ("nodeWithSource:kernelSize:")]
		MPSCnnCrossChannelNormalizationNode FromSource (MPSNnImageNode sourceNode, nuint kernelSize);

		[Export ("initWithSource:kernelSize:")]
		[DesignatedInitializer]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint kernelSize);

		[Export ("initWithSource:")]
		[DesignatedInitializer]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSNNScaleNode")]
	interface MPSNnScaleNode
	{
		[Static]
		[Export ("nodeWithSource:outputSize:")]
		MPSNnScaleNode FromSource (MPSNnImageNode sourceNode, MTLSize size);

		[Static]
		[Export ("nodeWithSource:transformProvider:outputSize:")]
		MPSNnScaleNode FromSource (MPSNnImageNode sourceNode, [NullAllowed] IMPSImageTransformProvider transformProvider, MTLSize size);

		[Export ("initWithSource:outputSize:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, MTLSize size);

		[Export ("initWithSource:transformProvider:outputSize:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, [NullAllowed] IMPSImageTransformProvider transformProvider, MTLSize size);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSNNBinaryArithmeticNode")]
	interface MPSNnBinaryArithmeticNode
	{
		[Static]
		[Export ("nodeWithSources:")]
		MPSNnBinaryArithmeticNode FromSources (MPSNnImageNode[] sourceNodes);

		[Static]
		[Export ("nodeWithLeftSource:rightSource:")]
		MPSNnBinaryArithmeticNode FromSources (MPSNnImageNode left, MPSNnImageNode right);

		[Export ("initWithSources:")]
		IntPtr Constructor (MPSNnImageNode[] sourceNodes);

		[Export ("initWithLeftSource:rightSource:")]
		IntPtr Constructor (MPSNnImageNode left, MPSNnImageNode right);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSNNConcatenationNode")]
	interface MPSNnConcatenationNode
	{
		[Static]
		[Export ("nodeWithSources:")]
		MPSNnConcatenationNode FromSources (MPSNnImageNode[] sourceNodes);

		[Export ("initWithSources:")]
		IntPtr Constructor (MPSNnImageNode[] sourceNodes);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSCNNSoftMaxNode")]
	interface MPSCnnSoftMaxNode
	{
		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnSoftMaxNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSCNNLogSoftMaxNode")]
	interface MPSCnnLogSoftMaxNode
	{
		[Static]
		[Export ("nodeWithSource:")]
		MPSCnnLogSoftMaxNode FromSource (MPSNnImageNode sourceNode);

		[Export ("initWithSource:")]
		IntPtr Constructor (MPSNnImageNode sourceNode);
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSCNNUpsamplingNearestNode")]
	interface MPSCnnUpsamplingNearestNode
	{
		[Static]
		[Export ("nodeWithSource:integerScaleFactorX:integerScaleFactorY:")]
		MPSCnnUpsamplingNearestNode FromSource (MPSNnImageNode sourceNode, nuint integerScaleFactorX, nuint integerScaleFactorY);

		[Export ("initWithSource:integerScaleFactorX:integerScaleFactorY:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint integerScaleFactorX, nuint integerScaleFactorY);

		[Export ("scaleFactorX")]
		double ScaleFactorX { get; }

		[Export ("scaleFactorY")]
		double ScaleFactorY { get; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnFilterNode), Name = "MPSCNNUpsamplingBilinearNode")]
	interface MPSCnnUpsamplingBilinearNode
	{
		[Static]
		[Export ("nodeWithSource:integerScaleFactorX:integerScaleFactorY:")]
		MPSCnnUpsamplingBilinearNode FromSource (MPSNnImageNode sourceNode, nuint integerScaleFactorX, nuint integerScaleFactorY);

		[Export ("initWithSource:integerScaleFactorX:integerScaleFactorY:")]
		IntPtr Constructor (MPSNnImageNode sourceNode, nuint integerScaleFactorX, nuint integerScaleFactorY);

		[Export ("scaleFactorX")]
		double ScaleFactorX { get; }

		[Export ("scaleFactorY")]
		double ScaleFactorY { get; }
	}

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSKernel), Name = "MPSNNGraph")]
	interface MPSNnGraph : NSCopying, NSSecureCoding
	{
		[Export ("initWithDevice:resultImage:")]
		[DesignatedInitializer]
		IntPtr Constructor (IMTLDevice device, MPSNnImageNode resultImage);

		[Export ("initWithCoder:device:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSCoder aDecoder, IMTLDevice device);

		[Export ("sourceImageHandles", ArgumentSemantic.Copy)]
		IMPSHandle[] SourceImageHandles { get; }

		[NullAllowed, Export ("sourceStateHandles", ArgumentSemantic.Copy)]
		IMPSHandle[] SourceStateHandles { get; }

		[NullAllowed, Export ("intermediateImageHandles", ArgumentSemantic.Copy)]
		IMPSHandle[] IntermediateImageHandles { get; }

		[NullAllowed, Export ("resultStateHandles", ArgumentSemantic.Copy)]
		IMPSHandle[] ResultStateHandles { get; }

		[NullAllowed, Export ("resultHandle")]
		IMPSHandle ResultHandle { get; }

		[Export ("outputStateIsTemporary")]
		bool OutputStateIsTemporary { get; set; }

		[Export ("destinationImageAllocator", ArgumentSemantic.Retain)]
		IMPSImageAllocator DestinationImageAllocator { get; set; }

		[Export ("encodeToCommandBuffer:sourceImages:sourceStates:intermediateImages:destinationStates:")]
		MPSImage Encode (IMTLCommandBuffer commandBuffer, MPSImage[] sourceImages, [NullAllowed] MPSState[] sourceStates, [NullAllowed] NSMutableArray<MPSImage> intermediateImages, [NullAllowed] NSMutableArray<MPSState> destinationStates);

		[Export ("encodeToCommandBuffer:sourceImages:")]
		MPSImage Encode (IMTLCommandBuffer commandBuffer, MPSImage[] sourceImages);

		[Async, Export ("executeAsyncWithSourceImages:completionHandler:")]
		MPSImage Execute (MPSImage[] sourceImages, Action<MPSImage, NSError> handler);
	}

	interface IMPSHandle { }

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface MPSHandle : NSSecureCoding
	{
		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; }
	}

	interface IMPSCnnConvolutionDataSource { }

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject), Name = "MPSCNNConvolutionDataSource")]
	interface MPSCnnConvolutionDataSource
	{
		[Abstract]
		[Export ("dataType")]
		MPSDataType DataType { get; }

		[Abstract]
		[Export ("descriptor")]
		MPSCnnConvolutionDescriptor Descriptor { get; }

		[Abstract]
		[Export ("weights")]
		IntPtr Weights { get; }

		[Abstract]
		[NullAllowed, Export ("biasTerms")]
		IntPtr /* float* */ BiasTerms { get; }

		[Abstract]
		[Export ("load")]
		bool Load { get; }

		[Abstract]
		[Export ("purge")]
		void Purge ();

		[Abstract]
		[NullAllowed, Export ("label")]
		string Label { get; }

		[Export ("rangesForUInt8Kernel")]
		IntPtr RangesForUInt8Kernel { get; }

		[Export ("lookupTableForUInt8Kernel")]
		IntPtr /* float* */ LookupTableForUInt8Kernel { get; }
	}

	interface IMPSNnPadding { }

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject), Name = "MPSNNPadding")]
	interface MPSNnPadding : NSSecureCoding
	{
		[Abstract]
		[Export ("paddingMethod")]
		MPSNnPaddingMethod PaddingMethod { get; }

		[Export ("label")]
		string Label { get; }

		[Export ("destinationImageDescriptorForSourceImages:sourceStates:forKernel:suggestedDescriptor:")]
		MPSImageDescriptor SourceStates (MPSImage[] sourceImages, [NullAllowed] MPSState[] sourceStates, MPSKernel kernel, MPSImageDescriptor inDescriptor);
	}

	interface IMPSImageSizeEncodingState { }

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface MPSImageSizeEncodingState
	{
		[Abstract]
		[Export ("sourceWidth")]
		nuint SourceWidth { get; }

		[Abstract]
		[Export ("sourceHeight")]
		nuint SourceHeight { get; }
	}

	interface IMPSImageAllocator { }

	[TV (11,0), Mac (10, 13, onlyOn64: true), iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface MPSImageAllocator : NSSecureCoding
	{
	    [Abstract]
	    [Export ("imageForCommandBuffer:imageDescriptor:kernel:")]
	    MPSImage GetImage (IMTLCommandBuffer cmdBuf, MPSImageDescriptor descriptor, MPSKernel kernel);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnStateNode), Name = "MPSCNNConvolutionStateNode")]
	interface MPSCnnConvolutionStateNode
	{
	}

	interface IMPSImageTransformProvider { }

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface MPSImageTransformProvider : NSSecureCoding
	{
	    [Abstract]
	    [Export ("transformForSourceImage:handle:")]
	    MPSScaleTransform GetTransform (MPSImage image, [NullAllowed] IMPSHandle handle);
	}

	[TV (11, 0), Mac (10,13, onlyOn64: true), iOS (11, 0)]
	[Protocol]
	interface MPSDeviceProvider
	{
	    [Abstract]
	    [Export ("mpsMTLDevice")]
	    IMTLDevice MpsMTLDevice { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnPoolingNode), Name="MPSCNNPoolingAverageNode")]
	interface MPSCnnPoolingAverageNode
	{
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnPoolingNode), Name="MPSCNNPoolingL2NormNode")]
	interface MPSCnnPoolingL2NormNode
	{
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSCnnPoolingNode), Name="MPSCNNPoolingMaxNode")]
	interface MPSCnnPoolingMaxNode
	{
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnBinaryArithmeticNode), Name="MPSNNAdditionNode")]
	interface MPSNnAdditionNode
	{
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnScaleNode), Name="MPSNNBilinearScaleNode")]
	interface MPSNnBilinearScaleNode
	{
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnBinaryArithmeticNode), Name="MPSNNDivisionNode")]
	interface MPSNnDivisionNode
	{
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnScaleNode), Name="MPSNNLanczosScaleNode")]
	interface MPSNnLanczosScaleNode
	{
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnBinaryArithmeticNode), Name="MPSNNMultiplicationNode")]
	interface MPSNnMultiplicationNode
	{
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof(MPSNnBinaryArithmeticNode), Name="MPSNNSubtractionNode")]
	interface MPSNnSubtractionNode
	{
	}
}
#endif