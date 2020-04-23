using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Listeners {
	public class SimpleTcpListener : SimpleListener, ITunnelListener {
		readonly bool autoExit;

		byte [] buffer = new byte [16 * 1024];
		bool useTcpTunnel = true;
		TcpListener server;
		TcpClient client;

		public TaskCompletionSource<bool> TunnelHoleThrough { get; private set; } = new TaskCompletionSource<bool> ();

		public SimpleTcpListener (ILog log, ILog testLog, bool autoExit, bool xmlOutput, bool tunnel = false) : base (log, testLog, xmlOutput)
		{
			this.autoExit = autoExit;
			this.useTcpTunnel = tunnel;
		}

		public SimpleTcpListener (int port, ILog log, ILog testLog, bool autoExit, bool xmlOutput, bool tunnel = false) : this (log, testLog, autoExit, xmlOutput, tunnel)
			=> Port = port;

		protected override void Stop ()
		{
			client?.Close ();
			client?.Dispose ();
			server?.Stop ();
		}

		public override void Initialize ()
		{
			if (useTcpTunnel && Port != 0)
				return;

			server = new TcpListener (Address, Port);
			server.Start ();

			if (Port == 0)
				Port = ((IPEndPoint) server.LocalEndpoint).Port;

			if (useTcpTunnel) {
				// close the listener. We have a port. This is not the best
				// way to find a free port, but there is nothing we can do
				// better than this.

				server.Stop ();
			}
		}

		void StartNetworkTcp ()
		{
			bool processed;

			try {
				do {
					Log.WriteLine ("Test log server listening on: {0}:{1}", Address, Port);
					using (client = server.AcceptTcpClient ()) {
						client.ReceiveBufferSize = buffer.Length;
						processed = Processing (client);
					}
				} while (!autoExit || !processed);
			} catch (Exception e) {
				var se = e as SocketException;
				if (se == null || se.SocketErrorCode != SocketError.Interrupted)
					Console.WriteLine ("[{0}] : {1}", DateTime.Now, e);
			} finally {
				try {
					server.Stop ();
				} finally {
					Finished ();
				}
			}
		}

		void StartTcpTunnel ()
		{
			if (!TunnelHoleThrough.Task.Result) { // do nothing until the tunnel is ready
				throw new InvalidOperationException ("Tcp tunnel could not be initialized.");
			}
			bool processed;
			try {
				int timeout = 100;
				var watch = new System.Diagnostics.Stopwatch ();
				watch.Start ();
				while (true) {
					try {
						client = new TcpClient ("localhost", Port);
						Log.WriteLine ("Test log server listening on: {0}:{1}", Address, Port);
						// let the device know we are ready!
						var stream = client.GetStream ();
						var ping = Encoding.UTF8.GetBytes ("ping");
						stream.Write (ping, 0, ping.Length);
						break;

					} catch (SocketException ex) {
						if (timeout == 100 && watch.ElapsedMilliseconds > 20000) {
							timeout = 250; // Switch to a 250ms timeout after 20 seconds
						} else if (timeout == 250 && watch.ElapsedMilliseconds > 120000) {
							// Give up after 2 minutes.
							throw ex;
						}
						Thread.Sleep (timeout);
					}
				}
				do {
					client.ReceiveBufferSize = buffer.Length;
					processed = Processing (client);
				} while (!autoExit || !processed);
			} catch (Exception e) {
				var se = e as SocketException;
				if (se == null || se.SocketErrorCode != SocketError.Interrupted)
					Console.WriteLine ("[{0}] : {1}", DateTime.Now, e);
			} finally {
				Finished ();
			}
		}

		protected override void Start ()
		{
			if (useTcpTunnel) {
				StartTcpTunnel ();
			} else {
				StartNetworkTcp ();
			}
		}

		bool Processing (TcpClient client)
		{
			Connected (client.Client.RemoteEndPoint.ToString ());
			// now simply copy what we receive
			int i;
			int total = 0;
			NetworkStream stream = client.GetStream ();
			var fs = OutputWriter;
			while ((i = stream.Read (buffer, 0, buffer.Length)) != 0) {
				fs.Write (buffer, 0, i);
				fs.Flush ();
				total += i;
			}
			
			if (total < 16) {
				// This wasn't a test run, but a connection from the app (on device) to find
				// the ip address we're reachable on.
				return false;
			}
			return true;
		}
	}
}

