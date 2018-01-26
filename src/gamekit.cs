//
// GameKit.cs: This file describes the API that the generator will produce for GameKit
//
// Authors:
//   Miguel de Icaza
//   Marek Safar (marek.safar@gmail.com)
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2013 Xamarin Inc. All rights reserved
//

#pragma warning disable 618

using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreFoundation;
#if MONOMAC
using XamCore.AppKit;
#else
using XamCore.UIKit;
#endif

namespace XamCore.GameKit {

#if !XAMCORE_2_0
	delegate void GKNotificationHandler ([NullAllowed] NSError error);
#endif
	delegate void GKFriendsHandler      (string [] friends, NSError error);
	delegate void GKPlayersHandler      (GKPlayer [] players, NSError error);
	delegate void GKLeaderboardsHandler (GKLeaderboard [] leaderboards, NSError error);
	delegate void GKScoresLoadedHandler (GKScore [] scoreArray, NSError error);
	delegate void GKNotificationMatch   (GKMatch match, NSError error);
	delegate void GKInviteHandler       (GKInvite invite, string [] playerIDs);
	delegate void GKQueryHandler        (nint activity, NSError error);
	delegate void GKCompletionHandler   (GKAchievement [] achivements, NSError error);
	delegate void GKAchievementDescriptionHandler (GKAchievementDescription [] descriptions, NSError error);
	delegate void GKCategoryHandler     (string [] categories, string [] titles, NSError error);
	delegate void GKPlayerStateUpdateHandler (string playerId, GKVoiceChatPlayerState state);
	delegate void GKIdentityVerificationSignatureHandler (NSUrl publicKeyUrl, NSData signature, NSData salt, ulong timestamp, NSError error);
	delegate void GKLeaderboardSetsHandler (GKLeaderboardSet [] leaderboardSets, NSError error);

#if MONOMAC
	delegate void GKImageLoadedHandler  (NSImage image, NSError error);
	delegate void GKPlayerPhotoLoaded (NSImage photo, NSError error);
	delegate void GKChallengeComposeHandler (NSViewController composeController, bool issuedChallenge, string [] sentPlayerIDs);
#else
	delegate void GKImageLoadedHandler  (UIImage image, NSError error);
	delegate void GKPlayerPhotoLoaded (UIImage photo, NSError error);
	delegate void GKChallengeComposeHandler (UIViewController composeController, bool issuedChallenge, string [] sentPlayerIDs);
#endif

#if WATCH
	// hacks to let [NoWatch] work properly
	interface UIAppearance {}
	interface UIViewController {}
	interface UINavigationController {}
#endif
	

#if !MONOMAC
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKPeerPickerControllerDelegate {
		[Export ("peerPickerController:didSelectConnectionType:")]
		void ConnectionTypeSelected (GKPeerPickerController picker, GKPeerPickerConnectionType type);

		[Export ("peerPickerController:sessionForConnectionType:")]
		GKSession GetSession (GKPeerPickerController picker, GKPeerPickerConnectionType forType);

		[Export ("peerPickerController:didConnectPeer:toSession:")]
		void PeerConnected (GKPeerPickerController picker, string peerId, GKSession toSession);

		[Export ("peerPickerControllerDidCancel:")]
		void ControllerCancelled (GKPeerPickerController picker);
	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Availability (Introduced = Platform.iOS_3_0, Deprecated = Platform.iOS_7_0, Message = "Use 'MCBrowserViewController' from the 'MultipeerConnectivity' framework instead.")]
	interface GKPeerPickerController {
		[Export ("connectionTypesMask", ArgumentSemantic.Assign)]
		GKPeerPickerConnectionType ConnectionTypesMask { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GKPeerPickerControllerDelegate Delegate { get; set; }

		[Export ("show")]
		void Show ();

		[Export ("dismiss")]
		void Dismiss ();

		[Export ("visible")]
		bool Visible { [Bind ("isVisible")] get; }
	}

	[NoWatch] // only exposed thru GKVoiceChatService (not in 3.0)
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKVoiceChatClient {
		[Abstract]
		[Export ("voiceChatService:sendData:toParticipantID:")]
		void SendData (GKVoiceChatService voiceChatService, NSData data, string toParticipant);

		[Export ("participantID")][Abstract]
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

	[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Availability (Introduced = Platform.iOS_3_0, Deprecated = Platform.iOS_7_0, Message = "Use 'GKVoiceChat' instead.")]
	interface GKVoiceChatService {

		[Export ("defaultVoiceChatService")][Static]
		GKVoiceChatService Default { get; }

		[NullAllowed] // by default this property is null
		[Export ("client", ArgumentSemantic.Assign)]
		[Protocolize]
		GKVoiceChatClient Client { get; set; }

		[Export ("startVoiceChatWithParticipantID:error:")]
#if XAMCORE_2_0
		bool StartVoiceChat (string participantID, out NSError error);
#else
		bool StartVoiceChat (string participantID, IntPtr ns_error_out);
#endif
		
		[Export ("stopVoiceChatWithParticipantID:")]
		void StopVoiceChat (string participantID);

		[Export ("acceptCallID:error:")]
#if XAMCORE_2_0
		bool AcceptCall (nint callID, out NSError error);
#else
		bool AcceptCall (nint callID, IntPtr ns_error_out);
#endif

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
	[NoWatch] // only exposed thru GKSession (not in 3.0)
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

	[NoTV]
	[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
	[BaseType (typeof (NSObject))]
	[Availability (Introduced = Platform.iOS_3_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'MultipeerConnectivity.MCSession' instead.")]
	interface GKSession {
		[Export ("initWithSessionID:displayName:sessionMode:")]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
		IntPtr Constructor ([NullAllowed] string sessionID, [NullAllowed] string displayName, GKSessionMode mode);

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GKSessionDelegate Delegate { get; set; }

		[Export ("sessionID")]
		string SessionID { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
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

		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
		[Export ("sendData:toPeers:withDataMode:error:")]
#if XAMCORE_2_0
		bool SendData (NSData data, string [] peers, GKSendDataMode mode, out NSError error);
#else
		bool SendData (NSData data, string [] peers, GKSendDataMode mode, IntPtr ns_error_out);
#endif

		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
		[Export ("sendDataToAllPeers:withDataMode:error:")]
#if XAMCORE_2_0
		bool SendDataToAllPeers (NSData data, GKSendDataMode mode, out NSError error);
#else
		bool SendDataToAllPeers (NSData data, GKSendDataMode mode, IntPtr ns_error_out);
#endif

		// // SEL = -receiveData:fromPeer:inSession:context:
		[Export ("setDataReceiveHandler:withContext:")][Internal]
		void _SetDataReceiveHandler (NSObject obj, IntPtr context);

		[Export ("connectToPeer:withTimeout:")]
		void Connect (string peerID, double timeout);
		
		[Export ("cancelConnectToPeer:")]
		void CancelConnect (string peerID);

		[Export ("acceptConnectionFromPeer:error:")]
#if XAMCORE_2_0
		bool AcceptConnection (string peerID, out NSError error);
#else
		bool AcceptConnection (string peerID, IntPtr error_out);
#endif
		
		[Export ("denyConnectionFromPeer:")]
		void DenyConnection (string peerID);

		[Export ("disconnectPeerFromAllPeers:")]
		void DisconnectPeerFromAllPeers (string peerID);

		[Export ("disconnectFromAllPeers")]
		void DisconnectFromAllPeers ();

		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
		[Export ("peersWithConnectionState:")]
		string [] PeersWithConnectionState (GKPeerConnectionState state);
	}
#endif

	[Watch (3,0)]
	[iOS (4,2), Mac (10, 8)]
	[BaseType (typeof (NSObject))]
	interface GKLeaderboard {
		[Export ("timeScope", ArgumentSemantic.Assign)]
		GKLeaderboardTimeScope TimeScope { get; set;  }

		[Export ("playerScope", ArgumentSemantic.Assign)]
		GKLeaderboardPlayerScope PlayerScope { get; set;  }

		[Export ("maxRange", ArgumentSemantic.Assign)]
		nint MaxRange { get; }

		[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'Identifier' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("category", ArgumentSemantic.Copy)]
		string Category { get; set;  }

		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get;  }

		[Export ("range", ArgumentSemantic.Assign)]
		NSRange Range { get; set;  }

		[Export ("scores", ArgumentSemantic.Retain)]
		GKScore [] Scores { get;  }

		[Export ("localPlayerScore", ArgumentSemantic.Retain)]
		GKScore LocalPlayerScore { get;  }

		[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'ctor (GKPlayer [] players)' instead.")]
		[Export ("initWithPlayerIDs:")]
		IntPtr Constructor ([NullAllowed] string [] players);

		[Export ("loadScoresWithCompletionHandler:")]
		[Async]
		void LoadScores ([NullAllowed] GKScoresLoadedHandler scoresLoadedHandler);

		[NoTV]
		[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
		[Availability (Deprecated = Platform.iOS_6_0 | Platform.Mac_10_9, Message = "Use 'LoadLeaderboards' instead.")]
		[Static]
		[Export ("loadCategoriesWithCompletionHandler:")]
		[Async (ResultTypeName = "GKCategoryResult")]
		void LoadCategories ([NullAllowed] GKCategoryHandler categoryHandler);

		[NoTV]
		[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
		[Static]
		[Since (5,0)]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'SetDefaultLeaderboard' on 'GKLocalPlayer' instead.")]
		[Export ("setDefaultLeaderboard:withCompletionHandler:")]
		[Async]
#if XAMCORE_2_0
		void SetDefaultLeaderboard ([NullAllowed] string leaderboardIdentifier, [NullAllowed] Action<NSError> notificationHandler);
#else
		void SetDefaultLeaderboard ([NullAllowed] string leaderboardIdentifier, [NullAllowed] GKNotificationHandler notificationHandler);
#endif

		[Since (6,0)]
		[Export ("groupIdentifier", ArgumentSemantic.Retain)]
		string GroupIdentifier { get; [NotImplemented] set; }

		[Since (6,0)]
		[Static]
		[Export ("loadLeaderboardsWithCompletionHandler:")]
		[Async]
		void LoadLeaderboards ([NullAllowed] Action<GKLeaderboard[], NSError> completionHandler);

		[Since (7,0)][Mac (10,10)]
		[NullAllowed]
		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; set; }

		[NoTV]
		[NoWatch]
		[Since (7,0)][Mac (10,8)]
		[Export ("loadImageWithCompletionHandler:")]
		[Async]
		void LoadImage ([NullAllowed] GKImageLoadedHandler completionHandler);

		[iOS (8,0), Mac (10,10)]
		[Export ("initWithPlayers:")]
		IntPtr Constructor (GKPlayer [] players);

		[Mac (10,10)] // should be 10,8 but tests fails before Yosemite
		[Export ("loading")]
		bool IsLoading { [Bind ("isLoading")] get; }
	}

	[Watch (3,0)]
	[Since (7,0)][Mac (10,10)]
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

		[Export ("loadLeaderboardsWithCompletionHandler:")]
		[Async]
		void LoadLeaderboards ([NullAllowed] GKLeaderboardsHandler completionHandler);

#if !MONOMAC
		[NoTV]
		[NoWatch]
		[Export ("loadImageWithCompletionHandler:")]
		[Async]
		void LoadImage ([NullAllowed] GKImageLoadedHandler completionHandler);
#endif
	}

	[Watch (3,0)]
	[iOS (10,0)][Mac (10,12)]
	[TV (10,0)]
	[BaseType (typeof(NSObject))]
	interface GKBasePlayer
	{
		[NullAllowed, Export ("playerID", ArgumentSemantic.Retain)]
		string PlayerID { get; }

		[NullAllowed, Export ("displayName")]
		string DisplayName { get; }
	}

	[NoWatch]
	[iOS (10,0)][Mac (10,12)]
	[TV (10,0)]
	[BaseType (typeof(GKBasePlayer))]
	interface GKCloudPlayer
	{
		[Static]
		[Export ("getCurrentSignedInPlayerForContainer:completionHandler:")]
		void GetCurrentSignedInPlayer ([NullAllowed] string containerName, Action<GKCloudPlayer, NSError> handler);
	}

	[Watch (3,0)]
	[Since (4,2)]
	[BaseType (typeof (GKBasePlayer))]
	// note: NSSecureCoding conformity is undocumented - but since it's a runtime check (on ObjC) we still need it
	interface GKPlayer : NSSecureCoding {
		[Export ("playerID", ArgumentSemantic.Retain)]
		string PlayerID { get;  }

		[Export ("alias", ArgumentSemantic.Copy)]
		string Alias { get;  }

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'GKLocalPlayer.LoadFriendPlayers' instead.")]
		[Export ("isFriend")]
		bool IsFriend { get;  }
		
		[Static, Export ("loadPlayersForIdentifiers:withCompletionHandler:")]
		[Async]
		void LoadPlayersForIdentifiers (string [] identifiers, [NullAllowed] GKPlayersHandler completionHandler);

		[Field ("GKPlayerDidChangeNotificationName")]
		[Notification]
		// This name looks wrong, see the "Notification" at the end.
		NSString DidChangeNotificationNameNotification { get; }

		[NoWatch]
		[Since (5,0)]
		[Export ("loadPhotoForSize:withCompletionHandler:")]
		[Async]
		void LoadPhoto (GKPhotoSize size, [NullAllowed] GKPlayerPhotoLoaded onCompleted);

		[Since (6,0)][Mac (10,8)]
		[Export ("displayName")]
		string DisplayName { get; }

		[NoWatch]
		[Availability (Introduced = Platform.iOS_9_0 | Platform.Mac_10_11)]
		[Static]
		[Export ("anonymousGuestPlayerWithIdentifier:")]
		GKPlayer GetAnonymousGuestPlayer (string guestIdentifier);

		[NoWatch]
		[Availability (Introduced = Platform.iOS_9_0 | Platform.Mac_10_11)]
		[Export ("guestIdentifier")]
		string GuestIdentifier { get; }
	}

	[Watch (3,0)]
	[Since (4,1)]
	[BaseType (typeof (NSObject))]
	interface GKScore : NSSecureCoding {
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'InitWithLeaderboardIdentifier' instead.")]
		[Internal][NullAllowed]
		[Export ("initWithCategory:")]
		IntPtr InitWithCategory ([NullAllowed] string category);

		[iOS (8,0)][Mac (10,10)]
		[Export ("initWithLeaderboardIdentifier:player:")]
		IntPtr Constructor (string identifier, GKPlayer player);

		[NoWatch]
		[Since (7,0)][Mac (10,8)]
		[Export ("initWithLeaderboardIdentifier:forPlayer:")]
		IntPtr Constructor (string identifier, string playerID);

		[Since (7,0)][Mac (10,8)]
		[Internal][NullAllowed]
		[Export ("initWithLeaderboardIdentifier:")]
		IntPtr InitWithLeaderboardIdentifier (string identifier);

#if !XAMCORE_2_0
		[NoWatch]
		// [Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'Player' instead.")] - Unlike rest of deprecations we are just ripping out due to poor naming
		[Export ("playerID", ArgumentSemantic.Retain)]
		string Player { get;  }

		[NullAllowed]
		[iOS (8,0)][Mac (10,10)]
		[Export ("player", ArgumentSemantic.Retain)]
		GKPlayer GamePlayer { get; }
#else
		[NullAllowed]
		[iOS (8,0)][Mac (10,10)]
		[Export ("player", ArgumentSemantic.Retain)]
		GKPlayer Player { get; }
#endif

		[Export ("rank", ArgumentSemantic.Assign)]
		nint Rank { get;  }

		[Export ("date", ArgumentSemantic.Retain)]
		NSDate Date { get; }

		[Export ("value", ArgumentSemantic.Assign)]
		long Value { get; set;  }

		[Export ("formattedValue", ArgumentSemantic.Copy)]
		string FormattedValue { get;  }

		[NoWatch]
		[NoTV]
		[Availability (Introduced = Platform.iOS_4_1 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'LeaderboardIdentifier' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("category", ArgumentSemantic.Copy)]
#if XAMCORE_2_0
		string Category { get; set;  }
#else
		[Obsolete ("Use the 'Category' property instead.")]
		string category { get; set;  }
#endif

		[NoWatch]
		[NoTV]
		[Availability (Introduced = Platform.iOS_4_1 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'ReportScores' instead.")]
		[Export ("reportScoreWithCompletionHandler:")]
		[Async]
#if XAMCORE_2_0
		void ReportScore ([NullAllowed] Action<NSError> errorHandler);
#else
		void ReportScore ([NullAllowed] GKNotificationHandler errorHandler);
#endif

		[Since (5,0)]
		[Export ("context", ArgumentSemantic.Assign)]
		ulong Context { get; set; }

		[Since (5,0)]
		[Export ("shouldSetDefaultLeaderboard", ArgumentSemantic.Assign)]
		bool ShouldSetDefaultLeaderboard { get; set; }

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ... )' and present the view controller instead.")]
		[iOS (6,0), Mac (10,8)]
		[Export ("issueChallengeToPlayers:message:")]
		void IssueChallengeToPlayers ([NullAllowed] string[] playerIDs, [NullAllowed] string message);

		[Since (6,0)]
		[Export ("reportScores:withCompletionHandler:"), Static]
		[Async]
		void ReportScores (GKScore[] scores, [NullAllowed] Action<NSError> completionHandler);

		[Since (7,0)][Mac (10,10)]
		[NullAllowed] // by default this property is null
		[Export ("leaderboardIdentifier", ArgumentSemantic.Copy)]
		string LeaderboardIdentifier { get; set; }

		[NoWatch]
		[Since (7,0)][Mac (10,10)]
		[Export ("reportScores:withEligibleChallenges:withCompletionHandler:"), Static]
		[Async]
		void ReportScores (GKScore[] scores, [NullAllowed] GKChallenge[] challenges, [NullAllowed] Action<NSError> completionHandler);

#if !MONOMAC
		[NoTV][NoWatch]
		[Availability (Deprecated = Platform.iOS_8_0, Message = "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ...)' instead.")]
		[iOS (7,0)]
		[Export ("challengeComposeControllerWithPlayers:message:completionHandler:")]
		UIViewController ChallengeComposeController ([NullAllowed] string[] playerIDs, [NullAllowed] string message, [NullAllowed] GKChallengeComposeHandler completionHandler);
#endif

#if MONOMAC
		[Async (ResultTypeName = "GKChallengeComposeResult")]
		[Mac (10,10)]
		[Export ("challengeComposeControllerWithMessage:players:completionHandler:")]
		NSViewController ChallengeComposeController ([NullAllowed] string message, [NullAllowed] GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler completionHandler);
#else
		[Async (ResultTypeName = "GKChallengeComposeResult")]
		[NoWatch]
		[iOS (8,0)]
		[Export ("challengeComposeControllerWithMessage:players:completionHandler:")]
		UIViewController ChallengeComposeController ([NullAllowed] string message, [NullAllowed] GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler completionHandler);
#endif
	}

	[NoWatch]
	[NoTV]
	[Since (4,2)]
	[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'GKGameCenterViewController' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoTV]
	interface GKLeaderboardViewControllerDelegate {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("leaderboardViewControllerDidFinish:")]
		void DidFinish (GKLeaderboardViewController viewController);
	}

	[NoTV][NoWatch]
	[Availability (Introduced = Platform.iOS_4_2 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'GKGameCenterViewController' instead.")]
#if MONOMAC
	[BaseType (typeof (GKGameCenterViewController), Events=new Type [] { typeof (GKLeaderboardViewControllerDelegate)}, Delegates=new string [] {"WeakDelegate"})]
	interface GKLeaderboardViewController 
#else
	[BaseType (typeof (GKGameCenterViewController), Events=new Type [] { typeof (GKLeaderboardViewControllerDelegate)}, Delegates=new string [] {"WeakDelegate"})]
	interface GKLeaderboardViewController : UIAppearance 
#endif
	{
		[Export ("leaderboardDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GKLeaderboardViewControllerDelegate Delegate { get; set; }

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

	[Watch (3,0)]
	[Since (4,2)]
	[MountainLion]
	[BaseType (typeof (GKPlayer))]
	interface GKLocalPlayer {
		[Export ("authenticated")]
		bool Authenticated { [Bind ("isAuthenticated")] get;  }

		[NoWatch]
		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'LoadFriendPlayers' instead and collect the friends from the invoked callback.")]
		[Export ("friends", ArgumentSemantic.Retain)]
		string [] Friends { get;  }

		[Static, Export ("localPlayer")]
		GKLocalPlayer LocalPlayer { get; }

		[Export ("isUnderage")]
		bool IsUnderage { get; }

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_6_0 | Platform.Mac_10_8, Message = "Set the 'AuthenticationHandler' instead.")]
		[Export ("authenticateWithCompletionHandler:")]
		[Async]
#if XAMCORE_2_0
		void Authenticate ([NullAllowed] Action<NSError> handler);
#else
		void Authenticate ([NullAllowed] GKNotificationHandler handler);
#endif

		[iOS (10,0)][Mac (10,12)]
		[Async]
		[Export ("loadRecentPlayersWithCompletionHandler:")]
		void LoadRecentPlayers ([NullAllowed] Action<GKPlayer[], NSError> completionHandler);

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'LoadRecentPlayers' instead.")]
		[Export ("loadFriendsWithCompletionHandler:")]
		[Async]
		void LoadFriends ([NullAllowed] GKFriendsHandler handler);

		[Field ("GKPlayerAuthenticationDidChangeNotificationName")]
		[Notification]
		NSString AuthenticationDidChangeNotificationName { get; }

		[NullAllowed] // by default this property is null
		[Export ("authenticateHandler", ArgumentSemantic.Copy)]
#if WATCH
		[Watch (3,0)]
		Action<NSError> AuthenticateHandler { get; set; }
#elif !MONOMAC
		[iOS (6,0)]
		Action<UIViewController, NSError> AuthenticateHandler { get; set; }
#else
		[Mac (10,9)]
		Action<NSViewController, NSError> AuthenticateHandler { get; set; }
#endif

		[iOS (7,0)][Mac (10,10)] // Mismarked in header, 17613142
		[Export ("loadDefaultLeaderboardIdentifierWithCompletionHandler:")]
		[Async]
		void LoadDefaultLeaderboardIdentifier ([NullAllowed] Action<string, NSError> completionHandler);

		[iOS (7,0)][Mac (10,10)] // Mismarked in header, 17613142
		[Export ("setDefaultLeaderboardIdentifier:completionHandler:")]
		[Async]
		void SetDefaultLeaderboardIdentifier (string leaderboardIdentifier, [NullAllowed] Action<NSError> completionHandler);

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'LoadDefaultLeaderboardIdentifier' instead.")]
		[Since (6,0)]
		[Export ("loadDefaultLeaderboardCategoryIDWithCompletionHandler:")]
		[Async]
		void LoadDefaultLeaderboardCategoryID ([NullAllowed] Action<string, NSError> completionHandler);

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'SetDefaultLeaderboardIdentifier' instead.")]
		[iOS (6,0), Mac (10,8)]
		[Export ("setDefaultLeaderboardCategoryID:completionHandler:")]
		[Async]
		void SetDefaultLeaderboardCategoryID ([NullAllowed] string categoryID, [NullAllowed] Action<NSError> completionHandler);

		[iOS (7,0), Mac (10,10)]
		[Export ("registerListener:")]
		void RegisterListener ([Protocolize] GKLocalPlayerListener listener);

		[iOS (7,0), Mac (10,10)]
		[Export ("unregisterListener:")]
		void UnregisterListener ([Protocolize] GKLocalPlayerListener listener);

		[iOS (7,0), Mac (10,10)]
		[Export ("unregisterAllListeners")]
		void UnregisterAllListeners ();

		[iOS (7,0), Mac (10,10)]
		[Async (ResultTypeName = "GKIdentityVerificationSignatureResult")]
		[Export ("generateIdentityVerificationSignatureWithCompletionHandler:")]
		void GenerateIdentityVerificationSignature ([NullAllowed] GKIdentityVerificationSignatureHandler completionHandler);

		[iOS (8,0), Mac (10,10)]
		[Async]
		[Export ("loadFriendPlayersWithCompletionHandler:")]
		void LoadFriendPlayers ([NullAllowed] Action<GKPlayer [], NSError> completionHandler);

		[NoWatch]
		[NoTV]
		[iOS (8,0)]
		[Mac (10,10)]
		[Export ("fetchSavedGamesWithCompletionHandler:")]
		void FetchSavedGames ([NullAllowed] Action<GKSavedGame[], NSError> handler);

		[NoWatch]
		[NoTV]
		[iOS (8,0)]
		[Mac (10,10)]
		[Export ("saveGameData:withName:completionHandler:")]
		void SaveGameData (NSData data, string name, [NullAllowed] Action<GKSavedGame, NSError> handler);

		[NoWatch]
		[NoTV]
		[iOS (8,0)]
		[Mac (10,10)]
		[Export ("deleteSavedGamesWithName:completionHandler:")]
		void DeleteSavedGames (string name, [NullAllowed] Action<NSError> handler);

		[NoWatch]
		[NoTV]
		[iOS (8,0)]
		[Mac (10,10)]
		[Export ("resolveConflictingSavedGames:withData:completionHandler:")]
		void ResolveConflictingSavedGames (GKSavedGame [] conflictingSavedGames, NSData data, [NullAllowed] Action<GKSavedGame[], NSError> handler);
	}

	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[Mac (10,10)] // dyld: Symbol not found: _OBJC_CLASS_$_GKSavedGame in 10.9
	[BaseType (typeof (NSObject))]
	interface GKSavedGame : NSCopying {
		[Export ("name")]
		string Name { get; }

		[Export ("deviceName")]
		string DeviceName { get; }

		[Export ("modificationDate")]
		NSDate ModificationDate { get; }

		[Export ("loadDataWithCompletionHandler:")]
		[Async]
		void LoadData ([NullAllowed] Action<NSData, NSError> handler);
	}

	[NoWatch]
	[NoTV]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface GKSavedGameListener {
		[Export ("player:didModifySavedGame:")]
		void DidModifySavedGame (GKPlayer player, GKSavedGame savedGame);

		[Export ("player:hasConflictingSavedGames:")]
		void HasConflictingSavedGames (GKPlayer player, GKSavedGame [] savedGames);
	}

	[NoWatch]
	[Since (4,2)]
	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(GKMatchDelegate)})]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[__NSCFDictionary setObject:forKey:]: attempt to insert nil value (key: 1500388194)
	// <quote>Your application never directly allocates GKMatch objects.</quote> http://developer.apple.com/library/ios/#documentation/GameKit/Reference/GKMatch_Ref/Reference/Reference.html
	[DisableDefaultCtor]
	interface GKMatch {
		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'Players' instead.")]
		[Export ("playerIDs")]
		string [] PlayersIDs { get;  }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GKMatchDelegate Delegate { get; set;  }
		
		[Export ("expectedPlayerCount")]
		nint ExpectedPlayerCount { get;  }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'SendDataToAllPlayers (NSData, GKPlayer[] players, GKMatchSendDataMode mode, NSError error)' instead.")]
		[Export ("sendData:toPlayers:withDataMode:error:")]
		// OOPS: bug we shipped with and can not realistically fix, but good news: this is deprecated (the NSError should have been an out)
		bool SendData (NSData data, string [] players, GKMatchSendDataMode mode, out NSError error);

		[Export ("sendDataToAllPlayers:withDataMode:error:")]
		bool SendDataToAllPlayers (NSData data, GKMatchSendDataMode mode, out NSError error);

		[Export ("disconnect")]
		void Disconnect ();

		[Export ("voiceChatWithName:")]
		GKVoiceChat VoiceChatWithName (string name);

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'ChooseBestHostingPlayer' instead.")]
		[iOS (6,0), Mac (10,9)]
		[Export ("chooseBestHostPlayerWithCompletionHandler:")]
		[Async]
		void ChooseBestHostPlayer (Action<string> completionHandler);

		[Since (6,0)][Mac (10,9)]
		[Export ("rematchWithCompletionHandler:")]
		[Async]
		void Rematch ([NullAllowed] Action<GKMatch, NSError> completionHandler);

		[iOS (8,0), Mac (10,10)]
		[Export ("players")]
		GKPlayer [] Players { get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("chooseBestHostingPlayerWithCompletionHandler:")]
		[Async]
		void ChooseBestHostingPlayer (Action<GKPlayer> completionHandler);

		[iOS (8,0), Mac(10,10)]
		[Export ("sendData:toPlayers:dataMode:error:")]
		bool SendData (NSData data, GKPlayer [] players, GKMatchSendDataMode mode, out NSError error);
	}

	[NoWatch]
	[Since (4,2)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKMatchDelegate {

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'DataReceivedFromPlayer (GKMatch,NSData,GKPlayer)' instead.")]
		[Export ("match:didReceiveData:fromPlayer:"), EventArgs ("GKData")]
		void DataReceived (GKMatch match, NSData data, string playerId);

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'StateChangedForPlayer (GKMatch,GKPlayer,GKPlayerConnectionState)' instead.")]
		[Export ("match:player:didChangeState:"), EventArgs ("GKState")]
		void StateChanged (GKMatch match, string playerId, GKPlayerConnectionState state);

#if MONOMAC
#if !XAMCORE_4_0
		// This API was removed or never existed. Can't cleanly remove due to EventsArgs/Delegate
		[Obsolete ("It will never be called.")]
		[Export ("xamarin:selector:removed:"), EventArgs ("GKPlayerError")]
		void ConnectionFailed (GKMatch match, string playerId, NSError error);
#endif
#elif !XAMCORE_2_0
		// but Apple now reject applications using this selector
		// it's not easy to remove from the bindings because it's a delegate, with events and *EventArgs
		// so we fake a selector (that Apple won't check) and let the generator do it's job
		// note: not worth throwing an exception using a [PreSnippet] since the code already throws a 
		//       You_Should_Not_Call_base_In_This_Method (so it would not be generated)
		[Obsolete ("It will never be called.")]
		[Export ("xamarin:selector:removed:"), EventArgs ("GKPlayerError")]
		void ConnectionFailed (GKMatch match, string playerId, NSError error);
#endif

		[Export ("match:didFailWithError:"), EventArgs ("GKError")]
		void Failed (GKMatch match, [NullAllowed] NSError error);

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'ShouldReinviteDisconnectedPlayer' instead.")]
		[Since (5,0)]
		[Export ("match:shouldReinvitePlayer:"), DelegateName ("GKMatchReinvitation"), DefaultValue (true)]
		bool ShouldReinvitePlayer (GKMatch match, string playerId);

		[iOS (8,0), Mac (10,10)]
		[Export ("match:didReceiveData:fromRemotePlayer:"), EventArgs ("GKMatchReceivedDataFromRemotePlayer")]
		void DataReceivedFromPlayer (GKMatch match, NSData data, GKPlayer player);

		[iOS (4,1), Mac (10,8)] //Yes, the header file says it was available in 4.1 and 10.8...
		[Export ("match:player:didChangeConnectionState:"), EventArgs ("GKMatchConnectionChanged")]
		void StateChangedForPlayer (GKMatch match, GKPlayer player, GKPlayerConnectionState state);

		[iOS (8,0), Mac (10,10)]
		[Export ("match:shouldReinviteDisconnectedPlayer:")]
		[DelegateName ("GKMatchReinvitationForDisconnectedPlayer"), DefaultValue (true)]
		bool ShouldReinviteDisconnectedPlayer (GKMatch match, GKPlayer player);

		[Availability (Introduced = Platform.iOS_9_0 | Platform.Mac_10_11)]
		[Export ("match:didReceiveData:forRecipient:fromRemotePlayer:"), EventArgs ("GKDataReceivedForRecipient")]
		void DataReceivedForRecipient (GKMatch match, NSData data, GKPlayer recipient, GKPlayer player);
	}

	[NoWatch]
	[Since (4,2)]
	[BaseType (typeof (NSObject))]
	interface GKVoiceChat {
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get;  }

		[Export ("active", ArgumentSemantic.Assign)]
		bool Active { [Bind ("isActive")] get; set;  }

		[Export ("volume", ArgumentSemantic.Assign)]
		float Volume { get; set;  } /* float, not CGFloat */

		[Export ("start")]
		void Start ();

		[Export ("stop")]
		void Stop ();

		[NoTV]
		// the API was removed in iOS8
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'SetMuteStatus' instead.")]
		[Export ("setMute:forPlayer:")]
		void SetMute (bool isMuted, string playerID);

		[Static]
		[Export ("isVoIPAllowed")]
		bool IsVoIPAllowed ();

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'SetPlayerVoiceChatStateChangeHandler' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("playerStateUpdateHandler", ArgumentSemantic.Copy)]
		GKPlayerStateUpdateHandler PlayerStateUpdateHandler { get; set; }
		//void SetPlayerStateUpdateHandler (GKPlayerStateUpdateHandler handler);

		[iOS (8,0)][Mac (10,10)]
		[Export ("setPlayerVoiceChatStateDidChangeHandler:", ArgumentSemantic.Copy)]
		void SetPlayerVoiceChatStateChangeHandler (Action<GKPlayer,GKVoiceChatPlayerState> handler);

		[NoTV]
		[Since (5,0)]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'Players' instead.")]
		[Export ("playerIDs")]
		string [] PlayerIDs { get; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("players")]
		GKPlayer [] Players { get; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("setPlayer:muted:")]
		void SetMuteStatus (GKPlayer player, bool isMuted);
	}

	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	interface GKMatchRequest {
		[Export ("minPlayers", ArgumentSemantic.Assign)]
		nint MinPlayers { get; set;  }

		[Export ("maxPlayers", ArgumentSemantic.Assign)]
		nint MaxPlayers { get; set;  }

		[Export ("playerGroup", ArgumentSemantic.Assign)]
		nint PlayerGroup { get; set;  }

		[Export ("playerAttributes", ArgumentSemantic.Assign)]
		uint PlayerAttributes { get; set;  } /* uint32_t */

		[NoTV]
		[NoWatch]
		[NullAllowed] // by default this property is null
		[Export ("playersToInvite", ArgumentSemantic.Retain)]
		string [] PlayersToInvite { get; set;  }

		[Since (6,0)][Mac (10,8)]
		[NullAllowed] // by default this property is null
		[Export ("inviteMessage", ArgumentSemantic.Copy)]
		string InviteMessage { get; set; }

		[Since (6,0)][Mac (10,8)]
		[Export ("defaultNumberOfPlayers", ArgumentSemantic.Assign)]
		nint DefaultNumberOfPlayers { get; set; }

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'RecipientResponseHandler' instead.")]
		[Since (6,0)]
		[NullAllowed] // by default this property is null
		[Export ("inviteeResponseHandler", ArgumentSemantic.Copy)]
		Action<string, GKInviteeResponse> InviteeResponseHandler { get; set; }

		[iOS (8,0)][Mac (10,10)]
		[NullAllowed, Export ("recipientResponseHandler", ArgumentSemantic.Copy)]
		Action<GKPlayer, GKInviteRecipientResponse> RecipientResponseHandler { get; set; }

		[Since (6,0)][Mac (10,9)]
		[Export ("maxPlayersAllowedForMatchOfType:"), Static]
		nint GetMaxPlayersAllowed (GKMatchType matchType);

		[iOS (8,0), Mac (10,10)]
		[NullAllowed] // by default this property is null
		[Export ("recipients", ArgumentSemantic.Retain)]
		GKPlayer [] Recipients { get; set; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface GKInvite {

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'Sender' instead.")]
		[Export ("inviter", ArgumentSemantic.Retain)]
		string Inviter { get;  }

		[Export ("hosted", ArgumentSemantic.Assign)]
		bool Hosted { [Bind ("isHosted")] get;  }

		[iOS (6,0), Mac (10,9)]
		[Export ("playerGroup")]
		nint PlayerGroup { get; }

		[iOS (6,0), Mac (10,9)]
		[Export ("playerAttributes")]
		uint PlayerAttributes { get; } /* uint32_t */

		[iOS (8,0), Mac (10,10)]
		[Export ("sender", ArgumentSemantic.Retain)]
		GKPlayer Sender { get; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface GKMatchmaker {
		[Static]
		[Export ("sharedMatchmaker")]
		GKMatchmaker SharedMatchmaker { get; }

		[NoTV]
		[NullAllowed, Export ("inviteHandler", ArgumentSemantic.Copy)]
		GKInviteHandler InviteHandler { get; set; }

		[Export ("findMatchForRequest:withCompletionHandler:")]
		[Async]
		void FindMatch (GKMatchRequest request, [NullAllowed] GKNotificationMatch matchHandler);

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'FindPlayersForHostedRequest' instead.")]
		[Export ("findPlayersForHostedMatchRequest:withCompletionHandler:")]
		[Async]
		void FindPlayers (GKMatchRequest request, [NullAllowed] GKFriendsHandler playerHandler);

		[Export ("addPlayersToMatch:matchRequest:completionHandler:")]
		[Async]
#if XAMCORE_2_0
		void AddPlayers (GKMatch toMatch, GKMatchRequest matchRequest, [NullAllowed] Action<NSError> completionHandler);
#else
		void AddPlayers (GKMatch toMatch, GKMatchRequest matchRequest, [NullAllowed] GKNotificationHandler completionHandler);
#endif

		[Export ("cancel")]
		void Cancel ();

		[Export ("queryPlayerGroupActivity:withCompletionHandler:")]
		[Async]
		void QueryPlayerGroupActivity (nint playerGroup, [NullAllowed] GKQueryHandler completionHandler);

		[Export ("queryActivityWithCompletionHandler:")]
		[Async]
		void QueryActivity ([NullAllowed] GKQueryHandler completionHandler);

		[NoWatch]
		[Since (6,0)][Mac (10,9)]
		[Export ("matchForInvite:completionHandler:")]
		[Async]
		void Match (GKInvite invite, [NullAllowed] Action<GKMatch, NSError> completionHandler);

		[NoTV]
		[Since (6,0)][Mac (10, 9)]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'CancelPendingInvite' instead.")]
		[Export ("cancelInviteToPlayer:")]
		void CancelInvite (string playerID);

		[Since (6,0)][Mac (10,9)]
		[Export ("finishMatchmakingForMatch:")]
		void FinishMatchmaking (GKMatch match);

		[NoTV]
		[Since (6,0)][Mac (10,9)]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, message: "Use 'StartBrowsingForNearbyPlayers(Action<GKPlayer, bool> handler)' instead.")]
		[Export ("startBrowsingForNearbyPlayersWithReachableHandler:")]
		void StartBrowsingForNearbyPlayers ([NullAllowed] Action<string, bool> reachableHandler);

		[Since (6,0)][Mac (10,9)]
		[Export ("stopBrowsingForNearbyPlayers")]
		void StopBrowsingForNearbyPlayers ();

		[iOS (8,0), Mac (10,10)]
		[Export ("cancelPendingInviteToPlayer:")]
		void CancelPendingInvite (GKPlayer player);

		[iOS (8,0), Mac (10,10)]
		[Export ("findPlayersForHostedRequest:withCompletionHandler:")]
		[Async]
		void FindPlayersForHostedRequest (GKMatchRequest request, [NullAllowed] Action<GKPlayer[], NSError> completionHandler);

		// Not truly an [Async] method since the handler can be called multiple times, for each player found
		[iOS (8,0), Mac (10,10)]
		[Export ("startBrowsingForNearbyPlayersWithHandler:")]
		void StartBrowsingForNearbyPlayers ([NullAllowed] Action<GKPlayer, bool> handler);
	}

	[NoWatch]
#if MONOMAC
	[BaseType (typeof (NSViewController), Delegates=new string [] { "WeakMatchmakerDelegate" }, Events=new Type [] {typeof(GKMatchmakerViewControllerDelegate)})]
	[Mac (10,8)]
#else
	[BaseType (typeof (UINavigationController), Delegates=new string [] { "WeakMatchmakerDelegate" }, Events=new Type [] {typeof(GKMatchmakerViewControllerDelegate)})]
	[Since (4,2)]
#endif
	// iOS 6 -> Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: <GKMatchmakerViewController: 0x16101160>: must use one of the designated initializers
	[DisableDefaultCtor]
	interface GKMatchmakerViewController {
		[Export ("matchmakerDelegate", ArgumentSemantic.Assign)]
		NSObject WeakMatchmakerDelegate { get; set; }
		
		[Wrap ("WeakMatchmakerDelegate")]
		[Protocolize]
		GKMatchmakerViewControllerDelegate MatchmakerDelegate { get; set;  }

		[Export ("matchRequest", ArgumentSemantic.Retain)]
		GKMatchRequest MatchRequest { get;  }

		[Export ("hosted", ArgumentSemantic.Assign)]
		bool Hosted { [Bind ("isHosted")] get; set;  }

		[Export ("initWithMatchRequest:")]
		IntPtr Constructor (GKMatchRequest request);

		[Export ("initWithInvite:")]
		IntPtr Constructor (GKInvite invite);

#if !MONOMAC
		[NoTV]
		[Availability (Deprecated = Platform.iOS_5_0, Message = "Use 'SetHostedPlayerConnected' instead.")]
		[Export ("setHostedPlayerReady:")]
		void SetHostedPlayerReady (string playerID);
#endif

		[NoTV]
		[Availability (Introduced = Platform.iOS_5_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
		[Export ("defaultInvitationMessage", ArgumentSemantic.Copy)]
		string DefaultInvitationMessage { get; set;  }

		[Since (5,0)]
		[Export ("addPlayersToMatch:")]
		void AddPlayersToMatch (GKMatch match);

		[NoTV]
		[Since (5,0)]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'SetHostedPlayerConnected (GKPlayer,bool)' instead.")]
		[Export ("setHostedPlayer:connected:")]
		void SetHostedPlayerConnected (string playerID, bool connected);

		[iOS (8,0), Mac (10,10)]
		[Export ("setHostedPlayer:didConnect:")]
		void SetHostedPlayerConnected (GKPlayer playerID, bool connected);
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	[Model]
	[Since (4,2)]
	[Protocol]
	interface GKMatchmakerViewControllerDelegate {
		[Abstract]
		[Export ("matchmakerViewControllerWasCancelled:")]
		void WasCancelled (GKMatchmakerViewController viewController);

		[Abstract]
		[Export ("matchmakerViewController:didFailWithError:"), EventArgs ("GKError")]
		void DidFailWithError (GKMatchmakerViewController viewController, NSError error);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("matchmakerViewController:didFindMatch:"), EventArgs ("GKMatch")]
		void DidFindMatch (GKMatchmakerViewController viewController, GKMatch match);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'DidFindHostedPlayers' instead.")]
		[Export ("matchmakerViewController:didFindPlayers:"), EventArgs ("GKPlayers")]
		void DidFindPlayers (GKMatchmakerViewController viewController, string [] playerIDs);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[iOS (8,0), Mac (10,10)]
		[Export ("matchmakerViewController:didFindHostedPlayers:"), EventArgs ("GKMatchmakingPlayers")]
		void DidFindHostedPlayers (GKMatchmakerViewController viewController, GKPlayer [] playerIDs);

		[NoTV]
		[Since (5,0)]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'HostedPlayerDidAccept' instead.")]
		[Export ("matchmakerViewController:didReceiveAcceptFromHostedPlayer:"), EventArgs ("GKPlayer")]
		void ReceivedAcceptFromHostedPlayer (GKMatchmakerViewController viewController, string playerID);

		[iOS (8,0), Mac (10,10)]
		[Export ("matchmakerViewController:hostedPlayerDidAccept:"), EventArgs ("GKMatchmakingPlayer")]
		void HostedPlayerDidAccept (GKMatchmakerViewController viewController, GKPlayer playerID);
	}

	[BaseType (typeof (NSObject))]
	[Since (4,2)][MountainLion]
	[Watch (3,0)]
	interface GKAchievement : NSSecureCoding {
		[NoTV]
		[Availability (Deprecated = Platform.iOS_6_0 | Platform.Mac_10_10, Message = "Use 'IsHidden' on the 'GKAchievementDescription' class instead.")]
		[Export ("hidden", ArgumentSemantic.Assign)]
		bool Hidden { [Bind ("isHidden")] get; }

		[NullAllowed] // by default this property is null
		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; set;  }

		[Export ("percentComplete", ArgumentSemantic.Assign)]
		double PercentComplete { get; set;  }

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
#if XAMCORE_2_0
		void ResetAchivements ([NullAllowed] Action<NSError> completionHandler);
#else
		void ResetAchivements ([NullAllowed] GKNotificationHandler completionHandler);
#endif
		
		[Export ("initWithIdentifier:")]
		IntPtr Constructor ([NullAllowed] string identifier);

#if !MONOMAC
		[iOS (7,0)]
		[Availability (Deprecated = Platform.iOS_8_0, Message = "Use 'ctor (string identifier, GKPlayer player)' instead.")]
		[Export ("initWithIdentifier:forPlayer:")]
		IntPtr Constructor ([NullAllowed] string identifier, string playerId);
#endif

		[Export ("reportAchievementWithCompletionHandler:")]
		[Async]
#if XAMCORE_2_0
		[NoWatch]
		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use ReportAchievements '(GKAchievement[] achievements, Action<NSError> completionHandler)' instead.")]
		void ReportAchievement ([NullAllowed] Action<NSError> completionHandler);
#else
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use ReportAchievements '(GKAchievement[] achievements, GKNotificationHandler completionHandler)' instead.")]
		void ReportAchievement ([NullAllowed] GKNotificationHandler completionHandler);
#endif

		[Since (5,0)]
		[Export ("showsCompletionBanner", ArgumentSemantic.Assign)]
		bool ShowsCompletionBanner { get; set;  }

		[iOS (6,0), Mac (10,8)]
		[Static]
		[Export ("reportAchievements:withCompletionHandler:")]
		[Async]
#if XAMCORE_2_0
		void ReportAchievements (GKAchievement[] achievements, [NullAllowed] Action<NSError> completionHandler);
#else
		void ReportAchievements (GKAchievement[] achievements, [NullAllowed] GKNotificationHandler completionHandler);
#endif

		[NoTV]
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Pass 'GKPlayers' to 'ChallengeComposeController(GKPlayer[] players, string message, ...)' and present the view controller instead.")]
		[iOS (6,0), Mac (10,8)]
		[Export ("issueChallengeToPlayers:message:")]
		void IssueChallengeToPlayers ([NullAllowed] string[] playerIDs, [NullAllowed] string message);

		[NoTV]
		[NoWatch]
		[iOS (6,0), Mac (10,8)]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Pass 'GKPlayers' to 'SelectChallengeablePlayers' instead.")]
		[Export ("selectChallengeablePlayerIDs:withCompletionHandler:")]
		[Async]
		void SelectChallengeablePlayerIDs ([NullAllowed] string[] playerIDs, [NullAllowed] Action<string[], NSError> completionHandler);

#if !MONOMAC
		[NoTV]
		[iOS (7,0)]
		[Availability (Deprecated = Platform.iOS_8_0, Message = "Use 'Player' instead.")]
		[Export ("playerID", ArgumentSemantic.Copy)]
		string PlayerID { 
			get;
#if !XAMCORE_2_0
			// binding bug - it should not have been exposed (and Apple now rejects it, desk #63237)
			// using [NotImplemented] makes generator emit a throw *and* does not use the selector!
			[NotImplemented] set;
#endif
		}
#endif

		[NoWatch]
		[iOS (7,0), Mac (10,10)]
		[Export ("reportAchievements:withEligibleChallenges:withCompletionHandler:"), Static]
		[Async]
		void ReportAchievements (GKAchievement[] achievements, [NullAllowed] GKChallenge[] challenges, [NullAllowed] Action<NSError> completionHandler);

		[NullAllowed]
		[iOS (8,0), Mac (10,10)]
		[Export ("player", ArgumentSemantic.Retain)]
		GKPlayer Player { get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("initWithIdentifier:player:")]
		IntPtr Constructor ([NullAllowed] string identifier, GKPlayer player);


#if MONOMAC
		[Mac (10,10)]
		[Async (ResultTypeName = "GKChallengeComposeResult")]
		[Export ("challengeComposeControllerWithMessage:players:completionHandler:")]
		NSViewController ChallengeComposeController ([NullAllowed] string message, GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler completionHandler);
#else
		[Async (ResultTypeName = "GKChallengeComposeResult")]
		[NoWatch]
		[iOS (8,0)]
		[Export ("challengeComposeControllerWithMessage:players:completionHandler:")]
		UIViewController ChallengeComposeController ([NullAllowed] string message, GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler completionHandler);
#endif
	
		[NoWatch]
		[iOS (8,0), Mac (10,10)]
		[Async]
		[Export ("selectChallengeablePlayers:withCompletionHandler:")]
		void SelectChallengeablePlayers (GKPlayer [] players, [NullAllowed] Action<GKPlayer [], NSError> completionHandler);

#if !MONOMAC
		[NoTV][NoWatch]
		[iOS (7,0)]
		[Availability (Deprecated = Platform.iOS_8_0)]
		[Export ("challengeComposeControllerWithPlayers:message:completionHandler:")]
		UIViewController ChallengeComposeController (GKPlayer [] playerIDs, [NullAllowed] string message, [NullAllowed] GKChallengeComposeHandler completionHandler);
#endif
	}

	[BaseType (typeof (NSObject))]
	[Since (4,2)][MountainLion]
	[Watch (3,0)]
	interface GKAchievementDescription : NSSecureCoding {
		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; }

		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; }

		[Export ("achievedDescription", ArgumentSemantic.Copy)]
		string AchievedDescription { get; }

		[Export ("unachievedDescription", ArgumentSemantic.Copy)]
		string UnachievedDescription { get; }

		[Export ("maximumPoints", ArgumentSemantic.Assign)]
		nint MaximumPoints { get; }

		[Export ("hidden", ArgumentSemantic.Assign)]
		bool Hidden { [Bind ("isHidden")] get; }

		[Static]
		[Export ("loadAchievementDescriptionsWithCompletionHandler:")]
		[Async]
		void LoadAchievementDescriptions ([NullAllowed] GKAchievementDescriptionHandler handler);

		[NoWatch]
		[Export ("loadImageWithCompletionHandler:")]
		[Async]
		void LoadImage ([NullAllowed] GKImageLoadedHandler imageLoadedHandler);
		
#if MONOMAC
		[Export ("image", ArgumentSemantic.Retain)]
		NSImage Image { get; }
		
		[Static]
		[Export ("incompleteAchievementImage")]
		NSImage IncompleteAchievementImage { get; }

		[Static]
		[Export ("placeholderCompletedAchievementImage")]
		NSImage PlaceholderCompletedAchievementImage { get; }

#else
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_8, Message = "Use 'LoadImage' instead.")]
		[Export ("image")]
		UIImage Image { get; }

		[NoWatch]
		[Static]
		[Export ("incompleteAchievementImage")]
		UIImage IncompleteAchievementImage { get; }

		[NoWatch]
		[Static]
		[Export ("placeholderCompletedAchievementImage")]
		UIImage PlaceholderCompletedAchievementImage { get; }

		[Since (6,0)]
		[Export ("groupIdentifier", ArgumentSemantic.Retain)]
		string GroupIdentifier { get; }

		[Since (6,0)]
		[Export ("replayable", ArgumentSemantic.Assign)]
		bool Replayable { [Bind ("isReplayable")] get; }
#endif
	}

	[NoWatch]
	[NoTV]
	[Availability (Introduced = Platform.iOS_4_1 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'GKGameCenterViewController' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoTV]
	interface GKAchievementViewControllerDelegate {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("achievementViewControllerDidFinish:")]
		void DidFinish (GKAchievementViewController viewController);
	}

	[NoTV][NoWatch]
	[Availability (Introduced = Platform.iOS_4_1 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'GKGameCenterViewController' instead.")]
#if MONOMAC
	[BaseType (typeof (GKGameCenterViewController), Events=new Type [] { typeof (GKAchievementViewControllerDelegate)}, Delegates=new string [] {"WeakDelegate"})]
	interface GKAchievementViewController 
#else
	[BaseType (typeof (GKGameCenterViewController), Events=new Type [] { typeof (GKAchievementViewControllerDelegate)}, Delegates=new string [] {"WeakDelegate"})]
	interface GKAchievementViewController : UIAppearance 
#endif
	{
		[Export ("achievementDelegate", ArgumentSemantic.Assign), NullAllowed]
#if !MONOMAC
		[Override]
#endif
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GKAchievementViewControllerDelegate Delegate { get; set; }
	}

#if MONOMAC
	[BaseType (typeof (NSResponder))]
	interface GKDialogController {
		[Export ("parentWindow", ArgumentSemantic.Assign)]
		NSWindow ParentWindow { get; set; }

		[Export ("presentViewController:")]
		bool PresentViewController (NSViewController viewController);

		[Export ("dismiss:")]
		void Dismiss (NSObject sender);

		[Static]
		[Export ("sharedDialogController")]
		GKDialogController SharedDialogController { get; }
	}
#endif

	[NoWatch]
#if MONOMAC
	[Mac (10,8)]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (NSViewController), Events=new Type [] { typeof (GKFriendRequestComposeViewControllerDelegate)}, Delegates=new string[] {"WeakComposeViewDelegate"})]
	interface GKFriendRequestComposeViewController 
#else
	[NoTV]
	[Since (4,2)]
	[Deprecated (PlatformName.iOS, 10, 0)]
	[BaseType (typeof (UINavigationController), Events=new Type [] { typeof (GKFriendRequestComposeViewControllerDelegate)}, Delegates=new string[] {"WeakComposeViewDelegate"})]
	interface GKFriendRequestComposeViewController : UIAppearance
#endif
	{
		[Export ("composeViewDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakComposeViewDelegate { get; set; }

		[Wrap ("WeakComposeViewDelegate")]
		[Protocolize]
		GKFriendRequestComposeViewControllerDelegate ComposeViewDelegate { get; set; }

		[Export ("maxNumberOfRecipients")][Static]
		nint MaxNumberOfRecipients { get; }
		
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'AddRecipientPlayers' instead.")]
		[Export ("addRecipientsWithEmailAddresses:")]
		void AddRecipientsFromEmails (string [] emailAddresses);

		[iOS (8,0), Mac (10,10)]
		[Export ("addRecipientPlayers:")]
		void AddRecipientPlayers ([NullAllowed]GKPlayer [] players);

		[Export ("addRecipientsWithPlayerIDs:")]
		void AddRecipientsFromPlayerIDs (string [] playerIDs);

#if XAMCORE_2_0
		[Export ("setMessage:")]
		void SetMessage ([NullAllowed] string message);
#else
		[Export ("message")]
		string Message { set; }
#endif
	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKFriendRequestComposeViewControllerDelegate {
		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[Abstract]
		[Export ("friendRequestComposeViewControllerDidFinish:")]
		void DidFinish (GKFriendRequestComposeViewController viewController);
	}

	[NoWatch]
	[Since (5,0)]
	[BaseType(typeof(NSObject))]
	partial interface GKNotificationBanner {
		[Static, Export ("showBannerWithTitle:message:completionHandler:")]
		[Async]
		void Show ([NullAllowed] string title, [NullAllowed] string message, [NullAllowed] NSAction onCompleted);

		[Since (6,0)][Mac (10,8)]
		[Export ("showBannerWithTitle:message:duration:completionHandler:"), Static]
		[Async]
		void Show ([NullAllowed] string title, [NullAllowed] string message, double durationSeconds, [NullAllowed] Action completionHandler);
	}

	[Since (5,0)]
	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	interface GKTurnBasedParticipant {
		[iOS (8,0)][Mac (10,10)]
		[Export ("player", ArgumentSemantic.Retain)]
		GKPlayer Player { get; }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'Player' instead.")]
		[Export ("playerID", ArgumentSemantic.Copy)]
		string PlayerID { get;  }

		[Export ("lastTurnDate", ArgumentSemantic.Copy)]
		NSDate LastTurnDate { get;  }

		[Export ("status")]
		GKTurnBasedParticipantStatus Status { get;  }

		[Export ("matchOutcome", ArgumentSemantic.Assign)]
		GKTurnBasedMatchOutcome MatchOutcome { get; set;  }

		[Since (6,0)][Mac (10,8)]
		[Export ("timeoutDate", ArgumentSemantic.Copy)]
		NSDate TimeoutDate { get; }
	}

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Availability (Introduced = Platform.iOS_5_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKTurnBasedEventListener'.")]
	[Watch (3,0)]
	interface GKTurnBasedEventHandlerDelegate {
#if !XAMCORE_2_0
		[Export ("handleInviteFromGameCenterDoNotUse:")]
		[Obsolete ("Use HandleInviteFromGameCenter(NSString[]).")]
		void HandleInviteFromGameCenter (GKPlayer [] playersToInvite);
#endif

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("handleInviteFromGameCenter:")]
		[Availability (Introduced = Platform.iOS_5_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
		void HandleInviteFromGameCenter (NSString [] playersToInvite);

		[Availability (Introduced = Platform.iOS_5_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_9, Message = "Use 'HandleTurnEvent' instead.")]
		[Export ("handleTurnEventForMatch:")]
		void HandleTurnEventForMatch (GKTurnBasedMatch match);

		[Availability (Introduced = Platform.iOS_6_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10)]
		[Export ("handleMatchEnded:")]
		void HandleMatchEnded (GKTurnBasedMatch match);

#if (XAMCORE_2_0 && !MONOMAC) || XAMCORE_4_0
		[Abstract]
#endif
		[Since (6,0)]
		[Export ("handleTurnEventForMatch:didBecomeActive:")]
		[Availability (Introduced = Platform.iOS_6_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_6_0 | Platform.Mac_10_10)]
		void HandleTurnEvent (GKTurnBasedMatch match, bool activated);
	}

	[NoTV]
	[Availability (Introduced = Platform.iOS_5_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use GKLocalPlayer.RegisterListener with an object that implements IGKTurnBasedEventListener.")]
	[BaseType (typeof (NSObject))]
	[Watch (3,0)]
	interface GKTurnBasedEventHandler {

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GKTurnBasedEventHandlerDelegate Delegate { get; set;  }

		[Export ("sharedTurnBasedEventHandler"), Static]
		GKTurnBasedEventHandler SharedTurnBasedEventHandler { get; }
	}

	[Since (5,0)]
	delegate void GKTurnBasedMatchRequest (GKTurnBasedMatch match, NSError error);

	[Since (5,0)]
	delegate void GKTurnBasedMatchesRequest (GKTurnBasedMatch [] matches, NSError error);

	[Since (5,0)]
	delegate void GKTurnBasedMatchData (NSData matchData, NSError error);

	[Since (5,0)]
	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	interface GKTurnBasedMatch {
		[Export ("matchID")]
		string MatchID { get;  }

		[Export ("creationDate")]
		NSDate CreationDate { get;  }

		[Export ("participants", ArgumentSemantic.Retain)]
		GKTurnBasedParticipant []Participants { get;  }

		[Export ("status")]
		GKTurnBasedMatchStatus Status { get;  }

		[Export ("currentParticipant", ArgumentSemantic.Retain)]
		GKTurnBasedParticipant CurrentParticipant { get;  }

		[Export ("matchData", ArgumentSemantic.Retain)]
		NSData MatchData { get;  }

		[NullAllowed] // by default this property is null
		[Export ("message", ArgumentSemantic.Copy)]
		string Message { get; set;  }

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
#if XAMCORE_2_0
		void Remove (Action<NSError> onCompletion);
#else
		void Remove (GKNotificationHandler onCompletion);
#endif

		[Export ("loadMatchDataWithCompletionHandler:")]
		[Async]
		void LoadMatchData ([NullAllowed] GKTurnBasedMatchData onCompletion);

		[NoTV]
		[Availability (Deprecated = Platform.iOS_6_0 | Platform.Mac_10_9, Message = "Use 'EndTurn' instead.")]
		[Export ("endTurnWithNextParticipant:matchData:completionHandler:")]
		[Async]
#if XAMCORE_2_0
		void EndTurnWithNextParticipant (GKTurnBasedParticipant nextParticipant, NSData matchData, [NullAllowed] Action<NSError> noCompletion);
#else
		void EndTurnWithNextParticipant (GKTurnBasedParticipant nextParticipant, NSData matchData, [NullAllowed] GKNotificationHandler noCompletion);
#endif

		[NoTV]
		[Availability (Deprecated = Platform.iOS_6_0 | Platform.Mac_10_9, Message = "Use 'ParticipantQuitInTurn (GKTurnBasedMatchOutcome, GKTurnBasedParticipant[], double, NSData, Action<NSError>)' instead.")]
		[Export ("participantQuitInTurnWithOutcome:nextParticipant:matchData:completionHandler:")]
		[Async]
#if XAMCORE_2_0
		void ParticipantQuitInTurn (GKTurnBasedMatchOutcome matchOutcome, GKTurnBasedParticipant nextParticipant, NSData matchData, [NullAllowed] Action<NSError> onCompletion);
#else
		void ParticipantQuitInTurn (GKTurnBasedMatchOutcome matchOutcome, GKTurnBasedParticipant nextParticipant, NSData matchData, [NullAllowed] GKNotificationHandler onCompletion);
#endif

		[Export ("participantQuitOutOfTurnWithOutcome:withCompletionHandler:")]
		[Async]
#if XAMCORE_2_0
		void ParticipantQuitOutOfTurn (GKTurnBasedMatchOutcome matchOutcome, [NullAllowed] Action<NSError> onCompletion);
#else
		void ParticipantQuitOutOfTurn (GKTurnBasedMatchOutcome matchOutcome, [NullAllowed] GKNotificationHandler onCompletion);
#endif

		[Export ("endMatchInTurnWithMatchData:completionHandler:")]
		[Async]
#if XAMCORE_2_0
		void EndMatchInTurn (NSData matchData, [NullAllowed] Action<NSError> onCompletion);
#else
		void EndMatchInTurn (NSData matchData, [NullAllowed] GKNotificationHandler onCompletion);
#endif

		[Since (6,0)][Mac (10,8)]
		[Static]
		[Export ("loadMatchWithID:withCompletionHandler:")]
		[Async]
		void LoadMatch (string matchId, [NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[Since (6,0)][Mac (10,8)]
		[Export ("acceptInviteWithCompletionHandler:")]
		[Async]
		void AcceptInvite ([NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[Since (6,0)][Mac (10,8)]
		[Export ("declineInviteWithCompletionHandler:")]
		[Async]
		void DeclineInvite ([NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[Since (6,0)][Mac (10,8)]
		[Export ("matchDataMaximumSize")]
		nint MatchDataMaximumSize { get; }

		[Since (6,0)][Mac (10,9)]
		[Export ("rematchWithCompletionHandler:")]
		[Async]
		void Rematch ([NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[Since (6,0)][Mac (10,9)]
		[Export ("endTurnWithNextParticipants:turnTimeout:matchData:completionHandler:")]
		[Async]
		void EndTurn (GKTurnBasedParticipant[] nextParticipants, double timeoutSeconds, NSData matchData, [NullAllowed] Action<NSError> completionHandler);

		[Since (6,0)][Mac (10,9)]
		[Export ("participantQuitInTurnWithOutcome:nextParticipants:turnTimeout:matchData:completionHandler:")]
		[Async]
		void ParticipantQuitInTurn (GKTurnBasedMatchOutcome matchOutcome, GKTurnBasedParticipant[] nextParticipants, double timeoutSeconds, NSData matchData, [NullAllowed] Action<NSError> completionHandler);

		[Since (6,0)][Mac (10,8)]
		[Export ("saveCurrentTurnWithMatchData:completionHandler:")]
		[Async]
		void SaveCurrentTurn (NSData matchData, [NullAllowed] Action<NSError> completionHandler);

		[Since (6,0)][Mac (10,9)]
		[Field ("GKTurnTimeoutDefault"), Static]
		double DefaultTimeout { get; }

		[Since (6,0)][Mac (10,9)]
		[Field ("GKTurnTimeoutNone"), Static]
		double NoTimeout { get; }

		[Since (7,0)][Mac (10,10)]
		[Export ("exchanges", ArgumentSemantic.Retain)]
		GKTurnBasedExchange [] Exchanges { get; }

		[Since (7,0)][Mac (10,10)]
		[Export ("activeExchanges", ArgumentSemantic.Retain)]
		GKTurnBasedExchange [] ActiveExchanges { get; }

		[Since (7,0)][Mac (10,10)]
		[Export ("completedExchanges", ArgumentSemantic.Retain)]
		GKTurnBasedExchange [] CompletedExchanges { get; }

		[Since (7,0)][Mac (10,10)]
		[Export ("exchangeDataMaximumSize")]
		nuint ExhangeDataMaximumSize { get; }

		[Since (7,0)][Mac (10,10)]
		[Export ("exchangeMaxInitiatedExchangesPerPlayer")]
		nuint ExchangeMaxInitiatedExchangesPerPlayer { get; }

		[Since (7,0)][Mac (10,10)]
		[Export ("setLocalizableMessageWithKey:arguments:")]
		void SetMessage (string localizableMessage, params NSObject [] arguments);

		[Since (7,0)][Mac (10,10)]
		[Export ("endMatchInTurnWithMatchData:scores:achievements:completionHandler:")]
		[Async]
		void EndMatchInTurn (NSData matchData, [NullAllowed] GKScore [] scores, [NullAllowed] GKAchievement [] achievements, [NullAllowed] Action<NSError> completionHandler);

		[Since (7,0)][Mac (10,10)]
		[Export ("saveMergedMatchData:withResolvedExchanges:completionHandler:")]
		[Async]
		void SaveMergedMatchData (NSData matchData, GKTurnBasedExchange [] exchanges, [NullAllowed] Action<NSError> completionHandler);

		[Since (7,0)][Mac (10,10)]
		[Export ("sendExchangeToParticipants:data:localizableMessageKey:arguments:timeout:completionHandler:")]
		[Async]
		void SendExchange (GKTurnBasedParticipant [] participants, NSData data, string localizableMessage, NSObject [] arguments, double timeout, [NullAllowed] Action<GKTurnBasedExchange, NSError> completionHandler);

		[Since (7,0)][Mac (10,10)]
		[Export ("sendReminderToParticipants:localizableMessageKey:arguments:completionHandler:")]
		[Async]
		void SendReminder (GKTurnBasedParticipant [] participants, string localizableMessage, NSObject [] arguments, [NullAllowed] Action<NSError> completionHandler);
	}

	[NoWatch]
	// iOS6 -> Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: <GKTurnBasedMatchmakerViewController: 0x18299df0>: must use one of the designated initializers
	[DisableDefaultCtor]
#if MONOMAC
	[Mac (10,8)]
	[BaseType (typeof (NSViewController))]
	interface GKTurnBasedMatchmakerViewController
#else
	[Since (5,0)]
	[BaseType (typeof (UINavigationController))]
	interface GKTurnBasedMatchmakerViewController : UIAppearance
#endif
		{
		[Export ("showExistingMatches", ArgumentSemantic.Assign)]
		bool ShowExistingMatches { get; set;  }

		[Export ("initWithMatchRequest:")]
		IntPtr Constructor (GKMatchRequest request);

		[Export ("turnBasedMatchmakerDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GKTurnBasedMatchmakerViewControllerDelegate Delegate { get; set; }
	}

	[NoWatch]
	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKTurnBasedMatchmakerViewControllerDelegate {
		[Abstract]
		[Export ("turnBasedMatchmakerViewControllerWasCancelled:")]
		void WasCancelled (GKTurnBasedMatchmakerViewController viewController);

		[Abstract]
		[Export ("turnBasedMatchmakerViewController:didFailWithError:")]
		void FailedWithError (GKTurnBasedMatchmakerViewController viewController, NSError error);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[NoTV]
		[Availability (Deprecated = Platform.iOS_9_0 | Platform.Mac_10_11, Message = "Use 'GKTurnBasedEventListener.ReceivedTurnEvent' instead.")]
		[Export ("turnBasedMatchmakerViewController:didFindMatch:")]
		void FoundMatch (GKTurnBasedMatchmakerViewController viewController, GKTurnBasedMatch match);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[NoTV]
		[Availability (Deprecated = Platform.iOS_9_0 | Platform.Mac_10_11, Message = "Use 'GKTurnBasedEventListener.WantsToQuitMatch' instead.")]
		[Export ("turnBasedMatchmakerViewController:playerQuitForMatch:")]
		void PlayerQuitForMatch (GKTurnBasedMatchmakerViewController viewController, GKTurnBasedMatch match);
	}

	[NoWatch]
	[Since (6,0)][Mavericks]
	[BaseType (typeof (NSObject))]
	interface GKChallenge : NSSecureCoding {
		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'IssuingPlayer' instead.")]
		[Export ("issuingPlayerID", ArgumentSemantic.Copy)]
		string IssuingPlayerID { get; }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use 'ReceivingPlayer' instead.")]
		[Export ("receivingPlayerID", ArgumentSemantic.Copy)]
		string ReceivingPlayerID { get; }

		[Export ("state", ArgumentSemantic.Assign)]
		GKChallengeState State { get; }

		[Export ("issueDate", ArgumentSemantic.Retain)]
		NSDate IssueDate { get; }

		[Export ("completionDate", ArgumentSemantic.Retain)]
		NSDate CompletionDate { get; }

		[Export ("message", ArgumentSemantic.Copy)]
		string Message { get; }

		[Export ("decline")]
		void Decline ();

		[Export ("loadReceivedChallengesWithCompletionHandler:"), Static]
		[Async]
		void LoadReceivedChallenges ([NullAllowed] Action<GKChallenge[], NSError> completionHandler);

		[iOS (8,0), Mac (10,10)]
		[Export ("issuingPlayer", ArgumentSemantic.Copy)]
		GKPlayer IssuingPlayer { get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("receivingPlayer", ArgumentSemantic.Copy)]
		GKPlayer ReceivingPlayer { get; }
	}

	[NoWatch]
	[Since (6,0)][Mavericks]
	[BaseType (typeof (GKChallenge))]
	interface GKScoreChallenge {

		[Export ("score", ArgumentSemantic.Retain)]
		GKScore Score { get; }
	}

	[NoWatch]
	[Since (6,0)][Mavericks]
	[BaseType (typeof (GKChallenge))]
	interface GKAchievementChallenge {

		[Export ("achievement", ArgumentSemantic.Retain)]
		GKAchievement Achievement { get; }
	}

	[NoWatch]
	[Since (6,0), Mac (10,9)]
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
	{
		[Export ("gameCenterDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GKGameCenterControllerDelegate Delegate { get; set;  }

		[NoTV]
		[Export ("viewState", ArgumentSemantic.Assign)]
		GKGameCenterViewControllerState ViewState { get; set; }

		[NoTV]
		[Export ("leaderboardTimeScope", ArgumentSemantic.Assign)]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "This class no longer support 'LeaderboardTimeScope', will always default to 'AllTime'.")]
		GKLeaderboardTimeScope LeaderboardTimeScope { get; set; }

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("leaderboardCategory", ArgumentSemantic.Retain)]
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Use 'LeaderboardIdentifier' instead.")]
		string LeaderboardCategory { get; set; }

		[NoTV]
		[iOS (7,0)][Mac (10,10)] // Marked 10.9 in header, apple 17612948
		[NullAllowed] // by default this property is null
		[Export ("leaderboardIdentifier")]
		string LeaderboardIdentifier { get; set; }
	}

	[NoWatch]
	[Since (6,0)]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	interface GKGameCenterControllerDelegate
	{
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("gameCenterViewControllerDidFinish:")]
		void Finished (GKGameCenterViewController controller);
	}

#if !MONOMAC
	[NoWatch]
	[NoTV]
	[Availability (Introduced = Platform.iOS_6_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[BaseType (typeof (NSObject), Events=new[] { typeof (GKChallengeEventHandlerDelegate) }, Delegates=new[] { "WeakDelegate"})]
	[DisableDefaultCtor]
	[NoTV]
	interface GKChallengeEventHandler
	{
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GKChallengeEventHandlerDelegate Delegate { get; set;  }

		[Export ("challengeEventHandler"), Static]
		GKChallengeEventHandler Instance { get; }
	}

	[NoWatch]
	[NoTV]
	[Availability (Introduced = Platform.iOS_6_0 | Platform.Mac_10_8, Deprecated = Platform.iOS_7_0 | Platform.Mac_10_10, Message = "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	[NoTV]
	interface GKChallengeEventHandlerDelegate
	{
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
#endif

	[iOS (7,0), Mac (10,10)]
	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	interface GKTurnBasedExchange
	{
		[Export ("exchangeID")]
		string ExchangeID { get; }

		[Export ("sender")]
		GKTurnBasedParticipant Sender { get; }

		[Export ("recipients")]
		GKTurnBasedParticipant [] Recipients { get; }

		[Export ("status", ArgumentSemantic.Assign)]
		GKTurnBasedExchangeStatus Status { get; }

		[Export ("message")]
		string Message { get; }

		[Export ("data")]
		NSData Data { get; }

		[Export ("sendDate")]
		NSDate SendDate { get; }

		[Export ("timeoutDate")]
		NSDate TimeoutDate { get; }

		[Export ("completionDate")]
		NSDate CompletionDate { get; }

		[Export ("replies")]
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

	[iOS (7,0), Mac (10,10)]
	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	interface GKTurnBasedExchangeReply
	{
		[Export ("recipient")]
		GKTurnBasedParticipant Recipient { get; }

		[Export ("message")]
		string Message { get; }

		[Export ("data")]
		NSData Data { get; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("replyDate")]
		NSDate ReplyDate { get; }
	}

	[Since (7,0), Mac (10,10)]
	[Watch (3,0)]
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
	[Since (7,0), Mac (10,10)]
	[Model, Protocol, BaseType (typeof (NSObject))]
	interface GKChallengeListener
	{
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
	[Since (7,0), Mac (10,10)]
	[Protocol, Model, BaseType (typeof (NSObject))]
	interface GKInviteEventListener
	{
		[Mac (10,10)][iOS (7,0)]
		[Export ("player:didAcceptInvite:")]
		void DidAcceptInvite (GKPlayer player, GKInvite invite);

#if !MONOMAC
		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0, Message = "Use 'DidRequestMatch (GKPlayer player, GKPlayer[] recipientPlayers)' instead.")]
		[Export ("player:didRequestMatchWithPlayers:")]
		void DidRequestMatch (GKPlayer player, string[] playerIDs);
#endif
		[iOS (8,0), Mac (10,10)]
		[Export ("player:didRequestMatchWithRecipients:")]
		void DidRequestMatch (GKPlayer player, GKPlayer [] recipientPlayers);
	}

	[Since (7,0), Mac (10,10)]
	[Watch (3,0)]
	[Model, Protocol, BaseType (typeof (NSObject))]
	interface GKTurnBasedEventListener
	{
		[NoWatch]
		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0, Message = "Use 'DidRequestMatchWithOtherPlayers' instead.")]
		[Export ("player:didRequestMatchWithPlayers:")]
		void DidRequestMatchWithPlayers (GKPlayer player, string[] playerIDsToInvite);

		[Export ("player:receivedTurnEventForMatch:didBecomeActive:")]
		void ReceivedTurnEvent (GKPlayer player, GKTurnBasedMatch match, bool becameActive);

		[Export ("player:matchEnded:")]
		void MatchEnded (GKPlayer player, GKTurnBasedMatch match);

		[Export ("player:receivedExchangeRequest:forMatch:")]
		void ReceivedExchangeRequest (GKPlayer player, GKTurnBasedExchange exchange, GKTurnBasedMatch match);

		[Export ("player:receivedExchangeCancellation:forMatch:")]
		void ReceivedExchangeCancellation (GKPlayer player, GKTurnBasedExchange exchange, GKTurnBasedMatch match);

		[Export ("player:receivedExchangeReplies:forCompletedExchange:forMatch:")]
		void ReceivedExchangeReplies (GKPlayer player, GKTurnBasedExchangeReply[] replies, GKTurnBasedExchange exchange, GKTurnBasedMatch match);

		[NoWatch]
		[iOS (8,0)]
		[Export ("player:didRequestMatchWithOtherPlayers:")]
		void DidRequestMatchWithOtherPlayers (GKPlayer player, GKPlayer [] playersToInvite);

		[Availability (Introduced = Platform.iOS_9_0 | Platform.Mac_10_11)]
		[Export ("player:wantsToQuitMatch:")]
		void WantsToQuitMatch (GKPlayer player, GKTurnBasedMatch match);
	}

	[NoWatch]
	[iOS (10,0)][Mac (10,12)][TV (10,0)]
	[BaseType (typeof(NSObject))]
	interface GKGameSession
	{
		[Export ("identifier")]
		string Identifier { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("owner")]
		GKCloudPlayer Owner { get; }

		[Export ("players")]
		GKCloudPlayer[] Players { get; }

		[Export ("lastModifiedDate")]
		NSDate LastModifiedDate { get; }

		[Export ("lastModifiedPlayer")]
		GKCloudPlayer LastModifiedPlayer { get; }

		[Export ("maxNumberOfConnectedPlayers")]
		nint MaxNumberOfConnectedPlayers { get; }

		[Export ("badgedPlayers")]
		GKCloudPlayer[] BadgedPlayers { get; }

		[Async]
		[Static]
		[Export ("createSessionInContainer:withTitle:maxConnectedPlayers:completionHandler:")]
		void CreateSession ([NullAllowed] string containerName, string title, nint maxPlayers, Action<GKGameSession, NSError> completionHandler);

		[Async]
		[Static]
		[Export ("loadSessionsInContainer:completionHandler:")]
		void LoadSessions ([NullAllowed] string containerName, Action<GKGameSession[], NSError> completionHandler);

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
		GKCloudPlayer[] GetPlayers (GKConnectionState state);

		[Async]
		[Export ("sendData:withTransportType:completionHandler:")]
		void SendData (NSData data, GKTransportType transport, Action<NSError> completionHandler);

		[Async]
		[Export ("sendMessageWithLocalizedFormatKey:arguments:data:toPlayers:badgePlayers:completionHandler:")]
		void SendMessage (string key, string[] arguments, NSData data, GKCloudPlayer[] players, bool badgePlayers, Action<NSError> completionHandler);

		[Async]
		[Export ("clearBadgeForPlayers:completionHandler:")]
		void ClearBadge (GKCloudPlayer[] players, Action<NSError> completionHandler);

		[Static]
		[Export ("addEventListener:")]
		void AddEventListener (IGKGameSessionEventListener listener);

		[Static]
		[Export ("removeEventListener:")]
		void RemoveEventListener (IGKGameSessionEventListener listener);

		// From GKGameSession (GKGameSessionEventListenerPrivate)

		[Static]
		[Export ("postSession:didAddPlayer:")]
		void DidAddPlayer (GKGameSession session, GKCloudPlayer player);

		[Static]
		[Export ("postSession:didRemovePlayer:")]
		void DidRemovePlayer (GKGameSession session, GKCloudPlayer player);

		[Static]
		[Export ("postSession:player:didChangeConnectionState:")]
		void DidChangeConnectionState (GKGameSession session, GKCloudPlayer player, GKConnectionState newState);

		[Static]
		[Export ("postSession:player:didSaveData:")]
		void DidSaveData (GKGameSession session, GKCloudPlayer player, NSData data);

		[Static]
		[Export ("postSession:didReceiveData:fromPlayer:")]
		void DidReceiveData (GKGameSession session, NSData data, GKCloudPlayer player);

		[Static]
		[Export ("postSession:didReceiveMessage:withData:fromPlayer:")]
		void DidReceiveMessage (GKGameSession session, string message, NSData data, GKCloudPlayer player);
	}

	interface IGKGameSessionEventListener {}

	[NoWatch]
	[iOS (10,0)][Mac (10,12)][TV (10,0)]
	[Protocol]
	interface GKGameSessionEventListener
	{
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

#if !MONOMAC
	[NoWatch]
	[NoiOS][TV (10,0)]
	[BaseType (typeof(UIViewController))]
	interface GKGameSessionSharingViewController
	{
		// inlined ctor
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("session", ArgumentSemantic.Strong)]
		GKGameSession Session { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IGKGameSessionSharingViewControllerDelegate Delegate { get; set; }

		[Export ("initWithSession:")]
		IntPtr Constructor (GKGameSession session);
	}

	interface IGKGameSessionSharingViewControllerDelegate {}

	[NoWatch]
	[NoiOS][TV (10,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface GKGameSessionSharingViewControllerDelegate
	{
		[Abstract]
		[Export ("sharingViewController:didFinishWithError:")]
		void DidFinish (GKGameSessionSharingViewController viewController, [NullAllowed] NSError error);
	}
#endif
}
