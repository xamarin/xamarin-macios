// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if MONOMAC
using System.Threading.Tasks;

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
			TestRuntime.AssertBooleanEntitlement ("com.apple.developer.networking.custom-protocol", true);

			using var monitor = new NWPathMonitor ();
			var pathUpdated = new TaskCompletionSource<NWPath> ();
			monitor.SetQueue (DispatchQueue.DefaultGlobalQueue);
			monitor.SnapshotHandler = currentPath => {
				pathUpdated.TrySetResult (currentPath);
			};
			monitor.Start ();
			try {
				Assert.That (pathUpdated.Task.Wait (30000), Is.True, "Path update");
			} finally {
				monitor.Cancel ();
			}
			var path = pathUpdated.Task.Result;
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
			var stateChanged = new TaskCompletionSource<NWEthernetChannelState> ();
			channel.SetQueue (DispatchQueue.DefaultGlobalQueue);
			if (useLegacyHandler) {
				Action<NWBrowserState, NWError?> legacyHandler = (currentState, error) => {
					stateChanged.TrySetResult ((NWEthernetChannelState) currentState);
				};
				channel.SetStateChangesHandler (legacyHandler);
			} else {
				channel.SetStateChangesHandler ((currentState, error) => {
					stateChanged.TrySetResult (currentState);
				});
			}
			channel.Start ();
			try {
				Assert.That (stateChanged.Task.Wait (30000), Is.True, "State callback");
				Assert.That (stateChanged.Task.Result, Is.AnyOf (
					NWEthernetChannelState.Waiting, NWEthernetChannelState.Preparing, NWEthernetChannelState.Ready,
					NWEthernetChannelState.Failed, NWEthernetChannelState.Cancelled), "Ethernet channel state");
			} finally {
				channel.Cancel ();
			}
		}
	}
}
#endif
