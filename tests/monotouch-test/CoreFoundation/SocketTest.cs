//
// Unit tests for CFSocket
//
// Authors:
//	Marius Ungureanu <maungu@microsoft.com>
//
// Copyright 2019 Microsoft Inc. All rights reserved.
//

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Foundation;
using CoreFoundation;
using ObjCRuntime;
using NUnit.Framework;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CFSocketTest {
		static readonly byte [] dataToSend = { 0, 1, 2, 3 };

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
					var senderHandle = sender.Handle;
					TestRuntime.CFRetain (senderHandle);

					sender.SendData (dataToSend, 0);

					while (!received.WaitOne (0, true)) {
						CFRunLoop.Current.RunInMode (CFRunLoop.ModeDefault, 0.5, true);
					}

					sender.Invalidate ();
					Assert.That (sender.Handle, Is.EqualTo (NativeHandle.Zero), "sender disposed");
					Assert.That (TestRuntime.CFGetRetainCount (senderHandle), Is.EqualTo ((nint) 1), "sender RetainCount");
					TestRuntime.CFRelease (senderHandle);
				}

				var receiverHandle = receiver.Handle;
				TestRuntime.CFRetain (receiverHandle);
				receiver.Invalidate ();
				Assert.That (receiver.Handle, Is.EqualTo (NativeHandle.Zero), "receiver disposed");
				Assert.That (TestRuntime.CFGetRetainCount (receiverHandle), Is.EqualTo ((nint) 1), "receiver RetainCount");
				TestRuntime.CFRelease (receiverHandle);
			}
		}

		[Test]
		public void Collected ()
		{
			// Allocate a determined amount of sockets on a background thread, have them process for a little while,
			// and ensure at least some of them were collected afterwards.
			var socketCount = 20;
			var gchandles = new Tuple<GCHandle, GCHandle> [socketCount];
			var mainLoop = CFRunLoop.Current;
			Exception ex = null;
			var thread = new Thread ((v) => {
				try {
					for (var i = 0; i < socketCount; i++) {
						var received = new ManualResetEvent (false);
						var receiver = new CFSocket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
						receiver.SetAddress (IPAddress.Loopback, 0);
						receiver.DataEvent += (o, a) => {
							var data = a.Data;
							Assert.AreEqual (dataToSend.Length, data.Length);
							for (int i = 0; i < data.Length; ++i)
								Assert.AreEqual (dataToSend [i], data [i]);

							received.Set ();
						};

						var sender = CFSocket.CreateConnectedToSocketSignature (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp, receiver.Address, 0);
						sender.SendData (dataToSend, 0);
						while (!received.WaitOne (0, true)) {
							CFRunLoop.Current.RunInMode (CFRunLoop.ModeDefault, 0.05, true);
						}
						sender.Invalidate ();
						receiver.Invalidate ();

						gchandles [i] = new Tuple<GCHandle, GCHandle> (GCHandle.Alloc (receiver, GCHandleType.Weak), GCHandle.Alloc (sender, GCHandleType.Weak));
						GC.Collect ();
					}
				} catch (Exception e) {
					ex = e;
				}
			}) {
				IsBackground = true,
				Name = "SocketTest.Collected",
			};
			thread.Start ();

			Assert.IsTrue (thread.Join (TimeSpan.FromSeconds (socketCount * 2)), "Completed");
			Assert.IsNull (ex, "No exceptions");
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			GC.Collect ();
			GC.WaitForPendingFinalizers ();

			var collectedPairs = gchandles.Count (v => v.Item1.Target is null && v.Item2.Target is null);
			Assert.That (collectedPairs, Is.GreaterThan (0), "Any pair of GCHandles collected");
		}
	}
}
