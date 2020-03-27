using System;
using System.IO;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Logging {

	[TestFixture]
	public class ConsoleLogTest {

		string testFile;
		TextWriter sdoutWriter;
		ConsoleLog log;

		[SetUp]
		public void SetUp ()
		{
			testFile = Path.GetTempFileName ();
			log = new ConsoleLog ();
			sdoutWriter = Console.Out;
		}

		[Test]
		public void FullPathTest () => Assert.Throws<NotSupportedException> (() => { string path = log.FullPath; });

		[Test]
		public void TestWrite ()
		{
			var message = "This is a log message";
			using (FileStream testStream = new FileStream (testFile, FileMode.OpenOrCreate, FileAccess.Write))
			using (StreamWriter writer = new StreamWriter (testStream)) {
				Console.SetOut (writer);
				// simply test that we do write in the file. We need to close the stream to be able to read it
				log.WriteLine (message);
			}

			using (FileStream testStream = new FileStream (testFile, FileMode.OpenOrCreate, FileAccess.Read))
			using (var reader = new StreamReader (testStream)) {
				var line = reader.ReadLine ();
				StringAssert.EndsWith (message, line, "output"); // consider the time stamp
			}

		}
		[TearDown]
		public void TearDown ()
		{
			Console.SetOut (sdoutWriter); // get back to write to the console
			File.Delete (testFile);
		}
	}
}
