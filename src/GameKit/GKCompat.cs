// Copyright 2016 Xamarin Inc. All rights reserved.

#if !XAMCORE_3_0

using System;
using ObjCRuntime;

namespace GameKit {

	public partial class GKMatchRequest {
		
		[iOS (8,0), Mac (10,10)]
		[Obsolete ("Use 'RecipientResponseHandler' property.")]
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
}

#endif
