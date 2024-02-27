using System;

using UIKit;

namespace Bug34042.Foo {
	[BaseType (typeof (UIButton))]
	interface FooButton {

	}
}

namespace Bug34042.Bar {
	[BaseType (typeof (Bug34042.Foo.FooButton))]
	interface BarButton {

	}
}
