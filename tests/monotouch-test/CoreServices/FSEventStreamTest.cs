//
// Unit tests for FSEventStream
//

#if __MACOS__

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using CoreFoundation;
using CoreServices;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreServices {
	using static FSEventStreamCreateFlags;
	using static FSEventStreamEventFlags;

	[TestFixture]
	[Preserve (AllMembers = true)]
	public sealed class FSEventStreamTest {
		[Test]
		public void TestPathsBeingWatched ()
		{
			FSEventStreamCreateOptions createOptions = new () {
				Flags = FileEvents | UseExtendedData,
				PathsToWatch = new [] {
					Xamarin.Cache.CreateTemporaryDirectory (),
					Xamarin.Cache.CreateTemporaryDirectory (),
					Xamarin.Cache.CreateTemporaryDirectory (),
					Xamarin.Cache.CreateTemporaryDirectory ()
				}
			};

			var stream = createOptions.CreateStream ();

			CollectionAssert.AreEqual (
				createOptions.PathsToWatch,
				stream.PathsBeingWatched);

			Assert.AreEqual (0, stream.DeviceBeingWatched);
		}

		[Test]
		public void TestPathsBeingWatchedRelativeToDevice ()
		{
			FSEventStreamCreateOptions createOptions = new () {
				Flags = FileEvents | UseExtendedData,
				DeviceToWatch = 123456789,
				PathsToWatch = new [] { string.Empty }
			};

			var stream = createOptions.CreateStream ();

			CollectionAssert.AreEqual (
				createOptions.PathsToWatch,
				stream.PathsBeingWatched);

			Assert.AreEqual (123456789, stream.DeviceBeingWatched);
		}

		[Test]
		public void TestFileEvents ()
			=> RunTest (FileEvents);

		[Test]
		public void TestExtendedFileEvents ()
			=> RunTest (FileEvents | UseExtendedData);

		static void RunTest (FSEventStreamCreateFlags createFlags)
			=> new TestFSMonitor (
				Xamarin.Cache.CreateTemporaryDirectory (),
				createFlags,
				maxFilesToCreate: 256).Run ();

		/// <summary>
		/// Creates a slew of files on a background thread in some directory
		/// while simultaneously running an FSEventStream against a private
		/// dispatch queue for that directory, and blocks/pumps the main thread
		/// while the following work is settling on the two other threads:
		///
		/// (1) create a bunch of files and directories;
		///
		/// (2) as the FSEventStream raises events on the dispatch queue,
		/// reflect the events (e.g. file created vs file deleted) in our state;
		/// if a file was created, delete it, which will trigger another event
		/// for the deletion to be recorded.
		///
		/// (3) when everything has settled (created + deleted), ensure that
		/// all created files were seen as created through the FSEventStream and
		/// then subsequently seen as deleted.
		/// </summary>
		sealed class TestFSMonitor : FSEventStream {
			static readonly TimeSpan s_testTimeout = TimeSpan.FromSeconds (10);

			readonly int _directoriesToCreate;
			readonly int _filesPerDirectoryToCreate;
			readonly List<string> _createdDirectories = new ();
			readonly List<string> _createdThenRemovedFiles = new ();
			readonly List<string> _createdFiles = new ();
			readonly List<string> _removedFiles = new ();
			readonly AutoResetEvent _monitor = new (false);
			readonly DispatchQueue _dispatchQueue = new (nameof (FSEventStreamTest));
			readonly List<Exception> _exceptions = new ();
			readonly string _rootPath;
			readonly FSEventStreamCreateFlags _createFlags;

			public TestFSMonitor (
				string rootPath,
				FSEventStreamCreateFlags createFlags,
				long maxFilesToCreate)
				: base (new [] { rootPath }, TimeSpan.Zero, createFlags)
			{
				_rootPath = rootPath;
				_createFlags = createFlags;

				_directoriesToCreate = (int) Math.Sqrt (maxFilesToCreate);
				_filesPerDirectoryToCreate = _directoriesToCreate;
			}

			public void Run ()
			{
				SetDispatchQueue (_dispatchQueue);
				Assert.IsTrue (Start ());

				var isWorking = true;

				Task.Run (CreateFilesAndWaitForFSEventsThread)
					.ContinueWith (task => {
						isWorking = false;
						if (task.Exception is not null) {
							if (task.Exception is AggregateException ae)
								_exceptions.AddRange (ae.InnerExceptions);
							else
								_exceptions.Add (task.Exception);
						}
					});

				while (isWorking)
					NSRunLoop.Current.RunUntil (NSDate.Now.AddSeconds (0.1));

				Invalidate ();

				if (_exceptions.Count > 0) {
					Console.WriteLine ($"Got {_exceptions.Count} exceptions:");
					for (var e = 0; e < _exceptions.Count; e++) {
						Console.WriteLine ($"    #{e + 1}: {_exceptions [e].ToString ().Replace ("\n", "\n        ")}");
					}
					if (_exceptions.Count > 1)
						throw new AggregateException (_exceptions);
					else
						throw _exceptions [0];
				}

				Assert.IsEmpty (_createdDirectories);
				Assert.IsEmpty (_createdFiles);
				Assert.IsNotEmpty (_removedFiles);

				_removedFiles.Sort ();
				_createdThenRemovedFiles.Sort ();
				CollectionAssert.AreEqual (_createdThenRemovedFiles, _removedFiles);

				Console.WriteLine (
					"Observed {0} files created and then removed (flags: {1})",
					_createdThenRemovedFiles.Count,
					_createFlags);
			}

			void CreateFilesAndWaitForFSEventsThread ()
			{
				for (var i = 0; i < _directoriesToCreate; i++) {
					var level1Path = Path.Combine (_rootPath, Guid.NewGuid ().ToString ());

					lock (_monitor) {
						_createdDirectories.Add (level1Path);
						Directory.CreateDirectory (level1Path);
					}

					for (var j = 0; j < _filesPerDirectoryToCreate; j++) {
						var level2Path = Path.Combine (level1Path, Guid.NewGuid ().ToString ());

						lock (_monitor) {
							_createdFiles.Add (level2Path);
							_createdThenRemovedFiles.Add (level2Path);
							File.Create (level2Path).Dispose ();
						}
					}

					FlushSync ();
				}

				while (true) {
					int createdDirCount;
					int createdFileCount;
					int removedFileCount;
					int createdThenRemovedFileCount;
					lock (_monitor) {
						createdDirCount = _createdDirectories.Count;
						createdFileCount = _createdFiles.Count;
						removedFileCount = _removedFiles.Count;
						createdThenRemovedFileCount = _createdThenRemovedFiles.Count;

					}

					var timedOut = !_monitor.WaitOne (s_testTimeout);

					lock (_monitor) {
						if (_createdDirectories.Count == 0 &&
							_createdFiles.Count == 0 &&
							_removedFiles.Count == _createdThenRemovedFiles.Count)
							break;
					}

					if (timedOut)
						throw new TimeoutException (
							$"test has timed out at {s_testTimeout.TotalSeconds}s; " +
							"increase the timeout or reduce the number of files created. " +
							$"Created directories: {createdDirCount} Created files: {createdFileCount} Removed files: {removedFileCount} Created then removed files: {createdThenRemovedFileCount}");
				}
			}

			protected override void OnEvents (FSEvent [] events)
			{
				try {
					foreach (var evnt in events) {
						lock (_monitor) {
							HandleEvent (evnt);
						}
					}
				} catch (Exception e) {
					_exceptions.Add (e);
				} finally {
					_monitor.Set ();
				}

				void HandleEvent (FSEvent evnt)
				{
					Assert.IsNotNull (evnt.Path);
					// Roslyn analyzer doesn't consider the assert above wrt nullability
					if (evnt.Path is null)
						return;

					if (_createFlags.HasFlag (UseExtendedData))
						Assert.Greater (evnt.FileId, 0);

					if (evnt.Flags.HasFlag (ItemCreated)) {
						if (evnt.Flags.HasFlag (ItemIsFile)) {
							_createdFiles.Remove (evnt.Path);

							File.Delete (evnt.Path);
						}

						if (evnt.Flags.HasFlag (ItemIsDir))
							_createdDirectories.Remove (evnt.Path);
					}

					if (evnt.Flags.HasFlag (ItemRemoved) && !_removedFiles.Contains (evnt.Path))
						_removedFiles.Add (evnt.Path);
				}
			}
		}
	}
}

#endif // __MACOS__
