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

#nullable enable

namespace GameKit {

	// NSUInteger -> GKPeerPickerController.h
	[NoMac]
	[NoWatch]
#if NET
	[NoTV]
#endif
	[Deprecated (PlatformName.iOS, 7, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Native]
	public enum GKPeerPickerConnectionType : ulong {
		Online = 1 << 0,
		Nearby = 1 << 1
	}

	// untyped enum -> GKPublicConstants.h
	[NoMac]
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.TvOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
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

	// untyped enum -> GKPublicConstants.h
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.TvOS, 9, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	public enum GKSendDataMode {
		Reliable,
		Unreliable,
	}

	// untyped enum -> GKPublicConstants.h
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.TvOS, 9, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	public enum GKSessionMode {
		Server,
		Client,
		Peer,
	}

	// untyped enum -> GKPublicConstants.h
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.TvOS, 9, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	public enum GKPeerConnectionState {
		Available,
		Unavailable,
		Connected,
		Disconnected,
		Connecting,
		ConnectedRelay = 5,
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
	[Native ("GKErrorCode")]
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
		PlayerStatusExceedsMaximumLength,
		PlayerStatusInvalid,
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
#if MONOMAC && !NET
		[Obsolete ("This value was re-used on macOS only and removed later.")]
		Offline = 25,
#endif
		InvitationsDisabled = 25, // iOS 7.0
		PlayerPhotoFailure = 26,  // iOS 8.0
		UbiquityContainerUnavailable = 27, // iOS 8.0
		MatchNotConnected = 28,
		GameSessionRequestInvalid = 29,
		RestrictedToAutomatch = 30,
		ApiNotAvailable = 31,
		NotAuthorized = 32,
		ConnectionTimeout = 33,
		ApiObsolete = 34,
		ICloudUnavailable = 35,
		LockdownMode = 36,
		FriendListDescriptionMissing = 100,
		FriendListRestricted = 101,
		FriendListDenied = 102,
		FriendRequestNotAvailable = 103,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum GKConnectionState : long {
		NotConnected,
		Connected,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum GKTransportType : long {
		Unreliable,
		Reliable,
	}

	[Deprecated (PlatformName.MacOSX, 10, 14)]
	[Deprecated (PlatformName.TvOS, 12, 0)]
	[Deprecated (PlatformName.iOS, 12, 0)]
	[Native]
#if WATCH
	// removed in Xcode 10 but a breaking change (for us) to remove
	[Obsolete ("Not used in watchOS.")]
#else
	[Unavailable (PlatformName.WatchOS)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[ErrorDomain ("GKGameSessionErrorDomain")]
#endif
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
	[Deprecated (PlatformName.TvOS, 9, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
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
	[MacCatalyst (13, 1)]
	[Native]
	public enum GKChallengeState : long {
		Invalid = 0,
		Pending,
		Completed,
		Declined
	}

	// NSInteger -> GKGameCenterViewController.h
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum GKGameCenterViewControllerState : long {
		Default = -1,
		Leaderboards,
		Achievements,
		Challenges,
		[iOS (14, 0)]
		[TV (14, 0)]
		[MacCatalyst (14, 0)]
		LocalPlayerProfile = 3,
		[iOS (14, 0)]
		[TV (14, 0)]
		[MacCatalyst (14, 0)]
		Dashboard = 4,
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[TV (15, 0)]
		[NoWatch]
		LocalPlayerFriendsList = 5,
	}

	// NSInteger -> GKMatchmaker.h
	[Native]
	public enum GKInviteeResponse : long {
		Accepted = 0,
		Declined = 1,
		Failed = 2,
		Incompatible = 3,
		UnableToConnect = 4,
		NoAnswer = 5,
	}

	// NSUInteger -> GKMatchmaker.h
	[Native]
	public enum GKMatchType : ulong {
		PeerToPeer,
		Hosted,
		TurnBased
	}

	// uint8_t -> GKTurnBasedMatch.h
	public enum GKTurnBasedExchangeStatus : sbyte {
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

#if !NET
	[Deprecated (PlatformName.iOS, 14, 0, message: "Do not use; this API was removed.")]
	[Deprecated (PlatformName.MacOSX, 11, 0, message: "Do not use; this API was removed.")]
	[Deprecated (PlatformName.TvOS, 14, 0, message: "Do not use; this API was removed.")]
	[Native]
	public enum GKAuthenticationType : ulong {
		WithoutUI = 0,
		GreenBuddyUI = 1,
		AuthKitInvocation = 2,
	}
#endif

	[TV (14, 0)]
	[iOS (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[Native]
	public enum GKAccessPointLocation : long {
		TopLeading,
		TopTrailing,
		BottomLeading,
		BottomTrailing,
	}

	[TV (14, 0)]
	[iOS (14, 0)]
	[Watch (7, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum GKLeaderboardType : long {
		Classic,
		Recurring,
	}

	[TV (14, 0)]
	[iOS (14, 0)]
	[NoWatch]
	[MacCatalyst (14, 0)]
	[Native]
	public enum GKMatchmakingMode : long {
		Default = 0,
		NearbyOnly = 1,
		AutomatchOnly = 2,
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		InviteOnly = 3,
	}

	[Watch (7, 4)]
	[TV (14, 5)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Native]
	public enum GKFriendsAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
	}
}
