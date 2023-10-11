//
// security.cs: Definitions for Security
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc.
//

using System;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

namespace Security {

	[Static]
	interface SecPolicyIdentifier {
		// they are CFString -> https://github.com/Apple-FOSS-Mirror/libsecurity_keychain/blob/master/lib/SecPolicy.cpp

		// the Apple* prefix was kept since they are Apple-specific (not an RFC) OIDs

		[Field ("kSecPolicyAppleX509Basic")]
		NSString AppleX509Basic { get; }

		[Field ("kSecPolicyAppleSSL")]
		NSString AppleSSL { get; }

		[Field ("kSecPolicyAppleSMIME")]
		NSString AppleSMIME { get; }

		[Field ("kSecPolicyAppleEAP")]
		NSString AppleEAP { get; }

		[Field ("kSecPolicyAppleIPsec")]
		NSString AppleIPsec { get; }

		[NoiOS]
		[NoWatch]
		[NoTV]
		[NoMacCatalyst]
		[Field ("kSecPolicyApplePKINITClient")]
		NSString ApplePKINITClient { get; }

		[NoiOS]
		[NoWatch]
		[NoTV]
		[NoMacCatalyst]
		[Field ("kSecPolicyApplePKINITServer")]
		NSString ApplePKINITServer { get; }

		[Field ("kSecPolicyAppleCodeSigning")]
		NSString AppleCodeSigning { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecPolicyMacAppStoreReceipt")]
		NSString MacAppStoreReceipt { get; }

		[Field ("kSecPolicyAppleIDValidation")]
		NSString AppleIDValidation { get; }

		[Field ("kSecPolicyAppleTimeStamping")]
		NSString AppleTimeStamping { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecPolicyAppleRevocation")]
		NSString AppleRevocation { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecPolicyApplePassbookSigning")]
		NSString ApplePassbookSigning { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecPolicyApplePayIssuerEncryption")]
		NSString ApplePayIssuerEncryption { get; }
	}

	[Static]
	interface SecPolicyPropertyKey {
		[Field ("kSecPolicyOid")]
		NSString Oid { get; }

		[Field ("kSecPolicyName")]
		NSString Name { get; }

		[Field ("kSecPolicyClient")]
		NSString Client { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecPolicyRevocationFlags")]
		NSString RevocationFlags { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecPolicyTeamIdentifier")]
		NSString TeamIdentifier { get; }
	}

	[Static]
	[NoWatch]
	[Mac (11, 0)]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV] // removed in tvOS 10
	interface SecSharedCredential {
		[Field ("kSecSharedPassword")]
		NSString SharedPassword { get; }
	}


	[Static]
	interface SecTrustPropertyKey {
		[Field ("kSecPropertyTypeTitle")]
		NSString Title { get; }

		[Field ("kSecPropertyTypeError")]
		NSString Error { get; }
	}

	[Static]
	[MacCatalyst (13, 1)]
	interface SecTrustResultKey {
		[Field ("kSecTrustEvaluationDate")]
		NSString EvaluationDate { get; }

		[Field ("kSecTrustExtendedValidation")]
		NSString ExtendedValidation { get; }

		[Field ("kSecTrustOrganizationName")]
		NSString OrganizationName { get; }

		[Field ("kSecTrustResultValue")]
		NSString ResultValue { get; }

		[Field ("kSecTrustRevocationChecked")]
		NSString RevocationChecked { get; }

		[Field ("kSecTrustRevocationValidUntilDate")]
		NSString RevocationValidUntilDate { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecTrustCertificateTransparency")]
		NSString CertificateTransparency { get; }

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.TvOS, 11, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("kSecTrustCertificateTransparencyWhiteList")]
		NSString CertificateTransparencyWhiteList { get; }
	}

	[Static]
	interface SecMatchLimit {
		[Field ("kSecMatchLimitOne")]
		IntPtr MatchLimitOne { get; }

		[Field ("kSecMatchLimitAll")]
		IntPtr MatchLimitAll { get; }
	}

	enum SecKeyType {
		Invalid = -1,

		[Field ("kSecAttrKeyTypeRSA")]
		RSA = 0,

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrKeyTypeEC")]
		EC = 1,

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrKeyTypeECSECPrimeRandom")]
		ECSecPrimeRandom = 2,
	}

	enum SecKeyClass {
		Invalid = -1,

		[Field ("kSecAttrKeyClassPublic")]
		Public = 0,

		[Field ("kSecAttrKeyClassPrivate")]
		Private = 1,

		[Field ("kSecAttrKeyClassSymmetric")]
		Symmetric = 2,
	}

	[Static]
	[Internal]
	interface KeysAuthenticationType {
		[Field ("kSecAttrAuthenticationTypeNTLM")]
		IntPtr NTLM { get; }

		[Field ("kSecAttrAuthenticationTypeMSN")]
		IntPtr MSN { get; }

		[Field ("kSecAttrAuthenticationTypeDPA")]
		IntPtr DPA { get; }

		[Field ("kSecAttrAuthenticationTypeRPA")]
		IntPtr RPA { get; }

		[Field ("kSecAttrAuthenticationTypeHTTPBasic")]
		IntPtr HTTPBasic { get; }

		[Field ("kSecAttrAuthenticationTypeHTTPDigest")]
		IntPtr HTTPDigest { get; }

		[Field ("kSecAttrAuthenticationTypeHTMLForm")]
		IntPtr HTMLForm { get; }

		[Field ("kSecAttrAuthenticationTypeDefault")]
		IntPtr Default { get; }
	}

	[Static]
	[Internal]
	interface SecProtocolKeys {
		[Field ("kSecAttrProtocolFTP")]
		IntPtr FTP { get; }

		[Field ("kSecAttrProtocolFTPAccount")]
		IntPtr FTPAccount { get; }

		[Field ("kSecAttrProtocolHTTP")]
		IntPtr HTTP { get; }

		[Field ("kSecAttrProtocolIRC")]
		IntPtr IRC { get; }

		[Field ("kSecAttrProtocolNNTP")]
		IntPtr NNTP { get; }

		[Field ("kSecAttrProtocolPOP3")]
		IntPtr POP3 { get; }

		[Field ("kSecAttrProtocolSMTP")]
		IntPtr SMTP { get; }

		[Field ("kSecAttrProtocolSOCKS")]
		IntPtr SOCKS { get; }

		[Field ("kSecAttrProtocolIMAP")]
		IntPtr IMAP { get; }

		[Field ("kSecAttrProtocolLDAP")]
		IntPtr LDAP { get; }

		[Field ("kSecAttrProtocolAppleTalk")]
		IntPtr AppleTalk { get; }

		[Field ("kSecAttrProtocolAFP")]
		IntPtr AFP { get; }

		[Field ("kSecAttrProtocolTelnet")]
		IntPtr Telnet { get; }

		[Field ("kSecAttrProtocolSSH")]
		IntPtr SSH { get; }

		[Field ("kSecAttrProtocolFTPS")]
		IntPtr FTPS { get; }

		[Field ("kSecAttrProtocolHTTPS")]
		IntPtr HTTPS { get; }

		[Field ("kSecAttrProtocolHTTPProxy")]
		IntPtr HTTPProxy { get; }

		[Field ("kSecAttrProtocolHTTPSProxy")]
		IntPtr HTTPSProxy { get; }

		[Field ("kSecAttrProtocolFTPProxy")]
		IntPtr FTPProxy { get; }

		[Field ("kSecAttrProtocolSMB")]
		IntPtr SMB { get; }

		[Field ("kSecAttrProtocolRTSP")]
		IntPtr RTSP { get; }

		[Field ("kSecAttrProtocolRTSPProxy")]
		IntPtr RTSPProxy { get; }

		[Field ("kSecAttrProtocolDAAP")]
		IntPtr DAAP { get; }

		[Field ("kSecAttrProtocolEPPC")]
		IntPtr EPPC { get; }

		[Field ("kSecAttrProtocolIPP")]
		IntPtr IPP { get; }

		[Field ("kSecAttrProtocolNNTPS")]
		IntPtr NNTPS { get; }

		[Field ("kSecAttrProtocolLDAPS")]
		IntPtr LDAPS { get; }

		[Field ("kSecAttrProtocolTelnetS")]
		IntPtr TelnetS { get; }

		[Field ("kSecAttrProtocolIMAPS")]
		IntPtr IMAPS { get; }

		[Field ("kSecAttrProtocolIRCS")]
		IntPtr IRCS { get; }

		[Field ("kSecAttrProtocolPOP3S")]
		IntPtr POP3S { get; }
	}

	[Static]
	[Internal]
	interface KeysAccessible {
		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessibleWhenUnlocked")]
		IntPtr WhenUnlocked { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessibleAfterFirstUnlock")]
		IntPtr AfterFirstUnlock { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessibleAlways")]
		IntPtr Always { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessibleWhenUnlockedThisDeviceOnly")]
		IntPtr WhenUnlockedThisDeviceOnly { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly")]
		IntPtr AfterFirstUnlockThisDeviceOnly { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessibleAlwaysThisDeviceOnly")]
		IntPtr AlwaysThisDeviceOnly { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessibleWhenPasscodeSetThisDeviceOnly")]
		IntPtr WhenPasscodeSetThisDeviceOnly { get; }
	}

	[StrongDictionary ("SecAttributeKeys")]
	interface SecPublicPrivateKeyAttrs {
		string Label { get; set; }

		bool IsPermanent { get; set; }

		NSData ApplicationTag { get; set; }

		int EffectiveKeySize { get; set; }

#if MONOMAC
		[Advice ("On macOS when passed to 'GenerateKeyPair', 'false' seems to be the only valid value. Otherwise 'UnsupportedKeyUsageMask' is returned.")]
#endif
		bool CanEncrypt { get; set; }

#if MONOMAC
		[Advice ("On macOS when passed to 'GenerateKeyPair', 'false' seems to be the only valid value. Otherwise 'UnsupportedKeyUsageMask' is returned.")]
#endif
		bool CanDecrypt { get; set; }

		bool CanDerive { get; set; }

#if MONOMAC
		[Advice ("On macOS when passed to 'GenerateKeyPair', 'false' seems to be the only valid value. Otherwise 'UnsupportedKeyUsageMask' is returned.")]
#endif
		bool CanSign { get; set; }

#if MONOMAC
		[Advice ("On macOS when passed to 'GenerateKeyPair', 'false' seems to be the only valid value. Otherwise 'UnsupportedKeyUsageMask' is returned.")]
#endif
		bool CanVerify { get; set; }

#if MONOMAC
		[Advice ("On macOS when passed to 'GenerateKeyPair', 'false' seems to be the only valid value. Otherwise 'UnsupportedKeyUsageMask' is returned.")]
#endif
		bool CanUnwrap { get; set; }
	}

	[Static]
	[Internal]
	interface SecAttributeKeys {
		[Field ("kSecAttrLabel")]
		NSString LabelKey { get; }

		[Field ("kSecAttrIsPermanent")]
		NSString IsPermanentKey { get; }

		[Field ("kSecAttrApplicationTag")]
		NSString ApplicationTagKey { get; }

		[Field ("kSecAttrEffectiveKeySize")]
		NSString EffectiveKeySizeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessControl")]
		NSString AccessControlKey { get; }

		[Field ("kSecAttrCanEncrypt")]
		NSString CanEncryptKey { get; }

		[Field ("kSecAttrCanDecrypt")]
		NSString CanDecryptKey { get; }

		[Field ("kSecAttrCanDerive")]
		NSString CanDeriveKey { get; }

		[Field ("kSecAttrCanSign")]
		NSString CanSignKey { get; }

		[Field ("kSecAttrCanVerify")]
		NSString CanVerifyKey { get; }

		[Field ("kSecAttrCanUnwrap")]
		NSString CanUnwrapKey { get; }
	}

	[Static]
	[Internal]
	interface SecKeyGenerationAttributeKeys : SecAttributeKeys {
		[Field ("kSecAttrKeyType")]
		NSString KeyTypeKey { get; }

		[Field ("kSecAttrKeySizeInBits")]
		NSString KeySizeInBitsKey { get; }

		[Field ("kSecPrivateKeyAttrs")]
		NSString PrivateKeyAttrsKey { get; }

		[Field ("kSecPublicKeyAttrs")]
		NSString PublicKeyAttrsKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrTokenID")]
		NSString TokenIDKey { get; }

		[Field ("kSecAttrCanWrap")]
		NSString CanWrapKey { get; }
	}

	[StrongDictionary ("SecAttributeKeys")]
	interface SecKeyParameters {
		string Label { get; set; }

		bool IsPermanent { get; set; }

		NSData ApplicationTag { get; set; }

		int EffectiveKeySize { get; set; }

		bool CanEncrypt { get; set; }

		bool CanDecrypt { get; set; }

		bool CanDerive { get; set; }

		bool CanSign { get; set; }

		bool CanVerify { get; set; }

		bool CanUnwrap { get; set; }
	}

	[StrongDictionary ("SecKeyGenerationAttributeKeys")]
	interface SecKeyGenerationParameters {
		int KeySizeInBits { get; set; }

		[StrongDictionary]
		[Export ("PrivateKeyAttrsKey")]
		SecKeyParameters PrivateKeyAttrs { get; set; }

		[StrongDictionary]
		[Export ("PublicKeyAttrsKey")]
		SecKeyParameters PublicKeyAttrs { get; set; }

		string Label { get; set; }

		bool IsPermanent { get; set; }

		NSData ApplicationTag { get; set; }

		int EffectiveKeySize { get; set; }

		bool CanEncrypt { get; set; }

		bool CanDecrypt { get; set; }

		bool CanDerive { get; set; }

		bool CanSign { get; set; }

		bool CanVerify { get; set; }

		bool CanWrap { get; set; }

		bool CanUnwrap { get; set; }
	}

	[Static]
	[Internal]
	interface SecAttributeKey {
		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessible")]
		IntPtr Accessible { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrSynchronizableAny")]
		IntPtr SynchronizableAny { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrSynchronizable")]
		IntPtr Synchronizable { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrSyncViewHint")]
		IntPtr SyncViewHint { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessGroup")]
		IntPtr AccessGroup { get; }

		[Field ("kSecAttrCreationDate")]
		IntPtr CreationDate { get; }

		[Field ("kSecAttrModificationDate")]
		IntPtr ModificationDate { get; }

		[Field ("kSecAttrDescription")]
		IntPtr Description { get; }

		[Field ("kSecAttrComment")]
		IntPtr Comment { get; }

		[Field ("kSecAttrCreator")]
		IntPtr Creator { get; }

		[Field ("kSecAttrType")]
		IntPtr Type { get; }

		[Field ("kSecAttrIsInvisible")]
		IntPtr IsInvisible { get; }

		[Field ("kSecAttrIsNegative")]
		IntPtr IsNegative { get; }

		[Field ("kSecAttrAccount")]
		IntPtr Account { get; }

		[Field ("kSecAttrService")]
		IntPtr Service { get; }

		[Field ("kSecAttrGeneric")]
		IntPtr Generic { get; }

		[Field ("kSecAttrSecurityDomain")]
		IntPtr SecurityDomain { get; }

		[Field ("kSecAttrServer")]
		IntPtr Server { get; }

		[Field ("kSecAttrProtocol")]
		IntPtr Protocol { get; }

		[Field ("kSecAttrAuthenticationType")]
		IntPtr AuthenticationType { get; }

		[Field ("kSecAttrPort")]
		IntPtr Port { get; }

		[Field ("kSecAttrPath")]
		IntPtr Path { get; }

		[Field ("kSecAttrSubject")]
		IntPtr Subject { get; }

		[Field ("kSecAttrIssuer")]
		IntPtr Issuer { get; }

		[Field ("kSecAttrSerialNumber")]
		IntPtr SerialNumber { get; }

		[Field ("kSecAttrSubjectKeyID")]
		IntPtr SubjectKeyID { get; }

		[Field ("kSecAttrPublicKeyHash")]
		IntPtr PublicKeyHash { get; }

		[Field ("kSecAttrCertificateType")]
		IntPtr CertificateType { get; }

		[Field ("kSecAttrCertificateEncoding")]
		IntPtr CertificateEncoding { get; }

		[Field ("kSecAttrKeyClass")]
		IntPtr KeyClass { get; }

		[Field ("kSecAttrApplicationLabel")]
		IntPtr ApplicationLabel { get; }

		[Field ("kSecAttrIsSensitive")]
		IntPtr IsSensitive { get; }

		[Field ("kSecAttrIsExtractable")]
		IntPtr IsExtractable { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrTokenIDSecureEnclave")]
		IntPtr SecureEnclave { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecAttrAccessGroupToken")]
		IntPtr AccessGroupToken { get; }

		// note: 'kSecAttrPersistantReference' with the word "persistent" (correct) written with an 'a', so "persistant" (incorrect) was a typo in Xcode 9 beta 1
		[MacCatalyst (13, 1)]
		[Field ("kSecAttrPersistentReference")]
		IntPtr PersistentReference { get; }
	}

	[Static]
	[Internal]
	interface SecClass {
		[Field ("kSecClass")]
		IntPtr SecClassKey { get; }

		[Field ("kSecClassGenericPassword")]
		IntPtr GenericPassword { get; }

		[Field ("kSecClassInternetPassword")]
		IntPtr InternetPassword { get; }

		[Field ("kSecClassCertificate")]
		IntPtr Certificate { get; }

		[Field ("kSecClassKey")]
		IntPtr Key { get; }

		[Field ("kSecClassIdentity")]
		IntPtr Identity { get; }
	}

	// Technically the type could be static but Apple might had non static members in future releases and break out API
	[DisableDefaultCtor] // not required, nor useful
	[Partial]
	interface SecImportExport {
		[Field ("kSecImportExportPassphrase")]
		NSString Passphrase { get; }

		[Field ("kSecImportItemLabel")]
		NSString Label { get; }

		[Field ("kSecImportItemKeyID")]
		NSString KeyId { get; }

		[Field ("kSecImportItemTrust")]
		NSString Trust { get; }

		[Field ("kSecImportItemCertChain")]
		NSString CertChain { get; }

		[Field ("kSecImportItemIdentity")]
		NSString Identity { get; }
	}

	[Static]
	[Internal]
	interface SecItem {
		[Field ("kSecMatchPolicy")]
		IntPtr MatchPolicy { get; }

		[Field ("kSecMatchItemList")]
		IntPtr MatchItemList { get; }

		[Field ("kSecMatchSearchList")]
		IntPtr MatchSearchList { get; }

		[Field ("kSecMatchIssuers")]
		IntPtr MatchIssuers { get; }

		[Field ("kSecMatchEmailAddressIfPresent")]
		IntPtr MatchEmailAddressIfPresent { get; }

		[Field ("kSecMatchSubjectContains")]
		IntPtr MatchSubjectContains { get; }

		[Field ("kSecMatchCaseInsensitive")]
		IntPtr MatchCaseInsensitive { get; }

		[Field ("kSecMatchTrustedOnly")]
		IntPtr MatchTrustedOnly { get; }

		[Field ("kSecMatchValidOnDate")]
		IntPtr MatchValidOnDate { get; }

		[Field ("kSecMatchLimit")]
		IntPtr MatchLimit { get; }

		[Field ("kSecReturnData")]
		IntPtr ReturnData { get; }

		[Field ("kSecReturnAttributes")]
		IntPtr ReturnAttributes { get; }

		[Field ("kSecReturnRef")]
		IntPtr ReturnRef { get; }

		[Field ("kSecReturnPersistentRef")]
		IntPtr ReturnPersistentRef { get; }

		[Field ("kSecValueData")]
		IntPtr ValueData { get; }

		[Field ("kSecValueRef")]
		IntPtr ValueRef { get; }

		[Field ("kSecValuePersistentRef")]
		IntPtr ValuePersistentRef { get; }

		[Field ("kSecUseItemList")]
		IntPtr UseItemList { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kSecUseDataProtectionKeychain")]
		IntPtr UseDataProtectionKeychain { get; }

		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LAContext.InteractionNotAllowed' instead.")]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LAContext.InteractionNotAllowed' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LAContext.InteractionNotAllowed' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LAContext.InteractionNotAllowed' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LAContext.InteractionNotAllowed' instead.")]
		[Field ("kSecUseOperationPrompt")]
		IntPtr UseOperationPrompt { get; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("kSecUseNoAuthenticationUI")]
		IntPtr UseNoAuthenticationUI { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecUseAuthenticationUI")]
		IntPtr UseAuthenticationUI { get; }

		[MacCatalyst (13, 1)]
		[Field ("kSecUseAuthenticationContext")]
		IntPtr UseAuthenticationContext { get; }

		[TV (17, 0), NoWatch, NoMacCatalyst, NoMac, NoiOS]
		[Field ("kSecUseUserIndependentKeychain")]
		IntPtr UseUserIndependentKeychain { get; }
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[Static]
	[Internal]
	interface SecCertificateOIDs {
		[Field ("kSecOIDX509V1SubjectPublicKey")]
		IntPtr SubjectPublicKey { get; }
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[Static]
	[Internal]
	interface SecPropertyKey {
		[Field ("kSecPropertyKeyType")]
		IntPtr Type { get; }

		[Field ("kSecPropertyKeyLabel")]
		IntPtr Label { get; }

		[Field ("kSecPropertyKeyLocalizedLabel")]
		IntPtr LocalizedLabel { get; }

		[Field ("kSecPropertyKeyValue")]
		IntPtr Value { get; }
	}

	[MacCatalyst (13, 1)]
	enum SecKeyAlgorithm {
		[Field ("kSecKeyAlgorithmRSASignatureRaw")]
		RsaSignatureRaw,

		[Field ("kSecKeyAlgorithmRSASignatureDigestPKCS1v15Raw")]
		RsaSignatureDigestPkcs1v15Raw,

		[Field ("kSecKeyAlgorithmRSASignatureDigestPKCS1v15SHA1")]
		RsaSignatureDigestPkcs1v15Sha1,

		[Field ("kSecKeyAlgorithmRSASignatureDigestPKCS1v15SHA224")]
		RsaSignatureDigestPkcs1v15Sha224,

		[Field ("kSecKeyAlgorithmRSASignatureDigestPKCS1v15SHA256")]
		RsaSignatureDigestPkcs1v15Sha256,

		[Field ("kSecKeyAlgorithmRSASignatureDigestPKCS1v15SHA384")]
		RsaSignatureDigestPkcs1v15Sha384,

		[Field ("kSecKeyAlgorithmRSASignatureDigestPKCS1v15SHA512")]
		RsaSignatureDigestPkcs1v15Sha512,

		[Field ("kSecKeyAlgorithmRSASignatureMessagePKCS1v15SHA1")]
		RsaSignatureMessagePkcs1v15Sha1,

		[Field ("kSecKeyAlgorithmRSASignatureMessagePKCS1v15SHA224")]
		RsaSignatureMessagePkcs1v15Sha224,

		[Field ("kSecKeyAlgorithmRSASignatureMessagePKCS1v15SHA256")]
		RsaSignatureMessagePkcs1v15Sha256,

		[Field ("kSecKeyAlgorithmRSASignatureMessagePKCS1v15SHA384")]
		RsaSignatureMessagePkcs1v15Sha384,

		[Field ("kSecKeyAlgorithmRSASignatureMessagePKCS1v15SHA512")]
		RsaSignatureMessagePkcs1v15Sha512,

		[Field ("kSecKeyAlgorithmECDSASignatureRFC4754")]
		EcdsaSignatureRfc4754,

		[Field ("kSecKeyAlgorithmECDSASignatureDigestX962")]
		EcdsaSignatureDigestX962,

		[Field ("kSecKeyAlgorithmECDSASignatureDigestX962SHA1")]
		EcdsaSignatureDigestX962Sha1,

		[Field ("kSecKeyAlgorithmECDSASignatureDigestX962SHA224")]
		EcdsaSignatureDigestX962Sha224,

		[Field ("kSecKeyAlgorithmECDSASignatureDigestX962SHA256")]
		EcdsaSignatureDigestX962Sha256,

		[Field ("kSecKeyAlgorithmECDSASignatureDigestX962SHA384")]
		EcdsaSignatureDigestX962Sha384,

		[Field ("kSecKeyAlgorithmECDSASignatureDigestX962SHA512")]
		EcdsaSignatureDigestX962Sha512,

		[Field ("kSecKeyAlgorithmECDSASignatureMessageX962SHA1")]
		EcdsaSignatureMessageX962Sha1,

		[Field ("kSecKeyAlgorithmECDSASignatureMessageX962SHA224")]
		EcdsaSignatureMessageX962Sha224,

		[Field ("kSecKeyAlgorithmECDSASignatureMessageX962SHA256")]
		EcdsaSignatureMessageX962Sha256,

		[Field ("kSecKeyAlgorithmECDSASignatureMessageX962SHA384")]
		EcdsaSignatureMessageX962Sha384,

		[Field ("kSecKeyAlgorithmECDSASignatureMessageX962SHA512")]
		EcdsaSignatureMessageX962Sha512,

		[Field ("kSecKeyAlgorithmRSAEncryptionRaw")]
		RsaEncryptionRaw,

		[Field ("kSecKeyAlgorithmRSAEncryptionPKCS1")]
		RsaEncryptionPkcs1,

		[Field ("kSecKeyAlgorithmRSAEncryptionOAEPSHA1")]
		RsaEncryptionOaepSha1,

		[Field ("kSecKeyAlgorithmRSAEncryptionOAEPSHA224")]
		RsaEncryptionOaepSha224,

		[Field ("kSecKeyAlgorithmRSAEncryptionOAEPSHA256")]
		RsaEncryptionOaepSha256,

		[Field ("kSecKeyAlgorithmRSAEncryptionOAEPSHA384")]
		RsaEncryptionOaepSha384,

		[Field ("kSecKeyAlgorithmRSAEncryptionOAEPSHA512")]
		RsaEncryptionOaepSha512,

		[Field ("kSecKeyAlgorithmRSAEncryptionOAEPSHA1AESGCM")]
		RsaEncryptionOaepSha1AesCgm,

		[Field ("kSecKeyAlgorithmRSAEncryptionOAEPSHA224AESGCM")]
		RsaEncryptionOaepSha224AesGcm,

		[Field ("kSecKeyAlgorithmRSAEncryptionOAEPSHA256AESGCM")]
		RsaEncryptionOaepSha256AesGcm,

		[Field ("kSecKeyAlgorithmRSAEncryptionOAEPSHA384AESGCM")]
		RsaEncryptionOaepSha384AesGcm,

		[Field ("kSecKeyAlgorithmRSAEncryptionOAEPSHA512AESGCM")]
		RsaEncryptionOaepSha512AesGcm,

		[Field ("kSecKeyAlgorithmECIESEncryptionStandardX963SHA1AESGCM")]
		EciesEncryptionStandardX963Sha1AesGcm,

		[Field ("kSecKeyAlgorithmECIESEncryptionStandardX963SHA224AESGCM")]
		EciesEncryptionStandardX963Sha224AesGcm,

		[Field ("kSecKeyAlgorithmECIESEncryptionStandardX963SHA256AESGCM")]
		EciesEncryptionStandardX963Sha256AesGcm,

		[Field ("kSecKeyAlgorithmECIESEncryptionStandardX963SHA384AESGCM")]
		EciesEncryptionStandardX963Sha384AesGcm,

		[Field ("kSecKeyAlgorithmECIESEncryptionStandardX963SHA512AESGCM")]
		EciesEncryptionStandardX963Sha512AesGcm,

		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorX963SHA1AESGCM")]
		EciesEncryptionCofactorX963Sha1AesGcm,

		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorX963SHA224AESGCM")]
		EciesEncryptionCofactorX963Sha224AesGcm,

		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorX963SHA256AESGCM")]
		EciesEncryptionCofactorX963Sha256AesGcm,

		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorX963SHA384AESGCM")]
		EciesEncryptionCofactorX963Sha384AesGcm,

		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorX963SHA512AESGCM")]
		EciesEncryptionCofactorX963Sha512AesGcm,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeStandard")]
		EcdhKeyExchangeStandard,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeStandardX963SHA1")]
		EcdhKeyExchangeStandardX963Sha1,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeStandardX963SHA224")]
		EcdhKeyExchangeStandardX963Sha224,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeStandardX963SHA256")]
		EcdhKeyExchangeStandardX963Sha256,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeStandardX963SHA384")]
		EcdhKeyExchangeStandardX963Sha384,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeStandardX963SHA512")]
		EcdhKeyExchangeStandardX963Sha512,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeCofactor")]
		EcdhKeyExchangeCofactor,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeCofactorX963SHA1")]
		EcdhKeyExchangeCofactorX963Sha1,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeCofactorX963SHA224")]
		EcdhKeyExchangeCofactorX963Sha224,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeCofactorX963SHA256")]
		EcdhKeyExchangeCofactorX963Sha256,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeCofactorX963SHA384")]
		EcdhKeyExchangeCofactorX963Sha384,

		[Field ("kSecKeyAlgorithmECDHKeyExchangeCofactorX963SHA512")]
		EcdhKeyExchangeCofactorX963Sha512,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmRSASignatureDigestPSSSHA1")]
		RsaSignatureDigestPssSha1,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmRSASignatureDigestPSSSHA224")]
		RsaSignatureDigestPssSha224,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmRSASignatureDigestPSSSHA256")]
		RsaSignatureDigestPssSha256,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmRSASignatureDigestPSSSHA384")]
		RsaSignatureDigestPssSha384,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmRSASignatureDigestPSSSHA512")]
		RsaSignatureDigestPssSha512,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmRSASignatureMessagePSSSHA1")]
		RsaSignatureMessagePssSha1,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmRSASignatureMessagePSSSHA224")]
		RsaSignatureMessagePssSha224,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmRSASignatureMessagePSSSHA256")]
		RsaSignatureMessagePssSha256,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmRSASignatureMessagePSSSHA384")]
		RsaSignatureMessagePssSha384,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmRSASignatureMessagePSSSHA512")]
		RsaSignatureMessagePssSha512,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmECIESEncryptionStandardVariableIVX963SHA224AESGCM")]
		EciesEncryptionStandardVariableIvx963Sha224AesGcm,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmECIESEncryptionStandardVariableIVX963SHA256AESGCM")]
		EciesEncryptionStandardVariableIvx963Sha256AesGcm,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmECIESEncryptionStandardVariableIVX963SHA384AESGCM")]
		EciesEncryptionStandardVariableIvx963Sha384AesGcm,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmECIESEncryptionStandardVariableIVX963SHA512AESGCM")]
		EciesEncryptionStandardVariableIvx963Sha512AesGcm,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorVariableIVX963SHA224AESGCM")]
		EciesEncryptionCofactorVariableIvx963Sha224AesGcm,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorVariableIVX963SHA256AESGCM")]
		EciesEncryptionCofactorVariableIvx963Sha256AesGcm,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorVariableIVX963SHA384AESGCM")]
		EciesEncryptionCofactorVariableIvx963Sha384AesGcm,

		[MacCatalyst (13, 1)]
		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorVariableIVX963SHA512AESGCM")]
		EciesEncryptionCofactorVariableIvx963Sha512AesGcm,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureDigestRFC4754")]
		EcdsaSignatureDigestRfc4754,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureDigestRFC4754SHA1")]
		EcdsaSignatureDigestRfc4754Sha1,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureDigestRFC4754SHA224")]
		EcdsaSignatureDigestRfc4754Sha224,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureDigestRFC4754SHA256")]
		EcdsaSignatureDigestRfc4754Sha256,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureDigestRFC4754SHA384")]
		EcdsaSignatureDigestRfc4754Sha384,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureDigestRFC4754SHA512")]
		EcdsaSignatureDigestRfc4754Sha512,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureMessageRFC4754SHA1")]
		EcdsaSignatureMessageRfc4754Sha1,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureMessageRFC4754SHA224")]
		EcdsaSignatureMessageRfc4754Sha224,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureMessageRFC4754SHA256")]
		EcdsaSignatureMessageRfc4754Sha256,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureMessageRFC4754SHA384")]
		EcdsaSignatureMessageRfc4754Sha384,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("kSecKeyAlgorithmECDSASignatureMessageRFC4754SHA512")]
		EcdsaSignatureMessageRfc4754Sha512,
	}

	[MacCatalyst (13, 1)]
	enum SslSessionConfig {
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.TvOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("kSSLSessionConfig_default")]
		Default,

		[Field ("kSSLSessionConfig_ATSv1")]
		Ats1,

		[Field ("kSSLSessionConfig_ATSv1_noPFS")]
		Ats1NoPfs,

		[Field ("kSSLSessionConfig_standard")]
		Standard,

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.TvOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("kSSLSessionConfig_RC4_fallback")]
		RC4Fallback,

		[Field ("kSSLSessionConfig_TLSv1_fallback")]
		Tls1Fallback,

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.TvOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("kSSLSessionConfig_TLSv1_RC4_fallback")]
		Tls1RC4Fallback,

		[Field ("kSSLSessionConfig_legacy")]
		Legacy,

		[Field ("kSSLSessionConfig_legacy_DHE")]
		LegacyDhe,

		[Field ("kSSLSessionConfig_anonymous")]
		Anonymous,

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.TvOS, 11, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("kSSLSessionConfig_3DES_fallback")]
		ThreeDesFallback,

		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.WatchOS, 4, 0)]
		[Deprecated (PlatformName.TvOS, 11, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("kSSLSessionConfig_TLSv1_3DES_fallback")]
		Tls1ThreeDesFallback,
	}

	[MacCatalyst (13, 1)]
	[Internal]
	[Static]
	interface SecKeyKeyExchangeParameterKey {
		[Field ("kSecKeyKeyExchangeParameterRequestedSize")]
		NSString RequestedSizeKey { get; }

		[Field ("kSecKeyKeyExchangeParameterSharedInfo")]
		NSString SharedInfoKey { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("SecKeyKeyExchangeParameterKey")]
	interface SecKeyKeyExchangeParameter {

		int RequestedSize { get; set; }

		NSData SharedInfo { get; set; }
	}

	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Internal]
	[Static]
	interface SecSharedCredentialKeys {
		[Field ("kSecAttrServer")]
		NSString ServerKey { get; }

		[Field ("kSecAttrAccount")]
		NSString AccountKey { get; }

		[MacCatalyst (14, 0)]
		[Field ("kSecSharedPassword")]
		NSString PasswordKey { get; }

		[Field ("kSecAttrPort")]
		NSString PortKey { get; }
	}

	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("SecSharedCredentialKeys")]
	interface SecSharedCredentialInfo {

		string Server { get; set; }

		string Account { get; set; }

		string Password { get; set; }

		int Port { get; set; }
	}

	delegate void SecProtocolVerifyComplete (bool complete);

	// Respond with the identity to use for this challenge.
	delegate void SecProtocolChallengeComplete (SecIdentity2 identity);

	//
	// These are fake NSObject types, used purely for the generator to do all the heavy lifting with block generation
	//
	delegate void SecProtocolKeyUpdate (SecProtocolMetadata metadata, [BlockCallback] Action complete);
	delegate void SecProtocolChallenge (SecProtocolMetadata metadata, [BlockCallback] SecProtocolChallengeComplete challengeComplete);
	delegate void SecProtocolVerify (SecProtocolMetadata metadata, SecTrust2 trust, [BlockCallback] SecProtocolVerifyComplete verifyComplete);

	[Internal]
	[Partial]
	interface Callbacks {
		[Export ("options:keyUpdateBlock:keyUpdateQueue:")]
		[NoMethod]
		void SetKeyUpdateBlock (SecProtocolOptions options, [BlockCallback] SecProtocolKeyUpdate keyUpdateBlock, DispatchQueue keyUpdateQueue);

		[Export ("options:challengeBlock:challengeQueue:")]
		[NoMethod]
		void SetChallengeBlock (SecProtocolOptions options, [BlockCallback] SecProtocolChallenge challengeBlock, DispatchQueue challengeQueue);

		[Export ("options:protocolVerify:verifyQueue:")]
		[NoMethod]
		void SetVerifyBlock (SecProtocolOptions options, [BlockCallback] SecProtocolVerify verifyBlock, DispatchQueue verifyQueue);
	}
}
