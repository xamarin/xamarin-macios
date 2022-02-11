#if !__MACCATALYST__
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using ObjCRuntime;

namespace VideoSubscriberAccount {

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("tvos10.0")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class VSAccountMetadataRequest {

#if NET
		[SupportedOSPlatform ("tvos10.1")]
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("macos10.14")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[TV (10,1)]
		[iOS (10,2)]
#endif
		public VSAccountProviderAuthenticationScheme[] SupportedAuthenticationSchemes {
			get {
				return VSAccountProviderAuthenticationSchemeExtensions.GetValues (SupportedAuthenticationSchemesString);
			}
			set {
				SupportedAuthenticationSchemesString = value?.GetConstants ();
			}
		}
	}
}
#endif // !__MACCATALYST__
