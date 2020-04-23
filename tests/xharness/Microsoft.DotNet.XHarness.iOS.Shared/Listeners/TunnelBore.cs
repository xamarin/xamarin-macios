using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Listeners {

	// manages the tunnels that are used to communicate with the devices. We want to create a single tunnel per device
	// since we can only run one app per device, this should not be a problem.
	//
	// definition: 
	// A tunnel boring machine, also known as a "mole", is a machine used to excavate tunnels with a circular cross section
	// through a variety of soil and rock strata. They may also be used for microtunneling. 
	public interface ITunnelBore : IDisposable {

		// create a new tunnel for the device with the given name.
		ITcpTunnel Create (string device, ILog mainLog);

		// close a given tunnel
		Task Close (string device);
	}

	public class TunnelBore : ITunnelBore {

		readonly object tunnelsLock = new object ();
		readonly IProcessManager processManager;
		readonly ConcurrentDictionary<string, TcpTunnel> tunnels = new ConcurrentDictionary<string, TcpTunnel> ();

		public TunnelBore (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		// Creates a new tcp tunnel to the given device that will use the port from the passed listener. 
		public ITcpTunnel Create (string device, ILog mainLog)
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

		public async Task Close (string device)
		{
			// closes a tcp tunnel that was created for the given device.
			if (tunnels.TryRemove (device, out var tunnel)) {
				await tunnel.Close ();
				tunnel.Dispose ();
			}
		}

		public void Dispose ()
		{
			var devices = tunnels.Keys.ToArray ();
			foreach (var d in devices) { 
				if (tunnels.TryRemove(d, out var tunnel)) {
					tunnel.Dispose ();
				}
			}
		}
	}
}
