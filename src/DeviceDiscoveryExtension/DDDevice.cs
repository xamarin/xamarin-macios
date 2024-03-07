//
// Custom methods for DDDevice
//
// Authors:
//   Israel Soto (issoto@microsoft.com)
//
// Copyright 2022 Microsoft Corporation.
//

#nullable enable

using ObjCRuntime;
using Foundation;
using Network;
using System;

using nw_endpoint_t = System.IntPtr;

namespace DeviceDiscoveryExtension {
	public partial class DDDevice {
		public NWEndpoint? NetworkEndpoint {
			get => _NetworkEndpoint != nw_endpoint_t.Zero ? new NWEndpoint (_NetworkEndpoint, false) : null;
			set => _NetworkEndpoint = value.GetHandle ();
		}
	}
}
