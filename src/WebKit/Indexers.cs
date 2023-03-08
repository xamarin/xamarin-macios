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

	public partial class DomCssStyleDeclaration {
		public string this [int index] {
			get {
				return GetItem (index);
			}
		}
	}

	public partial class DomHtmlCollection {
		public DomNode this [int index] {
			get {
				return GetItem (index);
			}
		}
	}

	public partial class DomMediaList {
		public string this [int index] {
			get {
				return GetItem (index);
			}
		}
	}

	public partial class DomNamedNodeMap {
		public DomNode this [int index] {
			get {
				return GetItem (index);
			}
		}

		public DomNode this [string name] {
			get {
				return GetNamedItem (name);
			}
		}
	}

	public partial class DomNodeList {
		public DomNode this [int index] {
			get {
				return GetItem (index);
			}
		}
	}

	public partial class DomStyleSheetList {
		public DomStyleSheet this [int index] {
			get {
				return GetItem (index);
			}
		}
	}
}

#endif // __MACOS__
