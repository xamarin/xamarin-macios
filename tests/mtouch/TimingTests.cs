using System;
using System.Diagnostics;
using MTouchTests;
using NUnit.Framework;

namespace Xamarin.Profiler
{
	[TestFixture (MTouch.Profile.Unified)]
	[TestFixture (MTouch.Profile.TVOS)]
	public class TimingTests
	{
		MTouch.Profile profile;

		public TimingTests (MTouch.Profile profile)
		{
			this.profile = profile;

			// Create dummy app to initialize the simulator.
			using (var buildTool = new MTouchTool ()) {
				buildTool.Profile = profile;
				buildTool.CreateTemporaryApp (profile, true);
				buildTool.Execute (MTouchAction.BuildSim);
				var mlaunch = new MLaunchTool ();
				mlaunch.AppPath = buildTool.AppPath;
				mlaunch.Profile = profile;
				mlaunch.Execute ();
			}
		}

		/// <summary>
		/// Time to launch an application on the simulators.</summary>
		/// <remarks>
		/// Note: the measurement is being done with the simulator closed and the app not yet installed.</remarks>
		[Test]
		public void AppLaunchTime ()
		{
			using (var buildTool = new MTouchTool ()) {
				buildTool.Profile = profile;
				buildTool.CreateTemporaryApp (profile, true, "AppLaunchTime" + profile);

				buildTool.Execute (MTouchAction.BuildSim);

				var sw = new Stopwatch ();
				var launchTool = new MLaunchTool ();
				launchTool.AppPath = buildTool.AppPath;
				launchTool.Profile = profile;

				sw.Start ();
				launchTool.Execute ();
				sw.Stop ();

				Console.WriteLine ("TimingTests - AppLaunchTime ({0}): {1} seconds", profile, sw.Elapsed.TotalSeconds.ToString ("#.000"));
			}
		}
	}
}
