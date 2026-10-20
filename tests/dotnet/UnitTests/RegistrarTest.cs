using Mono.Cecil;

namespace Xamarin.Tests {
	[TestFixture]
	public class RegistrarTest : TestBaseClass {
		// The 'Registrar' property is computed inside the 'SelectRegistrar' target (not at evaluation
		// time), so its value is only recorded in the binlog when MSBuild's property tracking is
		// enabled. This isn't the case by default (in particular not on CI), so explicitly enable it
		// (just for the child 'dotnet build' process) for the tests in this file that need to look up
		// the resulting value of $(Registrar) in the binlog.
		static readonly Dictionary<string, string?> EnablePropertyTracking = new Dictionary<string, string?> {
			{ "MSBuildLogPropertyTracking", "1" },
		};

		[TestCase (ApplePlatform.iOS, "None", "partial-static")]
		[TestCase (ApplePlatform.iOS, "SdkOnly", "trimmable-static")]
		public void DefaultCoreCLRSimulatorRegistrar (ApplePlatform platform, string linkMode, string expectedRegistrar)
		{
			var runtimeIdentifiers = "iossimulator-arm64";

			Configuration.IgnoreIfIgnoredPlatform (platform);

			var projectPath = GetProjectPath ("MySimpleApp", platform: platform);
			Clean (projectPath);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["MtouchLink"] = linkMode;
			properties ["UseMonoRuntime"] = "false";

			var result = DotNet.AssertBuild (projectPath, properties, environmentVariables: EnablePropertyTracking);

			Assert.That (BinLog.TryFindPropertyValue (result.BinLogPath, "Registrar", out var registrar), Is.True, "Could not find the 'Registrar' property in the binlog.");
			Assert.That (registrar, Is.EqualTo (expectedRegistrar), "Registrar");
			Assert.That (BinLog.TryFindPropertyValue (result.BinLogPath, "PrepareAssemblies", out var prepareAssemblies), Is.True, "Could not find the 'PrepareAssemblies' property in the binlog.");
			Assert.That (prepareAssemblies, Is.EqualTo ("true"), "PrepareAssemblies");
		}

		[TestCase (ApplePlatform.iOS)]
		public void ChangeDefaultCoreCLRSimulatorRegistrar (ApplePlatform platform)
		{
			var runtimeIdentifiers = "iossimulator-arm64";

			Configuration.IgnoreIfIgnoredPlatform (platform);

			var projectPath = GetProjectPath ("MySimpleApp", platform: platform);
			Clean (projectPath);

			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["MtouchLink"] = "None";
			properties ["UseMonoRuntime"] = "false";
			properties ["Registrar"] = "trimmable-static";
			DotNet.AssertBuild (projectPath, properties);

			properties.Remove ("Registrar");
			var result = DotNet.AssertBuild (projectPath, properties, environmentVariables: EnablePropertyTracking);

			Assert.That (BinLog.TryFindPropertyValue (result.BinLogPath, "Registrar", out var registrar), Is.True, "Could not find the 'Registrar' property in the binlog.");
			Assert.That (registrar, Is.EqualTo ("partial-static"), "Registrar");

			var targets = BinLog.GetAllTargets (result.BinLogPath);
			Assert.That (targets.Any (v => v.TargetName == "_ExecutePrepareAssemblies" && !v.Skipped), Is.True, "_ExecutePrepareAssemblies should execute when the registrar changes.");

			var objDir = GetObjDir (projectPath, platform, runtimeIdentifiers);
			var registrarItemsPath = Path.Combine (objDir, "linker-items", "_RegistrarFile.items");
			Assert.That (registrarItemsPath, Does.Exist, "_RegistrarFile.items");
			Assert.That (File.ReadAllText (registrarItemsPath), Does.Not.Contain ("<_RegistrarFile Include="), "_RegistrarFile.items");
		}

		// This test does evil things that the AOT runtime complains about, so it only works when not running the AOT compiler (aka x64 when using Mono).
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-x64")]
		[TestCase (ApplePlatform.MacOSX, "osx-x64")]
		public void InvalidStaticRegistrarValidation (ApplePlatform platform, string runtimeIdentifiers)
		{
			var project = "MyRegistrarApp";
			var configuration = "Debug";

			Configuration.IgnoreIfIgnoredPlatform (platform);

			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

			var projectPath = GetProjectPath (project, platform: platform);
			Clean (projectPath);
			var properties = GetDefaultProperties (runtimeIdentifiers);
			properties ["Registrar"] = "static";
			// enable the linker (so that the main assembly is modified)
			properties ["LinkMode"] = "full";
			properties ["MtouchLink"] = "full";

			DotNet.AssertBuild (projectPath, properties);

			var appDir = GetAppPath (projectPath, platform, runtimeIdentifiers, configuration);
			var asmDir = Path.Combine (appDir, GetRelativeAssemblyDirectory (platform));
			var appExecutable = Path.Combine (asmDir, project + ".dll");

			// Save the first version of the main assembly in memory
			var firstAssembly = File.ReadAllBytes (appExecutable);

			// Build again, including additional code
			properties ["AdditionalDefineConstants"] = "INCLUDED_ADDITIONAL_CODE";
			DotNet.AssertBuild (projectPath, properties);

			// Revert to the original version of the main assembly
			File.WriteAllBytes (appExecutable, firstAssembly);

			Environment.SetEnvironmentVariable ("XAMARIN_VALIDATE_STATIC_REGISTRAR_CODE", "1");
			try {
				ExecuteProjectWithMagicWordAndAssert (projectPath, platform, runtimeIdentifiers);
			} finally {
				Environment.SetEnvironmentVariable ("XAMARIN_VALIDATE_STATIC_REGISTRAR_CODE", null);
			}
		}

		[TestCase (ApplePlatform.MacCatalyst, false)]
		[TestCase (ApplePlatform.MacOSX, false)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.TVOS, false)]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.TVOS, true)]
		public void ClassRewriterTest (ApplePlatform platform, bool rewriteHandles)
		{
			var project = "MyClassRedirectApp";
			var configuration = "Debug";
			var runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform);
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var projectPath = GetProjectPath (project, platform: platform);
			Clean (projectPath);
			var properties = GetDefaultProperties ();
			properties ["Registrar"] = "static";
			// enable the linker (so that the main assembly is modified)
			properties ["LinkMode"] = "full";
			properties ["MtouchLink"] = "full";
			properties ["InlineClassGetHandle"] = "disabled";
			if (rewriteHandles)
				properties ["MtouchExtraArgs"] = "--optimize=redirect-class-handles";

			DotNet.AssertBuild (projectPath, properties);

			var appDir = GetAppPath (projectPath, platform, runtimeIdentifiers, configuration);
			var asmDir = Path.Combine (appDir, GetRelativeAssemblyDirectory (platform));

			var appExecutable = Path.Combine (asmDir, project + ".dll");
			var platformDll = Path.Combine (asmDir, Configuration.GetBaseLibraryName (platform));
			Assert.That (File.Exists (platformDll), "No platform dll.");
			var module = ModuleDefinition.ReadModule (platformDll);
			var classHandlesMaybe = AllTypes (module).FirstOrDefault (t => t.FullName == "ObjCRuntime.Runtime/ClassHandles");
			Assert.That (classHandlesMaybe, Is.Not.Null, "Couldn't find ClassHandles type.");
			var classHandles = classHandlesMaybe!;
			if (!rewriteHandles) {
				// NB: there is always at least one field named "unused"
				var fields = classHandles.Fields.Where (f => f.Name != "unused").Select (f => f.Name).ToList ();
				var sb = new StringBuilder ();
				foreach (var f in fields) {
					sb.Append (" ").Append (f);
				}
				Assert.That (fields.Count == 0, "There are fields in classHandles - rewriter was called when it should have done nothing." + sb);
			} else {
				// NB: there is always at least one field named "unused"
				Assert.That (classHandles.HasFields && classHandles.Fields.Count () > 1, "There are no fields in ClassHandles - rewriter did nothing.");
				var field = classHandles.Fields.FirstOrDefault (f => f.Name.Contains ("SomeObj"));
				Assert.That (field, Is.Not.Null, "Didn't find a field for 'SomeObj'");
			}
		}

		[TestCase (ApplePlatform.iOS, "iossimulator-arm64", "26.0", true, true)]
		[TestCase (ApplePlatform.iOS, "iossimulator-arm64", "27.0", false, false)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64", "26.0", false, true)]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64", "27.0", false, false)]
		public void UnavailablePlatformModelRegistrarOutput (ApplePlatform platform, string runtimeIdentifier, string deploymentTarget, bool expectNativeDeclaration, bool expectStaticProtocolLookup)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);

			var projectPath = GenerateProject (platform, nameof (UnavailablePlatformModelRegistrarOutput), runtimeIdentifier, out _);
			var projectDir = Path.GetDirectoryName (projectPath)!;
			File.WriteAllText (Path.Combine (projectDir, "Main.cs"), """
				using System;
				using ObjCRuntime;
				using UIKit;

				class MainClass {
					static int Main ()
					{
				#pragma warning disable 618, CA1422
						GC.KeepAlive (typeof (UIAccelerometerDelegate));
						GC.KeepAlive (new Protocol (typeof (IUIAccelerometerDelegate)));
				#pragma warning restore 618, CA1422
						return 0;
					}
				}
				""");

			var properties = GetDefaultProperties (runtimeIdentifier);
			properties ["UseMonoRuntime"] = "false";
			properties ["Registrar"] = "static";
			properties ["SupportedOSPlatformVersion"] = deploymentTarget;
			DotNet.AssertBuild (projectPath, properties);

			var objDir = GetObjDir (projectPath, platform, runtimeIdentifier);
			var registrarHeader = Directory.GetFiles (objDir, "registrar.h", SearchOption.AllDirectories).Single ();
			var registrarImplementation = Directory.GetFiles (objDir, "registrar.mm", SearchOption.AllDirectories).Single ();
			var headerContents = File.ReadAllText (registrarHeader);
			var implementationContents = File.ReadAllText (registrarImplementation);
			const string modelDeclaration = "UIAccelerometerDelegate : NSObject<UIAccelerometerDelegate>";

			if (expectNativeDeclaration) {
				Assert.That (headerContents, Does.Contain (modelDeclaration), "Native model declaration");
			} else {
				Assert.That (headerContents, Does.Not.Contain (modelDeclaration), "Native model declaration");
			}

			if (expectStaticProtocolLookup) {
				Assert.That (implementationContents, Does.Contain ("@protocol (UIAccelerometerDelegate)"), "Static protocol lookup");
			} else {
				Assert.That (implementationContents, Does.Not.Contain ("@protocol (UIAccelerometerDelegate)"), "Static protocol lookup");
				Assert.That (implementationContents, Does.Contain ("objc_getProtocol (\"UIAccelerometerDelegate\")"), "Dynamic protocol lookup");
			}
		}

		[TestCase (ApplePlatform.iOS, "ios-arm64")]
		[TestCase (ApplePlatform.MacCatalyst, "maccatalyst-arm64")]
		public void IncrementalTrimmableStaticRegistrar (ApplePlatform platform, string runtimeIdentifier)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifier);

			var projectPath = GenerateProject (platform, nameof (IncrementalTrimmableStaticRegistrar), runtimeIdentifier, out var appPath);
			var projectName = Path.GetFileNameWithoutExtension (projectPath);
			var sourcePath = Path.Combine (Path.GetDirectoryName (projectPath)!, "Main.cs");
			var source = """
				// Copyright (c) Microsoft Corporation.
				// Licensed under the MIT License.

				using System;
				using Foundation;
				using ObjCRuntime;

				[Register ("IncrementalRegistrarClass")]
				public class IncrementalRegistrarClass : NSObject {
					[Export ("value")]
					public string Value () => "VERSION";
					EXTRA_EXPORT
				}

				class Program {
					static void Main ()
					{
						using var instance = new IncrementalRegistrarClass ();
						var registeredClass = new Class (typeof (IncrementalRegistrarClass));
						if (registeredClass.Handle == NativeHandle.Zero || Class.Lookup (registeredClass) != typeof (IncrementalRegistrarClass) || instance.Value () != "VERSION")
							throw new InvalidOperationException ("The native class was not registered correctly.");
						Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));
					}
				}
				""";
			var properties = GetDefaultProperties (runtimeIdentifier);
			properties ["UseMonoRuntime"] = "false";
			properties ["Registrar"] = "trimmable-static";
			properties ["MtouchLink"] = "SdkOnly";

			var objDir = GetObjDir (projectPath, platform, runtimeIdentifier);
			var nativeExecutable = GetNativeExecutable (platform, appPath!);
			var appAssembly = Path.Combine (appPath!, GetRelativeAssemblyDirectory (platform), projectName + ".dll");
			var validationEnvironment = new Dictionary<string, string?> {
				{ "XAMARIN_VALIDATE_STATIC_REGISTRAR_CODE", "1" },
			};
			string? registrarContents = null;
			Guid previousMvid = Guid.Empty;
			DateTime registrarObjectTimestamp = default;
			DateTime assembliesObjectTimestamp = default;

			for (var version = 1; version <= 3; version++) {
				File.WriteAllText (sourcePath, source.Replace ("VERSION", version.ToString ()).Replace ("EXTRA_EXPORT", ""));
				var result = DotNet.AssertBuild (projectPath, properties);

				var registrarSource = Directory.GetFiles (objDir, "registrar.mm", SearchOption.AllDirectories).Single ();
				var assembliesObject = Directory.GetFiles (objDir, "registrar-assemblies.o", SearchOption.AllDirectories).Single ();
				var registrarObject = Directory.GetFiles (objDir, "registrar.o", SearchOption.AllDirectories).Single ();
				using var module = ModuleDefinition.ReadModule (appAssembly);
				var mvid = module.Mvid;
				Assert.That (Directory.GetFiles (objDir, "registrar-assemblies.mm", SearchOption.AllDirectories), Is.Empty, $"Version {version}: no assembly source to compile");
				Assert.That (File.ReadAllBytes (assembliesObject).AsSpan ().IndexOf (System.Text.Encoding.ASCII.GetBytes (mvid.ToString ())), Is.GreaterThanOrEqualTo (0), $"Version {version}: generated assembly object MVID");
				Assert.That (File.ReadAllText (registrarSource), Does.Not.Contain (mvid.ToString ()), $"Version {version}: stable registrar source");
				Assert.That (File.ReadAllBytes (nativeExecutable).AsSpan ().IndexOf (System.Text.Encoding.ASCII.GetBytes (mvid.ToString ())), Is.GreaterThanOrEqualTo (0), $"Version {version}: linked native MVID");
				ExecuteWithMagicWordAndAssert (platform, runtimeIdentifier, nativeExecutable, validationEnvironment);

				if (version == 1) {
					registrarContents = File.ReadAllText (registrarSource);
				} else {
					Assert.That (mvid, Is.Not.EqualTo (previousMvid), $"Version {version}: assembly MVID changed");
					Assert.That (File.ReadAllText (registrarSource), Is.EqualTo (registrarContents), $"Version {version}: registrar contents");
					Assert.That (File.GetLastWriteTimeUtc (registrarObject), Is.EqualTo (registrarObjectTimestamp), $"Version {version}: registrar object reused");
					Assert.That (File.GetLastWriteTimeUtc (assembliesObject), Is.GreaterThan (assembliesObjectTimestamp), $"Version {version}: assembly object regenerated");
					var targets = BinLog.GetAllTargets (result.BinLogPath);
					AssertTargetNotExecuted (targets, "_CompileNativeExecutable", $"Version {version}: no native compilation");
					AssertTargetExecuted (targets, "_LinkNativeExecutable", $"Version {version}: native relink");
				}

				previousMvid = mvid;
				registrarObjectTimestamp = File.GetLastWriteTimeUtc (registrarObject);
				assembliesObjectTimestamp = File.GetLastWriteTimeUtc (assembliesObject);
			}

			var unchanged = DotNet.AssertBuild (projectPath, properties);
			var currentAssembliesObject = Directory.GetFiles (objDir, "registrar-assemblies.o", SearchOption.AllDirectories).Single ();
			Assert.That (File.GetLastWriteTimeUtc (currentAssembliesObject), Is.EqualTo (assembliesObjectTimestamp), "Unchanged assembly object");
			AssertTargetNotExecuted (BinLog.GetAllTargets (unchanged.BinLogPath), "_CompileNativeExecutable", "Unchanged native sources");

			File.WriteAllText (sourcePath, source.Replace ("VERSION", "4").Replace ("EXTRA_EXPORT", "[Export (\"extraValue\")]\n\tpublic string ExtraValue () => \"extra\";"));
			DotNet.AssertBuild (projectPath, properties);
			var updatedRegistrarSource = Directory.GetFiles (objDir, "registrar.mm", SearchOption.AllDirectories).Single ();
			var updatedRegistrarObject = Directory.GetFiles (objDir, "registrar.o", SearchOption.AllDirectories).Single ();
			Assert.That (File.ReadAllText (updatedRegistrarSource), Does.Contain ("extraValue"), "New registered method");
			Assert.That (File.GetLastWriteTimeUtc (updatedRegistrarObject), Is.GreaterThan (registrarObjectTimestamp), "Registration change recompiles registrar");
			ExecuteWithMagicWordAndAssert (platform, runtimeIdentifier, nativeExecutable, validationEnvironment);

			DotNet.Execute ("clean", projectPath, properties);
			Assert.That (File.Exists (currentAssembliesObject), Is.False, "Clean removes generated assembly object");
		}

		IEnumerable<TypeDefinition> AllTypes (ModuleDefinition module)
		{
			foreach (var type in module.Types) {
				yield return type;
				foreach (var t in InnerTypes (type))
					yield return t;
			}
		}

		IEnumerable<TypeDefinition> InnerTypes (TypeDefinition type)
		{
			if (type.HasNestedTypes) {
				foreach (var t in type.NestedTypes) {
					yield return t;
					foreach (var nt in InnerTypes (t))
						yield return nt;
				}
			}
		}
	}
}
