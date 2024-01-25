using System;

using Foundation;

using ObjCRuntime;

namespace FieldEnumTests {

	[Native]
	enum FooNIntEnum : long {
		Zero,
		One,
		Two
	}

	[Native]
	enum FooNUIntEnum : ulong {
		Zero,
		One,
		Two
	}

	enum FooSmartEnum {
		[Field ("ZeroSmartField", "__Internal")]
		Zero,
		[Field ("OneSmartField", "__Internal")]
		One,
		[Field ("TwoSmartField", "__Internal")]
		Two
	}

	enum FooIntEnum {
		Zero,
		One,
		Two
	}

	[BaseType (typeof (NSObject))]
	interface MyFooClass {

		[Field ("UIntField", "__Internal")]
		uint UIntField { get; set; }

		[Field ("ULongField", "__Internal")]
		ulong ULongField { get; set; }

		[Field ("LongField", "__Internal")]
		long LongField { get; set; }

		[Field ("NUIntField", "__Internal")]
		nuint NUIntField { get; set; }

		[Field ("NIntField", "__Internal")]
		nint NIntField { get; set; }

		[Field ("NIntField", "__Internal")]
		FooNIntEnum FooNIntField { get; set; }

		[Field ("NUIntField", "__Internal")]
		FooNUIntEnum FooNUIntField { get; set; }

		[Field ("FooSmartField", "__Internal")]
		FooSmartEnum FooSmartField { get; set; }

		[Field ("FooIntEnumField", "__Internal")]
		FooIntEnum FooIntEnumField { get; set; }
	}
}
