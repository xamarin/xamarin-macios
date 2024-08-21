using System;
using Foundation;

namespace BackingField {
	[BackingFieldType (typeof (nint))]
	enum NSIntegerFieldType {
		[Field ("AField", "__Internal")]
		A,
	}

	[BackingFieldType (typeof (nuint))]
	enum NSUIntegerFieldType {
		[Field ("BField", "__Internal")]
		B,
	}

	[BackingFieldType (typeof (NSNumber))]
	enum NSNumberFieldType {
		[Field ("CField", "__Internal")]
		C,
	}

	[BaseType (typeof (NSObject))]
	interface SomeObj {
		[Export ("nsIntegerField")]
		NSIntegerFieldType NSIntegerField { get; set; }

		[Export ("nsuIntegerField")]
		NSUIntegerFieldType NSUIntegerField { get; set; }

		[Export ("nsNumberField")]
		NSNumberFieldType NSNumberField { get; set; }
	}
}
