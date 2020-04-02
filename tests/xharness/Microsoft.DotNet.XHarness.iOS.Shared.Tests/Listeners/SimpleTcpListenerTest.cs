using System.IO;
using System.Net.Sockets;
using Moq;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Listeners {

	[TestFixture]
	public class SimpleTcpListenerTest {

		Mock<ILog> log;
		Mock<ILog> testLog;

		[SetUp]
		public void SetUp ()
		{
			log = new Mock<ILog> ();
			testLog = new Mock<ILog> ();
		}

		[TearDown]
		public void TearDown ()
		{
			log = null;
			testLog = null;
		}

		[Test]
		public void ProcessTest ()
		{
			var tempResult = Path.GetTempFileName ();
			// create a stream to be used and write the data there
			var lines = new string [] { "first line", "second line", "last line" };
			// setup the expected data to be written
			testLog.Setup (l => l.Write (It.IsAny<byte []> (), 0, It.IsAny<int> ())).Callback<byte [], int, int> ((buffer, start, end) => {
				using (var resultStream = File.Create (tempResult)) {// opening closing a lot, but for the test we do not care
					resultStream.Write (buffer, start, end);
					resultStream.Flush ();
				}
			});
			// create a linstener that will start in an other thread, connect to it
			// and send the data.
			var listener = new SimpleTcpListener (log.Object, testLog.Object, true, true);
			listener.Initialize ();
			var connectionPort = listener.Port;
			listener.StartAsync ();
			// create a tcp client which will write the logs, then verity that
			// the expected data was provided
			var client = new TcpClient ();
			client.Connect ("localhost", connectionPort);
			using (var networkStream = client.GetStream ())
			using (var streamWriter = new StreamWriter (networkStream)) {
				foreach (var line in lines) {
					streamWriter.WriteLine (line);
					streamWriter.Flush ();
				}
			}
			listener.Cancel ();
			bool firstLineFound = false;
			bool secondLineFound = false;
			bool lastLineFound = false;
			// read the data in the tempResult and ensure lines are present
			using (var reader = new StreamReader (tempResult)) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					if (line.EndsWith (lines [0]))
						firstLineFound = true;
					if (line.EndsWith (lines [1]))
						secondLineFound = true;
					if (line.EndsWith (lines [2]))
						lastLineFound = true;
				}
			}
			Assert.IsTrue (firstLineFound, "first line");
			Assert.IsTrue (secondLineFound, "second line");
			Assert.IsTrue (lastLineFound, "last line");
		}
	}
}
