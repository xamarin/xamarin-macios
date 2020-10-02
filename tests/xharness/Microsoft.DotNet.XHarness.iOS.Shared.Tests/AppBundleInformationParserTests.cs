using System.IO;
using System.Reflection;
using Moq;
using NUnit.Framework;

using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests {
	[TestFixture]
	public class AppBundleInformationParserTests {

		const string appName = "com.xamarin.bcltests.SystemXunit";

		static readonly string outputPath = Path.GetDirectoryName (Assembly.GetAssembly (typeof (AppBundleInformationParser)).Location);
		static readonly string sampleProjectPath = Path.Combine (outputPath, "Samples", "TestProject");
		static readonly string appPath = Path.Combine (sampleProjectPath, "bin", appName + ".app");
		static readonly string projectFilePath = Path.Combine (sampleProjectPath, "SystemXunit.csproj");

		[SetUp]
		public void SetUp ()
		{
			Directory.CreateDirectory (appPath);
		}

		[TearDown]
		public void TearDown ()
		{
			Directory.Delete (appPath, true);
		}

		[Test]
		public void InitializeTest ()
		{
			var parser = new AppBundleInformationParser ();

			var log = new MemoryLog ();
			var processManager = new Mock<IProcessManager> ();
			var info = parser.ParseFromProjectAsync (log, processManager.Object, projectFilePath, TestTarget.Simulator_iOS64, "Debug").Result;

			Assert.AreEqual (appName, info.AppName);
			Assert.AreEqual (appPath, info.AppPath);
			Assert.AreEqual (appPath, info.LaunchAppPath);
			Assert.AreEqual (appName, info.BundleIdentifier);
		}
	}
}
