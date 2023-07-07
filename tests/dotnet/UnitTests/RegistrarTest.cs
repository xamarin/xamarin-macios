using Mono.Cecil;

namespace Xamarin.Tests {
	[TestFixture]
	public class RegistrarTest : TestBaseClass {
		[TestCase (ApplePlatform.MacCatalyst, true)]
		[TestCase (ApplePlatform.MacOSX, true)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.TVOS, false)]
		public void InvalidStaticRegistrarValidation (ApplePlatform platform, bool validated)
		{
			var project = "MyRegistrarApp";
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
				if (validated) {
					ExecuteProjectWithMagicWordAndAssert (projectPath, platform, runtimeIdentifiers);
				} else if (CanExecute (platform, runtimeIdentifiers)) {
					var rv = base.Execute (GetNativeExecutable (platform, appDir), out var output, out _);
					Assert.AreEqual (1, rv.ExitCode, "Expected no validation");
				}
			} finally {
				Environment.SetEnvironmentVariable ("XAMARIN_VALIDATE_STATIC_REGISTRAR_CODE", null);
			}
		}

#if NET
		[TestCase (ApplePlatform.MacCatalyst, false)]
		[TestCase (ApplePlatform.MacOSX, false)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.TVOS, false)]
// not currently passing
//		[TestCase (ApplePlatform.MacCatalyst, true)]
//		[TestCase (ApplePlatform.MacOSX, true)]
//		[TestCase (ApplePlatform.iOS, true)]
//		[TestCase (ApplePlatform.TVOS, true)]
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
			if (rewriteHandles)
				properties ["MtouchExtraArgs"] = "--optimize=redirect-class-handles";

			DotNet.AssertBuild (projectPath, properties);

			var appDir = GetAppPath (projectPath, platform, runtimeIdentifiers, configuration);
			var asmDir = Path.Combine (appDir, GetRelativeAssemblyDirectory (platform));

			var appExecutable = Path.Combine (asmDir, project + ".dll");
			var platformDll = Path.Combine (asmDir, GetPlatformDll (platform));
			Assert.That (File.Exists (platformDll), "No platform dll.");
			var module = ModuleDefinition.ReadModule (platformDll);
			var classHandlesMaybe = AllTypes (module).FirstOrDefault (t => t.FullName == "ObjCRuntime.Runtime/ClassHandles");
			Assert.NotNull (classHandlesMaybe, "Couldn't find ClassHandles type.");
			var classHandles = classHandlesMaybe!;
			if (!rewriteHandles) {
				Assert.That (!classHandles.HasFields, "There are fields in classHandles - rewriter was called when it should have done nothing.");
			} else {
				Assert.That (classHandles.HasFields, "There are no fields in ClassHandles - rewriter did nothing.");
				var field = classHandles.Fields.FirstOrDefault (f => f.Name.Contains ("SomeObj"));
				Assert.IsNotNull (field, "Didn't find a field for 'SomeObj'");
			}	
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

		static string GetPlatformDll (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
				return "Microsoft.iOS.dll";
			case ApplePlatform.TVOS:
				return "Microsoft.tvOS.dll";
			case ApplePlatform.WatchOS:
				return "Microsoft.WatchOS.dll";
			case ApplePlatform.MacOSX:
				return "Microsoft.macOS.dll";
			case ApplePlatform.MacCatalyst:
				return "Microsoft.MacCatalyst.dll";
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
		}
#endif
	}
}

