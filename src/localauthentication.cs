using System;
using ObjCRuntime;
using Foundation;
using Security;

namespace LocalAuthentication {

	[Mac (10,13,2)][iOS (11,0)]
	[Native]
	public enum LABiometryType : long {
		None,
		TouchId,
		[NoMac]
		FaceId,
#if !XAMCORE_4_0
		[NoMac]
		[Obsolete ("Use 'FaceId' instead.")]
		TypeFaceId = FaceId,
#endif
	}

	[iOS (8,0), Mac (10,10)]
	delegate void LAContextReplyHandler (bool success, NSError error);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)] // ".objc_class_name_LAContext", referenced from: '' not found
	[BaseType (typeof (NSObject))]
	interface LAContext {
		[NullAllowed] // by default this property is null
		[Export ("localizedFallbackTitle")]
		string LocalizedFallbackTitle { get; set; }

#if !XAMCORE_4_0
		[iOS (8,3), Mac (10,10)]
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

		[iOS (10,0)][Mac (10,12)]
		[NullAllowed, Export ("localizedCancelTitle")]
		string LocalizedCancelTitle { get; set; }

		[iOS (9,0)][Mac (10,12,4, onlyOn64 : true)]
		[Field ("LATouchIDAuthenticationMaximumAllowableReuseDuration")]
		double /* NSTimeInterval */ TouchIdAuthenticationMaximumAllowableReuseDuration { get; }

		[iOS (9,0)][Mac (10,12,4, onlyOn64 : true)]
		[Export ("touchIDAuthenticationAllowableReuseDuration")]
		double /* NSTimeInterval */ TouchIdAuthenticationAllowableReuseDuration { get; set; }

#if !MONOMAC
		[Availability (Introduced = Platform.iOS_8_3, Deprecated = Platform.iOS_9_0)]
		[NullAllowed]
		[Export ("maxBiometryFailures")]
		NSNumber MaxBiometryFailures { get; set; }
#endif
		[NoWatch, NoTV, Mac (10, 13), iOS (11, 0)]
		[Export ("localizedReason")]
		string LocalizedReason { get; set; }

		[NoWatch, NoTV, Mac (10, 13), iOS (11, 0)]
		[Export ("interactionNotAllowed")]
		bool InteractionNotAllowed { get; set; }

		[Mac (10,13,2)][iOS (11,0)]
		[Export ("biometryType")]
		LABiometryType BiometryType { get; }
	}
}
