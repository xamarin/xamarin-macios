//
// Unit tests for CBCentralManager
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Threading;
#if XAMCORE_2_0
using Foundation;
using CoreBluetooth;
using CoreFoundation;
#if !MONOMAC
using UIKit;
#endif
#else
using MonoTouch.CoreBluetooth;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreFoundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreBluetooth {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CBCentralManagerTest {

		class ManagerDelegate : CBCentralManagerDelegate {
			public AutoResetEvent Event { get; private set; }

			public ManagerDelegate (AutoResetEvent e) : base ()
			{
				Event = e;
			}

			#region implemented abstract members of MonoTouch.CoreBluetooth.CBCentralManagerDelegate
			public override void UpdatedState (CBCentralManager central)
			{
				Event.Set ();
			}

#if !XAMCORE_3_0
			public override void RetrievedPeripherals (CBCentralManager central, CBPeripheral[] peripherals)
			{
			}

			public override void RetrievedConnectedPeripherals (CBCentralManager central, CBPeripheral[] peripherals)
			{
			}
#endif // !XAMCORE_3_0

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

		[SetUp]
		public void SetUp ()
		{
			// Required API is available in macOS 10.8, but it doesn't work (hangs in 10.8-10.9, randomly crashes in 10.10) on the bots.
			TestRuntime.AssertMacSystemVersion (10, 11, throwIfOtherPlatform: false);
			var e = new AutoResetEvent (false);
			mgrDelegate = new ManagerDelegate (e);
			mgr = new CBCentralManager (mgrDelegate, new DispatchQueue ("com.xamarin.tests." + TestContext.CurrentContext.Test.Name));
		}

		[TearDown]
		public void TearDown ()
		{
			// should dispose the delegate
			mgr.Dispose ();
		}
			
		[Test, Timeout (5000)]
		public void Constructors ()
		{
			if (mgr.State == CBCentralManagerState.Unknown) {
				// ensure we do get called
				mgrDelegate.Event.WaitOne ();
			}

			// Manager creates it, we'll simply check it has a non-null delegate
			Assert.NotNull (mgr.Delegate, "Delegate");
		}

		[Test, Timeout (5000)]
		public void ScanForPeripherals ()
		{
			if (mgr.State == CBCentralManagerState.Unknown) {
				// ensure we do get called
				mgrDelegate.Event.WaitOne ();
			}
			if (mgr.State != CBCentralManagerState.PoweredOn)
				Assert.Inconclusive ("Bluetooth is off and therefore the test cannot be ran. State == {0}.", mgr.State);
			mgr.ScanForPeripherals ((CBUUID[])null, (NSDictionary)null);
		}

#if !XAMCORE_3_0
		[Test, Timeout (5000)]
		public void RetrievePeripherals ()
		{
			if (mgr.State == CBCentralManagerState.Unknown) {
				// ensure we do get called
				mgrDelegate.Event.WaitOne ();
			}
			if (mgr.State != CBCentralManagerState.PoweredOn)
				Assert.Inconclusive ("Bluetooth is off and therefore the test cannot be ran. State == {0}.", mgr.State);

			if (TestRuntime.CheckXcodeVersion (7, 0)) {
				using (var uuid = new NSUuid ("B9401000-F5F8-466E-AFF9-25556B57FE6D"))
					mgr.RetrievePeripheralsWithIdentifiers (uuid);
			} else {
				// that API was deprecated in 7.0 and removed from 9.0
				using (var uuid = CBUUID.FromString ("B9401000-F5F8-466E-AFF9-25556B57FE6D"))
					mgr.RetrievePeripherals (uuid);
			}
		}
#endif // !XAMCORE_3_0
	}
}

#endif // !__WATCHOS__