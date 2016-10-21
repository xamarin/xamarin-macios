using System;
using System.Diagnostics;
using MTouchTests;
using NUnit.Framework;
using System.Text;

namespace Xamarin.Profiler
{
	[TestFixture (MTouch.Profile.Unified)]
	[TestFixture (MTouch.Profile.TVOS)]
	public class TimingTests
	{
		MTouch.Profile profile;
		StringBuilder sb;
		int starsLenght;

		[TestFixtureSetUp]
		public void Init ()
		{
			sb = new StringBuilder ();
			var title = $"**** TimingTests ({profile}) ****";
			starsLenght = title.Length;
			sb.AppendLine (title);
		}

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
		/// Time to build and launch an application on the simulators.</summary>
		/// <remarks>
		/// Note: the measurement is being done with the simulator already open and, on the bots, the app not yet installed.
		/// Warning: If you're running those tests multiple times locally, you may want to reset the simulator manually each time.</remarks>
		[TestCase (MTouchLinker.DontLink)]
		[TestCase (MTouchLinker.LinkSdk)]
		[TestCase (MTouchLinker.LinkAll)]
		public void BuildAndLaunchTime (MTouchLinker linkerMode)
		{
			using (var buildTool = new MTouchTool ()) {
				var linkerModeName = Enum.GetName (typeof (MTouchLinker), linkerMode);
				buildTool.Profile = profile;
				buildTool.Linker = linkerMode;
				buildTool.CreateTemporaryApp (profile, true, "BuildAndLaunchTime" + linkerModeName + profile);

				var sw = new Stopwatch ();
				sw.Start ();
				buildTool.Execute (MTouchAction.BuildSim);
				sw.Stop ();
				var buildTime = sw.Elapsed.TotalSeconds;

				var launchTool = new MLaunchTool ();
				launchTool.AppPath = buildTool.AppPath;
				launchTool.Profile = profile;

				sw.Reset ();
				sw.Start ();
				launchTool.Execute ();
				sw.Stop ();
				var launchTime = sw.Elapsed.TotalSeconds;

				var totalTime = buildTime + launchTime;
				sb.AppendLine (string.Format ("BuildAndLaunchTime - {0}: {1} seconds [build time], {2} seconds [launch time], {3} seconds [total time]", linkerModeName, buildTime.ToString ("#.000"), launchTime.ToString ("#.000"), totalTime.ToString ("#.000")));
			}
		}

		[TestCase (MTouchRegistrar.Dynamic)]
		[TestCase (MTouchRegistrar.Static)]
		public void RegistrarTime (MTouchRegistrar registrarMode)
		{
			using (var buildTool = new MTouchTool ()) {
				var registrarModeName = Enum.GetName (typeof (MTouchRegistrar), registrarMode);
				buildTool.Profile = profile;
				buildTool.Registrar = registrarMode;
				buildTool.NoFastSim = true;
				buildTool.CreateTemporaryApp (profile, true, "RegistrarTime" + registrarModeName + profile);

				var sw = new Stopwatch ();
				sw.Start ();
				buildTool.Execute (MTouchAction.BuildSim);
				sw.Stop ();
				var buildTime = sw.Elapsed.TotalSeconds;

				var launchTool = new MLaunchTool ();
				launchTool.AppPath = buildTool.AppPath;
				launchTool.Profile = profile;

				sw.Reset ();
				sw.Start ();
				launchTool.Execute ();
				sw.Stop ();
				var launchTime = sw.Elapsed.TotalSeconds;

				var totalTime = buildTime + launchTime;
				sb.AppendLine (string.Format ("RegistrarTime - {0}: {1} seconds [build time], {2} seconds [launch time], {3} seconds [total time]", registrarModeName, buildTime.ToString ("#.000"), launchTime.ToString ("#.000"), totalTime.ToString ("#.000")));
			}
		}

		[TestFixtureTearDown]
		public void PrintLogs ()
		{
			sb.AppendLine (new string ('*', starsLenght));
			Console.WriteLine (sb);
		}
	}
}
