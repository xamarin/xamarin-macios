using System;
using Foundation;
using ObjCRuntime;

namespace NS {

	// injecting custom code makes the Dispose method not-optimizable by default
	[Dispose ("Console.WriteLine (\"Disposing!\");")]
	[BaseType (typeof (NSObject))]
	interface WithDispose {

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }
	}

	// but we can opt-in to make it optimizable
	[Dispose ("// just a comment, that's safe to optimize, if not very useful otherwise", Optimizable = true)]
	[BaseType (typeof (NSObject))]
	interface WithDisposeOptInOptimizable {

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }
	}

	// if nothing is injected then we know we generate code that our tools can optimize
	[BaseType (typeof (NSObject))]
	interface WithoutDispose {

		// this ensure we have a Dispose method generated for the type
		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }
	}
}
