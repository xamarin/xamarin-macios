using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

namespace Xamarin.MMP.Tests
{
	public partial class MMPTests 
	{
		const string NativeReferenceTemplate = @"<ItemGroup><NativeReference Include=""{0}""><IsCxx>False</IsCxx><Kind>{1}</Kind></NativeReference></ItemGroup>";

		public string SimpleDylibPath
		{
			get
			{
				string rootDir = TI.FindRootDirectory ();
				string buildLibPath = Path.Combine (rootDir, "../../maccore/tests/mmptest/bin/SimpleClass.dylib");
				Assert.IsTrue (File.Exists (buildLibPath), string.Format ("SimpleDylibPath missing? {0}", buildLibPath));
				return buildLibPath;
			}
		}

		public string SimpleStaticPath
		{
			get
			{
				string rootDir = TI.FindRootDirectory ();
				string buildLibPath = Path.Combine (rootDir, "../../maccore/tests/mmptest/bin/SimpleClassStatic.a");
				Assert.IsTrue (File.Exists (buildLibPath), string.Format ("SimpleStaticPath missing? {0}", buildLibPath));
				return buildLibPath;
			}
		}

		[Test]
		public void Unified_WithNativeReferences_InMainProjectWorks ()
		{
			RunMMPTest (tmpDir => {
				// Could be any dylib not in /System
				const string SystemLibPath = "/Library/Frameworks/Mono.framework/Versions/Current/lib/libsqlite3.0.dylib";

				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { ItemGroup = string.Format (NativeReferenceTemplate, SystemLibPath, "Dynamic") };
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - System", "libsqlite3.0.dylib", true);

				test.ItemGroup = string.Format (NativeReferenceTemplate, Path.GetFullPath (SimpleDylibPath), "Dynamic");
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - Local", "SimpleClass.dylib", true);

				test.ItemGroup = string.Format (NativeReferenceTemplate, Path.GetFullPath (SimpleStaticPath), "Static");
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - Static", null, true);

				test.ItemGroup = string.Format (NativeReferenceTemplate, "/Library/Frameworks/iTunesLibrary.framework", "Framework");
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - Framework", null, true);
			});
		}

		[Test]
		public void Unified_WithStaticNativeRef_ClangIncludesOurStaticLib ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { ItemGroup = string.Format (NativeReferenceTemplate, SimpleStaticPath, "Static") };
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - Static", null, true, false, s =>
				{
					string clangLine = s.Split ('\n').First (x => x.Contains ("xcrun -sdk macosx clang"));
					return clangLine.Contains ("SimpleClassStatic.a");
				});
			});
		}

		[Test]
		public void Unified_WithNativeReferences_MissingLibrariesActAsExpected ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { ItemGroup = string.Format (NativeReferenceTemplate, "/Library/Frameworks/ALibThatDoesNotExist.dylib", "Dynamic") };
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_MissingLibrariesActAsExpected - Nonexistant", null, false);

				// Test a system dylib. Does not matter which one
				test.ItemGroup = string.Format (NativeReferenceTemplate, "/System/Library/Frameworks/MapKit.framework/Versions/A/Resources/BridgeSupport/MapKit.dylib", "Dynamic");
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_MissingLibrariesActAsExpected - System", null, true);

				// Test one of the ignored libs
				test.ItemGroup = string.Format (NativeReferenceTemplate, "cups", "Dynamic");
				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_MissingLibrariesActAsExpected - Ignored", null, true);
			});
		}

		[Test]
		public void Unified_WithNativeReferences_IgnoredWorks ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					ItemGroup = string.Format (NativeReferenceTemplate, Path.GetFullPath (SimpleDylibPath), "Dynamic"),
					CSProjConfig = string.Format (@"<MonoBundlingExtraArgs>--ignore-native-library=""{0}""</MonoBundlingExtraArgs>", Path.GetFullPath (SimpleDylibPath))
				};

				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_InMainProjectWorks - Local", null, true, true);
			});
		}

		[Test]
		public void Unified_WithNativeReferences_ReadOnlyNativeLib ()
		{
			RunMMPTest (tmpDir => {
				string dylibPath = Path.Combine (tmpDir, "dll/");
				string filePath = Path.Combine (dylibPath, "SimpleClassDylib.dylib");
				Directory.CreateDirectory (dylibPath);
				File.Copy (Path.Combine (TI.AssemblyDirectory, TI.TestDirectory + "mac-binding-project/bin/SimpleClassDylib.dylib"), filePath );
				File.SetAttributes (filePath, FileAttributes.ReadOnly); 

				string itemGroup = "<ItemGroup><NativeReference Include=\".\\dll\\SimpleClassDylib.dylib\"> <IsCxx>False</IsCxx><Kind>Dynamic</Kind> </NativeReference> </ItemGroup>";
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { ProjectName = "UnifiedExample.csproj", ItemGroup = itemGroup };

				NativeReferenceTestCore (tmpDir, test, "Unified_WithNativeReferences_ReadOnlyLib", null, true, true);
			});
		}

		void NativeReferenceTestCore (string tmpDir, TI.UnifiedTestConfig test, string testName, string libraryName, bool buildShouldBeSuccessful, bool libraryShouldNotBeCopied = false, Func <string, bool> processBuildOutput = null)
		{
			// Mobile
			test.XM45 = false;
			string buildResults = TI.TestUnifiedExecutable (test, false);
			Assert.IsTrue (!buildShouldBeSuccessful || !buildResults.Contains ("MM2006"), string.Format ("{0} - Mobile had MM2006 state {1} not match expected\n{2}", testName, buildShouldBeSuccessful, buildResults));
			if (processBuildOutput != null)
				Assert.IsTrue (processBuildOutput (buildResults), string.Format ("{0} - Mobile - We did not see our expected item in the build output: {1}", testName, libraryName));

			string mobileBundlePath = Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle/");
			if (libraryName != null)
				Assert.IsTrue (Directory.GetFiles (mobileBundlePath).Any (x => x.Contains (libraryName) == !libraryShouldNotBeCopied), string.Format ("{0} - Mobile - We did not pull in native lib: {1}", testName, libraryName));

			// XM45
			test.XM45 = true;
			buildResults = TI.TestUnifiedExecutable (test, false);
			Assert.IsTrue (!buildShouldBeSuccessful || !buildResults.Contains ("MM2006"), string.Format ("{0} - XM45 had MM2006 state {1} not match expected\n{2}", testName, buildShouldBeSuccessful, buildResults));
			if (processBuildOutput != null)
				Assert.IsTrue (processBuildOutput (buildResults), string.Format ("{0} - Mobile - We did not see our expected item in the build output: {1}", testName, libraryName));

			string xm45BundlePath = Path.Combine (tmpDir, "bin/Debug/XM45Example.app/Contents/MonoBundle/");
			if (libraryName != null)
				Assert.IsTrue (Directory.GetFiles (xm45BundlePath).Any (x => x.Contains (libraryName) == !libraryShouldNotBeCopied), string.Format ("{0} - XM45 - We did not pull in native lib: {1}", testName, libraryName));
		}
	}
}
