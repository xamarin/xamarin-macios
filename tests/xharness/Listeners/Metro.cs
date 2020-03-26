using System;
using System.Collections.Generic;
using Xharness.Execution;
using Xharness.Logging;

namespace Xharness.Listeners {

	// manages the tunnels that are used to communicate with the devices. We want to create a single tunnel per device
	// since we can only run one app per device, this should not be a problem.
	//
	// definition: 
	// Metro: a subway system in a city, especially Paris.
	public interface IMetro : IDisposable {

		// create a new tunnel for the device with the given name.
		TcpTunnel Create (string device, ILog mainLog);

		// close a given tunnel
		void Close (string device);

		// return if a device has a tunnel, if it does, set the out param
		bool HasTunnel (string device, out TcpTunnel tunnel);
	}

	public class Metro : IMetro {

		readonly object tunnelsLock = new object ();
		readonly IHarness harness;
		readonly IProcessManager processManager;
		readonly Dictionary<string, TcpTunnel> tunnels = new Dictionary<string, TcpTunnel> ();

		public Metro (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		// Creates a new tcp tunnel to the given device that will use the port from the passed listener. 
		public TcpTunnel Create (string device, ILog mainLog)
		{
			lock (tunnelsLock) {
				if (tunnels.ContainsKey (device)) {
					string msg = $"Cannot create more than one tunnel to device {device}";
					mainLog.WriteLine (msg);
					throw new InvalidOperationException (msg);
				}
				tunnels [device] = new TcpTunnel (processManager);
				return tunnels [device];
			}
		}

		public void Close (string device)
		{ 
			// closes a tcp tunnel that was created for the given device.
			lock (tunnelsLock) { 
				if (tunnels.TryGetValue (device, out var tunnel)) {
					tunnel.Close ();
					tunnels.Remove (device);
				}
			}
		}

		// test if the device has a tunnel, if it does, set the simplet tcp listener to be a listener that is 
		// using the tunnel and ready to listen to test result
		public bool HasTunnel (string device, out TcpTunnel tunnel)
		{
			tunnel = null;
			lock (tunnelsLock) {
				if (tunnels.ContainsKey (device)) {
					// create a tcp listener for a tunnel with the port
					tunnel = tunnels [device];
					return true;
				}
				return false;
			}
		}

		public void Dispose ()
		{
			lock (tunnelsLock) {
				foreach (var t in tunnels.Values)
					t.Dispose ();
			}
		}
	}
}
