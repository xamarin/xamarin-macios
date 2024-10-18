using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace CustomLibrary;

[BindingType]
public enum CustomLibraryEnum {
	[Field ("None", "/path/to/customlibrary.framework")]
	None,
	[Field ("Medium", "/path/to/customlibrary.framework")]
	Medium,
	[Field ("High", "/path/to/customlibrary.framework")]
	High,
}
