namespace WebKit {

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	public partial class DomCssRuleList {
		public DomCssRule this [int index] {
			get {
				return GetItem (index);
			}
		}
	}
}
