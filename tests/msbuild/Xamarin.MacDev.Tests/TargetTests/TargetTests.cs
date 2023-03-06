using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;
using Xamarin.MacDev;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class TargetTests : TestBase {
		public TargetTests ()
			: base ("iPhoneSimulator")
		{
		}

		string [] ExpectedExecutableBundleResources {
			get {
				var files = new [] {
					Path.Combine ("Folder", "BundleResource.txt"),
					Path.Combine ("Folder", "Content.txt"),
					Path.Combine ("Folder", "LinkedBundleResource.txt"),
					Path.Combine ("Folder", "LinkedContent.txt"),
					Path.Combine ("MainStoryboard.storyboardc", "1-view-2.nib"),
					Path.Combine ("MainStoryboard.storyboardc", "Info.plist"),
					Path.Combine ("MainStoryboard.storyboardc", "UIViewController-1.nib"),
					Path.Combine ("Archer", "Archer_Attack.atlasc", "Archer_Attack.1.png"),
					Path.Combine ("Archer", "Archer_Attack.atlasc", "Archer_Attack.plist"),
					Path.Combine ("en.lproj", "TranslatedView.nib"),
					"FolderView.nib",
					"iPhoneView.nib",
					"image.png",
					"AppIcons60x60@2x.png",
					"LaunchImage-568h@2x.png",
					"LaunchImage.png",
					"LaunchImage@2x.png",
					"Assets.car",
				};
				return files.Select (s => Path.Combine (AppBundlePath, s)).ToArray ();
			}
		}

		string [] ExpectedLibraryBundleResources {
			get {
				var files = new [] {
					Path.Combine ("LibrarySecondStoryboard.storyboardc", "43-view-49.nib"),
					Path.Combine ("LibrarySecondStoryboard.storyboardc", "45-view-53.nib"),
					Path.Combine ("LibrarySecondStoryboard.storyboardc", "Info.plist"),
					Path.Combine ("LibrarySecondStoryboard.storyboardc", "UITabBarController-41.nib"),
					Path.Combine ("LibraryStoryboard.storyboardc", "1-view-2.nib"),
					Path.Combine ("LibraryStoryboard.storyboardc", "Info.plist"),
					Path.Combine ("LibraryStoryboard.storyboardc", "UIViewController-1.nib"),
					Path.Combine ("MyLibraryFolder", "LibraryBundleResource.txt"),
					Path.Combine ("MyLibraryFolder", "LibraryContent.txt"),
					Path.Combine ("MyLibraryFolder", "LibraryLinkedBundleResource.txt"),
					Path.Combine ("MyLibraryFolder", "LibraryLinkedContent.txt"),
				};
				return files.Select (s => Path.Combine (AppBundlePath, s)).ToArray ();
			}
		}

		string [] ExpectedExecutableFiles {
			get {
				var files = new [] {
					"MonoTouchDebugConfiguration.txt",
					"Info.plist",
					Path.Combine ("Settings.bundle", "Root.plist"),
					"MyLibrary.dll",
					"MyLibrary.pdb",
					"MySingleView",
					"MySingleView.exe",
					"MySingleView.pdb",
					"System.Core.dll",
					"System.Core.pdb",
					"System.Xml.dll",
					"System.Xml.pdb",
					"System.dll",
					"System.pdb",
					"Xamarin.iOS.dll",
					"Xamarin.iOS.pdb",
					"mscorlib.dll",
					"mscorlib.pdb",
					"runtime-options.plist",
				};

				var expected = new List<string> ();
				expected.AddRange (files.Select (s => Path.Combine (AppBundlePath, s)));
				expected.AddRange (ExpectedExecutableBundleResources);
				expected.AddRange (ExpectedLibraryBundleResources);

				return expected.ToArray ();
			}
		}

		static string [] ExpectedLibraryEmbeddedResources {
			get {
				return new [] {
					"MyLibrary.MyLibraryFolder.LibraryLinkedEmbeddedResource.txt",
					"MyLibrary.MyLibraryFolder.LibraryEmbeddedResource.txt",
					"__monotouch_content_MyLibraryFolder_fLibraryLinkedBundleResource.txt",
					"__monotouch_content_MyLibraryFolder_fLibraryBundleResource.txt",
					"__monotouch_content_MyLibraryFolder_fLibraryLinkedContent.txt",
					"__monotouch_content_MyLibraryFolder_fLibraryContent.txt",
					"__monotouch_content_LibraryStoryboard.storyboardc_f1-view-2.nib",
					"__monotouch_content_LibraryStoryboard.storyboardc_fInfo.plist",
					"__monotouch_content_LibraryStoryboard.storyboardc_fUIViewController-1.nib",
					"__monotouch_content_LibrarySecondStoryboard.storyboardc_f43-view-49.nib",
					"__monotouch_content_LibrarySecondStoryboard.storyboardc_f45-view-53.nib",
					"__monotouch_content_LibrarySecondStoryboard.storyboardc_fInfo.plist",
					"__monotouch_content_LibrarySecondStoryboard.storyboardc_fUITabBarController-41.nib"
				};
			}
		}

		static void BundleResourceExists (string path)
		{
			if (Path.GetExtension (path) == ".nib") {
				if (Directory.Exists (path)) {
					// Note: this suggests that things were built with the iOS 8 SDK...
					var objects8Nib = Path.Combine (path, "objects-8.0+.nib");
					var objects13Nib = Path.Combine (path, "objects-13.0+.nib");
					var objectsNib = Path.Combine (path, "objects.nib");
					var runtimeNib = Path.Combine (path, "runtime.nib");

					Assert.That (File.Exists (runtimeNib), $"File exists: {runtimeNib}");
					Assert.That (File.Exists (objectsNib) || File.Exists (objects8Nib) || File.Exists (objects13Nib), $"File exists: {objectsNib} || {objects8Nib} || {objects13Nib}");
					return;
				}
			}

			Assert.That (path, Does.Exist, $"Existence of {path}");
		}

		[Test]
		public void GetReferencedAssemblies_Executable ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (MonoTouchProject, TargetName.ResolveReferences);
			var references = MonoTouchProjectInstance.GetItems ("ReferencePath").ToArray ();
			var expected_references = new string [] {
				"MyLibrary.dll",
				"System.dll",
				"System.Xml.dll",
				"System.Core.dll",
				"mscorlib.dll",
				"Xamarin.iOS.dll",
				"System.Drawing.Common.dll",
			};
			Array.Sort (expected_references);

			var actual_references = references.Select ((v) => Path.GetFileName (v.EvaluatedInclude)).OrderBy ((v) => v);
			CollectionAssert.AreEquivalent (expected_references, actual_references, "References");
		}

		[Test]
		public void GetReferencedAssemblies_Library ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (LibraryProject, TargetName.ResolveReferences);
			var references = LibraryProjectInstance.GetItems ("ReferencePath").ToArray ();
			var expected_references = new string [] {
				"System.dll",
				"System.Xml.dll",
				"System.Core.dll",
				"mscorlib.dll",
				"Xamarin.iOS.dll",
				"System.Drawing.Common.dll",
			};
			Array.Sort (expected_references);

			var actual_references = references.Select ((v) => Path.GetFileName (v.EvaluatedInclude)).OrderBy ((v) => v);
			CollectionAssert.AreEquivalent (expected_references, actual_references, "References");
		}

		[Test]
		public void BuildExecutable ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var expectedFiles = ExpectedExecutableFiles;

			RunTarget (MonoTouchProject, TargetName.Build);

			Assert.IsTrue (Directory.Exists (AppBundlePath), "#1");

			var bundleResources = Directory.GetFileSystemEntries (AppBundlePath, "*", SearchOption.AllDirectories);
			var inexistentResource = expectedFiles.Except (expectedFiles).ToArray ();

			Assert.That (inexistentResource, Is.Empty, "No missing resources");

			foreach (var file in expectedFiles)
				BundleResourceExists (file);

			// Verify that we have not bundled BundleResource or Content items as embedded resources
			var assemblyDef = AssemblyDefinition.ReadAssembly (Path.Combine (AppBundlePath, "MySingleView.exe"));
			Assert.AreEqual (2, assemblyDef.MainModule.Resources.OfType<EmbeddedResource> ().Count (), "#3");

			var plist = PDictionary.FromFile (Path.Combine (AppBundlePath, "Info.plist"));
			Assert.IsTrue (plist.ContainsKey ("CFBundleExecutable"));
			Assert.IsTrue (plist.ContainsKey ("CFBundleVersion"));
			Assert.IsNotEmpty (((PString) plist ["CFBundleExecutable"]).Value);
			Assert.IsNotEmpty (((PString) plist ["CFBundleVersion"]).Value);
		}

		[Test]
		public void CopyContentToBundle ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (MonoTouchProject, TargetName.CopyResourcesToBundle);

			foreach (var v in ExpectedExecutableBundleResources)
				Assert.That (v, Does.Exist, string.Format ("{0} was not copied to the bundle", Path.GetFullPath (v)));
		}

		[Test]
		public void CleanExecutable ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (MonoTouchProject, TargetName.Clean);
			Assert.IsFalse (Directory.Exists (MonoTouchProjectBinPath), "#1a");
			Assert.IsFalse (Directory.Exists (MonoTouchProjectObjPath), "#1b");

			RunTarget (MonoTouchProject, TargetName.Build);
			RunTarget (MonoTouchProject, TargetName.Clean);
			Assert.IsEmpty (Directory.GetDirectories (MonoTouchProjectBinPath, "*.dSYM", SearchOption.AllDirectories), "#2a");
			Assert.IsEmpty (Directory.GetFiles (MonoTouchProjectBinPath, "*.*", SearchOption.AllDirectories), "#2b");
			Assert.IsFalse (Directory.Exists (MonoTouchProjectObjPath), "#2c");
		}

		[Test]
		public void CleanLibrary ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (LibraryProject, TargetName.Clean);
			Assert.IsFalse (Directory.Exists (LibraryProjectBinPath), "#1a");
			Assert.IsFalse (Directory.Exists (LibraryProjectObjPath), "#1b");

			RunTarget (LibraryProject, TargetName.Build);
			RunTarget (LibraryProject, TargetName.Clean);
			Assert.IsEmpty (Directory.GetFiles (LibraryProjectBinPath, "*.*", SearchOption.AllDirectories), "#2a");
			Assert.IsFalse (Directory.Exists (LibraryProjectObjPath), "#2b");
		}

		[Test]
		public void CompileInterfaceDefinitions_Library ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (LibraryProject, TargetName.CompileInterfaceDefinitions);
			Assert.IsNotEmpty (Directory.GetFiles (LibraryProjectObjPath, "*.*", SearchOption.AllDirectories), "#1");
		}

		[Test]
		public void OptimizePngs_DefaultValue ()
		{
			OptimizePngs_Core (true);
		}

		[Test]
		public void OptimizePngs_True ()
		{
			MonoTouchProjectInstance.SetProperty ("OptimizePNGs", "True");
			OptimizePngs_Core (true);
		}

		[Test]
		public void OptimizePngs_False ()
		{
			MonoTouchProjectInstance.SetProperty ("OptimizePNGs", "False");
			OptimizePngs_Core (false);
		}

		void OptimizePngs_Core (bool shouldBeDifferent)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var originalFile = Path.Combine (MonoTouchProjectPath, "Resources", "image.png");
			var optimisedFile = Path.Combine (AppBundlePath, "image.png");

			RunTarget (MonoTouchProject, TargetName.Build);

			Assert.IsTrue (File.Exists (optimisedFile), "#1");
			if (shouldBeDifferent)
				CollectionAssert.AreNotEqual (File.ReadAllBytes (originalFile), File.ReadAllBytes (optimisedFile), "#2a");
			else
				CollectionAssert.AreEqual (File.ReadAllBytes (originalFile), File.ReadAllBytes (optimisedFile), "#2b");
		}

		[Test]
		public void RebuildExecutable_NoModifications ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			// Put a thread.sleep so that the initial build happens a noticable amount of time after we copy
			// all the input files into the temporary directory. This means that any timestamps modified as
			// part of the original build will definitely be newer than the timestamps written during the
			// execution of the test fixture 'setup' method.
			EnsureFilestampChange ();
			RunTarget (MonoTouchProject, TargetName.Build);
			var timestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			EnsureFilestampChange ();
			RunTarget (MonoTouchProject, TargetName.Build);
			var newTimestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			foreach (var file in timestamps.Keys)
				Assert.AreEqual (timestamps [file], newTimestamps [file], "#1: " + file);
		}

		[Test]
		public void RebuildExecutable_TouchLibraryDll ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (MonoTouchProject, TargetName.Build);
			var timestamps = ExpectedExecutableFiles.ToDictionary (file => file, file => GetLastModified (file));

			Touch (Path.Combine (LibraryProjectObjPath, "MyLibrary.dll"));
			RunTarget (MonoTouchProject, TargetName.Build);
			var newTimestamps = ExpectedExecutableFiles.ToDictionary (file => file, file => GetLastModified (file));

			// At least some files should be modified now
			Assert.IsTrue (timestamps.Keys.Any (key => timestamps [key] != newTimestamps [key]), "#1");
		}

		[Test]
		public void RebuildLibrary_NoModifications ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var libraryPath = Path.Combine (LibraryProjectBinPath, "MyLibrary.dll");

			RunTarget (LibraryProject, TargetName.Build);
			var timestamp = GetLastModified (libraryPath);

			EnsureFilestampChange ();
			RunTarget (LibraryProject, TargetName.Build);
			Assert.AreEqual (timestamp, GetLastModified (libraryPath));
		}

		[Test]
		public void RebuildLibrary_TouchBundleResource ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var libraryPath = Path.Combine (LibraryProjectBinPath, "MyLibrary.dll");

			RunTarget (LibraryProject, TargetName.Build);
			var timestamp = GetLastModified (libraryPath);

			Touch (Path.Combine (LibraryProjectPath, "MyLibraryFolder", "LibraryBundleResource.txt"));
			RunTarget (LibraryProject, TargetName.Build);
			Assert.AreNotEqual (timestamp, GetLastModified (libraryPath));
		}

		[Test]
		public void RebuildLibrary_TouchEmbeddedResource ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var libraryPath = Path.Combine (LibraryProjectBinPath, "MyLibrary.dll");

			RunTarget (LibraryProject, TargetName.Build);
			var timestamp = GetLastModified (libraryPath);

			Touch (Path.Combine (LibraryProjectPath, "MyLibraryFolder", "LibraryEmbeddedResource.txt"));
			RunTarget (LibraryProject, TargetName.Build);
			Assert.AreNotEqual (timestamp, GetLastModified (libraryPath));
		}

		[Test]
		public void RebuildLibrary_TouchStoryboard ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var libraryPath = Path.Combine (LibraryProjectBinPath, "MyLibrary.dll");

			RunTarget (LibraryProject, TargetName.Build);
			var timestamp = GetLastModified (libraryPath);

			Touch (Path.Combine (LibraryProjectPath, "LibraryStoryboard.storyboard"));
			RunTarget (LibraryProject, TargetName.Build);
			Assert.AreNotEqual (timestamp, GetLastModified (libraryPath));
		}

		[Test]
		public void BuildLibrary ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			BuildLibraryCore (ExpectedLibraryEmbeddedResources);
		}

		[Test]
		public void BuildLibrary_NoInterfaceDefinitions ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			LibraryProjectInstance.RemoveItems ("InterfaceDefinition");

			BuildLibraryCore (ExpectedLibraryEmbeddedResources.Where (s => !s.Contains ("storyboardc")).ToArray ());
		}

		void BuildLibraryCore (string [] expectedResources)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var library = Path.Combine (LibraryProjectBinPath, "MyLibrary.dll");
			RunTarget (LibraryProject, TargetName.Build);

			Assert.IsTrue (string.IsNullOrEmpty (LibraryProjectInstance.GetPropertyValue ("AppBundleDir")), "#1");
			var entries = Directory.GetFileSystemEntries (LibraryProjectBinPath);
			Assert.AreEqual (2, entries.Length, "#1");
			Assert.IsTrue (File.Exists (library), "#2");
			Assert.IsTrue (File.Exists (Path.ChangeExtension (library, ".pdb")), "#3");

			var assemblyDef = AssemblyDefinition.ReadAssembly (library);
			var actualResources = assemblyDef.MainModule.Resources.Select (n => n.Name).ToList ();

			foreach (var resource in expectedResources)
				Assert.IsTrue (actualResources.Contains (resource), "#1. " + resource);
			Assert.AreEqual (expectedResources.Length, assemblyDef.MainModule.Resources.OfType<EmbeddedResource> ().Count (), "#2");
		}

		[Test]
		public void GenerateBundleName_ExecutableProject ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			// Run a target that doesn't exist (since it's not possible to evaluate the csproj without running a target)
			RunTarget (MonoTouchProject, "None", expectedErrorCount: 1);

			// Initially the AssemblyName is set and there is no app bundle dir
			Assert.AreEqual ("MySingleView", MonoTouchProjectInstance.GetPropertyValue ("AssemblyName"), "#1");
			Assert.IsTrue (string.IsNullOrEmpty (MonoTouchProjectInstance.GetPropertyValue ("AppBundleDir")), "#2");

			// Now we should have an AppBundleDir
			RunTarget (MonoTouchProject, TargetName.GenerateBundleName);
			Assert.AreEqual (@"bin/iPhoneSimulator/Debug/MySingleView.app", MonoTouchProjectInstance.GetPropertyValue ("AppBundleDir"), "#3");
		}

		[Test]
		public void PackLibraryResources_ExecutableProject ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (MonoTouchProject, TargetName.PackLibraryResources);
			var embeddedResources = MonoTouchProjectInstance.GetItems ("EmbeddedResource").ToArray ();
			Assert.AreEqual (2, embeddedResources.Length, "#1");
			Assert.IsTrue (embeddedResources.Any (i => i.EvaluatedInclude == "LinkedEmbeddedResource.txt"), "#1");
			Assert.IsTrue (embeddedResources.Any (i => i.EvaluatedInclude == Path.Combine ("Folder", "EmbeddedResource.txt")), "#2");
		}

		[Test]
		public void PackLibraryResources_LibraryProject ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (LibraryProject, TargetName.PackLibraryResources);
			var embeddedResources = LibraryProjectInstance.GetItems ("EmbeddedResource").ToArray ();
			Assert.AreEqual (13, embeddedResources.Length, "#1");
		}

		[Test]
		public void UnpackLibraryResources_ExecutableProject ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			// We unpack 4 embedded resources from the library project into BundleResources
			RunTarget (MonoTouchProject, TargetName.Build);
			var bundleResources = MonoTouchProjectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
			Assert.That (bundleResources.Length, Is.GreaterThanOrEqualTo (30), "#1\n\t" + string.Join ("\n\t", bundleResources.Select (v => v.EvaluatedInclude)));
		}

		[Test]
		public void UnpackLibraryResources_LibraryProject ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			// We should not unpack any EmbeddedResources into BundleResources
			RunTarget (LibraryProject, TargetName.Build);
			var bundleResources = LibraryProjectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
			Assert.AreEqual (11, bundleResources.Length, "#1");
		}

		[Test]
		public void BundleResources ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var actool = Path.Combine ("obj", "iPhoneSimulator", "Debug", "actool", "bundle");
			var ibtool = Path.Combine ("obj", "iPhoneSimulator", "Debug", "ibtool");
			var path = Path.Combine (MonoTouchProjectPath, "Info.plist");
			var plist = PDictionary.FromFile (path);

			plist.SetMinimumOSVersion ("7.0");
			plist.Save (path, true);

			RunTarget (MonoTouchProject, TargetName.CollectBundleResources);

			var bundleItems = MonoTouchProjectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();

			var allBundleItems = "\n\t" + string.Join ("\n\t", bundleItems.Select (v => v.EvaluatedInclude));

			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine ("Folder", "BundleResource.txt") && i.GetMetadataValue ("LogicalName") == "Folder/BundleResource.txt"), "#1" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine ("Folder", "Content.txt") && i.GetMetadataValue ("LogicalName") == "Folder/Content.txt"), "#2" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == "LinkedBundleResource.txt" && i.GetMetadataValue ("LogicalName") == "Folder/LinkedBundleResource.txt"), "#3" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == "LinkedContent.txt" && i.GetMetadataValue ("LogicalName") == "Folder/LinkedContent.txt"), "#4" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons60x60@2x.png") && i.GetMetadataValue ("LogicalName") == "AppIcons60x60@2x.png"), "#10" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "LaunchImage-568h@2x.png") && i.GetMetadataValue ("LogicalName") == "LaunchImage-568h@2x.png"), "#11" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "LaunchImage.png") && i.GetMetadataValue ("LogicalName") == "LaunchImage.png"), "#12" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "LaunchImage@2x.png") && i.GetMetadataValue ("LogicalName") == "LaunchImage@2x.png"), "#13" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "MainStoryboard.storyboardc", "1-view-2.nib") && i.GetMetadataValue ("LogicalName") == "MainStoryboard.storyboardc/1-view-2.nib"), "#14" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "MainStoryboard.storyboardc", "Info.plist") && i.GetMetadataValue ("LogicalName") == "MainStoryboard.storyboardc/Info.plist"), "#15" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "MainStoryboard.storyboardc", "UIViewController-1.nib") && i.GetMetadataValue ("LogicalName") == "MainStoryboard.storyboardc/UIViewController-1.nib"), "#16" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "en.lproj", "TranslatedView.nib") && i.GetMetadataValue ("LogicalName") == "en.lproj/TranslatedView.nib"), "#17" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "FolderView.nib") && i.GetMetadataValue ("LogicalName") == "FolderView.nib"), "#18" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "iPhoneView.nib") && i.GetMetadataValue ("LogicalName") == "iPhoneView.nib"), "#19" + allBundleItems);
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine ("Resources", "image.png") && i.GetMetadataValue ("LogicalName") == "image.png"), "#20" + allBundleItems);
		}

		[Test (Description = "Xambug #39137")]
		public void AddAppIcon_NoClean ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var actool = Path.Combine ("obj", "iPhoneSimulator", "Debug", "actool", "bundle");
			var path = Path.Combine (MonoTouchProjectPath, "Info.plist");
			var plist = PDictionary.FromFile (path);
			var plistCopy = PDictionary.FromFile (path);

			// Start without app icon.
			plist.Remove ("XSAppIconAssets");
			plist.SetMinimumOSVersion ("7.0");
			plist.Save (path, true);

			RunTarget (MonoTouchProject, TargetName.CompileImageAssets, 0);

			var bundleItemsNoAppIcon = MonoTouchProjectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
			Assert.IsFalse (bundleItemsNoAppIcon.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons60x60@2x.png") && i.GetMetadataValue ("LogicalName") == "AppIcons60x60@2x.png"), "#1");

			// Put a thread.sleep so that we get noticeable timestamps.
			EnsureFilestampChange ();

			// Re-save the original plist (adding app icon).
			plistCopy.Save (path, true);

			// Re-run the task with app icon set this time and no clean.
			// The task should be aware the app icon is now being used.
			RunTarget (MonoTouchProject, TargetName.CompileImageAssets, 0);

			var bundleItemsWithAppIcon = MonoTouchProjectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
			Assert.IsTrue (bundleItemsWithAppIcon.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons60x60@2x.png") && i.GetMetadataValue ("LogicalName") == "AppIcons60x60@2x.png"), "#2");
		}

		[Test (Description = "Xambug #16331")]
		public void Disappearing_Bundle_Resource ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			string resource = Path.Combine (MonoTouchProjectPath, "Folder", "BundleResource.txt");
			string resourceGone = resource + ".disabled";
			try {
				File.Move (resource, resourceGone);
				RunTarget (MonoTouchProject, "_CollectBundleResources", expectedErrorCount: 1);
				var bundleItems = MonoTouchProjectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
				Assert.IsNotNull (bundleItems, "#1");
				Assert.IsTrue (bundleItems.Length >= 17, "#2");
				Assert.IsFalse (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine ("Folder", "BundleResource.txt")), "#3");
			} finally {
				File.Move (resourceGone, resource);
			}
		}

		[Test (Description = "Xambug #16331")]
		public void Disappearing_Content ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			string resource = Path.Combine (MonoTouchProjectPath, "Folder", "Content.txt");
			string resourceGone = resource + ".disabled";
			try {
				File.Move (resource, resourceGone);
				RunTarget (MonoTouchProject, "_CollectBundleResources", expectedErrorCount: 1);
				var bundleItems = MonoTouchProjectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
				Assert.IsNotNull (bundleItems, "#1");
				Assert.IsTrue (bundleItems.Length >= 17, "#2");
				Assert.IsFalse (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine ("Folder", "Content.txt")), "#3");
			} finally {
				File.Move (resourceGone, resource);
			}
		}

		[Test]
		public void DetectAppManifest_ExecutableProject ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (MonoTouchProject, TargetName.DetectAppManifest);
			Assert.That (MonoTouchProjectInstance.GetPropertyValue ("AppBundleManifest"), Is.Not.Null.And.Not.Empty, "#1");
		}

		[Test]
		public void DetectAppManifest_ExecutableProject_NoPList ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			MonoTouchProjectInstance.RemoveItems ("None");

			RunTarget (MonoTouchProject, TargetName.DetectAppManifest, expectedErrorCount: 1);
			Assert.That (MonoTouchProjectInstance.GetPropertyValue ("AppBundleManifest"), Is.Null.Or.Empty, "#1");
		}

		[Test]
		public void DetectAppManifest_ExecutableProject_TwoPLists ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			MonoTouchProjectInstance.RemoveItems ("None");

			MonoTouchProjectInstance.AddItem ("None", "Fake/Info.plist");
			MonoTouchProjectInstance.AddItem ("None", "Info.plist");

			RunTarget (MonoTouchProject, TargetName.DetectAppManifest);
			Assert.AreEqual ("Info.plist", MonoTouchProjectInstance.GetPropertyValue ("AppBundleManifest"), "#1");
		}

		[Test]
		public void DetectAppManifest_ExecutableProject_LinkedPList ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			string linkedPlist = CreateTempFile ("Linked.plist");

			MonoTouchProjectInstance.RemoveItems ("None");

			MonoTouchProjectInstance.AddItem ("None", linkedPlist, new Dictionary<string, string> { { "Link", "Info.plist" } });

			RunTarget (MonoTouchProject, TargetName.DetectAppManifest);
			Assert.AreEqual (linkedPlist, MonoTouchProjectInstance.GetPropertyValue ("AppBundleManifest"), "#1");
		}

		[Test]
		public void DetectAppManifest_ExecutableProject_LogicalNamePList ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			string logicalPlist = CreateTempFile ("Logical.plist");

			MonoTouchProjectInstance.RemoveItems ("None");

			MonoTouchProjectInstance.AddItem ("None", logicalPlist, new Dictionary<string, string> { { "LogicalName", "Info.plist" } });

			RunTarget (MonoTouchProject, TargetName.DetectAppManifest);
			Assert.AreEqual (logicalPlist, MonoTouchProjectInstance.GetPropertyValue ("AppBundleManifest"), "#1");
		}

		[Test]
		public void DetectAppManifest_LibraryProject ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunTarget (LibraryProject, TargetName.DetectAppManifest);
			Assert.That (LibraryProjectInstance.GetPropertyValue ("AppBundleManifest"), Is.Null.Or.Empty, "#1");
		}
	}
}
