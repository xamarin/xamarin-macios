using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin;
using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Localization.MSBuild;

#nullable enable

namespace Xamarin.MacDev.Tasks {

	public abstract class ResolveNativeReferencesBase : XamarinTask
	{
		#region Inputs

		[Required]
		public string Architectures { get; set; }

		public ITaskItem [] NativeReferences { get; set; }

		public ITaskItem[] References { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] NativeFrameworks { get; set; }

		#endregion

		public override bool Execute ()
		{
			var native_frameworks = new List<ITaskItem> ();

			// there can be direct native references inside a project
			if (NativeReferences != null) {
				foreach (var nr in NativeReferences) {
					var name = nr.ItemSpec;
					switch (Path.GetExtension (name)) {
					case ".xcframework":
						var resolved = ResolveXCFramework (name);
						if (resolved == null)
							return false;
						native_frameworks.Add (new TaskItem (resolved));
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
					var manifest = Path.Combine (resources, "manifest");
					if (!File.Exists (manifest))
						continue;

					XmlDocument document = new XmlDocument ();
					document.LoadWithoutNetworkAccess (manifest);
					foreach (XmlNode referenceNode in document.GetElementsByTagName ("NativeReference")) {
						ITaskItem t;
						var name = referenceNode.Attributes ["Name"].Value;
						switch (Path.GetExtension (name)) {
						case ".xcframework":
							var resolved = ResolveXCFramework (Path.Combine (resources, name));
							if (resolved == null)
								return false;
							t = new TaskItem (resolved);
							t.SetMetadata ("Kind", "Framework");
							t.SetMetadata ("Name", resolved);
							break;
						case ".framework":
							t = new TaskItem (Path.Combine (resources, name));
							t.SetMetadata ("Kind", "Framework");
							break;
						case ".dylib": // macOS
							t = new TaskItem (Path.Combine (resources, name));
							t.SetMetadata ("Kind", "Dynamic");
							break;
						default:
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
			}

			NativeFrameworks = native_frameworks.ToArray ();

			return !Log.HasLoggedErrors;
		}

		protected string ResolveXCFramework (string xcframework)
		{
			var platformName = PlatformFrameworkHelper.GetOperatingSystem (TargetFrameworkMoniker);
			// PlatformFrameworkHelper.GetOperatingSystem returns "osx" which does not work for xcframework
			if (platformName == "osx")
				platformName = "macos";

			var variant = SdkIsSimulator ? "simulator" : null;
			try {
				var plist = PDictionary.FromFile (Path.Combine (xcframework, "Info.plist"));
				var dir = ResolveXCFramework (plist, platformName, variant, Architectures);
				if (!String.IsNullOrEmpty (dir))
					return Path.Combine (xcframework, dir);

				// either the format was incorrect or we could not find a matching framework
				// note: last part is not translated since it match the (non-translated) keys inside the `Info.plist`
				var msg = (dir == null) ? MSBStrings.E0174 : MSBStrings.E0175 + $" SupportedPlatform: '{platformName}', SupportedPlatformVariant: '{variant}', SupportedArchitectures: '{Architectures}'.";
				Log.LogError (msg, xcframework);
			}
			catch (Exception) {
				Log.LogError (MSBStrings.E0174, xcframework);
			}
			return null;
		}

		internal static string ResolveXCFramework (PDictionary plist, string platformName, string variant, string architectures)
		{
			// plist structure https://github.com/spouliot/xcframework#infoplist
			var bundle_package_type = (PString) plist ["CFBundlePackageType"];
			if (bundle_package_type?.Value != "XFWK")
				return null;
			var available_libraries = plist.GetArray ("AvailableLibraries");
			if ((available_libraries == null) || (available_libraries.Count == 0))
				return null;

			var platform = platformName.ToLowerInvariant ();
			var archs = architectures.Split (new char [] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (PDictionary item in available_libraries) {
				var supported_platform = (PString) item ["SupportedPlatform"];
				if (supported_platform.Value != platform)
					continue;
				// optional key
				var supported_platform_variant = (PString) item ["SupportedPlatformVariant"];
				if (supported_platform_variant?.Value != variant)
					continue;
				var supported_architectures = (PArray) item ["SupportedArchitectures"];
				// each architecture we request must be present in the xcframework
				// but extra architectures in the xcframework are perfectly fine
				foreach (var arch in archs) {
					bool found = false;
					foreach (PString xarch in supported_architectures) {
						found = String.Equals (arch, xarch.Value, StringComparison.OrdinalIgnoreCase);
						if (found)
							break;
					}
					if (!found)
						return String.Empty;
				}
				var library_path = (PString) item ["LibraryPath"];
				var library_identifier = (PString) item ["LibraryIdentifier"];
				return Path.Combine (library_identifier, library_path);
			}
			return String.Empty;
		}
	}
}
