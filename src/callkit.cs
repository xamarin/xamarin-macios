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

	[NoMac, NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CXCallDirectoryEnabledStatus : long {
		Unknown = 0,
		Disabled = 1,
		Enabled = 2
	}

	[NoMac, MacCatalyst (14, 0), Watch (9, 0)]
	[ErrorDomain ("CXErrorDomain")]
	[Native]
	public enum CXErrorCode : long {
		Unknown = 0,
		Unentitled = 1,
		InvalidArgument = 2,
		MissingVoIPBackgroundMode = 3,
	}

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
	}

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
	}

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

	[iOS (14, 5), Watch (9, 0), NoTV, NoMac]
	[Introduced (PlatformName.MacCatalyst, 14, 5)]
	[ErrorDomain ("CXErrorDomainNotificationServiceExtension")]
	[Native]
	public enum CXErrorCodeNotificationServiceExtensionError : long {
		Unknown = 0,
		InvalidClientProcess = 1,
		MissingNotificationFilteringEntitlement = 2,
	}

#if NET
	[NoMac]
#else
	[Mac (11, 0)]
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
	[NoMac]
#else
	[Mac (11, 0)]
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
	[NoMac]
#else
	[Mac (11, 0)]
	[Obsoleted (PlatformName.MacOSX, 12, 1)]
#endif
	[MacCatalyst (13, 1)]
	[Native]
	public enum CXHandleType : long {
		Generic = 1,
		PhoneNumber = 2,
		EmailAddress = 3,
	}

	[NoMac, Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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

	[NoMac, NoWatch]
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

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak), NoWatch]
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

	[NoMac, NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface CXCallDirectoryExtensionContextDelegate {

		[Abstract]
		[Export ("requestFailedForExtensionContext:withError:")]
		void RequestFailed (CXCallDirectoryExtensionContext extensionContext, NSError error);
	}

	[NoMac, NoWatch]
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

		[NoWatch, NoTV, NoMac, iOS (13, 4), MacCatalyst (14, 0)]
		[Async]
		[Export ("openSettingsWithCompletionHandler:")]
		void OpenSettings ([NullAllowed] Action<NSError> completion);
	}

	[NoMac, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CXCallDirectoryProvider : NSExtensionRequestHandling {

	}

	interface ICXCallObserverDelegate { }

	[NoMac, Watch (9, 0)]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CXCallObserverDelegate {

		[Abstract]
		[Export ("callObserver:callChanged:")]
		void CallChanged (CXCallObserver callObserver, CXCall call);
	}

	[NoMac, Watch (9, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CXCallObserver {

		[Export ("calls", ArgumentSemantic.Copy)]
		CXCall [] Calls { get; }

		[Export ("setDelegate:queue:")]
		void SetDelegate ([NullAllowed] ICXCallObserverDelegate aDelegate, [NullAllowed] DispatchQueue queue);
	}

	[NoMac, Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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

	interface ICXProviderDelegate { }

	[Protocol, Model]
	[NoMac, Watch (9, 0)]
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

	[NoMac]
	[Watch (9, 0)]
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

		[NoWatch, NoTV, NoMac, iOS (14, 5)]
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

	[Mac (11, 0), Watch (9, 0)]
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

	[NoMac]
	[Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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

	[NoMac, Watch (9, 0)]
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
