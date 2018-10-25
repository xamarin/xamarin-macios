//
// GameKit.cs: This file describes the API that the generator will produce for GameKit
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2014 Xamarin Inc. All rights reserved
//

using System;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace GameKit {
#if !MONOMAC && !TVOS && !WATCH
	public class GKDataReceivedEventArgs : EventArgs {
		public GKDataReceivedEventArgs (NSData data, string peer, GKSession session)
		{
			Data = data;
			PeerID = peer;
			Session = session;
		}
		public NSData Data { get; private set; }
		public string PeerID { get; private set; }
		public GKSession Session { get; private set; }
	}

#if !XAMCORE_2_0
	public partial class GKVoiceChatService {
		public bool StartVoiceChat (string participantID, out NSError error)
		{
			unsafe {
				IntPtr errhandle;
				IntPtr ptrtohandle = (IntPtr) (&errhandle);

				var ret = StartVoiceChat (participantID, ptrtohandle);
				if (errhandle != IntPtr.Zero)
					error = (NSError) Runtime.GetNSObject (errhandle);
				else
					error = null;
				return ret;
			}
		}

		public bool AcceptCall (int callId, out NSError error)
		{
			unsafe {
				IntPtr errhandle;
				IntPtr ptrtohandle = (IntPtr) (&errhandle);

				var ret = AcceptCall (callId, ptrtohandle);
				if (errhandle != IntPtr.Zero)
					error = (NSError) Runtime.GetNSObject (errhandle);
				else
					error = null;
				return ret;
			}
		}
	}
#endif

#if !TVOS && !WATCH
	public partial class GKSession {
#if !XAMCORE_2_0
		public bool SendData (NSData data, string [] peers, GKSendDataMode mode, out NSError error)
		{
			unsafe {
				IntPtr errhandle;
				IntPtr ptrtohandle = (IntPtr) (&errhandle);

				var ret = SendData (data, peers, mode, ptrtohandle);
				if (errhandle != IntPtr.Zero)
					error = (NSError) Runtime.GetNSObject (errhandle);
				else
					error = null;
				return ret;
			}
		}

		public bool SendDataToAllPeers (NSData data, GKSendDataMode mode, out NSError error)
		{
			unsafe {
				IntPtr errhandle;
				IntPtr ptrtohandle = (IntPtr) (&errhandle);

				var ret = SendDataToAllPeers (data, mode, ptrtohandle);
				if (errhandle != IntPtr.Zero)
					error = (NSError) Runtime.GetNSObject (errhandle);
				else
					error = null;
				return ret;
			}
		}
#endif
		[Register ("MonoTouch_GKSession_ReceivedObject")]
		internal class ReceiverObject : NSObject {
			internal EventHandler<GKDataReceivedEventArgs> receiver;

			public ReceiverObject ()
			{
				IsDirectBinding = false;
			}

			[Export ("receiveData:fromPeer:inSession:context:")]
			[Preserve (Conditional = true)]
			void Receive (NSData data, string peer, GKSession session, IntPtr context)
			{
				if (receiver != null)
					receiver (session, new GKDataReceivedEventArgs (data, peer, session));
			}
				
		}

		//
		// This delegate is used by the 
		ReceiverObject receiver;
		public event EventHandler<GKDataReceivedEventArgs> ReceiveData {
			add {
				if (receiver == null){
					receiver = new ReceiverObject ();
					_SetDataReceiveHandler (receiver, IntPtr.Zero);
					MarkDirty ();
				}
				receiver.receiver += value;
			}
			
			remove {
				if (receiver == null)
					return;
				receiver.receiver -= value;
			}
		}

		public void SetDataReceiveHandler (NSObject obj, IntPtr context)
		{
			receiver = null;
			_SetDataReceiveHandler (obj, context);
		}

		//
		// The C# event handlers
		//
		Mono_GKSessionDelegate EnsureDelegate ()
		{
                      NSObject del = WeakDelegate;
                        if (del == null || (!(del is Mono_GKSessionDelegate))){
                                del = new Mono_GKSessionDelegate ();
                                WeakDelegate = del;
                        }
                        return (Mono_GKSessionDelegate) del;
		}

		public event EventHandler<GKPeerChangedStateEventArgs> PeerChanged {
			add {
				EnsureDelegate ().cbPeerChanged += value;
			}

			remove {
				EnsureDelegate ().cbPeerChanged -= value;
			}
		}
		
		public event EventHandler<GKPeerConnectionEventArgs> ConnectionRequest {
			add {
				EnsureDelegate ().cbConnectionRequest += value;
			}

			remove {
				EnsureDelegate ().cbConnectionRequest -= value;
			}
		}

		public event EventHandler<GKPeerConnectionEventArgs> ConnectionFailed {
			add {
				EnsureDelegate ().cbConnectionFailed += value;
			}

			remove {
				EnsureDelegate ().cbConnectionFailed -= value;
			}
		}
		public event EventHandler<GKPeerConnectionEventArgs> Failed {
			add {
				EnsureDelegate ().cbFailedWithError += value;
			}

			remove {
				EnsureDelegate ().cbFailedWithError -= value;
			}
		}
	}

	class Mono_GKSessionDelegate : GKSessionDelegate {
		internal EventHandler<GKPeerChangedStateEventArgs> cbPeerChanged;
		internal EventHandler<GKPeerConnectionEventArgs> cbConnectionRequest, cbConnectionFailed, cbFailedWithError;

		public Mono_GKSessionDelegate ()
		{
			IsDirectBinding = false;
		}

		[Preserve (Conditional = true)]
		public override void PeerChangedState (GKSession session, string peerID, GKPeerConnectionState state)
		{
			if (cbPeerChanged != null)
				cbPeerChanged (session, new GKPeerChangedStateEventArgs (session, peerID, state));
		}
		
		[Preserve (Conditional = true)]
		public override void PeerConnectionRequest (GKSession session, string peerID)
		{
			if (cbConnectionRequest != null)
				cbConnectionRequest (session, new GKPeerConnectionEventArgs (session, peerID, null));
		}
			
		[Preserve (Conditional = true)]
		public override void PeerConnectionFailed (GKSession session, string peerID, NSError error)
		{
			if (cbConnectionFailed != null)
				cbConnectionFailed (session, new GKPeerConnectionEventArgs (session, peerID, error));

		}
			
		[Preserve (Conditional = true)]
		public override void FailedWithError (GKSession session, NSError error)
		{
			if (cbFailedWithError != null)
				cbFailedWithError (session, new GKPeerConnectionEventArgs (session, null, error));
		}
	}
#endif // !TVOS

	public class GKPeerChangedStateEventArgs : EventArgs {
		public GKPeerChangedStateEventArgs (GKSession session, string peerID, GKPeerConnectionState state)
		{
			Session = session;
			PeerID = peerID;
			State = state;
		}

		public GKSession Session { get; private set; }
		public string PeerID { get; private set; }
		public GKPeerConnectionState State { get; private set; }
	}

	public class GKPeerConnectionEventArgs : EventArgs {
		public GKPeerConnectionEventArgs (GKSession session, string peerID, NSError error)
		{
			Session = session;
			PeerID = peerID;
			Error = error;
		}
		public GKSession Session { get; private set; }
		public string PeerID { get; private set; }
		public NSError Error { get; private set; }
	}
#endif
	
#if !XAMCORE_2_0
	public partial class GKPlayer {
		// mistake, non-static
		public NSString DidChangeNotificationName {
			get {
				return DidChangeNotificationNameNotification;
			}
		}
	}
#endif

	public partial class GKVoiceChat {

#if !XAMCORE_2_0
		[Obsolete ("Use 'PlayerStateUpdateHandler' property.")]
		public virtual void SetPlayerStateUpdateHandler (GKPlayerStateUpdateHandler handler)
		{
			PlayerStateUpdateHandler = handler;
		}
#endif

#if !XAMCORE_3_0
		[Obsolete ("Use 'SetMute (bool, string)' method.")]
		public virtual void SetMute (bool isMuted, GKPlayer player)
		{
			SetMute (isMuted, player == null ? null : player.PlayerID);
		}
#endif
	}

	public partial class GKTurnBasedExchange {

		public override string ToString ()
		{
			return "GKTurnBasedExchange";
		}
	}

	public partial class GKTurnBasedExchangeReply {

		public override string ToString ()
		{
			return "GKTurnBasedExchangeReply";
		}
	}

#if !XAMCORE_2_0
	public partial class GKLeaderboardViewController {

		[Obsolete ("Apple never shipped the `initWithTimeScope:playerScope:` selector. Use the default constructor.")]
		public GKLeaderboardViewController (GKLeaderboardTimeScope timeScope, GKLeaderboardPlayerScope playerScope) : 
			base (NSObjectFlag.Empty)
		{
			// we can't set `playerScope` with the existing API and
			// setting `timeScope` does not work at this stage either so...
			throw new NotSupportedException ();
		}
	}
#endif

	public partial class GKChallenge {

		public override string ToString ()
		{
			return GetType ().ToString ();
		}
	}

	public partial class GKMatch {
#if !XAMCORE_3_0
		// Compatbility with the broken API, it is deprecated, so that is good.
		public virtual bool SendData (NSData data, string [] players, GKMatchSendDataMode mode, NSError error)
		{
			return SendData (data, players, mode, out error);
		}
#endif

#if !XAMCORE_2_0
		[Obsolete ("Use 'SendDataToAllPlayers'.")]
		public virtual bool SendDataToAllPlayer (NSData data, GKMatchSendDataMode mode, out NSError error)
		{
			return SendDataToAllPlayers (data, mode, out error);
		}

#if MONOMAC
		// never been released with XI
		[Obsolete ("Use 'SendDataToAllPlayers' that takes out NSError.")]
		public bool SendDataToAllPlayers (NSData data, GKMatchSendDataMode mode, IntPtr ptrToNSErrorHandle)
		{
			NSError error = new NSError (ptrToNSErrorHandle);
			return SendDataToAllPlayers (data, mode, out error);
		}
#endif
#endif

	}

#if !XAMCORE_2_0
	public partial class GKScore {
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'LeaderboardIdentifier' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'LeaderboardIdentifier' instead.")]
		public string Category {
			get { return category; }
			set { category = value; }
		}
	}
#endif
}
