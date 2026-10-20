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

using System.ComponentModel;

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

namespace GameKit {

	/// <summary>A delegate used with <see cref="GameKit.GKLocalPlayer.LoadFriends(GameKit.GKFriendsHandler)" /> and <see cref="GameKit.GKMatchmaker.FindPlayers(GameKit.GKMatchRequest,GameKit.GKFriendsHandler)" /> that defines behavior after the request completes.</summary>
	delegate void GKFriendsHandler ([NullAllowed] string [] friends, [NullAllowed] NSError error);
	/// <param name="players">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate used with <see cref="GameKit.GKPlayer.LoadPlayersForIdentifiers(System.String[],GameKit.GKPlayersHandler)" /> to specify behavior to happen after the players are loaded.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKPlayersHandler ([NullAllowed] GKPlayer [] players, [NullAllowed] NSError error);
	/// <param name="leaderboards">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>Completion handler for the <see cref="GameKit.GKLeaderboardSet.LoadLeaderboards(GameKit.GKLeaderboardsHandler)" /> method.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKLeaderboardsHandler ([NullAllowed] GKLeaderboard [] leaderboards, [NullAllowed] NSError error);
	/// <param name="scoreArray">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate used with <see cref="GameKit.GKLeaderboard.LoadScores(GameKit.GKScoresLoadedHandler)" /> that specifies behavior after the scores are loaded.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKScoresLoadedHandler ([NullAllowed] GKScore [] scoreArray, [NullAllowed] NSError error);
	/// <param name="match">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate used with <see cref="GameKit.GKMatchmaker.FindMatch(GameKit.GKMatchRequest,GameKit.GKNotificationMatch)" /> that specifies behavior after a match has been made.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKNotificationMatch ([NullAllowed] GKMatch match, [NullAllowed] NSError error);
	/// <summary>A delegate that is used to define behavior after a response to a <see cref="GameKit.GKInvite" />.</summary>
	delegate void GKInviteHandler (GKInvite invite, [NullAllowed] string [] playerIDs);
	/// <param name="activity">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate used with <see cref="GameKit.GKMatchmaker.QueryActivity(GameKit.GKQueryHandler)" /> and <see cref="GameKit.GKMatchmaker.QueryPlayerGroupActivity(System.IntPtr,GameKit.GKQueryHandler)" /> to specify behavior after the query is completed.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKQueryHandler (nint activity, [NullAllowed] NSError error);
	/// <param name="achivements">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate passed to <see cref="GameKit.GKAchievement.LoadAchievements(GameKit.GKCompletionHandler)" /> that specifies behavior after the downloading of achievements from Game Center is completed.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKCompletionHandler ([NullAllowed] GKAchievement [] achivements, [NullAllowed] NSError error);
	/// <param name="descriptions">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate that is called by <see cref="GameKit.GKAchievementDescription.LoadAchievementDescriptions(GameKit.GKAchievementDescriptionHandler)" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKAchievementDescriptionHandler ([NullAllowed] GKAchievementDescription [] descriptions, [NullAllowed] NSError error);
	/// <summary>A delegate that is called by <see cref="GameKit.GKLeaderboard.LoadCategories(GameKit.GKCategoryHandler)" />.</summary>
	delegate void GKCategoryHandler ([NullAllowed] string [] categories, [NullAllowed] string [] titles, [NullAllowed] NSError error);
	/// <summary>A delegate used with <see cref="GameKit.GKVoiceChat.PlayerStateUpdateHandler" /> that specifies behavior when the player's <see cref="GameKit.GKVoiceChatPlayerState" /> changes.</summary>
	delegate void GKPlayerStateUpdateHandler (string playerId, GKVoiceChatPlayerState state);
	/// <param name="publicKeyUrl">To be added.</param>
	/// <param name="signature">To be added.</param>
	/// <param name="salt">To be added.</param>
	/// <param name="timestamp">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>Completion handler for the <see cref="GameKit.GKLocalPlayer.GenerateIdentityVerificationSignature(GameKit.GKIdentityVerificationSignatureHandler)" /> method.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKIdentityVerificationSignatureHandler ([NullAllowed] NSUrl publicKeyUrl, [NullAllowed] NSData signature, [NullAllowed] NSData salt, ulong timestamp, [NullAllowed] NSError error);
	/// <param name="leaderboardSets">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>Completion handler for the <see cref="GameKit.GKLeaderboardSet.LoadLeaderboardSets(GameKit.GKLeaderboardSetsHandler)" /> method.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKLeaderboardSetsHandler ([NullAllowed] GKLeaderboardSet [] leaderboardSets, [NullAllowed] NSError error);
	delegate void GKEntriesForPlayerScopeHandler ([NullAllowed] GKLeaderboardEntry localPlayerEntry, [NullAllowed] GKLeaderboardEntry [] entries, nint totalPlayerCount, [NullAllowed] NSError error);
	delegate void GKEntriesForPlayersHandler ([NullAllowed] GKLeaderboardEntry localPlayerEntry, [NullAllowed] GKLeaderboardEntry [] entries, [NullAllowed] NSError error);

#if MONOMAC
	delegate void GKImageLoadedHandler ([NullAllowed] NSImage image, [NullAllowed] NSError error);
	delegate void GKPlayerPhotoLoaded ([NullAllowed] NSImage photo, [NullAllowed] NSError error);
	delegate void GKChallengeComposeHandler (NSViewController composeController, bool issuedChallenge, [NullAllowed] string [] sentPlayerIDs);
	delegate void GKChallengeComposeHandler2 (NSViewController composeController, bool issuedChallenge, [NullAllowed] GKPlayer [] sentPlayers);
#else
	/// <param name="image">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate passed to <see cref="GameKit.GKAchievementDescription" /> that defines behavior after the image has been loaded.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKImageLoadedHandler ([NullAllowed] UIImage image, [NullAllowed] NSError error);
	/// <param name="photo">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate used with <see cref="GameKit.GKPlayer.LoadPhoto(GameKit.GKPhotoSize,GameKit.GKPlayerPhotoLoaded)" /> to specify behavior after the photo is loaded.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKPlayerPhotoLoaded ([NullAllowed] UIImage photo, [NullAllowed] NSError error);
	/// <param name="composeController">To be added.</param>
	/// <param name="issuedChallenge">To be added.</param>
	/// <param name="sentPlayerIDs">To be added.</param>
	/// <summary>Completion handler for for the <see cref="GameKit.GKScore.ChallengeComposeControllerAsync(System.String,GameKit.GKPlayer[],out UIKit.UIViewController)" /> method.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKChallengeComposeHandler (UIViewController composeController, bool issuedChallenge, string [] sentPlayerIDs);
	delegate void GKChallengeComposeHandler2 (UIViewController composeController, bool issuedChallenge, [NullAllowed] GKPlayer [] sentPlayers);
#endif

	interface IGKVoiceChatClient { }

	/// <summary>The model that GKVoiceChatService uses.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKVoiceChatClient_Protocol/index.html">Apple documentation for <c>GKVoiceChatClient</c></related>
	[NoMac]
	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKVoiceChat' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKVoiceChat' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKVoiceChatClient {
		/// <param name="voiceChatService">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <param name="toParticipant">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("voiceChatService:sendData:toParticipantID:")]
		void SendData (GKVoiceChatService voiceChatService, NSData data, string toParticipant);

		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("participantID")]
		[Abstract]
		string ParticipantID ();

		/// <param name="voiceChatService">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <param name="participantID">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("voiceChatService:sendRealTimeData:toParticipantID:")]
		void SendRealTimeData (GKVoiceChatService voiceChatService, NSData data, string participantID);

		/// <param name="voiceChatService">To be added.</param>
		/// <param name="participantID">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("voiceChatService:didStartWithParticipantID:")]
		void Started (GKVoiceChatService voiceChatService, string participantID);

		/// <param name="voiceChatService">To be added.</param>
		/// <param name="participantID">To be added.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("voiceChatService:didNotStartWithParticipantID:error:")]
		void FailedToConnect (GKVoiceChatService voiceChatService, string participantID, [NullAllowed] NSError error);

		/// <param name="voiceChatService">To be added.</param>
		/// <param name="participantID">To be added.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("voiceChatService:didStopWithParticipantID:error:")]
		void Stopped (GKVoiceChatService voiceChatService, string participantID, [NullAllowed] NSError error);

		/// <param name="voiceChatService">To be added.</param>
		/// <param name="participantID">To be added.</param>
		/// <param name="callID">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("voiceChatService:didReceiveInvitationFromParticipantID:callID:")]
		void ReceivedInvitation (GKVoiceChatService voiceChatService, string participantID, nint callID);
	}

	/// <summary>Provides voice chat over connected iPhones/iPads.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKVoiceChatService_Class/index.html">Apple documentation for <c>GKVoiceChatService</c></related>
	[NoMac]
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

		/// <param name="callID">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("acceptCallID:error:")]
		bool AcceptCall (nint callID, out NSError error);

		/// <param name="callId">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("denyCallID:")]
		void DenyCall (nint callId);

		[Export ("receivedRealTimeData:fromParticipantID:")]
		void ReceivedRealTimeData (NSData audio, string participantID);

		[Export ("receivedData:fromParticipantID:")]
		void ReceivedData (NSData arbitraryData, string participantID);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("microphoneMuted")]
		bool MicrophoneMuted { [Bind ("isMicrophoneMuted")] get; set; }

		[Export ("remoteParticipantVolume")]
		float RemoteParticipantVolume { get; set; } /* float, not CGFloat */

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("outputMeteringEnabled")]
		bool OutputMeteringEnabled { [Bind ("isOutputMeteringEnabled")] get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

	/// <summary>Provides the functionality necessary to discover and connect to nearby iPhones and iPads over Bluetooth.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKSession_Class/index.html">Apple documentation for <c>GKSession</c></related>
	[NoTV]
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

		/// <summary>An instance of the GameKit.IGKSessionDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the GameKit.IGKSessionDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("timeScope", ArgumentSemantic.Assign)]
		GKLeaderboardTimeScope TimeScope { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("playerScope", ArgumentSemantic.Assign)]
		GKLeaderboardPlayerScope PlayerScope { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("maxRange", ArgumentSemantic.Assign)]
		nint MaxRange { get; }

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
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("range", ArgumentSemantic.Assign)]
		NSRange Range { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("scores", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKScore [] Scores { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("localPlayerScore", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKScore LocalPlayerScore { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Export ("init")]
		NativeHandle Constructor ();

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
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("loadScoresWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously loads scores from the Game Center.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadScores operation.   The value of the TResult parameter is a GameKit.GKScoresLoadedHandler.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadScores ([NullAllowed] GKScoresLoadedHandler scoresLoadedHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'LoadLeaderboards' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'LoadLeaderboards' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LoadLeaderboards' instead.")]
		[Static]
		[Export ("loadCategoriesWithCompletionHandler:")]
		[Async (ResultTypeName = "GKCategoryResult", XmlDocs = """
			<summary>Deprecated.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadCategories operation.   The value of the TResult parameter is of type GameKit.GKCategoryResult.  Category and title results from the asynchronous  method.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadCategories ([NullAllowed] GKCategoryHandler categoryHandler);

		[NoTV]
		[Static]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'SetDefaultLeaderboard' on 'GKLocalPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'SetDefaultLeaderboard' on 'GKLocalPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetDefaultLeaderboard' on 'GKLocalPlayer' instead.")]
		[Export ("setDefaultLeaderboard:withCompletionHandler:")]
		[Async (XmlDocs = """
			<param name="leaderboardIdentifier">To be added.</param>
			<summary>Deprecated.</summary>
			<returns>A task that represents the asynchronous SetDefaultLeaderboard operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void SetDefaultLeaderboard ([NullAllowed] string leaderboardIdentifier, [NullAllowed] Action<NSError?> notificationHandler);

		[Export ("groupIdentifier", ArgumentSemantic.Retain)]
		string GroupIdentifier { get; [NotImplemented] set; }

		[Static]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadLeaderBoards(string[] leaderboardIDs, GKLeaderboardsHandler completionHandler)' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadLeaderBoards(string[] leaderboardIDs, GKLeaderboardsHandler completionHandler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadLeaderBoards(string[] leaderboardIDs, GKLeaderboardsHandler completionHandler)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadLeaderBoards(string[] leaderboardIDs, GKLeaderboardsHandler completionHandler)' instead.")]
		[Export ("loadLeaderboardsWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Retrieves the list of leaderboards that have been configured for your application.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadLeaderboards operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKLeaderboard[],Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadLeaderboards ([NullAllowed] Action<GKLeaderboard []?, NSError?> completionHandler);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[NullAllowed]
		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("loadImageWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Loads the leaderboard image asynchronously.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadImage operation.  The result is of type System.Threading.Tasks.Task&lt;AppKit.NSImage&gt; on MacOS and System.Threading.Tasks.Task&lt;UIKit.UIImage&gt; on iOS.</para>
			        </returns>
			<remarks>
			          <para copied="true">The LoadImageAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void LoadImage ([NullAllowed] GKImageLoadedHandler completionHandler);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("initWithPlayers:")]
		NativeHandle Constructor (GKPlayer [] players);

		/// <summary>Gets a Boolean value that tells whether the leaderboard is currently retrieving scores.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadEntries' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadEntries' instead.")]
		[Export ("loading")]
		bool IsLoading { [Bind ("isLoading")] get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Async]
		[Export ("loadLeaderboardsWithIDs:completionHandler:")]
		void LoadLeaderboards ([NullAllowed] string [] leaderboardIds, GKLeaderboardsHandler completionHandler);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("submitScore:context:player:leaderboardIDs:completionHandler:")]
		[Async]
		void SubmitScore (nint score, nuint context, GKPlayer player, string [] leaderboardIds, Action<NSError?> completionHandler);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("baseLeaderboardID", ArgumentSemantic.Strong)]
		string BaseLeaderboardId { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("duration")]
		double Duration { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async (ResultTypeName = "GKEntriesForPlayerScopeResult")]
		[Export ("loadEntriesForPlayerScope:timeScope:range:completionHandler:")]
		void LoadEntries (GKLeaderboardPlayerScope playerScope, GKLeaderboardTimeScope timeScope, NSRange range, GKEntriesForPlayerScopeHandler completionHandler);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async (ResultTypeName = "GKEntriesForPlayersResult")]
		[Export ("loadEntriesForPlayers:timeScope:completionHandler:")]
		void LoadEntries (GKPlayer [] players, GKLeaderboardTimeScope timeScope, GKEntriesForPlayersHandler completionHandler);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("loadPreviousOccurrenceWithCompletionHandler:")]
		[Async]
		void LoadPreviousOccurrence (GKLeaderboardsHandler completionHandler);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("nextStartDate", ArgumentSemantic.Strong)]
		NSDate NextStartDate { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("submitScore:context:player:completionHandler:")]
		[Async]
		void SubmitScore (nint score, nuint context, GKPlayer player, Action<NSError?> completionHandler);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("type")]
		GKLeaderboardType Type { get; }

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Export ("leaderboardDescription")]
		string LeaderboardDescription { get; }

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Export ("releaseState", ArgumentSemantic.Assign)]
		GKReleaseState ReleaseState { get; }

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Export ("activityIdentifier")]
		string ActivityIdentifier { get; }

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Export ("activityProperties", ArgumentSemantic.Strong)]
		NSDictionary<NSString, NSString> ActivityProperties { get; }

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Export ("isHidden")]
		bool IsHidden { get; }
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
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadLeaderboardSets operation.   The value of the TResult parameter is a <see cref="GameKit.GKLeaderboardSetsHandler" />.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadLeaderboardSets ([NullAllowed] GKLeaderboardSetsHandler completionHandler);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'LoadLeaderboardsWithCompletionHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'LoadLeaderboardsWithCompletionHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'LoadLeaderboardsWithCompletionHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'LoadLeaderboardsWithCompletionHandler' instead.")]
		[Export ("loadLeaderboardsWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadLeaderboards operation.   The value of the TResult parameter is a <see cref="GameKit.GKLeaderboardsHandler" />.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadLeaderboards ([NullAllowed] GKLeaderboardsHandler completionHandler);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("loadLeaderboardsWithHandler:")]
		[Async]
		void LoadLeaderboardsWithCompletionHandler (GKLeaderboardsHandler handler);

		[NoTV]
		[Export ("loadImageWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadImage operation.   The value of the TResult parameter is a <see cref="GameKit.GKImageLoadedHandler" />.</para>
			        </returns>
			<remarks>
			          <para copied="true">The LoadImageAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void LoadImage ([NullAllowed] GKImageLoadedHandler completionHandler);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKBasePlayer : NSCopying {
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the GKPlayer.TeamPlayerId property to identify a player instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the GKPlayer.TeamPlayerId property to identify a player instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the GKPlayer.TeamPlayerId property to identify a player instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the GKPlayer.TeamPlayerId property to identify a player instead.")]
		[NullAllowed, Export ("playerID", ArgumentSemantic.Retain)]
		string PlayerID { get; }

		[NullAllowed, Export ("displayName")]
		string DisplayName { get; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GKPlayer' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GKPlayer' instead.")]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GKPlayer' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKPlayer' instead.")]
	[BaseType (typeof (GKBasePlayer))]
	interface GKCloudPlayer {
		[Static]
		[Export ("getCurrentSignedInPlayerForContainer:completionHandler:")]
		void GetCurrentSignedInPlayer ([NullAllowed] string containerName, Action<GKCloudPlayer?, NSError?> handler);
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
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'GKLocalPlayer.LoadFriendPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKLocalPlayer.LoadFriendPlayers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKLocalPlayer.LoadFriendPlayers' instead.")]
		[Export ("isFriend")]
		bool IsFriend { get; }

		[Static, Export ("loadPlayersForIdentifiers:withCompletionHandler:")]
		[Async (XmlDocs = """
			<param name="identifiers">To be added.</param>
			<summary>Loads information from the Game center for the players who are specified by the provided <paramref name="identifiers" /> and runs a completion handler after the information is loaded.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadPlayersForIdentifiers operation.   The value of the TResult parameter is a <see cref="GameKit.GKPlayersHandler" />.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadPlayersForIdentifiers (string [] identifiers, [NullAllowed] GKPlayersHandler completionHandler);

		[Field ("GKPlayerDidChangeNotificationName")]
		[Notification]
		// This name looks wrong, see the "Notification" at the end.
		NSString DidChangeNotificationNameNotification { get; }

		[MacCatalyst (13, 1)]
		[Export ("loadPhotoForSize:withCompletionHandler:")]
		[Async (XmlDocs = """
			<param name="size">To be added.</param>
			<summary>Asynchronously loads the player's photo from the Game Center.</summary>
			<returns>
			          <para>The result is of type System.Threading.Tasks.Task&lt;AppKit.NSImage&gt; on MacOS and System.Threading.Tasks.Task&lt;UIKit.UIImage&gt; on iOS.</para>
			        </returns>
			<remarks>
			          <para copied="true">The LoadPhotoAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void LoadPhoto (GKPhotoSize size, [NullAllowed] GKPlayerPhotoLoaded onCompleted);

		[Export ("displayName")]
		string DisplayName { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("anonymousGuestPlayerWithIdentifier:")]
		GKPlayer GetAnonymousGuestPlayer (string guestIdentifier);

		[MacCatalyst (13, 1)]
		[Export ("guestIdentifier")]
		[NullAllowed]
		string GuestIdentifier { get; }

		[MacCatalyst (13, 1)]
		[Export ("gamePlayerID", ArgumentSemantic.Retain)]
		string GamePlayerId { get; }

		[MacCatalyst (13, 1)]
		[Export ("teamPlayerID", ArgumentSemantic.Retain)]
		string TeamPlayerId { get; }

		[MacCatalyst (13, 1)]
		[Export ("scopedIDsArePersistent")]
		bool ScopedIdsArePersistent { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("GKPlayerIDNoLongerAvailable")]
		NSString IdNoLongerAvailable { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("isInvitable")]
		bool IsInvitable { get; }
	}

	[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'GKLeaderboardEntry' instead.")]
	[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'GKLeaderboardEntry' instead.")]
	[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'GKLeaderboardEntry' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'GKLeaderboardEntry' instead.")]
	[BaseType (typeof (NSObject))]
	interface GKScore : NSSecureCoding {
		[MacCatalyst (13, 1)]
		[Export ("initWithLeaderboardIdentifier:player:")]
		NativeHandle Constructor (string identifier, GKPlayer player);

		[Deprecated (PlatformName.iOS, 8, 0, message: "Use the overload that takes a 'GKPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that takes a 'GKPlayer' instead.")]
		[Export ("initWithLeaderboardIdentifier:forPlayer:")]
		NativeHandle Constructor (string identifier, string playerID);

#if XAMCORE_5_0
		/// <summary>Create a new <see cref="GKScore" /> for the specified leaderboard.</summary>
		/// <param name="identifier">The identifier for the leaderboard the score is sent to.</param>
#else
		/// <summary>Create a new <see cref="GKScore" /> for the specified leaderboard.</summary>
		/// <param name="categoryOrIdentifier">The identifier for the leaderboard the score is sent to.</param>
#endif
		[MacCatalyst (13, 1)]
		[Export ("initWithLeaderboardIdentifier:")]
#if XAMCORE_5_0
		NativeHandle Constructor (string identifier);
#else
		NativeHandle Constructor (string categoryOrIdentifier);
#endif

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

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'LeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'LeaderboardIdentifier' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LeaderboardIdentifier' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("category", ArgumentSemantic.Copy)]
		string Category { get; set; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'ReportScores' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'ReportScores' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ReportScores' instead.")]
		[Export ("reportScoreWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Deprecated.</summary>
			<returns>A task that represents the asynchronous ReportScore operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void ReportScore ([NullAllowed] Action<NSError?> errorHandler);

		[Export ("context", ArgumentSemantic.Assign)]
		ulong Context { get; set; }

		[Export ("shouldSetDefaultLeaderboard", ArgumentSemantic.Assign)]
		bool ShouldSetDefaultLeaderboard { get; set; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ... )' and present the view controller instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ... )' and present the view controller instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ... )' and present the view controller instead.")]
		[Export ("issueChallengeToPlayers:message:")]
		void IssueChallengeToPlayers ([NullAllowed] string [] playerIDs, [NullAllowed] string message);

		[Export ("reportScores:withCompletionHandler:"), Static]
		[Async (XmlDocs = """
			<param name="scores">Scores to report back to Game Center.</param>
			<summary>Reports the provided scores to the Game Center.</summary>
			<returns>A task that represents the asynchronous ReportScores operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void ReportScores (GKScore [] scores, [NullAllowed] Action<NSError?> completionHandler);

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("leaderboardIdentifier", ArgumentSemantic.Copy)]
		string LeaderboardIdentifier { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("reportScores:withEligibleChallenges:withCompletionHandler:"), Static]
		[Async (XmlDocs = """
			<param name="scores">To be added.</param>
			<param name="challenges">To be added.</param>
			<summary>Asynchronously reports the provided scores to the Game Center</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		void ReportScores (GKScore [] scores, GKChallenge [] challenges, [NullAllowed] Action<NSError?> completionHandler);

		[iOS (14, 0)]
		[TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Async]
		[Export ("reportLeaderboardScores:withEligibleChallenges:withCompletionHandler:")]
		void ReportLeaderboardScores (GKLeaderboardScore [] scores, GKChallenge [] eligibleChallenges, [NullAllowed] Action<NSError?> completionHandler);

		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ...)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Pass 'GKPlayers' to 'ChallengeComposeController (GKPlayer [] players, string message, ...)' instead.")]
		[Export ("challengeComposeControllerWithPlayers:message:completionHandler:")]
		[return: NullAllowed]
		UIViewController ChallengeComposeController ([NullAllowed] string [] playerIDs, [NullAllowed] string message, [NullAllowed] GKChallengeComposeHandler completionHandler);

		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "GKChallengeComposeResult", XmlDocs = """
			<param name="message">An editable message to display to the other players. May be .</param>
			<param name="players">The players to challenge.</param>
			<summary>Provides a view controller that can be used to send a challenge, with a message, to other players.</summary>
			<returns>
			          <para>A task that represents the asynchronous ChallengeComposeController operation.   The value of the TResult parameter is of type <c>Action&lt;GameKit.GKChallengeComposeResult&gt;</c>.</para>
			        </returns>
			<remarks>
			          <para copied="true">The ChallengeComposeControllerAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="message">An editable message to display to the other players. May be <see langword="null" />.</param>
			<param name="players">The players to challenge.</param>
			<param name="result">The view controller that displays the result of the challenge. May be <see langword="null" />.</param>
			<summary>Asynchronously provides a view controller that can be used to send a challenge, with a message, to other players, returning a task that provides the challenge result.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("challengeComposeControllerWithMessage:players:completionHandler:")]
		UIViewController ChallengeComposeController ([NullAllowed] string message, [NullAllowed] GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler completionHandler);
	}

	interface IGKLeaderboardViewControllerDelegate { }

	/// <summary>A delegate object that allows fine-grained control over <see cref="GameKit.GKLeaderboardViewController" /> events.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKLeaderboardViewControllerDelegate_Ref/index.html">Apple documentation for <c>GKLeaderboardViewControllerDelegate</c></related>
	[NoTV]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKGameCenterViewController' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKLeaderboardViewControllerDelegate {
		/// <param name="viewController">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Abstract]
		[Export ("leaderboardViewControllerDidFinish:")]
		void DidFinish (GKLeaderboardViewController viewController);
	}

	/// <summary>A <see cref="GameKit.GKGameCenterViewController" /> that presents leaderboards to the player.</summary>
	///     <remarks>
	///       <para>Application developers should use <see cref="GameKit.GKGameCenterViewController" /> instead of this class.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKLeaderboardViewController_Ref/index.html">Apple documentation for <c>GKLeaderboardViewController</c></related>
	[NoTV]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKGameCenterViewController' instead.")]
#if MONOMAC
	[BaseType (typeof (GKGameCenterViewController), Events = new Type [] { typeof (GKLeaderboardViewControllerDelegate) }, Delegates = new string [] { "WeakDelegate" })]
	interface GKLeaderboardViewController
#else
	[BaseType (typeof (GKGameCenterViewController), Events = new Type [] { typeof (GKLeaderboardViewControllerDelegate) }, Delegates = new string [] { "WeakDelegate" })]
	interface GKLeaderboardViewController : UIAppearance
#endif
	{
		[Export ("leaderboardDelegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the GameKit.IGKLeaderboardViewControllerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the GameKit.IGKLeaderboardViewControllerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		IGKLeaderboardViewControllerDelegate Delegate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("category", ArgumentSemantic.Copy)]
		string Category { get; set; }

		[Export ("timeScope", ArgumentSemantic.Assign)]
		GKLeaderboardTimeScope TimeScope { get; set; }
	}

	[TV (13, 4), iOS (13, 4)]
	[MacCatalyst (13, 1)]
	delegate void GKFetchItemsForIdentityVerificationSignatureCompletionHandler ([NullAllowed] NSUrl publicKeyUrl, [NullAllowed] NSData signature, [NullAllowed] NSData salt, ulong timestamp, [NullAllowed] NSError error);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKPlayer))]
	interface GKLocalPlayer
#if !TVOS // from GKSavedGame category
		: GKSavedGameListener
#endif
	{
		/// <summary>Current authentication state for the local player.</summary>
		///         <value>True if the user has been authenticated, false otherwise.</value>
		///         <remarks>
		///
		/// 	  This property changes as the application transitions from
		/// 	  background to foreground, so you should check the value of
		/// 	  this property on your handler and enable or disable features
		/// 	  that depend on it in your game.
		///
		/// 	</remarks>
		[Export ("authenticated")]
		bool Authenticated { [Bind ("isAuthenticated")] get; }

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

		/// <summary>Gets a value that tells whether the player is undreaged.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isUnderage")]
		bool IsUnderage { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Set the 'AuthenticationHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Set the 'AuthenticationHandler' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Set the 'AuthenticationHandler' instead.")]
		[Export ("authenticateWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Developers should not use this deprecated method. Set the 'AuthenticationHandler' instead.</summary>
			<returns>A task that represents the asynchronous Authenticate operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void Authenticate ([NullAllowed] Action<NSError?> handler);

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<summary>Loads the recent players.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadRecentPlayers operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKPlayer[],Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("loadRecentPlayersWithCompletionHandler:")]
		void LoadRecentPlayers ([NullAllowed] Action<GKPlayer []?, NSError?> completionHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'LoadRecentPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'LoadRecentPlayers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LoadRecentPlayers' instead.")]
		[Export ("loadFriendsWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously loads an array of the local player's friends' identifiers.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadFriends operation.   The value of the TResult parameter is a GameKit.GKFriendsHandler.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadFriends ([NullAllowed] GKFriendsHandler handler);

		[Field ("GKPlayerAuthenticationDidChangeNotificationName")]
		[Notification]
		NSString AuthenticationDidChangeNotificationName { get; }

		[NullAllowed] // by default this property is null
		[Export ("authenticateHandler", ArgumentSemantic.Copy)]
		[MacCatalyst (13, 1)]
#if !MONOMAC
		Action<UIViewController?, NSError?> AuthenticateHandler { get; set; }
#else
		Action<NSViewController?, NSError?> AuthenticateHandler { get; set; }
#endif

		[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("isPresentingFriendRequestViewController")]
		bool IsPresentingFriendRequestViewController { get; }

		[NoTV, NoMac, iOS (15, 0), NoMacCatalyst]
		[Export ("presentFriendRequestCreatorFromViewController:error:")]
		bool PresentFriendRequestCreator (UIViewController viewController, [NullAllowed] out NSError error);

		[NoTV, NoiOS, NoMacCatalyst]
		[Export ("presentFriendRequestCreatorFromWindow:error:")]
		bool PresentFriendRequestCreator ([NullAllowed] NSWindow window, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.iOS, 26, 2, message: "No longer supported.")]
		[Deprecated (PlatformName.MacOSX, 26, 2, message: "No longer supported.")]
		[Deprecated (PlatformName.TvOS, 26, 2, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 26, 2, message: "No longer supported.")]
		[MacCatalyst (13, 1)]
		[Export ("loadDefaultLeaderboardIdentifierWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously loads the local player's default leaderboard identifier.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadDefaultLeaderboardIdentifier operation.  The value of the TResult parameter is of type System.Action&lt;System.String,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadDefaultLeaderboardIdentifier ([NullAllowed] Action<string?, NSError?> completionHandler);

		[Deprecated (PlatformName.iOS, 26, 2, message: "No longer supported.")]
		[Deprecated (PlatformName.MacOSX, 26, 2, message: "No longer supported.")]
		[Deprecated (PlatformName.TvOS, 26, 2, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 26, 2, message: "No longer supported.")]
		[MacCatalyst (13, 1)]
		[Export ("setDefaultLeaderboardIdentifier:completionHandler:")]
		[Async (XmlDocs = """
			<param name="leaderboardIdentifier">To be added.</param>
			<summary>Asynchronously sets the local player's default leaderboard identifier.</summary>
			<returns>A task that represents the asynchronous SetDefaultLeaderboardIdentifier operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void SetDefaultLeaderboardIdentifier (string leaderboardIdentifier, [NullAllowed] Action<NSError?> completionHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'LoadDefaultLeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'LoadDefaultLeaderboardIdentifier' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'LoadDefaultLeaderboardIdentifier' instead.")]
		[Export ("loadDefaultLeaderboardCategoryIDWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously loads the local player's default leaderboard category identifier.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadDefaultLeaderboardCategoryID operation.  The value of the TResult parameter is of type System.Action&lt;System.String,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadDefaultLeaderboardCategoryID ([NullAllowed] Action<string?, NSError?> completionHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'SetDefaultLeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'SetDefaultLeaderboardIdentifier' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetDefaultLeaderboardIdentifier' instead.")]
		[Export ("setDefaultLeaderboardCategoryID:completionHandler:")]
		[Async (XmlDocs = """
			<param name="categoryID">To be added.</param>
			<summary>Asynchronously sets the local player's default leaderboard category identifier.</summary>
			<returns>A task that represents the asynchronous SetDefaultLeaderboardCategoryID operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void SetDefaultLeaderboardCategoryID ([NullAllowed] string categoryID, [NullAllowed] Action<NSError?> completionHandler);

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
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FetchItemsForIdentityVerificationSignature' instead.")]
		[Async (ResultTypeName = "GKIdentityVerificationSignatureResult", XmlDocs = """
			<summary>Creates and returns a signature for authenticating the local player on a third-party server. See remarks</summary>
			<returns>
			          <para>A task that represents the asynchronous GenerateIdentityVerificationSignature operation.   The value of the TResult parameter is of type GameKit.GKIdentityVerificationSignatureResult.  Holds the return values from the asynchronous method </para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("generateIdentityVerificationSignatureWithCompletionHandler:")]
		void GenerateIdentityVerificationSignature ([NullAllowed] GKIdentityVerificationSignatureHandler completionHandler);

		[TV (13, 4), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "GKFetchItemsForIdentityVerificationSignature")]
		[Export ("fetchItemsForIdentityVerificationSignature:")]
		void FetchItemsForIdentityVerificationSignature ([NullAllowed] GKFetchItemsForIdentityVerificationSignatureCompletionHandler completionHandler);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Async (XmlDocs = """
			<summary>Asynchronously loads an array of the local player's friends' identifiers.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadFriendPlayers operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKPlayer[],Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The LoadFriendPlayersAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("loadFriendPlayersWithCompletionHandler:")]
		void LoadFriendPlayers ([NullAllowed] Action<GKPlayer []?, NSError?> completionHandler);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("fetchSavedGamesWithCompletionHandler:")]
		void FetchSavedGames ([NullAllowed] Action<GKSavedGame []?, NSError?> handler);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("saveGameData:withName:completionHandler:")]
		void SaveGameData (NSData data, string name, [NullAllowed] Action<GKSavedGame?, NSError?> handler);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("deleteSavedGamesWithName:completionHandler:")]
		void DeleteSavedGames (string name, [NullAllowed] Action<NSError?> handler);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("resolveConflictingSavedGames:withData:completionHandler:")]
		void ResolveConflictingSavedGames (GKSavedGame [] conflictingSavedGames, NSData data, [NullAllowed] Action<GKSavedGame []?, NSError?> handler);

		[MacCatalyst (13, 1)]
		[Export ("multiplayerGamingRestricted")]
		bool MultiplayerGamingRestricted { [Bind ("isMultiplayerGamingRestricted")] get; }

		[MacCatalyst (13, 1)]
		[Export ("loadChallengableFriendsWithCompletionHandler:")]
		[Async]
		void LoadChallengeableFriends ([NullAllowed] Action<GKPlayer []?, NSError?> completionHandler);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("local")]
		GKLocalPlayer Local { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("personalizedCommunicationRestricted")]
		bool PersonalizedCommunicationRestricted { [Bind ("isPersonalizedCommunicationRestricted")] get; }

		// FriendsList Category

		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("loadFriendsAuthorizationStatus:")]
		void LoadFriendsAuthorizationStatus (Action<GKFriendsAuthorizationStatus, NSError?> completionHandler);

		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("loadFriends:")]
		void LoadFriendsList (Action<GKPlayer []?, NSError?> completionHandler);

		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("loadFriendsWithIdentifiers:completionHandler:")]
		void LoadFriendsList (string [] identifiers, Action<GKPlayer []?, NSError?> completionHandler);
	}

	/// <summary>Contains information that is needed to locate and load a saved game.</summary>
	///     
	///     <!-- Apple undocumented 2014-09-08 -->
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKSavedGame_Ref/index.html">Apple documentation for <c>GKSavedGame</c></related>
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
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadData operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSData,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The LoadDataAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void LoadData ([NullAllowed] Action<NSData?, NSError?> handler);
	}

	/// <summary>Application developers override this class to respond to conflicts or player modifications in saved games.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKSavedGameListener_Ref/index.html">Apple documentation for <c>GKSavedGameListener</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface GKSavedGameListener {
		/// <param name="player">To be added.</param>
		/// <param name="savedGame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:didModifySavedGame:")]
		void DidModifySavedGame (GKPlayer player, GKSavedGame savedGame);

		/// <param name="player">To be added.</param>
		/// <param name="savedGames">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:hasConflictingSavedGames:")]
		void HasConflictingSavedGames (GKPlayer player, GKSavedGame [] savedGames);
	}

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

		/// <summary>An instance of the GameKit.IGKMatchDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the GameKit.IGKMatchDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		IGKMatchDelegate Delegate { get; set; }

		[Export ("expectedPlayerCount")]
		nint ExpectedPlayerCount { get; }

		[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
		[NullAllowed, Export ("properties")]
		NSDictionary<NSString, NSObject> Properties { get; }

		[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
		[NullAllowed, Export ("playerProperties")]
		NSDictionary<GKPlayer, NSDictionary<NSString, NSObject>> PlayerProperties { get; }

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

		[Deprecated (PlatformName.iOS, 18, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "No longer supported.")]
		[Export ("voiceChatWithName:")]
		[return: NullAllowed]
		GKVoiceChat VoiceChatWithName (string name);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'ChooseBestHostingPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'ChooseBestHostingPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ChooseBestHostingPlayer' instead.")]
		[Export ("chooseBestHostPlayerWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Developers should not use this deprecated method. Developers should use 'ChooseBestHostingPlayer' instead.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous ChooseBestHostPlayer operation.  The value of the TResult parameter is of type System.Action&lt;System.String&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void ChooseBestHostPlayer (Action<string?> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("rematchWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous Rematch operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKMatch,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void Rematch ([NullAllowed] Action<GKMatch?, NSError?> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("players")]
		GKPlayer [] Players { get; }

		[MacCatalyst (13, 1)]
		[Export ("chooseBestHostingPlayerWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous ChooseBestHostingPlayer operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKPlayer&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The ChooseBestHostingPlayerAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void ChooseBestHostingPlayer (Action<GKPlayer?> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("sendData:toPlayers:dataMode:error:")]
		bool SendData (NSData data, GKPlayer [] players, GKMatchSendDataMode mode, out NSError error);
	}

	interface IGKMatchDelegate { }

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKMatchDelegate {

		/// <param name="match">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <param name="playerId">To be added.</param>
		/// <summary>Developers should not use this deprecated method. Developers should use 'DataReceivedFromPlayer (GKMatch,NSData,GKPlayer)' instead.</summary>
		/// <remarks>To be added.</remarks>
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'DataReceivedFromPlayer (GKMatch,NSData,GKPlayer)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'DataReceivedFromPlayer (GKMatch,NSData,GKPlayer)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DataReceivedFromPlayer (GKMatch,NSData,GKPlayer)' instead.")]
		[Export ("match:didReceiveData:fromPlayer:"), EventArgs ("GKData", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void DataReceived (GKMatch match, NSData data, string playerId);

		/// <param name="match">To be added.</param>
		/// <param name="playerId">To be added.</param>
		/// <param name="state">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'StateChangedForPlayer (GKMatch,GKPlayer,GKPlayerConnectionState)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'StateChangedForPlayer (GKMatch,GKPlayer,GKPlayerConnectionState)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'StateChangedForPlayer (GKMatch,GKPlayer,GKPlayerConnectionState)' instead.")]
		[Export ("match:player:didChangeState:"), EventArgs ("GKState", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void StateChanged (GKMatch match, string playerId, GKPlayerConnectionState state);

		/// <param name="match">To be added.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Method that is called when a match cannot connect to any of the players.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("match:didFailWithError:"), EventArgs ("GKError", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void Failed (GKMatch match, [NullAllowed] NSError error);

		/// <param name="match">To be added.</param>
		/// <param name="playerId">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Delegate invoked by the object to get a value.</summary>
			<value>To be added.</value>
			<remarks>Developers assign a function, delegate or anonymous method to this property to return a value to the object.   If developers assign a value to this property, it this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'ShouldReinviteDisconnectedPlayer' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'ShouldReinviteDisconnectedPlayer' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ShouldReinviteDisconnectedPlayer' instead.")]
		[Export ("match:shouldReinvitePlayer:"), DelegateName ("GKMatchReinvitation"), DefaultValue (true)]
		bool ShouldReinvitePlayer (GKMatch match, string playerId);

		/// <param name="match">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <param name="player">To be added.</param>
		/// <summary>Method that is called when data is received from a player.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("match:didReceiveData:fromRemotePlayer:"), EventArgs ("GKMatchReceivedDataFromRemotePlayer", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void DataReceivedFromPlayer (GKMatch match, NSData data, GKPlayer player);

		/// <param name="match">To be added.</param>
		/// <param name="player">To be added.</param>
		/// <param name="state">To be added.</param>
		/// <summary>Method that is called when a player's connection state changes.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("match:player:didChangeConnectionState:"), EventArgs ("GKMatchConnectionChanged", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void StateChangedForPlayer (GKMatch match, GKPlayer player, GKPlayerConnectionState state);

		/// <param name="match">To be added.</param>
		/// <param name="player">To be added.</param>
		/// <summary>Method that is called when a player is disconnected from a two-player match.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Delegate invoked by the object to get a value.</summary>
			<value>To be added.</value>
			<remarks>Developers assign a function, delegate or anonymous method to this property to return a value to the object.   If developers assign a value to this property, it this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[MacCatalyst (13, 1)]
		[Export ("match:shouldReinviteDisconnectedPlayer:")]
		[DelegateName ("GKMatchReinvitationForDisconnectedPlayer"), DefaultValue (true)]
		bool ShouldReinviteDisconnectedPlayer (GKMatch match, GKPlayer player);

		/// <param name="match">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <param name="recipient">To be added.</param>
		/// <param name="player">To be added.</param>
		/// <summary>Method that is called when the <paramref name="recipient" /> receives data from another <paramref name="player" />.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("match:didReceiveData:forRecipient:fromRemotePlayer:"), EventArgs ("GKDataReceivedForRecipient", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void DataReceivedForRecipient (GKMatch match, NSData data, GKPlayer recipient, GKPlayer player);
	}

	[Deprecated (PlatformName.iOS, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "No longer supported.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "No longer supported.")]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKVoiceChat {
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; }

		/// <summary>Gets or sets a Boolean value that controls whether the microphone is being sampled.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <param name="handler">To be added.</param>
		///         <summary>Sets the handler that is run when a player's voice chat status changes.</summary>
		///         <remarks>To be added.</remarks>
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

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'GKMatchmakerViewController.MatchmakingMode' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'GKMatchmakerViewController.MatchmakingMode' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'GKMatchmakerViewController.MatchmakingMode' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'GKMatchmakerViewController.MatchmakingMode' instead.")]
		[Export ("restrictToAutomatch")]
		bool RestrictToAutomatch { get; set; }

		[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
		[NullAllowed, Export ("queueName")]
		string QueueName { get; set; }

		[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
		[NullAllowed, Export ("properties", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Properties { get; set; }

		[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
		[NullAllowed, Export ("recipientProperties", ArgumentSemantic.Copy)]
		NSDictionary<GKPlayer, NSDictionary<NSString, NSObject>> RecipientProperties { get; set; }
	}

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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		[Async (XmlDocs = """
			<param name="request">To be added.</param>
			<summary>Finds players for a peer-to-peer match.</summary>
			<returns>
			          <para>A task that represents the asynchronous FindMatch operation.   The value of the TResult parameter is a GameKit.GKNotificationMatch.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void FindMatch (GKMatchRequest request, [NullAllowed] GKNotificationMatch matchHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'FindPlayersForHostedRequest' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'FindPlayersForHostedRequest' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FindPlayersForHostedRequest' instead.")]
		[Export ("findPlayersForHostedMatchRequest:withCompletionHandler:")]
		[Async (XmlDocs = """
			<param name="request">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous FindPlayers operation.   The value of the TResult parameter is a GameKit.GKFriendsHandler.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void FindPlayers (GKMatchRequest request, [NullAllowed] GKFriendsHandler playerHandler);

		[Export ("addPlayersToMatch:matchRequest:completionHandler:")]
		[Async (XmlDocs = """
			<param name="toMatch">To be added.</param>
			<param name="matchRequest">To be added.</param>
			<summary>Adds the players in the match request to the match.</summary>
			<returns>A task that represents the asynchronous AddPlayers operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void AddPlayers (GKMatch toMatch, GKMatchRequest matchRequest, [NullAllowed] Action<NSError?> completionHandler);

		[Export ("cancel")]
		void Cancel ();

		/// <param name="playerGroup">To be added.</param>
		/// <param name="completionHandler">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("queryPlayerGroupActivity:withCompletionHandler:")]
		[Async (XmlDocs = """
			<param name="playerGroup">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous QueryPlayerGroupActivity operation.   The value of the TResult parameter is a GameKit.GKQueryHandler.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void QueryPlayerGroupActivity (nint playerGroup, [NullAllowed] GKQueryHandler completionHandler);

		[Export ("queryActivityWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Queries for activity in all player groups.</summary>
			<returns>
			          <para>A task that represents the asynchronous QueryActivity operation.   The value of the TResult parameter is a GameKit.GKQueryHandler.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void QueryActivity ([NullAllowed] GKQueryHandler completionHandler);

		[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
		[Async]
		[Export ("queryQueueActivity:withCompletionHandler:")]
		void QueryQueueActivity (string queueName, [NullAllowed] Action<nint, NSError?> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("matchForInvite:completionHandler:")]
		[Async (XmlDocs = """
			<param name="invite">To be added.</param>
			<summary>Creates a match for the specified invitation.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous Match operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKMatch,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void Match (GKInvite invite, [NullAllowed] Action<GKMatch?, NSError?> completionHandler);

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
		[Async (XmlDocs = """
			<param name="request">To be added.</param>
			<summary>Finds players for a hosted match request.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous FindPlayersForHostedRequest operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKPlayer[],Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The FindPlayersForHostedRequestAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void FindPlayersForHostedRequest (GKMatchRequest request, [NullAllowed] Action<GKPlayer []?, NSError?> completionHandler);

		[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
		[Async]
		[Export ("findMatchedPlayers:withCompletionHandler:")]
		void FindMatchedPlayers (GKMatchRequest request, Action<GKMatchedPlayers?, NSError?> completionHandler);

		// Not truly an [Async] method since the handler can be called multiple times, for each player found
		[MacCatalyst (13, 1)]
		[Export ("startBrowsingForNearbyPlayersWithHandler:")]
		void StartBrowsingForNearbyPlayers ([NullAllowed] Action<GKPlayer, bool> handler);

		[iOS (16, 2), NoTV]
		[MacCatalyst (16, 2)]
		[Export ("startGroupActivityWithPlayerHandler:")]
		void StartGroupActivity (Action<GKPlayer> handler);

		[iOS (16, 2), NoTV]
		[MacCatalyst (16, 2)]
		[Export ("stopGroupActivity")]
		void StopGroupActivity ();
	}

	[MacCatalyst (13, 1)]
#if MONOMAC
	[BaseType (typeof (NSViewController), Delegates = new string [] { "WeakMatchmakerDelegate" }, Events = new Type [] { typeof (GKMatchmakerViewControllerDelegate) })]
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
		/// <param name="nibNameOrNull">To be added.</param>
		/// <param name="nibBundleOrNull">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[NullAllowed]
		[Export ("matchmakerDelegate", ArgumentSemantic.Assign)]
		NSObject WeakMatchmakerDelegate { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Wrap ("WeakMatchmakerDelegate")]
		IGKMatchmakerViewControllerDelegate MatchmakerDelegate { get; set; }

		[Export ("matchRequest", ArgumentSemantic.Strong)]
		GKMatchRequest MatchRequest { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("matchmakingMode", ArgumentSemantic.Assign)]
		GKMatchmakingMode MatchmakingMode { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("canStartWithMinimumPlayers")]
		bool CanStartWithMinimumPlayers { get; set; }
	}

	interface IGKMatchmakerViewControllerDelegate { }

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKMatchmakerViewControllerDelegate {
		/// <param name="viewController">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakMatchmakerDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Abstract]
		[Export ("matchmakerViewControllerWasCancelled:")]
		void WasCancelled (GKMatchmakerViewController viewController);

		/// <param name="viewController">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("matchmakerViewController:didFailWithError:"), EventArgs ("GKError", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakMatchmakerDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void DidFailWithError (GKMatchmakerViewController viewController, NSError error);

		/// <param name="viewController">To be added.</param>
		/// <param name="match">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("matchmakerViewController:didFindMatch:"), EventArgs ("GKMatch", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakMatchmakerDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void DidFindMatch (GKMatchmakerViewController viewController, GKMatch match);

		/// <param name="viewController">To be added.</param>
		/// <param name="playerIDs">To be added.</param>
		/// <summary>Developers should not use this deprecated method. Developers should use 'DidFindHostedPlayers' instead.</summary>
		/// <remarks>To be added.</remarks>
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'DidFindHostedPlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'DidFindHostedPlayers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidFindHostedPlayers' instead.")]
		[Export ("matchmakerViewController:didFindPlayers:"), EventArgs ("GKPlayers", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakMatchmakerDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void DidFindPlayers (GKMatchmakerViewController viewController, string [] playerIDs);

		/// <param name="viewController">To be added.</param>
		/// <param name="playerIDs">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("matchmakerViewController:didFindHostedPlayers:"), EventArgs ("GKMatchmakingPlayers", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakMatchmakerDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void DidFindHostedPlayers (GKMatchmakerViewController viewController, GKPlayer [] playerIDs);

		/// <param name="viewController">To be added.</param>
		/// <param name="playerID">To be added.</param>
		/// <summary>Developers should not use this deprecated method. Developers should use 'HostedPlayerDidAccept' instead.</summary>
		/// <remarks>To be added.</remarks>
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'HostedPlayerDidAccept' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'HostedPlayerDidAccept' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HostedPlayerDidAccept' instead.")]
		[Export ("matchmakerViewController:didReceiveAcceptFromHostedPlayer:"), EventArgs ("GKPlayer", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakMatchmakerDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void ReceivedAcceptFromHostedPlayer (GKMatchmakerViewController viewController, string playerID);

		/// <param name="viewController">To be added.</param>
		/// <param name="playerID">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("matchmakerViewController:hostedPlayerDidAccept:"), EventArgs ("GKMatchmakingPlayer", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakMatchmakerDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void HostedPlayerDidAccept (GKMatchmakerViewController viewController, GKPlayer playerID);

		[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
		[IgnoredInDelegate]
		[Export ("matchmakerViewController:getMatchPropertiesForRecipient:withCompletionHandler:")]
		void GetMatchProperties (GKMatchmakerViewController viewController, GKPlayer recipient, Action<NSDictionary<NSString, NSObject>> completionHandler);
	}

	[BaseType (typeof (NSObject))]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	interface GKAchievement : NSSecureCoding {
		/// <summary>Deprecated.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <summary>Gets a value that tells whether the player completed the achievement.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("completed")]
		bool Completed { [Bind ("isCompleted")] get; }

		[Export ("lastReportedDate", ArgumentSemantic.Copy)]
		NSDate LastReportedDate { get; [NotImplemented] set; }

		[Static]
		[Export ("loadAchievementsWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously loads the achievement progress.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadAchievements operation.   The value of the TResult parameter is a GameKit.GKCompletionHandler.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadAchievements ([NullAllowed] GKCompletionHandler completionHandler);

		[Static]
		[Export ("resetAchievementsWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously resets all achievements for the local player.</summary>
			<returns>A task that represents the asynchronous ResetAchivements operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void ResetAchivements ([NullAllowed] Action<NSError?> completionHandler);

		/// <summary>Default constructor that initializes a new instance of this class with no parameters.</summary>
		/// <remarks>
		///         </remarks>
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
		[Async (XmlDocs = """
			<summary>Deprecated.</summary>
			<returns>A task that represents the asynchronous ReportAchievement operation</returns>
			<remarks>To be added.</remarks>
			""")]
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use ReportAchievements '(GKAchievement[] achievements, Action<NSError?> completionHandler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use ReportAchievements '(GKAchievement[] achievements, Action<NSError?> completionHandler)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use ReportAchievements '(GKAchievement[] achievements, Action<NSError?> completionHandler)' instead.")]
		void ReportAchievement ([NullAllowed] Action<NSError?> completionHandler);

		[Export ("showsCompletionBanner", ArgumentSemantic.Assign)]
		bool ShowsCompletionBanner { get; set; }

		[Static]
		[Export ("reportAchievements:withCompletionHandler:")]
		[Async (XmlDocs = """
			<param name="achievements">Achievements to report to Game Center.</param>
			<summary>Asynchronously reports the provided achievements and matching challenges to Game Center.</summary>
			<returns>A task that represents the asynchronous ReportAchievements operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void ReportAchievements (GKAchievement [] achievements, [NullAllowed] Action<NSError?> completionHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Pass 'GKPlayers' to 'ChallengeComposeController(GKPlayer[] players, string message, ...)' and present the view controller instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Pass 'GKPlayers' to 'ChallengeComposeController(GKPlayer[] players, string message, ...)' and present the view controller instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Pass 'GKPlayers' to 'ChallengeComposeController(GKPlayer[] players, string message, ...)' and present the view controller instead.")]
		[Export ("issueChallengeToPlayers:message:")]
		void IssueChallengeToPlayers ([NullAllowed] string [] playerIDs, [NullAllowed] string message);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Pass 'GKPlayers' to 'SelectChallengeablePlayers' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Pass 'GKPlayers' to 'SelectChallengeablePlayers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Pass 'GKPlayers' to 'SelectChallengeablePlayers' instead.")]
		[Export ("selectChallengeablePlayerIDs:withCompletionHandler:")]
		[Async (XmlDocs = """
			<param name="playerIDs">To be added.</param>
			<summary>Deprecated.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous SelectChallengeablePlayerIDs operation.  The value of the TResult parameter is of type System.Action&lt;System.String[],Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void SelectChallengeablePlayerIDs ([NullAllowed] string [] playerIDs, [NullAllowed] Action<string []?, NSError?> completionHandler);

		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'Player' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Player' instead.")]
		[NullAllowed, Export ("playerID", ArgumentSemantic.Copy)]
		string PlayerID {
			get;
		}

		[MacCatalyst (13, 1)]
		[Export ("reportAchievements:withEligibleChallenges:withCompletionHandler:"), Static]
		[Async (XmlDocs = """
			<param name="achievements">To be added.</param>
			<param name="challenges">To be added.</param>
			<summary>Asynchronously reports the provided achievements and challenges to Game Center.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		void ReportAchievements (GKAchievement [] achievements, GKChallenge [] challenges, [NullAllowed] Action<NSError?> completionHandler);

		[NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("player", ArgumentSemantic.Retain)]
		GKPlayer Player { get; }

		[MacCatalyst (13, 1)]
		[Export ("initWithIdentifier:player:")]
		NativeHandle Constructor ([NullAllowed] string identifier, GKPlayer player);

		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "GKChallengeComposeResult", XmlDocs = """
			<summary>Provides a view controller that can be used to send a challenge, with a message, to other players.</summary>
			<param name="message">An editable message to display to the other players. May be <see langword="null" />.</param>
			<param name="players">The players to challenge.</param>
			<returns>
			  <para>A task that represents the asynchronous ChallengeComposeController operation. The value of the TResult parameter is of type GameKit.GKChallengeComposeResult. Holds the return values from the asynchronous method.</para>
			</returns>
			<remarks>To be added.</remarks>
			""",
#if __MACOS__
			XmlDocsWithOutParameter = """
			<summary>Asynchronously provides a view controller that can be used to send a challenge, with a message, to other players, returning a task with the response result.</summary>
			<param name="message">An editable message to display to the other players. May be <see langword="null" />.</param>
			<param name="players">The players to challenge.</param>
			<param name="result">The view controller that displays the result of the challenge. May be <see langword="null" />.</param>
			<remarks>
			  <para>The type of the <paramref name="result" /> out argument is <see cref="T:UIKit.UIViewController" /> on iOS, tvOS and Mac Catalyst and <see cref="AppKit.NSViewController" /> on macOS.</para>
			</remarks>
			"""
#else
			XmlDocsWithOutParameter = """
			<summary>Asynchronously provides a view controller that can be used to send a challenge, with a message, to other players, returning a task with the response result.</summary>
			<param name="message">An editable message to display to the other players. May be <see langword="null" />.</param>
			<param name="players">The players to challenge.</param>
			<param name="result">The view controller that displays the result of the challenge. May be <see langword="null" />.</param>
			<remarks>
			  <para>The type of the <paramref name="result" /> out argument is <see cref="UIKit.UIViewController" /> on iOS, tvOS and Mac Catalyst and <see cref="T:AppKit.NSViewController" /> on macOS.</para>
			</remarks>
			"""
#endif

			)]
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.MacOSX, 14, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0)]
		[Export ("challengeComposeControllerWithMessage:players:completionHandler:")]
		UIViewController ChallengeComposeController ([NullAllowed] string message, GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler completionHandler);

		[Deprecated (PlatformName.iOS, 26, 0)]
		[Deprecated (PlatformName.MacOSX, 26, 0)]
		[Deprecated (PlatformName.TvOS, 26, 0)]
		[Deprecated (PlatformName.MacCatalyst, 26, 0)]
		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("challengeComposeControllerWithMessage:players:completion:")]
		[Async (ResultTypeName = "GKChallengeComposeControllerResult")]
		UIViewController ChallengeComposeControllerWithMessage ([NullAllowed] string message, GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler2 completionHandler);

		[Deprecated (PlatformName.iOS, 26, 0)]
		[Deprecated (PlatformName.MacOSX, 26, 0)]
		[Deprecated (PlatformName.TvOS, 26, 0)]
		[Deprecated (PlatformName.MacCatalyst, 26, 0)]
		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="players">To be added.</param>
			<summary>Asynchronously selects the players who can earn the achievement.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous SelectChallengeablePlayers operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKPlayer[],Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The SelectChallengeablePlayersAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("selectChallengeablePlayers:withCompletionHandler:")]
		void SelectChallengeablePlayers (GKPlayer [] players, [NullAllowed] Action<GKPlayer []?, NSError?> completionHandler);

		[NoMac]
		[NoTV]
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("hidden", ArgumentSemantic.Assign)]
		bool Hidden { [Bind ("isHidden")] get; }

		[Static]
		[Export ("loadAchievementDescriptionsWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadAchievementDescriptions operation.   The value of the TResult parameter is a <see cref="GameKit.GKAchievementDescriptionHandler" />.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadAchievementDescriptions ([NullAllowed] GKAchievementDescriptionHandler handler);

		[MacCatalyst (14, 0)] // the headers lie, not usable until at least Mac Catalyst 14.0
		[Export ("loadImageWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para>The result is of type System.Threading.Tasks.Task&lt;AppKit.NSImage&gt; on MacOS and System.Threading.Tasks.Task&lt;UIKit.UIImage&gt; on iOS.</para>
			        </returns>
			<remarks>
			          <para copied="true">The LoadImageAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			        </remarks>
			""")]
		void LoadImage ([NullAllowed] GKImageLoadedHandler imageLoadedHandler);

		[Export ("groupIdentifier", ArgumentSemantic.Retain)]
		[NullAllowed]
		string GroupIdentifier { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		[Static]
		[Export ("incompleteAchievementImage")]
		UIImage IncompleteAchievementImage { get; }

		[MacCatalyst (14, 0)] // the headers lie, not usable until at least Mac Catalyst 14.0
		[Static]
		[Export ("placeholderCompletedAchievementImage")]
		UIImage PlaceholderCompletedAchievementImage { get; }

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("rarityPercent", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSNumber RarityPercent { get; }

		[TV (18, 4), Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4)]
		[Export ("releaseState", ArgumentSemantic.Assign)]
		GKReleaseState ReleaseState { get; }

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Export ("activityIdentifier")]
		string ActivityIdentifier { get; }

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Export ("activityProperties", ArgumentSemantic.Strong)]
		NSDictionary<NSString, NSString> ActivityProperties { get; }
	}

	interface IGKAchievementViewControllerDelegate { }

	/// <summary>A delegate object that allows <see cref="GameKit.GKAchievementViewController" /> to respond to lifecycle events.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKAchievementViewControllerDelegate_Ref/index.html">Apple documentation for <c>GKAchievementViewControllerDelegate</c></related>
	[NoTV]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKGameCenterViewController' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKAchievementViewControllerDelegate {
		/// <param name="viewController">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Abstract]
		[Export ("achievementViewControllerDidFinish:")]
		void DidFinish (GKAchievementViewController viewController);
	}

	/// <summary>A <see cref="GameKit.GKGameCenterViewController" /> that presents a standard user experience to display achievement progress.</summary>
	///     <remarks>
	///       <para>Application developers should use <see cref="GameKit.GKGameCenterViewController" /> instead of this class.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKAchievementViewController_Ref/index.html">Apple documentation for <c>GKAchievementViewController</c></related>
	[NoTV]
	[NoMacCatalyst]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKGameCenterViewController' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKGameCenterViewController' instead.")]
#if MONOMAC
	[BaseType (typeof (GKGameCenterViewController), Events = new Type [] { typeof (GKAchievementViewControllerDelegate) }, Delegates = new string [] { "WeakDelegate" })]
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

		/// <summary>An instance of the GameKit.IGKAchievementViewControllerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the GameKit.IGKAchievementViewControllerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		IGKAchievementViewControllerDelegate Delegate { get; set; }
	}

	[NoiOS]
	[NoMacCatalyst]
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

#if MONOMAC
	/// <summary>A <see cref="NSViewController" /> that presents a screen for sending friend requests.</summary>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKFriendRequestComposeViewController_Ref/index.html">Apple documentation for <c>GKFriendRequestComposeViewController</c></related>
#else
	/// <summary>A <see cref="UINavigationController" /> that presents a screen for sending friend requests.</summary>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKFriendRequestComposeViewController_Ref/index.html">Apple documentation for <c>GKFriendRequestComposeViewController</c></related>
#endif
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[Deprecated (PlatformName.iOS, 10, 0)]
	[NoMacCatalyst]
	[NoTV]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
#if MONOMAC
	[BaseType (typeof (NSViewController), Events = new Type [] { typeof (GKFriendRequestComposeViewControllerDelegate) }, Delegates = new string [] { "WeakComposeViewDelegate" })]
	interface GKFriendRequestComposeViewController : GKViewController {
		/// <param name="nibNameOrNull">To be added.</param>
		/// <param name="nibBundleOrNull">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
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

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
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

	/// <summary>A delegate object that allows fine-grained response to <see cref="GameKit.GKFriendRequestComposeViewController" /> life-cycle events.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKFriendRequestComposeViewControllerDelegate_Ref/index.html">Apple documentation for <c>GKFriendRequestComposeViewControllerDelegate</c></related>
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 10, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[Model]
	[Protocol]
	interface GKFriendRequestComposeViewControllerDelegate {
		/// <param name="viewController">To be added.</param>
		/// <summary>Developers should not use this deprecated method. </summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakComposeViewDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Abstract]
		[Export ("friendRequestComposeViewControllerDidFinish:")]
		void DidFinish (GKFriendRequestComposeViewController viewController);
	}

	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.iOS, 17, 0, message: "Use UNNotificationRequest or provide custom UI instead. This method will become a no-op in a future version of GameKit.")]
	[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use UNNotificationRequest or provide custom UI instead. This method will become a no-op in a future version of GameKit.")]
	[Deprecated (PlatformName.TvOS, 16, 1, message: "Use UNNotificationRequest or provide custom UI instead. This method will become a no-op in a future version of GameKit.")]
	[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use UNNotificationRequest or provide custom UI instead. This method will become a no-op in a future version of GameKit.")]
	[BaseType (typeof (NSObject))]
	partial interface GKNotificationBanner {
		[Static, Export ("showBannerWithTitle:message:completionHandler:")]
		[Async (XmlDocs = """
			<param name="title">Title for the message.This parameter can be .</param>
			<param name="message">Message to display.This parameter can be .</param>
			<summary>Shows a message for the specified time to the user, with a specified title.</summary>
			<returns>A task that represents the asynchronous Show operation</returns>
			<remarks>
			          <para copied="true">The ShowAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			        </remarks>
			""")]
		void Show ([NullAllowed] string title, [NullAllowed] string message, [NullAllowed] Action onCompleted);

		[Export ("showBannerWithTitle:message:duration:completionHandler:"), Static]
		[Async (XmlDocs = """
			<param name="title">To be added.</param>
			<param name="message">To be added.</param>
			<param name="durationSeconds">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
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

	/// <summary>A delegate object that is allows fine-grained response to <see cref="GameKit.GKTurnBasedEventHandler" /> events.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKTurnBasedEventHandlerDelegate_Ref/index.html">Apple documentation for <c>GKTurnBasedEventHandlerDelegate</c></related>
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKTurnBasedEventListener'.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKTurnBasedEventListener'.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKLocalPlayer.RegisterListener' with an object that implements 'IGKTurnBasedEventListener'.")]
	interface GKTurnBasedEventHandlerDelegate {
		/// <param name="playersToInvite">To be added.</param>
		/// <summary>Developers should not use this deprecated method. </summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("handleInviteFromGameCenter:")]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void HandleInviteFromGameCenter (NSString [] playersToInvite);

		/// <param name="match">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'HandleTurnEvent' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'HandleTurnEvent' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'HandleTurnEvent' instead.")]
		[Export ("handleTurnEventForMatch:")]
		void HandleTurnEventForMatch (GKTurnBasedMatch match);

		/// <param name="match">To be added.</param>
		/// <summary>Developers should not use this deprecated method. </summary>
		/// <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("handleMatchEnded:")]
		void HandleMatchEnded (GKTurnBasedMatch match);

		/// <param name="match">To be added.</param>
		/// <param name="activated">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("handleTurnEventForMatch:didBecomeActive:")]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void HandleTurnEvent (GKTurnBasedMatch match, bool activated);
	}

	/// <summary>A singleton object used to respond to important messages relating to turn-based matches.</summary>
	///     <remarks>
	///       <para>Application Developers should not instantiate this class, but rather use the <see cref="GameKit.GKTurnBasedEventHandler.SharedTurnBasedEventHandler" /> singleton.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKTurnBasedEventHandler_Ref/index.html">Apple documentation for <c>GKTurnBasedEventHandler</c></related>
	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use GKLocalPlayer.RegisterListener with an object that implements IGKTurnBasedEventListener.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use GKLocalPlayer.RegisterListener with an object that implements IGKTurnBasedEventListener.")]
	[BaseType (typeof (NSObject))]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use GKLocalPlayer.RegisterListener with an object that implements IGKTurnBasedEventListener.")]
	interface GKTurnBasedEventHandler {

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the GameKit.IGKTurnBasedEventHandlerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the GameKit.IGKTurnBasedEventHandlerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		IGKTurnBasedEventHandlerDelegate Delegate { get; set; }

		[Export ("sharedTurnBasedEventHandler"), Static]
		GKTurnBasedEventHandler SharedTurnBasedEventHandler { get; }
	}

	/// <param name="match">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate that specifies the completion handler for <see cref="GameKit.GKTurnBasedMatch.FindMatch(GameKit.GKMatchRequest,GameKit.GKTurnBasedMatchRequest)" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKTurnBasedMatchRequest ([NullAllowed] GKTurnBasedMatch match, [NullAllowed] NSError error);

	/// <param name="matches">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate used with <see cref="GameKit.GKTurnBasedMatch.LoadMatches(GameKit.GKTurnBasedMatchesRequest)" /> to specify behavior after the matches have been loaded.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKTurnBasedMatchesRequest ([NullAllowed] GKTurnBasedMatch [] matches, [NullAllowed] NSError error);

	/// <param name="matchData">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate that is used with <see cref="GameKit.GKTurnBasedMatch.LoadMatchData(GameKit.GKTurnBasedMatchData)" /> to specify behavior after the data is loaded.</summary>
	/// <remarks>To be added.</remarks>
	delegate void GKTurnBasedMatchData ([NullAllowed] NSData matchData, [NullAllowed] NSError error);

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
		[Async (XmlDocs = """
			<param name="request">To be added.</param>
			<summary>Asynchronously searches for and returns a match to join.</summary>
			<returns>
			          <para>A task that represents the asynchronous FindMatch operation.   The value of the TResult parameter is a GameKit.GKTurnBasedMatchRequest.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void FindMatch (GKMatchRequest request, GKTurnBasedMatchRequest onCompletion);

		[Static]
		[Export ("loadMatchesWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously loads all the matches for the current player.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadMatches operation.   The value of the TResult parameter is a GameKit.GKTurnBasedMatchesRequest.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadMatches ([NullAllowed] GKTurnBasedMatchesRequest onCompletion);

		[Export ("removeWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously removes the match from the Game Center.</summary>
			<returns>A task that represents the asynchronous Remove operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void Remove ([NullAllowed] Action<NSError?> onCompletion);

		[Export ("loadMatchDataWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously loads the match data.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadMatchData operation.   The value of the TResult parameter is a GameKit.GKTurnBasedMatchData.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadMatchData ([NullAllowed] GKTurnBasedMatchData onCompletion);

		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'EndTurn' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'EndTurn' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'EndTurn' instead.")]
		[Export ("endTurnWithNextParticipant:matchData:completionHandler:")]
		[Async (XmlDocs = """
			<param name="nextParticipant">To be added.</param>
			<param name="matchData">To be added.</param>
			<summary>Developers should not use this deprecated method. Developers should use 'EndTurn' instead.</summary>
			<returns>A task that represents the asynchronous EndTurnWithNextParticipant operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void EndTurnWithNextParticipant (GKTurnBasedParticipant nextParticipant, NSData matchData, [NullAllowed] Action<NSError?> noCompletion);

		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'ParticipantQuitInTurn (GKTurnBasedMatchOutcome, GKTurnBasedParticipant[], double, NSData, Action<NSError?>)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'ParticipantQuitInTurn (GKTurnBasedMatchOutcome, GKTurnBasedParticipant[], double, NSData, Action<NSError?>)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ParticipantQuitInTurn (GKTurnBasedMatchOutcome, GKTurnBasedParticipant[], double, NSData, Action<NSError?>)' instead.")]
		[Export ("participantQuitInTurnWithOutcome:nextParticipant:matchData:completionHandler:")]
		[Async (XmlDocs = """
			<param name="matchOutcome">To be added.</param>
			<param name="nextParticipant">To be added.</param>
			<param name="matchData">To be added.</param>
			<summary>Deprecated.</summary>
			<returns>A task that represents the asynchronous ParticipantQuitInTurn operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void ParticipantQuitInTurn (GKTurnBasedMatchOutcome matchOutcome, GKTurnBasedParticipant nextParticipant, NSData matchData, [NullAllowed] Action<NSError?> onCompletion);

		[Export ("participantQuitOutOfTurnWithOutcome:withCompletionHandler:")]
		[Async (XmlDocs = """
			<param name="matchOutcome">To be added.</param>
			<summary>Asynchronously resigns the player from the match out of turn.</summary>
			<returns>A task that represents the asynchronous ParticipantQuitOutOfTurn operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void ParticipantQuitOutOfTurn (GKTurnBasedMatchOutcome matchOutcome, [NullAllowed] Action<NSError?> onCompletion);

		[Export ("endMatchInTurnWithMatchData:completionHandler:")]
		[Async (XmlDocs = """
			<param name="matchData">To be added.</param>
			<summary>Asynchronously ends the match with the specified end state, scores, and achievements.</summary>
			<returns>A task that represents the asynchronous EndMatchInTurn operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void EndMatchInTurn (NSData matchData, [NullAllowed] Action<NSError?> onCompletion);

		[Static]
		[Export ("loadMatchWithID:withCompletionHandler:")]
		[Async (XmlDocs = """
			<param name="matchId">To be added.</param>
			<summary>Asynchronously loads the match that is identified by <paramref name="matchId" /> and returns it (<see langword="null" /> if an error occurs).</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadMatch operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKTurnBasedMatch,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LoadMatch (string matchId, [NullAllowed] Action<GKTurnBasedMatch?, NSError?> completionHandler);

		[Export ("acceptInviteWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously accepts an invitation to a match.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous AcceptInvite operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKTurnBasedMatch,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void AcceptInvite ([NullAllowed] Action<GKTurnBasedMatch?, NSError?> completionHandler);

		[Export ("declineInviteWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously declines an invitation to a match.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous DeclineInvite operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKTurnBasedMatch,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void DeclineInvite ([NullAllowed] Action<GKTurnBasedMatch, NSError> completionHandler);

		[Export ("matchDataMaximumSize")]
		nint MatchDataMaximumSize { get; }

		[MacCatalyst (13, 1)]
		[Export ("rematchWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Creates a new match with the same list of participants as the current match.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous Rematch operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKTurnBasedMatch,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void Rematch ([NullAllowed] Action<GKTurnBasedMatch?, NSError?> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("endTurnWithNextParticipants:turnTimeout:matchData:completionHandler:")]
		[Async (XmlDocs = """
			<param name="nextParticipants">To be added.</param>
			<param name="timeoutSeconds">To be added.</param>
			<param name="matchData">To be added.</param>
			<summary>Asynchronously ends the turn.</summary>
			<returns>A task that represents the asynchronous EndTurn operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void EndTurn (GKTurnBasedParticipant [] nextParticipants, double timeoutSeconds, NSData matchData, [NullAllowed] Action<NSError?> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("participantQuitInTurnWithOutcome:nextParticipants:turnTimeout:matchData:completionHandler:")]
		[Async (XmlDocs = """
			<param name="matchOutcome">To be added.</param>
			<param name="nextParticipants">To be added.</param>
			<param name="timeoutSeconds">To be added.</param>
			<param name="matchData">To be added.</param>
			<summary>Asynchronously resigns the current player from the match.</summary>
			<returns>A task that represents the asynchronous ParticipantQuitInTurn operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void ParticipantQuitInTurn (GKTurnBasedMatchOutcome matchOutcome, GKTurnBasedParticipant [] nextParticipants, double timeoutSeconds, NSData matchData, [NullAllowed] Action<NSError?> completionHandler);

		[Export ("saveCurrentTurnWithMatchData:completionHandler:")]
		[Async (XmlDocs = """
			<param name="matchData">To be added.</param>
			<summary>Asynchronously saves the current turn, does not advance to the next player.</summary>
			<returns>A task that represents the asynchronous SaveCurrentTurn operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void SaveCurrentTurn (NSData matchData, [NullAllowed] Action<NSError?> completionHandler);

		/// <summary>Represents the value associated with the constant GKTurnTimeoutDefault</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("GKTurnTimeoutDefault"), Static]
		double DefaultTimeout { get; }

		/// <summary>Represents the value associated with the constant GKTurnTimeoutNone</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
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
#if XAMCORE_5_0
		nuint ExchangeDataMaximumSize { get; }
#else
		[Obsolete ("Use 'ExchangeDataMaximumSize' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		nuint ExhangeDataMaximumSize { get; }

		[MacCatalyst (13, 1)]
		[Wrap ("ExhangeDataMaximumSize")]
		nuint ExchangeDataMaximumSize { get; }
#endif

		[MacCatalyst (13, 1)]
		[Export ("exchangeMaxInitiatedExchangesPerPlayer")]
		nuint ExchangeMaxInitiatedExchangesPerPlayer { get; }

		[MacCatalyst (13, 1)]
		[Export ("setLocalizableMessageWithKey:arguments:")]
		void SetMessage (string localizableMessage, [NullAllowed] params NSObject [] arguments);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'EndMatchInTurn (NSData, GKLeaderboardScore[], NSObject[], Action<NSError?>)' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'EndMatchInTurn (NSData, GKLeaderboardScore[], NSObject[], Action<NSError?>)' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'EndMatchInTurn (NSData, GKLeaderboardScore[], NSObject[], Action<NSError?>)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'EndMatchInTurn (NSData, GKLeaderboardScore[], NSObject[], Action<NSError?>)' instead.")]
		[Export ("endMatchInTurnWithMatchData:scores:achievements:completionHandler:")]
		[Async (XmlDocs = """
			<param name="matchData">To be added.</param>
			<param name="scores">To be added.</param>
			<param name="achievements">To be added.</param>
			<summary>Asynchronously the specified end state, scores, and achievements.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		void EndMatchInTurn (NSData matchData, [NullAllowed] GKScore [] scores, [NullAllowed] GKAchievement [] achievements, [NullAllowed] Action<NSError?> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("saveMergedMatchData:withResolvedExchanges:completionHandler:")]
		[Async (XmlDocs = """
			<param name="matchData">To be added.</param>
			<param name="exchanges">To be added.</param>
			<summary>Asynchronously saves merged match data without advancing play.</summary>
			<returns>A task that represents the asynchronous SaveMergedMatchData operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void SaveMergedMatchData (NSData matchData, GKTurnBasedExchange [] exchanges, [NullAllowed] Action<NSError?> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("sendExchangeToParticipants:data:localizableMessageKey:arguments:timeout:completionHandler:")]
		[Async (XmlDocs = """
			<param name="participants">To be added.</param>
			<param name="data">To be added.</param>
			<param name="localizableMessage">To be added.</param>
			<param name="arguments">To be added.</param>
			<param name="timeout">To be added.</param>
			<summary>Sends exchange requests to the players who are listed in <paramref name="participants" />.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous SendExchange operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKTurnBasedExchange,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void SendExchange (GKTurnBasedParticipant [] participants, NSData data, string localizableMessage, NSObject [] arguments, double timeout, [NullAllowed] Action<GKTurnBasedExchange?, NSError?> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("sendReminderToParticipants:localizableMessageKey:arguments:completionHandler:")]
		[Async (XmlDocs = """
			<param name="participants">To be added.</param>
			<param name="localizableMessage">To be added.</param>
			<param name="arguments">To be added.</param>
			<summary>Sends a reminder to the players who are listed in <paramref name="participants" />.</summary>
			<returns>A task that represents the asynchronous SendReminder operation</returns>
			<remarks>
			          <para copied="true">The SendReminderAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void SendReminder (GKTurnBasedParticipant [] participants, string localizableMessage, NSObject [] arguments, [NullAllowed] Action<NSError?> completionHandler);

		[iOS (14, 0)]
		[TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("endMatchInTurnWithMatchData:leaderboardScores:achievements:completionHandler:")]
		[Async]
		void EndMatchInTurn (NSData matchData, GKLeaderboardScore [] scores, NSObject [] achievements, Action<NSError?> completionHandler);
	}

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
		/// <param name="nibNameOrNull">To be added.</param>
		/// <param name="nibBundleOrNull">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[Export ("showExistingMatches", ArgumentSemantic.Assign)]
		bool ShowExistingMatches { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("matchmakingMode", ArgumentSemantic.Assign)]
		GKMatchmakingMode MatchmakingMode { get; set; }

		[Export ("initWithMatchRequest:")]
		NativeHandle Constructor (GKMatchRequest request);

		[Export ("turnBasedMatchmakerDelegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the GameKit.IGKTurnBasedMatchmakerViewControllerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the GameKit.IGKTurnBasedMatchmakerViewControllerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		IGKTurnBasedMatchmakerViewControllerDelegate Delegate { get; set; }
	}

	interface IGKTurnBasedMatchmakerViewControllerDelegate { }

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKTurnBasedMatchmakerViewControllerDelegate {
#if !XAMCORE_5_0
		/// <param name="viewController">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
#endif
		[Export ("turnBasedMatchmakerViewControllerWasCancelled:")]
		void WasCancelled (GKTurnBasedMatchmakerViewController viewController);

#if !XAMCORE_5_0
		/// <param name="viewController">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
#endif
		[Export ("turnBasedMatchmakerViewController:didFailWithError:")]
		void FailedWithError (GKTurnBasedMatchmakerViewController viewController, NSError error);

		/// <param name="viewController">To be added.</param>
		/// <param name="match">To be added.</param>
		/// <summary>Developers should not use this deprecated method. Developers should use 'GKTurnBasedEventListener.ReceivedTurnEvent' instead.</summary>
		/// <remarks>To be added.</remarks>
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'GKTurnBasedEventListener.ReceivedTurnEvent' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'GKTurnBasedEventListener.ReceivedTurnEvent' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKTurnBasedEventListener.ReceivedTurnEvent' instead.")]
		[Export ("turnBasedMatchmakerViewController:didFindMatch:")]
		void FoundMatch (GKTurnBasedMatchmakerViewController viewController, GKTurnBasedMatch match);

		/// <param name="viewController">To be added.</param>
		/// <param name="match">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'GKTurnBasedEventListener.WantsToQuitMatch' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'GKTurnBasedEventListener.WantsToQuitMatch' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKTurnBasedEventListener.WantsToQuitMatch' instead.")]
		[Export ("turnBasedMatchmakerViewController:playerQuitForMatch:")]
		void PlayerQuitForMatch (GKTurnBasedMatchmakerViewController viewController, GKTurnBasedMatch match);
	}

	[Deprecated (PlatformName.iOS, 26, 0)]
	[Deprecated (PlatformName.MacOSX, 26, 0)]
	[Deprecated (PlatformName.TvOS, 26, 0)]
	[Deprecated (PlatformName.MacCatalyst, 26, 0)]
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
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadReceivedChallenges operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKChallenge[],Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The LoadReceivedChallengesAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void LoadReceivedChallenges ([NullAllowed] Action<GKChallenge []?, NSError?> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("issuingPlayer", ArgumentSemantic.Copy)]
		[NullAllowed]
		GKPlayer IssuingPlayer { get; }

		[MacCatalyst (13, 1)]
		[Export ("receivingPlayer", ArgumentSemantic.Copy)]
		[NullAllowed]
		GKPlayer ReceivingPlayer { get; }
	}

	[Deprecated (PlatformName.iOS, 26, 0)]
	[Deprecated (PlatformName.MacOSX, 26, 0)]
	[Deprecated (PlatformName.TvOS, 26, 0)]
	[Deprecated (PlatformName.MacCatalyst, 26, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKChallenge))]
	interface GKScoreChallenge {

		[Deprecated (PlatformName.iOS, 17, 4, message: "Use 'GKScoreChallenge.LeaderboardEntry' instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 4, message: "Use 'GKScoreChallenge.LeaderboardEntry' instead.")]
		[Deprecated (PlatformName.TvOS, 17, 4, message: "Use 'GKScoreChallenge.LeaderboardEntry' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 4, message: "Use 'GKScoreChallenge.LeaderboardEntry' instead.")]
		[Export ("score", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKScore Score { get; }

		[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[NullAllowed, Export ("leaderboardEntry", ArgumentSemantic.Retain)]
		GKLeaderboardEntry LeaderboardEntry { get; }
	}

	[Deprecated (PlatformName.iOS, 26, 0)]
	[Deprecated (PlatformName.MacOSX, 26, 0)]
	[Deprecated (PlatformName.TvOS, 26, 0)]
	[Deprecated (PlatformName.MacCatalyst, 26, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKChallenge))]
	interface GKAchievementChallenge {

		[Export ("achievement", ArgumentSemantic.Retain)]
		[NullAllowed]
		GKAchievement Achievement { get; }
	}

	[Deprecated (PlatformName.iOS, 26, 0)]
	[Deprecated (PlatformName.MacOSX, 26, 0)]
	[Deprecated (PlatformName.TvOS, 26, 0)]
	[Deprecated (PlatformName.MacCatalyst, 26, 0)]
	[DisableDefaultCtor] // the native 'init' method returned nil.
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
		/// <param name="nibNameOrNull">To be added.</param>
		/// <param name="nibBundleOrNull">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithLeaderboardID:playerScope:timeScope:")]
		NativeHandle Constructor (string leaderboardId, GKLeaderboardPlayerScope playerScope, GKLeaderboardTimeScope timeScope);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithLeaderboard:playerScope:")]
		NativeHandle Constructor (GKLeaderboard leaderboard, GKLeaderboardPlayerScope playerScope);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithAchievementID:")]
		[Internal]
		NativeHandle _InitWithAchievementId (string achievementId);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithState:")]
		NativeHandle Constructor (GKGameCenterViewControllerState state);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("initWithLeaderboardSetID:")]
		[Internal]
		NativeHandle _InitWithLeaderboardSetId (string leaderboardSetId);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("initWithPlayer:")]
		NativeHandle Constructor (GKPlayer player);

		[Export ("gameCenterDelegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the GameKit.IGKGameCenterControllerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the GameKit.IGKGameCenterControllerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
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

	[Deprecated (PlatformName.iOS, 26, 0)]
	[Deprecated (PlatformName.MacOSX, 26, 0)]
	[Deprecated (PlatformName.TvOS, 26, 0)]
	[Deprecated (PlatformName.MacCatalyst, 26, 0)]
	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	interface GKGameCenterControllerDelegate {
		/// <param name="controller">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Abstract]
		[Export ("gameCenterViewControllerDidFinish:")]
		void Finished (GKGameCenterViewController controller);
	}

	/// <summary>The singleton <see cref="GameKit.GKChallengeEventHandler.Instance" /> is used to respond to events relating to <see cref="GameKit.GKChallenge" />s.</summary>
	///     <remarks>
	///       <para>Application developers should not instantiate new instances of this close. Rather, they should use the <see cref="GameKit.GKChallengeEventHandler.Instance" /> singleton.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKChallengeEventHandler_Ref/index.html">Apple documentation for <c>GKChallengeEventHandler</c></related>
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

		/// <summary>An instance of the GameKit.IGKChallengeEventHandlerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the GameKit.IGKChallengeEventHandlerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		IGKChallengeEventHandlerDelegate Delegate { get; set; }

		[Export ("challengeEventHandler"), Static]
		GKChallengeEventHandler Instance { get; }
	}

	interface IGKChallengeEventHandlerDelegate { }

	/// <summary>A delegate object that allows the application developer fine-grained response to life-cycle events relating to <see cref="GameKit.GKChallenge" />s, such as receiving or completing a challenge.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GLkit/Reference/GKChallengeEventHandlerDelegate_Ref/index.html">Apple documentation for <c>GKChallengeEventHandlerDelegate</c></related>
	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement the 'IGKChallengeListener' interface and register a listener with 'GKLocalPlayer'.")]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	interface GKChallengeEventHandlerDelegate {
		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("localPlayerDidSelectChallenge:")]
		void LocalPlayerSelectedChallenge (GKChallenge challenge);

		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Delegate invoked by the object to get a value.</summary>
			<value>To be added.</value>
			<remarks>Developers assign a function, delegate or anonymous method to this property to return a value to the object.   If developers assign a value to this property, it this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("shouldShowBannerForLocallyReceivedChallenge:")]
		[DelegateName ("GKChallengePredicate"), DefaultValue (true)]
		bool ShouldShowBannerForLocallyReceivedChallenge (GKChallenge challenge);

		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("localPlayerDidReceiveChallenge:")]
		void LocalPlayerReceivedChallenge (GKChallenge challenge);

		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Delegate invoked by the object to get a value.</summary>
			<value>To be added.</value>
			<remarks>Developers assign a function, delegate or anonymous method to this property to return a value to the object.   If developers assign a value to this property, it this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("shouldShowBannerForLocallyCompletedChallenge:")]
		[DelegateName ("GKChallengePredicate"), DefaultValue (true)]
		bool ShouldShowBannerForLocallyCompletedChallenge (GKChallenge challenge);

		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("localPlayerDidCompleteChallenge:")]
		void LocalPlayerCompletedChallenge (GKChallenge challenge);

		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Delegate invoked by the object to get a value.</summary>
			<value>To be added.</value>
			<remarks>Developers assign a function, delegate or anonymous method to this property to return a value to the object.   If developers assign a value to this property, it this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("shouldShowBannerForRemotelyCompletedChallenge:")]
		[DelegateName ("GKChallengePredicate"), DefaultValue (true)]
		bool ShouldShowBannerForRemotelyCompletedChallenge (GKChallenge challenge);

		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
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
		[Async (XmlDocs = """
			<param name="localizableMessage">To be added.</param>
			<param name="arguments">To be added.</param>
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous Cancel operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void Cancel (string localizableMessage, NSObject [] arguments, [NullAllowed] Action<NSError?> completionHandler);

		[Export ("replyWithLocalizableMessageKey:arguments:data:completionHandler:")]
		[Async (XmlDocs = """
			<param name="localizableMessage">To be added.</param>
			<param name="arguments">To be added.</param>
			<param name="data">To be added.</param>
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous Reply operation</returns>
			<remarks>
			          <para copied="true">The ReplyAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void Reply (string localizableMessage, NSObject [] arguments, NSData data, [NullAllowed] Action<NSError?> completionHandler);

		/// <summary>Represents the value associated with the constant GKExchangeTimeoutDefault</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("GKExchangeTimeoutDefault")]
		double TimeoutDefault { get; }

		/// <summary>Represents the value associated with the constant GKExchangeTimeoutNone</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
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
#if !TVOS
		, GKSavedGameListener
#endif
		, GKChallengeListener, GKInviteEventListener {
	}

	[MacCatalyst (13, 1)]
	[Model, Protocol, BaseType (typeof (NSObject))]
	interface GKChallengeListener {
		/// <param name="player">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:wantsToPlayChallenge:")]
		void WantsToPlayChallenge (GKPlayer player, GKChallenge challenge);

		/// <param name="player">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:didReceiveChallenge:")]
		void DidReceiveChallenge (GKPlayer player, GKChallenge challenge);

		/// <param name="player">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <param name="friendPlayer">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:didCompleteChallenge:issuedByFriend:")]
		void DidCompleteChallenge (GKPlayer player, GKChallenge challenge, GKPlayer friendPlayer);

		/// <param name="player">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <param name="friendPlayer">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:issuedChallengeWasCompleted:byFriend:")]
		void IssuedChallengeWasCompleted (GKPlayer player, GKChallenge challenge, GKPlayer friendPlayer);
	}

	[MacCatalyst (13, 1)]
	[Protocol, Model, BaseType (typeof (NSObject))]
	interface GKInviteEventListener {
		/// <param name="player">To be added.</param>
		/// <param name="invite">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("player:didAcceptInvite:")]
		void DidAcceptInvite (GKPlayer player, GKInvite invite);

		/// <param name="player">To be added.</param>
		/// <param name="playerIDs">To be added.</param>
		/// <summary>Developers should not use this deprecated method. Developers should use 'DidRequestMatch (GKPlayer player, GKPlayer[] recipientPlayers)' instead.</summary>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'DidRequestMatch (GKPlayer player, GKPlayer[] recipientPlayers)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidRequestMatch (GKPlayer player, GKPlayer[] recipientPlayers)' instead.")]
		[Export ("player:didRequestMatchWithPlayers:")]
		void DidRequestMatch (GKPlayer player, string [] playerIDs);

		/// <param name="player">To be added.</param>
		/// <param name="recipientPlayers">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("player:didRequestMatchWithRecipients:")]
		void DidRequestMatch (GKPlayer player, GKPlayer [] recipientPlayers);
	}

	/// <summary>Listens for events in turn-based games.</summary>
	/// <remarks>To be added.</remarks>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKTurnBasedEventListener_Ref/index.html">Apple documentation for <c>GKTurnBasedEventListener</c></related>
	[MacCatalyst (13, 1)]
	[Model, Protocol, BaseType (typeof (NSObject))]
	interface GKTurnBasedEventListener {
		/// <param name="player">To be added.</param>
		/// <param name="playerIDsToInvite">To be added.</param>
		/// <summary>Developers should not use this deprecated method. Developers should use 'DidRequestMatchWithOtherPlayers' instead.</summary>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'DidRequestMatchWithOtherPlayers' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidRequestMatchWithOtherPlayers' instead.")]
		[Export ("player:didRequestMatchWithPlayers:")]
		void DidRequestMatchWithPlayers (GKPlayer player, string [] playerIDsToInvite);

		/// <param name="player">To be added.</param>
		/// <param name="match">To be added.</param>
		/// <param name="becameActive">To be added.</param>
		/// <summary>Method that is called to activate a turn for <paramref name="player" />.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:receivedTurnEventForMatch:didBecomeActive:")]
		void ReceivedTurnEvent (GKPlayer player, GKTurnBasedMatch match, bool becameActive);

		/// <param name="player">To be added.</param>
		/// <param name="match">To be added.</param>
		/// <summary>Method that is called after the <paramref name="match" /> is ended.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:matchEnded:")]
		void MatchEnded (GKPlayer player, GKTurnBasedMatch match);

		/// <param name="player">To be added.</param>
		/// <param name="exchange">To be added.</param>
		/// <param name="match">To be added.</param>
		/// <summary>Method that is called when <paramref name="player" /> receives a request for an <paramref name="exchange" />.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:receivedExchangeRequest:forMatch:")]
		void ReceivedExchangeRequest (GKPlayer player, GKTurnBasedExchange exchange, GKTurnBasedMatch match);

		/// <param name="player">To be added.</param>
		/// <param name="exchange">To be added.</param>
		/// <param name="match">To be added.</param>
		/// <summary>Method that is called after <paramref name="player" /> cancels the <paramref name="exchange" />.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:receivedExchangeCancellation:forMatch:")]
		void ReceivedExchangeCancellation (GKPlayer player, GKTurnBasedExchange exchange, GKTurnBasedMatch match);

		/// <param name="player">To be added.</param>
		/// <param name="replies">To be added.</param>
		/// <param name="exchange">To be added.</param>
		/// <param name="match">To be added.</param>
		/// <summary>Method that is called after the <paramref name="exchange" /> with <paramref name="player" /> completes.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("player:receivedExchangeReplies:forCompletedExchange:forMatch:")]
		void ReceivedExchangeReplies (GKPlayer player, GKTurnBasedExchangeReply [] replies, GKTurnBasedExchange exchange, GKTurnBasedMatch match);

		/// <param name="player">To be added.</param>
		/// <param name="playersToInvite">To be added.</param>
		/// <summary>Method that is called when <paramref name="player" /> requests a match with <paramref name="playersToInvite" />.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("player:didRequestMatchWithOtherPlayers:")]
		void DidRequestMatchWithOtherPlayers (GKPlayer player, GKPlayer [] playersToInvite);

		/// <param name="player">To be added.</param>
		/// <param name="match">To be added.</param>
		/// <summary>Method that is called after <paramref name="player" /> indicates that they desire to quit the match.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("player:wantsToQuitMatch:")]
		void WantsToQuitMatch (GKPlayer player, GKTurnBasedMatch match);
	}

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

		/// <param name="containerName">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="title">To be added.</param>
		/// <param name="maxPlayers">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Async (XmlDocs = """
			<param name="containerName">To be added.</param>
			<param name="title">To be added.</param>
			<param name="maxPlayers">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous CreateSession operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKGameSession,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Static]
		[Export ("createSessionInContainer:withTitle:maxConnectedPlayers:completionHandler:")]
		void CreateSession ([NullAllowed] string containerName, string title, nint maxPlayers, Action<GKGameSession?, NSError?> completionHandler);

		[Async (XmlDocs = """
			<param name="containerName">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadSessions operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKGameSession[],Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Static]
		[Export ("loadSessionsInContainer:completionHandler:")]
		void LoadSessions ([NullAllowed] string containerName, Action<GKGameSession []?, NSError?> completionHandler);

		[Async (XmlDocs = """
			<param name="identifier">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadSession operation.  The value of the TResult parameter is of type System.Action&lt;GameKit.GKGameSession,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Static]
		[Export ("loadSessionWithIdentifier:completionHandler:")]
		void LoadSession (string identifier, Action<GKGameSession?, NSError?> completionHandler);

		[Async (XmlDocs = """
			<param name="identifier">To be added.</param>
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous RemoveSession operation</returns>
			<remarks>To be added.</remarks>
			""")]
		[Static]
		[Export ("removeSessionWithIdentifier:completionHandler:")]
		void RemoveSession (string identifier, Action<NSError?> completionHandler);

		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous GetShareUrl operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSUrl,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("getShareURLWithCompletionHandler:")]
		void GetShareUrl (Action<NSUrl?, NSError?> completionHandler);

		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadData operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSData,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("loadDataWithCompletionHandler:")]
		void LoadData (Action<NSData?, NSError?> completionHandler);

		[Async (XmlDocs = """
			<param name="data">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous SaveData operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSData,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("saveData:completionHandler:")]
		void SaveData (NSData data, Action<NSData?, NSError?> completionHandler);

		[Async (XmlDocs = """
			<param name="state">To be added.</param>
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous SetConnectionState operation</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("setConnectionState:completionHandler:")]
		void SetConnectionState (GKConnectionState state, Action<NSError?> completionHandler);

		[Export ("playersWithConnectionState:")]
		GKCloudPlayer [] GetPlayers (GKConnectionState state);

		[Async (XmlDocs = """
			<param name="data">To be added.</param>
			<param name="transport">To be added.</param>
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous SendData operation</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("sendData:withTransportType:completionHandler:")]
		void SendData (NSData data, GKTransportType transport, Action<NSError?> completionHandler);

		[Async (XmlDocs = """
			<param name="key">To be added.</param>
			<param name="arguments">To be added.</param>
			<param name="data">To be added.</param>
			<param name="players">To be added.</param>
			<param name="badgePlayers">To be added.</param>
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous SendMessage operation</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("sendMessageWithLocalizedFormatKey:arguments:data:toPlayers:badgePlayers:completionHandler:")]
		void SendMessage (string key, string [] arguments, [NullAllowed] NSData data, GKCloudPlayer [] players, bool badgePlayers, Action<NSError?> completionHandler);

		[Async (XmlDocs = """
			<param name="players">To be added.</param>
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous ClearBadge operation</returns>
			<remarks>
			          <para copied="true">The ClearBadgeAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("clearBadgeForPlayers:completionHandler:")]
		void ClearBadge (GKCloudPlayer [] players, Action<NSError?> completionHandler);

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

	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GKLocalPlayerListener' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GKLocalPlayerListener' instead.")]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GKLocalPlayerListener' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GKLocalPlayerListener' instead.")]
	[Protocol]
	interface GKGameSessionEventListener {
		/// <param name="session">To be added.</param>
		/// <param name="player">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("session:didAddPlayer:")]
		void DidAddPlayer (GKGameSession session, GKCloudPlayer player);

		/// <param name="session">To be added.</param>
		/// <param name="player">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("session:didRemovePlayer:")]
		void DidRemovePlayer (GKGameSession session, GKCloudPlayer player);

		/// <param name="session">To be added.</param>
		/// <param name="player">To be added.</param>
		/// <param name="newState">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("session:player:didChangeConnectionState:")]
		void DidChangeConnectionState (GKGameSession session, GKCloudPlayer player, GKConnectionState newState);

		/// <param name="session">To be added.</param>
		/// <param name="player">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("session:player:didSaveData:")]
		void DidSaveData (GKGameSession session, GKCloudPlayer player, NSData data);

		/// <param name="session">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <param name="player">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("session:didReceiveData:fromPlayer:")]
		void DidReceiveData (GKGameSession session, NSData data, GKCloudPlayer player);

		/// <param name="session">To be added.</param>
		/// <param name="message">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <param name="player">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("session:didReceiveMessage:withData:fromPlayer:")]
		void DidReceiveMessage (GKGameSession session, string message, NSData data, GKCloudPlayer player);
	}

	[NoMac]
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

	[NoiOS, NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	interface GKChallengesViewControllerDelegate {

		/// <param name="viewController">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("challengesViewControllerDidFinish:")]
		void DidFinish (GKChallengesViewController viewController);
	}

	[NoiOS, NoTV]
	[Deprecated (PlatformName.MacOSX, 10, 10)]
	[NoMacCatalyst]
	[BaseType (typeof (NSViewController))]
	interface GKChallengesViewController : GKViewController {

		/// <param name="nibName">To be added.</param>
		/// <param name="bundle">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[NullAllowed, Export ("challengeDelegate", ArgumentSemantic.Assign)]
		IGKChallengesViewControllerDelegate ChallengeDelegate { get; set; }
	}

	[NoiOS, NoTV]
	[NoMacCatalyst]
	[Protocol]
	interface GKViewController {
	}

	interface IGKSessionDelegate { }

	/// <summary>Delegate for the GKSession class.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameKit/Reference/GKSessionDelegate_Protocol/index.html">Apple documentation for <c>GKSessionDelegate</c></related>
	[NoTV]
	[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'MultipeerConnectivity.MCSessionDelegate' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'MultipeerConnectivity.MCSessionDelegate' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MultipeerConnectivity.MCSessionDelegate' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GKSessionDelegate {
		/// <param name="session">To be added.</param>
		/// <param name="peerID">To be added.</param>
		/// <param name="state">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("session:peer:didChangeState:")]
		void PeerChangedState (GKSession session, string peerID, GKPeerConnectionState state);

		/// <param name="session">To be added.</param>
		/// <param name="peerID">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("session:didReceiveConnectionRequestFromPeer:")]
		void PeerConnectionRequest (GKSession session, string peerID);

		/// <param name="session">To be added.</param>
		/// <param name="peerID">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("session:connectionWithPeerFailed:withError:")]
		void PeerConnectionFailed (GKSession session, string peerID, NSError error);

		/// <param name="session">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("session:didFailWithError:")]
		void FailedWithError (GKSession session, NSError error);
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
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

		[Deprecated (PlatformName.iOS, 26, 0, message: "Unsupported.")]
		[Deprecated (PlatformName.MacOSX, 26, 0, message: "Unsupported.")]
		[Deprecated (PlatformName.TvOS, 26, 0, message: "Unsupported.")]
		[Deprecated (PlatformName.MacCatalyst, 26, 0, message: "Unsupported.")]
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

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("triggerAccessPointWithAchievementID:handler:")]
		void TriggerAccessPointWithAchievementId (string achievementId, [NullAllowed] Action handler);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("triggerAccessPointWithLeaderboardSetID:handler:")]
		void TriggerAccessPointWithLeaderboardSetId (string leaderboardSetId, [NullAllowed] Action handler);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("triggerAccessPointWithLeaderboardID:playerScope:timeScope:handler:")]
		void TriggerAccessPoint (string leaderboardId, GKLeaderboardPlayerScope playerScope, GKLeaderboardTimeScope timeScope, [NullAllowed] Action handler);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("triggerAccessPointWithPlayer:handler:")]
		void TriggerAccessPoint (GKPlayer player, [NullAllowed] Action handler);

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Export ("triggerAccessPointForPlayTogetherWithHandler:")]
		void TriggerAccessPointForPlayTogether ([NullAllowed] Action handler);

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Export ("triggerAccessPointForChallengesWithHandler:")]
		void TriggerAccessPointForChallenges ([NullAllowed] Action handler);

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Export ("triggerAccessPointWithChallengeDefinitionID:handler:")]
		void TriggerAccessPointWithChallengeDefinitionId (string challengeDefinitionId, [NullAllowed] Action handler);

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Export ("triggerAccessPointWithGameActivityDefinitionID:handler:")]
		void TriggerAccessPointWithGameActivityDefinitionId (string gameActivityDefinitionId, [NullAllowed] Action handler);

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Export ("triggerAccessPointWithGameActivity:handler:")]
		void TriggerAccessPointWithGameActivity (GKGameActivity gameActivity, [NullAllowed] Action handler);

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Export ("triggerAccessPointForFriendingWithHandler:")]
		void TriggerAccessPointForFriending ([NullAllowed] Action handler);

		[NoTV, Mac (26, 2), iOS (26, 2), MacCatalyst (26, 2)]
		[Export ("triggerAccessPointForArcadeWithHandler:")]
		void TriggerAccessPointForArcade ([NullAllowed] Action handler);
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKLeaderboardEntry {
		[Export ("player", ArgumentSemantic.Strong)]
		GKPlayer Player { get; }

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

		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "GKChallengeComposeResult")]
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0)]
		[Deprecated (PlatformName.MacOSX, 14, 0)]
		[Export ("challengeComposeControllerWithMessage:players:completionHandler:")]
		UIViewController ChallengeComposeController ([NullAllowed] string message, [NullAllowed] GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler completionHandler);

		[Deprecated (PlatformName.iOS, 26, 0)]
		[Deprecated (PlatformName.MacOSX, 26, 0)]
		[Deprecated (PlatformName.TvOS, 26, 0)]
		[Deprecated (PlatformName.MacCatalyst, 26, 0)]
		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("challengeComposeControllerWithMessage:players:completion:")]
		[Async (ResultTypeName = "GKChallengeComposeControllerResult")]
		UIViewController ChallengeComposeControllerWithMessage ([NullAllowed] string message, [NullAllowed] GKPlayer [] players, [NullAllowed] GKChallengeComposeHandler2 completionHandler);
	}

	[TV (14, 0), iOS (14, 0)]
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

	[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
	[BaseType (typeof (NSObject))]
	interface GKMatchedPlayers {

		[NullAllowed, Export ("properties")]
		NSDictionary<NSString, NSObject> Properties { get; }

		[Export ("players")]
		GKPlayer [] Players { get; }

		[NullAllowed, Export ("playerProperties")]
		NSDictionary<GKPlayer, NSDictionary<NSString, NSObject>> PlayerProperties { get; }
	}

	[TV (18, 4), Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4)]
	[Native]
	public enum GKReleaseState : ulong {
		Unknown,
		Released,
		Prereleased,
	}

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	delegate void GKChallengeDefinitionLoadImageHandler ([NullAllowed] UIImage image, [NullAllowed] NSError error);

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	delegate void GKChallengeDefinitionLoadDefinitionsHandler ([NullAllowed] GKChallengeDefinition [] definitions, [NullAllowed] NSError error);

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	delegate void GKChallengeDefinitionHasActiveChallengesHandler (bool hasActiveChallenges, [NullAllowed] NSError error);

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GKChallengeDefinition {

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("groupIdentifier")]
		string GroupIdentifier { get; }

		[Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("details")]
		string Details { get; }

		[Export ("durationOptions", ArgumentSemantic.Copy)]
		NSDateComponents [] DurationOptions { get; }

		[Export ("isRepeatable")]
		bool IsRepeatable { get; }

		[NullAllowed, Export ("leaderboard", ArgumentSemantic.Strong)]
		GKLeaderboard Leaderboard { get; }

		[Export ("releaseState")]
		GKReleaseState ReleaseState { get; }

		[Async]
		[Export ("loadImageWithCompletionHandler:")]
		void LoadImage (GKChallengeDefinitionLoadImageHandler completionHandler);

		[Static]
		[Async]
		[Export ("loadChallengeDefinitionsWithCompletionHandler:")]
		void LoadDefinitions (GKChallengeDefinitionLoadDefinitionsHandler completionHandler);

		[Async]
		[Export ("hasActiveChallengesWithCompletionHandler:")]
		void HasActiveChallenges (GKChallengeDefinitionHasActiveChallengesHandler completionHandler);
	}

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	delegate void GKGameActivityDefinitionLoadAchievementDescriptionsHandler ([NullAllowed] GKAchievementDescription [] descriptions, [NullAllowed] NSError error);

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	delegate void GKGameActivityDefinitionLoadLeaderboardsHandler ([NullAllowed] GKLeaderboard [] leaderboards, [NullAllowed] NSError error);

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	delegate void GKGameActivityDefinitionLoadImageHandler ([NullAllowed] UIImage image, [NullAllowed] NSError error);

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	delegate void GKGameActivityDefinitionLoadGameActivityDefinitionsHandler ([NullAllowed] GKGameActivityDefinition [] definitions, [NullAllowed] NSError error);

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GKGameActivityDefinition {

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("groupIdentifier")]
		string GroupIdentifier { get; }

		[Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("details")]
		string Details { get; }

		[Export ("defaultProperties", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSString> DefaultProperties { get; }

		[NullAllowed, Export ("fallbackURL", ArgumentSemantic.Strong)]
		NSUrl FallbackUrl { get; }

		[Export ("supportsPartyCode")]
		bool SupportsPartyCode { get; }

		[Export ("maxPlayers", ArgumentSemantic.Strong)]
		[BindAs (typeof (nuint?))]
		NSNumber MaxPlayers { get; }

		[Export ("minPlayers", ArgumentSemantic.Strong)]
		[BindAs (typeof (nuint?))]
		NSNumber MinPlayers { get; }

		[Export ("supportsUnlimitedPlayers")]
		bool SupportsUnlimitedPlayers { get; }

		[Export ("playStyle", ArgumentSemantic.Assign)]
		GKGameActivityPlayStyle PlayStyle { get; }

		[Export ("releaseState")]
		GKReleaseState ReleaseState { get; }

		[Async]
		[Export ("loadAchievementDescriptionsWithCompletionHandler:")]
		void LoadAchievementDescriptions (GKGameActivityDefinitionLoadAchievementDescriptionsHandler completionHandler);

		[Async]
		[Export ("loadLeaderboardsWithCompletionHandler:")]
		void LoadLeaderboards (GKGameActivityDefinitionLoadLeaderboardsHandler completionHandler);

		[Async]
		[Export ("loadImageWithCompletionHandler:")]
		void LoadImage (GKGameActivityDefinitionLoadImageHandler completionHandler);

		[Async]
		[Static]
		[Export ("loadGameActivityDefinitionsWithCompletionHandler:")]
		void LoadDefinitions (GKGameActivityDefinitionLoadGameActivityDefinitionsHandler completionHandler);

		[Async]
		[Static]
		[Export ("loadGameActivityDefinitionsWithIDs:completionHandler:")]
		void LoadDefinitions ([NullAllowed] string [] activityDefinitionIds, GKGameActivityDefinitionLoadGameActivityDefinitionsHandler completionHandler);
	}

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	delegate void GKGameActivityFindMatchHandler ([NullAllowed] GKMatch match, [NullAllowed] NSError error);

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	delegate void GKGameActivityFindPlayersForHostedMatchHandler ([NullAllowed] GKPlayer match, [NullAllowed] NSError error);

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	delegate void GKGameActivityCheckPendingGameActivityExistenceHandler (bool pendingGameActivityExists);

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GKGameActivity {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("activityDefinition", ArgumentSemantic.Strong)]
		GKGameActivityDefinition ActivityDefinition { get; }

		[Export ("properties", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSString> Properties { get; set; }

		[Export ("state", ArgumentSemantic.Assign)]
		GKGameActivityState State { get; }

		[NullAllowed, Export ("partyCode")]
		string PartyCode { get; }

		[NullAllowed, Export ("partyURL", ArgumentSemantic.Strong)]
		NSUrl PartyUrl { get; }

		[Export ("creationDate", ArgumentSemantic.Strong)]
		NSDate CreationDate { get; }

		[NullAllowed, Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[NullAllowed, Export ("lastResumeDate", ArgumentSemantic.Strong)]
		NSDate LastResumeDate { get; }

		[NullAllowed, Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; }

		[Export ("duration")]
		double Duration { get; }

		[Export ("achievements", ArgumentSemantic.Copy)]
		NSSet<GKAchievement> Achievements { get; }

		[Export ("leaderboardScores", ArgumentSemantic.Copy)]
		NSSet<GKLeaderboardScore> LeaderboardScores { get; }

		[Static]
		[Export ("validPartyCodeAlphabet", ArgumentSemantic.Copy)]
		string [] ValidPartyCodeAlphabet { get; }

		[Static]
		[Export ("startWithDefinition:partyCode:error:")]
		[return: NullAllowed]
		GKGameActivity Create (GKGameActivityDefinition activityDefinition, string partyCode, [NullAllowed] out NSError error);

		[Static]
		[Export ("startWithDefinition:error:")]
		[return: NullAllowed]
		GKGameActivity Create (GKGameActivityDefinition activityDefinition, [NullAllowed] out NSError error);

		[Static]
		[Export ("isValidPartyCode:")]
		bool IsValidPartyCode (string partyCode);

		[Export ("initWithDefinition:")]
		NativeHandle Constructor (GKGameActivityDefinition activityDefinition);

		[Export ("start")]
		void Start ();

		[Export ("pause")]
		void Pause ();

		[Export ("resume")]
		void Resume ();

		[Export ("end")]
		void End ();

		[Export ("setScoreOnLeaderboard:toScore:context:")]
		void SetScore (GKLeaderboard leaderboard, nint score, nuint context);

		[Export ("setScoreOnLeaderboard:toScore:")]
		void SetScore (GKLeaderboard leaderboard, nint score);

		[Export ("getScoreOnLeaderboard:")]
		[return: NullAllowed]
		GKLeaderboardScore GetScore (GKLeaderboard leaderboard);

		[Export ("removeScoresFromLeaderboards:")]
		void RemoveScores (GKLeaderboard [] leaderboards);

		[Export ("setProgressOnAchievement:toPercentComplete:")]
		void SetProgressOnAchievement (GKAchievement achievement, double percentComplete);

		[Export ("setAchievementCompleted:")]
		void SetAchievementCompleted (GKAchievement achievement);

		[Export ("getProgressOnAchievement:")]
		double GetProgressOnAchievement (GKAchievement achievement);

		[Export ("removeAchievements:")]
		void RemoveAchievements (GKAchievement [] achievements);

		[return: NullAllowed]
		[Export ("makeMatchRequest")]
		GKMatchRequest MakeMatchRequest ();

		[Async]
		[Export ("findMatchWithCompletionHandler:")]
		void FindMatch (GKGameActivityFindMatchHandler completionHandler);

		[Async]
		[Export ("findPlayersForHostedMatchWithCompletionHandler:")]
		void FindPlayersForHostedMatch (GKGameActivityFindPlayersForHostedMatchHandler completionHandler);

		[Static]
		[Async]
		[Export ("checkPendingGameActivityExistenceWithCompletionHandler:")]
		void CheckPendingGameActivityExistence (GKGameActivityCheckPendingGameActivityExistenceHandler completionHandler);
	}

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum GKGameActivityPlayStyle : long {
		Unspecified = 0,
		Synchronous = 1,
		Asynchronous = 2,
	}

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum GKGameActivityState : ulong {
		Initialized = 0,
		Active = 1,
		Paused = 2,
		Ended = 4,
	}

	interface IGKGameActivityListener { }
	[TV (26, 0), MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface GKGameActivityListener {

		[Export ("player:wantsToPlayGameActivity:completionHandler:")]
		void WantsToPlayGameActivity (GKPlayer player, GKGameActivity activity, Action<bool> completionHandler);
	}
}
