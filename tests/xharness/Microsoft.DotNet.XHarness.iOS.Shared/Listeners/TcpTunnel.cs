using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Listeners {

	// interface to be implemented by those listener that can use a tcp tunnel
	public interface ITunnelListener : ISimpleListener { 
		TaskCompletionSource<bool> TunnelHoleThrough { get; }
	}

	// interface implemented by a tcp tunnel between the host and the device.
	public interface ITcpTunnel : IDisposable {
		public void Open (string device, ITunnelListener simpleListener, TimeSpan timeout, ILog mainLog);
		public Task Close ();
		public Task<bool> Started { get; }
	}

	// represents a tunnel created between a device and a host. This tunnel allows the communication between
	// the host and the device via the usb cable.
	public class TcpTunnel : ITcpTunnel {
		readonly object processExecutionLock = new object ();
		readonly IProcessManager processManager;

		bool disposed = false;
		Task<ProcessExecutionResult> tcpTunnelExecutionTask = null; 
		CancellationTokenSource cancellationToken;

		public TaskCompletionSource<bool> startedCompletionSource { get; private set; } = new TaskCompletionSource<bool> ();
		public Task<bool> Started => startedCompletionSource.Task;
		public int Port { get; private set; }

		public TcpTunnel (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public void Open (string device, ITunnelListener simpleListener, TimeSpan timeout, ILog mainLog)
		{
			if (device == null)
				throw new ArgumentNullException (nameof (device));
			if (simpleListener == null)
				throw new ArgumentNullException (nameof (simpleListener));
			if (mainLog == null)
				throw new ArgumentNullException (nameof (mainLog));

			lock (processExecutionLock) {
				// launch app, but do not await for the result, since we need to create the tunnel
				var tcpArgs = new MlaunchArguments {
					new TcpTunnelArgument (simpleListener.Port),
					new VerbosityArgument (),
					new DeviceNameArgument (device),
				};

				// use a cancelation token, later will be used to kill the tcp tunnel proces
				cancellationToken = new CancellationTokenSource ();
				mainLog.WriteLine ($"Starting tcp tunnel between mac port: {simpleListener.Port} and devie port {simpleListener.Port}.");
				Port = simpleListener.Port;
				var tunnelbackLog = new CallbackLog ((line) => {
					mainLog.WriteLine ($"The tcp tunnel output is {line}");
					if (line.Contains ("Tcp tunnel started on device")) {
						mainLog.Write ($"Tcp tunnel created on port {simpleListener.Port}");
						startedCompletionSource.TrySetResult (true);
						simpleListener.TunnelHoleThrough.TrySetResult (true);
					}
				});
				// do not await since we are going to be running the process is parallel
				tcpTunnelExecutionTask = processManager.ExecuteCommandAsync (tcpArgs, tunnelbackLog, timeout, cancellation_token: cancellationToken.Token);
			}
		}

		public async Task Close ()
		{
			if (cancellationToken == null)
				throw new InvalidOperationException ("Cannot close tunnel that was not opened.");
			// cancel process and wait for it to terminate, else we might want to start a second tunnel to the same device
			// which is going to give problems.
			cancellationToken.Cancel ();
			await tcpTunnelExecutionTask;
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposed)
				return;

			if (disposing) {
				// cancel the process that started the tunnel, else we will leave processes alive
				cancellationToken.Cancel ();
			}

			disposed = true;
		}

	}
}
