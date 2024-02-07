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
#if HAS_OPENGLES
using OpenGLES;
#endif
#if !MONOMAC
using UIKit;
#else
using AppKit;
using ImageKit;
#endif


#if !MONOMAC
using Color = UIKit.UIColor;
using NSImageRep = Foundation.NSObject;
using NSCompositingOperation = Foundation.NSObject;
using IKFilterUIView = Foundation.NSObject;
#else
using Color = AppKit.NSColor;
using UIImage = AppKit.NSImage;
#endif

#if !NET
using NativeHandle = System.IntPtr;
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

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("colorWithRed:green:blue:alpha:colorSpace:")]
		[return: NullAllowed]
		CIColor FromRgba (nfloat red, nfloat green, nfloat blue, nfloat alpha, CGColorSpace colorSpace);

		[Static]
		[Export ("colorWithRed:green:blue:")]
		CIColor FromRgb (nfloat red, nfloat green, nfloat blue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("colorWithRed:green:blue:colorSpace:")]
		[return: NullAllowed]
		CIColor FromRgb (nfloat red, nfloat green, nfloat blue, CGColorSpace colorSpace);

		[Static]
		[Export ("colorWithString:")]
		CIColor FromString (string representation);

		[DesignatedInitializer]
		[Export ("initWithCGColor:")]
		NativeHandle Constructor (CGColor c);

		[MacCatalyst (13, 1)]
		[Export ("initWithRed:green:blue:")]
		NativeHandle Constructor (nfloat red, nfloat green, nfloat blue);

		[MacCatalyst (13, 1)]
		[Export ("initWithRed:green:blue:colorSpace:")]
		NativeHandle Constructor (nfloat red, nfloat green, nfloat blue, CGColorSpace colorSpace);

		[MacCatalyst (13, 1)]
		[Export ("initWithRed:green:blue:alpha:")]
		NativeHandle Constructor (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[MacCatalyst (13, 1)]
		[Export ("initWithRed:green:blue:alpha:colorSpace:")]
		NativeHandle Constructor (nfloat red, nfloat green, nfloat blue, nfloat alpha, CGColorSpace colorSpace);

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

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("blackColor", ArgumentSemantic.Strong)]
		CIColor BlackColor { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("whiteColor", ArgumentSemantic.Strong)]
		CIColor WhiteColor { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("grayColor", ArgumentSemantic.Strong)]
		CIColor GrayColor { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("redColor", ArgumentSemantic.Strong)]
		CIColor RedColor { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("greenColor", ArgumentSemantic.Strong)]
		CIColor GreenColor { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("blueColor", ArgumentSemantic.Strong)]
		CIColor BlueColor { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("cyanColor", ArgumentSemantic.Strong)]
		CIColor CyanColor { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("magentaColor", ArgumentSemantic.Strong)]
		CIColor MagentaColor { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("yellowColor", ArgumentSemantic.Strong)]
		CIColor YellowColor { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("clearColor", ArgumentSemantic.Strong)]
		CIColor ClearColor { get; }

		[Export ("stringRepresentation")]
		string StringRepresentation ();

		[Export ("initWithColor:")]
		NativeHandle Constructor (Color color);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIContext {
		// marked iOS5 but it's not working in iOS8.0
		[MacCatalyst (13, 1)]
		[Export ("init")]
		NativeHandle Constructor ();

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contextWithMTLDevice:")]
		CIContext FromMetalDevice (IMTLDevice device);

		[MacCatalyst (13, 1)]
		[Internal] // This overload is needed for our strong dictionary support (but only for Unified, since for Classic the generic version is transformed to this signature)
		[Static]
		[Export ("contextWithMTLDevice:options:")]
		CIContext FromMetalDevice (IMTLDevice device, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contextWithMTLDevice:options:")]
		CIContext FromMetalDevice (IMTLDevice device, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[MacCatalyst (13, 1)]
		[Internal, Static]
		[Export ("contextWithCGContext:options:")]
		CIContext FromContext (CGContext ctx, [NullAllowed] NSDictionary options);

		[Static, Internal]
		[Export ("contextWithOptions:")]
		CIContext FromOptions ([NullAllowed] NSDictionary dictionary);

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("initWithOptions:")]
		NativeHandle Constructor ([NullAllowed] NSDictionary options);

		[Static]
		[Export ("context")]
		CIContext Create ();

#if HAS_OPENGLES
		[NoMac][NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Static]
		[Export ("contextWithEAGLContext:")]
		CIContext FromContext (EAGLContext eaglContext);

		[NoMac][NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Static]
		[Export ("contextWithEAGLContext:options:")]
		CIContext FromContext (EAGLContext eaglContext, [NullAllowed] NSDictionary dictionary);
#endif

		[MacCatalyst (13, 1)]
		[Export ("render:toCVPixelBuffer:")]
		void Render (CIImage image, CVPixelBuffer buffer);

		[MacCatalyst (13, 1)]
		[Export ("render:toCVPixelBuffer:bounds:colorSpace:")]
		// null is not documented for CGColorSpace but it makes sense with the other overload not having this parameter (unit tested)
		void Render (CIImage image, CVPixelBuffer buffer, CGRect rectangle, [NullAllowed] CGColorSpace cs);

		[MacCatalyst (13, 1)]
		[Export ("render:toIOSurface:bounds:colorSpace:")]
		void Render (CIImage image, IOSurface.IOSurface surface, CGRect bounds, [NullAllowed] CGColorSpace colorSpace);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("inputImageMaximumSize")]
		CGSize InputImageMaximumSize { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("outputImageMaximumSize")]
		CGSize OutputImageMaximumSize { get; }

		[MacCatalyst (13, 1)]
		[Export ("render:toMTLTexture:commandBuffer:bounds:colorSpace:")]
		void Render (CIImage image, IMTLTexture texture, [NullAllowed] IMTLCommandBuffer commandBuffer, CGRect bounds, CGColorSpace colorSpace);

		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'DrawImage (image, CGRect, CGRect)' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'DrawImage (image, CGRect, CGRect)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'DrawImage (image, CGRect, CGRect)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DrawImage (image, CGRect, CGRect)' instead.")]
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

		[MacCatalyst (13, 1)]
		[Export ("createCGImage:fromRect:format:colorSpace:deferred:")]
		[return: Release]
		[return: NullAllowed]
		CGImage CreateCGImage (CIImage image, CGRect fromRect, CIFormat format, [NullAllowed] CGColorSpace colorSpace, bool deferred);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Internal, Export ("createCGLayerWithSize:info:")]
		[return: NullAllowed]
		CGLayer CreateCGLayer (CGSize size, [NullAllowed] NSDictionary info);

		[Export ("render:toBitmap:rowBytes:bounds:format:colorSpace:")]
		void RenderToBitmap (CIImage image, IntPtr bitmapPtr, nint bytesPerRow, CGRect bounds, int /* CIFormat = int */ bitmapFormat, [NullAllowed] CGColorSpace colorSpace);

		//[Export ("render:toIOSurface:bounds:colorSpace:")]
		//void RendertoIOSurfaceboundscolorSpace (CIImage im, IOSurfaceRef surface, CGRect r, CGColorSpaceRef cs, );

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("reclaimResources")]
		void ReclaimResources ();

		[MacCatalyst (13, 1)]
		[Export ("clearCaches")]
		void ClearCaches ();

		[Internal, Field ("kCIContextOutputColorSpace", "+CoreImage")]
		NSString OutputColorSpace { get; }

		[Internal, Field ("kCIContextWorkingColorSpace", "+CoreImage")]
		NSString _WorkingColorSpace { get; }

		[Internal, Field ("kCIContextUseSoftwareRenderer", "+CoreImage")]
		NSString UseSoftwareRenderer { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("kCIContextPriorityRequestLow", "+CoreImage")]
		NSString PriorityRequestLow { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("kCIContextWorkingFormat", "+CoreImage")]
		NSString WorkingFormatField { get; }

		[MacCatalyst (13, 1)]
		[Internal]
		[Field ("kCIContextHighQualityDownsample", "+CoreImage")]
		NSString HighQualityDownsample { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Internal]
		[Field ("kCIContextAllowLowPower")]
		NSString AllowLowPower { get; }

		[iOS (14, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		[Internal]
		[Field ("kCIContextName")]
		NSString Name { get; }

		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Field ("kCIContextMemoryLimit")]
		NSString MemoryLimit { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("offlineGPUCount")]
		[Static]
		int OfflineGPUCount { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Export ("contextForOfflineGPUAtIndex:")]
		[Static]
		[return: NullAllowed]
		CIContext FromOfflineGpu (int gpuIndex);

		[MacCatalyst (13, 1)]
		[Export ("workingColorSpace")]
		[NullAllowed]
		CGColorSpace WorkingColorSpace { get; }

		[MacCatalyst (13, 1)]
		[Export ("workingFormat")]
		CIFormat WorkingFormat { get; }

		[Internal]
		[Field ("kCIContextOutputPremultiplied", "+CoreImage")]
		NSString OutputPremultiplied { get; }

		[MacCatalyst (13, 1)]
		[Internal]
		[Field ("kCIContextCacheIntermediates", "+CoreImage")]
		NSString CacheIntermediates { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contextWithMTLCommandQueue:")]
		CIContext Create (IMTLCommandQueue commandQueue);

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contextWithMTLCommandQueue:options:")]
		CIContext Create (IMTLCommandQueue commandQueue, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("writeOpenEXRRepresentationOfImage:toURL:options:error:")]
		bool WriteOpenExrRepresentation (CIImage image, NSUrl url, NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError errorPtr);

		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("OpenEXRRepresentationOfImage:options:error:")]
		[return: NullAllowed]
		NSData GetOpenEXRRepresentation (CIImage image, NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError errorPtr);
	}

	[Category]
	[BaseType (typeof (CIContext))]
	interface CIContext_ImageRepresentation {

		[MacCatalyst (13, 1)]
		[Export ("TIFFRepresentationOfImage:format:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetTiffRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("GetTiffRepresentation (This, image, format, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetTiffRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		[MacCatalyst (13, 1)]
		[Export ("JPEGRepresentationOfImage:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetJpegRepresentation (CIImage image, CGColorSpace colorSpace, NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("GetJpegRepresentation (This, image, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetJpegRepresentation (CIImage image, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		[MacCatalyst (13, 1)]
		[Export ("HEIFRepresentationOfImage:format:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetHeifRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("GetHeifRepresentation (This, image, format, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetHeifRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("HEIF10RepresentationOfImage:colorSpace:options:error:")]
		[return: NullAllowed]
		NSData GetHeif10Representation (CIImage image, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Wrap ("GetHeif10Representation (This, image, colorSpace, options.GetDictionary ()!, out error)")]
		[return: NullAllowed]
		NSData GetHeif10Representation (CIImage image, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("PNGRepresentationOfImage:format:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetPngRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("GetPngRepresentation (This, image, format, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetPngRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		[MacCatalyst (13, 1)]
		[Export ("writeTIFFRepresentationOfImage:toURL:format:colorSpace:options:error:")]
		bool WriteTiffRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, NSDictionary options, out NSError error);

		[MacCatalyst (13, 1)]
		[Wrap ("WriteTiffRepresentation (This, image, url, format, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteTiffRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("writeJPEGRepresentationOfImage:toURL:colorSpace:options:error:")]
		bool WriteJpegRepresentation (CIImage image, NSUrl url, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Wrap ("WriteJpegRepresentation (This, image, url, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteJpegRepresentation (CIImage image, NSUrl url, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("writeHEIFRepresentationOfImage:toURL:format:colorSpace:options:error:")]
		bool WriteHeifRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Wrap ("WriteHeifRepresentation (This, image, url, format, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteHeifRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("writeHEIF10RepresentationOfImage:toURL:colorSpace:options:error:")]
		bool WriteHeif10Representation (CIImage image, NSUrl url, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Wrap ("WriteHeif10Representation (This, image, url, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteHeif10Representation (CIImage image, NSUrl url, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("writePNGRepresentationOfImage:toURL:format:colorSpace:options:error:")]
		bool WritePngRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Wrap ("WritePngRepresentation (This, image, url, format, colorSpace, options.GetDictionary ()!, out error)")]
		bool WritePngRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);
	}


	[Category]
	[BaseType (typeof (CIContext))]
	interface CIContext_CIDepthBlurEffect {
		// as per the docs: The 'options' parameter is a key value/pair reserved for future use.
		[MacCatalyst (13, 1)]
		[Export ("depthBlurEffectFilterForImageURL:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (NSUrl url, [NullAllowed] NSDictionary options);

		// as per the docs: The 'options' parameter is a key value/pair reserved for future use.
		[MacCatalyst (13, 1)]
		[Export ("depthBlurEffectFilterForImageData:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (NSData data, [NullAllowed] NSDictionary options);

		// as per the docs: The 'options' parameter is a key value/pair reserved for future use.
		[MacCatalyst (13, 1)]
		[Export ("depthBlurEffectFilterForImage:disparityImage:portraitEffectsMatte:orientation:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (CIImage image, CIImage disparityImage, [NullAllowed] CIImage portraitEffectsMatte, CGImagePropertyOrientation orientation, [NullAllowed] NSDictionary options);

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("depthBlurEffectFilterForImage:disparityImage:portraitEffectsMatte:hairSemanticSegmentation:orientation:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (CIImage image, CIImage disparityImage, [NullAllowed] CIImage portraitEffectsMatte, [NullAllowed] CIImage hairSemanticSegmentation, CGImagePropertyOrientation orientation, [NullAllowed] NSDictionary options);

		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[MacCatalyst (14, 1)]
		[Export ("depthBlurEffectFilterForImage:disparityImage:portraitEffectsMatte:hairSemanticSegmentation:glassesMatte:gainMap:orientation:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (CIImage image, CIImage disparityImage, [NullAllowed] CIImage portraitEffectsMatte, [NullAllowed] CIImage hairSemanticSegmentation, [NullAllowed] CIImage glassesMatte, [NullAllowed] CIImage gainMap, CGImagePropertyOrientation orientation, [NullAllowed] NSDictionary options);
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
			[MacCatalyst (13, 1)]
			set;
		}

		[Static]
		[Export ("filterWithName:")]
		[return: NullAllowed]
		CIFilter FromName (string name);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("filterWithName:withInputParameters:")]
		[return: NullAllowed]
		CIFilter GetFilter (string name, [NullAllowed] NSDictionary inputParameters);

		[Static]
		[Export ("filterNamesInCategory:")]
		string [] FilterNamesInCategory ([NullAllowed] string category);

		[Static]
		[Export ("filterNamesInCategories:"), Internal]
		string [] _FilterNamesInCategories ([NullAllowed] string [] categories);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localizedNameForFilterName:")]
		[return: NullAllowed]
		string FilterLocalizedName (string filterName);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localizedNameForCategory:")]
		string CategoryLocalizedName (string category);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localizedDescriptionForFilterName:")]
		[return: NullAllowed]
		string FilterLocalizedDescription (string filterName);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localizedReferenceDocumentationForFilterName:")]
		[return: NullAllowed]
		NSUrl FilterLocalizedReferenceDocumentation (string filterName);

#if MONOMAC && !NET
		[Static]
		[Export ("registerFilterName:constructor:classAttributes:")]
		void RegisterFilterName (string name, NSObject constructorObject, NSDictionary classAttributes);
#else
		[MacCatalyst (13, 1)]
		[NoWatch]
		[Static]
		[Export ("registerFilterName:constructor:classAttributes:")]
#if NET
		void RegisterFilterName (string name, ICIFilterConstructor constructorObject, NSDictionary<NSString, NSObject> classAttributes);
#else
		[Advice ("The 'constructorObject' argument must implement 'ICIFilterConstructor'.")]
		void RegisterFilterName (string name, NSObject constructorObject, NSDictionary<NSString, NSObject> classAttributes);
#endif
#endif

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("apply:arguments:options:")]
		[return: NullAllowed]
		CIImage Apply (CIKernel k, [NullAllowed] NSArray args, [NullAllowed] NSDictionary options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("viewForUIConfiguration:excludedKeys:")]
		IKFilterUIView GetFilterUIView (NSDictionary configurationOptions, [NullAllowed] NSArray excludedKeys);

		// added in 10.10 but it was already accessible in a different way before (manual bindings)
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("outputImage")]
		[NullAllowed]
		CIImage OutputImage { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("serializedXMPFromFilters:inputImageExtent:"), Static]
		[return: NullAllowed]
		NSData SerializedXMP (CIFilter [] filters, CGRect extent);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("filterArrayFromSerializedXMP:inputImageExtent:error:"), Static]
		CIFilter [] FromSerializedXMP (NSData xmpData, CGRect extent, out NSError error);

		[Export ("setValue:forKey:"), Internal]
		void SetValueForKey ([NullAllowed] NSObject value, IntPtr key);

		[Export ("valueForKey:"), Internal]
		[return: NullAllowed]
		NSObject ValueForKey (IntPtr key);

		// CIRAWFilter (CIFilter)

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Export ("filterWithImageURL:options:")]
		CIFilter CreateRawFilter (NSUrl url, NSDictionary options);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Wrap ("CreateRawFilter (url, options.GetDictionary ()!)")]
		CIFilter CreateRawFilter (NSUrl url, CIRawFilterOptions options);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Export ("filterWithImageData:options:")]
		CIFilter CreateRawFilter (NSData data, NSDictionary options);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Wrap ("CreateRawFilter (data, options.GetDictionary ()!)")]
		CIFilter CreateRawFilter (NSData data, CIRawFilterOptions options);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Export ("filterWithCVPixelBuffer:properties:options:")]
		CIFilter CreateRawFilter (CVPixelBuffer pixelBuffer, NSDictionary properties, NSDictionary options);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Wrap ("CreateRawFilter (pixelBuffer, properties, options.GetDictionary ()!)")]
		CIFilter CreateRawFilter (CVPixelBuffer pixelBuffer, NSDictionary properties, CIRawFilterOptions options);

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0), TV (17, 0)]
		[Static]
		[Export ("blurredRectangleGeneratorFilter")]
		CIBlurredRectangleGenerator BlurredRectangleGeneratorFilter { get; }

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0), TV (17, 0)]
		[Static]
		[Export ("cannyEdgeDetectorFilter")]
		CICannyEdgeDetector CannyEdgeDetectorFilter { get; }

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0), TV (17, 0)]
		[Static]
		[Export ("roundedRectangleStrokeGeneratorFilter")]
		CIRoundedRectangleStrokeGenerator RoundedRectangleStrokeGeneratorFilter { get; }

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0), TV (17, 0)]
		[Static]
		[Export ("sobelGradientsFilter")]
		CISobelGradients SobelGradientsFilter { get; }
	}

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0)]
	[NativeName ("CIRAWDecoderVersion")]
	public enum CIRawDecoderVersion : long {

		[Field ("CIRAWDecoderVersionNone")]
		VersionNone,

		[Field ("CIRAWDecoderVersion8")]
		Version8,

		[Field ("CIRAWDecoderVersion8DNG")]
		Version8Dng,

		[Field ("CIRAWDecoderVersion7")]
		Version7,

		[Field ("CIRAWDecoderVersion7DNG")]
		Version7Dng,

		[Field ("CIRAWDecoderVersion6")]
		Version6,

		[Field ("CIRAWDecoderVersion6DNG")]
		Version6Dng,
	}

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0)]
	[BaseType (typeof (CIFilter), Name = "CIRAWFilter")]
	interface CIRawFilter : CIFilterProtocol {
		[Static]
		[Export ("supportedCameraModels")]
		string [] SupportedCameraModels { get; }

		[Export ("supportedDecoderVersions")]
		string [] SupportedDecoderVersions { get; }

		[Export ("nativeSize")]
		CGSize NativeSize { get; }

		[Export ("properties")]
		NSDictionary Properties { get; }

		[Export ("orientation", ArgumentSemantic.Assign)]
		CGImagePropertyOrientation Orientation { get; set; }

		[Export ("draftModeEnabled")]
		bool DraftModeEnabled { [Bind ("isDraftModeEnabled")] get; set; }

		[Export ("decoderVersion", ArgumentSemantic.Retain)]
		string DecoderVersion { get; set; }

		[Export ("scaleFactor")]
		float ScaleFactor { get; set; }

		[Export ("exposure")]
		float Exposure { get; set; }

		[Export ("baselineExposure")]
		float BaselineExposure { get; set; }

		[Export ("shadowBias")]
		float ShadowBias { get; set; }

		[Export ("boostAmount")]
		float BoostAmount { get; set; }

		[Export ("boostShadowAmount")]
		float BoostShadowAmount { get; set; }

		[Export ("gamutMappingEnabled")]
		bool GamutMappingEnabled { [Bind ("isGamutMappingEnabled")] get; set; }

		[Export ("lensCorrectionSupported")]
		bool LensCorrectionSupported { [Bind ("isLensCorrectionSupported")] get; }

		[Export ("lensCorrectionEnabled")]
		bool LensCorrectionEnabled { [Bind ("isLensCorrectionEnabled")] get; set; }

		[Export ("luminanceNoiseReductionSupported")]
		bool LuminanceNoiseReductionSupported { [Bind ("isLuminanceNoiseReductionSupported")] get; }

		[Export ("luminanceNoiseReductionAmount")]
		float LuminanceNoiseReductionAmount { get; set; }

		[Export ("colorNoiseReductionSupported")]
		bool ColorNoiseReductionSupported { [Bind ("isColorNoiseReductionSupported")] get; }

		[Export ("colorNoiseReductionAmount")]
		float ColorNoiseReductionAmount { get; set; }

		[Export ("sharpnessSupported")]
		bool SharpnessSupported { [Bind ("isSharpnessSupported")] get; }

		[Export ("sharpnessAmount")]
		float SharpnessAmount { get; set; }

		[Export ("contrastSupported")]
		bool ContrastSupported { [Bind ("isContrastSupported")] get; }

		[Export ("contrastAmount")]
		float ContrastAmount { get; set; }

		[Export ("detailSupported")]
		bool DetailSupported { [Bind ("isDetailSupported")] get; }

		[Export ("detailAmount")]
		float DetailAmount { get; set; }

		[Export ("moireReductionSupported")]
		bool MoireReductionSupported { [Bind ("isMoireReductionSupported")] get; }

		[Export ("moireReductionAmount")]
		float MoireReductionAmount { get; set; }

		[Export ("localToneMapSupported")]
		bool LocalToneMapSupported { [Bind ("isLocalToneMapSupported")] get; }

		[Export ("localToneMapAmount")]
		float LocalToneMapAmount { get; set; }

		[Export ("extendedDynamicRangeAmount")]
		float ExtendedDynamicRangeAmount { get; set; }

		[Export ("neutralChromaticity", ArgumentSemantic.Assign)]
		CGPoint NeutralChromaticity { get; set; }

		[Export ("neutralLocation", ArgumentSemantic.Assign)]
		CGPoint NeutralLocation { get; set; }

		[Export ("neutralTemperature")]
		float NeutralTemperature { get; set; }

		[Export ("neutralTint")]
		float NeutralTint { get; set; }

		[NullAllowed, Export ("linearSpaceFilter", ArgumentSemantic.Retain)]
		CIFilter LinearSpaceFilter { get; set; }

		[NullAllowed, Export ("previewImage")]
		CIImage PreviewImage { get; }

		[NullAllowed, Export ("portraitEffectsMatte")]
		CIImage PortraitEffectsMatte { get; }

		[NullAllowed, Export ("semanticSegmentationSkinMatte")]
		CIImage SemanticSegmentationSkinMatte { get; }

		[NullAllowed, Export ("semanticSegmentationHairMatte")]
		CIImage SemanticSegmentationHairMatte { get; }

		[NullAllowed, Export ("semanticSegmentationGlassesMatte")]
		CIImage SemanticSegmentationGlassesMatte { get; }

		[NullAllowed, Export ("semanticSegmentationSkyMatte")]
		CIImage SemanticSegmentationSkyMatte { get; }

		[NullAllowed, Export ("semanticSegmentationTeethMatte")]
		CIImage SemanticSegmentationTeethMatte { get; }

		[Static]
		[Export ("filterWithImageURL:")]
		[return: NullAllowed]
		CIRawFilter Create (NSUrl url);

		[Static]
		[Export ("filterWithImageData:identifierHint:")]
		[return: NullAllowed]
		CIRawFilter Create (NSData data, [NullAllowed] string identifierHint);

		[Static]
		[Export ("filterWithCVPixelBuffer:properties:")]
		[return: NullAllowed]
		CIRawFilter Create (CVPixelBuffer buffer, NSDictionary properties);
	}

	[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
	[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[Static]
	[Internal]
	interface CIRawFilterKeys {
		[MacCatalyst (13, 1)]
		[Field ("kCIInputAllowDraftModeKey")]
		NSString AllowDraftModeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputDecoderVersionKey")]
		NSString VersionKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCISupportedDecoderVersionsKey")]
		NSString SupportedDecoderVersionsKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputBaselineExposureKey")]
		NSString BaselineExposureKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputBoostKey")]
		NSString BoostKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputBoostShadowAmountKey")]
		NSString BoostShadowAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputDisableGamutMapKey")]
		NSString DisableGamutMapKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNeutralChromaticityXKey")]
		NSString NeutralChromaticityXKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNeutralChromaticityYKey")]
		NSString NeutralChromaticityYKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNeutralTemperatureKey")]
		NSString NeutralTemperatureKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNeutralTintKey")]
		NSString NeutralTintKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNeutralLocationKey")]
		NSString NeutralLocationKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputScaleFactorKey")]
		NSString ScaleFactorKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputIgnoreImageOrientationKey")]
		NSString IgnoreImageOrientationKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputImageOrientationKey")]
		NSString ImageOrientationKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputEnableSharpeningKey")]
		NSString EnableSharpeningKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputEnableChromaticNoiseTrackingKey")]
		NSString EnableChromaticNoiseTrackingKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNoiseReductionAmountKey")]
		NSString NoiseReductionAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputMoireAmountKey")]
		NSString MoireAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputEnableVendorLensCorrectionKey")]
		NSString EnableVendorLensCorrectionKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputLuminanceNoiseReductionAmountKey")]
		NSString LuminanceNoiseReductionAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputColorNoiseReductionAmountKey")]
		NSString ColorNoiseReductionAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNoiseReductionSharpnessAmountKey")]
		NSString NoiseReductionSharpnessAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNoiseReductionContrastAmountKey")]
		NSString NoiseReductionContrastAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNoiseReductionDetailAmountKey")]
		NSString NoiseReductionDetailAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputLinearSpaceFilter")]
		NSString LinearSpaceFilterKey { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCIInputEnableEDRModeKey")]
		NSString EnableEdrModeKey { get; }

		[iOS (14, 3), TV (14, 3), Mac (11, 1)]
		[MacCatalyst (14, 3)]
		[Field ("kCIInputLocalToneMapAmountKey")]
		NSString InputLocalToneMapAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIOutputNativeSizeKey")]
		NSString OutputNativeSizeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIActiveKeys")]
		NSString ActiveKeysKey { get; }
	}

	[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
	[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[StrongDictionary ("CIRawFilterKeys")]
	interface CIRawFilterOptions {

		[MacCatalyst (13, 1)]
		bool AllowDraftMode { get; set; }

		[MacCatalyst (13, 1)]
		string Version { get; set; }

		[MacCatalyst (13, 1)]
		NSDictionary [] SupportedDecoderVersions { get; set; }

		[MacCatalyst (13, 1)]
		float BaselineExposure { get; set; }

		[MacCatalyst (13, 1)]
		float Boost { get; set; }

		[MacCatalyst (13, 1)]
		float BoostShadowAmount { get; set; }

		[MacCatalyst (13, 1)]
		bool DisableGamutMap { get; set; }

		[MacCatalyst (13, 1)]
		float NeutralChromaticityX { get; set; }

		[MacCatalyst (13, 1)]
		float NeutralChromaticityY { get; set; }

		[MacCatalyst (13, 1)]
		float NeutralTemperature { get; set; }

		[MacCatalyst (13, 1)]
		float NeutralTint { get; set; }

		[MacCatalyst (13, 1)]
		CIVector NeutralLocation { get; set; }

		[MacCatalyst (13, 1)]
		float ScaleFactor { get; set; }

		[MacCatalyst (13, 1)]
		bool IgnoreImageOrientation { get; set; }

		[MacCatalyst (13, 1)]
		int ImageOrientation { get; set; }

		[MacCatalyst (13, 1)]
		bool EnableSharpening { get; set; }

		[MacCatalyst (13, 1)]
		bool EnableChromaticNoiseTracking { get; set; }

		[MacCatalyst (13, 1)]
		double NoiseReductionAmount { get; set; }

		[MacCatalyst (13, 1)]
		bool EnableVendorLensCorrection { get; set; }

		[MacCatalyst (13, 1)]
		double LuminanceNoiseReductionAmount { get; set; }

		[MacCatalyst (13, 1)]
		double ColorNoiseReductionAmount { get; set; }

		[MacCatalyst (13, 1)]
		double NoiseReductionSharpnessAmount { get; set; }

		[MacCatalyst (13, 1)]
		double NoiseReductionContrastAmount { get; set; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNoiseReductionDetailAmountKey")]
		double NoiseReductionDetailAmount { get; set; }

		[MacCatalyst (13, 1)]
		CIFilter LinearSpaceFilter { get; set; }

		[MacCatalyst (13, 1)]
		CIVector OutputNativeSize { get; set; }

		[MacCatalyst (13, 1)]
		NSSet ActiveKeys { get; }
	}

	[Static]
	interface CIFilterOutputKey {
		[Field ("kCIOutputImageKey", "+CoreImage")]
		NSString Image { get; }
	}

	[Static]
	interface CIFilterInputKey {
		[Field ("kCIInputBackgroundImageKey", "+CoreImage")]
		NSString BackgroundImage { get; }

		[Field ("kCIInputImageKey", "+CoreImage")]
		NSString Image { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputVersionKey", "+CoreImage")]
		NSString Version { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputRefractionKey", "+CoreImage")]
		NSString Refraction { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputGradientImageKey", "+CoreImage")]
		NSString GradientImage { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputShadingImageKey", "+CoreImage")]
		NSString ShadingImage { get; }

		[Field ("kCIInputTimeKey", "+CoreImage")]
		NSString Time { get; }

		[Field ("kCIInputTransformKey", "+CoreImage")]
		NSString Transform { get; }

		[Field ("kCIInputScaleKey", "+CoreImage")]
		NSString Scale { get; }

		[Field ("kCIInputAspectRatioKey", "+CoreImage")]
		NSString AspectRatio { get; }

		[Field ("kCIInputCenterKey", "+CoreImage")]
		NSString Center { get; }

		[Field ("kCIInputRadiusKey", "+CoreImage")]
		NSString Radius { get; }

		[Field ("kCIInputAngleKey", "+CoreImage")]
		NSString Angle { get; }

		[Field ("kCIInputWidthKey", "+CoreImage")]
		NSString Width { get; }

		[Field ("kCIInputSharpnessKey", "+CoreImage")]
		NSString Sharpness { get; }

		[Field ("kCIInputIntensityKey", "+CoreImage")]
		NSString Intensity { get; }

		[Field ("kCIInputEVKey", "+CoreImage")]
		NSString EV { get; }

		[Field ("kCIInputSaturationKey", "+CoreImage")]
		NSString Saturation { get; }

		[Field ("kCIInputColorKey", "+CoreImage")]
		NSString Color { get; }

		[Field ("kCIInputBrightnessKey", "+CoreImage")]
		NSString Brightness { get; }

		[Field ("kCIInputContrastKey", "+CoreImage")]
		NSString Contrast { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputBiasKey", "+CoreImage")]
		NSString BiasKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputWeightsKey", "+CoreImage")]
		NSString WeightsKey { get; }

		[Field ("kCIInputMaskImageKey", "+CoreImage")]
		NSString MaskImage { get; }

		[Field ("kCIInputTargetImageKey", "+CoreImage")]
		NSString TargetImage { get; }

		[Field ("kCIInputExtentKey", "+CoreImage")]
		NSString Extent { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputDepthImageKey", "+CoreImage")]
		NSString DepthImage { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputDisparityImageKey", "+CoreImage")]
		NSString DisparityImage { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputMatteImageKey", "+CoreImage")]
		NSString MatteImage { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputAmountKey", "+CoreImage")]
		NSString Amount { get; }
	}

	[Static]
	interface CIFilterAttributes {
		[Field ("kCIAttributeFilterName", "+CoreImage")]
		NSString FilterName { get; }

		[Field ("kCIAttributeFilterDisplayName", "+CoreImage")]
		NSString FilterDisplayName { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeDescription", "+CoreImage")]
		NSString Description { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeReferenceDocumentation", "+CoreImage")]
		NSString ReferenceDocumentation { get; }

		[Field ("kCIAttributeFilterCategories", "+CoreImage")]
		NSString FilterCategories { get; }

		[Field ("kCIAttributeClass", "+CoreImage")]
		NSString Class { get; }

		[Field ("kCIAttributeType", "+CoreImage")]
		NSString Type { get; }

		[Field ("kCIAttributeMin", "+CoreImage")]
		NSString Min { get; }

		[Field ("kCIAttributeMax", "+CoreImage")]
		NSString Max { get; }

		[Field ("kCIAttributeSliderMin", "+CoreImage")]
		NSString SliderMin { get; }

		[Field ("kCIAttributeSliderMax", "+CoreImage")]
		NSString SliderMax { get; }

		[Field ("kCIAttributeDefault", "+CoreImage")]
		NSString Default { get; }

		[Field ("kCIAttributeIdentity", "+CoreImage")]
		NSString Identity { get; }

		[Field ("kCIAttributeName", "+CoreImage")]
		NSString Name { get; }

		[Field ("kCIAttributeDisplayName", "+CoreImage")]
		NSString DisplayName { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIUIParameterSet", "+CoreImage")]
		NSString UIParameterSet { get; }

		[Field ("kCIAttributeTypeTime", "+CoreImage")]
		NSString TypeTime { get; }

		[Field ("kCIAttributeTypeScalar", "+CoreImage")]
		NSString TypeScalar { get; }

		[Field ("kCIAttributeTypeDistance", "+CoreImage")]
		NSString TypeDistance { get; }

		[Field ("kCIAttributeTypeAngle", "+CoreImage")]
		NSString TypeAngle { get; }

		[Field ("kCIAttributeTypeBoolean", "+CoreImage")]
		NSString TypeBoolean { get; }

		[Field ("kCIAttributeTypeInteger", "+CoreImage")]
		NSString TypeInteger { get; }

		[Field ("kCIAttributeTypeCount", "+CoreImage")]
		NSString TypeCount { get; }

		[Field ("kCIAttributeTypePosition", "+CoreImage")]
		NSString TypePosition { get; }

		[Field ("kCIAttributeTypeOffset", "+CoreImage")]
		NSString TypeOffset { get; }

		[Field ("kCIAttributeTypePosition3", "+CoreImage")]
		NSString TypePosition3 { get; }

		[Field ("kCIAttributeTypeRectangle", "+CoreImage")]
		NSString TypeRectangle { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeTypeOpaqueColor", "+CoreImage")]
		NSString TypeOpaqueColor { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeTypeGradient", "+CoreImage")]
		NSString TypeGradient { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeTypeImage", "+CoreImage")]
		NSString TypeImage { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeTypeTransform", "+CoreImage")]
		NSString TypeTransform { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeTypeColor", "+CoreImage")]
		NSString TypeColor { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeFilterAvailable_Mac", "+CoreImage")]
		NSString Available_Mac { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeFilterAvailable_iOS", "+CoreImage")]
		NSString Available_iOS { get; }
	}

	[Static]
	interface CIFilterCategory {
		[Field ("kCICategoryDistortionEffect", "+CoreImage")]
		NSString DistortionEffect { get; }

		[Field ("kCICategoryGeometryAdjustment", "+CoreImage")]
		NSString GeometryAdjustment { get; }

		[Field ("kCICategoryCompositeOperation", "+CoreImage")]
		NSString CompositeOperation { get; }

		[Field ("kCICategoryHalftoneEffect", "+CoreImage")]
		NSString HalftoneEffect { get; }

		[Field ("kCICategoryColorAdjustment", "+CoreImage")]
		NSString ColorAdjustment { get; }

		[Field ("kCICategoryColorEffect", "+CoreImage")]
		NSString ColorEffect { get; }

		[Field ("kCICategoryTransition", "+CoreImage")]
		NSString Transition { get; }

		[Field ("kCICategoryTileEffect", "+CoreImage")]
		NSString TileEffect { get; }

		[Field ("kCICategoryGenerator", "+CoreImage")]
		NSString Generator { get; }

		[Field ("kCICategoryReduction", "+CoreImage")]
		NSString Reduction { get; }

		[Field ("kCICategoryGradient", "+CoreImage")]
		NSString Gradient { get; }

		[Field ("kCICategoryStylize", "+CoreImage")]
		NSString Stylize { get; }

		[Field ("kCICategorySharpen", "+CoreImage")]
		NSString Sharpen { get; }

		[Field ("kCICategoryBlur", "+CoreImage")]
		NSString Blur { get; }

		[Field ("kCICategoryVideo", "+CoreImage")]
		NSString Video { get; }

		[Field ("kCICategoryStillImage", "+CoreImage")]
		NSString StillImage { get; }

		[Field ("kCICategoryInterlaced", "+CoreImage")]
		NSString Interlaced { get; }

		[Field ("kCICategoryNonSquarePixels", "+CoreImage")]
		NSString NonSquarePixels { get; }

		[Field ("kCICategoryHighDynamicRange", "+CoreImage")]
		NSString HighDynamicRange { get; }

		[Field ("kCICategoryBuiltIn", "+CoreImage")]
		NSString BuiltIn { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCICategoryFilterGenerator", "+CoreImage")]
		NSString FilterGenerator { get; }
	}

	[MacCatalyst (13, 1)]
	[Protocol]
	interface CIFilterConstructor {
		[Abstract]
		[Export ("filterWithName:")]
		[return: NullAllowed]
		CIFilter FilterWithName (string name);
	}

	interface ICIFilterConstructor { }

	[Static]
	[MacCatalyst (13, 1)]
	interface CIUIParameterSet {
		[Field ("kCIUISetBasic", "+CoreImage")]
		NSString Basic { get; }

		[Field ("kCIUISetIntermediate", "+CoreImage")]
		NSString Intermediate { get; }

		[Field ("kCIUISetAdvanced", "+CoreImage")]
		NSString Advanced { get; }

		[Field ("kCIUISetDevelopment", "+CoreImage")]
		NSString Development { get; }
	}

	[NoiOS]
	[NoMacCatalyst]
	[NoWatch]
	[NoTV]
	[Static]
	interface CIFilterApply {
		[Field ("kCIApplyOptionExtent", "+CoreImage")]
		NSString OptionExtent { get; }

		[Field ("kCIApplyOptionDefinition", "+CoreImage")]
		NSString OptionDefinition { get; }

		[Field ("kCIApplyOptionUserInfo", "+CoreImage")]
		NSString OptionUserInfo { get; }

		[Field ("kCIApplyOptionColorSpace", "+CoreImage")]
		NSString OptionColorSpace { get; }
	}

	[iOS (17, 0)]
	[MacCatalyst (17, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIFilterGenerator : CIFilterConstructor, NSSecureCoding, NSCopying {
		[Static, Export ("filterGenerator")]
		CIFilterGenerator Create ();

		[Static]
		[Export ("filterGeneratorWithContentsOfURL:")]
		[return: NullAllowed]
		CIFilterGenerator FromUrl (NSUrl aURL);

		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor (NSUrl aURL);

		[Export ("connectObject:withKey:toObject:withKey:")]
		void ConnectObject (NSObject sourceObject, [NullAllowed] string withSourceKey, NSObject targetObject, string targetKey);

		[Export ("disconnectObject:withKey:toObject:withKey:")]
		void DisconnectObject (NSObject sourceObject, string sourceKey, NSObject targetObject, string targetKey);

		[Export ("exportKey:fromObject:withName:")]
		void ExportKey (string key, NSObject targetObject, [NullAllowed] string exportedKeyName);

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

		[NoiOS, NoMacCatalyst]
		[Field ("kCIFilterGeneratorExportedKey", "+CoreImage")]
		NSString ExportedKey { get; }

		[NoiOS, NoMacCatalyst]
		[Field ("kCIFilterGeneratorExportedKeyTargetObject", "+CoreImage")]
		NSString ExportedKeyTargetObject { get; }

		[NoiOS, NoMacCatalyst]
		[Field ("kCIFilterGeneratorExportedKeyName", "+CoreImage")]
		NSString ExportedKeyName { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[MacCatalyst (13, 1)]
	interface CIFilterShape : NSCopying {
		[Static]
		[Export ("shapeWithRect:")]
		CIFilterShape FromRect (CGRect rect);

		[Export ("initWithRect:")]
		NativeHandle Constructor (CGRect rect);

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

		[MacCatalyst (13, 1)]
		bool ApplyOrientationProperty { get; set; }

		[MacCatalyst (13, 1)]
		bool NearestSampling { get; set; }

		[MacCatalyst (13, 1)]
		bool AuxiliaryDepth { get; set; }

		[MacCatalyst (13, 1)]
		bool AuxiliaryDisparity { get; set; }

		[MacCatalyst (13, 1)]
		bool AuxiliaryPortraitEffectsMatte { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		bool AuxiliarySemanticSegmentationSkinMatte { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		bool AuxiliarySemanticSegmentationHairMatte { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		bool AuxiliarySemanticSegmentationTeethMatte { get; set; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[MacCatalyst (14, 1)]
		bool AuxiliarySemanticSegmentationGlassesMatte { get; set; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[MacCatalyst (14, 1)]
		bool ToneMapHdrToSdr { get; set; }
	}

	[Internal]
	[Static]
	interface CIImageInitializationOptionsKeys {
		[Field ("kCIImageColorSpace")]
		NSString ColorSpaceKey { get; }

		[Field ("kCIImageProperties")]
		NSString PropertiesKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageNearestSampling")]
		NSString NearestSamplingKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageApplyOrientationProperty")]
		NSString ApplyOrientationPropertyKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliaryDepth")]
		NSString AuxiliaryDepthKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliaryDisparity")]
		NSString AuxiliaryDisparityKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliaryPortraitEffectsMatte")]
		NSString AuxiliaryPortraitEffectsMatteKey { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliarySemanticSegmentationSkinMatte")]
		NSString AuxiliarySemanticSegmentationSkinMatteKey { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliarySemanticSegmentationHairMatte")]
		NSString AuxiliarySemanticSegmentationHairMatteKey { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliarySemanticSegmentationTeethMatte")]
		NSString AuxiliarySemanticSegmentationTeethMatteKey { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[MacCatalyst (14, 1)]
		[Field ("kCIImageAuxiliarySemanticSegmentationGlassesMatte")]
		NSString AuxiliarySemanticSegmentationGlassesMatteKey { get; }

		[iOS (14, 3), TV (14, 3), Mac (11, 1)]
		[MacCatalyst (14, 3)]
		[Field ("kCIImageAuxiliarySemanticSegmentationSkyMatte")]
		NSString AuxiliarySemanticSegmentationSkyMatteKey { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[MacCatalyst (14, 1)]
		[Field ("kCIImageToneMapHDRtoSDR")]
		NSString ToneMapHdrToSdrKey { get; }

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

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithCGImageSource:index:options:")]
		CIImage FromCGImageSource (CGImageSource source, nuint index, [NullAllowed] NSDictionary options);

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("FromCGImageSource (source, index, options.GetDictionary ())")]
		CIImage FromCGImageSource (CGImageSource source, nuint index, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Static]
		[Export ("imageWithCGLayer:")]
		CIImage FromLayer (CGLayer layer);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Static]
		[Export ("imageWithCGLayer:options:")]
		CIImage FromLayer (CGLayer layer, [NullAllowed] NSDictionary options);

		[Static]
		[Export ("imageWithBitmapData:bytesPerRow:size:format:colorSpace:")]
		[Internal] // there's a CIFormat enum that maps to the kCIFormatARGB8, kCIFormatRGBA16, kCIFormatRGBAf, kCIFormatRGBAh constants
		CIImage FromData (NSData bitmapData, nint bytesPerRow, CGSize size, int /* CIFormat = int */ pixelFormat, [NullAllowed] CGColorSpace colorSpace);

		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("imageWithTexture:size:flipped:colorSpace:")]
		CIImage ImageWithTexture (uint /* unsigned int */ glTextureName, CGSize size, bool flipped, [NullAllowed] CGColorSpace colorspace);

		[Static]
		[Export ("imageWithContentsOfURL:")]
		[return: NullAllowed]
		CIImage FromUrl (NSUrl url);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithContentsOfURL:options:")]
		[return: NullAllowed]
		CIImage FromUrl (NSUrl url, [NullAllowed] NSDictionary d);

		[Static]
		[Wrap ("FromUrl (url, options.GetDictionary ())")]
		[return: NullAllowed]
		CIImage FromUrl (NSUrl url, [NullAllowed] CIImageInitializationOptions options);

		[Static]
		[Export ("imageWithData:")]
		[return: NullAllowed]
		CIImage FromData (NSData data);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithData:options:")]
		[return: NullAllowed]
		CIImage FromData (NSData data, [NullAllowed] NSDictionary d);

		[Static]
		[Wrap ("FromData (data, options.GetDictionary ())")]
		[return: NullAllowed]
		CIImage FromData (NSData data, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		[Static]
		[MacCatalyst (13, 1)]
		[Export ("imageWithCVImageBuffer:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer);

#if MONOMAC && !NET
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithCVImageBuffer:options:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary dict);
#else
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[MacCatalyst (13, 1)]
		[Internal] // This overload is needed for our strong dictionary support (but only for Unified, since for Classic the generic version is transformed to this signature)
		[Export ("imageWithCVImageBuffer:options:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary dict);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[MacCatalyst (13, 1)]
		[Export ("imageWithCVImageBuffer:options:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary<NSString, NSObject> dict);
#endif

		[Static]
		[MacCatalyst (13, 1)]
		[Wrap ("FromImageBuffer (imageBuffer, options.GetDictionary ())")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, CIImageInitializationOptions options);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithCVPixelBuffer:")]
		CIImage FromImageBuffer (CVPixelBuffer buffer);

		[NoMac]
		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithCVPixelBuffer:options:")]
		CIImage FromImageBuffer (CVPixelBuffer buffer, [NullAllowed] NSDictionary dict);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("FromImageBuffer (buffer, options.GetDictionary ())")]
		CIImage FromImageBuffer (CVPixelBuffer buffer, [NullAllowed] CIImageInitializationOptions options);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithIOSurface:")]
		CIImage FromSurface (IOSurface.IOSurface surface);

		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithIOSurface:options:")]
		CIImage FromSurface (IOSurface.IOSurface surface, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("FromSurface (surface, options.GetDictionary ())")]
		CIImage FromSurface (IOSurface.IOSurface surface, CIImageInitializationOptions options);

		[Static]
		[Export ("imageWithColor:")]
		CIImage ImageWithColor (CIColor color);

		[Static]
		[Export ("emptyImage")]
		CIImage EmptyImage { get; }

		[Export ("initWithCGImage:")]
		NativeHandle Constructor (CGImage image);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithCGImage:options:")]
		NativeHandle Constructor (CGImage image, [NullAllowed] NSDictionary d);

		[Wrap ("this (image, options.GetDictionary ())")]
		NativeHandle Constructor (CGImage image, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithCGImageSource:index:options:")]
		NativeHandle Constructor (CGImageSource source, nuint index, [NullAllowed] NSDictionary options);

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Wrap ("this (source, index, options.GetDictionary ())")]
		NativeHandle Constructor (CGImageSource source, nuint index, CIImageInitializationOptionsWithMetadata options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CIImage (CGImage)' instead.")]
		[Export ("initWithCGLayer:")]
		NativeHandle Constructor (CGLayer layer);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CIImage (CGImage)' instead.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithCGLayer:options:")]
		NativeHandle Constructor (CGLayer layer, [NullAllowed] NSDictionary d);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Wrap ("this (layer, options.GetDictionary ())")]
		NativeHandle Constructor (CGLayer layer, [NullAllowed] CIImageInitializationOptions options);

		[Export ("initWithData:")]
		NativeHandle Constructor (NSData data);

		[Export ("initWithData:options:")]
		NativeHandle Constructor (NSData data, [NullAllowed] NSDictionary d);

		[Wrap ("this (data, options.GetDictionary ())")]
		NativeHandle Constructor (NSData data, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		[Export ("initWithBitmapData:bytesPerRow:size:format:colorSpace:")]
		NativeHandle Constructor (NSData d, nint bytesPerRow, CGSize size, int /* CIFormat = int */ pixelFormat, [NullAllowed] CGColorSpace colorSpace);

		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.TvOS, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("initWithTexture:size:flipped:colorSpace:")]
		NativeHandle Constructor (int /* unsigned int */ glTextureName, CGSize size, bool flipped, [NullAllowed] CGColorSpace colorSpace);

		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor (NSUrl url);

		[Export ("initWithContentsOfURL:options:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] NSDictionary d);

		[Wrap ("this (url, options.GetDictionary ())")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] CIImageInitializationOptions options);

		[MacCatalyst (13, 1)]
		[Export ("initWithIOSurface:")]
		NativeHandle Constructor (IOSurface.IOSurface surface);

		[MacCatalyst (13, 1)]
		[Export ("initWithIOSurface:options:")]
		NativeHandle Constructor (IOSurface.IOSurface surface, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("this (surface, options.GetDictionary ())")]
		NativeHandle Constructor (IOSurface.IOSurface surface, [NullAllowed] CIImageInitializationOptions options);

		[MacCatalyst (13, 1)]
		[Export ("initWithCVImageBuffer:")]
		NativeHandle Constructor (CVImageBuffer imageBuffer);

#if MONOMAC && !NET
		[Export ("initWithCVImageBuffer:options:")]
		NativeHandle Constructor (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary dict);
#else
		[MacCatalyst (13, 1)]
		[Export ("initWithCVImageBuffer:options:")]
		NativeHandle Constructor (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary<NSString, NSObject> dict);

		[MacCatalyst (13, 1)]
		[Internal] // This overload is needed for our strong dictionary support (but only for Unified, since for Classic the generic version is transformed to this signature)
		[Sealed]
		[Export ("initWithCVImageBuffer:options:")]
		NativeHandle Constructor (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary dict);
#endif

		[MacCatalyst (13, 1)]
		[Wrap ("this (imageBuffer, options.GetDictionary ())")]
		NativeHandle Constructor (CVImageBuffer imageBuffer, [NullAllowed] CIImageInitializationOptions options);

		[MacCatalyst (13, 1)]
		[Export ("initWithCVPixelBuffer:")]
		NativeHandle Constructor (CVPixelBuffer buffer);

		[MacCatalyst (13, 1)]
		[Export ("initWithCVPixelBuffer:options:")]
		NativeHandle Constructor (CVPixelBuffer buffer, [NullAllowed] NSDictionary dict);

		[MacCatalyst (13, 1)]
		[Wrap ("this (buffer, options.GetDictionary ())")]
		NativeHandle Constructor (CVPixelBuffer buffer, [NullAllowed] CIImageInitializationOptions options);

		[Export ("initWithColor:")]
		NativeHandle Constructor (CIColor color);

		[MacCatalyst (13, 1)]
		[Export ("initWithMTLTexture:options:")]
		NativeHandle Constructor (IMTLTexture texture, [NullAllowed] NSDictionary options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("initWithBitmapImageRep:")]
		NativeHandle Constructor (NSImageRep imageRep);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("drawAtPoint:fromRect:operation:fraction:")]
		void Draw (CGPoint point, CGRect srcRect, NSCompositingOperation op, nfloat delta);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("drawInRect:fromRect:operation:fraction:")]
		void Draw (CGRect dstRect, CGRect srcRect, NSCompositingOperation op, nfloat delta);

		[Export ("imageByApplyingTransform:")]
		CIImage ImageByApplyingTransform (CGAffineTransform matrix);

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
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

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("definition")]
		CIFilterShape Definition { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIFormatRGBA16")]
		int FormatRGBA16 { get; } /* CIFormat = int */

		[Field ("kCIFormatARGB8")]
		int FormatARGB8 { get; } /* CIFormat = int */

		[Field ("kCIFormatRGBAh")]
		int FormatRGBAh { get; } /* CIFormat = int */

		[MacCatalyst (13, 1)]
		[Field ("kCIFormatRGBAf")]
		int FormatRGBAf { get; } /* CIFormat = int */

		[Field ("kCIFormatBGRA8")]
		int FormatBGRA8 { get; } /* CIFormat = int */

		[Field ("kCIFormatRGBA8")]
		int FormatRGBA8 { get; } /* CIFormat = int */

		[Field ("kCIFormatABGR8")]
		[MacCatalyst (13, 1)]
		int FormatABGR8 { get; }

		[Field ("kCIFormatA8")]
		[MacCatalyst (13, 1)]
		int FormatA8 { get; }

		[Field ("kCIFormatA16")]
		[MacCatalyst (13, 1)]
		int FormatA16 { get; }

		[Field ("kCIFormatAh")]
		[MacCatalyst (13, 1)]
		int FormatAh { get; }

		[Field ("kCIFormatAf")]
		[MacCatalyst (13, 1)]
		int FormatAf { get; }

		[Field ("kCIFormatR8")]
		[MacCatalyst (13, 1)]
		int FormatR8 { get; }

		[Field ("kCIFormatR16")]
		[MacCatalyst (13, 1)]
		int FormatR16 { get; }

		[Field ("kCIFormatRh")]
		[MacCatalyst (13, 1)]
		int FormatRh { get; }

		[Field ("kCIFormatRf")]
		[MacCatalyst (13, 1)]
		int FormatRf { get; }

		[Field ("kCIFormatRG8")]
		[MacCatalyst (13, 1)]
		int FormatRG8 { get; }

		[Field ("kCIFormatRG16")]
		[MacCatalyst (13, 1)]
		int FormatRG16 { get; }

		[Field ("kCIFormatRGh")]
		[MacCatalyst (13, 1)]
		int FormatRGh { get; }

		[Field ("kCIFormatRGf")]
		[MacCatalyst (13, 1)]
		int FormatRGf { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIFormatL8")]
		int FormatL8 { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIFormatL16")]
		int FormatL16 { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLh")]
		int FormatLh { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLf")]
		int FormatLf { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLA8")]
		int FormatLA8 { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLA16")]
		int FormatLA16 { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLAh")]
		int FormatLAh { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLAf")]
		int FormatLAf { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kCIFormatRGB10")]
		int FormatRgb10 { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kCIFormatRGBX16")]
		int FormatRgbX16 { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kCIFormatRGBXf")]
		int FormatRgbXf { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kCIFormatRGBXh")]
		int FormatRgbXh { get; }

		// UIKit extensions
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("initWithImage:")]
		NativeHandle Constructor (UIImage image);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("initWithImage:options:")]
		NativeHandle Constructor (UIImage image, [NullAllowed] NSDictionary options);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("this (image, options.GetDictionary ())")]
		NativeHandle Constructor (UIImage image, [NullAllowed] CIImageInitializationOptions options);

		[Field ("kCIImageAutoAdjustFeatures"), Internal]
		NSString AutoAdjustFeaturesKey { get; }

		[Field ("kCIImageAutoAdjustRedEye"), Internal]
		NSString AutoAdjustRedEyeKey { get; }

		[Field ("kCIImageAutoAdjustEnhance"), Internal]
		NSString AutoAdjustEnhanceKey { get; }

		[Export ("autoAdjustmentFiltersWithOptions:"), Internal]
		NSArray _GetAutoAdjustmentFilters ([NullAllowed] NSDictionary opts);

		[MacCatalyst (13, 1)]
		[Export ("regionOfInterestForImage:inRect:")]
		CGRect GetRegionOfInterest (CIImage im, CGRect r);

		//
		// iOS 8.0
		//
		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingOrientation:")]
		CIImage CreateWithOrientation (CIImageOrientation orientation);

		[MacCatalyst (13, 1)]
		[Export ("imageTransformForOrientation:")]
		CGAffineTransform GetImageTransform (CIImageOrientation orientation);

		[MacCatalyst (13, 1)]
		[Export ("imageByClampingToExtent")]
		CIImage CreateByClampingToExtent ();

		[MacCatalyst (13, 1)]
		[Export ("imageByCompositingOverImage:")]
		CIImage CreateByCompositingOverImage (CIImage dest);

		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingFilter:withInputParameters:")]
		CIImage CreateByFiltering (string filterName, [NullAllowed] NSDictionary inputParameters);

		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingFilter:")]
		CIImage CreateByFiltering (string filterName);

		[MacCatalyst (13, 1)]
		[Export ("imageBySamplingLinear")]
		CIImage CreateBySamplingLinear ();

		[MacCatalyst (13, 1)]
		[Export ("imageBySamplingNearest")]
		CIImage CreateBySamplingNearest ();

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAutoAdjustCrop"), Internal]
		NSString AutoAdjustCrop { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAutoAdjustLevel"), Internal]
		NSString AutoAdjustLevel { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("url")]
		NSUrl Url { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("colorSpace")]
		CGColorSpace ColorSpace { get; }

		[MacCatalyst (13, 1)]
		[Static, Internal]
		[Export ("imageWithImageProvider:size::format:colorSpace:options:")]
		CIImage FromProvider (ICIImageProvider provider, nuint width, nuint height, int format, [NullAllowed] CGColorSpace colorSpace, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("initWithImageProvider:size::format:colorSpace:options:")]
		NativeHandle Constructor (ICIImageProvider provider, nuint width, nuint height, int f, [NullAllowed] CGColorSpace colorSpace, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithMTLTexture:options:")]
		[return: NullAllowed]
		CIImage FromMetalTexture (IMTLTexture texture, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[MacCatalyst (13, 1)]
		[Export ("imageByClampingToRect:")]
		CIImage CreateByClamping (CGRect rect);

		[MacCatalyst (13, 1)]
		[Export ("imageByColorMatchingColorSpaceToWorkingSpace:")]
		[return: NullAllowed]
		CIImage CreateByColorMatchingColorSpaceToWorkingSpace (CGColorSpace colorSpace);

		[MacCatalyst (13, 1)]
		[Export ("imageByColorMatchingWorkingSpaceToColorSpace:")]
		[return: NullAllowed]
		CIImage CreateByColorMatchingWorkingSpaceToColorSpace (CGColorSpace colorSpace);

		[MacCatalyst (13, 1)]
		[Export ("imageByPremultiplyingAlpha")]
		CIImage CreateByPremultiplyingAlpha ();

		[MacCatalyst (13, 1)]
		[Export ("imageByUnpremultiplyingAlpha")]
		CIImage CreateByUnpremultiplyingAlpha ();

		[MacCatalyst (13, 1)]
		[Export ("imageBySettingAlphaOneInExtent:")]
		CIImage CreateBySettingAlphaOne (CGRect extent);

		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingGaussianBlurWithSigma:")]
		CIImage CreateByApplyingGaussianBlur (double sigma);

		[MacCatalyst (13, 1)]
		[Export ("imageBySettingProperties:")]
		CIImage CreateBySettingProperties (NSDictionary properties);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("CGImage")]
		CGImage CGImage { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("depthData")]
		AVDepthData DepthData { get; }

		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingCGOrientation:")]
		CIImage CreateByApplyingOrientation (CGImagePropertyOrientation orientation);

		[MacCatalyst (13, 1)]
		[Export ("imageTransformForCGOrientation:")]
		CGAffineTransform GetImageTransform (CGImagePropertyOrientation orientation);

		[MacCatalyst (13, 1)]
		[Export ("imageByInsertingIntermediate")]
		CIImage CreateByInsertingIntermediate ();

		[MacCatalyst (13, 1)]
		[Export ("imageByInsertingIntermediate:")]
		CIImage CreateByInsertingIntermediate (bool cache);

		// CIImage_AVPortraitEffectsMatte category

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("portraitEffectsMatte")]
		AVPortraitEffectsMatte PortraitEffectsMatte { get; }

		[MacCatalyst (13, 1)]
		[Export ("initWithPortaitEffectsMatte:options:")] // selector typo, rdar filled 42894821
		NativeHandle Constructor (AVPortraitEffectsMatte matte, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Export ("initWithPortaitEffectsMatte:")] // selector typo, rdar filled 42894821
		NativeHandle Constructor (AVPortraitEffectsMatte matte);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithPortaitEffectsMatte:options:")] // selector typo, rdar filled 42894821
		[return: NullAllowed]
		CIImage FromPortraitEffectsMatte (AVPortraitEffectsMatte matte, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithPortaitEffectsMatte:")] // selector typo, rdar filled 42894821
		[return: NullAllowed]
		CIImage FromPortraitEffectsMatte (AVPortraitEffectsMatte matte);

		// CIImage_AVSemanticSegmentationMatte

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("semanticSegmentationMatte")]
		AVSemanticSegmentationMatte SemanticSegmentationMatte { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithSemanticSegmentationMatte:options:")]
		NativeHandle Constructor (AVSemanticSegmentationMatte matte, [NullAllowed] NSDictionary options);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithSemanticSegmentationMatte:")]
		NativeHandle Constructor (AVSemanticSegmentationMatte matte);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithSemanticSegmentationMatte:options:")]
		[return: NullAllowed]
		CIImage FromSemanticSegmentationMatte (AVSemanticSegmentationMatte matte, [NullAllowed] NSDictionary options);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithSemanticSegmentationMatte:")]
		[return: NullAllowed]
		CIImage FromSemanticSegmentationMatte (AVSemanticSegmentationMatte matte);

		// CIImage_AVDepthData category

		[MacCatalyst (13, 1)]
		[Export ("initWithDepthData:options:")]
		NativeHandle Constructor (AVDepthData data, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Export ("initWithDepthData:")]
		NativeHandle Constructor (AVDepthData data);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithDepthData:options:")]
		[return: NullAllowed]
		CIImage FromDepthData (AVDepthData data, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithDepthData:")]
		[return: NullAllowed]
		CIImage FromDepthData (AVDepthData data);

		// colors

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("blackImage", ArgumentSemantic.Strong)]
		CIImage BlackImage { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("whiteImage", ArgumentSemantic.Strong)]
		CIImage WhiteImage { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("grayImage", ArgumentSemantic.Strong)]
		CIImage GrayImage { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("redImage", ArgumentSemantic.Strong)]
		CIImage RedImage { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("greenImage", ArgumentSemantic.Strong)]
		CIImage GreenImage { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("blueImage", ArgumentSemantic.Strong)]
		CIImage BlueImage { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("cyanImage", ArgumentSemantic.Strong)]
		CIImage CyanImage { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("magentaImage", ArgumentSemantic.Strong)]
		CIImage MagentaImage { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("yellowImage", ArgumentSemantic.Strong)]
		CIImage YellowImage { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("clearImage", ArgumentSemantic.Strong)]
		CIImage ClearImage { get; }

		[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("imageByConvertingWorkingSpaceToLab")]
		CIImage ConvertWorkingSpaceToLab ();

		[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("imageByConvertingLabToWorkingSpace")]
		CIImage ConvertLabToWorkingSpace ();
	}

	interface ICIImageProcessorInput { }

	[MacCatalyst (13, 1)]
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

#if NET
		[Abstract] // @required but it was added in Xcode9
#endif
		[MacCatalyst (13, 1)]
		[Export ("surface")]
		IOSurface.IOSurface Surface { get; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("digest")]
		ulong Digest { get; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("roiTileCount")]
		nuint RoiTileCount { get; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("roiTileIndex")]
		nuint RoiTileIndex { get; }
	}

	interface ICIImageProcessorOutput { }

	[MacCatalyst (13, 1)]
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

#if NET
		[Abstract] // @required but it was added in Xcode9
#endif
		[MacCatalyst (13, 1)]
		[Export ("surface")]
		IOSurface.IOSurface Surface { get; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("digest")]
		ulong Digest { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("CIImageProviderKeys")]
	interface CIImageProviderOptions {
		NSObject TileSize { get; set; }
		NSObject UserInfo { get; set; }
	}

	[Internal]
	[Static]
	[MacCatalyst (13, 1)]
	interface CIImageProviderKeys {
		[Field ("kCIImageProviderTileSize")]
		NSString TileSizeKey { get; }

		[Field ("kCIImageProviderUserInfo")]
		NSString UserInfoKey { get; }
	}

	interface ICIImageProvider { }

	// Informal protocol
	[Protocol (IsInformal = true)]
	interface CIImageProvider {
		[Abstract]
		[Export ("provideImageData:bytesPerRow:origin::size::userInfo:")]
		unsafe void ProvideImageData (IntPtr data, nuint rowbytes, nuint x, nuint y, nuint width, nuint height, [NullAllowed] NSObject info);
	}

	delegate CGRect CIKernelRoiCallback (int /* int, not NSInteger */ index, CGRect rect);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // avoid crashes
	interface CIKernel {

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Static]
		[Export ("kernelsWithMetalString:error:")]
		[return: NullAllowed]
		CIKernel [] FromMetalSource (string source, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static, Export ("kernelsWithString:")]
		[return: NullAllowed]
		CIKernel [] FromProgramMultiple (string coreImageShaderProgram);

		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static, Export ("kernelWithString:")]
		[return: NullAllowed]
		CIKernel FromProgramSingle (string coreImageShaderProgram);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("kernelWithFunctionName:fromMetalLibraryData:error:")]
		[return: NullAllowed]
		CIKernel FromFunction (string name, NSData data, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("kernelWithFunctionName:fromMetalLibraryData:outputPixelFormat:error:")]
		[return: NullAllowed]
		CIKernel FromFunction (string name, NSData data, CIFormat format, [NullAllowed] out NSError error);

		[iOS (14, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("kernelNamesFromMetalLibraryData:")]
		string [] GetKernelNamesFromMetalLibrary (NSData data);

		[Export ("name")]
		string Name { get; }

		[NoiOS]
		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setROISelector:")]
		void SetRegionOfInterestSelector (Selector aMethod);

		[MacCatalyst (13, 1)]
		[Export ("applyWithExtent:roiCallback:arguments:")]
		[return: NullAllowed]
		CIImage ApplyWithExtent (CGRect extent, CIKernelRoiCallback callback, [NullAllowed] NSObject [] args);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIKernel))]
	[DisableDefaultCtor] // returns a nil handle -> instances of this type are returned from `kernel[s]WithString:`
	interface CIColorKernel {
		[Export ("applyWithExtent:arguments:")]
		[return: NullAllowed]
		CIImage ApplyWithExtent (CGRect extent, [NullAllowed] NSObject [] args);

		// Note: the API is supported in iOS 8, but with iOS 9, they guarantee
		// a more derived result
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[New, Static, Export ("kernelWithString:")]
		[return: NullAllowed]
		CIColorKernel FromProgramSingle (string coreImageShaderProgram);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIKernel))]
	[DisableDefaultCtor] // returns a nil handle -> instances of this type are returned from `kernel[s]WithString:`
	interface CIWarpKernel {
		[Export ("applyWithExtent:roiCallback:inputImage:arguments:")]
		[return: NullAllowed]
		CIImage ApplyWithExtent (CGRect extent, CIKernelRoiCallback callback, CIImage image, [NullAllowed] NSObject [] args);

		// Note: the API is supported in iOS 8, but with iOS 9, they guarantee
		// a more derived result
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[New, Static, Export ("kernelWithString:")]
		[return: NullAllowed]
		CIWarpKernel FromProgramSingle (string coreImageShaderProgram);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // does not work in iOS 11 beta 4
	interface CIImageAccumulator {
#if !NET
		[Obsolete ("The default initializer does not work in recent iOS version (11b4).")]
		[Export ("init")]
		NativeHandle Constructor ();
#endif

		[Static]
		[Export ("imageAccumulatorWithExtent:format:")]
		[return: NullAllowed]
		CIImageAccumulator FromRectangle (CGRect rect, CIFormat format);

#if MONOMAC && !NET
		[Obsolete ("Use the overload acceping a 'CIFormat' enum instead of an 'int'.")]
		[Static]
		[Wrap ("FromRectangle (rect, (CIFormat) ciImageFormat)")]
		[return: NullAllowed]
		CIImageAccumulator FromRectangle (CGRect rect, int ciImageFormat);
#endif

		[Static]
		[Export ("imageAccumulatorWithExtent:format:colorSpace:")]
		[return: NullAllowed]
		CIImageAccumulator FromRectangle (CGRect extent, CIFormat format, CGColorSpace colorSpace);

		[Export ("initWithExtent:format:")]
		NativeHandle Constructor (CGRect rectangle, CIFormat format);

#if MONOMAC && !NET
		[Obsolete ("Use the overload acceping a 'CIFormat' enum instead of an 'int'.")]
		[Wrap ("this (rectangle, (CIFormat) ciImageFormat)")]
		NativeHandle Constructor (CGRect rectangle, int ciImageFormat);
#endif

		[Export ("initWithExtent:format:colorSpace:")]
		NativeHandle Constructor (CGRect extent, CIFormat format, CGColorSpace colorSpace);

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

	[NoiOS]
	[NoMacCatalyst]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface CIPlugIn {
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'LoadNonExecutablePlugIns' for non-executable plugins instead.")]
		[Static]
		[Export ("loadAllPlugIns")]
		void LoadAllPlugIns ();

		[Static]
		[Export ("loadNonExecutablePlugIns")]
		void LoadNonExecutablePlugIns ();

		[Static]
		[Export ("loadNonExecutablePlugIn:")]
		void LoadNonExecutablePlugIn (NSUrl url);

		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Static]
		[Export ("loadPlugIn:allowNonExecutable:")]
		void LoadPlugIn (NSUrl pluginUrl, bool allowNonExecutable);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CISampler : NSCopying {
		[Static, Export ("samplerWithImage:")]
		CISampler FromImage (CIImage sourceImage);

		[Internal, Static]
		[Export ("samplerWithImage:options:")]
		CISampler FromImage (CIImage sourceImag, [NullAllowed] NSDictionary options);

		[Export ("initWithImage:")]
		NativeHandle Constructor (CIImage sourceImage);

		[DesignatedInitializer]
		[Internal, Export ("initWithImage:options:")]
		NSObject Constructor (CIImage image, [NullAllowed] NSDictionary options);

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

		[MacCatalyst (13, 1)]
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

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("vectorWithCGPoint:")]
		CIVector Create (CGPoint point);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("vectorWithCGRect:")]
		CIVector Create (CGRect point);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("vectorWithCGAffineTransform:")]
		CIVector Create (CGAffineTransform affineTransform);

		[Static]
		[Export ("vectorWithString:")]
		CIVector FromString (string representation);

		[MacCatalyst (13, 1)]
		[Export ("initWithCGPoint:")]
		NativeHandle Constructor (CGPoint p);

		[MacCatalyst (13, 1)]
		[Export ("initWithCGRect:")]
		NativeHandle Constructor (CGRect r);

		[MacCatalyst (13, 1)]
		[Export ("initWithCGAffineTransform:")]
		NativeHandle Constructor (CGAffineTransform r);


		[Export ("initWithX:")]
		NativeHandle Constructor (nfloat x);

		[Export ("initWithX:Y:")]
		NativeHandle Constructor (nfloat x, nfloat y);

		[Export ("initWithX:Y:Z:")]
		NativeHandle Constructor (nfloat x, nfloat y, nfloat z);

		[Export ("initWithX:Y:Z:W:")]
		NativeHandle Constructor (nfloat x, nfloat y, nfloat z, nfloat w);

		[Export ("initWithString:")]
		NativeHandle Constructor (string representation);

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

		[MacCatalyst (13, 1)]
		[Export ("CGPointValue")]
		CGPoint Point { get; }

		[MacCatalyst (13, 1)]
		[Export ("CGRectValue")]
		CGRect Rectangle { get; }

		[MacCatalyst (13, 1)]
		[Export ("CGAffineTransformValue")]
		CGAffineTransform AffineTransform { get; }

		[Export ("stringRepresentation"), Internal]
		string StringRepresentation ();

	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIDetector {
		[Static, Export ("detectorOfType:context:options:"), Internal]
		[return: NullAllowed]
		CIDetector FromType (NSString detectorType, [NullAllowed] CIContext context, [NullAllowed] NSDictionary options);

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

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorMaxFeatureCount"), Internal]
		NSString MaxFeatureCount { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorEyeBlink"), Internal]
		NSString EyeBlink { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorSmile"), Internal]
		NSString Smile { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorAspectRatio")]
		NSString AspectRatio { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorFocalLength")]
		NSString FocalLength { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorTypeQRCode")]
		NSString TypeQRCode { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorTypeRectangle")]
		NSString TypeRectangle { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorNumberOfAngles")]
		NSString NumberOfAngles { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorReturnSubFeatures")]
		NSString ReturnSubFeatures { get; }

		[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Field ("CIFeatureTypeRectangle")]
		NSString TypeRectangle { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIFeatureTypeQRCode")]
		NSString TypeQRCode { get; }

		[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Export ("bounds", ArgumentSemantic.Assign)]
		CGRect Bounds { get; }

		[MacCatalyst (13, 1)]
		[Export ("faceAngle", ArgumentSemantic.Assign)]
		float FaceAngle { get; } /* float, not CGFloat */

		[MacCatalyst (13, 1)]
		[Export ("hasFaceAngle", ArgumentSemantic.Assign)]
		bool HasFaceAngle { get; }

		[MacCatalyst (13, 1)]
		[Export ("hasSmile", ArgumentSemantic.Assign)]
		bool HasSmile { get; }

		[MacCatalyst (13, 1)]
		[Export ("leftEyeClosed", ArgumentSemantic.Assign)]
		bool LeftEyeClosed { get; }

		[MacCatalyst (13, 1)]
		[Export ("rightEyeClosed", ArgumentSemantic.Assign)]
		bool RightEyeClosed { get; }
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
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
		[NullAllowed]
		string MessageString { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("symbolDescriptor")]
		CIQRCodeDescriptor SymbolDescriptor { get; }
	}

	[MacCatalyst (13, 1)]
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
		[NullAllowed]
		CIFeature [] SubFeatures { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CIImageProcessorKernel {
		[Static]
		[Export ("processWithInputs:arguments:output:error:")]
		bool Process ([NullAllowed] ICIImageProcessorInput [] inputs, [NullAllowed] NSDictionary<NSString, NSObject> arguments, ICIImageProcessorOutput output, out NSError error);

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
		CIImage Apply (CGRect extent, [NullAllowed] CIImage [] inputs, [NullAllowed] NSDictionary<NSString, NSObject> args, out NSError error);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("outputIsOpaque")]
		bool OutputIsOpaque { get; }

		[iOS (17, 0), MacCatalyst (17, 0), TV (17, 0), Mac (14, 0)]
		[Static]
		[Export ("roiTileArrayForInput:arguments:outputRect:")]
		CIVector [] GetRoiTileArray (int input, [NullAllowed] NSDictionary<NSString, NSObject> arguments, CGRect outputRect);
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIAccordionFoldTransition : CIAccordionFoldTransitionProtocol {

#if !NET
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

#if !NET
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

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaAverage")]
	interface CIAreaAverageProtocol : CIFilterProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaAverage {

		[CoreImageFilterProperty ("outputImageNonMPS")]
		CIImage OutputImageNonMps { get; }

		[iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[CoreImageFilterProperty ("outputImageMPS")]
		CIImage OutputImageMps { get; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaReductionFilter")]
	interface CIAreaReductionFilterProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaHistogram")]
	interface CIAreaHistogramProtocol : CIAreaReductionFilterProtocol {

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		[Abstract]
		[Export ("count")]
		nint InputCount { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIAreaHistogram : CIAreaHistogramProtocol {

#if !NET
		[Obsolete ("Use 'InputCount' instead.")]
		[CoreImageFilterProperty ("inputCount")]
		float Count { get; set; }
#endif

		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }

		[CoreImageFilterProperty ("outputImageNonMPS")]
		CIImage OutputImageNonMps { get; }

		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[CoreImageFilterProperty ("outputImageMPS")]
		CIImage OutputImageMps { get; }

		[CoreImageFilterProperty ("outputData")]
		NSData OutputData { get; }
	}

	[CoreImageFilter]
	[Abstract]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIReductionFilter {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMaximum")]
	interface CIAreaMaximumProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMaximum : CIAreaMaximumProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMaximumAlpha")]
	interface CIAreaMaximumAlphaProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMaximumAlpha : CIAreaMaximumAlphaProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMinimum")]
	interface CIAreaMinimumProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMinimum : CIAreaReductionFilterProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMinimumAlpha")]
	interface CIAreaMinimumAlphaProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMinimumAlpha : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CICodeGenerator {
		[CoreImageFilterProperty ("inputMessage")]
		NSData Message { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CICodeGenerator))]
	interface CIAztecCodeGenerator : CIAztecCodeGeneratorProtocol {

#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendWithMask))]
	interface CIBlendWithAlphaMask {
	}

	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIBlendFilter))]
	interface CIBlendWithMask : CIBlendWithMaskProtocol {

#if !NET
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
	[MacCatalyst (13, 1)]
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

#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIBumpDistortion")]
	interface CIBumpDistortionProtocol : CIFilterProtocol {

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
		[Export ("scale")]
		float Scale { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIBumpDistortion : CIBumpDistortionProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIBumpDistortionLinear")]
	interface CIBumpDistortionLinearProtocol : CIFilterProtocol {

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
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIBumpDistortionLinear : CIBumpDistortionLinearProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CICheckerboardGenerator : CICheckerboardGeneratorProtocol {

#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CICircleSplashDistortion")]
	interface CICircleSplashDistortionProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CICircleSplashDistortion : CICircleSplashDistortionProtocol {
	}

	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIScreenFilter {

		[CoreImageFilterProperty ("inputSharpness")]
		float Sharpness { get; set; }

#if !NET
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

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CICircularWrap")]
	interface CICircularWrapProtocol : CIFilterProtocol {

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
		[Export ("angle")]
		float Angle { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CICircularWrap : CICircularWrapProtocol {

#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter), Name = "CICMYKHalftone")]
	interface CICmykHalftone : CICmykHalftoneProtocol {

#if !NET
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
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIColorClamp : CIColorClampProtocol {

#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIColorCrossPolynomial : CIColorCrossPolynomialProtocol {
	}

	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIFilter))]
	interface CIColorCube : CIColorCubeProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
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
	interface CIColorMatrix : CIColorMatrixProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorMonochrome : CIColorMonochromeProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIColorCrossPolynomial))]
	interface CIColorPolynomial : CIColorPolynomialProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorPosterize : CIColorPosterizeProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIColumnAverage")]
	interface CIColumnAverageProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIColumnAverage : CIColumnAverageProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution3X3 {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution5X5 {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution7X7 {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution9Horizontal {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CICrystallize : CICrystallizeProtocol {

#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDiscBlur : CIDiscBlurProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIDisintegrateWithMaskTransition : CIDisintegrateWithMaskTransitionProtocol {

#if !NET
		[Obsolete ("Use 'MaskImage' instead.")]
		[CoreImageFilterProperty ("inputMaskImage")]
		CIImage Mask { get; set; }

		[Obsolete ("Use 'InputShadowOffset' instead.")]
		[CoreImageFilterProperty ("inputShadowOffset")]
		CIVector ShadowOffset { get; set; }
#endif
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIDisplacementDistortion")]
	interface CIDisplacementDistortionProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("displacementImage", ArgumentSemantic.Retain)]
		CIImage DisplacementImage { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDisplacementDistortion : CIDisplacementDistortionProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIDissolveTransition {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendFilter))]
	interface CIDivideBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIScreenFilter))]
	interface CIDotScreen : CIDotScreenProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIDroste")]
	interface CIDrosteProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("insetPoint0", ArgumentSemantic.Assign)]
		CGPoint InputInsetPoint0 { get; set; }

		[Abstract]
		[Export ("insetPoint1", ArgumentSemantic.Assign)]
		CGPoint InputInsetPoint1 { get; set; }

		[Abstract]
		[Export ("strands")]
		float Strands { get; set; }

		[Abstract]
		[Export ("periodicity")]
		float Periodicity { get; set; }

		[Abstract]
		[Export ("rotation")]
		float Rotation { get; set; }

		[Abstract]
		[Export ("zoom")]
		float Zoom { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDroste : CIDrosteProtocol {

#if !NET
		[Obsolete ("use 'InputInsetPoint0' instead.")]
		[CoreImageFilterProperty ("inputInsetPoint0")]
		CIVector InsetPoint0 { get; set; }

		[Obsolete ("use 'InputInsetPoint0' instead.")]
		[CoreImageFilterProperty ("inputInsetPoint1")]
		CIVector InsetPoint1 { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIEdges : CIEdgesProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIEdgeWork : CIEdgeWorkProtocol {
	}

	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CITileFilter {

		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

#if !NET
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

#if !NET
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

#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIGlassDistortion")]
	interface CIGlassDistortionProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("textureImage", ArgumentSemantic.Retain)]
		CIImage Texture { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIGlassDistortion : CIGlassDistortionProtocol {

#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIGlassLozenge")]
	interface CIGlassLozengeProtocol : CIFilterProtocol {

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
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("refraction")]
		float Refraction { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIGlassLozenge : CIGlassLozengeProtocol {

#if !NET
		[Obsolete ("Use 'InputPoint1' instead.")]
		[CoreImageFilterProperty ("inputPoint1")]
		CIVector Point1 { get; set; }

		[Obsolete ("Use 'InputPoint0' instead.")]
		[CoreImageFilterProperty ("inputPoint0")]
		CIVector Point0 { get; set; }
#endif
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIHeightFieldFromMask : CIHeightFieldFromMaskProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIHexagonalPixellate : CIHexagonalPixellateProtocol {

#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIHighlightShadowAdjust : CIHighlightShadowAdjustProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIHistogramDisplay")]
	interface CIHistogramDisplayProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("height")]
		float Height { get; set; }

		[Abstract]
		[Export ("highLimit")]
		float HighLimit { get; set; }

		[Abstract]
		[Export ("lowLimit")]
		float LowLimit { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	// incorrect version string for OSX: '10.?' Double-check documentation
	[BaseType (typeof (CIFilter))]
	interface CIHistogramDisplayFilter : CIHistogramDisplayProtocol {

		// [CoreImageFilterProperty ("inputImage")]
		// CIImage InputImage { get; set; }

		// [CoreImageFilterProperty ("inputHeight")]
		// float Height { get; set; }

		// [CoreImageFilterProperty ("inputHighLimit")]
		// float HighLimit { get; set; }

		// [CoreImageFilterProperty ("inputLowLimit")]
		// float LowLimit { get; set; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIHoleDistortion")]
	interface CIHoleDistortionProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIHoleDistortion : CIHoleDistortionProtocol {
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIKaleidoscope : CIKaleidoscopeProtocol {

#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CILenticularHaloGenerator : CILenticularHaloGeneratorProtocol {

#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CILightenBlendMode {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CILightTunnel")]
	interface CILightTunnelProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("rotation")]
		float Rotation { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CILightTunnel : CILightTunnelProtocol {

#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendFilter))]
	interface CILinearBurnBlendMode {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendFilter))]
	interface CILinearDodgeBlendMode {
	}

	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIFilter))]
	interface CILinearGradient : CILinearGradientProtocol {

#if !NET
		[Obsolete ("Use 'InputPoint1' instead.")]
		[CoreImageFilterProperty ("inputPoint1")]
		CIVector Point1 { get; set; }

		[Obsolete ("Use 'InputPoint0' instead.")]
		[CoreImageFilterProperty ("inputPoint0")]
		CIVector Point0 { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CILinearToSRGBToneCurve : CILinearToSrgbToneCurveProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
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

#if !NET
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CINoiseReduction : CINoiseReductionProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CITileFilter))]
	interface CIOpTile : CIOpTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIOverlayBlendMode {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CITransitionFilter))]
	interface CIPageCurlTransition : CIPageCurlTransitionProtocol {

#if !NET
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIPageCurlWithShadowTransition : CIPageCurlWithShadowTransitionProtocol {

#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CITileFilter))]
	interface CIParallelogramTile : CIParallelogramTileProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CICodeGenerator), Name = "CIPDF417BarcodeGenerator")]
	interface CIPdf417BarcodeGenerator : CIPdf417BarcodeGeneratorProtocol {
#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPerspectiveTransform))]
	interface CIPerspectiveCorrection : CIPerspectiveCorrectionProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIPerspectiveTile : CIPerspectiveTileProtocol {
#if !NET
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
#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPerspectiveTransform))]
	interface CIPerspectiveTransformWithExtent : CIPerspectiveTransformWithExtentProtocol {
#if !NET
		[Obsolete ("Use 'InputExtent' instead.")]
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
#endif
	}

	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIPhotoEffect : CIPhotoEffectProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectChrome {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectFade {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectInstant {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectMono {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectNoir {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectProcess {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectTonal {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectTransfer {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIPinchDistortion")]
	interface CIPinchDistortionProtocol : CIFilterProtocol {

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
		[Export ("scale")]
		float Scale { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIPinchDistortion : CIPinchDistortionProtocol {

		// [CoreImageFilterProperty ("inputScale")]
		// float Scale { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendFilter))]
	interface CIPinLightBlendMode {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIPixellate : CIPixellateProtocol {
#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIPointillize : CIPointillizeProtocol {
#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CICodeGenerator))]
	interface CIQRCodeGenerator : CIQRCodeGeneratorProtocol {

		[CoreImageFilterProperty ("outputCGImage")]
		CGImage OutputCGImage { get; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIRadialGradient : CIRadialGradientProtocol {

#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CITransitionFilter))]
	interface CIRippleTransition : CIRippleTransitionProtocol {
#if !NET
		[Obsolete ("Use 'InputExtent' instead.")]
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }

		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIRowAverage")]
	interface CIRowAverageProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIRowAverage : CIRowAverageProtocol {

#if !NET
		[Obsolete ("Use 'InputExtent' instead.")]
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
#endif
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
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CILinearGradient))]
	interface CISmoothLinearGradient : CISmoothLinearGradientProtocol {
#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISpotColor : CISpotColorProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISpotLight : CISpotLightProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISRGBToneCurveToLinear : CISrgbToneCurveToLinearProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIStarShineGenerator : CIStarShineGeneratorProtocol {
#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIStraightenFilter : CIStraightenProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIStretchCrop")]
	interface CIStretchCropProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("size", ArgumentSemantic.Assign)]
		CGPoint InputSize { get; set; }

		[Abstract]
		[Export ("cropAmount")]
		float CropAmount { get; set; }

		[Abstract]
		[Export ("centerStretchAmount")]
		float CenterStretchAmount { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIStretchCrop : CIStretchCropProtocol {

#if !NET
		[Obsolete ("Use 'InputSize' instead.")]
		[CoreImageFilterProperty ("inputSize")]
		CIVector Size { get; set; }
#endif
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIStripesGenerator : CIStripesGeneratorProtocol {
#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
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
#if !NET
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

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CITorusLensDistortion")]
	interface CITorusLensDistortionProtocol : CIFilterProtocol {

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
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("refraction")]
		float Refraction { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CITorusLensDistortion : CITorusLensDistortionProtocol {

#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CITriangleKaleidoscope : CITriangleKaleidoscopeProtocol {
#if !NET
		[Obsolete ("Use 'InputPoint' instead.")]
		[CoreImageFilterProperty ("inputPoint")]
		CIVector Point { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CITileFilter))]
	interface CITriangleTile : CITriangleTileProtocol {
	}

	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CITwelvefoldReflectedTile : CITwelvefoldReflectedTileProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CITwirlDistortion")]
	interface CITwirlDistortionProtocol : CIFilterProtocol {

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
		[Export ("angle")]
		float Angle { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CITwirlDistortion : CITwirlDistortionProtocol {

		// [CoreImageFilterProperty ("inputAngle")]
		// float Angle { get; set; }
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIVignette : CIVignetteProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIVignetteEffect : CIVignetteEffectProtocol {

#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIVortexDistortion")]
	interface CIVortexDistortionProtocol : CIFilterProtocol {

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
		[Export ("angle")]
		float Angle { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIVortexDistortion : CIVortexDistortionProtocol {

		// [CoreImageFilterProperty ("inputAngle")]
		// float Angle { get; set; }
	}

	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIWhitePointAdjust : CIWhitePointAdjustProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIZoomBlur : CIZoomBlurProtocol {
#if !NET
		[Obsolete ("Use 'InputCenter' instead.")]
		[CoreImageFilterProperty ("inputCenter")]
		CIVector Center { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDepthOfField : CIDepthOfFieldProtocol {

#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISunbeamsGenerator : CISunbeamsGeneratorProtocol {
#if !NET
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

	[MacCatalyst (13, 1)]
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIMaskedVariableBlur : CIMaskedVariableBlurProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIClamp {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIHueSaturationValueGradient : CIHueSaturationValueGradientProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CINinePartStretched")]
	interface CINinePartStretchedProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("breakpoint0", ArgumentSemantic.Assign)]
		CGPoint InputBreakpoint0 { get; set; }

		[Abstract]
		[Export ("breakpoint1", ArgumentSemantic.Assign)]
		CGPoint InputBreakpoint1 { get; set; }

		[Abstract]
		[Export ("growAmount", ArgumentSemantic.Assign)]
		CGPoint InputGrowAmount { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CINinePartStretched : CINinePartStretchedProtocol {

#if !NET
		[Obsolete ("Use 'InputGrowAmount' instead.")]
		[CoreImageFilterProperty ("inputGrowAmount")]
		CIVector GrowAmount { get; set; }

		[Obsolete ("Use 'InputBreakpoint0' instead.")]
		[CoreImageFilterProperty ("inputBreakpoint0")]
		CIVector Breakpoint0 { get; set; }

		[Obsolete ("Use 'InputBreakpoint1' instead.")]
		[CoreImageFilterProperty ("inputBreakpoint1")]
		CIVector Breakpoint1 { get; set; }
#endif
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CINinePartTiled")]
	interface CINinePartTiledProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }


		[Abstract]
		[Export ("breakpoint0", ArgumentSemantic.Assign)]
		CGPoint InputBreakpoint0 { get; set; }


		[Abstract]
		[Export ("breakpoint1", ArgumentSemantic.Assign)]
		CGPoint InputBreakpoint1 { get; set; }


		[Abstract]
		[Export ("growAmount", ArgumentSemantic.Assign)]
		CGPoint InputGrowAmount { get; set; }

		[Abstract]
		[Export ("flipYTiles")]
		bool FlipYTiles { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CINinePartTiled : CINinePartTiledProtocol {

#if !NET
		[Obsolete ("Use 'InputGrowAmount' instead.")]
		[CoreImageFilterProperty ("inputGrowAmount")]
		CIVector GrowAmount { get; set; }

		[Obsolete ("Use 'InputBreakpoint0' instead.")]
		[CoreImageFilterProperty ("inputBreakpoint0")]
		CIVector Breakpoint0 { get; set; }

		[Obsolete ("Use 'InputBreakpoint1' instead.")]
		[CoreImageFilterProperty ("inputBreakpoint1")]
		CIVector Breakpoint1 { get; set; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIThermal : CIThermalProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIXRay : CIXRayProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMinMaxRed")]
	interface CIAreaMinMaxRedProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIAreaMaximum))]
	interface CIAreaMinMaxRed : CIAreaMinMaxRedProtocol {
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	[CoreImageFilter]
	[Abstract]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIImageGenerator {
		[CoreImageFilterProperty ("inputScaleFactor")]
		float ScaleFactor { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIImageGenerator))]
	interface CIAttributedTextImageGenerator : CIAttributedTextImageGeneratorProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	// Maybe 'typeof (CIScaleTransform)' (shared 'Scale' and 'AspectRatio' property).
	// It's possible to add ours but it can bite us back in the future if Apple introduce the same with different properties.
	[BaseType (typeof (CIFilter))]
	interface CIBicubicScaleTransform : CIBicubicScaleTransformProtocol {

#if !NET
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CILinearBlur))]
	interface CIBokehBlur : CIBokehBlurProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))] // Could almost be typeof 'CIColorCube' but property is 'inputCube0Data' not 'inputCubeData'
	interface CIColorCubesMixedWithMask : CIColorCubesMixedWithMaskProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIColorCurves : CIColorCurvesProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
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

		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[MacCatalyst (14, 1)]
		[CoreImageFilterProperty ("inputGainMap")]
		CIImage GainMap { get; set; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[MacCatalyst (14, 1)]
		[CoreImageFilterProperty ("inputGlassesImage")]
		CIImage GlassesImage { get; set; }
	}

	[CoreImageFilter]
	[Abstract]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDepthDisparityConverter { }

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIDepthDisparityConverter))]
	interface CIDepthToDisparity : CIDepthToDisparityProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIDepthDisparityConverter))]
	interface CIDisparityToDepth : CIDisparityToDepthProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIEdgePreserveUpsampleFilter : CIEdgePreserveUpsampleProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CILabDeltaE : CILabDeltaEProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIImageGenerator))]
	interface CITextImageGenerator : CITextImageGeneratorProtocol {
	}

	[CoreImageFilter]
	[Abstract]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIMorphology {
		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIMorphology))]
	interface CIMorphologyGradient : CIMorphologyGradientProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIMorphology))]
	interface CIMorphologyMaximum : CIMorphologyMaximumProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIMorphology))]
	interface CIMorphologyMinimum : CIMorphologyMinimumProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendWithMask))]
	interface CIBlendWithBlueMask { }

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendWithMask))]
	interface CIBlendWithRedMask { }

	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface CIBarcodeDescriptor : NSSecureCoding, NSCopying {
		// empty
	}

	[MacCatalyst (13, 1)]
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
		NativeHandle Constructor (NSData errorCorrectedPayload, nint symbolVersion, byte maskPattern, CIQRCodeErrorCorrectionLevel errorCorrectionLevel);

		[Static]
		[Export ("descriptorWithPayload:symbolVersion:maskPattern:errorCorrectionLevel:")]
		[return: NullAllowed]
		CIQRCodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, nint symbolVersion, byte maskPattern, CIQRCodeErrorCorrectionLevel errorCorrectionLevel);
	}

	[MacCatalyst (13, 1)]
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
		NativeHandle Constructor (NSData errorCorrectedPayload, bool isCompact, nint layerCount, nint dataCodewordCount);

		[Static]
		[Export ("descriptorWithPayload:isCompact:layerCount:dataCodewordCount:")]
		[return: NullAllowed]
		CIAztecCodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, bool isCompact, nint layerCount, nint dataCodewordCount);
	}

	[MacCatalyst (13, 1)]
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
		NativeHandle Constructor (NSData errorCorrectedPayload, bool isCompact, nint rowCount, nint columnCount);

		[Static]
		[Export ("descriptorWithPayload:isCompact:rowCount:columnCount:")]
		[return: NullAllowed]
		CIPdf417CodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, bool isCompact, nint rowCount, nint columnCount);
	}

	[MacCatalyst (13, 1)]
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
		NativeHandle Constructor (NSData errorCorrectedPayload, nint rowCount, nint columnCount, CIDataMatrixCodeEccVersion eccVersion);

		[Static]
		[Export ("descriptorWithPayload:rowCount:columnCount:eccVersion:")]
		[return: NullAllowed]
		CIDataMatrixCodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, nint rowCount, nint columnCount, CIDataMatrixCodeEccVersion eccVersion);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIColorKernel))]
	[DisableDefaultCtor] // Handle is nil for `init`
	interface CIBlendKernel {

		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("kernelWithString:")]
		[return: NullAllowed]
		CIBlendKernel CreateKernel (string @string);

		[Export ("applyWithForeground:background:")]
		[return: NullAllowed]
		CIImage Apply (CIImage foreground, CIImage background);

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Handle is null if created thru `init`
	interface CIRenderDestination {

		[Export ("initWithPixelBuffer:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer);

		[Export ("initWithIOSurface:")]
		NativeHandle Constructor (IOSurface.IOSurface surface);

		[Export ("initWithMTLTexture:commandBuffer:")]
		NativeHandle Constructor (IMTLTexture texture, [NullAllowed] IMTLCommandBuffer commandBuffer);

		[Export ("initWithWidth:height:pixelFormat:commandBuffer:mtlTextureProvider:")]
		NativeHandle Constructor (nuint width, nuint height, MTLPixelFormat pixelFormat, [NullAllowed] IMTLCommandBuffer commandBuffer, [NullAllowed] Func<IMTLTexture> block);

		[Export ("initWithGLTexture:target:width:height:")]
		NativeHandle Constructor (uint texture, uint target, nuint width, nuint height);

		[Export ("initWithBitmapData:width:height:bytesPerRow:format:")]
		NativeHandle Constructor (IntPtr data, nuint width, nuint height, nuint bytesPerRow, CIFormat format);

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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no docs, but only returned from CIRenderTask.WaitUntilCompleted. Handle is null if created thru `init`
	interface CIRenderInfo {

		[Export ("kernelExecutionTime")]
		double KernelExecutionTime { get; }

		[Export ("passCount")]
		nint PassCount { get; }

		[Export ("pixelsProcessed")]
		nint PixelsProcessed { get; }

		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("kernelCompileTime")]
		double KernelCompileTime { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no docs, but only returned from CIContext.StartTaskToRender. Handle is null if created thru `init`
	interface CIRenderTask {
		[Export ("waitUntilCompletedAndReturnError:")]
		[return: NullAllowed]
		CIRenderInfo WaitUntilCompleted ([NullAllowed] out NSError error);
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationAVPortraitEffectsMatte")]
		NSString AVPortraitEffectsMatteKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationPortraitEffectsMatteImage")]
		NSString PortraitEffectsMatteImageKey { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationAVSemanticSegmentationMattes")]
		NSString SemanticSegmentationMattesKey { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationSemanticSegmentationSkinMatteImage")]
		NSString SemanticSegmentationSkinMatteImageKey { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationSemanticSegmentationHairMatteImage")]
		NSString SemanticSegmentationHairMatteImageKey { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationSemanticSegmentationTeethMatteImage")]
		NSString SemanticSegmentationTeethMatteImageKey { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[MacCatalyst (14, 1)]
		[Field ("kCIImageRepresentationSemanticSegmentationGlassesMatteImage")]
		NSString SemanticSegmentationGlassesMatteImage { get; }

		[iOS (14, 3), TV (14, 3), Mac (11, 1)]
		[MacCatalyst (14, 3)]
		[Field ("kCIImageRepresentationSemanticSegmentationSkyMatteImage")]
		NSString SemanticSegmentationSkyMatteImage { get; }

		[iOS (14, 1), TV (14, 1), Mac (11, 0), MacCatalyst (14, 1)]
		[Field ("kCIImageAuxiliaryHDRGainMap")]
		NSString AuxiliaryHdrGainMap { get; }

		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Field ("kCIImageCacheImmediately")]
		NSString CacheImmediately { get; }

		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Field ("kCIImageExpandToHDR")]
		NSString ExpandToHdr { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("CIImageRepresentationKeys")]
	interface CIImageRepresentationOptions {

		float LossyCompressionQuality { get; set; }

		AVDepthData AVDepthData { get; set; }

		CIImage DepthImage { get; set; }

		CIImage DisparityImage { get; set; }

		[MacCatalyst (13, 1)]
		CIImage PortraitEffectsMatteImage { get; set; }

		[MacCatalyst (13, 1)]
		AVPortraitEffectsMatte AVPortraitEffectsMatte { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		AVSemanticSegmentationMatte [] SemanticSegmentationMattes { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		CIImage SemanticSegmentationSkinMatteImage { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		CIImage SemanticSegmentationHairMatteImage { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		CIImage SemanticSegmentationTeethMatteImage { get; set; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMinMax")]
	interface CIAreaMinMaxProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMinMax : CIAreaReductionFilterProtocol {

		[CoreImageFilterProperty ("outputImageNonMPS")]
		CIImage OutputImageNonMps { get; }

		[iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[CoreImageFilterProperty ("outputImageMPS")]
		CIImage OutputImageMps { get; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDither : CIDitherProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIMeshGenerator : CIMeshGeneratorProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIMix : CIMixProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISampleNearest {

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISaliencyMapFilter : CISaliencyMapProtocol {
	}

	[CoreImageFilter]
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDocumentEnhancer : CIDocumentEnhancerProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIKMeans")]
	interface CIKMeansProtocol : CIAreaReductionFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputMeans", ArgumentSemantic.Retain)]
		CIImage Means { get; set; }

		[Abstract]
		[Export ("count")]
		nint InputCount { get; set; }

		[Abstract]
		[Export ("passes")]
		float InputPasses { get; set; }

		[Abstract]
		[Export ("perceptual")]
		bool Perceptual { get; set; }
	}

	[CoreImageFilter]
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIKMeans : CIKMeansProtocol {

#if !NET
		[Obsolete ("Use 'InputCount' instead.")]
		[CoreImageFilterProperty ("inputCount")]
		float Count { get; set; }

		[Obsolete ("Use 'InputPasses' instead.")]
		[CoreImageFilterProperty ("inputPasses")]
		int Passes { get; set; }
#endif
	}

	[CoreImageFilter]
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	[Abstract]
	interface CIMorphologyRectangle {

#if !NET
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
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIMorphologyRectangle))]
	interface CIMorphologyRectangleMaximum : CIMorphologyRectangleMaximumProtocol {
	}

	[CoreImageFilter]
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIMorphologyRectangle))]
	interface CIMorphologyRectangleMinimum : CIMorphologyRectangleMinimumProtocol {
	}

	[CoreImageFilter]
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIPaletteCentroid : CIPaletteCentroidProtocol {
	}

	[CoreImageFilter]
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIPalettize : CIPalettizeProtocol {
	}

	[CoreImageFilter]
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	[Abstract]
	interface CIKeystoneCorrection {

		[CoreImageFilterProperty ("inputFocalLength")]
		float FocalLength { get; set; }

#if !NET
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
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIKeystoneCorrection))]
	interface CIKeystoneCorrectionCombined : CIKeystoneCorrectionCombinedProtocol {

		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
	}

	[CoreImageFilter]
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIPerspectiveRotate : CIPerspectiveRotateProtocol {

		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
	}

	[CoreImageFilter]
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIGaborGradients : CIGaborGradientsProtocol {
	}

	[CoreImageFilter]
	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIRoundedRectangleGenerator : CIRoundedRectangleGeneratorProtocol {

#if !NET
		[Obsolete ("Use 'InputExtent' instead.")]
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
#endif
	}

	#region Protocols

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIAffineClamp")]
	interface CIAffineClampProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("transform", ArgumentSemantic.Assign)]
		CGAffineTransform Transform { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIAffineTile")]
	interface CIAffineTileProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("transform", ArgumentSemantic.Assign)]
		CGAffineTransform Transform { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIAttributedTextImageGenerator")]
	interface CIAttributedTextImageGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("text", ArgumentSemantic.Retain)]
		NSAttributedString Text { get; set; }

		[Abstract]
		[Export ("scaleFactor")]
		float ScaleFactor { get; set; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("padding")]
		float Padding { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIBarcodeGenerator")]
	interface CIBarcodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("barcodeDescriptor", ArgumentSemantic.Retain)]
		CIBarcodeDescriptor BarcodeDescriptor { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIBoxBlur")]
	interface CIBoxBlurProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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
		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Abstract]
		[Export ("barcodeHeight")]
		float BarcodeHeight { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("extrapolate")]
		bool Extrapolate { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("extrapolate")]
		bool Extrapolate { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("extrapolate")]
		bool Extrapolate { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorInvert")]
	interface CIColorInvertProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorMap")]
	interface CIColorMapProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("gradientImage", ArgumentSemantic.Retain)]
		CIImage GradientImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorPosterize")]
	interface CIColorPosterizeProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("levels")]
		float Levels { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIComicEffect")]
	interface CIComicEffectProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CICompositeOperation")]
	interface CICompositeOperationProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("backgroundImage", ArgumentSemantic.Retain)]
		CIImage BackgroundImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDepthToDisparity")]
	interface CIDepthToDisparityProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDiscBlur")]
	interface CIDiscBlurProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDisparityToDepth")]
	interface CIDisparityToDepthProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDissolveTransition")]
	interface CIDissolveTransitionProtocol : CIFilterProtocol {
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDither")]
	interface CIDitherProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDocumentEnhancer")]
	interface CIDocumentEnhancerProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("amount")]
		float Amount { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIEdges")]
	interface CIEdgesProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIEdgeWork")]
	interface CIEdgeWorkProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIExposureAdjust")]
	interface CIExposureAdjustProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("EV")]
		float EV { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIGaborGradients")]
	interface CIGaborGradientsProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIGammaAdjust")]
	interface CIGammaAdjustProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("power")]
		float Power { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIGaussianBlur")]
	interface CIGaussianBlurProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIHeightFieldFromMask")]
	interface CIHeightFieldFromMaskProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIHueAdjust")]
	interface CIHueAdjustProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIKeystoneCorrectionCombined")]
	interface CIKeystoneCorrectionCombinedProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIKeystoneCorrectionHorizontal")]
	interface CIKeystoneCorrectionHorizontalProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIKeystoneCorrectionVertical")]
	interface CIKeystoneCorrectionVerticalProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CILabDeltaE")]
	interface CILabDeltaEProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("image2", ArgumentSemantic.Retain)]
		CIImage Image2 { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CILinearToSRGBToneCurve")]
	interface CILinearToSrgbToneCurveProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMaskToAlpha")]
	interface CIMaskToAlphaProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMaximumComponent")]
	interface CIMaximumComponentProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMedian")]
	interface CIMedianProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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
		CIVector [] Mesh { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMinimumComponent")]
	interface CIMinimumComponentProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMorphologyGradient")]
	interface CIMorphologyGradientProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMorphologyMaximum")]
	interface CIMorphologyMaximumProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMorphologyMinimum")]
	interface CIMorphologyMinimumProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPerspectiveCorrection")]
	interface CIPerspectiveCorrectionProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("crop")]
		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		bool Crop { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPerspectiveTransform")]
	interface CIPerspectiveTransformProtocol : CIFourCoordinateGeometryFilterProtocol {
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPerspectiveTransformWithExtent")]
	interface CIPerspectiveTransformWithExtentProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPhotoEffect")]
	interface CIPhotoEffectProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("extrapolate")]
		bool Extrapolate { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIQRCodeGenerator")]
	interface CIQRCodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("message", ArgumentSemantic.Retain)]
		NSData Message { get; set; }

		[Abstract]
		[Export ("correctionLevel", ArgumentSemantic.Retain)]
		string CorrectionLevel { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIRandomGenerator")]
	interface CIRandomGeneratorProtocol : CIFilterProtocol {
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISaliencyMap")]
	interface CISaliencyMapProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISepiaTone")]
	interface CISepiaToneProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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
		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		float Radius { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISRGBToneCurveToLinear")]
	interface CISrgbToneCurveToLinearProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIStraighten")]
	interface CIStraightenProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("padding")]
		float Padding { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIThermal")]
	interface CIThermalProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIVibrance")]
	interface CIVibranceProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("amount")]
		float Amount { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIWhitePointAdjust")]
	interface CIWhitePointAdjustProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIXRay")]
	interface CIXRayProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[iOS (13, 0)]
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIColorAbsoluteDifference")]
	interface CIColorAbsoluteDifferenceProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("inputImage2")]
		CIImage Image2 { get; set; }
	}

	[CoreImageFilter]
	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIColorAbsoluteDifference : CIColorAbsoluteDifferenceProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIColorThreshold")]
	interface CIColorThresholdProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("threshold")]
		float Threshold { get; set; }
	}

	[CoreImageFilter]
	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIColorThreshold : CIColorThresholdProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIColorThresholdOtsu")]
	interface CIColorThresholdOtsuProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[CoreImageFilter]
	[iOS (14, 0)]
	[TV (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIColorThresholdOtsu : CIColorThresholdOtsuProtocol {
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionRGB3X3 : CIFilterProtocol {

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionRGB5X5 : CIFilterProtocol {

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionRGB7X7 : CIFilterProtocol {

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionRGB9Horizontal : CIFilterProtocol {

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionRGB9Vertical : CIFilterProtocol {

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CILinearLightBlendMode : CIFilterProtocol {

		[CoreImageFilterProperty ("inputBackgroundImage")]
		CIImage BackgroundImage { get; set; }

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIPersonSegmentation : CIPersonSegmentationProtocol {
	}

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0)]
	[Protocol (Name = "CIPersonSegmentation")]
	interface CIPersonSegmentationProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("qualityLevel")]
		nuint QualityLevel { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIVividLightBlendMode : CIFilterProtocol {

		[CoreImageFilterProperty ("inputBackgroundImage")]
		CIImage BackgroundImage { get; set; }

		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }
	}

	[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
	[Protocol (Name = "CIAreaLogarithmicHistogram")]
	interface CIAreaLogarithmicHistogramProtocol : CIAreaReductionFilterProtocol {
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		[Abstract]
		[Export ("count")]
		nint Count { get; set; }

		[Abstract]
		[Export ("minimumStop")]
		float MinimumStop { get; set; }

		[Abstract]
		[Export ("maximumStop")]
		float MaximumStop { get; set; }
	}

	[CoreImageFilter]
	[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIAreaLogarithmicHistogram : CIAreaLogarithmicHistogramProtocol {
	}

	[iOS (16, 0), TV (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
	[Protocol (Name = "CIConvertLab")]
	interface CIConvertLabProtocol : CIFilterProtocol {
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("normalize")]
		bool Normalize { get; set; }
	}

	[CoreImageFilter]
	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIBlurredRectangleGenerator : CIBlurredRectangleGeneratorProtocol {
	}

	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[Protocol (Name = "CIBlurredRectangleGenerator")]
	interface CIBlurredRectangleGeneratorProtocol : CIFilterProtocol {
		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("sigma")]
		float Sigma { get; set; }

		[Abstract]
		[NullAllowed, Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }
	}

	[CoreImageFilter]
	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CIFilter))]
	interface CICannyEdgeDetector : CICannyEdgeDetectorProtocol {
	}

	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[Protocol (Name = "CICannyEdgeDetector")]
	interface CICannyEdgeDetectorProtocol : CIFilterProtocol {
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("gaussianSigma")]
		float GaussianSigma { get; set; }

		[Abstract]
		[Export ("perceptual")]
		bool Perceptual { get; set; }

		[Abstract]
		[Export ("thresholdHigh")]
		float ThresholdHigh { get; set; }

		[Abstract]
		[Export ("thresholdLow")]
		float ThresholdLow { get; set; }

		[Abstract]
		[Export ("hysteresisPasses")]
		nint HysteresisPasses { get; set; }
	}

	[CoreImageFilter]
	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIRoundedRectangleStrokeGenerator : CIRoundedRectangleStrokeGeneratorProtocol {
	}

	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[Protocol (Name = "CIRoundedRectangleStrokeGenerator")]
	interface CIRoundedRectangleStrokeGeneratorProtocol : CIFilterProtocol {
		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[NullAllowed, Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }
	}

	[CoreImageFilter]
	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CIFilter))]
	interface CISobelGradients : CISobelGradientsProtocol {
	}

	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[Protocol (Name = "CISobelGradients")]
	interface CISobelGradientsProtocol : CIFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	// LabToRGBFilter and RGBtoLabFilter are names for filter API but return the same type CIConvertLab
	// Enabling these cause a large number of introspection errors
	// https://github.com/xamarin/xamarin-macios/issues/15662
#if false
	[CoreImageFilter]
	[iOS (16,0), TV (16,0), Mac (13,0), MacCatalyst (16,0)]
	[BaseType (typeof (CIFilter), Name = "CIConvertLab")]
	interface LabToRGBFilter : CIConvertLabProtocol
	{

	}

	[CoreImageFilter]
	[iOS (16,0), TV (16,0), Mac (13,0), MacCatalyst (16,0)]
	[BaseType (typeof (CIFilter), Name = "CIConvertLab")]
	interface RGBtoLabFilter : CIConvertLabProtocol
	{

	}
#endif

	#endregion

}
