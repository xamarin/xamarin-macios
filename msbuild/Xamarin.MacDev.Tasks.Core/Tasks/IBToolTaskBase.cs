using System;
using System.IO;
using System.Linq;
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

		protected override void AppendCommandLineArguments (IDictionary<string, string> environment, ProcessArgumentBuilder args, ITaskItem[] items)
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
			// Note: InterfaceDefinition files are *always* installed into the root of the app bundle
			// InterfaceDefinition files that are contained within an .lproj translation directory
			// will retain the .lproj directory as their parent, but the .lproj directory will be
			// in the root of the app bundle.
			//var bundleName = BundleResource.GetLogicalName (ProjectDir, ResourcePrefixes, input);
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

		static string GetPathWithoutExtension (string path)
		{
			int dot = path.LastIndexOf ('.');

			return path.Substring (0, dot);
		}

		IEnumerable<ITaskItem> GetCompilationDirectoryOutput (ITaskItem expected)
		{
			var dir = Path.GetDirectoryName (expected.ItemSpec);

			if (!Directory.Exists (dir))
				yield break;

			var name = Path.GetFileNameWithoutExtension (expected.ItemSpec);
			var extension = Path.GetExtension (expected.ItemSpec);
			var nibDir = expected.GetMetadata ("LogicalName");
			var targets = GetTargetDevices (plist).ToList ();

			foreach (var path in Directory.GetFileSystemEntries (dir)) {
				// check that the FileNameWithoutExtension matches *exactly*
				if (string.Compare (path, dir.Length + 1, name, 0, name.Length) != 0)
					continue;

				int startIndex = dir.Length + 1 + name.Length;

				// match against files that have a "~" + $target (iphone, ipad, etc)
				if (path.Length > startIndex && path[startIndex] == '~') {
					bool matched = false;

					startIndex++;

					foreach (var target in targets) {
						// Note: we match the target case-insensitively because of https://bugzilla.xamarin.com/show_bug.cgi?id=44811
						if (string.Compare (path, startIndex, target, 0, target.Length, StringComparison.OrdinalIgnoreCase) == 0) {
							startIndex += target.Length;
							matched = true;
							break;
						}
					}

					if (!matched)
						continue;
				}

				// at this point, all that should be left is the file/directory extension
				if (path.Length != startIndex + extension.Length || string.Compare (path, startIndex, extension, 0, extension.Length) != 0)
					continue;

				var fileName = Path.GetFileName (path);
				var logicalName = !string.IsNullOrEmpty (nibDir) ? Path.Combine (nibDir, fileName) : fileName;
				var item = new TaskItem (path);
				expected.CopyMetadataTo (item);
				item.SetMetadata ("LogicalName", logicalName);

				yield return item;
			}

			yield break;
		}

		IEnumerable<ITaskItem> GetCompiledBundleResources (ITaskItem output)
		{
			if (IsWatchApp && !UseCompilationDirectory) {
				var logicalName = output.GetMetadata ("LogicalName");

				foreach (var extension in WatchAppExtensions) {
					var path = GetPathWithoutExtension (output.ItemSpec) + extension;
					if (File.Exists (path)) {
						var item = new TaskItem (path);
						item.SetMetadata ("LogicalName", GetPathWithoutExtension (logicalName) + extension);
						item.SetMetadata ("Optimize", "false");
						yield return item;
					}
				}
			} else if (Directory.Exists (output.ItemSpec)) {
				// Note: historically, only storyboard files compiled to directories containing the real nib files, but the new iOS 8 .xib's do as well.
				foreach (var file in RecursivelyEnumerateFiles (output))
					yield return file;

				yield break;
			}

			yield return output;
		}

		static bool ManifestExists (string path)
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

		public override bool Execute ()
		{
			Log.LogTaskName ("IBTool");
			Log.LogTaskProperty ("AppManifest", AppManifest);
			Log.LogTaskProperty ("InterfaceDefinitions", InterfaceDefinitions);
			Log.LogTaskProperty ("IntermediateOutputPath", IntermediateOutputPath);
			Log.LogTaskProperty ("IsWatchApp", IsWatchApp);
			Log.LogTaskProperty ("IsWatch2App", IsWatch2App);
			Log.LogTaskProperty ("IsAppExtension", IsAppExtension);
			Log.LogTaskProperty ("ProjectDir", ProjectDir);
			Log.LogTaskProperty ("ResourcePrefix", ResourcePrefix);
			Log.LogTaskProperty ("SdkBinPath", SdkBinPath);
			Log.LogTaskProperty ("SdkPlatform", SdkPlatform);
			Log.LogTaskProperty ("SdkRoot", SdkRoot);
			Log.LogTaskProperty ("SdkVersion", SdkVersion);

			if (IsWatchApp && AppleSdkSettings.XcodeVersion < new Version (6, 2)) {
				Log.LogError ("Watch apps/extensions require Xcode 6.2 or later. The current Xcode version is {0}", AppleSdkSettings.XcodeVersion);

				return !Log.HasLoggedErrors;
			}

			var ibtoolManifestDir = Path.Combine (IntermediateOutputPath, ToolName + "-manifests");
			var ibtoolOutputDir = Path.Combine (IntermediateOutputPath, ToolName);
			var bundleResources = new List<ITaskItem> ();
			var outputManifests = new List<ITaskItem> ();
			var compiled = new List<ITaskItem> ();
			bool changed = false;

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
			}
				
			foreach (var item in InterfaceDefinitions) {
				var bundleName = GetBundleRelativeOutputPath (item);
				var manifest = new TaskItem (Path.Combine (ibtoolManifestDir, bundleName));
				var manifestDir = Path.GetDirectoryName (manifest.ItemSpec);
				var resourceTags = item.GetMetadata ("ResourceTags");
				ITaskItem expected, output;
				string rpath, outputDir;

				if (!File.Exists (item.ItemSpec)) {
					Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, "The file '{0}' does not exist.", item.ItemSpec);
					continue;
				}

				rpath = Path.Combine (ibtoolOutputDir, bundleName);
				outputDir = Path.GetDirectoryName (rpath);
				expected = new TaskItem (rpath);

				expected.SetMetadata ("LogicalName", bundleName);
				expected.SetMetadata ("Optimize", "false");

				if (!string.IsNullOrEmpty (resourceTags))
					expected.SetMetadata ("ResourceTags", resourceTags);

				if (UseCompilationDirectory) {
					// Note: When using --compilation-directory, we need to specify the output path as the parent directory
					output = new TaskItem (expected);
					output.ItemSpec = Path.GetDirectoryName (output.ItemSpec);
					output.SetMetadata ("LogicalName", Path.GetDirectoryName (bundleName));
				} else {
					output = expected;
				}

				if (!ManifestExists (manifest.ItemSpec) || File.GetLastWriteTime (manifest.ItemSpec) < File.GetLastWriteTime (item.ItemSpec)) {
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
					Log.LogError ("Failed to load output manifest for {0}: {1}", ToolName, ex.Message);
					if (File.Exists (manifest.ItemSpec))
						Log.LogError ("Output manifest contents: {0}", File.ReadAllText (manifest.ItemSpec));
					continue;
				}

				if (UseCompilationDirectory)
					compiled.AddRange (GetCompilationDirectoryOutput (expected));
				else
					bundleResources.AddRange (GetCompiledBundleResources (output));

				outputManifests.Add (manifest);
			}

			if (InterfaceDefinitions.Length > 0 && UseCompilationDirectory) {
				var output = new TaskItem (ibtoolOutputDir);
				output.SetMetadata ("LogicalName", "");

				if (!CanLinkStoryboards)
					bundleResources.AddRange (GetCompiledBundleResources (output));
			}

			if (CanLinkStoryboards && compiled.Count > 0) {
				var linkOutputDir = Path.Combine (IntermediateOutputPath, ToolName + "-link");
				var manifest = new TaskItem (Path.Combine (ibtoolManifestDir, "link"));
				var output = new TaskItem (linkOutputDir);

				if (changed) {
					if (Directory.Exists (output.ItemSpec))
						Directory.Delete (output.ItemSpec, true);

					if (File.Exists (manifest.ItemSpec))
						File.Delete (manifest.ItemSpec);

					Directory.CreateDirectory (Path.GetDirectoryName (manifest.ItemSpec));
					Directory.CreateDirectory (output.ItemSpec);

					Link = true;

					if ((Compile (compiled.ToArray (), output, manifest)) != 0)
						return false;
				}

				output = new TaskItem (linkOutputDir);
				output.SetMetadata ("LogicalName", "");

				bundleResources.AddRange (GetCompiledBundleResources (output));
				outputManifests.Add (manifest);
			}

			BundleResources = bundleResources.ToArray ();
			OutputManifests = outputManifests.ToArray ();

			Log.LogTaskProperty ("BundleResources Output", BundleResources);
			Log.LogTaskProperty ("OutputManifests Output", OutputManifests);

			return !Log.HasLoggedErrors;
		}
	}
}
