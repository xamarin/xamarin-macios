using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

using Xamarin.Tests;

namespace Xamarin.MMP.Tests
{
	public partial class MMPTests {
		const string ItemGroupTemplate = @"<ItemGroup>{0}</ItemGroup>";
		const string NativeReferenceTemplate = @"<NativeReference Include=""{0}""><IsCxx>False</IsCxx><Kind>{1}</Kind></NativeReference>";

		public string SimpleDylibPath {
			get {
				string rootDir = TI.FindRootDirectory ();
				string buildLibPath = Path.Combine (rootDir, "../tests/mac-binding-project/bin/SimpleClassDylib.dylib");
				Assert.IsTrue (File.Exists (buildLibPath), string.Format ("SimpleDylibPath missing? {0}", buildLibPath));
				return buildLibPath;
			}
		}

		public string SimpleStaticPath {
			get {
				string rootDir = TI.FindRootDirectory ();
				string buildLibPath = Path.Combine (rootDir, "../tests/mac-binding-project/bin/SimpleClassStatic.a");
				Assert.IsTrue (File.Exists (buildLibPath), string.Format ("SimpleStaticPath missing? {0}", buildLibPath));
				return buildLibPath;
			}
		}

		public string MobileStaticBindingPath {
			get {
				string rootDir = TI.FindRootDirectory ();
				string buildLibPath = Path.Combine (rootDir, "../tests/mac-binding-project/bin/Mobile-static/MobileBinding.dll");
				Assert.IsTrue (File.Exists (buildLibPath), string.Format ("MobileStaticBindingPath missing? {0}", buildLibPath));
				return buildLibPath;
			}
		}

		string CreateNativeRefInclude (string path, string kind) => string.Format (NativeReferenceTemplate, path, kind);
		string CreateItemGroup (IEnumerable<string> elements) => string.Format (ItemGroupTemplate, string.Concat (elements));
		string CreateSingleNativeRef (string path, string kind) => CreateItemGroup (CreateNativeRefInclude (path, kind).FromSingleItem ());

		string CreateCopyOfSimpleClassInTestDir (string tmpDir, string fileName = "SimpleClassDylib.dylib")
		{
			string dylibPath = Path.Combine (tmpDir, "dll/");
			string filePath = Path.Combine (dylibPath, fileName);
			Directory.CreateDirectory (dylibPath);
			File.Copy (Path.Combine (TI.AssemblyDirectory, TI.TestDirectory + "mac-binding-project/bin/SimpleClassDylib.dylib"), filePath);
			return filePath;
		}

		void NativeReferenceTestCore (string tmpDir, TI.UnifiedTestConfig test, string testName, string libraryName, bool buildShouldBeSuccessful, bool libraryShouldNotBeCopied = false, Func<string, bool> processBuildOutput = null)
		{
			// Mobile
			test.XM45 = false;
			string buildResults = TI.TestUnifiedExecutable (test, false).BuildOutput;
			Assert.IsTrue (!buildShouldBeSuccessful || !buildResults.Contains ("MM2006"), string.Format ("{0} - Mobile had MM2006 state {1} not match expected\n{2}", testName, buildShouldBeSuccessful, buildResults));
			if (processBuildOutput != null)
				Assert.IsTrue (processBuildOutput (buildResults), string.Format ("{0} - Mobile - We did not see our expected item in the build output: {1}", testName, libraryName));

			string mobileBundlePath = Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle/");
			if (libraryName != null)
				Assert.IsTrue (Directory.GetFiles (mobileBundlePath).Any (x => x.Contains (libraryName) == !libraryShouldNotBeCopied), string.Format ("{0} - Mobile - We did not pull in native lib: {1}", testName, libraryName));

			// XM45
			test.XM45 = true;
			buildResults = TI.TestUnifiedExecutable (test, false).BuildOutput;
			Assert.IsTrue (!buildShouldBeSuccessful || !buildResults.Contains ("MM2006"), string.Format ("{0} - XM45 had MM2006 state {1} not match expected\n{2}", testName, buildShouldBeSuccessful, buildResults));
			if (processBuildOutput != null)
				Assert.IsTrue (processBuildOutput (buildResults), string.Format ("{0} - Mobile - We did not see our expected item in the build output: {1}", testName, libraryName));

			string xm45BundlePath = Path.Combine (tmpDir, "bin/Debug/XM45Example.app/Contents/MonoBundle/");
			if (libraryName != null)
				Assert.IsTrue (Directory.GetFiles (xm45BundlePath).Any (x => x.Contains (libraryName) == !libraryShouldNotBeCopied), string.Format ("{0} - XM45 - We did not pull in native lib: {1}", testName, libraryName));
		}

		[Test]
		public void Unified_WithNativeReferences_InMainProjectWorks ()
		{
			RunMMPTest (tmpDir => {
				// Could be any dylib not in /System
				const string SystemLibPath = "/Library/Frameworks/Mono.framework/Versions/Current/lib/libsqlite3.0.dylib";

				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { ItemGroup = CreateSingleNativeRef (SystemLibPath, "Dynamic") };
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - System", "libsqlite3.0.dylib", true);

				test.ItemGroup = CreateSingleNativeRef (Path.GetFullPath (SimpleDylibPath), "Dynamic");
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - Local", "SimpleClassDylib.dylib", true);

				test.ItemGroup = CreateSingleNativeRef (Path.GetFullPath (SimpleStaticPath), "Static");
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - Static", null, true);

				test.ItemGroup = CreateSingleNativeRef ("/Library/Frameworks/iTunesLibrary.framework", "Framework");
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - Framework", null, true);
				Assert.True (Directory.Exists (Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/Frameworks/iTunesLibrary.framework")));

				string binaryPath = Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MacOS/UnifiedExample");
				string otoolText = TI.RunAndAssert ("/usr/bin/otool", new StringBuilder ("-l " + binaryPath), "Unified_WithNativeReferences_InMainProjectWorks - rpath");
				Assert.True (otoolText.Contains ("path @loader_path/../Frameworks"));
			});
		}

		[Test]
		public void Unified_WithStaticNativeRef_ClangIncludesOurStaticLib ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { ItemGroup = CreateSingleNativeRef (SimpleStaticPath, "Static") };
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - Static", null, true, false, s => {
					string clangLine = s.Split ('\n').First (x => x.Contains ("xcrun -sdk macosx clang"));
					return clangLine.Contains ("SimpleClassStatic.a");
				});
			});
		}

		[Test]
		public void Unified_WithStaticNativeRef_32bit ()
		{
			Configuration.AssertXcodeSupports32Bit ();

			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<XamMacArch>i386</XamMacArch>",
					References = $"<Reference Include=\"SimpleBinding_static\"><HintPath>{MobileStaticBindingPath}</HintPath></Reference>",
					TestCode = "System.Console.WriteLine (new Simple.SimpleClass ().DoIt ());"
				};
				NativeReferenceTestCore (tmpDir, test, "Unified_WithStaticNativeRef_32bit", null, true, false);
			});
		}

		[Test]
		public void Unified_WithNativeReferences_MissingLibrariesActAsExpected ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { ItemGroup = CreateSingleNativeRef ("/Library/Frameworks/ALibThatDoesNotExist.dylib", "Dynamic") };
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_MissingLibrariesActAsExpected - Nonexistant", null, false);

				// Test a system dylib. Does not matter which one
				test.ItemGroup = CreateSingleNativeRef ( "/System/Library/Frameworks/MapKit.framework/Versions/A/Resources/BridgeSupport/MapKit.dylib", "Dynamic");
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_MissingLibrariesActAsExpected - System", null, true);

				// Test one of the ignored libs
				test.ItemGroup = CreateSingleNativeRef ("cups", "Dynamic");
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_MissingLibrariesActAsExpected - Ignored", null, true);
			});
		}

		[Test]
		public void Unified_WithNativeReferences_IgnoredWorks ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					ItemGroup = CreateSingleNativeRef (Path.GetFullPath (SimpleDylibPath), "Dynamic"),
					CSProjConfig = string.Format (@"<MonoBundlingExtraArgs>--ignore-native-library=""{0}""</MonoBundlingExtraArgs>", Path.GetFullPath (SimpleDylibPath))
				};

				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - Local", null, true, true);
			});
		}

		[Test]
		public void Unified_WithNativeReferences_ReadOnlyNativeLib ()
		{
			RunMMPTest (tmpDir => {
				string filePath = CreateCopyOfSimpleClassInTestDir (tmpDir);
				File.SetAttributes (filePath, FileAttributes.ReadOnly);

				string itemGroup = CreateSingleNativeRef (@".\dll\SimpleClassDylib.dylib", "Dynamic");
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { ProjectName = "UnifiedExample.csproj", ItemGroup = itemGroup };

				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_ReadOnlyLib", null, true, true);
			});
		}

		[Test]
		public void NativeReference_WithDllImportOfSamePath_Builds ()
		{
			RunMMPTest (tmpDir => {
				string filePath = CreateCopyOfSimpleClassInTestDir (tmpDir);

				// Use absolute path here and in TestDecl to trigger bug
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) 
				{
					ProjectName = "UnifiedExample.csproj",
					ItemGroup = CreateSingleNativeRef (filePath, "Dynamic"),
					TestDecl = string.Format ("[System.Runtime.InteropServices.DllImport (\"{0}\")]public static extern int GetFour ();", filePath),
					TestCode = "GetFour ();"
				};

				NativeReferenceTestCore (tmpDir, test, "NativeReference_WithDllImportOfSamePath_Builds", null, true, true);
			});
		}

		[Test]
		public void MultipleNativeReferences_OnlyInvokeMMPOneTime_AndCopyEverythingIn ()
		{
			RunMMPTest (tmpDir => {
				string firstPath = CreateCopyOfSimpleClassInTestDir (tmpDir);
				string secondPath = CreateCopyOfSimpleClassInTestDir (tmpDir, "SeconClassDylib.dylib");
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					ProjectName = "UnifiedExample.csproj",
					CSProjConfig = "<EnableCodeSigning>true</EnableCodeSigning>",
					ItemGroup = CreateItemGroup (new string[] { CreateNativeRefInclude (firstPath, "Dynamic"), CreateNativeRefInclude (secondPath, "Dynamic") }),
				};
				NativeReferenceTestCore (tmpDir, test, "MultipleNativeReferences_OnlyInvokeMMPOneTime_AndCopyEverythingIn", null, true);
			});
		}
	}
}
