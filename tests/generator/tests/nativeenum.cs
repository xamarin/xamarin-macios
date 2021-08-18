using System;
using Foundation;
using ObjCRuntime;

namespace NS {
	[Native]
	public enum MyEnum6 : long {
		Value1,
		Value2,
	}

	[Native]
	public enum MyEnum7 : long {
		Value1 = long.MinValue,
		Value2 = long.MaxValue,
	}

	[Native]
	public enum MyEnum8 : ulong {
		Value1 = ulong.MinValue,
		Value2 = ulong.MaxValue,
	}

	[BaseType (typeof (NSObject))]
	interface MyClass {
		[Export ("myProp6")]
		MyEnum6 MyProp6 { get; set; }
		[Export ("myMethod6:")]
		MyEnum6 MyMethod6 (MyEnum6 arg);

		[Export ("myProp7")]
		MyEnum7 MyProp7 { get; set; }
		[Export ("myMethod7:")]
		MyEnum7 MyMethod7 (MyEnum7 arg);

		[Export ("myProp8")]
		MyEnum8 MyProp8 { get; set; }
		[Export ("myMethod8:")]
		MyEnum8 MyMethod8 (MyEnum8 arg);
	}
}
