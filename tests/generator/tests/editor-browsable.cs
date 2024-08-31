using System;
using System.ComponentModel;

using Foundation;

namespace EditorBrowsable {
	[EditorBrowsable (EditorBrowsableState.Never)]
	enum StrongEnum {
		[Field ("AField", "__Internal")]
		A,
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[BaseType (typeof (NSObject))]
	interface ObjCClass {
		[Export ("strongEnumField")]
		StrongEnum StrongEnumField { get; set; }
	}
}
