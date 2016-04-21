#if !MONOMAC && XAMCORE_2_0
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.GameKit {
	public partial class GKLocalPlayerListener {

		// GKInviteEventListener and GKTurnBasedEventListener both export same selector
		// but generator changes now catch this. Stub it out to prevent API break
		[Unavailable (PlatformName.TvOS, PlatformArchitecture.All)]
		[Deprecated (PlatformName.iOS, 8,0, message: "Use DidRequestMatchWithOtherPlayers instead")]
		[Introduced (PlatformName.iOS, 7,0)]
		[Introduced (PlatformName.MacOSX, 10,10)]
		[Obsolete ("Use DidRequestMatch(GKPlayer player, GKPlayer[] recipientPlayers) instead")]
		public virtual void DidRequestMatchWithPlayers (GKPlayer player, string[] playerIDsToInvite)
		{
		}
	}
}
#endif
