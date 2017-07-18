using System;
using System.Text;
using Xamarin.Utils;
using Xamarin.Tests;

namespace Xamarin
{
	class MLaunchTool : Tool
	{
		public enum MLaunchAction
		{
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

		string GetVerbosity ()
		{
			if (Verbosity == 0)
				return string.Empty;
			if (Verbosity > 0)
				return new string ('-', Verbosity).Replace ("-", "-v ");
			return new string ('-', -Verbosity).Replace ("-", "-q ");
		}

		public int Execute ()
		{
			return Execute (Configuration.MlaunchPath, BuildArguments ());
		}

		string BuildArguments ()
		{
			var sb = new StringBuilder ();

			switch (Action) {
			case MLaunchAction.None:
				break;
			case MLaunchAction.Sim:
				if (AppPath == null)
					throw new Exception ("No AppPath specified.");
				sb.Append (" --launchsim ").Append (StringUtils.Quote (AppPath));
				break;
			default:
				throw new Exception ("MLaunchAction not specified.");
			}

			if (SdkRoot == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (SdkRoot)) {
				sb.Append (" --sdkroot ").Append (StringUtils.Quote (SdkRoot));
			} else {
				sb.Append (" --sdkroot ").Append (StringUtils.Quote (Configuration.xcode_root));
			}

			sb.Append (" ").Append (GetVerbosity ());

			if (Sdk == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (Sdk)) {
				sb.Append (" --sdk ").Append (Sdk);
			} else {
				sb.Append (" --sdk ").Append (MTouch.GetSdkVersion (Profile));
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
				sb.Append (" --device:").Append (StringUtils.Quote (device));
			}

			return sb.ToString ();
		}

		protected override string ToolPath {
			get { return Configuration.MlaunchPath; }
		}

		protected override string MessagePrefix {
			get { return "HT"; }
		}
	}
}
