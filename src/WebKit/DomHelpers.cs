using System;
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;

namespace WebKit {
#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public partial class DomHtmlSelectElement { 
		public DomNode this [string name] { get { return this.NamedItem (name); } }
		public DomNode this [uint index] { get { return this.GetItem (index); } } 
	}
#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public partial class DomHtmlOptionsCollection { 
		public DomNode this [string name] { get { return this.NamedItem (name); } }
		public DomNode this [uint index] { get { return this.GetItem(index); } }
	}

}
