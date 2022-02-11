using System.Runtime.Versioning;

namespace Foundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class NSUrlRequest {
		public string this [string key] {
			get {
				return Header (key);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class NSMutableUrlRequest {
		public new string this [string key] {
			get {
				return Header (key);
			}

			set {
				_SetValue (value, key);
			}
		}
	}
}
