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

	[BackingFieldType (typeof (Int32))]
	enum Int32FieldType {
		[Field ("DField", "__Internal")]
		D,
	}

	[BackingFieldType (typeof (Int64))]
	enum Int64FieldType {
		[Field ("EField", "__Internal")]
		E,
	}

	[BackingFieldType (typeof (UInt32))]
	enum UInt32FieldType {
		[Field ("FField", "__Internal")]
		F,
	}

	[BackingFieldType (typeof (UInt64))]
	enum UInt64FieldType {
		[Field ("GField", "__Internal")]
		G,
	}

	[BaseType (typeof (NSObject))]
	interface SomeObj {
		[Export ("nsIntegerField")]
		NSIntegerFieldType NSIntegerField { get; set; }

		[Export ("nsuIntegerField")]
		NSUIntegerFieldType NSUIntegerField { get; set; }

		[Export ("nsNumberField")]
		NSNumberFieldType NSNumberField { get; set; }

		[Export ("int32Field")]
		Int32FieldType Int32Field { get; set; }

		[Export ("int64Field")]
		Int64FieldType Int64Field { get; set; }

		[Export ("uint32Field")]
		UInt32FieldType UInt32Field { get; set; }

		[Export ("uint64Field")]
		UInt64FieldType UInt64Field { get; set; }
	}
}
