using System;
using System.Collections.Generic;
using Xharness.Execution;
using Xharness.Logging;

namespace Xharness.Listeners {

	// manages the tunnels that are used to communicate with the devices. We want to create a single tunnel per device
	// since we can only run one app per device, this should not be a problem.
	//
	// definition: 
	// A tunnel boring machine, also known as a "mole", is a machine used to excavate tunnels with a circular cross section
	// through a variety of soil and rock strata. They may also be used for microtunneling. 
	public interface ITunnelBore : IDisposable {

		// create a new tunnel for the device with the given name.
		TcpTunnel Create (string device, ILog mainLog);

		// close a given tunnel
		void Close (string device);
	}

	public class TunnelBore : ITunnelBore {

		readonly object tunnelsLock = new object ();
		readonly IProcessManager processManager;
		readonly Dictionary<string, TcpTunnel> tunnels = new Dictionary<string, TcpTunnel> ();

		public TunnelBore (IProcessManager processManager)
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
					tunnel.Dispose ();
					tunnels.Remove (device);
				}
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
