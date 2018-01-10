#if !MONOMAC && !WATCH
using System;
using Foundation;
using ObjCRuntime;

namespace GameKit {
	public partial class GKLocalPlayerListener {

		// GKInviteEventListener and GKTurnBasedEventListener both export same selector
		// but generator changes now catch this. Stub it out to prevent API break
		[Unavailable (PlatformName.TvOS, PlatformArchitecture.All)]
		[Deprecated (PlatformName.iOS, 8,0, message: "Use 'DidRequestMatchWithOtherPlayers' instead.")]
		[iOS (7,0)]
		[Mac (10,10)]
		[Obsolete ("Use 'DidRequestMatch (GKPlayer player, GKPlayer[] recipientPlayers)' instead.")]
		public virtual void DidRequestMatchWithPlayers (GKPlayer player, string[] playerIDsToInvite)
		{
		}
	}
}
#endif
