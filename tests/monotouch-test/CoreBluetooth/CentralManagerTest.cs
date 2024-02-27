//
// Unit tests for CBCentralManager
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !__MACCATALYST__

using System;
using System.Threading;
using Foundation;
using CoreBluetooth;
using CoreFoundation;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreBluetooth {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CBCentralManagerTest {

		class ManagerDelegate : CBCentralManagerDelegate {
			public AutoResetEvent PoweredOnEvent { get; private set; } = new AutoResetEvent (false);

			#region implemented abstract members of MonoTouch.CoreBluetooth.CBCentralManagerDelegate
			public override void UpdatedState (CBCentralManager central)
			{
#if NET
				if (central.State == CBManagerState.PoweredOn)
#else
				if (central.State == CBCentralManagerState.PoweredOn)
#endif
					PoweredOnEvent.Set ();
			}

#if !XAMCORE_3_0 && !NET
			public override void RetrievedPeripherals (CBCentralManager central, CBPeripheral [] peripherals)
			{
			}

			public override void RetrievedConnectedPeripherals (CBCentralManager central, CBPeripheral [] peripherals)
			{
			}
#endif // !XAMCORE_3_0 && !NET

			public override void DiscoveredPeripheral (CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
			{
			}

			public override void ConnectedPeripheral (CBCentralManager central, CBPeripheral peripheral)
			{
			}

			public override void FailedToConnectPeripheral (CBCentralManager central, CBPeripheral peripheral, NSError error)
			{
			}

			public override void DisconnectedPeripheral (CBCentralManager central, CBPeripheral peripheral, NSError error)
			{
			}
			#endregion
		}

		CBCentralManager mgr;
		ManagerDelegate mgrDelegate;
		CBUUID heartRateMonitorUUID;

		[SetUp]
		public void SetUp ()
		{
			if (TestRuntime.IsInCI && TestRuntime.CheckXcodeVersion (14, 0))
				TestRuntime.AssertNotDesktop (); // Looks like this particular test doesn't like Desktop + M1 bot machines
												 // iOS 13 and friends require bluetooth permission
			if (TestRuntime.CheckXcodeVersion (11, 0))
				TestRuntime.CheckBluetoothPermission (true);
			//known UUID for a heart monitor, more common, we want to find something and make sure we do not crash
			heartRateMonitorUUID = CBUUID.FromPartial (0x180D);
			// Required API is available in macOS 10.8, but it doesn't work (hangs in 10.8-10.9, randomly crashes in 10.10) on the bots.
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);
			mgrDelegate = new ManagerDelegate ();
			mgr = new CBCentralManager (mgrDelegate, new DispatchQueue ("com.xamarin.tests." + TestContext.CurrentContext.Test.Name));
			if (!mgrDelegate.PoweredOnEvent.WaitOne (TimeSpan.FromSeconds (5)))
				Assert.Inconclusive ("Bluetooth never turned on.");
		}

		[TearDown]
		public void TearDown ()
		{
			heartRateMonitorUUID?.Dispose ();
			mgrDelegate?.Dispose ();  // make sure that our delegate does not get messages after the mgr was disposed
			mgr?.Dispose ();
		}

		[Test]
		public void Constructors ()
		{
			// Manager creates it, we'll simply check it has a non-null delegate
			Assert.NotNull (mgr.Delegate, "Delegate");
		}

		[Test]
		public void ScanForPeripherals ()
		{
			mgr.ScanForPeripherals ((CBUUID []) null, (NSDictionary) null);
		}

		[Test]
		public void RetrievePeripherals ()
		{
			if (TestRuntime.CheckXcodeVersion (7, 0)) {
				// ToString in a CBUUID with true returns the full uuid which can be used to create a NSUuid
				using (var uuid = new NSUuid (heartRateMonitorUUID.ToString (true)))
					mgr.RetrievePeripheralsWithIdentifiers (uuid);
			} else {
#if !XAMCORE_3_0 && !NET
				// that API was deprecated in 7.0 and removed from 9.0
				mgr.RetrievePeripherals (heartRateMonitorUUID);
#endif // !XAMCORE_3_0 && !NET
			}
		}
	}
}

#endif // !__WATCHOS__ && !__MACCATALYST__
