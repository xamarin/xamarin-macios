using System;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace PassKit {

#if !TVOS
	public partial class PKShareablePassMetadata {

		/// <summary>This enum describes how to interpret some arguments when creating <see cref="PKShareablePassMetadata" /> instances.</summary>
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
		public enum CardType {
			/// <summary>The 'templateIdentifier' parameter identifies a card template identifier.</summary>
			Template = 0,
			/// <summary>The 'templateIdentifier' parameter identifies a card configuration identifier.</summary>
			Configuration = 1,
		}

		/// <summary>Create a new <see cref="PKShareablePassMetadata" /> instance.</summary>
		/// <param name="credentialIdentifier">The credential identifier for the new <see cref="PKShareablePassMetadata" /> instance.</param>
		/// <param name="sharingInstanceIdentifier">The sharing instance identifer for the new <see cref="PKShareablePassMetadata" /> instance.</param>
		/// <param name="templateIdentifier">The card template identifier for the new <see cref="PKShareablePassMetadata" /> instance.</param>
		/// <param name="preview">The preview for the new <see cref="PKShareablePassMetadata" /> instance.</param>
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
		public PKShareablePassMetadata (string credentialIdentifier, string sharingInstanceIdentifier, string templateIdentifier, PKShareablePassMetadataPreview preview) :
			this (credentialIdentifier, sharingInstanceIdentifier, templateIdentifier, preview, CardType.Template)
		{
		}

		/// <summary>Create a new <see cref="PKShareablePassMetadata" /> instance.</summary>
		/// <param name="credentialIdentifier">The credential identifier for the new <see cref="PKShareablePassMetadata" /> instance.</param>
		/// <param name="sharingInstanceIdentifier">The sharing instance identifer for the new <see cref="PKShareablePassMetadata" /> instance.</param>
		/// <param name="templateIdentifier">The template identifier for the new <see cref="PKShareablePassMetadata" /> instance.</param>
		/// <param name="preview">The preview for the new <see cref="PKShareablePassMetadata" /> instance.</param>
		/// <param name="cardType">Specifies whether the <paramref name="templateIdentifier" /> parameter specifies a card template identifier or a card configuration identifier.</param>
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
		public PKShareablePassMetadata (string credentialIdentifier, string sharingInstanceIdentifier, string templateIdentifier, PKShareablePassMetadataPreview preview, CardType cardType) : base (NSObjectFlag.Empty)
		{
			switch (cardType) {
			case CardType.Template:
				InitializeHandle (_InitWithCardTemplate (credentialIdentifier, sharingInstanceIdentifier, templateIdentifier, preview),
					"initWithProvisioningCredentialIdentifier:sharingInstanceIdentifier:cardTemplateIdentifier:preview:");
				break;
			case CardType.Configuration:
				InitializeHandle (_InitWithCardConfiguration (credentialIdentifier, sharingInstanceIdentifier, templateIdentifier, preview),
					"initWithProvisioningCredentialIdentifier:sharingInstanceIdentifier:cardConfigurationIdentifier:preview:");
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (cardType));
			}
		}
	}
#endif // !TVOS
}
