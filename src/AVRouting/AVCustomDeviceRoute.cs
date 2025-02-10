#if IOS
//
// AVCustomDeviceRoute.cs: AVCustomDeviceRoute Complementing methods
//
// Author:
//   Manuel de la Pena <mandel@microsoft.com>
//
using System;
using Network;

#nullable enable

namespace AVRouting {
	partial class AVCustomDeviceRoute {

		 public NWEndpoint NetworkEndpoint => new NWEndpoint (_NetworkEndpoint, owns: false);

	}
}
#endif

