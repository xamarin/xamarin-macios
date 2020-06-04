//
// coreimage.cs: Definitions for CoreImage
//
// Copyright 2010, Novell, Inc.
// Copyright 2011-2013 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.ComponentModel;
using System.Reflection;
using AVFoundation;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreImage;
using CoreML;
using CoreVideo;
using ImageIO;
using IOSurface;
using Metal;
#if !MONOMAC
using OpenGLES;
using UIKit;
#else
using AppKit;
using ImageKit;
#endif

namespace CoreImage {

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIColor : NSSecureCoding, NSCopying {
		[Static]
		[Export ("colorWithCGColor:")]
		CIColor FromCGColor (CGColor c);

		[Static]
		[Export ("colorWithRed:green:blue:alpha:")]
		CIColor FromRgba (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("colorWithRed:green:blue:alpha:colorSpace:")]
		[return: NullAllowed]
		CIColor FromRgba (nfloat red, nfloat green, nfloat blue, nfloat alpha, CGColorSpace colorSpace);

		[Static]
		[Export ("colorWithRed:green:blue:")]
		CIColor FromRgb (nfloat red, nfloat green, nfloat blue);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("colorWithRed:green:blue:colorSpace:")]
		[return: NullAllowed]
		CIColor FromRgb (nfloat red, nfloat green, nfloat blue, CGColorSpace colorSpace);

		[Static]
		[Export ("colorWithString:")]
		CIColor FromString (string representation);

		[DesignatedInitializer]
		[Export ("initWithCGColor:")]
		IntPtr Constructor (CGColor c);

		[iOS (9,0)][Mac (10,11)]
		[Export ("initWithRed:green:blue:")]
		IntPtr Constructor (nfloat red, nfloat green, nfloat blue);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("initWithRed:green:blue:colorSpace:")]
		IntPtr Constructor (nfloat red, nfloat green, nfloat blue, CGColorSpace colorSpace);

		[iOS (9,0)][Mac (10,11)]
		[Export ("initWithRed:green:blue:alpha:")]
		IntPtr Constructor (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("initWithRed:green:blue:alpha:colorSpace:")]
		IntPtr Constructor (nfloat red, nfloat green, nfloat blue, nfloat alpha, CGColorSpace colorSpace);

		[Export ("numberOfComponents")]
		nint NumberOfComponents { get; }

		[Internal, Export ("components")]
		IntPtr GetComponents ();

		[Export ("alpha")]
		nfloat Alpha { get; }

		[Export ("colorSpace")]
		CGColorSpace ColorSpace { get; }

		[Export ("red")]
		nfloat Red { get; }

		[Export ("green")]
		nfloat Green { get; }

		[Export ("blue")]
		nfloat Blue { get; }

		// the above are the component of a CIColor instance
		// while the one below are CIColor convenience helpers
		// conflicting names means we have to keep the *Color suffix on them

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("blackColor", ArgumentSemantic.Strong)]
		CIColor BlackColor { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("whiteColor", ArgumentSemantic.Strong)]
		CIColor WhiteColor { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("grayColor", ArgumentSemantic.Strong)]
		CIColor GrayColor { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("redColor", ArgumentSemantic.Strong)]
		CIColor RedColor { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("greenColor", ArgumentSemantic.Strong)]
		CIColor GreenColor { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("blueColor", ArgumentSemantic.Strong)]
		CIColor BlueColor { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("cyanColor", ArgumentSemantic.Strong)]
		CIColor CyanColor { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("magentaColor", ArgumentSemantic.Strong)]
		CIColor MagentaColor { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("yellowColor", ArgumentSemantic.Strong)]
		CIColor YellowColor { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("clearColor", ArgumentSemantic.Strong)]
		CIColor ClearColor { get; }

		[Export ("stringRepresentation")]
		string StringRepresentation ();

#if !MONOMAC
		[Export ("initWithColor:")]
		IntPtr Constructor (UIColor color);
#else
		[Export ("initWithColor:")]
		IntPtr Constructor (NSColor color);
#endif
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIContext {
		// marked iOS5 but it's not working in iOS8.0
		[iOS (9,0)]
		[Export ("init")]
		IntPtr Constructor ();

		[iOS (9,0)][Mac (10,11)]
		[Static]
		[Export ("contextWithMTLDevice:")]
		CIContext FromMetalDevice (IMTLDevice device);

		[iOS (9,0)][Mac (10,11)]
		[Internal] // This overload is needed for our strong dictionary support (but only for Unified, since for Classic the generic version is transformed to this signature)
		[Static]
		[Export ("contextWithMTLDevice:options:")]
		CIContext FromMetalDevice (IMTLDevice device, [NullAllowed] NSDictionary options);

		[iOS (9,0)][Mac (10,11)]
		[Static]
		[Export ("contextWithMTLDevice:options:")]
		CIContext FromMetalDevice (IMTLDevice device, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[iOS (9,0)]
		[Internal, Static]
		[Export ("contextWithCGContext:options:")]
		CIContext FromContext (CGContext ctx, [NullAllowed] NSDictionary options);

		[Static, Internal]
		[Export ("contextWithOptions:")]
		CIContext FromOptions ([NullAllowed] NSDictionary dictionary);

		[iOS (9,0)] // documented as earlier but missing
		[Internal]
		[Export ("initWithOptions:")]
		IntPtr Constructor ([NullAllowed] NSDictionary options);

		[Static]
		[Export ("context")]
		CIContext Create ();

#if !MONOMAC
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Static]
		[Export ("contextWithEAGLContext:")]
		CIContext FromContext (EAGLContext eaglContext);

		[Deprecated (PlatformName.iOS, 12, 0)]
		[Static]
		[Export ("contextWithEAGLContext:options:")]
		CIContext FromContext (EAGLContext eaglContext, [NullAllowed] NSDictionary dictionary);
#endif

		[Mac (10,11)]
		[Export ("render:toCVPixelBuffer:")]
		void Render (CIImage image, CVPixelBuffer buffer);

		[Mac (10,11)]
		[Export ("render:toCVPixelBuffer:bounds:colorSpace:")]
		// null is not documented for CGColorSpace but it makes sense with the other overload not having this parameter (unit tested)
		void Render (CIImage image, CVPixelBuffer buffer, CGRect rectangle, [NullAllowed] CGColorSpace cs);

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Export ("render:toIOSurface:bounds:colorSpace:")]
		void Render (CIImage image, IOSurface.IOSurface surface, CGRect bounds, [NullAllowed] CGColorSpace colorSpace);

#if !MONOMAC
		[Export ("inputImageMaximumSize")]
		CGSize InputImageMaximumSize { get; }

		[Export ("outputImageMaximumSize")]
		CGSize OutputImageMaximumSize { get; }
#endif

		[iOS (9,0)][Mac (10,11)]
		[Export ("render:toMTLTexture:commandBuffer:bounds:colorSpace:")]
		void Render (CIImage image, IMTLTexture texture, [NullAllowed] IMTLCommandBuffer commandBuffer, CGRect bounds, [NullAllowed] CGColorSpace colorSpace);

		[Deprecated (PlatformName.iOS, 6, 0, message : "Use 'DrawImage (image, CGRect, CGRect)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message : "Use 'DrawImage (image, CGRect, CGRect)' instead.")]
		[Export ("drawImage:atPoint:fromRect:")]
		void DrawImage (CIImage image, CGPoint atPoint, CGRect fromRect);

		[Export ("drawImage:inRect:fromRect:")]
		void DrawImage (CIImage image, CGRect inRectangle, CGRect fromRectangle);

		[Export ("createCGImage:fromRect:")]
		[return: Release ()]
		[return: NullAllowed]
		CGImage CreateCGImage (CIImage image, CGRect fromRectangle);

		[Export ("createCGImage:fromRect:format:colorSpace:")]
		[return: Release ()]
		[return: NullAllowed]
		CGImage CreateCGImage (CIImage image, CGRect fromRect, int /* CIFormat = int */ ciImageFormat, [NullAllowed] CGColorSpace colorSpace);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("createCGImage:fromRect:format:colorSpace:deferred:")]
		[return: Release]
		[return: NullAllowed]
		CGImage CreateCGImage (CIImage image, CGRect fromRect, CIFormat format, [NullAllowed] CGColorSpace colorSpace, bool deferred);

#if MONOMAC
		[Internal, Export ("createCGLayerWithSize:info:")]
		CGLayer CreateCGLayer (CGSize size, [NullAllowed] NSDictionary info);
#endif

		[Export ("render:toBitmap:rowBytes:bounds:format:colorSpace:")]
		void RenderToBitmap (CIImage image, IntPtr bitmapPtr, nint bytesPerRow, CGRect bounds, int /* CIFormat = int */ bitmapFormat, [NullAllowed] CGColorSpace colorSpace);

		//[Export ("render:toIOSurface:bounds:colorSpace:")]
		//void RendertoIOSurfaceboundscolorSpace (CIImage im, IOSurfaceRef surface, CGRect r, CGColorSpaceRef cs, );

#if MONOMAC
		[Export ("reclaimResources")]
		void ReclaimResources ();
#endif

		[iOS (10,0)]
		[TV (10,0)]
		[Export ("clearCaches")]
		void ClearCaches ();

		[Internal, Field ("kCIContextOutputColorSpace", "+CoreImage")]
		NSString OutputColorSpace { get; }

		[Internal, Field ("kCIContextWorkingColorSpace", "+CoreImage")]
		NSString _WorkingColorSpace { get; }
		
		[Internal, Field ("kCIContextUseSoftwareRenderer", "+CoreImage")]
		NSString UseSoftwareRenderer { get; }

		[iOS (8,0)][Mac (10,12)]
		[Internal, Field ("kCIContextPriorityRequestLow", "+CoreImage")]
		NSString PriorityRequestLow { get; }

		[iOS (8,0)]
		[Internal, Field ("kCIContextWorkingFormat", "+CoreImage")]
		NSString WorkingFormatField { get; }

		[iOS (9,0)][Mac (10,11)]
		[Internal]
		[Field ("kCIContextHighQualityDownsample", "+CoreImage")]
		NSString HighQualityDownsample { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Internal]
		[Field ("kCIContextAllowLowPower")]
		NSString AllowLowPower { get; }


#if MONOMAC
		[Mac(10,11)]
		[Export ("offlineGPUCount")]
		[Static]
		int OfflineGPUCount { get; }

		[Mac(10,11)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Export ("contextForOfflineGPUAtIndex:")]
		[Static]
		CIContext FromOfflineGpu (int gpuIndex);
#endif

		[iOS (9,0)][Mac (10,11)]
		[Export ("workingColorSpace")]
		CGColorSpace WorkingColorSpace { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("workingFormat")]
		CIFormat WorkingFormat { get; }

		[iOS (7,0)]
		[Internal]
		[Field ("kCIContextOutputPremultiplied", "+CoreImage")]
		NSString OutputPremultiplied { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Internal]
		[Field ("kCIContextCacheIntermediates", "+CoreImage")]
		NSString CacheIntermediates { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("contextWithMTLCommandQueue:")]
		CIContext Create (IMTLCommandQueue commandQueue);

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("contextWithMTLCommandQueue:options:")]
		CIContext Create (IMTLCommandQueue commandQueue, [NullAllowed] NSDictionary<NSString, NSObject> options);
	}

	[Category]
	[BaseType (typeof (CIContext))]
	interface CIContext_ImageRepresentation {

		[iOS (10,0)][Mac (10,12)]
		[Export ("TIFFRepresentationOfImage:format:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetTiffRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, NSDictionary options);

		[iOS (10,0)][Mac (10,12)]
		[Wrap ("GetTiffRepresentation (This, image, format, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetTiffRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		[iOS (10,0)][Mac (10,12)]
		[Export ("JPEGRepresentationOfImage:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetJpegRepresentation (CIImage image, CGColorSpace colorSpace, NSDictionary options);

		[iOS (10,0)][Mac (10,12)]
		[Wrap ("GetJpegRepresentation (This, image, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetJpegRepresentation (CIImage image, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		[iOS (11,0)][TV (11,0)][Mac (10,13,4)]
		[Export ("HEIFRepresentationOfImage:format:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetHeifRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, NSDictionary options);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[Wrap ("GetHeifRepresentation (This, image, format, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetHeifRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[Export ("PNGRepresentationOfImage:format:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetPngRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, NSDictionary options);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[Wrap ("GetPngRepresentation (This, image, format, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetPngRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		[iOS (10,0)][Mac (10,12)]
		[Export ("writeTIFFRepresentationOfImage:toURL:format:colorSpace:options:error:")]
		bool WriteTiffRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, NSDictionary options, out NSError error);

		[iOS (10,0)][Mac (10,12)]
		[Wrap ("WriteTiffRepresentation (This, image, url, format, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteTiffRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options, out NSError error);

		[iOS (10,0)][Mac (10,12)]
		[Export ("writeJPEGRepresentationOfImage:toURL:colorSpace:options:error:")]
		bool WriteJpegRepresentation (CIImage image, NSUrl url, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		[iOS (10,0)][Mac (10,12)]
		[Wrap ("WriteJpegRepresentation (This, image, url, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteJpegRepresentation (CIImage image, NSUrl url, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);

		[iOS (11,0)][TV (11,0)][Mac (10,13,4)]
		[Export ("writeHEIFRepresentationOfImage:toURL:format:colorSpace:options:error:")]
		bool WriteHeifRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[Wrap ("WriteHeifRepresentation (This, image, url, format, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteHeifRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[Export ("writePNGRepresentationOfImage:toURL:format:colorSpace:options:error:")]
		bool WritePngRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[Wrap ("WritePngRepresentation (This, image, url, format, colorSpace, options.GetDictionary ()!, out error)")]
		bool WritePngRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);
	}


	[Category]
	[BaseType (typeof(CIContext))]
	interface CIContext_CIDepthBlurEffect
	{
		// as per the docs: The 'options' parameter is a key value/pair reserved for future use.
		[TV (12,0), iOS (12,0), Mac (10,14)]
		[Export ("depthBlurEffectFilterForImageURL:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (NSUrl url, [NullAllowed] NSDictionary options);

		// as per the docs: The 'options' parameter is a key value/pair reserved for future use.
		[TV (12,0), iOS (12,0), Mac (10,14)]
		[Export ("depthBlurEffectFilterForImageData:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (NSData data, [NullAllowed] NSDictionary options);

		// as per the docs: The 'options' parameter is a key value/pair reserved for future use.
		[TV (12,0), iOS (12,0), Mac (10,14)]
		[Export ("depthBlurEffectFilterForImage:disparityImage:portraitEffectsMatte:orientation:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (CIImage image, CIImage disparityImage, [NullAllowed] CIImage portraitEffectsMatte, CGImagePropertyOrientation orientation, [NullAllowed] NSDictionary options);

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Export ("depthBlurEffectFilterForImage:disparityImage:portraitEffectsMatte:hairSemanticSegmentation:orientation:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (CIImage image, CIImage disparityImage, [NullAllowed] CIImage portraitEffectsMatte, [NullAllowed] CIImage hairSemanticSegmentation, CGImagePropertyOrientation orientation, [NullAllowed] NSDictionary options);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] //  In iOS8 they expose custom filters, we expose a protected one in CIFilter.cs
	interface CIFilter : NSSecureCoding, NSCopying {
		[Export ("inputKeys")]
		string [] InputKeys { get; }

		[Export ("outputKeys")]
		string [] OutputKeys { get; }

		[Export ("setDefaults")]
		void SetDefaults ();

		[Export ("attributes")]
		NSDictionary Attributes { get; }

		[Export ("name")]
		string Name {
			get;
			[iOS (10,0)][TV (10,0)]
			set;
		 }

		[Static]
		[Export ("filterWithName:")]
		CIFilter FromName (string name);

		[iOS (8,0), Mac (10,10)]
		[Static]
		[Export ("filterWithName:withInputParameters:")]
		CIFilter GetFilter (string name, [NullAllowed] NSDictionary inputParameters);

		[Static]
		[Export ("filterNamesInCategory:")]
		string [] FilterNamesInCategory ([NullAllowed] string category);

		[Static]
		[Export ("filterNamesInCategories:"), Internal]
		string [] _FilterNamesInCategories ([NullAllowed] string [] categories);

		[iOS(9,0)]
		[Static]
		[Export ("localizedNameForFilterName:")]
		string FilterLocalizedName (string filterName);

		[iOS(9,0)]
		[Static]
		[Export ("localizedNameForCategory:")]
		string CategoryLocalizedName (string category);

		[iOS(9,0)]
		[Static]
		[Export ("localizedDescriptionForFilterName:")]
		string FilterLocalizedDescription (string filterName);

		[iOS(9,0)]
		[Static]
		[Export ("localizedReferenceDocumentationForFilterName:")]
		NSUrl FilterLocalizedReferenceDocumentation (string filterName);

#if MONOMAC && !XAMCORE_4_0
		[Static]
		[Export ("registerFilterName:constructor:classAttributes:")]
		void RegisterFilterName (string name, NSObject constructorObject, NSDictionary classAttributes);
#else
		[iOS(9,0)]
		[Static]
		[Export ("registerFilterName:constructor:classAttributes:")]
#if XAMCORE_4_0
		void RegisterFilterName (string name, ICIFilterConstructor constructorObject, NSDictionary<NSString, NSObject> classAttributes);
#else
		[Advice ("The 'constructorObject' argument must implement 'ICIFilterConstructor'.")]
		void RegisterFilterName (string name, NSObject constructorObject, NSDictionary<NSString, NSObject> classAttributes);
#endif
#endif

#if MONOMAC
		[Export ("apply:arguments:options:")]
		CIImage Apply (CIKernel k, [NullAllowed] NSArray args, [NullAllowed] NSDictionary options);

		[Export ("viewForUIConfiguration:excludedKeys:")]
		IKFilterUIView GetFilterUIView (NSDictionary configurationOptions, [NullAllowed] NSArray excludedKeys);

#else
		// added in 10.10 but it was already accessible in a different way before (manual bindings)
		[Export ("outputImage")]
		CIImage OutputImage { get; }

		[Export ("serializedXMPFromFilters:inputImageExtent:"), Static]
		NSData SerializedXMP (CIFilter[] filters, CGRect extent); 

		[Export ("filterArrayFromSerializedXMP:inputImageExtent:error:"), Static]
		CIFilter[] FromSerializedXMP (NSData xmpData, CGRect extent, out NSError error);
#endif

		[Export ("setValue:forKey:"), Internal]
		void SetValueForKey ([NullAllowed] NSObject value, NSString key);

		[Export ("valueForKey:"), Internal]
		NSObject ValueForKey (NSString key);

		// CIRAWFilter (CIFilter)

		[iOS (10,0)]
		[TV (10,0)]
		[Static]
		[Export ("filterWithImageURL:options:")]
		CIFilter CreateRawFilter (NSUrl url, NSDictionary options);

		[iOS (10,0)]
		[TV (10,0)]
		[Static]
		[Wrap ("CreateRawFilter (url, options.GetDictionary ()!)")]
		CIFilter CreateRawFilter (NSUrl url, CIRawFilterOptions options);

		[iOS (10,0)]
		[TV (10,0)]
		[Static]
		[Export ("filterWithImageData:options:")]
		CIFilter CreateRawFilter (NSData data, NSDictionary options);

		[iOS (10,0)]
		[TV (10,0)]
		[Static]
		[Wrap ("CreateRawFilter (data, options.GetDictionary ()!)")]
		CIFilter CreateRawFilter (NSData data, CIRawFilterOptions options);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Export ("filterWithCVPixelBuffer:properties:options:")]
		CIFilter CreateRawFilter (CVPixelBuffer pixelBuffer, NSDictionary properties, NSDictionary options);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Static]
		[Wrap ("CreateRawFilter (pixelBuffer, properties, options.GetDictionary ()!)")]
		CIFilter CreateRawFilter (CVPixelBuffer pixelBuffer, NSDictionary properties, CIRawFilterOptions options);
	}

	[TV (10,0)]
	[Static]
	[Internal]
	interface CIRawFilterKeys {
		[iOS (10,0)]
		[Field ("kCIInputAllowDraftModeKey")]
		NSString AllowDraftModeKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputDecoderVersionKey")]
		NSString VersionKey { get; }

		[iOS (10,0)]
		[Field ("kCISupportedDecoderVersionsKey")]
		NSString SupportedDecoderVersionsKey { get; }

		[iOS (10,0)][Mac (10,12)]
		[Field ("kCIInputBaselineExposureKey")]
		NSString BaselineExposureKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputBoostKey")]
		NSString BoostKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputBoostShadowAmountKey")]
		NSString BoostShadowAmountKey { get; }

		[iOS (10,0)][Mac (10,12)]
		[Field ("kCIInputDisableGamutMapKey")]
		NSString DisableGamutMapKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputNeutralChromaticityXKey")]
		NSString NeutralChromaticityXKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputNeutralChromaticityYKey")]
		NSString NeutralChromaticityYKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputNeutralTemperatureKey")]
		NSString NeutralTemperatureKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputNeutralTintKey")]
		NSString NeutralTintKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputNeutralLocationKey")]
		NSString NeutralLocationKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputScaleFactorKey")]
		NSString ScaleFactorKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputIgnoreImageOrientationKey")]
		NSString IgnoreImageOrientationKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputImageOrientationKey")]
		NSString ImageOrientationKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputEnableSharpeningKey")]
		NSString EnableSharpeningKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputEnableChromaticNoiseTrackingKey")]
		NSString EnableChromaticNoiseTrackingKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputNoiseReductionAmountKey")]
		NSString NoiseReductionAmountKey { get; }

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[Field ("kCIInputMoireAmountKey")]
		NSString MoireAmountKey { get; }

		[iOS (10,0)][Mac (10,10)]
		[Field ("kCIInputEnableVendorLensCorrectionKey")]
		NSString EnableVendorLensCorrectionKey { get; }

		[iOS (10,0)][Mac (10,10)]
		[Field ("kCIInputLuminanceNoiseReductionAmountKey")]
		NSString LuminanceNoiseReductionAmountKey { get; }

		[iOS (10,0)][Mac (10,10)]
		[Field ("kCIInputColorNoiseReductionAmountKey")]
		NSString ColorNoiseReductionAmountKey { get; }

		[iOS (10,0)][Mac (10,10)]
		[Field ("kCIInputNoiseReductionSharpnessAmountKey")]
		NSString NoiseReductionSharpnessAmountKey { get; }

		[iOS (10,0)][Mac (10,10)]
		[Field ("kCIInputNoiseReductionContrastAmountKey")]
		NSString NoiseReductionContrastAmountKey { get; }

		[iOS (10,0)][Mac (10,10)]
		[Field ("kCIInputNoiseReductionDetailAmountKey")]
		NSString NoiseReductionDetailAmountKey { get; }

		[iOS (10,0)]
		[Field ("kCIInputLinearSpaceFilter")]
		NSString LinearSpaceFilterKey { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Field ("kCIInputEnableEDRModeKey")]
		NSString EnableEdrModeKey { get; }

		[iOS (10,0)]
		[Field ("kCIOutputNativeSizeKey")]
		NSString OutputNativeSizeKey { get; }

		[iOS (10,0)]
		[Field ("kCIActiveKeys")]
		NSString ActiveKeysKey { get; }
	}

	[TV (10,0)]
	[StrongDictionary ("CIRawFilterKeys")]
	interface CIRawFilterOptions {

		[iOS (10,0)]
		bool AllowDraftMode { get; set; }

		[iOS (10,0)]
		string Version { get; set; }

		[iOS (10,0)]
		NSDictionary[] SupportedDecoderVersions { get; set; }

		[iOS (10,0)][Mac (10,12)]
		float BaselineExposure { get; set; }

		[iOS (10,0)]
		float Boost { get; set; }

		[iOS (10,0)]
		float BoostShadowAmount { get; set; }

		[iOS (10,0)][Mac (10,12)]
		bool DisableGamutMap { get; set; }

		[iOS (10,0)]
		float NeutralChromaticityX { get; set; }

		[iOS (10,0)]
		float NeutralChromaticityY { get; set; }

		[iOS (10,0)]
		float NeutralTemperature { get; set; }

		[iOS (10,0)]
		float NeutralTint { get; set; }

		[iOS (10,0)]
		CIVector NeutralLocation { get; set; }

		[iOS (10,0)]
		float ScaleFactor { get; set; }

		[iOS (10,0)]
		bool IgnoreImageOrientation { get; set; }

		[iOS (10,0)]
		int ImageOrientation { get; set; }

		[iOS (10,0)]
		bool EnableSharpening { get; set; }

		[iOS (10,0)]
		bool EnableChromaticNoiseTracking { get; set; }

		[iOS (10,0)]
		double NoiseReductionAmount { get; set; }

		[iOS (10,0)][Mac (10,10)]
		bool EnableVendorLensCorrection { get; set; }

		[iOS (10,0)][Mac (10,10)]
		double LuminanceNoiseReductionAmount { get; set; }

		[iOS (10,0)][Mac (10,10)]
		double ColorNoiseReductionAmount { get; set; }

		[iOS (10,0)][Mac (10,10)]
		double NoiseReductionSharpnessAmount { get; set; }

		[iOS (10,0)][Mac (10,10)]
		double NoiseReductionContrastAmount { get; set; }

		[iOS (10,0)][Mac (10,10)]
		[Field ("kCIInputNoiseReductionDetailAmountKey")]
		double NoiseReductionDetailAmount { get; set; }

		[iOS (10,0)]
		CIFilter LinearSpaceFilter { get; set; }

		[iOS (10,0)]
		CIVector OutputNativeSize { get; set; }

		[iOS (10,0)]
		NSSet ActiveKeys { get; }
	}

	[Static]
	interface CIFilterOutputKey {
		[Field ("kCIOutputImageKey", "+CoreImage")]
		NSString Image  { get; }
	}
	
	[Static]
	interface CIFilterInputKey {
		[Field ("kCIInputBackgroundImageKey", "+CoreImage")]
		NSString BackgroundImage  { get; }

		[Field ("kCIInputImageKey", "+CoreImage")]
		NSString Image  { get; }

		[Mac(10,11)]
		[Field ("kCIInputVersionKey", "+CoreImage")]
		NSString Version { get; }

		[iOS(9,0)]
		[Field ("kCIInputRefractionKey", "+CoreImage")]
		NSString Refraction  { get; }

		[iOS(9,0)]
		[Field ("kCIInputGradientImageKey", "+CoreImage")]
		NSString GradientImage  { get; }

		[iOS(9,0)]
		[Field ("kCIInputShadingImageKey", "+CoreImage")]
		NSString ShadingImage  { get; }

		[iOS (7,0)]
		[Field ("kCIInputTimeKey", "+CoreImage")]
		NSString Time  { get; }

		[iOS (7,0)]
		[Field ("kCIInputTransformKey", "+CoreImage")]
		NSString Transform  { get; }

		[iOS (7,0)]
		[Field ("kCIInputScaleKey", "+CoreImage")]
		NSString Scale  { get; }

		[iOS (7,0)]
		[Field ("kCIInputAspectRatioKey", "+CoreImage")]
		NSString AspectRatio  { get; }

		[iOS (7,0)]
		[Field ("kCIInputCenterKey", "+CoreImage")]
		NSString Center  { get; }

		[iOS (7,0)]
		[Field ("kCIInputRadiusKey", "+CoreImage")]
		NSString Radius  { get; }

		[iOS (7,0)]
		[Field ("kCIInputAngleKey", "+CoreImage")]
		NSString Angle  { get; }

		[iOS (7,0)]
		[Field ("kCIInputWidthKey", "+CoreImage")]
		NSString Width  { get; }

		[iOS (7,0)]
		[Field ("kCIInputSharpnessKey", "+CoreImage")]
		NSString Sharpness  { get; }

		[iOS (7,0)]
		[Field ("kCIInputIntensityKey", "+CoreImage")]
		NSString Intensity  { get; }

		[iOS (7,0)]
		[Field ("kCIInputEVKey", "+CoreImage")]
		NSString EV  { get; }

		[iOS (7,0)]
		[Field ("kCIInputSaturationKey", "+CoreImage")]
		NSString Saturation  { get; }

		[iOS (7,0)]
		[Field ("kCIInputColorKey", "+CoreImage")]
		NSString Color  { get; }

		[iOS (7,0)]
		[Field ("kCIInputBrightnessKey", "+CoreImage")]
		NSString Brightness  { get; }

		[iOS (7,0)]
		[Field ("kCIInputContrastKey", "+CoreImage")]
		NSString Contrast  { get; }

		[iOS (9,0)]
		[Field ("kCIInputBiasKey", "+CoreImage")]
		NSString BiasKey  { get; }

		[iOS (9,0)][Mac (10,11)]
		[Field ("kCIInputWeightsKey", "+CoreImage")]
		NSString WeightsKey { get; }

		[iOS (7,0)]
		[Field ("kCIInputMaskImageKey", "+CoreImage")]
		NSString MaskImage  { get; }

		[iOS (7,0)]
		[Field ("kCIInputTargetImageKey", "+CoreImage")]
		NSString TargetImage  { get; }

		[iOS (7,0)]
		[Field ("kCIInputExtentKey", "+CoreImage")]
		NSString Extent  { get; }

		[iOS (11,0), TV (11,0), Mac (10,13)]
		[Field ("kCIInputDepthImageKey", "+CoreImage")]
		NSString DepthImage { get; }

		[iOS (11,0), TV (11,0), Mac (10,13)]
		[Field ("kCIInputDisparityImageKey", "+CoreImage")]
		NSString DisparityImage { get; }

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		[Field ("kCIInputMatteImageKey", "+CoreImage")]
		NSString MatteImage { get; }

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		[Field ("kCIInputAmountKey", "+CoreImage")]
		NSString Amount { get; }
	}
		
	[Static]
	interface CIFilterAttributes {
		[Field ("kCIAttributeFilterName", "+CoreImage")]
		NSString FilterName  { get; }

		[Field ("kCIAttributeFilterDisplayName", "+CoreImage")]
		NSString FilterDisplayName  { get; }

		[iOS(9,0)]
		[Field ("kCIAttributeDescription", "+CoreImage")]
		NSString Description  { get; }

		[iOS (9,0)]
		[Field ("kCIAttributeReferenceDocumentation", "+CoreImage")]
		NSString ReferenceDocumentation  { get; }

		[Field ("kCIAttributeFilterCategories", "+CoreImage")]
		NSString FilterCategories  { get; }

		[Field ("kCIAttributeClass", "+CoreImage")]
		NSString Class  { get; }

		[Field ("kCIAttributeType", "+CoreImage")]
		NSString Type  { get; }

		[Field ("kCIAttributeMin", "+CoreImage")]
		NSString Min  { get; }

		[Field ("kCIAttributeMax", "+CoreImage")]
		NSString Max  { get; }

		[Field ("kCIAttributeSliderMin", "+CoreImage")]
		NSString SliderMin  { get; }

		[Field ("kCIAttributeSliderMax", "+CoreImage")]
		NSString SliderMax  { get; }

		[Field ("kCIAttributeDefault", "+CoreImage")]
		NSString Default  { get; }

		[Field ("kCIAttributeIdentity", "+CoreImage")]
		NSString Identity  { get; }

		[Field ("kCIAttributeName", "+CoreImage")]
		NSString Name  { get; }

		[Field ("kCIAttributeDisplayName", "+CoreImage")]
		NSString DisplayName  { get; }

		[iOS(9,0)]
		[Field ("kCIUIParameterSet", "+CoreImage")]
		NSString UIParameterSet  { get; }

		[Field ("kCIAttributeTypeTime", "+CoreImage")]
		NSString TypeTime  { get; }

		[Field ("kCIAttributeTypeScalar", "+CoreImage")]
		NSString TypeScalar  { get; }

		[Field ("kCIAttributeTypeDistance", "+CoreImage")]
		NSString TypeDistance  { get; }

		[Field ("kCIAttributeTypeAngle", "+CoreImage")]
		NSString TypeAngle  { get; }

		[Field ("kCIAttributeTypeBoolean", "+CoreImage")]
		NSString TypeBoolean  { get; }

		[Field ("kCIAttributeTypeInteger", "+CoreImage")]
		NSString TypeInteger  { get; }

		[Field ("kCIAttributeTypeCount", "+CoreImage")]
		NSString TypeCount  { get; }

		[Field ("kCIAttributeTypePosition", "+CoreImage")]
		NSString TypePosition  { get; }

		[Field ("kCIAttributeTypeOffset", "+CoreImage")]
		NSString TypeOffset  { get; }

		[Field ("kCIAttributeTypePosition3", "+CoreImage")]
		NSString TypePosition3  { get; }

		[Field ("kCIAttributeTypeRectangle", "+CoreImage")]
		NSString TypeRectangle  { get; }

		[iOS (9,0)]
		[Field ("kCIAttributeTypeOpaqueColor", "+CoreImage")]
		NSString TypeOpaqueColor  { get; }

		[iOS (9,0)]
		[Field ("kCIAttributeTypeGradient", "+CoreImage")]
		NSString TypeGradient  { get; }

		[Mac(10,11)]
		[Field ("kCIAttributeTypeImage", "+CoreImage")]
		NSString TypeImage  { get; }

		[Mac(10,11)]
		[Field ("kCIAttributeTypeTransform", "+CoreImage")]
		NSString TypeTransform  { get; }

		[Mac(10,11)]
		[Field ("kCIAttributeTypeColor", "+CoreImage")]
		NSString TypeColor  { get; }

		[iOS(9,0), Mac(10,11)]
		[Field ("kCIAttributeFilterAvailable_Mac", "+CoreImage")]
		NSString Available_Mac { get; }

		[iOS(9,0), Mac(10,11)]
		[Field ("kCIAttributeFilterAvailable_iOS", "+CoreImage")]
		NSString Available_iOS { get; }
	}

	[Static]
	interface CIFilterCategory {
		[Field ("kCICategoryDistortionEffect", "+CoreImage")]
		NSString DistortionEffect  { get; }

		[Field ("kCICategoryGeometryAdjustment", "+CoreImage")]
		NSString GeometryAdjustment  { get; }

		[Field ("kCICategoryCompositeOperation", "+CoreImage")]
		NSString CompositeOperation  { get; }

		[Field ("kCICategoryHalftoneEffect", "+CoreImage")]
		NSString HalftoneEffect  { get; }

		[Field ("kCICategoryColorAdjustment", "+CoreImage")]
		NSString ColorAdjustment  { get; }

		[Field ("kCICategoryColorEffect", "+CoreImage")]
		NSString ColorEffect  { get; }

		[Field ("kCICategoryTransition", "+CoreImage")]
		NSString Transition  { get; }

		[Field ("kCICategoryTileEffect", "+CoreImage")]
		NSString TileEffect  { get; }

		[Field ("kCICategoryGenerator", "+CoreImage")]
		NSString Generator  { get; }

		[Field ("kCICategoryReduction", "+CoreImage")]
		NSString Reduction  { get; }

		[Field ("kCICategoryGradient", "+CoreImage")]
		NSString Gradient  { get; }

		[Field ("kCICategoryStylize", "+CoreImage")]
		NSString Stylize  { get; }

		[Field ("kCICategorySharpen", "+CoreImage")]
		NSString Sharpen  { get; }

		[Field ("kCICategoryBlur", "+CoreImage")]
		NSString Blur  { get; }

		[Field ("kCICategoryVideo", "+CoreImage")]
		NSString Video  { get; }

		[Field ("kCICategoryStillImage", "+CoreImage")]
		NSString StillImage  { get; }

		[Field ("kCICategoryInterlaced", "+CoreImage")]
		NSString Interlaced  { get; }

		[Field ("kCICategoryNonSquarePixels", "+CoreImage")]
		NSString NonSquarePixels  { get; }

		[Field ("kCICategoryHighDynamicRange", "+CoreImage")]
		NSString HighDynamicRange  { get; }

		[Field ("kCICategoryBuiltIn", "+CoreImage")]
		NSString BuiltIn  { get; }

		[iOS(9,0)]
		[Field ("kCICategoryFilterGenerator", "+CoreImage")]
		NSString FilterGenerator  { get; }
	}

	[iOS (9,0)]
	[Protocol]
	interface CIFilterConstructor
	{
		[Abstract]
		[Export ("filterWithName:")]
		[return: NullAllowed]
		CIFilter FilterWithName (string name);
	}
	
	[Static]
	[iOS (9,0)]
	interface CIUIParameterSet {
		[Field ("kCIUISetBasic", "+CoreImage")]
		NSString Basic  { get; }

		[Field ("kCIUISetIntermediate", "+CoreImage")]
		NSString Intermediate  { get; }

		[Field ("kCIUISetAdvanced", "+CoreImage")]
		NSString Advanced  { get; }

		[Field ("kCIUISetDevelopment", "+CoreImage")]
		NSString Development  { get; }
	}

#if MONOMAC
	[Static]
	interface CIFilterApply {
		[Field ("kCIApplyOptionExtent", "+CoreImage")]
		NSString OptionExtent  { get; }

		[Field ("kCIApplyOptionDefinition", "+CoreImage")]
		NSString OptionDefinition  { get; }

		[Field ("kCIApplyOptionUserInfo", "+CoreImage")]
		NSString OptionUserInfo  { get; }

		[Field ("kCIApplyOptionColorSpace", "+CoreImage")]
		NSString OptionColorSpace  { get; }
	}
#endif
	
#if MONOMAC
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIFilterGenerator : CIFilterConstructor, NSSecureCoding, NSCopying {
		[Static, Export ("filterGenerator")]
		CIFilterGenerator Create ();

		[Static]
		[Export ("filterGeneratorWithContentsOfURL:")]
		CIFilterGenerator FromUrl (NSUrl aURL);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl aURL);

		[Export ("connectObject:withKey:toObject:withKey:")]
		void ConnectObject (NSObject sourceObject, string withSourceKey, NSObject targetObject, string targetKey);

		[Export ("disconnectObject:withKey:toObject:withKey:")]
		void DisconnectObject (NSObject sourceObject, string sourceKey, NSObject targetObject, string targetKey);

		[Export ("exportKey:fromObject:withName:")]
		void ExportKey (string key, NSObject targetObject, string exportedKeyName);

		[Export ("removeExportedKey:")]
		void RemoveExportedKey (string exportedKeyName);

		[Export ("exportedKeys")]
		NSDictionary ExportedKeys { get; }

		[Export ("setAttributes:forExportedKey:")]
		void SetAttributesforExportedKey (NSDictionary attributes, NSString exportedKey);

		[Export ("filter")]
		CIFilter CreateFilter ();

		[Export ("registerFilterName:")]
		void RegisterFilterName (string name);

		[Export ("writeToURL:atomically:")]
		bool Save (NSUrl toUrl, bool atomically);

		//Detected properties
		[Export ("classAttributes")]
		NSDictionary ClassAttributes { get; set; }

		[Field ("kCIFilterGeneratorExportedKey", "+CoreImage")]
		NSString ExportedKey { get; }

		[Field ("kCIFilterGeneratorExportedKeyTargetObject", "+CoreImage")]
		NSString ExportedKeyTargetObject { get; }

		[Field ("kCIFilterGeneratorExportedKeyName", "+CoreImage")]
		NSString ExportedKeyName { get; }
	}

#endif
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[iOS(9,0)]
	interface CIFilterShape : NSCopying {
		[Static]
		[Export ("shapeWithRect:")]
		CIFilterShape FromRect (CGRect rect);

		[Export ("initWithRect:")]
		IntPtr Constructor (CGRect rect);

		[Export ("transformBy:interior:")]
		CIFilterShape Transform (CGAffineTransform transformation, bool interiorFlag);

		[Export ("insetByX:Y:")]
		CIFilterShape Inset (int /* int, not NSInteger */ dx, int /* int, not NSInteger */  dy);

		[Export ("unionWith:")]
		CIFilterShape Union (CIFilterShape other);

		[Export ("unionWithRect:")]
		CIFilterShape Union (CGRect rectangle);

		[Export ("intersectWith:")]
		CIFilterShape Intersect (CIFilterShape other);

		[Export ("intersectWithRect:")]
		CIFilterShape Intersect (CGRect rectangle);

		[Export ("extent")]
		CGRect Extent { get; }
	}

	[StrongDictionary ("CIImageInitializationOptionsKeys")]
	interface CIImageInitializationOptions {
		// Bug #60726: [Generator] Support INativeObject in StrongDictionary
		// (https://bugzilla.xamarin.com/show_bug.cgi?id=60726)
		// CGColorSpace ColorSpace { get; set; }

		CoreGraphics.CGImageProperties Properties { get; set; }

		[iOS (11,0), TV (11,0), Mac (10,13)]
		bool ApplyOrientationProperty { get; set; }

		[iOS (11,0), TV (11,0), Mac (10,13)]
		bool NearestSampling { get; set; }

		[iOS (11,0), TV (11,0), Mac (10,13)]
		bool AuxiliaryDepth { get; set; }

		[iOS (11,0), TV (11,0), Mac (10,13)]
		bool AuxiliaryDisparity { get; set; }

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		bool AuxiliaryPortraitEffectsMatte { get; set; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		bool AuxiliarySemanticSegmentationSkinMatte { get; set; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		bool AuxiliarySemanticSegmentationHairMatte { get; set; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		bool AuxiliarySemanticSegmentationTeethMatte { get; set; }
	}

	[Internal]
	[Static]
	interface CIImageInitializationOptionsKeys {
		[Field ("kCIImageColorSpace")]
		NSString ColorSpaceKey { get; }

		[Field ("kCIImageProperties")]
		NSString PropertiesKey { get; }

		[iOS (11,0), TV (11,0), Mac (10,13)]
		[Field ("kCIImageNearestSampling")]
		NSString NearestSamplingKey { get; }

		[iOS (11,0), TV (11,0), Mac (10,13)]
		[Field ("kCIImageApplyOrientationProperty")]
		NSString ApplyOrientationPropertyKey { get; }

		[iOS (11,0), TV (11,0), Mac (10,13)]
		[Field ("kCIImageAuxiliaryDepth")]
		NSString AuxiliaryDepthKey { get; }

		[iOS (11,0), TV (11,0), Mac (10,13)]
		[Field ("kCIImageAuxiliaryDisparity")]
		NSString AuxiliaryDisparityKey { get; }

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		[Field ("kCIImageAuxiliaryPortraitEffectsMatte")]
		NSString AuxiliaryPortraitEffectsMatteKey { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Field ("kCIImageAuxiliarySemanticSegmentationSkinMatte")]
		NSString AuxiliarySemanticSegmentationSkinMatteKey { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Field ("kCIImageAuxiliarySemanticSegmentationHairMatte")]
		NSString AuxiliarySemanticSegmentationHairMatteKey { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Field ("kCIImageAuxiliarySemanticSegmentationTeethMatte")]
		NSString AuxiliarySemanticSegmentationTeethMatteKey { get; }
	}
	
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIImage : NSSecureCoding, NSCopying {
		[Static]
		[Export ("imageWithCGImage:")]
		CIImage FromCGImage (CGImage image);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithCGImage:options:")]
		CIImage FromCGImage (CGImage image, [NullAllowed] NSDictionary d);

		[Static]
		[Wrap ("FromCGImage (image, options.GetDictionary ())")]
		CIImage FromCGImage (CGImage image, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithCGImageSource:index:options:")]
		CIImage FromCGImageSource (CGImageSource source, nuint index, [NullAllowed] NSDictionary options);

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Wrap ("FromCGImageSource (source, index, options.GetDictionary ())")]
		CIImage FromCGImageSource (CGImageSource source, nuint index, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

#if MONOMAC
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Static]
		[Export ("imageWithCGLayer:")]
		CIImage FromLayer (CGLayer layer);

		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Static]
		[Export ("imageWithCGLayer:options:")]
		CIImage FromLayer (CGLayer layer, NSDictionary options);
#endif

		[Static]
		[Export ("imageWithBitmapData:bytesPerRow:size:format:colorSpace:")]
		[Internal] // there's a CIFormat enum that maps to the kCIFormatARGB8, kCIFormatRGBA16, kCIFormatRGBAf, kCIFormatRGBAh constants
		CIImage FromData (NSData bitmapData, nint bytesPerRow, CGSize size, int /* CIFormat = int */ pixelFormat, [NullAllowed] CGColorSpace colorSpace);

		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Static]
		[Export ("imageWithTexture:size:flipped:colorSpace:")]
		CIImage ImageWithTexture (uint /* unsigned int */ glTextureName, CGSize size, bool flipped, [NullAllowed] CGColorSpace colorspace);

		[Static]
		[Export ("imageWithContentsOfURL:")]
		CIImage FromUrl (NSUrl url);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithContentsOfURL:options:")]
		CIImage FromUrl (NSUrl url, [NullAllowed] NSDictionary d);

		[Static]
		[Wrap ("FromUrl (url, options.GetDictionary ())")]
		CIImage FromUrl (NSUrl url, [NullAllowed] CIImageInitializationOptions options);

		[Static]
		[Export ("imageWithData:")]
		CIImage FromData (NSData data);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithData:options:")]
		CIImage FromData (NSData data, [NullAllowed] NSDictionary d);

		[Static]
		[Wrap ("FromData (data, options.GetDictionary ())")]
		CIImage FromData (NSData data, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		[Static]
		[iOS(9,0)]
		[Export ("imageWithCVImageBuffer:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer);

#if MONOMAC && !XAMCORE_4_0
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithCVImageBuffer:options:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary dict);
#else
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[iOS(9,0)]
		[Internal] // This overload is needed for our strong dictionary support (but only for Unified, since for Classic the generic version is transformed to this signature)
		[Export ("imageWithCVImageBuffer:options:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary dict);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[iOS(9,0)]
		[Export ("imageWithCVImageBuffer:options:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary<NSString, NSObject> dict);
#endif

		[Static][iOS(9,0)]
		[Wrap ("FromImageBuffer (imageBuffer, options.GetDictionary ())")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, CIImageInitializationOptions options);
		
#if !MONOMAC
		[Static]
		[Export ("imageWithCVPixelBuffer:")]
		CIImage FromImageBuffer (CVPixelBuffer buffer);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithCVPixelBuffer:options:")]
		CIImage FromImageBuffer (CVPixelBuffer buffer, [NullAllowed] NSDictionary dict);

		[Static]
		[Wrap ("FromImageBuffer (buffer, options.GetDictionary ())")]
		CIImage FromImageBuffer (CVPixelBuffer buffer, [NullAllowed] CIImageInitializationOptions options);
#endif
		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Static]
		[Export ("imageWithIOSurface:")]
		CIImage FromSurface (IOSurface.IOSurface surface);
		
		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithIOSurface:options:")]
		CIImage FromSurface (IOSurface.IOSurface surface, NSDictionary options);

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Static]
		[Wrap ("FromSurface (surface, options.GetDictionary ()!)")]
		CIImage FromSurface (IOSurface.IOSurface surface, CIImageInitializationOptions options);

		[Static]
		[Export ("imageWithColor:")]
		CIImage ImageWithColor (CIColor color);

		[Static]
		[Export ("emptyImage")]
		CIImage EmptyImage { get; }
		
		[Export ("initWithCGImage:")]
		IntPtr Constructor (CGImage image);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithCGImage:options:")]
		IntPtr Constructor (CGImage image, [NullAllowed] NSDictionary d);

		[Wrap ("this (image, options.GetDictionary ())")]
		IntPtr Constructor (CGImage image, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithCGImageSource:index:options:")]
		IntPtr Constructor (CGImageSource source, nuint index, [NullAllowed] NSDictionary options);

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Wrap ("this (source, index, options.GetDictionary ())")]
		IntPtr Constructor (CGImageSource source, nuint index, CIImageInitializationOptionsWithMetadata options);

#if MONOMAC
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CIImage (CGImage)' instead.")]
		[Export ("initWithCGLayer:")]
		IntPtr Constructor (CGLayer layer);

		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CIImage (CGImage)' instead.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithCGLayer:options:")]
		IntPtr Constructor (CGLayer layer, [NullAllowed] NSDictionary d);

		[Wrap ("this (layer, options.GetDictionary ())")]
		IntPtr Constructor (CGLayer layer, [NullAllowed] CIImageInitializationOptions options);
#endif

		[Export ("initWithData:")]
		IntPtr Constructor (NSData data);

		[Export ("initWithData:options:")]
		IntPtr Constructor (NSData data, [NullAllowed] NSDictionary d);

		[Wrap ("this (data, options.GetDictionary ())")]
		IntPtr Constructor (NSData data, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		[Export ("initWithBitmapData:bytesPerRow:size:format:colorSpace:")]
		IntPtr Constructor (NSData d, nint bytesPerRow, CGSize size, int /* CIFormat = int */ pixelFormat, [NullAllowed] CGColorSpace colorSpace);

		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Export ("initWithTexture:size:flipped:colorSpace:")]
		IntPtr Constructor (int /* unsigned int */ glTextureName, CGSize size, bool flipped, [NullAllowed] CGColorSpace colorSpace);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl url);

		[Export ("initWithContentsOfURL:options:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] NSDictionary d);

		[Wrap ("this (url, options.GetDictionary ())")]
		IntPtr Constructor (NSUrl url, [NullAllowed] CIImageInitializationOptions options);

		[iOS (11,0)] // IOSurface was not exposed before Xcode 9
		[TV (11,0)]
		[Mac (10,13)]
		[Export ("initWithIOSurface:")]
		IntPtr Constructor (IOSurface.IOSurface surface);
		
		[iOS (11,0)] // IOSurface was not exposed before Xcode 9
		[TV (11,0)]
		[Mac (10,13)]
		[Export ("initWithIOSurface:options:")]
		IntPtr Constructor (IOSurface.IOSurface surface, [NullAllowed] NSDictionary options);

		[iOS (11,0)] // IOSurface was not exposed before Xcode 9
		[TV (11,0)]
		[Mac (10,13)]
		[Wrap ("this (surface, options.GetDictionary ())")]
		IntPtr Constructor (IOSurface.IOSurface surface, [NullAllowed] CIImageInitializationOptions options);

		[iOS(9,0)]
		[Export ("initWithCVImageBuffer:")]
		IntPtr Constructor (CVImageBuffer imageBuffer);

#if MONOMAC && !XAMCORE_4_0
		[Export ("initWithCVImageBuffer:options:")]
		IntPtr Constructor (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary dict);
#else
		[iOS(9,0)]
		[Export ("initWithCVImageBuffer:options:")]
		IntPtr Constructor (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary<NSString, NSObject> dict);

		[iOS(9,0)]
		[Internal] // This overload is needed for our strong dictionary support (but only for Unified, since for Classic the generic version is transformed to this signature)
		[Sealed]
		[Export ("initWithCVImageBuffer:options:")]
		IntPtr Constructor (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary dict);
#endif

		[iOS(9,0)]
		[Wrap ("this (imageBuffer, options.GetDictionary ())")]
		IntPtr Constructor (CVImageBuffer imageBuffer, [NullAllowed] CIImageInitializationOptions options);

		[Mac (10,11)]
		[Export ("initWithCVPixelBuffer:")]
		IntPtr Constructor (CVPixelBuffer buffer);

		[Mac (10,11)]
		[Export ("initWithCVPixelBuffer:options:")]
		IntPtr Constructor (CVPixelBuffer buffer, [NullAllowed] NSDictionary dict);

		[Mac (10,11)]
		[Wrap ("this (buffer, options.GetDictionary ())")]
		IntPtr Constructor (CVPixelBuffer buffer, [NullAllowed] CIImageInitializationOptions options);

		[Export ("initWithColor:")]
		IntPtr Constructor (CIColor color);

		[iOS (9,0)][Mac (10,11)]
		[Export ("initWithMTLTexture:options:")]
		IntPtr Constructor (IMTLTexture texture, [NullAllowed] NSDictionary options);

#if MONOMAC
		[Export ("initWithBitmapImageRep:")]
		IntPtr Constructor (NSImageRep imageRep);
		
		[Export ("drawAtPoint:fromRect:operation:fraction:")]
		void Draw (CGPoint point, CGRect srcRect, NSCompositingOperation op, nfloat delta);

		[Export ("drawInRect:fromRect:operation:fraction:")]
		void Draw (CGRect dstRect, CGRect srcRect, NSCompositingOperation op, nfloat delta);
#endif

		[Export ("imageByApplyingTransform:")]
		CIImage ImageByApplyingTransform (CGAffineTransform matrix);

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Export ("imageByApplyingTransform:highQualityDownsample:")]
		CIImage ImageByApplyingTransform (CGAffineTransform matrix, bool highQualityDownsample);

		[Export ("imageByCroppingToRect:")]
		CIImage ImageByCroppingToRect (CGRect r);

		[Export ("extent")]
		CGRect Extent { get; }

		[Export ("properties"), Internal]
		NSDictionary WeakProperties { get; }

		[Wrap ("WeakProperties")]
		CoreGraphics.CGImageProperties Properties { get; }

#if MONOMAC
		[Export ("definition")]
		CIFilterShape Definition { get; }
#endif
		[iOS (10,0)]
		[Field ("kCIFormatRGBA16")]
		int FormatRGBA16 { get; } /* CIFormat = int */

		[Field ("kCIFormatARGB8")]
		int FormatARGB8 { get; } /* CIFormat = int */
		
		[Field ("kCIFormatRGBAh")]
		int FormatRGBAh { get; } /* CIFormat = int */

		[iOS (8,0)]
		[Field ("kCIFormatRGBAf")]
		int FormatRGBAf { get; } /* CIFormat = int */

		[Field ("kCIFormatBGRA8")]
		int FormatBGRA8 { get; } /* CIFormat = int */

		[Field ("kCIFormatRGBA8")]
		int FormatRGBA8 { get; } /* CIFormat = int */

		[Field ("kCIFormatABGR8")]
		[iOS (9,0)][Mac (10,11)]
		int FormatABGR8 { get; }
		
		[Field ("kCIFormatA8")]
		[iOS (9,0)][Mac (10,11)]
		int FormatA8 { get; }
		
		[Field ("kCIFormatA16")]
		[iOS (9,0)][Mac (10,11)]
		int FormatA16 { get; }
		
		[Field ("kCIFormatAh")]
		[iOS (9,0)][Mac (10,11)]
		int FormatAh { get; }
		
		[Field ("kCIFormatAf")]
		[iOS (9,0)][Mac (10,11)]
		int FormatAf { get; }
		
		[Field ("kCIFormatR8")]
		[iOS (9,0)][Mac (10,11)]
		int FormatR8 { get; }
		
		[Field ("kCIFormatR16")]
		[iOS (9,0)][Mac (10,11)]
		int FormatR16 { get; }
		
		[Field ("kCIFormatRh")]
		[iOS (9,0)][Mac (10,11)]
		int FormatRh { get; }
		
		[Field ("kCIFormatRf")]
		[iOS (9,0)][Mac (10,11)]
		int FormatRf { get; }
		
		[Field ("kCIFormatRG8")]
		[iOS (9,0)][Mac (10,11)]
		int FormatRG8 { get; }
		
		[Field ("kCIFormatRG16")]
		[iOS (9,0)][Mac (10,11)]
		int FormatRG16 { get; }
		
		[Field ("kCIFormatRGh")]
		[iOS (9,0)][Mac (10,11)]
		int FormatRGh { get; }
		
		[Field ("kCIFormatRGf")]
		[iOS (9,0)][Mac (10,11)]
		int FormatRGf { get; }

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		[Field ("kCIFormatL8")]
		int FormatL8 { get; }

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		[Field ("kCIFormatL16")]
		int FormatL16 { get; }

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		[Field ("kCIFormatLh")]
		int FormatLh { get; }

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		[Field ("kCIFormatLf")]
		int FormatLf { get; }

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		[Field ("kCIFormatLA8")]
		int FormatLA8 { get; }

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		[Field ("kCIFormatLA16")]
		int FormatLA16 { get; }

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		[Field ("kCIFormatLAh")]
		int FormatLAh { get; }

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		[Field ("kCIFormatLAf")]
		int FormatLAf { get; }

#if !MONOMAC
		// UIKit extensions
		[Export ("initWithImage:")]
		IntPtr Constructor (UIImage image);

		[Export ("initWithImage:options:")]
		IntPtr Constructor (UIImage image, [NullAllowed] NSDictionary options);

		[Wrap ("this (image, options.GetDictionary ())")]
		IntPtr Constructor (UIImage image, [NullAllowed] CIImageInitializationOptions options);
#endif
	
		[Field ("kCIImageAutoAdjustFeatures"), Internal]
		NSString AutoAdjustFeaturesKey { get; }

		[Field ("kCIImageAutoAdjustRedEye"), Internal]
		NSString AutoAdjustRedEyeKey { get; }

		[Field ("kCIImageAutoAdjustEnhance"), Internal]
		NSString AutoAdjustEnhanceKey { get; }

		[Export ("autoAdjustmentFiltersWithOptions:"), Internal]
		NSArray _GetAutoAdjustmentFilters ([NullAllowed] NSDictionary opts);

		[Mac (10,12)]
		[Export ("regionOfInterestForImage:inRect:")]
		CGRect GetRegionOfInterest (CIImage im, CGRect r);

		//
		// iOS 8.0
		//
		[iOS (8,0), Mac (10,10)]
		[Export ("imageByApplyingOrientation:")]
		CIImage CreateWithOrientation (CIImageOrientation orientation);

		[iOS (8,0), Mac (10,10)]
		[Export ("imageTransformForOrientation:")]
		CGAffineTransform GetImageTransform (CIImageOrientation orientation);

		[iOS (8,0), Mac (10,10)]
		[Export ("imageByClampingToExtent")]
		CIImage CreateByClampingToExtent ();

		[iOS (8,0)]
		[Export ("imageByCompositingOverImage:")]
		CIImage CreateByCompositingOverImage (CIImage dest);

		[iOS (8,0), Mac (10,10)]
		[Export ("imageByApplyingFilter:withInputParameters:")]
		CIImage CreateByFiltering (string filterName, [NullAllowed] NSDictionary inputParameters);

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Export ("imageByApplyingFilter:")]
		CIImage CreateByFiltering (string filterName);

		[iOS (11, 0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Export ("imageBySamplingLinear")]
		CIImage CreateBySamplingLinear ();

		[iOS (11, 0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Export ("imageBySamplingNearest")]
		CIImage CreateBySamplingNearest ();

		[iOS (8,0), Mac (10,10)]
		[Field ("kCIImageAutoAdjustCrop"), Internal]
		NSString AutoAdjustCrop { get; }

		[iOS (8,0), Mac (10,10)]
		[Field ("kCIImageAutoAdjustLevel"), Internal]
		NSString AutoAdjustLevel { get; }

		[iOS (9,0)]
		[NullAllowed, Export ("url")]
		NSUrl Url { get; }

		[iOS (9,0)]
		[NullAllowed, Export ("colorSpace")]
		CGColorSpace ColorSpace { get; }

		[iOS (9,0)]
		[Static, Internal]
		[Export ("imageWithImageProvider:size::format:colorSpace:options:")]
		CIImage FromProvider (ICIImageProvider provider, nuint width, nuint height, int format, [NullAllowed] CGColorSpace colorSpace, [NullAllowed] NSDictionary options);
	
		[iOS (9,0)]
		[Internal]
		[Export ("initWithImageProvider:size::format:colorSpace:options:")]
		IntPtr Constructor (ICIImageProvider provider, nuint width, nuint height, int f, [NullAllowed] CGColorSpace colorSpace, [NullAllowed] NSDictionary options);

		[iOS (9,0)][Mac (10,11)]
		[Static]
		[Export ("imageWithMTLTexture:options:")]
		CIImage FromMetalTexture (IMTLTexture texture, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("imageByClampingToRect:")]
		CIImage CreateByClamping (CGRect rect);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("imageByColorMatchingColorSpaceToWorkingSpace:")]
		[return: NullAllowed]
		CIImage CreateByColorMatchingColorSpaceToWorkingSpace (CGColorSpace colorSpace);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("imageByColorMatchingWorkingSpaceToColorSpace:")]
		[return: NullAllowed]
		CIImage CreateByColorMatchingWorkingSpaceToColorSpace (CGColorSpace colorSpace);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("imageByPremultiplyingAlpha")]
		CIImage CreateByPremultiplyingAlpha ();

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("imageByUnpremultiplyingAlpha")]
		CIImage CreateByUnpremultiplyingAlpha ();

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("imageBySettingAlphaOneInExtent:")]
		CIImage CreateBySettingAlphaOne (CGRect extent);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("imageByApplyingGaussianBlurWithSigma:")]
		CIImage CreateByApplyingGaussianBlur (double sigma);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Export ("imageBySettingProperties:")]
		CIImage CreateBySettingProperties (NSDictionary properties);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[NullAllowed, Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[NullAllowed, Export ("CGImage")]
		CGImage CGImage { get; }

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[NullAllowed, Export ("depthData")]
		AVDepthData DepthData { get; }

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Export ("imageByApplyingCGOrientation:")]
		CIImage CreateByApplyingOrientation (CGImagePropertyOrientation orientation);

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Export ("imageTransformForCGOrientation:")]
		CGAffineTransform GetImageTransform (CGImagePropertyOrientation orientation);

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		[Export ("imageByInsertingIntermediate")]
		CIImage CreateByInsertingIntermediate ();

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		[Export ("imageByInsertingIntermediate:")]
		CIImage CreateByInsertingIntermediate (bool cache);

		// CIImage_AVPortraitEffectsMatte category

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		[NullAllowed, Export ("portraitEffectsMatte")]
		AVPortraitEffectsMatte PortraitEffectsMatte { get; }

		[TV (12,0), iOS (12,0), Mac (10, 14)]
		[Export ("initWithPortaitEffectsMatte:options:")] // selector typo, rdar filled 42894821
		IntPtr Constructor (AVPortraitEffectsMatte matte, [NullAllowed] NSDictionary options);

		[TV (11,0), iOS (11,0), Mac (10, 14)]
		[Export ("initWithPortaitEffectsMatte:")] // selector typo, rdar filled 42894821
		IntPtr Constructor (AVPortraitEffectsMatte matte);

		[TV (12,0), iOS (12,0), Mac (10, 14)]
		[Static]
		[Export ("imageWithPortaitEffectsMatte:options:")] // selector typo, rdar filled 42894821
		[return: NullAllowed]
		CIImage FromPortraitEffectsMatte (AVPortraitEffectsMatte matte, [NullAllowed] NSDictionary options);

		[TV (12,0), iOS (12,0), Mac (10, 14)]
		[Static]
		[Export ("imageWithPortaitEffectsMatte:")] // selector typo, rdar filled 42894821
		[return: NullAllowed]
		CIImage FromPortraitEffectsMatte (AVPortraitEffectsMatte matte);

		// CIImage_AVSemanticSegmentationMatte

		[TV (13,0), iOS (13,0), Mac (10,15)]
		[NullAllowed, Export ("semanticSegmentationMatte")]
		AVSemanticSegmentationMatte SemanticSegmentationMatte { get; }

		[TV (13,0), iOS (13,0), Mac (10,15)]
		[Export ("initWithSemanticSegmentationMatte:options:")]
		IntPtr Constructor (AVSemanticSegmentationMatte matte, [NullAllowed] NSDictionary options);

		[TV (13,0), iOS (13,0), Mac (10,15)]
		[Export ("initWithSemanticSegmentationMatte:")]
		IntPtr Constructor (AVSemanticSegmentationMatte matte);

		[TV (13,0), iOS (13,0), Mac (10,15)]
		[Static]
		[Export ("imageWithSemanticSegmentationMatte:options:")]
		[return: NullAllowed]
		CIImage FromSemanticSegmentationMatte (AVSemanticSegmentationMatte matte, [NullAllowed] NSDictionary options);

		[TV (13,0), iOS (13,0), Mac (10,15)]
		[Static]
		[Export ("imageWithSemanticSegmentationMatte:")]
		[return: NullAllowed]
		CIImage FromSemanticSegmentationMatte (AVSemanticSegmentationMatte matte);

		// CIImage_AVDepthData category

		[TV (11, 0), iOS (11, 0), Mac (10,13)]
		[Export ("initWithDepthData:options:")]
		IntPtr Constructor (AVDepthData data, [NullAllowed] NSDictionary options);

		[TV (11, 0), iOS (11, 0), Mac (10,13)]
		[Export ("initWithDepthData:")]
		IntPtr Constructor (AVDepthData data);

		[TV (11, 0), iOS (11, 0), Mac (10,13)]
		[Static]
		[Export ("imageWithDepthData:options:")]
		[return: NullAllowed]
		CIImage FromDepthData (AVDepthData data, [NullAllowed] NSDictionary options);

		[TV (11, 0), iOS (11, 0), Mac (10,13)]
		[Static]
		[Export ("imageWithDepthData:")]
		[return: NullAllowed]
		CIImage FromDepthData (AVDepthData data);

		// colors

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("blackImage", ArgumentSemantic.Strong)]
		CIImage BlackImage { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("whiteImage", ArgumentSemantic.Strong)]
		CIImage WhiteImage { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("grayImage", ArgumentSemantic.Strong)]
		CIImage GrayImage { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("redImage", ArgumentSemantic.Strong)]
		CIImage RedImage { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("greenImage", ArgumentSemantic.Strong)]
		CIImage GreenImage { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("blueImage", ArgumentSemantic.Strong)]
		CIImage BlueImage { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("cyanImage", ArgumentSemantic.Strong)]
		CIImage CyanImage { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("magentaImage", ArgumentSemantic.Strong)]
		CIImage MagentaImage { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("yellowImage", ArgumentSemantic.Strong)]
		CIImage YellowImage { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Static]
		[Export ("clearImage", ArgumentSemantic.Strong)]
		CIImage ClearImage { get; }
	}

	interface ICIImageProcessorInput {}

	[iOS (10,0)][Mac (10,12)]
	[Protocol]
	interface CIImageProcessorInput {
		[Abstract]
		[Export ("region")]
		CGRect Region { get; }

		[Abstract]
		[Export ("bytesPerRow")]
		nuint BytesPerRow { get; }

		[Abstract]
		[Export ("format")]
		CIFormat Format { get; }

		[Abstract]
		[Export ("baseAddress")]
		IntPtr BaseAddress { get; }

		[Abstract]
		[NullAllowed, Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		[Abstract]
		[NullAllowed, Export ("metalTexture")]
		IMTLTexture MetalTexture { get; }

#if XAMCORE_4_0
		[Abstract] // @required but it was added in Xcode9
#endif
		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Export ("surface")]
		IOSurface.IOSurface Surface { get; }
	}

	interface ICIImageProcessorOutput {}

	[iOS (10,0)][Mac (10,12)]
	[Protocol]
	interface CIImageProcessorOutput {
		[Abstract]
		[Export ("region")]
		CGRect Region { get; }

		[Abstract]
		[Export ("bytesPerRow")]
		nuint BytesPerRow { get; }

		[Abstract]
		[Export ("format")]
		CIFormat Format { get; }

		[Abstract]
		[Export ("baseAddress")]
		IntPtr BaseAddress { get; }

		[Abstract]
		[NullAllowed, Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		[Abstract]
		[NullAllowed, Export ("metalTexture")]
		IMTLTexture MetalTexture { get; }

		[Abstract]
		[NullAllowed, Export ("metalCommandBuffer")]
		IMTLCommandBuffer MetalCommandBuffer { get; }

#if XAMCORE_4_0
		[Abstract] // @required but it was added in Xcode9
#endif
		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Export ("surface")]
		IOSurface.IOSurface Surface { get; }
	}

	[iOS (9,0)]
	[StrongDictionary ("CIImageProviderKeys")]
	interface CIImageProviderOptions {
		NSObject TileSize { get;set; }
		NSObject UserInfo { get; set; }
	}

	[Internal]
	[Static]
	[iOS (9,0)]
	interface CIImageProviderKeys {
		[Field ("kCIImageProviderTileSize")]
		NSString TileSizeKey { get; }

		[Field ("kCIImageProviderUserInfo")]
		NSString UserInfoKey { get; } 
	}
	
	interface ICIImageProvider {}

	// Informal protocol
	[Protocol (IsInformal = true)]
	interface CIImageProvider {
		[Abstract]
		[Export ("provideImageData:bytesPerRow:origin::size::userInfo:")]
		unsafe void ProvideImageData (IntPtr data, nuint rowbytes, nuint x, nuint y, nuint width, nuint height, [NullAllowed] NSObject info);
	}
	
	delegate CGRect CIKernelRoiCallback (int /* int, not NSInteger */ index, CGRect rect);

	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // avoid crashes
	interface CIKernel {
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Static, Export ("kernelsWithString:")]
		CIKernel [] FromProgramMultiple (string coreImageShaderProgram);

		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Static, Export ("kernelWithString:")]
		CIKernel FromProgramSingle (string coreImageShaderProgram);

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Static]
		[Export ("kernelWithFunctionName:fromMetalLibraryData:error:")]
		[return: NullAllowed]
		CIKernel FromFunction (string name, NSData data, [NullAllowed] out NSError error);

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Static]
		[Export ("kernelWithFunctionName:fromMetalLibraryData:outputPixelFormat:error:")]
		[return: NullAllowed]
		CIKernel FromFunction (string name, NSData data, CIFormat format, [NullAllowed] out NSError error);

		[Export ("name")]
		string Name { get; }

#if MONOMAC
		[Export ("setROISelector:")]
		void SetRegionOfInterestSelector (Selector aMethod);
#endif
		[iOS (8,0), Mac (10,11)]
		[Export ("applyWithExtent:roiCallback:arguments:")]
		CIImage ApplyWithExtent (CGRect extent, CIKernelRoiCallback callback, [NullAllowed] NSObject [] args);
	}

	[iOS (8,0), Mac(10,11)]
	[BaseType (typeof (CIKernel))]
	[DisableDefaultCtor] // returns a nil handle -> instances of this type are returned from `kernel[s]WithString:`
	interface CIColorKernel {
		[Export ("applyWithExtent:arguments:")]
		CIImage ApplyWithExtent (CGRect extent, [NullAllowed] NSObject [] args);

		// Note: the API is supported in iOS 8, but with iOS 9, they guarantee
		// a more derived result
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[New, Static, Export ("kernelWithString:")]
		CIColorKernel FromProgramSingle (string coreImageShaderProgram);
	}

	[iOS (8,0)][Mac (10,11)]
	[BaseType (typeof (CIKernel))]
	[DisableDefaultCtor] // returns a nil handle -> instances of this type are returned from `kernel[s]WithString:`
	interface CIWarpKernel {
		[Export ("applyWithExtent:roiCallback:inputImage:arguments:")]
		CIImage ApplyWithExtent (CGRect extent, CIKernelRoiCallback callback, CIImage image, [NullAllowed] NSObject [] args);

		// Note: the API is supported in iOS 8, but with iOS 9, they guarantee
		// a more derived result
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[New, Static, Export ("kernelWithString:")]
		CIWarpKernel FromProgramSingle (string coreImageShaderProgram);
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // does not work in iOS 11 beta 4
	interface CIImageAccumulator {
#if !XAMCORE_4_0
		[Obsolete ("The default initializer does not work in recent iOS version (11b4).")]
		[Export ("init")]
		IntPtr Constructor ();
#endif

		[Static]
		[Export ("imageAccumulatorWithExtent:format:")]
		CIImageAccumulator FromRectangle (CGRect rect, CIFormat format);

#if MONOMAC && !XAMCORE_4_0
		[Obsolete ("Use the overload acceping a 'CIFormat' enum instead of an 'int'.")]
		[Static]
		[Wrap ("FromRectangle (rect, (CIFormat) ciImageFormat)")]
		CIImageAccumulator FromRectangle (CGRect rect, int ciImageFormat);
#endif

		[iOS (9,0)]
		[Static]
		[Export ("imageAccumulatorWithExtent:format:colorSpace:")]
		CIImageAccumulator FromRectangle (CGRect extent, CIFormat format, CGColorSpace colorSpace);
		
		[Export ("initWithExtent:format:")]
		IntPtr Constructor (CGRect rectangle, CIFormat format);

#if MONOMAC && !XAMCORE_4_0
		[Obsolete ("Use the overload acceping a 'CIFormat' enum instead of an 'int'.")]
		[Wrap ("this (rectangle, (CIFormat) ciImageFormat)")]
		IntPtr Constructor (CGRect rectangle, int ciImageFormat);
#endif

		[Export ("initWithExtent:format:colorSpace:")]
		IntPtr Constructor (CGRect extent, CIFormat format, CGColorSpace colorSpace);
		
		[Export ("extent")]
		CGRect Extent { get; }

		[Export ("format")]
		int CIImageFormat { get; } /* CIFormat = int */

		[Export ("setImage:dirtyRect:")]
		void SetImageDirty (CIImage image, CGRect dirtyRect);

		[Export ("clear")]
		void Clear ();

		//Detected properties
		[Export ("image")]
		CIImage Image { get; set; }
	}

#if MONOMAC
	[BaseType (typeof (NSObject))]
	interface CIPlugIn {
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'LoadNonExecutablePlugIns' for non-executable plugins instead.")]
		[Static]
		[Export ("loadAllPlugIns")]
		void LoadAllPlugIns ();

		[Static]
		[Export ("loadNonExecutablePlugIns")]
		void LoadNonExecutablePlugIns ();

		[Mac (10,15)]
		[Static]
		[Export ("loadNonExecutablePlugIn:")]
		void LoadNonExecutablePlugIn (NSUrl url);

		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Static]
		[Export ("loadPlugIn:allowNonExecutable:")]
		void LoadPlugIn (NSUrl pluginUrl, bool allowNonExecutable);
	}
#endif

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CISampler : NSCopying {
		[Static, Export ("samplerWithImage:")]
		CISampler FromImage (CIImage sourceImage);

		[Internal, Static]
		[Export ("samplerWithImage:options:")]
		CISampler FromImage (CIImage sourceImag, NSDictionary options);

		[Export ("initWithImage:")]
		IntPtr Constructor (CIImage sourceImage);

		[DesignatedInitializer]
		[Internal, Export ("initWithImage:options:")]
		NSObject Constructor (CIImage image, NSDictionary options);

		[Export ("definition")]
		CIFilterShape Definition { get; }

		[Export ("extent")]
		CGRect Extent { get; }

		[Field ("kCISamplerAffineMatrix", "+CoreImage"), Internal]
		NSString AffineMatrix { get; }

		[Field ("kCISamplerWrapMode", "+CoreImage"), Internal]
		NSString WrapMode { get; }

		[Field ("kCISamplerFilterMode", "+CoreImage"), Internal]
		NSString FilterMode { get; }

		[Field ("kCISamplerWrapBlack", "+CoreImage"), Internal]
		NSString WrapBlack { get; }

		[Field ("kCISamplerWrapClamp", "+CoreImage"), Internal]
		NSString WrapClamp { get; }
		
		[Field ("kCISamplerFilterNearest", "+CoreImage"), Internal]
		NSString FilterNearest { get; }

		[Field ("kCISamplerFilterLinear", "+CoreImage"), Internal]
		NSString FilterLinear { get; }

		[iOS (9,0)]
		[Field ("kCISamplerColorSpace", "+CoreImage"), Internal]
		NSString ColorSpace { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIVector : NSSecureCoding, NSCopying {
		[Static, Internal, Export ("vectorWithValues:count:")]
		CIVector _FromValues (IntPtr values, nint count);

		[Static]
		[Export ("vectorWithX:")]
		CIVector Create (nfloat x);

		[Static]
		[Export ("vectorWithX:Y:")]
		CIVector Create (nfloat x, nfloat y);

		[Static]
		[Export ("vectorWithX:Y:Z:")]
		CIVector Create (nfloat x, nfloat y, nfloat z);

		[Static]
		[Export ("vectorWithX:Y:Z:W:")]
		CIVector Create (nfloat x, nfloat y, nfloat z, nfloat w);

#if !MONOMAC
		[Static]
		[Export ("vectorWithCGPoint:")]
		CIVector Create (CGPoint point);

		[Static]
		[Export ("vectorWithCGRect:")]
		CIVector Create (CGRect point);

		[Static]
		[Export ("vectorWithCGAffineTransform:")]
		CIVector Create (CGAffineTransform affineTransform);
#endif

		[Static]
		[Export ("vectorWithString:")]
		CIVector FromString (string representation);

		[Mac (10,9)]
		[Export ("initWithCGPoint:")]
		IntPtr Constructor (CGPoint p);

		[Mac (10,9)]
		[Export ("initWithCGRect:")]
		IntPtr Constructor (CGRect r);

		[Mac (10,9)]
		[Export ("initWithCGAffineTransform:")]
		IntPtr Constructor (CGAffineTransform r);
		
		
		[Export ("initWithX:")]
		IntPtr Constructor(nfloat x);

		[Export ("initWithX:Y:")]
		IntPtr Constructor (nfloat x, nfloat y);

		[Export ("initWithX:Y:Z:")]
		IntPtr Constructor (nfloat x, nfloat y, nfloat z);

		[Export ("initWithX:Y:Z:W:")]
		IntPtr Constructor (nfloat x, nfloat y, nfloat z, nfloat w);

		[Export ("initWithString:")]
		IntPtr Constructor (string representation);

		[Export ("valueAtIndex:"), Internal]
		nfloat ValueAtIndex (nint index);

		[Export ("count")]
		nint Count { get; }

		[Export ("X")]
		nfloat X { get; }

		[Export ("Y")]
		nfloat Y { get; }

		[Export ("Z")]
		nfloat Z { get; }

		[Export ("W")]
		nfloat W { get; }

		[Mac (10,9)]
		[Export ("CGPointValue")]
		CGPoint Point { get; }

		[Mac (10,9)]
		[Export ("CGRectValue")]
		CGRect Rectangle { get; }

		[Mac (10,9)]
		[Export ("CGAffineTransformValue")]
		CGAffineTransform AffineTransform { get; }

		[Export ("stringRepresentation"), Internal]
		string StringRepresentation ();

	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIDetector {
		[Static, Export ("detectorOfType:context:options:"), Internal]
		CIDetector FromType ([NullAllowed] NSString detectorType, [NullAllowed] CIContext context, [NullAllowed] NSDictionary options);

		[Export ("featuresInImage:")]
		CIFeature [] FeaturesInImage (CIImage image);

		[Export ("featuresInImage:options:")]
		CIFeature [] FeaturesInImage (CIImage image, [NullAllowed] NSDictionary options);

		[Field ("CIDetectorTypeFace"), Internal]
		NSString TypeFace { get; }

		[Field ("CIDetectorImageOrientation"), Internal]
		NSString ImageOrientation { get; }

		[Field ("CIDetectorAccuracy"), Internal]
		NSString Accuracy { get; }

		[Field ("CIDetectorAccuracyLow"), Internal]
		NSString AccuracyLow { get; }

		[Field ("CIDetectorAccuracyHigh"), Internal]
		NSString AccuracyHigh { get; }

		[Field ("CIDetectorTracking"), Internal]
		NSString Tracking { get; }

		[Field ("CIDetectorMinFeatureSize"), Internal]
		NSString MinFeatureSize { get; }

		[iOS (10,0)]
		[Mac (10,12)]
		[TV (10,0)]
		[Field ("CIDetectorMaxFeatureCount"), Internal]
		NSString MaxFeatureCount { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("CIDetectorEyeBlink"), Internal]
		NSString EyeBlink { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("CIDetectorSmile"), Internal]
		NSString Smile { get; }

		[iOS (8,0), Mac (10,10)]
		[Field ("CIDetectorAspectRatio")]
		NSString AspectRatio { get; }

		[iOS (8,0), Mac (10,10)]
		[Field ("CIDetectorFocalLength")]
		NSString FocalLength { get; }

		[iOS (8,0), Mac (10,10)]
		[Field ("CIDetectorTypeQRCode")]
		NSString TypeQRCode { get; }

		[iOS (8,0), Mac (10,10)]
		[Field ("CIDetectorTypeRectangle")]
		NSString TypeRectangle { get; }

		[iOS (9,0), Mac (10,11)]
		[Field ("CIDetectorNumberOfAngles")]
		NSString NumberOfAngles { get; }

		[iOS (9,0), Mac(10, 11)]
		[Field ("CIDetectorReturnSubFeatures")]
		NSString ReturnSubFeatures { get; }
		
		[iOS (9,0), Mac(10, 11)]
		[Field ("CIDetectorTypeText")]
		NSString TypeText { get; }
	}
	
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIFeature {
		[Export ("type", ArgumentSemantic.Retain)]
		NSString Type { get; }

		[Export ("bounds", ArgumentSemantic.Assign)]
		CGRect Bounds { get; }

		[Field ("CIFeatureTypeFace")]
		NSString TypeFace { get; }

		[iOS (9,0)][Mac (10,10)]
		[Field ("CIFeatureTypeRectangle")]
		NSString TypeRectangle { get; }

		[iOS (9,0)][Mac (10,11)]
		[Field ("CIFeatureTypeQRCode")]
		NSString TypeQRCode { get; }
		
		[iOS (9,0)][Mac (10,11)]
		[Field ("CIFeatureTypeText")]
		NSString TypeText { get; }
	}

	[BaseType (typeof (CIFeature))]
	[DisableDefaultCtor]
	interface CIFaceFeature {
		[Export ("hasLeftEyePosition", ArgumentSemantic.Assign)]
		bool HasLeftEyePosition { get; }
		
		[Export ("leftEyePosition", ArgumentSemantic.Assign)]
		CGPoint LeftEyePosition { get; }
		
		[Export ("hasRightEyePosition", ArgumentSemantic.Assign)]
		bool HasRightEyePosition { get; }
		
		[Export ("rightEyePosition", ArgumentSemantic.Assign)]
		CGPoint RightEyePosition { get; }
		
		[Export ("hasMouthPosition", ArgumentSemantic.Assign)]
		bool HasMouthPosition { get; }
		
		[Export ("mouthPosition", ArgumentSemantic.Assign)]
		CGPoint MouthPosition { get; }

		[Export ("hasTrackingID", ArgumentSemantic.Assign)]
		bool HasTrackingId { get; }
		
		[Export ("trackingID", ArgumentSemantic.Assign)]
		int TrackingId { get; } /* int, not NSInteger */
		
		[Export ("hasTrackingFrameCount", ArgumentSemantic.Assign)]
		bool HasTrackingFrameCount { get; }

		[Export ("trackingFrameCount", ArgumentSemantic.Assign)]
		int TrackingFrameCount { get; } /* int, not NSInteger */

		[iOS (7,0), Mac (10, 9)]
		[Export ("bounds", ArgumentSemantic.Assign)]
		CGRect Bounds { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("faceAngle", ArgumentSemantic.Assign)]
		float FaceAngle { get; } /* float, not CGFloat */

		[iOS (7,0), Mac (10, 9)]
		[Export ("hasFaceAngle", ArgumentSemantic.Assign)]
		bool HasFaceAngle { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("hasSmile", ArgumentSemantic.Assign)]
		bool HasSmile { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("leftEyeClosed", ArgumentSemantic.Assign)]
		bool LeftEyeClosed { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("rightEyeClosed", ArgumentSemantic.Assign)]
		bool RightEyeClosed { get; }
	}

	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (CIFeature))]
	interface CIRectangleFeature {
		[Export ("bounds", ArgumentSemantic.UnsafeUnretained)]
		CGRect Bounds { get; }

		[Export ("topLeft", ArgumentSemantic.UnsafeUnretained)]
		CGPoint TopLeft { get; }

		[Export ("topRight", ArgumentSemantic.UnsafeUnretained)]
		CGPoint TopRight { get; }

		[Export ("bottomLeft", ArgumentSemantic.UnsafeUnretained)]
		CGPoint BottomLeft { get; }

		[Export ("bottomRight", ArgumentSemantic.UnsafeUnretained)]
		CGPoint BottomRight { get; }
	}

	[iOS (8,0)]
	[Mac (10,12)]
	[BaseType (typeof (CIFeature))]
	partial interface CIQRCodeFeature : NSSecureCoding, NSCopying {

		[Export ("bounds", ArgumentSemantic.Assign)]
		CGRect Bounds { get; }

		[Export ("topLeft", ArgumentSemantic.Assign)]
		CGPoint TopLeft { get; }

		[Export ("topRight", ArgumentSemantic.Assign)]
		CGPoint TopRight { get; }

		[Export ("bottomLeft", ArgumentSemantic.Assign)]
		CGPoint BottomLeft { get; }

		[Export ("bottomRight", ArgumentSemantic.Assign)]
		CGPoint BottomRight { get; }

		[Export ("messageString")]
		string MessageString { get; }

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[NullAllowed, Export ("symbolDescriptor")]
		CIQRCodeDescriptor SymbolDescriptor { get; }
	}

	[iOS (9,0)]
	[Mac (10,12)]
	[BaseType (typeof (CIFeature))]
	interface CITextFeature {
		[Export ("bounds")]
		CGRect Bounds { get; }

		[Export ("topLeft")]
		CGPoint TopLeft { get; }

		[Export ("topRight")]
		CGPoint TopRight { get; }

		[Export ("bottomLeft")]
		CGPoint BottomLeft { get; }

		[Export ("bottomRight")]
		CGPoint BottomRight { get; }

		[Export ("subFeatures")]
		CIFeature[] SubFeatures { get; }
	}

	[iOS (10,0)][Mac (10,12)]
	[TV (10,0)]
	[BaseType (typeof (NSObject))]
	interface CIImageProcessorKernel {
		[Static]
		[Export ("processWithInputs:arguments:output:error:")]
		bool Process ([NullAllowed] ICIImageProcessorInput[] inputs, [NullAllowed] NSDictionary<NSString, NSObject> arguments, ICIImageProcessorOutput output, out NSError error);

		[Static]
		[Export ("roiForInput:arguments:outputRect:")]
		CGRect GetRegionOfInterest (int input, [NullAllowed] NSDictionary<NSString, NSObject> arguments, CGRect outputRect);

		[Static]
		[Export ("formatForInputAtIndex:")]
		CIFormat GetFormat (int input);

		[Static]
		[Export ("outputFormat")]
		CIFormat OutputFormat { get; }

		[Static]
		[Export ("synchronizeInputs")]
		bool SynchronizeInputs { get; }

		[Static]
		[Export ("applyWithExtent:inputs:arguments:error:")]
		[return: NullAllowed]
		CIImage Apply (CGRect extent, [NullAllowed] CIImage[] inputs, [NullAllowed] NSDictionary<NSString, NSObject> args, out NSError error);

		[iOS (11,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[Static]
		[Export ("outputIsOpaque")]
		bool OutputIsOpaque { get; }
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[Mac (10,10)]
	[BaseType (typeof (CIFilter))]
	interface CIAccordionFoldTransition : CIAccordionFoldTransitionProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'FoldCount' instead.")]
		[CoreImageFilterProperty ("inputNumberOfFolds")]
		int NumberOfFolds { get; set; }
#endif
	}

	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CICompositingFilter {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBackgroundImage")]
		CIImage BackgroundImage { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CIAdditionCompositing {
	}

	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIAffineFilter : CIFilterProtocol {

#if !XAMCORE_4_0
		[NoMac]
		[Obsolete ("Not every subclass expose this property.")]
		[CoreImageFilterProperty ("inputTransform")]
		CGAffineTransform Transform { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIAffineFilter))]
	interface CIAffineClamp : CIAffineClampProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIAffineFilter))]
	interface CIAffineTile : CIAffineTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIAffineFilter))]
	interface CIAffineTransform {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputTransform")]
		CGAffineTransform Transform { get; set; }
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaAverage {

		[CoreImageFilterProperty ("outputImageNonMPS")]
		CIImage OutputImageNonMps { get; }

		[iOS (13,4)]
		[CoreImageFilterProperty ("outputImageMPS")]
		CIImage OutputImageMps { get; }
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[BaseType (typeof (CIFilter))]
	interface CIAreaHistogram {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputCount")]
		float Count { get; set; }

		[CoreImageFilterProperty ("inputScale")]
		float Scale { get; set; }

		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }

		[CoreImageFilterProperty ("outputImageNonMPS")]
		CIImage OutputImageNonMps { get; }

#if MONOMAC
		[CoreImageFilterProperty ("outputImageMPS")]
		CIImage OutputImageMps { get; }
#endif

		[CoreImageFilterProperty ("outputData")]
		NSData OutputData { get; }
	}

	[CoreImageFilter]
	[Abstract]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIReductionFilter {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[iOS (9,0)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMaximum {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMaximumAlpha {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMinimum {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMinimumAlpha {
	}

	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CICodeGenerator {
		[CoreImageFilterProperty ("inputMessage")]
		NSData Message { get; set; }
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[Mac (10,10)]
	[BaseType (typeof (CICodeGenerator))]
	interface CIAztecCodeGenerator : CIAztecCodeGeneratorProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCompactStyle' instead.")]
		[CoreImageFilterProperty ("inputCompactStyle")]
		bool CompactStyle { get; set; }

		[Obsolete ("Use 'InputLayers' instead.")]
		[CoreImageFilterProperty ("inputLayers")]
		int Layers { get; set; }
#endif

		[CoreImageFilterProperty ("outputCGImage")]
		CGImage OutputCGImage { get; }
	}

	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CITransitionFilter : CITransitionFilterProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIBarsSwipeTransition {

		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }

		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

		[CoreImageFilterProperty ("inputBarOffset")]
		float BarOffset { get; set; }
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIBlendWithMask))]
	interface CIBlendWithAlphaMask {
	}

	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIBlendFilter))]
	interface CIBlendWithMask : CIBlendWithMaskProtocol {

#if !XAMCORE_4_0
		// renamed for API compatibility
		[Obsolete ("Use 'MaskImage' instead.")]
		[CoreImageFilterProperty ("inputMaskImage")]
		CIImage Mask { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIBloom : CIBloomProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIBoxBlur : CIBoxBlurProtocol {
	}

	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIDistortionFilter {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }

		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter ))]
	interface CIBumpDistortion {

		[CoreImageFilterProperty ("inputScale")]
		float Scale { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIBumpDistortionLinear {

		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

		[CoreImageFilterProperty ("inputScale")]
		float Scale { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CICheckerboardGenerator : CICheckerboardGeneratorProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CICircleSplashDistortion {
	}

	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIScreenFilter {

		[CoreImageFilterProperty ("inputSharpness")]
		float Sharpness { get; set; }

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif

		[CoreImageFilterProperty ("inputCenter")]
		CGPoint InputCenter { get; set; }

		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIScreenFilter))]
	interface CICircularScreen : CICircularScreenProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CICircularWrap {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }

		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter), Name="CICMYKHalftone")]
	interface CICmykHalftone : CICmykHalftoneProtocol {

#if !XAMCORE_4_0
		// renamed for API compatibility
		[Obsolete ("Use 'Sharpness' instead.")]
		[CoreImageFilterProperty ("inputSharpness")]
		float InputSharpness { get; set; }

		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[Mac (10,10)]
	[BaseType (typeof (CICodeGenerator))]
	interface CICode128BarcodeGenerator : CICode128BarcodeGeneratorProtocol {

		[CoreImageFilterProperty ("outputCGImage")]
		CIImage OutputCGImage { get; }
	}

	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIBlendFilter {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBackgroundImage")]
		CIImage BackgroundImage { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIColorBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIColorBurnBlendMode {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIFilter))]
	interface CIColorClamp : CIColorClampProtocol {

#if !XAMCORE_4_0
		// here the prefix was not removed, edited to keep API compatibility
		[Obsolete ("Use 'MinComponents' instead.")]
		[CoreImageFilterProperty ("inputMinComponents")]
		CIVector InputMinComponents { get; set; }

		// here the prefix was not removed, edited to keep API compatibility
		[Obsolete ("Use 'MaxComponents' instead.")]
		[CoreImageFilterProperty ("inputMaxComponents")]
		CIVector InputMaxComponents { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorControls : CIColorControlsProtocol {
	}

	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[iOS (7,0)]		// not part of the attributes dictionary -> [NoiOS] is generated
	[Mac (10,9)]	// not part of the attributes dictionary -> [NoMac] is generated
	[BaseType (typeof (CIFilter))]
	interface CIColorCrossPolynomial : CIColorCrossPolynomialProtocol {
	}

	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIFilter))]
	interface CIColorCube : CIColorCubeProtocol {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIColorCube))]
	interface CIColorCubeWithColorSpace : CIColorCubeWithColorSpaceProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIColorDodgeBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorInvert : CIColorInvertProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorMap : CIColorMapProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorMatrix : CIColorMatrixProtocol  {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorMonochrome : CIColorMonochromeProtocol {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIColorCrossPolynomial))]
	interface CIColorPolynomial : CIColorPolynomialProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorPosterize : CIColorPosterizeProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIColumnAverage {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIComicEffect : CIComicEffectProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIConstantColorGenerator {

		[CoreImageFilterProperty ("inputColor")]
		CIColor Color { get; set; }
	}

	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionCore {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution3X3 {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution5X5 {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution7X7 {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution9Horizontal {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution9Vertical {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CICopyMachineTransition {

		[CoreImageFilterProperty ("inputColor")]
		CIColor Color { get; set; }

		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }

		[CoreImageFilterProperty ("inputOpacity")]
		float Opacity { get; set; }

		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CICrop {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputRectangle")]
		CIVector Rectangle { get; set; }
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CICrystallize : CICrystallizeProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIDarkenBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIDifferenceBlendMode {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIDiscBlur : CIDiscBlurProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIDisintegrateWithMaskTransition : CIDisintegrateWithMaskTransitionProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'MaskImage' instead.")]
		[CoreImageFilterProperty ("inputMaskImage")]
		CIImage Mask { get; set; }

		[Obsolete ("Use 'InputShadowOffset' instead.")]
		[CoreImageFilterProperty ("inputShadowOffset")]
		CIVector ShadowOffset { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIDisplacementDistortion {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputDisplacementImage")]
		CIImage DisplacementImage { get; set; }

		[CoreImageFilterProperty ("inputScale")]
		float Scale { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIDissolveTransition {
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[Mac (10,10)]
	[BaseType (typeof (CIBlendFilter))]
	interface CIDivideBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIScreenFilter))]
	interface CIDotScreen : CIDotScreenProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIDroste {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputStrands")]
		float Strands { get; set; }

		[CoreImageFilterProperty ("inputInsetPoint0")]
		CIVector InsetPoint0 { get; set; }

		[CoreImageFilterProperty ("inputRotation")]
		float Rotation { get; set; }

		[CoreImageFilterProperty ("inputInsetPoint1")]
		CIVector InsetPoint1 { get; set; }

		[CoreImageFilterProperty ("inputZoom")]
		float Zoom { get; set; }

		[CoreImageFilterProperty ("inputPeriodicity")]
		float Periodicity { get; set; }
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIEdges : CIEdgesProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIEdgeWork : CIEdgeWorkProtocol {
	}

	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CITileFilter {

		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif

		[CoreImageFilterProperty ("inputCenter")]
		CGPoint InputCenter { get; set; }

		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CIEightfoldReflectedTile : CIEightfoldReflectedTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIExclusionBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIExposureAdjust : CIExposureAdjustProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIFalseColor : CIFalseColorProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIFlashTransition : CIFlashTransitionProtocol {

#if !XAMCORE_4_0
		// for some reason we prefixed all Striation* with Max - API compatibility
		[Obsolete ("Use 'StriationContrast' instead.")]
		[CoreImageFilterProperty ("inputStriationContrast")]
		float MaxStriationContrast { get; set; }

		[Obsolete ("Use 'InputExtent' instead.")]
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }

		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }

		// for some reason we prefixed all Striation* with Max - API compatibility
		[Obsolete ("Use 'StriationStrength' instead.")]
		[CoreImageFilterProperty ("inputStriationStrength")]
		float MaxStriationStrength { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CIFourfoldReflectedTile : CIFourfoldReflectedTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CIFourfoldRotatedTile : CIFourfoldRotatedTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CIFourfoldTranslatedTile : CIFourfoldTranslatedTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIGammaAdjust : CIGammaAdjustProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIGaussianBlur : CIGaussianBlurProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIGaussianGradient : CIGaussianGradientProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[BaseType (typeof (CIFilter))]
	interface CIGlassDistortion {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }

		[CoreImageFilterProperty ("inputScale")]
		float Scale { get; set; }

		[CoreImageFilterProperty ("inputTexture")]
		CIImage Texture { get; set; }
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIGlassLozenge {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputPoint1")]
		CIVector Point1 { get; set; }

		[CoreImageFilterProperty ("inputPoint0")]
		CIVector Point0 { get; set; }

		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }

		[CoreImageFilterProperty ("inputRefraction")]
		float Refraction { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CIGlideReflectedTile : CIGlideReflectedTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIGloom : CIGloomProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIHardLightBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIScreenFilter))]
	interface CIHatchedScreen : CIHatchedScreenProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIHeightFieldFromMask : CIHeightFieldFromMaskProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIHexagonalPixellate : CIHexagonalPixellateProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIHighlightShadowAdjust : CIHighlightShadowAdjustProtocol {
	}

	[CoreImageFilter]
	[iOS (8,0)]
	// incorrect version string for OSX: '10.?' Double-check documentation
	[BaseType (typeof (CIFilter))]
	interface CIHistogramDisplayFilter {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputHeight")]
		float Height { get; set; }

		[CoreImageFilterProperty ("inputHighLimit")]
		float HighLimit { get; set; }

		[CoreImageFilterProperty ("inputLowLimit")]
		float LowLimit { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIHoleDistortion {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIHueAdjust : CIHueAdjustProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIHueBlendMode {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIKaleidoscope : CIKaleidoscopeProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCount' instead.")]
		[CoreImageFilterProperty ("inputCount")]
		float Count { get; set; }

		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CILanczosScaleTransform : CILanczosScaleTransformProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CILenticularHaloGenerator : CILenticularHaloGeneratorProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CILightenBlendMode {
	}

	[CoreImageFilter]
	[Mac (10,11)]
	[BaseType (typeof (CIFilter))]
	interface CILightTunnel {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputRotation")]
		float Rotation { get; set; }

		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }

		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[Mac (10,10)]
	[BaseType (typeof (CIBlendFilter))]
	interface CILinearBurnBlendMode {
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[Mac (10,10)]
	[BaseType (typeof (CIBlendFilter))]
	interface CILinearDodgeBlendMode {
	}

	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIFilter))]
	interface CILinearGradient : CILinearGradientProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputPoint1' instead.")]
		[CoreImageFilterProperty ("inputPoint1")]
		CIVector Point1 { get; set; }

		[Obsolete ("Use 'InputPoint0' instead.")]
		[CoreImageFilterProperty ("inputPoint0")]
		CIVector Point0 { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,10)]
	[BaseType (typeof (CIFilter))]
	interface CILinearToSRGBToneCurve : CILinearToSrgbToneCurveProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CILineOverlay : CILineOverlayProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIScreenFilter))]
	interface CILineScreen : CILineScreenProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CILuminosityBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIMaskToAlpha : CIMaskToAlphaProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIMaximumComponent : CIMaximumComponentProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CIMaximumCompositing {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIMedianFilter : CIMedianProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIMinimumComponent : CIMinimumComponentProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CIMinimumCompositing {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIModTransition : CIModTransitionProtocol {

#if !XAMCORE_4_0
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (8,3)]
	[BaseType (typeof (CILinearBlur))]
	interface CIMotionBlur : CIMotionBlurProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIMultiplyBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CIMultiplyCompositing {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CINoiseReduction : CINoiseReductionProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CITileFilter))]
	interface CIOpTile : CIOpTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIOverlayBlendMode {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CITransitionFilter))]
	interface CIPageCurlTransition : CIPageCurlTransitionProtocol {

#if !XAMCORE_4_0
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIPageCurlWithShadowTransition : CIPageCurlWithShadowTransitionProtocol {

#if !XAMCORE_4_0
		// prefixed for API compatibility
		[Obsolete ("Use 'Time' instead.")]
		[CoreImageFilterProperty ("inputTime")]
		float InputTime { get; set; }

		[Obsolete ("Use 'InputShadowExtent' instead.")]
		[CoreImageFilterProperty ("inputShadowExtent")]
		CIVector ShadowExtent { get; set; }

		[Obsolete ("Use 'InputExtent' instead.")]
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CITileFilter))]
	interface CIParallelogramTile : CIParallelogramTileProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[Mac (10,11)]
	[BaseType (typeof (CICodeGenerator), Name="CIPDF417BarcodeGenerator")]
	interface CIPdf417BarcodeGenerator : CIPdf417BarcodeGeneratorProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCorrectionLevel' instead.")]
		[CoreImageFilterProperty ("inputCorrectionLevel")]
		int CorrectionLevel { get; set; }

		[Obsolete ("Use 'InputAlwaysSpecifyCompaction' instead.")]
		[CoreImageFilterProperty ("inputAlwaysSpecifyCompaction")]
		bool AlwaysSpecifyCompaction { get; set; }

		[Obsolete ("Use 'InputCompactStyle' instead.")]
		[CoreImageFilterProperty ("inputCompactStyle")]
		bool CompactStyle { get; set; }

		[Obsolete ("Use 'InputCompactStyle' instead.")]
		[CoreImageFilterProperty ("inputDataColumns")]
		int DataColumns { get; set; }

		[Obsolete ("Use 'InputCompactionMode' instead.")]
		[CoreImageFilterProperty ("inputCompactionMode")]
		int CompactionMode { get; set; }

		[Obsolete ("Use 'InputRows' instead.")]
		[CoreImageFilterProperty ("inputRows")]
		int Rows { get; set; }
#endif

		[CoreImageFilterProperty ("outputCGImage")]
		CGImage OutputCGImage { get; }
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[Mac (10,10)]
	[BaseType (typeof (CIPerspectiveTransform))]
	interface CIPerspectiveCorrection : CIPerspectiveCorrectionProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIPerspectiveTile : CIPerspectiveTileProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputBottomLeft' instead.")]
		[CoreImageFilterProperty ("inputBottomLeft")]
		CIVector BottomLeft { get; set; }

		[Obsolete ("Use 'InputTopRight' instead.")]
		[CoreImageFilterProperty ("inputTopRight")]
		CIVector TopRight { get; set; }

		[Obsolete ("Use 'InputTopLeft' instead.")]
		[CoreImageFilterProperty ("inputTopLeft")]
		CIVector TopLeft { get; set; }

		[Obsolete ("Use 'InputBottomRight' instead.")]
		[CoreImageFilterProperty ("inputBottomRight")]
		CIVector BottomRight { get; set; }
#endif
	}

	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIFilter))]
	interface CIPerspectiveTransform : CIPerspectiveTransformProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputBottomLeft' instead.")]
		[CoreImageFilterProperty ("inputBottomLeft")]
		CIVector BottomLeft { get; set; }

		[Obsolete ("Use 'InputTopRight' instead.")]
		[CoreImageFilterProperty ("inputTopRight")]
		CIVector TopRight { get; set; }

		[Obsolete ("Use 'InputTopLeft' instead.")]
		[CoreImageFilterProperty ("inputTopLeft")]
		CIVector TopLeft { get; set; }

		[Obsolete ("Use 'InputBottomRight' instead.")]
		[CoreImageFilterProperty ("inputBottomRight")]
		CIVector BottomRight { get; set; }
#endif

		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
	}

	[CoreImageFilter]
	[Mac (10,11)]
	[BaseType (typeof (CIPerspectiveTransform))]
	interface CIPerspectiveTransformWithExtent : CIPerspectiveTransformWithExtentProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputExtent' instead.")]
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
#endif
	}

	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[iOS (7,0)]
	[Mac (10,9)]
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIPhotoEffect : CIPhotoEffectProtocol {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectChrome {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectFade {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectInstant {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectMono {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectNoir {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectProcess {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectTonal {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectTransfer {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIPinchDistortion {

		[CoreImageFilterProperty ("inputScale")]
		float Scale { get; set; }
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[Mac (10,10)]
	[BaseType (typeof (CIBlendFilter))]
	interface CIPinLightBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIPixellate : CIPixellateProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIPointillize : CIPointillizeProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CICodeGenerator))]
	interface CIQRCodeGenerator : CIQRCodeGeneratorProtocol {

		[CoreImageFilterProperty ("outputCGImage")]
		CGImage OutputCGImage { get; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIRadialGradient : CIRadialGradientProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIRandomGenerator : CIRandomGeneratorProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CITransitionFilter))]
	interface CIRippleTransition : CIRippleTransitionProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputExtent' instead.")]
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }

		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIRowAverage {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CISaturationBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIScreenBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CISepiaTone : CISepiaToneProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIShadedMaterial : CIShadedMaterialProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CISharpenLuminance : CISharpenLuminanceProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CISixfoldReflectedTile : CISixfoldReflectedTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CISixfoldRotatedTile : CISixfoldRotatedTileProtocol {
	}

	[CoreImageFilter]
	[Mac (10,11)]
	[BaseType (typeof (CILinearGradient))]
	interface CISmoothLinearGradient : CISmoothLinearGradientProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputPoint1' instead.")]
		[CoreImageFilterProperty ("inputPoint1")]
		CIVector Point1 { get; set; }

		[Obsolete ("Use 'InputPoint0' instead.")]
		[CoreImageFilterProperty ("inputPoint0")]
		CIVector Point0 { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CISoftLightBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CISourceAtopCompositing {
	}

	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CISourceInCompositing {
	}

	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CISourceOutCompositing {
	}

	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CISourceOverCompositing {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CISpotColor : CISpotColorProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CISpotLight : CISpotLightProtocol {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,10)]
	[BaseType (typeof (CIFilter))]
	interface CISRGBToneCurveToLinear : CISrgbToneCurveToLinearProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIStarShineGenerator : CIStarShineGeneratorProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIStraightenFilter : CIStraightenProtocol {
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIStretchCrop {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputCropAmount")]
		float CropAmount { get; set; }

		[CoreImageFilterProperty ("inputCenterStretchAmount")]
		float CenterStretchAmount { get; set; }

		[CoreImageFilterProperty ("inputSize")]
		CIVector Size { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIStripesGenerator : CIStripesGeneratorProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (8,0)]
	[Mac (10,10)]
	[BaseType (typeof (CIBlendFilter))]
	interface CISubtractBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CISwipeTransition {

		[CoreImageFilterProperty ("inputColor")]
		CIColor Color { get; set; }

		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }

		[CoreImageFilterProperty ("inputOpacity")]
		float Opacity { get; set; }

		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CITemperatureAndTint : CITemperatureAndTintProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIToneCurve : CIToneCurveProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputPoint0' instead.")]
		[CoreImageFilterProperty ("inputPoint0")]
		CIVector Point0 { get; set; }

		[Obsolete ("Use 'InputPoint1' instead.")]
		[CoreImageFilterProperty ("inputPoint1")]
		CIVector Point1 { get; set; }

		[Obsolete ("Use 'InputPoint2' instead.")]
		[CoreImageFilterProperty ("inputPoint2")]
		CIVector Point2 { get; set; }

		[Obsolete ("Use 'InputPoint3' instead.")]
		[CoreImageFilterProperty ("inputPoint3")]
		CIVector Point3 { get; set; }

		[Obsolete ("Use 'InputPoint4' instead.")]
		[CoreImageFilterProperty ("inputPoint4")]
		CIVector Point4 { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CITorusLensDistortion {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputRefraction")]
		float Refraction { get; set; }

		[CoreImageFilterProperty ("inputRadius")]
		float  Radius { get; set; }

		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }

		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }
	}

	[CoreImageFilter]
	[Mac (10,11)]
	[BaseType (typeof (CIFilter))]
	interface CITriangleKaleidoscope : CITriangleKaleidoscopeProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputPoint' instead.")]
		[CoreImageFilterProperty ("inputPoint")]
		CIVector Point { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CITileFilter))]
	interface CITriangleTile : CITriangleTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CITwelvefoldReflectedTile : CITwelvefoldReflectedTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CITwirlDistortion {

		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIUnsharpMask : CIUnsharpMaskProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIVibrance : CIVibranceProtocol {
	}

	[CoreImageFilter]
	[Mac (10,9)]
	[BaseType (typeof (CIFilter))]
	interface CIVignette : CIVignetteProtocol {
	}

	[CoreImageFilter]
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (CIFilter))]
	interface CIVignetteEffect : CIVignetteEffectProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIVortexDistortion {

		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIWhitePointAdjust : CIWhitePointAdjustProtocol {
	}

	[CoreImageFilter]
	[iOS (8,3)]
	[BaseType (typeof (CIFilter))]
	interface CIZoomBlur : CIZoomBlurProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CIDepthOfField : CIDepthOfFieldProtocol {

#if !XAMCORE_4_0
		// renamed 1 vs 0 for API compatibility
		[Obsolete ("Use 'InputPoint0' instead.")]
		[CoreImageFilterProperty ("inputPoint0")]
		CIVector Point1 { get; set; }

		// renamed 2 vs 1 for API compatibility
		[Obsolete ("Use 'InputPoint1' instead.")]
		[CoreImageFilterProperty ("inputPoint1")]
		CIVector Point2 { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (9,0)]
	[BaseType (typeof (CIFilter))]
	interface CISunbeamsGenerator : CISunbeamsGeneratorProtocol {
#if !XAMCORE_4_0
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif

#if !XAMCORE_3_0
		// binding mistake - it should never been added
		[CoreImageFilterProperty ("inputCropAmount")]
		float CropAmount { get; set; }
#endif
	}

	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.PrivateScope)]
	[BaseType (typeof (CIFilter))]
	interface CIFaceBalance {
	}

	[iOS (9,3)]
	[TV (9,2)]
	[Mac (10,10)]
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIMaskedVariableBlur : CIMaskedVariableBlurProtocol {
	}

	[CoreImageFilter]
	[iOS (10,0)]
	[Mac (10,12)]
	[TV (10,0)]
	[BaseType (typeof (CIFilter))]
	interface CIClamp {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	[CoreImageFilter]
	[iOS (10,0)]
	[Mac (10,12)]
	[TV (10,0)]
	[BaseType (typeof (CIFilter))]
	interface CIHueSaturationValueGradient : CIHueSaturationValueGradientProtocol {
	}

	[CoreImageFilter]
	[iOS (10,0)]
	[Mac (10,12)]
	[TV (10,0)]
	[BaseType (typeof (CIFilter))]
	interface CINinePartStretched {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputGrowAmount")]
		CIVector GrowAmount { get; set; }

		[CoreImageFilterProperty ("inputBreakpoint0")]
		CIVector Breakpoint0 { get; set; }

		[CoreImageFilterProperty ("inputBreakpoint1")]
		CIVector Breakpoint1 { get; set; }
	}

	[CoreImageFilter]
	[iOS (10,0)]
	[Mac (10,12)]
	[TV (10,0)]
	[BaseType (typeof (CIFilter))]
	interface CINinePartTiled {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputGrowAmount")]
		CIVector GrowAmount { get; set; }

		[CoreImageFilterProperty ("inputBreakpoint0")]
		CIVector Breakpoint0 { get; set; }

		[CoreImageFilterProperty ("inputFlipYTiles")]
		bool FlipYTiles { get; set; }

		[CoreImageFilterProperty ("inputBreakpoint1")]
		CIVector Breakpoint1 { get; set; }
	}

	[CoreImageFilter]
	[iOS (10,0)]
	[Mac (10,12)] // filter says 10.11 but it fails when I run it on El Capitan
	[TV (10,0)]
	[BaseType (typeof (CIFilter))]
	interface CIThermal : CIThermalProtocol {
	}

	[CoreImageFilter]
	[iOS (10,0)]
	[Mac (10,12)] // filter says 10.11 but it fails when I run it on El Capitan
	[TV (10,0)]
	[BaseType (typeof (CIFilter))]
	interface CIXRay : CIXRayProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIAreaMaximum))]
	interface CIAreaMinMaxRed {
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	[CoreImageFilter]
	[Abstract]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIFilter))]
	interface CIImageGenerator {
		[CoreImageFilterProperty ("inputScaleFactor")]
		float ScaleFactor { get; set; }
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIImageGenerator))]
	interface CIAttributedTextImageGenerator : CIAttributedTextImageGeneratorProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIFilter))]
	interface CIBarcodeGenerator : CIBarcodeGeneratorProtocol {

		[CoreImageFilterProperty ("outputCGImageForQRCodeDescriptor")]
		CGImage OutputCGImageForQRCodeDescriptor { get; }

		[CoreImageFilterProperty ("outputCGImageForPDF417CodeDescriptor")]
		CGImage OutputCGImageForPdf417CodeDescriptor { get; }

		[CoreImageFilterProperty ("outputCGImageForDataMatrixCodeDescriptor")]
		CGImage OutputCGImageForDataMatrixCodeDescriptor { get; }

		[CoreImageFilterProperty ("outputCGImageForAztecCodeDescriptor")]
		CGImage OutputCGImageForAztecCodeDescriptor { get; }

		[CoreImageFilterProperty ("outputCGImage")]
		CGImage OutputCGImage { get; }
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	// Maybe 'typeof (CIScaleTransform)' (shared 'Scale' and 'AspectRatio' property).
	// It's possible to add ours but it can bite us back in the future if Apple introduce the same with different properties.
	[BaseType (typeof (CIFilter))]
	interface CIBicubicScaleTransform : CIBicubicScaleTransformProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'ParameterB' instead.")]
		[CoreImageFilterProperty ("inputB")]
		float B { get; set; }

		[Obsolete ("Use 'ParameterC' instead.")]
		[CoreImageFilterProperty ("inputC")]
		float C { get; set; }
#endif
	}

	[CoreImageFilter]
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CILinearBlur {
		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CILinearBlur))]
	interface CIBokehBlur : CIBokehBlurProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIFilter))] // Could almost be typeof 'CIColorCube' but property is 'inputCube0Data' not 'inputCubeData'
	interface CIColorCubesMixedWithMask : CIColorCubesMixedWithMaskProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIFilter))]
	interface CIColorCurves : CIColorCurvesProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIFilter))]
	interface CIDepthBlurEffect {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputAperture")]
		float Aperture { get; set; }

		[CoreImageFilterProperty ("inputCalibrationData")]
		AVCameraCalibrationData CalibrationData { get; set; }

		// Radar: https://trello.com/c/9eA2BA2o
		// Don't know how to test this as I don't know which keys are valid.
		// [CoreImageFilterProperty ("inputTuningParameters")]
		// NSDictionary WeakTuningParameters { get; set; }

		[CoreImageFilterProperty ("inputNosePositions")]
		CIVector NosePositions { get; set; }

		[CoreImageFilterProperty ("inputLumaNoiseScale")]
		float LumaNoiseScale { get; set; }

		[CoreImageFilterProperty ("inputChinPositions")]
		CIVector ChinPositions { get; set; }

		[CoreImageFilterProperty ("inputDisparityImage")]
		CIImage DisparityImage { get; set; }

		[CoreImageFilterProperty ("inputScaleFactor")]
		float ScaleFactor { get; set; }

		[CoreImageFilterProperty ("inputRightEyePositions")]
		CIVector RightEyePositions { get; set; }

		[CoreImageFilterProperty ("inputLeftEyePositions")]
		CIVector LeftEyePositions { get; set; }

		[CoreImageFilterProperty ("inputFocusRect")]
		CIVector FocusRect { get; set; }

		[CoreImageFilterProperty ("inputMatteImage")]
		CIImage MatteImage { get; set; }

		[CoreImageFilterProperty ("inputHairImage")]
		CIImage HairImage { get; set; }

		[CoreImageFilterProperty ("inputShape")]
		string Shape { get; set; }

		[CoreImageFilterProperty ("inputAuxDataMetadata")]
		CGImageMetadata AuxDataMetadata { get; set; }
	}

	[CoreImageFilter]
	[Abstract]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIFilter))]
	interface CIDepthDisparityConverter {}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIDepthDisparityConverter))]
	interface CIDepthToDisparity : CIDepthToDisparityProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIDepthDisparityConverter))]
	interface CIDisparityToDepth : CIDisparityToDepthProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIFilter))]
	interface CIEdgePreserveUpsampleFilter : CIEdgePreserveUpsampleProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIFilter))]
	interface CILabDeltaE : CILabDeltaEProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIImageGenerator))]
	interface CITextImageGenerator : CITextImageGeneratorProtocol {
	}

	[CoreImageFilter]
	[Abstract]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIFilter))]
	interface CIMorphology {
		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIMorphology))]
	interface CIMorphologyGradient : CIMorphologyGradientProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIMorphology))]
	interface CIMorphologyMaximum : CIMorphologyMaximumProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIMorphology))]
	interface CIMorphologyMinimum : CIMorphologyMinimumProtocol {
	}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIBlendWithMask))]
	interface CIBlendWithBlueMask {}

	[CoreImageFilter]
	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIBlendWithMask))]
	interface CIBlendWithRedMask {}

	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface CIBarcodeDescriptor : NSSecureCoding, NSCopying {
		// empty
	}

	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIBarcodeDescriptor))]
	interface CIQRCodeDescriptor {

		[Export ("errorCorrectedPayload")]
		NSData ErrorCorrectedPayload { get; }

		[Export ("symbolVersion")]
		nint SymbolVersion { get; }

		[Export ("maskPattern")]
		byte /* uint8_t */ MaskPattern { get; }

		[Export ("errorCorrectionLevel")]
		CIQRCodeErrorCorrectionLevel ErrorCorrectionLevel { get; }

		[Export ("initWithPayload:symbolVersion:maskPattern:errorCorrectionLevel:")]
		IntPtr Constructor (NSData errorCorrectedPayload, nint symbolVersion, byte maskPattern, CIQRCodeErrorCorrectionLevel errorCorrectionLevel);

		[Static]
		[Export ("descriptorWithPayload:symbolVersion:maskPattern:errorCorrectionLevel:")]
		[return: NullAllowed]
		CIQRCodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, nint symbolVersion, byte maskPattern, CIQRCodeErrorCorrectionLevel errorCorrectionLevel);
	}

	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIBarcodeDescriptor))]
	interface CIAztecCodeDescriptor {

		[Export ("errorCorrectedPayload")]
		NSData ErrorCorrectedPayload { get; }

		[Export ("isCompact")]
		bool IsCompact { get; }

		[Export ("layerCount")]
		nint LayerCount { get; }

		[Export ("dataCodewordCount")]
		nint DataCodewordCount { get; }

		[Export ("initWithPayload:isCompact:layerCount:dataCodewordCount:")]
		IntPtr Constructor (NSData errorCorrectedPayload, bool isCompact, nint layerCount, nint dataCodewordCount);

		[Static]
		[Export ("descriptorWithPayload:isCompact:layerCount:dataCodewordCount:")]
		[return: NullAllowed]
		CIAztecCodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, bool isCompact, nint layerCount, nint dataCodewordCount);
	}

	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIBarcodeDescriptor), Name = "CIPDF417CodeDescriptor")]
	interface CIPdf417CodeDescriptor {

		[Export ("errorCorrectedPayload")]
		NSData ErrorCorrectedPayload { get; }

		[Export ("isCompact")]
		bool IsCompact { get; }

		[Export ("rowCount")]
		nint RowCount { get; }

		[Export ("columnCount")]
		nint ColumnCount { get; }

		[Export ("initWithPayload:isCompact:rowCount:columnCount:")]
		IntPtr Constructor (NSData errorCorrectedPayload, bool isCompact, nint rowCount, nint columnCount);

		[Static]
		[Export ("descriptorWithPayload:isCompact:rowCount:columnCount:")]
		[return: NullAllowed]
		CIPdf417CodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, bool isCompact, nint rowCount, nint columnCount);
	}

	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIBarcodeDescriptor))]
	interface CIDataMatrixCodeDescriptor {

		[Export ("errorCorrectedPayload")]
		NSData ErrorCorrectedPayload { get; }

		[Export ("rowCount")]
		nint RowCount { get; }

		[Export ("columnCount")]
		nint ColumnCount { get; }

		[Export ("eccVersion")]
		CIDataMatrixCodeEccVersion EccVersion { get; }

		[Export ("initWithPayload:rowCount:columnCount:eccVersion:")]
		IntPtr Constructor (NSData errorCorrectedPayload, nint rowCount, nint columnCount, CIDataMatrixCodeEccVersion eccVersion);

		[Static]
		[Export ("descriptorWithPayload:rowCount:columnCount:eccVersion:")]
		[return: NullAllowed]
		CIDataMatrixCodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, nint rowCount, nint columnCount, CIDataMatrixCodeEccVersion eccVersion);
	}

	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (CIColorKernel))]
	[DisableDefaultCtor] // Handle is nil for `init`
	interface CIBlendKernel {

		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Static]
		[Export ("kernelWithString:")]
		[return: NullAllowed]
		CIBlendKernel CreateKernel (string @string);

		[Export ("applyWithForeground:background:")]
		[return: NullAllowed]
		CIImage Apply (CIImage foreground, CIImage background);

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Export ("applyWithForeground:background:colorSpace:")]
		[return: NullAllowed]
		CIImage Apply (CIImage foreground, CIImage background, CGColorSpace colorSpace);

		// @interface BuiltIn (CIBlendKernel)

		[Static]
		[Export ("componentAdd", ArgumentSemantic.Strong)]
		CIBlendKernel ComponentAdd { get; }

		[Static]
		[Export ("componentMultiply", ArgumentSemantic.Strong)]
		CIBlendKernel ComponentMultiply { get; }

		[Static]
		[Export ("componentMin", ArgumentSemantic.Strong)]
		CIBlendKernel ComponentMin { get; }

		[Static]
		[Export ("componentMax", ArgumentSemantic.Strong)]
		CIBlendKernel ComponentMax { get; }

		[Static]
		[Export ("clear", ArgumentSemantic.Strong)]
		CIBlendKernel Clear { get; }

		[Static]
		[Export ("source", ArgumentSemantic.Strong)]
		CIBlendKernel Source { get; }

		[Static]
		[Export ("destination", ArgumentSemantic.Strong)]
		CIBlendKernel Destination { get; }

		[Static]
		[Export ("sourceOver", ArgumentSemantic.Strong)]
		CIBlendKernel SourceOver { get; }

		[Static]
		[Export ("destinationOver", ArgumentSemantic.Strong)]
		CIBlendKernel DestinationOver { get; }

		[Static]
		[Export ("sourceIn", ArgumentSemantic.Strong)]
		CIBlendKernel SourceIn { get; }

		[Static]
		[Export ("destinationIn", ArgumentSemantic.Strong)]
		CIBlendKernel DestinationIn { get; }

		[Static]
		[Export ("sourceOut", ArgumentSemantic.Strong)]
		CIBlendKernel SourceOut { get; }

		[Static]
		[Export ("destinationOut", ArgumentSemantic.Strong)]
		CIBlendKernel DestinationOut { get; }

		[Static]
		[Export ("sourceAtop", ArgumentSemantic.Strong)]
		CIBlendKernel SourceAtop { get; }

		[Static]
		[Export ("destinationAtop", ArgumentSemantic.Strong)]
		CIBlendKernel DestinationAtop { get; }

		[Static]
		[Export ("exclusiveOr", ArgumentSemantic.Strong)]
		CIBlendKernel ExclusiveOr { get; }

		[Static]
		[Export ("multiply", ArgumentSemantic.Strong)]
		CIBlendKernel Multiply { get; }

		[Static]
		[Export ("screen", ArgumentSemantic.Strong)]
		CIBlendKernel Screen { get; }

		[Static]
		[Export ("overlay", ArgumentSemantic.Strong)]
		CIBlendKernel Overlay { get; }

		[Static]
		[Export ("darken", ArgumentSemantic.Strong)]
		CIBlendKernel Darken { get; }

		[Static]
		[Export ("lighten", ArgumentSemantic.Strong)]
		CIBlendKernel Lighten { get; }

		[Static]
		[Export ("colorDodge", ArgumentSemantic.Strong)]
		CIBlendKernel ColorDodge { get; }

		[Static]
		[Export ("colorBurn", ArgumentSemantic.Strong)]
		CIBlendKernel ColorBurn { get; }

		[Static]
		[Export ("hardLight", ArgumentSemantic.Strong)]
		CIBlendKernel HardLight { get; }

		[Static]
		[Export ("softLight", ArgumentSemantic.Strong)]
		CIBlendKernel SoftLight { get; }

		[Static]
		[Export ("difference", ArgumentSemantic.Strong)]
		CIBlendKernel Difference { get; }

		[Static]
		[Export ("exclusion", ArgumentSemantic.Strong)]
		CIBlendKernel Exclusion { get; }

		[Static]
		[Export ("hue", ArgumentSemantic.Strong)]
		CIBlendKernel Hue { get; }

		[Static]
		[Export ("saturation", ArgumentSemantic.Strong)]
		CIBlendKernel Saturation { get; }

		[Static]
		[Export ("color", ArgumentSemantic.Strong)]
		CIBlendKernel Color { get; }

		[Static]
		[Export ("luminosity", ArgumentSemantic.Strong)]
		CIBlendKernel Luminosity { get; }

		[Static]
		[Export ("subtract", ArgumentSemantic.Strong)]
		CIBlendKernel Subtract { get; }

		[Static]
		[Export ("divide", ArgumentSemantic.Strong)]
		CIBlendKernel Divide { get; }

		[Static]
		[Export ("linearBurn", ArgumentSemantic.Strong)]
		CIBlendKernel LinearBurn { get; }

		[Static]
		[Export ("linearDodge", ArgumentSemantic.Strong)]
		CIBlendKernel LinearDodge { get; }

		[Static]
		[Export ("vividLight", ArgumentSemantic.Strong)]
		CIBlendKernel VividLight { get; }

		[Static]
		[Export ("linearLight", ArgumentSemantic.Strong)]
		CIBlendKernel LinearLight { get; }

		[Static]
		[Export ("pinLight", ArgumentSemantic.Strong)]
		CIBlendKernel PinLight { get; }

		[Static]
		[Export ("hardMix", ArgumentSemantic.Strong)]
		CIBlendKernel HardMix { get; }

		[Static]
		[Export ("darkerColor", ArgumentSemantic.Strong)]
		CIBlendKernel DarkerColor { get; }

		[Static]
		[Export ("lighterColor", ArgumentSemantic.Strong)]
		CIBlendKernel LighterColor { get; }
	}

	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Handle is null if created thru `init`
	interface CIRenderDestination {

		[Export ("initWithPixelBuffer:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer);

		[Export ("initWithIOSurface:")]
		IntPtr Constructor (IOSurface.IOSurface surface);

		[Export ("initWithMTLTexture:commandBuffer:")]
		IntPtr Constructor (IMTLTexture texture, [NullAllowed] IMTLCommandBuffer commandBuffer);

		[Export ("initWithWidth:height:pixelFormat:commandBuffer:mtlTextureProvider:")]
		IntPtr Constructor (nuint width, nuint height, MTLPixelFormat pixelFormat, [NullAllowed] IMTLCommandBuffer commandBuffer, Func<IMTLTexture> block);

		[Export ("initWithGLTexture:target:width:height:")]
		IntPtr Constructor (uint texture, uint target, nuint width, nuint height);

		[Export ("initWithBitmapData:width:height:bytesPerRow:format:")]
		IntPtr Constructor (IntPtr data, nuint width, nuint height, nuint bytesPerRow, CIFormat format);

		[Export ("width")]
		nuint Width { get; }

		[Export ("height")]
		nuint Height { get; }

		[Export ("alphaMode", ArgumentSemantic.Assign)]
		CIRenderDestinationAlphaMode AlphaMode { get; set; }

		[Export ("flipped")]
		bool Flipped { [Bind ("isFlipped")] get; set; }

		[Export ("dithered")]
		bool Dithered { [Bind ("isDithered")] get; set; }

		[Export ("clamped")]
		bool Clamped { [Bind ("isClamped")] get; set; }

		[NullAllowed, Export ("colorSpace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }

		[NullAllowed, Export ("blendKernel", ArgumentSemantic.Retain)]
		CIBlendKernel BlendKernel { get; set; }

		[Export ("blendsInDestinationColorSpace")]
		bool BlendsInDestinationColorSpace { get; set; }
	}

	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no docs, but only returned from CIRenderTask.WaitUntilCompleted. Handle is null if created thru `init`
	interface CIRenderInfo {

		[Export ("kernelExecutionTime")]
		double KernelExecutionTime { get; }

		[Export ("passCount")]
		nint PassCount { get; }

		[Export ("pixelsProcessed")]
		nint PixelsProcessed { get; }
	}

	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no docs, but only returned from CIContext.StartTaskToRender. Handle is null if created thru `init`
	interface CIRenderTask {
		[Export ("waitUntilCompletedAndReturnError:")]
		[return: NullAllowed]
		CIRenderInfo WaitUntilCompleted ([NullAllowed] out NSError error);
	}

	[iOS (11,0)]
	[Mac (10,13)]
	[TV (11,0)]
	[Category]
	[BaseType (typeof (CIContext))]
	interface CIContext_CIRenderDestination {

		[Export ("prepareRender:fromRect:toDestination:atPoint:error:")]
		bool PrepareRender (CIImage image, CGRect fromRect, CIRenderDestination destination, CGPoint atPoint, [NullAllowed] out NSError error);

		[Export ("startTaskToRender:fromRect:toDestination:atPoint:error:")]
		[return: NullAllowed]
		CIRenderTask StartTaskToRender (CIImage image, CGRect fromRect, CIRenderDestination destination, CGPoint atPoint, [NullAllowed] out NSError error);

		[Export ("startTaskToRender:toDestination:error:")]
		[return: NullAllowed]
		CIRenderTask StartTaskToRender (CIImage image, CIRenderDestination destination, [NullAllowed] out NSError error);

		[Export ("startTaskToClear:error:")]
		[return: NullAllowed]
		CIRenderTask StartTaskToClear (CIRenderDestination destination, [NullAllowed] out NSError error);
	}

	[iOS (11,0)]
	[TV (11,0)]
	[Mac (10,13)]
	[Static]
	[Internal]
	partial interface CIImageRepresentationKeys {

		[Field ("kCGImageDestinationLossyCompressionQuality", "ImageIO")]
		NSString LossyCompressionQualityKey { get; }

		[Field ("kCIImageRepresentationAVDepthData")]
		NSString AVDepthDataKey { get; }

		[Field ("kCIImageRepresentationDepthImage")]
		NSString DepthImageKey { get; }

		[Field ("kCIImageRepresentationDisparityImage")]
		NSString DisparityImageKey { get; }

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		[Field ("kCIImageRepresentationAVPortraitEffectsMatte")]
		NSString AVPortraitEffectsMatteKey { get; }

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		[Field ("kCIImageRepresentationPortraitEffectsMatteImage")]
		NSString PortraitEffectsMatteImageKey { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Field ("kCIImageRepresentationAVSemanticSegmentationMattes")]
		NSString SemanticSegmentationMattesKey { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Field ("kCIImageRepresentationSemanticSegmentationSkinMatteImage")]
		NSString SemanticSegmentationSkinMatteImageKey { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Field ("kCIImageRepresentationSemanticSegmentationHairMatteImage")]
		NSString SemanticSegmentationHairMatteImageKey { get; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Field ("kCIImageRepresentationSemanticSegmentationTeethMatteImage")]
		NSString SemanticSegmentationTeethMatteImageKey { get; }
	}

	[iOS (11,0)]
	[TV (11,0)]
	[Mac (10,13)]
	[StrongDictionary ("CIImageRepresentationKeys")]
	interface CIImageRepresentationOptions {

		float LossyCompressionQuality { get; set; }

		AVDepthData AVDepthData { get; set; }

		CIImage DepthImage { get; set; }

		CIImage DisparityImage { get; set; }

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		CIImage PortraitEffectsMatteImage { get; set; }

		[TV (12, 0), iOS (12, 0), Mac (10, 14)]
		AVPortraitEffectsMatte AVPortraitEffectsMatte { get; set; } 

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		AVSemanticSegmentationMatte[] SemanticSegmentationMattes { get; set; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		CIImage SemanticSegmentationSkinMatteImage { get; set; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		CIImage SemanticSegmentationHairMatteImage { get; set; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		CIImage SemanticSegmentationTeethMatteImage { get; set; }
	}

	[CoreImageFilter]
	[iOS (12,0)]
	[TV (12,0)]
	[Mac (10,14)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMinMax {

		[CoreImageFilterProperty ("outputImageNonMPS")]
		CIImage OutputImageNonMps { get; }

		[iOS (13,4)]
		[CoreImageFilterProperty ("outputImageMPS")]
		CIImage OutputImageMps { get; }
	}

	[CoreImageFilter]
	[iOS (12,0)]
	[TV (12,0)]
	[Mac (10,14)]
	[BaseType (typeof (CIFilter))]
	interface CIDither : CIDitherProtocol {
	}

	[CoreImageFilter]
	[iOS (12,0)]
	[TV (12,0)]
	[Mac (10,14)]
	[BaseType (typeof (CIFilter))]
	interface CIGuidedFilter {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputGuideImage")]
		CIImage GuideImage { get; set; }

		[CoreImageFilterProperty ("inputEpsilon")]
		float Epsilon { get; set; }

		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }
	}

	[CoreImageFilter]
	[iOS (12,0)]
	[TV (12,0)]
	[Mac (10,14)]
	[BaseType (typeof (CIFilter))]
	interface CIMeshGenerator : CIMeshGeneratorProtocol {
	}

	[CoreImageFilter]
	[iOS (12,0)]
	[TV (12,0)]
	[Mac (10,14)]
	[BaseType (typeof (CIFilter))]
	interface CIMix : CIMixProtocol {
	}

	[CoreImageFilter]
	[iOS (12,0)]
	[TV (12,0)]
	[Mac (10,14)]
	[BaseType (typeof (CIFilter))]
	interface CISampleNearest {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }
	}

	[CoreImageFilter]
	[iOS (12,0)]
	[TV (12,0)]
	[Mac (10,14)]
	[BaseType (typeof (CIFilter))]
	interface CICameraCalibrationLensCorrection {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputAVCameraCalibrationData")]
		AVCameraCalibrationData AVCameraCalibrationData { get; set; }

		[CoreImageFilterProperty ("inputUseInverseLookUpTable")]
		bool UseInverseLookUpTable { get; set; }
	}

	[CoreImageFilter]
	[iOS (12,0)]
	[TV (12,0)]
	[Mac (10,14)]
	[BaseType (typeof (CIFilter))]
	interface CICoreMLModelFilter {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputModel")]
		MLModel Model { get; set; }

		[CoreImageFilterProperty ("inputHeadIndex")]
		int HeadIndex { get; set; }

		[CoreImageFilterProperty ("inputSoftmaxNormalization")]
		bool SoftmaxNormalization { get; set; }
	}

	[CoreImageFilter]
	[iOS (12,0)]
	[TV (12,0)]
	[Mac (10,14)]
	[BaseType (typeof (CIFilter))]
	interface CISaliencyMapFilter : CISaliencyMapProtocol {
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIFilter))]
	interface CIDocumentEnhancer : CIDocumentEnhancerProtocol {
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIKMeans {

		[CoreImageFilterProperty ("inputMeans")]
		CIImage Means { get; set; }

		[CoreImageFilterProperty ("inputCount")]
		float Count { get; set; }

		[CoreImageFilterProperty ("inputPasses")]
		int Passes { get; set; }

		[CoreImageFilterProperty ("inputPerceptual")]
		bool Perceptual { get; set; }
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIFilter))]
	[Abstract]
	interface CIMorphologyRectangle {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputHeight' instead.")]
		[CoreImageFilterProperty ("inputHeight")]
		int Height { get; set; }

		[Obsolete ("Use 'InputWidth' instead.")]
		[CoreImageFilterProperty ("inputWidth")]
		int Width { get; set; }
#endif

		[CoreImageFilterProperty ("inputHeight")]
		float InputHeight { get; set; }

		[CoreImageFilterProperty ("inputWidth")]
		float InputWidth { get; set; }
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIMorphologyRectangle))]
	interface CIMorphologyRectangleMaximum : CIMorphologyRectangleMaximumProtocol {
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIMorphologyRectangle))]
	interface CIMorphologyRectangleMinimum : CIMorphologyRectangleMinimumProtocol {
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIFilter))]
	interface CIPaletteCentroid : CIPaletteCentroidProtocol {
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIFilter))]
	interface CIPalettize : CIPalettizeProtocol {
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIFilter))]
	[Abstract]
	interface CIKeystoneCorrection {

		[CoreImageFilterProperty ("inputFocalLength")]
		float FocalLength { get; set; }

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputTopRight' instead.")]
		[CoreImageFilterProperty ("inputTopRight")]
		CIVector TopRight { get; set; }

		[Obsolete ("Use 'InputBottomRight' instead.")]
		[CoreImageFilterProperty ("inputBottomRight")]
		CIVector BottomRight { get; set; }

		[Obsolete ("Use 'InputTopLeft' instead.")]
		[CoreImageFilterProperty ("inputTopLeft")]
		CIVector TopLeft { get; set; }

		[Obsolete ("Use 'InputBottomLeft' instead.")]
		[CoreImageFilterProperty ("inputBottomLeft")]
		CIVector BottomLeft { get; set; }
#endif

		[CoreImageFilterProperty ("inputTopRight")]
		CGPoint InputTopRight { get; set; }

		[CoreImageFilterProperty ("inputBottomRight")]
		CGPoint InputBottomRight { get; set; }

		[CoreImageFilterProperty ("inputTopLeft")]
		CGPoint InputTopLeft { get; set; }

		[CoreImageFilterProperty ("inputBottomLeft")]
		CGPoint InputBottomLeft { get; set; }
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIKeystoneCorrection))]
	interface CIKeystoneCorrectionCombined : CIKeystoneCorrectionCombinedProtocol {

		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIKeystoneCorrection))]
	interface CIKeystoneCorrectionHorizontal : CIKeystoneCorrectionHorizontalProtocol {

#if false // no documentation about the type
		[CoreImageFilterProperty ("outputRotationFilter")]
		NSObject OutputRotationFilter { get; }
#endif

		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIKeystoneCorrection))]
	interface CIKeystoneCorrectionVertical : CIKeystoneCorrectionVerticalProtocol {

#if false // no documentation about the type
		[CoreImageFilterProperty ("outputRotationFilter")]
		CGAffineTransform OutputRotationFilter { get; }
#endif

		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIFilter))]
	interface CIPerspectiveRotate : CIPerspectiveRotateProtocol {

		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIFilter))]
	interface CIGaborGradients : CIGaborGradientsProtocol {
	}

	[CoreImageFilter]
	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[BaseType (typeof (CIFilter))]
	interface CIRoundedRectangleGenerator : CIRoundedRectangleGeneratorProtocol {

#if !XAMCORE_4_0
		[Obsolete ("Use 'InputExtent' instead.")]
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
#endif
	}

#region Protocols

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIFilter")]
	// CIFilter already exists so we're using the Swift name
	interface CIFilterProtocol {

		[Abstract]
		[CoreImageFilterProperty ("outputImage")]
		[NullAllowed, Export ("outputImage")]
		CIImage OutputImage { get; }

		[Static]
		[NullAllowed, Export ("customAttributes")]
		NSDictionary<NSString, NSObject> CustomAttributes { get; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CITransitionFilter")]
	interface CITransitionFilterProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("targetImage", ArgumentSemantic.Retain)]
		CIImage TargetImage { get; set; }

		[Abstract]
		[Export ("time")]
		float Time { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIAccordionFoldTransition")]
	interface CIAccordionFoldTransitionProtocol : CITransitionFilterProtocol {

		[Abstract]
		[Export ("bottomHeight")]
		float BottomHeight { get; set; }

		[Abstract]
		[Export ("numberOfFolds")]
		// renamed for compatibility (originally bound as an integer)
		float FoldCount { get; set; }

		[Abstract]
		[Export ("foldShadowAmount")]
		float FoldShadowAmount { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIAffineClamp")]
	interface CIAffineClampProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("transform", ArgumentSemantic.Assign)]
		CGAffineTransform Transform { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIAffineTile")]
	interface CIAffineTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("transform", ArgumentSemantic.Assign)]
		CGAffineTransform Transform { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIAttributedTextImageGenerator")]
	interface CIAttributedTextImageGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("text", ArgumentSemantic.Retain)]
		NSAttributedString Text { get; set; }

		[Abstract]
		[Export ("scaleFactor")]
		float ScaleFactor { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIAztecCodeGenerator")]
	interface CIAztecCodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("message", ArgumentSemantic.Retain)]
		NSData Message { get; set; }

		[Abstract]
		[Export ("correctionLevel")]
		float CorrectionLevel { get; set; }

		[Abstract]
		[Export ("layers")]
		float InputLayers { get; set; }

		[Abstract]
		[Export ("compactStyle")]
		float InputCompactStyle { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIBarcodeGenerator")]
	interface CIBarcodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("barcodeDescriptor", ArgumentSemantic.Retain)]
		CIBarcodeDescriptor BarcodeDescriptor { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIBarsSwipeTransition")]
	interface CIBarsSwipeTransitionProtocol : CITransitionFilterProtocol {

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("barOffset")]
		float BarOffset { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIBicubicScaleTransform")]
	interface CIBicubicScaleTransformProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		[Abstract]
		[Export ("aspectRatio")]
		float AspectRatio { get; set; }

		[Abstract]
		[CoreImageFilterProperty ("inputB")] // name differs from the export
		[Export ("parameterB")]
		float ParameterB { get; set; }

		[Abstract]
		[CoreImageFilterProperty ("inputC")] // name differs from the export
		[Export ("parameterC")]
		float ParameterC { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIBlendWithMask")]
	interface CIBlendWithMaskProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("backgroundImage", ArgumentSemantic.Retain)]
		CIImage BackgroundImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("maskImage", ArgumentSemantic.Retain)]
		CIImage MaskImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIBloom")]
	interface CIBloomProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIBokehBlur")]
	interface CIBokehBlurProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("ringAmount")]
		float RingAmount { get; set; }

		[Abstract]
		[Export ("ringSize")]
		float RingSize { get; set; }

		[Abstract]
		[Export ("softness")]
		float Softness { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIBoxBlur")]
	interface CIBoxBlurProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CICheckerboardGenerator")]
	interface CICheckerboardGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CICircularScreen")]
	interface CICircularScreenProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CICMYKHalftone")]
	interface CICmykHalftoneProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }

		[Abstract]
		[CoreImageFilterProperty ("inputGCR")]
		[Export ("grayComponentReplacement")]
		float GrayComponentReplacement { get; set; }

		[Abstract]
		[CoreImageFilterProperty ("inputUCR")]
		[Export ("underColorRemoval")]
		float UnderColorRemoval { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CICode128BarcodeGenerator")]
	interface CICode128BarcodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("message", ArgumentSemantic.Retain)]
		NSData Message { get; set; }

		[Abstract]
		[Export ("quietSpace")]
		float QuietSpace { get; set; }

		// The availability attributes here look redundant because they're already on the type,
		// but it makes a difference when this member is inlined into another type, in which case
		// these attributes are copied as well (while the type's attributes aren't).
		[iOS (13,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[Abstract]
		[Export ("barcodeHeight")]
		float BarcodeHeight { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorClamp")]
	interface CIColorClampProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("minComponents", ArgumentSemantic.Retain)]
		CIVector MinComponents { get; set; }

		[Abstract]
		[Export ("maxComponents", ArgumentSemantic.Retain)]
		CIVector MaxComponents { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorControls")]
	interface CIColorControlsProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("saturation")]
		float Saturation { get; set; }

		[Abstract]
		[Export ("brightness")]
		float Brightness { get; set; }

		[Abstract]
		[Export ("contrast")]
		float Contrast { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorCrossPolynomial")]
	interface CIColorCrossPolynomialProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("redCoefficients", ArgumentSemantic.Retain)]
		CIVector RedCoefficients { get; set; }

		[Abstract]
		[Export ("greenCoefficients", ArgumentSemantic.Retain)]
		CIVector GreenCoefficients { get; set; }

		[Abstract]
		[Export ("blueCoefficients", ArgumentSemantic.Retain)]
		CIVector BlueCoefficients { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorCube")]
	interface CIColorCubeProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("cubeDimension")]
		float CubeDimension { get; set; }

		[Abstract]
		[Export ("cubeData", ArgumentSemantic.Retain)]
		NSData CubeData { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorCubesMixedWithMask")]
	interface CIColorCubesMixedWithMaskProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("maskImage", ArgumentSemantic.Retain)]
		CIImage MaskImage { get; set; }

		[Abstract]
		[Export ("cubeDimension")]
		float CubeDimension { get; set; }

		[Abstract]
		[Export ("cube0Data", ArgumentSemantic.Retain)]
		NSData Cube0Data { get; set; }

		[Abstract]
		[Export ("cube1Data", ArgumentSemantic.Retain)]
		NSData Cube1Data { get; set; }

		[Abstract]
		[NullAllowed, Export ("colorSpace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorCubeWithColorSpace")]
	interface CIColorCubeWithColorSpaceProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("cubeDimension")]
		float CubeDimension { get; set; }

		[Abstract]
		[Export ("cubeData", ArgumentSemantic.Retain)]
		NSData CubeData { get; set; }

		[Abstract]
		[NullAllowed, Export ("colorSpace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorCurves")]
	interface CIColorCurvesProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("curvesData", ArgumentSemantic.Retain)]
		NSData CurvesData { get; set; }

		[Abstract]
		[Export ("curvesDomain", ArgumentSemantic.Retain)]
		CIVector CurvesDomain { get; set; }

		[Abstract]
		[NullAllowed, Export ("colorSpace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorInvert")]
	interface CIColorInvertProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorMap")]
	interface CIColorMapProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("gradientImage", ArgumentSemantic.Retain)]
		CIImage GradientImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorMatrix")]
	interface CIColorMatrixProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("RVector", ArgumentSemantic.Retain)]
		CIVector RVector { get; set; }

		[Abstract]
		[Export ("GVector", ArgumentSemantic.Retain)]
		CIVector GVector { get; set; }

		[Abstract]
		[Export ("BVector", ArgumentSemantic.Retain)]
		CIVector BVector { get; set; }

		[Abstract]
		[Export ("AVector", ArgumentSemantic.Retain)]
		CIVector AVector { get; set; }

		[Abstract]
		[Export ("biasVector", ArgumentSemantic.Retain)]
		CIVector BiasVector { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorMonochrome")]
	interface CIColorMonochromeProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorPolynomial")]
	interface CIColorPolynomialProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("redCoefficients", ArgumentSemantic.Retain)]
		CIVector RedCoefficients { get; set; }

		[Abstract]
		[Export ("greenCoefficients", ArgumentSemantic.Retain)]
		CIVector GreenCoefficients { get; set; }

		[Abstract]
		[Export ("blueCoefficients", ArgumentSemantic.Retain)]
		CIVector BlueCoefficients { get; set; }

		[Abstract]
		[Export ("alphaCoefficients", ArgumentSemantic.Retain)]
		CIVector AlphaCoefficients { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIColorPosterize")]
	interface CIColorPosterizeProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("levels")]
		float Levels { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIComicEffect")]
	interface CIComicEffectProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CICompositeOperation")]
	interface CICompositeOperationProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("backgroundImage", ArgumentSemantic.Retain)]
		CIImage BackgroundImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIConvolution")]
	interface CIConvolutionProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("weights", ArgumentSemantic.Retain)]
		CIVector Weights { get; set; }

		[Abstract]
		[Export ("bias")]
		float Bias { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CICopyMachineTransition")]
	interface CICopyMachineTransitionProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect Extent { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("opacity")]
		float Opacity { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CICoreMLModel")]
	interface CICoreMLModelProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("model", ArgumentSemantic.Retain)]
		MLModel Model { get; set; }

		[Abstract]
		[Export ("headIndex")]
		float HeadIndex { get; set; }

		[Abstract]
		[Export ("softmaxNormalization")]
		bool SoftmaxNormalization { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CICrystallize")]
	interface CICrystallizeProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIDepthOfField")]
	interface CIDepthOfFieldProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("point0", ArgumentSemantic.Assign)]
		CGPoint InputPoint0 { get; set; }

		[Abstract]
		[Export ("point1", ArgumentSemantic.Assign)]
		CGPoint InputPoint1 { get; set; }

		[Abstract]
		[Export ("saturation")]
		float Saturation { get; set; }

		[Abstract]
		[Export ("unsharpMaskRadius")]
		float UnsharpMaskRadius { get; set; }

		[Abstract]
		[Export ("unsharpMaskIntensity")]
		float UnsharpMaskIntensity { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIDepthToDisparity")]
	interface CIDepthToDisparityProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIDiscBlur")]
	interface CIDiscBlurProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIDisintegrateWithMaskTransition")]
	interface CIDisintegrateWithMaskTransitionProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("maskImage", ArgumentSemantic.Retain)]
		CIImage MaskImage { get; set; }

		[Abstract]
		[Export ("shadowRadius")]
		float ShadowRadius { get; set; }

		[Abstract]
		[Export ("shadowDensity")]
		float ShadowDensity { get; set; }

		[Abstract]
		[Export ("shadowOffset", ArgumentSemantic.Assign)]
		CGPoint InputShadowOffset { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIDisparityToDepth")]
	interface CIDisparityToDepthProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIDissolveTransition")]
	interface CIDissolveTransitionProtocol : CIFilterProtocol {
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIDither")]
	interface CIDitherProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIDocumentEnhancer")]
	interface CIDocumentEnhancerProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("amount")]
		float Amount { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIDotScreen")]
	interface CIDotScreenProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIEdgePreserveUpsample")]
	interface CIEdgePreserveUpsampleProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("smallImage", ArgumentSemantic.Retain)]
		CIImage SmallImage { get; set; }

		[Abstract]
		[Export ("spatialSigma")]
		float SpatialSigma { get; set; }

		[Abstract]
		[Export ("lumaSigma")]
		float LumaSigma { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIEdges")]
	interface CIEdgesProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIEdgeWork")]
	interface CIEdgeWorkProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIEightfoldReflectedTile")]
	interface CIEightfoldReflectedTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIExposureAdjust")]
	interface CIExposureAdjustProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("EV")]
		float EV { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIFalseColor")]
	interface CIFalseColorProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIFlashTransition")]
	interface CIFlashTransitionProtocol : CITransitionFilterProtocol {

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("maxStriationRadius")]
		float MaxStriationRadius { get; set; }

		[Abstract]
		[Export ("striationStrength")]
		float StriationStrength { get; set; }

		[Abstract]
		[Export ("striationContrast")]
		float StriationContrast { get; set; }

		[Abstract]
		[Export ("fadeThreshold")]
		float FadeThreshold { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIFourCoordinateGeometryFilter")]
	interface CIFourCoordinateGeometryFilterProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("topLeft", ArgumentSemantic.Assign)]
		CGPoint InputTopLeft { get; set; }

		[Abstract]
		[Export ("topRight", ArgumentSemantic.Assign)]
		CGPoint InputTopRight { get; set; }

		[Abstract]
		[Export ("bottomRight", ArgumentSemantic.Assign)]
		CGPoint InputBottomRight { get; set; }

		[Abstract]
		[Export ("bottomLeft", ArgumentSemantic.Assign)]
		CGPoint InputBottomLeft { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIFourfoldReflectedTile")]
	interface CIFourfoldReflectedTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("acuteAngle")]
		float AcuteAngle { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIFourfoldRotatedTile")]
	interface CIFourfoldRotatedTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIFourfoldTranslatedTile")]
	interface CIFourfoldTranslatedTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("acuteAngle")]
		float AcuteAngle { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIGaborGradients")]
	interface CIGaborGradientsProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIGammaAdjust")]
	interface CIGammaAdjustProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("power")]
		float Power { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIGaussianBlur")]
	interface CIGaussianBlurProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIGaussianGradient")]
	interface CIGaussianGradientProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIGlideReflectedTile")]
	interface CIGlideReflectedTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIGloom")]
	interface CIGloomProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIHatchedScreen")]
	interface CIHatchedScreenProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIHeightFieldFromMask")]
	interface CIHeightFieldFromMaskProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIHexagonalPixellate")]
	interface CIHexagonalPixellateProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIHighlightShadowAdjust")]
	interface CIHighlightShadowAdjustProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("shadowAmount")]
		float ShadowAmount { get; set; }

		[Abstract]
		[Export ("highlightAmount")]
		float HighlightAmount { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIHueAdjust")]
	interface CIHueAdjustProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIHueSaturationValueGradient")]
	interface CIHueSaturationValueGradientProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("value")]
		float Value { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("softness")]
		float Softness { get; set; }

		[Abstract]
		[Export ("dither")]
		float Dither { get; set; }

		[Abstract]
		[NullAllowed, Export ("colorSpace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIKaleidoscope")]
	interface CIKaleidoscopeProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("count")]
		nint InputCount { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIKeystoneCorrectionCombined")]
	interface CIKeystoneCorrectionCombinedProtocol : CIFourCoordinateGeometryFilterProtocol {
		
		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIKeystoneCorrectionHorizontal")]
	interface CIKeystoneCorrectionHorizontalProtocol : CIFourCoordinateGeometryFilterProtocol {
		
		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIKeystoneCorrectionVertical")]
	interface CIKeystoneCorrectionVerticalProtocol : CIFourCoordinateGeometryFilterProtocol {
		
		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CILabDeltaE")]
	interface CILabDeltaEProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("image2", ArgumentSemantic.Retain)]
		CIImage Image2 { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CILanczosScaleTransform")]
	interface CILanczosScaleTransformProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		[Abstract]
		[Export ("aspectRatio")]
		float AspectRatio { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CILenticularHaloGenerator")]
	interface CILenticularHaloGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("haloRadius")]
		float HaloRadius { get; set; }

		[Abstract]
		[Export ("haloWidth")]
		float HaloWidth { get; set; }

		[Abstract]
		[Export ("haloOverlap")]
		float HaloOverlap { get; set; }

		[Abstract]
		[Export ("striationStrength")]
		float StriationStrength { get; set; }

		[Abstract]
		[Export ("striationContrast")]
		float StriationContrast { get; set; }

		[Abstract]
		[Export ("time")]
		float Time { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CILinearGradient")]
	interface CILinearGradientProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("point0", ArgumentSemantic.Assign)]
		CGPoint InputPoint0 { get; set; }

		[Abstract]
		[Export ("point1", ArgumentSemantic.Assign)]
		CGPoint InputPoint1 { get; set; }

		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CILinearToSRGBToneCurve")]
	interface CILinearToSrgbToneCurveProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CILineOverlay")]
	interface CILineOverlayProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("NRNoiseLevel")]
		float NRNoiseLevel { get; set; }

		[Abstract]
		[Export ("NRSharpness")]
		float NRSharpness { get; set; }

		[Abstract]
		[Export ("edgeIntensity")]
		float EdgeIntensity { get; set; }

		[Abstract]
		[Export ("threshold")]
		float Threshold { get; set; }

		[Abstract]
		[Export ("contrast")]
		float Contrast { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CILineScreen")]
	interface CILineScreenProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMaskedVariableBlur")]
	interface CIMaskedVariableBlurProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("mask", ArgumentSemantic.Retain)]
		CIImage Mask { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMaskToAlpha")]
	interface CIMaskToAlphaProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMaximumComponent")]
	interface CIMaximumComponentProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMedian")]
	interface CIMedianProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMeshGenerator")]
	interface CIMeshGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("mesh", ArgumentSemantic.Retain)]
		CIVector[] Mesh { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMinimumComponent")]
	interface CIMinimumComponentProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMix")]
	interface CIMixProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("backgroundImage", ArgumentSemantic.Retain)]
		CIImage BackgroundImage { get; set; }

		[Abstract]
		[Export ("amount")]
		float Amount { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIModTransition")]
	interface CIModTransitionProtocol : CITransitionFilterProtocol {

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("compression")]
		float Compression { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMorphologyGradient")]
	interface CIMorphologyGradientProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMorphologyMaximum")]
	interface CIMorphologyMaximumProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMorphologyMinimum")]
	interface CIMorphologyMinimumProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMorphologyRectangleMaximum")]
	interface CIMorphologyRectangleMaximumProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("width")]
		float InputWidth { get; set; }

		[Abstract]
		[Export ("height")]
		float InputHeight { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMorphologyRectangleMinimum")]
	interface CIMorphologyRectangleMinimumProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("width")]
		float InputWidth { get; set; }

		[Abstract]
		[Export ("height")]
		float InputHeight { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIMotionBlur")]
	interface CIMotionBlurProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CINoiseReduction")]
	interface CINoiseReductionProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("noiseLevel")]
		float NoiseLevel { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIOpTile")]
	interface CIOpTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPageCurlTransition")]
	interface CIPageCurlTransitionProtocol : CITransitionFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("backsideImage", ArgumentSemantic.Retain)]
		CIImage BacksideImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("shadingImage", ArgumentSemantic.Retain)]
		CIImage ShadingImage { get; set; }

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPageCurlWithShadowTransition")]
	interface CIPageCurlWithShadowTransitionProtocol : CITransitionFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("backsideImage", ArgumentSemantic.Retain)]
		CIImage BacksideImage { get; set; }

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("shadowSize")]
		float ShadowSize { get; set; }

		[Abstract]
		[Export ("shadowAmount")]
		float ShadowAmount { get; set; }

		[Abstract]
		[Export ("shadowExtent", ArgumentSemantic.Assign)]
		CGRect InputShadowExtent { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPaletteCentroid")]
	interface CIPaletteCentroidProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("paletteImage", ArgumentSemantic.Retain)]
		CIImage PaletteImage { get; set; }

		[Abstract]
		[Export ("perceptual")]
		bool Perceptual { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPalettize")]
	interface CIPalettizeProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("paletteImage", ArgumentSemantic.Retain)]
		CIImage PaletteImage { get; set; }

		[Abstract]
		[Export ("perceptual")]
		bool Perceptual { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIParallelogramTile")]
	interface CIParallelogramTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("acuteAngle")]
		float AcuteAngle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPDF417BarcodeGenerator")]
	interface CIPdf417BarcodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("message", ArgumentSemantic.Retain)]
		NSData Message { get; set; }

		[Abstract]
		[Export ("minWidth")]
		float MinWidth { get; set; }

		[Abstract]
		[Export ("maxWidth")]
		float MaxWidth { get; set; }

		[Abstract]
		[Export ("minHeight")]
		float MinHeight { get; set; }

		[Abstract]
		[Export ("maxHeight")]
		float MaxHeight { get; set; }

		[Abstract]
		[Export ("dataColumns")]
		float InputDataColumns { get; set; }

		[Abstract]
		[Export ("rows")]
		float InputRows { get; set; }

		[Abstract]
		[Export ("preferredAspectRatio")]
		float PreferredAspectRatio { get; set; }

		[Abstract]
		[Export ("compactionMode")]
		float InputCompactionMode { get; set; }

		[Abstract]
		[Export ("compactStyle")]
		float InputCompactStyle { get; set; }

		[Abstract]
		[Export ("correctionLevel")]
		float InputCorrectionLevel { get; set; }

		[Abstract]
		[Export ("alwaysSpecifyCompaction")]
		float InputAlwaysSpecifyCompaction { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPerspectiveCorrection")]
	interface CIPerspectiveCorrectionProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("crop")]
		[iOS (13,0)][TV (13,0)][Mac (10,15)] // repeated so it's inlined (new property in existing filter)
		bool Crop { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPerspectiveRotate")]
	interface CIPerspectiveRotateProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }

		[Abstract]
		[Export ("pitch")]
		float Pitch { get; set; }

		[Abstract]
		[Export ("yaw")]
		float Yaw { get; set; }

		[Abstract]
		[Export ("roll")]
		float Roll { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPerspectiveTile")]
	interface CIPerspectiveTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("topLeft", ArgumentSemantic.Assign)]
		CGPoint InputTopLeft { get; set; }

		[Abstract]
		[Export ("topRight", ArgumentSemantic.Assign)]
		CGPoint InputTopRight { get; set; }

		[Abstract]
		[Export ("bottomRight", ArgumentSemantic.Assign)]
		CGPoint InputBottomRight { get; set; }

		[Abstract]
		[Export ("bottomLeft", ArgumentSemantic.Assign)]
		CGPoint InputBottomLeft { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPerspectiveTransform")]
	interface CIPerspectiveTransformProtocol : CIFourCoordinateGeometryFilterProtocol {
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPerspectiveTransformWithExtent")]
	interface CIPerspectiveTransformWithExtentProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPhotoEffect")]
	interface CIPhotoEffectProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPixellate")]
	interface CIPixellateProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIPointillize")]
	interface CIPointillizeProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIQRCodeGenerator")]
	interface CIQRCodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("message", ArgumentSemantic.Retain)]
		NSData Message { get; set; }

		[Abstract]
		[Export ("correctionLevel", ArgumentSemantic.Retain)]
		string CorrectionLevel { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIRadialGradient")]
	interface CIRadialGradientProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius0")]
		float Radius0 { get; set; }

		[Abstract]
		[Export ("radius1")]
		float Radius1 { get; set; }

		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIRandomGenerator")]
	interface CIRandomGeneratorProtocol : CIFilterProtocol {
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIRippleTransition")]
	interface CIRippleTransitionProtocol : CITransitionFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("shadingImage", ArgumentSemantic.Retain)]
		CIImage ShadingImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIRoundedRectangleGenerator")]
	interface CIRoundedRectangleGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISaliencyMap")]
	interface CISaliencyMapProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISepiaTone")]
	interface CISepiaToneProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIShadedMaterial")]
	interface CIShadedMaterialProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("shadingImage", ArgumentSemantic.Retain)]
		CIImage ShadingImage { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISharpenLuminance")]
	interface CISharpenLuminanceProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }

		[Abstract]
		[Export ("radius")]
		[iOS (13,0)][TV (13,0)][Mac (10,15)] // repeated so it's inlined (new property in existing filter)
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISixfoldReflectedTile")]
	interface CISixfoldReflectedTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISixfoldRotatedTile")]
	interface CISixfoldRotatedTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISmoothLinearGradient")]
	interface CISmoothLinearGradientProtocol : CIFilterProtocol {
		[Abstract]
		[Export ("point0", ArgumentSemantic.Assign)]
		CGPoint InputPoint0 { get; set; }

		[Abstract]
		[Export ("point1", ArgumentSemantic.Assign)]
		CGPoint InputPoint1 { get; set; }

		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISpotColor")]
	interface CISpotColorProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("centerColor1", ArgumentSemantic.Retain)]
		CIColor CenterColor1 { get; set; }

		[Abstract]
		[Export ("replacementColor1", ArgumentSemantic.Retain)]
		CIColor ReplacementColor1 { get; set; }

		[Abstract]
		[Export ("closeness1")]
		float Closeness1 { get; set; }

		[Abstract]
		[Export ("contrast1")]
		float Contrast1 { get; set; }

		[Abstract]
		[Export ("centerColor2", ArgumentSemantic.Retain)]
		CIColor CenterColor2 { get; set; }

		[Abstract]
		[Export ("replacementColor2", ArgumentSemantic.Retain)]
		CIColor ReplacementColor2 { get; set; }

		[Abstract]
		[Export ("closeness2")]
		float Closeness2 { get; set; }

		[Abstract]
		[Export ("contrast2")]
		float Contrast2 { get; set; }

		[Abstract]
		[Export ("centerColor3", ArgumentSemantic.Retain)]
		CIColor CenterColor3 { get; set; }

		[Abstract]
		[Export ("replacementColor3", ArgumentSemantic.Retain)]
		CIColor ReplacementColor3 { get; set; }

		[Abstract]
		[Export ("closeness3")]
		float Closeness3 { get; set; }

		[Abstract]
		[Export ("contrast3")]
		float Contrast3 { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISpotLight")]
	interface CISpotLightProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("lightPosition", ArgumentSemantic.Retain)]
		CIVector LightPosition { get; set; }

		[Abstract]
		[Export ("lightPointsAt", ArgumentSemantic.Retain)]
		CIVector LightPointsAt { get; set; }

		[Abstract]
		[Export ("brightness")]
		float Brightness { get; set; }

		[Abstract]
		[Export ("concentration")]
		float Concentration { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISRGBToneCurveToLinear")]
	interface CISrgbToneCurveToLinearProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIStarShineGenerator")]
	interface CIStarShineGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("crossScale")]
		float CrossScale { get; set; }

		[Abstract]
		[Export ("crossAngle")]
		float CrossAngle { get; set; }

		[Abstract]
		[Export ("crossOpacity")]
		float CrossOpacity { get; set; }

		[Abstract]
		[Export ("crossWidth")]
		float CrossWidth { get; set; }

		[Abstract]
		[Export ("epsilon")]
		float Epsilon { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIStraighten")]
	interface CIStraightenProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIStripesGenerator")]
	interface CIStripesGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISunbeamsGenerator")]
	interface CISunbeamsGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("sunRadius")]
		float SunRadius { get; set; }

		[Abstract]
		[Export ("maxStriationRadius")]
		float MaxStriationRadius { get; set; }

		[Abstract]
		[Export ("striationStrength")]
		float StriationStrength { get; set; }

		[Abstract]
		[Export ("striationContrast")]
		float StriationContrast { get; set; }

		[Abstract]
		[Export ("time")]
		float Time { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CISwipeTransition")]
	interface CISwipeTransitionProtocol : CITransitionFilterProtocol {

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("opacity")]
		float Opacity { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CITemperatureAndTint")]
	interface CITemperatureAndTintProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("neutral", ArgumentSemantic.Retain)]
		CIVector Neutral { get; set; }

		[Abstract]
		[Export ("targetNeutral", ArgumentSemantic.Retain)]
		CIVector TargetNeutral { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CITextImageGenerator")]
	interface CITextImageGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("text", ArgumentSemantic.Retain)]
		string Text { get; set; }

		[Abstract]
		[Export ("fontName", ArgumentSemantic.Retain)]
		string FontName { get; set; }

		[Abstract]
		[Export ("fontSize")]
		float FontSize { get; set; }

		[Abstract]
		[Export ("scaleFactor")]
		float ScaleFactor { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIThermal")]
	interface CIThermalProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIToneCurve")]
	interface CIToneCurveProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("point0", ArgumentSemantic.Assign)]
		CGPoint InputPoint0 { get; set; }

		[Abstract]
		[Export ("point1", ArgumentSemantic.Assign)]
		CGPoint InputPoint1 { get; set; }

		[Abstract]
		[Export ("point2", ArgumentSemantic.Assign)]
		CGPoint InputPoint2 { get; set; }

		[Abstract]
		[Export ("point3", ArgumentSemantic.Assign)]
		CGPoint InputPoint3 { get; set; }

		[Abstract]
		[Export ("point4", ArgumentSemantic.Assign)]
		CGPoint InputPoint4 { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CITriangleKaleidoscope")]
	interface CITriangleKaleidoscopeProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("point", ArgumentSemantic.Assign)]
		CGPoint InputPoint { get; set; }

		[Abstract]
		[Export ("size")]
		float Size { get; set; }

		[Abstract]
		[Export ("rotation")]
		float Rotation { get; set; }

		[Abstract]
		[Export ("decay")]
		float Decay { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CITriangleTile")]
	interface CITriangleTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CITwelvefoldReflectedTile")]
	interface CITwelvefoldReflectedTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIUnsharpMask")]
	interface CIUnsharpMaskProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIVibrance")]
	interface CIVibranceProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("amount")]
		float Amount { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIVignette")]
	interface CIVignetteProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIVignetteEffect")]
	interface CIVignetteEffectProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }

		[Abstract]
		[Export ("falloff")]
		float Falloff { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIWhitePointAdjust")]
	interface CIWhitePointAdjustProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIXRay")]
	interface CIXRayProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[Protocol (Name = "CIZoomBlur")]
	interface CIZoomBlurProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("amount")]
		float Amount { get; set; }
	}
#endregion
}
