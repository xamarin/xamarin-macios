using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Xamarin.MMP.Tests
{
	public class BindingProjectTests
	{
		static string BindingName (bool full) => full ? "XM45Binding" : "MobileBinding";
		
		static void BuildLinkedTestProjects (string tmpDir, bool full = false, bool removeTFI = false)
		{
			string bindingName = BindingName (full);
			TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
				ProjectName = bindingName + ".csproj",
				ItemGroup = MMPTests.CreateSingleNativeRef (Path.GetFullPath (MMPTests.SimpleDylibPath), "Dynamic"),
				StructsAndEnumsConfig = "public class UnifiedWithDepNativeRefLibTestClass {}"
			};

			if (removeTFI)
				test.CustomProjectReplacement = new Tuple<string, string> (@"<TargetFrameworkVersion>v4.5</TargetFrameworkVersion>", "");

			string projectPath = TI.GenerateBindingLibraryProject (test);
			TI.BuildProject (projectPath, true);

			string referenceCode = string.Format (@"<Reference Include=""{0}""><HintPath>{1}</HintPath></Reference>", bindingName, Path.Combine (tmpDir, "bin/Debug", bindingName + ".dll"));

			test = new TI.UnifiedTestConfig (tmpDir) {
				References = referenceCode,
				TestCode = "System.Console.WriteLine (typeof (ExampleBinding.UnifiedWithDepNativeRefLibTestClass));",
				XM45 = full					
			};
			TI.TestUnifiedExecutable (test);
		}

		[Test]
		public void ShouldRemovePackagedLibrary_OnceInBundle ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				BuildLinkedTestProjects (tmpDir);

				string libPath = Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle/MobileBinding.dll");
				Assert.True (File.Exists (libPath));
				string monoDisResults = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/monodis", "--presources " + libPath, "monodis");
				Assert.IsFalse (monoDisResults.Contains ("SimpleClassDylib.dylib"));
			});
		}

		[TestCase (false, false)]
		[TestCase (true, false)]
		[TestCase (true, true)]
		public void ShouldBuildWithoutErrors_AndLinkCorrectFramework (bool full, bool removeTFI)
		{
			MMPTests.RunMMPTest (tmpDir => {
				BuildLinkedTestProjects (tmpDir, full, removeTFI);

				string libPath = Path.Combine (tmpDir, $"bin/Debug/{(full ? "XM45Example.app" : "UnifiedExample.app")}/Contents/MonoBundle/{BindingName (full)}.dll");
				Assert.True (File.Exists (libPath));
				string results = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/monop", "--refs -r:" + libPath, "monop");
				string mscorlibLine = results.Split (new char[] { '\n' }).First (x => x.Contains ("mscorlib"));

				string expectedVersion = full ? "4.0.0.0" : "2.0.5.0";
				Assert.True (mscorlibLine.Contains (expectedVersion), $"{mscorlibLine} did not contain expected version {expectedVersion}");
			});
		}
	}
}
