using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using NUnit.Framework;

#nullable enable

namespace Xamarin.MacDev.Tasks {

	// Regression tests for the binding resource package 'manifest' handling: a manifest is passive data
	// that may come from a restored package, so it must not be able to inject path/layout/identity
	// metadata that could redirect this task's (or a downstream task's) output outside the intended
	// output directory.
	[TestFixture]
	public class ResolveNativeReferencesSidecarTest : TestBase {

		ResolveNativeReferences CreateSidecarTask (string manifestContents, out string tmpdir)
		{
			tmpdir = Cache.CreateTemporaryDirectory ();
			var resources = Path.Combine (tmpdir, "Binding.resources");
			Directory.CreateDirectory (resources);
			File.WriteAllText (Path.Combine (resources, "manifest"), manifestContents);
			// Create the (fake) native library referenced by the manifest, for realism.
			File.WriteAllText (Path.Combine (resources, "libpayload.dylib"), "fake dylib");

			var task = CreateTask<ResolveNativeReferences> ();
			task.Architectures = "arm64";
			task.FrameworksDirectory = "Frameworks";
			task.IntermediateOutputPath = Path.Combine (tmpdir, "obj");
			task.SdkIsSimulator = false;
			task.TargetFrameworkMoniker = "net9.0-macos";
			task.References = new ITaskItem [] { new TaskItem (Path.Combine (tmpdir, "Binding.dll")) };
			return task;
		}

		[Test]
		public void StripsUnsafeManifestMetadata ()
		{
			var manifest = @"<BindingAssembly>
	<NativeReference Name=""libpayload.dylib"">
		<Kind>Dynamic</Kind>
		<ForceLoad>True</ForceLoad>
		<Frameworks>CoreFoundation</Frameworks>
		<NoDSymUtil>true</NoDSymUtil>
		<NoSymbolStrip>true</NoSymbolStrip>
		<RelativePath>../../../../../../tmp/escape/libpayload.dylib</RelativePath>
		<ReidentifiedPath>/tmp/escape/libpayload.dylib</ReidentifiedPath>
		<ComputedRelativePath>../../escape/libpayload.dylib</ComputedRelativePath>
		<DynamicLibraryId>@executable_path/../../../../escape</DynamicLibraryId>
		<PublishFolderType>Unknown</PublishFolderType>
		<TargetDirectory>/tmp/escape</TargetDirectory>
		<SourceDirectory>/tmp/escape</SourceDirectory>
	</NativeReference>
</BindingAssembly>";
			var task = CreateSidecarTask (manifest, out var _);

			ExecuteTask (task, 0);

			var item = task.NativeFrameworks!.Single (v => v.GetMetadata ("Kind") == "Dynamic");

			// Legitimate native-reference metadata (standard + binding-defined) is preserved.
			Assert.That (item.GetMetadata ("ForceLoad"), Is.EqualTo ("True"), "ForceLoad");
			Assert.That (item.GetMetadata ("Frameworks"), Is.EqualTo ("CoreFoundation"), "Frameworks");
			Assert.That (item.GetMetadata ("NoDSymUtil"), Is.EqualTo ("true"), "NoDSymUtil");
			Assert.That (item.GetMetadata ("NoSymbolStrip"), Is.EqualTo ("true"), "NoSymbolStrip");

			// Build-controlled path/layout/identity metadata is NOT copied from the manifest.
			foreach (var blocked in new [] { "RelativePath", "ReidentifiedPath", "ComputedRelativePath", "DynamicLibraryId", "TargetDirectory", "SourceDirectory" })
				Assert.That (item.GetMetadata (blocked), Is.Empty, blocked);
			// The code-set PublishFolderType survives (the manifest's value is ignored).
			Assert.That (item.GetMetadata ("PublishFolderType"), Is.EqualTo ("DynamicLibrary"), "PublishFolderType");

			// A warning is emitted for each ignored (blocked) metadata - and only for those (use the quoted
			// name so e.g. 'ComputedRelativePath' isn't counted as 'RelativePath').
			var warnings = Engine.Logger.WarningsEvents.Select (v => v.Message ?? "").ToArray ();
			foreach (var blocked in new [] { "RelativePath", "ReidentifiedPath", "ComputedRelativePath", "DynamicLibraryId", "PublishFolderType", "TargetDirectory", "SourceDirectory" })
				Assert.That (warnings.Count (v => v.Contains ($"'{blocked}'")), Is.EqualTo (1), $"{blocked} warning");
			foreach (var allowed in new [] { "NoDSymUtil", "NoSymbolStrip", "ForceLoad", "Frameworks" })
				Assert.That (warnings.Count (v => v.Contains ($"'{allowed}'")), Is.EqualTo (0), $"no warning for {allowed}");
		}

		[Test]
		public void IgnoresReservedMetadata ()
		{
			// A crafted manifest must not be able to crash the task with reserved MSBuild metadata names
			// (TaskItem.SetMetadata throws an ArgumentException for those).
			var manifest = @"<BindingAssembly>
	<NativeReference Name=""libpayload.dylib"">
		<Kind>Dynamic</Kind>
		<Filename>evil</Filename>
		<Identity>evil</Identity>
	</NativeReference>
</BindingAssembly>";
			var task = CreateSidecarTask (manifest, out var _);

			// The task completes without throwing/erroring (reaching this assertion means it didn't crash).
			ExecuteTask (task, 0);

			var item = task.NativeFrameworks!.Single (v => v.GetMetadata ("Kind") == "Dynamic");
			// The reserved metadata keeps its item-spec-derived value; the manifest can't override it.
			Assert.That (item.GetMetadata ("Filename"), Is.EqualTo ("libpayload"), "Filename");
		}

		[Test]
		public void IgnoresNativeReferenceWithoutName ()
		{
			var manifest = @"<BindingAssembly>
	<NativeReference>
		<Kind>Dynamic</Kind>
	</NativeReference>
</BindingAssembly>";
			var task = CreateSidecarTask (manifest, out var _);

			ExecuteTask (task, 0);

			// The nameless native reference is skipped (the managed reference itself isn't added).
			Assert.That (task.NativeFrameworks!, Is.Empty, "no native frameworks");
			Assert.That (Engine.Logger.WarningsEvents.Count (v => (v.Message ?? "").Contains ("no name")), Is.EqualTo (1), "no-name warning");
		}

		[Test]
		public void RejectsTraversalInName ()
		{
			var manifest = @"<BindingAssembly>
	<NativeReference Name=""../../../../../../tmp/escape/evil.dylib"">
		<Kind>Dynamic</Kind>
	</NativeReference>
</BindingAssembly>";
			var task = CreateSidecarTask (manifest, out var _);

			ExecuteTask (task, 1);

			Assert.That (Engine.Logger.ErrorEvents.Single ().Message, Does.Contain ("'..'"), "error mentions traversal");
		}

		[TestCase (".framework", false)]
		[TestCase (".Framework", true)]
		[TestCase (".FRAMEWORK", true)]
		public void WarnsForIncorrectFrameworkExtension (string extension, bool expectWarning)
		{
			var name = "XTest" + extension;
			var task = CreateSidecarTask ($@"<BindingAssembly>
	<NativeReference Name=""{name}"">
		<Kind>Framework</Kind>
	</NativeReference>
</BindingAssembly>", out var tmpdir);
			var framework = Path.Combine (tmpdir, "Binding.resources", name);
			Directory.CreateDirectory (framework);
			File.WriteAllText (Path.Combine (framework, "XTest"), "fake framework");

			ExecuteTask (task);

			var warnings = Engine.Logger.WarningsEvents.Where (v => (v.Code ?? "").EndsWith ("7198")).ToArray ();
			Assert.That (warnings.Length, Is.EqualTo (expectWarning ? 1 : 0), "Framework extension warning count");
			if (expectWarning)
				Assert.That (warnings [0].Message, Does.Contain (framework), "Warning identifies the framework");
		}
	}
}
