using System;
using UIKit;
using Network;
using CoreFoundation;
using Foundation;

using NUnit.Framework;


namespace monotouchtest.Network {
    [TestFixture]
    [Preserve(AllMembers = true)]
    public class NWPathMonitorTest {

        NWPathMonitor monitor;

        [TestFixtureSetUp]
        public void Init()
        {
            TestRuntime.AssertXcodeVersion (10, 0);
            monitor = new NWPathMonitor ();
        }

        [Test]
        public void StatusPropertyTest ()
        {
            Assert.That (monitor.currentPath, Is.Null);

            NWPath finalPath = null;
            bool isPathUpdated = false;
            
            TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () =>
            {

                monitor.SetUpdatedSnapshotHandler ((path) =>
                {
                    if (path != null) {
                        finalPath = monitor.currentPath;
                        isPathUpdated = true;
                    }

                });

                var q = new DispatchQueue (label: "monitor");
                monitor.SetQueue (q);
                monitor.Start ();

            }, () => isPathUpdated);

            Assert.That (finalPath, Is.Not.Null);
        }


        [TearDown]
        public void TearDown ()
        {
            monitor?.Dispose ();
        }

    }
}
