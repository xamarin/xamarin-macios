#if !__WATCHOS__
using System;
using System.Threading;

using Network;
using CoreFoundation;
using Foundation;

using NUnit.Framework;


namespace monotouchtest.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWPathMonitorTest {
		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void StatusPropertyTest ()
		{
			using var monitor = new NWPathMonitor ();
			Assert.That (monitor.CurrentPath, Is.Null, "'CurrentPath' property should be null");

			NWPath finalPath = null;
			bool isPathUpdated = false;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), () => {

				monitor.SnapshotHandler = ((path) => {
					if (path is not null) {
						finalPath = monitor.CurrentPath;
						isPathUpdated = true;
						monitor.Cancel ();
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
			using var monitor = new NWPathMonitor ();
			NWPath oldPath = null;
			NWPath newPath = null;

			var q = new DispatchQueue (label: "monitor");
			monitor.SetQueue (q);

			monitor.SnapshotHandler = ((path) => {
				if (path is not null) {
					oldPath = monitor.CurrentPath;
				}
			});
			monitor.Start ();
			TestRuntime.RunAsync (TimeSpan.FromSeconds (3), () => { }, () => oldPath is not null);

			// Set a different handler
			monitor.SnapshotHandler = ((path) => {
				if (path is not null) {
					newPath = monitor.CurrentPath;
				}
			});
			monitor.Start ();
			TestRuntime.RunAsync (TimeSpan.FromSeconds (3), () => { }, () => newPath is not null);
			monitor.Cancel ();

			Assert.IsNotNull (oldPath, "oldPath set (no timeout)");
			Assert.IsNotNull (newPath, "newPath set (no timeout)");
			// they might be the same native objects (happens on macOS and Catalyst) and,
			// in such case, they will have the same `Handle` value, making them equal on the
			// .net profile. However what we want to know here is if the path was updated
			Assert.False (Object.ReferenceEquals (oldPath, newPath), "different instances");
		}

		[Test]
		public void ProhibitInterfaceTypeTest ()
		{
			using var monitor = new NWPathMonitor ();
			TestRuntime.AssertXcodeVersion (13, 0);
			Assert.DoesNotThrow (() => {
				monitor.ProhibitInterfaceType (NWInterfaceType.Wifi);
			});
		}
#if MONOMAC
		[Ignore ("Unusable nil instance returned, verified with ObjC project. Filled rdar://FB11984039.")]
		[Test]
		public void CreateForEthernetChannelTest ()
		{
			TestRuntime.AssertXcodeVersion (14, 0);
			using var pathMonitor = NWPathMonitor.CreateForEthernetChannel ();
			Assert.NotNull (pathMonitor);
		}
#endif
	}
}
#endif
