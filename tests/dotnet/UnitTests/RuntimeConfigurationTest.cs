// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Linq;

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class RuntimeConfigurationTest : TestBaseClass {
		[TestCase (ApplePlatform.iOS, "ios-arm64", true)]
		[TestCase (ApplePlatform.iOS, "ios-arm64", false)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64", true)]
		[TestCase (ApplePlatform.TVOS, "tvossimulator-arm64", false)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64", true)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64", false)]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64", true)]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64", false)]
		public void NativeAotInvariantGlobalization (ApplePlatform platform, string runtimeIdentifiers, bool prepareAssemblies)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath ("MySimpleApp", platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["PublishAot"] = "true";
			properties ["_IsPublishing"] = "true";
			properties ["InvariantGlobalization"] = "true";
			properties ["PrepareAssemblies"] = prepareAssemblies.ToString ();

			DotNet.AssertBuild (project_path, properties);

			var appPath = GetAppPath (project_path, platform, runtimeIdentifiers);
			var appExecutable = GetNativeExecutable (platform, appPath);
			Assert.That (appExecutable, Does.Exist, "NativeAOT executable");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifiers, appExecutable);
		}

		[Test]
		[TestCase (ApplePlatform.MacOSX, "osx-arm64")]
		public void BakedIntoGeneratedMain (ApplePlatform platform, string runtimeIdentifiers)
		{
			// For CoreCLR the runtimeconfig.json 'configProperties' are baked into the app as C arrays in the
			// generated main (and assigned to the xamarin_runtime_config_property_* globals), instead of shipping
			// the binary runtimeconfig format and decoding it at startup. Here we build an app with a well-known
			// config property (System.Globalization.Invariant, from InvariantGlobalization) and verify both that
			// it ends up in the generated main and that there's no runtimeconfig.bin in the app bundle.
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["InvariantGlobalization"] = "true";

			DotNet.AssertBuild (project_path, properties);

			var objDir = GetObjDir (project_path, platform, runtimeIdentifiers);
			var generatedMains = Directory.GetFiles (objDir, "main.*.mm", SearchOption.AllDirectories);
			Assert.That (generatedMains, Is.Not.Empty, "A generated main.mm must exist.");

			foreach (var main in generatedMains) {
				var contents = File.ReadAllText (main);
				Assert.That (contents, Does.Contain ("xamarin_runtime_config_property_keys_array"), $"The generated main '{main}' must bake the runtime configuration properties.");
				Assert.That (contents, Does.Contain ("\"System.Globalization.Invariant\""), $"The generated main '{main}' must contain the System.Globalization.Invariant property.");
			}

			var appPath = GetAppPath (project_path, platform, runtimeIdentifiers);
			var runtimeConfigBin = Directory.GetFiles (appPath, "runtimeconfig.bin", SearchOption.AllDirectories);
			Assert.That (runtimeConfigBin, Is.Empty, "No runtimeconfig.bin should be shipped for CoreCLR (the configuration is baked into the app).");
		}
	}
}
