// this is an adaptation of NUnitLite's TcpWriter.cs with an additional 
// overrides and with network-activity UI enhancement

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

#if __IOS__
using UIKit;
#endif

namespace MonoTouch.NUnit {

	public class TcpTextWriter : TextWriter {
		
		private TcpClient client;
		TcpListener server;
		private StreamWriter writer;

		public TcpTextWriter (string hostName, int port, bool isTunnel = false)
		{
			if (hostName == null)
				throw new ArgumentNullException ("hostName");
			if ((port < 0) || (port > UInt16.MaxValue))
				throw new ArgumentException ("port");
			if (!isTunnel)
				HostName = hostName;

			Port = port;

#if __IOS__
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
#endif

			try {
				if (isTunnel) {
					server = new TcpListener (IPAddress.Any, Port);
					server.Server.ReceiveTimeout = 5000; // timeout after 5s
					server.Start ();
					client = server.AcceptTcpClient ();
					// block until we have the ping from the client side
					int i;
					byte [] buffer = new byte [16 * 1024];
					var stream = client.GetStream ();
					while ((i = stream.Read (buffer, 0, buffer.Length)) != 0) {
						var message = Encoding.UTF8.GetString (buffer);
						if (message.Contains ("ping"))
							break;
					}
				} else {
					client = new TcpClient (HostName, port);
				}
				writer = new StreamWriter (client.GetStream ());
			}
			catch {
#if __IOS__
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
#endif
				throw;
			}
		}
		
		public string HostName { get; private set; }
		
		public int Port { get; private set; }

		// we override everything that StreamWriter overrides from TextWriter
		
		public override System.Text.Encoding Encoding {
			// hardcoded to UTF8 so make it easier on the server side
			get { return System.Text.Encoding.UTF8; }
		}

		public override void Close ()
		{
#if __IOS__
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
#endif
			writer.Close ();
		}
		
		protected override void Dispose (bool disposing)
		{
			 writer.Dispose ();
		}

		public override void Flush ()
		{
			writer.Flush ();
		}

		// minimum to override - see http://msdn.microsoft.com/en-us/library/system.io.textwriter.aspx
		public override void Write (char value)
		{
			writer.Write (value);
		}
		
		public override void Write (char[] buffer)
		{
			 writer.Write (buffer);
		}
		
		public override void Write (char[] buffer, int index, int count)
		{
			writer.Write (buffer, index, count);
		}

		public override void Write (string value)
		{
			writer.Write (value);
		}
		
		// special extra override to ensure we flush data regularly

		public override void WriteLine ()
		{
			writer.WriteLine ();
			writer.Flush ();
		}
	}
}
