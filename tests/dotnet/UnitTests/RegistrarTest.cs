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
			Configuration.AssertRuntimeIdentifiersAvailable (platform, runtimeIdentifiers);

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
			if (rewriteHandles)
				properties ["MtouchExtraArgs"] = "--optimize=redirect-class-handles";

			DotNet.AssertBuild (projectPath, properties);

			var appDir = GetAppPath (projectPath, platform, runtimeIdentifiers, configuration);
			var asmDir = Path.Combine (appDir, GetRelativeAssemblyDirectory (platform));

			var appExecutable = Path.Combine (asmDir, project + ".dll");
			var platformDll = Path.Combine (asmDir, Configuration.GetBaseLibraryName (platform, true));
			Assert.That (File.Exists (platformDll), "No platform dll.");
			var module = ModuleDefinition.ReadModule (platformDll);
			var classHandlesMaybe = AllTypes (module).FirstOrDefault (t => t.FullName == "ObjCRuntime.Runtime/ClassHandles");
			Assert.NotNull (classHandlesMaybe, "Couldn't find ClassHandles type.");
			var classHandles = classHandlesMaybe!;
			if (!rewriteHandles) {
				// NB: there is always at least one field named "unused"
				var fields = classHandles.Fields.Where (f => f.Name != "unused").Select (f => f.Name).ToList ();
				var sb = new StringBuilder ();
				foreach (var f in fields) {
					sb.Append (" ").Append (f);
				}
				Assert.That (fields.Count == 0, "There are fields in classHandles - rewriter was called when it should have done nothing." + sb );
			} else {
				// NB: there is always at least one field named "unused"
				Assert.That (classHandles.HasFields && classHandles.Fields.Count () > 1, "There are no fields in ClassHandles - rewriter did nothing.");
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
#endif
	}
}

