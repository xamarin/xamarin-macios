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
using ObjCRuntime;
using Foundation;
using CoreFoundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
using NSViewController = Foundation.NSObject;
#endif

namespace GameKit {

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
	[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'MCBrowserViewController' from the 'MultipeerConnectivity' framework instead.")]
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
	[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'GKVoiceChat' instead.")]
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
#endif

	[NoTV]
	[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'MultipeerConnectivity.MCSession' instead.")]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'MultipeerConnectivity.MCSession' instead.")]
	interface GKSession {
		[Export ("initWithSessionID:displayName:sessionMode:")]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
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

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
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
		[Export ("sendData:toPeers:withDataMode:error:")]
#if XAMCORE_2_0
		bool SendData (NSData data, string [] peers, GKSendDataMode mode, out NSError error);
#else
		bool SendData (NSData data, string [] peers, GKSendDataMode mode, IntPtr ns_error_out);
#endif

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
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

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Export ("peersWithConnectionState:")]
		string [] PeersWithConnectionState (GKPeerConnectionState state);
	}

	[Watch (3,0)]
	[Mac (10, 8)]
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
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'Identifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'Identifier' instead.")]
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
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'ctor (GKPlayer [] players)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'ctor (GKPlayer [] players)' instead.")]
		[Export ("initWithPlayerIDs:")]
		IntPtr Constructor ([NullAllowed] string [] players);

		[Export ("loadScoresWithCompletionHandler:")]
		[Async]
		void LoadScores ([NullAllowed] GKScoresLoadedHandler scoresLoadedHandler);

		[NoTV]
		[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
		[Deprecated (PlatformName.iOS, 6, 0, message : "Use 'LoadLeaderboards' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use 'LoadLeaderboards' instead.")]
		[Static]
		[Export ("loadCategoriesWithCompletionHandler:")]
		[Async (ResultTypeName = "GKCategoryResult")]
		void LoadCategories ([NullAllowed] GKCategoryHandler categoryHandler);

		[NoTV]
		[NoWatch] // deprecated in 2.0 (but framework not added before 3.0)
		[Static]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'SetDefaultLeaderboard' on 'GKLocalPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'SetDefaultLeaderboard' on 'GKLocalPlayer' instead.")]
		[Export ("setDefaultLeaderboard:withCompletionHandler:")]
		[Async]
#if XAMCORE_2_0
		void SetDefaultLeaderboard ([NullAllowed] string leaderboardIdentifier, [NullAllowed] Action<NSError> notificationHandler);
#else
		void SetDefaultLeaderboard ([NullAllowed] string leaderboardIdentifier, [NullAllowed] GKNotificationHandler notificationHandler);
#endif

		[iOS (6,0)]
		[Export ("groupIdentifier", ArgumentSemantic.Retain)]
		string GroupIdentifier { get; [NotImplemented] set; }

		[iOS (6,0)]
		[Static]
		[Export ("loadLeaderboardsWithCompletionHandler:")]
		[Async]
		void LoadLeaderboards ([NullAllowed] Action<GKLeaderboard[], NSError> completionHandler);

		[iOS (7,0)][Mac (10,10)]
		[NullAllowed]
		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; set; }

		[NoTV]
		[NoWatch]
		[iOS (7,0)][Mac (10,8)]
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
	[iOS (7,0)][Mac (10,10)]
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
	[BaseType (typeof (GKBasePlayer))]
	// note: NSSecureCoding conformity is undocumented - but since it's a runtime check (on ObjC) we still need it
	interface GKPlayer : NSSecureCoding {
		[Export ("playerID", ArgumentSemantic.Retain)]
		string PlayerID { get;  }

		[Export ("alias", ArgumentSemantic.Copy)]
		string Alias { get;  }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'GKLocalPlayer.LoadFriendPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'GKLocalPlayer.LoadFriendPlayers' instead.")]
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
		[Export ("loadPhotoForSize:withCompletionHandler:")]
		[Async]
		void LoadPhoto (GKPhotoSize size, [NullAllowed] GKPlayerPhotoLoaded onCompleted);

		[iOS (6,0)][Mac (10,8)]
		[Export ("displayName")]
		string DisplayName { get; }

		[NoWatch]
		[iOS (9, 0)]
		[Mac (10, 11)]
		[Static]
		[Export ("anonymousGuestPlayerWithIdentifier:")]
		GKPlayer GetAnonymousGuestPlayer (string guestIdentifier);

		[NoWatch]
		[iOS (9, 0)]
		[Mac (10, 11)]
		[Export ("guestIdentifier")]
		string GuestIdentifier { get; }
	}

	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	interface GKScore : NSSecureCoding {
		[NoWatch]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'InitWithLeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'InitWithLeaderboardIdentifier' instead.")]
		[Internal][NullAllowed]
		[Export ("initWithCategory:")]
		IntPtr InitWithCategory ([NullAllowed] string category);

		[iOS (8,0)][Mac (10,10)]
		[Export ("initWithLeaderboardIdentifier:player:")]
		IntPtr Constructor (string identifier, GKPlayer player);

		[NoWatch]
		[iOS (7,0)][Mac (10,8)]
		[Export ("initWithLeaderboardIdentifier:forPlayer:")]
		IntPtr Constructor (string identifier, string playerID);

		[iOS (7,0)][Mac (10,8)]
		[Internal][NullAllowed]
		[Export ("initWithLeaderboardIdentifier:")]
		IntPtr InitWithLeaderboardIdentifier (string identifier);

#if !XAMCORE_2_0
		[NoWatch]
		// [Deprecated (PlatformName.iOS, 8, 0, message : "Use 'Player' instead.")] - Unlike rest of deprecations we are just ripping out due to poor naming
		// [Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'Player' instead.")] - Unlike rest of deprecations we are just ripping out due to poor naming
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
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'LeaderboardIdentifier' instead.")]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'LeaderboardIdentifier' instead.")]
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
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'ReportScores' instead.")]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'ReportScores' instead.")]
		[Export ("reportScoreWithCompletionHandler:")]
		[Async]
#if XAMCORE_2_0
		void ReportScore ([NullAllowed] Action<NSError> errorHandler);
#else
		void ReportScore ([NullAllowed] GKNotificationHandler errorHandler);
#endif

		[Export ("context", ArgumentSemantic.Assign)]
		ulong Context { get; set; }

		[Export ("shouldSetDefaultLeaderboard", ArgumentSemantic.Assign)]
		bool ShouldSetDefaultLeaderboard { get; set; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ... )' and present the view controller instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ... )' and present the view controller instead.")]
		[iOS (6,0), Mac (10,8)]
		[Export ("issueChallengeToPlayers:message:")]
		void IssueChallengeToPlayers ([NullAllowed] string[] playerIDs, [NullAllowed] string message);

		[iOS (6,0)]
		[Export ("reportScores:withCompletionHandler:"), Static]
		[Async]
		void ReportScores (GKScore[] scores, [NullAllowed] Action<NSError> completionHandler);

		[iOS (7,0)][Mac (10,10)]
		[NullAllowed] // by default this property is null
		[Export ("leaderboardIdentifier", ArgumentSemantic.Copy)]
		string LeaderboardIdentifier { get; set; }

		[NoWatch]
		[iOS (7,0)][Mac (10,10)]
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
	[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'GKGameCenterViewController' instead.")]
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
	[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'GKGameCenterViewController' instead.")]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'GKGameCenterViewController' instead.")]
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
	[Mac (10, 8)]
	[BaseType (typeof (GKPlayer))]
	interface GKLocalPlayer
#if !TVOS && !WATCH // from GKSavedGame category
		: GKSavedGameListener
#endif
	{
		[Export ("authenticated")]
		bool Authenticated { [Bind ("isAuthenticated")] get;  }

		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'LoadFriendPlayers' instead and collect the friends from the invoked callback.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'LoadFriendPlayers' instead and collect the friends from the invoked callback.")]
		[Export ("friends", ArgumentSemantic.Retain)]
		string [] Friends { get;  }

		[Static, Export ("localPlayer")]
		GKLocalPlayer LocalPlayer { get; }

		[Export ("isUnderage")]
		bool IsUnderage { get; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 6, 0, message : "Set the 'AuthenticationHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message : "Set the 'AuthenticationHandler' instead.")]
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
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'LoadRecentPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'LoadRecentPlayers' instead.")]
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
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'LoadDefaultLeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'LoadDefaultLeaderboardIdentifier' instead.")]
		[iOS (6,0)]
		[Export ("loadDefaultLeaderboardCategoryIDWithCompletionHandler:")]
		[Async]
		void LoadDefaultLeaderboardCategoryID ([NullAllowed] Action<string, NSError> completionHandler);

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'SetDefaultLeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'SetDefaultLeaderboardIdentifier' instead.")]
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
	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(GKMatchDelegate)})]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[__NSCFDictionary setObject:forKey:]: attempt to insert nil value (key: 1500388194)
	// <quote>Your application never directly allocates GKMatch objects.</quote> http://developer.apple.com/library/ios/#documentation/GameKit/Reference/GKMatch_Ref/Reference/Reference.html
	[DisableDefaultCtor]
	interface GKMatch {
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'Players' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'Players' instead.")]
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
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'SendDataToAllPlayers (NSData, GKPlayer[] players, GKMatchSendDataMode mode, NSError error)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'SendDataToAllPlayers (NSData, GKPlayer[] players, GKMatchSendDataMode mode, NSError error)' instead.")]
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
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'ChooseBestHostingPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'ChooseBestHostingPlayer' instead.")]
		[iOS (6,0), Mac (10,9)]
		[Export ("chooseBestHostPlayerWithCompletionHandler:")]
		[Async]
		void ChooseBestHostPlayer (Action<string> completionHandler);

		[iOS (6,0)][Mac (10,9)]
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
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKMatchDelegate {

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'DataReceivedFromPlayer (GKMatch,NSData,GKPlayer)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'DataReceivedFromPlayer (GKMatch,NSData,GKPlayer)' instead.")]
		[Export ("match:didReceiveData:fromPlayer:"), EventArgs ("GKData")]
		void DataReceived (GKMatch match, NSData data, string playerId);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'StateChangedForPlayer (GKMatch,GKPlayer,GKPlayerConnectionState)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'StateChangedForPlayer (GKMatch,GKPlayer,GKPlayerConnectionState)' instead.")]
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
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'ShouldReinviteDisconnectedPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'ShouldReinviteDisconnectedPlayer' instead.")]
		[Export ("match:shouldReinvitePlayer:"), DelegateName ("GKMatchReinvitation"), DefaultValue (true)]
		bool ShouldReinvitePlayer (GKMatch match, string playerId);

		[iOS (8,0), Mac (10,10)]
		[Export ("match:didReceiveData:fromRemotePlayer:"), EventArgs ("GKMatchReceivedDataFromRemotePlayer")]
		void DataReceivedFromPlayer (GKMatch match, NSData data, GKPlayer player);

		[Mac (10,8)] //Yes, the header file says it was available in 4.1 and 10.8...
		[Export ("match:player:didChangeConnectionState:"), EventArgs ("GKMatchConnectionChanged")]
		void StateChangedForPlayer (GKMatch match, GKPlayer player, GKPlayerConnectionState state);

		[iOS (8,0), Mac (10,10)]
		[Export ("match:shouldReinviteDisconnectedPlayer:")]
		[DelegateName ("GKMatchReinvitationForDisconnectedPlayer"), DefaultValue (true)]
		bool ShouldReinviteDisconnectedPlayer (GKMatch match, GKPlayer player);

		[iOS (9, 0)]
		[Mac (10, 11)]
		[Export ("match:didReceiveData:forRecipient:fromRemotePlayer:"), EventArgs ("GKDataReceivedForRecipient")]
		void DataReceivedForRecipient (GKMatch match, NSData data, GKPlayer recipient, GKPlayer player);
	}

	[NoWatch]
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
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'SetMuteStatus' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'SetMuteStatus' instead.")]
		[Export ("setMute:forPlayer:")]
		void SetMute (bool isMuted, string playerID);

		[Static]
		[Export ("isVoIPAllowed")]
		bool IsVoIPAllowed ();

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'SetPlayerVoiceChatStateChangeHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'SetPlayerVoiceChatStateChangeHandler' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("playerStateUpdateHandler", ArgumentSemantic.Copy)]
		GKPlayerStateUpdateHandler PlayerStateUpdateHandler { get; set; }
		//void SetPlayerStateUpdateHandler (GKPlayerStateUpdateHandler handler);

		[iOS (8,0)][Mac (10,10)]
		[Export ("setPlayerVoiceChatStateDidChangeHandler:", ArgumentSemantic.Copy)]
		void SetPlayerVoiceChatStateChangeHandler (Action<GKPlayer,GKVoiceChatPlayerState> handler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'Players' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'Players' instead.")]
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

		[iOS (6,0)][Mac (10,8)]
		[NullAllowed] // by default this property is null
		[Export ("inviteMessage", ArgumentSemantic.Copy)]
		string InviteMessage { get; set; }

		[iOS (6,0)][Mac (10,8)]
		[Export ("defaultNumberOfPlayers", ArgumentSemantic.Assign)]
		nint DefaultNumberOfPlayers { get; set; }

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'RecipientResponseHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'RecipientResponseHandler' instead.")]
		[iOS (6,0)]
		[NullAllowed] // by default this property is null
		[Export ("inviteeResponseHandler", ArgumentSemantic.Copy)]
		Action<string, GKInviteeResponse> InviteeResponseHandler { get; set; }

		[iOS (8,0)][Mac (10,10)]
		[NullAllowed, Export ("recipientResponseHandler", ArgumentSemantic.Copy)]
		Action<GKPlayer, GKInviteRecipientResponse> RecipientResponseHandler { get; set; }

		[iOS (6,0)][Mac (10,9)]
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
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'Sender' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'Sender' instead.")]
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
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'FindPlayersForHostedRequest' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'FindPlayersForHostedRequest' instead.")]
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
		[iOS (6,0)][Mac (10,9)]
		[Export ("matchForInvite:completionHandler:")]
		[Async]
		void Match (GKInvite invite, [NullAllowed] Action<GKMatch, NSError> completionHandler);

		[NoTV]
		[iOS (6,0)][Mac (10, 9)]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'CancelPendingInvite' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'CancelPendingInvite' instead.")]
		[Export ("cancelInviteToPlayer:")]
		void CancelInvite (string playerID);

		[iOS (6,0)][Mac (10,9)]
		[Export ("finishMatchmakingForMatch:")]
		void FinishMatchmaking (GKMatch match);

		[NoTV]
		[iOS (6,0)][Mac (10,9)]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'StartBrowsingForNearbyPlayers(Action<GKPlayer, bool> handler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'StartBrowsingForNearbyPlayers(Action<GKPlayer, bool> handler)' instead.")]
		[Export ("startBrowsingForNearbyPlayersWithReachableHandler:")]
		void StartBrowsingForNearbyPlayers ([NullAllowed] Action<string, bool> reachableHandler);

		[iOS (6,0)][Mac (10,9)]
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
#endif
	// iOS 6 -> Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: <GKMatchmakerViewController: 0x16101160>: must use one of the designated initializers
	[DisableDefaultCtor]
	interface GKMatchmakerViewController
#if MONOMAC
	: GKViewController
#endif
	{
		[NoiOS]
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);
		
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
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Export ("defaultInvitationMessage", ArgumentSemantic.Copy)]
		string DefaultInvitationMessage { get; set;  }

		[Export ("addPlayersToMatch:")]
		void AddPlayersToMatch (GKMatch match);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'SetHostedPlayerConnected (GKPlayer,bool)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'SetHostedPlayerConnected (GKPlayer,bool)' instead.")]
		[Export ("setHostedPlayer:connected:")]
		void SetHostedPlayerConnected (string playerID, bool connected);

		[iOS (8,0), Mac (10,10)]
		[Export ("setHostedPlayer:didConnect:")]
		void SetHostedPlayerConnected (GKPlayer playerID, bool connected);
	}

	[NoWatch]
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

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("matchmakerViewController:didFindMatch:"), EventArgs ("GKMatch")]
		void DidFindMatch (GKMatchmakerViewController viewController, GKMatch match);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'DidFindHostedPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'DidFindHostedPlayers' instead.")]
		[Export ("matchmakerViewController:didFindPlayers:"), EventArgs ("GKPlayers")]
		void DidFindPlayers (GKMatchmakerViewController viewController, string [] playerIDs);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[iOS (8,0), Mac (10,10)]
		[Export ("matchmakerViewController:didFindHostedPlayers:"), EventArgs ("GKMatchmakingPlayers")]
		void DidFindHostedPlayers (GKMatchmakerViewController viewController, GKPlayer [] playerIDs);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'HostedPlayerDidAccept' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'HostedPlayerDidAccept' instead.")]
		[Export ("matchmakerViewController:didReceiveAcceptFromHostedPlayer:"), EventArgs ("GKPlayer")]
		void ReceivedAcceptFromHostedPlayer (GKMatchmakerViewController viewController, string playerID);

		[iOS (8,0), Mac (10,10)]
		[Export ("matchmakerViewController:hostedPlayerDidAccept:"), EventArgs ("GKMatchmakingPlayer")]
		void HostedPlayerDidAccept (GKMatchmakerViewController viewController, GKPlayer playerID);
	}

	[BaseType (typeof (NSObject))]
	[Mac (10, 8)]
	[Watch (3,0)]
	interface GKAchievement : NSSecureCoding {
		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0, message : "Use 'IsHidden' on the 'GKAchievementDescription' class instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'IsHidden' on the 'GKAchievementDescription' class instead.")]
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
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use ReportAchievements '(GKAchievement[] achievements, Action<NSError> completionHandler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use ReportAchievements '(GKAchievement[] achievements, Action<NSError> completionHandler)' instead.")]
		void ReportAchievement ([NullAllowed] Action<NSError> completionHandler);
#else
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use ReportAchievements '(GKAchievement[] achievements, GKNotificationHandler completionHandler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use ReportAchievements '(GKAchievement[] achievements, GKNotificationHandler completionHandler)' instead.")]
		void ReportAchievement ([NullAllowed] GKNotificationHandler completionHandler);
#endif

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
		[Deprecated (PlatformName.iOS, 7, 0, message : "Pass 'GKPlayers' to 'ChallengeComposeController(GKPlayer[] players, string message, ...)' and present the view controller instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Pass 'GKPlayers' to 'ChallengeComposeController(GKPlayer[] players, string message, ...)' and present the view controller instead.")]
		[iOS (6,0), Mac (10,8)]
		[Export ("issueChallengeToPlayers:message:")]
		void IssueChallengeToPlayers ([NullAllowed] string[] playerIDs, [NullAllowed] string message);

		[NoTV]
		[NoWatch]
		[iOS (6,0), Mac (10,8)]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Pass 'GKPlayers' to 'SelectChallengeablePlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Pass 'GKPlayers' to 'SelectChallengeablePlayers' instead.")]
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
	[Mac (10, 8)]
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

		[iOS (6,0), Mac (10,8)]
		[Export ("groupIdentifier", ArgumentSemantic.Retain)]
		string GroupIdentifier { get; }

		[iOS (6,0), Mac (10,8)]
		[Export ("replayable", ArgumentSemantic.Assign)]
		bool Replayable { [Bind ("isReplayable")] get; }
		
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
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'LoadImage' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message : "Use 'LoadImage' instead.")]
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
#endif
	}

	[NoWatch]
	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'GKGameCenterViewController' instead.")]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'GKGameCenterViewController' instead.")]
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
	[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'GKGameCenterViewController' instead.")]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'GKGameCenterViewController' instead.")]
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
	interface GKFriendRequestComposeViewController : GKViewController {
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);
#else
	[NoTV]
	[Deprecated (PlatformName.iOS, 10, 0)]
	[BaseType (typeof (UINavigationController), Events=new Type [] { typeof (GKFriendRequestComposeViewControllerDelegate)}, Delegates=new string[] {"WeakComposeViewDelegate"})]
	interface GKFriendRequestComposeViewController : UIAppearance {
#endif
		[Export ("composeViewDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakComposeViewDelegate { get; set; }

		[Wrap ("WeakComposeViewDelegate")]
		[Protocolize]
		GKFriendRequestComposeViewControllerDelegate ComposeViewDelegate { get; set; }

		[Export ("maxNumberOfRecipients")][Static]
		nint MaxNumberOfRecipients { get; }
		
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'AddRecipientPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'AddRecipientPlayers' instead.")]
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
	[BaseType(typeof(NSObject))]
	partial interface GKNotificationBanner {
		[Static, Export ("showBannerWithTitle:message:completionHandler:")]
		[Async]
		void Show ([NullAllowed] string title, [NullAllowed] string message, [NullAllowed] Action onCompleted);

		[iOS (6,0)][Mac (10,8)]
		[Export ("showBannerWithTitle:message:duration:completionHandler:"), Static]
		[Async]
		void Show ([NullAllowed] string title, [NullAllowed] string message, double durationSeconds, [NullAllowed] Action completionHandler);
	}

	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	interface GKTurnBasedParticipant {
		[iOS (8,0)][Mac (10,10)]
		[Export ("player", ArgumentSemantic.Retain)]
		GKPlayer Player { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'Player' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'Player' instead.")]
		[Export ("playerID", ArgumentSemantic.Copy)]
		string PlayerID { get;  }

		[Export ("lastTurnDate", ArgumentSemantic.Copy)]
		NSDate LastTurnDate { get;  }

		[Export ("status")]
		GKTurnBasedParticipantStatus Status { get;  }

		[Export ("matchOutcome", ArgumentSemantic.Assign)]
		GKTurnBasedMatchOutcome MatchOutcome { get; set;  }

		[iOS (6,0)][Mac (10,8)]
		[Export ("timeoutDate", ArgumentSemantic.Copy)]
		NSDate TimeoutDate { get; }
	}

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKTurnBasedEventListener'.")]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKTurnBasedEventListener'.")]
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
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		void HandleInviteFromGameCenter (NSString [] playersToInvite);

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'HandleTurnEvent' instead.")]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use 'HandleTurnEvent' instead.")]
		[Export ("handleTurnEventForMatch:")]
		void HandleTurnEventForMatch (GKTurnBasedMatch match);

		[iOS (6, 0)]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Export ("handleMatchEnded:")]
		void HandleMatchEnded (GKTurnBasedMatch match);

#if (XAMCORE_2_0 && !MONOMAC) || XAMCORE_4_0
		[Abstract]
#endif
		[iOS (6,0)]
		[Export ("handleTurnEventForMatch:didBecomeActive:")]
		[iOS (6, 0)]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[Mac (10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		void HandleTurnEvent (GKTurnBasedMatch match, bool activated);
	}

	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message : "Use GKLocalPlayer.RegisterListener with an object that implements IGKTurnBasedEventListener.")]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use GKLocalPlayer.RegisterListener with an object that implements IGKTurnBasedEventListener.")]
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

	delegate void GKTurnBasedMatchRequest (GKTurnBasedMatch match, NSError error);

	delegate void GKTurnBasedMatchesRequest (GKTurnBasedMatch [] matches, NSError error);

	delegate void GKTurnBasedMatchData (NSData matchData, NSError error);

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
		[Deprecated (PlatformName.iOS, 6, 0, message : "Use 'EndTurn' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use 'EndTurn' instead.")]
		[Export ("endTurnWithNextParticipant:matchData:completionHandler:")]
		[Async]
#if XAMCORE_2_0
		void EndTurnWithNextParticipant (GKTurnBasedParticipant nextParticipant, NSData matchData, [NullAllowed] Action<NSError> noCompletion);
#else
		void EndTurnWithNextParticipant (GKTurnBasedParticipant nextParticipant, NSData matchData, [NullAllowed] GKNotificationHandler noCompletion);
#endif

		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0, message : "Use 'ParticipantQuitInTurn (GKTurnBasedMatchOutcome, GKTurnBasedParticipant[], double, NSData, Action<NSError>)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use 'ParticipantQuitInTurn (GKTurnBasedMatchOutcome, GKTurnBasedParticipant[], double, NSData, Action<NSError>)' instead.")]
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

		[iOS (6,0)][Mac (10,8)]
		[Static]
		[Export ("loadMatchWithID:withCompletionHandler:")]
		[Async]
		void LoadMatch (string matchId, [NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[iOS (6,0)][Mac (10,8)]
		[Export ("acceptInviteWithCompletionHandler:")]
		[Async]
		void AcceptInvite ([NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[iOS (6,0)][Mac (10,8)]
		[Export ("declineInviteWithCompletionHandler:")]
		[Async]
		void DeclineInvite ([NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[iOS (6,0)][Mac (10,8)]
		[Export ("matchDataMaximumSize")]
		nint MatchDataMaximumSize { get; }

		[iOS (6,0)][Mac (10,9)]
		[Export ("rematchWithCompletionHandler:")]
		[Async]
		void Rematch ([NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[iOS (6,0)][Mac (10,9)]
		[Export ("endTurnWithNextParticipants:turnTimeout:matchData:completionHandler:")]
		[Async]
		void EndTurn (GKTurnBasedParticipant[] nextParticipants, double timeoutSeconds, NSData matchData, [NullAllowed] Action<NSError> completionHandler);

		[iOS (6,0)][Mac (10,9)]
		[Export ("participantQuitInTurnWithOutcome:nextParticipants:turnTimeout:matchData:completionHandler:")]
		[Async]
		void ParticipantQuitInTurn (GKTurnBasedMatchOutcome matchOutcome, GKTurnBasedParticipant[] nextParticipants, double timeoutSeconds, NSData matchData, [NullAllowed] Action<NSError> completionHandler);

		[iOS (6,0)][Mac (10,8)]
		[Export ("saveCurrentTurnWithMatchData:completionHandler:")]
		[Async]
		void SaveCurrentTurn (NSData matchData, [NullAllowed] Action<NSError> completionHandler);

		[iOS (6,0)][Mac (10,9)]
		[Field ("GKTurnTimeoutDefault"), Static]
		double DefaultTimeout { get; }

		[iOS (6,0)][Mac (10,9)]
		[Field ("GKTurnTimeoutNone"), Static]
		double NoTimeout { get; }

		[iOS (7,0)][Mac (10,10)]
		[Export ("exchanges", ArgumentSemantic.Retain)]
		GKTurnBasedExchange [] Exchanges { get; }

		[iOS (7,0)][Mac (10,10)]
		[Export ("activeExchanges", ArgumentSemantic.Retain)]
		GKTurnBasedExchange [] ActiveExchanges { get; }

		[iOS (7,0)][Mac (10,10)]
		[Export ("completedExchanges", ArgumentSemantic.Retain)]
		GKTurnBasedExchange [] CompletedExchanges { get; }

		[iOS (7,0)][Mac (10,10)]
		[Export ("exchangeDataMaximumSize")]
		nuint ExhangeDataMaximumSize { get; }

		[iOS (7,0)][Mac (10,10)]
		[Export ("exchangeMaxInitiatedExchangesPerPlayer")]
		nuint ExchangeMaxInitiatedExchangesPerPlayer { get; }

		[iOS (7,0)][Mac (10,10)]
		[Export ("setLocalizableMessageWithKey:arguments:")]
		void SetMessage (string localizableMessage, params NSObject [] arguments);

		[iOS (7,0)][Mac (10,10)]
		[Export ("endMatchInTurnWithMatchData:scores:achievements:completionHandler:")]
		[Async]
		void EndMatchInTurn (NSData matchData, [NullAllowed] GKScore [] scores, [NullAllowed] GKAchievement [] achievements, [NullAllowed] Action<NSError> completionHandler);

		[iOS (7,0)][Mac (10,10)]
		[Export ("saveMergedMatchData:withResolvedExchanges:completionHandler:")]
		[Async]
		void SaveMergedMatchData (NSData matchData, GKTurnBasedExchange [] exchanges, [NullAllowed] Action<NSError> completionHandler);

		[iOS (7,0)][Mac (10,10)]
		[Export ("sendExchangeToParticipants:data:localizableMessageKey:arguments:timeout:completionHandler:")]
		[Async]
		void SendExchange (GKTurnBasedParticipant [] participants, NSData data, string localizableMessage, NSObject [] arguments, double timeout, [NullAllowed] Action<GKTurnBasedExchange, NSError> completionHandler);

		[iOS (7,0)][Mac (10,10)]
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
	interface GKTurnBasedMatchmakerViewController : GKViewController
#else
	[BaseType (typeof (UINavigationController))]
	interface GKTurnBasedMatchmakerViewController : UIAppearance
#endif
		{
		[NoiOS]
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

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
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'GKTurnBasedEventListener.ReceivedTurnEvent' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'GKTurnBasedEventListener.ReceivedTurnEvent' instead.")]
		[Export ("turnBasedMatchmakerViewController:didFindMatch:")]
		void FoundMatch (GKTurnBasedMatchmakerViewController viewController, GKTurnBasedMatch match);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'GKTurnBasedEventListener.WantsToQuitMatch' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'GKTurnBasedEventListener.WantsToQuitMatch' instead.")]
		[Export ("turnBasedMatchmakerViewController:playerQuitForMatch:")]
		void PlayerQuitForMatch (GKTurnBasedMatchmakerViewController viewController, GKTurnBasedMatch match);
	}

	[NoWatch]
	[iOS (6,0)][Mac (10, 9)]
	[BaseType (typeof (NSObject))]
	interface GKChallenge : NSSecureCoding {
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'IssuingPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'IssuingPlayer' instead.")]
		[Export ("issuingPlayerID", ArgumentSemantic.Copy)]
		string IssuingPlayerID { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'ReceivingPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'ReceivingPlayer' instead.")]
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
	[iOS (6,0)][Mac (10, 9)]
	[BaseType (typeof (GKChallenge))]
	interface GKScoreChallenge {

		[Export ("score", ArgumentSemantic.Retain)]
		GKScore Score { get; }
	}

	[NoWatch]
	[iOS (6,0)][Mac (10, 9)]
	[BaseType (typeof (GKChallenge))]
	interface GKAchievementChallenge {

		[Export ("achievement", ArgumentSemantic.Retain)]
		GKAchievement Achievement { get; }
	}

	[NoWatch]
	[iOS (6,0), Mac (10,9)]
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
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);
		
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
		[Deprecated (PlatformName.iOS, 7, 0, message : "This class no longer support 'LeaderboardTimeScope', will always default to 'AllTime'.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "This class no longer support 'LeaderboardTimeScope', will always default to 'AllTime'.")]
		GKLeaderboardTimeScope LeaderboardTimeScope { get; set; }

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("leaderboardCategory", ArgumentSemantic.Retain)]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'LeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'LeaderboardIdentifier' instead.")]
		string LeaderboardCategory { get; set; }

		[NoTV]
		[iOS (7,0)][Mac (10,10)] // Marked 10.9 in header, apple 17612948
		[NullAllowed] // by default this property is null
		[Export ("leaderboardIdentifier")]
		string LeaderboardIdentifier { get; set; }
	}

	[NoWatch]
	[iOS (6,0)]
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

	[NoWatch]
	[NoTV]
	[iOS (6, 0)]
	[Deprecated (PlatformName.iOS, 7, 0, message : "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
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
	[iOS (6, 0)]
	[Deprecated (PlatformName.iOS, 7, 0, message : "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[Mac (10, 8)]
	[Deprecated (PlatformName.MacOSX, 10, 10, message : "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
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

	[iOS (7,0), Mac (10,10)]
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
	[iOS (7,0), Mac (10,10)]
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
	[iOS (7,0), Mac (10,10)]
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

	[iOS (7,0), Mac (10,10)]
	[Watch (3,0)]
	[Model, Protocol, BaseType (typeof (NSObject))]
	interface GKTurnBasedEventListener
	{
#if XAMCORE_4_0		
		[NoMac]
#endif
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

		[iOS (9, 0)]
		[Mac (10, 11)]
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

		[Mac (10,13,4), TV (11,3), iOS (11,3)]
		[Static]
		[Export ("postSession:didAddPlayer:")]
		void DidAddPlayer (GKGameSession session, GKCloudPlayer player);

		[Mac (10,13,4), TV (11,3), iOS (11,3)]
		[Static]
		[Export ("postSession:didRemovePlayer:")]
		void DidRemovePlayer (GKGameSession session, GKCloudPlayer player);

		[Mac (10,13,4), TV (11,3), iOS (11,3)]
		[Static]
		[Export ("postSession:player:didChangeConnectionState:")]
		void DidChangeConnectionState (GKGameSession session, GKCloudPlayer player, GKConnectionState newState);

		[Mac (10,13,4), TV (11,3), iOS (11,3)]
		[Static]
		[Export ("postSession:player:didSaveData:")]
		void DidSaveData (GKGameSession session, GKCloudPlayer player, NSData data);

		[Mac (10,13,4), TV (11,3), iOS (11,3)]
		[Static]
		[Export ("postSession:didReceiveData:fromPlayer:")]
		void DidReceiveData (GKGameSession session, NSData data, GKCloudPlayer player);

		[Mac (10,13,4), TV (11,3), iOS (11,3)]
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
	interface IGKChallengesViewControllerDelegate { }

	[NoiOS, NoTV, NoWatch, Mac (10,8)]
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	interface GKChallengesViewControllerDelegate {
		
		[Abstract]
		[Export ("challengesViewControllerDidFinish:")]
		void DidFinish (GKChallengesViewController viewController);
	}

	[NoiOS, NoTV, NoWatch, Mac (10,8)]
	[Deprecated (PlatformName.MacOSX, 10,10)]
	[BaseType (typeof (NSViewController))]
	interface GKChallengesViewController : GKViewController {

		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
		
		[NullAllowed, Export ("challengeDelegate", ArgumentSemantic.Assign)]
		IGKChallengesViewControllerDelegate ChallengeDelegate { get; set; }
	}

	[NoiOS, NoTV, NoWatch, Mac (10,8)]
	[Protocol]
	interface GKViewController
	{
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
}
