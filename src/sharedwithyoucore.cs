//
// SharedWithYouCore C# bindings
//
// Authors:
//	Manuel de la Pena Saenz <mandel@microsoft.com>
//
// Copyright 2022 Microsoft Corporation All rights reserved.
//

using System;

using AVFoundation;

using CoreFoundation;

using Foundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace SharedWithYouCore {

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface SWAction : NSCopying, NSSecureCoding {
		[Export ("uuid")]
		NSUuid Uuid { get; }

		[Export ("complete")]
		bool Complete { [Bind ("isComplete")] get; }

		[Export ("fulfill")]
		void Fulfill ();

		[Export ("fail")]
		void Fail ();
	}

	interface ISWCollaborationActionHandler { }

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	[DisableDefaultCtor]
	interface SWCollaborationActionHandler {
		[Abstract]
		[Export ("collaborationCoordinator:handleStartCollaborationAction:")]
		void HandleStartCollaborationAction (SWCollaborationCoordinator coordinator, SWStartCollaborationAction action);

		[Abstract]
		[Export ("collaborationCoordinator:handleUpdateCollaborationParticipantsAction:")]
		void HandleUpdateCollaborationParticipantsAction (SWCollaborationCoordinator coordinator, SWUpdateCollaborationParticipantsAction action);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SWCollaborationCoordinator {
		[Static]
		[Export ("sharedCoordinator", ArgumentSemantic.Strong)]
		SWCollaborationCoordinator SharedCoordinator { get; }

		[NullAllowed, Export ("actionHandler", ArgumentSemantic.Weak)]
		ISWCollaborationActionHandler ActionHandler { get; set; }
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SWCollaborationOption : NSCopying, NSSecureCoding {
		[Export ("title")]
		string Title { get; set; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("subtitle")]
		string Subtitle { get; set; }

		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set; }

		[Export ("requiredOptionsIdentifiers", ArgumentSemantic.Copy)]
		string [] RequiredOptionsIdentifiers { get; set; }

		[Export ("initWithTitle:identifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string title, string identifier);

		[Static]
		[Export ("optionWithTitle:identifier:")]
		SWCollaborationOption Create (string title, string identifier);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SWCollaborationOptionsGroup : NSCopying, NSSecureCoding {

		[Field ("UTCollaborationOptionsTypeIdentifier")]
		NSString TypeIdentifier { get; }

		[Export ("title")]
		string Title { get; set; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("footer")]
		string Footer { get; set; }

		[Export ("options", ArgumentSemantic.Copy)]
		SWCollaborationOption [] Options { get; set; }

		[Export ("initWithIdentifier:options:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, SWCollaborationOption [] options);

		[Static]
		[Export ("optionsGroupWithIdentifier:options:")]
		SWCollaborationOptionsGroup Create (string identifier, SWCollaborationOption [] options);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (SWCollaborationOptionsGroup))]
	interface SWCollaborationOptionsPickerGroup {

		[Export ("initWithIdentifier:options:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, SWCollaborationOption [] options);

		[Export ("selectedOptionIdentifier", ArgumentSemantic.Strong)]
		string SelectedOptionIdentifier { get; set; }
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SWCollaborationShareOptions : NSCopying, NSSecureCoding {
		[Export ("optionsGroups", ArgumentSemantic.Copy)]
		SWCollaborationOptionsGroup [] OptionsGroups { get; set; }

		[Export ("summary")]
		string Summary { get; set; }

		[Export ("initWithOptionsGroups:summary:")]
		[DesignatedInitializer]
		NativeHandle Constructor (SWCollaborationOptionsGroup [] optionsGroups, string summary);

		[Export ("initWithOptionsGroups:")]
		NativeHandle Constructor (SWCollaborationOptionsGroup [] optionsGroups);

		[Static]
		[Export ("shareOptionsWithOptionsGroups:summary:")]
		SWCollaborationShareOptions Create (SWCollaborationOptionsGroup [] optionsGroups, string summary);

		[Static]
		[Export ("shareOptionsWithOptionsGroups:")]
		SWCollaborationShareOptions Create (SWCollaborationOptionsGroup [] optionsGroups);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SWCollaborationMetadata : NSSecureCoding, NSCopying, NSMutableCopying
#if IOS || MONOMAC
		, NSItemProviderReading
		, NSItemProviderWriting
#endif
	{
		[Export ("collaborationIdentifier")]
		string CollaborationIdentifier { get; }

		[Export ("localIdentifier")]
		string LocalIdentifier { get; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("defaultShareOptions", ArgumentSemantic.Copy)]
		SWCollaborationShareOptions DefaultShareOptions { get; set; }

		[NullAllowed, Export ("userSelectedShareOptions", ArgumentSemantic.Copy)]
		SWCollaborationShareOptions UserSelectedShareOptions { get; set; }

		[NullAllowed, Export ("initiatorHandle")]
		string InitiatorHandle { get; set; }

		[NullAllowed, Export ("initiatorNameComponents", ArgumentSemantic.Strong)]
		NSPersonNameComponents InitiatorNameComponents { get; set; }

		[Internal]
		[Export ("initWithLocalIdentifier:")]
		NativeHandle _InitWithLocalIdentifier (string localIdentifier);

		[iOS (16, 1), MacCatalyst (16, 1)]
		[Internal]
		[Export ("initWithCollaborationIdentifier:")]
		NativeHandle _InitWithCollaborationIdentifier (string collaborationIdentifier);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SWPerson : NSSecureCoding {
		[Export ("initWithHandle:identity:displayName:thumbnailImageData:")]
		NativeHandle Constructor ([NullAllowed] string handle, [NullAllowed] SWPersonIdentity identity, string displayName, [NullAllowed] NSData thumbnailImageData);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SWPersonIdentity : NSSecureCoding, NSCopying {
		[Export ("rootHash", ArgumentSemantic.Copy)]
		NSData RootHash { get; }

		[Export ("initWithRootHash:")]
		NativeHandle Constructor (NSData rootHash);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SWPersonIdentityProof : NSSecureCoding, NSCopying {
		[Export ("inclusionHashes", ArgumentSemantic.Copy)]
		NSData [] InclusionHashes { get; }

		[Export ("publicKey", ArgumentSemantic.Copy)]
		NSData PublicKey { get; }

		[Export ("publicKeyIndex")]
		nuint PublicKeyIndex { get; }
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0)]
	[MacCatalyst (16, 0)]
	[BaseType (typeof (SWPersonIdentityProof))]
	interface SWSignedPersonIdentityProof {
		[Export ("initWithPersonIdentityProof:signatureData:")]
		NativeHandle Constructor (SWPersonIdentityProof personIdentityProof, NSData data);

		[Export ("signatureData", ArgumentSemantic.Copy)]
		NSData SignatureData { get; }
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (SWAction))]
	[DisableDefaultCtor]
	interface SWStartCollaborationAction : NSSecureCoding, NSCopying {
		[Export ("collaborationMetadata")]
		SWCollaborationMetadata CollaborationMetadata { get; }

		[Export ("fulfillUsingURL:collaborationIdentifier:")]
		void Fulfill (NSUrl url, string collaborationIdentifier);
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0)]
	[MacCatalyst (16, 0)]
	[BaseType (typeof (SWAction))]
	[DisableDefaultCtor]
	interface SWUpdateCollaborationParticipantsAction : NSSecureCoding, NSCopying {
		[Export ("collaborationMetadata")]
		SWCollaborationMetadata CollaborationMetadata { get; }

		[Export ("addedIdentities")]
		SWPersonIdentity [] AddedIdentities { get; }

		[Export ("removedIdentities")]
		SWPersonIdentity [] RemovedIdentities { get; }
	}

}

