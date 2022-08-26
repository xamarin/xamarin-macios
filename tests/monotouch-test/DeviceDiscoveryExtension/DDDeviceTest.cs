//
// Unit tests for DDDevice
//
// Authors:
//	Israel Soto <issoto@microsoft.com>
//
// Copyright 2022 Microsoft Corporation.
//

#nullable enable

#if __IOS__

using System;
using DeviceDiscoveryExtension;
using Foundation;
using Network;
using ObjCRuntime;
using UniformTypeIdentifiers;
using NUnit.Framework;

namespace MonoTouchFixtures.DeviceDiscoveryExtension {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DDDeviceTest {
		
		[Test]
		public void NetworkEndpointTest ()
		{
			TestRuntime.AssertXcodeVersion (14,0);

			var uuid = Guid.NewGuid ();
			var endpoint = NWEndpoint.Create ("www.microsoft.com", "https");
			var device = new DDDevice ("MyDevice", DDDeviceCategory.LaptopComputer, UTType.CreateFromIdentifier ("com.adobe.pdf"), uuid.ToString ());
			
			device.NetworkEndpoint = endpoint;
			var tmpEndpoint = device.NetworkEndpoint;

			Assert.True (endpoint.GetHandle () == tmpEndpoint.GetHandle (), "NetworkEndpoint");
		}
	}
}

#endif // __IOS__
