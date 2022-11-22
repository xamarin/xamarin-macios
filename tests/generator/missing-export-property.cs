using Foundation;

namespace Test {
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	public interface NSTextInputClient {
		// missing [Export ("selectRange")] should report an error
		NSRange SelectedRange { get; }
	}
}
