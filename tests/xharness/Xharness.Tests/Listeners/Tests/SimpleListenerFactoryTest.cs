using System;
using Moq;
using NUnit.Framework;
using Xharness.Listeners;
using Xharness.Logging;

namespace Xharness.Tests.Listeners.Tests {

	[TestFixture]
	public class SimpleListenerFactoryTest {

		Mock<ILog> log;
		SimpleListenerFactory factory;

		[SetUp]
		public void SetUp ()
		{
			log = new Mock<ILog> ();
			factory = new SimpleListenerFactory ();
		}

		[TearDown]
		public void TearDown ()
		{
			log = null;
			factory = null;
		}

		[Test]
		public void CreateNotWatchListener ()
		{
			var transport = factory.Create ("ios", log.Object, true, out var listener, out var listenerFileTemp);
			Assert.AreEqual (ListenerTransport.Tcp, transport, "transport");
			Assert.IsInstanceOf (typeof (SimpleTcpListener), listener, "listener");
			Assert.IsNull (listenerFileTemp, "tmp file");
		}

		[Test]
		public void CreateWatchOSSimulator ()
		{
			var logFullPath = "myfullpath.txt";
			_ = log.Setup (l => l.FullPath).Returns (logFullPath);

			var transport = factory.Create ("watchos", log.Object, true, out var listener, out var listenerFileTemp);
			Assert.AreEqual (ListenerTransport.File, transport, "transport");
			Assert.IsInstanceOf (typeof (SimpleFileListener), listener, "listener");
			Assert.IsNotNull (listenerFileTemp, "tmp file");
			Assert.AreEqual (logFullPath + ".tmp", listenerFileTemp);

			log.Verify (l => l.FullPath, Times.Once);

		}

		[Test]
		public void CreateWatchOSDevice ()
		{
			var transport = factory.Create ("watchos", log.Object, false, out var listener, out var listenerFileTemp);
			Assert.AreEqual (ListenerTransport.Http, transport, "transport");
			Assert.IsInstanceOf (typeof (SimpleHttpListener), listener, "listener");
			Assert.IsNull (listenerFileTemp, "tmp file");
		}
	}
}
