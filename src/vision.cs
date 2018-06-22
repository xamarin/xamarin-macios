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
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreML;
using CoreVideo;
using Foundation;
using Metal;
using ObjCRuntime;
using ImageIO;

using Matrix3 = global::OpenTK.NMatrix3;

namespace Vision {

	[ErrorDomain ("VNErrorDomain")]
	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Native]
	enum VNErrorCode : long {
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
		UnsupportedRevision,
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Native]
	enum VNRequestTrackingLevel : ulong {
		Accurate = 0,
		Fast,
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Native]
	enum VNImageCropAndScaleOption : ulong {
		CenterCrop = 0,
		ScaleFit = 1,
		ScaleFill,
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	enum VNBarcodeSymbology {
		[Field ("VNBarcodeSymbologyAztec")]
		Aztec,

		[Field ("VNBarcodeSymbologyCode39")]
		Code39,

		[Field ("VNBarcodeSymbologyCode39Checksum")]
		Code39Checksum,

		[Field ("VNBarcodeSymbologyCode39FullASCII")]
		Code39FullAscii,

		[Field ("VNBarcodeSymbologyCode39FullASCIIChecksum")]
		Code39FullAsciiChecksum,

		[Field ("VNBarcodeSymbologyCode93")]
		Code93,

		[Field ("VNBarcodeSymbologyCode93i")]
		Code93i,

		[Field ("VNBarcodeSymbologyCode128")]
		Code128,

		[Field ("VNBarcodeSymbologyDataMatrix")]
		DataMatrix,

		[Field ("VNBarcodeSymbologyEAN8")]
		Ean8,

		[Field ("VNBarcodeSymbologyEAN13")]
		Ean13,

		[Field ("VNBarcodeSymbologyI2of5")]
		I2OF5,

		[Field ("VNBarcodeSymbologyI2of5Checksum")]
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

	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	[Native]
	enum VNRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		Two = 2,
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

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
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

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectFaceLandmarksRequest : VNFaceObservationAccepting {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectFaceRectanglesRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectHorizonRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
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

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
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

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
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
		[Export ("normalizedPoints")]
		IntPtr _GetNormalizedPoints ();

		[Internal]
		[Export ("pointsInImageOfSize:")]
		IntPtr _GetPointsInImage (CGSize imageSize);
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

		// Inlined from parent class
		[Export ("initWithTargetedCVPixelBuffer:options:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, options?.Dictionary)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:options:completionHandler:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, orientation, options?.Dictionary)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:completionHandler:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:options:")]
		IntPtr Constructor (CGImage cgImage, NSDictionary optionsDict);

		[Wrap ("this (cgImage, options?.Dictionary)")]
		IntPtr Constructor (CGImage cgImage, VNImageOptions options);

		[Export ("initWithTargetedCGImage:options:completionHandler:")]
		IntPtr Constructor (CGImage cgImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CGImage cgImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:orientation:options:")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (cgImage, orientation, options?.Dictionary)")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCGImage:orientation:options:completionHandler:")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:options:")]
		IntPtr Constructor (CIImage ciImage, NSDictionary optionsDict);

		[Wrap ("this (ciImage, options?.Dictionary)")]
		IntPtr Constructor (CIImage ciImage, VNImageOptions options);

		[Export ("initWithTargetedCIImage:options:completionHandler:")]
		IntPtr Constructor (CIImage ciImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CIImage ciImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:orientation:options:")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (ciImage, orientation, options?.Dictionary)")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCIImage:orientation:options:completionHandler:")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:options:")]
		IntPtr Constructor (NSUrl imageUrl, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, options?.Dictionary)")]
		IntPtr Constructor (NSUrl imageUrl, VNImageOptions options);

		[Export ("initWithTargetedImageURL:options:completionHandler:")]
		IntPtr Constructor (NSUrl imageUrl, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSUrl imageUrl, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:orientation:options:")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, orientation, options?.Dictionary)")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageURL:orientation:options:completionHandler:")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:options:")]
		IntPtr Constructor (NSData imageData, NSDictionary optionsDict);

		[Wrap ("this (imageData, options?.Dictionary)")]
		IntPtr Constructor (NSData imageData, VNImageOptions options);

		[Export ("initWithTargetedImageData:options:completionHandler:")]
		IntPtr Constructor (NSData imageData, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSData imageData, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:orientation:options:")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageData, orientation, options?.Dictionary)")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageData:orientation:options:completionHandler:")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageRegistrationRequest))]
	interface VNTranslationalImageRegistrationRequest {

		// Inlined from parent class
		[Export ("initWithTargetedCVPixelBuffer:options:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, options?.Dictionary)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:options:completionHandler:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, orientation, options?.Dictionary)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:completionHandler:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:options:")]
		IntPtr Constructor (CGImage cgImage, NSDictionary optionsDict);

		[Wrap ("this (cgImage, options?.Dictionary)")]
		IntPtr Constructor (CGImage cgImage, VNImageOptions options);

		[Export ("initWithTargetedCGImage:options:completionHandler:")]
		IntPtr Constructor (CGImage cgImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CGImage cgImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:orientation:options:")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (cgImage, orientation, options?.Dictionary)")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCGImage:orientation:options:completionHandler:")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:options:")]
		IntPtr Constructor (CIImage ciImage, NSDictionary optionsDict);

		[Wrap ("this (ciImage, options?.Dictionary)")]
		IntPtr Constructor (CIImage ciImage, VNImageOptions options);

		[Export ("initWithTargetedCIImage:options:completionHandler:")]
		IntPtr Constructor (CIImage ciImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CIImage ciImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:orientation:options:")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (ciImage, orientation, options?.Dictionary)")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCIImage:orientation:options:completionHandler:")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:options:")]
		IntPtr Constructor (NSUrl imageUrl, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, options?.Dictionary)")]
		IntPtr Constructor (NSUrl imageUrl, VNImageOptions options);

		[Export ("initWithTargetedImageURL:options:completionHandler:")]
		IntPtr Constructor (NSUrl imageUrl, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSUrl imageUrl, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:orientation:options:")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, orientation, options?.Dictionary)")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageURL:orientation:options:completionHandler:")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:options:")]
		IntPtr Constructor (NSData imageData, NSDictionary optionsDict);

		[Wrap ("this (imageData, options?.Dictionary)")]
		IntPtr Constructor (NSData imageData, VNImageOptions options);

		[Export ("initWithTargetedImageData:options:completionHandler:")]
		IntPtr Constructor (NSData imageData, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSData imageData, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:orientation:options:")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageData, orientation, options?.Dictionary)")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageData:orientation:options:completionHandler:")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageRegistrationRequest))]
	interface VNHomographicImageRegistrationRequest {

		// Inlined from parent class
		[Export ("initWithTargetedCVPixelBuffer:options:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, options?.Dictionary)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:options:completionHandler:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, orientation, options?.Dictionary)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:completionHandler:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:options:")]
		IntPtr Constructor (CGImage cgImage, NSDictionary optionsDict);

		[Wrap ("this (cgImage, options?.Dictionary)")]
		IntPtr Constructor (CGImage cgImage, VNImageOptions options);

		[Export ("initWithTargetedCGImage:options:completionHandler:")]
		IntPtr Constructor (CGImage cgImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CGImage cgImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:orientation:options:")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (cgImage, orientation, options?.Dictionary)")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCGImage:orientation:options:completionHandler:")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:options:")]
		IntPtr Constructor (CIImage ciImage, NSDictionary optionsDict);

		[Wrap ("this (ciImage, options?.Dictionary)")]
		IntPtr Constructor (CIImage ciImage, VNImageOptions options);

		[Export ("initWithTargetedCIImage:options:completionHandler:")]
		IntPtr Constructor (CIImage ciImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CIImage ciImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:orientation:options:")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (ciImage, orientation, options?.Dictionary)")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCIImage:orientation:options:completionHandler:")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:options:")]
		IntPtr Constructor (NSUrl imageUrl, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, options?.Dictionary)")]
		IntPtr Constructor (NSUrl imageUrl, VNImageOptions options);

		[Export ("initWithTargetedImageURL:options:completionHandler:")]
		IntPtr Constructor (NSUrl imageUrl, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSUrl imageUrl, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:orientation:options:")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, orientation, options?.Dictionary)")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageURL:orientation:options:completionHandler:")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:options:")]
		IntPtr Constructor (NSData imageData, NSDictionary optionsDict);

		[Wrap ("this (imageData, options?.Dictionary)")]
		IntPtr Constructor (NSData imageData, VNImageOptions options);

		[Export ("initWithTargetedImageData:options:completionHandler:")]
		IntPtr Constructor (NSData imageData, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSData imageData, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:orientation:options:")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageData, orientation, options?.Dictionary)")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageData:orientation:options:completionHandler:")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface VNObservation : NSCopying, NSSecureCoding, VNRequestRevisionProviding {

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

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNDetectedObjectObservation FromBoundingBox (VNRequestRevision requestRevision, CGRect boundingBox);

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

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNFaceObservation FromBoundingBox (VNRequestRevision requestRevision, CGRect boundingBox);

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Static]
		[Export ("faceObservationWithRequestRevision:boundingBox:roll:yaw:")]
		VNFaceObservation FromBoundingBox (VNRequestRevision requestRevision, CGRect boundingBox, [NullAllowed] [BindAs (typeof (nfloat?))] NSNumber roll, [NullAllowed] [BindAs (typeof (nfloat?))] NSNumber yaw);

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[BindAs (typeof (nfloat?))]
		[NullAllowed, Export ("roll", ArgumentSemantic.Strong)]
		NSNumber Roll { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[BindAs (typeof (nfloat?))]
		[NullAllowed, Export ("yaw", ArgumentSemantic.Strong)]
		NSNumber Yaw { get; }
	}

	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	[BaseType (typeof (VNDetectedObjectObservation))]
	[DisableDefaultCtor]
	interface VNRecognizedObjectObservation {

		[New]
		[Static]
		[Export ("observationWithBoundingBox:")]
		VNRecognizedObjectObservation FromBoundingBox (CGRect boundingBox);

		[New]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNRecognizedObjectObservation FromBoundingBox (VNRequestRevision requestRevision, CGRect boundingBox);

		[Export ("labels", ArgumentSemantic.Copy)]
		VNClassificationObservation [] Labels { get; }
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

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNRectangleObservation FromBoundingBox (VNRequestRevision requestRevision, CGRect boundingBox);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNRectangleObservation))]
	interface VNTextObservation {

		[NullAllowed, Export ("characterBoxes", ArgumentSemantic.Copy)]
		VNRectangleObservation [] CharacterBoxes { get; }

		[New]
		[Static]
		[Export ("observationWithBoundingBox:")]
		VNTextObservation FromBoundingBox (CGRect boundingBox);

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNTextObservation FromBoundingBox (VNRequestRevision requestRevision, CGRect boundingBox);
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (VNRectangleObservation))]
	interface VNBarcodeObservation {

		[Protected]
		[Export ("symbology")]
		NSString WeakSymbology { get; }

		[Wrap ("VNBarcodeSymbologyExtensions.GetValue (WeakSymbology)")]
		VNBarcodeSymbology Symbology { get; }

		[NullAllowed, Export ("barcodeDescriptor", ArgumentSemantic.Strong)]
		CIBarcodeDescriptor BarcodeDescriptor { get; }

		[New]
		[Static]
		[Export ("observationWithBoundingBox:")]
		VNBarcodeObservation FromBoundingBox (CGRect boundingBox);

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNBarcodeObservation FromBoundingBox (VNRequestRevision requestRevision, CGRect boundingBox);

		[NullAllowed, Export ("payloadStringValue")]
		string PayloadStringValue { get; }
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

		[Export ("usesCPUOnly")]
		bool UsesCpuOnly { get; set; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("revision")]
		VNRequestRevision Revision { get; set; }

		// We must inline the following static properties into subclasses so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
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

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Internal]
	[Static]
	interface VNImageOptionKeys {
		[Field ("VNImageOptionProperties")]
		NSString PropertiesKey { get; }

		[Field ("VNImageOptionCameraIntrinsics")]
		NSString CameraIntrinsicsKey { get; }

		[Field ("VNImageOptionCIContext")]
		NSString CIContextKey { get; }
	}

	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[StrongDictionary ("VNImageOptionKeys")]
	interface VNImageOptions {
		[Export ("PropertiesKey")] // Have the option to set your own dict
		NSDictionary WeakProperties { get; set; }

		[StrongDictionary] // Yep we need CoreGraphics to disambiguate
		CoreGraphics.CGImageProperties Properties { get; set; }

		NSData CameraIntrinsics { get; set; }
		CIContext CIContext { get; set; }
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
	[DisableDefaultCtor] // it's a designated initializer
	interface VNSequenceRequestHandler {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

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

		[Export ("initWithTargetedCVPixelBuffer:options:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, options?.Dictionary)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:options:completionHandler:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, orientation, options?.Dictionary)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:completionHandler:")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:options:")]
		IntPtr Constructor (CGImage cgImage, NSDictionary optionsDict);

		[Wrap ("this (cgImage, options?.Dictionary)")]
		IntPtr Constructor (CGImage cgImage, VNImageOptions options);

		[Export ("initWithTargetedCGImage:options:completionHandler:")]
		IntPtr Constructor (CGImage cgImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CGImage cgImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:orientation:options:")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (cgImage, orientation, options?.Dictionary)")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCGImage:orientation:options:completionHandler:")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:options:")]
		IntPtr Constructor (CIImage ciImage, NSDictionary optionsDict);

		[Wrap ("this (ciImage, options?.Dictionary)")]
		IntPtr Constructor (CIImage ciImage, VNImageOptions options);

		[Export ("initWithTargetedCIImage:options:completionHandler:")]
		IntPtr Constructor (CIImage ciImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CIImage ciImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:orientation:options:")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (ciImage, orientation, options?.Dictionary)")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCIImage:orientation:options:completionHandler:")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:options:")]
		IntPtr Constructor (NSUrl imageUrl, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, options?.Dictionary)")]
		IntPtr Constructor (NSUrl imageUrl, VNImageOptions options);

		[Export ("initWithTargetedImageURL:options:completionHandler:")]
		IntPtr Constructor (NSUrl imageUrl, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSUrl imageUrl, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:orientation:options:")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, orientation, options?.Dictionary)")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageURL:orientation:options:completionHandler:")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:options:")]
		IntPtr Constructor (NSData imageData, NSDictionary optionsDict);

		[Wrap ("this (imageData, options?.Dictionary)")]
		IntPtr Constructor (NSData imageData, VNImageOptions options);

		[Export ("initWithTargetedImageData:options:completionHandler:")]
		IntPtr Constructor (NSData imageData, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSData imageData, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:orientation:options:")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageData, orientation, options?.Dictionary)")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageData:orientation:options:completionHandler:")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, orientation, options?.Dictionary, completionHandler)")]
		IntPtr Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
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

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
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

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
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

		// We must inline the following static properties so the correct class_ptr is used.
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		VNRequestRevision [] SupportedRevisions { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("defaultRevision")]
		VNRequestRevision DefaultRevision { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[New]
		[Static]
		[Export ("currentRevision")]
		VNRequestRevision CurrentRevision { get; }
	}

	[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	[Protocol]
	interface VNRequestRevisionProviding {

		[Abstract]
		[Export ("requestRevision")]
		VNRequestRevision RequestRevision { get; }
	}
}
#endif
