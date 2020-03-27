using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Logging {

	[TestFixture]
	public class LogsTest {

		string directory;
		string fileName;
		string description;

		[SetUp]
		public void SetUp ()
		{
			directory = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ());
			fileName = "test-file.txt";
			description = "My description";

			Directory.CreateDirectory (directory);
		}

		[TearDown]
		public void TearDown ()
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
			File.WriteAllText (fullPath, "foo");

			using (var logs = new Logs (directory)) {
				var fileLog = logs.AddFile (fullPath, description);
				Assert.AreEqual (fullPath, fileLog.FullPath, "path"); // path && fullPath are the same
				Assert.AreEqual (Path.Combine (directory, fileName), fileLog.FullPath, "full path");
				Assert.AreEqual (description, fileLog.Description, "description");
			}
		}

		[Test]
		public void AddFileNotInDirTest ()
		{
			var dir1 = Path.Combine (directory, "dir1");
			var dir2 = Path.Combine (directory, "dir2");
			
			Directory.CreateDirectory (dir1);
			Directory.CreateDirectory (dir2);

			var filePath = Path.Combine (dir1, "test-file.txt");
			File.WriteAllText (filePath, "Hello world!");

			using (var logs = new Logs (dir2)) {
				var newPath = Path.Combine (dir2, Path.GetFileNameWithoutExtension (fileName));
				var fileLog = logs.AddFile (filePath, description);
				StringAssert.StartsWith (newPath, fileLog.FullPath, "path"); // assert new path
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
			File.WriteAllText (fullPath, "foo");
			using (var logs = new Logs (directory)) {
				Assert.DoesNotThrow (() => logs.Create (fullPath, null), "throws");
				Assert.AreEqual (1, logs.Count, "count");
			}
		}
	}
}
