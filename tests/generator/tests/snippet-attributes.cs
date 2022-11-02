using System;
using Foundation;
using ObjCRuntime;

namespace NS {

	// injecting custom code makes the method method not-optimizable by default
	[BaseType (typeof (NSObject))]
	interface NotOptimizable {

		[PreSnippet ("Console.WriteLine (\"Pre!\");")]
		[Export ("pre")]
		void Pre ();

		[PrologueSnippet ("Console.WriteLine (\"Prologue!\");")]
		[Export ("prologue")]
		void Prologue ();

		[PostSnippet ("Console.WriteLine (\"Post!\");")]
		[Export ("post")]
		void Post ();
	}

	// but we can opt-in to make it optimizable
	[BaseType (typeof (NSObject))]
	interface OptInOptimizable {

		[PreSnippet ("Console.WriteLine (\"Pre!\");", Optimizable = true)]
		[Export ("pre")]
		void Pre ();

		[PrologueSnippet ("Console.WriteLine (\"Prologue!\");", Optimizable = true)]
		[Export ("prologue")]
		void Prologue ();

		[PostSnippet ("Console.WriteLine (\"Post!\");", Optimizable = true)]
		[Export ("post")]
		void Post ();
	}

	// if nothing is injected then we know we generate code that our tools can optimize
	[BaseType (typeof (NSObject))]
	interface NoSnippet {

		[Export ("nothing")]
		void Nothing ();
	}
}
