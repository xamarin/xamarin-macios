using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class IBToolTaskBase : XcodeCompilerToolTask
	{
		static readonly string[] WatchAppExtensions = { "-glance.plist", "-notification.plist" };
		string minimumDeploymentTarget;
		PDictionary plist;

		#region Inputs

		public bool EnableOnDemandResources { get; set; }

		[Required]
		public ITaskItem[] InterfaceDefinitions { get; set; }

		public bool IsWatchApp { get; set; }
		public bool IsWatch2App { get; set; }

		public bool IsAppExtension { get; set; }

		public string SdkRoot { get; set; }

		#endregion

		protected override string DefaultBinDir {
			get { return DeveloperRootBinDir; }
		}

		protected override string ToolName {
			get { return "ibtool"; }
		}

		protected abstract bool AutoActivateCustomFonts { get; }

		protected override bool UseCompilationDirectory {
			get { return AppleSdkSettings.XcodeVersion >= new Version (6, 3); }
		}

		protected bool CanLinkStoryboards {
			get { return AppleSdkSettings.XcodeVersion.Major > 7 || (AppleSdkSettings.XcodeVersion.Major == 7 && AppleSdkSettings.XcodeVersion.Minor >= 2); }
		}

		protected override void AppendCommandLineArguments (IDictionary<string, string> environment, CommandLineArgumentBuilder args, ITaskItem[] items)
		{
			environment.Add ("IBSC_MINIMUM_COMPATIBILITY_VERSION", minimumDeploymentTarget);
			environment.Add ("IBC_MINIMUM_COMPATIBILITY_VERSION", minimumDeploymentTarget);

			if (AppleSdkSettings.XcodeVersion.Major >= 5)
				args.Add ("--minimum-deployment-target", minimumDeploymentTarget);
			
			foreach (var targetDevice in GetTargetDevices (plist))
				args.Add ("--target-device", targetDevice);

			if (AppleSdkSettings.XcodeVersion.Major >= 6 && AutoActivateCustomFonts)
				args.Add ("--auto-activate-custom-fonts");

			if (!string.IsNullOrEmpty (SdkRoot)) {
				args.Add ("--sdk");
				args.AddQuoted (SdkRoot);
			}
		}

		string GetBundleRelativeOutputPath (ITaskItem input)
		{
			// Note: InterfaceDefinition files are *always* installed into the root of the app bundle.
			//
			// InterfaceDefinition files that are contained within a *.lproj translation directory
			// will retain the *.lproj directory as their parent, but the *.lproj directory will be
			// in the root of the app bundle.
			var components = input.ItemSpec.Split (Path.DirectorySeparatorChar);
			var bundleName = components[components.Length - 1];
			if (components.Length > 1 && components[components.Length - 2].EndsWith (".lproj", StringComparison.Ordinal))
				bundleName = Path.Combine (components[components.Length - 2], bundleName);

			switch (Path.GetExtension (bundleName)) {
			case ".storyboard":
				return Path.ChangeExtension (bundleName, ".storyboardc");
			case ".xib":
				return Path.ChangeExtension (bundleName, ".nib");
			default:
				return bundleName;
			}
		}

		static string GetPathWithoutExtension (string path)
		{
			var fileName = Path.GetFileNameWithoutExtension (path);
			var dir = Path.GetDirectoryName (path);

			if (string.IsNullOrEmpty (dir))
				return fileName;

			return Path.Combine (dir, fileName);
		}

		IEnumerable<ITaskItem> GetCompilationDirectoryOutput (string baseOutputDir, IDictionary<string, IDictionary> mapping)
		{
			var baseOutputDirs = new List<string> ();

			baseOutputDirs.Add (baseOutputDir);

			// Note: all storyboardc's/nib's will be found in the top-level or within a top-level *.lproj dir (if they've been translated)
			for (int i = 0; i < baseOutputDirs.Count; i++) {
				foreach (var path in Directory.EnumerateFileSystemEntries (baseOutputDirs[i])) {
					if (i == 0 && path.EndsWith (".lproj", StringComparison.Ordinal) && Directory.Exists (path)) {
						baseOutputDirs.Add (path);
						continue;
					}

					IDictionary metadata;

					if (!mapping.TryGetValue (path, out metadata))
						continue;

					var compiled = new TaskItem (path, metadata);

					// adjust the LogicalName since the LogicalName metadata is based on the generic output name
					// (e.g. it does not include things like ~ipad or ~iphone)
					var logicalName = compiled.GetMetadata ("LogicalName");
					var logicalDir = Path.GetDirectoryName (logicalName);
					var fileName = Path.GetFileName (path);

					compiled.SetMetadata ("LogicalName", Path.Combine (logicalDir, fileName));

					yield return compiled;
				}
			}

			yield break;
		}

		IEnumerable<ITaskItem> GetCompilationOutput (ITaskItem expected)
		{
			if (IsWatchApp) {
				var logicalName = expected.GetMetadata ("LogicalName");

				foreach (var extension in WatchAppExtensions) {
					var path = GetPathWithoutExtension (expected.ItemSpec) + extension;
					if (File.Exists (path)) {
						var item = new TaskItem (path);
						expected.CopyMetadataTo (item);
						item.SetMetadata ("LogicalName", GetPathWithoutExtension (logicalName) + extension);
						yield return item;
					}
				}
			}

			yield return expected;
		}

		static bool LogExists (string path)
		{
			if (!File.Exists (path))
				return false;

			try {
				PDictionary.FromFile (path);
				return true;
			} catch {
				File.Delete (path);
				return false;
			}
		}

		static bool InterfaceDefinitionChanged (ITaskItem interfaceDefinition, ITaskItem log)
		{
			return !LogExists (log.ItemSpec) || File.GetLastWriteTimeUtc (log.ItemSpec) < File.GetLastWriteTimeUtc (interfaceDefinition.ItemSpec);
		}

		bool CompileInterfaceDefinitions (string baseManifestDir, string baseOutputDir, List<ITaskItem> compiled, IList<ITaskItem> manifests, out bool changed)
		{
			var mapping = new Dictionary<string, IDictionary> ();
			var unique = new Dictionary<string, ITaskItem> ();
			var targets = GetTargetDevices (plist).ToList ();

			changed = false;

			foreach (var item in InterfaceDefinitions) {
				var bundleName = GetBundleRelativeOutputPath (item);
				var manifest = new TaskItem (Path.Combine (baseManifestDir, bundleName));
				var manifestDir = Path.GetDirectoryName (manifest.ItemSpec);
				ITaskItem duplicate;
				string output;

				if (!File.Exists (item.ItemSpec)) {
					Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, "The file '{0}' does not exist.", item.ItemSpec);
					continue;
				}

				if (unique.TryGetValue (bundleName, out duplicate)) {
					Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, "The file '{0}' conflicts with '{1}'.", item.ItemSpec, duplicate.ItemSpec);
					continue;
				}

				unique.Add (bundleName, item);

				var resourceTags = item.GetMetadata ("ResourceTags");
				var path = Path.Combine (baseOutputDir, bundleName);
				var outputDir = Path.GetDirectoryName (path);
				var name = GetPathWithoutExtension (path);
				var extension = Path.GetExtension (path);
				var expected = new TaskItem (path);

				expected.SetMetadata ("InterfaceDefinition", item.ItemSpec);
				expected.SetMetadata ("LogicalName", bundleName);
				expected.SetMetadata ("Optimize", "false");

				if (EnableOnDemandResources && !string.IsNullOrEmpty (resourceTags))
					expected.SetMetadata ("ResourceTags", resourceTags);

				if (UseCompilationDirectory) {
					// Note: When using --compilation-directory, we need to specify the output path as the parent directory
					output = Path.GetDirectoryName (path);
				} else {
					output = expected.ItemSpec;
				}

				if (InterfaceDefinitionChanged (item, manifest)) {
					Directory.CreateDirectory (manifestDir);
					Directory.CreateDirectory (outputDir);

					if ((Compile (new[] { item }, output, manifest)) != 0)
						return false;

					changed = true;
				} else {
					Log.LogMessage (MessageImportance.Low, "Skipping `{0}' as the output file, `{1}', is newer.", item.ItemSpec, manifest.ItemSpec);
				}

				try {
					var dict = PDictionary.FromFile (manifest.ItemSpec);

					LogWarningsAndErrors (dict, item);
				} catch (Exception ex) {
					Log.LogError ("Failed to load output log file for {0}: {1}", ToolName, ex.Message);
					if (File.Exists (manifest.ItemSpec))
						Log.LogError ("ibtool log: {0}", File.ReadAllText (manifest.ItemSpec));
					continue;
				}

				if (UseCompilationDirectory) {
					// Note: When using a compilation-directory, we'll scan dir the baseOutputDir later as
					// an optimization to collect all of the compiled output in one fell swoop.
					var metadata = expected.CloneCustomMetadata ();

					foreach (var target in targets) {
						var key = name + "~" + target + extension;

						// Note: we don't blindly .Add() here because there may already be a mapping for this file if the
						// source file is named something like "MyView.xib" and we've already processed "MyView~ipad.xib".
						//
						// When a situation like this occurs, we don't want to override the metadata.
						if (!mapping.ContainsKey (key))
							mapping.Add (key, metadata);
					}

					// Note: we don't use .Add() here because there may already be a mapping for this file if the
					// source file is named something like "MyView~ipad.xib" and we've already processed "MyView.xib".
					//
					// In this case, we want to override the metadata for "MyView.xib" with the metadata for
					// "MyView~ipad.xib".
					mapping[path] = metadata;
				} else {
					compiled.AddRange (GetCompilationOutput (expected));
				}

				manifests.Add (manifest);
			}

			if (UseCompilationDirectory)
				compiled.AddRange (GetCompilationDirectoryOutput (baseOutputDir, mapping));

			return !Log.HasLoggedErrors;
		}

		bool LinkStoryboards (string baseManifestDir, string baseOutputDir, List<ITaskItem> storyboards, List<ITaskItem> linked, IList<ITaskItem> manifests, bool changed)
		{
			var manifest = new TaskItem (Path.Combine (baseManifestDir, "link"));
			var mapping = new Dictionary<string, IDictionary> ();
			var unique = new HashSet<string> ();
			var items = new List<ITaskItem> ();

			// Make sure that `Main.storyboardc` is listed *before* `Main~ipad.storyboardc` and `Main~iphone.storyboardc`,
			// this is important for the next step to filter out the device-specific storyboards based on the same source.
			storyboards.Sort ((x, y) => string.Compare (x.ItemSpec, y.ItemSpec, StringComparison.Ordinal));

			// Populate our metadata mapping table so we can properly restore the metadata to the linked items.
			//
			// While we are at it, we'll also filter out device-specific storyboards since ibtool doesn't
			// require them if we have an equivalent generic version.
			for (int i = 0; i < storyboards.Count; i++) {
				var interfaceDefinition = storyboards[i].GetMetadata ("InterfaceDefinition");
				var bundleName = storyboards[i].GetMetadata ("LogicalName");
				var path = Path.Combine (baseOutputDir, bundleName);

				storyboards[i].RemoveMetadata ("InterfaceDefinition");
				var metadata = storyboards[i].CloneCustomMetadata ();
				mapping.Add (path, metadata);

				if (unique.Add (interfaceDefinition))
					items.Add (storyboards[i]);
			}

			// We only need to run `ibtool --link` if storyboards have changed...
			if (changed) {
				if (Directory.Exists (baseOutputDir))
					Directory.Delete (baseOutputDir, true);

				if (File.Exists (manifest.ItemSpec))
					File.Delete (manifest.ItemSpec);

				Directory.CreateDirectory (baseManifestDir);
				Directory.CreateDirectory (baseOutputDir);

				try {
					Link = true;

					if ((Compile (items.ToArray (), baseOutputDir, manifest)) != 0)
						return false;
				} finally {
					Link = false;
				}
			}

			linked.AddRange (GetCompilationDirectoryOutput (baseOutputDir, mapping));

			manifests.Add (manifest);

			return true;
		}

		IEnumerable<ITaskItem> RecursivelyEnumerateFiles (ITaskItem output)
		{
			var nibDir = output.GetMetadata ("LogicalName");

			foreach (var entry in Directory.GetFileSystemEntries (output.ItemSpec)) {
				var fileName = Path.GetFileName (entry);
				var logicalName = !string.IsNullOrEmpty (nibDir) ? Path.Combine (nibDir, fileName) : fileName;
				var rpath = Path.Combine (output.ItemSpec, fileName);
				var item = new TaskItem (rpath);

				if (File.Exists (entry)) {
					item.SetMetadata ("LogicalName", logicalName);
					item.SetMetadata ("Optimize", "false");

					yield return item;
				} else {
					if (IsWatchApp && string.IsNullOrEmpty (nibDir)) {
						// Note: ignore top-level *.storyboardc directories when compiling Watch apps
						// See https://bugzilla.xamarin.com/show_bug.cgi?id=33853 for details
						item.SetMetadata ("LogicalName", string.Empty);
					} else {
						item.SetMetadata ("LogicalName", logicalName);
					}

					foreach (var file in RecursivelyEnumerateFiles (item))
						yield return file;
				}
			}

			yield break;
		}

		IEnumerable<ITaskItem> GetBundleResources (ITaskItem compiledItem)
		{
			var baseLogicalName = compiledItem.GetMetadata ("LogicalName");
			var baseDir = compiledItem.ItemSpec;

			// Note: Watch App storyboards will be compiled to something like Interface.storyboardc/Interface.plist, but
			// Interface.plist needs to be moved up 1 level (e.g. drop the "Interface.storyboardc").
			// See https://bugzilla.xamarin.com/show_bug.cgi?id=33853 for details
			if (IsWatchApp && baseLogicalName.EndsWith (".storyboardc", StringComparison.Ordinal))
				baseLogicalName = Path.GetDirectoryName (baseLogicalName);

			foreach (var path in Directory.EnumerateFiles (baseDir, "*.*", SearchOption.AllDirectories)) {
				var rpath = PathUtils.AbsoluteToRelative (baseDir, Path.GetFullPath (path));
				var bundleResource = new TaskItem (path);
				string logicalName;

				if (!string.IsNullOrEmpty (baseLogicalName))
					logicalName = Path.Combine (baseLogicalName, rpath);
				else
					logicalName = rpath;

				compiledItem.CopyMetadataTo (bundleResource);
				bundleResource.SetMetadata ("LogicalName", logicalName);

				yield return bundleResource;
			}

			yield break;
		}

		public override bool Execute ()
		{
			if (IsWatchApp && AppleSdkSettings.XcodeVersion < new Version (6, 2)) {
				Log.LogError ("Watch apps/extensions require Xcode 6.2 or later. The current Xcode version is {0}", AppleSdkSettings.XcodeVersion);

				return !Log.HasLoggedErrors;
			}

			var ibtoolManifestDir = Path.Combine (IntermediateOutputPath, "ibtool-manifests");
			var ibtoolOutputDir = Path.Combine (IntermediateOutputPath, "ibtool");
			var outputManifests = new List<ITaskItem> ();
			var compiled = new List<ITaskItem> ();
			bool changed;

			if (InterfaceDefinitions.Length > 0) {
				if (AppManifest != null) {
					plist = PDictionary.FromFile (AppManifest.ItemSpec);
					PString value;

					if (!plist.TryGetValue (MinimumDeploymentTargetKey, out value) || string.IsNullOrEmpty (value.Value))
						minimumDeploymentTarget = SdkVersion;
					else
						minimumDeploymentTarget = value.Value;
				} else {
					minimumDeploymentTarget = SdkVersion;
				}

				Directory.CreateDirectory (ibtoolManifestDir);
				Directory.CreateDirectory (ibtoolOutputDir);

				if (!CompileInterfaceDefinitions (ibtoolManifestDir, ibtoolOutputDir, compiled, outputManifests, out changed))
					return false;

				if (CanLinkStoryboards) {
					var storyboards = new List<ITaskItem> ();
					var linked = new List<ITaskItem> ();
					var unique = new HashSet<string> ();

					for (int i = 0; i < compiled.Count; i++) {
						// pretend that non-storyboardc items (e.g. *.nib) are already 'linked'
						if (compiled[i].ItemSpec.EndsWith (".storyboardc", StringComparison.Ordinal)) {
							var interfaceDefinition = compiled[i].GetMetadata ("InterfaceDefinition");
							unique.Add (interfaceDefinition);
							storyboards.Add (compiled[i]);
							continue;
						}

						// just pretend any *nib's have already been 'linked'...
						compiled[i].RemoveMetadata ("InterfaceDefinition");
						linked.Add (compiled[i]);
					}

					// only link the storyboards if there are multiple unique storyboards
					if (unique.Count > 1) {
						var linkOutputDir = Path.Combine (IntermediateOutputPath, "ibtool-link");

						if (!LinkStoryboards (ibtoolManifestDir, linkOutputDir, storyboards, linked, outputManifests, changed))
							return false;

						compiled = linked;
					}
				} else {
					for (int i = 0; i < compiled.Count; i++)
						compiled[i].RemoveMetadata ("InterfaceDefinition");
				}
			}

			var bundleResources = new List<ITaskItem> ();

			foreach (var compiledItem in compiled) {
				if (Directory.Exists (compiledItem.ItemSpec))
					bundleResources.AddRange (GetBundleResources (compiledItem));
				else if (File.Exists (compiledItem.ItemSpec))
					bundleResources.Add (compiledItem);
			}

			BundleResources = bundleResources.ToArray ();
			OutputManifests = outputManifests.ToArray ();

			Log.LogTaskProperty ("BundleResources Output", BundleResources);
			Log.LogTaskProperty ("OutputManifests Output", OutputManifests);

			return !Log.HasLoggedErrors;
		}
	}
}
