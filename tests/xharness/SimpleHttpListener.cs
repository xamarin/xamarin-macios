using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace xharness
{
	public class SimpleHttpListener : SimpleListener
	{
		HttpListener server;

		public override void Initialize ()
		{
			server = new HttpListener ();

			if (Port != 0)
				throw new NotImplementedException ();

			// Try and find an unused port
			int attemptsLeft = 50;
			Random r = new Random ((int) DateTime.Now.Ticks);
			while (attemptsLeft-- > 0) {
				var newPort = r.Next (49152, 65535); // The suggested range for dynamic ports is 49152-65535 (IANA)
				server.Prefixes.Clear ();
				server.Prefixes.Add ("http://*:" + newPort + "/");
				try {
					server.Start ();
					Port = newPort;
					break;
				} catch (Exception ex) {
					Log.WriteLine ("Failed to listen on port {0}: {1}", newPort, ex.Message);
				}
			}
		}

		protected override void Stop ()
		{
			server.Stop ();
		}

		protected override void Start ()
		{
			bool processed;

			try {
				Log.WriteLine ("Test log server listening on: {0}:{1}", Address, Port);
				do {
					var context = server.GetContext ();
					processed = Processing (context);
				} while (!AutoExit || !processed);
			} catch (Exception e) {
				var se = e as SocketException;
				if (se == null || se.SocketErrorCode != SocketError.Interrupted)
					Console.WriteLine ("[{0}] : {1}", DateTime.Now, e);
			} finally {
				try {
					server.Stop ();
				} finally {
					stopped.Set ();
				}
			}
		}

		bool Processing (HttpListenerContext context)
		{
			var finished = false;

			var request = context.Request;
			var response = "OK";

			var stream = request.InputStream;
			var data = string.Empty;
			using (var reader = new StreamReader (stream))
				data = reader.ReadToEnd ();
			stream.Close ();

			switch (request.RawUrl) {
			case "/Start":
				Connected (request.RemoteEndPoint.ToString ());
				break;
			case "/Finish":
				using (var writer = new StreamWriter (OutputStream)) {
					writer.Write (data);
					writer.Flush ();
				}
				finished = true;
				break;
			default:
				Log.WriteLine ("Unknown upload url: {0}", request.RawUrl);
				response = $"Unknown upload url: {request.RawUrl}";
				break;
			}

			var buf = System.Text.Encoding.UTF8.GetBytes (response);
			context.Response.ContentLength64 = buf.Length;
			context.Response.OutputStream.Write (buf, 0, buf.Length);
			context.Response.OutputStream.Close ();
			context.Response.Close ();

			return finished;
		}
	}
}

