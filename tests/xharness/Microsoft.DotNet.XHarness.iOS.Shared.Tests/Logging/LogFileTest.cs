using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Logging {

	[TestFixture]
	public class LogFileTest {

		string path;
		string description;

		Mock<ILogs> logs;

		[SetUp]
		public void SetUp ()
		{
			description = "My log";
			path = Path.GetTempFileName ();
			logs = new Mock<ILogs> ();
			File.Delete (path); // delete the empty file
		}

		[TearDown]
		public void TearDown ()
		{
			if (File.Exists (path))
				File.Delete (path);
			path = null;
			description = null;
			logs = null;
		}

		[Test]
		public void ConstructorTest ()
		{
			using (var log = new LogFile (description, path)) {
				Assert.AreEqual (description, log.Description, "description");
				Assert.AreEqual (path, log.FullPath, "path");
			}
		}

		[Test]
		public void ConstructorNullPathTest ()
		{
			Assert.Throws<ArgumentNullException> (() => { var log = new LogFile (description, null); });
		}

		[Test]
		public void ConstructorNullDescriptionTest ()
		{
			Assert.DoesNotThrow (() => {
#pragma warning disable CS0642 // Possible mistaken empty statement
				using (var log = new LogFile (null, path)) ;
#pragma warning restore CS0642 // Possible mistaken empty statement
			});
		}

		
		[Test]
		public void WriteTest ()
		{
			string oldLine = "Hello world!";
			string newLine = "Hola mundo!";
			// create a log, write to it and assert that we have the expected data
			using (var stream = File.Create (path))
			using (var writer = new StreamWriter (stream)) {
				writer.WriteLine (oldLine);
			}
			using (var log = new LogFile (description, path)) {
				log.WriteLine (newLine);
				log.Flush ();
			}
			bool oldLineFound = false;
			bool newLineFound = false;
			using (var reader = new StreamReader (path)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					if (line == oldLine)
						oldLineFound = true;
					if (line.EndsWith (newLine)) // consider time stamp
						newLineFound = true;
				}
			}

			Assert.IsTrue (oldLineFound, "old line");
			Assert.IsTrue (newLineFound, "new line");
		}

		[Test]
		public void WriteNotAppendTest ()
		{
			string oldLine = "Hello world!";
			string newLine = "Hola mundo!";
			// create a log, write to it and assert that we have the expected data
			using (var stream = File.Create (path))
			using (var writer = new StreamWriter (stream)) {
				writer.WriteLine (oldLine);
			}
			using (var log = new LogFile (description, path, false)) {
				log.WriteLine (newLine);
				log.Flush ();
			}
			bool oldLineFound = false;
			bool newLineFound = false;
			using (var reader = new StreamReader (path)) {
				string line;
				while ((line = reader.ReadLine ()) != null) {
					if (line == oldLine)
						oldLineFound = true;
					if (line.EndsWith (newLine)) // consider timestamp
						newLineFound = true;
				}
			}

			Assert.IsFalse (oldLineFound, "old line");
			Assert.IsTrue (newLineFound, "new line");
		}

	}
}
