
#if !XAMCORE_2_0
using Foundation;
#else
using Foundation;
#endif

namespace Test
{
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	public interface NSTextInputClient
	{
		NSRange SelectedRange { get; }
	}
}