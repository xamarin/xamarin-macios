#if !__MACCATALYST__
using CoreText;
using Foundation;
using System.Runtime.Versioning;

namespace AppKit {
#if NET
	[SupportedOSPlatform ("maccatalyst13.0")]
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class NSTextStorage {
		public NSTextStorage (string str, NSDictionary attributes) : base (str, attributes) {
		}

		public NSTextStorage (NSAttributedString other) : base (other) {
		}

		public NSTextStorage (string str, CTStringAttributes attributes) : base (str, attributes) {
		}
	}
}
#endif // !__MACCATALYST__
