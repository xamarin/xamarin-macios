using System;
using Foundation;

[BaseType (typeof (NSObject))]
public interface FooType<GenType>
	where GenType : NSObject {

	[Export ("bar")]
	GenType bar { get; set; }

	[Static]
	[Export ("fooType:value:")]
	GenType FromFoo (string fooType, GenType value);

	[Export ("initWithFoo:value:")]
	IntPtr Constructor (string foo, GenType value);

	[Export ("getBar:")]
	GenType GetBar (string bar);
}
