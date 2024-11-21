using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureSystemPressureLevel {
	[Field<EnumValue> ("AVCaptureSystemPressureLevelNominal")]
	Nominal,

	[Field<EnumValue> ("AVCaptureSystemPressureLevelFair")]
	Fair,

	[Field<EnumValue> ("AVCaptureSystemPressureLevelSerious")]
	Serious,

	[Field<EnumValue> ("AVCaptureSystemPressureLevelCritical")]
	Critical,

	[Field<EnumValue> ("AVCaptureSystemPressureLevelShutdown")]
	Shutdown,
}
