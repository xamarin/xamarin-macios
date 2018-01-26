//
// GameKit.cs: This file describes the API that the generator will produce for GameKit
//
// Authors:
//   Miguel de Icaza
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2009, Novell, Inc.
// Copyright 2012-2014 Xamarin Inc. All rights reserved
//

using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreFoundation;

namespace XamCore.GameKit {

#if !MONOMAC

	// NSUInteger -> GKPeerPickerController.h
#if XAMCORE_4_0
	[NoTV] // preserve binary compatibility with existing/shipping code
#endif
	[NoWatch]
	[Native]
	[Availability (Introduced = Platform.iOS_3_0, Deprecated = Platform.iOS_7_0)]
	public enum GKPeerPickerConnectionType : nuint_compat_int {
		Online = 1 << 0,
		Nearby = 1 << 1
	}

	// untyped enum -> GKPublicConstants.h
	[Availability (Introduced = Platform.iOS_3_0, Deprecated = Platform.iOS_7_0)]
	[ErrorDomain ("GKVoiceChatServiceErrorDomain")]
	public enum GKVoiceChatServiceError {
		Internal = 32000,	
		NoRemotePackets = 32001,
		UnableToConnect = 32002,
		RemoteParticipantHangup = 32003,
		InvalidCallID = 32004,
		AudioUnavailable = 32005,
		UninitializedClient = 32006,
		ClientMissingRequiredMethods = 32007,
		RemoteParticipantBusy = 32008,
		RemoteParticipantCancelled = 32009,
		RemoteParticipantResponseInvalid = 32010,
		RemoteParticipantDeclinedInvite = 32011,
		MethodCurrentlyInvalid = 32012,
		NetworkConfiguration = 32013,
		UnsupportedRemoteVersion = 32014,
		OutOfMemory = 32015,
		InvalidParameter = 32016
	}
#endif

	// untyped enum -> GKPublicConstants.h
	[Availability (Introduced = Platform.iOS_3_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
	public enum GKSendDataMode {
		Reliable,
		Unreliable,
	} 

	// untyped enum -> GKPublicConstants.h
	[Availability (Introduced = Platform.iOS_3_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
	public enum GKSessionMode {
	    Server, 
	    Client,
	    Peer,
	}

	// untyped enum -> GKPublicConstants.h
	[Availability (Introduced = Platform.iOS_3_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
	public enum GKPeerConnectionState {
		Available,
		Unavailable,
		Connected,   
		Disconnected,
		Connecting,  
	}

	// NSInteger -> GKLeaderboard.h
	[iOS (4,0)]
	[Native]
	public enum GKLeaderboardTimeScope : nint {
		Today, Week, AllTime
	}

	// NSInteger -> GKLeaderboard.h
	[iOS (4,0)]
	[Native]
	public enum GKLeaderboardPlayerScope : nint {
		Global, FriendsOnly
	}

	// NSInteger -> GKError.h
	[iOS (4,0)]
	[Native]
	[ErrorDomain ("GKErrorDomain")]
	public enum GKError : nint {
		None = 0,
		Unknown = 1,
		Cancelled,
		CommunicationsFailure,
		UserDenied,
		InvalidCredentials,
		NotAuthenticated,
		AuthenticationInProgress,
		InvalidPlayer,
		ScoreNotSet,
		ParentalControlsBlocked,
#if !MONOMAC
		PlayerStatusExceedsMaximumLength,
		PlayerStatusInvalid,
#endif
		MatchRequestInvalid = 13,
		Underage,
		GameUnrecognized,
		NotSupported,
		InvalidParameter,
		UnexpectedConnection,
		ChallengeInvalid = 19,
		TurnBasedMatchDataTooLarge,
		TurnBasedTooManySessions,
		TurnBasedInvalidParticipant,
		TurnBasedInvalidTurn,
		TurnBasedInvalidState,
#if MONOMAC
		Offline = 25,
#else
		InvitationsDisabled = 25, // iOS 7.0
		PlayerPhotoFailure = 26,  // iOS 8.0
		UbiquityContainerUnavailable = 27, // iOS 8.0
#endif
		MatchNotConnected = 28,
		GameSessionRequestInvalid = 29,
	}

	[Native]
	[iOS (10,0)][Mac (10,12)][TV (10,0)]
	public enum GKConnectionState : nint {
		NotConnected,
		Connected,
	}

	[Native]
	[iOS (10,0)][Mac (10,12)][TV (10,0)]
	public enum GKTransportType : nint {
		Unreliable,
		Reliable,
	}

	[Native]
	[ErrorDomain ("GKGameSessionErrorDomain")]
	public enum GKGameSessionErrorCode : nint {
		Unknown = 1,
		NotAuthenticated = 2,
		SessionConflict = 3,
		SessionNotShared = 4,
		ConnectionCancelledByUser = 5,
		ConnectionFailed = 6,
		SessionHasMaxConnectedPlayers = 7,
		SendDataNotConnected = 8,
		SendDataNoRecipients = 9,
		SendDataNotReachable = 10,
		SendRateLimitReached = 11,
		BadContainer = 12,
		CloudQuotaExceeded = 13,
		NetworkFailure = 14,
		CloudDriveDisabled = 15,
		InvalidSession = 16,
	}

	// NSInteger -> GKMatch.h
	[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
	[Native]
	public enum GKMatchSendDataMode : nint {
		Reliable, Unreliable
	}

	// NSInteger -> GKMatch.h
	[iOS (4,0)]
	[Native]
	public enum GKPlayerConnectionState : nint {
		Unknown, Connected, Disconnected
	}

	// NSInteger -> GKVoiceChat.h
	[iOS (4,0)]
	[Native]
	public enum GKVoiceChatPlayerState : nint {
		Connected,
		Disconnected,
		Speaking,
		Silent,
		Connecting
	}

	// NSInteger -> GKPlayer.h
	[iOS (5,0)]
	[Native]
	public enum GKPhotoSize : nint {
		Small, Normal
	}

	// NSInteger -> GKTurnBasedMatch.h
	[iOS (5,0)]
	[Native]
	public enum GKTurnBasedMatchStatus : nint {
		Unknown, Open, Ended, Matching
	}

	// NSInteger -> GKTurnBasedMatch.h
	[iOS (5,0)]
	[Native]
	public enum GKTurnBasedParticipantStatus : nint {
		Unknown, Invited, Declined, Matching, Active, Done
	}

	// NSInteger -> GKTurnBasedMatch.h
	[iOS (5,0)]
	[Native]
	public enum GKTurnBasedMatchOutcome : nint {
		None, Quit, Won, Lost, Tied, TimeExpired,
		First, Second, Third, Fourth, CustomRange = 0xff0000
	}

	// NSInteger -> GKChallenge.h
	[iOS (6,0)][Mac (10,9)]
	[Native]
	public enum GKChallengeState : nint	{
		Invalid = 0,
		Pending,
		Completed,
		Declined
	}

	// NSInteger -> GKGameCenterViewController.h
	[NoWatch]
	[Native]
	public enum GKGameCenterViewControllerState : nint {
		Default = -1,
		Leaderboards ,
		Achievements,
		Challenges,
	}

	// NSInteger -> GKMatchmaker.h
	[Native]
	public enum GKInviteeResponse : nint
	{
		Accepted           = 0,
		Declined           = 1,
		Failed             = 2,
		Incompatible       = 3,
		UnableToConnect    = 4,
 		NoAnswer           = 5,
	}

	// NSUInteger -> GKMatchmaker.h
	[Native]
	public enum GKMatchType : nuint_compat_int
	{
		PeerToPeer,
		Hosted,
		TurnBased
	}

	// uint8_t -> GKTurnBasedMatch.h
	[iOS (7,0)]
	public enum GKTurnBasedExchangeStatus : sbyte
	{
		Unknown,
		Active,
		Complete,
		Resolved,
		Canceled
	}

	[Native]
	public enum GKInviteRecipientResponse : nint {
		Accepted = 0,
		Declined = 1,
		Failed = 2,
		Incompatible = 3,
		UnableToConnect = 4,
		NoAnswer = 5,
	}

	[Native]
	public enum GKAuthenticationType : nuint {
		WithoutUI = 0,
		GreenBuddyUI = 1,
		AuthKitInvocation = 2,
	}
}
