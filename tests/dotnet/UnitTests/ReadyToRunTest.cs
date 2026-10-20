// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Mono.Cecil;

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class ReadyToRunTest : TestBaseClass {
		[TestCase (false)]
		[TestCase (true)]
		public void TrimmableRegistrarTypeMaps (bool publishReadyToRun)
		{
			var platform = ApplePlatform.MacOSX;
			var runtimeIdentifier = "osx-arm64";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);

			var projectPath = GetProjectPath ("MySimpleApp", runtimeIdentifiers: runtimeIdentifier, platform: platform, out var appPath, configuration: configuration);
			Clean (projectPath);

			var properties = GetDefaultProperties (runtimeIdentifier);
			properties ["Configuration"] = configuration;
			properties ["UseMonoRuntime"] = "false";
			properties ["PublishReadyToRun"] = publishReadyToRun.ToString ();
			properties ["Registrar"] = "trimmable-static";

			DotNet.AssertBuild (projectPath, properties);

			var typeMapDirectory = Path.Combine (GetObjDir (projectPath, platform, runtimeIdentifier, configuration), "typemap");
			var typeMapAssemblies = Directory.GetFiles (typeMapDirectory, "*.dll");
			Assert.That (typeMapAssemblies, Is.Not.Empty, "Generated type map assemblies");
			foreach (var path in typeMapAssemblies) {
				using var assembly = AssemblyDefinition.ReadAssembly (path);
				Assert.That (assembly.MainModule.GetType ("<TypeReferences>"), Is.Null, $"Unnecessary TypeRef workaround in {path}");
			}

			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifier, GetNativeExecutable (platform, appPath));
		}

		[TestCase ("Debug", "CoreLib")]
		[TestCase ("Release", "Full")]
		public void AssemblySelection (string configuration, string expectedReadyToRunConfiguration)
		{
			var platform = ApplePlatform.iOS;
			var runtimeIdentifier = "iossimulator-arm64";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);

			var projectPath = GetProjectPath ("MySimpleApp", runtimeIdentifiers: runtimeIdentifier, platform: platform, out _, configuration: configuration);
			Clean (projectPath);

			var properties = GetDefaultProperties (runtimeIdentifier);
			properties ["Configuration"] = configuration;
			properties ["UseMonoRuntime"] = "false";
			properties ["ExpectedReadyToRunConfiguration"] = expectedReadyToRunConfiguration;

			var rv = DotNet.AssertBuildFailure (projectPath, properties);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			AssertErrorMessages (errors, "All good!");
		}
	}
}
