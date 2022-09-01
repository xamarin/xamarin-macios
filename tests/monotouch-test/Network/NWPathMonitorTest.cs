#if !__WATCHOS__
using System;
using System.Threading;

using Network;
using CoreFoundation;
using Foundation;

using NUnit.Framework;


namespace monotouchtest.Network
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWPathMonitorTest
	{

		NWPathMonitor monitor;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			monitor = new NWPathMonitor ();
		}

		[Test]
		public void StatusPropertyTest ()
		{
			Assert.That (monitor.CurrentPath, Is.Null, "'CurrentPath' property should be null");

			NWPath finalPath = null;
			bool isPathUpdated = false;

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{

				monitor.SnapshotHandler = ( (path) =>
				{
					if (path != null)
					{
						finalPath = monitor.CurrentPath;
						isPathUpdated = true;
					}

				});

				var q = new DispatchQueue (label: "monitor");
				monitor.SetQueue (q);
				monitor.Start ();

			}, () => isPathUpdated);

			Assert.That (finalPath, Is.Not.Null, "'CurrentPath' property should not be null");
		}

		[Test]
		public void PathIsAlwaysUpdatedWithNewHandlerTest ()
		{
			NWPath oldPath = monitor.CurrentPath;
			NWPath newPath = monitor.CurrentPath;
			bool isOldPathSet = false;
			bool isNewPathSet = false;
			var cbEvent = new AutoResetEvent (false);

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{

				monitor.SnapshotHandler = ( (path) =>
				{
					if (path != null)
					{
						oldPath = monitor.CurrentPath;
						isOldPathSet = true;
						cbEvent.Set ();
					}
				});

				var q = new DispatchQueue (label: "monitor");
				monitor.SetQueue (q);
				monitor.Start ();

			}, () => isOldPathSet);


			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
			{
				cbEvent.WaitOne ();
				monitor.SnapshotHandler = ( (path) =>
				{
					if (path != null)
					{
						newPath = monitor.CurrentPath;
						isNewPathSet = true;
					}

				});

				var q = new DispatchQueue (label: "monitor");
				monitor.SetQueue (q);
				monitor.Start ();
			}, () => isNewPathSet);

			Assert.True (isOldPathSet, "isOldPathSet (no timeout)");
			Assert.True (isNewPathSet, "isNewPathSet (no timeout)");
			// they might be the same native objects (happens on macOS and Catalyst) and,
			// in such case, they will have the same `Handle` value, making them equal on the
			// .net profile. However what we want to know here is if the path was updated
			Assert.False (Object.ReferenceEquals (oldPath, newPath), "different instances");
		}

		[TearDown]
		public void TearDown ()
		{
			monitor?.Dispose ();
		}


		[Test]
		public void ProhibitInterfaceTypeTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			Assert.DoesNotThrow (() => {
				monitor.ProhibitInterfaceType (NWInterfaceType.Wifi);
			});
		}
		
		[Test]
		public void CreateForEthernetChannelTest ()
		{
			TestRuntime.AssertXcodeVersion (14, 0);
			using var pathMonitor = NWPathMonitor.CreateForEthernetChannel ();
			Assert.NotNull (pathMonitor);
		}
	}
}
#endif
