using System;
using System.Threading.Tasks;
using XamCore.ObjCRuntime;

namespace XamCore.VideoSubscriberAccount {

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
