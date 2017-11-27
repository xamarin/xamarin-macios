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
					using (TcpClient client = server.AcceptTcpClient ()) {
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

