#if IOS

#nullable enable

namespace CoreBluetooth {
	public partial class CBManager {

		[SupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		public static CBManagerAuthorization Authorization {
			get {
				return _SAuthorization;
			}
		}
	}
}
#endif
