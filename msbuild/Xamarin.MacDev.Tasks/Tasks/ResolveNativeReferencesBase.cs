using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin;
using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {

	public class ResolveNativeReferences : XamarinTask, ITaskCallback {
		#region Inputs

		[Required]
		public string? Architectures { get; set; }

		[Required]
		public string? IntermediateOutputPath { get; set; }

		public ITaskItem []? NativeReferences { get; set; }

		public ITaskItem []? References { get; set; }

		public ITaskItem []? BindingResourcePackages { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem []? NativeFrameworks { get; set; }

		#endregion

		// returns the Mach-O file for the given path:
		// * for frameworks, returns foo.framework/foo
		// * for anything else, returns the input path
		static string? GetActualLibrary (string? path)
		{
			if (path is null)
				return null;

			if (path.EndsWith (".framework", StringComparison.OrdinalIgnoreCase))
				return Path.Combine (path, Path.GetFileNameWithoutExtension (path));

			return path;
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return ExecuteLocally ();
		}

		bool ExecuteLocally ()
		{
			var native_frameworks = new List<ITaskItem> ();

			// there can be direct native references inside a project
			if (NativeReferences != null) {
				foreach (var nr in NativeReferences) {
					var name = nr.ItemSpec;
					switch (Path.GetExtension (name)) {
					// '.' can be used to represent a file (instead of the directory)
					case "":
						name = Path.GetDirectoryName (name);
						if (Path.GetExtension (name) == ".xcframework")
							goto case ".xcframework";
						break;
					case ".xcframework":
						var resolved = ResolveXCFramework (name);
						if (resolved == null)
							return false;
						var t = new TaskItem (resolved);

						// add metadata from the original item
						nr.CopyMetadataTo (t);

						t.SetMetadata ("Kind", "Framework");
						t.SetMetadata ("Name", resolved);

						native_frameworks.Add (t);
						break;
					case ".framework":
						native_frameworks.Add (nr);
						break;
					}
				}
			}
			// or (managed) reference to an assembly that bind a framework
			if (References != null) {
				foreach (var r in References) {
					// look for sidecar's manifest
					var resources = Path.ChangeExtension (r.ItemSpec, ".resources");
					if (Directory.Exists (resources)) {
						ProcessSidecar (r, resources, native_frameworks);
					} else {
						resources = resources + ".zip";
						if (File.Exists (resources))
							ProcessSidecar (r, resources, native_frameworks);
					}
				}
			}

			// or even just plain binding packages
			if (BindingResourcePackages is not null) {
				foreach (var bp in BindingResourcePackages) {
					ProcessSidecar (bp, bp.ItemSpec, native_frameworks);
				}
			}

			NativeFrameworks = native_frameworks.ToArray ();

			return !Log.HasLoggedErrors;
		}

		void ProcessSidecar (ITaskItem r, string resources, List<ITaskItem> native_frameworks)
		{
			// Check if we have a zipped sidecar, and if so, extract it before we keep processing
			if (resources.EndsWith (".zip", StringComparison.OrdinalIgnoreCase)) {
				var path = Path.Combine (IntermediateOutputPath, Path.GetFileName (resources));
				var arguments = new [] {
							"-u",
							"-o",
							"-d",
							path,
							resources,
						};
				ExecuteAsync ("/usr/bin/unzip", arguments).Wait ();
				resources = path;
			}

			if (!Directory.Exists (resources)) {
				Log.LogWarning (MSBStrings.W7093 /* The binding resource package {0} does not exist. */, resources);
				return;
			}

			var manifest = Path.Combine (resources, "manifest");
			if (!File.Exists (manifest)) {
				Log.LogWarning (MSBStrings.W7087 /* Expected a 'manifest' file in the directory {0} */, resources);
				return;
			}

			XmlDocument document = new XmlDocument ();
			document.LoadWithoutNetworkAccess (manifest);
			foreach (XmlNode referenceNode in document.GetElementsByTagName ("NativeReference")) {
				ITaskItem t;
				var name = referenceNode.Attributes ["Name"].Value;
				switch (Path.GetExtension (name)) {
				case ".xcframework":
					var resolved = ResolveXCFramework (Path.Combine (resources, name));
					if (resolved == null)
						return;
					t = new TaskItem (resolved);
					t.SetMetadata ("Kind", "Framework");
					t.SetMetadata ("Name", resolved);
					break;
				case ".framework":
					t = new TaskItem (Path.Combine (resources, name, Path.GetFileNameWithoutExtension (name)));
					t.SetMetadata ("Kind", "Framework");
					break;
				case ".dylib": // macOS
					t = new TaskItem (Path.Combine (resources, name));
					t.SetMetadata ("Kind", "Dynamic");
					break;
				case ".a": // static library
					t = new TaskItem (Path.Combine (resources, name));
					t.SetMetadata ("Kind", "Static");
					break;
				default:
					Log.LogWarning (MSBStrings.W7105 /* Unexpected extension '{0}' for native reference '{1}' in manifest '{2}'. */, Path.GetExtension (name), name, manifest);
					t = r;
					break;
				}

				// defaults
				t.SetMetadata ("ForceLoad", "False");
				t.SetMetadata ("NeedsGccExceptionHandling", "False");
				t.SetMetadata ("IsCxx", "False");
				t.SetMetadata ("SmartLink", "True");

				// values from manifest, overriding defaults if provided
				foreach (XmlNode attribute in referenceNode.ChildNodes)
					t.SetMetadata (attribute.Name, attribute.InnerText);

				native_frameworks.Add (t);
			}
		}

		protected string? ResolveXCFramework (string xcframework)
		{
			string platformName;

			switch (Platform) {
			case Utils.ApplePlatform.MacCatalyst:
				platformName = "ios";
				break;
			case Utils.ApplePlatform.MacOSX:
				// PlatformFrameworkHelper.GetOperatingSystem returns "osx" which does not work for xcframework
				platformName = "macos";
				break;
			default:
				platformName = PlatformFrameworkHelper.GetOperatingSystem (TargetFrameworkMoniker);
				break;
			}

			string? variant = null;
			if (Platform == Utils.ApplePlatform.MacCatalyst) {
				variant = "maccatalyst";
			} else if (SdkIsSimulator) {
				variant = "simulator";
			}

			try {
				var plist = PDictionary.FromFile (Path.Combine (xcframework, "Info.plist"))!;
				var path = ResolveXCFramework (plist, platformName, variant, Architectures!);
				if (!String.IsNullOrEmpty (path))
					return Path.Combine (xcframework, path);

				// either the format was incorrect or we could not find a matching framework
				// note: last part is not translated since it match the (non-translated) keys inside the `Info.plist`
				var msg = (path == null) ? MSBStrings.E0174 : MSBStrings.E0175 + $" SupportedPlatform: '{platformName}', SupportedPlatformVariant: '{variant}', SupportedArchitectures: '{Architectures}'.";
				Log.LogError (msg, xcframework);
			} catch (Exception) {
				Log.LogError (MSBStrings.E0174, xcframework);
			}
			return null;
		}

		internal static string? ResolveXCFramework (PDictionary plist, string platformName, string? variant, string architectures)
		{
			// plist structure https://github.com/spouliot/xcframework#infoplist
			var bundle_package_type = (PString?) plist ["CFBundlePackageType"];
			if (bundle_package_type?.Value != "XFWK")
				return null;
			var available_libraries = plist.GetArray ("AvailableLibraries");
			if ((available_libraries == null) || (available_libraries.Count == 0))
				return null;

			var platform = platformName.ToLowerInvariant ();
			var archs = architectures.Split (new char [] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (PDictionary item in available_libraries) {
				var supported_platform = (PString?) item ["SupportedPlatform"];
				if (supported_platform?.Value != platform)
					continue;
				// optional key
				var supported_platform_variant = (PString?) item ["SupportedPlatformVariant"];
				if (supported_platform_variant?.Value != variant)
					continue;
				var supported_architectures = (PArray?) item ["SupportedArchitectures"];
				// each architecture we request must be present in the xcframework
				// but extra architectures in the xcframework are perfectly fine
				foreach (var arch in archs) {
					bool found = false;
					foreach (PString xarch in supported_architectures!) {
						found = String.Equals (arch, xarch.Value, StringComparison.OrdinalIgnoreCase);
						if (found)
							break;
					}
					if (!found)
						return String.Empty;
				}
				var library_path = (PString?) item ["LibraryPath"];
				var library_identifier = (PString?) item ["LibraryIdentifier"];
				return GetActualLibrary (Path.Combine (library_identifier!, library_path!));
			}
			return String.Empty;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item)
		{
			// Don't copy any files to Windows, because
			// 1. They're not used in Inputs/Outputs, so the lack of them won't affect anything
			// 2. They may be directories, and as such we'd have to expand them to (potentially numerous and large) files to copy them (uselessly) to Windows.
			// 3. They may contain symlinks, which may not work correctly on Windows.
			return false;
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
