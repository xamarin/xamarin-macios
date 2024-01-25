#if !MONOMAC && !WATCH && !NET
using System;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace GameKit {
	public partial class GKLocalPlayerListener {

		// GKInviteEventListener and GKTurnBasedEventListener both export same selector
		// but generator changes now catch this. Stub it out to prevent API break
		[Obsolete ("Use 'DidRequestMatch (GKPlayer player, GKPlayer[] recipientPlayers)' instead.")]
		public virtual void DidRequestMatchWithPlayers (GKPlayer player, string [] playerIDsToInvite)
		{
		}
	}
}
#endif
