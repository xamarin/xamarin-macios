using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

using Xamarin.Utils;
using Xamarin.Tests;

namespace Xamarin.MMP.Tests {
	[TestFixture]
	public partial class MMPTests {
		public static void RunMMPTest (Action<string> test, string directoryName = null)
		{
			test (Cache.CreateTemporaryDirectory (directoryName));
		}

		// TODO - We have multiple tests using this. It doesn't take that long, but is it worth caching?
		public static string [] GetUnifiedProjectClangInvocation (string tmpDir, string projectConfig = "")
		{
			TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = projectConfig };
			var splitBuildOutput = TI.TestUnifiedExecutable (test).BuildResult.BuildOutputLines;
			string clangInvocation = splitBuildOutput.Single (x => x.Contains ("usr/bin/clang") && x.Contains ("mmacosx-version-min"));
			return clangInvocation.Split (new string [] { " " }, StringSplitOptions.None);
		}

		[Test]
		public void CollisionsBetweenLibraryNameAndEXE_ShouldFailBuild ()
		{
			RunMMPTest (tmpDir => {
				Action<string, string, bool> testCore = (projectName, assemblyName, XM45) => {
					// Build a library with the conflicting name
					TI.UnifiedTestConfig libConfig = new TI.UnifiedTestConfig (tmpDir) { XM45 = XM45, ProjectName = projectName, AssemblyName = assemblyName };
					string csprojTarget = TI.GenerateUnifiedLibraryProject (libConfig);
					TI.BuildProject (csprojTarget);

					// Build an exe using that library, it should fail the build
					string referenceCode = string.Format (@"<Reference Include=""TestApp""><HintPath>{0}</HintPath></Reference>", Path.Combine (tmpDir, "bin/Debug", assemblyName + ".dll"));

					TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = XM45, References = referenceCode };
					TI.TestUnifiedExecutable (test, shouldFail: true);
				};
				// These library assembly names conflict with the exe name
				testCore ("UnifiedLibrary", "UnifiedExample", false);
				testCore ("XM45Library", "XM45Example", true);
			});
		}

		[Test]
		public void CollisionBetweenEXEAndSDKAssembly_ShouldFailBuild ()
		{
			RunMMPTest (tmpDir => {
				Action<string, bool> testCore = (assemblyName, XM45) => {
					// Build a library with the conflicting name
					TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = XM45, AssemblyName = assemblyName };
					TI.TestUnifiedExecutable (test, shouldFail: true);
				};
				// These library assembly names conflict with the exe name
				testCore ("Xamarin.Mac", false);
				testCore ("Xamarin.Mac", true);

				testCore ("System.Xml", false);
				testCore ("System.Xml", true);
			});
		}

		[Test]
		public void XM_45_NotAddingIncorrectDependencies_LicenseTest ()
		{
			RunMMPTest (tmpDir => {
				TI.TestUnifiedExecutable (new TI.UnifiedTestConfig (tmpDir) { XM45 = true });

				// XM 4.5 projects were accidently pulling in every assembly in the 4.5 folder. Assert that isn't happening again.
				string monoBundlePath = Path.Combine (tmpDir, "bin/Debug/XM45Example.app/Contents/MonoBundle/");
				Assert.IsFalse (Directory.GetFiles (monoBundlePath).Any (x => x.Contains ("FSharp.Core.dll")), "F# was pulled in?");
			});
		}

		[Test]
		public void Mobile_NewRefCount_Warns ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<MonoBundlingExtraArgs>--new-refcount=false</MonoBundlingExtraArgs>" };
				var testResult = TI.TestUnifiedExecutable (test);
				testResult.Messages.AssertWarning (80, "Disabling the new refcount logic is deprecated.");
			});
		}

		[Test]
		public void Mobile_Verbose_Test ()
		{
			RunMMPTest (tmpDir => {
				// Just make sure we accept the option and it doesn't crash
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<MonoBundlingExtraArgs>-v -v</MonoBundlingExtraArgs>" };
				TI.TestUnifiedExecutable (test);
			});
		}

		// The check for name collisions between the root executable and other assemblies was mistakenly comparing against the entire path, not just the assembly name.
		// This meant that an assembly located in a folder/subfolder with a name matching the root executable name would incorrectly report as an error.
		[Test]
		public void FilePathCollisionShouldNotFailBuild ()
		{
			RunMMPTest (tmpDir => {
				var libraryDirectory = tmpDir + "/UnifiedExample/";
				var libraryName = "UnifiedLibrary";
				Directory.CreateDirectory (libraryDirectory);

				TI.UnifiedTestConfig libConfig = new TI.UnifiedTestConfig (libraryDirectory) { ProjectName = libraryName };
				string csprojTarget = TI.GenerateUnifiedLibraryProject (libConfig);

				TI.BuildProject (csprojTarget);

				string referenceCode = string.Format (@"<Reference Include=""UnifiedLibrary""><HintPath>{0}</HintPath></Reference>", Path.Combine (libraryDirectory, "bin/Debug/", $"{libraryName}.dll"));

				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { References = referenceCode };
				TI.TestUnifiedExecutable (test);
			});
		}

		[TestCase (false, false)]
		[TestCase (false, true)]
		[TestCase (true, false)]
		[TestCase (true, true)]
		public void Unified_HelloWorld_ShouldHaveNoWarnings (bool release, bool full)
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				test.Release = release;
				test.XM45 = full;

				var rv = TI.TestUnifiedExecutable (test);
				Console.WriteLine (rv.BuildResult);
				if (full && release) {
					rv.Messages.AssertWarning (5220, "Skipping framework 'QTKit'. It is prohibited (rejected) by the Mac App Store");
					// We get the MM5220 twice in the output, once from mmp and once from msbuild repeating what mmp said, so we can't assert that there's exactly 1 warning.
					// Instead assert that we get no warning but MM5220.
					Assert.IsFalse (rv.Messages.Messages.Any ((v) => v.Number != 5220), "No warnings except MM5220");
				} else {
					rv.Messages.AssertWarningCount (0);
				}
			});
		}

		[Test]
		public void Unified_HelloWorld_ShouldHaveNoRegistrarWarnings ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				// Mobile
				string output = TI.TestUnifiedExecutable (test).RunOutput;
				Assert.IsTrue (!output.Contains ("Could not register the assembly"), "Unified_HelloWorld_ShouldHaveNoRegistrarWarnings - Mobile had registrar issues: \n" + output);

				// XM45
				test.XM45 = true;
				output = TI.TestUnifiedExecutable (test).RunOutput;
				Assert.IsTrue (!output.Contains ("Could not register the assembly"), "Unified_HelloWorld_ShouldHaveNoRegistrarWarnings - XM45 had registrar issues: \n" + output);
			});
		}

		[Test]
		public void BuildUnifiedMobile_Program_WithNonASCIIName ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { AssemblyName = "piñata" };
				TI.TestUnifiedExecutable (test);

				test = new TI.UnifiedTestConfig (tmpDir) { AssemblyName = "你好世界" };
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void BuildUnifiedMobile_Program_WithSpace ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { AssemblyName = "Test With Space" };
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void Dontlink_Allow_ReadonlyAssembly ()
		{
			string [] sb;
			RunMMPTest (tmpDir => {
				// build b.dll
				string assemblyPath = string.Format ("{0}/b.dll", tmpDir);
				sb = new [] { "-target:library", "-debug", $"-out:{assemblyPath}", $"{tmpDir}/b.cs" };
				File.WriteAllText (Path.Combine (tmpDir, "b.cs"), "public class B { }");
				TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/csc", sb, "b");

				File.SetAttributes (assemblyPath, FileAttributes.ReadOnly);
				File.SetAttributes (Path.ChangeExtension (assemblyPath, ".pdb"), FileAttributes.ReadOnly);

				// build project referencing a.dll
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					References = string.Format (" <Reference Include=\"b\" > <HintPath>{0}</HintPath> </Reference> ", assemblyPath),
					TestCode = "System.Console.WriteLine (typeof (B));",
				};
				TI.GenerateAndBuildUnifiedExecutable (test, shouldFail: false);

				test.CSProjConfig = "<LinkMode>SdkOnly</LinkMode>";
				TI.GenerateAndBuildUnifiedExecutable (test, shouldFail: false);
			});
		}

		[Test]
		public void DefaultProject_ShouldPullInMonoPosix_AndNaitveLib ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = true };
				MonoPosixTestCore (tmpDir, test);
				test.XM45 = false;
				MonoPosixTestCore (tmpDir, test);
			});
		}

		static void MonoPosixTestCore (string tmpDir, TI.UnifiedTestConfig test)
		{
			TI.TestUnifiedExecutable (test, shouldFail: false);

			Assert.IsTrue (File.Exists (Path.Combine (tmpDir, "bin/Debug/XM45Example.app/Contents/MonoBundle/libMonoPosixHelper.dylib")), String.Format ("Does {0}/bin/Debug/XM45Example.app/Contents/MonoBundle/libMonoPosixHelper.dylib to exist?", tmpDir));
		}


		const string machineConfigMobileLocation = "bin/Debug/UnifiedExample.app/Contents/MonoBundle/mono/4.5/machine.config";
		const string machineConfigXM45Location = "bin/Debug/XM45Example.app/Contents/MonoBundle/mono/4.5/machine.config";

		[Test]
		public void Unified_ShouldNotGenerateMachineConfigInBundle_WithoutOption ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				// Mobile
				TI.TestUnifiedExecutable (test);

				Assert.IsFalse (File.Exists (Path.Combine (tmpDir, machineConfigMobileLocation)));

				// XM45
				test.XM45 = true;
				TI.TestUnifiedExecutable (test);

				Assert.IsFalse (File.Exists (Path.Combine (tmpDir, machineConfigXM45Location)));
			});
		}

		[Test]
		public void Unified_InvalidMachineConfigInBundle_ThrowsError ()
		{
			RunMMPTest (tmpDir => {
				string invalidConfigPath = Path.Combine (tmpDir, "nonexistant/machine.config");
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = string.Format ("<MonoBundlingExtraArgs>--machine-config={0}</MonoBundlingExtraArgs>", invalidConfigPath)
				};
				var testResult = TI.TestUnifiedExecutable (test, shouldFail: true);
				testResult.Messages.AssertError (97, $"machine.config file '{invalidConfigPath}' can not be found.");
			});
		}

		[Test]
		public void Unified_SgenConcurrent_Test ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<EnableSGenConc>true</EnableSGenConc>"
				};
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void Unified_ShouldGenerateMachineConfigInBundle_WithEmptyOption ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<MonoBundlingExtraArgs>--machine-config=\"\"</MonoBundlingExtraArgs>"
				};
				TI.TestUnifiedExecutable (test);
				Assert.IsTrue (File.Exists (Path.Combine (tmpDir, machineConfigMobileLocation)));

				test.XM45 = true;
				TI.TestUnifiedExecutable (test);
				Assert.IsTrue (File.Exists (Path.Combine (tmpDir, machineConfigXM45Location)));
			});
		}

		[Test]
		public void Unified_ShouldGenerateMachineConfigInBundle_WhenPassedIn ()
		{
			RunMMPTest (tmpDir => {
				const string configText = "THIS_IS_NOT_A_REAL_CONFIG_FILE";
				string configPath = Path.Combine (tmpDir, "machine.config");
				File.WriteAllLines (configPath, new string [] { configText });

				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = string.Format ("<MonoBundlingExtraArgs>--machine-config={0}</MonoBundlingExtraArgs>", configPath)
				};

				// Mobile
				TI.TestUnifiedExecutable (test);

				Assert.IsTrue (File.Exists (Path.Combine (tmpDir, machineConfigMobileLocation)));
				string [] text = File.ReadAllLines (Path.Combine (tmpDir, machineConfigMobileLocation));
				Assert.IsTrue (text.Length == 1 && text [0] == configText);

				// XM45
				test.XM45 = true;
				TI.TestUnifiedExecutable (test);

				Assert.IsTrue (File.Exists (Path.Combine (tmpDir, machineConfigXM45Location)));
				text = File.ReadAllLines (Path.Combine (tmpDir, machineConfigXM45Location));
				Assert.IsTrue (text.Length == 1 && text [0] == configText);
			});
		}

		[Test]
		public void Unified_FailedBuild_ShouldRequireAnotherBuildNotSkipMMP ()
		{
			RunMMPTest (tmpDir => {
				foreach (bool xm45 in new bool [] { false, true }) {
					// First build with a Non-existant file to force us to error inside mmp test
					TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<MonoBundlingExtraArgs>--resource=Foo.bar</MonoBundlingExtraArgs>", XM45 = xm45 };
					TI.GenerateAndBuildUnifiedExecutable (test, shouldFail: true);

					// Next, build again without the error MonoBundlingExtraArgs
					test.CSProjConfig = "";
					TI.GenerateUnifiedExecutableProject (test);

					// And try again. 
					// If we fail, we'll likely fail with "did not generate an exe" before returning but let's check anyway
					var secondBuildResult = TI.BuildProject (Path.Combine (tmpDir, TI.GetUnifiedExecutableProjectName (test)));
					var secondBuildOutput = secondBuildResult.BuildOutput;
					Assert.IsTrue (!secondBuildOutput.Contains ("Skipping target \"_CompileToNative"), "Did not skip");
					Assert.IsTrue (secondBuildOutput.Contains ("Building target \"_CompileToNative\" completely"), "Did need to build");
				}
			});
		}

		public const string BundleResourceTemplate = "<ItemGroup><BundleResource Include=\"{0}\" /></ItemGroup>";

		[Test]
		public void UnifiedWithDepLib_ThatContainsUserResource_ShouldBeRemovedUnderFullLink ()
		{
			RunMMPTest (tmpDir => {
				string resoucePath = Path.Combine (tmpDir, "foo.xml");
				File.Create (resoucePath);

				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					ProjectName = "UnifiedLibrary",
					ItemGroup = string.Format (BundleResourceTemplate, resoucePath),
				};
				var libProject = TI.GenerateUnifiedLibraryProject (test);
				TI.BuildProject (libProject);

				string referenceCode = string.Format (@"<Reference Include=""UnifiedLibrary""><HintPath>{0}</HintPath></Reference>", Path.Combine (tmpDir, "bin/Debug", "UnifiedLibrary.dll"));

				test = new TI.UnifiedTestConfig (tmpDir) { References = referenceCode, TestCode = "System.Console.WriteLine (typeof (Library.MyClass));" };
				TI.TestUnifiedExecutable (test);

				string libPath = Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle/UnifiedLibrary.dll");
				Assert.True (File.Exists (libPath));
				string monoDisResults = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/monodis", new [] { "--presources", libPath }, "monodis");
				Assert.IsFalse (monoDisResults.Contains ("foo.xml"));
			});
		}

		[Test]
		public void Unified_SideBySideXamMac_ConsoleTest ()
		{
			RunMMPTest (tmpDir => {
				string testPath = Path.Combine (TI.FindSourceDirectory (), @"ConsoleXMApp.csproj");
				TI.BuildProject (testPath);
				string exePath = Path.Combine (TI.FindSourceDirectory (), @"bin/Debug/ConsoleXMApp.exe");
				var output = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/mono64", new [] { exePath }, "RunSideBySizeXamMac");
				Assert.IsTrue (output.Split (Environment.NewLine.ToCharArray ()).Any (x => x.Contains ("True")), "Unified_SideBySideXamMac_ConsoleTest run");
			});
		}

		[TestCase (false)]
		[TestCase (true)]
		public void UnifiedDebugBuilds_ShouldLinkToPartialStatic_UnlessDisabled_Release (bool xm45)
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = xm45 };
				test.Release = true;
				var testResults = TI.TestUnifiedExecutable (test);
				Assert.IsFalse (testResults.BuildResult.BuildOutput.Contains ("Xamarin.Mac.registrar"), "Release build should not use partial static registrar");
			});
		}

		[TestCase (false)]
		[TestCase (true)]
		public void UnifiedDebugBuilds_ShouldLinkToPartialStatic_UnlessDisabled_Debug (bool xm45)
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = xm45 };
				test.Release = false;
				test.CSProjConfig = "<DebugSymbols>true</DebugSymbols>";
				var testResults = TI.TestUnifiedExecutable (test);
				Assert.IsTrue (testResults.BuildResult.BuildOutput.Contains ("Xamarin.Mac.registrar"), "Debug build should use partial static registrar");
			});
		}

		[TestCase (false)]
		[TestCase (true)]
		public void UnifiedDebugBuilds_ShouldLinkToPartialStatic_UnlessDisabled_Debug_dynamic (bool xm45)
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = xm45 };
				test.Release = false;
				test.CSProjConfig = "<DebugSymbols>true</DebugSymbols><MonoBundlingExtraArgs>--registrar=dynamic</MonoBundlingExtraArgs><XamMacArch>x86_64</XamMacArch>";
				var testResult = TI.TestUnifiedExecutable (test);
				Assert.IsFalse (testResult.BuildResult.BuildOutput.Contains ("Xamarin.Mac.registrar"), "registrar=dynamic build should not use partial static registrar");
			});
		}

		[TestCase (false)]
		[TestCase (true)]
		public void UnifiedDebugBuilds_ShouldLinkToPartialStatic_UnlessDisabled_Debug_partial (bool xm45)
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = xm45 };
				test.Release = false;
				test.CSProjConfig = "<DebugSymbols>true</DebugSymbols><MonoBundlingExtraArgs>--registrar=partial</MonoBundlingExtraArgs><XamMacArch>x86_64</XamMacArch>";
				var testResults = TI.TestUnifiedExecutable (test);
				Assert.IsTrue (testResults.BuildResult.BuildOutput.Contains ("Xamarin.Mac.registrar"), "registrar=partial build should use partial static registrar");
			});
		}
		//https://testrail.xamarin.com/index.php?/cases/view/234141&group_by=cases:section_id&group_order=asc&group_id=51097
		[Test]
		public void Unified45Build_CompileToNativeOutput ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = true };
				var output = TI.TestUnifiedExecutable (test);
				Assert.That (output.BuildResult.BuildOutput, Contains.Substring ("Selected target framework: Xamarin.Mac,Version=v4.5,Profile=Full; API: Unified"));
			});
		}

		[Test]
		public void UnifiedDebug_ShouldOnlyHaveOne_ObjCArgument ()
		{
			RunMMPTest (tmpDir => {
				string [] clangParts = GetUnifiedProjectClangInvocation (tmpDir);
				int objcCount = clangParts.Count (x => x.Contains ("-ObjC"));
				Assert.AreEqual (1, objcCount, "Found more than one -OjbC");
			});
		}

		[Test]
		[TestCase ("CFNetworkHandler", "CFNetworkHandler")]
		[TestCase ("NSUrlSessionHandler", "NSUrlSessionHandler")]
		[TestCase ("HttpClientHandler", "HttpClientHandler")]
		[TestCase (null, "HttpClientHandler")]
		[TestCase ("", "HttpClientHandler")]
		public void HttpClientHandler (string mmpHandler, string expectedHandler)
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					References = " <Reference Include=\"System.Net.Http\" />",
					TestCode = $@"
			var client = new System.Net.Http.HttpClient ();
			var field = client.GetType ().BaseType.GetField (""_handler"", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if (field is null)
				throw new System.Exception (""Could not find the field '_handler' in HttpClient's base type (which should be 'HttpMessageInvoker')."");
			var fieldValue = field.GetValue (client);
			if (fieldValue is null)
				throw new System.Exception (""Unexpected null value found in 'HttpMessageInvoker.handler' field."");
			var fieldValueType = fieldValue.GetType ().Name;
			if (fieldValueType != ""{expectedHandler}"")
				throw new System.Exception ($""Unexpected field type, found '{{fieldValueType}}', expected '{expectedHandler}'"");
",
				};
				if (mmpHandler is not null)
					test.CSProjConfig = "<HttpClientHandler>" + mmpHandler + "</HttpClientHandler>";
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		[TestCase (false)]
		[TestCase (true)]
		public void SystemMonoNotEmbedded (bool release)
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				test.Release = release;
				test.CSProjConfig = "<MonoBundlingExtraArgs>--embed-mono=no</MonoBundlingExtraArgs>";
				TI.TestSystemMonoExecutable (test);
			});
		}

		[Test]
		public void Unified_ShouldSupportDynamic ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					TestCode = @"
						NSObject o = new NSObject ();
						dynamic w = o;
						string x1 = o.Description;
						string x2 = w.Description;",
					References = " <Reference Include=\"Microsoft.CSharp\" />",
				};

				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void UnifiedFull_AllowsLinking_WithForceFlag ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					XM45 = true,
					CSProjConfig = "<LinkMode>Full</LinkMode>"
				};
				TI.TestUnifiedExecutable (test, shouldFail: true);

				test.CSProjConfig = test.CSProjConfig + "<MonoBundlingExtraArgs>--force-unsupported-linker</MonoBundlingExtraArgs>";
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void OldXcodeTest ()
		{
			var oldXcode = Xamarin.Tests.Configuration.GetOldXcodeRoot ();

			if (string.IsNullOrEmpty (oldXcode))
				Assert.Ignore ("This test needs an old Xcode.");

			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<DebugSymbols>True</DebugSymbols>", // This makes the msbuild tasks pass /debug to mmp
				};
				TI.TestUnifiedExecutable (test, shouldFail: false, environment: new Dictionary<string, string> { { "MD_APPLE_SDK_ROOT", Path.GetDirectoryName (Path.GetDirectoryName (oldXcode)) } });
			});
		}

		[Test]
		public void BuildUnified_BelowMinDeploymentTargetShouldFail ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					PlistReplaceStrings = new Dictionary<string, string> {
						{ $"<string>{SdkVersions.MinOSX}</string>", "<string>10.4</string>"}
					}
				};
				TI.TestUnifiedExecutable (test, shouldFail: true);
			});
		}

		[Test]
		public void BuildUnified_AboveSDKDeploymentTargetShouldFail ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					PlistReplaceStrings = new Dictionary<string, string> {
						{ $"<string>{SdkVersions.MinOSX}</string>", "<string>19.0</string>"}
					}
				};
				TI.TestUnifiedExecutable (test, shouldFail: true);
			});
		}

		[Test]
		public void BuildUnified_ValidSDKDeploymentTargetShouldWork ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					PlistReplaceStrings = new Dictionary<string, string> {
						{ "<string>10.9</string>", "<string>10.12</string>"}
					}
				};
				TI.TestUnifiedExecutable (test, shouldFail: false);
			});
		}

		[Test]
		[TestCase ("all")]
		[TestCase ("-all")]
		[TestCase ("remove-uithread-checks,dead-code-elimination,inline-isdirectbinding,inline-intptr-size")]
		public void Optimizations (string opt)
		{
			RunMMPTest (tmpDir => {
				var test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = $"<MonoBundlingExtraArgs>--optimize={opt}</MonoBundlingExtraArgs>" +
						"<LinkMode>Full</LinkMode>",
				};
				// register-protocols requires static registrar which is default in Release
				test.Release = true;

				var rv = TI.TestUnifiedExecutable (test, shouldFail: false);
				rv.Messages.AssertNoWarnings ();
				rv.Messages.AssertErrorCount (0);
			});
		}

		[Test]
		[TestCase ("foo")]
		public void MM0132 (string opt)
		{
			RunMMPTest (tmpDir => {
				var test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = $"<MonoBundlingExtraArgs>--optimize={opt}</MonoBundlingExtraArgs>" +
						"<LinkMode>Full</LinkMode>",
				};
				var rv = TI.TestUnifiedExecutable (test, shouldFail: false);
				rv.Messages.AssertWarning (132, $"Unknown optimization: '{opt}'. Valid optimizations are: remove-uithread-checks, dead-code-elimination, inline-isdirectbinding, inline-intptr-size, blockliteral-setupblock, register-protocols, inline-dynamic-registration-supported, static-block-to-delegate-lookup, trim-architectures, inline-is-arm64-calling-convention, cctor-beforefieldinit, custom-attributes-removal, experimental-xforms-product-type, redirect-class-handles.");
				rv.Messages.AssertErrorCount (0);
			});
		}

		[Test]
		public void MM0144 ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<XamMacArch>i386</XamMacArch>"
				};

				var rv = TI.TestUnifiedExecutable (test, shouldFail: true);
				rv.Messages.AssertError (144, "Building 32-bit apps is not supported anymore. Please change the architecture in the project's Mac Build options to 'x86_64'.");
				rv.Messages.AssertWarningCount (0);
			});
		}

		[Test]
		[TestCase (true, "v4.5.2")]
		[TestCase (true, null)]
		[TestCase (true, "v4.7.2")]
		[TestCase (false, null)]
		public void MM0176 (bool xm45, string tfv)
		{
			RunMMPTest (tmpDir => {
				var test = new TI.UnifiedTestConfig (tmpDir) {
					References = "<PackageReference Include=\"xunit.runner.utility\" Version=\"2.4.0\" />",
					TestCode = "System.Console.WriteLine (typeof (Xunit.AfterTestFinished));",
					XM45 = xm45,
				};
				if (tfv is not null)
					test.TargetFrameworkVersion = $"<TargetFrameworkVersion>{tfv}</TargetFrameworkVersion>";

				string project = TI.GenerateUnifiedExecutableProject (test);
				TI.NugetRestore (project);
				var rv = new OutputText (TI.BuildProject (project), string.Empty);
				Console.WriteLine (rv.BuildResult);
				if (xm45) {
					var referenced_version = tfv is null ? "2.0.0.0" : "4.0.0.0";
					rv.Messages.AssertWarningPattern (176, $"The assembly 'System.Web, Version={referenced_version}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' was resolved from the system's GAC: /Library/Frameworks/Mono.framework/Versions/.*/lib/mono/gac/System.Web/4.0.0.0__b03f5f7f11d50a3a/System.Web.dll. This could potentially be a problem in the future; to avoid such problems, please make sure to not use assemblies only available in the system's GAC.");
					rv.Messages.AssertWarningPattern (176, $"The assembly 'System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' was resolved from the system's GAC: /Library/Frameworks/Mono.framework/Versions/.*/lib/mono/gac/System.Drawing/4.0.0.0__b03f5f7f11d50a3a/System.Drawing.dll. This could potentially be a problem in the future; to avoid such problems, please make sure to not use assemblies only available in the system's GAC.");
					rv.Messages.AssertWarningPattern (176, $"The assembly 'System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' was resolved from the system's GAC: /Library/Frameworks/Mono.framework/Versions/.*/lib/mono/gac/System.Web.ApplicationServices/4.0.0.0__31bf3856ad364e35/System.Web.ApplicationServices.dll. This could potentially be a problem in the future; to avoid such problems, please make sure to not use assemblies only available in the system's GAC.");
					// Don't assert number of warnings, because we get a few "MM2006: Native library 'foo.dll' was referenced but could not be found." warnings as well.
				} else {
					rv.Messages.AssertWarningCount (0);
				}
			});
		}

		[Test]
		[TestCase ("inline-runtime-arch")] // This is valid for iOS, tvOS and watchOS.
		public void MM2003 (string opt)
		{
			RunMMPTest (tmpDir => {
				var test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = $"<MonoBundlingExtraArgs>--optimize={opt}</MonoBundlingExtraArgs>" +
						"<LinkMode>Full</LinkMode>",
				};
				var rv = TI.TestUnifiedExecutable (test, shouldFail: false);
				rv.Messages.AssertWarning (2003, $"Option '--optimize={opt}' will be ignored since it's only applicable to iOS, watchOS, tvOS.");
				rv.Messages.AssertErrorCount (0);
			});
		}
		[Test]
		public void BuildingSameSolutionTwice_ShouldNotRunACToolTwice ()
		{
			Func<string, bool> executedActool = (line) => {
				if (!line.Contains (" execution started with arguments"))
					return false;
				return line.Contains ("actool --errors --warnings --notices --output-format xml1 --output-partial-info-plist ");
			};

			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					AssetIcons = true
				};

				string project = TI.GenerateUnifiedExecutableProject (test);

				var buildResult = TI.BuildProject (project);
				Assert.True (buildResult.BuildOutputLines.Any (executedActool), $"Initial build should run actool");

				buildResult = TI.BuildProject (project);
				Assert.False (buildResult.BuildOutputLines.Any (executedActool), $"Second build should not run actool");

				TI.RunAndAssert ("touch", new [] { Path.Combine (tmpDir, "Assets.xcassets/AppIcon.appiconset/AppIcon-256@2x.png") }, "touch icon");

				buildResult = TI.BuildProject (project);
				Assert.True (buildResult.BuildOutputLines.Any (executedActool), $"Build after touching icon must run actool");
			});
		}

		[Test]
		public void HardenedRuntimeCodesignOption ()
		{
			// https://github.com/xamarin/xamarin-macios/issues/5653
			if (TI.InJenkins)
				Assert.Ignore ("Requires macOS entitlements on bots.");

			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<EnableCodeSigning>true</EnableCodeSigning>"
				};

				Func<OutputText, string> findCodesign = o => o.BuildResult.BuildOutputLines.Last (x => x.Contains ("Tool /usr/bin/codesign execution started with arguments"));

				var baseOutput = TI.TestUnifiedExecutable (test);
				string baseCodesign = findCodesign (baseOutput);
				Assert.False (baseCodesign.Contains ("-o runtime"), "Base codesign");
				Assert.True (baseCodesign.Contains ("--timestamp=none"), "Base codesign timestamp");

				test.CSProjConfig += "<UseHardenedRuntime>true</UseHardenedRuntime><CodeSignEntitlements>Entitlements.plist</CodeSignEntitlements>";

				const string entitlementText = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
<key>com.apple.security.cs.allow-jit</key>
<true/>
</dict>
</plist>";

				File.WriteAllText (Path.Combine (tmpDir, "Entitlements.plist"), entitlementText);

				var hardenedOutput = TI.TestUnifiedExecutable (test);
				string hardenedCodesign = findCodesign (hardenedOutput);
				Assert.True (hardenedCodesign.Contains ("-o runtime"), "Hardened codesign");
				Assert.True (hardenedCodesign.Contains ("--timestamp"), "Hardened codesign timestamp");

			});
		}

		[TestCase (false)]
		[TestCase (true)]
		public void ArchiveTask (bool full)
		{
			// https://github.com/xamarin/xamarin-macios/issues/5653
			if (TI.InJenkins)
				Assert.Ignore ("Requires macOS entitlements on bots.");

			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					XM45 = full,
					CSProjConfig = "<EnableCodeSigning>true</EnableCodeSigning>"
				};
				TI.TestUnifiedExecutable (test);
				TI.BuildProject (Path.Combine (tmpDir, full ? "XM45Example.csproj" : "UnifiedExample.csproj"), release: true, extraArgs: new [] { "/p:ArchiveOnBuild=true" });
			});

			// TODO: Add something to validate the archive is loadable by Xcode
		}

		[Test]
		public void BuildWithObjcArcFlag ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<MonoBundlingExtraArgs>-link_flags=-fobjc-arc</MonoBundlingExtraArgs>"
				};
				// Starting with Xcode 14.3, this happens if min OS <= 10.10:
				// ld: file not found: /Applications/Xcode_14.3.0-rc.2.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/lib/arc/libarclite_macosx.a
				// clang: error: linker command failed with exit code 1 (use -v to see invocation)
				if (Version.Parse (SdkVersions.MinOSX) < new Version (10, 10)) {
					test.PlistReplaceStrings = new Dictionary<string, string> {
						{ $"<string>{SdkVersions.MinOSX}</string>", "<string>10.11</string>"}
					};
				}
				TI.TestUnifiedExecutable (test);
				TI.BuildProject (Path.Combine (tmpDir, "UnifiedExample.csproj"));
			});

		}

		// some users add ',' in their assembly names :( 
		[Test]
		public void AssemblyNameWithCommaShouldNotFail () => RunMMPTest (tmpDir => {
			// Build a library with the conflicting name
			TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = true, AssemblyName = "UserLikes,ToEnumerate.Mac", ProjectName = "UserLikes,ToEnumerate.Mac" };
			TI.TestUnifiedExecutable (test, shouldFail: false);
		});
	}
}
