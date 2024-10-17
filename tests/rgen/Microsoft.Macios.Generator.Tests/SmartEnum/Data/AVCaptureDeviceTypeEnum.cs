using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureDeviceType {

	[Field ("AVCaptureDeviceTypeBuiltInMicrophone")]
	BuiltInMicrophone,

	[Field ("AVCaptureDeviceTypeBuiltInWideAngleCamera")]
	BuiltInWideAngleCamera,

	[Field ("AVCaptureDeviceTypeBuiltInTelephotoCamera")]
	BuiltInTelephotoCamera,

	[Field ("AVCaptureDeviceTypeBuiltInDuoCamera")]
	BuiltInDuoCamera,

	[Field ("AVCaptureDeviceTypeBuiltInDualCamera")]
	BuiltInDualCamera,

	[Field ("AVCaptureDeviceTypeBuiltInTrueDepthCamera")]
	BuiltInTrueDepthCamera,

	[Field ("AVCaptureDeviceTypeBuiltInUltraWideCamera")]
	BuiltInUltraWideCamera,

	[Field ("AVCaptureDeviceTypeBuiltInTripleCamera")]
	BuiltInTripleCamera,

	[Field ("AVCaptureDeviceTypeBuiltInDualWideCamera")]
	BuiltInDualWideCamera,

	[Field ("AVCaptureDeviceTypeExternalUnknown")]
	ExternalUnknown,

	[Field ("AVCaptureDeviceTypeBuiltInLiDARDepthCamera")]
	BuiltInLiDarDepthCamera,
}
