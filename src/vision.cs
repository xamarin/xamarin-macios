//
// Vision C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreML;
using CoreVideo;
using Foundation;
using Metal;
using ObjCRuntime;
using ImageIO;
using AVFoundation;

#if NET
using Vector2 = global::System.Numerics.Vector2;
using Vector3 = global::System.Numerics.Vector3;
using Vector4 = global::System.Numerics.Vector4;
using Matrix3 = global::CoreGraphics.NMatrix3;
#else
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;
using Matrix3 = global::OpenTK.NMatrix3;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Vision {

	[ErrorDomain ("VNErrorDomain")]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNErrorCode : long {
		TuriCore = -1,
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
		DataUnavailable,
		TimeStampNotFound,
		UnsupportedRequest,
		Timeout,
		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		UnsupportedComputeStage,
		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		UnsupportedComputeDevice,
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum VNRequestTrackingLevel : ulong {
		Accurate = 0,
		Fast,
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum VNImageCropAndScaleOption : ulong {
		CenterCrop = 0,
		ScaleFit = 1,
		ScaleFill = 2,
		[TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		ScaleFitRotate90Ccw = 256 + ScaleFit,
		[TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		ScaleFillRotate90Ccw = 256 + ScaleFill,
	}

	[MacCatalyst (13, 1)]
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

		[iOS (15, 0), Mac (12, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("VNBarcodeSymbologyCodabar")]
		Codabar,

		[iOS (15, 0), Mac (12, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("VNBarcodeSymbologyGS1DataBar")]
		GS1DataBar,

		[iOS (15, 0), Mac (12, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("VNBarcodeSymbologyGS1DataBarExpanded")]
		GS1DataBarExpanded,

		[iOS (15, 0), Mac (12, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("VNBarcodeSymbologyGS1DataBarLimited")]
		GS1DataBarLimited,

		[iOS (15, 0), Mac (12, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("VNBarcodeSymbologyMicroPDF417")]
		MicroPdf417,

		[iOS (15, 0), Mac (12, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("VNBarcodeSymbologyMicroQR")]
		MicroQR,

		[iOS (17, 0), Mac (14, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("VNBarcodeSymbologyMSIPlessey")]
		MsiPlessey,
	}

	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum VNChirality : long {
		Unknown = 0,
		Left = -1,
		Right = 1,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		Two = 2,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNCoreMLRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (12, 0), iOS (12, 0), MacCatalyst (15, 0)]
	[Native]
	enum VNDetectBarcodesRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		[TV (15, 0), Mac (12, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		Two = 2,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNDetectFaceLandmarksRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		Two = 2,
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Three = 3,
	}

	[TV (12, 0), iOS (12, 0), MacCatalyst (15, 0)]
	[Native]
	enum VNDetectFaceRectanglesRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		Two = 2,
		[TV (15, 0), Mac (12, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		Three = 3,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNDetectHorizonRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNDetectRectanglesRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNDetectTextRectanglesRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNTranslationalImageRegistrationRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNHomographicImageRegistrationRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNTrackObjectRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Two = 2,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNTrackRectangleRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNDetectedObjectObservationRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		Two = 2,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNFaceObservationRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		Two = 2,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNRecognizedObjectObservationRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		Two = 2,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNRectangleObservationRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		Two = 2,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNTextObservationRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		Two = 2,
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNBarcodeObservationRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		Two = 2,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNClassifyImageRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	enum VNDetectDocumentSegmentationRequestRevision : ulong {
		One = 1,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNDetectFaceCaptureQualityRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Two = 2,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNRequestFaceLandmarksConstellation : ulong {
		NotDefined = 0,
		SixtyFivePoints,
		SeventySixPoints,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNDetectHumanRectanglesRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		Two = 2,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNGenerateAttentionBasedSaliencyImageRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNGenerateImageFeaturePrintRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNGenerateObjectnessBasedSaliencyImageRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNRecognizeAnimalsRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		Two = 2,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNRequestTextRecognitionLevel : long {
		Accurate = 0,
		Fast,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNRecognizeTextRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Two = 2,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum VNElementType : ulong {
		Unknown = 0,
		Float = 1,
		Double = 2,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum VNDetectContourRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum VNDetectHumanBodyPoseRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum VNDetectHumanHandPoseRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum VNDetectTrajectoriesRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum VNGenerateOpticalFlowRequestComputationAccuracy : ulong {
		Low = 0,
		Medium,
		High,
		VeryHigh,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum VNGenerateOpticalFlowRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	enum VNGeneratePersonSegmentationRequestRevision : ulong {
		One = 1,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	enum VNStatefulRequestRevision : ulong {
		Unspecified = 0,
		One = 1,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	enum VNHumanBodyPoseObservationJointName {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("VNHumanBodyPoseObservationJointNameNose")]
		Nose,

		[Field ("VNHumanBodyPoseObservationJointNameLeftEye")]
		LeftEye,

		[Field ("VNHumanBodyPoseObservationJointNameRightEye")]
		RightEye,

		[Field ("VNHumanBodyPoseObservationJointNameLeftEar")]
		LeftEar,

		[Field ("VNHumanBodyPoseObservationJointNameRightEar")]
		RightEar,

		[Field ("VNHumanBodyPoseObservationJointNameLeftShoulder")]
		LeftShoulder,

		[Field ("VNHumanBodyPoseObservationJointNameRightShoulder")]
		RightShoulder,

		[Field ("VNHumanBodyPoseObservationJointNameNeck")]
		Neck,

		[Field ("VNHumanBodyPoseObservationJointNameLeftElbow")]
		LeftElbow,

		[Field ("VNHumanBodyPoseObservationJointNameRightElbow")]
		RightElbow,

		[Field ("VNHumanBodyPoseObservationJointNameLeftWrist")]
		LeftWrist,

		[Field ("VNHumanBodyPoseObservationJointNameRightWrist")]
		RightWrist,

		[Field ("VNHumanBodyPoseObservationJointNameLeftHip")]
		LeftHip,

		[Field ("VNHumanBodyPoseObservationJointNameRightHip")]
		RightHip,

		[Field ("VNHumanBodyPoseObservationJointNameRoot")]
		Root,

		[Field ("VNHumanBodyPoseObservationJointNameLeftKnee")]
		LeftKnee,

		[Field ("VNHumanBodyPoseObservationJointNameRightKnee")]
		RightKnee,

		[Field ("VNHumanBodyPoseObservationJointNameLeftAnkle")]
		LeftAnkle,

		[Field ("VNHumanBodyPoseObservationJointNameRightAnkle")]
		RightAnkle,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	enum VNHumanBodyPoseObservationJointsGroupName {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("VNHumanBodyPoseObservationJointsGroupNameFace")]
		Face,

		[Field ("VNHumanBodyPoseObservationJointsGroupNameTorso")]
		Torso,

		[Field ("VNHumanBodyPoseObservationJointsGroupNameLeftArm")]
		LeftArm,

		[Field ("VNHumanBodyPoseObservationJointsGroupNameRightArm")]
		RightArm,

		[Field ("VNHumanBodyPoseObservationJointsGroupNameLeftLeg")]
		LeftLeg,

		[Field ("VNHumanBodyPoseObservationJointsGroupNameRightLeg")]
		RightLeg,

		[Field ("VNHumanBodyPoseObservationJointsGroupNameAll")]
		All,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	enum VNHumanHandPoseObservationJointName {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("VNHumanHandPoseObservationJointNameWrist")]
		Wrist,

		[Field ("VNHumanHandPoseObservationJointNameThumbCMC")]
		ThumbCmc,

		[Field ("VNHumanHandPoseObservationJointNameThumbMP")]
		ThumbMP,

		[Field ("VNHumanHandPoseObservationJointNameThumbIP")]
		ThumbIP,

		[Field ("VNHumanHandPoseObservationJointNameThumbTip")]
		ThumbTip,

		[Field ("VNHumanHandPoseObservationJointNameIndexMCP")]
		IndexMcp,

		[Field ("VNHumanHandPoseObservationJointNameIndexPIP")]
		IndexPip,

		[Field ("VNHumanHandPoseObservationJointNameIndexDIP")]
		IndexDip,

		[Field ("VNHumanHandPoseObservationJointNameIndexTip")]
		IndexTip,

		[Field ("VNHumanHandPoseObservationJointNameMiddleMCP")]
		MiddleMcp,

		[Field ("VNHumanHandPoseObservationJointNameMiddlePIP")]
		MiddlePip,

		[Field ("VNHumanHandPoseObservationJointNameMiddleDIP")]
		MiddleDip,

		[Field ("VNHumanHandPoseObservationJointNameMiddleTip")]
		MiddleTip,

		[Field ("VNHumanHandPoseObservationJointNameRingMCP")]
		RingMcp,

		[Field ("VNHumanHandPoseObservationJointNameRingPIP")]
		RingPip,

		[Field ("VNHumanHandPoseObservationJointNameRingDIP")]
		RingDip,

		[Field ("VNHumanHandPoseObservationJointNameRingTip")]
		RingTip,

		[Field ("VNHumanHandPoseObservationJointNameLittleMCP")]
		LittleMcp,

		[Field ("VNHumanHandPoseObservationJointNameLittlePIP")]
		LittlePip,

		[Field ("VNHumanHandPoseObservationJointNameLittleDIP")]
		LittleDip,

		[Field ("VNHumanHandPoseObservationJointNameLittleTip")]
		LittleTip,
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	enum VNHumanHandPoseObservationJointsGroupName {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("VNHumanHandPoseObservationJointsGroupNameThumb")]
		Thumb,

		[Field ("VNHumanHandPoseObservationJointsGroupNameIndexFinger")]
		IndexFinger,

		[Field ("VNHumanHandPoseObservationJointsGroupNameMiddleFinger")]
		MiddleFinger,

		[Field ("VNHumanHandPoseObservationJointsGroupNameRingFinger")]
		RingFinger,

		[Field ("VNHumanHandPoseObservationJointsGroupNameLittleFinger")]
		LittleFinger,

		[Field ("VNHumanHandPoseObservationJointsGroupNameAll")]
		All,
	}

	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum VNGeneratePersonSegmentationRequestQualityLevel : ulong {
		Accurate = 0,
		Balanced,
		Fast,
	}

	[TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum VNPointsClassification : long {
		Disconnected = 0,
		OpenPath,
		ClosedPath,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum VNAnimalBodyPoseObservationJointName {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("VNAnimalBodyPoseObservationJointNameLeftEarTop")]
		LeftEarTop,

		[Field ("VNAnimalBodyPoseObservationJointNameRightEarTop")]
		RightEarTop,

		[Field ("VNAnimalBodyPoseObservationJointNameLeftEarMiddle")]
		LeftEarMiddle,

		[Field ("VNAnimalBodyPoseObservationJointNameRightEarMiddle")]
		RightEarMiddle,

		[Field ("VNAnimalBodyPoseObservationJointNameLeftEarBottom")]
		LeftEarBottom,

		[Field ("VNAnimalBodyPoseObservationJointNameRightEarBottom")]
		RightEarBottom,

		[Field ("VNAnimalBodyPoseObservationJointNameLeftEye")]
		LeftEye,

		[Field ("VNAnimalBodyPoseObservationJointNameRightEye")]
		RightEye,

		[Field ("VNAnimalBodyPoseObservationJointNameNose")]
		Nose,

		[Field ("VNAnimalBodyPoseObservationJointNameNeck")]
		Neck,

		[Field ("VNAnimalBodyPoseObservationJointNameLeftFrontElbow")]
		LeftFrontElbow,

		[Field ("VNAnimalBodyPoseObservationJointNameRightFrontElbow")]
		RightFrontElbow,

		[Field ("VNAnimalBodyPoseObservationJointNameLeftFrontKnee")]
		LeftFrontKnee,

		[Field ("VNAnimalBodyPoseObservationJointNameRightFrontKnee")]
		RightFrontKnee,

		[Field ("VNAnimalBodyPoseObservationJointNameLeftFrontPaw")]
		LeftFrontPaw,

		[Field ("VNAnimalBodyPoseObservationJointNameRightFrontPaw")]
		RightFrontPaw,

		[Field ("VNAnimalBodyPoseObservationJointNameLeftBackElbow")]
		LeftBackElbo,

		[Field ("VNAnimalBodyPoseObservationJointNameRightBackElbow")]
		RightBackElbow,

		[Field ("VNAnimalBodyPoseObservationJointNameLeftBackKnee")]
		LeftBackKnee,

		[Field ("VNAnimalBodyPoseObservationJointNameRightBackKnee")]
		RightBackKnee,

		[Field ("VNAnimalBodyPoseObservationJointNameLeftBackPaw")]
		LeftBackPaw,

		[Field ("VNAnimalBodyPoseObservationJointNameRightBackPaw")]
		RightBackPaw,

		[Field ("VNAnimalBodyPoseObservationJointNameTailTop")]
		TailTop,

		[Field ("VNAnimalBodyPoseObservationJointNameTailMiddle")]
		TailMiddle,

		[Field ("VNAnimalBodyPoseObservationJointNameTailBottom")]
		TailBottom,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum VNAnimalBodyPoseObservationJointsGroupName {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("VNAnimalBodyPoseObservationJointsGroupNameHead")]
		Head,

		[Field ("VNAnimalBodyPoseObservationJointsGroupNameTrunk")]
		Trunk,

		[Field ("VNAnimalBodyPoseObservationJointsGroupNameForelegs")]
		Forelegs,

		[Field ("VNAnimalBodyPoseObservationJointsGroupNameHindlegs")]
		Hindlegs,

		[Field ("VNAnimalBodyPoseObservationJointsGroupNameTail")]
		Tail,

		[Field ("VNAnimalBodyPoseObservationJointsGroupNameAll")]
		All,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum VNHumanBodyPose3DObservationJointName {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("VNHumanBodyPose3DObservationJointNameRoot")]
		Root,

		[Field ("VNHumanBodyPose3DObservationJointNameRightHip")]
		RightHip,

		[Field ("VNHumanBodyPose3DObservationJointNameRightKnee")]
		RightKnee,

		[Field ("VNHumanBodyPose3DObservationJointNameRightAnkle")]
		RightAnkle,

		[Field ("VNHumanBodyPose3DObservationJointNameRightShoulder")]
		RightShoulder,

		[Field ("VNHumanBodyPose3DObservationJointNameLeftHip")]
		LeftHip,

		[Field ("VNHumanBodyPose3DObservationJointNameLeftKnee")]
		LeftKnee,

		[Field ("VNHumanBodyPose3DObservationJointNameLeftAnkle")]
		LeftAnkle,

		[Field ("VNHumanBodyPose3DObservationJointNameSpine")]
		Spine,

		[Field ("VNHumanBodyPose3DObservationJointNameCenterShoulder")]
		CenterShoulder,

		[Field ("VNHumanBodyPose3DObservationJointNameCenterHead")]
		CenterHead,

		[Field ("VNHumanBodyPose3DObservationJointNameTopHead")]
		TopHead,

		[Field ("VNHumanBodyPose3DObservationJointNameLeftShoulder")]
		LeftShoulder,

		[Field ("VNHumanBodyPose3DObservationJointNameLeftElbow")]
		LeftElbow,

		[Field ("VNHumanBodyPose3DObservationJointNameLeftWrist")]
		LeftWrist,

		[Field ("VNHumanBodyPose3DObservationJointNameRightElbow")]
		RightElbow,

		[Field ("VNHumanBodyPose3DObservationJointNameRightWrist")]
		RightWrist,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum VNHumanBodyPose3DObservationJointsGroupName {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("VNHumanBodyPose3DObservationJointsGroupNameHead")]
		Head,

		[Field ("VNHumanBodyPose3DObservationJointsGroupNameTorso")]
		Torso,

		[Field ("VNHumanBodyPose3DObservationJointsGroupNameLeftArm")]
		LeftArm,

		[Field ("VNHumanBodyPose3DObservationJointsGroupNameRightArm")]
		RightArm,

		[Field ("VNHumanBodyPose3DObservationJointsGroupNameLeftLeg")]
		LeftLeg,

		[Field ("VNHumanBodyPose3DObservationJointsGroupNameRightLeg")]
		RightLeg,

		[Field ("VNHumanBodyPose3DObservationJointsGroupNameAll")]
		All,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum VNComputeStage {
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Field ("VNComputeStageMain")]
		Main,

		[Field ("VNComputeStagePostProcessing")]
		PostProcessing,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum VNBarcodeCompositeType : long {
		None = 0,
		Linked,
		Gs1TypeA,
		Gs1TypeB,
		Gs1TypeC,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum VNHumanBodyPose3DObservationHeightEstimation : long {
		Reference = 0,
		Measured,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum VNTrackOpticalFlowRequestComputationAccuracy : ulong {
		Low = 0,
		Medium,
		High,
		VeryHigh,
	}

	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (VNDetectedObjectObservation))]
	[DisableDefaultCtor]
	interface VNHumanObservation {
		[NullAllowed]
		[Export ("upperBodyOnly")]
		bool UpperBodyOnly { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNCoreMLModel {

		[Static]
		[Export ("modelForMLModel:error:")]
		[return: NullAllowed]
		VNCoreMLModel FromMLModel (MLModel model, out NSError error);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("inputImageFeatureName")]
		string InputImageFeatureName { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("featureProvider", ArgumentSemantic.Strong)]
		IMLFeatureProvider FeatureProvider { get; set; }
	}

	[MacCatalyst (13, 1)]
	delegate void VNRequestCompletionHandler (VNRequest request, NSError error);

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNCoreMLRequest {

		[Export ("model")]
		VNCoreMLModel Model { get; }

		[Export ("imageCropAndScaleOption", ArgumentSemantic.Assign)]
		VNImageCropAndScaleOption ImageCropAndScaleOption { get; set; }

		[Export ("initWithModel:")]
		NativeHandle Constructor (VNCoreMLModel model);

		[Export ("initWithModel:completionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (VNCoreMLModel model, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("revision")]
		VNCoreMLRequestRevision Revision { get; set; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNCoreMLRequestRevision> (WeakSupportedRevisions)")]
		VNCoreMLRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNCoreMLRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNCoreMLRequestRevision CurrentRevision { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectBarcodesRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'GetSupportedSymbologies' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'GetSupportedSymbologies' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'GetSupportedSymbologies' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'GetSupportedSymbologies' instead.")]
		[Static]
		[Protected]
		[Export ("supportedSymbologies", ArgumentSemantic.Copy)]
		NSString [] WeakSupportedSymbologies { get; }

		[Static]
		[Wrap ("VNBarcodeSymbologyExtensions.GetValues (WeakSupportedSymbologies)")]
		VNBarcodeSymbology [] SupportedSymbologies { get; }

		[Protected]
		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("supportedSymbologiesAndReturnError:")]
		[return: NullAllowed]
		NSString [] GetWeakSupportedSymbologies ([NullAllowed] out NSError error);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Wrap ("VNBarcodeSymbologyExtensions.GetValues (GetWeakSupportedSymbologies (out error))")]
		VNBarcodeSymbology [] GetSupportedSymbologies ([NullAllowed] out NSError error);

		[Protected]
		[Export ("symbologies", ArgumentSemantic.Copy)]
		NSString [] WeakSymbologies { get; set; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNBarcodeObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("revision")]
		VNDetectBarcodesRequestRevision Revision { get; set; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNDetectBarcodesRequestRevision> (WeakSupportedRevisions)")]
		VNDetectBarcodesRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNDetectBarcodesRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNDetectBarcodesRequestRevision CurrentRevision { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("coalesceCompositeSymbologies")]
		bool CoalesceCompositeSymbologies { get; set; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectFaceLandmarksRequest : VNFaceObservationAccepting {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("revision:supportsConstellation:")]
		bool SupportsConstellation (VNDetectFaceLandmarksRequestRevision revision, VNRequestFaceLandmarksConstellation constellation);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("constellation", ArgumentSemantic.Assign)]
		VNRequestFaceLandmarksConstellation Constellation { get; set; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNFaceObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("revision")]
		VNDetectFaceLandmarksRequestRevision Revision { get; set; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNDetectFaceLandmarksRequestRevision> (WeakSupportedRevisions)")]
		VNDetectFaceLandmarksRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNDetectFaceLandmarksRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNDetectFaceLandmarksRequestRevision CurrentRevision { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectFaceRectanglesRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNFaceObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("revision")]
		VNDetectFaceRectanglesRequestRevision Revision { get; set; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNDetectFaceRectanglesRequestRevision> (WeakSupportedRevisions)")]
		VNDetectFaceRectanglesRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNDetectFaceRectanglesRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNDetectFaceRectanglesRequestRevision CurrentRevision { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectHorizonRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNHorizonObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("revision")]
		VNDetectHorizonRequestRevision Revision { get; set; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNDetectHorizonRequestRevision> (WeakSupportedRevisions)")]
		VNDetectHorizonRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNDetectHorizonRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNDetectHorizonRequestRevision CurrentRevision { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectRectanglesRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

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

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNRectangleObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("revision")]
		VNDetectRectanglesRequestRevision Revision { get; set; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNDetectRectanglesRequestRevision> (WeakSupportedRevisions)")]
		VNDetectRectanglesRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNDetectRectanglesRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNDetectRectanglesRequestRevision CurrentRevision { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectTextRectanglesRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("reportCharacterBoxes")]
		bool ReportCharacterBoxes { get; set; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNTextObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("revision")]
		VNDetectTextRectanglesRequestRevision Revision { get; set; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNDetectTextRectanglesRequestRevision> (WeakSupportedRevisions)")]
		VNDetectTextRectanglesRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNDetectTextRectanglesRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNDetectTextRectanglesRequestRevision CurrentRevision { get; }
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface VNFaceLandmarkRegion : NSCopying, NSSecureCoding, VNRequestRevisionProviding {

		[Export ("pointCount")]
		nuint PointCount { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNFaceLandmarkRegion))]
	interface VNFaceLandmarkRegion2D {

		[Internal]
		[Export ("normalizedPoints")]
		IntPtr _GetNormalizedPoints ();

		[Internal]
		[Export ("pointsInImageOfSize:")]
		IntPtr _GetPointsInImage (CGSize imageSize);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[BindAs (typeof (nfloat []))]
		[NullAllowed, Export ("precisionEstimatesPerPoint")]
		NSNumber [] PrecisionEstimatesPerPoint { get; }

		[TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("pointsClassification")]
		VNPointsClassification PointsClassification { get; }
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface VNFaceLandmarks : NSCopying, NSSecureCoding, VNRequestRevisionProviding {

		[Export ("confidence")]
		float Confidence { get; }
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[Protocol]
	interface VNFaceObservationAccepting {

		[Abstract]
		[NullAllowed, Export ("inputFaceObservations", ArgumentSemantic.Copy)]
		VNFaceObservation [] InputFaceObservations { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (VNTargetedImageRequest))]
	interface VNImageRegistrationRequest {

		// Inlined from parent class
		[Export ("initWithTargetedCVPixelBuffer:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, options.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:options:completionHandler:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:completionHandler:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:options:")]
		NativeHandle Constructor (CGImage cgImage, NSDictionary optionsDict);

		[Wrap ("this (cgImage, options.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage cgImage, VNImageOptions options);

		[Export ("initWithTargetedCGImage:options:completionHandler:")]
		NativeHandle Constructor (CGImage cgImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CGImage cgImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:orientation:options:")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (cgImage, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCGImage:orientation:options:completionHandler:")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:options:")]
		NativeHandle Constructor (CIImage ciImage, NSDictionary optionsDict);

		[Wrap ("this (ciImage, options.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage ciImage, VNImageOptions options);

		[Export ("initWithTargetedCIImage:options:completionHandler:")]
		NativeHandle Constructor (CIImage ciImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CIImage ciImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:orientation:options:")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (ciImage, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCIImage:orientation:options:completionHandler:")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:options:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions options);

		[Export ("initWithTargetedImageURL:options:completionHandler:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:orientation:options:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageURL:orientation:options:completionHandler:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:options:")]
		NativeHandle Constructor (NSData imageData, NSDictionary optionsDict);

		[Wrap ("this (imageData, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions options);

		[Export ("initWithTargetedImageData:options:completionHandler:")]
		NativeHandle Constructor (NSData imageData, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:orientation:options:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageData, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageData:orientation:options:completionHandler:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageRegistrationRequest))]
	interface VNTranslationalImageRegistrationRequest {

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNImageTranslationAlignmentObservation [] Results { get; }

		// Inlined from parent class
		[Export ("initWithTargetedCVPixelBuffer:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, options.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:options:completionHandler:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:completionHandler:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:options:")]
		NativeHandle Constructor (CGImage cgImage, NSDictionary optionsDict);

		[Wrap ("this (cgImage, options.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage cgImage, VNImageOptions options);

		[Export ("initWithTargetedCGImage:options:completionHandler:")]
		NativeHandle Constructor (CGImage cgImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CGImage cgImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:orientation:options:")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (cgImage, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCGImage:orientation:options:completionHandler:")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:options:")]
		NativeHandle Constructor (CIImage ciImage, NSDictionary optionsDict);

		[Wrap ("this (ciImage, options.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage ciImage, VNImageOptions options);

		[Export ("initWithTargetedCIImage:options:completionHandler:")]
		NativeHandle Constructor (CIImage ciImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CIImage ciImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:orientation:options:")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (ciImage, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCIImage:orientation:options:completionHandler:")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:options:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions options);

		[Export ("initWithTargetedImageURL:options:completionHandler:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:orientation:options:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageURL:orientation:options:completionHandler:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:options:")]
		NativeHandle Constructor (NSData imageData, NSDictionary optionsDict);

		[Wrap ("this (imageData, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions options);

		[Export ("initWithTargetedImageData:options:completionHandler:")]
		NativeHandle Constructor (NSData imageData, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:orientation:options:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageData, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageData:orientation:options:completionHandler:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		// into subclasses so the correct class_ptr is used.
		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNTranslationalImageRegistrationRequestRevision> (WeakSupportedRevisions)")]
		VNTranslationalImageRegistrationRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNTranslationalImageRegistrationRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNTranslationalImageRegistrationRequestRevision CurrentRevision { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageRegistrationRequest))]
	interface VNHomographicImageRegistrationRequest {

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNImageHomographicAlignmentObservation [] Results { get; }

		// Inlined from parent class
		[Export ("initWithTargetedCVPixelBuffer:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, options.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:options:completionHandler:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:completionHandler:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:options:")]
		NativeHandle Constructor (CGImage cgImage, NSDictionary optionsDict);

		[Wrap ("this (cgImage, options.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage cgImage, VNImageOptions options);

		[Export ("initWithTargetedCGImage:options:completionHandler:")]
		NativeHandle Constructor (CGImage cgImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CGImage cgImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:orientation:options:")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (cgImage, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCGImage:orientation:options:completionHandler:")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:options:")]
		NativeHandle Constructor (CIImage ciImage, NSDictionary optionsDict);

		[Wrap ("this (ciImage, options.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage ciImage, VNImageOptions options);

		[Export ("initWithTargetedCIImage:options:completionHandler:")]
		NativeHandle Constructor (CIImage ciImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CIImage ciImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:orientation:options:")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (ciImage, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCIImage:orientation:options:completionHandler:")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:options:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions options);

		[Export ("initWithTargetedImageURL:options:completionHandler:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:orientation:options:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageURL:orientation:options:completionHandler:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:options:")]
		NativeHandle Constructor (NSData imageData, NSDictionary optionsDict);

		[Wrap ("this (imageData, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions options);

		[Export ("initWithTargetedImageData:options:completionHandler:")]
		NativeHandle Constructor (NSData imageData, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:orientation:options:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageData, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageData:orientation:options:completionHandler:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("revision")]
		VNHomographicImageRegistrationRequestRevision Revision { get; set; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNHomographicImageRegistrationRequestRevision> (WeakSupportedRevisions)")]
		VNHomographicImageRegistrationRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNHomographicImageRegistrationRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNHomographicImageRegistrationRequestRevision CurrentRevision { get; }
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface VNObservation : NSCopying, NSSecureCoding, VNRequestRevisionProviding {

		[Export ("uuid", ArgumentSemantic.Strong)]
		NSUuid Uuid { get; }

		[Export ("confidence")]
		float Confidence { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("timeRange", ArgumentSemantic.Assign)]
		CMTimeRange TimeRange { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNDetectedObjectObservation {

		[Static]
		[Export ("observationWithBoundingBox:")]
		VNDetectedObjectObservation FromBoundingBox (CGRect boundingBox);

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNDetectedObjectObservation FromBoundingBox (VNDetectedObjectObservationRequestRevision requestRevision, CGRect boundingBox);

		[Export ("boundingBox", ArgumentSemantic.Assign)]
		CGRect BoundingBox { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("globalSegmentationMask")]
		VNPixelBufferObservation GlobalSegmentationMask { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNDetectedObjectObservation))]
	interface VNFaceObservation {

		[NullAllowed, Export ("landmarks", ArgumentSemantic.Strong)]
		VNFaceLandmarks2D Landmarks { get; }

		[New]
		[Static]
		[Export ("observationWithBoundingBox:")]
		VNFaceObservation FromBoundingBox (CGRect boundingBox);

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNFaceObservation FromBoundingBox (VNFaceObservationRequestRevision requestRevision, CGRect boundingBox);

		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.TvOS, 15, 0)]
		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[Static]
		[Export ("faceObservationWithRequestRevision:boundingBox:roll:yaw:")]
		VNFaceObservation FromBoundingBox (VNFaceObservationRequestRevision requestRevision, CGRect boundingBox, [NullAllowed][BindAs (typeof (nfloat?))] NSNumber roll, [NullAllowed][BindAs (typeof (nfloat?))] NSNumber yaw);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("faceObservationWithRequestRevision:boundingBox:roll:yaw:pitch:")]
		VNFaceObservation FromBoundingBox (VNFaceObservationRequestRevision requestRevision, CGRect boundingBox, [NullAllowed][BindAs (typeof (nfloat?))] NSNumber roll, [NullAllowed][BindAs (typeof (nfloat?))] NSNumber yaw, [NullAllowed][BindAs (typeof (nfloat?))] NSNumber pitch);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[BindAs (typeof (float?))]
		[NullAllowed, Export ("faceCaptureQuality", ArgumentSemantic.Strong)]
		NSNumber FaceCaptureQuality { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[BindAs (typeof (nfloat?))]
		[NullAllowed, Export ("roll", ArgumentSemantic.Strong)]
		NSNumber Roll { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[BindAs (typeof (nfloat?))]
		[NullAllowed, Export ("yaw", ArgumentSemantic.Strong)]
		NSNumber Yaw { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[BindAs (typeof (nfloat?))]
		[NullAllowed, Export ("pitch", ArgumentSemantic.Strong)]
		NSNumber Pitch { get; }
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNDetectedObjectObservation))]
	[DisableDefaultCtor]
	interface VNRecognizedObjectObservation {

		[New]
		[Static]
		[Export ("observationWithBoundingBox:")]
		VNRecognizedObjectObservation FromBoundingBox (CGRect boundingBox);

		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNRecognizedObjectObservation FromBoundingBox (VNRecognizedObjectObservationRequestRevision requestRevision, CGRect boundingBox);

		[Export ("labels", ArgumentSemantic.Copy)]
		VNClassificationObservation [] Labels { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNClassificationObservation {

		[Export ("identifier")]
		string Identifier { get; }

		// From interface VNClassificationObservation

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hasPrecisionRecallCurve")]
		bool HasPrecisionRecallCurve { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hasMinimumRecall:forPrecision:")]
		bool HasMinimumRecall (float minimumRecall, float precision);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hasMinimumPrecision:forRecall:")]
		bool HasMinimumPrecision (float minimumPrecision, float recall);
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNCoreMLFeatureValueObservation {

		[Export ("featureValue", ArgumentSemantic.Copy)]
		MLFeatureValue FeatureValue { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("featureName")]
		string FeatureName { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNPixelBufferObservation {

		[Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("featureName")]
		string FeatureName { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNDetectedObjectObservation))]
	interface VNRectangleObservation {

		[Deprecated (PlatformName.MacOSX, 14, 0)]
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("rectangleObservationWithRequestRevision:topLeft:bottomLeft:bottomRight:topRight:")]
		VNRectangleObservation GetRectangleObservation (VNRectangleObservationRequestRevision requestRevision, CGPoint topLeft, CGPoint bottomLeft, CGPoint bottomRight, CGPoint topRight);

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

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNRectangleObservation FromBoundingBox (VNRectangleObservationRequestRevision requestRevision, CGRect boundingBox);

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("rectangleObservationWithRequestRevision:topLeft:topRight:bottomRight:bottomLeft:")]
		VNRectangleObservation FromRectangleObservation (VNRectangleObservationRequestRevision requestRevision, CGPoint topLeft, CGPoint topRight, CGPoint bottomRight, CGPoint bottomLeft);
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNRectangleObservation))]
	interface VNTextObservation {

		[NullAllowed, Export ("characterBoxes", ArgumentSemantic.Copy)]
		VNRectangleObservation [] CharacterBoxes { get; }

		[New]
		[Static]
		[Export ("observationWithBoundingBox:")]
		VNTextObservation FromBoundingBox (CGRect boundingBox);

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNTextObservation FromBoundingBox (VNTextObservationRequestRevision requestRevision, CGRect boundingBox);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNRectangleObservation))]
	[DisableDefaultCtor]
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

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("observationWithRequestRevision:boundingBox:")]
		VNBarcodeObservation FromBoundingBox (VNBarcodeObservationRequestRevision requestRevision, CGRect boundingBox);

		[NullAllowed, Export ("payloadStringValue")]
		string PayloadStringValue { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("payloadData", ArgumentSemantic.Copy)]
		NSData PayloadData { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("isGS1DataCarrier")]
		bool IsGS1DataCarrier { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("isColorInverted")]
		bool IsColorInverted { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("supplementalCompositeType")]
		VNBarcodeCompositeType SupplementalCompositeType { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("supplementalPayloadString")]
		string SupplementalPayloadString { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("supplementalPayloadData", ArgumentSemantic.Copy)]
		NSData SupplementalPayloadData { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNHorizonObservation {

		[Export ("transform", ArgumentSemantic.Assign)]
		CGAffineTransform Transform { get; }

		[Export ("angle")]
		nfloat Angle { get; }

		[TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("transformForImageWidth:height:")]
		CGAffineTransform CreateTransform (nuint width, nuint height);
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (VNObservation))]
	interface VNImageAlignmentObservation {
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageAlignmentObservation))]
	interface VNImageTranslationAlignmentObservation {

		[Export ("alignmentTransform", ArgumentSemantic.Assign)]
		CGAffineTransform AlignmentTransform {
			get;
#if !NET
			[NotImplemented]
			set;
#endif
		}
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNImageAlignmentObservation))]
	interface VNImageHomographicAlignmentObservation {

		[Export ("warpTransform", ArgumentSemantic.Assign)]
		Matrix3 WarpTransform {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
#if !NET
			[NotImplemented]
			set;
#endif
		}
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface VNRequest : NSCopying {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("preferBackgroundProcessing")]
		bool PreferBackgroundProcessing { get; set; }

		// From docs: VNObservation subclasses specific to the VNRequest subclass
		// Since downcasting is not easy we are exposing
		// this property as a generic method 'GetResults'.
		[Internal]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		IntPtr _Results { get; }

		[NullAllowed, Export ("completionHandler", ArgumentSemantic.Copy)]
		VNRequestCompletionHandler CompletionHandler { get; }

		[Deprecated (PlatformName.MacOSX, 14, 0)]
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
		[Export ("usesCPUOnly")]
		bool UsesCpuOnly { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("cancel")]
		void Cancel ();

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		//[TV (12,0), iOS (12,0)]
		//[Export ("revision")]
		//VNRequestRevision Revision { get; set; }

		//[TV (12,0), iOS (12,0)]
		//[Static]
		//[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		//NSIndexSet WeakSupportedRevisions { get; }

		//[TV (12,0), iOS (12,0)]
		//[Static]
		//[Wrap ("GetSupportedVersions (WeakSupportedRevisions)")]
		//VNRequestRevision [] SupportedRevisions { get; }

		//[TV (12,0), iOS (12,0)]
		//[Static]
		//[Export ("defaultRevision")]
		//VNRequestRevision DefaultRevision { get; }

		//[TV (12,0), iOS (12,0)]
		//[Static]
		//[Export ("currentRevision")]
		//VNRequestRevision CurrentRevision { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("supportedComputeStageDevicesAndReturnError:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSArray<IMLComputeDeviceProtocol>> GetSupportedComputeDevices ([NullAllowed] out NSError error);

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("computeDeviceForComputeStage:")]
		[return: NullAllowed]
		IMLComputeDeviceProtocol GetComputeDevice (string computeStage);

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("setComputeDevice:forComputeStage:")]
		void SetComputeDevice ([NullAllowed] IMLComputeDeviceProtocol computeDevice, string computeStage);
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[DisableDefaultCtor]
	[BaseType (typeof (VNRequest))]
	interface VNImageBasedRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("regionOfInterest", ArgumentSemantic.Assign)]
		CGRect RegionOfInterest { get; set; }
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[StrongDictionary ("VNImageOptionKeys")]
	interface VNImageOptions {
		[Export ("PropertiesKey")] // Have the option to set your own dict
		NSDictionary WeakProperties { get; set; }

		[StrongDictionary] // Yep we need CoreGraphics to disambiguate
		CoreGraphics.CGImageProperties Properties { get; set; }

		NSData CameraIntrinsics { get; set; }
		CIContext CIContext { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNImageRequestHandler {

		[Export ("initWithCVPixelBuffer:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary options);

		[Wrap ("this (pixelBuffer, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions imageOptions);

		[Export ("initWithCVPixelBuffer:orientation:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary options);

		[Wrap ("this (pixelBuffer, orientation, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[Export ("initWithCGImage:options:")]
		NativeHandle Constructor (CGImage image, NSDictionary options);

		[Wrap ("this (image, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage image, VNImageOptions imageOptions);

		[Export ("initWithCGImage:orientation:options:")]
		NativeHandle Constructor (CGImage image, CGImagePropertyOrientation orientation, NSDictionary options);

		[Wrap ("this (image, orientation, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage image, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[Export ("initWithCIImage:options:")]
		NativeHandle Constructor (CIImage image, NSDictionary options);

		[Wrap ("this (image, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage image, VNImageOptions imageOptions);

		[Export ("initWithCIImage:orientation:options:")]
		NativeHandle Constructor (CIImage image, CGImagePropertyOrientation orientation, NSDictionary options);

		[Wrap ("this (image, orientation, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage image, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[Export ("initWithURL:options:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary options);

		[Wrap ("this (imageUrl, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions imageOptions);

		[Export ("initWithURL:orientation:options:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary options);

		[Wrap ("this (imageUrl, orientation, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[Export ("initWithData:options:")]
		NativeHandle Constructor (NSData imageData, NSDictionary options);

		[Wrap ("this (imageData, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions imageOptions);

		[Export ("initWithData:orientation:options:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary options);

		[Wrap ("this (imageData, orientation, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithCMSampleBuffer:options:")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, NSDictionary options);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Wrap ("this (sampleBuffer, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, VNImageOptions imageOptions);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithCMSampleBuffer:orientation:options:")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, CGImagePropertyOrientation orientation, NSDictionary options);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Wrap ("this (sampleBuffer, orientation, imageOptions.GetDictionary ()!)")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, CGImagePropertyOrientation orientation, VNImageOptions imageOptions);

		[Export ("performRequests:error:")]
		bool Perform (VNRequest [] requests, out NSError error);

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithCVPixelBuffer:depthData:orientation:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, AVDepthData depthData, CGImagePropertyOrientation orientation, NSDictionary<NSString, NSObject> options);

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithCMSampleBuffer:depthData:orientation:options:")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, AVDepthData depthData, CGImagePropertyOrientation orientation, NSDictionary<NSString, NSObject> options);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // it's a designated initializer
	interface VNSequenceRequestHandler {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

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

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("performRequests:onCMSampleBuffer:error:")]
		bool Perform (VNRequest [] requests, CMSampleBuffer sampleBuffer, [NullAllowed] out NSError error);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("performRequests:onCMSampleBuffer:orientation:error:")]
		bool Perform (VNRequest [] requests, CMSampleBuffer sampleBuffer, CGImagePropertyOrientation orientation, [NullAllowed] out NSError error);
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNTargetedImageRequest {

		[Export ("initWithTargetedCVPixelBuffer:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, options.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:options:completionHandler:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:completionHandler:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:options:")]
		NativeHandle Constructor (CGImage cgImage, NSDictionary optionsDict);

		[Wrap ("this (cgImage, options.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage cgImage, VNImageOptions options);

		[Export ("initWithTargetedCGImage:options:completionHandler:")]
		NativeHandle Constructor (CGImage cgImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CGImage cgImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:orientation:options:")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (cgImage, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCGImage:orientation:options:completionHandler:")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:options:")]
		NativeHandle Constructor (CIImage ciImage, NSDictionary optionsDict);

		[Wrap ("this (ciImage, options.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage ciImage, VNImageOptions options);

		[Export ("initWithTargetedCIImage:options:completionHandler:")]
		NativeHandle Constructor (CIImage ciImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CIImage ciImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:orientation:options:")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (ciImage, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCIImage:orientation:options:completionHandler:")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:options:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions options);

		[Export ("initWithTargetedImageURL:options:completionHandler:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:orientation:options:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageURL:orientation:options:completionHandler:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:options:")]
		NativeHandle Constructor (NSData imageData, NSDictionary optionsDict);

		[Wrap ("this (imageData, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions options);

		[Export ("initWithTargetedImageData:options:completionHandler:")]
		NativeHandle Constructor (NSData imageData, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:orientation:options:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageData, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageData:orientation:options:completionHandler:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithTargetedCMSampleBuffer:options:")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, NSDictionary optionsDict);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Wrap ("this (sampleBuffer, options.GetDictionary ()!)")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, VNImageOptions options);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithTargetedCMSampleBuffer:options:completionHandler:")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Wrap ("this (sampleBuffer, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithTargetedCMSampleBuffer:orientation:options:")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Wrap ("this (sampleBuffer, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, CGImagePropertyOrientation orientation, VNImageOptions options);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithTargetedCMSampleBuffer:orientation:options:completionHandler:")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Wrap ("this (sampleBuffer, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNTrackingRequest))]
	[DisableDefaultCtor]
	interface VNTrackObjectRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithDetectedObjectObservation:")]
		NativeHandle Constructor (VNDetectedObjectObservation observation);

		[Export ("initWithDetectedObjectObservation:completionHandler:")]
		NativeHandle Constructor (VNDetectedObjectObservation observation, [NullAllowed] VNRequestCompletionHandler completionHandler);

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("revision")]
		VNTrackObjectRequestRevision Revision { get; set; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNTrackObjectRequestRevision> (WeakSupportedRevisions)")]
		VNTrackObjectRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNTrackObjectRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNTrackObjectRequestRevision CurrentRevision { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNTrackingRequest))]
	[DisableDefaultCtor]
	interface VNTrackRectangleRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithRectangleObservation:")]
		NativeHandle Constructor (VNRectangleObservation observation);

		[Export ("initWithRectangleObservation:completionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (VNRectangleObservation observation, [NullAllowed] VNRequestCompletionHandler completionHandler);

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("revision")]
		VNTrackRectangleRequestRevision Revision { get; set; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetSupportedVersions<VNTrackRectangleRequestRevision> (WeakSupportedRevisions)")]
		VNTrackRectangleRequestRevision [] SupportedRevisions { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultRevision")]
		VNTrackRectangleRequestRevision DefaultRevision { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentRevision")]
		VNTrackRectangleRequestRevision CurrentRevision { get; }
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNTrackingRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("inputObservation", ArgumentSemantic.Strong)]
		VNDetectedObjectObservation InputObservation { get; set; }

		[Export ("trackingLevel", ArgumentSemantic.Assign)]
		VNRequestTrackingLevel TrackingLevel { get; set; }

		[Export ("lastFrame")]
		bool LastFrame { [Bind ("isLastFrame")] get; set; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("supportedNumberOfTrackersAndReturnError:")]
		nuint GetSupportedNumberOfTrackers ([NullAllowed] out NSError error);
	}

	[TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface VNRequestRevisionProviding {

		[Abstract]
		[Export ("requestRevision")]
		VNRequestRevision RequestRevision { get; }
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNClassifyImageRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'GetSupportedIdentifiers' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'GetSupportedIdentifiers' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'GetSupportedIdentifiers' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'GetSupportedIdentifiers' instead.")]
		[Static]
		[Export ("knownClassificationsForRevision:error:")]
		[return: NullAllowed]
		VNClassificationObservation [] GetKnownClassifications (VNClassifyImageRequestRevision revision, [NullAllowed] out NSError error);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("supportedIdentifiersAndReturnError:")]
		[return: NullAllowed]
		NSString [] GetSupportedIdentifiers ([NullAllowed] out NSError error);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNClassificationObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNClassifyImageRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNClassifyImageRequestRevision> (WeakSupportedRevisions)")]
		VNClassifyImageRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNClassifyImageRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNClassifyImageRequestRevision CurrentRevision { get; }
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNDetectFaceCaptureQualityRequest : VNFaceObservationAccepting {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNFaceObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNDetectFaceCaptureQualityRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNDetectFaceCaptureQualityRequestRevision> (WeakSupportedRevisions)")]
		VNDetectFaceCaptureQualityRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNDetectFaceCaptureQualityRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNDetectFaceCaptureQualityRequestRevision CurrentRevision { get; }
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNDetectHumanRectanglesRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("upperBodyOnly")]
		bool UpperBodyOnly { get; set; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNHumanObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNDetectHumanRectanglesRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNDetectHumanRectanglesRequestRevision> (WeakSupportedRevisions)")]
		VNDetectHumanRectanglesRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNDetectHumanRectanglesRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNDetectHumanRectanglesRequestRevision CurrentRevision { get; }
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNGenerateAttentionBasedSaliencyImageRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNRecognizedTextObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNGenerateAttentionBasedSaliencyImageRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNGenerateAttentionBasedSaliencyImageRequestRevision> (WeakSupportedRevisions)")]
		VNGenerateAttentionBasedSaliencyImageRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNGenerateAttentionBasedSaliencyImageRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNGenerateAttentionBasedSaliencyImageRequestRevision CurrentRevision { get; }
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNGenerateImageFeaturePrintRequest {

		[Export ("imageCropAndScaleOption", ArgumentSemantic.Assign)]
		VNImageCropAndScaleOption ImageCropAndScaleOption { get; set; }

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNFeaturePrintObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNGenerateImageFeaturePrintRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNGenerateImageFeaturePrintRequestRevision> (WeakSupportedRevisions)")]
		VNGenerateImageFeaturePrintRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNGenerateImageFeaturePrintRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNGenerateImageFeaturePrintRequestRevision CurrentRevision { get; }
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNGenerateObjectnessBasedSaliencyImageRequest {

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNSaliencyImageObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNGenerateObjectnessBasedSaliencyImageRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNGenerateObjectnessBasedSaliencyImageRequestRevision> (WeakSupportedRevisions)")]
		VNGenerateObjectnessBasedSaliencyImageRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNGenerateObjectnessBasedSaliencyImageRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNGenerateObjectnessBasedSaliencyImageRequestRevision CurrentRevision { get; }
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNRecognizedText : NSCopying, NSSecureCoding, VNRequestRevisionProviding {

		[Export ("string")]
		string String { get; }

		[Export ("confidence")]
		float Confidence { get; }

		[Export ("boundingBoxForRange:error:")]
		[return: NullAllowed]
		VNRectangleObservation GetBoundingBox (NSRange range, [NullAllowed] out NSError error);
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNRectangleObservation))]
	[DisableDefaultCtor]
	interface VNRecognizedTextObservation {

		[Export ("topCandidates:")]
		VNRecognizedText [] TopCandidates (nuint maxCandidateCount);

		[Static]
		[Export ("observationWithBoundingBox:")]
		VNRecognizedTextObservation Create (CGRect boundingBox);
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (VNPixelBufferObservation))]
	interface VNSaliencyImageObservation {

		[NullAllowed, Export ("salientObjects")]
		VNRectangleObservation [] SalientObjects { get; }
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNObservation))]
	[DisableDefaultCtor]
	interface VNFeaturePrintObservation {

		[Export ("elementType", ArgumentSemantic.Assign)]
		VNElementType ElementType { get; }

		[Export ("elementCount")]
		nuint ElementCount { get; }

		[Export ("data", ArgumentSemantic.Strong)]
		NSData Data { get; }

		[Internal]
		[Export ("computeDistance:toFeaturePrintObservation:error:")]
		bool _ComputeDistance (IntPtr outDistance, VNFeaturePrintObservation featurePrint, [NullAllowed] out NSError error);
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	enum VNAnimalIdentifier {

		[DefaultEnumValue]
		[Field (null)]
		Unknown = -1,

		[Field ("VNAnimalIdentifierDog")]
		Dog,

		[Field ("VNAnimalIdentifierCat")]
		Cat,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNRecognizeAnimalsRequest {

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'GetSupportedIdentifiers' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'GetSupportedIdentifiers' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'GetSupportedIdentifiers' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'GetSupportedIdentifiers' instead.")]
		[Static]
		[Export ("knownAnimalIdentifiersForRevision:error:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNAnimalIdentifier []))]
		NSString [] GetKnownAnimalIdentifiers (VNRecognizeAnimalsRequestRevision revision, [NullAllowed] out NSError error);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("supportedIdentifiersAndReturnError:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNAnimalIdentifier []))]
		NSString [] GetSupportedIdentifiers ([NullAllowed] out NSError error);

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNRecognizedObjectObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNRecognizeAnimalsRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNRecognizeAnimalsRequestRevision> (WeakSupportedRevisions)")]
		VNRecognizeAnimalsRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNRecognizeAnimalsRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNRecognizeAnimalsRequestRevision CurrentRevision { get; }
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNRecognizeTextRequest : VNRequestProgressProviding {

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'GetSupportedRecognitionLanguages' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'GetSupportedRecognitionLanguages' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'GetSupportedRecognitionLanguages' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'GetSupportedRecognitionLanguages' instead.")]
		[Static]
		[Export ("supportedRecognitionLanguagesForTextRecognitionLevel:revision:error:")]
		[return: NullAllowed]
		string [] GetSupportedRecognitionLanguages (VNRequestTextRecognitionLevel textRecognitionLevel, VNRecognizeTextRequestRevision revision, [NullAllowed] out NSError error);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("supportedRecognitionLanguagesAndReturnError:")]
		[return: NullAllowed]
		NSString [] GetSupportedRecognitionLanguages ([NullAllowed] out NSError error);

		[Export ("recognitionLanguages", ArgumentSemantic.Copy)]
		string [] RecognitionLanguages { get; set; }

		[Export ("customWords", ArgumentSemantic.Copy)]
		string [] CustomWords { get; set; }

		[Export ("recognitionLevel", ArgumentSemantic.Assign)]
		VNRequestTextRecognitionLevel RecognitionLevel { get; set; }

		[Export ("usesLanguageCorrection")]
		bool UsesLanguageCorrection { get; set; }

		[Export ("minimumTextHeight")]
		float MinimumTextHeight { get; set; }

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNRecognizedTextObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNRecognizeTextRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNRecognizeTextRequestRevision> (WeakSupportedRevisions)")]
		VNRecognizeTextRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNRecognizeTextRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNRecognizeTextRequestRevision CurrentRevision { get; }

		[TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("automaticallyDetectsLanguage")]
		bool AutomaticallyDetectsLanguage { get; set; }
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	delegate void VNRequestProgressHandler (VNRequest request, double fractionCompleted, [NullAllowed] NSError error);
	interface IVNRequestProgressProviding { }

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface VNRequestProgressProviding {

		[Abstract]
		[Export ("progressHandler", ArgumentSemantic.Copy)]
		VNRequestProgressHandler ProgressHandler { get; set; }

		[Abstract]
		[Export ("indeterminate")]
		bool Indeterminate { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNDetectContoursRequest {

		[Export ("contrastAdjustment")]
		float ContrastAdjustment { get; set; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("contrastPivot", ArgumentSemantic.Assign)]
		NSNumber ContrastPivot { get; set; }

		[Export ("detectsDarkOnLight")]
		bool DetectsDarkOnLight { get; set; }

		[Export ("maximumImageDimension")]
		nuint MaximumImageDimension { get; set; }

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNContoursObservation [] Results { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNDetectContourRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNDetectContourRequestRevision> (WeakSupportedRevisions)")]
		VNDetectContourRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNDetectContourRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNDetectContourRequestRevision CurrentRevision { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNRecognizedPointsObservation))]
	[DisableDefaultCtor]
	interface VNHumanBodyPoseObservation {

		[BindAs (typeof (VNHumanBodyPoseObservationJointName []))]
		[Export ("availableJointNames", ArgumentSemantic.Copy)]
		NSString [] AvailableJointNames { get; }

		[BindAs (typeof (VNHumanBodyPoseObservationJointsGroupName []))]
		[Export ("availableJointsGroupNames", ArgumentSemantic.Copy)]
		NSString [] AvailableJointsGroupNames { get; }

		[Export ("recognizedPointForJointName:error:")]
		[return: NullAllowed]
		VNRecognizedPoint GetRecognizedPoint ([BindAs (typeof (VNHumanBodyPoseObservationJointName))] NSString jointName, [NullAllowed] out NSError error);

		[Export ("recognizedPointsForJointsGroupName:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, VNRecognizedPoint> GetRecognizedPoints ([BindAs (typeof (VNHumanBodyPoseObservationJointsGroupName))] NSString jointsGroupName, [NullAllowed] out NSError error);
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNDetectHumanBodyPoseRequest {

		[Deprecated (PlatformName.MacOSX, 14, 0)]
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
		[Static]
		[Export ("supportedJointNamesForRevision:error:")]
		[return: BindAs (typeof (VNHumanBodyPoseObservationJointName []))]
		[return: NullAllowed]
		NSString [] GetSupportedJointNames (VNDetectHumanBodyPoseRequestRevision revision, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 14, 0)]
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
		[Static]
		[Export ("supportedJointsGroupNamesForRevision:error:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNHumanBodyPoseObservationJointsGroupName []))]
		NSString [] GetSupportedJointsGroupNames (VNDetectHumanBodyPoseRequestRevision revision, [NullAllowed] out NSError error);

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNHumanBodyPoseObservation [] Results { get; }

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNDetectHumanBodyPoseRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNDetectHumanBodyPoseRequestRevision> (WeakSupportedRevisions)")]
		VNDetectHumanBodyPoseRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNDetectHumanBodyPoseRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNDetectHumanBodyPoseRequestRevision CurrentRevision { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("supportedJointsGroupNamesAndReturnError:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNHumanBodyPoseObservationJointsGroupName []))]
		NSString [] GetSupportedJointsGroupNames ([NullAllowed] out NSError error);

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("supportedJointNamesAndReturnError:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNHumanBodyPoseObservationJointName []))]
		NSString [] GetSupportedJointNames ([NullAllowed] out NSError error);
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNRecognizedPointsObservation))]
	[DisableDefaultCtor]
	interface VNHumanHandPoseObservation {

		[BindAs (typeof (VNHumanHandPoseObservationJointName []))]
		[Export ("availableJointNames", ArgumentSemantic.Copy)]
		NSString [] AvailableJointNames { get; }

		[BindAs (typeof (VNHumanHandPoseObservationJointsGroupName []))]
		[Export ("availableJointsGroupNames", ArgumentSemantic.Copy)]
		NSString [] AvailableJointsGroupNames { get; }

		[Export ("recognizedPointForJointName:error:")]
		[return: NullAllowed]
		VNRecognizedPoint GetRecognizedPoint ([BindAs (typeof (VNHumanHandPoseObservationJointName))] NSString jointName, [NullAllowed] out NSError error);

		[Export ("recognizedPointsForJointsGroupName:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, VNRecognizedPoint> GetRecognizedPoints ([BindAs (typeof (VNHumanHandPoseObservationJointsGroupName))] NSString jointsGroupName, [NullAllowed] out NSError error);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("chirality")]
		VNChirality Chirality { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNDetectHumanHandPoseRequest {

		[Deprecated (PlatformName.MacOSX, 14, 0)]
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
		[Static]
		[Export ("supportedJointNamesForRevision:error:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNHumanHandPoseObservationJointName []))]
		NSString [] GetSupportedJointNames (VNDetectHumanHandPoseRequestRevision revision, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 14, 0)]
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
		[Static]
		[Export ("supportedJointsGroupNamesForRevision:error:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNHumanHandPoseObservationJointsGroupName []))]
		NSString [] GetSupportedJointsGroupNames (VNDetectHumanHandPoseRequestRevision revision, [NullAllowed] out NSError error);

		[Export ("maximumHandCount")]
		nuint MaximumHandCount { get; set; }

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNHumanHandPoseObservation [] Results { get; }

		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNDetectHumanHandPoseRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNDetectHumanHandPoseRequestRevision> (WeakSupportedRevisions)")]
		VNDetectHumanHandPoseRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNDetectHumanHandPoseRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNDetectHumanHandPoseRequestRevision CurrentRevision { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("supportedJointNamesAndReturnError:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNHumanHandPoseObservationJointName []))]
		NSString [] GetSupportedJointNames ([NullAllowed] out NSError error);

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("supportedJointsGroupNamesAndReturnError:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNHumanHandPoseObservationJointsGroupName []))]
		NSString [] GetSupportedJointsGroupNames ([NullAllowed] out NSError error);
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNStatefulRequest))]
	[DisableDefaultCtor]
	interface VNDetectTrajectoriesRequest {

		[Export ("initWithFrameAnalysisSpacing:trajectoryLength:completionHandler:")]
		NativeHandle Constructor (CMTime frameAnalysisSpacing, nint trajectoryLength, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("trajectoryLength")]
		nint TrajectoryLength { get; }

		[Export ("objectMinimumNormalizedRadius")]
		float ObjectMinimumNormalizedRadius { get; set; }

		[Export ("objectMaximumNormalizedRadius")]
		float ObjectMaximumNormalizedRadius { get; set; }

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNTrajectoryObservation [] Results { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("targetFrameTime", ArgumentSemantic.Assign)]
		CMTime TargetFrameTime { get; set; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNDetectTrajectoriesRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNDetectTrajectoriesRequestRevision> (WeakSupportedRevisions)")]
		VNDetectTrajectoriesRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNDetectTrajectoriesRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNDetectTrajectoriesRequestRevision CurrentRevision { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNPoint : NSCopying, NSSecureCoding {

		[Static]
		[Export ("zeroPoint", ArgumentSemantic.Strong)]
		VNPoint Zero { get; }

		[Static]
		[Export ("pointByApplyingVector:toPoint:")]
		VNPoint Create (VNVector vector, VNPoint point);

		[Export ("distanceToPoint:")]
		double GetDistanceToPoint (VNPoint point);

		[Export ("initWithX:y:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double x, double y);

		[Export ("initWithLocation:")]
		NativeHandle Constructor (CGPoint location);

		[Export ("location")]
		CGPoint Location { get; }

		[Export ("x")]
		double X { get; }

		[Export ("y")]
		double Y { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNVector : NSCopying, NSSecureCoding {

		[Static]
		[Export ("zeroVector", ArgumentSemantic.Strong)]
		VNVector Zero { get; }

		[Static]
		[Export ("unitVectorForVector:")]
		VNVector CreateUnitVector (VNVector vector);

		[Static]
		[Export ("vectorByMultiplyingVector:byScalar:")]
		VNVector CreateByMultiplyingVector (VNVector vector, double scalar);

		[Static]
		[Export ("vectorByAddingVector:toVector:")]
		VNVector CreateByAddingVector (VNVector v1, VNVector v2);

		[Static]
		[Export ("vectorBySubtractingVector:fromVector:")]
		VNVector CreateBySubtractingVector (VNVector v1, VNVector v2);

		[Static]
		[Export ("dotProductOfVector:vector:")]
		double GetDotProduct (VNVector v1, VNVector v2);

		[Export ("initWithXComponent:yComponent:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double x, double y);

		[Internal]
		[Export ("initWithR:theta:")]
		IntPtr InitWithRTheta (double r, double theta);

		[Export ("initWithVectorHead:tail:")]
		NativeHandle Constructor (VNPoint head, VNPoint tail);

		[Export ("x")]
		double X { get; }

		[Export ("y")]
		double Y { get; }

		[Export ("r")]
		double R { get; }

		[Export ("theta")]
		double Theta { get; }

		[Export ("length")]
		double Length { get; }

		[Export ("squaredLength")]
		double SquaredLength { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNCircle : NSCopying, NSSecureCoding {

		[Static]
		[Export ("zeroCircle", ArgumentSemantic.Strong)]
		VNCircle Zero { get; }

		[Internal]
		[Export ("initWithCenter:radius:")]
		IntPtr InitWithCenterRadius (VNPoint center, double radius);

		[Internal]
		[Export ("initWithCenter:diameter:")]
		IntPtr InitWithCenterDiameter (VNPoint center, double diameter);

		[Export ("containsPoint:")]
		bool Contains (VNPoint point);

		[Export ("containsPoint:inCircumferentialRingOfWidth:")]
		bool Contains (VNPoint point, double ringWidth);

		[Export ("center", ArgumentSemantic.Strong)]
		VNPoint Center { get; }

		[Export ("radius")]
		double Radius { get; }

		[Export ("diameter")]
		double Diameter { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Not meant to be created but obtained via VNContoursObservation
	interface VNContour : NSCopying, VNRequestRevisionProviding {

		[Export ("indexPath")]
		NSIndexPath IndexPath { get; }

		[Export ("childContourCount")]
		nint ChildContourCount { get; }

		[Export ("childContours")]
		VNContour [] ChildContours { get; }

		[Export ("childContourAtIndex:error:")]
		[return: NullAllowed]
		VNContour GetChildContour (nuint childContourIndex, [NullAllowed] out NSError error);

		[Export ("pointCount")]
		nint PointCount { get; }

		[Export ("normalizedPoints")]
		Vector2 NormalizedPoints {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("normalizedPath")]
		CGPath NormalizedPath { get; }

		[Export ("aspectRatio")]
		float AspectRatio { get; }

		[Export ("polygonApproximationWithEpsilon:error:")]
		[return: NullAllowed]
		VNContour GetPolygonApproximation (float epsilon, [NullAllowed] out NSError error);
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNPoint))]
	[DisableDefaultCtor]
	interface VNDetectedPoint {

		[Export ("confidence")]
		float Confidence { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNDetectedPoint))]
	[DisableDefaultCtor]
	interface VNRecognizedPoint {

		// This can be one of the several smart enums so can't really BindAs
		[Export ("identifier")]
		NSString Identifier { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNGeometryUtils {

		[Static]
		[Export ("boundingCircleForContour:error:")]
		[return: NullAllowed]
		VNCircle CreateBoundingCircle (VNContour contour, [NullAllowed] out NSError error);

		[Static]
		[Export ("boundingCircleForPoints:error:")]
		[return: NullAllowed]
		VNCircle CreateBoundingCircle (VNPoint [] points, [NullAllowed] out NSError error);

		[Internal]
		[Static]
		[Export ("boundingCircleForSIMDPoints:pointCount:error:")]
		[return: NullAllowed]
		VNCircle CreateBoundingCircle (IntPtr pointsPtr, nint pointCount, out NSError error);

		[Static]
		[Export ("calculateArea:forContour:orientedArea:error:")]
		bool CalculateArea (out double area, VNContour contour, bool orientedArea, [NullAllowed] out NSError error);

		[Static]
		[Export ("calculatePerimeter:forContour:error:")]
		bool CalculatePerimeter (out double perimeter, VNContour contour, [NullAllowed] out NSError error);
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNTargetedImageRequest))]
	interface VNGenerateOpticalFlowRequest {

		[Export ("computationAccuracy", ArgumentSemantic.Assign)]
		VNGenerateOpticalFlowRequestComputationAccuracy ComputationAccuracy { get; set; }

		[Advice ("The only allowed values here are 'TwoComponent16Half' or 'TwoComponent32Float' (Default).")]
		[Export ("outputPixelFormat")]
		CVPixelFormatType OutputPixelFormat { get; set; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNPixelBufferObservation [] Results { get; }

		// Inlined from parent class
		[Export ("initWithTargetedCVPixelBuffer:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, options.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:options:completionHandler:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (pixelBuffer, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCVPixelBuffer:orientation:options:completionHandler:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (pixelBuffer, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:options:")]
		NativeHandle Constructor (CGImage cgImage, NSDictionary optionsDict);

		[Wrap ("this (cgImage, options.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage cgImage, VNImageOptions options);

		[Export ("initWithTargetedCGImage:options:completionHandler:")]
		NativeHandle Constructor (CGImage cgImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CGImage cgImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCGImage:orientation:options:")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (cgImage, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCGImage:orientation:options:completionHandler:")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (cgImage, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CGImage cgImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:options:")]
		NativeHandle Constructor (CIImage ciImage, NSDictionary optionsDict);

		[Wrap ("this (ciImage, options.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage ciImage, VNImageOptions options);

		[Export ("initWithTargetedCIImage:options:completionHandler:")]
		NativeHandle Constructor (CIImage ciImage, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CIImage ciImage, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedCIImage:orientation:options:")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (ciImage, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedCIImage:orientation:options:completionHandler:")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (ciImage, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (CIImage ciImage, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:options:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions options);

		[Export ("initWithTargetedImageURL:options:completionHandler:")]
		NativeHandle Constructor (NSUrl imageUrl, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSUrl imageUrl, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageURL:orientation:options:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageUrl, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageURL:orientation:options:completionHandler:")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageUrl, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSUrl imageUrl, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:options:")]
		NativeHandle Constructor (NSData imageData, NSDictionary optionsDict);

		[Wrap ("this (imageData, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions options);

		[Export ("initWithTargetedImageData:options:completionHandler:")]
		NativeHandle Constructor (NSData imageData, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSData imageData, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		[Export ("initWithTargetedImageData:orientation:options:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict);

		[Wrap ("this (imageData, orientation, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options);

		[Export ("initWithTargetedImageData:orientation:options:completionHandler:")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, NSDictionary optionsDict, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Wrap ("this (imageData, orientation, options.GetDictionary ()!, completionHandler)")]
		NativeHandle Constructor (NSData imageData, CGImagePropertyOrientation orientation, VNImageOptions options, VNRequestCompletionHandler completionHandler);

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNGenerateOpticalFlowRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNGenerateOpticalFlowRequestRevision> (WeakSupportedRevisions)")]
		VNGenerateOpticalFlowRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNGenerateOpticalFlowRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNGenerateOpticalFlowRequestRevision CurrentRevision { get; }

		[TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("keepNetworkOutput")]
		bool KeepNetworkOutput { get; set; }
	}

	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (VNStatefulRequest))]
	interface VNGeneratePersonSegmentationRequest {
		[Static]
		[Export ("new")]
		[return: Release]
		VNGeneratePersonSegmentationRequest Create ();

		[Export ("initWithCompletionHandler:")]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("initWithFrameAnalysisSpacing:completionHandler:")]
		NativeHandle Constructor (CMTime frameAnalysisSpacing, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("qualityLevel", ArgumentSemantic.Assign)]
		VNGeneratePersonSegmentationRequestQualityLevel QualityLevel { get; set; }

		[Export ("outputPixelFormat")]
		uint OutputPixelFormat { get; set; }

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNPixelBufferObservation [] Results { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNObservation))]
	[DisableDefaultCtor]
	interface VNTrajectoryObservation {

		[Export ("detectedPoints", ArgumentSemantic.Copy)]
		VNPoint [] DetectedPoints { get; }

		[Export ("projectedPoints", ArgumentSemantic.Copy)]
		VNPoint [] ProjectedPoints { get; }

		[Export ("equationCoefficients")]
		Vector3 EquationCoefficients {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("movingAverageRadius")]
		nfloat MovingAverageRadius { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNObservation))]
	[DisableDefaultCtor]
	interface VNContoursObservation {

		[Field ("VNRecognizedPointGroupKeyAll")]
		NSString RecognizedPointGroupKeyAll { get; }

		[Export ("contourCount")]
		nint ContourCount { get; }

		[Export ("contourAtIndex:error:")]
		[return: NullAllowed]
		VNContour GetContour (nint contourIndex, [NullAllowed] out NSError error);

		[Export ("topLevelContourCount")]
		nint TopLevelContourCount { get; }

		[Export ("topLevelContours")]
		VNContour [] TopLevelContours { get; }

		[Export ("contourAtIndexPath:error:")]
		[return: NullAllowed]
		VNContour GetContour (NSIndexPath indexPath, [NullAllowed] out NSError error);

		[Export ("normalizedPath")]
		CGPath NormalizedPath { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNObservation))]
	[DisableDefaultCtor]
	interface VNRecognizedPointsObservation {

		[Advice ("You can use 'GetAvailableKeys<T> ()' where 'T' can be 'VNHumanBodyPoseObservationJointName' or 'VNHumanHandPoseObservationJointName' enum.")]
		[Export ("availableKeys", ArgumentSemantic.Copy)]
		NSString [] AvailableKeys { get; }

		[Advice ("You can use 'GetAvailableGroupKeys<T> ()' where 'T' can be 'VNHumanBodyPoseObservationJointsGroupName' or 'VNHumanHandPoseObservationJointsGroupName' enum.")]
		[Export ("availableGroupKeys", ArgumentSemantic.Copy)]
		NSString [] AvailableGroupKeys { get; }

		[Export ("recognizedPointForKey:error:")]
		[return: NullAllowed]
		VNRecognizedPoint GetRecognizedPoint (NSString pointKey, out NSError error);

		[Wrap ("GetRecognizedPoint (handLandmark.GetConstant ()!, out error)")]
		[return: NullAllowed]
		VNRecognizedPoint GetRecognizedPoint (VNHumanHandPoseObservationJointName handLandmark, out NSError error);

		[Wrap ("GetRecognizedPoint (bodyLandmark.GetConstant ()!, out error)")]
		[return: NullAllowed]
		VNRecognizedPoint GetRecognizedPoint (VNHumanBodyPoseObservationJointName bodyLandmark, out NSError error);

		[Export ("recognizedPointsForGroupKey:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, VNRecognizedPoint> GetRecognizedPoints (NSString groupKey, out NSError error);

		[Wrap ("GetRecognizedPoints (bodyLandmarkRegion.GetConstant ()!, out error)")]
		[return: NullAllowed]
		NSDictionary<NSString, VNRecognizedPoint> GetRecognizedPoints (VNHumanBodyPoseObservationJointsGroupName bodyLandmarkRegion, out NSError error);

		[Wrap ("GetRecognizedPoints (handLandmarkRegion.GetConstant ()!, out error)")]
		[return: NullAllowed]
		NSDictionary<NSString, VNRecognizedPoint> GetRecognizedPoints (VNHumanHandPoseObservationJointsGroupName handLandmarkRegion, out NSError error);

		[Export ("keypointsMultiArrayAndReturnError:")]
		[return: NullAllowed]
		MLMultiArray GetKeypoints (out NSError error);
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNStatefulRequest {

		[Export ("initWithFrameAnalysisSpacing:completionHandler:")]
		NativeHandle Constructor (CMTime frameAnalysisSpacing, [NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("minimumLatencyFrameCount")]
		nint MinimumLatencyFrameCount { get; }

		[Export ("frameAnalysisSpacing")]
		CMTime FrameAnalysisSpacing { get; }

		// We must inline the following 5 properties
		// ('Revision', 'WeakSupportedRevisions', 'SupportedRevisions', 'DefaultRevision' and 'CurrentRevision')
		// into subclasses so the correct class_ptr is used for the static members and the right enum type is also used.

		[Export ("revision")]
		VNStatefulRequestRevision Revision { get; set; }

		[Static]
		[Export ("supportedRevisions", ArgumentSemantic.Copy)]
		NSIndexSet WeakSupportedRevisions { get; }

		[Static]
		[Wrap ("GetSupportedVersions<VNStatefulRequestRevision> (WeakSupportedRevisions)")]
		VNStatefulRequestRevision [] SupportedRevisions { get; }

		[Static]
		[Export ("defaultRevision")]
		VNStatefulRequestRevision DefaultRevision { get; }

		[Static]
		[Export ("currentRevision")]
		VNStatefulRequestRevision CurrentRevision { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNVideoProcessor {

		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl videoUrl);

		[Export ("addRequest:processingOptions:error:")]
		bool AddRequest (VNRequest request, VNVideoProcessorRequestProcessingOptions processingOptions, [NullAllowed] out NSError error);

		[Export ("removeRequest:error:")]
		bool RemoveRequest (VNRequest request, [NullAllowed] out NSError error);

		[Export ("analyzeTimeRange:error:")]
		bool Analyze (CMTimeRange timeRange, [NullAllowed] out NSError error);

		[Export ("cancel")]
		void Cancel ();
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNVideoProcessorCadence : NSCopying {

	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNVideoProcessorCadence))]
	[DisableDefaultCtor]
	interface VNVideoProcessorFrameRateCadence {

		[Export ("initWithFrameRate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (nint frameRate);

		[Export ("frameRate")]
		nint FrameRate { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (VNVideoProcessorCadence))]
	[DisableDefaultCtor]
	interface VNVideoProcessorTimeIntervalCadence {

		[Export ("initWithTimeInterval:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double timeInterval);

		[Export ("timeInterval")]
		double TimeInterval { get; }
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface VNVideoProcessorRequestProcessingOptions : NSCopying {

		[NullAllowed, Export ("cadence", ArgumentSemantic.Copy)]
		VNVideoProcessorCadence Cadence { get; set; }
	}

	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (VNImageBasedRequest))]
	interface VNDetectDocumentSegmentationRequest {
		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNRectangleObservation [] Results { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNGenerateForegroundInstanceMaskRequest {
		[Export ("initWithCompletionHandler:")]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNInstanceMaskObservation [] Results { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNGeneratePersonInstanceMaskRequest {
		[Export ("initWithCompletionHandler:")]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNInstanceMaskObservation [] Results { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNStatefulRequest))]
	[DisableDefaultCtor]
	interface VNTrackTranslationalImageRegistrationRequest {
		[Export ("initWithCompletionHandler:")]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNImageTranslationAlignmentObservation [] Results { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNStatefulRequest))]
	[DisableDefaultCtor]
	interface VNTrackHomographicImageRegistrationRequest {
		[Export ("initWithCompletionHandler:")]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNImageHomographicAlignmentObservation [] Results { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNStatefulRequest))]
	[DisableDefaultCtor]
	interface VNTrackOpticalFlowRequest {
		[Export ("initWithCompletionHandler:")]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("computationAccuracy", ArgumentSemantic.Assign)]
		VNTrackOpticalFlowRequestComputationAccuracy ComputationAccuracy { get; set; }

		[Export ("outputPixelFormat")]
		uint OutputPixelFormat { get; set; }

		[Export ("keepNetworkOutput")]
		bool KeepNetworkOutput { get; set; }

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNPixelBufferObservation [] Results { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNImageBasedRequest))]
	[DisableDefaultCtor]
	interface VNDetectAnimalBodyPoseRequest {
		[Export ("initWithCompletionHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("supportedJointNamesAndReturnError:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNAnimalBodyPoseObservationJointName []))]
		NSString [] GetSupportedJointNames ([NullAllowed] out NSError error);

		[Export ("supportedJointsGroupNamesAndReturnError:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNAnimalBodyPoseObservationJointsGroupName []))]
		NSString [] GetSupportedJointsGroupNames ([NullAllowed] out NSError error);

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNAnimalBodyPoseObservation [] Results { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNStatefulRequest))]
	[DisableDefaultCtor]
	interface VNDetectHumanBodyPose3DRequest {
		[Export ("initWithCompletionHandler:")]
		NativeHandle Constructor ([NullAllowed] VNRequestCompletionHandler completionHandler);

		[Export ("supportedJointNamesAndReturnError:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNHumanBodyPose3DObservationJointName []))]
		NSString [] GetSupportedJointNames ([NullAllowed] out NSError error);

		[Export ("supportedJointsGroupNamesAndReturnError:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNHumanBodyPose3DObservationJointsGroupName []))]
		NSString [] GetSupportedJointsGroupNames ([NullAllowed] out NSError error);

		[NullAllowed, Export ("results", ArgumentSemantic.Copy)]
		VNHumanBodyPose3DObservation [] Results { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VNPoint3D : NSCopying, NSSecureCoding {
		[Export ("initWithPosition:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[DesignatedInitializer]
		NativeHandle Constructor (Vector4 position);

		[Export ("position")]
		Vector4 Position {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNPoint3D))]
	[DisableDefaultCtor]
	interface VNRecognizedPoint3D {
		[Export ("initWithPosition:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[DesignatedInitializer]
		NativeHandle Constructor (Vector4 position);

		[Export ("identifier")]
		string Identifier { get; }

		[Field ("VNRecognizedPoint3DGroupKeyAll")]
		NSString GroupKeyAll { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNObservation))]
	[DisableDefaultCtor]
	interface VNRecognizedPoints3DObservation {
		[Export ("availableKeys", ArgumentSemantic.Copy)]
		NSString [] AvailableKeys { get; }

		[Export ("availableGroupKeys", ArgumentSemantic.Copy)]
		NSString [] AvailableGroupKeys { get; }

		[Export ("recognizedPointForKey:error:")]
		[return: NullAllowed]
		VNRecognizedPoint3D GetRecognizedPoint (NSString pointKey, [NullAllowed] out NSError error);

		[Export ("recognizedPointsForGroupKey:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, VNRecognizedPoint3D> GetRecognizedPoints (NSString groupKey, [NullAllowed] out NSError error);
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNRecognizedPoint3D))]
	[DisableDefaultCtor]
	interface VNHumanBodyRecognizedPoint3D {
		[Export ("initWithPosition:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[DesignatedInitializer]
		NativeHandle Constructor (Vector4 position);

		[Export ("localPosition")]
		Vector4 LocalPosition {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("parentJoint")]
		VNHumanBodyPose3DObservationJointName ParentJoint { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNObservation))]
	[DisableDefaultCtor]
	interface VNInstanceMaskObservation {
		[Export ("instanceMask")]
		CVPixelBuffer InstanceMask { get; }

		[Export ("allInstances", ArgumentSemantic.Copy)]
		NSIndexSet AllInstances { get; }

		[Export ("generateMaskForInstances:error:")]
		[return: NullAllowed]
		CVPixelBuffer GenerateMask (NSIndexSet instances, [NullAllowed] out NSError error);

		[Export ("generateMaskedImageOfInstances:fromRequestHandler:croppedToInstancesExtent:error:")]
		[return: NullAllowed]
		CVPixelBuffer GenerateMaskedImage (NSIndexSet instances, VNImageRequestHandler requestHandler, bool cropResult, [NullAllowed] out NSError error);

		[Export ("generateScaledMaskForImageForInstances:fromRequestHandler:error:")]
		[return: NullAllowed]
		CVPixelBuffer GenerateScaledMask (NSIndexSet instances, VNImageRequestHandler requestHandler, [NullAllowed] out NSError error);
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNRecognizedPoints3DObservation))]
	[DisableDefaultCtor]
	interface VNHumanBodyPose3DObservation {
		[Export ("heightEstimation")]
		VNHumanBodyPose3DObservationHeightEstimation HeightEstimation { get; }

		[Export ("cameraOriginMatrix")]
		Vector4 CameraOriginMatrix {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("availableJointsGroupNames", ArgumentSemantic.Copy)]
		[BindAs (typeof (VNHumanBodyPose3DObservationJointsGroupName []))]
		NSString [] AvailableJointsGroupNames { get; }

		[Export ("availableJointNames", ArgumentSemantic.Copy)]
		[BindAs (typeof (VNHumanBodyPose3DObservationJointName []))]
		NSString [] AvailableJointNames { get; }

		[Export ("bodyHeight")]
		float BodyHeight { get; }

		[Export ("recognizedPointsForJointsGroupName:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, VNHumanBodyRecognizedPoint3D> GetRecognizedPoints ([BindAs (typeof (VNHumanBodyPose3DObservationJointsGroupName))] NSString jointName, [NullAllowed] out NSError error);

		[Export ("recognizedPointForJointName:error:")]
		[return: NullAllowed]
		VNHumanBodyRecognizedPoint3D GetRecognizedPoint ([BindAs (typeof (VNHumanBodyPose3DObservationJointName))] NSString jointName, [NullAllowed] out NSError error);

		[Export ("pointInImageForJointName:error:")]
		[return: NullAllowed]
		VNPoint GetPointInImage ([BindAs (typeof (VNHumanBodyPose3DObservationJointName))] NSString jointName, [NullAllowed] out NSError error);

		[Export ("parentJointNameForJointName:")]
		[return: NullAllowed]
		[return: BindAs (typeof (VNHumanBodyPose3DObservationJointName))]
		NSString GetParentJointName ([BindAs (typeof (VNHumanBodyPose3DObservationJointName))] NSString jointName);

		[Export ("getCameraRelativePosition:forJointName:error:")]
		bool GetCameraRelativePosition (ref Vector4 modelPositionOut, [BindAs (typeof (VNHumanBodyPose3DObservationJointName))] NSString jointName, [NullAllowed] out NSError error);
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (VNRecognizedPointsObservation))]
	[DisableDefaultCtor]
	interface VNAnimalBodyPoseObservation {
		[Export ("availableJointNames", ArgumentSemantic.Copy)]
		[BindAs (typeof (VNAnimalBodyPoseObservationJointName []))]
		NSString [] AvailableJointNames { get; }

		[Export ("availableJointGroupNames", ArgumentSemantic.Copy)]
		[BindAs (typeof (VNAnimalBodyPoseObservationJointsGroupName []))]
		NSString [] AvailableJointGroupNames { get; }

		[Export ("recognizedPointForJointName:error:")]
		[return: NullAllowed]
		VNRecognizedPoint GetRecognizedPoint ([BindAs (typeof (VNAnimalBodyPoseObservationJointName))] NSString jointName, [NullAllowed] out NSError error);

		[Export ("recognizedPointsForJointsGroupName:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, VNRecognizedPoint> GetRecognizedPoints ([BindAs (typeof (VNAnimalBodyPoseObservationJointsGroupName))] NSString jointsGroupName, [NullAllowed] out NSError error);
	}
}
