#if !__MACCATALYST__
using CoreText;
using Foundation;

#nullable enable

namespace AppKit {
	public partial class NSTextStorage {
		public NSTextStorage (string str, NSDictionary attributes) : base (str, attributes)
		{
		}

		public NSTextStorage (NSAttributedString other) : base (other)
		{
		}

		public NSTextStorage (string str, CTStringAttributes attributes) : base (str, attributes)
		{
		}
	}
}
#endif // !__MACCATALYST__
