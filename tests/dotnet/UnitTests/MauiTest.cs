using System.Diagnostics;
using System.Xml;

using Mono.Cecil;

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class MauiTest : TestBaseClass {
		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void BuildMauiApp (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MyMauiApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath);
			Clean (project_path);

			DotNet.InstallWorkload ("maui-tizen");

			var properties = GetDefaultProperties (runtimeIdentifiers);
			var rv = DotNet.AssertBuild (project_path, properties);
			AssertThatLinkerExecuted (rv);
			var infoPlistPath = GetInfoPListPath (platform, appPath);
			var infoPlist = PDictionary.FromFile (infoPlistPath)!;
			Assert.AreEqual ("com.xamarin.mymauiapp", infoPlist.GetString ("CFBundleIdentifier").Value, "CFBundleIdentifier");
			Assert.AreEqual ("MyMauiApp", infoPlist.GetString ("CFBundleDisplayName").Value, "CFBundleDisplayName");
			Assert.AreEqual ("1", infoPlist.GetString ("CFBundleVersion").Value, "CFBundleVersion");
			Assert.AreEqual ("1.0", infoPlist.GetString ("CFBundleShortVersionString").Value, "CFBundleShortVersionString");


			Assert.IsTrue (BinLog.TryFindPropertyValue (rv.BinLogPath, "TrimMode", out var trimModeValue), "Could not find the property 'TrimMode' in the binlog.");
			Assert.IsTrue (BinLog.TryFindPropertyValue (rv.BinLogPath, "_LinkMode", out var linkModeValue), "Could not find the property '_LinkMode' in the binlog.");
			Assert.IsTrue (BinLog.TryFindPropertyValue (rv.BinLogPath, "MtouchLink", out var mtouchLinkValue), "Could not find the property 'MtouchLink' in the binlog.");

			Assert.AreEqual ("copy", trimModeValue, "TrimMode");
			Assert.AreEqual ("None", linkModeValue, "LinkMode");
			Assert.AreEqual ("None", mtouchLinkValue, "MtouchLink");
		}
	}
}
