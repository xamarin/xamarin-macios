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
			var (transport, listener, listenerTmpFile) = factory.Create (RunMode.iOS, log.Object, true);
			Assert.AreEqual (ListenerTransport.Tcp, transport, "transport");
			Assert.IsInstanceOf (typeof (SimpleTcpListener), listener, "listener");
			Assert.IsNull (listenerTmpFile, "tmp file");
		}

		[Test]
		public void CreateWatchOSSimulator ()
		{
			var logFullPath = "myfullpath.txt";
			_ = log.Setup (l => l.FullPath).Returns (logFullPath);

			var (transport, listener, listenerTmpFile) = factory.Create (RunMode.WatchOS, log.Object, true);
			Assert.AreEqual (ListenerTransport.File, transport, "transport");
			Assert.IsInstanceOf (typeof (SimpleFileListener), listener, "listener");
			Assert.IsNotNull (listenerTmpFile, "tmp file");
			Assert.AreEqual (logFullPath + ".tmp", listenerTmpFile);

			log.Verify (l => l.FullPath, Times.Once);

		}

		[Test]
		public void CreateWatchOSDevice ()
		{
			var (transport, listener, listenerTmpFile) = factory.Create (RunMode.WatchOS, log.Object, false);
			Assert.AreEqual (ListenerTransport.Http, transport, "transport");
			Assert.IsInstanceOf (typeof (SimpleHttpListener), listener, "listener");
			Assert.IsNull (listenerTmpFile, "tmp file");
		}
	}
}
