using System;
using Foundation;

namespace UnderlyingFieldType {
	[BaseType (typeof (NSObject))]
	interface FieldClass {
		[Field ("SByteField", "__Internal")]
		sbyte SByteField { get; set; }

		[Field ("ByteField", "__Internal")]
		byte ByteField { get; set; }

		[Field ("ShortField", "__Internal")]
		short ShortField { get; set; }

		[Field ("UShortField", "__Internal")]
		ushort UShortField { get; set; }

		[Field ("IntField", "__Internal")]
		int IntField { get; set; }

		[Field ("UIntField", "__Internal")]
		uint UIntField { get; set; }

		[Field ("LongField", "__Internal")]
		long LongField { get; set; }

		[Field ("ULongField", "__Internal")]
		ulong ULongField { get; set; }

		[Field ("DoubleField", "__Internal")]
		double DoubleField { get; set; }

		[Field ("UIntPtrField", "__Internal")]
		UIntPtr UIntPtrField { get; set; }

		[Field ("IntPtrield", "__Internal")]
		IntPtr IntPtrField { get; set; }
	}
}
