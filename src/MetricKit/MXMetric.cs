#nullable enable

namespace MetricKit {

	public partial class MXMetric {
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		public virtual NSDictionary DictionaryRepresentation {
			get {
				return _DictionaryRepresentation14;
			}
		}
	}
}
