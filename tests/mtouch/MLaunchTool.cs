using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Utils;
using Xamarin.Tests;

namespace Xamarin {
	class MLaunchTool : Tool {
		public enum MLaunchAction {
			None,
			Sim,
			Dev,
		}

		public const string None = "None";

#pragma warning disable 649
		public int Verbosity;
		public string SdkRoot;
		public string Sdk;
		public string AppPath;
#pragma warning restore 649

		// These are a bit smarter
		public MLaunchAction Action = MLaunchAction.Sim;
		public Profile Profile = Profile.iOS;

		void AddVerbosity (IList<string> args)
		{
			if (Verbosity == 0) {
				// do nothing
			} else if (Verbosity > 0) {
				args.Add ("-" + new string ('v', Verbosity));
			} else {
				args.Add ("-" + new string ('q', -Verbosity));
			}
		}

		public int Execute ()
		{
			return Execute (Configuration.MlaunchPath, BuildArguments ());
		}

		IList<string> BuildArguments ()
		{
			var sb = new List<string> ();

			switch (Action) {
			case MLaunchAction.None:
				break;
			case MLaunchAction.Sim:
				if (AppPath is null)
					throw new Exception ("No AppPath specified.");
				sb.Add ("--launchsim");
				sb.Add (AppPath);
				break;
			default:
				throw new Exception ("MLaunchAction not specified.");
			}

			if (SdkRoot == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (SdkRoot)) {
				sb.Add ("--sdkroot");
				sb.Add (SdkRoot);
			} else {
				sb.Add ("--sdkroot");
				sb.Add (Configuration.xcode_root);
			}

			AddVerbosity (sb);

			if (Sdk == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (Sdk)) {
				sb.Add ("--sdk");
				sb.Add (Sdk);
			} else {
				sb.Add ("--sdk");
				sb.Add (Profile.ToString ());
			}

			string platformName = null;
			string simType = null;

			switch (Profile) {
			case Profile.iOS:
				platformName = "iOS";
				simType = "iPhone-SE";
				break;
			case Profile.tvOS:
				platformName = "tvOS";
				simType = "Apple-TV-1080p";
				break;
			default:
				throw new Exception ("Profile not specified.");
			}

			if (!string.IsNullOrEmpty (platformName) && !string.IsNullOrEmpty (simType)) {
				var device = string.Format (":v2:runtime=com.apple.CoreSimulator.SimRuntime.{0}-{1},devicetype=com.apple.CoreSimulator.SimDeviceType.{2}", platformName, Configuration.sdk_version.Replace ('.', '-'), simType);
				sb.Add ($"--device:{device}");
			}

			return sb;
		}

		protected override string ToolPath {
			get { return Configuration.MlaunchPath; }
		}

		protected override string MessagePrefix {
			get { return "HT"; }
		}
	}
}
