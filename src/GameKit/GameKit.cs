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
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace GameKit {

#if !MONOMAC

	// NSUInteger -> GKPeerPickerController.h
#if XAMCORE_4_0
	[NoTV] // preserve binary compatibility with existing/shipping code
#endif
	[NoWatch]
	[Native]
	[Deprecated (PlatformName.iOS, 7, 0)]
	public enum GKPeerPickerConnectionType : ulong {
		Online = 1 << 0,
		Nearby = 1 << 1
	}

	// untyped enum -> GKPublicConstants.h
	[Deprecated (PlatformName.iOS, 7, 0)]
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
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	public enum GKSendDataMode {
		Reliable,
		Unreliable,
	} 

	// untyped enum -> GKPublicConstants.h
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	public enum GKSessionMode {
	    Server, 
	    Client,
	    Peer,
	}

	// untyped enum -> GKPublicConstants.h
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	public enum GKPeerConnectionState {
		Available,
		Unavailable,
		Connected,   
		Disconnected,
		Connecting,  
	}

	// NSInteger -> GKLeaderboard.h
	[Native]
	public enum GKLeaderboardTimeScope : long {
		Today, Week, AllTime
	}

	// NSInteger -> GKLeaderboard.h
	[Native]
	public enum GKLeaderboardPlayerScope : long {
		Global, FriendsOnly
	}

	// NSInteger -> GKError.h
	[Native]
	[ErrorDomain ("GKErrorDomain")]
	public enum GKError : long {
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
	public enum GKConnectionState : long {
		NotConnected,
		Connected,
	}

	[Native]
	[iOS (10,0)][Mac (10,12)][TV (10,0)]
	public enum GKTransportType : long {
		Unreliable,
		Reliable,
	}

	[Native]
	[ErrorDomain ("GKGameSessionErrorDomain")]
	public enum GKGameSessionErrorCode : long {
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
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	[Native]
	public enum GKMatchSendDataMode : long {
		Reliable, Unreliable
	}

	// NSInteger -> GKMatch.h
	[Native]
	public enum GKPlayerConnectionState : long {
		Unknown, Connected, Disconnected
	}

	// NSInteger -> GKVoiceChat.h
	[Native]
	public enum GKVoiceChatPlayerState : long {
		Connected,
		Disconnected,
		Speaking,
		Silent,
		Connecting
	}

	// NSInteger -> GKPlayer.h
	[Native]
	public enum GKPhotoSize : long {
		Small, Normal
	}

	// NSInteger -> GKTurnBasedMatch.h
	[Native]
	public enum GKTurnBasedMatchStatus : long {
		Unknown, Open, Ended, Matching
	}

	// NSInteger -> GKTurnBasedMatch.h
	[Native]
	public enum GKTurnBasedParticipantStatus : long {
		Unknown, Invited, Declined, Matching, Active, Done
	}

	// NSInteger -> GKTurnBasedMatch.h
	[Native]
	public enum GKTurnBasedMatchOutcome : long {
		None, Quit, Won, Lost, Tied, TimeExpired,
		First, Second, Third, Fourth, CustomRange = 0xff0000
	}

	// NSInteger -> GKChallenge.h
	[iOS (6,0)][Mac (10,9)]
	[Native]
	public enum GKChallengeState : long	{
		Invalid = 0,
		Pending,
		Completed,
		Declined
	}

	// NSInteger -> GKGameCenterViewController.h
	[NoWatch]
	[Native]
	public enum GKGameCenterViewControllerState : long {
		Default = -1,
		Leaderboards ,
		Achievements,
		Challenges,
	}

	// NSInteger -> GKMatchmaker.h
	[Native]
	public enum GKInviteeResponse : long
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
	public enum GKMatchType : ulong
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
	public enum GKInviteRecipientResponse : long {
		Accepted = 0,
		Declined = 1,
		Failed = 2,
		Incompatible = 3,
		UnableToConnect = 4,
		NoAnswer = 5,
	}

	[Mac (10,13,4), TV (11,3), iOS (11,3)]
	[Native]
	public enum GKAuthenticationType : ulong {
		WithoutUI = 0,
		GreenBuddyUI = 1,
		AuthKitInvocation = 2,
	}
}
