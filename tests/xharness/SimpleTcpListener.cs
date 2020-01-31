using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace xharness
{
	public class SimpleTcpListener : SimpleListener
	{
		byte[] buffer = new byte [16 * 1024];
		TcpListener server;

		protected override void Stop ()
		{
			// TODO: should we close the accepted socket?
			server.Stop ();
		}

		public override void Initialize ()
		{
			server = new TcpListener (Address, Port);
			server.Start ();

			if (Port == 0)
				Port = ((IPEndPoint) server.LocalEndpoint).Port;
		}

		protected override void Start ()
		{
			bool processed;

			try {
				do {
					Log.WriteLine ("Test log server listening on: {0}:{1}", Address, Port);
					using (var client = server.AcceptSocket ()) {
						processed = Processing (client);
					}
				} while (!AutoExit || !processed);
			}catch (Exception e) {
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

		bool Processing (Socket client)
		{
			Connected (client.RemoteEndPoint.ToString ());
			var fs = OutputWriter;
			int total = 0;
			// now simply copy what we receive but using the socket, not the stream
			// this is due to https://github.com/xamarin/maccore/issues/827
			while (true) {
				byte [] buffer = new byte [client.ReceiveBufferSize];
				int read = 0;

				try {
					read = client.Receive (buffer);
				} catch { // lost connection, bad :/
					break;
				}

				if (read > 0) { //Handle data
					fs.Write (buffer, 0, read);
					fs.Flush ();
					total += read;
				} else { // read 0 means that client disconnected
					break;
				}
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

