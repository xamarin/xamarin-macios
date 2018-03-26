using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Xamarin.Utils;

namespace Xamarin.MMP.Tests
{
	public class BindingProjectTests
	{
		static string RemoveCSProj (string s) => s.Remove (s.IndexOf (".csproj", StringComparison.InvariantCulture));
		
		static Tuple<string, string> BuildLinkedTestProjects (TI.UnifiedTestConfig binding, TI.UnifiedTestConfig project, string tmpDir)
		{
			binding.ItemGroup = NativeReferenceTests.CreateSingleNativeRef (Path.GetFullPath (NativeReferenceTests.SimpleDylibPath), "Dynamic");
			binding.StructsAndEnumsConfig = "public class UnifiedWithDepNativeRefLibTestClass {}";

			string projectPath = TI.GenerateBindingLibraryProject (binding);
			string bindingBuildLog = TI.BuildProject (projectPath, true);

			string bindingName = RemoveCSProj (binding.ProjectName);

			project.References = string.Format (@"<Reference Include=""{0}""><HintPath>{1}</HintPath></Reference>", bindingName, Path.Combine (tmpDir, "bin/Debug", bindingName + ".dll"));
			project.TestCode = "System.Console.WriteLine (typeof (ExampleBinding.UnifiedWithDepNativeRefLibTestClass));";

			string appBuildLog = TI.TestUnifiedExecutable (project).BuildOutput;

			return new Tuple<string, string> (bindingBuildLog, appBuildLog);
		}

		// There are a mess of different binding project configurations in the wild
		public enum ProjectType { 
			Modern, // The ideal Modern - Sets TargetFrameworkVersion and TargetFrameworkIdentifier correclty
			ModernNoTag, // Sets neither TargetFrameworkVersion nor TargetFrameworkIdentifier
			Full, // Sets both TargetFrameworkVersion and UseXamMacFullFramework
			FullTVF, // Sets just TargetFrameworkVersion to 4.5 or later
			FullXamMacTag, // Sets just UseXamMacFullFramework
		}

		Tuple <TI.UnifiedTestConfig, TI.UnifiedTestConfig> GenerateTestProject (ProjectType type, string tmpDir)
		{
			TI.UnifiedTestConfig binding = new TI.UnifiedTestConfig (tmpDir);
			TI.UnifiedTestConfig project = new TI.UnifiedTestConfig (tmpDir);
			switch (type) {
			case ProjectType.Modern:
				binding.ProjectName = "MobileBinding.csproj";
				project.XM45 = false;
				break;
			case ProjectType.ModernNoTag:
				binding.ProjectName = "BindingProjectWithNoTag.csproj";
				project.XM45 = false;
				break;
			case ProjectType.Full:
				binding.ProjectName = "XM45Binding.csproj";
				binding.CustomProjectReplacement = new Tuple<string, string> (
				"<TargetFrameworkVersion>v4.5</TargetFrameworkVersion>",
				"<UseXamMacFullFramework>true</UseXamMacFullFramework><TargetFrameworkVersion>4.5</TargetFrameworkVersion>");
				project.XM45 = true;
				break;
			case ProjectType.FullTVF:
				binding.ProjectName = "XM45Binding.csproj";
				project.XM45 = true;
				break;
			case ProjectType.FullXamMacTag:
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

		[TestCase (ProjectType.Modern)]
		[TestCase (ProjectType.ModernNoTag)]
		[TestCase (ProjectType.Full)]
		[TestCase (ProjectType.FullTVF)]
		[TestCase (ProjectType.FullXamMacTag)]
		public void ShouldRemovePackagedLibrary_OnceInBundle (ProjectType type)
		{
			MMPTests.RunMMPTest (tmpDir => {
				var projects = GenerateTestProject (type, tmpDir);
				BuildLinkedTestProjects (projects.Item1, projects.Item2, tmpDir);

				string bindingName = RemoveCSProj (projects.Item1.ProjectName);
				string appName = RemoveCSProj (projects.Item2.ProjectName);

				string libPath = Path.Combine (tmpDir, $"bin/Debug/{appName}.app/Contents/MonoBundle/{bindingName}.dll");
				Assert.True (File.Exists (libPath), $"Did not find expected library: {libPath}");
				string monoDisResults = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/monodis", "--presources " + libPath, "monodis");
				Assert.IsFalse (monoDisResults.Contains ("SimpleClassDylib.dylib"));
			});
		}

		[TestCase (ProjectType.Modern)]
		[TestCase (ProjectType.ModernNoTag)]
		[TestCase (ProjectType.Full)]
		[TestCase (ProjectType.FullTVF)]
		[TestCase (ProjectType.FullXamMacTag)]
		public void ShouldBuildWithoutErrors_AndLinkCorrectFramework (ProjectType type)
		{
			MMPTests.RunMMPTest (tmpDir => {
				var projects = GenerateTestProject (type, tmpDir);
				var logs = BuildLinkedTestProjects (projects.Item1, projects.Item2, tmpDir);

				var bgenInvocation = logs.Item1.SplitLines ().First (x => x.Contains ("bin/bgen"));
				var bgenParts = bgenInvocation.Split (new char[] { ' ' });
				var mscorlib = bgenParts.First (x => x.Contains ("mscorlib.dll"));
				var system = bgenParts.First (x => x.Contains ("System.dll"));

				switch (type) {
				case ProjectType.Modern:
				case ProjectType.ModernNoTag:
					Assert.True (mscorlib.EndsWith ("lib/mono/Xamarin.Mac/mscorlib.dll", StringComparison.Ordinal), "mscorlib not found in expected Modern location: " + mscorlib);
					Assert.True (system.EndsWith ("lib/mono/Xamarin.Mac/System.dll", StringComparison.Ordinal), "system not found in expected Modern location: " + system);
					break;
				case ProjectType.Full:
				case ProjectType.FullTVF:
				case ProjectType.FullXamMacTag:
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

		static string GetExpectedBCLVersion (ProjectType type)
		{
			switch (type) {
			case ProjectType.Modern:
			case ProjectType.ModernNoTag:
				return "2.0.5.0";
			case ProjectType.Full:
			case ProjectType.FullTVF:
			case ProjectType.FullXamMacTag:
				return "4.0.0.0";
			default:
				throw new NotImplementedException ();
			}
		}
	}
}
