// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if MONOMAC
using System.Threading;

using Network;

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWEthernetChannelTest {
		[TestCase (false)]
		[TestCase (true)]
		public void StateChangesHandler (bool useLegacyHandler)
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			TestRuntime.IgnoreInCI ("This test requires a wired interface and the custom-protocol entitlement.");

			using var monitor = new NWPathMonitor ();
			using var pathUpdated = new AutoResetEvent (false);
			NWPath? path = null;
			monitor.SetQueue (DispatchQueue.DefaultGlobalQueue);
			monitor.SnapshotHandler = currentPath => {
				path = currentPath;
				pathUpdated.Set ();
			};
			monitor.Start ();
			Assert.That (pathUpdated.WaitOne (30000), Is.True, "Path update");
			monitor.Cancel ();
			Assert.That (path, Is.Not.Null, "Current path");

			NWInterface? ethernetInterface = null;
			path.EnumerateInterfaces (networkInterface => {
				if (networkInterface.InterfaceType == NWInterfaceType.Wired)
					ethernetInterface = networkInterface;
				return true;
			});
			if (ethernetInterface is null)
				Assert.Ignore ("This test requires a wired interface.");

			using var channel = new NWEthernetChannel (0x88b5, ethernetInterface);
			using var stateChanged = new AutoResetEvent (false);
			NWEthernetChannelState state = NWEthernetChannelState.Invalid;
			channel.SetQueue (DispatchQueue.DefaultGlobalQueue);
			if (useLegacyHandler) {
				Action<NWBrowserState, NWError?> legacyHandler = (currentState, error) => {
					state = (NWEthernetChannelState) currentState;
					stateChanged.Set ();
				};
				channel.SetStateChangesHandler (legacyHandler);
			} else {
				channel.SetStateChangesHandler ((currentState, error) => {
					state = currentState;
					stateChanged.Set ();
				});
			}
			channel.Start ();
			try {
				Assert.That (stateChanged.WaitOne (30000), Is.True, "State callback");
				Assert.That (state, Is.AnyOf (
					NWEthernetChannelState.Waiting, NWEthernetChannelState.Preparing, NWEthernetChannelState.Ready,
					NWEthernetChannelState.Failed, NWEthernetChannelState.Cancelled), "Ethernet channel state");
			} finally {
				channel.Cancel ();
			}
		}
	}
}
#endif
