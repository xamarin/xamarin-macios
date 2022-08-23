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

namespace DeviceDiscoveryExtension {
	public partial class DDDevice {

		NWEndpoint networkEndpoint = null;
		public NWEndpoint NetworkEndpoint { 
			get => networkEndpoint;
			set {
				networkEndpoint?.Dispose ();
				networkEndpoint = null;
				networkEndpoint = value;
				_NetworkEndpoint = networkEndpoint?.Handle ?? IntPtr.Zero;
			}
		}
	}
}
