using System.IO;
using System.Reflection;

using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.iOS.Tasks;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class CollectITunesArtworkTaskTests : TestBase {
		string AppPath => Path.GetDirectoryName (GetType ().Assembly.Location);

		[Test]
		public void UnknownImageFormat ()
		{
			var task = CreateTask<CollectITunesArtwork> ();
			task.ITunesArtwork = new TaskItem [] { new TaskItem (Assembly.GetExecutingAssembly ().Location) };

			Assert.IsFalse (task.Execute (), "Execute failure");
			Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
			Assert.That (Engine.Logger.ErrorEvents [0].Message, Does.Match ("Error loading '.*/Xamarin.MacDev.Tasks.Tests.dll': Unknown image format."), "ErrorMessage");
		}

		[Test]
		public void InexistentFile ()
		{
			var task = CreateTask<CollectITunesArtwork> ();
			task.ITunesArtwork = new TaskItem [] { new TaskItem ("this-file-does-not-exist.tiff") };

			Assert.IsFalse (task.Execute (), "Execute failure");
			Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
			Assert.That (Engine.Logger.ErrorEvents [0].Message, Does.Match ("'.*/this-file-does-not-exist.tiff' not found."), "ErrorMessage");
		}

		[Test]
		[TestCase ("jpg")]
		[TestCase ("png")]
		public void InvalidSize (string extension)
		{
			var task = CreateTask<CollectITunesArtwork> ();
			task.ITunesArtwork = new TaskItem [] { new TaskItem (Path.Combine (AppPath, "Resources", "iTunesArtwork-invalid-size." + extension)) };

			Assert.IsFalse (task.Execute (), "Execute failure");
			Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
			Assert.That (Engine.Logger.ErrorEvents [0].Message, Does.Match ($"Invalid iTunesArtwork dimensions [(]124x124[)] for '.*/iTunesArtwork-invalid-size.{extension}'."), "ErrorMessage");
		}

		[Test]
		[TestCase ("", "512x512")]
		[TestCase ("@2x", "1024x1024")]
		public void MultipleSameSize (string size, string dimension)
		{
			var task = CreateTask<CollectITunesArtwork> ();
			task.ITunesArtwork = new TaskItem [] {
				new TaskItem (Path.Combine (AppPath, "Resources", $"iTunesArtwork{size}.jpg")),
				new TaskItem (Path.Combine (AppPath, "Resources", $"iTunesArtwork{size}.png")),
			};

			Assert.IsFalse (task.Execute (), "Execute failure");
			Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
			Assert.That (Engine.Logger.ErrorEvents [0].Message, Does.Match ($"Multiple iTunesArtwork files with the same dimensions detected [(]{dimension}[)] for '.*/Resources/iTunesArtwork{size}.png'."), "ErrorMessage");
		}

		[Test]
		[TestCase ("jpg")]
		[TestCase ("png")]
		public void Valid (string extension)
		{
			var task = CreateTask<CollectITunesArtwork> ();
			task.ITunesArtwork = new TaskItem [] {
				new TaskItem (Path.Combine (AppPath, "Resources", $"iTunesArtwork.{extension}")),
				new TaskItem (Path.Combine (AppPath, "Resources", $"iTunesArtwork@2x.{extension}")),
			};

			Assert.IsTrue (task.Execute (), "Execute");
			Assert.AreEqual (0, Engine.Logger.ErrorEvents.Count, "ErrorCount");
			Assert.AreEqual (2, task.ITunesArtworkWithLogicalNames.Length, "ITunesArtworkWithLogicalNames.Count");
			for (var i = 0; i < task.ITunesArtworkWithLogicalNames.Length; i++) {
				Assert.AreEqual (Path.GetFileNameWithoutExtension (task.ITunesArtwork [i].GetMetadata ("FullPath")), task.ITunesArtworkWithLogicalNames [i].GetMetadata ("LogicalName"), $"LogicalName #{i}");
				Assert.AreEqual ("false", task.ITunesArtworkWithLogicalNames [i].GetMetadata ("Optimize"), $"Optimize #{i}");
			}

		}

	}
}
