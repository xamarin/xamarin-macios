using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Tests;

using Xamarin.Utils;

namespace Xamarin {
	class MmpTool : BundlerTool, IDisposable {
		public string ApplicationName;
		public string OutputPath;

		protected override string ToolPath {
			get { return Configuration.MmpPath; }
		}

		protected override string MessagePrefix {
			get { return "MM"; }
		}

		public void Dispose ()
		{
			// Nothing to do here yet
		}

		protected override void BuildArguments (IList<string> sb)
		{
			base.BuildArguments (sb);

			if (!string.IsNullOrEmpty (ApplicationName))
				sb.Add ($"--name={ApplicationName}");

			if (!string.IsNullOrEmpty (OutputPath))
				sb.Add ($"--output={OutputPath}");

			switch (Profile) {
			case Profile.None:
				break;
			case Profile.macOSMobile:
				sb.Add ("--profile=Xamarin.Mac,Version=v2.0,Profile=Mobile");
				break;
			case Profile.macOSFull:
				sb.Add ("--profile=Xamarin.Mac,Version=v4.5,Profile=Full");
				break;
			case Profile.macOSSystem:
				sb.Add ("--profile=Xamarin.Mac,Version=v4.5,Profile=System");
				break;
			default:
				throw new NotImplementedException (Profile.ToString ());
			}
		}

		public override void CreateTemporaryApp (Profile profile, string appName = "testApp", string code = null, IList<string> extraArgs = null, string extraCode = null, string usings = null)
		{
			if (RootAssembly is null) {
				OutputPath = CreateTemporaryDirectory ();
			} else {
				// We're rebuilding an existing executable, so just reuse that
				OutputPath = Path.GetDirectoryName (RootAssembly);
			}
			ApplicationName = appName;
			var app = Path.Combine (OutputPath, appName + ".app");
			Directory.CreateDirectory (app);
			RootAssembly = CompileTestAppExecutable (OutputPath, code, extraArgs, profile, appName, extraCode, usings);
		}

		public override string GetAppAssembliesDirectory ()
		{
			return Path.Combine (OutputPath, ApplicationName + ".app", "Contents", "MonoBundle");
		}
	}
}
