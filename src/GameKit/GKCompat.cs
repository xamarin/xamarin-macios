// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using System.ComponentModel;
using Foundation;
using ObjCRuntime;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace GameKit {

#if !XAMCORE_3_0
	public partial class GKMatchRequest {

#if !NET
		[Obsolete ("Use 'RecipientResponseHandler' property.")]
#else
		[Obsolete ("Use 'RecipientResponseHandler' property.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public virtual void SetRecipientResponseHandler (Action<GKPlayer, GKInviteRecipientResponse> handler)
		{
			RecipientResponseHandler = handler;
		}
	}

	public partial class GKMatchmaker {

		[Obsolete ("Use 'InviteHandler' property.")]
		public virtual void SetInviteHandler (GKInviteHandler handler)
		{
			InviteHandler = handler;
		}
	}
#endif // !XAMCORE_3_0

#if WATCH && !NET
	[Unavailable (PlatformName.WatchOS)]
	[Obsolete ("This API is not available on this platform.")]
	public static class GKGameSessionErrorCodeExtensions {
		[Obsolete ("Always returns null.")]
		public static NSString? GetDomain (this GKGameSessionErrorCode self) => null;
	}
#endif

#if !NET && !WATCH
	public partial class GKGameSession {

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidAddPlayer (GKGameSession session, GKCloudPlayer player) { }

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidChangeConnectionState (GKGameSession session, GKCloudPlayer player, GKConnectionState newState) { }

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidReceiveData (GKGameSession session, Foundation.NSData data, GKCloudPlayer player) { }

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidReceiveMessage (GKGameSession session, string message, Foundation.NSData data, GKCloudPlayer player) { }

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidRemovePlayer (GKGameSession session, GKCloudPlayer player) { }

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidSaveData (GKGameSession session, GKCloudPlayer player, Foundation.NSData data) { }
	}
#endif

#if !XAMCORE_5_0
#if __IOS__ || __MACCATALYST__
	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("Use 'MCBrowserViewController' from the 'MultipeerConnectivity' framework instead.")]
#if NET
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Unavailable (PlatformName.WatchOS)]
#endif
	public interface IGKPeerPickerControllerDelegate : INativeObject, IDisposable
	{
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("Use 'MCBrowserViewController' from the 'MultipeerConnectivity' framework instead.")]
#if NET
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Unavailable (PlatformName.WatchOS)]
#endif
	public static class GKPeerPickerControllerDelegate_Extensions {
		public static void ConnectionTypeSelected (this IGKPeerPickerControllerDelegate This, GKPeerPickerController picker, GKPeerPickerConnectionType type)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		public static GKSession GetSession (this IGKPeerPickerControllerDelegate This, GKPeerPickerController picker, GKPeerPickerConnectionType forType)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		public static void PeerConnected (this IGKPeerPickerControllerDelegate This, GKPeerPickerController picker, string peerId, GKSession toSession)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		public static void ControllerCancelled (this IGKPeerPickerControllerDelegate This, GKPeerPickerController picker)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("Use 'MCBrowserViewController' from the 'MultipeerConnectivity' framework instead.")]
#if NET
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Unavailable (PlatformName.WatchOS)]
#endif
	public unsafe class GKPeerPickerControllerDelegate : NSObject, IGKPeerPickerControllerDelegate {
		public GKPeerPickerControllerDelegate () : base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		protected GKPeerPickerControllerDelegate (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		protected internal GKPeerPickerControllerDelegate (NativeHandle handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		public virtual void ConnectionTypeSelected (GKPeerPickerController picker, GKPeerPickerConnectionType type)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		public virtual void ControllerCancelled (GKPeerPickerController picker)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		public virtual GKSession GetSession (GKPeerPickerController picker, GKPeerPickerConnectionType forType)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		public virtual void PeerConnected (GKPeerPickerController picker, string peerId, GKSession toSession)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}
	} /* class GKPeerPickerControllerDelegate */

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("Use 'MCBrowserViewController' from the 'MultipeerConnectivity' framework instead.")]
#if NET
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Unavailable (PlatformName.WatchOS)]
#endif
	public class GKPeerPickerController : NSObject {
		public override NativeHandle ClassHandle { get { throw new PlatformNotSupportedException (Constants.TypeUnavailable); } }

		public GKPeerPickerController () : base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		protected GKPeerPickerController (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		protected internal GKPeerPickerController (NativeHandle handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		public virtual void Dismiss ()
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		public virtual void Show ()
		{
			throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}

		public virtual GKPeerPickerConnectionType ConnectionTypesMask {
			get {
				throw new PlatformNotSupportedException (Constants.TypeUnavailable);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeUnavailable);
			}
		}

		public IGKPeerPickerControllerDelegate Delegate {
			get {
				throw new PlatformNotSupportedException (Constants.TypeUnavailable);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeUnavailable);
			}
		}

		public virtual bool Visible {
			get {
				throw new PlatformNotSupportedException (Constants.TypeUnavailable);
			}
		}

		public virtual NSObject? WeakDelegate {
			get {
				throw new PlatformNotSupportedException (Constants.TypeUnavailable);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeUnavailable);
			}
		}

		protected override void Dispose (bool disposing)
		{
				throw new PlatformNotSupportedException (Constants.TypeUnavailable);
		}
	} /* class GKPeerPickerController */
#endif //  __IOS__ || __MACCATALYST__
#endif // !XAMCORE_5_0
}
