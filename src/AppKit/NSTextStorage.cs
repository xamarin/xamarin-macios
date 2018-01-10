using CoreText;
using Foundation;

namespace AppKit {
	public partial class NSTextStorage {
		public NSTextStorage (string str, NSDictionary attributes) : base (str, attributes) {
		}

		public NSTextStorage (NSAttributedString other) : base (other) {
		}

		public NSTextStorage (string str, CTStringAttributes attributes) : base (str, attributes) {
		}
	}
}
