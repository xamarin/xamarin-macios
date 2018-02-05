//
// NSProxy.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

// HACK:
// Yep this is just a hack to workaround https://bugzilla.xamarin.com/show_bug.cgi?id=59247
// we need to have a better solution that removes all what NSProxy does not responds to.
// Right now this works because NSObject implements NSObject protocol just like NSProxy, but
// NSProxy responds to far less selectors hence doing it internal so it is not a breaking change
// when we provide the correct fix.

using System;
using System.ComponentModel;
using Foundation;
using ObjCRuntime;

namespace Foundation {
	[EditorBrowsable (EditorBrowsableState.Never)]
	[Register ("NSProxy", true)]
	internal abstract class NSProxy : NSObject {
	}
}

namespace WebKit {
	// We need to keep NSProxy if WKNavigationDelegate or IWKNavigationDelegate are used
	// This cannot be done on an interface but the protocol won't be used without a WKWebView
	// so a reference (from the static constructor) ensure NSProxy will be available
	public partial class WKWebView {
		static Type hack = typeof (NSProxy);
	}
}
