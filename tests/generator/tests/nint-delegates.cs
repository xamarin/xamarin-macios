using System;

using Foundation;

using ObjCRuntime;

namespace NS {
	[NoMacCatalyst]
	[BaseType (typeof (NSObject), Delegates = new string [] { "Delegate" }, Events = new Type [] { typeof (NSTableViewDelegate) })]
	partial interface NSTableView {
		[NullAllowed]
		[Export ("delegate")]
		NSObject Delegate { get; set; }

	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface NSTableViewDelegate {
		[Export ("row:"), DelegateName ("NSTableViewColumnRowPredicate"), DefaultValue (0)]
		nint ShouldEditTableColumn (nint row);
	}

	[BaseType (typeof (NSObject))]
	interface DelegateMethods {
		[Export ("delegates:b:c:")]
		void DelegateSomething (D1 a, D2 b, D3 c/*, D4 d*/);
	}

	delegate nint D1 (nint a);
	delegate nuint D2 (ref nuint b);
	delegate nint D3 (out nint c);
}
