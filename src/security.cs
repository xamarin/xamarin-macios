//
// security.cs: Definitions for Security
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Security {

	[Static]
	[iOS (7,0)]
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
#if MONOMAC
		[Field ("kSecPolicyApplePKINITClient")]
		NSString ApplePKINITClient { get; }
			
		[Field ("kSecPolicyApplePKINITServer")]
		NSString ApplePKINITServer { get; }
#endif	
		[Field ("kSecPolicyAppleCodeSigning")]
		NSString AppleCodeSigning { get; }

		[iOS (9,0)][Mac (10,7)]
		[Field ("kSecPolicyMacAppStoreReceipt")]
		NSString MacAppStoreReceipt { get; }

		[Field ("kSecPolicyAppleIDValidation")]
		NSString AppleIDValidation { get; }
			
		[Mac (10, 8)]
		[Field ("kSecPolicyAppleTimeStamping")]
		NSString AppleTimeStamping { get; }

		[Mac (10, 9)]
		[Field ("kSecPolicyAppleRevocation")]
		NSString AppleRevocation { get; }

		[iOS (7,0)][Mac (10,9)]
		[Field ("kSecPolicyApplePassbookSigning")]
		NSString ApplePassbookSigning { get; }

		[Mac(10,11), iOS (9,0)]
		[Field ("kSecPolicyApplePayIssuerEncryption")]
		NSString ApplePayIssuerEncryption { get; }
	}

	[Static]
	[iOS (7,0)]
	interface SecPolicyPropertyKey {
		[Field ("kSecPolicyOid")]
		NSString Oid { get; }

		[Field ("kSecPolicyName")]
		NSString Name { get; }

		[Field ("kSecPolicyClient")]
		NSString Client { get; }

		[Mac (10, 9)]
		[Field ("kSecPolicyRevocationFlags")]
		NSString RevocationFlags { get; }

		[iOS (7,0)][Mac (10,9)]
		[Field ("kSecPolicyTeamIdentifier")]
		NSString TeamIdentifier { get; }
	}
	
	[Static]
	[iOS (8,0), NoMac, NoWatch]
	[NoTV] // removed in tvOS 10
	interface SecSharedCredential {
		[Field ("kSecSharedPassword")]
		NSString SharedPassword { get; }
	}
	

	[Static]
	[iOS (7,0)]
	interface SecTrustPropertyKey {
		[Field ("kSecPropertyTypeTitle")]
		NSString Title { get; }

		[Field ("kSecPropertyTypeError")]
		NSString Error { get; }
	}

	[Static]
	[iOS (7,0), Mac (10, 9)]
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

		[iOS (9,0)]
		[Mac (10,12)] // headers says 10.11 but it's not present in 10.11
		[Field ("kSecTrustCertificateTransparency")]
		NSString CertificateTransparency { get; }

		[iOS (10,0)][Deprecated (PlatformName.iOS, 11,0)]
		[Mac (10,12)][Deprecated (PlatformName.MacOSX, 10,13)]
		[Watch (3,0)][Deprecated (PlatformName.WatchOS, 4,0)]
		[TV (10,0)][Deprecated (PlatformName.TvOS, 11,0)]
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

		[Mac (10,9)]
		[Field ("kSecAttrKeyTypeEC")]
		EC = 1,

		[iOS (10,0)]
		[Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
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

	[Static][Internal]
	interface SecAuthenticationTypeKey {
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

	[Static][Internal]
	interface SecProtocolKey {
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

	[Static][Internal]
	interface SecAccessibleKey {
		[Mac (10,9)]
		[Field ("kSecAttrAccessibleWhenUnlocked")]
		IntPtr WhenUnlocked { get; }

		[Mac (10,9)]
		[Field ("kSecAttrAccessibleAfterFirstUnlock")]
		IntPtr AfterFirstUnlock { get; }

		[Mac (10,9)]
		[Field ("kSecAttrAccessibleAlways")]
		IntPtr Always { get; }

		[Mac (10,9)]
		[Field ("kSecAttrAccessibleWhenUnlockedThisDeviceOnly")]
		IntPtr WhenUnlockedThisDeviceOnly { get; }

		[Mac (10,9)]
		[Field ("kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly")]
		IntPtr AfterFirstUnlockThisDeviceOnly { get; }

		[Mac (10,9)]
		[Field ("kSecAttrAccessibleAlwaysThisDeviceOnly")]
		IntPtr AlwaysThisDeviceOnly { get; }

		[iOS (8,0)]
		[Mac (10,10)]
		[Field ("kSecAttrAccessibleWhenPasscodeSetThisDeviceOnly")]
		IntPtr WhenPasscodeSetThisDeviceOnly { get; }
	}

	[Static]
	interface SecAttributeKey {
		[iOS (4,0), Mac (10,9), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrAccessible")]
		IntPtr Accessible { get; }

		[iOS (8,0), Mac (10,10), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrAccessControl")]
		IntPtr AccessControl { get; }

		[iOS (7,0), Mac (10,9), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrSynchronizableAny")]
		IntPtr SynchronizableAny { get; }

		[iOS (7,0), Mac (10,9), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrSynchronizable")]
		IntPtr Synchronizable { get; }

		[iOS (9,0), Mac (10, 12, 4), TV (9,0), Watch(2,0)]
		[Field ("kSecAttrSyncViewHint")]
		IntPtr SyncViewHint { get; }

		[iOS (3,0), Mac (10,9), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrAccessGroup")]
		IntPtr AccessGroup { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCreationDate")]
		IntPtr CreationDate { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrModificationDate")]
		IntPtr ModificationDate { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrDescription")]
		IntPtr Description { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrComment")]
		IntPtr Comment { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCreator")]
		IntPtr Creator { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrType")]
		IntPtr Type { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrLabel")]
		IntPtr Label { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrIsInvisible")]
		IntPtr IsInvisible { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrIsNegative")]
		IntPtr IsNegative { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrAccount")]
		IntPtr Account { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrService")]
		IntPtr Service { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrGeneric")]
		IntPtr Generic { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrSecurityDomain")]
		IntPtr SecurityDomain { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrServer")]
		IntPtr Server { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrProtocol")]
		IntPtr Protocol { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrAuthenticationType")]
		IntPtr AuthenticationType { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrPort")]
		IntPtr Port { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrPath")]
		IntPtr Path { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrSubject")]
		IntPtr Subject { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrIssuer")]
		IntPtr Issuer { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrSerialNumber")]
		IntPtr SerialNumber { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrSubjectKeyID")]
		IntPtr SubjectKeyID { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrPublicKeyHash")]
		IntPtr PublicKeyHash { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCertificateType")]
		IntPtr CertificateType { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCertificateEncoding")]
		IntPtr CertificateEncoding { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrKeyClass")]
		IntPtr KeyClass { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrApplicationLabel")]
		IntPtr ApplicationLabel { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrIsPermanent")]
		IntPtr IsPermanent { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrIsSensitive")]
		IntPtr IsSensitive { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrIsExtractable")]
		IntPtr IsExtractable { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrApplicationTag")]
		IntPtr ApplicationTag { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrKeyType")]
		IntPtr KeyType { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrKeySizeInBits")]
		IntPtr KeySizeInBits { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrEffectiveKeySize")]
		IntPtr EffectiveKeySize { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCanEncrypt")]
		IntPtr CanEncrypt { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCanDecrypt")]
		IntPtr CanDecrypt { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCanDerive")]
		IntPtr CanDerive { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCanSign")]
		IntPtr CanSign { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCanVerify")]
		IntPtr CanVerify { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCanWrap")]
		IntPtr CanWrap { get; }

		[iOS (2,0), Mac (10,6), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrCanUnwrap")]
		IntPtr CanUnwrap { get; }

		[iOS (9,0), Mac (10,12), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrTokenID")]
		IntPtr TokenID { get; }

		[iOS (9,0), Mac (10,12,1), TV (9,0), Watch (2,0)]
		[Field ("kSecAttrTokenIDSecureEnclave")]
		IntPtr SecureEnclave { get; }

		[iOS (10,0), Mac (10,12), TV (10,0), Watch (3,0)]
		[Field ("kSecAttrAccessGroupToken")]
		IntPtr AccessGroupToken { get; }

		// note: kSecAttrPersistentReference (beta 1) is a typo that was not removed
		[iOS (11,0), Mac (10,13), TV (11,0), Watch (4,0)]
		[Field ("kSecAttrPersistentReference")]
		IntPtr PersistentReference { get; }

		[iOS (2,0), Mac (10,8), TV (9,0), Watch (2,0)]
		[Field ("kSecPrivateKeyAttrs")]
		IntPtr PrivateKeyAttributes { get; }

		[iOS (2,0), Mac (10,8), TV (9,0), Watch (2,0)]
		[Field ("kSecPublicKeyAttrs")]
		IntPtr PublicKeyAttributes { get; }
	}

	[Static][Internal]
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

#if XAMCORE_2_0
	// Technically the type could be static but Apple might had non static members in future releases and break out API
	[DisableDefaultCtor] // not required, nor useful
#endif
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

	[Static][Internal]
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

#if !MONOMAC || !XAMCORE_2_0 // Don't break compat API
		[iOS (8,0)]
		[Field ("kSecUseOperationPrompt")]
		IntPtr UseOperationPrompt { get; }

		[Availability (Introduced = Platform.iOS_8_0, Deprecated = Platform.iOS_9_0)]
		[Field ("kSecUseNoAuthenticationUI")]
		IntPtr UseNoAuthenticationUI { get; }
#endif

		[iOS (9,0)][Mac (10,11)]
		[Field ("kSecUseAuthenticationUI")]
		IntPtr UseAuthenticationUI { get; }

		[iOS (9,0)][Mac (10,11)]
		[Field ("kSecUseAuthenticationContext")]
		IntPtr UseAuthenticationContext { get; }
	}

	[NoiOS][NoTV][NoWatch]
	[Static][Internal]
	interface SecCertificateOIDs
	{
		[Mac (10,7)]
		[Field ("kSecOIDX509V1SubjectPublicKey")]
		IntPtr SubjectPublicKey { get; }
	}

	[NoiOS][NoTV][NoWatch]
	[Mac (10,7)]
	[Static][Internal]
	interface SecPropertyKey
	{
		[Field ("kSecPropertyKeyType")]
		IntPtr Type { get; }

		[Field ("kSecPropertyKeyLabel")]
		IntPtr Label { get; }

		[Field ("kSecPropertyKeyLocalizedLabel")]
		IntPtr LocalizedLabel { get; }

		[Field ("kSecPropertyKeyValue")]
		IntPtr Value { get; }
	}

	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
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

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmRSASignatureDigestPSSSHA1")]
		RsaSignatureDigestPssSha1,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmRSASignatureDigestPSSSHA224")]
		RsaSignatureDigestPssSha224,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmRSASignatureDigestPSSSHA256")]
		RsaSignatureDigestPssSha256,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmRSASignatureDigestPSSSHA384")]
		RsaSignatureDigestPssSha384,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmRSASignatureDigestPSSSHA512")]
		RsaSignatureDigestPssSha512,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmRSASignatureMessagePSSSHA1")]
		RsaSignatureMessagePssSha1,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmRSASignatureMessagePSSSHA224")]
		RsaSignatureMessagePssSha224,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmRSASignatureMessagePSSSHA256")]
		RsaSignatureMessagePssSha256,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmRSASignatureMessagePSSSHA384")]
		RsaSignatureMessagePssSha384,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmRSASignatureMessagePSSSHA512")]
		RsaSignatureMessagePssSha512,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmECIESEncryptionStandardVariableIVX963SHA224AESGCM")]
		EciesEncryptionStandardVariableIvx963Sha224AesGcm,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmECIESEncryptionStandardVariableIVX963SHA256AESGCM")]
		EciesEncryptionStandardVariableIvx963Sha256AesGcm,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmECIESEncryptionStandardVariableIVX963SHA384AESGCM")]
		EciesEncryptionStandardVariableIvx963Sha384AesGcm,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmECIESEncryptionStandardVariableIVX963SHA512AESGCM")]
		EciesEncryptionStandardVariableIvx963Sha512AesGcm,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorVariableIVX963SHA224AESGCM")]
		EciesEncryptionCofactorVariableIvx963Sha224AesGcm,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorVariableIVX963SHA256AESGCM")]
		EciesEncryptionCofactorVariableIvx963Sha256AesGcm,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorVariableIVX963SHA384AESGCM")]
		EciesEncryptionCofactorVariableIvx963Sha384AesGcm,

		[iOS (11,0)][TV (11,0)][Watch (4,0)][Mac (10,13)]
		[Field ("kSecKeyAlgorithmECIESEncryptionCofactorVariableIVX963SHA512AESGCM")]
		EciesEncryptionCofactorVariableIvx963Sha512AesGcm,
	}

	[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
	enum SslSessionConfig {
		[Deprecated (PlatformName.iOS, 11,0)]
		[Deprecated (PlatformName.MacOSX, 10,13)]
		[Deprecated (PlatformName.WatchOS, 4,0)]
		[Deprecated (PlatformName.TvOS, 11,0)]
		[Field ("kSSLSessionConfig_default")]
		Default,

		[Field ("kSSLSessionConfig_ATSv1")]
		Ats1,

		[Field ("kSSLSessionConfig_ATSv1_noPFS")]
		Ats1NoPfs,

		[Field ("kSSLSessionConfig_standard")]
		Standard,

		[Deprecated (PlatformName.iOS, 11,0)]
		[Deprecated (PlatformName.MacOSX, 10,13)]
		[Deprecated (PlatformName.WatchOS, 4,0)]
		[Deprecated (PlatformName.TvOS, 11,0)]
		[Field ("kSSLSessionConfig_RC4_fallback")]
		RC4Fallback,

		[Field ("kSSLSessionConfig_TLSv1_fallback")]
		Tls1Fallback,

		[Deprecated (PlatformName.iOS, 11,0)]
		[Deprecated (PlatformName.MacOSX, 10,13)]
		[Deprecated (PlatformName.WatchOS, 4,0)]
		[Deprecated (PlatformName.TvOS, 11,0)]
		[Field ("kSSLSessionConfig_TLSv1_RC4_fallback")]
		Tls1RC4Fallback,

		[Field ("kSSLSessionConfig_legacy")]
		Legacy,

		[Field ("kSSLSessionConfig_legacy_DHE")]
		LegacyDhe,

		[Field ("kSSLSessionConfig_anonymous")]
		Anonymous,

		[iOS (10,2)][TV (10,1)][Mac (10,12,2)]
		[Watch (3,2)]
		[Deprecated (PlatformName.iOS, 11,0)]
		[Deprecated (PlatformName.MacOSX, 10,13)]
		[Deprecated (PlatformName.WatchOS, 4,0)]
		[Deprecated (PlatformName.TvOS, 11,0)]
		[Field ("kSSLSessionConfig_3DES_fallback")]
		ThreeDesFallback,

		[iOS (10,2)][TV (10,1)][Mac (10,12,2)]
		[Watch (3,2)]
		[Deprecated (PlatformName.iOS, 11,0)]
		[Deprecated (PlatformName.MacOSX, 10,13)]
		[Deprecated (PlatformName.WatchOS, 4,0)]
		[Deprecated (PlatformName.TvOS, 11,0)]
		[Field ("kSSLSessionConfig_TLSv1_3DES_fallback")]
		Tls1ThreeDesFallback,
	}

	[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
	[Internal][Static]
	interface SecKeyKeyExchangeParameterKey {
		[Field ("kSecKeyKeyExchangeParameterRequestedSize")]
		NSString RequestedSizeKey { get; }

		[Field ("kSecKeyKeyExchangeParameterSharedInfo")]
		NSString SharedInfoKey { get; }
	}

	[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
	[StrongDictionary ("SecKeyKeyExchangeParameterKey")]
	interface SecKeyKeyExchangeParameter {

		int RequestedSize { get; set; }

		NSData SharedInfo { get; set; }
	}

#if IOS
	[iOS (8,0)][NoTV][NoWatch][NoMac]
	[Internal][Static]
	interface SecSharedCredentialKeys {
		[Field ("kSecAttrServer")]
		NSString ServerKey { get; }

		[Field ("kSecAttrAccount")]
		NSString AccountKey { get; }

		[Field ("kSecSharedPassword")]
		NSString PasswordKey { get; }

		[Field ("kSecAttrPort")]
		NSString PortKey { get; }
	}

	[iOS (8,0)][NoTV][NoWatch][NoMac]
	[StrongDictionary ("SecSharedCredentialKeys")]
	interface SecSharedCredentialInfo {

		string Server { get; set; }

		string Account { get; set; }

		string Password { get; set; }

		int Port { get; set; }
	}
#endif
}
