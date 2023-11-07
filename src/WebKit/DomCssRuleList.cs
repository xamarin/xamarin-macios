#if __MACOS__

#nullable enable

namespace WebKit {

	public partial class DomCssRuleList {
		public DomCssRule this [int index] {
			get {
				return GetItem (index);
			}
		}
	}
}

#endif // __MACOS__
