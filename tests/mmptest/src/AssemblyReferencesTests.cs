using System;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Xamarin.MMP.Tests {
	[TestFixture]
	public class AssemblyReferencesTests {
		[Test]
		public void ShouldNotAllowReference_ToSystemDrawing ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					References = " <Reference Include=\"System.Drawing\" />",
					TestCode = "System.Drawing.RectangleF f = new System.Drawing.RectangleF ();",
					XM45 = true
				};
				TI.TestUnifiedExecutable (test);
				var allAssembliesInBundle = Directory.GetFiles (test.BundlePath, "*.dll", SearchOption.AllDirectories).Select (Path.GetFileName);
				Assert.That (allAssembliesInBundle, Does.Contain ("mscorlib.dll"), "mscorlib.dll");
				Assert.That (allAssembliesInBundle, Does.Contain ("System.Drawing.Common.dll"), "System.Drawing.Common.dll");
				Assert.That (allAssembliesInBundle, Does.Not.Contain ("System.Drawing.dll"), "System.Drawing.dll");
			});
		}

		[TestCase (false)]
		[TestCase (true)]
		public void ShouldAllowReference_ToOpenTK (bool full)
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					References = " <Reference Include=\"OpenTK\" />",
					TestCode = "var matrix = new OpenTK.Matrix2 ();",
					XM45 = full
				};
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void AllowsUnresolvableReferences ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				string [] sb;

				// build b.dll
				sb = new [] { "-target:library", $"-out:{tmpDir}/b.dll", $"{tmpDir}/b.cs" };
				File.WriteAllText (Path.Combine (tmpDir, "b.cs"), "public class B { }");
				TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/csc", sb, "b");

				// build a.dll
				sb = new [] { "-target:library", $"-out:{tmpDir}/a.dll", $"{tmpDir}/a.cs", $"-r:{tmpDir}/b.dll" };
				File.WriteAllText (Path.Combine (tmpDir, "a.cs"), "public class A { public A () { System.Console.WriteLine (typeof (B)); }}");
				TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/csc", sb, "a");

				File.Delete (Path.Combine (tmpDir, "b.dll"));

				// build project referencing a.dll
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					References = string.Format (" <Reference Include=\"a\" > <HintPath>{0}/a.dll</HintPath> </Reference> ", tmpDir),
					TestCode = "System.Console.WriteLine (typeof (A));",
				};
				TI.GenerateAndBuildUnifiedExecutable (test);
			});
		}

		[TestCase (false)]
		[TestCase (true)]
		public void UnsafeGACResolutionOptions_AllowsWindowsBaseResolution (bool full)
		{
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					XM45 = full,
					TestCode = "System.Console.WriteLine (typeof (System.Windows.DependencyObject));",
					References = "<Reference Include=\"WindowsBase\" /><Reference Include=\"System.Xaml\" />"
				};

				TI.TestUnifiedExecutable (test, shouldFail: true);

				// Modern will fail terribly due to mismatch BCL, but Full should allow if we ask "nicely"
				if (full) {
					test.CSProjConfig = "<MonoBundlingExtraArgs>--allow-unsafe-gac-resolution</MonoBundlingExtraArgs>";
					TI.TestUnifiedExecutable (test);
				}
			});
		}
	}
}
