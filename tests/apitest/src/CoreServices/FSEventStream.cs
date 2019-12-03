using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using CoreServices;
using CoreFoundation;
using ObjCRuntime;
using Foundation;
using NUnit.Framework;
using Xamarin.Mac.Tests;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	public class FSEventStreamTests {

		DirectoryInfo dir;
		FileStream fileToWatch;
		FileStream fileToExclude;
		bool watchedFileChanged;
		bool exludedFileChanged;
		ulong st_dev;
		FSEventStream fsEventStream;
		NSArray pathsToWatchRelativeToDevice;
		EventWaitHandle waitHandle;
		AutoResetEvent watchEventCalled;
		AutoResetEvent excludeEventCalled;

		[TestFixtureSetUp]
		public void Init ()
		{
			dir = Directory.CreateDirectory ("FSEventStreamTests");
			fileToWatch = File.Create ("TempFileToWatch.txt");
			fileToExclude = File.Create ("TempFileToExclude.txt");
			var process = new Process () {
				StartInfo = new ProcessStartInfo {
					FileName = "/usr/bin/stat",
					Arguments = $"-f '%d' FSEventStreamTests/TempFileToWatch.txt",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				}
			};
			process.Start ();
			string result = process.StandardOutput.ReadToEnd ();
			process.WaitForExit ();
			st_dev = UInt64.Parse (result);
			pathsToWatchRelativeToDevice = NSArray.FromStrings (new [] { "FSEventStreamTests" });
		}

		[TestFixtureTearDown]
		public void Destroy ()
		{
			Directory.Delete ("FSEventStreamTests");
			pathsToWatchRelativeToDevice.Dispose ();
		}

		[SetUp]
		public void CreateFSEventStream ()
		{
			watchedFileChanged = false;
			exludedFileChanged = false;
			fsEventStream = new FSEventStream (st_dev, pathsToWatchRelativeToDevice, ulong.MaxValue, TimeSpan.MinValue, FSEventStreamCreateFlags.None);
			Assert.IsNotNull (fsEventStream, "Null");
			fsEventStream.Events += FsEventStream_WatchEvents;
			fsEventStream.ScheduleWithRunLoop (CFRunLoop.Current);
			waitHandle = new EventWaitHandle (false, EventResetMode.AutoReset);
			watchEventCalled = new AutoResetEvent (false);
			excludeEventCalled = new AutoResetEvent (false);
			fsEventStream.Start ();
		}

		public void FsEventStream_WatchEvents (object sender, FSEventStreamEventsArgs args)
		{
			Console.WriteLine ("FsEventStream_WatchEvents");
			watchedFileChanged = true;
			watchEventCalled.Set ();
		}

		[TearDown]
		public void EndFSEventStream ()
		{
			fsEventStream.Stop ();
			fsEventStream.Invalidate ();
			fsEventStream.Dispose ();
		}

		[Test]
		public void GetDeviceBeingWatchedTest ()
		{
			var deviceId = fsEventStream.GetDevice ();
			Console.WriteLine ($"GetDeviceBeingWatchedTest - {st_dev} : {deviceId}");
			Assert.AreEqual (st_dev, deviceId, "Device ID"); //Check if device id is the same
		}

		[Test]
		public void UnscheduleFromRunLoopTest ()
		{
			fsEventStream.UnscheduleFromRunLoop (CFRunLoop.Current);
			File.AppendAllText ("FSEventStreamTests/TempFileToWatch.txt", "Hello World!");
			watchEventCalled.WaitOne (Int32.MaxValue);
			//waitHandle.WaitOne ();
			//fsEventStream.Events
			Assert.IsFalse (watchedFileChanged); //This shouldn't have changed as we unscheduled from loop?
		}

		[Test]
		public void SetExclusionPathsTest ()
		{
			using (var fsEventStreamForExclusion = new FSEventStream (st_dev, pathsToWatchRelativeToDevice, ulong.MaxValue, TimeSpan.MinValue, FSEventStreamCreateFlags.None)) {
				fsEventStreamForExclusion.Events += FsEventStream_ExcludeWatchEvents;

				using (var pathsToExclude = NSArray.FromStrings (new [] { "FSEventStreamTests" })) {
					fsEventStreamForExclusion.SetExclusionPaths (pathsToExclude);
					fsEventStreamForExclusion.ScheduleWithRunLoop (CFRunLoop.Current);
					fsEventStreamForExclusion.Start ();

					File.AppendAllText ("FSEventStreamTests/TempFileToWatch.txt", "Hello World!");
					watchEventCalled.WaitOne (Int32.MaxValue);
					excludeEventCalled.WaitOne (Int32.MaxValue);

					Assert.IsTrue (watchedFileChanged); //fsEventStream callback should've changed this to true?
					Assert.IsFalse (exludedFileChanged);  //fsEventStreamForExclusion callback shouldn't have changed this?
				}

			}
		}

		private void FsEventStream_ExcludeWatchEvents (object sender, FSEventStreamEventsArgs args)
		{
			Console.WriteLine ("FsEventStream_ExcludeWatchEvents");
			exludedFileChanged = true;
			excludeEventCalled.Set ();
		}
	}
}
