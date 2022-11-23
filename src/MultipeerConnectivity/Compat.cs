//
// Compatibility Helpers
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc.
//

#nullable enable

using System;

namespace MultipeerConnectivity {

#if !XAMCORE_3_0
	public partial class MCPeerID {

		[Obsolete ("This constructor does not create a valid instance")]
		public MCPeerID ()
		{
		}
	}

	public partial class MCAdvertiserAssistant {

		[Obsolete ("This constructor does not create a valid instance")]
		public MCAdvertiserAssistant ()
		{
		}
	}
#endif
}
