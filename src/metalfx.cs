using System;
using System.ComponentModel;

using CoreFoundation;
using Foundation;
using Metal;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif


namespace MetalFX {

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0)]
	[Native]
	public enum MTLFXSpatialScalerColorProcessingMode : long
	{
		Perceptual = 0,
		Linear = 1,
		Hdr = 2,
	}

	interface IMTLFXSpatialScaler {}

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0)]
	[Protocol]
	interface MTLFXSpatialScaler
	{
		[Abstract]
		[Export ("colorTextureUsage")]
		MTLTextureUsage ColorTextureUsage { get; }

		[Abstract]
		[Export ("outputTextureUsage")]
		MTLTextureUsage OutputTextureUsage { get; }

		[Abstract]
		[Export ("inputContentWidth")]
		nuint InputContentWidth { get; set; }

		[Abstract]
		[Export ("inputContentHeight")]
		nuint InputContentHeight { get; set; }

		[Abstract]
		[NullAllowed, Export ("colorTexture", ArgumentSemantic.Retain)]
		IMTLTexture ColorTexture { get; set; }

		[Abstract]
		[NullAllowed, Export ("outputTexture", ArgumentSemantic.Retain)]
		IMTLTexture OutputTexture { get; set; }

		[Abstract]
		[Export ("colorTextureFormat")]
		MTLPixelFormat ColorTextureFormat { get; }

		[Abstract]
		[Export ("outputTextureFormat")]
		MTLPixelFormat OutputTextureFormat { get; }

		[Abstract]
		[Export ("inputWidth")]
		nuint InputWidth { get; }

		[Abstract]
		[Export ("inputHeight")]
		nuint InputHeight { get; }

		[Abstract]
		[Export ("outputWidth")]
		nuint OutputWidth { get; }

		[Abstract]
		[Export ("outputHeight")]
		nuint OutputHeight { get; }

		[Abstract]
		[Export ("colorProcessingMode")]
		MTLFXSpatialScalerColorProcessingMode ColorProcessingMode { get; }

		[Abstract]
		[NullAllowed, Export ("fence", ArgumentSemantic.Retain)]
		IMTLFence Fence { get; set; }

		[Abstract]
		[Export ("encodeToCommandBuffer:")]
		void Encode (IMTLCommandBuffer commandBuffer);
	}

	interface IMTLFXTemporalScaler {}

	[Mac (13,0), iOS (16,0)]
	[Protocol]
	interface MTLFXTemporalScaler
	{
		[Abstract]
		[Export ("colorTextureUsage")]
		MTLTextureUsage ColorTextureUsage { get; }

		[Abstract]
		[Export ("depthTextureUsage")]
		MTLTextureUsage DepthTextureUsage { get; }

		[Abstract]
		[Export ("motionTextureUsage")]
		MTLTextureUsage MotionTextureUsage { get; }

		[Abstract]
		[Export ("outputTextureUsage")]
		MTLTextureUsage OutputTextureUsage { get; }

		[Abstract]
		[Export ("inputContentWidth")]
		nuint InputContentWidth { get; set; }

		[Abstract]
		[Export ("inputContentHeight")]
		nuint InputContentHeight { get; set; }

		[Abstract]
		[NullAllowed, Export ("colorTexture", ArgumentSemantic.Retain)]
		IMTLTexture ColorTexture { get; set; }

		[Abstract]
		[NullAllowed, Export ("depthTexture", ArgumentSemantic.Retain)]
		IMTLTexture DepthTexture { get; set; }

		[Abstract]
		[NullAllowed, Export ("motionTexture", ArgumentSemantic.Retain)]
		IMTLTexture MotionTexture { get; set; }

		[Abstract]
		[NullAllowed, Export ("outputTexture", ArgumentSemantic.Retain)]
		IMTLTexture OutputTexture { get; set; }

		[Abstract]
		[NullAllowed, Export ("exposureTexture", ArgumentSemantic.Retain)]
		IMTLTexture ExposureTexture { get; set; }

		[Abstract]
		[Export ("preExposure")]
		float PreExposure { get; set; }

		[Abstract]
		[Export ("jitterOffsetX")]
		float JitterOffsetX { get; set; }

		[Abstract]
		[Export ("jitterOffsetY")]
		float JitterOffsetY { get; set; }

		[Abstract]
		[Export ("motionVectorScaleX")]
		float MotionVectorScaleX { get; set; }

		[Abstract]
		[Export ("motionVectorScaleY")]
		float MotionVectorScaleY { get; set; }

		[Abstract]
		[Export ("reset")]
		bool Reset { get; set; }

		[Abstract]
		[Export ("depthReversed")]
		bool DepthReversed { [Bind ("isDepthReversed")] get; set; }

		[Abstract]
		[Export ("colorTextureFormat")]
		MTLPixelFormat ColorTextureFormat { get; }

		[Abstract]
		[Export ("depthTextureFormat")]
		MTLPixelFormat DepthTextureFormat { get; }

		[Abstract]
		[Export ("motionTextureFormat")]
		MTLPixelFormat MotionTextureFormat { get; }

		[Abstract]
		[Export ("outputTextureFormat")]
		MTLPixelFormat OutputTextureFormat { get; }

		[Abstract]
		[Export ("inputWidth")]
		nuint InputWidth { get; }

		[Abstract]
		[Export ("inputHeight")]
		nuint InputHeight { get; }

		[Abstract]
		[Export ("outputWidth")]
		nuint OutputWidth { get; }

		[Abstract]
		[Export ("outputHeight")]
		nuint OutputHeight { get; }

		[Abstract]
		[Export ("inputContentMinScale")]
		float InputContentMinScale { get; }

		[Abstract]
		[Export ("inputContentMaxScale")]
		float InputContentMaxScale { get; }

		[Abstract]
		[NullAllowed, Export ("fence", ArgumentSemantic.Retain)]
		IMTLFence Fence { get; set; }

		[Abstract]
		[Export ("encodeToCommandBuffer:")]
		void Encode (IMTLCommandBuffer commandBuffer);
	}

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MTLFXSpatialScalerDescriptor : NSCopying
	{
		[Export ("colorTextureFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat ColorTextureFormat { get; set; }

		[Export ("outputTextureFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat OutputTextureFormat { get; set; }

		[Export ("inputWidth")]
		nuint InputWidth { get; set; }

		[Export ("inputHeight")]
		nuint InputHeight { get; set; }

		[Export ("outputWidth")]
		nuint OutputWidth { get; set; }

		[Export ("outputHeight")]
		nuint OutputHeight { get; set; }

		[Export ("colorProcessingMode", ArgumentSemantic.Assign)]
		MTLFXSpatialScalerColorProcessingMode ColorProcessingMode { get; set; }

		[Export ("newSpatialScalerWithDevice:")]
		[return: NullAllowed, Release]
		IMTLFXSpatialScaler Create (IMTLDevice device);

		[Static]
		[Export ("supportsDevice:")]
		bool SupportsDevice (IMTLDevice device);
	}

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MTLFXTemporalScalerDescriptor : NSCopying
	{
		[Export ("colorTextureFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat ColorTextureFormat { get; set; }

		[Export ("depthTextureFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat DepthTextureFormat { get; set; }

		[Export ("motionTextureFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat MotionTextureFormat { get; set; }

		[Export ("outputTextureFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat OutputTextureFormat { get; set; }

		[Export ("inputWidth")]
		nuint InputWidth { get; set; }

		[Export ("inputHeight")]
		nuint InputHeight { get; set; }

		[Export ("outputWidth")]
		nuint OutputWidth { get; set; }

		[Export ("outputHeight")]
		nuint OutputHeight { get; set; }

		[Export ("autoExposureEnabled")]
		bool AutoExposureEnabled { [Bind ("isAutoExposureEnabled")] get; set; }

		[Export ("inputContentPropertiesEnabled")]
		bool InputContentPropertiesEnabled { [Bind ("isInputContentPropertiesEnabled")] get; set; }

		[Export ("inputContentMinScale")]
		float InputContentMinScale { get; set; }

		[Export ("inputContentMaxScale")]
		float InputContentMaxScale { get; set; }

		[Export ("newTemporalScalerWithDevice:")]
		[return: NullAllowed, Release]
		IMTLFXTemporalScaler Create (IMTLDevice device);

		[Mac (14,0), iOS (17,0), MacCatalyst (17,0)]
		[Static]
		[Export ("supportedInputContentMinScaleForDevice:")]
		float SupportedInputContentMinScale (IMTLDevice device);

		[Mac (14,0), iOS (17,0), MacCatalyst (17,0)]
		[Static]
		[Export ("supportedInputContentMaxScaleForDevice:")]
		float SupportedInputContentMaxScale (IMTLDevice device);

		[Static]
		[Export ("supportsDevice:")]
		bool SupportsDevice (IMTLDevice device);
	}

}
