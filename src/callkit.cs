//
// CallKit bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using AVFoundation;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CallKit {

	/// <summary>Enumerates call directory states.</summary>
	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CXCallDirectoryEnabledStatus : long {
		Unknown = 0,
		Disabled = 1,
		Enabled = 2
	}

	/// <summary>Enumerates Call Kit errors.</summary>
	[NoMac, MacCatalyst (14, 0)]
	[ErrorDomain ("CXErrorDomain")]
	[Native]
	public enum CXErrorCode : long {
		Unknown = 0,
		Unentitled = 1,
		InvalidArgument = 2,
		MissingVoIPBackgroundMode = 3,
	}

	/// <summary>Enumerates incoming call errors.</summary>
	[NoMac]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("CXErrorDomainIncomingCall")]
	[Native]
	public enum CXErrorCodeIncomingCallError : long {
		Unknown = 0,
		Unentitled = 1,
		CallUuidAlreadyExists = 2,
		FilteredByDoNotDisturb = 3,
		FilteredByBlockList = 4,
		FilteredDuringRestrictedSharingMode = 5,
		CallIsProtected = 6,
	}

	/// <summary>Enumerates transaction request errors.</summary>
	[NoMac]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("CXErrorDomainRequestTransaction")]
	[Native]
	public enum CXErrorCodeRequestTransactionError : long {
		Unknown = 0,
		Unentitled = 1,
		UnknownCallProvider = 2,
		EmptyTransaction = 3,
		UnknownCallUuid = 4,
		CallUuidAlreadyExists = 5,
		InvalidAction = 6,
		MaximumCallGroupsReached = 7,
		CallIsProtected = 8,
	}

	/// <summary>Enumerates directory manager errors.</summary>
	[NoMac]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("CXErrorDomainCallDirectoryManager")]
	[Native]
	public enum CXErrorCodeCallDirectoryManagerError : long {
		Unknown = 0,
		NoExtensionFound = 1,
		LoadingInterrupted = 2,
		EntriesOutOfOrder = 3,
		DuplicateEntries = 4,
		MaximumEntriesExceeded = 5,
		ExtensionDisabled = 6,
		CurrentlyLoading = 7,
		UnexpectedIncrementalRemoval = 8,
	}

	[iOS (14, 5), NoTV, NoMac]
	[Introduced (PlatformName.MacCatalyst, 14, 5)]
	[ErrorDomain ("CXErrorDomainNotificationServiceExtension")]
	[Native]
	public enum CXErrorCodeNotificationServiceExtensionError : long {
		Unknown = 0,
		InvalidClientProcess = 1,
		MissingNotificationFilteringEntitlement = 2,
	}

#if NET
	/// <summary>Enumerates DTMF play action types.</summary>
	[NoMac]
#else
	[Obsoleted (PlatformName.MacOSX, 12, 1)]
#endif
	[MacCatalyst (13, 1)]
	[Native]
	public enum CXPlayDtmfCallActionType : long {
		SingleTone = 1,
		SoftPause = 2,
		HardPause = 3,
	}

#if NET
	/// <summary>Enumerates reasons that calls can end.</summary>
	[NoMac]
#else
	[Obsoleted (PlatformName.MacOSX, 12, 1)]
#endif
	[MacCatalyst (13, 1)]
	[Native]
	public enum CXCallEndedReason : long {
		Failed = 1,
		RemoteEnded = 2,
		Unanswered = 3,
		AnsweredElsewhere = 4,
		DeclinedElsewhere = 5,
	}

#if NET
	/// <summary>Enumerates handle types.</summary>
	///     <remarks>Handles act as identifiers for VOIP users.</remarks>
	[NoMac]
#else
	[Obsoleted (PlatformName.MacOSX, 12, 1)]
#endif
	[MacCatalyst (13, 1)]
	[Native]
	public enum CXHandleType : long {
		Generic = 1,
		PhoneNumber = 2,
		EmailAddress = 3,
	}

	/// <summary>A unique identifier for a VOIP user.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXHandle">Apple documentation for <c>CXHandle</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CXHandle : NSCopying, NSSecureCoding {

		[Export ("type")]
		CXHandleType Type { get; }

		[Export ("value")]
		string Value { get; }

		[Export ("initWithType:value:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CXHandleType type, string value);

		[Export ("isEqualToHandle:")]
		bool IsEqual (CXHandle handle);
	}

	/// <summary>Base class for CallKit actions, such as those taken when a call begins or ends, a call is put on hold, and so on.</summary>
	///     <remarks>Developers manage the life cycle of a call by sending and receiving objects that derive from <see cref="T:CallKit.CXAction" /> to and from <see cref="T:CallKit.CXProvider" /> and <see cref="T:CallKit.CXCallController" /> objects.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXAction">Apple documentation for <c>CXAction</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface CXAction : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("UUID", ArgumentSemantic.Copy)]
		NSUuid Uuid { get; }

		[Export ("complete", ArgumentSemantic.Assign)]
		bool Complete { [Bind ("isComplete")] get; }

		[Export ("timeoutDate", ArgumentSemantic.Strong)]
		NSDate TimeoutDate { get; }

		[Export ("fulfill")]
		void Fulfill ();

		[Export ("fail")]
		void Fail ();
	}

	/// <summary>Contains the information that the application needs in order to answer a call at the user's request.</summary>
	///     <remarks>
	///       <see cref="T:CallKit.CXAnswerCallAction" /> objects are passed to the developer's <see cref="M:CallKit.CXProviderDelegate.PerformAnswerCallAction(CallKit.CXProvider,CallKit.CXAnswerCallAction)" /> method when the user answers a call.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXAnswerCallAction">Apple documentation for <c>CXAnswerCallAction</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CXCallAction))]
	[DisableDefaultCtor]
	interface CXAnswerCallAction {

		[Export ("initWithCallUUID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUuid callUuid);

		[Export ("fulfillWithDateConnected:")]
		void Fulfill (NSDate dateConnected);
	}

	/// <summary>Represents a CallKit call.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXCall">Apple documentation for <c>CXCall</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CXCall {

		[Export ("UUID", ArgumentSemantic.Copy)]
		NSUuid Uuid { get; }

		[Export ("outgoing")]
		bool Outgoing { [Bind ("isOutgoing")] get; }

		[Export ("onHold")]
		bool OnHold { [Bind ("isOnHold")] get; }

		[Export ("hasConnected")]
		bool HasConnected { get; }

		[Export ("hasEnded")]
		bool HasEnded { get; }

		[Export ("isEqualToCall:")]
		bool IsEqual (CXCall call);
	}

	/// <summary>Base class for objects that contain the information that is needed to perform an action on a call.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXCallAction">Apple documentation for <c>CXCallAction</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CXAction))]
	[DisableDefaultCtor]
	interface CXCallAction {

		[Export ("callUUID", ArgumentSemantic.Copy)]
		NSUuid CallUuid { get; }

		[Export ("initWithCallUUID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUuid callUuid);
	}

	/// <summary>Informs the system about in-band user actions, such as reqeusts to start a call, or to put a call on hold.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXCallController">Apple documentation for <c>CXCallController</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CXCallController {

		[Export ("initWithQueue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (DispatchQueue queue);

		[Export ("callObserver", ArgumentSemantic.Strong)]
		CXCallObserver CallObserver { get; }

		[Async]
		[Export ("requestTransaction:completion:")]
		void RequestTransaction (CXTransaction transaction, Action<NSError> completion);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("requestTransactionWithActions:completion:")]
		void RequestTransaction (CXAction [] actions, Action<NSError> completion);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("requestTransactionWithAction:completion:")]
		void RequestTransaction (CXAction action, Action<NSError> completion);
	}

	/// <summary>Extension context for a call directory.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXCallDirectoryExtensionContext">Apple documentation for <c>CXCallDirectoryExtensionContext</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSExtensionContext))]
	interface CXCallDirectoryExtensionContext {

		[Export ("addBlockingEntryWithNextSequentialPhoneNumber:")]
		void AddBlockingEntry (/* CXCallDirectoryPhoneNumber -> int64_t */ long phoneNumber);

		[Export ("addIdentificationEntryWithNextSequentialPhoneNumber:label:")]
		void AddIdentificationEntry (/* CXCallDirectoryPhoneNumber -> int64_t */ long phoneNumber, string label);

		[Async]
		[Export ("completeRequestWithCompletionHandler:")]
		void CompleteRequest ([NullAllowed] Action<bool> completion);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		[MacCatalyst (13, 1)]
		ICXCallDirectoryExtensionContextDelegate Delegate { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("incremental")]
		bool Incremental { [Bind ("isIncremental")] get; }

		[MacCatalyst (13, 1)]
		[Export ("removeBlockingEntryWithPhoneNumber:")]
		void RemoveBlockingEntry (/* CXCallDirectoryPhoneNumber -> int64_t */ long phoneNumber);

		[MacCatalyst (13, 1)]
		[Export ("removeAllBlockingEntries")]
		void RemoveAllBlockingEntries ();

		[MacCatalyst (13, 1)]
		[Export ("removeIdentificationEntryWithPhoneNumber:")]
		void RemoveIdentificationEntry (/* CXCallDirectoryPhoneNumber -> int64_t */ long phoneNumber);

		[MacCatalyst (13, 1)]
		[Export ("removeAllIdentificationEntries")]
		void RemoveAllIdentificationEntries ();
	}

	interface ICXCallDirectoryExtensionContextDelegate { }

	/// <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXCallDirectoryExtensionContextDelegate">Apple documentation for <c>CXCallDirectoryExtensionContextDelegate</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface CXCallDirectoryExtensionContextDelegate {

		[Abstract]
		[Export ("requestFailedForExtensionContext:withError:")]
		void RequestFailed (CXCallDirectoryExtensionContext extensionContext, NSError error);
	}

	/// <summary>Manages a call directory extension.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXCallDirectoryManager">Apple documentation for <c>CXCallDirectoryManager</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CXCallDirectoryManager {

		[Static]
		[Export ("sharedInstance")]
		CXCallDirectoryManager SharedInstance { get; }

		[Async]
		[Export ("reloadExtensionWithIdentifier:completionHandler:")]
		void ReloadExtension (string identifier, [NullAllowed] Action<NSError> completion);

		[Async]
		[Export ("getEnabledStatusForExtensionWithIdentifier:completionHandler:")]
		void GetEnabledStatusForExtension (string identifier, Action<CXCallDirectoryEnabledStatus, NSError> completion);

		[NoTV, NoMac, iOS (13, 4), MacCatalyst (14, 0)]
		[Async]
		[Export ("openSettingsWithCompletionHandler:")]
		void OpenSettings ([NullAllowed] Action<NSError> completion);
	}

	/// <summary>Call directory extension provider.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXCallDirectoryProvider">Apple documentation for <c>CXCallDirectoryProvider</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CXCallDirectoryProvider : NSExtensionRequestHandling {

	}

	/// <summary>Interface that represents the required methods (if any) of the <see cref="T:CallKit.CXCallObserverDelegate" /> protocol.</summary>
	interface ICXCallObserverDelegate { }

	/// <summary>Delegate object that responds to call changes.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXCallObserverDelegate">Apple documentation for <c>CXCallObserverDelegate</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CXCallObserverDelegate {

		[Abstract]
		[Export ("callObserver:callChanged:")]
		void CallChanged (CXCallObserver callObserver, CXCall call);
	}

	/// <summary>Observer for the calls in a <see cref="T:CallKit.CXCallController" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXCallObserver">Apple documentation for <c>CXCallObserver</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CXCallObserver {

		[Export ("calls", ArgumentSemantic.Copy)]
		CXCall [] Calls { get; }

		[Export ("setDelegate:queue:")]
		void SetDelegate ([NullAllowed] ICXCallObserverDelegate aDelegate, [NullAllowed] DispatchQueue queue);
	}

	/// <summary>Contains values with which to update a call's parameters.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXCallUpdate">Apple documentation for <c>CXCallUpdate</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CXCallUpdate : NSCopying {

		[NullAllowed, Export ("remoteHandle", ArgumentSemantic.Copy)]
		CXHandle RemoteHandle { get; set; }

		[NullAllowed, Export ("localizedCallerName")]
		string LocalizedCallerName { get; set; }

		[Export ("supportsHolding", ArgumentSemantic.Assign)]
		bool SupportsHolding { get; set; }

		[Export ("supportsGrouping", ArgumentSemantic.Assign)]
		bool SupportsGrouping { get; set; }

		[Export ("supportsUngrouping", ArgumentSemantic.Assign)]
		bool SupportsUngrouping { get; set; }

		[Export ("supportsDTMF", ArgumentSemantic.Assign)]
		bool SupportsDtmf { get; set; }

		[Export ("hasVideo")]
		bool HasVideo { get; set; }
	}

	/// <summary>Contains the information that the application needs in order to end a call.</summary>
	///     <remarks>
	///       <see cref="T:CallKit.CXAnswerCallAction" /> objects are passed to the developer's <see cref="M:CallKit.CXProviderDelegate.PerformEndCallAction(CallKit.CXProvider,CallKit.CXEndCallAction)" /> method when a call is ended.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXEndCallAction">Apple documentation for <c>CXEndCallAction</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (CXCallAction))]
	interface CXEndCallAction {

		[Export ("initWithCallUUID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUuid callUuid);

		[Export ("fulfillWithDateEnded:")]
		void Fulfill (NSDate dateEnded);
	}

	/// <summary>Contains the information that is needed to play a DTMF signal that represents a touch tone.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXPlayDTMFCallAction">Apple documentation for <c>CXPlayDTMFCallAction</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (CXCallAction), Name = "CXPlayDTMFCallAction")]
	interface CXPlayDtmfCallAction {

		[Export ("initWithCallUUID:digits:type:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUuid callUuid, string digits, CXPlayDtmfCallActionType type);

		[Export ("digits")]
		string Digits { get; set; }

		[Export ("type", ArgumentSemantic.Assign)]
		CXPlayDtmfCallActionType Type { get; set; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:CallKit.CXProviderDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:CallKit.CXProviderDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:CallKit.CXProviderDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:CallKit.CXProviderDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ICXProviderDelegate { }

	/// <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXProviderDelegate">Apple documentation for <c>CXProviderDelegate</c></related>
	[Protocol, Model]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CXProviderDelegate {

		[Abstract]
		[Export ("providerDidReset:")]
		void DidReset (CXProvider provider);

		[Export ("providerDidBegin:")]
		void DidBegin (CXProvider provider);

		[Export ("provider:executeTransaction:")]
		bool ExecuteTransaction (CXProvider provider, CXTransaction transaction);

		[Export ("provider:performStartCallAction:")]
		void PerformStartCallAction (CXProvider provider, CXStartCallAction action);

		[Export ("provider:performAnswerCallAction:")]
		void PerformAnswerCallAction (CXProvider provider, CXAnswerCallAction action);

		[Export ("provider:performEndCallAction:")]
		void PerformEndCallAction (CXProvider provider, CXEndCallAction action);

		[Export ("provider:performSetHeldCallAction:")]
		void PerformSetHeldCallAction (CXProvider provider, CXSetHeldCallAction action);

		[Export ("provider:performSetMutedCallAction:")]
		void PerformSetMutedCallAction (CXProvider provider, CXSetMutedCallAction action);

		[Export ("provider:performSetGroupCallAction:")]
		void PerformSetGroupCallAction (CXProvider provider, CXSetGroupCallAction action);

		[Export ("provider:performPlayDTMFCallAction:")]
		void PerformPlayDtmfCallAction (CXProvider provider, CXPlayDtmfCallAction action);

		[Export ("provider:timedOutPerformingAction:")]
		void TimedOutPerformingAction (CXProvider provider, CXAction action);

		// Xcode 12 beta 1 issue, AVAudioSession does not appear on Mac OS X but this methods do: https://github.com/xamarin/maccore/issues/2257 
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("provider:didActivateAudioSession:")]
		void DidActivateAudioSession (CXProvider provider, AVAudioSession audioSession);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("provider:didDeactivateAudioSession:")]
		void DidDeactivateAudioSession (CXProvider provider, AVAudioSession audioSession);
	}

	/// <summary>Reports external (out-of-band) events, such as incoming calls, to the system, and receives internal (in-band) user action events from the system.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXProvider">Apple documentation for <c>CXProvider</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CXProvider {

		[Export ("initWithConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CXProviderConfiguration configuration);

		[Export ("setDelegate:queue:")]
		void SetDelegate ([NullAllowed] ICXProviderDelegate aDelegate, [NullAllowed] DispatchQueue queue);

		[Async]
		[Export ("reportNewIncomingCallWithUUID:update:completion:")]
		void ReportNewIncomingCall (NSUuid uuid, CXCallUpdate update, Action<NSError> completion);

		[Export ("reportCallWithUUID:updated:")]
		void ReportCall (NSUuid uuid, CXCallUpdate update);

		[Export ("reportCallWithUUID:endedAtDate:reason:")]
		void ReportCall (NSUuid uuid, [NullAllowed] NSDate dateEnded, CXCallEndedReason endedReason);

		[Export ("reportOutgoingCallWithUUID:startedConnectingAtDate:")]
		void ReportConnectingOutgoingCall (NSUuid uuid, [NullAllowed] NSDate dateStartedConnecting);

		[Export ("reportOutgoingCallWithUUID:connectedAtDate:")]
		void ReportConnectedOutgoingCall (NSUuid uuid, [NullAllowed] NSDate dateConnected);

		[NoTV, NoMac, iOS (14, 5)]
		[Introduced (PlatformName.MacCatalyst, 14, 5)]
		[Static, Async]
		[Export ("reportNewIncomingVoIPPushPayload:completion:")]
		void ReportNewIncomingVoIPPushPayload (NSDictionary dictionaryPayload, [NullAllowed] Action<NSError> completion);

		[Export ("configuration", ArgumentSemantic.Copy)]
		CXProviderConfiguration Configuration { get; set; }

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("pendingTransactions", ArgumentSemantic.Copy)]
		CXTransaction [] PendingTransactions { get; }

		[Export ("pendingCallActionsOfClass:withCallUUID:")]
		CXCallAction [] GetPendingCallActions (Class callActionClass, NSUuid callUuid);
	}

	/// <summary>Contains values that control miscellaneous call properties, such as the ringtone, whether the call supports video, the maximum number of callers, and so on.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXProviderConfiguration">Apple documentation for <c>CXProviderConfiguration</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CXProviderConfiguration : NSCopying {

		[NoMac] // deprecated and was never added to Mac OS X before
		[Deprecated (PlatformName.iOS, 14, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("localizedName"), NullAllowed]
		string LocalizedName { get; }

		[NullAllowed, Export ("ringtoneSound", ArgumentSemantic.Strong)]
		string RingtoneSound { get; set; }

		[Advice ("40x40 points squared image.")]
		[NullAllowed, Export ("iconTemplateImageData", ArgumentSemantic.Copy)]
		NSData IconTemplateImageData { get; set; }

		[Export ("maximumCallGroups")]
		nuint MaximumCallGroups { get; set; }

		[Export ("maximumCallsPerCallGroup")]
		nuint MaximumCallsPerCallGroup { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("includesCallsInRecents")]
		bool IncludesCallsInRecents { get; set; }

		[Export ("supportsVideo")]
		bool SupportsVideo { get; set; }

		[Export ("supportedHandleTypes", ArgumentSemantic.Copy)]
		NSSet<NSNumber> SupportedHandleTypes { get; set; }

		[NoMac] // deprecated and was never added to Mac OS X before 
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use the default constructor instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the default constructor instead.")]
		[MacCatalyst (13, 1)]
		[Export ("initWithLocalizedName:")]
		NativeHandle Constructor (string localizedName);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();
	}

	/// <summary>Contains the data that are needed to join a group call.</summary>
	///     <remarks>
	///       <see cref="T:CallKit.CXSetGroupCallAction" /> objects are passed to the developer's <see cref="M:CallKit.CXProviderDelegate.PerformSetGroupCallAction(CallKit.CXProvider,CallKit.CXSetGroupCallAction)" /> method when the user joins a call.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXSetGroupCallAction">Apple documentation for <c>CXSetGroupCallAction</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CXCallAction))]
	[DisableDefaultCtor]
	interface CXSetGroupCallAction {

		[Export ("initWithCallUUID:callUUIDToGroupWith:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUuid callUuid, [NullAllowed] NSUuid callUuidToGroupWith);

		[NullAllowed, Export ("callUUIDToGroupWith", ArgumentSemantic.Copy)]
		NSUuid CallUuidToGroupWith { get; set; }
	}

	/// <summary>Contains the information that is needed to put a call on hold or take a call off hold.</summary>
	///     <remarks>
	///       <see cref="T:CallKit.CXSetHeldCallAction" /> objects are passed to the developer's <see cref="M:CallKit.CXProviderDelegate.PerformSetHeldCallAction(CallKit.CXProvider,CallKit.CXSetHeldCallAction)" /> method when the user puts a call on hold or takes a call off hold.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXSetHeldCallAction">Apple documentation for <c>CXSetHeldCallAction</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (CXCallAction))]
	interface CXSetHeldCallAction {

		[Export ("initWithCallUUID:onHold:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUuid callUuid, bool onHold);

		[Export ("onHold")]
		bool OnHold { [Bind ("isOnHold")] get; set; }
	}

	/// <summary>Contains the information that is necessary to mute or unmute a call.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXSetMutedCallAction">Apple documentation for <c>CXSetMutedCallAction</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CXCallAction))]
	[DisableDefaultCtor]
	interface CXSetMutedCallAction {

		// needs to be reexposed
		[Export ("initWithCallUUID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUuid callUuid);

		[Export ("initWithCallUUID:muted:")]
		NativeHandle Constructor (NSUuid callUuid, bool muted);

		[Export ("muted")]
		bool Muted { [Bind ("isMuted")] get; set; }
	}

	/// <summary>Contains the information that is necessary to start a call.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXStartCallAction">Apple documentation for <c>CXStartCallAction</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (CXCallAction))]
	interface CXStartCallAction {

		// initWithCallUUID: explicitly marked with NS_UNAVAILABLE

		[Export ("initWithCallUUID:handle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUuid callUuid, CXHandle callHandle);

		[Export ("handle", ArgumentSemantic.Copy)]
		CXHandle CallHandle { get; set; }

		[NullAllowed, Export ("contactIdentifier")]
		string ContactIdentifier { get; set; }

		[Export ("video")]
		bool Video { [Bind ("isVideo")] get; set; }

		[Export ("fulfillWithDateStarted:")]
		void Fulfill (NSDate dateStarted);
	}

	/// <summary>Runs a group of Call Kit actions atomically.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CallKit/CXTransaction">Apple documentation for <c>CXTransaction</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // there's a designated initializer that does not accept null
	interface CXTransaction : NSCopying, NSSecureCoding {

		[Export ("initWithActions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CXAction [] actions);

		[Export ("initWithAction:")]
		NativeHandle Constructor (CXAction action);

		[Export ("UUID", ArgumentSemantic.Copy)]
		NSUuid Uuid { get; }

		[Export ("complete", ArgumentSemantic.Assign)]
		bool Complete { [Bind ("isComplete")] get; }

		[Export ("actions", ArgumentSemantic.Copy)]
		CXAction [] Actions { get; }

		[Export ("addAction:")]
		void AddAction (CXAction action);
	}
}
