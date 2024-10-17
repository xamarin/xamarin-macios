using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureLevel {
	[Field ("AVCaptureSystemPressureLevelNominal")]
	Nominal,

	[Field ("AVCaptureSystemPressureLevelFair")]
	Fair,

	[Field ("AVCaptureSystemPressureLevelSerious")]
	Serious,

	[Field ("AVCaptureSystemPressureLevelCritical")]
	Critical,

	[Field ("AVCaptureSystemPressureLevelShutdown")]
	Shutdown,
}
