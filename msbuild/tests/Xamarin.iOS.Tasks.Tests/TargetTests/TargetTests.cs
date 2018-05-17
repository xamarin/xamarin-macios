using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Mono.Cecil;
using NUnit.Framework;
using Xamarin.MacDev;
using Microsoft.Build.Execution;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class TargetTests : TestBase
	{
		string[] ExpectedExecutableBundleResources {
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
					"AppIcons29x29.png",
					"AppIcons29x29@2x.png",
					"AppIcons40x40@2x.png",
					"AppIcons57x57.png",
					"AppIcons57x57@2x.png",
					"AppIcons60x60@2x.png",
					"LaunchImage-568h@2x.png",
					"LaunchImage.png",
					"LaunchImage@2x.png",
					"Assets.car",
				};
				return files.Select (s => Path.Combine (AppBundlePath, s)).ToArray ();
			}
		}

		string[] ExpectedLibraryBundleResources {
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

		string[] ExpectedExecutableFiles {
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
		
		static string[] ExpectedLibraryEmbeddedResources {
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

		static bool BundleResourceExists (string path, ref int extra)
		{
			if (Path.GetExtension (path) == ".nib") {
				if (Directory.Exists (path)) {
					// Note: this suggests that things were built with the iOS 8 SDK...
					var objects8Nib = Path.Combine (path, "objects-8.0+.nib");
					var objectsNib = Path.Combine (path, "objects.nib");
					var runtimeNib = Path.Combine (path, "runtime.nib");

					extra++;

					return File.Exists (runtimeNib) && (File.Exists (objectsNib) || File.Exists (objects8Nib));
				}
			}

			return File.Exists (path);
		}

		[Test]
		public void GetReferencedAssemblies_Executable ()
		{
			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.ResolveReferences);
			var references = MonoTouchProjectInstance.GetItems ("ReferencePath").ToArray ();

			Assert.AreEqual (6, references.Length, "#1");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("MyLibrary")), "#2");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("System")), "#3a");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("System.Xml")), "#3b");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("System.Core")), "#3c");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("mscorlib")), "#3d");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("Xamarin.iOS")), "#3e");
		}

		[Test]
		public void GetReferencedAssemblies_Library ()
		{
			RunTargetOnInstance (LibraryProjectInstance, TargetName.ResolveReferences);
			var references = LibraryProjectInstance.GetItems ("ReferencePath").ToArray ();

			Assert.AreEqual (5, references.Length, "#1");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("System")), "#2a");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("System.Xml")), "#2b");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("System.Core")), "#2c");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("mscorlib")), "#2d");
			Assert.IsTrue (references.Any (t => t.EvaluatedInclude.Contains ("Xamarin.iOS")), "#2e");
		}

		[Test]
		public void BuildExecutable ()
		{
			var expectedFiles = ExpectedExecutableFiles;
			int extra = 0;

			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.Build);

			Assert.IsTrue (Directory.Exists (AppBundlePath), "#1");
			foreach (var file in expectedFiles)
				Assert.IsTrue (BundleResourceExists (file, ref extra), "#2. " + file);

			// Verify that we have not bundled BundleResource or Content items as embedded resources
			var assemblyDef = AssemblyDefinition.ReadAssembly (Path.Combine (AppBundlePath, "MySingleView.exe"));
			Assert.AreEqual (2, assemblyDef.MainModule.Resources.OfType <EmbeddedResource> ().Count (), "#3");
			
			var plist = PDictionary.FromFile (Path.Combine (AppBundlePath, "Info.plist"));
			Assert.IsTrue (plist.ContainsKey ("CFBundleExecutable"));
			Assert.IsTrue (plist.ContainsKey ("CFBundleVersion"));
			Assert.IsNotEmpty (((PString)plist["CFBundleExecutable"]).Value);
			Assert.IsNotEmpty (((PString)plist["CFBundleVersion"]).Value);
		}

		[Test]
		public void CopyContentToBundle ()
		{
			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.CopyResourcesToBundle);

			foreach (var v in ExpectedExecutableBundleResources)
				Assert.IsTrue (File.Exists (v) || Directory.Exists (v), string.Format ("{0} was not copied to the bundle", Path.GetFullPath (v)));
		}

		[Test]
		public void CleanExecutable ()
		{
			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.Clean);
			Assert.IsFalse (Directory.Exists (MonoTouchProjectBinPath), "#1a");
			Assert.IsFalse (Directory.Exists (MonoTouchProjectObjPath), "#1b");

			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.Build);
			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.Clean);
			Assert.IsEmpty (Directory.GetDirectories (MonoTouchProjectBinPath, "*.dSYM", SearchOption.AllDirectories), "#2a");
			Assert.IsEmpty (Directory.GetFiles (MonoTouchProjectBinPath, "*.*", SearchOption.AllDirectories), "#2b");
			Assert.IsFalse (Directory.Exists (MonoTouchProjectObjPath), "#2c");
		}

		[Test]
		public void CleanLibrary ()
		{
			RunTargetOnInstance (LibraryProjectInstance, TargetName.Clean);
			Assert.IsFalse (Directory.Exists (LibraryProjectBinPath), "#1a");
			Assert.IsFalse (Directory.Exists (LibraryProjectObjPath), "#1b");

			RunTargetOnInstance (LibraryProjectInstance, TargetName.Build);
			RunTargetOnInstance (LibraryProjectInstance, TargetName.Clean);
			Assert.IsEmpty (Directory.GetFiles (LibraryProjectBinPath, "*.*", SearchOption.AllDirectories), "#2a");
			Assert.IsFalse (Directory.Exists (LibraryProjectObjPath), "#2b");
		}

		[Test]
		public void CompileInterfaceDefinitions_Library ()
		{
			RunTargetOnInstance (LibraryProjectInstance, TargetName.CompileInterfaceDefinitions);
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
			var originalFile = Path.Combine (MonoTouchProjectPath, "Resources", "image.png");
			var optimisedFile = Path.Combine (AppBundlePath, "image.png");

			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.Build);

			Assert.IsTrue (File.Exists (optimisedFile), "#1");
			if (shouldBeDifferent)
				CollectionAssert.AreNotEqual (File.ReadAllBytes (originalFile), File.ReadAllBytes (optimisedFile), "#2a");
			else
				CollectionAssert.AreEqual (File.ReadAllBytes (originalFile), File.ReadAllBytes (optimisedFile), "#2b");
		}

		[Test]
		public void RebuildExecutable_NoModifications ()
		{
			// Put a thread.sleep so that the initial build happens a noticable amount of time after we copy
			// all the input files into the temporary directory. This means that any timestamps modified as
			// part of the original build will definitely be newer than the timestamps written during the
			// execution of the test fixture 'setup' method.
			Thread.Sleep (1000);
			RunTarget (MonoTouchProject, TargetName.Build);
			var timestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			Thread.Sleep (1000);
			RunTarget (MonoTouchProject, TargetName.Build);
			var newTimestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			foreach (var file in timestamps.Keys)
				Assert.AreEqual (timestamps [file], newTimestamps [file], "#1: " + file);
		}

		[Test]
		public void RebuildExecutable_TouchLibraryDll ()
		{
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
			var libraryPath = Path.Combine (LibraryProjectBinPath, "MyLibrary.dll");

			RunTarget (LibraryProject, TargetName.Build);
			var timestamp = GetLastModified (libraryPath);

			Thread.Sleep (1000);
			RunTarget (LibraryProject, TargetName.Build);
			Assert.AreEqual (timestamp, GetLastModified (libraryPath));
		}

		[Test]
		public void RebuildLibrary_TouchBundleResource ()
		{
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
			BuildLibraryCore (ExpectedLibraryEmbeddedResources);
		}

		[Test]
		public void BuildLibrary_NoInterfaceDefinitions ()
		{
			LibraryProject.RemoveItems (LibraryProject.GetItems ("InterfaceDefinition"));
			LibraryProjectInstance = LibraryProject.CreateProjectInstance ();
			
			BuildLibraryCore (ExpectedLibraryEmbeddedResources.Where (s => !s.Contains ("storyboardc")).ToArray ());
		}

		void BuildLibraryCore (string[] expectedResources)
		{
			var library = Path.Combine (LibraryProjectBinPath, "MyLibrary.dll");
			RunTargetOnInstance (LibraryProjectInstance, TargetName.Build);
			
			Assert.IsTrue (string.IsNullOrEmpty (LibraryProjectInstance.GetPropertyValue ("AppBundleDir")), "#1");
			var entries = Directory.GetFileSystemEntries (LibraryProjectBinPath);
			Assert.AreEqual (2, entries.Length, "#1");
			Assert.IsTrue (File.Exists (library), "#2");
			Assert.IsTrue (File.Exists (Path.ChangeExtension (library, ".pdb")), "#3");
			
			var assemblyDef = AssemblyDefinition.ReadAssembly (library);
			var actualResources = assemblyDef.MainModule.Resources.Select (n => n.Name).ToList ();
			
			foreach (var resource in expectedResources)
				Assert.IsTrue (actualResources.Contains (resource), "#1. " + resource);
			Assert.AreEqual (expectedResources.Length, assemblyDef.MainModule.Resources.OfType <EmbeddedResource> ().Count (), "#2");
		}

		[Test]
		public void GenerateBundleName_ExecutableProject ()
		{
			// Initially the AssemblyName is set and there is no app bundle dir
			Assert.AreEqual ("MySingleView", MonoTouchProjectInstance.GetPropertyValue ("AssemblyName"), "#1");
			Assert.IsTrue (string.IsNullOrEmpty (MonoTouchProjectInstance.GetPropertyValue ("AppBundleDir")), "#2");

			// Now we should have an AppBundleDir
			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.GenerateBundleName);
			Assert.AreEqual ("bin/iPhoneSimulator/Debug/MySingleView.app", MonoTouchProjectInstance.GetPropertyValue ("AppBundleDir"), "#3");
		}

		[Test]
		public void PackLibraryResources_ExecutableProject ()
		{
			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.PackLibraryResources);
			var embeddedResources = MonoTouchProjectInstance.GetItems ("EmbeddedResource").ToArray ();
			Assert.AreEqual (2, embeddedResources.Length, "#1");
			Assert.IsTrue (embeddedResources.Any (i => i.EvaluatedInclude == "LinkedEmbeddedResource.txt"), "#1"); 
			Assert.IsTrue (embeddedResources.Any (i => i.EvaluatedInclude == Path.Combine ("Folder", "EmbeddedResource.txt")), "#2"); 
		}

		[Test]
		public void PackLibraryResources_LibraryProject ()
		{
			RunTargetOnInstance (LibraryProjectInstance, TargetName.PackLibraryResources);
			var embeddedResources = LibraryProjectInstance.GetItems ("EmbeddedResource").ToArray ();
			Assert.AreEqual (13, embeddedResources.Length, "#1");
		}

		[Test]
		public void UnpackLibraryResources_ExecutableProject ()
		{
			// We unpack 4 embedded resources from the library project into BundleResources
			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.Build);
			var bundleResources = MonoTouchProjectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
			Assert.IsTrue (bundleResources.Length >= 33, "#1");
		}

		[Test]
		public void UnpackLibraryResources_LibraryProject ()
		{
			// We should not unpack any EmbeddedResources into BundleResources
			RunTargetOnInstance (LibraryProjectInstance, TargetName.Build);
			var bundleResources = LibraryProjectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
			Assert.AreEqual (11, bundleResources.Length, "#1");
		}

		[Test]
		public void BundleResources ()
		{
			var actool = Path.Combine ("obj", "iPhoneSimulator", "Debug", "actool", "bundle");
			var ibtool = Path.Combine ("obj", "iPhoneSimulator", "Debug", "ibtool");
			var path = Path.Combine (MonoTouchProjectPath, "Info.plist");
			var plist = PDictionary.FromFile (path);

			plist.SetMinimumOSVersion ("7.0");
			plist.Save (path, true);

			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.CollectBundleResources);

			var bundleItems = MonoTouchProjectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine ("Folder", "BundleResource.txt") && i.GetMetadataValue ("LogicalName") == "Folder/BundleResource.txt"), "#1");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine ("Folder", "Content.txt") && i.GetMetadataValue ("LogicalName") == "Folder/Content.txt"), "#2");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == "LinkedBundleResource.txt" && i.GetMetadataValue ("LogicalName") == "Folder/LinkedBundleResource.txt"), "#3");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == "LinkedContent.txt" && i.GetMetadataValue ("LogicalName") == "Folder/LinkedContent.txt"), "#4");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons29x29.png") && i.GetMetadataValue ("LogicalName") == "AppIcons29x29.png"), "#5");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons29x29@2x.png") && i.GetMetadataValue ("LogicalName") == "AppIcons29x29@2x.png"), "#6");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons40x40@2x.png") && i.GetMetadataValue ("LogicalName") == "AppIcons40x40@2x.png"), "#7");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons57x57.png") && i.GetMetadataValue ("LogicalName") == "AppIcons57x57.png"), "#8");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons57x57@2x.png") && i.GetMetadataValue ("LogicalName") == "AppIcons57x57@2x.png"), "#9");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons60x60@2x.png") && i.GetMetadataValue ("LogicalName") == "AppIcons60x60@2x.png"), "#10");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "LaunchImage-568h@2x.png") && i.GetMetadataValue ("LogicalName") == "LaunchImage-568h@2x.png"), "#11");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "LaunchImage.png") && i.GetMetadataValue ("LogicalName") == "LaunchImage.png"), "#12");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (actool, "LaunchImage@2x.png") && i.GetMetadataValue ("LogicalName") == "LaunchImage@2x.png"), "#13");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "MainStoryboard.storyboardc", "1-view-2.nib") && i.GetMetadataValue ("LogicalName") == "MainStoryboard.storyboardc/1-view-2.nib"), "#14");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "MainStoryboard.storyboardc", "Info.plist") && i.GetMetadataValue ("LogicalName") == "MainStoryboard.storyboardc/Info.plist"), "#15");
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "MainStoryboard.storyboardc", "UIViewController-1.nib") && i.GetMetadataValue ("LogicalName") == "MainStoryboard.storyboardc/UIViewController-1.nib"), "#16");
			if (bundleItems.Length > ExpectedExecutableBundleResources.Length) {
				Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "en.lproj", "TranslatedView.nib", "runtime.nib") && i.GetMetadataValue ("LogicalName") == "en.lproj/TranslatedView.nib/runtime.nib"), "#17");
				Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "FolderView.nib", "runtime.nib") && i.GetMetadataValue ("LogicalName") == "FolderView.nib/runtime.nib"), "#18");
				Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "iPhoneView.nib", "runtime.nib") && i.GetMetadataValue ("LogicalName") == "iPhoneView.nib/runtime.nib"), "#19");
			} else {
				Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "en.lproj", "TranslatedView.nib") && i.GetMetadataValue ("LogicalName") == "en.lproj/TranslatedView.nib"), "#17");
				Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "FolderView.nib") && i.GetMetadataValue ("LogicalName") == "FolderView.nib"), "#18");
				Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine (ibtool, "iPhoneView.nib") && i.GetMetadataValue ("LogicalName") == "iPhoneView.nib"), "#19");
			}
			Assert.IsTrue (bundleItems.Any (i => i.EvaluatedInclude == Path.Combine ("Resources", "image.png") && i.GetMetadataValue ("LogicalName") == "image.png"), "#20");
		}

		[Test (Description = "Xambug #39137")]
		public void AddAppIcon_NoClean()
		{
			var project = SetupProject (Engine, MonoTouchProjectCSProjPath);
			var actool = Path.Combine ("obj", "iPhoneSimulator", "Debug", "actool", "bundle");
			var path = Path.Combine (MonoTouchProjectPath, "Info.plist");
			var plist = PDictionary.FromFile (path);
			var plistCopy = PDictionary.FromFile (path);

			// Start without app icon.
			plist.Remove("XSAppIconAssets");
			plist.SetMinimumOSVersion ("7.0");
			plist.Save (path, true);

			var projectInstance = project.CreateProjectInstance ();
			RunTargetOnInstance (projectInstance, TargetName.CompileImageAssets, 0);

			var bundleItemsNoAppIcon = projectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
			Assert.IsFalse (bundleItemsNoAppIcon.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons60x60@2x.png") && i.GetMetadataValue ("LogicalName") == "AppIcons60x60@2x.png"), "#1");

			project = SetupProject (Engine, MonoTouchProjectCSProjPath);
			projectInstance = project.CreateProjectInstance ();

			// Put a thread.sleep so that we get noticeable timestamps.
			Thread.Sleep (1000);

			// Re-save the original plist (adding app icon).
			plistCopy.Save (path, true);

			// Re-run the task with app icon set this time and no clean.
			// The task should be aware the app icon is now being used.
			RunTargetOnInstance (projectInstance, TargetName.CompileImageAssets, 0);

			var bundleItemsWithAppIcon = projectInstance.GetItems ("_BundleResourceWithLogicalName").ToArray ();
			Assert.IsTrue (bundleItemsWithAppIcon.Any (i => i.EvaluatedInclude == Path.Combine (actool, "AppIcons60x60@2x.png") && i.GetMetadataValue ("LogicalName") == "AppIcons60x60@2x.png"), "#2");
		}

		[Test (Description = "Xambug #16331")]
		public void Disappearing_Bundle_Resource ()
		{
			string resource = Path.Combine (MonoTouchProjectPath, "Folder", "BundleResource.txt");
			string resourceGone = resource + ".disabled";
			try {
				File.Move (resource, resourceGone);
				RunTarget_WithErrors (MonoTouchProjectInstance, "_CollectBundleResources");
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
			string resource = Path.Combine (MonoTouchProjectPath, "Folder", "Content.txt");
			string resourceGone = resource + ".disabled";
			try {
				File.Move (resource, resourceGone);
				RunTarget_WithErrors (MonoTouchProjectInstance, "_CollectBundleResources");
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
			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.DetectAppManifest);
			Assert.IsFalse (string.IsNullOrEmpty(MonoTouchProjectInstance.GetPropertyValue ("_AppManifest")), "#1");
		}

		[Test]
		public void DetectAppManifest_ExecutableProject_NoPList ()
		{
			RemoveItemsByName (MonoTouchProject, "None");
			MonoTouchProjectInstance = MonoTouchProject.CreateProjectInstance ();

			RunTarget_WithErrors (MonoTouchProjectInstance, TargetName.DetectAppManifest);
			Assert.IsNullOrEmpty (MonoTouchProjectInstance.GetPropertyValue ("_AppManifest"), "#1");
		}

		[Test]
		public void DetectAppManifest_ExecutableProject_TwoPLists ()
		{
			RemoveItemsByName (MonoTouchProject, "None");
			MonoTouchProjectInstance = MonoTouchProject.CreateProjectInstance ();

			MonoTouchProjectInstance.AddItem ("None", "Fake/Info.plist");
			MonoTouchProjectInstance.AddItem ("None", "Info.plist");

			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.DetectAppManifest);
			Assert.AreEqual ("Info.plist", MonoTouchProjectInstance.GetPropertyValue ("_AppManifest"), "#1");
		}

		[Test]
		public void DetectAppManifest_ExecutableProject_LinkedPList ()
		{
			string linkedPlist = CreateTempFile (Path.Combine (TempDir, "Linked.plist"));

			RemoveItemsByName (MonoTouchProject, "None");
			MonoTouchProjectInstance = MonoTouchProject.CreateProjectInstance ();

			var item = MonoTouchProjectInstance.AddItem ("None", linkedPlist);
			item.SetMetadata ("Link", "Info.plist");

			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.DetectAppManifest);
			Assert.AreEqual (linkedPlist, MonoTouchProjectInstance.GetPropertyValue ("_AppManifest"), "#1");
		}

		[Test]
		public void DetectAppManifest_ExecutableProject_LogicalNamePList ()
		{
			string logicalPlist = CreateTempFile (Path.Combine (TempDir, "Logical.plist"));

			RemoveItemsByName (MonoTouchProject, "None");
			MonoTouchProjectInstance = MonoTouchProject.CreateProjectInstance ();

			var item = MonoTouchProjectInstance.AddItem ("None", logicalPlist);
			item.SetMetadata ("LogicalName", "Info.plist");

			RunTargetOnInstance (MonoTouchProjectInstance, TargetName.DetectAppManifest);
			Assert.AreEqual (logicalPlist, MonoTouchProjectInstance.GetPropertyValue ("_AppManifest"), "#1");
		}

		[Test]
		public void DetectAppManifest_LibraryProject ()
		{
			RunTargetOnInstance (LibraryProjectInstance, TargetName.DetectAppManifest);
			Assert.IsNullOrEmpty (LibraryProjectInstance.GetPropertyValue ("_AppManifest"), "#1");
		}
	}
}
