using System;
using System.Diagnostics;
using Xamarin;
using NUnit.Framework;
using System.Text;

using Xamarin.Tests;

using MTouchLinker = Xamarin.Tests.LinkerOption;
using MTouchRegistrar = Xamarin.Tests.RegistrarOption;

namespace Xamarin.Profiler
{
	[TestFixture (Profile.iOS)]
	[TestFixture (Profile.tvOS)]
	public class TimingTests
	{
		Profile profile;
		StringBuilder sb;
		int starsLenght;

		[OneTimeSetUp]
		public void Init ()
		{
			sb = new StringBuilder ();
			var title = $"**** TimingTests ({profile}) ****";
			starsLenght = title.Length;
			sb.AppendLine (title);
		}

		public TimingTests (Profile profile)
		{
			this.profile = profile;

			// Create dummy app to initialize the simulator.
			using (var buildTool = new MTouchTool ()) {
				buildTool.Profile = profile;
				buildTool.CreateTemporaryApp (true);
				buildTool.Execute (MTouchAction.BuildSim);
				var mlaunch = new MLaunchTool ();
				mlaunch.AppPath = buildTool.AppPath;
				mlaunch.Profile = profile;
				mlaunch.Execute ();
			}
		}

		/// <summary>
		/// Time to build and launch an app on the simulators with different linker modes.</summary>
		/// <param name="linkerMode">
		/// Set the linker to DontLink, LinkSdk or LinkAll.</param>
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
				buildTool.CreateTemporaryApp (true, "BuildAndLaunchTime" + linkerModeName + profile);

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

		/// <summary>
		/// Time to build and launch an app on the simulators with different registrar modes.</summary>
		/// <param name="registrarMode">
		/// Use the dynamic or static registrar.</param>
		/// <remarks>
		/// Note: the measurement is being done with the simulator already open and, on the bots, the app not yet installed.
		/// Warning: If you're running those tests multiple times locally, you may want to reset the simulator manually each time.</remarks>
		[TestCase (MTouchRegistrar.Dynamic)]
		[TestCase (MTouchRegistrar.Static)]
		public void RegistrarTime (MTouchRegistrar registrarMode)
		{
			using (var buildTool = new MTouchTool ()) {
				var registrarModeName = Enum.GetName (typeof (MTouchRegistrar), registrarMode);
				buildTool.Profile = profile;
				buildTool.Registrar = registrarMode;
				buildTool.NoFastSim = true;
				buildTool.CreateTemporaryApp (true, "RegistrarTime" + registrarModeName + profile);

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

		[OneTimeTearDown]
		public void PrintLogs ()
		{
			sb.AppendLine (new string ('*', starsLenght));
			Console.WriteLine (sb);
		}
	}
}
