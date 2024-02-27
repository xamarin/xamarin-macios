using Foundation;

namespace Bug {
	delegate string ReturnsString ();

	[BaseType (typeof (NSObject))]
	interface Type {
		[Export ("returnString")]
		ReturnsString ReturnString { get; set; }
	}
}
