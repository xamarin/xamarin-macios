
using Foundation;

namespace Test
{
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	public interface NSTextInputClient
	{
		NSRange SelectedRange { get; }
	}
}