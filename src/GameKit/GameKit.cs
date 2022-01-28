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

#if !MONOMAC

	// NSUInteger -> GKPeerPickerController.h
#if NET
	[UnsupportedOSPlatform ("ios7.0")]
#if IOS
	[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoTV] // preserve binary compatibility with existing/shipping code
	[NoWatch]
	[Deprecated (PlatformName.iOS, 7, 0)]
#endif
	[Native]
	public enum GKPeerPickerConnectionType : ulong {
		Online = 1 << 0,
		Nearby = 1 << 1
	}

	// untyped enum -> GKPublicConstants.h
#if NET
	[UnsupportedOSPlatform ("ios7.0")]
#if IOS
	[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 7, 0)]
#endif
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
#if NET
	[UnsupportedOSPlatform ("macos10.10")]
	[UnsupportedOSPlatform ("ios7.0")]
#if MONOMAC
	[Obsolete ("Starting with macos10.10.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
#endif
	public enum GKSendDataMode {
		Reliable,
		Unreliable,
	} 

	// untyped enum -> GKPublicConstants.h
#if NET
	[UnsupportedOSPlatform ("macos10.10")]
	[UnsupportedOSPlatform ("ios7.0")]
#if MONOMAC
	[Obsolete ("Starting with macos10.10.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
#endif
	public enum GKSessionMode {
	    Server, 
	    Client,
	    Peer,
	}

	// untyped enum -> GKPublicConstants.h
#if NET
	[UnsupportedOSPlatform ("macos10.10")]
	[UnsupportedOSPlatform ("ios7.0")]
#if MONOMAC
	[Obsolete ("Starting with macos10.10.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
#endif
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
#if MONOMAC && !XAMCORE_4_0
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

		FriendListDescriptionMissing = 100,
		FriendListRestricted = 101,
		FriendListDenied = 102,
		FriendRequestNotAvailable = 103,
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("tvos10.0")]
#else
	[iOS (10,0)]
	[Mac (10,12)]
	[TV (10,0)]
#endif
	[Native]
	public enum GKConnectionState : long {
		NotConnected,
		Connected,
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("tvos10.0")]
#else
	[iOS (10,0)]
	[Mac (10,12)]
	[TV (10,0)]
#endif
	[Native]
	public enum GKTransportType : long {
		Unreliable,
		Reliable,
	}

#if NET
	[UnsupportedOSPlatform ("macos10.14")]
	[UnsupportedOSPlatform ("tvos12.0")]
	[UnsupportedOSPlatform ("ios12.0")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
	[Obsolete ("Starting with tvos12.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios12.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.MacOSX, 10,14)]
	[Deprecated (PlatformName.TvOS, 12,0)]
	[Deprecated (PlatformName.iOS, 12,0)]
	[Unavailable (PlatformName.WatchOS)]
#endif
	[Native]
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
#if NET
	[UnsupportedOSPlatform ("macos10.10")]
	[UnsupportedOSPlatform ("ios7.0")]
#if MONOMAC
	[Obsolete ("Starting with macos10.10.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 7, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
#endif
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
#if NET
	[SupportedOSPlatform ("macos10.9")]
#else
	[Mac (10,9)]
#endif
	[Native]
	public enum GKChallengeState : long	{
		Invalid = 0,
		Pending,
		Completed,
		Declined
	}

	// NSInteger -> GKGameCenterViewController.h
#if !NET
	[NoWatch]
#endif
	[Native]
	public enum GKGameCenterViewControllerState : long {
		Default = -1,
		Leaderboards ,
		Achievements,
		Challenges,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
#else
		[iOS (14,0)]
		[TV (14,0)]
#endif
		LocalPlayerProfile = 3,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
#else
		[iOS (14,0)]
		[TV (14,0)]
#endif
		Dashboard = 4,
#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
		[SupportedOSPlatform ("tvos15.0")]
#else
		[iOS (15,0)]
		[Mac (12,0)]
		[MacCatalyst (15,0)]
		[TV (15,0)]
		[NoWatch]
#endif
		LocalPlayerFriendsList = 5,
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
#if NET
	[SupportedOSPlatform ("ios7.0")]
#else
	[iOS (7,0)]
#endif
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

#if NET
	[SupportedOSPlatform ("ios11.3")]
	[SupportedOSPlatform ("macos10.13.4")]
	[SupportedOSPlatform ("tvos11.3")]
	[UnsupportedOSPlatform ("macos11.0")]
	[UnsupportedOSPlatform ("tvos14.0")]
	[UnsupportedOSPlatform ("ios14.0")]
#if MONOMAC
	[Obsolete ("Starting with macos11.0 do not use; this API was removed.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
	[Obsolete ("Starting with tvos14.0 do not use; this API was removed.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios14.0 do not use; this API was removed.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[iOS (11,3)]
	[Deprecated (PlatformName.iOS, 14,0, message: "Do not use; this API was removed.")]
	[Mac (10,13,4)]
	[Deprecated (PlatformName.MacOSX, 11,0, message: "Do not use; this API was removed.")]
	[TV (11,3)]
	[Deprecated (PlatformName.TvOS, 14,0, message: "Do not use; this API was removed.")]
	[NoMacCatalyst]
#endif
	[Native]
	public enum GKAuthenticationType : ulong {
		WithoutUI = 0,
		GreenBuddyUI = 1,
		AuthKitInvocation = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[TV (14,0)]
	[Mac (11,0)]
	[iOS (14,0)]
	[NoWatch]
#endif
	[Native]
	public enum GKAccessPointLocation : long
	{
		TopLeading,
		TopTrailing,
		BottomLeading,
		BottomTrailing,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[TV (14,0)]
	[Mac (11,0)]
	[iOS (14,0)]
	[Watch (7,0)]
#endif
	[Native]
	public enum GKLeaderboardType : long
	{
		Classic,
		Recurring,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[TV (14,0)]
	[Mac (11,0)]
	[iOS (14,0)]
	[NoWatch]
#endif
	[Native]
	public enum GKMatchmakingMode : long
	{
		Default = 0,
		NearbyOnly = 1,
		AutomatchOnly = 2,
#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[TV (15,0)]
		[Mac (12,0)]
		[iOS (15,0)]
		[MacCatalyst (15,0)]
#endif
		InviteOnly = 3,
	}

#if NET
	[SupportedOSPlatform ("tvos14.5")]
	[SupportedOSPlatform ("macos11.3")]
	[SupportedOSPlatform ("ios14.5")]
#else
	[Watch (7,4)]
	[TV (14,5)]
	[Mac (11,3)]
	[iOS (14,5)]
#endif
	[Native]
	public enum GKFriendsAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
	}
}
