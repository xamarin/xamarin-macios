using System;

using Foundation;

using ObjCRuntime;

namespace NS {
	[Native (ConvertToManaged = "Extensions.ToManaged1", ConvertToNative = "Extensions.ToNative1")]
	public enum MyEnum1 : long {
		Value1,
		Value2,
	}

	[Native (ConvertToManaged = "Extensions.ToManaged2", ConvertToNative = "Extensions.ToNative2")]
	public enum MyEnum2 : long {
		Value1 = long.MinValue,
		Value2 = long.MaxValue,
	}

	[Native (ConvertToManaged = "Extensions.ToManaged3", ConvertToNative = "Extensions.ToNative3")]
	public enum MyEnum3 : ulong {
		Value1 = ulong.MinValue,
		Value2 = ulong.MaxValue,
	}

	[Native (ConvertToManaged = "Extensions.ToManaged4", ConvertToNative = "Extensions.ToNative4")]
	public enum MyEnum4 : long {
		Zero,
		One,
		Two
	}

	[Native (ConvertToManaged = "Extensions.ToManaged5", ConvertToNative = "Extensions.ToNative5")]
	public enum MyEnum5 : ulong {
		Zero,
		One,
		Two
	}

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

	delegate void EnumHandler (MyEnum7 seven, MyEnum8 eight);

	[BaseType (typeof (NSObject))]
	interface MyClass {
		[Export ("myProp1")]
		MyEnum1 MyProp1 { get; set; }
		[Export ("myMethod1:")]
		MyEnum1 MyMethod1 (MyEnum1 arg);

		[Export ("myProp2")]
		MyEnum2 MyProp2 { get; set; }
		[Export ("myMethod2:")]
		MyEnum2 MyMethod2 (MyEnum2 arg);

		[Export ("myProp3")]
		MyEnum3 MyProp3 { get; set; }
		[Export ("myMethod3:")]
		MyEnum3 MyMethod3 (MyEnum3 arg);

		[Field ("NIntField", "__Internal")]
		MyEnum4 FooNIntField { get; set; }

		[Field ("NUIntField", "__Internal")]
		MyEnum5 FooNUIntField { get; set; }

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

		[Export ("myDelegateProp")]
		EnumHandler MyDelegateProp { get; set; }
		[Export ("myDelegateMethod:")]
		EnumHandler MyDelegateMethod (EnumHandler arg);
	}
}
