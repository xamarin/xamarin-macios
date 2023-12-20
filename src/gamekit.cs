//
// GameKit.cs: This file describes the API that the generator will produce for GameKit
//
// Authors:
//   Miguel de Icaza
//   Marek Safar (marek.safar@gmail.com)
//   Aaron Bockover (abock@xamarin.com)
//   Whitney Schmidt (whschm@microsoft.com)
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2013 Xamarin Inc. All rights reserved
// Copyright 2020 Microsoft Corp. All rights reserved
//

#pragma warning disable 618

using System;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;
#if MONOMAC
using AppKit;
using UIWindow = AppKit.NSWindow;
using UIViewController = AppKit.NSViewController;
using UIImage = AppKit.NSImage;
#else
using UIKit;
using NSViewController = Foundation.NSObject;
using NSWindow = Foundation.NSObject;
using NSResponder = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace GameKit {

	delegate void GKFriendsHandler (string [] friends, NSError error);
	delegate void GKPlayersHandler (GKPlayer [] players, NSError error);
	delegate void GKLeaderboardsHandler (GKLeaderboard [] leaderboards, NSError error);
	delegate void GKScoresLoadedHandler (GKScore [] scoreArray, NSError error);
	delegate void GKNotificationMatch (GKMatch match, NSError error);
	delegate void GKInviteHandler (GKInvite invite, string [] playerIDs);
	delegate void GKQueryHandler (nint activity, NSError error);
	delegate void GKCompletionHandler (GKAchievement [] achivements, NSError error);
	delegate void GKAchievementDescriptionHandler (GKAchievementDescription [] descriptions, NSError error);
	delegate void GKCategoryHandler (string [] categories, string [] titles, NSError error);
	delegate void GKPlayerStateUpdateHandler (string playerId, GKVoiceChatPlayerState state);
	delegate void GKIdentityVerificationSignatureHandler (NSUrl publicKeyUrl, NSData signature, NSData salt, ulong timestamp, NSError error);
	delegate void GKLeaderboardSetsHandler (GKLeaderboardSet [] leaderboardSets, NSError error);
	delegate void GKEntriesForPlayerScopeHandler (GKLeaderboardEntry localPlayerEntry, GKLeaderboardEntry [] entries, nint totalPlayerCount, NSError error);
	delegate void GKEntriesForPlayersHandler (GKLeaderboardEntry localPlayerEntry, GKLeaderboardEntry [] entries, NSError error);

#if MONOMAC
	delegate void GKImageLoadedHandler  (NSImage image, NSError error);
	delegate void GKPlayerPhotoLoaded (NSImage photo, NSError error);
	delegate void GKChallengeComposeHandler (NSViewController composeController, bool issuedChallenge, string [] sentPlayerIDs);
	delegate void GKChallengeComposeHandler2 (NSViewController composeController, bool issuedChallenge, GKPlayer[] sentPlayers);
#else
	delegate void GKImageLoadedHandler (UIImage image, NSError error);
	delegate void GKPlayerPhotoLoaded (UIImage photo, NSError error);
	delegate void GKChallengeComposeHandler (UIViewController composeController, bool issuedChallenge, string [] sentPlayerIDs);
	delegate void GKChallengeComposeHandler2 (UIViewController composeController, bool issuedChallenge, [NullAllowed] GKPlayer [] sentPlayers);
#endif

#if WATCH
	// hacks to let [NoWatch] work properly
	interface UIAppearance {}
	interface UIViewController {}
	interface UINavigationController {}
	interface UIWindow {}
#endif

	interface IGKVoiceChatClient { }

	[NoMac]
	[NoWatch] // only exposed thru GKVoiceChatService (not in 3.0)
	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKVoiceChat' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKVoiceChat' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKVoiceChatClient {
		[Abstract]
		[Export ("voiceChatService:sendData:toParticipantID:")]
		void SendData (GKVoiceChatService voiceChatService, NSData data, string toParticipant);

		[Export ("participantID")]
		[Abstract]
		string ParticipantID ();

		[Export ("voiceChatService:sendRealTimeData:toParticipantID:")]
		void SendRealTimeData (GKVoiceChatService voiceChatService, NSData data, string participantID);

		[Export ("voiceChatService:didStartWithParticipantID:")]
		void Started (GKVoiceChatService voiceChatService, string participantID);

		[Export ("voiceChatService:didNotStartWithParticipantID:error:")]
		void FailedToConnect (GKVoiceChatService voiceChatService, string participantID, [NullAllowed] NSError error);

		[Export ("voiceChatService:didStopWithParticipantID:error:")]
		void Stopped (GKVoiceChatService voiceChatService, string participantID, [NullAllowed] NSError error);

		[Export ("voiceChatService:didReceiveInvitationFromParticipantID:callID:")]
		void ReceivedInvitation (GKVoiceChatService voiceChatService, string participantID, nint callID);
	}

	[NoMac]
	[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
	[NoTV]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKVoiceChat' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKVoiceChat' instead.")]
	interface GKVoiceChatService {

		[Export ("defaultVoiceChatService")]
		[Static]
		GKVoiceChatService Default { get; }

		[NullAllowed] // by default this property is null
		[Export ("client", ArgumentSemantic.Assign)]
		IGKVoiceChatClient Client { get; set; }

		[Export ("startVoiceChatWithParticipantID:error:")]
		bool StartVoiceChat (string participantID, out NSError error);

		[Export ("stopVoiceChatWithParticipantID:")]
		void StopVoiceChat (string participantID);

		[Export ("acceptCallID:error:")]
		bool AcceptCall (nint callID, out NSError error);

		[Export ("denyCallID:")]
		void DenyCall (nint callId);

		[Export ("receivedRealTimeData:fromParticipantID:")]
		void ReceivedRealTimeData (NSData audio, string participantID);

		[Export ("receivedData:fromParticipantID:")]
		void ReceivedData (NSData arbitraryData, string participantID);

		[Export ("microphoneMuted")]
		bool MicrophoneMuted { [Bind ("isMicrophoneMuted")] get; set; }

		[Export ("remoteParticipantVolume")]
		float RemoteParticipantVolume { get; set; } /* float, not CGFloat */

		[Export ("outputMeteringEnabled")]
		bool OutputMeteringEnabled { [Bind ("isOutputMeteringEnabled")] get; set; }

		[Export ("inputMeteringEnabled")]
		bool InputMeteringEnabled { [Bind ("isInputMeteringEnabled")] get; set; }

		[Export ("outputMeterLevel")]
		float OutputMeterLevel { get; }  /* float, not CGFloat */

		[Export ("inputMeterLevel")]
		float InputMeterLevel { get; }  /* float, not CGFloat */

		[Static]
		[Export ("isVoIPAllowed")]
		bool IsVoIPAllowed { get; }
	}

	[NoTV]
	[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'MultipeerConnectivity.MCSession' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'MultipeerConnectivity.MCSession' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MultipeerConnectivity.MCSession' instead.")]
	interface GKSession {
		[Export ("initWithSessionID:displayName:sessionMode:")]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		NativeHandle Constructor ([NullAllowed] string sessionID, [NullAllowed] string displayName, GKSessionMode mode);

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IGKSessionDelegate Delegate { get; set; }

		[Export ("sessionID")]
		string SessionID { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("sessionMode")]
		GKSessionMode SessionMode { get; }

		[Export ("peerID")]
		string PeerID { get; }

		[Export ("available")]
		bool Available { [Bind ("isAvailable")] get; set; }

		[Export ("disconnectTimeout", ArgumentSemantic.Assign)]
		double DisconnectTimeout { get; set; }

		[Export ("displayNameForPeer:")]
		string DisplayNameForPeer (string peerID);

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("sendData:toPeers:withDataMode:error:")]
		bool SendData (NSData data, string [] peers, GKSendDataMode mode, out NSError error);

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("sendDataToAllPeers:withDataMode:error:")]
		bool SendDataToAllPeers (NSData data, GKSendDataMode mode, out NSError error);

		// // SEL = -receiveData:fromPeer:inSession:context:
		[Export ("setDataReceiveHandler:withContext:")]
		[Internal]
		void _SetDataReceiveHandler (NSObject obj, IntPtr context);

		[Export ("connectToPeer:withTimeout:")]
		void Connect (string peerID, double timeout);

		[Export ("cancelConnectToPeer:")]
		void CancelConnect (string peerID);

		[Export ("acceptConnectionFromPeer:error:")]
		bool AcceptConnection (string peerID, out NSError error);

		[Export ("denyConnectionFromPeer:")]
		void DenyConnection (string peerID);

		[Export ("disconnectPeerFromAllPeers:")]
		void DisconnectPeerFromAllPeers (string peerID);

		[Export ("disconnectFromAllPeers")]
		void DisconnectFromAllPeers ();

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("peersWithConnectionState:")]
		string [] PeersWithConnectionState (GKPeerConnectionState state);
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKLeaderboard {
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("timeScope", ArgumentSemantic.Assign)]
		GKLeaderboardTimeScope TimeScope { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("playerScope", ArgumentSemantic.Assign)]
		GKLeaderboardPlayerScope PlayerScope { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("maxRange", ArgumentSemantic.Assign)]
		nint MaxRange { get; }

		[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'Identifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'Identifier' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Identifier' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("category", ArgumentSemantic.Copy)]
		string Category { get; set; }

		[Export ("title", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Title { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("range", ArgumentSemantic.Assign)]
		NSRange Range { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("scores", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKScore [] Scores { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("localPlayerScore", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKScore LocalPlayerScore { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use '.ctor (GKPlayer [] players)' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use '.ctor (GKPlayer [] players)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use '.ctor (GKPlayer [] players)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use '.ctor (GKPlayer [] players)' instead.")]
		[Export ("initWithPlayerIDs:")]
		NativeHandle Constructor ([NullAllowed] string [] players);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("loadScoresWithCompletionHandler:")]
		[Async]
		void LoadScores ([NullAllowed] GKScoresLoadedHandler scoresLoadedHandler);

		[NoTV]
		[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'LoadLeaderboards' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LoadLeaderboards' instead.")]
		[Static]
		[Export ("loadCategoriesWithCompletionHandler:")]
		[Async (ResultTypeName = "GKCategoryResult")]
		void LoadCategories ([NullAllowed] GKCategoryHandler categoryHandler);

		[NoTV]
		[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
		[Static]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'SetDefaultLeaderboard' on 'GKLocalPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'SetDefaultLeaderboard' on 'GKLocalPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetDefaultLeaderboard' on 'GKLocalPlayer' instead.")]
		[Export ("setDefaultLeaderboard:withCompletionHandler:")]
		[Async]
		void SetDefaultLeaderboard ([NullAllowed] string leaderboardIdentifier, [NullAllowed] Action<NSError> notificationHandler);

		[Export ("groupIdentifier", ArgumentSemantic.Retain)]
		string GroupIdentifier { get; [NotImplemented] set; }

		[Static]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadLeaderBoards(string[] leaderboardIDs, GKLeaderboardsHandler completionHandler)' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadLeaderBoards(string[] leaderboardIDs, GKLeaderboardsHandler completionHandler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadLeaderBoards(string[] leaderboardIDs, GKLeaderboardsHandler completionHandler)' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadLeaderBoards(string[] leaderboardIDs, GKLeaderboardsHandler completionHandler)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadLeaderBoards(string[] leaderboardIDs, GKLeaderboardsHandler completionHandler)' instead.")]
		[Export ("loadLeaderboardsWithCompletionHandler:")]
		[Async]
		void LoadLeaderboards ([NullAllowed] Action<GKLeaderboard [], NSError> completionHandler);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadEntries' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[NullAllowed]
		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; set; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("loadImageWithCompletionHandler:")]
		[Async]
		void LoadImage ([NullAllowed] GKImageLoadedHandler completionHandler);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadEntries' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("initWithPlayers:")]
		NativeHandle Constructor (GKPlayer [] players);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadEntries' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("loading")]
		bool IsLoading { [Bind ("isLoading")] get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Async]
		[Export ("loadLeaderboardsWithIDs:completionHandler:")]
		void LoadLeaderboards ([NullAllowed] string [] leaderboardIds, GKLeaderboardsHandler completionHandler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("submitScore:context:player:leaderboardIDs:completionHandler:")]
		[Async]
		void SubmitScore (nint score, nuint context, GKPlayer player, string [] leaderboardIds, Action<NSError> completionHandler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("baseLeaderboardID", ArgumentSemantic.Strong)]
		string BaseLeaderboardId { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("duration")]
		double Duration { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async (ResultTypeName = "GKEntriesForPlayerScopeResult")]
		[Export ("loadEntriesForPlayerScope:timeScope:range:completionHandler:")]
		void LoadEntries (GKLeaderboardPlayerScope playerScope, GKLeaderboardTimeScope timeScope, NSRange range, GKEntriesForPlayerScopeHandler completionHandler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async (ResultTypeName = "GKEntriesForPlayersResult")]
		[Export ("loadEntriesForPlayers:timeScope:completionHandler:")]
		void LoadEntries (GKPlayer [] players, GKLeaderboardTimeScope timeScope, GKEntriesForPlayersHandler completionHandler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("loadPreviousOccurrenceWithCompletionHandler:")]
		[Async]
		void LoadPreviousOccurrence (GKLeaderboardsHandler completionHandler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("nextStartDate", ArgumentSemantic.Strong)]
		NSDate NextStartDate { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("submitScore:context:player:completionHandler:")]
		[Async]
		void SubmitScore (nint score, nuint context, GKPlayer player, Action<NSError> completionHandler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("type")]
		GKLeaderboardType Type { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKLeaderboardSet : NSCoding, NSSecureCoding {

		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; [NotImplemented] set; }

		[Export ("groupIdentifier", ArgumentSemantic.Retain)]
		string GroupIdentifier { get; [NotImplemented] set; }

		[NullAllowed] // by default this property is null
		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; set; }

		[Export ("loadLeaderboardSetsWithCompletionHandler:")]
		[Static]
		[Async]
		void LoadLeaderboardSets ([NullAllowed] GKLeaderboardSetsHandler completionHandler);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadLeaderboardsWithCompletionHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadLeaderboardsWithCompletionHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadLeaderboardsWithCompletionHandler' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'LoadLeaderboardsWithCompletionHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadLeaderboardsWithCompletionHandler' instead.")]
		[Export ("loadLeaderboardsWithCompletionHandler:")]
		[Async]
		void LoadLeaderboards ([NullAllowed] GKLeaderboardsHandler completionHandler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("loadLeaderboardsWithHandler:")]
		[Async]
		void LoadLeaderboardsWithCompletionHandler (GKLeaderboardsHandler handler);

		[NoMac]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Export ("loadImageWithCompletionHandler:")]
		[Async]
		void LoadImage ([NullAllowed] GKImageLoadedHandler completionHandler);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKBasePlayer {
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the GKPlayer.TeamPlayerId property to identify a player instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the GKPlayer.TeamPlayerId property to identify a player instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use the GKPlayer.TeamPlayerId property to identify a player instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the GKPlayer.TeamPlayerId property to identify a player instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the GKPlayer.TeamPlayerId property to identify a player instead.")]
		[NullAllowed, Export ("playerID", ArgumentSemantic.Retain)]
		string PlayerID { get; }

		[NullAllowed, Export ("displayName")]
		string DisplayName { get; }
	}

	[NoWatch]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GKPlayer' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GKPlayer' instead.")]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GKPlayer' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKPlayer' instead.")]
	[BaseType (typeof (GKBasePlayer))]
	interface GKCloudPlayer {
		[Static]
		[Export ("getCurrentSignedInPlayerForContainer:completionHandler:")]
		void GetCurrentSignedInPlayer ([NullAllowed] string containerName, Action<GKCloudPlayer, NSError> handler);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKBasePlayer))]
	// note: NSSecureCoding conformity is undocumented - but since it's a runtime check (on ObjC) we still need it
	interface GKPlayer : NSSecureCoding {

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TeamPlayerId' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'TeamPlayerId' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'TeamPlayerId' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'TeamPlayerId' instead.")]
		[Export ("playerID", ArgumentSemantic.Retain)]
		string PlayerID { get; }

		[Export ("alias", ArgumentSemantic.Copy)]
		string Alias { get; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'GKLocalPlayer.LoadFriendPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKLocalPlayer.LoadFriendPlayers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKLocalPlayer.LoadFriendPlayers' instead.")]
		[Export ("isFriend")]
		bool IsFriend { get; }

		[Static, Export ("loadPlayersForIdentifiers:withCompletionHandler:")]
		[Async]
		void LoadPlayersForIdentifiers (string [] identifiers, [NullAllowed] GKPlayersHandler completionHandler);

		[Field ("GKPlayerDidChangeNotificationName")]
		[Notification]
		// This name looks wrong, see the "Notification" at the end.
		NSString DidChangeNotificationNameNotification { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("loadPhotoForSize:withCompletionHandler:")]
		[Async]
		void LoadPhoto (GKPhotoSize size, [NullAllowed] GKPlayerPhotoLoaded onCompleted);

		[Export ("displayName")]
		string DisplayName { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("anonymousGuestPlayerWithIdentifier:")]
		GKPlayer GetAnonymousGuestPlayer (string guestIdentifier);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("guestIdentifier")]
		[NullAllowed]
		string GuestIdentifier { get; }

		[NoWatch]
		[TV (12, 4)]
		[iOS (12, 4)]
		[MacCatalyst (13, 1)]
		[Export ("gamePlayerID", ArgumentSemantic.Retain)]
		string GamePlayerId { get; }

		[NoWatch]
		[TV (12, 4)]
		[iOS (12, 4)]
		[MacCatalyst (13, 1)]
		[Export ("teamPlayerID", ArgumentSemantic.Retain)]
		string TeamPlayerId { get; }

		[NoWatch]
		[TV (13, 0)]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("scopedIDsArePersistent")]
		bool ScopedIdsArePersistent { get; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("GKPlayerIDNoLongerAvailable")]
		NSString IdNoLongerAvailable { get; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("isInvitable")]
		bool IsInvitable { get; }
	}

	[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'GKLeaderboardEntry' instead.")]
	[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'GKLeaderboardEntry' instead.")]
	[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'GKLeaderboardEntry' instead.")]
	[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'GKLeaderboardEntry' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'GKLeaderboardEntry' instead.")]
	[BaseType (typeof (NSObject))]
	interface GKScore : NSSecureCoding {
		[NoWatch]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'InitWithLeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'InitWithLeaderboardIdentifier' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'InitWithLeaderboardIdentifier' instead.")]
		[Internal]
		[Export ("initWithCategory:")]
		IntPtr InitWithCategory ([NullAllowed] string category);

		[MacCatalyst (13, 1)]
		[Export ("initWithLeaderboardIdentifier:player:")]
		NativeHandle Constructor (string identifier, GKPlayer player);

		[NoWatch]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use the overload that takes a 'GKPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that takes a 'GKPlayer' instead.")]
		[Export ("initWithLeaderboardIdentifier:forPlayer:")]
		NativeHandle Constructor (string identifier, string playerID);

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("initWithLeaderboardIdentifier:")]
		IntPtr InitWithLeaderboardIdentifier (string identifier);

		[NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("player", ArgumentSemantic.Retain)]
		GKPlayer Player { get; }

		[Export ("rank", ArgumentSemantic.Assign)]
		nint Rank { get; }

		[Export ("date", ArgumentSemantic.Retain)]
		NSDate Date { get; }

		[Export ("value", ArgumentSemantic.Assign)]
		long Value { get; set; }

		[Export ("formattedValue", ArgumentSemantic.Copy)]
		[NullAllowed]
		string FormattedValue { get; }

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'LeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'LeaderboardIdentifier' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LeaderboardIdentifier' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("category", ArgumentSemantic.Copy)]
		string Category { get; set; }

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'ReportScores' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'ReportScores' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ReportScores' instead.")]
		[Export ("reportScoreWithCompletionHandler:")]
		[Async]
		void ReportScore ([NullAllowed] Action<NSError> errorHandler);

		[Export ("context", ArgumentSemantic.Assign)]
		ulong Context { get; set; }

		[Export ("shouldSetDefaultLeaderboard", ArgumentSemantic.Assign)]
		bool ShouldSetDefaultLeaderboard { get; set; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ... )' and present the view controller instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ... )' and present the view controller instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ... )' and present the view controller instead.")]
		[Export ("issueChallengeToPlayers:message:")]
		void IssueChallengeToPlayers ([NullAllowed] string [] playerIDs, [NullAllowed] string message);

		[Export ("reportScores:withCompletionHandler:"), Static]
		[Async]
		void ReportScores (GKScore [] scores, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("leaderboardIdentifier", ArgumentSemantic.Copy)]
		string LeaderboardIdentifier { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("reportScores:withEligibleChallenges:withCompletionHandler:"), Static]
		[Async]
		void ReportScores (GKScore [] scores, GKChallenge [] challenges, [NullAllowed] Action<NSError> completionHandler);

		[NoWatch]
		[iOS (14, 0)]
		[Mac (11, 0)]
		[TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Async]
		[Export ("reportLeaderboardScores:withEligibleChallenges:withCompletionHandler:")]
		void ReportLeaderboardScores (GKLeaderboardScore [] scores, GKChallenge [] eligibleChallenges, [NullAllowed] Action<NSError> completionHandler);

		[NoMac]
		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ...)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ...)' instead.")]
		[Export ("challengeComposeControllerWithPlayers:message:completionHandler:")]
		[return: NullAllowed]
		UIViewController ChallengeComposeController ([NullAllowed] string [] playerIDs, [NullAllowed] string message, [NullAllowed] GKChallengeComposeHandler completionHandler);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "GKChallengeComposeResult")]
		[Export ("challengeComposeControllerWithMessage:players:completionHandler:")]
		UIViewController ChallengeComposeController ([NullAllowed] string message, [NullAllowed] GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler completionHandler);
	}

	interface IGKLeaderboardViewControllerDelegate { }

	[NoWatch]
	[NoTV]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKGameCenterViewController' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKLeaderboardViewControllerDelegate {
		[Abstract]
		[Export ("leaderboardViewControllerDidFinish:")]
		void DidFinish (GKLeaderboardViewController viewController);
	}

	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKGameCenterViewController' instead.")]
#if MONOMAC
	[BaseType (typeof (GKGameCenterViewController), Events=new Type [] { typeof (GKLeaderboardViewControllerDelegate)}, Delegates=new string [] {"WeakDelegate"})]
	interface GKLeaderboardViewController 
#else
	[BaseType (typeof (GKGameCenterViewController), Events = new Type [] { typeof (GKLeaderboardViewControllerDelegate) }, Delegates = new string [] { "WeakDelegate" })]
	interface GKLeaderboardViewController : UIAppearance
#endif
	{
		[Export ("leaderboardDelegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IGKLeaderboardViewControllerDelegate Delegate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("category",
			// Way to go, Apple
#if MONOMAC
			ArgumentSemantic.Copy
#else
			ArgumentSemantic.Copy //iOS 8 this changed to Copy for iOS
#endif
		)]
		string Category { get; set; }

		[Export ("timeScope", ArgumentSemantic.Assign)]
		GKLeaderboardTimeScope TimeScope { get; set; }
	}

	[Watch (6, 2), TV (13, 4), iOS (13, 4)]
	[MacCatalyst (13, 1)]
	delegate void GKFetchItemsForIdentityVerificationSignatureCompletionHandler (NSUrl publicKeyUrl, NSData signature, NSData salt, ulong timestamp, NSError error);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKPlayer))]
	interface GKLocalPlayer
#if !TVOS && !WATCH // from GKSavedGame category
		: GKSavedGameListener
#endif
	{
		[Export ("authenticated")]
		bool Authenticated { [Bind ("isAuthenticated")] get; }

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'LoadFriendPlayers' instead and collect the friends from the invoked callback.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'LoadFriendPlayers' instead and collect the friends from the invoked callback.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LoadFriendPlayers' instead and collect the friends from the invoked callback.")]
		[Export ("friends", ArgumentSemantic.Retain)]
		[NullAllowed]
		string [] Friends { get; }

		[Static, Export ("localPlayer")]
		GKLocalPlayer LocalPlayer { get; }

		[Export ("isUnderage")]
		bool IsUnderage { get; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Set the 'AuthenticationHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Set the 'AuthenticationHandler' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Set the 'AuthenticationHandler' instead.")]
		[Export ("authenticateWithCompletionHandler:")]
		[Async]
		void Authenticate ([NullAllowed] Action<NSError> handler);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("loadRecentPlayersWithCompletionHandler:")]
		void LoadRecentPlayers ([NullAllowed] Action<GKPlayer [], NSError> completionHandler);

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'LoadRecentPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'LoadRecentPlayers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LoadRecentPlayers' instead.")]
		[Export ("loadFriendsWithCompletionHandler:")]
		[Async]
		void LoadFriends ([NullAllowed] GKFriendsHandler handler);

		[Field ("GKPlayerAuthenticationDidChangeNotificationName")]
		[Notification]
		NSString AuthenticationDidChangeNotificationName { get; }

		[NullAllowed] // by default this property is null
		[Export ("authenticateHandler", ArgumentSemantic.Copy)]
		[MacCatalyst (13, 1)]
#if WATCH
		Action<NSError> AuthenticateHandler { get; set; }
#elif !MONOMAC
		Action<UIViewController, NSError> AuthenticateHandler { get; set; }
#else
		Action<NSViewController, NSError> AuthenticateHandler { get; set; }
#endif

		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0)]
		[NoMacCatalyst]
		[Export ("isPresentingFriendRequestViewController")]
		bool IsPresentingFriendRequestViewController { get; }

		[NoWatch, NoTV, NoMac, iOS (15, 0), NoMacCatalyst]
		[Export ("presentFriendRequestCreatorFromViewController:error:")]
		bool PresentFriendRequestCreator (UIViewController viewController, [NullAllowed] out NSError error);

		[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
		[Export ("presentFriendRequestCreatorFromWindow:error:")]
		bool PresentFriendRequestCreator ([NullAllowed] NSWindow window, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("loadDefaultLeaderboardIdentifierWithCompletionHandler:")]
		[Async]
		void LoadDefaultLeaderboardIdentifier ([NullAllowed] Action<string, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("setDefaultLeaderboardIdentifier:completionHandler:")]
		[Async]
		void SetDefaultLeaderboardIdentifier (string leaderboardIdentifier, [NullAllowed] Action<NSError> completionHandler);

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'LoadDefaultLeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'LoadDefaultLeaderboardIdentifier' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LoadDefaultLeaderboardIdentifier' instead.")]
		[Export ("loadDefaultLeaderboardCategoryIDWithCompletionHandler:")]
		[Async]
		void LoadDefaultLeaderboardCategoryID ([NullAllowed] Action<string, NSError> completionHandler);

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'SetDefaultLeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'SetDefaultLeaderboardIdentifier' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetDefaultLeaderboardIdentifier' instead.")]
		[Export ("setDefaultLeaderboardCategoryID:completionHandler:")]
		[Async]
		void SetDefaultLeaderboardCategoryID ([NullAllowed] string categoryID, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("registerListener:")]
		void RegisterListener (IGKLocalPlayerListener listener);

		[MacCatalyst (13, 1)]
		[Export ("unregisterListener:")]
		void UnregisterListener (IGKLocalPlayerListener listener);

		[MacCatalyst (13, 1)]
		[Export ("unregisterAllListeners")]
		void UnregisterAllListeners ();

		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'FetchItemsForIdentityVerificationSignature' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 4, message: "Use 'FetchItemsForIdentityVerificationSignature' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, 4, message: "Use 'FetchItemsForIdentityVerificationSignature' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'FetchItemsForIdentityVerificationSignature' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FetchItemsForIdentityVerificationSignature' instead.")]
		[Async (ResultTypeName = "GKIdentityVerificationSignatureResult")]
		[Export ("generateIdentityVerificationSignatureWithCompletionHandler:")]
		void GenerateIdentityVerificationSignature ([NullAllowed] GKIdentityVerificationSignatureHandler completionHandler);

		[Watch (6, 2), TV (13, 4), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "GKFetchItemsForIdentityVerificationSignature")]
		[Export ("fetchItemsForIdentityVerificationSignature:")]
		void FetchItemsForIdentityVerificationSignature ([NullAllowed] GKFetchItemsForIdentityVerificationSignatureCompletionHandler completionHandler);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Async]
		[Export ("loadFriendPlayersWithCompletionHandler:")]
		void LoadFriendPlayers ([NullAllowed] Action<GKPlayer [], NSError> completionHandler);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("fetchSavedGamesWithCompletionHandler:")]
		void FetchSavedGames ([NullAllowed] Action<GKSavedGame [], NSError> handler);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("saveGameData:withName:completionHandler:")]
		void SaveGameData (NSData data, string name, [NullAllowed] Action<GKSavedGame, NSError> handler);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("deleteSavedGamesWithName:completionHandler:")]
		void DeleteSavedGames (string name, [NullAllowed] Action<NSError> handler);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("resolveConflictingSavedGames:withData:completionHandler:")]
		void ResolveConflictingSavedGames (GKSavedGame [] conflictingSavedGames, NSData data, [NullAllowed] Action<GKSavedGame [], NSError> handler);

		[NoWatch]
		[TV (13, 0)]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("multiplayerGamingRestricted")]
		bool MultiplayerGamingRestricted { [Bind ("isMultiplayerGamingRestricted")] get; }

		[TV (13, 0)]
		[iOS (13, 0)]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("loadChallengableFriendsWithCompletionHandler:")]
		[Async]
		void LoadChallengeableFriends ([NullAllowed] Action<GKPlayer [], NSError> completionHandler);

		[NoWatch]
		[TV (13, 0)]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("local")]
		GKLocalPlayer Local { get; }

		[NoWatch]
		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("personalizedCommunicationRestricted")]
		bool PersonalizedCommunicationRestricted { [Bind ("isPersonalizedCommunicationRestricted")] get; }

		// FriendsList Category

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("loadFriendsAuthorizationStatus:")]
		void LoadFriendsAuthorizationStatus (Action<GKFriendsAuthorizationStatus, NSError> completionHandler);

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("loadFriends:")]
		void LoadFriendsList (Action<GKPlayer [], NSError> completionHandler);

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("loadFriendsWithIdentifiers:completionHandler:")]
		void LoadFriendsList (string [] identifiers, Action<GKPlayer [], NSError> completionHandler);
	}

	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKSavedGame : NSCopying {
		[Export ("name")]
		[NullAllowed]
		string Name { get; }

		[Export ("deviceName")]
		[NullAllowed]
		string DeviceName { get; }

		[Export ("modificationDate")]
		[NullAllowed]
		NSDate ModificationDate { get; }

		[Export ("loadDataWithCompletionHandler:")]
		[Async]
		void LoadData ([NullAllowed] Action<NSData, NSError> handler);
	}

	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface GKSavedGameListener {
		[Export ("player:didModifySavedGame:")]
		void DidModifySavedGame (GKPlayer player, GKSavedGame savedGame);

		[Export ("player:hasConflictingSavedGames:")]
		void HasConflictingSavedGames (GKPlayer player, GKSavedGame [] savedGames);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (GKMatchDelegate) })]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[__NSCFDictionary setObject:forKey:]: attempt to insert nil value (key: 1500388194)
	// <quote>Your application never directly allocates GKMatch objects.</quote> http://developer.apple.com/library/ios/#documentation/GameKit/Reference/GKMatch_Ref/Reference/Reference.html
	[DisableDefaultCtor]
	interface GKMatch {
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'Players' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'Players' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Players' instead.")]
		[NullAllowed, Export ("playerIDs")]
		string [] PlayersIDs { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IGKMatchDelegate Delegate { get; set; }

		[Export ("expectedPlayerCount")]
		nint ExpectedPlayerCount { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'SendDataToAllPlayers (NSData, GKPlayer[] players, GKMatchSendDataMode mode, NSError error)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'SendDataToAllPlayers (NSData, GKPlayer[] players, GKMatchSendDataMode mode, NSError error)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SendDataToAllPlayers (NSData, GKPlayer[] players, GKMatchSendDataMode mode, NSError error)' instead.")]
		[Export ("sendData:toPlayers:withDataMode:error:")]
		// OOPS: bug we shipped with and can not realistically fix, but good news: this is deprecated (the NSError should have been an out)
		bool SendData (NSData data, string [] players, GKMatchSendDataMode mode, out NSError error);

		[Export ("sendDataToAllPlayers:withDataMode:error:")]
		bool SendDataToAllPlayers (NSData data, GKMatchSendDataMode mode, out NSError error);

		[Export ("disconnect")]
		void Disconnect ();

		[Export ("voiceChatWithName:")]
		[return: NullAllowed]
		GKVoiceChat VoiceChatWithName (string name);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'ChooseBestHostingPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'ChooseBestHostingPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ChooseBestHostingPlayer' instead.")]
		[Export ("chooseBestHostPlayerWithCompletionHandler:")]
		[Async]
		void ChooseBestHostPlayer (Action<string> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("rematchWithCompletionHandler:")]
		[Async]
		void Rematch ([NullAllowed] Action<GKMatch, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("players")]
		GKPlayer [] Players { get; }

		[MacCatalyst (13, 1)]
		[Export ("chooseBestHostingPlayerWithCompletionHandler:")]
		[Async]
		void ChooseBestHostingPlayer (Action<GKPlayer> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("sendData:toPlayers:dataMode:error:")]
		bool SendData (NSData data, GKPlayer [] players, GKMatchSendDataMode mode, out NSError error);
	}

	interface IGKMatchDelegate { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKMatchDelegate {

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'DataReceivedFromPlayer (GKMatch,NSData,GKPlayer)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'DataReceivedFromPlayer (GKMatch,NSData,GKPlayer)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DataReceivedFromPlayer (GKMatch,NSData,GKPlayer)' instead.")]
		[Export ("match:didReceiveData:fromPlayer:"), EventArgs ("GKData")]
		void DataReceived (GKMatch match, NSData data, string playerId);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'StateChangedForPlayer (GKMatch,GKPlayer,GKPlayerConnectionState)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'StateChangedForPlayer (GKMatch,GKPlayer,GKPlayerConnectionState)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'StateChangedForPlayer (GKMatch,GKPlayer,GKPlayerConnectionState)' instead.")]
		[Export ("match:player:didChangeState:"), EventArgs ("GKState")]
		void StateChanged (GKMatch match, string playerId, GKPlayerConnectionState state);

#if MONOMAC
#if !NET
		// This API was removed or never existed. Can't cleanly remove due to EventsArgs/Delegate
		[Obsolete ("It will never be called.")]
		[Export ("xamarin:selector:removed:"), EventArgs ("GKPlayerError")]
		void ConnectionFailed (GKMatch match, string playerId, NSError error);
#endif
#endif

		[Export ("match:didFailWithError:"), EventArgs ("GKError")]
		void Failed (GKMatch match, [NullAllowed] NSError error);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'ShouldReinviteDisconnectedPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'ShouldReinviteDisconnectedPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ShouldReinviteDisconnectedPlayer' instead.")]
		[Export ("match:shouldReinvitePlayer:"), DelegateName ("GKMatchReinvitation"), DefaultValue (true)]
		bool ShouldReinvitePlayer (GKMatch match, string playerId);

		[MacCatalyst (13, 1)]
		[Export ("match:didReceiveData:fromRemotePlayer:"), EventArgs ("GKMatchReceivedDataFromRemotePlayer")]
		void DataReceivedFromPlayer (GKMatch match, NSData data, GKPlayer player);

		[Export ("match:player:didChangeConnectionState:"), EventArgs ("GKMatchConnectionChanged")]
		void StateChangedForPlayer (GKMatch match, GKPlayer player, GKPlayerConnectionState state);

		[MacCatalyst (13, 1)]
		[Export ("match:shouldReinviteDisconnectedPlayer:")]
		[DelegateName ("GKMatchReinvitationForDisconnectedPlayer"), DefaultValue (true)]
		bool ShouldReinviteDisconnectedPlayer (GKMatch match, GKPlayer player);

		[MacCatalyst (13, 1)]
		[Export ("match:didReceiveData:forRecipient:fromRemotePlayer:"), EventArgs ("GKDataReceivedForRecipient")]
		void DataReceivedForRecipient (GKMatch match, NSData data, GKPlayer recipient, GKPlayer player);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKVoiceChat {
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; }

		[Export ("active", ArgumentSemantic.Assign)]
		bool Active { [Bind ("isActive")] get; set; }

		[Export ("volume", ArgumentSemantic.Assign)]
		float Volume { get; set; } /* float, not CGFloat */

		[Export ("start")]
		void Start ();

		[Export ("stop")]
		void Stop ();

		[NoTV]
		// the API was removed in iOS8
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'SetMuteStatus' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'SetMuteStatus' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetMuteStatus' instead.")]
		[Export ("setMute:forPlayer:")]
		void SetMute (bool isMuted, string playerID);

		[Static]
		[Export ("isVoIPAllowed")]
		bool IsVoIPAllowed ();

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'SetPlayerVoiceChatStateChangeHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'SetPlayerVoiceChatStateChangeHandler' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetPlayerVoiceChatStateChangeHandler' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("playerStateUpdateHandler", ArgumentSemantic.Copy)]
		GKPlayerStateUpdateHandler PlayerStateUpdateHandler { get; set; }
		//void SetPlayerStateUpdateHandler (GKPlayerStateUpdateHandler handler);

		[MacCatalyst (13, 1)]
		[Export ("setPlayerVoiceChatStateDidChangeHandler:", ArgumentSemantic.Copy)]
		void SetPlayerVoiceChatStateChangeHandler (Action<GKPlayer, GKVoiceChatPlayerState> handler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'Players' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'Players' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Players' instead.")]
		[NullAllowed, Export ("playerIDs")]
		string [] PlayerIDs { get; }

		[MacCatalyst (13, 1)]
		[Export ("players")]
		GKPlayer [] Players { get; }

		[MacCatalyst (13, 1)]
		[Export ("setPlayer:muted:")]
		void SetMuteStatus (GKPlayer player, bool isMuted);

		[MacCatalyst (13, 1)]
		[Export ("playerVoiceChatStateDidChangeHandler", ArgumentSemantic.Copy)]
		Action<GKPlayer, GKVoiceChatPlayerState> PlayerVoiceChatStateDidChangeHandler { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKMatchRequest {
		[Export ("minPlayers", ArgumentSemantic.Assign)]
		nint MinPlayers { get; set; }

		[Export ("maxPlayers", ArgumentSemantic.Assign)]
		nint MaxPlayers { get; set; }

		[Export ("playerGroup", ArgumentSemantic.Assign)]
		nint PlayerGroup { get; set; }

		[Export ("playerAttributes", ArgumentSemantic.Assign)]
		uint PlayerAttributes { get; set; } /* uint32_t */

		[NoTV]
		[NoWatch]
		[NullAllowed] // by default this property is null
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'Recipients' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'Recipients' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Recipients' instead.")]
		[Export ("playersToInvite", ArgumentSemantic.Retain)]
		string [] PlayersToInvite { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("inviteMessage", ArgumentSemantic.Copy)]
		string InviteMessage { get; set; }

		[Export ("defaultNumberOfPlayers", ArgumentSemantic.Assign)]
		nint DefaultNumberOfPlayers { get; set; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'RecipientResponseHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'RecipientResponseHandler' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RecipientResponseHandler' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("inviteeResponseHandler", ArgumentSemantic.Copy)]
		Action<string, GKInviteeResponse> InviteeResponseHandler { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("recipientResponseHandler", ArgumentSemantic.Copy)]
		Action<GKPlayer, GKInviteRecipientResponse> RecipientResponseHandler { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maxPlayersAllowedForMatchOfType:"), Static]
		nint GetMaxPlayersAllowed (GKMatchType matchType);

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("recipients", ArgumentSemantic.Retain)]
		GKPlayer [] Recipients { get; set; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'GKMatchmakerViewController.MatchmakingMode' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'GKMatchmakerViewController.MatchmakingMode' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'GKMatchmakerViewController.MatchmakingMode' instead.")]
		[TV (13, 0)]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'GKMatchmakerViewController.MatchmakingMode' instead.")]
		[Export ("restrictToAutomatch")]
		bool RestrictToAutomatch { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKInvite {

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'Sender' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'Sender' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Sender' instead.")]
		[Export ("inviter", ArgumentSemantic.Retain)]
		string Inviter { get; }

		[Export ("hosted", ArgumentSemantic.Assign)]
		bool Hosted { [Bind ("isHosted")] get; }

		[MacCatalyst (13, 1)]
		[Export ("playerGroup")]
		nint PlayerGroup { get; }

		[MacCatalyst (13, 1)]
		[Export ("playerAttributes")]
		uint PlayerAttributes { get; } /* uint32_t */

		[MacCatalyst (13, 1)]
		[Export ("sender", ArgumentSemantic.Retain)]
		GKPlayer Sender { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKMatchmaker {
		[Static]
		[Export ("sharedMatchmaker")]
		GKMatchmaker SharedMatchmaker { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKInviteEventListenerProtocol'.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKInviteEventListenerProtocol'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKInviteEventListenerProtocol'.")]
		[NullAllowed, Export ("inviteHandler", ArgumentSemantic.Copy)]
		GKInviteHandler InviteHandler { get; set; }

		[Export ("findMatchForRequest:withCompletionHandler:")]
		[Async]
		void FindMatch (GKMatchRequest request, [NullAllowed] GKNotificationMatch matchHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'FindPlayersForHostedRequest' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'FindPlayersForHostedRequest' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FindPlayersForHostedRequest' instead.")]
		[Export ("findPlayersForHostedMatchRequest:withCompletionHandler:")]
		[Async]
		void FindPlayers (GKMatchRequest request, [NullAllowed] GKFriendsHandler playerHandler);

		[Export ("addPlayersToMatch:matchRequest:completionHandler:")]
		[Async]
		void AddPlayers (GKMatch toMatch, GKMatchRequest matchRequest, [NullAllowed] Action<NSError> completionHandler);

		[Export ("cancel")]
		void Cancel ();

		[Export ("queryPlayerGroupActivity:withCompletionHandler:")]
		[Async]
		void QueryPlayerGroupActivity (nint playerGroup, [NullAllowed] GKQueryHandler completionHandler);

		[Export ("queryActivityWithCompletionHandler:")]
		[Async]
		void QueryActivity ([NullAllowed] GKQueryHandler completionHandler);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("matchForInvite:completionHandler:")]
		[Async]
		void Match (GKInvite invite, [NullAllowed] Action<GKMatch, NSError> completionHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'CancelPendingInvite' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'CancelPendingInvite' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CancelPendingInvite' instead.")]
		[Export ("cancelInviteToPlayer:")]
		void CancelInvite (string playerID);

		[MacCatalyst (13, 1)]
		[Export ("finishMatchmakingForMatch:")]
		void FinishMatchmaking (GKMatch match);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'StartBrowsingForNearbyPlayers(Action<GKPlayer, bool> handler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'StartBrowsingForNearbyPlayers(Action<GKPlayer, bool> handler)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'StartBrowsingForNearbyPlayers(Action<GKPlayer, bool> handler)' instead.")]
		[Export ("startBrowsingForNearbyPlayersWithReachableHandler:")]
		void StartBrowsingForNearbyPlayers ([NullAllowed] Action<string, bool> reachableHandler);

		[MacCatalyst (13, 1)]
		[Export ("stopBrowsingForNearbyPlayers")]
		void StopBrowsingForNearbyPlayers ();

		[MacCatalyst (13, 1)]
		[Export ("cancelPendingInviteToPlayer:")]
		void CancelPendingInvite (GKPlayer player);

		[MacCatalyst (13, 1)]
		[Export ("findPlayersForHostedRequest:withCompletionHandler:")]
		[Async]
		void FindPlayersForHostedRequest (GKMatchRequest request, [NullAllowed] Action<GKPlayer [], NSError> completionHandler);

		// Not truly an [Async] method since the handler can be called multiple times, for each player found
		[MacCatalyst (13, 1)]
		[Export ("startBrowsingForNearbyPlayersWithHandler:")]
		void StartBrowsingForNearbyPlayers ([NullAllowed] Action<GKPlayer, bool> handler);

		[Mac (13, 1), iOS (16, 2), NoTV]
		[MacCatalyst (16, 2)]
		[Export ("startGroupActivityWithPlayerHandler:")]
		void StartGroupActivity (Action<GKPlayer> handler);

		[Mac (13, 1), iOS (16, 2), NoTV]
		[MacCatalyst (16, 2)]
		[Export ("stopGroupActivity")]
		void StopGroupActivity ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
#if MONOMAC
	[BaseType (typeof (NSViewController), Delegates=new string [] { "WeakMatchmakerDelegate" }, Events=new Type [] {typeof(GKMatchmakerViewControllerDelegate)})]
#else
	[BaseType (typeof (UINavigationController), Delegates = new string [] { "WeakMatchmakerDelegate" }, Events = new Type [] { typeof (GKMatchmakerViewControllerDelegate) })]
#endif
	// iOS 6 -> Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: <GKMatchmakerViewController: 0x16101160>: must use one of the designated initializers
	[DisableDefaultCtor]
	interface GKMatchmakerViewController
#if MONOMAC
	: GKViewController
#endif
	{
		[NoiOS]
		[NoMacCatalyst]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[NullAllowed]
		[Export ("matchmakerDelegate", ArgumentSemantic.Assign)]
		NSObject WeakMatchmakerDelegate { get; set; }

		[NullAllowed]
		[Wrap ("WeakMatchmakerDelegate")]
		IGKMatchmakerViewControllerDelegate MatchmakerDelegate { get; set; }

		[Export ("matchRequest", ArgumentSemantic.Strong)]
		GKMatchRequest MatchRequest { get; }

		[Export ("hosted", ArgumentSemantic.Assign)]
		bool Hosted { [Bind ("isHosted")] get; set; }

		[Export ("initWithMatchRequest:")]
		NativeHandle Constructor (GKMatchRequest request);

		[Export ("initWithInvite:")]
		NativeHandle Constructor (GKInvite invite);

		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 5, 0, message: "Use 'SetHostedPlayerConnected' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetHostedPlayerConnected' instead.")]
		[Export ("setHostedPlayerReady:")]
		void SetHostedPlayerReady (string playerID);

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("defaultInvitationMessage", ArgumentSemantic.Copy)]
		[NullAllowed]
		string DefaultInvitationMessage { get; set; }

		[Export ("addPlayersToMatch:")]
		void AddPlayersToMatch (GKMatch match);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'SetHostedPlayerConnected (GKPlayer,bool)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'SetHostedPlayerConnected (GKPlayer,bool)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetHostedPlayerConnected (GKPlayer,bool)' instead.")]
		[Export ("setHostedPlayer:connected:")]
		void SetHostedPlayerConnected (string playerID, bool connected);

		[MacCatalyst (13, 1)]
		[Export ("setHostedPlayer:didConnect:")]
		void SetHostedPlayerConnected (GKPlayer playerID, bool connected);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("matchmakingMode", ArgumentSemantic.Assign)]
		GKMatchmakingMode MatchmakingMode { get; set; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("canStartWithMinimumPlayers")]
		bool CanStartWithMinimumPlayers { get; set; }
	}

	interface IGKMatchmakerViewControllerDelegate { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKMatchmakerViewControllerDelegate {
		[Abstract]
		[Export ("matchmakerViewControllerWasCancelled:")]
		void WasCancelled (GKMatchmakerViewController viewController);

		[Abstract]
		[Export ("matchmakerViewController:didFailWithError:"), EventArgs ("GKError")]
		void DidFailWithError (GKMatchmakerViewController viewController, NSError error);

#if !NET && !XAMCORE_5_0
		[Abstract]
#endif
		[Export ("matchmakerViewController:didFindMatch:"), EventArgs ("GKMatch")]
		void DidFindMatch (GKMatchmakerViewController viewController, GKMatch match);

#if !NET && !XAMCORE_5_0
		[Abstract]
#endif
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'DidFindHostedPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'DidFindHostedPlayers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidFindHostedPlayers' instead.")]
		[Export ("matchmakerViewController:didFindPlayers:"), EventArgs ("GKPlayers")]
		void DidFindPlayers (GKMatchmakerViewController viewController, string [] playerIDs);

#if !NET && !XAMCORE_5_0
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("matchmakerViewController:didFindHostedPlayers:"), EventArgs ("GKMatchmakingPlayers")]
		void DidFindHostedPlayers (GKMatchmakerViewController viewController, GKPlayer [] playerIDs);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'HostedPlayerDidAccept' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'HostedPlayerDidAccept' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HostedPlayerDidAccept' instead.")]
		[Export ("matchmakerViewController:didReceiveAcceptFromHostedPlayer:"), EventArgs ("GKPlayer")]
		void ReceivedAcceptFromHostedPlayer (GKMatchmakerViewController viewController, string playerID);

		[MacCatalyst (13, 1)]
		[Export ("matchmakerViewController:hostedPlayerDidAccept:"), EventArgs ("GKMatchmakingPlayer")]
		void HostedPlayerDidAccept (GKMatchmakerViewController viewController, GKPlayer playerID);
	}

	[BaseType (typeof (NSObject))]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	interface GKAchievement : NSSecureCoding {
		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'IsHidden' on the 'GKAchievementDescription' class instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'IsHidden' on the 'GKAchievementDescription' class instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'IsHidden' on the 'GKAchievementDescription' class instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'IsHidden' on the 'GKAchievementDescription' class instead.")]
		[Export ("hidden", ArgumentSemantic.Assign)]
		bool Hidden { [Bind ("isHidden")] get; }

		[NullAllowed] // by default this property is null
		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; set; }

		[Export ("percentComplete", ArgumentSemantic.Assign)]
		double PercentComplete { get; set; }

		[Export ("completed")]
		bool Completed { [Bind ("isCompleted")] get; }

		[Export ("lastReportedDate", ArgumentSemantic.Copy)]
		NSDate LastReportedDate { get; [NotImplemented] set; }

		[Static]
		[Export ("loadAchievementsWithCompletionHandler:")]
		[Async]
		void LoadAchievements ([NullAllowed] GKCompletionHandler completionHandler);

		[Static]
		[Export ("resetAchievementsWithCompletionHandler:")]
		[Async]
		void ResetAchivements ([NullAllowed] Action<NSError> completionHandler);

		[Wrap ("this ((string) null!)")]
		NativeHandle Constructor ();

		[Export ("initWithIdentifier:")]
		NativeHandle Constructor ([NullAllowed] string identifier);

		[NoMac]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'ctor (string identifier, GKPlayer player)' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'ctor (string identifier, GKPlayer player)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ctor (string identifier, GKPlayer player)' instead.")]
		[Export ("initWithIdentifier:forPlayer:")]
		NativeHandle Constructor ([NullAllowed] string identifier, string playerId);

		[Export ("reportAchievementWithCompletionHandler:")]
		[Async]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use ReportAchievements '(GKAchievement[] achievements, Action<NSError> completionHandler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use ReportAchievements '(GKAchievement[] achievements, Action<NSError> completionHandler)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use ReportAchievements '(GKAchievement[] achievements, Action<NSError> completionHandler)' instead.")]
		void ReportAchievement ([NullAllowed] Action<NSError> completionHandler);

		[Export ("showsCompletionBanner", ArgumentSemantic.Assign)]
		bool ShowsCompletionBanner { get; set; }

		[Static]
		[Export ("reportAchievements:withCompletionHandler:")]
		[Async]
		void ReportAchievements (GKAchievement [] achievements, [NullAllowed] Action<NSError> completionHandler);

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Pass 'GKPlayers' to 'ChallengeComposeController(GKPlayer[] players, string message, ...)' and present the view controller instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Pass 'GKPlayers' to 'ChallengeComposeController(GKPlayer[] players, string message, ...)' and present the view controller instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Pass 'GKPlayers' to 'ChallengeComposeController(GKPlayer[] players, string message, ...)' and present the view controller instead.")]
		[Export ("issueChallengeToPlayers:message:")]
		void IssueChallengeToPlayers ([NullAllowed] string [] playerIDs, [NullAllowed] string message);

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Pass 'GKPlayers' to 'SelectChallengeablePlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Pass 'GKPlayers' to 'SelectChallengeablePlayers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Pass 'GKPlayers' to 'SelectChallengeablePlayers' instead.")]
		[Export ("selectChallengeablePlayerIDs:withCompletionHandler:")]
		[Async]
		void SelectChallengeablePlayerIDs ([NullAllowed] string [] playerIDs, [NullAllowed] Action<string [], NSError> completionHandler);

		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'Player' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Player' instead.")]
		[NullAllowed, Export ("playerID", ArgumentSemantic.Copy)]
		string PlayerID {
			get;
		}

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("reportAchievements:withEligibleChallenges:withCompletionHandler:"), Static]
		[Async]
		void ReportAchievements (GKAchievement [] achievements, GKChallenge [] challenges, [NullAllowed] Action<NSError> completionHandler);

		[NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("player", ArgumentSemantic.Retain)]
		GKPlayer Player { get; }

		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:player:")]
		NativeHandle Constructor ([NullAllowed] string identifier, GKPlayer player);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "GKChallengeComposeResult")]
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.MacOSX, 14, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0)]
		[Export ("challengeComposeControllerWithMessage:players:completionHandler:")]
		UIViewController ChallengeComposeController ([NullAllowed] string message, GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler completionHandler);

		[TV (17, 0), iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0), NoWatch]
		[Export ("challengeComposeControllerWithMessage:players:completion:")]
		[Async (ResultTypeName = "GKChallengeComposeControllerResult")]
		UIViewController ChallengeComposeControllerWithMessage ([NullAllowed] string message, GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler2 completionHandler);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("selectChallengeablePlayers:withCompletionHandler:")]
		void SelectChallengeablePlayers (GKPlayer [] players, [NullAllowed] Action<GKPlayer [], NSError> completionHandler);

		[NoMac]
		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("challengeComposeControllerWithPlayers:message:completionHandler:")]
		[return: NullAllowed]
		UIViewController ChallengeComposeController ([NullAllowed] GKPlayer [] playerIDs, [NullAllowed] string message, [NullAllowed] GKChallengeComposeHandler completionHandler);
	}

	[BaseType (typeof (NSObject))]
	[MacCatalyst (13, 1)]
	interface GKAchievementDescription : NSSecureCoding {
		[Export ("identifier", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Identifier { get; }

		[Export ("title", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Title { get; }

		[Export ("achievedDescription", ArgumentSemantic.Copy)]
		[NullAllowed]
		string AchievedDescription { get; }

		[Export ("unachievedDescription", ArgumentSemantic.Copy)]
		[NullAllowed]
		string UnachievedDescription { get; }

		[Export ("maximumPoints", ArgumentSemantic.Assign)]
		nint MaximumPoints { get; }

		[Export ("hidden", ArgumentSemantic.Assign)]
		bool Hidden { [Bind ("isHidden")] get; }

		[Static]
		[Export ("loadAchievementDescriptionsWithCompletionHandler:")]
		[Async]
		void LoadAchievementDescriptions ([NullAllowed] GKAchievementDescriptionHandler handler);

		[MacCatalyst (14, 0)] // the headers lie, not usable until at least Mac Catalyst 14.0
		[NoWatch]
		[Export ("loadImageWithCompletionHandler:")]
		[Async]
		void LoadImage ([NullAllowed] GKImageLoadedHandler imageLoadedHandler);

		[Export ("groupIdentifier", ArgumentSemantic.Retain)]
		[NullAllowed]
		string GroupIdentifier { get; }

		[Export ("replayable", ArgumentSemantic.Assign)]
		bool Replayable { [Bind ("isReplayable")] get; }

#if MONOMAC
		[Export ("image", ArgumentSemantic.Retain)]
#else
		[Export ("image")]
#endif
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'LoadImage' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'LoadImage' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'LoadImage' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LoadImage' instead.")]
		[NullAllowed]
		UIImage Image { get; }

		[MacCatalyst (14, 0)] // the headers lie, not usable until at least Mac Catalyst 14.0
		[NoWatch]
		[Static]
		[Export ("incompleteAchievementImage")]
		UIImage IncompleteAchievementImage { get; }

		[MacCatalyst (14, 0)] // the headers lie, not usable until at least Mac Catalyst 14.0
		[NoWatch]
		[Static]
		[Export ("placeholderCompletedAchievementImage")]
		UIImage PlaceholderCompletedAchievementImage { get; }

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("rarityPercent", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSNumber RarityPercent { get; }
	}

	interface IGKAchievementViewControllerDelegate { }

	[NoWatch]
	[NoTV]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKGameCenterViewController' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKAchievementViewControllerDelegate {
		[Abstract]
		[Export ("achievementViewControllerDidFinish:")]
		void DidFinish (GKAchievementViewController viewController);
	}

	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKGameCenterViewController' instead.")]
#if MONOMAC
	[BaseType (typeof (GKGameCenterViewController), Events=new Type [] { typeof (GKAchievementViewControllerDelegate)}, Delegates=new string [] {"WeakDelegate"})]
	interface GKAchievementViewController 
#else
	[BaseType (typeof (GKGameCenterViewController), Events = new Type [] { typeof (GKAchievementViewControllerDelegate) }, Delegates = new string [] { "WeakDelegate" })]
	interface GKAchievementViewController : UIAppearance
#endif
	{
		[Export ("achievementDelegate", ArgumentSemantic.Weak), NullAllowed]
#if !MONOMAC
		[Override]
#endif
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IGKAchievementViewControllerDelegate Delegate { get; set; }
	}

	[NoiOS]
	[NoMacCatalyst]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSResponder))]
	interface GKDialogController {
		[Export ("parentWindow", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSWindow ParentWindow { get; set; }

		[Export ("presentViewController:")]
		bool PresentViewController (NSViewController viewController);

		[Export ("dismiss:")]
		void Dismiss (NSObject sender);

		[Static]
		[Export ("sharedDialogController")]
		GKDialogController SharedDialogController { get; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[Deprecated (PlatformName.iOS, 10, 0)]
	[NoMacCatalyst]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
#if MONOMAC
	[BaseType (typeof (NSViewController), Events=new Type [] { typeof (GKFriendRequestComposeViewControllerDelegate)}, Delegates=new string[] {"WeakComposeViewDelegate"})]
	interface GKFriendRequestComposeViewController : GKViewController {
		[Export ("initWithNibName:bundle:")]
		[NoiOS]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);
#else
	[BaseType (typeof (UINavigationController), Events = new Type [] { typeof (GKFriendRequestComposeViewControllerDelegate) }, Delegates = new string [] { "WeakComposeViewDelegate" })]
	interface GKFriendRequestComposeViewController : UIAppearance {
#endif
		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("composeViewDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakComposeViewDelegate { get; set; }

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Wrap ("WeakComposeViewDelegate")]
		IGKFriendRequestComposeViewControllerDelegate ComposeViewDelegate { get; set; }

		[Export ("maxNumberOfRecipients")]
		[Static]
		nint MaxNumberOfRecipients { get; }

		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'AddRecipientPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'AddRecipientPlayers' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AddRecipientPlayers' instead.")]
		[Export ("addRecipientsWithEmailAddresses:")]
		void AddRecipientsFromEmails (string [] emailAddresses);

		[Export ("addRecipientPlayers:")]
		void AddRecipientPlayers (GKPlayer [] players);

		[Export ("addRecipientsWithPlayerIDs:")]
		void AddRecipientsFromPlayerIDs (string [] playerIDs);

		[Export ("setMessage:")]
		void SetMessage ([NullAllowed] string message);
	}

	interface IGKFriendRequestComposeViewControllerDelegate { }

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 10, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Model]
	[Protocol]
	interface GKFriendRequestComposeViewControllerDelegate {
		[Abstract]
		[Export ("friendRequestComposeViewControllerDidFinish:")]
		void DidFinish (GKFriendRequestComposeViewController viewController);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.iOS, 17, 0, message: "Use UNNotificationRequest or provide custom UI instead. This method will become a no-op in a future version of GameKit.")]
	[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use UNNotificationRequest or provide custom UI instead. This method will become a no-op in a future version of GameKit.")]
	[Deprecated (PlatformName.TvOS, 16, 1, message: "Use UNNotificationRequest or provide custom UI instead. This method will become a no-op in a future version of GameKit.")]
	[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use UNNotificationRequest or provide custom UI instead. This method will become a no-op in a future version of GameKit.")]
	[BaseType (typeof (NSObject))]
	partial interface GKNotificationBanner {
		[Static, Export ("showBannerWithTitle:message:completionHandler:")]
		[Async]
		void Show ([NullAllowed] string title, [NullAllowed] string message, [NullAllowed] Action onCompleted);

		[Export ("showBannerWithTitle:message:duration:completionHandler:"), Static]
		[Async]
		void Show ([NullAllowed] string title, [NullAllowed] string message, double durationSeconds, [NullAllowed] Action completionHandler);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKTurnBasedParticipant {
		[MacCatalyst (13, 1)]
		[Export ("player", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKPlayer Player { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'Player' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'Player' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Player' instead.")]
		[Export ("playerID", ArgumentSemantic.Copy)]
		[NullAllowed]
		string PlayerID { get; }

		[Export ("lastTurnDate", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDate LastTurnDate { get; }

		[Export ("status")]
		GKTurnBasedParticipantStatus Status { get; }

		[Export ("matchOutcome", ArgumentSemantic.Assign)]
		GKTurnBasedMatchOutcome MatchOutcome { get; set; }

		[Export ("timeoutDate", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDate TimeoutDate { get; }
	}

	interface IGKTurnBasedEventHandlerDelegate { }

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKTurnBasedEventListener'.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKTurnBasedEventListener'.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKTurnBasedEventListener'.")]
	interface GKTurnBasedEventHandlerDelegate {
		[Abstract]
		[Export ("handleInviteFromGameCenter:")]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void HandleInviteFromGameCenter (NSString [] playersToInvite);

		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'HandleTurnEvent' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'HandleTurnEvent' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HandleTurnEvent' instead.")]
		[Export ("handleTurnEventForMatch:")]
		void HandleTurnEventForMatch (GKTurnBasedMatch match);

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("handleMatchEnded:")]
		void HandleMatchEnded (GKTurnBasedMatch match);

#if !MONOMAC || NET || XAMCORE_5_0
		[Abstract]
#endif
		[Export ("handleTurnEventForMatch:didBecomeActive:")]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void HandleTurnEvent (GKTurnBasedMatch match, bool activated);
	}

	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use GKLocalPlayer.RegisterListener with an object that implements IGKTurnBasedEventListener.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use GKLocalPlayer.RegisterListener with an object that implements IGKTurnBasedEventListener.")]
	[BaseType (typeof (NSObject))]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use GKLocalPlayer.RegisterListener with an object that implements IGKTurnBasedEventListener.")]
	interface GKTurnBasedEventHandler {

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IGKTurnBasedEventHandlerDelegate Delegate { get; set; }

		[Export ("sharedTurnBasedEventHandler"), Static]
		GKTurnBasedEventHandler SharedTurnBasedEventHandler { get; }
	}

	delegate void GKTurnBasedMatchRequest (GKTurnBasedMatch match, NSError error);

	delegate void GKTurnBasedMatchesRequest (GKTurnBasedMatch [] matches, NSError error);

	delegate void GKTurnBasedMatchData (NSData matchData, NSError error);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKTurnBasedMatch {
		[Export ("matchID")]
		[NullAllowed]
		string MatchID { get; }

		[Export ("creationDate")]
		[NullAllowed]
		NSDate CreationDate { get; }

		[Export ("participants", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKTurnBasedParticipant [] Participants { get; }

		[Export ("status")]
		GKTurnBasedMatchStatus Status { get; }

		[Export ("currentParticipant", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKTurnBasedParticipant CurrentParticipant { get; }

		[Export ("matchData", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSData MatchData { get; }

		[NullAllowed] // by default this property is null
		[Export ("message", ArgumentSemantic.Copy)]
		string Message { get; set; }

		[Static]
		[Export ("findMatchForRequest:withCompletionHandler:")]
		[Async]
		void FindMatch (GKMatchRequest request, GKTurnBasedMatchRequest onCompletion);

		[Static]
		[Export ("loadMatchesWithCompletionHandler:")]
		[Async]
		void LoadMatches ([NullAllowed] GKTurnBasedMatchesRequest onCompletion);

		[Export ("removeWithCompletionHandler:")]
		[Async]
		void Remove ([NullAllowed] Action<NSError> onCompletion);

		[Export ("loadMatchDataWithCompletionHandler:")]
		[Async]
		void LoadMatchData ([NullAllowed] GKTurnBasedMatchData onCompletion);

		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'EndTurn' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'EndTurn' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'EndTurn' instead.")]
		[Export ("endTurnWithNextParticipant:matchData:completionHandler:")]
		[Async]
		void EndTurnWithNextParticipant (GKTurnBasedParticipant nextParticipant, NSData matchData, [NullAllowed] Action<NSError> noCompletion);

		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'ParticipantQuitInTurn (GKTurnBasedMatchOutcome, GKTurnBasedParticipant[], double, NSData, Action<NSError>)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'ParticipantQuitInTurn (GKTurnBasedMatchOutcome, GKTurnBasedParticipant[], double, NSData, Action<NSError>)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ParticipantQuitInTurn (GKTurnBasedMatchOutcome, GKTurnBasedParticipant[], double, NSData, Action<NSError>)' instead.")]
		[Export ("participantQuitInTurnWithOutcome:nextParticipant:matchData:completionHandler:")]
		[Async]
		void ParticipantQuitInTurn (GKTurnBasedMatchOutcome matchOutcome, GKTurnBasedParticipant nextParticipant, NSData matchData, [NullAllowed] Action<NSError> onCompletion);

		[Export ("participantQuitOutOfTurnWithOutcome:withCompletionHandler:")]
		[Async]
		void ParticipantQuitOutOfTurn (GKTurnBasedMatchOutcome matchOutcome, [NullAllowed] Action<NSError> onCompletion);

		[Export ("endMatchInTurnWithMatchData:completionHandler:")]
		[Async]
		void EndMatchInTurn (NSData matchData, [NullAllowed] Action<NSError> onCompletion);

		[Static]
		[Export ("loadMatchWithID:withCompletionHandler:")]
		[Async]
		void LoadMatch (string matchId, [NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[Export ("acceptInviteWithCompletionHandler:")]
		[Async]
		void AcceptInvite ([NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[Export ("declineInviteWithCompletionHandler:")]
		[Async]
		void DeclineInvite ([NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[Export ("matchDataMaximumSize")]
		nint MatchDataMaximumSize { get; }

		[MacCatalyst (13, 1)]
		[Export ("rematchWithCompletionHandler:")]
		[Async]
		void Rematch ([NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("endTurnWithNextParticipants:turnTimeout:matchData:completionHandler:")]
		[Async]
		void EndTurn (GKTurnBasedParticipant [] nextParticipants, double timeoutSeconds, NSData matchData, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("participantQuitInTurnWithOutcome:nextParticipants:turnTimeout:matchData:completionHandler:")]
		[Async]
		void ParticipantQuitInTurn (GKTurnBasedMatchOutcome matchOutcome, GKTurnBasedParticipant [] nextParticipants, double timeoutSeconds, NSData matchData, [NullAllowed] Action<NSError> completionHandler);

		[Export ("saveCurrentTurnWithMatchData:completionHandler:")]
		[Async]
		void SaveCurrentTurn (NSData matchData, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Field ("GKTurnTimeoutDefault"), Static]
		double DefaultTimeout { get; }

		[MacCatalyst (13, 1)]
		[Field ("GKTurnTimeoutNone"), Static]
		double NoTimeout { get; }

		[MacCatalyst (13, 1)]
		[Export ("exchanges", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKTurnBasedExchange [] Exchanges { get; }

		[MacCatalyst (13, 1)]
		[Export ("activeExchanges", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKTurnBasedExchange [] ActiveExchanges { get; }

		[MacCatalyst (13, 1)]
		[Export ("completedExchanges", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKTurnBasedExchange [] CompletedExchanges { get; }

		[MacCatalyst (13, 1)]
		[Export ("exchangeDataMaximumSize")]
		nuint ExhangeDataMaximumSize { get; }

		[MacCatalyst (13, 1)]
		[Export ("exchangeMaxInitiatedExchangesPerPlayer")]
		nuint ExchangeMaxInitiatedExchangesPerPlayer { get; }

		[MacCatalyst (13, 1)]
		[Export ("setLocalizableMessageWithKey:arguments:")]
		void SetMessage (string localizableMessage, [NullAllowed] params NSObject [] arguments);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'EndMatchInTurn (NSData, GKLeaderboardScore[], NSObject[], Action<NSError>)' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'EndMatchInTurn (NSData, GKLeaderboardScore[], NSObject[], Action<NSError>)' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'EndMatchInTurn (NSData, GKLeaderboardScore[], NSObject[], Action<NSError>)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'EndMatchInTurn (NSData, GKLeaderboardScore[], NSObject[], Action<NSError>)' instead.")]
		[Export ("endMatchInTurnWithMatchData:scores:achievements:completionHandler:")]
		[Async]
		void EndMatchInTurn (NSData matchData, [NullAllowed] GKScore [] scores, [NullAllowed] GKAchievement [] achievements, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("saveMergedMatchData:withResolvedExchanges:completionHandler:")]
		[Async]
		void SaveMergedMatchData (NSData matchData, GKTurnBasedExchange [] exchanges, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("sendExchangeToParticipants:data:localizableMessageKey:arguments:timeout:completionHandler:")]
		[Async]
		void SendExchange (GKTurnBasedParticipant [] participants, NSData data, string localizableMessage, NSObject [] arguments, double timeout, [NullAllowed] Action<GKTurnBasedExchange, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("sendReminderToParticipants:localizableMessageKey:arguments:completionHandler:")]
		[Async]
		void SendReminder (GKTurnBasedParticipant [] participants, string localizableMessage, NSObject [] arguments, [NullAllowed] Action<NSError> completionHandler);

		[iOS (14, 0)]
		[Mac (11, 0)]
		[Watch (7, 0)]
		[TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("endMatchInTurnWithMatchData:leaderboardScores:achievements:completionHandler:")]
		[Async]
		void EndMatchInTurn (NSData matchData, GKLeaderboardScore [] scores, NSObject [] achievements, Action<NSError> completionHandler);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	// iOS6 -> Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: <GKTurnBasedMatchmakerViewController: 0x18299df0>: must use one of the designated initializers
	[DisableDefaultCtor]
#if MONOMAC
	[BaseType (typeof (NSViewController))]
	interface GKTurnBasedMatchmakerViewController : GKViewController
#else
	[BaseType (typeof (UINavigationController))]
	interface GKTurnBasedMatchmakerViewController : UIAppearance
#endif
		{
		[NoiOS]
		[NoMacCatalyst]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[Export ("showExistingMatches", ArgumentSemantic.Assign)]
		bool ShowExistingMatches { get; set; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("matchmakingMode", ArgumentSemantic.Assign)]
		GKMatchmakingMode MatchmakingMode { get; set; }

		[Export ("initWithMatchRequest:")]
		NativeHandle Constructor (GKMatchRequest request);

		[Export ("turnBasedMatchmakerDelegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IGKTurnBasedMatchmakerViewControllerDelegate Delegate { get; set; }
	}

	interface IGKTurnBasedMatchmakerViewControllerDelegate { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKTurnBasedMatchmakerViewControllerDelegate {
#if !XAMCORE_5_0
		[Abstract]
#endif
		[Export ("turnBasedMatchmakerViewControllerWasCancelled:")]
		void WasCancelled (GKTurnBasedMatchmakerViewController viewController);

#if !XAMCORE_5_0
		[Abstract]
#endif
		[Export ("turnBasedMatchmakerViewController:didFailWithError:")]
		void FailedWithError (GKTurnBasedMatchmakerViewController viewController, NSError error);

#if !NET && !XAMCORE_5_0
		[Abstract]
#endif
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'GKTurnBasedEventListener.ReceivedTurnEvent' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'GKTurnBasedEventListener.ReceivedTurnEvent' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKTurnBasedEventListener.ReceivedTurnEvent' instead.")]
		[Export ("turnBasedMatchmakerViewController:didFindMatch:")]
		void FoundMatch (GKTurnBasedMatchmakerViewController viewController, GKTurnBasedMatch match);

#if !NET && !XAMCORE_5_0
		[Abstract]
#endif
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'GKTurnBasedEventListener.WantsToQuitMatch' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'GKTurnBasedEventListener.WantsToQuitMatch' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKTurnBasedEventListener.WantsToQuitMatch' instead.")]
		[Export ("turnBasedMatchmakerViewController:playerQuitForMatch:")]
		void PlayerQuitForMatch (GKTurnBasedMatchmakerViewController viewController, GKTurnBasedMatch match);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKChallenge : NSSecureCoding {
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'IssuingPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'IssuingPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'IssuingPlayer' instead.")]
		[Export ("issuingPlayerID", ArgumentSemantic.Copy)]
		[NullAllowed]
		string IssuingPlayerID { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'ReceivingPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'ReceivingPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ReceivingPlayer' instead.")]
		[Export ("receivingPlayerID", ArgumentSemantic.Copy)]
		[NullAllowed]
		string ReceivingPlayerID { get; }

		[Export ("state", ArgumentSemantic.Assign)]
		GKChallengeState State { get; }

		[Export ("issueDate", ArgumentSemantic.Retain)]
		NSDate IssueDate { get; }

		[Export ("completionDate", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSDate CompletionDate { get; }

		[Export ("message", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Message { get; }

		[Export ("decline")]
		void Decline ();

		[Export ("loadReceivedChallengesWithCompletionHandler:"), Static]
		[Async]
		void LoadReceivedChallenges ([NullAllowed] Action<GKChallenge [], NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("issuingPlayer", ArgumentSemantic.Copy)]
		[NullAllowed]
		GKPlayer IssuingPlayer { get; }

		[MacCatalyst (13, 1)]
		[Export ("receivingPlayer", ArgumentSemantic.Copy)]
		[NullAllowed]
		GKPlayer ReceivingPlayer { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKChallenge))]
	interface GKScoreChallenge {

		[Export ("score", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKScore Score { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKChallenge))]
	interface GKAchievementChallenge {

		[Export ("achievement", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKAchievement Achievement { get; }
	}

#if NET
	[DisableDefaultCtor] // the native 'init' method returned nil.
#endif
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (
#if MONOMAC
		typeof (NSViewController),
#else
		typeof (UINavigationController),
#endif
		Events = new [] { typeof (GKGameCenterControllerDelegate) },
		Delegates = new [] { "WeakDelegate" }
	)]
	interface GKGameCenterViewController
#if MONOMAC
	: GKViewController
#endif
	{
		[NoiOS]
		[NoMacCatalyst]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithLeaderboardID:playerScope:timeScope:")]
		NativeHandle Constructor (string leaderboardId, GKLeaderboardPlayerScope playerScope, GKLeaderboardTimeScope timeScope);

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithLeaderboard:playerScope:")]
		NativeHandle Constructor (GKLeaderboard leaderboard, GKLeaderboardPlayerScope playerScope);

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithAchievementID:")]
		NativeHandle Constructor (string achievementId);

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithState:")]
		NativeHandle Constructor (GKGameCenterViewControllerState state);

		[Export ("gameCenterDelegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IGKGameCenterControllerDelegate Delegate { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use '.ctor (GKGameCenterViewControllerState)' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use '.ctor (GKGameCenterViewControllerState)' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use '.ctor (GKGameCenterViewControllerState)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use '.ctor (GKGameCenterViewControllerState)' instead.")]
		[Export ("viewState", ArgumentSemantic.Assign)]
		GKGameCenterViewControllerState ViewState { get; set; }

		[NoTV]
		[Export ("leaderboardTimeScope", ArgumentSemantic.Assign)]
		[Deprecated (PlatformName.iOS, 7, 0, message: "This class no longer support 'LeaderboardTimeScope', will always default to 'AllTime'.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "This class no longer support 'LeaderboardTimeScope', will always default to 'AllTime'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This class no longer support 'LeaderboardTimeScope', will always default to 'AllTime'.")]
		GKLeaderboardTimeScope LeaderboardTimeScope { get; set; }

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("leaderboardCategory", ArgumentSemantic.Strong)]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'LeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'LeaderboardIdentifier' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LeaderboardIdentifier' instead.")]
		string LeaderboardCategory { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use '.ctor (GKLeaderboard, GKLeaderboardPlayerScope)' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use '.ctor (GKLeaderboard, GKLeaderboardPlayerScope)' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use '.ctor (GKLeaderboard, GKLeaderboardPlayerScope)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use '.ctor (GKLeaderboard, GKLeaderboardPlayerScope)' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("leaderboardIdentifier", ArgumentSemantic.Strong)]
		string LeaderboardIdentifier { get; set; }
	}

	interface IGKGameCenterControllerDelegate { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	interface GKGameCenterControllerDelegate {
		[Abstract]
		[Export ("gameCenterViewControllerDidFinish:")]
		void Finished (GKGameCenterViewController controller);
	}

	[NoWatch]
	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[BaseType (typeof (NSObject), Events = new [] { typeof (GKChallengeEventHandlerDelegate) }, Delegates = new [] { "WeakDelegate" })]
	[DisableDefaultCtor]
	interface GKChallengeEventHandler {
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IGKChallengeEventHandlerDelegate Delegate { get; set; }

		[Export ("challengeEventHandler"), Static]
		GKChallengeEventHandler Instance { get; }
	}

	interface IGKChallengeEventHandlerDelegate { }

	[NoWatch]
	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	interface GKChallengeEventHandlerDelegate {
		[Export ("localPlayerDidSelectChallenge:")]
		void LocalPlayerSelectedChallenge (GKChallenge challenge);

		[Export ("shouldShowBannerForLocallyReceivedChallenge:")]
		[DelegateName ("GKChallengePredicate"), DefaultValue (true)]
		bool ShouldShowBannerForLocallyReceivedChallenge (GKChallenge challenge);

		[Export ("localPlayerDidReceiveChallenge:")]
		void LocalPlayerReceivedChallenge (GKChallenge challenge);

		[Export ("shouldShowBannerForLocallyCompletedChallenge:")]
		[DelegateName ("GKChallengePredicate"), DefaultValue (true)]
		bool ShouldShowBannerForLocallyCompletedChallenge (GKChallenge challenge);

		[Export ("localPlayerDidCompleteChallenge:")]
		void LocalPlayerCompletedChallenge (GKChallenge challenge);

		[Export ("shouldShowBannerForRemotelyCompletedChallenge:")]
		[DelegateName ("GKChallengePredicate"), DefaultValue (true)]
		bool ShouldShowBannerForRemotelyCompletedChallenge (GKChallenge challenge);

		[Export ("remotePlayerDidCompleteChallenge:")]
		void RemotePlayerCompletedChallenge (GKChallenge challenge);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKTurnBasedExchange {
		[Export ("exchangeID")]
		[NullAllowed]
		string ExchangeID { get; }

		[Export ("sender")]
		[NullAllowed]
		GKTurnBasedParticipant Sender { get; }

		[Export ("recipients")]
		[NullAllowed]
		GKTurnBasedParticipant [] Recipients { get; }

		[Export ("status", ArgumentSemantic.Assign)]
		GKTurnBasedExchangeStatus Status { get; }

		[Export ("message")]
		[NullAllowed]
		string Message { get; }

		[Export ("data")]
		[NullAllowed]
		NSData Data { get; }

		[Export ("sendDate")]
		[NullAllowed]
		NSDate SendDate { get; }

		[Export ("timeoutDate")]
		[NullAllowed]
		NSDate TimeoutDate { get; }

		[Export ("completionDate")]
		[NullAllowed]
		NSDate CompletionDate { get; }

		[Export ("replies")]
		[NullAllowed]
		GKTurnBasedExchangeReply [] Replies { get; }

		[Export ("cancelWithLocalizableMessageKey:arguments:completionHandler:")]
		[Async]
		void Cancel (string localizableMessage, NSObject [] arguments, [NullAllowed] Action<NSError> completionHandler);

		[Export ("replyWithLocalizableMessageKey:arguments:data:completionHandler:")]
		[Async]
		void Reply (string localizableMessage, NSObject [] arguments, NSData data, [NullAllowed] Action<NSError> completionHandler);

		[Field ("GKExchangeTimeoutDefault")]
		double TimeoutDefault { get; }

		[Field ("GKExchangeTimeoutNone")]
		double TimeoutNone { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKTurnBasedExchangeReply {
		[Export ("recipient")]
		[NullAllowed]
		GKTurnBasedParticipant Recipient { get; }

		[Export ("message")]
		[NullAllowed]
		string Message { get; }

		[Export ("data")]
		[NullAllowed]
		NSData Data { get; }

		[MacCatalyst (13, 1)]
		[Export ("replyDate")]
		[NullAllowed]
		NSDate ReplyDate { get; }
	}

	interface IGKLocalPlayerListener { }

	[MacCatalyst (13, 1)]
	[Model, Protocol, BaseType (typeof (NSObject))]
	interface GKLocalPlayerListener : GKTurnBasedEventListener
#if !TVOS && !WATCH
		, GKSavedGameListener
#endif
#if !WATCH
		, GKChallengeListener, GKInviteEventListener
#endif
	{
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model, Protocol, BaseType (typeof (NSObject))]
	interface GKChallengeListener {
		[Export ("player:wantsToPlayChallenge:")]
		void WantsToPlayChallenge (GKPlayer player, GKChallenge challenge);

		[Export ("player:didReceiveChallenge:")]
		void DidReceiveChallenge (GKPlayer player, GKChallenge challenge);

		[Export ("player:didCompleteChallenge:issuedByFriend:")]
		void DidCompleteChallenge (GKPlayer player, GKChallenge challenge, GKPlayer friendPlayer);

		[Export ("player:issuedChallengeWasCompleted:byFriend:")]
		void IssuedChallengeWasCompleted (GKPlayer player, GKChallenge challenge, GKPlayer friendPlayer);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model, BaseType (typeof (NSObject))]
	interface GKInviteEventListener {
		[MacCatalyst (13, 1)]
		[Export ("player:didAcceptInvite:")]
		void DidAcceptInvite (GKPlayer player, GKInvite invite);

		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'DidRequestMatch (GKPlayer player, GKPlayer[] recipientPlayers)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidRequestMatch (GKPlayer player, GKPlayer[] recipientPlayers)' instead.")]
		[Export ("player:didRequestMatchWithPlayers:")]
		void DidRequestMatch (GKPlayer player, string [] playerIDs);

		[MacCatalyst (13, 1)]
		[Export ("player:didRequestMatchWithRecipients:")]
		void DidRequestMatch (GKPlayer player, GKPlayer [] recipientPlayers);
	}

	[MacCatalyst (13, 1)]
	[Model, Protocol, BaseType (typeof (NSObject))]
	interface GKTurnBasedEventListener {
#if NET
		[NoMac]
#endif
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'DidRequestMatchWithOtherPlayers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidRequestMatchWithOtherPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 0, message: "Use 'DidRequestMatchWithOtherPlayers' instead.")]
		[Export ("player:didRequestMatchWithPlayers:")]
		void DidRequestMatchWithPlayers (GKPlayer player, string [] playerIDsToInvite);

		[Export ("player:receivedTurnEventForMatch:didBecomeActive:")]
		void ReceivedTurnEvent (GKPlayer player, GKTurnBasedMatch match, bool becameActive);

		[Export ("player:matchEnded:")]
		void MatchEnded (GKPlayer player, GKTurnBasedMatch match);

		[Export ("player:receivedExchangeRequest:forMatch:")]
		void ReceivedExchangeRequest (GKPlayer player, GKTurnBasedExchange exchange, GKTurnBasedMatch match);

		[Export ("player:receivedExchangeCancellation:forMatch:")]
		void ReceivedExchangeCancellation (GKPlayer player, GKTurnBasedExchange exchange, GKTurnBasedMatch match);

		[Export ("player:receivedExchangeReplies:forCompletedExchange:forMatch:")]
		void ReceivedExchangeReplies (GKPlayer player, GKTurnBasedExchangeReply [] replies, GKTurnBasedExchange exchange, GKTurnBasedMatch match);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("player:didRequestMatchWithOtherPlayers:")]
		void DidRequestMatchWithOtherPlayers (GKPlayer player, GKPlayer [] playersToInvite);

		[MacCatalyst (13, 1)]
		[Export ("player:wantsToQuitMatch:")]
		void WantsToQuitMatch (GKPlayer player, GKTurnBasedMatch match);
	}

	[NoWatch]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GKMatchmakerViewController' (real-time) or 'GKTurnBasedMatchmakerViewController' (turn-based) instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GKMatchmakerViewController' (real-time) or 'GKTurnBasedMatchmakerViewController' (turn-based) instead.")]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GKMatchmakerViewController' (real-time) or 'GKTurnBasedMatchmakerViewController' (turn-based) instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKMatchmakerViewController' (real-time) or 'GKTurnBasedMatchmakerViewController' (turn-based) instead.")]
	[BaseType (typeof (NSObject))]
	interface GKGameSession {
		[Export ("identifier")]
		string Identifier { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("owner")]
		GKCloudPlayer Owner { get; }

		[Export ("players")]
		GKCloudPlayer [] Players { get; }

		[Export ("lastModifiedDate")]
		NSDate LastModifiedDate { get; }

		[Export ("lastModifiedPlayer")]
		GKCloudPlayer LastModifiedPlayer { get; }

		[Export ("maxNumberOfConnectedPlayers")]
		nint MaxNumberOfConnectedPlayers { get; }

		[Export ("badgedPlayers")]
		GKCloudPlayer [] BadgedPlayers { get; }

		[Async]
		[Static]
		[Export ("createSessionInContainer:withTitle:maxConnectedPlayers:completionHandler:")]
		void CreateSession ([NullAllowed] string containerName, string title, nint maxPlayers, Action<GKGameSession, NSError> completionHandler);

		[Async]
		[Static]
		[Export ("loadSessionsInContainer:completionHandler:")]
		void LoadSessions ([NullAllowed] string containerName, Action<GKGameSession [], NSError> completionHandler);

		[Async]
		[Static]
		[Export ("loadSessionWithIdentifier:completionHandler:")]
		void LoadSession (string identifier, Action<GKGameSession, NSError> completionHandler);

		[Async]
		[Static]
		[Export ("removeSessionWithIdentifier:completionHandler:")]
		void RemoveSession (string identifier, Action<NSError> completionHandler);

		[Async]
		[Export ("getShareURLWithCompletionHandler:")]
		void GetShareUrl (Action<NSUrl, NSError> completionHandler);

		[Async]
		[Export ("loadDataWithCompletionHandler:")]
		void LoadData (Action<NSData, NSError> completionHandler);

		[Async]
		[Export ("saveData:completionHandler:")]
		void SaveData (NSData data, Action<NSData, NSError> completionHandler);

		[Async]
		[Export ("setConnectionState:completionHandler:")]
		void SetConnectionState (GKConnectionState state, Action<NSError> completionHandler);

		[Export ("playersWithConnectionState:")]
		GKCloudPlayer [] GetPlayers (GKConnectionState state);

		[Async]
		[Export ("sendData:withTransportType:completionHandler:")]
		void SendData (NSData data, GKTransportType transport, Action<NSError> completionHandler);

		[Async]
		[Export ("sendMessageWithLocalizedFormatKey:arguments:data:toPlayers:badgePlayers:completionHandler:")]
		void SendMessage (string key, string [] arguments, [NullAllowed] NSData data, GKCloudPlayer [] players, bool badgePlayers, Action<NSError> completionHandler);

		[Async]
		[Export ("clearBadgeForPlayers:completionHandler:")]
		void ClearBadge (GKCloudPlayer [] players, Action<NSError> completionHandler);

		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GKLocalPlayer.RegisterListener' instead.")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GKLocalPlayer.RegisterListener' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GKLocalPlayer.RegisterListener' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKLocalPlayer.RegisterListener' instead.")]
		[Static]
		[Export ("addEventListener:")]
		void AddEventListener (IGKGameSessionEventListener listener);

		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GKLocalPlayer.UnregisterListener' instead.")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GKLocalPlayer.UnregisterListener' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GKLocalPlayer.UnregisterListener' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKLocalPlayer.UnregisterListener' instead.")]
		[Static]
		[Export ("removeEventListener:")]
		void RemoveEventListener (IGKGameSessionEventListener listener);
	}

	interface IGKGameSessionEventListener { }

	[NoWatch]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GKLocalPlayerListener' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GKLocalPlayerListener' instead.")]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GKLocalPlayerListener' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKLocalPlayerListener' instead.")]
	[Protocol]
	interface GKGameSessionEventListener {
		[Export ("session:didAddPlayer:")]
		void DidAddPlayer (GKGameSession session, GKCloudPlayer player);

		[Export ("session:didRemovePlayer:")]
		void DidRemovePlayer (GKGameSession session, GKCloudPlayer player);

		[Export ("session:player:didChangeConnectionState:")]
		void DidChangeConnectionState (GKGameSession session, GKCloudPlayer player, GKConnectionState newState);

		[Export ("session:player:didSaveData:")]
		void DidSaveData (GKGameSession session, GKCloudPlayer player, NSData data);

		[Export ("session:didReceiveData:fromPlayer:")]
		void DidReceiveData (GKGameSession session, NSData data, GKCloudPlayer player);

		[Export ("session:didReceiveMessage:withData:fromPlayer:")]
		void DidReceiveMessage (GKGameSession session, string message, NSData data, GKCloudPlayer player);
	}

	[NoMac]
	[NoWatch]
	[NoiOS]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GKMatchmakerViewController' (real-time) or 'GKTurnBasedMatchmakerViewController' (turn-based) instead.")]
	[NoMacCatalyst]
	[BaseType (typeof (UIViewController))]
	interface GKGameSessionSharingViewController {
		// inlined ctor
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("session", ArgumentSemantic.Strong)]
		GKGameSession Session { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IGKGameSessionSharingViewControllerDelegate Delegate { get; set; }

		[Export ("initWithSession:")]
		NativeHandle Constructor (GKGameSession session);
	}

	interface IGKGameSessionSharingViewControllerDelegate { }

	[NoMac]
	[NoWatch]
	[NoiOS]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GKMatchmakerViewControllerDelegate' (real-time) or 'GKTurnBasedMatchmakerViewControllerDelegate' (turn-based) instead.")]
	[NoMacCatalyst]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface GKGameSessionSharingViewControllerDelegate {
		[Abstract]
		[Export ("sharingViewController:didFinishWithError:")]
		void DidFinish (GKGameSessionSharingViewController viewController, [NullAllowed] NSError error);
	}

	interface IGKChallengesViewControllerDelegate { }

	[NoiOS, NoTV, NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	interface GKChallengesViewControllerDelegate {

		[Abstract]
		[Export ("challengesViewControllerDidFinish:")]
		void DidFinish (GKChallengesViewController viewController);
	}

	[NoiOS, NoTV, NoWatch]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	[NoMacCatalyst]
	[BaseType (typeof (NSViewController))]
	interface GKChallengesViewController : GKViewController {

		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[NullAllowed, Export ("challengeDelegate", ArgumentSemantic.Assign)]
		IGKChallengesViewControllerDelegate ChallengeDelegate { get; set; }
	}

	[NoiOS, NoTV, NoWatch]
	[NoMacCatalyst]
	[Protocol]
	interface GKViewController {
	}

	interface IGKSessionDelegate { }

	[NoTV]
	[NoWatch] // only exposed thru GKSession (not in 3.0)
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'MultipeerConnectivity.MCSessionDelegate' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'MultipeerConnectivity.MCSessionDelegate' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MultipeerConnectivity.MCSessionDelegate' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKSessionDelegate {
		[Export ("session:peer:didChangeState:")]
		void PeerChangedState (GKSession session, string peerID, GKPeerConnectionState state);

		[Export ("session:didReceiveConnectionRequestFromPeer:")]
		void PeerConnectionRequest (GKSession session, string peerID);

		[Export ("session:connectionWithPeerFailed:withError:")]
		void PeerConnectionFailed (GKSession session, string peerID, NSError error);

		[Export ("session:didFailWithError:")]
		void FailedWithError (GKSession session, NSError error);
	}

	[TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GKAccessPoint {
		[Static]
		[Export ("shared")]
		GKAccessPoint Shared { get; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; set; }

		[NoMac, NoiOS]
		[NoMacCatalyst]
		[Export ("focused")]
		bool Focused { [Bind ("isFocused")] get; set; }

		[Export ("visible")]
		bool Visible { [Bind ("isVisible")] get; }

		[Export ("isPresentingGameCenter")]
		bool IsPresentingGameCenter { get; }

		[Export ("showHighlights")]
		bool ShowHighlights { get; set; }

		[Export ("location", ArgumentSemantic.Assign)]
		GKAccessPointLocation Location { get; set; }

		[Export ("frameInScreenCoordinates")]
		CGRect FrameInScreenCoordinates { get; }

		[NullAllowed, Export ("parentWindow", ArgumentSemantic.Weak)]
		UIWindow ParentWindow { get; set; }

		[Export ("triggerAccessPointWithHandler:")]
		void TriggerAccessPoint (Action handler);

		[Export ("triggerAccessPointWithState:handler:")]
		void TriggerAccessPoint (GKGameCenterViewControllerState state, Action handler);
	}

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKLeaderboardEntry {
		[Export ("player", ArgumentSemantic.Strong)]
		GKPlayer Player { get; }

#if false
		// Some APIs missing on iOS, tvOS, watchOS as of Xcode 12 beta 3 - https://github.com/xamarin/maccore/issues/2269
		// disabled since the selectors don't respond on macOS 11.0
		[Export ("rank")]
		nint Rank { get; }

		[Export ("score")]
		nint Score { get; }

		[Export ("formattedScore", ArgumentSemantic.Strong)]
		string FormattedScore { get; }

		[Export ("context")]
		nuint Context { get; }

		[Export ("date", ArgumentSemantic.Strong)]
		NSDate Date { get; }
#endif

		[NoWatch] // header lists watch as supported, but UIViewController is not available on Watch!
		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "GKChallengeComposeResult")]
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0)]
		[Deprecated (PlatformName.MacOSX, 14, 0)]
		[Export ("challengeComposeControllerWithMessage:players:completionHandler:")]
		UIViewController ChallengeComposeController ([NullAllowed] string message, [NullAllowed] GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler completionHandler);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0), Mac (14, 0), NoWatch]
		[Export ("challengeComposeControllerWithMessage:players:completion:")]
		[Async (ResultTypeName = "GKChallengeComposeControllerResult")]
		UIViewController ChallengeComposeControllerWithMessage ([NullAllowed] string message, [NullAllowed] GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler2 completionHandler);
	}

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface GKLeaderboardScore {
		[Export ("player", ArgumentSemantic.Strong)]
		GKPlayer Player { get; set; }

		[Export ("value")]
		nint Value { get; set; }

		[Export ("context")]
		nuint Context { get; set; }

		[Export ("leaderboardID", ArgumentSemantic.Strong)]
		string LeaderboardId { get; set; }
	}
}
