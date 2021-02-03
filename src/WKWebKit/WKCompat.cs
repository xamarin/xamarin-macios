using System;

using Foundation;

#nullable enable

namespace WebKit {

#if !XAMCORE_4_0
	public partial class WKWebsiteDataStore {

		[Obsolete ("This constructor does not create a valid instance of the type.")]
		public WKWebsiteDataStore ()
		{
		}
	}
#endif
}
