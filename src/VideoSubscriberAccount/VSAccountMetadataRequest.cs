using System;
using System.Threading.Tasks;
using ObjCRuntime;

namespace VideoSubscriberAccount {

	public partial class VSAccountMetadataRequest {

		[TV (10,1)][iOS (10,2)]
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
