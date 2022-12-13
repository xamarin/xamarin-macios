using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Xamarin.Utils;

namespace Xamarin.MMP.Tests {
	// There are a mess of different binding project configurations in the wild
	public enum BindingProjectType {
		Modern, // The ideal Modern - Sets TargetFrameworkVersion and TargetFrameworkIdentifier correclty
		ModernNoTag, // Sets neither TargetFrameworkVersion nor TargetFrameworkIdentifier
		Full, // Sets both TargetFrameworkVersion and UseXamMacFullFramework
		FullTVF, // Sets just TargetFrameworkVersion to 4.5 or later
		FullXamMacTag, // Sets just UseXamMacFullFramework
	}

	public class BindingProjectTests {
		internal static string RemoveCSProj (string s) => s.Remove (s.IndexOf (".csproj", StringComparison.InvariantCulture));

		internal static (BuildResult BindingBuildResult, OutputText AppTestResult) SetupAndBuildLinkedTestProjects (TI.UnifiedTestConfig binding, TI.UnifiedTestConfig project, string tmpDir, bool useProjectReference, bool setupDefaultNativeReference)
		{
			var bindingBuildLog = SetupAndBuildBindingProject (binding, setupDefaultNativeReference);

			string bindingName = RemoveCSProj (binding.ProjectName);

			if (useProjectReference)
				project.References = $@"<ProjectReference Include=""{bindingName}.csproj"" />";
			else
				project.References = $@"<Reference Include=""{bindingName}""><HintPath>{Path.Combine (tmpDir, "bin/Debug", bindingName + ".dll")}</HintPath></Reference>";

			project.TestCode = "System.Console.WriteLine (typeof (ExampleBinding.UnifiedWithDepNativeRefLibTestClass));";

			var appBuildLog = TI.TestUnifiedExecutable (project);

			(BuildResult BindingBuildResult, OutputText AppTestResult) rv = (bindingBuildLog, appBuildLog);
			return rv;
		}

		internal static BuildResult SetupAndBuildBindingProject (TI.UnifiedTestConfig binding, bool setupDefaultNativeReference, bool shouldFail = false)
		{
			if (setupDefaultNativeReference)
				binding.ItemGroup += NativeReferenceTests.CreateSingleNativeRef (Path.GetFullPath (NativeReferenceTests.SimpleDylibPath), "Dynamic");

			binding.StructsAndEnumsConfig = "public class UnifiedWithDepNativeRefLibTestClass {}";

			string projectPath = TI.GenerateBindingLibraryProject (binding);
			return TI.BuildProject (projectPath, shouldFail: shouldFail);
		}

		internal static Tuple<TI.UnifiedTestConfig, TI.UnifiedTestConfig> GenerateTestProject (BindingProjectType type, string tmpDir)
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
				string monoDisResults = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/monodis", new [] { "--presources", libPath }, "monodis");
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

				var logs = SetupAndBuildLinkedTestProjects (projects.Item1, projects.Item2, tmpDir, useProjectReference: false, setupDefaultNativeReference: noEmbedding);

				Assert.True (logs.BindingBuildResult.BuildOutput.Contains ("csc"), "Bindings project must use csc:\n" + logs.Item1);

				var bgenInvocation = logs.BindingBuildResult.BuildOutputLines.First (x => x.Contains ("bin/bgen"));
				Assert.IsTrue (StringUtils.TryParseArguments (bgenInvocation, out var bgenArguments, out var _), "Parse bgen arguments");
				// unfurl any response files
				var bgenParts = bgenArguments.ToList ();
				var responseFiles = bgenParts.Where (v => v [0] == '@').ToArray ();
				bgenParts.RemoveAll (v => v [0] == '@');
				foreach (var rsp in responseFiles) {
					Assert.IsTrue (StringUtils.TryParseArguments (File.ReadAllText (rsp.Substring (1)).Replace ('\n', ' '), out var args, out var _), "Parse response file");
					bgenParts.AddRange (args);
				}
				var mscorlib = bgenParts.First (x => x.Contains ("mscorlib.dll"));
				var system = bgenParts.First (x => x.Contains ("System.dll"));

				switch (type) {
				case BindingProjectType.Modern:
				case BindingProjectType.ModernNoTag:
					Assert.That (mscorlib, Does.EndWith ("lib/mono/Xamarin.Mac/mscorlib.dll"), "mscorlib not found in expected Modern location: " + mscorlib);
					Assert.That (system, Does.EndWith ("lib/mono/Xamarin.Mac/System.dll"), "system not found in expected Modern location: " + system);
					break;
				case BindingProjectType.Full:
				case BindingProjectType.FullTVF:
				case BindingProjectType.FullXamMacTag:
					Assert.That (mscorlib, Does.EndWith ("lib/mono/4.5/mscorlib.dll"), "mscorlib not found in expected Full location: " + mscorlib);
					Assert.That (system, Does.EndWith ("lib/mono/4.5/System.dll"), "system not found in expected Full location: " + system);
					break;
				default:
					throw new NotImplementedException ();
				}

				Assert.False (logs.BindingBuildResult.BuildOutput.Contains ("CS1685"), "Binding should not contains CS1685 multiple definition warning.");

				Assert.False (logs.BindingBuildResult.BuildOutput.Contains ("MSB9004"), "Binding should not contains MSB9004 warning");

				string bindingName = RemoveCSProj (projects.Item1.ProjectName);
				string appName = RemoveCSProj (projects.Item2.ProjectName);
				string libPath = Path.Combine (tmpDir, $"bin/Debug/{appName}.app/Contents/MonoBundle/{bindingName}.dll");

				Assert.True (File.Exists (libPath));
				string results = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/monop", new [] { "--refs", "-r:" + libPath }, "monop");
				string mscorlibLine = results.Split (new char [] { '\n' }).First (x => x.Contains ("mscorlib"));

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
