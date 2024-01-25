using System;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace Xamarin.MMP.Tests {
	public partial class MMPTests {
		void CreateRemotingConfigFile (string path)
		{
			using (Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Xamarin.MMP.Tests.remoting.config")) {
				using (StreamReader reader = new StreamReader (stream)) {
					string result = reader.ReadToEnd ();
					File.WriteAllText (path, result);
				}
			}
		}

		const string RemotingConfigCSProjText = "<ItemGroup>" +
													"<BundleResource Include=\"remoting.config\">" +
														"<Link>remoting.config</Link>" +
													"</BundleResource>" +
												"</ItemGroup>";

		const string RemotingTestCode = "System.Runtime.Remoting.RemotingConfiguration.Configure(\"remoting.config\", false);";

		//[Test] Disabled due to https://bugzilla.xamarin.com/show_bug.cgi?id=50230
		public void RemotingConfigruation_RemoteConfigTests ()
		{
			RunMMPTest (tmpDir => {
				CreateRemotingConfigFile (Path.Combine (tmpDir, "remoting.config"));

				var config = new TI.UnifiedTestConfig (tmpDir) {
					ItemGroup = RemotingConfigCSProjText,
					TestCode = RemotingTestCode
				};

				TI.TestUnifiedExecutable (config);

				config.CSProjConfig = "<MonoBundlingExtraArgs>--machine-config=\"\"</MonoBundlingExtraArgs>";
				TI.TestUnifiedExecutable (config);
			});
		}
	}
}
