using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Xamarin.Utils;

namespace Xamarin.MMP.Tests
{
	// There are a mess of different binding project configurations in the wild
	public enum BindingProjectType
	{
		Modern, // The ideal Modern - Sets TargetFrameworkVersion and TargetFrameworkIdentifier correclty
		ModernNoTag, // Sets neither TargetFrameworkVersion nor TargetFrameworkIdentifier
		Full, // Sets both TargetFrameworkVersion and UseXamMacFullFramework
		FullTVF, // Sets just TargetFrameworkVersion to 4.5 or later
		FullXamMacTag, // Sets just UseXamMacFullFramework
	}

	public class BindingProjectTests
	{
		internal static string RemoveCSProj (string s) => s.Remove (s.IndexOf (".csproj", StringComparison.InvariantCulture));

		internal static Tuple<string, string> SetupAndBuildLinkedTestProjects (TI.UnifiedTestConfig binding, TI.UnifiedTestConfig project, string tmpDir, bool useProjectReference, bool setupDefaultNativeReference)
		{
			string bindingBuildLog = SetupAndBuildBindingProject (binding, setupDefaultNativeReference);

			string bindingName = RemoveCSProj (binding.ProjectName);

			if (useProjectReference)
				project.References = $@"<ProjectReference Include=""{bindingName}.csproj"" />";
			else
				project.References = $@"<Reference Include=""{bindingName}""><HintPath>{Path.Combine (tmpDir, "bin/Debug", bindingName + ".dll")}</HintPath></Reference>";

			project.TestCode = "System.Console.WriteLine (typeof (ExampleBinding.UnifiedWithDepNativeRefLibTestClass));";

			string appBuildLog = TI.TestUnifiedExecutable (project).BuildOutput;

			return new Tuple<string, string> (bindingBuildLog, appBuildLog);
		}

		internal static string SetupAndBuildBindingProject (TI.UnifiedTestConfig binding, bool setupDefaultNativeReference)
		{
			if (setupDefaultNativeReference)
				binding.ItemGroup += NativeReferenceTests.CreateSingleNativeRef (Path.GetFullPath (NativeReferenceTests.SimpleDylibPath), "Dynamic");

			binding.StructsAndEnumsConfig = "public class UnifiedWithDepNativeRefLibTestClass {}";

			string projectPath = TI.GenerateBindingLibraryProject (binding);
			return TI.BuildProject (projectPath, true);
		}

		internal static Tuple <TI.UnifiedTestConfig, TI.UnifiedTestConfig> GenerateTestProject (BindingProjectType type, string tmpDir)
		{
			TI.UnifiedTestConfig binding = new TI.UnifiedTestConfig (tmpDir);
			TI.UnifiedTestConfig project = new TI.UnifiedTestConfig (tmpDir);
			switch (type) {
			case BindingProjectType.Modern:
				binding.ProjectName = "MobileBinding.csproj";
				project.XM45 = false;
				break;
			case BindingProjectType.ModernNoTag:
				binding.ProjectName = "BindingProjectWithNoTag.csproj";
				project.XM45 = false;
				break;
			case BindingProjectType.Full:
				binding.ProjectName = "XM45Binding.csproj";
				binding.CustomProjectReplacement = new Tuple<string, string> (
				"<TargetFrameworkVersion>v4.5</TargetFrameworkVersion>",
				"<UseXamMacFullFramework>true</UseXamMacFullFramework><TargetFrameworkVersion>4.5</TargetFrameworkVersion>");
				project.XM45 = true;
				break;
			case BindingProjectType.FullTVF:
				binding.ProjectName = "XM45Binding.csproj";
				project.XM45 = true;
				break;
			case BindingProjectType.FullXamMacTag:
				binding.ProjectName = "XM45Binding.csproj";
				binding.CustomProjectReplacement = new Tuple<string, string> (
					"<TargetFrameworkVersion>v4.5</TargetFrameworkVersion>", 
					"<UseXamMacFullFramework>true</UseXamMacFullFramework>");
				project.XM45 = true;
				break;	
			default:
				throw new NotImplementedException ();
			}
			return new Tuple<TI.UnifiedTestConfig, TI.UnifiedTestConfig> (binding, project);
		}

		internal static void SetNoEmbedding (TI.UnifiedTestConfig project)
		{
			project.CSProjConfig = "<NoBindingEmbedding>true</NoBindingEmbedding>";
		}

		[TestCase (BindingProjectType.Modern)]
		[TestCase (BindingProjectType.ModernNoTag)]
		[TestCase (BindingProjectType.Full)]
		[TestCase (BindingProjectType.FullTVF)]
		[TestCase (BindingProjectType.FullXamMacTag)]
		public void ShouldRemovePackagedLibrary_OnceInBundle (BindingProjectType type)
		{
			MMPTests.RunMMPTest (tmpDir => {
				var projects = GenerateTestProject (type, tmpDir);
				SetupAndBuildLinkedTestProjects (projects.Item1, projects.Item2, tmpDir, useProjectReference: false, setupDefaultNativeReference: false);

				string bindingName = RemoveCSProj (projects.Item1.ProjectName);
				string appName = RemoveCSProj (projects.Item2.ProjectName);

				string libPath = Path.Combine (tmpDir, $"bin/Debug/{appName}.app/Contents/MonoBundle/{bindingName}.dll");
				Assert.True (File.Exists (libPath), $"Did not find expected library: {libPath}");
				string monoDisResults = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/monodis", "--presources " + libPath, "monodis");
				Assert.IsFalse (monoDisResults.Contains ("SimpleClassDylib.dylib"));
			});
		}

		[TestCase (BindingProjectType.Modern)]
		[TestCase (BindingProjectType.Modern, true)]
		[TestCase (BindingProjectType.ModernNoTag)]
		[TestCase (BindingProjectType.Full)]
		[TestCase (BindingProjectType.Full, true)]
		[TestCase (BindingProjectType.FullTVF)]
		[TestCase (BindingProjectType.FullXamMacTag)]
		public void ShouldBuildWithoutErrors_AndLinkCorrectFramework (BindingProjectType type, bool noEmbedding = false)
		{
			MMPTests.RunMMPTest (tmpDir => {
				var projects = GenerateTestProject (type, tmpDir);
				if (noEmbedding)
					SetNoEmbedding (projects.Item1);
				var logs = SetupAndBuildLinkedTestProjects (projects.Item1, projects.Item2, tmpDir, useProjectReference: false, setupDefaultNativeReference: false);

				Assert.False (logs.Item1.Contains ("mcs"), "Bindings project must not use mcs:\n" + logs.Item1);
				Assert.True (logs.Item1.Contains ("csc"), "Bindings project must use csc:\n" + logs.Item1); 

				var bgenInvocation = logs.Item1.SplitLines ().First (x => x.Contains ("bin/bgen"));
				var bgenParts = bgenInvocation.Split (new char[] { ' ' });
				var mscorlib = bgenParts.First (x => x.Contains ("mscorlib.dll"));
				var system = bgenParts.First (x => x.Contains ("System.dll"));

				switch (type) {
				case BindingProjectType.Modern:
				case BindingProjectType.ModernNoTag:
					Assert.True (mscorlib.EndsWith ("lib/mono/Xamarin.Mac/mscorlib.dll", StringComparison.Ordinal), "mscorlib not found in expected Modern location: " + mscorlib);
					Assert.True (system.EndsWith ("lib/mono/Xamarin.Mac/System.dll", StringComparison.Ordinal), "system not found in expected Modern location: " + system);
					break;
				case BindingProjectType.Full:
				case BindingProjectType.FullTVF:
				case BindingProjectType.FullXamMacTag:
					Assert.True (mscorlib.EndsWith ("lib/mono/4.5/mscorlib.dll", StringComparison.Ordinal), "mscorlib not found in expected Full location: " + mscorlib);
					Assert.True (system.EndsWith ("lib/mono/4.5/System.dll", StringComparison.Ordinal), "system not found in expected Full location: " + system);
					break;
				default:
					throw new NotImplementedException ();
				}

				Assert.False (logs.Item1.Contains ("CS1685"), "Binding should not contains CS1685 multiple definition warning:\n" + logs.Item1);

				string bindingName = RemoveCSProj (projects.Item1.ProjectName);
				string appName = RemoveCSProj (projects.Item2.ProjectName);
				string libPath = Path.Combine (tmpDir, $"bin/Debug/{appName}.app/Contents/MonoBundle/{bindingName}.dll");

				Assert.True (File.Exists (libPath));
				string results = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/monop", "--refs -r:" + libPath, "monop");
				string mscorlibLine = results.Split (new char[] { '\n' }).First (x => x.Contains ("mscorlib"));

				string expectedVersion = GetExpectedBCLVersion (type);
				Assert.True (mscorlibLine.Contains (expectedVersion), $"{mscorlibLine} did not contain expected version {expectedVersion}");
			});
		}

		static string GetExpectedBCLVersion (BindingProjectType type)
		{
			switch (type) {
			case BindingProjectType.Modern:
			case BindingProjectType.ModernNoTag:
				return "2.0.5.0";
			case BindingProjectType.Full:
			case BindingProjectType.FullTVF:
			case BindingProjectType.FullXamMacTag:
				return "4.0.0.0";
			default:
				throw new NotImplementedException ();
			}
		}
	}
}
