using System;
using ObjCRuntime;
using Foundation;
using Security;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace LocalAuthentication {

	[Mac (10,13,2)][iOS (11,0)][NoWatch][NoTV]
	[Native]
	public enum LABiometryType : long {
		None,
		TouchId,
		[Mac (10,15)]
		FaceId,
#if !NET
		[NoMac]
		[Obsolete ("Use 'FaceId' instead.")]
		TypeFaceId = FaceId,
#endif
	}

	[iOS (8,0), Mac (10,10)]
	delegate void LAContextReplyHandler (bool success, NSError error);

	[iOS (8,0), Mac (10,10), NoTV] // ".objc_class_name_LAContext", referenced from: '' not found
	[BaseType (typeof (NSObject))]
	interface LAContext {
		[NoWatch]
		[NullAllowed] // by default this property is null
		[Export ("localizedFallbackTitle")]
		string LocalizedFallbackTitle { get; set; }

#if !NET
		[iOS (8,3), NoTV]
		[Field ("LAErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[Export ("canEvaluatePolicy:error:")]
		bool CanEvaluatePolicy (LAPolicy policy, out NSError error);

		[Async]
		[Export ("evaluatePolicy:localizedReason:reply:")]
		void EvaluatePolicy (LAPolicy policy, string localizedReason, LAContextReplyHandler reply);

		[iOS (9,0), Mac(10,11)]
		[Export ("invalidate")]
		void Invalidate ();

		[iOS (9,0), Mac(10,11)]
		[Export ("setCredential:type:")]
		bool SetCredentialType ([NullAllowed] NSData credential, LACredentialType type);

		
		[iOS (9,0), Mac(10,11)]
		[Export ("isCredentialSet:")]
		bool IsCredentialSet (LACredentialType type);

		
		[iOS (9,0), Mac(10,11)]
		[Export ("evaluateAccessControl:operation:localizedReason:reply:")]
		void EvaluateAccessControl (SecAccessControl accessControl, LAAccessControlOperation operation, string localizedReason, Action<bool,NSError> reply);
		
		
		[iOS (9,0), Mac(10,11)]
		[Export ("evaluatedPolicyDomainState")]
		[NullAllowed]
		NSData EvaluatedPolicyDomainState { get; }

		[iOS (10,0)][Mac (10,12)][NoWatch]
		[NullAllowed, Export ("localizedCancelTitle")]
		string LocalizedCancelTitle { get; set; }

		[iOS (9,0)][Mac (10,12,4)][NoWatch]
		[Field ("LATouchIDAuthenticationMaximumAllowableReuseDuration")]
		double /* NSTimeInterval */ TouchIdAuthenticationMaximumAllowableReuseDuration { get; }

		[iOS (9,0)][Mac (10,12,4)]
		[Export ("touchIDAuthenticationAllowableReuseDuration")]
		double /* NSTimeInterval */ TouchIdAuthenticationAllowableReuseDuration { get; set; }

		[iOS (8, 3), Deprecated (PlatformName.iOS, 9, 0)]
		[Mac (10, 10, 3), Deprecated (PlatformName.MacOSX, 10, 11)]
		[NullAllowed]
		[Export ("maxBiometryFailures")]
		NSNumber MaxBiometryFailures { get; set; }

		[NoWatch, NoTV, Mac (10, 13), iOS (11, 0)]
		[Export ("localizedReason")]
		string LocalizedReason { get; set; }

		[Watch (9,0), NoTV, Mac (10, 13), iOS (11, 0)]
		[Export ("interactionNotAllowed")]
		bool InteractionNotAllowed { get; set; }

		[Mac (10,13,2)][iOS (11,0)][NoWatch]
		[Export ("biometryType")]
		LABiometryType BiometryType { get; }
	}

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), NoWatch, NoTV]
	[BaseType (typeof (LARight))]
	[DisableDefaultCtor]
	interface LAPersistedRight
	{
		[Export ("key")]
		LAPrivateKey Key { get; }

		[Export ("secret")]
		LASecret Secret { get; }
	}

	delegate void LAPrivateKeyCompletionHandler ([NullAllowed] NSData data, [NullAllowed] NSError error);

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface LAPrivateKey
	{
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

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface LAPublicKey
	{
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

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	interface LAAuthenticationRequirement
	{
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

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	interface LABiometryFallbackRequirement
	{
		[Static]
		[Export ("defaultRequirement")]
		LABiometryFallbackRequirement DefaultRequirement { get; }

		[Static]
		[Export ("devicePasscodeRequirement")]
		LABiometryFallbackRequirement DevicePasscodeRequirement { get; }
	}

	delegate void LARightAuthorizeCompletionHandler ([NullAllowed] NSError error);

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	interface LARight
	{
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

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), NoWatch, NoTV]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface LARightStore
	{
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

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface LASecret
	{
		[Async]
		[Export ("loadDataWithCompletion:")]
		void LoadData (LASecretCompletionHandler handler);
	}
}
