// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Mono.Cecil;

#nullable enable

namespace Xamarin.Tests {
	[TestFixture]
	public class PrepareAssembliesTest : TestBaseClass {
		[Test]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64", "managed-static", "SdkOnly", "Release")]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64", "dynamic", "None", "Debug")]
		public void IncrementalBuild (ApplePlatform platform, string runtimeIdentifiers, string registrar, string linkMode, string configuration)
		{
			// An incremental (second, no-source-change) build must not fail.
			// The PrepareAssemblies task must not run in a partial incremental build,
			// because the assembly-preparer needs the complete set of assemblies to
			// resolve inter-assembly references (otherwise it fails with MT4116/MT2362). See
			// https://github.com/dotnet/macios/issues/25938.
			var project = "MySimpleApp";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out var appPath, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = configuration;
			properties ["MtouchLink"] = linkMode;
			properties ["Registrar"] = registrar;

			// The first (clean) build must succeed.
			DotNet.AssertBuild (project_path, properties);

			// The second (incremental) build, without any changes, must also succeed.
			var result = DotNet.AssertBuild (project_path, properties);
			var targets = BinLog.GetAllTargets (result.BinLogPath);
			AssertTargetNotExecuted (targets, "_ExecutePrepareAssemblies", "Incremental preparation");
			AssertTargetNotExecuted (targets, "_ExecutePostprocessAssemblies", "Incremental post-processing");

			var sourcePath = Path.Combine (Path.GetDirectoryName (project_path)!, "..", "AppDelegate.cs");
			Configuration.Touch (sourcePath);
			result = DotNet.AssertBuild (project_path, properties);
			targets = BinLog.GetAllTargets (result.BinLogPath);
			AssertTargetExecuted (targets, "_ExecutePrepareAssemblies", "Source change preparation");
			AssertTargetExecuted (targets, "_ExecutePostprocessAssemblies", "Source change post-processing");

			properties ["DynamicRegistrationSupported"] = "true";
			result = DotNet.AssertBuild (project_path, properties);
			targets = BinLog.GetAllTargets (result.BinLogPath);
			AssertTargetExecuted (targets, "_ExecutePrepareAssemblies", "Configuration change preparation");
			AssertTargetExecuted (targets, "_ExecutePostprocessAssemblies", "Configuration change post-processing");
		}

		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		public void NativeAotIncrementalBuild (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MySimpleApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var project_path = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out _, configuration: configuration);
			Clean (project_path);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = configuration;
			properties ["PublishAot"] = "true";
			properties ["_IsPublishing"] = "true";
			properties ["Registrar"] = "trimmable-static";

			DotNet.AssertBuild (project_path, properties);

			var result = DotNet.AssertBuild (project_path, properties);
			var targets = BinLog.GetAllTargets (result.BinLogPath);
			AssertTargetNotExecuted (targets, "_ExecutePrepareAssemblies", "Incremental NativeAOT preparation");
			AssertTargetNotExecuted (targets, "_ExecutePostprocessAssembliesAfterIlc", "Incremental NativeAOT post-processing");
		}

		[TestCase ("trimmable-static", null, null, true)]
		[TestCase ("managed-static", null, null, false)]
		[TestCase ("trimmable-static", "false", null, false)]
		[TestCase ("trimmable-static", null, "true", false)]
		public void ExportAttributeRemovalEligibility (string registrar, string? trimExportAttributes, string? dynamicRegistrationSupported, bool expectedRemoval)
		{
			var platform = ApplePlatform.iOS;
			var runtimeIdentifiers = "iossimulator-arm64";
			var project = "ExportAttributeRemovalApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var projectPath = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out _, configuration: configuration);
			Clean (projectPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Configuration"] = configuration;
			properties ["EnableAssemblyILStripping"] = "true";
			properties ["MtouchLink"] = registrar == "managed-static" ? "SdkOnly" : "Full";
			properties ["PublishReadyToRun"] = "false";
			properties ["Registrar"] = registrar;
			if (trimExportAttributes is not null)
				properties ["TrimExportAttributes"] = trimExportAttributes;
			if (dynamicRegistrationSupported is not null)
				properties ["DynamicRegistrationSupported"] = dynamicRegistrationSupported;

			var target = "Compile;_ComputePublishTrimmed;_ComputeLinkMode;_ComputeLinkerArguments;_PrepareAssemblies;_SetDynamicRegistrationSupportedFeature;_SetTrimExportAttributesFeature;_ComputeFrameworkFilesToPublish;_ComputeDynamicLibrariesToPublish;ComputeFilesToPublish;_ComputeStripAssemblyIL;_StripAssemblyIL";
			var assemblyDirectory = Path.Combine (GetObjDir (projectPath, platform, runtimeIdentifiers, configuration), "stripped");
			DotNet.AssertBuild (projectPath, properties, target: target);
			var platformAssemblyPath = Path.Combine (assemblyDirectory, Configuration.GetBaseLibraryName (platform));
			var appAssemblyPath = Path.Combine (assemblyDirectory, project + ".dll");

			AssertExportMetadata (platformAssemblyPath, appAssemblyPath, !expectedRemoval);

			if (expectedRemoval) {
				properties ["DynamicRegistrationSupported"] = "true";
				DotNet.AssertBuild (projectPath, properties, target: target);
				AssertExportMetadata (platformAssemblyPath, appAssemblyPath, true);
			}
		}

		static void AssertExportMetadata (string platformAssemblyPath, string appAssemblyPath, bool expected)
		{
			using var platformAssembly = AssemblyDefinition.ReadAssembly (platformAssemblyPath);
			var bundlePath = platformAssembly.MainModule.GetType ("Foundation.NSBundle").Properties.Single (v => v.Name == "BundlePath");
			AssertExport (bundlePath.GetMethod, expected, "direct-binding NSObject wrapper");

			var nsObjectDescription = platformAssembly.MainModule.GetType ("Foundation.NSObject").Properties.Single (v => v.Name == "Description");
			AssertExport (nsObjectDescription.GetMethod, expected, "wrapper ancestor with application subclasses");

			using var appAssembly = AssemblyDefinition.ReadAssembly (appAssemblyPath);
			var applicationType = appAssembly.MainModule.GetType ("ExportAttributeRemovalApp.ExportMetadataApplicationType");
			AssertAttribute (applicationType.Methods.Single (v => v.Name == "ApplicationExport"), "ExportAttribute", expected, "application export");
			AssertAttribute (applicationType.Methods.Single (v => v.Name == "ApplicationAction"), "ActionAttribute", expected, "application action");
			AssertAttribute (applicationType.Properties.Single (v => v.Name == "ApplicationOutlet"), "OutletAttribute", expected, "application outlet");
		}

		[TestCase (false)]
		[TestCase (true)]
		public void ExportAttributeRemovalWithNSXpcInterfaceUsage (bool explicitlyEnabled)
		{
			var platform = ApplePlatform.iOS;
			var runtimeIdentifiers = "iossimulator-arm64";
			var project = "ExportAttributeRemovalApp";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var projectPath = GetProjectPath (project, runtimeIdentifiers: runtimeIdentifiers, platform: platform, out _, configuration: configuration);
			Clean (projectPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["AdditionalDefineConstants"] = "EXPORT_ATTRIBUTE_REMOVAL_NSXPC";
			properties ["Configuration"] = configuration;
			properties ["MtouchLink"] = "Full";
			properties ["Registrar"] = "trimmable-static";
			properties ["DynamicRegistrationSupported"] = "false";
			if (explicitlyEnabled)
				properties ["TrimExportAttributes"] = "true";

			var target = "Compile;_ComputePublishTrimmed;_ComputeLinkMode;_ComputeLinkerArguments;_PrepareAssemblies";
			var expectedMessage = "Export attributes cannot be removed because the application uses an NSXpcInterface overload that obtains a selector from MethodInfo.";
			if (explicitlyEnabled) {
				for (var i = 0; i < 2; i++) {
					var result = DotNet.AssertBuildFailure (projectPath, properties, target: target);
					var errors = BinLog.GetBuildLogErrors (result.BinLogPath).ToArray ();
					AssertErrorMessages (errors, expectedMessage);
				}
			} else {
				var result = DotNet.AssertBuild (projectPath, properties, target: target);
				var warnings = BinLog.GetBuildLogWarnings (result.BinLogPath).FilterWarnings (platform).ToArray ();
				AssertWarningMessages (warnings, expectedMessage);
			}
		}

		static void AssertExport (ICustomAttributeProvider provider, bool expected, string message)
		{
			AssertAttribute (provider, "ExportAttribute", expected, message);
		}

		static void AssertAttribute (ICustomAttributeProvider provider, string name, bool expected, string message)
		{
			var actual = provider.CustomAttributes.Any (v => v.AttributeType.Namespace == "Foundation" && v.AttributeType.Name == name);
			Assert.That (actual, Is.EqualTo (expected), message);
		}
	}
}
