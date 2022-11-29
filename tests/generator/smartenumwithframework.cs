using System;
using ObjCRuntime;
using Foundation;

namespace SmartEnumWithFramework {

	enum FooEnumTest {
		[Field ("First", "+CoreImage")]
		First,

		[Field ("Second", "+CoreImage")]
		Second,
	}
}
