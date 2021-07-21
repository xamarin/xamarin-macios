//
// Unit tests for CFSocket
//
// Authors:
//	Marius Ungureanu <maungu@microsoft.com>
//
// Copyright 2019 Microsoft Inc. All rights reserved.
//

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
#if XAMCORE_2_0
using Foundation;
using CoreFoundation;
using ObjCRuntime;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CFSocketTest {
		static readonly byte[] dataToSend = { 0, 1, 2, 3 };

		[Test]
		public void RetainCount ()
		{
			var received = new ManualResetEvent (false);

			// All constructors end up using the shared private constructor.
			using (var receiver = new CFSocket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
				receiver.SetAddress (IPAddress.Loopback, 0);

				receiver.DataEvent += (o, a) => {
					var data = a.Data;
					Assert.AreEqual (dataToSend.Length, data.Length);
					for (int i = 0; i < data.Length; ++i)
						Assert.AreEqual (dataToSend [i], data [i]);

					received.Set ();
				};

				using (CFSocket sender = CFSocket.CreateConnectedToSocketSignature (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp, receiver.Address, 0)) {
					sender.SendData (dataToSend, 0);

					while (!received.WaitOne (0, true)) {
						CFRunLoop.Current.RunInMode (CFRunLoop.ModeDefault, 0.5, true);
					}

					sender.Invalidate ();
					Assert.That (TestRuntime.CFGetRetainCount (sender.Handle), Is.EqualTo (1), "sender RetainCount");
				}

				receiver.Invalidate ();
				Assert.That (TestRuntime.CFGetRetainCount (receiver.Handle), Is.EqualTo (1), "receiver RetainCount");
			}
		}
	}
}
