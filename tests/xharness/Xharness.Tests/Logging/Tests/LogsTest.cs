using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Xharness.Logging;

namespace Xharness.Tests.Logging.Tests {

	[TestFixture]
	public class LogsTest {

		string directory;
		string fileName;
		string description;

		[SetUp]
		public void SetUp ()
		{
			directory = Path.GetTempFileName ();
			fileName = "test-log.txt";
			description = "My description";
			File.Delete (directory);
			Directory.CreateDirectory (directory);

		}

		[TearDown]
		public void TeatDown ()
		{
			if (Directory.Exists (directory))
				Directory.Delete (directory, true);
		}

		[Test]
		public void ConstructorTest ()
		{
			using (var logs = new Logs (directory)) {
				Assert.AreEqual (directory, logs.Directory);
			}
		}

		[Test]
		public void ConstructorNullDirTest ()
		{
			Assert.Throws<ArgumentNullException> (() => new Logs (null));
		}

		[Test]
		public void CreateFileTest ()
		{
			using (var logs = new Logs (directory)) {
				var file = logs.CreateFile (fileName, description);
				Assert.IsTrue (File.Exists (file), "exists");
				Assert.AreEqual (fileName, Path.GetFileName (file), "file name");
				Assert.AreEqual (1, logs.Count, "count");
			}
		}

		[Test]
		public void CreateFileNullPathTest ()
		{
			using (var logs = new Logs (directory)) {
				fileName = null;
				var description = "My description";
				Assert.Throws<ArgumentNullException> (() => logs.CreateFile (fileName, description));
			}
		}

		[Test]
		public void CreateFileNullDescriptionTest ()
		{
			using (var logs = new Logs (directory)) {
				string description = null;
				Assert.DoesNotThrow (() => logs.CreateFile (fileName, description), "null description");
				Assert.AreEqual (1, logs.Count, "count");
			}
		}

		[Test]
		public void AddFileTest ()
		{
			var fullPath = Path.Combine (directory, fileName);
			File.Create (fullPath);
			using (var logs = new Logs (directory)) {
				var fileLog = logs.AddFile (fullPath, description);
				Assert.AreEqual (fullPath, fileLog.Path, "path"); // path && fullPath are the same
				Assert.AreEqual (Path.Combine (directory, fileName), fileLog.FullPath, "full path");
				Assert.AreEqual (description, fileLog.Description, "description");
			}
		}

		[Test]
		public void AddFileNotInDirTest ()
		{
			var fullPath = Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), fileName);
			using (var stream = File.Create (fullPath))
			using (var writer = new StreamWriter (stream)){
				writer.WriteLine ("Hello world!");
			}
			using (var logs = new Logs (directory)) {
				var newPath = Path.Combine (directory, Path.GetFileNameWithoutExtension (fileName));
				var fileLog = logs.AddFile (fileName, description);
				StringAssert.StartsWith (newPath, fileLog.Path, "path"); // assert new path
				StringAssert.StartsWith (newPath, fileLog.FullPath, "full path"); // assert new path is used
				Assert.IsTrue (File.Exists (fileLog.FullPath), "copy");
			}
		}

		[Test]
		public void AddFilePathNullTest ()
		{
			using (var logs = new Logs (directory)) {
				Assert.Throws<ArgumentNullException> (() => logs.AddFile (null, description));
			}
		}

		[Test]
		public void AddFileDescriptionNull ()
		{
			var fullPath = Path.Combine (directory, fileName);
			File.Create (fullPath);
			using (var logs = new Logs (directory)) {
				Assert.DoesNotThrow (() => logs.Create (fullPath, null), "throws");
				Assert.AreEqual (1, logs.Count, "count");
			}
		}

	}
}
