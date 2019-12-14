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
		FileStream fileToWatch;
		FileStream fileToExclude;
		bool watchedFileChanged;
		bool exludedFileChanged;
		ulong st_dev;
		FSEventStream fsEventStream;
		string [] pathsToWatchRelativeToDevice;
		NSArray nsPathsToWatchRelativeToDevice;
		EventWaitHandle waitHandle;
		AutoResetEvent watchEventCalled;
		AutoResetEvent excludeEventCalled;

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
			st_dev = GetDeviceId ();

			pathsToWatchRelativeToDevice = new [] { dirPath };
			nsPathsToWatchRelativeToDevice = NSArray.FromStrings (pathsToWatchRelativeToDevice);
			dir = Directory.CreateDirectory (dirPath);

			using (FileStream fileToWatch = File.Create (Path.Combine (dirPath, "TempFileToWatch.txt")))
			using (FileStream fileToExclude = File.Create (Path.Combine (dirPath, "TempFileToExclude.txt"))) { }
		}

		[TestFixtureTearDown]
		public void Destroy ()
		{
			Directory.Delete ("FSEventStreamTests");
			nsPathsToWatchRelativeToDevice.Dispose ();
		}

		[SetUp]
		public void CreateFSEventStream ()
		{
			watchedFileChanged = false;
			exludedFileChanged = false;

			fsEventStream = new FSEventStream (st_dev, pathsToWatchRelativeToDevice, FSEvent.SinceNowId, TimeSpan.FromSeconds (1), FSEventStreamCreateFlags.WatchRoot | FSEventStreamCreateFlags.FileEvents | FSEventStreamCreateFlags.NoDefer);
			Assert.IsNotNull (fsEventStream, "Null");
		}

		[TearDown]
		public void EndFSEventStream ()
		{
			fsEventStream.Stop ();
			fsEventStream.Invalidate ();
			fsEventStream.Dispose ();
		}

		//[Test]
		public void GetDeviceBeingWatchedTest ()
			=> Assert.AreEqual (st_dev, fsEventStream.GetDevice (), "Device ID");

		//[Test]
		public void FSEventFileChangedTest ()
		{
			Console.WriteLine ("FSEventFileChangedTest");
			string path = Path.Combine (dirPath, "TempFileToWatch.txt");
			var taskCompletionSource = new TaskCompletionSource<FSEventStreamEventsArgs> ();
			FSEventStreamEventsArgs args = null;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (10), async () => {
				fsEventStream.Events += (sender, eventArgs) => {
					taskCompletionSource.SetResult (eventArgs);
					watchedFileChanged = true;
				};
				fsEventStream.ScheduleWithRunLoop (CFRunLoop.Current);
				fsEventStream.Start ();
				File.AppendAllText (path, "Hello World!");
				Assert.IsTrue (File.Exists (path));
				args = await taskCompletionSource.Task.ConfigureAwait (false);

			}, () => watchedFileChanged);

			Assert.IsNotNull (args, "Null args");
		}

		//[Test]
		public void UnscheduleFromRunLoopTest ()
		{
			Console.WriteLine ("UnscheduleFromRunLoopTest");
			string path = Path.Combine (dirPath, "TempFileToWatch.txt");
			var taskCompletionSource = new TaskCompletionSource<FSEventStreamEventsArgs> ();
			FSEventStreamEventsArgs args = null;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (10), async () => {
				fsEventStream.Events += (sender, eventArgs) => {
					taskCompletionSource.SetResult (eventArgs);
					watchedFileChanged = true;
				};
				fsEventStream.UnscheduleFromRunLoop (CFRunLoop.Current); // unscheduling from the RunLoop shouldn't trigger events 
				fsEventStream.Start ();
				File.AppendAllText (path, "Hello World!");
				Assert.IsTrue (File.Exists (path));
				args = await taskCompletionSource.Task.ConfigureAwait (false);

			}, () => watchedFileChanged);

			Assert.IsNull(args, "Null args");
		}

		[Test]
		public void SetExclusionPathsTest ()
		{
			Console.WriteLine ("SetExclusionPathsTest");

			//var watchDir = Directory.CreateDirectory (Path.Combine (dirPath, "WatchDir"));
			var unWatchDir = Directory.CreateDirectory (Path.Combine (dirPath, "UnwatchDir"));
			//var fsEventStream2 = new FSEventStream (st_dev, pathsToWatchRelativeToDevice, FSEvent.SinceNowId, TimeSpan.FromSeconds (1), FSEventStreamCreateFlags.WatchRoot | FSEventStreamCreateFlags.FileEvents | FSEventStreamCreateFlags.NoDefer);
			//var succ = fsEventStream2.SetExclusionPaths (NSArray.FromStrings (new [] { unWatchDir.FullName })); // excluding the unwatch dir so changing the file there won't trigger events
			//var succ = fsEventStream2.SetExclusionPaths (new [] { unWatchDir.FullName, Path.Combine (unWatchDir.FullName, "TempFileToExclude.txt") });
			var succ = fsEventStream.SetExclusionPaths (new [] { unWatchDir.FullName });
			Console.WriteLine ($"unWatchDir.FullName - {unWatchDir.FullName}");


			//using (FileStream fileToWatch1 = File.Create (Path.Combine (watchDir.FullName, "TempFileToWatch.txt")))
			using (FileStream fileToWatch1 = File.Create (Path.Combine (dir.FullName, "TempFileToWatch.txt")))
			using (FileStream fileToExclude1 = File.Create (Path.Combine (unWatchDir.FullName, "TempFileToExclude.txt"))) {
				fileToWatch1.Close ();
				fileToExclude1.Close ();

				//Console.WriteLine ($"Files made - {Path.Combine (watchDir.FullName, "TempFileToWatch.txt")} and {Path.Combine (unWatchDir.FullName, "TempFileToExclude.txt")}");

				var taskCompletionSource = new TaskCompletionSource<FSEventStreamEventsArgs> ();
				FSEventStreamEventsArgs args = null;

				TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
					fsEventStream.Events += (sender, eventArgs) => {
						taskCompletionSource.SetResult (eventArgs);
					};

					fsEventStream.ScheduleWithRunLoop (CFRunLoop.Current);
					fsEventStream.Start ();
					File.AppendAllText (Path.Combine (unWatchDir.FullName, "TempFileToExclude.txt"), "Adding to excluded file!");
					File.AppendAllText (Path.Combine (dir.FullName, "TempFileToWatch.txt"), "Adding to included file!");
					//File.AppendAllText (Path.Combine (watchDir.FullName, "TempFileToWatch.txt"), "Adding to included file!");

					args = await taskCompletionSource.Task.ConfigureAwait (false);

				}, () => watchedFileChanged);

				fsEventStream.Show ();

				Assert.IsNotNull (args, "Null args");
				Console.WriteLine ($"args.Events.Length - {args.Events.Length}");
				Assert.AreEqual (args.Events.Length, 1, "more events triggered"); // Should only trigger the watched file event
				Console.WriteLine ($"args.Events [0].Path - {args.Events [0].Path}");
				//Assert.AreEqual (args.Events [0].Path, Path.Combine (watchDir.FullName, "TempFileToWatch.txt"), "event triggered path not the same"); // the triggered event should be from the watched dir
				Assert.AreEqual (args.Events [0].Path, Path.Combine (dir.FullName, "TempFileToWatch.txt"), "event triggered path not the same"); // the triggered event should be from the watched dir

				//fsEventStream2.Stop ();
				//fsEventStream2.Invalidate ();
				//fsEventStream2.Dispose ();
			}
		}

		//private void FsEventStream_ExcludeWatchEvents (object sender, FSEventStreamEventsArgs args)
		//{
		//	Console.WriteLine ("FsEventStream_ExcludeWatchEvents");
		//	exludedFileChanged = true;
		//	excludeEventCalled.Set ();
		//}

		//[Test] // Null is not working - binding incorrect?
		public void SetDispatchQueueTest ()
		{
			Console.WriteLine ("SetDispatchQueueTest");
			string path = Path.Combine (dirPath, "TempFileToWatch.txt");
			var taskCompletionSource = new TaskCompletionSource<FSEventStreamEventsArgs> ();
			FSEventStreamEventsArgs args = null;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (10), async () => {
				fsEventStream.Events += (sender, eventArgs) => {
					taskCompletionSource.SetResult (eventArgs);
					watchedFileChanged = true;
				};
				fsEventStream.ScheduleWithRunLoop (CFRunLoop.Current);
				fsEventStream.SetDispatchQueue (null); // shouldn't recieve events after this
				fsEventStream.Start ();
				File.AppendAllText (path, "Hello World!");
				Assert.IsTrue (File.Exists (path));
				args = await taskCompletionSource.Task.ConfigureAwait (false);

			}, () => watchedFileChanged);

			Assert.IsNull (args, "Null args");
		}
	}
}
