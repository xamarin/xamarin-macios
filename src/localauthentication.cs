using System;
using ObjCRuntime;
using Foundation;
using Security;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace LocalAuthentication {

	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum LABiometryType : long {
		None,
		TouchId,
		[MacCatalyst (13, 1)]
		FaceId,
#if !NET
		[NoMac]
		[Obsolete ("Use 'FaceId' instead.")]
		TypeFaceId = FaceId,
#endif
		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		OpticId = 1L << 2,
	}

	[MacCatalyst (13, 1)]
	delegate void LAContextReplyHandler (bool success, NSError error);

	[NoTV] // ".objc_class_name_LAContext", referenced from: '' not found
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface LAContext {
		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("localizedFallbackTitle")]
		string LocalizedFallbackTitle { get; set; }

#if !NET
		[NoTV]
		[Field ("LAErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[Export ("canEvaluatePolicy:error:")]
		bool CanEvaluatePolicy (LAPolicy policy, out NSError error);

		[Async]
		[Export ("evaluatePolicy:localizedReason:reply:")]
		void EvaluatePolicy (LAPolicy policy, string localizedReason, LAContextReplyHandler reply);

		[MacCatalyst (13, 1)]
		[Export ("invalidate")]
		void Invalidate ();

		[MacCatalyst (13, 1)]
		[Export ("setCredential:type:")]
		bool SetCredentialType ([NullAllowed] NSData credential, LACredentialType type);


		[MacCatalyst (13, 1)]
		[Export ("isCredentialSet:")]
		bool IsCredentialSet (LACredentialType type);


		[MacCatalyst (13, 1)]
		[Export ("evaluateAccessControl:operation:localizedReason:reply:")]
		void EvaluateAccessControl (SecAccessControl accessControl, LAAccessControlOperation operation, string localizedReason, Action<bool, NSError> reply);


		[MacCatalyst (13, 1)]
		[Export ("evaluatedPolicyDomainState")]
		[NullAllowed]
		NSData EvaluatedPolicyDomainState { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("localizedCancelTitle")]
		string LocalizedCancelTitle { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("LATouchIDAuthenticationMaximumAllowableReuseDuration")]
		double /* NSTimeInterval */ TouchIdAuthenticationMaximumAllowableReuseDuration { get; }

		[MacCatalyst (13, 1)]
		[Export ("touchIDAuthenticationAllowableReuseDuration")]
		double /* NSTimeInterval */ TouchIdAuthenticationAllowableReuseDuration { get; set; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[NullAllowed]
		[Export ("maxBiometryFailures")]
		NSNumber MaxBiometryFailures { get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("localizedReason")]
		string LocalizedReason { get; set; }

		[Watch (9, 0), NoTV]
		[MacCatalyst (13, 1)]
		[Export ("interactionNotAllowed")]
		bool InteractionNotAllowed { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("biometryType")]
		LABiometryType BiometryType { get; }
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
	[BaseType (typeof (LARight))]
	[DisableDefaultCtor]
	interface LAPersistedRight {
		[Export ("key")]
		LAPrivateKey Key { get; }

		[Export ("secret")]
		LASecret Secret { get; }
	}

	delegate void LAPrivateKeyCompletionHandler ([NullAllowed] NSData data, [NullAllowed] NSError error);

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface LAPrivateKey {
		[Export ("publicKey")]
		LAPublicKey PublicKey { get; }

		[Async]
		[Export ("signData:secKeyAlgorithm:completion:")]
		void Sign (NSData data, SecKeyAlgorithm algorithm, LAPrivateKeyCompletionHandler handler);

		[Export ("canSignUsingSecKeyAlgorithm:")]
		bool CanSign (SecKeyAlgorithm algorithm);

		[Async]
		[Export ("decryptData:secKeyAlgorithm:completion:")]
		void Decrypt (NSData data, SecKeyAlgorithm algorithm, LAPrivateKeyCompletionHandler handler);

		[Export ("canDecryptUsingSecKeyAlgorithm:")]
		bool CanDecrypt (SecKeyAlgorithm algorithm);

		[Async]
		[Export ("exchangeKeysWithPublicKey:secKeyAlgorithm:secKeyParameters:completion:")]
		void ExchangeKeys (NSData publicKey, SecKeyAlgorithm algorithm, NSDictionary parameters, LAPrivateKeyCompletionHandler handler);

		[Export ("canExchangeKeysUsingSecKeyAlgorithm:")]
		bool CanExchangeKeys (SecKeyAlgorithm algorithm);
	}

	delegate void LAPublicKeyCompletionHandler ([NullAllowed] NSData data, [NullAllowed] NSError error);
	delegate void LAPublicKeyVerifyDataCompletionHandler ([NullAllowed] NSError error);

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface LAPublicKey {
		[Async]
		[Export ("exportBytesWithCompletion:")]
		void ExportBytes (LAPublicKeyCompletionHandler handler);

		[Async]
		[Export ("encryptData:secKeyAlgorithm:completion:")]
		void Encrypt (NSData data, SecKeyAlgorithm algorithm, LAPublicKeyCompletionHandler handler);

		[Export ("canEncryptUsingSecKeyAlgorithm:")]
		bool CanEncrypt (SecKeyAlgorithm algorithm);

		[Async]
		[Export ("verifyData:signature:secKeyAlgorithm:completion:")]
		void Verify (NSData signedData, NSData signature, SecKeyAlgorithm algorithm, LAPublicKeyVerifyDataCompletionHandler handler);

		[Export ("canVerifyUsingSecKeyAlgorithm:")]
		bool CanVerify (SecKeyAlgorithm algorithm);
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	interface LAAuthenticationRequirement {
		[Static]
		[Export ("defaultRequirement")]
		LAAuthenticationRequirement DefaultRequirement { get; }

		[Static]
		[Export ("biometryRequirement")]
		LAAuthenticationRequirement BiometryRequirement { get; }

		[Static]
		[Export ("biometryCurrentSetRequirement")]
		LAAuthenticationRequirement BiometryCurrentSetRequirement { get; }

		[Static]
		[Export ("biometryRequirementWithFallback:")]
		LAAuthenticationRequirement GetBiometryRequirement (LABiometryFallbackRequirement fallback);
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	interface LABiometryFallbackRequirement {
		[Static]
		[Export ("defaultRequirement")]
		LABiometryFallbackRequirement DefaultRequirement { get; }

		[Static]
		[Export ("devicePasscodeRequirement")]
		LABiometryFallbackRequirement DevicePasscodeRequirement { get; }
	}

	delegate void LARightAuthorizeCompletionHandler ([NullAllowed] NSError error);

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	interface LARight {
		[Export ("state")]
		LARightState State { get; }

		[Export ("tag")]
		nint Tag { get; set; }

		[Export ("initWithRequirement:")]
		NativeHandle Constructor (LAAuthenticationRequirement requirement);

		[Async]
		[Export ("authorizeWithLocalizedReason:completion:")]
		void Authorize (string localizedReason, LARightAuthorizeCompletionHandler handler);

		[Async]
		[Export ("checkCanAuthorizeWithCompletion:")]
		void CheckCanAuthorize (LARightAuthorizeCompletionHandler handler);

		[Async]
		[Export ("deauthorizeWithCompletion:")]
		void Deauthorize (Action handler);
	}

	delegate void LARightStoreCompletionHandler ([NullAllowed] LAPersistedRight right, [NullAllowed] NSError error);
	delegate void LARightStoreRemoveRightCompletionHandler ([NullAllowed] NSError error);

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface LARightStore {
		[Static]
		[Export ("sharedStore")]
		LARightStore SharedStore { get; }

		[Async]
		[Export ("rightForIdentifier:completion:")]
		void Get (string identifier, LARightStoreCompletionHandler handler);

		[Async]
		[Export ("saveRight:identifier:completion:")]
		void Save (LARight right, string identifier, LARightStoreCompletionHandler handler);

		[Async]
		[Export ("saveRight:identifier:secret:completion:")]
		void Save (LARight right, string identifier, NSData secret, LARightStoreCompletionHandler handler);

		[Async]
		[Export ("removeRight:completion:")]
		void Remove (LAPersistedRight right, LARightStoreRemoveRightCompletionHandler handler);

		[Async]
		[Export ("removeRightForIdentifier:completion:")]
		void Remove (string identifier, LARightStoreRemoveRightCompletionHandler handler);

		[Async]
		[Export ("removeAllRightsWithCompletion:")]
		void RemoveAll (LARightStoreRemoveRightCompletionHandler handler);
	}

	delegate void LASecretCompletionHandler ([NullAllowed] NSData data, [NullAllowed] NSError error);

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface LASecret {
		[Async]
		[Export ("loadDataWithCompletion:")]
		void LoadData (LASecretCompletionHandler handler);
	}
}
