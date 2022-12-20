#if __MACOS__

#nullable enable

using System;
using ObjCRuntime;
using Foundation;

namespace WebKit {
	public partial class DomHtmlSelectElement {
		public DomNode this [string name] { get { return this.NamedItem (name); } }
		public DomNode this [uint index] { get { return this.GetItem (index); } }
	}
	public partial class DomHtmlOptionsCollection {
		public DomNode this [string name] { get { return this.NamedItem (name); } }
		public DomNode this [uint index] { get { return this.GetItem (index); } }
	}
}

#endif // __MACOS__
