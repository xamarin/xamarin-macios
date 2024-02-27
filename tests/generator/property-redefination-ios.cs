using Foundation;

namespace Test {
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	public interface NSTextInputClient {
		[Export ("selectedRange")]
		NSRange SelectedRange { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSText {
		[Export ("selectedRange")]
		NSRange SelectedRange { get; set; }
	}

	[BaseType (typeof (NSText))]
	interface NSTextView : NSTextInputClient {
	}
}
