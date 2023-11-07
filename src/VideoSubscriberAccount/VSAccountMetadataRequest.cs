#nullable enable

#if !__MACCATALYST__
using System;
using System.Threading.Tasks;
using ObjCRuntime;

namespace VideoSubscriberAccount {

	public partial class VSAccountMetadataRequest {

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#endif
		public VSAccountProviderAuthenticationScheme [] SupportedAuthenticationSchemes {
			get {
				return VSAccountProviderAuthenticationSchemeExtensions.GetValues (SupportedAuthenticationSchemesString);
			}
			set {
				var constants = value.GetConstants ();
				if (constants is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SupportedAuthenticationSchemesString = constants!;
			}
		}
	}
}
#endif // !__MACCATALYST__
