using System;
using System.Runtime.Versioning;

namespace WebKit {

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public partial class DomCssRuleList {
		public DomCssRule this [int index] {
			get {
				return GetItem (index);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public partial class DomCssStyleDeclaration {
		public string this [int index] {
			get {
				return GetItem (index);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public partial class DomHtmlCollection {
		public DomNode this [int index] {
			get {
				return GetItem (index);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public partial class DomMediaList {
		public string this [int index] {
			get {
				return GetItem (index);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
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

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public partial class DomNodeList {
		public DomNode this [int index] {
			get {
				return GetItem (index);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public partial class DomStyleSheetList {
		public DomStyleSheet this [int index] {
			get {
				return GetItem (index);
			}

		}
	}
}
