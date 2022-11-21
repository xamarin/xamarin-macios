using System;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace PassKit {

#if !TVOS && !WATCH
	public partial class PKShareablePassMetadata {

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
#endif
		public enum CardType {
			Template = 0,
			Configuration = 1,
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
#endif
		public PKShareablePassMetadata (string credentialIdentifier, string sharingInstanceIdentifier, string templateIdentifier, PKShareablePassMetadataPreview preview) :
			this (credentialIdentifier, sharingInstanceIdentifier, templateIdentifier, preview, CardType.Template)
		{
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
#endif
		public PKShareablePassMetadata (string credentialIdentifier, string sharingInstanceIdentifier, string templateIdentifier, PKShareablePassMetadataPreview preview, CardType cardType) : base (NSObjectFlag.Empty)
		{
			switch (cardType) {
			case CardType.Template:
				InitializeHandle (InitWithCardTemplate (credentialIdentifier, sharingInstanceIdentifier, templateIdentifier, preview),
					"initWithProvisioningCredentialIdentifier:sharingInstanceIdentifier:cardTemplateIdentifier:preview:");
				break;
			case CardType.Configuration:
				InitializeHandle (InitWithCardConfiguration (credentialIdentifier, sharingInstanceIdentifier, templateIdentifier, preview),
					"initWithProvisioningCredentialIdentifier:sharingInstanceIdentifier:cardConfigurationIdentifier:preview:");
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (cardType));
			}
		}
	}
#endif // !TVOS && !WATCH
}
