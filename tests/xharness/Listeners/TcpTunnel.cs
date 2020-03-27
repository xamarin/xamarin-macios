using System;
using System.Threading;
using System.Threading.Tasks;
using Xharness.Execution;
using Xharness.Execution.Mlaunch;
using Xharness.Logging;

namespace Xharness.Listeners {
	// represents a tunnel created between a device and a host. This tunnel allows the communication between
	// the host and the device via the usb cable.
	public class TcpTunnel : IDisposable {
		bool disposed = false;
		readonly IProcessManager processManager;
		CancellationTokenSource cancellationToken;
		public TaskCompletionSource<bool> startedCompletionSource { get; private set; } = new TaskCompletionSource<bool> ();
		public Task<bool> Started => startedCompletionSource.Task;
		public int Port { get; private set; }

		public TcpTunnel (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public void Start (string device, SimpleTcpListener simpleListener, TimeSpan timeout, ILog mainLog)
		{
			if (device == null)
				throw new ArgumentNullException (nameof (device));
			if (simpleListener == null)
				throw new ArgumentNullException (nameof (simpleListener));
			if (mainLog == null)
				throw new ArgumentNullException (nameof (mainLog));

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
			// do not await, we are not going to block for a process
			// TODO: what to do with the task?
			var tcpTunnelExecutionTask = processManager.ExecuteCommandAsync (tcpArgs, tunnelbackLog, timeout, cancellation_token: cancellationToken.Token);
		}

		public void Close () => cancellationToken.Cancel ();

		public async Task Connect (SimpleTcpListener listener, ILog mainLog) {
			if (listener.Port != Port)
				throw new ArgumentException ($"Listener is not using the correct port. Found {listener.Port} when it should be using {Port}");
			if (mainLog == null)
				throw new ArgumentNullException (nameof (mainLog));
			mainLog.WriteLine ($"Connecting to already existing tunnel at port {Port}");
			// we cannot connect until we have started, we will block here until done
			var canConnect = await Started;
			if (canConnect)
				listener.TunnelHoleThrough.TrySetResult (true);
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
