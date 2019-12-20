using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using CoreServices;
using CoreFoundation;
using ObjCRuntime;
using Foundation;
using NUnit.Framework;
using Xamarin.Mac.Tests;
using System.Threading.Tasks;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	public class FSEventStreamTests {

		string dirPath;
		DirectoryInfo dir;
		bool watchedFileChanged;
		bool exludedFileChanged;
		ulong st_dev;
		FSEventStream fsEventStream;
		string [] pathsToWatchRelativeToDevice;
		bool fsEventStreamStarted;

		ulong GetDeviceId ()
		{
			var process = new Process () {
				StartInfo = new ProcessStartInfo {
					FileName = "/usr/bin/stat",
					Arguments = $"-f '%d' {Path.Combine (dirPath, "TempFileToWatch.txt")}",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				}
			};
			process.Start ();
			string result = process.StandardOutput.ReadToEnd ();
			process.WaitForExit ();
			return UInt64.Parse (result);
		}

		[TestFixtureSetUp]
		public void Init ()
		{
			dirPath = Path.Combine (Path.GetTempPath (), "FSEventStreamTests");

			// The reason why we take from substring from index 1 is because in this particular API, the paths should be relative to
			// the root of the device. So instead of "/Pictures/July", we need to provide "Pictures/July".
			// Refer to https://developer.apple.com/documentation/coreservices/1447341-fseventstreamcreaterelativetodev?language=objc
			pathsToWatchRelativeToDevice = new [] { dirPath.Substring (1) };
			dir = Directory.CreateDirectory (dirPath);

			using (FileStream fileToWatch = File.Create (Path.Combine (dirPath, "TempFileToWatch.txt")))
			using (FileStream fileToExclude = File.Create (Path.Combine (dirPath, "TempFileToExclude.txt"))) { }
			st_dev = GetDeviceId ();
		}

		[TestFixtureTearDown]
		public void Destroy ()
		{
			Directory.Delete (dirPath, true);
		}

		[SetUp]
		public void CreateFSEventStream ()
		{
			watchedFileChanged = false;
			exludedFileChanged = false;
			fsEventStreamStarted = false;
			fsEventStream = new FSEventStream (st_dev, pathsToWatchRelativeToDevice, FSEvent.SinceNowId, TimeSpan.FromSeconds (1), FSEventStreamCreateFlags.WatchRoot | FSEventStreamCreateFlags.FileEvents | FSEventStreamCreateFlags.NoDefer);
			Assert.IsNotNull (fsEventStream, "Null");
		}

		[TearDown]
		public void EndFSEventStream ()
		{
			if (fsEventStreamStarted) {
				fsEventStream.Stop ();
				fsEventStream.Invalidate ();
			}
			fsEventStream.Dispose ();
		}

		[Test]
		public void EmptyArrayToConstructor ()
			=> Assert.Throws<InvalidOperationException> (() => new FSEventStream (st_dev, new string [] { }, FSEvent.SinceNowId, TimeSpan.FromSeconds (1), FSEventStreamCreateFlags.WatchRoot | FSEventStreamCreateFlags.FileEvents | FSEventStreamCreateFlags.NoDefer));

		[Test]
		public void GetDeviceBeingWatchedTest ()
			=> Assert.AreEqual (st_dev, fsEventStream.GetDevice (), "Device ID");

		[Test]
		public void FSEventFileChangedTest ()
		{
			string path = Path.Combine (dirPath, "TempFileToWatch.txt");
			var taskCompletionSource = new TaskCompletionSource<FSEventStreamEventsArgs> ();
			FSEventStreamEventsArgs args = null;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				fsEventStream.Events += (sender, eventArgs) => {
					taskCompletionSource.SetResult (eventArgs);
					watchedFileChanged = true;
				};
				fsEventStream.ScheduleWithRunLoop (CFRunLoop.Current);
				fsEventStreamStarted = fsEventStream.Start ();
				File.AppendAllText (path, "Hello World!");
				Assert.IsTrue (File.Exists (path));
				args = await taskCompletionSource.Task.ConfigureAwait (false);

			}, () => watchedFileChanged);

			Assert.IsNotNull (args, "Null args");
		}

		[Test]
		public void UnscheduleFromRunLoopTest ()
		{
			string path = Path.Combine (dirPath, "TempFileToWatch.txt");
			var taskCompletionSource = new TaskCompletionSource<FSEventStreamEventsArgs> ();
			FSEventStreamEventsArgs args = null;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				fsEventStream.Events += (sender, eventArgs) => {
					taskCompletionSource.SetResult (eventArgs);
					watchedFileChanged = true;
				};
				fsEventStream.ScheduleWithRunLoop (CFRunLoop.Current); // need to schedule first before calling unschedule
				fsEventStream.UnscheduleFromRunLoop (CFRunLoop.Current); // unscheduling from the RunLoop shouldn't trigger events 
				fsEventStreamStarted = fsEventStream.Start ();
				File.AppendAllText (path, "Hello World!");
				Assert.IsTrue (File.Exists (path));
				args = await taskCompletionSource.Task.ConfigureAwait (false);

			}, () => watchedFileChanged);

			Assert.IsNull(args, "Null args");
		}

		[Test]
		public void SetExclusionPathsTest ()
		{
			var watchDirPath = Path.Combine (dirPath, "WatchDir");
			var watchDir = Directory.CreateDirectory (watchDirPath);
			var unWatchDirPath = Path.Combine (dirPath, "UnwatchDir");
			var unWatchDir = Directory.CreateDirectory (unWatchDirPath);

			// Passing empty array returns a false
			Assert.IsFalse (fsEventStream.SetExclusionPaths (new string [] { }), "SetExclusionPaths empty array failed");

			// Excluding the unWatchDirPath from the watcher so any event inside it doesn't get
			var exclusionPaths = new string [] { Path.Combine (fsEventStream.PathsBeingWatched [0], "UnwatchDir"), Path.Combine (fsEventStream.PathsBeingWatched [0], "UnwatchDir") };
			Assert.IsTrue (fsEventStream.SetExclusionPaths (exclusionPaths), "SetExclusionPaths failed");

			Assert.True (true);
			FileStream fileToWatch = File.Create (Path.Combine (watchDirPath, "TempFileToWatch.txt"));
			FileStream fileToExclude = File.Create (Path.Combine (unWatchDirPath, "TempFileToExclude.txt"));
			fileToWatch.Close ();
			fileToExclude.Close ();

			var taskCompletionSource = new TaskCompletionSource<FSEventStreamEventsArgs> ();
			FSEventStreamEventsArgs args = null;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				fsEventStream.Events += (sender, eventArgs) => {
					taskCompletionSource.SetResult (eventArgs);
				};

				fsEventStream.ScheduleWithRunLoop (CFRunLoop.Current);
				fsEventStreamStarted = fsEventStream.Start ();
				File.AppendAllText (Path.Combine (unWatchDirPath, "TempFileToExclude.txt"), "Adding to excluded file!");
				File.AppendAllText (Path.Combine (watchDirPath, "TempFileToWatch.txt"), "Adding to included file!");

				args = await taskCompletionSource.Task.ConfigureAwait (false);

			}, () => watchedFileChanged);

			fsEventStream.Show ();

			Assert.IsNotNull (args, "Null args");
			// Assert that only one event is triggered and that the path is that of the watched file
			Assert.AreEqual (args.Events.Length, 1, "More events triggered");
			Assert.AreEqual (Path.Combine (fsEventStream.PathsBeingWatched [0], "WatchDir", "TempFileToWatch.txt"), args.Events [0].Path, "Watched file not triggered");
		}
	}
}
