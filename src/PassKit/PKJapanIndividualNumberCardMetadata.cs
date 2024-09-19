#nullable enable

#if !WATCH

using System;
using Foundation;
using ObjCRuntime;

namespace PassKit {

	/// <summary>This enum is used to select how to initialize a new instance of a <see cref="PKJapanIndividualNumberCardMetadata" />.</summary>
	public enum PKJapanIndividualNumberCardMetadataConstructorOption {
		/// <summary>The <c>cardIdentifier</c> parameter passed to the constructor is an card template identifier.</summary>
		CardTemplateIdentifier,
		/// <summary>The <c>cardIdentifier</c> parameter passed to the constructor is an card configuration identifier.</summary>
		CardConfigurationIdentifier,
	}

	public partial class PKJapanIndividualNumberCardMetadata {
		public PKJapanIndividualNumberCardMetadata (string credentialIdentifier, string sharingInstanceIdentifier, string cardIdentifier, PKAddPassMetadataPreview preview, PKJapanIndividualNumberCardMetadataConstructorOption option)
			: base (NSObjectFlag.Empty)
		{
			switch (option) {
			case PKJapanIndividualNumberCardMetadataConstructorOption.CardTemplateIdentifier:
				InitializeHandle (_InitWithProvisioningCredentialIdentifier_CardTemplateIdentifier (credentialIdentifier, sharingInstanceIdentifier, cardIdentifier, preview)); ;
				break;
			case PKJapanIndividualNumberCardMetadataConstructorOption.CardConfigurationIdentifier:
				InitializeHandle (_InitWithProvisioningCredentialIdentifier_CardConfigurationIdentifier (credentialIdentifier, sharingInstanceIdentifier, cardIdentifier, preview));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}
	}
}
#endif // !WATCH
