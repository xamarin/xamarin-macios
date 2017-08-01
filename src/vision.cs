//
// Vision C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using XamCore.CoreFoundation;
using XamCore.CoreGraphics;
using XamCore.CoreImage;
using XamCore.CoreML;
using XamCore.CoreVideo;
using XamCore.Foundation;
using XamCore.Metal;
using XamCore.ObjCRuntime;
using XamCore.ImageIO;

using Vector2 = global::OpenTK.Vector2;
using Matrix3 = global::OpenTK.Matrix3;

namespace XamCore.Vision {

	[ErrorDomain ("VNErrorDomain")]
	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Native]
	enum VNErrorCode : nint {
		Ok = 0,
		RequestCancelled,
		InvalidFormat,
		OperationFailed,
		OutOfBoundsError,
		InvalidOption,
		IOError,
		MissingOption,
		NotImplemented,
		InternalError,
		OutOfMemory,
		UnknownError,
		InvalidOperation,
		InvalidImage,
		InvalidArgument,
		InvalidModel,
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Native]
	enum VNRequestTrackingLevel : nuint {
		Accurate = 0,
		Fast,
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Native]
	enum VNImageCropAndScaleOption : nuint {
		CenterCrop = 0,
		ScaleFit = 1,
		ScaleFill,
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	enum VNBarcodeSymbology {
		[Field ("VNBarcodeSymbologyAztec")]
		Aztec,

		[Field ("VNBarcodeSymbologyCODE39")]
		Code39,

		[Field ("VNBarcodeSymbologyCODE39Checksum")]
		Code39Checksum,

		[Field ("VNBarcodeSymbologyCODE39FullASCII")]
		Code39FullAscii,

		[Field ("VNBarcodeSymbologyCODE39FullASCIIChecksum")]
		Code39FullAsciiChecksum,

		[Field ("VNBarcodeSymbologyCODE93")]
		Code93,

		[Field ("VNBarcodeSymbologyCODE93i")]
		Code93i,

		[Field ("VNBarcodeSymbologyCODE128")]
		Code128,

		[Field ("VNBarcodeSymbologyDataMatrix")]
		DataMatrix,

		[Field ("VNBarcodeSymbologyEAN8")]
		Ean8,

		[Field ("VNBarcodeSymbologyEAN13")]
		Ean13,

		[Field ("VNBarcodeSymbologyI2OF5")]
		I2OF5,

		[Field ("VNBarcodeSymbologyI2OF5Checksum")]
		I2OF5Checksum,

		[Field ("VNBarcodeSymbologyITF14")]
		Itf14,

		[Field ("VNBarcodeSymbologyPDF417")]
		Pdf417,

		[Field ("VNBarcodeSymbologyQR")]
		QR,

		[Field ("VNBarcodeSymbologyUPCE")]
		Upce,
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNCoreMLModel {

		[Static]
		[Export ("modelForMLModel:error:")]
		[return: NullAllowed]
		VNCoreMLModel FromMLModel (MLModel model, out NSError error);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	delegate void VNRequestCompletionHandler (VNRequest request, NSError error);

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNCoreMLRequest {

		[Export ("model")]
		VNCoreMLModel Model { get; }

		[Export ("imageCropAndScaleOption", ArgumentSemantic.Assign)]
		VNImageCropAndScaleOption ImageCropAndScaleOption { get; set; }

		[Export ("initWithModel:")]
		IntPtr Constructor (VNCoreMLModel model);

		[Export ("initWithModel:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (VNCoreMLModel model, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectBarcodesRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Static]
		[Protected]
		[Export ("supportedSymbologies", ArgumentSemantic.Copy)]
		NSString [] WeakSupportedSymbologies { get; }

		[Static]
		[Wrap ("VNBarcodeSymbologyExtensions.GetValues (WeakSupportedSymbologies)")]
		VNBarcodeSymbology [] SupportedSymbologies { get; }

		[Protected]
		[Export ("symbologies", ArgumentSemantic.Copy)]
		NSString [] WeakSymbologies { get; set; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectFaceLandmarksRequest : VNFaceObservationAccepting {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectFaceRectanglesRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectHorizonRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectRectanglesRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("minimumAspectRatio")]
		float MinimumAspectRatio { get; set; }

		[Export ("maximumAspectRatio")]
		float MaximumAspectRatio { get; set; }

		[Export ("quadratureTolerance")]
		float QuadratureTolerance { get; set; }

		[Export ("minimumSize")]
		float MinimumSize { get; set; }

		[Export ("minimumConfidence")]
		float MinimumConfidence { get; set; }

		[Export ("maximumObservations")]
		nuint MaximumObservations { get; set; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectTextRectanglesRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("reportCharacterBoxes")]
		bool ReportCharacterBoxes { get; set; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface VNFaceLandmarkRegion {

		[Export ("pointCount")]
		nuint PointCount { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNFaceLandmarkRegion))]
	interface VNFaceLandmarkRegion2D {

		[Internal]
		[Export ("points")]
		IntPtr _GetPoints ();

		[Export ("pointAtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector2 GetPoint (nuint index);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface VNFaceLandmarks {

		[Export ("confidence")]
		float Confidence { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNFaceLandmarks))]
	interface VNFaceLandmarks2D {

		[NullAllowed, Export ("allPoints")]
		VNFaceLandmarkRegion2D AllPoints { get; }

		[NullAllowed, Export ("faceContour")]
		VNFaceLandmarkRegion2D FaceContour { get; }

		[NullAllowed, Export ("leftEye")]
		VNFaceLandmarkRegion2D LeftEye { get; }

		[NullAllowed, Export ("rightEye")]
		VNFaceLandmarkRegion2D RightEye { get; }

		[NullAllowed, Export ("leftEyebrow")]
		VNFaceLandmarkRegion2D LeftEyebrow { get; }

		[NullAllowed, Export ("rightEyebrow")]
		VNFaceLandmarkRegion2D RightEyebrow { get; }

		[NullAllowed, Export ("nose")]
		VNFaceLandmarkRegion2D Nose { get; }

		[NullAllowed, Export ("noseCrest")]
		VNFaceLandmarkRegion2D NoseCrest { get; }

		[NullAllowed, Export ("medianLine")]
		VNFaceLandmarkRegion2D MedianLine { get; }

		[NullAllowed, Export ("outerLips")]
		VNFaceLandmarkRegion2D OuterLips { get; }

		[NullAllowed, Export ("innerLips")]
		VNFaceLandmarkRegion2D InnerLips { get; }

		[NullAllowed, Export ("leftPupil")]
		VNFaceLandmarkRegion2D LeftPupil { get; }

		[NullAllowed, Export ("rightPupil")]
		VNFaceLandmarkRegion2D RightPupil { get; }
	}

	interface IVNFaceObservationAccepting { }

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Protocol]
	interface VNFaceObservationAccepting {

		[Abstract]
		[NullAllowed, Export ("inputFaceObservations", ArgumentSemantic.Copy)]
		VNFaceObservation [] InputFaceObservations { get; set; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (VNTargetedImageRequest))]
	interface VNImageRegistrationRequest {

		[Export ("initWithTargetedCVPixelBuffer:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer);

		[Export ("initWithTargetedCVPixelBuffer:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:")]
		IntPtr Constructor (CGImage image);

		[Export ("initWithTargetedCGImage:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGImage image, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:")]
		IntPtr Constructor (CIImage image);

		[Export ("initWithTargetedCIImage:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CIImage image, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:")]
		IntPtr Constructor (NSUrl imageUrl);

		[Export ("initWithTargetedImageURL:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl imageUrl, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:")]
		IntPtr Constructor (NSData imageData);

		[Export ("initWithTargetedImageData:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSData imageData, [NullAllowed] VNRequestCompletionHandler completionHandler);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageRegistrationRequest))]
	interface VNTranslationalImageRegistrationRequest {

		// Inlined from parent class
		[Export ("initWithTargetedCVPixelBuffer:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer);

		[Export ("initWithTargetedCVPixelBuffer:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:")]
		IntPtr Constructor (CGImage image);

		[Export ("initWithTargetedCGImage:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGImage image, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:")]
		IntPtr Constructor (CIImage image);

		[Export ("initWithTargetedCIImage:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CIImage image, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:")]
		IntPtr Constructor (NSUrl imageUrl);

		[Export ("initWithTargetedImageURL:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl imageUrl, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:")]
		IntPtr Constructor (NSData imageData);

		[Export ("initWithTargetedImageData:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSData imageData, [NullAllowed] VNRequestCompletionHandler completionHandler);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageRegistrationRequest))]
	interface VNHomographicImageRegistrationRequest {

		// Inlined from parent class
		[Export ("initWithTargetedCVPixelBuffer:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer);

		[Export ("initWithTargetedCVPixelBuffer:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:")]
		IntPtr Constructor (CGImage image);

		[Export ("initWithTargetedCGImage:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGImage image, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:")]
		IntPtr Constructor (CIImage image);

		[Export ("initWithTargetedCIImage:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CIImage image, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:")]
		IntPtr Constructor (NSUrl imageUrl);

		[Export ("initWithTargetedImageURL:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl imageUrl, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:")]
		IntPtr Constructor (NSData imageData);

		[Export ("initWithTargetedImageData:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSData imageData, [NullAllowed] VNRequestCompletionHandler completionHandler);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface VNObservation : NSCopying, NSSecureCoding {

		[Export ("uuid", ArgumentSemantic.Strong)]
		NSUuid Uuid { get; }

		[Export ("confidence")]
		float Confidence { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNDetectedObjectObservation {

		[Static]
		[Export ("observationWithBoundingBox:")]
		VNDetectedObjectObservation FromBoundingBox (CGRect boundingBox);

		[Export ("boundingBox", ArgumentSemantic.Assign)]
		CGRect BoundingBox { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNDetectedObjectObservation))]
	interface VNFaceObservation {

		[NullAllowed, Export ("landmarks", ArgumentSemantic.Strong)]
		VNFaceLandmarks2D Landmarks { get; }

		[New]
		[Static]
		[Export ("observationWithBoundingBox:")]
		VNFaceObservation FromBoundingBox (CGRect boundingBox);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNClassificationObservation {

		[Export ("identifier")]
		string Identifier { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNCoreMLFeatureValueObservation {

		[Export ("featureValue", ArgumentSemantic.Copy)]
		MLFeatureValue FeatureValue { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNPixelBufferObservation {

		[Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNDetectedObjectObservation))]
	interface VNRectangleObservation {

		[Export ("topLeft", ArgumentSemantic.Assign)]
		CGPoint TopLeft { get; }

		[Export ("topRight", ArgumentSemantic.Assign)]
		CGPoint TopRight { get; }

		[Export ("bottomLeft", ArgumentSemantic.Assign)]
		CGPoint BottomLeft { get; }

		[Export ("bottomRight", ArgumentSemantic.Assign)]
		CGPoint BottomRight { get; }

		[New]
		[Static]
		[Export ("observationWithBoundingBox:")]
		VNRectangleObservation FromBoundingBox (CGRect boundingBox);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNDetectedObjectObservation))]
	interface VNTextObservation {

		[NullAllowed, Export ("characterBoxes", ArgumentSemantic.Copy)]
		VNRectangleObservation [] CharacterBoxes { get; }

		[New]
		[Static]
		[Export ("observationWithBoundingBox:")]
		VNTextObservation FromBoundingBox (CGRect boundingBox);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (VNRectangleObservation))]
	interface VNBarcodeObservation {

		[Protected]
		[Export ("symbology")]
		NSString WeakSymbology { get; }

		[Wrap ("VNBarcodeSymbologyExtensions.GetValue (WeakSymbology)")]
		VNBarcodeSymbology Symbology { get; }

		// TODO: Enable once CoreImage Xcode 9 is bound -> https://bugzilla.xamarin.com/show_bug.cgi?id=58197
		//[NullAllowed, Export ("barcodeDescriptor", ArgumentSemantic.Strong)]
		//CIBarcodeDescriptor BarcodeDescriptor { get; }

		[New]
		[Static]
		[Export ("observationWithBoundingBox:")]
		VNBarcodeObservation FromBoundingBox (CGRect boundingBox);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNHorizonObservation {

		[Export ("transform", ArgumentSemantic.Assign)]
		CGAffineTransform Transform { get; }

		[Export ("angle")]
		nfloat Angle { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNImageAlignmentObservation {
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageAlignmentObservation))]
	interface VNImageTranslationAlignmentObservation {

		[Export ("alignmentTransform", ArgumentSemantic.Assign)]
		CGAffineTransform AlignmentTransform { get; set; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageAlignmentObservation))]
	interface VNImageHomographicAlignmentObservation {

		[Export ("warpTransform", ArgumentSemantic.Assign)]
		Matrix3 WarpTransform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface VNRequest : NSCopying {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("preferBackgroundProcessing")]
		bool PreferBackgroundProcessing { get; set; }

		[NullAllowed, Export ("preferredMetalContext", ArgumentSemantic.Retain)]
		IMTLDevice PreferredMetalContext { get; set; }

		// From docs: VNObservation subclasses specific to the VNRequest subclass
		// Since downcasting is not easy we are exposing
		// this property as a generic method 'GetResults'.
		[Internal]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		IntPtr _Results { get; }

		[NullAllowed, Export ("completionHandler", ArgumentSemantic.Copy)]
		VNRequestCompletionHandler CompletionHandler { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (VNRequest))]
	interface VNImageBasedRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("regionOfInterest", ArgumentSemantic.Assign)]
		CGRect RegionOfInterest { get; set; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Internal]
	[Static]
	interface VNImageOptionKeys {
		[Field ("VNImageOptionProperties")]
		NSString PropertiesKey { get; }

		[Field ("VNImageOptionCameraIntrinsics")]
		NSString CameraIntrinsicsKey { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[StrongDictionary ("VNImageOptionKeys")]
	interface VNImageOptions {
		[Export ("PropertiesKey")] // Have the option to set your own dict
		NSDictionary WeakProperties { get; set; }

		[StrongDictionary] // Yep we need CoreGraphics to disambiguate
		CoreGraphics.CGImageProperties Properties { get; set; }

		NSData CameraIntrinsics { get; set; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNImageRequestHandler {

		[Export ("initWithCVPixelBuffer:options:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, NSDictionary options);

		[Wrap ("this (pixelBuffer, imageOptions?.Dictionary)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, VNImageOptions imageOptions);

		[Export ("initWithCVPixelBuffer:orientation:options:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary options);

		[Wrap ("this (pixelBuffer, orientation, imageOptions?.Dictionary)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[Export ("initWithCGImage:options:")]
		IntPtr Constructor (CGImage image, NSDictionary options);

		[Wrap ("this (image, imageOptions?.Dictionary)")]
		IntPtr Constructor (CGImage image, VNImageOptions imageOptions);

		[Export ("initWithCGImage:orientation:options:")]
		IntPtr Constructor (CGImage image, CGImagePropertyOrientation orientation, NSDictionary options);

		[Wrap ("this (image, orientation, imageOptions?.Dictionary)")]
		IntPtr Constructor (CGImage image, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[Export ("initWithCIImage:options:")]
		IntPtr Constructor (CIImage image, NSDictionary options);

		[Wrap ("this (image, imageOptions?.Dictionary)")]
		IntPtr Constructor (CIImage image, VNImageOptions imageOptions);

		[Export ("initWithCIImage:orientation:options:")]
		IntPtr Constructor (CIImage image, CGImagePropertyOrientation orientation, NSDictionary options);

		[Wrap ("this (image, orientation, imageOptions?.Dictionary)")]
		IntPtr Constructor (CIImage image, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[Export ("initWithURL:options:")]
		IntPtr Constructor (NSUrl imageUrl, NSDictionary options);

		[Wrap ("this (imageUrl, imageOptions?.Dictionary)")]
		IntPtr Constructor (NSUrl imageUrl, VNImageOptions imageOptions);

		[Export ("initWithURL:orientation:options:")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary options);

		[Wrap ("this (imageUrl, orientation, imageOptions?.Dictionary)")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[Export ("initWithData:options:")]
		IntPtr Constructor (NSData imageData, NSDictionary options);

		[Wrap ("this (imageData, imageOptions?.Dictionary)")]
		IntPtr Constructor (NSData imageData, VNImageOptions imageOptions);

		[Export ("initWithData:orientation:options:")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary options);

		[Wrap ("this (imageData, orientation, imageOptions?.Dictionary)")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[Export ("performRequests:error:")]
		bool Perform (VNRequest [] requests, out NSError error);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface VNSequenceRequestHandler {

		[Export ("performRequests:onCVPixelBuffer:error:")]
		bool Perform (VNRequest [] requests, CVPixelBuffer pixelBuffer, out NSError error);

		[Export ("performRequests:onCVPixelBuffer:orientation:error:")]
		bool Perform (VNRequest [] requests, CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, out NSError error);

		[Export ("performRequests:onCGImage:error:")]
		bool Perform (VNRequest [] requests, CGImage image, out NSError error);

		[Export ("performRequests:onCGImage:orientation:error:")]
		bool Perform (VNRequest [] requests, CGImage image, CGImagePropertyOrientation orientation, out NSError error);

		[Export ("performRequests:onCIImage:error:")]
		bool Perform (VNRequest [] requests, CIImage image, out NSError error);

		[Export ("performRequests:onCIImage:orientation:error:")]
		bool Perform (VNRequest [] requests, CIImage image, CGImagePropertyOrientation orientation, out NSError error);

		[Export ("performRequests:onImageURL:error:")]
		bool Perform (VNRequest [] requests, NSUrl imageUrl, out NSError error);

		[Export ("performRequests:onImageURL:orientation:error:")]
		bool Perform (VNRequest [] requests, NSUrl imageUrl, CGImagePropertyOrientation orientation, out NSError error);

		[Export ("performRequests:onImageData:error:")]
		bool Perform (VNRequest [] requests, NSData imageData, out NSError error);

		[Export ("performRequests:onImageData:orientation:error:")]
		bool Perform (VNRequest [] requests, NSData imageData, CGImagePropertyOrientation orientation, out NSError error);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNTargetedImageRequest {

		[Export ("initWithTargetedCVPixelBuffer:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer);

		[Export ("initWithTargetedCVPixelBuffer:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:")]
		IntPtr Constructor (CGImage image);

		[Export ("initWithTargetedCGImage:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGImage image, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:")]
		IntPtr Constructor (CIImage image);

		[Export ("initWithTargetedCIImage:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CIImage image, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:")]
		IntPtr Constructor (NSUrl imageUrl);

		[Export ("initWithTargetedImageURL:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl imageUrl, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:")]
		IntPtr Constructor (NSData imageData);

		[Export ("initWithTargetedImageData:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSData imageData, [NullAllowed] VNRequestCompletionHandler completionHandler);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (VNTrackingRequest))]
	[DisableDefaultCtor]
	interface VNTrackObjectRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithDetectedObjectObservation:")]
		IntPtr Constructor (VNDetectedObjectObservation observation);

		[Export ("initWithDetectedObjectObservation:completionHandler:")]
		IntPtr Constructor (VNDetectedObjectObservation observation, [NullAllowed] VNRequestCompletionHandler completionHandler);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (VNTrackingRequest))]
	[DisableDefaultCtor]
	interface VNTrackRectangleRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithRectangleObservation:")]
		IntPtr Constructor (VNRectangleObservation observation);

		[Export ("initWithRectangleObservation:completionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor (VNRectangleObservation observation, [NullAllowed] VNRequestCompletionHandler completionHandler);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNTrackingRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("inputObservation", ArgumentSemantic.Strong)]
		VNDetectedObjectObservation InputObservation { get; set; }

		[Export ("trackingLevel", ArgumentSemantic.Assign)]
		VNRequestTrackingLevel TrackingLevel { get; set; }

		[Export ("lastFrame")]
		bool LastFrame { [Bind ("isLastFrame")] get; set; }
	}
}
#endif
