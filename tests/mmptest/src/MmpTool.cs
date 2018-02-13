using System;
using System.IO;
using System.Text;
using Xamarin.Tests;

using Xamarin.Utils;

namespace Xamarin
{
	class MmpTool : BundlerTool, IDisposable
	{
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

		protected override void BuildArguments (StringBuilder sb)
		{
			base.BuildArguments (sb);

			if (!string.IsNullOrEmpty (ApplicationName))
				sb.Append (" --name=").Append (StringUtils.Quote (ApplicationName));

			if (!string.IsNullOrEmpty (OutputPath))
				sb.Append (" --output=").Append (StringUtils.Quote (OutputPath));

			switch (Profile) {
			case Profile.macOSMobile:
				sb.Append (" --profile=Xamarin.Mac,Version=v2.0,Profile=Mobile");
				break;
			case Profile.macOSFull:
				sb.Append (" --profile=Xamarin.Mac,Version=v4.5,Profile=Full");
				break;
			case Profile.macOSSystem:
				sb.Append (" --profile=Xamarin.Mac,Version=v4.5,Profile=System");
				break;
			default:
				throw new NotImplementedException (Profile.ToString ());
			}
		}

		public override void CreateTemporaryApp (Profile profile, string appName = "testApp", string code = null, string extraArg = "", string extraCode = null, string usings = null, bool use_csc = false)
		{
			if (RootAssembly == null) {
				OutputPath = CreateTemporaryDirectory ();
			} else {
				// We're rebuilding an existing executable, so just reuse that
				OutputPath = Path.GetDirectoryName (RootAssembly);
			}
			ApplicationName = appName;
			var app = Path.Combine (OutputPath, appName + ".app");
			Directory.CreateDirectory (app);
			RootAssembly = CompileTestAppExecutable (OutputPath, code, extraArg, profile, appName, extraCode, usings, use_csc);
		}

		public override string GetAppAssembliesDirectory()
		{
			return Path.Combine (OutputPath, ApplicationName + ".app", "Contents", "MonoBundle");
		}
	}
}
