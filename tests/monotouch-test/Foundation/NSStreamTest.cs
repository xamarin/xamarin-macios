using System;
using NUnit.Framework;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Foundation;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSStreamTest {
		[Test]
		public void BoundPairTest ()
		{
			NSInputStream read;
			NSOutputStream write;

			NSStream.CreateBoundPair (out read, out write, 1024);
			read.Open ();
			write.Open ();

			var send = Encoding.ASCII.GetBytes ("hello, world");
			nint n = send.Length;

			Assert.AreEqual (n, write.Write (send));
			var result = new byte [n + 10];

			Assert.AreEqual (n, read.Read (result, (uint) n));
			for (int i = 0; i < n; i++)
				Assert.AreEqual (send [i], result [i], "Item " + i);

		}

#if !__WATCHOS__
		TcpListener FindPort (out int port)
		{
			// This does not work well on watchOS:
			// The request to start the tcp listener will fail, but
			// at the same time leave a file descriptor (the socket) open
			// until the TcpListener is collected by the GC.
			// Since we create 3000 TcpListeners here, we end up using
			// up all the available file descriptors, causing trouble
			// for later tests.
			for (port = 3000; port < 6000; port++) {
				var listener = new TcpListener (IPAddress.Any, port);
				try {
					listener.Start ();
					return listener;
				} catch {
				}
			}
			return null;
		}

		[Test]
		public void ConnectToHost ()
		{
			NSInputStream read;
			NSOutputStream write;

			int port;
			var listener = FindPort (out port);
			if (listener is null) {
				Assert.Inconclusive ("Not possible to bind a port");
				return;
			}

			var listenThread = new Thread (new ParameterizedThreadStart (DebugListener));
			listenThread.Start (listener);
			NSStream.CreatePairWithSocketToHost (new IPEndPoint (IPAddress.Loopback, port), out read, out write);
			read.Open ();
			write.Open ();
			var send = new byte [] { 1, 2, 3, 4, 5 };
			Assert.AreEqual ((nint) 5, write.Write (send));
			var result = new byte [5];
			Assert.AreEqual ((nint) 5, read.Read (result, 5));
			for (int i = 0; i < 5; i++)
				Assert.AreEqual (send [i] * 10, result [i]);
			listenThread.Join ();
			listener.Stop ();
			read.Close ();
			write.Close ();
		}

		[Test]
		public void ConnectToPeer ()
		{
			NSInputStream read;
			NSOutputStream write;

			int port;
			var listener = FindPort (out port);
			if (listener is null) {
				Assert.Inconclusive ("Not possible to bind a port");
				return;
			}

			var listenThread = new Thread (new ParameterizedThreadStart (DebugListener));
			listenThread.Start (listener);
			NSStream.CreatePairWithPeerSocketSignature (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, new IPEndPoint (IPAddress.Loopback, port), out read, out write);
			read.Open ();
			write.Open ();
			var send = new byte [] { 1, 2, 3, 4, 5 };
			Assert.AreEqual ((nint) 5, write.Write (send), "Write");
			var result = new byte [5];
			Assert.AreEqual ((nint) 5, read.Read (result, 5), "Read");
			for (int i = 0; i < 5; i++)
				Assert.AreEqual (send [i] * 10, result [i]);
			listenThread.Join ();
			listener.Stop ();
			read.Close ();
			write.Close ();
		}

		void DebugListener (object data)
		{
			var listener = data as TcpListener;
			var client = listener.AcceptTcpClient ();
			var stream = client.GetStream ();

			byte [] buffer = new byte [512];
			if (stream.Read (buffer, 0, 5) == 5) {
				stream.Write (new byte [] { 10, 20, 30, 40, 50 }, 0, 5);
				stream.Flush ();
			}
			client.Close ();
		}
#endif // !__WATCHOS__
	}
}
