using Foundation;
using ObjCRuntime;

namespace GHIssue18645 {

	[Protocol]
	[BaseType (typeof (NSObject))]
	interface ASCredentialIdentity {

		[Abstract]
		[Export ("user")]
		string User { get; }

		[Abstract]
		[NullAllowed, Export ("recordIdentifier")]
		string RecordIdentifier { get; }

		[Abstract]
		[Export ("rank")]
		nint Rank { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface ASPasskeyCredentialIdentity : ASCredentialIdentity {
		[Export ("relyingPartyIdentifier")]
		string RelyingPartyIdentifier { get; }

		[Export ("userName")]
		string UserName { get; }

		[Export ("credentialID", ArgumentSemantic.Copy)]
		NSData CredentialID { get; }

		[Export ("userHandle", ArgumentSemantic.Copy)]
		NSData UserHandle { get; }

		[NullAllowed, Export ("recordIdentifier")]
		new string RecordIdentifier { get; }

		[Export ("rank")]
		new nint Rank { get; set; }
	}

}
