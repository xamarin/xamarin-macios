// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if MONOMAC
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Network;

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWEthernetChannelTest {
		[DllImport (Constants.SecurityLibrary)]
		static extern IntPtr SecTaskCreateFromSelf (IntPtr allocator);

		[DllImport (Constants.SecurityLibrary)]
		static extern IntPtr SecTaskCopyValueForEntitlement (IntPtr task, IntPtr entitlement, out IntPtr error);

		static void AssertCustomProtocolEntitlement ()
		{
			const string entitlement = "com.apple.developer.networking.custom-protocol";
			var task = SecTaskCreateFromSelf (IntPtr.Zero);
			Assert.That (task, Is.Not.EqualTo (IntPtr.Zero), "SecTaskCreateFromSelf");
			try {
				using var name = new NSString (entitlement);
				var valueHandle = SecTaskCopyValueForEntitlement (task, name.Handle, out var errorHandle);
				using var value = Runtime.GetNSObject<NSNumber> (valueHandle, owns: true);
				using var error = Runtime.GetNSObject<NSError> (errorHandle, owns: true);
				Assert.That (error, Is.Null, "Reading the custom-protocol entitlement");
				if (value is null || !value.BoolValue)
					Assert.Ignore ($"This test requires the '{entitlement}' entitlement, authorized by a provisioning profile.");
			} finally {
				TestRuntime.CFRelease (task);
			}
		}

		[TestCase (false)]
		[TestCase (true)]
		public void StateChangesHandler (bool useLegacyHandler)
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			AssertCustomProtocolEntitlement ();

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
