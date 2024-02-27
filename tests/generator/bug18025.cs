using System;
using Foundation;

[BaseType (typeof (NSObject))]
public interface FooType {
	[Introduced (PlatformName.iOS, 8, 0)]
	[Export ("getBar:")]
	string GetBar (string bar);
}
